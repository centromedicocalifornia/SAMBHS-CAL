using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using SAMBHS.Security.BL;
using FineUI;

namespace SAMBHS.Server.WebClientAdmin.UI.Security
{
    public partial class FRM006B : System.Web.UI.Page
    {
        SystemParameterBL objSystemParameterBL = new SystemParameterBL();
        SecurityBL objSecurityBL = new SecurityBL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
                btnClose.OnClientClick = ActiveWindow.GetConfirmHideReference();
            }

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            SecurityBL objSecurityBL = new SecurityBL();
            systemusernodeDto objsystemusernodeDto = new systemusernodeDto();
            if (ddlNodeId.SelectedValue == "-1")
            {
                Alert.ShowInTop(string.Format("Porfavor elija una Empresa"));
                return;
            }
            objsystemusernodeDto.i_SystemUserId = (int)ViewState["SystemUserId"];
            objsystemusernodeDto.i_NodeId =int.Parse(ddlNodeId.SelectedValue.ToString());
            objSecurityBL.AddSystemUserNode(ref objOperationResult, objsystemusernodeDto, ((ClientSession)Session["objClientSession"]).GetAsList());


            if (objOperationResult.ErrorMessage != null)
            {
                Alert.ShowInTop(string.Format("<font color='red'> {0} </font> ya se encuentra registrado. Por favor elija otro.", ddlNodeId.SelectedText));
                return;
            }

            if (objOperationResult.Success != 1)
            {
                Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage);
                return;
            }

            BindGrid();
        }

        private void LoadData()
        {
            OperationResult objOperationResult = new OperationResult();
            //Llenado de combos
            SAMBHS.Common.Resource.Utils UtilComboBox = new SAMBHS.Common.Resource.Utils();

            Utils.Web.LoadDropDownList(ddlNodeId, "Value1", "Id", objSystemParameterBL.GetNode(ref objOperationResult, ""), DropDownListAction.Select);

            int SystemUserId = int.Parse(Request.QueryString["systemUserId"].ToString());
            ViewState["SystemUserId"] = SystemUserId;

            BindGrid();
        }

        protected void grdData_RowCommand(object sender, FineUI.GridCommandEventArgs e)
        {
            if (e.CommandName == "DeleteAction")
            {
                OperationResult objOperationResult = new OperationResult();
                // Obtener los IDs de la fila seleccionada
                int SystemUserNodeId = int.Parse(grdData.DataKeys[grdData.SelectedRowIndex][0].ToString());

                // Borrar  
                objSecurityBL.DeleteSystemUserNode(ref objOperationResult, SystemUserNodeId, ((ClientSession)Session["objClientSession"]).GetAsList());

                if (objOperationResult.Success != 1)
                {
                    Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage);
                }

                BindGrid();
            }
        }

        protected void grdData_PageIndexChange(object sender, FineUI.GridPageEventArgs e)
        {
            grdData.PageIndex = e.NewPageIndex;
        }

        private void BindGrid()
        {
            //string strFilterExpression = Convert.ToString(Session["strFilterExpression"]);

            string strFilterExpression = Convert.ToString(ViewState["strFilterExpression"]);
            //grdData.RecordCount = GetTotalCount();
            grdData.DataSource = GetData((int)ViewState["SystemUserId"]);
            grdData.DataBind();
        }

        private List<systemusernodeDto> GetData(int pintSystemUserId)
        {
            OperationResult objOperationResult = new OperationResult();
            List<systemusernodeDto> _objData = objSecurityBL.GetSystemUserNode(ref objOperationResult, pintSystemUserId).ToList();

            if (objOperationResult.Success != 1)
            {
                Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage);
            }

            return _objData;
        }

    }
}