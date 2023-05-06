using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Almacen.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmBuscarProductoComprobante : Form
    {
        MovimientoBL _objProductoBL = new MovimientoBL();
        productoDto _productoDto = new productoDto();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        string _strFilterExpression;
        public int _IdAlmacen;
        public string _IdProducto, _NombreProducto, _CodigoInternoProducto, _tipBusqueda, _stockActual, _UnidadMedida;
        public decimal _Empaque;
        public frmBuscarProductoComprobante()
        {
            InitializeComponent();
        }

        private void frmBuscarProductoComprobante_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadDropDownList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento??-1), DropDownListAction.Select);
        }

        private void BindGrid()
        {
            var objData = GetData("v_Descripcion ASC", _strFilterExpression);

            grdData.DataSource = objData;

            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<productoshortDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objProductoBL.ListarBusquedaServicios(ref objOperationResult, null, null, null, _IdAlmacen);
            return _objData;
        }

        private void grdData_DoubleClick(object sender, EventArgs e)
        {
            if (grdData.Rows.Count == 0) return;
            _IdProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString();
            _NombreProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value.ToString();
            _CodigoInternoProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Value.ToString();
            _stockActual = grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value == null ? null : grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value.ToString();
            _UnidadMedida = grdData.Rows[grdData.ActiveRow.Index].Cells["EmpaqueUnidadMedida"].Value == null ? null : grdData.Rows[grdData.ActiveRow.Index].Cells["EmpaqueUnidadMedida"].Value.ToString();
            _Empaque = grdData.Rows[grdData.ActiveRow.Index].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Empaque"].Value.ToString());
            _IdAlmacen = grdData.Rows[grdData.ActiveRow.Index].Cells["IdAlmacen"].Value == null ? 0 : int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["IdAlmacen"].Value.ToString());
            this.Close();
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (grdData.Rows.Count == 0) return;
                _IdProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString();
                _NombreProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value.ToString();
                _CodigoInternoProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Value.ToString();
                _stockActual = grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value == null ? null : grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value.ToString();
                _UnidadMedida = grdData.Rows[grdData.ActiveRow.Index].Cells["EmpaqueUnidadMedida"].Value == null ? null : grdData.Rows[grdData.ActiveRow.Index].Cells["EmpaqueUnidadMedida"].Value.ToString();
                _Empaque = grdData.Rows[grdData.ActiveRow.Index].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Empaque"].Value.ToString());
                _IdAlmacen = grdData.Rows[grdData.ActiveRow.Index].Cells["IdAlmacen"].Value == null ? 0 : int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["IdAlmacen"].Value.ToString());
                this.Close();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid)
            {
                List<string> Filters = new List<string>();
                if (!string.IsNullOrEmpty(txtBuscarNombre.Text)) Filters.Add("v_Descripcion.Contains(\"" + txtBuscarNombre.Text.Trim().ToUpper() + "\")" + " ||" + "v_CodInterno== (\"" + txtBuscarNombre.Text.Trim().ToUpper() + "\")");
                _IdAlmacen = int.Parse(cboAlmacen.SelectedValue.ToString());
                // Create the Filter Expression
                _strFilterExpression = null;
                if (Filters.Count > 0)
                {
                    foreach (string item in Filters)
                    {
                        _strFilterExpression = _strFilterExpression + item + " && ";
                    }
                    _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                }

                BindGrid();
            }
           
        }

        private void frmBuscarProductoComprobante_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
 