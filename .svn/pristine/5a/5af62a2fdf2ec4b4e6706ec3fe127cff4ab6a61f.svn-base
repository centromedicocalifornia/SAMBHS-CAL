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
    public class RoleBL
    {
        public roleDto GetRoleById(ref OperationResult pobjOperationResult, int pintRoleId)
        {
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                roleDto objDtoEntity = null;

                var objEntity = (from a in dbContext.role
                                 where a.i_RoleId == pintRoleId
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = roleAssembler.ToDTO(objEntity);

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

        public bool CheckIfRoleExistsByName(string pstrRoleName)
        {
            using (var dbContext = new SAMBHSEntitiesModel())
            {
                return dbContext.role.ToList().Any(p => p.i_IsDeleted == 0 && string.Equals(p.v_Name.Trim(), pstrRoleName.Trim(), StringComparison.CurrentCultureIgnoreCase));
            }
        }

        public List<roleDto> GetRolePagedAndFiltered(ref OperationResult pobjOperationResult, int? pintPageIndex, int? pintResultsPerPage, string pstrSortExpression, string pstrFilterExpression)
        {

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = (from n in dbContext.role

                             join su2 in dbContext.systemuser on new { i_InsertUserId = n.i_InsertUserId.Value }
                                                           equals new { i_InsertUserId = su2.i_SystemUserId } into su2_join
                             from su2 in su2_join.DefaultIfEmpty()

                             join su3 in dbContext.systemuser on new { i_UpdateUserId = n.i_UpdateUserId.Value }
                                                           equals new { i_UpdateUserId = su3.i_SystemUserId } into su3_join
                             from su3 in su3_join.DefaultIfEmpty()
                             where n.i_IsDeleted == 0

                             select new roleDto
                             {
                                 i_RoleId = n.i_RoleId,
                                 v_Name = n.v_Name,
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

                List<roleDto> objData = query.ToList();
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

        public int GetRoleCount(ref OperationResult pobjOperationResult, string filterExpression)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                var query = from a in dbContext.role select a;

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

        public void AddRole(ref OperationResult pobjOperationResult, roleDto pobjDtoEntity, List<string> ClientSession)
        {
            int SecuentialId = 0;
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                role objEntity = roleAssembler.ToEntity(pobjDtoEntity);

                objEntity.d_InsertDate = DateTime.Now;
                objEntity.i_InsertUserId = Int32.Parse(ClientSession[2]);
                objEntity.i_IsDeleted = 0;


                // Autogeneramos el Pk de la tabla

                dbContext.AddTorole(objEntity);
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

        public void UpdateRole(ref OperationResult pobjOperationResult, roleDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.role
                                       where a.i_RoleId == pobjDtoEntity.i_RoleId
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.d_UpdateDate = DateTime.Now;
                pobjDtoEntity.i_UpdateUserId = Int32.Parse(ClientSession[2]);


                role objEntity = roleAssembler.ToEntity(pobjDtoEntity);

                // Copiar los valores desde la entidad actualizada a la Entidad Fuente
                dbContext.role.ApplyCurrentValues(objEntity);

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

        public void DeleteRole(ref OperationResult pobjOperationResult, int pintRoleId, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.role
                                       where a.i_RoleId == pintRoleId
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

    }
}
