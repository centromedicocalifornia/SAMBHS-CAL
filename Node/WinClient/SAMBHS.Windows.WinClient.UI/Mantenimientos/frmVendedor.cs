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
using SAMBHS.Venta.BL;
using SAMBHS.Almacen.BL;
namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    /// <summary>
    /// Autor: Eduardo Quiroz Cosme
    /// Fecha: 28/10/2014
    /// </summary>
    public partial class frmVendedor : Form
    {
        SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL _objEstablecimientoBL = new EstablecimientoBL();
        VendedorBL _objVendedorBL = new VendedorBL();
        CarteraClienteBL _objCarteraClienteBL = new CarteraClienteBL();
        vendedorDto _vendedorDto = new vendedorDto();
        carteraclienteDto _carteraclienteDto;
        string _strFilterExpression, _mode, _IdVendedor;
        string _IdClienteOrigen, _NombreClienteOrigen;
        public bool _OrigenProcesado = false;
        public string _NombreVendedor;
        public frmVendedor(string Parametro)
        {
            InitializeComponent();
        }

        public frmVendedor(string IdCliente, string NombreCliente)
        {
            _IdClienteOrigen = IdCliente;
            _NombreClienteOrigen = NombreCliente;
            InitializeComponent();
        }

        private void frmVendedor_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            tpAdicionales.BackColor = new GlobalFormColors().FormColor;
            tpDatos.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            _mode = "New";
            txtCodigoAnexo.Focus();
            //Combo País
            var ListaPaises = (_objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 112, null)).FindAll(p => p.Value3 == "-1");
            Utils.Windows.LoadUltraComboEditorList(ddlPais, "Value1", "Id", ListaPaises, DropDownListAction.Select);
            //Combo Departamento
            Utils.Windows.LoadUltraComboEditorList(ddlDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            //Combo Provincia
            Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            //Combo Distrito
            Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);

            Utils.Windows.LoadUltraComboEditorList(cboTipoPersona, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 2, null), DropDownListAction.Select);

            Utils.Windows.LoadUltraComboEditorList(cboUsuario, "Value1", "Id", _objVendedorBL.ObtenerUsuariosParaCombo(ref objOperationResult, null), DropDownListAction.Select);

            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", _objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);

            ddlPais.Value = "1";
            ddlDepartamento.Value = "1391";
            ddlProvincia.Value = "1392";
            ddlDistrito.Value = "1393";

            if (_IdClienteOrigen != null)
            {
                tcDetalle.Controls["tpAdicionales"].Enabled = true;
                tcDetalle.Controls["tpDatos"].Enabled = false;
                tcDetalle.SelectedIndex = 1;
                txtBuscarCliente.Enabled = false;
                txtBuscarCliente.Text = _NombreClienteOrigen.Trim();
                btnAgregarAval.Text = "Agregar";
            }
            else
            {
                tcDetalle.Controls["tpAdicionales"].Enabled = false;
                tcDetalle.Controls["tpDatos"].Enabled = false;
            }
            cboEstablecimiento.Value = "-1";
        }

        private void frmVendedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        #region Clases/Validaciones
        private void MantenerSeleccion(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            {
                if (row.Cells["v_NroDocIdentificacion"].Text == ValorSeleccionado)
                {
                    grdData.ActiveRow = row;
                    grdData.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        private void MantenerSeleccionCliente(string ValorSeleccionado)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdClientes.Rows)
            {
                if (row.Cells["v_IdCliente"].Text == ValorSeleccionado)
                {
                    grdClientes.ActiveRow = row;
                    grdClientes.ActiveRow.Selected = true;
                    break;
                }
            }
        }

        public void LimpiaDetalle()
        {
            txtNombres.Clear();
            txtNroDocumento.Clear();
            txtTelefono.Clear();
            txtBuscarCodigo.Clear();
            txtBuscarDoc.Clear();
            txtBuscarNombre.Clear();
            txtCodigoAnexo.Clear();
            txtContacto.Clear();
            txtDireccionSec.Clear();
            txtEmail.Clear();
            txtFax.Clear();
            cboTipoPersona.Value = "-1";
            cboTipoDocumento.Value = "-1";
            cboEstablecimiento.Value = "-1";
            uckActivo.Checked = false;
        }
        #endregion

        #region Cabecera
        private void BindGrid()
        {
            var objData = GetData("v_NombreCompleto ASC", _strFilterExpression);
            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<vendedorDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objVendedorBL.ObtenerListadoVendedor(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // Get the filters from the UI
            List<string> Filters = new List<string>();
            if (!string.IsNullOrEmpty(txtBuscarCodigo.Text)) Filters.Add("v_CodVendedor.Contains(\"" + txtBuscarCodigo.Text.Trim().ToUpper() + "\")");
            if (!string.IsNullOrEmpty(txtBuscarDoc.Text)) Filters.Add("v_NroDocIdentificacion.Contains(\"" + txtBuscarDoc.Text.Trim().ToUpper() + "\")");
            if (!string.IsNullOrEmpty(txtBuscarNombre.Text)) Filters.Add("v_NombreCompleto.Contains(\"" + txtBuscarNombre.Text.Trim().ToUpper() + "\")");
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
            tcDetalle.Controls["tpDatos"].Enabled = grdData.Rows.Count() == 0 ? false : true;
            btnEliminar.Enabled = grdData.Rows.Count() == 0 ? false : true;
            btnAgregarAval.Enabled = grdData.Rows.Count() == 0 ? false : true;
            if (grdData.Rows.Count() == 0 && _IdClienteOrigen == null)
            {
                LimpiaDetalle();
                tcDetalle.Controls["tpAdicionales"].Enabled = false;
            }
            btnAgregarAval.Enabled = grdData.Rows.Count() == 0 ? false : true;
        }

        private void ddlPais_SelectedIndexChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlPais.Value == null) return;

            //si el combo esta en seleccione tengo que reiniciar el combo departamento
            if (ddlPais.Value == "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(ddlDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(ddlDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlPais.Value.ToString()), 112, ""), DropDownListAction.Select);
            }
        }

        private void ddlDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlDepartamento.Value == null) return;

            if (ddlDepartamento.Value == "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlDepartamento.Value.ToString()), 112, ""), DropDownListAction.Select);
            }
        }

        private void ddlProvincia_SelectedIndexChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlProvincia.Value == null) return;

            if (ddlProvincia.Value == "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlProvincia.Value.ToString()), 112, ""), DropDownListAction.Select);
            }
        }

        private void cboTipoPersona_SelectedIndexChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (cboTipoPersona.Value.ToString() != "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(cboTipoDocumento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 150, null), DropDownListAction.Select);

                if (cboTipoPersona.Value.ToString() == "2")
                {
                    cboTipoDocumento.Value = "6";
                    cboTipoDocumento.Enabled = false;
                }
                else
                {
                    cboTipoDocumento.Value = "-1";
                    cboTipoDocumento.Enabled = true;
                }
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(cboTipoDocumento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, -1, null), DropDownListAction.Select);
            }
        }

        private void cboTipoDocumento_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTipoDocumento.Value != "-1")
            {
                var x = (KeyValueDTO)cboTipoDocumento.SelectedItem.ListObject;
                if (x == null) return;
                txtNroDocumento.MaxLength = int.Parse(x.Value2);
                txtNroDocumento.Focus();
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            tcDetalle.Controls["tpDatos"].Enabled = true;
            LimpiaDetalle();
            _mode = "New";
            tcDetalle.Controls["tpAdicionales"].Enabled = false;
            ddlPais.Value = "1";
            ddlDepartamento.Value = "1391";
            ddlProvincia.Value = "1392";
            ddlDistrito.Value = "1393";
            txtCodigoAnexo.Focus();
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string IdVendedor = "";
            //_mode = "Edit";
            if (grdData.ActiveRow.Index != null)
            {
                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                {
                    _mode = "Edit";
                    IdVendedor = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdVendedor"].Value.ToString();
                    _IdVendedor = IdVendedor;
                    _vendedorDto = new vendedorDto();
                    _vendedorDto = _objVendedorBL.ObtenerVendedor(ref objOperationResult, IdVendedor);
                    txtCodigoAnexo.Text = _vendedorDto.v_CodVendedor;
                    txtNombres.Text = _vendedorDto.v_NombreCompleto;
                    txtNroDocumento.Text = _vendedorDto.v_NroDocIdentificacion;
                    txtTelefono.Text = _vendedorDto.v_Telefono;
                    txtCodigoAnexo.Text = _vendedorDto.v_CodVendedor;
                    txtContacto.Text = _vendedorDto.v_Contacto;
                    txtDireccionSec.Text = _vendedorDto.v_Direccion;
                    txtEmail.Text = _vendedorDto.v_Correo;
                    txtFax.Text = _vendedorDto.v_Fax;
                    cboTipoPersona.Value = _vendedorDto.i_IdTipoPersona.ToString();
                    ddlPais.Value = _vendedorDto.i_IdPais.ToString();
                    ddlDepartamento.Value = _vendedorDto.i_IdDepartamento.ToString();
                    ddlProvincia.Value = _vendedorDto.i_IdProvincia.ToString();
                    cboTipoDocumento.Value = _vendedorDto.i_IdTipoIdentificacion.ToString();
                    ddlDistrito.Value = _vendedorDto.i_IdDistrito.ToString();
                    tcDetalle.Controls["tpAdicionales"].Enabled = true;
                    grdData.Focus();
                    txtBuscarCliente.Text = _IdClienteOrigen == null ? "" : txtBuscarCliente.Text;
                    grdClientes.DataSource = _objCarteraClienteBL.ObtenerListadoCarteraClientePorVendedor(ref objOperationResult, null, null, IdVendedor);
                    lblClienteContadorFilas.Text = string.Format("Se encontraron {0} registros.", grdClientes.Rows.Count());
                    btnClienteEliminar.Enabled = grdClientes.Rows.Count() == 0 ? false : true;
                    cboUsuario.Value = _vendedorDto.i_SystemUser != null ? _vendedorDto.i_SystemUser.ToString() : "-1";
                    chkPermisoAnular.Checked = _vendedorDto.i_PermiteAnularVentas == 1;
                    chkPermisoEliminar.Checked = _vendedorDto.i_PermiteEliminarVentas == 1;
                    cboEstablecimiento.Value = _vendedorDto.i_IdAlmacen==null ?"-1": _vendedorDto.i_IdAlmacen.Value.ToString();
                    uckActivo.Checked = _vendedorDto.i_EsActivo == 1 ? true : false;
                }
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
        #endregion

        #region Detalle

        #region Datos
        private void btnGrabar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvDatos.Validate(true, false).IsValid)
            {
                if (txtCodigoAnexo.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Código.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCodigoAnexo.Focus();
                    return;
                }

                if (txtNroDocumento.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Nro. Documento.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNroDocumento.Focus();
                    return;
                }

                if (txtNombres.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Nombre.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombres.Focus();
                    return;
                }

                if (txtDireccionSec.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese una Dirección.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDireccionSec.Focus();
                    return;
                }
                if (_mode == "New")
                {
                    _vendedorDto = new vendedorDto();
                    _vendedorDto.v_CodVendedor = txtCodigoAnexo.Text.Trim();
                    _vendedorDto.v_Contacto = txtContacto.Text.Trim();
                    _vendedorDto.v_Correo = txtEmail.Text.Trim();
                    _vendedorDto.v_Direccion = txtDireccionSec.Text.Trim();
                    _vendedorDto.v_Fax = txtFax.Text.Trim();
                    _vendedorDto.v_Telefono = txtTelefono.Text.Trim();
                    _vendedorDto.v_NroDocIdentificacion = txtNroDocumento.Text.Trim();
                    _vendedorDto.v_NombreCompleto = txtNombres.Text.Trim();
                    _vendedorDto.i_IdPais = int.Parse(ddlPais.Value.ToString());
                    _vendedorDto.i_IdDepartamento = int.Parse(ddlDepartamento.Value.ToString());
                    _vendedorDto.i_IdProvincia = int.Parse(ddlProvincia.Value.ToString());
                    _vendedorDto.i_IdDistrito = int.Parse(ddlDistrito.Value.ToString());
                    _vendedorDto.i_IdTipoIdentificacion = int.Parse(cboTipoDocumento.Value.ToString());
                    _vendedorDto.i_IdTipoPersona = int.Parse(cboTipoPersona.Value.ToString());
                    _vendedorDto.i_SystemUser = int.Parse(cboUsuario.Value.ToString());
                    _vendedorDto.i_PermiteAnularVentas = chkPermisoAnular.Checked ? 1 : 0;
                    _vendedorDto.i_PermiteEliminarVentas = chkPermisoEliminar.Checked ? 1 : 0;
                    _vendedorDto.i_IdAlmacen = int.Parse ( cboEstablecimiento.Value.ToString());
                    _vendedorDto.i_EsActivo  = uckActivo.Checked ? 1 : 0;
                    // Save the data
                    _objVendedorBL.InsertarVendedor(ref objOperationResult, _vendedorDto, Globals.ClientSession.GetAsList());
                    tcDetalle.Controls["tpAdicionales"].Enabled = true;
                }
                else if (_mode == "Edit")
                {
                    _vendedorDto.v_CodVendedor = txtCodigoAnexo.Text.Trim();
                    _vendedorDto.v_Contacto = txtContacto.Text.Trim();
                    _vendedorDto.v_Correo = txtEmail.Text.Trim();
                    _vendedorDto.v_Direccion = txtDireccionSec.Text.Trim();
                    _vendedorDto.v_Fax = txtFax.Text.Trim();
                    _vendedorDto.v_Telefono = txtTelefono.Text.Trim();
                    _vendedorDto.v_NroDocIdentificacion = txtNroDocumento.Text.Trim();
                    _vendedorDto.v_NombreCompleto = txtNombres.Text.Trim();
                    _vendedorDto.i_IdPais = int.Parse(ddlPais.Value.ToString());
                    _vendedorDto.i_IdDepartamento = int.Parse(ddlDepartamento.Value.ToString());
                    _vendedorDto.i_IdProvincia = int.Parse(ddlProvincia.Value.ToString());
                    _vendedorDto.i_IdDistrito = int.Parse(ddlDistrito.Value.ToString());
                    _vendedorDto.i_IdTipoIdentificacion = int.Parse(cboTipoDocumento.Value.ToString());
                    _vendedorDto.i_IdTipoPersona = int.Parse(cboTipoPersona.Value.ToString());
                    _vendedorDto.i_SystemUser = int.Parse(cboUsuario.Value.ToString());
                    _vendedorDto.i_PermiteAnularVentas = chkPermisoAnular.Checked ? 1 : 0;
                    _vendedorDto.i_PermiteEliminarVentas = chkPermisoEliminar.Checked ? 1 : 0;
                    _vendedorDto.i_IdAlmacen = int.Parse(cboEstablecimiento.Value.ToString());
                    _vendedorDto.i_EsActivo = uckActivo.Checked ? 1 : 0;
                    // Save the data
                    _objVendedorBL.ActualizarVendedor(ref objOperationResult, _vendedorDto, Globals.ClientSession.GetAsList());
                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {
                    btnBuscar_Click(sender, e);
                    MantenerSeleccion(_vendedorDto.v_NroDocIdentificacion);
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region CarteraCliente
        private void txtBuscarCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                btnAgregarAval_Click(sender, e);
            }
        }

        private void btnChoferEliminar_Click(object sender, EventArgs e)
        {
            if (grdClientes.Rows.Count() == 0) return;
            OperationResult objOperationResult = new OperationResult();
            DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                _objCarteraClienteBL.EliminarCarteraCliente(ref objOperationResult, grdClientes.Rows[grdClientes.ActiveRow.Index].Cells["v_IdCarteraCliente"].Value.ToString(), Globals.ClientSession.GetAsList());
                grdClientes.DataSource = _objCarteraClienteBL.ObtenerListadoCarteraClientePorVendedor(ref objOperationResult, null, null, _IdVendedor);
                lblClienteContadorFilas.Text = string.Format("Se encontraron {0} registros.", grdClientes.Rows.Count());
                LimpiaDetalle();
                btnClienteEliminar.Enabled = grdClientes.Rows.Count == 0 ? false : true;
            }
        }

        private void btnAgregarAval_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            if (_IdClienteOrigen == null)
            {
                if (txtBuscarCliente.Text.Trim().Length < 3)
                {
                    UltraMessageBox.Show("Por favor ingrese 3 caracteres para iniciar una búsqueda.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                frmBuscarCliente frm = new frmBuscarCliente("V", txtBuscarCliente.Text.Trim());
                frm.ShowDialog();
                if (frm._IdCliente != null)
                {
                    int Result;
                    OperationResult objOperationResult = new OperationResult();
                    Result = _objCarteraClienteBL.ConsultarSiExisteCliente(ref objOperationResult, frm._IdCliente);
                    if (Result == 0) //SI es 0 no existe en los registros y se procede al Insert
                    {
                        objOperationResult = new OperationResult();
                        _carteraclienteDto = new carteraclienteDto();
                        _carteraclienteDto.v_IdCliente = frm._IdCliente;
                        _carteraclienteDto.v_IdVendedor = _IdVendedor;
                        _objCarteraClienteBL.InsertarCarteraCliente(ref objOperationResult, _carteraclienteDto, Globals.ClientSession.GetAsList());
                        if (objOperationResult.Success == 1)  // Operación sin error
                        {
                            grdClientes.DataSource = _objCarteraClienteBL.ObtenerListadoCarteraClientePorVendedor(ref objOperationResult, null, null, _IdVendedor);
                            lblClienteContadorFilas.Text = string.Format("Se encontraron {0} registros.", grdClientes.Rows.Count());
                            MantenerSeleccionCliente(_carteraclienteDto.v_IdCliente);
                            txtBuscarCliente.Clear();
                            btnClienteEliminar.Enabled = grdClientes.Rows.Count() == 0 ? false : true;
                        }
                        else  // Operación con error
                        {
                            UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        UltraMessageBox.Show("El Cliente ya existe en una Cartera de Cliente", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    txtBuscarCliente.Focus();
                }
            }
            else
            {
                int Result;
                OperationResult objOperationResult = new OperationResult();
                Result = _objCarteraClienteBL.ConsultarSiExisteCliente(ref objOperationResult, _IdClienteOrigen);
                if (Result == 0) //SI es 0 no existe en los registros y se procede al Insert
                {
                    objOperationResult = new OperationResult();
                    _carteraclienteDto = new carteraclienteDto();
                    _carteraclienteDto.v_IdCliente = _IdClienteOrigen;
                    _carteraclienteDto.v_IdVendedor = _IdVendedor;
                    _objCarteraClienteBL.InsertarCarteraCliente(ref objOperationResult, _carteraclienteDto, Globals.ClientSession.GetAsList());
                    if (objOperationResult.Success == 1)  // Operación sin error
                    {
                        grdClientes.DataSource = _objCarteraClienteBL.ObtenerListadoCarteraClientePorVendedor(ref objOperationResult, null, null, _IdVendedor);
                        lblClienteContadorFilas.Text = string.Format("Se encontraron {0} registros.", grdClientes.Rows.Count());
                        MantenerSeleccionCliente(_carteraclienteDto.v_IdCliente);
                        txtBuscarCliente.Clear();
                        btnClienteEliminar.Enabled = grdClientes.Rows.Count() == 0 ? false : true;
                        _OrigenProcesado = true;
                        _NombreVendedor = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreCompleto"].Value.ToString();
                    }
                    else  // Operación con error
                    {
                        UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    UltraMessageBox.Show("El Cliente ya existe en una Cartera de Cliente", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

        }
        #endregion

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Count() == 0) return;
            OperationResult objOperationResult = new OperationResult();
            DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (Result == System.Windows.Forms.DialogResult.Yes)
            {
                _objVendedorBL.EliminarVendedor(ref objOperationResult, grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdVendedor"].Value.ToString(), Globals.ClientSession.GetAsList());
                btnBuscar_Click(sender, e);
                lblClienteContadorFilas.Text = string.Format("Se encontraron {0} registros.", grdData.Rows.Count());
                LimpiaDetalle();
                btnEliminar.Enabled = grdData.Rows.Count == 0 ? false : true;
                if (grdData.Rows.Count() == 0) tcDetalle.Controls["tpAdicionales"].Enabled = false;
            }
        }

        #endregion

        private void frmVendedor_FormClosing(object sender, FormClosingEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Globals.CacheCombosVentaDto.cboVendedor = new VendedorBL().ObtenerListadoVendedorParaCombo(ref objOperationResult, null);
            Globals.CacheCombosVentaDto.cboVendedorRef = new VendedorBL().ObtenerListadoVendedorParaCombo(ref objOperationResult, null);
        }


    }
}
