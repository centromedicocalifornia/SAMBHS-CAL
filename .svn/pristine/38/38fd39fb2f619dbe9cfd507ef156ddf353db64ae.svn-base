using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using Dapper;

namespace SAMBHS.Venta.BL
{
    public class ImportacionVentasBl
    {
        private readonly BackgroundWorker _backgroundWorker;
        public delegate void Error(string mensaje);
        public delegate void Terminado(bool conExito, string mensaje);
        public event Error ErrorEvent;
        public event Terminado TerminadoEvent;
        public ImportacionRegistroVentaDto VentaImportada;
        public Dictionary<string, KeyValueDTO> ListaClientes { get; set; }
        public Dictionary<string, string> ListaProductos { get; set; }
        public List<string> ClientSession { get; set; }
        private OperationResult _objOperationResult = new OperationResult();
        private bool _terminadoConExito;
        private string _errorMessage;

        public ImportacionVentasBl()
        {
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += _backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
        }

        public void Comenzar(ImportacionRegistroVentaDto ventaImportada)
        {
            VentaImportada = ventaImportada;
            _backgroundWorker.RunWorkerAsync();
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                #region Regulariza cliente

                KeyValueDTO cliente;
                var idCliente = string.Empty;
                cliente = ListaClientes.TryGetValue(VentaImportada.NroDocumentoIdentidad, out cliente) ? cliente : null;
                if (cliente != null)
                {
                    idCliente = cliente.Id;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(VentaImportada.NroDocumentoIdentidad) && !VentaImportada.NroDocumentoIdentidad.Equals("0"))
                    {
                        var objOperationResult = new OperationResult();
                        var anexoCreado = ClienteBL.RegistroRapidoCliente(ref objOperationResult,
                            new clienteDto
                            {
                                v_NroDocIdentificacion = VentaImportada.NroDocumentoIdentidad,
                                v_RazonSocial = VentaImportada.RazonSocialNombre,
                                v_DirecPrincipal = VentaImportada.Direccion
                            },
                            TipoAnexo.Cliente);

                        if (anexoCreado != null)
                        {
                            idCliente = anexoCreado.v_IdCliente;
                        }
                    }
                    else
                        idCliente = "N002-CL000000000";
                }

                #endregion

                #region Regulariza productos

                //if (VentaImportada.DetalleVenta.Any())
                //{
                //    foreach (var det in VentaImportada.DetalleVenta)
                //    {
                //        string idProducto;
                //        if (ListaProductos.TryGetValue(det.CodArticulo.Trim(), out idProducto))
                //        {
                //            det.IdProducto = idProducto;
                //        }
                //        else
                //        {
                //            var productoDto = new productoDto
                //            {
                //                v_CodInterno = det.CodArticulo,
                //                v_IdLinea = "N002-LN000000000", //<- mejorar
                //                v_Descripcion = det.DescripionArticulo,
                //                d_Empaque = 1,
                //                i_IdUnidadMedida = 15,
                //                d_Peso = 0,
                //                v_Ubicacion = string.Empty,
                //                v_Caracteristica = string.Empty,
                //                v_CodProveedor = string.Empty,
                //                v_Descripcion2 = string.Empty,
                //                i_IdTipoProducto = -1,
                //                i_EsServicio = 0,
                //                i_EsLote = 0,
                //                i_EsAfectoDetraccion = 0,
                //                i_EsActivo = 1,
                //                i_NombreEditable = 0,
                //                d_PrecioCosto = 0,
                //                i_ValidarStock = 0,
                //                i_IdProveedor = -1,
                //                i_IdTipo = -1,
                //                d_PrecioVenta = 0,
                //                d_separacion = 0,
                //                i_EsActivoFijo = 0,
                //                d_StockMaximo = 0,
                //                d_StockMinimo = 0,
                //                i_IdUsuario = -1,
                //                i_IdTela = -1,
                //                i_IdEtiqueta = -1,
                //                i_IdCuello = -1,
                //                i_IdAplicacion = -1,
                //                i_IdArte = -1,
                //                i_IdColeccion = -1,
                //                i_IdTemporada = -1,
                //                i_EsAfectoPercepcion = 0,
                //                d_TasaPercepcion = 0,
                //                i_Anio = 0,
                //            };

                //            var idProductoProductoDetalle = new ProductoBL().InsertarProducto(ref _objOperationResult, productoDto, ClientSession);
                //            if (_objOperationResult.Success == 0)
                //                throw new Exception("Error al insertar un artículo, " + _objOperationResult.ErrorMessage);

                //            if (!string.IsNullOrWhiteSpace(idProductoProductoDetalle))
                //            {
                //                var ids = idProductoProductoDetalle.Split(';');
                //                if (ids.Length == 2)
                //                {
                //                    det.IdProducto = ids[1].Trim();
                //                    ListaProductos.Add(det.CodArticulo, det.IdProducto);
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion

                #region Arma cabecera

                int i;
                var fechaVenta = DateTime.Parse(VentaImportada.FechaEmision);
                var correlativo = new VentaBL().ObtenerCorrelativoVenta(fechaVenta.Year.ToString(), fechaVenta.Month.ToString("00"));
                var venta = new ventaDto();
                venta.v_SerieDocumento = int.TryParse(VentaImportada.Serie.Trim(), out i) ? i.ToString("0000") : VentaImportada.Serie.Trim();
                venta.v_CorrelativoDocumento = int.Parse(VentaImportada.Correlativo.Trim()).ToString("00000000");
                venta.v_Periodo = fechaVenta.Year.ToString();
                venta.v_Mes = fechaVenta.Month.ToString("00");
                venta.v_Correlativo = string.Format("01{0}", correlativo.ToString("000000"));
                venta.i_IdIgv = 1;
                venta.i_IdTipoDocumento = VentaImportada.IdTipoDoc;
                venta.v_CorrelativoDocumentoFin = string.Empty;
                venta.i_IdTipoDocumentoRef = VentaImportada.IdTipoDocRef;
                venta.v_SerieDocumentoRef = string.IsNullOrWhiteSpace(VentaImportada.SerieRef)
                    ? string.Empty
                    : int.Parse(VentaImportada.SerieRef).ToString("0000");
                venta.v_CorrelativoDocumentoRef = string.IsNullOrWhiteSpace(VentaImportada.CorrelativoRef)
                    ? string.Empty
                    : int.Parse(VentaImportada.CorrelativoRef).ToString("00000000");
                venta.t_FechaRef = fechaVenta;
                venta.i_IdEstado = !VentaImportada.Anulado.Equals("S") ? 1 : 0;
                venta.v_IdCliente = idCliente;
                venta.v_IdVendedor = "-1";
                venta.v_NombreClienteTemporal = VentaImportada.RazonSocialNombre;
                venta.v_DireccionClienteTemporal = string.Empty;
                venta.t_FechaRegistro = fechaVenta;
                venta.d_TipoCambio = VentaImportada.TipoCambio;
                venta.i_NroDias = 0;
                venta.t_FechaVencimiento = fechaVenta;
                venta.i_IdCondicionPago = 1;
                venta.i_EsAfectoIgv = 1;
                venta.i_PreciosIncluyenIgv = 1;
                venta.v_IdVendedorRef = "-1";
                venta.d_PorcDescuento = 0;
                venta.d_PocComision = 0;
                venta.d_Descuento = 0;
                venta.d_Percepcion = 0;
                venta.d_Anticipio = 0;
                venta.i_DeduccionAnticipio = 0;
                venta.v_NroPedido = string.Empty;
                venta.v_NroGuiaRemisionSerie = string.Empty;
                venta.v_NroGuiaRemisionCorrelativo = string.Empty;
                venta.v_NroBulto = string.Empty;
                venta.t_FechaOrdenCompra = fechaVenta;
                venta.v_OrdenCompra = string.Empty;
                venta.i_IdTipoVenta = VentaImportada.Moneda.Equals("S") ? 3 : 4;
                venta.i_IdTipoOperacion = 1;
                venta.Almacen = 1;
                venta.i_IdEstablecimiento = 1;
                venta.i_IdMoneda = VentaImportada.Moneda.Equals("S") ? 1 : 2;
                venta.v_Concepto = VentaImportada.Glosa;
                venta.d_PesoBrutoKG = 0;
                venta.d_PesoNetoKG = 0;
                venta.i_IdPuntoEmbarque = -1;
                venta.i_IdPuntoDestino = -1;
                venta.i_IdTipoEmbarque = -1;
                venta.i_IdMedioPagoVenta = -1;
                venta.v_Marca = string.Empty;
                venta.i_DrawBack = 0;
                venta.v_BultoDimensiones = string.Empty;
                venta.d_IGV = VentaImportada.ImporteVenta - Utils.Windows.DevuelveValorRedondeado(VentaImportada.ImporteVenta / 1.18m, 2);
                venta.d_Valor = Utils.Windows.DevuelveValorRedondeado(VentaImportada.ImporteVenta / 1.18m, 2);
                venta.d_ValorVenta = Utils.Windows.DevuelveValorRedondeado(VentaImportada.ImporteVenta / 1.18m, 2);
                venta.d_Total = VentaImportada.ImporteVenta;

                #endregion

                #region Arma detalle
                List<ventadetalleDto> detalles;

                if (/*!VentaImportada.DetalleVenta.Any()*/ true)
                {
                    detalles = new List<ventadetalleDto>
                    {
                        new ventadetalleDto
                        {
                            v_NroCuenta = VentaImportada.CuentaMercaderia,
                            i_Anticipio = 0,
                            v_IdProductoDetalle = null,
                            v_DescripcionProducto = string.Empty,
                            d_Cantidad = 1,
                            d_CantidadEmpaque = 1,
                            d_Precio = VentaImportada.ImporteVenta,
                            d_Valor = venta.d_ValorVenta,
                            d_ValorVenta = venta.d_ValorVenta,
                            d_Igv = venta.d_IGV,
                            d_PrecioVenta = venta.d_Total,
                            i_IdUnidadMedida = 15,
                            d_Descuento = 0,
                            i_IdAlmacen = 1,
                            i_IdTipoOperacion = 1,
                            v_Observaciones = string.Empty
                        }
                    };
                }
                else
                {
                    //detalles = VentaImportada.DetalleVenta
                    //    .Select(vd => new ventadetalleDto
                    //    {
                    //        v_NroCuenta = vd.Cuenta,
                    //        i_Anticipio = 0,
                    //        v_IdProductoDetalle = vd.IdProducto,
                    //        v_DescripcionProducto = vd.DescripionArticulo,
                    //        d_Cantidad = vd.Cantidad,
                    //        d_CantidadEmpaque = vd.Cantidad,
                    //        d_Precio = vd.Precio,
                    //        d_Valor = vd.Valor,
                    //        d_ValorVenta = vd.Valor,
                    //        d_Igv = vd.Igv,
                    //        d_PrecioVenta = vd.Total,
                    //        i_IdUnidadMedida = 15,
                    //        d_Descuento = 0,
                    //        i_IdAlmacen = 1,
                    //        i_IdTipoOperacion = 1,
                    //        v_Observaciones = string.Empty
                    //    }).ToList();
                }

                #endregion

                #region Inserta venta

                var idVentaAnterior = new VentaBL().ObtieneIdPorCorrelativoDocumento(ref _objOperationResult,
                    venta.i_IdTipoDocumento ?? 1, venta.v_SerieDocumento.Trim(), venta.v_CorrelativoDocumento.Trim());

                if (!string.IsNullOrWhiteSpace(idVentaAnterior))
                    throw new Exception("La venta ya fue ingresada anteriormente.!");

                var r = new VentaBL().InsertarVenta(ref _objOperationResult, venta, Globals.ClientSession.GetAsList(),
                    detalles);
                _terminadoConExito = _objOperationResult.Success == 1;

                var sb = new StringBuilder();
                sb.AppendLine("- " + _objOperationResult.ErrorMessage);

                if (!string.IsNullOrWhiteSpace(_objOperationResult.ExceptionMessage))
                    sb.AppendLine("- " + _objOperationResult.ExceptionMessage);

                _errorMessage = sb.ToString();

                #endregion
            }
            catch (Exception ex)
            {
                _terminadoConExito = false;
                var sb = new StringBuilder();
                sb.AppendLine("- " + ex.Message);
                if (ex.InnerException != null)
                    sb.AppendLine("- " + ex.InnerException.Message);
                _errorMessage = sb.ToString();
            }

        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (TerminadoEvent != null)
                TerminadoEvent(_terminadoConExito, _errorMessage);
        }
    }
}
