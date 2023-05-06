using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using SAMBHS.Common.BL;
using SAMBHS.Common.BE;
using FineUI;


namespace SAMBHS.Server.WebClientAdmin.UI.Security
{
    public partial class FRM006A : System.Web.UI.Page
    {

        #region Declarations
            SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
            PersonBL _objPersonBL = new PersonBL();
            SecurityBL _objSecurityBL = new SecurityBL();
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
                btnClose.OnClientClick = ActiveWindow.GetConfirmHideReference();
            }

        }

        private void LoadComboBox()
        {
            OperationResult objOperationResult = new OperationResult();

            var _DocType = _objSystemParameterBL.GetSystemParameterForCombo(ref objOperationResult, 150);

            Utils.Web.LoadDropDownList(ddlDocType, "Value1", "Id", _DocType, DropDownListAction.Select);
            Utils.Web.LoadDropDownList(ddlSexType, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForCombo(ref objOperationResult, 100), DropDownListAction.Select);
            Utils.Web.LoadDropDownList(ddlMaritalStatus, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForCombo(ref objOperationResult, 101), DropDownListAction.Select);
            Utils.Web.LoadDropDownList(ddlLevelOfId, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForCombo(ref objOperationResult, 151), DropDownListAction.Select);
            Utils.Web.LoadDropDownList(ddlRole, "Value1", "Id", _objSystemParameterBL.GetRole(ref objOperationResult, ""), DropDownListAction.Select);

            Session["objDocType"] = _DocType;

        }

        private bool IsValidDocumentNumberLenght()
        {
            var lenght = Convert.ToInt32(ViewState["lenght"]);
            if (txtDocNumber.Text.Trim().Length < lenght || txtDocNumber.Text.Trim().Length > lenght)
            {
                txtDocNumber.MarkInvalid(String.Format("El número de Carateres requeridos es {0}", lenght));
                return false;
            }
            return true;
        }

        protected void btnSaveRefresh_Click(object sender, EventArgs e)
        {
            string Mode = Request.QueryString["Mode"].ToString();
            int personId = 0;
            int systemUserId = -1;

            int intCurrentExecutionNodeId = ((ClientSession)Session["objClientSession"]).i_CurrentExecutionNodeId;
            int intCurrentOrganizationId = ((ClientSession)Session["objClientSession"]).i_CurrentOrganizationId;

            if (Mode == "New")
            {
                #region Validations
                // Validar la longitud de los numeros de documentos
                //if (!IsValidDocumentNumberLenght())
                //{
                //    return;
                //}
                #endregion

                // Datos de persona
                personDto objPerson = new personDto();
                objPerson.v_FirstName = txtFirstName.Text.Trim().ToUpper();
                objPerson.v_FirstLastName = txtFirstLastName.Text.Trim().ToUpper();
                objPerson.v_SecondLastName = txtSecondLastName.Text.Trim().ToUpper();
                objPerson.i_DocTypeId = Convert.ToInt32(ddlDocType.SelectedValue);
                objPerson.i_SexTypeId = Convert.ToInt32(ddlSexType.SelectedValue);
                objPerson.i_MaritalStatusId = Convert.ToInt32(ddlMaritalStatus.SelectedValue);
                objPerson.i_LevelOfId = Convert.ToInt32(ddlLevelOfId.SelectedValue);
                objPerson.v_DocNumber = txtDocNumber.Text.Trim();
                //objPerson.d_Birthdate = dpBirthdate.SelectedDate;
                //objPerson.v_BirthPlace = txtBirthPlace.Text.Trim().ToUpper();
                objPerson.v_TelephoneNumber = txtTelephoneNumber.Text.Trim();
                objPerson.v_AdressLocation = txtAdressLocation.Text.Trim().ToUpper();
                //objPerson.v_Mail = txtMail.Text.Trim();


                // Datos de usuario
                systemuserDto pobjSystemUser = new systemuserDto();
                pobjSystemUser.i_PersonId = personId;
                pobjSystemUser.v_UserName = txtUserName.Text.Trim();
                pobjSystemUser.v_Password = Utils.Encrypt(txtPassword2.Text.Trim());
                pobjSystemUser.i_RoleId = Convert.ToInt32(ddlRole.SelectedValue);

                // Graba persona      
                OperationResult objOperationResult1 = new OperationResult();
                personId = _objPersonBL.AddPerson(ref objOperationResult1,
                                                          objPerson,
                    //objProfessional,
                                                          pobjSystemUser,
                                                          ((ClientSession)Session["objClientSession"]).GetAsList());

                if (personId == -1)
                {
                    Alert.ShowInTop(objOperationResult1.ErrorMessage);
                    return;
                }
                else
                {
                    if (objOperationResult1.Success != 1)
                    {
                        Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult1.ExceptionMessage);
                        return;
                    }
                }

            }
            else if (Mode == "Edit")
            {

                if (Request.QueryString["personId"] != null)
                    personId = int.Parse(Request.QueryString["personId"].ToString());
                if (Request.QueryString["systemUserId"] != null)
                    systemUserId = int.Parse(Request.QueryString["systemUserId"].ToString());

                systemuserDto systemUser = new systemuserDto();
                personDto _objPerson = Session["objEntity"] as personDto;
                systemuserDto _objSystemUserTemp = Session["objSystemUser"] as systemuserDto;

                #region Validate Document Number Lenght
                // Validar la longitud de los numeros de documentos
                //if (!IsValidDocumentNumberLenght())
                //{
                //    return;
                //}
                #endregion

                bool isChangeUserName = false;
                bool isChangeDocNumber = false;

                #region Validate SystemUSer
                // Almacenar temporalmente el nombre de usuario actual
                var _userNameTemp = _objSystemUserTemp.v_UserName;
                if (txtUserName.Text != _userNameTemp)
                {
                    isChangeUserName = true;
                }
                #endregion

                #region Validate Document Number
                // Almacenar temporalmente el número de documento del usuario actual
                var _docNumberTemp = _objPerson.v_DocNumber;
                if (txtDocNumber.Text != _docNumberTemp)
                {
                    isChangeDocNumber = true;
                }
                #endregion

                // Datos de persona
                _objPerson.v_FirstName = txtFirstName.Text.Trim().ToUpper();
                _objPerson.v_FirstLastName = txtFirstLastName.Text.Trim().ToUpper();
                _objPerson.v_SecondLastName = txtSecondLastName.Text.Trim().ToUpper();
                _objPerson.i_DocTypeId = Convert.ToInt32(ddlDocType.SelectedValue);
                _objPerson.i_SexTypeId = Convert.ToInt32(ddlSexType.SelectedValue);
                _objPerson.i_MaritalStatusId = Convert.ToInt32(ddlMaritalStatus.SelectedValue);
                _objPerson.i_LevelOfId = Convert.ToInt32(ddlLevelOfId.SelectedValue);
                _objPerson.v_DocNumber = txtDocNumber.Text;
                //_objPerson.d_Birthdate = dpBirthdate.SelectedDate;
                //_objPerson.v_BirthPlace = txtBirthPlace.Text.Trim().ToUpper();
                _objPerson.v_TelephoneNumber = txtTelephoneNumber.Text;
                _objPerson.v_AdressLocation = txtAdressLocation.Text.Trim().ToUpper();
                //_objPerson.v_Mail = txtMail.Text;
                _objPerson.i_UpdateNodeId = ((ClientSession)Session["objClientSession"]).i_CurrentExecutionNodeId;

                // Almacenar temporalmente el password del usuario actual
                var _passTemp = _objSystemUserTemp.v_Password;

                // Si el password actual es diferente al ingresado en la cajita de texto, quiere decir que se ha cambiado el password por lo tanto
                // se bede encriptar el nuevo password
                if (txtPassword2.Text != _passTemp)
                {
                    systemUser.v_Password = Utils.Encrypt(txtPassword2.Text.Trim());
                }
                else
                {
                    systemUser.v_Password = txtPassword2.Text.Trim();
                }

                // Datos de Usuario
                systemUser.i_SystemUserId = _objSystemUserTemp.i_SystemUserId;
                systemUser.i_PersonId = personId;
                systemUser.v_UserName = txtUserName.Text;
                systemUser.i_RoleId = Convert.ToInt32(ddlRole.SelectedValue);
                systemUser.d_InsertDate = _objSystemUserTemp.d_InsertDate;
                systemUser.i_InsertUserId = _objSystemUserTemp.i_SystemUserId;
                systemUser.i_IsDeleted = _objSystemUserTemp.i_IsDeleted;

                // Actualiza persona
                OperationResult objOperationResult1 = new OperationResult();
                _objPersonBL.UpdatePerson(ref objOperationResult1,
                                                isChangeDocNumber,
                                                _objPerson,
                                                isChangeUserName,
                                                systemUser,
                                                ((ClientSession)Session["objClientSession"]).GetAsList());

                if (objOperationResult1.ErrorMessage != null)
                {
                    Alert.ShowInTop(objOperationResult1.ErrorMessage);
                    return;
                }
                else
                {
                    if (objOperationResult1.Success != 1)
                    {
                        Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult1.ExceptionMessage);
                        return;
                    }
                }
            }

            // Cerrar página actual y hacer postback en el padre para actualizar
            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
        }

        private void LoadData()
        {
            string Mode = Request.QueryString["Mode"].ToString();
            int systemUserId = -1;
            int personId = 0;

            LoadComboBox();

            if (Mode == "New")
            {
                // Additional logic here.

                txtFirstName.Focus(true);

            }
            else if (Mode == "Edit")
            {
                if (Request.QueryString["systemUserId"] != null)
                    systemUserId = int.Parse(Request.QueryString["systemUserId"].ToString());
                if (Request.QueryString["personId"] != null)
                    personId = int.Parse(Request.QueryString["personId"].ToString());

                OperationResult objCommonOperationResultedit = new OperationResult();
                personDto personDTO = _objPersonBL.GetPerson(ref objCommonOperationResultedit, personId);

                Session["objEntity"] = personDTO;

                 //Informacion de la persona
                txtFirstName.Text = personDTO.v_FirstName;
                txtFirstLastName.Text = personDTO.v_FirstLastName;
                txtSecondLastName.Text = personDTO.v_SecondLastName;
                txtDocNumber.Text = personDTO.v_DocNumber;
                //dpBirthdate.SelectedDate = personDTO.d_Birthdate;
                //txtBirthPlace.Text = personDTO.v_BirthPlace;
                ddlMaritalStatus.SelectedValue = personDTO.i_MaritalStatusId.ToString();
                ddlLevelOfId.SelectedValue = personDTO.i_LevelOfId.ToString();
                ddlDocType.SelectedValue = personDTO.i_DocTypeId.ToString();
                ddlSexType.SelectedValue = personDTO.i_SexTypeId.ToString();
                txtTelephoneNumber.Text = personDTO.v_TelephoneNumber;
                txtAdressLocation.Text = personDTO.v_AdressLocation;
                //txtMail.Text = personDTO.v_Mail;

                 //Setear lenght dimamicos de numero de documento
                SetLenght(ddlDocType.SelectedValue);



                // Informacion del usuario
                OperationResult objOperationResult = new OperationResult();
                systemuserDto objSystemUser = _objSecurityBL.GetSystemUser(ref objOperationResult, systemUserId);

                Session["objSystemUser"] = objSystemUser;

                txtUserName.Text = objSystemUser.v_UserName;
                txtPassword1.Text = objSystemUser.v_Password;
                txtPassword2.Text = objSystemUser.v_Password;
                ddlRole.SelectedValue = objSystemUser.i_RoleId.ToString();
            }
        }

        protected void ddlDocType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetLenght(ddlDocType.SelectedValue);
        }

        private void SetLenght(string SelectedValue)
        {
            //if (SelectedValue == "-1") return;

            //var docType = Session["objDocType"] as List<DataForTreeView>;
            // //Buscar la longitud adecuada en funcion al tipo de documento seleccionado
            //var searchResult = docType.Single(p => p.Id == Convert.ToInt32(SelectedValue));
            //ViewState["lenght"] = Convert.ToInt32(searchResult.Description2);
            //txtDocNumber.Text = string.Empty;
            //txtDocNumber.Focus();

        }
    }
}