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


namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmGastoImportacion : Form
    {
        #region Fields
        GastosImportacionBL _objGastoImportacionBL = new GastosImportacionBL();
        gastosimportacionDto _gastoImportacionDto;
        string _strFilterExpression;
        string _mode;
        #endregion

        public frmGastoImportacion(string Parametro)
        {
            InitializeComponent();

            DetalleView(false);
            this.BackColor = new GlobalFormColors().FormColor;
            btnEliminar.Enabled = false;
            //this.grdData.KeyDown += (_, e) =>
            //{
            //    if (e.KeyCode == Keys.Enter) EditarRow();
            //};
            this.txtBuscarNombre.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter) this.InvokeOnClick(btnBuscar, new EventArgs());
            };
        }

        #region Methods Privates
        private void DetalleView(bool show)
        {
            gbDatos.Visible = show;
            gbBusqueda.Visible = !show;
        }
        private void BindGrid()
        {
            var objData = GetData("v_Codigo ASC", _strFilterExpression);
            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<gastosimportacionDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();

            var _objData = _objGastoImportacionBL.ListarGastosImportacion(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void LimpiarDetalle()
        {
            txtBuscarNombre.Clear();
            txtNombre.Clear();
            txtCuenta.Clear();
            txtCcuenta.Clear();
            txtCodigo.Clear();
        }

        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["v_GastoImportacion"].Text == ValorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        private void EditarRow()
        {
            if (grdData.ActiveRow == null) return;
            OperationResult objOperationResult = new OperationResult();
            string _v_GastoImportacion = "";
            _mode = "Edit";
            //LimpiarDetalle();
            _v_GastoImportacion = (grdData.ActiveRow.ListObject as gastosimportacionDto).v_GastoImportacion;
            _gastoImportacionDto = _objGastoImportacionBL.ObtenerGastosImportacion(ref objOperationResult, _v_GastoImportacion);
            txtNombre.Text = _gastoImportacionDto.v_Nombre;
            txtCuenta.Text = _gastoImportacionDto.v_Cuenta;
            txtCcuenta.Text = _gastoImportacionDto.v_CCuenta;
            txtCodigo.Text = _gastoImportacionDto.v_Codigo;
            DetalleView(true);
        }
        #endregion

        #region Events UI
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
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
            btnEliminar.Enabled = (grdData.Rows.Count() > 0);

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            _mode = "New";
            LimpiarDetalle();
            DetalleView(true);
            btnGrabar.Enabled = true;
            txtCodigo.Focus();

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            DialogResult Resultado = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Resultado == DialogResult.Yes)
            {
                _objGastoImportacionBL.EliminarGastosImportacion(ref objOperationResult, (grdData.ActiveRow.ListObject as gastosimportacionDto).v_GastoImportacion, Globals.ClientSession.GetAsList());
                this.InvokeOnClick(btnBuscar, e);
                LimpiarDetalle();
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvGastoImportacion.Validate(true, false).IsValid)
            {
                if (txtNombre.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Nombre.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (_gastoImportacionDto != null)
                {
                    if (_objGastoImportacionBL.ObtenerCodigo(ref objOperationResult, txtCodigo.Text, _gastoImportacionDto.v_GastoImportacion) != null)
                    {
                        UltraMessageBox.Show("Este Código  ya ha sido registrado", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }


                if (_mode == "New")
                {


                    _gastoImportacionDto = new gastosimportacionDto();
                    _gastoImportacionDto.v_Codigo = txtCodigo.Text.Trim();
                    _gastoImportacionDto.v_Nombre = txtNombre.Text.Trim();
                    _gastoImportacionDto.v_Cuenta = txtCuenta.Text.Trim();
                    _gastoImportacionDto.v_CCuenta = txtCcuenta.Text.Trim();


                    _objGastoImportacionBL.InsertarGastosImportacion(ref objOperationResult, _gastoImportacionDto, Globals.ClientSession.GetAsList());
                }
                else if (_mode == "Edit")
                {
                    _gastoImportacionDto.v_Codigo = txtCodigo.Text.Trim();
                    _gastoImportacionDto.v_Nombre = txtNombre.Text.Trim();
                    _gastoImportacionDto.v_Cuenta = txtCuenta.Text.Trim();
                    _gastoImportacionDto.v_CCuenta = txtCcuenta.Text.Trim();
                    _objGastoImportacionBL.ActualizarGastosImportacion(ref objOperationResult, _gastoImportacionDto, Globals.ClientSession.GetAsList());
                }

                if (objOperationResult.Success == 1)
                {
                    btnBuscar_Click(sender, e);
                    MantenerSeleccion(_gastoImportacionDto.v_GastoImportacion);
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistemas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //LimpiarDetalle();
                    DetalleView(false);

                }
                else
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            this.EditarRow();
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (gbDatos.Visible) EditarRow();
        }
        private void txtCuenta_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            frmPlanCuentasConsulta formPlanCuentas = new frmPlanCuentasConsulta("65");
            formPlanCuentas.ShowDialog();

            if (formPlanCuentas._NroSubCuenta != null)
                txtCuenta.Text = formPlanCuentas._NroSubCuenta.Trim();

        }

        private void txtCcuenta_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            frmPlanCuentasConsulta formPlanCuentas = new frmPlanCuentasConsulta("");
            formPlanCuentas.ShowDialog();

            if (formPlanCuentas._NroSubCuenta != null)
                txtCcuenta.Text = formPlanCuentas._NroSubCuenta.Trim();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DetalleView(false);
        }
        #endregion


    }
}
