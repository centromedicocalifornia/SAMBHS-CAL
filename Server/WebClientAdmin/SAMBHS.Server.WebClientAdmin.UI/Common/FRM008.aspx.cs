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
    public partial class FRM008 : System.Web.UI.Page
    {
        SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        OrganizationBL _objOrganizationBL = new OrganizationBL();

        //private Utils.Utils _Util = new Utils.Utils();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                OperationResult objOperationResult = new OperationResult();
                SecurityBL obSecurityBL = new SecurityBL();
                var formActions = obSecurityBL.GetFormAction(ref objOperationResult, ((ClientSession)Session["objClientSession"]).i_CurrentExecutionNodeId, ((ClientSession)Session["objClientSession"]).i_SystemUserId, "FRM008", ((ClientSession)Session["objClientSession"]).i_RoleId);
                SAMBHS.Common.Resource.Utils.Web.SetFormActionsInSession("FRM008", formActions);

                Session["strFilterExpression"] = null;
                btnNew.OnClientClick = winEdit.GetSaveStateReference(hfRefresh.ClientID) + winEdit.GetShowReference("FRM008A.aspx?Mode=New");


                btnNew.Enabled = Utils.Web.IsActionEnabled("FRM008_ADD");

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

        // No se usa
        protected string GetEditUrl(object id)
        {
            string url = string.Format("/FRM080A.aspx?Mode=Edit&i_OrganizationId={0}", id);
            string ret = winEdit.GetShowReference(url);
            return ret;
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
            return _objOrganizationBL.GetorganizationCount(ref objOperationResult, strFilterExpression);
        }

        private List<organizationDto> GetData(int pintPageIndex, int pintPageSize, string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();

            List<organizationDto> _objData = _objOrganizationBL.GetorganizationPagedAndFiltered(ref objOperationResult, pintPageIndex, pintPageSize, pstrSortExpression, pstrFilterExpression).ToList();

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
            int strOrganizationId = int.Parse(grdData.DataKeys[grdData.SelectedRowIndex][0].ToString());

            // Delete the item
            OperationResult objOperationResult = new OperationResult();
            _objOrganizationBL.Deleteorganization(ref objOperationResult, strOrganizationId, ((ClientSession)Session["objClientSession"]).GetAsList());
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            // Get the filters from the UI
            List<string> Filters = new List<string>();
            if (!string.IsNullOrEmpty(txtIdentificationNumber.Text)) Filters.Add("v_IdentificationNumber==" + "\"" + txtIdentificationNumber.Text.Trim() + "\"");
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
            wfEdit1.Enabled = Utils.Web.IsActionEnabled("FRM008_EDIT");

            LinkButtonField lbfDelete = grdData.FindColumn("lbfAction2") as LinkButtonField;
            lbfDelete.Enabled = Utils.Web.IsActionEnabled("FRM008_DELETE");
        }
    }
}