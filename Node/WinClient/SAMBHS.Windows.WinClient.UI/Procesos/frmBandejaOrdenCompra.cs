using SAMBHS.Common.Resource;
using SAMBHS.Compra.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BE;
using Infragistics.Win.UltraWinGrid;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaOrdenCompra : Form
    {
        #region Fields
        private readonly OrdenCompraBL _objOrdenCompraBL = new OrdenCompraBL(); 
        #endregion

        #region Init
        public frmBandejaOrdenCompra(string N)
        {
            InitializeComponent();
        }

        private void frmBandejaOrdenCompra_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            var objOperationResult = new OperationResult();
            var objDatahierarchyBL = new DatahierarchyBL();
            Utils.Windows.LoadUltraComboEditorList(cboAreaSolicita, "Value1", "Id", objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 90, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstado, "Value1", "Id", objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 92, null), DropDownListAction.Select);
            var fecha = DateTime.Parse(string.Format("{0}/{1}/{2}", DateTime.Today.Day, DateTime.Today.Month, Globals.ClientSession.i_Periodo));
            dtpFechaRegistroIni.Value = fecha;
            dtpFechaRegistroFin.Value = fecha;
            dtpFechaRegistroIni.MaxDate = dtpFechaRegistroFin.Value;
            dtpFechaEntregaIni.Value = fecha;
            dtpFechaEntregaFin.Value = fecha;
            dtpFechaEntregaIni.MaxDate = dtpFechaEntregaFin.Value;
            cboAreaSolicita.Value = "-1";
            cboEstado.Value = "-1";
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            cboMoneda.Value = "-1";
        }
        #endregion

        #region Events
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmOrdenCompra))) return;
            var f = new frmOrdenCompra("Nuevo", string.Empty);
            (this.MdiParent as frmMaster).RegistrarForm(this,f);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                var Filters = new Queue<string>();
                if (cboAreaSolicita.Value.ToString() != "-1") Filters.Enqueue("i_IdAreaSolicita==" + cboAreaSolicita.Value.ToString());
                if (cboEstado.Value.ToString() != "-1") Filters.Enqueue("i_IdEstado==" + cboEstado.Value.ToString());
                if (txtProveedor.Tag != null) Filters.Enqueue("v_IdProveedor==\"" + txtProveedor.Tag.ToString() + "\"");
                if (cboMoneda.Value.ToString() != "-1") Filters.Enqueue("Moneda==\"" + (cboMoneda.Value.ToString().Equals("1") ? "S" : "D") + "\"");
                var strFilterExpression = string.Join(" && ", Filters);
                BindGrid(strFilterExpression);
                if (grdData.Rows.Count == 0)
                {
                    btnEliminar.Enabled = false;
                    btnEditar.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }

        private void dtpFechaRegistroFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaRegistroIni.MaxDate = dtpFechaRegistroFin.Value;
        }

        private void dtpFechaEntregaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaEntregaIni.MaxDate = dtpFechaEntregaFin.Value;
        }

        private void BindGrid(string filter)
        {
            var objData = GetData("v_IdOrdenCompra ASC", filter);

            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row == null)
            {
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
            else
            {
                btnEditar.Enabled = true;
                btnEliminar.Enabled = true;
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                if (grdData.ActiveRow.Index < 0) return;
                if (Utils.Windows.HaveFormChild(this, typeof(frmOrdenCompra), true)) return;
                frmOrdenCompra f = new frmOrdenCompra("Edicion", grdData.ActiveRow.Cells["v_IdOrdenCompra"].Value.ToString());
                (this.MdiParent as frmMaster).RegistrarForm(this, f);
            }

        }

        private void btnEstado_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                if (grdData.ActiveRow.Index < 0) return;
                if (Utils.Windows.HaveFormChild(this, typeof(frmOrdenCompra), true)) return;
                var f = new frmOrdenCompraEstado(grdData.ActiveRow.Cells["v_IdOrdenCompra"].Value.ToString());
                (this.MdiParent as frmMaster).RegistrarForm(this, f);
            }
        }
        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarProveedor")
            {
                Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(txtProveedor.Text.Trim(), "Nombre");
                frm.ShowDialog();
                if (!string.IsNullOrEmpty(frm._RazonSocial))
                {
                    txtProveedor.Text = frm._RazonSocial;
                    txtProveedor.Tag = frm._IdProveedor;
                }
            }
        }

        private void txtProveedor_Validated(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtProveedor.Text))
            {
                txtProveedor.Tag = null;
            }
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                if (btnEditar.Enabled) btnEditar_Click(sender, e);
            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                if (grdData.DisplayLayout.Override.FilterUIType == FilterUIType.Default)
                {
                    grdData.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;
                }
                else
                {
                    grdData.DisplayLayout.Override.FilterUIType = FilterUIType.Default;
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;

            var resp = MessageBox.Show("¿Seguro de eliminar el registro seleccionado?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp == DialogResult.No) return;

            var id = grdData.ActiveRow.Cells["v_IdOrdenCompra"].Value.ToString();
            var objOperationResult = new OperationResult();
            _objOrdenCompraBL.EliminarOrdenCompra(ref objOperationResult, id, Globals.ClientSession.GetAsList());

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            btnBuscar_Click(sender, e);
        }
        #endregion

        #region Methods
        private List<ordendecompraShortDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objOrdenCompraBL.ListarOrdenCompras(ref objOperationResult, dtpFechaRegistroIni.Value.Date, dtpFechaRegistroFin.Value, dtpFechaEntregaIni.Value.Date, dtpFechaEntregaFin.Value.Date, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        #endregion
    }
}
