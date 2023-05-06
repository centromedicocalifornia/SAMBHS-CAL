using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    /// <summary>
    /// Autor: Eduardo Quiroz Cosme
    /// Fecha: 24/10/2014
    /// </summary>
    public partial class frmTransportista : Form
    {
        #region BussinesLayers
        SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        TransportistaBL _objTransportistaBL = new TransportistaBL();
        TransportistaChoferBL _objTransportistaChoferBL = new TransportistaChoferBL();
        TransportistaUnidadTransporteBL _objTransportistaUnidadTransporteBL = new TransportistaUnidadTransporteBL();
        #endregion

        #region Entities
        transportistaDto _transportistaDto = new transportistaDto();
        transportistachoferDto _transportistachoferDto;
        transportistaunidadtransporteDto _transportistaunidadtransporteDto;
        #endregion
       
        string _strFilterExpression;
        string _mode = "", _modeChofer = "", _modeTransporte = "";
        string _modeDetalle, _IdTransportista;

        public frmTransportista(string Parametro)
        {
            InitializeComponent();
        }

        private void frmTransportista_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            tpChofer.BackColor = new GlobalFormColors().FormColor;
            tpDatos.BackColor = new GlobalFormColors().FormColor;
            tpUTransporte.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();

            #region Cargar combos
            //Combo País
            var ListaPaises = (_objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 112, null)).FindAll(p => p.Value3 == "-1");
            Utils.Windows.LoadDropDownList(ddlPais, "Value1", "Id", ListaPaises, DropDownListAction.Select);
            //Combo Departamento
            Utils.Windows.LoadDropDownList(ddlDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            //Combo Provincia
            Utils.Windows.LoadDropDownList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            //Combo Distrito
            Utils.Windows.LoadDropDownList(ddlDistrito, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);

            Utils.Windows.LoadDropDownList(ddlbTipoPersona, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 2, null), DropDownListAction.Select);

            Utils.Windows.LoadDropDownList(cboTipoDocChofer, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 150, null), DropDownListAction.Select);
            #endregion
            tcDetalle.Controls["tpChofer"].Enabled = false;
            tcDetalle.Controls["tpUTransporte"].Enabled = false;
            _mode = "New";
            ddlPais.SelectedValue = "1";
            ddlDepartamento.SelectedValue = "1391";
            ddlProvincia.SelectedValue = "1392";
            ddlDistrito.SelectedValue = "1393";
            txtCodigo.Focus();
            tcDetalle.Controls["tpDatos"].Enabled = false;
        }

        private void frmTransportista_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        #region Clases/Validaciones

            private void LimpiaDetalle()
            {
                txtCodigo.Clear();
                txtContacto.Clear();
                txtDireccion.Clear();
                txtEMail.Clear();
                txtFax.Clear();
                txtNombre.Clear();
                txtNroDocumento.Clear();
                txtTelefono.Clear();
                ddlPais.SelectedValue = "-1";
                ddlProvincia.SelectedValue = "-1";
                ddlDepartamento.SelectedValue = "-1";
                ddlTipoIdentificacion.SelectedValue = "-1";
                ddlDistrito.SelectedValue = "-1";
                ddlbTipoPersona.SelectedValue = "-1";
            }

            private void MantenerSeleccion(string ValorSeleccionado)
            {
                foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
                {
                    if (row.Cells["v_NumeroDocumento"].Text == ValorSeleccionado)
                    {
                        grdData.ActiveRow = row;
                        grdData.ActiveRow.Selected = true;
                        break;
                    }
                }
            }

            private void MantenerSeleccionChofer(string ValorSeleccionado)
            {
                foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdChoferData.Rows)
                {
                    if (row.Cells["v_Brevete"].Text == ValorSeleccionado)
                    {
                        grdChoferData.ActiveRow = row;
                        grdChoferData.ActiveRow.Selected = true;
                        break;
                    }
                }
            }

            private void MantenerSeleccionUnidadTransporte(string ValorSeleccionado)
            {
                foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdUndTransData.Rows)
                {
                    if (row.Cells["v_TractoPlaca"].Text == ValorSeleccionado)
                    {
                        grdUndTransData.ActiveRow = row;
                        grdUndTransData.ActiveRow.Selected = true;
                        break;
                    }
                }
            }
            
            private void ActivaDesactivaBotonesChofer()
            {
                if (grdChoferData.Rows.Count() == 0)
                {
                    btnChoferEditar.Enabled = false;
                    btnChoferEliminar.Enabled = false;
                }
                else
                {
                    btnChoferEditar.Enabled = true;
                    btnChoferEliminar.Enabled = true;
                }
            }

            private void ActivaDesactivaBotonesUT()
            {
                if (grdUndTransData.Rows.Count() == 0)
                {
                    btnUTEditar.Enabled = false;
                    btnUTEliminar.Enabled = false;  
                }
                else
                {
                    btnUTEditar.Enabled = true;
                    btnUTEliminar.Enabled = true;
                }
                

            }

            private void txtBuscarCodigo_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnBuscar_Click(sender, e);
                }
            }

            private void txtBuscarNombre_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnBuscar_Click(sender, e);
                }
            }

            private void txtBuscarDoc_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnBuscar_Click(sender, e);
                }
            }
        
        #endregion

        #region Cabecera
            private void btnBuscar_Click(object sender, EventArgs e)
            {
                // Get the filters from the UI
                List<string> Filters = new List<string>();
                if (!string.IsNullOrEmpty(txtBuscarCodigo.Text)) Filters.Add("v_Codigo.Contains(\"" + txtBuscarCodigo.Text + "\")");
                if (!string.IsNullOrEmpty(txtBuscarDoc.Text)) Filters.Add("v_NumeroDocumento.Contains(\"" + txtBuscarDoc.Text.Trim().ToUpper() + "\")");
                if (!string.IsNullOrEmpty(txtBuscarNombre.Text)) Filters.Add("v_NombreRazonSocial.Contains(\"" + txtBuscarNombre.Text.Trim().ToUpper() + "\")");
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
                if (grdData.Rows.Count() == 0)
                {
                    LimpiaDetalle();
                    tcDetalle.Controls["tpChofer"].Enabled = false;
                    tcDetalle.Controls["tpUTransporte"].Enabled = false;
                } 
            }

            private void BindGrid()
            {
                var objData = GetData("v_NombreRazonSocial ASC", _strFilterExpression, 0);

                grdData.DataSource = objData;
                lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
            }

            private List<transportistaDto> GetData(string pstrSortExpression, string pstrFilterExpression, int pintGrupoId)
            {
                OperationResult objOperationResult = new OperationResult();
                var _objData = _objTransportistaBL.ObtenerListadoTransportista(ref objOperationResult, pstrSortExpression, pstrFilterExpression);

                if (objOperationResult.Success != 1)
                {
                    UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return _objData;
            }

            private void btnAgregar_Click(object sender, EventArgs e)
            {
                tcDetalle.Controls["tpDatos"].Enabled = true;
                _mode = "New";
                _modeChofer = "NewChofer";
                txtCodigo.Focus();
                LimpiaDetalle();
                tcDetalle.Controls["tpChofer"].Enabled = false;
                tcDetalle.Controls["tpUTransporte"].Enabled = false;
                ddlPais.SelectedValue = "1";
                ddlDepartamento.SelectedValue = "1391";
                ddlProvincia.SelectedValue = "1392";
                ddlDistrito.SelectedValue = "1393";
                _transportistaDto = new transportistaDto();
            }

            private void grdData_AfterRowActivate(object sender, EventArgs e)
            {
                OperationResult objOperationResult = new OperationResult();
                string IdTransportista = "";
                //_mode = "Edit";
                if (grdData.ActiveRow.Index != null)
                {
                    _mode = "Edit";
                    IdTransportista = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdTransportista"].Value.ToString();
                    _transportistaDto = new transportistaDto();
                    _transportistaDto = _objTransportistaBL.ObtenerTransportista(ref objOperationResult, IdTransportista);
                    txtCodigo.Text = _transportistaDto.v_Codigo;
                    txtContacto.Text = _transportistaDto.v_NombreContacto;
                    txtDireccion.Text = _transportistaDto.v_Direccion;
                    txtEMail.Text = _transportistaDto.v_CorreoElectronico;
                    txtFax.Text = _transportistaDto.v_Fax;
                    txtNombre.Text = _transportistaDto.v_NombreRazonSocial;
                    txtNroDocumento.Text = _transportistaDto.v_NumeroDocumento;
                    txtTelefono.Text = _transportistaDto.v_Telefono;
                    ddlbTipoPersona.SelectedValue = _transportistaDto.i_IdTipoPersona.ToString();
                    ddlPais.SelectedValue = _transportistaDto.i_IdPais.ToString();
                    ddlDepartamento.SelectedValue = _transportistaDto.i_IdDepartamento.ToString();
                    ddlProvincia.SelectedValue = _transportistaDto.i_IdProvincia.ToString();
                    ddlDistrito.SelectedValue = _transportistaDto.i_IdDistrito.ToString();
                    ddlTipoIdentificacion.SelectedValue = _transportistaDto.i_IdTipoIdentificacion.ToString();
                    _IdTransportista = IdTransportista;
                    tcDetalle.Controls["tpChofer"].Enabled = true;
                    tcDetalle.Controls["tpUTransporte"].Enabled = true;
                    grdChoferData.DataSource = _objTransportistaChoferBL.ObtenerListadoTransportistaChofer(ref objOperationResult, null, null, IdTransportista);
                    lblContadorChofer.Text = string.Format("Se encontraron {0} registros.", grdChoferData.Rows.Count());
                    grdUndTransData.DataSource = _objTransportistaUnidadTransporteBL.ObtenerListadoTransportistaUnidadTransporte(ref objOperationResult, null, null, IdTransportista);
                    lblContadorUnidadTransporte.Text = string.Format("Se encontraron {0} registros.", grdUndTransData.Rows.Count());
                    _modeChofer = "NewChofer";
                    _modeTransporte = "NewUT";
                    btnAgregar.Enabled = true;
                    btnChoferAgregar.Enabled = true;
                    btnChoferEditar.Enabled = true;
                    btnChoferEliminar.Enabled = true;
                    btnGrabar.Enabled = true;
                    btnUTAgregar.Enabled = true;
                    btnUTEditar.Enabled = true;
                    btnUTEliminar.Enabled = true;
                    grdData.Focus();
                    ActivaDesactivaBotonesChofer();
                    ActivaDesactivaBotonesUT();
                }
            }

            private void btnEliminar_Click(object sender, EventArgs e)
            {
                OperationResult objOperationResult = new OperationResult();
                if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdTransportista"].Value == null) return;

                if (_objTransportistaBL.TransportistasenGuiasRemision(ref objOperationResult, grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdTransportista"].Value.ToString()))
                {
                    UltraMessageBox.Show("Transportista está  siendo utilizado en otros procesos ,no se puede Eliminar", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;

                }
                else
                {

                    DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (Result == System.Windows.Forms.DialogResult.Yes)
                    {
                        _objTransportistaBL.EliminarTransportista(ref objOperationResult, grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdTransportista"].Value.ToString(), Globals.ClientSession.GetAsList());
                        btnBuscar_Click(sender, e);
                        LimpiaDetalle();
                        btnEliminar.Enabled = grdData.Rows.Count() == 0 ? false : true;
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
            private void ddlPais_SelectedIndexChanged(object sender, EventArgs e)
            {
                OperationResult objOperationResult = new OperationResult();
                if (ddlPais.SelectedValue == null) return;

                //si el combo esta en seleccione tengo que reiniciar el combo departamento
                if (ddlPais.SelectedValue == "-1")
                {
                    Utils.Windows.LoadDropDownList(ddlDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
                }
                else
                {
                    Utils.Windows.LoadDropDownList(ddlDepartamento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlPais.SelectedValue.ToString()), 112, ""), DropDownListAction.Select);
                }
            }

            private void ddlDepartamento_SelectedIndexChanged(object sender, EventArgs e)
            {
                OperationResult objOperationResult = new OperationResult();
                if (ddlDepartamento.SelectedValue == null) return;

                if (ddlDepartamento.SelectedValue == "-1")
                {
                    Utils.Windows.LoadDropDownList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
                }
                else
                {
                    Utils.Windows.LoadDropDownList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlDepartamento.SelectedValue.ToString()), 112, ""), DropDownListAction.Select);
                }
            }

            private void ddlProvincia_SelectedIndexChanged(object sender, EventArgs e)
            {
                OperationResult objOperationResult = new OperationResult();
                if (ddlProvincia.SelectedValue == null) return;

                if (ddlProvincia.SelectedValue == "-1")
                {
                    Utils.Windows.LoadDropDownList(ddlDistrito, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
                }
                else
                {
                    Utils.Windows.LoadDropDownList(ddlDistrito, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlProvincia.SelectedValue.ToString()), 112, ""), DropDownListAction.Select);
                }
            }

            private void ddlbTipoPersona_SelectedIndexChanged(object sender, EventArgs e)
            {
                OperationResult objOperationResult = new OperationResult();
                if (ddlbTipoPersona.SelectedValue.ToString() != "-1")
                {
                    Utils.Windows.LoadDropDownList(ddlTipoIdentificacion, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 150, null), DropDownListAction.Select);

                    if (ddlbTipoPersona.SelectedValue.ToString() == "2")
                    {

                        ddlTipoIdentificacion.SelectedValue = "6";
                        ddlTipoIdentificacion.Enabled = false;
                    }
                    else
                    {
                        ddlTipoIdentificacion.SelectedValue = "-1";
                        ddlTipoIdentificacion.Enabled = true;
                    }
                }
                else
                {
                    Utils.Windows.LoadDropDownList(ddlTipoIdentificacion, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, -1, null), DropDownListAction.Select);    
                }


            }

            private void btnGrabar_Click(object sender, EventArgs e)
            {
                OperationResult objOperationResult = new OperationResult();
                if (uvDatos.Validate(true, false).IsValid)
                {
                    if (txtCodigo.Text.Trim() == "")
                    {
                        UltraMessageBox.Show("Por favor ingrese un Código.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCodigo.Focus();
                        return;
                    }

                    if (txtNombre.Text.Trim() == "")
                    {
                        UltraMessageBox.Show("Por favor ingrese un Nombre/Razón Social.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtNombre.Focus();
                        return;
                    }

                    if (txtNroDocumento.Text.Trim() == "")
                    {
                        UltraMessageBox.Show("Por favor ingrese un Número de Documento.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtNroDocumento.Focus();
                        return;
                    }

                    if (_mode == "New")
                    {
                        _transportistaDto = new transportistaDto();

                        _transportistaDto.v_Codigo = txtCodigo.Text.Trim();
                        _transportistaDto.v_NombreRazonSocial = txtNombre.Text.Trim();
                        _transportistaDto.v_NombreContacto = txtContacto.Text.Trim();
                        _transportistaDto.v_NumeroDocumento = txtNroDocumento.Text.Trim();
                        _transportistaDto.v_Direccion = txtDireccion.Text.Trim();
                        _transportistaDto.v_CorreoElectronico = txtEMail.Text.Trim();
                        _transportistaDto.v_Fax = txtFax.Text.Trim();
                        _transportistaDto.v_Telefono = txtTelefono.Text.Trim();
                        _transportistaDto.i_IdTipoIdentificacion = int.Parse(ddlTipoIdentificacion.SelectedValue.ToString());
                        _transportistaDto.i_IdTipoPersona = int.Parse(ddlbTipoPersona.SelectedValue.ToString());
                        _transportistaDto.i_IdPais = int.Parse(ddlPais.SelectedValue.ToString());
                        _transportistaDto.i_IdDistrito = int.Parse(ddlDistrito.SelectedValue.ToString());
                        _transportistaDto.i_IdDepartamento = int.Parse(ddlDepartamento.SelectedValue.ToString());
                        _transportistaDto.i_IdProvincia = int.Parse(ddlProvincia.SelectedValue.ToString());
                        _transportistaDto.v_CorreoElectronico = txtEMail.Text.Trim();
                        // Save the data
                        _objTransportistaBL.InsertarTransportista(ref objOperationResult, _transportistaDto, Globals.ClientSession.GetAsList(),false );
                        tcDetalle.Controls["tpChofer"].Enabled = true;
                        tcDetalle.Controls["tpUTransporte"].Enabled = true;
                    }
                    else if (_mode == "Edit")
                    {
                        _transportistaDto.v_Codigo = txtCodigo.Text.Trim();
                        _transportistaDto.v_NombreRazonSocial = txtNombre.Text.Trim();
                        _transportistaDto.v_NombreContacto = txtContacto.Text.Trim();
                        _transportistaDto.v_NumeroDocumento = txtNroDocumento.Text.Trim();
                        _transportistaDto.v_Direccion = txtDireccion.Text.Trim();
                        _transportistaDto.v_CorreoElectronico = txtEMail.Text.Trim();
                        _transportistaDto.v_Fax = txtFax.Text.Trim();
                        _transportistaDto.v_Telefono = txtTelefono.Text.Trim();
                        _transportistaDto.i_IdTipoIdentificacion = int.Parse(ddlTipoIdentificacion.SelectedValue.ToString());
                        _transportistaDto.i_IdTipoPersona = int.Parse(ddlbTipoPersona.SelectedValue.ToString());
                        _transportistaDto.i_IdPais = int.Parse(ddlPais.SelectedValue.ToString());
                        _transportistaDto.i_IdDistrito = int.Parse(ddlDistrito.SelectedValue.ToString());
                        _transportistaDto.i_IdDepartamento = int.Parse(ddlDepartamento.SelectedValue.ToString());
                        _transportistaDto.i_IdProvincia = int.Parse(ddlProvincia.SelectedValue.ToString());
                        _transportistaDto.v_CorreoElectronico = txtEMail.Text.Trim();
                        // Save the data
                        _objTransportistaBL.ActualizarTransportista(ref objOperationResult, _transportistaDto, Globals.ClientSession.GetAsList());
                    }
                    //// Analizar el resultado de la operación
                    if (objOperationResult.Success == 1)  // Operación sin error
                    {
                        btnBuscar_Click(sender, e);
                        MantenerSeleccion(_transportistaDto.v_NumeroDocumento);
                        UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else  // Operación con error
                    {
                        UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            private void ddlTipoIdentificacion_SelectedIndexChanged(object sender, EventArgs e)
            {

                if (ddlTipoIdentificacion.SelectedValue != "-1")
                {
                    var x = (KeyValueDTO)ddlTipoIdentificacion.SelectedItem;
                    if (x == null) return;
                    txtNroDocumento.MaxLength = int.Parse(x.Value2);
                    txtNroDocumento.Focus();
                    btnConsultaInternet.Enabled = ddlTipoIdentificacion.SelectedValue.ToString() == "6" || ddlTipoIdentificacion.SelectedValue.ToString() == "1";
                }
            }
            #endregion

            #region Chofer

            private void btnChoferAgregar_Click(object sender, EventArgs e)
            {
                txtChoferNombre.Focus();
                OperationResult objOperationResult = new OperationResult();
                if (uvChofer.Validate(true, false).IsValid)
                {
                    if (txtChoferNombre.Text.Trim() == "")
                    {
                        UltraMessageBox.Show("Por favor ingrese un Nombre al Chofer.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtChoferNombre.Focus();
                        return;
                    }
                    if (txtChoferBrevete.Text.Trim() == "")
                    {
                        UltraMessageBox.Show("Por favor ingrese un Brevete al Chofer.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtChoferBrevete.Focus();
                        return;
                    }

                    if (_modeChofer == "NewChofer")
                    {
                        _transportistachoferDto = new transportistachoferDto();
                        _transportistachoferDto.v_NombreCompleto = txtChoferNombre.Text.Trim();
                        _transportistachoferDto.v_Brevete = txtChoferBrevete.Text.Trim();
                        _transportistachoferDto.v_IdTransportista = _IdTransportista;
                        _transportistachoferDto.i_IdTipoIdentificacion = int.Parse(cboTipoDocChofer.SelectedValue.ToString());
                        _transportistachoferDto.v_NroDocIdentificacion = txtNroDocChofer.Text;
                        _objTransportistaChoferBL.InsertarTransportistaChofer(ref objOperationResult, _transportistachoferDto, Globals.ClientSession.GetAsList());
                        grdChoferData.DataSource = _objTransportistaChoferBL.ObtenerListadoTransportistaChofer(ref objOperationResult, null, null, _IdTransportista);
                        txtChoferBrevete.Clear();
                        txtChoferNombre.Clear(); 
                        txtNroDocChofer.Clear();
                        cboTipoDocChofer.SelectedValue = "-1";
                    }
                    else if (_modeChofer == "EditChofer")
                    {
                        _transportistachoferDto.v_NombreCompleto = txtChoferNombre.Text.Trim();
                        _transportistachoferDto.v_Brevete = txtChoferBrevete.Text.Trim();
                        _transportistachoferDto.i_IdTipoIdentificacion = int.Parse(cboTipoDocChofer.SelectedValue.ToString());
                        _transportistachoferDto.v_NroDocIdentificacion = txtNroDocChofer.Text;
                        _objTransportistaChoferBL.ActualizarTransportistaChofer(ref objOperationResult, _transportistachoferDto, Globals.ClientSession.GetAsList());
                        grdChoferData.DataSource = _objTransportistaChoferBL.ObtenerListadoTransportistaChofer(ref objOperationResult, null, null, _IdTransportista);
                        txtChoferBrevete.Clear();
                        txtChoferNombre.Clear();
                        txtNroDocChofer.Clear();
                        cboTipoDocChofer.SelectedValue = "-1";
                        _modeChofer = "NewChofer";
                    }
                    MantenerSeleccionChofer(_transportistachoferDto.v_Brevete);
                    lblContadorChofer.Text = string.Format("Se encontraron {0} registros.", grdChoferData.Rows.Count());
                    ActivaDesactivaBotonesChofer();
                    ActivaDesactivaBotonesUT();
                }
                else
                {
                    txtChoferNombre.Focus();
                }
            }

            private void btnChoferEditar_Click(object sender, EventArgs e)
            {
                if (grdChoferData.Rows.Count() == 0) return;
                _modeChofer = "EditChofer";
                OperationResult objOperationResult = new OperationResult();
                _transportistachoferDto = new transportistachoferDto();
                _transportistachoferDto = _objTransportistaChoferBL.ObtenerTransportistaChofer(ref objOperationResult, grdChoferData.Rows[grdChoferData.ActiveRow.Index].Cells["v_IdChofer"].Value.ToString());
                txtChoferNombre.Text = _transportistachoferDto.v_NombreCompleto;
                txtChoferBrevete.Text = _transportistachoferDto.v_Brevete;
                cboTipoDocChofer.SelectedValue = (_transportistachoferDto.i_IdTipoIdentificacion ?? -1).ToString();
                txtNroDocChofer.Text = _transportistachoferDto.v_NroDocIdentificacion;
                txtChoferNombre.Focus();
            }

            private void btnChoferEliminar_Click(object sender, EventArgs e)
            {
                if (grdChoferData.Rows.Count() == 0) return;
                OperationResult objOperationResult = new OperationResult();
                DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Result == System.Windows.Forms.DialogResult.Yes)
                {
                    _objTransportistaChoferBL.EliminarTransportistaChofer(ref objOperationResult, grdChoferData.Rows[grdChoferData.ActiveRow.Index].Cells["v_IdChofer"].Value.ToString(), Globals.ClientSession.GetAsList());
                    grdChoferData.DataSource = _objTransportistaChoferBL.ObtenerListadoTransportistaChofer(ref objOperationResult, null, null, _IdTransportista);
                    txtChoferBrevete.Clear();
                    txtChoferNombre.Clear();
                    txtNroDocChofer.Clear();
                    cboTipoDocChofer.SelectedValue = "-1";
                    ActivaDesactivaBotonesChofer();
                    lblContadorChofer.Text = string.Format("Se encontraron {0} registros.", grdChoferData.Rows.Count());
                }
            }

            private void grdChoferData_AfterRowActivate(object sender, EventArgs e)
            {
                btnChoferEditar.Enabled = true;
                btnChoferEliminar.Enabled = true;
            }

            private void grdChoferData_MouseDown(object sender, MouseEventArgs e)
            {
                Point point = new System.Drawing.Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null) return;

                Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

                if (row == null)
                {
                    btnChoferEditar.Enabled = false;
                    btnChoferEliminar.Enabled = false;
                }
                else
                {
                    btnChoferEditar.Enabled = true;
                    btnChoferEliminar.Enabled = true;
                }
            }

            #endregion

            #region UnidadTransporte
            private void btnUTAgregar_Click(object sender, EventArgs e)
            {
                OperationResult objOperationResult = new OperationResult();
                if (uvTransporte.Validate(true, false).IsValid)
                {
                    if (txtTractoPlaca.Text.Trim() == "")
                    {
                        UltraMessageBox.Show("Por favor ingrese una Placa al Tracto.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTractoPlaca.Focus();
                        return;
                    }
                    if (txtTractoMarca.Text.Trim() == "")
                    {
                        UltraMessageBox.Show("Por favor ingrese una Marca al Tracto.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTractoMarca.Focus();
                        return;
                    }
                    if (txtTractoCertificado.Text.Trim() == "")
                    {
                        UltraMessageBox.Show("Por favor ingrese un Certificado al Tracto.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTractoCertificado.Focus();
                        return;
                    }
                    if (_modeTransporte == "NewUT")
                    {
                        _transportistaunidadtransporteDto = new transportistaunidadtransporteDto();
                        _transportistaunidadtransporteDto.v_IdTransportista = _IdTransportista;
                        _transportistaunidadtransporteDto.v_TractoCertificado = txtTractoCertificado.Text.Trim();
                        _transportistaunidadtransporteDto.v_TractoMarca = txtTractoMarca.Text.Trim();
                        _transportistaunidadtransporteDto.v_TractoPlaca = txtTractoPlaca.Text.Trim();
                        _transportistaunidadtransporteDto.v_CarretaCertificado = txtCarretaCertificado.Text.Trim();
                        _transportistaunidadtransporteDto.v_CarretaMarca = txtCarretaMarca.Text.Trim();
                        _transportistaunidadtransporteDto.v_CarretaPlaca = txtCarretaPlaca.Text.Trim();
                        _objTransportistaUnidadTransporteBL.InsertarTransportistaUnidadTransporte(ref objOperationResult, _transportistaunidadtransporteDto, Globals.ClientSession.GetAsList());
                        grdUndTransData.DataSource = _objTransportistaUnidadTransporteBL.ObtenerListadoTransportistaUnidadTransporte(ref objOperationResult, null, null, _IdTransportista);
                        txtCarretaCertificado.Clear();
                        txtCarretaMarca.Clear();
                        txtCarretaPlaca.Clear();
                        txtTractoCertificado.Clear();
                        txtTractoMarca.Clear();
                        txtTractoPlaca.Clear();
                    }
                    else if (_modeTransporte == "EditUT")
                    {
                        _transportistaunidadtransporteDto.v_TractoCertificado = txtTractoCertificado.Text.Trim();
                        _transportistaunidadtransporteDto.v_TractoMarca = txtTractoMarca.Text.Trim();
                        _transportistaunidadtransporteDto.v_TractoPlaca = txtTractoPlaca.Text.Trim();
                        _transportistaunidadtransporteDto.v_CarretaCertificado = txtCarretaCertificado.Text.Trim();
                        _transportistaunidadtransporteDto.v_CarretaMarca = txtCarretaMarca.Text.Trim();
                        _transportistaunidadtransporteDto.v_CarretaPlaca = txtCarretaPlaca.Text.Trim();
                        _objTransportistaUnidadTransporteBL.ActualizarTransportistaUnidadTransporte(ref objOperationResult, _transportistaunidadtransporteDto, Globals.ClientSession.GetAsList());
                        grdUndTransData.DataSource = _objTransportistaUnidadTransporteBL.ObtenerListadoTransportistaUnidadTransporte(ref objOperationResult, null, null, _IdTransportista);
                        txtCarretaCertificado.Clear();
                        txtCarretaMarca.Clear();
                        txtCarretaPlaca.Clear();
                        txtTractoCertificado.Clear();
                        txtTractoMarca.Clear();
                        txtTractoPlaca.Clear();
                        _modeTransporte = "NewUT";
                    }
                    MantenerSeleccionUnidadTransporte(_transportistaunidadtransporteDto.v_TractoPlaca);
                    lblContadorUnidadTransporte.Text = string.Format("Se encontraron {0} registros.", grdUndTransData.Rows.Count());
                }
            }

            private void btnUTEditar_Click(object sender, EventArgs e)
            {
                if (grdUndTransData.Rows.Count() == 0) return;
                _modeTransporte = "EditUT";
                OperationResult objOperationResult = new OperationResult();
                _transportistaunidadtransporteDto = new transportistaunidadtransporteDto();
                _transportistaunidadtransporteDto = _objTransportistaUnidadTransporteBL.ObtenerTransportistaUnidadTransporte(ref objOperationResult, grdUndTransData.Rows[grdUndTransData.ActiveRow.Index].Cells["v_IdUnidadTransporte"].Value.ToString());
                txtCarretaCertificado.Text = _transportistaunidadtransporteDto.v_CarretaCertificado;
                txtCarretaMarca.Text = _transportistaunidadtransporteDto.v_CarretaMarca;
                txtCarretaPlaca.Text = _transportistaunidadtransporteDto.v_CarretaPlaca;
                txtTractoCertificado.Text = _transportistaunidadtransporteDto.v_TractoCertificado;
                txtTractoMarca.Text = _transportistaunidadtransporteDto.v_TractoMarca;
                txtTractoPlaca.Text = _transportistaunidadtransporteDto.v_TractoPlaca;
                txtTractoPlaca.Focus();
            }

            private void btnUTEliminar_Click(object sender, EventArgs e)
            {
                if (grdUndTransData.Rows.Count() == 0) return;
                OperationResult objOperationResult = new OperationResult();
                DialogResult Result = UltraMessageBox.Show("¿Está seguro de eliminar este registro?:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ADVERTENCIA!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Result == System.Windows.Forms.DialogResult.Yes)
                {
                    _objTransportistaUnidadTransporteBL.EliminarTransportistaUnidadTransporte(ref objOperationResult, grdUndTransData.Rows[grdUndTransData.ActiveRow.Index].Cells["v_IdUnidadTransporte"].Value.ToString(), Globals.ClientSession.GetAsList());
                    grdUndTransData.DataSource = _objTransportistaUnidadTransporteBL.ObtenerListadoTransportistaUnidadTransporte(ref objOperationResult, null, null, _IdTransportista);
                    txtCarretaCertificado.Clear();
                    txtCarretaMarca.Clear();
                    txtCarretaPlaca.Clear();
                    txtTractoCertificado.Clear();
                    txtTractoMarca.Clear();
                    txtTractoPlaca.Clear();
                    lblContadorUnidadTransporte.Text = string.Format("Se encontraron {0} registros.", grdUndTransData.Rows.Count());
                    ActivaDesactivaBotonesUT();
                }
            }

            private void grdUndTransData_AfterRowActivate(object sender, EventArgs e)
            {
                btnUTEditar.Enabled = true;
                btnUTEliminar.Enabled = true;
            }

            private void grdUndTransData_MouseDown(object sender, MouseEventArgs e)
            {
                Point point = new System.Drawing.Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null) return;

                Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

                if (row == null)
                {
                    btnUTEditar.Enabled = false;
                    btnUTEliminar.Enabled = false;
                }
                else
                {
                    btnUTEditar.Enabled = true;
                    btnUTEliminar.Enabled = true;
                }
            }

            #endregion

            private void txtCodigo_TextChanged(object sender, EventArgs e)
            {

            }
            private void btnConsultaInternet_Click(object sender, EventArgs e)
            {
                string nroDoc = this.txtNroDocumento.Text.Trim();
                if (ddlTipoIdentificacion.SelectedValue.ToString() == "6")//evaluar
                {
                    if (nroDoc.Length != txtNroDocumento.MaxLength)
                    {
                        UltraMessageBox.Show("El RUC Ingresado es incorrecto", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtNombre.Clear();
                        txtDireccion.Clear();
                        return;
                    }
                    else
                    {
                        if (Utils.Windows.ValidarRuc(nroDoc) != true)
                        {
                            UltraMessageBox.Show("El RUC Ingresado es incorrecto", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtNombre.Clear();
                            txtDireccion.Clear();
                            return;
                        }
                    }

                    string[] _Contribuyente = new string[10];

                    frmCustomerCapchaSUNAT frm = new frmCustomerCapchaSUNAT(nroDoc);
                    frm.ShowDialog();
                    if (frm.ConectadoRecibido == true)
                    {
                        _Contribuyente = frm.DatosContribuyente;

                        if (txtNroDocumento.Text.StartsWith("1") && ddlbTipoPersona.SelectedValue.ToString() == "1")
                        {
                            string[] Cadena = _Contribuyente[0].ToUpper().Trim().Split(new Char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                            if (Cadena.GetUpperBound(0) == 1)
                            {
                                txtNombre.Text = Cadena[0] + " " + Cadena[1] + " " + Cadena[Cadena.Length - 1];
                            }

                            if (Cadena.GetUpperBound(0) == 2)
                            {
                                txtNombre.Text = Cadena[0] + " " + Cadena[1] + " " + Cadena[Cadena.Length - 1];
                            }

                            if (Cadena.GetUpperBound(0) >= 3)
                            {
                                txtNombre.Text = Cadena[0] + " " + Cadena[1] + " " + Cadena[Cadena.Length - 1];
                            }
                        }
                        else
                        {
                            txtNombre.Text = _Contribuyente[0].ToUpper().Trim();
                        }
                        txtDireccion.Text = Regex.Replace(_Contribuyente[5], @"[ ]+", " ");
                        var resultUbigueo = Utils.Ubigeo.GetUbigueo(txtDireccion.Text);
                        if (resultUbigueo != null)
                        {
                            ddlDepartamento.SelectedValue = resultUbigueo[0].Key.ToString();
                            ddlProvincia.SelectedValue = resultUbigueo[1].Key.ToString();
                            ddlDistrito.SelectedValue = resultUbigueo[2].Key.ToString();
                        }
                        txtTelefono.Text = _Contribuyente[6];
                    }
                }

                if (ddlTipoIdentificacion.SelectedValue.ToString() == "1")
                {
                    if (nroDoc.Length != txtNroDocumento.MaxLength)
                    {
                        UltraMessageBox.Show("El DNI Ingresado es incorrecto", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    string[] _Persona = new string[3];

                    frmCustomerCapchaRENIEC frm = new frmCustomerCapchaRENIEC(nroDoc);
                    frm.ShowDialog();
                    if (frm.ConectadoRecibido)
                    {
                        _Persona = frm.DatosPersona;
                        if (_Persona != null)
                        {
                            string[] Cadena = (_Persona[0] + " " + _Persona[1] + " " + _Persona[2]).ToUpper().Trim().Split(new Char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                            if (Cadena.GetUpperBound(0) == 1)
                            {
                                txtNombre.Text = Cadena[0] + " " + Cadena[1] + " " + Cadena[Cadena.Length - 1];
                            }

                            if (Cadena.GetUpperBound(0) == 2)
                            {
                                txtNombre.Text = Cadena[0] + " " + Cadena[1] + " " + Cadena[Cadena.Length - 1];
                            }

                            if (Cadena.GetUpperBound(0) >= 3)
                            {
                                txtNombre.Text = Cadena[0] + " " + Cadena[1] + " " + Cadena[Cadena.Length - 1];
                            }
                        }
                        txtDireccion.Focus();
                    }
                }
            }

            private void txtCodigo_Validating(object sender, CancelEventArgs e)
            {
                if (!string.IsNullOrEmpty(txtCodigo.Text.Trim()))
                {
                    if (TransportistaBL.ExisteTransportista(txtCodigo.Text, _transportistaDto.v_IdTransportista))
                    {
                        MessageBox.Show(@"El código del transportista ya fue registrado anteriormente.!", @"Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txtCodigo.Focus();
                    }
                }
            }
        #endregion 


    }
}
