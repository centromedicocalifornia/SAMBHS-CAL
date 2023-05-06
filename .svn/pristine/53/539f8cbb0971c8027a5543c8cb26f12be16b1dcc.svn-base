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
    public partial class FRM009A : System.Web.UI.Page
    {
        WarehouseBL _objWarehouseBL = new WarehouseBL();

        #region Properties

        public string WarehouseId
        {
            get
            {
                if (Request.QueryString["i_WarehouseId"] != null)
                {
                    string _organizatonId = Request.QueryString["i_WarehouseId"].ToString();
                    if (!string.IsNullOrEmpty(_organizatonId))
                    {
                        return _organizatonId;
                    }
                }

                return "";
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

        private bool IsValidNodeName(string pstrName)
        {
            // Validar existencia de un Warehouse
            OperationResult objOperationResult6 = new OperationResult();
            string filterExpression = string.Format("v_Name==\"{0}\"", pstrName);
            var recordCount = _objWarehouseBL.GetwarehouseCount(ref objOperationResult6, filterExpression);

            if (recordCount != 0)
            {
                Alert.ShowInTop("El nombre de Almacén  <font color='red'>" + pstrName + "</font> ya se encuentra registrado.<br> Por favor ingrese otro nombre de Almacén.");
                return false;
            }
            return true;
        }

        protected void btnSaveRefresh_Click(object sender, EventArgs e)
        {
            if (Mode == "New")
            {
                #region Validate

                // Validar Almacén
                if (!IsValidNodeName(txtName.Text.Trim().ToUpper()))
                {
                    return;
                }

                #endregion

                // Datos de Warehouse
                warehouseDto objwarehouseDTO = new warehouseDto();
                objwarehouseDTO.v_Name = txtName.Text.Trim().ToUpper();
                objwarehouseDTO.v_Address = txtAddress.Text.Trim().ToUpper();
                objwarehouseDTO.v_PhoneNumber = txtPhoneNumber.Text;
                objwarehouseDTO.v_TicketSerialNumber = txtTicketSerialNumber.Text.Trim();
                objwarehouseDTO.v_EstablishmentCode = txtAlm_Nro_Cod_Etb.Text.Trim();
                OperationResult objOperationResult1 = new OperationResult();
                // Graba Warehouse
                _objWarehouseBL.Addwarehouse(ref objOperationResult1, objwarehouseDTO, ((ClientSession)Session["objClientSession"]).GetAsList());

                if (objOperationResult1.Success != 1)
                {
                    Alert.ShowInTop("Error en operación:" + System.Environment.NewLine + objOperationResult1.ExceptionMessage);
                }

            }
            else if (Mode == "Edit")
            {
                warehouseDto objWarehouseDTO = Session["sobjWarehouseDTO"] as warehouseDto;

                #region Validate Node

                // Almacenar temporalmente el nombre de Warehouse
                var _nodeRucTemp = objWarehouseDTO.v_Name;

                if (txtName.Text != _nodeRucTemp)
                {
                    // Validar Warehouse
                    if (!IsValidNodeName(txtName.Text.Trim().ToUpper()))
                    {
                        return;
                    }
                }

                #endregion

                // Datos de Warehouse
                objWarehouseDTO.v_Name = txtName.Text.Trim().ToUpper();
                objWarehouseDTO.v_Address = txtAddress.Text.Trim().ToUpper();
                objWarehouseDTO.v_PhoneNumber = txtPhoneNumber.Text;
                objWarehouseDTO.v_TicketSerialNumber = txtTicketSerialNumber.Text.Trim();
                objWarehouseDTO.v_EstablishmentCode = txtAlm_Nro_Cod_Etb.Text.Trim();

                // Actualiza Warehouse
                OperationResult objOperationResult1 = new OperationResult();
                _objWarehouseBL.Updatewarehouse(ref objOperationResult1, objWarehouseDTO, ((ClientSession)Session["objClientSession"]).GetAsList());

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
                warehouseDto objWarehouseDTO = _objWarehouseBL.GetwarehouseBywarehouseId(ref objCommonOperationResultedit, int.Parse(WarehouseId));

                Session["sobjWarehouseDTO"] = objWarehouseDTO;

                // Informacion del Warehouse
                txtName.Text = objWarehouseDTO.v_Name;
                txtAddress.Text = objWarehouseDTO.v_Address;
                txtPhoneNumber.Text = objWarehouseDTO.v_PhoneNumber;
                txtTicketSerialNumber.Text = objWarehouseDTO.v_TicketSerialNumber;
                txtAlm_Nro_Cod_Etb.Text = objWarehouseDTO.v_EstablishmentCode;

            }
        }
    }
}