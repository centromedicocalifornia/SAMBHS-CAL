
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System.Linq.Dynamic;
using System.Data.Entity;
using SAMBHS.Common.BL;
using System.Transactions;
using SAMBHS.CommonWIN.BL;
using System.ComponentModel;
using System.Globalization;
using SAMBHS.Almacen.BL;
using SAMBHS.Tesoreria.BL;
using SAMBHS.Contabilidad.BL;
using System.Threading.Tasks;





namespace SAMBHS.ActivoFijo.BL
{
    public class ActivoFijoBL
    {
        #region Mantenimietos-Procesos
        private string PeriodoActual = Globals.ClientSession.i_Periodo.ToString();
        public List<ActivoFijoTipoActivoDto> ObtenerTiposActivoFijos(int pIntGrupo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {

                List<ActivoFijoTipoActivoDto> DatosDatahierarchy = (from datahierarchys in dbContext.datahierarchy

                                                                    where datahierarchys.i_IsDeleted == 0 && datahierarchys.i_GroupId == pIntGrupo

                                                                    select new ActivoFijoTipoActivoDto
                                                                    {

                                                                        Padre = datahierarchys.i_Header == 1 ? datahierarchys.v_Value2 + " | " + datahierarchys.v_Value1 : "",
                                                                        Nodos = datahierarchys.i_Header != 1 ? datahierarchys.v_Value2 + " | " + datahierarchys.v_Value1 : "",
                                                                        Codigo = datahierarchys.v_Value2,
                                                                        Orden = datahierarchys.i_Sort.Value,
                                                                    }).ToList();
                return DatosDatahierarchy;

            }



        }

        public string InsertaCuentasInventarios(ref OperationResult pobjOperationResult, cuentasinventariosDto pobjDtoEntity, List<string> ClientSession)
        {
            int SecuentialId = 0;
            string newId = string.Empty;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    cuentasinventarios objEntity = cuentasinventariosAssembler.ToEntity(pobjDtoEntity);
                    objEntity.t_InsertaFecha = DateTime.Now;
                    //  objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;
                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 68);
                    newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "CI");
                    objEntity.v_IdCuentaInventario = newId;
                    //  dbContext.AddToconcepto(objEntity);
                    dbContext.AddTocuentasinventarios(objEntity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "cuentasInventarios", newId);
                    return newId;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ActivoFijoBL.InsertarCuentasInventarios()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return "";
            }
        }

        public string ActualizarActivoFijo(ref OperationResult pobjOperationResult, cuentasinventariosDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.cuentasinventarios
                                           where a.v_IdCuentaInventario == pobjDtoEntity.v_IdCuentaInventario
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    cuentasinventarios objEntity = cuentasinventariosAssembler.ToEntity(pobjDtoEntity);


                    dbContext.cuentasinventarios.ApplyCurrentValues(objEntity);

                    // Guardar los cambios
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "CuentasInventarios", objEntitySource.v_IdCuentaInventario);
                    pobjOperationResult.Success = 1;
                    return pobjDtoEntity.v_IdCuentaInventario;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ActivoFijoBL.ActualizarActivoFijo()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return "";
            }
        }


        public void ActualizarNroRegistroActivoFijo(ref OperationResult objOperationResult)
        {

            try
            {
                //Task.Factory.StartNew(() =>
                //{
                //    lock (this)
                //    {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var pobjDtoEntities =
                            dbContext.activofijo.Where(p => p.i_Eliminado == 0 && p.i_EsTemporal == 0).OrderBy(o => o.v_IdActivoFijo).ToList();

                        var Productos = dbContext.producto.Where(o => o.i_Eliminado == 0 && o.i_EsActivoFijo == 1).ToList();
                        var ProductoDetalle = dbContext.productodetalle.Where(o => o.i_Eliminado == 0).ToList();
                        //var total = pobjDtoEntities.Count;
                        //var pos = 0;
                        int NumeroRegistro = 102;
                        foreach (var pobjDtoEntity in pobjDtoEntities)
                        {
                            pobjDtoEntity.v_CodigoActivoFijo = "10000" + NumeroRegistro.ToString();
                            dbContext.activofijo.ApplyCurrentValues(pobjDtoEntity);

                            var ProductoDet = ProductoDetalle.Where(o => o.v_IdProductoDetalle == pobjDtoEntity.v_IdProducto).FirstOrDefault();
                            var Prod = Productos.Where(o => o.v_IdProducto == ProductoDet.v_IdProducto).FirstOrDefault();
                            Prod.v_CodInterno = pobjDtoEntity.v_CodigoActivoFijo;
                            dbContext.producto.ApplyCurrentValues(Prod);
                            NumeroRegistro = NumeroRegistro + 1;

                        }

                        dbContext.SaveChanges();
                        ts.Complete();
                        objOperationResult.Success = 1;


                    }
                }
                //}

                //}, TaskCreationOptions.LongRunning);
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ActivoFijoBL.ActualizarNroRegistroActivoFijo()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);

            }
        }


        public BindingList<activofijoanexoDto> ObtenerActivoFijoAnexo(ref  OperationResult objOperationResult, string IdActivoFijo)
        {
            try
            {
                //  https://www.youtube.com/watch?v=yNiWQgtRUPw
                objOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var activosAnexos = (from a in dbContext.activofijoanexo


                                         where a.i_Eliminado == 0 && a.v_IdActivoFijo == IdActivoFijo

                                         select new activofijoanexoDto
                                         {
                                             i_IdTipoDocumento = a.i_IdTipoDocumento,
                                             v_NroDocumento = a.v_NroDocumento,
                                             t_FechaEmision = a.t_FechaEmision,
                                             v_Observaciones = a.v_Observaciones,
                                             b_Foto = a.b_Foto,
                                             i_Eliminado = a.i_Eliminado,
                                             t_InsertaFecha = a.t_InsertaFecha,
                                             t_ActualizaFecha = a.t_ActualizaFecha,
                                             v_IdActivoFijo = a.v_IdActivoFijo,
                                             v_UbicacionFoto = a.v_UbicacionFoto,
                                             i_IdActivoFijoAnexo =a.i_IdActivoFijoAnexo ,
                                             i_ActualizaIdUsuario =a.i_ActualizaIdUsuario ,
                                             i_InsertaIdUsuario =a.i_InsertaIdUsuario ,

                                         }).ToList();

                    return new BindingList<activofijoanexoDto>(activosAnexos);

                }



            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;

            }
        }

        public List<cuentasinventariosDto> ObtenerListadoCuentasInventarios(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from CuentasInventarios in dbContext.cuentasinventarios

                                join TipoActivo in dbContext.datahierarchy on new { TipoActivo = CuentasInventarios.i_IdTipoActivo.Value, eliminado = 0, Grupo = 104 }
                                                            equals new { TipoActivo = TipoActivo.i_ItemId, eliminado = TipoActivo.i_IsDeleted.Value, Grupo = TipoActivo.i_GroupId } into TipoActivo_join
                                from TipoActivo in TipoActivo_join.DefaultIfEmpty()

                                join Cuenta33 in dbContext.asientocontable on new { psCuenta33 = CuentasInventarios.v_Cuenta33, eliminado = 0, p = PeriodoActual }
                                                            equals new { psCuenta33 = Cuenta33.v_NroCuenta, eliminado = Cuenta33.i_Eliminado.Value, p = Cuenta33.v_Periodo } into Cuenta33_join
                                from Cuenta33 in Cuenta33_join.DefaultIfEmpty()

                                join Cuenta39 in dbContext.asientocontable on new { psCuenta39 = CuentasInventarios.v_Cuenta39, eliminado = 0, p = PeriodoActual }
                                                                            equals new { psCuenta39 = Cuenta39.v_NroCuenta, eliminado = Cuenta39.i_Eliminado.Value, p = Cuenta39.v_Periodo } into Cuenta39_join

                                from Cuenta39 in Cuenta39_join.DefaultIfEmpty()


                                join CuentaGasto in dbContext.asientocontable on new { psCuentaG = CuentasInventarios.v_CuentaGasto, eliminado = 0, p = PeriodoActual }
                                                                                 equals new { psCuentaG = CuentaGasto.v_NroCuenta, eliminado = CuentaGasto.i_Eliminado.Value, p = CuentaGasto.v_Periodo } into CuentaGasto_join

                                from CuentaGasto in CuentaGasto_join.DefaultIfEmpty()

                                //join UsuarioCreacion in dbContext.systemuser on new { i_InsertUserId = CuentasInventarios.i_InsertaIdUsuario.Value, eliminado = 0 }
                                //                        equals new { i_InsertUserId = J1.i_SystemUserId, eliminado = J1.i_IsDeleted.Value } into J1_join
                                //from J1 in J1_join.DefaultIfEmpty()

                                join UsuarioMod in dbContext.systemuser on new { i_UpdateUserId = CuentasInventarios.i_ActualizaIdUsuario.Value, eliminado = 0 }
                                                            equals new { i_UpdateUserId = UsuarioMod.i_SystemUserId, eliminado = UsuarioMod.i_IsDeleted.Value } into UsuarioMod_join
                                from UsuarioMod in UsuarioMod_join.DefaultIfEmpty()


                                where CuentasInventarios.i_Eliminado == 0

                                select new cuentasinventariosDto
                                {

                                    v_IdCuentaInventario = CuentasInventarios.v_IdCuentaInventario,
                                    v_Cuenta33 = Cuenta33 == null ? Cuenta33.v_NroCuenta + " | " + "**NO EXISTE EN EL PLAN CUENTAS**" : Cuenta33.v_NroCuenta + " | " + Cuenta33.v_NombreCuenta,
                                    v_Cuenta39 = Cuenta39 == null ? Cuenta39.v_NroCuenta + " | " + "**NO EXISTE EN EL PLAN CUENTAS**" : Cuenta39.v_NroCuenta + " | " + Cuenta39.v_NombreCuenta,
                                    v_CuentaGasto = CuentaGasto == null ? CuentaGasto.v_NroCuenta + " | " + "**NO EXISTE EN EL PLAN CUENTAS**" : CuentaGasto.v_NroCuenta + " | " + Cuenta33.v_NombreCuenta,
                                    DescripcionTipoActivo = TipoActivo == null ? "**NO EXISTE TIPO ACTIVO**" : TipoActivo.v_Value1,
                                    CodigoTipoActivo = TipoActivo == null ? "**NO EXISTE TIPO ACTIVO**" : TipoActivo.v_Value2,
                                    v_UsuarioModificacion = UsuarioMod.v_UserName,
                                    t_InsertaFecha = CuentasInventarios.t_InsertaFecha,
                                    t_ActualizaFecha = CuentasInventarios.t_ActualizaFecha,
                                    cuenta33 = Cuenta33 == null ? "**NO EXISTE**" : Cuenta33.v_NroCuenta,
                                    cuenta39 = Cuenta39 == null ? "**NO EXISTE**" : Cuenta39.v_NroCuenta,
                                    cuentagasto = CuentaGasto == null ? "**NO EXISTE**" : CuentaGasto.v_NroCuenta,
                                    i_Eliminado = CuentasInventarios.i_Eliminado.Value,
                                    i_IdTipoActivo = CuentasInventarios.i_IdTipoActivo,

                                };

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }


                    List<cuentasinventariosDto> objData = query.ToList();
                    pobjOperationResult.Success = 1;
                    return objData;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public int ObtenerTipoActivoFijo(string pstrCodigo, int pIntGrupo)
        {

            using (var dbContext = new SAMBHSEntitiesModelWin())
            {

                var TipoActivo = (from n in dbContext.datahierarchy

                                  where n.i_GroupId == pIntGrupo && n.i_IsDeleted == 0 && n.v_Value2 == pstrCodigo

                                  select new
                                  {

                                      i_ItemId = n.i_ItemId,

                                  }).FirstOrDefault();

                if (TipoActivo != null)
                {
                    return TipoActivo.i_ItemId;
                }
                else
                {
                    return -1;
                }
            }

        }

        public cuentasinventariosDto ObtenerCuentasInventarios(ref OperationResult pobjOperationResult, string pstrCuentasInventarios)
        {

            try
            {


                using (var dbContext = new SAMBHSEntitiesModelWin())
                {


                    var objEntity = (from cuentasInventarios in dbContext.cuentasinventarios
                                     join tipoActivo in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, pstrTipoActivo = cuentasInventarios.i_IdTipoActivo.Value }
                                                                                equals new { Grupo = tipoActivo.i_GroupId, eliminado = tipoActivo.i_IsDeleted.Value, pstrTipoActivo = tipoActivo.i_ItemId } into tipoActivo_join

                                     from tipoActivo in tipoActivo_join.DefaultIfEmpty()
                                     where cuentasInventarios.v_IdCuentaInventario == pstrCuentasInventarios && cuentasInventarios.i_Eliminado == 0
                                     select new cuentasinventariosDto
                                     {
                                         v_Cuenta33 = cuentasInventarios.v_Cuenta33,
                                         v_Cuenta39 = cuentasInventarios.v_Cuenta39,
                                         v_CuentaGasto = cuentasInventarios.v_CuentaGasto,
                                         CodigoTipoActivo = tipoActivo == null ? "**NO EXISTE**" : tipoActivo.v_Value2,
                                         v_IdCuentaInventario = cuentasInventarios.v_IdCuentaInventario,
                                         i_Eliminado = cuentasInventarios.i_Eliminado,
                                         t_InsertaFecha = cuentasInventarios.t_InsertaFecha,
                                         t_ActualizaFecha = cuentasInventarios.t_ActualizaFecha,
                                         i_ActualizaIdUsuario = cuentasInventarios.i_ActualizaIdUsuario,
                                         i_IdTipoActivo = tipoActivo.i_ItemId,


                                     }).FirstOrDefault();
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


        public void EliminarCuentasInventarios(ref OperationResult pobjOperationResult, string pstrIdCuentaInventario, List<string> ClientSession)
        {
            try
            {


                // Obtener la entidad fuente
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntitySource = (from a in dbContext.cuentasinventarios
                                           where a.v_IdCuentaInventario == pstrIdCuentaInventario
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;

                    // Guardar los cambios
                    dbContext.SaveChanges();
                }
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return;
            }
        }

        public List<datahierarchyDto> ObtenerExistencias(int IdGrupo, string pstrCodigo)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var Ubicaciones = (from ubicacion in dbContext.datahierarchy

                                   join datahierarchys in dbContext.datahierarchy on new { ItemId = ubicacion.i_ParentItemId.Value, groupId = ubicacion.i_GroupId }
                                                          equals new { ItemId = datahierarchys.i_ItemId, groupId = datahierarchys.i_GroupId } into datahierarchys_join
                                   from datahierarchys in datahierarchys_join.DefaultIfEmpty()


                                   where ubicacion.i_GroupId == IdGrupo && (ubicacion.i_IsDeleted == 0 || ubicacion.i_IsDeleted == null)
                                   && ubicacion.v_Value2 == pstrCodigo
                                   select new datahierarchyDto
                                   {
                                       i_ItemId = ubicacion.i_ItemId,
                                       v_Value1 = ubicacion.v_Value1,
                                       v_Value2 = ubicacion.v_Value2,
                                       i_GroupId = ubicacion.i_GroupId,

                                   }).ToList();
                return Ubicaciones;



                //var query = from A in dbContext.datahierarchy

                //           join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertUserId.Value }
                //                                           equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                //           from J1 in J1_join.DefaultIfEmpty()

                //           join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_UpdateUserId.Value }
                //                                           equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                //           from J2 in J2_join.DefaultIfEmpty()

                //           join J4 in dbContext.datahierarchy on new { ItemId = A.i_ParentItemId.Value, groupId = A.i_GroupId }
                //                                         equals new { ItemId = J4.i_ItemId, groupId = J4.i_GroupId } into J4_join
                //           from J4 in J4_join.DefaultIfEmpty()

                //           where A.i_GroupId == pintGroupId
                //                 && (A.i_IsDeleted == 0 || A.i_IsDeleted == null)






            }



        }


        public List<KeyValueDTO> ObtenerListadoActivosFijos(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbcontext.activofijo
                                 where n.i_Eliminado == 0 && n.i_EsTemporal == 0
                                 orderby n.v_CodigoActivoFijo ascending
                                 select new
                                 {

                                     v_CodigoActivoFijo = n.v_CodigoActivoFijo,
                                     v_IdActivoFijo = n.v_IdActivoFijo,

                                 }
                                 );
                    var query2 = query.AsEnumerable()
                                .Select(x => new KeyValueDTO
                                {
                                    Value1 = x.v_CodigoActivoFijo,
                                    Value2 = x.v_IdActivoFijo
                                }).ToList();

                    return query2;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool ExisteNroCorrelativo(string pstrCorrelativo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

            var Registro = (from n in dbContext.activofijo
                            where n.i_Eliminado == 0 && n.v_CodigoActivoFijo == pstrCorrelativo && n.i_EsTemporal == 0

                            select n).FirstOrDefault();

            if (Registro == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public string InsertarActivoFijo(ref OperationResult pobjOperationResult, activofijoDto pobjDtoEntity, List<string> ClientSession, List<activofijodetalleDto> pTemp_Insertar, bool Compra, List<activofijoanexoDto> pTemp_InsertarAnexo = null)
        {
            try
            {
                int SecuentialId = 0, intNodeId;
                productoDto _productoDto = new productoDto();
                ProductoBL _objProductoBL = new ProductoBL();
                string newIdActivo = string.Empty, newIdActivoDetalle = string.Empty, IdProductoDetalle = string.Empty, ProductoDetalle = null;
                string[] IdProdDetalle;
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {

                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        activofijo objEntityActivo = activofijoAssembler.ToEntity(pobjDtoEntity);
                        pedidodetalleDto pobjDtoPedidoDetalle = new pedidodetalleDto();
                        activofijodetalleDto pobjDtoActivoDetalle = new activofijodetalleDto();
                        OperationResult objOperationResult = new OperationResult();

                        if (Compra)
                        {
                            #region Inserta Producto
                            _productoDto = new productoDto();
                            _productoDto.v_CodInterno = pobjDtoEntity.v_CodigoActivoFijo.Trim();
                            //while (ExisteNroCorrelativo(  pobjDtoEntity.v_CodigoActivoFijo.Trim ()))
                            //{
                            //    _productoDto.v_CodInterno = (int.Parse(pobjDtoEntity.v_CodigoActivoFijo) + 1).ToString("00000000");

                            //}
                            _productoDto.v_CodInterno = pobjDtoEntity.v_CodigoActivoFijo.Trim();
                            _productoDto.v_IdLinea = null;
                            _productoDto.v_IdMarca = null;
                            _productoDto.v_IdModelo = null;
                            _productoDto.v_IdTalla = null;
                            _productoDto.v_IdColor = null;
                            _productoDto.v_Descripcion = pobjDtoEntity.v_Descricpion.Trim();
                            _productoDto.d_Empaque = 0;
                            _productoDto.i_IdUnidadMedida = 15;
                            _productoDto.d_Peso = 0;
                            _productoDto.v_Ubicacion = "";
                            _productoDto.v_Caracteristica = "";
                            _productoDto.v_CodProveedor = "";
                            _productoDto.v_Descripcion2 = "";
                            _productoDto.i_IdTipoProducto = -1;
                            _productoDto.i_EsServicio = 0;
                            _productoDto.i_EsLote = 0;
                            _productoDto.i_EsAfectoDetraccion = 0;
                            _productoDto.i_EsActivo = 0;
                            _productoDto.d_PrecioVenta = 0;
                            _productoDto.d_PrecioCosto = 0;
                            _productoDto.d_StockMinimo = 0;
                            _productoDto.d_StockMaximo = 0;
                            _productoDto.i_NombreEditable = 0;
                            _productoDto.i_ValidarStock = 1;
                            _productoDto.i_IdProveedor = -1;
                            _productoDto.i_IdTipo = -1;
                            _productoDto.i_IdUsuario = -1;
                            _productoDto.i_IdTela = -1;
                            _productoDto.i_IdEtiqueta = -1;
                            _productoDto.i_IdCuello = -1;
                            _productoDto.i_IdAplicacion = -1;
                            _productoDto.i_IdArte = -1;
                            _productoDto.i_IdColeccion = -1;
                            _productoDto.i_IdTemporada = -1;
                            _productoDto.i_Anio = 0;
                            _productoDto.i_EsAfectoPercepcion = 0;
                            _productoDto.d_TasaPercepcion = 0;
                            _productoDto.i_PrecioEditable = 0;
                            _productoDto.i_EsActivoFijo = 1;
                            // Save the data
                            IdProductoDetalle = _objProductoBL.InsertarProducto(ref objOperationResult, _productoDto, Globals.ClientSession.GetAsList());
                            IdProdDetalle = IdProductoDetalle.Split(new Char[] { ';' });
                            ProductoDetalle = IdProdDetalle[1].Trim();
                            #endregion
                        }


                        #region Inserta Cabecera Activo Fijo

                        objEntityActivo.v_IdProducto = ProductoDetalle;
                        objEntityActivo.t_InsertaFecha = DateTime.Now;
                        objEntityActivo.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityActivo.i_Eliminado = 0;

                        intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 74);
                        newIdActivo = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "AF");
                        //75- FD
                        objEntityActivo.v_IdActivoFijo = newIdActivo;
                        dbContext.AddToactivofijo(objEntityActivo);
                        //dbContext.AddTopedido(objEntityPedido);
                        #endregion

                        #region Inserta Detalle ActivoFijo

                        foreach (activofijodetalleDto activodetalleDto in pTemp_Insertar)
                        {
                            activofijodetalle objEntityActivoDetalle = activofijodetalleAssembler.ToEntity(activodetalleDto);

                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 75);
                            newIdActivoDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "FD");

                            objEntityActivoDetalle.v_IdActivoFijoDetalle = newIdActivoDetalle;
                            objEntityActivoDetalle.v_IdActivoFijo = newIdActivo;
                            objEntityActivoDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityActivoDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityActivoDetalle.i_Eliminado = 0;
                            dbContext.AddToactivofijodetalle(objEntityActivoDetalle);
                            // dbContext.AddTopedidodetalle(objEntityPedidoDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "activofijodetalle", newIdActivoDetalle);
                        }

                        #endregion


                        #region Insertar Anexo Activo Fijo
                        if (pTemp_InsertarAnexo != null)
                        {
                            foreach (activofijoanexoDto activoAnexoDto in pTemp_InsertarAnexo)
                            {

                                activofijoanexo objEntityActivoAnexo = activofijoanexoAssembler.ToEntity(activoAnexoDto);
                                objEntityActivoAnexo.v_IdActivoFijo = newIdActivo;
                                objEntityActivoAnexo.t_InsertaFecha = DateTime.Now;
                                objEntityActivoAnexo.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                                objEntityActivoAnexo.i_Eliminado = 0;
                                dbContext.AddToactivofijoanexo(objEntityActivoAnexo);
                                Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "activofijoanexo", activoAnexoDto.i_IdActivoFijoAnexo.ToString());
                            }
                        }


                        #endregion
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "activofijo", newIdActivo);
                        pobjOperationResult.Success = 1;
                        // Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "pedido", pobjDtoEntity.v_IdActivoFijo);
                        ts.Complete();

                        if (Compra)
                        {
                            return ProductoDetalle + ";" + newIdActivo;
                        }
                        else
                        {
                            return newIdActivo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ActivoFijoBL.InsertarActivoFijo()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }


        public productoshortDto DevolverArticuloporIdProductoDetalleActivoFijo(string IdProductoDetalle)
        {


            productoshortDto objProducto = new productoshortDto();

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                objProducto = (from a in dbContext.producto

                               join b in dbContext.productodetalle on new { IdProducto = a.v_IdProducto, eliminado = 0 } equals new { IdProducto = b.v_IdProducto, eliminado = b.i_Eliminado.Value } into b_join

                               from b in b_join.DefaultIfEmpty()


                               join c in dbContext.datahierarchy on new { a = a.i_IdUnidadMedida.Value, eliminado = 0, Grupo = 17 } equals new { a = c.i_ItemId, eliminado = c.i_IsDeleted.Value, Grupo = c.i_GroupId } into c_join

                               from c in c_join.DefaultIfEmpty()
                               where b.v_IdProductoDetalle == IdProductoDetalle && a.i_Eliminado == 0

                               select new productoshortDto
                               {
                                   v_IdProducto = a.v_IdProducto,
                                   v_IdProductoDetalle = b.v_IdProductoDetalle,
                                   v_Descripcion = a.v_Descripcion,
                                   v_CodInterno = a.v_CodInterno,
                                   i_EsServicio = a.i_EsServicio,
                                   i_EsLote = a.i_EsLote,
                                   i_IdTipoProducto = a.i_IdTipoProducto,
                                   d_Empaque = a.d_Empaque, // Empaque
                                   i_IdUnidadMedida = a.i_IdUnidadMedida,
                                   EmpaqueUnidadMedida = c.v_Value1,
                                   i_EsAfectoDetraccion = a.i_EsAfectoDetraccion,
                                   i_NombreEditable = a.i_NombreEditable,
                                   //StockDisponible = J3.d_StockActual - J3.d_SeparacionTotal,
                                   //i_ValidarStock = n.i_ValidarStock,
                                   //i_EsAfectoPercepcion = n.i_EsAfectoPercepcion,
                                   //d_TasaPercepcion = n.d_TasaPercepcion,
                                   //i_PrecioEditable = n.i_PrecioEditable,
                               }).FirstOrDefault();

            }

            return objProducto;
        }


        public activofijoDto ObtenerMovimientoCabecera(ref OperationResult pobjOperationResult, string pstrIdActivo)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var ActivoFijo = (from a in dbContext.activofijo

                                      join TipoActivo in dbContext.datahierarchy on new { IdMotivo = a.i_IdTipoActivo.Value, eliminado = 0, Grupo = 104 }

                                                                            equals new { IdMotivo = TipoActivo.i_ItemId, eliminado = TipoActivo.i_IsDeleted.Value, Grupo = TipoActivo.i_GroupId } into TipoActivo_join

                                      from TipoActivo in TipoActivo_join.DefaultIfEmpty()

                                      join Adquisicion in dbContext.datahierarchy on new { IdAdquisicion = a.i_IdTipoAdquisicion.Value, eliminado = 0, Grupo = 107 }
                                                                 equals new { IdAdquisicion = Adquisicion.i_ItemId, eliminado = Adquisicion.i_IsDeleted.Value, Grupo = Adquisicion.i_GroupId } into Adquisicion_join
                                      from Adquisicion in Adquisicion_join.DefaultIfEmpty()

                                      join Proveedor in dbContext.cliente on new { IdProveedor = a.v_IdCliente, eliminado = 0, Flag = "V" } equals new { IdProveedor = Proveedor.v_IdCliente, eliminado = Proveedor.i_Eliminado.Value, Flag = Proveedor.v_FlagPantalla } into Proveedor_join

                                      from Proveedor in Proveedor_join.DefaultIfEmpty()

                                      join Responsable in dbContext.cliente on new { IdResponsable = a.v_IdResponsable, eliminado = 0, Flag = "T" } equals new { IdResponsable = Responsable.v_IdCliente, eliminado = 0, Flag = Responsable.v_FlagPantalla } into Responsable_join

                                      from Responsable in Responsable_join.DefaultIfEmpty()

                                      join Ubicacion in dbContext.datahierarchy on new { Grupo = 103, eliminado = 0, IdUbicacion = a.i_IdUbicacion.Value } equals new { Grupo = Ubicacion.i_GroupId, eliminado = Ubicacion.i_IsDeleted.Value, IdUbicacion = Ubicacion.i_ItemId } into Ubicacion_join

                                      from Ubicacion in Ubicacion_join.DefaultIfEmpty()

                                      join CentroCosto in dbContext.datahierarchy on new { Grupo = 31, eliminado = 0, IdCentroCosto = a.i_IdCentroCosto.Value } equals new { Grupo = CentroCosto.i_GroupId, eliminado = CentroCosto.i_IsDeleted.Value, IdCentroCosto = CentroCosto.i_ItemId } into CentroCosto_join

                                      from CentroCosto in CentroCosto_join.DefaultIfEmpty()

                                      join TipoActivoTransferencia in dbContext.datahierarchy on new { IdMotivo = a.i_IdTipoActivoTransferencia.Value, eliminado = 0, Grupo = 104 }

                                                                          equals new { IdMotivo = TipoActivoTransferencia.i_ItemId, eliminado = TipoActivoTransferencia.i_IsDeleted.Value, Grupo = TipoActivoTransferencia.i_GroupId } into TipoActivoTransferencia_join

                                      from TipoActivoTransferencia in TipoActivoTransferencia_join.DefaultIfEmpty()

                                      join ProductoDetalle in dbContext.productodetalle on new { ProdDetalle = a.v_IdProducto, eliminado = 0 } equals new { ProdDetalle = ProductoDetalle.v_IdProductoDetalle, eliminado = ProductoDetalle.i_Eliminado.Value } into ProductoDetalle_join

                                      from ProductoDetalle in ProductoDetalle_join.DefaultIfEmpty()

                                      join Producto in dbContext.producto on new { Prod = ProductoDetalle.v_IdProducto, eliminado = 0 } equals new { Prod = Producto.v_IdProducto, eliminado = 0 } into Producto_join

                                      from Producto in Producto_join.DefaultIfEmpty()


                                      where a.i_Eliminado == 0 && a.v_IdActivoFijo == pstrIdActivo

                                      select new activofijoDto
                                      {

                                          v_IdActivoFijo = a.v_IdActivoFijo,
                                          v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                          i_IdTipoMotivo = a.i_IdTipoMotivo,
                                          v_Descricpion = a.v_Descricpion,
                                          v_Marca = a.v_Marca,
                                          v_Modelo = a.v_Modelo,
                                          v_Serie = a.v_Serie,
                                          v_Placa = a.v_Placa,
                                          i_IdTipoActivo = a.i_IdTipoActivo,
                                          CodigoTipoActivo = TipoActivo == null ? "" : TipoActivo.v_Value2,
                                          i_IdTipoAdquisicion = a.i_IdTipoAdquisicion,
                                          CodigoAdquisicion = Adquisicion == null ? "" : Adquisicion.v_Value2,
                                          v_Color = a.v_Color,
                                          v_CodigoAnterior = a.v_CodigoAnterior,
                                          i_IdEstado = a.i_IdEstado,
                                          i_IdTipoIntangible = a.i_IdTipoIntangible,
                                          v_IdCliente = a.v_IdCliente,
                                          CodigoProveedor = Proveedor == null ? "**NO EXISTE PROVEEDOR**" : Proveedor.v_CodCliente,
                                          Proveedor = Proveedor == null ? "**NO EXISTE PROVEEDOR **" : (Proveedor.v_ApePaterno + " " + Proveedor.v_ApeMaterno + " " + Proveedor.v_PrimerNombre + " " + Proveedor.v_RazonSocial).Trim(),
                                          v_NumeroFactura = a.v_NumeroFactura,
                                          t_FechaFactura = a.t_FechaFactura,
                                          v_OrdenCompra = a.v_OrdenCompra,
                                          t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                          d_MonedaExtranjera = a.d_MonedaExtranjera ?? 0,
                                          d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                          i_IdUbicacion = a.i_IdUbicacion,
                                          Ubicacion = Ubicacion.v_Value2,
                                          i_IdCentroCosto = a.i_IdCentroCosto,
                                          CodigoCentroCosto = CentroCosto == null ? "" : CentroCosto.v_Value2,
                                          v_NroContrato = a.v_NroContrato,
                                          t_FechaUso = a.t_FechaUso,
                                          i_NumeroCuotas = a.i_NumeroCuotas,
                                          i_Depreciara = a.i_Depreciara,
                                          i_IdMesesDepreciara = a.i_IdMesesDepreciara,
                                          i_Baja = a.i_Baja,
                                          t_FechaBaja = a.t_FechaBaja,
                                          v_MotivoBaja = a.v_MotivoBaja,
                                          i_Transferencia = a.i_Transferencia,
                                          t_FechaTransferencia = a.t_FechaTransferencia,
                                          CodigoTipoActivoTransferencia = TipoActivoTransferencia == null ? "" : TipoActivoTransferencia.v_Value2,
                                          i_IdTipoActivoTransferencia = a.i_IdTipoActivoTransferencia,
                                          i_Ajuste = a.i_Ajuste,
                                          t_FechaAjuste = a.t_FechaAjuste,
                                          i_MesesAjuste = a.i_MesesAjuste,
                                          i_Asignacion = a.i_Asignacion,
                                          i_Eliminado = a.i_Eliminado,
                                          i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                          i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                          t_ActualizaFecha = a.t_ActualizaFecha,
                                          t_InsertaFecha = a.t_InsertaFecha,
                                          d_ValorAdquisicionMe = a.d_ValorAdquisicionMe,
                                          d_ValorAdquisicionMn = a.d_ValorAdquisicionMn,
                                          v_IdProducto = a.v_IdProducto,
                                          IdProducto = Producto == null ? null : Producto.v_IdProducto,
                                          CodigoResponsable = Responsable == null ? "" : Responsable.v_CodCliente,
                                          Responsable = Responsable == null ? "" : (Responsable.v_ApePaterno + " " + Responsable.v_ApeMaterno + " " + Responsable.v_PrimerNombre + " " + Responsable.v_RazonSocial).Trim(),
                                          v_IdResponsable = Responsable == null ? null : Responsable.v_IdCliente,
                                          i_MesesDepreciadosPAnterior = a.i_MesesDepreciadosPAnterior == null ? 0 : a.i_MesesDepreciadosPAnterior.Value,
                                          v_PeriodoAnterior = string.IsNullOrEmpty(a.v_PeriodoAnterior) ? "2015" : a.v_PeriodoAnterior,
                                          b_Foto = a.b_Foto,
                                          i_IdTipoDocumento = a.i_IdTipoDocumento ?? -1,
                                          v_UbicacionFoto =a.v_UbicacionFoto ,
                                          i_IdSituacionActivoFijo =a.i_IdSituacionActivoFijo ??-1,
                                          i_IdClaseActivoFijo =a.i_IdClaseActivoFijo ??-1,
                                          v_AnioFabricacion =a.v_AnioFabricacion ,
                                          v_CodigoOriginal =a.v_CodigoOriginal ,

                                          v_CodigoBarras =a.v_CodigoBarras ,
                                       
                                      }).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return ActivoFijo;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }



        }

        public decimal[] ObtenerMonedasActivoFijo(string IdActivoFijo)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {
                decimal[] xx = new decimal[2];

                var Monedas = (from n in dbContext.activofijo

                               where n.i_Eliminado == 0 && n.v_IdActivoFijo == IdActivoFijo

                               select new
                               {
                                   MonedaNacional = n.d_MonedaNacional.Value,
                                   MonedaExtranjera = n.d_MonedaExtranjera.Value,

                               }).FirstOrDefault();
                if (Monedas != null)
                {
                    xx[0] = Monedas.MonedaExtranjera;
                    xx[1] = Monedas.MonedaNacional;
                }


                return xx;
            }


        }


        public BindingList<activofijodetalleDto> ObtenerActivoDetalles(ref OperationResult pobjOperationResult, string pstrIdActivo)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.activofijodetalle

                                 join centrocosto in dbContext.datahierarchy on new { IdCosto = n.i_Ccosto.Value, eliminado = 0, Grupo = 31 }
                                                                            equals new { IdCosto = centrocosto.i_ItemId, eliminado = centrocosto.i_IsDeleted.Value, Grupo = centrocosto.i_GroupId } into centrocosto_join
                                 from centrocosto in centrocosto_join.DefaultIfEmpty()

                                 join proveedor in dbContext.cliente on new { IdProv = n.v_IdResponsableAsignacion, eliminado = 0, Flag = "V" }
                                                                    equals new { IdProv = proveedor.v_IdCliente, eliminado = proveedor.i_Eliminado.Value, Flag = proveedor.v_FlagPantalla }

                                 where n.i_Eliminado == 0 && n.v_IdActivoFijo == pstrIdActivo

                                 orderby n.v_IdActivoFijoDetalle ascending

                                 select new activofijodetalleDto
                                 {
                                     t_FechaAsignacion = n.t_FechaAsignacion,
                                     v_CodResponsableAsignacion = proveedor.v_CodCliente,
                                     i_Ccosto = n.i_Ccosto,
                                     CentroCosto = centrocosto == null ? "**NO EXISTE**" : centrocosto.v_Value1,
                                     v_Observacion = n.v_Observacion,
                                     i_Eliminado = n.i_Eliminado,
                                     i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                     i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                     t_InsertaFecha = n.t_InsertaFecha,
                                     t_ActualizaFecha = n.t_ActualizaFecha,
                                     ResponsableAsig = (proveedor.v_ApePaterno + " " + proveedor.v_ApeMaterno + " " + proveedor.v_PrimerNombre + " " + proveedor.v_RazonSocial).Trim(),
                                     v_IdActivoFijo = n.v_IdActivoFijo,
                                     v_IdActivoFijoDetalle = n.v_IdActivoFijoDetalle,
                                     v_IdResponsableAsignacion = proveedor.v_IdCliente,
                                     CodigoCentroCosto = centrocosto.v_Value2,




                                 }
                                 ).ToList();

                    pobjOperationResult.Success = 1;

                    return new BindingList<activofijodetalleDto>(query);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }


        public string ActualizarActivoFijo(ref OperationResult pobjOperationResult, activofijoDto pobjDtoEntity, List<string> ClientSession, List<activofijodetalleDto> pTemp_Insertar, List<activofijodetalleDto> pTemp_Editar, List<activofijodetalleDto> pTemp_Eliminar, List<activofijoanexoDto> pTemp_InsertarAnexo, List<activofijoanexoDto> pTemp_EditarAnexo, List<activofijoanexoDto> pTemp_EliminarAnexo)
        {
            try
            {

                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    activofijo objEntityActivoFijo = activofijoAssembler.ToEntity(pobjDtoEntity);
                    // activofijodetalleDto pobjDtoActivoFijoDetalle = new activofijodetalleDto();

                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                    int SecuentialId = 0, intNodeId;
                   
                    #region Actualiza Cabecera
                    intNodeId = int.Parse(ClientSession[0]);


                    var objEntitySource = (from a in dbContext.activofijo
                                           where a.v_IdActivoFijo == pobjDtoEntity.v_IdActivoFijo
                                           select a).FirstOrDefault();

                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    //activofijo objEntity = activofijoAssembler.ToEntity(pobjDtoEntity);
                    activofijo objEntity = activofijoAssembler.ToEntity(pobjDtoEntity);

                    dbContext.activofijo.ApplyCurrentValues(objEntity);

                    #endregion
                    #region Actualiza Detalle
                    foreach (activofijodetalleDto activofijodetalleDto in pTemp_Insertar)
                    {
                        activofijodetalle objEntityActivoFijos = activofijodetalleAssembler.ToEntity(activofijodetalleDto);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 75);
                        string newIdActivoFijoDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "FD");
                        objEntityActivoFijos.v_IdActivoFijoDetalle = newIdActivoFijoDetalle;
                        objEntityActivoFijos.t_InsertaFecha = DateTime.Now;
                        objEntityActivoFijos.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityActivoFijos.i_Eliminado = 0;
                        dbContext.AddToactivofijodetalle(objEntityActivoFijos);
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "activofijodetalle", newIdActivoFijoDetalle);

                    }


                    foreach (activofijodetalleDto activofijodetalleDto in pTemp_Editar)
                    {
                        activofijodetalle _objEntity = activofijodetalleAssembler.ToEntity(activofijodetalleDto);

                        var query = (from n in dbContext.activofijodetalle
                                     where n.v_IdActivoFijoDetalle == activofijodetalleDto.v_IdActivoFijoDetalle
                                     select n).FirstOrDefault();

                        _objEntity.t_ActualizaFecha = DateTime.Now;
                        _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                        dbContext.activofijodetalle.ApplyCurrentValues(_objEntity);
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "activofijodetalle", query.v_IdActivoFijoDetalle);
                    }

                    foreach (activofijodetalleDto activofijodetalleDto in pTemp_Eliminar)
                    {
                        activofijodetalle _objEntity = activofijodetalleAssembler.ToEntity(activofijodetalleDto);
                        var query = (from n in dbContext.activofijodetalle
                                     where n.v_IdActivoFijoDetalle == activofijodetalleDto.v_IdActivoFijoDetalle
                                     select n).FirstOrDefault();

                        if (query != null)
                        {
                            query.t_ActualizaFecha = DateTime.Now;
                            query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            query.i_Eliminado = 1;
                        }

                        dbContext.activofijodetalle.ApplyCurrentValues(query);
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "activofijodetalle", query.v_IdActivoFijoDetalle);
                    }
                    #endregion




                    #region Actualiza Anexo
                    foreach (activofijoanexoDto activofijoAnexoDto in pTemp_InsertarAnexo)
                    {
                        activofijoanexo objEntityActivoAnexo = activofijoanexoAssembler.ToEntity(activofijoAnexoDto);
                        //SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 75);
                        //newIdActivoFijo = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "FD");
                        objEntityActivoAnexo.v_IdActivoFijo = pobjDtoEntity.v_IdActivoFijo;
                        objEntityActivoAnexo.t_InsertaFecha = DateTime.Now;
                        objEntityActivoAnexo.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityActivoAnexo.i_Eliminado = 0;
                        dbContext.AddToactivofijoanexo(objEntityActivoAnexo);
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "activofijoanexo", objEntityActivoAnexo.i_IdActivoFijoAnexo.ToString ());

                    }


                    foreach (activofijoanexoDto activofijoAnexoDto in pTemp_EditarAnexo)
                    {
                        activofijoanexo _objEntity = activofijoanexoAssembler.ToEntity(activofijoAnexoDto);

                        var query = (from n in dbContext.activofijoanexo
                                     where   n.i_IdActivoFijoAnexo  == activofijoAnexoDto.i_IdActivoFijoAnexo 
                                     select n).FirstOrDefault();

                        _objEntity.t_ActualizaFecha = DateTime.Now;
                        _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                        dbContext.activofijoanexo.ApplyCurrentValues(_objEntity);
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "activofijoanexo", query.i_IdActivoFijoAnexo.ToString ());
                    }

                    foreach (activofijoanexoDto activofijoAnexoDto in pTemp_EliminarAnexo)
                    {
                        activofijoanexo _objEntity = activofijoanexoAssembler.ToEntity(activofijoAnexoDto);
                        var query = (from n in dbContext.activofijoanexo
                                     where n.i_IdActivoFijoAnexo == activofijoAnexoDto.i_IdActivoFijoAnexo
                                     select n).FirstOrDefault();

                        if (query != null)
                        {
                            query.t_ActualizaFecha = DateTime.Now;
                            query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            query.i_Eliminado = 1;
                        }

                        dbContext.activofijoanexo.ApplyCurrentValues(query);
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "activofijoanexo", query.i_IdActivoFijoAnexo.ToString ());
                    }
                    #endregion




                    # region ActualizaProducto
                    if (objEntitySource.v_IdProducto != null)
                    {

                        var ProductoDetalle = (from n in dbContext.productodetalle
                                               where n.i_Eliminado == 0 && n.v_IdProductoDetalle == objEntitySource.v_IdProducto

                                               select n).FirstOrDefault();

                        productodetalleDto objProductoDetalle = productodetalleAssembler.ToDTO(ProductoDetalle);
                        var Producto = (from n in dbContext.producto

                                        where n.i_Eliminado == 0 && n.v_IdProducto == ProductoDetalle.v_IdProducto

                                        select n).FirstOrDefault();

                        productoDto objProducto = productoAssembler.ToDTO(Producto);
                        objProducto.v_Descripcion = pobjDtoEntity.v_Descricpion;

                        new ProductoBL().ActualizarProducto(ref pobjOperationResult, objProducto, objProductoDetalle, ClientSession, false, 0);
                    }
                    #endregion
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "activofijo", pobjDtoEntity.v_IdActivoFijo);
                    ts.Complete();
                    return pobjDtoEntity.v_IdActivoFijo;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ActivoFijoBL.ActualizarActivoFijo()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return "";
            }
        }


        public bool EsValidoResponsable(string pstrCodigo)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    cliente P = (from p in dbContext.cliente
                                 where p.v_CodCliente == pstrCodigo && p.i_Eliminado == 0 && p.v_FlagPantalla == "V"
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


        //public bool CalcularDepreciacion(int periodo, int mes)
        //{
        //    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
        //    {
        //        // Para periodo Anterior solo se toma en cuenta los valores ingresados en su registros de alta y desde ahi se empieza a depreciar
        //        DateTime future = DateTime.Parse(DateTime.DaysInMonth(periodo, mes).ToString() + "/" + mes.ToString() + "/" + periodo.ToString()); //FechaReporte
        //        using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
        //        {
        //            OperationResult objOperationResul = new OperationResult();
        //            List<activofijodepreciacionDto> ListaActivoFijoDepreciacion = new List<activofijodepreciacionDto>();
        //            activofijodepreciacionDto objActivoDepreciacion = new activofijodepreciacionDto();
        //            DateTime FechaInicioUsoPeriodoActual = DateTime.Parse("01/01/" + periodo.ToString());
        //            string strPeriodo = periodo.ToString();

        //            var ActivosPeriodoActual = (from a in dbContext.activofijo

        //                                        join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join
        //                                        from b in b_join.DefaultIfEmpty()

        //                                        where a.i_Eliminado == 0 && a.i_Depreciara == 1 && a.t_FechaUso != null && a.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoActual
        //                                        && a.t_FechaUso.Value <= future && a.t_FechaUso.Value >= FechaInicioUsoPeriodoActual && a.i_EsTemporal == 0
        //                                        select new activofijoDto
        //                                        {

        //                                            v_IdActivoFijo = a.v_IdActivoFijo,
        //                                            d_MonedaNacional = a.d_MonedaNacional ?? 0,
        //                                            mesesAdepreciar = b.v_Value1,
        //                                            t_FechaUso = a.t_FechaUso,
        //                                            i_Baja = a.i_Baja,
        //                                            t_FechaBaja = a.t_FechaBaja,
        //                                        }).ToList();



        //            var ActivosPeriodoAnterior = (from a in dbContext.activofijo
        //                                          join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join

        //                                          from b in b_join.DefaultIfEmpty()

        //                                          where a.i_Eliminado == 0 && a.i_Depreciara == 1 && a.t_FechaUso != null && a.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior
        //                                          && a.v_Periodo == strPeriodo && a.i_EsTemporal == 0
        //                                          select new activofijoDto
        //                                          {

        //                                              v_IdActivoFijo = a.v_IdActivoFijo,
        //                                              d_MonedaNacional = a.d_MonedaNacional ?? 0,
        //                                              mesesAdepreciar = b.v_Value1,
        //                                              t_FechaUso = a.t_FechaUso,
        //                                              i_Baja = a.i_Baja,
        //                                              t_FechaBaja = a.t_FechaBaja,
        //                                          }

        //                                        ).ToList();

        //            var ActivosTotal = ActivosPeriodoActual.Concat(ActivosPeriodoAnterior);
        //            #region Elimina ActivoFijoDepreciacion-Periodo

        //            string pstrPeriodo = periodo.ToString();
        //            var ActivosFijoPeriodo = (from a in dbContext.activofijodepreciacion
        //                                      where a.v_Periodo == pstrPeriodo
        //                                      select a).ToList();


        //            foreach (var objActivoDepreciacionesPeriodo in ActivosFijoPeriodo)
        //            {
        //                dbContext.activofijodepreciacion.DeleteObject(objActivoDepreciacionesPeriodo);
        //            }
        //            // dbContext.SaveChanges(); 
        //            #endregion
        //            foreach (var item in ActivosTotal.AsParallel())
        //            {
        //                DateTime past = item.t_FechaUso.Value.Date; // Fecha Uso
        //                decimal valoradquisicion = item.d_MonedaNacional.Value;
        //                int mesesAdepreciar = int.Parse(item.mesesAdepreciar);
        //                int anioInicio = int.Parse(future.Date.Year.ToString());
        //                int anioFin = int.Parse(future.Date.Year.ToString());
        //                while (anioInicio <= anioFin)
        //                {
        //                    //if (past.Date.Month != future.Date.Month || past.Date.Year != future.Date.Year)
        //                    //{
        //                    for (int i = 1; i <= 12; i++)
        //                    {
        //                        if (i <= int.Parse(future.Date.Month.ToString()) || anioInicio < anioFin)
        //                        {
        //                            objActivoDepreciacion = new activofijodepreciacionDto();
        //                            objActivoDepreciacion.v_IdActivoFijo = item.v_IdActivoFijo;
        //                            objActivoDepreciacion.v_Mes = i.ToString("00");
        //                            objActivoDepreciacion.v_Periodo = anioInicio.ToString();
        //                            #region ActivoSinBaja
        //                            if (item.i_Baja == 0)
        //                            {
        //                                DateTime FechaInicio = new DateTime();
        //                                if (past.Date.Month == 12)
        //                                {
        //                                    FechaInicio = DateTime.Parse("01/01" + past.Date.Year + 1);
        //                                }
        //                                else
        //                                {
        //                                    FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
        //                                }
        //                                DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());
        //                                var elapsedtime = FechaFin.Subtract(FechaInicio);
        //                                var months = elapsedtime.Days / (365.25 / 12);

        //                                objActivoDepreciacion.i_MesesDepreciados = int.Parse(Math.Round(decimal.Parse(months.ToString())) < 0 ? "0" : Math.Round(decimal.Parse(months.ToString())).ToString());

        //                                if (objActivoDepreciacion.i_MesesDepreciados <= mesesAdepreciar)
        //                                {

        //                                    objActivoDepreciacion.d_ImporteMensualDepreciacion = objActivoDepreciacion.i_MesesDepreciados == 0 ? 0 : mesesAdepreciar == 0 ? 0 : valoradquisicion / mesesAdepreciar;
        //                                    objActivoDepreciacion.d_AcumuladoHistorico = objActivoDepreciacion.i_MesesDepreciados * objActivoDepreciacion.d_ImporteMensualDepreciacion;
        //                                    objActivoDepreciacion.d_ValorNetoActual = valoradquisicion - objActivoDepreciacion.d_AcumuladoHistorico;
        //                                    objActivoDepreciacion.d_Comparacion = objActivoDepreciacion.d_ImporteMensualDepreciacion * (mesesAdepreciar - objActivoDepreciacion.i_MesesDepreciados);
        //                                    objActivoDepreciacion.d_AjusteDepreciacion = objActivoDepreciacion.d_Comparacion - objActivoDepreciacion.d_ValorNetoActual;

        //                                }
        //                                else
        //                                {
        //                                    objActivoDepreciacion.d_ImporteMensualDepreciacion = 0;
        //                                    objActivoDepreciacion.d_AcumuladoHistorico = 0;
        //                                    objActivoDepreciacion.d_ValorNetoActual = 0;
        //                                    objActivoDepreciacion.d_Comparacion = 0;
        //                                    objActivoDepreciacion.d_AjusteDepreciacion = 0;

        //                                }
        //                            }
        //                            #endregion
        //                            else
        //                            #region ActivoBaja
        //                            {

        //                                if (i <= int.Parse(item.t_FechaBaja.Value.Month.ToString()))
        //                                {
        //                                    DateTime FechaInicio = new DateTime();
        //                                    if (past.Date.Month == 12)
        //                                    {
        //                                        FechaInicio = DateTime.Parse("01/01" + past.Date.Year + 1);
        //                                    }
        //                                    else
        //                                    {
        //                                        FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
        //                                    }
        //                                    DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());
        //                                    var elapsedtime = FechaFin.Subtract(FechaInicio);
        //                                    var months = elapsedtime.Days / (365.25 / 12);

        //                                    objActivoDepreciacion.i_MesesDepreciados = int.Parse(Math.Round(decimal.Parse(months.ToString())) < 0 ? "0" : Math.Round(decimal.Parse(months.ToString())).ToString());

        //                                    if (objActivoDepreciacion.i_MesesDepreciados <= mesesAdepreciar)
        //                                    {

        //                                        objActivoDepreciacion.d_ImporteMensualDepreciacion = objActivoDepreciacion.i_MesesDepreciados == 0 ? 0 : mesesAdepreciar == 0 ? 0 : valoradquisicion / mesesAdepreciar;
        //                                        objActivoDepreciacion.d_AcumuladoHistorico = objActivoDepreciacion.i_MesesDepreciados * objActivoDepreciacion.d_ImporteMensualDepreciacion;
        //                                        objActivoDepreciacion.d_ValorNetoActual = valoradquisicion - objActivoDepreciacion.d_AcumuladoHistorico;
        //                                        objActivoDepreciacion.d_Comparacion = objActivoDepreciacion.d_ImporteMensualDepreciacion * (mesesAdepreciar - objActivoDepreciacion.i_MesesDepreciados);
        //                                        objActivoDepreciacion.d_AjusteDepreciacion = objActivoDepreciacion.d_Comparacion - objActivoDepreciacion.d_ValorNetoActual;

        //                                    }
        //                                    else
        //                                    {
        //                                        objActivoDepreciacion.d_ImporteMensualDepreciacion = 0;
        //                                        objActivoDepreciacion.d_AcumuladoHistorico = 0;
        //                                        objActivoDepreciacion.d_ValorNetoActual = 0;
        //                                        objActivoDepreciacion.d_Comparacion = 0;
        //                                        objActivoDepreciacion.d_AjusteDepreciacion = 0;

        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    objActivoDepreciacion.i_MesesDepreciados = 0;
        //                                    objActivoDepreciacion.d_ImporteMensualDepreciacion = 0;
        //                                    objActivoDepreciacion.d_AcumuladoHistorico = 0;
        //                                    objActivoDepreciacion.d_ValorNetoActual = 0;
        //                                    objActivoDepreciacion.d_Comparacion = 0;
        //                                    objActivoDepreciacion.d_AjusteDepreciacion = 0;
        //                                    ListaActivoFijoDepreciacion.Add(objActivoDepreciacion);
        //                                }

        //                            }
        //                            #endregion
        //                            if (objActivoDepreciacion.i_MesesDepreciados > 0)
        //                            {
        //                                ListaActivoFijoDepreciacion.Add(objActivoDepreciacion);
        //                            }
        //                        }
        //                        //}
        //                    }
        //                    anioInicio = anioInicio + 1;
        //                }

        //            }
        //            InsertaActivoFijoDepreciacion(ref objOperationResul, ListaActivoFijoDepreciacion.ToList(), Globals.ClientSession.GetAsList());
        //            if (objOperationResul.Success == 1)
        //            {

        //                dbContext.SaveChanges();
        //                ts.Complete();
        //                return true;

        //            }
        //            else
        //            {
        //                return false;
        //            }
        //        }
        //    }
        //}
        private int CalcularMesesDepreciados(int HastaMes, int Periodo, DateTime FechaUso)
        {
            int MesFechaUso = HastaMes - int.Parse(FechaUso.Date.Month.ToString()) + 1;
            return MesFechaUso;


        }


        public void InsertaActivoFijoDepreciacion(ref OperationResult pobjOperationResult, List<activofijodepreciacionDto> ListapobjDtoEntity, List<string> ClientSession)
        {
            int SecuentialId = 0;
            string newId = string.Empty;

            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    foreach (var item in ListapobjDtoEntity)
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        activofijodepreciacion objEntity = activofijodepreciacionAssembler.ToEntity(item);
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        // objEntity.i_Eliminado = 0;
                        // Autogeneramos el Pk de la tabla
                        int intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 98);
                        newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "RR");
                        objEntity.v_IdDepreciacion = newId;
                        //  dbContext.AddToconcepto(objEntity);
                        dbContext.AddToactivofijodepreciacion(objEntity);
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "activofijodepreciacion", newId);
                        // return newId;
                    }

                    pobjOperationResult.Success = 1;

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ActivoFijoBL.InsertarCuentasInventarios()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                // return "";
            }
        }

        public void RevertirActivoFijoProducto(List<string> ListaIdProductoDetalle)
        {

            foreach (var item in ListaIdProductoDetalle)
            {

            }

        }


        public void EliminarActivoFijoDesdeCompras(ref OperationResult objOperationResult, List<string> ClientSession, string pstrIdProductoDetalle)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var IdProducto = (from n in dbContext.productodetalle
                                          where n.i_Eliminado == 0 && n.v_IdProductoDetalle == pstrIdProductoDetalle

                                          select new
                                          {
                                              n.v_IdProducto
                                          }).FirstOrDefault();

                        var IdActivo = (from n in dbContext.activofijo
                                        where n.i_Eliminado == 0 && n.v_IdProducto == pstrIdProductoDetalle
                                        select new
                                        {
                                            n.v_IdActivoFijo
                                        }).FirstOrDefault();
                        new ProductoBL().EliminarProducto(ref objOperationResult, IdProducto.v_IdProducto, ClientSession);

                        if (objOperationResult.Success == 1)
                        {
                            new ActivoFijoBL().EliminarActivoFijo(ref objOperationResult, IdActivo.v_IdActivoFijo, ClientSession);
                        }

                    }
                    objOperationResult.Success = 1;
                    ts.Complete();
                }
            }

            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ActivoFijoBL.EliminarActivoFijoDesdeCompras()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return;
            }

        }

        //public string DevolverTipoCambioPorFecha(ref OperationResult pobjOperationResult, DateTime Fecha)
        //{
        //    try
        //    {
        //        var query = new TipoCambioBL().DevolverTipoCambioPorFechaCompra(ref pobjOperationResult, Fecha);
        //        if (pobjOperationResult.Success == 0) return "0";
        //        return query;
        //    }
        //    catch (Exception ex)
        //    {
        //        pobjOperationResult.Success = 0;
        //        pobjOperationResult.AdditionalInformation = "ComprasBL.DevolverTipoCambioPorFecha()\nLinea:" +
        //                                                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
        //        pobjOperationResult.ErrorMessage = ex.Message;
        //        pobjOperationResult.ExceptionMessage = ex.InnerException != null
        //            ? ex.InnerException.Message
        //            : string.Empty;
        //        Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
        //        return null;
        //    }
        //}


        public bool TieneCuentaContable(string TipoActivo)
        {

            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var CuentasInventarios = (from a in dbContext.datahierarchy

                                          join b in dbContext.cuentasinventarios on new { TipoActivo = a.i_ItemId, eliminado = 0 } equals new { TipoActivo = b.i_IdTipoActivo.Value, eliminado = b.i_Eliminado.Value }


                                          where a.i_GroupId == 104 && a.i_IsDeleted.Value == 0 && a.v_Value2.Trim() == TipoActivo

                                          select b).FirstOrDefault();

                if (CuentasInventarios != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string CuentaInventarios(string TipoActivo)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var CuentasInventarios = (from a in dbContext.datahierarchy

                                          join b in dbContext.cuentasinventarios on new { TipoActivo = a.i_ItemId, eliminado = 0 } equals new { TipoActivo = b.i_IdTipoActivo.Value, eliminado = b.i_Eliminado.Value }


                                          where a.i_GroupId == 104 && a.i_IsDeleted.Value == 0 && a.v_Value2.Trim() == TipoActivo

                                          select b).FirstOrDefault();

                if (CuentasInventarios != null)
                {
                    return CuentasInventarios.v_Cuenta33.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }

        }



        #endregion

        #region Bandeja
        public List<activofijoDto> ListarBusquedaActivosFijos(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, DateTime F_Ini, DateTime F_Fin, bool RangoFecha)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {


                    if (RangoFecha)
                    {
                        var query = (from n in dbContext.activofijo

                                     join B in dbContext.cliente on new { n.v_IdCliente, eliminado = 0, Flag = "V" } equals new { B.v_IdCliente, eliminado = B.i_Eliminado.Value, Flag = B.v_FlagPantalla } into B_join
                                     from B in B_join.DefaultIfEmpty()

                                     join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                                    equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                     from J2 in J2_join.DefaultIfEmpty()

                                     join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                                    equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                                     from J3 in J3_join.DefaultIfEmpty()

                                     join J4 in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, TipoActivo = n.i_IdTipoActivo.Value } equals new { Grupo = J4.i_GroupId, eliminado = J4.i_IsDeleted.Value, TipoActivo = J4.i_ItemId } into J4_join
                                     from J4 in J4_join.DefaultIfEmpty()
                                     // join J4 in dbContext.cliente on new { n.v_IdResponsable, eliminado = 0, Flag = "T" } equals new { J4.v_IdCliente ,eliminado=J4.i_Eliminado.Value ,Flag=J4.v_FlagPantalla  }
                                     //join J4 in dbContext.compradetalle on new { IdProductoDetalle = n.v_IdProducto, eliminado = 0 } equals new { IdProductoDetalle = J4.v_IdProductoDetalle, eliminado = J4.i_Eliminado.Value } into J4_join

                                     //from J4 in J4_join.DefaultIfEmpty()

                                     //join J5 in dbContext.compra on new { IdCompra = J4.v_IdCompra, eliminado = 0 } equals new { IdCompra = J5.v_IdCompra, eliminado = J5.i_Eliminado.Value } into J5_join

                                     //from J5 in J5_join.DefaultIfEmpty() 

                                     where n.i_Eliminado == 0 && n.t_FechaFactura >= F_Ini && n.t_FechaFactura <= F_Fin && n.i_EsTemporal == 0
                                     select new
                                     {
                                         v_IdActivoFijo = n.v_IdActivoFijo,
                                         v_CodigoActivoFijo = n.v_CodigoActivoFijo,
                                         v_Descricpion = n.v_Descricpion,
                                         CodigoProveedor = B == null ? "**CLIENTE NO EXISTE**" : B.v_CodCliente.Trim(),
                                         t_ActualizaFecha = n.t_ActualizaFecha,
                                         t_InsertaFecha = n.t_InsertaFecha,
                                         Proveedor = B == null ? "**CLIENTE NO EXISTE**" : (B.v_CodCliente.Trim() + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_RazonSocial).Trim(),
                                         v_UsuarioModificacion = J2.v_UserName,
                                         v_UsuarioCreacion = J3.v_UserName,
                                         v_Periodo = n.v_Periodo,
                                         i_Baja = n.i_Baja ?? 0,
                                         i_Transferencia = n.i_Transferencia ?? 0,
                                         i_Ajuste = n.i_Ajuste ?? 0,
                                         i_Asignacion = n.i_Asignacion ?? 0,
                                         t_FechaFactura = n.t_FechaFactura,
                                         t_FechaOrdenCompra = n.t_FechaOrdenCompra,
                                         v_IdProducto = n.v_IdProducto,
                                         Origen = n.v_IdProducto == "" ? null : n.v_IdProducto,
                                         i_Depreciara = n.i_Depreciara ?? 0,
                                         CodigoTipoActivo = J4.v_Value2,
                                         d_MonedaNacional = n.d_MonedaNacional ?? 0,
                                         d_MonedaExtranjera = n.d_MonedaExtranjera ?? 0,
                                         t_FechaUso = n.t_FechaUso,

                                     }
                                    ).ToList().Select(n => new activofijoDto
                                    {
                                        v_IdActivoFijo = n.v_IdActivoFijo,
                                        v_CodigoActivoFijo = n.v_CodigoActivoFijo,
                                        v_Descricpion = n.v_Descricpion,
                                        CodigoProveedor = n.CodigoProveedor,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        Proveedor = n.Proveedor,
                                        v_UsuarioModificacion = n.v_UsuarioCreacion,
                                        v_UsuarioCreacion = n.v_UsuarioCreacion,
                                        v_Periodo = n.v_Periodo,
                                        i_Baja = n.i_Baja,
                                        i_Transferencia = n.i_Transferencia,
                                        i_Ajuste = n.i_Ajuste,
                                        i_Asignacion = n.i_Asignacion,
                                        t_FechaFactura = n.t_FechaFactura,
                                        t_FechaOrdenCompra = n.t_FechaOrdenCompra,
                                        v_IdProducto = n.v_IdProducto,
                                        Origen = n.v_IdProducto == "" ? null : n.v_IdProducto,
                                        i_Depreciara = n.i_Depreciara,
                                        CodigoTipoActivo = n.CodigoTipoActivo,
                                        d_MonedaNacional = n.d_MonedaNacional,
                                        d_MonedaExtranjera = n.d_MonedaExtranjera,
                                        sFechaUso = n.i_Depreciara == 1 ? n.t_FechaUso == null ? "" : n.t_FechaUso.Value.ToShortDateString() : "",


                                    }).ToList().AsQueryable();

                        if (!string.IsNullOrEmpty(pstrFilterExpression))
                        {
                            query = query.Where(pstrFilterExpression);
                        }
                        if (!string.IsNullOrEmpty(pstrSortExpression))
                        {
                            query = query.OrderBy(pstrSortExpression);
                        }

                        List<activofijoDto> objData = query.ToList();
                        pobjOperationResult.Success = 1;
                        return objData;
                    }
                    else
                    {


                        var query = (from n in dbContext.activofijo

                                     join B in dbContext.cliente on new { n.v_IdCliente, eliminado = 0, Flag = "V" } equals new { B.v_IdCliente, eliminado = B.i_Eliminado.Value, Flag = B.v_FlagPantalla } into B_join
                                     from B in B_join.DefaultIfEmpty()

                                     join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                                    equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                     from J2 in J2_join.DefaultIfEmpty()

                                     join J3 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                                    equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                                     from J3 in J3_join.DefaultIfEmpty()
                                     join J4 in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, TipoActivo = n.i_IdTipoActivo.Value } equals new { Grupo = J4.i_GroupId, eliminado = J4.i_IsDeleted.Value, TipoActivo = J4.i_ItemId } into J4_join
                                     from J4 in J4_join.DefaultIfEmpty()
                                     where n.i_Eliminado == 0 && n.i_EsTemporal == 0
                                     select new activofijoDto
                                     {
                                         v_IdActivoFijo = n.v_IdActivoFijo,
                                         v_CodigoActivoFijo = n.v_CodigoActivoFijo,
                                         v_Descricpion = n.v_Descricpion,
                                         CodigoProveedor = B == null ? "**CLIENTE NO EXISTE**" : B.v_CodCliente.Trim(),
                                         t_ActualizaFecha = n.t_ActualizaFecha,
                                         t_InsertaFecha = n.t_InsertaFecha,
                                         Proveedor = B == null ? "**CLIENTE NO EXISTE**" : (B.v_CodCliente.Trim() + " " + B.v_ApePaterno + " " + B.v_ApeMaterno + " " + B.v_PrimerNombre + " " + B.v_RazonSocial).Trim(),
                                         v_UsuarioModificacion = J2.v_UserName,
                                         v_UsuarioCreacion = J3.v_UserName,
                                         v_Periodo = n.v_Periodo,
                                         i_Baja = n.i_Baja ?? 0,
                                         i_Transferencia = n.i_Transferencia ?? 0,
                                         i_Ajuste = n.i_Ajuste ?? 0,
                                         i_Asignacion = n.i_Asignacion ?? 0,
                                         t_FechaFactura = n.t_FechaFactura,
                                         t_FechaOrdenCompra = n.t_FechaOrdenCompra,
                                         v_IdProducto = n.v_IdProducto,
                                         Origen = n.v_IdProducto == "" ? null : n.v_IdProducto,
                                         i_Depreciara = n.i_Depreciara ?? 0,
                                         CodigoTipoActivo = J4.v_Value2,
                                         d_MonedaNacional = n.d_MonedaNacional ?? 0,
                                         d_MonedaExtranjera = n.d_MonedaExtranjera ?? 0,
                                         t_FechaUso = n.t_FechaUso,

                                     }
                                    ).ToList().Select(n => new activofijoDto
                                    {
                                        v_IdActivoFijo = n.v_IdActivoFijo,
                                        v_CodigoActivoFijo = n.v_CodigoActivoFijo,
                                        v_Descricpion = n.v_Descricpion,
                                        CodigoProveedor = n.CodigoProveedor,
                                        t_ActualizaFecha = n.t_ActualizaFecha,
                                        t_InsertaFecha = n.t_InsertaFecha,
                                        Proveedor = n.Proveedor,
                                        v_UsuarioModificacion = n.v_UsuarioCreacion,
                                        v_UsuarioCreacion = n.v_UsuarioCreacion,
                                        v_Periodo = n.v_Periodo,
                                        i_Baja = n.i_Baja,
                                        i_Transferencia = n.i_Transferencia,
                                        i_Ajuste = n.i_Ajuste,
                                        i_Asignacion = n.i_Asignacion,
                                        t_FechaFactura = n.t_FechaFactura,
                                        t_FechaOrdenCompra = n.t_FechaOrdenCompra,
                                        v_IdProducto = n.v_IdProducto,
                                        Origen = n.v_IdProducto == "" ? null : n.v_IdProducto,
                                        i_Depreciara = n.i_Depreciara,
                                        CodigoTipoActivo = n.CodigoTipoActivo,
                                        d_MonedaNacional = n.d_MonedaNacional,
                                        d_MonedaExtranjera = n.d_MonedaExtranjera,
                                        sFechaUso = n.i_Depreciara == 1 ? n.t_FechaUso == null ? "" : n.t_FechaUso.Value.ToShortDateString() : "",


                                    }).ToList().AsQueryable();

                        if (!string.IsNullOrEmpty(pstrFilterExpression))
                        {
                            query = query.Where(pstrFilterExpression);
                        }
                        if (!string.IsNullOrEmpty(pstrSortExpression))
                        {
                            query = query.OrderBy(pstrSortExpression);
                        }

                        List<activofijoDto> objData = query.ToList();
                        pobjOperationResult.Success = 1;
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

        public void EliminarActivoFijo(ref OperationResult pobjOperationResult, string pstrIdActivoFijo, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();


                    #region Elimina Cabecera
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.activofijo
                                           where a.v_IdActivoFijo == pstrIdActivoFijo
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;
                    #endregion

                    #region Elimina Detalles
                    //Eliminar detalles del movimiento eliminado.
                    var objEntitySourceDetallesActivoFijo = (from a in dbContext.activofijodetalle
                                                             where a.v_IdActivoFijo == pstrIdActivoFijo
                                                             select a).ToList();

                    foreach (var ActivoFijoDetalle in objEntitySourceDetallesActivoFijo)
                    {
                        ActivoFijoDetalle.t_ActualizaFecha = DateTime.Now;
                        ActivoFijoDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        ActivoFijoDetalle.i_Eliminado = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "activofijodetalle", ActivoFijoDetalle.v_IdActivoFijoDetalle);
                    }
                    #endregion

                    #region Elimina Anexo

                    var objEntitySourceAnexoActivoFijo = (from a in dbContext.activofijoanexo
                                                             where a.v_IdActivoFijo == pstrIdActivoFijo
                                                             select a).ToList();

                    foreach (var ActivoFijoAnexo in objEntitySourceAnexoActivoFijo)
                    {
                        ActivoFijoAnexo.t_ActualizaFecha = DateTime.Now;
                        ActivoFijoAnexo.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        ActivoFijoAnexo.i_Eliminado = 1;
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "activofijoanexo", ActivoFijoAnexo.i_IdActivoFijoAnexo.ToString ());
                    }
                    #endregion

                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "activofijo", objEntitySource.v_IdActivoFijo);
                    pobjOperationResult.Success = 1;
                    ts.Complete();
                    return;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ActivoFijoBL.EliminarActivoFijo()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }

        public string BuscarOrigen(string IdProductoDetalle)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {

                var origen = (from a in dbContext.compra

                              join b in dbContext.compradetalle on new { IdCompra = a.v_IdCompra, eliminado = 0 } equals new { IdCompra = b.v_IdCompra, eliminado = b.i_Eliminado.Value } into b_join

                              from b in b_join.DefaultIfEmpty()

                              where a.i_Eliminado == 0 && b.v_IdProductoDetalle == IdProductoDetalle

                              select a != null ? "C" + " " + a.v_Mes.Trim() + "-" + a.v_Correlativo.Trim() : null).FirstOrDefault();

                return origen;
            }


        }


        #endregion

        #region Reportes

        public List<actaentregaDto> ReporteActaEntrega(ref OperationResult objOperationResult, string pstrFilterExpression, string GrupoLlave, string Orden)
        {
            try
            {

                objOperationResult.Success = 1;


                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var ActivosxAnio = (from n in dbContext.activofijo

                                        join a in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, TipoActivo = n.i_IdTipoActivo.Value } equals new { Grupo = a.i_GroupId, eliminado = a.i_IsDeleted.Value, TipoActivo = a.i_ItemId } into a_join

                                        from a in a_join.DefaultIfEmpty()

                                        join b in dbContext.datahierarchy on new { Grupo = 105, eliminado = 0, Estado = n.i_IdEstado.Value } equals new { Grupo = b.i_GroupId, eliminado = b.i_IsDeleted.Value, Estado = b.i_ItemId } into b_join

                                        from b in b_join.DefaultIfEmpty()

                                        join c in dbContext.datahierarchy on new { Grupo = 103, eliminado = 0, Ubicacion = n.i_IdUbicacion.Value } equals new { Grupo = c.i_GroupId, eliminado = c.i_IsDeleted.Value, Ubicacion = c.i_ItemId } into c_join

                                        from c in c_join.DefaultIfEmpty()

                                        join d in dbContext.cliente on new { Flag = "T", eliminado = 0, IdCliente = n.v_IdResponsable } equals new { Flag = d.v_FlagPantalla, eliminado = d.i_Eliminado.Value, IdCliente = d.v_IdCliente } into d_join

                                        from d in d_join.DefaultIfEmpty()
                                        where n.i_Eliminado == 0 && n.i_EsTemporal == 0

                                        select new actaentregaDto
                                        {
                                            AnioCompra = n.t_FechaOrdenCompra.Value.Year,
                                            CodigoActivoFijo = n.v_CodigoActivoFijo.Trim(),
                                            DescripcionActivoFijo = n.v_Descricpion.Trim(),
                                            i_Baja = n.i_Baja.Value,
                                            AnioBaja = n.i_Baja == 1 ? n.t_FechaBaja.Value.Year : 0,
                                            CodTipoActivo = a == null ? "" : a.v_Value2,
                                            TipoActivo = a == null ? "" : a.v_Value2 + " " + a.v_Value1,
                                            Marca = n.v_Marca,
                                            Modelo = n.v_Modelo,
                                            Serie = n.v_Serie,
                                            Placa = n.v_Placa,
                                            Color = n.v_Color,
                                            Estado = b.v_Value1,
                                            CodUbicacion = c == null ? "" : c.v_Value2,
                                            Ubicacion = c == null ? "" : c.v_Value2 + " " + c.v_Value1,
                                            CodResponsable = d == null ? "" : d.v_CodCliente,
                                            Responsable = d == null ? "" : d.v_CodCliente.Trim() + " " + (d.v_ApePaterno.Trim() + " " + d.v_ApeMaterno.Trim() + " " + d.v_PrimerNombre.Trim() + " " + d.v_RazonSocial.Trim()).Trim(),
                                            GrupoLlave = GrupoLlave == "Responsable" ? d == null ? "**RESPONSABLE NO EXISTE**" : "RESPONSABLE : " + (d.v_CodCliente.Trim() + " " + d.v_ApePaterno.Trim() + " " + d.v_ApeMaterno.Trim() + " " + d.v_PrimerNombre.Trim() + " " + d.v_RazonSocial.Trim()).Trim() : "",
                                        }).AsQueryable();
                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        ActivosxAnio = ActivosxAnio.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(Orden))
                    {
                        ActivosxAnio = ActivosxAnio.OrderBy(Orden);
                    }
                    List<actaentregaDto> objData = ActivosxAnio.ToList();
                    return objData;

                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public List<ReporteRelacionBienesDto> ReporteRelacionBienes(ref OperationResult objOperationResult, string pstrFilterExpression, string GrupoLLave1, string GrupoLlave2, string Orden, int PeriodoReporte, int MesReporte, int CodigoDesde, int CodigoHasta)
        {
            try
            {
                objOperationResult.Success = 1;

                List<ReporteRelacionBienesDto> ReporteFinal = new List<ReporteRelacionBienesDto>();
                DateTime future = DateTime.Parse(DateTime.DaysInMonth(PeriodoReporte, MesReporte).ToString() + "/" + MesReporte.ToString() + "/" + PeriodoReporte.ToString() + " 23:59");
                ReporteRelacionBienesDto objReporte = new ReporteRelacionBienesDto();
                DateTime FechaFinPeriodoAnterior = DateTime.Parse("31/12/" + (PeriodoReporte - 1).ToString() + " 23:59");
                DateTime FechaInicioPeriodoAnterior = DateTime.Parse("01/01/ " + (PeriodoReporte - 1).ToString());
                DateTime FechaInicioPeriodoActual = DateTime.Parse("01/01/" + PeriodoReporte.ToString());
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {


                    var ListaReporteActivo = ReporteLibroActivo(ref objOperationResult, PeriodoReporte, MesReporte, CodigoDesde, CodigoHasta, true, pstrFilterExpression);
                    var ActivosActuales = ListaReporteActivo.GroupBy(l => new { l.v_IdActivoFijo }).Select(d =>
                    {
                        var k = d.LastOrDefault();
                        k.DepreciacionEjercicio = d.Sum(h => h.d_ImporteMensualDepreciacion.Value);
                        k.d_AjusteDepreciacion = d.Sum(h => h.d_AjusteDepreciacion.Value);
                        return k;
                    }).ToList();

                    if (objOperationResult.Success == 0) return null;
                    foreach (var item in ActivosActuales)
                    {
                        objReporte = new ReporteRelacionBienesDto();
                        objReporte.v_IdActivoFijo = item.v_IdActivoFijo;
                        objReporte.CodigoActivoFijo = item.CodigoActivoFijo;
                        objReporte.Foto = item.Foto;
                        objReporte.DescripcionActivoFijo = item.SoloDescripionActivo;
                        objReporte.TipoActivo = item.TipoActivo;
                        objReporte.Marca = item.v_Marca;
                        objReporte.Modelo = item.v_Modelo;
                        objReporte.Placa = item.v_Placa;
                        objReporte.Serie = item.v_Serie;
                        objReporte.FechaCompra = item.FechaCompra;
                        objReporte.FechaUso = item.FechaUso;
                        objReporte.Factura = item.v_NumeroFactura;
                        objReporte.CodCentroCosto = item.CentroCosto;
                        objReporte.Cuenta33 = item.Cuenta33;
                        objReporte.sFechaAdquisicion = item.FechaCompra.ToShortDateString();
                        objReporte.TotalMeses = item.MesesDepreciados.Value;
                        objReporte.ValorCompra = item.ValorAdquisicionHistorico.Value;
                        objReporte.ValorHistorico = objReporte.ValorCompra;
                        objReporte.Adquisicion = item.TipoAdquisicion;
                        objReporte.DepreciacionEjercicio = item.DepreciacionEjercicio;
                        objReporte.Ajuste = item.d_AjusteDepreciacion ?? 0;
                        objReporte.AdquisionesAdicionales = 0;
                        objReporte.Mejoras = 0;
                        objReporte.Bajas = 0;
                        objReporte.Responsable = item.Responsable;
                        objReporte.DepreciacionAcumulada = item.d_AcumuladoTotal ?? 0;
                        objReporte.PorcentajeDepreciacion = item.PorcentajeDepreciacion;
                        objReporte.MesesPorcentajeDepreciacion = item.MesesPorcentajeDepreciacion;
                        objReporte.Ubicacion = DescripcionUbicacion(item.Ubicacion, objOperationResult);
                        if (objOperationResult.Success == 0) return null;
                        objReporte.DepreciacionCierreAnterior = item.AcumuladoAnterior ?? 0;
                        objReporte.GrupoLlave1 = GrupoLLave1 == "Responsable" ? "RESPONSABLE : " + objReporte.Responsable : GrupoLLave1 == "Centro" ? "CENTRO COSTO : " + objReporte.CodCentroCosto + " " + objReporte.CentroCosto : "";
                        objReporte.GrupoLlave2 = GrupoLlave2 == "Ubicacion" ? "UBICACION : " + objReporte.Ubicacion : "";
                        ReporteFinal.Add(objReporte);
                    }

                }
                return ReporteFinal.OrderBy(o => o.GrupoLlave1).ThenBy(o => o.GrupoLlave2).ThenBy(o => o.CodigoActivoFijo).ToList();
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                return null;
            }
        }

        public string DescripcionUbicacion(string CodUbicacion, OperationResult objOperationResult)
        {

            string DescripcionUbicacion = "";
            int IdUbicacion = ObtenerTipoActivoFijo(CodUbicacion, 103);
            if (IdUbicacion != -1)
            {
                var Ubicaciones = new DatahierarchyBL().GetDataHierarchiesPagedAndFiltered(ref  objOperationResult, "", "", 103);
                var codigoAct = new DatahierarchyBL().GetDataHierarchy(ref objOperationResult, 103, IdUbicacion).v_Value2.Substring(0, 3);
                var local = Ubicaciones.Where(i => i.i_Header == 1 && i.v_Value2.Substring(0, 3) == codigoAct).ToList();
                var slocal = local.Any() ? Ubicaciones.Where(i => i.i_Header == 1 && i.v_Value2.Substring(0, 3) == codigoAct).Select(y => y.v_Value1).FirstOrDefault().Trim() : "";
                var area = Ubicaciones.Where(i => i.i_Header != 1 && (i.v_Value2.Substring(0, 3) == CodUbicacion.Substring(0, 3)) && i.v_Value2.Substring(i.v_Value2.Length - 1, 1) == "0").Select(y => y.v_Value1).FirstOrDefault();
                var sArea = area;
                var ubic = Ubicaciones.Where(x => x.v_Value2 == CodUbicacion).ToList();
                var Oficina = ubic.Any() ? ubic.Select(y => y.v_Value1).FirstOrDefault().Trim() : "";
                DescripcionUbicacion = slocal + " - " + sArea + " - " + Oficina;

            }
            return DescripcionUbicacion;
        }



        public List<ReporteLibroActivoDto> ReporteLibroActivo_(ref  OperationResult objOperationResult, int PeriodoReporte, int MesReporte, int CodigoDesde, int CodigoHasta, bool UsoOtrosReportes, string Filtro)
        {
            try
            {
                objOperationResult.Success = 1;
                decimal AjusteDepreciacionActivo = 0;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    DateTime future = new DateTime();
                    if (UsoOtrosReportes)
                    {
                        future = DateTime.Parse(DateTime.DaysInMonth(PeriodoReporte, MesReporte) + "/" + MesReporte + "/" + PeriodoReporte + " 23:59");
                    }
                    else
                    {
                        if (PeriodoReporte < DateTime.Now.Date.Year)
                        {

                            future = DateTime.Parse("31/12/" + PeriodoReporte + " 23:59");
                        }
                        else
                        {
                            future = DateTime.Parse(DateTime.DaysInMonth(PeriodoReporte, MesReporte) + "/" + MesReporte + "/" + PeriodoReporte + " 23:59"); //FechaReporte
                        }
                    }

                    List<ReporteLibroActivoDto> ActivosCierreAnterior = AcumuladoPeriodoAnterior(ref objOperationResult, PeriodoReporte - 1, CodigoDesde, CodigoHasta, UsoOtrosReportes, Filtro);

                    List<ReporteLibroActivoDto> ListaActivoFijoDepreciacion = new List<ReporteLibroActivoDto>();
                    List<activofijoDto> ActivosTotal = new List<activofijoDto>();
                    ReporteLibroActivoDto objReporte = new ReporteLibroActivoDto();
                    DateTime FechaInicio = new DateTime();
                    var ActivosPeriodoActualTodos = (from a in dbContext.activofijo

                                                     join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join
                                                     from b in b_join.DefaultIfEmpty()

                                                     join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                     from c in c_join.DefaultIfEmpty()

                                                     join d in dbContext.datahierarchy on new { cc = a.i_IdCentroCosto.Value, eliminado = 0, grupo = 31 } equals new { cc = d.i_ItemId, eliminado = d.i_IsDeleted.Value, grupo = d.i_GroupId } into d_join
                                                     from d in d_join.DefaultIfEmpty()
                                                     join e in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, tipoactivo = a.i_IdTipoActivo.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, tipoactivo = e.i_ItemId } into e_join
                                                     from e in e_join.DefaultIfEmpty()

                                                     join f in dbContext.datahierarchy on new { Grupo = 107, eliminado = 0, tipoactivo = a.i_IdTipoAdquisicion.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, tipoactivo = f.i_ItemId } into f_join
                                                     from f in f_join.DefaultIfEmpty()

                                                     where a.i_Eliminado == 0 && a.i_Depreciara == 1 && a.t_FechaUso != null && a.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoActual
                                                     && a.t_FechaOrdenCompra.Value.Year <= PeriodoReporte && a.i_EsTemporal == 0  //Se agrego porque en el proceso depreciacion no se toma en cuenta

                                                     select new activofijoDto
                                                     {
                                                         v_IdActivoFijo = a.v_IdActivoFijo,
                                                         d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                         mesesAdepreciar = b.v_Value1,
                                                         t_FechaUso = a.t_FechaUso,
                                                         i_Baja = a.i_Baja == null ? 0 : a.i_Baja,
                                                         t_FechaBaja = a.t_FechaBaja,
                                                         v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                         v_Descricpion = a.v_Descricpion.Trim(),
                                                         v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                         t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                         CodigoActivoFijo = a.v_CodigoActivoFijo,
                                                         Cuenta39 = c == null ? "0" : c.v_Cuenta39,
                                                         Cuenta33 = c == null ? "0" : c.v_Cuenta33,
                                                         CuentaGasto = c == null ? "0" : c.v_CuentaGasto,
                                                         i_MesesDepreciadosPAnterior = a.i_MesesDepreciadosPAnterior,
                                                         i_IdTipoMotivo = a.i_IdTipoMotivo.Value,
                                                         v_PeriodoAnterior = string.IsNullOrEmpty(a.v_PeriodoAnterior) ? "0" : a.v_PeriodoAnterior.Trim(),
                                                         d_ValorAdquisicionMe = a.d_ValorAdquisicionMe == null ? 0 : a.d_ValorAdquisicionMe,
                                                         CentroCosto = d.v_Value2,
                                                         i_IdCentroCosto = a.i_IdCentroCosto,
                                                         CodTipoActivo = e.v_Value2,
                                                         v_Modelo = a.v_Modelo,
                                                         v_Marca = a.v_Marca,
                                                         v_Serie = a.v_Serie,
                                                         v_Placa = a.v_Placa,
                                                         TipoAdquisicion = f.v_Value2,
                                                         i_Depreciara = a.i_Depreciara ?? 0,
                                                         PorcentajeDepreciacion = b != null ? b.v_Value2 : "",


                                                     }).ToList().AsQueryable();

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ActivosPeriodoActualTodos = ActivosPeriodoActualTodos.Where(Filtro);

                    }

                    var ActivosPeriodoAnteriorTodos = (from a in dbContext.activofijo
                                                       join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join

                                                       from b in b_join.DefaultIfEmpty()
                                                       join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                       from c in c_join.DefaultIfEmpty()

                                                       join d in dbContext.datahierarchy on new { cc = a.i_IdCentroCosto.Value, eliminado = 0, grupo = 31 } equals new { cc = d.i_ItemId, eliminado = d.i_IsDeleted.Value, grupo = d.i_GroupId } into d_join
                                                       from d in d_join.DefaultIfEmpty()
                                                       join e in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, tipoactivo = a.i_IdTipoActivo.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, tipoactivo = e.i_ItemId } into e_join
                                                       from e in e_join.DefaultIfEmpty()

                                                       join f in dbContext.datahierarchy on new { Grupo = 107, eliminado = 0, tipoactivo = a.i_IdTipoAdquisicion.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, tipoactivo = f.i_ItemId } into f_join
                                                       from f in f_join.DefaultIfEmpty()


                                                       where a.i_Eliminado == 0 && a.i_Depreciara == 1 && a.t_FechaUso != null && a.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior
                                                        && a.i_EsTemporal == 0 // && a.v_Periodo == strPeriodo
                                                       select new activofijoDto
                                                       {

                                                           v_IdActivoFijo = a.v_IdActivoFijo,
                                                           d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                           mesesAdepreciar = b.v_Value1,
                                                           t_FechaUso = a.t_FechaUso,
                                                           i_Baja = a.i_Baja == null ? 0 : a.i_Baja,
                                                           t_FechaBaja = a.t_FechaBaja,
                                                           v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                           v_Descricpion = a.v_Descricpion.Trim(),
                                                           v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                           t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                           Cuenta39 = c == null ? "0" : c.v_Cuenta39,
                                                           CodigoActivoFijo = a.v_CodigoActivoFijo,
                                                           Cuenta33 = c == null ? "0" : c.v_Cuenta33,
                                                           i_MesesDepreciadosPAnterior = a.i_MesesDepreciadosPAnterior,
                                                           i_IdTipoMotivo = a.i_IdTipoMotivo.Value,
                                                           v_PeriodoAnterior = string.IsNullOrEmpty(a.v_PeriodoAnterior) ? "0" : a.v_PeriodoAnterior.Trim(),
                                                           d_ValorAdquisicionMe = a.d_ValorAdquisicionMe == null ? 0 : a.d_ValorAdquisicionMe,
                                                           CuentaGasto = c == null ? "0" : c.v_CuentaGasto,
                                                           CentroCosto = d.v_Value2,
                                                           i_IdCentroCosto = a.i_IdCentroCosto,
                                                           CodTipoActivo = e.v_Value2,
                                                           v_Modelo = a.v_Modelo,
                                                           v_Marca = a.v_Marca,
                                                           v_Serie = a.v_Serie,
                                                           v_Placa = a.v_Placa,
                                                           TipoAdquisicion = f.v_Value2,
                                                           i_Depreciara = a.i_Depreciara ?? 0,
                                                           PorcentajeDepreciacion = b != null ? b.v_Value2 : "",
                                                       }).ToList().AsQueryable();

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ActivosPeriodoAnteriorTodos = ActivosPeriodoAnteriorTodos.Where(Filtro);

                    }


                    var ActivosNoDepreciaran = (from a in dbContext.activofijo

                                                join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join
                                                from b in b_join.DefaultIfEmpty()

                                                join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                from c in c_join.DefaultIfEmpty()

                                                join d in dbContext.datahierarchy on new { cc = a.i_IdCentroCosto.Value, eliminado = 0, grupo = 31 } equals new { cc = d.i_ItemId, eliminado = d.i_IsDeleted.Value, grupo = d.i_GroupId } into d_join
                                                from d in d_join.DefaultIfEmpty()
                                                join e in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, tipoactivo = a.i_IdTipoActivo.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, tipoactivo = e.i_ItemId } into e_join
                                                from e in e_join.DefaultIfEmpty()

                                                join f in dbContext.datahierarchy on new { Grupo = 107, eliminado = 0, tipoactivo = a.i_IdTipoAdquisicion.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, tipoactivo = f.i_ItemId } into f_join
                                                from f in f_join.DefaultIfEmpty()

                                                where a.i_Eliminado == 0 && a.i_Depreciara == 0
                                                select new activofijoDto
                                                {
                                                    v_IdActivoFijo = a.v_IdActivoFijo,
                                                    d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                    mesesAdepreciar = b.v_Value1,
                                                    t_FechaUso = a.t_FechaUso,
                                                    i_Baja = a.i_Baja == null ? 0 : a.i_Baja,
                                                    t_FechaBaja = a.t_FechaBaja,
                                                    v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                    v_Descricpion = a.v_Descricpion.Trim(),
                                                    v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                    t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                    CodigoActivoFijo = a.v_CodigoActivoFijo,
                                                    Cuenta39 = c == null ? "0" : c.v_Cuenta39,
                                                    Cuenta33 = c == null ? "0" : c.v_Cuenta33,
                                                    CuentaGasto = c == null ? "0" : c.v_CuentaGasto,
                                                    i_MesesDepreciadosPAnterior = a.i_MesesDepreciadosPAnterior,
                                                    i_IdTipoMotivo = a.i_IdTipoMotivo.Value,
                                                    v_PeriodoAnterior = string.IsNullOrEmpty(a.v_PeriodoAnterior) ? "0" : a.v_PeriodoAnterior.Trim(),
                                                    d_ValorAdquisicionMe = a.d_ValorAdquisicionMe == null ? 0 : a.d_ValorAdquisicionMe,
                                                    CentroCosto = d.v_Value2,
                                                    i_IdCentroCosto = a.i_IdCentroCosto,
                                                    CodTipoActivo = e.v_Value2,
                                                    v_Modelo = a.v_Modelo,
                                                    v_Marca = a.v_Marca,
                                                    v_Serie = a.v_Serie,
                                                    v_Placa = a.v_Placa,
                                                    TipoAdquisicion = f.v_Value2,

                                                    i_Depreciara = a.i_Depreciara ?? 0,
                                                    PorcentajeDepreciacion = b != null ? b.v_Value2 : "",
                                                }).ToList().AsQueryable();
                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ActivosNoDepreciaran = ActivosNoDepreciaran.Where(Filtro);

                    }

                    if (CodigoDesde != 0 && CodigoHasta != 0)
                    {
                        var ActivosPeriodoAnterior = (from a in ActivosPeriodoAnteriorTodos

                                                      where int.Parse(a.v_CodigoActivoFijo) >= CodigoDesde && int.Parse(a.v_CodigoActivoFijo) <= CodigoHasta

                                                      select a);



                        var ActivosPeriodoActual = (from a in ActivosPeriodoActualTodos

                                                    where int.Parse(a.v_CodigoActivoFijo) >= CodigoDesde && int.Parse(a.v_CodigoActivoFijo) <= CodigoHasta

                                                    select a);


                        var ActivosQueNoSeDepreciaran = (from a in ActivosNoDepreciaran
                                                         where int.Parse(a.v_CodigoActivoFijo) >= CodigoDesde && int.Parse(a.v_CodigoActivoFijo) <= CodigoHasta

                                                         select a);

                        ActivosTotal = ActivosPeriodoAnterior.Concat(ActivosPeriodoActual).Concat(ActivosQueNoSeDepreciaran).ToList().OrderBy(o => o.CodigoActivoFijo).ToList();
                    }
                    else
                    {
                        ActivosTotal = ActivosPeriodoActualTodos.Concat(ActivosPeriodoAnteriorTodos).Concat(ActivosNoDepreciaran).ToList().OrderBy(o => o.CodigoActivoFijo).ToList();
                    }
                    foreach (var item in ActivosTotal.AsParallel())
                    {
                        if (item.CodigoActivoFijo == "10000102")
                        {
                            string g = "";
                        }
                        AjusteDepreciacionActivo = 0;
                        decimal AcumuladoHistorico = 0;
                        DateTime past = item.t_FechaUso.Value.Date; // Fecha Uso
                        decimal valoradquisicion = item.d_MonedaNacional.Value;
                        int mesesAdepreciar = string.IsNullOrEmpty(item.mesesAdepreciar) ? 0 : int.Parse(item.mesesAdepreciar);
                        int anioInicio = int.Parse(future.Date.Year.ToString()); // future .... Ultimo dia de la fecha del reporte 31/08/2015
                        int MesesDepreciados = 0;
                        int anioFin = int.Parse(future.Date.Year.ToString());
                        while (anioInicio <= anioFin)
                        {
                            int MesDepreciar = item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior && PeriodoReporte == int.Parse(item.v_PeriodoAnterior) ? 12 : 1;
                            for (int i = MesDepreciar; i <= 12; i++)
                            {
                                if (i <= int.Parse(future.Date.Month.ToString()) || anioInicio < anioFin)
                                {


                                    objReporte = new ReporteLibroActivoDto();
                                    objReporte.NombreMes = new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(i).ToUpper();
                                    objReporte.Cuenta33 = item.Cuenta33;
                                    objReporte.Cuenta39 = item.Cuenta39;
                                    objReporte.v_IdActivoFijo = item.v_IdActivoFijo;
                                    objReporte.i_Baja = item.i_Baja.Value;
                                    objReporte.AnioBaja = item.t_FechaBaja == null ? "" : item.t_FechaBaja.Value.Year.ToString();
                                    objReporte.FechaUso = item.t_FechaUso.Value;
                                    objReporte.sFechaUso = item.i_Depreciara == 1 ? objReporte.FechaUso.ToShortDateString() : "";
                                    objReporte.FechaCompra = item.t_FechaOrdenCompra.Value;
                                    objReporte.CuentaGasto = item.CuentaGasto;
                                    objReporte.CentroCosto = item.CentroCosto;
                                    objReporte.CodigoActivoFijo = item.CodigoActivoFijo;
                                    objReporte.FechaBaja = item.t_FechaBaja == null ? DateTime.Now : item.t_FechaBaja.Value;
                                    objReporte.i_IdCentroCosto = item.i_IdCentroCosto.Value;
                                    objReporte.TipoActivo = item.CodTipoActivo;
                                    objReporte.v_NumeroFactura = item.v_NumeroFactura;
                                    objReporte.i_Depreciara = item.i_Depreciara.Value;
                                    objReporte.v_Serie = item.v_Serie;
                                    objReporte.v_Marca = item.v_Marca;
                                    objReporte.v_Modelo = item.v_Modelo;
                                    objReporte.v_Placa = item.v_Placa;
                                    objReporte.TipoAdquisicion = item.TipoAdquisicion;
                                    objReporte.AnioCompra = item.t_FechaOrdenCompra == null ? "" : item.t_FechaOrdenCompra.Value.Year.ToString();
                                    objReporte.MesCompra = item.t_FechaOrdenCompra == null ? "" : item.t_FechaOrdenCompra.Value.Month.ToString();
                                    objReporte.AnioUso = item.t_FechaUso == null ? "" : item.t_FechaUso.Value.Year.ToString();
                                    objReporte.MesesDepreciar = string.IsNullOrEmpty(item.mesesAdepreciar) ? "0" : item.mesesAdepreciar;
                                    string x = ("CÓDIGO : " + item.v_CodigoActivoFijo + "  ,  " + "DESCRIPCIÓN : " + item.v_Descricpion + "  ,  " + "FACTURA : " + item.v_NumeroFactura + "  ,  " + "FECHA ADQUISICIÓN : " + item.t_FechaOrdenCompra.Value.Date.Day.ToString("00") + "/" + item.t_FechaOrdenCompra.Value.Month.ToString("00") + "/"
                                        + item.t_FechaOrdenCompra.Value.Year.ToString() + "  ,  " + "FECHA USO : ");
                                    string y = (item.t_FechaUso == null ? "" : item.t_FechaUso.Value.Date.Day.ToString("00") + "/" + item.t_FechaUso.Value.Date.Month.ToString("00") + "/" + item.t_FechaUso.Value.Date.Year.ToString() + "  ,  " + "\n" + "MESES A DEPRECIAR : " + mesesAdepreciar.ToString());
                                    string baja = item.t_FechaBaja == null ? " " : " FECHA BAJA : " + item.t_FechaBaja.Value.Date.Day.ToString("00") + "/" + item.t_FechaBaja.Value.Month.ToString("00") + "/" + item.t_FechaBaja.Value.Date.Year.ToString();
                                    objReporte.DescripcionActivo = (x + y + baja).ToUpper();
                                    objReporte.SoloDescripionActivo = item.v_Descricpion.Trim();
                                    objReporte.ValorAdquisicionHistorico = item.d_MonedaNacional.Value;
                                    objReporte.ValorAdquisicionHistoricoFinal = item.d_MonedaNacional.Value;
                                    objReporte.PorcentajeDepreciacion = item.PorcentajeDepreciacion;
                                    decimal ImporteMensualHistorico = mesesAdepreciar == 0 ? 0 : Math.Round(valoradquisicion / mesesAdepreciar, 2);

                                    if (item.i_Depreciara == 0)
                                    {
                                        MesesDepreciados = 0;
                                        objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                        objReporte.d_AcumuladoHistorico = 0;
                                        objReporte.d_ValorNetoActual = valoradquisicion;
                                        objReporte.d_Comparacion = 0;
                                        objReporte.d_AjusteDepreciacion = 0;
                                        objReporte.d_ImporteMensualDepreciacion = 0;
                                        objReporte.d_Importe1SoloMes = objReporte.d_ImporteMensualDepreciacion.Value;
                                        AjusteDepreciacionActivo = objReporte.d_AjusteDepreciacion.Value == 0 ? AjusteDepreciacionActivo : objReporte.d_AjusteDepreciacion.Value;

                                    }
                                    else
                                    {

                                        #region ActivoSinBaja
                                        if (item.i_Baja == 0)
                                        {

                                            if (past.Date.Month == 12)
                                            {
                                                FechaInicio = DateTime.Parse("01/01/" + (past.Date.Year + 1).ToString());
                                            }
                                            else
                                            {
                                                FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
                                            }
                                            DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());

                                            if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                            {
                                                if (PeriodoReporte == int.Parse(item.v_PeriodoAnterior) + 1)
                                                {
                                                    MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 1 : MesesDepreciados + 1;
                                                }
                                                else if (PeriodoReporte < int.Parse(item.v_PeriodoAnterior) + 1)
                                                {
                                                    //MesesDepreciados = item.i_MesesDepreciadosPAnterior.Value;
                                                    MesesDepreciados = 0;
                                                }
                                                else
                                                {
                                                    var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()) + 1));
                                                    MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + (12 * (DiferenciaAnios)) + 1 : MesesDepreciados + 1;
                                                }

                                            }
                                            else
                                            {
                                                MesesDepreciados = int.Parse((Math.Round((FechaFin.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                                            }
                                            objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                            var AcumPeriodoAnterior = ActivosCierreAnterior.Where(o => o.v_IdActivoFijo == objReporte.v_IdActivoFijo);
                                            objReporte.AcumuladoAnterior = AcumPeriodoAnterior.Any() ? AcumPeriodoAnterior.LastOrDefault().d_AcumuladoHistorico.Value : 0;
                                            objReporte.d_AjusteDepreciacion = 0;

                                            var faltantres = mesesAdepreciar - MesesDepreciados;
                                            var ad = (objReporte.ValorAdquisicionHistorico - ImporteMensualHistorico);
                                            //objReporte.d_Comparacion =  objReporte.MesesDepreciados == 0 && objReporte.d_AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : objReporte.d_ImporteMensualDepreciacion * (mesesAdepreciar - objReporte.MesesDepreciados);
                                            objReporte.AcumuladoComparacion = objReporte.AcumuladoAnterior == 0 ? objReporte.ValorAdquisicionHistorico - (MesesDepreciados * ImporteMensualHistorico) : objReporte.AcumuladoAnterior - (MesesDepreciados * ImporteMensualHistorico);
                                            objReporte.d_Comparacion = objReporte.MesesDepreciados == 0 && objReporte.d_AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : ImporteMensualHistorico * faltantres;
                                            objReporte.d_AjusteDepreciacion = objReporte.AcumuladoComparacion - objReporte.d_Comparacion;


                                            if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                            {
                                                objReporte.d_ImporteMensualDepreciacion = objReporte.MesesDepreciados == 0 ? 0 : mesesAdepreciar == 0 ? 0 : ImporteMensualHistorico;
                                                if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                {

                                                    if (i == 1 && item.v_PeriodoAnterior.Trim() == (PeriodoReporte - 1).ToString())
                                                    {
                                                        objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value + ImporteMensualHistorico;
                                                        objReporte.d_AcumuladoTotal = objReporte.d_AcumuladoHistorico + objReporte.d_AjusteDepreciacion;
                                                    }

                                                    else if (i == 12 && PeriodoReporte < (int.Parse(item.v_PeriodoAnterior) + 1))
                                                    {
                                                        objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value;
                                                        objReporte.d_AcumuladoTotal = objReporte.d_AcumuladoHistorico + objReporte.d_AjusteDepreciacion;
                                                    }
                                                    else if (PeriodoReporte > int.Parse(item.v_PeriodoAnterior))
                                                    {
                                                        var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()))) - 1;
                                                        objReporte.AcumuladoAnterior = (objReporte.d_ImporteMensualDepreciacion.Value * 12 * DiferenciaAnios) + item.d_ValorAdquisicionMe;
                                                        objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior.Value + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;
                                                        objReporte.d_AcumuladoTotal = objReporte.d_AcumuladoHistorico + objReporte.d_AjusteDepreciacion;

                                                    }
                                                    else
                                                    {
                                                        objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;
                                                        objReporte.d_AcumuladoTotal = objReporte.d_AcumuladoHistorico + objReporte.d_AjusteDepreciacion;
                                                    }

                                                }
                                                else
                                                {
                                                    objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion.Value;
                                                    objReporte.d_AcumuladoTotal = objReporte.d_AcumuladoHistorico + objReporte.d_AjusteDepreciacion;
                                                }

                                                objReporte.d_AcumuladoTotal = objReporte.d_AcumuladoHistorico + objReporte.d_AjusteDepreciacion;

                                                AcumuladoHistorico = objReporte.d_AcumuladoHistorico.Value;
                                                if (i != 1 && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                {
                                                    objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoHistorico;
                                                }
                                                else
                                                {

                                                    //objReporte.d_ValorNetoActual = AjusteDepreciacionActivo != 0 ? (valoradquisicion - objReporte.d_AcumuladoHistorico) + AjusteDepreciacionActivo : (valoradquisicion - objReporte.d_AcumuladoHistorico);
                                                    objReporte.d_ValorNetoActual = AjusteDepreciacionActivo != 0 ? (valoradquisicion - objReporte.d_AcumuladoHistorico) : (valoradquisicion - objReporte.d_AcumuladoHistorico);
                                                }



                                                // objReporte.d_AjusteDepreciacion = objReporte.d_ValorNetoActual-objReporte.d_Comparacion ;

                                                if (objReporte.MesesDepreciados == mesesAdepreciar)
                                                {
                                                    objReporte.d_AcumuladoHistorico = objReporte.d_AcumuladoHistorico + objReporte.d_ValorNetoActual;
                                                    objReporte.d_ValorNetoActual = objReporte.d_ValorNetoActual + objReporte.d_AjusteDepreciacion;
                                                    objReporte.d_AjusteDepreciacion = 0;
                                                }

                                            }
                                            else
                                            {
                                                objReporte.d_ImporteMensualDepreciacion = 0;
                                                objReporte.d_AcumuladoHistorico = AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : AcumuladoHistorico;
                                                objReporte.d_ValorNetoActual = 0;
                                                objReporte.d_Comparacion = 0;
                                                objReporte.d_AjusteDepreciacion = 0;

                                            }

                                            objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados > int.Parse(item.mesesAdepreciar) ? int.Parse(item.mesesAdepreciar) : MesesDepreciados;


                                        }
                                        #endregion
                                        else
                                        #region ActivoBaja
                                        {
                                            if (anioInicio < int.Parse(item.t_FechaBaja.Value.Year.ToString())) // No se toma en cuenta el mes de Baja
                                            {

                                                if (past.Date.Month == 12)
                                                {
                                                    FechaInicio = DateTime.Parse("01/01/" + (past.Date.Year + 1).ToString());
                                                }
                                                else
                                                {
                                                    FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
                                                }
                                                DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());

                                                if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                {
                                                    if (PeriodoReporte >= int.Parse(item.v_PeriodoAnterior) + 1)
                                                    {
                                                        MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 1 : MesesDepreciados + 1;
                                                    }
                                                    else if (PeriodoReporte < int.Parse(item.v_PeriodoAnterior) + 1)
                                                    {
                                                        //MesesDepreciados = item.i_MesesDepreciadosPAnterior.Value;
                                                        MesesDepreciados = 0;
                                                    }
                                                    else
                                                    {
                                                        MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 12 : MesesDepreciados + 1;
                                                    }

                                                }
                                                else
                                                {
                                                    MesesDepreciados = int.Parse((Math.Round((FechaFin.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                                                }
                                                objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;


                                                var AcumPeriodoAnterior = ActivosCierreAnterior.Where(o => o.v_IdActivoFijo == objReporte.v_IdActivoFijo);
                                                objReporte.AcumuladoAnterior = AcumPeriodoAnterior.Any() ? AcumPeriodoAnterior.LastOrDefault().d_AcumuladoHistorico.Value : 0;
                                                if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                                {


                                                    objReporte.d_ImporteMensualDepreciacion = objReporte.MesesDepreciados == 0 ? 0 : mesesAdepreciar == 0 ? 0 : ImporteMensualHistorico;
                                                    if (i == 1 && item.v_PeriodoAnterior.Trim() == (PeriodoReporte - 1).ToString() && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value + ImporteMensualHistorico;
                                                    }

                                                    else if (i == 12 && PeriodoReporte < (int.Parse(item.v_PeriodoAnterior) + 1) && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value;
                                                    }
                                                    else if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior && PeriodoReporte > int.Parse(item.v_PeriodoAnterior))
                                                    {
                                                        var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()))) - 1;
                                                        objReporte.AcumuladoAnterior = (objReporte.d_ImporteMensualDepreciacion.Value * 12 * DiferenciaAnios) + item.d_ValorAdquisicionMe;
                                                        objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior.Value + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;


                                                    }

                                                    else
                                                    {
                                                        objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion.Value;
                                                    }
                                                    AcumuladoHistorico = objReporte.d_AcumuladoHistorico.Value;
                                                    if (i != 1 && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoHistorico;
                                                    }
                                                    else
                                                    {

                                                        objReporte.d_ValorNetoActual = AjusteDepreciacionActivo != 0 ? (valoradquisicion - objReporte.d_AcumuladoHistorico) + AjusteDepreciacionActivo : (valoradquisicion - objReporte.d_AcumuladoHistorico);
                                                    }
                                                    objReporte.d_Comparacion = objReporte.MesesDepreciados == 0 && objReporte.d_AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : objReporte.d_ImporteMensualDepreciacion * (mesesAdepreciar - objReporte.MesesDepreciados);
                                                    objReporte.d_AjusteDepreciacion = objReporte.d_ValorNetoActual - objReporte.d_Comparacion;


                                                    if (objReporte.MesesDepreciados == mesesAdepreciar)
                                                    {
                                                        objReporte.d_AcumuladoHistorico = objReporte.d_AcumuladoHistorico + objReporte.d_ValorNetoActual;
                                                        objReporte.d_ValorNetoActual = objReporte.d_ValorNetoActual + objReporte.d_AjusteDepreciacion;
                                                        objReporte.d_AjusteDepreciacion = 0;
                                                    }

                                                }
                                                else
                                                {
                                                    objReporte.d_ImporteMensualDepreciacion = 0;
                                                    objReporte.d_AcumuladoHistorico = 0;
                                                    objReporte.d_ValorNetoActual = 0;
                                                    objReporte.d_Comparacion = 0;
                                                    objReporte.d_AjusteDepreciacion = 0;
                                                    objReporte.ValorAdquisicionHistoricoFinal = 0;

                                                }
                                                objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados > int.Parse(item.mesesAdepreciar) ? int.Parse(item.mesesAdepreciar) : MesesDepreciados;
                                            }
                                            else
                                            {

                                                if (i <= int.Parse(item.t_FechaBaja.Value.Month.ToString()) && anioInicio == item.t_FechaBaja.Value.Year)
                                                {

                                                    if (past.Date.Month == 12)
                                                    {
                                                        FechaInicio = DateTime.Parse("01/01/" + (past.Date.Year + 1).ToString());
                                                    }
                                                    else
                                                    {
                                                        FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
                                                    }
                                                    DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());


                                                    if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        if (PeriodoReporte >= int.Parse(item.v_PeriodoAnterior) + 1)
                                                        {
                                                            MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 1 : MesesDepreciados + 1;
                                                        }
                                                        else if (PeriodoReporte < int.Parse(item.v_PeriodoAnterior) + 1)
                                                        {
                                                            //MesesDepreciados = item.i_MesesDepreciadosPAnterior.Value;
                                                            MesesDepreciados = 0;
                                                        }
                                                        else
                                                        {
                                                            MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 12 : MesesDepreciados + 1;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        MesesDepreciados = int.Parse((Math.Round((FechaFin.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                                                    }

                                                    objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                                    var AcumPeriodoAnterior = ActivosCierreAnterior.Where(o => o.v_IdActivoFijo == objReporte.v_IdActivoFijo);
                                                    objReporte.AcumuladoAnterior = AcumPeriodoAnterior.Any() ? AcumPeriodoAnterior.LastOrDefault().d_AcumuladoHistorico.Value : 0;

                                                    if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                                    {
                                                        objReporte.d_ImporteMensualDepreciacion = objReporte.MesesDepreciados == 0 ? 0 : mesesAdepreciar == 0 ? 0 : ImporteMensualHistorico;
                                                        if (i == 1 && item.v_PeriodoAnterior.Trim() == (PeriodoReporte - 1).ToString() && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                        {
                                                            objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value + ImporteMensualHistorico;
                                                        }

                                                        else if (i == 12 && PeriodoReporte < (int.Parse(item.v_PeriodoAnterior) + 1) && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                        {
                                                            objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value;
                                                        }
                                                        else if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior && PeriodoReporte > int.Parse(item.v_PeriodoAnterior))
                                                        {
                                                            var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()))) - 1;
                                                            objReporte.AcumuladoAnterior = (objReporte.d_ImporteMensualDepreciacion.Value * 12 * DiferenciaAnios) + item.d_ValorAdquisicionMe;
                                                            objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior.Value + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;


                                                        }
                                                        else
                                                        {
                                                            objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion.Value;
                                                        }
                                                        AcumuladoHistorico = objReporte.d_AcumuladoHistorico.Value;
                                                        if (i != 1 && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                        {
                                                            objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoHistorico;
                                                        }
                                                        else
                                                        {

                                                            objReporte.d_ValorNetoActual = AjusteDepreciacionActivo != 0 ? (valoradquisicion - objReporte.d_AcumuladoHistorico) + AjusteDepreciacionActivo : (valoradquisicion - objReporte.d_AcumuladoHistorico);
                                                        }

                                                        objReporte.d_Comparacion = objReporte.MesesDepreciados == 0 && objReporte.d_AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : objReporte.d_ImporteMensualDepreciacion * (mesesAdepreciar - objReporte.MesesDepreciados);
                                                        objReporte.d_AjusteDepreciacion = objReporte.d_ValorNetoActual - objReporte.d_Comparacion;




                                                        if (objReporte.MesesDepreciados == mesesAdepreciar)
                                                        {
                                                            objReporte.d_AcumuladoHistorico = objReporte.d_AcumuladoHistorico + objReporte.d_ValorNetoActual;
                                                            objReporte.d_ValorNetoActual = objReporte.d_ValorNetoActual + objReporte.d_AjusteDepreciacion;
                                                            objReporte.d_AjusteDepreciacion = 0;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        objReporte.d_ImporteMensualDepreciacion = 0;
                                                        objReporte.d_AcumuladoHistorico = 0;
                                                        objReporte.d_ValorNetoActual = 0;
                                                        objReporte.d_Comparacion = 0;
                                                        objReporte.d_AjusteDepreciacion = 0;
                                                        objReporte.ValorAdquisicionHistoricoFinal = 0;

                                                    }
                                                    objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados > int.Parse(item.mesesAdepreciar) ? int.Parse(item.mesesAdepreciar) : MesesDepreciados;

                                                }
                                                else
                                                {
                                                    objReporte.MesesDepreciados = 0;
                                                    objReporte.d_ImporteMensualDepreciacion = 0;
                                                    objReporte.d_AcumuladoHistorico = 0;
                                                    objReporte.d_ValorNetoActual = 0;
                                                    objReporte.d_Comparacion = 0;
                                                    objReporte.d_AjusteDepreciacion = 0;
                                                    objReporte.ValorAdquisicionHistoricoFinal = 0;
                                                    MesesDepreciados = 0;
                                                }
                                            }

                                        }

                                        #endregion
                                    }

                                    if (objReporte.MesesDepreciados >= 0 && MesesDepreciados >= 0)//&&  MesesDepreciados>=0  agregue 29 Diciembre
                                    {

                                        if (objReporte.CodigoActivoFijo == "10000102")
                                        {
                                            string h = "";
                                        }
                                        objReporte.d_Importe1SoloMes = objReporte.d_ImporteMensualDepreciacion.Value;
                                        ListaActivoFijoDepreciacion.Add(objReporte);
                                        AjusteDepreciacionActivo = objReporte.d_AjusteDepreciacion.Value == 0 ? AjusteDepreciacionActivo : objReporte.d_AjusteDepreciacion.Value;
                                    }

                                }
                            }

                            anioInicio = anioInicio + 1;
                        }

                    }
                    return ListaActivoFijoDepreciacion;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }


        public List<ReporteLibroActivoDto> ReporteLibroActivo(ref  OperationResult objOperationResult, int PeriodoReporte, int MesReporte, int CodigoDesde, int CodigoHasta, bool UsoOtrosReportes, string Filtro)
        {
            try
            {
                objOperationResult.Success = 1;
                decimal AjusteDepreciacionActivo = 0;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    DateTime future = new DateTime();
                    if (UsoOtrosReportes)
                    {
                        future = DateTime.Parse(DateTime.DaysInMonth(PeriodoReporte, MesReporte) + "/" + MesReporte + "/" + PeriodoReporte + " 23:59");
                    }
                    else
                    {
                        if (PeriodoReporte < DateTime.Now.Date.Year)
                        {

                            future = DateTime.Parse("31/12/" + PeriodoReporte + " 23:59");
                        }
                        else
                        {
                            future = DateTime.Parse(DateTime.DaysInMonth(PeriodoReporte, MesReporte) + "/" + MesReporte + "/" + PeriodoReporte + " 23:59"); //FechaReporte
                        }
                    }

                    List<ReporteLibroActivoDto> ActivosCierreAnterior = AcumuladoPeriodoAnterior(ref objOperationResult, PeriodoReporte - 1, CodigoDesde, CodigoHasta, UsoOtrosReportes, Filtro);

                    if (objOperationResult.Success == 0)
                    {
                        return null;
                    }
                    List<ReporteLibroActivoDto> ListaActivoFijoDepreciacion = new List<ReporteLibroActivoDto>();
                    List<activofijoDto> ActivosTotal = new List<activofijoDto>();
                    ReporteLibroActivoDto objReporte = new ReporteLibroActivoDto();
                    DateTime FechaInicio = new DateTime();
                    var ActivosPeriodoActualTodos = (from a in dbContext.activofijo

                                                     join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join
                                                     from b in b_join.DefaultIfEmpty()

                                                     join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                     from c in c_join.DefaultIfEmpty()

                                                     join d in dbContext.datahierarchy on new { cc = a.i_IdCentroCosto.Value, eliminado = 0, grupo = 31 } equals new { cc = d.i_ItemId, eliminado = d.i_IsDeleted.Value, grupo = d.i_GroupId } into d_join
                                                     from d in d_join.DefaultIfEmpty()
                                                     join e in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, tipoactivo = a.i_IdTipoActivo.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, tipoactivo = e.i_ItemId } into e_join
                                                     from e in e_join.DefaultIfEmpty()

                                                     join f in dbContext.datahierarchy on new { Grupo = 107, eliminado = 0, tipoactivo = a.i_IdTipoAdquisicion.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, tipoactivo = f.i_ItemId } into f_join
                                                     from f in f_join.DefaultIfEmpty()

                                                     join g in dbContext.cliente on new { resp = a.v_IdResponsable, eliminado = 0 } equals new { resp = g.v_IdCliente, eliminado = g.i_Eliminado.Value } into g_join
                                                     from g in g_join.DefaultIfEmpty()

                                                     join h in dbContext.datahierarchy on new { Grupo = 103, eliminado = 0, ubicacion = a.i_IdUbicacion.Value } equals new { Grupo = h.i_GroupId, eliminado = h.i_IsDeleted.Value, ubicacion = h.i_ItemId } into h_join
                                                     from h in h_join.DefaultIfEmpty()

                                                     where a.i_Eliminado == 0 && a.i_Depreciara == 1 && a.t_FechaUso != null && a.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoActual
                                                     && a.t_FechaOrdenCompra.Value.Year <= PeriodoReporte && a.i_EsTemporal == 0  //Se agrego porque en el proceso depreciacion no se toma en cuenta

                                                     select new activofijoDto
                                                     {
                                                         v_IdActivoFijo = a.v_IdActivoFijo,
                                                         d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                         mesesAdepreciar = b.v_Value1,
                                                         t_FechaUso = a.t_FechaUso,
                                                         i_Baja = a.i_Baja == null ? 0 : a.i_Baja,
                                                         t_FechaBaja = a.t_FechaBaja,
                                                         v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                         v_Descricpion = a.v_Descricpion.Trim(),
                                                         v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                         t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                         CodigoActivoFijo = a.v_CodigoActivoFijo,
                                                         Cuenta39 = c == null ? "0" : c.v_Cuenta39,
                                                         Cuenta33 = c == null ? "0" : c.v_Cuenta33,
                                                         CuentaGasto = c == null ? "0" : c.v_CuentaGasto,
                                                         i_MesesDepreciadosPAnterior = a.i_MesesDepreciadosPAnterior,
                                                         i_IdTipoMotivo = a.i_IdTipoMotivo.Value,
                                                         v_PeriodoAnterior = string.IsNullOrEmpty(a.v_PeriodoAnterior) ? "0" : a.v_PeriodoAnterior.Trim(),
                                                         d_ValorAdquisicionMe = a.d_ValorAdquisicionMe == null ? 0 : a.d_ValorAdquisicionMe,
                                                         CentroCosto = d.v_Value2,
                                                         i_IdCentroCosto = a.i_IdCentroCosto,
                                                         CodTipoActivo = e.v_Value2,
                                                         v_Modelo = a.v_Modelo,
                                                         v_Marca = a.v_Marca,
                                                         v_Serie = a.v_Serie,
                                                         v_Placa = a.v_Placa,
                                                         TipoAdquisicion = f.v_Value2,
                                                         i_Depreciara = a.i_Depreciara ?? 0,
                                                         PorcentajeDepreciacion = b != null ? b.v_Value2 : "",
                                                         b_Foto = a.b_Foto,
                                                         Responsable = (g.v_ApePaterno + " " + g.v_ApeMaterno + " " + g.v_PrimerNombre + " " + g.v_RazonSocial).Trim(),
                                                         Ubicacion = h.v_Value2,
                                                         MesesPorcentajeDepreciacion = b != null ? b.v_Value1 + " Meses | " + b.v_Value2 + " %" : "",

                                                     }).ToList().AsQueryable();

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ActivosPeriodoActualTodos = ActivosPeriodoActualTodos.Where(Filtro);

                    }

                    var ActivosPeriodoAnteriorTodos = (from a in dbContext.activofijo
                                                       join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join

                                                       from b in b_join.DefaultIfEmpty()
                                                       join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                       from c in c_join.DefaultIfEmpty()

                                                       join d in dbContext.datahierarchy on new { cc = a.i_IdCentroCosto.Value, eliminado = 0, grupo = 31 } equals new { cc = d.i_ItemId, eliminado = d.i_IsDeleted.Value, grupo = d.i_GroupId } into d_join
                                                       from d in d_join.DefaultIfEmpty()
                                                       join e in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, tipoactivo = a.i_IdTipoActivo.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, tipoactivo = e.i_ItemId } into e_join
                                                       from e in e_join.DefaultIfEmpty()

                                                       join f in dbContext.datahierarchy on new { Grupo = 107, eliminado = 0, tipoactivo = a.i_IdTipoAdquisicion.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, tipoactivo = f.i_ItemId } into f_join
                                                       from f in f_join.DefaultIfEmpty()

                                                       join g in dbContext.cliente on new { resp = a.v_IdResponsable, eliminado = 0 } equals new { resp = g.v_IdCliente, eliminado = g.i_Eliminado.Value } into g_join
                                                       from g in g_join.DefaultIfEmpty()

                                                       join h in dbContext.datahierarchy on new { Grupo = 103, eliminado = 0, ubicacion = a.i_IdUbicacion.Value } equals new { Grupo = h.i_GroupId, eliminado = h.i_IsDeleted.Value, ubicacion = h.i_ItemId } into h_join
                                                       from h in h_join.DefaultIfEmpty()


                                                       where a.i_Eliminado == 0 && a.i_Depreciara == 1 && a.t_FechaUso != null && a.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior
                                                        && a.i_EsTemporal == 0 // && a.v_Periodo == strPeriodo
                                                       select new activofijoDto
                                                       {

                                                           v_IdActivoFijo = a.v_IdActivoFijo,
                                                           d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                           mesesAdepreciar = b.v_Value1,
                                                           t_FechaUso = a.t_FechaUso,
                                                           i_Baja = a.i_Baja == null ? 0 : a.i_Baja,
                                                           t_FechaBaja = a.t_FechaBaja,
                                                           v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                           v_Descricpion = a.v_Descricpion.Trim(),
                                                           v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                           t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                           Cuenta39 = c == null ? "0" : c.v_Cuenta39,
                                                           CodigoActivoFijo = a.v_CodigoActivoFijo,
                                                           Cuenta33 = c == null ? "0" : c.v_Cuenta33,
                                                           i_MesesDepreciadosPAnterior = a.i_MesesDepreciadosPAnterior,
                                                           i_IdTipoMotivo = a.i_IdTipoMotivo.Value,
                                                           v_PeriodoAnterior = string.IsNullOrEmpty(a.v_PeriodoAnterior) ? "0" : a.v_PeriodoAnterior.Trim(),
                                                           d_ValorAdquisicionMe = a.d_ValorAdquisicionMe == null ? 0 : a.d_ValorAdquisicionMe,
                                                           CuentaGasto = c == null ? "0" : c.v_CuentaGasto,
                                                           CentroCosto = d.v_Value2,
                                                           i_IdCentroCosto = a.i_IdCentroCosto,
                                                           CodTipoActivo = e.v_Value2,
                                                           v_Modelo = a.v_Modelo,
                                                           v_Marca = a.v_Marca,
                                                           v_Serie = a.v_Serie,
                                                           v_Placa = a.v_Placa,
                                                           TipoAdquisicion = f.v_Value2,
                                                           i_Depreciara = a.i_Depreciara ?? 0,
                                                           PorcentajeDepreciacion = b != null ? b.v_Value2 : "",
                                                           b_Foto = a.b_Foto,
                                                           Responsable = (g.v_ApePaterno + " " + g.v_ApeMaterno + " " + g.v_PrimerNombre + " " + g.v_RazonSocial).Trim(),
                                                           Ubicacion = h.v_Value2,
                                                           MesesPorcentajeDepreciacion = b != null ? b.v_Value1 + " Meses | " + b.v_Value2 + " %" : "",
                                                       }).ToList().AsQueryable();


                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ActivosPeriodoAnteriorTodos = ActivosPeriodoAnteriorTodos.Where(Filtro);

                    }


                    var ActivosNoDepreciaran = (from a in dbContext.activofijo

                                                join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join
                                                from b in b_join.DefaultIfEmpty()

                                                join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                from c in c_join.DefaultIfEmpty()

                                                join d in dbContext.datahierarchy on new { cc = a.i_IdCentroCosto.Value, eliminado = 0, grupo = 31 } equals new { cc = d.i_ItemId, eliminado = d.i_IsDeleted.Value, grupo = d.i_GroupId } into d_join
                                                from d in d_join.DefaultIfEmpty()
                                                join e in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, tipoactivo = a.i_IdTipoActivo.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, tipoactivo = e.i_ItemId } into e_join
                                                from e in e_join.DefaultIfEmpty()

                                                join f in dbContext.datahierarchy on new { Grupo = 107, eliminado = 0, tipoactivo = a.i_IdTipoAdquisicion.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, tipoactivo = f.i_ItemId } into f_join
                                                from f in f_join.DefaultIfEmpty()

                                                join g in dbContext.cliente on new { resp = a.v_IdResponsable, eliminado = 0 } equals new { resp = g.v_IdCliente, eliminado = g.i_Eliminado.Value } into g_join
                                                from g in g_join.DefaultIfEmpty()

                                                join h in dbContext.datahierarchy on new { Grupo = 103, eliminado = 0, ubicacion = a.i_IdUbicacion.Value } equals new { Grupo = h.i_GroupId, eliminado = h.i_IsDeleted.Value, ubicacion = h.i_ItemId } into h_join
                                                from h in h_join.DefaultIfEmpty()

                                                where a.i_Eliminado == 0 && a.i_Depreciara == 0
                                                select new activofijoDto
                                                {
                                                    v_IdActivoFijo = a.v_IdActivoFijo,
                                                    d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                    mesesAdepreciar = b.v_Value1,
                                                    t_FechaUso = a.t_FechaUso,
                                                    i_Baja = a.i_Baja == null ? 0 : a.i_Baja,
                                                    t_FechaBaja = a.t_FechaBaja,
                                                    v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                    v_Descricpion = a.v_Descricpion.Trim(),
                                                    v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                    t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                    CodigoActivoFijo = a.v_CodigoActivoFijo,
                                                    Cuenta39 = c == null ? "0" : c.v_Cuenta39,
                                                    Cuenta33 = c == null ? "0" : c.v_Cuenta33,
                                                    CuentaGasto = c == null ? "0" : c.v_CuentaGasto,
                                                    i_MesesDepreciadosPAnterior = a.i_MesesDepreciadosPAnterior,
                                                    i_IdTipoMotivo = a.i_IdTipoMotivo.Value,
                                                    v_PeriodoAnterior = string.IsNullOrEmpty(a.v_PeriodoAnterior) ? "0" : a.v_PeriodoAnterior.Trim(),
                                                    d_ValorAdquisicionMe = a.d_ValorAdquisicionMe == null ? 0 : a.d_ValorAdquisicionMe,
                                                    CentroCosto = d.v_Value2,
                                                    i_IdCentroCosto = a.i_IdCentroCosto,
                                                    CodTipoActivo = e.v_Value2,
                                                    v_Modelo = a.v_Modelo,
                                                    v_Marca = a.v_Marca,
                                                    v_Serie = a.v_Serie,
                                                    v_Placa = a.v_Placa,
                                                    TipoAdquisicion = f.v_Value2,

                                                    i_Depreciara = a.i_Depreciara ?? 0,
                                                    PorcentajeDepreciacion = b != null ? b.v_Value2 : "",
                                                    b_Foto = a.b_Foto,
                                                    Responsable = (g.v_ApePaterno + " " + g.v_ApeMaterno + " " + g.v_PrimerNombre + " " + g.v_RazonSocial).Trim(),
                                                    Ubicacion = h.v_Value2,
                                                    MesesPorcentajeDepreciacion = b != null ? b.v_Value1 + " Meses | " + b.v_Value2 + " %" : "",
                                                }).ToList().AsQueryable();
                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ActivosNoDepreciaran = ActivosNoDepreciaran.Where(Filtro);

                    }

                    if (CodigoDesde != 0 && CodigoHasta != 0)
                    {
                        var ActivosPeriodoAnterior = (from a in ActivosPeriodoAnteriorTodos

                                                      where int.Parse(a.v_CodigoActivoFijo) >= CodigoDesde && int.Parse(a.v_CodigoActivoFijo) <= CodigoHasta

                                                      select a);



                        var ActivosPeriodoActual = (from a in ActivosPeriodoActualTodos

                                                    where int.Parse(a.v_CodigoActivoFijo) >= CodigoDesde && int.Parse(a.v_CodigoActivoFijo) <= CodigoHasta

                                                    select a);


                        var ActivosQueNoSeDepreciaran = (from a in ActivosNoDepreciaran
                                                         where int.Parse(a.v_CodigoActivoFijo) >= CodigoDesde && int.Parse(a.v_CodigoActivoFijo) <= CodigoHasta

                                                         select a);

                        ActivosTotal = ActivosPeriodoAnterior.Concat(ActivosPeriodoActual).Concat(ActivosQueNoSeDepreciaran).ToList().OrderBy(o => o.CodigoActivoFijo).ToList();
                    }
                    else
                    {
                        ActivosTotal = ActivosPeriodoActualTodos.Concat(ActivosPeriodoAnteriorTodos).Concat(ActivosNoDepreciaran).ToList().OrderBy(o => o.CodigoActivoFijo).ToList();
                    }
                    foreach (var item in ActivosTotal.AsParallel())
                    {
                        if (item.CodigoActivoFijo == "10000102")
                        {
                            string g = "";
                        }
                        //AjusteDepreciacionActivo = 0;
                        decimal AcumuladoHistorico = 0;
                        decimal AcumuladoTotal = 0;
                        decimal Ajuste = 0;
                        decimal ValorNeto = 0;
                        //decimal ImporteMensualHistorico =0;
                        decimal AcumuladoComparacion = 0;
                        decimal Comparacion = 0;
                        decimal AcumuladoAnterior = 0;
                        DateTime past = item.t_FechaUso.Value.Date; // Fecha Uso
                        decimal valoradquisicion = item.d_MonedaNacional.Value;
                        int mesesAdepreciar = string.IsNullOrEmpty(item.mesesAdepreciar) ? 0 : int.Parse(item.mesesAdepreciar);
                        int anioInicio = int.Parse(future.Date.Year.ToString()); // future .... Ultimo dia de la fecha del reporte 31/08/2015
                        int MesesDepreciados = 0;
                        int anioFin = int.Parse(future.Date.Year.ToString());
                        while (anioInicio <= anioFin)
                        {
                            int MesDepreciar = item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior && PeriodoReporte == int.Parse(item.v_PeriodoAnterior) ? 12 : 1;
                            for (int i = MesDepreciar; i <= 12; i++)
                            {
                                if (i <= int.Parse(future.Date.Month.ToString()) || anioInicio < anioFin)
                                {


                                    objReporte = new ReporteLibroActivoDto();
                                    objReporte.Responsable = item.Responsable;
                                    objReporte.MesesPorcentajeDepreciacion = item.MesesPorcentajeDepreciacion;
                                    objReporte.Ubicacion = item.Ubicacion;
                                    objReporte.Foto = item.b_Foto == null ? null : item.b_Foto;
                                    objReporte.NombreMes = new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(i).ToUpper();
                                    objReporte.Cuenta33 = item.Cuenta33;
                                    objReporte.Cuenta39 = item.Cuenta39;
                                    objReporte.v_IdActivoFijo = item.v_IdActivoFijo;
                                    objReporte.i_Baja = item.i_Baja.Value;
                                    objReporte.AnioBaja = item.t_FechaBaja == null ? "" : item.t_FechaBaja.Value.Year.ToString();
                                    objReporte.FechaUso = item.t_FechaUso.Value;
                                    objReporte.sFechaUso = item.i_Depreciara == 1 ? objReporte.FechaUso.ToShortDateString() : "";
                                    objReporte.FechaCompra = item.t_FechaOrdenCompra.Value;
                                    objReporte.CuentaGasto = item.CuentaGasto;
                                    objReporte.CentroCosto = item.CentroCosto;
                                    objReporte.CodigoActivoFijo = item.CodigoActivoFijo;
                                    objReporte.FechaBaja = item.t_FechaBaja == null ? DateTime.Now : item.t_FechaBaja.Value;
                                    objReporte.i_IdCentroCosto = item.i_IdCentroCosto.Value;
                                    objReporte.TipoActivo = item.CodTipoActivo;
                                    objReporte.v_NumeroFactura = item.v_NumeroFactura;
                                    objReporte.i_Depreciara = item.i_Depreciara.Value;
                                    objReporte.v_Serie = item.v_Serie;
                                    objReporte.v_Marca = item.v_Marca;
                                    objReporte.v_Modelo = item.v_Modelo;
                                    objReporte.v_Placa = item.v_Placa;
                                    objReporte.TipoAdquisicion = item.TipoAdquisicion;
                                    objReporte.AnioCompra = item.t_FechaOrdenCompra == null ? "" : item.t_FechaOrdenCompra.Value.Year.ToString();
                                    objReporte.MesCompra = item.t_FechaOrdenCompra == null ? "" : item.t_FechaOrdenCompra.Value.Month.ToString();
                                    objReporte.AnioUso = item.t_FechaUso == null ? "" : item.t_FechaUso.Value.Year.ToString();
                                    objReporte.MesesDepreciar = string.IsNullOrEmpty(item.mesesAdepreciar) ? "0" : item.mesesAdepreciar;
                                    string x = ("CÓDIGO : " + item.v_CodigoActivoFijo + "  ,  " + "DESCRIPCIÓN : " + item.v_Descricpion + "  ,  " + "FACTURA : " + item.v_NumeroFactura + "  ,  " + "FECHA ADQUISICIÓN : " + item.t_FechaOrdenCompra.Value.Date.Day.ToString("00") + "/" + item.t_FechaOrdenCompra.Value.Month.ToString("00") + "/"
                                        + item.t_FechaOrdenCompra.Value.Year.ToString() + "  ,  " + "FECHA USO : ");
                                    string y = (item.t_FechaUso == null ? "" : item.t_FechaUso.Value.Date.Day.ToString("00") + "/" + item.t_FechaUso.Value.Date.Month.ToString("00") + "/" + item.t_FechaUso.Value.Date.Year.ToString() + "  ,  " + "\n" + "MESES A DEPRECIAR : " + mesesAdepreciar.ToString());
                                    string baja = item.t_FechaBaja == null ? " " : " FECHA BAJA : " + item.t_FechaBaja.Value.Date.Day.ToString("00") + "/" + item.t_FechaBaja.Value.Month.ToString("00") + "/" + item.t_FechaBaja.Value.Date.Year.ToString();
                                    objReporte.DescripcionActivo = (x + y + baja).ToUpper();
                                    objReporte.SoloDescripionActivo = item.v_Descricpion.Trim();
                                    objReporte.ValorAdquisicionHistorico = item.d_MonedaNacional.Value;
                                    objReporte.ValorAdquisicionHistoricoFinal = item.d_MonedaNacional.Value;
                                    objReporte.PorcentajeDepreciacion = item.PorcentajeDepreciacion;
                                    decimal ImporteMensualHistorico = mesesAdepreciar == 0 ? 0 : Math.Round(valoradquisicion / mesesAdepreciar, 2);

                                    if (item.i_Depreciara == 0)
                                    {
                                        MesesDepreciados = 0;
                                        objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                        objReporte.d_AcumuladoHistorico = 0;
                                        objReporte.d_ValorNetoActual = valoradquisicion;
                                        objReporte.d_Comparacion = 0;
                                        objReporte.d_AjusteDepreciacion = 0;
                                        objReporte.d_ImporteMensualDepreciacion = 0;
                                        objReporte.d_Importe1SoloMes = objReporte.d_ImporteMensualDepreciacion.Value;


                                    }
                                    else
                                    {

                                        #region ActivoSinBaja
                                        if (item.i_Baja == 0)
                                        {

                                            if (past.Date.Month == 12)
                                            {
                                                FechaInicio = DateTime.Parse("01/01/" + (past.Date.Year + 1).ToString());
                                            }
                                            else
                                            {
                                                FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
                                            }
                                            DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());
                                            List<ReporteLibroActivoDto> AcumPeriodoAnterior = new List<ReporteLibroActivoDto>();

                                            switch (item.i_IdTipoMotivo)
                                            {

                                                case (int)MotivoActivoFijo.PeriodoAnterior:

                                                    if (PeriodoReporte == int.Parse(item.v_PeriodoAnterior) + 1)
                                                    {


                                                        MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 1 : MesesDepreciados + 1;
                                                        var MesesFaltantes = mesesAdepreciar - MesesDepreciados;
                                                        objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                                        objReporte.d_ImporteMensualDepreciacion = ImporteMensualHistorico;


                                                        if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                                        {
                                                            objReporte.AcumuladoAnterior = item.d_ValorAdquisicionMe;
                                                            objReporte.HistoricoAnterior = item.d_ValorAdquisicionMe;
                                                            objReporte.ValorNetoAnterior = item.d_ValorAdquisicionMe;
                                                            Ajuste = Ajuste > 0 ? Ajuste : AcumPeriodoAnterior.Any() ? AcumPeriodoAnterior.LastOrDefault().AjusteAnterior ?? 0 : 0;
                                                            objReporte.d_ImporteMensualDepreciacion = ImporteMensualHistorico;
                                                            objReporte.d_AcumuladoHistorico = ImporteMensualHistorico + AcumuladoHistorico;
                                                            objReporte.AcumuladoComparacion = i == MesDepreciar ? (objReporte.ValorAdquisicionHistorico - objReporte.AcumuladoAnterior - ImporteMensualHistorico) + Ajuste : AcumuladoComparacion - ImporteMensualHistorico;
                                                            objReporte.d_Comparacion = ImporteMensualHistorico * MesesFaltantes;
                                                            objReporte.d_AjusteDepreciacion = (objReporte.AcumuladoComparacion - objReporte.d_Comparacion);
                                                            Ajuste = objReporte.d_AjusteDepreciacion ?? 0;
                                                            objReporte.d_AcumuladoTotal = i == MesDepreciar ? objReporte.AcumuladoAnterior + ImporteMensualHistorico + Ajuste : AcumuladoTotal + ImporteMensualHistorico;
                                                            objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoTotal;
                                                        }
                                                        else
                                                        {
                                                            objReporte.MesesDepreciados = int.Parse(objReporte.MesesDepreciar.ToString());
                                                            objReporte.d_ImporteMensualDepreciacion = 0;
                                                            objReporte.AcumuladoAnterior = AcumuladoAnterior;
                                                            objReporte.HistoricoAnterior = null;
                                                            objReporte.ValorNetoAnterior = null;
                                                            objReporte.d_AcumuladoHistorico = item.d_MonedaNacional;
                                                            objReporte.AcumuladoComparacion = null;
                                                            objReporte.d_Comparacion = Comparacion;
                                                            objReporte.d_AjusteDepreciacion = 0;
                                                            //objReporte.d_AcumuladoTotal = AcumuladoTotal;
                                                            objReporte.d_AcumuladoTotal = item.d_MonedaNacional.Value;
                                                            objReporte.d_ValorNetoActual = ValorNeto;


                                                        }


                                                    }
                                                    else if (PeriodoReporte < int.Parse(item.v_PeriodoAnterior) + 1)
                                                    {

                                                        MesesDepreciados = 0;
                                                    }
                                                    else
                                                    {
                                                        var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()) + 1));
                                                        MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + (12 * (DiferenciaAnios)) + 1 : MesesDepreciados + 1;
                                                        var MesesFaltantes = mesesAdepreciar - MesesDepreciados;
                                                        AcumPeriodoAnterior = ActivosCierreAnterior.Where(o => o.v_IdActivoFijo == objReporte.v_IdActivoFijo).ToList();
                                                        objReporte.MesesDepreciados = MesesDepreciados;
                                                        objReporte.d_ImporteMensualDepreciacion = ImporteMensualHistorico;
                                                        if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                                        {

                                                            objReporte.AcumuladoAnterior = AcumPeriodoAnterior.Any() ? AcumPeriodoAnterior.LastOrDefault().d_AcumuladoTotal : 0;
                                                            objReporte.HistoricoAnterior = null;
                                                            objReporte.ValorNetoAnterior = null;
                                                            objReporte.d_AcumuladoHistorico = ImporteMensualHistorico + AcumuladoHistorico;
                                                            objReporte.AcumuladoComparacion = i == MesDepreciar ? (objReporte.ValorAdquisicionHistorico - objReporte.AcumuladoAnterior - ImporteMensualHistorico) + Ajuste : AcumuladoComparacion - ImporteMensualHistorico;
                                                            objReporte.d_Comparacion = ImporteMensualHistorico * MesesFaltantes;
                                                            objReporte.d_AjusteDepreciacion = (objReporte.AcumuladoComparacion - objReporte.d_Comparacion);
                                                            objReporte.d_AcumuladoTotal = i == MesDepreciar ? objReporte.AcumuladoAnterior + ImporteMensualHistorico : AcumuladoTotal + ImporteMensualHistorico;
                                                            objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoTotal;

                                                        }
                                                        else
                                                        {

                                                            objReporte.d_ImporteMensualDepreciacion = 0;
                                                            objReporte.d_AcumuladoHistorico = AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : AcumuladoHistorico;
                                                            //objReporte.d_AcumuladoTotal = objReporte.d_AcumuladoHistorico;
                                                            objReporte.d_AcumuladoTotal = item.d_MonedaNacional.Value;
                                                            objReporte.d_ValorNetoActual = 0;
                                                            objReporte.d_Comparacion = 0;
                                                            objReporte.d_AjusteDepreciacion = 0;
                                                            objReporte.AcumuladoAnterior = AcumPeriodoAnterior.Any() ? AcumPeriodoAnterior.LastOrDefault().d_AcumuladoTotal : objReporte.AcumuladoAnterior;
                                                        }





                                                    }

                                                    AcumuladoTotal = objReporte.d_AcumuladoTotal ?? 0;

                                                    AcumuladoComparacion = objReporte.AcumuladoComparacion ?? 0;
                                                    AcumuladoHistorico = objReporte.d_AcumuladoHistorico ?? 0;
                                                    ValorNeto = objReporte.d_ValorNetoActual ?? 0;
                                                    AcumuladoAnterior = objReporte.AcumuladoAnterior ?? 0;
                                                    Comparacion = objReporte.d_Comparacion ?? 0;
                                                    Ajuste = objReporte.d_AjusteDepreciacion > Ajuste ? objReporte.d_AjusteDepreciacion.Value : Ajuste;
                                                    objReporte.d_AjusteDepreciacion = i == MesDepreciar ? objReporte.d_AjusteDepreciacion : 0;
                                                    objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados > int.Parse(item.mesesAdepreciar) ? int.Parse(item.mesesAdepreciar) : MesesDepreciados;



                                                    break;

                                                case (int)MotivoActivoFijo.PeriodoActual:
                                                    MesesDepreciados = int.Parse((Math.Round((FechaFin.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                                                    objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                                    AcumPeriodoAnterior = ActivosCierreAnterior.Where(o => o.v_IdActivoFijo == objReporte.v_IdActivoFijo).ToList();
                                                    objReporte.AcumuladoAnterior = AcumPeriodoAnterior.Any() ? AcumPeriodoAnterior.LastOrDefault().d_AcumuladoTotal : 0;
                                                    objReporte.HistoricoAnterior = AcumPeriodoAnterior.Any() ? AcumPeriodoAnterior.LastOrDefault().d_AcumuladoHistorico : 0;
                                                    objReporte.ValorNetoAnterior = AcumPeriodoAnterior.Any() ? AcumPeriodoAnterior.LastOrDefault().d_ValorNetoActual : 0;
                                                    Ajuste = Ajuste > 0 ? Ajuste : AcumPeriodoAnterior.Any() ? AcumPeriodoAnterior.LastOrDefault().d_AjusteInicial ?? 0 : 0;
                                                    objReporte.d_ImporteMensualDepreciacion = ImporteMensualHistorico;
                                                    if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                                    {


                                                        if (objReporte.MesesDepreciados == 0)
                                                        {
                                                            objReporte.d_AcumuladoHistorico = 0;
                                                            objReporte.d_ImporteMensualDepreciacion = 0;
                                                            objReporte.d_ValorNetoActual = objReporte.ValorAdquisicionHistorico;
                                                            objReporte.d_Comparacion = 0;
                                                            objReporte.d_AjusteDepreciacion = 0;
                                                            objReporte.d_AcumuladoTotal = 0;

                                                        }
                                                        else
                                                        {
                                                            var MesesFaltantes = mesesAdepreciar - MesesDepreciados;

                                                            if (AcumPeriodoAnterior.Any())
                                                            {

                                                                objReporte.d_AcumuladoHistorico = ImporteMensualHistorico + AcumuladoHistorico;
                                                                objReporte.AcumuladoComparacion = i == MesDepreciar ? objReporte.ValorAdquisicionHistorico - (MesesDepreciados * ImporteMensualHistorico) - Ajuste : objReporte.ValorAdquisicionHistorico - (MesesDepreciados * ImporteMensualHistorico) - Ajuste;
                                                                objReporte.d_Comparacion = ImporteMensualHistorico * MesesFaltantes;
                                                                objReporte.d_AjusteDepreciacion = (objReporte.AcumuladoComparacion - objReporte.d_Comparacion);
                                                                objReporte.d_AcumuladoTotal = i == MesDepreciar ? objReporte.AcumuladoAnterior + ImporteMensualHistorico + objReporte.d_AjusteDepreciacion : AcumuladoTotal + ImporteMensualHistorico;

                                                            }
                                                            else
                                                            {


                                                                objReporte.d_AcumuladoHistorico = ImporteMensualHistorico + AcumuladoHistorico;
                                                                objReporte.AcumuladoComparacion = i == MesDepreciar ? objReporte.ValorAdquisicionHistorico - (MesesDepreciados * ImporteMensualHistorico) - Ajuste : objReporte.ValorAdquisicionHistorico - (MesesDepreciados * ImporteMensualHistorico) - Ajuste;
                                                                objReporte.d_Comparacion = ImporteMensualHistorico * MesesFaltantes;
                                                                objReporte.d_AjusteDepreciacion = (objReporte.AcumuladoComparacion - objReporte.d_Comparacion);
                                                                Ajuste = Ajuste > 0 ? Ajuste : objReporte.d_AjusteDepreciacion ?? 0;
                                                                objReporte.d_AjusteDepreciacion = objReporte.MesesDepreciados != 1 ? objReporte.d_AjusteDepreciacion < 0 ? 0 : objReporte.d_AjusteDepreciacion : objReporte.d_AjusteDepreciacion;

                                                                if (i == MesDepreciar)
                                                                {
                                                                    objReporte.d_AcumuladoTotal = (objReporte.MesesDepreciados * ImporteMensualHistorico) + objReporte.d_AjusteDepreciacion;
                                                                }
                                                                else if (objReporte.MesesDepreciados == 1)
                                                                {
                                                                    objReporte.d_AcumuladoTotal = (objReporte.MesesDepreciados * ImporteMensualHistorico) + Ajuste;
                                                                }
                                                                else
                                                                {
                                                                    objReporte.d_AcumuladoTotal = AcumuladoTotal + ImporteMensualHistorico;
                                                                }

                                                            }
                                                        }


                                                        objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoTotal;


                                                    }
                                                    else
                                                    {

                                                        objReporte.d_ImporteMensualDepreciacion = 0;
                                                        objReporte.d_AcumuladoHistorico = AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : AcumuladoHistorico;
                                                        //objReporte.d_AcumuladoTotal = AcumuladoTotal;
                                                        objReporte.d_AcumuladoTotal = objReporte.d_AcumuladoHistorico;
                                                        objReporte.d_ValorNetoActual = 0;
                                                        objReporte.d_Comparacion = 0;
                                                        objReporte.d_AjusteDepreciacion = 0;
                                                        objReporte.AcumuladoAnterior = objReporte.AcumuladoAnterior == 0 ? AcumPeriodoAnterior.Any() ? AcumPeriodoAnterior.LastOrDefault().d_AcumuladoHistorico : 0 : objReporte.AcumuladoAnterior;

                                                    }

                                                    AcumuladoTotal = objReporte.d_AcumuladoTotal ?? 0;
                                                    AcumuladoHistorico = objReporte.d_AcumuladoHistorico ?? 0;
                                                    ValorNeto = objReporte.d_ValorNetoActual ?? 0;
                                                    Ajuste = objReporte.d_AjusteDepreciacion > Ajuste ? objReporte.d_AjusteDepreciacion.Value : Ajuste;
                                                    objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados > int.Parse(item.mesesAdepreciar) ? int.Parse(item.mesesAdepreciar) : MesesDepreciados;

                                                    break;

                                            }


                                        }
                                        #endregion
                                        else
                                        #region ActivoBaja
                                        {
                                            if (anioInicio < int.Parse(item.t_FechaBaja.Value.Year.ToString())) // No se toma en cuenta el mes de Baja
                                            {

                                                if (past.Date.Month == 12)
                                                {
                                                    FechaInicio = DateTime.Parse("01/01/" + (past.Date.Year + 1).ToString());
                                                }
                                                else
                                                {
                                                    FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
                                                }
                                                DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());

                                                if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                {
                                                    if (PeriodoReporte >= int.Parse(item.v_PeriodoAnterior) + 1)
                                                    {
                                                        MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 1 : MesesDepreciados + 1;
                                                    }
                                                    else if (PeriodoReporte < int.Parse(item.v_PeriodoAnterior) + 1)
                                                    {
                                                        //MesesDepreciados = item.i_MesesDepreciadosPAnterior.Value;
                                                        MesesDepreciados = 0;
                                                    }
                                                    else
                                                    {
                                                        MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 12 : MesesDepreciados + 1;
                                                    }

                                                }
                                                else
                                                {
                                                    MesesDepreciados = int.Parse((Math.Round((FechaFin.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                                                }
                                                objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;


                                                var AcumPeriodoAnterior = ActivosCierreAnterior.Where(o => o.v_IdActivoFijo == objReporte.v_IdActivoFijo);
                                                objReporte.AcumuladoAnterior = AcumPeriodoAnterior.Any() ? AcumPeriodoAnterior.LastOrDefault().d_AcumuladoHistorico.Value : 0;
                                                if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                                {


                                                    objReporte.d_ImporteMensualDepreciacion = objReporte.MesesDepreciados == 0 ? 0 : mesesAdepreciar == 0 ? 0 : ImporteMensualHistorico;
                                                    if (i == 1 && item.v_PeriodoAnterior.Trim() == (PeriodoReporte - 1).ToString() && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value + ImporteMensualHistorico;
                                                    }

                                                    else if (i == 12 && PeriodoReporte < (int.Parse(item.v_PeriodoAnterior) + 1) && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value;
                                                    }
                                                    else if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior && PeriodoReporte > int.Parse(item.v_PeriodoAnterior))
                                                    {
                                                        var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()))) - 1;
                                                        objReporte.AcumuladoAnterior = (objReporte.d_ImporteMensualDepreciacion.Value * 12 * DiferenciaAnios) + item.d_ValorAdquisicionMe;
                                                        objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior.Value + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;


                                                    }

                                                    else
                                                    {
                                                        objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion.Value;
                                                    }
                                                    AcumuladoHistorico = objReporte.d_AcumuladoHistorico.Value;
                                                    if (i != 1 && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoHistorico;
                                                    }
                                                    else
                                                    {

                                                        objReporte.d_ValorNetoActual = AjusteDepreciacionActivo != 0 ? (valoradquisicion - objReporte.d_AcumuladoHistorico) + AjusteDepreciacionActivo : (valoradquisicion - objReporte.d_AcumuladoHistorico);
                                                    }
                                                    objReporte.d_Comparacion = objReporte.MesesDepreciados == 0 && objReporte.d_AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : objReporte.d_ImporteMensualDepreciacion * (mesesAdepreciar - objReporte.MesesDepreciados);
                                                    objReporte.d_AjusteDepreciacion = objReporte.d_ValorNetoActual - objReporte.d_Comparacion;


                                                    if (objReporte.MesesDepreciados == mesesAdepreciar)
                                                    {
                                                        objReporte.d_AcumuladoHistorico = objReporte.d_AcumuladoHistorico + objReporte.d_ValorNetoActual;
                                                        objReporte.d_ValorNetoActual = objReporte.d_ValorNetoActual + objReporte.d_AjusteDepreciacion;
                                                        objReporte.d_AjusteDepreciacion = 0;
                                                    }

                                                }
                                                else
                                                {
                                                    objReporte.d_ImporteMensualDepreciacion = 0;
                                                    objReporte.d_AcumuladoHistorico = 0;
                                                    objReporte.d_ValorNetoActual = 0;
                                                    objReporte.d_Comparacion = 0;
                                                    objReporte.d_AjusteDepreciacion = 0;
                                                    objReporte.ValorAdquisicionHistoricoFinal = 0;

                                                }
                                                objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados > int.Parse(item.mesesAdepreciar) ? int.Parse(item.mesesAdepreciar) : MesesDepreciados;
                                            }
                                            else
                                            {

                                                if (i <= int.Parse(item.t_FechaBaja.Value.Month.ToString()) && anioInicio == item.t_FechaBaja.Value.Year)
                                                {

                                                    if (past.Date.Month == 12)
                                                    {
                                                        FechaInicio = DateTime.Parse("01/01/" + (past.Date.Year + 1).ToString());
                                                    }
                                                    else
                                                    {
                                                        FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
                                                    }
                                                    DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());


                                                    if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        if (PeriodoReporte >= int.Parse(item.v_PeriodoAnterior) + 1)
                                                        {
                                                            MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 1 : MesesDepreciados + 1;
                                                        }
                                                        else if (PeriodoReporte < int.Parse(item.v_PeriodoAnterior) + 1)
                                                        {
                                                            //MesesDepreciados = item.i_MesesDepreciadosPAnterior.Value;
                                                            MesesDepreciados = 0;
                                                        }
                                                        else
                                                        {
                                                            MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 12 : MesesDepreciados + 1;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        MesesDepreciados = int.Parse((Math.Round((FechaFin.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                                                    }

                                                    objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                                    var AcumPeriodoAnterior = ActivosCierreAnterior.Where(o => o.v_IdActivoFijo == objReporte.v_IdActivoFijo);
                                                    objReporte.AcumuladoAnterior = AcumPeriodoAnterior.Any() ? AcumPeriodoAnterior.LastOrDefault().d_AcumuladoHistorico.Value : 0;

                                                    if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                                    {
                                                        objReporte.d_ImporteMensualDepreciacion = objReporte.MesesDepreciados == 0 ? 0 : mesesAdepreciar == 0 ? 0 : ImporteMensualHistorico;
                                                        if (i == 1 && item.v_PeriodoAnterior.Trim() == (PeriodoReporte - 1).ToString() && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                        {
                                                            objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value + ImporteMensualHistorico;
                                                        }

                                                        else if (i == 12 && PeriodoReporte < (int.Parse(item.v_PeriodoAnterior) + 1) && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                        {
                                                            objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value;
                                                        }
                                                        else if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior && PeriodoReporte > int.Parse(item.v_PeriodoAnterior))
                                                        {
                                                            var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()))) - 1;
                                                            objReporte.AcumuladoAnterior = (objReporte.d_ImporteMensualDepreciacion.Value * 12 * DiferenciaAnios) + item.d_ValorAdquisicionMe;
                                                            objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior.Value + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;


                                                        }
                                                        else
                                                        {
                                                            objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion.Value;
                                                        }
                                                        AcumuladoHistorico = objReporte.d_AcumuladoHistorico.Value;
                                                        if (i != 1 && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                        {
                                                            objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoHistorico;
                                                        }
                                                        else
                                                        {

                                                            objReporte.d_ValorNetoActual = AjusteDepreciacionActivo != 0 ? (valoradquisicion - objReporte.d_AcumuladoHistorico) + AjusteDepreciacionActivo : (valoradquisicion - objReporte.d_AcumuladoHistorico);
                                                        }

                                                        objReporte.d_Comparacion = objReporte.MesesDepreciados == 0 && objReporte.d_AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : objReporte.d_ImporteMensualDepreciacion * (mesesAdepreciar - objReporte.MesesDepreciados);
                                                        objReporte.d_AjusteDepreciacion = objReporte.d_ValorNetoActual - objReporte.d_Comparacion;




                                                        if (objReporte.MesesDepreciados == mesesAdepreciar)
                                                        {
                                                            objReporte.d_AcumuladoHistorico = objReporte.d_AcumuladoHistorico + objReporte.d_ValorNetoActual;
                                                            objReporte.d_ValorNetoActual = objReporte.d_ValorNetoActual + objReporte.d_AjusteDepreciacion;
                                                            objReporte.d_AjusteDepreciacion = 0;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        objReporte.d_ImporteMensualDepreciacion = 0;
                                                        objReporte.d_AcumuladoHistorico = 0;
                                                        objReporte.d_ValorNetoActual = 0;
                                                        objReporte.d_Comparacion = 0;
                                                        objReporte.d_AjusteDepreciacion = 0;
                                                        objReporte.ValorAdquisicionHistoricoFinal = 0;

                                                    }
                                                    objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados > int.Parse(item.mesesAdepreciar) ? int.Parse(item.mesesAdepreciar) : MesesDepreciados;

                                                }
                                                else
                                                {
                                                    objReporte.MesesDepreciados = 0;
                                                    objReporte.d_ImporteMensualDepreciacion = 0;
                                                    objReporte.d_AcumuladoHistorico = 0;
                                                    objReporte.d_ValorNetoActual = 0;
                                                    objReporte.d_Comparacion = 0;
                                                    objReporte.d_AjusteDepreciacion = 0;
                                                    objReporte.ValorAdquisicionHistoricoFinal = 0;
                                                    MesesDepreciados = 0;
                                                }
                                            }

                                        }

                                        #endregion
                                    }

                                    if (objReporte.MesesDepreciados >= 0 && MesesDepreciados >= 0)
                                    {

                                        if (objReporte.CodigoActivoFijo == "10000102")
                                        {
                                            string h = "";
                                        }
                                        objReporte.d_Importe1SoloMes = objReporte.d_ImporteMensualDepreciacion ?? 0;
                                        ListaActivoFijoDepreciacion.Add(objReporte);
                                        AjusteDepreciacionActivo = objReporte.d_AjusteDepreciacion == 0 ? AjusteDepreciacionActivo : objReporte.d_AjusteDepreciacion ?? 0;
                                    }

                                }
                            }

                            anioInicio = anioInicio + 1;
                        }

                    }
                    objOperationResult.Success = 1;
                    return ListaActivoFijoDepreciacion;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ActivoFijoBL.ReporteLibroActivo()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return null;
            }
        }



        public List<ReporteLibroActivoDto> ReporteAcumuladoPeriodoAnterior(ref  OperationResult objOperationResult, int PeriodoReporte, int MesReporte, int CodigoDesde, int CodigoHasta, bool UsoOtrosReportes, string Filtro)
        {
            try
            {
                objOperationResult.Success = 1;
                decimal AjusteDepreciacionActivo = 0;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    DateTime future = new DateTime();
                    if (UsoOtrosReportes)
                    {
                        future = DateTime.Parse(DateTime.DaysInMonth(PeriodoReporte, MesReporte) + "/" + MesReporte + "/" + PeriodoReporte + " 23:59");
                    }
                    else
                    {
                        if (PeriodoReporte < DateTime.Now.Date.Year)
                        {

                            future = DateTime.Parse("31/12/" + PeriodoReporte + " 23:59");
                        }
                        else
                        {
                            future = DateTime.Parse(DateTime.DaysInMonth(PeriodoReporte, MesReporte) + "/" + MesReporte + "/" + PeriodoReporte + " 23:59"); //FechaReporte
                        }
                    }
                    List<ReporteLibroActivoDto> ListaActivoFijoDepreciacion = new List<ReporteLibroActivoDto>();
                    List<activofijoDto> ActivosTotal = new List<activofijoDto>();
                    ReporteLibroActivoDto objReporte = new ReporteLibroActivoDto();
                    DateTime FechaInicio = new DateTime();
                    var ActivosPeriodoActualTodos = (from a in dbContext.activofijo

                                                     join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join
                                                     from b in b_join.DefaultIfEmpty()

                                                     join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                     from c in c_join.DefaultIfEmpty()

                                                     join d in dbContext.datahierarchy on new { cc = a.i_IdCentroCosto.Value, eliminado = 0, grupo = 31 } equals new { cc = d.i_ItemId, eliminado = d.i_IsDeleted.Value, grupo = d.i_GroupId } into d_join
                                                     from d in d_join.DefaultIfEmpty()
                                                     join e in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, tipoactivo = a.i_IdTipoActivo.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, tipoactivo = e.i_ItemId } into e_join
                                                     from e in e_join.DefaultIfEmpty()

                                                     join f in dbContext.datahierarchy on new { Grupo = 107, eliminado = 0, tipoactivo = a.i_IdTipoAdquisicion.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, tipoactivo = f.i_ItemId } into f_join
                                                     from f in f_join.DefaultIfEmpty()

                                                     where a.i_Eliminado == 0 && a.i_Depreciara == 1 && a.t_FechaUso != null && a.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoActual
                                                     && a.t_FechaOrdenCompra.Value.Year <= PeriodoReporte && a.i_EsTemporal == 0  //Se agrego porque en el proceso depreciacion no se toma en cuenta

                                                     select new activofijoDto
                                                     {
                                                         v_IdActivoFijo = a.v_IdActivoFijo,
                                                         d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                         mesesAdepreciar = b.v_Value1,
                                                         t_FechaUso = a.t_FechaUso,
                                                         i_Baja = a.i_Baja == null ? 0 : a.i_Baja,
                                                         t_FechaBaja = a.t_FechaBaja,
                                                         v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                         v_Descricpion = a.v_Descricpion.Trim(),
                                                         v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                         t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                         CodigoActivoFijo = a.v_CodigoActivoFijo,
                                                         Cuenta39 = c == null ? "0" : c.v_Cuenta39,
                                                         Cuenta33 = c == null ? "0" : c.v_Cuenta33,
                                                         CuentaGasto = c == null ? "0" : c.v_CuentaGasto,
                                                         i_MesesDepreciadosPAnterior = a.i_MesesDepreciadosPAnterior,
                                                         i_IdTipoMotivo = a.i_IdTipoMotivo.Value,
                                                         v_PeriodoAnterior = string.IsNullOrEmpty(a.v_PeriodoAnterior) ? "0" : a.v_PeriodoAnterior.Trim(),
                                                         d_ValorAdquisicionMe = a.d_ValorAdquisicionMe == null ? 0 : a.d_ValorAdquisicionMe,
                                                         CentroCosto = d.v_Value2,
                                                         i_IdCentroCosto = a.i_IdCentroCosto,
                                                         CodTipoActivo = e.v_Value2,
                                                         v_Modelo = a.v_Modelo,
                                                         v_Marca = a.v_Marca,
                                                         v_Serie = a.v_Serie,
                                                         v_Placa = a.v_Placa,
                                                         TipoAdquisicion = f.v_Value2,
                                                         i_Depreciara = a.i_Depreciara ?? 0,



                                                     }).ToList().AsQueryable();

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ActivosPeriodoActualTodos = ActivosPeriodoActualTodos.Where(Filtro);

                    }

                    var ActivosPeriodoAnteriorTodos = (from a in dbContext.activofijo
                                                       join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join

                                                       from b in b_join.DefaultIfEmpty()
                                                       join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                       from c in c_join.DefaultIfEmpty()

                                                       join d in dbContext.datahierarchy on new { cc = a.i_IdCentroCosto.Value, eliminado = 0, grupo = 31 } equals new { cc = d.i_ItemId, eliminado = d.i_IsDeleted.Value, grupo = d.i_GroupId } into d_join
                                                       from d in d_join.DefaultIfEmpty()
                                                       join e in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, tipoactivo = a.i_IdTipoActivo.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, tipoactivo = e.i_ItemId } into e_join
                                                       from e in e_join.DefaultIfEmpty()

                                                       join f in dbContext.datahierarchy on new { Grupo = 107, eliminado = 0, tipoactivo = a.i_IdTipoAdquisicion.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, tipoactivo = f.i_ItemId } into f_join
                                                       from f in f_join.DefaultIfEmpty()


                                                       where a.i_Eliminado == 0 && a.i_Depreciara == 1 && a.t_FechaUso != null && a.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior
                                                        && a.i_EsTemporal == 0 // && a.v_Periodo == strPeriodo
                                                       select new activofijoDto
                                                       {

                                                           v_IdActivoFijo = a.v_IdActivoFijo,
                                                           d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                           mesesAdepreciar = b.v_Value1,
                                                           t_FechaUso = a.t_FechaUso,
                                                           i_Baja = a.i_Baja == null ? 0 : a.i_Baja,
                                                           t_FechaBaja = a.t_FechaBaja,
                                                           v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                           v_Descricpion = a.v_Descricpion.Trim(),
                                                           v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                           t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                           Cuenta39 = c == null ? "0" : c.v_Cuenta39,
                                                           CodigoActivoFijo = a.v_CodigoActivoFijo,
                                                           Cuenta33 = c == null ? "0" : c.v_Cuenta33,
                                                           i_MesesDepreciadosPAnterior = a.i_MesesDepreciadosPAnterior,
                                                           i_IdTipoMotivo = a.i_IdTipoMotivo.Value,
                                                           v_PeriodoAnterior = string.IsNullOrEmpty(a.v_PeriodoAnterior) ? "0" : a.v_PeriodoAnterior.Trim(),
                                                           d_ValorAdquisicionMe = a.d_ValorAdquisicionMe == null ? 0 : a.d_ValorAdquisicionMe,
                                                           CuentaGasto = c == null ? "0" : c.v_CuentaGasto,
                                                           CentroCosto = d.v_Value2,
                                                           i_IdCentroCosto = a.i_IdCentroCosto,
                                                           CodTipoActivo = e.v_Value2,
                                                           v_Modelo = a.v_Modelo,
                                                           v_Marca = a.v_Marca,
                                                           v_Serie = a.v_Serie,
                                                           v_Placa = a.v_Placa,
                                                           TipoAdquisicion = f.v_Value2,
                                                           i_Depreciara = a.i_Depreciara ?? 0,
                                                       }).ToList().AsQueryable();

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ActivosPeriodoAnteriorTodos = ActivosPeriodoAnteriorTodos.Where(Filtro);

                    }


                    var ActivosNoDepreciaran = (from a in dbContext.activofijo

                                                join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join
                                                from b in b_join.DefaultIfEmpty()

                                                join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                from c in c_join.DefaultIfEmpty()

                                                join d in dbContext.datahierarchy on new { cc = a.i_IdCentroCosto.Value, eliminado = 0, grupo = 31 } equals new { cc = d.i_ItemId, eliminado = d.i_IsDeleted.Value, grupo = d.i_GroupId } into d_join
                                                from d in d_join.DefaultIfEmpty()
                                                join e in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, tipoactivo = a.i_IdTipoActivo.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, tipoactivo = e.i_ItemId } into e_join
                                                from e in e_join.DefaultIfEmpty()

                                                join f in dbContext.datahierarchy on new { Grupo = 107, eliminado = 0, tipoactivo = a.i_IdTipoAdquisicion.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, tipoactivo = f.i_ItemId } into f_join
                                                from f in f_join.DefaultIfEmpty()

                                                where a.i_Eliminado == 0 && a.i_Depreciara == 0
                                                select new activofijoDto
                                                {
                                                    v_IdActivoFijo = a.v_IdActivoFijo,
                                                    d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                    mesesAdepreciar = b.v_Value1,
                                                    t_FechaUso = a.t_FechaUso,
                                                    i_Baja = a.i_Baja == null ? 0 : a.i_Baja,
                                                    t_FechaBaja = a.t_FechaBaja,
                                                    v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                    v_Descricpion = a.v_Descricpion.Trim(),
                                                    v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                    t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                    CodigoActivoFijo = a.v_CodigoActivoFijo,
                                                    Cuenta39 = c == null ? "0" : c.v_Cuenta39,
                                                    Cuenta33 = c == null ? "0" : c.v_Cuenta33,
                                                    CuentaGasto = c == null ? "0" : c.v_CuentaGasto,
                                                    i_MesesDepreciadosPAnterior = a.i_MesesDepreciadosPAnterior,
                                                    i_IdTipoMotivo = a.i_IdTipoMotivo.Value,
                                                    v_PeriodoAnterior = string.IsNullOrEmpty(a.v_PeriodoAnterior) ? "0" : a.v_PeriodoAnterior.Trim(),
                                                    d_ValorAdquisicionMe = a.d_ValorAdquisicionMe == null ? 0 : a.d_ValorAdquisicionMe,
                                                    CentroCosto = d.v_Value2,
                                                    i_IdCentroCosto = a.i_IdCentroCosto,
                                                    CodTipoActivo = e.v_Value2,
                                                    v_Modelo = a.v_Modelo,
                                                    v_Marca = a.v_Marca,
                                                    v_Serie = a.v_Serie,
                                                    v_Placa = a.v_Placa,
                                                    TipoAdquisicion = f.v_Value2,

                                                    i_Depreciara = a.i_Depreciara ?? 0,

                                                }).ToList().AsQueryable();
                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ActivosNoDepreciaran = ActivosNoDepreciaran.Where(Filtro);

                    }

                    if (CodigoDesde != 0 && CodigoHasta != 0)
                    {
                        var ActivosPeriodoAnterior = (from a in ActivosPeriodoAnteriorTodos

                                                      where int.Parse(a.v_CodigoActivoFijo) >= CodigoDesde && int.Parse(a.v_CodigoActivoFijo) <= CodigoHasta

                                                      select a);



                        var ActivosPeriodoActual = (from a in ActivosPeriodoActualTodos

                                                    where int.Parse(a.v_CodigoActivoFijo) >= CodigoDesde && int.Parse(a.v_CodigoActivoFijo) <= CodigoHasta

                                                    select a);


                        var ActivosQueNoSeDepreciaran = (from a in ActivosNoDepreciaran
                                                         where int.Parse(a.v_CodigoActivoFijo) >= CodigoDesde && int.Parse(a.v_CodigoActivoFijo) <= CodigoHasta

                                                         select a);

                        ActivosTotal = ActivosPeriodoAnterior.Concat(ActivosPeriodoActual).Concat(ActivosQueNoSeDepreciaran).ToList().OrderBy(o => o.CodigoActivoFijo).ToList();
                    }
                    else
                    {
                        ActivosTotal = ActivosPeriodoActualTodos.Concat(ActivosPeriodoAnteriorTodos).Concat(ActivosNoDepreciaran).ToList().OrderBy(o => o.CodigoActivoFijo).ToList();
                    }


                    foreach (var item in ActivosTotal.AsParallel())
                    {

                        decimal AcumuladoTotal = 0, Ajuste = 0, AjusteVirtual = 0;
                        DateTime past = item.t_FechaUso.Value.Date; // Fecha Uso
                        decimal valoradquisicion = item.d_MonedaNacional.Value;
                        decimal AcumuladoHistorico = 0;
                        decimal AcumuladoComparacion = 0, AcumuladoAnterior = 0, Comparacion = 0, ValorNeto = 0;
                        int mesesAdepreciar = string.IsNullOrEmpty(item.mesesAdepreciar) ? 0 : int.Parse(item.mesesAdepreciar);
                        int anioInicio = int.Parse(future.Date.Year.ToString()); // future .... Ultimo dia de la fecha del reporte 31/08/2015
                        int MesesDepreciados = 0;
                        int anioFin = int.Parse(future.Date.Year.ToString());
                        while (anioInicio <= anioFin)
                        {
                            int MesDepreciar = item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior && PeriodoReporte == int.Parse(item.v_PeriodoAnterior) ? 12 : 1;
                            for (int i = MesDepreciar; i <= 12; i++)
                            {
                                if (i <= int.Parse(future.Date.Month.ToString()) || anioInicio < anioFin)
                                {


                                    objReporte = new ReporteLibroActivoDto();
                                    objReporte.NombreMes = new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(i).ToUpper();
                                    objReporte.Cuenta33 = item.Cuenta33;
                                    objReporte.Cuenta39 = item.Cuenta39;
                                    objReporte.v_IdActivoFijo = item.v_IdActivoFijo;
                                    objReporte.i_Baja = item.i_Baja.Value;
                                    objReporte.AnioBaja = item.t_FechaBaja == null ? "" : item.t_FechaBaja.Value.Year.ToString();
                                    objReporte.FechaUso = item.t_FechaUso.Value;
                                    objReporte.sFechaUso = item.i_Depreciara == 1 ? objReporte.FechaUso.ToShortDateString() : "";
                                    objReporte.FechaCompra = item.t_FechaOrdenCompra.Value;
                                    objReporte.CuentaGasto = item.CuentaGasto;
                                    objReporte.CentroCosto = item.CentroCosto;
                                    objReporte.CodigoActivoFijo = item.CodigoActivoFijo;
                                    objReporte.FechaBaja = item.t_FechaBaja == null ? DateTime.Now : item.t_FechaBaja.Value;
                                    objReporte.i_IdCentroCosto = item.i_IdCentroCosto.Value;
                                    objReporte.TipoActivo = item.CodTipoActivo;
                                    objReporte.v_NumeroFactura = item.v_NumeroFactura;
                                    objReporte.i_Depreciara = item.i_Depreciara.Value;
                                    objReporte.v_Serie = item.v_Serie;
                                    objReporte.v_Marca = item.v_Marca;
                                    objReporte.v_Modelo = item.v_Modelo;
                                    objReporte.v_Placa = item.v_Placa;
                                    objReporte.TipoAdquisicion = item.TipoAdquisicion;
                                    objReporte.AnioCompra = item.t_FechaOrdenCompra == null ? "" : item.t_FechaOrdenCompra.Value.Year.ToString();
                                    objReporte.MesCompra = item.t_FechaOrdenCompra == null ? "" : item.t_FechaOrdenCompra.Value.Month.ToString();
                                    objReporte.AnioUso = item.t_FechaUso == null ? "" : item.t_FechaUso.Value.Year.ToString();
                                    objReporte.MesesDepreciar = string.IsNullOrEmpty(item.mesesAdepreciar) ? "0" : item.mesesAdepreciar;
                                    string x = ("CÓDIGO : " + item.v_CodigoActivoFijo + "  ,  " + "DESCRIPCIÓN : " + item.v_Descricpion + "  ,  " + "FACTURA : " + item.v_NumeroFactura + "  ,  " + "FECHA ADQUISICIÓN : " + item.t_FechaOrdenCompra.Value.Date.Day.ToString("00") + "/" + item.t_FechaOrdenCompra.Value.Month.ToString("00") + "/"
                                        + item.t_FechaOrdenCompra.Value.Year.ToString() + "  ,  " + "FECHA USO : ");
                                    string y = (item.t_FechaUso == null ? "" : item.t_FechaUso.Value.Date.Day.ToString("00") + "/" + item.t_FechaUso.Value.Date.Month.ToString("00") + "/" + item.t_FechaUso.Value.Date.Year.ToString() + "  ,  " + "\n" + "MESES A DEPRECIAR : " + mesesAdepreciar.ToString());
                                    string baja = item.t_FechaBaja == null ? " " : " FECHA BAJA : " + item.t_FechaBaja.Value.Date.Day.ToString("00") + "/" + item.t_FechaBaja.Value.Month.ToString("00") + "/" + item.t_FechaBaja.Value.Date.Year.ToString();
                                    objReporte.DescripcionActivo = (x + y + baja).ToUpper();
                                    objReporte.SoloDescripionActivo = item.v_Descricpion.Trim();
                                    objReporte.ValorAdquisicionHistorico = item.d_MonedaNacional.Value;
                                    decimal ImporteMensualHistorico = mesesAdepreciar == 0 ? 0 : Math.Round(valoradquisicion / mesesAdepreciar, 2);
                                    if (item.i_Depreciara == 0) ///122
                                    {
                                        MesesDepreciados = 0;
                                        objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                        objReporte.d_AcumuladoHistorico = 0;
                                        objReporte.d_ValorNetoActual = valoradquisicion;
                                        objReporte.d_Comparacion = 0;
                                        objReporte.d_AjusteDepreciacion = 0;
                                        objReporte.d_ImporteMensualDepreciacion = 0;
                                        objReporte.d_Importe1SoloMes = objReporte.d_ImporteMensualDepreciacion.Value;

                                    }
                                    else
                                    {

                                        #region ActivoSinBaja
                                        if (item.i_Baja == 0)
                                        {

                                            if (past.Date.Month == 12)
                                            {
                                                FechaInicio = DateTime.Parse("01/01/" + (past.Date.Year + 1).ToString());
                                            }
                                            else
                                            {
                                                FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
                                            }
                                            DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());

                                            if (FechaFin.Date > FechaInicio.Date)
                                            {

                                                switch (item.i_IdTipoMotivo)
                                                {

                                                    case (int)MotivoActivoFijo.PeriodoAnterior:
                                                        if (PeriodoReporte == int.Parse(item.v_PeriodoAnterior) + 1)
                                                        {
                                                            MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 1 : MesesDepreciados + 1;

                                                        }
                                                        else if (PeriodoReporte < int.Parse(item.v_PeriodoAnterior) + 1)
                                                        {
                                                            MesesDepreciados = 0;
                                                        }
                                                        else
                                                        {
                                                            var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()) + 1));
                                                            MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + (12 * (DiferenciaAnios)) + 1 : MesesDepreciados + 1;
                                                        }

                                                        objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                                        objReporte.d_ImporteMensualDepreciacion = ImporteMensualHistorico;

                                                        if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                                        {
                                                            if (PeriodoReporte >= int.Parse(item.v_PeriodoAnterior) + 1)
                                                            {
                                                                var MesesFaltantes = mesesAdepreciar - MesesDepreciados;
                                                                objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                                                objReporte.d_ImporteMensualDepreciacion = ImporteMensualHistorico;
                                                                objReporte.AcumuladoAnterior = item.d_ValorAdquisicionMe;
                                                                objReporte.HistoricoAnterior = item.d_ValorAdquisicionMe;
                                                                objReporte.ValorNetoAnterior = item.d_ValorAdquisicionMe;
                                                                objReporte.d_ImporteMensualDepreciacion = ImporteMensualHistorico;
                                                                objReporte.d_AcumuladoHistorico = i * ImporteMensualHistorico; //i == MesDepreciar ? objReporte.HistoricoAnterior + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;
                                                                objReporte.AcumuladoComparacion = i == MesDepreciar ? (objReporte.ValorAdquisicionHistorico - objReporte.AcumuladoAnterior - ImporteMensualHistorico) + Ajuste : objReporte.ValorAdquisicionHistorico - AcumuladoTotal - ImporteMensualHistorico;
                                                                objReporte.d_Comparacion = ImporteMensualHistorico * MesesFaltantes;

                                                                objReporte.d_AjusteDepreciacion = (objReporte.AcumuladoComparacion - objReporte.d_Comparacion);
                                                                //Ajuste = objReporte.d_AjusteDepreciacion > Ajuste ? objReporte.d_AjusteDepreciacion.Value : Ajuste;
                                                                Ajuste = objReporte.d_AjusteDepreciacion ?? 0;
                                                                //if (Ajuste > 0)
                                                                //{
                                                                objReporte.d_AcumuladoTotal = i == MesDepreciar ? objReporte.AcumuladoAnterior + ImporteMensualHistorico + Ajuste : AcumuladoTotal + ImporteMensualHistorico;
                                                                //}
                                                                //else
                                                                //{
                                                                //    objReporte.d_AcumuladoTotal = i == MesDepreciar ? objReporte.AcumuladoAnterior + ImporteMensualHistorico : AcumuladoTotal + ImporteMensualHistorico;
                                                                //}
                                                                objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoTotal;
                                                            }
                                                        }
                                                        else
                                                        {




                                                            objReporte.MesesDepreciados = MesesDepreciados;
                                                            objReporte.d_ImporteMensualDepreciacion = 0;
                                                            objReporte.AcumuladoAnterior = AcumuladoAnterior;
                                                            objReporte.HistoricoAnterior = null;
                                                            objReporte.ValorNetoAnterior = null;
                                                            objReporte.d_AcumuladoHistorico = AcumuladoHistorico;
                                                            objReporte.AcumuladoComparacion = null;
                                                            objReporte.d_Comparacion = Comparacion;
                                                            objReporte.d_AjusteDepreciacion = 0;
                                                            //objReporte.d_AcumuladoTotal = AcumuladoTotal;
                                                            objReporte.d_AcumuladoTotal = valoradquisicion;
                                                            objReporte.d_ValorNetoActual = ValorNeto;


                                                        }


                                                        break;

                                                    case (int)MotivoActivoFijo.PeriodoActual:
                                                        MesesDepreciados = int.Parse((Math.Round((FechaFin.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                                                        objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                                        objReporte.d_ImporteMensualDepreciacion = ImporteMensualHistorico;
                                                        // Si el activo se esta iniciando a depreciar entonces el acumulado serà 0
                                                        // Si el activo se esta depreciando despues del primer año entonces se objReporte.MesesDepreciados deberà tenerse en cuenta con un mes menos


                                                        if (objOperationResult.Success == 0) return null;
                                                        objReporte.MesesDepreciados = objReporte.MesesDepreciados == 1 ? MesesDepreciados : MesesDepreciados - 1;
                                                        objReporte.AcumuladoAnterior = objReporte.MesesDepreciados == 1 ? 0 : objReporte.MesesDepreciados * objReporte.d_ImporteMensualDepreciacion;// Ajuste > 0 ? objReporte.MesesDepreciados == 1 ? 0 : objReporte.MesesDepreciados * objReporte.d_ImporteMensualDepreciacion + Ajuste : objReporte.MesesDepreciados == 1 ? 0 : objReporte.MesesDepreciados * objReporte.d_ImporteMensualDepreciacion;
                                                        if (MesesDepreciados <= mesesAdepreciar)
                                                        {
                                                            var MesesFaltantes = mesesAdepreciar - MesesDepreciados;
                                                            objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;
                                                            objReporte.AcumuladoComparacion = i == MesDepreciar ? objReporte.ValorAdquisicionHistorico - (MesesDepreciados * ImporteMensualHistorico) : objReporte.ValorAdquisicionHistorico - (MesesDepreciados * ImporteMensualHistorico) - Ajuste;
                                                            objReporte.d_Comparacion = ImporteMensualHistorico * MesesFaltantes;
                                                            objReporte.d_AjusteDepreciacion = (objReporte.AcumuladoComparacion - objReporte.d_Comparacion);
                                                            Ajuste = Ajuste != 0 ? Ajuste : objReporte.d_AjusteDepreciacion ?? 0; // Se agregó
                                                            if (i == MesDepreciar)
                                                            {
                                                                objReporte.d_AcumuladoTotal = objReporte.d_AcumuladoHistorico + objReporte.d_AjusteDepreciacion;
                                                            }
                                                            else if (objReporte.MesesDepreciados == 1)
                                                            {

                                                                objReporte.d_AcumuladoTotal = objReporte.d_AcumuladoHistorico + Ajuste;
                                                            }
                                                            else
                                                            {
                                                                objReporte.d_AcumuladoTotal = AcumuladoTotal + ImporteMensualHistorico;
                                                            }
                                                            objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoTotal;

                                                        }


                                                        else
                                                        {
                                                            objReporte.d_ImporteMensualDepreciacion = 0;
                                                            objReporte.d_AcumuladoHistorico = AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : AcumuladoHistorico;
                                                            objReporte.d_ValorNetoActual = 0;
                                                            objReporte.d_Comparacion = 0;
                                                            objReporte.d_AjusteDepreciacion = 0;
                                                            objReporte.d_AcumuladoTotal = valoradquisicion;
                                                            //  objReporte.d_AcumuladoTotal = AcumuladoTotal;

                                                        }

                                                        break;

                                                }
                                                AcumuladoTotal = objReporte.d_AcumuladoTotal ?? 0;
                                                AcumuladoHistorico = objReporte.d_AcumuladoHistorico ?? 0;
                                                Comparacion = objReporte.d_Comparacion ?? 0;
                                                AcumuladoComparacion = objReporte.AcumuladoComparacion ?? 0;
                                                Ajuste = objReporte.d_AjusteDepreciacion > Ajuste ? objReporte.d_AjusteDepreciacion.Value : Ajuste;
                                                objReporte.AjusteAnterior = Ajuste;
                                                // AjusteVirtual = Ajuste <0 ?Ajuste :AjusteVirtual ;
                                                //objReporte.d_AjusteInicial = AjusteVirtual;
                                                objReporte.d_AjusteInicial = CalcularAjuste(ref objOperationResult, objReporte.ValorAdquisicionHistorico.Value, mesesAdepreciar);
                                                if (objOperationResult.Success == 0) return null;
                                                objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados > int.Parse(item.mesesAdepreciar) ? int.Parse(item.mesesAdepreciar) : MesesDepreciados;
                                            }

                                        }
                                        #endregion
                                        else
                                        #region ActivoBaja
                                        {
                                            if (anioInicio < int.Parse(item.t_FechaBaja.Value.Year.ToString())) // No se toma en cuenta el mes de Baja
                                            {
                                                if (past.Date.Month == 12)
                                                {
                                                    FechaInicio = DateTime.Parse("01/01/" + (past.Date.Year + 1).ToString());
                                                }
                                                else
                                                {
                                                    FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
                                                }
                                                DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());

                                                if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                {
                                                    if (PeriodoReporte >= int.Parse(item.v_PeriodoAnterior) + 1)
                                                    {
                                                        MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 1 : MesesDepreciados + 1;
                                                    }
                                                    else if (PeriodoReporte < int.Parse(item.v_PeriodoAnterior) + 1)
                                                    {

                                                        MesesDepreciados = 0;
                                                    }
                                                    else
                                                    {
                                                        MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 12 : MesesDepreciados + 1;
                                                    }

                                                }
                                                else
                                                {
                                                    MesesDepreciados = int.Parse((Math.Round((FechaFin.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                                                }
                                                objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;

                                                if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                                {
                                                    objReporte.d_ImporteMensualDepreciacion = objReporte.MesesDepreciados == 0 ? 0 : mesesAdepreciar == 0 ? 0 : ImporteMensualHistorico;

                                                    if (i == 1 && item.v_PeriodoAnterior.Trim() == (PeriodoReporte - 1).ToString() && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value + ImporteMensualHistorico;
                                                    }
                                                    else if (i == 12 && PeriodoReporte < (int.Parse(item.v_PeriodoAnterior) + 1) && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value;
                                                    }
                                                    else if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior && PeriodoReporte > int.Parse(item.v_PeriodoAnterior))
                                                    {
                                                        var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()))) - 1;
                                                        objReporte.AcumuladoAnterior = (objReporte.d_ImporteMensualDepreciacion.Value * 12 * DiferenciaAnios) + item.d_ValorAdquisicionMe;
                                                        objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior.Value + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;


                                                    }
                                                    else
                                                    {

                                                        objReporte.AcumuladoAnterior = objReporte.MesesDepreciados * objReporte.d_ImporteMensualDepreciacion;
                                                        objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;

                                                    }

                                                    AcumuladoHistorico = objReporte.d_AcumuladoHistorico.Value;

                                                    if (i != 1 && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoHistorico;
                                                    }
                                                    else
                                                    {

                                                        objReporte.d_ValorNetoActual = AjusteDepreciacionActivo != 0 ? (valoradquisicion - objReporte.d_AcumuladoHistorico) + AjusteDepreciacionActivo : (valoradquisicion - objReporte.d_AcumuladoHistorico);
                                                    }

                                                    objReporte.d_Comparacion = objReporte.MesesDepreciados == 0 && objReporte.d_AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : objReporte.d_ImporteMensualDepreciacion * (mesesAdepreciar - objReporte.MesesDepreciados);
                                                    objReporte.d_AjusteDepreciacion = objReporte.d_ValorNetoActual - objReporte.d_Comparacion;


                                                    if (objReporte.MesesDepreciados == mesesAdepreciar)
                                                    {
                                                        objReporte.d_AcumuladoHistorico = objReporte.d_AcumuladoHistorico + objReporte.d_ValorNetoActual;
                                                        objReporte.d_ValorNetoActual = objReporte.d_ValorNetoActual + objReporte.d_AjusteDepreciacion;
                                                        objReporte.d_AjusteDepreciacion = 0;
                                                    }

                                                }
                                                else
                                                {
                                                    objReporte.d_ImporteMensualDepreciacion = 0;
                                                    objReporte.d_AcumuladoHistorico = 0;
                                                    objReporte.d_ValorNetoActual = 0;
                                                    objReporte.d_Comparacion = 0;
                                                    objReporte.d_AjusteDepreciacion = 0;
                                                    objReporte.ValorAdquisicionHistoricoFinal = 0;

                                                }
                                                objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados > int.Parse(item.mesesAdepreciar) ? int.Parse(item.mesesAdepreciar) : MesesDepreciados;
                                            }
                                            else
                                            {

                                                if (i <= int.Parse(item.t_FechaBaja.Value.Month.ToString()) && anioInicio == item.t_FechaBaja.Value.Year)
                                                {

                                                    if (past.Date.Month == 12)
                                                    {
                                                        FechaInicio = DateTime.Parse("01/01/" + (past.Date.Year + 1).ToString());
                                                    }
                                                    else
                                                    {
                                                        FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
                                                    }
                                                    DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());
                                                    if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        if (PeriodoReporte >= int.Parse(item.v_PeriodoAnterior) + 1)
                                                        {
                                                            MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 1 : MesesDepreciados + 1;
                                                        }
                                                        else if (PeriodoReporte < int.Parse(item.v_PeriodoAnterior) + 1)
                                                        {

                                                            MesesDepreciados = 0;
                                                        }
                                                        else
                                                        {
                                                            MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 12 : MesesDepreciados + 1;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        MesesDepreciados = int.Parse((Math.Round((FechaFin.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                                                    }

                                                    objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                                    if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                                    {

                                                        objReporte.d_ImporteMensualDepreciacion = objReporte.MesesDepreciados == 0 ? 0 : mesesAdepreciar == 0 ? 0 : ImporteMensualHistorico;

                                                        if (i == 1 && item.v_PeriodoAnterior.Trim() == (PeriodoReporte - 1).ToString() && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                        {
                                                            objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value + ImporteMensualHistorico;
                                                        }
                                                        else if (i == 12 && PeriodoReporte < (int.Parse(item.v_PeriodoAnterior) + 1) && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                        {
                                                            objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value;
                                                        }
                                                        else if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior && PeriodoReporte > int.Parse(item.v_PeriodoAnterior))
                                                        {
                                                            var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()))) - 1;
                                                            objReporte.AcumuladoAnterior = (objReporte.d_ImporteMensualDepreciacion.Value * 12 * DiferenciaAnios) + item.d_ValorAdquisicionMe;
                                                            objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior.Value + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;


                                                        }
                                                        else
                                                        {

                                                            objReporte.AcumuladoAnterior = objReporte.MesesDepreciados * objReporte.d_ImporteMensualDepreciacion;
                                                            objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;

                                                        }

                                                        AcumuladoHistorico = objReporte.d_AcumuladoHistorico.Value;

                                                        if (i != 1 && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                        {
                                                            objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoHistorico;
                                                        }
                                                        else
                                                        {

                                                            objReporte.d_ValorNetoActual = AjusteDepreciacionActivo != 0 ? (valoradquisicion - objReporte.d_AcumuladoHistorico) + AjusteDepreciacionActivo : (valoradquisicion - objReporte.d_AcumuladoHistorico);
                                                        }

                                                        objReporte.d_Comparacion = objReporte.MesesDepreciados == 0 && objReporte.d_AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : objReporte.d_ImporteMensualDepreciacion * (mesesAdepreciar - objReporte.MesesDepreciados);
                                                        objReporte.d_AjusteDepreciacion = objReporte.d_ValorNetoActual - objReporte.d_Comparacion;
                                                        if (objReporte.MesesDepreciados == mesesAdepreciar)
                                                        {
                                                            objReporte.d_AcumuladoHistorico = objReporte.d_AcumuladoHistorico + objReporte.d_ValorNetoActual;
                                                            objReporte.d_ValorNetoActual = objReporte.d_ValorNetoActual + objReporte.d_AjusteDepreciacion;
                                                            objReporte.d_AjusteDepreciacion = 0;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        objReporte.d_ImporteMensualDepreciacion = 0;
                                                        objReporte.d_AcumuladoHistorico = 0;
                                                        objReporte.d_ValorNetoActual = 0;
                                                        objReporte.d_Comparacion = 0;
                                                        objReporte.d_AjusteDepreciacion = 0;
                                                        objReporte.ValorAdquisicionHistoricoFinal = 0;

                                                    }
                                                    objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados > int.Parse(item.mesesAdepreciar) ? int.Parse(item.mesesAdepreciar) : MesesDepreciados;

                                                }
                                                else
                                                {
                                                    objReporte.MesesDepreciados = 0;
                                                    objReporte.d_ImporteMensualDepreciacion = 0;
                                                    objReporte.d_AcumuladoHistorico = 0;
                                                    objReporte.d_ValorNetoActual = 0;
                                                    objReporte.d_Comparacion = 0;
                                                    objReporte.d_AjusteDepreciacion = 0;
                                                    objReporte.ValorAdquisicionHistoricoFinal = 0;
                                                    MesesDepreciados = 0;
                                                }
                                            }

                                        }

                                        #endregion
                                    }

                                    if (objReporte.MesesDepreciados >= 0 && MesesDepreciados >= 0)//&&  MesesDepreciados>=0  agregue 29 Diciembre
                                    {

                                        if (objReporte.CodigoActivoFijo == "10000102")
                                        {
                                            string h = "";
                                        }
                                        objReporte.d_Importe1SoloMes = objReporte.d_ImporteMensualDepreciacion.Value;
                                        ListaActivoFijoDepreciacion.Add(objReporte);
                                        AjusteDepreciacionActivo = objReporte.d_AjusteDepreciacion == 0 ? AjusteDepreciacionActivo : objReporte.d_AjusteDepreciacion ?? 0;
                                    }

                                }
                            }

                            anioInicio = anioInicio + 1;
                        }

                    }
                    return ListaActivoFijoDepreciacion;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }

        public List<ReporteLibroActivoDto> ReporteAcumuladoPeriodoAnterior_(ref  OperationResult objOperationResult, int PeriodoReporte, int MesReporte, int CodigoDesde, int CodigoHasta, bool UsoOtrosReportes, string Filtro)
        {
            try
            {
                objOperationResult.Success = 1;
                decimal AjusteDepreciacionActivo = 0;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    DateTime future = new DateTime();
                    if (UsoOtrosReportes)
                    {
                        future = DateTime.Parse(DateTime.DaysInMonth(PeriodoReporte, MesReporte) + "/" + MesReporte + "/" + PeriodoReporte + " 23:59");
                    }
                    else
                    {
                        if (PeriodoReporte < DateTime.Now.Date.Year)
                        {

                            future = DateTime.Parse("31/12/" + PeriodoReporte + " 23:59");
                        }
                        else
                        {
                            future = DateTime.Parse(DateTime.DaysInMonth(PeriodoReporte, MesReporte) + "/" + MesReporte + "/" + PeriodoReporte + " 23:59"); //FechaReporte
                        }
                    }
                    List<ReporteLibroActivoDto> ListaActivoFijoDepreciacion = new List<ReporteLibroActivoDto>();
                    List<activofijoDto> ActivosTotal = new List<activofijoDto>();
                    ReporteLibroActivoDto objReporte = new ReporteLibroActivoDto();
                    DateTime FechaInicio = new DateTime();
                    var ActivosPeriodoActualTodos = (from a in dbContext.activofijo

                                                     join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join
                                                     from b in b_join.DefaultIfEmpty()

                                                     join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                     from c in c_join.DefaultIfEmpty()

                                                     join d in dbContext.datahierarchy on new { cc = a.i_IdCentroCosto.Value, eliminado = 0, grupo = 31 } equals new { cc = d.i_ItemId, eliminado = d.i_IsDeleted.Value, grupo = d.i_GroupId } into d_join
                                                     from d in d_join.DefaultIfEmpty()
                                                     join e in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, tipoactivo = a.i_IdTipoActivo.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, tipoactivo = e.i_ItemId } into e_join
                                                     from e in e_join.DefaultIfEmpty()

                                                     join f in dbContext.datahierarchy on new { Grupo = 107, eliminado = 0, tipoactivo = a.i_IdTipoAdquisicion.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, tipoactivo = f.i_ItemId } into f_join
                                                     from f in f_join.DefaultIfEmpty()

                                                     where a.i_Eliminado == 0 && a.i_Depreciara == 1 && a.t_FechaUso != null && a.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoActual
                                                     && a.t_FechaOrdenCompra.Value.Year <= PeriodoReporte && a.i_EsTemporal == 0  //Se agrego porque en el proceso depreciacion no se toma en cuenta

                                                     select new activofijoDto
                                                     {
                                                         v_IdActivoFijo = a.v_IdActivoFijo,
                                                         d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                         mesesAdepreciar = b.v_Value1,
                                                         t_FechaUso = a.t_FechaUso,
                                                         i_Baja = a.i_Baja == null ? 0 : a.i_Baja,
                                                         t_FechaBaja = a.t_FechaBaja,
                                                         v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                         v_Descricpion = a.v_Descricpion.Trim(),
                                                         v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                         t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                         CodigoActivoFijo = a.v_CodigoActivoFijo,
                                                         Cuenta39 = c == null ? "0" : c.v_Cuenta39,
                                                         Cuenta33 = c == null ? "0" : c.v_Cuenta33,
                                                         CuentaGasto = c == null ? "0" : c.v_CuentaGasto,
                                                         i_MesesDepreciadosPAnterior = a.i_MesesDepreciadosPAnterior,
                                                         i_IdTipoMotivo = a.i_IdTipoMotivo.Value,
                                                         v_PeriodoAnterior = string.IsNullOrEmpty(a.v_PeriodoAnterior) ? "0" : a.v_PeriodoAnterior.Trim(),
                                                         d_ValorAdquisicionMe = a.d_ValorAdquisicionMe == null ? 0 : a.d_ValorAdquisicionMe,
                                                         CentroCosto = d.v_Value2,
                                                         i_IdCentroCosto = a.i_IdCentroCosto,
                                                         CodTipoActivo = e.v_Value2,
                                                         v_Modelo = a.v_Modelo,
                                                         v_Marca = a.v_Marca,
                                                         v_Serie = a.v_Serie,
                                                         v_Placa = a.v_Placa,
                                                         TipoAdquisicion = f.v_Value2,
                                                         i_Depreciara = a.i_Depreciara ?? 0,



                                                     }).ToList().AsQueryable();

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ActivosPeriodoActualTodos = ActivosPeriodoActualTodos.Where(Filtro);

                    }

                    var ActivosPeriodoAnteriorTodos = (from a in dbContext.activofijo
                                                       join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join

                                                       from b in b_join.DefaultIfEmpty()
                                                       join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                       from c in c_join.DefaultIfEmpty()

                                                       join d in dbContext.datahierarchy on new { cc = a.i_IdCentroCosto.Value, eliminado = 0, grupo = 31 } equals new { cc = d.i_ItemId, eliminado = d.i_IsDeleted.Value, grupo = d.i_GroupId } into d_join
                                                       from d in d_join.DefaultIfEmpty()
                                                       join e in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, tipoactivo = a.i_IdTipoActivo.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, tipoactivo = e.i_ItemId } into e_join
                                                       from e in e_join.DefaultIfEmpty()

                                                       join f in dbContext.datahierarchy on new { Grupo = 107, eliminado = 0, tipoactivo = a.i_IdTipoAdquisicion.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, tipoactivo = f.i_ItemId } into f_join
                                                       from f in f_join.DefaultIfEmpty()


                                                       where a.i_Eliminado == 0 && a.i_Depreciara == 1 && a.t_FechaUso != null && a.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior
                                                        && a.i_EsTemporal == 0 // && a.v_Periodo == strPeriodo
                                                       select new activofijoDto
                                                       {

                                                           v_IdActivoFijo = a.v_IdActivoFijo,
                                                           d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                           mesesAdepreciar = b.v_Value1,
                                                           t_FechaUso = a.t_FechaUso,
                                                           i_Baja = a.i_Baja == null ? 0 : a.i_Baja,
                                                           t_FechaBaja = a.t_FechaBaja,
                                                           v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                           v_Descricpion = a.v_Descricpion.Trim(),
                                                           v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                           t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                           Cuenta39 = c == null ? "0" : c.v_Cuenta39,
                                                           CodigoActivoFijo = a.v_CodigoActivoFijo,
                                                           Cuenta33 = c == null ? "0" : c.v_Cuenta33,
                                                           i_MesesDepreciadosPAnterior = a.i_MesesDepreciadosPAnterior,
                                                           i_IdTipoMotivo = a.i_IdTipoMotivo.Value,
                                                           v_PeriodoAnterior = string.IsNullOrEmpty(a.v_PeriodoAnterior) ? "0" : a.v_PeriodoAnterior.Trim(),
                                                           d_ValorAdquisicionMe = a.d_ValorAdquisicionMe == null ? 0 : a.d_ValorAdquisicionMe,
                                                           CuentaGasto = c == null ? "0" : c.v_CuentaGasto,
                                                           CentroCosto = d.v_Value2,
                                                           i_IdCentroCosto = a.i_IdCentroCosto,
                                                           CodTipoActivo = e.v_Value2,
                                                           v_Modelo = a.v_Modelo,
                                                           v_Marca = a.v_Marca,
                                                           v_Serie = a.v_Serie,
                                                           v_Placa = a.v_Placa,
                                                           TipoAdquisicion = f.v_Value2,
                                                           i_Depreciara = a.i_Depreciara ?? 0,
                                                       }).ToList().AsQueryable();

                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ActivosPeriodoAnteriorTodos = ActivosPeriodoAnteriorTodos.Where(Filtro);

                    }


                    var ActivosNoDepreciaran = (from a in dbContext.activofijo

                                                join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join
                                                from b in b_join.DefaultIfEmpty()

                                                join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                from c in c_join.DefaultIfEmpty()

                                                join d in dbContext.datahierarchy on new { cc = a.i_IdCentroCosto.Value, eliminado = 0, grupo = 31 } equals new { cc = d.i_ItemId, eliminado = d.i_IsDeleted.Value, grupo = d.i_GroupId } into d_join
                                                from d in d_join.DefaultIfEmpty()
                                                join e in dbContext.datahierarchy on new { Grupo = 104, eliminado = 0, tipoactivo = a.i_IdTipoActivo.Value } equals new { Grupo = e.i_GroupId, eliminado = e.i_IsDeleted.Value, tipoactivo = e.i_ItemId } into e_join
                                                from e in e_join.DefaultIfEmpty()

                                                join f in dbContext.datahierarchy on new { Grupo = 107, eliminado = 0, tipoactivo = a.i_IdTipoAdquisicion.Value } equals new { Grupo = f.i_GroupId, eliminado = f.i_IsDeleted.Value, tipoactivo = f.i_ItemId } into f_join
                                                from f in f_join.DefaultIfEmpty()

                                                where a.i_Eliminado == 0 && a.i_Depreciara == 0
                                                select new activofijoDto
                                                {
                                                    v_IdActivoFijo = a.v_IdActivoFijo,
                                                    d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                    mesesAdepreciar = b.v_Value1,
                                                    t_FechaUso = a.t_FechaUso,
                                                    i_Baja = a.i_Baja == null ? 0 : a.i_Baja,
                                                    t_FechaBaja = a.t_FechaBaja,
                                                    v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                    v_Descricpion = a.v_Descricpion.Trim(),
                                                    v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                    t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                    CodigoActivoFijo = a.v_CodigoActivoFijo,
                                                    Cuenta39 = c == null ? "0" : c.v_Cuenta39,
                                                    Cuenta33 = c == null ? "0" : c.v_Cuenta33,
                                                    CuentaGasto = c == null ? "0" : c.v_CuentaGasto,
                                                    i_MesesDepreciadosPAnterior = a.i_MesesDepreciadosPAnterior,
                                                    i_IdTipoMotivo = a.i_IdTipoMotivo.Value,
                                                    v_PeriodoAnterior = string.IsNullOrEmpty(a.v_PeriodoAnterior) ? "0" : a.v_PeriodoAnterior.Trim(),
                                                    d_ValorAdquisicionMe = a.d_ValorAdquisicionMe == null ? 0 : a.d_ValorAdquisicionMe,
                                                    CentroCosto = d.v_Value2,
                                                    i_IdCentroCosto = a.i_IdCentroCosto,
                                                    CodTipoActivo = e.v_Value2,
                                                    v_Modelo = a.v_Modelo,
                                                    v_Marca = a.v_Marca,
                                                    v_Serie = a.v_Serie,
                                                    v_Placa = a.v_Placa,
                                                    TipoAdquisicion = f.v_Value2,

                                                    i_Depreciara = a.i_Depreciara ?? 0,

                                                }).ToList().AsQueryable();
                    if (!string.IsNullOrEmpty(Filtro))
                    {
                        ActivosNoDepreciaran = ActivosNoDepreciaran.Where(Filtro);

                    }

                    if (CodigoDesde != 0 && CodigoHasta != 0)
                    {
                        var ActivosPeriodoAnterior = (from a in ActivosPeriodoAnteriorTodos

                                                      where int.Parse(a.v_CodigoActivoFijo) >= CodigoDesde && int.Parse(a.v_CodigoActivoFijo) <= CodigoHasta

                                                      select a);



                        var ActivosPeriodoActual = (from a in ActivosPeriodoActualTodos

                                                    where int.Parse(a.v_CodigoActivoFijo) >= CodigoDesde && int.Parse(a.v_CodigoActivoFijo) <= CodigoHasta

                                                    select a);


                        var ActivosQueNoSeDepreciaran = (from a in ActivosNoDepreciaran
                                                         where int.Parse(a.v_CodigoActivoFijo) >= CodigoDesde && int.Parse(a.v_CodigoActivoFijo) <= CodigoHasta

                                                         select a);

                        ActivosTotal = ActivosPeriodoAnterior.Concat(ActivosPeriodoActual).Concat(ActivosQueNoSeDepreciaran).ToList().OrderBy(o => o.CodigoActivoFijo).ToList();
                    }
                    else
                    {
                        ActivosTotal = ActivosPeriodoActualTodos.Concat(ActivosPeriodoAnteriorTodos).Concat(ActivosNoDepreciaran).ToList().OrderBy(o => o.CodigoActivoFijo).ToList();
                    }


                    foreach (var item in ActivosTotal.AsParallel())
                    {

                        AjusteDepreciacionActivo = 0;
                        DateTime past = item.t_FechaUso.Value.Date; // Fecha Uso
                        decimal valoradquisicion = item.d_MonedaNacional.Value;
                        decimal AcumuladoHistorico = 0;
                        int mesesAdepreciar = string.IsNullOrEmpty(item.mesesAdepreciar) ? 0 : int.Parse(item.mesesAdepreciar);
                        int anioInicio = int.Parse(future.Date.Year.ToString()); // future .... Ultimo dia de la fecha del reporte 31/08/2015
                        int MesesDepreciados = 0;
                        int anioFin = int.Parse(future.Date.Year.ToString());
                        while (anioInicio <= anioFin)
                        {
                            int MesDepreciar = item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior && PeriodoReporte == int.Parse(item.v_PeriodoAnterior) ? 12 : 1;
                            for (int i = MesDepreciar; i <= 12; i++)
                            {
                                if (i <= int.Parse(future.Date.Month.ToString()) || anioInicio < anioFin)
                                {


                                    objReporte = new ReporteLibroActivoDto();
                                    objReporte.NombreMes = new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(i).ToUpper();
                                    objReporte.Cuenta33 = item.Cuenta33;
                                    objReporte.Cuenta39 = item.Cuenta39;
                                    objReporte.v_IdActivoFijo = item.v_IdActivoFijo;
                                    objReporte.i_Baja = item.i_Baja.Value;
                                    objReporte.AnioBaja = item.t_FechaBaja == null ? "" : item.t_FechaBaja.Value.Year.ToString();
                                    objReporte.FechaUso = item.t_FechaUso.Value;
                                    objReporte.sFechaUso = item.i_Depreciara == 1 ? objReporte.FechaUso.ToShortDateString() : "";
                                    objReporte.FechaCompra = item.t_FechaOrdenCompra.Value;
                                    objReporte.CuentaGasto = item.CuentaGasto;
                                    objReporte.CentroCosto = item.CentroCosto;
                                    objReporte.CodigoActivoFijo = item.CodigoActivoFijo;
                                    objReporte.FechaBaja = item.t_FechaBaja == null ? DateTime.Now : item.t_FechaBaja.Value;
                                    objReporte.i_IdCentroCosto = item.i_IdCentroCosto.Value;
                                    objReporte.TipoActivo = item.CodTipoActivo;
                                    objReporte.v_NumeroFactura = item.v_NumeroFactura;
                                    objReporte.i_Depreciara = item.i_Depreciara.Value;
                                    objReporte.v_Serie = item.v_Serie;
                                    objReporte.v_Marca = item.v_Marca;
                                    objReporte.v_Modelo = item.v_Modelo;
                                    objReporte.v_Placa = item.v_Placa;
                                    objReporte.TipoAdquisicion = item.TipoAdquisicion;
                                    objReporte.AnioCompra = item.t_FechaOrdenCompra == null ? "" : item.t_FechaOrdenCompra.Value.Year.ToString();
                                    objReporte.MesCompra = item.t_FechaOrdenCompra == null ? "" : item.t_FechaOrdenCompra.Value.Month.ToString();
                                    objReporte.AnioUso = item.t_FechaUso == null ? "" : item.t_FechaUso.Value.Year.ToString();
                                    objReporte.MesesDepreciar = string.IsNullOrEmpty(item.mesesAdepreciar) ? "0" : item.mesesAdepreciar;
                                    string x = ("CÓDIGO : " + item.v_CodigoActivoFijo + "  ,  " + "DESCRIPCIÓN : " + item.v_Descricpion + "  ,  " + "FACTURA : " + item.v_NumeroFactura + "  ,  " + "FECHA ADQUISICIÓN : " + item.t_FechaOrdenCompra.Value.Date.Day.ToString("00") + "/" + item.t_FechaOrdenCompra.Value.Month.ToString("00") + "/"
                                        + item.t_FechaOrdenCompra.Value.Year.ToString() + "  ,  " + "FECHA USO : ");
                                    string y = (item.t_FechaUso == null ? "" : item.t_FechaUso.Value.Date.Day.ToString("00") + "/" + item.t_FechaUso.Value.Date.Month.ToString("00") + "/" + item.t_FechaUso.Value.Date.Year.ToString() + "  ,  " + "\n" + "MESES A DEPRECIAR : " + mesesAdepreciar.ToString());
                                    string baja = item.t_FechaBaja == null ? " " : " FECHA BAJA : " + item.t_FechaBaja.Value.Date.Day.ToString("00") + "/" + item.t_FechaBaja.Value.Month.ToString("00") + "/" + item.t_FechaBaja.Value.Date.Year.ToString();
                                    objReporte.DescripcionActivo = (x + y + baja).ToUpper();
                                    objReporte.SoloDescripionActivo = item.v_Descricpion.Trim();
                                    objReporte.ValorAdquisicionHistorico = item.d_MonedaNacional.Value;
                                    decimal ImporteMensualHistorico = mesesAdepreciar == 0 ? 0 : Math.Round(valoradquisicion / mesesAdepreciar, 2);
                                    if (item.i_Depreciara == 0)
                                    {
                                        MesesDepreciados = 0;
                                        objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                        objReporte.d_AcumuladoHistorico = 0;
                                        objReporte.d_ValorNetoActual = valoradquisicion;
                                        objReporte.d_Comparacion = 0;
                                        objReporte.d_AjusteDepreciacion = 0;
                                        objReporte.d_ImporteMensualDepreciacion = 0;
                                        objReporte.d_Importe1SoloMes = objReporte.d_ImporteMensualDepreciacion.Value;
                                        AjusteDepreciacionActivo = objReporte.d_AjusteDepreciacion.Value == 0 ? AjusteDepreciacionActivo : objReporte.d_AjusteDepreciacion.Value;

                                    }
                                    else
                                    {

                                        #region ActivoSinBaja
                                        if (item.i_Baja == 0)
                                        {

                                            if (past.Date.Month == 12)
                                            {
                                                FechaInicio = DateTime.Parse("01/01/" + (past.Date.Year + 1).ToString());
                                            }
                                            else
                                            {
                                                FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
                                            }
                                            DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());

                                            if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                            {
                                                if (PeriodoReporte == int.Parse(item.v_PeriodoAnterior) + 1)
                                                {
                                                    MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 1 : MesesDepreciados + 1;
                                                }
                                                else if (PeriodoReporte < int.Parse(item.v_PeriodoAnterior) + 1)
                                                {
                                                    MesesDepreciados = 0;
                                                }
                                                else
                                                {
                                                    var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()) + 1));
                                                    MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + (12 * (DiferenciaAnios)) + 1 : MesesDepreciados + 1;
                                                }

                                            }
                                            else
                                            {
                                                MesesDepreciados = int.Parse((Math.Round((FechaFin.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                                            }
                                            objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                            if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                            {
                                                objReporte.d_ImporteMensualDepreciacion = objReporte.MesesDepreciados == 0 ? 0 : mesesAdepreciar == 0 ? 0 : ImporteMensualHistorico;

                                                if (i == 1 && item.v_PeriodoAnterior.Trim() == (PeriodoReporte - 1).ToString() && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                {
                                                    objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value + ImporteMensualHistorico;
                                                }
                                                else if (i == 12 && PeriodoReporte < (int.Parse(item.v_PeriodoAnterior) + 1) && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                {
                                                    objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value;
                                                }
                                                else if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior && PeriodoReporte > int.Parse(item.v_PeriodoAnterior))
                                                {
                                                    var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()))) - 1;
                                                    objReporte.AcumuladoAnterior = (objReporte.d_ImporteMensualDepreciacion.Value * 12 * DiferenciaAnios) + item.d_ValorAdquisicionMe;
                                                    objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior.Value + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;


                                                }
                                                else
                                                {
                                                    objReporte.AcumuladoAnterior = objReporte.MesesDepreciados * objReporte.d_ImporteMensualDepreciacion;
                                                    objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;

                                                }

                                                AcumuladoHistorico = objReporte.d_AcumuladoHistorico.Value;

                                                if (i != 1 && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                {
                                                    objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoHistorico;
                                                }
                                                else
                                                {

                                                    //objReporte.d_ValorNetoActual = AjusteDepreciacionActivo != 0 ? (valoradquisicion - objReporte.d_AcumuladoHistorico) + AjusteDepreciacionActivo : (valoradquisicion - objReporte.d_AcumuladoHistorico);
                                                    objReporte.d_ValorNetoActual = AjusteDepreciacionActivo != 0 ? (valoradquisicion - objReporte.d_AcumuladoHistorico) : (valoradquisicion - objReporte.d_AcumuladoHistorico);
                                                }

                                                objReporte.d_Comparacion = objReporte.MesesDepreciados == 0 && objReporte.d_AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : objReporte.d_ImporteMensualDepreciacion * (mesesAdepreciar - objReporte.MesesDepreciados);
                                                objReporte.d_AjusteDepreciacion = objReporte.d_ValorNetoActual - objReporte.d_Comparacion;

                                                if (objReporte.MesesDepreciados == mesesAdepreciar)
                                                {
                                                    objReporte.d_AcumuladoHistorico = objReporte.d_AcumuladoHistorico + objReporte.d_ValorNetoActual;
                                                    objReporte.d_ValorNetoActual = objReporte.d_ValorNetoActual + objReporte.d_AjusteDepreciacion;
                                                    objReporte.d_AjusteDepreciacion = 0;
                                                }


                                            }
                                            else
                                            {// Cuando el activo se acabó de depreciar  debe aparecer el acumulado historico
                                                objReporte.d_ImporteMensualDepreciacion = 0;
                                                objReporte.d_AcumuladoHistorico = AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : AcumuladoHistorico;
                                                objReporte.d_ValorNetoActual = 0;
                                                objReporte.d_Comparacion = 0;
                                                objReporte.d_AjusteDepreciacion = 0;
                                                objReporte.ValorAdquisicionHistoricoFinal = 0;

                                            }

                                            objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados > int.Parse(item.mesesAdepreciar) ? int.Parse(item.mesesAdepreciar) : MesesDepreciados;



                                        }
                                        #endregion
                                        else
                                        #region ActivoBaja
                                        {
                                            if (anioInicio < int.Parse(item.t_FechaBaja.Value.Year.ToString())) // No se toma en cuenta el mes de Baja
                                            {
                                                //  DateTime FechaInicio = new DateTime();
                                                if (past.Date.Month == 12)
                                                {
                                                    FechaInicio = DateTime.Parse("01/01/" + (past.Date.Year + 1).ToString());
                                                }
                                                else
                                                {
                                                    FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
                                                }
                                                DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());
                                                // MesesDepreciados = int.Parse((Math.Round((FechaFin.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                                                if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                {
                                                    if (PeriodoReporte >= int.Parse(item.v_PeriodoAnterior) + 1)
                                                    {
                                                        MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 1 : MesesDepreciados + 1;
                                                    }
                                                    else if (PeriodoReporte < int.Parse(item.v_PeriodoAnterior) + 1)
                                                    {

                                                        MesesDepreciados = 0;
                                                    }
                                                    else
                                                    {
                                                        MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 12 : MesesDepreciados + 1;
                                                    }

                                                }
                                                else
                                                {
                                                    MesesDepreciados = int.Parse((Math.Round((FechaFin.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                                                }
                                                objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;

                                                if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                                {
                                                    objReporte.d_ImporteMensualDepreciacion = objReporte.MesesDepreciados == 0 ? 0 : mesesAdepreciar == 0 ? 0 : ImporteMensualHistorico;

                                                    if (i == 1 && item.v_PeriodoAnterior.Trim() == (PeriodoReporte - 1).ToString() && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value + ImporteMensualHistorico;
                                                    }
                                                    else if (i == 12 && PeriodoReporte < (int.Parse(item.v_PeriodoAnterior) + 1) && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value;
                                                    }
                                                    else if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior && PeriodoReporte > int.Parse(item.v_PeriodoAnterior))
                                                    {
                                                        var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()))) - 1;
                                                        objReporte.AcumuladoAnterior = (objReporte.d_ImporteMensualDepreciacion.Value * 12 * DiferenciaAnios) + item.d_ValorAdquisicionMe;
                                                        objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior.Value + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;


                                                    }
                                                    else
                                                    {

                                                        objReporte.AcumuladoAnterior = objReporte.MesesDepreciados * objReporte.d_ImporteMensualDepreciacion;
                                                        objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;

                                                    }

                                                    AcumuladoHistorico = objReporte.d_AcumuladoHistorico.Value;

                                                    if (i != 1 && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoHistorico;
                                                    }
                                                    else
                                                    {

                                                        objReporte.d_ValorNetoActual = AjusteDepreciacionActivo != 0 ? (valoradquisicion - objReporte.d_AcumuladoHistorico) + AjusteDepreciacionActivo : (valoradquisicion - objReporte.d_AcumuladoHistorico);
                                                    }

                                                    objReporte.d_Comparacion = objReporte.MesesDepreciados == 0 && objReporte.d_AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : objReporte.d_ImporteMensualDepreciacion * (mesesAdepreciar - objReporte.MesesDepreciados);
                                                    objReporte.d_AjusteDepreciacion = objReporte.d_ValorNetoActual - objReporte.d_Comparacion;


                                                    if (objReporte.MesesDepreciados == mesesAdepreciar)
                                                    {
                                                        objReporte.d_AcumuladoHistorico = objReporte.d_AcumuladoHistorico + objReporte.d_ValorNetoActual;
                                                        objReporte.d_ValorNetoActual = objReporte.d_ValorNetoActual + objReporte.d_AjusteDepreciacion;
                                                        objReporte.d_AjusteDepreciacion = 0;
                                                    }

                                                }
                                                else
                                                {
                                                    objReporte.d_ImporteMensualDepreciacion = 0;
                                                    objReporte.d_AcumuladoHistorico = 0;
                                                    objReporte.d_ValorNetoActual = 0;
                                                    objReporte.d_Comparacion = 0;
                                                    objReporte.d_AjusteDepreciacion = 0;
                                                    objReporte.ValorAdquisicionHistoricoFinal = 0;

                                                }
                                                objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados > int.Parse(item.mesesAdepreciar) ? int.Parse(item.mesesAdepreciar) : MesesDepreciados;
                                            }
                                            else
                                            {

                                                if (i <= int.Parse(item.t_FechaBaja.Value.Month.ToString()) && anioInicio == item.t_FechaBaja.Value.Year)
                                                {

                                                    if (past.Date.Month == 12)
                                                    {
                                                        FechaInicio = DateTime.Parse("01/01/" + (past.Date.Year + 1).ToString());
                                                    }
                                                    else
                                                    {
                                                        FechaInicio = DateTime.Parse("01/" + (int.Parse(past.Date.Month.ToString()) + 1).ToString() + "/" + past.Date.Year.ToString());
                                                    }
                                                    DateTime FechaFin = DateTime.Parse(DateTime.DaysInMonth(anioInicio, i).ToString() + "/" + i.ToString() + "/" + anioInicio.ToString());
                                                    if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                    {
                                                        if (PeriodoReporte >= int.Parse(item.v_PeriodoAnterior) + 1)
                                                        {
                                                            MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 1 : MesesDepreciados + 1;
                                                        }
                                                        else if (PeriodoReporte < int.Parse(item.v_PeriodoAnterior) + 1)
                                                        {
                                                            //MesesDepreciados = item.i_MesesDepreciadosPAnterior.Value;
                                                            MesesDepreciados = 0;
                                                        }
                                                        else
                                                        {
                                                            MesesDepreciados = i == 1 ? item.i_MesesDepreciadosPAnterior.Value + 12 : MesesDepreciados + 1;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        MesesDepreciados = int.Parse((Math.Round((FechaFin.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                                                    }

                                                    objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados;
                                                    if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                                    {

                                                        objReporte.d_ImporteMensualDepreciacion = objReporte.MesesDepreciados == 0 ? 0 : mesesAdepreciar == 0 ? 0 : ImporteMensualHistorico;

                                                        if (i == 1 && item.v_PeriodoAnterior.Trim() == (PeriodoReporte - 1).ToString() && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                        {
                                                            objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value + ImporteMensualHistorico;
                                                        }
                                                        else if (i == 12 && PeriodoReporte < (int.Parse(item.v_PeriodoAnterior) + 1) && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                        {
                                                            objReporte.d_AcumuladoHistorico = item.d_ValorAdquisicionMe.Value;
                                                        }
                                                        else if (item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior && PeriodoReporte > int.Parse(item.v_PeriodoAnterior))
                                                        {
                                                            var DiferenciaAnios = (PeriodoReporte - (int.Parse(item.v_PeriodoAnterior.Trim()))) - 1;
                                                            objReporte.AcumuladoAnterior = (objReporte.d_ImporteMensualDepreciacion.Value * 12 * DiferenciaAnios) + item.d_ValorAdquisicionMe;
                                                            objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior.Value + objReporte.d_ImporteMensualDepreciacion.Value : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;


                                                        }
                                                        else
                                                        {
                                                            // objReporte.d_AcumuladoHistorico = objReporte.MesesDepreciados * objReporte.d_ImporteMensualDepreciacion;
                                                            objReporte.AcumuladoAnterior = objReporte.MesesDepreciados * objReporte.d_ImporteMensualDepreciacion;
                                                            objReporte.d_AcumuladoHistorico = i == MesDepreciar ? objReporte.AcumuladoAnterior : AcumuladoHistorico + objReporte.d_ImporteMensualDepreciacion;

                                                        }

                                                        AcumuladoHistorico = objReporte.d_AcumuladoHistorico.Value;

                                                        if (i != 1 && item.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior)
                                                        {
                                                            objReporte.d_ValorNetoActual = valoradquisicion - objReporte.d_AcumuladoHistorico;
                                                        }
                                                        else
                                                        {

                                                            objReporte.d_ValorNetoActual = AjusteDepreciacionActivo != 0 ? (valoradquisicion - objReporte.d_AcumuladoHistorico) + AjusteDepreciacionActivo : (valoradquisicion - objReporte.d_AcumuladoHistorico);
                                                        }

                                                        objReporte.d_Comparacion = objReporte.MesesDepreciados == 0 && objReporte.d_AcumuladoHistorico == 0 ? objReporte.ValorAdquisicionHistorico : objReporte.d_ImporteMensualDepreciacion * (mesesAdepreciar - objReporte.MesesDepreciados);
                                                        objReporte.d_AjusteDepreciacion = objReporte.d_ValorNetoActual - objReporte.d_Comparacion;
                                                        if (objReporte.MesesDepreciados == mesesAdepreciar)
                                                        {
                                                            objReporte.d_AcumuladoHistorico = objReporte.d_AcumuladoHistorico + objReporte.d_ValorNetoActual;
                                                            objReporte.d_ValorNetoActual = objReporte.d_ValorNetoActual + objReporte.d_AjusteDepreciacion;
                                                            objReporte.d_AjusteDepreciacion = 0;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        objReporte.d_ImporteMensualDepreciacion = 0;
                                                        objReporte.d_AcumuladoHistorico = 0;
                                                        objReporte.d_ValorNetoActual = 0;
                                                        objReporte.d_Comparacion = 0;
                                                        objReporte.d_AjusteDepreciacion = 0;
                                                        objReporte.ValorAdquisicionHistoricoFinal = 0;

                                                    }
                                                    objReporte.MesesDepreciados = MesesDepreciados < 0 ? 0 : MesesDepreciados > int.Parse(item.mesesAdepreciar) ? int.Parse(item.mesesAdepreciar) : MesesDepreciados;

                                                }
                                                else
                                                {
                                                    objReporte.MesesDepreciados = 0;
                                                    objReporte.d_ImporteMensualDepreciacion = 0;
                                                    objReporte.d_AcumuladoHistorico = 0;
                                                    objReporte.d_ValorNetoActual = 0;
                                                    objReporte.d_Comparacion = 0;
                                                    objReporte.d_AjusteDepreciacion = 0;
                                                    objReporte.ValorAdquisicionHistoricoFinal = 0;
                                                    //ListaActivoFijoDepreciacion.Add(objReporte);
                                                    MesesDepreciados = 0;
                                                }
                                            }

                                        }

                                        #endregion
                                    }

                                    if (objReporte.MesesDepreciados >= 0 && MesesDepreciados >= 0)//&&  MesesDepreciados>=0  agregue 29 Diciembre
                                    {

                                        if (objReporte.CodigoActivoFijo == "10000102")
                                        {
                                            string h = "";
                                        }
                                        objReporte.d_Importe1SoloMes = objReporte.d_ImporteMensualDepreciacion.Value;
                                        ListaActivoFijoDepreciacion.Add(objReporte);
                                        AjusteDepreciacionActivo = objReporte.d_AjusteDepreciacion.Value == 0 ? AjusteDepreciacionActivo : objReporte.d_AjusteDepreciacion.Value;
                                    }

                                }
                            }

                            anioInicio = anioInicio + 1;
                        }

                    }
                    return ListaActivoFijoDepreciacion;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }
        }


        private decimal CalcularAjuste(ref  OperationResult objOpetarionResult, decimal ValorCosto, int MesesADepreciar)
        {
            try
            {
                objOpetarionResult.Success = 1;
                int MesesFaltantes = MesesADepreciar - 1;
                decimal Mensual = Utils.Windows.DevuelveValorRedondeado(ValorCosto / MesesADepreciar, 2);
                decimal Comparacion = ValorCosto - Mensual;
                decimal ValorNeto = MesesFaltantes * Mensual;
                return Comparacion - ValorNeto;
            }
            catch (Exception ex)
            {
                objOpetarionResult.Success = 0;
                return 0;
            }
        }

        public List<ReporteAsientoDiario> ReporteAsientoInventario(ref OperationResult objOperationResult, int PeriodoReporte, int MesReporte, bool DepreciacionHistorico, bool AjusteDepreciacion, decimal TipoCambio)
        {
            List<ReporteAsientoDiario> ListaFinal = new List<ReporteAsientoDiario>();
            var ListaDepreciacionHistorica = new List<DiarioDepreciacionHistorica>();
            var objDepreciacionHist = new DiarioDepreciacionHistorica();
            var objAjuste = new DiarioAjusteDepreciacion();
            var ListaAjuste = new List<DiarioAjusteDepreciacion>();
            ReporteAsientoDiario _diarioDetalleDto = new ReporteAsientoDiario();
            var TodosActivos = ReporteLibroActivo(ref objOperationResult, PeriodoReporte, MesReporte, 0, 0, true, null);
            AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
            TodosActivos = TodosActivos.GroupBy(o => o.v_IdActivoFijo).Select(o => o.LastOrDefault()).ToList().OrderBy(o => o.CodigoActivoFijo).ToList();

            foreach (var item in TodosActivos)
            {

                objDepreciacionHist = new DiarioDepreciacionHistorica();
                objAjuste = new DiarioAjusteDepreciacion();
                objDepreciacionHist.CantidadMensual = item.d_Importe1SoloMes;
                objDepreciacionHist.CuentaGasto = item.CuentaGasto;
                objDepreciacionHist.KeyCuenta3 = objDepreciacionHist.CuentaGasto.Substring(0, 1);
                objDepreciacionHist.Cuenta39 = item.Cuenta39;
                objDepreciacionHist.CentroCosto = item.CentroCosto;
                objDepreciacionHist.CodigoActivoFijo = item.CodigoActivoFijo;
                objAjuste.CantidadAjuste = item.d_AjusteDepreciacion;

                objAjuste.Cuenta39 = item.Cuenta39;
                objAjuste.CentroCosto = item.CentroCosto;
                objAjuste.CuentaGasto = item.CuentaGasto;
                objAjuste.KeyCuenta3 = objAjuste.Cuenta39.Substring(0, 1);
                objAjuste.CodigoActivoFijo = item.CodigoActivoFijo;
                ListaDepreciacionHistorica.Add(objDepreciacionHist);
                ListaAjuste.Add(objAjuste);
            }

            List<DiarioDepreciacionHistorica> ListaResultHistorico = new List<DiarioDepreciacionHistorica>();


            var GruposHistoricos = ListaDepreciacionHistorica.GroupBy(x => new { x.CuentaGasto, x.Cuenta39, x.CentroCosto }).Select(x => x.FirstOrDefault()).OrderBy(o => o.Cuenta39).ToList();
            var GruposAjuste = ListaAjuste.GroupBy(x => new { x.CuentaGasto, x.Cuenta39, x.CentroCosto }).Select(x => x.FirstOrDefault()).OrderBy(o => o.Cuenta39).ToList();
            foreach (var GrupoHistorico in GruposHistoricos)
            {
                DiarioDepreciacionHistorica Item = new DiarioDepreciacionHistorica();
                Item.CuentaGasto = GrupoHistorico.CuentaGasto;
                Item.CantidadMensual = ListaDepreciacionHistorica.Where(x => x.CuentaGasto == GrupoHistorico.CuentaGasto && x.CentroCosto == GrupoHistorico.CentroCosto && x.Cuenta39 == GrupoHistorico.Cuenta39).Sum(x => x.CantidadMensual);
                Item.Cuenta39 = GrupoHistorico.Cuenta39;
                Item.i_IdCentroCosto = GrupoHistorico.i_IdCentroCosto;
                Item.CentroCosto = GrupoHistorico.CentroCosto;
                Item.KeyCuenta3 = Item.Cuenta39.Substring(0, 1);
                Item.CodigoActivoFijo = GrupoHistorico.CodigoActivoFijo;
                ListaResultHistorico.Add(Item);
            }
            //List<DiarioAjusteDepreciacion> ListaResultAjuste = new List<DiarioAjusteDepreciacion>();
            //foreach (var GrupoAjuste in GruposAjuste)
            //{
            //    DiarioAjusteDepreciacion ItemAjuste = new DiarioAjusteDepreciacion();
            //    ItemAjuste.CantidadAjuste = ListaAjuste.Where(x => x.CuentaGasto == GrupoAjuste.CuentaGasto && x.CentroCosto == GrupoAjuste.CentroCosto && x.Cuenta39 ==GrupoAjuste.Cuenta39 ).Sum(x => x.CantidadAjuste);
            //    if (ItemAjuste.CantidadAjuste > 0)
            //    {

            //        ItemAjuste.CuentaGasto = GrupoAjuste.CuentaGasto;
            //        ItemAjuste.Cuenta39 = GrupoAjuste.Cuenta39;
            //        ItemAjuste.i_IdCentroCosto = GrupoAjuste.i_IdCentroCosto;
            //        ItemAjuste.KeyCuenta3 = ItemAjuste.Cuenta39.Substring(0, 1);
            //        ItemAjuste.CentroCosto = GrupoAjuste.CentroCosto;
            //        ItemAjuste.CodigoActivoFijo = GrupoAjuste.CodigoActivoFijo;
            //        ListaResultAjuste.Add(ItemAjuste);
            //    }
            //}

            if (DepreciacionHistorico)
            {
                #region Genera Libro Diario Depreciación Histórico

                ListaResultHistorico = ListaResultHistorico.OrderBy(o => o.Cuenta39).ToList();
                foreach (var CuentaActivoFijo in ListaResultHistorico)
                {
                    int i = 1;


                    if (i == 1)
                    {
                        _diarioDetalleDto = new ReporteAsientoDiario();
                        _diarioDetalleDto.Cuenta = CuentaActivoFijo.Cuenta39;
                        _diarioDetalleDto.TipoCambio = TipoCambio;
                        var AsientoContable39 = _objAsientoContableBL.ObtenAsientosPorNro(ref  objOperationResult, CuentaActivoFijo.Cuenta39);
                        _diarioDetalleDto.Anexo = null;
                        _diarioDetalleDto.Naturaleza = AsientoContable39 != null ? AsientoContable39.i_Naturaleza == 1 ? "D" : AsientoContable39.i_Naturaleza == 2 ? "H" : "" : "";
                        _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(CuentaActivoFijo.CantidadMensual.Value, 2);
                        _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe / TipoCambio, 2);
                        _diarioDetalleDto.Debe = _diarioDetalleDto.Naturaleza == "D" ? _diarioDetalleDto.d_Importe : _diarioDetalleDto.d_Cambio;
                        _diarioDetalleDto.Haber = _diarioDetalleDto.Naturaleza == "H" ? _diarioDetalleDto.d_Importe : _diarioDetalleDto.d_Cambio;
                        _diarioDetalleDto.Documento = CuentaActivoFijo.CodigoActivoFijo;
                        _diarioDetalleDto.FechaAsiento = DateTime.Now.ToShortDateString();
                        _diarioDetalleDto.FechaVencimiento = DateTime.Now.ToShortDateString();
                        _diarioDetalleDto.CentroCosto = CuentaActivoFijo.CentroCosto;
                        _diarioDetalleDto.GlosaAsiento = "";
                        _diarioDetalleDto.MonedaExtranjera = 0;
                        _diarioDetalleDto.NombreAsiento = "";
                        _diarioDetalleDto.KeyCuenta3 = _diarioDetalleDto.Cuenta != null ? _diarioDetalleDto.Cuenta.Substring(0, 1) : "";
                        _diarioDetalleDto.NroAsiento = "ASIENTO DEPRECIACIÓN";
                        ListaFinal.Add(_diarioDetalleDto);
                        i = i + 1;
                    }
                    if (i == 2)
                    {
                        _diarioDetalleDto = new ReporteAsientoDiario();
                        _diarioDetalleDto.Cuenta = CuentaActivoFijo.CuentaGasto;
                        _diarioDetalleDto.TipoCambio = TipoCambio;
                        var AsientoContableGasto = _objAsientoContableBL.ObtenAsientosPorNro(ref  objOperationResult, CuentaActivoFijo.CuentaGasto);
                        _diarioDetalleDto.Anexo = null;
                        _diarioDetalleDto.Naturaleza = AsientoContableGasto != null ? AsientoContableGasto.i_Naturaleza == 1 ? "D" : AsientoContableGasto.i_Naturaleza == 2 ? "H" : "" : "";
                        _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(CuentaActivoFijo.CantidadMensual.Value, 2);
                        _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe / TipoCambio, 2);
                        _diarioDetalleDto.Debe = _diarioDetalleDto.Naturaleza == "D" ? _diarioDetalleDto.d_Importe : _diarioDetalleDto.d_Cambio;
                        _diarioDetalleDto.Haber = _diarioDetalleDto.Naturaleza == "H" ? _diarioDetalleDto.d_Importe : _diarioDetalleDto.d_Cambio;
                        _diarioDetalleDto.Documento = CuentaActivoFijo.CodigoActivoFijo;
                        _diarioDetalleDto.FechaAsiento = DateTime.Now.ToShortDateString();
                        _diarioDetalleDto.FechaVencimiento = DateTime.Now.ToShortDateString();
                        _diarioDetalleDto.CentroCosto = CuentaActivoFijo.CentroCosto;
                        _diarioDetalleDto.GlosaAsiento = "";
                        _diarioDetalleDto.MonedaExtranjera = 0;
                        _diarioDetalleDto.NombreAsiento = "";
                        _diarioDetalleDto.KeyCuenta3 = _diarioDetalleDto.Cuenta != null ? _diarioDetalleDto.Cuenta.Substring(0, 1) : "";
                        _diarioDetalleDto.NroAsiento = "ASIENTO DEPRECIACIÓN";
                        ListaFinal.Add(_diarioDetalleDto);
                    }

                }





                #endregion
            }
            if (AjusteDepreciacion)
            {

                #region  Genera Libro Diario Ajuste Depreciación

                foreach (var CuentaActivoFijo in ListaAjuste.Where(x => x.CantidadAjuste != 0))
                {
                    int i = 1;

                    if (i == 1)
                    {
                        _diarioDetalleDto = new ReporteAsientoDiario();
                        _diarioDetalleDto.Cuenta = CuentaActivoFijo.Cuenta39;
                        var AsientoContable39 = _objAsientoContableBL.ObtenAsientosPorNro(ref  objOperationResult, CuentaActivoFijo.Cuenta39);
                        if (CuentaActivoFijo.CantidadAjuste > 0)
                        {
                            _diarioDetalleDto.Naturaleza = AsientoContable39 != null ? AsientoContable39.i_Naturaleza == 1 ? "D" : AsientoContable39.i_Naturaleza == 2 ? "H" : "" : "";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(CuentaActivoFijo.CantidadAjuste.Value, 2);
                            _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe / TipoCambio, 2);

                        }
                        else if (CuentaActivoFijo.CantidadAjuste < 0)
                        {
                            _diarioDetalleDto.Naturaleza = AsientoContable39 != null ? AsientoContable39.i_Naturaleza == 1 ? "H" : AsientoContable39.i_Naturaleza == 2 ? "D" : "" : "";

                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(CuentaActivoFijo.CantidadAjuste.Value, 2) * -1;
                            _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe / TipoCambio, 2);
                        }

                        _diarioDetalleDto.Anexo = null;
                        _diarioDetalleDto.Debe = _diarioDetalleDto.Naturaleza == "D" ? _diarioDetalleDto.d_Importe : _diarioDetalleDto.d_Cambio;
                        _diarioDetalleDto.Haber = _diarioDetalleDto.Naturaleza == "H" ? _diarioDetalleDto.d_Importe : _diarioDetalleDto.d_Cambio;
                        _diarioDetalleDto.Documento = CuentaActivoFijo.CodigoActivoFijo;
                        _diarioDetalleDto.FechaAsiento = DateTime.Now.ToShortDateString();
                        _diarioDetalleDto.FechaVencimiento = DateTime.Now.ToShortDateString();
                        _diarioDetalleDto.KeyCuenta3 = _diarioDetalleDto.Cuenta != null ? _diarioDetalleDto.Cuenta.Substring(0, 1) : "";
                        _diarioDetalleDto.GlosaAsiento = "";
                        _diarioDetalleDto.MonedaExtranjera = 0;
                        _diarioDetalleDto.NombreAsiento = "";
                        _diarioDetalleDto.NroAsiento = "ASIENTO AJUSTE";
                        _diarioDetalleDto.CentroCosto = CuentaActivoFijo.CentroCosto;
                        ListaFinal.Add(_diarioDetalleDto);
                        i = i + 1;


                    }
                    if (i == 2)
                    {
                        _diarioDetalleDto = new ReporteAsientoDiario();
                        _diarioDetalleDto.Cuenta = CuentaActivoFijo.CuentaGasto;
                        var AsientoContableGasto = _objAsientoContableBL.ObtenAsientosPorNro(ref  objOperationResult, CuentaActivoFijo.CuentaGasto);
                        if (CuentaActivoFijo.CantidadAjuste > 0)
                        {
                            _diarioDetalleDto.Naturaleza = AsientoContableGasto != null ? AsientoContableGasto.i_Naturaleza == 1 ? "D" : AsientoContableGasto.i_Naturaleza == 2 ? "H" : "" : "";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(CuentaActivoFijo.CantidadAjuste.Value, 2);
                            _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe / TipoCambio, 2);
                        }
                        else if (CuentaActivoFijo.CantidadAjuste < 0)
                        {
                            _diarioDetalleDto.Naturaleza = AsientoContableGasto != null ? AsientoContableGasto.i_Naturaleza == 1 ? "H" : AsientoContableGasto.i_Naturaleza == 2 ? "D" : "" : "";
                            _diarioDetalleDto.d_Importe = Utils.Windows.DevuelveValorRedondeado(CuentaActivoFijo.CantidadAjuste.Value, 2) * -1;
                            _diarioDetalleDto.d_Cambio = Utils.Windows.DevuelveValorRedondeado(_diarioDetalleDto.d_Importe / TipoCambio, 2);
                        }
                        _diarioDetalleDto.Anexo = null;
                        _diarioDetalleDto.Debe = _diarioDetalleDto.Naturaleza == "D" ? _diarioDetalleDto.d_Importe : _diarioDetalleDto.d_Cambio;
                        _diarioDetalleDto.Haber = _diarioDetalleDto.Naturaleza == "H" ? _diarioDetalleDto.d_Importe : _diarioDetalleDto.d_Cambio;
                        _diarioDetalleDto.Documento = CuentaActivoFijo.CodigoActivoFijo;
                        _diarioDetalleDto.FechaAsiento = DateTime.Now.ToShortDateString();
                        _diarioDetalleDto.FechaVencimiento = DateTime.Now.ToShortDateString();
                        _diarioDetalleDto.GlosaAsiento = "";
                        _diarioDetalleDto.KeyCuenta3 = _diarioDetalleDto.Cuenta != null ? _diarioDetalleDto.Cuenta.Substring(0, 1) : "";
                        _diarioDetalleDto.CentroCosto = CuentaActivoFijo.CentroCosto;
                        _diarioDetalleDto.MonedaExtranjera = 0;
                        _diarioDetalleDto.NombreAsiento = "";
                        _diarioDetalleDto.NroAsiento = "ASIENTO AJUSTE";
                        ListaFinal.Add(_diarioDetalleDto);

                    }
                }

                #endregion

            }

            return ListaFinal.OrderBy(o => o.Cuenta).ToList();


        }
        /*
        public List<ReporteValorActualActivosDto> ReporteValorActualActivos(ref OperationResult objOperationResult, int periodo, int mes, string CuentaDesde, string CuentaHasta, string CentroCosto)
        {

            try
            {
                objOperationResult.Success = 1;
                DateTime FechaFinAcumuladoAnterior = new DateTime();
                DateTime FechaHasta = DateTime.Parse(DateTime.DaysInMonth(periodo, mes) + "/" + mes + "/" + periodo + " 23:59");   //Depreciar Hasta
                DateTime FechaPeriodoInicial = DateTime.Parse("01/01/" + periodo);
                List<ReporteValorActualActivosDto> ListaReporteValorActualActivos = new List<ReporteValorActualActivosDto>();
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    ReporteValorActualActivosDto objReporte = new ReporteValorActualActivosDto();
                    string strPeriodo = periodo.ToString();

                    List<activofijoDto> ActivosPeriodoActualTodos = (from a in dbContext.activofijo

                                                                     join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join
                                                                     from b in b_join.DefaultIfEmpty()

                                                                     join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                                     from c in c_join.DefaultIfEmpty()

                                                                     join d in dbContext.asientocontable on new { Cuenta = c.v_Cuenta33, eliminado = 0, p = PeriodoActual } equals new { Cuenta = d.v_NroCuenta, eliminado = d.i_Eliminado.Value, p = d.v_Periodo } into d_join

                                                                     from d in d_join.DefaultIfEmpty()

                                                                     join e in dbContext.datahierarchy on new { Ccosto = a.i_IdCentroCosto.Value, eliminado = 0, Grupo = 31 } equals new { Ccosto = e.i_ItemId, eliminado = e.i_IsDeleted.Value, Grupo = e.i_GroupId } into e_join

                                                                     from e in e_join.DefaultIfEmpty()

                                                                     where a.i_Eliminado == 0 && a.i_Depreciara == 1 && a.t_FechaUso != null && a.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoActual

                                                                     && a.t_FechaOrdenCompra.Value.Year <= periodo && a.i_EsTemporal == 0  //Se agrego porque en el proceso depreciacion no se toma en cuenta

                                                                     && (e.v_Value2 == CentroCosto || CentroCosto == "")
                                                                     select new activofijoDto
                                                                     {
                                                                         v_IdActivoFijo = a.v_IdActivoFijo,
                                                                         d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                                         mesesAdepreciar = b == null ? "0" : b.v_Value1,
                                                                         t_FechaUso = a.t_FechaUso,
                                                                         i_Baja = a.i_Baja == null ? 0 : a.i_Baja,
                                                                         t_FechaBaja = a.t_FechaBaja,
                                                                         v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                                         v_Descricpion = a.v_Descricpion.Trim(),
                                                                         v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                                         t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                                         NumeroCuenta = d == null ? c == null ? "*NO EXISTE CUENTA ASOCIADA*" : c.v_Cuenta33 + "* CUENTA NO EXISTE EN PLAN DE CUENTAS *" : c.v_Cuenta33 + " " + d.v_NombreCuenta.ToUpper(),
                                                                         CodigoCentroCosto = e == null ? "*CENTRO DE COSTO NO EXISTE* " : "CENTRO DE COSTO :" + e.v_Value2 + " " + e.v_Value1,
                                                                         //SoloCuenta = c == null ? "0" : c.v_Cuenta33.Trim(),
                                                                         // CuentaMayor = c == null ? 0 : c.v_Cuenta33.Length > NumeroCuentaDesde ? c.v_Cuenta33.Substring(0, NumeroCuentaDesde).ToString () : 0, 
                                                                     }).ToList();

                    if (!string.IsNullOrEmpty(CuentaDesde) && !string.IsNullOrEmpty(CuentaHasta))
                    {
                        var rangoCtas = Utils.Windows.RangoDeCuentas(CuentaDesde, CuentaHasta);
                        ActivosPeriodoActualTodos = ActivosPeriodoActualTodos.Where(p => rangoCtas.Contains(p.SoloCuenta)).ToList();
                    }

                    List<activofijoDto> ActivosPeriodoAnteriorTodos = (from a in dbContext.activofijo

                                                                       join b in dbContext.datahierarchy on new { Grupo = 109, Id = a.i_IdMesesDepreciara.Value, eliminado = 0 } equals new { Grupo = b.i_GroupId, Id = b.i_ItemId, eliminado = b.i_IsDeleted.Value } into b_join

                                                                       from b in b_join.DefaultIfEmpty()

                                                                       join c in dbContext.cuentasinventarios on new { TipoActivo = a.i_IdTipoActivo, eliminado = 0 } equals new { TipoActivo = c.i_IdTipoActivo, eliminado = c.i_Eliminado.Value } into c_join

                                                                       from c in c_join.DefaultIfEmpty()

                                                                       join d in dbContext.asientocontable on new { Cuenta = c.v_Cuenta33, eliminado = 0, p = PeriodoActual } equals new { Cuenta = d.v_NroCuenta, eliminado = d.i_Eliminado.Value, p = d.v_Periodo } into d_join

                                                                       from d in d_join.DefaultIfEmpty()

                                                                       join e in dbContext.datahierarchy on new { Ccosto = a.i_IdCentroCosto.Value, eliminado = 0, Grupo = 31 } equals new { Ccosto = e.i_ItemId, eliminado = e.i_IsDeleted.Value, Grupo = e.i_GroupId } into e_join

                                                                       from e in e_join.DefaultIfEmpty()


                                                                       where a.i_Eliminado == 0 && a.i_Depreciara == 1 && a.t_FechaUso != null && a.i_IdTipoMotivo == (int)MotivoActivoFijo.PeriodoAnterior

                                                                       && a.v_Periodo == strPeriodo && a.i_EsTemporal == 0
                                                                       && (e.v_Value2 == CentroCosto || CentroCosto == "")
                                                                       select new activofijoDto
                                                                       {

                                                                           v_IdActivoFijo = a.v_IdActivoFijo,
                                                                           d_MonedaNacional = a.d_MonedaNacional ?? 0,
                                                                           mesesAdepreciar = b.v_Value1,
                                                                           t_FechaUso = a.t_FechaUso,
                                                                           i_Baja = a.i_Baja,
                                                                           t_FechaBaja = a.t_FechaBaja,
                                                                           v_CodigoActivoFijo = a.v_CodigoActivoFijo.Trim(),
                                                                           v_Descricpion = a.v_Descricpion.Trim(),
                                                                           v_NumeroFactura = a.v_NumeroFactura.Trim(),
                                                                           t_FechaOrdenCompra = a.t_FechaOrdenCompra,
                                                                           NumeroCuenta = d == null ? c == null ? "*No Existe Cuenta Asociada*" : c.v_Cuenta33 + "*Cuenta no Existe Plan Cuentas* " : c.v_Cuenta33 + " " + d.v_NombreCuenta,
                                                                           CodigoCentroCosto = e == null ? "*CENTRO DE COSTO NO EXISTE* " : "CENTRO DE COSTO :" + e.v_Value2 + " " + e.v_Value1,
                                                                           SoloCuenta = c == null ? "0" : c.v_Cuenta33.Trim(),
                                                                           // CuentaMayor = c == null ? "0" : c.v_Cuenta33.Length > 2 ? c.v_Cuenta33.Substring(0, 2) : "0", 
                                                                       }).ToList();

                    if (!string.IsNullOrEmpty(CuentaDesde) && !string.IsNullOrEmpty(CuentaHasta))
                    {
                        var rangoCtas = Utils.Windows.RangoDeCuentas(CuentaDesde, CuentaHasta);
                        ActivosPeriodoAnteriorTodos = ActivosPeriodoAnteriorTodos.Where(p => rangoCtas.Contains(p.SoloCuenta)).ToList();
                    }

                    var ActivosTotal = ActivosPeriodoActualTodos.Concat(ActivosPeriodoAnteriorTodos);
                    foreach (var item in ActivosTotal.AsParallel())
                    {

                        DateTime FechaDesde = item.t_FechaUso.Value.Date;
                        decimal valoradquisicion = item.d_MonedaNacional.Value;
                        int mesesAdepreciar = int.Parse(item.mesesAdepreciar);
                        int anioInicio = int.Parse(FechaHasta.Date.Year.ToString()); // future .... Ultimo dia de la fecha del reporte 31/08/2015
                        objReporte = new ReporteValorActualActivosDto();
                        objReporte.CodigoDescripcionActivo = item.v_CodigoActivoFijo.Trim().ToUpper() + "     " + item.v_Descricpion.Trim().ToUpper();
                        objReporte.Cuenta = item.NumeroCuenta;
                        objReporte.CentroCosto = string.IsNullOrEmpty(CentroCosto) ? "" : item.CodigoCentroCosto.ToUpper();
                        objReporte.FechaCompra = item.t_FechaOrdenCompra;
                        objReporte.MesesDepreciar = mesesAdepreciar;
                        objReporte.ValorCompra = valoradquisicion;
                        objReporte.FechaUso = item.t_FechaUso;
                        objReporte.FechaBaja = item.t_FechaBaja;
                        DateTime FechaInicio = new DateTime();
                        if (FechaDesde.Date.Month == 12) // Se empieza a depreciar un mes después
                        {
                            FechaInicio = DateTime.Parse("01/01/" + (FechaDesde.Date.Year + 1).ToString());
                        }
                        else
                        {
                            FechaInicio = DateTime.Parse("01/" + (int.Parse(FechaDesde.Date.Month.ToString()) + 1).ToString() + "/" + FechaDesde.Date.Year.ToString());
                        }

                        FechaFinAcumuladoAnterior = DateTime.Parse("31/12/" + (anioInicio - 1).ToString());
                        DateTime AnioAcabaraDepreciar = FechaDesde.AddMonths(mesesAdepreciar);
                        DateTime FechaFinReporte = DateTime.Parse(DateTime.DaysInMonth(anioInicio, mes).ToString() + "/" + mes.ToString() + "/" + anioInicio.ToString() + " 23:59"); // Fecha Fin Hasta que dia se hará la depreciacion ( Usualmente es el mes del Reporte )
                        decimal MensualHist = Math.Round(objReporte.ValorCompra.Value / objReporte.MesesDepreciar, 2);

                        int MesesActual = int.Parse((Math.Round((FechaFinReporte.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));

                        int MesesAnioAnterior = int.Parse((Math.Round((FechaFinAcumuladoAnterior.Subtract(FechaInicio).Days / (365.25 / 12))).ToString()));
                        #region ActivoSinBaja
                        if (item.i_Baja == 0)
                        {
                            objReporte.MesesDepreciados = MesesActual < 0 ? 0 : MesesActual;

                            if (objReporte.MesesDepreciados <= mesesAdepreciar)
                            {
                                objReporte.MensualHistorica = objReporte.MesesDepreciados.Value == 0 ? 0 : mesesAdepreciar == 0 ? 0 : MensualHist;
                                if (FechaDesde.Date.Year == FechaHasta.Date.Year)
                                {
                                    objReporte.AcumuladoHistoricoAnio = objReporte.MesesDepreciados * objReporte.MensualHistorica;
                                }
                                else
                                {
                                    objReporte.AcumuladoHistoricoAnio = mes * objReporte.MensualHistorica;
                                }
                                objReporte.AcumuladoAnterior = MesesAnioAnterior <= 0 ? 0 : MesesAnioAnterior * objReporte.MensualHistorica.Value;
                                objReporte.AcumuladoTotal = objReporte.AcumuladoAnterior + objReporte.AcumuladoHistoricoAnio;
                                objReporte.ValorNetoActual = objReporte.ValorCompra - objReporte.AcumuladoTotal;
                            }
                            else
                            {
                                objReporte.MensualHistorica = 0;
                                if (AnioAcabaraDepreciar.Date.Year == FechaHasta.Date.Year)    // Se hizo para que estos datos ,sólo aparezca cuando se cumplen los  meses depreciación
                                {
                                    //int calculomesesTranscurridosAnioAnt = int.Parse(((FechaFinAcumuladoAnterior.Subtract(FechaInicio).Days) / (365.25 / 12)).ToString());
                                    //double jj = FechaFinAcumuladoAnterior.Subtract(FechaInicio).Days;
                                    //double ff = (FechaFinAcumuladoAnterior.Subtract(FechaInicio).Days) / (365.25 / 12);

                                    //decimal xxx = (decimal)((FechaFinAcumuladoAnterior.Subtract(FechaInicio).Days) / (365.25 / 12));
                                    //var jjj = Utils.Windows.DevuelveValorRedondeado((decimal)((FechaFinAcumuladoAnterior.Subtract(FechaInicio).Days) / (365.25 / 12)), 0);
                                    //int calculomesesTranscurridosAnioAnt = (int)jjj;
                                    int calculomesesTranscurridosAnioAnt = (int)Utils.Windows.DevuelveValorRedondeado((decimal)((FechaFinAcumuladoAnterior.Subtract(FechaInicio).Days) / (365.25 / 12)), 0);

                                    int mesesRest = int.Parse((mesesAdepreciar - calculomesesTranscurridosAnioAnt).ToString());
                                    objReporte.AcumuladoHistoricoAnio = int.Parse(mesesRest.ToString()) * MensualHist;
                                    objReporte.AcumuladoAnterior = MesesAnioAnterior < 0 ? 0 : MesesAnioAnterior * MensualHist;
                                }
                                else
                                {

                                    objReporte.AcumuladoHistoricoAnio = 0;
                                    objReporte.AcumuladoAnterior = objReporte.ValorCompra;
                                }
                                objReporte.AcumuladoTotal = objReporte.ValorCompra;
                                objReporte.ValorNetoActual = 0;
                                objReporte.Comparacion = objReporte.MensualHistorica * (objReporte.MesesDepreciar - objReporte.MesesDepreciados);
                                objReporte.AjusteDepreciacion = objReporte.Comparacion - objReporte.ValorNetoActual;

                            }

                            objReporte.Cuenta = item.NumeroCuenta;
                            //objReporte.CentroCosto = item.CodigoCentroCosto;
                            objReporte.CentroCosto = string.IsNullOrEmpty(CentroCosto) ? "" : item.CodigoCentroCosto.ToUpper();
                            objReporte.Comparacion = objReporte.MensualHistorica * (objReporte.MesesDepreciar - objReporte.MesesDepreciados);
                            objReporte.AjusteDepreciacion = objReporte.Comparacion - objReporte.ValorNetoActual;
                            if (objReporte.MesesDepreciados > 0)
                            {
                                ListaReporteValorActualActivos.Add(objReporte);
                            }

                        }
                        #endregion
                        else
                        #region ActivoBaja
                        {
                            //FechaDesde ... FechaUso 
                            if (anioInicio < int.Parse(item.t_FechaBaja.Value.Year.ToString())) // No se toma en cuenta el mes de Baja ,AnioInicio --> Año del Reporte
                            {
                                objReporte.MesesDepreciados = MesesActual < 0 ? 0 : MesesActual;
                                if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                {
                                    objReporte.MensualHistorica = objReporte.MesesDepreciados.Value == 0 ? 0 : mesesAdepreciar == 0 ? 0 : MensualHist;
                                    if (FechaDesde.Date.Year == FechaHasta.Date.Year)
                                    {
                                        objReporte.AcumuladoHistoricoAnio = objReporte.MesesDepreciados * objReporte.MensualHistorica;
                                    }
                                    else
                                    {
                                        objReporte.AcumuladoHistoricoAnio = mes * objReporte.MensualHistorica;
                                    }

                                    //objReporte.AcumuladoHistoricoAnio = objReporte.MesesDepreciados * objReporte.MensualHistorica;
                                    objReporte.AcumuladoAnterior = MesesAnioAnterior < 0 ? 0 : MesesAnioAnterior * objReporte.MensualHistorica;
                                    objReporte.AcumuladoTotal = objReporte.AcumuladoAnterior + objReporte.AcumuladoHistoricoAnio;
                                    objReporte.ValorNetoActual = objReporte.ValorCompra - objReporte.AcumuladoTotal;
                                }
                                else
                                {
                                    objReporte.MensualHistorica = 0;

                                    if (AnioAcabaraDepreciar.Date.Year == FechaHasta.Date.Year)    // Se hizo para que estos datos ,sólo aparezca cuando se cumplen los  meses depreciación
                                    {
                                        //var calculomesesTranscurridosAnioAnt = (FechaFinAcumuladoAnterior.Subtract(FechaInicio).Days) / (365.25 / 12);
                                        int calculomesesTranscurridosAnioAnt = (int)Utils.Windows.DevuelveValorRedondeado((decimal)((FechaFinAcumuladoAnterior.Subtract(FechaInicio).Days) / (365.25 / 12)), 0);
                                        var mesesRest = mesesAdepreciar - calculomesesTranscurridosAnioAnt;
                                        objReporte.AcumuladoHistoricoAnio = int.Parse(mesesRest.ToString()) * MensualHist;
                                        objReporte.AcumuladoAnterior = MesesAnioAnterior < 0 ? 0 : MesesAnioAnterior * MensualHist;
                                    }
                                    else
                                    {
                                        objReporte.AcumuladoHistoricoAnio = 0;
                                        objReporte.AcumuladoAnterior = objReporte.ValorCompra;
                                    }
                                    objReporte.AcumuladoTotal = objReporte.ValorCompra;
                                    objReporte.ValorNetoActual = 0;
                                }

                            }
                            else
                            {
                                //Fecha Inicio ..Fecha Uso
                                if (mes <= int.Parse(item.t_FechaBaja.Value.Month.ToString()))
                                {
                                    objReporte.MesesDepreciados = MesesActual < 0 ? 0 : MesesActual;

                                    if (objReporte.MesesDepreciados <= mesesAdepreciar)
                                    {
                                        objReporte.MensualHistorica = objReporte.MesesDepreciados.Value == 0 ? 0 : mesesAdepreciar == 0 ? 0 : MensualHist;

                                        if (item.t_FechaUso <= FechaPeriodoInicial)
                                        {
                                            var mesesDepreciadosDesdeInicioPeriodo = Math.Round(decimal.Parse((FechaHasta.Subtract(FechaPeriodoInicial).Days / (365.25 / 12)).ToString()));
                                            objReporte.AcumuladoHistoricoAnio = int.Parse(mesesDepreciadosDesdeInicioPeriodo.ToString()) * MensualHist;
                                        }
                                        else
                                        {

                                            objReporte.AcumuladoHistoricoAnio = objReporte.MesesDepreciados * MensualHist;
                                        }

                                        //decimal  x= Math.Round(decimal.Parse(calculomesesTranscurridosAnioAnterior.ToString());
                                        objReporte.AcumuladoAnterior = MesesAnioAnterior < 0 ? 0 : MesesAnioAnterior * objReporte.MensualHistorica;
                                        objReporte.AcumuladoTotal = objReporte.AcumuladoAnterior + objReporte.AcumuladoHistoricoAnio;
                                        objReporte.ValorNetoActual = objReporte.ValorCompra - objReporte.AcumuladoTotal;

                                    }
                                    else
                                    {
                                        objReporte.MensualHistorica = 0;
                                        if (AnioAcabaraDepreciar.Date.Year == FechaHasta.Date.Year)    // Se hizo para que estos datos ,sólo aparezca cuando se cumplen los  meses depreciación
                                        {
                                            //var calculomesesTranscurridosAnioAnt = (FechaFinAcumuladoAnterior.Subtract(FechaInicio).Days) / (365.25 / 12);
                                            int calculomesesTranscurridosAnioAnt = (int)Utils.Windows.DevuelveValorRedondeado((decimal)((FechaFinAcumuladoAnterior.Subtract(FechaInicio).Days) / (365.25 / 12)), 0);
                                            var mesesRest = mesesAdepreciar - calculomesesTranscurridosAnioAnt;
                                            objReporte.AcumuladoHistoricoAnio = int.Parse(mesesRest.ToString()) * MensualHist;
                                            objReporte.AcumuladoAnterior = MesesAnioAnterior < 0 ? 0 : MesesAnioAnterior * MensualHist;
                                        }
                                        else
                                        {
                                            objReporte.AcumuladoHistoricoAnio = 0;
                                            objReporte.AcumuladoAnterior = objReporte.ValorCompra;
                                        }
                                        objReporte.AcumuladoTotal = objReporte.ValorCompra;
                                        objReporte.ValorNetoActual = 0;
                                        objReporte.Cuenta = item.NumeroCuenta;
                                        objReporte.CentroCosto = item.CodigoCentroCosto;
                                        ListaReporteValorActualActivos.Add(objReporte);
                                    }
                                    objReporte.Comparacion = objReporte.MensualHistorica * (objReporte.MesesDepreciar - objReporte.MesesDepreciados);
                                    objReporte.AjusteDepreciacion = objReporte.Comparacion - objReporte.ValorNetoActual;
                                }
                                else
                                {
                                }
                                if (objReporte.MesesDepreciados > 0)
                                {
                                    ListaReporteValorActualActivos.Add(objReporte);
                                }
                            }
                        }
                        #endregion
                    }

                }
                return ListaReporteValorActualActivos;
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;


                objOperationResult.AdditionalInformation = "ActivoFijoBL.ReporteValorActualActivos.\n Linea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }
        */

        public List<ReporteLibroActivoDto> AcumuladoPeriodoAnterior(ref OperationResult objOperationResult, int PeridoAnterior, int CodigoDesde, int CodigoHasta, bool UsoOtrosReportes, string Filtro)
        {

            try
            {
                objOperationResult.Success = 1;
                List<ReporteLibroActivoDto> ListaReporteCierreAnterior = ReporteAcumuladoPeriodoAnterior(ref objOperationResult, PeridoAnterior, 12, CodigoDesde, CodigoHasta, UsoOtrosReportes, Filtro);
                if (objOperationResult.Success == 0)
                {
                    return null;
                }
                List<ReporteLibroActivoDto> ActivosCierreAnterior = ListaReporteCierreAnterior.GroupBy(o => new { o.v_IdActivoFijo }).Select(d =>
                {
                    var k = d.LastOrDefault();
                    k.d_ImportePeriodoDepreciacion = d.Sum(h => h.d_ImporteMensualDepreciacion.Value);
                    return k;
                }).ToList();

                return ActivosCierreAnterior;
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                return null;
            }


        }



        public List<ReporteValorActualActivosDto> ReporteValorActualActivos_(ref OperationResult objOperationResult, int PeriodoReporte, int MesReporte, string CuentaDesde, string CuentaHasta, string CentroCosto)
        {

            try
            {
                objOperationResult.Success = 1;

                SaldoEstadoBancarioBL _objSaldoEstadoBancarioBL = new SaldoEstadoBancarioBL();
                List<ReporteValorActualActivosDto> ListaFinal = new List<ReporteValorActualActivosDto>();
                ReporteValorActualActivosDto objReporte = new ReporteValorActualActivosDto();



                var ListaReporteActivo = ReporteAcumuladoPeriodoAnterior(ref objOperationResult, PeriodoReporte, MesReporte, 0, 0, true, null);
                var ActivosActuales = ListaReporteActivo.GroupBy(l => new { l.v_IdActivoFijo }).Select(d =>
                {
                    var k = d.LastOrDefault();
                    k.d_ImportePeriodoDepreciacion = d.Sum(h => h.d_ImporteMensualDepreciacion.Value);
                    return k;
                }).ToList();




                if (!string.IsNullOrEmpty(CuentaDesde) && !string.IsNullOrEmpty(CuentaHasta))
                {
                    var rangoCtas = Utils.Windows.RangoDeCuentas(CuentaDesde, CuentaHasta);
                    ActivosActuales = ActivosActuales.Where(p => rangoCtas.Contains(p.Cuenta33)).ToList();
                }
                List<ReporteLibroActivoDto> ActivosCierreAnterior = new List<ReporteLibroActivoDto>();

                //List<ReporteLibroActivoDto> ListaReporteCierreAnterior = ReporteLibroActivo(ref objOperationResult, PeriodoReporte - 1, 12, 0, 0, true, null);
                //List<ReporteLibroActivoDto> ActivosCierreAnterior = ListaReporteCierreAnterior.GroupBy(o => new { o.v_IdActivoFijo }).Select(d =>
                //{
                //    var k = d.LastOrDefault();
                //    k.d_ImportePeriodoDepreciacion = d.Sum(h => h.d_ImporteMensualDepreciacion.Value);
                //    return k;
                //}).ToList();
                ActivosCierreAnterior = AcumuladoPeriodoAnterior(ref objOperationResult, (PeriodoReporte - 1), 0, 0, true, null);
                if (objOperationResult.Success == 0)
                {
                    return null;
                }
                if (!string.IsNullOrEmpty(CuentaDesde) && !string.IsNullOrEmpty(CuentaHasta))
                {
                    var rangoCtas = Utils.Windows.RangoDeCuentas(CuentaDesde, CuentaHasta);
                    ActivosCierreAnterior = ActivosCierreAnterior.Where(p => rangoCtas.Contains(p.Cuenta33)).ToList();
                }



                if (objOperationResult.Success == 0) return null;
                foreach (var item in ActivosActuales)
                {
                    objReporte = new ReporteValorActualActivosDto();


                    var AcumAnterior = ActivosCierreAnterior.Where(o => o.v_IdActivoFijo == item.v_IdActivoFijo).FirstOrDefault();
                    objReporte.CodigoActivoFijo = item.CodigoActivoFijo;
                    objReporte.DescripcionActivoFijo = item.SoloDescripionActivo;


                    objReporte.ValorHistorico = PeriodoReporte > int.Parse(item.AnioCompra) ? item.ValorAdquisicionHistorico.Value : 0;
                    objReporte.ValorActualizado = item.ValorAdquisicionHistorico.Value;
                    objReporte.FechaCompra = item.FechaCompra;

                    objReporte.FechaUso = item.FechaUso;
                    objReporte.sFechaUso = item.sFechaUso;
                    objReporte.MesesDepreciar = int.Parse(item.MesesDepreciar);
                    objReporte.ValorCompra = 0;
                    if (PeriodoReporte.ToString() == item.AnioCompra && MesReporte >= int.Parse(item.MesCompra))
                        objReporte.ValorCompra = item.ValorAdquisicionHistorico.Value;

                    objReporte.MesesDepreciados = item.MesesDepreciados;
                    objReporte.MensualHistorica = item.d_ImporteMensualDepreciacion;
                    objReporte.AcumuladoAnterior = AcumAnterior != null ? AcumAnterior.d_AcumuladoHistorico ?? 0 : 0;
                    objReporte.AcumuladoHistoricoAnio = item.d_ImportePeriodoDepreciacion;
                    objReporte.AcumuladoTotal = item.d_AcumuladoHistorico ?? 0;
                    objReporte.AjusteDepreciacion = item.d_AjusteDepreciacion;
                    objReporte.ValorNetoActual = item.d_ValorNetoActual;
                    objReporte.Cuenta = item.Cuenta33 + " " + _objSaldoEstadoBancarioBL.ObtenerNombreCuenta(item.Cuenta33);
                    objReporte.CentroCosto = string.IsNullOrEmpty(CentroCosto) ? "" : item.CentroCosto.ToUpper();
                    if (item.i_Depreciara == 1)
                    {

                        ListaFinal.Add(objReporte);
                    }
                    else if (item.i_Depreciara == 0)
                    {
                        if (PeriodoReporte == int.Parse(item.AnioCompra) && int.Parse(MesReporte.ToString()) >= int.Parse(item.MesCompra))
                        {
                            ListaFinal.Add(objReporte);
                        }
                        else if (PeriodoReporte > int.Parse(item.AnioCompra))
                        {
                            ListaFinal.Add(objReporte);
                        }
                    }
                }


                return ListaFinal.OrderBy(o => o.Cuenta).ThenBy(o => o.CodigoActivoFijo).ToList();
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;


                objOperationResult.AdditionalInformation = "ActivoFijoBL.ReporteValorActualActivos.\n Linea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }




        public List<ReporteValorActualActivosDto> ReporteValorActualActivos(ref OperationResult objOperationResult, int PeriodoReporte, int MesReporte, string CuentaDesde, string CuentaHasta, string CentroCosto)
        {

            try
            {
                objOperationResult.Success = 1;

                SaldoEstadoBancarioBL _objSaldoEstadoBancarioBL = new SaldoEstadoBancarioBL();
                List<ReporteValorActualActivosDto> ListaFinal = new List<ReporteValorActualActivosDto>();
                ReporteValorActualActivosDto objReporte = new ReporteValorActualActivosDto();



                var ListaReporteActivo = ReporteLibroActivo(ref objOperationResult, PeriodoReporte, MesReporte, 0, 0, true, null);
                var ActivosActuales = ListaReporteActivo.GroupBy(l => new { l.v_IdActivoFijo }).Select(d =>
                {
                    var k = d.LastOrDefault();
                    k.d_ImportePeriodoDepreciacion = d.Sum(h => h.d_ImporteMensualDepreciacion.Value);
                    return k;
                }).ToList();




                if (!string.IsNullOrEmpty(CuentaDesde) && !string.IsNullOrEmpty(CuentaHasta))
                {
                    var rangoCtas = Utils.Windows.RangoDeCuentas(CuentaDesde, CuentaHasta);
                    ActivosActuales = ActivosActuales.Where(p => rangoCtas.Contains(p.Cuenta33)).ToList();
                }
                //List<ReporteLibroActivoDto> ActivosCierreAnterior = new List<ReporteLibroActivoDto>();


                //ActivosCierreAnterior = AcumuladoPeriodoAnterior(ref objOperationResult, (PeriodoReporte - 1), 0, 0, true, null);
                //if (objOperationResult.Success == 0)
                //{
                //    return null;
                //}
                //if (!string.IsNullOrEmpty(CuentaDesde) && !string.IsNullOrEmpty(CuentaHasta))
                //{
                //    var rangoCtas = Utils.Windows.RangoDeCuentas(CuentaDesde, CuentaHasta);
                //    ActivosCierreAnterior = ActivosCierreAnterior.Where(p => rangoCtas.Contains(p.Cuenta33)).ToList();
                //}



                if (objOperationResult.Success == 0) return null;
                ActivosActuales = ActivosActuales.OrderBy(o => o.CodigoActivoFijo).ToList();
                foreach (var item in ActivosActuales)
                {
                    objReporte = new ReporteValorActualActivosDto();


                    //var AcumAnterior = ActivosCierreAnterior.Where(o => o.v_IdActivoFijo == item.v_IdActivoFijo).FirstOrDefault();
                    objReporte.CodigoActivoFijo = item.CodigoActivoFijo;
                    objReporte.DescripcionActivoFijo = item.SoloDescripionActivo;


                    objReporte.ValorHistorico = PeriodoReporte > int.Parse(item.AnioCompra) ? item.ValorAdquisicionHistorico.Value : 0;
                    objReporte.ValorActualizado = item.ValorAdquisicionHistorico.Value;
                    objReporte.FechaCompra = item.FechaCompra;

                    objReporte.FechaUso = item.FechaUso;
                    objReporte.sFechaUso = item.sFechaUso;
                    objReporte.MesesDepreciar = int.Parse(item.MesesDepreciar);
                    objReporte.ValorCompra = 0;
                    if (PeriodoReporte.ToString() == item.AnioCompra && MesReporte >= int.Parse(item.MesCompra))
                        objReporte.ValorCompra = item.ValorAdquisicionHistorico.Value;

                    objReporte.MesesDepreciados = item.MesesDepreciados;
                    objReporte.MensualHistorica = item.d_ImporteMensualDepreciacion;
                    objReporte.AcumuladoAnterior = item.AcumuladoAnterior;       //AcumAnterior != null ? AcumAnterior.d_AcumuladoHistorico ?? 0 : 0;
                    objReporte.AcumuladoHistoricoAnio = item.d_ImportePeriodoDepreciacion;
                    objReporte.AcumuladoTotal = item.d_AcumuladoTotal ?? 0;
                    objReporte.AjusteDepreciacion = item.d_AjusteDepreciacion;
                    objReporte.ValorNetoActual = item.d_ValorNetoActual;
                    objReporte.Cuenta = item.Cuenta33 + " " + _objSaldoEstadoBancarioBL.ObtenerNombreCuenta(item.Cuenta33);
                    objReporte.CentroCosto = string.IsNullOrEmpty(CentroCosto) ? "" : item.CentroCosto.ToUpper();
                    if (item.i_Depreciara == 1)
                    {

                        ListaFinal.Add(objReporte);
                    }
                    else if (item.i_Depreciara == 0)
                    {
                        if (PeriodoReporte == int.Parse(item.AnioCompra) && int.Parse(MesReporte.ToString()) >= int.Parse(item.MesCompra))
                        {
                            ListaFinal.Add(objReporte);
                        }
                        else if (PeriodoReporte > int.Parse(item.AnioCompra))
                        {
                            ListaFinal.Add(objReporte);
                        }
                    }
                }


                return ListaFinal.OrderBy(o => o.Cuenta).ThenBy(o => o.CodigoActivoFijo).ToList();
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;


                objOperationResult.AdditionalInformation = "ActivoFijoBL.ReporteValorActualActivos.\n Linea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }













        public List<ReporteResumenActivoFijoDto> ReporteResumenActivoFijo(ref OperationResult objOperationResult, int Periodo, int mes, bool Detallado)
        {


            //try
            //{
            //    objOperationResult.Success = 1;
            DateTime FechaReporte = DateTime.Parse(DateTime.DaysInMonth(Periodo, mes).ToString() + "/" + mes + "/" + Periodo + " 23:59");
            ReporteResumenActivoFijoDto objReporte = new ReporteResumenActivoFijoDto();
            List<ReporteResumenActivoFijoDto> ListaFinal = new List<ReporteResumenActivoFijoDto>();
            DateTime AnioActual = DateTime.Parse("01/01/" + Periodo.ToString());
            DateTime FechaAnterior = DateTime.Parse("31/12/" + (Periodo - 1).ToString());

            DateTime FechaInicio = DateTime.Parse("01/01/" + Periodo.ToString());
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var ActivosTodosPeriodos = new ActivoFijoBL().ReporteLibroActivo(ref objOperationResult, Periodo, mes, 0, 0, true, null);
                var ActivosActuales = ActivosTodosPeriodos.Where(o => o.FechaUso < AnioActual).ToList().GroupBy(l => new { l.v_IdActivoFijo }).Select(d =>
                {
                    var k = d.FirstOrDefault();
                    k.d_AcumuladoHistorico = d.Sum(h => h.d_AcumuladoHistorico);
                    k.d_AcumuladoTotal = d.LastOrDefault() != null ? d.LastOrDefault().d_AcumuladoTotal ?? 0 : d.FirstOrDefault().d_AcumuladoTotal ?? 0;
                    return k;
                }).ToList();

                var CompradosEsteAnio = ActivosTodosPeriodos.Select(o => (ReporteLibroActivoDto)o.Clone()).ToList();
                CompradosEsteAnio = CompradosEsteAnio.GroupBy(o => o.v_IdActivoFijo).Select(o => o.LastOrDefault()).ToList();

                var CompradosPeriodoyDadosAlta = (from a in CompradosEsteAnio

                                                  where a.AnioCompra == Periodo.ToString() && a.AnioUso == Periodo.ToString() && a.i_Baja == 0

                                                  select a).ToList();


                var CompradosPeriodoyDadosBaja = (from a in ActivosActuales
                                                  where a.AnioBaja == Periodo.ToString() && a.i_Baja == 1 && a.FechaBaja.Value.Month <= mes
                                                  select a).ToList();

                var Cuentas = ActivosActuales.Concat(CompradosPeriodoyDadosAlta).Concat(CompradosPeriodoyDadosBaja).Select(x => x.Cuenta33).Distinct().ToList();

                if (Detallado)
                {

                    var Totales = ActivosActuales.Concat(CompradosPeriodoyDadosAlta).Concat(CompradosPeriodoyDadosBaja).ToList().Select(o => new { cuenta = o.Cuenta33, IdActivoFijo = o.v_IdActivoFijo, codigoActivo = o.CodigoActivoFijo }).Distinct().ToList();


                    foreach (var item in Totales)
                    {//39143

                        objReporte = new ReporteResumenActivoFijoDto();
                        objReporte.KeyCuenta = item.cuenta == "0" ? "NO EXISTECUENTA NO ASOCIADA" : item.cuenta + "    " + NombreCuenta(item.cuenta);
                        objReporte.CodigoActivoFijo = item.codigoActivo;
                        if (objReporte.CodigoActivoFijo == "10000064")
                        {
                            string h = "";
                        }

                        objReporte.CuentaDescripcion = item.cuenta == "0" ? "NO EXISTECUENTA NO ASOCIADA" : item + "    " + NombreCuenta(item.cuenta);
                        var SaldoAl = ActivosActuales.Where(x => x.Cuenta33 == item.cuenta && x.v_IdActivoFijo == item.IdActivoFijo && x.FechaCompra <= FechaAnterior).LastOrDefault();
                        objReporte.SaldoAl = SaldoAl != null ? SaldoAl.ValorAdquisicionHistoricoFinal.Value : 0;// ActivosTodosPeriodos.Where(x => x.Cuenta33 == item.cuenta && x.v_IdActivoFijo == item.IdActivoFijo && x.FechaCompra <= FechaAnterior).LastOrDefault().ValorAdquisicionHistorico.Value;//Todos los precios de Compra
                        objReporte.Adiciones = CompradosPeriodoyDadosAlta.Where(x => x.Cuenta33 == item.cuenta && x.v_IdActivoFijo == item.IdActivoFijo && x.FechaCompra >= FechaInicio && x.FechaCompra <= FechaReporte).Sum(x => x.ValorAdquisicionHistorico).Value;
                        var Ret = CompradosPeriodoyDadosBaja.Where(x => x.Cuenta33 == item.cuenta && x.v_IdActivoFijo == item.IdActivoFijo && x.i_Baja == 1 && x.FechaBaja >= FechaInicio && x.FechaBaja <= FechaReporte).LastOrDefault();
                        objReporte.Retiros = Ret != null ? Ret.ValorAdquisicionHistorico.Value : 0;
                        objReporte.Reclasificaciones = 0;
                        objReporte.SaldoAlMesAnio = (objReporte.SaldoAl + objReporte.Adiciones) - objReporte.Retiros;

                        var TodosActivos = ActivosTodosPeriodos.GroupBy(o => o.v_IdActivoFijo).Select(a => a.LastOrDefault()).ToList();
                        var Dep = TodosActivos.Where(x => x.Cuenta33 == item.cuenta && x.v_IdActivoFijo == item.IdActivoFijo).ToList();
                        objReporte.Depreciacion = Dep.Any() ? Dep.Sum(o => o.d_AcumuladoTotal ?? 0) : 0;
                        ListaFinal.Add(objReporte);

                    }

                }
                else
                {

                    foreach (var item in Cuentas)
                    {//39143


                        objReporte = new ReporteResumenActivoFijoDto();
                        objReporte.CuentaDescripcion = item == "0" ? "NO EXISTECUENTA NO ASOCIADA" : item + "    " + NombreCuenta(item);

                        if (item == "39143")
                        {
                            string h = "";
                        }


                        objReporte.CuentaDescripcion = item == "0" ? "NO EXISTECUENTA NO ASOCIADA" : item + "    " + NombreCuenta(item);
                        objReporte.SaldoAl = ActivosActuales.Where(x => x.Cuenta33 == item && x.FechaCompra <= FechaAnterior).Sum(x => x.ValorAdquisicionHistoricoFinal).Value;//Todos los precios de Compra
                        objReporte.Adiciones = CompradosPeriodoyDadosAlta.Where(x => x.Cuenta33 == item && x.FechaCompra >= FechaInicio && x.FechaCompra <= FechaReporte).Sum(x => x.ValorAdquisicionHistorico).Value;
                        objReporte.Retiros = CompradosPeriodoyDadosBaja.Where(x => x.Cuenta33 == item && x.FechaBaja >= FechaInicio && x.i_Baja == 1 && x.FechaBaja <= FechaReporte).Sum(x => x.ValorAdquisicionHistorico).Value;
                        objReporte.Reclasificaciones = 0;
                        objReporte.SaldoAlMesAnio = (objReporte.SaldoAl + objReporte.Adiciones) - objReporte.Retiros;

                        var TodosActivos = ActivosTodosPeriodos.GroupBy(o => o.v_IdActivoFijo).Select(a => a.LastOrDefault()).ToList();
                        objReporte.Depreciacion = TodosActivos.Where(x => x.Cuenta33 == item).Sum(x => x.d_AcumuladoTotal ?? 0);

                        ListaFinal.Add(objReporte);

                    }
                }

            }

            if (Detallado)
            {
                return ListaFinal.OrderBy(o => o.KeyCuenta).ToList();
            }
            else
            {
                return ListaFinal.OrderBy(o => o.CuentaDescripcion).ToList();
            }
        }

        public string NombreCuenta(string pstrNumeroCuenta)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var nombreCuentaMayor = (from n in dbContext.asientocontable

                                         where n.i_Eliminado == 0 && n.v_NroCuenta == pstrNumeroCuenta && n.v_Periodo == PeriodoActual

                                         select new
                                         {
                                             nombreCuenta = n.v_NombreCuenta
                                         }).FirstOrDefault();
                return nombreCuentaMayor != null ? nombreCuentaMayor.nombreCuenta.Trim() : "*Cuenta No Existe Plan Cuentas*";
            }
        }




        public List<ReporteResumenDepreciacionDto> ReporteResumenDepreciacion(ref OperationResult objOperationResult, int Periodo, int mes, bool Detallado)
        {
            //try
            //{
            //    objOperationResult.Success = 1;
            DateTime FechaReporte = DateTime.Parse(DateTime.DaysInMonth(Periodo, mes).ToString() + "/" + mes + "/" + Periodo + " 23:59");
            ReporteResumenDepreciacionDto objReporte = new ReporteResumenDepreciacionDto();
            List<ReporteResumenDepreciacionDto> ListaFinal = new List<ReporteResumenDepreciacionDto>();
            DateTime AnioActual = DateTime.Parse("01/01/" + Periodo.ToString());
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {

                List<ReporteLibroActivoDto> ActivosTodosPeriodos = new ActivoFijoBL().ReporteLibroActivo(ref objOperationResult, Periodo, mes, 0, 0, true, null);
                var TodosPeriodos = ActivosTodosPeriodos.Select(o => (ReporteLibroActivoDto)o.Clone()).ToList();
                var TodosPeriodosTotal = TodosPeriodos.GroupBy(o => o.v_IdActivoFijo).Select(o => o.LastOrDefault()).ToList();
                var ActivosActuales = TodosPeriodos.Where(o => o.FechaUso < AnioActual).ToList().GroupBy(l => new { l.v_IdActivoFijo }).Select(d =>
                {
                    var k = d.Where(o => o.d_AcumuladoHistorico > 0).LastOrDefault();
                    if (k != null)
                    {

                        k.d_ImporteMensualDepreciacion = d.Sum(h => h.d_ImporteMensualDepreciacion);
                        k.d_Importe1SoloMes = d.LastOrDefault().d_Importe1SoloMes;
                        k.d_AjusteDepreciacion = d.Sum(p => p.d_AjusteDepreciacion ?? 0);
                    }
                    else
                    {
                        k = d.FirstOrDefault();
                        k.d_ImporteMensualDepreciacion = 0;
                        k.d_Importe1SoloMes = d.FirstOrDefault().d_Importe1SoloMes;
                        k.v_IdActivoFijo = d.FirstOrDefault().v_IdActivoFijo;
                        k.CodigoActivoFijo = d.FirstOrDefault().CodigoActivoFijo;
                        k.d_AjusteDepreciacion = d.Sum(o => o.d_AjusteDepreciacion ?? 0);
                        k.Cuenta33 = d.FirstOrDefault().Cuenta33;
                        k.Cuenta39 = d.FirstOrDefault().Cuenta39;
                    }
                    return k;
                }).ToList();


                var ActivosDepreciadosPeriodoAnterios = new ActivoFijoBL().ReporteLibroActivo(ref objOperationResult, Periodo - 1, 12, 0, 0, true, null);

                List<ReporteLibroActivoDto> PeriodosAnteriores = ActivosDepreciadosPeriodoAnterios != null ? ActivosDepreciadosPeriodoAnterios.Select(o => (ReporteLibroActivoDto)o.Clone()).ToList() : null;

                List<ReporteLibroActivoDto> ActivosAnteriores = new List<ReporteLibroActivoDto>();
                if (PeriodosAnteriores != null)
                {
                    ActivosAnteriores = PeriodosAnteriores.ToList().GroupBy(l => new { l.v_IdActivoFijo }).Select(d =>
                    {

                        ReporteLibroActivoDto k = d.Where(o => o.d_AcumuladoHistorico > 0).LastOrDefault();
                        if (k != null)
                        {
                            k.d_ImporteMensualDepreciacion = d.Sum(h => h.d_ImporteMensualDepreciacion);
                            k.d_Importe1SoloMes = d.LastOrDefault().d_Importe1SoloMes.Value;
                            k.d_AjusteDepreciacion = d.Sum(p => p.d_AjusteDepreciacion ?? 0);
                        }
                        else
                        {
                            k = d.FirstOrDefault();
                            k.d_ImporteMensualDepreciacion = 0;
                            k.d_Importe1SoloMes = d.FirstOrDefault().d_Importe1SoloMes.Value;
                            k.v_IdActivoFijo = d.FirstOrDefault().v_IdActivoFijo;
                            k.CodigoActivoFijo = d.FirstOrDefault().CodigoActivoFijo;
                            k.Cuenta33 = d.FirstOrDefault().Cuenta33;
                            k.Cuenta39 = d.FirstOrDefault().Cuenta39;
                            k.d_AjusteDepreciacion = d.Sum(o => o.d_AjusteDepreciacion ?? 0);
                        }


                        return k;
                    }).ToList();
                }



                var CompradosPeriodoyDadosAlta = (from a in TodosPeriodosTotal

                                                  where a.AnioCompra == Periodo.ToString() && a.AnioUso == Periodo.ToString() && a.i_Baja == 0

                                                  select a).ToList();


                var CompradosPeriodoyDadosBaja = (from a in ActivosActuales

                                                  where a.AnioBaja == Periodo.ToString() && a.i_Baja == 1 && a.FechaBaja.Value.Month <= mes

                                                  select a).ToList();
                var Cuentas = ActivosActuales.Concat(ActivosAnteriores).Concat(CompradosPeriodoyDadosAlta).Concat(CompradosPeriodoyDadosBaja).Select(x => x.Cuenta39).Distinct().ToList();

                if (Detallado)
                {

                    var Totales = ActivosAnteriores.Concat(ActivosActuales).Concat(CompradosPeriodoyDadosAlta).Concat(CompradosPeriodoyDadosBaja).ToList().Select(o => new { cuenta = o.Cuenta39, IdActivoFijo = o.v_IdActivoFijo, codigoActivo = o.CodigoActivoFijo + " " + o.SoloDescripionActivo }).Distinct().ToList();


                    foreach (var item in Totales)
                    {//39143

                        objReporte = new ReporteResumenDepreciacionDto();
                        objReporte.KeyCuenta = item.cuenta == "0" ? "NO EXISTECUENTA NO ASOCIADA" : item.cuenta + "    " + NombreCuenta(item.cuenta);
                        objReporte.CodigoActivoFijo = item.codigoActivo;

                        //var SaldoPeriodoAnterior = ActivosAnteriores.Where(x => x.Cuenta39 == item.cuenta && x.v_IdActivoFijo == item.IdActivoFijo).FirstOrDefault();
                        var SaldoPeriodoAnterior = TodosPeriodosTotal.Where(x => x.Cuenta39 == item.cuenta && x.v_IdActivoFijo == item.IdActivoFijo).FirstOrDefault();
                        objReporte.SaldoAl = SaldoPeriodoAnterior != null ? SaldoPeriodoAnterior.AcumuladoAnterior : 0;//Todos los precios de Compra
                        var Solo1mES = ActivosTodosPeriodos.Where(x => x.Cuenta39 == item.cuenta && x.v_IdActivoFijo == item.IdActivoFijo).GroupBy(o => o.v_IdActivoFijo).Select(o => o.LastOrDefault());
                        var Ajuste = ActivosTodosPeriodos.Where(x => x.Cuenta39 == item.cuenta && x.v_IdActivoFijo == item.IdActivoFijo).ToList();


                        objReporte.DepreciacionMensual = Solo1mES.Sum(o => o.d_Importe1SoloMes).Value;
                        objReporte.DepreciacionActual = ActivosTodosPeriodos.Where(x => x.Cuenta39 == item.cuenta && x.v_IdActivoFijo == item.IdActivoFijo).Sum(x => x.d_ImporteMensualDepreciacion);
                        objReporte.Ajuste = Ajuste.Sum(o => o.d_AjusteDepreciacion);

                        var Adiciones = CompradosPeriodoyDadosAlta.Where(x => x.Cuenta39 == item.cuenta && x.v_IdActivoFijo == item.IdActivoFijo).FirstOrDefault();
                        objReporte.Adiciones = Adiciones != null ? Adiciones.d_AcumuladoHistorico : 0;
                        objReporte.DepreciacionActual = objReporte.DepreciacionActual - objReporte.Adiciones;
                        objReporte.Retiros = CompradosPeriodoyDadosBaja.Where(x => x.Cuenta39 == item.cuenta && x.v_IdActivoFijo == item.IdActivoFijo).Sum(x => x.d_ImporteMensualDepreciacion);
                        objReporte.Total = (objReporte.DepreciacionActual + objReporte.Adiciones) - objReporte.Retiros; //Depreciacion al
                        //objReporte.SaldoTotal = objReporte.SaldoAl + objReporte.Total;

                        objReporte.SaldoTotal = SaldoPeriodoAnterior != null ? SaldoPeriodoAnterior.d_AcumuladoTotal : 0;
                        ListaFinal.Add(objReporte);

                    }

                }
                else
                {

                    foreach (var item in Cuentas)
                    {//39143


                        objReporte = new ReporteResumenDepreciacionDto();
                        objReporte.CuentaDescripcion = item == "0" ? "NO EXISTECUENTA NO ASOCIADA" : item + "    " + NombreCuenta(item);
                        var SaldoPeriodoAnterior = TodosPeriodosTotal.Where(x => x.Cuenta39 == item).ToList();
                        objReporte.SaldoAl = SaldoPeriodoAnterior != null ? SaldoPeriodoAnterior.Sum(o => o.AcumuladoAnterior) : 0;//Todos los precios de Compra
                        objReporte.DepreciacionActual = ActivosTodosPeriodos.Where(x => x.Cuenta39 == item).Sum(x => x.d_ImporteMensualDepreciacion);
                        var Solo1mES = ActivosTodosPeriodos.Where(x => x.Cuenta39 == item).GroupBy(o => o.v_IdActivoFijo).Select(o => o.LastOrDefault());
                        var Ajuste = ActivosTodosPeriodos.Where(x => x.Cuenta39 == item).ToList();
                        objReporte.DepreciacionMensual = Solo1mES.Sum(o => o.d_Importe1SoloMes).Value;
                        objReporte.Ajuste = Ajuste.Sum(o => o.d_AjusteDepreciacion);
                        objReporte.Adiciones = CompradosPeriodoyDadosAlta.Where(x => x.Cuenta39 == item).Sum(d => d.d_AcumuladoHistorico);
                        // se agrego la resta de la depreciacion de la alta
                        objReporte.DepreciacionActual = ActivosTodosPeriodos.Where(x => x.Cuenta39 == item).Sum(x => x.d_ImporteMensualDepreciacion) - objReporte.Adiciones;
                        objReporte.Retiros = CompradosPeriodoyDadosBaja.Where(x => x.Cuenta39 == item).Sum(x => x.d_ImporteMensualDepreciacion);
                        objReporte.Total = (objReporte.DepreciacionActual + objReporte.Adiciones) - objReporte.Retiros; //Depreciacion al
                        objReporte.SaldoTotal = SaldoPeriodoAnterior != null ? SaldoPeriodoAnterior.Sum(o => o.d_AcumuladoTotal) : 0;
                        ListaFinal.Add(objReporte);

                    }
                }

            }


            if (Detallado)
            {
                return ListaFinal.OrderBy(o => o.KeyCuenta).ToList();
            }
            else
            {
                return ListaFinal.OrderBy(o => o.CuentaDescripcion).ToList();
            }

        }
        #endregion

        #region AsientoInventario

        public List<IdDiarioAsientosInventarios> GenerarAsientoInventario(ref OperationResult objOperationResult, int PeriodoReporte, int MesReporte, int TipoDocumento, decimal TipoCambio, string GlosaHistorico, string GlosaAjuste, DateTime Fecha, bool DepreciacionHistorico, bool AjusteDepreciacion, string ReferenciaDepreciacion, string ReferenciaAjuste)
        {
            try
            {
                var IdDiarios = new List<IdDiarioAsientosInventarios>();
                var FechaGeneracion = DateTime.Parse(DateTime.DaysInMonth(PeriodoReporte, MesReporte) + "/" + MesReporte + "/" + PeriodoReporte + " 23:59");
                var objDepreciacionHist = new DiarioDepreciacionHistorica();
                var objAjuste = new DiarioAjusteDepreciacion();
                Globals.ProgressbarStatus.i_TotalProgress = DepreciacionHistorico && AjusteDepreciacion ? 2 : 1;
                using (var ts = TransactionUtils.CreateTransactionScope())
                {

                    List<diariodetalleDto> TempDiarioInsertarAF = new List<diariodetalleDto>();
                    List<KeyValueDTO> _ListadoDiarios = new List<KeyValueDTO>();
                    diarioDto _diarioDto = new diarioDto();
                    diariodetalleDto _diarioDetalleDto = new diariodetalleDto();
                    decimal TotDebe, TotHaber, TotDebeC, TotHaberC;
                    DiarioBL _objDiarioBL = new DiarioBL();
                    int _MaxMovimiento;
                    AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();

                    List<ReporteAsientoDiario> aptitudeCertificate = new ActivoFijoBL().ReporteAsientoInventario(ref objOperationResult, PeriodoReporte, MesReporte, DepreciacionHistorico, AjusteDepreciacion, TipoCambio);

                    if (DepreciacionHistorico)
                    {


                        #region Genera Libro Diario Depreciación Histórico
                        _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref objOperationResult, PeriodoReporte.ToString(), MesReporte.ToString("00"), TipoDocumento);
                        _MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                        _MaxMovimiento++;
                        _diarioDto.v_IdDocumentoReferencia = "AF-" + ((int)TipoAsientoInventario.MensualHistoricos).ToString("00") + MesReporte.ToString("00") + PeriodoReporte.ToString();
                        _diarioDto.v_Periodo = PeriodoReporte.ToString();
                        _diarioDto.v_Mes = MesReporte.ToString("00");
                        _diarioDto.v_Nombre = GlosaHistorico;
                        _diarioDto.v_Glosa = GlosaHistorico;
                        _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                        _diarioDto.d_TipoCambio = TipoCambio;
                        _diarioDto.i_IdMoneda = (int)Currency.Soles;
                        _diarioDto.i_IdTipoDocumento = TipoDocumento;
                        _diarioDto.t_Fecha = Fecha;
                        _diarioDto.i_IdTipoComprobante = 2;

                        var SoloDepreciacion = aptitudeCertificate.Where(o => o.NroAsiento == "ASIENTO DEPRECIACIÓN");
                        foreach (var CuentaActivoFijo in SoloDepreciacion)
                        {

                            _diarioDetalleDto = new diariodetalleDto();
                            _diarioDetalleDto.v_NroCuenta = CuentaActivoFijo.Cuenta;
                            _diarioDetalleDto.v_Naturaleza = CuentaActivoFijo.Naturaleza;
                            _diarioDetalleDto.d_Importe = CuentaActivoFijo.d_Importe;
                            _diarioDetalleDto.d_Cambio = CuentaActivoFijo.d_Cambio;
                            _diarioDetalleDto.i_IdCentroCostos = CuentaActivoFijo.CentroCosto;
                            _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                            _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes.Trim() + " " + _diarioDto.v_Correlativo.Trim();
                            _diarioDetalleDto.i_IdTipoDocumentoRef = -1;
                            _diarioDetalleDto.v_NroDocumentoRef = String.Empty;
                            _diarioDetalleDto.v_Analisis = String.Empty;
                            _diarioDetalleDto.t_Fecha = Fecha;
                            _diarioDetalleDto.v_OrigenDestino = null;
                            _diarioDetalleDto.v_Pedido = null;
                            TempDiarioInsertarAF.Add(_diarioDetalleDto);

                        }


                        TotDebe = TempDiarioInsertarAF.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                        TotHaber = TempDiarioInsertarAF.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                        TotDebeC = TempDiarioInsertarAF.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                        TotHaberC = TempDiarioInsertarAF.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                        _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                        _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                        _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                        _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                        _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                        _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);


                        var DiarioHistorico = _objDiarioBL.BuscarDiarioInventarioPorDocReferencia("AF-" + ((int)TipoAsientoInventario.MensualHistoricos).ToString("00") + MesReporte.ToString("00") + PeriodoReporte);
                        string[] EliminadoDiarioHistorico = new string[3];
                        if (DiarioHistorico.Any())
                        {
                            EliminadoDiarioHistorico = _objDiarioBL.EliminarDiarioXDocRef(ref objOperationResult, DiarioHistorico.FirstOrDefault().v_IdDocumentoReferencia, Globals.ClientSession.GetAsList(), false);
                            _diarioDto.v_Periodo = EliminadoDiarioHistorico[0];
                            _diarioDto.v_Mes = EliminadoDiarioHistorico[1];
                            _diarioDto.v_Correlativo = EliminadoDiarioHistorico[2];


                            foreach (var item in TempDiarioInsertarAF)
                            {
                                item.i_IdTipoDocumento = _diarioDto.i_IdTipoDocumento;
                                item.v_NroDocumento = _diarioDto.v_Mes.Trim() + " " + _diarioDto.v_Correlativo.Trim();
                            }
                        }

                        if (TempDiarioInsertarAF.Count() > 0)
                        {
                            _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), TempDiarioInsertarAF, (int)TipoMovimientoTesoreria.Ninguno);

                            if (objOperationResult.Success == 1)
                            {
                                IdDiarioAsientosInventarios IdDiar = new IdDiarioAsientosInventarios();
                                IdDiar.Tipo = (int)TipoAsientoInventario.MensualHistoricos;
                                IdDiar.Id = _diarioDto.v_IdDocumentoReferencia;
                                IdDiarios.Add(IdDiar);
                            }

                        }
                        else
                        {

                            objOperationResult.Success = 1;
                        }

                        Globals.ProgressbarStatus.i_Progress++;
                        #endregion
                    }
                    if (AjusteDepreciacion)
                    {

                        #region  Genera Libro Diario Ajuste Depreciación
                        TempDiarioInsertarAF = new List<diariodetalleDto>();
                        var newDiario = _diarioDto;
                        _diarioDto = new diarioDto();
                        _diarioDto = newDiario;
                        _diarioDto.v_IdDocumentoReferencia = "AF-" + ((int)TipoAsientoInventario.Ajustes).ToString("00") + MesReporte.ToString("00") + PeriodoReporte.ToString();
                        _diarioDto.v_Nombre = GlosaAjuste;
                        _diarioDto.v_Glosa = GlosaAjuste;
                        _ListadoDiarios = _objDiarioBL.ObtenerListadoDiario(ref objOperationResult, PeriodoReporte.ToString(), MesReporte.ToString("00"), TipoDocumento);
                        _MaxMovimiento = _ListadoDiarios.Count() > 0 ? int.Parse(_ListadoDiarios[_ListadoDiarios.Count() - 1].Value1.ToString()) : 0;
                        _MaxMovimiento++;
                        _diarioDto.v_Correlativo = _MaxMovimiento.ToString("00000000");
                        _diarioDto.v_Mes = MesReporte.ToString("00");
                        foreach (var CuentaActivoFijo in aptitudeCertificate.Where(o => o.NroAsiento == "ASIENTO AJUSTE"))
                        {
                            _diarioDetalleDto = new diariodetalleDto();
                            _diarioDetalleDto.v_NroCuenta = CuentaActivoFijo.Cuenta;

                            _diarioDetalleDto.v_Naturaleza = CuentaActivoFijo.Naturaleza;
                            _diarioDetalleDto.d_Importe = CuentaActivoFijo.d_Importe;
                            _diarioDetalleDto.d_Cambio = CuentaActivoFijo.d_Cambio;
                            _diarioDetalleDto.i_IdCentroCostos = CuentaActivoFijo.CentroCosto;
                            _diarioDetalleDto.i_IdTipoDocumento = TipoDocumento;
                            _diarioDetalleDto.v_NroDocumento = _diarioDto.v_Mes.Trim() + " " + _diarioDto.v_Correlativo.Trim();
                            _diarioDetalleDto.i_IdTipoDocumentoRef = -1;
                            _diarioDetalleDto.i_IdCentroCostos = CuentaActivoFijo.CentroCosto;
                            _diarioDetalleDto.v_NroDocumentoRef = String.Empty;
                            _diarioDetalleDto.v_Analisis = String.Empty;
                            _diarioDetalleDto.t_Fecha = Fecha;
                            _diarioDetalleDto.v_OrigenDestino = null;
                            _diarioDetalleDto.v_Pedido = null;
                            TempDiarioInsertarAF.Add(_diarioDetalleDto);

                        }
                        TotDebe = TempDiarioInsertarAF.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Importe.Value);
                        TotHaber = TempDiarioInsertarAF.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Importe.Value);
                        TotDebeC = TempDiarioInsertarAF.Where(p => p.v_Naturaleza == "D").Sum(o => o.d_Cambio.Value);
                        TotHaberC = TempDiarioInsertarAF.Where(p => p.v_Naturaleza == "H").Sum(o => o.d_Cambio.Value);
                        _diarioDto.d_TotalDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe, 2);
                        _diarioDto.d_TotalHaber = Utils.Windows.DevuelveValorRedondeado(TotHaber, 2);
                        _diarioDto.d_TotalDebeCambio = Utils.Windows.DevuelveValorRedondeado(TotDebeC, 2);
                        _diarioDto.d_TotalHaberCambio = Utils.Windows.DevuelveValorRedondeado(TotHaberC, 2);
                        _diarioDto.d_DiferenciaDebe = Utils.Windows.DevuelveValorRedondeado(TotDebe - TotHaber, 2);
                        _diarioDto.d_DiferenciaHaber = Utils.Windows.DevuelveValorRedondeado(TotDebeC - TotHaberC, 2);


                        var DiarioHistoricoDepreciacion = _objDiarioBL.BuscarDiarioInventarioPorDocReferencia("AF-" + ((int)TipoAsientoInventario.Ajustes).ToString("00") + MesReporte.ToString("00") + PeriodoReporte);
                        string[] EliminadoDiarioDepreciacion = new string[3];
                        if (DiarioHistoricoDepreciacion.Any())
                        {
                            EliminadoDiarioDepreciacion = _objDiarioBL.EliminarDiarioXDocRef(ref objOperationResult, DiarioHistoricoDepreciacion.FirstOrDefault().v_IdDocumentoReferencia, Globals.ClientSession.GetAsList(), false);
                            _diarioDto.v_Periodo = EliminadoDiarioDepreciacion[0];
                            _diarioDto.v_Mes = EliminadoDiarioDepreciacion[1];
                            _diarioDto.v_Correlativo = EliminadoDiarioDepreciacion[2];
                            foreach (var item in TempDiarioInsertarAF)
                            {
                                item.i_IdTipoDocumento = _diarioDto.i_IdTipoDocumento;
                                item.v_NroDocumento = _diarioDto.v_Mes.Trim() + " " + _diarioDto.v_Correlativo.Trim();
                            }
                        }
                        if (TempDiarioInsertarAF.Count() > 0)
                        {
                            _objDiarioBL.InsertarDiario(ref objOperationResult, _diarioDto, Globals.ClientSession.GetAsList(), TempDiarioInsertarAF, (int)TipoMovimientoTesoreria.Ninguno);

                            if (objOperationResult.Success == 1)
                            {
                                IdDiarioAsientosInventarios IdDiar = new IdDiarioAsientosInventarios();
                                IdDiar.Tipo = (int)TipoAsientoInventario.Ajustes;
                                IdDiar.Id = _diarioDto.v_IdDocumentoReferencia;
                                IdDiarios.Add(IdDiar);
                            }

                        }
                        else // Solo para que mensaje salga OK , Pero indicar que no se generó ningun Diario 
                        {

                            objOperationResult.Success = 1;
                        }
                        objOperationResult.Success = objOperationResult.Success == 0 ? 0 : 1;

                        Globals.ProgressbarStatus.i_Progress++;
                        #endregion

                    }

                    ts.Complete();
                }
                return IdDiarios;
            }

            catch (Exception ex)
            {
                return null;
            }

        }

        #endregion
    }
}
