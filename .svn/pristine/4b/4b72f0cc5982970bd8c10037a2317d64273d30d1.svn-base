using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System.Linq.Dynamic;

namespace SAMBHS.Contabilidad.BL
{
    public class ConceptoBL
    {

        public List<conceptoDto> ObtenerListadoConcepto(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {
            try
            {
               var  Periodo= Globals.ClientSession.i_Periodo.ToString ();
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {

                    var query = from A in dbContext.concepto

                                join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value, eliminado = 0 }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId, eliminado = J1.i_IsDeleted.Value } into J1_join
                                from J1 in J1_join.DefaultIfEmpty()

                                join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value, eliminado = 0 }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId, eliminado = J2.i_IsDeleted.Value } into J2_join
                                from J2 in J2_join.DefaultIfEmpty()

                                join J3 in dbContext.datahierarchy on new { a = A.i_IdArea.Value, b = 28, eliminado = 0 } //Grupo 28
                                                                 equals new { a = J3.i_ItemId, b = J3.i_GroupId, eliminado = J3.i_IsDeleted.Value } into J3_join
                                from J3 in J3_join.DefaultIfEmpty()

                                where A.i_Eliminado == 0 && A.v_Periodo == Periodo

                                select new conceptoDto
                                {

                                    v_IdConcepto = A.v_IdConcepto,
                                    v_Codigo = A.v_Codigo,
                                    v_Nombre = A.v_Nombre,
                                    i_IdArea = A.i_IdArea,
                                    i_Eliminado = A.i_Eliminado,
                                    i_InsertaIdUsuario = A.i_InsertaIdUsuario,
                                    i_ActualizaIdUsuario = A.i_ActualizaIdUsuario,
                                    t_InsertaFecha = A.t_InsertaFecha,
                                    v_UsuarioCreacion = J1.v_UserName,
                                    v_UsuarioModificacion = J2.v_UserName,
                                    t_ActualizaFecha = A.t_ActualizaFecha,
                                    v_NombreArea = J3.v_Value1
                                };

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<conceptoDto> objData = query.ToList();
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


        public void EliminarConcepto(ref OperationResult pobjOperationResult, string pstrIdConcepto, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.concepto
                                       where a.v_IdConcepto  == pstrIdConcepto
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.t_ActualizaFecha = DateTime.Now;
                objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntitySource.i_Eliminado = 1;
                dbContext.SaveChanges();
                Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "concepto", objEntitySource.v_IdConcepto);
                pobjOperationResult.Success = 1;
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptoBL.EliminarConcepto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }


        public string InsertarConcepto(ref OperationResult pobjOperationResult, conceptoDto pobjDtoEntity, List<string> ClientSession)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    concepto objEntity = conceptoAssembler.ToEntity(pobjDtoEntity);
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;


                    // Autogeneramos el Pk de la tabla
                    int intNodeId = int.Parse(ClientSession[0]);
                    var SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 34);
                    var newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZT");
                    objEntity.v_IdConcepto = newId;
                    dbContext.AddToconcepto(objEntity);
                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "concepto", newId);
                    return newId; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptoBL.InsertarConcepto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return string.Empty;
            }
        }

        public string ActualizarConcepto(ref OperationResult pobjOperationResult, conceptoDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.concepto
                                           where a.v_IdConcepto == pobjDtoEntity.v_IdConcepto
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    concepto objEntity = conceptoAssembler.ToEntity(pobjDtoEntity);

                    dbContext.concepto.ApplyCurrentValues(objEntity);

                    // Guardar los cambios
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "concepto", objEntitySource.v_IdConcepto);
                    pobjOperationResult.Success = 1;
                    return pobjDtoEntity.v_IdConcepto; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptoBL.ActualizarConcepto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null ;
            }
        }


        public conceptoDto ObtenerConcepto(ref OperationResult pobjOperationResult, string pstrConceptoId)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    conceptoDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.concepto
                                     where a.v_IdConcepto == pstrConceptoId && a.i_Eliminado == 0
                                     select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = conceptoAssembler.ToDTO(objEntity);

                    pobjOperationResult.Success = 1;
                    return objDtoEntity; 
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }

        }


        public conceptoDto ObtenerCodigoConcepto(ref OperationResult pobjOperationResult, string pstrcodConcepto)
        {

            try
            {
                string Periodo =Globals.ClientSession.i_Periodo.ToString ();
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    conceptoDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.concepto 
                                     where a.v_Codigo == pstrcodConcepto && a.i_Eliminado == 0  && a.v_Periodo ==Periodo 
                                     select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = conceptoAssembler.ToDTO(objEntity);

                    pobjOperationResult.Success = 1;
                    return objDtoEntity; 
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
}
