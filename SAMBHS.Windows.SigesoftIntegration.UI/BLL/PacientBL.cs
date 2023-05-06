using SAMBHS.Common.BE.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using System.Transactions;
using System.Data.SqlClient;
using System.Data;

namespace SAMBHS.Windows.SigesoftIntegration.UI.BLL
{
    public class PacientBL
    {
        
        public List<PersonList_2> LlenarPerson(ref OperationResult objOperationResult)
        {
            try
            {
                List<PersonList_2> PersonList = new List<PersonList_2>();
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query =
                        "select per.v_FirstLastName,  per.v_SecondLastName, per.v_FirstName, per.v_PersonId from person per " +
                        "inner join pacient pac on per.v_PersonId = pac.v_PersonId " +
                        "where per.i_IsDeleted = 0";
                    
                    
                    var List = cnx.Query<PacientList>(query).ToList();
                    foreach (var obj in List)
                    {
                        PersonList_2 objPerson = new PersonList_2();
                        objPerson.v_name = obj.v_FirstLastName + " " + obj.v_SecondLastName + " " + obj.v_FirstName + " | " + obj.v_PersonId;
                        objPerson.v_personId = obj.v_PersonId;
                        PersonList.Add(objPerson);
                    }
                    var objData = PersonList.AsEnumerable().
                        GroupBy(g => g.v_name)
                        .Select(s => s.First());

                    List<PersonList_2> x = objData.ToList();
                    objOperationResult.Success = 1;
                    return x;


                }


                
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.ExceptionMessage = Utils.ExceptionFormatter(ex);
                return null;
            }
        }

        public List<PacientList> GetPacientsPagedAndFilteredByPErsonId(ref OperationResult pobjOperationResult, int? pintPageIndex, int pintResultsPerPage, string pstrPErsonId)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select pac.v_PersonId, per.v_FirstName, per.v_FirstLastName, per.v_SecondLastName, " +
                                "per.v_AdressLocation, per.v_TelephoneNumber, per.v_Mail, sys.v_UserName AS v_CreationUser, " +
                                "sys2.v_UserName AS v_UpdateUser, pac.d_UpdateDate, pac.d_InsertDate AS d_CreationDate, " +
                                "pac.d_UpdateDate AS d_UpdateDate, per.i_DepartmentId, per.i_ProvinceId, per.i_DistrictId, per.i_ResidenceInWorkplaceId, " +
                                "per.v_ResidenceTimeInWorkplace, per.i_TypeOfInsuranceId, per.i_NumberLivingChildren, per.i_NumberDependentChildren, " +
                                "per.i_NumberLiveChildren, per.i_NumberDeadChildren, per.v_DocNumber " +
                                "from person per " +
                                "inner join pacient pac on per.v_PersonId = pac.v_PersonId " +
                                "left join systemuser sys on pac.i_InsertUserId = sys.i_SystemUserId " +
                                "left join systemuser sys2 on pac.i_UpdateUserId = sys2.i_SystemUserId " +
                                "where pac.v_PersonId = '" + pstrPErsonId + "' and per.i_IsDeleted = 0";

                    var List = cnx.Query<PacientList>(query).OrderBy(x => x.v_FirstLastName).Take(pintResultsPerPage).ToList();
                    
                    pobjOperationResult.Success = 1;
                    return List;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<PacientList> GetPacientsPagedAndFiltered(ref OperationResult pobjOperationResult, int pintPageIndex, int pintResultsPerPage, string pstrFirstLastNameorDocNumber)
        {
            //mon.IsActive = true;
            try
            {
                int intId = -1;
                bool FindById = int.TryParse(pstrFirstLastNameorDocNumber, out intId);
                var Id = intId.ToString();

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select pac.v_PersonId, per.v_FirstName, per.v_FirstLastName, per.v_SecondLastName, " +
                                "per.v_AdressLocation, per.v_TelephoneNumber, per.v_Mail, sys.v_UserName AS v_CreationUser, " +
                                "sys2.v_UserName AS v_UpdateUser, pac.d_UpdateDate, pac.d_InsertDate AS d_CreationDate, " +
                                "pac.d_UpdateDate AS d_UpdateDate, per.i_DepartmentId, per.i_ProvinceId, per.i_DistrictId, per.i_ResidenceInWorkplaceId, " +
                                "per.v_ResidenceTimeInWorkplace, per.i_TypeOfInsuranceId, per.i_NumberLivingChildren, per.i_NumberDependentChildren " +
                                "from person per " +
                                "inner join pacient pac on per.v_PersonId = pac.v_PersonId " +
                                "left join systemuser sys on pac.i_InsertUserId = sys.i_SystemUserId " +
                                "left join systemuser sys2 on pac.i_UpdateUserId = sys2.i_SystemUserId " +
                                "where per.i_IsDeleted = 0 and (per.v_FirstName like '%" + pstrFirstLastNameorDocNumber + "%' or per.v_FirstLastName like '%" + pstrFirstLastNameorDocNumber + "%' " +
                                "or per.v_SecondLastName like '%" + pstrFirstLastNameorDocNumber + "%')";

                    var List = cnx.Query<PacientList>(query).OrderBy(x => x.v_FirstLastName).Take(pintResultsPerPage).ToList();

                    pobjOperationResult.Success = 1;
                    return List;
                }
        
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<PacientList> GetPacientsPagedAndFiltered_Apellidos(ref OperationResult objOperationResult, int pintPageIndex, int pintResultsPerPage, string pstrFilterExpression, string apPat, string apMat)
        {
            try
            {
                int intId = -1;
                bool FindById = int.TryParse(pstrFilterExpression, out intId);
                var Id = intId.ToString();

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select pac.v_PersonId, per.v_FirstName, per.v_FirstLastName, per.v_SecondLastName, " +
                                "per.v_AdressLocation, per.v_TelephoneNumber, per.v_Mail, sys.v_UserName AS v_CreationUser, " +
                                "sys2.v_UserName AS v_UpdateUser, pac.d_UpdateDate, pac.d_InsertDate AS d_CreationDate, " +
                                "pac.d_UpdateDate AS d_UpdateDate, per.i_DepartmentId, per.i_ProvinceId, per.i_DistrictId, per.i_ResidenceInWorkplaceId, " +
                                "per.v_ResidenceTimeInWorkplace, per.i_TypeOfInsuranceId, per.i_NumberLivingChildren, per.i_NumberDependentChildren " +
                                "from person per " +
                                "inner join pacient pac on per.v_PersonId = pac.v_PersonId " +
                                "left join systemuser sys on pac.i_InsertUserId = sys.i_SystemUserId " +
                                "left join systemuser sys2 on pac.i_UpdateUserId = sys2.i_SystemUserId " +
                                "where (per.v_FirstLastName like '%" + apPat + "%' " +
                                "or per.v_SecondLastName like '%" + apMat + "%') and per.i_IsDeleted = 0";

                    var List = cnx.Query<PacientList>(query).OrderBy(x => x.v_FirstLastName).Take(pintResultsPerPage).ToList();

                    objOperationResult.Success = 1;
                    return List;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<PacientList> GetPacientsPagedAndFiltered_Apellidos_Nombre(ref OperationResult objOperationResult, int pintPageIndex, int pintResultsPerPage, string pstrFilterExpression, string apPat, string apMat, string nombre)
        {
            try
            {
                int intId = -1;
                bool FindById = int.TryParse(pstrFilterExpression, out intId);
                var Id = intId.ToString();

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select pac.v_PersonId, per.v_FirstName, per.v_FirstLastName, per.v_SecondLastName, " +
                                "per.v_AdressLocation, per.v_TelephoneNumber, per.v_Mail, sys.v_UserName AS v_CreationUser, " +
                                "sys2.v_UserName AS v_UpdateUser, pac.d_UpdateDate, pac.d_InsertDate AS d_CreationDate, " +
                                "pac.d_UpdateDate AS d_UpdateDate, per.i_DepartmentId, per.i_ProvinceId, per.i_DistrictId, per.i_ResidenceInWorkplaceId, " +
                                "per.v_ResidenceTimeInWorkplace, per.i_TypeOfInsuranceId, per.i_NumberLivingChildren, per.i_NumberDependentChildren " +
                                "from person per " +
                                "inner join pacient pac on per.v_PersonId = pac.v_PersonId " +
                                "left join systemuser sys on pac.i_InsertUserId = sys.i_SystemUserId " +
                                "left join systemuser sys2 on pac.i_UpdateUserId = sys2.i_SystemUserId " +
                                "where (per.v_FirstLastName like '%" + apPat + "%' or per.v_FirstName like '%" + nombre + "%' " +
                                "or per.v_SecondLastName like '%" + apMat + "%') and per.i_IsDeleted = 0";

                    var List = cnx.Query<PacientList>(query).OrderBy(x => x.v_FirstLastName).Take(pintResultsPerPage).ToList();

                    objOperationResult.Success = 1;
                    return List;
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public PacientList GetPacient(ref OperationResult pobjOperationResult, string pstrPacientId, string pstNroDocument)
        {
            //mon.IsActive = true;

            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select pac.v_PersonId, per.v_FirstName, per.v_FirstLastName, per.v_SecondLastName, " +
                                "per.v_DocNumber, per.v_BirthPlace, per.i_MaritalStatusId, per.i_LevelOfId, " +
                                "per.i_DocTypeId, per.i_SexTypeId, per.v_TelephoneNumber, per.v_AdressLocation, " +
                                "per.v_Mail, per.b_PersonImage AS b_Photo, per.d_Birthdate, per.i_BloodFactorId, " +
                                "per.i_BloodGroupId, per.b_FingerPrintTemplate, per.b_FingerPrintImage, per.b_RubricImage, " +
                                "per.t_RubricImageText, per.v_CurrentOccupation, per.i_DepartmentId, per.i_ProvinceId, " +
                                "per.i_DistrictId, per.i_ResidenceInWorkplaceId, per.v_ResidenceTimeInWorkplace, per.i_TypeOfInsuranceId, " +
                                "per.i_NumberLivingChildren, per.i_NumberDependentChildren, per.i_Relationship, per.v_ExploitedMineral, " +
                                "per.i_AltitudeWorkId, per.i_PlaceWorkId, per.v_OwnerName, per.v_NroPoliza, " +
                                "per.v_Deducible, per.i_NroHermanos, per.i_NumberLiveChildren, per.i_NumberDeadChildren, " +
                                "per.v_Nacionalidad, per.v_ResidenciaAnterior, sysp.v_Value1 AS GrupoSanguineo, sysp2.v_Value1 AS FactorSanguineo, per.v_Religion " +
                                "from person per " +
                                "inner join pacient pac on per.v_PersonId = pac.v_PersonId " +
                                "left join systemuser sys on pac.i_InsertUserId = sys.i_SystemUserId " +
                                "left join systemparameter sysp on per.i_BloodGroupId = sysp.i_ParameterId and sysp.i_GroupId = 154" +
                                "left join systemparameter sysp2 on per.i_BloodFactorId = sysp2.i_ParameterId and sysp2.i_GroupId = 155" +
                                "left join systemuser sys2 on pac.i_UpdateUserId = sys2.i_SystemUserId " +
                                "where pac.v_PersonId = '" + pstrPacientId + "' or per.v_DocNumber = '" + pstNroDocument + "'";

                    var List = cnx.Query<PacientList>(query).FirstOrDefault();

                    pobjOperationResult.Success = 1;
                    return List;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<SAMBHS.Windows.SigesoftIntegration.UI.AgendaBl.PuestoList> GetAllPuestos()
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select v_CurrentOccupation AS PuestoId, v_CurrentOccupation AS Puesto from  person where i_IsDeleted = 0";

                    var List = cnx.Query<SAMBHS.Windows.SigesoftIntegration.UI.AgendaBl.PuestoList>(query).ToList();

                    var objData = List.AsEnumerable().
                        GroupBy(g => g.Puesto)
                        .Select(s => s.First());

                    List<SAMBHS.Windows.SigesoftIntegration.UI.AgendaBl.PuestoList> x = objData.ToList().FindAll(p => p.Puesto != "" || p.Puesto != null);
                    return x;
                }
                
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string UpdatePerson(ref OperationResult pobjOperationResult, personCustom dataPerson, List<string> ClientSession, string NumbreDocument, string _NumberDocument)
        {
            try
            {

                bool IsOtherDocNumber = false;

                if (NumbreDocument != _NumberDocument)
                {
                    IsOtherDocNumber = true;
                }


                if (dataPerson != null && IsOtherDocNumber == true)
                {
                    OperationResult objOperationResult6 = new OperationResult();
                    var _recordCount1 = GetPersonCount(ref objOperationResult6, dataPerson.v_DocNumber);

                    if (_recordCount1 != 0)
                    {
                        pobjOperationResult.ErrorMessage = "El número de documento  <font color='red'>" + dataPerson.v_DocNumber + "</font> ya se encuentra registrado.<br> Por favor ingrese otro número de documento.";
                        return "-1";
                    }
                }

                if (dataPerson.v_Deducible == null)
                {
                    dataPerson.v_Deducible = decimal.Parse("0.00");
                }

                if (dataPerson.i_NumberLivingChildren == null) dataPerson.i_NumberLivingChildren = 0;
                if (dataPerson.i_NumberDependentChildren == null) dataPerson.i_NumberDependentChildren = 0;

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "UPDATE person " +
                                "SET  " +
                                "v_FirstName= '" + dataPerson.v_FirstName + "', " +
                                "v_FirstLastName= '" + dataPerson.v_FirstLastName + "', " +
                                "v_SecondLastName= '" + dataPerson.v_SecondLastName + "', " +
                                "i_DocTypeId =  " + dataPerson.i_DocTypeId + ", " +
                                "i_SexTypeId =  " + dataPerson.i_SexTypeId + ", " +
                                "i_MaritalStatusId=  " + dataPerson.i_MaritalStatusId + ", " +
                                "i_LevelOfId=  " + dataPerson.i_LevelOfId + ", " +
                                "v_DocNumber= '" + dataPerson.v_DocNumber + "', " +
                                "d_Birthdate=  '" + dataPerson.d_Birthdate.ToShortDateString() + "', " +
                                "v_BirthPlace = '" + dataPerson.v_BirthPlace + "', " +
                                "v_TelephoneNumber = '" + dataPerson.v_TelephoneNumber + "', " +
                                "v_AdressLocation = '" + dataPerson.v_AdressLocation + "',  " +
                                "v_Mail = '" + dataPerson.v_Mail + "', " +
                                "v_CurrentOccupation = '" + dataPerson.v_CurrentOccupation + "', " +
                                "i_BloodGroupId =  " + dataPerson.i_BloodGroupId + ", " +
                                "i_BloodFactorId =  " + dataPerson.i_BloodFactorId + ", " +
                                "v_NroPoliza = '" + dataPerson.v_NroPoliza + "', " +
                                "v_Deducible = " + dataPerson.v_Deducible + ", " +
                                "i_DepartmentId =  " + dataPerson.i_DepartmentId + ", " +
                                "i_ProvinceId =  " + dataPerson.i_ProvinceId + ", " +
                                "i_DistrictId =  " + dataPerson.i_DistrictId + ", " +
                                "i_ResidenceInWorkplaceId =  " + dataPerson.i_ResidenceInWorkplaceId + ", " +
                                "v_ResidenceTimeInWorkplace = '" + dataPerson.v_ResidenceTimeInWorkplace + "', " +
                                "i_TypeOfInsuranceId =  " + dataPerson.i_TypeOfInsuranceId + ", " +
                                "i_NumberLivingChildren =  " + dataPerson.i_NumberLivingChildren + ", " +
                                "i_NumberDependentChildren =  " + dataPerson.i_NumberDependentChildren + ", " +
                                "i_Relationship = " + dataPerson.i_Relationship + ", " +
                                "i_AltitudeWorkId = " + dataPerson.i_AltitudeWorkId + ", " +
                                "i_PlaceWorkId = " + dataPerson.i_PlaceWorkId + ", " +
                                "v_OwnerName = '" + dataPerson.v_OwnerName + "', " +
                                "v_ExploitedMineral = '" + dataPerson.v_ExploitedMineral + "', " +
                                "v_Nacionalidad = '" + dataPerson.v_Nacionalidad + "', " +
                                "v_ResidenciaAnterior = '" + dataPerson.v_ResidenciaAnterior + "', " +
                                "v_Religion = '" + dataPerson.v_Religion + "', " +
                                "v_ComentaryUpdate = '" + dataPerson.v_ComentaryUpdate + "', " +
                                "d_UpdateDate =  '" + DateTime.Now + "', " +
                                "i_UpdateUserId =  " + Int32.Parse(ClientSession[2]) + " " +
                                "WHERE v_PersonId = '"+ dataPerson.v_PersonId +"'";
                    
                    cnx.Execute(query);
                    var result = UpdateImagesPerson(dataPerson.v_PersonId, dataPerson);
                    if (result != "1")
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage = "Sucedió un error al guardar las imagenes.";
                    }
                    pobjOperationResult.Success = 1;
                    return "1";
                }
            }
            catch (Exception e)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ErrorMessage = "Sucedió un error al actualizar al paciente, por favor vuelva a intentar.";
                return "-1";
            }
        }

        public int GetPersonCount(ref OperationResult pobjOperationResult, string DocNumber)
        {
            //mon.IsActive = true;
            try
            {
                int intResult = 0;
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select * from person where v_DocNumber = '" + DocNumber + "' and i_IsDeleted = 0";

                    var objperson = cnx.Query<personCustom>(query).ToList();

                    if (objperson != null)
                    {
                        intResult = objperson.Count();
                    }
                    
                }

                

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

        public string AddPacient(ref OperationResult pobjOperationResult, personCustom pobjDtoEntity, List<string> ClientSession)
        {
            string NewId = "(No generado)";
            try
            {

                var personId = AddPerson(ref pobjOperationResult, pobjDtoEntity, ClientSession);
                
                if (personId == "-1")
                {
                    pobjOperationResult.Success = 0;
                    return "-1";
                }

                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "INSERT INTO pacient (v_PersonId, i_IsDeleted, i_InsertUserId, d_InsertDate, i_UpdateUserId, d_UpdateDate, i_UpdateNodeId) " +
                                "VALUES ('"+ personId +"', 0, "+ Int32.Parse(ClientSession[2]) +", '"+ DateTime.Now +"', NULL, NULL, NULL)";


                    cnx.Execute(query);
                    pobjOperationResult.Success = 1;
                    return personId;
                }
            }
            catch (Exception e)
            {
                pobjOperationResult.Success = 0;
                return "-1";
            }
        }

        public string AddPerson(ref OperationResult pobjOperationResult, personCustom pobjDtoEntity, List<string> ClientSession)
        {
            try
            {
                
                string NewId = "(No generado)";
                pobjDtoEntity.i_IsDeleted = 0;
                var SecuentialId = Utilidades.GetNextSecuentialId(Int32.Parse(ClientSession[0]), 8);
                var newId = Utilidades.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "PP");
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {

                    var query = "INSERT INTO person (v_PersonId, v_FirstName, v_FirstLastName, v_SecondLastName, i_DocTypeId, " +
                                "v_DocNumber, d_Birthdate, v_BirthPlace, i_SexTypeId, i_MaritalStatusId, i_LevelOfId, " +
                                "v_TelephoneNumber, v_AdressLocation, v_GeografyLocationId, v_ContactName, v_EmergencyPhone, " +
                                "b_PersonImage, v_Mail, i_BloodGroupId, i_BloodFactorId, b_FingerPrintTemplate, b_RubricImage, " +
                                "b_FingerPrintImage, t_RubricImageText, v_CurrentOccupation, i_DepartmentId, i_ProvinceId, " +
                                "i_DistrictId, i_ResidenceInWorkplaceId, v_ResidenceTimeInWorkplace, i_TypeOfInsuranceId, " +
                                "i_NumberLivingChildren, i_NumberDependentChildren, i_OccupationTypeId, v_OwnerName, " +
                                "i_NumberLiveChildren, i_NumberDeadChildren, i_IsDeleted, i_InsertUserId, d_InsertDate, i_UpdateUserId, " +
                                "d_UpdateDate, i_InsertNodeId, i_UpdateNodeId, i_Relationship, v_ExploitedMineral, i_AltitudeWorkId, " +
                                "i_PlaceWorkId, v_NroPoliza, v_Deducible, i_NroHermanos, v_Password, v_Procedencia, v_CentroEducativo, " +
                                "v_Religion, v_Nacionalidad, v_ResidenciaAnterior, v_Subs) " +
                                "VALUES ('" + newId + "', '" + pobjDtoEntity.v_FirstName + "', '" + pobjDtoEntity.v_FirstLastName + "', '" + pobjDtoEntity.v_SecondLastName + "'," +
                                "" + pobjDtoEntity.i_DocTypeId + ", '" + pobjDtoEntity.v_DocNumber + "', '" + pobjDtoEntity.d_Birthdate.ToShortDateString() + "', '" + pobjDtoEntity.v_BirthPlace + "'," +
                                "" + pobjDtoEntity.i_SexTypeId + ", " + pobjDtoEntity.i_MaritalStatusId + ", " + pobjDtoEntity.i_LevelOfId + ", '" + pobjDtoEntity.v_TelephoneNumber + "', " +
                                "'" + pobjDtoEntity.v_AdressLocation + "', '" + pobjDtoEntity.v_GeografyLocationId + "', '" + pobjDtoEntity.v_ContactName + "', '" + pobjDtoEntity.v_EmergencyPhone + "', " +
                                " NULL, '" + pobjDtoEntity.v_Mail + "', " + pobjDtoEntity.i_BloodGroupId + ", " + pobjDtoEntity.i_BloodFactorId + ", " +
                                " NULL, NULL, NULL, '" + pobjDtoEntity.t_RubricImageText + "'," +
                                "'" + pobjDtoEntity.v_CurrentOccupation + "', " + pobjDtoEntity.i_DepartmentId + ", " + pobjDtoEntity.i_ProvinceId + ", " + pobjDtoEntity.i_DistrictId + "," +
                                "" + pobjDtoEntity.i_ResidenceInWorkplaceId + ", '" + pobjDtoEntity.v_ResidenceTimeInWorkplace + "', " + pobjDtoEntity.i_TypeOfInsuranceId + ", " +
                                " 0, 0, -1, '" + pobjDtoEntity.v_OwnerName + "', " +
                                "" + pobjDtoEntity.i_NumberLiveChildren + ", " + pobjDtoEntity.i_NumberDeadChildren + ", " + pobjDtoEntity.i_IsDeleted + ", " + Int32.Parse(ClientSession[2]) + ", '" + DateTime.Now + "', " +
                                " NULL, NULL, NULL, NULL, " +
                                "" + pobjDtoEntity.i_Relationship + ", '" + pobjDtoEntity.v_ExploitedMineral + "', " + pobjDtoEntity.i_AltitudeWorkId + ", " + pobjDtoEntity.i_PlaceWorkId + ", " +
                                "'" + pobjDtoEntity.v_NroPoliza + "', " + pobjDtoEntity.v_Deducible + ", " + pobjDtoEntity.i_NroHermanos + ", '" + pobjDtoEntity.v_Password + "', '" + pobjDtoEntity.v_Procedencia + "', " +
                                "'" + pobjDtoEntity.v_CentroEducativo + "', '" + pobjDtoEntity.v_Religion + "', '" + pobjDtoEntity.v_Nacionalidad + "', " +
                                "'" + pobjDtoEntity.v_ResidenciaAnterior + "', '" + pobjDtoEntity.v_Subs + "' )";


                    cnx.Execute(query);
                    var result = UpdateImagesPerson(newId, pobjDtoEntity);
                    pobjOperationResult.Success = 1;
                    if (result != "1")
                    {
                        pobjOperationResult.Success = 0;
                        pobjOperationResult.ErrorMessage = "Sucedió un error al guardar las imagenes.";
                    }
                    
                    return newId;
                }
                
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                return "-1";
            }
        }

        public personCustom GetPerson(ref OperationResult pobjOperationResult, string pstrPersonId)
        {
            //mon.IsActive = true;
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select * from person where v_PersonId = '" + pstrPersonId + "'";
       
                    var objPerson = cnx.Query<personCustom>(query).FirstOrDefault();
                    pobjOperationResult.Success = 1;
                    return objPerson;
                }
    
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public string UpdateImagesPerson(string personId, personCustom pobjDtoEntity)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)ConnectionHelper.GetNewSigesoftConnection;

                SqlCommand com = new SqlCommand("UPDATE person SET b_PersonImage = @PersonImage, b_FingerPrintTemplate = @FingerPrintTemplate, b_FingerPrintImage = @FingerPrintImage, b_RubricImage = @RubricImage, t_RubricImageText = @RubricImageText  WHERE v_PersonId = '" + personId + "'", cmd.Connection);
                if (pobjDtoEntity.b_PersonImage != null)
                {
                    com.Parameters.AddWithValue("@PersonImage", pobjDtoEntity.b_PersonImage);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@PersonImage", SqlDbType.Image);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }

                if (pobjDtoEntity.b_FingerPrintTemplate != null)
                {
                    com.Parameters.AddWithValue("@FingerPrintTemplate", pobjDtoEntity.b_FingerPrintTemplate);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@FingerPrintTemplate", SqlDbType.Image);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }

                if (pobjDtoEntity.b_FingerPrintImage != null)
                {
                    com.Parameters.AddWithValue("@FingerPrintImage", pobjDtoEntity.b_FingerPrintImage);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@FingerPrintImage", SqlDbType.Image);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }

                if (pobjDtoEntity.b_RubricImage != null)
                {
                    com.Parameters.AddWithValue("@RubricImage", pobjDtoEntity.b_RubricImage);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@RubricImage", SqlDbType.Image);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }

                if (pobjDtoEntity.t_RubricImageText != null)
                {
                    com.Parameters.AddWithValue("@RubricImageText", pobjDtoEntity.t_RubricImageText);
                }
                else
                {
                    SqlParameter imageParameter = new SqlParameter("@RubricImageText", SqlDbType.Text);
                    imageParameter.Value = DBNull.Value;
                    com.Parameters.Add(imageParameter);
                }
                cmd.Connection.Open();
                com.ExecuteNonQuery();

                return "1";
            }
            catch (Exception ex)
            {
                return "-1";
            }
        }

        public string FusionServices(ref OperationResult objOperationResult, List<string> servicesId, List<string> ClientSession)
        {
            try
            {

                List<hospitalizacionserviceCustom> ListHospServices = new List<hospitalizacionserviceCustom>();
                List<hospitalizacionserviceCustom> ListHospServicesDistintos = new List<hospitalizacionserviceCustom>();
                List<string> ServicesNoEncontrados = new List<string>();
                
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    #region FindListHospServices
                    foreach (var serviceId in servicesId)
                    {
                        var query = "select * from hospitalizacionservice where v_ServiceId = '" + serviceId +
                                    "' and i_IsDeleted = 0";

                        var List = cnx.Query<hospitalizacionserviceCustom>(query).FirstOrDefault();

                        if (List != null)
                        {
                            ListHospServices.Add(List);
                        }
                        else
                        {
                            ServicesNoEncontrados.Add(serviceId);
                        }

                    }
                    #endregion

                    string HospitalizacionId = "";
                    var objHospitlizacion = ListHospServices.FindAll(x => x.v_HopitalizacionId != null).FirstOrDefault();
                    if (objHospitlizacion != null)
                    {
                        HospitalizacionId = objHospitlizacion.v_HopitalizacionId;
                    }
                    if (HospitalizacionId != "" && HospitalizacionId != null)
                    {
                        //Actualizo la HospitalizacionService con los mismos HospitalizacionId
                        foreach (var HospService in ListHospServices)
                        {
                            if (HospitalizacionId != HospService.v_HopitalizacionId)
                            {
                                UpdateHospService(HospitalizacionId, HospService.v_HospitalizacionServiceId, ClientSession);
                            }

                        }
                    }
                    else
                    {
                        if (ListHospServices.Count > 0)
                        {
                            HospitalizacionId = AddHospitalizacion(ListHospServices[0].v_ServiceId, ClientSession);
                            foreach (var HospService in ListHospServices)
                            {
                                UpdateHospService(HospitalizacionId, HospService.v_HospitalizacionServiceId, ClientSession);
                            }
                        }

                    }
                    if (ServicesNoEncontrados.Count > 0)
                    {
                        //Agrego una nueva HospitalizacionService
                        if (HospitalizacionId != "" && HospitalizacionId != null)
                        {
                            foreach (var _serviceId in ServicesNoEncontrados)
                            {
                                string reult = AddHospitalizacionService(_serviceId, HospitalizacionId, ClientSession);
                                if (reult == null)
                                {
                                    throw new Exception("Sucedió un error al generar las nuevas hospitalizaciones services");
                                }
                            }
                        }
                        else //Agrego una nueva Hospitalizacion
                        {
                            string _HospitalizacionId = AddHospitalizacion(ServicesNoEncontrados[0], ClientSession);
                            foreach (var serviceId in ServicesNoEncontrados)
                            {
                                if (_HospitalizacionId != null)
                                {
                                    //Agrego la hospitalizacionService
                                    string reult = AddHospitalizacionService(serviceId, _HospitalizacionId, ClientSession);
                                    if (reult == null)
                                    {
                                        throw new Exception("Sucedió un error al generar las nuevas hospitalizaciones services");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Sucedió un error al generar las nuevas hospitalizaciones");
                                }
                            }
                        }
                    }
                }

                objOperationResult.Success = 1;
                return "ok";
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.ErrorMessage = ex.Message;
                return null;
            }
        }


        public string AddHospitalizacionService(string serviceId, string hospitalizacionId, List<string> ClientSession)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var SecuentialId = Utilidades.GetNextSecuentialId(Int32.Parse(ClientSession[0]), 351);
                    var newId = Utilidades.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "HS");

                    var insert = "INSERT INTO hospitalizacionservice(v_HospitalizacionServiceId, v_HopitalizacionId, v_ServiceId, i_IsDeleted, i_InsertUserId, d_InsertDate, i_UpdateUserId, d_UpdateDate, v_ComentaryUpdate)" +
                                 "VALUES ('"+ newId +"', '"+ hospitalizacionId +"', '"+ serviceId +"', 0, "+ int.Parse(ClientSession[2]) +", '"+ DateTime.Now +"', NULL, NULL, NULL)";
                    cnx.Execute(insert);
                }

                return "ok";
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public void UpdateHospService(string HospitalizacionId, string HospitalizacionServiceId, List<string> ClientSession)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var update = "UPDATE hospitalizacionservice " +
                                 "SET v_HopitalizacionId = '"+ HospitalizacionId +"' , d_UpdateDate = '"+ DateTime.Now +"', i_UpdateUserId = "+ int.Parse(ClientSession[2]) +" " +
                                 "WHERE v_HospitalizacionServiceId = '" + HospitalizacionServiceId + "'";

                    cnx.Execute(update);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string AddHospitalizacion(string serviceId, List<string> ClientSession)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {

                    var qCalendar = "select * from calendar where v_ServiceId = '"+ serviceId +"' and i_IsDeleted = 0";
                    var objCalendar = cnx.Query<calendarCustom>(qCalendar).FirstOrDefault();

                    var SecuentialId = Utilidades.GetNextSecuentialId(Int32.Parse(ClientSession[0]), 350);
                    var newId = Utilidades.GetNewId(int.Parse(ClientSession[0]), SecuentialId, "HP");

                    var query = "INSERT INTO hospitalizacion (v_HopitalizacionId, v_PersonId, d_FechaIngreso, d_FechaAlta, v_Comentario, v_NroLiquidacion, i_IsDeleted, i_InsertUserId, d_InsertDate, i_UpdateUserId, d_UpdateDate, d_PagoMedico, i_MedicoPago, d_PagoPaciente, i_PacientePago, v_ComentaryUpdate) " +
                                "VALUES ('" + newId + "', '"+objCalendar.v_PersonId+"', '" + objCalendar.d_EntryTimeCM.Value + "', NULL, NULL, NULL, 0, "+ int.Parse(ClientSession[2]) +", '"+DateTime.Now+"', NULL, NULL, NULL, NULL, NULL, NULL, NULL)";
                    cnx.Execute(query);

                    return newId;
                }

            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public string GetComentaryUpdateByPersonId(string personId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = "select * from person where v_PersonId = '" + personId + "'";
                var objperson = cnx.Query<personCustom>(query).FirstOrDefault();

                return objperson.v_ComentaryUpdate;
            }

        }

        public List<Professional> GetListProfessionales(ref OperationResult pobjOperationResult, int pintResultsPerPage, string Filtro)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "SELECT b.v_FirstLastName + ' ' + v_SecondLastName + ', ' + b.v_FirstName as 'Nombre', " +
                                " a.v_UserName as 'Usuario', " +
                                " a.i_SystemUserId as 'IdUsuario', " +
                                " b.v_PersonId as 'PersonId', " +
                                " ISNULL(p.v_ProfessionalCode, '- - -') as 'ProfessionalCode', " +
                                " ISNULL(p.v_ProfessionalInformation, '- - -') as 'ProfessionalInformation', " +
                                " ISNULL(p.i_GrupoHorario, 0) as 'i_GrupoHorario', " +
                                " p.i_ProfessionId as 'ProfessionalId', " +
                                " CASE WHEN p.i_GrupoHorario IS NULL THEN 'SIN HORARIO' ELSE 'HORARIO' END 'v_GrupoHorario', " +
                                " ISNULL(p.i_CodigoProfesion, 0) as 'i_CodigoProfesion', " +
                                " CASE WHEN p.i_CodigoProfesion IS NULL THEN 'SIN PROFESION' ELSE sp.v_Value1 END AS 'v_CodigoProfesion' " +
                                " FROM systemuser a " +
                                " INNER JOIN person b on a.v_PersonId = b.v_PersonId " +
                                " INNER JOIN professional p on b.v_PersonId=p.v_PersonId " +
                                " LEFT JOIN systemparameter sp on sp.i_GroupId = 373 and p.i_CodigoProfesion = sp.i_ParameterId" +
                                " inner join datahierarchy dt on p.i_ProfessionId=dt.i_ItemId and dt.i_GroupId=101 and i_ProfessionId in (30, 31, 32, 34) " +
                                " WHERE a.i_IsDeleted = 0 and b.i_IsDeleted = 0 and p.i_IsDeleted = 0  ";
                    var List = cnx.Query<Professional>(query).OrderBy(x => x.Nombre).Take(pintResultsPerPage).ToList();

                    pobjOperationResult.Success = 1;
                    return List;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }


        public List<HorariosListados> ObtenerHorariosProfesionales(string Profesional)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

                var query = @"declare @usuario int= " + Profesional +
                            @" SELECT per.v_PersonId, 
                            su.i_SystemUserId as i_SystemUserId, 
                            hor.IdHorario as IdHorario,
                            per.v_FirstName + ' ' + per.v_FirstLastName + ' ' + per.v_SecondLastName AS 'PROFESIONAL',
                            hor.d_FechaInicio as d_FechaInicio,
                            hor.d_FechaFin as d_FechaFin,
                            CASE WHEN hor.i_01 IS NULL THEN '-' ELSE ISNULL(' X / ' + 
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_01 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_01),'-') END  AS 'i_01', 
                            CASE WHEN hor.i_02 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_02 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_02),'-') END  AS 'i_02', 
                            CASE WHEN hor.i_03 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_03 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_03),'-') END  AS 'i_03', 
                            CASE WHEN hor.i_04 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_04 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_04),'-') END  AS 'i_04', 
                            CASE WHEN hor.i_05 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_05 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_05),'-') END  AS 'i_05', 
                            CASE WHEN hor.i_06 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_06 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_06),'-') END  AS 'i_06',
                            CASE WHEN hor.i_07 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_07 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_07),'-') END  AS 'i_07',
                            CASE WHEN hor.i_08 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_08 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_08),'-') END  AS 'i_08', 
                            CASE WHEN hor.i_09 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_09 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_09),'-') END  AS 'i_09',
                            CASE WHEN hor.i_10 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_10 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_10),'-') END  AS 'i_10',  
                            CASE WHEN hor.i_11 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_11 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_11),'-') END  AS 'i_11', 
                            CASE WHEN hor.i_12 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_12 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_12),'-') END  AS 'i_12',
                            CASE WHEN hor.i_13 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_13 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_13),'-') END  AS 'i_13',  
                            CASE WHEN hor.i_14 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_14 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_14),'-') END  AS 'i_14',
                            CASE WHEN hor.i_15 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_15 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_15),'-') END  AS 'i_15', 
                            CASE WHEN hor.i_16 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_16 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_16),'-') END  AS 'i_16',  
                            CASE WHEN hor.i_17 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_17 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_17),'-') END  AS 'i_17', 
                            CASE WHEN hor.i_18 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_18 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_18),'-') END  AS 'i_18',
                            CASE WHEN hor.i_19 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_19 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_19),'-') END  AS 'i_19',  
                            CASE WHEN hor.i_20 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_20 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_20),'-') END  AS 'i_20', 
                            CASE WHEN hor.i_21 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_21 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_21),'-') END  AS 'i_21', 
                            CASE WHEN hor.i_22 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_22 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_22),'-') END  AS 'i_22', 
                            CASE WHEN hor.i_23 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_23 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_23),'-') END  AS 'i_23', 
                            CASE WHEN hor.i_24 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_24 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_24),'-') END  AS 'i_24',
                            CASE WHEN hor.i_25 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_25 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_25),'-') END  AS 'i_25', 
                            CASE WHEN hor.i_26 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_26 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_26),'-') END  AS 'i_26',
                            CASE WHEN hor.i_27 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_27 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_27),'-') END  AS 'i_27', 
                            CASE WHEN hor.i_28 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_28 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_28),'-') END  AS 'i_28',   
                            CASE WHEN hor.i_29 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_29 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_29),'-') END  AS 'i_29', 
                            CASE WHEN hor.i_30 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_30 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_30),'-') END  AS 'i_30', 
                            CASE WHEN hor.i_31 IS NULL THEN '-' ELSE ISNULL(' X / ' +
                            (STUFF((SELECT SUBSTRING(c.v_Value1,1,1) + '-' from systemparameter c
                            where c.i_GroupId = hor.i_31 and c.i_IsDeleted = 0 and c.v_Field = '0' FOR XML PATH('')),2,0,'')) + ' / '+CONVERT(varchar, hor.i_31),'-') END  AS 'i_31',
                            hor.v_Comentary as 'v_Comentary'
                            FROM systemuser su
                            join person per on su.v_PersonId = per.v_PersonId
                            JOIN professional prof on su.v_PersonId = prof.v_PersonId
                            join Horarios hor on su.i_SystemUserId = hor.i_SystemUserId
                            where  @usuario is null or su.i_SystemUserId = @usuario";

                var data = cnx.Query<HorariosListados>(query).ToList();
                return data;
            }

        }


        public List<SystemParameter_Horario> GetListTurnos(ref OperationResult pobjOperationResult, int pintResultsPerPage, string Filtro)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select i_GroupId as 'i_GroupId' , " +
                                " i_ParameterId as 'i_ParameterId', " +
                                " v_Value1 as 'v_Value1', " +
                                " v_Value2 as 'v_Value2', " +
                                " v_Field as 'v_Field', " +
                                " i_ParentParameterId as 'i_ParentParameterId', " +
                                " i_IsDeleted as 'i_IsDeleted' " +
                                " from systemparameter " +
                                " where i_GroupId = '" + Filtro + "' and i_IsDeleted = 0 and v_Field = '0' ";
                    var List = cnx.Query<SystemParameter_Horario>(query).OrderBy(x => x.i_ParameterId).Take(pintResultsPerPage).ToList();

                    pobjOperationResult.Success = 1;
                    return List;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<SystemParameter_Horario> GetListTurnosIncludeDelete(ref OperationResult pobjOperationResult, int pintResultsPerPage, string Filtro)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select i_GroupId as 'i_GroupId' , " +
                                " i_ParameterId as 'i_ParameterId', " +
                                " v_Value1 as 'v_Value1', " +
                                " v_Value2 as 'v_Value2', " +
                                " v_Field as 'v_Field', " +
                                " i_ParentParameterId as 'i_ParentParameterId', " +
                                " i_IsDeleted as 'i_IsDeleted' " +
                                " from systemparameter " +
                                " where i_GroupId = '" + Filtro + "' and v_Field = '0' ";
                    var List = cnx.Query<SystemParameter_Horario>(query).OrderBy(x => x.i_ParameterId).Take(pintResultsPerPage).ToList();

                    pobjOperationResult.Success = 1;
                    return List;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<Horarios> GetListHorarios(ref OperationResult pobjOperationResult, int pintResultsPerPage, string Filtro)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = @"select * " +
                                " from Horarios " +
                                " where i_SystemUserId = '" + Filtro + "' and i_IsDeleted = 0 ";
                    var List = cnx.Query<Horarios>(query).OrderBy(x => x.IdHorario).Take(pintResultsPerPage).ToList();

                    pobjOperationResult.Success = 1;
                    return List;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public List<Dias> GetListDias(ref OperationResult pobjOperationResult, int pintResultsPerPage, int Filtro, int diasMes)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = @"DECLARE @idHorario int = " + Filtro +
                                    @"SELECT '01' as 'Dia',
                                    CASE WHEN i_01 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_01,0) as 'Grupo',
                                    '1' as 'Orden' 
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '02' as 'Dia',
                                    CASE WHEN i_02 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_02,0) as 'Grupo',
                                    '2' as 'Orden' 
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '03' as 'Dia',
                                    CASE WHEN i_03 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_03,0) as 'Grupo',
                                    '3' as 'Orden' 
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '04' as 'Dia',
                                    CASE WHEN i_04 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_04,0) as 'Grupo',
                                    '4' as 'Orden' 
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '05' as 'Dia',
                                    CASE WHEN i_05 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_05,0) as 'Grupo',
                                    '5' as 'Orden' 
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '06' as 'Dia',
                                    CASE WHEN i_06 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_06,0) as 'Grupo',
                                    '6' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '07' as 'Dia',
                                    CASE WHEN i_07 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_07,0) as 'Grupo',
                                    '7' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '08' as 'Dia',
                                    CASE WHEN i_08 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_08,0) as 'Grupo',
                                    '8' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '09' as 'Dia',
                                    CASE WHEN i_09 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_09,0) as 'Grupo',
                                    '9' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '10' as 'Dia',
                                    CASE WHEN i_10 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_10,0) as 'Grupo',
                                    '10' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '11' as 'Dia',
                                    CASE WHEN i_11 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_11,0) as 'Grupo',
                                    '11' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '12' as 'Dia',
                                    CASE WHEN i_12 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_12,0) as 'Grupo',
                                    '12' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '13' as 'Dia',
                                    CASE WHEN i_13 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_13,0) as 'Grupo',
                                    '13' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '14' as 'Dia',
                                    CASE WHEN i_14 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_14,0) as 'Grupo',
                                    '14' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '15' as 'Dia',
                                    CASE WHEN i_15 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_15,0) as 'Grupo',
                                    '15' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '16' as 'Dia',
                                    CASE WHEN i_16 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_16,0) as 'Grupo',
                                    '16' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '17' as 'Dia',
                                    CASE WHEN i_17 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_17,0) as 'Grupo',
                                    '17' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '18' as 'Dia',
                                    CASE WHEN i_18 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_18,0) as 'Grupo',
                                    '18' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '19' as 'Dia',
                                    CASE WHEN i_19 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_19,0) as 'Grupo',
                                    '19' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '20' as 'Dia',
                                    CASE WHEN i_20 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_20,0) as 'Grupo',
                                    '20' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '21' as 'Dia',
                                    CASE WHEN i_21 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_21,0) as 'Grupo',
                                    '21' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '22' as 'Dia',
                                    CASE WHEN i_22 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_22,0) as 'Grupo',
                                    '22' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '23' as 'Dia',
                                    CASE WHEN i_23 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_23,0) as 'Grupo',
                                    '23' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '24' as 'Dia',
                                    CASE WHEN i_24 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_24,0) as 'Grupo',
                                    '24' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '25' as 'Dia',
                                    CASE WHEN i_25 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_25,0) as 'Grupo',
                                    '25' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '26' as 'Dia',
                                    CASE WHEN i_26 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_26,0) as 'Grupo',
                                    '26' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '27' as 'Dia',
                                    CASE WHEN i_27 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_27,0) as 'Grupo',
                                    '27' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '28' as 'Dia',
                                    CASE WHEN i_28 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_28,0) as 'Grupo',
                                    '28' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '29' as 'Dia',
                                    CASE WHEN i_29 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_29,0) as 'Grupo',
                                    '29' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '30' as 'Dia',
                                    CASE WHEN i_30 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_30,0) as 'Grupo',
                                    '30' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    UNION

                                    SELECT '31' as 'Dia',
                                    CASE WHEN i_31 IS NULL THEN 'SIN GRUPO HORARIO' ELSE 'CON GRUPO HORARIO' END AS 'ESTADO',
                                    ISNULL(i_31,0) as 'Grupo',
                                    '31' as 'Orden'
                                    FROM HORARIOS WHERE IdHorario = @idHorario

                                    order by 'Orden'";
                    var List = cnx.Query<Dias>(query).OrderBy(x => x.Orden).Take(pintResultsPerPage).ToList();
                    List<Dias> Dias_ = new List<Dias>();
                    for (int i = 1; i <= diasMes; i++)
                    {
                        foreach (var item in List)
                        {
                            if (int.Parse(item.Orden) == i)
                            {
                                Dias_.Add(item);
                            }
                        }
                    }
                    pobjOperationResult.Success = 1;
                    return Dias_;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }


        public List<SystemParameter_Horario> GetListHoras(ref OperationResult pobjOperationResult, int pintResultsPerPage, string grupo, string horas)
        {
            try
            {
                using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
                {
                    var query = "select i_GroupId as 'i_GroupId' , " +
                                " i_ParameterId as 'i_ParameterId', " +
                                " v_Value1 as 'v_Value1', " +
                                " v_Value2 as 'v_Value2', " +
                                " v_Field as 'v_Field', " +
                                " i_ParentParameterId as 'i_ParentParameterId', " +
                                " i_IsDeleted as 'i_IsDeleted', " +
                                " v_ComentaryUpdate as 'v_ComentaryUpdate' " +
                                " from systemparameter " +
                                " where i_GroupId = '" + grupo + "' and i_IsDeleted = 0 and v_Field = '0-0'  and i_ParentParameterId = '" + horas + "' order by i_ParameterId";
                    var List = cnx.Query<SystemParameter_Horario>(query).OrderBy(x => x.i_ParameterId).Take(pintResultsPerPage).ToList();

                    pobjOperationResult.Success = 1;
                    return List;
                }
            }
            catch (Exception ex)
            {
                pobjOperationResult.Success = 0;
                pobjOperationResult.ExceptionMessage = ex.Message;
                return null;
            }
        }

        public int CreateGrupoHorarioMedico(string v_personId, string horario, string dia)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                string dia_code = "";
                if (dia == "01" || dia == "1") dia_code = "i_01";
                else if (dia == "02" || dia == "2") dia_code = "i_02";
                else if (dia == "03" || dia == "3") dia_code = "i_03";
                else if (dia == "04" || dia == "4") dia_code = "i_04";
                else if (dia == "05" || dia == "5") dia_code = "i_05";
                else if (dia == "06" || dia == "6") dia_code = "i_06";
                else if (dia == "07" || dia == "7") dia_code = "i_07";
                else if (dia == "08" || dia == "8") dia_code = "i_08";
                else if (dia == "09" || dia == "9") dia_code = "i_09";
                else if (dia == "10") dia_code = "i_10";
                else if (dia == "11") dia_code = "i_11";
                else if (dia == "12") dia_code = "i_12";
                else if (dia == "13") dia_code = "i_13";
                else if (dia == "14") dia_code = "i_14";
                else if (dia == "15") dia_code = "i_15";
                else if (dia == "16") dia_code = "i_16";
                else if (dia == "17") dia_code = "i_17";
                else if (dia == "18") dia_code = "i_18";
                else if (dia == "19") dia_code = "i_19";
                else if (dia == "20") dia_code = "i_20";
                else if (dia == "21") dia_code = "i_21";
                else if (dia == "22") dia_code = "i_22";
                else if (dia == "23") dia_code = "i_23";
                else if (dia == "24") dia_code = "i_24";
                else if (dia == "25") dia_code = "i_25";
                else if (dia == "26") dia_code = "i_26";
                else if (dia == "27") dia_code = "i_27";
                else if (dia == "28") dia_code = "i_28";
                else if (dia == "29") dia_code = "i_29";
                else if (dia == "30") dia_code = "i_30";
                else if (dia == "31") dia_code = "i_31";

                string ultimoQuery = "select Top 1 ( i_ParameterId + 1) as 'i_GroupId' from systemparameter  where i_GroupId = 0 order by i_ParameterId desc";
                int ultimo = cnx.Query<int>(ultimoQuery).FirstOrDefault();

                var query = "INSERT INTO systemparameter values(0, " + ultimo.ToString() + ", 'HORARIO - " + v_personId + " - " + horario + " - " + dia + "', '', '', -1,null,0,11,'" + DateTime.Now.ToString() + "', null,null,null)";
                cnx.Execute(query);

                var updateHorarioMed = "UPDATE Horarios set " + dia_code + " = '" + ultimo + "' where IdHorario = '" + horario + "'";
                cnx.Execute(updateHorarioMed);

                return ultimo;
            }
        }

        public int ClonarHorarioMedico(string v_personId, string horario, string dia, int grupo)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                string dia_code = "";
                if (dia == "01" || dia == "1") dia_code = "i_01";
                else if (dia == "02" || dia == "2") dia_code = "i_02";
                else if (dia == "03" || dia == "3") dia_code = "i_03";
                else if (dia == "04" || dia == "4") dia_code = "i_04";
                else if (dia == "05" || dia == "5") dia_code = "i_05";
                else if (dia == "06" || dia == "6") dia_code = "i_06";
                else if (dia == "07" || dia == "7") dia_code = "i_07";
                else if (dia == "08" || dia == "8") dia_code = "i_08";
                else if (dia == "09" || dia == "9") dia_code = "i_09";
                else if (dia == "10") dia_code = "i_10";
                else if (dia == "11") dia_code = "i_11";
                else if (dia == "12") dia_code = "i_12";
                else if (dia == "13") dia_code = "i_13";
                else if (dia == "14") dia_code = "i_14";
                else if (dia == "15") dia_code = "i_15";
                else if (dia == "16") dia_code = "i_16";
                else if (dia == "17") dia_code = "i_17";
                else if (dia == "18") dia_code = "i_18";
                else if (dia == "19") dia_code = "i_19";
                else if (dia == "20") dia_code = "i_20";
                else if (dia == "21") dia_code = "i_21";
                else if (dia == "22") dia_code = "i_22";
                else if (dia == "23") dia_code = "i_23";
                else if (dia == "24") dia_code = "i_24";
                else if (dia == "25") dia_code = "i_25";
                else if (dia == "26") dia_code = "i_26";
                else if (dia == "27") dia_code = "i_27";
                else if (dia == "28") dia_code = "i_28";
                else if (dia == "29") dia_code = "i_29";
                else if (dia == "30") dia_code = "i_30";
                else if (dia == "31") dia_code = "i_31";

                string ultimoQuery = "select Top 1 ( i_ParameterId + 1) as 'i_GroupId' from systemparameter  where i_GroupId = 0 order by i_ParameterId desc";
                int ultimo = cnx.Query<int>(ultimoQuery).FirstOrDefault();

                var query = "INSERT INTO systemparameter values(0, " + ultimo.ToString() + ", 'HORARIO - " + v_personId + " - " + horario + " - " + dia + "', '', '', -1,null,0,11,'" + DateTime.Now.ToString() + "', null,null,null)";
                cnx.Execute(query);

                var updateHorarioMed = "UPDATE Horarios set " + dia_code + " = '" + ultimo + "' where IdHorario = '" + horario + "'";
                cnx.Execute(updateHorarioMed);

                return ultimo;
            }
        }

        public int DeleteGrupoHorarioMedico(string v_personId, string horario, string dia)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                string dia_code = "";
                if (dia == "01" || dia == "1") dia_code = "i_01";
                else if (dia == "02" || dia == "2") dia_code = "i_02";
                else if (dia == "03" || dia == "3") dia_code = "i_03";
                else if (dia == "04" || dia == "4") dia_code = "i_04";
                else if (dia == "05" || dia == "5") dia_code = "i_05";
                else if (dia == "06" || dia == "6") dia_code = "i_06";
                else if (dia == "07" || dia == "7") dia_code = "i_07";
                else if (dia == "08" || dia == "8") dia_code = "i_08";
                else if (dia == "09" || dia == "9") dia_code = "i_09";
                else if (dia == "10") dia_code = "i_10";
                else if (dia == "11") dia_code = "i_11";
                else if (dia == "12") dia_code = "i_12";
                else if (dia == "13") dia_code = "i_13";
                else if (dia == "14") dia_code = "i_14";
                else if (dia == "15") dia_code = "i_15";
                else if (dia == "16") dia_code = "i_16";
                else if (dia == "17") dia_code = "i_17";
                else if (dia == "18") dia_code = "i_18";
                else if (dia == "19") dia_code = "i_19";
                else if (dia == "20") dia_code = "i_20";
                else if (dia == "21") dia_code = "i_21";
                else if (dia == "22") dia_code = "i_22";
                else if (dia == "23") dia_code = "i_23";
                else if (dia == "24") dia_code = "i_24";
                else if (dia == "25") dia_code = "i_25";
                else if (dia == "26") dia_code = "i_26";
                else if (dia == "27") dia_code = "i_27";
                else if (dia == "28") dia_code = "i_28";
                else if (dia == "29") dia_code = "i_29";
                else if (dia == "30") dia_code = "i_30";
                else if (dia == "31") dia_code = "i_31";


                var updateHorarioMed = "UPDATE Horarios set " + dia_code + " = NULL where IdHorario = '" + horario + "'";
                cnx.Execute(updateHorarioMed);

                return 1;
            }
        }


        public int CreateGrupoHorarioDia(string v_personId)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                string ultimoQuery = "select Top 1 ( i_ParameterId + 1) as 'i_GroupId' from systemparameter  where i_GroupId = 0 order by i_ParameterId desc";
                int ultimo = cnx.Query<int>(ultimoQuery).FirstOrDefault();

                var query = "INSERT INTO systemparameter values(0, " + ultimo.ToString() + ", 'HORARIO - " + v_personId + "', '', '', -1,null,0,11,'" + DateTime.Now.ToString() + "', null,null,null)";
                cnx.Execute(query);

                var updateHorarioMed = "UPDATE professional set i_GrupoHorario = '" + ultimo + "' where v_PersonId = '" + v_personId + "'";
                cnx.Execute(updateHorarioMed);

                return ultimo;
            }
        }

        public int CreateTurno(string grupoId, string detalle)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                string ultimoQuery = "IF (SELECT COUNT(*) FROM systemparameter WHERE i_GroupId = '" + grupoId + "' ) = 0  select 1 ELSE select Top 1 ( i_ParameterId + 1) from systemparameter  where i_GroupId = '" + grupoId + "' order by i_ParameterId desc ";
                int ultimo = cnx.Query<int>(ultimoQuery).FirstOrDefault();

                var query = "INSERT INTO systemparameter values('" + grupoId + "', " + ultimo.ToString() + ", '" + detalle + "', '1', '0', -1,null,0,11,'" + DateTime.Now.ToString() + "', null,null,null)";
                cnx.Execute(query);

                return ultimo;
            }
        }

        public int ClonarTurno(string grupoId, string grupoaClonar)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                string ultimoQuery = "select i_ParameterId as i_ParameterId, v_Value1 as v_Value1, i_ParentParameterId as  i_ParentParameterId, v_Field as v_Field, i_IsDeleted as i_IsDeleted from systemparameter where i_GroupId = '" + grupoaClonar + "'  ";
                var ListaHorarios = cnx.Query<HorarioClonar>(ultimoQuery).ToList();

                foreach (var item in ListaHorarios)
                {
                    var query = "INSERT INTO systemparameter values('" + grupoId + "', " + item.i_ParameterId.ToString() + ", '" + item.v_Value1 + "', '1', '" + item.v_Field + "', " + item.i_ParentParameterId + ",null," + item.i_IsDeleted + ",11,'" + DateTime.Now.ToString() + "', null,null,null)";
                    cnx.Execute(query);
                }

                return ListaHorarios[ListaHorarios.Count - 1].i_ParameterId; ;
            }
        }

        public int CreateHorarios(Horarios horarios)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = @"INSERT INTO Horarios (i_SystemUserId, d_FechaInicio, d_FechaFin, v_Comentary, i_IsDeleted,i_InsertUserId, d_InsertDate ) 
                            values(" + horarios.i_SystemUserId + ", '" + horarios.d_FechaInicio + "', '" + horarios.d_FechaFin + "', '" + horarios.v_Comentary + "', " + horarios.i_IsDeleted + ", " + horarios.i_InsertUserId + ", '" + horarios.d_InsertDate + "')";
                cnx.Execute(query);

                return 1;
            }
        }

        public int UpdateHorarios(Horarios horarios)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = @"UPDATE Horarios SET d_FechaInicio = '" + horarios.d_FechaInicio + "', d_FechaFin = '" + horarios.d_FechaFin + "', v_Comentary  = '" + horarios.v_Comentary + "', i_UpdateUserId = " + horarios.i_UpdateUserId + " , d_UpdateDate = '" + horarios.d_UpdateDate + "' where IdHorario = '" + horarios.IdHorario + "'";
                cnx.Execute(query);

                return 1;
            }
        }

        public int DeleteHorarios(int horario, int usuario)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var query = @"UPDATE Horarios SET i_IsDeleted = 1 , i_UpdateUserId = " + usuario + " , d_UpdateDate = '" + DateTime.Now + "' where IdHorario = '" + horario + "'";
                cnx.Execute(query);

                return 1;
            }
        }
        public int EditarDetalle(string grupoId, string detalle, string parametro, string descrpcion)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var updateHorarioMed = "UPDATE systemparameter set v_Value1 = '" + detalle + "', v_ComentaryUpdate = '" + descrpcion + "' where i_GroupId = '" + grupoId + "' and i_ParameterId = '" + parametro + "'";
                cnx.Execute(updateHorarioMed);

                return 1;
            }
        }

        public int EditarProfesional(string personId, int GRUPO)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var updateHorarioMed = "UPDATE professional set i_CodigoProfesion = " + GRUPO + " where v_PersonId = '" + personId + "'";
                cnx.Execute(updateHorarioMed);

                return 1;
            }
        }

        //public ValidacionUltimoComprobante GetEstadoUltimoComprobante(string v_SerieDocumento)
        //{
        //    try
        //    {
        //        if (v_SerieDocumento == "BV01" || v_SerieDocumento == "FT01" || v_SerieDocumento == "BPP1" || v_SerieDocumento == "F002" || v_SerieDocumento == "B003" || v_SerieDocumento == "BPP1")
        //        {
        //            using (var cnx = ConnectionHelper.GetNewContasolConnection)
        //            {
        //                if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

        //                var query = "select TOP 1 ISNULL(v_EnlaceEnvio,'NO DESCARGA') AS EstadoComprobante from venta where v_SerieDocumento = '" + v_SerieDocumento + "' and i_Eliminado = 0 ORDER BY v_IdVenta desc";
        //                return cnx.Query<ValidacionUltimoComprobante>(query).FirstOrDefault();
        //            }
        //        }
        //        else
        //        {
        //            ValidacionUltimoComprobante _ValidacionUltimoComprobante = new ValidacionUltimoComprobante();

        //            _ValidacionUltimoComprobante.EstadoComprobante = "- - -";
        //            return _ValidacionUltimoComprobante;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        public int EliminarDetalle(string grupoId, string parametro)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var updateHorarioMed = "UPDATE systemparameter set i_IsDeleted = 1 where i_GroupId = '" + grupoId + "' and i_ParameterId = '" + parametro + "'";
                cnx.Execute(updateHorarioMed);

                return 1;
            }
        }

        public int EliminarDetalleDeBaja2(string grupoId, string parametro)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var updateHorarioMed = "UPDATE systemparameter set i_IsDeleted = 1, v_Value1 = '(XX) ' + v_Value1 where i_GroupId = '" + grupoId + "' and i_ParameterId = '" + parametro + "'";
                cnx.Execute(updateHorarioMed);

                return 1;
            }
        }

        public int EliminarDetalleDeBaja(string grupoId, string parametro)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                var updateHorarioMed = "UPDATE systemparameter set i_IsDeleted = 1, v_Value1 = '(X) ' + v_Value1 where i_GroupId = '" + grupoId + "' and i_ParameterId = '" + parametro + "'";
                cnx.Execute(updateHorarioMed);

                return 1;
            }
        }


        public int CreateHorario(string grupoId, string detalle, string parent, string descripcion)
        {
            using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
            {
                string ultimoQuery = "IF (SELECT COUNT(*) FROM systemparameter WHERE i_GroupId = '" + grupoId + "' ) = 0  select 1 ELSE select Top 1 ( i_ParameterId + 1) from systemparameter  where i_GroupId = '" + grupoId + "' order by i_ParameterId desc ";
                int ultimo = cnx.Query<int>(ultimoQuery).FirstOrDefault();

                var query = "INSERT INTO systemparameter values('" + grupoId + "', " + ultimo.ToString() + ", '" + detalle + "', '1', '0-0', '" + parent + "',null,0,11,'" + DateTime.Now.ToString() + "', null,null, '" + descripcion + "')";
                cnx.Execute(query);

                return ultimo;
            }
        }

    }
}
