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
    public partial class FRM007A : System.Web.UI.Page
    {
        //SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        RoleBL _objRoleBL = new RoleBL();

        #region Properties

        public int RoleId
        {
            get
            {
                if (Request.QueryString["roleId"] != null)
                {
                    string _roleId = Request.QueryString["roleId"].ToString();
                    if (!string.IsNullOrEmpty(_roleId))
                    {
                        return Convert.ToInt32(_roleId);
                    }
                }

                return 0;
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

  
        private bool IsValidNodeName(string pstrNode)
        {
            // Validar existencia de un nodo
            OperationResult objOperationResult6 = new OperationResult();
            string filterExpression = string.Format("v_Name==\"{0}\"", pstrNode);
            var recordCount = _objRoleBL.GetRoleCount(ref objOperationResult6, filterExpression);

            if (recordCount != 0)
            {
                Alert.ShowInTop("El nombre de nodo  <font color='red'>" + pstrNode + "</font> ya se encuentra registrado.<br> Por favor ingrese otro nombre de Rol.");
                return false;
            }
            return true;
        }

        protected void btnSaveRefresh_Click(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                #region Validate

                // Validar nodo
                if (!IsValidNodeName(txtName.Text.Trim().ToUpper()))
                {
                    return;
                }

                #endregion

                // Datos de nodo
                roleDto objroleDTO = new roleDto();
                objroleDTO.v_Name = txtName.Text.Trim().ToUpper();
               
                OperationResult objOperationResult1 = new OperationResult();
                // Graba Nodo
                _objRoleBL.AddRole(ref objOperationResult1, objroleDTO, ((ClientSession)Session["objClientSession"]).GetAsList());

                if (objOperationResult1.Success != 1)
                {
                    Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult1.ExceptionMessage);
                }

            }
            else if (Mode == "Edit")
            {
                roleDto objRoleDTO = Session["sobjroleDTO"] as roleDto;

                #region Validate Node

                // Almacenar temporalmente el nombre de nodo
                var _nodeNameTemp = objRoleDTO.v_Name;

                if (txtName.Text != _nodeNameTemp)
                {
                    // Validar nodo
                    if (!IsValidNodeName(txtName.Text.Trim().ToUpper()))
                    {
                        return;
                    }
                }

                #endregion

                // Datos de Role
                objRoleDTO.v_Name = txtName.Text.Trim().ToUpper();
               
                // Actualiza role
                OperationResult objOperationResult1 = new OperationResult();
                _objRoleBL.UpdateRole(ref objOperationResult1, objRoleDTO, ((ClientSession)Session["objClientSession"]).GetAsList());

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
                roleDto objRoleDTO = _objRoleBL.GetRoleById(ref objCommonOperationResultedit, RoleId);

                Session["sobjroleDTO"] = objRoleDTO;

                // Informacion del nodo
                txtName.Text = objRoleDTO.v_Name;

            }
        }
    }
}