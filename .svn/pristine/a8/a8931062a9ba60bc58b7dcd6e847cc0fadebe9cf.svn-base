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
    public partial class FRM004B : System.Web.UI.Page
    {
        SystemParameterBL objSystemParameterBL = new SystemParameterBL();
        NodeBL objNodeBL = new NodeBL();

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
            NodeBL objNodeBL = new NodeBL();
            nodewarehouseDto objnodewarehouseDto = new nodewarehouseDto();
            if (ddlWarehouseId.SelectedValue == "-1")
            {
                Alert.ShowInTop(string.Format("Porfavor escoja un almacén antes de guardar"));
                return;
            }

            objnodewarehouseDto.i_NodeId = (Int32)ViewState["NodeId"];
            objnodewarehouseDto.i_WarehouseId = int.Parse(ddlWarehouseId.SelectedValue.ToString());

            int Contador = objNodeBL.FillWarehouseId(ref objOperationResult, objnodewarehouseDto.i_WarehouseId, objnodewarehouseDto.i_NodeId);

            if (Contador >0)
            {
                 Alert.ShowInTop(string.Format("<font color='red'> {0} </font> ya se encuentra registrado en otro nodo. Por favor elija otro.", ddlWarehouseId.SelectedText));
                return;
            }

            objNodeBL.AddNodeWarehouse(ref objOperationResult,objnodewarehouseDto,((ClientSession)Session["objClientSession"]).GetAsList());


            if (objOperationResult.ErrorMessage != null)
            {
                Alert.ShowInTop(string.Format("<font color='red'> {0} </font>", objOperationResult.ErrorMessage));
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

            Utils.Web.LoadDropDownList(ddlWarehouseId, "Value1", "Id", objSystemParameterBL.GetWarehouse(ref objOperationResult, ""), DropDownListAction.Select);

            //string Mode = Request.QueryString["Mode"].ToString();
            int NodeId = int.Parse(Request.QueryString["nodeId"].ToString());
            ViewState["NodeId"] = NodeId;

            //if (Mode == "Edit")
            //{

                BindGrid();
            //}
        }

        protected void grdData_RowCommand(object sender, FineUI.GridCommandEventArgs e)
        {
            if (e.CommandName == "DeleteAction")
            {
                OperationResult objOperationResult = new OperationResult();
                // Obtener los IDs de la fila seleccionada
                //int nodeId = Convert.ToInt32(grd.DataKeys[grd.SelectedRowIndex][0]);
                //string organizationId = grd.DataKeys[grd.SelectedRowIndex][1].ToString();
                int NodeWarehouseId = int.Parse(grdData.DataKeys[grdData.SelectedRowIndex][0].ToString());

                // Borrar  
                objNodeBL.DeleteNodeWarehouse(ref objOperationResult, NodeWarehouseId, ((ClientSession)Session["objClientSession"]).GetAsList());

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
            grdData.DataSource = GetData((int)ViewState["NodeId"]);
            grdData.DataBind();
        }
        
        private List<nodewarehouseDto> GetData(int pintNodeId)
        {
            OperationResult objOperationResult = new OperationResult();
            List<nodewarehouseDto> _objData = objNodeBL.GetNodeWarehouse(ref objOperationResult, pintNodeId).ToList();

            if (objOperationResult.Success != 1)
            {
                Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage);
            }

            return _objData;
        }


    }
}