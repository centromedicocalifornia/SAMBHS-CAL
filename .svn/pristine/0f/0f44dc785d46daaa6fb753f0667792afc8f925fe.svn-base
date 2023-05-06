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
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel;
using System.Data.Objects;

namespace SAMBHS.CommonWIN.BL
{
    public class EstablecimientoBL
    {
        #region  Cabecera
        public List<establecimientoDto> GetestablecimientoPagedAndFiltered(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from A in dbContext.establecimiento

                            join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                            from J1 in J1_join.DefaultIfEmpty()

                            join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                            from J2 in J2_join.DefaultIfEmpty()

                            where (A.i_Eliminado == 0 || A.i_Eliminado == null)

                            select new establecimientoDto
                            {
                                i_IdEstablecimiento = A.i_IdEstablecimiento,
                                v_Nombre = A.v_Nombre,
                                v_Direccion = A.v_Direccion,
                                i_Eliminado = A.i_Eliminado,
                                v_UsuarioCreacion = J1.v_UserName,
                                v_UsuarioModificacion = J2.v_UserName,
                                t_InsertaFecha = A.t_InsertaFecha,
                                t_ActualizaFecha = A.t_ActualizaFecha,
                                i_IdCentroCosto = A.i_IdCentroCosto

                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                List<establecimientoDto> objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }
        public establecimientoDto Getestablecimiento(ref OperationResult pobjOperationResult, int pintIdEstablecimiento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = (from a in dbContext.establecimiento

                                     join J1 in dbContext.datahierarchy on a.i_IdCentroCosto equals J1.v_Value2 into J1_join
                                     from J1 in J1_join.DefaultIfEmpty()

                                     where a.i_IdEstablecimiento == pintIdEstablecimiento

                                     select new establecimientoDto
                                     {
                                         i_ActualizaIdUsuario = a.i_ActualizaIdUsuario,
                                         i_Eliminado = a.i_Eliminado,
                                         i_IdEstablecimiento = a.i_IdEstablecimiento,
                                         i_IdCentroCosto = a.i_IdCentroCosto,
                                         i_InsertaIdUsuario = a.i_InsertaIdUsuario,
                                         t_InsertaFecha = a.t_InsertaFecha,
                                         CentroCosto = J1.v_Value1,
                                         t_ActualizaFecha = a.t_ActualizaFecha,
                                         v_Direccion = a.v_Direccion,
                                         v_Nombre = a.v_Nombre
                                     }
                                             ).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return objEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }
        public void Addestablecimiento(ref OperationResult pobjOperationResult, establecimientoDto pobjDtoEntity, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    establecimiento objEntity = establecimientoAssembler.ToEntity(pobjDtoEntity);

                    #region Inserta Cabecera
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;

                    dbContext.AddToestablecimiento(objEntity);
                    dbContext.SaveChanges();
                    #endregion

                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "establecimiento", objEntity.v_Nombre);
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "EstablecimientoBL.Addestablecimiento()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }
        public void Updateestablecimiento(ref OperationResult pobjOperationResult, establecimientoDto pobjDtoEntity, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    //Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.establecimiento
                                           where a.i_IdEstablecimiento == pobjDtoEntity.i_IdEstablecimiento
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);

                    establecimiento objEntity = establecimientoAssembler.ToEntity(pobjDtoEntity);

                    // Copiar los valores desde la entidad actualizada a la Entidad Fuente
                    dbContext.establecimiento.ApplyCurrentValues(objEntity);

                    // Guardar los cambios
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;

                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "establecimiento", objEntity.i_IdEstablecimiento.ToString());

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "EstablecimientoBL.Updateestablecimiento()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);

            }
        }
        public void DeleteEstablecimiento(ref OperationResult pobjOperationResult, int pintIdEstablecimiento, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySourceDetalle = (from a in dbContext.establecimientodetalle
                                                  where a.i_IdEstablecimiento == pintIdEstablecimiento
                                                  select a).ToList();

                    foreach (establecimientodetalle Detalle in objEntitySourceDetalle)
                    {
                        Detalle.t_ActualizaFecha = DateTime.Now;
                        Detalle.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                        Detalle.i_Eliminado = 1;
                        dbContext.establecimientodetalle.ApplyCurrentValues(Detalle);
                    }


                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.establecimiento
                                           where a.i_IdEstablecimiento == pintIdEstablecimiento
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;

                    // Guardar los cambios
                    dbContext.SaveChanges();


                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "establecimiento", objEntitySource.i_IdEstablecimiento.ToString());
                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "EstablecimientoBL.DeleteEstablecimiento()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }
        public List<KeyValueDTO> ObtenerEstablecimientosValueDto(ref OperationResult pobjOperationResult, string pstrSortExpression, bool Value1Value2 = false)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from a in dbContext.establecimiento
                                where a.i_Eliminado == 0
                                select a;

                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_Nombre");
                    List<KeyValueDTO> query2;
                    if (Value1Value2)
                        query2 = query.AsEnumerable()
                                        .Select(x => new KeyValueDTO
                                        {
                                            Id = x.i_IdEstablecimiento.ToString(),
                                            Value1 = x.i_IdEstablecimiento.ToString("00"),
                                            Value2 = x.v_Nombre,
                                            Value3 = x.v_Direccion,
                                        }).ToList();
                    else
                        query2 = query.AsEnumerable()
                                    .Select(x => new KeyValueDTO
                                    {
                                        Id = x.i_IdEstablecimiento.ToString(),
                                        Value1 = x.i_IdEstablecimiento.ToString("00") + " | " + x.v_Nombre,
                                        Value3 = x.v_Direccion,
                                    }).ToList();

                    pobjOperationResult.Success = 1;
                    return query2;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "EstablecimientoBL.ObtenerEstablecimientosValueDto()";
                return null;
            }
        }

        public List<KeyValueDTO> ObtenerSerieEstablecimiento(ref OperationResult pobjOperationResult, int establecimiento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var documentos = dbContext.documento.Where(o => o.i_Eliminado == 0 && o.i_UsadoDocumentoContable == 1).ToList();


                    var query = (from a in dbContext.establecimientodetalle
                                 join b in dbContext.documento on new { doc = a.i_IdTipoDocumento.Value, eliminado = 0, docContable = 1 } equals new { doc = b.i_CodigoDocumento, eliminado = b.i_Eliminado.Value, docContable = b.i_UsadoDocumentoContable.Value } into b_join
                                 from b in b_join.DefaultIfEmpty()

                                 where a.i_Eliminado == 0 && a.i_IdEstablecimiento == establecimiento && b.i_UsadoDocumentoContable == 1 //&& b.i_CodigoDocumento == 1
                                 select a).ToList().GroupBy(l => l.v_Serie).Select(l => l.FirstOrDefault()).ToList();


                    List<KeyValueDTO> query2;

                    query2 = query.AsEnumerable()
                                .Select(x => new KeyValueDTO
                                {
                                    Id = x.v_Serie.Trim(),
                                    Value1 = x.v_Serie.Trim(),

                                }).ToList();

                    pobjOperationResult.Success = 1;
                    return query2;
                }

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                pobjOperationResult.AdditionalInformation = "EstablecimientoBL.ObtenerEstablecimientosValueDto()";
                return null;
            }
        }



        public int DevolverEstablecimientoXSerie(int idTipoDoc, string serie)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var xx = (from x in dbContext.establecimientodetalle
                          where x.v_Serie == serie && x.i_Eliminado == 0 && x.i_IdTipoDocumento == idTipoDoc
                          select new { x.i_IdEstablecimiento }).FirstOrDefault();
                if (xx != null)
                {
                    return xx.i_IdEstablecimiento.Value;
                }
                return -1;
            }
        }
        #endregion

        #region  Detalle
        public List<establecimientodetalleDto> GetestablecimientodetallePagedAndFiltered(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, int pintIdEstablecimiento)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = from A in dbContext.establecimientodetalle

                            join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                            from J1 in J1_join.DefaultIfEmpty()

                            join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                            from J2 in J2_join.DefaultIfEmpty()

                            join J3 in dbContext.almacen on new { i_IdAlmacen = A.i_Almacen.Value }
                                                      equals new { i_IdAlmacen = J3.i_IdAlmacen } into J3_join
                            from J3 in J3_join.DefaultIfEmpty()

                            join J4 in dbContext.documento on new { i_CodigoDocumento = A.i_IdTipoDocumento.Value }
                                                      equals new { i_CodigoDocumento = J4.i_CodigoDocumento } into J4_join
                            from J4 in J4_join.DefaultIfEmpty()

                            where (A.i_Eliminado == 0 || A.i_Eliminado == null) && A.i_IdEstablecimiento == pintIdEstablecimiento

                            select new establecimientodetalleDto
                            {
                                i_IdEstablecimientoDetalle = A.i_IdEstablecimientoDetalle,
                                v_Serie = A.v_Serie,
                                i_Correlativo = A.i_Correlativo,
                                i_Eliminado = A.i_Eliminado,
                                v_UsuarioCreacion = J1.v_UserName,
                                v_UsuarioModificacion = J2.v_UserName,
                                t_InsertaFecha = A.t_InsertaFecha,
                                t_ActualizaFecha = A.t_ActualizaFecha,
                                v_Documento = J4.v_Nombre,
                                v_WarehouseName = J3.v_Nombre,
                                i_ImpresionVistaPrevia = A.i_ImpresionVistaPrevia == null ? 0 : A.i_ImpresionVistaPrevia.Value,
                                i_IdEstablecimiento = A.i_IdEstablecimiento,
                                v_NombreImpresora = A.v_NombreImpresora == null ? "" : A.v_NombreImpresora,
                                i_DocumentoPredeterminado = A.i_DocumentoPredeterminado == null ? 0 : A.i_DocumentoPredeterminado.Value,
                                i_NumeroItems = A.i_NumeroItems ?? 0,

                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }

                List<establecimientodetalleDto> objData = query.ToList();
                pobjOperationResult.Success = 1;
                return objData;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }
        public void AddestablecimientoDetalle(ref OperationResult pobjOperationResult, establecimientodetalleDto pobjDtoEntity, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                establecimientodetalle objEntity = establecimientodetalleAssembler.ToEntity(pobjDtoEntity);

                #region Inserta Detalle
                objEntity.t_InsertaFecha = DateTime.Now;
                objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntity.i_Eliminado = 0;

                dbContext.AddToestablecimientodetalle(objEntity);
                dbContext.SaveChanges();
                #endregion

                pobjOperationResult.Success = 1;
                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.CREACION, "DATA HIERARCHY", "GroupId=" + objEntity.i_GroupId.ToString() + " / Descripción = " + objEntity.v_Value1, Success.Ok, null);

                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.CREACION, "DATA HIERARCHY", "GroupId=" + pobjDtoEntity.i_GroupId.ToString() + " / Descripción = " + pobjDtoEntity.v_Value1, Success.Failed, ex.Message);
                return;
            }

        }
        public void UpdateestablecimientoDetalle(ref OperationResult pobjOperationResult, establecimientodetalleDto pobjDtoEntity, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                //Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.establecimientodetalle
                                       where a.i_IdEstablecimientoDetalle == pobjDtoEntity.i_IdEstablecimientoDetalle && a.i_Eliminado == 0
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                pobjDtoEntity.i_Eliminado = 0;

                establecimientodetalle objEntity = establecimientodetalleAssembler.ToEntity(pobjDtoEntity);

                // Copiar los valores desde la entidad actualizada a la Entidad Fuente
                dbContext.establecimientodetalle.ApplyCurrentValues(objEntity);

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;

                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.ACTUALIZACION, "DATA HIERARCHY", "GroupId=" + objEntity.i_GroupId.ToString() + " / Descripción = " + objEntity.v_Value1, Success.Ok, null);
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.ACTUALIZACION, "DATA HIERARCHY", "GroupId=" + pobjDtoEntity.i_GroupId.ToString() + " / Descripción = " + pobjDtoEntity.v_Value1, Success.Failed, ex.Message);
                return;
            }
        }
        public void DeleteestablecimientoDetalle(ref OperationResult pobjOperationResult, int pintIdEstablecimientoDetalle, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.establecimientodetalle
                                       where a.i_IdEstablecimientoDetalle == pintIdEstablecimientoDetalle
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.t_ActualizaFecha = DateTime.Now;
                objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntitySource.i_Eliminado = 1;

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.ACTUALIZACION, "DATA HIERARCHY", "", Success.Ok, null);
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.ACTUALIZACION, "DATA HIERARCHY", "", Success.Failed, ex.Message);
                return;
            }
        }
        public establecimientodetalleDto GetestablecimientoDetalle(ref OperationResult pobjOperationResult, int pintIdEstablecimientoDetalle)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                establecimientodetalleDto objDtoEntity = null;

                var objEntity = (from a in dbContext.establecimientodetalle
                                 where a.i_IdEstablecimientoDetalle == pintIdEstablecimientoDetalle
                                 orderby a.v_Serie ascending
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = establecimientodetalleAssembler.ToDTO(objEntity);

                pobjOperationResult.Success = 1;
                return objDtoEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public establecimientodetalleDto GetestablecimientoDetalle(ref OperationResult pobjOperationResult, int pIntIdEstablecimiento, int pIntDoc)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                establecimientodetalleDto objDtoEntity = null;

                var objEntity = (from a in dbContext.establecimientodetalle
                                 where a.i_IdEstablecimiento == pIntIdEstablecimiento
                                  && a.i_IdTipoDocumento == pIntDoc && a.i_DocumentoPredeterminado == 1
                                  && a.i_Eliminado == 0
                                 select a).FirstOrDefault();

                if (objEntity != null)
                {
                    objDtoEntity = establecimientodetalleAssembler.ToDTO(objEntity);
                    pobjOperationResult.Success = 1;
                }
                return objDtoEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public bool ComprobarExistenciaCorrelativoDocumento(ref OperationResult pobjOperationResult, int pintAlmacen, string pstrSerieDoc, int pintIdTipoDocumento)
        {
            try
            {
                if (pintIdTipoDocumento != -1 && pstrSerieDoc != null && pintAlmacen != -1) // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    var query = (from n in dbContext.establecimientodetalle
                                 where n.i_Eliminado == 0
                                 && n.i_IdTipoDocumento == pintIdTipoDocumento
                                 && n.v_Serie == pstrSerieDoc && n.i_Almacen == pintAlmacen
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
                return false;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return false;
            }
        }
        public bool ComprobarExistenciaSerie(ref OperationResult pobjOperationResult, int pintIdEstablecimiento, string pstrSerieDoc, int pintIdTipoDocumento)
        {
            try
            {
                if (pintIdEstablecimiento != -1 && pstrSerieDoc != null && pintIdTipoDocumento != -1) // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                {
                    SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                    var query = (from n in dbContext.establecimientodetalle

                                 where n.i_Eliminado == 0
                                                  && n.i_IdTipoDocumento == pintIdTipoDocumento
                                     //&& n.i_IdEstablecimiento == pintIdEstablecimiento
                                 && n.v_Serie == pstrSerieDoc
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
                return false;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return false;
            }
        }

        public string ComprobarExistenciaComprobantePredeterminado(ref OperationResult pobjOperationResult, int pintIdEstablecimiento, int pintIdTipoDocumento, establecimientodetalleDto objEstablecimientoDetalle, int Almacen)
        {
            try
            {
                int IdEstablecimientoDetalle = objEstablecimientoDetalle != null ? objEstablecimientoDetalle.i_IdEstablecimientoDetalle : -1;
                pobjOperationResult.Success = 1;
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    if (pintIdEstablecimiento != -1 && pintIdTipoDocumento != -1 && IdEstablecimientoDetalle != -1) // comprueba que tenga los datos necesarios para no consultar a la Bd en vano.
                    {


                        var query = (from n in dbContext.establecimientodetalle

                                     where n.i_Eliminado == 0 && n.i_DocumentoPredeterminado.Value == 1
                                                      && n.i_IdTipoDocumento == pintIdTipoDocumento && n.i_IdEstablecimientoDetalle != IdEstablecimientoDetalle
                                         && n.i_IdEstablecimiento == pintIdEstablecimiento
                                     // && n.v_Serie == pstrSerieDoc
                                     select n
                                 ).FirstOrDefault();
                        if (query == null)
                        {
                            return "";
                        }
                        else
                        {
                            return query.v_Serie.Trim();
                        }

                    }
                    else if (IdEstablecimientoDetalle == -1 && pintIdTipoDocumento != -1)
                    {

                        var query = (from a in dbContext.establecimientodetalle

                                     where a.i_Almacen == Almacen && a.i_Eliminado == 0 && a.i_DocumentoPredeterminado == 1 && a.i_IdEstablecimiento == pintIdEstablecimiento

                                     && a.i_IdTipoDocumento == pintIdTipoDocumento

                                     select a).FirstOrDefault();

                        if (query == null)
                        {
                            return "";
                        }
                        else
                        {
                            return query.v_Serie.Trim();
                        }
                    }

                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return "";
            }

        }

        public string GetestablecimientoDetalleNombreAlmacen(ref OperationResult pobjOperationResult, string pstrSerieDoc, int pintIdTipoDocumento)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                var query = (from A in dbContext.establecimientodetalle
                             join J1 in dbContext.establecimiento on new { i_IdEstablecimiento = A.i_IdEstablecimiento.Value }
                                                         equals new { i_IdEstablecimiento = J1.i_IdEstablecimiento } into J1_join
                             from J1 in J1_join.DefaultIfEmpty()

                             where A.i_Eliminado == 0 && A.i_IdTipoDocumento == pintIdTipoDocumento && A.v_Serie == pstrSerieDoc

                             select new
                             {

                                 J1.v_Nombre

                             }).FirstOrDefault();

                if (query != null)
                {
                    return query.v_Nombre;
                }
                else
                {
                    return null;
                }
            }


            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region Asignar Almacenes


        public List<establecimientoalmacenDto> ObtenerEstablecimientoAlmacen(ref OperationResult pobjOperationResult)
        {
            try
            {
                pobjOperationResult.Success = 1;
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = dbContext.establecimientoalmacen.Where(o => o.i_Eliminado == 0).ToList().ToDTOs();
                    return query;

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                return null;

            }



        }

        public List<establecimientoalmacenDto> GetEstablecimientoAlmacenPagedAndFiltered(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, int pintIdEstablecimiento)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from A in dbContext.establecimientoalmacen

                                join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value }
                                                                equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                                from J1 in J1_join.DefaultIfEmpty()

                                join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value }
                                                                equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                from J2 in J2_join.DefaultIfEmpty()

                                join J3 in dbContext.almacen on new { i_IdAlmacen = A.i_IdAlmacen.Value }
                                                          equals new { i_IdAlmacen = J3.i_IdAlmacen } into J3_join
                                from J3 in J3_join.DefaultIfEmpty()



                                where (A.i_Eliminado == 0 || A.i_Eliminado == null) && A.i_IdEstablecimiento == pintIdEstablecimiento

                                select new establecimientoalmacenDto
                                {
                                    i_IdEstablecimientoAlmacen = A.i_IdEstablecimientoAlmacen,
                                    i_IdEstablecimiento = A.i_IdEstablecimiento,
                                    v_AlmacenNombre = J3.v_Nombre,
                                    t_InsertaFecha = A.t_InsertaFecha,
                                    t_ActualizaFecha = A.t_ActualizaFecha,

                                };

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<establecimientoalmacenDto> objData = query.ToList();
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

        public void AddestablecimientoAlmacen(ref OperationResult pobjOperationResult, establecimientoalmacenDto pobjDtoEntity, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntity = pobjDtoEntity.ToEntity();

                    #region Inserta Detalle
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;

                    dbContext.AddToestablecimientoalmacen(objEntity);
                    dbContext.SaveChanges();
                    #endregion

                    pobjOperationResult.Success = 1;
                    // Llenar entidad Log
                    //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.CREACION, "DATA HIERARCHY", "GroupId=" + objEntity.i_GroupId.ToString() + " / Descripción = " + objEntity.v_Value1, Success.Ok, null);

                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.CREACION, "DATA HIERARCHY", "GroupId=" + pobjDtoEntity.i_GroupId.ToString() + " / Descripción = " + pobjDtoEntity.v_Value1, Success.Failed, ex.Message);
            }

        }

        public void UpdateestablecimientoAlmacen(ref OperationResult pobjOperationResult, establecimientoalmacenDto pobjDtoEntity, List<string> ClientSession)
        {
            try
            {
                //Obtener la entidad fuente
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    var objEntitySource = (from a in dbContext.establecimientoalmacen
                                           where a.i_IdEstablecimientoAlmacen == pobjDtoEntity.i_IdEstablecimientoAlmacen && a.i_Eliminado == 0
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    pobjDtoEntity.i_Eliminado = 0;

                    establecimientoalmacen objEntity = establecimientoalmacenAssembler.ToEntity(pobjDtoEntity);

                    // Copiar los valores desde la entidad actualizada a la Entidad Fuente
                    dbContext.establecimientoalmacen.ApplyCurrentValues(objEntity);

                    // Guardar los cambios
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;
                }

                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.ACTUALIZACION, "DATA HIERARCHY", "GroupId=" + objEntity.i_GroupId.ToString() + " / Descripción = " + objEntity.v_Value1, Success.Ok, null);
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.ACTUALIZACION, "DATA HIERARCHY", "GroupId=" + pobjDtoEntity.i_GroupId.ToString() + " / Descripción = " + pobjDtoEntity.v_Value1, Success.Failed, ex.Message);
            }
        }

        public void DeleteestablecimientoAlmacen(ref OperationResult pobjOperationResult, int pintIdEstablecimientoAlmacen, List<string> ClientSession)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.establecimientoalmacen
                                           where a.i_IdEstablecimientoAlmacen == pintIdEstablecimientoAlmacen
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    objEntitySource.t_ActualizaFecha = DateTime.Now;
                    objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntitySource.i_Eliminado = 1;

                    // Guardar los cambios
                    dbContext.SaveChanges();

                    pobjOperationResult.Success = 1;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
            }
        }

        public establecimientoalmacenDto GetestablecimientoAlmacen(ref OperationResult pobjOperationResult, int pintIdEstablecimientoAlmacen)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    establecimientoalmacenDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.establecimientoalmacen
                                     where a.i_IdEstablecimientoAlmacen == pintIdEstablecimientoAlmacen
                                     select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = objEntity.ToDTO();

                    pobjOperationResult.Success = 1;
                    return objDtoEntity;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public bool ValidarDuplicidadAlmacen(int pintIdEstablecimiento, int pinIdtAlmacen)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var objEntity = (from a in dbContext.establecimientoalmacen
                                 where a.i_IdEstablecimiento == pintIdEstablecimiento && a.i_IdAlmacen == pinIdtAlmacen
                                 && a.i_Eliminado == 0
                                 select a).FirstOrDefault();
                return objEntity != null;
            }
        }

        public bool ValidarSiAlmacenPerteneceAOtroEstablecimiento(int pintIdEstablecimientoAlmacenActual, int pinIdtAlmacen)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var objEntity = (from a in dbContext.establecimientoalmacen
                                 where a.i_IdEstablecimiento != pintIdEstablecimientoAlmacenActual && a.i_IdAlmacen == pinIdtAlmacen
                                 && a.i_Eliminado == 0
                                 select a).FirstOrDefault();
                return objEntity != null;
            }
        }

        public List<KeyValueDTO> GetAlmacenesXEstablecimiento(int pintIdEstablecimiento)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {

                var query = from a in dbContext.establecimientoalmacen
                            where a.i_Eliminado == 0 && a.i_IdEstablecimiento == pintIdEstablecimiento
                            select a;

                var query1 = from a in query
                             join b in dbContext.almacen on a.i_IdAlmacen equals b.i_IdAlmacen
                             select b;

                List<KeyValueDTO> query2 = query1.AsEnumerable()
                    .Select(x => new KeyValueDTO
                    {
                        Id = x.i_IdAlmacen.ToString(),
                        Value1 = x.i_IdAlmacen.ToString("00") + " | " + x.v_Nombre
                    }).ToList();

                return query2;
            }
        }









       
        
        
        
        
        
        
        
        
        
        
        
        
        #endregion

        #region Reporte
        public List<ReporteEstablecimiento> ReporteEstablecimiento()
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                #region Query
                var query =
                        (from A in dbContext.establecimiento

                         where A.i_Eliminado == 0
                         select new ReporteEstablecimiento
                         {
                             i_IdEstablecimiento = A.i_IdEstablecimiento,
                             v_Nombre = A.v_Nombre,
                             v_Direccion = A.v_Direccion,

                         });
                #endregion

                //query = query.OrderBy(pstrt_Orden);


                return query.ToList();


                //List<ventaDto> objData = query.ToList();

                //pobjOperationResult.Success = 1;

            }
            catch (Exception ex)
            {
                //pobjOperationResult.Success = 0;
                //pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public establecimientodetalleDto GetEstablecimiento_Serie(ref OperationResult pobjOperationResult, int pintIdAlmacen)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                establecimientodetalleDto objDtoEntity = null;

                var objEntity = (from a in dbContext.establecimientodetalle
                                 where a.i_Almacen == pintIdAlmacen
                                 orderby a.v_Serie ascending
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = establecimientodetalleAssembler.ToDTO(objEntity);

                pobjOperationResult.Success = 1;

                return objDtoEntity;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }



        public List<establecimientodetalleDto> GetEstablecimiento_SerieReporte(ref OperationResult pobjOperationResult, int pintIdAlmacen)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();
                establecimientodetalleDto objDtoEntity = null;

                var objEntity = (from a in dbContext.establecimientodetalle
                                 where a.i_Almacen == pintIdAlmacen
                                 orderby a.v_Serie ascending
                                 select new establecimientodetalleDto
                                 {
                                     v_Serie = a.v_Serie,
                                     i_IdTipoDocumento = a.i_IdTipoDocumento,

                                 }).ToList();
                pobjOperationResult.Success = 1;
                return objEntity;


            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }



        #endregion

        public List<almacenDto> DevolverTodosAlmacenes(int Establecimiento)
        {
            using (var dbContext = new SAMBHSEntitiesModelWin())
            {
                var objEntity = (from a in dbContext.establecimientoalmacen


                                 join b in dbContext.almacen on new { almacen = a.i_IdAlmacen.Value, eliminado = 0 } equals new { almacen = b.i_IdAlmacen, eliminado = b.i_Eliminado.Value } into b_join

                                 from b in b_join.DefaultIfEmpty()

                                 where a.i_Eliminado == 0 && a.i_IdEstablecimiento == Establecimiento
                                 select new almacenDto
                                 {
                                     i_IdAlmacen = b.i_IdAlmacen,
                                     v_Nombre = b.v_Nombre
                                 }).ToList();

                return objEntity;
            }
        }

        public List<string> ObtenerUsuariosContables()
        {
            using (var dbContext = new SAMBHSEntitiesModel())
            {
                return
                    dbContext.systemuser.Where(p => p.i_IsDeleted == 0 && p.i_UsuarioContable == 1)
                        .Select(o => o.v_UserName).ToList();
            }
        }

        /// <summary>
        /// Re-Asigna los Nro de registros de todos los procesos para que no halla conflicto en la replicación.
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrPeriodo"></param>
        public void RecalcularNroRegistros(ref OperationResult pobjOperationResult, string pstrPeriodo, int pintMes = -1, bool soloCompras = false, bool soloVentas = false)
        {
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        var _movimientos = dbContext.movimiento.Where(p => p.i_Eliminado == 0).ToList();

                        if (pintMes == -1)
                        {
                            if (!soloVentas)
                            {
                                if (!soloCompras)
                                {
                                    #region Transferencias entre Almacenes

                                    {
                                    }

                                    #endregion

                                    #region IngresosSalidasAlmacen

                                    {
                                        var listadoMovimiento =
                                            dbContext.movimiento.Where(p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0)
                                                .ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoMovimiento.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_Fecha.Value.Month,
                                                            establecimiento = p.v_IdMovimiento.Substring(2, 2),
                                                            tipoMovimiento = p.i_IdTipoMovimiento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes = operacionesPorMes.OrderBy(o => o.t_Fecha).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdMovimiento.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_Fecha.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.movimiento.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Diarios
                                    {
                                        var listadoDiarios =
                                            dbContext.diario.Where(p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0)
                                                .ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoDiarios.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_Fecha.Value.Month,
                                                            establecimiento = p.v_IdDiario.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes = operacionesPorMes.OrderBy(o => o.t_Fecha).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdDiario.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_Fecha.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.diario.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Tesorerias

                                    {
                                        var listadoTesorerias =
                                            dbContext.tesoreria.Where(p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0)
                                                .ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoTesorerias.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaRegistro.Value.Month,
                                                            establecimiento = p.v_IdTesoreria.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaRegistro).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdTesoreria.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.tesoreria.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Cobranzas

                                    {
                                        var listadoCobranzas =
                                            dbContext.cobranza.Where(p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0)
                                                .ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoCobranzas.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaRegistro.Value.Month,
                                                            establecimiento = p.v_IdCobranza.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaRegistro).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdCobranza.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.cobranza.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Pagos

                                    {
                                        var listadoPagos =
                                            dbContext.pago.Where(p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0)
                                                .ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoPagos.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaRegistro.Value.Month,
                                                            establecimiento = p.v_IdPago.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaRegistro).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdPago.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.pago.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region ReciboHonorarios

                                    {
                                        var listadoRecibosHonorarios =
                                            dbContext.recibohonorario.Where(
                                                p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0).ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoRecibosHonorarios.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaRegistro.Value.Month,
                                                            establecimiento = p.v_IdReciboHonorario.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaRegistro).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdReciboHonorario.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.recibohonorario.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region GuiaRemisionVenta

                                    {
                                        var listadoGuiaRemisionVenta =
                                            dbContext.guiaremision.Where(
                                                p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0).ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoGuiaRemisionVenta.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaEmision.Value.Month,
                                                            establecimiento = p.v_IdGuiaRemision.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaEmision).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdGuiaRemision.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaEmision.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.guiaremision.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region OrdenCompra

                                    {
                                        var listadoOrdenCompra =
                                            dbContext.ordendecompra.Where(
                                                p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0).ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoOrdenCompra.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaRegistro.Value.Month,
                                                            establecimiento = p.v_IdOrdenCompra.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaRegistro).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdOrdenCompra.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.ordendecompra.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region PedidosCotizacion

                                    {
                                        var listadoPedidosCotizacion =
                                            dbContext.pedido.Where(p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0)
                                                .ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoPedidosCotizacion.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaEmision.Value.Month,
                                                            establecimiento = p.v_IdPedido.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaEmision).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdPedido.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaEmision.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.pedido.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion
                                }

                                #region Compras
                                {
                                    var listadoCompras =
                                        dbContext.compra.Where(p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0).ToList();

                                    foreach (
                                        var operacionesPorMes in
                                            listadoCompras.GroupBy(
                                                p =>
                                                    new
                                                    {
                                                        mes = p.t_FechaRegistro.Value.Month,
                                                        establecimiento = p.v_IdCompra.Substring(2, 2)
                                                    }))
                                    {
                                        var counter = 1;
                                        var listaOperacionesPorMes = operacionesPorMes.OrderBy(o => o.t_FechaEmision).ThenBy(p => p.v_IdCompra).ToList();
                                        foreach (var operacion in listaOperacionesPorMes)
                                        {
                                            var idAlmacen = int.Parse(operacion.v_IdCompra.Substring(2, 2));
                                            var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                            var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                            var movimientoRelacionado =
                                                _movimientos.FirstOrDefault(
                                                    p =>
                                                        p.v_OrigenTipo == "C" && p.v_OrigenRegPeriodo == pstrPeriodo &&
                                                        p.v_OrigenRegMes.Trim() == operacion.v_Mes.Trim() &&
                                                        p.v_OrigenRegCorrelativo == operacion.v_Correlativo);

                                            operacion.v_Mes = mes;
                                            operacion.v_Correlativo = correlativo;
                                            dbContext.compra.ApplyCurrentValues(operacion);

                                            if (movimientoRelacionado != null)
                                            {
                                                movimientoRelacionado.v_OrigenRegMes = mes;
                                                movimientoRelacionado.v_OrigenRegCorrelativo = correlativo;
                                                dbContext.movimiento.ApplyCurrentValues(movimientoRelacionado);
                                            }

                                            counter++;
                                        }
                                    }
                                }

                                #endregion

                                var _counter = 1;
                                #region Liquidacion de Compras

                                {
                                    var listadoCompras =
                                        dbContext.liquidacioncompra.Where(
                                            p =>
                                                p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0 &&
                                                p.t_FechaRegistro.Value.Month == pintMes).ToList();

                                    foreach (
                                        var operacionesPorMes in
                                            listadoCompras.GroupBy(
                                                p =>
                                                    new
                                                    {
                                                        mes = p.t_FechaRegistro.Value.Month,
                                                        establecimiento = p.v_IdLiquidacionCompra.Substring(2, 2)
                                                    }))
                                    {
                                        var listaOperacionesPorMes =
                                            operacionesPorMes.OrderBy(o => o.t_FechaEmision).ToList();
                                        foreach (var operacion in listaOperacionesPorMes)
                                        {
                                            var idAlmacen = int.Parse(operacion.v_IdLiquidacionCompra.Substring(2, 2));
                                            var correlativo = idAlmacen.ToString("00") + _counter.ToString("000000");
                                            var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                            var movimientoRelacionado =
                                                _movimientos.FirstOrDefault(
                                                    p =>
                                                        p.v_OrigenTipo == "C" && p.v_OrigenRegPeriodo == pstrPeriodo &&
                                                        p.v_OrigenRegMes.Trim() == operacion.v_Mes.Trim() &&
                                                        p.v_OrigenRegCorrelativo == operacion.v_Correlativo);

                                            operacion.v_Mes = mes;
                                            operacion.v_Correlativo = correlativo;
                                            dbContext.liquidacioncompra.ApplyCurrentValues(operacion);

                                            if (movimientoRelacionado != null)
                                            {
                                                movimientoRelacionado.v_OrigenRegMes = mes;
                                                movimientoRelacionado.v_OrigenRegCorrelativo = correlativo;
                                                dbContext.movimiento.ApplyCurrentValues(movimientoRelacionado);
                                            }

                                            _counter++;
                                        }
                                    }
                                }

                                #endregion
                            }
                            else if (!soloCompras)
                            {
                                #region Ventas

                                {
                                    var listadoVentas =
                                        dbContext.venta.Where(p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0)
                                            .ToList();

                                    foreach (
                                        var operacionesPorMes in
                                            listadoVentas.GroupBy(
                                                p =>
                                                    new
                                                    {
                                                        mes = p.t_FechaRegistro.Value.Month,
                                                        establecimiento = p.v_IdVenta.Substring(2, 2)
                                                    }))
                                    {
                                        var counter = 1;
                                        var listaOperacionesPorMes = operacionesPorMes.OrderBy(x => x.i_IdTipoDocumento).ThenBy(o => o.v_SerieDocumento)
                                            .ThenBy(p => p.v_CorrelativoDocumento).ToList();
                                        foreach (var operacion in listaOperacionesPorMes)
                                        {
                                            var idAlmacen = int.Parse(operacion.v_IdVenta.Substring(2, 2));
                                            var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                            var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                            var movimientoRelacionado =
                                                _movimientos.FirstOrDefault(
                                                    p =>
                                                        p.v_OrigenTipo == "V" && p.v_OrigenRegPeriodo == pstrPeriodo &&
                                                        p.v_OrigenRegMes.Trim() == operacion.v_Mes.Trim() &&
                                                        p.v_OrigenRegCorrelativo == operacion.v_Correlativo);

                                            operacion.v_Mes = mes;
                                            operacion.v_Correlativo = correlativo;
                                            dbContext.venta.ApplyCurrentValues(operacion);

                                            if (movimientoRelacionado != null)
                                            {
                                                movimientoRelacionado.v_OrigenRegMes = mes;
                                                movimientoRelacionado.v_OrigenRegCorrelativo = correlativo;
                                                dbContext.movimiento.ApplyCurrentValues(movimientoRelacionado);
                                            }
                                            counter++;
                                        }
                                    }
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            if (!soloVentas)
                            {
                                if (!soloCompras)
                                {
                                    #region Transferencias entre Almacenes

                                    {
                                        var listadoTransferencias =
                                            dbContext.movimiento.Where(
                                                p =>
                                                    p.v_Periodo == pstrPeriodo && p.i_IdTipoMovimiento == 3 &&
                                                    p.i_Eliminado == 0 && p.t_Fecha.Value.Month == pintMes).ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoTransferencias.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_Fecha.Value.Month,
                                                            establecimiento = p.v_IdMovimiento.Substring(2, 2)
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes = operacionesPorMes.OrderBy(o => o.t_Fecha).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdMovimiento.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_Fecha.Value.Month.ToString("00");

                                                var movimientoRelacionados =
                                                    _movimientos.Where(
                                                        p =>
                                                            p.v_OrigenTipo == "T" && p.v_OrigenRegPeriodo == pstrPeriodo &&
                                                            p.v_OrigenRegMes.Trim() == operacion.v_Mes.Trim() &&
                                                            p.v_OrigenRegCorrelativo == operacion.v_Correlativo);

                                                foreach (var movimientoRelacionado in movimientoRelacionados)
                                                {
                                                    movimientoRelacionado.v_OrigenRegMes = mes;
                                                    movimientoRelacionado.v_OrigenRegCorrelativo = correlativo;
                                                    dbContext.movimiento.ApplyCurrentValues(movimientoRelacionado);
                                                }

                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region IngresosSalidasAlmacen

                                    {
                                        var listadoMovimiento =
                                            dbContext.movimiento.Where(
                                                p =>
                                                    p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0 &&
                                                    p.t_Fecha.Value.Month == pintMes).ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoMovimiento.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_Fecha.Value.Month,
                                                            establecimiento = p.v_IdMovimiento.Substring(2, 2),
                                                            tipoMovimiento = p.i_IdTipoMovimiento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes = operacionesPorMes.OrderBy(o => o.t_Fecha).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdMovimiento.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_Fecha.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.movimiento.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Diarios

                                    {
                                        var listadoDiarios =
                                            dbContext.diario.Where(
                                                p =>
                                                    p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0 &&
                                                    p.t_Fecha.Value.Month == pintMes).ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoDiarios.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_Fecha.Value.Month,
                                                            establecimiento = p.v_IdDiario.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes = operacionesPorMes.OrderBy(o => o.t_Fecha).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdDiario.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_Fecha.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.diario.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Tesorerias

                                    {
                                        var listadoTesorerias =
                                            dbContext.tesoreria.Where(
                                                p =>
                                                    p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0 &&
                                                    p.t_FechaRegistro.Value.Month == pintMes).ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoTesorerias.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaRegistro.Value.Month,
                                                            establecimiento = p.v_IdTesoreria.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaRegistro).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdTesoreria.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.tesoreria.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Cobranzas

                                    {
                                        var listadoCobranzas =
                                            dbContext.cobranza.Where(
                                                p =>
                                                    p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0 &&
                                                    p.t_FechaRegistro.Value.Month == pintMes).ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoCobranzas.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaRegistro.Value.Month,
                                                            establecimiento = p.v_IdCobranza.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaRegistro).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdCobranza.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.cobranza.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Pagos

                                    {
                                        var listadoPagos =
                                            dbContext.pago.Where(
                                                p =>
                                                    p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0 &&
                                                    p.t_FechaRegistro.Value.Month == pintMes).ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoPagos.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaRegistro.Value.Month,
                                                            establecimiento = p.v_IdPago.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaRegistro).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdPago.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.pago.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region ReciboHonorarios

                                    {
                                        var listadoRecibosHonorarios =
                                            dbContext.recibohonorario.Where(
                                                p =>
                                                    p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0 &&
                                                    p.t_FechaRegistro.Value.Month == pintMes).ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoRecibosHonorarios.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaRegistro.Value.Month,
                                                            establecimiento = p.v_IdReciboHonorario.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaRegistro).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdReciboHonorario.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.recibohonorario.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region GuiaRemisionVenta

                                    {
                                        var listadoGuiaRemisionVenta =
                                            dbContext.guiaremision.Where(
                                                p =>
                                                    p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0 &&
                                                    p.t_FechaEmision.Value.Month == pintMes).ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoGuiaRemisionVenta.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaEmision.Value.Month,
                                                            establecimiento = p.v_IdGuiaRemision.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaEmision).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdGuiaRemision.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaEmision.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.guiaremision.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region OrdenCompra

                                    {
                                        var listadoOrdenCompra =
                                            dbContext.ordendecompra.Where(
                                                p =>
                                                    p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0 &&
                                                    p.t_FechaRegistro.Value.Month == pintMes).ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoOrdenCompra.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaRegistro.Value.Month,
                                                            establecimiento = p.v_IdOrdenCompra.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaRegistro).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdOrdenCompra.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.ordendecompra.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion

                                    #region PedidosCotizacion

                                    {
                                        var listadoPedidosCotizacion =
                                            dbContext.pedido.Where(
                                                p =>
                                                    p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0 &&
                                                    p.t_FechaEmision.Value.Month == pintMes).ToList();

                                        foreach (
                                            var operacionesPorMes in
                                                listadoPedidosCotizacion.GroupBy(
                                                    p =>
                                                        new
                                                        {
                                                            mes = p.t_FechaEmision.Value.Month,
                                                            establecimiento = p.v_IdPedido.Substring(2, 2),
                                                            tipo = p.i_IdTipoDocumento
                                                        }))
                                        {
                                            var counter = 1;
                                            var listaOperacionesPorMes =
                                                operacionesPorMes.OrderBy(o => o.t_FechaEmision).ToList();
                                            foreach (var operacion in listaOperacionesPorMes)
                                            {
                                                var idAlmacen = int.Parse(operacion.v_IdPedido.Substring(2, 2));
                                                var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                                var mes = operacion.t_FechaEmision.Value.Month.ToString("00");

                                                operacion.v_Mes = mes;
                                                operacion.v_Correlativo = correlativo;
                                                dbContext.pedido.ApplyCurrentValues(operacion);
                                                counter++;
                                            }
                                        }
                                    }

                                    #endregion
                                }

                                var _counter = 1;
                                #region Compras
                                {
                                    var listadoCompras =
                                        dbContext.compra.Where(p => p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0 && p.t_FechaRegistro.Value.Month == pintMes).ToList();

                                    foreach (
                                        var operacionesPorMes in
                                            listadoCompras.GroupBy(
                                                p =>
                                                    new
                                                    {
                                                        mes = p.t_FechaRegistro.Value.Month,
                                                        establecimiento = p.v_IdCompra.Substring(2, 2)
                                                    }))
                                    {
                                        var listaOperacionesPorMes = operacionesPorMes.OrderBy(o => o.t_FechaEmision).ThenBy(p => p.v_IdCompra).ToList();
                                        foreach (var operacion in listaOperacionesPorMes)
                                        {
                                            var idAlmacen = int.Parse(operacion.v_IdCompra.Substring(2, 2));
                                            var correlativo = idAlmacen.ToString("00") + _counter.ToString("000000");
                                            var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                            var movimientoRelacionado =
                                                _movimientos.FirstOrDefault(
                                                    p =>
                                                        p.v_OrigenTipo == "C" && p.v_OrigenRegPeriodo == pstrPeriodo &&
                                                        p.v_OrigenRegMes.Trim() == operacion.v_Mes.Trim() &&
                                                        p.v_OrigenRegCorrelativo == operacion.v_Correlativo);

                                            operacion.v_Mes = mes;
                                            operacion.v_Correlativo = correlativo;
                                            dbContext.compra.ApplyCurrentValues(operacion);

                                            if (movimientoRelacionado != null)
                                            {
                                                movimientoRelacionado.v_OrigenRegMes = mes;
                                                movimientoRelacionado.v_OrigenRegCorrelativo = correlativo;
                                                dbContext.movimiento.ApplyCurrentValues(movimientoRelacionado);
                                            }

                                            _counter++;
                                        }
                                    }
                                }

                                #endregion

                                #region Liquidacion de Compras

                                {
                                    var listadoCompras =
                                        dbContext.liquidacioncompra.Where(
                                            p =>
                                                p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0 &&
                                                p.t_FechaRegistro.Value.Month == pintMes).ToList();

                                    foreach (
                                        var operacionesPorMes in
                                            listadoCompras.GroupBy(
                                                p =>
                                                    new
                                                    {
                                                        mes = p.t_FechaRegistro.Value.Month,
                                                        establecimiento = p.v_IdLiquidacionCompra.Substring(2, 2)
                                                    }))
                                    {
                                        var listaOperacionesPorMes =
                                            operacionesPorMes.OrderBy(o => o.t_FechaEmision).ToList();
                                        foreach (var operacion in listaOperacionesPorMes)
                                        {
                                            var idAlmacen = int.Parse(operacion.v_IdLiquidacionCompra.Substring(2, 2));
                                            var correlativo = idAlmacen.ToString("00") + _counter.ToString("000000");
                                            var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                            var movimientoRelacionado =
                                                _movimientos.FirstOrDefault(
                                                    p =>
                                                        p.v_OrigenTipo == "C" && p.v_OrigenRegPeriodo == pstrPeriodo &&
                                                        p.v_OrigenRegMes.Trim() == operacion.v_Mes.Trim() &&
                                                        p.v_OrigenRegCorrelativo == operacion.v_Correlativo);

                                            operacion.v_Mes = mes;
                                            operacion.v_Correlativo = correlativo;
                                            dbContext.liquidacioncompra.ApplyCurrentValues(operacion);

                                            if (movimientoRelacionado != null)
                                            {
                                                movimientoRelacionado.v_OrigenRegMes = mes;
                                                movimientoRelacionado.v_OrigenRegCorrelativo = correlativo;
                                                dbContext.movimiento.ApplyCurrentValues(movimientoRelacionado);
                                            }

                                            _counter++;
                                        }
                                    }
                                }

                                #endregion}
                            }
                            else if (!soloCompras)
                            {
                                #region Ventas

                                {
                                    var listadoVentas =
                                        dbContext.venta.Where(
                                            p =>
                                                p.v_Periodo == pstrPeriodo && p.i_Eliminado == 0 &&
                                                p.t_FechaRegistro.Value.Month == pintMes).ToList();

                                    foreach (
                                        var operacionesPorMes in
                                            listadoVentas.GroupBy(
                                                p =>
                                                    new
                                                    {
                                                        mes = p.t_FechaRegistro.Value.Month,
                                                        establecimiento = p.v_IdVenta.Substring(2, 2)
                                                    }))
                                    {
                                        var counter = 1;
                                        var listaOperacionesPorMes = operacionesPorMes.OrderBy(x => x.i_IdTipoDocumento).ThenBy(o => o.v_SerieDocumento)
                                            .ThenBy(p => p.v_CorrelativoDocumento).ToList();
                                        foreach (var operacion in listaOperacionesPorMes)
                                        {
                                            var idAlmacen = int.Parse(operacion.v_IdVenta.Substring(2, 2));
                                            var correlativo = idAlmacen.ToString("00") + counter.ToString("000000");
                                            var mes = operacion.t_FechaRegistro.Value.Month.ToString("00");

                                            var movimientoRelacionado =
                                                _movimientos.FirstOrDefault(
                                                    p =>
                                                        p.v_OrigenTipo == "V" && p.v_OrigenRegPeriodo == pstrPeriodo &&
                                                        p.v_OrigenRegMes.Trim() == operacion.v_Mes.Trim() &&
                                                        p.v_OrigenRegCorrelativo == operacion.v_Correlativo);

                                            operacion.v_Mes = mes;
                                            operacion.v_Correlativo = correlativo;
                                            dbContext.venta.ApplyCurrentValues(operacion);

                                            if (movimientoRelacionado != null)
                                            {
                                                movimientoRelacionado.v_OrigenRegMes = mes;
                                                movimientoRelacionado.v_OrigenRegCorrelativo = correlativo;
                                                dbContext.movimiento.ApplyCurrentValues(movimientoRelacionado);
                                            }
                                            counter++;
                                        }
                                    }
                                }

                                #endregion
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
                pobjOperationResult.AdditionalInformation = "EstablecimientoBL.RecalcularNroRegistros()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }


       
    
    }
}
