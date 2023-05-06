using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmRegistroRapidoTransportista : Form
    {
        string _NroDocumento, _mode;

        SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        transportistaDto _transportistaDto = new transportistaDto();
        transportistachoferDto _transportistachoferDto = new transportistachoferDto();
        transportistaunidadtransporteDto _transportistaunidadtransporteDto = new transportistaunidadtransporteDto();
        TransportistaBL _objTransportistaBL = new TransportistaBL();
        public bool _Guardado = false;
        public string _NroDocumentoReturn = "";
        public frmRegistroRapidoTransportista(string NroDocumento)
        {
            _NroDocumento = NroDocumento;
            InitializeComponent();
        }

        private void txtDireccion_ValueChanged(object sender, EventArgs e)
        {

        }

        private void frmRegistroRapidoTransportista_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            // panel1.BackColor = new GlobalFormColors().FormColor;

            //if (_Origen == "V")
            //{
            //    this.Text = "Registro Rápido de Proveedor";
            //}
            //else
            //{
            //    this.Text = "Registro Rápido de Cliente";
            //}
            OperationResult objOperationResult = new OperationResult();

            #region Cargar combos
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
            #endregion


            _mode = "New";
            ddlPais.Value = "1";
            ddlDepartamento.Value = "1391";
            ddlProvincia.Value = "1392";
            ddlDistrito.Value = "1393";
            if (!string.IsNullOrEmpty(_NroDocumento))
            {
                txtNroDocumento.Text = _NroDocumento.Trim();
                txtCodigoAnexo.Text = _NroDocumento.Trim();

                if (_NroDocumento.Length == 11)
                {
                    if (_NroDocumento.Substring(0, 1) == "2")
                    {
                        cboTipoPersona.Value = "2";
                        cboTipoDocumento.Value = "6";
                    }
                    else if (_NroDocumento.Substring(0, 1) == "1")
                    {
                        cboTipoPersona.Value = "1";
                        cboTipoDocumento.Value = "6";
                    }
                }
                else if (_NroDocumento.Length == 8)
                {
                    cboTipoPersona.Value = "1";
                    cboTipoDocumento.Value = "1";
                }
            }
        }

        private void cboTipoPersona_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (cboTipoPersona.Value.ToString() != "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(cboTipoDocumento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 150, null), DropDownListAction.Select);

                if (cboTipoPersona.Value.ToString() == "2")
                {

                    cboTipoDocumento.Value = "6";
                    cboTipoDocumento.Enabled = false;
                    lblNombreRS.Text = "Nombre/Razón Social:";
                    //txtSegundoNombre.Enabled = false;
                    //txtApeMaterno.Enabled = false;
                    //txtApePaterno.Enabled = false;
                    txtPrimerNombre.MaxLength = 120;

                }
                else
                {
                    cboTipoDocumento.Value = "-1";
                    cboTipoDocumento.Enabled = true;
                    lblNombreRS.Text = "Nombre/Razón Social:";
                    //txtSegundoNombre.Enabled = true;
                    //txtApeMaterno.Enabled = true;
                    //txtApePaterno.Enabled = true;
                    txtPrimerNombre.MaxLength = 120;
                }
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(cboTipoDocumento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, -1, null), DropDownListAction.Select);
            }
        }

        private void cboTipoDocumento_ValueChanged(object sender, EventArgs e)
        {
            if (cboTipoDocumento.Value != "-1")
            {
                var x = (KeyValueDTO)cboTipoDocumento.SelectedItem.ListObject;
                if (x == null) return;
                txtNroDocumento.MaxLength = int.Parse(x.Value2);
                txtNroDocumento.Focus();
                btnConsultaInternet.Enabled = cboTipoDocumento.Value.ToString() == "6" || cboTipoDocumento.Value.ToString() == "1" ? true : false;
            }
        }

        private void btnGuardarT_Click(object sender, EventArgs e)
        {

            if (Validar.Validate(true, false).IsValid)
            {

                if (txtCodigoAnexo.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Código Transportista.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCodigoAnexo.Focus();
                    return;
                }

                if (txtPrimerNombre.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Nombre/Razón Social Transportista.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPrimerNombre.Focus();
                    return;
                }

                if (txtNroDocumento.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese el Número de Documento del Transportista.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNroDocumento.Focus();
                    return;
                }

                if (txtNroBrevete.Text.Trim() != string.Empty && txtNombreChofer.Text.Trim() == string.Empty)
                {
                    UltraMessageBox.Show("Por favor ingrese Nombre del Chofer.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombreChofer.Focus();
                    return;
                }

                if (!string.IsNullOrEmpty(txtCodigoAnexo.Text.Trim()))
                {
                    if (TransportistaBL.ExisteTransportista(txtCodigoAnexo.Text,null))
                    {
                        MessageBox.Show("El código del transportista ya fue registrado anteriormente.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(txtTractoPlaca.Text.Trim()) && (string.IsNullOrEmpty(txtTractoMarca.Text.Trim()) || string.IsNullOrEmpty(txtTractoCertificado.Text.Trim())))
                {

                    if (string.IsNullOrEmpty(txtTractoMarca.Text.Trim()))
                    {
                        UltraMessageBox.Show("Por favor ingrese Marca de la Unidad Transporte.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTractoPlaca.Focus();
                        return;
                    }
                    else
                    {
                        UltraMessageBox.Show("Por favor ingrese Certificado de la Unidad Transporte.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTractoCertificado.Focus();
                        return;
                    }
                }

                OperationResult objOperationResult = new OperationResult();
                _transportistaDto = new transportistaDto();
                _transportistaDto.v_Codigo = txtCodigoAnexo.Text.Trim();
                _transportistaDto.v_NombreRazonSocial = txtPrimerNombre.Text.Trim();
                _transportistaDto.v_NombreContacto = txtContacto.Text.Trim();
                _transportistaDto.v_NumeroDocumento = txtNroDocumento.Text.Trim();
                _transportistaDto.v_Direccion = txtDireccion.TextLength > 150 ? txtDireccion.Text.Substring(0, 140) : txtDireccion.Text.Trim();
                _transportistaDto.v_CorreoElectronico = "";
                _transportistaDto.v_Fax = "";
                _transportistaDto.v_Telefono = txtTelefono.Text.Trim();
                _transportistaDto.i_IdTipoIdentificacion = int.Parse(cboTipoDocumento.Value.ToString());
                _transportistaDto.i_IdTipoPersona = int.Parse(cboTipoPersona.Value.ToString());
                _transportistaDto.i_IdPais = int.Parse(ddlPais.Value.ToString());
                _transportistaDto.i_IdDistrito = int.Parse(ddlDistrito.Value.ToString());
                _transportistaDto.i_IdDepartamento = int.Parse(ddlDepartamento.Value.ToString());
                _transportistaDto.i_IdProvincia = int.Parse(ddlProvincia.Value.ToString());
                _transportistaDto.v_CorreoElectronico = "";
                _transportistachoferDto = new transportistachoferDto();
                _transportistachoferDto.v_NombreCompleto = txtNombreChofer.Text.Trim();
                _transportistachoferDto.v_Brevete = txtNroBrevete.Text.Trim();
                _transportistaunidadtransporteDto = new transportistaunidadtransporteDto();
                _transportistaunidadtransporteDto.v_TractoCertificado = txtTractoCertificado.Text.Trim();
                _transportistaunidadtransporteDto.v_TractoMarca = txtTractoMarca.Text.Trim();
                _transportistaunidadtransporteDto.v_TractoPlaca = txtTractoPlaca.Text.Trim();
                _transportistaunidadtransporteDto.v_CarretaCertificado = txtCarretaCertificado.Text.Trim();
                _transportistaunidadtransporteDto.v_CarretaMarca = txtCarretaMarca.Text.Trim();
                _transportistaunidadtransporteDto.v_CarretaPlaca = txtCarretaPlaca.Text.Trim();

                // Save the data
                _objTransportistaBL.InsertarTransportista(ref objOperationResult, _transportistaDto, Globals.ClientSession.GetAsList(), true, _transportistachoferDto, _transportistaunidadtransporteDto);

                if (objOperationResult.Success == 1)
                {

                    _Guardado = true;
                    _NroDocumentoReturn = txtNroDocumento.Text.Trim();
                    this.Close();
                }
                else
                {
                    UltraMessageBox.Show("Ocurrió un Error al Guardar , contactese con el Administrador de Sistema", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }

            }
        }

        private void txtNroDocumento_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtNroDocumento, e);
        }

        private void ddlPais_ValueChanged(object sender, EventArgs e)
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

        private void ddlDepartamento_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlDepartamento.Value == null) return;

            if (ddlDepartamento.Value.ToString() == "-1")
            {
                Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, int.Parse(ddlDepartamento.Value.ToString()), 112, ""), DropDownListAction.Select);
            }
        }

        private void ddlProvincia_ValueChanged(object sender, EventArgs e)
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

        private void btnConsultaInternet_Click(object sender, EventArgs e)
        {
            string nroDoc = this.txtNroDocumento.Text.Trim();
            if (cboTipoDocumento.Value.ToString() == "6")//evaluar
            {
                if (nroDoc.Length != txtNroDocumento.MaxLength)
                {
                    UltraMessageBox.Show("El RUC Ingresado es incorrecto", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtPrimerNombre.Clear();
                    txtDireccion.Clear();
                    _transportistaDto.v_NumeroDocumento = null;
                    return;
                }
                else
                {
                    if (Utils.Windows.ValidarRuc(nroDoc) != true)
                    {
                        UltraMessageBox.Show("El RUC Ingresado es incorrecto", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtPrimerNombre.Clear();
                        txtDireccion.Clear();
                        _transportistaDto.v_NumeroDocumento = null;
                        txtCodigoAnexo.Clear();
                        return;
                    }
                }
                string[] _Contribuyente = new string[10];

                frmCustomerCapchaSUNAT frm = new frmCustomerCapchaSUNAT(nroDoc);
                frm.ShowDialog();
                if (frm.ConectadoRecibido == true)
                {
                    _Contribuyente = frm.DatosContribuyente;

                    if (txtNroDocumento.Text.StartsWith("1") && cboTipoPersona.Value.ToString() == "1")
                    {
                        string[] Cadena = _Contribuyente[0].ToUpper().Trim().Split(new Char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                        if (Cadena.GetUpperBound(0) == 1)
                        {
                            // txtApePaterno.Text = Cadena[0];
                            //txtApeMaterno.Text = Cadena[1];
                            txtPrimerNombre.Text = Cadena[0] + " " + Cadena[1] + " " + Cadena[Cadena.Length - 1];
                            //txtSegundoNombre.Text = string.Empty;
                        }

                        if (Cadena.GetUpperBound(0) == 2)
                        {
                            // txtApePaterno.Text = Cadena[0];
                            //txtApeMaterno.Text = Cadena[1];
                            txtPrimerNombre.Text = Cadena[0] + " " + Cadena[1] + " " + Cadena[Cadena.Length - 1];
                            // txtSegundoNombre.Text = string.Empty;
                        }

                        if (Cadena.GetUpperBound(0) >= 3)
                        {
                            //  txtApePaterno.Text = Cadena[0];
                            // txtApeMaterno.Text = Cadena[1];
                            txtPrimerNombre.Text = Cadena[0] + " " + Cadena[1] + " " + Cadena[Cadena.Length - 2];
                            //txtSegundoNombre.Text = Cadena[Cadena.Length - 1];
                        }
                    }
                    else
                    {
                        txtPrimerNombre.Text = _Contribuyente[0].ToUpper().Trim();
                        //   txtSegundoNombre.Text = string.Empty;
                        //  txtApePaterno.Text = string.Empty;
                        // txtApeMaterno.Text = string.Empty;
                    }

                    txtDireccion.Text = _Contribuyente[5];
                    lblEstadoContribuyente.Text = _Contribuyente[3];
                    // _clienteDto.v_TelefonoFijo = _Contribuyente[6];
                }
            }

            if (cboTipoDocumento.Value.ToString() == "1")
            {
                if (nroDoc.Length != txtNroDocumento.MaxLength)
                {
                    UltraMessageBox.Show("El DNI Ingresado es incorrecto", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string[] _Persona = new string[3];

                frmCustomerCapchaRENIEC frm = new frmCustomerCapchaRENIEC(nroDoc);
                frm.ShowDialog();
                if (frm.ConectadoRecibido == true)
                {
                    _Persona = frm.DatosPersona;
                    if (_Persona != null)
                    {
                        string[] Cadena = (_Persona[0] + " " + _Persona[1] + " " + _Persona[2]).ToUpper().Trim().Split(new Char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                        if (Cadena.GetUpperBound(0) == 1)
                        {
                            //txtApePaterno.Text = Cadena[0];
                            //txtApeMaterno.Text = Cadena[1];
                            txtPrimerNombre.Text = Cadena[0] + " " + Cadena[1] + " " + Cadena[Cadena.Length - 1];
                            // txtSegundoNombre.Text = string.Empty;
                        }

                        if (Cadena.GetUpperBound(0) == 2)
                        {
                            //txtApePaterno.Text = Cadena[0];
                            //txtApeMaterno.Text = Cadena[1];
                            txtPrimerNombre.Text = Cadena[0] + " " + Cadena[1] + " " + Cadena[Cadena.Length - 1];
                            //txtSegundoNombre.Text = string.Empty;
                        }

                        if (Cadena.GetUpperBound(0) >= 3)
                        {
                            //txtApePaterno.Text = Cadena[0];
                            //txtApeMaterno.Text = Cadena[1];
                            txtPrimerNombre.Text = Cadena[0] + " " + Cadena[1] + " " + Cadena[Cadena.Length - 2];
                            //    txtSegundoNombre.Text = Cadena[Cadena.Length - 1];
                        }
                    }
                    txtDireccion.Clear();
                    txtDireccion.Focus();
                }
            }
        }
    }
}
