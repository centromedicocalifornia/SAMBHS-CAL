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
    public partial class FRM009 : System.Web.UI.Page
    {
        SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        //DataHierarchyBL _objDatahierarchyBL = new DataHierarchyBL();
        WarehouseBL _objWarehouseBL = new WarehouseBL();

        //private Utils.Utils _Util = new Utils.Utils();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                OperationResult objOperationResult = new OperationResult();
                SecurityBL obSecurityBL = new SecurityBL();
                var formActions = obSecurityBL.GetFormAction(ref objOperationResult, ((ClientSession)Session["objClientSession"]).i_CurrentExecutionNodeId, ((ClientSession)Session["objClientSession"]).i_SystemUserId, "FRM009", ((ClientSession)Session["objClientSession"]).i_RoleId);
                SAMBHS.Common.Resource.Utils.Web.SetFormActionsInSession("FRM009", formActions);
                Session["strFilterExpression"] = null;
                btnNew.OnClientClick = winEdit.GetSaveStateReference(hfRefresh.ClientID) + winEdit.GetShowReference("FRM009A.aspx?Mode=New");

                //BindGrid();
                btnFilter_Click(sender, e);
            }
        }

        protected void grdData_PageIndexChange(object sender, FineUI.GridPageEventArgs e)
        {
            grdData.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        // Esto actualiza la grilla al cerrar el popup de edición. Solo se invoca si al cerrar el popup se hace el postback.
        protected void winEdit_Close(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            string strFilterExpression = Convert.ToString(Session["strFilterExpression"]);
            grdData.RecordCount = GetTotalCount();
            grdData.DataSource = GetData(grdData.PageIndex, grdData.PageSize, "v_Name ASC", strFilterExpression);
            grdData.DataBind();
        }

        private int GetTotalCount()
        {
            OperationResult objOperationResult = new OperationResult();
            string strFilterExpression = Convert.ToString(Session["strFilterExpression"]);
            return _objWarehouseBL.GetwarehouseCount(ref objOperationResult, strFilterExpression);
        }

        private List<warehouseDto> GetData(int pintPageIndex, int pintPageSize, string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();

            List<warehouseDto> _objData = _objWarehouseBL.GetwarehousePagedAndFiltered(ref objOperationResult, pintPageIndex, pintPageSize, pstrSortExpression, pstrFilterExpression).ToList();

            if (objOperationResult.Success != 1)
            {
                Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage);
            }

            return _objData;
        }

        protected void grdData_RowCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "DeleteAction")
            {
                DeleteItem();
                BindGrid();
            }

        }

        private void DeleteItem()
        {
            // Obtener el usuario autenticado
            int intUserPersonId = ((ClientSession)Session["objClientSession"]).i_SystemUserId;

            // Get the DataKeys from the selected row.
            int strWarewhouseId = int.Parse(grdData.DataKeys[grdData.SelectedRowIndex][0].ToString());

            // Delete the item
            OperationResult objOperationResult = new OperationResult();
            _objWarehouseBL.Deletewarehouse(ref objOperationResult, strWarewhouseId, ((ClientSession)Session["objClientSession"]).GetAsList());
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            // Get the filters from the UI
            List<string> Filters = new List<string>();
            if (!string.IsNullOrEmpty(txtName.Text)) Filters.Add("v_Name.Contains(\"" + txtName.Text.Trim().ToUpper() + "\")");

            // Create the Filter Expression
            string strFilterExpression = null;
            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    strFilterExpression = strFilterExpression + item + " && ";
                }
                strFilterExpression = strFilterExpression.Substring(0, strFilterExpression.Length - 4);
            }

            // Save the Filter expression in the Session
            Session["strFilterExpression"] = strFilterExpression;

            // Refresh the grid
            grdData.PageIndex = 0;
            this.BindGrid();
        }

        protected void grdData_PreRowDataBound(object sender, GridPreRowEventArgs e)
        {
            WindowField wfEdit1 = grdData.FindColumn("myWindowField") as WindowField;
            wfEdit1.Enabled = Utils.Web.IsActionEnabled("FRM009_EDIT");

            LinkButtonField lbfDelete = grdData.FindColumn("lbfAction2") as LinkButtonField;
            lbfDelete.Enabled = Utils.Web.IsActionEnabled("FRM009_DELETE");
        }
   
    }
}