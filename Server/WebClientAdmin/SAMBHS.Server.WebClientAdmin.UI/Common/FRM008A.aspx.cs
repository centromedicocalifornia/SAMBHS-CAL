using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using FineUI;


namespace SAMBHS.Server.WebClientAdmin.UI.Common
{
    public partial class FRM008A : System.Web.UI.Page
    {
        OrganizationBL _objOrganizationBL = new OrganizationBL();

        #region Properties

        public string OrganizationId
        {
            get
            {
                if (Request.QueryString["i_OrganizationId"] != null)
                {
                    string _organizatonId = Request.QueryString["i_OrganizationId"].ToString();
                    if (!string.IsNullOrEmpty(_organizatonId))
                    {
                        return _organizatonId;
                    }
                }

                return "";
            }
        }

        public string Mode
        {
            get
            {
                if (Request.QueryString["Mode"] != null)
                {
                    string _mode = Request.QueryString["Mode"].ToString();
                    if (!string.IsNullOrEmpty(_mode))
                    {
                        return _mode;
                    }
                }

                return string.Empty;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
                btnClose.OnClientClick = ActiveWindow.GetConfirmHideReference();

            }

        }
       
        private bool IsValidNodeName(string pstrRuc)
        {
            // Validar existencia de un Organization
            OperationResult objOperationResult6 = new OperationResult();
            string filterExpression = string.Format("v_IdentificationNumber==\"{0}\"", pstrRuc);
            var recordCount = _objOrganizationBL.GetorganizationCount(ref objOperationResult6, filterExpression);

            if (recordCount != 0)
            {
                Alert.ShowInTop("El nombre de Organization  <font color='red'>" + pstrRuc + "</font> ya se encuentra registrado.<br> Por favor ingrese otro nombre de Organization.");
                return false;
            }
            return true;
        }

        protected void btnSaveRefresh_Click(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                #region Validate

                // Validar Empresa
                if (!IsValidNodeName(txtIdentificationNumber.Text.Trim().ToUpper()))
                {
                    return;
                }

                #endregion

                // Datos de Organization
                organizationDto objOrganizationDTO = new organizationDto();
                objOrganizationDTO.v_Name = txtName.Text.Trim().ToUpper();
                objOrganizationDTO.v_IdentificationNumber = txtIdentificationNumber.Text.Trim().ToUpper();
                objOrganizationDTO.v_Address = txtAddress.Text.Trim().ToUpper();
                objOrganizationDTO.v_PhoneNumber = txtPhoneNumber.Text;
                objOrganizationDTO.v_Mail = txtMail.Text.Trim();
                objOrganizationDTO.v_ContacName = txtContacName.Text;
                OperationResult objOperationResult1 = new OperationResult();
                // Graba Organization
                _objOrganizationBL.Addorganization(ref objOperationResult1, objOrganizationDTO, ((ClientSession)Session["objClientSession"]).GetAsList());

                if (objOperationResult1.Success != 1)
                {
                    Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult1.ExceptionMessage);
                }

            }
            else if (Mode == "Edit")
            {
                organizationDto objOrganizationDTO = Session["sobjOrganizationDTO"] as organizationDto;

                #region Validate Node

                // Almacenar temporalmente el nombre de Organization
                var _nodeRucTemp = objOrganizationDTO.v_IdentificationNumber;

                if (txtIdentificationNumber.Text != _nodeRucTemp)
                {
                    // Validar Organization
                    if (!IsValidNodeName(txtIdentificationNumber.Text.Trim().ToUpper()))
                    {
                        return;
                    }
                }

                #endregion

                // Datos de Organization
                objOrganizationDTO.v_Name = txtName.Text.Trim().ToUpper();
                objOrganizationDTO.v_IdentificationNumber = txtIdentificationNumber.Text.Trim().ToUpper();
                objOrganizationDTO.v_Address = txtAddress.Text.Trim().ToUpper();
                objOrganizationDTO.v_PhoneNumber = txtPhoneNumber.Text;
                objOrganizationDTO.v_Mail = txtMail.Text.Trim();
                objOrganizationDTO.v_ContacName = txtContacName.Text;

                // Actualiza Organization
                OperationResult objOperationResult1 = new OperationResult();
                _objOrganizationBL.Updateorganization(ref objOperationResult1, objOrganizationDTO, ((ClientSession)Session["objClientSession"]).GetAsList());

                if (objOperationResult1.Success != 1)
                {
                    Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult1.ExceptionMessage);
                }

            }

            // Cerrar página actual y hacer postback en el padre para actualizar
            PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());

        }

        private void LoadData()
        {
            if (Mode == "New")
            {
                // Additional logic here.

            }
            else if (Mode == "Edit")
            {
                OperationResult objCommonOperationResultedit = new OperationResult();
                organizationDto objOrganizationDTO = _objOrganizationBL.GetorganizationByorganizationId(ref objCommonOperationResultedit, int.Parse(OrganizationId));

                Session["sobjOrganizationDTO"] = objOrganizationDTO;

                // Informacion del Organization
                txtName.Text = objOrganizationDTO.v_Name ;
                txtIdentificationNumber.Text = objOrganizationDTO.v_IdentificationNumber;
                txtAddress.Text = objOrganizationDTO.v_Address;
                txtPhoneNumber.Text = objOrganizationDTO.v_PhoneNumber;
                txtMail.Text = objOrganizationDTO.v_Mail;
                txtContacName.Text = objOrganizationDTO.v_ContacName;

            }
        }
    }
}