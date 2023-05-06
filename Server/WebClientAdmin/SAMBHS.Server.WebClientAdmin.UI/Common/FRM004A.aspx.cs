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
    public partial class FRM004A : System.Web.UI.Page
    {
        SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        NodeBL _objNodeBL = new NodeBL();
        #region Properties

        public int NodeId
        {
            get
            {
                if (Request.QueryString["nodeId"] != null)
                {
                    string _nodeId = Request.QueryString["nodeId"].ToString();
                    if (!string.IsNullOrEmpty(_nodeId))
                    {
                        return Convert.ToInt32(_nodeId);
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
        public string _Ruc
        {
            get
            {
                if (Request.QueryString["RUC"] != null)
                {
                    string _Ruc = Request.QueryString["RUC"].ToString();
                    if (!string.IsNullOrEmpty(_Ruc))
                    {
                        return _Ruc;
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
                LoadComboBox();
                LoadData();
                btnClose.OnClientClick = ActiveWindow.GetConfirmHideReference();

            }
        }

        private void LoadComboBox()
        {
            OperationResult objOperationResult = new OperationResult();
            var docType = _objSystemParameterBL.GetSystemParameterForCombo(ref objOperationResult, 105); // tipo de nodo
            //Sigesoft.Server.WebClientAdmin.UI.Utils.LoadDropDownList(ddlNodeType, "Value1", "Id", docType, DropDownListAction.Select);
            //Utils.Web.LoadDropDownList(ddlNodeType, "Value1", "Id", docType, DropDownListAction.All);
               
        }

        private bool IsValidNodeName(string pstrNode)
        {
            // Validar existencia de un nodo
            OperationResult objOperationResult6 = new OperationResult();
            string filterExpression = string.Format("v_RazonSocial==\"{0}\"", pstrNode);
            var recordCount = _objNodeBL.GetNodeCount(ref objOperationResult6, filterExpression);

            if (recordCount != 0)
            {
                Alert.ShowInTop("El nombre de nodo  <font color='red'>" + pstrNode + "</font> ya se encuentra registrado.<br> Por favor ingrese otro nombre de nodo.");
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
                if (!IsValidNodeName(txtDescription.Text.Trim().ToUpper()))
                {
                    return;
                }

                #endregion

                // Datos de nodo
                nodeDto objNodeDTO = new nodeDto();
                objNodeDTO.v_RazonSocial = txtDescription.Text.Trim().ToUpper();
                objNodeDTO.v_RUC = txtGeografyLocationId.Text.Trim().ToUpper();
                objNodeDTO.v_Direccion = txtGeografyLocationDescription.Text.Trim().ToUpper();
                OperationResult objOperationResult1 = new OperationResult();
                // Graba Nodo
                _objNodeBL.AddNode(ref objOperationResult1, objNodeDTO, ((ClientSession)Session["objClientSession"]).GetAsList());

                if (objOperationResult1.Success != 1)
                {
                    Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult1.ExceptionMessage);
                }

            }
            else if (Mode == "Edit")
            {
                nodeDto objNodeDTO = Session["sobjNodeDTO"] as nodeDto;
                if (_Ruc != null)
                {
                    if (txtGeografyLocationId.Text != _Ruc)
                    {
                        Alert.ShowInTop("Se ha modificado el RUC.\nSi ha generado una Base de datos para esta empresa es recomendable que lo vuelva a hacer y restaure la Base de datos con un backup", "Mensaje", MessageBoxIcon.Warning, "");
                    }
                }

                #region Validate Node

                // Almacenar temporalmente el nombre de nodo
                var _nodeNameTemp = objNodeDTO.v_RazonSocial;

                if (txtDescription.Text != _nodeNameTemp)
                {
                    // Validar nodo
                    if (!IsValidNodeName(txtDescription.Text.Trim().ToUpper()))
                    {
                        return;
                    }
                }

                #endregion

                // Datos de Nodo
                objNodeDTO.v_RazonSocial = txtDescription.Text.Trim().ToUpper();
                objNodeDTO.v_RUC = txtGeografyLocationId.Text.Trim().ToUpper();
                objNodeDTO.v_Direccion = txtGeografyLocationDescription.Text.Trim().ToUpper();
                // Actualiza Nodo
                OperationResult objOperationResult1 = new OperationResult();
                _objNodeBL.UpdateNode(ref objOperationResult1, objNodeDTO, ((ClientSession)Session["objClientSession"]).GetAsList());

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
                nodeDto objNodeDTO = _objNodeBL.GetNodeByNodeId(ref objCommonOperationResultedit, NodeId);

                Session["sobjNodeDTO"] = objNodeDTO;

                // Informacion del nodo
                txtDescription.Text = objNodeDTO.v_RazonSocial;
                txtGeografyLocationId.Text = objNodeDTO.v_RUC;
                txtGeografyLocationDescription.Text = objNodeDTO.v_Direccion;
                
            }
        }
    }
}