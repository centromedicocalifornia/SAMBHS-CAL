using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Common.DataModel;
using System.Linq.Dynamic;
using System.Data;
using System.ComponentModel;
using System.Transactions;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.OrdenProduccion.BL
{
    public class OrdenProduccionBL
    {

        public List<ordenproduccionDto> ListarBusquedaOrdenProduccion(ref OperationResult pobjOperationResult, string pstrSortExpression,
            DateTime F_Inicio, DateTime F_Fin)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                List<ordenproduccionDto> OrdenProduccion = new List<ordenproduccionDto>();

                OrdenProduccion = (from a in dbContext.ordenproduccion
                                   join b in dbContext.systemuser on new { i_UpdateUserId = a.i_ActualizaIdUsuario.Value }
                                equals new { i_UpdateUserId = b.i_SystemUserId } into b_join
                                   from b in b_join.DefaultIfEmpty()

                                   join c in dbContext.systemuser on new { i_InsertUserId = a.i_InsertaIdUsuario.Value }
                                       equals new { i_InsertUserId = c.i_SystemUserId } into c_join
                                   from c in c_join.DefaultIfEmpty()
                                   join d in dbContext.productodetalle on new { pd = a.v_IdProductoDetalle } equals new { pd = d.v_IdProductoDetalle } into d_join
                                   from d in d_join.DefaultIfEmpty()
                                   where a.i_Eliminado == 0 && a.t_FechaRegistro >= F_Inicio && a.t_FechaRegistro <= F_Fin

                                   select new ordenproduccionDto
                                   {
                                       i_IdOrdenProduccion = a.i_IdOrdenProduccion,
                                       NombreProducto = d.producto.v_Descripcion,
                                       t_FechaRegistro = a.t_FechaRegistro,
                                       v_Observacion = a.v_Observacion,
                                       UsuarioCreacion = c.v_UserName,
                                       UsuarioModificacion = b.v_UserName,
                                       NroRegistro = a.v_Mes + "-" + a.v_Correlativo,
                                       t_InsertaFecha = a.t_InsertaFecha,
                                       t_ActualizaFecha = a.t_ActualizaFecha,

                                   }).ToList();




                List<ordenproduccionDto> objData = OrdenProduccion.OrderBy(k => k.i_IdOrdenProduccion).ToList();
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



        public void EliminarOrdenProduccion(ref OperationResult pobjOperationResult, int IdOrdenProduccion, List<string> clientSession)
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
                    var objEntitySource = (from a in dbContext.ordenproduccion
                                           where a.i_IdOrdenProduccion == IdOrdenProduccion
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(clientSession[2]);
                    objEntitySource.i_Eliminado = 1;
                    #endregion
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "ordenproduccion",
                        IdOrdenProduccion.ToString());
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



        public bool ExisteNroRegistro(string Periodo, string Mes, string Correlativo)
        {
            SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
            string replicationID = Globals.ClientSession.ReplicationNodeID;
            var Registro = (from n in dbContext.ordenproduccion
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


        public ordenproduccionDto ObtenerOrdenProduccion(ref OperationResult objOperationResult, int IdOrdenProduccion)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                var objEntity = (from a in dbContext.ordenproduccion
                                 join b in dbContext.productodetalle on new { pd = a.v_IdProductoDetalle, eliminado = 0 } equals new { pd = b.v_IdProductoDetalle, eliminado = b.i_Eliminado.Value } into b_join
                                 from b in b_join.DefaultIfEmpty()
                                 where a.i_IdOrdenProduccion == IdOrdenProduccion && a.i_Eliminado == 0

                                 select new ordenproduccionDto
                                 {
                                     i_IdOrdenProduccion = a.i_IdOrdenProduccion,
                                     v_IdProductoDetalle = a.v_IdProductoDetalle,
                                     NombreProducto = b.producto.v_Descripcion.Trim(),
                                     CodigoProducto = b.producto.v_CodInterno.Trim(),
                                     v_Mes = a.v_Mes,
                                     v_Periodo = a.v_Periodo,
                                     v_Correlativo = a.v_Correlativo,
                                     t_FechaRegistro = a.t_FechaRegistro,
                                     t_FechaInicio = a.t_FechaInicio,
                                     t_FechaTermino = a.t_FechaTermino,
                                     t_InsertaFecha = a.t_InsertaFecha,
                                     i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                     t_ActualizaFecha = a.t_ActualizaFecha,
                                     i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                     i_Eliminado = a.i_Eliminado,
                                     d_Cantidad = a.d_Cantidad ?? 0,
                                     d_CantidadUnidadMedida = a.d_CantidadUnidadMedida ?? 0,
                                     v_Observacion = a.v_Observacion,
                                 }
                                 ).FirstOrDefault();

                objOperationResult.Success = 1;

                return objEntity;
            }
            catch (Exception ex)
            {
                

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ProduccionBL.ObtenerOrdenProduccion()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            }
        }


        public BindingList<ordenproducciondocumentosDto> ObtenerOrdenProduccionDocumentos(ref  OperationResult objOperationResult, int IdOrdenProduccion)
        {
            try
            {
                objOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {

                    var ordenproduccionDocumentos = (from a in dbContext.ordenproducciondocumentos

                                                     where a.i_Eliminado == 0 && a.i_IdOrdenProduccion == IdOrdenProduccion

                                                     select new ordenproducciondocumentosDto
                                                     {
                                                         i_IdOrdenProduccionDocumentos =a.i_IdOrdenProduccionDocumentos ,
                                                         i_IdOrdenProduccion =a.i_IdOrdenProduccion ,
                                                         i_IdTipoDocumento =a.i_IdTipoDocumento ,
                                                         v_SerieDocumento =a.v_SerieDocumento ,
                                                         v_CorrelativoDocumento =a.v_CorrelativoDocumento ,
                                                         i_Eliminado = a.i_Eliminado,
                                                         i_InsertaIdUsuario =a.i_InsertaIdUsuario ,
                                                         i_ActualizaIdUsuario =a.i_ActualizaIdUsuario ,
                                                         t_ActualizaFecha =a.t_ActualizaFecha ,
                                                         t_InsertaFecha =a.t_InsertaFecha ,
                                                         i_RegistroEstado ="NoModificado",
                                                         i_RegistroTipo ="NoTemporal",
                                                     }).ToList();

                    var jjj = new  BindingList<ordenproducciondocumentosDto>(ordenproduccionDocumentos);
                    return jjj;
                   
                   
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ProduccionBL.ObtenerOrdenProduccionDocumentos()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return null;
            
            }
        }




        public List<KeyValueDTO> ObtenerListadoOrdenProduccion(ref OperationResult pobjOperationResult, string pstringPeriodo, string pstringMes)
        {
            try
            {
                SAMBHSEntitiesModelWin dbcontext = new SAMBHSEntitiesModelWin();
                var almacenpredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00");
                string replicationID = Globals.ClientSession.ReplicationNodeID;
                var query = (from n in dbcontext.ordenproduccion
                             where n.i_Eliminado == 0 && n.v_Periodo == pstringPeriodo && n.v_Mes == pstringMes
                             orderby n.v_Correlativo ascending
                             select new
                             {
                                 v_Correlativo = n.v_Correlativo,
                                 i_IdOrdenProduccion = n.i_IdOrdenProduccion,
                             }
                             );

                if (query.Any())
                {
                    var query2 = query.AsEnumerable()
                                .Select(x => new KeyValueDTO
                                {
                                    Value1 = x.v_Correlativo,
                                    Value2 = x.i_IdOrdenProduccion.ToString()
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






        public int InsertarOrdenProduccion(ref OperationResult objOperationResult, ordenproduccionDto pobjDtoEntity, List<ordenproducciondocumentosDto> pTemp_Insertar  , List<string> ClientSession)
        {
            try
            {


                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {
                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {


                        ordenproduccion objEntityOrdenProduccion = ordenproduccionAssembler.ToEntity(pobjDtoEntity);
                        SecuentialBL objSecuentialBL = new SecuentialBL();
                        int intNodeId;
                        objEntityOrdenProduccion.t_InsertaFecha = DateTime.Now;
                        objEntityOrdenProduccion.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntityOrdenProduccion.i_Eliminado = 0;
                        // Autogeneramos el Pk de la tabla
                        intNodeId = int.Parse(ClientSession[0]);
                        dbContext.AddToordenproduccion(objEntityOrdenProduccion);
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "OrdenProduccion", objEntityOrdenProduccion.i_IdOrdenProduccion.ToString());


                        #region Inserta Detalle

                        foreach (ordenproducciondocumentosDto ordenproducciondocumentosDto in pTemp_Insertar)
                        {
                            ordenproducciondocumentos objEntityOrdenProduccionDocumentos = ordenproducciondocumentosAssembler.ToEntity(ordenproducciondocumentosDto);
                            objEntityOrdenProduccionDocumentos.i_IdOrdenProduccion = objEntityOrdenProduccion.i_IdOrdenProduccion;
                            objEntityOrdenProduccionDocumentos.t_InsertaFecha = DateTime.Now;
                            objEntityOrdenProduccionDocumentos.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityOrdenProduccionDocumentos.i_Eliminado = 0;
                            dbContext.AddToordenproducciondocumentos(objEntityOrdenProduccionDocumentos);
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "OrdenProduccionDocumentos", objEntityOrdenProduccionDocumentos.i_IdOrdenProduccionDocumentos.ToString ());
                        }
    
                        #endregion
                        dbContext.SaveChanges();
                        objOperationResult.Success = 1;
                        ts.Complete();
                        return objEntityOrdenProduccion.i_IdOrdenProduccion;
                    }
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "OrdenProduccionBL.InsertarOrdenProduccion()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return 0;
            }
        }

        public int ActualizarOrdenProduccion(ref OperationResult objOperationResult, ordenproduccionDto pobjDtoEntity, List<ordenproducciondocumentosDto> pTemp_Insertar ,List<ordenproducciondocumentosDto> pTemp_Editar,List<ordenproducciondocumentosDto> pTemp_Eliminar,  List<string> ClientSession)
        {
            try
            {


                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                {


                    ordenproduccion objEntityReciboHonorario = ordenproduccionAssembler.ToEntity(pobjDtoEntity);

                    using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                    {
                        string newIdReciboHonorarioDetalle = string.Empty;
                        int intNodeId;
                        intNodeId = int.Parse(ClientSession[0]);
                        var objEntitySource = (from a in dbContext.ordenproduccion
                                               where a.i_IdOrdenProduccion == pobjDtoEntity.i_IdOrdenProduccion
                                               select a).FirstOrDefault();

                        pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                        pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        ordenproduccion objEntity = ordenproduccionAssembler.ToEntity(pobjDtoEntity);
                        dbContext.ordenproduccion.ApplyCurrentValues(objEntity);
                        dbContext.SaveChanges();


                        #region Actualiza Detalle
                        foreach (ordenproducciondocumentosDto ordenproducciondocumentosDto in pTemp_Insertar)
                        {
                            ordenproducciondocumentos objEntityOrdenProduccionDocumentos = ordenproducciondocumentosAssembler.ToEntity(ordenproducciondocumentosDto);


                            objEntityOrdenProduccionDocumentos.i_IdOrdenProduccion = objEntitySource.i_IdOrdenProduccion;
                            objEntityOrdenProduccionDocumentos.t_InsertaFecha = DateTime.Now;
                            objEntityOrdenProduccionDocumentos.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                            objEntityOrdenProduccionDocumentos.i_Eliminado = 0;
                            dbContext.AddToordenproducciondocumentos(objEntityOrdenProduccionDocumentos);
                            dbContext.SaveChanges();
                            Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "OrdenProduccionDocumentos", objEntityOrdenProduccionDocumentos.i_IdOrdenProduccionDocumentos.ToString ());

                        }


                        foreach (ordenproducciondocumentosDto ordenproducciondocumentosDto in pTemp_Editar)
                        {
                            ordenproducciondocumentos _objEntity = ordenproducciondocumentosAssembler.ToEntity(ordenproducciondocumentosDto);
                            var query = (from n in dbContext.ordenproducciondocumentos
                                         where n.i_IdOrdenProduccionDocumentos == ordenproducciondocumentosDto.i_IdOrdenProduccionDocumentos
                                         select n).FirstOrDefault();

                            _objEntity.t_ActualizaFecha = DateTime.Now;
                            _objEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                            dbContext.ordenproducciondocumentos.ApplyCurrentValues(_objEntity);
                            Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "OrdenProduccionDocumentos", query.i_IdOrdenProduccionDocumentos.ToString ());
                        }

                        foreach (ordenproducciondocumentosDto ordenproducciondocumentosDto in pTemp_Eliminar)
                        {
                            ordenproducciondocumentos _objEntity = ordenproducciondocumentosAssembler.ToEntity(ordenproducciondocumentosDto);
                            var query = (from n in dbContext.ordenproducciondocumentos
                                         where n.i_IdOrdenProduccionDocumentos == ordenproducciondocumentosDto.i_IdOrdenProduccionDocumentos
                                         select n).FirstOrDefault();

                            if (query != null)
                            {
                                query.t_ActualizaFecha = DateTime.Now;
                                query.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                                query.i_Eliminado = 1;
                            }

                            dbContext.ordenproducciondocumentos.ApplyCurrentValues(query);
                            Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "OrdenProduccionDocumentos", query.i_IdOrdenProduccionDocumentos.ToString ());
                        }
                        #endregion
                        objOperationResult.Success = 1;
                        dbContext.SaveChanges();
                        Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "OrdenProduccion", pobjDtoEntity.i_IdOrdenProduccion.ToString());
                        ts.Complete();
                        return pobjDtoEntity.i_IdOrdenProduccion;
                    }
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "OrdenProduccionBL.ActualizarOrdenProduccion()\nLinea:" +
                                                            ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null
                    ? ex.InnerException.Message
                    : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);
                return 0;
            }
        }










    }

}
