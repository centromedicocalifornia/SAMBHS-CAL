using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Compra.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmBuscarCompraPorProveedor : Form
    {
        public compraDto objCompraDto { get { return _compraDto; } }
        compraDto _compraDto = new compraDto();
        ComprasBL _objComprasBL = new ComprasBL();
        
        string _strFilterExpression = string.Empty;

        public frmBuscarCompraPorProveedor(string RazonSocial, string IdProveedor)
        {
            InitializeComponent();
            txtRazonSocial.Text = RazonSocial;
            txtRazonSocial.Tag = IdProveedor;
        }

        private void frmBuscarCompraPorProveedor_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            dtpFechaInicio.Value = DateTime.Parse("01/" + DateTime.Today.Month.ToString() + "/" + Globals.ClientSession.i_Periodo.ToString());
            dtpFechaFin.Value = DateTime.Parse(DateTime.Today.Day.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + Globals.ClientSession.i_Periodo.ToString());
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid)
            {
                List<string> Filters = new List<string>();

                if (txtRazonSocial.Tag != null && txtRazonSocial.Tag.ToString() != "-1") Filters.Add("v_IdProveedor==\"" + txtRazonSocial.Tag.ToString() + "\"");
                _strFilterExpression = null;
                if (Filters.Count > 0)
                {
                    foreach (string item in Filters)
                    {
                        _strFilterExpression = _strFilterExpression + item + " && ";
                    }
                    _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                }

                this.BindGrid();
                grdData.Focus();
            }
        }

        private void BindGrid()
        {
            var objData = GetData("v_IdCompra ASC", _strFilterExpression);

            grdData.DataSource = objData;
            if (objData != null)
            {
                if (objData.Count > 0)
                {
                    lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
                }
            }
        }

        private List<compraDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objComprasBL.ListarBusquedaCompras(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, dtpFechaFin.Value,false );

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                _compraDto = (compraDto)grdData.ActiveRow.ListObject;
                this.Close();
            }
        }
    }
}
