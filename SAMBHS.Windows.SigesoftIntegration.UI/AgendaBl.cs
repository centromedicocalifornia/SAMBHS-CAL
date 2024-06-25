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
using System.Transactions;
using SAMBHS.Windows.SigesoftIntegration.UI.Reports;
using SAMBHS.Common.BE.Custom;
using SAMBHS.Common.DataModel;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public class AgendaBl
    {
        //SqlConnection conexx = new SqlConnection(ConnectionHelper.GetNewSigesoftConnection);

        public class DataReniec
        {
            public bool Success { get; set; }
            public string Dni { get; set; }
            public string Nombres { get; set; }
            public string ApellidoPaterno { get; set; }
            public string ApellidoMaterno { get; set; }
            public int CodVerifica { get; set; }
        }

        public List<EsoDto> LlenarPacientesNew()
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = @"select ISNULL(v_DocNumber, '')  as 'Id' , v_FirstLastName + ' ' + v_SecondLastName + ', ' + v_FirstName as 'Nombre' from person where i_IsDeleted = 0 order by v_DocNumber";

                var listaPac = cnx.Query<EsoDto>(query).ToList();

                return listaPac;

            }

        }

        public static void LlenarComboVendedorExterno(ComboBox cbo)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
								where i_GroupId = 97 and i_IsDeleted = 0 and v_Value1 != '' ";

                    var data = cnx.Query<EsoDto>(query).ToList();
                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Nombre";
                    cbo.ValueMember = "EsoId";
                    cbo.SelectedIndex = 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static void LlenarComboEspecialidad(ComboBox cbo)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
								where i_GroupId = 403 and i_IsDeleted = 0 and v_Value1 != '' ";

                    var data = cnx.Query<EsoDto>(query).ToList();
                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Nombre";
                    cbo.ValueMember = "EsoId";
                    cbo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static void LlenarComboEstablecimiento(ComboBox cbo)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
								where i_GroupId = 99 and i_IsDeleted = 0 and v_Value1 != '' ";

                    var data = cnx.Query<EsoDto>(query).ToList();
                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Nombre";
                    cbo.ValueMember = "EsoId";
                    cbo.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void LlenarComboMedicoSolicitanteExterno(ComboBox cbo)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
								where i_GroupId = 96 and i_IsDeleted = 0 and v_Value1 != '' ";

                    var data = cnx.Query<EsoDto>(query).ToList();
                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Nombre";
                    cbo.ValueMember = "EsoId";
                    cbo.SelectedIndex = 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int CreateItemEstablecimiento(string name)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                string ultimoQuery = "select i_ParameterId from systemparameter where i_GroupId = 99 order by i_ParameterId desc";
                List<int> list = cnx.Query<int>(ultimoQuery).ToList();
                int ultimo = list.Count() == 0 ? 0 : list[0] + 1;
                var query = "INSERT INTO systemparameter values(99, " + ultimo.ToString() + ", '" + name +
                            "', null, null,-1,null,0,12,'" + DateTime.Now.ToString() + "', null,null,null)";

                cnx.Execute(query);

                return ultimo;
            }
        }
        public static int CreateItemVendedorExterno(string name)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                string ultimoQuery = "select i_ParameterId from systemparameter where i_GroupId = 97 order by i_ParameterId desc";
                List<int> list = cnx.Query<int>(ultimoQuery).ToList();
                int ultimo = list.Count() == 0 ? 0 : list[0] + 1;
                var query = "INSERT INTO systemparameter values(97, " + ultimo.ToString() + ", '" + name +
                            "', null, null,-1,null,0,12,'" + DateTime.Now.ToString() + "', null,null,null)";

                cnx.Execute(query);

                return ultimo;
            }
        }

        public static int CreateItemMedicoExterno(string name)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                string ultimoQuery = "select i_ParameterId from systemparameter where i_GroupId = 96 order by i_ParameterId desc";
                List<int> list = cnx.Query<int>(ultimoQuery).ToList();
                int ultimo = list.Count() == 0 ? 0 : list[0] + 1;
                var query = "INSERT INTO systemparameter values(96, " + ultimo.ToString() + ", '" + name +
                            "', null, null,-1,null,0,12,'" + DateTime.Now.ToString() + "', null,null,null)";

                cnx.Execute(query);

                return ultimo;
            }
        }
        public static DatosTrabajador GetDatosTrabajador(string pstNroDocument)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                DatosTrabajador query = (from a in dbContext.getdatostrabajador_sp(pstNroDocument)
                                               select new DatosTrabajador
                                             {
                                                 PersonId = a.PersonId,
                                                 Nombres = a.Nombres,
                                                 ApellidoPaterno = a.ApellidoPaterno,
                                                 ApellidoMaterno = a.ApellidoMaterno,
                                                 TipoDocumentoId = a.TipoDocumentoId,
                                                 NroDocumento = a.NroDocumento,
                                                 GeneroId = a.GeneroId,
                                                 FechaNacimiento  = a.FechaNacimiento,
                                                 EstadoCivil  = a.EstadoCivil,
                                                 LugarNacimiento  = a.LugarNacimiento,
                                                 Distrito  = a.Distrito,
                                                 Provincia  = a.Provincia,
                                                 Departamento  = a.Departamento,
                                                 Reside = a.Reside,
                                                 Email  = a.Email,
                                                 Direccion = a.Direccion,
                                                 Puesto = a.Puesto,
                                                 Altitud  = a.Altitud,

                                                 Minerales = a.Minerales,
                                                 Estudios = a.Estudios,
                                                 Grupo = a.Grupo,
                                                 Factor = a.Factor,
                                                 TiempoResidencia  = a.TiempoResidencia,
                                                 TipoSeguro = a.TipoSeguro,
                                                 Vivos = a.Vivos,
                                                 Muertos = a.Muertos,
                                                 Hermanos = a.Hermanos,
                                                 Telefono = a.Telefono,
                                                 Parantesco = a.Parantesco,
                                                 Labor = a.Labor,
                                                 ResidenciaAnterior = a.ResidenciaAnterior,
                                                 Nacionalidad = a.Nacionalidad,
                                                 Religion = a.Religion,
                                                 titular = a.titular,
                                                 ContactoEmergencia = a.ContactoEmergencia,
                                                 CelularEmergencia = a.CelularEmergencia,

                                                 b_PersonImage = a.b_PersonImage,
                                                 b_FingerPrintTemplate = a.b_FingerPrintTemplate,
                                                 b_FingerPrintImage = a.b_FingerPrintImage,
                                                 b_RubricImage = a.b_RubricImage,
                                                 t_RubricImageText = a.t_RubricImageText,
                                                 i_EtniaRaza = a.i_EtniaRaza.Value,
                                                 i_Migrante = a.i_Migrante.Value,
                                                 PaisOrigen = a.v_Migrante

                                             }).FirstOrDefault();
                return query;

                //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                //{
                //    //v_PersonId,v_FirstName,v_FirstLastName,v_SecondLastName,i_DocTypeId,v_DocNumber,i_SexTypeId,d_Birthdate,i_IsDeleted,i_MaritalStatusId,v_BirthPlace,i_DistrictId,i_ProvinceId,i_DepartmentId,i_ResidenceInWorkplaceId,v_Mail,v_AdressLocation,v_CurrentOccupation,i_AltitudeWorkId,v_ExploitedMineral,i_LevelOfId,i_BloodGroupId,i_BloodFactorId,v_ResidenceTimeInWorkplace,i_TypeOfInsuranceId,i_NumberLivingChildren,i_NumberDependentChildren,i_NroHermanos,v_TelephoneNumber,i_Relationship,i_PlaceWorkId
                //    //var query =
                //    //    "select v_PersonId as PersonId, v_FirstName as Nombres, v_FirstLastName as ApellidoPaterno, v_SecondLastName as ApellidoMaterno, i_DocTypeId as TipoDocumentoId, v_DocNumber as  NroDocumento, i_SexTypeId as GeneroId, d_Birthdate as FechaNacimiento ,i_MaritalStatusId as EstadoCivil,v_BirthPlace as LugarNacimiento,i_DistrictId as Distrito,i_ProvinceId as Provincia,i_DepartmentId as Departamento,i_ResidenceInWorkplaceId as Reside,v_Mail as Email,v_AdressLocation as Direccion,v_CurrentOccupation as Puesto,i_AltitudeWorkId as Altitud,v_ExploitedMineral as Minerales,i_LevelOfId as Estudios,i_BloodGroupId as Grupo,i_BloodFactorId as Factor,v_ResidenceTimeInWorkplace as TiempoResidencia,i_TypeOfInsuranceId as TipoSeguro,i_NumberLivingChildren as Vivos,i_NumberDependentChildren as Muertos,i_NroHermanos as Hermanos,v_TelephoneNumber as Telefono,i_Relationship as Parantesco ,i_PlaceWorkId as Labor,b_FingerPrintTemplate,b_FingerPrintImage,b_RubricImage,t_RubricImageText,b_PersonImage,v_Religion as Religion ,v_Nacionalidad as Nacionalidad,v_ResidenciaAnterior as ResidenciaAnterior, v_OwnerName as titular , v_ContactName as ContactoEmergencia, v_EmergencyPhone as CelularEmergencia " +
                //    //    "from person where v_DocNumber = '" + pstNroDocument + "'";

                //    var query = "EXEC [GetDatosTrabajador_SP] '" + pstNroDocument + "'";

                //    return cnx.Query<DatosTrabajador>(query).FirstOrDefault();
                //}
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string UpdateComprobante_Estado(int facturado, string ruta, string comprobanteId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query2 = " update [20505310072].dbo.venta set i_EstadoSunat = " + facturado + ", v_EnlaceEnvio = " + ruta + " where v_IdVenta = '" + comprobanteId + "'";
                cnx.Execute(query2);

                return comprobanteId;

            }
        }
        public static ServiceDto GetDatosServicio(string servicioId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    //v_PersonId,v_FirstName,v_FirstLastName,v_SecondLastName,i_DocTypeId,v_DocNumber,i_SexTypeId,d_Birthdate,i_IsDeleted,i_MaritalStatusId,v_BirthPlace,i_DistrictId,i_ProvinceId,i_DepartmentId,i_ResidenceInWorkplaceId,v_Mail,v_AdressLocation,v_CurrentOccupation,i_AltitudeWorkId,v_ExploitedMineral,i_LevelOfId,i_BloodGroupId,i_BloodFactorId,v_ResidenceTimeInWorkplace,i_TypeOfInsuranceId,i_NumberLivingChildren,i_NumberDependentChildren,i_NroHermanos,v_TelephoneNumber,i_Relationship,i_PlaceWorkId
                    var query =
                        "select v_ServiceId as ServiceId, v_ProtocolId as ProtocolId, v_PersonId as PersonId, v_OrganizationId as OrganizationId, v_Area as Area, v_centrocosto as CCosto, v_LicenciaConducir as v_LicenciaConducir, i_ModTrabajo as i_ModTrabajo, i_ProcedenciaPac_Mkt as i_ProcedenciaPac_Mkt, ISNULL(i_Establecimiento,-1) as Establecimiento, ISNULL(i_VendedorExterno,-1) as VendedorExterno , ISNULL(i_MedicoSolicitanteExterno,-1) as MedicoSolicitanteExterno  " +
                        "from service where v_ServiceId = '" + servicioId + "'";
                    return cnx.Query<ServiceDto>(query).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

       
        public List<EsoDto> LlenarComboTipoDocumento(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcombotipodocumento()
                                      select new EsoDto
                                      {
                                          EsoId = a.EsoId,
                                          Nombre = a.Nombre
                                      }).ToList();

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return query;

                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 0;


//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

//                    var query = @"select i_ItemId as 'EsoId', v_Value1 as 'Nombre' from datahierarchy
//                    								where i_GroupId = 106 and i_IsDeleted = 0";
//                    var query = "EXEC [LlenarComboTipoDocumento]";

//                    var data = cnx.Query<EsoDto>(query).ToList();
//                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

//                    cbo.DataSource = data;
//                    cbo.DisplayMember = "Nombre";
//                    cbo.ValueMember = "EsoId";
//                    cbo.SelectedIndex = 0;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboMarcaVacunaCovid(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                //List<EsoDto> query = (from a in dbContext.llenarcombotipodocumento()
                //    select new EsoDto
                //    {
                //        EsoId = a.EsoId,
                //        Nombre = a.Nombre
                //    }).ToList();

                //query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                //return query;

                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 0;


                                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                                {
                //                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                //                    var query = @"select i_ItemId as 'EsoId', v_Value1 as 'Nombre' from datahierarchy
                //                    								where i_GroupId = 106 and i_IsDeleted = 0";
                var query = "EXEC [LlenarComboMarcaVacunaCovid]";

                var data = cnx.Query<EsoDto>(query).ToList();
                data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return data;
                //                    cbo.DataSource = data;
                //                    cbo.DisplayMember = "Nombre";
                //                    cbo.ValueMember = "EsoId";
                //                    cbo.SelectedIndex = 0;
                                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class PuestoList
        {
            public string PuestoId { get; set; }
            public string Puesto { get; set; }
        }
        public class NacionalidadList
        {
            public string NacionalidadId { get; set; }
            public string Nacionalidad { get; set; }
        }

        public class PaisOrigenList
        {
            public string PaisOrigenId { get; set; }
            public string PaisOrigen { get; set; }
        }

        public class AreaList
        {
            public string AreaId { get; set; }
            public string Area { get; set; }
        }

        public class ContactoEmergenciaList
        {
            public string ContactoEmergenciaId { get; set; }
            public string ContactoEmergencia { get; set; }
        }

        public class CCostoList
        {
            public string CostoId { get; set; }
            public string Costo { get; set; }
        }

        public static List<PuestoList> ObtenerPuestos()
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<PuestoList> query = (from a in dbContext.obtenerpuestos_sp()
                                          select new PuestoList
                                           {
                                               PuestoId = a.PuestoId,
                                               Puesto = a.Puesto
                                           }).ToList();
                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<NacionalidadList> ObtenerNacionalidad()
        {
            try
            {
                //SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                //List<PuestoList> query = (from a in dbContext.obtenerpuestos_sp()
                //                          select new PuestoList
                //                          {
                //                              PuestoId = a.PuestoId,
                //                              Puesto = a.Puesto
                //                          }).ToList();
                //return query;

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select distinct v_Nacionalidad as NacionalidadId, v_Nacionalidad as Nacionalidad  from person where i_IsDeleted = 0";

                    //var query = "EXEC [ObtenerContactoEmergencia_SP]";

                    var data = cnx.Query<NacionalidadList>(query).ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static int CreateItemEtniaRaza(string name)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                string ultimoQuery = "select i_ParameterId from systemparameter where i_GroupId = 401 order by i_ParameterId desc";
                List<int> list = cnx.Query<int>(ultimoQuery).ToList();
                int ultimo = list[0] + 1;
                var query = "INSERT INTO systemparameter values(401, " + ultimo.ToString() + ", '" + name +
                            "', null, null,-1,null,0,12,'" + DateTime.Now.ToString() + "', null,null,null)";

                cnx.Execute(query);

                return ultimo;
            }
        }

        public static List<PaisOrigenList> ObtenerPaisOrigen()
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select distinct ISNULL(v_Migrante,'') as PaisOrigenId, ISNULL(v_Migrante,'') as PaisOrigen from person where i_IsDeleted = 0";

                    //var query = "EXEC [ObtenerContactoEmergencia_SP]";

                    var data = cnx.Query<PaisOrigenList>(query).ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static List<ContactoEmergenciaList> ObtenerContactoEmergencia()
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<ContactoEmergenciaList> query = (from a in dbContext.obtenercontactoemergencia_sp()
                                                      select new ContactoEmergenciaList
                                                        {
                                                            ContactoEmergenciaId = a.ContactoEmergenciaId,
                                                            ContactoEmergencia = a.ContactoEmergencia
                                                        }).ToList();
                return query;

                //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                //{
                //    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                //    //var query = @"select distinct v_ContactName as ContactoEmergenciaId, v_ContactName as ContactoEmergencia  from person where i_IsDeleted = 0";

                //    var query = "EXEC [ObtenerContactoEmergencia_SP]";

                //    var data = cnx.Query<ContactoEmergenciaList>(query).ToList();
                //    return data;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<AreaList> ObtenerAreas()
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<AreaList> query = (from a in dbContext.obtenerareas_sp()
                                        select new AreaList
                                          {
                                              AreaId = a.AreaId,
                                              Area = a.Area
                                          }).ToList();
                return query;

                //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                //{
                //    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                //    //var query = @"select distinct v_Area as AreaId, v_Area as Area  from service where i_IsDeleted = 0";

                //    var query = "EXEC [ObtenerAreas_SP]";

                //    var data = cnx.Query<AreaList>(query).ToList();
                //    return data;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static List<CCostoList> ObtenerCC()
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select distinct v_centrocosto as CostoId, v_centrocosto as Costo  from service where i_IsDeleted = 0";

                    var data = cnx.Query<CCostoList>(query).ToList();
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboNivelEstudio(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcombonivelestudio_sp()
                                      select new EsoDto
                                      {
                                          EsoId = a.EsoId,
                                          Nombre = a.Nombre
                                      }).ToList();

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 0;
//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

////                    var query = @"select i_ItemId as 'EsoId', v_Value1 as 'Nombre' from datahierarchy
////								where i_GroupId = 108 and i_IsDeleted = 0";
//                    var query = "EXEC [LlenarComboNivelEstudio_SP]";
//                    var data = cnx.Query<EsoDto>(query).ToList();
//                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

//                    cbo.DataSource = data;
//                    cbo.DisplayMember = "Nombre";
//                    cbo.ValueMember = "EsoId";
//                    cbo.SelectedIndex = 0;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LlenarComboPlan(ComboBox cbo)
        {
            try
            {
                
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

//                    var query = @"select i_ItemId as 'EsoId', v_Value1 as 'Nombre' from datahierarchy
//								where i_GroupId = 108 and i_IsDeleted = 0";

                List<EsoDto> data = new List<EsoDto>();
                data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                    //cbo.Items.Insert(-1, "--Seleccionar--");
                    cbo.DataSource = data;
                    cbo.DisplayMember = "Nombre";
                    cbo.ValueMember = "EsoId";
                    cbo.SelectedIndex = 0;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<EsoDto> LlenarComboDistrito(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcombodistrito_sp()
                                      select new EsoDto
                                      {
                                          EsoId = a.EsoId,
                                          Nombre = a.Nombre
                                      }).ToList();

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 0;

//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

////                    var query = @"select i_ItemId as 'EsoId', v_Value1 as 'Nombre' from datahierarchy
////								where i_GroupId = 113 and i_IsDeleted = 0";                 
                    
//                    var query = "EXEC [LlenarComboDistrito_SP]";
                  
//                    var data = cnx.Query<EsoDto>(query).ToList();
//                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

//                    cbo.DataSource = data;
//                    cbo.DisplayMember = "Nombre";
//                    cbo.ValueMember = "EsoId";
//                    cbo.SelectedIndex = 0;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<KeyValueDTO> BuscarDistritos(string text)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<KeyValueDTO> query = (from a in dbContext.buscardistritos_sp(text)
                                           select new KeyValueDTO
                                      {
                                          Id = a.Id.ToString(),
                                          Value1 = a.Value1,
                                          Value2 = a.Value2,
                                          Value4 = Convert.ToInt32(a.Value4)
                                      }).ToList();
                

                return query;
//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

//                    var query = @"select i_ItemId as 'Id', v_Value1 as 'Value1' , v_Value2 as 'Value2', i_ParentItemId as 'Value4'  
//                                from datahierarchy
//								where i_GroupId = 113 and i_IsDeleted = 0 and v_Value1 = '"  + text + "' order by Value4 desc";


//                    var data = cnx.Query<KeyValueDTO>(query).ToList();

//                    return data;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<KeyValueDTO> ObtenerProvincia(int? pintParentItemId)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<KeyValueDTO> query = (from a in dbContext.obtenerprovincia_sp(pintParentItemId.ToString())
                                           select new KeyValueDTO
                                           {
                                               Id = a.Id.ToString(),
                                               Value1 = a.Value1,
                                               Value2 = a.Value2,
                                               Value4 = Convert.ToInt32(a.Value4)
                                           }).ToList();


                return query;

//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

//                    var query = @"select i_ItemId as 'Id', v_Value1 as 'Value1' , v_Value2 as 'Value2', i_ParentItemId as 'Value4'  
//                                from datahierarchy
//								where i_ItemId = " + pintParentItemId + " and i_IsDeleted = 0 and i_GroupId = 113 order by v_Value1 ";

//                    var data = cnx.Query<KeyValueDTO>(query).ToList();

//                    return data;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public static List<KeyValueDTO> BuscarCoincidenciaDistritos(ref OperationResult pobjOperationResult, int pintGroupId, string text)
        {
            //mon.IsActive = true;

            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {

                    var query = "select * from datahierarchy where i_GroupId = " + pintGroupId +
                                " and i_IsDeleted = 0 and v_Value1 = '" + text + "' Order By i_ParentItemId";

                    var data = cnx.Query<datahierarchyDto>(query).ToList();

                    var query2 = data.AsEnumerable()
                        .Select(x => new KeyValueDTO
                        {
                            Id = x.i_ItemId.ToString(),
                            Value1 = x.v_Value1,
                            Value2 = x.v_Value2,
                            Value4 = x.i_ParentItemId.Value
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

        public static void LlenarComboProfesion(ComboBox cbo)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select i_ItemId as 'EsoId', v_Value1 as 'Nombre' from datahierarchy
								where i_GroupId = 126 and i_IsDeleted = 0 and v_Value1 != '' ";

                    var data = cnx.Query<EsoDto>(query).ToList();
                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Nombre";
                    cbo.ValueMember = "EsoId";
                    cbo.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LlenarComboUsuariosN(ComboBox cbo)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"SELECT b.v_FirstLastName + ' ' + v_SecondLastName + ', ' + b.v_FirstName as Value1, a.i_SystemUserId as Id
                                FROM systemuser a
                                INNER JOIN person b on a.v_PersonId = b.v_PersonId
                                INNER JOIN professional p on b.v_PersonId=p.v_PersonId
								inner join datahierarchy dt on p.i_ProfessionId=dt.i_ItemId and i_GroupId=101 and i_ProfessionId in (30, 31, 32, 34)
                                WHERE a.i_IsDeleted = 0 and b.i_IsDeleted = 0 and p.i_IsDeleted = 0 ";

                    var data = cnx.Query<KeyValueDTO>(query).ToList();
                    data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Value1";
                    cbo.ValueMember = "Id";
                    //cbo.SelectedIndex = -1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }



        #region AMC
        public List<KeyValueDTO> LlenarComboSystemParametro(ComboBox cbo, int id)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<KeyValueDTO> query = (from a in dbContext.llenarcombosystemparametro_sp(id)
                                           select new KeyValueDTO
                                           {
                                               Value1 = a.Value1,
                                               Id = a.Id.ToString()
                                           }).ToList();

                query.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Value1";
                //cbo.ValueMember = "Id";
                //cbo.SelectedIndex = 0;

//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

//                    var query = @"select i_ParameterId as 'Id', v_Value1 as 'Value1' from systemparameter
//								where i_GroupId =" + id + " and i_IsDeleted = 0";

//                    var data = cnx.Query<KeyValueDTO>(query).ToList();
//                    data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

//                    return data;
//                    //cbo.DataSource = data;
//                    //cbo.DisplayMember = "Value1";
//                    //cbo.ValueMember = "Id";
//                    //cbo.SelectedIndex = 0;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static void LlenarComboGetServiceType(ComboBox cbo, int id)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<KeyValueDTO> query = (from a in dbContext.llenarcombogetservicetype_sp()
                                           select new KeyValueDTO
                                      {
                                          Value1 = a.Value1,
                                          Id = a.Id.ToString()
                                      }).ToList();

                query.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                cbo.DataSource = query;
                cbo.DisplayMember = "Value1";
                cbo.ValueMember = "Id";
                cbo.SelectedIndex = 0;

                //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                //{
                //    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                //    var query = @"select Distinct i_ParameterId as 'Id', v_Value1 as 'Value1' from nodeserviceprofile np inner join systemparameter sp on np.i_ServiceTypeId = sp.i_ParameterId and sp.i_GroupId = 119 where np.i_NodeId = 9";

                //    var data = cnx.Query<KeyValueDTO>(query).ToList();
                //    data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                //    cbo.DataSource = data;
                //    cbo.DisplayMember = "Value1";
                //    cbo.ValueMember = "Id";
                //    cbo.SelectedIndex = 0;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LlenarComboDatahierarchy(ComboBox cbo, int id)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select i_ItemId as 'Id', v_Value1 as 'Value1' from datahierarchy
								where i_GroupId =" + id + " and i_IsDeleted = 0";

                    var data = cnx.Query<KeyValueDTO>(query).ToList();
                    data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Value1";
                    cbo.ValueMember = "Id";
                    cbo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LlenarComboServicios(ComboBox cbo, int? pintServiceTypeId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"SELECT i_MasterServiceId as Id, b.v_Value1 as Value1
                                FROM nodeserviceprofile a
                                INNER JOIN systemparameter b on a.i_MasterServiceId = b.i_ParameterId and b.i_GroupId = 119
                                WHERE a.i_IsDeleted = 0 and a.i_NodeId = 9 and a.i_ServiceTypeId =" + pintServiceTypeId;

                    var data = cnx.Query<KeyValueDTO>(query).ToList();
                    data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Value1";
                    cbo.ValueMember = "Id";
                    cbo.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            } 
        }

        public static void LlenarComboPlanes(ComboBox cbo, string protocolo)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select PL.i_PlanId as Id, lin.v_Nombre as Value1 " +
                                " from protocol PR " +
                                " left join [dbo].[plan] PL on PR.v_ProtocolId=PL.v_ProtocoloId " +
                                " left join [20505310072].dbo.linea as lin on PL.v_IdUnidadProductiva = lin.v_IdLinea " +
                                " where v_ProtocolId='" + protocolo + "'";

                    var data = cnx.Query<KeyValueDTO>(query).ToList();
                    data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Value1";
                    cbo.ValueMember = "Id";
                    cbo.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<KeyValueDTO> LlenarComboUsuarios(ComboBox cbo)
        {
            try
            {
                //SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                //List<KeyValueDTO> query = (from a in dbContext.llenarcombousuarios_sp("")
                //                           select new KeyValueDTO
                //                           {
                //                               Value1 = a.Value1,
                //                               Id = a.Id.ToString()
                //                           }).ToList();

                //query.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                //return query;

                //cbo.DataSource = query;
                //cbo.DisplayMember = "Value1";
                //cbo.ValueMember = "Id";
                //cbo.SelectedIndex = 0;


                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"SELECT b.v_FirstName + ' '  + b.v_FirstLastName + ' ' + v_SecondLastName as Value1, a.i_SystemUserId as Id, sp.v_Value1 as 'Value2'
                                FROM systemuser a
                                INNER JOIN person b on a.v_PersonId = b.v_PersonId
								JOIN professional pr on a.v_PersonId = pr.v_PersonId
								join systemparameter sp on sp.i_ParameterId = pr.i_Profesion and sp.i_GroupId = 403
								UNION
								SELECT b.v_FirstName + ' '  + b.v_FirstLastName + ' ' + v_SecondLastName as Value1, a.i_SystemUserId as Id, '- - -' as 'Value2'
                                FROM systemuser a
                                INNER JOIN person b on a.v_PersonId = b.v_PersonId
								
                                WHERE a.i_SystemUserId = 11

								ORDER BY Value2 ";

                    var data = cnx.Query<KeyValueDTO>(query).ToList();
                    data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                    //cbo.DataSource = data;
                    //cbo.DisplayMember = "Value1";
                    //cbo.ValueMember = "Id";
                    //cbo.SelectedIndex = 1;

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Value1";
                    cbo.ValueMember = "Id";
                    cbo.SelectedIndex = 0;


                    return data;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static List<AgendaDto> ObtenerListaAgendados(FiltroAgenda filtros)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                var fi = filtros.FechaInicio.Value.ToShortDateString();
                var ff = filtros.FechaFin.Value.AddDays(1).ToShortDateString();
                var nroDoc = filtros.NroDocumento == "" ? "null" : "'" +filtros.NroDocumento +"'";
                //var tipoServicio = filtros.TipoServicio.ToString() == "-1" ? "null" : filtros.TipoServicio.ToString();
                var servicio = filtros.Servicio.ToString() == "-1" ? "null" : filtros.Servicio.ToString();
                var modalidad = filtros.Modalidad.ToString() == "-1" ? "null" : filtros.Modalidad.ToString();
                var cola = filtros.Cola.ToString() == "-1" ? "null" : filtros.Cola.ToString();
                var vip = filtros.Vip.ToString() == "-1" ? "null" : filtros.Vip.ToString();
                var estadoCita = filtros.EstadoCita.ToString() == "-1" ? "null" : filtros.EstadoCita.ToString();
                var paciente = filtros.Paciente == "" ? "null" : "'%"+ filtros.Paciente +"%'";
                var query = @"SELECT d.v_ServiceId, a.d_DateTimeCalendar," + 
                            "B.v_FirstLastName + ' ' + B.v_SecondLastName + ' ' + B.v_FirstName as v_Pacient,"+
                            "B.v_DocNumber as v_NumberDocument,"+
                            "sp1.v_Value1 as v_LineStatusName,"+
                            "sp8.v_Value1 as v_ServiceStatusName,"+
                            "A.d_SalidaCM,"+
                            "sp9.v_Value1 as v_AptitudeStatusName,"+
                            "sp2.v_Value1 as v_ServiceTypeName,"+
                            "sp3.v_Value1 as v_ServiceName,"+
                            "sp4.v_Value1 as v_NewContinuationName,"+
                            "sp7.v_Value1 as v_EsoTypeName,"+
                            "a.i_ServiceTypeId," +
                            "sp5.v_Value1 as v_CalendarStatusName,"+
                            "e.v_Name as v_ProtocolName,"+
                            "sp6.v_Value1 as v_IsVipName,"+
                            "f.v_Name + ' / '+ g.v_Name as v_OrganizationLocationProtocol,"+
                            "j.v_Name + ' / ' + k.v_Name as v_OrganizationLocationService,"+
                            "A.d_EntryTimeCM,"+
                            "l.v_Name as GESO,"+
                            "b.v_CurrentOccupation as Puesto,"+
                            "B.v_FirstName as Nombres,"+
                            "B.v_FirstLastName as ApePaterno,"+
                            "B.v_SecondLastName as ApeMaterno, "+
                            //"B.b_PersonImage as FotoTrabajador, " +
                            //"B.b_FingerPrintImage as HuellaTrabajador, " +
                            //"B.b_RubricImage as FirmaTrabajador, " +
                            "h.v_Name as v_WorkingOrganizationName, " +
                            "e.v_ProtocolId as v_ProtocolId, " +
                            "a.v_CalendarId as v_CalendarId, " +
                            "b.d_Birthdate  as d_Birthdate, " +
                            "b.v_PersonId  as v_PersonId, " +
                            "b.v_DocNumber as v_DocNumber, "+
                            "z.v_UserName as v_CreationUser, " +
                            "d.i_MasterServiceId  as i_MasterServiceId, " +
                            //"SUM (m.r_Price)  as PrecioTotalProtocolo, " +
                            "d.v_OrganizationId, " +
                            "d.v_ServiceId, " +
                            //"d.v_ComprobantePago, " +
                            " ISNULL(d.v_ComprobantePago,'SIN COBRO') AS v_ComprobantePago,  " +
                            "d.v_NroLiquidacion, " +
                            "f1.v_IdentificationNumber as RucEmpFact, " +
                            "DATEDIFF(YEAR,b.d_Birthdate,GETDATE())-(CASE WHEN DATEADD(YY,DATEDIFF(YEAR,b.d_Birthdate,GETDATE()),b.d_Birthdate)>GETDATE() THEN 1 ELSE 0 END) as i_Edad , " +                           //"m.i_MedicoTratanteId " +
                            "a.i_CalendarStatusId as i_CalendarStatusId, " +
                            " d.d_InsertDate as d_InsertDate, " +
                            " a.v_CalendarId as v_CalendarId, "    +
                            " a.i_LineStatusId as i_LineStatusId, " +
                            " d.i_AptitudeStatusId, " +
                            " a.i_NewContinuationId, " +
                            " d.i_ServiceStatusId, " +
                            " ISNULL(d.v_ObservacionesAdicionales,'---') AS v_ObservacionesAdicionales, b.v_TelephoneNumber as v_TelephoneNumber , " +
                            " ISNULL(mkt.v_Value1, '- - -' ) as 'MKT', " +
                            " ISNULL(vt.v_SerieDocumento + '-' + VT.v_CorrelativoDocumento,'- - -')as ComprobanteCobro, " + 
                            " ISNULL(vt.d_Total, 0) as TotalPagado "  +
                            "FROM calendar a "+
                            "INNER JOIN systemuser z on a.i_InsertUserId = z.i_SystemUserId " +
                            "INNER JOIN person b on a.v_PersonId = b.v_PersonId "+
                            "INNER JOIN systemparameter sp1 on a.i_LineStatusId = sp1.i_ParameterId and sp1.i_GroupId = 120 "+
                            "INNER JOIN service d on a.v_ServiceId = d.v_ServiceId "+
                            " LEFT JOIN systemparameter mkt on d.i_ProcedenciaPac_Mkt = mkt.i_ParameterId and mkt.i_GroupId = 413 " + 
                            "LEFT JOIN organization f1 on d.v_OrganizationId = f1.v_OrganizationId " +
                            "INNER JOIN systemparameter sp2 on a.i_ServiceTypeId = sp2.i_ParameterId and sp2.i_GroupId = 119 "+
                            "INNER JOIN systemparameter sp3 on a.i_ServiceId = sp3.i_ParameterId and sp3.i_GroupId = 119 "+
                            "INNER JOIN systemparameter sp4 on a.i_NewContinuationId = sp4.i_ParameterId and sp4.i_GroupId = 121 "+
                            "INNER JOIN systemparameter sp5 on a.i_CalendarStatusId = sp5.i_ParameterId and sp5.i_GroupId = 122 "+
                            "INNER JOIN systemparameter sp6 on a.i_IsVipId = sp6.i_ParameterId and sp6.i_GroupId = 111 "+
                            "LEFT JOIN protocol e on d.v_ProtocolId = e.v_ProtocolId "+
                            "LEFT JOIN systemparameter sp7 on e.i_EsoTypeId = sp7.i_ParameterId and sp7.i_GroupId = 118 " +
                            "INNER JOIN systemparameter sp8 on d.i_ServiceStatusId = sp8.i_ParameterId and sp8.i_GroupId = 125 "+
                            "INNER JOIN systemparameter sp9 on d.i_AptitudeStatusId = sp9.i_ParameterId and sp9.i_GroupId = 124 "+

                            "INNER JOIN datahierarchy dt1 on b.i_DocTypeId = dt1.i_ItemId and dt1.i_GroupId = 106 "+
                            
                            "LEFT JOIN organization f on e.v_CustomerOrganizationId = f.v_OrganizationId "+
                            "LEFT JOIN location g on g.v_OrganizationId = e.v_CustomerOrganizationId and e.v_CustomerLocationId = g.v_LocationId "+

                            "LEFT JOIN organization h on e.v_WorkingOrganizationId = h.v_OrganizationId "+
                            "LEFT JOIN location i on i.v_OrganizationId = e.v_WorkingOrganizationId and e.v_WorkingLocationId = i.v_LocationId "+

                            "LEFT JOIN organization j on d.v_OrganizationId = j.v_OrganizationId "+
                            "LEFT JOIN location k on d.v_OrganizationId = k.v_OrganizationId and d.v_LocationId = k.v_LocationId "+
                            "INNER JOIN groupoccupation l on l.v_GroupOccupationId = e.v_GroupOccupationId " +
                            " left join [20505310072].dbo.venta vt On replace((replace(d.v_ComprobantePago,' | ', '')),' ','') = vt.v_SerieDocumento + '-' + VT.v_CorrelativoDocumento " +
                            //"INNER JOIN servicecomponent m on d.v_ServiceId = m.v_ServiceId " +
                            " WHERE (" + nroDoc + " is null or " + nroDoc + "  = B.v_DocNumber )" +
                            "AND(d_DateTimeCalendar > CONVERT(datetime,'" + fi + "',103) and  d_DateTimeCalendar < CONVERT(datetime,'" + ff + "',103)) " +
                            //"AND(" + tipoServicio + " is null or " + tipoServicio + " = i_ServiceTypeId) " +
                            "AND(" + servicio + " is null or " + servicio + " = i_ServiceId) " +
                            "AND(" + modalidad + " is null or " + modalidad + " = i_NewContinuationId) " +
                            "AND(" + cola + " is null or " + cola + " = i_LineStatusId) " +
                            "AND(" + vip + " is null or " + vip + " = i_IsVipId) " +
                            "AND(" + estadoCita + " is null or " + estadoCita + " = i_CalendarStatusId) " +
                            "AND(" + paciente + " is null or B.v_FirstLastName + ' ' + B.v_SecondLastName + ' ' + B.v_FirstName  like " + paciente + ") " +
                            "AND a.i_IsDeleted = 0 "+
                            "AND a.i_IsDeleted = 0 " +
                            //"AND m.i_IsDeleted = 0  " +
                            //"AND m.i_IsRequiredId = 1  " +
                            "GROUP BY a.i_ServiceTypeId, d.v_ServiceId,a.d_DateTimeCalendar,B.v_FirstLastName,B.v_SecondLastName,B.v_FirstName,B.v_DocNumber,sp1.v_Value1,sp8.v_Value1,a.d_SalidaCM,sp9.v_Value1,sp2.v_Value1,sp3.v_Value1,sp4.v_Value1,sp7.v_Value1,sp5.v_Value1,e.v_Name,sp6.v_Value1,f.v_Name,g.v_Name,j.v_Name,k.v_Name,a.d_EntryTimeCM,l.v_Name,b.v_CurrentOccupation,l.v_Name,b.v_CurrentOccupation,B.v_FirstName,B.v_FirstLastName,B.v_SecondLastName,h.v_Name,e.v_ProtocolId,a.v_CalendarId,b.d_Birthdate,d.i_MasterServiceId,b.v_PersonId,d.v_OrganizationId,f1.v_IdentificationNumber, z.v_UserName, d.v_ComprobantePago, d.v_NroLiquidacion, i_CalendarStatusId, d.d_InsertDate, a.v_CalendarId, a.i_LineStatusId, d.i_AptitudeStatusId, a.i_NewContinuationId, d.i_ServiceStatusId, d.v_ObservacionesAdicionales, b.v_TelephoneNumber, mkt.v_Value1, vt.v_SerieDocumento, VT.v_CorrelativoDocumento, vt.d_Total ";

                var data = cnx.Query<AgendaDto>(query).ToList();
                return data;
            }
           
        }

        public static ImagenesTrabajadorDto ObtenerImagenesTrabajador(string personId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                var query = @"SELECT " +
                            "a.b_PersonImage as FotoTrabajador, " +
                            "a.b_FingerPrintImage as HuellaTrabajador, " +
                            "a.b_RubricImage as FirmaTrabajador " +
                            "FROM person a " +
                            "WHERE ('" + personId + "'  = a.v_PersonId ) ";

                var data = cnx.Query<ImagenesTrabajadorDto>(query).ToList().FirstOrDefault();
                return data;
            }
        }

        public static List<Categoria> GetAllComponentsByService(string pstrString)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                var query = @"SELECT A.v_ComponentId as  v_ComponentId, " +
                            "b.v_Name as v_ComponentName, " +

                            "a.i_ServiceComponentStatusId as i_ServiceComponentStatusId, " +
                            "sp2.v_Value1 as v_ServiceComponentStatusName, " +
                            "a.d_StartDate as d_StartDate, " +
                            "a.d_EndDate as d_EndDate, " +
                            "a.i_QueueStatusId as i_QueueStatusId, " +
                            "sp3.v_Value1 as v_QueueStatusName, " +

                            "c.i_ServiceStatusId as ServiceStatusId, " +
                            "c.v_Motive as v_Motive, " +
                            "b.i_CategoryId as i_CategoryId, " +
                            "sp4.v_Value1 as v_CategoryName, " +
                            "c.v_ServiceId as v_ServiceId, " +
                            "a.v_ServiceComponentId as v_ServiceComponentId, " +
                            " a.r_Price as r_Price, "+
                            " a.d_SaldoPaciente as d_SaldoPaciente, " +
                            " a.d_SaldoAseguradora as d_SaldoAseguradora " +
                            "FROM servicecomponent a "+
                            "INNER JOIN systemparameter sp2 on a.i_ServiceComponentStatusId = sp2.i_ParameterId and sp2.i_GroupId = 127 " +
                            "INNER JOIN component b on a.v_ComponentId = b.v_ComponentId " +
                            "INNER JOIN systemparameter sp3 on a.i_QueueStatusId = sp3.i_ParameterId and sp3.i_GroupId = 128 " +
                            "INNER JOIN service c on a.v_ServiceId = c.v_ServiceId " +
                            "LEFT JOIN systemparameter sp4 on b.i_CategoryId = sp4.i_ParameterId and sp4.i_GroupId = 116 " + 
                            "WHERE ('" + pstrString + "'  = a.v_ServiceId ) " +
                            "AND a.i_IsDeleted = 0"+
                            "AND a.i_IsRequiredId = 1"
                            ;
                var data = cnx.Query<ServiceComponentList>(query).ToList();

                var xxx = new List<Categoria>();
                Categoria oCategoria = null;
                foreach (var item in data)
                {
                    oCategoria = new Categoria
                    {
                        v_ComponentId = item.v_ComponentId,
                        v_ComponentName = item.v_ComponentName,
                        v_ServiceComponentId = item.v_ServiceComponentId,
                        i_CategoryId = item.i_CategoryId,
                        v_CategoryName = item.v_CategoryName,
                        v_ServiceComponentStatusName = item.v_ServiceComponentStatusName,
                        v_QueueStatusName = item.v_QueueStatusName,
                        i_ServiceComponentStatusId = item.i_ServiceComponentStatusId.Value,
                        r_Price = decimal.Parse(item.r_Price.ToString()),
                        d_SaldoPaciente = decimal.Parse(item.d_SaldoPaciente.ToString()),
                        d_SaldoAseguradora = decimal.Parse(item.d_SaldoAseguradora.ToString())
                    };
                    xxx.Add(oCategoria);
                }

                var objData = xxx.AsEnumerable()
                       .Where(s => s.i_CategoryId != -1)
                       .GroupBy(x => x.i_CategoryId)
                       .Select(group => group.First());
                var obj = objData.ToList();
                Categoria objCategoriaList;
                var Lista = new List<Categoria>();

                for (int i = 0; i < obj.Count(); i++)
                {
                    objCategoriaList = new Categoria();

                    objCategoriaList.i_CategoryId = obj[i].i_CategoryId.Value;
                    objCategoriaList.v_CategoryName = obj[i].v_CategoryName;
                    objCategoriaList.v_ServiceComponentStatusName = obj[i].v_ServiceComponentStatusName;
                    objCategoriaList.v_QueueStatusName = obj[i].v_QueueStatusName;
                    objCategoriaList.i_ServiceComponentStatusId = obj[i].i_ServiceComponentStatusId;
                    var x = data.ToList().FindAll(p => p.i_CategoryId == obj[i].i_CategoryId.Value);

                    x.Sort((z, y) => z.v_ComponentName.CompareTo(y.v_ComponentName));
                    ComponentDetailList objComponentDetailList;
                    List<ComponentDetailList> ListaComponentes = new List<ComponentDetailList>();
                    foreach (var item in x)
                    {
                        objComponentDetailList = new ComponentDetailList();

                        objComponentDetailList.v_ComponentId = item.v_ComponentId;
                        objComponentDetailList.v_ComponentName = item.v_ComponentName;
                        objComponentDetailList.v_ServiceComponentId = item.v_ServiceComponentId;
                        objComponentDetailList.r_Price = "S/. "+item.r_Price.ToString();
                        objComponentDetailList.d_SaldoPaciente = "S/. " + item.d_SaldoPaciente.ToString();
                        objComponentDetailList.d_SaldoAseguradora = "S/. " + item.d_SaldoAseguradora.ToString();
                        ListaComponentes.Add(objComponentDetailList);
                    }
                    objCategoriaList.Componentes = ListaComponentes;

                    Lista.Add(objCategoriaList);

                }
                return Lista;
            }
        }

        public static List<ServiceComponentList> GetServiceComponents_(string pstrServiceId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                var query = @"SELECT a.v_ComponentId as v_ComponentId, c.v_Name as v_ComponentName, a.v_ServiceComponentId " +
                            "FROM servicecomponent a inner join component c on a.v_ComponentId = c.v_ComponentId" +
                            " WHERE a.i_IsDeleted = 0 AND a.i_IsRequiredId = 1 and a.v_ServiceId = '" + pstrServiceId + "'";

                var data = cnx.Query<ServiceComponentList>(query).ToList();
                return data;
            }
        }

        public static DatosSeguro DatosSeguro_(string planId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                var query = @"SELECT pl.d_Importe as Deducible, pl.d_ImporteCo as Coaseguro " +
                            "FROM [SigesoftDesarrollo_2].dbo.[plan] as pl" +
                            " WHERE pl.i_PlanId = '" + planId + "'";

                var data = cnx.Query<DatosSeguro>(query).ToList().FirstOrDefault();
                return data;
            }
        }


        public static void UpdateAdditionalExam(List<ServiceComponentList> pobjDtoEntity, string serviceId, int? isRequiredId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                var serviceComponentId = pobjDtoEntity.Select(p => p.v_ServiceComponentId).ToArray();

                var query = @"SELECT * " +
                            "FROM servicecomponent a " +
                            "WHERE a.v_ServiceId = '" + serviceId + "' and a.v_ServiceComponentId in (" + ObtenerArrayConcatenado(serviceComponentId) + ")  ";
                var data = cnx.Query<ServiceComponentList>(query).ToList();

                foreach (var item in data)
                {
                    var actualizar = "UPDATE servicecomponent SET " +
                                     "d_UpdateDate = GETDATE(), " +
                                     "i_IsRequiredId = " + isRequiredId +
                                     "WHERE v_ServiceComponentId = '" + item.v_ServiceComponentId + "'";
                    cnx.Execute(actualizar);
                }
            }
        }

        public void UpdateIsFact(string serviceId, int? value)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                
                    var actualizar = "UPDATE service SET " +
                                     "d_UpdateDate = GETDATE(), " +
                                     "i_IsFac = " + value +
                                     "WHERE v_ServiceId = '" + serviceId + "'";
                    cnx.Execute(actualizar);
               
            }
        }

        public void UpdateServiceComponentPrice(decimal _Price, decimal _Cantidad, decimal _Descuento, decimal _SaldoPaciente, decimal _SaldoSeguro, string _ServiceComponentId, int _Plan)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                if (_Plan == 0)
                {
                    var actualizar = "UPDATE servicecomponent SET " +
                                " r_Price = " + _Price + " , " +
                                " i_Cantidad = " + _Cantidad + " , " +
                                " d_Descuento = " + _Descuento +
                                " WHERE v_ServiceComponentId = '" + _ServiceComponentId + "'";
                    cnx.Execute(actualizar);
                }
                else
                {
                    var actualizar = "UPDATE servicecomponent SET " +
                                     " r_Price = " + _Price + " , " +
                                     " i_Cantidad = " + _Cantidad + " , " +
                                     " d_Descuento = " + _Descuento + " , " +
                                     " d_SaldoPaciente = " + _SaldoPaciente + " , " +
                                     " d_SaldoAseguradora = " + _SaldoSeguro +
                                     " WHERE v_ServiceComponentId = '" + _ServiceComponentId + "'";
                    cnx.Execute(actualizar);
                }
            }
        }


        public void UpdateNroLiquiEnServicio(string serviceId, string nroLiquidacion)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != ConnectionState.Open) cnx.Open();

                var actualizar = "UPDATE service SET " +
                                 " d_UpdateDate = GETDATE(), " +
                                 " i_UpdateUserId = 11," +
                                 " v_NroLiquidacion = '" + nroLiquidacion + "'" +
                                 " WHERE v_ServiceId = '" + serviceId + "'";
                cnx.Execute(actualizar);

            }
        }

        public void GenerarLiquidacion(string serviceId, string organizationId, decimal montoFactura, DateTime fechaVencimiento,string nroFact)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != ConnectionState.Open) cnx.Open();
                    var nroLiq = ObtenerNroLiquidacionContado(9);

                    var oliquidacionDto = new liquidacionDto();

                    oliquidacionDto.v_NroLiquidacion = nroLiq;
                    oliquidacionDto.v_OrganizationId = organizationId;
                    oliquidacionDto.d_Monto = montoFactura;
                    oliquidacionDto.d_FechaVencimiento = fechaVencimiento.Date;
                    oliquidacionDto.v_NroFactura = nroFact;
                    oliquidacionDto.i_InsertUserId = 11;
                    oliquidacionDto.d_InsertDate = DateTime.Now;
                    AddLiquidacion(oliquidacionDto);
                    UpdateNroLiquiEnServicio(serviceId, nroLiq);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void AddLiquidacion(liquidacionDto pobjDtoEntity)
        {
            try
            {
                var secuentialId = GetNextSecuentialId(400).SecuentialId;
                var newId = GetNewId(9, secuentialId, "LQ");
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query =
                        @"INSERT INTO liquidacion (v_LiquidacionId,v_NroLiquidacion,v_OrganizationId,d_Monto,d_FechaVencimiento,v_NroFactura) " +
                        "VALUES('" + newId + "', " +
                        "'" + pobjDtoEntity.v_NroLiquidacion + "', " +
                         "'" + pobjDtoEntity.v_OrganizationId + "', " +
                        "" + pobjDtoEntity.d_Monto + ", " +
                        //"" + CONVERT(datetime,'" + pobjDtoEntity.d_FechaVencimiento + "',103) + ", " +
                        "CONVERT(datetime,'" + pobjDtoEntity.d_FechaVencimiento.Date.ToShortDateString() + "',103), " +
                        "'" + pobjDtoEntity.v_NroFactura + "' )";
                    cnx.Execute(query);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public class liquidacionDto
        {
            public string v_LiquidacionId { get; set; }
            public string v_NroLiquidacion { get; set; }
            public string v_OrganizationId { get; set; }
            public decimal d_Monto { get; set; }
            public DateTime d_FechaVencimiento { get; set; }
            public int i_InsertUserId { get; set; }
            public DateTime d_InsertDate { get; set; }
            public string v_NroFactura { get; set; }
        }
        public string ObtenerNroLiquidacionContado(int nodeId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query =
                        "select top 1 v_NroLiquidacion from liquidacion where v_NroLiquidacion like 'N009-CON%' order by v_NroLiquidacion desc";

                    var nroLiquidacionContado = cnx.Query<string>(query).FirstOrDefault();

                    

                    if (nroLiquidacionContado == null)
                    {
                       return string.Format("N{0}-{1}", nodeId.ToString("000"), "CON000001");
                    }

                    var nro = int.Parse(nroLiquidacionContado.Split('-').ToArray()[1].Substring(3,6)) + 1;

                    return string.Format("N{0}-{1}", nodeId.ToString("000"), nro.ToString("CON000000"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void UpdateLiquidacion(string nroLiqui, string nroFact, DateTime? fechaVencimiento)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                
                    var actualizar = "UPDATE liquidacion SET " +
                                     " v_NroFactura = '"+nroFact + "', " +
                                     " d_FechaVencimiento = '" + fechaVencimiento.Value.ToShortDateString() + "' " +
                                     " WHERE v_NroLiquidacion = '" + nroLiqui + "'";
                    cnx.Execute(actualizar);


                    var servicios = @"SELECT b.v_ServiceId " +
                           " FROM service b " +
                           " WHERE b.i_IsDeleted = 0 and b.v_NroLiquidacion = '" + nroLiqui + "'";

                    var data = cnx.Query<Liquidacion>(servicios).ToList();

                    foreach (var item in data)
                    {
                        UpdateIsFact(item.v_ServiceId, 2);
                    }
                   
               
            }
            }
            catch (Exception ex)
            {
                
                throw;
            }
        }


        public void UpdatePagoHospitalizacion(string hopitalizacionId, string aCargo, int esCancelado)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    var actualizar = "";
                    if (aCargo == "Paciente")
	                {
		                 actualizar = "UPDATE hospitalizacion SET " +
                                        " i_PacientePago = " + esCancelado  +
                                        " WHERE v_HopitalizacionId = '" + hopitalizacionId + "'";
                      
	                }
                    else if (aCargo == "Medico"){

                        actualizar = "UPDATE hospitalizacion SET " +
                                       " i_MedicoPago = " + esCancelado +
                                       " WHERE v_HopitalizacionId = '" + hopitalizacionId + "'";
                    }

                    cnx.Execute(actualizar);

                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        private static string ObtenerArrayConcatenado(string[] strings)
        {
            try
            {
                return string.Join(", ", strings.Select(p => GetQuotedString(p)));
            }
            catch (Exception ex)
            {
                return "''";
            }
        }

        public static string GetQuotedString(string str)
        {
            return str != null ? "'" + str.Trim() + "'" : "''";
        }
        
        public static List<Categoria> GetAllComponents(int? filterType, string name)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                string codigoSegus = "";
                string nameCategory = "";
                string nameComponent = "";
                string nameSubCategory = "";
                string componentId = "";
                if (filterType == (int)Enums.TipoBusqueda.CodigoSegus)
                {
                    codigoSegus = name;

                }
                else if (filterType == (int)Enums.TipoBusqueda.NombreCategoria)
                {
                    nameCategory = name;
                }
                else if (filterType == (int)Enums.TipoBusqueda.NombreComponent)
                {
                    nameComponent = name;
                }
                else if (filterType == (int)Enums.TipoBusqueda.ComponentId)
                {
                    componentId = name;
                }
                else if (filterType == (int)Enums.TipoBusqueda.NombreSubCategoria)
                {
                    nameSubCategory = name;
                };


                string query = "";


                if (name == "")
                {
                    query = @"SELECT sp4.v_Value1 as v_CategoryName, b.i_CategoryId as i_CategoryId, b.v_Name as v_ComponentName, b.v_ComponentId as v_ComponentId " +
                            "FROM component b " +
                            "LEFT JOIN systemparameter sp4 on b.i_CategoryId = sp4.i_ParameterId and sp4.i_GroupId = 116 " +
                            "WHERE b.i_IsDeleted = 0 ";
                }
                else if (filterType == (int)Enums.TipoBusqueda.ComponentId)
                {
                    query = @"SELECT sp4.v_Value1 as v_CategoryName, b.i_CategoryId as i_CategoryId, b.v_Name as v_ComponentName, b.v_ComponentId as v_ComponentId " +
                            "FROM component b " +
                            "LEFT JOIN systemparameter sp4 on b.i_CategoryId = sp4.i_ParameterId and sp4.i_GroupId = 116 " +
                            "WHERE b.i_IsDeleted = 0 and b.v_ComponentId = '" + componentId + "'";
                }     
                else
                {
                    query = @"SELECT distinct sp4.v_Value1 as v_CategoryName, b.i_CategoryId as i_CategoryId, b.v_Name as v_ComponentName, b.v_ComponentId as v_ComponentId " +
                            "FROM component b " +
                            "LEFT JOIN systemparameter sp4 on b.i_CategoryId = sp4.i_ParameterId and sp4.i_GroupId = 116 " +
                            "LEFT JOIN systemparameter sp5 on sp4.i_ParameterId = sp5.i_ParentParameterId and sp5.i_GroupId = 116 " +
                            "WHERE b.i_IsDeleted = 0 and b.v_CodigoSegus like '%" + codigoSegus + "%' and b.v_Name like '%" + nameComponent + "%' and sp4.v_Value1 like '%" + nameCategory + "%' and sp5.v_Value1 like '%" + nameSubCategory + "%'";
                }
                

                var data = cnx.Query<Categoria>(query).ToList();

                var objData = data.AsEnumerable()
                           .Where(s => s.i_CategoryId != -1)
                           .GroupBy(x => x.i_CategoryId)
                           .Select(group => group.First());

                List<Categoria> obj = objData.ToList();

                Categoria objCategoriaList;
                List<Categoria> Lista = new List<Categoria>();

                //int CategoriaId_Old = 0;
                for (int i = 0; i < obj.Count(); i++)
                {
                    objCategoriaList = new Categoria();

                    objCategoriaList.i_CategoryId = obj[i].i_CategoryId.Value;
                    objCategoriaList.v_CategoryName = obj[i].v_CategoryName;
                    var x = data.ToList().FindAll(p => p.i_CategoryId == obj[i].i_CategoryId.Value);

                    x.Sort((z, y) => z.v_ComponentName.CompareTo(y.v_ComponentName));
                    ComponentDetailList objComponentDetailList;
                    List<ComponentDetailList> ListaComponentes = new List<ComponentDetailList>();
                    foreach (var item in x)
                    {
                        objComponentDetailList = new ComponentDetailList();

                        objComponentDetailList.v_ComponentId = item.v_ComponentId;
                        objComponentDetailList.v_ComponentName = item.v_ComponentName;
                        //objComponentDetailList.v_ServiceComponentId = item.v_ServiceComponentId;
                        ListaComponentes.Add(objComponentDetailList);
                    }
                    objCategoriaList.Componentes = ListaComponentes;

                    Lista.Add(objCategoriaList);

                }
                return Lista;
            }
        }

        public static componentDto GetMedicalExam(string pstrMedicalExamId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                var query = @"SELECT * " +
                            "FROM component a " +
                            "WHERE a.i_IsDeleted = 0 AND a.v_ComponentId = '" + pstrMedicalExamId + "'";

                var data = cnx.Query<componentDto>(query).ToList().FirstOrDefault();
                return data;
            }
        }


        public static systemparameterDto GetSystemParameter(int pintGroupId, int pintParameterId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                var query = @"SELECT * " +
                            "FROM systemparameter a " +
                            "WHERE a.i_GroupId = " + pintGroupId + " AND a.i_ParameterId = " + pintParameterId + "";

                var data = cnx.Query<systemparameterDto>(query).ToList().FirstOrDefault();
                return data;
            }
        }

        public static void AddServiceComponent(ServiceComponentList pobjDtoEntity)
        {
            try
            {
                var secuentialId = GetNextSecuentialId(24).SecuentialId;
                var newId = GetNewId(9, secuentialId, "SC");

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (pobjDtoEntity.r_Price != null)
                    {
                        pobjDtoEntity.r_Price = SetNewPrice(pobjDtoEntity.r_Price.Value, pobjDtoEntity.v_ComponentId);
                    }
                    var query =
                        @"INSERT INTO servicecomponent (v_ServiceComponentId,v_ServiceId,i_ExternalInternalId,i_ServiceComponentTypeId,i_IsVisibleId,i_IsInheritedId,i_index,i_InsertUserId,r_Price,v_ComponentId,i_IsInvoicedId,i_ServiceComponentStatusId,i_QueueStatusId,i_Iscalling,i_Iscalling_1,i_IsManuallyAddedId,i_IsRequiredId,v_IdUnidadProductiva,d_SaldoPaciente,d_SaldoAseguradora, i_ConCargoA, i_MedicoTratanteId,d_InsertDate,i_IsDeleted, i_TipoDesc, i_ApplicantMedic, i_PayMedic, i_Cantidad, d_Descuento ) " +
                        "VALUES('" + newId + "', " +
                        "'" + pobjDtoEntity.v_ServiceId + "', " +
                        "'" + pobjDtoEntity.i_ExternalInternalId + "', " +
                         "" + pobjDtoEntity.i_ServiceComponentTypeId + ", " +
                        "" + pobjDtoEntity.i_IsVisibleId + ", " +
                        "" + pobjDtoEntity.i_IsInheritedId + ", " +
                        //"" + pobjDtoEntity.d_StartDate + ", " +
                        //"" + pobjDtoEntity.d_EndDate + ", " +
                        "" + pobjDtoEntity.i_index + ", " +
                        "" + pobjDtoEntity.i_InsertUserId + ", " +
                        "" + pobjDtoEntity.r_Price + ", " +
                        "'" + pobjDtoEntity.v_ComponentId + "', " +
                        "" + pobjDtoEntity.i_IsInvoicedId + ", " +
                        "" + pobjDtoEntity.i_ServiceComponentStatusId + ", " +
                        "" + pobjDtoEntity.i_QueueStatusId + ", " +
                        "" + pobjDtoEntity.i_Iscalling + ", " +
                        "" + pobjDtoEntity.i_Iscalling_1 + ", " +
                        "" + pobjDtoEntity.i_IsManuallyAddedId + ", " +
                        "" + pobjDtoEntity.i_IsRequiredId + ", " +
                        "'" + pobjDtoEntity.v_IdUnidadProductiva + "', "+
                        "" + pobjDtoEntity.d_SaldoPaciente + ", " +
                        "" + pobjDtoEntity.d_SaldoAseguradora + ", " +
                        "" + pobjDtoEntity.i_ConCargoA + ", " +
                         "" + pobjDtoEntity.i_MedicoTratanteId + ", " +
                        "GETDATE()," +
                        " 0, "+
                        "" + pobjDtoEntity.i_TipoDesc + " , "+
                        "" + pobjDtoEntity.i_MedicoRealizaId + " , " +
                        "" + pobjDtoEntity.i_PayMedic + " , " + 
                        "" + pobjDtoEntity.i_Cantidad + " ,"+
                        ""+ pobjDtoEntity.d_Descuento + " ) "
                        ;
                    cnx.Execute(query);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        #region antes
        public static List<CalendarList> GetHeaderRoadMap(string calendarId, DateTime fechaNacimiento)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                var query = @"SELECT " +
                            "a.v_CalendarId as v_CalendarId, " +
                            "a.i_InsertUserId as i_InsertUserId, " +
                            "b1.v_UserName as v_UserName, " +
                            "b2.v_FirstLastName + ' ' + b2.v_SecondLastName as UsuarioCalendar, " +
                            "B.v_FirstLastName + ' ' + B.v_SecondLastName + ' ' + B.v_FirstName as v_Pacient, " +
                            "e.v_Name as v_ProtocolName, " +
                            "sp2.v_Value1 as v_ServiceTypeName, " +
                            "sp3.v_Value1 as v_ServiceName, " +
                            "b.v_PersonId as v_PersonId, " +
                            "b.v_DocNumber as v_DocNumber," +
                            "b.d_Birthdate as FechaNacimiento, "+
                            "sp7.v_Value1 as v_EsoTypeName, " +
                            "f.v_Name as v_OrganizationName, " +
                            "g.v_Name + ' / ' + g.v_Name as v_OrganizationLocationService, " +
                            "h.v_Name as v_OrganizationIntermediaryName, "+
                            "ll.v_Name as EmpresaPropietariaDireccion, " +
                            "B.b_PersonImage as FotoTrabajador, " +
                            "d.d_ServiceDate as d_ServiceDate, " +
                            "b.v_CurrentOccupation as Puesto," +
                            "b.v_TelephoneNumber as v_TelephoneNumber, " +
                            "d.v_ServiceId as ServicioId, " +
                            "sp1.v_Value1 as Genero, " +
                            GetAge(fechaNacimiento) + " as i_Edad " +
                            "FROM calendar a " +
                            "INNER JOIN person b on a.v_PersonId = b.v_PersonId " +
                            "INNER JOIN systemuser b1 on a.i_InsertUserId = b1.i_SystemuserId " +
                            "INNER JOIN person b2 on b1.v_PersonId = b2.v_PersonId " +
                            "INNER JOIN service d on a.v_ServiceId = d.v_ServiceId " +
                            "INNER JOIN systemparameter sp2 on a.i_ServiceTypeId = sp2.i_ParameterId and sp2.i_GroupId = 119 " +
                            "INNER JOIN systemparameter sp3 on a.i_ServiceId = sp3.i_ParameterId and sp3.i_GroupId = 119 " +
                            "LEFT JOIN protocol e on d.v_ProtocolId = e.v_ProtocolId " +
                            "LEFT JOIN systemparameter sp7 on e.i_EsoTypeId = sp7.i_ParameterId and sp7.i_GroupId = 118 " +
                            "LEFT JOIN organization f on e.v_CustomerOrganizationId = f.v_OrganizationId " +
                            "LEFT JOIN location g on g.v_OrganizationId = e.v_CustomerOrganizationId and e.v_CustomerLocationId = g.v_LocationId " +
                            "LEFT JOIN organization j on d.v_OrganizationId = j.v_OrganizationId " +
                            "LEFT JOIN location k on d.v_OrganizationId = k.v_OrganizationId and d.v_LocationId = k.v_LocationId " +
                            "LEFT JOIN organization h on e.v_EmployerOrganizationId = h.v_OrganizationId " +
                            "LEFT JOIN organization ll on e.v_WorkingOrganizationId = ll.v_OrganizationId " +
                            "LEFT JOIN location i on i.v_OrganizationId = e.v_WorkingOrganizationId and e.v_WorkingLocationId = i.v_LocationId " +
                            "INNER JOIN systemparameter sp1 on b.i_SexTypeId = sp1.i_ParameterId and sp1.i_GroupId = 100 " +
                            "WHERE a.v_CalendarId = '" + calendarId + "'"+
                            "AND a.i_IsDeleted = 0";

                var data = cnx.Query<CalendarList>(query).ToList();
                return data;
            }
            return null;
        }
        #endregion
        public static List<ServiceComponentList> GetServiceComponentsByCategoryExceptLab(string pstrServiceId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                //var query = @"SELECT " +
                //            "a.v_ComponentId as v_ComponentId, " +
                //            "b.v_Name as v_ComponentName, " +
                //            "a.i_ServiceComponentStatusId as i_ServiceComponentStatusId, " +
                //            "sp1.v_Value1 as v_ServiceComponentStatusName, " +
                //            "a.d_StartDate as d_StartDate, " +
                //            "a.d_EndDate as d_EndDate, " +
                //            "a.i_QueueStatusId as i_QueueStatusId, " +
                //            "sp2.v_Value1 as v_QueueStatusName, " +
                //            "d.i_ServiceStatusId as ServiceStatusId, " +
                //            "d.v_Motive as v_Motive, " +
                //            "b.i_CategoryId as i_CategoryId, " +
                //            "sp3.v_Value1 as v_CategoryName, " +
                //             "d.v_ServiceId as v_ServiceId " +

                //            "FROM servicecomponent a " +
                //            "INNER JOIN systemparameter sp1 on a.i_ServiceComponentStatusId = sp1.i_ParameterId and sp1.i_GroupId = 127 " +
                //            "INNER JOIN component b on a.v_ComponentId = b.v_ComponentId " +
                //            "INNER JOIN systemparameter sp2 on a.i_QueueStatusId = sp2.i_ParameterId and sp2.i_GroupId = 128 " +
                //            "INNER JOIN service d on a.v_ServiceId = d.v_ServiceId " +
                //            "LEFT JOIN systemparameter sp3 on b.i_CategoryId = sp3.i_ParameterId and sp3.i_GroupId = 116 " +
                //            "WHERE a.v_ServiceId = '" + pstrServiceId + "' " +
                //            "AND a.i_IsDeleted = 0 " +
                //            "AND a.i_IsRequiredId = 1 " +
                //            "AND a.v_ComponentId != 'N009-ME000000002' " + "AND a.v_ComponentId != 'N009-ME000000440' " //+ "AND a.v_ComponentId != 'N009-ME000000442' "
                //            ;
                var query = @" EXEC GetTicketReport '" + pstrServiceId + "'";

                var data = cnx.Query<ServiceComponentList>(query).ToList();
                var objData = data.AsEnumerable()
                             .Where(s => s.i_CategoryId != -1)
                             .GroupBy(x => x.i_CategoryId)
                             .Select(group => group.First());

                List<ServiceComponentList> obj = objData.ToList();

                obj.AddRange(data.Where(p => p.i_CategoryId == -1));
                obj.AddRange(data.Where(p => p.i_CategoryId == 22));
                //obj.AddRange(data.Where(p => p.i_CategoryId == 6));
                //obj.AddRange(data.Where(p => p.i_CategoryId == 14));
                var orden = obj.OrderBy(o => o.i_CategoryId).ToList();
                return orden.FindAll(p => p.i_CategoryId != 10);
            }
        }

        public static List<ServiceComponentList> GetServiceComponentsByCategoryExceptLab_Asist(string pstrServiceId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                var query = @"SELECT " +
                            "a.v_ComponentId as v_ComponentId, " +
                            "b.v_Name as v_ComponentName, " +
                            "a.i_ServiceComponentStatusId as i_ServiceComponentStatusId, " +
                            "sp1.v_Value1 as v_ServiceComponentStatusName, " +
                            "a.d_StartDate as d_StartDate, " +
                            "a.d_EndDate as d_EndDate, " +
                            "a.i_QueueStatusId as i_QueueStatusId, " +
                            "sp2.v_Value1 as v_QueueStatusName, " +
                            "d.i_ServiceStatusId as ServiceStatusId, " +
                            "d.v_Motive as v_Motive, " +
                            "b.i_CategoryId as i_CategoryId, " +
                            "sp3.v_Value1 as v_CategoryName, " +
                             "d.v_ServiceId as v_ServiceId " +

                            "FROM servicecomponent a " +
                            "INNER JOIN systemparameter sp1 on a.i_ServiceComponentStatusId = sp1.i_ParameterId and sp1.i_GroupId = 127 " +
                            "INNER JOIN component b on a.v_ComponentId = b.v_ComponentId " +
                            "INNER JOIN systemparameter sp2 on a.i_QueueStatusId = sp2.i_ParameterId and sp2.i_GroupId = 128 " +
                            "INNER JOIN service d on a.v_ServiceId = d.v_ServiceId " +
                            "LEFT JOIN systemparameter sp3 on b.i_CategoryId = sp3.i_ParameterId and sp3.i_GroupId = 116 " +
                            "WHERE a.v_ServiceId = '" + pstrServiceId + "' " +
                            "AND a.i_IsDeleted = 0 " +
                            "AND a.i_IsRequiredId = 1 " +
                            "AND a.v_ComponentId != 'N009-ME000000002' " + "AND a.v_ComponentId != 'N009-ME000000440' " //+ "AND a.v_ComponentId != 'N009-ME000000442' "
                            ;

                var data = cnx.Query<ServiceComponentList>(query).ToList();
                //var objData = data.AsEnumerable()
                //             .Where(s => s.i_CategoryId != -1)
                //             .GroupBy(x => x.i_CategoryId)
                //             .Select(group => group.First());

                //List<ServiceComponentList> obj = objData.ToList();

                //obj.AddRange(data.Where(p => p.i_CategoryId == -1));
                //obj.AddRange(data.Where(p => p.i_CategoryId == 22));
                //obj.AddRange(data.Where(p => p.i_CategoryId == 6));
                //obj.AddRange(data.Where(p => p.i_CategoryId == 14));
                var orden = data.OrderBy(o => o.i_CategoryId).ToList();
                return orden.FindAll(p => p.i_CategoryId != 10);
            }
        }
        public static int GetAge(DateTime FechaNacimiento)
        {
            return int.Parse((DateTime.Today.AddTicks(-FechaNacimiento.Ticks).Year - 1).ToString());

        }

        public List<KeyValueDTO> ObtenerTodasProvincia(ComboBox cbo, int pintGroupId)
        {
            SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

            List<KeyValueDTO> query = (from a in dbContext.obtenertodasprovincia_sp(pintGroupId)
                                            select new KeyValueDTO
                                       {
                                           Id = a.i_ItemId.ToString(),
                                           Value1 = a.v_Value1,
                                           Value2 = a.v_Value2,
                                           Value4 = a.i_ParentItemId.Value
                                       }).ToList();

            query.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

            return query;
            //cbo.DataSource = query;
            //cbo.DisplayMember = "Value1";
            //cbo.ValueMember = "Id";
            //cbo.SelectedIndex = 0;

            //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            //{
            //     if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
            //    //var query = @"SELECT * " +
            //    //            "FROM datahierarchy   where i_IsDeleted = 0 and i_GroupId = " + pintGroupId + " ORDER BY v_Value1";

            //    var query = "EXEC [ObtenerTodasProvincia_SP] " + pintGroupId;
            //    var data = cnx.Query<datahierarchydTO>(query).ToList();

            //    var query2 = data.AsEnumerable()
            //                .Select(x => new KeyValueDTO
            //                {
            //                    Id = x.i_ItemId.ToString(),
            //                    Value1 = x.v_Value1,
            //                    Value2 = x.v_Value2,
            //                    Value4 = x.i_ParentItemId.Value
            //                }).ToList();

            //    query2.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

            //    cbo.DataSource = query2;
            //    cbo.DisplayMember = "Value1";
            //    cbo.ValueMember = "Id";
            //}
        }

        public List<KeyValueDTO> ObtenerTodosDepartamentos(ComboBox cbo, int pintGroupId)
        {
            SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

            List<KeyValueDTO> query = (from a in dbContext.obtenertodosdepartamentos_sp(pintGroupId)
                                       select new KeyValueDTO
                                       {
                                           Id = a.i_ItemId.ToString(),
                                           Value1 = a.v_Value1,
                                           Value2 = a.v_Value2,
                                           Value4 = a.i_ParentItemId.Value
                                       }).ToList();

            query.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

            return query;
            //cbo.DataSource = query;
            //cbo.DisplayMember = "Value1";
            //cbo.ValueMember = "Id";

            //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            //{
            //    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                
            //    var query = "EXEC [ObtenerTodosDepartamentos_SP] " + pintGroupId;
            //    var data = cnx.Query<datahierarchydTO>(query).ToList();

            //    var query2 = data.AsEnumerable()
            //                .Select(x => new KeyValueDTO
            //                {
            //                    Id = x.i_ItemId.ToString(),
            //                    Value1 = x.v_Value1,
            //                    Value2 = x.v_Value2,
            //                    Value4 = x.i_ParentItemId.Value
            //                }).ToList();


            //    query2.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

            //    cbo.DataSource = query2;
            //    cbo.DisplayMember = "Value1";
            //    cbo.ValueMember = "Id";
            //}
        }

        public List<KeyValueDTO> ObtenerGesoProtocol(ComboBox cbo, string organizationId, string locationId)
        {
            SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

            List<KeyValueDTO> query = (from a in dbContext.obtenergesoprotocol_sp(organizationId, locationId)
                                       select new KeyValueDTO
                                       {
                                           Id = a.Id.ToString(),
                                           Value1 = a.Value1,
                                          
                                       }).ToList();

            query.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

            return query;

            //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            //{
            //    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

            //    var query = @"SELECT a.v_GroupOccupationId as Id, a.v_Name as Value1 " +
            //               "FROM groupoccupation a "+
            //               "INNER JOIN location b on a.v_LocationId = b.v_LocationId " + 
            //               "INNER JOIN organization c on b.v_OrganizationId = c.v_OrganizationId " +
            //               "WHERE a.i_IsDeleted = 0 " +
            //               "AND c.v_OrganizationId = '" + organizationId + "' " +
            //               "AND b.v_LocationId = '" + locationId + "'" ;

            //    var data = cnx.Query<KeyValueDTO>(query).ToList();
            //    data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

            //    return data;
            //    //cbo.DataSource = data;
            //    //cbo.DisplayMember = "Value1";
            //    //cbo.ValueMember = "Id";
            //    //cbo.SelectedIndex = 0;
            //}
        }

        public static string GetRecomendaciones(string pServiceId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"SELECT ddd.v_Name as v_Recommendation " +
                                "FROM recommendation ccc " +
                                "INNER JOIN masterrecommendationrestricction ddd ON ccc.v_MasterRecommendationId = ddd.v_MasterRecommendationRestricctionId " +
                                "WHERE ccc.i_IsDeleted = 0 and ccc.v_ServiceId = '" + pServiceId + "'";

                    var data = cnx.Query<string>(query).ToList();
                    return string.Join(", ", data.Select(p => p));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetRestricciones(string pServiceId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"SELECT ddd.v_Name as v_RestriccitionName " +
                                "FROM restriction ccc " +
                                "INNER JOIN masterrecommendationrestricction ddd ON ccc.v_MasterRestrictionId = ddd.v_MasterRecommendationRestricctionId " +
                                "WHERE ccc.i_IsDeleted = 0 and ccc.v_ServiceId = '" + pServiceId + "'";

                    var data = cnx.Query<string>(query).ToList();
                    return string.Join(", ", data.Select(p => p));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<recetadespachoDto> GetRecetaToReport( string serviceId, string v_DiagnosticRepositoryId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    var medicamentos = ObtenerContasolMedicamentos();
                    var datosMedico = ObtenerFirmaMedicoExamen(serviceId, "N009-ME000000405", "N009-ME000000052");
                    var nombreMedico = datosMedico == null ? "" : datosMedico.Value2;
                    var firmaMedico = datosMedico == null ? null : datosMedico.Value5_;
                    //var firmaMedico = "";// datosMedico.Value5_;
                    var cpmMedico = datosMedico == null ? "" : datosMedico.Value3;
                    //'" + nombreMedico + "' as  NombreMedico, '" + firmaMedico + "' as RubricaMedico, '" + cpmMedico + "' as MedicoNroCmp, 
                    var consulta = @"exec getrecetatoreport_sp '" + serviceId + "', '"+v_DiagnosticRepositoryId+"'";

                    var data = cnx.Query<recetadespachoDto>(consulta).ToList();
                    List<recetadespachoDto> recetadespachoDtoList = new List<recetadespachoDto>();
                    foreach (var item in data)
                    {
                        //var prod = medicamentos.FirstOrDefault(p => p.IdProductoDetalle.Equals(item.MedicinaId));
                        //if (prod == null) continue;
                        //item.Medicamento = prod.NombreCompleto;
                        //item.Presentacion = prod.Presentacion;
                        //item.Ubicacion = prod.Ubicacion;
                        //item.NombreMedico = nombreMedico;
                        //item.RubricaMedico = firmaMedico;
                        //item.MedicoNroCmp = cpmMedico;
                        recetadespachoDto _recetadespachoDto = new recetadespachoDto();
                        _recetadespachoDto.RecetaId = item.RecetaId;
                        _recetadespachoDto.CantidadRecetada = item.CantidadRecetada;
                        _recetadespachoDto.NombrePaciente = item.NombrePaciente;
                        _recetadespachoDto.FechaFin = item.FechaFin;
                        _recetadespachoDto.Duracion = item.Duracion;
                        _recetadespachoDto.Dosis = item.Dosis;
                        _recetadespachoDto.NombreMedico = nombreMedico;
                        _recetadespachoDto.RubricaMedico = firmaMedico;
                        _recetadespachoDto.MedicoNroCmp = cpmMedico;
                        _recetadespachoDto.NombreClinica = item.NombreClinica;
                        _recetadespachoDto.DireccionClinica = item.DireccionClinica;
                        _recetadespachoDto.LogoClinica = item.LogoClinica;
                        _recetadespachoDto.Despacho = item.Despacho;
                        _recetadespachoDto.MedicinaId = item.MedicinaId;
                        _recetadespachoDto.FechaNacimiento = item.FechaNacimiento;
                        _recetadespachoDto.USUARIO = item.USUARIO;
                        _recetadespachoDto.ATENCION = item.ATENCION;
                        _recetadespachoDto.ESPECIALIDAD = item.ESPECIALIDAD;
                        _recetadespachoDto.CAMA = item.CAMA;
                        _recetadespachoDto.Dx = item.Dx;
                        _recetadespachoDto.Cie10 = item.Cie10;
                        _recetadespachoDto.FechaAtencion = item.FechaAtencion;
                        _recetadespachoDto.Edad = GetAge(item.FechaNacimiento.Date);

                        var prod = medicamentos.FirstOrDefault(p => p.IdProductoDetalle.Equals(item.MedicinaId));
                        if (prod == null) continue;
                        _recetadespachoDto.Medicamento = prod.NombreCompleto;
                        _recetadespachoDto.Presentacion = prod.Presentacion;
                        _recetadespachoDto.Ubicacion = prod.Ubicacion;
                        recetadespachoDtoList.Add(_recetadespachoDto);
                    }

                    return recetadespachoDtoList;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private KeyValueDTO ObtenerFirmaMedicoExamen(string pstrServiceId, string p1, string p2)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                var consulta = "EXEC getobtenerfirmamedicoexamen_sp '" + pstrServiceId + "', '" + p1 + "', '" + p2 + "'";

                var data = cnx.Query<KeyValueDTO>(consulta).FirstOrDefault();
                return data;
            }
        }

        public static List<MedicamentoDto> ObtenerContasolMedicamentos()
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewContasolConnection)
                {
                    if (cnx.State != ConnectionState.Open) cnx.Open();

                    const string query =
                        "select \"v_IdProductoDetalle\" as 'IdProductoDetalle', \"v_CodInterno\" as 'CodInterno', " +
                        "\"v_Descripcion\" as 'Nombre', " +
                        "\"v_Presentacion\" as 'Presentacion', \"v_Concentracion\" as 'Concentracion', " +
                        "\"v_Ubicacion\" as 'Ubicacion'" +
                        "from producto p " +
                        "join productodetalle pd on p.\"v_IdProducto\" = pd.\"v_IdProducto\" " +
                        "where pd.\"i_Eliminado\" = 0";

                    return cnx.Query<MedicamentoDto>(query).ToList();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        public string Report (){
            return "";
        }
        public List<EsoDto>  LlenarComboGennero(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcombogennero_sp()
                                      select new EsoDto
                                      {
                                          EsoId = a.EsoId,
                                          Nombre = a.Nombre
                                      }).ToList();

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return query;


                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 0;



                //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                //{
                //    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                //    //                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
                //    //								where i_GroupId = 100 and i_IsDeleted = 0";
                //    var query = "EXEC [LlenarComboGennero_SP]";

                //    var data = cnx.Query<EsoDto>(query).ToList();
                //    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                //    cbo.DataSource = data;
                //    cbo.DisplayMember = "Nombre";
                //    cbo.ValueMember = "EsoId";
                //    cbo.SelectedIndex = 0;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboGrupo(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcombogrupo_sp()
                                      select new EsoDto
                                      {
                                          EsoId = a.EsoId,
                                          Nombre = a.Nombre
                                      }).ToList();

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 0;

//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

////                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
////								where i_GroupId = 154 and i_IsDeleted = 0";
//                    var query = "[LlenarComboGrupo_SP]";
//                    var data = cnx.Query<EsoDto>(query).ToList();
//                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

//                    cbo.DataSource = data;
//                    cbo.DisplayMember = "Nombre";
//                    cbo.ValueMember = "EsoId";
//                    cbo.SelectedIndex = 0;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboFactor(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcombofactor_sp()
                                      select new EsoDto
                                      {
                                          EsoId = a.EsoId,
                                          Nombre = a.Nombre
                                      }).ToList();

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboEtnia(ComboBox cbo)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
								where i_GroupId = 401 and i_IsDeleted = 0 and v_Value1 != '' ";

                    var data = cnx.Query<EsoDto>(query).ToList();
                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                    //cbo.DataSource = data;
                    //cbo.DisplayMember = "Nombre";
                    //cbo.ValueMember = "EsoId";
                    //cbo.SelectedIndex = -1;
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarModalidadTrabajo(ComboBox cbo)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
								where i_GroupId = 407 and i_IsDeleted = 0 and v_Value1 != '' ";

                    var data = cnx.Query<EsoDto>(query).ToList();
                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                    //cbo.DataSource = data;
                    //cbo.DisplayMember = "Nombre";
                    //cbo.ValueMember = "EsoId";
                    //cbo.SelectedIndex = -1;
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboMarketing(ComboBox cbo)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
								where i_GroupId = 413 and i_IsDeleted = 0 and v_Value1 != '' ";

                    var data = cnx.Query<EsoDto>(query).ToList();
                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                    //cbo.DataSource = data;
                    //cbo.DisplayMember = "Nombre";
                    //cbo.ValueMember = "EsoId";
                    //cbo.SelectedIndex = -1;
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<EsoDto> LlenarComboResidencia(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcomboresidencia_sp()
                                      select new EsoDto
                                      {
                                          EsoId = a.EsoId,
                                          Nombre = a.Nombre
                                      }).ToList();

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 0;

//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

////                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
////								where i_GroupId = 111 and i_IsDeleted = 0";

//                    var query = "EXEC [LlenarComboResidencia_SP]";

//                    var data = cnx.Query<EsoDto>(query).ToList();
//                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

//                    cbo.DataSource = data;
//                    cbo.DisplayMember = "Nombre";
//                    cbo.ValueMember = "EsoId";
//                    cbo.SelectedIndex = 0;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboAltitud(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcomboaltitud_sp()
                                      select new EsoDto
                                      {
                                          EsoId = a.EsoId,
                                          Nombre = a.Nombre
                                      }).ToList();

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 0;

//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

////                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
////								where i_GroupId = 208 and i_IsDeleted = 0";
//                    var query = "EXEC [LlenarComboAltitud_SP]";
//                    var data = cnx.Query<EsoDto>(query).ToList();
//                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

//                    cbo.DataSource = data;
//                    cbo.DisplayMember = "Nombre";
//                    cbo.ValueMember = "EsoId";
//                    cbo.SelectedIndex = 0;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboTipoSeguro(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcombotiposeguro_sp()
                                      select new EsoDto
                                      {
                                          EsoId = a.EsoId,
                                          Nombre = a.Nombre
                                      }).ToList();

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 0;

//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

////                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
////								where i_GroupId = 188 and i_IsDeleted = 0";
//                    var query = "EXEC [LlenarComboTipoSeguro_SP]";

//                    var data = cnx.Query<EsoDto>(query).ToList();
//                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

//                    cbo.DataSource = data;
//                    cbo.DisplayMember = "Nombre";
//                    cbo.ValueMember = "EsoId";
//                    cbo.SelectedIndex = 0;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboParentesco(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcomboparentesco_sp()
                                      select new EsoDto
                                      {
                                          EsoId = a.EsoId,
                                          Nombre = a.Nombre
                                      }).ToList();

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 0;

//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

////                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
////								where i_GroupId = 207 and i_IsDeleted = 0";

//                    var query = "EXEC [LlenarComboParentesco_SP]";
//                    var data = cnx.Query<EsoDto>(query).ToList();
//                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

//                    cbo.DataSource = data;
//                    cbo.DisplayMember = "Nombre";
//                    cbo.ValueMember = "EsoId";
//                    cbo.SelectedIndex = -1;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboPacHospSala(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                //List<EsoDto> query = (from a in dbContext.llenarcomboparentesco_sp()
                //                      select new EsoDto
                //                      {
                //                          EsoId = a.EsoId,
                //                          Nombre = a.Nombre
                //                      }).ToList();

                //query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                //return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 0;

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    //                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
                    //								where i_GroupId = 207 and i_IsDeleted = 0";

                    var query = "EXEC [LlenarComboPacHospSala_SP]";
                    var data = cnx.Query<EsoDto>(query).ToList();
                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                    return data;
                    //cbo.DataSource = data;
                    //cbo.DisplayMember = "Nombre";
                    //cbo.ValueMember = "EsoId";
                    //cbo.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboLugarLabor(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcombolugarlabor_sp()
                                      select new EsoDto
                                      {
                                          EsoId = a.EsoId,
                                          Nombre = a.Nombre
                                      }).ToList();

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 0;

//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

////                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
////								where i_GroupId = 204 and i_IsDeleted = 0";
//                    var query = "EXEC [LlenarComboLugarLabor_SP]";

//                    var data = cnx.Query<EsoDto>(query).ToList();
//                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

//                    cbo.DataSource = data;
//                    cbo.DisplayMember = "Nombre";
//                    cbo.ValueMember = "EsoId";
//                    cbo.SelectedIndex = 0;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LlenarProvincia(List<KeyValueDTO> lista, ComboBox cbo)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    
                    lista.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                    cbo.DataSource = lista;
                    cbo.DisplayMember = "Value1";
                    cbo.ValueMember = "Id";
                    cbo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboEstadoCivil(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcomboestadocivil_sp()
                                      select new EsoDto
                                      {
                                          EsoId = a.EsoId,
                                          Nombre = a.Nombre
                                      }).ToList();

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 0;

//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

////                    var query = @"select i_ParameterId as 'EsoId', v_Value1 as 'Nombre' from systemparameter
////								where i_GroupId = 101 and i_IsDeleted = 0";
//                    var query = "EXEC [LlenarComboEstadoCivil_SP]";

//                    var data = cnx.Query<EsoDto>(query).ToList();
//                    data.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

//                    cbo.DataSource = data;
//                    cbo.DisplayMember = "Nombre";
//                    cbo.ValueMember = "EsoId";
//                    cbo.SelectedIndex = 0;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboTipoServicio(ComboBox cbo)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcombotiposervicio_sp()
                                      select new EsoDto
                                      {
                                          EsoId = a.EsoId,
                                          Nombre = a.Nombre
                                      }).ToList();

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "EsoId";
                //cbo.SelectedIndex = 1;
//                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

////                    var query = @"select DISTINCT b.i_ParameterId as 'EsoId', b.v_Value1 as 'Nombre' 
////                            from nodeserviceprofile a
////                            inner join systemparameter b on (a.i_ServiceTypeId = b.i_ParameterId) and (119 = b.i_GroupId)
////							where b.i_IsDeleted = 0 and a.i_NodeId = 9";
//                    var query = "EXEC [LlenarComboTipoServicio_SP]";

//                    var data = cnx.Query<EsoDto>(query).ToList();
//                    data.Insert(0, new EsoDto {EsoId = -1, Nombre = "--Seleccionar--"});

//                    cbo.DataSource = data;
//                    cbo.DisplayMember = "Nombre";
//                    cbo.ValueMember = "EsoId";
//                    cbo.SelectedIndex = 1;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDto> LlenarComboServicio(ComboBox cbo, int? pServiceTypeId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"select DISTINCT b.i_ParameterId as 'EsoId', b.v_Value1 as 'Nombre' 
                            from nodeserviceprofile a
                            inner join systemparameter b on (a.i_MasterServiceId = b.i_ParameterId) and (119 = b.i_GroupId)
							where a.i_ServiceTypeId = " + pServiceTypeId + " and b.i_IsDeleted = 0 and a.i_NodeId = 9";

                    var data = cnx.Query<EsoDto>(query).ToList();
                    data.Insert(0, new EsoDto {EsoId = -1, Nombre = "--Seleccionar--"});

                    return data;
                    //cbo.DataSource = data;
                    //cbo.DisplayMember = "Nombre";
                    //cbo.ValueMember = "EsoId";
                    //cbo.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDtoProt> LlenarComboProtocolo(ComboBox cbo, int? pServiceTypeId, int? pService)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDtoProt> query = (from a in dbContext.llenarcomboprotocolo_sp(pServiceTypeId, pService)
                                          select new EsoDtoProt
                                          {
                                              Nombre = a.Nombre.Split('/')[0].Trim(),
                                              Id = a.Id,
                                              Consultorio = a.Nombre.Split('/')[1].Trim() == null ? "-" : a.Nombre.Split('/')[1].Trim()
                                          }).ToList();

                query.Insert(0, new EsoDtoProt { Id = "-1", Nombre = "--Seleccionar--", Consultorio = "- - -" });

                return query;
                //cbo.DataSource = query;
                //cbo.DisplayMember = "Nombre";
                //cbo.ValueMember = "Id";
                //cbo.SelectedIndex = 0;

                //                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                //                {
                //                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                ////                    var query = @"SELECT v_ProtocolId AS Id, v_Name AS Nombre
                ////                            FROM Protocol
                ////                            WHERE i_MasterServiceTypeId =" + pServiceTypeId + "and i_IsDeleted = 0 and i_MasterServiceId =" + pService;

                //                    var query = "EXEC [LlenarComboProtocolo_SP] " + pServiceTypeId + " , " + pService;

                //                    var data = cnx.Query<EsoDto>(query).ToList();
                //                    data.Insert(0, new EsoDto {Id = "-1", Nombre = "--Seleccionar--"});

                //                    cbo.DataSource = data;
                //                    cbo.DisplayMember = "Nombre";
                //                    cbo.ValueMember = "Id";
                //                    cbo.SelectedIndex = 0;
                //                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EsoDtoProt> LlenarComboProtocoloFiltrado(ComboBox cbo, int? pServiceTypeId, int? pService, string consultorio)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDtoProt> query = (from a in dbContext.llenarcomboprotocolo_sp(pServiceTypeId, pService)
                                          select new EsoDtoProt
                                          {
                                              Nombre = a.Nombre.Split('/')[0].Trim(),
                                              Id = a.Id,
                                              Consultorio = a.Nombre.Split('/')[1].Trim() == null ? "-" : a.Nombre.Split('/')[0].Trim()
                                          }).ToList();
                //query = query.FindAll(p => p.Consultorio == consultorio && p.Consultorio == "- - -");


                query.Insert(0, new EsoDtoProt { Id = "-1", Nombre = "--Seleccionar--", Consultorio = "- - -" });


                return query;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public static ProtocolDto GetDatosProtocolo(string pProtocoloId)
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                ProtocolDto query = (from a in dbContext.getdatosprotocolo_sp(pProtocoloId)
                                           select new ProtocolDto
                                                     {
                                                         Geso = a.Geso,
                                                         TipoEso = a.TipoEso,
                                                         i_EsoTypeId = Convert.ToInt32(a.i_EsoTypeId),
                                                         EmpresaCliente = a.EmpresaCliente,
                                                         EmpresaEmpleadora = a.EmpresaEmpleadora,
                                                         EmpresaTrabajo = a.EmpresaTrabajo,
                                                         v_EmployerOrganizationId = a.v_EmployerOrganizationId,
                                                         v_EmployerLocationId = a.v_EmployerLocationId,
                                                         v_GroupOccupationId = a.v_GroupOccupationId
                                                     }).FirstOrDefault();

                return query;

                //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
//                {
//                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

//                    var query = @"SELECT b.v_Name AS Geso, c.v_Value1 AS TipoEso,
//                    a.i_EsoTypeId, 
//                    a.v_CustomerOrganizationId  + '|' + a.v_CustomerLocationId AS EmpresaCliente, 
//                    a.v_EmployerOrganizationId + '|' + a.v_EmployerLocationId  AS EmpresaEmpleadora, 
//                    a.v_WorkingOrganizationId + '|' + a.v_WorkingLocationId AS EmpresaTrabajo,
//                    a.v_EmployerOrganizationId,a.v_EmployerLocationId,a.v_GroupOccupationId
//                                FROM Protocol a
//                                INNER JOIN groupoccupation b ON a.v_GroupOccupationId = b.v_GroupOccupationId
//                                INNER JOIN systemparameter c ON a.i_EsoTypeId = c.i_ParameterId and c.i_GroupId = 118
//                                INNER JOIN organization d ON a.v_CustomerOrganizationId = d.v_OrganizationId
//                                INNER JOIN organization e ON a.v_EmployerOrganizationId = e.v_OrganizationId
//                                INNER JOIN organization f ON a.v_WorkingOrganizationId = f.v_OrganizationId
//                                WHERE a.v_ProtocolId = '" + pProtocoloId + "'";

//                    var data = cnx.Query<ProtocolDto>(query).FirstOrDefault();
//                    return data;
//                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static ServiceComponentUpdatePrice GetServiceComponentEditPrecio(string _ComponentId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = @"SELECT sc.v_ServiceComponentId, sc.v_ServiceId, sc.r_Price, CASE WHEN sc.i_Cantidad IS NULL THEN 1 ELSE sc.i_Cantidad END AS Cantidad, CASE WHEN sc.d_Descuento IS NULL THEN 0 ELSE sc.d_Descuento END AS Descuento, CASE WHEN sc.d_SaldoPaciente IS NULL THEN 0 ELSE sc.d_SaldoPaciente END AS SaldoPaciente, CASE WHEN sc.d_SaldoAseguradora IS NULL THEN 0 ELSE sc.d_SaldoAseguradora END AS SaldoSeguro, CASE WHEN s.i_PlanId IS NULL THEN 0 ELSE s.i_PlanId END AS PlanSeguro, CASE WHEN pl.d_Importe IS NULL THEN 0 ELSE pl.d_Importe END AS Deducible, CASE WHEN pl.d_ImporteCo IS NULL THEN 0 ELSE pl.d_ImporteCo END AS Coaseguro
                                FROM servicecomponent sc
                                JOIN service s on sc.v_ServiceId = s.v_ServiceId
                                LEFT JOIN dbo.[plan] pl on s.i_PlanId = pl.i_PlanId
                                WHERE sc.v_ServiceComponentId = '" + _ComponentId + "'";

                    var data = cnx.Query<ServiceComponentUpdatePrice>(query).FirstOrDefault();
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string AddPerson(PersonDto personDto)
        {
            try
            {
                var secuentialId = GetNextSecuentialId(8).SecuentialId;
                var newId = GetNewId(9, secuentialId, "PP");

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {

                    var searchPerson = "select * from person where v_DocNumber = '" + personDto.NroDocumento + "'";
                    var firstOrDefault = cnx.Query<PersonDto>(searchPerson).FirstOrDefault();

                    if (firstOrDefault != null)
                    {
                        return "El paciente ya se encuentra registrado";
                    }

                    var query =
                        "INSERT INTO person (v_PersonId,v_FirstName,v_FirstLastName,v_SecondLastName,i_DocTypeId,v_DocNumber,i_SexTypeId,d_Birthdate,i_IsDeleted,i_MaritalStatusId,v_BirthPlace,i_DistrictId,i_ProvinceId,i_DepartmentId,i_ResidenceInWorkplaceId,v_Mail,v_AdressLocation,v_CurrentOccupation,i_AltitudeWorkId,v_ExploitedMineral,i_LevelOfId,i_BloodGroupId,i_BloodFactorId,v_ResidenceTimeInWorkplace,i_TypeOfInsuranceId,i_NumberLivingChildren,i_NumberDependentChildren,i_NroHermanos,v_TelephoneNumber,i_Relationship,i_PlaceWorkId,v_Religion,v_Nacionalidad,v_ResidenciaAnterior, v_OwnerName, v_ContactName, v_EmergencyPhone, i_EtniaRaza, i_Migrante, v_Migrante) " +
                        "VALUES ('" + newId + "' , '" + personDto.Nombres + "', '" + personDto.ApellidoPaterno + "', '" +
                        personDto.ApellidoMaterno + "', '" + personDto.TipoDocumento + "', '" + personDto.NroDocumento +
                        "', '" + personDto.GeneroId + "',  CONVERT(datetime,'" +
                        personDto.FechaNacimiento.ToShortDateString() + "',103), 0 ,'" + personDto.EstadoCivil + "', '" + personDto.LugarNacimiento + "', '" + personDto.Distrito + "'  , '" + personDto.Provincia + "' , '" + personDto.Departamento + "', '" + personDto.Reside + "', '" + personDto.Email + "', '" + personDto.Direccion + "', '" + personDto.Puesto + "', '" + personDto.Altitud + "', '" + personDto.Minerales + "', '" + personDto.Estudios + "', '" + personDto.Grupo + "', '" + personDto.Factor + "', '" + personDto.TiempoResidencia + "', '" + personDto.TipoSeguro + "', '" + personDto.Vivos + "', '" + personDto.Muertos + "', '" + personDto.Hermanos + "', '" + personDto.Telefono + "', '" + personDto.Parantesco + "', '" + personDto.Labor + "' , '" + personDto.Religion + "', '" + personDto.Nacionalidad + "', '" + personDto.ResidenciaAnte + "', '" + personDto.titular + "', '" + personDto.ContactoEmergencia + "', '" + personDto.CelularEmergencia + "' ," + personDto.i_EtniaRaza + ", " + personDto.i_Migrante + " , '" + personDto.v_Migrante + "' "
                     + " )";
                    cnx.Execute(query);

                    var query2 = "INSERT INTO pacient (v_PersonId, i_IsDeleted) VALUES ('" + newId + "', 0) ";
                    cnx.Execute(query2);


                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = (SqlConnection)ConnectionHelper.GetNewSigesoftConnection;

                    SqlCommand com = new SqlCommand("UPDATE person SET b_PersonImage = @PersonImage, b_FingerPrintTemplate = @FingerPrintTemplate, b_FingerPrintImage = @FingerPrintImage, b_RubricImage = @RubricImage, t_RubricImageText = @RubricImageText  WHERE v_PersonId = '" + newId
                        
                        + "'", cmd.Connection);
                    if (personDto.b_PersonImage != null)
                    {
                        com.Parameters.AddWithValue("@PersonImage", personDto.b_PersonImage);
                    }
                    else
                    {
                        SqlParameter imageParameter = new SqlParameter("@PersonImage", SqlDbType.Image);
                        imageParameter.Value = DBNull.Value;
                        com.Parameters.Add(imageParameter);
                    }

                    if (personDto.b_FingerPrintTemplate != null)
                    {
                        com.Parameters.AddWithValue("@FingerPrintTemplate", personDto.b_FingerPrintTemplate);
                    }
                    else
                    {
                        SqlParameter imageParameter = new SqlParameter("@FingerPrintTemplate", SqlDbType.Image);
                        imageParameter.Value = DBNull.Value;
                        com.Parameters.Add(imageParameter);
                    }

                    if (personDto.b_FingerPrintImage != null)
                    {
                        com.Parameters.AddWithValue("@FingerPrintImage", personDto.b_FingerPrintImage);
                    }
                    else
                    {
                        SqlParameter imageParameter = new SqlParameter("@FingerPrintImage", SqlDbType.Image);
                        imageParameter.Value = DBNull.Value;
                        com.Parameters.Add(imageParameter);
                    }

                    if (personDto.b_RubricImage != null)
                    {
                        com.Parameters.AddWithValue("@RubricImage", personDto.b_RubricImage);
                    }
                    else
                    {
                        SqlParameter imageParameter = new SqlParameter("@RubricImage", SqlDbType.Image);
                        imageParameter.Value = DBNull.Value;
                        com.Parameters.Add(imageParameter);
                    }

                    if (personDto.t_RubricImageText != null)
                    {
                        com.Parameters.AddWithValue("@RubricImageText", personDto.t_RubricImageText);
                    }
                    else
                    {
                        SqlParameter imageParameter = new SqlParameter("@RubricImageText", SqlDbType.Text);
                        imageParameter.Value = DBNull.Value;
                        com.Parameters.Add(imageParameter);
                    }
                    cmd.Connection.Open();
                    com.ExecuteNonQuery();

                    #region Creacion de habitos nocivos

                    List<noxioushabitsDto> _noxioushabitsDto = new List<noxioushabitsDto>();
                    noxioushabitsDto noxioushabitsDto = new noxioushabitsDto();
                    noxioushabitsDto.v_Frequency = "NO";
                    noxioushabitsDto.v_Comment = "";
                    noxioushabitsDto.v_PersonId = newId;
                    noxioushabitsDto.i_TypeHabitsId = 1;
                    _noxioushabitsDto.Add(noxioushabitsDto);

                    noxioushabitsDto = new noxioushabitsDto();
                    noxioushabitsDto.v_Frequency = "NO";
                    noxioushabitsDto.v_Comment = "";
                    noxioushabitsDto.v_PersonId = newId;
                    noxioushabitsDto.i_TypeHabitsId = 2;
                    _noxioushabitsDto.Add(noxioushabitsDto);

                    noxioushabitsDto = new noxioushabitsDto();
                    noxioushabitsDto.v_Frequency = "NO";
                    noxioushabitsDto.v_Comment = "";
                    noxioushabitsDto.v_PersonId = newId;
                    noxioushabitsDto.i_TypeHabitsId = 3;
                    _noxioushabitsDto.Add(noxioushabitsDto);


                    AddNoxiousHabits(_noxioushabitsDto);

                    #endregion

                    #region Creación de Médicos Familiares
                    List<familymedicalantecedentsDto> _familymedicalantecedentsDto = new List<familymedicalantecedentsDto>();
                    familymedicalantecedentsDto familymedicalantecedentsDto = new familymedicalantecedentsDto();

                    //Padre
                    familymedicalantecedentsDto.v_PersonId = newId;
                    familymedicalantecedentsDto.v_DiseasesId = "N009-DD000000649";
                    familymedicalantecedentsDto.i_TypeFamilyId = 53;
                    familymedicalantecedentsDto.v_Comment = "";
                    _familymedicalantecedentsDto.Add(familymedicalantecedentsDto);

                    //Madre
                    familymedicalantecedentsDto = new familymedicalantecedentsDto();
                    familymedicalantecedentsDto.v_PersonId = newId;
                    familymedicalantecedentsDto.v_DiseasesId = "N009-DD000000649";
                    familymedicalantecedentsDto.i_TypeFamilyId = 41;
                    familymedicalantecedentsDto.v_Comment = "";
                    _familymedicalantecedentsDto.Add(familymedicalantecedentsDto);


                    //Hermanos
                    familymedicalantecedentsDto = new familymedicalantecedentsDto();
                    familymedicalantecedentsDto.v_PersonId = newId;
                    familymedicalantecedentsDto.v_DiseasesId = "N009-DD000000649";
                    familymedicalantecedentsDto.i_TypeFamilyId = 32;
                    familymedicalantecedentsDto.v_Comment = "";
                    _familymedicalantecedentsDto.Add(familymedicalantecedentsDto);


                    //Esposos
                    familymedicalantecedentsDto = new familymedicalantecedentsDto();
                    familymedicalantecedentsDto.v_PersonId = newId;
                    familymedicalantecedentsDto.v_DiseasesId = "N009-DD000000649";
                    familymedicalantecedentsDto.i_TypeFamilyId = 19;
                    familymedicalantecedentsDto.v_Comment = "";
                    _familymedicalantecedentsDto.Add(familymedicalantecedentsDto);


                    //Hijos
                    familymedicalantecedentsDto = new familymedicalantecedentsDto();
                    familymedicalantecedentsDto.v_PersonId = newId;
                    familymedicalantecedentsDto.v_DiseasesId = "N009-DD000000649";
                    familymedicalantecedentsDto.i_TypeFamilyId = 67;
                    familymedicalantecedentsDto.v_Comment = "";
                    _familymedicalantecedentsDto.Add(familymedicalantecedentsDto);


                    AddFamilyMedicalAntecedents( _familymedicalantecedentsDto);


                    #endregion

                    return newId;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static string AddUpdateVaacunaciones(InmunizacionesDto inmunizaciones)
        {
            string id = "";
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var searchInmunizacion = "select * from inmunizaciones where v_PersonId = '" + inmunizaciones.v_PersonId + "' and i_Dosis = " + inmunizaciones.i_Dosis + " and i_IsDeleted = 0";
                    var firstOrDefault = cnx.Query<InmunizacionesDto>(searchInmunizacion).FirstOrDefault();
                    
                    if (firstOrDefault == null)
                    {
                        if (inmunizaciones.i_Marca != -1)
                        {
                            var secuentialId = GetNextSecuentialId(358).SecuentialId;
                            var newId = GetNewId(9, secuentialId, "IN");
                            id = newId;

                            var usuariosigesoft = Usuariosigesoft(inmunizaciones.i_InsertUserId);

                            var query =
                                "INSERT INTO inmunizaciones (v_VacunacionId, v_PersonId, i_TipoVacuna, i_Marca, v_Lote, d_FechaVacuna, i_IsDeleted, i_InsertUserId, d_InsertDate, i_Dosis, v_Lugar) " +
                                "VALUES ('" + newId + "' , '" + inmunizaciones.v_PersonId + "', " +
                                inmunizaciones.i_TipoVacuna + ", " +
                                inmunizaciones.i_Marca + ", '" + inmunizaciones.v_Lote + "',  CONVERT(datetime,'" +
                                inmunizaciones.d_FechaVacuna.ToShortDateString() + "',103), " +
                                inmunizaciones.i_IsDeleted + " , " + usuariosigesoft + " , GETDATE() , " +
                                inmunizaciones.i_Dosis + "  , '" + inmunizaciones.v_Lugar + "' "
                                + " )";

                            cnx.Execute(query);
                        }
                    }
                    else
                    {
                        var usuariosigesoft = Usuariosigesoft(inmunizaciones.i_UpdateUserId);

                        if (inmunizaciones.i_Marca == -1)
                        {
                            var query =
                                "update inmunizaciones set  i_IsDeleted = 1 , i_UpdateUserId = " +
                                usuariosigesoft + " , d_UpdateDate = GETDATE()  where  v_VacunacionId = '" +
                                firstOrDefault.v_VacunacionId + "' ";
                            cnx.Execute(query);
                        }
                        else
                        {
                            var query =
                                "update inmunizaciones set  " +
                                " i_Marca = "+inmunizaciones.i_Marca+" , " +
                                " v_Lote = '"+inmunizaciones.v_Lote+"', "+
                                " d_FechaVacuna = '"+inmunizaciones.d_FechaVacuna+"', "+
                                " v_Lugar = '"+inmunizaciones.v_Lugar + "', "+
                                " i_UpdateUserId = " +
                                usuariosigesoft + " , d_UpdateDate = GETDATE()  where  v_VacunacionId = '"+firstOrDefault.v_VacunacionId + "' ";
                            cnx.Execute(query);  
                        } 
                           
                        id = inmunizaciones.v_VacunacionId;
                    }



                    return id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static List<InmunizacionesDto> InmunizacionesListadas(string v_personId)
        {
            string id = "";
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var searchInmunizacion = "select DISTINCT * from inmunizaciones where v_PersonId = '" + v_personId + "' and i_IsDeleted = 0";
                    var listaInmunizaciones = cnx.Query<InmunizacionesDto>(searchInmunizacion).ToList();
                    
                    return listaInmunizaciones;

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        private static void AddFamilyMedicalAntecedents(List<familymedicalantecedentsDto> familymedicalantecedentsDto)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                foreach (var item in familymedicalantecedentsDto)
                {
                    var secuentialId = GetNextSecuentialId(42).SecuentialId;
                    var newId = GetNewId(9, secuentialId, "FA");

                    var query2 = "INSERT INTO familymedicalantecedents (v_FamilyMedicalAntecedentsId, v_PersonId, v_DiseasesId, i_TypeFamilyId, v_Comment,i_IsDeleted) "+
                        " VALUES ('" + newId + "','"+ item.v_PersonId +"', '"+ item.v_DiseasesId+"',"+ item.i_TypeFamilyId+", '"+ item.v_Comment+"',0 ) ";
                    cnx.Execute(query2);
                }
            }
        }

        private static void AddNoxiousHabits(List<noxioushabitsDto> noxioushabitsDto)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                foreach (var item in noxioushabitsDto)
                {

                    var secuentialId = GetNextSecuentialId(41).SecuentialId;
                    var newId = GetNewId(9, secuentialId, "NX");

                    var query2 =
                        "INSERT INTO noxioushabits (v_NoxiousHabitsId, v_PersonId, i_TypeHabitsId, v_Frequency, v_Comment, v_DescriptionHabit,v_DescriptionQuantity,i_IsDeleted) " +
                        " VALUES ('" + newId + "', '" + item.v_PersonId +"', "+ item.i_TypeHabitsId +", '"+ item.v_Frequency +"', '" + item.v_Comment +"', '"+ item.v_DescriptionHabit +"', '"+ item.v_DescriptionQuantity+"', 0 ) ";
                    cnx.Execute(query2);
                }
            }
        }

        public static string UpdatePerson(PersonDto personDto, string personId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = "UPDATE person SET" +
                    " v_FirstName = " + "'" + personDto.Nombres + "'"  +
                    ", v_FirstLastName = " + "'" + personDto.ApellidoPaterno + "'" +
                    ", v_SecondLastName = " + "'" + personDto.ApellidoMaterno + "'" + 
                    ", i_DocTypeId = " + personDto.TipoDocumento +
                    ", v_DocNumber =" + "'" + personDto.NroDocumento + "'" + 
                    ", i_SexTypeId =" + personDto.GeneroId + 
                    ", i_MaritalStatusId = " + personDto.EstadoCivil +
                    ", v_BirthPlace = " + "'" + personDto.LugarNacimiento + "'" +
                    ", d_Birthdate = CONVERT(datetime, '" + personDto.FechaNacimiento.ToShortDateString() + "',103)" +
                    ", i_DistrictId = " + personDto.Distrito +
                    ", i_ProvinceId = " + personDto.Provincia +
                    ", i_DepartmentId = " + personDto.Departamento + 
                    ", i_ResidenceInWorkplaceId = " + personDto.Reside + 
                    ", v_Mail = " + "'" + personDto.Email + "'" + 
                    ", v_AdressLocation = " + "'" + personDto.Direccion + "'" +
                    ", v_CurrentOccupation = " + "'" + personDto.Puesto + "'" +
                    ", i_AltitudeWorkId = " + personDto.Altitud +
                    ", v_ExploitedMineral = " + "'" + personDto.Minerales + "'" +
                    ", i_LevelOfId = " + personDto.Estudios +
                    ", i_BloodGroupId = " + personDto.Grupo +
                    ", i_BloodFactorId = " + personDto.Factor +
                        ", v_ResidenceTimeInWorkplace = " + "'" + personDto.TiempoResidencia + "'" +
                    ", i_TypeOfInsuranceId = " + personDto.TipoSeguro +
                    ", i_NumberLivingChildren = " + personDto.Vivos +
                    ", i_NumberDependentChildren = " + personDto.Muertos +
                    ", i_NroHermanos = " + personDto.Hermanos +
                    ", v_TelephoneNumber = " + "'" + personDto.Telefono + "'" +
                    ", i_Relationship = " + personDto.Parantesco +
                    ", i_PlaceWorkId = " + personDto.Labor +
                    ", v_Nacionalidad = " + "'" + personDto.Nacionalidad + "'" +
                    ", v_ResidenciaAnterior = " + "'" + personDto.ResidenciaAnte + "'" +
                    ", v_Religion = " + "'" + personDto.Religion + "'" +
                    ", v_OwnerName = " + "'" + personDto.titular + "'" +
                    ", v_ContactName = " + "'" + personDto.ContactoEmergencia + "'" +
                    ", v_EmergencyPhone = " + "'" + personDto.CelularEmergencia + "'" +
                    ",v_ComentaryUpdate = " + "'" + personDto.CommentaryUpdate + "'" +
                    ", v_Deducible = 0.00  " +
                    ", i_EtniaRaza = " + personDto.i_EtniaRaza +
                    ", i_Migrante = " + personDto.i_Migrante +
                    ", v_Migrante = " + "'" + personDto.v_Migrante + "'" +
                    " WHERE v_PersonId = '" + personId + "'";
                cnx.Execute(query);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection) ConnectionHelper.GetNewSigesoftConnection;
                
                SqlCommand com = new SqlCommand("UPDATE person SET b_PersonImage = @PersonImage, b_FingerPrintTemplate = @FingerPrintTemplate, b_FingerPrintImage = @FingerPrintImage, b_RubricImage = @RubricImage, t_RubricImageText = @RubricImageText  WHERE v_PersonId = '" + personId + "'", cmd.Connection);
                if (personDto.b_PersonImage != null)
                {
                    com.Parameters.AddWithValue("@PersonImage", personDto.b_PersonImage);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@PersonImage", SqlDbType.Image);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }

                if (personDto.b_FingerPrintTemplate != null)
                {
                    com.Parameters.AddWithValue("@FingerPrintTemplate", personDto.b_FingerPrintTemplate);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@FingerPrintTemplate", SqlDbType.Image);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }

                if (personDto.b_FingerPrintImage != null)
                {
                    com.Parameters.AddWithValue("@FingerPrintImage", personDto.b_FingerPrintImage);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@FingerPrintImage", SqlDbType.Image);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }

                if (personDto.b_RubricImage != null)
                {
                    com.Parameters.AddWithValue("@RubricImage", personDto.b_RubricImage);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@RubricImage", SqlDbType.Image);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }
              
                if (personDto.t_RubricImageText != null)
                {
                    com.Parameters.AddWithValue("@RubricImageText", personDto.t_RubricImageText);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@RubricImageText", SqlDbType.Text);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }

                cmd.Connection.Open();
                //cmd.CommandTimeout = 10000;
                com.ExecuteNonQuery();

                cmd.Connection.Close();
                com.Connection.Close();
                return personId;

              
            } 
        }

        public static string UpdatePerson_Asist(PersonDto personDto, string personId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = "UPDATE person SET" +
                    " v_FirstName = " + "'" + personDto.Nombres + "'" +
                    ", v_FirstLastName = " + "'" + personDto.ApellidoPaterno + "'" +
                    ", v_SecondLastName = " + "'" + personDto.ApellidoMaterno + "'" +
                    ", i_DocTypeId = " + personDto.TipoDocumento +
                    ", v_DocNumber =" + "'" + personDto.NroDocumento + "'" +
                    ", i_SexTypeId =" + personDto.GeneroId +
                    ", i_MaritalStatusId = " + personDto.EstadoCivil +
                    ", v_BirthPlace = " + "'" + personDto.LugarNacimiento + "'" +
                    ", d_Birthdate = CONVERT(datetime, '" + personDto.FechaNacimiento.ToShortDateString() + "',103)" +
                    ", i_DistrictId = " + personDto.Distrito +
                    ", i_ProvinceId = " + personDto.Provincia +
                    ", i_DepartmentId = " + personDto.Departamento +
                    ", i_ResidenceInWorkplaceId = " + personDto.Reside +
                    ", v_Mail = " + "'" + personDto.Email + "'" +
                    ", v_AdressLocation = " + "'" + personDto.Direccion + "'" +
                    ", v_CurrentOccupation = " + "'" + personDto.Puesto + "'" +
                    ", i_LevelOfId = " + personDto.Estudios +
                    ", i_BloodGroupId = " + personDto.Grupo +
                    ", i_BloodFactorId = " + personDto.Factor +
                    ", v_TelephoneNumber = " + "'" + personDto.Telefono + "'" +
                    ", i_Relationship = " + personDto.Parantesco +
                    ", v_Nacionalidad = " + "'" + personDto.Nacionalidad + "'" +
                    ", v_ResidenciaAnterior = " + "'" + personDto.ResidenciaAnte + "'" +
                    ", v_Religion = " + "'" + personDto.Religion + "'" +
                    ", v_OwnerName = " + "'" + personDto.titular + "'" +
                    ", v_ContactName = " + "'" + personDto.ContactoEmergencia + "'" +
                    ", v_EmergencyPhone = " + "'" + personDto.CelularEmergencia + "'" +
                    ",v_ComentaryUpdate = " + "'" + personDto.CommentaryUpdate + "'" +
                    ", v_Deducible = 0.00 " +
                    ", i_TypeOfInsuranceId = " + personDto.TipoSeguro +
                    ", i_EtniaRaza = " + personDto.i_EtniaRaza +
                    ", i_Migrante = " + personDto.i_Migrante +
                    ", v_Migrante = " + "'" + personDto.v_Migrante + "'" +
                    " WHERE v_PersonId = '" + personId + "'";
                cnx.Execute(query);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)ConnectionHelper.GetNewSigesoftConnection;

                SqlCommand com = new SqlCommand("UPDATE person SET b_PersonImage = @PersonImage, b_FingerPrintTemplate = @FingerPrintTemplate, b_FingerPrintImage = @FingerPrintImage, b_RubricImage = @RubricImage, t_RubricImageText = @RubricImageText  WHERE v_PersonId = '" + personId + "'", cmd.Connection);
                if (personDto.b_PersonImage != null)
                {
                    com.Parameters.AddWithValue("@PersonImage", personDto.b_PersonImage);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@PersonImage", SqlDbType.Image);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }

                if (personDto.b_FingerPrintTemplate != null)
                {
                    com.Parameters.AddWithValue("@FingerPrintTemplate", personDto.b_FingerPrintTemplate);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@FingerPrintTemplate", SqlDbType.Image);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }

                if (personDto.b_FingerPrintImage != null)
                {
                    com.Parameters.AddWithValue("@FingerPrintImage", personDto.b_FingerPrintImage);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@FingerPrintImage", SqlDbType.Image);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }

                if (personDto.b_RubricImage != null)
                {
                    com.Parameters.AddWithValue("@RubricImage", personDto.b_RubricImage);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@RubricImage", SqlDbType.Image);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }

                if (personDto.t_RubricImageText != null)
                {
                    com.Parameters.AddWithValue("@RubricImageText", personDto.t_RubricImageText);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@RubricImageText", SqlDbType.Text);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }

                cmd.Connection.Open();
                com.ExecuteNonQuery();
                return personId;


            }
        }

        public static string UpdateService(ServiceDto serviceDto, string serviceId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = "UPDATE service SET" +
                    " v_OrganizationId = " + "'" + serviceDto.OrganizationId + "', v_ComentaryUpdate = '" + serviceDto.CommentaryUpdate + "' " +
                    " WHERE v_ServiceId = '" + serviceId + "'";
                cnx.Execute(query);

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)ConnectionHelper.GetNewSigesoftConnection;

                //SqlCommand com = new SqlCommand("UPDATE service SET b_PersonImage = @PersonImage, b_FingerPrintTemplate = @FingerPrintTemplate, b_FingerPrintImage = @FingerPrintImage, b_RubricImage = @RubricImage, t_RubricImageText = @RubricImageText  WHERE v_PersonId = '" + personId + "'", cmd.Connection);
               
                cmd.Connection.Open();
                //cmd.ExecuteNonQuery();
                return serviceId;


            }
        }

        public static string UpdateCalendar(string serviceId, int estado, int eliminadoService)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = "UPDATE calendar SET i_CalendarStatusId = " + estado + ", i_LineStatusId = " + (int)LineStatus.EnCircuito + " WHERE v_ServiceId = '" + serviceId + "'";
                               
                cnx.Execute(query);

                var query2 = "UPDATE service SET i_IsDeleted = " + eliminadoService + " WHERE v_ServiceId = '" + serviceId + "'";
                cnx.Execute(query2);

                if (eliminadoService == 0)
                {
                    var queryDigital = "update DigitalContactCenter set i_EstadoAtencion = '3' where v_ServiceId = '" + serviceId + "'";
                    cnx.Execute(queryDigital);
                }

                return serviceId;


            }
        }

        public static bool CancelarCita(string calendarId, int userId, string serviceId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    //var query = "UPDATE calendar SET i_CalendarStatusId = " + estado + " WHERE v_ServiceId = '" + serviceId + "'";
                    var query = "update calendar set " +
                                          " i_LineStatusId = " + (int)LineStatus.FueraCircuito +
                                          ", i_CalendarStatusId = " + (int)CalendarStatus.Cancelado +
                                          ", i_UpdateUserId = " + userId +
                                          ", d_UpdateDate = '" + DateTime.Now + "'" +
                                          "where v_CalendarId = '" + calendarId + "'";

                    cnx.Execute(query);

                    //var query2 = "UPDATE service SET i_IsDeleted = " + eliminadoService + " WHERE v_ServiceId = '" + serviceId + "'";
                    var query2 = "update service set " +
                                          " i_IsDeleted = 1" +
                                          ", i_UpdateUserId = " + userId +
                                          ", d_UpdateDate = '" + DateTime.Now + "'" +
                                          "where v_ServiceId = '" + serviceId + "'";
                    cnx.Execute(query2);

                    var queryDigital = "update DigitalContactCenter set i_EstadoAtencion = '5' where v_ServiceId = '" + serviceId + "'";
                    cnx.Execute(queryDigital);

                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string UpdateComprobanteNew(int facturado, string comprobante, string liquidacion, string serviceId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query2 = "UPDATE service SET i_IsFac = " + facturado + ", v_ComprobantePago = " + comprobante + " , v_NroLiquidacion = "+ liquidacion+"  WHERE v_ServiceId = '" + serviceId + "'";
                cnx.Execute(query2);

                return serviceId;

            }
        }

        public static string UpdateObservacionServicio(string detalle, string serviceId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query2 = "UPDATE service SET v_ObservacionesAdicionales = '" + detalle + "' WHERE v_ServiceId = '" + serviceId + "'";
                cnx.Execute(query2);

                return serviceId;

            }
        }
        //public Image byteArrayToImage(byte[] byteArrayIn)
        //{
        //    try
        //    {
        //        MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
        //        ms.Write(byteArrayIn, 0, byteArrayIn.Length);
        //        returnImage = Image.FromStream(ms, true);//Exception occurs here
        //    }
        //    catch { }
        //    return returnImage;
        //}

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        public static string ByteArrayToString_(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static Secuential GetNextSecuentialId(int tableId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query =
                    "update secuential set i_SecuentialId = (select i_SecuentialId from secuential where i_NodeId = 9 and  i_TableId =" +
                    tableId + " ) + 1 where i_NodeId = 9 and  i_TableId = " + tableId +
                    " select i_NodeId as NodeId ,i_TableId as TableId ,i_SecuentialId as SecuentialId from secuential where i_NodeId = 9 and  i_TableId =" +
                    tableId;
                return cnx.Query<Secuential>(query).FirstOrDefault();
            }
        }

        public static SystemUser GetSystemUser(string userId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query =
                    "select * from systemuser where v_PersonId = '" + userId + "' and i_SystemUserTypeId = 5 "; 
                return cnx.Query<SystemUser>(query).FirstOrDefault();
            }
        }
        public static SystemUser UltimoUsuario()
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query =
                    "select top 1 * from systemuser order by i_SystemUserId desc ";
                return cnx.Query<SystemUser>(query).FirstOrDefault();
            }
        }
        public static void InsertSystemUser(SystemUser newusuario)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var secuentialId = UltimoUsuario();

                int ultimoId = secuentialId.i_SystemUserId + 1 ;

                var query2 = "INSERT INTO systemuser (i_SystemUserId, v_PersonId, v_UserName, v_Password, i_IsDeleted, i_InsertUserId, d_InsertDate, i_SystemUserTypeId, i_RolVentaId) " +
                    " VALUES (" + ultimoId + ", '" + newusuario.v_PersonId + "', '" + newusuario.v_UserName + "', '" + newusuario.v_Password + "', '" + newusuario.i_IsDeleted + "', '" + newusuario.i_InsertUserId + "', '" + newusuario.d_InsertDate + "', '" + newusuario.i_SystemUserTypeId + "', '-1') ";
                cnx.Execute(query2);

                var actualizarSecuencial = @"update secuential set i_SecuentialId = " + ultimoId + " where i_NodeId = 1 and i_TableId = 9";
                cnx.Execute(actualizarSecuencial);
            }
        }

        public static string GetNewId(int pintNodeId, int pintSequential, string pstrPrefix)
        {
            return string.Format("N{0:000}-{1}{2:000000000}", pintNodeId, pstrPrefix, pintSequential);
        }

        public static int ObtenerTipoEmpresaByProtocol(string protocolId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                var query = @"SELECT i_OrganizationTypeId
                                FROM Protocol a         
                                INNER JOIN  organization  o ON a.v_EmployerOrganizationId = o.v_OrganizationId          
                                WHERE a.v_ProtocolId = '" + protocolId + "'";

                var data = cnx.Query<ProtocolDto>(query).FirstOrDefault();
                return data.i_OrganizationTypeId;
            }
        }

        public static BindingList<ventadetalleDto> SheduleServiceAtx(ServiceDto oServiceDto, int usuarioGraba, int atenciondia, int medicoPagado)
        {
            try
            {
                var result = new BindingList<ventadetalleDto>();
                var secuentialId = GetNextSecuentialId(23).SecuentialId;
                var serviceId = GetNewId(9, secuentialId, "SR");
                oServiceDto.ServiceId = serviceId;
                var tipoEmpresa = ObtenerTipoEmpresaByProtocol(oServiceDto.ProtocolId);

                using (var ts = new TransactionScope())
                {
                    using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                    {
                        string plan = "null";
                        if (oServiceDto.Plan.ToString() != "-1" && oServiceDto.Plan.ToString() != "")
                        {
                            plan = oServiceDto.Plan.ToString();
                        }
                        //if (atenciondia == 1)
                        //{
                        var query =
                        "INSERT INTO [dbo].[service]([v_ServiceId],[v_ProtocolId],[v_PersonId],[i_MasterServiceId],[i_ServiceStatusId],[i_AptitudeStatusId],[d_ServiceDate],[d_GlobalExpirationDate],[d_ObsExpirationDate],[v_OrganizationId],[i_FlagAgentId],[v_Motive],[i_IsFac],[i_StatusLiquidation],i_IsFacMedico,i_IsDeleted,i_InsertUserId,d_InsertDate,v_centrocosto,i_MedicoPagado,i_PlanId, v_LicenciaConducir, v_ObservacionesAdicionales, i_ProcedenciaPac_Mkt, i_MedicoSolicitanteExterno, i_Establecimiento, i_VendedorExterno, i_MedicoAtencion, i_CodigoAtencion, i_GrupoAtencion)" +
                        "VALUES ('" + serviceId + "','" + oServiceDto.ProtocolId + "','" + oServiceDto.PersonId + "'," +
                        oServiceDto.MasterServiceId + "," + oServiceDto.ServiceStatusId + "," +
                        oServiceDto.AptitudeStatusId + ",GETDATE(),NULL,NULL,'" + oServiceDto.OrganizationId + "',1,'',1,1,0,0,11,GETDATE(),'" + oServiceDto.v_centrocosto + "', " + medicoPagado + ", " + plan + ", '" + oServiceDto.v_LicenciaConducir + "', '" + oServiceDto.ObservacionesAtencion + "', '" + oServiceDto.i_ProcedenciaPac_Mkt + "', '" + oServiceDto.MedicoSolicitanteExterno + "', '" + oServiceDto.Establecimiento + "', '" + oServiceDto.VendedorExterno + "', " + oServiceDto.i_MedicoAtencion + ", " + oServiceDto.i_CodigoAtencion + ", " + oServiceDto.i_GrupoAtencion + ")";
                        cnx.Execute(query);
                        //}
                        //else
                        //{
                        //var query =
                        //"INSERT INTO [dbo].[service]([v_ServiceId],[v_ProtocolId],[v_PersonId],[i_MasterServiceId],[i_ServiceStatusId],[i_AptitudeStatusId],[d_ServiceDate],[d_GlobalExpirationDate],[d_ObsExpirationDate],[v_OrganizationId],[i_FlagAgentId],[v_Motive],[i_IsFac],[i_StatusLiquidation],i_IsFacMedico,i_IsDeleted,i_InsertUserId,d_InsertDate,v_centrocosto,i_MedicoPagado,i_PlanId, v_LicenciaConducir, v_ObservacionesAdicionales , i_ProcedenciaPac_Mkt, i_MedicoSolicitanteExterno, i_Establecimiento, i_VendedorExterno, i_MedicoAtencion, i_CodigoAtencion, i_GrupoAtencion)" +
                        //"VALUES ('" + serviceId + "','" + oServiceDto.ProtocolId + "','" + oServiceDto.PersonId + "'," +
                        //oServiceDto.MasterServiceId + "," + oServiceDto.ServiceStatusId + "," +
                        //oServiceDto.AptitudeStatusId + ", '" + oServiceDto.ServiceDate.ToString() + "',NULL,NULL,'" + oServiceDto.OrganizationId + "',1,'',1,1,0,0,11,GETDATE(),'" + oServiceDto.v_centrocosto + "', " + medicoPagado + ", " + plan + ", '" + oServiceDto.v_LicenciaConducir + "', '" + oServiceDto.ObservacionesAtencion + "', '" + oServiceDto.i_ProcedenciaPac_Mkt + "', '" + oServiceDto.MedicoSolicitanteExterno + "', '" + oServiceDto.Establecimiento + "', '" + oServiceDto.VendedorExterno + "', " + oServiceDto.i_MedicoAtencion + ", " + oServiceDto.i_CodigoAtencion + ", " + oServiceDto.i_GrupoAtencion + ")";
                        //cnx.Execute(query);
                        //}

                        if (oServiceDto._idccEditarNew != "")
                        {
                            var querDigital = "update DigitalContactCenter set i_EstadoAtencion = '2', v_ServiceId = '" + serviceId + "' where v_DigitalContactCenterId = '" + oServiceDto._idccEditarNew + "'";
                            cnx.Execute(querDigital);
                        }


                        var qProtocolComponents =
                            "select pc.v_ComponentId AS ComponentId, c.v_Name AS ComponentName, sp1.v_Field AS Porcentajes, pc.v_ProtocolComponentId AS ProtocolComponentId, pc.r_Price AS Price, sp2.v_Value1 AS Operator, pc.i_Age AS Age, sp3.v_Value1 AS Gender, pc.i_IsConditionalIMC AS IsConditionalImc, pc.r_Imc AS Imc, pc.i_IsConditionalId AS IsConditional, pc.i_IsAdditional AS IsAdditional,  sp4.v_Value1 AS ComponentTypeName, pc.i_GenderId AS GenderId, pc.i_GrupoEtarioId AS GrupoEtarioId, pc.i_IsConditionalId AS IsConditionalId, pc.i_OperatorId AS OperatorId, c.i_CategoryId AS CategoryId,c.i_ComponentTypeId AS ComponentTypeId, i_UIIsVisibleId AS UiIsVisibleId,i_UIIndex AS UiIndex,v_IdUnidadProductiva AS IdUnidadProductiva " +
                            "from protocolcomponent pc " +
                            "inner join component c on pc.v_ComponentId = c.v_ComponentId " +
                            "left join systemparameter sp1 on c.i_CategoryId = sp1.i_ParameterId and sp1.i_GroupId = 116 " +
                            "left join systemparameter sp2 on pc.i_OperatorId = sp2.i_ParameterId and sp2.i_GroupId = 117 " +
                            "left join systemparameter sp3 on pc.i_GenderId = sp3.i_ParameterId and sp3.i_GroupId = 130 " +
                            "left join systemparameter sp4 on c.i_ComponentTypeId= sp4.i_ParameterId and sp4.i_GroupId = 126 " +
                            "where pc.i_IsDeleted = 0 and c.i_IsDeleted =  0 and pc.v_ProtocolId ='" + oServiceDto.ProtocolId + "'";
                        var components = cnx.Query<ProtocolComponentList>(qProtocolComponents).ToList();

                        var oServiceComponentDto = new ServiceComponentDto();
                        foreach (var t in components)
                        {
                            var componentId = t.ComponentId;
                            oServiceComponentDto.ComponentName = t.ComponentName;
                            oServiceComponentDto.i_MedicoTratanteId = oServiceDto.MedicoTratanteId;
                            oServiceComponentDto.i_MedicoRealizaId = oServiceDto.MedicoRealizaId;
                            oServiceComponentDto.ServiceId = serviceId;
                            oServiceComponentDto.ExternalInternalId = (int)ComponenteProcedencia.Interno;
                            oServiceComponentDto.ServiceComponentTypeId = t.ComponentTypeId;
                            oServiceComponentDto.IsVisibleId = t.UiIsVisibleId;
                            oServiceComponentDto.IsInheritedId = (int)SiNo.No;
                            oServiceComponentDto.StartDate = null;
                            if (medicoPagado == 1)
                            {
                                oServiceComponentDto.i_PayMedic = 1;
                            }
                            else
                            {
                                oServiceComponentDto.i_PayMedic = 0;
                            }
                            oServiceComponentDto.EndDate = null;
                            oServiceComponentDto.Index = t.UiIndex;
                            var porcentajes = t.Porcentajes.Split('-');
                            float p1 = porcentajes[0] == null || porcentajes[0] == "" ? 0 : float.Parse(porcentajes[0]);
                            float p2 = porcentajes[1] == null || porcentajes[1] == "" ? 0 : float.Parse(porcentajes[1]);
                            var pb = t.Price.Value;
                            oServiceComponentDto.Price = pb + (pb * p1 / 100) + (pb * p2 / 100);
                            oServiceComponentDto.ComponentId = t.ComponentId;
                            oServiceComponentDto.IsInvoicedId = (int)SiNo.No;
                            oServiceComponentDto.ServiceComponentStatusId = (int)ServiceStatus.PorIniciar;
                            oServiceComponentDto.QueueStatusId = (int)QueueStatusId.Libre;
                            oServiceComponentDto.Iscalling = (int)FlagCall.NoseLlamo;
                            oServiceComponentDto.Iscalling1 = (int)FlagCall.NoseLlamo;
                            oServiceComponentDto.IdUnidadProductiva = t.IdUnidadProductiva;
                            oServiceComponentDto.i_Cantidad = 1;
                            oServiceComponentDto.d_Descuento = 0;
                            var Plan = "select * from [plan]  where v_ProtocoloId = '" + oServiceDto.ProtocolId + "' and v_IdUnidadProductiva = '" + t.IdUnidadProductiva + "'";
                            var resultplan = cnx.Query<PlanDto>(Plan).ToList();
                            var tienePlan = false;
                            if (resultplan.Count > 0) tienePlan = true;
                            else tienePlan = false;
                            if (tienePlan)
                            {
                                if (resultplan[0].i_EsCoaseguro == 1)
                                {
                                    oServiceComponentDto.d_SaldoPaciente = resultplan[0].d_Importe * decimal.Parse(oServiceComponentDto.Price.ToString()) / 100;
                                    oServiceComponentDto.d_SaldoAseguradora = decimal.Parse(oServiceComponentDto.Price.ToString()) - oServiceComponentDto.d_SaldoPaciente;
                                }
                                if (resultplan[0].i_EsDeducible == 1)
                                {
                                    oServiceComponentDto.d_SaldoPaciente = resultplan[0].d_Importe;
                                    oServiceComponentDto.d_SaldoAseguradora = decimal.Parse(oServiceComponentDto.Price.ToString()) - resultplan[0].d_Importe;
                                    
                                }
                            }

                            //Condicionales
                            var conditional = t.IsConditionalId;
                            if (conditional == (int)SiNo.Si)
                            {
                                var fechaNacimiento = oServiceDto.FechaNacimiento;
                                //Datos del paciente

                                if (fechaNacimiento != null)
                                {
                                    var pacientAge = DateTime.Today.AddTicks(-fechaNacimiento.Value.Ticks).Year - 1;

                                    var pacientGender = Convert.ToInt32(oServiceDto.GeneroId);

                                    //Datos del protocolo
                                    int analyzeAge = t.Age;
                                    int analyzeGender = t.GenderId;
                                    var @operator = (Operator2Values)t.OperatorId;
                                    GrupoEtario oGrupoEtario = (GrupoEtario)t.GrupoEtarioId;
                                    if ((int)@operator == -1)
                                    {
                                        //si la condicional del operador queda en --Seleccionar--
                                        if (analyzeGender == (int)GenderConditional.AMBOS)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else if (pacientGender == analyzeGender)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.No;
                                        }
                                    }
                                    else
                                    {
                                        if (analyzeGender == (int)GenderConditional.MASCULINO)
                                        {
                                            oServiceComponentDto.IsRequiredId = SwitchOperator2Values(pacientAge, analyzeAge,
                                                @operator, pacientGender, analyzeGender);
                                        }
                                        else if (analyzeGender == (int)GenderConditional.FEMENINO)
                                        {
                                            oServiceComponentDto.IsRequiredId = SwitchOperator2Values(pacientAge, analyzeAge,
                                                @operator, pacientGender, analyzeGender);
                                        }
                                        else if (analyzeGender == (int)GenderConditional.AMBOS)
                                        {
                                            oServiceComponentDto.IsRequiredId = SwitchOperator2Values(pacientAge, analyzeAge,
                                                @operator, pacientGender, analyzeGender);
                                        }
                                    }
                                    if (componentId == "N009-ME000000402") //Adolecente
                                    {
                                        if ((int)oGrupoEtario == -1)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else if (13 <= pacientAge && pacientAge <= 18)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.No;
                                        }

                                    }
                                    else if (componentId == "N009-ME000000403") //Adulto
                                    {
                                        if ((int)oGrupoEtario == -1)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else if (19 <= pacientAge && pacientAge <= 60)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.No;
                                        }
                                    }
                                    else if (componentId == "N009-ME000000404") //AdultoMayor
                                    {
                                        if ((int)oGrupoEtario == -1)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else if (61 <= pacientAge)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.No;
                                        }
                                    }
                                    else if (componentId == "N009-ME000000406")
                                    {
                                        if ((int)oGrupoEtario == -1)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else if (12 >= pacientAge)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.No;
                                        }
                                    }
                                    else if (componentId == "N009-ME000000401") //plan integral
                                    {
                                        if ((int)oGrupoEtario == -1)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else if (12 >= pacientAge)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.No;
                                        }
                                    }
                                    else if (componentId == "N009-ME000000400") //atencion integral
                                    {
                                        if ((int)oGrupoEtario == -1)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else if (12 >= pacientAge)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.No;
                                        }
                                    }
                                    else if (componentId == "N009-ME000000405") //consulta
                                    {
                                        if ((int)oGrupoEtario == -1)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else if (12 >= pacientAge)
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                        }
                                        else
                                        {
                                            oServiceComponentDto.IsRequiredId = (int)SiNo.No;
                                        }
                                    }
                                    else
                                    {
                                        oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                    }
                                }
                            }
                            else
                            {
                                oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                if (t.IsAdditional == null) continue;
                                var adicional = t.IsAdditional;
                                if (adicional == 1)
                                {
                                    oServiceComponentDto.IsRequiredId = (int)SiNo.No;
                                }
                            }
                            oServiceComponentDto.IsManuallyAddedId = (int)SiNo.No;
                            oServiceComponentDto.i_ConCargoA = 0;
                            AddServiceComponent(oServiceComponentDto);

                        }
                        AddCalendar(oServiceDto, usuarioGraba, atenciondia);

                        if (oServiceDto.MasterServiceId == 19 || oServiceDto.MasterServiceId == 13 || oServiceDto.MasterServiceId == 29 || oServiceDto.MasterServiceId == 30 || ((oServiceDto.MasterServiceId == 10 || oServiceDto.MasterServiceId == 15 || oServiceDto.MasterServiceId == 16 || oServiceDto.MasterServiceId == 17 || oServiceDto.MasterServiceId == 18 || oServiceDto.MasterServiceId == 19) && tipoEmpresa == 4))
                        {
                            AddHospitalizacion(oServiceDto.PersonId, serviceId, oServiceDto.PacienteHospSala, oServiceDto.PasoSop, oServiceDto.PasoHosp);
                        }

                        ts.Complete();
                    }
                }
               

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static BindingList<ventadetalleDto> SheduleService(ServiceDto oServiceDto, int usuarioGraba)
        {
            try
            {
                var result = new BindingList<ventadetalleDto>();
                var secuentialId = GetNextSecuentialId(23).SecuentialId;
                var serviceId = GetNewId(9, secuentialId, "SR");
                oServiceDto.ServiceId = serviceId;
                var tipoEmpresa = ObtenerTipoEmpresaByProtocol(oServiceDto.ProtocolId);
                using (var ts = new TransactionScope())
                {

                    using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                    {
                        var query =
                            "INSERT INTO [dbo].[service]([v_ServiceId],[v_ProtocolId],[v_PersonId],[i_MasterServiceId],[i_ServiceStatusId],[i_AptitudeStatusId],[d_ServiceDate],[d_GlobalExpirationDate],[d_ObsExpirationDate],[v_OrganizationId],[i_FlagAgentId],[v_Motive],[i_IsFac],[i_StatusLiquidation],i_IsFacMedico,i_IsDeleted,i_InsertUserId,d_InsertDate,v_centrocosto,i_MedicoPagado,v_Area, v_ObservacionesAdicionales, i_ModTrabajo, i_ProcedenciaPac_Mkt )" +
                            "VALUES ('" + serviceId + "','" + oServiceDto.ProtocolId + "','" + oServiceDto.PersonId + "'," +
                            oServiceDto.MasterServiceId + "," + oServiceDto.ServiceStatusId + "," +
                            oServiceDto.AptitudeStatusId + ",GETDATE(),NULL,NULL,'" + oServiceDto.OrganizationId + "',1,'',1,1,0,0,11,GETDATE(),'" + oServiceDto.v_centrocosto + "',0,'" + oServiceDto.Area + "', '" + oServiceDto.ObservacionesAtencion + "', " + oServiceDto.i_ModTrabajo + ", " + oServiceDto.i_ProcedenciaPac_Mkt + ")";
                        cnx.Execute(query);

                        if (oServiceDto._idccEditarNew != "")
                        {
                            var querDigital = "update DigitalContactCenter set i_EstadoAtencion = '2', v_ServiceId = '" + serviceId + "' where v_DigitalContactCenterId = '" + oServiceDto._idccEditarNew + "'";
                        }

                        var qProtocolComponents =
                            "select pc.v_ComponentId AS ComponentId, c.v_Name AS ComponentName, sp1.v_Field AS Porcentajes, pc.v_ProtocolComponentId AS ProtocolComponentId, pc.r_Price AS Price, sp2.v_Value1 AS Operator, pc.i_Age AS Age, sp3.v_Value1 AS Gender, pc.i_IsConditionalIMC AS IsConditionalImc, pc.r_Imc AS Imc, pc.i_IsConditionalId AS IsConditional, pc.i_IsAdditional AS IsAdditional,  sp4.v_Value1 AS ComponentTypeName, pc.i_GenderId AS GenderId, pc.i_GrupoEtarioId AS GrupoEtarioId, pc.i_IsConditionalId AS IsConditionalId, pc.i_OperatorId AS OperatorId, c.i_CategoryId AS CategoryId,c.i_ComponentTypeId AS ComponentTypeId, i_UIIsVisibleId AS UiIsVisibleId,i_UIIndex AS UiIndex,v_IdUnidadProductiva AS IdUnidadProductiva " +
                            "from protocolcomponent pc " +
                            "inner join component c on pc.v_ComponentId = c.v_ComponentId " +
                            "left join systemparameter sp1 on c.i_CategoryId = sp1.i_ParameterId and sp1.i_GroupId = 116 " +
                            "left join systemparameter sp2 on pc.i_OperatorId = sp2.i_ParameterId and sp2.i_GroupId = 117 " +
                            "left join systemparameter sp3 on pc.i_GenderId = sp3.i_ParameterId and sp3.i_GroupId = 130 " +
                            "left join systemparameter sp4 on c.i_ComponentTypeId= sp4.i_ParameterId and sp4.i_GroupId = 126 " +
                            "where c.i_IsDeleted = 0 and pc.i_IsDeleted = 0 and pc.v_ProtocolId ='" + oServiceDto.ProtocolId + "'";
                        var components = cnx.Query<ProtocolComponentList>(qProtocolComponents).ToList();

                        var oServiceComponentDto = new ServiceComponentDto();
                        foreach (var t in components)
                        {
                            var componentId = t.ComponentId;
                            oServiceComponentDto.ComponentName = t.ComponentName;
                            oServiceComponentDto.i_MedicoTratanteId = oServiceDto.MedicoTratanteId;
                            oServiceComponentDto.ServiceId = serviceId;
                            oServiceComponentDto.ExternalInternalId = (int)ComponenteProcedencia.Interno;
                            oServiceComponentDto.ServiceComponentTypeId = t.ComponentTypeId;
                            oServiceComponentDto.IsVisibleId = t.UiIsVisibleId;
                            oServiceComponentDto.IsInheritedId = (int)SiNo.No;
                            oServiceComponentDto.StartDate = null;
                            oServiceComponentDto.EndDate = null;
                            oServiceComponentDto.Index = t.UiIndex;
                            var porcentajes = t.Porcentajes.Split('-');
                            float p1 = porcentajes[0] == null || porcentajes[0] == "" ? 0 : float.Parse(porcentajes[0]);
                            float p2 = porcentajes[1] == null || porcentajes[1] == "" ? 0 : float.Parse(porcentajes[1]);

                            var pb = t.Price.Value;
                            oServiceComponentDto.Price = pb + (pb * p1 / 100) + (pb * p2 / 100);

                            oServiceComponentDto.ComponentId = t.ComponentId;
                            oServiceComponentDto.IsInvoicedId = (int)SiNo.No;
                            oServiceComponentDto.ServiceComponentStatusId = (int)ServiceStatus.PorIniciar;
                            oServiceComponentDto.QueueStatusId = (int)QueueStatusId.Libre;
                            oServiceComponentDto.Iscalling = (int)FlagCall.NoseLlamo;
                            oServiceComponentDto.Iscalling1 = (int)FlagCall.NoseLlamo;
                            oServiceComponentDto.IdUnidadProductiva = t.IdUnidadProductiva;
                            oServiceComponentDto.i_Cantidad = 1;
                            oServiceComponentDto.d_Descuento = 0;


                            var Plan = "select * from [plan]  where v_ProtocoloId = '" + oServiceDto.ProtocolId + "' and v_IdUnidadProductiva = '" + t.IdUnidadProductiva + "'";
                            var resultplan = cnx.Query<PlanDto>(Plan).ToList();
                            var tienePlan = false;
                            if (resultplan.Count > 0) tienePlan = true;
                            else tienePlan = false;


                            if (tienePlan)
                            {
                                if (resultplan[0].i_EsCoaseguro == 1)
                                {
                                    oServiceComponentDto.d_SaldoPaciente = resultplan[0].d_Importe * decimal.Parse(oServiceComponentDto.Price.ToString()) / 100;
                                    oServiceComponentDto.d_SaldoAseguradora = decimal.Parse(oServiceComponentDto.Price.ToString()) - oServiceComponentDto.d_SaldoPaciente;
                                }
                                if (resultplan[0].i_EsDeducible == 1)
                                {
                                    oServiceComponentDto.d_SaldoPaciente = resultplan[0].d_Importe;
                                    oServiceComponentDto.d_SaldoAseguradora = decimal.Parse(oServiceComponentDto.Price.ToString()) - resultplan[0].d_Importe;
                                    
                                }
                            }

                            //Condicionales
                            var conditional = t.IsConditionalId;
                            if (conditional == (int)SiNo.Si)
                            {
                                var fechaNacimiento = oServiceDto.FechaNacimiento;
                                //Datos del paciente

                                if (fechaNacimiento != null)
                                {
                                    var pacientAge = DateTime.Today.AddTicks(-fechaNacimiento.Value.Ticks).Year - 1;

                                    var pacientGender = oServiceDto.GeneroId;

                                    //Datos del protocolo
                                    int analyzeAge = t.Age;
                                    int analyzeGender = t.GenderId;
                                    var @operator = (Operator2Values)t.OperatorId;
                                    GrupoEtario oGrupoEtario = (GrupoEtario)t.GrupoEtarioId;
                                    if (analyzeAge >= 0)//condicional edad (SI)
                                    {
                                        if (analyzeGender != (int)GenderConditional.AMBOS)//condicional genero (SI)
                                        {
                                            if (@operator == Operator2Values.X_esIgualque_A)
                                            {
                                                if (pacientAge == analyzeAge && pacientGender == analyzeGender){oServiceComponentDto.IsRequiredId = (int)SiNo.Si;}
                                                else{oServiceComponentDto.IsRequiredId = (int)SiNo.No;}
                                            }
                                            if (@operator == Operator2Values.X_esMayorIgualque_A)
                                            {
                                                if (pacientAge >= analyzeAge && pacientGender == analyzeGender){oServiceComponentDto.IsRequiredId = (int)SiNo.Si;}
                                                else{oServiceComponentDto.IsRequiredId = (int)SiNo.No;}
                                            }
                                            if (@operator == Operator2Values.X_esMayorque_A)
                                            {
                                                if (pacientAge > analyzeAge && pacientGender == analyzeGender){oServiceComponentDto.IsRequiredId = (int)SiNo.Si;}
                                                else{oServiceComponentDto.IsRequiredId = (int)SiNo.No;}
                                            }
                                            if (@operator == Operator2Values.X_esMenorIgualque_A)
                                            {
                                                if (pacientAge <= analyzeAge && pacientGender == analyzeGender){oServiceComponentDto.IsRequiredId = (int)SiNo.Si;}
                                                else{oServiceComponentDto.IsRequiredId = (int)SiNo.No;}
                                            }
                                        }
                                        else//condicional genero (NO)
                                        {
                                            if (@operator == Operator2Values.X_esIgualque_A)
                                            {
                                                if (pacientAge == analyzeAge){oServiceComponentDto.IsRequiredId = (int)SiNo.Si;}
                                                else{oServiceComponentDto.IsRequiredId = (int)SiNo.No;}
                                            }
                                            if (@operator == Operator2Values.X_esMayorIgualque_A)
                                            {
                                                if (pacientAge >= analyzeAge){oServiceComponentDto.IsRequiredId = (int)SiNo.Si;}
                                                else{oServiceComponentDto.IsRequiredId = (int)SiNo.No;}
                                            }
                                            if (@operator == Operator2Values.X_esMayorque_A)
                                            {
                                                if (pacientAge > analyzeAge){oServiceComponentDto.IsRequiredId = (int)SiNo.Si;}
                                                else{oServiceComponentDto.IsRequiredId = (int)SiNo.No;}
                                            }
                                            if (@operator == Operator2Values.X_esMenorIgualque_A)
                                            {
                                                if (pacientAge <= analyzeAge){oServiceComponentDto.IsRequiredId = (int)SiNo.Si;}
                                                else{oServiceComponentDto.IsRequiredId = (int)SiNo.No;}
                                            }
                                        }
                                        
                                    }

                                    //if ((int)@operator == -1)
                                    //{

                                    //    //si la condicional del operador queda en --Seleccionar--
                                    //    if (analyzeGender == (int)GenderConditional.AMBOS)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                    //    }
                                    //    else if (pacientGender == analyzeGender)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                    //    }
                                    //    else
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int)SiNo.No;
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    if (analyzeGender == pacientGender)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = SwitchOperator2Values(pacientAge, analyzeAge,
                                    //            @operator, pacientGender, analyzeGender);
                                    //    }
                                    //    else if (analyzeGender == pacientGender)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = SwitchOperator2Values(pacientAge, analyzeAge,
                                    //            @operator, pacientGender, analyzeGender);
                                    //    }
                                    //    else if (analyzeGender == pacientGender)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = SwitchOperator2Values(pacientAge, analyzeAge,
                                    //            @operator, pacientGender, analyzeGender);
                                    //    }
                                    //}

                                    #region ...




                                    //if (componentId == "N009-ME000000402") //Adolecente
                                    //{
                                    //    if ((int) oGrupoEtario == -1)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else if (13 <= pacientAge && pacientAge <= 18)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.No;
                                    //    }

                                    //}
                                    //else if (componentId == "N009-ME000000403") //Adulto
                                    //{
                                    //    if ((int) oGrupoEtario == -1)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else if (19 <= pacientAge && pacientAge <= 60)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.No;
                                    //    }
                                    //}
                                    //else if (componentId == "N009-ME000000404") //AdultoMayor
                                    //{
                                    //    if ((int) oGrupoEtario == -1)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else if (61 <= pacientAge)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.No;
                                    //    }
                                    //}
                                    //else if (componentId == "N009-ME000000406")
                                    //{
                                    //    if ((int) oGrupoEtario == -1)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else if (12 >= pacientAge)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.No;
                                    //    }
                                    //}
                                    //else if (componentId == "N009-ME000000401") //plan integral
                                    //{
                                    //    if ((int) oGrupoEtario == -1)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else if (12 >= pacientAge)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.No;
                                    //    }
                                    //}
                                    //else if (componentId == "N009-ME000000400") //atencion integral
                                    //{
                                    //    if ((int) oGrupoEtario == -1)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else if (12 >= pacientAge)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.No;
                                    //    }
                                    //}
                                    //else if (componentId == "N009-ME000000405") //consulta
                                    //{
                                    //    if ((int) oGrupoEtario == -1)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else if (12 >= pacientAge)
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                                    //    }
                                    //    else
                                    //    {
                                    //        oServiceComponentDto.IsRequiredId = (int) SiNo.No;
                                    //    }
                                    //}
                                    ////else
                                    ////{
                                    ////    oServiceComponentDto.IsRequiredId = (int) SiNo.No;
                                    ////}
                                    #endregion
                                }
                            }
                            else
                            {
                                oServiceComponentDto.IsRequiredId = (int)SiNo.Si;
                                if (t.IsAdditional == null) continue;
                                var adicional = t.IsAdditional;
                                if (adicional == 1)
                                {
                                    oServiceComponentDto.IsRequiredId = (int)SiNo.No;
                                }
                            }

                            oServiceComponentDto.i_ConCargoA = 0;
                            oServiceComponentDto.IsManuallyAddedId = (int)SiNo.No;
                            AddServiceComponent(oServiceComponentDto);

                        }
                        AddCalendar(oServiceDto, usuarioGraba, 1);

                        if (oServiceDto.MasterServiceId == 19 || ((oServiceDto.MasterServiceId == 10 || oServiceDto.MasterServiceId == 15 || oServiceDto.MasterServiceId == 16 || oServiceDto.MasterServiceId == 17 || oServiceDto.MasterServiceId == 18 || oServiceDto.MasterServiceId == 19) && tipoEmpresa == 4))
                        {
                            AddHospitalizacion(oServiceDto.PersonId, serviceId, oServiceDto.PacienteHospSala, oServiceDto.PasoSop, oServiceDto.PasoHosp);
                        }

                    }
                   
                    ts.Complete();

                }
                return result;
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void AddHospitalizacion(string personId, string serviceId, string PacHospSala, int PasoSop, int PasoHosp)
        {
            try
            {
                var secHospiId = GetNextSecuentialId(350).SecuentialId;
                var newHospiId = GetNewId(9, secHospiId, "HP");

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var queryHospi = "INSERT INTO [dbo].[hospitalizacion]([v_HopitalizacionId],[v_PersonId],[d_FechaIngreso],[i_IsDeleted], [v_ProcedenciaPac], [i_PasoSop], [i_PasoHosp])" +
                                "VALUES ('" + newHospiId + "', '" + personId + "', GETDATE(), 0, '" + PacHospSala + "', " + PasoSop + ", " + PasoHosp + ")";
                    cnx.Execute(queryHospi);


                    var secHospiServ = GetNextSecuentialId(351).SecuentialId;
                    var newHospiServId = GetNewId(9, secHospiServ, "HS");


                    var query = "INSERT INTO [dbo].[hospitalizacionservice]([v_HospitalizacionServiceId],[v_HopitalizacionId],[v_ServiceId],[i_IsDeleted])" +
                              "VALUES ('" + newHospiServId + "', '" + newHospiId + "', '" + serviceId + "', 0)";
                    cnx.Execute(query);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void AddCalendar(ServiceDto oServiceDto, int usuarioGraba, int atencionDia)
        {
            try
            {
                var secuentialId = GetNextSecuentialId(22).SecuentialId;
                var newId = GetNewId(9, secuentialId, "CA");
                var usuariosigesoft = Usuariosigesoft(usuarioGraba);
                if (atencionDia == 1)
                {
                  using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                    {
                        var query = "INSERT INTO [dbo].[calendar]([v_CalendarId],[v_PersonId],[v_ServiceId],[d_DateTimeCalendar],[d_CircuitStartDate],[d_EntryTimeCM],[i_ServiceTypeId],[i_CalendarStatusId],[i_ServiceId],[v_ProtocolId],[i_NewContinuationId],[i_LineStatusId],[i_IsVipId],[i_IsDeleted],i_InsertUserId, d_InsertDate )" +
                                    "VALUES ('" + newId + "', '" + oServiceDto.PersonId + "', '" + oServiceDto.ServiceId + "', GETDATE(),  GETDATE(), GETDATE()," + oServiceDto.MasterServiceId + ",1, " + oServiceDto.MasterServiceId + ", '" + oServiceDto.ProtocolId + "', 1, 1, 0,0," + usuariosigesoft + ", GETDATE())";
                       
                        cnx.Execute(query);

                    }  
                }
                else
                {
                    using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                    {
                        var query = "INSERT INTO [dbo].[calendar]([v_CalendarId],[v_PersonId],[v_ServiceId],[d_DateTimeCalendar],[i_ServiceTypeId],[i_CalendarStatusId],[i_ServiceId],[v_ProtocolId],[i_NewContinuationId],[i_LineStatusId],[i_IsVipId],[i_IsDeleted],i_InsertUserId, d_InsertDate)" +
                                    "VALUES ('" + newId + "', '" + oServiceDto.PersonId + "', '" + oServiceDto.ServiceId + "', '" + oServiceDto.ServiceDate.ToString() + "', " + oServiceDto.ServiceTypeId + ", 5, " + oServiceDto.MasterServiceId + ", '" + oServiceDto.ProtocolId + "', " + 1 + ", 2, 0,0," + usuariosigesoft + ", GETDATE())";

                        cnx.Execute(query);
                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static int Usuariosigesoft(int usuarioGraba)
        {
            int usuariosigesoft = 11;
            if (usuarioGraba == 9053)
            {
                usuariosigesoft = 337;
            }
            

            return usuariosigesoft;
        }

        private static void AddServiceComponent(ServiceComponentDto oServiceComponentDto)
        {
            try
            {
                var secuentialId = GetNextSecuentialId(24).SecuentialId;
                var newId = GetNewId(9, secuentialId, "SC");

                if (oServiceComponentDto.Price != null)
	            {
                    oServiceComponentDto.Price = SetNewPrice(oServiceComponentDto.Price, oServiceComponentDto.ComponentId);
	            }
                
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "INSERT INTO [dbo].[servicecomponent]([v_ServiceComponentId],[v_ServiceId],[i_ExternalInternalId],[i_ServiceComponentTypeId],[i_IsVisibleId],[i_IsInheritedId],[i_index],[r_Price],[v_ComponentId],[i_IsInvoicedId],[i_ServiceComponentStatusId],[i_QueueStatusId],[i_IsRequiredId],[i_Iscalling],[v_IdUnidadProductiva],[i_IsManuallyAddedId],[i_IsDeleted],[d_InsertDate],[i_InsertUserId],i_MedicoTratanteId,d_SaldoPaciente,d_SaldoAseguradora, i_ConCargoA, i_ApplicantMedic, i_PayMedic, i_Cantidad, d_Descuento )" +
                                "VALUES ('" + newId + "', '" + oServiceComponentDto.ServiceId + "', " + oServiceComponentDto.ExternalInternalId + ", " + oServiceComponentDto.ServiceComponentTypeId + ", " + oServiceComponentDto.IsVisibleId + "," + oServiceComponentDto.IsInheritedId + "," + oServiceComponentDto.Index + "," + oServiceComponentDto.Price + ", '" + oServiceComponentDto.ComponentId + "', 0," + oServiceComponentDto.ServiceComponentStatusId + ", " + oServiceComponentDto.QueueStatusId + ", " + oServiceComponentDto.IsRequiredId + ", " + oServiceComponentDto.Iscalling + ",'" + oServiceComponentDto.IdUnidadProductiva + "', " + oServiceComponentDto.IsManuallyAddedId + ",0,GETDATE(),11," + oServiceComponentDto.i_MedicoTratanteId + ", " + oServiceComponentDto.d_SaldoPaciente + ", " + oServiceComponentDto.d_SaldoAseguradora + "," + oServiceComponentDto.i_ConCargoA + "," + oServiceComponentDto.i_MedicoRealizaId + ", " + oServiceComponentDto.i_PayMedic + " , " + oServiceComponentDto.i_Cantidad + "  ,  " + oServiceComponentDto.d_Descuento + " )";
                    cnx.Execute(query);

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static float SetNewPrice(float value, string componentId)
        {
            try
            {
                if (value == null) return value;
                if (value <= 0) return value;

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select * from component where v_ComponentId = '" + componentId + "'";
                    var obj = cnx.Query<ComponentDetailList>(query).FirstOrDefault();

                    if (obj.i_PriceIsRecharged != (int)SiNo.Si) return value;
                } 

                DateTime now = DateTime.Now;
                string year = now.Year.ToString();
                string day = now.Day.ToString();
                string month = now.Month.ToString();

                bool IsRecharged = false;

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query2 = "select * from holidays where d_Date = '" + now.ToShortDateString() + "' and i_Year = " + now.Year + "";
                    var obj2 = cnx.Query<HolidayDto>(query2).FirstOrDefault();

                    if (obj2 != null)
                    {
                        IsRecharged = true;
                    }
                    else if (now >= DateTime.Parse(day + "/" + month + "/" + year + " 20:00:00") && now < DateTime.Parse(day + "/" + month + "/" + year + " 08:00:00").AddDays(1))
                    {
                        IsRecharged = true;
                    }
                    else if (now.DayOfWeek == DayOfWeek.Sunday)
                    {
                        IsRecharged = true;
                    }

                    if (IsRecharged)
                    {
                        float newValueRecharged = value + (value * float.Parse("0.2"));
                        newValueRecharged = float.Parse(newValueRecharged.ToString("N2"));
                        return newValueRecharged;
                    }

                    return value;
                } 

                
                
            }
            catch (Exception ex)
            {
                return value;
            }
        }

        public static int SwitchOperator2Values(int pacientAge, int analyzeAge, Operator2Values @operator,
            int pacientGender, int analyzeGender)
        {
            ServiceComponentDto objServiceComponentDto = new ServiceComponentDto();
            switch (@operator)
            {
                case Operator2Values.X_esIgualque_A:
                    if (analyzeGender == (int) GenderConditional.AMBOS)
                    {
                        if (pacientAge == analyzeAge)
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                        }
                        else
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.No;
                        }
                    }
                    else
                    {
                        if (pacientAge == analyzeAge && pacientGender == analyzeGender)
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                        }
                        else
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.No;
                        }
                    }

                    break;
                case Operator2Values.X_noesIgualque_A:
                    if (analyzeGender == (int) GenderConditional.AMBOS)
                    {
                        if (pacientAge != analyzeAge)
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                        }
                        else
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.No;
                        }
                    }
                    else
                    {
                        if (pacientAge != analyzeAge && pacientGender == analyzeGender)
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                        }
                        else
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.No;
                        }
                    }

                    break;
                case Operator2Values.X_esMenorque_A:

                    if (analyzeGender == (int) GenderConditional.AMBOS)
                    {
                        if (pacientAge < analyzeAge)
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                        }
                        else
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.No;
                        }
                    }
                    else
                    {
                        if (pacientAge < analyzeAge && pacientGender == analyzeGender)
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                        }
                        else
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.No;
                        }
                    }

                    break;
                case Operator2Values.X_esMenorIgualque_A:

                    if (analyzeGender == (int) GenderConditional.AMBOS)
                    {
                        if (pacientAge <= analyzeAge)
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                        }
                        else
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.No;
                        }
                    }
                    else
                    {
                        if (pacientAge <= analyzeAge && pacientGender == analyzeGender)
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                        }
                        else
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.No;
                        }
                    }

                    break;
                case Operator2Values.X_esMayorque_A:
                    if (analyzeGender == (int) GenderConditional.AMBOS)
                    {
                        if (pacientAge > analyzeAge)
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                        }
                        else
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.No;
                        }
                    }
                    else
                    {
                        if (pacientAge > analyzeAge && pacientGender == analyzeGender)
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                        }
                        else
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.No;
                        }
                    }
                    break;
                case Operator2Values.X_esMayorIgualque_A:
                    if (analyzeGender == (int) GenderConditional.AMBOS)
                    {
                        if (pacientAge >= analyzeAge)
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                        }
                        else
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.No;
                        }
                    }
                    else
                    {
                        if (pacientAge >= analyzeAge && pacientGender == analyzeGender)
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.Si;
                        }
                        else
                        {
                            objServiceComponentDto.IsRequiredId = (int) SiNo.No;
                        }
                    }

                    break;
            }

            return objServiceComponentDto.IsRequiredId;
        }

        public static List<PlanDto> TienePlan(string protocolId, string unidadProd)
        {
             try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                     var Plan = "select * from [plan]  where v_ProtocoloId = '" + protocolId + "' and v_IdUnidadProductiva = '" + unidadProd + "'";
                    var resultplan = cnx.Query<PlanDto>(Plan).ToList();

                   return resultplan;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<ventaDto> EvaluarLista(List<ventaDto> list, int tipoServicio)
        {
            foreach (var item in list)
            {
                
            }

            return null;
        }

        public class DatosTrabajador
        {
            public string PersonId { get; set; }
            public string Nombres { get; set; }
            public string ApellidoPaterno { get; set; }
            public string ApellidoMaterno { get; set; }
            public int? TipoDocumentoId { get; set; }
            public string NroDocumento { get; set; }
            public int? GeneroId { get; set; }            
            public DateTime? FechaNacimiento { get; set; }
            public int? EstadoCivil { get; set; }
            public string LugarNacimiento { get; set; }
            public int? Distrito { get; set; }
            public int? Provincia { get; set; }
            public int? Departamento { get; set; }
            public int? Reside { get; set; }
            public string Email { get; set; }
            public string Direccion { get; set; }
            public string Puesto { get; set; }
            public int? Altitud { get; set; }

            public string Minerales { get; set; }
            public int? Estudios { get; set; }
            public int? Grupo { get; set; }
            public int? Factor { get; set; }
            public string TiempoResidencia { get; set; }
            public int? TipoSeguro { get; set; }
            public int? Vivos { get; set; }
            public int? Muertos { get; set; }
            public int? Hermanos { get; set; }
            public string Telefono { get; set; }
            public int? Parantesco { get; set; }
            public int? Labor { get; set; }
            public string ResidenciaAnterior { get; set; }
            public string Nacionalidad { get; set; }
            public string Religion { get; set; }
            public string titular { get; set; }
            public string ContactoEmergencia { get; set; }
            public string CelularEmergencia { get; set; }

            public int i_EtniaRaza { get; set; }
            public int i_Migrante { get; set; }
            public string PaisOrigen { get; set; }

            public byte[] b_PersonImage { get; set; }
            public byte[] b_FingerPrintTemplate { get; set; }
            public byte[] b_FingerPrintImage { get; set; }
            public byte[] b_RubricImage { get; set; }
            public string t_RubricImageText { get; set; }
        }

        public class Secuential
        {
            public int NodeId { get; set; }
            public int TableId { get; set; }
            public int SecuentialId { get; set; }
        }

        public enum SiNo
        {
            No = 0,
            Si = 1,
            None = 2
        }

        public enum ComponenteProcedencia
        {
            Interno = 1,
            Externo = 2
        }

        public enum ServiceStatus
        {
            PorIniciar = 1,
            Iniciado = 2,
            Culminado = 3,
            Incompleto = 4,
            Cancelado = 5,
            EsperandoAptitud = 6
        }

        public enum QueueStatusId
        {
            Libre = 1,
            Llamando = 2,
            Ocupado = 3
        }

        public enum FlagCall
        {
            NoseLlamo = 0,
            Sellamo = 1
        }

        public enum Operator2Values
        {
            X_esIgualque_A = 1,
            X_noesIgualque_A = 2,
            X_esMenorque_A = 3,
            X_esMenorIgualque_A = 4,
            X_esMayorque_A = 5,
            X_esMayorIgualque_A = 6,
            X_esMayorque_A_yMenorque_B = 7,
            X_esMayorque_A_yMenorIgualque_B = 8,
            X_esMayorIgualque_A_yMenorque_B = 9,
            X_esMayorIgualque_A_yMenorIgualque_B = 12,
        }

        public enum GenderConditional
        {
            MASCULINO = 1,
            FEMENINO = 2,
            AMBOS = 3
        }

        public enum GrupoEtario
        {
            Ninio = 1,
            Adolecente = 2,
            Adulto = 3,
            AdultoMayor = 4
        }


        public static List<PacientesList> ObtenerPacientes()
        {
            try
            {
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<PacientesList> query = (from a in dbContext.obtenerpacientes_sp()
                                             select new PacientesList
                                             {
                                                 v_personId = a.v_personId,
                                                 v_name = a.v_name
                                             }).ToList();         
                return query;

                //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                //{
                //    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                //    //var query = @"select PP.v_PersonId as v_personId, PP.v_FirstLastName + ' ' + PP.v_SecondLastName + ', ' + PP.v_FirstName + ' | ' + PP.v_PersonId as v_name from person PP inner join pacient PC on PP.v_PersonId = PC.v_PersonId where PP.i_IsDeleted=0";

                //    var query = "EXEC [ObtenerPacientes_SP]";
                //    var data = cnx.Query<PacientesList>(query).ToList();
                //    return data;
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LlenarComboProtocolo_pre(ComboBox cboProtocolo, int p1, int p2, string empresa, string contrata)
        {
            try
            {
                bool result = false;

                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();
                List<EsoDto> query = new List<EsoDto>();
                if (contrata == "")
                {

                    query = (from a in dbContext.llenarcomboprotocolo_pre_1_sp(empresa)
                             select new EsoDto
                                               {
                                                   Id = a.Id,
                                                   Nombre = a.Name
                                               }).ToList();
                    result = true;
                }
                else
                {
                    query = (from a in dbContext.llenarcomboprotocolo_pre_2_sp(empresa)
                             select new EsoDto
                             {
                                 Id = a.Id,
                                 Nombre = a.Name
                             }).ToList();
                    result = true;
                }

                if (result == true)
                {
                    cboProtocolo.DataSource = query;
                    cboProtocolo.DisplayMember = "Nombre";
                    cboProtocolo.ValueMember = "Id";
                    cboProtocolo.SelectedIndex = 0;
                }
                else
                {

                }
                //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                //{
                //    ConexionSigesoft conectasam = new ConexionSigesoft();
                //    conectasam.opensigesoft();
                //    var cadena1 = "";
                //    if (contrata == "")
                //    {
                //        //cadena1 ="select v_Name as Name, v_ProtocolId as Id from protocol where v_CustomerOrganizationId='" + empresa + "' or v_EmployerOrganizationId='" + empresa + "' or v_WorkingOrganizationId='" +empresa + "'";
                //        cadena1 = "EXEC [LlenarComboProtocolo_pre_1_SP] '" + empresa + "'";
                //    }
                //    else
                //    {
                //        //cadena1 ="select v_Name as Name, v_ProtocolId as Id from protocol where v_CustomerOrganizationId='" +empresa + "' and v_EmployerOrganizationId='" + contrata + "'";
                //        cadena1 = "EXEC [LlenarComboProtocolo_pre_2_SP] '" + empresa + "'";
                //    }
                    
                //    SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                //    SqlDataReader lector = comando.ExecuteReader();
                //    List<EsoDto> list = new List<EsoDto>();
                //    while (lector.Read())
                //    {
                //        list.Add(new EsoDto(){
                //            Nombre = lector.GetValue(0).ToString(),
                //            Id = lector.GetValue(1).ToString(),
                //            });
                //        result = true;
                //    }
                //    lector.Close();
                //    conectasam.closesigesoft();

                //    if (result == true)
                //    {
                //        cboProtocolo.DataSource = list;
                //        cboProtocolo.DisplayMember = "Nombre";
                //        cboProtocolo.ValueMember = "Id";
                //        cboProtocolo.SelectedIndex = 0;
                //    }
                //    else
                //    {
                        
                //    }
                   
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LlenarComboProtocolo_Particular(ComboBox cboProtocolo, int p1, int p2, string empresa, string filtroEspecialidad)
        {
            try
            {
                bool result = false;

                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcomboprotocolo_particular_sp(p1, p2, empresa)
                                      select new EsoDto
                                      {
                                          Nombre = a.Name.Split('/')[0].Trim(),
                                          Id = a.Id,
                                          Consultorio = a.Name.Split('/')[1].Trim() == null ? "-" : a.Name.Split('/')[1].Trim()
                                      }).ToList();

                if (filtroEspecialidad != "")
                {
                    if (filtroEspecialidad == "ECOGRAFÍA" || filtroEspecialidad == "RADIOLOGÍA")
                    {
                        query = query.FindAll(p => p.Consultorio == "ECOGRAFÍA" || p.Consultorio == "RADIOLOGÍA" || p.Consultorio == "- - -");
                    }
                    else
                    {
                        query = query.FindAll(p => p.Consultorio == filtroEspecialidad || p.Consultorio == "- - -");
                    }
                }

                query.Insert(0, new EsoDto { EsoId = -1, Nombre = "--Seleccionar--" });

                if (query != null)
                {
                    result = true;
                }
                if (result == true)
                {
                    cboProtocolo.DataSource = query;
                    cboProtocolo.DisplayMember = "Nombre";
                    cboProtocolo.ValueMember = "Id";
                    cboProtocolo.SelectedIndex = 0;
                }
                else
                {

                }

                //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                //{
                //    ConexionSigesoft conectasam = new ConexionSigesoft();
                //    conectasam.opensigesoft();
                //    var cadena1 =
                //        "select PR.v_Name as Name, PR.v_ProtocolId as Id " +
                //        "from protocol PR " +
                //        "inner join organization OO on PR.v_CustomerOrganizationId=OO.v_OrganizationId " +
                //        "where OO.i_OrganizationTypeId=3 and PR.i_MasterServiceId="+p1+" and PR.i_MasterServiceTypeId ="+p2;
                //    SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                //    SqlDataReader lector = comando.ExecuteReader();
                //    List<EsoDto> list = new List<EsoDto>();
                //    while (lector.Read())
                //    {
                //        list.Add(new EsoDto()
                //        {
                //            Nombre = lector.GetValue(0).ToString(),
                //            Id = lector.GetValue(1).ToString(),
                //        });
                //        result = true;
                //    }
                //    lector.Close();
                //    conectasam.closesigesoft();
                //    if (result == true)
                //    {
                //        cboProtocolo.DataSource = list;
                //        cboProtocolo.DisplayMember = "Nombre";
                //        cboProtocolo.ValueMember = "Id";
                //        cboProtocolo.SelectedIndex = 0;
                //    }
                //    else
                //    {

                //    }

                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<EsoDtoProt> LlenarComboProtocolo_Particular_new(ComboBox cboProtocolo, int p1, int p2, string empresa, string filtroEspecialidad)
        {
            try
            {
                bool result = false;

                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDtoProt> query = (from a in dbContext.llenarcomboprotocolo_particular_sp(p1, p2, empresa)
                                          select new EsoDtoProt
                                          {
                                              Nombre = a.Name.Split('/')[0].Trim(),
                                              Id = a.Id,
                                              Consultorio = a.Name.Split('/')[1].Trim() == null ? "-" : a.Name.Split('/')[1].Trim()
                                          }).ToList();

                if (filtroEspecialidad != "")
                {
                    if (filtroEspecialidad == "ECOGRAFÍA" || filtroEspecialidad == "RADIOLOGÍA")
                    {
                        query = query.FindAll(p => p.Consultorio == "ECOGRAFÍA" || p.Consultorio == "RADIOLOGÍA" || p.Consultorio == "- - -");
                    }
                    else
                    {
                        query = query.FindAll(p => p.Consultorio == filtroEspecialidad || p.Consultorio == "- - -");
                    }
                }

                query.Insert(0, new EsoDtoProt { Id = "-1", Nombre = "--Seleccionar--", Consultorio = "---" });

                return query;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LlenarComboProtocolo_Seguros(ComboBox cboProtocolo, int p1, int p2, string seguro, string empresa)
        {
            try
            {
                bool result = false;
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDto> query = (from a in dbContext.llenarcomboprotocolo_seguros_sp(p1, p2, seguro, empresa)
                                      select new EsoDto
                                      {
                                          Nombre = a.Name,
                                          Id = a.Id
                                      }).ToList();
                query.Insert(0, new EsoDto { Id = "-1", Nombre = "--Seleccionar--" });

                //return query;
                cboProtocolo.DataSource = query;
                cboProtocolo.DisplayMember = "Nombre";
                cboProtocolo.ValueMember = "Id";
                cboProtocolo.SelectedIndex = 0;

                //using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                //{
                //    ConexionSigesoft conectasam = new ConexionSigesoft();
                //    conectasam.opensigesoft();
                //    var cadena1 =
                //        "select PR.v_Name as Name, PR.v_ProtocolId as Id " +
                //        "from protocol PR " +
                //        "inner join organization OO on PR.v_AseguradoraOrganizationId=OO.v_OrganizationId " +
                //        "where OO.i_OrganizationTypeId=4 and PR.i_MasterServiceId="+p2+" and PR.i_MasterServiceTypeId ="+p1+
                //        " and PR.v_AseguradoraOrganizationId='"+seguro+"' and PR.v_CustomerOrganizationId='"+empresa+"' ";
                //    SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                //    SqlDataReader lector = comando.ExecuteReader();
                //    List<EsoDto> list = new List<EsoDto>();
                //    while (lector.Read())
                //    {
                //        list.Add(new EsoDto()
                //        {
                //            Nombre = lector.GetValue(0).ToString(),
                //            Id = lector.GetValue(1).ToString(),
                //        });
                //        result = true;
                //    }
                //    lector.Close();
                //    conectasam.closesigesoft();
                //    if (result == true)
                //    {
                //        cboProtocolo.DataSource = list;
                //        cboProtocolo.DisplayMember = "Nombre";
                //        cboProtocolo.ValueMember = "Id";
                //        cboProtocolo.SelectedIndex = 0;
                //    }
                //    else
                //    {

                //    }

                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<EsoDtoProt> LlenarComboProtocolo_Seguros_new(ComboBox cboProtocolo, int p1, int p2, string seguro, string empresa)
        {
            try
            {
                bool result = false;
                SigesoftEntitiesModel dbContext = new SigesoftEntitiesModel();

                List<EsoDtoProt> query = (from a in dbContext.llenarcomboprotocolo_seguros_sp(p1, p2, seguro, empresa)
                                          select new EsoDtoProt
                                          {
                                              Nombre = a.Name.Split('/')[0].Trim(),
                                              Id = a.Id,
                                              Consultorio = a.Name.Split('/')[1].Trim() == null ? "-" : a.Name.Split('/')[1].Trim()
                                          }).ToList();
                query.Insert(0, new EsoDtoProt { Id = "-1", Nombre = "--Seleccionar--", Consultorio = "---" });

                return query;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void LlenarComboPlanes_Seguros(ComboBox cboProtocolo, string protocolo)
        {
            try
            {
                bool result = false;
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    SAMBHS.Common.BE.Custom.ConexionSigesoft conectasam = new SAMBHS.Common.BE.Custom.ConexionSigesoft();
                    conectasam.opensigesoft();
                    var cadena1 = @" from protocol PR " +
                                " left join [dbo].[plan] PL on PR.v_ProtocolId=PL.v_ProtocoloId " +
                                " left join [20505310072].dbo.linea as lin on PL.v_IdUnidadProductiva = lin.v_IdLinea " +
                                " where v_ProtocolId='" + protocolo + "";
                    SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                    SqlDataReader lector = comando.ExecuteReader();
                    List<EsoDto> list = new List<EsoDto>();
                    while (lector.Read())
                    {
                        list.Add(new EsoDto()
                        {
                            Nombre = lector.GetValue(0).ToString(),
                            Id = lector.GetValue(1).ToString(),
                        });
                        result = true;
                    }
                    lector.Close();
                    conectasam.closesigesoft();
                    if (result == true)
                    {
                        cboProtocolo.DataSource = list;
                        cboProtocolo.DisplayMember = "Nombre";
                        cboProtocolo.ValueMember = "Id";
                        cboProtocolo.SelectedIndex = 0;
                    }
                    else
                    {

                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string PermisoUsuario(int nodeId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewContasolConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = "select s.v_SecretAnswer from systemuser s where i_SystemUserId = " + nodeId  ;

                    var validacion = cnx.Query<string>(query).FirstOrDefault();

                    return validacion;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public ProductoStock UpdateStockProducto(string codigoInterno)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewContasolConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = "select p.v_IdProducto, p.v_CodInterno, p.v_Descripcion, pa.v_ProductoDetalleId, pa.d_StockActual from producto p left join productodetalle pd on p.v_IdProducto = pd.v_IdProducto left join productoalmacen pa on pd.v_IdProductoDetalle = pa.v_ProductoDetalleId where p.v_CodInterno = '" + codigoInterno + "' and pa.v_Periodo = '" + DateTime.Now.ToShortDateString().Split('/')[2] + "' and pa.i_IdAlmacen = 1";

                    return cnx.Query<ProductoStock>(query).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public tipoServicioforServicio getTiposServicioForProtocolo(string protocoloId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewContasolConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    var query = "select i_MasterServiceTypeId as 'TipoServicio', i_MasterServiceId as 'Servicio' from [SigesoftDesarrollo_2].[dbo].[protocol] where v_ProtocolId = '" + protocoloId + "'";

                    return cnx.Query<tipoServicioforServicio>(query).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void UpdateStockActual(string ProductoDetalleId, decimal cantidad)
        {
            using (var cnx = ConnectionHelper.GetNewContasolConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                var actualizar = "UPDATE productoalmacen SET d_StockActual =  " + cantidad + " WHERE v_ProductoDetalleId = '" + ProductoDetalleId + "' and v_Periodo = '" + DateTime.Now.ToShortDateString().Split('/')[2] + "' and i_IdAlmacen = 1";
                cnx.Execute(actualizar);

            }
        }


        public static int CreateItemMarketing(string name)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                string ultimoQuery = "select i_ItemId from datahierarchy where i_GroupId = 126 order by i_ItemId desc";
                List<int> list = cnx.Query<int>(ultimoQuery).ToList();
                int ultimo = list.Count() == 0 ? 0 : list[0] + 1;
                var query = "INSERT INTO datahierarchy values(126, " + ultimo.ToString() + ", '" + name +
                            "', '', '',null,0,11,'" + DateTime.Now.ToString() + "', null,null,null)";

                cnx.Execute(query);

                return ultimo;
            }
        }


        public static int LlenarComboTurno(ComboBox cbo, string usuario, string cal)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                    int grupo = GRUPO_ATENCION(usuario, cal, cnx);

                    string validacion = grupo == 0 ? "NULL" : "'" + grupo.ToString() + "'";

                    var query = @"select sp.i_ParameterId as Id, sp.v_Value1  as Value1
                                from  systemparameter SP
                                where sp.i_GroupId = " + validacion + " and sp.i_IsDeleted = 0 and sp.v_Field = '0'";

                    var data = cnx.Query<KeyValueDTO>(query).ToList();
                    data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Value1";
                    cbo.ValueMember = "Id";
                    cbo.SelectedIndex = 0;

                    return grupo;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static int GRUPO_ATENCION(string usuario, string cal, IDbConnection cnx)
        {
            var query1 = @"select top 1 *
                                from Horarios orr 
                                where orr.i_SystemUserId = '" + usuario + "' and " +
                        "((CONVERT(varchar,orr.d_FechaInicio,23)) <= '" + cal + "' and  '" + cal + "' <=  (CONVERT(varchar,orr.d_FechaFin,23))) AND i_IsDeleted = 0";
            var data1 = cnx.Query<Horarios>(query1).FirstOrDefault();
            List<GrupoDia> GrupoDia_ = new List<GrupoDia>();
            if (data1 != null)
            {
                if (data1.i_01 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "01";
                    objGrupoDia.Grupo = data1.i_01;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_02 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "02";
                    objGrupoDia.Grupo = data1.i_02;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_03 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "03";
                    objGrupoDia.Grupo = data1.i_03;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_04 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "04";
                    objGrupoDia.Grupo = data1.i_04;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_05 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "05";
                    objGrupoDia.Grupo = data1.i_05;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_06 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "06";
                    objGrupoDia.Grupo = data1.i_06;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_07 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "07";
                    objGrupoDia.Grupo = data1.i_07;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_08 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "08";
                    objGrupoDia.Grupo = data1.i_08;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_09 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "09";
                    objGrupoDia.Grupo = data1.i_09;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_10 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "10";
                    objGrupoDia.Grupo = data1.i_10;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_11 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "11";
                    objGrupoDia.Grupo = data1.i_11;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_12 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "12";
                    objGrupoDia.Grupo = data1.i_12;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_13 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "13";
                    objGrupoDia.Grupo = data1.i_13;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_14 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "14";
                    objGrupoDia.Grupo = data1.i_14;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_15 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "15";
                    objGrupoDia.Grupo = data1.i_15;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_16 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "16";
                    objGrupoDia.Grupo = data1.i_16;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_17 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "17";
                    objGrupoDia.Grupo = data1.i_17;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_18 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "18";
                    objGrupoDia.Grupo = data1.i_18;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_19 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "19";
                    objGrupoDia.Grupo = data1.i_19;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_20 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "20";
                    objGrupoDia.Grupo = data1.i_20;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_21 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "21";
                    objGrupoDia.Grupo = data1.i_21;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_22 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "22";
                    objGrupoDia.Grupo = data1.i_22;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_23 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "23";
                    objGrupoDia.Grupo = data1.i_23;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_24 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "24";
                    objGrupoDia.Grupo = data1.i_24;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_25 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "25";
                    objGrupoDia.Grupo = data1.i_25;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_26 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "26";
                    objGrupoDia.Grupo = data1.i_26;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_27 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "27";
                    objGrupoDia.Grupo = data1.i_27;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_28 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "28";
                    objGrupoDia.Grupo = data1.i_28;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_29 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "29";
                    objGrupoDia.Grupo = data1.i_29;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_30 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "30";
                    objGrupoDia.Grupo = data1.i_30;
                    GrupoDia_.Add(objGrupoDia);
                }
                if (data1.i_31 != 0)
                {
                    GrupoDia objGrupoDia = new GrupoDia();
                    objGrupoDia.Dia = "31";
                    objGrupoDia.Grupo = data1.i_31;
                    GrupoDia_.Add(objGrupoDia);
                }
            }

            int grupo = 0;

            foreach (var item in GrupoDia_)
            {
                if (item.Dia == cal.Split('-')[2])
                {
                    grupo = item.Grupo;
                }
            }
            return grupo;
        }

        public static void LlenarComboHorario(ComboBox cbo, string usuario, string turno, string fecha)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();
                    int grupo = GRUPO_ATENCION(usuario, fecha, cnx);
                    var validacion = turno == "SAMBHS.Common.Resource.KeyValueDTO" ? "-1" : turno;
                    var query = @"select sp.i_ParameterId as Id, sp.v_Value1  as Value1
                                    from systemparameter sp 
                                    where sp.i_GroupId = '" + grupo + @"' and sp.i_IsDeleted = 0 and sp.v_Field = '0-0' and i_ParentParameterId = '" + validacion + @"'
                                    EXCEPT    
                                    select i_CodigoAtencion as Id, sp.v_Value1  as Value1 
                                    from service s
                                    join calendar c on s.v_ServiceId = c.v_ServiceId
                                    join systemparameter sp on sp.i_GroupId  = '" + grupo + @"' 
                                    where s.i_MedicoAtencion = '" + usuario + @"' and (c.i_CalendarStatusId != 4 and c.i_IsDeleted != 1) and sp.i_ParameterId = s.i_CodigoAtencion
                                    and (CONVERT(varchar,s.d_ServiceDate,23)) = '" + fecha + "'  order by Value1 ";

                    var data = cnx.Query<KeyValueDTO>(query).ToList();
                    data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });

                    cbo.DataSource = data;
                    cbo.DisplayMember = "Value1";
                    cbo.ValueMember = "Id";
                    cbo.SelectedIndex = 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
