using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Contabilidad.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.ActivoFijo.BL;
namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.ActivoFijo
{
    public partial class frmCuentasInventarios : Form
    {
        #region  Fields
        AsientosContablesBL _objAsientoContableBL = new AsientosContablesBL();
        cuentasinventariosDto objCuentaInventario = new cuentasinventariosDto();
        ActivoFijoBL _objActivoFijoBL = new ActivoFijoBL();
        public string _mode = string.Empty,  _strFilterExpression ,IdCuentaInventario=String.Empty  ;
        public int TipoActivo = -1;
        #endregion

        public frmCuentasInventarios(string pstrParametro)
        {
            InitializeComponent();
        }

        private void frmUbicacion_Load(object sender, EventArgs e)
        {
            //ActivarBotones(false);
            this.BackColor = new GlobalFormColors().FormColor;
            DetalleView(false);
        }

        #region Methods
        private void EditRow()
        {
            txtTipoActivoFijo.Enabled = false;
            OperationResult objOperationResult = new OperationResult();
            string v_IdCuentaInventario = "";
            _mode = "Edit";
            v_IdCuentaInventario = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdCuentaInventario"].Value.ToString();
            objCuentaInventario = new cuentasinventariosDto();
            objCuentaInventario = _objActivoFijoBL.ObtenerCuentasInventarios(ref objOperationResult, v_IdCuentaInventario);
            txtTipoActivoFijo.Text = objCuentaInventario.CodigoTipoActivo.Trim();
            txtCuenta33.Text = objCuentaInventario.v_Cuenta33.Trim();
            txtCuenta39.Text = objCuentaInventario.v_Cuenta39.Trim();
            txtCuentaGasto.Text = objCuentaInventario.v_CuentaGasto.Trim();
            TipoActivo = objCuentaInventario.i_IdTipoActivo.Value;
        }
        private void DetalleView(bool show)
        {
            gbDatos.Visible = show;
            gbSearch.Visible = !show;
        }

        private void LimpiarDetalle()
        {
            txtTipoActivoFijo.Clear();
            txtCuenta33.Clear();
            txtCuenta39.Clear();
            txtCuentaGasto.Clear();

        }
        private void BindGrid()
        {
            string Ordenar = string.Empty;
            if (_mode == string.Empty)
            {
                Ordenar = "CodigoTipoActivo ASC";
            }
            var objData = GetData(Ordenar, _strFilterExpression);
            grdData.DataSource = objData;
        }
        private List<cuentasinventariosDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objActivoFijoBL.ObtenerListadoCuentasInventarios(ref objOperationResult, pstrSortExpression, pstrFilterExpression);


            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }
        #endregion 

        #region ComportamientoControles
        private void txtTipoActivoFijo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarTipoActivoFijo")
            {
                OperationResult objOperationResult = new OperationResult();
                //Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
                Mantenimientos.ActivoFijo.frmBuscarTipoActivo frm = new Mantenimientos.ActivoFijo.frmBuscarTipoActivo("CRUD",104);    
                frm.ShowDialog();
                txtTipoActivoFijo.Text=frm.CodigoTipoActivoFijo.Trim ();
            }
        }

        private void txtCuenta33_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            string NumeroCuenta1 = txtCuenta33.Text == string.Empty ? "33" : txtCuenta33.Text.Trim();
            Mantenimientos.frmPlanCuentasConsulta frmPlanCuentasConsulta = new Mantenimientos.frmPlanCuentasConsulta(NumeroCuenta1);
           // frmPlanCuentasConsulta.Location = new Point(1500, 500); 
            frmPlanCuentasConsulta.ShowDialog();
            if (frmPlanCuentasConsulta._NroSubCuenta != null)
            {

                txtCuenta33.Text = frmPlanCuentasConsulta._NroSubCuenta.Trim();
            }
            else
            {
                txtCuenta33.Clear();
            }
        }

        private void txtCuenta39_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            string NumeroCuenta1 = txtCuenta39.Text == string.Empty ? "39" : txtCuenta39.Text.Trim();
            Mantenimientos.frmPlanCuentasConsulta frmPlanCuentasConsulta = new Mantenimientos.frmPlanCuentasConsulta(NumeroCuenta1);
            frmPlanCuentasConsulta.ShowDialog();
            if (frmPlanCuentasConsulta._NroSubCuenta != null)
            {

                txtCuenta39.Text = frmPlanCuentasConsulta._NroSubCuenta.Trim();
            }
            else
            {
                txtCuenta39.Clear();
            }
        }

        private void txtCuentaGasto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            string NumeroCuenta1 = txtCuentaGasto.Text == string.Empty ? "68" : txtCuentaGasto.Text.Trim();
            Mantenimientos.frmPlanCuentasConsulta frmPlanCuentasConsulta = new Mantenimientos.frmPlanCuentasConsulta(NumeroCuenta1);
            frmPlanCuentasConsulta.ShowDialog();
            if (frmPlanCuentasConsulta._NroSubCuenta != null)
            {

                txtCuentaGasto.Text = frmPlanCuentasConsulta._NroSubCuenta.Trim();
            }
            else
            {
                txtCuentaGasto.Clear();
            }
        }
        private void txtTipoActivoFijo_Validated(object sender, EventArgs e)
        {

            List<string> Filters = new List<string>();
            OperationResult pobjOperationResult = new OperationResult();
            if (txtTipoActivoFijo.Text.Trim() != string.Empty)
            {

                TipoActivo = _objActivoFijoBL.ObtenerTipoActivoFijo(txtTipoActivoFijo.Text.Trim(),104);

            }
            Filters.Add("CodigoTipoActivo.Contains(\"" + txtTipoActivoFijo.Text.Trim().ToUpper() + "\")");

            _strFilterExpression = null;
            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }

            if (_mode == "New")
            {
                var CuentasInventariosExistentes = _objActivoFijoBL.ObtenerListadoCuentasInventarios(ref pobjOperationResult, "", _strFilterExpression);

                if (CuentasInventariosExistentes.Count() > 0)
                {
                    txtCuenta33.Text = CuentasInventariosExistentes.FirstOrDefault().cuenta33.Trim();
                    txtCuenta39.Text = CuentasInventariosExistentes.FirstOrDefault().cuenta39.Trim();
                    txtCuentaGasto.Text = CuentasInventariosExistentes.FirstOrDefault().cuentagasto.Trim();
                    objCuentaInventario.i_Eliminado = CuentasInventariosExistentes.FirstOrDefault().i_Eliminado.Value;
                    objCuentaInventario.t_InsertaFecha = CuentasInventariosExistentes.FirstOrDefault().t_InsertaFecha.Value;
                    objCuentaInventario.i_IdTipoActivo = CuentasInventariosExistentes.FirstOrDefault().i_IdTipoActivo.Value;
                    TipoActivo = CuentasInventariosExistentes.FirstOrDefault().i_IdTipoActivo.Value ; 
                    _mode = "Edit";
                }
            }
        }

        private void txtBuscarTipoActivoFijo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            if (e.Button.Key == "btnBusqquedaTipoActivoFijo")
            {
                OperationResult objOperationResult = new OperationResult();
                //Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
                Mantenimientos.ActivoFijo.frmBuscarTipoActivo frm = new Mantenimientos.ActivoFijo.frmBuscarTipoActivo("",104);
                frm.ShowDialog();
                txtBuscarTipoActivoFijo.Text  = frm.CodigoTipoActivoFijo.Trim();
            }


        }

        #endregion

        #region Events UI
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            _mode = "New";
            LimpiarDetalle();
            txtTipoActivoFijo.Enabled = true;
            DetalleView(true);
        }
        private void btnGrabar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (uvValidar.Validate(true, false).IsValid)
            {
                if (TipoActivo == -1)
                {
                    UltraMessageBox.Show("Por favor Ingrese Código Correcto para Tipo Activo Fijo ", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTipoActivoFijo.Focus(); 
                    return;
                }

                if ( ( !txtCuenta33.Text.StartsWith("33")  && !txtCuenta33.Text.StartsWith("32") &&  ! txtCuenta33.Text.StartsWith("34") ) || !_objAsientoContableBL.EsImputable(txtCuenta33.Text.Trim()))
                {
                    UltraMessageBox.Show("Por favor Ingrese Cuenta 33 Hist ", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCuenta33.Focus(); 
                    return;
                
                }
                if (!txtCuenta39.Text.StartsWith("39") || !_objAsientoContableBL.EsImputable(txtCuenta39.Text.Trim()))
                {
                    UltraMessageBox.Show("Por favor Ingrese Cuenta 39 Hist ", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCuenta39.Focus();
                    return;

                }
            
                if (_mode == "New")
                    objCuentaInventario = new cuentasinventariosDto();

                objCuentaInventario.i_IdTipoActivo = TipoActivo;
                objCuentaInventario.v_Cuenta33 = txtCuenta33.Text.Trim();
                objCuentaInventario.v_Cuenta39 = txtCuenta39.Text.Trim();
                objCuentaInventario.v_CuentaGasto = txtCuentaGasto.Text.Trim();

                if (_mode == "New")
                    IdCuentaInventario = _objActivoFijoBL.InsertaCuentasInventarios(ref objOperationResult, objCuentaInventario, Globals.ClientSession.GetAsList());                
                else //_mode == "Edit"
                   IdCuentaInventario= _objActivoFijoBL.ActualizarActivoFijo(ref objOperationResult, objCuentaInventario, Globals.ClientSession.GetAsList()); 

                if (objOperationResult.Success == 1)
                {
                    this.InvokeOnClick(btnBuscar, e);
                    MantenerSeleccion(IdCuentaInventario);

                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistemas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //LimpiarDetalle();
                    //DetalleOFF();
                    // btnGrabar.Enabled = false;
                    //btnEliminar.Enabled = false;
                }
                else
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                _objActivoFijoBL.EliminarCuentasInventarios(ref objOperationResult, grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdCuentaInventario"].Value.ToString(), Globals.ClientSession.GetAsList());
                this.InvokeOnClick(btnBuscar, e);
            }
        }
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();

            if (!string.IsNullOrEmpty(txtBuscarTipoActivoFijo.Text)) Filters.Add("CodigoTipoActivo.Contains(\"" + txtBuscarTipoActivoFijo.Text.Trim().ToUpper() + "\")");
            // if (!string.IsNullOrEmpty(txtBuscarNombre.Text)) Filters.Add("v_Nombre.Contains(\"" + txtBuscarNombre.Text.Trim().ToUpper() + "\")");
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
            DetalleView(false);
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DetalleView(false);
        }
        #endregion

        #region Grilla
        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["v_IdCuentaInventario"].Text == ValorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }
        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (gbDatos.Visible)
                EditRow();
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));
            btnEliminar.Enabled = row != null;
        }

        private void grdData_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            DetalleView(true);
            EditRow();
        }
        #endregion

    }
}
