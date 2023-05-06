using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using Dapper;
using Infragistics.Win.UltraWinEditors;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using System.Data;
using SAMBHS.Common.DataModel;


namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public class ProtocoloBl
    {
        public static bool BuscarProtocoloPropuesto(string organizationEmployerId,  int masterServiceTypeId, int masterServiceId, string groupOccupationName ,int esoTypeId)
        {
            SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

            List<ProtocolDto> query = (from a in dbContext.buscarprotocolopropuesto_sp(organizationEmployerId, masterServiceTypeId, masterServiceId, groupOccupationName, esoTypeId)
                                       select new ProtocolDto
                                       {
                                           v_ProtocolId = a.v_ProtocolId
                                       }).ToList();


            return query.Count > 0;

            //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            //{
            //    if (cnx.State != ConnectionState.Open) cnx.Open();

            //var query = @"SELECT v_ProtocolId " +
            //            "FROM protocol A " +
            //            "INNER JOIN groupoccupation B on A.v_EmployerLocationId = b.v_LocationId " +
            //            "WHERE ( '" + organizationEmployerId + "' = v_EmployerOrganizationId and " + masterServiceTypeId + "  = i_masterServiceTypeId   and B.v_Name = '" + groupOccupationName + "' and  i_masterServiceId = " + masterServiceId + " and  i_esoTypeId = " + esoTypeId + ")";

            //    var data = cnx.Query<ProtocolDto>(query).ToList();
            //    return data.Count > 0;
            //}
        }

        public static List<ProtocolComponentList> GetProtocolComponents(string pstrProtocolId)
        {
            SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

            List<ProtocolComponentList> query = (from a in dbContext.getprotocolcomponents_sp(pstrProtocolId)
                                                 select new ProtocolComponentList
                                       {
                                           ComponentId = a.ComponentId,
                                           ComponentName = a.ComponentName,
                                           Porcentajes = a.Porcentajes,
                                           ProtocolComponentId = a.ProtocolComponentId,
                                           Price = a.Price,
                                           Operator = a.Operator.ToString(),
                                           Age = Convert.ToInt32(a.Age),
                                           Gender = a.Gender,
                                           IsConditionalImc = Convert.ToInt32(a.IsConditionalImc),
                                           Imc = Convert.ToDecimal(a.Imc),
                                           IsConditional = a.IsConditional,
                                           IsAdditional = a.IsAdditional,
                                           ComponentTypeName = a.ComponentTypeName,
                                           GenderId = Convert.ToInt32(a.GenderId),
                                           GrupoEtarioId = Convert.ToInt32(a.GrupoEtarioId),
                                           IsConditionalId = Convert.ToInt32(a.IsConditionalId),
                                           OperatorId = Convert.ToInt32(a.OperatorId),
                                           i_IsDeleted = Convert.ToInt32(a.i_IsDeleted),
                                           v_CategoryName = a.v_CategoryName,
                                           CategoryId = Convert.ToInt32(a.CategoryId)
                                       }).ToList();


            return query;

//            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//            {
//                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

//                var query = @"SELECT a.v_ComponentId as ComponentId,
//                        b.v_Name as ComponentName,sp2.v_Field as Porcentajes,v_ProtocolComponentId as ProtocolComponentId,
//                                                r_Price as Price,a.i_OperatorId as Operator,i_Age as Age,sp4.v_Value1 as  Gender,
//                            i_IsConditionalIMC as IsConditionalImc,r_Imc as Imc, 
//                            case when a.i_IsConditionalId = 1 then 'si' else 'no'end as IsConditional ,
//                            i_isAdditional as IsAdditional,sp5.v_Value1 as ComponentTypeName,i_GenderId as GenderId,i_GrupoEtarioId as GrupoEtarioId,
//                    i_IsConditionalId as IsConditionalId,i_OperatorId as OperatorId,a.i_IsDeleted,sp2.v_Value1 as v_CategoryName ,
//                        i_CategoryId as CategoryId " +
//                            "FROM protocolcomponent a " +
//                            "INNER JOIN component b on a.v_ComponentId = b.v_ComponentId " +
//                            "LEFT JOIN systemparameter sp2 on b.i_CategoryId = sp2.i_ParameterId and sp2.i_GroupId = 116 " +
//                            "LEFT JOIN systemparameter sp3 on a.i_OperatorId = sp3.i_ParameterId and sp3.i_GroupId = 117 " +
//                            "LEFT JOIN systemparameter sp4 on a.i_GenderId = sp4.i_ParameterId and sp4.i_GroupId = 130 " +
//                            "LEFT JOIN systemparameter sp5 on b.i_ComponentTypeId = sp5.i_ParameterId and sp5.i_GroupId = 126 " +

//                            "WHERE ('" + pstrProtocolId + "'  = a.v_ProtocolId ) " +
//                            "AND a.i_IsDeleted = 0";

//                var data = cnx.Query<ProtocolComponentList>(query).ToList();
//                return data;
//            }
        }

        public void AddProtocolSystemUser(List<protocolsystemuserDto> ListProtocolSystemUserDto, int? pintSystemUserId)
        {
            try
            {
                var secuentialId = UtilsSigesoft.GetNextSecuentialId(44).SecuentialId;
                var newId = UtilsSigesoft.GetNewId(9, secuentialId, "PU");
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    foreach (var item in ListProtocolSystemUserDto)
                    {
                        var query = @"INSERT INTO protocolsystemuser (v_ProtocolSystemUserId,i_SystemUserId, v_ProtocolId, i_IsDeleted, i_InsertUserId, d_InsertDate) " +
                                    " VALUES ('"+ newId + "',"+
                                    "" + pintSystemUserId + ", " +
                                    "'" + item.v_ProtocolId+ "', " +
                                    "0,11,GETDATE())";

                        cnx.Execute(query);    
                    }
                    
                }

            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public string AddProtocol(ProtocolDto pobjProtocol,List<ProtocolComponentList> pobjProtocolComponent)
        {
            try
            {
                var secuentialId = UtilsSigesoft.GetNextSecuentialId(20).SecuentialId;
                var newId = UtilsSigesoft.GetNewId(9, secuentialId, "PR");
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query =
                        @"INSERT INTO protocol (v_ProtocolId,v_Name,v_EmployerOrganizationId,v_EmployerLocationId,i_EsoTypeId,v_GroupOccupationId,v_CustomerOrganizationId,v_CustomerLocationId,v_NombreVendedor,v_WorkingOrganizationId,v_WorkingLocationId,v_CostCenter,i_MasterServiceTypeId,i_MasterServiceId,i_HasVigency,i_IsActive,i_IsDeleted,i_InsertUserId,d_InsertDate,v_AseguradoraOrganizationId) " +
                        "VALUES('" + newId + "', " +
                        "'" + pobjProtocol.v_Name + "', " +
                        "'" + pobjProtocol.v_EmployerOrganizationId + "', " +
                        "'" + pobjProtocol.v_EmployerLocationId + "', " +
                        "" + pobjProtocol.i_EsoTypeId + ", " +
                        "'" + pobjProtocol.v_GroupOccupationId + "', " +
                        "'" + pobjProtocol.v_CustomerOrganizationId + "', " +
                        "'" + pobjProtocol.v_CustomerLocationId + "', " +
                        "'" + pobjProtocol.v_NombreVendedor + "', " +
                        "'" + pobjProtocol.v_WorkingOrganizationId + "', " +
                        "'" + pobjProtocol.v_WorkingLocationId + "', " +
                        "'" + pobjProtocol.v_CostCenter + "', " +
                        "" + pobjProtocol.i_MasterServiceTypeId + ", " +
                        "" + pobjProtocol.i_MasterServiceId + ", " +
                        "" + pobjProtocol.i_HasVigency + ", " +
                        "" + pobjProtocol.i_IsActive + ", " +
                        "0,11, GETDATE(),'')";
                    cnx.Execute(query);

                    foreach (var item in pobjProtocolComponent)
                    {
                        var secuentialId1 = UtilsSigesoft.GetNextSecuentialId(21).SecuentialId;
                        var newId1 = UtilsSigesoft.GetNewId(9, secuentialId1, "PC");
                        var query1 =
                        @"INSERT INTO protocolcomponent (v_ProtocolComponentId,v_ProtocolId,v_ComponentId,r_Price,i_OperatorId,i_Age ,i_GenderId, i_GrupoEtarioId , i_IsConditionalId,i_IsConditionalIMC,r_Imc, i_IsAdditional, i_IsDeleted,i_InsertUserId,d_InsertDate) " +
                        "VALUES('" + newId1 + "', " +
                        "'" + newId + "', " +
                        "'" + item.ComponentId + "', " +
                        "" + item.Price + ", " +
                        "" + item.OperatorId + ", " +
                        "" + item.Age + ", " +
                        "" + item.GenderId + ", " +
                        "" + item.GrupoEtarioId + ", " +
                        "" + item.IsConditionalId + ", " +
                        "" + item.IsConditionalImc + ", " +
                        "" + item.Imc + ", " +
                        "" + item.IsAdditional + ", " +
                        "0,11, GETDATE())";
                          cnx.Execute(query1);
                    }


                    #region ProtocolSystemUser

                    var extUser = (@"select i_SystemUserId,v_SystemUserByOrganizationId from systemuser where i_SystemUserTypeId = 2");
                    var oExtUser = cnx.Query<SystemUserSigesoft>(extUser).ToList();

                    var extUserWithCustomer = oExtUser.FindAll(p => p.v_SystemUserByOrganizationId == pobjProtocol.v_CustomerOrganizationId).ToList();
                    var extUserWithEmployer = oExtUser.FindAll(p => p.v_SystemUserByOrganizationId == pobjProtocol.v_EmployerOrganizationId).ToList();
                    var extUserWithWorking = oExtUser.FindAll(p => p.v_SystemUserByOrganizationId == pobjProtocol.v_WorkingOrganizationId).ToList();

                    foreach (var extUs in extUserWithCustomer)
                    {
                        var getPermissionsExtUser = (@"select i_ApplicationHierarchyId from protocolsystemuser where i_SystemUserId = " + extUs.i_SystemUserId + " group by i_ApplicationHierarchyId");

                        var oGetPermissionsExtUser = cnx.Query<ProtocolSystemUSer>(getPermissionsExtUser).ToList();

                        var list = new List<protocolsystemuserDto>();
                        foreach (var perm in oGetPermissionsExtUser)
                        {
                            var oProtocolSystemUserDto = new protocolsystemuserDto();
                            oProtocolSystemUserDto.i_SystemUserId = extUs.i_SystemUserId;
                            oProtocolSystemUserDto.v_ProtocolId = newId;
                            oProtocolSystemUserDto.i_ApplicationHierarchyId = perm.i_ApplicationHierarchyId;
                            list.Add(oProtocolSystemUserDto);
                        }

                    }

                    foreach (var extUs in extUserWithEmployer)
                    {
                        var getPermissionsExtUser = (@"select i_ApplicationHierarchyId from protocolsystemuser where i_SystemUserId = " + extUs.i_SystemUserId + " group by i_ApplicationHierarchyId");

                        var oGetPermissionsExtUser = cnx.Query<ProtocolSystemUSer>(getPermissionsExtUser).ToList();


                        var list = new List<protocolsystemuserDto>();
                        foreach (var perm in oGetPermissionsExtUser)
                        {
                            var oProtocolSystemUserDto = new protocolsystemuserDto();
                            oProtocolSystemUserDto.i_SystemUserId = extUs.i_SystemUserId;
                            oProtocolSystemUserDto.v_ProtocolId = newId;
                            oProtocolSystemUserDto.i_ApplicationHierarchyId = perm.i_ApplicationHierarchyId;
                            list.Add(oProtocolSystemUserDto);
                        }

                        AddProtocolSystemUser(list, extUs.i_SystemUserId);
                    }


                    foreach (var extUs in extUserWithWorking)
                    {
                        var getPermissionsExtUser = (@"select i_ApplicationHierarchyId from protocolsystemuser where i_SystemUserId = " + extUs.i_SystemUserId + " group by i_ApplicationHierarchyId");

                        var oGetPermissionsExtUser = cnx.Query<ProtocolSystemUSer>(getPermissionsExtUser).ToList();

                        var list = new List<protocolsystemuserDto>();
                        foreach (var perm in oGetPermissionsExtUser)
                        {
                            var oProtocolSystemUserDto = new protocolsystemuserDto();
                            oProtocolSystemUserDto.i_SystemUserId = extUs.i_SystemUserId;
                            oProtocolSystemUserDto.v_ProtocolId = newId;
                            oProtocolSystemUserDto.i_ApplicationHierarchyId = perm.i_ApplicationHierarchyId;
                            list.Add(oProtocolSystemUserDto);
                        }

                        AddProtocolSystemUser(list, extUs.i_SystemUserId);
                    }



                    #endregion
                }
                    
                
                return newId;
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

       public class SystemUserSigesoft
        {
            public int i_SystemUserId { get; set; }
            public string v_SystemUserByOrganizationId { get; set; }
        }

       public class ProtocolSystemUSer
        {
            public string v_ProtocolSystemUserId { get; set; }
            public int i_SystemUserId { get; set; }
            public string v_ProtocolId { get; set; }
            public int i_ApplicationHierarchyId { get; set; }
            public int i_IsDeleted { get; set; }
            public int i_InsertUserId { get; set; }
            public DateTime? d_InsertDate { get; set; }
            public int i_UpdateUserId { get; set; }
            public DateTime? d_UpdateDate { get; set; }

        }

       public class protocolsystemuserDto
        {
            public string v_ProtocolSystemUserId { get; set; }
            public int i_SystemUserId { get; set; }
            public string v_ProtocolId { get; set; }
            public int i_ApplicationHierarchyId { get; set; }
            public int i_IsDeleted { get; set; }
            public int i_InsertUserId { get; set; }
            public DateTime? d_InsertDate { get; set; }
            public int i_UpdateUserId { get; set; }
            public DateTime? d_UpdateDate { get; set; }

        }
        public static void UpdateProtocol(ProtocolDto pobjProtocol, int medicoTrantanteId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != ConnectionState.Open) cnx.Open();

                var query = @" update protocol set i_MasterServiceTypeId = " + pobjProtocol.i_MasterServiceTypeId + ", i_MasterServiceId = " + pobjProtocol.i_MasterServiceId + ", v_GroupOccupationId= '" + pobjProtocol.v_GroupOccupationId + "' ,i_EsoTypeId =" + pobjProtocol.i_EsoTypeId + ", v_CustomerOrganizationId = '" + pobjProtocol.v_CustomerOrganizationId + "' , v_CustomerLocationId ='" + pobjProtocol.v_CustomerLocationId + "', v_WorkingOrganizationId ='" + pobjProtocol.v_WorkingOrganizationId + "', v_WorkingLocationId = '" + pobjProtocol.v_WorkingLocationId + "',  v_EmployerOrganizationId ='" + pobjProtocol.v_EmployerOrganizationId + "', v_EmployerLocationId ='" + pobjProtocol.v_EmployerLocationId + "' where v_ProtocolId = '" + pobjProtocol.v_ProtocolId + "'";

                cnx.Execute(query);
            }
        }


        public static void UpdateServiceComponent(string protocolId, int serviceTypeId, int medicoTrantanteId, string serviceId, string centroCosto, int masterservice, int usuarioactualiza, string Area, string planSeguro, string Licencia, int ModalidadTrabajo, int mkt, int establecimiento, int vendedorExterno, int medicoExterno)

        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != ConnectionState.Open) cnx.Open();

                //, i_MasterServiceId= "+masterservice+"
                string plan = "null";
                if (planSeguro != "-1" && planSeguro != "")
                {
                    plan = planSeguro;
                }

                //string areaTrabajador = "null";
                //if (Area != "")
                //{
                //    areaTrabajador = Area;
                //}

                var service = @" update service set  v_ProtocolId = '" + protocolId + "', v_centrocosto = '" + centroCosto + "' , i_MasterServiceId= " + masterservice + " , i_UpdateUserId = " + usuarioactualiza + ", v_Area = '" + Area + "' , i_PlanId = " + plan + ", i_ModTrabajo = " + ModalidadTrabajo + ", i_ProcedenciaPac_Mkt = " + mkt + ", i_Establecimiento = '" + establecimiento + "', i_VendedorExterno = '" + vendedorExterno + "', i_MedicoSolicitanteExterno = '" + medicoExterno + "'  where v_ServiceId = '" + serviceId + "'";

                cnx.Execute(service);


                var calendar = @"update calendar set  i_ServiceTypeId = " + serviceTypeId + ", i_ServiceId = " + masterservice + " , i_UpdateUserId = " + usuarioactualiza + " where v_ServiceId = '" + serviceId + "'";

                cnx.Execute(calendar);

                if (serviceTypeId == 34)
                {
                    var service2 = @" update service set  v_LicenciaConducir = '" + Licencia + "'  where v_ServiceId = '" + serviceId + "'";

                    cnx.Execute(service2);
                }
                //if (comentario != "" || comentario != null)
                //{
                //    var coment = @" update service set  v_ComentaryUpdate = '" + comentario + "'  where v_ServiceId = '" + serviceId + "'";

                //    cnx.Execute(coment);
                //}


                var medico = @"update servicecomponent set i_MedicoTratanteId = '" + medicoTrantanteId + "' where v_ServiceId = '" + serviceId + "' and i_ConCargoA = 0";

                cnx.Execute(medico);
                //var query1 = @" update servicecomponent set  i_MedicoTratanteId = " + medicoTrantanteId + " where v_ServiceComponentId = '" + serviceComponentId + "'";

                //cnx.Execute(query1);
            }
        }


        public void UpdateServiceComponentMedicTratante(string v_ServiceComponent, int medicoTratanteId, int medicoIndica)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != ConnectionState.Open) cnx.Open();

                var medico = @"update servicecomponent set i_MedicoTratanteId = '" + medicoTratanteId + "' , i_ApplicantMedic = '" + medicoIndica + "' where v_ServiceComponentId = '" + v_ServiceComponent + "'";

                cnx.Execute(medico);
            }
        }

        public static string GetCommentaryUpdateByserviceId(string serviceId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != ConnectionState.Open) cnx.Open();

                var query = "select v_ComentaryUpdate from service where v_ServiceId = '" + serviceId + "'";

                var comentario = cnx.Query<Service>(query).FirstOrDefault();
                if (comentario.v_ComentaryUpdate == null) return "";

                return comentario.v_ComentaryUpdate;
            }
        }

        public static ServiceComponentDat GetServiceComponent(string pServiceComponent)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select sc.v_ServiceComponentId as 'ServiceComponent', sc.i_MedicoTratanteId as 'MedicoTratante', sc.i_ApplicantMedic as 'MedicoIndica' from servicecomponent sc where v_ServiceComponentId = '" + pServiceComponent + "'";

                    var data = cnx.Query<ServiceComponentDat>(query).FirstOrDefault();
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public systemuserDto GetUsuario(string usuario)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != ConnectionState.Open) cnx.Open();

                    var query = "select i_SystemUserId, v_PersonId, v_UserName, v_Password, v_ComentaryUpdate from systemuser where v_UserName = '" + usuario + "'";

                    var systemUser_Obj = cnx.Query<systemuserDto>(query).FirstOrDefault();
                    return systemUser_Obj;
                }
            }
            catch (Exception ex)
            {
                return null;
            }


        }

       
    }

    public class Service
    {
        public string v_ComentaryUpdate { get; set; }
    }

    public class systemuserDto
    {
        public Int32 i_SystemUserId { get; set; }

        public String v_PersonId { get; set; }

        public String v_UserName { get; set; }

        public String v_Password { get; set; }

        public String v_SecretQuestion { get; set; }

        public String v_SecretAnswer { get; set; }

        public String v_ComentaryUpdate { get; set; }
    }
}
