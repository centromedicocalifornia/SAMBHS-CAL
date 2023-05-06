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
    public partial class FRM008B : System.Web.UI.Page
    {
    //    SystemParameterBL objSystemParameterBL = new SystemParameterBL();
    //    OrganizationBL objOrganizationBL = new OrganizationBL();

    //    protected void Page_Load(object sender, EventArgs e)
    //    {
    //        if (!IsPostBack)
    //        {
    //            LoadData();
    //            btnClose.OnClientClick = ActiveWindow.GetConfirmHideReference();
    //        }

    //    }

    //    protected void btnAdd_Click(object sender, EventArgs e)
    //    {
    //        OperationResult objOperationResult = new OperationResult();
    //        OrganizationBL objOrganizationBL = new OrganizationBL();
    //        organizationwarehouseDto objorganizationwarehouseDto = new organizationwarehouseDto();

    //        objorganizationwarehouseDto.i_OrganizationId = ViewState["OrganizationId"];
    //        objorganizationwarehouseDto.i_WarehouseId = int.Parse(ddlWarehouseId.SelectedValue);

    //        int Contador = objOrganizationBL.FillWarehouseId(ref objOperationResult, objorganizationwarehouseDto.i_WarehouseId, objorganizationwarehouseDto.i_OrganizationId);

    //        if (Contador > 0)
    //        {
    //            Alert.ShowInTop(string.Format("<font color='red'> {0} </font> ya se encuentra registrado en otra Empresa. Por favor elija otro.", ddlWarehouseId.SelectedText));
    //            return;
    //        }


    //        objOrganizationBL.AddOrganizationWarehouse(ref objOperationResult, objorganizationwarehouseDto, ((ClientSession)Session["objClientSession"]).GetAsList());


    //        if (objOperationResult.ErrorMessage != null)
    //        {
    //            Alert.ShowInTop(string.Format("<font color='red'> {0} </font> ya se encuentra registrado. Por favor elija otro.", ddlWarehouseId.SelectedText));
    //            return;
    //        }

    //        if (objOperationResult.Success != 1)
    //        {
    //            Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage);
    //            return;
    //        }

    //        BindGrid();
    //    }

    //    private void LoadData()
    //    {
    //        OperationResult objOperationResult = new OperationResult();
    //        //Llenado de combos
           
    //        Utils.Web.LoadDropDownList(ddlWarehouseId, "Value1", "Id", objSystemParameterBL.GetWarehouse(ref objOperationResult, ""), DropDownListAction.Select);

    //        //string Mode = Request.QueryString["Mode"].ToString();
    //        string OrganizationId = Request.QueryString["OrganizationId"].ToString();
    //        ViewState["OrganizationId"] = OrganizationId;

    //            BindGrid();
    //    }

    //    protected void grdData_RowCommand(object sender, FineUI.GridCommandEventArgs e)
    //    {
    //        if (e.CommandName == "DeleteAction")
    //        {
    //            OperationResult objOperationResult = new OperationResult();
    //            // Obtener los IDs de la fila seleccionada
    //            //int nodeId = Convert.ToInt32(grd.DataKeys[grd.SelectedRowIndex][0]);
    //            //string organizationId = grd.DataKeys[grd.SelectedRowIndex][1].ToString();
    //            string OrganizationWarehouseId = grdData.DataKeys[grdData.SelectedRowIndex][0].ToString();

    //            // Borrar  
    //            objOrganizationBL.DeleteOrganizationWarehouse(ref objOperationResult, OrganizationWarehouseId, ((ClientSession)Session["objClientSession"]).GetAsList());

    //            if (objOperationResult.Success != 1)
    //            {
    //                Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage);
    //            }

    //            BindGrid();
    //        }
    //    }

    //    protected void grdData_PageIndexChange(object sender, FineUI.GridPageEventArgs e)
    //    {
    //        grdData.PageIndex = e.NewPageIndex;
    //    }

    //    private void BindGrid()
    //    {
    //        //string strFilterExpression = Convert.ToString(Session["strFilterExpression"]);

    //        string strFilterExpression = Convert.ToString(ViewState["strFilterExpression"]);
    //        //grdData.RecordCount = GetTotalCount();
    //        grdData.DataSource = GetData(ViewState["OrganizationId"].ToString());
    //        grdData.DataBind();
    //    }

    //    private List<organizationwarehouseDto> GetData(string pintWarehouseId)
    //    {
    //        OperationResult objOperationResult = new OperationResult();
    //        List<organizationwarehouseDto> _objData = objOrganizationBL.GetOrganizationWarehouse(ref objOperationResult, pintWarehouseId).ToList();

    //        if (objOperationResult.Success != 1)
    //        {
    //            Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage);
    //        }

    //        return _objData;
    //    }

    }
}