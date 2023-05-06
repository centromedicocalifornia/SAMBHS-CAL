using SAMBHS.Common.BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System.ComponentModel;
using System.Transactions;

namespace SAMBHS.Compra.BL
{
    public class OrdenCompraBL
    {
        public List<ordendecompraShortDto> ListarOrdenCompras(ref OperationResult pobjOperationResult, DateTime F_IniReg, DateTime F_FinReg, DateTime F_IniEnt, DateTime F_FinEnt, string pstrSortExpression, bool UsadaExtraccion = false)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    pobjOperationResult.Success = 1;
                    if (UsadaExtraccion)
                    {

                        var Query = (from n in dbContext.ordendecompra

                                     join J1 in dbContext.cliente on n.v_IdProveedor equals J1.v_IdCliente into J1_join
                                     from J1 in J1_join.DefaultIfEmpty()

                                     join J2 in dbContext.datahierarchy on new { Area = n.i_IdAreaSolicita.Value, b = 90 }
                                                                        equals new { Area = J2.i_ItemId, b = J2.i_GroupId } into J2_join
                                     from J2 in J2_join.DefaultIfEmpty()

                                     join J3 in dbContext.datahierarchy on new { Area = n.i_IdEstado.Value, b = 92 }
                                                                        equals new { Area = J3.i_ItemId, b = J3.i_GroupId } into J3_join
                                     from J3 in J3_join.DefaultIfEmpty()

                                     join J4 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                                     equals new { i_InsertUserId = J4.i_SystemUserId } into J4_join
                                     from J4 in J4_join.DefaultIfEmpty()

                                     join J5 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                                     equals new { i_UpdateUserId = J5.i_SystemUserId } into J5_join
                                     from J5 in J5_join.DefaultIfEmpty()

                                     join J6 in dbContext.documento on new { TipoDoc = n.i_IdTipoDocumento.Value, eliminado = 0 } 
                                                                equals new { TipoDoc =J6.i_CodigoDocumento ,eliminado = J6.i_Eliminado.Value  } into J6_join
                                     from J6 in J6_join.DefaultIfEmpty ()

                                     where n.t_FechaRegistro >= F_IniReg && n.t_FechaRegistro <= F_FinReg && n.i_Eliminado == 0

                                     && n.i_IdEstado !=3 && n.i_IdTipoOrdenCompra ==2

                                     select new ordendecompraShortDto
                                     {
                                         Importe = n.d_Total,
                                         Estado = J3.v_Value1,
                                         AreaSolicita = J2.v_Value1,
                                         FechaActualiza = n.t_ActualizaFecha,
                                         UsuarioActualiza = J5.v_UserName,
                                         UsuarioCrea = J4.v_UserName,
                                         FechaCrea = n.t_InsertaFecha,
                                         FechaEntrega = n.t_FechaEntrega,
                                         FechaRegistro = n.t_FechaRegistro,
                                         RazonSocialProveedor = (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre + " " + J1.v_SegundoNombre + " " + J1.v_RazonSocial).Trim(),
                                         v_IdOrdenCompra = n.v_IdOrdenCompra,
                                         v_IdProveedor = n.v_IdProveedor,
                                         i_IdAreaSolicita = n.i_IdAreaSolicita,
                                         i_IdEstado = n.i_IdEstado,
                                         NroOrdenCompra = n.v_SerieDocumento + "-" + n.v_CorrelativoDocumento,
                                         Moneda = n.i_IdMoneda == 1 ? "S" : "D",
                                         DocInterno = n.v_DocumentoInterno,
                                         TipoDocumento = J6.v_Siglas,
                                         iTipoDocumento = n.i_IdTipoDocumento ?? -1,
                                     });

                        if (!string.IsNullOrEmpty(pstrSortExpression))
                        {
                            Query = Query.Where(pstrSortExpression);
                        }

                        pobjOperationResult.Success = 1;

                        return Query.ToList();
                    }
                    else
                    {
                        var Query = (from n in dbContext.ordendecompra

                                     join J1 in dbContext.cliente on n.v_IdProveedor equals J1.v_IdCliente into J1_join
                                     from J1 in J1_join.DefaultIfEmpty()

                                     join J2 in dbContext.datahierarchy on new { Area = n.i_IdAreaSolicita.Value, b = 90 }
                                                                        equals new { Area = J2.i_ItemId, b = J2.i_GroupId } into J2_join
                                     from J2 in J2_join.DefaultIfEmpty()

                                     join J3 in dbContext.datahierarchy on new { Area = n.i_IdEstado.Value, b = 92 }
                                                                        equals new { Area = J3.i_ItemId, b = J3.i_GroupId } into J3_join
                                     from J3 in J3_join.DefaultIfEmpty()

                                     join J4 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                                     equals new { i_InsertUserId = J4.i_SystemUserId } into J4_join
                                     from J4 in J4_join.DefaultIfEmpty()

                                     join J5 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                                     equals new { i_UpdateUserId = J5.i_SystemUserId } into J5_join
                                     from J5 in J5_join.DefaultIfEmpty()



                                     where n.t_FechaRegistro >= F_IniReg && n.t_FechaRegistro <= F_FinReg && n.t_FechaEntrega >= F_IniEnt &&
                                           n.t_FechaEntrega <= F_FinEnt && n.i_Eliminado == 0

                                     select new ordendecompraShortDto
                                     {
                                         Importe = n.d_Total,
                                         Estado = J3.v_Value1,
                                         AreaSolicita = J2.v_Value1,
                                         FechaActualiza = n.t_ActualizaFecha,
                                         UsuarioActualiza = J5.v_UserName,
                                         UsuarioCrea = J4.v_UserName,
                                         FechaCrea = n.t_InsertaFecha,
                                         FechaEntrega = n.t_FechaEntrega,
                                         FechaRegistro = n.t_FechaRegistro,
                                         RazonSocialProveedor = (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre + " " + J1.v_SegundoNombre + " " + J1.v_RazonSocial).Trim(),
                                         v_IdOrdenCompra = n.v_IdOrdenCompra,
                                         v_IdProveedor = n.v_IdProveedor,
                                         i_IdAreaSolicita = n.i_IdAreaSolicita,
                                         i_IdEstado = n.i_IdEstado,
                                         NroOrdenCompra = n.v_SerieDocumento + "-" + n.v_CorrelativoDocumento,
                                         Moneda = n.i_IdMoneda == 1 ? "S" : "D",
                                         DocInterno = n.v_DocumentoInterno,

                                     });

                        if (!string.IsNullOrEmpty(pstrSortExpression))
                        {
                            Query = Query.Where(pstrSortExpression);
                        }


                        return Query.ToList();
                    }

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "OrdenCompraBL.ListarOrdenCompras()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public ordendecompraDto ObtenerOrdenCompra(ref OperationResult pobjOperationResult, string IdOrdenCompra)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    ordendecompraDto EntityDto = (from n in dbContext.ordendecompra

                                                  join J1 in dbContext.cliente on n.v_IdProveedor equals J1.v_IdCliente into J1_join
                                                  from J1 in J1_join.DefaultIfEmpty()

                                                  join J2 in dbContext.datahierarchy on new { grupo = 111, id = n.i_IdEntidadBancaria.Value }
                                                                                    equals new { grupo = J2.i_GroupId, id = J2.i_ItemId } into J2_join
                                                  from J2 in J2_join.DefaultIfEmpty()

                                                  where n.v_IdOrdenCompra == IdOrdenCompra

                                                  select new ordendecompraDto
                                                  {
                                                      d_IGV = n.d_IGV,
                                                      d_SubTotal = n.d_SubTotal,
                                                      d_TipoCambio = n.d_TipoCambio,
                                                      d_Total = n.d_Total,
                                                      v_DocumentoInterno = n.v_DocumentoInterno,
                                                      v_AdjuntarAnexo = n.v_AdjuntarAnexo,
                                                      v_Correlativo = n.v_Correlativo,
                                                      v_IdOrdenCompra = n.v_IdOrdenCompra,
                                                      v_IdProveedor = n.v_IdProveedor,
                                                      v_Importante = n.v_Importante,
                                                      v_LugarEntrega = n.v_LugarEntrega,
                                                      v_Mes = n.v_Mes,
                                                      v_Periodo = n.v_Periodo,
                                                      i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                                      i_Eliminado = n.i_Eliminado,
                                                      i_IdAreaSolicita = n.i_IdAreaSolicita,
                                                      i_IdEstado = n.i_IdEstado,
                                                      i_IdFormaPago = n.i_IdFormaPago,
                                                      i_NodeId = n.i_NodeId,
                                                      i_IdMoneda = n.i_IdMoneda,
                                                      i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                                      t_InsertaFecha = n.t_InsertaFecha,
                                                      t_ActualizaFecha = n.t_ActualizaFecha,
                                                      t_FechaEntrega = n.t_FechaEntrega,
                                                      t_FechaRegistro = n.t_FechaRegistro,
                                                      RazonSocialProveedor = (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre + " " + J1.v_SegundoNombre + " " + J1.v_RazonSocial).Trim(),
                                                      RUCProveedor = J1.v_NroDocIdentificacion,
                                                      i_IdTipoDocumento = n.i_IdTipoDocumento,
                                                      v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                                      v_SerieDocumento = n.v_SerieDocumento,
                                                      i_PreciosAfectosIgv = n.i_PreciosAfectosIgv,
                                                      i_PreciosIncluyeIgv = n.i_PreciosIncluyeIgv,
                                                      i_IdEntidadBancaria = n.i_IdEntidadBancaria,
                                                      EntidadBancaria = J2.v_Value1,
                                                      i_NroDias = n.i_NroDias,
                                                      v_NroCheque = n.v_NroCheque,
                                                      i_IdTipoOrdenCompra = n.i_IdTipoOrdenCompra == null ? 1 : n.i_IdTipoOrdenCompra,
                                                      //v_CodCliente = J1.v_CodCliente,
                                                      //v_RazonSocial = (J1.v_ApePaterno + " " + J1.v_ApeMaterno + " " + J1.v_PrimerNombre + " " + J1.v_SegundoNombre + " " + J1.v_RazonSocial).Trim(),
                                                      //v_IdCliente = J1.v_IdCliente,



                                                  }).FirstOrDefault();


                    if (EntityDto != null)
                    {
                        pobjOperationResult.Success = 1;

                        return EntityDto;
                    }
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "No se encontró la Orden de Compra!";
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    return null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "OrdenCompraBL.ObtenerOrdenCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }


        public  BindingList<importaciondetallefobDto> ListaObtenerOrdenCompraExtraccion(ref OperationResult pobjOperationResult, string IdOrdenCompra)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                  var EntityDto = (from n in dbContext.ordendecompra

                                                  join J1 in dbContext.cliente on n.v_IdProveedor equals J1.v_IdCliente into J1_join
                                                  from J1 in J1_join.DefaultIfEmpty()

                                                  join J2 in dbContext.datahierarchy on new { grupo = 111, id = n.i_IdEntidadBancaria.Value }
                                                                                    equals new { grupo = J2.i_GroupId, id = J2.i_ItemId } into J2_join
                                                  from J2 in J2_join.DefaultIfEmpty()

                                                  where n.v_IdOrdenCompra == IdOrdenCompra

                                                  select new importaciondetallefobDto 
                                                  {
                                                     
                                                      v_IdCliente = J1.v_IdCliente,
                                                      v_RazonSocial =(J1.v_ApePaterno +" "+ J1.v_ApeMaterno +" "+J1.v_PrimerNombre +" "+ J1.v_SegundoNombre + " "+ J1.v_RazonSocial ).Trim (),
                                                      d_ValorFob =0,
                                                      d_TipoCambio =0,
                                                      i_IdTipoDocumento =-1,
                                                      v_SerieDocumento ="",
                                                      v_CorrelativoDocumento ="",
                                                      t_FechaEmision =DateTime.Now ,
                                                      v_CodCliente=J1.v_CodCliente ,
                                                      v_IdImportacion =n.v_IdOrdenCompra ,
                                                      i_IdTipoDocumentoReferencia = n.i_IdTipoDocumento ?? -1,
                                                      v_SerieDocumentoReferencia = n.v_SerieDocumento.Trim(),
                                                      v_CorrelativoDocumentoReferencia = n.v_CorrelativoDocumento.Trim(),
                                                     

                                                  }).ToList();


                    if (EntityDto != null)
                    {
                        pobjOperationResult.Success = 1;
                        var query = new BindingList<importaciondetallefobDto>(EntityDto);
                        pobjOperationResult.Success = 1;
                        return query;
                    }
                    pobjOperationResult.Success = 0;
                    pobjOperationResult.AdditionalInformation = "No se encontró la Orden de Compra!";
                    Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                    return null;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "OrdenCompraBL.ObtenerOrdenCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public BindingList<ordendecompradetalleDto> CargarDetalle(ref OperationResult pobjOperationResult, string IdOrdenCompra)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    List<ordendecompradetalleDto> Query = (from n in dbContext.ordendecompradetalle

                                                           join J1 in dbContext.productodetalle on n.v_IdProductoDetalle equals J1.v_IdProductoDetalle into J1_join
                                                           from J1 in J1_join.DefaultIfEmpty()

                                                           join J2 in dbContext.producto on J1.v_IdProducto equals J2.v_IdProducto into J2_join
                                                           from J2 in J2_join.DefaultIfEmpty()

                                                           join J4 in dbContext.datahierarchy on new { a = J2.i_IdUnidadMedida.Value, b = 17 }
                                                                                        equals new { a = J4.i_ItemId, b = J4.i_GroupId } into J4_join
                                                           from J4 in J4_join.DefaultIfEmpty()

                                                           where n.v_IdOrdenCompra == IdOrdenCompra && n.i_Eliminado == 0

                                                           select new ordendecompradetalleDto
                                                           {
                                                               i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                                               i_Eliminado = n.i_Eliminado,
                                                               i_IdUnidadMedida = n.i_IdUnidadMedida,
                                                               i_IdAlmacen = n.i_IdAlmacen,
                                                               i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                                               d_IGV = n.d_IGV,
                                                               t_InsertaFecha = n.t_InsertaFecha,
                                                               v_IdOrdenCompra = n.v_IdOrdenCompra,
                                                               v_IdOrdenCompraDetalle = n.v_IdOrdenCompraDetalle,
                                                               v_IdProductoDetalle = n.v_IdProductoDetalle,
                                                               d_Cantidad = n.d_Cantidad,
                                                               d_CantidadEmpaque = n.d_CantidadEmpaque,
                                                               d_PrecioTotal = n.d_PrecioTotal,
                                                               d_PrecioUnitario = n.d_PrecioUnitario,
                                                               d_PrecioVenta = n.d_PrecioVenta,
                                                               d_SubTotal = n.d_SubTotal,
                                                               Empaque = J2.d_Empaque,
                                                               EmpaqueUM = J4.v_Value1,
                                                               CodProducto = J2.v_CodInterno,
                                                               NombreProducto = J2.v_Descripcion,
                                                               t_ActualizaFecha = n.t_ActualizaFecha,
                                                               UMProducto = J2.i_IdUnidadMedida ?? -1,
                                                              

                                                           }).ToList().OrderBy(o => o.v_IdOrdenCompraDetalle).ToList();

                    BindingList<ordendecompradetalleDto> Result = new BindingList<ordendecompradetalleDto>(Query);
                    pobjOperationResult.Success = 1;
                    return Result;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "OrdenCompraBL.CargarDetalle()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public List<ordendeCompraDetailDto> CargarDetalleWithCompras(ref OperationResult pobjOperationResult, string IdOrdenCompra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var q = (from n in dbContext.ordendecompradetalle
                             join J1 in dbContext.productodetalle on n.v_IdProductoDetalle equals J1.v_IdProductoDetalle into J1_join
                             from J1 in J1_join.DefaultIfEmpty()

                             join J2 in dbContext.producto on J1.v_IdProducto equals J2.v_IdProducto into J2_join
                             from J2 in J2_join.DefaultIfEmpty()

                             where n.v_IdOrdenCompra == IdOrdenCompra && n.i_Eliminado == 0

                             select new ordendeCompraDetailDto
                             {
                                 i_IdUnidadMedida = n.i_IdUnidadMedida,
                                 v_IdOrdenCompraDetalle = n.v_IdOrdenCompraDetalle,
                                 v_IdProductoDetalle = n.v_IdProductoDetalle,
                                 d_Cantidad = n.d_Cantidad,
                                 CodProducto = J2.v_CodInterno,
                                 NombreProducto = J2.v_Descripcion,
                                 d_CantidadCancelada = n.d_CantidadCancelada
                             }).ToList();
                    var oc = dbContext.ordendecompra
                            .Where(r => r.v_IdOrdenCompra == IdOrdenCompra && r.i_Eliminado == 0)
                            .Select(r => new { r.v_SerieDocumento, r.v_CorrelativoDocumento })
                            .FirstOrDefault();

                    var qC = (from n in dbContext.compra
                              join j1 in dbContext.compradetalle on n.v_IdCompra equals j1.v_IdCompra into j1_Join
                              from j1 in j1_Join.DefaultIfEmpty()
                              where n.i_Eliminado == 0 && j1.i_Eliminado == 0 &&
                              n.v_ODCSerie == oc.v_SerieDocumento && n.v_ODCCorrelativo == oc.v_CorrelativoDocumento
                              group
                              new CompraDetail
                              {
                                  i_IdTipoDocumento = n.i_IdTipoDocumento ?? 1,
                                  v_SerieDocumento = n.v_SerieDocumento,
                                  v_CorrelativoDocumento = n.v_CorrelativoDocumento,
                                  d_Cantidad = j1.d_Cantidad ?? 0
                              } by j1.v_IdProductoDetalle);

                    var details = q.ToList();
                    foreach (var item in details)
                    {
                        var rel = qC.FirstOrDefault(g => g.Key == item.v_IdProductoDetalle);
                        if (rel != null)
                            item.listaComprasDto = rel.ToList();
                    }
                    pobjOperationResult.Success = 1;
                    return details;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "OrdenCompraBL.CargarDetalleWithCompras()";
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public void UpdateCantidadCancelada(ref OperationResult pobjOperationResult, IEnumerable<Tuple<string, decimal>> details)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    foreach (var item in details)
                    {
                        var objEntity = dbContext.ordendecompradetalle.FirstOrDefault(r => r.v_IdOrdenCompraDetalle == item.Item1);
                        if (objEntity != null)
                        {
                            objEntity.d_CantidadCancelada = item.Item2;
                        }
                    }
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception er)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = er.Message;
            }
        }

        public string InsertarOrdenCompra(ref OperationResult pobjOperationResult, ordendecompraDto EntityDto, List<ordendecompradetalleDto> Temp_Insertar, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        DocumentoBL _objDocumentoBL = new DocumentoBL();
                        ordendecompra objEntityOrdenCompra = EntityDto.ToEntity();
                        ordendecompraDto pobjDtoOrdenCompraDetalle = new ordendecompraDto();

                        int SecuentialId = 0;
                        string newIdOrdenCompra = string.Empty;
                        string newIdOrdenCompraDetalle = string.Empty;
                        int intNodeId;

                        #region Inserta Cabecera
                        objEntityOrdenCompra.t_InsertaFecha = DateTime.Now;
                        objEntityOrdenCompra.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityOrdenCompra.i_Eliminado = 0;
                        intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 70);
                        newIdOrdenCompra = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LK");
                        objEntityOrdenCompra.v_IdOrdenCompra = newIdOrdenCompra;
                        dbContext.AddToordendecompra(objEntityOrdenCompra);
                        #endregion

                        #region Inserta Detalles
                        foreach (ordendecompradetalleDto DetalleEntityDto in Temp_Insertar)
                        {
                            ordendecompradetalle objEntityOCDetalle = new ordendecompradetalle();
                            objEntityOCDetalle = ordendecompradetalleAssembler.ToEntity(DetalleEntityDto);

                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 71);
                            newIdOrdenCompraDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "LL");
                            objEntityOCDetalle.v_IdOrdenCompraDetalle = newIdOrdenCompraDetalle;
                            objEntityOCDetalle.v_IdOrdenCompra = newIdOrdenCompra;
                            objEntityOCDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityOCDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityOCDetalle.i_Eliminado = 0;
                            dbContext.AddToordendecompradetalle(objEntityOCDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "ordendecompradetalle", newIdOrdenCompraDetalle);
                        }
                        #endregion

                        #region Actualiza Correlativo Empresa
                        _objDocumentoBL.ActualizarCorrelativoPorSerie(ref pobjOperationResult, Globals.ClientSession.i_IdEstablecimiento, EntityDto.i_IdTipoDocumento.Value, EntityDto.v_SerieDocumento, int.Parse(EntityDto.v_CorrelativoDocumento) + 1);
                        if (pobjOperationResult.Success == 0) return string.Empty;
                        #endregion

                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "ordendecompra", newIdOrdenCompra);
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                        return newIdOrdenCompra;
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "OrdenCompraBL.InsertarOrdenCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public string ActualizarOrdenCompra(ref OperationResult pobjOperationResult, ordendecompraDto EntityDto, List<ordendecompradetalleDto> Temp_Insertar, List<ordendecompradetalleDto> Temp_Modificar, List<ordendecompradetalleDto> Temp_Eliminar, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        DocumentoBL _objDocumentoBL = new DocumentoBL();
                        string newIdOrdenCompraDetalle = string.Empty;
                        int SecuentialId = 0;
                        int intNodeId;

                        #region Actualiza Cabecera
                        intNodeId = int.Parse(ClientSession[0]);
                        var EntitySource = (from n in dbContext.ordendecompra
                                            where n.v_IdOrdenCompra == EntityDto.v_IdOrdenCompra
                                            select n).FirstOrDefault();

                        EntitySource.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);
                        EntitySource.t_ActualizaFecha = DateTime.Now;

                        ordendecompra Entity = ordendecompraAssembler.ToEntity(EntityDto);
                        dbContext.ordendecompra.ApplyCurrentValues(Entity);
                        #endregion

                        #region Actualiza Detalle

                        #region Insertar
                        foreach (ordendecompradetalleDto _ordendecompradetalleDto in Temp_Insertar)
                        {
                            ordendecompradetalle objEntityCompraDetalle = ordendecompradetalleAssembler.ToEntity(_ordendecompradetalleDto);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 32);
                            newIdOrdenCompraDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZY");
                            objEntityCompraDetalle.v_IdOrdenCompra = Entity.v_IdOrdenCompra;
                            objEntityCompraDetalle.v_IdOrdenCompraDetalle = newIdOrdenCompraDetalle;
                            objEntityCompraDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityCompraDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityCompraDetalle.i_Eliminado = 0;
                            dbContext.AddToordendecompradetalle(objEntityCompraDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "ordendecompradetalle", newIdOrdenCompraDetalle);
                        }

                        #endregion

                        #region Modificar
                        foreach (ordendecompradetalleDto _ordendecompradetalleDto in Temp_Modificar)
                        {
                            ordendecompradetalle Detalle = (from n in dbContext.ordendecompradetalle
                                                            where n.v_IdOrdenCompraDetalle == _ordendecompradetalleDto.v_IdOrdenCompraDetalle
                                                            select n).FirstOrDefault();

                            Detalle.v_IdProductoDetalle = _ordendecompradetalleDto.v_IdProductoDetalle;
                            Detalle.d_Cantidad = _ordendecompradetalleDto.d_Cantidad;
                            Detalle.d_CantidadEmpaque = _ordendecompradetalleDto.d_CantidadEmpaque;
                            Detalle.i_IdAlmacen = _ordendecompradetalleDto.i_IdAlmacen;
                            Detalle.i_IdUnidadMedida = _ordendecompradetalleDto.i_IdUnidadMedida;
                            Detalle.d_IGV = _ordendecompradetalleDto.d_IGV;
                            Detalle.d_PrecioTotal = _ordendecompradetalleDto.d_PrecioTotal;
                            Detalle.d_PrecioUnitario = _ordendecompradetalleDto.d_PrecioUnitario;
                            Detalle.d_SubTotal = _ordendecompradetalleDto.d_SubTotal;

                            Detalle.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);
                            Detalle.t_ActualizaFecha = DateTime.Now;

                            dbContext.ordendecompradetalle.ApplyCurrentValues(Detalle);
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "ordendecompradetalle", Detalle.v_IdOrdenCompraDetalle);
                        }
                        #endregion

                        #region Eliminar
                        foreach (ordendecompradetalleDto _ordendecompradetalleDto in Temp_Eliminar)
                        {
                            ordendecompradetalle Detalle = (from n in dbContext.ordendecompradetalle
                                                            where n.v_IdOrdenCompraDetalle == _ordendecompradetalleDto.v_IdOrdenCompraDetalle
                                                            select n).FirstOrDefault();

                            Detalle = ordendecompradetalleAssembler.ToEntity(_ordendecompradetalleDto);

                            Detalle.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);
                            Detalle.t_ActualizaFecha = DateTime.Now;
                            Detalle.i_Eliminado = 1;

                            dbContext.ordendecompradetalle.ApplyCurrentValues(Detalle);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "ordendecompradetalle", Detalle.v_IdOrdenCompraDetalle);
                        }
                        #endregion

                        #endregion

                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "ordendecompra", EntitySource.v_IdOrdenCompra);
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                        return EntitySource.v_IdOrdenCompra;
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "OrdenCompraBL.ActualizarOrdenCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var Registro = (from n in dbContext.ordendecompra
                            where n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo
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

        public bool ExisteDocumento(string pstrSerie, string pstrCorrelativo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            var Registro = (from n in dbContext.ordendecompra
                            where n.i_Eliminado == 0 && n.v_SerieDocumento == pstrSerie && n.v_CorrelativoDocumento == pstrCorrelativo
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

        public List<ReporteOrdenCompraDto> ReporteOrdenCompra(ref OperationResult pobjOperationResult, string IdOrdenCompra)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Consulta = (from n in dbContext.ordendecompradetalle

                                    join J1 in dbContext.ordendecompra on n.v_IdOrdenCompra equals J1.v_IdOrdenCompra into J1_join
                                    from J1 in J1_join.DefaultIfEmpty()

                                    join J2 in dbContext.productodetalle on n.v_IdProductoDetalle equals J2.v_IdProductoDetalle into J2_join
                                    from J2 in J2_join.DefaultIfEmpty()

                                    join J3 in dbContext.producto on J2.v_IdProducto equals J3.v_IdProducto into J3_join
                                    from J3 in J3_join.DefaultIfEmpty()

                                    join Proveedor in dbContext.cliente on J1.v_IdProveedor equals Proveedor.v_IdCliente into Proveedor_join
                                    from Proveedor in Proveedor_join.DefaultIfEmpty()

                                    join J4 in dbContext.datahierarchy on new { grupo = 91, id = J1.i_IdFormaPago.Value }
                                                                        equals new { grupo = J4.i_GroupId, id = J4.i_ItemId } into J4_join
                                    from J4 in J4_join.DefaultIfEmpty()

                                    join J5 in dbContext.datahierarchy on new { grupo = 111, id = J1.i_IdEntidadBancaria.Value }
                                                                        equals new { grupo = J5.i_GroupId, id = J5.i_ItemId } into J5_join
                                    from J5 in J5_join.DefaultIfEmpty()

                                    join J6 in dbContext.datahierarchy on new { grupo = 17, id = n.i_IdUnidadMedida.Value, eliminado = 0 }
                                    equals new { grupo = J6.i_GroupId, id = J6.i_ItemId, eliminado = J6.i_IsDeleted.Value } into J6_join
                                    from J6 in J6_join.DefaultIfEmpty()

                                    where n.v_IdOrdenCompra == IdOrdenCompra

                                    select new
                                    {
                                        Cantidad = n.d_Cantidad.Value,
                                        CodArticulo = J3.v_CodInterno,
                                        NroOrdenCompra = J1.v_SerieDocumento + "-" + J1.v_CorrelativoDocumento,
                                        DescripcionArticulo = J3.v_Descripcion,
                                        Fecha = J1.t_FechaRegistro.Value,
                                        Igv = n.d_IGV.Value,
                                        Total = n.d_PrecioTotal.Value,
                                        Moneda = J1.i_IdMoneda == 1 ? "SOLES" : "DÓLARES",
                                        Obsevaciones = J1.v_Importante,
                                        LugarEntrega = J1.v_LugarEntrega,
                                        PrecioUnitario = n.d_PrecioUnitario.Value,
                                        Proveedor = (Proveedor.v_ApePaterno + " " + Proveedor.v_ApeMaterno + " " + Proveedor.v_PrimerNombre + " " + Proveedor.v_RazonSocial).Trim(),
                                        RUCProveedor = Proveedor.v_NroDocIdentificacion,
                                        DireccionProveedor = Proveedor.v_DirecPrincipal,
                                        Contacto = Proveedor.v_NombreContacto,
                                        TelefonoProveedor = Proveedor.v_TelefonoMovil + " " + Proveedor.v_TelefonoFijo,
                                        Plazo = J4.v_Value1,
                                        EntidadBancaria = J5.v_Value1,
                                        NroCheque = J1.v_NroCheque,
                                        NroDias = J1.i_NroDias,
                                        TipoCambio = J1.d_TipoCambio,
                                        FechaEntrega = J1.t_FechaEntrega.Value,
                                        SubTotal = n.d_SubTotal ?? 0,
                                        UnidadMedida = J6 != null ? J6.v_Field : "",
                                        AdjuntarAnexo = J1.v_AdjuntarAnexo,

                                    }).ToList().Select(p => new ReporteOrdenCompraDto
                                    {
                                        Cantidad = p.Cantidad,
                                        CodArticulo = p.CodArticulo,
                                        NroOrdenCompra = p.NroOrdenCompra,
                                        DescripcionArticulo = p.DescripcionArticulo,
                                        Fecha = p.Fecha,
                                        Igv = p.Igv,
                                        Total = p.Total,
                                        Moneda = p.Moneda,
                                        Obsevaciones = p.Obsevaciones,
                                        LugarEntrega = p.LugarEntrega,
                                        PrecioUnitario = p.PrecioUnitario,
                                        Proveedor = p.Proveedor,
                                        RUCProveedor = p.RUCProveedor,
                                        Plazo = p.Plazo,
                                        EntidadBancaria = p.EntidadBancaria,
                                        NroCheque = p.NroCheque,
                                        NroDias = p.NroDias != null ? p.NroDias.Value.ToString("000") : "000",
                                        TipoCambio = p.TipoCambio.ToString(),
                                        FechaEntrega = p.FechaEntrega,
                                        DireccionProveedor = p.DireccionProveedor,
                                        Contacto = p.Contacto,
                                        Telefono = p.TelefonoProveedor,
                                        SubTotal = p.SubTotal,
                                        UnidadMedida = p.UnidadMedida,
                                        AdjuntarAnexo = p.AdjuntarAnexo,


                                    }).ToList();

                    pobjOperationResult.Success = 1;

                    return Consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "OrdenCompraBL.ReporteOrdenCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void EliminarOrdenCompra(ref OperationResult pobjOperationResult, string pstrIdOrdenCompra, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var EntitySource = dbContext.ordendecompra.FirstOrDefault(p => p.v_IdOrdenCompra == pstrIdOrdenCompra);
                    if (EntitySource == null) throw new ArgumentNullException("Orden de Compra no Existe");
                    EntitySource.i_Eliminado = 1;
                    EntitySource.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);
                    EntitySource.t_ActualizaFecha = DateTime.Now;

                    EntitySource.ordendecompradetalle.ToList().ForEach(f =>
                    {
                        f.i_Eliminado = 1;
                        f.i_ActualizaIdUsuario = int.Parse(ClientSession[2]);
                        f.t_ActualizaFecha = DateTime.Now;
                        dbContext.ordendecompradetalle.ApplyCurrentValues(f);
                    });

                    dbContext.ordendecompra.ApplyCurrentValues(EntitySource);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "OrdenCompraBL.EliminarOrdenCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public List<ReporteOrdenCompraEstado> ReporteOrdenCompraEstado(ref OperationResult pobjOperationResult, DateTime init, DateTime end, string idProveedor)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var listCompra = (from n in dbContext.ordendecompra
                                      join j1 in dbContext.ordendecompradetalle on new { n.v_IdOrdenCompra, Value = 0 } equals new { j1.v_IdOrdenCompra, j1.i_Eliminado.Value } into j1_Join
                                      from j1 in j1_Join.DefaultIfEmpty()
                                      join j2 in dbContext.productodetalle on j1.v_IdProductoDetalle equals j2.v_IdProductoDetalle into j2_Join
                                      from j2 in j2_Join.DefaultIfEmpty()
                                      join j3 in dbContext.producto on j2.v_IdProducto equals j3.v_IdProducto into j3_Join
                                      from j3 in j3_Join.DefaultIfEmpty()
                                      join j4 in dbContext.compra on new { s = n.v_SerieDocumento, c = n.v_CorrelativoDocumento } equals new { s = j4.v_ODCSerie, c = j4.v_ODCCorrelativo } into j4_Join
                                      from j4 in j4_Join.DefaultIfEmpty()
                                      join j5 in dbContext.documento on j4.i_IdTipoDocumento equals j5.i_CodigoDocumento into j5_Join
                                      from j5 in j5_Join.DefaultIfEmpty()
                                      join j6 in dbContext.compradetalle on new { j4.v_IdCompra, j1.v_IdProductoDetalle, e = 0 } equals new { j6.v_IdCompra, j6.v_IdProductoDetalle, e = j6.i_Eliminado.Value } into j6_Join
                                      from j6 in j6_Join.DefaultIfEmpty()
                                      where n.i_Eliminado == 0 && n.t_FechaRegistro >= init && n.t_FechaRegistro <= end
                                      && (n.v_IdProveedor == idProveedor || idProveedor == "")
                                      select new ReporteOrdenCompraEstado
                                      {
                                          CodProd = j3.v_CodInterno,
                                          NombreProd = j3.v_Descripcion,
                                          d_Cantidad = j1.d_Cantidad ?? 0,
                                          d_CantidadCancelada = j1.d_CantidadCancelada ?? 0,
                                          OrdenCompraDoc = n.v_SerieDocumento + "-" + n.v_CorrelativoDocumento,
                                          v_Documento = j5 != null ? j5.v_Siglas : "",
                                          v_SerieDocumento = j4 != null ? j4.v_SerieDocumento : "",
                                          v_CorrelativoDocumento = j4 != null ? j4.v_CorrelativoDocumento : "",
                                          d_CantidadFact = j6 != null ? (j6.d_Cantidad ?? 0M) : 0M
                                      }).ToList();
                    pobjOperationResult.Success = 1;
                    return listCompra;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
            }
            return null;
        }
    }
}
