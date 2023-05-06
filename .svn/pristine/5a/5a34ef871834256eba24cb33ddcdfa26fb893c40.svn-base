using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Venta.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource; 


namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmCondicionPago : Form
    {
        #region Fields
        CondicionPagoBL _objCondicionPagoBL = new CondicionPagoBL();
        condicionpagoDto _condicionpagoDto;

        string _strFilterExpression;
        string _mode;
        #endregion

        public frmCondicionPago(string Parametro)
        {
            InitializeComponent();
        }

        private void frmCondicionPago_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            _mode = "New";
            DetalleView(false);
        }

        #region Methods
        private void BindGrid()
        {
            var objData = GetData("v_NombreCondicion ASC", _strFilterExpression);
            grdData.DataSource = objData;
        }

        private List<condicionpagoDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objCondicionPagoBL.ObtenerListadoCondicionPago(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _objData;
        }

        private void LimpiarDetalle()
        {
            txtNombre.Clear();
            chkCredito.Checked = false;
            txtDias.Clear();
        }

        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["v_IdCondicionPago"].Text == ValorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        private void DetalleView(bool show)
        {
            gbDatos.Visible = show;
            gbSearch.Visible = !show;
        }

        private void LoadRow()
        {
            if (grdData.ActiveRow != null)
            {
                OperationResult objOperationResult = new OperationResult();

                _mode = "Edit";
                var _IdCondicionPago = (grdData.ActiveRow.ListObject as condicionpagoDto).v_IdCondicionPago;
                _condicionpagoDto = new condicionpagoDto();
                _condicionpagoDto = _objCondicionPagoBL.ObtenerCondicionPago(ref objOperationResult, _IdCondicionPago);
                txtNombre.Text = _condicionpagoDto.v_NombreCondicion;
                chkCredito.Checked = _condicionpagoDto.i_CreditoLetras == 0 ? false : true;
                txtDias.Text = _condicionpagoDto.i_Dias == 0 ? string.Empty : _condicionpagoDto.i_Dias.ToString();
            }
        }
        #endregion

        #region Events UI
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            if (!string.IsNullOrEmpty(txtBuscarNombre.Text)) Filters.Add("v_NombreCondicion.Contains(\"" + txtBuscarNombre.Text.Trim().ToUpper() + "\")");
            Filters.Add("i_Eliminado==0");

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
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            _mode = "New";
            LimpiarDetalle();
            DetalleView(true);
            txtNombre.Focus();
            chkCredito.Checked = false;
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvCondicionPago.Validate(true, false).IsValid)
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    UltraMessageBox.Show("Por favor ingrese un Nombre.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
             
                if (_mode == "New")
                    _condicionpagoDto = new condicionpagoDto();

                _condicionpagoDto.v_NombreCondicion = txtNombre.Text.Trim();
                _condicionpagoDto.i_CreditoLetras = chkCredito.Checked == true ? 1 : 0;
                _condicionpagoDto.i_Dias = txtDias.Text == string.Empty ? 0 : int.Parse(txtDias.Text.ToString());

                if (_mode == "New")
                    _objCondicionPagoBL.InsertarCondicionPago(ref objOperationResult, _condicionpagoDto, Globals.ClientSession.GetAsList());
                else //Edit
                    _objCondicionPagoBL.ActualizarCondicionPago(ref objOperationResult, _condicionpagoDto, Globals.ClientSession.GetAsList());

                if (objOperationResult.Success == 1)
                {
                    btnBuscar_Click(sender, e);
                    MantenerSeleccion(_condicionpagoDto.v_IdCondicionPago);
                    DetalleView(false);
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistemas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DetalleView(false);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            DialogResult Resultado = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Resultado == DialogResult.Yes)
            {
                _objCondicionPagoBL.EliminarCondicionPago(ref objOperationResult, grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdCondicionPago"].Value.ToString(), Globals.ClientSession.GetAsList());
                this.InvokeOnClick(btnBuscar, new EventArgs());
            }
        }

        private void txtDias_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtDias, e);
        }
        #endregion

        #region Grilla

        private void grdData_MouseDown_1(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));
            btnEliminar.Enabled = (row != null);
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (!gbDatos.Visible) return;
            LoadRow();
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            DetalleView(true);
            LoadRow();
            txtNombre.Focus();
        }
        #endregion

    }
}
