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
    public class SystemParameterBL
    {
        #region Metodos SystemParameter
 
        public List<systemparameterDto> GetSystemParametersPagedAndFiltered(ref OperationResult pobjOperationResult, int? pintPageIndex, int? pintResultsPerPage, string pstrSortExpression, string pstrFilterExpression, int pintGroupId)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                var query = from A in dbContext.systemparameter
                            join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertUserId.Value }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                            from J1 in J1_join.DefaultIfEmpty()

                            join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_UpdateUserId.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                            from J2 in J2_join.DefaultIfEmpty()

                            //join J3 in dbContext.systemparameter on new { ItemId = A.i_ParentGroupId.Value }
                            //                                 equals new { ItemId = J3.i_ParameterId } into J3_join
                            //from J3 in J3_join.DefaultIfEmpty()

                            //join J4 in dbContext.systemparameter on new { ItemId = A.i_ParentParameterId.Value, groupId = A.i_ParentGroupId.Value }
                            //                              equals new { ItemId = J4.i_ParameterId, groupId = J4.i_GroupId } into J4_join
                            //from J4 in J4_join.DefaultIfEmpty()

                            select new systemparameterDto
                            {
                                i_GroupId = A.i_GroupId,
                                i_ParameterId = A.i_ParameterId,
                                i_ParentGroupId = A.i_GroupId,
                                i_ParentParameterId = A.i_ParentParameterId.Value,
                                v_Value1 = A.v_Value1,
                                v_Value2 = A.v_Value2,
                                v_CreationUser = J1.v_UserName,
                                v_UpdateUser = J2.v_UserName,
                                d_CreationDate = A.d_InsertDate,
                                d_UpdateDate = A.d_UpdateDate,
                                i_IsDeleted = A.i_IsDeleted
                            };

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
                    query = query.Take(pintResultsPerPage.Value);
                }

                List<systemparameterDto> objData = query.ToList();
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



        public List<systemparameterDto> GetSystemParametersxGrupo(ref OperationResult pobjOperationResult, string pstrSortExpression, string pstrFilterExpression, int pintGroupId)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                var query = from A in dbContext.systemparameter
                            join J1 in dbContext.systemuser on new { i_InsertUserId = A.i_InsertUserId.Value }
                                                            equals new { i_InsertUserId = J1.i_SystemUserId } into J1_join
                            from J1 in J1_join.DefaultIfEmpty()

                            join J2 in dbContext.systemuser on new { i_UpdateUserId = A.i_UpdateUserId.Value }
                                                            equals new { i_UpdateUserId = J2.i_SystemUserId } into J2_join
                            from J2 in J2_join.DefaultIfEmpty()

                            where A.i_GroupId ==pintGroupId && A.i_IsDeleted ==0 
                          
                            select new systemparameterDto
                            {
                                i_GroupId = A.i_GroupId,
                                i_ParameterId = A.i_ParameterId,
                                i_ParentGroupId = A.i_GroupId,
                                i_ParentParameterId = A.i_ParentParameterId.Value,
                                v_Value1 = A.v_Value1,
                                v_Value2 = A.v_Value2,
                                v_CreationUser = J1.v_UserName,
                                v_UpdateUser = J2.v_UserName,
                                d_CreationDate = A.d_InsertDate,
                                d_UpdateDate = A.d_UpdateDate,
                                i_IsDeleted = A.i_IsDeleted,
                                v_Value3 =A.v_Value3 ,
                                v_Field =A.v_Field ,
                                
                            };

                if (!string.IsNullOrEmpty(pstrFilterExpression))
                {
                    query = query.Where(pstrFilterExpression);
                }
                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }
                //if (pintPageIndex.HasValue && pintResultsPerPage.HasValue)
                //{
                //    int intStartRowIndex = pintPageIndex.Value * pintResultsPerPage.Value;
                //    query = query.Skip(intStartRowIndex);
                //    query = query.Take(pintResultsPerPage.Value);
                //}

                List<systemparameterDto> objData = query.ToList();
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



        public int GetSystemParametersCount(ref OperationResult pobjOperationResult, string filterExpression)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                var query = from a in dbContext.systemparameter select a;

                if (!string.IsNullOrEmpty(filterExpression))
                    query = query.Where(filterExpression);

                int intResult = query.Count();

                pobjOperationResult.Success = 1;
                return intResult;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return 0;
            }
        }

        public systemparameterDto GetSystemParameter(ref OperationResult pobjOperationResult, int pintGroupId, int pintParameterId)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                systemparameterDto objDtoEntity = null;

                var objEntity = (from a in dbContext.systemparameter
                                 where a.i_GroupId == pintGroupId && a.i_ParameterId == pintParameterId
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = systemparameterAssembler.ToDTO(objEntity);

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

        public void AddSystemParameter(ref OperationResult pobjOperationResult, systemparameterDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                systemparameter objEntity = systemparameterAssembler.ToEntity(pobjDtoEntity);

                objEntity.d_InsertDate = DateTime.Now;
                objEntity.i_InsertUserId = Int32.Parse(ClientSession[2]);
                objEntity.i_IsDeleted = 0;

             
                dbContext.AddTosystemparameter(objEntity);
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;

                // Llenar entidad Log                        
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.CREACION, "PARÁMETRO", "GroupId=" + objEntity.i_GroupId.ToString() + " / Descripción = " + objEntity.v_Value1, Success.Ok, null);
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.CREACION, "PARÁMETRO", "GroupId=" + pobjDtoEntity.i_GroupId.ToString() + " / Descripción = " + pobjDtoEntity.v_Value1, Success.Failed, ex.Message);
                return;
            }
        }

        public void UpdateSystemParameter(ref OperationResult pobjOperationResult, systemparameterDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.systemparameter
                                       where a.i_GroupId == pobjDtoEntity.i_GroupId && a.i_ParameterId == pobjDtoEntity.i_ParameterId
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.d_UpdateDate = DateTime.Now;
                pobjDtoEntity.i_UpdateUserId = Int32.Parse(ClientSession[2]);


                systemparameter objEntity = systemparameterAssembler.ToEntity(pobjDtoEntity);

                // Copiar los valores desde la entidad actualizada a la Entidad Fuente
                dbContext.systemparameter.ApplyCurrentValues(objEntity);

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;

                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.ACTUALIZACION, "PARÁMETRO", "GroupId=" + objEntity.i_GroupId.ToString() + " / Descripción = " + objEntity.v_Value1, Success.Ok, null);
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.ACTUALIZACION, "PARÁMETRO", "GroupId=" + pobjDtoEntity.i_GroupId.ToString() + " / Descripción = " + pobjDtoEntity.v_Value1, Success.Failed, ex.Message);
                return;
            }
        }

        public void DeleteSystemParameter(ref OperationResult pobjOperationResult, int pintGroupId, int pintParameterId, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var objEntity = (from a in dbContext.systemparameter
                                 where a.i_GroupId == pintGroupId && a.i_ParameterId == pintParameterId
                                 select a).FirstOrDefault();

                if (objEntity != null)
                {
                    dbContext.DeleteObject(objEntity);
                    dbContext.SaveChanges();
                }

                pobjOperationResult.Success = 1;
                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.ELIMINACION, "PARÁMETRO", "GroupId=" + objEntity.i_GroupId.ToString() + " / Descripción = " + objEntity.v_Value1, Success.Ok, null);
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                // Llenar entidad Log
                //LogBL.SaveLog(ClientSession[0], ClientSession[1], ClientSession[2], LogEventType.ELIMINACION, "PARÁMETRO", "", Success.Failed, ex.Message);
                return;
            }
        }

        public List<KeyValueDTO> GetSystemParameterForCombo(ref OperationResult pobjOperationResult, int pintGroupId)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                List<KeyValueDTO> DataSystemParameterList;
                DataSystemParameterList = (from a in dbContext.systemparameter
                                           where a.i_GroupId == pintGroupId
                                           select a).AsEnumerable()
                                           .Select(x => new KeyValueDTO
                                           {
                                               Id = x.i_ParameterId.ToString(),
                                               Value1 = x.v_Value1,
                                               Value2 = x.v_Value2
                                           }).ToList();

                pobjOperationResult.Success = 1;
                return DataSystemParameterList;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<KeyValueDTO> GetSystemParameterByParentForCombo(ref OperationResult pobjOperationResult, int i_ParentGroupId, int i_ParentParameterId)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                List<KeyValueDTO> DataSystemParameterList;
                DataSystemParameterList = (from a in dbContext.systemparameter
                                           where  a.i_ParentParameterId == i_ParentParameterId
                                           select new KeyValueDTO
                                           {
                                               Id = a.i_ParameterId.ToString(),
                                               Value1 = a.v_Value1,
                                               Value2 = a.v_Value2
                                           }).ToList();

                pobjOperationResult.Success = 1;
                return DataSystemParameterList;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }
        
        public List<DataForTreeViewSP> GetSystemParameterForComboTreeView(ref OperationResult pobjOperationResult, int pintGroupId)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                List<DataForTreeViewSP> SystemParameterList;
                List<DataForTreeViewSP> SystemParameterListForBinding = new List<DataForTreeViewSP>();
                SystemParameterList = (from a in dbContext.systemparameter
                                     where a.i_GroupId == pintGroupId && a.i_IsDeleted == 0
                                     orderby a.i_ParentParameterId ascending
                                       select new DataForTreeViewSP
                                     {
                                         Id = a.i_ParameterId,
                                         Description = a.v_Value1,
                                         Description2 = a.v_Value2,
                                         ParentId = a.i_ParentParameterId.Value,
                                         EnabledSelect = true
                                     }
                                           ).ToList();

                // Iterar y ordenar la data
                ProcessData(SystemParameterList, -1, SystemParameterListForBinding, 0);

                pobjOperationResult.Success = 1;
                return SystemParameterListForBinding;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        private void ProcessData(List<DataForTreeViewSP> pDataToIterate, int pParentId, List<DataForTreeViewSP> pResults, int pLevel)
        {
            var query = from i in pDataToIterate
                        where i.ParentId == pParentId
                        orderby i.Description ascending
                        select i;

            foreach (var item in query)
            {
                item.Level = pLevel;
                pResults.Add(item);
                ProcessData(pDataToIterate, item.Id, pResults, pLevel + 1);
            }
        }

        public List<DataTreeViewForGridViewSP> GetSystemParameterForGridView(ref OperationResult pobjOperationResult, int pintGroupId)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                List<DataTreeViewForGridViewSP> SystemParameterList;
                List<DataTreeViewForGridViewSP> SystemParameterListForBinding = new List<DataTreeViewForGridViewSP>();
                SystemParameterList = (from a in dbContext.systemparameter
                                     where a.i_GroupId == pintGroupId && a.i_IsDeleted == 0
                                     orderby a.i_ParentParameterId ascending
                                     select new DataTreeViewForGridViewSP
                                     {
                                         i_GroupId = a.i_GroupId,
                                         i_ParameterId = a.i_ParameterId,
                                         v_Value1 = a.v_Value1,
                                         v_Value2 = a.v_Value2,
                                         i_ParentItemId = a.i_ParentParameterId.Value
                                     }
                                           ).ToList();

                // Iterar y ordenar la data
                ProcessDataGridView(SystemParameterList, -1, SystemParameterListForBinding, 0);

                pobjOperationResult.Success = 1;
                return SystemParameterListForBinding;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        private void ProcessDataGridView(List<DataTreeViewForGridViewSP> pDataToIterate, int pParentId, List<DataTreeViewForGridViewSP> pResults, int pLevel)
        {
            var query = from i in pDataToIterate
                        where i.i_ParentItemId == pParentId
                        orderby i.v_Value1 ascending
                        select i;

            foreach (var item in query)
            {
                item.Level = pLevel;
                pResults.Add(item);
                ProcessDataGridView(pDataToIterate, item.i_ParameterId, pResults, pLevel + 1);
            }
        }

        public List<systemparameterDto> GetSystemParametersPagedAndFiltered(int? pintPageIndex, int? pintResultsPerPage, string pstrSortExpression, string pstrFilterExpression, int pintGroupId)
        {

            SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

            var query = (from a in dbContext.systemparameter
                         select new systemparameterDto
                         {
                             i_GroupId = a.i_GroupId,
                             i_ParameterId = a.i_ParameterId,
                             v_Value1 = a.v_Value1
                         }).ToList();

            return query;
        }
        public systemparameterDto GetSystemParameterSystemColor(ref OperationResult pobjOperationResult)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                systemparameterDto objDtoEntity = null;

                var objEntity = (from a in dbContext.systemparameter
                                 where a.i_GroupId == 120 && a.v_Value2 == "1"
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = systemparameterAssembler.ToDTO(objEntity);

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

        #endregion
      
        #region KeyValueDTO

        public  List<KeyValueDTO> GetSystemParameterForCombo(ref OperationResult pobjOperationResult, int pintGroupId, string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = from a in dbContext.systemparameter
                            where a.i_GroupId == pintGroupId && a.i_IsDeleted == 0
                            select a;

                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }
                else
                {
                    query = query.OrderBy("v_Value1");
                }

                var query2 = query.AsEnumerable()
                            .Select(x => new KeyValueDTO
                            {
                                Id = x.i_ParameterId.ToString(),
                                Value1 = x.v_Value1,
                                Value2 = x.v_Value2
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

        public  List<KeyValueDTO> GetNode(ref OperationResult pobjOperationResult,string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = from a in dbContext.node
                            where a.i_IsDeleted == 0
                            select a;

                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }
                else
                {
                    query = query.OrderBy("v_RazonSocial");
                }

                var query2 = query.AsEnumerable()
                            .Select(x => new KeyValueDTO
                            {
                                Id = x.i_NodeId.ToString(),
                                Value1 = x.v_RazonSocial,
                                Value3 = x.v_RUC
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

        public List<KeyValueDTO> GetRole(ref OperationResult pobjOperationResult, string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = from a in dbContext.role
                            where a.i_IsDeleted == 0 && a.i_RoleId != 1
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
                                Id = x.i_RoleId.ToString(),
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

        public  List<KeyValueDTO> GetWarehouse(ref OperationResult pobjOperationResult, string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = from a in dbContext.warehouse
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
                                Id = x.i_WarehouseId.ToString(),
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

        public List<KeyValueDTO> GetSystemParameterForComboUbigeoKeyValueDto(ref OperationResult pobjOperationResult, int? pintParentItemID, int pintGropudID, string pstrSortExpression)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModel())
                {
                    var query = from a in dbContext.systemparameter
                        where a.i_GroupId == pintGropudID && a.i_ParentParameterId == pintParentItemID && a.i_IsDeleted == 0
                        select a;

                    query = query.OrderBy(!string.IsNullOrEmpty(pstrSortExpression) ? pstrSortExpression : "v_Value1");

                    var query2 = query.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_ParameterId.ToString(),
                            Value1 = x.v_Value1,
                            Value2 = x.v_Value2,
                            Value3 = x.i_ParentParameterId.ToString()

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
        /// <summary>
        /// Gets the ubigueo by ids.
        /// </summary>
        /// <param name="idDep">The identifier dep.</param>
        /// <param name="idProv">The identifier prov.</param>
        /// <param name="idDist">The identifier dist.</param>
        /// <returns>System.String.</returns>
        public string GetUbigueoByIds(int idDep, int idProv, int idDist)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModel())
                {
                    var query = (from a in dbContext.systemparameter
                        join c in dbContext.systemparameter on
                            new { grId = 112, id = idProv, e = 0 } equals new { grId = c.i_GroupId, id = c.i_ParameterId, e = c.i_IsDeleted.Value }
                            into j2Join
                        from c in j2Join.DefaultIfEmpty()
                        join d in dbContext.systemparameter on
                            new { grId = 112, id = idDist, e = 0 } equals new { grId = d.i_GroupId, id = d.i_ParameterId, e = d.i_IsDeleted.Value }
                            into j3Join
                        from d in j3Join.DefaultIfEmpty()
                        where a.i_GroupId == 112 && a.i_ParameterId ==idDep 
                        && a.i_ParentParameterId == 1 && a.i_IsDeleted == 0
                        
                        select new KeyValueDTO
                        {
                            Value1 = a.v_Value2,
                            Value2 = c.v_Value2,
                            Value3 = d.v_Value2
                        }).FirstOrDefault();

                    if (query == null) return string.Empty;

                    string[] cods = {query.Value1, query.Value2, query.Value3};

                    var result = string.Empty;
                    foreach (var cod in cods)
                    {
                        int val;
                        if (int.TryParse(cod, out val))
                        {
                            result += val.ToString("00");
                        }
                    }
                    return result;
                }

            }
            catch
            {
                return string.Empty;
            }
        }

        public List<KeyValueDTO> GetSystemParameterForComboKeyValueDto(ref OperationResult pobjOperationResult, int pintGroupId, string pstrSortExpression)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = from a in dbContext.systemparameter
                            where a.i_GroupId == pintGroupId && a.i_IsDeleted == 0
                            select a;

                if (!string.IsNullOrEmpty(pstrSortExpression))
                {
                    query = query.OrderBy(pstrSortExpression);
                }
                else
                {
                    query = query.OrderBy("v_Value1");
                }

                var query2 = query.AsEnumerable()
                            .Select(x => new KeyValueDTO
                            {
                                Id = x.i_ParameterId.ToString(),
                                Value1 = x.v_Value1,
                                Value2 = x.v_Value2,
                                Value3 = x.i_ParentParameterId.ToString(),
                                Value5 =x.v_Field ,
                              
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
