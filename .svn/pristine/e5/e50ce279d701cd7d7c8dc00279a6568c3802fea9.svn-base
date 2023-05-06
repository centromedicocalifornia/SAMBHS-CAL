using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;


namespace SAMBHS.Security.BL
{
    public class ApplicationHierarchyBL
    {
        #region"ApplicationHierarchy"

        public applicationhierarchyDto GetApplicationHierarchy(ref OperationResult pobjOperationResult, int pintApplicationHierarchyId)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                applicationhierarchyDto objDtoEntity = null;

                var objEntity = (from a in dbContext.applicationhierarchy
                                 where a.i_ApplicationHierarchyId == pintApplicationHierarchyId
                                 select a).FirstOrDefault();

                if (objEntity != null)
                    objDtoEntity = applicationhierarchyAssembler.ToDTO(objEntity);

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

        public void AddApplicationHierarchy(ref OperationResult pobjOperationResult, applicationhierarchyDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                applicationhierarchy objEntity = applicationhierarchyAssembler.ToEntity(pobjDtoEntity);

                objEntity.d_InsertDate = DateTime.Now;
                objEntity.i_InsertUserId = Int32.Parse(ClientSession[2]);
                objEntity.i_IsDeleted = 0;


                //// Autogeneramos el Pk de la tabla

                dbContext.AddToapplicationhierarchy(objEntity);
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                // Llenar entidad Log
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                // Llenar entidad Log
                return;
            }
        }

        public void UpdateApplicationHierarchy(ref OperationResult pobjOperationResult, applicationhierarchyDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.applicationhierarchy
                                       where a.i_ApplicationHierarchyId == pobjDtoEntity.i_ApplicationHierarchyId
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.d_UpdateDate = DateTime.Now;
                pobjDtoEntity.i_UpdateUserId = Int32.Parse(ClientSession[2]);


                applicationhierarchy objEntity = applicationhierarchyAssembler.ToEntity(pobjDtoEntity);

                // Copiar los valores desde la entidad actualizada a la Entidad Fuente
                dbContext.applicationhierarchy.ApplyCurrentValues(objEntity);

                // Guardar los cambios
                dbContext.SaveChanges();

                pobjOperationResult.Success = 1;
                // Llenar entidad Log
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                // Llenar entidad Log
                return;
            }
        }

        public void DeleteApplicationHierarchy(ref OperationResult pobjOperationResult, int pintApplicationHierarchyId, List<string> ClientSession)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.applicationhierarchy
                                       where a.i_ApplicationHierarchyId == pintApplicationHierarchyId
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
                pobjOperationResult.ExceptionMessage = ex.Message;
                // Llenar entidad Log
                return;
            }
        }

        public List<DtvForGrwAppHierarchy> GetApplicationHierarchyForGridView(ref OperationResult pobjOperationResult)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                List<DtvForGrwAppHierarchy> ApplicationHierarchyList;
                List<DtvForGrwAppHierarchy> DataHierarchyListForBinding = new List<DtvForGrwAppHierarchy>();
                ApplicationHierarchyList = (from a in dbContext.applicationhierarchy
                                            join J1 in dbContext.systemparameter on new { a = a.i_ApplicationHierarchyTypeId.Value, b = 106 } equals new { a = J1.i_ParameterId, b = J1.i_GroupId }
                                            join J2 in dbContext.systemparameter on new { a = a.i_ScopeId.Value, b = 104 }
                                                                       equals new { a = J2.i_ParameterId, b = J2.i_GroupId } into J2_join
                                            from J2 in J2_join.DefaultIfEmpty()
                                            where a.i_IsDeleted == 0
                                            select new DtvForGrwAppHierarchy
                                            {
                                                i_GroupId = a.i_ApplicationHierarchyId,
                                                i_ApplicationHierarchyId = a.i_ApplicationHierarchyId,
                                                v_Value1 = a.v_Description,
                                                i_ParentItemId = a.i_ParentId.Value,
                                                v_Form = a.v_Form,
                                                v_Code = a.v_Code,
                                                v_ScopeName = J2.v_Value1,
                                                v_ApplicationHierarchyTypeName = J1.v_Value1
                                            }).ToList();

                // Iterar y ordenar la data
                ProcessDataGridView(ApplicationHierarchyList, -1, DataHierarchyListForBinding, 0);

                pobjOperationResult.Success = 1;
                return DataHierarchyListForBinding;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        private static void ProcessDataGridView(IEnumerable<DtvForGrwAppHierarchy> pDataToIterate, int pParentId, ICollection<DtvForGrwAppHierarchy> pResults, int pLevel)
        {
            //mon.IsActive = true;
            var dtvForGrwAppHierarchies = pDataToIterate as IList<DtvForGrwAppHierarchy> ?? pDataToIterate.ToList();
            var query = from i in dtvForGrwAppHierarchies
                        where i.i_ParentItemId == pParentId
                        orderby i.v_Value1 ascending
                        select i;

            foreach (var item in query)
            {
                item.Level = pLevel;
                pResults.Add(item);
                ProcessDataGridView(dtvForGrwAppHierarchies, item.i_ApplicationHierarchyId, pResults, pLevel + 1);
            }
        }

        public List<applicationhierarchyDto> GetAllApplicationHierarchy(ref OperationResult pobjOperationResult)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                List<applicationhierarchyDto> oListApplicationHierarchy;
                oListApplicationHierarchy = (from a in dbContext.applicationhierarchy
                                             select new applicationhierarchyDto
                                             {
                                                 i_ApplicationHierarchyId = a.i_ApplicationHierarchyId,
                                                 i_ApplicationHierarchyTypeId = a.i_ApplicationHierarchyTypeId,
                                                 i_Level = a.i_Level,
                                                 v_Description = a.v_Description,
                                                 v_Form = a.v_Form,
                                                 i_ParentId = a.i_ParentId,
                                                 i_ScopeId = a.i_ScopeId,
                                                 v_Code = a.v_Code
                                             }).ToList();
                pobjOperationResult.Success = 1;
                return oListApplicationHierarchy;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }

        }

        public List<applicationhierarchyDto> GetApplicationHierarchyByScopeId(ref OperationResult pobjOperationResult, int pintScopeId)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                List<applicationhierarchyDto> oListApplicationHierarchy;
                oListApplicationHierarchy = (from a in dbContext.applicationhierarchy
                                             orderby a.i_ApplicationHierarchyId
                                             where (a.i_ScopeId == pintScopeId && a.i_IsDeleted == 0) || (a.i_ScopeId == -1)
                                             select new applicationhierarchyDto
                                             {
                                                 i_ApplicationHierarchyId = a.i_ApplicationHierarchyId,
                                                 i_ApplicationHierarchyTypeId = a.i_ApplicationHierarchyTypeId,
                                                 i_Level = a.i_Level,
                                                 v_Description = a.v_Description,
                                                 v_Form = a.v_Form,
                                                 i_ParentId = a.i_ParentId,
                                                 i_ScopeId = a.i_ScopeId,
                                                 v_Code = a.v_Code
                                             }).ToList();
                pobjOperationResult.Success = 1;
                return oListApplicationHierarchy;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }

        }

        public List<DtvAppHierarchy> GetApplicationHierarchyForCombo(ref OperationResult pobjOperationResult)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                List<DtvAppHierarchy> DataHierarchyList;
                List<DtvAppHierarchy> DataHierarchyListForBinding = new List<DtvAppHierarchy>();
                DataHierarchyList = (from a in dbContext.applicationhierarchy
                                     where a.i_IsDeleted == 0
                                     select new DtvAppHierarchy
                                     {
                                         Id = a.i_ApplicationHierarchyId,
                                         Description = a.v_Description,
                                         ParentId = a.i_ParentId.Value,
                                         EnabledSelect = true
                                     }
                                           ).ToList();

                // Iterar y ordenar la data
                ProcessData(DataHierarchyList, -1, DataHierarchyListForBinding, 0);

                pobjOperationResult.Success = 1;
                return DataHierarchyListForBinding;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        private void ProcessData(List<DtvAppHierarchy> pDataToIterate, int pParentId, List<DtvAppHierarchy> pResults, int pLevel)
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

        #endregion

        #region Windows Version
        public List<ConsultaApplictionhierarchyDto> ObtenerMenus(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel())
                {
                    var tabla = dbContext.applicationhierarchy.Where(p => p.i_IsDeleted == 0).ToList();

                    var consulta = (from n in tabla

                                    where n.i_IsDeleted == 0 && n.i_ParentId == -1

                                    select n).ToList().Select(n => new ConsultaApplictionhierarchyDto
                                    {
                                        Descripcion = n.v_Description,
                                        Id = n.i_ApplicationHierarchyId,
                                        Form = n.v_Form
                                    }
                                    ).ToList();

                    foreach (var item in consulta)
                    {
                        DevolverHijos(item, item.Id, tabla);
                    }

                    pobjOperationResult.Success = 1;

                    return consulta;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "ApplicationHierarchyBL.ObtenerMenus()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
                return null;
            }
        }

        static void DevolverHijos(ConsultaApplictionhierarchyDto consulta, int parentId, IEnumerable<applicationhierarchy> tabla)
        {
            if (tabla == null) return;
            var applicationhierarchies = tabla as applicationhierarchy[] ?? tabla.ToArray();

            var hijo = (from p in applicationhierarchies
                        where p.i_IsDeleted == 0 && p.i_ParentId == parentId
                        select p).ToList().Select(p => new ConsultaApplictionhierarchyDto
                        {
                            Descripcion = p.v_Description,
                            Id = p.i_ApplicationHierarchyId,
                            Form = p.v_Form
                        }
                        ).ToList();

            if (consulta != null) consulta.Hijo = hijo;

            foreach (var item in hijo)
            {
                DevolverHijos(item, item.Id, applicationhierarchies);
            }
        }
        #endregion
    }
}
