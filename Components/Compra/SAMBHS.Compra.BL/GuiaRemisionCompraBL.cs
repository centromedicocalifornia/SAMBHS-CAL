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
using SAMBHS.Almacen.BL;
using System.Data.Objects;
using System.Transactions;

namespace SAMBHS.Compra.BL
{
    public class GuiaRemisionCompraBL
    {
        public List<KeyValueDTO> ObtenerListadoGuiaRemisionCompra(ref OperationResult pobjOperationResult, string pstringPeriodo, string pstringMes)
        {
            using (SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin())
            {
                string idAlmacen = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                var query = (from n in dbcontext.guiaremisioncompra
                    where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes && n.v_IdGuiaCompra.Substring(2, 2) == idAlmacen
                    orderby n.v_Correlativo ascending
                    select new
                    {
                        v_Correlativo = n.v_Correlativo,
                        v_IdCompra = n.v_IdGuiaCompra
                    }
                );

                var query2 = query.AsEnumerable()
                    .Select(x => new KeyValueDTO
                    {
                        Value1 = x.v_Correlativo,
                        Value2 = x.v_IdCompra
                    }).ToList();

                return query2; 
            }
        }

        public guiaremisioncompraDto ObtenerGuiaRemisionCompraCabecera(ref OperationResult pobjOperationResult, string pstrIdCompra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    guiaremisioncompra _guiaremisioncompra = (from g in dbContext.guiaremisioncompra
                                                              where g.v_IdGuiaCompra == pstrIdCompra
                                                              select g).FirstOrDefault();

                    guiaremisioncompraDto _guiaremisioncompraDto = new guiaremisioncompraDto();

                    _guiaremisioncompraDto = _guiaremisioncompra.ToDTO();

                    pobjOperationResult.Success = 1;

                    return _guiaremisioncompraDto; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public BindingList<guiaremisioncompradetalleDto> ObtenerGuiaRemisionCompraDetalles(ref OperationResult pobjOperationResult, string pstrIdCompra)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.guiaremisioncompradetalle
                                 join A in dbContext.productodetalle on n.v_IdProductoDetalle equals A.v_IdProductoDetalle
                                 join B in dbContext.producto on A.v_IdProducto equals B.v_IdProducto
                                 join J1 in dbContext.datahierarchy on new { a = B.i_IdUnidadMedida.Value, b = 17 }
                                                                    equals new { a = J1.i_ItemId, b = J1.i_GroupId } into J1_join
                                 from J1 in J1_join.DefaultIfEmpty()
                                 where n.i_Eliminado == 0 && n.v_IdGuiaCompra == pstrIdCompra
                                 orderby n.t_InsertaFecha ascending
                                 select new guiaremisioncompradetalleDto
                                 {
                                     d_Cantidad = n.d_Cantidad,
                                     d_CantidadEmpaque = n.d_CantidadEmpaque,
                                     d_PrecioUnitario = n.d_PrecioUnitario,
                                     i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                     i_Eliminado = n.i_Eliminado,
                                     i_EsServicio = B.i_EsServicio.Value,
                                     i_IdAlmacen = n.i_IdAlmacen,
                                     i_IdUnidadMedida = n.i_IdUnidadMedida,
                                     i_IdUnidadMedidaProducto = B.i_IdUnidadMedida,
                                     i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                     t_ActualizaFecha = n.t_ActualizaFecha,
                                     t_InsertaFecha = n.t_InsertaFecha,
                                     UMEmpaque = J1.v_Value1,
                                     v_CodigoInterno = B.v_CodInterno,
                                     v_IdGuiaCompra = n.v_IdGuiaCompra,
                                     v_IdGuiaCompraDetalle = n.v_IdGuiaCompraDetalle,
                                     v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                                     v_IdProductoDetalle = n.v_IdProductoDetalle,
                                     v_NombreProducto = B.v_Descripcion,
                                     v_NroCuenta = n.v_NroCuenta,
                                     d_Empaque = B.d_Empaque,
                                     v_NroLote =n.v_NroLote ,
                                     v_NroSerie =n.v_NroSerie ,
                                     t_FechaCaducidad =n.t_FechaCaducidad ,
                                     i_SolicitarNroLoteIngreso = B.i_SolicitarNroLoteIngreso ?? 0,
                                     i_SolicitarNroSerieIngreso = B.i_SolicitarNroSerieIngreso ?? 0,
                                     i_SolicitaOrdenProduccionIngreso = B.i_SolicitaOrdenProduccionIngreso ?? 0,

                                 }
                        ).ToList();

                    pobjOperationResult.Success = 1;

                    return new BindingList<guiaremisioncompradetalleDto>(query);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public void InsertarGuiaRemisionCompra(ref OperationResult pobjOperationResult, guiaremisioncompraDto pobjDtoEntity, List<string> ClientSession, List<guiaremisioncompradetalleDto> pTemp_Insertar)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        guiaremisioncompra objEntityCompra = pobjDtoEntity.ToEntity();
                        guiaremisioncompradetalle pobjDtoCompraDetalle = new guiaremisioncompradetalle();

                        int SecuentialId = 0;
                        string newIdCompra = string.Empty;
                        string newIdCompraDetalle = string.Empty;
                        int intNodeId;

                        #region Inserta Cabecera

                        objEntityCompra.t_InsertaFecha = DateTime.Now;
                        objEntityCompra.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityCompra.i_Eliminado = 0;

                        intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 57);
                        newIdCompra = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XG");
                        objEntityCompra.v_IdGuiaCompra = newIdCompra;
                        dbContext.AddToguiaremisioncompra(objEntityCompra);
                        #endregion

                        #region Inserta Detalle
                        foreach (guiaremisioncompradetalleDto guiaremisioncompradetalleDto in pTemp_Insertar)
                        {
                            var NotadeIngresoDetalle = (from n in dbContext.movimientodetalle

                                                        join J1 in dbContext.movimiento on new { idMovimiento = n.v_IdMovimiento, IdAlmacen = guiaremisioncompradetalleDto.i_IdAlmacen }
                                                                                        equals new { idMovimiento = J1.v_IdMovimiento, IdAlmacen = J1.i_IdAlmacenOrigen } into J1_join
                                                        from J1 in J1_join.DefaultIfEmpty()

                                                        where n.i_Eliminado == 0 && n.v_IdProductoDetalle == guiaremisioncompradetalleDto.v_IdProductoDetalle && J1.v_OrigenTipo == "G" && J1.v_OrigenRegPeriodo == objEntityCompra.v_Periodo
                                                              && J1.v_OrigenRegMes == objEntityCompra.v_Mes && J1.v_OrigenRegCorrelativo == objEntityCompra.v_Correlativo

                                                        select new { n.v_IdMovimientoDetalle }).FirstOrDefault();

                            guiaremisioncompradetalle objEntityCompraDetalle = guiaremisioncompradetalleAssembler.ToEntity(guiaremisioncompradetalleDto);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 58);
                            newIdCompraDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XH");
                            objEntityCompraDetalle.v_IdGuiaCompraDetalle = newIdCompraDetalle;
                            objEntityCompraDetalle.v_IdGuiaCompra = newIdCompra;
                            objEntityCompraDetalle.v_IdMovimientoDetalle = NotadeIngresoDetalle != null ? NotadeIngresoDetalle.v_IdMovimientoDetalle : null;
                            objEntityCompraDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityCompraDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityCompraDetalle.i_Eliminado = 0;
                            dbContext.AddToguiaremisioncompradetalle(objEntityCompraDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "guiaremisioncompradetalle", newIdCompraDetalle);
                        }
                        #endregion

                        #region Actualizar EstadoOrdenCompra
                        if (!string.IsNullOrEmpty(pobjDtoEntity.v_NroOrdenCompra) && pobjDtoEntity.v_NroOrdenCompra.Contains('-'))
                        {
                            var serCorr = pobjDtoEntity.v_NroOrdenCompra.Split('-');
                            var serie = serCorr[0];
                            var correlativo = serCorr[1];


                            var ordenCompra =
                         dbContext.ordendecompra.FirstOrDefault(
                             p =>

                                 p.v_SerieDocumento == serie &&
                                 p.v_CorrelativoDocumento == correlativo && p.i_Eliminado == 0);

                            if (ordenCompra != null)
                            {
                                ordenCompra.i_IdEstado = 2;
                                dbContext.ordendecompra.ApplyCurrentValues(ordenCompra);
                            }
                        }
                        #endregion

                        dbContext.SaveChanges();

                        #region Genera el ingreso a almacén
                        GenerarIngresoAlmacen(ref pobjOperationResult, newIdCompra);
                        if (pobjOperationResult.Success == 0) return;
                        #endregion

                        pobjOperationResult.Success = 1;

                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "guiaremisioncompra", objEntityCompra.v_IdGuiaCompra);
                        ts.Complete(); 
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionCompraBL.InsertarGuiaRemisionCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void ActualizarGuiaRemisionCompra(ref OperationResult pobjOperationResult, guiaremisioncompraDto pobjDtoEntity, List<string> ClientSession, List<guiaremisioncompradetalleDto> pTemp_Insertar, List<guiaremisioncompradetalleDto> pTemp_Editar, List<guiaremisioncompradetalleDto> pTemp_Eliminar)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    guiaremisioncompra objEntityCompra = guiaremisioncompraAssembler.ToEntity(pobjDtoEntity);
                    compradetalleDto pobjDtoCompraDetalle = new compradetalleDto();
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        int SecuentialId = 0;
                        string newIdCompraDetalle = string.Empty;
                        int intNodeId;

                        #region Actualiza Cabecera
                        intNodeId = int.Parse(ClientSession[0]);
                        var objEntitySource = (from a in dbContext.guiaremisioncompra
                                               where a.v_IdGuiaCompra == pobjDtoEntity.v_IdGuiaCompra
                                               select a).FirstOrDefault();

                        pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                        pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                        guiaremisioncompra objEntity = guiaremisioncompraAssembler.ToEntity(pobjDtoEntity);
                        dbContext.guiaremisioncompra.ApplyCurrentValues(objEntity);
                        #endregion

                        #region Actualiza Detalle
                        foreach (guiaremisioncompradetalleDto compradetalleDto in pTemp_Insertar)
                        {
                            var NotadeIngresoDetalle = (from n in dbContext.movimientodetalle

                                                        join J1 in dbContext.movimiento on new { idMovimiento = n.v_IdMovimiento, IdAlmacen = compradetalleDto.i_IdAlmacen }
                                                                                        equals new { idMovimiento = J1.v_IdMovimiento, IdAlmacen = J1.i_IdAlmacenOrigen } into J1_join
                                                        from J1 in J1_join.DefaultIfEmpty()

                                                        where n.i_Eliminado == 0 && n.v_IdProductoDetalle == compradetalleDto.v_IdProductoDetalle && J1.v_OrigenTipo == "G" && J1.v_OrigenRegPeriodo == objEntityCompra.v_Periodo
                                                              && J1.v_OrigenRegMes == objEntityCompra.v_Mes && J1.v_OrigenRegCorrelativo == objEntityCompra.v_Correlativo
                                                        select new { n.v_IdMovimientoDetalle }).FirstOrDefault();

                            guiaremisioncompradetalle objEntityCompraDetalle = guiaremisioncompradetalleAssembler.ToEntity(compradetalleDto);
                            SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 58);
                            newIdCompraDetalle = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "XH");
                            objEntityCompraDetalle.v_IdGuiaCompraDetalle = newIdCompraDetalle;
                            objEntityCompraDetalle.v_IdGuiaCompra = objEntitySource.v_IdGuiaCompra;
                            objEntityCompraDetalle.v_IdMovimientoDetalle = NotadeIngresoDetalle != null ? NotadeIngresoDetalle.v_IdMovimientoDetalle : null;
                            objEntityCompraDetalle.t_InsertaFecha = DateTime.Now;
                            objEntityCompraDetalle.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityCompraDetalle.i_Eliminado = 0;
                            dbContext.AddToguiaremisioncompradetalle(objEntityCompraDetalle);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "guiaremisioncompradetalle", newIdCompraDetalle);
                        }

                        foreach (guiaremisioncompradetalleDto compradetalleDto in pTemp_Editar)
                        {
                            guiaremisioncompradetalle _objEntity = guiaremisioncompradetalleAssembler.ToEntity(compradetalleDto);
                            var query = (from n in dbContext.guiaremisioncompradetalle
                                         where n.v_IdGuiaCompraDetalle == compradetalleDto.v_IdGuiaCompraDetalle
                                         select n).FirstOrDefault();

                            _objEntity.t_ActualizaFecha = DateTime.Now;
                            _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                            dbContext.guiaremisioncompradetalle.ApplyCurrentValues(_objEntity);
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "guiaremisioncompradetalle", query.v_IdGuiaCompraDetalle);
                        }

                        foreach (guiaremisioncompradetalleDto compradetalleDto in pTemp_Eliminar)
                        {
                            guiaremisioncompradetalle _objEntity = guiaremisioncompradetalleAssembler.ToEntity(compradetalleDto);
                            var query = (from n in dbContext.guiaremisioncompradetalle
                                         where n.v_IdGuiaCompraDetalle == compradetalleDto.v_IdGuiaCompraDetalle
                                         select n).FirstOrDefault();

                            if (query != null)
                            {
                                query.t_ActualizaFecha = DateTime.Now;
                                query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                query.i_Eliminado = 1;
                            }

                            dbContext.guiaremisioncompradetalle.ApplyCurrentValues(query);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "guiaremisioncompradetalle", query.v_IdGuiaCompraDetalle);
                        }
                        #endregion

                        dbContext.SaveChanges();

                        #region Regenera el ingreso a almacén
                        RegenerarIngresoAlmacen(ref pobjOperationResult, objEntitySource.v_IdGuiaCompra);
                        if (pobjOperationResult.Success == 0) return;
                        #endregion

                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "guiaremisioncompra", objEntityCompra.v_IdGuiaCompra);
                        pobjOperationResult.Success = 1;
                        ts.Complete(); 
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionCompraBL.ActualizarGuiaRemisionCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public void EliminarGuiaRemisionCompra(ref OperationResult pobjOperationResult, string pstrIdCompra, List<string> ClientSession)
        {
            try
            {
                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {

                        #region Elimina Cabecera
                        // Obtener la entidad fuente
                        var objEntitySource = (from a in dbContext.guiaremisioncompra
                                               where a.v_IdGuiaCompra == pstrIdCompra
                                               select a).FirstOrDefault();

                        // Crear la entidad con los datos actualizados
                        objEntitySource.t_ActualizaFecha = DateTime.Now;
                        objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntitySource.i_Eliminado = 1;
                        #endregion

                        #region Elimina Detalles
                        //Eliminar detalles del movimiento eliminado.
                        var objEntitySourceDetallesCompra = (from a in dbContext.guiaremisioncompradetalle
                                                             where a.v_IdGuiaCompra == pstrIdCompra
                                                             select a).ToList();

                        foreach (var RegistroCompraDetalle in objEntitySourceDetallesCompra)
                        {
                            RegistroCompraDetalle.t_ActualizaFecha = DateTime.Now;
                            RegistroCompraDetalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            RegistroCompraDetalle.i_Eliminado = 1;
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "guiaremisioncompradetalle", RegistroCompraDetalle.v_IdGuiaCompraDetalle);
                        }
                        #endregion

                        #region Elimina Notas de Ingresos Relacionadas
                        EliminarIngresoAlmacen(ref pobjOperationResult, objEntitySource.v_IdGuiaCompra);
                        if (pobjOperationResult.Success == 0) return;
                        #endregion

                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "guiaremisioncompra", objEntitySource.v_IdGuiaCompra);
                        pobjOperationResult.Success = 1;
                        ts.Complete(); 
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionCompraBL.EliminarGuiaRemisionCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var Registro = (from n in dbContext.guiaremisioncompra
                                where n.i_Eliminado == 0 && n.v_Periodo == Periodo && n.v_Mes == Mes && n.v_Correlativo == Correlativo
                                select n).FirstOrDefault();

                return Registro == null; 
            }
        }

        public List<string[]> DevolverNombres(string pstrIdGuia)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var query2 = DevuelveNombresGuia(dbContext, pstrIdGuia);

                if (query2 != null)
                {
                    List<string[]> Lista = new List<string[]>();
                    foreach (var Fila in query2)
                    {
                        string[] Cadena = new string[6];
                        Cadena[0] = Fila.CodigoInterno;
                        Cadena[1] = Fila.Empaque.ToString();
                        Cadena[2] = Fila.UMEmpaque;
                        Cadena[3] = Fila.Descripcion;
                        Cadena[4] = Fila.i_EsServicio.Value.ToString();
                        Cadena[5] = Fila.i_IdUnidadMedida.ToString();
                        Lista.Add(Cadena);
                    }
                    return Lista;
                }
                return null;
            }
        }

        public string[] DevolverProveedorPorID(ref OperationResult pobjOperationResult, string pstrIdProveedor)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from n in dbContext.cliente
                                 where n.i_Eliminado == 0 && n.v_FlagPantalla == "V" && n.v_IdCliente == pstrIdProveedor
                                 select n
                                         ).FirstOrDefault();

                    pobjOperationResult.Success = 1;

                    if (query != null)
                    {
                        string[] Cadena = new string[3];
                        Cadena[0] = query.v_NroDocIdentificacion;
                        Cadena[1] = query.v_CodCliente;
                        Cadena[2] = (query.v_ApePaterno + " " + query.v_ApeMaterno + " " + query.v_PrimerNombre + " " + query.v_RazonSocial).Trim();
                        return Cadena;
                    }
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionCompraBL.DevolverProveedorPorID()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
            return null;
        }

        public bool ComprobarRelacionDocumentoProveedor(ref OperationResult pobjOperationResult, string pstrIdProveedor, int pintIdTipoDocumento, string pstrSerieDoc, string pstrCorrelativoDoc, string pstrIdCompra)
        {
            try
            {
                if (pintIdTipoDocumento != -1 && pstrIdProveedor != null && pstrSerieDoc != null && pstrCorrelativoDoc != null) // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        if (pstrIdCompra == null) // SI el idCompra es nulo se esta consultado de una compra nueva que no ha sido guardada
                        {
                            var query = (from n in dbContext.guiaremisioncompra
                                         where n.i_Eliminado == 0 && n.v_IdProveedor == pstrIdProveedor
                                         && n.i_IdTipoDocumento == pintIdTipoDocumento
                                         && n.v_SerieDocumento == pstrSerieDoc && n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                         select n
                                        ).FirstOrDefault();

                            return query != null;
                        }
                        else // si no es nulo se comprueba de una compra que esta siendo modificada
                        {
                            var query = (from n in dbContext.guiaremisioncompra
                                         where n.i_Eliminado == 0 && n.v_IdProveedor == pstrIdProveedor
                                         && n.i_IdTipoDocumento == pintIdTipoDocumento
                                         && n.v_SerieDocumento == pstrSerieDoc && n.v_CorrelativoDocumento == pstrCorrelativoDoc
                                         && n.v_IdGuiaCompra != pstrIdCompra
                                         select n
                                         ).FirstOrDefault();
                            return query != null;
                        } 
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionCompraBL.ComprobarRelacionDocumentoProveedor()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
            return false;
        }

        public bool ComprobarSiFueLlamadaEnCompras(ref OperationResult pobjOperationResult, string pstrSerie, string pstrCorrelativo, string pstrIdProveedor)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Result = (from c in dbContext.compra
                                  where c.v_GuiaRemisionSerie == pstrSerie && c.v_GuiaRemisionCorrelativo == pstrCorrelativo && c.v_IdProveedor == pstrIdProveedor
                                  && c.i_Eliminado == 0
                                  select c).Count();

                    if (Result > 0)
                    {
                        pobjOperationResult.Success = 1;
                        return true;
                    }
                    else
                    {
                        pobjOperationResult.Success = 1;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionCompraBL.ComprobarSiFueLlamadaEnCompras()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return false;
            }
        }

        /// <summary>
        /// Obtiene la orden de compra deacuerdo a la serie y correlativo señalado.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="serie"></param>
        /// <param name="correlativo"></param>
        /// <returns></returns>
        public BindingList<guiaremisioncompradetalleDto> ObtenerOrdenDeCompra(ref OperationResult pobjOperationResult, string serie, string correlativo, out clienteDto proveedor)
        {
            proveedor = new clienteDto();
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var oc = dbContext.ordendecompra.FirstOrDefault(p =>
                                p.v_SerieDocumento == serie && p.v_CorrelativoDocumento == correlativo &&
                                p.i_Eliminado == 0);

                    if (oc == null) throw new ArgumentNullException(@"No Existe la Orden de Compra");

                    proveedor = dbContext.cliente.FirstOrDefault(p => p.v_IdCliente == oc.v_IdProveedor).ToDTO();

                    var result = oc.ordendecompradetalle.Where(p => p.i_Eliminado == 0).Select(o =>
                    {
                        var producto = dbContext.productodetalle.FirstOrDefault(p => p.v_IdProductoDetalle == o.v_IdProductoDetalle);
                        if (producto != null)
                            return new guiaremisioncompradetalleDto
                            {
                                v_NroCuenta = CuentaPorProductoDetalle(o.v_IdProductoDetalle),
                                v_IdProductoDetalle = o.v_IdProductoDetalle,
                                d_Cantidad = o.d_Cantidad,
                                d_CantidadEmpaque = o.d_CantidadEmpaque,
                                i_IdAlmacen = o.i_IdAlmacen,
                                i_IdUnidadMedida = o.i_IdUnidadMedida,
                                v_NombreProducto = producto.producto.v_Descripcion,
                                v_CodigoInterno = producto.producto.v_CodInterno,
                                i_EsServicio = producto.producto.i_EsServicio.Value,
                                d_Empaque = producto.producto.d_Empaque,
                                i_IdUnidadMedidaProducto = producto.producto.i_IdUnidadMedida,
                                d_PrecioUnitario = o.d_PrecioUnitario,
                                t_FechaCaducidad =  DateTime.Parse(Constants.FechaNula),
                            };
                        return null;
                    }).ToList();

                    pobjOperationResult.Success = 1;
                    return new BindingList<guiaremisioncompradetalleDto>(result);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionCompraBL.ObtenerOrdenDeCompra()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        private string CuentaPorProductoDetalle(string productodetalle)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var pd = dbContext.productodetalle.FirstOrDefault(p => p.v_IdProductoDetalle == productodetalle);
                if (pd != null)
                {
                    var producto = dbContext.producto.FirstOrDefault(p => p.v_IdProducto == pd.v_IdProducto);
                    var linea = dbContext.linea.FirstOrDefault(p => p.v_IdLinea == producto.v_IdLinea);
                    if (linea != null) return linea.v_NroCuentaCompra;
                }
            }

            return null;
        }

        public static string ObtenerNiRelacionada(string pstrIdGrm)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var movimiento =
                        dbContext.movimiento.FirstOrDefault(
                            p => p.v_IdMovimientoOrigen.Equals(pstrIdGrm) && p.i_Eliminado == 0);

                    var nroMovimiento = movimiento != null
                        ? string.Join("-", movimiento.v_Periodo, movimiento.v_Mes, movimiento.v_Correlativo)
                        : string.Empty;

                    return !string.IsNullOrWhiteSpace(nroMovimiento)
                        ? "Nota de Ingreso Generada: " + nroMovimiento
                        : "Nota de Ingreso fue consumida por una compra.";
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        #region Bandeja
        public List<guiaremisioncompraShortDto> ListarGuiasRemisionCompra(ref OperationResult pobjOperationResult, DateTime F_Ini, DateTime F_Fin, string pstrFilterExpression)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var Lista = (from a in dbContext.guiaremisioncompra

                                 join A in dbContext.cliente on a.v_IdProveedor equals A.v_IdCliente into A_join
                                 from A in A_join.DefaultIfEmpty()

                                 join J2 in dbContext.systemuser on new { i_UpdateUserId = a.i_ActualizaIdUsuario.Value }
                                                                equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                 from J2 in J2_join.DefaultIfEmpty()

                                 join J3 in dbContext.systemuser on new { i_InsertUserId = a.i_InsertaIdUsuario.Value }
                                                                equals new { i_InsertUserId = J3.i_SystemUserId } into J3_join
                                 from J3 in J3_join.DefaultIfEmpty()

                                 where a.t_Fecha >= F_Ini && a.t_Fecha <= F_Fin && a.i_Eliminado == 0

                                 select new guiaremisioncompraShortDto
                                 {
                                     v_IdGuiaCompra = a.v_IdGuiaCompra,
                                     NroRegistro = a.v_Mes.Trim() + "-" + a.v_Correlativo,
                                     Documento = a.v_SerieDocumento + " - " + a.v_CorrelativoDocumento,
                                     NombreProveedor = (A.v_ApePaterno + " " + A.v_ApeMaterno + " " + A.v_PrimerNombre + " " + A.v_RazonSocial).Trim(),
                                     t_Fecha = a.t_Fecha,
                                     t_ActualizaFecha = a.t_ActualizaFecha,
                                     t_InsertaFecha = a.t_InsertaFecha,
                                     v_IdProveedor = a.v_IdProveedor,
                                     v_UsuarioCreacion = J3.v_UserName,
                                     v_UsuarioModificacion = J2.v_UserName,
                                     Moneda = a.i_IdMoneda == 1 ? "S" : "D"
                                 }
                                 );

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        Lista = Lista.Where(pstrFilterExpression);
                    }

                    if (Lista != null)
                    {
                        pobjOperationResult.Success = 1;
                        return Lista.ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {
                pobjOperationResult.Success = 0;
                return null;
            }
        }
        #endregion

        #region QuerysCompilados
        public static Func<SAMBHSEntitiesModelWin, string, IQueryable<CompiladoResult>>
                        DevuelveNombresGuia = CompiledQuery.Compile((SAMBHSEntitiesModelWin db, string ID) =>
                        from n in db.guiaremisioncompradetalle
                        join A in db.productodetalle on n.v_IdProductoDetalle equals A.v_IdProductoDetalle
                        join B in db.producto on A.v_IdProducto equals B.v_IdProducto

                        join J1 in db.datahierarchy on new { a = B.i_IdUnidadMedida.Value, b = 17 }
                                                           equals new { a = J1.i_ItemId, b = J1.i_GroupId } into J1_join
                        from J1 in J1_join.DefaultIfEmpty()

                        where n.v_IdGuiaCompra == ID && n.i_Eliminado == 0

                        select new CompiladoResult
                        {
                            CodigoInterno = B.v_CodInterno,
                            Empaque = B.d_Empaque,
                            UMEmpaque = J1.v_Value1,
                            Descripcion = B.v_Descripcion,
                            i_EsServicio = B.i_EsServicio,
                            i_IdUnidadMedida = B.i_IdUnidadMedida
                        });
        #endregion

        #region Almacén
        public static void GenerarIngresoAlmacen(ref OperationResult pobjOperationResult, string pstrIdGuiaCompra)
        {
            pobjOperationResult.Success = 1;
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var guiaCompra = dbContext.guiaremisioncompra.FirstOrDefault(p => p.v_IdGuiaCompra.Equals(pstrIdGuiaCompra));

                        if (guiaCompra == null) return;

                        #region recopila los detalles de la compra
                        var guiaCompraDetalles = (from p in dbContext.guiaremisioncompradetalle

                                                  join J1 in dbContext.productodetalle on p.v_IdProductoDetalle equals J1.v_IdProductoDetalle
                                                      into J1_join
                                                  from J1 in J1_join.DefaultIfEmpty()

                                                  join J2 in dbContext.producto on J1.v_IdProducto equals J2.v_IdProducto into J2_join
                                                  from J2 in J2_join.DefaultIfEmpty()

                                                  where
                                                      p.i_Eliminado == 0 && p.v_IdProductoDetalle != null &&
                                                      !p.v_IdProductoDetalle.Trim().Equals("") && J2.i_EsServicio == 0
                                                      && p.v_IdGuiaCompra.Equals(pstrIdGuiaCompra)
                                                  select p).ToList();
                        #endregion

                        if (!guiaCompraDetalles.Any()) return;
                        var listaMovimientos = new MovimientoBL().ObtenerListadoMovimientos(ref pobjOperationResult, guiaCompra.v_Periodo, guiaCompra.v_Mes, (int)TipoDeMovimiento.NotadeIngreso);
                        int maxMovimiento = listaMovimientos.Any() ? int.Parse(listaMovimientos[listaMovimientos.Count - 1].Value1) : 0;

                        foreach (var detalle in guiaCompraDetalles.GroupBy(p => p.i_IdAlmacen))
                        {
                            var movimientoDto = new movimientoDto();
                            maxMovimiento++;
                            movimientoDto.d_TipoCambio = guiaCompra.d_TipoCambio;
                            movimientoDto.i_IdAlmacenOrigen = detalle.Key;
                            movimientoDto.i_IdMoneda = guiaCompra.i_IdMoneda;
                            movimientoDto.i_IdTipoMotivo = 1;
                            movimientoDto.t_Fecha = guiaCompra.t_Fecha;
                            movimientoDto.v_Mes = guiaCompra.v_Mes.Trim();
                            movimientoDto.v_Periodo = guiaCompra.v_Periodo.Trim();
                            movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                            movimientoDto.v_Correlativo = maxMovimiento.ToString("00000000");
                            movimientoDto.v_IdCliente = guiaCompra.v_IdProveedor;
                            movimientoDto.v_OrigenTipo = "G";
                            movimientoDto.i_EsDevolucion = guiaCompra.i_IdTipoDocumento.Value == 7 ? 1 : 0;
                            movimientoDto.v_OrigenRegCorrelativo = guiaCompra.v_Correlativo;
                            movimientoDto.v_OrigenRegMes = guiaCompra.v_Mes;
                            movimientoDto.v_OrigenRegPeriodo = guiaCompra.v_Periodo;
                            movimientoDto.d_TotalPrecio = 0;
                            movimientoDto.i_IdEstablecimiento = 1;
                            movimientoDto.v_IdMovimientoOrigen = guiaCompra.v_IdGuiaCompra;

                            var movimientosDetalleDto = detalle.ToList()
                                .Select(d => new movimientodetalleDto
                                {
                                    v_IdProductoDetalle = d.v_IdProductoDetalle,
                                    i_IdTipoDocumento = -1,
                                    v_NumeroDocumento = string.Empty,
                                    v_NroGuiaRemision = guiaCompra.v_SerieDocumento + "-" + guiaCompra.v_CorrelativoDocumento,
                                    d_Cantidad = d.d_Cantidad ?? 0,
                                    i_IdUnidad = d.i_IdUnidadMedida ?? -1,
                                    d_CantidadEmpaque = d.d_CantidadEmpaque ?? 0,
                                    d_Precio = d.d_PrecioUnitario ?? 0,
                                    d_Total = Utils.Windows.DevuelveValorRedondeado((d.d_PrecioUnitario ?? 0) * (d.d_Cantidad ?? 0), 2),
                                    d_CantidadAdministrativa = d.d_Cantidad,
                                    d_CantidadEmpaqueAdministrativa = d.d_CantidadEmpaque,
                                    t_FechaCaducidad =d.t_FechaCaducidad ,
                                    v_NroSerie =d.v_NroSerie ,
                                    v_NroLote =d.v_NroLote ,

                                }).ToList();

                            movimientoDto.d_TotalCantidad = movimientosDetalleDto.Sum(p => p.d_Cantidad ?? 0);
                            movimientoDto.d_TotalPrecio = movimientosDetalleDto.Sum(p => p.d_Total ?? 0);
                            new MovimientoBL().InsertarMovimiento(ref pobjOperationResult, movimientoDto,
                                Globals.ClientSession.GetAsList(), movimientosDetalleDto);
                            if (pobjOperationResult.Success == 0) return;

                        }
                    }
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionCompraBL.GenerarIngresoAlmacen()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        public static void EliminarIngresoAlmacen(ref OperationResult pobjOperationResult, string pstrIdGuiaCompra)
        {
            pobjOperationResult.Success = 1;
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var movimientosRelacionado = dbContext.movimiento.Where(p => p.v_IdMovimientoOrigen.Equals(pstrIdGuiaCompra) && p.i_Eliminado == 0).ToList();
                        if (movimientosRelacionado.Any())
                        {
                            foreach (var movimientoRef in movimientosRelacionado)
                            {
                                new MovimientoBL().EliminarMovimiento(ref pobjOperationResult, movimientoRef.v_IdMovimiento, Globals.ClientSession.GetAsList());
                                if (pobjOperationResult.Success == 0) return;
                            }
                        }

                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionCompraBL.EliminarIngresoAlmacen()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        private static void RegenerarIngresoAlmacen(ref OperationResult pobjOperationResult, string pstrIdGuiaCompra)
        {
            pobjOperationResult.Success = 1;
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    EliminarIngresoAlmacen(ref pobjOperationResult, pstrIdGuiaCompra);
                    if (pobjOperationResult.Success == 0) return;
                    GenerarIngresoAlmacen(ref pobjOperationResult, pstrIdGuiaCompra);
                    if (pobjOperationResult.Success == 0) return;
                    ts.Complete();
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "GuiaRemisionCompraBL.RegenerarIngresoAlmacen()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        #endregion
    }
}
