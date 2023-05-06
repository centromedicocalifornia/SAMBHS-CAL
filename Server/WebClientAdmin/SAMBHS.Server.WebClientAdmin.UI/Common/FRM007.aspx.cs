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
using SAMBHS.Security.BL;

namespace SAMBHS.Server.WebClientAdmin.UI.Common
{
    public partial class FRM007 : System.Web.UI.Page
    {
        RoleBL _objRoleBL = new RoleBL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                OperationResult objOperationResult = new OperationResult();

                Session["strFilterExpression"] = null;

                SecurityBL obSecurityBL = new SecurityBL();
                var formActions = obSecurityBL.GetFormAction(ref objOperationResult, ((ClientSession)Session["objClientSession"]).i_CurrentExecutionNodeId, ((ClientSession)Session["objClientSession"]).i_SystemUserId, "FRM007", ((ClientSession)Session["objClientSession"]).i_RoleId);
                SAMBHS.Common.Resource.Utils.Web.SetFormActionsInSession("FRM007", formActions);

                btnNew.OnClientClick = winEdit.GetSaveStateReference(hfRefresh.ClientID) + winEdit.GetShowReference("FRM007A.aspx?Mode=New");
                BindGrid();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string filter = string.Empty;
            if (!string.IsNullOrEmpty(txtRoleFilter.Text))
            {
                filter = "v_Name.Contains(\"" + txtRoleFilter.Text.Trim().ToUpper() + "\")";
            }

            Session["strFilterExpression"] = filter;

            // Refresh the grid
            grdData.PageIndex = 0;
            this.BindGrid();
        }

        private void BindGrid()
        {
            string _filterExpression = Convert.ToString(Session["strFilterExpression"]);
            grdData.RecordCount = GetTotalCount(_filterExpression);
            grdData.DataSource = GetData(grdData.PageIndex, grdData.PageSize, "v_Name ASC", _filterExpression);
            grdData.DataBind();

        }

        private int GetTotalCount(string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var RoleCount = _objRoleBL.GetRoleCount(ref objOperationResult, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage);
            }

            return RoleCount;
        }

        private List<roleDto> GetData(int pintPageIndex, int pintPageSize, string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            List<roleDto> objData = _objRoleBL.GetRolePagedAndFiltered(ref objOperationResult, pintPageIndex, pintPageSize, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage);
            }

            return objData;
        }

        protected void grdData_PageIndexChange(object sender, FineUI.GridPageEventArgs e)
        {
            grdData.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void grdData_RowCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "DeleteAction")
            {
                OperationResult objOperationResult = new OperationResult();
                // Obtener los IDs de la fila seleccionada
                int RoleId = Convert.ToInt32(grdData.DataKeys[grdData.SelectedRowIndex][0]);

                // Borrar Nodo 
                _objRoleBL.DeleteRole(ref objOperationResult, RoleId, ((ClientSession)Session["objClientSession"]).GetAsList());

                // Borrar Organizaciones asociadas del nodo actual
                //OperationResult objOperationResult1 = new OperationResult();
                //_objNodeBL.DeleteNodeOrganizations(ref objOperationResult1, _nodeId, 1);

                BindGrid();
            }

        }

        protected void winEdit_Close(object sender, EventArgs e)
        {
            BindGrid();
        }
              
        protected void winEditPermission_Close(object sender, WindowCloseEventArgs e)
        {
            BindGrid();
        }

        protected void grdData_PreRowDataBound(object sender, GridPreRowEventArgs e)
        {
            WindowField wfEdit1 = grdData.FindColumn("myWindowField") as WindowField;
            wfEdit1.Enabled = Utils.Web.IsActionEnabled("FRM007_EDIT");

            WindowField wfEdit2 = grdData.FindColumn("myChildren") as WindowField;
            wfEdit2.Enabled = Utils.Web.IsActionEnabled("FRM007_ADD");

            LinkButtonField lbfDelete = grdData.FindColumn("lbfAction2") as LinkButtonField;
            lbfDelete.Enabled = Utils.Web.IsActionEnabled("FRM007_DELETE");
        }
    }
}