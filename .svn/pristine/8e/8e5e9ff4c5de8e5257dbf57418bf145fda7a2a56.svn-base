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
using System.IO;
namespace SAMBHS.Server.WebClientAdmin.UI.Common
{
    public partial class FRM004 : System.Web.UI.Page
    {
        NodeBL _objNodeBL = new NodeBL();
        //DbConfigBL _objDbConfigBL = new DbConfigBL();
        dbconfigDto _dbconfigDto = new dbconfigDto();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                OperationResult objOperationResult = new OperationResult();
                Session["strFilterExpression"] = null;

                btnNew.OnClientClick = winEdit.GetSaveStateReference(hfRefresh.ClientID) + winEdit.GetShowReference("FRM004A.aspx?Mode=New");
                SecurityBL obSecurityBL = new SecurityBL();
                var formActions = obSecurityBL.GetFormAction(ref objOperationResult, ((ClientSession)Session["objClientSession"]).i_CurrentExecutionNodeId, ((ClientSession)Session["objClientSession"]).i_SystemUserId, "FRM004", ((ClientSession)Session["objClientSession"]).i_RoleId);
                SAMBHS.Common.Resource.Utils.Web.SetFormActionsInSession("FRM004", formActions);

                btnNew.OnClientClick = winEdit.GetSaveStateReference(hfRefresh.ClientID) + winEdit.GetShowReference("FRM004A.aspx?Mode=New");
                

                btnMigrarTablas.OnClientClick = WinMigracion.GetSaveStateReference(hfRefresh.ClientID) + WinMigracion.GetShowReference("FRM014.aspx");

                BindGrid();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string filter = string.Empty;
            if (!string.IsNullOrEmpty(txtNodeFilter.Text))
            {
                filter = "v_RazonSocial.Contains(\"" + txtNodeFilter.Text.Trim() + "\")";
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
            grdData.DataSource = GetData(grdData.PageIndex, grdData.PageSize, "v_RazonSocial ASC", _filterExpression);
            grdData.DataBind();

        }

        private int GetTotalCount(string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var nodeCount = _objNodeBL.GetNodeCount(ref objOperationResult, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage);
            }

            return nodeCount;
        }

        private List<nodeDto> GetData(int pintPageIndex, int pintPageSize, string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            List<nodeDto> objData = _objNodeBL.GetNodePagedAndFiltered(ref objOperationResult, pintPageIndex, pintPageSize, pstrSortExpression, pstrFilterExpression);

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
                int nodeId = Convert.ToInt32(grdData.DataKeys[grdData.SelectedRowIndex][0]);

                // Borrar Nodo 
                _objNodeBL.DeleteNode(ref objOperationResult, nodeId, ((ClientSession)Session["objClientSession"]).GetAsList());

                BindGrid();
            }
            if (e.CommandName == "DBGenerate")
            {
                //OperationResult objOperationResult = new OperationResult();
                //string _Ruc;
                //_Ruc = grdData.DataKeys[grdData.SelectedRowIndex][1].ToString().Trim();
                //_dbconfigDto = new dbconfigDto();
                //_dbconfigDto = _objDbConfigBL.GetDbConfig(ref objOperationResult);
                //List<StoredProcedureResultDto> result = _objDbConfigBL.ReplicaBdPlantilla(_Ruc, _dbconfigDto.v_RutaMSSQL, _dbconfigDto.v_RutaBDPlantilla, ref objOperationResult);
                //if (result[0].Valor_Retorno == 1)
                //{
                //    Alert alert = new Alert();
                //    alert.Title = "Mensaje";
                //    alert.Message = "Base de Datos Creada！";
                //    alert.Icon = Icon.Accept;
                //    alert.Target = Target.Top;
                //    alert.Show();
                //}
                //else
                //{
                //    Alert alert = new Alert();
                //    alert.Title = "Mensaje";
                //    alert.Message = "No se pudo crear la Base de Datos!"+ "\nNúmero de Error: " + result[0].ErrorNumber + "\nError: " + result[0].ErrorMessage;
                //    alert.Icon = Icon.Exclamation;
                //    alert.Target = Target.Top;
                //    alert.Show();
                //}
            }
            if (e.CommandName == "DbBackUP")
            {
                //string _Ruc, _FileName, _RutaBackup;
                //_Ruc = grdData.DataKeys[grdData.SelectedRowIndex][1].ToString().Trim();
                //_FileName = _Ruc;
                ////_RutaBackup = Server.MapPath(@"~\BD_Backups_Clientes\download");
                //_RutaBackup = @"C:\BD_BACKUPS_CLIENTES\download\";
                //OperationResult objOperationResult = new OperationResult();
                //List<StoredProcedureResultDto> result = _objDbConfigBL.GeneraBackup(_Ruc, _FileName, _RutaBackup, ref objOperationResult);
                //if (result[0].Valor_Retorno == 1)
                //{
                //    Alert alert = new Alert();
                //    alert.Title = "Mensaje";
                //    alert.Message = "Copia de la Base de Datos Creada！";
                //    alert.Icon = Icon.Accept;
                //    alert.Target = Target.Top;
                //    Response.Redirect("~/DownloadFile.ashx/DownloadFile.ashx?FileName=" + _FileName);
                //    alert.Show();
                //}
                //else
                //{
                //    Alert alert = new Alert();
                //    alert.Title = "Mensaje";
                //    alert.Message = "No se pudo crear la copia!" + "\nNúmero de Error: " + result[0].ErrorNumber + "\nError: " + result[0].ErrorMessage;
                //    alert.Icon = Icon.Exclamation;
                //    alert.Target = Target.Top;
                //    alert.Show();
                //}
            }
        }

        protected void winEdit_Close(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void winEdit_Close(object sender, WindowCloseEventArgs e)
        {
            BindGrid();
        }

        protected void grdData_PreDataBound(object sender, EventArgs e)
        {
            WindowField wfEdit1 = grdData.FindColumn("myWindowField") as WindowField;
            wfEdit1.Enabled = Utils.Web.IsActionEnabled("FRM004_EDIT");

            LinkButtonField lbfDelete = grdData.FindColumn("lbfAction4") as LinkButtonField;
            lbfDelete.Enabled = Utils.Web.IsActionEnabled("FRM004_DELETE");
        }
    }
}