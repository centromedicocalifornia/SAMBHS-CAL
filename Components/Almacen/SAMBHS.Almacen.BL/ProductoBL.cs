using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using SAMBHS.CommonWIN.BL;
using System.Data.Objects;
using SAMBHS.Common.BL;
using System.ComponentModel;

namespace SAMBHS.Almacen.BL
{
    public class ProductoBL
    {
        public delegate void ProcesoTerminado(int result, OperationResult pobjOperationResult);

        public event ProcesoTerminado OnProcesoTerminado;

        public productoDto ObtenerProducto(ref OperationResult pobjOperationResult, string pstrProductoId)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var objEntity = (from a in dbContext.producto
                                     join b in dbContext.productodetalle on a.v_IdProducto equals b.v_IdProducto
                                     join j1 in dbContext.systemuser on new { i_InsertUserId = a.i_InsertaIdUsuario.Value }
                                     equals new { i_InsertUserId = j1.i_SystemUserId } into j1Join
                                     from j1 in j1Join.DefaultIfEmpty()

                                     join j2 in dbContext.systemuser on new { i_UpdateUserId = a.i_ActualizaIdUsuario.Value }
                                     equals new { i_UpdateUserId = j2.i_SystemUserId } into j2Join
                                     from j2 in j2Join.DefaultIfEmpty()

                                     where a.v_IdProducto == pstrProductoId
                                     select new productoDto
                                     {
                                         v_IdProductoDetalle = b.v_IdProductoDetalle,
                                         v_IdProducto = a.v_IdProducto,
                                         v_CodInterno = a.v_CodInterno,
                                         v_IdLinea = a.v_IdLinea,
                                         v_IdMarca = a.v_IdMarca,
                                         v_IdModelo = a.v_IdModelo,
                                         v_IdTalla = b.v_IdTalla,
                                         v_IdColor = b.v_IdColor,
                                         v_Descripcion = a.v_Descripcion,
                                         d_Empaque = a.d_Empaque,
                                         i_IdUnidadMedida = a.i_IdUnidadMedida,
                                         d_Peso = a.d_Peso,
                                         i_NombreEditable = a.i_NombreEditable,
                                         v_Ubicacion = a.v_Ubicacion,
                                         v_Caracteristica = a.v_Caracteristica,
                                         v_CodProveedor = a.v_CodProveedor,
                                         v_Descripcion2 = a.v_Descripcion2,
                                         i_IdTipoProducto = a.i_IdTipoProducto,
                                         i_EsServicio = a.i_EsServicio,
                                         i_EsLote = a.i_EsLote,
                                         i_EsActivo = a.i_EsActivo,
                                         d_PrecioVenta = a.d_PrecioVenta,
                                         d_PrecioCosto = a.d_PrecioCosto,
                                         d_StockMinimo = a.d_StockMinimo,
                                         d_StockMaximo = a.d_StockMaximo,
                                         i_Eliminado = a.i_Eliminado,
                                         i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                         t_InsertaFecha = a.t_InsertaFecha,
                                         t_ActualizaFecha = a.t_ActualizaFecha,
                                         v_UsuarioCreacion = j1.v_UserName,
                                         v_UsuarioModificacion = j2.v_UserName,
                                         i_IdProveedor = a.i_IdProveedor,
                                         i_IdTipo = a.i_IdTipo,
                                         i_IdUsuario = a.i_IdUsuario,
                                         i_IdTela = a.i_IdTela,
                                         i_IdEtiqueta = a.i_IdEtiqueta,
                                         i_IdCuello = a.i_IdCuello,
                                         i_IdAplicacion = a.i_IdAplicacion,
                                         i_IdArte = a.i_IdArte,
                                         i_IdColeccion = a.i_IdColeccion,
                                         i_IdTemporada = a.i_IdTemporada,
                                         i_Anio = a.i_Anio,
                                         i_EsAfectoDetraccion = a.i_EsAfectoDetraccion,
                                         i_ValidarStock = a.i_ValidarStock,
                                         i_EsAfectoPercepcion = a.i_EsAfectoPercepcion,
                                         d_TasaPercepcion = a.d_TasaPercepcion,
                                         i_PrecioEditable = a.i_PrecioEditable,
                                         b_Foto = a.b_Foto,
                                         v_Modelo = a.v_Modelo,
                                         i_EsAfectoIsc = a.i_EsAfectoIsc,
                                         i_CantidadFabricacionMensual = a.i_CantidadFabricacionMensual ?? 0,
                                         v_NroPartidaArancelaria = a.v_NroPartidaArancelaria,
                                         i_IndicaFormaParteOtrosTributos = a.i_IndicaFormaParteOtrosTributos ?? 0,
                                         v_NroParte = string.IsNullOrEmpty(a.v_NroParte) ? "" : a.v_NroParte,
                                         v_NroOrdenProduccion = string.IsNullOrEmpty(a.v_NroOrdenProduccion) ? "" : a.v_NroOrdenProduccion,
                                         i_IdTipoTributo = a.i_IdTipoTributo ?? -1,
                                         d_Utilidad = a.d_Utilidad ?? 0,
                                         d_PrecioMayorista = a.d_PrecioMayorista ?? 0,
                                         i_IdPerfilDetraccion = a.i_IdPerfilDetraccion ?? -1,

                                         i_SolicitarNroLoteIngreso = a.i_SolicitarNroLoteIngreso ?? 0,
                                         i_SolicitarNroSerieIngreso = a.i_SolicitarNroSerieIngreso ?? 0,
                                         i_SolicitaOrdenProduccionIngreso = a.i_SolicitaOrdenProduccionIngreso ?? 0,
                                         i_SolicitarNroSerieSalida = a.i_SolicitarNroSerieSalida ?? 0,
                                         i_SolicitarNroLoteSalida = a.i_SolicitarNroLoteSalida ?? 0,
                                         i_SolicitaOrdenProduccionSalida = a.i_SolicitaOrdenProduccionSalida ?? 0,
                                         i_Insumo = a.i_Insumo
                                     }
                    ).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    return objEntity;
                }
            }
            catch (Exception ex)
            {


                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ObtenerProducto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public productoDto ObtenerProductoPorCodigoDetallado(ref OperationResult pobjOperationResult, string codigo)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from a in dbContext.producto
                                     join b in dbContext.productodetalle on a.v_IdProducto equals b.v_IdProducto
                                     join j1 in dbContext.systemuser on new { i_InsertUserId = a.i_InsertaIdUsuario.Value }
                                                   equals new { i_InsertUserId = j1.i_SystemUserId } into j1Join
                                     from j1 in j1Join.DefaultIfEmpty()

                                     join j2 in dbContext.systemuser on new { i_UpdateUserId = a.i_ActualizaIdUsuario.Value }
                                                                   equals new { i_UpdateUserId = j2.i_SystemUserId } into j2Join
                                     from j2 in j2Join.DefaultIfEmpty()

                                     where a.v_CodInterno == codigo
                                     select new productoDto
                                     {
                                         v_IdProductoDetalle = b.v_IdProductoDetalle,
                                         v_IdProducto = a.v_IdProducto,
                                         v_CodInterno = a.v_CodInterno,
                                         v_IdLinea = a.v_IdLinea,
                                         v_IdMarca = a.v_IdMarca,
                                         v_IdModelo = a.v_IdModelo,
                                         v_IdTalla = b.v_IdTalla,
                                         v_IdColor = b.v_IdColor,
                                         v_Descripcion = a.v_Descripcion,
                                         d_Empaque = a.d_Empaque,
                                         i_IdUnidadMedida = a.i_IdUnidadMedida,
                                         d_Peso = a.d_Peso,
                                         i_NombreEditable = a.i_NombreEditable,
                                         v_Ubicacion = a.v_Ubicacion,
                                         v_Caracteristica = a.v_Caracteristica,
                                         v_CodProveedor = a.v_CodProveedor,
                                         v_Descripcion2 = a.v_Descripcion2,
                                         i_IdTipoProducto = a.i_IdTipoProducto,
                                         i_EsServicio = a.i_EsServicio,
                                         i_EsLote = a.i_EsLote,
                                         i_EsActivo = a.i_EsActivo,
                                         d_PrecioVenta = a.d_PrecioVenta,
                                         d_PrecioCosto = a.d_PrecioCosto,
                                         d_StockMinimo = a.d_StockMinimo,
                                         d_StockMaximo = a.d_StockMaximo,
                                         i_Eliminado = a.i_Eliminado,
                                         i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                         t_InsertaFecha = a.t_InsertaFecha,
                                         t_ActualizaFecha = a.t_ActualizaFecha,
                                         v_UsuarioCreacion = j1.v_UserName,
                                         v_UsuarioModificacion = j2.v_UserName,
                                         i_IdProveedor = a.i_IdProveedor,
                                         i_IdTipo = a.i_IdTipo,
                                         i_IdUsuario = a.i_IdUsuario,
                                         i_IdTela = a.i_IdTela,
                                         i_IdEtiqueta = a.i_IdEtiqueta,
                                         i_IdCuello = a.i_IdCuello,
                                         i_IdAplicacion = a.i_IdAplicacion,
                                         i_IdArte = a.i_IdArte,
                                         i_IdColeccion = a.i_IdColeccion,
                                         i_IdTemporada = a.i_IdTemporada,
                                         i_Anio = a.i_Anio,
                                         i_EsAfectoDetraccion = a.i_EsAfectoDetraccion,
                                         i_ValidarStock = a.i_ValidarStock,
                                         i_EsAfectoPercepcion = a.i_EsAfectoPercepcion,
                                         d_TasaPercepcion = a.d_TasaPercepcion,
                                         i_PrecioEditable = a.i_PrecioEditable,
                                         b_Foto = a.b_Foto,
                                         v_Modelo = a.v_Modelo,
                                     }
                                             ).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return objEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<productodetalleDto> ListaTodosProductosDetalles()
        {

            //using (var dbContext = new SAMBHSEntitiesModelWin())
            //{


            //    var productosDetalles = (from a in dbContext.productodetalle 
            //                             join b in dbContext.productoalmacen on a.v_IdProductoDetalle equals b.v_ProductoDetalleId

            //                             where a.i_Eliminado == 0 && a.producto.v_IdProducto != "N002-PD000000000" && a.producto != null
            //                             && a.producto.i_EsServicio == 0 && a.producto.i_EsActivoFijo == 0 && a.producto.i_EsActivo == 1
            //                             && b.i_Eliminado == 0

            //                             select new productodetalleDto
            //                             {

            //                                 v_IdProductoDetalle = a.v_IdProductoDetalle,
            //                                 i_ActualizaIdUsuario = a.producto.i_IdUnidadMedida.Value,
            //                                 v_IdProducto = a.v_IdProducto,
            //                             }).ToList();
            //    string codigo = "";
            //    List<productodetalleDto> _newlist = new List<productodetalleDto>();
                
            //    foreach (var item in productosDetalles)
            //    {    
            //        if (codigo != item.v_IdProducto)
            //        {
            //            codigo = item.v_IdProducto;
            //            _newlist.Add(item);
            //        }
            //    }

            //    return _newlist.ToList();
            //}

            using (var dbContext = new SAMBHSEntitiesModelWin())
            {


                var productosDetalles = (from a in dbContext.productodetalle

                    where a.i_Eliminado == 0 && a.producto.v_IdProducto != "N002-PD000000000" && a.producto != null
                          && a.producto.i_EsServicio == 0 && a.producto.i_EsActivoFijo == 0 && a.producto.i_EsActivo == 1

                    select new productodetalleDto
                    {

                        v_IdProductoDetalle = a.v_IdProductoDetalle,
                        i_ActualizaIdUsuario = a.producto.i_IdUnidadMedida.Value,
                        v_IdProducto = a.v_IdProducto,
                    }).ToList();
                return productosDetalles;
            }
        }


        public List<productodetalleDto> ListaTodosProductosDetallesAct(string IdLinea)
        {

            using (var dbContext = new SAMBHSEntitiesModelWin())
            {


                var productosDetalles = (from a in dbContext.productodetalle

                                         where a.i_Eliminado == 0 && a.producto.v_IdProducto != "N002-PD000000000" && a.producto != null
                                         && a.producto.i_EsServicio == 0 && a.producto.i_EsActivoFijo == 0
                                         && (a.producto.v_IdLinea == IdLinea || IdLinea == null)
                                         select new productodetalleDto
                                         {

                                             v_IdProductoDetalle = a.v_IdProductoDetalle,
                                             i_ActualizaIdUsuario = a.producto != null ? 1 : a.producto.i_IdUnidadMedida.Value,
                                             v_IdProducto = a.v_IdProducto,
                                             v_IdColor = a.producto == null ? "" : a.producto.v_CodInterno,
                                             v_IdTalla = a.producto == null ? "" : a.producto.v_Descripcion
                                         }).ToList();
                return productosDetalles;
            }
        }

        public productoDto ObtenerProductoDesdeProdDetalle(ref OperationResult pobjOperationResult, string IdProductoDetalle)
        {
            try
            {
                pobjOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var producto = (from a in dbContext.productodetalle
                                    join b in dbContext.producto on new { p = a.v_IdProducto, eliminado = 0 } equals new { p = b.v_IdProducto, eliminado = b.i_Eliminado.Value } into b_join
                                    from b in b_join.DefaultIfEmpty()
                                    join c in dbContext.linea on b.v_IdLinea equals c.v_IdLinea into cjoin
                                    from c in cjoin.DefaultIfEmpty()
                                    where a.v_IdProductoDetalle == IdProductoDetalle && a.i_Eliminado == 0

                                    select new productoDto
                                    {
                                        v_CodInterno = b.v_CodInterno,
                                        v_IdProductoDetalle = a.v_IdProductoDetalle,
                                        v_Descripcion = b.v_Descripcion,
                                        d_Empaque = b.d_Empaque ?? 1,
                                        NroCuenta = c.v_NroCuentaVenta,
                                        v_Descripcion2 = b.v_Descripcion2,
                                        v_NroPartidaArancelaria = b.v_NroPartidaArancelaria,
                                        i_IndicaFormaParteOtrosTributos = b.i_IndicaFormaParteOtrosTributos ?? 0,
                                    }).FirstOrDefault();
                    return producto;

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                return null;
            }

        }
        public List<productoDto> ListarProducto(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, int SoloActivos)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = (from n in dbContext.producto

                                 join A in dbContext.linea on new { n.v_IdLinea } equals new { A.v_IdLinea } into A_join
                                 from A in A_join.DefaultIfEmpty()

                                 join B in dbContext.marca on n.v_IdMarca equals B.v_IdMarca into B_join
                                 from B in B_join.DefaultIfEmpty()

                                 join D in dbContext.productodetalle on new { n.v_IdProducto } equals new { D.v_IdProducto } into D_join
                                 from D in D_join.DefaultIfEmpty()

                                 join E in dbContext.color on D.v_IdColor equals E.v_IdColor into E_join
                                 from E in E_join.DefaultIfEmpty()

                                 join F in dbContext.talla on D.v_IdTalla equals F.v_IdTalla into F_join
                                 from F in F_join.DefaultIfEmpty()

                                 join J1 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                 equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                                 from J1 in J1_join.DefaultIfEmpty()

                                 join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                 equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                 from J2 in J2_join.DefaultIfEmpty()

                                 join J3 in dbContext.datahierarchy on new { g = 6, e = 0, id = n.i_IdTipoProducto.Value }
                                                                equals new { g = J3.i_GroupId, e = J3.i_IsDeleted.Value, id = J3.i_ItemId } into J3_join
                                 from J3 in J3_join.DefaultIfEmpty()
                                 where n.i_Eliminado == 0 && n.i_EsActivoFijo == 0

                                 select new productoDto
                                 {
                                     v_IdProducto = n.v_IdProducto,
                                     v_IdProductoDetalle = D.v_IdProductoDetalle,
                                     v_Descripcion = n.v_Descripcion.ToUpper(),
                                     v_CodInterno = n.v_CodInterno,
                                     v_IdLinea = n.v_IdLinea,
                                     NombreLinea = A == null ? "" : A.v_Nombre,
                                     v_IdMarca = n.v_IdMarca,
                                     NombreMarca = B.v_Nombre,
                                     v_IdModelo = n.v_IdModelo,
                                     NombreModelo = n.v_Modelo,
                                     v_IdTalla = D.v_IdTalla,
                                     NombreTalla = F.v_Nombre,
                                     v_IdColor = D.v_IdColor,
                                     NombreColor = E.v_Nombre,
                                     i_EsServicio = n.i_EsServicio,
                                     i_EsLote = n.i_EsLote,
                                     i_NombreEditable = n.i_NombreEditable,
                                     i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                     t_InsertaFecha = n.t_InsertaFecha,
                                     t_ActualizaFecha = n.t_ActualizaFecha,
                                     v_UsuarioCreacion = J1.v_UserName,
                                     v_UsuarioModificacion = J2.v_UserName,
                                     i_IdTipoProducto = n.i_IdTipoProducto,
                                     i_PrecioEditable = n.i_PrecioEditable,
                                     i_EsActivo = n.i_EsActivo ?? 0,
                                     v_NroParte = n.v_NroParte,
                                     TipoProducto = J3 == null ? "" : J3.v_Value1,
                                     i_IdTipoTributo = n.i_IdTipoTributo ?? -1,
                                     v_Ubicacion = n.v_Ubicacion,

                                     i_SolicitarNroLoteIngreso = n.i_SolicitarNroLoteIngreso ?? 0,
                                     i_SolicitarNroSerieIngreso = n.i_SolicitarNroSerieIngreso ?? 0,
                                     i_SolicitaOrdenProduccionIngreso = n.i_SolicitaOrdenProduccionIngreso ?? 0,
                                     i_SolicitarNroSerieSalida = n.i_SolicitarNroSerieSalida ?? 0,
                                     i_SolicitarNroLoteSalida = n.i_SolicitarNroLoteSalida ?? 0,
                                     i_SolicitaOrdenProduccionSalida = n.i_SolicitaOrdenProduccionSalida ?? 0,


                                 }
                    );

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<productoDto> objData = query.ToList();
                    pobjOperationResult.Success = 1;

                    if (SoloActivos == 1 || SoloActivos == 0)
                    {
                        return objData.Where(o => o.i_EsActivo == SoloActivos).ToList();
                    }
                    else
                    {
                        return objData;
                    }

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public string InsertarProducto(ref OperationResult pobjOperationResult, productoDto pobjDtoEntity, List<string> ClientSession = null, SAMBHSEntitiesModelWin objContext = null)
        {
            try
            {
                var dbContext = objContext ?? new SAMBHSEntitiesModelWin();
                
                    if (ClientSession == null) ClientSession = new List<string> { "1", "1", "1" };
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    producto objEntityProducto = productoAssembler.ToEntity(pobjDtoEntity);

                    productodetalleDto pobjDtoProductoDetalle = new productodetalleDto();
                    productodetalle objEntityProductoDetalle = productodetalleAssembler.ToEntity(pobjDtoProductoDetalle);

                    objEntityProducto.t_InsertaFecha = DateTime.Now;
                    objEntityProducto.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntityProducto.i_Eliminado = 0;


                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    var SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 24, objContext);
                    var newIdProducto = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PD");
                    objEntityProducto.v_IdProducto = newIdProducto;
                    dbContext.AddToproducto(objEntityProducto);



                    //ProductoDetalle
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 26, objContext);
                    var newIdProductoDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PE");
                    objEntityProductoDetalle.v_IdProductoDetalle = newIdProductoDetalle;
                    objEntityProductoDetalle.v_IdProducto = newIdProducto;
                    objEntityProductoDetalle.v_IdTalla = pobjDtoEntity.v_IdTalla;
                    objEntityProductoDetalle.v_IdColor = pobjDtoEntity.v_IdColor;
                    objEntityProductoDetalle.t_InsertaFecha = DateTime.Now;
                    objEntityProductoDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntityProductoDetalle.i_Eliminado = 0;
                    dbContext.AddToproductodetalle(objEntityProductoDetalle);
                    dbContext.SaveChanges();

                    if (pobjDtoEntity.i_EsActivoFijo != 1)
                    {
                        InscribirProductoEnLosAlmacenes(ref pobjOperationResult, newIdProductoDetalle, ClientSession, objContext);
                        if (pobjOperationResult.Success == 0) return null;
                    }

                    //Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "producto", newIdProducto);
                    //Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "productodetalle", newIdProductoDetalle);
                    pobjOperationResult.Success = 1;
                    return newIdProducto + ";" + newIdProductoDetalle;
                
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.InsertarProducto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }


        public void InscribirProductosEnAlmacen(ref OperationResult objOperationResult, int Almacen, string periodo, List<string> ClientSession)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    objOperationResult.Success = 1;
                    var productosDetalles = dbContext.productodetalle.Where(l => l.i_Eliminado == 0 && l.producto.i_EsActivoFijo != 1).ToList().Select(l => l.v_IdProductoDetalle).ToList();
                    var productoAlmacenExistente = dbContext.productoalmacen.Where(l => l.i_IdAlmacen == Almacen && l.v_Periodo == periodo && l.i_Eliminado == 0).ToList();
                    var xx = productoAlmacenExistente.Where(l => l.v_ProductoDetalleId == "N001-PE000037184").ToList();
                    var productosInsertar = productoAlmacenExistente.Where(p => !productosDetalles.Contains(p.v_ProductoDetalleId)).Select(o => o.v_ProductoDetalleId).ToList();
                    if (productosInsertar.Any())
                    {
                        Globals.ProgressbarStatus.i_TotalProgress = productosInsertar.Count;
                        foreach (var item in productosInsertar)
                        {
                            var productoAlmacenExistent = productoAlmacenExistente.Any(l => l.v_ProductoDetalleId == item);
                            if (!productoAlmacenExistent)
                            {
                                var productoalmacen = new productoalmacen();
                                var intNodeId = int.Parse(ClientSession[0]);
                                var SecuentialId = new SecuentialBL().GetNextSecuentialId(intNodeId, 30);
                                var newIdProductoAlmacen = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PA");
                                productoalmacen.v_IdProductoAlmacen = newIdProductoAlmacen;
                                productoalmacen.i_IdAlmacen = Almacen;
                                productoalmacen.v_NroPedido = null;
                                productoalmacen.v_ProductoDetalleId = item;
                                productoalmacen.t_InsertaFecha = DateTime.Now;
                                productoalmacen.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                productoalmacen.i_Eliminado = 0;
                                productoalmacen.d_StockMinimo = 0;
                                productoalmacen.d_StockMaximo = 0;
                                productoalmacen.d_StockActual = 0;
                                productoalmacen.d_SeparacionTotal = 0;
                                productoalmacen.i_Eliminado = 0;
                                productoalmacen.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                                productoalmacen.t_InsertaFecha = DateTime.Now;
                                productoalmacen.v_Periodo = periodo;
                                dbContext.AddToproductoalmacen(productoalmacen);

                                Globals.ProgressbarStatus.i_Progress++;
                            }
                            else
                            {
                                Globals.ProgressbarStatus.i_Progress++;
                            }
                        }

                    }
                    else
                    {

                        Globals.ProgressbarStatus.i_TotalProgress = productosDetalles.Count;
                        foreach (var item in productosDetalles)
                        {
                            var productoAlmacenExistent = productoAlmacenExistente.Any(l => l.v_ProductoDetalleId == item);
                            if (!productoAlmacenExistent)
                            {
                                var productoalmacen = new productoalmacen();
                                var intNodeId = int.Parse(ClientSession[0]);
                                var SecuentialId = new SecuentialBL().GetNextSecuentialId(intNodeId, 30);
                                var newIdProductoAlmacen = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PA");
                                productoalmacen.v_IdProductoAlmacen = newIdProductoAlmacen;
                                productoalmacen.i_IdAlmacen = Almacen;
                                productoalmacen.v_NroPedido = null;
                                productoalmacen.v_ProductoDetalleId = item;
                                productoalmacen.t_InsertaFecha = DateTime.Now;
                                productoalmacen.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                productoalmacen.i_Eliminado = 0;
                                productoalmacen.d_StockMinimo = 0;
                                productoalmacen.d_StockMaximo = 0;
                                productoalmacen.d_StockActual = 0;
                                productoalmacen.d_SeparacionTotal = 0;
                                productoalmacen.i_Eliminado = 0;
                                productoalmacen.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                                productoalmacen.t_InsertaFecha = DateTime.Now;
                                productoalmacen.v_Periodo = periodo;
                                dbContext.AddToproductoalmacen(productoalmacen);

                                Globals.ProgressbarStatus.i_Progress++;
                            }

                        }
                    }

                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;

            }

        }
        /// <summary>
        /// Al insertar el producto en la bd, este metodo registra al 
        /// producto en producto almacen para que se evite errores al vender productos sin estar en un almacen
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdProductoDetalle"></param>
        /// <param name="ClientSession"></param>
        /// 



        private static void InscribirProductoEnLosAlmacenes(ref OperationResult pobjOperationResult, string pstrIdProductoDetalle, List<string> ClientSession = null, SAMBHSEntitiesModelWin objContext = null)
        {
            try
            {
                var dbContext = objContext ?? new SAMBHSEntitiesModelWin();
                
                    if (ClientSession == null) ClientSession = new List<string> { "1", "1", "1" };
                    string periodo = Globals.ClientSession != null ? Globals.ClientSession.i_Periodo.ToString() : DateTime.Now.Year.ToString();
                    var listadoAlmacenes = dbContext.almacen.Where(p => p.i_Eliminado == 0).ToList().Select(o => o.i_IdAlmacen).Distinct();

                    foreach (var almacen in listadoAlmacenes)
                    {
                        var existeEnAlmacen =
                            dbContext.productoalmacen.Any(
                                p =>
                                    p.i_IdAlmacen == almacen && p.v_ProductoDetalleId == pstrIdProductoDetalle &&
                                    p.i_Eliminado == 0 && p.v_Periodo == periodo);

                        if (!existeEnAlmacen)
                        {
                            var itemStock = new productoalmacen();
                            var intNodeId = int.Parse(ClientSession[0]);
                            var SecuentialId = new SecuentialBL().GetNextSecuentialId(intNodeId, 30, objContext);
                            var newIdProductoAlmacen = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PA");
                            itemStock.v_IdProductoAlmacen = newIdProductoAlmacen;

                            itemStock.i_IdAlmacen = almacen;
                            itemStock.v_ProductoDetalleId = pstrIdProductoDetalle;
                            itemStock.d_StockMinimo = 0;
                            itemStock.d_StockMaximo = 0;
                            itemStock.d_StockActual = 0;
                            itemStock.d_SeparacionTotal = 0;
                            itemStock.i_Eliminado = 0;
                            itemStock.i_InsertaIdUsuario = int.Parse(ClientSession[2]);
                            itemStock.t_InsertaFecha = DateTime.Now;
                            itemStock.v_Periodo = periodo;
                            dbContext.AddToproductoalmacen(itemStock);
                        }
                    }

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.InscribirProductoEnLosAlmacenes()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public List<string> InsertarProductos(ref OperationResult pobjOperationResult, List<productoDto> pobjDtoEntitys, List<string> ClientSession)
        {

            int SecuentialId = 0;
            string newIdProducto = string.Empty;
            string newIdProductoDetalle = string.Empty;
            try
            {
                using (var transaction = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    dbContext.producto.MergeOption = MergeOption.NoTracking;
                    dbContext.productodetalle.MergeOption = MergeOption.NoTracking;
                    List<producto> ProductosXInsertar = new List<producto>();
                    List<productodetalle> ProductosDetalleXInsertar = new List<productodetalle>();
                    List<string> ids = new List<string>();
                    foreach (productoDto pobjDtoEntity in pobjDtoEntitys)
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        producto objEntityProducto = productoAssembler.ToEntity(pobjDtoEntity);

                        productodetalleDto pobjDtoProductoDetalle = new productodetalleDto();
                        productodetalle objEntityProductoDetalle = productodetalleAssembler.ToEntity(pobjDtoProductoDetalle);

                        objEntityProducto.t_InsertaFecha = DateTime.Now;
                        objEntityProducto.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityProducto.i_Eliminado = 0;

                        // Autogeneramos el Pk de la tabla
                        int intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 24);
                        newIdProducto = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PD");
                        objEntityProducto.v_IdProducto = newIdProducto;
                        ProductosXInsertar.Add(objEntityProducto);

                        //ProductoDetalle
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 26);
                        newIdProductoDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PE");
                        objEntityProductoDetalle.v_IdProductoDetalle = newIdProductoDetalle;
                        objEntityProductoDetalle.v_IdProducto = newIdProducto;
                        objEntityProductoDetalle.v_IdTalla = pobjDtoEntity.v_IdTalla;
                        objEntityProductoDetalle.v_IdColor = pobjDtoEntity.v_IdColor;
                        objEntityProductoDetalle.t_InsertaFecha = DateTime.Now;
                        objEntityProductoDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityProductoDetalle.i_Eliminado = 0;
                        ProductosDetalleXInsertar.Add(objEntityProductoDetalle);

                        dbContext.AddToproducto(objEntityProducto);
                        dbContext.AddToproductodetalle(objEntityProductoDetalle);
                        dbContext.SaveChanges();

                        ids.Add(newIdProductoDetalle);


                        if (objEntityProducto.i_EsActivoFijo != 1)
                        {
                            InscribirProductoEnLosAlmacenes(ref pobjOperationResult, newIdProductoDetalle, ClientSession);
                            if (pobjOperationResult.Success == 0) return null;
                        }
                    }

                    #region Insertar Producto Redondeo si es necesario
                    producto cpg = (from c in dbContext.producto
                                    where c.v_IdProducto == "N002-PD000000000"
                                    select c).FirstOrDefault();

                    if (cpg == null)
                    {
                        producto pr = new producto();
                        pr.v_IdProducto = "N002-PD000000000";
                        pr.v_CodInterno = "0000";
                        pr.v_Descripcion = "REDONDEO";
                        pr.d_Empaque = 1;
                        pr.i_IdUnidadMedida = 15;
                        pr.d_Peso = 0;
                        pr.i_EsActivo = 1;
                        pr.i_EsAfectoDetraccion = 0;
                        pr.i_EsLote = 0;
                        pr.i_EsServicio = 1;
                        pr.i_ValidarStock = 0;
                        pr.i_NombreEditable = 0;
                        pr.i_InsertaIdUsuario = 1;
                        pr.t_InsertaFecha = DateTime.Today;
                        pr.i_Eliminado = 0;
                        dbContext.AddToproducto(pr);
                    }

                    productodetalle _cpg = (from c in dbContext.productodetalle
                                            where c.v_IdProductoDetalle == "N002-PE000000000"
                                            select c).FirstOrDefault();

                    if (_cpg == null)
                    {
                        productodetalle pd = new productodetalle();
                        pd.v_IdProductoDetalle = "N002-PE000000000";
                        pd.v_IdProducto = "N002-PD000000000";
                        pd.i_InsertaIdUsuario = 1;
                        pd.t_InsertaFecha = DateTime.Today;
                        pd.i_Eliminado = 0;
                        dbContext.AddToproductodetalle(pd);
                    }

                    #endregion

                    dbContext.SaveChanges();



                    pobjOperationResult.Success = 1;

                    transaction.Complete();
                    return ids;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.InsertarProductos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public void ActualizarProducto(ref OperationResult pobjOperationResult, productoDto pobjDtoEntity, productodetalleDto pobjDtoEntityDetail, List<string> ClientSession, bool ActualizarLp, decimal TipoCambio)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente Producto
                    var objEntitySource = (from a in dbContext.producto
                                           where a.v_IdProducto == pobjDtoEntity.v_IdProducto
                                           select a).FirstOrDefault();

                    // Obtener la entidad fuente Producto Detalle
                    var objEntitySourceDetalle = (from a in dbContext.productodetalle
                                                  where a.v_IdProductoDetalle == pobjDtoEntityDetail.v_IdProductoDetalle
                                                  select a).FirstOrDefault();

                    var periodo = Globals.ClientSession.i_Periodo.ToString();
                    // Crear la entidad con los datos actualizados Producto
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    // Crear la entidad con los datos actualizados Producto Detalle
                    pobjDtoEntityDetail.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntityDetail.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    producto objEntity = productoAssembler.ToEntity(pobjDtoEntity);
                    productodetalle objEntityDetalle = productodetalleAssembler.ToEntity(pobjDtoEntityDetail);

                    dbContext.producto.ApplyCurrentValues(objEntity);
                    dbContext.productodetalle.ApplyCurrentValues(objEntityDetalle);

                    //Actualiza el costo de ListaPrecio
                    if (ActualizarLp)
                    {
                        var objEntityListaProductosAlmacen = (from a in dbContext.productodetalle
                                                              where a.v_IdProductoDetalle == pobjDtoEntity.v_IdProductoDetalle
                                                              select a).FirstOrDefault();
                        if (objEntityListaProductosAlmacen != null)
                        {
                            var objListaPrecioDetalle = (from a in dbContext.listapreciodetalle
                                                         where a.v_IdProductoDetalle == objEntityListaProductosAlmacen.v_IdProductoDetalle && a.i_Eliminado == 0
                                                         select a).ToList();



                            foreach (var ListaPrecioDetalle in objListaPrecioDetalle)
                            {

                                ListaPrecioDetalle.d_Costo = pobjDtoEntity.d_PrecioCosto ?? 0;

                                if (Globals.ClientSession.i_SeUsaraSoloUnaListaPrecioEmpresa == 1)
                                {
                                    ListaPrecioDetalle.d_Precio = pobjDtoEntity.d_PrecioVenta;
                                    ListaPrecioDetalle.d_Utilidad = pobjDtoEntity.d_Utilidad;
                                }
                                else
                                {
                                    ListaPrecioDetalle.d_Precio = ListaPrecioDetalle.d_Utilidad == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado((pobjDtoEntity.d_PrecioCosto + ((pobjDtoEntity.d_PrecioCosto * ListaPrecioDetalle.d_Utilidad) / 100)).Value, Globals.ClientSession.i_PrecioDecimales.Value);
                                }
                                ListaPrecioDetalle.d_PrecioMinSoles = ListaPrecioDetalle.d_Precio - ((ListaPrecioDetalle.d_Precio * ListaPrecioDetalle.d_Descuento) / 100);
                                ListaPrecioDetalle.d_PrecioMinDolares = ListaPrecioDetalle.d_PrecioMinSoles / TipoCambio;
                                ListaPrecioDetalle.t_ActualizaFecha = DateTime.Now;
                                ListaPrecioDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                dbContext.listapreciodetalle.ApplyCurrentValues(ListaPrecioDetalle);
                                Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "listapreciodetalle", ListaPrecioDetalle.v_idListaPrecioDetalle);
                            }
                        }
                    }


                    // Guardar los cambios
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "producto", objEntitySource.v_IdProducto);
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "productodetalle", objEntitySourceDetalle.v_IdProductoDetalle);
                }
                pobjOperationResult.Success = 1;
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ActualizarProducto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public void EliminarProducto(ref OperationResult pobjOperationResult, string pstrProductoId, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.producto
                                           where a.v_IdProducto == pstrProductoId
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;

                    // Guardar los cambios
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "producto", objEntitySource.v_IdProducto);
                }
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.EliminarProducto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }



        public productoDto ObtenerProductoCodigo(ref OperationResult pobjOperationResult, string pstrCodigo)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    productoDto objDtoEntity = null;
                    var objEntity = (from A in dbContext.producto
                                     where A.v_CodInterno == pstrCodigo && A.i_Eliminado == 0
                                     select A
                                     ).FirstOrDefault();
                    if (objEntity != null)
                        objDtoEntity = productoAssembler.ToDTO(objEntity);

                    pobjOperationResult.Success = 1;
                    return objDtoEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ObtenerProductoCodigo()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }

        }

        public void ActualizarProductosAsync(ref OperationResult pobjOperationResult, productoDto objEntidadModelo,
            IEnumerable<string> pListIds)
        {
            try
            {
                int i;
                var tarea = new Task(() =>
                {
                    lock (this)
                    {
                        using (var dbContext = new SAMBHSEntitiesModelWin())
                        {
                            foreach (var id in pListIds)
                            {
                                var entidadProducto = dbContext.producto.FirstOrDefault(p => p.v_IdProducto.Equals(id));
                                if (entidadProducto == null) continue;
                                if (objEntidadModelo.v_IdLinea != "-1" && objEntidadModelo.v_IdLinea != null)
                                    entidadProducto.v_IdLinea = objEntidadModelo.v_IdLinea;
                                if (objEntidadModelo.v_IdColor != "-1" && objEntidadModelo.v_IdColor != null)
                                    entidadProducto.v_IdColor = objEntidadModelo.v_IdColor;
                                if (objEntidadModelo.b_Foto != null)
                                    entidadProducto.b_Foto = objEntidadModelo.b_Foto;
                                if (objEntidadModelo.d_Empaque != 0)
                                    entidadProducto.d_Empaque = objEntidadModelo.d_Empaque;
                                if (!string.IsNullOrWhiteSpace(objEntidadModelo.v_Caracteristica))
                                    entidadProducto.v_Caracteristica = objEntidadModelo.v_Caracteristica;
                                if (objEntidadModelo.v_IdMarca != "-1" && objEntidadModelo.v_IdMarca != null)
                                    entidadProducto.v_IdMarca = objEntidadModelo.v_IdMarca;
                                if (objEntidadModelo.v_IdTalla != "-1" && objEntidadModelo.v_IdTalla != null)
                                    entidadProducto.v_IdTalla = objEntidadModelo.v_IdTalla;
                                if (objEntidadModelo.d_Peso != 0)
                                    entidadProducto.d_Peso = objEntidadModelo.d_Peso;
                                if (!string.IsNullOrWhiteSpace(objEntidadModelo.v_Ubicacion))
                                    entidadProducto.v_Ubicacion = objEntidadModelo.v_Ubicacion;
                                if (objEntidadModelo.i_EsActivo != null)
                                    entidadProducto.i_EsActivo = objEntidadModelo.i_EsActivo;
                                if (objEntidadModelo.i_ValidarStock != null)
                                    entidadProducto.i_ValidarStock = objEntidadModelo.i_ValidarStock;
                                if (objEntidadModelo.i_EsServicio != null)
                                    entidadProducto.i_EsServicio = objEntidadModelo.i_EsServicio;
                                if (objEntidadModelo.i_IdUnidadMedida != -1)
                                    entidadProducto.i_IdUnidadMedida = objEntidadModelo.i_IdUnidadMedida;
                                if (objEntidadModelo.v_Modelo != "-1" && objEntidadModelo.v_Modelo != null)
                                    entidadProducto.v_Modelo = objEntidadModelo.v_Modelo;
                                if (objEntidadModelo.i_IdTipoProducto != -1)
                                    entidadProducto.i_IdTipoProducto = objEntidadModelo.i_IdTipoProducto;
                                if (objEntidadModelo.i_EsAfectoPercepcion != null)
                                    entidadProducto.i_EsAfectoPercepcion = objEntidadModelo.i_EsAfectoPercepcion;
                                if (objEntidadModelo.i_EsAfectoDetraccion != null)
                                    entidadProducto.i_EsAfectoDetraccion = objEntidadModelo.i_EsAfectoDetraccion;

                                if (objEntidadModelo.i_CantidadFabricacionMensual != null)
                                    entidadProducto.i_CantidadFabricacionMensual = objEntidadModelo.i_CantidadFabricacionMensual;
                                dbContext.producto.ApplyCurrentValues(entidadProducto);
                            }
                            i = dbContext.SaveChanges();
                            if (OnProcesoTerminado != null)
                                OnProcesoTerminado(i, null);
                        }
                    }
                });

                tarea.Start();
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ActualizarProductos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                if (OnProcesoTerminado != null)
                    OnProcesoTerminado(0, pobjOperationResult);
            }
        }

        public string DevolverTipoCambioPorFecha(ref OperationResult pobjOperationResult, DateTime Fecha)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
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


                    //return string.Empty;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.DevolverTipoCambioPorFecha()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        public bool MovimientosProducto(ref OperationResult pobjOperationResult, string pstrProductoId)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {

                var movimientos = (from a in dbContext.producto
                                 join b in dbContext.productodetalle on new { p = a.v_IdProducto, eliminado = 0 } equals
                                 new { p = b.v_IdProducto, eliminado = b.i_Eliminado.Value } into b_join

                                 from b in b_join.DefaultIfEmpty()

                                 join c in dbContext.movimientodetalle on new { pd = b.v_IdProductoDetalle, eliminado = 0 } equals
                                 new { pd = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join

                                 from c in c_join.DefaultIfEmpty()

                                 where a.v_IdProducto == pstrProductoId && a.i_Eliminado == 0

                                 select c).Count(r => r != null);

                var pedidos = (from a in dbContext.producto

                                 join b in dbContext.productodetalle on new { p = a.v_IdProducto, eliminado = 0 } equals
                                 new { p = b.v_IdProducto, eliminado = b.i_Eliminado.Value } into b_join

                                 from b in b_join.DefaultIfEmpty()

                                 join c in dbContext.pedidodetalle on new { pd = b.v_IdProductoDetalle, eliminado = 0 } equals
                                 new { pd = c.v_IdProductoDetalle, eliminado = c.i_Eliminado.Value } into c_join

                                 from c in c_join.DefaultIfEmpty()
                                 where a.v_IdProducto == pstrProductoId && a.i_Eliminado == 0
                                 select c).Count(r => r != null);

                var productoAlmacen = (from a in dbContext.productoalmacen

                                       join b in dbContext.productodetalle on new { pa = a.v_IdProductoAlmacen, eliminado = 0 } equals
                                       new { pa = b.v_IdProductoDetalle, eliminado = b.i_Eliminado.Value } into b_join

                                       from b in b_join.DefaultIfEmpty()
                                       join c in dbContext.producto on new { p = b.v_IdProducto, eliminado = 0 } equals
                                       new { p = c.v_IdProducto, eliminado = c.i_Eliminado.Value } into c_join
                                       from c in c_join.DefaultIfEmpty()
                                       where c.v_IdProducto == pstrProductoId && a.i_Eliminado == 0
                                       select c).Count(r => r != null);
                return movimientos > 0 || pedidos > 0 || productoAlmacen > 0;
            }
        }

        public List<KeyValueDTO> DevolverProductos()
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                List<KeyValueDTO> query = (from n in dbContext.producto

                                           join D in dbContext.productodetalle on n.v_IdProducto equals D.v_IdProducto into D_join
                                           from D in D_join.DefaultIfEmpty()
                                           where n.i_Eliminado == 0
                                           select new
                                           {
                                               Value1 = n.v_CodInterno,
                                               Value2 = D.v_IdProductoDetalle,
                                               Value3 = n.v_Descripcion,
                                               Value5 = n.i_IdUnidadMedida,
                                               Estado = n.i_EsServicio ?? 0,
                                               Value6 = n.d_Empaque ?? 0
                                           }
                    ).ToList().Select
                    (p => new KeyValueDTO
                    {
                        Value1 = p.Value1,
                        Value2 = p.Value2,
                        Value3 = p.Value3,
                        Value5 = p.Value5 != null ? p.Value5.Value.ToString() : "-1",
                        Estado = p.Estado,
                        Value6 = p.Value6
                    }).ToList();

                return query.GroupBy(g => g.Value1).Select(p => p.FirstOrDefault()).ToList();
            }
        }

        public int CalcularMontoVendidoPorMes(int idMes, string pstrIdProducto)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var montoSaldoInicial =
                        dbContext.movimientodetalle.Where(
                            p =>
                                p.movimiento.i_IdTipoMovimiento == 1 && p.i_Eliminado == 0 && p.movimiento.t_Fecha.Value.Month == idMes &&
                                p.movimiento.i_IdTipoMotivo.Value == 5 && p.v_IdProductoDetalle == pstrIdProducto).Sum(o => o.d_CantidadEmpaque.Value);

                    var montoCompras =
                        dbContext.compradetalle.Where(
                            p =>
                                p.compra.i_IdEstado == 1 && p.i_Eliminado == 0 &&
                                p.compra.t_FechaEmision.Value.Month == idMes && p.v_IdProductoDetalle == pstrIdProducto).Sum(o => o.d_CantidadEmpaque.Value);

                    return (int)(montoSaldoInicial + montoCompras);
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Retorna verdadero cuando el producto detalle es servicio.
        /// </summary>
        /// <param name="pstrIdProductoDetalle"></param>
        /// <returns></returns>
        public bool ProductoDetalleEsServicio(string pstrIdProductoDetalle)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var result = dbContext.productodetalle.FirstOrDefault(p => p.v_IdProductoDetalle == pstrIdProductoDetalle);
                if (result != null) return result.producto.i_EsServicio == 1;
                return false;
            }
        }

        /// <summary>
        /// Determina si existe algun Producto que cumple con el Filtro.
        /// </summary>
        /// <param name="filter">Consulta a ejecutar</param>
        /// <returns>True si existe algun producto.</returns>
        public bool ExistAnyProduct(string filter)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var result = dbContext.producto.Select(p => p).Where(filter).ToList();

                    return result.Any();
                }
            }
            catch { }
            return true;
        }
        #region ProductoDetalle
        public productodetalleDto ObtenerProductoDetalle(ref OperationResult pobjOperationResult, string pstrIdProductoDetalle)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    productodetalleDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.productodetalle
                                     where a.v_IdProductoDetalle == pstrIdProductoDetalle
                                     select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = productodetalleAssembler.ToDTO(objEntity);

                    pobjOperationResult.Success = 1;

                    return objDtoEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ObtenerProductoDetalle()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }
        #endregion

        #region Kardex


        #endregion

        #region Reportes

        public List<ReporteProducto> ReporteProducto(string pstrv_IdProducto, string pstrv_IdLinea, string pstrt_Orden, string pstr_grupollave)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Query
                    var query =
                            (from A in dbContext.producto

                             join B in dbContext.linea on new { IdLinea = A.v_IdLinea, eliminado = 0 } equals new { IdLinea = B.v_IdLinea, eliminado = B.i_Eliminado.Value } into B_join
                             from B in B_join.DefaultIfEmpty()
                             join C in dbContext.datahierarchy on new { Item = A.i_IdUnidadMedida.Value, eliminado = 0, Grupo = 17 } equals new { Item = C.i_ItemId, eliminado = C.i_IsDeleted.Value, Grupo = C.i_GroupId }
                             where A.i_Eliminado == 0 && (A.v_CodInterno == pstrv_IdProducto || pstrv_IdProducto == "")
                             && (B.v_IdLinea == pstrv_IdLinea || pstrv_IdLinea == "-1")

                             select new ReporteProducto
                             {
                                 IdProducto = A.v_CodInterno,
                                 NombreProducto = A.v_Descripcion,
                                 NombreLinea = B == null ? "** NO EXISTE LINEA **" : B.v_Nombre,
                                 Caracteristicas = "",
                                 UnidadMedida = C == null ? "** U.M. NO EXISTE **" : C.v_Value1,
                                 Grupollave = pstr_grupollave == "LINEA" ? B == null ? "** LINEA NO EXISTE **" : "LINEA : " + B.v_Nombre : "",

                             });
                    #endregion
                    query = query.OrderBy(pstrt_Orden);
                    return query.ToList();
                }

            }
            catch (Exception ex)
            {

                return null;
            }
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

        public List<ReporteProducto> ReporteEmpresa()
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    #region Query
                    var query =
                            (from A in dbContext.nodewarehouse


                             select new ReporteProducto
                             {
                                 NombreEmpresaPropietaria = "",

                             });
                    #endregion
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                //pobjOperationResult.Success = 0;
                //pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }
        public List<ReporteUnidadMedida> ReporteUnidadMedida(string pstrtOrden)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    #region Query
                    var query =
                            (from A in dbContext.datahierarchy

                             where A.i_IsDeleted == 0 && A.i_GroupId == 17

                             select new ReporteUnidadMedida
                             {
                                 IdUnidadMedida = A.i_ItemId,
                                 UnidadMedida = A.v_Value1,

                             });
                    #endregion
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        public void ActualizarListaPreciosUtilidadMigracion(ref OperationResult pobjOperationResult, int pintIdListaPrecios, List<ProductoListaUtilidadDto> pobjListaUtilidadDtos, ProductoListaTipo penumProductoListaTipo)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (penumProductoListaTipo != ProductoListaTipo.Costo)
                        {
                            var listaprecio = dbContext.listaprecio.FirstOrDefault(p => p.i_IdLista == pintIdListaPrecios);

                            if (listaprecio != null)
                            {
                                var idListaPrecios = listaprecio.v_IdListaPrecios;

                                var listaPreciosDetalle =
                                    dbContext.listapreciodetalle.Where(
                                        p => p.v_IdListaPrecios == idListaPrecios && p.i_Eliminado == 0).ToList();

                                foreach (var precioUtilidad in pobjListaUtilidadDtos)
                                {
                                    var listaPrecio =
                                        listaPreciosDetalle.FirstOrDefault(
                                            p =>
                                                p.productodetalle.producto.v_CodInterno.Trim() ==
                                                precioUtilidad.CodProducto.Trim());



                                    if (listaPrecio == null) continue;

                                    if (penumProductoListaTipo == ProductoListaTipo.Utilidad)
                                        listaPrecio.d_Utilidad = precioUtilidad.Monto;
                                    else
                                        if (penumProductoListaTipo == ProductoListaTipo.Descuento)
                                            listaPrecio.d_Descuento = precioUtilidad.Monto;
                                        else
                                        {
                                            listaPrecio.d_Precio = Utils.Windows.DevuelveValorRedondeado(precioUtilidad.Monto, (int)Globals.ClientSession.i_PrecioDecimales);
                                            listaPrecio.d_PrecioMinSoles = listaPrecio.d_Precio;
                                            listaPrecio.d_PrecioMinDolares = listaPrecio.d_PrecioMinSoles / decimal.Parse("3.2");
                                            listaPrecio.d_Utilidad = listaPrecio.d_Costo != 0 ? ((listaPrecio.d_Precio - listaPrecio.d_Costo) / listaPrecio.d_Costo) * 100 : listaPrecio.d_Costo;

                                        }







                                }
                            }
                        }
                        else
                        {
                            var tempProductos = dbContext.producto.Where(p => p.i_Eliminado == 0);
                            foreach (var precioUtilidad in pobjListaUtilidadDtos)
                            {
                                var producto = tempProductos.FirstOrDefault(p => p.v_CodInterno.Trim() == precioUtilidad.CodProducto.Trim());

                                if (producto == null) continue;
                                producto.d_PrecioCosto = precioUtilidad.Monto;
                                dbContext.producto.ApplyCurrentValues(producto);
                            }
                        }

                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ActualizarListaPreciosUtilidadMigracion()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }




        public string ExisteListaPreciosPorAlmacen(List<ProductoBL.ProductoListaUtilidadDto> ListaProductos)
        {
            string Mensaje = "";
            var ListaPrecios = ListaProductos.GroupBy(o => new { IdAlmacen = o.IdAlmacen, IdLista = o.IdLista }).Select
                (o => o.FirstOrDefault()).ToList();
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var ListaPreciosExistentes = dbContext.listaprecio.ToList().Where(o => o.i_Eliminado == 0).Select(o => new { key = o.i_IdLista.ToString() + " " + o.i_IdAlmacen }).ToList();
                foreach (var lp in ListaPrecios)
                {

                    if (ListaPreciosExistentes.Select(o => o.key).Contains(lp.IdLista + " " + lp.IdAlmacen))
                    {
                    }
                    else
                    {
                        Mensaje = Mensaje + " " + "No existe Tarifario asignada al almacén : " + new AlmacenBL().ReporteAlmacen(lp.IdAlmacen).FirstOrDefault().v_Nombre + "\n";
                    }
                }
            }
            return Mensaje;

        }


        public class ProductoListaUtilidadDto
        {
            public string CodProducto { get; set; }
            public decimal Monto { get; set; }
            public int IdAlmacen { get; set; }
            public int IdLista { get; set; }
        }

        public enum ProductoListaTipo
        {
            Utilidad,
            Descuento,
            Costo,
            Precio

        }

        #region RecetaSalida
        public List<KeyValueDtoImage> GetProductsRecetaSalida(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.producto

                                 join d in dbContext.productodetalle on n.v_IdProducto equals d.v_IdProducto into dJoin
                                 from d in dJoin.DefaultIfEmpty()

                                 join j1 in dbContext.datahierarchy on new { e = 0, g = 6, id = n.i_IdTipoProducto.Value }
                                                                    equals new { e = j1.i_IsDeleted.Value, g = j1.i_GroupId, id = j1.i_ItemId } into j1_join
                                 from j1 in j1_join.DefaultIfEmpty()

                                 where n.i_Eliminado == 0 && n.i_EsActivoFijo == 0 && (n.i_IdTipoTributo == 1 || n.i_IdTipoTributo == 2)
                                 select new KeyValueDtoImage
                                 {
                                     Id = d.v_IdProductoDetalle,
                                     Value1 = n.v_CodInterno,
                                     Value2 = n.v_Descripcion,
                                     Imagen = n.b_Foto
                                 });
                    pobjOperationResult.Success = 1;
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public BindingList<productorecetasalidaDto> ObtenerRecetaSalida(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, string pstrIdProducto)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.productorecetasalida

                                 join C in dbContext.productodetalle on new { pd = n.v_IdProductoTributo, eliminado = 0 } equals new { pd = C.v_IdProductoDetalle, eliminado = C.i_Eliminado.Value } into C_join
                                 from C in C_join.DefaultIfEmpty()

                                 join D in dbContext.producto on C.v_IdProducto equals D.v_IdProducto into D_join
                                 from D in D_join.DefaultIfEmpty()

                                 join J1 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                               equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                                 from J1 in J1_join.DefaultIfEmpty()

                                 join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                               equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                 from J2 in J2_join.DefaultIfEmpty()
                                 where n.i_Eliminado == 0 && n.v_IdProductoExportacion == pstrIdProducto

                                 select new productorecetasalidaDto
                                 {
                                     i_IdRecetaSalida = n.i_IdRecetaSalida,
                                     v_IdProductoExportacion = n.v_IdProductoExportacion,
                                     v_IdProductoTributo = n.v_IdProductoTributo,
                                     v_Descripcion = D.v_Descripcion,
                                     v_CodInterno = D.v_CodInterno,
                                     i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                     i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                     t_InsertaFecha = n.t_InsertaFecha,
                                     t_ActualizaFecha = n.t_ActualizaFecha,
                                     v_UsuarioCreacion = J1.v_UserName,
                                     v_UsuarioModificacion = J2.v_UserName,
                                     i_Eliminado = n.i_Eliminado,

                                 }
                                        );

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<productorecetasalidaDto> objData = query.ToList();
                    var res = new BindingList<productorecetasalidaDto>(objData);
                    pobjOperationResult.Success = 1;
                    return res;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public void InsertarRecetaSalida(ref OperationResult pobjOperationResult, productorecetasalidaDto objProdReceta, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity = objProdReceta.ToEntity();
                    entity.t_InsertaFecha = DateTime.Now;
                    entity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    entity.i_Eliminado = 0;
                    dbContext.AddToproductorecetasalida(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.InsertarRecetaSalida()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void EliminarRecetaSalidaById(ref OperationResult pobjOperationResult, int pintIdReceta, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var obj = (from n in dbContext.productorecetasalida
                               where n.i_IdRecetaSalida == pintIdReceta
                               select n).FirstOrDefault();
                    obj.t_ActualizaFecha = DateTime.Now;
                    obj.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    obj.i_Eliminado = 1;
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.EliminarRecetaById()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }




        public void ActualizaRecetaSalida(ref OperationResult pobjOperationResult, productorecetasalidaDto objProdReceta, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntitySource = (from a in dbContext.productorecetasalida
                                           where a.i_IdRecetaSalida == objProdReceta.i_IdRecetaSalida
                                           select a).FirstOrDefault();
                    var entity = objProdReceta.ToEntity();
                    entity.t_ActualizaFecha = DateTime.Now;
                    entity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    dbContext.productorecetasalida.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ActualizaRecetaSalida()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }


        #endregion

        #region Receta
        /// <summary>
        /// Obtiene datos importantes de producto, para Recetas
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <returns></returns>
        public List<KeyValueDtoImage> GetProductsReceta(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.producto

                                 join d in dbContext.productodetalle on n.v_IdProducto equals d.v_IdProducto into dJoin
                                 from d in dJoin.DefaultIfEmpty()

                                 join j1 in dbContext.datahierarchy on new { e = 0, g = 6, id = n.i_IdTipoProducto.Value }
                                                                    equals new { e = j1.i_IsDeleted.Value, g = j1.i_GroupId, id = j1.i_ItemId } into j1_join
                                 from j1 in j1_join.DefaultIfEmpty()

                                 where n.i_Eliminado == 0 && n.i_EsActivoFijo == 0 && !j1.v_Value1.Contains("TERMINADO")
                                 select new KeyValueDtoImage
                                 {
                                     Id = d.v_IdProductoDetalle,
                                     Value1 = n.v_CodInterno,
                                     Value2 = n.v_Descripcion,
                                     Imagen = n.b_Foto
                                 });
                    pobjOperationResult.Success = 1;
                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }


        public BindingList<productorecetaDto> GetRecetasByCodeProduct(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, string pstrIdProducto)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.productoreceta

                                 join C in dbContext.productodetalle on n.v_IdProdInsumo equals C.v_IdProductoDetalle into C_join
                                 from C in C_join.DefaultIfEmpty()

                                 join D in dbContext.producto on C.v_IdProducto equals D.v_IdProducto into D_join
                                 from D in D_join.DefaultIfEmpty()

                                 join J1 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                               equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                                 from J1 in J1_join.DefaultIfEmpty()

                                 join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                               equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                 from J2 in J2_join.DefaultIfEmpty()
                                 where n.i_Eliminado == 0 && n.v_IdProdTerminado == pstrIdProducto

                                 select new productorecetaDto
                                 {
                                     i_IdReceta = n.i_IdReceta,
                                     v_IdProdTerminado = n.v_IdProdTerminado,
                                     v_IdProdInsumo = n.v_IdProdInsumo,
                                     v_Observacion = n.v_Observacion,
                                     d_Cantidad = n.d_Cantidad,
                                     NombreInsumo = D.v_Descripcion,
                                     CodInternoInsumo = D.v_CodInterno,
                                     i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                     i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                     t_InsertaFecha = n.t_InsertaFecha,
                                     t_ActualizaFecha = n.t_ActualizaFecha,
                                     v_UsuarioCreacion = J1.v_UserName,
                                     v_UsuarioModificacion = J2.v_UserName,
                                     i_Eliminado = n.i_Eliminado,
                                     Foto = D.b_Foto,
                                     i_IdAlmacen = n.i_IdAlmacen ?? -1
                                 }
                                        );

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<productorecetaDto> objData = query.ToList();
                    var res = new BindingList<productorecetaDto>(objData);
                    pobjOperationResult.Success = 1;
                    return res;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }


        public void InsertarReceta(ref OperationResult pobjOperationResult, productorecetaDto objProdReceta, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var entity = objProdReceta.ToEntity();
                    entity.t_InsertaFecha = DateTime.Now;
                    entity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    entity.i_Eliminado = 0;
                    dbContext.AddToproductoreceta(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.InsertarReceta()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }


        public void ActualizaReceta(ref OperationResult pobjOperationResult, productorecetaDto objProdReceta, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntitySource = (from a in dbContext.productoreceta
                                           where a.i_IdReceta == objProdReceta.i_IdReceta
                                           select a).FirstOrDefault();
                    var entity = objProdReceta.ToEntity();
                    entity.t_ActualizaFecha = DateTime.Now;
                    entity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    dbContext.productoreceta.ApplyCurrentValues(entity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ActualizaReceta()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        public void EliminarRecetaById(ref OperationResult pobjOperationResult, int pintIdReceta, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var obj = (from n in dbContext.productoreceta
                               where n.i_IdReceta == pintIdReceta
                               select n).FirstOrDefault();
                    obj.t_ActualizaFecha = DateTime.Now;
                    obj.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    obj.i_Eliminado = 1;
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.EliminarRecetaById()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }




        #endregion

        #region Receta Final
        /// <summary>
        /// Obtiene la receta del producto y le saca una copia para ser guardada en la tabla de movimientodetallerecetafinal previa modificacion del usuario.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdProductoDetalle"></param>
        /// <returns></returns>
        public List<movimientodetallerecetafinalDto> ConvertirRecetaARecetaFinal(ref OperationResult pobjOperationResult, string pstrIdProductoDetalle)
        {
            try
            {
                var receta = GetRecetasByCodeProduct(ref pobjOperationResult, null, null, pstrIdProductoDetalle);
                var recetaFinal = receta.Select(p => new movimientodetallerecetafinalDto
                {
                    v_IdProdTerminado = p.v_IdProdTerminado,
                    v_IdProdInsumo = p.v_IdProdInsumo,
                    d_Cantidad = p.d_Cantidad.Value,
                    CodigoInsumo = p.CodInternoInsumo,
                    NombreInsumo = p.NombreInsumo,
                    Foto = p.Foto,
                    i_IdAlmacen = p.i_IdAlmacen ?? 1
                }).ToList();

                return recetaFinal;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ConvertirRecetaARecetaFinal()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Inserta el conjunto de recetas finales previa revision del usuario y/o modificacion.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="objEntityList"></param>
        /// <param name="ClientSession"></param>
        /// <param name="pstrIdMovimiento"></param>
        public void InsertaRecetaFinalLista(ref OperationResult pobjOperationResult, List<movimientodetallerecetafinalDto> objEntityList, List<string> ClientSession, string pstrIdMovimiento)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        foreach (var objEntity in objEntityList)
                        {
                            var objSecuentialBL = new SecuentialBL();
                            var entidad = objEntity.ToEntity();
                            entidad.v_IdMovimiento = pstrIdMovimiento;
                            entidad.t_InsertaFecha = DateTime.Now;
                            entidad.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            entidad.i_Eliminado = 0;
                            var newId = Utils.GetNewId(int.Parse(ClientSession[0]), objSecuentialBL.GetNextSecuentialId(int.Parse(ClientSession[0]), 106), "RF");
                            entidad.v_IdRecetaFinal = newId;
                            dbContext.AddTomovimientodetallerecetafinal(entidad);
                        }

                        dbContext.SaveChanges();

                        InsertarMovimientoInsumos(ref pobjOperationResult, pstrIdMovimiento);
                        if (pobjOperationResult.Success == 0) return;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.InsertaRecetaFinalLista()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Actualiza las recetas al modificar la nota de ingreso.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="objEntityListInsert"></param>
        /// <param name="objEntityListUpdate"></param>
        /// <param name="objEntityListDelete"></param>
        /// <param name="ClientSession"></param>
        /// <param name="pstrIdMovimiento"></param>
        public void ActualizarRecetaFinalLista(ref OperationResult pobjOperationResult, List<movimientodetallerecetafinalDto> objEntityListInsert, List<movimientodetallerecetafinalDto> objEntityListUpdate, List<movimientodetallerecetafinalDto> objEntityListDelete, List<string> ClientSession, string pstrIdMovimiento)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        #region Insertar Detalles
                        foreach (var objEntity in objEntityListInsert)
                        {
                            SecuentialBL objSecuentialBL = new SecuentialBL();
                            var entidad = objEntity.ToEntity();
                            entidad.v_IdMovimiento = pstrIdMovimiento;
                            entidad.t_InsertaFecha = DateTime.Now;
                            entidad.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            entidad.i_Eliminado = 0;
                            var newId = Utils.GetNewId(int.Parse(ClientSession[0]), objSecuentialBL.GetNextSecuentialId(int.Parse(ClientSession[0]), 106), "RF");
                            entidad.v_IdRecetaFinal = newId;
                            dbContext.AddTomovimientodetallerecetafinal(entidad);
                        }
                        #endregion

                        #region Actualizar Detalles
                        foreach (var objEntity in objEntityListUpdate)
                        {
                            var entidad = dbContext.movimientodetallerecetafinal.FirstOrDefault(p => p.v_IdRecetaFinal.Equals(objEntity.v_IdRecetaFinal));
                            var entindadRef = objEntity.ToEntity();
                            entindadRef.t_ActualizaFecha = DateTime.Now;
                            entindadRef.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            dbContext.movimientodetallerecetafinal.ApplyCurrentValues(entindadRef);
                        }
                        #endregion

                        #region Eliminar Detalles
                        foreach (var objEntity in objEntityListDelete)
                        {
                            var entidad = dbContext.movimientodetallerecetafinal.FirstOrDefault(p => p.v_IdRecetaFinal.Equals(objEntity.v_IdRecetaFinal));
                            var entindadRef = objEntity.ToEntity();
                            entindadRef.t_ActualizaFecha = DateTime.Now;
                            entindadRef.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            entindadRef.i_Eliminado = 1;
                            dbContext.movimientodetallerecetafinal.ApplyCurrentValues(entindadRef);
                        }
                        #endregion

                        dbContext.SaveChanges();

                        ReHacerMovimientoInsumos(ref pobjOperationResult, pstrIdMovimiento);
                        if (pobjOperationResult.Success == 0) return;

                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.InsertaRecetaFinalLista()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Al eliminar la nota de ingreso, se elimina tambien las recetas finales y se elimina tambien la nota de salida.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdMovimiento"></param>
        public void EliminarRecetaMovimiento(ref OperationResult pobjOperationResult, string pstrIdMovimiento)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var listaEliminar =
                            dbContext.movimientodetallerecetafinal.Where(
                                p => p.v_IdMovimiento.Equals(pstrIdMovimiento) && p.i_Eliminado.Value.Equals(0)).ToList();

                        if (listaEliminar.Any())
                        {
                            foreach (var movimientodetallerecetafinal in listaEliminar)
                            {
                                movimientodetallerecetafinal.i_Eliminado = 1;
                                dbContext.movimientodetallerecetafinal.ApplyCurrentValues(movimientodetallerecetafinal);
                            }

                            dbContext.SaveChanges();
                        }

                        RevertirMovimientoInsumos(ref pobjOperationResult, pstrIdMovimiento);
                        if (pobjOperationResult.Success == 0) return;

                        ts.Complete();
                        pobjOperationResult.Success = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.EliminarRecetaMovimiento()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Genera la nota de salida con los insumos de los productos terminados de la nota de ingreso.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdMovimiento"></param>
        public void InsertarMovimientoInsumos(ref OperationResult pobjOperationResult, string pstrIdMovimiento)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    var rucEmpresa = Globals.ClientSession.v_RucEmpresa;
                    var idCliente = DevuelveIdCliente(rucEmpresa);

                    if (!string.IsNullOrEmpty(idCliente))
                    {
                        using (var dbContext = new SAMBHSEntitiesModelWin())
                        {
                            var movimientoOrigen = dbContext.movimiento.FirstOrDefault(p => p.v_IdMovimiento.Equals(pstrIdMovimiento));
                            var recetaFinalList = dbContext.movimientodetallerecetafinal.Where(p => p.v_IdMovimiento.Equals(pstrIdMovimiento) && p.i_Eliminado == 0).ToList();
                            var pAlmacenes = recetaFinalList.Where(o => o.i_IdAlmacen.HasValue).Select(p => p.i_IdAlmacen.Value).Distinct().ToList();

                            if (movimientoOrigen != null && pAlmacenes.Any())
                            {
                                var listaMovimientos = new MovimientoBL().ObtenerListadoMovimientos(ref pobjOperationResult,
                                    movimientoOrigen.v_Periodo, movimientoOrigen.v_Mes, (int)TipoDeMovimiento.NotadeSalida);

                                var maxMovimiento = listaMovimientos.Any()
                                    ? int.Parse(listaMovimientos[listaMovimientos.Count - 1].Value1)
                                    : 0;

                                #region Genera e inserta las notas de salida.
                                foreach (int pAlmacen in pAlmacenes)
                                {
                                    var movimientoDto = new movimientoDto();
                                    maxMovimiento++;
                                    movimientoDto.d_TipoCambio = movimientoOrigen.d_TipoCambio;
                                    movimientoDto.i_IdAlmacenOrigen = pAlmacen;
                                    movimientoDto.i_IdMoneda = movimientoOrigen.i_IdMoneda;
                                    movimientoDto.i_IdTipoMotivo = 9;
                                    movimientoDto.t_Fecha = movimientoOrigen.t_Fecha;
                                    movimientoDto.v_Mes = movimientoOrigen.v_Mes.Trim();
                                    movimientoDto.v_Periodo = movimientoOrigen.v_Periodo.Trim();
                                    movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeSalida;
                                    movimientoDto.v_Correlativo = maxMovimiento.ToString("00000000");
                                    movimientoDto.v_IdCliente = idCliente;
                                    movimientoDto.i_EsDevolucion = 0;
                                    movimientoDto.v_IdMovimientoOrigen = movimientoOrigen.v_IdMovimiento;
                                    movimientoDto.d_TotalPrecio = movimientoOrigen.d_TotalPrecio;

                                    if (movimientoOrigen.i_IdEstablecimiento != null)
                                        movimientoDto.i_IdEstablecimiento = movimientoOrigen.i_IdEstablecimiento.Value;

                                    var movimientoIngresoDetalle = recetaFinalList
                                        .Where(o => o.i_IdAlmacen.HasValue && o.i_IdAlmacen == pAlmacen)
                                        .Select(_movimientodetalleDto => new movimientodetalleDto
                                        {
                                            v_IdProductoDetalle = _movimientodetalleDto.v_IdProdInsumo,
                                            i_IdTipoDocumento = -1,
                                            v_NumeroDocumento = string.Empty,
                                            d_Cantidad = _movimientodetalleDto.d_Cantidad,
                                            i_IdUnidad = 15,
                                            d_Precio = 0,
                                            d_Total = 0,
                                            d_CantidadEmpaque = _movimientodetalleDto.d_Cantidad,
                                            v_NroPedido = string.Empty,

                                        }).ToList();

                                    decimal sum = movimientoIngresoDetalle.Sum(p => p.d_Cantidad.Value);
                                    movimientoDto.d_TotalCantidad = sum;
                                    movimientoDto.d_TotalPrecio = 0;
                                    new MovimientoBL().InsertarMovimiento(ref pobjOperationResult, movimientoDto,
                                        Globals.ClientSession.GetAsList(), movimientoIngresoDetalle);
                                    if (pobjOperationResult.Success == 0) return;
                                }
                                #endregion
                            }

                            ts.Complete();
                        }
                    }
                    else
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage =
                            "No se pudo crear un cliente para la nota de salida por insumos.";
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.InsertaRecetaFinalLista()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Elimina la nota de salida relacionada a la nota de ingreso.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdMovimiento"></param>
        public void RevertirMovimientoInsumos(ref OperationResult pobjOperationResult, string pstrIdMovimiento)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var notaSalidaRelacionada =
                            dbContext.movimiento.Where(p => p.v_IdMovimientoOrigen.Equals(pstrIdMovimiento)
                                    && p.i_Eliminado == 0 && p.i_IdTipoMovimiento.HasValue && p.i_IdTipoMovimiento.Value == 2).ToList();

                        if (notaSalidaRelacionada.Any())
                        {
                            foreach (var ns in notaSalidaRelacionada)
                            {
                                new MovimientoBL().EliminarMovimiento(ref pobjOperationResult, ns.v_IdMovimiento, Globals.ClientSession.GetAsList());
                                if (pobjOperationResult.Success == 0) return;
                            }
                        }

                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.RevertirMovimientoInsumos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Metodo utilizado al momento de editar la receta final, para mantener la nota de salida acorde a la receta final.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrIdMovimiento"></param>
        public void ReHacerMovimientoInsumos(ref OperationResult pobjOperationResult, string pstrIdMovimiento)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    RevertirMovimientoInsumos(ref pobjOperationResult, pstrIdMovimiento);
                    if (pobjOperationResult.Success == 1)
                    {
                        InsertarMovimientoInsumos(ref pobjOperationResult, pstrIdMovimiento);
                        if (pobjOperationResult.Success == 0)
                            return;
                    }
                    else
                        return;

                    pobjOperationResult.Success = 1;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.ReHacerMovimientoInsumos()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        /// <summary>
        /// Devuelve el el id del cliente de la empresa propietaria, si no existe la crea.
        /// </summary>
        /// <param name="pstrRucEmpresa"></param>
        /// <returns></returns>
        public static string DevuelveIdCliente(string pstrRucEmpresa)
        {
            var pobjOperationResult = new OperationResult();
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var clienteEmpresa =
                            dbContext.cliente.FirstOrDefault(
                                c =>
                                    c.v_FlagPantalla.Equals("C") && c.v_NroDocIdentificacion.Equals(pstrRucEmpresa) &&
                                    c.i_Eliminado == 0);

                    if (clienteEmpresa == null)
                    {
                        var empresaPropietaria = new NodeBL().ReporteEmpresa().FirstOrDefault();
                        var clienteEntity = dbContext.cliente.CreateObject();
                        clienteEntity.v_NroDocIdentificacion = pstrRucEmpresa;
                        clienteEntity.v_PrimerNombre = string.Empty;
                        clienteEntity.v_SegundoNombre = string.Empty;
                        clienteEntity.v_ApeMaterno = string.Empty;
                        clienteEntity.v_ApePaterno = string.Empty;
                        clienteEntity.v_RazonSocial = empresaPropietaria != null
                            ? empresaPropietaria.NombreEmpresaPropietaria : "EMPRESA PROPIETARIA NO REGISTRADA";
                        clienteEntity.v_CodCliente = pstrRucEmpresa;
                        clienteEntity.i_IdTipoPersona = 1;
                        clienteEntity.i_IdTipoIdentificacion = 6;
                        clienteEntity.i_UsaLineaCredito = 0;
                        clienteEntity.v_DirecPrincipal = empresaPropietaria != null ? empresaPropietaria.Direccion : string.Empty;
                        clienteEntity.v_DirecPrincipal = clienteEntity.v_DirecPrincipal.Length <= 200 ? clienteEntity.v_DirecPrincipal : clienteEntity.v_DirecPrincipal.Substring(0, 200);
                        clienteEntity.v_DirecSecundaria = string.Empty;
                        clienteEntity.i_IdPais = 1;
                        clienteEntity.i_IdDistrito = 1393;
                        clienteEntity.i_IdDepartamento = 1391;
                        clienteEntity.i_IdProvincia = 1392;
                        clienteEntity.i_Nacionalidad = 0;
                        clienteEntity.i_Activo = 1;
                        clienteEntity.i_IdSexo = 1;
                        clienteEntity.v_FlagPantalla = "C";

                        var lineacreditoempresaDto = new lineacreditoempresaDto
                        {
                            d_Acuenta = 0,
                            d_Credito = 0,
                            d_Saldo = 0,
                            i_IdMoneda = 1
                        };

                        clienteEntity.i_EsPrestadorServicios = 0;
                        return InsertarCliente(ref pobjOperationResult, clienteEntity.ToDTO(), Globals.ClientSession.GetAsList(), lineacreditoempresaDto, null, null, null, null, null);
                    }
                    return clienteEmpresa.v_IdCliente;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.DevuelveIdCliente()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        /// <summary>
        /// Inserta un cliente, usado para insertar a la empresa propietara como cliente, si ésta no existe como cliente.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pobjDtoEntity"></param>
        /// <param name="ClientSession"></param>
        /// <param name="_lineacreditoempresaDto"></param>
        /// <param name="pobjTrabajador"></param>
        /// <param name="AgregarListContratoDto"></param>
        /// <param name="AgregarListContratoDetalleDto"></param>
        /// <param name="AgregarRegimen"></param>
        /// <param name="ListaAgregarDH"></param>
        /// <param name="ListaAgregarAreas"></param>
        /// <returns></returns>
        private static string InsertarCliente(ref OperationResult pobjOperationResult, clienteDto pobjDtoEntity, List<string> ClientSession, lineacreditoempresaDto _lineacreditoempresaDto, trabajadorDto pobjTrabajador, List<contratotrabajadorDto> AgregarListContratoDto, List<contratodetalletrabajadorDto> AgregarListContratoDetalleDto, List<regimenpensionariotrabajadorDto> AgregarRegimen, List<derechohabientetrabajadorDto> ListaAgregarDH, List<areaslaboratrabajadorDto> ListaAgregarAreas = null)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        cliente objEntity = pobjDtoEntity.ToEntity();
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntity.i_Eliminado = 0;
                        int intNodeId = int.Parse(ClientSession[0]);
                        var SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 14);
                        var newIdCliente = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "CL");
                        objEntity.v_IdCliente = newIdCliente;
                        dbContext.AddTocliente(objEntity);
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "cliente", newIdCliente);
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();

                        return newIdCliente;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ProductoBL.InsertarCliente()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }
        #endregion

        #region Requerimientos


        #endregion

        #region Consultas para recalculostock

        public class ProductoFiltro
        {
            public string Id { get; set; }
            public string Codigo { get; set; }
            public string Descripcion { get; set; }
        }

        public BindingList<ProductoFiltro> ObtenerFiltrosProducto(List<string> lista = null)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var r = (from n in dbContext.producto
                         join j1 in dbContext.productodetalle on n.v_IdProducto equals j1.v_IdProducto
                         where n.i_Eliminado == 0
                         select new ProductoFiltro
                         {
                             Codigo = n.v_CodInterno,
                             Descripcion = n.v_Descripcion,
                             Id = j1.v_IdProductoDetalle
                         }).ToList();

                if (lista != null)
                    r = r.Where(o => lista.Contains(o.Id)).ToList();

                return new BindingList<ProductoFiltro>(r);
            }
        }

        #endregion
    }
}
