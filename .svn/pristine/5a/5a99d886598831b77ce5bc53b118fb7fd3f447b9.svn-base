using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using SAMBHS.CommonWIN.BL;



namespace SAMBHS.Compra.BL
{
    public class GastosImportacionBL
    {
      
        
     public gastosimportacionDto ObtenerGastosImportacion(ref OperationResult pobjOperationResult, string pstrGastosImportacionId)
     {

            try
            {
                using (var dbContext = new SAMBHSEntitiesModelWin())
                {
                    gastosimportacionDto objDtoEntity = null;

                    var objEntity = (from a in dbContext.gastosimportacion
                                     where a.v_GastoImportacion == pstrGastosImportacionId
                                     select a).FirstOrDefault();

                    if (objEntity != null)
                        objDtoEntity = gastosimportacionAssembler.ToDTO(objEntity);


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



     public List<gastosimportacionDto> ListarGastosImportacion(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression)
     {

         try
         {
             using (var dbContext = new SAMBHSEntitiesModelWin())
             {
                 var query = (from n in dbContext.gastosimportacion

                              join J1 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertaIdUsuario.Value }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                              from J1 in J1_join.DefaultIfEmpty()

                              join J2 in dbContext.systemuser on new { i_UpdateUserId = n.i_ActualizaIdUsuario.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                              from J2 in J2_join.DefaultIfEmpty()

                              where n.i_Eliminado == 0


                              select new gastosimportacionDto
                              {

                                  v_GastoImportacion = n.v_GastoImportacion,
                                  v_Codigo = n.v_Codigo,
                                  v_Nombre = n.v_Nombre,
                                  v_Cuenta = n.v_Cuenta,
                                  v_CCuenta = n.v_CCuenta,
                                  i_Eliminado = n.i_Eliminado,
                                  t_InsertaFecha = n.t_InsertaFecha,
                                  i_InsertaIdUsuario = n.i_InsertaIdUsuario,
                                  i_ActualizaIdUsuario = n.i_ActualizaIdUsuario,
                                  t_ActualizaFecha = n.t_ActualizaFecha,
                                  v_UsuarioCreacion = J1.v_UserName,
                                  v_UsuarioModificacion = J2.v_UserName,

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

                 List<gastosimportacionDto> objData = query.ToList();

                 pobjOperationResult.Success = 1;

                 return objData; 
             }
         }
         catch (Exception ex)
         {
             pobjOperationResult.Success = 0;

             pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
             return null;
         }
     }



     public void InsertarGastosImportacion(ref OperationResult pobjOperationResult, gastosimportacionDto pobjDtoEntity, List<string> ClientSession)
     {
         int SecuentialId = 0;
         string newId = string.Empty;

         try
         {
             using (var dbContext = new SAMBHSEntitiesModelWin())
             {
                 SecuentialBL objSecuentialBL = new SecuentialBL();
                 gastosimportacion objEntity = gastosimportacionAssembler.ToEntity(pobjDtoEntity);
                 objEntity.t_InsertaFecha = DateTime.Now;
                 objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                 objEntity.i_Eliminado = 0;


                 // Autogeneramos el Pk de la tabla
                 int intNodeId = int.Parse(ClientSession[0]);
                 SecuentialId = objSecuentialBL.GetNextSecuentialId(intNodeId, 28);
                 newId = Utils.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "GI");
                 objEntity.v_GastoImportacion = newId;
                 dbContext.AddTogastosimportacion(objEntity);
                 dbContext.SaveChanges();
                 pobjOperationResult.Success = 1; 
             }
         }
         catch (Exception ex)
         {
             pobjOperationResult.Success = 0;
             pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
         }
     }


     public void ActualizarGastosImportacion(ref OperationResult pobjOperationResult, gastosimportacionDto  pobjDtoEntity, List<string> ClientSession)
     {
         //mon.IsActive = true;
         try
         {
             using (var dbContext = new SAMBHSEntitiesModelWin())
             {
                 // Obtener la entidad fuente
                 var objEntitySource = (from a in dbContext.gastosimportacion
                                        where a.v_GastoImportacion == pobjDtoEntity.v_GastoImportacion
                                        select a).FirstOrDefault();

                 // Crear la entidad con los datos actualizados
                 pobjDtoEntity.t_ActualizaFecha = DateTime.Now;
                 pobjDtoEntity.i_ActualizaIdUsuario = Int32.Parse(ClientSession[2]);
                 gastosimportacion objEntity = gastosimportacionAssembler.ToEntity(pobjDtoEntity);

                 dbContext.gastosimportacion.ApplyCurrentValues(objEntity);

                 // Guardar los cambios
                 dbContext.SaveChanges();

                 pobjOperationResult.Success = 1; 
             }
         }
         catch (Exception ex)
         {
             pobjOperationResult.Success = 0;
             pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
         }
     }



     public void EliminarGastosImportacion(ref OperationResult pobjOperationResult, string pstrGastosImportacionId, List<string> ClientSession)
     {
         //mon.IsActive = true;
         try
         {
             using (var dbContext = new SAMBHSEntitiesModelWin())
             {
                 // Obtener la entidad fuente
                 var objEntitySource = (from a in dbContext.gastosimportacion
                                        where a.v_GastoImportacion == pstrGastosImportacionId
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
             pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
         }
     }


     public gastosimportacionDto ObtenerCodigo(ref OperationResult pobjOperatioResult, string pstrCodigo, string pstringIdGastosImportacion)
     {

         try
         {
             using (var dbContext = new SAMBHSEntitiesModelWin())
             {
                 gastosimportacionDto objDtoEntity = null;
                 if (pstringIdGastosImportacion == null)
                 {
                     var objEntity = (from A in dbContext.gastosimportacion
                                      where A.v_Codigo == pstrCodigo & A.i_Eliminado == 0
                                      select A
                                      ).FirstOrDefault();
                     if (objEntity != null)
                         objDtoEntity = gastosimportacionAssembler.ToDTO(objEntity);

                     pobjOperatioResult.Success = 1;
                     return objDtoEntity;
                 }
                 else
                 {
                     var objEntity = (from A in dbContext.gastosimportacion
                                      where A.v_Codigo == pstrCodigo & A.i_Eliminado == 0
                                      & A.v_GastoImportacion != pstringIdGastosImportacion
                                      select A
                                         ).FirstOrDefault();
                     if (objEntity != null)
                         objDtoEntity = gastosimportacionAssembler.ToDTO(objEntity);

                     pobjOperatioResult.Success = 1;
                     return objDtoEntity;

                 } 
             }
         }
         catch (Exception ex)
         {
             pobjOperatioResult.Success = 0;
             pobjOperatioResult.ExceptionMessage = ex.Message;
             return null;
         }

     }



    }
}
