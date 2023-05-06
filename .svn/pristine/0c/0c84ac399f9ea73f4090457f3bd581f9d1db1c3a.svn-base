using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BL;
using System.ComponentModel;
using System.Data.Objects;
using System.Transactions;
using SAMBHS.Almacen.BL;

namespace SAMBHS.Venta.BL
{
    public class PedidoBL
    {
        #region Formulario

        private DocumentoBL _objDocumentoBL = new DocumentoBL();

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

        public string[] PublicoGeneral()
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var PublicoGeneral = (from pg in dbContext.cliente
                                      join cd in dbContext.clientedirecciones on new { dc = pg.v_IdCliente, eliminado = 0, predeterminado = 1 } equals new { dc = cd.v_IdCliente, eliminado = cd.i_Eliminado.Value, predeterminado = cd.i_EsDireccionPredeterminada.Value } into cd_join
                                      from cd in cd_join.DefaultIfEmpty()
                                      where pg.v_IdCliente == "N002-CL000000000"
                                      select
                                          new
                                          {
                                              v_IdCliente = pg.v_IdCliente,
                                              v_CodCliente = pg.v_CodCliente,
                                              v_RazonSocial = pg.v_RazonSocial,
                                              v_NroDocIdentificacion = pg.v_NroDocIdentificacion,
                                              v_DirecPrincipal = pg.v_DirecPrincipal,
                                              i_IdListaPrecios = pg.i_IdListaPrecios,
                                              i_IdDireccionCliente = cd != null ? cd.i_IdDireccionCliente : -1,
                                          }).FirstOrDefault();

                if (PublicoGeneral != null)
                {
                    string[] Cadena = new string[7];
                    Cadena[0] = PublicoGeneral.v_IdCliente;
                    Cadena[1] = PublicoGeneral.v_CodCliente;
                    Cadena[2] = PublicoGeneral.v_RazonSocial;
                    Cadena[3] = PublicoGeneral.v_NroDocIdentificacion;
                    Cadena[4] = PublicoGeneral.v_DirecPrincipal;
                    Cadena[5] = PublicoGeneral.i_IdListaPrecios.Value.ToString();
                    Cadena[6] = PublicoGeneral.i_IdDireccionCliente.ToString();
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








        public string[] ObtenerClientePorID(ref OperationResult pobjOperationResult, string ID, int IdDireccion)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var Cliente = (from pg in dbContext.cliente
                               join cd in dbContext.clientedirecciones on new { dc = pg.v_IdCliente, eliminado = 0, dir = IdDireccion } equals new { dc = cd.v_IdCliente, eliminado = cd.i_Eliminado.Value, dir = cd.i_IdDireccionCliente } into cd_join
                               from cd in cd_join.DefaultIfEmpty()
                               join z in dbContext.datahierarchy on new { grupo = 161, eliminado = 0, item = cd.i_IdZona.Value } equals new { grupo = z.i_GroupId, eliminado = z.i_IsDeleted.Value, item = z.i_ItemId } into z_join
                               from z in z_join.DefaultIfEmpty()
                               where pg.v_IdCliente == ID && pg.i_Eliminado == 0
                               select
                                   new
                                   {
                                       v_IdCliente = pg.v_IdCliente,
                                       v_CodCliente = pg.v_CodCliente.Trim(),
                                       v_RazonSocial = (pg.v_ApePaterno + " " + pg.v_ApeMaterno + " " + pg.v_PrimerNombre + " " + pg.v_SegundoNombre + " " + pg.v_RazonSocial).Trim(),
                                       v_NroDocIdentificacion = pg.v_NroDocIdentificacion.Trim(),
                                       //v_DirecPrincipal = pg.v_DirecPrincipal,
                                       v_DirecPrincipal = cd.v_Direccion != null ? cd.v_Direccion.Trim() : "",
                                       i_IdListaPrecios = pg.i_IdListaPrecios,
                                       i_IdDireccionCliente = cd != null ? cd.i_IdDireccionCliente : -1,
                                       zona = z.v_Value1
                                   }).FirstOrDefault();

                if (Cliente != null)
                {
                    string[] Cadena = new string[8];
                    Cadena[0] = Cliente.v_IdCliente;
                    Cadena[1] = Cliente.v_CodCliente;
                    Cadena[2] = Cliente.v_RazonSocial;
                    Cadena[3] = Cliente.v_NroDocIdentificacion;
                    Cadena[4] = Cliente.v_DirecPrincipal;
                    Cadena[5] = Cliente.i_IdListaPrecios.Value.ToString();
                    Cadena[6] = Cliente.i_IdDireccionCliente.ToString();
                    Cadena[7] = Cliente.zona.ToString();
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


        public List<KeyValueDTO> ObtenerListadoPedidos(ref OperationResult pobjOperationResult, string pstringPeriodo,
            string pstringMes)
        {
            try
            {
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                var almacenpredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                string replicationID = Globals.ClientSession.ReplicationNodeID;
                var query = (from n in dbcontext.pedido
                             where
                                 n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes &&
                                 n.v_IdPedido.Substring(2, 2) == almacenpredeterminado && n.v_IdPedido.Substring(0, 1) == replicationID

                             orderby n.v_Correlativo ascending
                             select new
                             {
                                 v_Correlativo = n.v_Correlativo,
                                 v_IdPedido = n.v_IdPedido


                             }
                    );
                if (query.Any())
                {
                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Value1 = x.v_Correlativo,
                            Value2 = x.v_IdPedido
                        }).ToList();

                    return query2;
                }
                else
                {
                    return new List<KeyValueDTO> { new KeyValueDTO { Value1 = almacenpredeterminado + "000000" } };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public pedidoDto ObtenerPedidoCabecera(ref OperationResult objOperationResult, string strIdPedido)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var objEntity = (from a in dbContext.pedido

                                 join D in dbContext.datahierarchy on new { a = a.i_IdEstado.Value, b = 43 } //Estado
                                     equals new { a = D.i_ItemId, b = D.i_GroupId } into D_join


                                 from D in D_join.DefaultIfEmpty()

                                 join F in dbContext.cliente on a.v_IdCliente equals F.v_IdCliente into F_join
                                 from F in F_join.DefaultIfEmpty()

                                 where a.v_IdPedido == strIdPedido

                                 select new pedidoDto
                                 {
                                     v_IdPedido = a.v_IdPedido,
                                     v_Mes = a.v_Mes,
                                     v_Periodo = a.v_Periodo,
                                     v_Correlativo = a.v_Correlativo,
                                     i_IdTipoDocumento = a.i_IdTipoDocumento,
                                     v_SerieDocumento = a.v_SerieDocumento,
                                     v_CorrelativoDocumento = a.v_CorrelativoDocumento,
                                     t_FechaEmision = a.t_FechaEmision,
                                     d_TipoCambio = a.d_TipoCambio,
                                     i_DiasVigencia = a.i_DiasVigencia,
                                     t_FechaVencimiento = a.t_FechaVencimiento,
                                     v_IdCliente = a.v_IdCliente,
                                     i_IdCondicionPago = a.i_IdCondicionPago,
                                     v_Glosa = a.v_Glosa,
                                     i_IdMoneda = a.i_IdMoneda,
                                     i_IdEstado = a.i_IdEstado,
                                     i_AfectoIgv = a.i_AfectoIgv,
                                     i_PrecionInclIgv = a.i_PrecionInclIgv,
                                     v_IdVendedor = a.v_IdVendedor,
                                     v_IdVendedorRef = a.v_IdVendedorRef,
                                     d_Dscto = a.d_Dscto,
                                     d_Valor = a.d_Valor,
                                     d_VVenta = a.d_VVenta,
                                     d_Descuento = a.d_Descuento,
                                     i_IdIgv = a.i_IdIgv,
                                     d_CantidadTotal = a.d_CantidadTotal,
                                     d_PrecioVenta = a.d_PrecioVenta,
                                     i_Eliminado = a.i_Eliminado,
                                     i_InsertaUsuario = a.i_InsertaUsuario,
                                     t_InsertaFecha = a.t_InsertaFecha,
                                     i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                     t_ActualizaFecha = a.t_ActualizaFecha,
                                     RucCliente = F.v_NroDocIdentificacion,
                                     CodCliente = F.v_CodCliente,
                                     IdLista = F.i_IdListaPrecios,
                                     v_NombreClienteTemporal = a.v_NombreClienteTemporal,
                                     RazonSocial =
                                         (F.v_ApePaterno + " " + F.v_ApeMaterno + " " + " " + F.v_PrimerNombre + " " +
                                          F.v_SegundoNombre + " " + F.v_RazonSocial).Trim(),
                                     v_DireccionClienteTemporal = a.v_DireccionClienteTemporal,
                                     //Direccion =  F.v_DirecPrincipal,
                                     Direccion = string.IsNullOrEmpty(a.v_DireccionClienteTemporal) ? F.v_DirecPrincipal : a.v_DireccionClienteTemporal,
                                     i_IdEstablecimiento = a.i_IdEstablecimiento.Value,
                                     t_FechaDespacho = a.t_FechaDespacho == null ? DateTime.Now : a.t_FechaDespacho,
                                     v_IdAgenciaTransporte = a.v_IdAgenciaTransporte ?? "-1",
                                     i_IdTipoOperacion = a.i_IdTipoOperacion,
                                     TipoDocumentoCliente = F.i_IdTipoIdentificacion ?? 0,
                                     i_IdDireccionCliente = a.i_IdDireccionCliente ?? -1,
                                     i_IdTipoVerificacion = a.i_IdTipoVerificacion ?? -1,


                                 }
                    ).FirstOrDefault();
                objOperationResult.Success = 1;

                return objEntity;
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }


        }

        public BindingList<BindingPedidoDetalle> ObtenerPedidoDetalles(ref OperationResult pobjOperationResult,
            string pstrIdPedido)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                string periodo = Globals.ClientSession.i_Periodo.ToString();

                var query = (from n in dbContext.pedidodetalle

                             join A in dbContext.productodetalle on new { n.v_IdProductoDetalle, eliminado = 0 } equals new { A.v_IdProductoDetalle, eliminado = A.i_Eliminado.Value } into A_join
                             from A in A_join.DefaultIfEmpty()

                             join B in dbContext.producto on new { A.v_IdProducto, eliminado = 0 } equals new { B.v_IdProducto, eliminado = B.i_Eliminado.Value } into B_join

                             from B in B_join.DefaultIfEmpty()

                             join C in dbContext.productoalmacen on new { strIdProducto = A.v_IdProductoDetalle, a = n.i_IdAlmacen.Value, eliminado = 0, _periodo = periodo, pedido = n.v_NroPedido, serie = n.v_NroSerie, lote = n.v_NroLote }
                                                                equals new { strIdProducto = C.v_ProductoDetalleId, a = C.i_IdAlmacen, eliminado = C.i_Eliminado.Value, _periodo = C.v_Periodo, pedido = string.IsNullOrEmpty(C.v_NroPedido) ? null : C.v_NroPedido, serie =string.IsNullOrEmpty ( C.v_NroSerie)? null : C.v_NroSerie, lote = string.IsNullOrEmpty ( C.v_NroLote)?null :C.v_NroLote } into C_join
                             from C in C_join.DefaultIfEmpty()

                             join D in dbContext.almacen on new { Almacen = n.i_IdAlmacen.Value, eliminado = 0 } equals new { Almacen = D.i_IdAlmacen, eliminado = D.i_Eliminado.Value } into D_join
                             from D in D_join.DefaultIfEmpty()

                             join J1 in dbContext.datahierarchy on new { a = B.i_IdUnidadMedida.Value, b = 17, eliminado = 0 } equals new { a = J1.i_ItemId, b = J1.i_GroupId, eliminado = J1.i_IsDeleted.Value } into J1_join
                             from J1 in J1_join.DefaultIfEmpty()

                             where n.i_Eliminado == 0 && n.v_IdPedido == pstrIdPedido

                             orderby n.t_InsertaFecha ascending
                             //orderby n.v_IdPedidoDetalle ascending

                             select new BindingPedidoDetalle
                             {
                                 v_NombreProducto = n.v_NombreProducto,
                                 v_CodigoInterno = B.v_CodInterno,
                                 Empaque = B.d_Empaque ?? 0,
                                 UMEmpaque = J1 == null ? "" : J1.v_Value1,
                                 i_IdUnidadMedida = n.i_IdUnidadMedida,
                                 v_IdProductoAlmacen = C.v_IdProductoAlmacen,
                                 d_StockActual = C.d_StockActual,
                                 d_SeparacionTotal = C.d_SeparacionTotal,
                                 i_EsServicio = B.i_EsServicio ?? 0,
                                 i_EsAfectoDetraccion = B.i_EsAfectoDetraccion,
                                 EsNombreEditable = B.i_NombreEditable,
                                 i_ValidarStock = B.i_ValidarStock,
                                 i_IdUnidadMedidaProducto = B == null ? -1 : B.i_IdUnidadMedida ?? -1,
                                 i_IdAlmacen = n.i_IdAlmacen,
                                 d_Cantidad = n.d_Cantidad,
                                 d_PrecioUnitario = n.d_PrecioUnitario,
                                 i_NroUnidades = n.i_NroUnidades,
                                 v_Observacion = n.v_Observacion,
                                 v_IdPedido = n.v_IdPedido,
                                 d_Valor = n.d_Valor,
                                 d_Descuento = n.d_Descuento,
                                 d_ValorVenta = n.d_ValorVenta,
                                 d_Igv = n.d_Igv,
                                 d_PrecioVenta = n.d_PrecioVenta,
                                 d_CantidadEmpaque = n.d_CantidadEmpaque,
                                 v_IdProductoDetalle = n.v_IdProductoDetalle,
                                 i_Eliminado = n.i_Eliminado,
                                 i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                 v_IdPedidoDetalle = n.v_IdPedidoDetalle,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 EsRedondeo = n.v_IdProductoDetalle == "N002-PE000000000" ? 1 : (int?)null,
                                 i_PrecioEditable = B.i_PrecioEditable,
                                 v_Descuento = n.v_Descuento,
                                 i_IdTipoOperacion = n.i_IdTipoOperacion,
                                 i_LiberacionUsuario = n.i_LiberacionUsuario,
                                 t_FechaLiberacion = n.t_FechaLiberacion,
                                 t_FechaCaducidad = n.t_FechaCaducidad.Value,
                                 v_NroLote = n.v_NroLote,
                                 v_NroSerie = n.v_NroSerie,
                                 v_NroPedido = n.v_NroPedido,

                                 i_SolicitarNroLoteIngreso = B.i_SolicitarNroLoteIngreso ?? 0,
                                 i_SolicitarNroSerieIngreso = B.i_SolicitarNroSerieIngreso ?? 0,
                                 i_SolicitaOrdenProduccionIngreso = B.i_SolicitaOrdenProduccionIngreso ?? 0,
                                 i_SolicitarNroSerieSalida = B.i_SolicitarNroSerieSalida ?? 0,
                                 i_SolicitarNroLoteSalida = B.i_SolicitarNroLoteSalida ?? 0,
                                 i_SolicitaOrdenProduccionSalida = B.i_SolicitaOrdenProduccionSalida ?? 0,

                             }
                    ).ToList();

                pobjOperationResult.Success = 1;

                return new BindingList<BindingPedidoDetalle>(query);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }







        public BindingList<BindingPedidoDetalle> ObtenerPedidoDetallesParaExtraccion(ref OperationResult pobjOperationResult,
           string pstrIdPedido)
        {
            try
            {
                var dbContext = new SAMBHSEntitiesModelWin();
                var periodo = Globals.ClientSession.i_Periodo.ToString();
                var pedido = dbContext.pedido.FirstOrDefault(p => p.v_IdPedido.Equals(pstrIdPedido));
                if (pedido == null) return null;
                var nroSinEspacioPedido = string.Join("-", pedido.v_SerieDocumento, pedido.v_CorrelativoDocumento);
                var nroConEspacioPedido = string.Join(" - ", pedido.v_SerieDocumento, pedido.v_CorrelativoDocumento);
                var ventasConPedido =
                    dbContext.venta.Where(
                        p => (p.v_NroPedido.Equals(nroSinEspacioPedido) || p.v_NroPedido.Equals(nroConEspacioPedido))
                            && p.i_Eliminado == 0 && p.i_IdEstado == 1).ToList();
                var ventasDetalles = new List<ventadetalleDto>();
                ventasConPedido.ForEach(v => ventasDetalles
                    .AddRange(v.ventadetalle.Where(p => p.i_Eliminado == 0).ToDTOs()));

                #region Consulta el Pedido
                var query = (from n in dbContext.pedidodetalle

                             join A in dbContext.productodetalle on new { n.v_IdProductoDetalle, eliminado = 0 } equals new { A.v_IdProductoDetalle, eliminado = A.i_Eliminado.Value } into
                                 A_join
                             from A in A_join.DefaultIfEmpty()

                             join B in dbContext.producto on new { A.v_IdProducto, eliminado = 0 } equals new { B.v_IdProducto, eliminado = B.i_Eliminado.Value } into B_join

                             from B in B_join.DefaultIfEmpty()

                             join C in dbContext.productoalmacen on new { strIdProducto = A.v_IdProductoDetalle, a = n.i_IdAlmacen.Value, eliminado = 0, _periodo = periodo, pedido = n.v_NroPedido, serie = n.v_NroSerie, lote = n.v_NroLote }
                                                                equals new { strIdProducto = C.v_ProductoDetalleId, a = C.i_IdAlmacen, eliminado = C.i_Eliminado.Value, _periodo = C.v_Periodo, pedido = string.IsNullOrEmpty(C.v_NroPedido) ? null : C.v_NroPedido, serie = string.IsNullOrEmpty ( C.v_NroSerie)?null : C.v_NroSerie, lote = string.IsNullOrEmpty ( C.v_NroLote)?null :C.v_NroLote} into C_join
                             from C in C_join.DefaultIfEmpty()

                             join D in dbContext.almacen on new { Almacen = n.i_IdAlmacen.Value, eliminado = 0 } equals new { Almacen = D.i_IdAlmacen, eliminado = D.i_Eliminado.Value } into D_join
                             from D in D_join.DefaultIfEmpty()

                             join J1 in dbContext.datahierarchy on new { a = B.i_IdUnidadMedida.Value, b = 17, eliminado = 0 } equals new { a = J1.i_ItemId, b = J1.i_GroupId, eliminado = J1.i_IsDeleted.Value } into J1_join
                             from J1 in J1_join.DefaultIfEmpty()

                             join J2 in dbContext.linea on new { id = B.v_IdLinea, eliminado = 0 } equals new { id = J2.v_IdLinea, eliminado = J2.i_Eliminado.Value } into J2_join
                             from J2 in J2_join.DefaultIfEmpty()

                             where n.i_Eliminado == 0 && n.v_IdPedido == pstrIdPedido && n.v_IdProductoDetalle != "N002-PE000000000"

                             orderby n.t_InsertaFecha ascending

                             select new BindingPedidoDetalle
                             {
                                 v_NombreProducto = n.v_NombreProducto,
                                 v_CodigoInterno = B.v_CodInterno,
                                 Empaque = B.d_Empaque,
                                 UMEmpaque = J1.v_Value1,
                                 i_IdUnidadMedida = n.i_IdUnidadMedida,
                                 v_IdProductoAlmacen = C.v_IdProductoAlmacen,
                                 d_StockActual = C.d_StockActual,
                                 d_SeparacionTotal = C.d_SeparacionTotal,
                                 i_EsServicio = B.i_EsServicio,
                                 i_EsAfectoDetraccion = B.i_EsAfectoDetraccion,
                                 EsNombreEditable = B.i_NombreEditable,
                                 i_ValidarStock = B.i_ValidarStock,
                                 i_IdUnidadMedidaProducto = B.i_IdUnidadMedida,
                                 i_IdAlmacen = n.i_IdAlmacen,
                                 d_Cantidad = n.d_Cantidad,
                                 d_PrecioUnitario = n.d_PrecioUnitario,
                                 i_NroUnidades = n.i_NroUnidades,
                                 v_Observacion = n.v_Observacion,
                                 v_IdPedido = n.v_IdPedido,
                                 d_Valor = n.d_Valor,
                                 d_Descuento = n.d_Descuento,
                                 d_ValorVenta = n.d_ValorVenta,
                                 d_Igv = n.d_Igv,
                                 d_PrecioVenta = n.d_PrecioVenta,
                                 d_CantidadEmpaque = n.d_CantidadEmpaque,
                                 v_IdProductoDetalle = n.v_IdProductoDetalle,
                                 i_Eliminado = n.i_Eliminado,
                                 i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                 v_IdPedidoDetalle = n.v_IdPedidoDetalle,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 EsRedondeo = n.v_IdProductoDetalle == "N002-PE000000000" ? 1 : (int?)null,
                                 i_PrecioEditable = B.i_PrecioEditable,
                                 v_Descuento = n.v_Descuento,
                                 NroCuenta = J2.v_NroCuentaVenta,
                                 NroPedido = pedido.v_SerieDocumento + "-" + pedido.v_CorrelativoDocumento,
                                 CantidadOriginal = n.d_Cantidad ?? 0,
                                 

                                 i_SolicitarNroLoteIngreso = B.i_SolicitarNroLoteIngreso ?? 0,
                                 i_SolicitarNroSerieIngreso = B.i_SolicitarNroSerieIngreso ?? 0,
                                 i_SolicitaOrdenProduccionIngreso = B.i_SolicitaOrdenProduccionIngreso ?? 0,
                                 i_SolicitarNroSerieSalida = B.i_SolicitarNroSerieSalida ?? 0,
                                 i_SolicitarNroLoteSalida = B.i_SolicitarNroLoteSalida ?? 0,
                                 i_SolicitaOrdenProduccionSalida = B.i_SolicitaOrdenProduccionSalida ?? 0,

                                 v_NroPedido = n.v_NroPedido,
                                 v_NroLote =n.v_NroLote ,
                                 v_NroSerie =n.v_NroSerie ,
                                 t_FechaCaducidad =n.t_FechaCaducidad.Value ,
                             }
                            ).ToList();
                #endregion

                query.ForEach(p =>
                {
                    var usadasEnVenta = ventasDetalles
                        .Where(o => o.v_IdProductoDetalle.Equals(p.v_IdProductoDetalle)).ToList();
                    var montoUsado = usadasEnVenta.Sum(o => o.d_Cantidad ?? 0);
                    p.d_Cantidad -= montoUsado;
                    p.CantidadFacturada = montoUsado;
                });

                pobjOperationResult.Success = 1;

                return new BindingList<BindingPedidoDetalle>(query.Where(p => p.d_Cantidad > 0).ToList());
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return new BindingList<BindingPedidoDetalle>();
            }
        }

        public bool PedidoDespachadoCompletamente(ref OperationResult pobjOperationResult,
           string pstrIdPedido, IEnumerable<pedido> tempPedidos, IEnumerable<pedidodetalle> tempPedidoDetalle, IEnumerable<venta> tempVentas)
        {
            try
            {
                var pedido = tempPedidos.FirstOrDefault(p => p.v_IdPedido.Equals(pstrIdPedido));
                if (pedido == null) return false;
                var nroPedido = string.Join("-", pedido.v_SerieDocumento, pedido.v_CorrelativoDocumento);
                var ventasConPedido = tempVentas.Where(p => p.v_NroPedido.Replace (" ","").Equals(nroPedido) && p.i_Eliminado == 0 && p.i_IdEstado == 1).ToList();
                var ventasDetalles = new List<ventadetalleDto>();
                ventasConPedido.ForEach(v => ventasDetalles.AddRange(v.ventadetalle.Where(p => p.i_Eliminado == 0).ToDTOs()));

                #region Consulta el Pedido
                var query = tempPedidoDetalle.Where(p => p.v_IdPedido.Equals(pstrIdPedido) && p.i_Eliminado == 0).ToList().ToDTOs();
                #endregion

                query.ForEach(p =>
                {
                    var usadasEnVenta = ventasDetalles.Where(o => o.v_IdProductoDetalle.Equals(p.v_IdProductoDetalle)).ToList();
                    var montoUsado = usadasEnVenta.Sum(o => o.d_Cantidad ?? 0);
                    p.d_Cantidad -= montoUsado;
                    p.CantidadFacturada = montoUsado;
                });

                pobjOperationResult.Success = 1;

                return !query.Where(p => p.d_Cantidad > 0).ToList().Any();
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return false;
            }
        }

        public string[] DevolverProductos(string pstringIdProductoDetalle) // Productos x almacen
        {
            string success;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();


                var EntityProducto = (from n in dbContext.productodetalle

                                      join A in dbContext.producto on n.v_IdProducto equals A.v_IdProducto into A_join
                                      from A in A_join.DefaultIfEmpty()

                                      join J1 in dbContext.datahierarchy on new { a = A.i_IdUnidadMedida.Value, b = 17 }
                                          equals new { a = J1.i_ItemId, b = J1.i_GroupId } into J1_join
                                      from J1 in J1_join.DefaultIfEmpty()

                                      where n.v_IdProductoDetalle == pstringIdProductoDetalle

                                      select new
                                      {
                                          Nombre = A.v_Descripcion,
                                          CodigoInterno = A.v_CodInterno,
                                          Empaque = A.d_Empaque,

                                          UMEmpaque = J1.v_Value1,

                                          i_IdUnidadMedida = A.i_IdUnidadMedida,
                                          d_StockActual = 0,
                                          d_SeparacionTotal = 0,
                                          i_EsServicio = A.i_EsServicio,
                                          i_EsAfectoDetraccion = A.i_EsAfectoDetraccion,
                                          i_NombreEditable = A.i_NombreEditable

                                      }
                    ).FirstOrDefault();


                string[] Cadena = new string[11];
                if (EntityProducto != null)
                {
                    Cadena[0] = EntityProducto.CodigoInterno;
                    Cadena[1] = EntityProducto.Nombre;
                    Cadena[2] = EntityProducto.Empaque.ToString();
                    Cadena[3] = EntityProducto.UMEmpaque.ToString();
                    Cadena[4] = EntityProducto.i_IdUnidadMedida.ToString();
                    //Cadena[5] = EntityProducto.v_IdProductoAlmacen;
                    Cadena[6] = EntityProducto.d_SeparacionTotal.ToString();
                    Cadena[7] = EntityProducto.d_StockActual.ToString();
                    Cadena[8] = EntityProducto.i_EsServicio.ToString();
                    Cadena[9] = EntityProducto.i_EsAfectoDetraccion.ToString();
                    Cadena[10] = EntityProducto.i_NombreEditable.ToString();
                }
                else
                {
                    success = "0";

                }

                return Cadena;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string[] DevolverClientePorNroDocumento(ref OperationResult pobjOperationResult, string NroDocumento)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();



                var query = (from n in dbContext.cliente
                             where n.i_Eliminado == 0 && n.v_FlagPantalla == "C" && n.v_NroDocIdentificacion == NroDocumento
                             select n
                    ).FirstOrDefault();



                pobjOperationResult.Success = 1;

                if (query != null)
                {
                    var DireccionCliente = dbContext.clientedirecciones.Where(o => o.i_EsDireccionPredeterminada == 1 && o.i_Eliminado == 0 && o.v_IdCliente == query.v_IdCliente).FirstOrDefault();
                    string[] Cadena = new string[6];
                    Cadena[0] = query.v_IdCliente;
                    Cadena[1] = query.v_CodCliente;
                    Cadena[2] =
                        (query.v_PrimerNombre + " " + query.v_SegundoNombre + " " + query.v_ApePaterno + " " +
                         query.v_ApeMaterno + " " + query.v_RazonSocial).Trim();
                    Cadena[3] = query.i_IdListaPrecios.Value.ToString();
                    //Cadena[4] = query.v_DirecPrincipal;
                    Cadena[4] = DireccionCliente != null ? DireccionCliente.v_Direccion : query.v_DirecPrincipal;
                    Cadena[5] = DireccionCliente != null ? DireccionCliente.i_IdDireccionCliente.ToString() : "-1";
                    return Cadena;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public bool ObtenerExistenciaPedidoCotizacion(OperationResult pobjOperationResult, int iTipoDocumento,
            string strSerieDocumento, string strNumeroDocumento, string strIdPedido)
        {

            try
            {
                if (strSerieDocumento != null && strNumeroDocumento != null && iTipoDocumento != -1)
                // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    if (strIdPedido == null)
                    // SI el idGuia es nula, se esta consultado de una guia  nueva que no ha sido guardada
                    {
                        var query = (from n in dbContext.pedido
                                     where n.i_Eliminado == 0
                                           && n.v_SerieDocumento == strSerieDocumento &&
                                           n.v_CorrelativoDocumento == strNumeroDocumento &&
                                           n.i_IdTipoDocumento == iTipoDocumento
                                     select n
                            ).FirstOrDefault();
                        if (query == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else // si no es nulo se comprueba de una guiaremision que esta siendo modificada
                    {
                        var query = (from n in dbContext.pedido
                                     where n.i_Eliminado == 0
                                           && n.v_SerieDocumento == strSerieDocumento &&
                                           n.v_CorrelativoDocumento == strNumeroDocumento &&
                                           n.i_IdTipoDocumento == iTipoDocumento
                                           && n.v_IdPedido != strIdPedido
                                     select n
                            ).FirstOrDefault();
                        if (query == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
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

        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            string ReplicationID = Globals.ClientSession.ReplicationNodeID;
            var Registro = (from n in dbContext.pedido
                            where n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo && n.v_IdPedido.Substring(0, 1) == ReplicationID

                            select n).FirstOrDefault();

            if (Registro == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ExisteNroDocumento(int TipoDocumento, string pstrSerie, string pstrCorrelativo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var Registro = (from n in dbContext.pedido
                            where
                                n.i_Eliminado == 0 && n.v_SerieDocumento == pstrSerie && n.v_CorrelativoDocumento == pstrCorrelativo &&
                                n.i_IdTipoDocumento == TipoDocumento
                            select n).FirstOrDefault();

            if (Registro == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ComprobarExistenciaCorrelativoDocumento(ref OperationResult pobjOperationResult,
            int pintIdTipoDocumento, string pstrSerieDoc, string pstrCorrelativoDoc, string pstrIdPedido)
        {
            try
            {
                if (pintIdTipoDocumento != -1 && pstrSerieDoc != null && pstrCorrelativoDoc != null)
                // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    if (pstrIdPedido == null)
                    // SI el idVenta es nulo se esta consultado de una venta nueva que no ha sido guardada
                    {
                        var query = (from n in dbContext.pedido
                                     where n.i_Eliminado == 0
                                           && n.i_IdTipoDocumento == pintIdTipoDocumento
                                           && n.v_SerieDocumento == pstrSerieDoc &&
                                           n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                     select n
                            ).FirstOrDefault();
                        if (query == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else // si no es nulo se comprueba de una venta que esta siendo modificada
                    {
                        var query = (from n in dbContext.pedido
                                     where n.i_Eliminado == 0
                                           && n.i_IdTipoDocumento == pintIdTipoDocumento
                                           && n.v_SerieDocumento == pstrSerieDoc &&
                                           n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                           && n.v_IdPedido != pstrIdPedido
                                     select n
                            ).FirstOrDefault();
                        if (query == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
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

        public void DuplicarPedido(ref OperationResult objOperationResult, string pstrIdPedido)
        {
            //using (var dbContext = new SAMBHSEntitiesModelWin())
            //{
            //    var pedido = dbContext.pedido.FirstOrDefault(p => p.v_IdPedido == pstrIdPedido);

            //    var pedidoCabeceraDto = pedido.ToDTO();
            //    var pedidoDetallesDto = pedido.pedidodetalle.Select(detalle => detalle.ToDTO()).ToList();
            //    var separacionDto = dbContext.separacionproducto.Where(x => x.v_IdPedido == pstrIdPedido).ToList().ToDTOs ();
            //    InsertarPedido(ref objOperationResult, pedidoCabeceraDto, Globals.ClientSession.GetAsList(), pedidoDetallesDto, separacionDto);
            //  _objDocumentoBL.ActualizarCorrelativoPorSerie(ref objOperationResult, Globals.ClientSession.i_IdEstablecimiento, int.Parse(pedidoCabeceraDto.i_IdTipoDocumento.Value.ToString()), txtSerieDoc.Text.Trim(), int.Parse(txtNumeroDoc.Text) + 1);

            //}
        }

        public string InsertarPedido(ref OperationResult pobjOperationResult, pedidoDto pobjDtoEntity,
             List<string> ClientSession, List<pedidodetalleDto> pTemp_Insertar,
             List<separacionproductoDto> pTemp_InsertarSeparacion)
        {
            try
            {

                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        pedido objEntityPedido = pedidoAssembler.ToEntity(pobjDtoEntity);
                        pedidodetalleDto pobjDtoPedidoDetalle = new pedidodetalleDto();

                        int SecuentialId = 0;
                        string newIdPedido = string.Empty;
                        string newIdPedidoDetalle = string.Empty;
                        string newIdSeparacionProducto = string.Empty;
                        int intNodeId;
                        OperationResult objOperationResult = new OperationResult();

                        #region Inserta Cabecera

                        objEntityPedido.t_InsertaFecha = DateTime.Now;

                        objEntityPedido.i_InsertaUsuario = Int32.Parse(ClientSession[2]);
                        objEntityPedido.i_Eliminado = 0;

                        // Autogeneramos el Pk de la tabla
                        intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 42);
                        newIdPedido = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZL");

                        objEntityPedido.v_IdPedido = newIdPedido;
                        dbContext.AddTopedido(objEntityPedido);

                        #endregion

                        #region Inserta Detalle

                        foreach (pedidodetalleDto pedidodetalleDto in pTemp_Insertar)
                        {
                            pedidodetalle objEntityPedidoDetalle = pedidodetalleAssembler.ToEntity(pedidodetalleDto);

                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 43);
                            newIdPedidoDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZN");

                            objEntityPedidoDetalle.v_IdPedidoDetalle = newIdPedidoDetalle;
                            objEntityPedidoDetalle.v_IdPedido = newIdPedido;
                            objEntityPedidoDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityPedidoDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityPedidoDetalle.i_Eliminado = 0;
                            dbContext.AddTopedidodetalle(objEntityPedidoDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName,
                                "pedidodetalle", newIdPedidoDetalle);
                        }

                        #endregion

                        var periodo = Globals.ClientSession.i_Periodo.ToString();
                        dbContext.SaveChanges();
                        if (pobjDtoEntity.i_IdEstado != 3)
                        {
                            #region ActualizaProductoAlmacen

                            //Recorrer el detalle del movimiento
                            if (pTemp_InsertarSeparacion != null)
                            {
                                foreach (var objEntityDetail in pTemp_InsertarSeparacion)
                                {
                                    if (!EsServicio(ref objOperationResult, objEntityDetail.v_IdProductoAlmacen))
                                    // Cuando no es Servicioo
                                    {
                                        // Actualizar la separacion del producto en el almacén correspondiente
                                        productoalmacen itemSeparación = new productoalmacen();
                                        if (objEntityDetail.v_NroPedido != null && objEntityDetail.v_NroPedido.Trim() != string.Empty && Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta == 1) //Se agrego por Wortec
                                        {

                                            if (string.IsNullOrEmpty(objEntityDetail.v_NroSerie) && string.IsNullOrEmpty(objEntityDetail.v_NroLote))
                                            {
                                                itemSeparación = (from A in dbContext.productoalmacen
                                                                  where A.v_IdProductoAlmacen == objEntityDetail.v_IdProductoAlmacen && A.i_Eliminado == 0
                                                                  && A.v_Periodo == periodo && A.v_NroPedido.Trim() == objEntityDetail.v_NroPedido.Trim()
                                                                  select A).FirstOrDefault();
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(objEntityDetail.v_NroSerie))
                                                {
                                                    itemSeparación = (from A in dbContext.productoalmacen
                                                                      where A.v_IdProductoAlmacen == objEntityDetail.v_IdProductoAlmacen && A.i_Eliminado == 0
                                                                      && A.v_Periodo == periodo && A.v_NroPedido.Trim() == objEntityDetail.v_NroPedido.Trim()
                                                                      && A.v_NroLote == null && A.v_NroSerie.Trim() == objEntityDetail.v_NroSerie.Trim()
                                                                      select A).FirstOrDefault();
                                                }
                                                else
                                                {
                                                    itemSeparación = (from A in dbContext.productoalmacen
                                                                      where A.v_IdProductoAlmacen == objEntityDetail.v_IdProductoAlmacen && A.i_Eliminado == 0
                                                                      && A.v_Periodo == periodo && A.v_NroPedido.Trim() == objEntityDetail.v_NroPedido.Trim()
                                                                      && A.v_NroSerie == null && A.v_NroLote.Trim() == objEntityDetail.v_NroLote.Trim()
                                                                      select A).FirstOrDefault();
                                                }
                                            }


                                        }
                                        else
                                        {
                                            if (string.IsNullOrEmpty(objEntityDetail.v_NroSerie) && string.IsNullOrEmpty(objEntityDetail.v_NroLote))
                                            {
                                                itemSeparación = (from A in dbContext.productoalmacen
                                                                  where A.v_IdProductoAlmacen == objEntityDetail.v_IdProductoAlmacen && A.i_Eliminado == 0 && A.v_Periodo == periodo
                                                                  && (A.v_NroPedido == null || A.v_NroPedido=="")
                                                                  select A).FirstOrDefault();
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(objEntityDetail.v_NroLote))
                                                {
                                                    itemSeparación = (from A in dbContext.productoalmacen
                                                                      where A.v_IdProductoAlmacen == objEntityDetail.v_IdProductoAlmacen && A.i_Eliminado == 0 && A.v_Periodo == periodo
                                                                      && (A.v_NroPedido == null  || A.v_NroPedido =="")
                                                                      && A.v_NroSerie == null && A.v_NroLote.Trim() == objEntityDetail.v_NroLote.Trim()
                                                                      select A).FirstOrDefault();
                                                }
                                                else
                                                {
                                                    itemSeparación = (from A in dbContext.productoalmacen
                                                                      where A.v_IdProductoAlmacen == objEntityDetail.v_IdProductoAlmacen && A.i_Eliminado == 0 && A.v_Periodo == periodo
                                                                      && (A.v_NroPedido == null  || A.v_NroPedido =="")
                                                                      && A.v_NroLote == null && A.v_NroSerie.Trim() == objEntityDetail.v_NroSerie.Trim()
                                                                      select A).FirstOrDefault();
                                                }
                                            }

                                        }


                                        if (itemSeparación != null)
                                        {
                                            if (itemSeparación.d_SeparacionTotal == null)
                                            {
                                                itemSeparación.d_SeparacionTotal = 0;
                                            }
                                            itemSeparación.d_SeparacionTotal = itemSeparación.d_SeparacionTotal +
                                                                               objEntityDetail.d_Separacion_CantidadEmpaque;

                                            itemSeparación.i_ActualizaIdUsuario = int.Parse(ClientSession[2]); // Auditoría
                                            itemSeparación.t_ActualizaFecha = DateTime.Now;
                                            dbContext.SaveChanges();
                                            itemSeparación = new productoalmacen();
                                            if (pobjDtoEntity.i_IdTipoDocumento == (int)TiposDocumentos.Pedido)
                                            {

                                                if (!EsServicio(ref objOperationResult, itemSeparación.v_IdProductoAlmacen))
                                                {
                                                    separacionproducto objEntitySeparacionProducto = separacionproductoAssembler.ToEntity(objEntityDetail);
                                                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 47);
                                                    newIdSeparacionProducto = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZD");
                                                    objEntitySeparacionProducto.v_IdSeparacionProducto = newIdSeparacionProducto;
                                                    objEntitySeparacionProducto.v_IdPedido = newIdPedido;
                                                    objEntitySeparacionProducto.t_InsertaFecha = DateTime.Now;
                                                    objEntitySeparacionProducto.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                                    objEntitySeparacionProducto.i_Eliminado = 0;
                                                    dbContext.AddToseparacionproducto(objEntitySeparacionProducto);
                                                    dbContext.SaveChanges();
                                                }
                                            }
                                        }
                                        else
                                        {


                                            itemSeparación = new productoalmacen();
                                            //int intNodeId = int.Parse(ClientSession[0]);
                                            int SecuentialIdProductoAlmacen = objSecuentialBL.GetNextSecuentialId(
                                                intNodeId, 30);
                                            string newIdProductoAlmacen = Utils.GetNewId(int.Parse(ClientSession[0]),
                                                SecuentialIdProductoAlmacen, "PA");
                                            itemSeparación.v_IdProductoAlmacen = newIdProductoAlmacen;
                                            itemSeparación.i_IdAlmacen = objEntityDetail.i_IdAlmacen;
                                            itemSeparación.v_ProductoDetalleId = objEntityDetail.v_IdProductoDetalle;
                                            itemSeparación.d_StockMinimo = 0;
                                            itemSeparación.d_StockActual = objEntityDetail.d_Separacion_CantidadEmpaque.Value * -1;
                                            itemSeparación.i_Eliminado = 0;
                                            itemSeparación.d_SeparacionTotal = objEntityDetail.d_Separacion_CantidadEmpaque.Value;
                                            itemSeparación.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                                            itemSeparación.t_InsertaFecha = DateTime.Now;
                                            itemSeparación.v_Periodo = periodo;
                                            dbContext.AddToproductoalmacen(itemSeparación);

                                            if (pobjDtoEntity.i_IdTipoDocumento == (int)TiposDocumentos.Pedido)
                                            {

                                                if (!EsServicio(ref objOperationResult, newIdProductoAlmacen))
                                                {
                                                    separacionproducto objEntitySeparacionProducto = separacionproductoAssembler.ToEntity(objEntityDetail);
                                                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 47);
                                                    newIdSeparacionProducto = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZD");
                                                    objEntitySeparacionProducto.v_IdSeparacionProducto = newIdSeparacionProducto;
                                                    objEntitySeparacionProducto.v_IdPedido = newIdPedido;
                                                    objEntitySeparacionProducto.t_InsertaFecha = DateTime.Now;
                                                    objEntitySeparacionProducto.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                                    objEntitySeparacionProducto.i_Eliminado = 0;
                                                    objEntitySeparacionProducto.v_IdProductoAlmacen = newIdProductoAlmacen;
                                                    dbContext.AddToseparacionproducto(objEntitySeparacionProducto);
                                                    dbContext.SaveChanges();
                                                }
                                            }


                                            dbContext.SaveChanges();

                                        }
                                    }
                                }


                            }

                            #endregion

                        }
                        #region Actualiza Correlativo EmpresaDetalle

                        //_objDocumentoBL.ActualizarCorrelativoPorSerie(ref objOperationResult,
                        //    Globals.ClientSession.i_IdEstablecimiento.Value, pobjDtoEntity.i_IdTipoDocumento.Value,
                        //    pobjDtoEntity.v_SerieDocumento, int.Parse(pobjDtoEntity.v_CorrelativoDocumento) + 1);

                        _objDocumentoBL.ActualizarCorrelativoPorSerie(ref objOperationResult,
                            pobjDtoEntity.i_IdEstablecimiento.Value, pobjDtoEntity.i_IdTipoDocumento.Value,
                            pobjDtoEntity.v_SerieDocumento, int.Parse(pobjDtoEntity.v_CorrelativoDocumento) + 1);

                        #endregion

                        pobjOperationResult.Success = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "pedido",
                            newIdPedido);
                        ts.Complete();
                        return newIdPedido;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PedidoBL.InsertarPedido()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }



        public bool EsServicio(ref OperationResult pObjOperationResult, string pstringProductoAlmacen)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            string periodo = Globals.ClientSession.i_Periodo.ToString();
            var query = (from n in dbContext.productoalmacen

                         join b in dbContext.productodetalle on n.v_ProductoDetalleId equals b.v_IdProductoDetalle into b_join

                         from b in b_join.DefaultIfEmpty()

                         join c in dbContext.producto on b.v_IdProducto equals c.v_IdProducto into c_join

                         from c in c_join.DefaultIfEmpty()

                         where n.v_IdProductoAlmacen == pstringProductoAlmacen && n.i_Eliminado == 0 && n.v_Periodo == periodo

                         select new { c.i_EsServicio }).FirstOrDefault();

            if (query == null)
            {
                return false;
            }
            else if (query.i_EsServicio.Value == 1)
            {

                return true;
            }
            else
            {
                return false;
            }
            // pObjOperationResult.Success  = 1;

        }

        public bool VerificarSiTieneFacturas(int TipoDoc, string SeriePedido, string NroPedido)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                if (TipoDoc == (int)TiposDocumentos.Pedido)
                {
                    var Ventas = dbContext.venta.Where(l => l.i_Eliminado == 0 && l.v_NroPedido == SeriePedido + " - " + NroPedido).ToList();
                    if (Ventas.Any())
                        return true;
                    else
                        return false;
                }
                else
                    return false;


            }


        }

        public decimal ObtenerCantidadSeparacionProducto(ref OperationResult pobjOperationResult, string strIdPedido,
            string strIdProductoAlmacen)
        {
            decimal SeparacionAnterior = 0;

            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var query = (from n in dbContext.separacionproducto
                         where n.v_IdPedido == strIdPedido && n.v_IdProductoAlmacen == strIdProductoAlmacen && n.i_Eliminado == 0
                         // select new { n.d_Separacion_Cantidad }).FirstOrDefault();
                         select new { n.d_Separacion_CantidadEmpaque, n.d_Separacion_Cantidad }).FirstOrDefault();
            if (query != null)
            {
                //SeparacionAnterior = Convert.ToDecimal(query.d_Separacion_Cantidad );
                SeparacionAnterior = query.d_Separacion_CantidadEmpaque != null ? Convert.ToDecimal(query.d_Separacion_CantidadEmpaque) : Convert.ToDecimal(query.d_Separacion_Cantidad);
            }
            return SeparacionAnterior;

        }

        public int ValidarStock(ref OperationResult pobjOperationResult, string strIdProductoDetalle)
        {
            try
            {

                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                try
                {
                    var query = (from a in dbContext.producto

                                 join b in dbContext.productodetalle on new { Prod = a.v_IdProducto, eliminado = 0 } equals new { Prod = b.v_IdProducto, eliminado = b.i_Eliminado.Value } into a_join

                                 from b in a_join.DefaultIfEmpty()

                                 where b.v_IdProductoDetalle == strIdProductoDetalle && a.i_Eliminado == 0
                                 select new { i_ValidarStock = a.i_ValidarStock == null ? 0 : a.i_ValidarStock }).FirstOrDefault();


                    pobjOperationResult.Success = 1;
                    return query.i_ValidarStock.Value;
                }
                catch (Exception ex)
                {

                    pobjOperationResult.Success = 0;
                    return 0;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PedidoBL.ValidarStock()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return 0;
            }


        }

        public productoalmacenDto ObtenerProductoAlmacen(ref OperationResult pobjOperationResult, int IdAlmacen, string strIdProductoDetalle)
        {
            try
            {

                var periodo = Globals.ClientSession.i_Periodo.ToString();

                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                productoalmacenDto _productoalmacenDto = new productoalmacenDto();

                //productoalmacen query = (from n in dbContext.productoalmacen
                //                         where n.v_IdProductoAlmacen == strIdProductoAlmacen && n.i_Eliminado ==0 && n.v_Periodo ==periodo 
                //                         select n
                //    ).FirstOrDefault();

                productoalmacen query = (from n in dbContext.productoalmacen
                                         where n.v_ProductoDetalleId == strIdProductoDetalle && n.i_Eliminado == 0 && n.v_Periodo == periodo && n.i_IdAlmacen == IdAlmacen
                                         select n
                    ).FirstOrDefault();

                _productoalmacenDto = productoalmacenAssembler.ToDTO(query);

                pobjOperationResult.Success = 1;
                return _productoalmacenDto;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PedidoBL.ObtenerProductoAlmacen()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public bool ComprobarExistenciaDocumento(ref OperationResult pobjOperationResult, int pintIdTipoDocumento,
            string pstrSerieDoc, string pstrCorrelativoDoc, string pstrIdPedido)
        {
            try
            {
                if (pintIdTipoDocumento != -1 && pstrSerieDoc != null && pstrCorrelativoDoc != null)
                // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    if (pstrIdPedido == null)
                    // SI el idVenta es nulo se esta consultado de un pedido nuevo que no ha sido guardada
                    {
                        var query = (from n in dbContext.pedido
                                     where n.i_Eliminado == 0
                                           && n.i_IdTipoDocumento == pintIdTipoDocumento
                                           && n.v_SerieDocumento == pstrSerieDoc &&
                                           n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                     select n
                            ).FirstOrDefault();
                        if (query == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else // si no es nulo se comprueba de un pedido que esta siendo modificada
                    {
                        var query = (from n in dbContext.pedido
                                     where n.i_Eliminado == 0
                                           && n.i_IdTipoDocumento == pintIdTipoDocumento
                                           && n.v_SerieDocumento == pstrSerieDoc &&
                                           n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                           && n.v_IdPedido != pstrIdPedido
                                     select n
                            ).FirstOrDefault();
                        if (query == null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
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
                pobjOperationResult.AdditionalInformation = "PedidoBL.ComprobarExistenciaDocumento()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        public string ActualizarPedido(ref OperationResult pobjOperationResult, pedidoDto pobjDtoEntity,
            List<string> ClientSession, List<pedidodetalleDto> pTemp_Insertar, List<pedidodetalleDto> pTemp_Editar,
            List<pedidodetalleDto> pTemp_Eliminar, List<separacionproductoDto> pTemp_InsertarSeparacion,
            List<separacionproductoDto> pTemp_EditarSeparacion, List<separacionproductoDto> pTemp_EliminarSeparacion)
        {
            try
            {

                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    OperationResult objOperationResult = new OperationResult();
                    pedidodetalleDto pobjDtoPedidoDetalle = new pedidodetalleDto();
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    int SecuentialId = 0;
                    string newIdPedidoDetalle = string.Empty, newIdSeparacionProducto = string.Empty;
                    int intNodeId;
                    string _periodo = Globals.ClientSession.i_Periodo.ToString();

                    #region Actualiza Cabecera

                    intNodeId = int.Parse(ClientSession[0]);
                    pedido objEntity = (from a in dbContext.pedido
                                        where a.v_IdPedido == pobjDtoEntity.v_IdPedido
                                        select a).FirstOrDefault();

                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    objEntity = pedidoAssembler.ToEntity(pobjDtoEntity);
                    dbContext.pedido.ApplyCurrentValues(objEntity);

                    #endregion


                    #region Actualiza Detalle

                    foreach (pedidodetalleDto pedidodetalleDto in pTemp_Insertar)
                    {
                        pedidodetalle objEntityPedidoDetalle = pedidodetalleAssembler.ToEntity(pedidodetalleDto);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 43);
                        newIdPedidoDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZN");
                        objEntityPedidoDetalle.v_IdPedidoDetalle = newIdPedidoDetalle;
                        objEntityPedidoDetalle.t_InsertaFecha = DateTime.Now;
                        objEntityPedidoDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityPedidoDetalle.i_Eliminado = 0;
                        dbContext.AddTopedidodetalle(objEntityPedidoDetalle);
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName,
                            "pedidodetalle", newIdPedidoDetalle);
                    }


                    foreach (pedidodetalleDto pedidodetalleDto in pTemp_Editar)
                    {
                        pedidodetalle _objEntity = pedidodetalleAssembler.ToEntity(pedidodetalleDto);

                        var query = (from n in dbContext.pedidodetalle
                                     where n.v_IdPedidoDetalle == pedidodetalleDto.v_IdPedidoDetalle
                                     select n).FirstOrDefault();

                        _objEntity.t_ActualizaFecha = DateTime.Now;
                        _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        dbContext.pedidodetalle.ApplyCurrentValues(_objEntity);
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName,
                            "pedidodetalle", query.v_IdPedidoDetalle);
                    }

                    foreach (pedidodetalleDto pedidodetalleDto in pTemp_Eliminar)
                    {
                        pedidodetalle _objEntity = pedidodetalleAssembler.ToEntity(pedidodetalleDto);
                        var query = (from n in dbContext.pedidodetalle
                                     where n.v_IdPedidoDetalle == pedidodetalleDto.v_IdPedidoDetalle
                                     select n).FirstOrDefault();

                        if (query != null)
                        {
                            query.t_ActualizaFecha = DateTime.Now;
                            query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            query.i_Eliminado = 1;
                        }

                        dbContext.pedidodetalle.ApplyCurrentValues(query);
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                            "pedidodetalle", query.v_IdPedidoDetalle);
                    }

                    #endregion

                    dbContext.SaveChanges();

                    //  if (pobjDtoEntity.i_IdTipoDocumento == 430)
                    if (pobjDtoEntity.i_IdTipoDocumento == (int)TiposDocumentos.Pedido)
                    {

                        #region Actualiza-ProductoAlmacen

                        if (pTemp_InsertarSeparacion != null)
                        {
                            foreach (var objEntityDetail in pTemp_InsertarSeparacion)
                            {
                                // Actualizar la separacion del producto en el almacén correspondiente
                                if (!EsServicio(ref objOperationResult, objEntityDetail.v_IdProductoAlmacen))
                                {

                                   

                                    productoalmacen itemSeparación = (from A in dbContext.productoalmacen
                                                              where A.v_IdProductoAlmacen == objEntityDetail.v_IdProductoAlmacen && A.v_Periodo == _periodo && A.i_Eliminado == 0
                                                              select A).FirstOrDefault();
                                       
                                    itemSeparación.d_SeparacionTotal = itemSeparación.d_SeparacionTotal +
                                                                       objEntityDetail.d_Separacion_CantidadEmpaque;
                                    itemSeparación.i_ActualizaIdUsuario = int.Parse(ClientSession[2]); // Auditoría
                                    itemSeparación.t_ActualizaFecha = DateTime.Now;
                                    dbContext.productoalmacen.ApplyCurrentValues(itemSeparación);
                                }


                            }
                        }

                        if (pTemp_EditarSeparacion != null)
                        {
                            foreach (var objEntityDetail in pTemp_EditarSeparacion)
                            {
                                if (!EsServicio(ref objOperationResult, objEntityDetail.v_IdProductoAlmacen))
                                // CUando no es Servicio
                                {
                                    productoalmacen itemSeparacion = (from A in dbContext.productoalmacen
                                                                      where A.v_IdProductoAlmacen == objEntityDetail.v_IdProductoAlmacen && A.i_Eliminado.Value == 0 && A.v_Periodo == _periodo
                                                                      select A).FirstOrDefault();
                                    //Revertir del mov. anterior
                                    // Y luego ingresar

                                    separacionproducto separacionProductoAnterior = (from B in dbContext.separacionproducto
                                                                                     where
                                                                                         B.v_IdPedido == objEntityDetail.v_IdPedido &&
                                                                                         B.v_IdProductoAlmacen == objEntityDetail.v_IdProductoAlmacen
                                                                                     select B).FirstOrDefault();




                                    itemSeparacion.d_SeparacionTotal = separacionProductoAnterior == null ? itemSeparacion.d_SeparacionTotal : itemSeparacion.d_SeparacionTotal -
                                                                       separacionProductoAnterior.d_Separacion_CantidadEmpaque;
                                    itemSeparacion.d_SeparacionTotal = itemSeparacion.d_SeparacionTotal +
                                                                       objEntityDetail.d_Separacion_CantidadEmpaque;


                                    itemSeparacion.t_ActualizaFecha = DateTime.Now;
                                    itemSeparacion.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                    dbContext.productoalmacen.ApplyCurrentValues(itemSeparacion);
                                }

                            }

                        }

                        if (pTemp_EliminarSeparacion != null)
                        {
                            foreach (var objEntityDetail in pTemp_EliminarSeparacion)
                            {
                                if (!EsServicio(ref objOperationResult, objEntityDetail.v_IdProductoAlmacen))
                                // CUando no es Servicio
                                {

                                    productoalmacen itemSeparacion = (from A in dbContext.productoalmacen
                                                                      where A.v_IdProductoAlmacen == objEntityDetail.v_IdProductoAlmacen && A.i_Eliminado == 0 && A.v_Periodo == _periodo
                                                                      select A).FirstOrDefault();
                                    //Revertir del mov. anterior
                                    // Y luego ingresar

                                    separacionproducto separacionProductoEliminar =
                                        (from B in dbContext.separacionproducto
                                         where
                                             B.v_IdPedido == objEntityDetail.v_IdPedido &&
                                             B.v_IdProductoAlmacen == objEntityDetail.v_IdProductoAlmacen
                                         select B).FirstOrDefault();


                                    itemSeparacion.d_SeparacionTotal = separacionProductoEliminar != null ? itemSeparacion.d_SeparacionTotal -
                                        separacionProductoEliminar.d_Separacion_Cantidad : itemSeparacion.d_SeparacionTotal;
                                    itemSeparacion.t_ActualizaFecha = DateTime.Now;
                                    itemSeparacion.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                    dbContext.productoalmacen.ApplyCurrentValues(itemSeparacion);
                                }

                            }

                        }

                        #endregion

                        # region Actualiza-SeparacionProducto

                        foreach (separacionproductoDto separacionproductoDto in pTemp_InsertarSeparacion)
                        {
                            if (!EsServicio(ref objOperationResult, separacionproductoDto.v_IdProductoAlmacen))
                            // CUando no es Servicio
                            {

                                separacionproducto objEntitySeparacionProducto =
                                    separacionproductoAssembler.ToEntity(separacionproductoDto);
                                SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 47);
                                newIdSeparacionProducto = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZD");
                                objEntitySeparacionProducto.v_IdSeparacionProducto = newIdSeparacionProducto;
                                objEntitySeparacionProducto.t_InsertaFecha = DateTime.Now;
                                objEntitySeparacionProducto.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                objEntitySeparacionProducto.i_Eliminado = 0;
                                dbContext.AddToseparacionproducto(objEntitySeparacionProducto);
                                dbContext.SaveChanges();
                            }
                        }


                        foreach (separacionproductoDto separacionproductoDto in pTemp_EditarSeparacion)
                        {
                            if (!EsServicio(ref objOperationResult, separacionproductoDto.v_IdProductoAlmacen))
                            // CUando no es Servicio
                            {
                                separacionproducto query = (from n in dbContext.separacionproducto
                                                            where
                                                                n.v_IdPedido == separacionproductoDto.v_IdPedido &&
                                                                n.v_IdProductoAlmacen == separacionproductoDto.v_IdProductoAlmacen
                                                            select n).FirstOrDefault();

                                if (query != null)
                                {
                                    query.d_Separacion_Cantidad = separacionproductoDto.d_Separacion_Cantidad.Value;
                                    query.d_Separacion_CantidadEmpaque = separacionproductoDto.d_Separacion_CantidadEmpaque;
                                    query.t_ActualizaFecha = DateTime.Now;
                                    query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                    dbContext.separacionproducto.ApplyCurrentValues(query);
                                }
                            }
                        }

                        foreach (separacionproductoDto separacionproductoDto in pTemp_EliminarSeparacion)
                        {
                            if (!EsServicio(ref objOperationResult, separacionproductoDto.v_IdProductoAlmacen))
                            // CUando no es Servicio
                            {
                                separacionproducto _objEntity =
                                    separacionproductoAssembler.ToEntity(separacionproductoDto);

                                var query = (from n in dbContext.separacionproducto
                                             where
                                                 n.v_IdPedido == separacionproductoDto.v_IdPedido &&
                                                 n.v_IdProductoAlmacen == separacionproductoDto.v_IdProductoAlmacen
                                             select n).FirstOrDefault();

                                if (query != null)
                                {
                                    query.t_ActualizaFecha = DateTime.Now;
                                    query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                    query.i_Eliminado = 1;
                                    dbContext.separacionproducto.ApplyCurrentValues(query);
                                }


                            }

                        }

                        #endregion
                    }

                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "pedido",
                        objEntity.v_IdPedido);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return pobjDtoEntity.v_IdPedido;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PedidoBL.ActualizarPedido()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void ActualizarEstadoCotizacion(ref OperationResult pobjOperationResult, string pstrIdPedido)
        {
            //  SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            //   var Cotizacion=(from n in dbContext.pedido 
            //                    where n.v_IdPedido == pstrIdPedido
            //                   select n).FirstOrDefault ();  

            //Cotizacion.i_IdEstado =

        }

        public List<string[]> DevolverNombres(string pstrIdPedido)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = DevuelveNombresPedido(dbContext, pstrIdPedido, Globals.ClientSession.i_Periodo.ToString());


                if (query != null)
                {
                    List<string[]> Lista = new List<string[]>();
                    foreach (var Fila in query)
                    {
                        string[] Cadena = new string[13];
                        Cadena[0] = Fila.CodigoInterno;
                        Cadena[1] = null;
                        Cadena[2] = Fila.Empaque.ToString();
                        Cadena[3] = Fila.UMEmpaque;
                        Cadena[4] = Fila.i_IdUnidadMedida.ToString();
                        Cadena[5] = Fila.v_IdProductoAlmacen;
                        Cadena[6] = Fila.d_SeparacionTotal.ToString();
                        Cadena[7] = Fila.d_StockActual.ToString();
                        Cadena[8] = Fila.i_EsServicio.ToString();
                        Cadena[9] = Fila.i_EsAfectoDetraccion.ToString();
                        Cadena[10] = Fila.i_NombreEditable.ToString();
                        Cadena[11] = Fila.i_ValidarStock.ToString();
                        Cadena[12] = Fila.i_IdUnidadMedidaEmpaque.ToString();
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


        public bool EsValidoCodProducto(string pstrCodInterno)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    producto P = (from p in dbContext.producto
                                  where p.v_CodInterno == pstrCodInterno && p.i_Eliminado == 0
                                  select p).FirstOrDefault();

                    if (P == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
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
            try
            {

                string pstrPeriodo = Globals.ClientSession.i_Periodo.ToString();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region QueryCompilado

                    var _CompiledQuery =
                        CompiledQuery.Compile(
                            (SAMBHSEntitiesModelWin DataContext, int Almacen, string Cliente, string CodInterno, string periodo) =>
                                from n in DataContext.producto

                                join D in DataContext.productodetalle on n.v_IdProducto equals D.v_IdProducto into
                                    D_join
                                from D in D_join.DefaultIfEmpty()

                                join J4 in DataContext.datahierarchy on new { a = n.i_IdUnidadMedida.Value, b = 17 }
                                    equals new { a = J4.i_ItemId, b = J4.i_GroupId } into J4_join
                                from J4 in J4_join.DefaultIfEmpty()

                                join c in DataContext.cliente on pstrIdCliente equals c.v_IdCliente into c_join
                                from c in c_join.DefaultIfEmpty()


                                join J2 in DataContext.listaprecio on new { IdListaPrecios = c.i_IdListaPrecios.Value }
                                    equals new { IdListaPrecios = J2.i_IdLista.Value } into J2_join
                                from J2 in J2_join.DefaultIfEmpty()


                                join J5 in DataContext.listapreciodetalle on new { lp = J2.v_IdListaPrecios, eliminado = 0 } equals new { lp = J5.v_IdListaPrecios, eliminado = J5.i_Eliminado.Value } into J5_join
                                from J5 in J5_join.DefaultIfEmpty()



                                join J3 in DataContext.productoalmacen on new { a = D.v_IdProductoDetalle, IdProductoDetalle = J5.v_IdProductoDetalle, almacen = J5.i_IdAlmacen.Value, eliminado = 0, _periodo = periodo }
                                                                     equals new { a = J3.v_ProductoDetalleId, IdProductoDetalle = J3.v_ProductoDetalleId, almacen = J3.i_IdAlmacen, eliminado = J3.i_Eliminado.Value, _periodo = J3.v_Periodo } into J3_join

                                from J3 in J3_join.DefaultIfEmpty()

                                where
                                    n.i_Eliminado == 0 && J3.v_ProductoDetalleId == J5.v_IdProductoDetalle &&
                                    n.v_CodInterno == CodInterno && J3.i_IdAlmacen == pintIdAlmacen &&
                                    J2.i_IdAlmacen == pintIdAlmacen
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
                                    i_PrecioEditable = n.i_PrecioEditable,
                                    IdMoneda = J2 != null ? J2.i_IdMoneda.Value : 0,
                                }
                            );

                    #endregion

                    var query = _CompiledQuery(dbContext, pintIdAlmacen, pstrIdCliente, pstrCodInterno, Globals.ClientSession.i_Periodo.ToString()).FirstOrDefault();

                    if (query == null)
                    {
                        var query2 = (from n in dbContext.producto
                                      join D in dbContext.productodetalle on n.v_IdProducto equals D.v_IdProducto into D_join
                                      from D in D_join.DefaultIfEmpty()

                                      join J3 in dbContext.productoalmacen on new { a = D.v_IdProductoDetalle, eliminado = 0, almacen = pintIdAlmacen, _periodo = pstrPeriodo } equals new { a = J3.v_ProductoDetalleId, eliminado = J3.i_Eliminado.Value, almacen = J3.i_IdAlmacen, _periodo = J3.v_Periodo }

                                      join J4 in dbContext.datahierarchy on new { a = n.i_IdUnidadMedida.Value, b = 17 }
                                          equals new { a = J4.i_ItemId, b = J4.i_GroupId } into J4_join
                                      from J4 in J4_join.DefaultIfEmpty()

                                      where
                                          n.i_Eliminado == 0 && J3.v_IdProductoAlmacen != "N002-PA000000000" &&
                                          n.v_CodInterno == pstrCodInterno && J3.i_IdAlmacen == pintIdAlmacen

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
                                          i_PrecioEditable = n.i_PrecioEditable,
                                          IdMoneda = 0,

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




        public List<productoshortDto> DevolverArticuloPorCodInternoNuevo(ref OperationResult pobjOperationResult,
            string pstrSortExpression, string pstrFilterExpression, int pintIdAlmacen, string pstrIdCliente,
            string codigoProducto)
        {
            try
            {
                var periodo = Globals.ClientSession.i_Periodo.ToString();
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var Cliente = dbContext.cliente.Where(p => p.v_IdCliente == pstrIdCliente).FirstOrDefault();

                int IdListaPrecios = Cliente != null ? Cliente.i_IdListaPrecios.Value : 1;

                var ListaPrecioDto =
                    dbContext.listaprecio.FirstOrDefault(
                        p => p.i_IdAlmacen == pintIdAlmacen && p.i_IdLista == IdListaPrecios && p.i_Eliminado == 0);

                var DSListaPreciosDetalle =
                    dbContext.listapreciodetalle.Where(
                        p =>
                            p.listaprecio.i_IdAlmacen == pintIdAlmacen && p.listaprecio.i_IdLista == IdListaPrecios &&
                            p.i_Eliminado == 0).ToList();

                var query = (from n in dbContext.producto
                             join D in dbContext.productodetalle on n.v_IdProducto equals D.v_IdProducto into D_join
                             from D in D_join.DefaultIfEmpty()

                             join J3 in dbContext.productoalmacen on new { a = D.v_IdProductoDetalle, b = pintIdAlmacen, eliminado = 0, _periodo = periodo }
                                 equals new { a = J3.v_ProductoDetalleId, b = J3.i_IdAlmacen, eliminado = J3.i_Eliminado.Value, _periodo = J3.v_Periodo } into J3_join
                             from J3 in J3_join.DefaultIfEmpty()

                             join J4 in dbContext.datahierarchy on new { a = n.i_IdUnidadMedida.Value, b = 17 }
                                 equals new { a = J4.i_ItemId, b = J4.i_GroupId } into J4_join
                             from J4 in J4_join.DefaultIfEmpty()

                             where
                                 n.i_Eliminado == 0 && n.i_EsActivoFijo == 0 && n.v_CodInterno == codigoProducto &&
                                 J3.v_IdProductoAlmacen != null && n.i_EsActivo == 1

                             select new
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
                                 i_PrecioEditable = n.i_PrecioEditable,
                                 ProductoAlmacen = J3.v_IdProductoAlmacen,
                                 i_IdAlmacen = J3.i_IdAlmacen,
                                 v_Descripcion2 = n.v_Descripcion2,
                                 
                             }
                    ).ToList().Select(p => new productoshortDto
                    {
                        v_IdProducto = p.v_IdProducto,
                        v_IdProductoDetalle = p.v_IdProductoDetalle,
                        v_Descripcion = p.v_Descripcion,
                        v_CodInterno = p.v_CodInterno,
                        i_EsServicio = p.i_EsServicio,
                        i_EsLote = p.i_EsLote,
                        i_IdTipoProducto = p.i_IdTipoProducto,
                        stockActual = p.stockActual,
                        v_IdProductoAlmacen = p.v_IdProductoAlmacen,
                        d_separacion = p.d_separacion,
                        i_IdUnidadMedida = p.i_IdUnidadMedida,
                        EmpaqueUnidadMedida = p.EmpaqueUnidadMedida,
                        d_Empaque = p.d_Empaque,
                        i_EsAfectoDetraccion = p.i_EsAfectoDetraccion,
                        i_NombreEditable = p.i_NombreEditable,
                        StockDisponible = p.StockDisponible,
                        i_ValidarStock = p.i_ValidarStock,
                        i_EsAfectoPercepcion = p.i_EsAfectoPercepcion,
                        d_TasaPercepcion = p.d_TasaPercepcion,
                        i_PrecioEditable = p.i_PrecioEditable,
                        d_Precio =
                            ObtenerPrecioDescuento(p.v_IdProductoDetalle, p.i_IdAlmacen, DSListaPreciosDetalle, TipoResultado.Precio),
                        d_Descuento =
                            ObtenerPrecioDescuento(p.v_IdProductoDetalle, p.i_IdAlmacen, DSListaPreciosDetalle, TipoResultado.Descuento),

                        IdMoneda = ListaPrecioDto != null ? ListaPrecioDto.i_IdMoneda.Value : -1,
                        v_Descripcion2 = p.v_Descripcion2,
                    }
                    ).AsQueryable();

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);

                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                List<productoshortDto> objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "MovimientoBL.DevolverArticuloPorCodInternoNuevo()";
                return null;
            }
        }

        public List<productoshortDto> DevolverArticuloPorCodInternoNuevo(
                ref OperationResult pobjOperationResult,
                int pintIdAlmacen,
                string codigoProducto)
        {
            try
            {
                var periodo = Globals.ClientSession.i_Periodo.ToString();
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.producto
                                 join D in dbContext.productodetalle on n.v_IdProducto equals D.v_IdProducto into D_join
                                 from D in D_join.DefaultIfEmpty()

                                 join J3 in dbContext.productoalmacen on new { a = D.v_IdProductoDetalle, b = pintIdAlmacen, eliminado = 0, _periodo = periodo }
                                     equals new { a = J3.v_ProductoDetalleId, b = J3.i_IdAlmacen, eliminado = J3.i_Eliminado.Value, _periodo = J3.v_Periodo } into J3_join
                                 from J3 in J3_join.DefaultIfEmpty()

                                 join J4 in dbContext.datahierarchy on new { a = n.i_IdUnidadMedida.Value, b = 17, eliminado = 0 }
                                     equals new { a = J4.i_ItemId, b = J4.i_GroupId, eliminado = J4.i_IsDeleted.Value } into J4_join
                                 from J4 in J4_join.DefaultIfEmpty()

                                 where
                                     n.i_Eliminado == 0 && n.i_EsActivoFijo == 0 && n.v_CodInterno == codigoProducto &&
                                     J3.v_IdProductoAlmacen != null && n.i_EsActivo == 1

                                 select new
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
                                     i_PrecioEditable = n.i_PrecioEditable,
                                     ProductoAlmacen = J3.v_IdProductoAlmacen,
                                     n.d_PrecioVenta,
                                     v_Descripcion2 = n.v_Descripcion2,
                                     

                                     i_SolicitarNroLoteIngreso = n.i_SolicitarNroLoteIngreso ?? 0,
                                     i_SolicitarNroSerieIngreso = n.i_SolicitarNroSerieIngreso ?? 0,
                                     i_SolicitaOrdenProduccionIngreso = n.i_SolicitaOrdenProduccionIngreso ?? 0,
                                     i_SolicitarNroSerieSalida =n.i_SolicitarNroSerieSalida ?? 0,
                                     i_SolicitarNroLoteSalida = n.i_SolicitarNroLoteSalida ?? 0,
                                     i_SolicitaOrdenProduccionSalida = n.i_SolicitaOrdenProduccionSalida ?? 0,

                                 }
                        ).ToList().Select(p => new productoshortDto
                        {
                            v_IdProducto = p.v_IdProducto,
                            v_IdProductoDetalle = p.v_IdProductoDetalle,
                            v_Descripcion = p.v_Descripcion,
                            v_CodInterno = p.v_CodInterno,
                            i_EsServicio = p.i_EsServicio,
                            i_EsLote = p.i_EsLote,
                            i_IdTipoProducto = p.i_IdTipoProducto,
                            stockActual = p.stockActual,
                            v_IdProductoAlmacen = p.v_IdProductoAlmacen,
                            d_separacion = p.d_separacion,
                            i_IdUnidadMedida = p.i_IdUnidadMedida,
                            EmpaqueUnidadMedida = p.EmpaqueUnidadMedida,
                            d_Empaque = p.d_Empaque,
                            i_EsAfectoDetraccion = p.i_EsAfectoDetraccion,
                            i_NombreEditable = p.i_NombreEditable,
                            StockDisponible = p.StockDisponible,
                            i_ValidarStock = p.i_ValidarStock,
                            i_EsAfectoPercepcion = p.i_EsAfectoPercepcion,
                            d_TasaPercepcion = p.d_TasaPercepcion,
                            i_PrecioEditable = p.i_PrecioEditable,
                            d_Precio = p.d_PrecioVenta ?? 0,
                            d_Descuento = 0,
                            IdMoneda = 1,
                            v_Descripcion2 = p.v_Descripcion2,

                            i_SolicitarNroLoteIngreso = p.i_SolicitarNroLoteIngreso,
                            i_SolicitarNroSerieIngreso = p.i_SolicitarNroSerieIngreso,
                            i_SolicitaOrdenProduccionIngreso = p.i_SolicitaOrdenProduccionIngreso,
                            i_SolicitarNroSerieSalida = p.i_SolicitarNroSerieSalida,
                            i_SolicitarNroLoteSalida = p.i_SolicitarNroLoteSalida,
                            i_SolicitaOrdenProduccionSalida = p.i_SolicitaOrdenProduccionSalida,
                        }
                        ).AsQueryable();

                    List<productoshortDto> objData = query.ToList();
                    pobjOperationResult.Success = 1;
                    return objData;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                pobjOperationResult.AdditionalInformation = "MovimientoBL.DevolverArticuloPorCodInternoNuevo()";
                return null;
            }
        }


        private decimal ObtenerPrecioDescuento(string pstrIdProductoDetalle, int Almacen, List<listapreciodetalle> DS,
            TipoResultado oTipoResultado)
        {
            try
            {
                var data = DS.FirstOrDefault(p => p.v_IdProductoDetalle == pstrIdProductoDetalle && p.i_Eliminado.Value == 0 && p.i_IdAlmacen == Almacen);

                switch (oTipoResultado)
                {
                    case TipoResultado.Precio:
                        return data != null ? data.d_Precio.Value : 0;

                    default:
                        return data != null ? data.d_Descuento.Value : 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private enum TipoResultado
        {
            Precio = 1,
            Descuento = 2
        }


        #endregion

        #region QueryCompilado

        public static Func<SAMBHSEntitiesModelWin, string, string, IQueryable<SAMBHS.Common.BE.pedidoDto.CompiladoResultPedido>>
            DevuelveNombresPedido = CompiledQuery.Compile((SAMBHSEntitiesModelWin db, string ID, string Periodo) =>
                from n in db.pedidodetalle
                join A in db.productodetalle on new { pd = n.v_IdProductoDetalle, eliminado = 0 } equals new { pd = A.v_IdProductoDetalle, eliminado = A.i_Eliminado.Value } into A_join
                from A in A_join.DefaultIfEmpty()

                join B in db.producto on new { pd = A.v_IdProducto, eliminado = 0 } equals new { pd = B.v_IdProducto, eliminado = B.i_Eliminado.Value } into B_join

                from B in B_join.DefaultIfEmpty()

                join C in db.productoalmacen on new { strIdProducto = A.v_IdProductoDetalle, a = n.i_IdAlmacen.Value, eliminado = 0 }
                    equals new { strIdProducto = C.v_ProductoDetalleId, a = C.i_IdAlmacen, eliminado = C.i_Eliminado.Value } into C_join
                from C in C_join.DefaultIfEmpty()

                join D in db.almacen on new { a = n.i_IdAlmacen.Value, eliminado = 0 } equals new { a = D.i_IdAlmacen, eliminado = D.i_Eliminado.Value } into D_join
                from D in D_join.DefaultIfEmpty()

                join J1 in db.datahierarchy on new { a = B.i_IdUnidadMedida.Value, b = 17 }
                    equals new { a = J1.i_ItemId, b = J1.i_GroupId } into J1_join
                from J1 in J1_join.DefaultIfEmpty()

                where n.v_IdPedido == ID && n.i_Eliminado == 0 && C.v_Periodo == Periodo

                select new SAMBHS.Common.BE.pedidoDto.CompiladoResultPedido
                {
                    Nombre = B.v_Descripcion,
                    CodigoInterno = B.v_CodInterno,
                    Empaque = B.d_Empaque,
                    UMEmpaque = J1.v_Value1,
                    i_IdUnidadMedida = B.i_IdUnidadMedida,
                    v_IdProductoAlmacen = C.v_IdProductoAlmacen,
                    d_StockActual = C.d_StockActual,
                    d_SeparacionTotal = C.d_SeparacionTotal,
                    i_EsServicio = B.i_EsServicio,
                    i_EsAfectoDetraccion = B.i_EsAfectoDetraccion,
                    i_NombreEditable = B.i_NombreEditable,
                    i_ValidarStock = B.i_ValidarStock,
                    i_IdUnidadMedidaEmpaque = B.i_IdUnidadMedida,
                    v_Periodo = C.v_Periodo
                }
                );

        #endregion

        #region Bandeja-AdministracionPedidos

        public List<pedidoDto> ListarBusquedaPedidos(ref OperationResult pobjOperationResult, string pstrSortExpression,
            DateTime F_Inicio, DateTime F_Fin, int TipoDocumento, string SerieDoc, string CorrelativoDoc, string Mes, string CorrelativoMes, string v_IdCliente, int Estado, int Moneda, string IdVendedor)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                string serie = string.Empty;
                string correlativo = string.Empty;
                List<PedidosVentas> Ventas = new List<PedidosVentas>();
                if (Globals.ClientSession.v_RucEmpresa == Constants.RucCMR)
                {
                    Ventas = (from a in dbContext.venta
                              join b in dbContext.documento on new { td = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join
                              from b in b_join.DefaultIfEmpty()
                              where a.i_Eliminado == 0 && a.v_NroPedido.Trim() != string.Empty && a.v_NroPedido.Trim() != null
                              && a.t_FechaRegistro >= F_Inicio && a.i_IdEstado == 1
                              select new PedidosVentas
                              {
                                  TodoNumeroPeedido = a.v_NroPedido.Trim(),
                                  NroFactura = b.v_Siglas + " " + a.v_SerieDocumento.Trim() + "-" + a.v_CorrelativoDocumento.Trim(),
                              }).ToList().Select(x =>
                                      {


                                          string[] SerieCorrelativo;
                                          SerieCorrelativo = x.TodoNumeroPeedido.Split(new Char[] { '-' });
                                          serie = SerieCorrelativo != null && SerieCorrelativo.Count() >= 2 ? SerieCorrelativo[0].Trim() : "";
                                          correlativo = SerieCorrelativo != null && SerieCorrelativo.Count() >= 2 ? SerieCorrelativo[1].Trim() : "";

                                          return new PedidosVentas
                                          {
                                              SeriePedido = serie.Trim(),
                                              CorrelativoPedido = correlativo.Trim(),
                                              NroFactura = x.NroFactura,

                                          };
                                      }).ToList();
                }
                else
                {
                    Ventas = (from a in dbContext.venta
                              join b in dbContext.documento on new { td = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join
                              from b in b_join.DefaultIfEmpty()
                              where a.i_Eliminado == 0 && a.v_NroPedido.Trim() != string.Empty && a.v_NroPedido.Trim() != null
                              && a.t_FechaRegistro >= F_Inicio && a.i_IdEstado == 1 && a.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value
                              select new PedidosVentas
                              {
                                  TodoNumeroPeedido = a.v_NroPedido.Trim(),
                                  NroFactura = b.v_Siglas + " " + a.v_SerieDocumento.Trim() + "-" + a.v_CorrelativoDocumento.Trim(),
                              }).ToList().Select(x =>
                                   {


                                       string[] SerieCorrelativo;
                                       SerieCorrelativo = x.TodoNumeroPeedido.Split(new Char[] { '-' });
                                       serie = SerieCorrelativo != null && SerieCorrelativo.Count() >= 2 ? SerieCorrelativo[0].Trim() : "";
                                       correlativo = SerieCorrelativo != null && SerieCorrelativo.Count() >= 2 ? SerieCorrelativo[1].Trim() : "";

                                       return new PedidosVentas
                                       {
                                           SeriePedido = serie.Trim(),
                                           CorrelativoPedido = correlativo.Trim(),
                                           NroFactura = x.NroFactura,

                                       };
                                   }).ToList();

                }

                List<pedidoDto> Pedidos = new List<pedidoDto>();
                if ((Globals.ClientSession.UsuarioEsContable == 1 && Globals.ClientSession.v_RucEmpresa == Constants.RucCMR) || (Globals.ClientSession.v_RucEmpresa != Constants.RucCMR))
                {
                    Pedidos = (from n in dbContext.pedido

                               join B in dbContext.cliente on n.v_IdCliente equals B.v_IdCliente into B_join
                               from B in B_join.DefaultIfEmpty()

                               join J1 in dbContext.documento on n.i_IdTipoDocumento equals J1.i_CodigoDocumento into J1_join

                               from J1 in J1_join.DefaultIfEmpty()

                               join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                   equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                               from J2 in J2_join.DefaultIfEmpty()

                               join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaUsuario.Value }
                                   equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                               from J3 in J3_join.DefaultIfEmpty()
                               where n.i_Eliminado == 0 && n.t_FechaEmision >= F_Inicio && n.t_FechaEmision <= F_Fin

                               && (n.v_IdCliente == v_IdCliente || v_IdCliente == "") && (n.i_IdTipoDocumento == TipoDocumento || TipoDocumento == -1) &&
                               (n.v_SerieDocumento == SerieDoc || SerieDoc == "") && (n.v_CorrelativoDocumento == CorrelativoDoc || CorrelativoDoc == "") &&
                               (n.v_Mes == Mes || Mes == "") && (n.v_Correlativo == CorrelativoMes || CorrelativoMes == "") && (n.i_IdEstado == Estado || Estado == -1) &&
                               (n.i_IdMoneda == Moneda || Moneda == -1)
                               && n.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value
                               && (n.v_IdVendedor == IdVendedor || IdVendedor == "-1")
                               select new pedidoDto
                               {

                                   v_IdPedido = n.v_IdPedido,
                                   v_Periodo = n.v_Periodo,
                                   v_Mes = n.v_Mes,
                                   v_Correlativo = n.v_Correlativo,
                                   NroRegistro = (n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim()).Trim(),
                                   v_SerieDocumento = n.v_SerieDocumento,
                                   v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                   i_IdTipoDocumento = n.i_IdTipoDocumento,
                                   t_FechaEmision = n.t_FechaEmision,
                                   t_FechaVencimiento = n.t_FechaVencimiento,
                                   i_IdEstado = n.i_IdEstado,
                                   t_InsertaFecha = n.t_InsertaFecha,
                                   i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                   t_ActualizaFecha = n.t_ActualizaFecha,
                                   v_UsuarioModificacion = J2.v_UserName,
                                   v_UsuarioCreacion = J3.v_UserName,
                                   v_IdCliente = B.v_IdCliente,
                                   d_PrecioVenta = n.d_PrecioVenta,
                                   Documento = (n.v_SerieDocumento.Trim() + "-" + n.v_CorrelativoDocumento.Trim()),
                                   TipoDocumento = J1.v_Siglas,
                                   CodCliente = B.v_CodCliente,
                                   NombreCliente =
                                       n.v_NombreClienteTemporal == ""
                                           ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " +
                                              B.v_SegundoNombre + " " + B.v_RazonSocial).Trim()
                                           : (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " +
                                              n.v_NombreClienteTemporal).Trim(),
                                   i_IdEstablecimiento = n.i_IdEstablecimiento.Value,
                                   Moneda = n.i_IdMoneda == 1 ? "S" : "D",
                                   d_TipoCambio = n.d_TipoCambio ?? 0,
                                   v_Glosa = n.v_Glosa,
                               }).ToList().Select(x =>
                                 {
                                     var Factura = Ventas.Where(a => a.SeriePedido.Trim() == x.v_SerieDocumento && a.CorrelativoPedido == x.v_CorrelativoDocumento).ToList();
                                     return new pedidoDto
                                     {
                                         v_IdPedido = x.v_IdPedido,
                                         v_Periodo = x.v_Periodo,
                                         v_Mes = x.v_Mes,
                                         v_Correlativo = x.v_Correlativo,
                                         NroRegistro = x.NroRegistro,
                                         v_SerieDocumento = x.v_SerieDocumento,
                                         v_CorrelativoDocumento = x.v_CorrelativoDocumento,
                                         i_IdTipoDocumento = x.i_IdTipoDocumento,
                                         t_FechaEmision = x.t_FechaEmision,
                                         t_FechaVencimiento = x.t_FechaVencimiento,
                                         i_IdEstado = x.i_IdEstado,
                                         t_InsertaFecha = x.t_InsertaFecha,
                                         i_ActualizaIdUsuario = x.i_ActualizaIdUsuario,
                                         t_ActualizaFecha = x.t_ActualizaFecha,
                                         v_UsuarioModificacion = x.v_UsuarioModificacion,
                                         v_UsuarioCreacion = x.v_UsuarioCreacion,
                                         v_IdCliente = x.v_IdCliente,
                                         d_PrecioVenta = x.d_PrecioVenta,
                                         Documento = x.Documento,
                                         TipoDocumento = x.TipoDocumento,
                                         CodCliente = x.CodCliente,
                                         NombreCliente = x.NombreCliente,
                                         i_IdEstablecimiento = x.i_IdEstablecimiento,
                                         NroFactura = x.i_IdTipoDocumento == (int)TiposDocumentos.Pedido ? string.Join(", ", Factura.Select(o => o.NroFactura)) : "",
                                         Moneda = x.Moneda,
                                         d_TipoCambio = x.d_TipoCambio,
                                         v_Glosa = x.v_Glosa,
                                         Total = x.d_PrecioVenta ?? 0
                                     };
                                 }).ToList();
                }
                else
                {

                    /// para las tiendas CMR .. Usuario pueden ver los pedidos dirigidos a cualquier  establecimiento pero solo los que hayan hechos ellos 
                    Pedidos = (from n in dbContext.pedido

                               join B in dbContext.cliente on n.v_IdCliente equals B.v_IdCliente into B_join
                               from B in B_join.DefaultIfEmpty()

                               join J1 in dbContext.documento on n.i_IdTipoDocumento equals J1.i_CodigoDocumento into J1_join

                               from J1 in J1_join.DefaultIfEmpty()

                               join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                   equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                               from J2 in J2_join.DefaultIfEmpty()

                               join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaUsuario.Value }
                                   equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                               from J3 in J3_join.DefaultIfEmpty()

                               where n.i_Eliminado == 0 && n.t_FechaEmision >= F_Inicio && n.t_FechaEmision <= F_Fin
                               && (n.v_IdCliente == v_IdCliente || v_IdCliente == "") && (n.i_IdTipoDocumento == TipoDocumento || TipoDocumento == -1) &&
                               (n.v_SerieDocumento == SerieDoc || SerieDoc == "") && (n.v_CorrelativoDocumento == CorrelativoDoc || CorrelativoDoc == "") &&
                               (n.v_Mes == Mes || Mes == "") && (n.v_Correlativo == CorrelativoMes || CorrelativoMes == "") && (n.i_IdEstado == Estado || Estado == -1) &&
                               (n.i_IdMoneda == Moneda || Moneda == -1)
                                   //&& n.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value
                               && n.i_InsertaUsuario == Globals.ClientSession.i_SystemUserId
                               && (n.v_IdVendedor == IdVendedor || IdVendedor == "-1")
                               select new pedidoDto
                               {

                                   v_IdPedido = n.v_IdPedido,
                                   v_Periodo = n.v_Periodo,
                                   v_Mes = n.v_Mes,
                                   v_Correlativo = n.v_Correlativo,
                                   NroRegistro = (n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim()).Trim(),
                                   v_SerieDocumento = n.v_SerieDocumento,
                                   v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                   i_IdTipoDocumento = n.i_IdTipoDocumento,
                                   t_FechaEmision = n.t_FechaEmision,
                                   t_FechaVencimiento = n.t_FechaVencimiento,
                                   i_IdEstado = n.i_IdEstado,
                                   t_InsertaFecha = n.t_InsertaFecha,
                                   i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                   t_ActualizaFecha = n.t_ActualizaFecha,
                                   v_UsuarioModificacion = J2.v_UserName,
                                   v_UsuarioCreacion = J3.v_UserName,
                                   v_IdCliente = B.v_IdCliente,
                                   d_PrecioVenta = n.d_PrecioVenta,
                                   Documento = (n.v_SerieDocumento.Trim() + "-" + n.v_CorrelativoDocumento.Trim()),
                                   TipoDocumento = J1.v_Siglas,
                                   CodCliente = B.v_CodCliente,
                                   NombreCliente =
                                       n.v_NombreClienteTemporal == ""
                                           ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " +
                                              B.v_SegundoNombre + " " + B.v_RazonSocial).Trim()
                                           : (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " +
                                              n.v_NombreClienteTemporal).Trim(),
                                   i_IdEstablecimiento = n.i_IdEstablecimiento.Value,
                                   Moneda = n.i_IdMoneda == 1 ? "S" : "D",
                                   d_TipoCambio = n.d_TipoCambio ?? 0,
                                   v_Glosa = n.v_Glosa,
                               }).ToList().Select(x =>
                               {
                                   var Factura = Ventas.Where(a => a.SeriePedido.Trim() == x.v_SerieDocumento && a.CorrelativoPedido == x.v_CorrelativoDocumento).ToList();
                                   return new pedidoDto
                                   {
                                       v_IdPedido = x.v_IdPedido,
                                       v_Periodo = x.v_Periodo,
                                       v_Mes = x.v_Mes,
                                       v_Correlativo = x.v_Correlativo,
                                       NroRegistro = x.NroRegistro,
                                       v_SerieDocumento = x.v_SerieDocumento,
                                       v_CorrelativoDocumento = x.v_CorrelativoDocumento,
                                       i_IdTipoDocumento = x.i_IdTipoDocumento,
                                       t_FechaEmision = x.t_FechaEmision,
                                       t_FechaVencimiento = x.t_FechaVencimiento,
                                       i_IdEstado = x.i_IdEstado,
                                       t_InsertaFecha = x.t_InsertaFecha,
                                       i_ActualizaIdUsuario = x.i_ActualizaIdUsuario,
                                       t_ActualizaFecha = x.t_ActualizaFecha,
                                       v_UsuarioModificacion = x.v_UsuarioModificacion,
                                       v_UsuarioCreacion = x.v_UsuarioCreacion,
                                       v_IdCliente = x.v_IdCliente,
                                       d_PrecioVenta = x.d_PrecioVenta,
                                       Documento = x.Documento,
                                       TipoDocumento = x.TipoDocumento,
                                       CodCliente = x.CodCliente,
                                       NombreCliente = x.NombreCliente,
                                       i_IdEstablecimiento = x.i_IdEstablecimiento,
                                       NroFactura = x.i_IdTipoDocumento == (int)TiposDocumentos.Pedido ? string.Join(", ", Factura.Select(o => o.NroFactura)) : "",
                                       Moneda = x.Moneda,
                                       d_TipoCambio = x.d_TipoCambio,
                                       v_Glosa = x.v_Glosa,
                                   };
                               }).ToList();

                }


                List<pedidoDto> objData = Pedidos.OrderBy(k => k.NroRegistro).ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }


        public List<pedidoDto> ListarBusquedaPedidos_(ref OperationResult pobjOperationResult, string pstrSortExpression,
            string pstrFilterExpression, DateTime F_Inicio, DateTime F_Fin)
        {
            try
            {
                var dbContext = new SAMBHSEntitiesModelWin();

                var query = (from n in dbContext.pedido

                             join B in dbContext.cliente on n.v_IdCliente equals B.v_IdCliente into B_join
                             from B in B_join.DefaultIfEmpty()

                             join J1 in dbContext.documento on n.i_IdTipoDocumento equals J1.i_CodigoDocumento into J1_join

                             from J1 in J1_join.DefaultIfEmpty()

                             join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                             from J2 in J2_join.DefaultIfEmpty()

                             join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaUsuario.Value }
                                                            equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                             from J3 in J3_join.DefaultIfEmpty()

                             where n.i_Eliminado == 0 && n.t_FechaEmision >= F_Inicio && n.t_FechaEmision <= F_Fin
                             && B_join.Any(p => p.v_IdCliente == n.v_IdCliente)
                             && n.i_IdEstado != 3 && n.i_IdEstablecimiento == Globals.ClientSession.i_IdEstablecimiento.Value

                             select new pedidoDto
                             {
                                 v_IdPedido = n.v_IdPedido,
                                 v_Periodo = n.v_Periodo,
                                 v_Mes = n.v_Mes,
                                 v_Correlativo = n.v_Correlativo,
                                 NroRegistro = (n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim()).Trim(),
                                 v_SerieDocumento = n.v_SerieDocumento,
                                 v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                 i_IdTipoDocumento = n.i_IdTipoDocumento,
                                 t_FechaEmision = n.t_FechaEmision,
                                 t_FechaVencimiento = n.t_FechaVencimiento,
                                 i_IdEstado = n.i_IdEstado,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                 t_ActualizaFecha = n.t_ActualizaFecha,
                                 v_UsuarioModificacion = J2.v_UserName,
                                 v_UsuarioCreacion = J3.v_UserName,
                                 v_IdCliente = B.v_IdCliente,
                                 d_PrecioVenta = n.d_PrecioVenta,
                                 Documento = (n.v_SerieDocumento.Trim() + "-" + n.v_CorrelativoDocumento.Trim()),
                                 TipoDocumento = J1.v_Siglas,
                                 CodCliente = B.v_CodCliente,
                                 NombreCliente =
                                     n.v_NombreClienteTemporal == ""
                                         ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " +
                                            B.v_SegundoNombre + " " + B.v_RazonSocial).Trim()
                                         : (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " +
                                            n.v_NombreClienteTemporal).Trim(),
                                 i_IdEstablecimiento = n.i_IdEstablecimiento.Value,
                                 Moneda = n.i_IdMoneda == 1 ? "S" : "D",
                                 Total = n.d_PrecioVenta ?? 0,
                                 Origen = "P",
                                 NroDocCliente = B.v_NroDocIdentificacion,
                             });

                var objData = query.ToList();
                var tempPedidos = objData.ToEntities();
                var idsPedidos = objData.Select(p => p.v_IdPedido).Distinct().ToList();
                var nropedidos = tempPedidos.Select(p => p.v_SerieDocumento.Trim() + "-" + p.v_CorrelativoDocumento.Trim()).ToList();
                var tempPedidoDetalles = dbContext.pedidodetalle.Where(p => idsPedidos.Contains(p.v_IdPedido) && p.i_Eliminado == 0).ToList();
                var tempVentas = dbContext.venta.Where(p => nropedidos.Contains(p.v_NroPedido.Replace (" ","").Trim()) && p.i_Eliminado == 0).ToList();
                foreach (var p in objData)
                {
                    p.DespachadoTotalmente = PedidoDespachadoCompletamente(ref pobjOperationResult, p.v_IdPedido, tempPedidos, tempPedidoDetalles, tempVentas);
                }
                pobjOperationResult.Success = 1;
                return objData.Where(p => !p.DespachadoTotalmente).ToList();
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public void EliminarPedido(ref OperationResult pobjOperationResult, string pstrIdPedido,
            List<string> clientSession)
        {
            var objOperationResult = new OperationResult();
            var periodo = Globals.ClientSession.i_Periodo.ToString();
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    var dbContext = new SAMBHSEntitiesModelWin();

                    #region Elimina Cabecera

                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.pedido
                                           where a.v_IdPedido == pstrIdPedido
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                    objEntitySource.i_Eliminado = 1;

                    #endregion

                    #region Elimina Detalles

                    //Eliminar detalles del movimiento eliminado.
                    var objEntitySourceDetallesPedido = (from a in dbContext.pedidodetalle
                                                         where a.v_IdPedido == pstrIdPedido
                                                         select a).ToList();

                    foreach (var PedidoDetalle in objEntitySourceDetallesPedido)
                    {
                        PedidoDetalle.t_ActualizaFecha = DateTime.Now;
                        PedidoDetalle.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                        PedidoDetalle.i_Eliminado = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName,
                            "PedidoDetalle", PedidoDetalle.v_IdPedidoDetalle);

                    }

                    #endregion

                    if (objEntitySource.i_IdTipoDocumento == (int)TiposDocumentos.Pedido)
                    {
                        # region Actualiza-ProductoAlmacen

                        List<separacionproducto> objEntitySeparacionProductos = (from b in dbContext.separacionproducto
                                                                                 where b.v_IdPedido == pstrIdPedido && b.i_Eliminado == 0
                                                                                 select b).ToList();

                        foreach (var listaSeparacionProducto in objEntitySeparacionProductos)
                        {
                            if (!EsServicio(ref objOperationResult, listaSeparacionProducto.v_IdProductoAlmacen))
                            // CUando no es Servicio
                            {

                                productoalmacen objEntityAlmacenProducto = (from c in dbContext.productoalmacen
                                                                            where c.v_IdProductoAlmacen == listaSeparacionProducto.v_IdProductoAlmacen && c.i_Eliminado == 0 && c.v_Periodo == periodo
                                                                            select c).FirstOrDefault();

                                //objEntityAlmacenProducto.d_SeparacionTotal =
                                //    objEntityAlmacenProducto.d_SeparacionTotal -
                                //    listaSeparacionProducto.d_Separacion_Cantidad;
                                if (objEntityAlmacenProducto != null)
                                {
                                    if (listaSeparacionProducto.d_Separacion_CantidadEmpaque != null)
                                    {
                                        if (objEntityAlmacenProducto.d_SeparacionTotal > 0)
                                        {
                                            objEntityAlmacenProducto.d_SeparacionTotal =
                                                objEntityAlmacenProducto.d_SeparacionTotal -
                                                listaSeparacionProducto.d_Separacion_CantidadEmpaque;
                                        }
                                    }
                                    else
                                    {
                                        if (objEntityAlmacenProducto.d_SeparacionTotal > 0)
                                        {
                                            objEntityAlmacenProducto.d_SeparacionTotal =
                                                 objEntityAlmacenProducto.d_SeparacionTotal -
                                                 listaSeparacionProducto.d_Separacion_Cantidad;
                                        }

                                    }


                                    objEntityAlmacenProducto.t_ActualizaFecha = DateTime.Now;
                                    objEntityAlmacenProducto.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                                    dbContext.productoalmacen.ApplyCurrentValues(objEntityAlmacenProducto);
                                }
                            }

                        }


                        #endregion


                        #region Elimina Separación-Producto

                        var objEntitySeparacionProducto = (from b in dbContext.separacionproducto
                                                           where b.v_IdPedido == pstrIdPedido
                                                           select b).ToList();

                        foreach (var separacionProducto in objEntitySeparacionProducto)
                        {
                            if (!EsServicio(ref objOperationResult, separacionProducto.v_IdProductoAlmacen))
                            // CUando no es Servicio
                            {
                                separacionProducto.t_ActualizaFecha = DateTime.Now;
                                separacionProducto.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                                separacionProducto.i_Eliminado = 1;
                            }
                        }

                        #endregion
                    }

                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "Pedido",
                        pstrIdPedido);
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }




        public void EliminarPedidosCaducados(ref OperationResult pobjOperationResult, List<string> ClientSession, DateTime Fecha, int IdAlmacen)
        {
            try
            {

                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        DateTime FechaInicio = Fecha.AddDays(-30);
                        string serie = string.Empty;
                        string correlativo = string.Empty;

                        var Ventas = (from a in dbContext.venta
                                      join b in dbContext.documento on new { td = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join
                                      from b in b_join.DefaultIfEmpty()
                                      where a.i_Eliminado == 0 && a.v_NroPedido.Trim() != string.Empty && a.v_NroPedido.Trim() != null
                                      && a.t_FechaRegistro >= FechaInicio && a.i_IdEstado == 1
                                      select new PedidosVentas
                                      {
                                          TodoNumeroPeedido = a.v_NroPedido.Trim(),
                                          NroFactura = b.v_Siglas + " " + a.v_SerieDocumento.Trim() + "-" + a.v_CorrelativoDocumento.Trim(),
                                      }).ToList().Select(x =>
                                      {


                                          string[] SerieCorrelativo;
                                          SerieCorrelativo = x.TodoNumeroPeedido.Split(new Char[] { '-' });
                                          serie = SerieCorrelativo != null && SerieCorrelativo.Any() ? SerieCorrelativo[0].Trim() : "";
                                          correlativo = SerieCorrelativo != null && SerieCorrelativo.Any() ? SerieCorrelativo[1].Trim() : "";

                                          return new PedidosVentas
                                          {
                                              SeriePedido = serie.Trim(),
                                              CorrelativoPedido = correlativo.Trim(),
                                              NroFactura = x.NroFactura,

                                          };
                                      }).ToList();

                        var PedidosCaducados = (from a in dbContext.pedido
                                                join b in dbContext.pedidodetalle on new { pd = a.v_IdPedido, eliminado = 0 } equals new { pd = b.v_IdPedido, eliminado = b.i_Eliminado.Value } into b_join
                                                from b in b_join.DefaultIfEmpty()
                                                where a.i_IdEstado == 0 && a.t_FechaEmision <= Fecha && a.i_Eliminado == 0 && a.i_IdTipoDocumento == (int)TiposDocumentos.Pedido
                                                && b.i_IdAlmacen == IdAlmacen
                                                select new
                                                    {
                                                        SeriePedido = a.v_SerieDocumento,
                                                        CorrelativoPedido = a.v_CorrelativoDocumento,
                                                        v_IdPedido = a.v_IdPedido
                                                    }).ToList().GroupBy(o => new { IdPedido = o.v_IdPedido }).Select(l => l.FirstOrDefault())

                                                    .Select(l =>
                                                    {
                                                        var Facturas = Ventas.FirstOrDefault(a => a.SeriePedido.Trim() == l.SeriePedido && a.CorrelativoPedido == l.CorrelativoPedido);
                                                        return new
                                                         {
                                                             v_IdPedido = l.v_IdPedido,
                                                             NroFactura = Facturas != null && Facturas.NroFactura != null ? Facturas.NroFactura : "",

                                                         };
                                                    }).ToList();
                        PedidosCaducados = PedidosCaducados.Where(k => string.IsNullOrEmpty(k.NroFactura)).ToList();

                        Globals.ProgressbarStatus.i_TotalProgress = PedidosCaducados.Count;

                        foreach (var pedidosCad in PedidosCaducados)
                        {
                            Globals.ProgressbarStatus.i_Progress++;
                            string IdPedido = pedidosCad.v_IdPedido;
                            EliminarPedido(ref pobjOperationResult, IdPedido, ClientSession);

                        }

                        RecalcularSeparacionProductoAlmacen(ref pobjOperationResult, IdAlmacen, ClientSession, Fecha.Year.ToString());
                        //_objPedidoLBL.RecalcularSeparacionProductoAlmacen(ref objOperationResult, IDAlmacen, Globals.ClientSession.GetAsList(), Globals.ClientSession.i_Periodo.ToString());
                        dbContext.SaveChanges();
                    }
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
            }


        }

        public void RecalcularSeparacionProductoAlmacen(ref OperationResult objOperationResult, int IdAlmacen, List<string> ClientSession, string periodo)
        {

            try
            {
                SecuentialBL objSecuentialBL = new SecuentialBL();
                int intNodeId = 0;
                intNodeId = int.Parse(ClientSession[0]);

                var Separacion = new List<productoalmacen>();
                string serie = string.Empty;
                string correlativo = string.Empty;
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        objOperationResult.Success = 1;

                        #region vacia todos las separacion de producto almacén
                        if (IdAlmacen != -1)
                        {
                            Separacion = dbContext.productoalmacen.Where(p => p.v_Periodo == periodo && p.i_IdAlmacen == IdAlmacen && p.i_Eliminado == 0).ToList();
                        }
                        else
                        {
                            Separacion = dbContext.productoalmacen.Where(p => p.v_Periodo == periodo && p.i_Eliminado == 0).ToList();
                        }
                        Separacion.ForEach(p => p.d_SeparacionTotal = 0);
                        Separacion.ForEach(p => dbContext.productoalmacen.ApplyCurrentValues(p));
                        dbContext.SaveChanges();

                        #endregion

                        #region Obtiene las separaciones de los pedidos

                        List<productoalmacenDto> SeparacionProductosVigentes = new List<productoalmacenDto>();


                        List<string> Ventas = (from a in dbContext.venta
                                               where a.i_Eliminado == 0 && a.v_NroPedido.Trim() != string.Empty && a.v_NroPedido.Trim() != null
                                               && a.v_Periodo == periodo
                                               select a.v_NroPedido.Trim()).ToList();


                        SeparacionProductosVigentes = (from a in dbContext.pedidodetalle

                                                       join e in dbContext.pedido on new { p = a.v_IdPedido, eliminado = 0 } equals new { p = e.v_IdPedido, eliminado = e.i_Eliminado.Value } into e_join
                                                       from e in e_join.DefaultIfEmpty()
                                                       where a.i_Eliminado == 0 && e.i_IdEstado == 0 && e.i_IdTipoDocumento == (int)TiposDocumentos.Pedido
                                                        && (a.i_IdAlmacen == IdAlmacen || IdAlmacen == -1)
                                                        && e.v_Periodo == periodo
                                                       orderby a.v_IdProductoDetalle ascending
                                                       select new productoalmacenDto
                                                       {

                                                           //v_IdProductoAlmacen = d.v_IdProductoAlmacen,
                                                           d_SeparacionTotal = a.d_CantidadEmpaque,
                                                           v_ProductoDetalleId = a.v_IdProductoDetalle,
                                                           i_IdAlmacen = a.i_IdAlmacen ?? -1,
                                                           v_NroPedido = Globals.ClientSession.v_RucEmpresa == Constants.RucChayna ? e.v_SerieDocumento + " - " + e.v_CorrelativoDocumento : e.v_SerieDocumento.Trim() + "-" + e.v_CorrelativoDocumento.Trim(), // a.v_IdPedido,
                                                           v_Periodo = a.v_IdPedidoDetalle,
                                                           v_IdProductoDetalle = e.v_IdPedido,
                                                       }).ToList();

                        SeparacionProductosVigentes = SeparacionProductosVigentes.Where(o => !Ventas.Contains(o.v_NroPedido)).ToList();

                        #endregion
                        #region ActualizaProductoAlmacen
                        productoalmacen ProductoAlmacenFinal = new productoalmacen();
                        Globals.ProgressbarStatus.i_TotalProgress = SeparacionProductosVigentes.Count;

                        if (SeparacionProductosVigentes != null)
                        {
                            foreach (var objEntityDetail in SeparacionProductosVigentes)
                            {

                                var ProductoAlmacens = (from a in dbContext.productoalmacen
                                                        where a.v_ProductoDetalleId == objEntityDetail.v_ProductoDetalleId && a.i_Eliminado == 0 && a.i_IdAlmacen == objEntityDetail.i_IdAlmacen
                                                        && a.v_Periodo == periodo
                                                        select a).FirstOrDefault();

                                if (ProductoAlmacens != null)
                                {
                                    productoalmacenDto objEntityProductoAlmacen = productoalmacenAssembler.ToDTO(ProductoAlmacens);
                                    if (ProductoAlmacens.d_SeparacionTotal == null)
                                    {
                                        ProductoAlmacens.d_SeparacionTotal = 0;
                                    }
                                    var SeparacionFinal = ProductoAlmacens.d_SeparacionTotal + objEntityDetail.d_SeparacionTotal;
                                    if (ProductoAlmacens.d_SeparacionTotal == SeparacionFinal) continue;
                                    ProductoAlmacens.d_SeparacionTotal = SeparacionFinal;
                                    ProductoAlmacens.t_ActualizaFecha = DateTime.Now;

                                    dbContext.productoalmacen.ApplyCurrentValues(ProductoAlmacens);
                                    dbContext.SaveChanges();

                                }

                                Globals.ProgressbarStatus.i_Progress++;
                            }
                        }

                        #endregion
                    }
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
            }



        }

        public void LiberarSeparacion(ref  OperationResult objOperationResult, DateTime FechaInicio, string SeriePedido, string CorrelativoPedido)
        {



            var ProductosFaltantes = ReportePedidosFaltantes(ref objOperationResult, FechaInicio, DateTime.Parse("31/12/" + Globals.ClientSession.i_Periodo.ToString() + " 23:59"), (int)EstadoPedido.Pendientes, (int)Currency.Soles, "-1", "", SeriePedido, CorrelativoPedido, "", "-1", (int)FormatoCantidad.UnidadMedidaProducto);


        }


        public void ActualizarDescuentos(ref OperationResult objOperationResult)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {


                        var queryPedidoDetalles = (from a in dbContext.pedidodetalle

                                                   where a.d_Descuento > 0 && a.v_Descuento == null

                                                   select a).ToList();


                        foreach (var item in queryPedidoDetalles)
                        {

                            pedidodetalle ObjPedidoDetalle = (from a in dbContext.pedidodetalle
                                                              where a.v_IdPedidoDetalle == item.v_IdPedidoDetalle
                                                              select a).FirstOrDefault();

                            ObjPedidoDetalle.v_Descuento = ObjPedidoDetalle.d_Descuento.ToString();



                            string DescuentoFormateado = Utils.Windows.DarFormatoDescuentoUnDecimal(ObjPedidoDetalle.v_Descuento);
                            var DescuentosSucesivos = Utils.Windows.CalcularDescuentosSucesivosDecimales(DescuentoFormateado, ObjPedidoDetalle.d_Valor.Value);
                            ObjPedidoDetalle.d_Descuento = DescuentosSucesivos;
                            dbContext.pedidodetalle.ApplyCurrentValues(ObjPedidoDetalle);



                        }





                        var queryVentaDetalles = (from a in dbContext.ventadetalle
                                                  where a.d_Descuento > 0 && a.v_FacturaRef == null

                                                  select a).ToList();


                        foreach (var item in queryVentaDetalles)
                        {

                            ventadetalle ObjVentaDetalle = (from a in dbContext.ventadetalle
                                                            where a.v_IdVentaDetalle == item.v_IdVentaDetalle
                                                            select a).FirstOrDefault();

                            ObjVentaDetalle.v_FacturaRef = ObjVentaDetalle.d_Descuento.ToString();



                            string DescuentoFormateado = Utils.Windows.DarFormatoDescuentoUnDecimal(ObjVentaDetalle.v_FacturaRef);
                            var DescuentosSucesivos = Utils.Windows.CalcularDescuentosSucesivosDecimales(DescuentoFormateado, ObjVentaDetalle.d_Valor.Value);
                            ObjVentaDetalle.d_Descuento = DescuentosSucesivos;
                            dbContext.ventadetalle.ApplyCurrentValues(ObjVentaDetalle);

                        }
                        dbContext.SaveChanges();
                        ts.Complete();
                        objOperationResult.Success = 1;


                    }
                }
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "PedidoBL.ActualizarNulosSeparacionProducto()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
            }



        }


        public void ActualizarNulosSeparacionProducto(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {

                        var NulosSeparacion = (from a in dbContext.separacionproducto

                                               join b in dbContext.pedidodetalle on new { Id = a.v_IdPedido } equals new { Id = b.v_IdPedido } into b_join

                                               from b in b_join.DefaultIfEmpty()

                                               where a.d_Separacion_CantidadEmpaque == null

                                               select new
                                               {

                                                   CantidadEmpaquePedido = b.d_CantidadEmpaque,
                                                   CantidadEmpaqueSeparacion = a.d_Separacion_CantidadEmpaque,
                                                   IdSeparacionProducto = a.v_IdSeparacionProducto,
                                                   IdPedido = b.v_IdPedido,

                                               }).ToList();

                        foreach (var item in NulosSeparacion)
                        {
                            if (item.CantidadEmpaqueSeparacion == null)
                            {

                                var Obj = (from a in dbContext.separacionproducto

                                           where a.v_IdSeparacionProducto == item.IdSeparacionProducto

                                           select a).FirstOrDefault();


                                if (Obj != null)
                                {
                                    separacionproductoDto _Dto = separacionproductoAssembler.ToDTO(Obj);

                                    separacionproducto _objEntity = separacionproductoAssembler.ToEntity(_Dto);

                                    _objEntity.d_Separacion_CantidadEmpaque = item.CantidadEmpaquePedido;
                                    dbContext.separacionproducto.ApplyCurrentValues(_objEntity);

                                }



                            }

                        }

                        dbContext.SaveChanges();
                        ts.Complete();
                        pobjOperationResult.Success = 1;




                    }

                }
            }
            catch (Exception ex)
            {

                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PedidoBL.ActualizarNulosSeparacionProducto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);


            }



        }
        #endregion

        #region Bandeja-ConsultaPedidos

        public List<pedidoDto> ListarPedidosPendientes(ref OperationResult pobjOperationResult,
            string pstrSortExpression, string pstrFilterExpression, DateTime F_Inicio, DateTime F_Fin)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var query = (from n in dbContext.pedido

                             join B in dbContext.cliente on n.v_IdCliente equals B.v_IdCliente into B_join
                             from B in B_join.DefaultIfEmpty()

                             join J1 in dbContext.documento on n.i_IdTipoDocumento equals J1.i_CodigoDocumento into J1_join

                             from J1 in J1_join.DefaultIfEmpty()


                             join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                 equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                             from J2 in J2_join.DefaultIfEmpty()

                             join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaUsuario.Value }
                                 equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                             from J3 in J3_join.DefaultIfEmpty()

                             join J4 in dbContext.vendedor on n.v_IdVendedor equals J4.v_IdVendedor into J4_join

                             from J4 in J4_join.DefaultIfEmpty()
                             where
                                 n.i_Eliminado == 0 && n.t_FechaEmision >= F_Inicio && n.t_FechaEmision <= F_Fin &&
                                 //n.i_IdEstado == 0 && n.i_IdTipoDocumento == 430

                                  n.i_IdEstado == 0 && n.i_IdTipoDocumento == (int)TiposDocumentos.Pedido


                             select new pedidoDto
                             {
                                 v_IdPedido = n.v_IdPedido,
                                 v_IdCliente = n.v_IdCliente,
                                 v_Periodo = n.v_Periodo,
                                 v_Mes = n.v_Mes,
                                 v_Correlativo = n.v_Correlativo,
                                 NroRegistro = (n.v_Mes.Trim() + "-" + n.v_Correlativo.Trim()).Trim(),
                                 v_SerieDocumento = n.v_SerieDocumento,
                                 v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                 i_IdTipoDocumento = n.i_IdTipoDocumento,
                                 t_FechaEmision = n.t_FechaEmision,
                                 t_FechaVencimiento = n.t_FechaVencimiento,
                                 i_IdEstado = n.i_IdEstado,
                                 t_InsertaFecha = n.t_InsertaFecha,
                                 i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                 t_ActualizaFecha = n.t_ActualizaFecha,
                                 v_UsuarioModificacion = J2.v_UserName,
                                 v_UsuarioCreacion = J3.v_UserName,
                                 d_PrecioVenta = n.d_PrecioVenta,
                                 Documento = (n.v_SerieDocumento.Trim() + "-" + n.v_CorrelativoDocumento.Trim()),
                                 TipoDocumento = J1.v_Siglas,
                                 CodCliente = B.v_CodCliente,
                                 NumeroPedido = n.v_SerieDocumento + "-" + n.v_CorrelativoDocumento,
                                 Vendedor = J4.v_NombreCompleto,
                                 CodigoCliente = B.v_CodCliente,
                                 Cliente =
                                     n.v_NombreClienteTemporal == ""
                                         ? (B.v_ApePaterno.Trim() + " " + B.v_ApeMaterno.Trim() + " " + B.v_PrimerNombre.Trim() +
                                            " " + B.v_SegundoNombre.Trim() + " " + B.v_RazonSocial.Trim()).Trim()
                                         : n.v_NombreClienteTemporal,

                             });
                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                List<pedidoDto> objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }


        }

        #endregion

        #region REPORTE

        public List<ReporteDocumentoPedido> ReporteDocumentoPedido(ref  OperationResult pobjOperationResult, string pstrv_IdPedido, string pstrv_IdVendedor)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                List<ReporteDocumentoPedido> ReporteFinal = new List<ReporteDocumentoPedido>();
                #region Query

                var query =
                    (from A in dbContext.pedido

                     join B in dbContext.cliente on new { IdCliente = A.v_IdCliente, eliminado = 0 } equals new { IdCliente = B.v_IdCliente, eliminado = B.i_Eliminado.Value } into B_join
                     from B in B_join.DefaultIfEmpty()

                     join C in dbContext.documento on new { IdTipoDocumento = A.i_IdTipoDocumento.Value, eliminado = 0 } equals new { IdTipoDocumento = C.i_CodigoDocumento, eliminado = C.i_Eliminado.Value } into C_join
                     from C in C_join.DefaultIfEmpty()

                     join D in dbContext.vendedor on new { IdVendedor = A.v_IdVendedor, eliminado = 0 } equals new { IdVendedor = D.v_IdVendedor, eliminado = D.i_Eliminado.Value } into D_join
                     from D in D_join.DefaultIfEmpty()

                     join E in dbContext.datahierarchy on new { IdCondicionPago = A.i_IdCondicionPago.Value, eliminado = 0, Grupo = 41 } equals new { IdCondicionPago = E.i_ItemId, eliminado = E.i_IsDeleted.Value, Grupo = E.i_GroupId } into E_join
                     from E in E_join.DefaultIfEmpty()

                     join F in dbContext.pedidodetalle on new { IdPedido = A.v_IdPedido, eliminado = 0 } equals new { IdPedido = F.v_IdPedido, eliminado = F.i_Eliminado.Value } into F_join

                     from F in F_join.DefaultIfEmpty()

                     join G in dbContext.productodetalle on new { IdProductoDetalle = F.v_IdProductoDetalle, eliminado = 0 } equals new { IdProductoDetalle = G.v_IdProductoDetalle, eliminado = G.i_Eliminado.Value } into G_join
                     from G in G_join.DefaultIfEmpty()

                     join H in dbContext.producto on new { IdProducto = G.v_IdProducto, eliminado = 0 } equals new { IdProducto = H.v_IdProducto, eliminado = H.i_Eliminado.Value } into H_join
                     from H in H_join.DefaultIfEmpty()

                     join I in dbContext.datahierarchy on new { IdUnidad = F.i_IdUnidadMedida.Value, Grupo = 17, eliminado = 0 } equals new { IdUnidad = I.i_ItemId, Grupo = I.i_GroupId, eliminado = I.i_IsDeleted.Value } into I_join
                     from I in I_join.DefaultIfEmpty()

                     join J in dbContext.datahierarchy on new { IdMoneda = A.i_IdMoneda.Value, Grupo = 18, eliminado = 0 } equals new { IdMoneda = J.i_ItemId, Grupo = J.i_GroupId, eliminado = J.i_IsDeleted.Value } into J_join

                     from J in J_join.DefaultIfEmpty()

                     join K in dbContext.datahierarchy on new { Igv = A.i_IdIgv.Value, Grupo = 27, eliminado = 0 } equals new { Igv = K.i_ItemId, Grupo = K.i_GroupId, eliminado = K.i_IsDeleted.Value } into K_join

                     from K in K_join.DefaultIfEmpty()

                     where A.v_IdPedido == pstrv_IdPedido && A.i_Eliminado == 0

                     select new
                     {
                         Documento = (A.v_SerieDocumento.Trim() + " - " + A.v_CorrelativoDocumento.Trim()),
                         TipoDocumento = C.v_Siglas,
                         NombreCliente =
                             A.v_NombreClienteTemporal == ""
                                 ? (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " +
                                    B.v_RazonSocial).Trim()
                                 : (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " +
                                    A.v_NombreClienteTemporal).Trim(),
                         Glosa = A.v_Glosa,
                         t_FechaEmision = A.t_FechaEmision.Value,
                         RucCliente = B.v_NroDocIdentificacion,
                         Vendedor = D.v_NombreCompleto,
                         CondicionPago = E.v_Value1,
                         Descripcion = F.v_NombreProducto,
                         Unidad = I.v_Value1,
                         CodigoArticulo = H.v_CodInterno,
                         Cantidad = F.d_Cantidad.Value,
                         PrecioUnitario = F.d_PrecioUnitario.Value,
                         Valor = F.d_PrecioVenta.Value,
                         Direccion = string.IsNullOrEmpty(A.v_DireccionClienteTemporal) ? B.v_DirecPrincipal : A.v_DireccionClienteTemporal,
                         Moneda = J.v_Value1.Trim(),
                         MonedaSiglas = A.i_IdMoneda == (int)Currency.Soles ? "S/" : "US$.",
                         d_PrecioVenta = A.d_PrecioVenta ?? 0,
                         i_idMoneda = A.i_IdMoneda.Value,
                         i_NroDias = A.i_DiasVigencia.Value,
                         Igv = K.v_Value1 == null ? "" : K.v_Value1,
                         i_IdTipoDocumento = A.i_IdTipoDocumento.Value,
                         v_SerieDocumento = A.v_SerieDocumento.Trim(),
                         TelefonoCliente = B.v_TelefonoFijo + " " + B.v_TelefonoMovil,
                         UnidadSiglas = I.v_Field ?? "",
                         v_Descuento = F.v_Descuento ?? "",
                         ValorVenta = A.d_VVenta ?? 0,
                         IgvVenta = A.d_Igv ?? 0,
                         ValorVentaDetalle = F.d_ValorVenta.Value,
                         v_IdPedidoDetalle = F.v_IdPedidoDetalle,


                     }).ToList().Select(A => new ReporteDocumentoPedido
                              {
                                  Documento = A.Documento,
                                  TipoDocumento = A.TipoDocumento,
                                  NombreCliente = A.NombreCliente,
                                  Glosa = A.Glosa,
                                  t_FechaEmision = A.t_FechaEmision,
                                  RucCliente = A.RucCliente,
                                  Vendedor = A.Vendedor,
                                  CondicionPago = A.CondicionPago,
                                  Descripcion = A.Descripcion,
                                  Unidad = A.Unidad,
                                  CodigoArticulo = A.CodigoArticulo,
                                  Cantidad = A.Cantidad,
                                  PrecioUnitario = A.PrecioUnitario,
                                  Valor = A.Valor,
                                  Direccion = A.Direccion,
                                  Moneda = A.Moneda,
                                  MonedaSiglas = A.MonedaSiglas,
                                  d_PrecioVenta = A.d_PrecioVenta,
                                  i_idMoneda = A.i_idMoneda,
                                  StrNroDias = A.i_NroDias + " Dias ",
                                  Igv = A.Igv + " %",
                                  i_IdTipoDocumento = A.i_IdTipoDocumento,
                                  v_SerieDocumento = A.v_SerieDocumento.Trim(),
                                  TelefonoCliente = A.TelefonoCliente == null ? "" : A.TelefonoCliente.Trim(),
                                  UnidadSiglas = A == null ? "" : A.UnidadSiglas,
                                  v_Descuento = A.v_Descuento,
                                  ValorVenta = A.ValorVenta,
                                  IgvVenta = A.IgvVenta,
                                  ValorVentaDetalle = A.ValorVentaDetalle,
                                  DescripcionUnidadMedida = A.UnidadSiglas + "     " + A.Descripcion.Trim(),
                                  v_IdPedidoDetalle = A.v_IdPedidoDetalle,

                              }).ToList().OrderBy(o => o.v_IdPedidoDetalle).ToList();


                #endregion
                int i = 0;
                foreach (var item in query)
                {

                    item.Item = (i + 1).ToString();
                    i = i + 1;
                    ReporteFinal.Add(item);
                }


                pobjOperationResult.Success = 1;
                return ReporteFinal.ToList(); ;




            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                //pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        private string DevolverNombreEmpresaPropietaria_()
        {
            OperationResult objOperationResult = new OperationResult();

            NodeBL objNodeBL = new NodeBL();
            int _intNodeId = int.Parse(Utils.Windows.GetApplicationConfigValue("NodeId"));

            var x = objNodeBL.GetNodeByNodeId(ref objOperationResult, _intNodeId);

            return x.v_RazonSocial;

        }

        public List<string> DevolverNombreEmpresaPropietaria()
        {
            OperationResult objOperationResult = new OperationResult();
            List<string> Retonar = new List<string>();
            NodeBL objNodeBL = new NodeBL();
            int _intNodeId = int.Parse(Utils.Windows.GetApplicationConfigValue("NodeId"));

            var x = objNodeBL.GetNodeByNodeId(ref objOperationResult, _intNodeId);
            Retonar.Add(x.v_RazonSocial);
            Retonar.Add(x.v_RUC);
            return Retonar;

        }

        public List<ReporteRegistroPedido> ReporteRegistroPedido(string pstrv_Periodo, int pstri_IdEstablecimiento,
            DateTime? pstrt_FechaRegistroIni, DateTime? pstrt_FechaRegistroFin, int pintTipoDocumentoId,
            string pstrt_IdVendedor, string pstrt_Orden)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                #region Query

                var query =
                    (from A in dbContext.pedido
                     join B in dbContext.cliente on A.v_IdCliente equals B.v_IdCliente
                     join C in dbContext.documento on A.i_IdTipoDocumento equals C.i_CodigoDocumento
                     join D in dbContext.vendedor on A.v_IdVendedor equals D.v_IdVendedor
                     join E in dbContext.documento on A.i_IdTipoDocumento equals E.i_CodigoDocumento
                     join F in dbContext.datahierarchy on A.i_IdMoneda equals F.i_ItemId
                     join G in dbContext.datahierarchy on A.i_IdIgv equals G.i_ItemId
                     where
                         A.i_Eliminado == 0 && B.i_Eliminado == 0 && D.i_Eliminado == 0 && E.i_Eliminado == 0 &&
                         (F.i_GroupId == 18 && F.i_IsDeleted == 0) && (G.i_GroupId == 27 && G.i_IsDeleted == 0)
                         && A.v_Periodo == pstrv_Periodo && (A.t_FechaEmision >= pstrt_FechaRegistroIni
                                                             && A.t_FechaEmision <= pstrt_FechaRegistroFin)
                         && (A.i_IdTipoDocumento == pintTipoDocumentoId || pintTipoDocumentoId == -1)
                         && (A.v_IdVendedor == pstrt_IdVendedor || pstrt_IdVendedor == "-1")

                     select new ReporteRegistroPedido
                     {
                         NombreEmpresaPropietaria = "",
                         NombreAlmacen = "",
                         FechaRegistro = A.t_FechaEmision.Value,
                         TipoDocumento = C.v_Siglas,
                         NombreDocumento = (A.v_SerieDocumento.Trim() + "-" + A.v_CorrelativoDocumento.Trim()),
                         NombreCliente =
                             A.v_NombreClienteTemporal == ""
                                 ? (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " +
                                    B.v_RazonSocial).Trim()
                                 : (B.v_PrimerNombre + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " +
                                    A.v_NombreClienteTemporal).Trim(),
                         NroDocCliente = B.v_NroDocIdentificacion,
                         NombreVendedor = D.v_NombreCompleto,
                         NroDocVendedor = D.v_NroDocIdentificacion,
                         Moneda = F.v_Value2,
                         ValorVenta = A.d_VVenta.Value,
                         Igv = A.d_Igv.Value,
                         Total = A.d_PrecioVenta.Value,
                         Descuento = A.d_Descuento.Value,
                         DocumentoRef = A.v_SerieDocumento.Trim() + "-" + A.v_CorrelativoDocumento.Trim(),
                         FechaRegistroRef = A.t_FechaVencimiento.Value,
                         TipoDocumentoRef = C.v_Siglas,
                         SerieDocumento = A.v_SerieDocumento,
                         CorrelativoDocumento = A.v_CorrelativoDocumento,
                         IdTipoDocumento = A.i_IdTipoDocumento.Value,
                         IdVendedor = A.v_IdVendedor,
                         TipoCambio = A.d_TipoCambio.Value,
                         IdMoneda = A.i_IdMoneda.Value,
                         IgvNombre = "IGV Al " + G.v_Value1 + " :",
                         Documento = "TOTAL: " + C.v_Nombre + " :",
                         RucEmpresaPropietaria = "",

                     });

                switch (pstrt_Orden)
                {
                    case "CorrelativoDocumento":
                        query = query.OrderBy("CorrelativoDocumento ASC");
                        break;

                    case "FechaRegistro":
                        query = query.OrderBy("FechaRegistro ASC");
                        break;
                    case "TipoDocumento":
                        query = query.OrderBy("TipoDocumento ASC");
                        break;

                }
                var query1 = (from A in query.ToList()
                              let ValorVenta =
                                  CalcularVenta(A.IdTipoDocumento, A.ValorVenta, A.Igv, A.Total, A.Descuento, A.TipoCambio,
                                      A.IdMoneda)

                              select new ReporteRegistroPedido
                              {
                                  NombreEmpresaPropietaria = DevolverNombreEmpresaPropietaria()[0],
                                  NombreAlmacen = "",
                                  FechaRegistro = A.FechaRegistro,
                                  TipoDocumento = A.TipoDocumento,
                                  NombreDocumento = A.NombreDocumento,
                                  NombreCliente = A.NombreCliente,
                                  NroDocCliente = A.NroDocCliente,
                                  NombreVendedor = A.NombreVendedor,
                                  NroDocVendedor = A.NroDocVendedor,
                                  Moneda = A.Moneda,
                                  ValorVenta = ValorVenta.ValorVentaSoles,
                                  Igv = ValorVenta.IgvSoles,
                                  Total = ValorVenta.TotalSoles,
                                  Descuento = A.Descuento,
                                  DocumentoRef = A.DocumentoRef,
                                  FechaRegistroRef = A.FechaRegistroRef,
                                  TipoDocumentoRef = A.TipoDocumentoRef,
                                  SerieDocumento = A.SerieDocumento,
                                  CorrelativoDocumento = A.CorrelativoDocumento,
                                  IdTipoDocumento = A.IdTipoDocumento,
                                  IdVendedor = A.IdVendedor,
                                  TipoCambio = A.TipoCambio,
                                  ValorVentaD = ValorVenta.ValorVentaDolares,
                                  IgvD = ValorVenta.IgvDolares,
                                  TotalD = ValorVenta.TotalDolares,
                                  IdMoneda = A.IdMoneda,
                                  IgvNombre = A.IgvNombre,
                                  Documento = A.Documento,
                                  RucEmpresaPropietaria = DevolverNombreEmpresaPropietaria()[1],
                              });

                #endregion


                return query1.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public List<ReporteRegistroPedidoPendientes> ReportePedidosFaltantes(ref  OperationResult objOperationResult, DateTime FechaInicio, DateTime FechaFin, int EstPedido, int MonedaReporte, string IdVendedor, string IdCliente, string seriePedido, string CorrelativoPedido, string CodigoProducto, string Grupo, int FormatCantidad)
        {
            try
            {
                List<ReporteRegistroPedidoPendientes> ListaPedidosFaltantes = new List<ReporteRegistroPedidoPendientes>();
                List<PedidosVentas> Ventas = new List<PedidosVentas>();
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    objOperationResult.Success = 1;
                    string serie = string.Empty;
                    string correlativo = string.Empty;
                    string NroPedido = !string.IsNullOrEmpty(seriePedido) && !string.IsNullOrEmpty(CorrelativoPedido) ? seriePedido + "-" + CorrelativoPedido : "";

                    if (string.IsNullOrEmpty(NroPedido))
                    {
                        Ventas = (from z in dbContext.ventadetalle
                                  join a in dbContext.venta on new { v = z.v_IdVenta, eliminado = 0 } equals new { v = a.v_IdVenta, eliminado = a.i_Eliminado.Value } into a_join
                                  from a in a_join.DefaultIfEmpty()
                                  join b in dbContext.documento on new { td = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join
                                  from b in b_join.DefaultIfEmpty()
                                  join c in dbContext.datahierarchy on new { umGrupo = 17, eliminado = 0, umItem = z.i_IdUnidadMedida.Value } equals new { umGrupo = c.i_GroupId, eliminado = c.i_IsDeleted.Value, umItem = c.i_ItemId } into c_join
                                  from c in c_join.DefaultIfEmpty()
                                  where a.i_Eliminado == 0 && a.v_NroPedido.Trim() != string.Empty && a.v_NroPedido.Trim() != null
                                   && a.i_IdEstado == 1 && z.i_Eliminado == 0 && z.i_IdAlmacen == Globals.ClientSession.i_IdAlmacenPredeterminado.Value


                                  select new PedidosVentas
                                  {
                                      TodoNumeroPeedido = a.v_NroPedido.Trim(),
                                      NroFactura = a.v_SerieDocumento.Trim() + "-" + a.v_CorrelativoDocumento.Trim(),
                                      TipoDocumento = b != null ? b.v_Siglas : "",
                                      FechaEmisionDocVenta = a.t_FechaRegistro.Value,
                                      ProductoDetalle = z != null ? z.v_IdProductoDetalle : "",
                                      CantidadVendida = z.d_CantidadEmpaque.Value,
                                      //UnidadMedidaVenta = c.v_Value2 != "1" ? "UND" : c.v_Field,
                                      UnidadMedidaVenta = FormatCantidad == (int)FormatoCantidad.Unidades ? c.v_Value2 != "1" ? "UND" : c.v_Field : c.v_Field,
                                      UM = c != null ? c.v_Value2 : "1",
                                      i_IdTipoDocumentoReferencia = a.i_IdTipoDocumentoRef ?? -1,
                                      v_NumeroDocumentoReferencia = a.v_SerieDocumentoRef.Trim() + "-" + a.v_CorrelativoDocumentoRef.Trim(),
                                      i_IdTipoDocumento = a.i_IdTipoDocumento ?? -1,

                                  }).ToList().Select(x =>
                                      {
                                          string[] SerieCorrelativo;
                                          SerieCorrelativo = x.TodoNumeroPeedido.Split(new Char[] { '-' });
                                          if (SerieCorrelativo.Count() == 2)
                                          {
                                              serie = SerieCorrelativo[0].Trim();
                                              correlativo = SerieCorrelativo[1].Trim();
                                          }

                                          return new PedidosVentas
                                          {
                                              SeriePedido = serie.Trim(),
                                              CorrelativoPedido = correlativo.Trim(),
                                              NroFactura = x.NroFactura,
                                              TipoDocumento = x.TipoDocumento,
                                              FechaEmisionDocVenta = x.FechaEmisionDocVenta,
                                              ProductoDetalle = x.ProductoDetalle,
                                              CantidadVendida = FormatCantidad == (int)FormatoCantidad.Unidades ? x.CantidadVendida : decimal.Parse(x.UM) == 0 ? 0 : x.CantidadVendida / decimal.Parse(x.UM),
                                              //UnidadMedidaVenta = x.UnidadMedidaVenta,
                                              i_IdTipoDocumentoReferencia = x.i_IdTipoDocumentoReferencia,
                                              v_NumeroDocumentoReferencia = x.v_NumeroDocumentoReferencia,
                                              i_IdTipoDocumento = x.i_IdTipoDocumento,
                                          };

                                      }).ToList();

                        ListaPedidosFaltantes = (from a in dbContext.pedidodetalle

                                                 join b in dbContext.productodetalle on new { pd = a.v_IdProductoDetalle, eliminado = 0 } equals new { pd = b.v_IdProductoDetalle, eliminado = b.i_Eliminado.Value } into b_join
                                                 from b in b_join.DefaultIfEmpty()

                                                 join n in dbContext.pedido on new { pedido = a.v_IdPedido, eliminado = 0 } equals new { pedido = n.v_IdPedido, eliminado = n.i_Eliminado.Value } into n_join
                                                 from n in n_join.DefaultIfEmpty()

                                                 join B in dbContext.cliente on n.v_IdCliente equals B.v_IdCliente into B_join
                                                 from B in B_join.DefaultIfEmpty()

                                                 join J1 in dbContext.documento on n.i_IdTipoDocumento equals J1.i_CodigoDocumento into J1_join

                                                 from J1 in J1_join.DefaultIfEmpty()

                                                 //join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                 //                               equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                                 //from J2 in J2_join.DefaultIfEmpty()

                                                 //join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaUsuario.Value }
                                                 //                               equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                                                 //from J3 in J3_join.DefaultIfEmpty()

                                                 join J4 in dbContext.datahierarchy on new { CondicionPago = 41, eliminado = 0, Item = n.i_IdCondicionPago.Value }
                                                                                    equals new { CondicionPago = J4.i_GroupId, eliminado = J4.i_IsDeleted.Value, Item = J4.i_ItemId } into J4_join
                                                 from J4 in J4_join.DefaultIfEmpty()
                                                 join J5 in dbContext.vendedor on new { vendedor = n.v_IdVendedor, eliminado = 0 } equals new { vendedor = J5.v_IdVendedor, eliminado = J5.i_Eliminado.Value } into J5_join

                                                 from J5 in J5_join.DefaultIfEmpty()

                                                 join J6 in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, umItem = a.i_IdUnidadMedida.Value } equals new { Grupo = J6.i_GroupId, eliminado = J6.i_IsDeleted.Value, umItem = J6.i_ItemId } into J6_join
                                                 from J6 in J6_join.DefaultIfEmpty()

                                                 join J7 in dbContext.clientedirecciones on new { cliente = B.v_IdCliente, eliminado = B.i_Eliminado.Value, Direcc = n.i_IdDireccionCliente.Value } equals new { cliente = J7.v_IdCliente, eliminado = 0, Direcc = J7.i_IdDireccionCliente } into J7_join
                                                 from J7 in J7_join.DefaultIfEmpty()


                                                 join J8 in dbContext.datahierarchy on new { Grupo = 161, eliminado = 0, Zona = J7.i_IdZona.Value } equals new { Grupo = J8.i_GroupId, eliminado = J8.i_IsDeleted.Value, Zona = J8.i_ItemId } into J8_join
                                                 from J8 in J8_join.DefaultIfEmpty()

                                                 where a.i_Eliminado == 0 && n.t_FechaEmision >= FechaInicio && n.t_FechaEmision <= FechaFin
                                                 && (n.v_IdVendedor == IdVendedor || IdVendedor == "-1")
                                                 && (n.v_IdCliente == IdCliente || IdCliente == "")

                                                 && n.i_IdEstado != 3 && (b.producto.v_CodInterno == CodigoProducto || CodigoProducto == "")

                                                 && n.i_IdTipoDocumento == (int)TiposDocumentos.Pedido

                                                 && a.i_IdAlmacen == Globals.ClientSession.i_IdAlmacenPredeterminado.Value
                                                 select new
                                                  {
                                                      NroPedido = J1.v_Siglas + " " + n.v_SerieDocumento + " " + n.v_CorrelativoDocumento,
                                                      CondicionPago = J4.v_Value1,
                                                      FechaPedido = n.t_FechaEmision.Value,
                                                      Producto = b.producto != null ? b.producto.v_CodInterno + "  -  " + b.producto.v_Descripcion : "PRODUCTO NO EXISTE",
                                                      Cliente = B != null ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim() : "",
                                                      CantidadPedido = a.d_CantidadEmpaque.Value,
                                                      PrecioUnitario = MonedaReporte == n.i_IdMoneda ? a.d_PrecioUnitario : MonedaReporte == (int)Currency.Soles ? a.d_PrecioUnitario * n.d_TipoCambio : a.d_PrecioUnitario / n.d_TipoCambio,
                                                      Total = MonedaReporte == n.i_IdMoneda ? a.d_PrecioVenta : MonedaReporte == (int)Currency.Soles ? a.d_PrecioVenta * n.d_TipoCambio : a.d_PrecioVenta / n.d_TipoCambio,
                                                      SeriePedido = n.v_SerieDocumento.Trim(),
                                                      CorrelativoPedido = n.v_CorrelativoDocumento.Trim(),
                                                      Vendedor = J5 == null ? "VENDEDOR NO EXISTE" : "VENDEDOR : " + J5.v_NombreCompleto,
                                                      ProductoDetalle = a.v_IdProductoDetalle,
                                                      MonedaOp = n.i_IdMoneda == (int)Currency.Soles ? "S/." : "US$.",
                                                      IdProductoDetalle = a.v_IdProductoDetalle,
                                                      UnidadMedida = FormatCantidad == (int)FormatoCantidad.Unidades ? J6.v_Value2 != "1" ? "UND" : J6.v_Field : J6.v_Field,
                                                      Zona = J8 != null ? J8.v_Value1 : "",
                                                      UM = J6.v_Value2,
                                                      v_IdPedidoDetalle = a.v_IdPedidoDetalle,
                                                  }

                                         ).ToList().Select(l =>
                                             {
                                                 return new ReporteRegistroPedidoPendientes
                                              {

                                                  NroPedido = l.NroPedido,
                                                  CondicionPago = l.CondicionPago,
                                                  FechaPedido = l.FechaPedido.ToShortDateString(),
                                                  Producto = l.Producto,
                                                  Cliente = l.Cliente,
                                                  CantidadPedido = FormatCantidad == (int)FormatoCantidad.Unidades ? l.CantidadPedido : decimal.Parse(l.UM) == 0 ? 0 : l.CantidadPedido / decimal.Parse(l.UM),
                                                  PrecioUnitario = l.PrecioUnitario.Value,
                                                  MonedaOp = l.MonedaOp,
                                                  Total = l.Total.Value,
                                                  SeriePedido = l.SeriePedido,
                                                  CorrelativoPedido = l.CorrelativoPedido,
                                                  ProductoDetalle = l.IdProductoDetalle,
                                                  UnidadMedidaPedido = l.UnidadMedida,
                                                  //Grupo =    l.Vendedor,
                                                  Grupo = Grupo == "Vendedor" ? l.Vendedor : Grupo == "NroPedido" ? l.NroPedido : Grupo == "Cliente" ? l.Cliente + " - ZONA : " + l.Zona : Grupo == "Producto" ? "PRODUCTO :" + l.Producto : "",
                                                  ZonaCliente = l.Zona,
                                                  v_IdPedidoDetalle = l.v_IdPedidoDetalle,

                                              };

                                             }).ToList();
                    }
                    else
                    {

                        Ventas = (from z in dbContext.ventadetalle
                                  join a in dbContext.venta on new { v = z.v_IdVenta, eliminado = 0 } equals new { v = a.v_IdVenta, eliminado = a.i_Eliminado.Value } into a_join
                                  from a in a_join.DefaultIfEmpty()
                                  join b in dbContext.documento on new { td = a.i_IdTipoDocumento.Value, eliminado = 0 } equals new { td = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value } into b_join
                                  from b in b_join.DefaultIfEmpty()
                                  join c in dbContext.datahierarchy on new { umGrupo = 17, eliminado = 0, umItem = z.i_IdUnidadMedida.Value } equals new { umGrupo = c.i_GroupId, eliminado = c.i_IsDeleted.Value, umItem = c.i_ItemId } into c_join
                                  from c in c_join.DefaultIfEmpty()
                                  where a.i_Eliminado == 0 && a.v_NroPedido.Trim() != string.Empty && a.v_NroPedido.Trim() != null
                                   && a.i_IdEstado == 1 && z.i_Eliminado == 0
                                   && a.v_NroPedido == NroPedido

                                   && z.i_IdAlmacen == Globals.ClientSession.i_IdAlmacenPredeterminado.Value
                                  select new PedidosVentas
                                  {
                                      TodoNumeroPeedido = a.v_NroPedido.Trim(),
                                      NroFactura = a.v_SerieDocumento.Trim() + "-" + a.v_CorrelativoDocumento.Trim(),
                                      TipoDocumento = b != null ? b.v_Siglas : "",
                                      FechaEmisionDocVenta = a.t_FechaRegistro.Value,
                                      ProductoDetalle = z != null ? z.v_IdProductoDetalle : "",
                                      CantidadVendida = z.d_CantidadEmpaque.Value,
                                      //UnidadMedidaVenta = c.v_Value2 != "1" ? "UND" : c.v_Field,
                                      UnidadMedidaVenta = FormatCantidad == (int)FormatoCantidad.Unidades ? c.v_Value2 != "1" ? "UND" : c.v_Field : c.v_Field,
                                      i_IdTipoDocumento = a.i_IdTipoDocumento ?? -1,
                                      UM = c != null ? c.v_Value2 : "1",


                                  }).ToList().Select(x =>
                                   {


                                       string[] SerieCorrelativo;
                                       SerieCorrelativo = x.TodoNumeroPeedido.Split(new Char[] { '-' });
                                       serie = SerieCorrelativo[0].Trim();
                                       correlativo = SerieCorrelativo[1].Trim();

                                       return new PedidosVentas
                                       {
                                           SeriePedido = serie.Trim(),
                                           CorrelativoPedido = correlativo.Trim(),
                                           NroFactura = x.NroFactura,
                                           TipoDocumento = x.TipoDocumento,
                                           FechaEmisionDocVenta = x.FechaEmisionDocVenta,
                                           ProductoDetalle = x.ProductoDetalle,
                                           CantidadVendida = FormatCantidad == (int)FormatoCantidad.Unidades ? x.CantidadVendida : decimal.Parse(x.UM) == 0 ? 0 : x.CantidadVendida / decimal.Parse(x.UM),
                                           UnidadMedidaVenta = x.UnidadMedidaVenta,
                                           i_IdTipoDocumento = x.i_IdTipoDocumento,
                                           i_IdTipoDocumentoReferencia = x.i_IdTipoDocumentoReferencia,
                                           v_NumeroDocumentoReferencia = x.v_NumeroDocumentoReferencia,
                                       };

                                   }).ToList();

                        ListaPedidosFaltantes = (from a in dbContext.pedidodetalle

                                                 join b in dbContext.productodetalle on new { pd = a.v_IdProductoDetalle, eliminado = 0 } equals new { pd = b.v_IdProductoDetalle, eliminado = b.i_Eliminado.Value } into b_join
                                                 from b in b_join.DefaultIfEmpty()

                                                 join n in dbContext.pedido on new { pedido = a.v_IdPedido, eliminado = 0 } equals new { pedido = n.v_IdPedido, eliminado = n.i_Eliminado.Value } into n_join
                                                 from n in n_join.DefaultIfEmpty()

                                                 join B in dbContext.cliente on n.v_IdCliente equals B.v_IdCliente into B_join
                                                 from B in B_join.DefaultIfEmpty()

                                                 join J1 in dbContext.documento on n.i_IdTipoDocumento equals J1.i_CodigoDocumento into J1_join

                                                 from J1 in J1_join.DefaultIfEmpty()

                                                 //join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                 //                               equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                                 //from J2 in J2_join.DefaultIfEmpty()

                                                 //join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaUsuario.Value }
                                                 //                               equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                                                 //from J3 in J3_join.DefaultIfEmpty()

                                                 join J4 in dbContext.datahierarchy on new { CondicionPago = 41, eliminado = 0, Item = n.i_IdCondicionPago.Value }
                                                                                    equals new { CondicionPago = J4.i_GroupId, eliminado = J4.i_IsDeleted.Value, Item = J4.i_ItemId } into J4_join
                                                 from J4 in J4_join.DefaultIfEmpty()
                                                 join J5 in dbContext.vendedor on new { vendedor = n.v_IdVendedor, eliminado = 0 } equals new { vendedor = J5.v_IdVendedor, eliminado = J5.i_Eliminado.Value } into J5_join

                                                 from J5 in J5_join.DefaultIfEmpty()

                                                 join J6 in dbContext.datahierarchy on new { Grupo = 17, eliminado = 0, umItem = a.i_IdUnidadMedida.Value } equals new { Grupo = J6.i_GroupId, eliminado = J6.i_IsDeleted.Value, umItem = J6.i_ItemId } into J6_join
                                                 from J6 in J6_join.DefaultIfEmpty()



                                                 join J7 in dbContext.clientedirecciones on new { cliente = B.v_IdCliente, eliminado = B.i_Eliminado.Value, Direcc = n.i_IdDireccionCliente.Value } equals new { cliente = J7.v_IdCliente, eliminado = 0, Direcc = J7.i_IdDireccionCliente } into J7_join
                                                 from J7 in J7_join.DefaultIfEmpty()


                                                 join J8 in dbContext.datahierarchy on new { Grupo = 161, eliminado = 0, Zona = J7.i_IdZona.Value } equals new { Grupo = J8.i_GroupId, eliminado = J8.i_IsDeleted.Value, Zona = J8.i_ItemId } into J8_join
                                                 from J8 in J8_join.DefaultIfEmpty()

                                                 where a.i_Eliminado == 0 && n.t_FechaEmision >= FechaInicio && n.t_FechaEmision <= FechaFin
                                                 && (n.v_IdVendedor == IdVendedor || IdVendedor == "-1")
                                                 && (n.v_IdCliente == IdCliente || IdCliente == "")

                                                 && n.i_IdEstado != 3
                                                 && n.v_SerieDocumento == seriePedido
                                                 && n.v_CorrelativoDocumento == CorrelativoPedido
                                                  && n.i_IdTipoDocumento == (int)TiposDocumentos.Pedido
                                                 && (b.producto.v_CodInterno == CodigoProducto || CodigoProducto == "")
                                                 && a.i_IdAlmacen == Globals.ClientSession.i_IdAlmacenPredeterminado.Value
                                                 select new
                                                  {
                                                      NroPedido = J1.v_Siglas + " " + n.v_SerieDocumento + " " + n.v_CorrelativoDocumento,
                                                      CondicionPago = J4.v_Value1,
                                                      FechaPedido = n.t_FechaEmision.Value,
                                                      Producto = b.producto != null ? b.producto.v_CodInterno + "  -  " + b.producto.v_Descripcion : "PRODUCTO NO EXISTE",
                                                      Cliente = B != null ? (B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_SegundoNombre + " " + B.v_RazonSocial).Trim() : "",
                                                      CantidadPedido = a.d_CantidadEmpaque.Value,
                                                      PrecioUnitario = MonedaReporte == n.i_IdMoneda ? a.d_PrecioUnitario : MonedaReporte == (int)Currency.Soles ? a.d_PrecioUnitario * n.d_TipoCambio : a.d_PrecioUnitario / n.d_TipoCambio,
                                                      Total = MonedaReporte == n.i_IdMoneda ? a.d_PrecioVenta : MonedaReporte == (int)Currency.Soles ? a.d_PrecioVenta * n.d_TipoCambio : a.d_PrecioVenta / n.d_TipoCambio,
                                                      SeriePedido = n.v_SerieDocumento.Trim(),
                                                      CorrelativoPedido = n.v_CorrelativoDocumento.Trim(),
                                                      Vendedor = J5 == null ? "VENDEDOR NO EXISTE" : "VENDEDOR : " + J5.v_NombreCompleto,
                                                      ProductoDetalle = a.v_IdProductoDetalle,
                                                      MonedaOp = n.i_IdMoneda == (int)Currency.Soles ? "S/." : "US$.",
                                                      IdProductoDetalle = a.v_IdProductoDetalle,
                                                      //UnidadMedida = J6.v_Value2 != "1" ? "UND" : J6.v_Field,
                                                      UnidadMedida = FormatCantidad == (int)FormatoCantidad.Unidades ? J6.v_Value2 != "1" ? "UND" : J6.v_Field : J6.v_Field,
                                                      Zona = J8 != null ? J8.v_Value1 : "",
                                                      UM = J6.v_Value2,
                                                      v_IdPedidoDetalle = a.v_IdPedidoDetalle,
                                                  }

                                         ).ToList().Select(l =>
                                             {
                                                 return new ReporteRegistroPedidoPendientes
                                              {

                                                  NroPedido = l.NroPedido,
                                                  CondicionPago = l.CondicionPago,
                                                  FechaPedido = l.FechaPedido.ToShortDateString(),
                                                  Producto = l.Producto,
                                                  Cliente = l.Cliente,
                                                  CantidadPedido = FormatCantidad == (int)FormatoCantidad.Unidades ? l.CantidadPedido : decimal.Parse(l.UM) == 0 ? 0 : l.CantidadPedido / decimal.Parse(l.UM),
                                                  PrecioUnitario = l.PrecioUnitario.Value,
                                                  MonedaOp = l.MonedaOp,
                                                  Total = l.Total.Value,
                                                  SeriePedido = l.SeriePedido,
                                                  CorrelativoPedido = l.CorrelativoPedido,
                                                  ProductoDetalle = l.IdProductoDetalle,
                                                  UnidadMedidaPedido = l.UnidadMedida,
                                                  // Grupo = l.Vendedor,
                                                  // Grupo = Grupo == "Vendedor" ? l.Vendedor : Grupo == "NroPedido" ? l.NroPedido : Grupo == "Cliente" ? l.Cliente + " - ZONA : " + l.Zona : "",
                                                  Grupo = Grupo == "Vendedor" ? l.Vendedor : Grupo == "NroPedido" ? l.NroPedido : Grupo == "Cliente" ? l.Cliente + " - ZONA : " + l.Zona : Grupo == "Producto" ? "PRODUCTO :" + l.Producto : "",
                                                  ZonaCliente = l.Zona,
                                                  v_IdPedidoDetalle = l.v_IdPedidoDetalle,
                                              };

                                             }).ToList();

                    }

                    List<ReporteRegistroPedidoPendientes> Reporte = new List<ReporteRegistroPedidoPendientes>();

                    foreach (ReporteRegistroPedidoPendientes pedpendientes in ListaPedidosFaltantes)
                    {
                        decimal SaldoAnterior = 0;
                        var Facturas = Ventas.Where(a => a.SeriePedido.Trim() == pedpendientes.SeriePedido && a.CorrelativoPedido == pedpendientes.CorrelativoPedido && pedpendientes.ProductoDetalle == a.ProductoDetalle).ToList();
                        var SaldoFinalProducto = Facturas.Any() ? pedpendientes.CantidadPedido - Facturas.Sum(l => l.CantidadVendida) : pedpendientes.CantidadPedido;
                        var CantidadTotalPedido = ListaPedidosFaltantes.Where(l => l.NroPedido == pedpendientes.NroPedido).Sum(l => l.CantidadPedido);
                        var CantidadTotalFact = Ventas.Where(a => a.SeriePedido.Trim() == pedpendientes.SeriePedido && a.CorrelativoPedido == pedpendientes.CorrelativoPedido).ToList();
                        var CantidadTotalFacturas = CantidadTotalFact.Any() ? CantidadTotalFact.Sum(l => l.CantidadVendida) : 0;
                        if (Facturas.Any())
                        {
                            foreach (var DetalleFact in Facturas)
                            {
                                var _det = new ReporteRegistroPedidoPendientes();
                                _det = (ReporteRegistroPedidoPendientes)pedpendientes.Clone();
                                _det.TipoDocumento = DetalleFact.TipoDocumento;
                                _det.ZonaCliente = pedpendientes.ZonaCliente;
                                _det.NroFactura = DetalleFact.NroFactura;
                                _det.FechaEmisionDocVenta = DetalleFact.FechaEmisionDocVenta.ToShortDateString();
                                _det.CantidadFactura = DetalleFact.CantidadVendida;

                                if (DetalleFact.i_IdTipoDocumento != 7)
                                {
                                    _det.Saldo = SaldoAnterior == 0 ? pedpendientes.CantidadPedido - DetalleFact.CantidadVendida : SaldoAnterior - DetalleFact.CantidadVendida;
                                }
                                else
                                {
                                    var BuscamosFactura = Ventas.Where(x => x.i_IdTipoDocumentoReferencia == DetalleFact.i_IdTipoDocumentoReferencia && x.v_NumeroDocumentoReferencia == DetalleFact.v_NumeroDocumentoReferencia && x.ProductoDetalle == DetalleFact.ProductoDetalle).FirstOrDefault();
                                    if (BuscamosFactura != null)
                                    {
                                        _det.Saldo = DetalleFact.CantidadVendida;  //pedpendientes.CantidadPedido == DetalleFact.CantidadVendida ? pedpendientes.CantidadPedido : pedpendientes.CantidadPedido - BuscamosFactura.CantidadVendida;
                                    }
                                }

                                _det.Saldo = _det.Saldo < 0 ? 0 : _det.Saldo;
                                SaldoAnterior = _det.Saldo;
                                _det.SaldoFinalProducto = SaldoFinalProducto;
                                _det.CantidadTotalPedido = CantidadTotalPedido;
                                _det.UnidadMedidaVenta = DetalleFact.UnidadMedidaVenta;
                                _det.UnidadMedidaPedido = pedpendientes.UnidadMedidaPedido;

                                if (EstPedido == (int)EstadoPedido.Todos)
                                {
                                    Reporte.Add(_det);
                                }

                                else if (EstPedido == (int)EstadoPedido.Despachado)
                                {
                                    if (SaldoFinalProducto == 0)
                                        Reporte.Add(_det);

                                }
                                else
                                {
                                    if (SaldoFinalProducto != 0) // !=0 Se agregó porque tomaba en cuenta los negativos pero los mostraba con saldo 0
                                        Reporte.Add(_det);
                                }

                            }
                        }
                        else
                        {
                            ReporteRegistroPedidoPendientes obj = new ReporteRegistroPedidoPendientes();
                            obj = pedpendientes;
                            obj.Saldo = obj.CantidadPedido - obj.CantidadFactura;
                            obj.CantidadTotalPedido = CantidadTotalPedido;
                            obj.SaldoFinalProducto = obj.Saldo;

                            obj.UnidadMedidaVenta = pedpendientes.UnidadMedidaVenta;
                            obj.UnidadMedidaPedido = pedpendientes.UnidadMedidaPedido;

                            if (EstPedido == (int)EstadoPedido.Todos)
                            {
                                Reporte.Add(obj);
                            }

                            else if (EstPedido == (int)EstadoPedido.Despachado)
                            {
                                if (SaldoFinalProducto == 0)
                                    Reporte.Add(obj);

                            }
                            else
                            {
                                if (SaldoFinalProducto != 0) // !=0 Se agregó porque tomaba en cuenta los negativos pero los mostraba con saldo 0
                                    Reporte.Add(obj);
                            }

                        }

                    }


                    return Reporte.OrderBy(l => l.NroPedido).ThenBy(l => l.Producto).ToList();




                }
            }
            catch (Exception ex)
            {


                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "PedidoBL.ReportePedidosFaltantes()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;

            }

        }


        #endregion

        private valoresRegistroVenta CalcularVenta(int pintTipoDocumento, decimal pdecValorVenta, decimal pdecIgv,
            decimal pdecTotal, decimal pdecDescuento, decimal pdecTipoCambio, int pintMonedaId)
        {

            valoresRegistroVenta objvaloresRegistroVenta = new valoresRegistroVenta();
            if (pdecTipoCambio == 0 || pdecTipoCambio == null)
            {
                pdecTipoCambio = 1;
            }
            if (pintTipoDocumento == (int)DocumentType.NotaCredito)
            {
                if (pintMonedaId == (int)Currency.Soles)
                {
                    objvaloresRegistroVenta.ValorVentaSoles = pdecValorVenta * -1;
                    objvaloresRegistroVenta.ValorVentaDolares = (pdecValorVenta * -1) / pdecTipoCambio;

                    objvaloresRegistroVenta.IgvSoles = pdecIgv * -1;
                    objvaloresRegistroVenta.IgvDolares = (pdecIgv * -1) / pdecTipoCambio;

                    objvaloresRegistroVenta.TotalSoles = pdecTotal * -1;
                    objvaloresRegistroVenta.TotalDolares = (pdecTotal * -1) / pdecTipoCambio;

                    objvaloresRegistroVenta.DescuentoSoles = pdecDescuento * -1;
                    objvaloresRegistroVenta.DescuentoDolares = (pdecDescuento * -1) / pdecTipoCambio;

                }
                else if (pintMonedaId == (int)Currency.Dolares)
                {
                    objvaloresRegistroVenta.ValorVentaSoles = pdecValorVenta * -1 * pdecTipoCambio;
                    objvaloresRegistroVenta.ValorVentaDolares = (pdecValorVenta * -1);

                    objvaloresRegistroVenta.IgvSoles = pdecIgv * -1 * pdecTipoCambio;
                    objvaloresRegistroVenta.IgvDolares = (pdecIgv * -1);

                    objvaloresRegistroVenta.TotalSoles = pdecTotal * -1 * pdecTipoCambio;
                    objvaloresRegistroVenta.TotalDolares = (pdecTotal * -1);

                    objvaloresRegistroVenta.DescuentoSoles = pdecDescuento * -1 * pdecTipoCambio;
                    objvaloresRegistroVenta.DescuentoDolares = (pdecDescuento * -1);
                }

            }
            else
            {
                if (pintMonedaId == (int)Currency.Soles)
                {
                    objvaloresRegistroVenta.ValorVentaSoles = pdecValorVenta;
                    objvaloresRegistroVenta.ValorVentaDolares = (pdecValorVenta) / pdecTipoCambio;

                    objvaloresRegistroVenta.IgvSoles = pdecIgv;
                    objvaloresRegistroVenta.IgvDolares = (pdecIgv) / pdecTipoCambio;

                    objvaloresRegistroVenta.TotalSoles = pdecTotal;
                    objvaloresRegistroVenta.TotalDolares = (pdecTotal) / pdecTipoCambio;

                    objvaloresRegistroVenta.DescuentoSoles = pdecDescuento;
                    objvaloresRegistroVenta.DescuentoDolares = (pdecDescuento) / pdecTipoCambio;

                }
                else if (pintMonedaId == (int)Currency.Dolares)
                {
                    objvaloresRegistroVenta.ValorVentaSoles = pdecValorVenta * pdecTipoCambio;
                    objvaloresRegistroVenta.ValorVentaDolares = (pdecValorVenta);

                    objvaloresRegistroVenta.IgvSoles = pdecIgv * pdecTipoCambio;
                    objvaloresRegistroVenta.IgvDolares = (pdecIgv);

                    objvaloresRegistroVenta.TotalSoles = pdecTotal * pdecTipoCambio;
                    objvaloresRegistroVenta.TotalDolares = (pdecTotal);

                    objvaloresRegistroVenta.DescuentoSoles = pdecDescuento * pdecTipoCambio;
                    objvaloresRegistroVenta.DescuentoDolares = (pdecDescuento);
                }
            }
            return objvaloresRegistroVenta;

        }

        private valoresRegistroVenta CalcularVentaDetalle(int pintTipoDocumento, decimal pdecPrecioDetalle,
            decimal pdecValorDetalle, decimal pdecValorVentaDetalle, decimal pdecDescuentoDetalle,
            decimal pdecPrecioVentaDetalle, decimal pdecIgvDetalle, decimal pdecTipoCambio, int pintMonedaId)
        {

            valoresRegistroVenta objvaloresRegistroVenta = new valoresRegistroVenta();
            if (pdecTipoCambio == 0 || pdecTipoCambio == null)
            {
                pdecTipoCambio = 1;
            }
            if (pintTipoDocumento == (int)DocumentType.NotaCredito)
            {
                if (pintMonedaId == (int)Currency.Soles)
                {
                    objvaloresRegistroVenta.PrecioDetalleSoles = pdecPrecioDetalle * -1;
                    objvaloresRegistroVenta.PrecioDetalleDolares = (pdecPrecioDetalle * -1) / pdecTipoCambio;

                    objvaloresRegistroVenta.ValorDetalleSoles = pdecValorDetalle * -1;
                    objvaloresRegistroVenta.ValorDetalleDolares = (pdecValorDetalle * -1) / pdecTipoCambio;

                    objvaloresRegistroVenta.ValorVentaDetalleSoles = pdecValorVentaDetalle * -1;
                    objvaloresRegistroVenta.ValorVentaDetalleDolares = (pdecValorVentaDetalle * -1) / pdecTipoCambio;

                    objvaloresRegistroVenta.DescuentoDetalleSoles = pdecDescuentoDetalle * -1;
                    objvaloresRegistroVenta.DescuentoDetalleDolares = (pdecDescuentoDetalle * -1) / pdecTipoCambio;

                    objvaloresRegistroVenta.PrecioVentaDetalleSoles = pdecPrecioVentaDetalle * -1;
                    objvaloresRegistroVenta.PrecioVentaDetalleDolares = (pdecPrecioVentaDetalle * -1) / pdecTipoCambio;

                    objvaloresRegistroVenta.IgvDetalleSoles = pdecIgvDetalle * -1;
                    objvaloresRegistroVenta.IgvDetalleDolares = (pdecIgvDetalle * -1) / pdecTipoCambio;



                }
                else if (pintMonedaId == (int)Currency.Dolares)
                {



                    objvaloresRegistroVenta.PrecioDetalleSoles = pdecPrecioDetalle * -1 * pdecTipoCambio;
                    objvaloresRegistroVenta.PrecioDetalleDolares = (pdecPrecioDetalle * -1);

                    objvaloresRegistroVenta.ValorDetalleSoles = pdecValorDetalle * -1 * pdecTipoCambio;
                    objvaloresRegistroVenta.ValorDetalleDolares = (pdecValorDetalle * -1);

                    objvaloresRegistroVenta.ValorVentaDetalleSoles = pdecValorVentaDetalle * -1 * pdecTipoCambio;
                    objvaloresRegistroVenta.ValorVentaDetalleDolares = (pdecValorVentaDetalle * -1);

                    objvaloresRegistroVenta.DescuentoDetalleSoles = pdecDescuentoDetalle * -1 * pdecTipoCambio;
                    objvaloresRegistroVenta.DescuentoDetalleDolares = (pdecDescuentoDetalle * -1);

                    objvaloresRegistroVenta.PrecioVentaDetalleSoles = pdecPrecioVentaDetalle * -1 * pdecTipoCambio;
                    objvaloresRegistroVenta.PrecioVentaDetalleDolares = (pdecPrecioVentaDetalle * -1);

                    objvaloresRegistroVenta.IgvDetalleSoles = pdecIgvDetalle * -1 * pdecTipoCambio;
                    objvaloresRegistroVenta.IgvDetalleDolares = (pdecIgvDetalle * -1);

                }

            }
            else
            {
                if (pintMonedaId == (int)Currency.Soles)
                {
                    objvaloresRegistroVenta.PrecioDetalleSoles = pdecPrecioDetalle;
                    objvaloresRegistroVenta.PrecioDetalleDolares = (pdecPrecioDetalle) / pdecTipoCambio;

                    objvaloresRegistroVenta.ValorDetalleSoles = pdecValorDetalle;
                    objvaloresRegistroVenta.ValorDetalleDolares = (pdecValorDetalle) / pdecTipoCambio;

                    objvaloresRegistroVenta.ValorVentaDetalleSoles = pdecValorVentaDetalle;
                    objvaloresRegistroVenta.ValorVentaDetalleDolares = (pdecValorVentaDetalle) / pdecTipoCambio;

                    objvaloresRegistroVenta.DescuentoDetalleSoles = pdecDescuentoDetalle;
                    objvaloresRegistroVenta.DescuentoDetalleDolares = (pdecDescuentoDetalle) / pdecTipoCambio;

                    objvaloresRegistroVenta.PrecioVentaDetalleSoles = pdecPrecioVentaDetalle;
                    objvaloresRegistroVenta.PrecioVentaDetalleDolares = (pdecPrecioVentaDetalle) / pdecTipoCambio;

                    objvaloresRegistroVenta.IgvDetalleSoles = pdecIgvDetalle;
                    objvaloresRegistroVenta.IgvDetalleDolares = (pdecIgvDetalle) / pdecTipoCambio;


                }
                else if (pintMonedaId == (int)Currency.Dolares)
                {
                    objvaloresRegistroVenta.PrecioDetalleSoles = pdecPrecioDetalle * pdecTipoCambio;
                    objvaloresRegistroVenta.PrecioDetalleDolares = (pdecPrecioDetalle);

                    objvaloresRegistroVenta.ValorDetalleSoles = pdecValorDetalle * pdecTipoCambio;
                    objvaloresRegistroVenta.ValorDetalleDolares = (pdecValorDetalle);

                    objvaloresRegistroVenta.ValorVentaDetalleSoles = pdecValorVentaDetalle * pdecTipoCambio;
                    objvaloresRegistroVenta.ValorVentaDetalleDolares = (pdecValorVentaDetalle);

                    objvaloresRegistroVenta.DescuentoDetalleSoles = pdecDescuentoDetalle * pdecTipoCambio;
                    objvaloresRegistroVenta.DescuentoDetalleDolares = (pdecDescuentoDetalle);

                    objvaloresRegistroVenta.PrecioVentaDetalleSoles = pdecPrecioVentaDetalle * pdecTipoCambio;
                    objvaloresRegistroVenta.PrecioVentaDetalleDolares = (pdecPrecioVentaDetalle);

                    objvaloresRegistroVenta.IgvDetalleSoles = pdecIgvDetalle * pdecTipoCambio;
                    objvaloresRegistroVenta.IgvDetalleDolares = (pdecIgvDetalle);


                }
            }
            return objvaloresRegistroVenta;

        }



        public static bool PedidoAunExiste(string pstrIdPedido, out string pstrMensaje)
        {
            pstrMensaje = string.Empty;
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var existe = dbContext.pedido.Any(p => p.v_IdPedido == pstrIdPedido && p.i_Eliminado == 0);

                if (!existe)
                {
                    var pedidoEliminado = (from n in dbContext.pedido
                                           join J1 in dbContext.systemuser on n.i_InsertaUsuario.Value equals J1.i_SystemUserId into
                                               J1_join
                                           from J1 in J1_join.DefaultIfEmpty()

                                           join J2 in dbContext.systemuser on n.i_ActualizaIdUsuario.Value equals J2.i_SystemUserId into
                                               J2_join
                                           from J2 in J2_join.DefaultIfEmpty()

                                           where n.v_IdPedido == pstrIdPedido

                                           select new
                                           {
                                               CreaUsuario = J1.v_UserName,
                                               CreaFecha = n.t_InsertaFecha,
                                               ActUsuario = J2.v_UserName,
                                               ActFecha = n.t_ActualizaFecha
                                           }).FirstOrDefault();

                    if (pedidoEliminado != null)
                    {
                        var mensajeRespuesta = new StringBuilder();
                        mensajeRespuesta.AppendLine("EL PEDIDO YA NO EXISTE.");
                        mensajeRespuesta.AppendLine("Razón: Fue elminado.");
                        mensajeRespuesta.AppendLine("Usuario de creación: " + pedidoEliminado.CreaUsuario + " " +
                                                    pedidoEliminado.CreaFecha.Value.ToLongTimeString());
                        mensajeRespuesta.AppendLine("Usuario de eliminación: " + pedidoEliminado.ActUsuario + " " +
                                                    pedidoEliminado.ActFecha.Value.ToLongTimeString());
                        pstrMensaje = mensajeRespuesta.ToString();
                    }
                }
                return existe;
            }
        }

        public string ClonarPedido(ref OperationResult pobjOperationResult, string pstrIdPedido,
            List<string> clientSession)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var fechaClonacion = DateTime.Now;
                        var cabecera = dbContext.pedido.FirstOrDefault(p => p.v_IdPedido == pstrIdPedido);
                        var detalles = dbContext.pedidodetalle.Where(p => p.v_IdPedido == pstrIdPedido && p.i_Eliminado == 0);
                        if (cabecera == null || !detalles.Any()) return null;

                        var nuevaCabecera = Utils.ObjectCopier.Clone(cabecera);
                        nuevaCabecera.i_IdEstado = 0;
                        nuevaCabecera.i_ActualizaIdUsuario = null;
                        nuevaCabecera.t_ActualizaFecha = null;
                        nuevaCabecera.t_FechaEmision = fechaClonacion.Date;
                        nuevaCabecera.v_Mes = fechaClonacion.Date.Month.ToString("00");

                        nuevaCabecera.v_SerieDocumento = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, cabecera.i_IdTipoDocumento.Value);
                        nuevaCabecera.v_CorrelativoDocumento = _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, cabecera.i_IdTipoDocumento.Value).ToString("00000000");
                        var _ListadoPedidos = ObtenerListadoPedidos(ref pobjOperationResult, cabecera.v_Periodo, nuevaCabecera.v_Mes);
                        var _MaxV = _ListadoPedidos.Count - 1;
                        nuevaCabecera.v_Correlativo = (int.Parse(_ListadoPedidos[_MaxV].Value1) + 1).ToString("00000000");

                        var nuevoDetalles = detalles.ToList().Select(d => d.ToDTO()).ToList();
                        var listaSeparacion = nuevoDetalles.Where(p => !new ProductoBL().ProductoDetalleEsServicio(p.v_IdProductoDetalle))
                            .Select(p => new separacionproductoDto
                        {
                            v_IdProductoAlmacen = ObtenerProductoALmacen(p.v_IdProductoDetalle, p.i_IdAlmacen.Value),
                            i_IdAlmacen = p.i_IdAlmacen.Value,
                            v_IdProductoDetalle = p.v_IdProductoDetalle,
                            d_Separacion_Cantidad = p.d_Cantidad,
                            d_Separacion_CantidadEmpaque = p.d_CantidadEmpaque
                        }
                        ).ToList();

                        var idRetorno = InsertarPedido(ref pobjOperationResult, nuevaCabecera.ToDTO(), clientSession, nuevoDetalles, listaSeparacion);
                        Utils.Windows.GeneraHistorial(LogEventType.CLONACION, Globals.ClientSession.v_UserName, "pedido", pstrIdPedido + "=>" + idRetorno);

                        if (pobjOperationResult.Success == 0) return null;

                        pobjOperationResult.Success = 1;
                        ts.Complete();
                        return idRetorno;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "PedidoBL.ClonarPedido()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }


        private string ObtenerProductoALmacen(string pstrProductoDetalle, int idAlmacen)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var periodo = Globals.ClientSession.i_Periodo.ToString();
                var result =
                    dbContext.productoalmacen.FirstOrDefault(p => p.v_ProductoDetalleId == pstrProductoDetalle && p.i_IdAlmacen == idAlmacen && p.i_Eliminado == 0 && p.v_Periodo == periodo);

                if (result != null)
                    return result.v_IdProductoAlmacen;

                return null;
            }
        }

    }
}
