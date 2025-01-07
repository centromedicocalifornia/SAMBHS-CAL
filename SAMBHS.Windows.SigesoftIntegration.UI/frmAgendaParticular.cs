using SAMBHS.Common.BE;
using SAMBHS.Common.DataModel;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using ScrapperReniecSunat;
using Sigesoft.Node.WinClient.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows.Forms;

using System.Text.RegularExpressions;
using Newtonsoft.Json;
//using Newtonsoft.Json;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmAgendaParticular : Form
    {
        private string _personId;
        private DateTime? _fechaNacimiento;
        private int _sexTypeId;
        private string _modoPre;
        private string _dni;
        private string _empresa;
        private int _tipoAtencion;
        private string _contrata;
        string Mode;
        private AgendaBl agendaBl_ = new AgendaBl();
        private EmpresaBl empresaBl_ = new EmpresaBl();
        public int cierre = 0;
        List<EsoDtoProt> listaProtoColos = new List<EsoDtoProt>();
        private string _protocoloId;
        private DateTime? _FechaCita;
        private string _idccEditar = "";

        private string modoAgenda;
        private string CalendarId;
        private string GLobalv_descuentoDetalleId = "";
        public int isFact;
        public string Comprobante;
        int grupoSeleccionado = 0;
        public string Medico;
        public string Turnoss;
        public string Hora;
        public string Grupo;
        DateTime? Fechass;

        int grupo = 0;


        #region Properties


        public byte[] FingerPrintTemplate { get; set; }

        public byte[] FingerPrintImage { get; set; }

        public byte[] RubricImage { get; set; }

        public string RubricImageText { get; set; }

        #endregion

        public frmAgendaParticular(string modoPre, string dni, string empresa, int tipoAtencion, string contrata, string _modoAgenda, string _CalendarId,
            int _isFact, string _Comprobante, string protocoloId, string _Medico, string _Turno, string _Hora, string _Grupo, DateTime? _Fecha, string idccEditar)//,int isFact_, string _Comprobante)
        {
            _protocoloId = protocoloId;
            CalendarId = _CalendarId;
            modoAgenda = _modoAgenda;
            _modoPre = modoPre;
            _dni = dni;
            _empresa = empresa;
            _contrata = contrata;

            _tipoAtencion = tipoAtencion;
            InitializeComponent();
            Medico = _Medico;
            Turnoss = _Turno;
            Hora = _Hora;
            Grupo = _Grupo;
            Fechass = _Fecha;
            isFact = _isFact;
            Comprobante = _Comprobante;
            _FechaCita = _Fecha;
            _idccEditar = idccEditar;
        }


        //public frmAgendaParticular(string modoPre, string dni, string empresa, int tipoAtencion, string contrata, string protocoloId, DateTime? FechaCita, string idccEditar)
        //{

        //    _FechaCita = FechaCita;
        //    _idccEditar = idccEditar;
        //    InitializeComponent();
        //}

        private void frmAgendaParticular_Load(object sender, EventArgs e)
        {
            using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
            {

                AgendaBl.LlenarComboMedicoSolicitanteExterno(cboMedicoSolicitanteExterno);
                AgendaBl.LlenarComboEstablecimiento(cboEstablecimiento);
                AgendaBl.LlenarComboVendedorExterno(cboVendedorExterno);

                AgendaBl.LlenarComboEspecialidad(cboEspecialidad);

                AgendaBl.LlenarComboTurno(cboTurno, "-1", DateTime.Now.ToString("yyyy-MM-dd"));
                AgendaBl.LlenarComboHorario(cboHorario, "-1", "-1", DateTime.Now.ToString("yyyy-MM-dd"));

                cboTurno.SelectedValue = "-1";
                cboHorario.SelectedValue = "-1";

                cboEstablecimiento.SelectedValue = 1;
                cboVendedorExterno.SelectedValue = 1;
                cboMedicoSolicitanteExterno.SelectedValue = 1;


                cboTipoSeguro.DataSource = agendaBl_.LlenarComboTipoSeguro(cboTipoSeguro);
                cboTipoSeguro.SelectedValue = -1;

                cboNivelEstudio.DataSource = agendaBl_.LlenarComboNivelEstudio(cboNivelEstudio);
                cboNivelEstudio.SelectedIndex = 0;

                cboTipoDocumento.DataSource = agendaBl_.LlenarComboTipoDocumento(cboTipoDocumento);
                cboTipoDocumento.SelectedIndex = 0;

                cboGenero.DataSource = agendaBl_.LlenarComboGennero(cboGenero);
                cboGenero.SelectedIndex = 0;

                cboEstadoCivil.DataSource = agendaBl_.LlenarComboEstadoCivil(cboEstadoCivil);
                cboEstadoCivil.SelectedIndex = 0;

                cboParentesco.DataSource = agendaBl_.LlenarComboParentesco(cboParentesco);
                cboParentesco.SelectedIndex = 0;

                // hosp tipo de paciente
                cboTipoHosp.DataSource = agendaBl_.LlenarComboPacHospSala(cboTipoHosp);
                cboTipoHosp.SelectedIndex = 0;

                cboTipoServicio.DataSource = agendaBl_.LlenarComboTipoServicio(cboTipoServicio);
                cboTipoServicio.SelectedIndex = 1;

                cboDistrito.DataSource = agendaBl_.LlenarComboDistrito(cboDistrito);
                cboDistrito.SelectedIndex = 0;

                cboProvincia.DataSource = agendaBl_.LlenarComboDistrito(cboProvincia);
                cboProvincia.SelectedIndex = 0;

                cboDepartamento.DataSource = agendaBl_.LlenarComboDistrito(cboDepartamento);
                cboDepartamento.SelectedIndex = 0;

                cboEmpresaFacturacion.DataSource = empresaBl_.GetOrganizationFacturacion(cboEmpresaFacturacion, 9);
                //cboEmpresaFacturacion.SelectedIndex = 0;

                cboEmpresaFacturacion.SelectedValue = "N009-OO000000670";


                cboEtniaRaza.DataSource = agendaBl_.LlenarComboEtnia(cboEtniaRaza);
                cboEtniaRaza.SelectedIndex = 0;

                cboMarketing.DataSource = agendaBl_.LlenarComboMarketing(cboMarketing);
                cboMarketing.SelectedIndex = 0;

                cboMigrante.DataSource = agendaBl_.LlenarComboResidencia(cboMigrante);
                cboMigrante.SelectedValue = -1;

                if (AgendaBl.ObtenerNacionalidad() == null)
                {
                    return;
                }
                else 
                {
                    var listaNacionalidad = AgendaBl.ObtenerNacionalidad();
                    txtNacionalidad.DataSource = listaNacionalidad;
                    txtNacionalidad.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;

                    txtNacionalidad.DisplayLayout.Bands[0].Columns[0].Width = 10;
                    txtNacionalidad.DisplayLayout.Bands[0].Columns[1].Width = 250;
                    if (!string.IsNullOrEmpty("")) { txtNacionalidad.Value = ""; }
                }


                if (AgendaBl.ObtenerPaisOrigen() == null)
                {
                    return;
                }
                else
                {
                    var listaPaisOrigen = AgendaBl.ObtenerPaisOrigen(); ;
                    txtPaisOrigen.DataSource = listaPaisOrigen;
                    txtPaisOrigen.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;

                    txtPaisOrigen.DisplayLayout.Bands[0].Columns[0].Width = 10;
                    txtPaisOrigen.DisplayLayout.Bands[0].Columns[1].Width = 250;
                    if (!string.IsNullOrEmpty("")) { txtPaisOrigen.Value = ""; }
                }

                //ARNOLD NEW STORES
                cboMedicoTratante.DataSource = new AgendaBl().LlenarComboUsuarios(cboMedicoTratante);
                AgendaBl.LlenarComboPlan(cboPlanAtencion);
                cboTipoDocumento.SelectedValue = 1;
                cboGenero.SelectedValue = 1;
                cboEstadoCivil.SelectedValue = 1;
                cboTipoServicio.SelectedValue = 1;
                cboServicio.SelectedValue = 2;
                var listaPaciente = AgendaBl.ObtenerPacientes();
                txtNombreTitular.DataSource = listaPaciente;
                txtNombreTitular.DisplayMember = "v_name";
                txtNombreTitular.ValueMember = "v_personId";
                txtNombreTitular.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                txtNombreTitular.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
                this.txtNombreTitular.DropDownWidth = 550;
                txtNombreTitular.DisplayLayout.Bands[0].Columns[0].Width = 20;
                txtNombreTitular.DisplayLayout.Bands[0].Columns[1].Width = 400;

                var lista_ContactEmergency = AgendaBl.ObtenerContactoEmergencia();
                txtContactEmergency.DataSource = lista_ContactEmergency;
                txtContactEmergency.DisplayMember = "ContactoEmergencia";
                txtContactEmergency.ValueMember = "ContactoEmergenciaId";
                txtContactEmergency.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                txtContactEmergency.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
                this.txtContactEmergency.DropDownWidth = 250;
                txtContactEmergency.DisplayLayout.Bands[0].Columns[0].Width = 10;
                txtContactEmergency.DisplayLayout.Bands[0].Columns[1].Width = 300;
                if (!string.IsNullOrEmpty("")) { txtContactEmergency.Value = ""; }

                var lista = AgendaBl.ObtenerPuestos();
                txtPuesto.DataSource = lista;
                txtPuesto.DisplayMember = "Puesto";
                txtPuesto.ValueMember = "Puesto";
                txtPuesto.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
                txtPuesto.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
                this.txtPuesto.DropDownWidth = 250;
                txtPuesto.DisplayLayout.Bands[0].Columns[0].Width = 10;
                txtPuesto.DisplayLayout.Bands[0].Columns[1].Width = 250;
                if (!string.IsNullOrEmpty("")) { txtPuesto.Value = ""; }


            };
            Mode = "New";

            string filtroEspecialidad = "";


            if (cboEspecialidad.SelectedValue.ToString() != "-1")
            {
                filtroEspecialidad = cboEspecialidad.Text;
            }

            var selectedValue = int.Parse(cboServicio.SelectedValue.ToString());
            var selectedType = int.Parse(cboTipoServicio.SelectedValue.ToString());


            if (_modoPre == "BUSCAR")
            {
                var datosTrabajador = AgendaBl.GetDatosTrabajador(_dni);
                if (datosTrabajador != null)
                {
                    Mode = "Edit";
                    LlenarCampos(datosTrabajador);
                    _sexTypeId = Convert.ToInt32(datosTrabajador.GeneroId);
                    _fechaNacimiento = datosTrabajador.FechaNacimiento;
                }
                else
                {
                    txtSearchNroDocument.Text = _dni;

                    btnBuscarTrabajador_Click(sender, e);
                    //ObtenerDatosDNI(_dni);

                }


                switch (_tipoAtencion)
                {
                    case 2:
                        {
                            cboTipoServicio.SelectedValue = 9;
                            cboServicio.SelectedValue = 10;
                            // ARNOLD NEW STORES
                            AgendaBl.LlenarComboProtocolo_Particular(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);
                            listaProtoColos = AgendaBl.LlenarComboProtocolo_Particular_new(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);

                            btnNuevoRegistro.Visible = false;

                            //PLAN
                            label18.Visible = false;
                            cboPlanAtencion.Visible = false;
                            //
                            txtNombreTitular.Visible = false;
                            btnNuevoRegistro.Visible = false;
                            lblTitular.Visible = false;
                            cboParentesco.Visible = false;
                            lblParentesco.Visible = false;
                            lblLicencia.Visible = false;
                            txtLicenciadeConducir.Visible = false;
                            break;
                        }

                    case 3:
                        {
                            cboTipoServicio.SelectedValue = 11;
                            cboServicio.SelectedValue = 12;
                            //ARNOLD NEW STORES
                            AgendaBl.LlenarComboProtocolo_Seguros(cboProtocolo, int.Parse(cboTipoServicio.SelectedValue.ToString()), int.Parse(cboServicio.SelectedValue.ToString()), _empresa, _contrata);

                            listaProtoColos = AgendaBl.LlenarComboProtocolo_Seguros_new(cboProtocolo, selectedType, selectedValue, _empresa, _contrata);
                            //cboProtocolo.SelectedIndex = -1;
                            btnNuevoRegistro.Visible = true;
                            //PLAN
                            label18.Visible = true;
                            cboPlanAtencion.Visible = true;
                            lblLicencia.Visible = false;
                            txtLicenciadeConducir.Visible = false;
                            break;
                            //
                        }
                    case 4:
                        {
                            cboTipoServicio.SelectedValue = 34;
                            cboServicio.SelectedValue = 35;
                            // ARNOLD NEW STORES
                            AgendaBl.LlenarComboProtocolo_Particular(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);
                            listaProtoColos = AgendaBl.LlenarComboProtocolo_Particular_new(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);

                            btnNuevoRegistro.Visible = false;
                            //PLAN
                            label18.Visible = false;
                            cboPlanAtencion.Visible = false;
                            //
                            txtNombreTitular.Visible = false;
                            btnNuevoRegistro.Visible = false;
                            lblTitular.Visible = false;
                            cboParentesco.Visible = false;
                            lblParentesco.Visible = false;
                            lblLicencia.Visible = true;
                            txtLicenciadeConducir.Visible = true;
                            break;
                        }

                }
            }
            else if (_modoPre == "DIGITAL")
            {
                var datosTrabajador = AgendaBl.GetDatosTrabajador(_dni);
                if (datosTrabajador != null)
                {
                    Mode = "Edit";
                    LlenarCampos(datosTrabajador);
                    _sexTypeId = Convert.ToInt32(datosTrabajador.GeneroId);
                    _fechaNacimiento = datosTrabajador.FechaNacimiento;
                }
                else
                {
                    txtSearchNroDocument.Text = _dni;

                    btnBuscarTrabajador_Click(sender, e);
                    //ObtenerDatosDNI(_dni);

                }



                switch (_tipoAtencion)
                {
                    case 2:
                        {
                            if (_protocoloId != "")
                            {
                                var Tipos = agendaBl_.getTiposServicioForProtocolo(_protocoloId);

                                cboTipoServicio.SelectedValue = Tipos.TipoServicio;
                                cboServicio.SelectedValue = Tipos.Servicio;
                            }
                            else
                            {
                                cboTipoServicio.SelectedValue = 9;
                                cboServicio.SelectedValue = 10;
                            }

                            // ARNOLD NEW STORES
                            //AgendaBl.LlenarComboProtocolo_Particular(cboProtocolo, int.Parse(cboServicio.SelectedValue.ToString()), int.Parse(cboTipoServicio.SelectedValue.ToString()), _empresa);

                            //listaProtoColos = AgendaBl.LlenarComboProtocolo_Particular_new(cboProtocolo, int.Parse(cboServicio.SelectedValue.ToString()), int.Parse(cboTipoServicio.SelectedValue.ToString()), _empresa);

                            AgendaBl.LlenarComboProtocolo_Particular(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);
                            listaProtoColos = AgendaBl.LlenarComboProtocolo_Particular_new(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);





                            btnNuevoRegistro.Visible = false;

                            //PLAN
                            label18.Visible = false;
                            cboPlanAtencion.Visible = false;
                            //
                            txtNombreTitular.Visible = false;
                            btnNuevoRegistro.Visible = false;
                            lblTitular.Visible = false;
                            cboParentesco.Visible = false;
                            lblParentesco.Visible = false;
                            lblLicencia.Visible = false;
                            txtLicenciadeConducir.Visible = false;
                            break;
                        }

                    case 3:
                        {
                            if (_protocoloId != "")
                            {
                                var Tipos = agendaBl_.getTiposServicioForProtocolo(_protocoloId);

                                cboTipoServicio.SelectedValue = Tipos.TipoServicio;
                                cboServicio.SelectedValue = Tipos.Servicio;
                            }
                            else
                            {
                                cboTipoServicio.SelectedValue = 11;
                                cboServicio.SelectedValue = 12;
                            }


                            //ARNOLD NEW STORES
                            //AgendaBl.LlenarComboProtocolo_Seguros(cboProtocolo, int.Parse(cboTipoServicio.SelectedValue.ToString()), int.Parse(cboServicio.SelectedValue.ToString()), _empresa, _contrata);

                            //listaProtoColos = AgendaBl.LlenarComboProtocolo_Seguros_new(cboProtocolo, int.Parse(cboTipoServicio.SelectedValue.ToString()), int.Parse(cboServicio.SelectedValue.ToString()), _empresa, _contrata);


                            AgendaBl.LlenarComboProtocolo_Seguros(cboProtocolo, selectedType, selectedValue, _empresa, _contrata);
                            listaProtoColos = AgendaBl.LlenarComboProtocolo_Seguros_new(cboProtocolo, selectedType, selectedValue, _empresa, _contrata);

                            //cboProtocolo.SelectedIndex = -1;
                            btnNuevoRegistro.Visible = true;
                            //PLAN
                            label18.Visible = true;
                            cboPlanAtencion.Visible = true;
                            lblLicencia.Visible = false;
                            txtLicenciadeConducir.Visible = false;
                            break;
                            //
                        }
                    case 4:
                        {
                            if (_protocoloId != "")
                            {
                                var Tipos = agendaBl_.getTiposServicioForProtocolo(_protocoloId);

                                cboTipoServicio.SelectedValue = Tipos.TipoServicio;
                                cboServicio.SelectedValue = Tipos.Servicio;
                            }
                            else
                            {
                                cboTipoServicio.SelectedValue = 34;
                                cboServicio.SelectedValue = 35;
                            }


                            // ARNOLD NEW STORES
                            //AgendaBl.LlenarComboProtocolo_Particular(cboProtocolo, int.Parse(cboServicio.SelectedValue.ToString()), int.Parse(cboTipoServicio.SelectedValue.ToString()), _empresa);

                            //listaProtoColos = AgendaBl.LlenarComboProtocolo_Particular_new(cboProtocolo, int.Parse(cboServicio.SelectedValue.ToString()), int.Parse(cboTipoServicio.SelectedValue.ToString()), _empresa);   

                            AgendaBl.LlenarComboProtocolo_Particular(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);
                            listaProtoColos = AgendaBl.LlenarComboProtocolo_Particular_new(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);

                            btnNuevoRegistro.Visible = false;
                            //PLAN
                            label18.Visible = false;
                            cboPlanAtencion.Visible = false;
                            //
                            txtNombreTitular.Visible = false;
                            btnNuevoRegistro.Visible = false;
                            lblTitular.Visible = false;
                            cboParentesco.Visible = false;
                            lblParentesco.Visible = false;
                            lblLicencia.Visible = true;
                            txtLicenciadeConducir.Visible = true;
                            break;
                        }

                }

                cboProtocolo.SelectedValue = _protocoloId;
                //checkDia.Checked = false;
                dtDateCalendar.Value = _FechaCita.Value;
            }
            else
            {
                switch (_tipoAtencion)
                {
                    case 2:
                        {
                            cboTipoServicio.SelectedValue = 9;
                            cboServicio.SelectedValue = 10;
                            AgendaBl.LlenarComboProtocolo_Particular(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);
                            listaProtoColos = AgendaBl.LlenarComboProtocolo_Particular_new(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);
                            //PLAN
                            label18.Visible = false;
                            cboPlanAtencion.Visible = false;
                            //
                            txtNombreTitular.Visible = false;
                            btnNuevoRegistro.Visible = false;
                            lblTitular.Visible = false;
                            cboParentesco.Visible = false;
                            lblParentesco.Visible = false;
                            lblLicencia.Visible = false;
                            txtLicenciadeConducir.Visible = false;
                            break;
                        }
                    case 3:
                        {
                            cboTipoServicio.SelectedValue = 11;
                            cboServicio.SelectedValue = 12;
                            AgendaBl.LlenarComboProtocolo_Seguros(cboProtocolo, int.Parse(cboTipoServicio.SelectedValue.ToString()), int.Parse(cboServicio.SelectedValue.ToString()), _empresa, _contrata);

                            listaProtoColos = AgendaBl.LlenarComboProtocolo_Seguros_new(cboProtocolo, int.Parse(cboTipoServicio.SelectedValue.ToString()), int.Parse(cboServicio.SelectedValue.ToString()), _empresa, _contrata);
                            //cboProtocolo.SelectedIndex = -1;
                            //PLAN
                            label18.Visible = true;
                            cboPlanAtencion.Visible = true;
                            lblLicencia.Visible = false;
                            txtLicenciadeConducir.Visible = false;
                            break;
                            //
                        }
                    case 4:
                        {
                            cboTipoServicio.SelectedValue = 34;
                            cboServicio.SelectedValue = 35;
                            // ARNOLD NEW STORES
                            AgendaBl.LlenarComboProtocolo_Particular(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);
                            listaProtoColos = AgendaBl.LlenarComboProtocolo_Particular_new(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);

                            btnNuevoRegistro.Visible = false;
                            //PLAN
                            label18.Visible = false;
                            cboPlanAtencion.Visible = false;
                            //
                            txtNombreTitular.Visible = false;
                            btnNuevoRegistro.Visible = false;
                            lblTitular.Visible = false;
                            cboParentesco.Visible = false;
                            lblParentesco.Visible = false;
                            lblLicencia.Visible = true;
                            txtLicenciadeConducir.Visible = true;
                            break;
                        }
                }
            }

            if (Medico != string.Empty || Medico != null)
            {
                cboMedicoTratante.SelectedValue = Medico;
            }

            dtDateCalendar.Text = Fechass.Value.ToShortDateString();

            if (Turnoss != string.Empty || Turnoss != null)
            {
                cboTurno.SelectedValue = Turnoss;
            }

            if (Hora != string.Empty || Hora != null)
            {
                cboHorario.SelectedValue = Hora;
            }
        }


        

        private void ObtenerDatosDNI(string dni)
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

        private void LlenarCampos(AgendaBl.DatosTrabajador datosTrabajador)
        {
            txtNombres.Text = datosTrabajador.Nombres.Trim();
            txtApellidoPaterno.Text = datosTrabajador.ApellidoPaterno.Trim();
            txtApellidoMaterno.Text = datosTrabajador.ApellidoMaterno.Trim();
            cboTipoDocumento.SelectedValue = datosTrabajador.TipoDocumentoId;
            txtNroDocumento.Text = datosTrabajador.NroDocumento.Trim();
            cboGenero.SelectedValue = datosTrabajador.GeneroId;
            dtpBirthdate.Value = datosTrabajador.FechaNacimiento.Value;
            cboEstadoCivil.SelectedValue = datosTrabajador.EstadoCivil;
            txtBirthPlace.Text = datosTrabajador.LugarNacimiento;
            cboDistrito.SelectedValue = datosTrabajador.Distrito;
            //cboProvincia.DataSource = new AgendaBl().ObtenerTodasProvincia(cboProvincia, 113);
            cboProvincia.SelectedValue = datosTrabajador.Provincia == null ? -1 : datosTrabajador.Provincia;
            //cboDepartamento.DataSource = new AgendaBl().ObtenerTodosDepartamentos(cboDepartamento, 113);
            cboDepartamento.SelectedValue = datosTrabajador.Departamento == null ? -1 : datosTrabajador.Departamento;
            txtMail.Text = datosTrabajador.Email;
            txtAdressLocation.Text = datosTrabajador.Direccion;
            txtTelephoneNumber.Text = datosTrabajador.Telefono;
            cboParentesco.SelectedValue = datosTrabajador.Parantesco;
            txtResidenciaAnte.Text = datosTrabajador.ResidenciaAnterior;
            txtReligion.Text = datosTrabajador.Religion;
            cboNivelEstudio.SelectedValue = datosTrabajador.Estudios;
            FingerPrintTemplate = datosTrabajador.b_FingerPrintTemplate;
            FingerPrintImage = datosTrabajador.b_FingerPrintImage;
            RubricImage = datosTrabajador.b_RubricImage;
            RubricImageText = datosTrabajador.t_RubricImageText;
            pbPersonImage.Image = null;
            pbPersonImage.ImageLocation = null;
            pbPersonImage.Image = UtilsSigesoft.BytesArrayToImage(datosTrabajador.b_PersonImage, pbPersonImage);
            txtNombreTitular.Text = datosTrabajador.titular;

            if (!string.IsNullOrEmpty(datosTrabajador.PaisOrigen))
                txtPaisOrigen.Value = datosTrabajador.PaisOrigen;
            else
                txtPaisOrigen.Value = "";

            if (!string.IsNullOrEmpty(datosTrabajador.Nacionalidad))
                txtNacionalidad.Value = datosTrabajador.Nacionalidad;
            else
                txtNacionalidad.Value = "";

            cboEtniaRaza.SelectedValue = datosTrabajador.i_EtniaRaza == null ? -1 : datosTrabajador.i_EtniaRaza;
            cboMigrante.SelectedValue = datosTrabajador.i_Migrante == null ? -1 : datosTrabajador.i_Migrante;
           
            if (!string.IsNullOrEmpty(datosTrabajador.Puesto))
            {
                txtPuesto.Value = datosTrabajador.Puesto;
            }
            else
                txtPuesto.Value = "";

            if (!string.IsNullOrEmpty(datosTrabajador.CelularEmergencia))
            {
                txtPhoneEmergency.Text = datosTrabajador.CelularEmergencia;
            }
            else
                txtPhoneEmergency.Text = "";

            if (!string.IsNullOrEmpty(datosTrabajador.ContactoEmergencia))
            {
                txtContactEmergency.Value = datosTrabajador.ContactoEmergencia;
            }
            else
                txtContactEmergency.Value = "";

            cboTipoSeguro.SelectedValue = datosTrabajador.TipoSeguro;

            _personId = datosTrabajador.PersonId;
        }

        private void cboDistrito_Leave(object sender, EventArgs e)
        {

            if (cboDistrito.Text == "--Seleccionar--") return;

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

        private void cboTipoServicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTipoServicio.SelectedIndex == 0 || cboTipoServicio.SelectedIndex == -1)
                cboServicio.DataSource = new AgendaBl().LlenarComboServicio(cboServicio, 1000);
            else
            {
                cboServicio.DataSource = new AgendaBl().LlenarComboServicio(cboServicio, int.Parse(cboTipoServicio.SelectedValue.ToString()));
             
                if (int.Parse(cboTipoServicio.SelectedValue.ToString()) == 9 || int.Parse(cboTipoServicio.SelectedValue.ToString()) == 11 || int.Parse(cboTipoServicio.SelectedValue.ToString()) == 12 || int.Parse(cboTipoServicio.SelectedValue.ToString()) == 13 || int.Parse(cboTipoServicio.SelectedValue.ToString()) == 34)
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

        private void btnBuscarTrabajador_Click(object sender, EventArgs e)
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
                _sexTypeId = Convert.ToInt32(datosTrabajador.GeneroId);
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
                }
                
                

            }
        }

        private void LimpiarDatos()
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
            txtMail.Text = "";
            txtAdressLocation.Text = "";
            txtTelephoneNumber.Text = "";
            cboParentesco.SelectedValue = -1;
            txtResidenciaAnte.Text = "";
            txtNacionalidad.Text = "";
            txtReligion.Text = "";
            FingerPrintTemplate = null;
            FingerPrintImage = null;
            RubricImage = null;
            RubricImageText = null;
            cboNivelEstudio.SelectedValue = 5;
            cboEstablecimiento.SelectedValue = -1;
            cboVendedorExterno.SelectedValue = 1;
            cboMedicoSolicitanteExterno.SelectedValue = 1;

            pbPersonImage.Image = SAMBHS.Windows.SigesoftIntegration.UI.Properties.Resources.usuario;
            //pbPersonImage.Image = null;
            //pbPersonImage.ImageLocation = Application.StartupPath + "\\Resources\\usuario.jpg";
            txtNombreTitular.Text = "";
            _personId = null;
        }

        private void txtSearchNroDocument_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtSearchNroDocument_TextChanged(object sender, EventArgs e)
        {
            btnBuscarTrabajador.Enabled = (txtSearchNroDocument.TextLength > 0);
        }

        private void btnRecargarEmpresa_Click(object sender, EventArgs e)
        {
            cboEmpresaFacturacion.DataSource = null;
            cboEmpresaFacturacion.Items.Clear();
            cboEmpresaFacturacion.DataSource = new EmpresaBl().GetOrganizationFacturacion(cboEmpresaFacturacion, 9);
            cboEmpresaFacturacion.DisplayMember = "Value1";
            cboEmpresaFacturacion.ValueMember = "Id";
            cboEmpresaFacturacion.SelectedIndex = 0;
        }

        private void btnNuevoRegistro_Click(object sender, EventArgs e)
        {
            frmNuevoRegistro frm = new frmNuevoRegistro();
            frm.ShowDialog();
            var listaPaciente = AgendaBl.ObtenerPacientes();
            txtNombreTitular.DataSource = listaPaciente;
        }

        private void btnSavePacient_Click(object sender, EventArgs e)
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

                return ;
            }

            if (cboDistrito.SelectedValue.ToString() == "-1")
            {
                MessageBox.Show(@"Debe Seleccionar Distrito", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

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
            var listaPaciente = AgendaBl.ObtenerPacientes();
            txtNombreTitular.DataSource = listaPaciente;
        }

        private void GrabarTrabajadorNuevo()
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
            oPersonDto.Email = txtMail.Text.Trim();
            oPersonDto.Direccion = txtAdressLocation.Text.Trim();
            oPersonDto.Puesto = txtPuesto.Text.Trim();
            oPersonDto.Estudios = cboNivelEstudio.SelectedValue == null ? -1 : int.Parse(cboNivelEstudio.SelectedValue.ToString());
            oPersonDto.Telefono = txtTelephoneNumber.Text.Trim();
            oPersonDto.Parantesco = cboParentesco.SelectedValue == null ? -1 : int.Parse(cboParentesco.SelectedValue.ToString());
            oPersonDto.Nacionalidad = txtNacionalidad.Text.Trim();
            oPersonDto.ResidenciaAnte = txtResidenciaAnte.Text.Trim();
            oPersonDto.Religion = txtReligion.Text.Trim();
            oPersonDto.titular = txtNombreTitular.Text.Trim();
            oPersonDto.ContactoEmergencia = txtContactEmergency.Text.Trim();
            oPersonDto.CelularEmergencia = txtPhoneEmergency.Text.Trim();
            oPersonDto.TipoSeguro = cboTipoSeguro.SelectedValue == null ? -1 : int.Parse(cboTipoSeguro.SelectedValue.ToString());

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
                _personId = AgendaBl.UpdatePerson_Asist(oPersonDto, _personId);
                resultadoSMS = InsertarUsuario(_personId, oPersonDto);
            }
            else
            {
                oPersonDto.Reside = 1;
                oPersonDto.Altitud = 1;
                oPersonDto.Minerales = "---";
                oPersonDto.Grupo = -1;
                oPersonDto.Factor = -1;
                oPersonDto.TiempoResidencia = "---";
                //oPersonDto.TipoSeguro = 1;
                oPersonDto.Vivos = 0;
                oPersonDto.Muertos = 0;
                oPersonDto.Hermanos = 0;
                oPersonDto.Labor = 1;
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

            MessageBox.Show(@"Se grabó correctamente - " + resultadoSMS, @"Información");
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
            else if (usuariograba == 10055)
            {
                usuariograba = 1771;
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
                //return "USUARIO INSERTADO";
            }
            //else
            //{
                return "USUARIO REGISTRADO";
            //}




            //if (_PersonDto.Telefono != null || _PersonDto.Telefono != "")
            //{
            //    return ConsumirPostSMS(_PersonDto.Nombres.Trim().Split(' ')[0] + " " + _PersonDto.ApellidoPaterno.Trim(),
            //        txtNombres.Text.Trim().Split(' ')[0].ToLower() + "." + txtApellidoPaterno.Text.Trim().Replace(" ", "").ToLower(),
            //        _PersonDto.NroDocumento, _PersonDto.Telefono);
            //}
            //else
            //{
                
            //}

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
        static void ConsumirPostSMS()
        {
            WebRequest request = WebRequest.Create("https://apismscsl.somee.com/home/smspost");
            request.Method = "POST";
            request.ContentType = "application/json; charset=UTF-8";

            string json = "{\"v_phoneNummber\":\"950687514\",\"v_NameClient\":\"ARNOLD ODAR\",\"v_user\":\"arnold.odar\"},\"v_pass\":\"72087308\"},\"i_status\":\"4\"}";
            var byteArray = Encoding.UTF8.GetBytes(json);
            request.ContentLength = byteArray.Length;

            Stream stream = request.GetRequestStream();
            stream.Write(byteArray, 0, byteArray.Length);
            stream.Close();

            var response = (HttpWebResponse)request.GetResponse();
            string respuesta = "";
            if (response.StatusCode == HttpStatusCode.Created)
            {
                respuesta = "CREADO";
            }
            else
            {
                respuesta = "NO CREADO";
            }


        }
        private void btnCapturedFingerPrintAndRubric_Click(object sender, EventArgs e)
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

        private void btnschedule_Click(object sender, EventArgs e)
        {
            if (cboVendedorExterno.SelectedValue != null)
            {
                if (cboVendedorExterno.SelectedValue.ToString() == "-1")
                {
                    MessageBox.Show(@"Debe Seleccionar Vendedor Externo para atención", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            if (cboMedicoSolicitanteExterno.SelectedValue != null)
            {
                if (cboMedicoSolicitanteExterno.SelectedValue.ToString() == "-1")
                {
                    MessageBox.Show(@"Debe Seleccionar Médico Externo para atención", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }

            if (cboMedicoSolicitanteExterno.SelectedValue.ToString() != "-1")
            {
                if (cboEstablecimiento.SelectedValue != null)
                {
                    if (cboEstablecimiento.SelectedValue.ToString() == "-1")
                    {
                        MessageBox.Show(@"Debe Seleccionar Establecimiento para atención", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
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

            if (string.IsNullOrEmpty(_personId))
            {
                MessageBox.Show("Tiene que grabar al paciente antes de agendar.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cboEmpresaFacturacion.SelectedValue == "-1")
            {
                MessageBox.Show("Tiene que seleccionar facturacion de destino.", "ADVERTENCIA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            AgendarServicio(Globals.ClientSession.GetAsList());
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

        private void AgendarServicio(List<string> ClientSession)
        {
            var oServiceDto = OServiceDto();
            int atenciondia = 0;
            //if (checkDia.Checked == true)
            //{
                atenciondia = 0;
            //}

            int medicoPagado = 0;
            if (checkMedicoPagado.Checked == true)
            {
                medicoPagado = 1;
            }
            AgendaBl.SheduleServiceAtx(oServiceDto, Int32.Parse(ClientSession[2]), atenciondia, medicoPagado);
        }

        private ServiceDto OServiceDto()
        {
            int establecimiento = 0;
            if (cboEstablecimiento.SelectedValue == null)
            {
                establecimiento = AgendaBl.CreateItemEstablecimiento(cboEstablecimiento.Text);
            }
            else
            {
                establecimiento = int.Parse(cboEstablecimiento.SelectedValue.ToString());
            }

            int vendedorExterno = -1;
            if (cboVendedorExterno.SelectedValue == null)
            {
                vendedorExterno = AgendaBl.CreateItemVendedorExterno(cboVendedorExterno.Text);
            }
            else
            {
                vendedorExterno = int.Parse(cboVendedorExterno.SelectedValue.ToString());
            }

            int medicoExterno = -1;
            if (cboMedicoSolicitanteExterno.SelectedValue == null)
            {
                medicoExterno = AgendaBl.CreateItemMedicoExterno(cboMedicoSolicitanteExterno.Text);
            }
            else
            {
                medicoExterno = int.Parse(cboMedicoSolicitanteExterno.SelectedValue.ToString());
            }

            var horaAtencion = cboTurno.SelectedValue.ToString() == "-1" ? dtTimaCalendar.Value.TimeOfDay : DateTime.Parse(cboHorario.Text.ToString().Split('-')[0].Trim(), System.Globalization.CultureInfo.CurrentCulture).TimeOfDay;
            int zona = 0;
            
            var oServiceDto = new ServiceDto
            {
                ProtocolId = cboProtocolo.SelectedValue.ToString(),
                PersonId = _personId,
                MasterServiceId = int.Parse(cboServicio.SelectedValue.ToString()),
                ServiceTypeId = int.Parse(cboTipoServicio.SelectedValue.ToString()),
                ServiceStatusId = (int) ServiceStatus.Iniciado,
                AptitudeStatusId = (int) AptitudeStatus.SinAptitud,
                //ServiceDate = null,
                ServiceDate = dtDateCalendar.Value.Date + horaAtencion,
                GlobalExpirationDate = null,
                ObsExpirationDate = null,
                FlagAgentId = 1,
                OrganizationId = cboEmpresaFacturacion.SelectedValue.ToString(),
                Motive = string.Empty,
                IsFac = 0,
                FechaNacimiento = _fechaNacimiento,
                GeneroId = _sexTypeId,
                MedicoTratanteId = int.Parse(cboMedicoTratante.SelectedValue.ToString()),
                MedicoRealizaId = 11,
                v_centrocosto = "- - -",
                Plan = int.Parse(cboPlanAtencion.SelectedValue.ToString()),
                v_LicenciaConducir = _tipoAtencion == 4?txtLicenciadeConducir.Text:string.Empty,
                Establecimiento = establecimiento,
                VendedorExterno = vendedorExterno,
                MedicoSolicitanteExterno = medicoExterno,

                ObservacionesAtencion = txtDetalleAtencion.Text,
                PacienteHospSala = cboTipoHosp.SelectedValue.ToString(), 
                PasoSop = checkSop.Checked == true ? 1:0,
                PasoHosp = checkHospi.Checked == true ?1:0,
                i_ProcedenciaPac_Mkt = cboMarketing.SelectedValue == null ? -1 : int.Parse(cboMarketing.SelectedValue.ToString()),
                _idccEditarNew = _idccEditar,

                i_MedicoAtencion = int.Parse(cboMedicoTratante.SelectedValue.ToString()),
                i_CodigoAtencion = int.Parse(cboHorario.SelectedValue.ToString()),
                i_GrupoAtencion = grupo,

            };
            return oServiceDto;
        }
        private void cboProtocolo_Click(object sender, EventArgs e)
        {
            var selectedValue = int.Parse(cboServicio.SelectedValue.ToString());
            var selectedType = int.Parse(cboTipoServicio.SelectedValue.ToString());

            string filtroEspecialidad = "";

            if (cboEspecialidad.SelectedValue.ToString() != "-1")
            {
                filtroEspecialidad = cboEspecialidad.Text;
            }

            if (_contrata == "")
            {
                AgendaBl.LlenarComboProtocolo_Particular(cboProtocolo, int.Parse(cboServicio.SelectedValue.ToString()), int.Parse(cboTipoServicio.SelectedValue.ToString()), _empresa, filtroEspecialidad);

                listaProtoColos = AgendaBl.LlenarComboProtocolo_Particular_new(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);
            }
            else
            {
                AgendaBl.LlenarComboProtocolo_Seguros(cboProtocolo, int.Parse(cboTipoServicio.SelectedValue.ToString()), int.Parse(cboServicio.SelectedValue.ToString()), _empresa, _contrata);

                listaProtoColos = AgendaBl.LlenarComboProtocolo_Particular_new(cboProtocolo, selectedValue, selectedType, _empresa, filtroEspecialidad);
            }

        }


        private void cboProtocolo_SelectedValueChanged(object sender, EventArgs e)
        {
            //string protocolo;
            ////int id;
            ////if (!int.TryParse(cboProtocolo.SelectedValue.ToString(), out id)) return;
            //if (_tipoAtencion == 3)
            //{
            //    if (cboProtocolo.SelectedIndex == -1) return;
            //    protocolo = cboProtocolo.SelectedValue.ToString();
            //    if (protocolo != "SAMBHS.Windows.SigesoftIntegration.UI.EsoDto")
            //    {
            //        AgendaBl.LlenarComboPlanes(cboPlanAtencion, protocolo);
            //    }
            //}
        }

        private void cboProtocolo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string protocolo;
            //int id;
            //if (!int.TryParse(cboProtocolo.SelectedValue.ToString(), out id)) return;
            if (_tipoAtencion == 3)
            {
                if (cboProtocolo.SelectedIndex == -1) return;
                protocolo = cboProtocolo.SelectedValue.ToString();
                if (protocolo != "SAMBHS.Windows.SigesoftIntegration.UI.EsoDto")
                {
                    AgendaBl.LlenarComboPlanes(cboPlanAtencion, protocolo);
                }
            }
            
        }

        private void cboPlanAtencion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string plan;

            if (cboPlanAtencion.SelectedIndex == 0) return;

            plan = cboPlanAtencion.SelectedValue.ToString();
            if (plan != "SAMBHS.Windows.SigesoftIntegration.UI.EsoDto")
            {
                var datosPlan = AgendaBl.DatosSeguro_(plan);

                txtDeducible.Text = "S/. " + datosPlan.Deducible.ToString();
                txtCoaseguro.Text = datosPlan.Coaseguro.ToString() + " %";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboEspecialidad.SelectedValue != null)
                {
                    if (cboEspecialidad.SelectedValue.ToString() != "-1")
                    {
                        //cboEspecialidad.Text
                        listaProtoColos = new List<EsoDtoProt>();
                        cboProtocolo.DataSource = null;
                        cboProtocolo.Items.Clear();
                        cboProtocolo.DataSource = listaProtoColos =
                            new AgendaBl().LlenarComboProtocoloFiltrado(cboProtocolo,
                                Convert.ToInt32(cboTipoServicio.SelectedValue),
                                Convert.ToInt32(cboServicio.SelectedValue), cboEspecialidad.Text);
                        cboProtocolo.DisplayMember = "Nombre";
                        cboProtocolo.ValueMember = "Id";
                        cboProtocolo.SelectedIndex = 0;
                    }
                    else
                    {
                        listaProtoColos = new List<EsoDtoProt>();
                        cboProtocolo.DataSource = null;
                        cboProtocolo.Items.Clear();
                        cboProtocolo.DataSource = listaProtoColos = new AgendaBl().LlenarComboProtocolo(cboProtocolo, Convert.ToInt32(cboTipoServicio.SelectedValue), Convert.ToInt32(cboServicio.SelectedValue));
                        cboProtocolo.DisplayMember = "Nombre";
                        cboProtocolo.ValueMember = "Id";
                        cboProtocolo.SelectedIndex = 0;
                    }
                }
                else
                {
                    listaProtoColos = new List<EsoDtoProt>();
                    cboProtocolo.DataSource = null;
                    cboProtocolo.Items.Clear();
                    cboProtocolo.DataSource = listaProtoColos = new AgendaBl().LlenarComboProtocolo(cboProtocolo, Convert.ToInt32(cboTipoServicio.SelectedValue), Convert.ToInt32(cboServicio.SelectedValue));
                    cboProtocolo.DisplayMember = "Nombre";
                    cboProtocolo.ValueMember = "Id";
                    cboProtocolo.SelectedIndex = 0;
                }

            }
            catch (Exception a)
            {
                MessageBox.Show(a.ToString(), "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void checkDia_CheckedChanged(object sender, EventArgs e)
        {
            //if (checkDia.Checked == true)
            //{
            //    dtDateCalendar.Enabled = false;
            //    dtTimaCalendar.Enabled = false;
            //}
            //else
            //{
            //    dtDateCalendar.Enabled = true;
            //    dtTimaCalendar.Enabled = true;
            //}
        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void cboServicio_SelectedValueChanged(object sender, EventArgs e)
        {
            if (int.Parse(cboServicio.SelectedValue.ToString()) == 13 ||
                int.Parse(cboServicio.SelectedValue.ToString()) == 19 || int.Parse(cboServicio.SelectedValue.ToString()) == 29 ||
                int.Parse(cboServicio.SelectedValue.ToString()) == 30 )
            {
                cboTipoHosp.Visible = true;
                label29.Visible = true;
                checkHospi.Visible = true;
                checkSop.Visible = true;
            }
            else
            {
                cboTipoHosp.Visible = false;
                label29.Visible = false;
                checkHospi.Visible = false;
                checkSop.Visible = false;
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

        private void cboMedicoTratante_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboMedicoTratante.SelectedValue != null && cboMedicoTratante.SelectedValue.ToString() != "-1" && cboMedicoTratante.SelectedValue.ToString() != "SAMBHS.Common.Resource.KeyValueDTO")
            {
                string cal = dtDateCalendar.Value.ToString("yyyy-MM-dd");

                AgendaBl.LlenarComboTurno(cboTurno, cboMedicoTratante.SelectedValue.ToString(), cal);
            }
            else
            {
                cboTurno.SelectedValue = "-1";

            }
        }

        private void dtDateCalendar_ValueChanged(object sender, EventArgs e)
        {
            if (cboMedicoTratante.SelectedValue != null && cboMedicoTratante.SelectedValue.ToString() != "-1" && cboMedicoTratante.SelectedValue.ToString() != "SAMBHS.Common.Resource.KeyValueDTO")
            {
                string cal = dtDateCalendar.Value.ToString("yyyy-MM-dd");

                grupo = AgendaBl.LlenarComboTurno(cboTurno, cboMedicoTratante.SelectedValue.ToString(), cal);
            }
            else
            {

                grupo = AgendaBl.LlenarComboTurno(cboTurno, "-1", DateTime.Now.ToString("yyyy-MM-dd"));
                AgendaBl.LlenarComboHorario(cboHorario, "-1", "-1", DateTime.Now.ToString("yyyy-MM-dd"));
                cboTurno.SelectedValue = "-1";
                cboHorario.SelectedValue = "-1";

            }
        }

        private void cboTurno_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboTurno.SelectedValue != null && cboTurno.SelectedValue.ToString() != "-1" && cboTurno.SelectedValue.ToString() != "SAMBHS.Common.Resource.KeyValueDTO")
            {
                string cal = dtDateCalendar.Value.ToString("yyyy-MM-dd");
                AgendaBl.LlenarComboHorario(cboHorario, cboMedicoTratante.SelectedValue.ToString(), cboTurno.SelectedValue.ToString(), cal);
                dtTimaCalendar.Visible = false;
                label18.Visible = false;
            }
            else
            {
                cboHorario.SelectedValue = "-1";
                dtTimaCalendar.Visible = true;
                label18.Visible = true;
            }
        }

        private void cboHorario_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private void cboEspecialidad_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboEspecialidad.SelectedValue.ToString() != "-1")
                {
                    var KeyValueDtos_ = new AgendaBl().LlenarComboUsuarios(cboMedicoTratante);

                    var dataMedicoTratante = KeyValueDtos_.FindAll(x => x.Value2 == cboEspecialidad.Text || x.Value2 == "- - -");
                    cboMedicoTratante.Invoke((Action)(() =>
                    {
                        ConfigureComboKeyValue(cboMedicoTratante, dataMedicoTratante);
                    }));

                }
                else
                {
                    var KeyValueDtos_ = new AgendaBl().LlenarComboUsuarios(cboMedicoTratante);

                    var dataMedicoTratante = KeyValueDtos_;
                    cboMedicoTratante.Invoke((Action)(() =>
                    {
                        ConfigureComboKeyValue(cboMedicoTratante, dataMedicoTratante);
                    }));

                }
            }
            catch (Exception)
            {

                throw;
            } 
        }

        private void ConfigureComboKeyValue(ComboBox combo, List<KeyValueDTO> data)
        {
            data.Insert(0, new KeyValueDTO { Id = "-1", Value1 = "--Seleccionar--" });
            combo.DataSource = data;
            combo.DisplayMember = "Value1";
            combo.ValueMember = "Id";
            combo.SelectedIndex = 0;
        }
    }
}
