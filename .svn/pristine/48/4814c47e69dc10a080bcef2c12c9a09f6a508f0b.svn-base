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
    public partial class FRM001A : System.Web.UI.Page
    {

        SystemParameterBL _objBL = new SystemParameterBL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
                btnClose.OnClientClick = ActiveWindow.GetConfirmHideReference();
            }
        }

        private void LoadData()
        {
            OperationResult objOperationResult = new OperationResult();
            //Llenado de combos
            SAMBHS.Common.Resource.Utils UtilComboBox = new SAMBHS.Common.Resource.Utils();

            string Mode = Request.QueryString["Mode"].ToString();
            int GroupId = -1, ParameterId = -1;

            if (Request.QueryString["i_GroupId"] != null)
                GroupId = int.Parse(Request.QueryString["i_GroupId"].ToString());
            if (Request.QueryString["i_ParameterId"] != null)
                ParameterId = int.Parse(Request.QueryString["i_ParameterId"].ToString());

            //Llenar combo ItemParameter Tree
            ddlParentParameterId.DataTextField = "Description";
            ddlParentParameterId.DataValueField = "Id";
            ddlParentParameterId.DataSimulateTreeLevelField = "Level";
            ddlParentParameterId.DataEnableSelectField = "EnabledSelect";
            List<DataForTreeViewSP> t = _objBL.GetSystemParameterForComboTreeView(ref objOperationResult, GroupId).ToList();
            ddlParentParameterId.DataSource = t;
            ddlParentParameterId.DataBind();
            this.ddlParentParameterId.Items.Insert(0, new FineUI.ListItem("-- Seleccione --", "-1"));

            if (Mode == "New")
            {
                txtGroupId.Enabled = false;
                txtGroupId.Text = "0";
                ddlParentParameterId.Enabled = false;
            }
            else if (Mode == "Edit")
            {

                // Bloquear algunos campos
                txtGroupId.Enabled = false;
                txtParameterId.Enabled = false;
                ddlParentParameterId.Enabled = true;

                // Get the Entity Data
                systemparameterDto objEntity = _objBL.GetSystemParameter(ref objOperationResult, GroupId, ParameterId);

                // Save the entity on the session
                Session["objEntity"] = objEntity;

                // Show the data on the form
                txtGroupId.Text = objEntity.i_GroupId.ToString();
                txtParameterId.Text = objEntity.i_ParameterId.ToString();
                txtDescription.Text = objEntity.v_Value1;
                if (objEntity.i_Sort.HasValue) txtUserInterfaceOrder.Text = objEntity.i_Sort.Value.ToString();
                txtDescription2.Text = objEntity.v_Value2;
                txtField.Text = objEntity.v_Field;

                //Llenado de combos
                //Utils.LoadDropDownList(ddlParentGroupId, "Value1", "Id", _objProxy.GetSystemParameterForCombo(ref objOperationResult, 0), DropDownListAction.Select);
                //Utils.LoadDropDownList(ddlParentParameterId, "Value1", "Id", _objProxy.GetSystemParameterForCombo(ref objOperationResult, (int)objEntity.i_ParentGroupId), DropDownListAction.Select);

                //ddlParentGroupId.SelectedValue = objEntity.i_ParentGroupId.ToString();
                ddlParentParameterId.SelectedValue = objEntity.i_ParentParameterId.ToString();
                txtDescription2.Focus(true);
                if (GroupId == 0) ddlParentParameterId.Enabled = false;
            }
            if (Mode == "NewChildren")
            {
                txtGroupId.Text = GroupId.ToString();
                txtGroupId.Enabled = false;
                txtParameterId.Focus();
            }
        }

        protected void btnSaveRefresh_Click(object sender, EventArgs e)
        {
            string Mode = Request.QueryString["Mode"].ToString();
            OperationResult objOperationResult = new OperationResult();

            //Response.ContentType = "APPLICATION/OCTET-STREAM";
            //String Header = "Attachment; Filename=XMLFile.xml";
            //Response.AppendHeader("Content-Disposition", Header);
            //System.IO.FileInfo Dfile = new System.IO.FileInfo(Server.MapPath("File/Chrysanthemum.jpg"));
            //Response.WriteFile(Dfile.FullName);
            ////Don't forget to add the following line
            //Response.End();


            


            //if (Mode == "New")
            //{
            //    // Create the entity
            //    systemparameterDto objEntity = new systemparameterDto();

            //    // Populate the entity
            //    objEntity.i_GroupId = int.Parse(txtGroupId.Text.Trim());
            //    objEntity.i_ParameterId = int.Parse(txtParameterId.Text.Trim());
            //    objEntity.v_Value1 = txtDescription.Text.Trim().ToUpper();
            //    if (txtUserInterfaceOrder.Text == "") objEntity.i_Sort = null;
            //    else objEntity.i_Sort = int.Parse(txtUserInterfaceOrder.Text.Trim());
            //    objEntity.v_Value2 = txtDescription2.Text.Trim().ToUpper();
            //    objEntity.v_Field = txtField.Text.Trim().ToUpper();
            //    objEntity.i_ParentParameterId = string.IsNullOrEmpty(ddlParentParameterId.SelectedValue) ? (Int32?)null : Int32.Parse(ddlParentParameterId.SelectedValue);

            //    // Obtener el usuario autenticado
            //    int intUserPersonId = ((ClientSession)Session["objClientSession"]).i_SystemUserId;

            //    if (_objBL.GetSystemParameter(ref objOperationResult, objEntity.i_GroupId, objEntity.i_ParameterId) != null)
            //    {
            //        Alert.Show("¡ La clave primaria ya existe！", MessageBoxIcon.Warning);
            //        return;
            //    }
            //    else
            //    {
            //        // Save the data                  
            //        _objBL.AddSystemParameter(ref objOperationResult, objEntity, ((ClientSession)Session["objClientSession"]).GetAsList());
            //    }

            //}
            //else if (Mode == "Edit")
            //{
            //    // Obtener el usuario autenticado
            //    int intUserPersonId = ((ClientSession)Session["objClientSession"]).i_SystemUserId;

            //    // Get the entity from the session
            //    systemparameterDto objEntity = (systemparameterDto)Session["objEntity"];

            //    // Populate the entity
            //    objEntity.i_GroupId = int.Parse(txtGroupId.Text.Trim());
            //    objEntity.i_ParameterId = int.Parse(txtParameterId.Text.Trim());
            //    objEntity.v_Value1 = txtDescription.Text.Trim().ToUpper();
            //    if (txtUserInterfaceOrder.Text == "") objEntity.i_Sort = null;
            //    else objEntity.i_Sort = int.Parse(txtUserInterfaceOrder.Text.Trim());
            //    objEntity.v_Value2 = txtDescription2.Text.Trim().ToUpper();
            //    objEntity.v_Field = txtField.Text.Trim().ToUpper();
            //    //objEntity.i_ParentGroupId = string.IsNullOrEmpty(ddlParentGroupId.SelectedValue) ? (Int32?)null : Int32.Parse(ddlParentGroupId.SelectedValue);
            //    objEntity.i_ParentParameterId = string.IsNullOrEmpty(ddlParentParameterId.SelectedValue) ? (Int32?)null : Int32.Parse(ddlParentParameterId.SelectedValue);
            //    // Save the data
            //    _objBL.UpdateSystemParameter(ref objOperationResult, objEntity, ((ClientSession)Session["objClientSession"]).GetAsList());
            //}
            //else if (Mode == "NewChildren")
            //{
            //    // Obtener el usuario autenticado
            //    int intUserPersonId = ((ClientSession)Session["objClientSession"]).i_SystemUserId;

            //    // Create the entity
            //    systemparameterDto objEntity = new systemparameterDto();

            //    // Populate the entity
            //    objEntity.i_GroupId = int.Parse(txtGroupId.Text.Trim());
            //    objEntity.i_ParameterId = int.Parse(txtParameterId.Text.Trim());
            //    objEntity.v_Value1 = txtDescription.Text.Trim().ToUpper();
            //    if (txtUserInterfaceOrder.Text == "") objEntity.i_Sort = null;
            //    else objEntity.i_Sort = int.Parse(txtUserInterfaceOrder.Text.Trim());
            //    objEntity.v_Value2 = txtDescription2.Text.Trim().ToUpper();
            //    objEntity.v_Field = txtField.Text.Trim().ToUpper();
            //    //objEntity.i_ParentGroupId = string.IsNullOrEmpty(ddlParentGroupId.SelectedValue) ? (Int32?)null : Int32.Parse(ddlParentGroupId.SelectedValue);
            //    objEntity.i_ParentParameterId = string.IsNullOrEmpty(ddlParentParameterId.SelectedValue) ? (Int32?)null : Int32.Parse(ddlParentParameterId.SelectedValue);
            //    if (_objBL.GetSystemParameter(ref objOperationResult, objEntity.i_GroupId, objEntity.i_ParameterId) != null)
            //    {
            //        Alert.Show("¡La clave primaria ya existe！", MessageBoxIcon.Warning);
            //        return;
            //    }
            //    else
            //    {
            //        // Save the data
            //        _objBL.AddSystemParameter(ref objOperationResult, objEntity, ((ClientSession)Session["objClientSession"]).GetAsList());
            //    }
            //}
            ////Analizar el resultado de la operación
            //if (objOperationResult.Success == 1)  // Operación sin error
            //{
            //    // Cerrar página actual y hacer postback en el padre para actualizar
            //    PageContext.RegisterStartupScript(ActiveWindow.GetHidePostBackReference());
            //}
            //else  // Operación con error
            //{
            //    Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage);
            //    // Se queda en el formulario.
            //}
        }

        public static void Download(string sFileName, string sFilePath)
        {
            HttpContext.Current.Response.ContentType = "APPLICATION/OCTET-STREAM";
            String Header = "Attachment; Filename=" + sFileName;
            HttpContext.Current.Response.AppendHeader("Content-Disposition", Header);
            System.IO.FileInfo Dfile = new System.IO.FileInfo(HttpContext.Current.Server.MapPath(sFilePath));
            HttpContext.Current.Response.WriteFile(Dfile.FullName);
            HttpContext.Current.Response.End();
        }

        protected void link_Click(object sender, EventArgs e)
        {
            Download("Chrysanthemum.jpg", "File/Chrysanthemum.jpg");
        }
    }

   

}