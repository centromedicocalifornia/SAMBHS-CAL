using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;

namespace SAMBHS.Common.BL
{
   public class OrganizationBL
   {

        #region Organization
       public organizationDto GetorganizationByorganizationId(ref OperationResult pobjOperationResult, int pstrorganizationId)
       {
           try
           {
               SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
               organizationDto objDtoEntity = null;

               var objEntity = (from a in dbContext.organization
                                where a.i_OrganizationId == pstrorganizationId
                                select a).FirstOrDefault();

               if (objEntity != null)
                   objDtoEntity = objEntity.ToDTO();

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

       public List<organizationDto> GetorganizationPagedAndFiltered(ref OperationResult pobjOperationResult, int? pintPageIndex, int? pintResultsPerPage, string pstrSortExpression, string pstrFilterExpression)
       {

           try
           {
               SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

               var query = (from n in dbContext.organization

                            join su2 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertUserId.Value }
                                                          equals new { i_InsertUserId = su2.i_SystemUserId } into su2_join
                            from su2 in su2_join.DefaultIfEmpty()

                            join su3 in dbContext.systemuser on new { i_UpdateUserId = n.i_UpdateUserId.Value }
                                                          equals new { i_UpdateUserId = su3.i_SystemUserId } into su3_join
                            from su3 in su3_join.DefaultIfEmpty()
                            where n.i_IsDeleted == 0


                            select new organizationDto
                            {
                                i_OrganizationId = n.i_OrganizationId,
                                v_Name = n.v_Name,
                                v_Address = n.v_Address,
                                v_IdentificationNumber = n.v_IdentificationNumber,
                                v_ContacName = n.v_ContacName,
                                v_Mail = n.v_Mail,
                                v_Observation = n.v_Observation,
                                v_PhoneNumber = n.v_PhoneNumber,
                                d_InsertDate = n.d_InsertDate,
                                d_UpdateDate = n.d_UpdateDate,
                                v_InsertUser = su2.v_UserName,
                                v_UpdateUser = su3.v_UserName

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
               if (pintPageIndex.HasValue && pintResultsPerPage.HasValue)
               {
                   int intStartRowIndex = pintPageIndex.Value * pintResultsPerPage.Value;
                   query = query.Skip(intStartRowIndex);
               }
               if (pintResultsPerPage.HasValue)
               {
                   query = query.Take(pintResultsPerPage.Value);
               }

               List<organizationDto> objData = query.ToList();
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

       public int GetorganizationCount(ref OperationResult pobjOperationResult, string filterExpression)
       {
           //mon.IsActive = true;
           try
           {
               SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
               var query = from a in dbContext.organization select a;

               string _filterEx = "i_IsDeleted==0";
               query = query.Where(_filterEx);

               if (!string.IsNullOrEmpty(filterExpression))
                   query = query.Where(filterExpression);

               int intResult = query.Count();

               pobjOperationResult.Success = 1;
               return intResult;
           }
           catch (Exception ex)
           {
               pobjOperationResult.Success = 0;
               pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
               return 0;
           }
       }

       public void Addorganization(ref OperationResult pobjOperationResult, organizationDto pobjDtoEntity, List<string> ClientSession)
       {
           int SecuentialId = 0;
           string newId = string.Empty;
           //mon.IsActive = true;

           try
           {
               SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
               organization objEntity = organizationAssembler.ToEntity(pobjDtoEntity);

               objEntity.d_InsertDate = DateTime.Now;
               objEntity.i_InsertUserId = Int32.Parse(ClientSession[2]);
               objEntity.i_IsDeleted = 0;

               // Autogeneramos el Pk de la tabla
               int intNodeId = int.Parse(ClientSession[0]);

               dbContext.AddToorganization(objEntity);
               dbContext.SaveChanges();

               pobjOperationResult.Success = 1;
               // Llenar entidad Log
               return;
           }
           catch (Exception ex)
           {
               pobjOperationResult.Success = 0;
               pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
               // Llenar entidad Log
               return;
           }
       }

       public void Updateorganization(ref OperationResult pobjOperationResult, organizationDto pobjDtoEntity, List<string> ClientSession)
       {
           //mon.IsActive = true;
           try
           {
               SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

               // Obtener la entidad fuente
               var objEntitySource = (from a in dbContext.organization
                                      where a.i_OrganizationId == pobjDtoEntity.i_OrganizationId
                                      select a).FirstOrDefault();

               // Crear la entidad con los datos actualizados
               pobjDtoEntity.d_UpdateDate = DateTime.Now;
               pobjDtoEntity.i_UpdateUserId = Int32.Parse(ClientSession[2]);

               organization objEntity = organizationAssembler.ToEntity(pobjDtoEntity);

               // Copiar los valores desde la entidad actualizada a la Entidad Fuente
               dbContext.organization.ApplyCurrentValues(objEntity);

               // Guardar los cambios
               dbContext.SaveChanges();

               pobjOperationResult.Success = 1;
               // Llenar entidad Log
               return;
           }
           catch (Exception ex)
           {
               pobjOperationResult.Success = 0;
               pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
               // Llenar entidad Log
                return;
           }
       }

       public void Deleteorganization(ref OperationResult pobjOperationResult, int pstrorganizationId, List<string> ClientSession)
       {
           //mon.IsActive = true;
           try
           {
               SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

               // Obtener la entidad fuente
               var objEntitySource = (from a in dbContext.organization
                                      where a.i_OrganizationId == pstrorganizationId
                                      select a).FirstOrDefault();

               // Crear la entidad con los datos actualizados
               objEntitySource.d_UpdateDate = DateTime.Now;
               objEntitySource.i_UpdateUserId = Int32.Parse(ClientSession[2]);
               objEntitySource.i_IsDeleted = 1;


               // Guardar los cambios
               dbContext.SaveChanges();

               pobjOperationResult.Success = 1;
               // Llenar entidad Log
               return;
           }
           catch (Exception ex)
           {
               pobjOperationResult.Success = 0;
               pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
               // Llenar entidad Log
               return;
           }
       }

       #endregion       

        #region KeyValueDTO

        public static List<KeyValueDTO> GetAllOrganization(ref OperationResult pobjOperationResult,string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = from a in dbContext.organization
                            where a.i_IsDeleted == 0
                            select a;

                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }
                else
                {
                    query = query.OrderBy("v_Name");
                }

                var query2 = query.AsEnumerable()
                            .Select(x => new KeyValueDTO
                            {
                                Id = x.i_OrganizationId.ToString(),
                                Value1 = x.v_Name
                            }).ToList();

                pobjOperationResult.Success = 1;
                return query2;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

 
        #endregion
   }
}
