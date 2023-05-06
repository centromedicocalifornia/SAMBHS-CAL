using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using SAMBHS.Contabilidad.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmCuentasMayor : Form
    {
        AsientosContablesBL _objBL = new AsientosContablesBL();
        public asientocontableDto _objAsientosDto = new asientocontableDto();
        asientocontableDto _asientosDto = new asientocontableDto();
        string _Mode;
        string strFilterExpression;
        public string _Origen ,v_NroCuenta;
        List<destinoDto> ListaDestinoAgregar = new List<destinoDto>();
        List<destinoDto> ListaDestinoEditar = new List<destinoDto>();
        List<destinoDto> ListaDestinoEliminar = new List<destinoDto>();
        public frmCuentasMayor(string Origen)
        {
            InitializeComponent();
            _Origen = Origen;
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
           
            if (Origen == "PlanContable")
            {
                btnNuevaCuenta.Visible = false;
                btnEditarCuenta.Visible = false;
                btnEliminarCuenta.Visible = false;
                btnGuardarCuenta.Visible = false;
                btnCancelarCuenta.Visible = false;
            
            }
        }

        private void BindGrid()
        {
            var objData = GetData(0, null, "v_NroCuenta ASC", strFilterExpression);

            grdData.DataSource = objData;
        }

        private List<asientocontableDto> GetData(int pintPageIndex, int? pintPageSize, string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objBL.ObtenCuentasMayoresPaginadoFiltrado(ref objOperationResult, pintPageIndex, pintPageSize, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }
        
        private void frmCuentasMayor_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            _Mode = "New";
            BindGrid();
        }
        
        private void txtNroCuenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            Entrada_Numerica(e);
        }

        private void txtNroCuenta_Validated(object sender, EventArgs e)
        {
            int numero;
            if (!string.IsNullOrEmpty(txtNroCuenta.Text))
            {
                if (IsNumeric(txtNroCuenta.Text) == true)
                {
                    numero = Convert.ToInt32(txtNroCuenta.Text);
                    txtNroCuenta.Text = string.Format("{0:00}", numero);
                }
                else
                {
                    txtNroCuenta.Text = "";
                }
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            _Mode = "Edit";
            EdicionActivo();
            ActivarBotones(false, false, true, true, false);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CLR();
            ActivarBotones(false, false, false, false, true);
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            Nuevo();
            _Mode = "New";
            ActivarBotones(false, false, true, true, false);
            txtNroCuenta.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            strFilterExpression = null;
            if (uvAsientos.Validate(true, false).IsValid)
            {
                if (txtNroCuenta.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Código apropiado para el Documento.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNroCuenta.Focus();
                    return;
                }
                if (_Mode == "New")
                {
                    if (RevisaNroCuenta() == 0) return;

                    _asientosDto = new asientocontableDto();

                    _asientosDto.v_NroCuenta = txtNroCuenta.Text.Trim();
                    _asientosDto.v_NombreCuenta = txtNombreCuenta.Text.Trim();
                    if (chkIdentBancos.Checked == true){ _asientosDto.i_IdentificaCtaBancos = 1;} else{_asientosDto.i_IdentificaCtaBancos = 0;}
                    if (rb_Acreedora.Checked == true)
                    {
                        _asientosDto.i_Naturaleza = 1;
                    }
                    else
                    {
                        _asientosDto.i_Naturaleza = 2;
                    }
                    _asientosDto.i_LongitudJerarquica = 2;
                    
                    _objBL.AsientoNuevo(ref objOperationResult, _asientosDto, Globals.ClientSession.GetAsList(),ListaDestinoAgregar);
                }
                else if (_Mode == "Edit")
                {
                    _asientosDto.v_NroCuenta = txtNroCuenta.Text.Trim();
                    _asientosDto.v_NombreCuenta = txtNombreCuenta.Text.Trim();
                    if (chkIdentBancos.Checked == true) { _asientosDto.i_IdentificaCtaBancos = 1; } else { _asientosDto.i_IdentificaCtaBancos = 0; }
                    if (rb_Acreedora.Checked == true)
                    {
                        _asientosDto.i_Naturaleza = 1;
                    }
                    else
                    {
                        _asientosDto.i_Naturaleza = 2;
                    }
                    _asientosDto.i_LongitudJerarquica = 2;
                    // Save the data
                    _objBL.AsientoActualizar(ref objOperationResult, _asientosDto, Globals.ClientSession.GetAsList(),ListaDestinoAgregar,ListaDestinoEditar ,ListaDestinoEliminar  );
                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    EdicionDesactivo();
                    ActivarBotones(true, true, false, false, true);
                    grdData.Enabled = true;
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(objOperationResult.ExceptionMessage, "ERROR!!!-", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }
                BindGrid();
                MantenerSeleccion(_asientosDto.v_NroCuenta);
            }

        }

        private void grdData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            Infragistics.Win.UltraWinGrid.UltraGridBand Band = this.grdData.DisplayLayout.Bands[0];
            Band.SortedColumns.Add("v_NroCuenta", false, true);
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow.IsActiveRow == true)
            {
                EdicionDesactivo();
                string intNroDoc = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value.ToString();
                OperationResult objOperationResult = new OperationResult();
                _asientosDto = _objBL.ObtenAsientosPorNro(ref objOperationResult, intNroDoc);
                txtNroCuenta.Text = _asientosDto.v_NroCuenta;
                txtNombreCuenta.Text = _asientosDto.v_NombreCuenta;
                if (_asientosDto.i_Naturaleza == 1) rb_Acreedora.Checked = true; else { rb_Deudora.Checked = true; }
                if (_asientosDto.i_IdentificaCtaBancos == 1) chkIdentBancos.Checked = true; else { chkIdentBancos.Checked = false; }
            }
            ActivarBotones(true, true, false, false, true);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            string strNroDoc = grdData.Rows[grdData.ActiveRow.Index].Cells[0].Value.ToString();
            if (strNroDoc != null && strNroDoc != "")
            {
                OperationResult objOperationResult = new OperationResult();
                DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Result == System.Windows.Forms.DialogResult.Yes)
                {
                    _objBL.AsientoBorrar(ref objOperationResult, strNroDoc, Globals.ClientSession.GetAsList());
                    BindGrid();
                    txtNroCuenta.Text = "";
                    txtNombreCuenta.Text = "";
                    rb_Deudora.Checked = true;
                    chkIdentBancos.Checked = false;
                    txtNroCuenta.Focus();

                }
            }
        }

        #region Clases/Validaciones
        private void Entrada_Numerica(KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                e.Handled = false;
                return;
            }
            bool IsDec = false;
            if (e.KeyChar >= 48 && e.KeyChar <= 57)
                e.Handled = false;
            else if (e.KeyChar == 46)
                e.Handled = (IsDec) ? true : false;
            else
                e.Handled = true;
        }

        public bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        public void EdicionDesactivo()
        {
            txtNombreCuenta.Enabled = false;
            txtNroCuenta.Enabled = false;
            rb_Acreedora.Enabled = false;
            rb_Deudora.Enabled = false;
            rb_Acreedora.Checked = true;
            chkIdentBancos.Checked = false;
            chkIdentBancos.Enabled = false;
        }

        public void EdicionActivo()
        {
            txtNombreCuenta.Enabled = true;
            txtNroCuenta.Enabled = false;
            rb_Acreedora.Enabled = true;
            rb_Deudora.Enabled = true;
            chkIdentBancos.Enabled = true;
        }

        public void CLR()
        {
            rb_Acreedora.Checked = true;
            txtNombreCuenta.Text = "";
            txtNroCuenta.Text = "";
            grdData.Enabled = true;
            EdicionDesactivo();
            chkIdentBancos.Checked = false;
        }

        public void ActivarBotones(bool editar, bool eliminar, bool guardar, bool cancelar, bool nuevo)
        {
            btnEditarCuenta.Enabled = editar;
            btnEliminarCuenta.Enabled = eliminar;
            btnGuardarCuenta.Enabled = guardar;
            btnCancelarCuenta.Enabled = cancelar;
            btnNuevaCuenta.Enabled = nuevo;
        }

        public void Nuevo()
        {
            txtNombreCuenta.Enabled = true;
            txtNroCuenta.Enabled = true;
            rb_Acreedora.Enabled = true;
            rb_Deudora.Enabled = true;
            txtNombreCuenta.Text = "";
            txtNroCuenta.Text = "";
            grdData.Enabled = false;
            chkIdentBancos.Enabled = true;
        }

        private int RevisaNroCuenta()
        {
            OperationResult objOperationResult = new OperationResult();
            var _objCheck = _objBL.CheckByNroCuenta(ref objOperationResult, txtNroCuenta.Text.Trim());
            if (_objCheck.Count != 0)
            {
                UltraMessageBox.Show("El Número de cuenta ingresado ya existe en los registros", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                txtNroCuenta.Focus();
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["v_NroCuenta"].Text == ValorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        #endregion

     
        

        private void btnNuevaCuenta_Click(object sender, EventArgs e)
        {
            Nuevo();
            _Mode = "New";
            ActivarBotones(false, false, true, true, false);
            txtNroCuenta.Focus();
        }

     

        private void btnEditarCuenta_Click(object sender, EventArgs e)
        {
            _Mode = "Edit";
            EdicionActivo();
            ActivarBotones(false, false, true, true, false);
        }

        private void btnEliminarCuenta_Click(object sender, EventArgs e)
        {
            string strNroDoc = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value.ToString();
            if (strNroDoc != null && strNroDoc != "")
            {
                OperationResult objOperationResult = new OperationResult();
                DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Result == System.Windows.Forms.DialogResult.Yes)
                {
                    _objBL.AsientoBorrar(ref objOperationResult, strNroDoc, Globals.ClientSession.GetAsList());
                    BindGrid();
                    txtNroCuenta.Text = "";
                    txtNombreCuenta.Text = "";
                    rb_Deudora.Checked = true;
                    chkIdentBancos.Checked = false;
                    txtNroCuenta.Focus();

                }
            }
        }

        private void btnGuardarCuenta_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            strFilterExpression = null;
            if (uvAsientos.Validate(true, false).IsValid)
            {
                if (txtNroCuenta.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un numero de Cuenta Válido", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNroCuenta.Focus();
                    return;
                }
                if (_Mode == "New")
                {
                    if (RevisaNroCuenta() == 0) return;

                    _asientosDto = new asientocontableDto();

                    _asientosDto.v_NroCuenta = txtNroCuenta.Text.Trim();
                    _asientosDto.v_NombreCuenta = txtNombreCuenta.Text.Trim();
                    if (chkIdentBancos.Checked == true) { _asientosDto.i_IdentificaCtaBancos = 1; } else { _asientosDto.i_IdentificaCtaBancos = 0; }
                    if (rb_Acreedora.Checked == true)
                    {
                        _asientosDto.i_Naturaleza = 1;
                    }
                    else
                    {
                        _asientosDto.i_Naturaleza = 2;
                    }
                    _asientosDto.i_LongitudJerarquica = 2;
                    _objBL.AsientoNuevo(ref objOperationResult, _asientosDto, Globals.ClientSession.GetAsList(),ListaDestinoAgregar);
                }
                else if (_Mode == "Edit")
                {
                    _asientosDto.v_NroCuenta = txtNroCuenta.Text.Trim();
                    _asientosDto.v_NombreCuenta = txtNombreCuenta.Text.Trim();
                    if (chkIdentBancos.Checked == true) { _asientosDto.i_IdentificaCtaBancos = 1; } else { _asientosDto.i_IdentificaCtaBancos = 0; }
                    if (rb_Acreedora.Checked == true)
                    {
                        _asientosDto.i_Naturaleza = 1;
                    }
                    else
                    {
                        _asientosDto.i_Naturaleza = 2;
                    }
                    _asientosDto.i_LongitudJerarquica = 2;
                    // Save the data
                    _objBL.AsientoActualizar(ref objOperationResult, _asientosDto, Globals.ClientSession.GetAsList(),ListaDestinoAgregar,ListaDestinoEditar,ListaDestinoEliminar);
                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    EdicionDesactivo();
                    ActivarBotones(true, true, false, false, true);
                    grdData.Enabled = true;
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(objOperationResult.ExceptionMessage, "ERROR!!!-", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // Se queda en el formulario.
                }
                BindGrid();
                MantenerSeleccion(_asientosDto.v_NroCuenta);
            }

        }

        private void btnCancelarPlan_Click(object sender, EventArgs e)
        {
            CLR();
            ActivarBotones(false, false, false, false, true);
        }

        private void btnCancelarCuenta_Click(object sender, EventArgs e)
        {
            CLR();
            ActivarBotones(false, false, false, false, true);
        }

        private void txtNroCuenta_TextChanged(object sender, EventArgs e)
        {

        }

        private void grdData_Click(object sender, EventArgs e)
        {
           
        }

        private void grdData_DoubleClick(object sender, EventArgs e)
        {
            if (_Origen == "PlanContable")
            {
                if (grdData.Rows.Count == 0) return; //se cambio
                if (grdData.ActiveRow != null)
                {
                    if (grdData.ActiveRow.Cells["v_NroCuenta"].Value != null)
                    {
                        v_NroCuenta = grdData.ActiveRow.Cells["v_NroCuenta"].Value.ToString();
                        this.Close();
                    }
                }
            }
        }
       

       
    }
}
