using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Windows.SigesoftIntegration.UI;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using System.Threading.Tasks;
using SAMBHS.Common.BE;
using Task = System.Threading.Tasks.Task;
using ScrapperReniecSunat;
using Sigesoft.Node.WinClient.UI;
using System.Drawing.Imaging;
using SAMBHS.Common.Resource;
using System.Transactions;
using System.Net;
using System.Text;
using System.Configuration;
using SAMBHS.Common.DataModel;
using System.Text.RegularExpressions;
using Newtonsoft.Json;


//SAMBHS.Windows.WinClient.UI.Sigesoft
namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmAgendarTrabajador : Form
    {
        private string _personId;
        private DateTime? _fechaNacimiento;
        private int? _sexTypeId;
        private string _fileName;
        private string _filePath;
        private string _OrganizationEmployerId;
        private string _LocationEmployerId;
        string Mode;
        private string _modoPre;
        private string _protocolIdNew;
        private string _dni;
        private string _empresa;
        private string _contrata;
        private int _tipoAtencion;
        public int cierre = 0;
        bool loadd = false;
        private AgendaBl agendaBl_ = new AgendaBl();
        private EmpresaBl empresaBl_= new EmpresaBl();
        List<EsoDtoProt> listaProtoColos = new List<EsoDtoProt>();

        #region Properties


        public byte[] FingerPrintTemplate { get; set; }

        public byte[] FingerPrintImage { get; set; }

        public byte[] RubricImage { get; set; }

        public string RubricImageText { get; set; }

        #endregion

        public frmAgendarTrabajador(string modoPre, string dni, string empresa, int tipoAtencion, string contrata)
        {
            _modoPre = modoPre;
            _dni = dni;
            _empresa = empresa;
            _contrata = contrata;
            _tipoAtencion = tipoAtencion;
            InitializeComponent();
        }

        private void frmAgendarTrabajador_Load(object sender, EventArgs e)
        {
            try
            {
                //using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                //{
                    //ARNOLD
                    bool validar = false;
                    List<EsoDto> Esodto_1 = new List<EsoDto>();
                    List<EsoDto> Esodto_2 = new List<EsoDto>();
                    List<EsoDto> Esodto_3 = new List<EsoDto>();
                    List<EsoDto> Esodto_4 = new List<EsoDto>();
                    List<EsoDto> Esodto_5 = new List<EsoDto>();
                    List<EsoDto> Esodto_6 = new List<EsoDto>();
                    List<EsoDto> Esodto_7 = new List<EsoDto>();
                    List<EsoDto> Esodto_8 = new List<EsoDto>();
                    List<EsoDto> Esodto_9 = new List<EsoDto>();
                    List<EsoDto> Esodto_10 = new List<EsoDto>();
                    List<EsoDto> Esodto_11 = new List<EsoDto>();
                    List<EsoDto> Esodto_12 = new List<EsoDto>();
                    List<EsoDto> Esodto_13 = new List<EsoDto>();
                    List<EsoDto> Esodto_14 = new List<EsoDto>();
                    List<EsoDto> Esodto_15 = new List<EsoDto>();
                    List<EsoDto> Esodto_16 = new List<EsoDto>();
                    List<EsoDto> Esodto_17 = new List<EsoDto>();
                    List<EsoDto> Esodto_18 = new List<EsoDto>();
                    List<EsoDto> Esodto_19 = new List<EsoDto>();

                    List<KeyValueDTO> KeyValueDTO_1 = new List<KeyValueDTO>();
                    List<KeyValueDTO> KeyValueDTO_2 = new List<KeyValueDTO>();
                    List<KeyValueDTO> KeyValueDTO_3 = new List<KeyValueDTO>();
                    List<KeyValueDTO> KeyValueDTO_4 = new List<KeyValueDTO>();

                    List<KeyValueDTO> KeyValueDTO_5 = new List<KeyValueDTO>();

                    List<AgendaBl.PuestoList> PuestoList_ = new List<AgendaBl.PuestoList>();

                    List<AgendaBl.NacionalidadList> NacionalidadList_ = new List<AgendaBl.NacionalidadList>();

                    List<AgendaBl.PaisOrigenList> PaisOrigenList_ = new List<AgendaBl.PaisOrigenList>();

                    List<AgendaBl.AreaList> AreaList_ = new List<AgendaBl.AreaList>();

                    List<AgendaBl.ContactoEmergenciaList> ContactoEmergenciaList_ = new List<AgendaBl.ContactoEmergenciaList>();

                    List<PacientesList> PacientesList_ = new List<PacientesList>();

                    List<AgendaBl.CCostoList> CCostoList_ = new List<AgendaBl.CCostoList>();

                    Task.Factory.StartNew(() =>
                    {
                            Esodto_1 = agendaBl_.LlenarComboTipoDocumento(cboTipoDocumento);
                            Esodto_2 = agendaBl_.LlenarComboGennero(cboGenero);
                            Esodto_3 = agendaBl_.LlenarComboEstadoCivil(cboEstadoCivil);
                            Esodto_4 = agendaBl_.LlenarComboTipoServicio(cboTipoServicio);
                            Esodto_5 = agendaBl_.LlenarComboNivelEstudio(cboNivelEstudio);
                            Esodto_6 = agendaBl_.LlenarComboResidencia(cboResidencia);
                            Esodto_7 = agendaBl_.LlenarComboAltitud(cboAltitud);
                            Esodto_8 = agendaBl_.LlenarComboTipoSeguro(cboTipoSeguro);
                            Esodto_9 = agendaBl_.LlenarComboParentesco(cboParentesco);
                            Esodto_10 = agendaBl_.LlenarComboLugarLabor(cboLugarLabor);
                            Esodto_11 = agendaBl_.LlenarComboDistrito(cboDistrito);
                            Esodto_12 = agendaBl_.LlenarComboDistrito(cboProvincia);
                            Esodto_13 = agendaBl_.LlenarComboDistrito(cboDepartamento);
                            Esodto_14 = agendaBl_.LlenarComboGrupo(cboGrupo);
                            Esodto_15 = agendaBl_.LlenarComboFactor(cboFactorSan);
                            Esodto_16 = agendaBl_.LlenarComboEtnia(cboEtniaRaza);
                            Esodto_17 = agendaBl_.LlenarComboResidencia(cboMigrante);
                            Esodto_18 = agendaBl_.LlenarModalidadTrabajo(cboModalidadTrabajo);
                            Esodto_19 = agendaBl_.LlenarComboMarketing(cboMarketing);

                            KeyValueDTO_1 = empresaBl_.GetOrganizationFacturacion(cboEmpresaFacturacion, 9);
                            KeyValueDTO_2 = empresaBl_.GetJoinOrganizationAndLocation(cboEmpresaEmpleadora, 9);
                            KeyValueDTO_3 = empresaBl_.GetJoinOrganizationAndLocation(cboEmpresaCliente, 9);
                            KeyValueDTO_4 = empresaBl_.GetJoinOrganizationAndLocation(cboEmpresaTrabajo, 9);

                            KeyValueDTO_5 = new AgendaBl().LlenarComboUsuarios(cboMedicoTratante);

                            PuestoList_ = AgendaBl.ObtenerPuestos();
                            NacionalidadList_ = AgendaBl.ObtenerNacionalidad();
                            PaisOrigenList_ = AgendaBl.ObtenerPaisOrigen();
                            AreaList_ = AgendaBl.ObtenerAreas();
                            ContactoEmergenciaList_ = AgendaBl.ObtenerContactoEmergencia();
                            PacientesList_ = AgendaBl.ObtenerPacientes();
                            CCostoList_ = AgendaBl.ObtenerCC();
                        
                    }).ContinueWith(t =>
                    {
                        using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                        {
                            #region
                            if (Esodto_1 == null) return;
                            else
                            {
                                cboTipoDocumento.DataSource = Esodto_1;
                                //cboTipoDocumento.SelectedIndex = 0;
                                //
                                cboTipoDocumento.SelectedValue = 1;

                            }

                            if (Esodto_2 == null) return;
                            else
                            {
                                cboGenero.DataSource = Esodto_2;
                                //cboGenero.SelectedIndex = 0;
                                //
                                cboGenero.SelectedValue = 1;

                            }

                            if (Esodto_3 == null) return;
                            else
                            {
                                cboEstadoCivil.DataSource = Esodto_3;
                                //cboEstadoCivil.SelectedIndex = 0;
                                //
                                cboEstadoCivil.SelectedValue = 1;

                            }

                            if (Esodto_4 == null) return;
                            else
                            {
                                cboTipoServicio.DataSource = Esodto_4;
                                //cboTipoServicio.SelectedIndex = 1;
                                //
                                cboTipoServicio.SelectedValue = 1;
                                cboServicio.SelectedValue = 2;
                            }

                            if (Esodto_5 == null) return;
                            else
                            {
                                cboNivelEstudio.DataSource = Esodto_5;
                                //cboNivelEstudio.SelectedIndex = 0;
                                //
                                cboNivelEstudio.SelectedValue = 5;

                            }

                            if (Esodto_6 == null) return;
                            else
                            {
                                cboResidencia.DataSource = Esodto_6;
                                //cboResidencia.SelectedIndex = 0;
                                //
                                cboResidencia.SelectedValue = 0;

                            }

                            if (Esodto_7 == null) return;
                            else
                            {
                                cboAltitud.DataSource = Esodto_7;
                                cboAltitud.SelectedIndex = 0;
                            }

                            if (Esodto_8 == null) return;
                            else
                            {
                                cboTipoSeguro.DataSource = Esodto_8;
                                //cboTipoSeguro.SelectedIndex = 0;
                                //
                                cboTipoSeguro.SelectedValue = -1;

                            }

                            if (Esodto_9 == null) return;
                            else
                            {
                                cboParentesco.DataSource = Esodto_9;
                                cboParentesco.SelectedIndex = -1;
                            }

                            if (Esodto_10 == null) return;
                            else
                            {
                                cboLugarLabor.DataSource = Esodto_10;
                                cboLugarLabor.SelectedIndex = 0;
                            }

                            if (Esodto_11 == null) return;
                            else
                            {
                                cboDistrito.DataSource = Esodto_11;
                                cboDistrito.SelectedIndex = 0;
                            }

                            if (Esodto_12 == null) return;
                            else
                            {
                                cboProvincia.DataSource = Esodto_12;
                                cboProvincia.SelectedIndex = 0;
                            }

                            if (Esodto_13 == null) return;
                            else
                            {
                                cboDepartamento.DataSource = Esodto_13;
                                cboDepartamento.SelectedIndex = 0;
                            }

                            if (Esodto_14 == null) return;
                            else
                            {
                                cboGrupo.DataSource = Esodto_14;
                                cboGrupo.SelectedIndex = 0;
                            }

                            if (Esodto_15 == null) return;
                            else
                            {
                                cboFactorSan.DataSource = Esodto_15;
                                cboFactorSan.SelectedIndex = 0;
                            }

                            if (Esodto_16 == null) return;
                            else
                            {
                                cboEtniaRaza.DataSource = Esodto_16;
                                cboEtniaRaza.SelectedIndex = 0;
                            }

                            if (Esodto_17 == null) return;
                            else
                            {
                                cboMigrante.DataSource = Esodto_17;
                                cboMigrante.SelectedValue = -1;

                            }

                            if (Esodto_18 == null) return;
                            else
                            {
                                cboModalidadTrabajo.DataSource = Esodto_18;
                                cboModalidadTrabajo.SelectedValue = -1;

                            }

                            if (Esodto_19 == null) return;
                            else
                            {
                                cboMarketing.DataSource = Esodto_19;
                                cboMarketing.SelectedValue = -1;

                            }

                            if (KeyValueDTO_1 == null) return;
                            else
                            {
                                cboEmpresaFacturacion.DataSource = KeyValueDTO_1;
                                cboEmpresaFacturacion.SelectedIndex = 0;
                            }

                            if (KeyValueDTO_2 == null) return;
                            else
                            {
                                cboEmpresaEmpleadora.DataSource = KeyValueDTO_2;
                                cboEmpresaEmpleadora.SelectedIndex = 0;
                            }

                            if (KeyValueDTO_3 == null) return;
                            else
                            {
                                cboEmpresaCliente.DataSource = KeyValueDTO_3;
                                cboEmpresaCliente.SelectedIndex = 0;
                            }

                            if (KeyValueDTO_4 == null) return;
                            else
                            {
                                cboEmpresaTrabajo.DataSource = KeyValueDTO_4;
                                cboEmpresaTrabajo.SelectedIndex = 0;
                            }
                            //CargaCombos(); 

                            if (KeyValueDTO_5 == null) return;
                            else
                            {
                                cboMedicoTratante.DataSource = KeyValueDTO_5;
                                cboMedicoTratante.SelectedIndex = 0;
                            }
                            #endregion
                            LimpiarControlesProtocolo();

                            if (PuestoList_ == null) return;
                            else
                            {
                                var lista = PuestoList_;
                                txtPuesto.DataSource = lista;
                                txtPuesto.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;

                                txtPuesto.DisplayLayout.Bands[0].Columns[0].Width = 10;
                                txtPuesto.DisplayLayout.Bands[0].Columns[1].Width = 250;
                                if (!string.IsNullOrEmpty("")) { txtPuesto.Value = ""; }
                            }

                            if (NacionalidadList_ == null) return;
                            else
                            {
                                var listaNacionalidad = NacionalidadList_;
                                txtNacionalidad.DataSource = listaNacionalidad;
                                txtNacionalidad.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;

                                txtNacionalidad.DisplayLayout.Bands[0].Columns[0].Width = 10;
                                txtNacionalidad.DisplayLayout.Bands[0].Columns[1].Width = 250;
                                if (!string.IsNullOrEmpty("")) { txtNacionalidad.Value = ""; }
                            }

                            if (PaisOrigenList_ == null) return;
                            else
                            {
                                var listaPaisOrigen = PaisOrigenList_;
                                txtPaisOrigen.DataSource = listaPaisOrigen;
                                txtPaisOrigen.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;

                                txtPaisOrigen.DisplayLayout.Bands[0].Columns[0].Width = 10;
                                txtPaisOrigen.DisplayLayout.Bands[0].Columns[1].Width = 250;
                                if (!string.IsNullOrEmpty("")) { txtPaisOrigen.Value = ""; }
                            }

                            if (AreaList_ == null) return;
                            else
                            {

                                var listaAreas = AreaList_;
                                txtArea.DataSource = listaAreas;
                                txtArea.DisplayMember = "Area";
                                txtArea.ValueMember = "Area";
                                txtArea.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                                txtArea.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
                                this.txtArea.DropDownWidth = 250;
                                txtArea.DisplayLayout.Bands[0].Columns[0].Width = 10;
                                txtArea.DisplayLayout.Bands[0].Columns[1].Width = 250;
                                if (!string.IsNullOrEmpty("")) { txtArea.Value = ""; }
                            }

                            if (ContactoEmergenciaList_ == null) return;
                            else
                            {
                                var lista_ContactEmergency = ContactoEmergenciaList_;
                                txtContactEmergency.DataSource = lista_ContactEmergency;
                                txtContactEmergency.DisplayMember = "ContactoEmergencia";
                                txtContactEmergency.ValueMember = "ContactoEmergenciaId";
                                txtContactEmergency.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                                txtContactEmergency.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
                                this.txtContactEmergency.DropDownWidth = 250;
                                txtContactEmergency.DisplayLayout.Bands[0].Columns[0].Width = 10;
                                txtContactEmergency.DisplayLayout.Bands[0].Columns[1].Width = 300;
                                if (!string.IsNullOrEmpty("")) { txtContactEmergency.Value = ""; }
                            }

                            if (PacientesList_ == null) return;
                            else
                            {
                                var listaPaciente = PacientesList_;
                                txtNombreTitular.DataSource = listaPaciente;
                                txtNombreTitular.DisplayMember = "v_name";
                                txtNombreTitular.ValueMember = "v_personId";
                                txtNombreTitular.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                                txtNombreTitular.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
                                this.txtNombreTitular.DropDownWidth = 550;
                                txtNombreTitular.DisplayLayout.Bands[0].Columns[0].Width = 20;
                                txtNombreTitular.DisplayLayout.Bands[0].Columns[1].Width = 400;
                            }

                            if (CCostoList_ == null) return;
                            else
                            {
                                var listaCCo = CCostoList_;
                                txtCCosto.DataSource = listaCCo;
                                txtCCosto.DisplayMember = "Costo";
                                txtCCosto.ValueMember = "Costo";
                                txtCCosto.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                                txtCCosto.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
                                this.txtCCosto.DropDownWidth = 250;
                                txtCCosto.DisplayLayout.Bands[0].Columns[0].Width = 10;
                                txtCCosto.DisplayLayout.Bands[0].Columns[1].Width = 250;
                                if (!string.IsNullOrEmpty("")) { txtCCosto.Value = ""; }
                            }
                            //if (!ListadoVentaDetalle.Any()) return;
                            //DialogResult = DialogResult.OK;
                            //Close();
                        };
                    }, TaskScheduler.FromCurrentSynchronizationContext());

                //};
                Mode = "New";
                if (_modoPre == "BUSCAR")
                {
                    var datosTrabajador = AgendaBl.GetDatosTrabajador(_dni);
                    if (datosTrabajador != null)
                    {
                        Mode = "Edit";
                        LlenarCampos(datosTrabajador);
                        _sexTypeId = datosTrabajador.GeneroId;
                        _fechaNacimiento = datosTrabajador.FechaNacimiento;
                    }
                    else
                    {
                        //ObtenerDatosDNI(_dni);
                        //txtNroDocumento.Text = _dni;
                        txtSearchNroDocument.Text = _dni;

                        btnBuscarTrabajador_Click(sender, e);
                    }

                    AgendaBl.LlenarComboProtocolo_pre(cboProtocolo, 1, 2, _empresa, _contrata);
                }
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private bool CargaCombos()
        {

            cboTipoDocumento.DataSource = agendaBl_.LlenarComboTipoDocumento(cboTipoDocumento);
            //cboTipoDocumento.SelectedIndex = 0;

            cboGenero.DataSource = agendaBl_.LlenarComboGennero(cboGenero);
            //cboGenero.SelectedIndex = 0;

            cboEstadoCivil.DataSource = agendaBl_.LlenarComboEstadoCivil(cboEstadoCivil);
            //cboEstadoCivil.SelectedIndex = 0;

            cboTipoServicio.DataSource = agendaBl_.LlenarComboTipoServicio(cboTipoServicio);
            //cboTipoServicio.SelectedIndex = 1;

            cboNivelEstudio.DataSource = agendaBl_.LlenarComboNivelEstudio(cboNivelEstudio);
            //cboNivelEstudio.SelectedIndex = 0;

            cboResidencia.DataSource = agendaBl_.LlenarComboResidencia(cboResidencia);

            cboMigrante.DataSource = agendaBl_.LlenarComboResidencia(cboMigrante);

            //cboResidencia.SelectedIndex = 0;

            cboAltitud.DataSource = agendaBl_.LlenarComboAltitud(cboAltitud);
            //cboAltitud.SelectedIndex = 0;

            cboTipoSeguro.DataSource = agendaBl_.LlenarComboTipoSeguro(cboTipoSeguro);
            //cboTipoSeguro.SelectedIndex = 0;

            cboParentesco.DataSource = agendaBl_.LlenarComboParentesco(cboParentesco);
            //cboParentesco.SelectedIndex = -1;

            cboLugarLabor.DataSource = agendaBl_.LlenarComboLugarLabor(cboLugarLabor);
            //cboLugarLabor.SelectedIndex = 0;

            cboDistrito.DataSource = agendaBl_.LlenarComboDistrito(cboDistrito);
            //cboDistrito.SelectedIndex = 0;

            cboProvincia.DataSource = agendaBl_.LlenarComboDistrito(cboProvincia);
            //cboProvincia.SelectedIndex = 0;

            cboDepartamento.DataSource = agendaBl_.LlenarComboDistrito(cboDepartamento);
            //cboDepartamento.SelectedIndex = 0;

            cboGrupo.DataSource = agendaBl_.LlenarComboGrupo(cboGrupo);
            //cboGrupo.SelectedIndex = 0;

            cboFactorSan.DataSource = agendaBl_.LlenarComboFactor(cboFactorSan);
            //cboFactorSan.SelectedIndex = 0;

            cboEmpresaFacturacion.DataSource = empresaBl_.GetOrganizationFacturacion(cboEmpresaFacturacion, 9);
            //cboEmpresaFacturacion.SelectedIndex = 0;
            //cboEmpresaFacturacion.SelectedIndex = 0;

            cboEmpresaEmpleadora.DataSource = empresaBl_.GetJoinOrganizationAndLocation(cboEmpresaEmpleadora, 9);
            //cboEmpresaEmpleadora.SelectedIndex = 1;
            cboEmpresaCliente.DataSource = empresaBl_.GetJoinOrganizationAndLocation(cboEmpresaCliente, 9);
            //cboEmpresaCliente.SelectedIndex = 1;
            cboEmpresaTrabajo.DataSource = empresaBl_.GetJoinOrganizationAndLocation(cboEmpresaTrabajo, 9);
            //cboEmpresaTrabajo.SelectedIndex = 1;

            return true;
            //AgendaBl.LlenarComboTipoDocumento(cboTipoDocumento)
            //AgendaBl.LlenarComboGennero(cboGenero);
            //AgendaBl.LlenarComboEstadoCivil(cboEstadoCivil);
            //AgendaBl.LlenarComboTipoServicio(cboTipoServicio);
            //AgendaBl.LlenarComboNivelEstudio(cboNivelEstudio);
            //AgendaBl.LlenarComboResidencia(cboResidencia);
            //AgendaBl.LlenarComboAltitud(cboAltitud);
            //AgendaBl.LlenarComboTipoSeguro(cboTipoSeguro);
            //AgendaBl.LlenarComboParentesco(cboParentesco);
            //AgendaBl.LlenarComboLugarLabor(cboLugarLabor);
            //AgendaBl.LlenarComboDistrito(cboDistrito);
            //AgendaBl.LlenarComboDistrito(cboProvincia);
            //AgendaBl.LlenarComboDistrito(cboDepartamento);
            //AgendaBl.LlenarComboGrupo(cboGrupo);
            //AgendaBl.LlenarComboFactor(cboFactorSan);
            //EmpresaBl.GetOrganizationFacturacion(cboEmpresaFacturacion, 9);
            //EmpresaBl.GetJoinOrganizationAndLocation(cboEmpresaEmpleadora, 9);
            //EmpresaBl.GetJoinOrganizationAndLocation(cboEmpresaCliente, 9);
            //EmpresaBl.GetJoinOrganizationAndLocation(cboEmpresaTrabajo, 9);
        }

        private void btnschedule_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboModalidadTrabajo.SelectedValue.ToString() == "-1")
                {
                    MessageBox.Show(@"Debe Seleccionar Modalidad de Trabajo", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboMarketing.SelectedValue.ToString() == "-1")
                {
                    MessageBox.Show("Tiene que seleccionar procedencia de paciente para atención.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtMail.Text == string.Empty && txtMail.Text == "")
                {
                    MessageBox.Show("Registre correctamente correo de paciente.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtContactEmergency.Text == string.Empty && txtContactEmergency.Text == "")
                {
                    MessageBox.Show("Registre correctamente contacto de emergencia de paciente.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtPhoneEmergency.Text == string.Empty && txtPhoneEmergency.Text == "")
                {
                    MessageBox.Show("Registre correctamente celular de emergencia de paciente.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtTelephoneNumber.Text == string.Empty && txtTelephoneNumber.Text == "")
                {
                    MessageBox.Show("Registre correctamente celular de paciente.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            if (cboEmpresaFacturacion.SelectedValue.ToString() == "-1")
            {
                MessageBox.Show("Tiene que seleccionar empresa de Facturación.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(_personId))
            {
                MessageBox.Show("Tiene que grabar al paciente antes de agendar.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cboProtocolo.SelectedValue == "-1")
            {
                MessageBox.Show("Tiene que seleccionar un protocolo antes de agendar.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboTipoServicio.SelectedValue.ToString() == "9")
            {
                if (cboMedicoTratante.SelectedValue.ToString() == "-1")
                {
                    MessageBox.Show(@"Tiene que seleccionar un MÉDICO TRATANTE.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (cboTipoServicio.SelectedValue.ToString() == "11")
            {
                if (cboParentesco.SelectedValue.ToString() == "-1")
                {
                    MessageBox.Show(@"Tiene que seleccionar un parentesco del titular.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtNombreTitular.Text == "")
                {
                    MessageBox.Show(@"Tiene que registrar un titular.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

             List<ProtocolComponentList> _protocolcomponentListDTO = null;

             var id = cboEmpresaEmpleadora.SelectedValue.ToString().Split('|');
             var id1 = cboEmpresaCliente.SelectedValue.ToString().Split('|');
             var id2 = cboEmpresaTrabajo.SelectedValue.ToString().Split('|');


            //Verificar si existe el protocolo propuesto
            int tipoServicioId = int.Parse(cboTipoServicio.SelectedValue.ToString());
            int servicioId = int.Parse(cboServicio.SelectedValue.ToString());
            var geso = cboGeso.Text.ToString();
            int tipoEsoId = int.Parse(cboTipoEso.SelectedValue.ToString());
            var protocolId = cboProtocolo.SelectedValue.ToString();

                //ARNOLD NEW STORES
            var result = ProtocoloBl.BuscarProtocoloPropuesto(id[0], tipoServicioId, servicioId, geso, tipoEsoId);
            if (result)
            {

            }
            else
            {
                var dataListPc = ProtocoloBl.GetProtocolComponents(protocolId);
                ProtocolDto oProtocolDto = new ProtocolDto();

                var sufProtocol = cboEmpresaEmpleadora.Text.Split('/');
                oProtocolDto.v_Name = cboProtocolo.Text + " " + sufProtocol[0].ToString();
                oProtocolDto.v_EmployerOrganizationId = id[0];
                oProtocolDto.v_EmployerLocationId = id[1];
                oProtocolDto.i_EsoTypeId = int.Parse(cboTipoEso.SelectedValue.ToString());
                //obtener GESO
                var gesoId = EmpresaBl.ObtenerGesoId(id[1], geso);
                oProtocolDto.v_GroupOccupationId = gesoId;
                oProtocolDto.v_CustomerOrganizationId = id1[0];
                oProtocolDto.v_CustomerLocationId = id1[1];
                oProtocolDto.v_WorkingOrganizationId = id2[0];
                oProtocolDto.v_WorkingLocationId = cboEmpresaEmpleadora.SelectedValue.ToString() != "-1" ? id2[1] : "-1";
                oProtocolDto.i_MasterServiceId = int.Parse(cboServicio.SelectedValue.ToString());
                oProtocolDto.v_CostCenter = string.Empty;
                oProtocolDto.i_MasterServiceTypeId = int.Parse(cboTipoServicio.SelectedValue.ToString());
                oProtocolDto.i_HasVigency = 1;
                oProtocolDto.i_ValidInDays = (int?)null;
                oProtocolDto.i_IsActive = 1;
                oProtocolDto.v_NombreVendedor = string.Empty;

                _protocolcomponentListDTO = new List<ProtocolComponentList>();
                foreach (var item in dataListPc)
                {
                    ProtocolComponentList protocolComponent = new ProtocolComponentList();

                    protocolComponent.ComponentId = item.ComponentId;
                    protocolComponent.Price = item.Price;
                    protocolComponent.Operator = item.Operator;
                    protocolComponent.Age = item.Age;
                    protocolComponent.Gender = item.Gender;
                    protocolComponent.IsAdditional = item.IsAdditional;
                    protocolComponent.IsConditionalId = item.IsConditionalId;
                    protocolComponent.GrupoEtarioId = item.GrupoEtarioId;
                    protocolComponent.IsConditionalImc = item.IsConditionalImc;
                    protocolComponent.Imc = item.Imc;

                    _protocolcomponentListDTO.Add(protocolComponent);
                }


                _protocolIdNew = new  ProtocoloBl().AddProtocol(oProtocolDto, _protocolcomponentListDTO);

                cboProtocolo.DataSource = new AgendaBl().LlenarComboProtocolo(cboProtocolo, int.Parse(cboTipoServicio.SelectedValue.ToString()), int.Parse(cboServicio.SelectedValue.ToString()));

                listaProtoColos = new AgendaBl().LlenarComboProtocolo(cboProtocolo, int.Parse(cboTipoServicio.SelectedValue.ToString()), int.Parse(cboServicio.SelectedValue.ToString()));


                cboProtocolo.SelectedValue = _protocolIdNew;
            }


            //var scope = new TransactionScope(
            //  TransactionScopeOption.RequiresNew,
            //              new TransactionOptions()
            //              {

            //                  IsolationLevel = System.Transactions.IsolationLevel.Snapshot
            //              });


            //using (scope)
            //{
                AgendarServicio(Globals.ClientSession.GetAsList());
            //}

           

           var resp = MessageBox.Show("Se agendó correctamente.", "CONFIRMACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
           if (resp == DialogResult.OK)
            {
                this.Close();
            }
           else
           {
               this.Close();
           }

           cierre = 1;
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        public BindingList<ventadetalleDto> ListadoVentaDetalle = new BindingList<ventadetalleDto>();
        private void AgendarServicio(List<string> ClientSession)
        {
            try
            {
                var oServiceDto = OServiceDto();
                if (cboTipoServicio.SelectedValue.ToString() == "1")
                {
                    AgendaBl.SheduleService(oServiceDto, Int32.Parse(ClientSession[2]));

                }
                else
                {
                    AgendaBl.SheduleServiceAtx(oServiceDto, Int32.Parse(ClientSession[2]), 1, 0);
                }
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            

           
        }

        private void limpiarFormulario()
        {
            try
            {
                txtSearchNroDocument.Text = "";

            txtNombres.Text  = "";
            cboTipoDocumento.SelectedValue = 1;
            txtNroDocumento.Text  = "";
            txtApellidoPaterno.Text  = "";
            txtApellidoMaterno.Text  = "";
            cboGenero.SelectedValue = 1;
            dtpBirthdate.Value = DateTime.Now;

            cboEstadoCivil.SelectedValue = 1;
            txtBirthPlace.Text  = "";
            cboDistrito.SelectedValue = -1; 
            cboProvincia.SelectedValue = -1;
            cboDepartamento.SelectedValue = -1;
            cboResidencia.SelectedValue = 0;
            txtMail.Text  = "";
            txtAdressLocation.Text  = "";;
            txtPuesto.Text  = "";
            txtPaisOrigen.Text = "";
            cboMigrante.SelectedValue = -1;
            cboEtniaRaza.SelectedValue = -1;
            txtArea.Text = "";
            txtContactEmergency.Text = "";
            txtPhoneEmergency.Text = ""; 
            txtCCosto.Text = "";
            cboAltitud.SelectedValue = -1;
            txtExploitedMineral.Text  = "";
            cboEmpresaFacturacion.SelectedValue = -1;
            cboNivelEstudio.SelectedValue = 5;
            cboGrupo.SelectedValue = -1;
            cboFactorSan.SelectedValue = -1;
            txtResidenceTimeInWorkplace.Text  = "";
            cboTipoSeguro.SelectedValue = 1;
            txtNumberLivingChildren.Text  = "";
            txtNumberDependentChildren.Text  = "";
            txtNroHermanos.Text  = "";
            txtTelephoneNumber.Text  = "";;
            cboParentesco.SelectedValue = -1; 
            cboLugarLabor.SelectedValue = -1;
            pbPersonImage.Image = SAMBHS.Windows.SigesoftIntegration.UI.Properties.Resources.usuario;
            txtResidenciaAnte.Text = "";
            txtNacionalidad.Text = "";
            txtReligion.Text = "";

            cboTipoServicio.SelectedValue = 1;

            cboServicio.SelectedValue = 2;
            cboProtocolo.SelectedValue = -1;
            cboGeso.SelectedValue = -1;
            cboTipoEso.SelectedValue = -1;
            cboMedicoTratante.SelectedValue = -1;
            //cboEmpresaEmpleadora.SelectedValue = -1;
            //cboEmpresaCliente.SelectedValue = -1;
            //cboEmpresaTrabajo.SelectedValue = -1;
            txtRucCliente.Text = "";
            cboEmpresaEmpleadora.DataSource = new EmpresaBl().GetJoinOrganizationAndLocation(cboEmpresaEmpleadora, 9);
            cboEmpresaEmpleadora.SelectedIndex = 1;
                
                cboEmpresaCliente.DataSource = new EmpresaBl().GetJoinOrganizationAndLocation(cboEmpresaCliente, 9);
                cboEmpresaCliente.SelectedIndex = 1;
                
                cboEmpresaTrabajo.DataSource = new EmpresaBl().GetJoinOrganizationAndLocation(cboEmpresaTrabajo, 9);
                cboEmpresaTrabajo.SelectedIndex = 1;
                //ARNOLD NEW STORES
                cboMedicoTratante.DataSource = new AgendaBl().LlenarComboUsuarios(cboMedicoTratante);
                cboMedicoTratante.SelectedIndex = 1;

            txtSearchNroDocument.Focus();
            //oPersonDto.b_FingerPrintTemplate = FingerPrintTemplate;
            //oPersonDto.b_FingerPrintImage = FingerPrintImage;
            //oPersonDto.b_RubricImage = RubricImage;
            //oPersonDto.t_RubricImageText = RubricImageText;
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private ServiceDto OServiceDto()
        {
            try
            {
                int medico = -1;
                if (cboMedicoTratante.SelectedValue != null)
                {
                    medico = int.Parse(cboMedicoTratante.SelectedValue.ToString());
                }
                var oServiceDto = new ServiceDto
                {
                    ProtocolId = cboProtocolo.SelectedValue.ToString(),
                    OrganizationId = cboEmpresaFacturacion.SelectedValue.ToString(),
                    PersonId = _personId,
                    MasterServiceId = int.Parse(cboServicio.SelectedValue.ToString()),
                    ServiceStatusId = (int)ServiceStatus.Iniciado,
                    AptitudeStatusId = (int)AptitudeStatus.SinAptitud,
                    ServiceDate = null,
                    GlobalExpirationDate = null,
                    ObsExpirationDate = null,
                    FlagAgentId = 1,
                    Motive = string.Empty,
                    Area = txtArea.Text,
                    IsFac = 0,
                    FechaNacimiento = _fechaNacimiento,
                    GeneroId = Convert.ToInt32(_sexTypeId),
                    MedicoTratanteId = medico,
                    v_centrocosto = txtCCosto.Text,
                    ObservacionesAtencion = txtDetalleAtencion.Text,
                    i_ModTrabajo = cboModalidadTrabajo.SelectedValue == null ? -1 : int.Parse(cboModalidadTrabajo.SelectedValue.ToString()),
                    i_ProcedenciaPac_Mkt = cboMarketing.SelectedValue == null ? -1 : int.Parse(cboMarketing.SelectedValue.ToString())
                };
                return oServiceDto;
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ObtenerDatosDNI(string dni)
        {
            try
            {
                var f = new frmBuscarDatos(dni);
            if (f.ConexionDisponible)
            {
                f.ShowDialog();
                switch (f.Estado)
                {
                    case Estado.NoResul:
                        MessageBox.Show("No se encontró datos de el DNI");
                        break;

                    case Estado.Ok:
                        if (f.Datos != null)
                        {
                            if (!f.EsContribuyente)
                            {
                                var datos = (ReniecResultDto)f.Datos;
                                txtNroDocumento.Text = txtSearchNroDocument.Text.Trim();
                                txtNombres.Text = datos.Nombre.Trim();
                                txtApellidoPaterno.Text = datos.ApellidoPaterno.Trim();
                                txtApellidoMaterno.Text = datos.ApellidoMaterno.Trim();
                                dtpBirthdate.Value = datos.FechaNacimiento;
                                _personId = null;
                            }
                        }
                        break;
                }

                Mode = "New";
            }
            else
                MessageBox.Show("No se pudo conectar la página", "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception a)
            {
                 MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void txtSearchNroDocument_TextChanged(object sender, EventArgs e)
        {
            try
            {
                btnBuscarTrabajador.Enabled = (txtSearchNroDocument.TextLength > 0);
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }
        
        private void txtSearchNroDocument_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
                if (Char.IsControl(e.KeyChar)) //permitir teclas de control como retroceso
                {
                    e.Handled = false;
                }
                else
                {
                    //el resto de teclas pulsadas se desactivan
                    e.Handled = true;
                }
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                btnBuscarTrabajador_Click(sender, e);
            }
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void btnBuscarTrabajador_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSearchNroDocument.Text == "" || txtSearchNroDocument.Text.Length != 8)
                {
                    if (txtSearchNroDocument.Text == "")
                    {
                        MessageBox.Show(@"No hay nro. de documento para buscar", @"Información");
                        return;
                    }
                    //else if (txtSearchNroDocument.Text.Length != 8)
                    //{
                    //    MessageBox.Show(@"Nro. de dígitos incorrecto (DNI=8 dígitos)", @"Información");
                    //}
                   
                }
                if (Mode == "New" || _personId != null)
                {
                    LimpiarDatos();
                    btnSavePacient.Enabled = true;
                }
                var datosTrabajador = AgendaBl.GetDatosTrabajador(txtSearchNroDocument.Text);
                if (datosTrabajador != null)
                {
                    Mode = "Edit";
                    LlenarCampos(datosTrabajador);
                    _sexTypeId = datosTrabajador.GeneroId;
                    _fechaNacimiento = datosTrabajador.FechaNacimiento;
                }
                else
                {
                    try
                    {
                        string _ApiDni = GetApplicationConfigValue("ApiDni").ToString();

                        var urlReniec = "https://dniruc.apisperu.com/api/v1/dni/" + txtSearchNroDocument.Text.Trim() + _ApiDni;
                        System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

                        System.Net.WebClient wcReniec = new System.Net.WebClient();
                        wcReniec.Encoding = System.Text.Encoding.UTF8;
                        string DataReniecString = wcReniec.DownloadString(urlReniec);
                        AgendaBl.DataReniec DataReniec = JsonConvert.DeserializeObject<AgendaBl.DataReniec>(DataReniecString);


                        if (DataReniec.Success)
                        {
                            txtNombres.Text = DataReniec.Nombres;
                            txtApellidoPaterno.Text = DataReniec.ApellidoPaterno;
                            txtApellidoMaterno.Text = DataReniec.ApellidoMaterno;
                            txtNroDocumento.Text = txtSearchNroDocument.Text.Trim();
                            _personId = null;
                        }
                        else
                        {
                            MessageBox.Show(@"Nro. de Documento no se encontró en la Api 'dniruc.apisperu.com'", @"Información");
                        }

                    }
                    catch (Exception)
                    {
                        MessageBox.Show(@"Nro. de DNI incorrecto", @"Información");
                        //throw;
                    }
               

                }
                //TrabajadorNoEncontrado(txtSearchNroDocument.Text);
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void LimpiarDatos()
        {
            try
            {
                txtNombres.Text = "";
                txtApellidoPaterno.Text = "";
                txtApellidoMaterno.Text = "";
                cboTipoDocumento.SelectedValue = 1;
                txtNroDocumento.Text = "";
                cboGenero.SelectedValue = 1;
                dtpBirthdate.Value = DateTime.Now;
                cboEstadoCivil.SelectedValue = 1;
                txtBirthPlace.Text = "";
                cboDistrito.SelectedValue = -1;
                cboProvincia.SelectedValue = -1;
                cboDepartamento.SelectedValue = -1;
                cboResidencia.SelectedValue = 0;
                txtMail.Text = "";
                txtAdressLocation.Text = "";
                txtPuesto.Value = null;
                txtArea.Value = null; ;
                cboAltitud.SelectedValue = -1;
                txtExploitedMineral.Text = "";
                cboNivelEstudio.SelectedValue = 5;
                cboGrupo.SelectedValue = -1;
                cboFactorSan.SelectedValue = -1;
                txtResidenceTimeInWorkplace.Text = " - - -";
                cboTipoSeguro.SelectedValue = 1;
                txtNumberLivingChildren.Text = "0";
                txtNumberDependentChildren.Text = "0";
                txtNroHermanos.Text = "0";
                txtTelephoneNumber.Text = "";
                cboParentesco.SelectedValue = -1;
                cboLugarLabor.SelectedValue = -1;
                txtResidenciaAnte.Text = "";
                txtNacionalidad.Text = "";
                txtReligion.Text = "";
                FingerPrintTemplate = null;
                FingerPrintImage = null;
                RubricImage = null;
                RubricImageText = null;
                pbPersonImage.Image = SAMBHS.Windows.SigesoftIntegration.UI.Properties.Resources.usuario;
                txtContactEmergency.Text = "";
                txtPhoneEmergency.Text = "";
                //pbPersonImage.ImageLocation = Application.StartupPath + "\\Resources\\usuario.jpg";
                txtNombreTitular.Text = "";
                _personId = null;
                cboModalidadTrabajo.SelectedValue = -1;
                cboMarketing.SelectedValue = -1;
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
           
        }

        private void TrabajadorNoEncontrado(string nrodocumento)
        {
            try
            {
                MessageBox.Show(@"El trabajador con el nro. documento " + nrodocumento + @" no se encuentra en registrado",
                @"Información");
                LimparControlesTrabajador();
                txtNroDocumento.Text = txtSearchNroDocument.Text;
                txtNombres.Focus();
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }
        
        private void LimparControlesTrabajador()
        {
            txtNombres.Text = "";
            txtApellidoPaterno.Text = "";
            txtApellidoMaterno.Text = "";
            cboTipoDocumento.SelectedValue = "-1";
            cboGenero.SelectedValue = "-1";
            txtNroDocumento.Text = "";
        }

        private void LlenarCampos(AgendaBl.DatosTrabajador datosTrabajador)
        {
            try
            {
                txtNombres.Text = datosTrabajador.Nombres.Trim();
                txtApellidoPaterno.Text = datosTrabajador.ApellidoPaterno.Trim();
                txtApellidoMaterno.Text = datosTrabajador.ApellidoMaterno.Trim();
                cboTipoDocumento.SelectedValue = datosTrabajador.TipoDocumentoId;
                txtNroDocumento.Text = datosTrabajador.NroDocumento.Trim();
                cboGenero.SelectedValue = datosTrabajador.GeneroId;
                dtpBirthdate.Value = datosTrabajador.FechaNacimiento.Value;

                cboEstadoCivil.SelectedValue = datosTrabajador.EstadoCivil;
                txtBirthPlace.Text = datosTrabajador.LugarNacimiento.Trim();

                cboDistrito.SelectedValue = datosTrabajador.Distrito;

                //cboProvincia.DataSource = new AgendaBl().ObtenerTodasProvincia(cboProvincia, 113);

                cboProvincia.SelectedValue = datosTrabajador.Provincia == null ? -1 : datosTrabajador.Provincia;

                //cboDepartamento.DataSource = new AgendaBl().ObtenerTodosDepartamentos(cboDepartamento, 113);
                cboDepartamento.SelectedValue = datosTrabajador.Departamento == null ? -1 : datosTrabajador.Departamento;


                cboResidencia.SelectedValue = datosTrabajador.Reside;
                txtMail.Text = datosTrabajador.Email.Trim();
                txtAdressLocation.Text = datosTrabajador.Direccion.Trim();
                //txtPuesto.Text = datosTrabajador.Puesto;
                //var lista = AgendaBl.ObtenerPuestos();
                //txtPuesto.DataSource = lista;
                //txtPuesto.DisplayMember = "Puesto";
                //txtPuesto.ValueMember = "Puesto";

                //txtPuesto.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                //txtPuesto.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
                //this.txtPuesto.DropDownWidth = 250;
                //txtPuesto.DisplayLayout.Bands[0].Columns[0].Width = 10;
                //txtPuesto.DisplayLayout.Bands[0].Columns[1].Width = 250;

                if (!string.IsNullOrEmpty(datosTrabajador.Puesto))
                    txtPuesto.Value = datosTrabajador.Puesto.Trim();
                else
                    txtPuesto.Value = "";

                if (!string.IsNullOrEmpty(datosTrabajador.PaisOrigen))
                    txtPaisOrigen.Value = datosTrabajador.PaisOrigen.Trim();
                else
                    txtPaisOrigen.Value = "";

                if (!string.IsNullOrEmpty(datosTrabajador.Nacionalidad))
                    txtNacionalidad.Value = datosTrabajador.Nacionalidad.Trim();
                else
                    txtNacionalidad.Value = "";

                cboEtniaRaza.SelectedValue = datosTrabajador.i_EtniaRaza == null ? -1 : datosTrabajador.i_EtniaRaza;
                cboMigrante.SelectedValue = datosTrabajador.i_Migrante == null ? -1 : datosTrabajador.i_Migrante;

                if (!string.IsNullOrEmpty(datosTrabajador.CelularEmergencia))
                    txtPhoneEmergency.Text = datosTrabajador.CelularEmergencia;
                else
                    txtPhoneEmergency.Text = "";

                //var contact = AgendaBl.ObtenerContactoEmergencia();
                //txtContactEmergency.DataSource = contact;
                //txtContactEmergency.DisplayMember = "ContactoEmergencia";
                //txtContactEmergency.ValueMember = "ContactoEmergenciaId";

                //txtContactEmergency.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                //txtContactEmergency.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
                //this.txtPuesto.DropDownWidth = 250;
                //txtContactEmergency.DisplayLayout.Bands[0].Columns[0].Width = 10;
                //txtContactEmergency.DisplayLayout.Bands[0].Columns[1].Width = 300;

                if (!string.IsNullOrEmpty(datosTrabajador.ContactoEmergencia))
                    txtContactEmergency.Value = datosTrabajador.ContactoEmergencia;
                else
                    txtContactEmergency.Value = "";


                //var listaCCo = AgendaBl.ObtenerCC();
                //txtCCosto.DataSource = listaCCo;
                //txtCCosto.DisplayMember = "Puesto";
                //txtCCosto.ValueMember = "Puesto";

                //txtCCosto.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                //txtCCosto.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
                //this.txtCCosto.DropDownWidth = 250;
                //txtCCosto.DisplayLayout.Bands[0].Columns[0].Width = 10;
                //txtCCosto.DisplayLayout.Bands[0].Columns[1].Width = 250;
                //if (!string.IsNullOrEmpty(datosTrabajador.Puesto))
                //{
                //    txtCCosto.Value = datosTrabajador.;
                //}



                cboAltitud.SelectedValue = datosTrabajador.Altitud;
                txtExploitedMineral.Text = datosTrabajador.Minerales.Trim();
                cboNivelEstudio.SelectedValue = datosTrabajador.Estudios;
                cboGrupo.SelectedValue = datosTrabajador.Grupo;
                cboFactorSan.SelectedValue = datosTrabajador.Factor;
                txtResidenceTimeInWorkplace.Text = datosTrabajador.TiempoResidencia.Trim();
                cboTipoSeguro.SelectedValue = datosTrabajador.TipoSeguro;
                txtNumberLivingChildren.Text = datosTrabajador.Vivos.ToString().Trim();
                txtNumberDependentChildren.Text = datosTrabajador.Muertos.ToString().Trim();
                txtNroHermanos.Text = datosTrabajador.Hermanos.ToString().Trim();
                txtTelephoneNumber.Text = datosTrabajador.Telefono.Trim();
                cboParentesco.SelectedValue = datosTrabajador.Parantesco;
                cboLugarLabor.SelectedValue = datosTrabajador.Labor;
                txtResidenciaAnte.Text = datosTrabajador.ResidenciaAnterior.Trim();
                //txtNacionalidad.Text = datosTrabajador.Nacionalidad;
                txtReligion.Text = datosTrabajador.Religion.Trim();

                FingerPrintTemplate = datosTrabajador.b_FingerPrintTemplate;
                FingerPrintImage = datosTrabajador.b_FingerPrintImage;
                RubricImage = datosTrabajador.b_RubricImage;
                RubricImageText = datosTrabajador.t_RubricImageText;
                pbPersonImage.Image = null;
                pbPersonImage.ImageLocation = null;
                pbPersonImage.Image = UtilsSigesoft.BytesArrayToImage(datosTrabajador.b_PersonImage, pbPersonImage);
                txtNombreTitular.Text = datosTrabajador.titular.Trim();

                _personId = datosTrabajador.PersonId;
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void cboServicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboServicio.SelectedIndex == 0 || cboServicio.SelectedIndex == -1)
                {

                    cboProtocolo.DataSource = new AgendaBl().LlenarComboProtocolo(cboProtocolo, 1000, 1000);
                    listaProtoColos = new AgendaBl().LlenarComboProtocolo(cboProtocolo, 1000, 1000);

                    cboProtocolo.SelectedIndex = 0;
                }
                else
                {
                    listaProtoColos = new AgendaBl().LlenarComboProtocolo(cboProtocolo, int.Parse(cboTipoServicio.SelectedValue.ToString()), int.Parse(cboServicio.SelectedValue.ToString()));

                    cboProtocolo.DataSource = new AgendaBl().LlenarComboProtocolo(cboProtocolo, int.Parse(cboTipoServicio.SelectedValue.ToString()), int.Parse(cboServicio.SelectedValue.ToString()));
                    cboProtocolo.SelectedIndex = 0;
                }
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void cboProtocolo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboProtocolo.SelectedIndex == 0 || cboProtocolo.SelectedIndex == -1)
                    LimpiarControlesProtocolo();
                else
                {
                    //if (cboEmpresaEmpleadora.DataSource == null)
                    //{
                    //    using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                    //    {
                    //        EmpresaBl.GetJoinOrganizationAndLocation(cboEmpresaEmpleadora, 9);
                    //        EmpresaBl.GetJoinOrganizationAndLocation(cboEmpresaCliente, 9);
                    //        EmpresaBl.GetJoinOrganizationAndLocation(cboEmpresaTrabajo, 9);
                    //    }
                    //}

                    LlenarControlesProtocolo();
                }
                //cboEmpresaFacturacion.AutoCompleteMode
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void LimpiarControlesProtocolo()
        {
            cboGeso.SelectedValue = -1;
            cboTipoEso.SelectedValue = -1; ;
            cboEmpresaCliente.SelectedValue = -1;
            cboEmpresaEmpleadora.SelectedValue = -1;
            cboEmpresaTrabajo.SelectedValue = -1;

            loadd = true;
            //cboEmpresaFacturacion.SelectedValue = -1;
        }

        private void LlenarControlesProtocolo()
        {
            try
            {
                //ARNOLD NEW STORES
                cboTipoEso.DataSource = new AgendaBl().LlenarComboSystemParametro(cboTipoEso, 118);
                cboTipoEso.SelectedIndex = 0;

                var datosProtocolo = AgendaBl.GetDatosProtocolo(cboProtocolo.SelectedValue.ToString());
                cboGeso.DataSource = new AgendaBl().ObtenerGesoProtocol(cboGeso, datosProtocolo.v_EmployerOrganizationId, datosProtocolo.v_EmployerLocationId);
                cboGeso.SelectedIndex = 0;

                cboGeso.SelectedValue = datosProtocolo.v_GroupOccupationId;
                cboTipoEso.SelectedValue = datosProtocolo.i_EsoTypeId.ToString();
                cboEmpresaCliente.SelectedValue = datosProtocolo.EmpresaCliente;
                cboEmpresaEmpleadora.SelectedValue = datosProtocolo.EmpresaEmpleadora;
                cboEmpresaTrabajo.SelectedValue = datosProtocolo.EmpresaTrabajo;
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void btnSavePacient_Click(object sender, EventArgs e)
        {
            try
            {
                String sFormato = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
                if (Regex.IsMatch(txtMail.Text.Trim(), sFormato))
                {
                    if (Regex.Replace(txtMail.Text.Trim(), sFormato, String.Empty).Length != 0)
                    {
                        MessageBox.Show(@"Debe ingresar correctamente el E-mail.", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        return;
                    }
                }
                else
                {
                    MessageBox.Show(@"Debe ingresar correctamente el E-mail.", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }


                if (cboDistrito.SelectedValue.ToString() == "-1")
                {
                    MessageBox.Show(@"Debe Seleccionar Distrito", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //if (cboEtniaRaza.SelectedValue != null)
                //{
                //    if (cboEtniaRaza.SelectedValue.ToString() == "-1")
                //    {
                //        MessageBox.Show(@"Debe Seleccionar ETIA / RAZA", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //        return;
                //    }
                //}


                if (cboTipoSeguro.SelectedValue.ToString() == "-1")
                {
                    MessageBox.Show(@"Debe Seleccionar Tipo de Seguro.", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (cboEtniaRaza.SelectedValue.ToString() == "-1")
                {
                    MessageBox.Show(@"Debe Seleccionar Raza / Etnia", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (cboMigrante.SelectedValue.ToString() == "-1")
                {
                    MessageBox.Show(@"Debe Seleccionar Si es migrante", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (txtPaisOrigen.Text == string.Empty)
                {
                    MessageBox.Show(@"Debe Seleccionar País de origen", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (txtNacionalidad.Text == string.Empty)
                {
                    MessageBox.Show(@"Debe Seleccionar Nacionalidad", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                GrabarTrabajadorNuevo();
                btnSavePacient.Enabled = false;
                //var listaPaciente = AgendaBl.ObtenerPacientes();
                //txtNombreTitular.DataSource = listaPaciente;


            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void GrabarTrabajadorNuevo(){
            try
            {
                PersonDto oPersonDto = new PersonDto();

                oPersonDto.Nombres = txtNombres.Text.Trim();
                oPersonDto.TipoDocumento = int.Parse(cboTipoDocumento.SelectedValue.ToString());
                oPersonDto.NroDocumento = txtNroDocumento.Text.Trim();
                oPersonDto.ApellidoPaterno = txtApellidoPaterno.Text.Trim();
                oPersonDto.ApellidoMaterno = txtApellidoMaterno.Text.Trim();
                oPersonDto.GeneroId = int.Parse(cboGenero.SelectedValue.ToString());
                oPersonDto.FechaNacimiento = dtpBirthdate.Value;

                oPersonDto.EstadoCivil = cboEstadoCivil.SelectedValue == null ? -1 : int.Parse(cboEstadoCivil.SelectedValue.ToString());
                oPersonDto.LugarNacimiento = txtBirthPlace.Text.Trim();
                oPersonDto.Distrito = cboDistrito.SelectedValue == null ? -1 : int.Parse(cboDistrito.SelectedValue.ToString());
                oPersonDto.Provincia = int.Parse(cboProvincia.SelectedValue.ToString());
                oPersonDto.Departamento = int.Parse(cboDepartamento.SelectedValue.ToString());
                oPersonDto.Reside = int.Parse(cboResidencia.SelectedValue.ToString());
                oPersonDto.Email = txtMail.Text.Trim();
                oPersonDto.Direccion = txtAdressLocation.Text.Trim();
                oPersonDto.Puesto = txtPuesto.Text.Trim();
                oPersonDto.Area = txtArea.Text.Trim();
                
                oPersonDto.Altitud = cboAltitud.SelectedValue == null ? -1 : int.Parse(cboAltitud.SelectedValue.ToString());
                oPersonDto.Minerales = txtExploitedMineral.Text.Trim();
                oPersonDto.ContactoEmergencia = txtContactEmergency.Text.Trim();
                oPersonDto.CelularEmergencia = txtPhoneEmergency.Text.Trim();

                if (cboEtniaRaza.SelectedValue == null)
                {
                    oPersonDto.i_EtniaRaza = AgendaBl.CreateItemEtniaRaza(cboEtniaRaza.Text);
                }
                else
                {
                    oPersonDto.i_EtniaRaza = int.Parse(cboEtniaRaza.SelectedValue.ToString());
                }
                oPersonDto.i_Migrante = int.Parse(cboMigrante.SelectedValue.ToString());
                oPersonDto.v_Migrante = txtPaisOrigen.Text.Trim();
                oPersonDto.Estudios = cboNivelEstudio.SelectedValue == null ? -1 : int.Parse(cboNivelEstudio.SelectedValue.ToString());
                oPersonDto.Grupo = -1;
                oPersonDto.Factor = -1;
                oPersonDto.TiempoResidencia = txtResidenceTimeInWorkplace.Text.Trim();
                oPersonDto.TipoSeguro = cboTipoSeguro.SelectedValue == null ? -1 : int.Parse(cboTipoSeguro.SelectedValue.ToString());
                oPersonDto.Vivos = int.Parse(txtNumberLivingChildren.Text.ToString());
                oPersonDto.Muertos = txtNumberDependentChildren == null ? 0 : int.Parse(txtNumberDependentChildren.Text.ToString());
                oPersonDto.Hermanos = txtNroHermanos.Text == null ? 0 : int.Parse(txtNroHermanos.Text.ToString());
                oPersonDto.Telefono = txtTelephoneNumber.Text.Trim();
                oPersonDto.Parantesco = cboParentesco.SelectedValue == null ? -1 : int.Parse(cboParentesco.SelectedValue.ToString());
                oPersonDto.Labor = cboLugarLabor.SelectedValue == null ? -1 : int.Parse(cboLugarLabor.SelectedValue.ToString());

                oPersonDto.Nacionalidad = txtNacionalidad.Text.Trim();
                oPersonDto.ResidenciaAnte = txtResidenciaAnte.Text.Trim();
                oPersonDto.Religion = txtReligion.Text.Trim();
                oPersonDto.titular = txtNombreTitular.Text.Trim();

                if (pbPersonImage.Image != null)
                {
                    MemoryStream ms = new MemoryStream();
                    Bitmap bm = new Bitmap(pbPersonImage.Image);
                    bm.Save(ms, ImageFormat.Jpeg);
                    oPersonDto.b_PersonImage = UtilsSigesoft.ResizeUploadedImage(ms);
                    pbPersonImage.Image.Dispose();
                }
                else
                {
                    oPersonDto.b_PersonImage = null;
                }

                oPersonDto.b_FingerPrintTemplate = FingerPrintTemplate;
                oPersonDto.b_FingerPrintImage = FingerPrintImage;
                oPersonDto.b_RubricImage = RubricImage;
                oPersonDto.t_RubricImageText = RubricImageText;
                string resultadoSMS = "";
                if (_personId != null)
                {
                    _personId = AgendaBl.UpdatePerson(oPersonDto, _personId);
                    resultadoSMS = InsertarUsuario(_personId, oPersonDto);
                }
                else
                {
                    _personId = AgendaBl.AddPerson(oPersonDto);
                    if (_personId == "El paciente ya se encuentra registrado")
                    {
                        MessageBox.Show(@"El paciente ya se encuentra registrado", @"Información");
                        return;
                    }
                    resultadoSMS = InsertarUsuario(_personId, oPersonDto);
                    #region Grabar en SAM
                    clienteDto _clienteDto = new clienteDto();
                    _clienteDto.v_CodCliente = txtNroDocumento.Text; ;
                    _clienteDto.i_IdTipoPersona = 1;
                    int tipodoc = 1;
                    if (cboTipoDocumento.SelectedValue.ToString() == "1")
                    {
                        tipodoc = 1;
                    }
                    else if (cboTipoDocumento.SelectedValue.ToString() == "2")
                    {
                        tipodoc = 7;
                    }
                    else if (cboTipoDocumento.SelectedValue.ToString() == "3")
                    {
                        tipodoc = 0;
                    }
                    else if (cboTipoDocumento.SelectedValue.ToString() == "4")
                    {
                        tipodoc = 4;
                    }
                    _clienteDto.i_IdTipoIdentificacion = tipodoc;
                    _clienteDto.v_PrimerNombre = txtNombres.Text;
                    _clienteDto.v_SegundoNombre = "";
                    _clienteDto.v_ApePaterno = txtApellidoPaterno.Text;
                    _clienteDto.v_ApeMaterno = txtApellidoMaterno.Text;
                    _clienteDto.v_RazonSocial = string.Empty;

                    _clienteDto.i_UsaLineaCredito = 0;
                    _clienteDto.v_NombreContacto = "";
                    _clienteDto.v_NroDocIdentificacion = txtNroDocumento.Text.Trim();
                    _clienteDto.v_DirecPrincipal = txtAdressLocation.Text;
                    _clienteDto.v_DirecPrincipal = txtAdressLocation.Text;
                    _clienteDto.v_DirecSecundaria = "";
                    _clienteDto.v_Correo = txtMail.Text;
                    _clienteDto.v_TelefonoFax = txtTelephoneNumber.Text;
                    _clienteDto.v_TelefonoFijo = txtTelephoneNumber.Text;
                    _clienteDto.v_TelefonoMovil = txtTelephoneNumber.Text;
                    _clienteDto.i_IdPais = 51;
                    _clienteDto.i_IdDistrito = cboDistrito.SelectedValue == null ? -1 : int.Parse(cboDistrito.SelectedValue.ToString());
                    _clienteDto.i_IdDepartamento = int.Parse(cboDepartamento.SelectedValue.ToString());
                    _clienteDto.i_IdListaPrecios = -1;
                    _clienteDto.i_IdProvincia = int.Parse(cboProvincia.SelectedValue.ToString());
                    _clienteDto.t_FechaNacimiento = dtpBirthdate.Value;
                    _clienteDto.i_Nacionalidad = 0;
                    _clienteDto.i_Activo = 1;
                    _clienteDto.i_IdSexo = int.Parse(cboGenero.SelectedValue.ToString());
                    _clienteDto.i_IdGrupoCliente = 0;
                    _clienteDto.i_IdZona = 0;

                    _clienteDto.v_FlagPantalla = "C";

                    _clienteDto.v_NroCuentaDetraccion = "";
                    _clienteDto.i_AfectoDetraccion = 0;


                    _clienteDto.i_EsPrestadorServicios = 0;
                    _clienteDto.v_Servicio = "";
                    _clienteDto.i_IdConvenioDobleTributacion = 0;
                    _clienteDto.v_Alias = "";
                    _clienteDto.v_Password = "";
                    // Save the data
                    OperationResult objOperationResult = new OperationResult();
                    InsertarCliente(ref objOperationResult, _clienteDto, Globals.ClientSession.GetAsList());


                    #endregion

                    
                }
                _sexTypeId = oPersonDto.GeneroId;
                _fechaNacimiento = oPersonDto.FechaNacimiento;

                MessageBox.Show(@"Se grabó correctamente. - " + resultadoSMS, @"Información");
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
                
        }

        public void InsertarCliente(ref OperationResult pobjOperationResult, clienteDto pobjDtoEntity, List<string> ClientSession)
        {
            //TRABAJADOR ....TT-86
            //CONTRATOTRABAJADOR  TC-87
            //CONTRATODETALLETRABAJADOR TD- 88
            //REGIMEN PENSIONARIO  TR- 89
            //DERECHO HABIENTE TV-92
            //AREAS LABORADAS TZ-93

            int SecuentialId = 0;
            string newIdCliente = string.Empty;
            string newIdTrabajador = string.Empty;
            string newIdContrato = string.Empty;
            string newIdContratoDetalle = string.Empty, newIdRegimen = String.Empty, newIdDH = String.Empty;
            try
            {
                using (var ts = TransactionUtils.CreateTransactionScope())
                {
                    using (var dbContext = new SAMBHSEntitiesModelWin())
                    {
                        cliente objEntity = clienteAssembler.ToEntity(pobjDtoEntity);
                        objEntity.t_InsertaFecha = DateTime.Now;
                        objEntity.i_InsertaIdUsuario = Int32.Parse(ClientSession[2]);
                        objEntity.i_Eliminado = 0;
                        int intNodeId = int.Parse(ClientSession[0]);
                        SecuentialId = GetNextSecuentialId(intNodeId, 14);
                        newIdCliente = GetNewId(int.Parse(ClientSession[0]), SecuentialId, "CL");
                        objEntity.v_IdCliente = newIdCliente;
                        dbContext.AddTocliente(objEntity);
                        dbContext.SaveChanges();
                        pobjOperationResult.Success = 1;
                        ts.Complete();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static string GetNewId(int pintNodeId, int pintSequential, string pstrPrefix)
        {
            var nodeSufix = Globals.ClientSession != null ? Globals.ClientSession.ReplicationNodeID : "N";
            return string.Format("{0}{1}-{2}{3}", nodeSufix, pintNodeId.ToString("000"), pstrPrefix, pintSequential.ToString("000000000"));
        }

        public int GetNextSecuentialId(int pintNodeId, int pintTableId, SAMBHSEntitiesModelWin objContext = null)
        {
            var dbContext = objContext ?? new SAMBHSEntitiesModelWin();

            string replicationId = Globals.ClientSession != null ? Globals.ClientSession.ReplicationNodeID : "N";
            secuential objSecuential = (from a in dbContext.secuential
                                        where a.i_TableId == pintTableId && a.i_NodeId == pintNodeId && a.v_ReplicationID == replicationId
                                        select a).SingleOrDefault();

            // Actualizar el campo con el nuevo valor a efectos de reservar el ID autogenerado para evitar colisiones entre otros nodos
            if (objSecuential != null)
            {
                objSecuential.i_SecuentialId = objSecuential.i_SecuentialId + 1;
            }
            else
            {
                objSecuential = new secuential
                {
                    i_NodeId = pintNodeId,
                    i_TableId = pintTableId,
                    i_SecuentialId = 1,
                    v_ReplicationID = replicationId
                };
                dbContext.AddTosecuential(objSecuential);
            }

            dbContext.SaveChanges();

            return objSecuential.i_SecuentialId.Value;

        }

        string InsertarUsuario(string _personId, PersonDto _PersonDto)
        {
            var usuarioSistrema = Globals.ClientSession.GetAsList();
            var usuario = AgendaBl.GetSystemUser(_personId);
            int usuariograba = Int32.Parse(usuarioSistrema[2]);

            if (usuariograba == 2034)
            {
                usuariograba = 199;
            }
            else if (usuariograba == 2035)
            {
                usuariograba = 197;
            }
            else
            {
                usuariograba = 11;
            }
            if (usuario == null)
            {
                SystemUser _newUser = new SystemUser();
                _newUser.v_PersonId = _personId;
                _newUser.v_UserName = txtNombres.Text.Trim().Split(' ')[0].ToLower() + "." + txtApellidoPaterno.Text.Trim().Replace(" ", "").ToLower();
                _newUser.v_Password = new EncryptPassword().EncryptString(txtNroDocumento.Text.Trim());
                _newUser.i_IsDeleted = 0;
                _newUser.d_InsertDate = DateTime.Now;
                _newUser.i_InsertUserId = usuariograba;
                _newUser.i_SystemUserTypeId = 5;
                AgendaBl.InsertSystemUser(_newUser);
            }
            if (_PersonDto.Telefono != null || _PersonDto.Telefono != "" )
            {
                return ConsumirPostSMS(_PersonDto.Nombres.Trim().Split(' ')[0] + " " + _PersonDto.ApellidoPaterno.Trim(),
                    txtNombres.Text.Trim().Split(' ')[0].ToLower() + "." + txtApellidoPaterno.Text.Trim().Replace(" ", "").ToLower(),
                    _PersonDto.NroDocumento, _PersonDto.Telefono);
            }
            else
            {
                return "No registrado para sms.";
            }
            
        }
        public static string GetApplicationConfigValue(string nombre)
        {
            return Convert.ToString(ConfigurationManager.AppSettings[nombre]);
        }
        public string ConsumirPostSMS(string persona, string usuario, string contraseña, string celular)
        {
            string ruta = GetApplicationConfigValue("APISMS").ToString();

            string url = ruta;
            
            //SMSUsuario _SMSUsuario = new SMSUsuario();
            //_SMSUsuario.v_smsContent = "Estimado Sr(a). ARNOLD ODAR para visualizar sus resultados de CSL ingresar a: wwww.clinicasanlorenzo.com.pe Usuario: arnold.odar Contraseña: 72087308.";
            //_SMSUsuario.v_phoneNummber = "950687514";

            //string json = "{\"v_smsContent\":\"Sr(a). " + persona + " ha sido registrado con éxito en CLINICA SAN LORENZO S.R.L. para poder visualizar sus resultados acceda a: * cslresultados.ddns.net:8040/frmLogin.aspx Usuario: " + usuario + " Contraseña: " + contraseña + ". !Gracias por su preferencia¡\",\"v_phoneNummber\":\"" + celular + "\"}";
            string json = "{\"v_phoneNummber\":\"" + celular + "\", \"i_status\":\"4\", \"v_pass\":\"" + contraseña + "\", \"v_user\":\"" + usuario + "\",\"v_NameClient\":\"" + persona + "\"}";

            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.PreAuthenticate = true;
            request.ContentType = "application/json;charset=utf-8";
            request.Timeout = 90000;
            //using (var stream = new MemoryStream())
            //using (var streamWriter = new StreamWriter(stream))    
            //{

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                };

                var httpResponse = (HttpWebResponse)request.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    return "¡SMS enviado correctamente!";
                } 
            //}
            //else
            //{  
            //    return "¡ERROR DE ENVIO DE SMSM!";
            //}
            


        }
        #region Enumeradores

        public enum ServiceType
        {
            Empresarial = 1,
            Particular = 9,
            Preventivo = 11
        }

        public enum ServiceStatus
        {
            PorIniciar = 1,
            Iniciado = 2,
            Culminado = 3,
            Incompleto = 4,
            Cancelado = 5,
            EsperandoAptitud = 6
        }

        public enum AptitudeStatus
        {
            Apto = 2,
            NoApto = 3,
            AptoObs = 4,
            AptRestriccion = 5,
            SinAptitud = 1
        }

        #endregion

        private void cboEstadoCivil_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void cboEstadoCivil_Leave(object sender, EventArgs e)
        {
           
        }

        private void cboDistrito_Leave(object sender, EventArgs e)
        {
            try
            {
                if (cboDistrito.Text == "--Seleccionar--") return;
                //ARNOLD NEW STORES
                var distritos = AgendaBl.BuscarDistritos(cboDistrito.Text);

                var idDistrito = distritos[0].Value4.ToString();

                var provincia = AgendaBl.ObtenerProvincia(int.Parse(idDistrito));

                AgendaBl.LlenarProvincia(provincia, cboProvincia);
                if (provincia.Count > 1)
                {
                    cboProvincia.SelectedValue = provincia[1].Id;
                }

                var idDepartamento = provincia[1].Value4.ToString();

                var departamento = AgendaBl.ObtenerProvincia(int.Parse(idDepartamento));

                AgendaBl.LlenarProvincia(departamento, cboDepartamento);

                if (departamento.Count > 1)
                {
                    cboDepartamento.SelectedValue = departamento[1].Id;
                }
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
           
        }

        private bool IsValidImageSize(string pfilePath)
        {
            using (FileStream fs = new FileStream(pfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Image original = Image.FromStream(fs);

                if (original.Width > ConstantsSigesoft.WIDTH_MAX_SIZE_IMAGE || original.Height > ConstantsSigesoft.HEIGHT_MAX_SIZE_IMAGE)
                {
                    MessageBox.Show("La imagen que está tratando de subir es damasiado grande.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }

        private void LoadFile(string pfilePath)
        {
            Image img = pbPersonImage.Image;

            // Destruyo la posible imagen existente en el control
            //
            if (img != null)
            {
                img.Dispose();
            }

            using (FileStream fs = new FileStream(pfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Image original = Image.FromStream(fs);
                pbPersonImage.Image = original;

            }
        }
        
        private void btnArchivo1_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.FileName = string.Empty;
                openFileDialog1.Filter = "Image Files (*.jpg;*.gif;*.jpeg;*.png)|*.jpg;*.gif;*.jpeg;*.png";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (!IsValidImageSize(openFileDialog1.FileName))
                        return;

                    // Seteaar propiedades del control PictutreBox
                    LoadFile(openFileDialog1.FileName);
                    //pbPersonImage.SizeMode = PictureBoxSizeMode.Zoom;
                    txtFileName.Text = Path.GetFileName(openFileDialog1.FileName);
                    // Setear propiedades de usuario
                    _fileName = Path.GetFileName(openFileDialog1.FileName);
                    _filePath = openFileDialog1.FileName;

                    var Ext = Path.GetExtension(txtFileName.Text);

                    if (Ext == ".JPG" || Ext == ".GIF" || Ext == ".JPEG" || Ext == ".PNG" || Ext == "")
                    {

                        System.Drawing.Bitmap bmp1 = new System.Drawing.Bitmap(pbPersonImage.Image);

                        Decimal Hv = 280;
                        Decimal Wv = 383;

                        Decimal k = -1;

                        Decimal Hi = bmp1.Height;
                        Decimal Wi = bmp1.Width;

                        Decimal Dh = -1;
                        Decimal Dw = -1;

                        Dh = Hi - Hv;
                        Dw = Wi - Wv;

                        if (Dh > Dw)
                        {
                            k = Hv / Hi;
                        }
                        else
                        {
                            k = Wv / Wi;
                        }

                        pbPersonImage.Height = (int)(k * Hi);
                        pbPersonImage.Width = (int)(k * Wi);
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            pbPersonImage.Image = null;
        }

        private void btnWebCam_Click(object sender, EventArgs e)
        {
            try
            {
                frmCamera frm = new frmCamera();
                frm.ShowDialog();

                if (System.Windows.Forms.DialogResult.Cancel != frm.DialogResult)
                {
                    pbPersonImage.Image = frm._Image;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("ddd");
            }
        }

        private void btnCapturedFingerPrintAndRubric_Click(object sender, EventArgs e)
        {
            try
            {
                var frm = new frmCapturedFingerPrint();
                frm.Mode = Mode;

                if (Mode == "Edit")
                {
                    frm.FingerPrintTemplate = FingerPrintTemplate;
                    frm.FingerPrintImage = FingerPrintImage;
                    frm.RubricImage = RubricImage;
                    frm.RubricImageText = RubricImageText;
                }

                frm.ShowDialog();

                FingerPrintTemplate = frm.FingerPrintTemplate;
                FingerPrintImage = frm.FingerPrintImage;
                RubricImage = frm.RubricImage;
                RubricImageText = frm.RubricImageText;
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void txtRucCliente_MouseDown(object sender, MouseEventArgs e)
        {
           
        }

        private void txtRucCliente_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (!txtRucCliente.IsDroppedDown && e.KeyCode == Keys.Enter)
                {
                    frmEmpresa frm = new frmEmpresa(txtRucCliente.Text.Trim());
                    frm.ShowDialog();
                    cboEmpresaEmpleadora.DataSource = new EmpresaBl().GetJoinOrganizationAndLocation(cboEmpresaEmpleadora, 9);
                    cboEmpresaEmpleadora.SelectedValue = frm.orgnizationEmployerId ?? "-1";
                }
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
        }

        private void frmAgendarTrabajador_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                listaProtoColos = new List<EsoDtoProt>();

                cboProtocolo.DataSource = null;
                cboProtocolo.Items.Clear();
                cboProtocolo.DataSource = new AgendaBl().LlenarComboProtocolo(cboProtocolo, Convert.ToInt32(cboTipoServicio.SelectedValue), Convert.ToInt32(cboServicio.SelectedValue));
                listaProtoColos = new AgendaBl().LlenarComboProtocolo(cboProtocolo, Convert.ToInt32(cboTipoServicio.SelectedValue), Convert.ToInt32(cboServicio.SelectedValue));

                cboProtocolo.DisplayMember = "Nombre";
                cboProtocolo.ValueMember = "Id";
                cboProtocolo.SelectedIndex = 0;
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void cboTipoServicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTipoServicio.SelectedIndex == 0 || cboTipoServicio.SelectedIndex == -1)
                {
                    cboServicio.DataSource = new AgendaBl().LlenarComboServicio(cboServicio, 1000);
                    cboServicio.SelectedIndex = 0;
                }
                else
                {
                    cboServicio.DataSource = new AgendaBl().LlenarComboServicio(cboServicio, int.Parse(cboTipoServicio.SelectedValue.ToString()));
                    cboServicio.SelectedIndex = 0;
                    if (int.Parse(cboTipoServicio.SelectedValue.ToString()) == 9 || int.Parse(cboTipoServicio.SelectedValue.ToString()) == 11 || int.Parse(cboTipoServicio.SelectedValue.ToString()) == 12 || int.Parse(cboTipoServicio.SelectedValue.ToString()) == 13)
                    {
                        cboMedicoTratante.Enabled = true;
                    }
                    else
                    {
                        cboMedicoTratante.Enabled = false;
                        cboMedicoTratante.SelectedValue = "-1";
                    }
                }
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            

        }
        
        private void cboTipoServicio_TabIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnRecargarEmpresa_Click(object sender, EventArgs e)
        {
            try
            {
                //cboEmpresaFacturacion
                cboEmpresaFacturacion.DataSource = null;
                cboEmpresaFacturacion.Items.Clear();
                cboEmpresaFacturacion.DataSource = new EmpresaBl().GetOrganizationFacturacion(cboEmpresaFacturacion, 9);
                cboEmpresaFacturacion.DisplayMember = "Value1";
                cboEmpresaFacturacion.ValueMember = "Id";
                cboEmpresaFacturacion.SelectedIndex = 0;
                //AgendaBl.LlenarComboProtocolo(cboProtocolo, int.Parse(cboTipoServicio.SelectedValue.ToString()), int.Parse(cboServicio.SelectedValue.ToString()));
        
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //private void cboEmpresaFacturacion_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    //if (cboEmpresaFacturacion.SelectedIndex == 0 || cboEmpresaFacturacion.SelectedIndex == -1)
        //    //    LimpiarControlesProtocolo();
            
        //        //LlenarControlesProtocolo();
        //}

        private void btnNuevoRegistro_Click(object sender, EventArgs e)
        {
            try
            {
                frmAgendarTrabajador frm1 = new frmAgendarTrabajador("CONTINUE", "", "", 1, "");
                frm1.groupBox2.Visible = false;
                frm1.btnCancel.Visible = false;
                frm1.btnschedule.Visible = false;
                frm1.Size = new Size(1040, 550);
                frm1.label29.Visible = false;
                frm1.txtNombreTitular.Visible = false;
                frm1.btnNuevoRegistro.Visible = false;
                frm1.cboParentesco.Visible = false;
                frm1.Parentesco.Visible = false;
                frm1.ShowDialog();
                var listaPaciente = AgendaBl.ObtenerPacientes();
                txtNombreTitular.DataSource = listaPaciente;
            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_personId))
            {
                MessageBox.Show("Tiene que grabar al paciente antes de agendar.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var frmVacunas = new frmVacunasCovid19(_personId);
            frmVacunas.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmBuscarProtocolo frm = new frmBuscarProtocolo(listaProtoColos);
            frm.ShowDialog();
            cboProtocolo.SelectedValue = frm.protocoloId;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frmListarPacientes frm = new frmListarPacientes();
            frm.ShowDialog();
            txtSearchNroDocument.Text = frm.PacienteId;

            if (txtSearchNroDocument.Text != String.Empty)
            {
                btnBuscarTrabajador_Click(sender, e);
            }
        }

        private void cboMarketing_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        
    }
}
