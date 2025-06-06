﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.NubefactIntegration.Modelos;
using System.Text.RegularExpressions;


namespace SAMBHS.Windows.NubefactIntegration
{
    /// <summary>
    /// Consume los servicios del API de Nubefact.
    /// EQC 24-6-2018
    /// </summary>
    public class NubeFacTManager
    {
        public static bool CheckConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("https://www.nubefact.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        #region Propiedades
        /// <summary>
        /// Indica la ruta proporcionada por NubeFacT
        /// </summary>
        public string Ruta { get; set; }
        /// <summary>
        /// Indica el Token proporcionado por NubeFacT
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Especifica la acción a tomar con el documento seleccionado.
        /// </summary>
        public TipoAccion TipoAccionRealizar { get; set; }
        /// <summary>
        /// Especifica el formato del documento a generar.
        /// </summary>
        public string FormatoImpresion { get; set; }
        /// <summary>
        /// Especifica si se desea enviar a la SUNAT al momento de enviarlo a Nubefact.
        /// Si el valor es falso, debe enviarlo a SUNAT desde el portal de Nubefact.
        /// </summary>
        public bool EnviarAutomaticamente { get; set; }
        /// <summary>
        /// Estado con el que resulta la venta al terminar el proceso
        /// </summary>
        private EstadoSunat EstadoSunatResult { get; set; }
        #endregion

        #region Variables privadas
        private bool _erroresOcurridos;
        private string _motivoBaja;
        #endregion

        #region Eventos
        public delegate void Terminado(IRespuesta rpt, int rowIndex, EstadoSunat estadoSunat);
        public delegate void Error(Exception ex, int rowIndex, TipoAccion action);
        public delegate void Estado(string msg, int rowIndex);

        public event Terminado TerminadoEvent;
        public event Error ErrorEvent;
        public event Estado EstadoEvent;
        #endregion

        public NubeFacTManager()
        {
            EstadoSunatResult = EstadoSunat.PENDIENTE;
            EnviarAutomaticamente = false;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
        }

        /// <summary>
        /// Inicia el proceso de envio y recepcion de los cpe con nubefact y sunat.
        /// </summary>
        /// <param name="pstrIdVenta">Id de la venta a procesar</param>
        /// <param name="rowIndex">Inidce de la fila en la que se mostraran los estados</param>
        /// <param name="motivoBaja">Motivo de la baja en caso se requiera anular un cpe</param>
        public void Comenzar(string pstrIdVenta, int rowIndex = -1, string motivoBaja = null)
        {
            try
            {
                var bw = new BackgroundWorker();
                bw.RunWorkerCompleted += _bw_RunWorkerCompleted;
                bw.DoWork += _bw_DoWork;
                _motivoBaja = motivoBaja;
                bw.RunWorkerAsync(new Tuple<string, int>(pstrIdVenta, rowIndex));
            }
            catch (Exception e)
            {
                _erroresOcurridos = true;
                if (ErrorEvent != null) ErrorEvent(e, rowIndex, TipoAccionRealizar);
            }
        }

        private void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var parm = (Tuple<string, int>)e.Argument;
            try
            {
                var rpt = RealizarSolicitud(parm.Item1, parm.Item2);
                if (rpt != null)
                {
                    ConsumirRespuesta(rpt, parm.Item1, TipoAccionRealizar, parm.Item2);
                    e.Result = new Tuple<IRespuesta, int>(rpt, parm.Item2);
                }
            }
            catch (Exception exception)
            {
                _erroresOcurridos = true;
                if (ErrorEvent != null) ErrorEvent(exception, parm.Item2, TipoAccionRealizar);
            }
        }

        private void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null) return;
            var result = (Tuple<IRespuesta, int>)e.Result;
            if (TerminadoEvent != null && !_erroresOcurridos)
            {
                TerminadoEvent(result.Item1, result.Item2, EstadoSunatResult);
            }
        }

        /// <summary>
        /// Arma el archivo json segun el tipo de solicitud y la envia al api de nubefact.
        /// Retorna la respuesta del api
        /// En caso ocurra una excepcion no retorna nada y dispara el evento de error.
        /// </summary>
        /// <param name="pstrIdVenta"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        private IRespuesta RealizarSolicitud(string pstrIdVenta, int rowIndex)
        {
            try
            {
                var json = string.Empty;
                if (EstadoEvent != null) EstadoEvent("Preparando...", rowIndex);

                #region Crea el request en formato json para enviarlo al api
                switch (TipoAccionRealizar)
                {
                    case TipoAccion.EnviarComprobante:
                        var invoice = ObtenerEnvioInvoiceRequest(pstrIdVenta, rowIndex);
                        if (invoice != null) json = invoice.ToJson();
                        break;

                    case TipoAccion.ConsultarComprobante:
                        var reqConsultaCpe = ObtenConsultaEstadoRequest(pstrIdVenta, rowIndex);
                        if (reqConsultaCpe != null) json = reqConsultaCpe.ToJson();
                        break;

                    case TipoAccion.DarBajaComprobante:
                        if (!string.IsNullOrWhiteSpace(_motivoBaja))
                        {
                            var reqBaja = ObtenerBajaRequest(pstrIdVenta, rowIndex);
                            if (reqBaja != null) json = reqBaja.ToJson();
                        }
                        else throw new Exception("Por favor especifique el motivo de la baja.");

                        break;

                    case TipoAccion.ConsultarBajaComprobante:
                        goto case TipoAccion.ConsultarComprobante;

                    default:
                        throw new NotImplementedException("Caso de envío no soportado.");
                }
                #endregion

                if (!string.IsNullOrEmpty(json))
                {
                    #region Realiza la consulta al api de nubefact y recibe la respuesta

                    if (EstadoEvent != null) EstadoEvent("Enviando...", rowIndex);

                    var bytes = Encoding.Default.GetBytes(json);
                    var jsonEnUtf8 = Encoding.UTF8.GetString(bytes);
                    var jsonDeRespuesta = SendJson(Ruta, jsonEnUtf8, Token, rowIndex);
                    if (string.IsNullOrWhiteSpace(jsonDeRespuesta))
                        throw new Exception("No se pudo constituir el archivo Json");
                    if (!jsonDeRespuesta.Contains("errors"))
                        return RespuestaEnvio.FromJson(jsonDeRespuesta);

                    EstadoSunatResult = EstadoSunat.ERROR_RECEPCION_ENVIO;
                    return RespuestaError.FromJson(jsonDeRespuesta);

                    #endregion
                }

                throw new Exception("No se pudo constituir el archivo Json");
            }
            catch (Exception e)
            {
                _erroresOcurridos = true;
                if (ErrorEvent != null) ErrorEvent(e, rowIndex, TipoAccionRealizar);
                return null;
            }
        }

        private string SendJson(string ruta, string json, string token, int rowIndex)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json; charset=utf-8";
                    client.Headers[HttpRequestHeader.Authorization] = "Token token=" + token;
                    var respuesta = client.UploadString(ruta, "POST", json);
                    return respuesta;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    EstadoSunatResult = EstadoSunat.ERROR_RECEPCION_ENVIO;
                    var respuesta = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    return respuesta;
                }
                if (EstadoEvent != null) EstadoEvent(ex.Message, rowIndex);
                throw;
            }
        }

        public enum TipoAccion
        {
            EnviarComprobante,
            ConsultarComprobante,
            DarBajaComprobante,
            ConsultarBajaComprobante
        }

        #region Genera los requests para el API de Nubefact
        private InvoiceComprobante ObtenerEnvioInvoiceRequest(string pstrIdVenta, int rowIndex)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var v = dbContext.venta.FirstOrDefault(p => p.v_IdVenta.Equals(pstrIdVenta));
                    if (v == null) return null;

                    var cobdet = dbContext.cobranzadetalle.FirstOrDefault(p => p.v_IdVenta.Equals(pstrIdVenta) && p.i_Eliminado == 0);
                    string condicionPago = "";
                    if (cobdet == null)
                    {
                        condicionPago = "Pago con Fecha Tope: " + GetDateFormatted(v.t_FechaVencimiento ?? DateTime.Today);
                        //return null;
                    }
                    else
                    {
                        var cobdetsys = dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 46 && p.i_ItemId == (cobdet.i_IdFormaPago ?? 1));

                        condicionPago = (cobdetsys != null ? cobdetsys.v_Value1 : "EFECTIVO SOLES" + " - " + cobdet.v_DocumentoRef).Trim();

                    }

                    var c = dbContext.cliente.FirstOrDefault(p => p.v_IdCliente.Equals(v.v_IdCliente));
                    string correo = "";
                    String sFormato = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
                    if (Regex.IsMatch(c.v_Correo, sFormato))
                    {
                        if (Regex.Replace(c.v_Correo.Trim(), sFormato, String.Empty).Length == 0)
                        {
                            correo = c.v_Correo;
                        }
                        else
                        {
                            correo = "";
                        }
                    }
                    else
                    {
                        correo = "";
                    }


                    if (c == null) return null;
                    var det = dbContext.ventadetalle.Where(p => p.v_IdVenta.Equals(pstrIdVenta) && p.i_Eliminado == 0).ToList();
                    if (!det.Any()) return null;

                    var esNcr = (v.i_IdTipoDocumento ?? 0) == 7 || (v.i_IdTipoDocumento ?? 0) == 8;
                    var cp = dbContext.datahierarchy.FirstOrDefault(p => p.i_GroupId == 23 && p.i_ItemId == (v.i_IdCondicionPago ?? 1));
                    var condicionP = cp == null ? "" : cp.v_Value1 == "CRÉDITO" ? "CREDITO" : cp.v_Value1;
                    var tipoCpe = DevuelveTipoCpe(v.i_IdTipoDocumento ?? 0);
                    if (tipoCpe.Equals("-1")) throw new Exception("Tipo Documento de venta no soportado");
                    decimal totalsindetracción = v.d_Total ?? 0;
                    double detraccion = 0.12;
                    if (v.d_Total > 700)
                    {
                        totalsindetracción = totalsindetracción - (totalsindetracción * decimal.Parse(detraccion.ToString()));
                    }
                    string simboloMoneda = "S/.";
                    if (v.i_IdMoneda != 1)
                    {
                        simboloMoneda = "$";
                    }

                    string mensajeAdicional = @"EL PAGO REALIZADO SERA VIGENTE POR 7 DIAS, PASADO ESTE PLAZO NO HABRA OPCION A REEMBOLSO.  
                                                UNA VEZ EMITIDO EL COMPROBANTE DE PAGO, NO SE ACEPTARA EL CAMBIO POR OTRO TIPO DE COMPROBANTE (FACTURA/BOLETA), NI POR CAMBIO DE NOMBRE O RAZAN SOCIAL.  
                                                ¡ANTES DE RETIRARSE, POR FAVOR VERIFIQUE SU COMPROBANTE, NO SE ACEPTARA CAMBIOS NI DEVOLUCIONES!";
                    var header = new InvoiceComprobante
                    {
                        Operacion = "generar_comprobante",
                        TipoDeComprobante = tipoCpe,
                        Serie = v.v_SerieDocumento.Trim(),
                        Numero = long.Parse(v.v_CorrelativoDocumento),
                        SunatTransaction = 1,
                        ClienteTipoDeDocumento = c.v_IdCliente.Equals("N002-CL000000000") ? "-" : (c.i_IdTipoIdentificacion ?? 1).ToString(),
                        ClienteNumeroDeDocumento = c.v_NroDocIdentificacion,
                        ClienteDenominacion = (c.v_RazonSocial + " " + c.v_ApePaterno + " " + c.v_ApeMaterno + " " + c.v_PrimerNombre).Trim(),
                        ClienteDireccion = c.v_DirecPrincipal,
                        ClienteEmail = correo ?? "",
                        ClienteEmail1 = "",
                        ClienteEmail2 = "",
                        FechaDeEmision = GetDateFormatted(v.t_FechaRegistro ?? DateTime.Today),
                        FechaDeVencimiento = GetDateFormatted(v.t_FechaVencimiento ?? DateTime.Today),
                        Moneda = v.i_IdMoneda ?? 1,
                        TipoDeCambio = (v.d_TipoCambio ?? 3).ToString("F"),
                        PorcentajeDeIgv = "18.00",
                        DescuentoGlobal = (v.d_Descuento ?? 0).ToString("F"),
                        TotalDescuento = (v.d_Descuento ?? 0).ToString("F"),
                        TotalAnticipo = (v.d_Anticipio ?? 0).ToString("F"),
                        TotalGravada = det.Where(p => p.i_IdTipoOperacion.ToString().StartsWith("1")).Sum(s => s.d_ValorVenta ?? 0).ToString("F"),
                        TotalInafecta = det.Where(p => p.i_IdTipoOperacion.ToString().StartsWith("3")).Sum(s => s.d_ValorVenta ?? 0).ToString("F"),
                        TotalExonerada = det.Where(p => p.i_IdTipoOperacion.ToString().StartsWith("2")).Sum(s => s.d_ValorVenta ?? 0).ToString("F"),
                        TotalIgv = (v.d_IGV ?? 0).ToString("F"),
                        TotalGratuita = "0.00",
                        TotalOtrosCargos = ((v.d_total_otrostributos ?? 0) + (v.d_total_isc ?? 0)).ToString("F"),
                        Total = (v.d_Total ?? 0).ToString("F"),
                        PercepcionTipo = "",
                        PercepcionBaseImponible = "",
                        TotalPercepcion = "",
                        TotalIncluidoPercepcion = "",
                        Detraccion = (v.i_AfectaDetraccion ?? 0) == 1 ? "verdadero" : "falso",
                        //Observaciones = v.v_Concepto.ToUpper(),
                        Observaciones = "CONDICION DE PAGO: " + condicionP + ", CUOTA 1: MONTO NETO: " + simboloMoneda + " " + totalsindetracción + " / IMPORTANTE: " + mensajeAdicional,
                        DocumentoQueSeModificaTipo = esNcr ? DevuelveTipoCpe(v.i_IdTipoDocumentoRef ?? 0) : "",
                        DocumentoQueSeModificaSerie = esNcr ? v.v_SerieDocumentoRef.Trim().ToUpper() : "",
                        DocumentoQueSeModificaNumero = esNcr ? long.Parse(v.v_CorrelativoDocumentoRef).ToString().Trim() : "",
                        TipoDeNotaDeCredito = v.i_IdTipoNota.HasValue ? v.i_IdTipoNota > 0 ? v.i_IdTipoNota.ToString() : "" : "",
                        TipoDeNotaDeDebito = v.i_IdTipoNota.HasValue ? v.i_IdTipoNota > 0 ? v.i_IdTipoNota.ToString() : "" : "",
                        EnviarAutomaticamenteALaSunat = EnviarAutomaticamente ? "verdadero" : "falso",
                        EnviarAutomaticamenteAlCliente = string.IsNullOrWhiteSpace(correo) ? "falso" : "verdadero",
                        CodigoUnico = v.v_Periodo + v.v_Mes + v.v_Correlativo,
                        CondicionesDePago = condicionP,
                        MedioDePago = condicionP == "CREDITO" ? "credito" : condicionPago,
                        PlacaVehiculo = v.v_PlacaVehiculo,
                        OrdenCompraServicio = v.v_OrdenCompra,
                        TablaPersonalizadaCodigo = "",
                        FormatoDePdf = FormatoImpresion
                    };
                    List<VentaAlCredito> VentaAlCreditoLista = new List<VentaAlCredito>();
                    if (header.MedioDePago == "credito")
                    {
                        VentaAlCredito _VentaAlCredito = new VentaAlCredito();
                        _VentaAlCredito.Cuota = "1";
                        _VentaAlCredito.FechaDePago = header.FechaDeVencimiento;
                        _VentaAlCredito.Importe = totalsindetracción.ToString();
                        VentaAlCreditoLista.Add(_VentaAlCredito);
                    }
                    header.VentaAlCredito = VentaAlCreditoLista.ToArray();
                    var details = det.Select(d =>
                    {
                        var prod = dbContext.productodetalle.FirstOrDefault(p => p.v_IdProductoDetalle.Equals(d.v_IdProductoDetalle));
                        if (prod == null) return null;
                        var prodInfo = new { prod.producto.v_CodInterno, EServicio = (prod.producto.i_EsServicio ?? 0) == 1 };
                        return new Item
                        {
                            UnidadDeMedida = prodInfo.EServicio ? "ZZ" : "NIU",
                            Codigo = prodInfo.v_CodInterno,
                            Descripcion = d.v_DescripcionProducto,
                            Cantidad = (d.d_Cantidad ?? 0).ToString("F"),
                            ValorUnitario = (d.d_Valor / d.d_Cantidad).ToString(),
                            PrecioUnitario = (((d.d_Valor ?? 0) / (d.d_Cantidad ?? 0)) + ((d.d_Igv ?? 0) / (d.d_Cantidad ?? 0))).ToString("F"), // (d.d_Precio ?? 0).ToString("F"),
                            Descuento = (d.d_Descuento ?? 0).ToString("F"),
                            Subtotal = (d.d_ValorVenta ?? 0).ToString("F"),
                            TipoDeIgv = DevuelveTipoOperacion(d.i_IdTipoOperacion ?? 1),
                            Igv = (d.d_Igv ?? 0).ToString("F"),
                            Total = (d.d_PrecioVenta ?? 0).ToString("F"),
                            AnticipoRegularizacion = "false",
                            AnticipoDocumentoSerie = "",
                            AnticipoDocumentoNumero = v.v_IdDocAnticipo ?? "",

                        };
                    }).ToArray();

                    header.Items = details;


                    if (!string.IsNullOrWhiteSpace(v.v_NroGuiaRemisionCorrelativo) && !string.IsNullOrWhiteSpace(v.v_NroGuiaRemisionSerie))
                    {
                        var guiasRemision = v.v_NroGuiaRemisionCorrelativo.Split(',');
                        var guias = guiasRemision.Select(g => new Guia
                        {
                            GuiaSerieNumero = v.v_NroGuiaRemisionSerie.Trim() + "-" + long.Parse(g).ToString(),
                            GuiaTipo = 1 //Remitente
                        }).ToArray();

                        header.Guias = guias;
                    }
                    return header;
                }
            }
            catch (Exception e)
            {
                _erroresOcurridos = true;
                if (ErrorEvent != null) ErrorEvent(e, rowIndex, TipoAccionRealizar);
                return null;
            }
        }

        private GeneraAnulacion ObtenerBajaRequest(string pstrIdVenta, int rowIndex)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var v = dbContext.venta.FirstOrDefault(p => p.v_IdVenta.Equals(pstrIdVenta));
                    if (v == null) return null;

                    var req = new GeneraAnulacion
                    {
                        Operacion = "generar_anulacion",
                        Motivo = _motivoBaja.Trim().ToUpper(),
                        Serie = v.v_SerieDocumento.Trim(),
                        Numero = long.Parse(v.v_CorrelativoDocumento).ToString(),
                        TipoDeComprobante = DevuelveTipoCpe(v.i_IdTipoDocumento ?? 1),
                        CodigoUnico = ""
                    };

                    return req;
                }
            }
            catch (Exception e)
            {
                _erroresOcurridos = true;
                if (ErrorEvent != null) ErrorEvent(e, rowIndex, TipoAccionRealizar);
                return null;
            }
        }

        private ConsultaEstado ObtenConsultaEstadoRequest(string pstrIdVenta, int rowIndex)
        {
            try
            {
                if (TipoAccionRealizar != TipoAccion.ConsultarComprobante &&
                    TipoAccionRealizar != TipoAccion.ConsultarBajaComprobante)
                    return null;

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var v = dbContext.venta.FirstOrDefault(p => p.v_IdVenta.Equals(pstrIdVenta));
                    if (v == null) return null;

                    var req = new ConsultaEstado
                    {
                        Operacion = TipoAccionRealizar == TipoAccion.ConsultarComprobante ? "consultar_comprobante" : "consultar_anulacion",
                        Serie = v.v_SerieDocumento.Trim(),
                        Numero = long.Parse(v.v_CorrelativoDocumento).ToString(),
                        TipoDeComprobante = DevuelveTipoCpe(v.i_IdTipoDocumento ?? 1),
                    };

                    return req;
                }
            }
            catch (Exception e)
            {
                _erroresOcurridos = true;
                if (ErrorEvent != null) ErrorEvent(e, rowIndex, TipoAccionRealizar);
                return null;
            }
        }

        #endregion

        #region Equivalencias tablas contasol-nubefact.
        /// <summary>
        /// Equivalencias de tipo de CPE de acuerdo al MANUAL DE INTEGRACIÓN ARCHIVO .JSON ver 1.1
        /// </summary>
        /// <param name="i">Id del CPE según Contasol.</param>
        /// <returns></returns>
        private static string DevuelveTipoCpe(int i)
        {
            switch (i)
            {
                case 1: return "1";
                case 3: return "2";
                case 7: return "3";
                case 8: return "4";
                default: return "-1";
            }
        }

        /// <summary>
        /// Equivalencias del tipo de Operación segun el MANUAL DE INTEGRACIÓN ARCHIVO .JSON ver 1.1
        /// </summary>
        /// <param name="i">Tipo de Operacion del detalle de la venta.</param>
        /// <returns></returns>
        private static string DevuelveTipoOperacion(int i)
        {
            switch (i)
            {
                case 1: return "1";
                case 2: return "8";
                case 3: return "9";
                case 4: return "16";
                case 11: return "2";
                case 12: return "3";
                case 13: return "4";
                case 14: return "5";
                case 15: return "6";
                case 16: return "7";
                case 17: return "4";
                case 21: return "8";
                case 31: return "10";
                case 32: return "11";
                case 33: return "12";
                case 34: return "13";
                case 35: return "14";
                case 36: return "15";
                default: return "-1";
            }
        }
        #endregion

        /// <summary>
        /// Procesa la respuesta del api de nubefact y guarda los cambios en la bd
        /// </summary>
        /// <param name="rpt"></param>
        /// <param name="pstrIdVenta"></param>
        /// <param name="tipoAccion"></param>
        /// <param name="rowIndex"></param>
        private void ConsumirRespuesta(IRespuesta rpt, string pstrIdVenta, TipoAccion tipoAccion, int rowIndex)
        {
            try
            {
                if (EstadoEvent != null) EstadoEvent("Recibiendo...", rowIndex);

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var vta = dbContext.venta.FirstOrDefault(p => p.v_IdVenta.Equals(pstrIdVenta));
                    if (vta == null) return;
                    EstadoSunatResult = (EstadoSunat)(vta.i_EstadoSunat ?? 1);
                    if (rpt is RespuestaEnvio)
                    {
                        var r = (RespuestaEnvio)rpt;

                        if (tipoAccion == TipoAccion.EnviarComprobante)
                        {
                            #region Manejo de respuesta de envio de cpe
                            EstadoSunatResult = r.AceptadaPorSunat ? EstadoSunat.CDR_ACEPTADO : EstadoSunat.ENVIADO_POR_CONSULTAR_ESTADO;

                            if (string.IsNullOrEmpty(r.CodigoHash))
                                EstadoSunatResult = EstadoSunat.ERROR_RECEPCION_ENVIO;

                            if (!string.IsNullOrEmpty(r.SunatResponsecode))
                            {
                                int codSunat;
                                if (int.TryParse(r.SunatResponsecode, out codSunat))
                                {
                                    #region Excepciones
                                    if (codSunat >= 100 && codSunat <= 1999)
                                        EstadoSunatResult = EstadoSunat.ERROR_RECEPCION_ENVIO;
                                    #endregion

                                    #region Rechazos
                                    if (codSunat >= 2000 && codSunat <= 3999)
                                        EstadoSunatResult = EstadoSunat.CDR_RECHAZADO;
                                    #endregion

                                    #region Observados
                                    if (codSunat > 4000)
                                        EstadoSunatResult = EstadoSunat.CDR_ACEPTADO_CON_OBSERV;
                                    #endregion
                                }
                            }

                            vta.i_EstadoSunat = (short)EstadoSunatResult;
                            vta.v_SunatResponseCode = r.SunatResponsecode;
                            vta.v_CadenaCodigoQr = r.CadenaParaCodigoQr;
                            vta.v_CodigoBarras = r.CodigoDeBarras;
                            vta.v_Hash = r.CodigoHash;
                            vta.v_EnlaceEnvio = r.Enlace;
                            vta.v_KeySunat = r.Key;
                            dbContext.venta.ApplyCurrentValues(vta);
                            #endregion
                        }

                        if (tipoAccion == TipoAccion.DarBajaComprobante)
                        {
                            #region Manejo de respuesta de dada de baja

                            if (string.IsNullOrEmpty(r.EnlaceDelXml))
                                EstadoSunatResult = EstadoSunat.ERROR_RECEPCION_BAJA;
                            else
                                EstadoSunatResult = r.AceptadaPorSunat ? EstadoSunat.DE_BAJA_CDR_ACEPTADO : EstadoSunat.DE_BAJA;

                            vta.i_EstadoSunat = (short)EstadoSunatResult;
                            vta.v_EnlaceBaja = r.Enlace;
                            dbContext.venta.ApplyCurrentValues(vta);
                            #endregion
                        }

                        #region Manejo de consultas de estado
                        if (tipoAccion == TipoAccion.ConsultarBajaComprobante)
                        {
                            if (r.AceptadaPorSunat)
                            {
                                EstadoSunatResult = EstadoSunat.DE_BAJA_CDR_ACEPTADO;
                                vta.i_EstadoSunat = (short)EstadoSunatResult;
                                vta.v_EnlaceBaja = r.Enlace;
                                dbContext.venta.ApplyCurrentValues(vta);
                            }
                        }

                        if (tipoAccion == TipoAccion.ConsultarComprobante)
                        {
                            if (r.AceptadaPorSunat)
                            {
                                EstadoSunatResult = EstadoSunat.CDR_ACEPTADO;
                                vta.i_EstadoSunat = (short)EstadoSunatResult;
                                vta.v_EnlaceEnvio = r.Enlace;
                                dbContext.venta.ApplyCurrentValues(vta);
                            }
                        }
                        #endregion
                    }
                    else if (rpt is RespuestaError)
                    {
                        var r = (RespuestaError)rpt;
                        if (r.Codigo == 23)
                        {
                            if (EstadoEvent != null) EstadoEvent("Actualizando datos...", rowIndex);
                            EstadoSunatResult = EstadoSunat.ENVIADO_ANTERIORMENTE;
                            vta.i_EstadoSunat = (short)EstadoSunatResult;
                            dbContext.venta.ApplyCurrentValues(vta);
                        }
                        else
                            throw new Exception(r.Errors);
                    }

                    dbContext.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                _erroresOcurridos = true;
                if (ErrorEvent != null) ErrorEvent(exception, rowIndex, TipoAccionRealizar);
            }
        }

        private static string GetDateFormatted(DateTime d)
        {
            return d.Day + "-" + d.Month + "-" + d.Year;
        }
    }
}