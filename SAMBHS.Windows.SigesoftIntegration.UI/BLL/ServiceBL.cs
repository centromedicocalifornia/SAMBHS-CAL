using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic;
using System.Collections;
using System.Transactions;
using System.Threading;

using SAMBHS.Windows.SigesoftIntegration.UI;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using SAMBHS.Windows.SigesoftIntegration.UI.Reports;

using System.ComponentModel;
using System.Data.SqlClient;
using System.Globalization;

using System.Runtime.CompilerServices;

using System.Windows.Forms;
using Dapper;
using Infragistics.Win.UltraWinEditors;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using System.Data;
using System.Drawing.Text;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.BE.Custom;


namespace SAMBHS.Windows.SigesoftIntegration.UI.BLL
{
    public class ServiceBL
    {
        public OrganizationDto1 GetInfoMedicalCenter()
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query =
                        "select v_OrganizationId, v_IdentificationNumber,v_Name, v_Address, v_PhoneNumber, v_Mail, v_ContacName, v_Contacto, b_Image " +
                        "from organization where v_OrganizationId = 'N009-OO000000052' ";
                    return cnx.Query<OrganizationDto1>(query).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public PacientList GetDatosTrabajador(string pstNroServicio)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    //v_PersonId,v_FirstName,v_FirstLastName,v_SecondLastName,i_DocTypeId,v_DocNumber,i_SexTypeId,d_Birthdate,i_IsDeleted,i_MaritalStatusId,v_BirthPlace,i_DistrictId,i_ProvinceId,i_DepartmentId,i_ResidenceInWorkplaceId,v_Mail,v_AdressLocation,v_CurrentOccupation,i_AltitudeWorkId,v_ExploitedMineral,i_LevelOfId,i_BloodGroupId,i_BloodFactorId,v_ResidenceTimeInWorkplace,i_TypeOfInsuranceId,i_NumberLivingChildren,i_NumberDependentChildren,i_NroHermanos,v_TelephoneNumber,i_Relationship,i_PlaceWorkId
                    var query =
                    "select s.v_ServiceId as v_IdService, s.d_GlobalExpirationDate, s.d_UpdateDate, s.d_ServiceDate as FechaServicio, p.v_PersonId, p.v_FirstName, p.v_FirstLastName, p.v_SecondLastName, p.i_DocTypeId, " +
                    " p.v_DocNumber, (Select v_Value1 from systemparameter where i_ParameterId = p.i_SexTypeId and i_GroupId = 100 ) as Genero, p.d_Birthdate, " +
                    " (Select v_Value1 from systemparameter where i_ParameterId = p.i_MaritalStatusId and i_GroupId = 101 ) as v_MaritalStatus, p.v_BirthPlace, " +
                    " (Select v_Value1 from datahierarchy where i_ItemId = p.i_DistrictId and i_GroupId = 113 ) as v_DistrictName, " +
                    " (Select v_Value1 from datahierarchy where i_ItemId = p.i_ProvinceId and i_GroupId = 113 ) as v_ProvinceName," +
                    " (Select v_Value1 from datahierarchy where i_ItemId = p.i_DepartmentId and i_GroupId = 113 ) as v_DepartamentName, p.i_ResidenceInWorkplaceId, p.v_Mail, p.v_AdressLocation, " +
                    " p.v_CurrentOccupation, p.i_AltitudeWorkId, p.v_ExploitedMineral, " +
                    " (Select v_Value1 from datahierarchy where i_ItemId = p.i_LevelOfId and i_GroupId = 108 ) as GradoInstruccion, " +
                    " (Select v_Value1 from systemparameter where i_ParameterId = p.i_BloodGroupId and i_GroupId = 154 ) as v_BloodGroupName, " +
                    " (Select v_Value1 from systemparameter where i_ParameterId = p.i_BloodFactorId and i_GroupId = 154 ) as v_BloodFactorName, p.v_ResidenceTimeInWorkplace, p.i_TypeOfInsuranceId, " +
                    " p.i_NumberLivingChildren, p.i_NumberDependentChildren, p.i_NroHermanos, p.v_TelephoneNumber, p.i_Relationship, p.i_PlaceWorkId, p.b_FingerPrintTemplate, p.b_FingerPrintImage, " +
                    " p.b_RubricImage, p.t_RubricImageText, p.b_PersonImage, p.v_Religion, p.v_Nacionalidad, p.v_ResidenciaAnterior " +
                    " from service as s join person as p on s.v_PersonId = p.v_PersonId " +
                    " where s.v_ServiceId = '" + pstNroServicio + "' and s.i_IsDeleted = 0 ";
                    return cnx.Query<PacientList>(query).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ProtocoloDescuentosDto GetDescuentosProtocolo(string _ServiceId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query =
                    "select s.v_ServiceId, p.v_ProtocolId, p.d_PrecioConsulta, p.d_DescuentoLaboratorio,  " +
                    " p.d_DescuentoRayosX, p.d_DescuentoEcografias, p.d_DescuentoFarmacia, p.d_DescuentoOdontologia, " +
                    " p.d_CamaHosp, p.d_SalaOperaciones, p.d_PrecioAmbulancia  " +
                    " from service s " +
                    " JOIN protocol p on s.v_ProtocolId = p.v_ProtocolId " +
                    " where s.v_ServiceId = '" + _ServiceId + "'";
                    return cnx.Query<ProtocoloDescuentosDto>(query).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CategoriaExamen getCategoriaExamen(string _ComponentName)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query =
                    "select c.v_ComponentId, c.v_Name, c.i_CategoryId, sp.v_Value1  " +
                    " from component c " +
                    " join systemparameter sp on c.i_CategoryId = sp.i_ParameterId " +
                    " where c.v_Name = '" + _ComponentName + "' and sp.i_GroupId = 116 and c.i_IsDeleted = 0 ";
                    return cnx.Query<CategoriaExamen>(query).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public List<Categoria> GetAllComponents(ref OperationResult pobjOperationResult, int? filterType, string name)
        {

            int isDeleted = (int)SiNo.NO;
            string codigoSegus = "";
            string nameCategory = "";
            string nameComponent = "";
            string nameSubCategory = "";
            string componentId = "";
            if (filterType == 1)
            {
                codigoSegus = name;

            }
            else if (filterType == 2)
            {
                nameCategory = name;
            }
            else if (filterType == 4)
            {
                nameComponent = name;
            }
            else if (filterType == 3)
            {
                nameSubCategory = name;
            }
            else if (filterType == 5)
            {
                componentId = name;
            }


            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                //System.Linq.IQueryable<Categoria> query;
                List<Categoria> obj = new List<Categoria>();
                if (name == "")
                {
                    using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                    {
                        var query = @"select C.v_ComponentId as v_ComponentId,
                            C.v_Name as v_ComponentName,
                            C.v_CodigoSegus as v_CodigoSegus,
                            C.i_CategoryId as i_CategoryId,
                            CASE WHEN C.i_CategoryId = -1 THEN C.v_Name ELSE F.v_Value1 END as v_CategoryName
                            from component C
                            join systemparameter F on C.i_CategoryId = F.i_ParameterId and  F.i_GroupId = 116 
                            where C.i_IsDeleted = 0";

                        var List = cnx.Query<Categoria>(query).ToList();

                        var objData = List.AsEnumerable()
                            .Where(s => s.i_CategoryId != -1)
                            .GroupBy(x => x.i_CategoryId)
                            .Select(group => group.First());

                        obj = objData.ToList();

                    }

                }
                else if (filterType == 5)
                {
                    using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                    {
                       var query = @"select C.v_ComponentId as v_ComponentId,
                            C.v_Name as v_ComponentName,
                            C.v_CodigoSegus as v_CodigoSegus,
                            C.i_CategoryId as i_CategoryId,
                            CASE WHEN C.i_CategoryId = -1 THEN C.v_Name ELSE F.v_Value1 END as v_CategoryName
                            from component C
                            join systemparameter F on C.i_CategoryId = F.i_ParameterId and  F.i_GroupId = 116 
                            where C.i_IsDeleted = 0 and C.v_ComponentId = '" + componentId + "'";
                        var List = cnx.Query<Categoria>(query).ToList();

                        var objData = List.AsEnumerable()
                            .Where(s => s.i_CategoryId != -1)
                            .GroupBy(x => x.i_CategoryId)
                            .Select(group => group.First());

                        obj = objData.ToList();
                    }
                }
                else
                {
                    using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                    {
                        var query = @"select  v_ComponentId = C.v_ComponentId,
                            v_ComponentName = C.v_Name,
                            v_CodigoSegus = C.v_CodigoSegus,
                            i_CategoryId = C.i_CategoryId,
                            CASE WHEN C.i_CategoryId = -1 THEN C.v_Name ELSE F.v_Value1 END as v_CategoryName
                            from component C
                            join systemparameter F on C.i_CategoryId = F.i_ParameterId and F.i_GroupId = 116 
                            join systemparameter G on F.i_ParameterId = G.i_ParentParameterId and G.i_GroupId = 116 
                            where C.i_IsDeleted = 0 
                            and (G.v_Value1 like '% " + nameSubCategory + @"%' 
                            and C.v_Name like '% " + nameComponent + @"%' 
                            and F.v_Value1 like '% " + nameCategory + @"%' 
                            and C.v_CodigoSegus like '% " + codigoSegus + @"%'";

                        var List = cnx.Query<Categoria>(query).ToList();

                        var objData = List.AsEnumerable()
                            .Where(s => s.i_CategoryId != -1)
                            .GroupBy(x => x.i_CategoryId)
                            .Select(group => group.First());

                        obj = objData.ToList();
                    }

                }


                
                Categoria objCategoriaList;
                List<Categoria> Lista = new List<Categoria>();

                //int CategoriaId_Old = 0;
                for (int i = 0; i < obj.Count(); i++)
                {
                    objCategoriaList = new Categoria();

                    objCategoriaList.i_CategoryId = obj[i].i_CategoryId.Value;
                    objCategoriaList.v_CategoryName = obj[i].v_CategoryName;

                    var x = obj.ToList().FindAll(p => p.i_CategoryId == obj[i].i_CategoryId.Value);

                    x.Sort((z, y) => z.v_ComponentName.CompareTo(y.v_ComponentName));
                    ComponentDetailList objComponentDetailList;
                    List<ComponentDetailList> ListaComponentes = new List<ComponentDetailList>();
                    foreach (var item in x)
                    {
                        objComponentDetailList = new ComponentDetailList();
                        objComponentDetailList.v_ComponentId = item.v_ComponentId;
                        objComponentDetailList.v_ComponentName = item.v_ComponentName;
                        //objComponentDetailList.v_ServiceComponentId = item.v_ServiceComponentId;
                        var list = ListaComponentes.Find(z => z.v_ComponentId == item.v_ComponentId);
                        if (list == null)
                        {
                            ListaComponentes.Add(objComponentDetailList);
                        }

                    }

                    objCategoriaList.Componentes = ListaComponentes;

                    Lista.Add(objCategoriaList);

                }



                pobjOperationResult.Success = 1;
                return Lista;
            }
            catch (Exception ex)
            {
                //pobjOperationResult.Success = 0;
                //pobjOperationResult.ExceptionMessage = Common.Utils.ExceptionFormatter(ex);
                return null;
            }
        }


        public MedicoTratanteAtenciones GetMedicoTratante(string pstrServiceId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    //v_PersonId,v_FirstName,v_FirstLastName,v_SecondLastName,i_DocTypeId,v_DocNumber,i_SexTypeId,d_Birthdate,i_IsDeleted,i_MaritalStatusId,v_BirthPlace,i_DistrictId,i_ProvinceId,i_DepartmentId,i_ResidenceInWorkplaceId,v_Mail,v_AdressLocation,v_CurrentOccupation,i_AltitudeWorkId,v_ExploitedMineral,i_LevelOfId,i_BloodGroupId,i_BloodFactorId,v_ResidenceTimeInWorkplace,i_TypeOfInsuranceId,i_NumberLivingChildren,i_NumberDependentChildren,i_NroHermanos,v_TelephoneNumber,i_Relationship,i_PlaceWorkId
//                    var query = @"declare @resultado varchar(max);
//                                declare @servicio varchar(16) = '"+pstrServiceId+@"' ;
//                                select @resultado = (select case when (select  top 1 syu.i_SystemUserId 
//                                from servicecomponent scu 
//                                left join systemuser syu on scu.i_MedicoTratanteId = syu.i_SystemUserId 
//                                where scu.v_ServiceId = @servicio
//                                and scu.r_Price != 0   and i_IsRequiredId = 1) is null then (
//                                 (select  top 1  syu.i_SystemUserId 
//                                from servicecomponent scu 
//                                left join systemuser syu on scu.i_MedicoTratanteId = syu.i_SystemUserId   
//                                where scu.v_ServiceId = @servicio  and i_IsRequiredId = 1) ) ELSE  
//                                (select top 1 syu.i_SystemUserId 
//                                from servicecomponent scu    
//                                left join systemuser syu on scu.i_MedicoTratanteId = syu.i_SystemUserId          
//                                where scu.v_ServiceId = @servicio and scu.r_Price != 0 and i_IsRequiredId = 1) END)
//
//                                select p.v_FirstLastName + ' ' + p.v_SecondLastName + ', ' + p.v_FirstName as Nombre,
//                                CASE WHEN pro.v_ProfessionalCode IS NULL THEN '-' ELSE pro.v_ProfessionalCode END as Colegiatura, 
//                                CASE WHEN p.v_AdressLocation IS NULL THEN '-' ELSE p.v_AdressLocation END as Direccion  
//                                from systemuser s
//                                join person p on s.v_PersonId = p.v_PersonId
//                                left join professional as pro on p.v_PersonId = pro.v_PersonId  
//                                where s.i_SystemUserId = @resultado ";

                    var query = @"  declare @resultado varchar(max);
                                declare @servicio varchar(16) = '"+pstrServiceId+@"' ;
                                select @resultado = (select 
								CASE WHEN (select  top 1 syu.i_SystemUserId 
								from servicecomponent scu left join systemuser syu on scu.i_MedicoTratanteId = syu.i_SystemUserId   
								left join person pru on syu.v_PersonId = pru.v_PersonId   
								where scu.v_ServiceId = d.v_ServiceId   and scu.r_Price != 0   and i_IsRequiredId = 1 order by scu.v_ServiceComponentId) IS NULL 
								THEN (
								 (select  top 1 syu.i_SystemUserId 
								 from servicecomponent scu 
								 left join systemuser syu on scu.i_MedicoTratanteId = syu.i_SystemUserId   
								 left join person pru on syu.v_PersonId = pru.v_PersonId   
								 where scu.v_ServiceId = d.v_ServiceId    
								 and i_IsRequiredId = 1 order by scu.v_ServiceComponentId) ) 
								 ELSE   (select top 1 syu.i_SystemUserId   
									from servicecomponent scu    
									left join systemuser syu on scu.i_MedicoTratanteId = syu.i_SystemUserId       
									left join person pru on syu.v_PersonId = pru.v_PersonId      
									where scu.v_ServiceId = d.v_ServiceId       and i_IsRequiredId = 1 order by scu.v_ServiceComponentId) END  AS 'ID_MEDICO'
									from service d
									where d.v_ServiceId = @servicio )

                                select p.v_FirstLastName + ' ' + p.v_SecondLastName + ', ' + p.v_FirstName as Nombre,
                                CASE WHEN pro.v_ProfessionalCode IS NULL THEN '-' ELSE pro.v_ProfessionalCode END as Colegiatura, 
                                CASE WHEN p.v_AdressLocation IS NULL THEN '-' ELSE p.v_AdressLocation END as Direccion  
                                from systemuser s
                                join person p on s.v_PersonId = p.v_PersonId
                                left join professional as pro on p.v_PersonId = pro.v_PersonId  
                                where s.i_SystemUserId = @resultado";

                    return cnx.Query<MedicoTratanteAtenciones>(query).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
       
        public int GetAge(DateTime FechaNacimiento)
        {
            return int.Parse((DateTime.Today.AddTicks(-FechaNacimiento.Ticks).Year - 1).ToString());

        }

        public List<ListadoCovid> GetServicesByCovid(string value, DateTime inicio, DateTime fin)
        {
            try
            {
                fin = fin.Date;

                DateTime newFin = fin.AddHours(24);
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    //var query =
                    //    "select s.v_ServiceId as 'SERVICIO', "+
                    //    " p.v_FirstName + ' ' + p.v_FirstLastName + ' ' + p.v_SecondLastName as 'PACIENTE', " +
                    //    " c.v_Name as 'EXAMEN', " +
                    //    " sc.r_Price as 'PRECIO', " + 
                    //    " s.d_ServiceDate as 'FECHA', " + 
                    //    " s.v_ComprobantePago as 'COMPROBANTE', " +
                    //    " prot.v_Name as 'PROTOCOLO', " +
                    //    " (select TOP 1 CASE WHEN scfv.v_Value1 IS NULL THEN '- - -' ELSE sp.v_Value1 END " +
                    //    " from servicecomponent scp " +
                    //    " join servicecomponentfields scf on scp.v_ServiceComponentId = scf.v_ServiceComponentId " +
                    //    " left join servicecomponentfieldvalues scfv on scf.v_ServiceComponentFieldsId = scfv.v_ServiceComponentFieldsId " +
                    //    " left join systemparameter sp on sp.i_GroupId = 388 and scfv.v_Value1 = sp.i_ParameterId " +
                    //    " where scp.v_ServiceId = s.v_ServiceId and (scf.v_ComponentFieldId = 'N009-MF000006038' or scf.v_ComponentFieldId = 'N009-MF000006061')) as 'RESULTADO', " +
                    //    " CASE WHEN prot.i_MasterServiceTypeId = 9 THEN 'PARTICULAR' "+
                    //    " WHEN prot.i_MasterServiceTypeId = 1 THEN  'OCUPACIONAL' " +
                    //    " ELSE 'SEGUROS' END  AS 'ATENCION' " +
                    //    " from service s" +
                    //    " join servicecomponent sc on s.v_ServiceId = sc.v_ServiceId " +
                    //    " join component c on sc.v_ComponentId = c.v_ComponentId " +
                    //    " join person p on s.v_PersonId = p.v_PersonId " +
                    //    " join protocol prot on s.v_ProtocolId = prot.v_ProtocolId " +
                    //    " join calendar cal on s.v_ServiceId = cal.v_ServiceId " +
                    //    " where (sc.v_ComponentId like 'N009-ME000001466' or sc.v_ComponentId like 'N009-ME000001467') AND sc.i_IsRequiredId = 1  AND cal.i_CalendarStatusId <> 4 " +
                    //    " and (p.v_DocNumber like '%" + value + "%' or p.v_FirstName like '%" + value + "%' or p.v_FirstLastName like '%" + value + "%' or p.v_SecondLastName like '%" + value + "%')" +
                    //    " and (s.d_ServiceDate >= '" + inicio.ToShortDateString() + "' and s.d_ServiceDate <= '" + newFin.ToShortDateString() + "') order by s.v_ServiceId";

                    var query =
                        " select DISTINCT s.v_ServiceId as 'SERVICIO', " +
                        " p.v_FirstLastName + ' ' + p.v_SecondLastName + ', ' +  p.v_FirstName as 'PACIENTE', " +
                        " c.v_Name as 'EXAMEN', " +
                        " sc.r_Price as 'PRECIO', " +
                        " s.d_ServiceDate as 'FECHA', " +
                        " s.v_ComprobantePago as 'COMPROBANTE', " +
                        " prot.v_Name as 'PROTOCOLO', " +
                        " CASE WHEN sp.v_Value1 IS NULL THEN 'SIN RESULTADO' ELSE sp.v_Value1 END AS 'RESULTADO', " +
                        " CASE WHEN prot.i_MasterServiceTypeId = 9 THEN 'PARTICULAR' " +
                        " WHEN prot.i_MasterServiceTypeId = 1 THEN  'OCUPACIONAL' " +
                        " ELSE 'SEGUROS' END  AS 'ATENCION' " +
                        " from service s" +

                        " left join servicecomponent scp  on scp.i_IsRequiredId = 1 and s.v_ServiceId = scp.v_ServiceId  " +
                        " left join servicecomponentfields scf on scp.v_ServiceComponentId = scf.v_ServiceComponentId " +
                        " left join servicecomponentfieldvalues scfv on scf.v_ServiceComponentFieldsId = scfv.v_ServiceComponentFieldsId " +
                        " left join systemparameter sp on sp.i_GroupId = 388 and scfv.v_Value1 = sp.i_ParameterId " +

                        " join servicecomponent sc on s.v_ServiceId = sc.v_ServiceId " +
                        " join component c on sc.v_ComponentId = c.v_ComponentId " +
                        " join person p on s.v_PersonId = p.v_PersonId  " +
                        " join protocol prot on s.v_ProtocolId = prot.v_ProtocolId  " +
                        " join calendar cal on s.v_ServiceId = cal.v_ServiceId " +
                        " where (sc.v_ComponentId like 'N009-ME000001466' or sc.v_ComponentId like 'N009-ME000001467') AND sc.i_IsRequiredId = 1  AND cal.i_CalendarStatusId <> 4 " +
                        " and (p.v_DocNumber like '%" + value + "%' or p.v_FirstName like '%" + value + "%' or p.v_FirstLastName like '%" + value + "%' or p.v_SecondLastName like '%" + value + "%')" +
                        " and (s.d_ServiceDate >= '" + inicio.ToShortDateString() + "' and s.d_ServiceDate <= '" + newFin.ToShortDateString() + "') and (scf.v_ComponentFieldId = 'N009-MF000006038' or scf.v_ComponentFieldId = 'N009-MF000006061') order by s.v_ServiceId";
                    
                    var List = cnx.Query<ListadoCovid>(query).ToList();

                    return List;

                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<ListadoCovid> GetServicesByCovidAntigeno(string value, DateTime inicio, DateTime fin)
        {
            try
            {
                fin = fin.Date;

                DateTime newFin = fin.AddHours(24);
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                 
                    var query =
                        " select DISTINCT s.v_ServiceId as 'SERVICIO', " +
                        " p.v_FirstLastName + ' ' + p.v_SecondLastName + ', ' +  p.v_FirstName as 'PACIENTE', " +
                        " c.v_Name as 'EXAMEN', " +
                        " sc.r_Price as 'PRECIO', " +
                        " s.d_ServiceDate as 'FECHA', " +
                        " s.v_ComprobantePago as 'COMPROBANTE', " +
                        " prot.v_Name as 'PROTOCOLO', " +
                        " CASE WHEN sp.v_Value1 IS NULL THEN 'SIN RESULTADO' ELSE sp.v_Value1 END AS 'RESULTADO', " +
                        " CASE WHEN prot.i_MasterServiceTypeId = 9 THEN 'PARTICULAR' " +
                        " WHEN prot.i_MasterServiceTypeId = 1 THEN  'OCUPACIONAL' " +
                        " ELSE 'SEGUROS' END  AS 'ATENCION' " +
                        " from service s" +

                        " left join servicecomponent scp  on scp.i_IsRequiredId = 1 and s.v_ServiceId = scp.v_ServiceId  " +
                        " left join servicecomponentfields scf on scp.v_ServiceComponentId = scf.v_ServiceComponentId " +
                        " left join servicecomponentfieldvalues scfv on scf.v_ServiceComponentFieldsId = scfv.v_ServiceComponentFieldsId " +
                        " left join systemparameter sp on sp.i_GroupId = 388 and scfv.v_Value1 = sp.i_ParameterId " +

                        " join servicecomponent sc on s.v_ServiceId = sc.v_ServiceId " +
                        " join component c on sc.v_ComponentId = c.v_ComponentId " +
                        " join person p on s.v_PersonId = p.v_PersonId  " +
                        " join protocol prot on s.v_ProtocolId = prot.v_ProtocolId  " +
                        " join calendar cal on s.v_ServiceId = cal.v_ServiceId " +
                        " where (sc.v_ComponentId like 'N003-ME000000054' or sc.v_ComponentId like 'N003-ME000000057') AND sc.i_IsRequiredId = 1  AND cal.i_CalendarStatusId <> 4 " +
                        " and (p.v_DocNumber like '%" + value + "%' or p.v_FirstName like '%" + value + "%' or p.v_FirstLastName like '%" + value + "%' or p.v_SecondLastName like '%" + value + "%')" +
                        " and (s.d_ServiceDate >= '" + inicio.ToShortDateString() + "' and s.d_ServiceDate <= '" + newFin.ToShortDateString() + "') and (scf.v_ComponentFieldId = 'N009-MF000006436' or scf.v_ComponentFieldId = 'N003-MF000000488') order by s.v_ServiceId";

                    var List = cnx.Query<ListadoCovid>(query).ToList();

                    return List;

                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<ListadoCovid> GetServicesByCovidMolecular(string value, DateTime inicio, DateTime fin)
        {
            try
            {
                fin = fin.Date;

                DateTime newFin = fin.AddHours(24);
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {

                    var query =
                        " select DISTINCT s.v_ServiceId as 'SERVICIO', " +
                        " p.v_FirstLastName + ' ' + p.v_SecondLastName + ', ' +  p.v_FirstName as 'PACIENTE', " +
                        " c.v_Name as 'EXAMEN', " +
                        " sc.r_Price as 'PRECIO', " +
                        " s.d_ServiceDate as 'FECHA', " +
                        " s.v_ComprobantePago as 'COMPROBANTE', " +
                        " prot.v_Name as 'PROTOCOLO', " +
                        " CASE WHEN sp.v_Value1 IS NULL THEN 'SIN RESULTADO' ELSE sp.v_Value1 END AS 'RESULTADO', " +
                        " CASE WHEN prot.i_MasterServiceTypeId = 9 THEN 'PARTICULAR' " +
                        " WHEN prot.i_MasterServiceTypeId = 1 THEN  'OCUPACIONAL' " +
                        " ELSE 'SEGUROS' END  AS 'ATENCION' " +
                        " from service s" +

                        " left join servicecomponent scp  on scp.i_IsRequiredId = 1 and s.v_ServiceId = scp.v_ServiceId  " +
                        " left join servicecomponentfields scf on scp.v_ServiceComponentId = scf.v_ServiceComponentId " +
                        " left join servicecomponentfieldvalues scfv on scf.v_ServiceComponentFieldsId = scfv.v_ServiceComponentFieldsId " +
                        " left join systemparameter sp on sp.i_GroupId = 388 and scfv.v_Value1 = sp.i_ParameterId " +

                        " join servicecomponent sc on s.v_ServiceId = sc.v_ServiceId " +
                        " join component c on sc.v_ComponentId = c.v_ComponentId " +
                        " join person p on s.v_PersonId = p.v_PersonId  " +
                        " join protocol prot on s.v_ProtocolId = prot.v_ProtocolId  " +
                        " join calendar cal on s.v_ServiceId = cal.v_ServiceId " +
                        " where (sc.v_ComponentId like 'N003-ME000000049' or sc.v_ComponentId like 'N003-ME000000052') AND sc.i_IsRequiredId = 1  AND cal.i_CalendarStatusId <> 4 " +
                        " and (p.v_DocNumber like '%" + value + "%' or p.v_FirstName like '%" + value + "%' or p.v_FirstLastName like '%" + value + "%' or p.v_SecondLastName like '%" + value + "%')" +
                        " and (s.d_ServiceDate >= '" + inicio.ToShortDateString() + "' and s.d_ServiceDate <= '" + newFin.ToShortDateString() + "') and (scf.v_ComponentFieldId = 'N003-MF000000019' or scf.v_ComponentFieldId = 'N003-MF000000149') order by s.v_ServiceId";

                    var List = cnx.Query<ListadoCovid>(query).ToList();

                    return List;

                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<ServiceCustom> GetServicesByDataPerson(string value, DateTime inicio, DateTime fin)
        {
            try
            {
                fin = fin.Date;

                DateTime newFin = fin.AddHours(24);
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query =
                        "select per.v_FirstName + ' ' + per.v_FirstLastName + ' ' + per.v_SecondLastName AS v_Paciente, ser.v_ServiceId, org.v_Name AS v_Empresa, ser.d_ServiceDate AS d_FechaAtencion, ser.v_ProtocolId, pro.v_Name as Prococolo, pro.r_PriceFactor as Factor, CONVERT (varchar,cast(pro.r_MedicineDiscount as int)) + '%'  as Descuento_PPS, per.v_DocNumber as Documento, 'S/.   ' + CONVERT (varchar,cast(_plan.d_Importe as money)) as Deducible, CONVERT (varchar,cast(_plan.d_ImporteCo as int)) + '%' AS Coaseguro" +
                        " from service ser" +
                        " inner join calendar c on ser.v_ServiceId = c.v_ServiceId " +
                        " inner join person per on ser.v_PersonId = per.v_PersonId " +
                        " inner join [dbo].[plan] _plan on ser.i_PlanId = _plan.i_PlanId " +
                        " inner join protocol pro on ser.v_ProtocolId = pro.v_ProtocolId" +
                        " inner join organization org on pro.v_EmployerOrganizationId = org.v_OrganizationId" +
                        " where (per.v_DocNumber like '%" + value + "%' or per.v_FirstName like '%" + value + "%' or per.v_FirstLastName like '%" + value + "%' or per.v_SecondLastName like '%" + value + "%')" +
                        " and (ser.d_ServiceDate >= '" + inicio.ToShortDateString() + "' and ser.d_ServiceDate <= '" + newFin.ToShortDateString() + "') and  c.i_CalendarStatusId != 4 order by ser.v_ServiceId";

                    var List = cnx.Query<ServiceCustom>(query).OrderBy(x => x.d_FechaAtencion).ToList();

                    return List;

                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<ServiceComponentList> GetServiceComponents(string serviceId)
        {
            try
            {
                List<ServiceComponentList> list = new List<ServiceComponentList>();
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select com.v_Name as v_ComponentName, src.r_Price, src.v_ServiceComponentId, src.v_ServiceId from servicecomponent src " +
                                "join component com on src.v_ComponentId = com.v_ComponentId" +
                                " where src.v_ServiceId = '" + serviceId + "' and src.i_IsRequiredId = 1 and src.i_IsDeleted = 0 and src.r_Price > 0 and (src.i_IsFact != 1 or src.i_IsFact is null) " +
                                "GROUP BY com.v_Name, src.r_Price, src.v_ServiceComponentId, src.v_ServiceId ";
                    return cnx.Query<ServiceComponentList>(query).ToList();

                }


            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<ServiceCustom> GetServicesByDataPerson_Particular(string value, DateTime inicio, DateTime fin)
        {
            try
            {
                fin = fin.Date;

                DateTime newFin = fin.AddHours(24);
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query =
                        "select per.v_FirstName + ' ' + per.v_FirstLastName + ' ' + per.v_SecondLastName AS v_Paciente, ser.v_ServiceId, org.v_Name AS v_Empresa, ser.d_ServiceDate AS d_FechaAtencion, ser.v_ProtocolId, pro.v_Name as Prococolo, pro.r_PriceFactor as Factor, CONVERT (varchar,cast(pro.r_MedicineDiscount as int)) + '%'  as Descuento_PPS, per.v_DocNumber as Documento, 'S/.   ' + CONVERT (varchar,cast(0.0 as money)) as Deducible, CONVERT (varchar,cast(0.0 as int)) + '%' AS Coaseguro" +
                        " from service ser" +
                        " inner join person per on ser.v_PersonId = per.v_PersonId " +
                        " inner join protocol pro on ser.v_ProtocolId = pro.v_ProtocolId" +
                        " inner join organization org on pro.v_EmployerOrganizationId = org.v_OrganizationId" +
                        " where (per.v_DocNumber like '%" + value + "%' or per.v_FirstName like '%" + value + "%' or per.v_FirstLastName like '%" + value + "%' or per.v_SecondLastName like '%" + value + "%')" +
                        " and (ser.d_ServiceDate >= '" + inicio.ToShortDateString() + "' and ser.d_ServiceDate <= '" + newFin.ToShortDateString() + "') and pro.i_MasterServiceTypeId = 9 order by ser.v_ServiceId";

                    var List = cnx.Query<ServiceCustom>(query).OrderBy(x => x.d_FechaAtencion).ToList();

                    return List;

                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<ServiceCustom> GetServicesByDataPerson_Empresarial(string value, DateTime inicio, DateTime fin)
        {
            try
            {
                fin = fin.Date;

                DateTime newFin = fin.AddHours(24);
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query =
                        "select per.v_FirstName + ' ' + per.v_FirstLastName + ' ' + per.v_SecondLastName AS v_Paciente, ser.v_ServiceId, org.v_Name AS v_Empresa, ser.d_ServiceDate AS d_FechaAtencion, ser.v_ProtocolId, pro.v_Name as Prococolo, pro.r_PriceFactor as Factor, CONVERT (varchar,cast(pro.r_MedicineDiscount as int)) + '%'  as Descuento_PPS, per.v_DocNumber as Documento, 'S/.   ' + CONVERT (varchar,cast(0.0 as money)) as Deducible, CONVERT (varchar,cast(0.0 as int)) + '%' AS Coaseguro" +
                        " from service ser" +
                        " inner join person per on ser.v_PersonId = per.v_PersonId " +
                        " inner join protocol pro on ser.v_ProtocolId = pro.v_ProtocolId" +
                        " inner join organization org on pro.v_EmployerOrganizationId = org.v_OrganizationId" +
                        " where (per.v_DocNumber like '%" + value + "%' or per.v_FirstName like '%" + value + "%' or per.v_FirstLastName like '%" + value + "%' or per.v_SecondLastName like '%" + value + "%')" +
                        " and (ser.d_ServiceDate >= '" + inicio.ToShortDateString() + "' and ser.d_ServiceDate <= '" + newFin.ToShortDateString() + "') and pro.i_MasterServiceTypeId = 1 order by ser.v_ServiceId";

                    var List = cnx.Query<ServiceCustom>(query).OrderBy(x => x.d_FechaAtencion).ToList();

                    return List;

                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public KeyValueDTO ObtenerFirmaMedicoExamen(string pstrServiceId, string p1, string p2)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = "SELECT prof.b_SignatureImage AS Value5_, per.v_FirstLastName + ' ' + per.v_SecondLastName + ' ' + per.v_FirstName AS Value2, prof.v_ProfessionalCode AS Value3 FROM servicecomponent src " +
                            "LEFT JOIN systemuser sys on src.i_ApprovedUpdateUserId = sys.i_SystemUserId " +
                            "LEFT JOIN professional prof on sys.v_PersonId = prof.v_PersonId " +
                            "INNER JOIN person per on sys.v_PersonId = per.v_PersonId " +
                            "WHERE src.v_ServiceId = '" + pstrServiceId + "' and (src.v_ComponentId = '" + p1 + "' or src.v_ComponentId ='" + p2 + "')";
                
                var objEntity = cnx.Query<KeyValueDTO>(query).FirstOrDefault();

                return objEntity;
            }
        }

        public List<DiagnosticRepositoryList> GetServiceComponentDisgnosticsByServiceId(ref OperationResult pobjOperationResult, string pstrServiceId)
        {

            try
            {

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "SELECT dry.v_DiagnosticRepositoryId, dry.v_ServiceId, dry.v_ComponentId, dry.v_DiseasesId, dry.i_AutoManualId, dry.i_PreQualificationId, dry.i_FinalQualificationId, dry.i_DiagnosticTypeId, " +
                                "dry.i_IsSentToAntecedent, dry.d_ExpirationDateDiagnostic, dry.i_GenerateMedicalBreak, dis.v_Name AS v_DiseasesName, sys.v_Value1 AS v_AutoManualName, com.i_CategoryId, sys2.v_Value1, " +
                                "sys3.v_Value1 AS v_PreQualificationName, sys4.v_Value1 AS v_FinalQualificationName, sys5.v_Value1 AS v_DiagnosticTypeName, sys6.v_Value1 AS v_IsSentToAntecedentName, " +
                                "sysu.v_UserName AS v_UpdateUser, sysu.d_UpdateDate, dry.i_IsDeleted " +
                                "FROM diagnosticrepository dry " +
                                "LEFT JOIN component com on dry.v_ComponentId = com.v_ComponentId " +
                                "LEFT JOIN servicecomponent src on dry.v_ComponentId = src.v_ComponentId and src.v_ServiceId = '" + pstrServiceId + "' " +
                                "INNER JOIN diseases dis on dry.v_DiseasesId = dis.v_DiseasesId " +
                                "LEFT JOIN systemparameter sys on dry.i_AutoManualId = sys.i_ParameterId and sys.i_GroupId = 136 " +
                                "LEFT JOIN systemparameter sys2 on com.i_CategoryId = sys2.i_ParameterId and sys2.i_GroupId = 116 " +
                                "LEFT JOIN systemparameter sys3 on dry.i_PreQualificationId = sys3.i_ParameterId and sys3.i_GroupId = 137 " +
                                "LEFT JOIN systemparameter sys4 on dry.i_FinalQualificationId = sys4.i_ParameterId and sys4.i_GroupId = 138 " +
                                "LEFT JOIN systemparameter sys5 on dry.i_DiagnosticTypeId = sys5.i_ParameterId and sys5.i_GroupId = 139 " +
                                "LEFT JOIN systemparameter sys6 on dry.i_IsSentToAntecedent = sys6.i_ParameterId and sys6.i_GroupId = 111 " +
                                "LEFT JOIN systemuser sysu on src.i_ApprovedUpdateUserId = sysu.i_SystemUserId " +
                                "WHERE dry.v_ServiceId = '" + pstrServiceId + "' and com.i_IsDeleted = 0 and dry.i_IsDeleted = 0 and dis.i_IsDeleted = 0 ORDER BY com.v_Name";

                    var List = cnx.Query<DiagnosticRepositoryList>(query).ToList();

                    foreach (var item in List)
                    {
                        item.i_RecordStatus = (int) RecordStatus.Grabado;
                        item.i_RecordType = (int)RecordType.NoTemporal;
                        item.v_ComponentId = item.i_CategoryId == -1 ? item.v_Name : item.v_Value1;
                    }

                    List = List.GroupBy(p => p.v_DiagnosticRepositoryId).Select(p => p.FirstOrDefault()).ToList();
                    if (List.Count > 0)
                    {
                        var q = (from a in List
                            select new DiagnosticRepositoryList
                            {
                                v_DiagnosticRepositoryId = a.v_DiagnosticRepositoryId,
                                v_ServiceId = a.v_ServiceId,
                                v_ComponentId = a.v_ComponentId,
                                v_DiseasesId = a.v_DiseasesId,
                                i_AutoManualId = a.i_AutoManualId,
                                i_PreQualificationId = a.i_PreQualificationId,
                                i_FinalQualificationId = a.i_FinalQualificationId,
                                i_DiagnosticTypeId = a.i_DiagnosticTypeId,
                                i_IsSentToAntecedent = a.i_IsSentToAntecedent,
                                d_ExpirationDateDiagnostic = a.d_ExpirationDateDiagnostic,
                                i_GenerateMedicalBreak = a.i_GenerateMedicalBreak,

                                v_RestrictionsName = ConcatenateRestriction(a.v_DiagnosticRepositoryId),
                                v_RecomendationsName = ConcatenateRecommendation(a.v_DiagnosticRepositoryId),
                                v_DiseasesName = a.v_DiseasesName,
                                v_AutoManualName = a.v_AutoManualName,
                                v_ComponentName = a.v_ComponentName,

                                v_PreQualificationName = a.v_PreQualificationName,
                                v_FinalQualificationName = a.v_FinalQualificationName,
                                v_DiagnosticTypeName = a.v_DiagnosticTypeName,
                                v_IsSentToAntecedentName = a.v_IsSentToAntecedentName,
                                i_RecordStatus = a.i_RecordStatus,
                                i_RecordType = a.i_RecordType,

                                v_UpdateUser = a.v_UpdateUser,

                                d_UpdateDate = a.d_UpdateDate,
                                i_IsDeleted = a.i_IsDeleted

                            }).ToList();
                        pobjOperationResult.Success = 1;
                        return q;
                    }
                   

                    pobjOperationResult.Success = 1;
                    return new List<DiagnosticRepositoryList>();
                }                
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        private string ConcatenateRestriction(string pstrDiagnosticRepositoryId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = "SELECT masrr.v_Name FROM restriction res " +
                            "INNER JOIN masterrecommendationrestricction masrr  on res.v_MasterRestrictionId = masrr.v_MasterRecommendationRestricctionId " +
                            "WHERE res.v_DiagnosticRepositoryId = '" + pstrDiagnosticRepositoryId + "' and res.i_IsDeleted = 0 and masrr.i_TypifyingId = " + (int)Typifying.Restricciones + "";
                var List = cnx.Query<DiagnosticRepositoryList>(query).ToList();

                return string.Join(", ", List.Select(p => p.v_Name));
            }
        }

        private string ConcatenateRecommendation(string pstrDiagnosticRepositoryId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = "SELECT masrr.v_Name FROM recommendation recm " +
                            "INNER JOIN masterrecommendationrestricction masrr  on recm.v_MasterRecommendationId = masrr.v_MasterRecommendationRestricctionId " +
                            "WHERE recm.v_DiagnosticRepositoryId = '" + pstrDiagnosticRepositoryId + "' and recm.i_IsDeleted = 0 and masrr.i_TypifyingId = " + (int)Typifying.Recomendaciones + "";
                var List = cnx.Query<DiagnosticRepositoryList>(query).ToList();

                return string.Join(", ", List.Select(p => p.v_Name));
            }

        }

        public string AddGenericDiseasesByServiceId(string serviceId, List<string> ClientSession)
        {
            try
            {
                var secuential = Utilidades.GetNextSecuentialId(int.Parse(ClientSession[0]), 29);
                var newId = Utilidades.GetNewId(int.Parse(ClientSession[0]), secuential, "DR");
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var add = "INSERT INTO diagnosticrepository(v_DiagnosticRepositoryId, v_ServiceId, v_DiseasesId, v_ComponentId, i_AutoManualId, i_PreQualificationId, i_FinalQualificationId, i_DiagnosticTypeId, i_IsDeleted, i_InsertUserId, d_InsertDate) " +
                                "VALUES ('" + newId + "', '" + serviceId + "', '" + Constants.ATENCION_MEDICA_NO_ESPECIFICADA + "', 'N009-ME000001140', 1, 1, 4, 1, 0, " + int.Parse(ClientSession[2]).ToString() + ", '" + DateTime.Now.ToShortDateString() + "')";

                    cnx.Execute(add);
                }

                return newId;
            }//cambia el config apunta a prueba
            catch (Exception ex)
            {
                throw;
            }
        }
        public void AddRecipeNEwsByServiceId(string recetaId, string serviceId, decimal total, int isdelete, List<string> ClientSession, string v_diaxrepositoryId)
        {
            try
            {
                

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var add = "INSERT INTO receipHeader(v_ReceipId, v_ServiceId, i_MedicoId, v_MedicoName, d_Total, i_IsDeleted, i_InsertUserId, d_InsertDate, i_UpdateUserId, d_UpdateDate, v_diaxrepositoryId) " +
                                "VALUES ('" + recetaId + "', '" + serviceId + "', 11 , 'SA' ," + total + ", " + isdelete + ", " + int.Parse(ClientSession[2]).ToString() + ", '" + DateTime.Now.ToShortDateString() + "', NULL, NULL, '" + v_diaxrepositoryId + "')";
                    cnx.Execute(add);
                }
            }//cambia el config apunta a prueba
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
