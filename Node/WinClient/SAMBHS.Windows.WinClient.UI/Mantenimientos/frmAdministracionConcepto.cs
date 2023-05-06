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
using SAMBHS.Contabilidad.BL;


namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmAdministracionConcepto : Form
    {
        string _strFilterExpression;
        administracionconceptosDto _administracionconceptosDto = new administracionconceptosDto();
        AdministracionConceptosBL _objAdministracionConceptosBL = new AdministracionConceptosBL();
        AsientosContablesBL _objAsientosContablesBL = new AsientosContablesBL();
        ConceptoBL _objConceptoBL = new ConceptoBL();
        asientocontableDto CuentaVenta = new asientocontableDto();
        asientocontableDto CuentaIgv = new asientocontableDto();
        asientocontableDto CuentaDetraccion = new asientocontableDto();
        string _mode;
        public frmAdministracionConcepto(string parametro)
        {
            InitializeComponent();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            List<string> Filters = new List<string>();
            string codigo;
            //string pstrSortExpression = "v_Codigo ASC";
            if (e.KeyCode == Keys.Enter)
            {
                txtNombre.Clear();
                if (txtCodigo.Text == "") return;
                //Verificamos si ha escrito el codigo ya no sale pop-pup
                codigo = txtCodigo.Text.Trim();
                if (!string.IsNullOrEmpty(txtCodigo.Text)) Filters.Add("v_Codigo.Contains(\"" + codigo + "\")");
                _strFilterExpression = null;
                if (Filters.Count > 0)
                {
                    foreach (string item in Filters)
                    {
                        _strFilterExpression = _strFilterExpression + item + " && ";
                    }
                    _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                }

                var _objData = _objConceptoBL.ObtenerCodigoConcepto(ref objOperationResult, codigo);

                if (_objData != null)
                {

                    txtNombre.Text = _objData.v_Nombre;
                }
                else
                {
                    txtNombre.Text = String.Empty;
                    _administracionconceptosDto.v_IdAdministracionConceptos = null;
                    UltraMessageBox.Show("Concepto  No Existe ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    //Mantenimientos.frmBuscarConcepto frm = new Mantenimientos.frmBuscarConcepto(txtCodigo.Text);
                    //frm.ShowDialog();
                    //if (frm._IdConcepto != null)
                    //{
                    //    txtCodigo.Text = frm._CodigoConcepto;
                    //    txtNombre.Text = frm._NombreConcepto;
                    //    _administracionconceptosDto.v_IdAdministracionConceptos = frm._IdConcepto;

                    //}
                }
            }

        }

        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {

        }

        private void frmAdministracionConcepto_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;

            txtNombre.Enabled = false;
            btnGrabar.Enabled = false;
            btnEliminar.Enabled = false;

            DetalleOFF();


        }

        private void txtCuentaVenta_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    if (txtCuentaVenta.Text == "") return;
            //    Mantenimientos.frmPlanCuentasConsulta frm = new Mantenimientos.frmPlanCuentasConsulta(txtCuentaVenta.Text.Trim());
            //    frm.ShowDialog();
            //    if (frm._NroSubCuenta != null)
            //    {
            //        txtCuentaVenta.Text = frm._NroSubCuenta;
            //        txtCventa.Text = frm._NombreCuenta;

            //    }
            //}
        }

        private void txtCuentaIgv_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    if (txtCuentaIgv.Text == "") return;
            //    Mantenimientos.frmPlanCuentasConsulta frm = new Mantenimientos.frmPlanCuentasConsulta(txtCuentaIgv.Text.Trim());
            //    frm.ShowDialog();
            //    if (frm._NroSubCuenta != null)
            //    {
            //        txtCuentaIgv.Text = frm._NroSubCuenta;

            //        txtCigv.Text = frm._NombreCuenta;
            //        //_administracionconceptosDto.v_IdAdministracionConceptos = frm._NroSubCuenta;

            //    }
            //}
        }

        private void txtCuentaDetraccion_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    if (txtCuentaDetraccion.Text == "") return;
            //    Mantenimientos.frmPlanCuentasConsulta frm = new Mantenimientos.frmPlanCuentasConsulta(txtCuentaDetraccion.Text.Trim());
            //    frm.ShowDialog();
            //    if (frm._NroSubCuenta != null)
            //    {
            //        txtCuentaDetraccion.Text = frm._NroSubCuenta;

            //        txtCdetraccion.Text = frm._NombreCuenta;
            //        //_administracionconceptosDto.v_IdAdministracionConceptos = frm._NroSubCuenta;

            //    }
            //}
        }

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
            if (grdData.Rows.Count() == 0)
            {
                DetalleOFF();
                btnEliminar.Enabled = false;
                LimpiarDetalle();
                btnGrabar.Enabled = false;
            }
            else
            {
                DetalleON();
                btnEliminar.Enabled = true;
                btnGrabar.Enabled = true;
            }
        }


        private void BindGrid()
        {
            var objData = GetData("v_Codigo ASC", _strFilterExpression);

            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<administracionconceptosDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objAdministracionConceptosBL.ObtenerListadoAdministracionConceptos(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }


        private void DetalleOFF()
        {
            txtCodigo.Enabled = false;
            txtCuentaVenta.Enabled = false;
            txtCuentaIgv.Enabled = false;
            txtCuentaDetraccion.Enabled = false;
            txtCventa.Enabled = false;
            txtCigv.Enabled = false;
            txtCdetraccion.Enabled = false;
            btnBuscarCodigo.Enabled = false;
            btnBuscarCventa.Enabled = false;
            btnBuscarCigv.Enabled = false;
            btnBuscarCdetraccion.Enabled = false;

        }

        private void DetalleON()
        {
            txtCodigo.Enabled = true;
            txtCuentaVenta.Enabled = true;
            txtCuentaIgv.Enabled = true;
            txtCuentaDetraccion.Enabled = true;
            btnBuscarCodigo.Enabled = true;
            btnBuscarCventa.Enabled = true;
            btnBuscarCigv.Enabled = true;
            btnBuscarCdetraccion.Enabled = true;

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            _mode = "New";
            LimpiarDetalle();
            DetalleON();
            btnGrabar.Enabled = true;
            txtCodigo.Focus();
        }

        private void LimpiarDetalle()
        {
            txtNombre.Clear();
            txtCodigo.Clear();
            txtCuentaVenta.Clear();
            txtCuentaIgv.Clear();
            txtCuentaDetraccion.Clear();
            txtCodigo.Enabled = false;
            txtCventa.Enabled = false;
            txtCigv.Enabled = false;
            txtCdetraccion.Enabled = false;

        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();


            if (!_objAdministracionConceptosBL.ExistenciaAdmConceptosDiversosProcesos(int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["v_Codigo"].Value.ToString())))
            {
                DialogResult Resultado = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Resultado == DialogResult.Yes)
                {
                    _objAdministracionConceptosBL.EliminarAdministracionConceptos(ref objOperationResult, grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdAdministracionConceptos"].Value.ToString(), Globals.ClientSession.GetAsList());
                    btnBuscar_Click(sender, e);
                    LimpiarDetalle();
                }
            }
            else
            {
                UltraMessageBox.Show("No se puede eliminar,este código esta siendo utilizado en Otros procesos" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {

            string IdAdministracion = String.Empty;
            OperationResult objOperationResult = new OperationResult();

            if (uvAdministracionConceptos.Validate(true, false).IsValid)
            {

                if (txtNombre.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Codigo Concepto Correcto", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCodigo.Focus();
                    return;
                }

                if (txtCdetraccion.Text.Trim() == string.Empty & txtCigv.Text.Trim() == string.Empty & txtCventa.Text.Trim() == string.Empty)
                {
                    UltraMessageBox.Show("Por favor ingrese al menos una de las Cuentas Imputables", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                     
                }

                if (txtCuentaDetraccion.Text .Trim () != string.Empty && CuentaDetraccion == null)
                {
                    UltraMessageBox.Show("Por favor ingrese Cuenta Detracción Imputable", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    txtCuentaDetraccion.Focus();
                    return;
                
                }

                if (txtCuentaVenta.Text .Trim () != string.Empty && CuentaVenta == null)
                {
                    UltraMessageBox.Show("Por favor ingrese Cuenta Venta Imputable", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCuentaVenta.Focus(); 
                    return;
                }

                if (txtCuentaIgv.Text.Trim () != String.Empty && CuentaIgv == null)
                {
                    UltraMessageBox.Show("Por favor ingrese Cuenta Igv Imputable", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtCuentaIgv.Focus();
                    return;
                
                }
                if (_mode == "New")
                {
                    //var Administracion = _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod(ref objOperationResult, txtCodigo.Text.Trim());
                    //if (Administracion != null)
                    //{

                    //    UltraMessageBox.Show("Código de Administración de Conceptos ya Existe", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //    txtCodigo.Focus();
                    //    return;
                    //}
                    _administracionconceptosDto = new administracionconceptosDto();
                    _administracionconceptosDto.v_Codigo = txtCodigo.Text.Trim();
                    _administracionconceptosDto.v_Nombre = txtNombre.Text.Trim();
                    _administracionconceptosDto.v_CuentaPVenta = txtCuentaVenta.Text.Trim();
                    _administracionconceptosDto.v_CuentaIGV = txtCuentaIgv.Text.Trim();
                    _administracionconceptosDto.v_CuentaDetraccion = txtCuentaDetraccion.Text.Trim();
                    _administracionconceptosDto.v_Periodo = Globals.ClientSession.i_Periodo.ToString();
                   IdAdministracion= _objAdministracionConceptosBL.InsertarAdministracionConceptos(ref objOperationResult, _administracionconceptosDto, Globals.ClientSession.GetAsList());

                }
                else if (_mode == "Edit")
                {

                    _administracionconceptosDto.v_Codigo = txtCodigo.Text.Trim();
                    _administracionconceptosDto.v_Nombre = txtNombre.Text.Trim();
                    _administracionconceptosDto.v_CuentaPVenta = txtCuentaVenta.Text.Trim();
                    _administracionconceptosDto.v_CuentaIGV = txtCuentaIgv.Text.Trim();
                    _administracionconceptosDto.v_CuentaDetraccion = txtCuentaDetraccion.Text.Trim();
                    _administracionconceptosDto.v_Periodo = Globals.ClientSession.i_Periodo.ToString();
                   IdAdministracion= _objAdministracionConceptosBL.ActualizarAdministracionConceptos(ref objOperationResult, _administracionconceptosDto, Globals.ClientSession.GetAsList());

                }

                if (objOperationResult.Success == 1)
                {
                    btnBuscar_Click(sender, e);
                   // MantenerSeleccion(_administracionconceptosDto.v_IdAdministracionConceptos);
                    MantenerSeleccion(IdAdministracion);
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistemas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //LimpiarDetalle();
                    //DetalleOFF();
                    //btnGrabar.Enabled = false;
                    //btnEliminar.Enabled = false;
                }
                else
                {
                    UltraMessageBox.Show("Ocurrió un error : "+objOperationResult.AdditionalInformation +"\n"+ objOperationResult.ErrorMessage  , "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

                   
                }
            }
        }


        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["v_IdAdministracionConceptos"].Text == ValorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string _IdAdministracionConceptos = "";
            _mode = "Edit";
            _IdAdministracionConceptos = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdAdministracionConceptos"].Value.ToString();
            _administracionconceptosDto = new administracionconceptosDto();
            _administracionconceptosDto = _objAdministracionConceptosBL.ObtenerAdministracionConceptos(ref objOperationResult, _IdAdministracionConceptos);
            txtCodigo.Text = _administracionconceptosDto.v_Codigo;
            txtNombre.Text = _administracionconceptosDto.v_Nombre;
            txtCuentaVenta.Text = _administracionconceptosDto.v_CuentaPVenta;
            txtCuentaIgv.Text = _administracionconceptosDto.v_CuentaIGV;
            txtCuentaDetraccion.Text = _administracionconceptosDto.v_CuentaDetraccion;
            txtCodigo.Enabled = false;
            txtCdetraccion.Enabled = false;
            txtCigv.Enabled = false;
            txtCventa.Enabled = false;
            btnBuscarCodigo.Enabled = false; 
            var asientoContableVenta = _objAsientosContablesBL.ObtenAsientosPorNro(ref objOperationResult, txtCuentaVenta.Text);
            if (asientoContableVenta != null)
            {
                txtCventa.Text = asientoContableVenta.v_NombreCuenta;

            }
            else
            {
                txtCventa.Clear();
            }

            var asientoContableIgv = _objAsientosContablesBL.ObtenAsientosPorNro(ref objOperationResult, txtCuentaIgv.Text);
            if (asientoContableIgv != null)
            {
                txtCigv.Text = asientoContableIgv.v_NombreCuenta;
            }
            else
            {
                txtCigv.Clear();
            }
            var asientoContableDetraccion = _objAsientosContablesBL.ObtenAsientosPorNro(ref objOperationResult, txtCuentaDetraccion.Text);

            if (asientoContableDetraccion != null)
            {
                txtCdetraccion.Text = asientoContableDetraccion.v_NombreCuenta;
            }
            else
            {
                txtCdetraccion.Clear();
            }


        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row == null)
            {
                btnEliminar.Enabled = false;
            }
            else
            {
                btnEliminar.Enabled = true;
            }
        }

        private void grdData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {

        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {

        }

        private void btnBuscarCodigo_Click(object sender, EventArgs e)
        {
            //if (txtCliente.Text.Trim().Length < 3)
            //{
            //    UltraMessageBox.Show("Por favor ingrese 3 caracteres para iniciar una búsqueda.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            Mantenimientos.frmBuscarConcepto frm = new Mantenimientos.frmBuscarConcepto(txtCodigo.Text.Trim()); // le mando N de que se esta utilizando en la NOTA DE SALIDA
            frm.ShowDialog();

            if (frm._IdConcepto != null)
            {
                txtCodigo.Text = frm._CodigoConcepto;
                txtNombre.Text = frm._NombreConcepto;
                _administracionconceptosDto.v_IdAdministracionConceptos = frm._IdConcepto;
                txtCodigo_Leave(sender, e);

            }
        }

        private void btnBuscarCventa_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmPlanCuentasConsulta frm = new Mantenimientos.frmPlanCuentasConsulta(txtCuentaVenta.Text.Trim()); // le mando N de que se esta utilizando en la NOTA DE SALIDA
            frm.ShowDialog();

            if (frm._NroSubCuenta != null)
            {

                txtCuentaVenta.Text = frm._NroSubCuenta;
                txtCventa.Text = frm._NombreCuenta;
                CuentaVenta = _objAsientosContablesBL.ObtenerAsientoImputable(txtCuentaVenta.Text.Trim());
                //_administracionconceptosDto.v_IdAdministracionConceptos = frm._IdConcepto;

            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void txtCuentaDetraccion_TextChanged(object sender, EventArgs e)
        {
            if (txtCuentaDetraccion.Text == "")
            {

                txtCdetraccion.Text = "";
            }
        }

        private void btnBuscarCigv_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmPlanCuentasConsulta frm = new Mantenimientos.frmPlanCuentasConsulta(txtCuentaIgv.Text.Trim()); // le mando N de que se esta utilizando en la NOTA DE SALIDA
            frm.ShowDialog();

            if (frm._NroSubCuenta != null)
            {

                txtCuentaIgv.Text = frm._NroSubCuenta;

                txtCigv.Text = frm._NombreCuenta;
                CuentaIgv = _objAsientosContablesBL.ObtenerAsientoImputable(txtCuentaIgv.Text.Trim());
                //_administracionconceptosDto.v_IdAdministracionConceptos = frm._IdConcepto;

            }
        }

        private void btnBuscarCdetraccion_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmPlanCuentasConsulta frm = new Mantenimientos.frmPlanCuentasConsulta(txtCuentaDetraccion.Text.Trim()); // le mando N de que se esta utilizando en la NOTA DE SALIDA
            frm.ShowDialog();

            if (frm._NroSubCuenta != null)
            {

                txtCuentaDetraccion.Text = frm._NroSubCuenta;
                txtCdetraccion.Text = frm._NombreCuenta;
                CuentaDetraccion = _objAsientosContablesBL.ObtenerAsientoImputable(txtCuentaDetraccion.Text.Trim());
                //_administracionconceptosDto.v_IdAdministracionConceptos = frm._IdConcepto;

            }
        }

        private void txtCuentaVenta_TextChanged(object sender, EventArgs e)
        {
            if (txtCuentaVenta.Text == "")
            {
                txtCventa.Text = "";
            }
        }

        private void txtCuentaIgv_TextChanged(object sender, EventArgs e)
        {
            if (txtCuentaIgv.Text == "")
            {
                txtCigv.Text = "";
            }
        }

        private void txtBuscarCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    btnBuscar_Click(sender, e);
            //}
        }

        private void txtBuscarCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtBuscarCodigo, e);
        }

        private void txtBuscarNombre_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    btnBuscar_Click(sender, e);
            //}
        }

        private void txtCuentaVenta_Leave(object sender, EventArgs e)
        {
            
            if (CuentaVenta != null)
            {

                txtCventa.Text = CuentaVenta.v_NombreCuenta;
               
            }
            else
            {
                //UltraMessageBox.Show("Cuenta No Existe en Plan Cuentas", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //txtCventa.Clear();
            }
        }

        private void txtCuentaIgv_Leave(object sender, EventArgs e)
        {
           CuentaIgv = _objAsientosContablesBL.ObtenerAsientoImputable(txtCuentaIgv.Text.Trim());
            if (CuentaIgv != null)
            {
                txtCigv.Text = CuentaIgv.v_NombreCuenta;
            }
            else
            {
                //UltraMessageBox.Show("Cuenta No Existe en Plan Cuentas", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //txtCigv.Clear();
            }
        }

        private void txtCuentaDetraccion_Leave(object sender, EventArgs e)
        {
            CuentaDetraccion = _objAsientosContablesBL.ObtenerAsientoImputable(txtCuentaDetraccion.Text.Trim());
            if (CuentaDetraccion != null)
            {

                txtCdetraccion.Text = CuentaDetraccion.v_NombreCuenta;
            }
            else
            {
                //UltraMessageBox.Show("Cuenta No Existe en Plan Cuentas", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //txtCdetraccion.Clear();
            }
        }

        private void txtCodigo_Leave(object sender, EventArgs e)
        {

            if (txtCodigo.Text == string.Empty) return;
            txtCodigo.Text = int.Parse(txtCodigo.Text.Trim()).ToString("00");
            OperationResult objOperatioResult = new OperationResult();
            var ExisteConcepto = _objConceptoBL.ObtenerCodigoConcepto(ref objOperatioResult, txtCodigo.Text.Trim());
            if (ExisteConcepto != null)
            {
                txtNombre.Text = ExisteConcepto.v_Nombre.Trim();
                
            }
            else
            {
                txtNombre.Clear();
               
            }
            var AdministracionConcepto =  _objAdministracionConceptosBL.ObtenerAdministracionConceptosxCod (ref objOperatioResult,txtCodigo.Text.Trim () );
            if (AdministracionConcepto != null)
            {
                txtCuentaDetraccion.Text = AdministracionConcepto.v_CuentaDetraccion.Trim ();
                txtCuentaIgv.Text = AdministracionConcepto.v_CuentaIGV.Trim ();
                txtCuentaVenta.Text = AdministracionConcepto.v_CuentaPVenta.Trim();
                //var Cventa=_objAsientosContablesBL.ObtenAsientosPorNro();
                CuentaVenta = _objAsientosContablesBL.ObtenerAsientoImputable( txtCuentaVenta.Text.Trim());
                if (CuentaVenta != null)
                {
                    txtCventa.Text = CuentaVenta.v_NombreCuenta.Trim();
                }
                CuentaDetraccion = _objAsientosContablesBL.ObtenerAsientoImputable( txtCuentaDetraccion.Text.Trim());
                if (CuentaDetraccion != null)
                {
                    txtCdetraccion.Text = CuentaDetraccion.v_NombreCuenta.Trim(); 
                }

                CuentaIgv =_objAsientosContablesBL.ObtenAsientosPorNro(ref objOperatioResult,txtCuentaIgv.Text.Trim ());
                if (CuentaIgv != null)
                {
                    txtCigv.Text = CuentaIgv.v_NombreCuenta.Trim();
                
                }
                _mode = "Edit";
            }
            else
            {
                txtCuentaDetraccion.Clear();
                txtCuentaVenta.Clear();
                txtCuentaIgv.Clear();
                _mode = "New";
            }
           
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCodigo, e);
        }

        private void txtCuentaVenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCuentaVenta ,e);
        }

        private void txtCuentaIgv_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCuentaIgv, e);
        }

        private void txtCuentaDetraccion_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCuentaDetraccion, e);
        }
    }
}
