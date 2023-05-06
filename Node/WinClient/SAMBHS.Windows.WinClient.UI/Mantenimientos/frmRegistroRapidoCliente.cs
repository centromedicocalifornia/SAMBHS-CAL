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
using System.Text.RegularExpressions;
using Infragistics.Documents.Excel;
using System.Net;
using System.Configuration;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmRegistroRapidoCliente : Form
    {
        ClienteBL _objClienteBL = new ClienteBL();
        SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        clienteDto _clienteDto = new clienteDto();
        string _NroDocumento, _mode;
        string _Origen;
        public bool _Guardado = false;
        public string _NroDocumentoReturn = string.Empty, _Codigo = string.Empty, _Nombres = string.Empty;
        public int TipoDocumentoReturn = 0;


        public frmRegistroRapidoCliente(string NroDocumento, string Origen)
        {
            _NroDocumento = NroDocumento;
            _Origen = Origen;
            InitializeComponent();
        }

        private void frmRegistroRapidoCliente_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().FormColor;

            this.Text = _Origen == "V" ? "Registro Rápido de Proveedor" :  _Origen =="T"?"Registro Rápido de Trabajador": "Registro Rápido de Cliente";
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

            //Utils.Windows.LoadUltraComboEditorList(cboGenero, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 3, null), DropDownListAction.Select);
            #endregion

            _mode = "New";
            ddlPais.Value = "1";
            ddlDepartamento.Value = "1391";
            ddlProvincia.Value = "1392";
            ddlDistrito.Value = "1393";
            cboTipoPersona.Value = "1";
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

            lblEstadoContribuyente.Text = string.Empty;
            pbEstadoCondicion.Visible = false;
            txtNroDocumento.Focus();
        }

        private void btnConsultaInternet_Click(object sender, EventArgs e)
        {
            string nroDoc = this.txtNroDocumento.Text.Trim();
            if (cboTipoDocumento.Value.ToString() == "6")//evaluar
            {
                if (nroDoc.Length != txtNroDocumento.MaxLength)
                {
                    UltraMessageBox.Show("El RUC Ingresado es incorrecto", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtApeMaterno.Clear();
                    txtApePaterno.Clear();
                    txtPrimerNombre.Clear();
                    txtSegundoNombre.Clear();
                    txtDireccion.Clear();
                    _clienteDto.v_NroDocIdentificacion = null;
                    return;
                }
                else
                {
                    if (Utils.Windows.ValidarRuc(nroDoc) != true)
                    {
                        UltraMessageBox.Show("El RUC Ingresado es incorrecto", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtApeMaterno.Clear();
                        txtApePaterno.Clear();
                        txtPrimerNombre.Clear();
                        txtSegundoNombre.Clear();
                        txtDireccion.Clear();
                        _clienteDto.v_NroDocIdentificacion = null;
                        txtCodigoAnexo.Clear();
                        return;
                    }
                }
                string[] _Contribuyente = new string[10];

                //frmCustomerCapchaSUNAT frm = new frmCustomerCapchaSUNAT(nroDoc);
                //frm.ShowDialog();
                //if (frm.ConectadoRecibido == true)
                //{
                //_Contribuyente = frm.DatosContribuyente;
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                string ruta = GetApplicationConfigValue("tokenApisunat").ToString();
                var urlReniec = "https://dniruc.apisperu.com/api/v1/ruc/" + txtNroDocumento.Text.Trim() + "?token=" + ruta;

                System.Net.WebClient wcReniec = new System.Net.WebClient();
                string DataReniec = wcReniec.DownloadString(urlReniec);
                string[] Cadena = DataReniec.ToUpper().Trim().Split(new Char[] { ',', ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (txtNroDocumento.Text.StartsWith("1") && cboTipoPersona.Value.ToString() == "1")
                {
                    string nombre = Cadena[3].Replace("\"", "");
                    nombre = nombre.Replace("[", "");
                    nombre = nombre.Replace("]", "");
                    if (nombre.Split(' ').Count() > 3)
                    {
                        txtApePaterno.Text = nombre.Split(' ')[0];
                        txtApeMaterno.Text = nombre.Split(' ')[1];
                        txtPrimerNombre.Text = nombre.Split(' ')[2];
                        txtSegundoNombre.Text = nombre.Split(' ')[3];
                    }
                    else
                    {
                        txtApePaterno.Text = nombre.Split(' ')[0];
                        txtApeMaterno.Text = nombre.Split(' ')[1];
                        txtPrimerNombre.Text = nombre.Split(' ')[2];
                        txtSegundoNombre.Text = string.Empty;
                    }
                }
                else
                {
                    string razonsocial = Cadena[3].Replace("\"", "");
                    razonsocial = razonsocial.Replace("[", "");
                    razonsocial = razonsocial.Replace("]", "");

                    txtPrimerNombre.Text = razonsocial.ToUpper().Trim();
                    txtSegundoNombre.Text = string.Empty;
                    txtApePaterno.Text = string.Empty;
                    txtApeMaterno.Text = string.Empty;
                }
                string direccion = Cadena[15].Replace("\"", "");

                string departamento = Cadena[17].Replace("\"", "");
                departamento = Regex.Replace(departamento, @"[ ]+", "");
                string provincia = Cadena[19].Replace("\"", "");
                provincia = Regex.Replace(provincia, @"[ ]+", "");
                string distrito = Cadena[21].Replace("\"", "");
                distrito = Regex.Replace(distrito, @"[ ]+", "");

                var direccionConcatenada = direccion + " " + departamento + " - " + provincia + " - " + distrito;
                txtDireccion.Text = direccionConcatenada;
                var resultUbigueo = Utils.Ubigeo.GetUbigueo(direccionConcatenada);
                if (resultUbigueo != null)
                {
                    ddlDepartamento.Value = resultUbigueo[0].Key;
                    ddlProvincia.Value = resultUbigueo[1].Key;
                    ddlDistrito.Value = resultUbigueo[2].Key;
                }
                pbEstadoCondicion.Visible = true;
                string estado = Cadena[11].Replace("\"", "");
                estado = Regex.Replace(estado, @"[ ]+", "");
                string condicion = Cadena[13].Replace("\"", "");
                condicion = Regex.Replace(condicion, @"[ ]+", "");

                pbEstadoCondicion.Image = estado.Trim().ToUpper().Equals("ACTIVO") &&
                                          condicion.Trim().ToUpper().Equals("HABIDO")
                    ? Resource.accept
                    : Resource.alerta;
                lblEstadoContribuyente.Text = string.Format("ESTADO: {0} | CONDICIÓN: {1}", estado, condicion);
                _clienteDto.v_TelefonoFijo = string.Empty;
                //}
            }

            if (cboTipoDocumento.Value.ToString() == "1")
            {
                if (nroDoc.Length != txtNroDocumento.MaxLength)
                {
                    UltraMessageBox.Show("El DNI Ingresado es incorrecto", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                try
                {
                    System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    var urlEssalud = "http://ww1.essalud.gob.pe/sisep/postulante/postulante/postulante_obtenerDatosPostulante.htm?strDni=" + nroDoc.Trim();

                    WebClient wcEssalud = new WebClient();

                    var DataEssalud = wcEssalud.DownloadString(urlEssalud);

                    string validar = DataEssalud.Split(',', ':')[6].Replace("\"", "").Trim();
                    string validar2 = DataEssalud.Split('>', ' ')[0].Replace("<", "").Trim();
                    if ((validar != "" && validar != null) && validar2 != "html")
                    {
                        string[] desconcat = DataEssalud.Split(',', ':');

                        //txtPrimerNombre.Text = desconcat[6].Replace("\"", "").Trim();
                        string[] nombres = desconcat[6].Replace("\"", "").Trim().Split(' ');
                        txtPrimerNombre.Text = nombres[0];
                        if (nombres.Count() >= 1)
                        {
                            txtSegundoNombre.Text = nombres[1];
                        }
                        else
                        {
                            txtSegundoNombre.Text = "";
                        }

                        txtApePaterno.Text = desconcat[4].Replace("\"", "").Trim();

                        txtApeMaterno.Text = desconcat[12].Replace("\"", "").Trim();
                        txtApeMaterno.Text = txtApeMaterno.Text.Replace("}", "").Trim();
                        txtApeMaterno.Text = txtApeMaterno.Text.Replace("]", "").Trim();

                        txtNroDocumento.Text = desconcat[2].Replace("\"", "").Trim();
                    }
                    else
                    {
                        string _ApiDni = GetApplicationConfigValue("ApiDni").ToString();

                        var urlReniec = "https://dniruc.apisperu.com/api/v1/dni/" + nroDoc.Trim() + _ApiDni;
                        System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                        //var urlReniec = "https://api.reniec.cloud/dni/" + txtSearchNroDocument.Text.Trim();

                        System.Net.WebClient wcReniec = new System.Net.WebClient();
                        string DataReniec = wcReniec.DownloadString(urlReniec);

                        if (DataReniec != null && DataReniec != "null")
                        {
                            string[] desconcat = DataReniec.Split(',', ':');

                            //string[] nombres = desconcat[3].Replace("\"", "").Trim().Split(' ');
                            string[] nombres = desconcat[3].Replace("}", "").Trim().Split(' ');
                            txtPrimerNombre.Text = nombres[0].Replace("\"", "").Trim();
                            if (nombres.Count() >= 1)
                            {
                                txtSegundoNombre.Text = nombres[1].Replace("\"", "").Trim();
                            }
                            else
                            {
                                txtSegundoNombre.Text = "";
                            }

                            txtApePaterno.Text = desconcat[5].Replace("\"", "").Trim();
                            txtApeMaterno.Text = desconcat[7].Replace("\"", "").Trim();
                            txtNroDocumento.Text = desconcat[1].Replace("\"", "").Trim();
                        }


                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(@"Nro. de DNI incorrecto", @"Información");
                    //throw;
                }
                //string[] _Persona = new string[3];

                //frmCustomerCapchaRENIEC frm = new frmCustomerCapchaRENIEC(nroDoc);
                //frm.ShowDialog();
                //if (frm.ConectadoRecibido == true)
                //{
                //    _Persona = frm.DatosPersona;
                //    if (_Persona != null)
                //    {
                //        string[] Cadena = (_Persona[0] + " " + _Persona[1] + " " + _Persona[2]).ToUpper().Trim().Split(new Char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                //        if (Cadena.GetUpperBound(0) == 1)
                //        {
                //            txtApePaterno.Text = Cadena[0];
                //            txtApeMaterno.Text = Cadena[1];
                //            txtPrimerNombre.Text = Cadena[Cadena.Length - 1];
                //            txtSegundoNombre.Text = string.Empty;
                //        }

                //        if (Cadena.GetUpperBound(0) == 2)
                //        {
                //            txtApePaterno.Text = Cadena[0];
                //            txtApeMaterno.Text = Cadena[1];
                //            txtPrimerNombre.Text = Cadena[Cadena.Length - 1];
                //            txtSegundoNombre.Text = string.Empty;
                //        }

                //        if (Cadena.GetUpperBound(0) >= 3)
                //        {
                //            txtApePaterno.Text = Cadena[0];
                //            txtApeMaterno.Text = Cadena[1];
                //            txtPrimerNombre.Text = Cadena[Cadena.Length - 2];
                //            txtSegundoNombre.Text = Cadena[Cadena.Length - 1];
                //        }
                //    }
                //    txtDireccion.Clear();
                //    txtDireccion.Focus();
                //}
            }

            button1.Focus();
        }

        public static string GetApplicationConfigValue(string nombre)
        {
            return Convert.ToString(ConfigurationManager.AppSettings[nombre]);
        }

        private void cboTipoDocumento_SelectedIndexChanged(object sender, EventArgs e)
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
                    lblNombreRS.Text = "Razón Social:";
                    txtSegundoNombre.Enabled = false;
                    txtApeMaterno.Enabled = false;
                    txtApePaterno.Enabled = false;
                    txtPrimerNombre.MaxLength = 120;

                }
                else
                {
                    cboTipoDocumento.Value = "1";
                    cboTipoDocumento.Enabled = true;
                    lblNombreRS.Text = "Primer Nombre:";
                    txtSegundoNombre.Enabled = true;
                    txtApeMaterno.Enabled = true;
                    txtApePaterno.Enabled = true;
                    txtPrimerNombre.MaxLength = 30;
                }
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(cboTipoDocumento, "Value1", "Id", _objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, -1, null), DropDownListAction.Select);
            }

        }

        private void ddlDepartamento_SelectedIndexChanged(object sender, EventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            if (uvDatos.Validate(true, false).IsValid)
            {

                if (txtCodigoAnexo.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Código.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCodigoAnexo.Focus();
                    return;
                }

                if (txtNroDocumento.Text.Trim() == "" && cboTipoPersona.Value.ToString() != "3" && cboTipoDocumento.Value.ToString() != "0")
                {
                    UltraMessageBox.Show("Por favor ingrese un Nro. Documento.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNroDocumento.Focus();
                    return;
                }

                if (txtPrimerNombre.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese un Nombre o Razón Social.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPrimerNombre.Focus();
                    return;
                }
                if (txtDireccion.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por favor ingrese una Dirección.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDireccion.Focus();
                    return;
                }

                if (_objClienteBL.ObtenerClienteCodigo(ref objOperationResult, txtCodigoAnexo.Text, _clienteDto.v_IdCliente, _Origen) != null)
                {
                    UltraMessageBox.Show("Este Código Anexo ya ha sido registrado ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (cboTipoPersona.Value.ToString() != "3" && cboTipoDocumento.Value.ToString() != "0")
                {

                    if (_objClienteBL.ObtenerClienteDocumentoIdentificacion(ref objOperationResult, txtNroDocumento.Text, _clienteDto.v_IdCliente, _Origen) != null)
                    {
                        UltraMessageBox.Show("Este Nro. Documento ya ha sido registrado ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                var RucEmpresa = new NodeBL().ReporteEmpresa().FirstOrDefault().RucEmpresaPropietaria.Trim ();
                if (RucEmpresa != null && RucEmpresa != Constants.RucNotariaBecerrSosaya)
                {
                    if (cboTipoDocumento.Value.ToString() == "6") // Si es Ruc-->Busco si que no se haya registrado con Dni
                    {

                        var objClienteConsulta = _objClienteBL.ObtenerClienteDocumentoIdentificacion(ref objOperationResult, txtNroDocumento.Text.Substring(2, 8), _clienteDto.v_IdCliente, "V");
                        if (objClienteConsulta != null)
                        {
                            UltraMessageBox.Show("Este Nro. Documento ya ha sido registrado con :" + objClienteConsulta.v_NroDocIdentificacion, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                    else if (cboTipoDocumento.Value.ToString() == "1") // Si es Dni ,verifico que no se haya registrado con su Ruc
                    {
                        if (cboTipoPersona.Value.ToString() == "1") //Natural
                        {
                            string Ruc = "10" + txtNroDocumento.Text + "X";
                            Ruc = "10" + txtNroDocumento.Text + CalcularUltimoDigitoRuc(Ruc);
                            var objClienteConsulta = _objClienteBL.ObtenerClienteDocumentoIdentificacion(ref objOperationResult, Ruc, _clienteDto.v_IdCliente, "V");
                            if (objClienteConsulta != null)
                            {
                                UltraMessageBox.Show("Este Nro. Documento ya ha sido registrado con :" + objClienteConsulta.v_NroDocIdentificacion, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                        }
                        else if (cboTipoPersona.Value.ToString() == "2") //Juridica
                        {
                            string Ruc = "20" + txtNroDocumento.Text + "X";
                            Ruc = "20" + txtNroDocumento.Text + CalcularUltimoDigitoRuc(Ruc);
                            var objClienteConsulta = _objClienteBL.ObtenerClienteDocumentoIdentificacion(ref objOperationResult, Ruc, _clienteDto.v_IdCliente, "V");

                            if (objClienteConsulta != null)
                            {
                                UltraMessageBox.Show("Este Nro. Documento ya ha sido registrado con :" + objClienteConsulta.v_NroDocIdentificacion, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                    }
                }
                if (_mode == "New")
                {
                    trabajadorDto _objTrabajador = new trabajadorDto();
                    if (_Origen == Constants.FlagTrabajador)
                    {
                        _objTrabajador.v_CodInterno = txtCodigoAnexo.Text.Trim();
                    }
                    _clienteDto = new clienteDto();
                    _clienteDto.v_CodCliente = txtCodigoAnexo.Text.Trim();
                    _clienteDto.i_IdTipoPersona = int.Parse(cboTipoPersona.Value.ToString());
                    _clienteDto.i_IdTipoIdentificacion = int.Parse(cboTipoDocumento.Value.ToString());
                    if (cboTipoPersona.Value.ToString() != "2")
                    {
                        _clienteDto.v_PrimerNombre = txtPrimerNombre.Text.Trim();
                        _clienteDto.v_SegundoNombre = txtSegundoNombre.Text.Trim();
                        _clienteDto.v_ApePaterno = txtApePaterno.Text.Trim();
                        _clienteDto.v_ApeMaterno = txtApeMaterno.Text.Trim();
                        _clienteDto.v_RazonSocial = string.Empty;
                    }
                    else
                    {
                        _clienteDto.v_PrimerNombre = string.Empty;
                        _clienteDto.v_SegundoNombre = string.Empty;
                        _clienteDto.v_ApePaterno = string.Empty;
                        _clienteDto.v_ApeMaterno = string.Empty;
                        _clienteDto.v_RazonSocial = txtPrimerNombre.Text.Trim();
                    }
                    _clienteDto.v_NroDocIdentificacion = txtNroDocumento.Text.Trim();
                    _clienteDto.v_DirecPrincipal = Regex.Replace(txtDireccion.Text, @"[ ]+", " ");
                    _clienteDto.v_DirecPrincipal = _clienteDto.v_DirecPrincipal.Length <= 200 ? _clienteDto.v_DirecPrincipal : _clienteDto.v_DirecPrincipal.Substring(0, 200);
                    _clienteDto.v_Correo = txtEMail.Text.Trim();
                    _clienteDto.i_IdPais = int.Parse(ddlPais.Value.ToString());
                    _clienteDto.i_IdDistrito = int.Parse(ddlDistrito.Value.ToString());
                    _clienteDto.i_IdDepartamento = int.Parse(ddlDepartamento.Value.ToString());
                    _clienteDto.i_IdProvincia = int.Parse(ddlProvincia.Value.ToString());
                    _clienteDto.i_IdSexo = -1;
                    _clienteDto.v_FlagPantalla = _Origen;
                    _clienteDto.i_Activo = 1;
                    _clienteDto.i_IdListaPrecios = 1;
                    _clienteDto.v_TelefonoFijo = txtTelefono.Text.Trim();
                    _clienteDto.v_TelefonoMovil = txtCelular.Text.Trim();

                    // Save the data
                    _objClienteBL.InsertarCliente(ref objOperationResult, _clienteDto, Globals.ClientSession.GetAsList(), null, _objTrabajador, null, null, null, null);
                }
                //// Analizar el resultado de la operación
                if (objOperationResult.Success == 1)  // Operación sin error
                {
                    _Guardado = true;
                    _NroDocumentoReturn = txtNroDocumento.Text.Trim();
                    _Codigo = txtCodigoAnexo.Text.Trim();
                    _Nombres = txtPrimerNombre.Text.Trim() + " " + txtSegundoNombre.Text.Trim() + " " + txtApePaterno.Text.Trim() + " " + txtApeMaterno.Text.Trim();
                    TipoDocumentoReturn = int.Parse(cboTipoDocumento.Value.ToString());
                    //lblDatosGuardados.Visible = true;
                    this.Close();
                }
                else  // Operación con error
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        public int CalcularUltimoDigitoRuc(string NroDocumento)
        {

            int[] Factores = { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };
            int[] Productos = new int[10];
            //int LongitudDocumento = NroDocumento.Length;
            int SumaProductos, Resultado;
            //int NroIdentificador = int.Parse(NroDocumento.Substring(LongitudDocumento - 1, 1));

            for (int i = 0; i < 10; i++)
            {
                int Valor = int.Parse(NroDocumento.Substring(i, 1));
                Productos[i] = Valor * Factores[i];
            }

            SumaProductos = Productos.Sum();
            Resultado = 11 - (SumaProductos % 11);

            switch (Resultado)
            {
                case 10:
                    Resultado = 0;
                    break;

                case 11:
                    Resultado = 1;
                    break;
            }

            if (Resultado > 11)
            {
                string _Result;
                _Result = Resultado.ToString();
                Resultado = int.Parse(_Result.Substring(_Result.Length - 1, 1));
                return Resultado;
            }
            else
            {
                return Resultado;
            }




        }
        private void txtNroDocumento_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNroDocumento.Text.Trim()))
            {
                if (cboTipoDocumento.Value.ToString() != "-1")
                {
                    if (txtNroDocumento.TextLength != txtNroDocumento.MaxLength)
                    {
                        UltraMessageBox.Show("Nro. Documento Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtApeMaterno.Clear();
                        txtApePaterno.Clear();
                        txtPrimerNombre.Clear();
                        txtSegundoNombre.Clear();
                        txtDireccion.Clear();
                        _clienteDto.v_NroDocIdentificacion = null;
                        txtNroDocumento.Focus();
                    }
                    else if (cboTipoDocumento.Value.ToString() == "6")
                    {
                        if (Utils.Windows.ValidarRuc(txtNroDocumento.Text.Trim()) == false)
                        {
                            UltraMessageBox.Show("Nro. Documento Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            txtApeMaterno.Clear();
                            txtApePaterno.Clear();
                            txtPrimerNombre.Clear();
                            txtSegundoNombre.Clear();
                            txtDireccion.Clear();
                            _clienteDto.v_NroDocIdentificacion = null;
                            txtNroDocumento.Focus();
                        }
                    }
                }
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtNroDocumento_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroEnteroUltraTextBox(txtNroDocumento, e);
            btnConsultaInternet_Click(sender, e);
        }

        private void txtNroDocumento_ValueChanged(object sender, EventArgs e)
        {
            txtCodigoAnexo.Text = txtNroDocumento.Text;
        }
    }
}
