using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SAMBHS.Cobranza.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Common.DataModel;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Tesoreria.BL;

namespace SAMBHS.Letras.BL
{
    public class LetrasCobrarDescuentoBl
    {
        private static string periodo = Globals.ClientSession.i_Periodo.Value.ToString();

        public static BindingList<letrasdescuentomantenimientoDto> ObtenerLetrasProcesadasPorCliente(
            ref OperationResult pobjOperationResult, string pstrIdCliente)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var consulta = (from n in dbContext.letrasdescuentomantenimiento
                                    join J1 in dbContext.letrasdetalle on n.v_IdLetrasDetalle equals J1.v_IdLetrasDetalle into
                                        J1_join
                                    from J1 in J1_join.DefaultIfEmpty()
                                    where J1.v_IdCliente.Equals(pstrIdCliente) && n.i_Eliminado == 0
                                    select new letrasdescuentomantenimientoDto
                                    {
                                        i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                        i_Eliminado = n.i_Eliminado,
                                        i_Estado = n.i_Estado,
                                        i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                        IdCliente = J1.v_IdCliente,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        t_FechaCancelacion = n.t_FechaCancelacion,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        v_IdLetraDescuentoCancelacion = n.v_IdLetraDescuentoCancelacion,
                                        v_IdLetrasDetalle = n.v_IdLetrasDetalle,
                                        Letra = "LEC " + J1.v_Serie + "-" + J1.v_Correlativo,
                                        d_Acuenta = n.d_Acuenta ?? 0,
                                        d_Saldo = n.d_Saldo ?? 0
                                    }).ToList();
                    pobjOperationResult.Success = 1;
                    return new BindingList<letrasdescuentomantenimientoDto>(consulta);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasCobrarDescuentoBl.ObtenerLetrasProcesadasPorCliente()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static List<LetrasDescuentoPorMantenimientoDto> ObtenerLetrasDescuentoPorMantenerPorCliente(
            ref OperationResult pobjOperationResult, string pstrIdCliente)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Obtiene las letras en descuento que fueron abonadas

                    var letrasCanceladas = (from n in dbContext.letrasdetalle
                                            join j1 in dbContext.cobranzadetalle on new { eliminado = 0, id = n.v_IdLetrasDetalle }
                                                equals new { eliminado = j1.i_Eliminado ?? 0, id = j1.v_IdVenta } into j1Join
                                            from j1 in j1Join.DefaultIfEmpty()
                                            where n.v_IdCliente.Equals(pstrIdCliente) && j1Join.Any()
                                            select n).ToList();

                    var nroLetras = letrasCanceladas.Select(p => p.v_Correlativo.Substring(0,5)).Distinct().ToList();
                    foreach (var nroLetra in nroLetras)
                    {
                        var ldctos = (from n in dbContext.letrasdetalle
                            where
                                n.v_Correlativo.StartsWith(nroLetra) && n.v_Correlativo != nroLetra + "-00" &&
                                n.i_Eliminado == 0
                            select n).ToList();
                        letrasCanceladas.AddRange(ldctos);
                    }                                     
                    #endregion

                    if (letrasCanceladas.Any())
                    {
                        #region Consulta las letras por procesar con sus saldos

                        var letrasCanceladasDescuento = (from p in letrasCanceladas
                            join J1 in dbContext.cobranzaletraspendiente on
                                new {eliminado = 0, id = p.v_IdLetrasDetalle}
                                equals new {eliminado = J1.i_Eliminado ?? 0, id = J1.v_IdLetrasDetalle} into J1_join
                            from J1 in J1_join.DefaultIfEmpty()
                            join J2 in dbContext.letrasdetalle on p.v_IdLetrasDetalle equals J2.v_IdLetrasDetalle into
                                J2_join
                            from J2 in J2_join.DefaultIfEmpty()
                            where CobranzaBL.EsLetraDescuento(p.v_IdLetrasDetalle, false) && J1.d_Saldo > 0   
                            select new LetrasDescuentoPorMantenimientoDto
                            {
                                FechaVencimiento = p.t_FechaVencimiento ?? DateTime.Now,
                                IdLetraDetalle = p.v_IdLetrasDetalle,
                                Letra = string.Format("LEC {0}-{1}", p.v_Serie.Trim(), p.v_Correlativo.Trim()),
                                MontoLetra = J2.d_Importe ?? 0,
                                Saldo = J1.d_Saldo ?? 0
                            }).ToList();

                        #endregion

                        pobjOperationResult.Success = 1;
                        return letrasCanceladasDescuento;
                    }
                    pobjOperationResult.Success = 1;
                    return null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "LetrasCobrarDescuentoBl.ObtenerLetrasDescuentoPorMantenerPorCliente()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public static void ActualizarMantenimientoLetrasDescuento(ref OperationResult pobjOperationResult,
            IEnumerable<letrasdescuentomantenimientoDto> ptemInsertar,
            IEnumerable<letrasdescuentomantenimientoDto> ptemModificar,
            IEnumerable<letrasdescuentomantenimientoDto> ptemEliminar,
            List<string> clientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region insertar

                    foreach (var objEntidadDto in ptemInsertar)
                    {
                        var entidad = objEntidadDto.ToEntity();
                        var newIdLetrasDetalle = Utils.GetNewId(int.Parse(clientSession[0]),
                            new SecuentialBL().GetNextSecuentialId(int.Parse(clientSession[0]), 107), "LO");
                        objEntidadDto.v_IdLetraDescuentoCancelacion = entidad.v_IdLetraDescuentoCancelacion = newIdLetrasDetalle;
                        entidad.t_InsertaFecha = DateTime.Now;
                        entidad.i_InsertaIdUsuario = int.Parse(clientSession[2]);
                        entidad.i_Eliminado = 0;
                        dbContext.letrasdescuentomantenimiento.AddObject(entidad);
                        if (objEntidadDto.i_Estado == 1)
                        {
                            GenerarLibroDiario(ref pobjOperationResult, objEntidadDto);
                            if (pobjOperationResult.Success == 0) return;
                            SaldarCuentaLetraDetalle(ref pobjOperationResult, objEntidadDto);
                            if (pobjOperationResult.Success == 0) return;
                        }
                    }

                    #endregion

                    #region actualizar

                    foreach (var objEntidadDto in ptemModificar)
                    {
                        var entidadDetalleOriginal = dbContext.letrasdescuentomantenimiento
                            .FirstOrDefault(
                                p => p.v_IdLetraDescuentoCancelacion == objEntidadDto.v_IdLetraDescuentoCancelacion);

                        var letrasmantenimientodetalle = objEntidadDto.ToEntity();
                        letrasmantenimientodetalle.t_ActualizaFecha = DateTime.Now;
                        letrasmantenimientodetalle.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        dbContext.letrasdescuentomantenimiento.ApplyCurrentValues(letrasmantenimientodetalle);
                    }

                    #endregion

                    #region eliminar

                    foreach (var objEntidadDto in ptemEliminar)
                    {
                        var entidadDetalleOriginal = dbContext.letrasdescuentomantenimiento
                            .FirstOrDefault(
                                p => p.v_IdLetraDescuentoCancelacion == objEntidadDto.v_IdLetraDescuentoCancelacion);

                        var letrasmantenimientodetalle = objEntidadDto.ToEntity();
                        letrasmantenimientodetalle.t_ActualizaFecha = DateTime.Now;
                        letrasmantenimientodetalle.i_ActualizaIdUsuario = int.Parse(clientSession[2]);
                        letrasmantenimientodetalle.i_Eliminado = 1;
                        dbContext.letrasdescuentomantenimiento.ApplyCurrentValues(letrasmantenimientodetalle);

                        if (objEntidadDto.i_Estado == 1)
                        {
                            new DiarioBL().EliminarDiarioXDocRef(ref pobjOperationResult, objEntidadDto.v_IdLetraDescuentoCancelacion, clientSession, false);
                            if (pobjOperationResult.Success == 0) return;
                            ExtornarCuentaLetraDetalle(ref pobjOperationResult, objEntidadDto);
                            if (pobjOperationResult.Success == 0) return;
                        }
                    }

                    #endregion

                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation =
                    "LetrasCobrarDescuentoBl.ActualizarMantenimientoLetrasDescuento()\nLinea:" +
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private static void GenerarLibroDiario(ref OperationResult pobjOperationResult, letrasdescuentomantenimientoDto objLetraDescuento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Genera Libro Diario

                    if (string.IsNullOrWhiteSpace(Globals.ClientSession.NroCtaObligacionesFinancieros) ||
                        !Utils.Windows.EsCuentaImputable(Globals.ClientSession.NroCtaObligacionesFinancieros))
                        throw new Exception("La cuenta de obligaciones financieras no está configurada correctamente.!");

                    var pobjletrasDto = dbContext.letrasdetalle.FirstOrDefault(p => p.v_IdLetrasDetalle.Equals(objLetraDescuento.v_IdLetrasDetalle));
                    if (pobjletrasDto == null) throw new Exception("La letra no existe en la base de datos");

                    var idConcepto = pobjletrasDto.i_IdMoneda == 1 ? "30" : "31";

                    var aa = (dbContext.administracionconceptos.Where(a => a.v_Codigo == idConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo))
                        .Select(a => new { a.v_CuentaIGV, a.v_CuentaPVenta })).FirstOrDefault();

                    if (aa != null && aa.v_CuentaIGV.Trim() != string.Empty &&
                        aa.v_CuentaPVenta.Trim() != string.Empty)
                    {
                        var objDiarioBl = new DiarioBL();
                        var diarioDto = new diarioDto();
                        var tempXInsertar = new List<diariodetalleDto>();

                        #region Diario Cabecera
                        var documentoDiario = Globals.ClientSession.i_IdDocumentoContableLEC ?? 335;
                        var listadoDiarios = objDiarioBl.ObtenerListadoDiario(ref pobjOperationResult,
                            objLetraDescuento.t_FechaCancelacion.Value.Year.ToString(),
                            objLetraDescuento.t_FechaCancelacion.Value.Month.ToString("00"), documentoDiario);
                        var cliente = dbContext.cliente.FirstOrDefault(p => p.v_IdCliente.Equals(pobjletrasDto.v_IdCliente));
                        if (cliente == null) throw new Exception("El cliente ligado a esta letra no existe");
                        var maxMovimiento = listadoDiarios.Any() ? int.Parse(listadoDiarios[listadoDiarios.Count - 1].Value1) : 0;
                        maxMovimiento++;
                        diarioDto.v_IdDocumentoReferencia = objLetraDescuento.v_IdLetraDescuentoCancelacion;
                        diarioDto.v_Periodo = objLetraDescuento.t_FechaCancelacion.Value.Year.ToString();
                        diarioDto.v_Mes = int.Parse(objLetraDescuento.t_FechaCancelacion.Value.Month.ToString()).ToString("00");
                        diarioDto.v_Glosa = "ASIENTO CLIENTE DE LETRA DESCUENTO AL BANCO";
                        diarioDto.v_Nombre =
                            (cliente.v_ApePaterno + " " + cliente.v_ApeMaterno + " " + cliente.v_PrimerNombre + " " +
                             cliente.v_RazonSocial).Trim();

                        diarioDto.v_Correlativo = maxMovimiento.ToString("00000000");
                        diarioDto.d_TipoCambio = pobjletrasDto.d_TipoCambio ?? 1;
                        diarioDto.i_IdMoneda = pobjletrasDto.i_IdMoneda ?? 1;
                        diarioDto.i_IdTipoDocumento = documentoDiario;
                        diarioDto.t_Fecha = objLetraDescuento.t_FechaCancelacion.Value;
                        diarioDto.i_IdTipoComprobante = 2;

                        #endregion

                        #region Ventas Canjeadas
                        var subTotal = objLetraDescuento.d_Acuenta;
                        var dSubTotalVenta = new diariodetalleDto
                        {
                            d_Importe = subTotal > 0 ? subTotal : subTotal * -1,
                            d_Cambio = pobjletrasDto.i_IdMoneda.Value == 1
                                ? Utils.Windows.DevuelveValorRedondeado(
                                    subTotal.Value / pobjletrasDto.d_TipoCambio.Value, 2)
                                : Utils.Windows.DevuelveValorRedondeado(
                                    subTotal.Value * pobjletrasDto.d_TipoCambio.Value, 2),
                            i_IdCentroCostos = "0",
                            i_IdTipoDocumento = pobjletrasDto.i_IdTipoDocumento ?? -1,
                            t_Fecha = objLetraDescuento.t_FechaCancelacion.Value,
                            v_IdCliente = pobjletrasDto.v_IdCliente,
                            v_Naturaleza = "H",
                            v_NroDocumento = pobjletrasDto.v_Serie + "-" + pobjletrasDto.v_Correlativo,
                            v_NroCuenta = (from a in dbContext.administracionconceptos
                                           where a.v_Codigo == idConcepto && a.v_Periodo.Equals(periodo)
                                           select new { a.v_CuentaPVenta }).First().v_CuentaPVenta,
                            i_IdTipoDocumentoRef = -1
                        };

                        tempXInsertar.Add(dSubTotalVenta);

                        var hSubTotalVenta = new diariodetalleDto
                        {
                            d_Importe = subTotal > 0 ? subTotal : subTotal * -1,
                            d_Cambio = pobjletrasDto.i_IdMoneda.Value == 1
                                ? Utils.Windows.DevuelveValorRedondeado(
                                    subTotal.Value / pobjletrasDto.d_TipoCambio.Value, 2)
                                : Utils.Windows.DevuelveValorRedondeado(
                                    subTotal.Value * pobjletrasDto.d_TipoCambio.Value, 2),
                            i_IdCentroCostos = "0",
                            i_IdTipoDocumento = pobjletrasDto.i_IdTipoDocumento ?? -1,
                            t_Fecha = objLetraDescuento.t_FechaCancelacion.Value,
                            v_IdCliente = pobjletrasDto.v_IdCliente,
                            v_Naturaleza = "D",
                            v_NroDocumento = pobjletrasDto.v_Serie + "-" + pobjletrasDto.v_Correlativo,
                            v_NroCuenta = Globals.ClientSession.NroCtaObligacionesFinancieros,
                            i_IdTipoDocumentoRef = -1
                        };
                        tempXInsertar.Add(hSubTotalVenta);
                        #endregion

                        var totDebe = tempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                        var totHaber = tempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                        var totDebeC = tempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                        var totHaberC = tempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);

                        diarioDto.d_TotalDebe =
                            decimal.Parse(Math.Round(totDebe, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                        diarioDto.d_TotalHaber =
                            decimal.Parse(Math.Round(totHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                        diarioDto.d_TotalDebeCambio =
                            decimal.Parse(Math.Round(totDebeC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                        diarioDto.d_TotalHaberCambio =
                            decimal.Parse(Math.Round(totHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                        diarioDto.d_DiferenciaDebe =
                            decimal.Parse(
                                Math.Round(totDebe - totHaber, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                        diarioDto.d_DiferenciaHaber =
                            decimal.Parse(
                                Math.Round(totDebeC - totHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));

                        objDiarioBl.InsertarDiario(ref pobjOperationResult, diarioDto,
                            Globals.ClientSession.GetAsList(), tempXInsertar, (int)TipoMovimientoTesoreria.Ingreso);
                        if (pobjOperationResult.Success == 0) return;
                    }
                    else
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage =
                            "No existe el concepto 30 (CARTERA MN) o 31 (CARTERA ME) para Letras.";
                        pobjOperationResult.ExceptionMessage =
                            "No se pudo completar la transacción, por favor agrege los conceptos señalados.";
                        pobjOperationResult.AdditionalInformation = "LetrasBL.InsertarCanjeoLetras()";
                        return;
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;

                    #endregion
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation =
                    "LetrasCobrarDescuentoBl.GenerarLibroDiario()\nLinea:" +
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private static void SaldarCuentaLetraDetalle(ref OperationResult pobjOperationResult, letrasdescuentomantenimientoDto objLetraDescuento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var letra = dbContext.letrasdetalle.FirstOrDefault(p => p.v_IdLetrasDetalle.Equals(objLetraDescuento.v_IdLetrasDetalle));
                    if (letra == null) return;


                    var cps = dbContext.cobranzaletraspendiente
                        .Where(n => n.v_IdLetrasDetalle == objLetraDescuento.v_IdLetrasDetalle && n.i_Eliminado == 0)
                        .ToList();

                    foreach (var c in cps)
                    {
                        c.d_Acuenta += objLetraDescuento.d_Acuenta;
                        c.d_Saldo -= objLetraDescuento.d_Acuenta; 
                    }

                    letra.i_Pagada = cps.Sum(p => p.d_Acuenta ?? 0) == (letra.d_Importe ?? 0) ? 1 : 0;
                    dbContext.letrasdetalle.ApplyCurrentValues(letra);

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation =
                    "LetrasCobrarDescuentoBl.SaldarCuentaLetraDetalle()\nLinea:" +
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private static void ExtornarCuentaLetraDetalle(ref OperationResult pobjOperationResult, letrasdescuentomantenimientoDto objLetraDescuento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var letra = dbContext.letrasdetalle.FirstOrDefault(p => p.v_IdLetrasDetalle.Equals(objLetraDescuento.v_IdLetrasDetalle));
                    if (letra == null) return;

                    var cps = dbContext.cobranzaletraspendiente
                        .Where(n => n.v_IdLetrasDetalle == objLetraDescuento.v_IdLetrasDetalle && n.i_Eliminado == 0)
                        .ToList();

                    foreach (var c in cps)
                    {
                        c.d_Saldo += objLetraDescuento.d_Acuenta;
                        c.d_Acuenta -= objLetraDescuento.d_Acuenta;
                    }

                    letra.i_Pagada = cps.Sum(p => p.d_Acuenta ?? 0) == (letra.d_Importe ?? 0) ? 1 : 0;
                    dbContext.letrasdetalle.ApplyCurrentValues(letra);

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation =
                    "LetrasCobrarDescuentoBl.SaldarCuentaLetraDetalle()\nLinea:" +
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
    }

    public class LetrasDescuentoPorMantenimientoDto
    {
        public string IdLetraDetalle { get; set; }
        public string Letra { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal MontoLetra { get; set; }
        public decimal Saldo { get; set; }
    }
}
