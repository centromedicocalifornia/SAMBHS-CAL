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
using SAMBHS.Compra.BL;
using SAMBHS.Contabilidad.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmConcepto : Form
    {
        #region Fields
        ConceptoBL _objConceptoBL = new ConceptoBL();
        string _strFilterExpression, _mode;
        conceptoDto _conceptoDto;
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        AdministracionConceptosBL _objAdministracionConceptosBL = new AdministracionConceptosBL();
        public string IdConcepto = string.Empty;
        #endregion

        public frmConcepto(string parametro)
        {
            InitializeComponent();
        }

        private void frmConcepto_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboArea, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 28, null), DropDownListAction.Select);
            btnEliminar.Enabled = false;
            DetalleView(false);
        }

        #region Private Methods
        
        private void DetalleView(bool show)
        {
            gbDatos.Visible = show;
            gbSearch.Visible = !show;
        }

        private void LimpiarDetalle()
        {
            txtNombre.Clear();
            //cboArea.SelectedValue =-1;
            txtCodigo.Clear();
        }

        private void BindGrid()
        {
            var objData = GetData("v_Codigo ASC", _strFilterExpression);
            grdData.DataSource = objData;
        }

        private List<conceptoDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objConceptoBL.ObtenerListadoConcepto(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["v_IdConcepto"].Text == ValorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        private void EditRow()
        {
            txtCodigo.Enabled = false;
            OperationResult objOperationResult = new OperationResult();
            string _IdConcepto = "";
            _mode = "Edit";
            _IdConcepto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdConcepto"].Value.ToString();
            _conceptoDto = new conceptoDto();
            _conceptoDto = _objConceptoBL.ObtenerConcepto(ref objOperationResult, _IdConcepto);
            txtNombre.Text = _conceptoDto.v_Nombre;
            txtCodigo.Text = _conceptoDto.v_Codigo;
            cboArea.Value = _conceptoDto.i_IdArea.ToString();
        }
        #endregion

        #region Evetns UI
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();

            if (!string.IsNullOrEmpty(txtBuscarCodigo.Text)) Filters.Add("v_Codigo.Contains(\"" + txtBuscarCodigo.Text.Trim().ToUpper() + "\")");
            if (!string.IsNullOrEmpty(txtBuscarNombre.Text)) Filters.Add("v_Nombre.Contains(\"" + txtBuscarNombre.Text.Trim().ToUpper() + "\")");
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
            DetalleView(false);
            btnEliminar.Enabled = grdData.Rows.Count != 0; 
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            OperationResult objOperationResult = new OperationResult();
            List<string> Filters = new List<string>();
            var v_Codigo = (grdData.ActiveRow.ListObject as conceptoDto).v_Codigo;
            Filters.Add("v_Codigo.Contains(\"" + v_Codigo + "\")");
            _strFilterExpression = null;

            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }

            string pstrSortExpression = "v_Codigo ASC";

            var _objData = _objAdministracionConceptosBL.ObtenerListadoAdministracionConceptos(ref objOperationResult, pstrSortExpression, _strFilterExpression);

            if (!_objData.Any())
            {

                DialogResult Resultado = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Resultado == DialogResult.Yes)
                {
                    _objConceptoBL.EliminarConcepto(ref objOperationResult, (grdData.ActiveRow.ListObject as conceptoDto).v_IdConcepto, Globals.ClientSession.GetAsList());
                    this.InvokeOnClick(btnBuscar, new EventArgs());
                }
            }
            else
            {
                 UltraMessageBox.Show("No se puede eliminar,este código esta siendo utilizado en Administracion de Conceptos" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            if (uvConcepto.Validate(true, false).IsValid)
            {
                if (txtCodigo.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Codigo Correcto.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_mode == "New")
                    _conceptoDto = new conceptoDto();
               
                _conceptoDto.v_Codigo = txtCodigo.Text.Trim();
                _conceptoDto.v_Nombre = txtNombre.Text.Trim();
                _conceptoDto.i_IdArea = int.Parse(cboArea.Value.ToString());
                _conceptoDto.v_Periodo = Globals.ClientSession.i_Periodo.ToString();
                OperationResult objOperationResult = new OperationResult();
                if (_mode == "New")
                    IdConcepto = _objConceptoBL.InsertarConcepto(ref objOperationResult, _conceptoDto, Globals.ClientSession.GetAsList());
                else // "Edit"
                    IdConcepto = _objConceptoBL.ActualizarConcepto(ref objOperationResult, _conceptoDto, Globals.ClientSession.GetAsList());

                if (objOperationResult.Success == 1)
                {
                    btnBuscar_Click(sender, e);
                    MantenerSeleccion(IdConcepto);
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistemas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DetalleView(false);
                }
                else
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            _mode = "New";
            LimpiarDetalle();
            txtCodigo.Enabled = true;
            DetalleView(true);
            txtCodigo.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DetalleView(false);
        }

        #region Event Txt

        private void txtNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCodigo, e);
        }

        private void txtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                this.InvokeOnClick(btnBuscar, e);
        }
        #endregion

        private void txtCodigo_Validated(object sender, EventArgs e)
        {
            if (txtCodigo.Text == String.Empty) return;
            txtCodigo.Text = int.Parse(txtCodigo.Text.Trim()).ToString("00");
            OperationResult objOperatioResult = new OperationResult();
            var ExisteConcepto = _objConceptoBL.ObtenerCodigoConcepto(ref objOperatioResult, txtCodigo.Text.Trim());
            if (ExisteConcepto != null)
            {
                txtNombre.Text = ExisteConcepto.v_Nombre.Trim();
                _mode = "Edit";
            }
            else
            {
                txtNombre.Clear();
            }
        }
        #endregion

        #region Events Grilla

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            btnEliminar.Enabled = (row != null);
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (gbDatos.Visible)
                EditRow();
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            EditRow();
            DetalleView(true);
        }
        #endregion

    }
}
