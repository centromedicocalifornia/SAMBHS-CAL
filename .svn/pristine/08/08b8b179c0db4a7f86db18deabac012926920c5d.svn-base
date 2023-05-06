using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BL;
using System.Linq.Dynamic;
using System.Data;

namespace SAMBHS.Security.BL
{
    public class SecurityBL
    {

        #region SystemUser

        #region Authentication
        /// <summary>
        /// Valida si el susuario esta registrado en el sistema
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pstrUser"></param>
        /// <param name="pstrPassword"></param>
        /// <param name="pintNode"></param>
        /// <returns></returns>
        public systemuserDto ValidateSystemUser(ref OperationResult pobjOperationResult, int pintNodeId, string pstrUser, string pstrPassword)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                var objSystemUserDto = (from a in dbContext.systemuser

                                        join p in dbContext.person on a.i_PersonId equals p.i_PersonId into p_join
                                        from p in p_join.DefaultIfEmpty()

                                        where a.v_UserName == pstrUser && a.v_Password == pstrPassword && a.i_IsDeleted == 0 && p.i_IsDeleted == 0

                                        select new systemuserDto
                                        {
                                            i_SystemUserId = a.i_SystemUserId,
                                            v_UserName = a.v_UserName,
                                            i_PersonId = a.i_PersonId,
                                            i_RoleId = a.i_RoleId,
                                            v_PersonName = p.v_FirstLastName.Trim() + " " + p.v_SecondLastName.Trim() + " " + p.v_FirstName.Trim(),
                                            i_UsuarioContable = a.i_UsuarioContable
                                        }
                    ).FirstOrDefault();

                if (objSystemUserDto == null)
                {
                    pobjOperationResult.AdditionalInformation = "Las credenciales son incorrectas. Asegúrese que el Usuario y Password sean los correctos.";
                    pobjOperationResult.ReturnValue = "NOTEXIST";

                    // Llenar entidad Log

                }
                else
                {
                    pobjOperationResult.ReturnValue = "EXIST";
                    // Llenar entidad Log

                }

                pobjOperationResult.Success = 1;
                return objSystemUserDto;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }

        }

        public systemuserDto ValidateUserBarCode(ref OperationResult pobjOperationResult, int pintNodeId, string pstrCodeBar)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                systemuserDto objSystemUserDto = null;
                objSystemUserDto = (from a in dbContext.systemuser
                                    join b in dbContext.person on a.i_PersonId equals b.i_PersonId
                                    where a.v_CodeBar == pstrCodeBar && a.i_IsDeleted == 0
                                    select new systemuserDto
                                    {
                                        i_SystemUserId = a.i_SystemUserId,
                                        v_UserName = a.v_UserName,
                                        i_PersonId = a.i_PersonId,
                                        i_RoleId = a.i_RoleId,
                                        v_PersonName = b.v_FirstName + " " + b.v_FirstLastName + " " + b.v_SecondLastName

                                    }).FirstOrDefault();

                if (objSystemUserDto == null)
                {
                    pobjOperationResult.AdditionalInformation = "El usuario no existe o está desahibilitado";
                    pobjOperationResult.ReturnValue = "NOTEXIST";
                }
                else
                {
                    pobjOperationResult.ReturnValue = "EXIST";
                }

                pobjOperationResult.Success = 1;
                return objSystemUserDto;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<GridKeyValueDTO> ReturnNodes(ref OperationResult pobjOperationResult)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModel())
                {
                    dbContext.CommandTimeout = 50;

                    var Query = (from n in dbContext.node
                                 where n.i_IsDeleted == 0
                                 select n).ToList();

                    var Result = Query.Select(p => new GridKeyValueDTO
                                            {
                                                Id = p.i_NodeId.ToString(),
                                                Value1 = p.v_RUC,
                                                Value2 = p.v_RazonSocial,
                                            }).ToList();

                    pobjOperationResult.Success = 1;

                    return Result;
                }
            }
            catch (EntityException ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                return null;
            }
        }

        public bool ValidateSystemUserNode(int i_SystemUser, int i_NodeId)
        {
            using (SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel())
            {
                return dbContext.systemusernode.Any(n => n.i_NodeId == i_NodeId && n.i_SystemUserId == i_SystemUser && n.i_IsDeleted == 0);
            }
        }

        public organizationDto OrganizacionActiva()
        {
            using (var dbContext = new SAMBHSEntitiesModel())
            {
                var o = dbContext.organization.FirstOrDefault();
                if (o != null) return o.ToDTO();
                return null;
            }
        }

        #endregion

        #region Authorization

        /// <summary>
        /// Retorna información de su profile de un usuario por nodo y empresa para el armado de su menú
        /// </summary>
        /// <param name="pobjOperationResult"></param>
        /// <param name="pintNodeId"></param>
        /// <param name="pintOrganizationId"></param>
        /// <returns></returns>
        public List<AutorizationList> GetAuthorizationWeb(ref OperationResult pobjOperationResult, int pintRoleId)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModel())
                {
                    var query = (from a in dbContext.roleprofile
                                 join b in dbContext.applicationhierarchy on a.i_ApplicationHierarchyId equals b.i_ApplicationHierarchyId
                                 join c in dbContext.role on a.i_RoleId equals c.i_RoleId
                                 where a.i_RoleId == pintRoleId &&
                                      (b.i_ApplicationHierarchyTypeId == 2 || b.i_ApplicationHierarchyTypeId == 1) &&
                                     b.i_IsDeleted == 0 && a.i_IsDeleted == 0 && c.i_IsDeleted == 0 && b.i_TypeFormId == (int)TypeForm.Web
                                 select new AutorizationList
                                 {
                                     I_ApplicationHierarchyId = a.i_ApplicationHierarchyId.Value,
                                     I_ApplicationHierarchyTypeId = b.i_ApplicationHierarchyTypeId,
                                     V_Description = b.v_Description,
                                     I_ParentId = b.i_ParentId,
                                     V_Form = b.v_Form == null ? string.Empty : b.v_Form,
                                     i_RoleId = a.i_RoleId,
                                     V_RoleName = c.v_Name
                                 });

                    List<AutorizationList> objAutorizationList = query.AsEnumerable()
                                                                .OrderBy(p => p.I_ApplicationHierarchyId)
                                                                .GroupBy(x => x.I_ApplicationHierarchyId)
                                                                .Select(group => group.First())
                                                                .ToList();

                    pobjOperationResult.Success = 1;
                    return objAutorizationList;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }

        }

        public List<AutorizationList> GetAuthorizationNode(ref OperationResult pobjOperationResult, int pintRoleId)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModel())
                {
                    var query = (from a in dbContext.roleprofile

                                 join b in dbContext.applicationhierarchy on a.i_ApplicationHierarchyId equals b.i_ApplicationHierarchyId into b_join
                                 from b in b_join.DefaultIfEmpty()

                                 join c in dbContext.role on a.i_RoleId equals c.i_RoleId into c_join
                                 from c in c_join.DefaultIfEmpty()

                                 where a.i_RoleId == pintRoleId &&
                                      (b.i_ApplicationHierarchyTypeId == (int)Menu.Modulo ||
                                        b.i_ApplicationHierarchyTypeId == (int)Menu.Pantalla ||
                                        b.i_ApplicationHierarchyTypeId == (int)Menu.AgrupadorMenu) &&
                                     b.i_IsDeleted == 0 && a.i_IsDeleted == 0 && c.i_IsDeleted == 0 &&
                                     b.i_TypeFormId == (int)TypeForm.Windows &&
                                     b.i_ParentId != -1

                                 select new AutorizationList
                                 {
                                     I_ApplicationHierarchyId = a.i_ApplicationHierarchyId.Value,
                                     I_ApplicationHierarchyTypeId = b.i_ApplicationHierarchyTypeId,
                                     V_Description = b.v_Description,
                                     I_ParentId = b.i_ParentId,
                                     V_Form = b.v_Form ?? string.Empty,
                                     i_RoleId = a.i_RoleId,
                                     V_RoleName = c.v_Name,
                                     i_Level = b.i_Level,
                                     v_Code = b.v_Code
                                 });

                    List<AutorizationList> objAutorizationList = query.AsEnumerable()
                                                                .OrderBy(p => p.i_Level)
                                                                .GroupBy(x => x.I_ApplicationHierarchyId)
                                                                .Select(group => group.First())
                                                                .ToList();

                    pobjOperationResult.Success = 1;
                    return objAutorizationList;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }

        }

        public List<AutorizationList> GetAuthorizationNodeWeb(ref OperationResult pobjOperationResult, int pintRoleId)
        {
            //mon.IsActive = true;
            try
            {
                using (var dbContext = new SAMBHSEntitiesModel())
                {
                    var query = (from a in dbContext.roleprofile

                                 join b in dbContext.applicationhierarchy on a.i_ApplicationHierarchyId equals b.i_ApplicationHierarchyId into b_join
                                 from b in b_join.DefaultIfEmpty()

                                 join c in dbContext.role on a.i_RoleId equals c.i_RoleId into c_join
                                 from c in c_join.DefaultIfEmpty()

                                 where a.i_RoleId == pintRoleId &&
                                      (b.i_ApplicationHierarchyTypeId == (int)Menu.Modulo ||
                                        b.i_ApplicationHierarchyTypeId == (int)Menu.Pantalla ||
                                        b.i_ApplicationHierarchyTypeId == (int)Menu.AgrupadorMenu) &&
                                     b.i_IsDeleted == 0 && a.i_IsDeleted == 0 && c.i_IsDeleted == 0 &&
                                     b.i_TypeFormId == (int)TypeForm.Web

                                 select new AutorizationList
                                 {
                                     I_ApplicationHierarchyId = a.i_ApplicationHierarchyId.Value,
                                     I_ApplicationHierarchyTypeId = b.i_ApplicationHierarchyTypeId,
                                     V_Description = b.v_Description,
                                     I_ParentId = b.i_ParentId,
                                     V_Form = b.v_Form == null ? string.Empty : b.v_Form,
                                     i_RoleId = a.i_RoleId,
                                     V_RoleName = c.v_Name,
                                     i_Level = b.i_Level,
                                     v_Code = b.v_Code
                                 });

                    List<AutorizationList> objAutorizationList = query.AsEnumerable()
                                                                .OrderBy(p => p.i_Level)
                                                                .GroupBy(x => x.I_ApplicationHierarchyId)
                                                                .Select(group => group.First())
                                                                .ToList();

                    pobjOperationResult.Success = 1;
                    return objAutorizationList;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }

        }

        #endregion

        #region FormActions
        public List<KeyValueDTO> GetFormAction(ref OperationResult pobjOperationResult, int pintNodeId, int pintSystemUserId, string pstrFormCode, int pintRoleId)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = (from rnp in dbContext.roleprofile
                             join ah in dbContext.applicationhierarchy on rnp.i_ApplicationHierarchyId equals ah.i_ApplicationHierarchyId
                             where
                                    rnp.i_RoleId == pintRoleId &&
                                    ah.v_Form == pstrFormCode &&
                                   ah.i_ApplicationHierarchyTypeId == 3 &&  // solo acciones
                                   rnp.i_IsDeleted == 0 && ah.i_IsDeleted == 0 && rnp.i_RoleId == pintRoleId
                             select new KeyValueDTO
                             {
                                 Value1 = ah.v_Description,
                                 Value2 = ah.v_Code
                             });

                List<KeyValueDTO> objFormAction = query.OrderBy(P => P.Value1).ToList();
                pobjOperationResult.Success = 1;
                return objFormAction;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }
        #endregion

        #region Crud

        public List<systemuserDto> GetSystemUserPagedAndFiltered(ref OperationResult pobjOperationResult, int? pintPageIndex, int? pintResultsPerPage, string pstrSortExpression, string pstrFilterExpression)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = (from su1 in dbContext.systemuser
                             where su1.i_IsDeleted == 0
                             join su2 in dbContext.systemuser on new { i_InsertUserId = su1.i_InsertUserId.Value }
                                                           equals new { i_InsertUserId = su2.i_SystemUserId } into su2_join
                             from su2 in su2_join.DefaultIfEmpty()

                             join su3 in dbContext.systemuser on new { i_UpdateUserId = su1.i_UpdateUserId.Value }
                                                           equals new { i_UpdateUserId = su3.i_SystemUserId } into su3_join
                             from su3 in su3_join.DefaultIfEmpty()

                             select new systemuserDto
                             {
                                 i_SystemUserId = su1.i_SystemUserId,
                                 i_PersonId = su1.i_PersonId,
                                 v_UserName = su1.v_UserName,
                                 v_Password = su1.v_Password,
                                 v_SecretQuestion = su1.v_SecretQuestion,
                                 v_SecretAnswer = su1.v_SecretAnswer,
                                 i_IsDeleted = su1.i_IsDeleted,
                                 i_InsertUserId = su1.i_InsertUserId,
                                 d_InsertDate = su1.d_InsertDate,
                                 i_UpdateUserId = su1.i_UpdateUserId,
                                 d_UpdateDate = su1.d_UpdateDate,
                                 v_InsertUser = su2.v_UserName,
                                 v_UpdateUser = su3.v_UserName,
                                 v_PersonName = (su1.person.v_FirstLastName + " " + su1.person.v_SecondLastName + " " + su1.person.v_FirstName).Trim().ToUpper(),
                                 NroDocumento = su1.person.v_DocNumber

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

                List<systemuserDto> objData = query.ToList();
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

        public int GetSystemUserCount(ref OperationResult pobjOperationResult, string filterExpression)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                var query = from a in dbContext.systemuser select a;

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

        public systemuserDto GetSystemUser(ref OperationResult pobjOperationResult, int pintSystemUserId)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                systemuserDto objDtoEntity = null;

                var objEntity = (from a in dbContext.systemuser
                                 where a.i_SystemUserId == pintSystemUserId
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

        public static void RestorePassword(ref OperationResult pobjOperationResult, int pintIdUsuario)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModel())
                {
                    const string passwordPorDefecto = "12345678";
                    var usuario = dbContext.systemuser.FirstOrDefault(p => p.i_SystemUserId == pintIdUsuario);

                    if (usuario != null)
                    {
                        usuario.v_Password = Utils.Encrypt(passwordPorDefecto);
                        dbContext.systemuser.ApplyCurrentValues(usuario);
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        return;
                    }
                    throw new ArgumentNullException(@"Usuario no Existe!");
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
            }
        }

        public int AddSystemUSer(ref OperationResult pobjOperationResult, systemuserDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            int SecuentialId = 0;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                systemuser objEntity = systemuserAssembler.ToEntity(pobjDtoEntity);

                objEntity.d_InsertDate = DateTime.Now;
                objEntity.i_InsertUserId = Int32.Parse(ClientSession[2]);
                objEntity.i_IsDeleted = 0;
                objEntity.i_SystemUserId = SecuentialId;

                dbContext.AddTosystemuser(objEntity);
                dbContext.SaveChanges();

                ReplicaCambiosDeUsuariosATodasLasEmpresas(Proceso.Insertar, objEntity);

                pobjOperationResult.Success = 1;
                // Llenar entidad Log
                return SecuentialId;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return 0;
            }

        }

        public void UpdateSystemUSer(ref OperationResult pobjOperationResult, systemuserDto pobjDtoEntity, List<string> ClientSession)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad fuente
                var objEntitySource = (from a in dbContext.systemuser
                                       where a.i_SystemUserId == pobjDtoEntity.i_SystemUserId
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                pobjDtoEntity.d_UpdateDate = DateTime.Now;
                pobjDtoEntity.i_UpdateUserId = Int32.Parse(ClientSession[2]);

                systemuser objEntity = systemuserAssembler.ToEntity(pobjDtoEntity);

                // Copiar los valores desde la entidad actualizada a la Entidad Fuente
                dbContext.systemuser.ApplyCurrentValues(objEntity);

                // Guardar los cambios
                dbContext.SaveChanges();

                ReplicaCambiosDeUsuariosATodasLasEmpresas(Proceso.Editar, objEntity);

                pobjOperationResult.Success = 1;

                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);

                return;
            }
        }

        public void DeleteSystemUSer(ref OperationResult pobjOperationResult, int pintSystemUSerId, List<string> ClientSession)
        {
            //mon.IsActive = true;
            systemuser objEntitySource = null;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad fuente
                objEntitySource = (from a in dbContext.systemuser
                                   where a.i_SystemUserId == pintSystemUSerId
                                   select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.d_UpdateDate = DateTime.Now;
                objEntitySource.i_UpdateUserId = int.Parse(ClientSession[2]);
                objEntitySource.i_IsDeleted = 1;

                //elimina la persona relacionada
                var person = dbContext.person.FirstOrDefault(p => p.i_PersonId == objEntitySource.i_PersonId.Value);
                if (person != null)
                {
                    person.d_UpdateDate = DateTime.Now;
                    person.i_UpdateUserId = int.Parse(ClientSession[2]);
                    person.i_IsDeleted = 1;
                    dbContext.person.ApplyCurrentValues(person);
                }

                // Guardar los cambios
                dbContext.SaveChanges();

                ReplicaCambiosDeUsuariosATodasLasEmpresas(Proceso.Eliminar, objEntitySource);

                pobjOperationResult.Success = 1;

                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);

                return;
            }
        }

        void ReplicaCambiosDeUsuariosATodasLasEmpresas(Proceso _Proceso, systemuser Usuario)
        {
            try
            {
                using (SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel())
                {
                    List<string> Empresas = (from n in dbContext.node where n.i_IsDeleted == 0 select n.v_RUC).ToList();

                    if (Empresas != null)
                    {
                        switch (_Proceso)
                        {
                            //case Proceso.Insertar:

                            //    Empresas.ForEach(RUC => dbContext.spi_usuarios_empresas(RUC, (int?)Usuario.i_SystemUserId, (int?)Usuario.i_PersonId.Value, (int?)Usuario.i_RoleId.Value, Usuario.v_UserName, Usuario.v_Password, (int?)Usuario.i_InsertUserId.Value, Usuario.d_InsertDate.Value.ToShortDateString()));

                            //    break;
                            //case Proceso.Editar:

                            //    Empresas.ForEach(RUC => dbContext.spe_usuarios_empresas(RUC, (int?)Usuario.i_SystemUserId, (int?)Usuario.i_PersonId.Value, (int?)Usuario.i_RoleId.Value, Usuario.v_UserName, Usuario.v_Password));

                            //    break;
                            //case Proceso.Eliminar:

                            //    Empresas.ForEach(RUC => dbContext.spd_usuarios_empresas(RUC, (int?)Usuario.i_SystemUserId));

                            //    break;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #endregion

        #region RoleProfile

        private void DeleteRoleProfiles(ref OperationResult pobjOperationResult, List<roleprofileDto> ListRoleProfileDto, List<string> ClientSession)
        {

            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                foreach (var item in ListRoleProfileDto)
                {
                    // Obtener la entidad a eliminar
                    var objEntitySource = (from a in dbContext.roleprofile
                                           where a.i_ApplicationHierarchyId == item.i_ApplicationHierarchyId &&
                                                 a.i_RoleId == item.i_RoleId
                                           select a).FirstOrDefault();

                    if (objEntitySource != null)
                    {
                        objEntitySource.d_UpdateDate = DateTime.Now;
                        objEntitySource.i_UpdateUserId = Int32.Parse(ClientSession[2]);
                        objEntitySource.i_IsDeleted = 1;
                    }
                    else
                    {
                        var roleprofile = item.ToEntity();
                        roleprofile.d_UpdateDate = DateTime.Now;
                        roleprofile.i_UpdateUserId = Int32.Parse(ClientSession[2]);
                        roleprofile.i_IsDeleted = 1;
                        dbContext.roleprofile.AddObject(roleprofile);
                    }
                }

                dbContext.SaveChanges();
                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SecurityBL.DeleteRoleProfiles()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }

        }

        public void AddRoleProfiles(ref OperationResult pobjOperationResult, List<roleprofileDto> ListRoleProfileDto, int pintUserPersonId, List<string> ClientSession, bool pbRegisterLog)
        {
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                foreach (var item in ListRoleProfileDto)
                {

                    var objEntitySource = (from a in dbContext.roleprofile
                                           where a.i_RoleId == item.i_RoleId &&
                                                   a.i_ApplicationHierarchyId == item.i_ApplicationHierarchyId
                                           select a).FirstOrDefault();

                    if (objEntitySource != null)
                    {
                        if (objEntitySource.i_IsDeleted == 1)  // Registro macado como eliminado
                        {
                            // dar de alta el registro existente
                            objEntitySource.d_UpdateDate = DateTime.Now;
                            objEntitySource.i_UpdateUserId = Int32.Parse(ClientSession[2]);
                            objEntitySource.i_IsDeleted = 0;
                            dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        // Grabar como nuevo
                        roleprofile objEntity = roleprofileAssembler.ToEntity(item);

                        objEntity.d_InsertDate = DateTime.Now;
                        objEntity.i_InsertUserId = pintUserPersonId;
                        objEntity.i_IsDeleted = 0;

                        dbContext.roleprofile.AddObject(objEntity);
                        dbContext.SaveChanges();
                    }
                }


                pobjOperationResult.Success = 1;
                if (pbRegisterLog == true)
                {
                    // Llenar entidad Log

                }

                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SecurityBL.AddRoleProfiles()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }

        }

        public List<AutorizationList> GetRoleProfiles(ref OperationResult pobjOperationResult, int pintRoleId)
        {
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                List<AutorizationList> oAutorizationList;

                oAutorizationList = (from a in dbContext.roleprofile
                                     join b in dbContext.applicationhierarchy on a.i_ApplicationHierarchyId equals b.i_ApplicationHierarchyId
                                     where a.i_RoleId == pintRoleId &&
                                           a.i_IsDeleted == 0 && a.applicationhierarchy.i_IsDeleted == 0
                                     select new AutorizationList
                                     {
                                         I_ApplicationHierarchyId = (int)a.i_ApplicationHierarchyId,
                                         V_Description = b.v_Description,
                                         I_ParentId = b.i_ParentId
                                     }).ToList();
                return oAutorizationList;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public void DeleteRoleProfile(ref OperationResult pobjOperationResult, int pintSystemUserId, List<string> ClientSession)
        {

            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                // Obtener la entidad a eliminar
                var objEntitySource = (from a in dbContext.roleprofile
                                       //where a.i_SystemUserId == pintSystemUserId
                                       select a);

                foreach (var item in objEntitySource)
                {
                    dbContext.DeleteObject(item);
                }

                dbContext.SaveChanges();
                pobjOperationResult.Success = 1;
                // Llenar entidad Log
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }

        }

        public int GetRoleProfileCount(ref OperationResult pobjOperationResult, string filterExpression)
        {
            //mon.IsActive = true;
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                var query = from a in dbContext.roleprofile select a;

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

        public void UpdateRoleProfiles(ref OperationResult pobjOperationResult, List<roleprofileDto> ListRoleProfileUpdate, List<roleprofileDto> ListRoleProfileDelete, List<string> ClientSession)
        {
            try
            {
                if (ListRoleProfileUpdate.Count != 0)
                {
                    AddRoleProfiles(ref pobjOperationResult, ListRoleProfileUpdate, int.Parse(ClientSession[2]), ClientSession, false);
                    if (pobjOperationResult.Success == 0) return;
                }

                if (ListRoleProfileDelete.Count != 0)
                {
                    DeleteRoleProfiles(ref pobjOperationResult, ListRoleProfileDelete, ClientSession);
                    if (pobjOperationResult.Success == 0) return;
                }

                pobjOperationResult.Success = 1;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.AdditionalInformation = "SecurityBL.UpdateRoleProfiles()\nLinea:" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '));
                pobjOperationResult.ErrorMessage = ex.Message;
                pobjOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, pobjOperationResult);
            }
        }

        #endregion

        #region SystemUserNode
        public List<systemusernodeDto> GetSystemUserNode(ref OperationResult pobjOperationResult, int pintSystemUserId)
        {
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = (from a in dbContext.systemusernode
                             join b in dbContext.node on a.i_NodeId equals b.i_NodeId

                             where a.i_SystemUserId == pintSystemUserId &&
                                    a.i_IsDeleted == 0
                             select new systemusernodeDto
                             {
                                 i_SystemUserNodeId = a.i_SystemUserNodeId,
                                 i_NodeId = a.i_NodeId,
                                 v_RazonSocial = b.v_RazonSocial,
                                 v_RUC = b.v_RUC
                             }).ToList();

                pobjOperationResult.Success = 1;
                return query;

            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public void AddSystemUserNode(ref OperationResult pobjOperationResult, systemusernodeDto pobjDtoEntity, List<string> ClientSession)
        {
            //int SecuentialId = 0;
            //mon.IsActive = true;

            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();
                var objEntitySource = (from a in dbContext.systemusernode
                                       where a.i_SystemUserId == pobjDtoEntity.i_SystemUserId &&
                                            a.i_NodeId == pobjDtoEntity.i_NodeId
                                       select a).FirstOrDefault();

                if (objEntitySource != null)
                {
                    if (objEntitySource.i_IsDeleted == 0)   // Registro ya esta grabado
                    {
                        // validar que no se vuelva a registrar datos ya existentes
                        pobjOperationResult.ErrorMessage = "Este nodo ya existe para este nodo, agregue otro por favor.)";
                        pobjOperationResult.Success = 1;
                        return;
                    }
                    else if (objEntitySource.i_IsDeleted == 1)  // Registro macado como eliminado
                    {
                        // Actualizar registro (dar de alta al registro ya existente "no volver a insertar")
                        OperationResult objOperationResult2 = new OperationResult();

                        UpdateSystemUserNode(ref objOperationResult2,
                                                 (int)pobjDtoEntity.i_SystemUserId,
                                                 (int)pobjDtoEntity.i_NodeId,
                                                 ClientSession);

                        pobjOperationResult = objOperationResult2;
                        return;
                    }
                }
                else
                {
                    // Autogeneramos el Pk de la tabla


                    // Grabar nuevo registro
                    systemusernode objEntity;

                    //// Sub-consulta para alberto
                    //var objEntitySource11 = (from a in dbContext.systemparameter
                    //                         where a.i_GroupId == 119 && a.i_ParameterId == NodeServiceProfile.i_MasterServiceId
                    //                         select a).FirstOrDefault();

                    objEntity = systemusernodeAssembler.ToEntity(pobjDtoEntity);
                    objEntity.i_NodeId = pobjDtoEntity.i_NodeId;
                    objEntity.i_SystemUserId = pobjDtoEntity.i_SystemUserId;
                    objEntity.d_InsertDate = DateTime.Now;
                    objEntity.i_InsertUserId = int.Parse(ClientSession[2]);
                    objEntity.i_IsDeleted = 0;

                    dbContext.AddTosystemusernode(objEntity);

                    dbContext.SaveChanges();
                    pobjOperationResult.Success = 1;

                    // Llenar entidad Log

                    return;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                // Llenar entidad Log
                return;
            }
        }

        public void UpdateSystemUserNode(ref OperationResult pobjOperationResult, int pintSystemUserId, int pintNodeId, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var objEntitySource = (from a in dbContext.systemusernode
                                       where a.i_SystemUserId == pintSystemUserId &&
                                             a.i_NodeId == pintNodeId
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.d_UpdateDate = DateTime.Now;
                objEntitySource.i_UpdateUserId = Int32.Parse(ClientSession[2]);
                objEntitySource.i_IsDeleted = 0;


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
                return;
            }
        }

        public void DeleteSystemUserNode(ref OperationResult pobjOperationResult, int pintSystemUserNodeId, List<string> ClientSession)
        {
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var objEntitySource = (from a in dbContext.systemusernode
                                       where a.i_SystemUserNodeId == pintSystemUserNodeId
                                       select a).FirstOrDefault();

                // Crear la entidad con los datos actualizados
                objEntitySource.d_UpdateDate = DateTime.Now;
                objEntitySource.i_UpdateUserId = Int32.Parse(ClientSession[2]);
                objEntitySource.i_IsDeleted = 1;


                // Guardar los cambios
                dbContext.SaveChanges();
                pobjOperationResult.Success = 1;
                return;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return;
            }
        }
        #endregion

        #region FormActions

        public List<KeyValueDTO> GetFormAction(ref OperationResult pobjOperationResult, int pintNodeId, int pintRoleId, string pstrFormCode)
        {
            try
            {
                SAMBHSEntitiesModel dbContext = new SAMBHSEntitiesModel();

                var query = (from rnp in dbContext.roleprofile
                             join ah in dbContext.applicationhierarchy on rnp.i_ApplicationHierarchyId equals ah.i_ApplicationHierarchyId
                             where
                                    ah.v_Code.Substring(0, 6) == pstrFormCode &&
                                   ah.i_ApplicationHierarchyTypeId == 3 &&   // solo acciones
                                   rnp.i_IsDeleted == 0 && ah.i_IsDeleted == 0 && rnp.i_RoleId == pintRoleId
                             select new KeyValueDTO
                             {
                                 Value1 = ah.v_Description,
                                 Value2 = ah.v_Code
                             });

                List<KeyValueDTO> objFormAction = query.OrderBy(P => P.Value1).ToList();
                pobjOperationResult.Success = 1;
                return objFormAction;
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                return null;
            }
        }

        #endregion

        /// <summary>
        /// Obtiene la licencia de la pc por su id unico
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public string ObtenerLicenciaByUid(string uid)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModel())
                {
                    var lic = dbContext.licenses.FirstOrDefault(p => p.UID.Equals(uid));
                    return lic == null ? string.Empty : lic.v_License.Trim();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<licensesDto> ObtenerLicencias(string descripcion)
        {
            try
            {
                descripcion = string.IsNullOrWhiteSpace(descripcion) ? null : descripcion.Trim().ToLower();
                using (var dbContext = new SAMBHSEntitiesModel())
                {
                    var listado = dbContext.licenses.Where(p => descripcion == null || p.v_Descripcion.ToLower().Contains(descripcion)).ToList();
                    return listado.ToDTOs();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void EliminarLicencia(string uid)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModel())
                {
                    var lic = dbContext.licenses.FirstOrDefault(p => p.UID.Equals(uid));
                    if (lic == null) throw new Exception("Licencia no encontrada");
                    dbContext.licenses.DeleteObject(lic);
                    dbContext.SaveChanges();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool ActualizarLicencia(licensesDto licencia)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModel())
                {
                    var lic = dbContext.licenses.FirstOrDefault(p => p.UID.Equals(licencia.UID));
                    if (lic == null)
                    {
                        var maximo = SystemValidator.ObtenerLimiteLicencias;
                        var usadas = SystemValidator.ObtenerLicenciasUsadas;
                        if (maximo > 0 && usadas < maximo)
                        {
                            var entidad = licencia.ToEntity();
                            dbContext.licenses.AddObject(entidad);
                        }
                        else return false;
                    }
                    else
                    {
                        lic.t_FechaCreacion = DateTime.Now;
                        lic.v_Descripcion = licencia.v_Descripcion;
                        lic.v_License = licencia.v_License;
                        dbContext.licenses.ApplyCurrentValues(lic);
                    }

                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool InsertarLicencia(licensesDto licencia)
        {
            try
            {
                using (var dbContext = new SAMBHSEntitiesModel())
                {
                    var lic = dbContext.licenses.FirstOrDefault(p => p.UID.Equals(licencia.UID));
                    if (lic == null)
                    {
                        var entidad = licencia.ToEntity();
                        dbContext.licenses.AddObject(entidad);
                    }

                    dbContext.SaveChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        enum Proceso
        {
            Insertar = 1,
            Editar = 2,
            Eliminar = 3
        }

    }
}
