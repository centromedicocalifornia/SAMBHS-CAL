using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.DataModel;
using System.ComponentModel;
using System.Linq.Dynamic;

namespace SAMBHS.Venta.BL
{
    public class ConceptosChicaBL
    {
        private static string periodo = Globals.ClientSession != null ? (Globals.ClientSession.i_Periodo ?? DateTime.Now.Year).ToString() : DateTime.Now.Year.ToString();
        public int InsertarConceptoCajaChica(ref OperationResult pobjOperationResult, conceptoscajachicaDto pobjDtoEntity, List<string> ClientSession)
        {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    SecuentialBL objSecuentialBL = new SecuentialBL();
                    conceptoscajachica objEntity = conceptoscajachicaAssembler.ToEntity(pobjDtoEntity);
                    objEntity.t_InsertaFecha = DateTime.Now;
                    objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                    objEntity.i_Eliminado = 0;
                    // Autogeneramos el Pk de la tabla
                    //int intNodeId = int.Parse(ClientSession[0]);
                    //var SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 34);
                    //var newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "ZT");
                    dbContext.AddToconceptoscajachica(objEntity);


                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;
                    Utils.Windows.GeneraHistorial(LogEventType.CREACION, Globals.ClientSession.v_UserName, "conceptoscajachica", "");
                    return objEntity.i_IdConceptosCajaChica;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptosCajaChicaBL.InsertarConceptoCajaChica()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return -1;
            }
        }
        public int ActualizarConceptoCajaChica(ref OperationResult pobjOperationResult, conceptoscajachicaDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    // Obtener la entidad fuente
                    var objEntitySource = (from a in dbContext.conceptoscajachica
                                           where a.i_IdConceptosCajaChica == pobjDtoEntity.i_IdConceptosCajaChica
                                           select a).FirstOrDefault();

                    // Crear la entidad con los datos actualizados
                    pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                    pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                    conceptoscajachica objEntity = conceptoscajachicaAssembler.ToEntity(pobjDtoEntity);

                    dbContext.conceptoscajachica.ApplyCurrentValues(objEntity);

                    // Guardar los cambios
                    dbContext.SaveChanges();
                    Utils.Windows.GeneraHistorial(LogEventType.ACTUALIZACION, Globals.ClientSession.v_UserName, "conceptoscajachica", objEntitySource.i_IdConceptosCajaChica.ToString());
                    pobjOperationResult.Success = 1;
                    return pobjDtoEntity.i_IdConceptosCajaChica;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptosCajaChicaBL.ActualizarConcepto()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return -1;
            }
        }
        public conceptoscajachicaDto ObtenerConceptoCajaChica(ref OperationResult pobjOperationResult, int icodConcepto)
        {

            try
            {

                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    //conceptoscajachicaDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.conceptoscajachica
                                     join b in dbContext.systemuser on new { i_UpdateUserId = a.i_ActualizaIdUsuario.Value } equals new { i_UpdateUserId = b.i_SystemUserId } into b_join
                                     from b in b_join.DefaultIfEmpty()

                                     join c in dbContext.systemuser on new { i_InsertUserId = a.i_InsertaIdUsuario.Value } equals new { i_InsertUserId = c.i_SystemUserId } into c_join
                                     from c in c_join.DefaultIfEmpty()

                                     join d in dbContext.asientocontable on new {c=a.v_NroCuenta ,periodo =periodo } equals new {c= d.v_NroCuenta ,periodo= d.v_Periodo } into d_join
                                     from d in d_join.DefaultIfEmpty ()

                                     where a.i_IdConceptosCajaChica == icodConcepto && a.i_Eliminado == 0
                                     select new conceptoscajachicaDto
                                     {
                                         i_IdConceptosCajaChica = a.i_IdConceptosCajaChica,
                                         v_NombreConceptoCajaChica = a.v_NombreConceptoCajaChica,
                                         v_NroCuenta = a.v_NroCuenta,
                                         v_UsuarioCreacion = c.v_UserName,
                                         v_UsuarioModificacion = b.v_UserName,
                                         i_Eliminado = a.i_Eliminado,
                                         t_InsertaFecha = a.t_InsertaFecha,
                                         t_ActualizaFecha = a.t_ActualizaFecha,
                                         NombreCuenta =d.v_NombreCuenta ,
                                         i_RequiereAnexo =a.i_RequiereAnexo ??0,
                                         i_RequiereTipoDocumento =a.i_RequiereTipoDocumento ??0,
                                         i_RequiereNumeroDocumento =a.i_RequiereNumeroDocumento ??0,

                                     }).FirstOrDefault();
                    //if (objEntity != null)
                    //    objDtoEntity = conceptoscajachicaAssembler.ToDTO(objEntity);

                    pobjOperationResult.Success = 1;
                    return objEntity;
                }
            }
            catch (Exception ex)
            {



                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptosCajaChicaBL.ObtenerConceptoCajaChica()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;

            }

        }
        public List<conceptoscajachicaDto> ObtenerListadoConceptosCajaChica(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = from A in dbContext.conceptoscajachica

                                join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                                from J1 in J1_join.DefaultIfEmpty()

                                join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                                from J2 in J2_join.DefaultIfEmpty()
                                

                                where A.i_Eliminado == 0

                                select new conceptoscajachicaDto
                               {
                                   i_IdConceptosCajaChica = A.i_IdConceptosCajaChica,
                                   v_NombreConceptoCajaChica = A.v_NombreConceptoCajaChica,
                                   v_NroCuenta = A.v_NroCuenta,
                                   i_Eliminado = A.i_Eliminado,
                                   i_InsertaIdUsuario = A.i_InsertaIdUsuario,
                                   i_ActualizaIdUsuario = A.i_ActualizaIdUsuario,
                                   t_InsertaFecha = A.t_InsertaFecha,
                                   v_UsuarioCreacion = J1.v_UserName,
                                   v_UsuarioModificacion = J2.v_UserName,
                                   t_ActualizaFecha = A.t_ActualizaFecha,
                                   i_RequiereAnexo =A.i_RequiereAnexo ??0,
                                   i_RequiereTipoDocumento =A.i_RequiereTipoDocumento ??0,
                                   i_RequiereNumeroDocumento =A.i_RequiereNumeroDocumento ??0,
                                 

                               };

                    if (!string.IsNullOrEmpty(pstrFilterExpression))
                    {
                        query = query.Where(pstrFilterExpression);
                    }
                    if (!string.IsNullOrEmpty(pstrSortExpression))
                    {
                        query = query.OrderBy(pstrSortExpression);
                    }

                    List<conceptoscajachicaDto> objData = query.ToList();
                    pobjOperationResult.Success = 1;
                    return objData;
                }
            }
            catch (Exception ex)
            {

                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptosCajaChicaBL.ObtenerListadoConceptosCajaChica()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }
        public bool ExistenciaConceptosDiversasCajaChicaDetalle(int IdConceptoCajaChica)
        {
            using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
            {

                var CajaChicaDetalle = (from a in dbContext.cajachicadetalle
                                        where a.i_Eliminado == 0 && a.i_IdConceptosCajaChica == IdConceptoCajaChica
                                        select a).ToList();

                if (CajaChicaDetalle.Any())
                {

                    return true;

                }
                else
                {
                    return false;
                }

            }
        }
        public void EliminarConceptosCajaChica(ref OperationResult pobjOperationResult,int  IdConceptoCajaChica, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.conceptoscajachica
                                       where a.i_IdConceptosCajaChica == IdConceptoCajaChica && a.i_Eliminado ==0
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.t_ActualizaFecha = DateTime.Now;
                objEntitySource.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                objEntitySource.i_Eliminado = 1;
                dbContext.SaveChanges();
                Utils.Windows.GeneraHistorial(LogEventType.ELIMINACION, Globals.ClientSession.v_UserName, "conceptoscajachica", objEntitySource.i_IdConceptosCajaChica.ToString ());
                pobjOperationResult.Success = 1;
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ConceptosCajaChicaBL.EliminarConceptosCajaChica()";
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return;
            }
        }


        public List<GridKeyValueDTO> ObtenerConceptosCajaChica(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (SAMBHSEntitiesModelWin dbContext = new SAMBHSEntitiesModelWin())
                {
                    var query = (from a in dbContext.conceptoscajachica 

                                 where a.i_Eliminado == 0
                                 select a).Distinct();

                  

                    var query2 = query.AsEnumerable()
                                .Select(x => new GridKeyValueDTO
                                {
                                    Id = x.i_IdConceptosCajaChica.ToString(),
                                    Value1 = x.v_NombreConceptoCajaChica ,
                                    //Value2 = x.v_NombreConceptoCajaChica,
                                }).ToList();

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



    
    }
}
