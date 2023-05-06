using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BL;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel;
using System.Data.Common;
using SAMBHS.Almacen.BL;
using System.Data.Objects;
using SAMBHS.Tesoreria.BL;
using System.Transactions;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Globalization;
using System.IO;
using System.Threading;
using SAMBHS.Common.Resource;
using System.Diagnostics;
using Dapper;
using IsolationLevel = System.Transactions.IsolationLevel;
using SAMBHS.Windows.SigesoftIntegration.UI;


namespace SAMBHS.Venta.BL
{
    
    public partial class VentaBL
    {
        

        public static string ObtenerServicios(List<string> servicios)
        {
            try
            {
                return string.Join(", ", servicios);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<string> ObtenerServicios(string pstrServicios)
        {
            try
            {
                var servicios = pstrServicios.Split(',').Select(p => p.Trim()).ToList();
                return servicios;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Consulta si el vendedor relacionado con el usuario del login tiene permisos para anular.
        /// </summary>
        public static bool TienePermisoAnular
        {
            get
            {
                var pobjOperationResult = new OperationResult();
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var idUsuario = Globals.ClientSession.i_SystemUserId;
                        var vendedorRelacionado =
                            dbContext.vendedor.FirstOrDefault(p => p.i_SystemUser == idUsuario && p.i_Eliminado == 0);

                        if (vendedorRelacionado != null)
                        {
                            return vendedorRelacionado.i_PermiteAnularVentas == 1;
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "VentaBL.TienePermisoAnular()\nLinea:" +
                                                                ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null
                        ? ex.InnerException.Message
                        : string.Empty;
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    return false;
                }
            }
        }

        public int ObtenerCorrelativoVenta(string periodo, string mes)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var ultimaVenta =
                        dbContext.venta.Where(
                            p => p.i_Eliminado == 0 && p.v_Periodo.Equals(periodo) && p.v_Mes.Equals(mes))
                            .OrderBy(o => o.v_Correlativo)
                            .ToList()
                            .LastOrDefault();

                    if (ultimaVenta == null) return 1;
                    int i;
                    var correlativo = int.TryParse(ultimaVenta.v_Correlativo.Substring(2, 6), out i) ? i + 1 : 1;
                    return correlativo;
                }
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Consulta si el vendedor relacionado con el usuario del login tiene permisos para eliminar.
        /// </summary>
        public static bool TienePermisoEliminar
        {
            get
            {
                var pobjOperationResult = new OperationResult();
                try
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var idUsuario = Globals.ClientSession.i_SystemUserId;
                        var vendedorRelacionado =
                            dbContext.vendedor.FirstOrDefault(p => p.i_SystemUser == idUsuario && p.i_Eliminado == 0);

                        if (vendedorRelacionado != null)
                        {
                            return vendedorRelacionado.i_PermiteEliminarVentas == 1;
                        }
                        return Globals.ClientSession.UsuarioEsContable == 1;
                    }
                }
                catch (Exception ex)
                {
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "VentaBL.TienePermisoAnular()\nLinea:" +
                                                                ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                    pobjOperationResult.ErrorMessage = ex.Message;
                    pobjOperationResult.ExceptionMessage = ex.InnerException != null
                        ? ex.InnerException.Message
                        : string.Empty;
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    return false;
                }
            }
        }

        private DocumentoBL _objDocumentoBL = new DocumentoBL();
        private string periodo = Globals.ClientSession.i_Periodo.Value.ToString();
        public List<KeyValueDTO> ObtenerListadoVentas(ref OperationResult pobjOperationResult, string pstringPeriodo,
            string pstringMes)
        {
            try
            {
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                string idAlmacen = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                string replicationID = Globals.ClientSession.ReplicationNodeID;
                var query = (from n in dbcontext.venta
                             where
                                 n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes &&
                                 n.v_IdVenta.Substring(2, 2) == idAlmacen
                                 && n.v_IdVenta.Substring(0, 1) == replicationID
                             orderby n.v_Correlativo ascending
                             select new
                             {
                                 v_Correlativo = n.v_Correlativo,
                                 v_IdVenta = n.v_IdVenta
                             }
                    );

                var query2 = query.AsEnumerable()
                    .Select(x => new KeyValueDTO
                    {
                        Value1 = x.v_Correlativo,
                        Value2 = x.v_IdVenta
                    }).ToList();

                return query2;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ventaDto ObtenerVentaCabecera(ref OperationResult pobjOperationResult, string pstrIdVenta)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from a in dbContext.venta
                                     join A in dbContext.cliente on new { Cliente = a.v_IdCliente, eliminado = 0 } equals
                                         new { Cliente = A.v_IdCliente, eliminado = A.i_Eliminado.Value } into A_join
                                     from A in A_join.DefaultIfEmpty()
                                     join b in dbContext.nbs_formatounicofacturacion on
                                         new { fuf = a.v_IdFormatoUnicoFacturacion, eliminado = 0 } equals
                                         new { fuf = b.v_IdFormatoUnicoFacturacion, eliminado = b.i_Eliminado.Value } into b_join
                                     from b in b_join.DefaultIfEmpty()

                                     join c in dbContext.cobranzapendiente on
                                         new { idVenta = a.v_IdVenta, eliminado = 0 } equals
                                         new { idVenta = c.v_IdVenta, eliminado = c.i_Eliminado.Value } into c_join
                                     from c in c_join.DefaultIfEmpty()

                                     where a.v_IdVenta == pstrIdVenta
                                     select new ventaDto
                                     {
                                         v_IdVenta = a.v_IdVenta,
                                         v_Periodo = a.v_Periodo,
                                         v_Mes = a.v_Mes,
                                         v_Correlativo = a.v_Correlativo,
                                         i_IdIgv = a.i_IdIgv,
                                         i_IdTipoDocumento = a.i_IdTipoDocumento,
                                         v_SerieDocumento = a.v_SerieDocumento,
                                         v_CorrelativoDocumento = a.v_CorrelativoDocumento,
                                         v_CorrelativoDocumentoFin = a.v_CorrelativoDocumentoFin,
                                         i_IdTipoDocumentoRef = a.i_IdTipoDocumentoRef,
                                         v_SerieDocumentoRef = a.v_SerieDocumentoRef,
                                         v_CorrelativoDocumentoRef = a.v_CorrelativoDocumentoRef,
                                         t_FechaRef = a.t_FechaRef,
                                         v_IdCliente = a.v_IdCliente,
                                         t_FechaRegistro = a.t_FechaRegistro,
                                         d_TipoCambio = a.d_TipoCambio,
                                         i_NroDias = a.i_NroDias,
                                         t_FechaVencimiento = a.t_FechaVencimiento,
                                         i_IdCondicionPago = a.i_IdCondicionPago,
                                         v_Concepto = a.v_Concepto,
                                         i_EsAfectoIgv = a.i_EsAfectoIgv,
                                         i_PreciosIncluyenIgv = a.i_PreciosIncluyenIgv,
                                         i_DeduccionAnticipio = a.i_DeduccionAnticipio,
                                         v_IdVendedor = a.v_IdVendedor,
                                         v_IdVendedorRef = a.v_IdVendedorRef,
                                         d_PorcDescuento = a.d_PorcDescuento,
                                         d_PocComision = a.d_PocComision,
                                         i_IdMoneda = a.i_IdMoneda,
                                         i_IdEstado = a.i_IdEstado,
                                         i_IdEstablecimiento = a.i_IdEstablecimiento,
                                         d_Anticipio = a.d_Anticipio,
                                         d_ValorVenta = a.d_ValorVenta,
                                         d_IGV = a.d_IGV,
                                         d_Total = a.d_Total,
                                         i_Eliminado = a.i_Eliminado,
                                         i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                         t_InsertaFecha = a.t_InsertaFecha,
                                         i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                         t_ActualizaFecha = a.t_ActualizaFecha,
                                         v_NroPedido = a.v_NroPedido,
                                         v_NroGuiaRemisionSerie = a.v_NroGuiaRemisionSerie,
                                         v_NroGuiaRemisionCorrelativo = a.v_NroGuiaRemisionCorrelativo,
                                         v_OrdenCompra = a.v_OrdenCompra,
                                         t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                         i_IdTipoVenta = a.i_IdTipoVenta,
                                         i_IdTipoOperacion = a.i_IdTipoOperacion,
                                         i_IdPuntoEmbarque = a.i_IdPuntoEmbarque,
                                         i_IdPuntoDestino = a.i_IdPuntoDestino,
                                         i_IdTipoEmbarque = a.i_IdTipoEmbarque,
                                         i_IdMedioPagoVenta = a.i_IdMedioPagoVenta,
                                         v_Marca = a.v_Marca,
                                         i_DrawBack = a.i_DrawBack,
                                         v_NroBulto = a.v_NroBulto,
                                         v_BultoDimensiones = a.v_BultoDimensiones,
                                         d_PesoBrutoKG = a.d_PesoBrutoKG,
                                         d_PesoNetoKG = a.d_PesoNetoKG,
                                         d_Valor = a.d_Valor,
                                         d_Descuento = a.d_Descuento,
                                         NombreCliente =
                                             a.v_IdCliente != "N002-CL000000000"
                                                 ? (A.v_ApePaterno + " " + A.v_ApeMaterno + " " + A.v_PrimerNombre + " " +
                                                    A.v_RazonSocial).Trim()
                                                 : (a.v_NombreClienteTemporal != null && a.v_NombreClienteTemporal.Trim() != "") ? a.v_NombreClienteTemporal : "PÚBLICO GENERAL",
                                         CodigoCliente = A == null ? "CLIENTE NO EXISTE" : A.v_CodCliente,
                                         NroDocCliente = A == null ? "CLIENTE NO EXISTE" : A.v_NroDocIdentificacion,
                                         //Direccion =
                                         //    a.v_IdCliente != "N002-CL000000000" ? A.v_DirecPrincipal : a.v_DireccionClienteTemporal,
                                         Direccion =
                                            a.v_IdCliente != "N002-CL000000000" ? string.IsNullOrEmpty(a.v_DireccionClienteTemporal) ? A.v_DirecPrincipal : a.v_DireccionClienteTemporal : a.v_DireccionClienteTemporal,
                                         i_IdTipoIdentificacion = A != null ? A.i_IdTipoIdentificacion ?? 0 : 0,
                                         v_PlacaVehiculo = a.v_PlacaVehiculo,
                                         v_NroFUF = b == null ? "" : b.v_NroFormato,
                                         v_IdTipoKardex = a.v_IdTipoKardex == null ? "" : a.v_IdTipoKardex.Trim(),
                                         Saldo = c != null ? c.d_Saldo ?? 0 : 0,
                                         v_IdFormatoUnicoFacturacion = a.v_IdFormatoUnicoFacturacion,
                                         i_EstadoSunat = a.i_EstadoSunat,
                                         i_IdTipoNota = a.i_IdTipoNota,
                                         i_EsGratuito = a.i_EsGratuito,
                                         v_NombreClienteTemporal = a.v_NombreClienteTemporal,
                                         i_IdTipoBulto = a.i_IdTipoBulto ?? -1,
                                         TipoPersonaCliente = A.i_IdTipoPersona ?? -1,
                                         i_IdDireccionCliente = a.i_IdDireccionCliente ?? -1,
                                         d_CantidaTotal = a.d_CantidaTotal,
                                         d_FleteTotal = a.d_FleteTotal,
                                         d_SeguroTotal = a.d_SeguroTotal,
                                         v_NroBL = string.IsNullOrEmpty(a.v_NroBL) ? "" : a.v_NroBL,
                                         t_FechaPagoBL = a.t_FechaPagoBL == null ? DateTime.Now : a.t_FechaPagoBL,
                                         v_Contenedor = string.IsNullOrEmpty(a.v_Contenedor) ? "" : a.v_Contenedor,
                                         v_Banco = string.IsNullOrEmpty(a.v_Banco) ? "" : a.v_Banco,
                                         v_Naviera = string.IsNullOrEmpty(a.v_Naviera) ? "" : a.v_Naviera,
                                         i_AplicaPercepcion = a.i_AplicaPercepcion ?? 0,
                                         i_ClienteEsAgente = a.i_ClienteEsAgente ?? -1,
                                         i_ItemsAfectosPercepcion = a.i_ItemsAfectosPercepcion ?? 0,
                                         d_PorcentajePercepcion = a.d_PorcentajePercepcion ?? 0,
                                         d_Percepcion = a.d_Percepcion ?? 0,
                                         v_NroBultoIngles = a.v_NroBultoIngles,
                                         i_AfectaDetraccion = a.i_AfectaDetraccion ?? 0,
                                         d_TasaDetraccion = a.d_TasaDetraccion ?? 0m,
                                         i_IdTipoOperacionDetraccion = a.i_IdTipoOperacionDetraccion,
                                         i_IdCodigoDetraccion = a.i_IdCodigoDetraccion,
                                         i_EsAnticipo = a.i_EsAnticipo,
                                         v_IdDocAnticipo = a.v_IdDocAnticipo,
                                         i_FacturacionCliente = a.i_FacturacionCliente ?? 0,
                                         v_SigesoftServiceId = a.v_SigesoftServiceId
                                     }
                        ).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    return objEntity;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.ObtenerVentaCabecera()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public BindingList<ventadetalleDto> ObtenerVentaDetalles(ref OperationResult pobjOperationResult,
            string pstrIdVenta)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query1 = (from n in dbContext.ventadetalle

                              join A in dbContext.productodetalle on new { n.v_IdProductoDetalle } equals new { A.v_IdProductoDetalle } into A_join
                              from A in A_join.DefaultIfEmpty()

                              join B in dbContext.producto on A.v_IdProducto equals B.v_IdProducto into B_join
                              from B in B_join.DefaultIfEmpty()

                              join C in dbContext.asientocontable on new { cta = n.v_NroCuenta, e = 0, p = periodo }
                                                                equals new { cta = C.v_NroCuenta, e = C.i_Eliminado ?? 0, p = C.v_Periodo } into C_join
                              from C in C_join.DefaultIfEmpty()

                              join J1 in dbContext.datahierarchy on new { a = B.i_IdUnidadMedida.Value, b = 17 }
                                  equals new { a = J1.i_ItemId, b = J1.i_GroupId } into J1_join


                              from J1 in J1_join.DefaultIfEmpty()


                              join J2 in dbContext.ventadetalleanexo on new { vda = n.i_IdVentaDetalleAnexo ?? -1 } equals new { vda = J2.i_IdVentaDetalleAnexo } into J2_join
                              from J2 in J2_join.DefaultIfEmpty()

                              where n.i_Eliminado == 0 && n.v_IdVenta == pstrIdVenta
                              /*  && B.i_EsServicio ==0 */
                              orderby n.v_IdVentaDetalle ascending
                              select new ventadetalleDto
                              {
                                  i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                  i_Anticipio = n.i_Anticipio,
                                  i_Eliminado = n.i_Eliminado,
                                  i_IdAlmacen = n.i_IdAlmacen,
                                  i_IdCentroCosto = n.i_IdCentroCosto,
                                  i_IdTipoOperacion = n.i_IdTipoOperacion,
                                  i_IdTipoOperacionAnexo = n.i_IdTipoOperacionAnexo,
                                  i_IdUnidadMedida = n.i_IdUnidadMedida,
                                  i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                  i_NroUnidades = n.i_NroUnidades,
                                  ProductoNombre = B != null ? B.v_Descripcion : string.Empty,
                                  v_DescripcionProducto = n.v_DescripcionProducto,
                                  v_FacturaRef = n.v_FacturaRef,
                                  v_IdDiarioDetalle = n.v_IdDiarioDetalle,
                                  v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                                  v_IdProductoDetalle = n.v_IdProductoDetalle,
                                  v_IdVenta = n.v_IdVenta,
                                  v_IdVentaDetalle = n.v_IdVentaDetalle,
                                  v_NroCuenta = n.v_NroCuenta,
                                  v_PedidoExportacion = n.v_PedidoExportacion,
                                  d_PrecioVenta = n.d_PrecioVenta,
                                  d_Cantidad = n.d_Cantidad,
                                  d_CantidadEmpaque = n.d_CantidadEmpaque,
                                  d_Descuento = n.d_Descuento,
                                  d_Igv = n.d_Igv,
                                  d_isc = n.d_isc,
                                  d_otrostributos = n.d_otrostributos,
                                  d_Percepcion = n.d_Percepcion,
                                  d_PorcentajeComision = n.d_PorcentajeComision,
                                  d_Precio = n.d_Precio,
                                  d_PrecioImpresion = n.d_PrecioImpresion,
                                  d_Valor = n.d_Valor,
                                  d_ValorVenta = n.d_ValorVenta,
                                  v_CodigoInterno = B != null ? B.v_CodInterno : string.Empty,
                                  Empaque = B != null ? B.d_Empaque : 0,
                                  UMEmpaque = J1.v_Value1,
                                  i_EsServicio = B != null ? B.i_EsServicio : 1,
                                  i_EsAfectoDetraccion = B != null ? B.i_EsAfectoDetraccion : 0,
                                  i_EsNombreEditable = B != null ? B.i_NombreEditable : 1,
                                  NombreCuenta = C != null ? C.v_NombreCuenta : string.Empty,
                                  i_EsAfectoPercepcion = B != null ? B.i_EsAfectoPercepcion : 0,
                                  d_TasaPercepcion = B != null ? B.d_TasaPercepcion : 0,
                                  EsRedondeo =
                                      n.v_IdProductoDetalle != null && n.v_IdProductoDetalle == "N002-PE000000000" ? "1" : null,
                                  i_IdUnidadMedidaProducto = B != null ? B.i_IdUnidadMedida : -1,
                                  t_ActualizaFecha = n.t_ActualizaFecha,
                                  t_InsertaFecha = n.t_InsertaFecha,
                                  i_ValidarStock = B != null ? B.i_ValidarStock.Value : 0,
                                  i_IdMonedaLP = n.i_IdMonedaLP != null ? n.i_IdMonedaLP.Value : -1,
                                  v_IdFormatoUnicoFacturacionDetalle = null,
                                  i_IdVentaDetalleAnexo = n.i_IdVentaDetalleAnexo ?? -1,
                                  d_SeguroXProducto = n.d_SeguroXProducto,
                                  d_FleteXProducto = n.d_FleteXProducto,
                                  d_PrecioPactado = n.d_PrecioPactado,
                                  v_Observaciones = n.v_Observaciones,
                                  DetalleAnexo = J2 != null ? J2.v_Anexo : "",


                                  i_SolicitarNroSerieSalida = B.i_SolicitarNroSerieSalida ?? 0,
                                  i_SolicitarNroLoteSalida = B.i_SolicitarNroLoteSalida ?? 0,
                                  i_SolicitaOrdenProduccionSalida = B.i_SolicitaOrdenProduccionSalida ?? 0,


                                  v_NroLote = n.v_NroLote,
                                  v_NroSerie = n.v_NroSerie,
                                  t_FechaCaducidad = n.t_FechaCaducidad.Value,

                              }).ToList();

                var query = new BindingList<ventadetalleDto>(query1);
                pobjOperationResult.Success = 1;
                return query;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.ObtenerVentaDetalles()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public BindingList<ventadetalleanexoDto> ObtenerVentaDetallesAnexo(ref OperationResult pobjOperationResult
           )
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query1 = (from n in dbContext.ventadetalleanexo


                              where n.i_Eliminado == 0
                              orderby n.i_IdVentaDetalleAnexo ascending
                              select new ventadetalleanexoDto
                              {
                                  i_IdVentaDetalleAnexo = n.i_IdVentaDetalleAnexo,
                                  v_Anexo = n.v_Anexo,
                                  i_Eliminado = n.i_Eliminado,
                                  t_InsertaFecha = n.t_InsertaFecha,
                                  t_ActualizaFecha = n.t_ActualizaFecha,


                              }

                    ).ToList();

                var query = new BindingList<ventadetalleanexoDto>(query1);
                pobjOperationResult.Success = 1;
                return query;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public ventadetalleanexoDto ObtenerVentaDetallesAnexoPorId(ref OperationResult pobjOperationResult, int Id
         )
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query1 = (from n in dbContext.ventadetalleanexo


                              where n.i_Eliminado == 0 && n.i_IdVentaDetalleAnexo == Id
                              orderby n.i_IdVentaDetalleAnexo ascending
                              select new ventadetalleanexoDto
                              {
                                  i_IdVentaDetalleAnexo = n.i_IdVentaDetalleAnexo,
                                  v_Anexo = n.v_Anexo,
                                  i_Eliminado = n.i_Eliminado,
                                  t_InsertaFecha = n.t_InsertaFecha,
                                  t_ActualizaFecha = n.t_ActualizaFecha,
                                  i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                  i_InsertaIdUsuario = n.i_InsertaIdUsuario,


                              }

                    ).FirstOrDefault();

                //var query = new ventadetalleanexoDto(query1);
                pobjOperationResult.Success = 1;
                return query1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public BindingList<ventadetalleDto> ObtenerFormatoUnicoFacturacionDetalles(ref OperationResult pobjOperationResult,
            string pstrIdFormatoUnico, int TipoOperacion)
        {
            try
            {

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = (from a in dbContext.nbs_formatounicofacturaciondetalle
                                  join b in dbContext.productodetalle on new { pd = a.v_IdProductoDetalle, eliminado = 0 } equals new { pd = b.v_IdProductoDetalle, eliminado = 0 } into b_join
                                  from b in b_join.DefaultIfEmpty()
                                  join c in dbContext.producto on new { p = b.v_IdProducto, eliminado = 0 } equals new { p = c.v_IdProducto, eliminado = c.i_Eliminado.Value } into c_join
                                  from c in c_join.DefaultIfEmpty()
                                  join d in dbContext.linea on new { l = c.v_IdLinea, eliminado = 0 } equals new { l = d.v_IdLinea, eliminado = d.i_Eliminado.Value } into d_join
                                  from d in d_join.DefaultIfEmpty()


                                  where a.v_IdFormatoUnicoFacturacion == pstrIdFormatoUnico && a.i_Eliminado == 0 && a.i_FacturadoContabilidad == 1
                                  select new ventadetalleDto
                                          {
                                              i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                              i_Anticipio = 0,
                                              i_Eliminado = a.i_Eliminado,
                                              i_IdAlmacen = -1,
                                              i_IdCentroCosto = "",
                                              i_IdTipoOperacion = -1,
                                              i_IdTipoOperacionAnexo = 0,
                                              i_IdUnidadMedida = c.i_IdUnidadMedida,
                                              i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                              i_NroUnidades = 0,
                                              ProductoNombre = c != null ? c.v_Descripcion : string.Empty,
                                              v_DescripcionProducto = a.v_DescripcionTemporal, ///c.v_Descripcion,
                                              v_FacturaRef = null,
                                              v_IdMovimientoDetalle = null,
                                              v_IdProductoDetalle = b.v_IdProductoDetalle,
                                              v_IdVenta = null,
                                              v_IdVentaDetalle = null,
                                              v_NroCuenta = d.v_NroCuentaVenta,
                                              v_Observaciones = null,
                                              v_PedidoExportacion = null,
                                              d_PrecioVenta = a.d_ImporteRegistral ?? 0,
                                              d_Cantidad = a.i_Cantidad ?? 0,
                                              d_CantidadEmpaque = a.i_Cantidad ?? 0,
                                              d_Descuento = 0,
                                              d_Igv = 0,
                                              d_isc = 0,
                                              d_otrostributos = 0,
                                              d_Percepcion = 0,
                                              d_PorcentajeComision = 0,
                                              d_Total = a.d_Total ?? 0,
                                              d_Precio = a.d_ImporteRegistral ?? 0,
                                              d_PrecioImpresion = a.d_ImporteRegistral ?? 0,
                                              d_Valor = 0,
                                              d_ValorVenta = 0,
                                              v_CodigoInterno = c != null ? c.v_CodInterno : "",
                                              Empaque = c != null ? c.d_Empaque : 0,
                                              i_EsServicio = c != null ? c.i_EsServicio : 1,
                                              i_EsAfectoDetraccion = c != null ? c.i_EsAfectoDetraccion : 0,
                                              i_EsNombreEditable = c != null ? c.i_NombreEditable : 1,

                                              d_TasaPercepcion = c != null ? c.d_TasaPercepcion : 0,
                                              EsRedondeo =
                                              b.v_IdProductoDetalle != null && b.v_IdProductoDetalle == "N002-PE000000000" ? "1" : null,
                                              i_IdUnidadMedidaProducto = c != null ? c.i_IdUnidadMedida : -1,

                                              i_ValidarStock = c != null ? c.i_ValidarStock.Value : 0,
                                              i_IdMonedaLP = -1,
                                              v_IdFormatoUnicoFacturacionDetalle = a.v_IdFormatoUnicoFacturacionDetalle,
                                          }
                                ).ToList();

                    //var query = new BindingList<ventadetalleDto>(result);
                    //Agregue 20 diciembre
                    var result1 = result.Where(l => l.d_Precio > 0).GroupBy(l => new { cod = l.v_CodigoInterno }).Select(p =>
                    {
                        var k = p.FirstOrDefault();
                        k.d_Cantidad = 1;
                        k.d_Precio = p.Sum(l => l.d_Total);
                        k.d_Total = p.Sum(l => l.d_Total);
                        return k;

                    }).ToList();

                    var query = new BindingList<ventadetalleDto>(result1);
                    pobjOperationResult.Success = 1;
                    return query;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<string[]> DevolverNombres(string pstrIdVenta)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query2 = DevuelveNombresVenta(dbContext, pstrIdVenta);

                if (query2 != null)
                {
                    List<string[]> Lista = new List<string[]>();
                    foreach (var Fila in query2)
                    {
                        string[] Cadena = new string[11];
                        Cadena[0] = Fila.CodigoInterno;
                        Cadena[1] = null;
                        Cadena[2] = Fila.Empaque.ToString();
                        Cadena[3] = Fila.UMEmpaque;
                        Cadena[4] = Fila.i_EsServicio.ToString();
                        Cadena[5] = Fila.i_EsAfectoDetraccion.ToString();
                        Cadena[6] = Fila.i_EsNombreEditable.ToString();
                        Cadena[7] = Fila.NombreCuenta;
                        Cadena[8] = Fila.i_EsAfectoPercepcion.ToString();
                        Cadena[9] = Fila.d_TasaPercepcion.ToString();
                        Cadena[10] = Fila.i_IdUnidadMedida.ToString();
                        Lista.Add(Cadena);
                    }
                    return Lista;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static bool CorrelativoYaExiste(int tipoDocumento, string serie, string correlativo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var existe = dbContext.venta.Any(p => p.i_IdTipoDocumento == tipoDocumento && p.v_SerieDocumento.Trim().Equals(serie.Trim()) &&
                                p.v_CorrelativoDocumento.Trim().Equals(correlativo.Trim()) && p.i_Eliminado == 0);

                    return existe;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string[] DevolverNombresParaVentaRapida(string pstringIdProductoDetalle)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var EntityProducto = (from n in dbContext.productodetalle

                                      join A in dbContext.producto on n.v_IdProducto equals A.v_IdProducto into A_join
                                      from A in A_join.DefaultIfEmpty()

                                      join B in dbContext.producto on n.v_IdProducto equals B.v_IdProducto into B_join
                                      from B in B_join.DefaultIfEmpty()

                                      join J1 in dbContext.datahierarchy on new { a = B.i_IdUnidadMedida.Value, b = 17 }
                                          equals new { a = J1.i_ItemId, b = J1.i_GroupId } into J1_join
                                      from J1 in J1_join.DefaultIfEmpty()

                                      where n.v_IdProductoDetalle == pstringIdProductoDetalle
                                      select new
                                      {
                                          Nombre = A.v_Descripcion,
                                          CodigoInterno = A.v_CodInterno,
                                          Empaque = B.d_Empaque,
                                          UMEmpaque = J1.v_Value1,
                                          i_EsServicio = A.i_EsServicio,
                                          i_EsAfectoDetraccion = A.i_EsAfectoDetraccion
                                      }
                    ).FirstOrDefault();

                if (EntityProducto != null)
                {
                    string[] Cadena = new string[8];
                    if (EntityProducto != null)
                    {
                        Cadena[0] = EntityProducto.CodigoInterno;
                        Cadena[1] = EntityProducto.Nombre;
                        Cadena[2] = EntityProducto.Empaque.ToString();
                        Cadena[3] = EntityProducto.UMEmpaque;
                        Cadena[4] = EntityProducto.i_EsServicio.ToString();
                        Cadena[5] = EntityProducto.i_EsAfectoDetraccion.ToString();
                    }

                    return Cadena;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string InsertarVenta(ref OperationResult pobjOperationResult, ventaDto pobjDtoEntity,
            List<string> ClientSession, List<ventadetalleDto> pTemp_Insertar,
            List<nbs_ventakardexDto> pTemp_InsertarKardex = null, bool _guardadoSinProceso = false, bool ActualizarUsadoeEnFactura = false, List<string> anularRegistrosGuia = null, bool EliminarVentas = false, bool Extraccion = false)
        {
            try
            {
                string newIdVenta = string.Empty;
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        dbContext.venta.MergeOption = MergeOption.NoTracking;
                        dbContext.ventadetalle.MergeOption = MergeOption.NoTracking;
                        dbContext.ventadetalle.EnablePlanCaching = true;

                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        DocumentoBL _objDocumentoBL = new DocumentoBL();
                        venta objEntityVenta = ventaAssembler.ToEntity(pobjDtoEntity);
                        ventadetalleDto pobjDtoVentaDetalle = new ventadetalleDto();

                        int SecuentialId = 0;
                        
                        string newIdVentaDetalle = string.Empty;
                        int intNodeId;

                        #region Inserta Cabecera
                        if (CorrelativoYaExiste(pobjDtoEntity.i_IdTipoDocumento ?? -1, pobjDtoEntity.v_SerieDocumento,
                            pobjDtoEntity.v_CorrelativoDocumento))
                            throw new Exception("El correlativo de la venta ya existe.");
                        //objEntityVenta.t_InsertaFecha = DateTime.Now;
                        objEntityVenta.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityVenta.i_Eliminado = 0;

                        if (!_guardadoSinProceso)
                        {
                            if (objEntityVenta.v_SerieDocumento.Trim().StartsWith("F") ||
                                                       objEntityVenta.v_SerieDocumento.Trim().StartsWith("B"))
                                objEntityVenta.i_EstadoSunat = (short)EstadoSunat.PENDIENTE;
                            else
                                objEntityVenta.i_EstadoSunat = null;
                        }

                        var factDesc = 1 - (objEntityVenta.d_PorcDescuento ?? 0M) / 100;
                        objEntityVenta.d_Total = Utils.Windows.DevuelveValorRedondeado(pTemp_Insertar.Sum(p => p.i_IdTipoOperacion > 10 ? 0M : p.d_PrecioVenta ?? 0) * factDesc, 2);
                        objEntityVenta.d_Valor = Utils.Windows.DevuelveValorRedondeado(pTemp_Insertar.Sum(p => p.i_IdTipoOperacion > 10 ? 0M : p.d_Valor ?? 0), 2);
                        objEntityVenta.d_ValorVenta = Utils.Windows.DevuelveValorRedondeado(pTemp_Insertar.Sum(p => p.i_IdTipoOperacion > 10 ? 0M : p.d_ValorVenta ?? 0) * factDesc, 2);
                        objEntityVenta.d_IGV = Utils.Windows.DevuelveValorRedondeado(pTemp_Insertar.Sum(p => p.i_IdTipoOperacion > 10 ? 0M : p.d_Igv ?? 0) * factDesc, 2);

                        intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 40);
                        newIdVenta = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZQ");
                        objEntityVenta.v_IdVenta = newIdVenta;
                        dbContext.AddToventa(objEntityVenta);

                        #endregion

                        #region Inserta Detalle

                        foreach (ventadetalleDto ventadetalleDto in pTemp_Insertar)
                        {
                            ventadetalle objEntityVentaDetalle = ventadetalleAssembler.ToEntity(ventadetalleDto);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 41);
                            newIdVentaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZN");
                            objEntityVentaDetalle.v_IdVentaDetalle = newIdVentaDetalle;
                            objEntityVentaDetalle.v_IdVenta = newIdVenta;
                            objEntityVentaDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityVentaDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityVentaDetalle.i_Eliminado = 0;
                            dbContext.AddToventadetalle(objEntityVentaDetalle);
                        }

                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "venta",
                            newIdVenta);

                        #endregion
                        
                        #region Genera Cobranza Pendiente

                        if (newIdVenta != null && _guardadoSinProceso != true)
                            if (objEntityVenta.i_IdEstado == 1)
                            {
                                InsertarCobranzaPendiente(ref pobjOperationResult, newIdVenta, objEntityVenta.d_Total.Value,
                                    Globals.ClientSession.GetAsList());
                                if (pobjOperationResult.Success == 0) return string.Empty;
                            }

                        #endregion

                        #region Actualiza Correlativo EmpresaDetalle

                        if (_guardadoSinProceso != true)
                        {
                            _objDocumentoBL.ActualizarCorrelativoPorSerie(ref pobjOperationResult,
                                Globals.ClientSession.i_IdEstablecimiento, objEntityVenta.i_IdTipoDocumento.Value,
                                objEntityVenta.v_SerieDocumento, int.Parse(objEntityVenta.v_CorrelativoDocumento) + 1);
                            if (pobjOperationResult.Success == 0) return string.Empty;
                        }

                        #endregion

                        if (Extraccion)
                        {
                            #region Despacho Pedido

                            string[] Pedido = new string[2];

                            Pedido = pobjDtoEntity.v_NroPedido.Split(new Char[] { '-' });  //  pstrIdPedido.Split(new Char[] { '-' });

                            if (Pedido.Count() == 2)
                            {
                                var Serie = Pedido[0].Trim();
                                var Correlativo = Pedido[1].Trim();

                                string pstrIdPedido = dbContext.pedido.Where(o => o.v_SerieDocumento == Serie && o.v_CorrelativoDocumento == Correlativo && o.i_Eliminado == 0 && o.i_IdTipoDocumento == (int)TiposDocumentos.Pedido).Select(o => o.v_IdPedido).FirstOrDefault();
                                DespacharPedidoExtraccion(ref pobjOperationResult, pstrIdPedido, Globals.ClientSession.GetAsList(), objEntityVenta.v_IdVenta);
                                if (pobjOperationResult.Success == 0) return null;
                            }
                            #endregion
                        }

                        pobjOperationResult.Success = 1;

                        #region Genera la salida a almacén
                        GenerarSalidaAlmacen(ref pobjOperationResult, newIdVenta, "", "", "");
                        if (pobjOperationResult.Success == 0) return null;
                        #endregion

                       

                        ts.Complete();
                        
                    }
                }

                if (!string.IsNullOrEmpty(pobjDtoEntity.v_SigesoftServiceId))
                {
                    var servicios = ObtenerServicios(pobjDtoEntity.v_SigesoftServiceId);
                    foreach (var serv in servicios)
                    {
                        var tipo = pobjDtoEntity.i_FacturacionCliente == 0 ? TipoFacturacion.Aseguradora : TipoFacturacion.Asistencial;
                        var tipoDespacho = pobjDtoEntity.i_IdEstado == 1 ? TipoAccionFacturacion.Facturar : TipoAccionFacturacion.Anular;
                        FacturacionServiciosBl.ProcesarServicio(tipo, tipoDespacho, serv, newIdVenta);
                    }
                } 
                return newIdVenta;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.InsertarVenta()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        #region Asiento Contables
        public void GeneraAsientoContable(ref OperationResult pobjOperationResult, string pstridVenta)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Genera Libro Diario

                    var pobjDtoEntity = dbContext.venta.FirstOrDefault(p => p.v_IdVenta.Equals(pstridVenta));
                    if (pobjDtoEntity == null) return;
                    var clienteDto = dbContext.cliente.FirstOrDefault(p => p.v_IdCliente.Equals(pobjDtoEntity.v_IdCliente)).ToDTO();

                    if (new DocumentoBL().DocumentoEsContable(pobjDtoEntity.i_IdTipoDocumento ?? -1) && pobjDtoEntity.i_IdEstado != 0)
                    {
                        string IDConcepto = pobjDtoEntity.i_IdTipoVenta.Value.ToString("00");

                        var aa = (from a in dbContext.administracionconceptos
                                  where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                  select new { a.v_CuentaIGV, a.v_CuentaPVenta }).FirstOrDefault();

                        if (aa != null && aa.v_CuentaIGV.Trim() != string.Empty &&
                            aa.v_CuentaPVenta.Trim() != string.Empty)
                        {
                            DiarioBL _objDiarioBL = new DiarioBL();
                            diarioDto _diarioDto = new diarioDto();
                            List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                            List<diariodetalleDto> TempXInsertar = new List<diariodetalleDto>();

                            var DetalleVenta = (from d in dbContext.ventadetalle
                                                where d.v_IdVenta == pstridVenta && d.i_Eliminado == 0
                                                select d).ToList();

                            diariodetalleDto H_SubTotalVenta = new diariodetalleDto();
                            diariodetalleDto H_IGV = new diariodetalleDto();
                            diariodetalleDto D_PrecioVenta = new diariodetalleDto();
                            diariodetalleDto H_Percepcion = new diariodetalleDto();
                            var H_ISC = new diariodetalleDto();

                            #region Diario Cabecera
                            _diarioDto.v_IdDocumentoReferencia = pstridVenta;
                            _diarioDto.v_Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                            _diarioDto.v_Mes = pobjDtoEntity.v_Mes;
                            _diarioDto.v_Nombre = clienteDto.NombreRazonSocial;
                            _diarioDto.v_Glosa = pobjDtoEntity.v_Concepto;
                            _diarioDto.v_Correlativo = pobjDtoEntity.v_Correlativo;
                            _diarioDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio.Value;
                            _diarioDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;
                            _diarioDto.i_IdTipoDocumento = 337; // D/V = Diario de venta
                            _diarioDto.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                            _diarioDto.i_IdTipoComprobante = 2;
                            _diarioDto.i_AfectaDetraccion = pobjDtoEntity.i_AfectaDetraccion ?? 0;
                            _diarioDto.d_TasaDetraccion = pobjDtoEntity.d_TasaDetraccion ?? 0m;
                            _diarioDto.i_IdTipoOperacionDetraccion = pobjDtoEntity.i_IdTipoOperacionDetraccion ?? -1;
                            _diarioDto.i_IdCodigoDetraccion = pobjDtoEntity.i_IdCodigoDetraccion;
                            _diarioDto.t_FechaVencimiento = pobjDtoEntity.t_FechaVencimiento;
                            _diarioDto.t_FechaEmision = pobjDtoEntity.t_FechaRegistro;
                            #endregion

                            #region SubTotalVenta

                            var ventaDetalleAgrupado = DetalleVenta.GroupBy(g => new { nroCuenta = g.v_NroCuenta, anticipio = g.i_Anticipio ?? 0, cCosto = !string.IsNullOrWhiteSpace(g.i_IdCentroCosto) ? g.i_IdCentroCosto.Trim() : string.Empty });
                            foreach (var ventaDetalle in ventaDetalleAgrupado)
                            {
                                decimal SubTotal = ventaDetalle.Sum(o => o.d_ValorVenta ?? 0);
                                H_SubTotalVenta.d_Importe = SubTotal > 0 ? SubTotal : SubTotal * -1;
                                H_SubTotalVenta.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1
                                    ? Utils.Windows.DevuelveValorRedondeado(
                                        SubTotal / pobjDtoEntity.d_TipoCambio.Value, 2)
                                    : Utils.Windows.DevuelveValorRedondeado(
                                        SubTotal * pobjDtoEntity.d_TipoCambio.Value, 2);
                                H_SubTotalVenta.i_IdCentroCostos = ventaDetalle.Key.cCosto;
                                H_SubTotalVenta.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                H_SubTotalVenta.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                H_SubTotalVenta.v_IdCliente = pobjDtoEntity.v_IdCliente;
                                if (ventaDetalle.Key.anticipio.Equals(0))
                                    H_SubTotalVenta.v_Naturaleza = SubTotal > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";
                                else
                                    H_SubTotalVenta.v_Naturaleza = SubTotal > 0 ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H" : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D";

                                H_SubTotalVenta.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" + pobjDtoEntity.v_CorrelativoDocumento;
                                H_SubTotalVenta.v_NroCuenta = ventaDetalle.Key.nroCuenta;

                                if (_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ||
                                    pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                                {
                                    H_SubTotalVenta.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdTipoDocumentoRef.Value;
                                    H_SubTotalVenta.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                                        pobjDtoEntity.v_CorrelativoDocumentoRef;
                                }

                                TempXInsertar.Add(H_SubTotalVenta);
                                H_SubTotalVenta = new diariodetalleDto();
                            }
                            #endregion

                            #region Percepción

                            if (DetalleVenta.Sum(p => p.d_Percepcion) > 0)
                            {
                                if (Utils.Windows.DevuelveCuentaDatos(Globals.ClientSession.NroCtaPercepcion) == null)
                                    throw new ArgumentNullException("La cuenta de percepción no es válida!.");

                                H_Percepcion.d_Importe =
                                    decimal.Parse(
                                        Math.Round(DetalleVenta.Sum(p => p.d_Percepcion).Value, 2,
                                            MidpointRounding.AwayFromZero).ToString("0.00"));
                                H_Percepcion.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1
                                    ? Utils.Windows.DevuelveValorRedondeado(
                                        DetalleVenta.Sum(p => p.d_Percepcion.Value) / pobjDtoEntity.d_TipoCambio.Value,
                                        2)
                                    : Utils.Windows.DevuelveValorRedondeado(
                                        DetalleVenta.Sum(p => p.d_Percepcion.Value) * pobjDtoEntity.d_TipoCambio.Value,
                                        2);
                                H_Percepcion.i_IdCentroCostos = DetalleVenta.FirstOrDefault().i_IdCentroCosto;
                                H_Percepcion.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                H_Percepcion.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                H_Percepcion.v_IdCliente = pobjDtoEntity.v_IdCliente;
                                H_Percepcion.v_Naturaleza =
                                    !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value)
                                        ? "H"
                                        : "D";
                                H_Percepcion.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" +
                                                              pobjDtoEntity.v_CorrelativoDocumento;
                                H_Percepcion.v_NroCuenta = Globals.ClientSession.NroCtaPercepcion;

                                if (_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ||
                                    pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                                {
                                    H_Percepcion.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdTipoDocumentoRef.Value;
                                    H_Percepcion.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                                     pobjDtoEntity.v_CorrelativoDocumentoRef;
                                }
                                TempXInsertar.Add(H_Percepcion);
                            }

                            #endregion

                            #region ISC

                            if (DetalleVenta.Sum(p => p.d_isc ?? 0) > 0)
                            {
                                if (Utils.Windows.DevuelveCuentaDatos(Globals.ClientSession.NroCtaISC) == null)
                                    throw new ArgumentNullException("La cuenta de ISC no es válida!.");

                                H_ISC.d_Importe = decimal.Parse(Math.Round(DetalleVenta.Sum(p => p.d_isc ?? 0), 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                                H_ISC.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1
                                    ? Utils.Windows.DevuelveValorRedondeado(
                                        DetalleVenta.Sum(p => p.d_isc.Value) / pobjDtoEntity.d_TipoCambio.Value,
                                        2)
                                    : Utils.Windows.DevuelveValorRedondeado(
                                        DetalleVenta.Sum(p => p.d_isc.Value) * pobjDtoEntity.d_TipoCambio.Value,
                                        2);
                                H_ISC.i_IdCentroCostos = DetalleVenta.FirstOrDefault().i_IdCentroCosto;
                                H_ISC.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                H_ISC.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                H_ISC.v_IdCliente = pobjDtoEntity.v_IdCliente;
                                H_ISC.v_Naturaleza =
                                    !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value)
                                        ? "H"
                                        : "D";
                                H_ISC.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" +
                                                              pobjDtoEntity.v_CorrelativoDocumento;
                                H_ISC.v_NroCuenta = Globals.ClientSession.NroCtaISC;

                                if (_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ||
                                    pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                                {
                                    H_ISC.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdTipoDocumentoRef.Value;
                                    H_ISC.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                                     pobjDtoEntity.v_CorrelativoDocumentoRef;
                                }

                                TempXInsertar.Add(H_ISC);
                                H_ISC = new diariodetalleDto();
                            }

                            #endregion

                            #region IGV
                            var igvAgrupado = DetalleVenta.GroupBy(g => new { anticipio = g.i_Anticipio ?? 0 });
                            foreach (var igvDetalle in igvAgrupado)
                            {
                                H_IGV.d_Importe = decimal.Parse(Math.Round(igvDetalle.Sum(p => p.d_Igv.Value), 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                                H_IGV.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1 ? Utils.Windows.DevuelveValorRedondeado(H_IGV.d_Importe.Value / pobjDtoEntity.d_TipoCambio.Value, 2) : Utils.Windows.DevuelveValorRedondeado(H_IGV.d_Importe.Value * pobjDtoEntity.d_TipoCambio.Value, 2);
                                H_IGV.i_IdCentroCostos = string.Empty;
                                H_IGV.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                H_IGV.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                H_IGV.v_IdCliente = pobjDtoEntity.v_IdCliente;
                                if (igvDetalle.Key.anticipio.Equals(0))
                                    H_IGV.v_Naturaleza = !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "H" : "D";
                                else
                                    H_IGV.v_Naturaleza = !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";

                                H_IGV.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" + pobjDtoEntity.v_CorrelativoDocumento;
                                H_IGV.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                     where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                                     select new { a.v_CuentaIGV }).First().v_CuentaIGV;

                                if (_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ||
                                    pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                                {
                                    H_IGV.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdTipoDocumentoRef.Value;
                                    H_IGV.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                              pobjDtoEntity.v_CorrelativoDocumentoRef;
                                }
                                TempXInsertar.Add(H_IGV);
                                H_IGV = new diariodetalleDto();
                            }

                            #endregion

                            #region PrecioVenta
                            D_PrecioVenta.v_Naturaleza = !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ? "D" : "H";

                            var montoAnticipio = DetalleVenta.Where(p => p.i_Anticipio == 1).Sum(p => p.d_PrecioVenta ?? 0);
                            var importe = DetalleVenta.Where(p => p.i_Anticipio != 1).Sum(p => p.d_PrecioVenta ?? 0) - montoAnticipio;

                            D_PrecioVenta.d_Importe = importe;

                            D_PrecioVenta.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1
                                    ? Utils.Windows.DevuelveValorRedondeado(
                                        importe / pobjDtoEntity.d_TipoCambio.Value,
                                        2)
                                    : Utils.Windows.DevuelveValorRedondeado(
                                        importe * pobjDtoEntity.d_TipoCambio.Value,
                                        2); ;
                            D_PrecioVenta.i_IdCentroCostos = string.Empty;
                            D_PrecioVenta.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                            D_PrecioVenta.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                            D_PrecioVenta.v_IdCliente = pobjDtoEntity.v_IdCliente;


                            D_PrecioVenta.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" +
                                                           pobjDtoEntity.v_CorrelativoDocumento;
                            D_PrecioVenta.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                         where a.v_Codigo == IDConcepto && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                                         select new { a.v_CuentaPVenta }).First().v_CuentaPVenta;

                            if (_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ||
                                pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                            {
                                D_PrecioVenta.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdTipoDocumentoRef.Value;
                                D_PrecioVenta.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                                  pobjDtoEntity.v_CorrelativoDocumentoRef;
                            }
                            TempXInsertar.Add(D_PrecioVenta);

                            #endregion

                            decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                            TotDebe = TempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                            TotHaber = TempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                            TotDebeC = TempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                            TotHaberC = TempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);

                            _diarioDto.d_TotalDebe =
                                decimal.Parse(Math.Round(TotDebe, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            _diarioDto.d_TotalHaber =
                                decimal.Parse(Math.Round(TotHaber, 2, MidpointRounding.AwayFromZero)
                                    .ToString("0.00"));
                            _diarioDto.d_TotalDebeCambio =
                                decimal.Parse(Math.Round(TotDebeC, 2, MidpointRounding.AwayFromZero)
                                    .ToString("0.00"));
                            _diarioDto.d_TotalHaberCambio =
                                decimal.Parse(
                                    Math.Round(TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                            _diarioDto.d_DiferenciaDebe =
                                decimal.Parse(
                                    Math.Round(TotDebe - TotHaber, 2, MidpointRounding.AwayFromZero)
                                        .ToString("0.00"));
                            _diarioDto.d_DiferenciaHaber =
                                decimal.Parse(
                                    Math.Round(TotDebeC - TotHaberC, 2, MidpointRounding.AwayFromZero)
                                        .ToString("0.00"));

                            _objDiarioBL.InsertarDiario(ref pobjOperationResult, _diarioDto,
                                Globals.ClientSession.GetAsList(), TempXInsertar,
                                (int)TipoMovimientoTesoreria.Ingreso);
                            if (pobjOperationResult.Success == 0) return;
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.GeneraAsientoContable()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void EliminarAsientoContable(ref OperationResult pobjOperationResult, string pstridVenta)
        {
            try
            {
                new DiarioBL().EliminarDiarioXDocRef(ref pobjOperationResult, pstridVenta, Globals.ClientSession.GetAsList(), false);
                if (pobjOperationResult.Success == 0) return;
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.EliminarAsientoContable()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void RegenerarAsientoContable(ref OperationResult pobjOperationResult, string pstridVenta)
        {
            try
            {
                EliminarAsientoContable(ref pobjOperationResult, pstridVenta);
                if (pobjOperationResult.Success == 0) return;
                GeneraAsientoContable(ref pobjOperationResult, pstridVenta);
                if (pobjOperationResult.Success == 0) return;

                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.RegenerarAsientoContable()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion

        public ventadetalleanexoDto InsertarVentaDetalleAnexo(ref OperationResult pobjOperationResult, ventadetalleanexoDto ObjventadetalleanexoDto, List<string> ClientSession)
        {
            //mon.IsActive = true;
            int SecuentialId = 0;
            string newId = string.Empty;
            try
            {
                //  SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                using (SAMBHSEntitiesModelWin dbContext = new Common.DataModel.SAMBHSEntitiesModelWin())
                {
                    // tipodecambio objEntity = tipodecambioAssembler.ToEntity(ObjventadetalleanexoDto);
                    ventadetalleanexo objEntity = ventadetalleanexoAssembler.ToEntity(ObjventadetalleanexoDto);

                    objEntity.i_IdVentaDetalleAnexo = dbContext.ventadetalleanexo.Any() ? dbContext.ventadetalleanexo.Max(p => p.i_IdVentaDetalleAnexo) + 1 : 1;
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;
                    dbContext.AddToventadetalleanexo(objEntity);
                    ObjventadetalleanexoDto.i_IdVentaDetalleAnexo = objEntity.i_IdVentaDetalleAnexo;
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;

                    return ObjventadetalleanexoDto;
                }
            }
            catch (Exception ex)
            {


                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.InsertarVentaDetalleAnexo()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void EliminarVentaDetalleAnexo(ref OperationResult pobjOperationResult, int pIntDocId, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.ventadetalleanexo
                                       where a.i_IdVentaDetalleAnexo == pIntDocId
                                       select a).FirstOrDefault();

                dbContext.DeleteObject(objEntitySource);

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "documento", objEntitySource.i_IdVentaDetalleAnexo.ToString());
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.EliminarVentaDetalleAnexo()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }


        public void EliminarDireccionesCliente(ref OperationResult pobjOperationResult, int pIntDocId, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.clientedirecciones
                                       where a.i_IdDireccionCliente == pIntDocId && a.i_Eliminado == 0
                                       select a).FirstOrDefault();

                //dbContext.DeleteObject(objEntitySource);
                objEntitySource.t_ActualizaFecha = DateTime.Now;
                objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntitySource.i_Eliminado = 1;
                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "documento", objEntitySource.i_IdDireccionCliente.ToString());
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.EliminarDireccionesCliente()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public ventadetalleanexoDto ActualizarVentaDetalleAnexo(ref OperationResult pobjOperationResult, ventadetalleanexoDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.ventadetalleanexo
                                           where a.i_IdVentaDetalleAnexo == pobjDtoEntity.i_IdVentaDetalleAnexo
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);


                    ventadetalleanexo objEntity = ventadetalleanexoAssembler.ToEntity(pobjDtoEntity);
                    dbContext.ventadetalleanexo.ApplyCurrentValues(objEntity);
                    // Guardar los cambios
                    dbContext.SaveChanges();
                    pobjDtoEntity.i_IdVentaDetalleAnexo = objEntitySource.i_IdVentaDetalleAnexo;
                    pobjOperationResult.Success = 1;
                    return pobjDtoEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.ActualizarVentaDetalleAnexo()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }
        [Obsolete]
        public string AnularGuiaRemisionPorId(ref OperationResult pobjOperationResult, string IdGuiaRemision, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    MovimientoBL _objMovimientoBL = new MovimientoBL();
                    DocumentoBL _objDocumentoBL = new DocumentoBL();
                    OperationResult objOperationResult = new OperationResult();
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    #region Actualiza Cabecera

                    var objEntitySource = (from a in dbContext.guiaremision
                                           where a.v_IdGuiaRemision == IdGuiaRemision
                                           select a).FirstOrDefault();
                    guiaremisionDto pobjDtoEntity = guiaremisionAssembler.ToDTO(objEntitySource);
                    pobjDtoEntity.i_IdEstado = 0;
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    guiaremision objEntity = guiaremisionAssembler.ToEntity(pobjDtoEntity);

                    dbContext.guiaremision.ApplyCurrentValues(objEntity);

                    #region EliminaNotaSalida
                    if (_objDocumentoBL.DocumentoGeneraStock(objEntitySource.i_IdTipoGuia.Value))
                    {
                        var NotaSalida = (from a in dbContext.movimiento
                                          where a.v_OrigenTipo == "G" && a.v_OrigenRegPeriodo == objEntitySource.v_Periodo && a.v_OrigenRegMes == objEntitySource.v_Mes.Trim() && a.v_OrigenRegCorrelativo == objEntitySource.v_Correlativo.Trim()
                                          && a.i_Eliminado == 0

                                          select new

                                          {
                                              IdMovimiento = a.v_IdMovimiento
                                          }).FirstOrDefault();

                        if (NotaSalida != null && NotaSalida.IdMovimiento != null)
                        {

                            _objMovimientoBL.EliminarMovimiento(ref objOperationResult, NotaSalida.IdMovimiento, Globals.ClientSession.GetAsList());

                        }
                    }

                    #endregion

                    dbContext.SaveChanges();
                    ts.Complete();
                    pobjOperationResult.Success = 1;
                    return objEntitySource.v_IdGuiaRemision;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                return null;

            }
        }

        public string AnularVentaPorId(ref OperationResult pobjOperationResult, string IdVenta, List<string> ClientSession)
        {
            try
            {

                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    MovimientoBL _objMovimientoBL = new MovimientoBL();
                    OperationResult objOperationResult = new OperationResult();
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    #region Actualiza Cabecera

                    var objEntitySource = (from a in dbContext.venta
                                           where a.v_IdVenta == IdVenta
                                           select a).FirstOrDefault();
                    ventaDto pobjDtoEntity = ventaAssembler.ToDTO(objEntitySource);
                    pobjDtoEntity.i_IdEstado = 0;
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    venta objEntity = ventaAssembler.ToEntity(pobjDtoEntity);

                    dbContext.venta.ApplyCurrentValues(objEntity);

                    #region EliminaNotaSalida
                    var notaSalidas = (from a in dbContext.movimiento
                                       where a.v_IdMovimientoOrigen.Equals(IdVenta)
                                       && a.i_Eliminado == 0
                                       select new
                                       {
                                           IdMovimiento = a.v_IdMovimiento
                                       }).ToList();

                    foreach (var notaSalida in notaSalidas)
                    {
                        if (notaSalida != null && notaSalida.IdMovimiento != null)
                        {
                            _objMovimientoBL.EliminarMovimiento(ref objOperationResult, notaSalida.IdMovimiento, Globals.ClientSession.GetAsList());
                        }
                    }

                    #endregion

                    #region EliminarCobranzaPendiente
                    EliminarCobranzaPendiente(ref pobjOperationResult, pobjDtoEntity.v_IdVenta,
                            Globals.ClientSession.GetAsList(), pobjDtoEntity.i_IdEstado.Value, 0);

                    #endregion

                    dbContext.SaveChanges();
                    ts.Complete();
                    pobjOperationResult.Success = 1;
                    return objEntitySource.v_IdVenta;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                return null;

            }
        }

        public void ActualizarVenta(ref OperationResult pobjOperationResult, ventaDto pobjDtoEntity,
            List<string> ClientSession, List<ventadetalleDto> pTemp_Insertar, List<ventadetalleDto> pTemp_Editar,
            List<ventadetalleDto> pTemp_Eliminar, List<nbs_ventakardexDto> pTemp_InsertarKardex = null,
            List<nbs_ventakardexDto> _pTemp_ModificarKardex = null,
            List<nbs_ventakardexDto> _pTemp_EliminarKardex = null, bool _guardadoSinProceso = false, bool EliminarCobranza = true)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    venta objEntityVenta = ventaAssembler.ToEntity(pobjDtoEntity);
                    ventadetalleDto pobjDtoVentaDetalle = new ventadetalleDto();
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    DocumentoBL _objDocumentoBL = new DocumentoBL();
                    int SecuentialId = 0;
                    string newIdVentaDetalle = string.Empty;
                    int intNodeId;

                    #region Actualiza Cabecera
                    intNodeId = int.Parse(ClientSession[0]);
                    var objEntitySource = (from a in dbContext.venta
                                           where a.v_IdVenta == pobjDtoEntity.v_IdVenta
                                           select a).FirstOrDefault();
                    var objCabecera = objEntitySource.ToDTO();

                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    venta objEntity = ventaAssembler.ToEntity(pobjDtoEntity);
                    dbContext.venta.ApplyCurrentValues(objEntity);

                    #endregion

                    #region Actualiza Detalle

                    foreach (ventadetalleDto ventadetalleDto in pTemp_Insertar)
                    {
                        movimientodetalleDto NotadeSalidaDetalle = new movimientodetalleDto();

                        if (Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta == 1)
                        {
                            NotadeSalidaDetalle = (from n in dbContext.movimientodetalle

                                                   join J1 in dbContext.movimiento on
                                                       new { idMovimiento = n.v_IdMovimiento, IdAlmacen = ventadetalleDto.i_IdAlmacen }
                                                       equals new { idMovimiento = J1.v_IdMovimiento, IdAlmacen = J1.i_IdAlmacenOrigen } into
                                                       J1_join
                                                   from J1 in J1_join.DefaultIfEmpty()

                                                   where
                                                       n.i_Eliminado == 0 && n.v_IdProductoDetalle == ventadetalleDto.v_IdProductoDetalle &&
                                                       J1.v_OrigenTipo == "V" && J1.v_OrigenRegPeriodo == objEntityVenta.v_Periodo
                                                       && J1.v_OrigenRegMes == objEntityVenta.v_Mes &&
                                                       J1.v_OrigenRegCorrelativo == objEntityVenta.v_Correlativo && n.v_NroPedido == ventadetalleDto.v_PedidoExportacion

                                                   select new movimientodetalleDto { v_IdMovimientoDetalle = n.v_IdMovimientoDetalle }).FirstOrDefault();
                        }
                        else
                        {

                            NotadeSalidaDetalle = (from n in dbContext.movimientodetalle

                                                   join J1 in dbContext.movimiento on
                                                       new { idMovimiento = n.v_IdMovimiento, IdAlmacen = ventadetalleDto.i_IdAlmacen }
                                                       equals new { idMovimiento = J1.v_IdMovimiento, IdAlmacen = J1.i_IdAlmacenOrigen } into
                                                       J1_join
                                                   from J1 in J1_join.DefaultIfEmpty()

                                                   where
                                                       n.i_Eliminado == 0 && n.v_IdProductoDetalle == ventadetalleDto.v_IdProductoDetalle &&
                                                       J1.v_OrigenTipo == "V" && J1.v_OrigenRegPeriodo == objEntityVenta.v_Periodo
                                                       && J1.v_OrigenRegMes == objEntityVenta.v_Mes &&
                                                       J1.v_OrigenRegCorrelativo == objEntityVenta.v_Correlativo

                                                   select new movimientodetalleDto { v_IdMovimientoDetalle = n.v_IdMovimientoDetalle }).FirstOrDefault();
                        }

                        ventadetalle objEntityVentaDetalle = ventadetalleAssembler.ToEntity(ventadetalleDto);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 32);
                        newIdVentaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZY");
                        objEntityVentaDetalle.v_IdVentaDetalle = newIdVentaDetalle;
                        objEntityVentaDetalle.v_IdMovimientoDetalle = NotadeSalidaDetalle != null
                            ? NotadeSalidaDetalle.v_IdMovimientoDetalle
                            : null;
                        objEntityVentaDetalle.t_InsertaFecha = DateTime.Now;
                        objEntityVentaDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityVentaDetalle.i_Eliminado = 0;
                        dbContext.AddToventadetalle(objEntityVentaDetalle);
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName,
                            "ventadetalle", newIdVentaDetalle);



                        if (ventadetalleDto.v_IdFormatoUnicoFacturacionDetalle != null)
                        {

                            nbs_formatounicofacturaciondetalle FormatoUnicoFactDetalle = (from a in dbContext.nbs_formatounicofacturaciondetalle
                                                                                          where a.i_Eliminado == 0 && a.v_IdFormatoUnicoFacturacionDetalle == ventadetalleDto.v_IdFormatoUnicoFacturacionDetalle
                                                                                          select a).FirstOrDefault();

                            if (FormatoUnicoFactDetalle != null)
                            {
                                FormatoUnicoFactDetalle.i_UsadoVenta = 0;
                                FormatoUnicoFactDetalle.t_ActualizaFecha = DateTime.Now;
                                FormatoUnicoFactDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                dbContext.nbs_formatounicofacturaciondetalle.ApplyCurrentValues(FormatoUnicoFactDetalle);
                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName,
                                "nbs_formatounicofacturaciondetalle", ventadetalleDto.v_IdFormatoUnicoFacturacionDetalle);
                            }


                        }
                    }

                    foreach (ventadetalleDto ventadetalleDto in pTemp_Editar)
                    {
                        ventadetalle _objEntity = ventadetalleAssembler.ToEntity(ventadetalleDto);
                        var query = (from n in dbContext.ventadetalle
                                     where n.v_IdVentaDetalle == ventadetalleDto.v_IdVentaDetalle
                                     select n).FirstOrDefault();
                        movimientodetalleDto NotadeSalidaDetalle = new movimientodetalleDto();
                        if (Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta == 1)
                        {


                            NotadeSalidaDetalle = (from n in dbContext.movimientodetalle

                                                   join J1 in dbContext.movimiento on
                                                       new { idMovimiento = n.v_IdMovimiento, IdAlmacen = ventadetalleDto.i_IdAlmacen }
                                                       equals new { idMovimiento = J1.v_IdMovimiento, IdAlmacen = J1.i_IdAlmacenOrigen } into
                                                       J1_join
                                                   from J1 in J1_join.DefaultIfEmpty()

                                                   where
                                                       n.i_Eliminado == 0 && n.v_IdProductoDetalle == ventadetalleDto.v_IdProductoDetalle &&
                                                       J1.v_OrigenTipo == "V" && J1.v_OrigenRegPeriodo == objEntityVenta.v_Periodo
                                                       && J1.v_OrigenRegMes == objEntityVenta.v_Mes && n.v_NroPedido == ventadetalleDto.v_PedidoExportacion &&
                                                       J1.v_OrigenRegCorrelativo == objEntityVenta.v_Correlativo

                                                   select new movimientodetalleDto { v_IdMovimientoDetalle = n.v_IdMovimientoDetalle }).FirstOrDefault();
                        }
                        else
                        {


                            NotadeSalidaDetalle = (from n in dbContext.movimientodetalle

                                                   join J1 in dbContext.movimiento on
                                                       new { idMovimiento = n.v_IdMovimiento, IdAlmacen = ventadetalleDto.i_IdAlmacen }
                                                       equals new { idMovimiento = J1.v_IdMovimiento, IdAlmacen = J1.i_IdAlmacenOrigen } into
                                                       J1_join
                                                   from J1 in J1_join.DefaultIfEmpty()

                                                   where
                                                       n.i_Eliminado == 0 && n.v_IdProductoDetalle == ventadetalleDto.v_IdProductoDetalle &&
                                                       J1.v_OrigenTipo == "V" && J1.v_OrigenRegPeriodo == objEntityVenta.v_Periodo
                                                       && J1.v_OrigenRegMes == objEntityVenta.v_Mes &&
                                                       J1.v_OrigenRegCorrelativo == objEntityVenta.v_Correlativo

                                                   select new movimientodetalleDto { v_IdMovimientoDetalle = n.v_IdMovimientoDetalle }).FirstOrDefault();

                        }
                        if (NotadeSalidaDetalle != null)
                            _objEntity.v_IdMovimientoDetalle = NotadeSalidaDetalle.v_IdMovimientoDetalle;
                        _objEntity.t_ActualizaFecha = DateTime.Now;
                        _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                        dbContext.ventadetalle.ApplyCurrentValues(_objEntity);
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName,
                            "ventadetalle", query.v_IdVentaDetalle);
                    }

                    foreach (ventadetalleDto ventadetalleDto in pTemp_Eliminar)
                    {
                        ventadetalle _objEntity = ventadetalleAssembler.ToEntity(ventadetalleDto);
                        var query = (from n in dbContext.ventadetalle
                                     where n.v_IdVentaDetalle == ventadetalleDto.v_IdVentaDetalle
                                     select n).FirstOrDefault();

                        if (query != null)
                        {
                            query.t_ActualizaFecha = DateTime.Now;
                            query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            query.i_Eliminado = 1;
                            dbContext.ventadetalle.ApplyCurrentValues(query);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                                "ventadetalle", query.v_IdVentaDetalle);
                        }
                    }

                    #endregion

                    #region ActualizaDetalleKardex

                    if (pTemp_InsertarKardex != null)
                    {
                        foreach (nbs_ventakardexDto objKardex in pTemp_InsertarKardex)
                        {
                            nbs_ventakardex objEntityVentaKardex = nbs_ventakardexAssembler.ToEntity(objKardex);

                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 105);
                            string newIdKardex = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "KX");
                            objEntityVentaKardex.v_IdVentaKardex = newIdKardex;
                            objEntityVentaKardex.v_IdVenta = pobjDtoEntity.v_IdVenta;
                            objEntityVentaKardex.t_InsertaFecha = DateTime.Now;
                            objEntityVentaKardex.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityVentaKardex.i_Eliminado = 0;
                            dbContext.AddTonbs_ventakardex(objEntityVentaKardex);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName,
                                "nbs_ventakardex", newIdKardex);
                        }


                    }


                    if (_pTemp_ModificarKardex != null)
                    {

                        foreach (nbs_ventakardexDto objKardex in _pTemp_ModificarKardex)
                        {
                            nbs_ventakardex _objEntity = nbs_ventakardexAssembler.ToEntity(objKardex);
                            var query = (from n in dbContext.nbs_ventakardex
                                         where n.v_IdVentaKardex == objKardex.v_IdVentaKardex
                                         select n).FirstOrDefault();

                            _objEntity.t_ActualizaFecha = DateTime.Now;
                            _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            dbContext.nbs_ventakardex.ApplyCurrentValues(_objEntity);
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName,
                                "nbs_ventakardex", query.v_IdVentaKardex);
                        }
                    }

                    if (_pTemp_EliminarKardex != null)
                    {
                        foreach (nbs_ventakardexDto objKardex in _pTemp_EliminarKardex)
                        {
                            nbs_ventakardex _objEntity = nbs_ventakardexAssembler.ToEntity(objKardex);

                            var query = (from n in dbContext.nbs_ventakardex
                                         where n.v_IdVentaKardex == objKardex.v_IdVentaKardex
                                         select n).FirstOrDefault();

                            if (query != null)
                            {
                                query.t_ActualizaFecha = DateTime.Now;
                                query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                query.i_Eliminado = 1;
                            }

                            dbContext.nbs_ventakardex.ApplyCurrentValues(query);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                                "ventadetalle", query.v_IdVentaKardex);
                        }
                    }

                    dbContext.SaveChanges();

                    #endregion

                    #region Genera Cobranza Pendiente
                    if (EliminarCobranza)
                    {
                        if (pobjDtoEntity.v_IdVenta != null && _guardadoSinProceso != true)
                        {
                            EliminarCobranzaPendiente(ref pobjOperationResult, pobjDtoEntity.v_IdVenta,
                                Globals.ClientSession.GetAsList(), pobjDtoEntity.i_IdEstado.Value, 0);
                            if (objEntityVenta.i_IdEstado == 1)
                                InsertarCobranzaPendiente(ref pobjOperationResult, pobjDtoEntity.v_IdVenta,
                                    objEntityVenta.d_Total.Value, Globals.ClientSession.GetAsList());

                            if (pobjOperationResult.Success == 0) return;

                        }
                    }

                    #endregion

                    #region Actualiza Correlativo EmpresaDetalle si fue insertada sin procesar

                    if (_guardadoSinProceso != true && string.IsNullOrEmpty(objCabecera.v_SerieDocumento) &&
                        string.IsNullOrEmpty(objCabecera.v_CorrelativoDocumento))
                    {
                        _objDocumentoBL.ActualizarCorrelativoPorSerie(ref pobjOperationResult,
                            Globals.ClientSession.i_IdEstablecimiento, objEntityVenta.i_IdTipoDocumento.Value,
                            objEntityVenta.v_SerieDocumento, int.Parse(objEntityVenta.v_CorrelativoDocumento) + 1);
                        if (pobjOperationResult.Success == 0) return;
                    }

                    #endregion

                    #region Regenera asiento contable
                    if (_objDocumentoBL.DocumentoEsContable(pobjDtoEntity.i_IdTipoDocumento.Value) &&
                                    pobjDtoEntity.i_IdEstado == 1)
                    {
                        RegenerarAsientoContable(ref pobjOperationResult, pobjDtoEntity.v_IdVenta);
                        if (pobjOperationResult.Success == 0) return;
                    }
                    else
                    {
                        EliminarAsientoContable(ref pobjOperationResult, pobjDtoEntity.v_IdVenta);
                        if (pobjOperationResult.Success == 0) return;
                    }
                    #endregion

                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "venta",
                        objEntityVenta.v_IdVenta);

                    #region Genera la salida a almacén

                    RegenerarSalidasAlmacen(ref pobjOperationResult, objEntitySource.v_IdVenta);
                    //EliminarSalidaAlmacenyRegenerar(ref pobjOperationResult, objEntitySource.v_IdVenta);
                    if (pobjOperationResult.Success == 0) return;
                    #endregion

                   

                    pobjOperationResult.Success = 1;
                    ts.Complete();                    
                }

                if (!string.IsNullOrEmpty(pobjDtoEntity.v_SigesoftServiceId))
                {
                    var servicios = ObtenerServicios(pobjDtoEntity.v_SigesoftServiceId);
                    foreach (var serv in servicios)
                    {
                        var tipo = pobjDtoEntity.i_FacturacionCliente == 0 ? TipoFacturacion.Aseguradora : TipoFacturacion.Asistencial;
                        var tipoDespacho = pobjDtoEntity.i_IdEstado == 1 ? TipoAccionFacturacion.Facturar : TipoAccionFacturacion.Anular;
                        FacturacionServiciosBl.ProcesarServicio(tipo, tipoDespacho, serv, pobjDtoEntity.v_IdVenta);
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.ActualizarVenta()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void EliminarVenta(ref OperationResult pobjOperationResult, string pstrIdVenta,
            List<string> ClientSession, string motivo = null)
        {
            try
            {
                venta objEntitySource;
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        #region Elimina Cabecera

                        // Obtener la entidad fuente
                        objEntitySource = (from a in dbContext.venta
                                               where a.v_IdVenta == pstrIdVenta
                                               select a).FirstOrDefault();

                        // Crear la entidad con los datos actualizados
                        objEntitySource.v_MotivoEliminacion = motivo;
                        objEntitySource.t_ActualizaFecha = DateTime.Now;
                        objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntitySource.i_Eliminado = 1;

                        #endregion
                        #region Restaura Cabecer FUF

                        if (objEntitySource.v_IdFormatoUnicoFacturacion != null)
                        {
                            var objEntityFUF = (from a in dbContext.nbs_formatounicofacturacion
                                                where a.v_IdFormatoUnicoFacturacion == objEntitySource.v_IdFormatoUnicoFacturacion
                                                select a).FirstOrDefault();
                            if (objEntityFUF != null)
                            {
                                objEntityFUF.t_ActualizaFecha = DateTime.Now;
                                objEntityFUF.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                objEntityFUF.i_Facturado = 0;
                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "nbs_formatounicofacturacion", objEntityFUF.v_IdFormatoUnicoFacturacion);
                            }
                        }

                        #endregion

                        #region Elimina Detalles

                        //Eliminar detalles del movimiento eliminado.
                        var objEntitySourceDetallesVenta = (from a in dbContext.ventadetalle
                                                            where a.v_IdVenta == pstrIdVenta
                                                            select a).ToList();

                        foreach (var RegistroVentaDetalle in objEntitySourceDetallesVenta)
                        {
                            RegistroVentaDetalle.t_ActualizaFecha = DateTime.Now;
                            RegistroVentaDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            RegistroVentaDetalle.i_Eliminado = 1;
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                                "ventadetalle", RegistroVentaDetalle.v_IdVentaDetalle);

                            #region restauraFormatoUnicoFacturacionDetalles
                            if (objEntitySource.v_IdFormatoUnicoFacturacion != null)
                            {


                                var objEntityFUFDetalles = (from a in dbContext.nbs_formatounicofacturaciondetalle
                                                            where a.v_IdFormatoUnicoFacturacion == objEntitySource.v_IdFormatoUnicoFacturacion && a.v_IdProductoDetalle == RegistroVentaDetalle.v_IdProductoDetalle && a.i_Eliminado == 0
                                                            select a).FirstOrDefault();

                                if (objEntityFUFDetalles != null)
                                {
                                    objEntityFUFDetalles.t_ActualizaFecha = DateTime.Now;
                                    objEntityFUFDetalles.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                    objEntityFUFDetalles.i_UsadoVenta = 0;
                                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName,
                                        "nbs_formatounicofacturaciondetalle", objEntityFUFDetalles.v_IdFormatoUnicoFacturacionDetalle);
                                }

                            }
                            #endregion
                        }

                        #endregion

                        #region Elimina Detalles Venta Kardex

                        var objVentaKardex = (from a in dbContext.nbs_ventakardex

                                              where a.i_Eliminado == 0 && a.v_IdVenta == pstrIdVenta

                                              select a).ToList();

                        foreach (var ventaKardex in objVentaKardex)
                        {
                            ventaKardex.t_ActualizaFecha = DateTime.Now;
                            ventaKardex.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            ventaKardex.i_Eliminado = 1;
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                                "nbs_ventakardex", ventaKardex.v_IdVentaKardex);
                        }


                        #endregion

                        #region Elimina la salida a almacén
                        EliminarSalidaAlmacen(ref pobjOperationResult, objEntitySource.v_IdVenta);
                        if (pobjOperationResult.Success == 0) return;
                        #endregion

                        #region Elimina Cobranza Pendiente

                        if (objEntitySource.i_IdEstado != 0)
                        {
                            EliminarCobranzaPendiente(ref pobjOperationResult, pstrIdVenta, ClientSession,
                                objEntitySource.i_IdEstado.Value, 1);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        #endregion

                        #region Elimina Libro Diario

                        if (objEntitySource.i_IdEstado != 0)
                        {
                            DiarioBL _objDiarioBL = new DiarioBL();

                            _objDiarioBL.EliminarDiarioXDocRef(ref pobjOperationResult, pstrIdVenta, ClientSession, true);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        #endregion

                        #region Restaura Pedido

                        if (objEntitySource.i_IdTipoDocumento != null &&
                            (objEntitySource.i_IdEstado != 0 &&
                             !_objDocumentoBL.DocumentoEsInverso(objEntitySource.i_IdTipoDocumento.Value) &&
                             objEntitySource.i_IdTipoDocumento.Value != 8))
                        {
                            RestauraPedido(ref pobjOperationResult, objEntitySource.v_NroPedido, ClientSession, pstrIdVenta);
                            if (pobjOperationResult.Success == 0) return;
                        }

                        #endregion

                        dbContext.SaveChanges();                        

                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                            "venta", pstrIdVenta);
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }

                if (!string.IsNullOrEmpty(objEntitySource.v_SigesoftServiceId))
                {
                    var servicios = ObtenerServicios(objEntitySource.v_SigesoftServiceId);
                    foreach (var serv in servicios)
                    {
                        var tipo = objEntitySource.i_FacturacionCliente == 0 ? TipoFacturacion.Aseguradora : TipoFacturacion.Asistencial;
                        var tipoDespacho = TipoAccionFacturacion.Anular;
                        FacturacionServiciosBl.ProcesarServicio(tipo, tipoDespacho, serv, objEntitySource.v_IdVenta);
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.EliminarVenta()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public string[] DevolverClientePorNroDocumento(ref OperationResult pobjOperationResult, string NroDocumento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = (from n in dbContext.cliente
                                 where n.i_Eliminado == 0 && n.v_FlagPantalla == "C" && n.v_NroDocIdentificacion == NroDocumento
                                 select n
                    ).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    if (query != null)
                    {
                        var DireccionCliente = dbContext.clientedirecciones.Where(o => o.i_EsDireccionPredeterminada == 1 && o.i_Eliminado == 0 && o.v_IdCliente == query.v_IdCliente).FirstOrDefault();
                        string[] Cadena = new string[7];
                        Cadena[0] = query.v_IdCliente;
                        Cadena[1] = query.v_CodCliente;
                        Cadena[2] =
                        (query.v_PrimerNombre + " " + query.v_ApePaterno + " " + query.v_ApeMaterno + " " +
                         query.v_RazonSocial).Trim();
                        //Cadena[3] = query.v_DirecPrincipal;
                        Cadena[3] = DireccionCliente != null ? DireccionCliente.v_Direccion : query.v_DirecPrincipal;
                        Cadena[4] = query.i_IdTipoPersona.Value == null ? "-1" : query.i_IdTipoPersona.Value.ToString();
                        Cadena[5] = query.i_IdTipoIdentificacion == null ? "-1" : query.i_IdTipoIdentificacion.Value.ToString();
                        Cadena[6] = DireccionCliente != null ? DireccionCliente.i_IdDireccionCliente.ToString() : "-1";
                        return Cadena;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.DevolverClientePorNroDocumento()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public string[] DevolverClientePorIdCliente(ref OperationResult pobjOperationResult, string IdCliente)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = (from n in dbContext.cliente
                                 where n.i_Eliminado == 0 /*&& n.v_FlagPantalla == "C"*/&& n.v_IdCliente == IdCliente
                                 select n
                    ).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    if (query != null)
                    {
                        string[] Cadena = new string[4];
                        Cadena[0] = query.v_NroDocIdentificacion;
                        Cadena[1] = query.v_CodCliente;
                        Cadena[2] =
                        (query.v_ApePaterno + " " + query.v_ApeMaterno + " " + query.v_PrimerNombre + " " +
                         query.v_RazonSocial).Trim();
                        Cadena[3] = query.v_DirecPrincipal;
                        return Cadena;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.DevolverClientePorIdCliente()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public string DevolverTipoCambioPorFecha(ref OperationResult pobjOperationResult, DateTime Fecha)
        {
            try
            {
                var query = new TipoCambioBL().DevolverTipoCambioPorFechaVenta(ref pobjOperationResult, Fecha);
                if (pobjOperationResult.Success == 0) return "0";
                return query;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<KeyValueDTO> ObtenerConceptosParaCombo(ref OperationResult pobjOperationResult, int pintIdArea,
            string pstrSortExpression)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from a in dbContext.concepto

                                 join A in dbContext.administracionconceptos on a.v_Codigo equals A.v_Codigo into A_join
                                 from A in A_join.DefaultIfEmpty()

                                 where a.i_IdArea == pintIdArea && a.i_Eliminado == 0 && a.v_Periodo.Equals(periodo)
                                 select new
                                 {
                                     v_Codigo = a.v_Codigo,
                                     v_Nombre = a.v_Nombre,
                                     CuentaVenta = A.v_CuentaPVenta
                                 }
                    );

                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }
                    else
                    {
                        query = query.OrderBy("v_Nombre");
                    }

                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                            {
                                Id = x.v_Codigo,
                                Value1 = x.v_Codigo + " | " + x.v_Nombre,
                                Value2 = x.CuentaVenta
                            }
                        ).ToList();

                    pobjOperationResult.Success = 1;
                    return query2;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public bool ComprobarExistenciaCorrelativoDocumento(ref OperationResult pobjOperationResult,
            int pintIdTipoDocumento, string pstrSerieDoc, string pstrCorrelativoDoc, string pstrIdVenta)
        {
            try
            {
                if (pintIdTipoDocumento != -1 && pstrSerieDoc != null && pstrCorrelativoDoc != null)
                // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (pstrIdVenta == null)
                        // SI el idVenta es nulo se esta consultado de una venta nueva que no ha sido guardada
                        {
                            var query = (from n in dbContext.venta
                                         where n.i_Eliminado == 0 && n.i_IdEstado == 1
                                               && n.i_IdTipoDocumento == pintIdTipoDocumento
                                               && n.v_SerieDocumento == pstrSerieDoc &&
                                               n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                         select n
                            ).FirstOrDefault();
                            return query != null;
                        }
                        else // si no es nulo se comprueba de una venta que esta siendo modificada
                        {
                            var query = (from n in dbContext.venta
                                         where n.i_Eliminado == 0 && n.i_IdEstado == 1
                                               && n.i_IdTipoDocumento == pintIdTipoDocumento
                                               && n.v_SerieDocumento == pstrSerieDoc &&
                                               n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                               && n.v_IdVenta != pstrIdVenta
                                         select n
                            ).FirstOrDefault();
                            return query != null;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return false;
            }
        }

        public string ObtieneIdPorCorrelativoDocumento(ref OperationResult pobjOperationResult,
            int pintIdTipoDocumento, string pstrSerieDoc, string pstrCorrelativoDoc)
        {
            try
            {
                // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                if (pintIdTipoDocumento != -1 && pstrSerieDoc != null && pstrCorrelativoDoc != null)
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var query = (from n in dbContext.venta
                                     where n.i_Eliminado == 0
                                           && n.i_IdTipoDocumento == pintIdTipoDocumento
                                           && n.v_SerieDocumento == pstrSerieDoc &&
                                           n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                     select n).FirstOrDefault();
                        return query != null ? query.v_IdVenta : string.Empty;

                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return string.Empty;
            }
        }

        public bool VentaYaTieneNRC(ref OperationResult pobjOperationResult, int pintTipoDocRef, string pstrNroSerieRef,
            string pstrNroCorrelativoRef)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result =
                        dbContext.venta.Any(
                            p =>
                                p.i_Eliminado == 0 &&
                                p.i_IdTipoDocumento == 7 && p.i_IdTipoDocumentoRef == pintTipoDocRef &&
                                p.v_SerieDocumentoRef.Trim() == pstrNroSerieRef.Trim() &&
                                p.v_CorrelativoDocumentoRef.Trim() == pstrNroCorrelativoRef.Trim() && p.i_IdEstado == 1);
                    pobjOperationResult.Success = 1;
                    return result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.VentaYaTieneNRC()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public string DevolverIdVenta(ref OperationResult pobjOperationResult, int pintIdTipoDocumento,
            string pstrSerieDoc, string pstrCorrelativoDoc)
        {
            try
            {
                if (pintIdTipoDocumento != -1 && pstrSerieDoc != null && pstrCorrelativoDoc != null)
                // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var query = (from n in dbContext.venta
                                     where n.i_Eliminado == 0
                                           && n.i_IdTipoDocumento == pintIdTipoDocumento
                                           && n.v_SerieDocumento == pstrSerieDoc.Trim() &&
                                           n.v_CorrelativoDocumento == pstrCorrelativoDoc.Trim()
                                     select new { v_IdVenta = n.v_IdVenta }).FirstOrDefault();

                        pobjOperationResult.Success = 1;

                        if (query != null)
                            return query.v_IdVenta;
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.DevolverIdVenta()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                string ReplicationID = Globals.ClientSession.ReplicationNodeID;
                var Registro = (from n in dbContext.venta
                                where
                                n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo &&
                                n.v_IdVenta.Substring(0, 1) == ReplicationID
                                select n).FirstOrDefault();

                return Registro == null;
            }
        }

        public bool ExisteDocumento(int TipoDocumento, string pstrSerie, string pstrCorrelativo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var Registro = (from n in dbContext.venta
                                where
                                n.i_Eliminado == 0 && n.v_SerieDocumento == pstrSerie && n.v_CorrelativoDocumento == pstrCorrelativo &&
                                n.i_IdTipoDocumento == TipoDocumento && n.i_IdEstado == 1
                                select n).FirstOrDefault();

                return (Registro == null);
            }
        }

        public void InsertarCobranzaPendiente(ref OperationResult pobjOperationResult, string IdVenta,
            decimal TotalVenta, List<string> ClientSession)
        {
            int SecuentialId = 0;
            string newId = string.Empty;
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    cobranzapendienteDto objCobranzaPendienteEntity = new cobranzapendienteDto();
                    cobranzapendiente objEntity = objCobranzaPendienteEntity.ToEntity();
                    venta objEntidadVenta = dbContext.venta.FirstOrDefault(p => p.v_IdVenta == IdVenta);
                    decimal SaldoProcesadoDocumentoRefrerencia = 0;

                    #region Procesa casos en que la venta sea nota de crédito

                    if (_objDocumentoBL.DocumentoEsInverso(objEntidadVenta.i_IdTipoDocumento.Value) &&
                        objEntidadVenta.i_IdEstado == 1)
                    {
                        string IdVentaReferencia = DevolverIdVenta(ref pobjOperationResult,
                            objEntidadVenta.i_IdTipoDocumentoRef.Value, objEntidadVenta.v_SerieDocumentoRef,
                            objEntidadVenta.v_CorrelativoDocumentoRef);

                        if (!string.IsNullOrEmpty(IdVentaReferencia))
                        {
                            decimal TCambio = objEntidadVenta.d_TipoCambio.Value;
                            cobranzapendiente _CobranzaPendienteEntity = (from n in dbContext.cobranzapendiente
                                                                          where n.v_IdVenta == IdVentaReferencia && n.i_Eliminado == 0
                                                                          select n).FirstOrDefault();

                            if (_CobranzaPendienteEntity != null)
                            {
                                if (_CobranzaPendienteEntity.d_Saldo >= objEntidadVenta.d_Total.Value)
                                {
                                    int Moneda = (from m in dbContext.venta
                                                  where m.v_IdVenta == IdVentaReferencia && m.i_Eliminado == 0
                                                  select new { m.i_IdMoneda }).FirstOrDefault().i_IdMoneda.Value;

                                    switch (objEntidadVenta.i_IdMoneda)
                                    {
                                        case 1:
                                            switch (Moneda)
                                            {
                                                case 1:
                                                    _CobranzaPendienteEntity.d_Acuenta =
                                                        _CobranzaPendienteEntity.d_Acuenta.Value +
                                                        objEntidadVenta.d_Total.Value;
                                                    _CobranzaPendienteEntity.d_Saldo =
                                                        _CobranzaPendienteEntity.d_Saldo.Value -
                                                        objEntidadVenta.d_Total.Value >= 0
                                                            ? _CobranzaPendienteEntity.d_Saldo.Value -
                                                              objEntidadVenta.d_Total.Value
                                                            : 0;
                                                    break;

                                                case 2:
                                                    decimal Monto;
                                                    Monto =
                                                        decimal.Parse(
                                                            Math.Round(
                                                                _CobranzaPendienteEntity.d_Saldo.Value -
                                                                (objEntidadVenta.d_Total.Value / TCambio), 1,
                                                                MidpointRounding.AwayFromZero).ToString("0.00"));
                                                    _CobranzaPendienteEntity.d_Acuenta =
                                                        _CobranzaPendienteEntity.d_Acuenta.Value +
                                                        (objEntidadVenta.d_Total.Value / TCambio);
                                                    _CobranzaPendienteEntity.d_Saldo = Monto >= 0 ? Monto : 0;
                                                    break;
                                            }
                                            break;

                                        case 2:
                                            switch (Moneda)
                                            {
                                                case 1:
                                                    decimal Monto;
                                                    Monto =
                                                        decimal.Parse(
                                                            Math.Round(
                                                                _CobranzaPendienteEntity.d_Saldo.Value -
                                                                (objEntidadVenta.d_Total.Value * TCambio), 1,
                                                                MidpointRounding.AwayFromZero).ToString("0.00"));
                                                    _CobranzaPendienteEntity.d_Acuenta =
                                                        _CobranzaPendienteEntity.d_Acuenta.Value +
                                                        (objEntidadVenta.d_Total.Value * TCambio);
                                                    _CobranzaPendienteEntity.d_Saldo = Monto >= 0 ? Monto : 0;
                                                    break;

                                                case 2:
                                                    _CobranzaPendienteEntity.d_Acuenta =
                                                        _CobranzaPendienteEntity.d_Acuenta.Value +
                                                        objEntidadVenta.d_Total.Value;
                                                    _CobranzaPendienteEntity.d_Saldo =
                                                        _CobranzaPendienteEntity.d_Saldo.Value -
                                                        objEntidadVenta.d_Total.Value >= 0
                                                            ? _CobranzaPendienteEntity.d_Saldo.Value -
                                                              objEntidadVenta.d_Total
                                                            : 0;
                                                    break;
                                            }
                                            break;
                                    }

                                    _CobranzaPendienteEntity.t_ActualizaFecha = DateTime.Now;
                                    _CobranzaPendienteEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                                    dbContext.cobranzapendiente.ApplyCurrentValues(_CobranzaPendienteEntity);
                                    SaldoProcesadoDocumentoRefrerencia = objEntidadVenta.d_Total.Value;
                                }
                            }
                            else
                            {
                                pobjOperationResult.Success = 0;
                                pobjOperationResult.ErrorMessage = "No se encontró la Cobranza Pendiente";
                                pobjOperationResult.AdditionalInformation = "CobranzaBL.InsertarCobranza()";
                                return;
                            }
                        }

                    }

                    #endregion

                    objEntity.v_IdVenta = IdVenta;
                    objEntity.d_Acuenta = 0 + SaldoProcesadoDocumentoRefrerencia;
                    objEntity.d_Saldo =
                        decimal.Parse(Math.Round(TotalVenta, 2, MidpointRounding.AwayFromZero).ToString("0.00")) -
                        SaldoProcesadoDocumentoRefrerencia;
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;

                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 46);
                    newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZC");
                    objEntity.v_IdCobranzaPendiente = newId;

                    #region Actualiza Linea de Crédito del Cliente

                    var Cliente = (from n in dbContext.venta
                                   join c in dbContext.cliente on n.v_IdCliente equals c.v_IdCliente into c_join
                                   from c in c_join.DefaultIfEmpty()
                                   where n.v_IdVenta == IdVenta
                                   select c).FirstOrDefault();


                    var Venta = dbContext.venta.FirstOrDefault(p => p.v_IdVenta == IdVenta);

                    if (Cliente != null)
                    {
                        if (Cliente.i_UsaLineaCredito != null && Cliente.i_UsaLineaCredito == 1)
                        {
                            var LineaCredito =
                                dbContext.lineacreditoempresa.FirstOrDefault(p => p.v_IdCliente == Cliente.v_IdCliente);

                            if (LineaCredito != null)
                            {
                                switch (LineaCredito.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Venta.i_IdMoneda)
                                        {
                                            case 1:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value + TotalVenta;
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value -
                                                                       LineaCredito.d_Acuenta.Value;
                                                break;

                                            case 2:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value +
                                                                         (TotalVenta * Venta.d_TipoCambio.Value);
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value -
                                                                       LineaCredito.d_Acuenta.Value;
                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Venta.i_IdMoneda)
                                        {
                                            case 1:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value +
                                                                         (TotalVenta / Venta.d_TipoCambio.Value);
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value -
                                                                       LineaCredito.d_Acuenta.Value;
                                                break;

                                            case 2:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value + TotalVenta;
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value -
                                                                       LineaCredito.d_Acuenta.Value;
                                                break;
                                        }
                                        break;
                                }

                                dbContext.lineacreditoempresa.ApplyCurrentValues(LineaCredito);
                            }
                        }
                    }

                    #endregion

                    dbContext.AddTocobranzapendiente(objEntity);
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName,
                        "cobranzapendiente", newId);

                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.InsertarCobranzaPendiente()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void EliminarCobranzaPendiente(ref OperationResult pobjOperationResult, string pstrIdVenta,
            List<string> ClientSession, int estadoDocumento, int eliminado)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    venta objEntidadVenta = dbContext.venta.Where(p => p.v_IdVenta == pstrIdVenta).FirstOrDefault();
                    cobranzapendiente _cobranzapendienteEntity =
                        (from _CobranzaPendienteEntity in dbContext.cobranzapendiente
                         where
                             _CobranzaPendienteEntity.v_IdVenta == pstrIdVenta &&
                             _CobranzaPendienteEntity.i_Eliminado == 0
                         select _CobranzaPendienteEntity).FirstOrDefault();

                    if (_cobranzapendienteEntity != null)
                    {
                        _cobranzapendienteEntity.t_ActualizaFecha = DateTime.Now;
                        _cobranzapendienteEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        _cobranzapendienteEntity.i_Eliminado = 1;
                        dbContext.cobranzapendiente.ApplyCurrentValues(_cobranzapendienteEntity);
                    }

                    #region Procesa casos en que la venta sea nota de crédito

                    if (objEntidadVenta.i_IdTipoDocumento == 7 &&
                        (eliminado == 1 || objEntidadVenta.i_IdEstado.Value != estadoDocumento))
                        if (_objDocumentoBL.DocumentoEsInverso(objEntidadVenta.i_IdTipoDocumento.Value))
                        {
                            string IdVentaReferencia = DevolverIdVenta(ref pobjOperationResult,
                                objEntidadVenta.i_IdTipoDocumentoRef.Value, objEntidadVenta.v_SerieDocumentoRef,
                                objEntidadVenta.v_CorrelativoDocumentoRef);

                            if (!string.IsNullOrEmpty(IdVentaReferencia))
                            {
                                decimal TCambio = objEntidadVenta.d_TipoCambio.Value;
                                cobranzapendiente _CobranzaPendienteEntity = (from n in dbContext.cobranzapendiente
                                                                              where n.v_IdVenta == IdVentaReferencia && n.i_Eliminado == 0
                                                                              select n).FirstOrDefault();

                                if (_CobranzaPendienteEntity != null)
                                {
                                    int Moneda = (from m in dbContext.venta
                                                  where m.v_IdVenta == IdVentaReferencia && m.i_Eliminado == 0
                                                  select new { m.i_IdMoneda }).FirstOrDefault().i_IdMoneda.Value;


                                    switch (objEntidadVenta.i_IdMoneda)
                                    {
                                        case 1:
                                            switch (Moneda)
                                            {
                                                case 1:
                                                    _CobranzaPendienteEntity.d_Acuenta =
                                                        _CobranzaPendienteEntity.d_Acuenta.Value -
                                                        objEntidadVenta.d_Total.Value;
                                                    _CobranzaPendienteEntity.d_Saldo =
                                                        _CobranzaPendienteEntity.d_Saldo.Value +
                                                        objEntidadVenta.d_Total.Value >= 0
                                                            ? _CobranzaPendienteEntity.d_Saldo.Value +
                                                              objEntidadVenta.d_Total.Value
                                                            : 0;

                                                    break;

                                                case 2:
                                                    _CobranzaPendienteEntity.d_Acuenta =
                                                        _CobranzaPendienteEntity.d_Acuenta.Value -
                                                        (objEntidadVenta.d_Total.Value / TCambio);
                                                    _CobranzaPendienteEntity.d_Saldo =
                                                        _CobranzaPendienteEntity.d_Saldo.Value +
                                                        (objEntidadVenta.d_Total.Value / TCambio) >= 0
                                                            ? _CobranzaPendienteEntity.d_Saldo.Value +
                                                              (objEntidadVenta.d_Total.Value / TCambio)
                                                            : 0;

                                                    break;
                                            }
                                            break;

                                        case 2:
                                            switch (Moneda)
                                            {
                                                case 1:
                                                    _CobranzaPendienteEntity.d_Acuenta =
                                                        _CobranzaPendienteEntity.d_Acuenta.Value -
                                                        (objEntidadVenta.d_Total.Value * TCambio);
                                                    _CobranzaPendienteEntity.d_Saldo =
                                                        _CobranzaPendienteEntity.d_Saldo.Value +
                                                        (objEntidadVenta.d_Total.Value * TCambio) >= 0
                                                            ? _CobranzaPendienteEntity.d_Saldo.Value +
                                                              (objEntidadVenta.d_Total.Value * TCambio)
                                                            : 0;

                                                    break;

                                                case 2:
                                                    _CobranzaPendienteEntity.d_Acuenta =
                                                        _CobranzaPendienteEntity.d_Acuenta.Value -
                                                        objEntidadVenta.d_Total.Value;
                                                    _CobranzaPendienteEntity.d_Saldo =
                                                        _CobranzaPendienteEntity.d_Saldo.Value +
                                                        objEntidadVenta.d_Total.Value >= 0
                                                            ? _CobranzaPendienteEntity.d_Saldo.Value +
                                                              objEntidadVenta.d_Total.Value
                                                            : 0;

                                                    break;
                                            }
                                            break;
                                    }

                                    _CobranzaPendienteEntity.t_ActualizaFecha = DateTime.Now;
                                    _CobranzaPendienteEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                                    dbContext.cobranzapendiente.ApplyCurrentValues(_CobranzaPendienteEntity);
                                }
                                //else
                                //{
                                //    pobjOperationResult.Success = 0;
                                //    pobjOperationResult.ErrorMessage = "No se encontró la Cobranza Pendiente";
                                //    pobjOperationResult.AdditionalInformation = "CobranzaBL.InsertarCobranza()";
                                //    return;
                                //}
                            }

                        }

                    #endregion

                    #region Actualiza Linea de Crédito del Cliente

                    var Cliente = (from n in dbContext.venta
                                   join c in dbContext.cliente on n.v_IdCliente equals c.v_IdCliente into c_join
                                   from c in c_join.DefaultIfEmpty()
                                   where n.v_IdVenta == pstrIdVenta
                                   select c).FirstOrDefault();

                    var Venta = dbContext.venta.Where(p => p.v_IdVenta == pstrIdVenta).FirstOrDefault();

                    if (Cliente != null && Venta != null)
                    {
                        if (Cliente.i_UsaLineaCredito != null && Cliente.i_UsaLineaCredito == 1)
                        {
                            var LineaCredito =
                                dbContext.lineacreditoempresa.Where(p => p.v_IdCliente == Cliente.v_IdCliente)
                                    .FirstOrDefault();

                            if (LineaCredito != null)
                            {
                                switch (LineaCredito.i_IdMoneda)
                                {
                                    case 1:
                                        switch (Venta.i_IdMoneda)
                                        {
                                            case 1:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value -
                                                                         Venta.d_Total.Value;
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value -
                                                                       LineaCredito.d_Acuenta.Value;
                                                break;

                                            case 2:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value -
                                                                         (Venta.d_Total.Value * Venta.d_TipoCambio.Value);
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value -
                                                                       LineaCredito.d_Acuenta.Value;
                                                break;
                                        }
                                        break;

                                    case 2:
                                        switch (Venta.i_IdMoneda)
                                        {
                                            case 1:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value -
                                                                         (Venta.d_Total.Value / Venta.d_TipoCambio.Value);
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value -
                                                                       LineaCredito.d_Acuenta.Value;
                                                break;

                                            case 2:
                                                LineaCredito.d_Acuenta = LineaCredito.d_Acuenta.Value -
                                                                         Venta.d_Total.Value;
                                                LineaCredito.d_Saldo = LineaCredito.d_Credito.Value -
                                                                       LineaCredito.d_Acuenta.Value;
                                                break;
                                        }
                                        break;
                                }

                                dbContext.lineacreditoempresa.ApplyCurrentValues(LineaCredito);
                            }
                        }
                    }

                    #endregion

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.EliminarCobranzaPendiente()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public bool TieneCobranzaPendiente(string pstrIdVenta)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var Cobranza = (from c in dbContext.cobranzapendiente
                            where c.v_IdVenta == pstrIdVenta && c.i_Eliminado == 0
                            select new { c.d_Saldo }).FirstOrDefault();

            return Cobranza != null && Cobranza.d_Saldo != null && Cobranza.d_Saldo > 0;
        }

        public int DevuelveTipoPersona(string IdCLiente)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var IdPersona = (from n in dbContext.cliente
                                 where n.v_IdCliente == IdCLiente
                                 select new { n.i_IdTipoPersona }).FirstOrDefault();

                int Id = IdPersona.i_IdTipoPersona.Value;
                return Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool VerificarStockDisponible(ref OperationResult pobjOperationResult, List<ventadetalleDto> listadoDetalle, string idPedidoExtraccion = null)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    pobjOperationResult.Success = 1;
                    var sb = new StringBuilder();
                    var resultado = true;
                    listadoDetalle = listadoDetalle.Where(p => !string.IsNullOrWhiteSpace(p.v_IdProductoDetalle) &&
                        (p.i_EsServicio ?? 0) == 0 && p.i_ValidarStock == 1).ToList();

                    if (!listadoDetalle.Any()) return true;

                    var agrupadoAlmacen = listadoDetalle.GroupBy(g => g.i_IdAlmacen ?? 1);

                    var idVenta = listadoDetalle.FirstOrDefault().v_IdVenta;

                    #region Obtiene temporales de venta y pedido para tenerlos en cuenta para el cálculo.
                    var ventaDetalleAnterior = !string.IsNullOrEmpty(idVenta) ?
                                   dbContext.ventadetalle.Where(p => p.v_IdVenta.Equals(idVenta) && p.i_Eliminado == 0).ToList() : null;

                    var pedidoDetalleExtraccion = !string.IsNullOrEmpty(idPedidoExtraccion) ?
                        dbContext.pedidodetalle.Where(p => p.i_Eliminado == 0 && p.v_IdPedido.Equals(idPedidoExtraccion)) : null;
                    #endregion

                    foreach (var ventasAlmacen in agrupadoAlmacen)
                    {
                        var idAlmacen = ventasAlmacen.Key;
                        var almacenInfo = new AlmacenBL().ObtenerAlmacen(ref pobjOperationResult, idAlmacen);
                        if (pobjOperationResult.Success == 0) return false;

                        var validarStockAlmacen = almacenInfo != null ? almacenInfo.i_ValidarStockAlmacen ?? 0 : 0;
                        if (validarStockAlmacen == 0) continue;

                        var prodIds = listadoDetalle.Select(p => p.v_IdProductoDetalle).Distinct();
                        var stocks = dbContext.productoalmacen.Where(n => n.i_IdAlmacen == idAlmacen && prodIds.Contains(n.v_ProductoDetalleId)
                            && n.i_Eliminado == 0 && n.v_Periodo.Equals(periodo)).ToDTOs();

                        var agrupadoProductoLoteSerie = ventasAlmacen.GroupBy(g => new
                        {
                            prod = g.v_IdProductoDetalle,
                            lote = (g.v_NroLote ?? "").Trim(),
                            serie = (g.v_NroSerie ?? "").Trim(),
                            pedido = (g.v_PedidoExportacion ?? "").Trim()
                        });

                        foreach (var ventaLoteSerie in agrupadoProductoLoteSerie)
                        {
                            var detalleVenta = ventaLoteSerie.FirstOrDefault();
                            if (detalleVenta == null) continue;

                            var cantidadSolicitada = ventaLoteSerie.Select(p =>
                                Utils.Windows.DevolverTotalUnidades(p.v_IdProductoDetalle, p.d_Cantidad ?? 0, p.i_IdUnidadMedida ?? 5)).Sum();

                            #region Disminuye la cantidad de la venta guardada anteriormente.
                            if (ventaDetalleAnterior != null)
                            {
                                var detalleAnterior = ventaDetalleAnterior.Where(p => p.v_IdProductoDetalle != null && p.i_IdAlmacen == idAlmacen &&
                                                          p.v_IdProductoDetalle.Equals(ventaLoteSerie.Key.prod) &&
                                                          (p.v_NroSerie ?? "").Equals(ventaLoteSerie.Key.serie) &&
                                                          (p.v_NroLote ?? "").Equals(ventaLoteSerie.Key.lote) &&
                                                          (p.v_PedidoExportacion ?? "").Equals(ventaLoteSerie.Key.pedido)).ToList();

                                if (detalleAnterior.Any()) cantidadSolicitada -= detalleAnterior.Sum(s => s.d_CantidadEmpaque ?? 0);
                            }
                            #endregion

                            #region Toma en cuenta la separacion del pedido en actual despacho si es que hay.
                            if (pedidoDetalleExtraccion != null)
                            {
                                var detallePedido = pedidoDetalleExtraccion.Where(p => p.v_IdProductoDetalle != null && p.i_IdAlmacen == idAlmacen &&
                                                          p.v_IdProductoDetalle.Equals(ventaLoteSerie.Key.prod) &&
                                                          (p.v_NroSerie ?? "").Equals(ventaLoteSerie.Key.serie) &&
                                                          (p.v_NroLote ?? "").Equals(ventaLoteSerie.Key.lote)).ToList();

                                if (detallePedido.Any()) cantidadSolicitada -= detallePedido.Sum(s => s.d_CantidadEmpaque ?? 0);
                            }
                            #endregion

                            #region Obtiene el producto almacén correspondiente.
                            var prodAlmacenCorrespondiente = stocks.FirstOrDefault(p => p.v_ProductoDetalleId != null &&
                                                            p.v_ProductoDetalleId.Equals(ventaLoteSerie.Key.prod) &&
                                                            (p.v_NroSerie ?? "").Equals(ventaLoteSerie.Key.serie) &&
                                                            (p.v_NroLote ?? "").Equals(ventaLoteSerie.Key.lote) &&
                                                            (p.v_NroPedido ?? "").Equals(ventaLoteSerie.Key.pedido));
                            #endregion

                            #region Realiza la comprobación de stock
                            if (prodAlmacenCorrespondiente != null)
                            {
                                var cantidadDisponible = (prodAlmacenCorrespondiente.d_StockActual ?? 0) - (prodAlmacenCorrespondiente.d_SeparacionTotal ?? 0);

                                if (cantidadSolicitada > cantidadDisponible)
                                {
                                    resultado = false;
                                    sb.Append(Environment.NewLine);
                                    sb.AppendLine(string.Format("- Cod. Producto: {0} \rRequerido: {1}UND | Disponible: {2}UND", detalleVenta.v_CodigoInterno.ToUpper().Trim(), cantidadSolicitada, cantidadDisponible));
                                    if (!string.IsNullOrEmpty(detalleVenta.v_NroLote))
                                        sb.Append(string.Format(" Lote: {0}", detalleVenta.v_NroLote));

                                    if (!string.IsNullOrEmpty(detalleVenta.v_NroLote))
                                        sb.Append(string.Format(" Serie: {0}", detalleVenta.v_NroSerie));
                                }
                            }
                            #endregion
                        }
                    }

                    pobjOperationResult.AdditionalInformation = sb.ToString();
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        [Obsolete]
        public decimal CantidadExcedentePorVentaDetalle(int pIdAlmacen, string pIdProductoDetalle, decimal pCantidad,
            string IdDetalleVenta, int IdUnidadVenta, string NroPedido, string NroSerie, string NroLote)
        {
            try
            {
                var periodo = Globals.ClientSession.i_Periodo.ToString();
                var dbContext = new SAMBHSEntitiesModelWin();

                productoalmacen _productoalmacen = new productoalmacen();
                if (string.IsNullOrWhiteSpace(NroPedido))
                {

                    if (string.IsNullOrEmpty(NroLote) && string.IsNullOrEmpty(NroSerie))
                    {
                        _productoalmacen = (from n in dbContext.productoalmacen
                                            where
                                                n.i_IdAlmacen == pIdAlmacen && n.v_ProductoDetalleId == pIdProductoDetalle && n.i_Eliminado == 0 &&
                                                n.v_Periodo == periodo && n.v_NroPedido == null && n.v_NroLote == null && n.v_NroSerie == null
                                            select n).FirstOrDefault();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(NroLote))
                        {
                            _productoalmacen = (from n in dbContext.productoalmacen
                                                where
                                                    n.i_IdAlmacen == pIdAlmacen && n.v_ProductoDetalleId == pIdProductoDetalle && n.i_Eliminado == 0 &&
                                                    n.v_Periodo == periodo && n.v_NroPedido == null && n.v_NroLote != null && n.v_NroLote.Trim() == NroLote
                                                select n).FirstOrDefault();
                        }
                        else
                        {
                            _productoalmacen = (from n in dbContext.productoalmacen
                                                where
                                                    n.i_IdAlmacen == pIdAlmacen && n.v_ProductoDetalleId == pIdProductoDetalle && n.i_Eliminado == 0 &&
                                                    n.v_Periodo == periodo && n.v_NroPedido == null && n.v_NroSerie != null && n.v_NroSerie.Trim() == NroSerie
                                                select n).FirstOrDefault();
                        }

                    }
                }
                else
                {

                    if (string.IsNullOrEmpty(NroLote) && string.IsNullOrEmpty(NroSerie))
                    {
                        _productoalmacen = (from n in dbContext.productoalmacen
                                            where
                                                n.i_IdAlmacen == pIdAlmacen && n.v_ProductoDetalleId == pIdProductoDetalle && n.i_Eliminado == 0 &&
                                                n.v_Periodo == periodo && n.v_NroPedido.Trim() == NroPedido.Trim()
                                            select n).FirstOrDefault();
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(NroSerie))
                        {
                            _productoalmacen = (from n in dbContext.productoalmacen
                                                where
                                                    n.i_IdAlmacen == pIdAlmacen && n.v_ProductoDetalleId == pIdProductoDetalle && n.i_Eliminado == 0 &&
                                                    n.v_Periodo == periodo && n.v_NroPedido.Trim() == NroPedido.Trim() && n.v_NroSerie != null && n.v_NroSerie.Trim() == NroSerie
                                                select n).FirstOrDefault();
                        }
                        else
                        {

                            _productoalmacen = (from n in dbContext.productoalmacen
                                                where
                                                    n.i_IdAlmacen == pIdAlmacen && n.v_ProductoDetalleId == pIdProductoDetalle && n.i_Eliminado == 0 &&
                                                    n.v_Periodo == periodo && n.v_NroPedido.Trim() == NroPedido.Trim() && n.v_NroLote != null && n.v_NroLote.Trim() == NroLote
                                                select n).FirstOrDefault();
                        }

                    }
                }

                if (_productoalmacen != null)
                {
                    decimal saldoAlmacen = 0;
                    if (IdDetalleVenta != null)
                    {
                        var _ventadetalleCantidad = (from n in dbContext.ventadetalle
                                                     where n.v_IdVentaDetalle == IdDetalleVenta && n.i_Eliminado == 0
                                                     select new { n.d_Cantidad }).FirstOrDefault();

                        decimal Separacion = _productoalmacen.d_SeparacionTotal ?? 0;
                        if (_productoalmacen.d_StockActual != null)
                            if (_ventadetalleCantidad != null)
                                saldoAlmacen = (_productoalmacen.d_StockActual.Value +
                                                _ventadetalleCantidad.d_Cantidad.Value) - (Separacion);
                    }
                    else
                    {
                        var Stock = _productoalmacen.d_StockActual ?? 0;
                        var Separacion = _productoalmacen.d_SeparacionTotal ?? 0;
                        saldoAlmacen = Stock - Separacion;
                    }

                    var Excedente = saldoAlmacen -
                                    Utils.Windows.DevolverTotalUnidades(pIdProductoDetalle, pCantidad, IdUnidadVenta);

                    return Excedente;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Verifica si una venta modificada puede ser guardada sin necesidad de eliminar el canje de letras o la cobrnaza
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="ventaModificada"></param>
        /// <returns></returns>
        public bool ReGuardadoValido(ref OperationResult pobjOperationResult, ventaDto ventaModificada)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (string.IsNullOrWhiteSpace(ventaModificada.v_IdVenta))
                        throw new ArgumentNullException("Id de la venta está nulo");

                    var ventaOriginal = dbContext.venta.FirstOrDefault(p => p.v_IdVenta.Equals(ventaModificada.v_IdVenta) && p.i_Eliminado == 0);
                    if (ventaOriginal != null)
                    {
                        string ubicacionCobranzas;
                        var seriesIguales = ventaOriginal.v_SerieDocumento.Trim().Equals(ventaModificada.v_SerieDocumento.Trim(), StringComparison.CurrentCultureIgnoreCase);
                        var correlativosIguales = ventaOriginal.v_CorrelativoDocumento.Trim().Equals(ventaModificada.v_CorrelativoDocumento.Trim(), StringComparison.CurrentCultureIgnoreCase);
                        var tieneCobrnazs = TieneCobranzasRealizadas(ventaModificada.v_IdVenta, out ubicacionCobranzas);
                        var seEstaAnulando = ventaOriginal.i_IdEstado == 1 && ventaModificada.i_IdEstado == 0;
                        var tieneLetras = VentaFueCanjeadaALetras(ref pobjOperationResult, ventaModificada.v_IdVenta);
                        var montosDiferentes = ventaModificada.d_Total != ventaOriginal.d_Total;
                        var documentosDiferentes = ventaOriginal.i_IdTipoDocumento != ventaModificada.i_IdTipoDocumento;
                        var estaAnulado = ventaOriginal.i_IdEstado == 0;

                        if (seEstaAnulando && (tieneCobrnazs || tieneLetras))
                            throw new Exception("No se puede anular un documento con cobranzas o letras \r" + ubicacionCobranzas);

                        if (!seEstaAnulando && !estaAnulado)
                        {
                            if (tieneCobrnazs)
                            {
                                if (montosDiferentes)
                                    throw new Exception("Se ha modificado el monto a un documento con cobranzas, no se puede modificar.");

                                if (!seriesIguales)
                                    throw new Exception("Se ha modificado la serie a un documento con cobranzas, no se puede modificar.");

                                if (!correlativosIguales)
                                    throw new Exception("Se ha modificado el correlativo a un documento con cobranzas, no se puede modificar.");

                                if (documentosDiferentes)
                                    throw new Exception("Se ha modificado el tipo documento a un documento con cobranzas, no se puede modificar.");
                            }

                            if (tieneLetras)
                            {
                                if (montosDiferentes)
                                    throw new Exception("Se ha modificado el monto a un documento con canje de letras, no se puede modificar.");

                                if (!seriesIguales)
                                    throw new Exception("Se ha modificado la serie a un documento con canje de letras, no se puede modificar.");

                                if (!correlativosIguales)
                                    throw new Exception("Se ha modificado el correlativo a un documento con canje de letras, no se puede modificar.");

                                if (documentosDiferentes)
                                    throw new Exception("Se ha modificado el tipo documento a un documento con canje de letras, no se puede modificar.");
                            }
                        }
                        pobjOperationResult.Success = 1;
                        return true;
                    }

                    throw new Exception("Otro usuario acaba de eliminar la venta.");
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.ReGuardadoValido()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public bool VentaFueCanjeadaALetras(ref OperationResult pobjOperationResult, string pstrIdVenta)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var r = dbContext.letrascanje.Count(p => p.v_IdVenta == pstrIdVenta && p.i_Eliminado == 0);
                    pobjOperationResult.Success = 1;
                    return r > 0;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.VentaFueCanjeadaALetras()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public bool TieneCobranzasRealizadas(string pstrIdVenta, out string nroCobranza)
        {
            try
            {
                nroCobranza = "";
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                List<cobranzadetalle> cpEntity = new List<cobranzadetalle>();
                var Venta = dbContext.venta.FirstOrDefault(p => p.v_IdVenta == pstrIdVenta);

                if (Venta != null)
                {
                    if (!_objDocumentoBL.DocumentoEsInverso(Venta.i_IdTipoDocumento.Value))
                    {
                        cpEntity =
                            dbContext.cobranzadetalle.Where(p => p.v_IdVenta == pstrIdVenta && p.i_Eliminado == 0 && p.cobranza.i_IdEstado == 1)
                                .ToList();
                    }
                    else
                    {
                        string NroDocumentoRef = Venta.v_SerieDocumento + "-" + Venta.v_CorrelativoDocumento;

                        cpEntity =
                            dbContext.cobranzadetalle.Where(
                                p => p.v_DocumentoRef == NroDocumentoRef && p.i_Eliminado == 0 && p.cobranza.i_IdEstado == 1)
                                .ToList()
                                .Where(p => _objDocumentoBL.DocumentoEsInverso(p.i_IdTipoDocumentoRef.Value))
                                .ToList();
                    }
                }

                if (cpEntity != null && cpEntity.Count > 0)
                {
                    if (cpEntity.Sum(p => p.d_ImporteSoles.Value) > 0)
                    {
                        nroCobranza = "Cobranza: " + string.Join("-", cpEntity.Select(p => p.cobranza.i_IdTipoDocumento + "-" +
                                                                        p.cobranza.v_Correlativo + " | " +
                                                                        p.cobranza.t_FechaRegistro.Value
                                                                            .ToShortDateString()).ToList().Distinct());
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        public bool EsServicio(string pstrIdProductoDetalle)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var cpEntity = (from p in dbContext.producto

                                join J1 in dbContext.productodetalle on p.v_IdProducto equals J1.v_IdProducto into J1_join
                                from J1 in J1_join.DefaultIfEmpty()

                                where J1.v_IdProductoDetalle == pstrIdProductoDetalle
                                select new { p.i_EsServicio }).FirstOrDefault();

                if (cpEntity != null)
                {
                    return cpEntity.i_EsServicio.Value == 1;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        public string[] PublicoGeneral()
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var PublicoGeneral = (from pg in dbContext.cliente
                                      join dir in dbContext.clientedirecciones on new { d = pg.v_IdCliente, eliminado = 0, pred = 1 }
                                                                  equals new { d = dir.v_IdCliente, eliminado = dir.i_Eliminado ?? 1, pred = dir.i_EsDireccionPredeterminada ?? 1 } into dir_join
                                      from dir in dir_join.DefaultIfEmpty()
                                      where pg.v_IdCliente == "N002-CL000000000"
                                      select new { v_IdCliente = pg.v_IdCliente, v_CodCliente = pg.v_CodCliente, v_RazonSocial = pg.v_RazonSocial, v_NroDocIdentificacion = pg.v_NroDocIdentificacion, i_IdDireccionCliente = dir == null ? -1 : dir.i_IdDireccionCliente })
                    .FirstOrDefault();

                if (PublicoGeneral != null)
                {
                    string[] Cadena = new string[5];
                    Cadena[0] = PublicoGeneral.v_IdCliente;
                    Cadena[1] = PublicoGeneral.v_CodCliente;
                    Cadena[2] = PublicoGeneral.v_RazonSocial;
                    Cadena[3] = PublicoGeneral.v_NroDocIdentificacion;
                    Cadena[4] = PublicoGeneral.i_IdDireccionCliente.ToString();
                    return Cadena;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Converts the order to sale.
        /// </summary>
        /// <param name="pobjOperationResult">The pobj operation result.</param>
        /// <param name="pstrIdPedido">The PSTR identifier pedido.</param>
        /// <param name="ClientSession">The client session.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.Exception">El pedido necesita re-hacerse para evitar errores de impresión.</exception>
        public bool ConvertOrderToSale(ref OperationResult pobjOperationResult, string pstrIdPedido, List<string> ClientSession)
        {
            try
            {
                var tpCambio = decimal.Parse(DevolverTipoCambioPorFecha(ref pobjOperationResult, DateTime.Now.Date));
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    #region Pedido
                    pedido _pedido = (from p in dbContext.pedido
                                      where p.v_IdPedido == pstrIdPedido
                                      select p).FirstOrDefault();

                    string nroPedido = _pedido.v_SerieDocumento + " - " + _pedido.v_CorrelativoDocumento;
                    string periodo = Globals.ClientSession.i_Periodo.Value.ToString();
                    string NombreRazonSocialCliente = string.Empty;
                    var pedidoExisteEnOtraVenta = dbContext.venta.Any(p => p.v_Periodo == periodo && p.v_NroPedido.Contains(nroPedido) && p.i_Eliminado == 0);
                    if (pedidoExisteEnOtraVenta)
                        throw new Exception("El pedido necesita re-hacerse para evitar errores de impresión.");
                    #endregion

                    #region Cliente
                    var NroDocCliente = (from c in dbContext.cliente
                                         where c.v_IdCliente == _pedido.v_IdCliente
                                         select c.v_NroDocIdentificacion).FirstOrDefault();
                    var Cliente = dbContext.cliente.FirstOrDefault(p => p.v_IdCliente == _pedido.v_IdCliente);

                    if (_pedido.v_IdCliente != "N002-CL000000000")
                    {
                        if (Cliente != null)
                        {
                            NombreRazonSocialCliente =
                                (Cliente.v_ApePaterno + " " + Cliente.v_ApeMaterno + " " + Cliente.v_PrimerNombre + " " +
                                 Cliente.v_RazonSocial).Trim();
                        }
                        else
                        {
                            NombreRazonSocialCliente = "CLIENTE NO EXISTE.";
                        }
                    }


                    var tipoDoc = NroDocCliente.Length == 11 ? DocumentType.Factura : DocumentType.Boleta;
                    //10 para facturas y 9 para boletas.
                    var maxItems = tipoDoc == DocumentType.Factura ? 10 : 9;
                    #endregion

                    #region Pedido Detalles
                    var allPed = (from pd in _pedido.pedidodetalle
                                  join prd in dbContext.productodetalle on pd.v_IdProductoDetalle equals prd.v_IdProductoDetalle into prd_Join
                                  from prd in prd_Join.DefaultIfEmpty()
                                  join p in dbContext.producto on prd.v_IdProducto equals p.v_IdProducto into p_Join
                                  from p in p_Join
                                  where
                                      pd.v_IdPedido == pstrIdPedido && pd.i_Eliminado == 0 &&
                                      pd.v_IdProductoDetalle != "N002-PE000000000"
                                  select new KeyValuePair<int, pedidodetalle>(p.i_EsServicio ?? 0, pd));

                    var _pedidodetalle = allPed.Where(r => tipoDoc == DocumentType.Factura || r.Key == 0).Select(r => r.Value).ToList();
                    var _pedidoDetalleServicio = allPed.Where(r => tipoDoc == DocumentType.Boleta && r.Key == 1).Select(r => r.Value).OrderByDescending(r => r.d_PrecioVenta).ToList();
                    #endregion

                    #region Header Venta
                    var objVenta = new ventaDto();
                    objVenta.i_IdMoneda = _pedido.i_IdMoneda;

                    objVenta.d_Anticipio = 0;
                    objVenta.d_IGV = Utils.Windows.DevuelveValorRedondeado(_pedido.d_Igv.Value, 2);
                    objVenta.d_TipoCambio = tpCambio;
                    objVenta.d_Total = Utils.Windows.DevuelveValorRedondeado(_pedido.d_VVenta.Value, 2);
                    objVenta.i_DeduccionAnticipio = 0;
                    objVenta.i_EsAfectoIgv = _pedido.i_AfectoIgv;
                    objVenta.i_PreciosIncluyenIgv = _pedido.i_PrecionInclIgv;
                    objVenta.t_FechaRef = DateTime.Today;
                    objVenta.t_FechaVencimiento = DateTime.Today;
                    objVenta.t_FechaRegistro = DateTime.Today;
                    objVenta.i_IdCondicionPago = 1;
                    objVenta.i_IdEstado = 1;
                    objVenta.i_IdIgv = _pedido.i_IdIgv;
                    objVenta.i_IdMoneda = _pedido.i_IdMoneda;
                    objVenta.i_IdTipoDocumentoRef = -1;
                    objVenta.v_CorrelativoDocumentoRef = null;
                    objVenta.v_CorrelativoDocumentoFin = null;
                    objVenta.v_Concepto = string.IsNullOrEmpty(_pedido.v_Glosa.Trim()) ? "VENTA DE MERCADERÍA" : _pedido.v_Glosa.Trim();
                    objVenta.v_SerieDocumentoRef = null;
                    objVenta.d_PorcDescuento = _pedido.d_Dscto;
                    objVenta.d_PocComision = 0;
                    objVenta.d_Descuento = _pedido.d_Descuento;
                    objVenta.v_BultoDimensiones = null;
                    objVenta.v_NroGuiaRemisionCorrelativo = null;
                    objVenta.v_NroGuiaRemisionSerie = null;
                    objVenta.d_IGV = _pedido.d_Igv;
                    objVenta.v_Marca = null;
                    objVenta.v_NroBulto = null;
                    objVenta.i_NroDias = 0;
                    objVenta.v_OrdenCompra = null;
                    objVenta.d_PesoBrutoKG = 0;
                    objVenta.d_PesoNetoKG = 0;
                    objVenta.t_FechaOrdenCompra = DateTime.Today;
                    objVenta.i_IdMedioPagoVenta = -1;
                    objVenta.i_IdPuntoDestino = -1;
                    objVenta.i_IdPuntoEmbarque = -1;
                    objVenta.i_IdTipoEmbarque = -1;
                    objVenta.i_IdTipoOperacion = _pedido.i_IdTipoOperacion;
                    objVenta.i_IdTipoVenta = 3;
                    objVenta.i_DrawBack = 0;
                    objVenta.v_IdVendedor = _pedido.v_IdVendedor;
                    objVenta.v_IdVendedorRef = _pedido.v_IdVendedorRef;
                    objVenta.v_NroPedido = _pedido.v_SerieDocumento + " - " + _pedido.v_CorrelativoDocumento;
                    objVenta.v_IdCliente = _pedido.v_IdCliente;
                    objVenta.v_Mes = DateTime.Now.Month.ToString("00");
                    objVenta.v_Periodo = DateTime.Now.Year.ToString("0000");

                    if (_pedido.v_IdCliente == "N002-CL000000000")
                    {
                        objVenta.v_NombreClienteTemporal = string.IsNullOrWhiteSpace(_pedido.v_NombreClienteTemporal)
                                ? "PÚBLICO GENERAL" : _pedido.v_NombreClienteTemporal;
                        objVenta.v_DireccionClienteTemporal = _pedido.v_DireccionClienteTemporal;
                    }


                    objVenta.NombreCliente = string.IsNullOrEmpty(objVenta.v_NombreClienteTemporal)
                          ? NombreRazonSocialCliente
                          : objVenta.v_NombreClienteTemporal;
                    #endregion

                    #region GEN VENTAS
                    #region Func GenVenta
                    Func<SAMBHSEntitiesModelWin, ventaDto, List<pedidodetalle>, OperationResult> genSaleFromOrder = (db, oVenta, orderDetail) =>
                    {
                        var ventaDetails = new List<ventadetalleDto>();
                        DocumentoBL _objDocumentoBL = new DocumentoBL();
                        EstablecimientoBL _objEstablecimientoBL = new EstablecimientoBL();
                        var idAlmacen = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                        var pobjResult = new OperationResult();

                        #region Head
                        var x = (from n in db.venta
                                 where
                                     n.v_Mes == oVenta.v_Mes && n.v_Periodo == oVenta.v_Periodo && n.i_Eliminado == 0 &&
                                     n.v_IdVenta.Substring(2, 2) == idAlmacen
                                 orderby n.v_Correlativo ascending
                                 select n.v_Correlativo);

                        oVenta.v_Correlativo = x != null && x.Any()
                                                ? (int.Parse(x.Max()) + 1).ToString("D8")
                                                : "00000001";
                        var docName = string.Empty;
                        switch (oVenta.i_IdTipoDocumento)
                        {
                            case 1:
                                docName = "FAC";
                                break;
                            case 3:
                                docName = "BOL";
                                break;
                            case 12:
                                docName = "TCK";
                                break;
                        }

                        oVenta.v_SerieDocumento = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, oVenta.i_IdTipoDocumento ?? 1).Trim();
                        if (string.IsNullOrEmpty(oVenta.v_SerieDocumento))
                            throw new Exception("No existe una configuración para este documento (" + docName + ") o no existe uno marcado como predeterminado.");

                        oVenta.v_CorrelativoDocumento = _objDocumentoBL.CorrelativoxSerie(oVenta.i_IdTipoDocumento ?? 1, oVenta.v_SerieDocumento).Trim();
                        oVenta.i_IdEstablecimiento = _objEstablecimientoBL.DevolverEstablecimientoXSerie(oVenta.i_IdTipoDocumento ?? 1, oVenta.v_SerieDocumento);
                        oVenta.i_IdDireccionCliente = _pedido.i_IdDireccionCliente ?? -1;
                        #endregion

                        #region Detail
                        ventaDetails.Clear();
                        foreach (var prod in orderDetail)
                        {
                            string v_IdProductoDetalle = prod.v_IdProductoDetalle;
                            #region obtiene la cuenta
                            var nroCuenta = (from n in db.productodetalle
                                             join J0 in db.producto on n.v_IdProducto equals J0.v_IdProducto into J0_join
                                             from J0 in J0_join.DefaultIfEmpty()
                                             join J1 in db.linea on J0.v_IdLinea equals J1.v_IdLinea into J1_join
                                             from J1 in J1_join.DefaultIfEmpty()
                                             where n.v_IdProductoDetalle == v_IdProductoDetalle
                                             select new { nroCuenta = J1 != null ? J1.v_NroCuentaVenta ?? "-1" : "-1" }).FirstOrDefault
                                                    ().nroCuenta;
                            #endregion

                            #region obtiene el precio lista 1
                            var precioLista = (from p in db.listaprecio
                                               join J1 in db.listapreciodetalle on new { id = p.v_IdListaPrecios, eliminado = 0, almacen = p.i_IdAlmacen.Value }
                                                                                       equals new { id = J1.v_IdListaPrecios, eliminado = J1.i_Eliminado.Value, almacen = J1.i_IdAlmacen.Value }
                                               //join J2 in db.productoalmacen on new { J1.v_IdProductoAlmacen, eliminado = 0, _periodo = periodo } equals new { J2.v_IdProductoAlmacen, eliminado = J2.i_Eliminado.Value, _periodo = J2.v_Periodo } into J2_join
                                               //from J2 in J2_join.DefaultIfEmpty()

                                               //join J2 in db.productoalmacen on new { pd= J1.v_IdProductoDetalle ,almacen = J1.i_IdAlmacen.Value  , eliminado = 0, _periodo = periodo } equals new {  pd= J2.v_ProductoDetalleId ,almacen = J2.i_IdAlmacen, eliminado = J2.i_Eliminado.Value, _periodo = J2.v_Periodo } into J2_join
                                               //from J2 in J2_join.DefaultIfEmpty()


                                               join J3 in db.productodetalle on new { pd = J1.v_IdProductoDetalle } equals new { pd = J3.v_IdProductoDetalle } into J3_join
                                               from J3 in J3_join.DefaultIfEmpty()

                                               where
                                                   p.i_IdLista == 1 && p.i_Eliminado == 0 &&
                                                   J3.v_IdProductoDetalle == v_IdProductoDetalle
                                               select J1.d_Precio).FirstOrDefault();
                            #endregion

                            var precioImpresion = precioLista != null && precioLista != 0 && precioLista >= prod.d_PrecioUnitario
                                                  ? precioLista : prod.d_PrecioUnitario;

                            var objDetalle = new ventadetalleDto();
                            objDetalle.i_IdUnidadMedida = prod.i_IdUnidadMedida;
                            objDetalle.d_Cantidad = prod.d_Cantidad;
                            objDetalle.d_Precio = prod.d_PrecioUnitario;
                            objDetalle.i_Anticipio = 0;
                            objDetalle.v_DescripcionProducto = prod.v_NombreProducto;
                            objDetalle.v_IdProductoDetalle = prod.v_IdProductoDetalle;
                            objDetalle.i_IdAlmacen = prod.i_IdAlmacen;
                            objDetalle.d_Valor = Utils.Windows.DevuelveValorRedondeado(prod.d_Valor.Value, 2);
                            objDetalle.d_ValorVenta = Utils.Windows.DevuelveValorRedondeado(prod.d_ValorVenta.Value, 2);
                            objDetalle.d_Igv = Utils.Windows.DevuelveValorRedondeado(prod.d_Igv.Value, 2);
                            objDetalle.d_PrecioVenta = Utils.Windows.DevuelveValorRedondeado(prod.d_PrecioVenta.Value, 2);
                            objDetalle.i_IdCentroCosto = CentroCostoDeEstablecimiento(ref pobjResult);
                            objDetalle.d_Descuento = Utils.Windows.DevuelveValorRedondeado(prod.d_Descuento.Value, 2);
                            objDetalle.v_FacturaRef = prod.v_Descuento;
                            objDetalle.d_PrecioImpresion = precioImpresion;
                            objDetalle.i_IdTipoOperacion = prod.i_IdTipoOperacion;
                            objDetalle.v_NroCuenta = nroCuenta;
                            objDetalle.t_FechaCaducidad = prod.t_FechaCaducidad;
                            objDetalle.v_NroSerie = prod.v_NroSerie;
                            objDetalle.v_PedidoExportacion = prod.v_NroPedido;
                            objDetalle.v_NroLote = prod.v_NroLote;
                            objDetalle.d_CantidadEmpaque = Utils.Windows.DevolverTotalUnidades(objDetalle.v_IdProductoDetalle, objDetalle.d_Cantidad.Value, objDetalle.i_IdUnidadMedida.Value);
                            ventaDetails.Add(objDetalle);
                        }
                        var factDesc = 1 - (oVenta.d_PorcDescuento ?? 0) / 100M;
                        //_ventaDto.d_ValorVenta =  Utils.Windows.DevuelveValorRedondeado(_ventadetalleDtos.Sum(p => p.d_ValorVenta ?? 0) * factDesc, 2);
                        //_ventaDto.d_IGV = _ventadetalleDtos.Sum(p => p.d_Igv ?? 0) * factDesc;
                        oVenta.d_Total = Utils.Windows.DevuelveValorRedondeado(ventaDetails.Sum(p => p.d_PrecioVenta ?? 0) * factDesc, 2);
                        oVenta.d_Valor = Utils.Windows.DevuelveValorRedondeado(ventaDetails.Sum(p => p.d_Valor ?? 0), 2);

                        var redondeo = Utils.Windows.DevuelveValorRedondeado(oVenta.d_Total.Value, 1);
                        var residuo = (oVenta.d_Total.Value - redondeo) * -1;
                        if (Globals.ClientSession.i_RedondearVentas == 1 && residuo != 0)
                        {
                            var objRedondeo = new ventadetalleDto();
                            objRedondeo.i_IdUnidadMedida = 15;
                            objRedondeo.d_Cantidad = 1;
                            objRedondeo.d_Precio = residuo;
                            objRedondeo.t_FechaCaducidad = DateTime.Parse(Constants.FechaNula);
                            objRedondeo.i_Anticipio = 0;
                            objRedondeo.v_NroCuenta = residuo < 0
                                ? Globals.ClientSession.v_NroCuentaRedondeoPerdida
                                : Globals.ClientSession.v_NroCuentaRedondeoGanancia;
                            objRedondeo.v_DescripcionProducto = "REDONDEO";
                            objRedondeo.v_IdProductoDetalle = "N002-PE000000000";
                            objRedondeo.i_IdAlmacen = Globals.ClientSession.i_IdAlmacenPredeterminado.Value;
                            objRedondeo.d_Valor = residuo;
                            objRedondeo.d_ValorVenta = residuo;
                            objRedondeo.i_IdCentroCosto = CentroCostoDeEstablecimiento(ref pobjResult);
                            objRedondeo.d_ValorVenta = residuo;
                            objRedondeo.d_Igv = 0;
                            objRedondeo.d_PrecioVenta = residuo;
                            objRedondeo.d_PrecioImpresion = residuo;
                            objRedondeo.EsServicio = (from p in db.producto
                                                      join J1 in db.productodetalle on p.v_IdProducto equals J1.v_IdProducto into
                                                          J1_join
                                                      from J1 in J1_join.DefaultIfEmpty()
                                                      where
                                                          J1.v_IdProductoDetalle == objRedondeo.v_IdProductoDetalle &&
                                                          J1.i_Eliminado == 0
                                                      select p.i_EsServicio).FirstOrDefault() ?? 0;
                            objRedondeo.d_CantidadEmpaque = 0;
                            ventaDetails.Add(objRedondeo);
                            oVenta.d_Total = Utils.Windows.DevuelveValorRedondeado(ventaDetails.Sum(p => p.d_PrecioVenta.Value) * factDesc, 2);
                        }
                        oVenta.d_ValorVenta = Utils.Windows.DevuelveValorRedondeado(ventaDetails.Sum(p => p.d_ValorVenta.Value) * factDesc, 2);
                        oVenta.d_IGV = Utils.Windows.DevuelveValorRedondeado((ventaDetails.Sum(p => p.d_Igv.Value) * factDesc), 2);
                        #endregion

                        string IdVenta = InsertarVenta(ref pobjResult, oVenta, ClientSession,
                            ventaDetails);

                        return pobjResult;
                    };
                    #endregion

                    objVenta.i_IdTipoDocumento = (int)tipoDoc;
                    while (_pedidodetalle.Count > 0)
                    {
                        pobjOperationResult = genSaleFromOrder(dbContext, objVenta, _pedidodetalle.Take(maxItems).ToList());
                        if (pobjOperationResult.Success == 0) return false;
                        _pedidodetalle = _pedidodetalle.Skip(maxItems).ToList();
                    }
                    // Servicios
                    if (_pedidoDetalleServicio.Count > 0)
                    {
                        if (tipoDoc == DocumentType.Boleta)
                            objVenta.i_IdTipoDocumento = (int)DocumentType.TicketBoleta;
                        pobjOperationResult = genSaleFromOrder(dbContext, objVenta, _pedidoDetalleServicio);
                        if (pobjOperationResult.Success == 0) return false;
                    }
                    #endregion

                    #region Despacho Pedido
                    DespacharPedido(ref pobjOperationResult, pstrIdPedido, Globals.ClientSession.GetAsList());
                    if (pobjOperationResult.Success == 0) return false;
                    #endregion

                    ts.Complete();
                    dbContext.Dispose();
                    return true;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "MovimientoBL.ConvertirPedidoAVenta()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public bool ConvertirPedidoAVenta(ref OperationResult pobjOperationResult, string pstrIdPedido,
            List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    venta _venta = new venta();
                    ventaDto _ventaDto = new ventaDto();
                    List<ventadetalleDto> _ventadetalleDtos = new List<ventadetalleDto>();
                    DocumentoBL _objDocumentoBL = new DocumentoBL();
                    EstablecimientoBL _objEstablecimientoBL = new EstablecimientoBL();
                    string NroDocCliente, NombreRazonSocialCliente = string.Empty;
                    int PosicionActual = 0, PosicionMaxima = 0;

                    pedido _pedido = (from p in dbContext.pedido
                                      where p.v_IdPedido == pstrIdPedido
                                      select p).FirstOrDefault();

                    string nroPedido = _pedido.v_SerieDocumento + " - " + _pedido.v_CorrelativoDocumento;
                    string periodo = Globals.ClientSession.i_Periodo.Value.ToString();
                    var pedidoExisteEnOtraVenta = dbContext.venta.Any(p => p.v_Periodo == periodo && p.v_NroPedido.Contains(nroPedido) && p.i_Eliminado == 0);
                    if (pedidoExisteEnOtraVenta)
                        throw new Exception("El pedido necesita re-hacerse para evitar errores de impresión.");
                    var Cliente = dbContext.cliente.FirstOrDefault(p => p.v_IdCliente == _pedido.v_IdCliente);

                    if (_pedido.v_IdCliente != "N002-CL000000000")
                    {
                        if (Cliente != null)
                        {
                            NombreRazonSocialCliente =
                                (Cliente.v_ApePaterno + " " + Cliente.v_ApeMaterno + " " + Cliente.v_PrimerNombre + " " +
                                 Cliente.v_RazonSocial).Trim();
                        }
                        else
                        {
                            NombreRazonSocialCliente = "CLIENTE NO EXISTE.";
                        }
                    }

                    List<pedidodetalle> _pedidodetalle = (from pd in _pedido.pedidodetalle



                                                          where
                                                              pd.v_IdPedido == pstrIdPedido && pd.i_Eliminado == 0

                                                          select pd).ToList();

                    NroDocCliente = Cliente.v_NroDocIdentificacion;

                    PosicionMaxima = NroDocCliente.Length == 11 && NroDocCliente != "00000000000" ? 10 : 9;
                    //10 para facturas y 9 para boletas.
                    int TipoDocumento = PosicionMaxima == 10 ? 1 : 3;

                    while (PosicionActual < PosicionMaxima)
                    {
                        PosicionActual = PosicionMaxima > 10 ? PosicionActual + 1 : PosicionActual;
                        //revisa si es segunda ronda o  más

                        #region Cabecera
                        _venta.i_IdMoneda = _pedido.i_IdMoneda;
                        _venta.v_Mes = _pedido.v_Mes.Trim();
                        _venta.v_Periodo = _pedido.v_Periodo.Trim();
                        var idAlmacen = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                        var x = (from n in dbContext.venta
                                 where
                                     n.v_Mes == _venta.v_Mes && n.v_Periodo == _venta.v_Periodo && n.i_Eliminado == 0 &&
                                     n.v_IdVenta.Substring(2, 2) == idAlmacen
                                 orderby n.v_Correlativo ascending
                                 select new { n.v_Correlativo });

                        _venta.v_Correlativo = x != null && x.Count() != 0
                            ? (int.Parse(x.Max(p => p.v_Correlativo).ToString()) + 1).ToString("00000000")
                            : "00000001";
                        _venta.d_Anticipio = 0;
                        _venta.d_IGV = Utils.Windows.DevuelveValorRedondeado(_pedido.d_Igv.Value, 2);
                        _venta.d_TipoCambio =
                            decimal.Parse(DevolverTipoCambioPorFecha(ref pobjOperationResult, DateTime.Now.Date));
                        _venta.d_Total = Utils.Windows.DevuelveValorRedondeado(_pedido.d_VVenta.Value, 2);
                        _venta.i_DeduccionAnticipio = 0;
                        _venta.i_EsAfectoIgv = 1;
                        _venta.t_FechaRef = DateTime.Today;
                        _venta.t_FechaVencimiento = DateTime.Today;
                        _venta.t_FechaRegistro = DateTime.Today;
                        _venta.i_IdCondicionPago = 1;
                        _venta.i_IdEstado = 1;
                        _venta.i_IdIgv = _pedido.i_IdIgv;
                        _venta.i_IdMoneda = _pedido.i_IdMoneda;
                        _venta.i_IdTipoDocumento = TipoDocumento;
                        _venta.i_IdTipoDocumentoRef = -1;
                        _venta.i_PreciosIncluyenIgv = 1;
                        _venta.v_CorrelativoDocumentoRef = null;
                        _venta.v_Mes = _pedido.v_Mes;
                        _venta.v_Periodo = _pedido.v_Periodo;

                        var documento = TipoDocumento == 1 ? "FAC" : "BOL";
                        _venta.v_SerieDocumento =
                            _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento,
                                TipoDocumento).Trim();
                        if (string.IsNullOrEmpty(_venta.v_SerieDocumento))
                            throw new Exception("No existe una configuración para este documento (" + documento +
                                                ") o no existe uno marcado como predeterminado.");

                        _venta.v_CorrelativoDocumento =
                            _objDocumentoBL.CorrelativoxSerie(TipoDocumento, _venta.v_SerieDocumento).Trim();
                        _venta.v_CorrelativoDocumentoFin = null;
                        _venta.v_Concepto = String.IsNullOrEmpty(_pedido.v_Glosa.Trim())
                            ? "VENTA DE MERCADERÍA"
                            : _pedido.v_Glosa.Trim();
                        _venta.i_IdEstablecimiento =
                            _objEstablecimientoBL.DevolverEstablecimientoXSerie(_venta.i_IdTipoDocumento ?? 1, _venta.v_SerieDocumento);
                        _venta.v_SerieDocumentoRef = null;
                        _venta.d_PorcDescuento = _pedido.d_Dscto;
                        _venta.d_PocComision = 0;
                        _venta.d_Descuento = _pedido.d_Descuento;
                        _venta.v_BultoDimensiones = null;
                        _venta.v_NroGuiaRemisionCorrelativo = null;
                        _venta.v_NroGuiaRemisionSerie = null;
                        _venta.d_IGV = _pedido.d_Igv;
                        _venta.v_Marca = null;
                        _venta.v_NroBulto = null;
                        _venta.i_NroDias = 0;
                        _venta.v_OrdenCompra = null;
                        _venta.d_PesoBrutoKG = 0;
                        _venta.d_PesoNetoKG = 0;
                        _venta.t_FechaOrdenCompra = DateTime.Today;
                        _venta.i_IdMedioPagoVenta = -1;
                        _venta.i_IdPuntoDestino = -1;
                        _venta.i_IdPuntoEmbarque = -1;
                        _venta.i_IdTipoEmbarque = -1;
                        _venta.i_IdTipoOperacion = _pedido.i_IdTipoOperacion;
                        _venta.i_IdTipoVenta = 3;
                        _venta.i_DrawBack = 0;
                        _venta.v_IdVendedor = _pedido.v_IdVendedor;
                        _venta.v_IdVendedorRef = _pedido.v_IdVendedorRef;
                        _venta.v_NroPedido = _pedido.v_SerieDocumento + " - " + _pedido.v_CorrelativoDocumento;
                        _venta.v_IdCliente = _pedido.v_IdCliente;
                        _venta.v_NombreClienteTemporal = _pedido.v_IdCliente == "N002-CL000000000"
                            ? string.IsNullOrEmpty(_pedido.v_NombreClienteTemporal.Trim())
                                ? "PÚBLICO GENERAL"
                                : _pedido.v_NombreClienteTemporal
                            : string.Empty;
                        _venta.v_DireccionClienteTemporal = _pedido.v_IdCliente == "N002-CL000000000"
                            ? _pedido.v_DireccionClienteTemporal
                            : string.Empty;
                        _ventaDto = _venta.ToDTO();
                        _ventaDto.CodigoCliente = Cliente.v_CodCliente;
                        _ventaDto.NombreCliente = string.IsNullOrEmpty(_venta.v_NombreClienteTemporal)
                            ? NombreRazonSocialCliente
                            : _venta.v_NombreClienteTemporal;

                        #endregion

                        #region Detalle

                        for (int i = PosicionActual; i < _pedidodetalle.Count; i++)
                        {
                            string v_IdProductoDetalle = _pedidodetalle[i].v_IdProductoDetalle;
                            #region obtiene la cuenta
                            var nroCuenta = (from n in dbContext.productodetalle
                                             join J0 in dbContext.producto on n.v_IdProducto equals J0.v_IdProducto into J0_join
                                             from J0 in J0_join.DefaultIfEmpty()
                                             join J1 in dbContext.linea on J0.v_IdLinea equals J1.v_IdLinea into J1_join
                                             from J1 in J1_join.DefaultIfEmpty()
                                             where n.v_IdProductoDetalle == v_IdProductoDetalle
                                             select new { nroCuenta = J1 != null ? J1.v_NroCuentaVenta ?? "-1" : "-1" }).FirstOrDefault
                                                    ().nroCuenta;
                            #endregion

                            #region define si es servicio
                            var Servicio = (from a in dbContext.producto

                                            join b in dbContext.productodetalle on new { p = a.v_IdProducto, eliminado = 0 } equals
                                                new { p = b.v_IdProducto, eliminado = b.i_Eliminado.Value } into b_join
                                            from b in b_join.DefaultIfEmpty()

                                            select new { EsServicio = a.i_EsServicio }).FirstOrDefault().EsServicio;
                            #endregion

                            #region obtiene el precio lista 1
                            var PrecioLista1 = (from p in dbContext.listaprecio

                                                join J1 in dbContext.listapreciodetalle on new { id = p.v_IdListaPrecios, eliminado = 0 }
                                                                                        equals new { id = J1.v_IdListaPrecios, eliminado = J1.i_Eliminado.Value }

                                                //join J2 in dbContext.productoalmacen on new { J1.v_IdProductoAlmacen, eliminado = 0, _periodo = periodo } equals new { J2.v_IdProductoAlmacen, eliminado = J2.i_Eliminado.Value, _periodo = J2.v_Periodo } into J2_join
                                                //from J2 in J2_join.DefaultIfEmpty()

                                                join J2 in dbContext.productoalmacen on new { ProdDetalle = J1.v_IdProductoDetalle, almacen = J1.i_IdAlmacen.Value, eliminado = 0, _periodo = periodo } equals new { ProdDetalle = J2.v_ProductoDetalleId, almacen = J2.i_IdAlmacen, eliminado = J2.i_Eliminado.Value, _periodo = J2.v_Periodo } into J2_join
                                                from J2 in J2_join.DefaultIfEmpty()

                                                join J3 in dbContext.productodetalle on J2.v_ProductoDetalleId equals
                                                    J3.v_IdProductoDetalle into J3_join
                                                from J3 in J3_join.DefaultIfEmpty()

                                                where
                                                    p.i_IdLista == 1 && p.i_Eliminado == 0 &&
                                                    J3.v_IdProductoDetalle == v_IdProductoDetalle

                                                select new { J1.d_Precio }).FirstOrDefault();
                            #endregion

                            var PrecioImpresion = PrecioLista1 != null && PrecioLista1.d_Precio != 0
                                && PrecioLista1.d_Precio.Value >= _pedidodetalle[i].d_PrecioUnitario
                                ? PrecioLista1.d_Precio.Value : _pedidodetalle[i].d_PrecioUnitario;

                            ventadetalleDto _ventadetalleDto = new ventadetalleDto();
                            _ventadetalleDto.i_IdUnidadMedida = _pedidodetalle[i].i_IdUnidadMedida;
                            _ventadetalleDto.d_Cantidad = _pedidodetalle[i].d_Cantidad;
                            _ventadetalleDto.d_Precio = _pedidodetalle[i].d_PrecioUnitario;
                            _ventadetalleDto.i_Anticipio = 0;
                            _ventadetalleDto.v_DescripcionProducto = _pedidodetalle[i].v_NombreProducto;
                            _ventadetalleDto.v_IdProductoDetalle = _pedidodetalle[i].v_IdProductoDetalle;
                            _ventadetalleDto.i_IdAlmacen = _pedidodetalle[i].i_IdAlmacen;
                            _ventadetalleDto.d_Valor =
                                Utils.Windows.DevuelveValorRedondeado(_pedidodetalle[i].d_Valor.Value, 2);
                            _ventadetalleDto.d_ValorVenta =
                                Utils.Windows.DevuelveValorRedondeado(_pedidodetalle[i].d_ValorVenta.Value, 2);
                            _ventadetalleDto.d_Igv = Utils.Windows.DevuelveValorRedondeado(
                                _pedidodetalle[i].d_Igv.Value, 2);
                            _ventadetalleDto.d_PrecioVenta = Utils.Windows.DevuelveValorRedondeado(_pedidodetalle[i].d_PrecioVenta.Value, 2);
                            _ventadetalleDto.i_IdCentroCosto = CentroCostoDeEstablecimiento(ref pobjOperationResult);
                            _ventadetalleDto.d_Descuento = Utils.Windows.DevuelveValorRedondeado(_pedidodetalle[i].d_Descuento.Value, 2);
                            _ventadetalleDto.v_FacturaRef = _pedidodetalle[i].v_Descuento;
                            _ventadetalleDto.d_PrecioImpresion = PrecioImpresion.Value;
                            _ventadetalleDto.EsServicio = ((from p in dbContext.producto

                                                            join J1 in dbContext.productodetalle on p.v_IdProducto equals J1.v_IdProducto into
                                                                J1_join
                                                            from J1 in J1_join.DefaultIfEmpty()

                                                            where
                                                                J1.v_IdProductoDetalle == _ventadetalleDto.v_IdProductoDetalle && p.i_Eliminado == 0

                                                            select new { p.i_EsServicio }).FirstOrDefault().i_EsServicio.Value);
                            _ventadetalleDto.i_IdTipoOperacion = _pedidodetalle[i].i_IdTipoOperacion;
                            _ventadetalleDto.v_NroCuenta = nroCuenta;

                            _ventadetalleDto.d_CantidadEmpaque =
                                Utils.Windows.DevolverTotalUnidades(_ventadetalleDto.v_IdProductoDetalle,
                                    _ventadetalleDto.d_Cantidad.Value, _ventadetalleDto.i_IdUnidadMedida.Value);

                            if (i == PosicionMaxima) break;
                            PosicionActual = i;
                            _ventadetalleDtos.Add(_ventadetalleDto);
                        }

                        #endregion

                        decimal Redondeado, Residuo;
                        decimal val_igv = decimal.Parse((from n in dbContext.datahierarchy
                                                         where n.i_GroupId == 27 && n.i_ItemId == _ventaDto.i_IdIgv.Value && n.i_IsDeleted == 0
                                                         select new { n.v_Value1 }).FirstOrDefault().v_Value1);

                        var factDesc = 1 - (_pedido.d_Dscto ?? 0) / 100M;
                        _ventaDto.d_ValorVenta =
                            Utils.Windows.DevuelveValorRedondeado(_ventadetalleDtos.Sum(p => p.d_ValorVenta ?? 0) * factDesc, 2);
                        _ventaDto.d_IGV = _ventadetalleDtos.Sum(p => p.d_Igv ?? 0) * factDesc;
                        _ventaDto.d_Total =
                            Utils.Windows.DevuelveValorRedondeado(_ventadetalleDtos.Sum(p => p.d_PrecioVenta ?? 0) * factDesc, 2);
                        _ventaDto.d_Valor =
                            Utils.Windows.DevuelveValorRedondeado(_ventadetalleDtos.Sum(p => p.d_Valor ?? 0), 2);

                        Redondeado = Utils.Windows.DevuelveValorRedondeado(_ventaDto.d_Total.Value, 1);
                        Residuo = (_ventaDto.d_Total.Value - Redondeado) * -1;

                        if (Globals.ClientSession.i_RedondearVentas == 1 && Residuo != 0)
                        {
                            ventadetalleDto _ventadetalleDto = new ventadetalleDto();
                            _ventadetalleDto.i_IdUnidadMedida = 15;
                            _ventadetalleDto.d_Cantidad = 1;
                            _ventadetalleDto.d_Precio = Residuo;
                            _ventadetalleDto.i_Anticipio = 0;
                            _ventadetalleDto.v_NroCuenta = Residuo < 0
                                ? Globals.ClientSession.v_NroCuentaRedondeoPerdida
                                : Globals.ClientSession.v_NroCuentaRedondeoGanancia;
                            _ventadetalleDto.v_DescripcionProducto = "REDONDEO";
                            _ventadetalleDto.v_IdProductoDetalle = "N002-PE000000000";
                            _ventadetalleDto.i_IdAlmacen = Globals.ClientSession.i_IdAlmacenPredeterminado.Value;
                            _ventadetalleDto.d_Valor = Residuo;
                            _ventadetalleDto.d_ValorVenta = Residuo;
                            _ventadetalleDto.i_IdCentroCosto = CentroCostoDeEstablecimiento(ref pobjOperationResult);
                            _ventadetalleDto.d_ValorVenta = Residuo;
                            _ventadetalleDto.d_Igv = 0;
                            _ventadetalleDto.d_PrecioVenta = Residuo;
                            _ventadetalleDto.d_PrecioImpresion = Residuo;
                            _ventadetalleDtos.Add(_ventadetalleDto);
                            _ventaDto.d_ValorVenta = Utils.Windows.DevuelveValorRedondeado(_ventadetalleDtos.Sum(p => p.d_ValorVenta.Value) * factDesc, 2);
                            _ventaDto.d_IGV = Utils.Windows.DevuelveValorRedondeado((_ventadetalleDtos.Sum(p => p.d_Igv.Value) * factDesc), 2);
                            _ventaDto.d_Total = _ventadetalleDtos.Sum(p => p.d_PrecioVenta.Value) * factDesc;
                            _ventadetalleDto.EsServicio = (from p in dbContext.producto
                                                           join J1 in dbContext.productodetalle on p.v_IdProducto equals J1.v_IdProducto into
                                                               J1_join
                                                           from J1 in J1_join.DefaultIfEmpty()
                                                           where
                                                               J1.v_IdProductoDetalle == _ventadetalleDto.v_IdProductoDetalle &&
                                                               J1.i_Eliminado == 0
                                                           select new { p.i_EsServicio }
                                ).FirstOrDefault().i_EsServicio.Value;
                            _ventadetalleDto.d_CantidadEmpaque = 0;
                        }

                        string IdVenta = InsertarVenta(ref pobjOperationResult, _ventaDto, ClientSession,
                            _ventadetalleDtos);
                        if (pobjOperationResult.Success == 0) return false;

                        #region Realiza Despacho del Pedido

                        DespacharPedido(ref pobjOperationResult, pstrIdPedido, Globals.ClientSession.GetAsList());
                        if (pobjOperationResult.Success == 0) return false;

                        #endregion

                        if (PosicionActual == _pedidodetalle.Count - 1) break;
                        PosicionMaxima = PosicionActual < (_pedidodetalle.Count() - 1)
                            ? PosicionMaxima + (_venta.i_IdTipoDocumento.Value == 1 ? 10 : 9)
                            : PosicionMaxima;
                        _ventadetalleDtos = new List<ventadetalleDto>();
                        pobjOperationResult.Success = 1;
                    }
                    ts.Complete();
                    dbContext.Dispose();
                    return true;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "MovimientoBL.ConvertirPedidoAVenta()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public string ObtenerFuturosCorrelativos(string IdPedido, bool EsFactura)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                DocumentoBL _objDocumentoBL = new DocumentoBL();

                List<pedidodetalle> pd = (from p in dbContext.pedidodetalle
                                          where p.v_IdPedido == IdPedido && p.i_Eliminado == 0 && p.v_IdProductoDetalle != "N002-PE000000000"
                                          select p).ToList();
                int maxCC;
                string Mensaje = "El comprobante se generará con el correlativo: ";
                string Siglas;

                if (EsFactura == true)
                {
                    Siglas = (from a in dbContext.documento
                              where a.i_CodigoDocumento == 1 && a.i_Eliminado == 0
                              select new { a.v_Siglas }).FirstOrDefault().v_Siglas.ToString();
                    string Serie =
                        _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, 1).Trim();

                    maxCC = int.Parse(_objDocumentoBL.CorrelativoxSerie(EsFactura ? 1 : 3, Serie));

                    if (pd.Count() <= 10)
                    {
                        return string.Format(Mensaje + "[" + Siglas + " {0}-{1}]", Serie, maxCC.ToString("00000000"));
                    }

                    if (pd.Count() > 10 && pd.Count() <= 20)
                    {
                        return string.Format(Mensaje + "[" + Siglas + " {0}-{1}] " + "[" + Siglas + "{2}-{3}]", Serie,
                            maxCC.ToString("00000000"), Serie, (maxCC + 1).ToString("00000000"));
                    }

                    if (pd.Count() > 20 && pd.Count() <= 30)
                    {
                        return
                            string.Format(
                                Mensaje + "[" + Siglas + " {0}-{1}] " + "[" + Siglas + "{2}-{3}] " + "[" + Siglas +
                                "{4}-{5}]", Serie, maxCC.ToString("00000000"), Serie, (maxCC + 1).ToString("00000000"),
                                Serie, (maxCC + 2).ToString("00000000"));
                    }

                    if (pd.Count() > 30 && pd.Count() <= 40)
                    {
                        return
                            string.Format(
                                Mensaje + "[" + Siglas + " {0}-{1}] " + "[" + Siglas + "{2}-{3}] " + "[" + Siglas +
                                "{4}-{5}]" + "[" + Siglas + "{6}-{7}]", Serie, maxCC.ToString("00000000"), Serie, (maxCC + 1).ToString("00000000"),
                                Serie, (maxCC + 2).ToString("00000000"), Serie, (maxCC + 3).ToString("00000000"));
                    }

                    if (pd.Count() > 40 && pd.Count() <= 50)
                    {
                        return
                            string.Format(
                                Mensaje + "[" + Siglas + " {0}-{1}] " + "[" + Siglas + "{2}-{3}] " + "[" + Siglas +
                                "{4}-{5}]" + "[" + Siglas + "{6}-{7}]" + "[" + Siglas + "{8}-{9}]", Serie, maxCC.ToString("00000000"), Serie, (maxCC + 1).ToString("00000000"),
                                Serie, (maxCC + 2).ToString("00000000"), Serie, (maxCC + 3).ToString("00000000"), Serie, (maxCC + 4).ToString("00000000"));
                    }

                }
                else
                {
                    Siglas = (from a in dbContext.documento
                              where a.i_CodigoDocumento == 3 && a.i_Eliminado == 0
                              select new { a.v_Siglas }).FirstOrDefault().v_Siglas.ToString();

                    string Serie =
                        _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, 3).Trim();
                    maxCC = int.Parse(_objDocumentoBL.CorrelativoxSerie(EsFactura ? 1 : 3, Serie));

                    if (pd.Count() <= 9)
                    {
                        return string.Format(Mensaje + "[" + Siglas + " {0}-{1}]", Serie, maxCC.ToString("00000000"));
                    }

                    if (pd.Count() > 9 && pd.Count() <= 18)
                    {
                        return string.Format(Mensaje + "[" + Siglas + " {0}-{1}] " + "[" + Siglas + " {2}-{3}]", Serie,
                            maxCC.ToString("00000000"), Serie, (maxCC + 1).ToString("00000000"));
                    }

                    if (pd.Count() > 18 && pd.Count() <= 27)
                    {
                        return
                            string.Format(
                                Mensaje + "[" + Siglas + " {0}-{1}] " + "[" + Siglas + " {2}-{3}] " + "[" + Siglas +
                                " {4}-{5}]", Serie, maxCC.ToString("00000000"), Serie, (maxCC + 1).ToString("00000000"),
                                Serie, (maxCC + 2).ToString("00000000"));
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void InsertaClientePublicoGeneralSiEsNecesario()
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    cliente cpg = (from c in dbContext.cliente
                                   where c.v_IdCliente == "N002-CL000000000"
                                   select c).FirstOrDefault();

                    if (cpg == null)
                    {
                        cliente clientepg = new cliente();
                        clientepg.v_IdCliente = "N002-CL000000000";
                        clientepg.v_CodCliente = "000000000";
                        clientepg.v_FlagPantalla = "C";
                        clientepg.i_IdTipoIdentificacion = 6;
                        clientepg.v_NroDocIdentificacion = "00000000000";
                        clientepg.i_IdTipoPersona = 1;
                        clientepg.v_RazonSocial = "PÚBLICO GENERAL";
                        clientepg.v_PrimerNombre = "";
                        clientepg.v_SegundoNombre = "";
                        clientepg.v_ApeMaterno = "";
                        clientepg.v_ApePaterno = "";
                        clientepg.i_Activo = 1;
                        clientepg.i_Nacionalidad = 0;
                        clientepg.i_IdListaPrecios = 1;
                        clientepg.i_Eliminado = 0;
                        dbContext.AddTocliente(clientepg);
                        dbContext.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool EsValidoCodProducto(string pstrCodInterno)
        {
            if (string.IsNullOrEmpty(pstrCodInterno)) return false;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    return dbContext.producto.Any(p => p.v_CodInterno.Equals(pstrCodInterno) &&
                        p.i_Eliminado == 0 && p.i_EsActivo == 1 && p.i_EsActivoFijo == 0);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public productoshortDto DevolverArticuloPorCodInterno(string pstrCodInterno, string pstrIdCliente,
            int pintIdAlmacen)
        {
            //Con Tarifario
            try
            {
                var periodo = Globals.ClientSession.i_Periodo.ToString();

                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.producto

                                 join D in dbContext.productodetalle on n.v_IdProducto equals D.v_IdProducto into
                                     D_join
                                 from D in D_join.DefaultIfEmpty()

                                 join J4 in dbContext.datahierarchy on new { a = n.i_IdUnidadMedida.Value, b = 17 }
                                     equals new { a = J4.i_ItemId, b = J4.i_GroupId } into J4_join
                                 from J4 in J4_join.DefaultIfEmpty()

                                 join c in dbContext.cliente on new { c = pstrIdCliente } equals new { c = c.v_IdCliente } into c_join
                                 from c in c_join.DefaultIfEmpty()

                                 join J2 in dbContext.listaprecio on
                                     new { IdListaPrecios = c.i_IdListaPrecios.Value, IdAlmacen = pintIdAlmacen, eliminado = 0 }
                                     equals new { IdListaPrecios = J2.i_IdLista.Value, IdAlmacen = J2.i_IdAlmacen.Value, eliminado = J2.i_Eliminado.Value }
                                     into J2_join

                                 from J2 in J2_join.DefaultIfEmpty()

                                 join J5 in dbContext.listapreciodetalle on new { J2.v_IdListaPrecios, eliminado = 0 }
                                 equals new { J5.v_IdListaPrecios, eliminado = J5.i_Eliminado.Value } into J5_join
                                 from J5 in J5_join.DefaultIfEmpty()

                                 join J6 in dbContext.linea on new { n.v_IdLinea } equals new { J6.v_IdLinea } into J6_join
                                 from J6 in J6_join.DefaultIfEmpty()

                                 join J7 in dbContext.datahierarchy on new { a = n.i_IdPerfilDetraccion.Value, b = 176 }
                                     equals new { a = J7.i_ItemId, b = J7.i_GroupId } into J7_join
                                 from J7 in J7_join.DefaultIfEmpty()

                                 join J3 in dbContext.productoalmacen on new { a = D.v_IdProductoDetalle, b = pintIdAlmacen, eliminado = 0, pperiodo = periodo }
                                 equals new { a = J3.v_ProductoDetalleId, b = J3.i_IdAlmacen, eliminado = 0, pperiodo = J3.v_Periodo } into J3_join
                                 from J3 in J3_join.DefaultIfEmpty()

                                 where n.i_Eliminado == 0
                                     && J3.v_ProductoDetalleId == J5.v_IdProductoDetalle && J3.i_IdAlmacen == J5.i_IdAlmacen
                                     && n.v_CodInterno == pstrCodInterno && n.i_EsActivo == 1 && n.i_EsActivoFijo == 0 && J5.i_Eliminado == 0

                                 select new productoshortDto
                                 {
                                     v_IdProducto = n.v_IdProducto,
                                     v_IdProductoDetalle = D.v_IdProductoDetalle,
                                     v_Descripcion = n.v_Descripcion,
                                     v_CodInterno = n.v_CodInterno,
                                     d_Precio = J5.d_Precio,
                                     d_Descuento = J5.d_Descuento,
                                     i_EsServicio = n.i_EsServicio,
                                     i_EsLote = n.i_EsLote,
                                     i_IdTipoProducto = n.i_IdTipoProducto,
                                     stockActual = J3.d_StockActual,
                                     v_IdProductoAlmacen = J3.v_IdProductoAlmacen, //se agrego al final
                                     d_separacion = J3.d_SeparacionTotal, // se agrego al final
                                     i_IdUnidadMedida = n.i_IdUnidadMedida,
                                     EmpaqueUnidadMedida = J4.v_Value1,
                                     d_Empaque = n.d_Empaque,
                                     i_EsAfectoDetraccion = n.i_EsAfectoDetraccion,
                                     i_NombreEditable = n.i_NombreEditable,
                                     i_ValidarStock = n.i_ValidarStock,
                                     i_EsAfectoPercepcion = n.i_EsAfectoPercepcion,
                                     d_TasaPercepcion = n.d_TasaPercepcion,
                                     IdMoneda = J2 != null ? J2.i_IdMoneda.Value : -1,
                                     NroCuentaVenta = J6.v_NroCuentaVenta,
                                     NroCuentaCompra = J6.v_NroCuentaCompra,
                                     v_NroPedidoExportacion = J3.v_NroPedido,
                                     EsAfectoIsc = n.i_EsAfectoIsc != null && n.i_EsAfectoIsc == 1,
                                     v_Descripcion2 = n.v_Descripcion2,
                                     TasaDetraccion = J7 != null ? J7.v_Value2 : "0",
                                     TopeDetraccion = J7 != null ? J7.v_Field : "0"
                                 }
                            ).FirstOrDefault();

                    if (query != null) return query;

                    var query2 = (from n in dbContext.producto
                                  join D in dbContext.productodetalle on n.v_IdProducto equals D.v_IdProducto into D_join
                                  from D in D_join.DefaultIfEmpty()
                                  join J3 in dbContext.productoalmacen on
                                      new { a = D.v_IdProductoDetalle, b = pintIdAlmacen, eliminado = 0, pperiodo = periodo }
                                      equals
                                      new
                                      {
                                          a = J3.v_ProductoDetalleId,
                                          b = J3.i_IdAlmacen,
                                          eliminado = J3.i_Eliminado.Value,
                                          pperiodo = J3.v_Periodo
                                      }
                                      into J3Join
                                  from J3 in J3Join.DefaultIfEmpty()
                                  join J4 in dbContext.datahierarchy on new { a = n.i_IdUnidadMedida.Value, b = 17 }
                                      equals new { a = J4.i_ItemId, b = J4.i_GroupId } into J4_join
                                  from J4 in J4_join.DefaultIfEmpty()

                                  join J5 in dbContext.linea on n.v_IdLinea equals J5.v_IdLinea into J5_join
                                  from J5 in J5_join.DefaultIfEmpty()

                                  join J7 in dbContext.datahierarchy on new { a = n.i_IdPerfilDetraccion.Value, b = 176 }
                                      equals new { a = J7.i_ItemId, b = J7.i_GroupId } into J7_join
                                  from J7 in J7_join.DefaultIfEmpty()

                                  where
                                      n.i_Eliminado == 0 && (n.i_EsServicio == 1 || J3.v_IdProductoAlmacen != "N002-PA000000000") &&
                                      n.v_CodInterno == pstrCodInterno && n.i_EsActivo == 1

                                  select new productoshortDto
                                  {
                                      v_IdProducto = n.v_IdProducto,
                                      v_IdProductoDetalle = D.v_IdProductoDetalle,
                                      v_Descripcion = n.v_Descripcion,
                                      v_CodInterno = n.v_CodInterno,
                                      i_EsServicio = n.i_EsServicio,
                                      i_EsLote = n.i_EsLote,
                                      i_IdTipoProducto = n.i_IdTipoProducto,
                                      stockActual = J3.d_StockActual,
                                      v_IdProductoAlmacen = J3.v_IdProductoAlmacen,
                                      d_separacion = J3.d_SeparacionTotal,
                                      i_IdUnidadMedida = n.i_IdUnidadMedida,
                                      EmpaqueUnidadMedida = J4.v_Value1,
                                      d_Empaque = n.d_Empaque,
                                      i_EsAfectoDetraccion = n.i_EsAfectoDetraccion,
                                      i_NombreEditable = n.i_NombreEditable,
                                      StockDisponible = J3.d_StockActual - J3.d_SeparacionTotal,
                                      i_ValidarStock = n.i_ValidarStock,
                                      i_EsAfectoPercepcion = n.i_EsAfectoPercepcion,
                                      d_TasaPercepcion = n.d_TasaPercepcion,
                                      NroCuentaVenta = J5.v_NroCuentaVenta,
                                      NroCuentaCompra = J5.v_NroCuentaCompra,
                                      v_NroPedidoExportacion = J3.v_NroPedido,
                                      Observacion = n.v_Caracteristica,
                                      EsAfectoIsc = n.i_EsAfectoIsc != null && n.i_EsAfectoIsc == 1,
                                      v_Descripcion2 = n.v_Descripcion2,
                                      TasaDetraccion = J7 != null ? J7.v_Value2 : "0",
                                      TopeDetraccion = J7 != null ? J7.v_Field : "0"
                                  }
                            ).FirstOrDefault();

                    return query2;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }



        public productoshortDto DevolverArticuloPorCodInternoLista1(string pstrCodInterno, string pstrIdCliente,
           int pintIdAlmacen)
        {
            //Con Tarifario
            try
            {

                ///PRUEBAS CAMBIO DE LISTAPRECIODETALLE
                var periodo = Globals.ClientSession.i_Periodo.ToString();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region QueryCompilado
                    var query = (from n in dbContext.producto

                                 join D in dbContext.productodetalle on n.v_IdProducto equals D.v_IdProducto into
                                     D_join
                                 from D in D_join.DefaultIfEmpty()

                                 join J4 in dbContext.datahierarchy on new { a = n.i_IdUnidadMedida.Value, b = 17 }
                                     equals new { a = J4.i_ItemId, b = J4.i_GroupId } into J4_join
                                 from J4 in J4_join.DefaultIfEmpty()

                                 //join c in dbContext.cliente on new { c = pstrIdCliente } equals new { c = c.v_IdCliente } into c_join
                                 //from c in c_join.DefaultIfEmpty()
                                 join J2 in dbContext.listaprecio on
                                     new { IdListaPrecios = 1, IdAlmacen = pintIdAlmacen, eliminado = 0 }
                                     equals new { IdListaPrecios = J2.i_IdLista.Value, IdAlmacen = J2.i_IdAlmacen.Value, eliminado = J2.i_Eliminado.Value }
                                     into J2_join
                                 from J2 in J2_join.DefaultIfEmpty()

                                 join J5 in dbContext.listapreciodetalle on new { J2.v_IdListaPrecios, eliminado = 0 } equals new { J5.v_IdListaPrecios, eliminado = J5.i_Eliminado.Value } into J5_join
                                 from J5 in J5_join.DefaultIfEmpty()

                                 join J6 in dbContext.linea on n.v_IdLinea equals J6.v_IdLinea into J6_join
                                 from J6 in J6_join.DefaultIfEmpty()


                                 join J3 in dbContext.productoalmacen on new { a = D.v_IdProductoDetalle, b = pintIdAlmacen, IdProductoAlmacen = J5.v_IdProductoDetalle, eliminado = 0, pperiodo = periodo } equals new { a = J3.v_ProductoDetalleId, b = J3.i_IdAlmacen, IdProductoAlmacen = J3.v_ProductoDetalleId, eliminado = 0, pperiodo = J3.v_Periodo } into J3_join
                                 from J3 in J3_join.DefaultIfEmpty()
                                 where n.i_Eliminado == 0

                                     && J3.v_ProductoDetalleId == J5.v_IdProductoDetalle && J3.i_IdAlmacen == J5.i_IdAlmacen
                                     && n.v_CodInterno == pstrCodInterno && n.i_EsActivo == 1 && n.i_EsActivoFijo == 0 && J5.i_Eliminado == 0

                                 select new productoshortDto
                                 {
                                     v_IdProducto = n.v_IdProducto,
                                     v_IdProductoDetalle = D.v_IdProductoDetalle,
                                     v_Descripcion = n.v_Descripcion,
                                     v_CodInterno = n.v_CodInterno,
                                     d_Precio = J5.d_Precio,
                                     d_Descuento = J5.d_Descuento,
                                     i_EsServicio = n.i_EsServicio,
                                     i_EsLote = n.i_EsLote,
                                     i_IdTipoProducto = n.i_IdTipoProducto,
                                     stockActual = J3.d_StockActual,
                                     v_IdProductoAlmacen = J3.v_IdProductoAlmacen, //se agrego al final
                                     d_separacion = J3.d_SeparacionTotal, // se agrego al final
                                     i_IdUnidadMedida = n.i_IdUnidadMedida,
                                     EmpaqueUnidadMedida = J4.v_Value1,
                                     d_Empaque = n.d_Empaque,
                                     i_EsAfectoDetraccion = n.i_EsAfectoDetraccion,
                                     i_NombreEditable = n.i_NombreEditable,
                                     i_ValidarStock = n.i_ValidarStock,
                                     i_EsAfectoPercepcion = n.i_EsAfectoPercepcion,
                                     d_TasaPercepcion = n.d_TasaPercepcion,
                                     IdMoneda = J2 != null ? J2.i_IdMoneda.Value : -1,
                                     NroCuentaVenta = J6.v_NroCuentaVenta,
                                     NroCuentaCompra = J6.v_NroCuentaCompra,
                                     v_NroPedidoExportacion = J3.v_NroPedido,
                                     EsAfectoIsc = n.i_EsAfectoIsc != null ? n.i_EsAfectoIsc == 1 : false,
                                     v_Descripcion2 = n.v_Descripcion2,
                                 }
                            ).FirstOrDefault();


                    #endregion

                    if (query == null)
                    {
                        var query2 = (from n in dbContext.producto
                                      join D in dbContext.productodetalle on n.v_IdProducto equals D.v_IdProducto into D_join
                                      from D in D_join.DefaultIfEmpty()
                                      join J3 in dbContext.productoalmacen on
                                          new { a = D.v_IdProductoDetalle, b = pintIdAlmacen, eliminado = 0, pperiodo = periodo }
                                          equals
                                          new
                                          {
                                              a = J3.v_ProductoDetalleId,
                                              b = J3.i_IdAlmacen,
                                              eliminado = J3.i_Eliminado.Value,
                                              pperiodo = J3.v_Periodo
                                          }

                                      join J4 in dbContext.datahierarchy on new { a = n.i_IdUnidadMedida.Value, b = 17 }
                                          equals new { a = J4.i_ItemId, b = J4.i_GroupId } into J4_join
                                      from J4 in J4_join.DefaultIfEmpty()

                                      join J5 in dbContext.linea on n.v_IdLinea equals J5.v_IdLinea into J5_join
                                      from J5 in J5_join.DefaultIfEmpty()

                                      where
                                          n.i_Eliminado == 0 && J3.v_IdProductoAlmacen != "N002-PA000000000" &&
                                          n.v_CodInterno == pstrCodInterno && n.i_EsActivo == 1

                                      select new productoshortDto
                                      {
                                          v_IdProducto = n.v_IdProducto,
                                          v_IdProductoDetalle = D.v_IdProductoDetalle,
                                          v_Descripcion = n.v_Descripcion,
                                          v_CodInterno = n.v_CodInterno,
                                          i_EsServicio = n.i_EsServicio,
                                          i_EsLote = n.i_EsLote,
                                          i_IdTipoProducto = n.i_IdTipoProducto,
                                          stockActual = J3.d_StockActual,
                                          v_IdProductoAlmacen = J3.v_IdProductoAlmacen,
                                          d_separacion = J3.d_SeparacionTotal,
                                          i_IdUnidadMedida = n.i_IdUnidadMedida,
                                          EmpaqueUnidadMedida = J4.v_Value1,
                                          d_Empaque = n.d_Empaque,
                                          i_EsAfectoDetraccion = n.i_EsAfectoDetraccion,
                                          i_NombreEditable = n.i_NombreEditable,
                                          StockDisponible = J3.d_StockActual - J3.d_SeparacionTotal,
                                          i_ValidarStock = n.i_ValidarStock,
                                          i_EsAfectoPercepcion = n.i_EsAfectoPercepcion,
                                          d_TasaPercepcion = n.d_TasaPercepcion,
                                          NroCuentaVenta = J5.v_NroCuentaVenta,
                                          NroCuentaCompra = J5.v_NroCuentaCompra,
                                          v_NroPedidoExportacion = J3.v_NroPedido,
                                          Observacion = n.v_Caracteristica,
                                          EsAfectoIsc = n.i_EsAfectoIsc != null ? n.i_EsAfectoIsc == 1 : false,
                                          v_Descripcion2 = n.v_Descripcion2,
                                      }
                            ).FirstOrDefault();

                        return query2;
                    }
                    else
                    {
                        return query;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }



        public productoshortDto DevolverArticuloPorCodInterno(string pstrCodInterno, int pintIdAlmacen)
        {
            //Sin Tarifario
            OperationResult pobjOperationResult = new Common.Resource.OperationResult();
            try
            {

                var periodo = Globals.ClientSession.i_Periodo.ToString();
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region QueryCompilado
                    var query = (from n in dbContext.producto

                                 join D in dbContext.productodetalle on n.v_IdProducto equals D.v_IdProducto into
                                     D_join
                                 from D in D_join.DefaultIfEmpty()

                                 join J4 in dbContext.datahierarchy on new { a = n.i_IdUnidadMedida.Value, b = 17 }
                                     equals new { a = J4.i_ItemId, b = J4.i_GroupId } into J4_join
                                 from J4 in J4_join.DefaultIfEmpty()

                                 join J6 in dbContext.linea on n.v_IdLinea equals J6.v_IdLinea into J6_join
                                 from J6 in J6_join.DefaultIfEmpty()

                                 join J7 in dbContext.datahierarchy on new { a = n.i_IdPerfilDetraccion.Value, b = 176 }
                                      equals new { a = J7.i_ItemId, b = J7.i_GroupId } into J7_join
                                 from J7 in J7_join.DefaultIfEmpty()

                                 join J3 in dbContext.productoalmacen on
                                     new
                                     {
                                         a = D.v_IdProductoDetalle,
                                         b = pintIdAlmacen,
                                         eliminado = 0,
                                         pperiodo = periodo
                                     }
                                     equals
                                     new
                                     {
                                         a = J3.v_ProductoDetalleId,
                                         b = J3.i_IdAlmacen,
                                         eliminado = 0,
                                         pperiodo = J3.v_Periodo
                                     }

                                 where
                                     n.i_Eliminado == 0 &&
                                     n.v_CodInterno == pstrCodInterno && n.i_EsActivo == 1 && n.i_EsActivoFijo == 0
                                 select new productoshortDto
                                 {
                                     v_IdProducto = n.v_IdProducto,
                                     v_IdProductoDetalle = D.v_IdProductoDetalle,
                                     v_Descripcion = n.v_Descripcion,
                                     v_CodInterno = n.v_CodInterno,
                                     d_Precio = n.d_PrecioVenta,
                                     d_Descuento = 0,
                                     i_EsServicio = n.i_EsServicio,
                                     i_EsLote = n.i_EsLote,
                                     i_IdTipoProducto = n.i_IdTipoProducto,
                                     stockActual = J3.d_StockActual,
                                     v_IdProductoAlmacen = J3.v_IdProductoAlmacen, //se agrego al final
                                     d_separacion = J3.d_SeparacionTotal, // se agrego al final
                                     i_IdUnidadMedida = n.i_IdUnidadMedida,
                                     EmpaqueUnidadMedida = J4.v_Value1,
                                     d_Empaque = n.d_Empaque,
                                     i_EsAfectoDetraccion = n.i_EsAfectoDetraccion,
                                     i_NombreEditable = n.i_NombreEditable,
                                     i_ValidarStock = n.i_ValidarStock,
                                     i_EsAfectoPercepcion = n.i_EsAfectoPercepcion,
                                     d_TasaPercepcion = n.d_TasaPercepcion,
                                     IdMoneda = 1,
                                     NroCuentaVenta = J6.v_NroCuentaVenta,
                                     NroCuentaCompra = J6.v_NroCuentaCompra,
                                     v_NroPedidoExportacion = J3.v_NroPedido,
                                     EsAfectoIsc = n.i_EsAfectoIsc != null && n.i_EsAfectoIsc == 1,
                                     v_Descripcion2 = n.v_Descripcion2,
                                     TasaDetraccion = J7 != null ? J7.v_Value2 : "0",
                                     TopeDetraccion = J7 != null ? J7.v_Field : "0"
                                 }
                            ).FirstOrDefault();

                    #endregion

                    if (query == null)
                    {
                        var query2 = (from n in dbContext.producto
                                      join D in dbContext.productodetalle on n.v_IdProducto equals D.v_IdProducto into D_join
                                      from D in D_join.DefaultIfEmpty()
                                      join J3 in dbContext.productoalmacen on
                                          new { a = D.v_IdProductoDetalle, b = pintIdAlmacen, eliminado = 0, pperiodo = periodo }
                                          equals
                                          new
                                          {
                                              a = J3.v_ProductoDetalleId,
                                              b = J3.i_IdAlmacen,
                                              eliminado = J3.i_Eliminado.Value,
                                              pperiodo = J3.v_Periodo
                                          }

                                      join J4 in dbContext.datahierarchy on new { a = n.i_IdUnidadMedida.Value, b = 17 }
                                          equals new { a = J4.i_ItemId, b = J4.i_GroupId } into J4_join
                                      from J4 in J4_join.DefaultIfEmpty()

                                      join J5 in dbContext.linea on n.v_IdLinea equals J5.v_IdLinea into J5_join
                                      from J5 in J5_join.DefaultIfEmpty()
                                      join J7 in dbContext.datahierarchy on new { a = n.i_IdPerfilDetraccion.Value, b = 176 }
                                                                            equals new { a = J7.i_ItemId, b = J7.i_GroupId } into J7_join
                                      from J7 in J7_join.DefaultIfEmpty()
                                      where
                                          n.i_Eliminado == 0 && J3.v_IdProductoAlmacen != "N002-PA000000000" &&
                                          n.v_CodInterno == pstrCodInterno && n.i_EsActivo == 1

                                      select new productoshortDto
                                      {
                                          v_IdProducto = n.v_IdProducto,
                                          v_IdProductoDetalle = D.v_IdProductoDetalle,
                                          v_Descripcion = n.v_Descripcion,
                                          v_CodInterno = n.v_CodInterno,
                                          i_EsServicio = n.i_EsServicio,
                                          i_EsLote = n.i_EsLote,
                                          i_IdTipoProducto = n.i_IdTipoProducto,
                                          stockActual = J3.d_StockActual,
                                          v_IdProductoAlmacen = J3.v_IdProductoAlmacen,
                                          d_separacion = J3.d_SeparacionTotal,
                                          i_IdUnidadMedida = n.i_IdUnidadMedida,
                                          EmpaqueUnidadMedida = J4.v_Value1,
                                          d_Empaque = n.d_Empaque,
                                          i_EsAfectoDetraccion = n.i_EsAfectoDetraccion,
                                          i_NombreEditable = n.i_NombreEditable,
                                          StockDisponible = J3.d_StockActual - J3.d_SeparacionTotal,
                                          i_ValidarStock = n.i_ValidarStock,
                                          i_EsAfectoPercepcion = n.i_EsAfectoPercepcion,
                                          d_TasaPercepcion = n.d_TasaPercepcion,
                                          NroCuentaVenta = J5.v_NroCuentaVenta,
                                          NroCuentaCompra = J5.v_NroCuentaCompra,
                                          v_NroPedidoExportacion = J3.v_NroPedido,
                                          Observacion = n.v_Caracteristica,
                                          EsAfectoIsc = n.i_EsAfectoIsc != null && n.i_EsAfectoIsc == 1,
                                          v_Descripcion2 = n.v_Descripcion2,
                                          TasaDetraccion = J7 != null ? J7.v_Value2 : "0",
                                          TopeDetraccion = J7 != null ? J7.v_Field : "0"
                                      }
                            ).FirstOrDefault();

                        return query2;
                    }
                    else
                    {
                        return query;
                    }
                }
            }
            catch (Exception ex)
            {


                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.DevolverArticuloPorCodInterno()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;

                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);

                throw;
            }
        }
        public string InsertarVentaImportada(ref OperationResult pobjOperationResult, ventaDto pobjDtoEntity,
            List<string> ClientSession, List<ventadetalleDto> pTemp_Insertar, bool GeneraNotaSalida,
            bool GeneraPendientesXCobrar)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    dbContext.venta.MergeOption = MergeOption.NoTracking;
                    dbContext.ventadetalle.MergeOption = MergeOption.NoTracking;
                    dbContext.ventadetalle.EnablePlanCaching = true;

                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    DocumentoBL _objDocumentoBL = new DocumentoBL();
                    venta objEntityVenta = ventaAssembler.ToEntity(pobjDtoEntity);
                    ventadetalleDto pobjDtoVentaDetalle = new ventadetalleDto();

                    int SecuentialId = 0;
                    string newIdVenta = string.Empty;
                    string newIdVentaDetalle = string.Empty;
                    int intNodeId;

                    if (GeneraNotaSalida && pTemp_Insertar.Where(p => p.v_IdProductoDetalle != null).Count() > 0 &&
                        pobjDtoEntity.i_IdEstado == 1)
                    {
                        #region Genera Nota de Salida

                        movimientoDto _movimientoDto = new movimientoDto();
                        movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
                        MovimientoBL _objMovimientoBL = new MovimientoBL();
                        List<string> Almacenes = new List<string>();
                        List<movimientodetalleDto> _movimientodetalleDtos = new List<movimientodetalleDto>();


                        if ((pTemp_Insertar.Find(p => p.EsServicio == 0) != null))
                        {
                            List<int?> pAlmacenes = pTemp_Insertar.Select(p => p.i_IdAlmacen).Distinct().ToList();

                            List<KeyValueDTO> ListaMovimientos = new List<KeyValueDTO>();
                            ListaMovimientos = _objMovimientoBL.ObtenerListadoMovimientos(ref pobjOperationResult,
                                objEntityVenta.v_Periodo, objEntityVenta.v_Mes, (int)TipoDeMovimiento.NotadeSalida);

                            int MaxMovimiento;
                            MaxMovimiento = ListaMovimientos.Count() > 0
                                ? int.Parse(ListaMovimientos[ListaMovimientos.Count() - 1].Value1.ToString())
                                : 0;

                            foreach (int? pAlmacen in pAlmacenes)
                            {
                                _movimientoDto = new movimientoDto();
                                MaxMovimiento++;
                                _movimientoDto.d_TipoCambio = objEntityVenta.d_TipoCambio;
                                _movimientoDto.i_IdAlmacenOrigen = pAlmacen;
                                _movimientoDto.i_IdMoneda = objEntityVenta.i_IdMoneda;
                                _movimientoDto.i_IdTipoMotivo = 1; //Compra nacional
                                _movimientoDto.t_Fecha = objEntityVenta.t_FechaRegistro;
                                _movimientoDto.v_Mes = objEntityVenta.v_Mes.Trim();
                                _movimientoDto.v_Periodo = objEntityVenta.v_Periodo.Trim();
                                _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeSalida;
                                _movimientoDto.v_Correlativo = MaxMovimiento.ToString("00000000");
                                _movimientoDto.v_IdCliente = objEntityVenta.v_IdCliente;
                                _movimientoDto.v_OrigenTipo = "V";
                                // _movimientoDto.i_EsDevolucion = objEntityVenta.i_IdTipoDocumento.Value == 7 ? 1 : 0;
                                _movimientoDto.i_EsDevolucion =
                                    _objDocumentoBL.DocumentoEsInverso(objEntityVenta.i_IdTipoDocumento.Value) ? 1 : 0;
                                _movimientoDto.v_OrigenRegCorrelativo = objEntityVenta.v_Correlativo;
                                _movimientoDto.v_OrigenRegMes = objEntityVenta.v_Mes;
                                _movimientoDto.v_OrigenRegPeriodo = objEntityVenta.v_Periodo;
                                _movimientoDto.d_TotalPrecio = objEntityVenta.d_Total;

                                foreach (
                                    ventadetalleDto _ventadetalleDto in
                                        pTemp_Insertar.Where(
                                            p =>
                                                p.EsServicio == 0 && p.i_IdAlmacen == pAlmacen &&
                                                p.v_IdProductoDetalle != null).ToList())
                                {
                                    _movimientodetalleDto = new movimientodetalleDto();
                                    _movimientodetalleDto.v_IdProductoDetalle =
                                        _ventadetalleDto.v_IdProductoDetalle.Trim();
                                    _movimientodetalleDto.i_IdTipoDocumento = objEntityVenta.i_IdTipoDocumento;
                                    _movimientodetalleDto.v_NumeroDocumento = objEntityVenta.v_SerieDocumento.Trim() +
                                                                              "-" + objEntityVenta.v_Correlativo.Trim();
                                    _movimientodetalleDto.d_Cantidad = _ventadetalleDto.d_Cantidad;
                                    _movimientodetalleDto.i_IdUnidad = _ventadetalleDto.i_IdUnidadMedida;
                                    _movimientodetalleDto.d_Precio = _ventadetalleDto.d_Precio;
                                    _movimientodetalleDto.d_Total = _ventadetalleDto.d_Cantidad *
                                                                    _ventadetalleDto.d_Precio;
                                    _movimientodetalleDto.d_CantidadEmpaque = _ventadetalleDto.d_CantidadEmpaque;
                                    _movimientodetalleDtos.Add(_movimientodetalleDto);
                                }

                                if (_movimientodetalleDtos.Count() > 0)
                                {
                                    _movimientoDto.d_TotalCantidad = _movimientodetalleDtos.Sum(p => p.d_Cantidad.Value);
                                    _movimientoDto.d_TotalPrecio = _movimientodetalleDtos.Sum(p => p.d_Total.Value);
                                    _objMovimientoBL.InsertarMovimiento(ref pobjOperationResult, _movimientoDto,
                                        Globals.ClientSession.GetAsList(), _movimientodetalleDtos);
                                    if (pobjOperationResult.Success == 0) return string.Empty;
                                }
                                _movimientodetalleDtos = new List<movimientodetalleDto>();
                            }
                        }


                        #endregion
                    }

                    #region Inserta Cabecera

                    objEntityVenta.t_InsertaFecha = DateTime.Now;
                    objEntityVenta.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntityVenta.i_Eliminado = 0;

                    intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 40);
                    newIdVenta = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZQ");
                    objEntityVenta.v_IdVenta = newIdVenta;

                    decimal Redondeado, Residuo;

                    if (Globals.ClientSession.i_RedondearVentas == 1)
                    {
                        #region Ingresa Residuo si es Necesario

                        Redondeado =
                            decimal.Parse(
                                Math.Round(objEntityVenta.d_Total.Value, 2, MidpointRounding.AwayFromZero)
                                    .ToString("0.00"));
                        Residuo = (objEntityVenta.d_Total.Value - Redondeado) * -1;

                        if (Residuo != 0)
                        {
                            ventadetalleDto _ventadetalleDto = new ventadetalleDto();
                            _ventadetalleDto.i_IdUnidadMedida = 15;
                            _ventadetalleDto.d_Cantidad = 1;
                            _ventadetalleDto.d_Precio = Residuo;
                            _ventadetalleDto.i_Anticipio = 0;
                            _ventadetalleDto.v_NroCuenta = Residuo < 0 ? "6792103" : "7792103";
                            _ventadetalleDto.v_DescripcionProducto = "REDONDEO";
                            _ventadetalleDto.v_IdProductoDetalle = "N002-PE000000000";
                            _ventadetalleDto.i_IdAlmacen = 1;
                            _ventadetalleDto.d_Valor = Residuo;
                            _ventadetalleDto.d_ValorVenta = Residuo - (Residuo * (18 / 100));
                            _ventadetalleDto.d_Igv = Residuo * (18 / 100);
                            _ventadetalleDto.d_PrecioVenta = Residuo;
                            pTemp_Insertar.Add(_ventadetalleDto);
                            objEntityVenta.d_ValorVenta = pTemp_Insertar.Sum(p => p.d_ValorVenta.Value);
                            objEntityVenta.d_IGV = pTemp_Insertar.Sum(p => p.d_Igv);
                            objEntityVenta.d_Total = pTemp_Insertar.Sum(p => p.d_PrecioVenta);
                            objEntityVenta.d_Valor = pTemp_Insertar.Sum(p => p.d_Valor.Value);
                        }

                        #endregion
                    }

                    dbContext.AddToventa(objEntityVenta);

                    #endregion

                    #region Inserta Detalle

                    foreach (ventadetalleDto ventadetalleDto in pTemp_Insertar)
                    {
                        var NotadeSalidaDetalle = (from n in dbContext.movimientodetalle

                                                   join J1 in dbContext.movimiento on
                                                       new { idMovimiento = n.v_IdMovimiento, IdAlmacen = ventadetalleDto.i_IdAlmacen }
                                                       equals new { idMovimiento = J1.v_IdMovimiento, IdAlmacen = J1.i_IdAlmacenOrigen } into
                                                       J1_join
                                                   from J1 in J1_join.DefaultIfEmpty()

                                                   where
                                                       n.i_Eliminado == 0 && n.v_IdProductoDetalle == ventadetalleDto.v_IdProductoDetalle &&
                                                       J1.v_OrigenTipo == "V" && J1.v_OrigenRegPeriodo == objEntityVenta.v_Periodo
                                                       && J1.v_OrigenRegMes == objEntityVenta.v_Mes &&
                                                       J1.v_OrigenRegCorrelativo == objEntityVenta.v_Correlativo

                                                   select new { n.v_IdMovimientoDetalle }).FirstOrDefault();

                        ventadetalle objEntityVentaDetalle = ventadetalleAssembler.ToEntity(ventadetalleDto);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 41);
                        newIdVentaDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZN");
                        objEntityVentaDetalle.v_IdVentaDetalle = newIdVentaDetalle;
                        objEntityVentaDetalle.v_IdVenta = newIdVenta;
                        objEntityVentaDetalle.v_IdMovimientoDetalle = NotadeSalidaDetalle != null
                            ? NotadeSalidaDetalle.v_IdMovimientoDetalle
                            : null;
                        objEntityVentaDetalle.t_InsertaFecha = DateTime.Now;
                        objEntityVentaDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityVentaDetalle.i_Eliminado = 0;
                        dbContext.AddToventadetalle(objEntityVentaDetalle);
                    }
                    dbContext.SaveChanges();

                    #endregion

                    if (GeneraPendientesXCobrar && pobjDtoEntity.i_IdEstado == 1)
                    {
                        #region Genera Cobranza Pendiente

                        if (newIdVenta != null)
                            if (objEntityVenta.i_IdEstado == 1)
                                InsertarCobranzaPendiente(ref pobjOperationResult, newIdVenta,
                                    objEntityVenta.d_Total.Value, Globals.ClientSession.GetAsList());
                        if (pobjOperationResult.Success == 0) return string.Empty;

                        #endregion

                        #region Genera Libro Diario
                        if (_objDocumentoBL.DocumentoEsContable(pobjDtoEntity.i_IdTipoDocumento.Value)) //Nota de credito y nota de debito no generan Diarios
                        {
                            string IDConcepto = pobjDtoEntity.i_IdTipoVenta.Value.ToString("00");

                            var aa = (from a in dbContext.administracionconceptos
                                      where a.v_Codigo == IDConcepto && a.v_Periodo.Equals(periodo)
                                      select new { a.v_CuentaIGV, a.v_CuentaPVenta }).FirstOrDefault();

                            if (aa != null && aa.v_CuentaIGV.Trim() != string.Empty &&
                                aa.v_CuentaPVenta.Trim() != string.Empty)
                            {
                                DiarioBL _objDiarioBL = new DiarioBL();
                                diarioDto _diarioDto = new diarioDto();
                                List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                                List<diariodetalleDto> TempXInsertar = new List<diariodetalleDto>();

                                var DetalleVenta = (from d in dbContext.ventadetalle
                                                    where d.v_IdVenta == newIdVenta && d.i_Eliminado == 0
                                                    select d).ToList();

                                diariodetalleDto H_SubTotalVenta = new diariodetalleDto();
                                diariodetalleDto H_IGV = new diariodetalleDto();
                                diariodetalleDto D_PrecioVenta = new diariodetalleDto();
                                diariodetalleDto H_Percepcion = new diariodetalleDto();


                                #region Diario Cabecera

                                OperationResult objOperationResult = new OperationResult();
                                _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref pobjOperationResult,
                                    pobjDtoEntity.t_FechaRegistro.Value.Year.ToString(),
                                    pobjDtoEntity.t_FechaRegistro.Value.Month.ToString("00"), 337);

                                int _MaxMovimiento;
                                _MaxMovimiento = _ListadoDiarios.Count() > 0
                                    ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString())
                                    : 0;
                                _MaxMovimiento++;
                                _diarioDto.v_IdDocumentoReferencia = newIdVenta;
                                _diarioDto.v_Periodo = pobjDtoEntity.t_FechaRegistro.Value.Year.ToString();
                                _diarioDto.v_Mes =
                                    int.Parse(pobjDtoEntity.t_FechaRegistro.Value.Month.ToString()).ToString("00");
                                _diarioDto.v_Nombre = pobjDtoEntity.NombreCliente;
                                _diarioDto.v_Glosa = pobjDtoEntity.v_Concepto;
                                _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                                _diarioDto.d_TipoCambio = pobjDtoEntity.d_TipoCambio.Value;
                                _diarioDto.i_IdMoneda = pobjDtoEntity.i_IdMoneda.Value;
                                _diarioDto.i_IdTipoDocumento = 337; // D/V = Diario de venta
                                _diarioDto.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                _diarioDto.i_IdTipoComprobante = 2;

                                #endregion

                                List<string> CuentasDetalle = new List<string>();

                                CuentasDetalle = DetalleVenta.Select(p => p.v_NroCuenta).Distinct().ToList();

                                #region SubTotalVenta

                                foreach (string CuentaDetalle in CuentasDetalle)
                                {
                                    decimal SubTotal =
                                        DetalleVenta.Where(p => p.v_NroCuenta == CuentaDetalle)
                                            .Sum(o => o.d_ValorVenta.Value);
                                    H_SubTotalVenta.d_Importe = SubTotal > 0 ? SubTotal : SubTotal * -1;
                                    H_SubTotalVenta.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1
                                        ? SubTotal / pobjDtoEntity.d_TipoCambio.Value
                                        : SubTotal * pobjDtoEntity.d_TipoCambio.Value;
                                    H_SubTotalVenta.i_IdCentroCostos = "";
                                    H_SubTotalVenta.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                    H_SubTotalVenta.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    H_SubTotalVenta.v_IdCliente = pobjDtoEntity.v_IdCliente;
                                    //H_SubTotalVenta.v_Naturaleza = SubTotal > 0 ? pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D" : pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";
                                    H_SubTotalVenta.v_Naturaleza = SubTotal > 0
                                        ? !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value)
                                            ? "H"
                                            : "D"
                                        : !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value)
                                            ? "D"
                                            : "H";
                                    H_SubTotalVenta.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" +
                                                                     pobjDtoEntity.v_CorrelativoDocumento;
                                    H_SubTotalVenta.v_NroCuenta = CuentaDetalle;
                                    //if (pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "7" || pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                                    if (_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ||
                                        pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                                    {
                                        H_SubTotalVenta.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdTipoDocumentoRef.Value;
                                        H_SubTotalVenta.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                                            pobjDtoEntity.v_CorrelativoDocumentoRef;
                                    }

                                    TempXInsertar.Add(H_SubTotalVenta);
                                    H_SubTotalVenta = new diariodetalleDto();
                                }

                                #endregion

                                #region Percepción

                                if (DetalleVenta.Sum(p => p.d_Percepcion) > 0)
                                {
                                    H_Percepcion.d_Importe =
                                        decimal.Parse(
                                            Math.Round(DetalleVenta.Sum(p => p.d_Percepcion).Value, 2,
                                                MidpointRounding.AwayFromZero).ToString("0.00"));
                                    H_Percepcion.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1
                                        ? DetalleVenta.Sum(p => p.d_Percepcion.Value) / pobjDtoEntity.d_TipoCambio.Value
                                        : DetalleVenta.Sum(p => p.d_Percepcion.Value) * pobjDtoEntity.d_TipoCambio.Value;
                                    H_Percepcion.i_IdCentroCostos = "";
                                    H_Percepcion.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                    H_Percepcion.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                    H_Percepcion.v_IdCliente = pobjDtoEntity.v_IdCliente;
                                    //H_Percepcion.v_Naturaleza = pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";

                                    H_Percepcion.v_Naturaleza =
                                        !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value)
                                            ? "H"
                                            : "D";
                                    H_Percepcion.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" +
                                                                  pobjDtoEntity.v_CorrelativoDocumento;
                                    H_Percepcion.v_NroCuenta = "4011301";

                                    //if (pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "7" || pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                                    if (_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ||
                                        pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                                    {
                                        H_Percepcion.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdTipoDocumentoRef.Value;
                                        H_Percepcion.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                                         pobjDtoEntity.v_CorrelativoDocumentoRef;
                                    }
                                    TempXInsertar.Add(H_Percepcion);
                                }

                                #endregion

                                #region IGV

                                H_IGV.d_Importe =
                                    decimal.Parse(
                                        Math.Round(DetalleVenta.Sum(p => p.d_Igv.Value), 2,
                                            MidpointRounding.AwayFromZero).ToString("0.00"));
                                H_IGV.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1
                                    ? DetalleVenta.Sum(p => p.d_Igv.Value) / pobjDtoEntity.d_TipoCambio.Value
                                    : DetalleVenta.Sum(p => p.d_Igv.Value) * pobjDtoEntity.d_TipoCambio.Value;
                                H_IGV.i_IdCentroCostos = "";
                                H_IGV.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                H_IGV.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                H_IGV.v_IdCliente = pobjDtoEntity.v_IdCliente;
                                //H_IGV.v_Naturaleza = pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "H" : "D";

                                H_IGV.v_Naturaleza =
                                    !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value)
                                        ? "H"
                                        : "D";

                                H_IGV.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" +
                                                       pobjDtoEntity.v_CorrelativoDocumento;
                                H_IGV.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                     where a.v_Codigo == IDConcepto && a.v_Periodo.Equals(periodo)
                                                     select new { a.v_CuentaIGV }).First().v_CuentaIGV;
                                //if (pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "7" || pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                                if (_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ||
                                    pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                                {
                                    H_IGV.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdTipoDocumentoRef.Value;
                                    H_IGV.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                              pobjDtoEntity.v_CorrelativoDocumentoRef;
                                }
                                TempXInsertar.Add(H_IGV);

                                #endregion

                                #region PrecioVenta

                                D_PrecioVenta.d_Importe =
                                    decimal.Parse(
                                        Math.Round(
                                            DetalleVenta.Sum(p => p.d_PrecioVenta).Value +
                                            (H_Percepcion.d_Importe != null ? H_Percepcion.d_Importe.Value : 0), 2,
                                            MidpointRounding.AwayFromZero).ToString("0.00"));
                                D_PrecioVenta.d_Cambio = pobjDtoEntity.i_IdMoneda.Value == 1
                                    ? (DetalleVenta.Sum(p => p.d_PrecioVenta.Value) +
                                       (H_Percepcion.d_Importe != null ? H_Percepcion.d_Importe.Value : 0)) /
                                      pobjDtoEntity.d_TipoCambio.Value
                                    : (DetalleVenta.Sum(p => p.d_PrecioVenta.Value) +
                                       (H_Percepcion.d_Importe != null ? H_Percepcion.d_Importe.Value : 0)) *
                                      pobjDtoEntity.d_TipoCambio.Value;
                                D_PrecioVenta.i_IdCentroCostos = "";
                                D_PrecioVenta.i_IdTipoDocumento = pobjDtoEntity.i_IdTipoDocumento.Value;
                                D_PrecioVenta.t_Fecha = pobjDtoEntity.t_FechaRegistro.Value;
                                D_PrecioVenta.v_IdCliente = pobjDtoEntity.v_IdCliente;
                                //D_PrecioVenta.v_Naturaleza = pobjDtoEntity.i_IdTipoDocumento.Value.ToString() != "7" ? "D" : "H";
                                D_PrecioVenta.v_Naturaleza =
                                    !_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value)
                                        ? "D"
                                        : "H";
                                D_PrecioVenta.v_NroDocumento = pobjDtoEntity.v_SerieDocumento + "-" +
                                                               pobjDtoEntity.v_CorrelativoDocumento;
                                D_PrecioVenta.v_NroCuenta = (from a in dbContext.administracionconceptos
                                                             where a.v_Codigo == IDConcepto && a.v_Periodo.Equals(periodo)
                                                             select new { a.v_CuentaPVenta }).First().v_CuentaPVenta;

                                //if (pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "7" || pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                                if (_objDocumentoBL.DocumentoEsInverso(pobjDtoEntity.i_IdTipoDocumento.Value) ||
                                    pobjDtoEntity.i_IdTipoDocumento.Value.ToString() == "8")
                                {
                                    D_PrecioVenta.i_IdTipoDocumentoRef = pobjDtoEntity.i_IdTipoDocumentoRef.Value;
                                    D_PrecioVenta.v_NroDocumentoRef = pobjDtoEntity.v_SerieDocumentoRef + "-" +
                                                                      pobjDtoEntity.v_CorrelativoDocumentoRef;
                                }
                                TempXInsertar.Add(D_PrecioVenta);

                                #endregion

                                decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                                TotDebe = TempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                                TotHaber = TempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                                TotDebeC = TempXInsertar.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                                TotHaberC = TempXInsertar.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);

                                _diarioDto.d_TotalDebe =
                                    decimal.Parse(Math.Round(TotDebe, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                                _diarioDto.d_TotalHaber =
                                    decimal.Parse(Math.Round(TotHaber, 2, MidpointRounding.AwayFromZero)
                                        .ToString("0.00"));
                                _diarioDto.d_TotalDebeCambio =
                                    decimal.Parse(Math.Round(TotDebeC, 2, MidpointRounding.AwayFromZero)
                                        .ToString("0.00"));
                                _diarioDto.d_TotalHaberCambio =
                                    decimal.Parse(
                                        Math.Round(TotHaberC, 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                                _diarioDto.d_DiferenciaDebe =
                                    decimal.Parse(
                                        Math.Round(TotDebe - TotHaber, 2, MidpointRounding.AwayFromZero)
                                            .ToString("0.00"));
                                _diarioDto.d_DiferenciaHaber =
                                    decimal.Parse(
                                        Math.Round(TotDebeC - TotHaberC, 2, MidpointRounding.AwayFromZero)
                                            .ToString("0.00"));

                                _objDiarioBL.InsertarDiario(ref pobjOperationResult, _diarioDto,
                                    Globals.ClientSession.GetAsList(), TempXInsertar,
                                    (int)TipoMovimientoTesoreria.Ingreso);
                                if (pobjOperationResult.Success == 0) return string.Empty;
                            }
                        }

                        #endregion
                    }
                    ts.Complete();
                    pobjOperationResult.Success = 1;
                    return newIdVenta;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.InsertarVentaImportada()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return null;
            }
        }

        public string CentroCostoDeEstablecimiento(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    int IdEstablecimiento = Globals.ClientSession.i_IdEstablecimiento.Value;

                    var Establecimiento = (from n in dbContext.establecimiento
                                           where n.i_IdEstablecimiento == IdEstablecimiento
                                           select n).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return Establecimiento != null && Establecimiento.i_IdCentroCosto != null
                        ? Establecimiento.i_IdCentroCosto
                        : "0";
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "VentaBL.CentroCostoDeEstablecimiento()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                return null;
            }
        }

        public List<guiaremisiondetalleDto> ObtenerDetalleGuiaRemisionporDocumentoRef(int TipoDoc, string SerieDoc,
            string CorrelativoDoc)
        {


            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                var DetallesGuia = (from a in dbContext.guiaremision

                                    join b in dbContext.guiaremisiondetalle on new { IdGuiaRemision = a.v_IdGuiaRemision, eliminado = 0 }
                                        equals new { IdGuiaRemision = b.v_IdGuiaRemision, eliminado = b.i_Eliminado.Value } into b_join

                                    from b in b_join.DefaultIfEmpty()

                                    where
                                        a.i_Eliminado == 0 && a.i_IdTipoDocumento == TipoDoc && a.v_SerieDocumentoRef == SerieDoc &&
                                        a.v_NumeroDocumentoRef == CorrelativoDoc

                                        && a.i_IdEstado == 1

                                    select new guiaremisiondetalleDto
                                    {
                                        v_IdProductoDetalle = b.v_IdProductoDetalle,
                                        d_Cantidad = b.d_Cantidad,
                                        v_IdGuiaRemision = a.v_IdGuiaRemision,
                                        d_CantidadEmpaque = b.d_CantidadEmpaque,
                                        v_IdGuiaRemisionDetalle = b.v_IdGuiaRemisionDetalle,
                                        v_Observacion = "GRM. " + a.v_SerieGuiaRemision.Trim() + "-" + a.v_NumeroGuiaRemision.Trim(),
                                        v_IdMovimientoDetalle = a.v_IdCliente,

                                    }).ToList();
                return DetallesGuia;
            }

        }

        /// <summary>
        /// Obtiene las formas de pago según la moneda de las cuentas del documento a las que estan enlazadas en sus Relaciones.
        /// </summary>
        /// <param name="idMoneda"></param>
        /// <returns></returns>
        public List<KeyValueDTO> ObtenerFormasPagoPorMoneda(int idMoneda, bool mostrarPorAlmacen = false)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Result = new List<KeyValueDTO> { new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" } };
                    var ids = new List<int>();
                    ids = (from n in dbContext.relacionformapagodocumento
                           join J1 in dbContext.documento on n.i_CodigoDocumento equals J1.i_CodigoDocumento into J1_join
                           from J1 in J1_join.DefaultIfEmpty()

                           join J2 in dbContext.asientocontable on new { cta = J1.v_NroCuenta, e = 0, p = periodo }
                                                            equals new { cta = J2.v_NroCuenta, e = J2.i_Eliminado ?? 0, p = J2.v_Periodo } into J2_join
                           from J2 in J2_join.DefaultIfEmpty()

                           where J2.i_IdMoneda == idMoneda && J2.i_Eliminado == 0 && J1.i_Eliminado == 0

                           select new
                           {
                               idFormaPago = n.i_IdFormaPago
                           }).Select(p => p.idFormaPago).ToList();

                    if (mostrarPorAlmacen)
                    {
                        var idAlmacen = (Globals.ClientSession.i_IdAlmacenPredeterminado ?? 1).ToString();

                        ids = (from n in dbContext.datahierarchy
                               where ids.Contains(n.i_ItemId) && n.i_GroupId == 46 && n.v_Value4.Trim().Equals(idAlmacen)
                               select n.i_ItemId).ToList();
                    }

                    Result.AddRange(ids.Select(item => new KeyValueDTO { Id = item.ToString(), Value1 = NombreFormaPago(item) }));

                    return Result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string NombreFormaPago(int idFormaPago)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result =
                        dbContext.datahierarchy.FirstOrDefault(
                            p => p.i_GroupId == 46 && p.i_ItemId == idFormaPago && p.i_IsDeleted == 0);

                    return result != null ? result.v_Value1 : "No Existe";
                }
            }
            catch (Exception ex)
            {
                return "*Error*" + ex.Message;
            }
        }

        /// <summary>
        /// Devuelve el asiento por el id de la venta seleccionada.
        /// </summary>
        /// <param name="pstrIdVenta"></param>
        /// <returns></returns>
        public static string DevolverAsientoContable(string pstrIdVenta)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var asientoRef = dbContext.diario.FirstOrDefault(p => p.v_IdDocumentoReferencia.Equals(pstrIdVenta) && p.i_Eliminado == 0);
                    return asientoRef != null ? asientoRef.v_IdDiario : null;
                }
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<ventaDto> ObtenerVentasParaMigracion()
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var ventas = dbContext.venta.Where(p => p.i_Eliminado == 0);
                foreach (var venta in ventas)
                {
                    yield return venta.ToDTO();
                }
            }
        }

        public List<establecimientoalmacenDto> ObtenerEstablecimientoalmacenDtos()
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var result = dbContext.establecimientoalmacen.Where(p => p.i_Eliminado == 0).ToList();
                return result.ToDTOs();
            }
        }


    }

    public class VentaResumenBL
    {
        /// <summary>
        /// Obtiene todos los Resumen Diarios Enviados en un Rango de Fecha Determinado
        /// </summary>
        /// <param name="pobjOperationResult">Resultado de la Operacion</param>
        /// <param name="fechaInicial">Fecha Inicial</param>
        /// <param name="FechaFinal">Fecha Final</param>
        /// <returns></returns>
        public List<ventaresumenhomologacionDto> getListResumenDiario(out OperationResult pobjOperationResult, DateTime fechaInicial, DateTime FechaFinal)
        {
            pobjOperationResult = new OperationResult();
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var summarys = (from n in dbContext.ventaresumenhomologacion
                                    where n.t_FechaResumen >= fechaInicial && n.t_FechaResumen <= FechaFinal
                                    select new ventaresumenhomologacionDto
                                    {
                                        i_Idventaresumen = n.i_Idventaresumen,
                                        t_FechaResumen = n.t_FechaResumen,
                                        v_Ticket = n.v_Ticket,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        i_Estado = n.i_Estado,
                                    }).ToList();
                    pobjOperationResult.Success = 1;
                    return summarys;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        /// <summary>
        /// Obtiene el Array de Byte que contienen un Zip de Respuesta al Resumen Diario Enviado
        /// </summary>
        /// <param name="pobjOperationResult">Resultado de la Operacion</param>
        /// <param name="pintIdResumen">Id del Resumen</param>
        /// <returns>Zip File in Bytes</returns>
        public byte[] getByteZip(out OperationResult pobjOperationResult, int pintIdResumen)
        {
            pobjOperationResult = new OperationResult();
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var bytes = (from n in dbContext.ventaresumenhomologacion
                                 where n.i_Idventaresumen == pintIdResumen
                                 select n.b_FileZip).First();
                    pobjOperationResult.Success = 1;
                    return bytes;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }
    }

    /// <summary>
    /// Proceso que ejecuta un recálculo completo de las notas de Salidas de las Ventas del sistema por periodo.
    /// </summary>
    public class ReprocesarSalidasWorker
    {
        readonly BackgroundWorker bw = new BackgroundWorker();
        public delegate void OnProcesar(string pstrEstadoProceso);
        public delegate void OnError(OperationResult pobjOperationResult);
        public delegate void OnFinalizado();
        public event OnProcesar OnProcesarEvent;
        public event OnError OnErrorEvent;
        public event OnFinalizado OnFinalizadoEvent;
        private string _periodo;
        public List<string> FiltroProductos { get; set; }
        public bool IsBusy
        {
            get { return bw.IsBusy; }
        }

        /// <summary>
        /// Indica el periodo que se va a procesar.
        /// </summary>
        public string Periodo
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    _periodo = "2016";
                else
                    _periodo = value;
            }
        }

        /// <summary>
        /// Comienza el proceso de recálculo
        /// </summary>
        public void Comenzar()
        {
            bw.DoWork += bw_DoWork;
            bw.RunWorkerAsync();
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            OperationResult pobjOperationResult = new OperationResult();
            try
            {

                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (OnProcesarEvent != null)
                            OnProcesarEvent("Recopilando Data de Ventas...");

                        //var movimientoSalidas =
                        //    dbContext.movimiento.Where(p => p.v_OrigenTipo != null && p.v_OrigenTipo.Equals("V") &&
                        //                                    p.v_Periodo.Equals(_periodo) && p.i_Eliminado == 0).ToList()
                        //                                    .OrderBy(o => o.v_Mes).ThenBy(t => t.v_Correlativo);

                        var ventasPorProcesar =
                            dbContext.venta.Where(p => p.v_Periodo.Equals(_periodo) && p.i_Eliminado == 0).ToList().OrderBy(o => o.v_Mes)
                                                            .ThenBy(t => t.v_Correlativo);

                        if (FiltroProductos != null && FiltroProductos.Any())
                        {
                            if (OnProcesarEvent != null)
                                OnProcesarEvent("Filtrando datos...");

                            //var movimientosFiltrados =
                            //    dbContext.movimientodetalle.Where(m => FiltroProductos.Contains(m.v_IdProductoDetalle) && m.i_Eliminado == 0)
                            //                                    .Select(n => n.v_IdMovimiento).Distinct().ToList();

                            var ventasFiltrados =
                               dbContext.ventadetalle.Where(m => FiltroProductos.Contains(m.v_IdProductoDetalle) && m.i_Eliminado == 0)
                                                               .Select(n => n.v_IdVenta).Distinct().ToList();

                            //movimientoSalidas = movimientoSalidas.Where(o => movimientosFiltrados.Contains(o.v_IdMovimiento)).OrderBy(o => o.v_Mes).ThenBy(t => t.v_Correlativo);
                            ventasPorProcesar = ventasPorProcesar.Where(o => ventasFiltrados.Contains(o.v_IdVenta)).OrderBy(o => o.v_Mes).ThenBy(t => t.v_Correlativo);
                        }

                        //if (movimientoSalidas.Any())
                        //{
                        //    foreach (var movimientoSalida in movimientoSalidas.AsParallel())
                        //    {
                        //        var nroSalida = string.Format("{0}-{1}", movimientoSalida.v_Mes, movimientoSalida.v_Correlativo);
                        //        if (OnProcesarEvent != null)
                        //            OnProcesarEvent(string.Format("Eliminado Salida: {0}", nroSalida));

                        //        new MovimientoBL().EliminarMovimiento(ref pobjOperationResult,
                        //            movimientoSalida.v_IdMovimiento, Globals.ClientSession.GetAsList());
                        //        if (pobjOperationResult.Success == 0)
                        //            throw new Exception("Error al Eliminar Nota de Salida: " + movimientoSalida.v_IdMovimiento);
                        //    }
                        //}

                        if (ventasPorProcesar.Any())
                        {
                            foreach (var venta in ventasPorProcesar.AsParallel())
                            {
                                var nroVenta = string.Format("{0}-{1}", venta.v_Mes.Trim(), venta.v_Correlativo);
                                if (OnProcesarEvent != null)
                                    OnProcesarEvent(string.Format("Regenerando Salida de: {0}", nroVenta));

                                VentaBL.RegenerarSalidasAlmacen(ref pobjOperationResult, venta.v_IdVenta);
                                if (pobjOperationResult.Success == 0)
                                    throw new Exception("Error al Generar Salida de: " + venta.v_IdVenta);
                            }
                        }
                    }

                    ts.Complete();
                    if (OnFinalizadoEvent != null)
                        OnFinalizadoEvent();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ReprocesarSalidasWorker.Comenzar()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                if (OnErrorEvent != null)
                    OnErrorEvent(pobjOperationResult);
                return;
            }
        }

        /// <summary>
        /// Manda una solicitud de cancelacion del proceso.
        /// </summary>
        public void Detener()
        {
            bw.CancelAsync();
        }
    }

    public class RegenerarAsientosVentaWorker
    {
        public delegate void Proceso(int porcentaje);
        public delegate void Error(OperationResult pobjOperationResult);

        public event Proceso ProcesoEvent;
        public event Error ErrorEvent;
        public event EventHandler FinalizadoEvent;

        public void ComenzarAsync(int pintMes, int pintPeriodo)
        {
            var pobjOperationResult = new OperationResult();
            try
            {
                Task.Factory.StartNew(() =>
                {
                    lock (this)
                    {
                        using (var ts = TransactionUtils.CreateTransactionScope())
                        {
                            using (var dbContext = new SAMBHSEntitiesModelWin())
                            {
                                var pobjDtoEntities = dbContext.venta.
                                    Where(p => p.t_FechaRegistro.Value.Month == pintMes && p.t_FechaRegistro.Value.Year == pintPeriodo).ToDTOs();

                                var total = pobjDtoEntities.Count;
                                var pos = 0;

                                foreach (var pobjDtoEntity in pobjDtoEntities)
                                {
                                    pos++;
                                    if (pobjDtoEntity.i_Eliminado == 0)
                                        new VentaBL().RegenerarAsientoContable(ref pobjOperationResult, pobjDtoEntity.v_IdVenta);
                                    else
                                        new VentaBL().EliminarAsientoContable(ref pobjOperationResult, pobjDtoEntity.v_IdVenta);

                                    if (pobjOperationResult.Success == 0) return;

                                    if (ProcesoEvent != null)
                                        ProcesoEvent((pos * 100) / total);
                                }

                                ts.Complete();
                                pobjOperationResult.Success = 1;

                                if (ProcesoEvent != null)
                                    ProcesoEvent(100);

                                if (FinalizadoEvent != null)
                                    FinalizadoEvent(this, new EventArgs());
                            }
                        }
                    }

                }, TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "RegenerarAsientosVentaWorker.ComenzarAsync()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                if (ErrorEvent != null)
                    ErrorEvent(pobjOperationResult);
            }
        }

        protected virtual void OnProcesoEvent(int mensaje)
        {
            var handler = ProcesoEvent;
            if (handler != null) handler(mensaje);
        }

        
    }
}







