using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win.UltraWinSchedule;
using SAMBHS.Windows.SigesoftIntegration.UI;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using System.Threading.Tasks;
using SAMBHS.Common.BE;
using Task = System.Threading.Tasks.Task;
using ScrapperReniecSunat;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Sigesoft
{
    public partial class frmAgendarTrabajador : Form
    {
        private string _personId;
        private DateTime? _fechaNacimiento;
        private int _sexTypeId;
        private AgendaBl agendaBl_ = new AgendaBl();
        public frmAgendarTrabajador()
        {
            InitializeComponent();
        }

        private void frmAgendarTrabajador_Load(object sender, EventArgs e)
        {
            agendaBl_.LlenarComboTipoDocumento(cboTipoDocumento);
            cboGenero.DataSource = agendaBl_.LlenarComboGennero(cboGenero);
            cboEstadoCivil.DataSource = agendaBl_.LlenarComboEstadoCivil(cboEstadoCivil);
            cboTipoServicio.DataSource = agendaBl_.LlenarComboTipoServicio(cboTipoServicio);
            cboNivelEstudio.DataSource = agendaBl_.LlenarComboNivelEstudio(cboNivelEstudio);
            cboResidencia.DataSource = agendaBl_.LlenarComboResidencia(cboResidencia);
            cboAltitud.DataSource = agendaBl_.LlenarComboAltitud(cboAltitud);
            cboTipoSeguro.DataSource = agendaBl_.LlenarComboTipoSeguro(cboTipoSeguro);
            cboParentesco.DataSource = agendaBl_.LlenarComboParentesco(cboParentesco);
            cboLugarLabor.DataSource = agendaBl_.LlenarComboLugarLabor(cboLugarLabor);
            cboDistrito.DataSource = agendaBl_.LlenarComboDistrito(cboDistrito);
            cboProvincia.DataSource = agendaBl_.LlenarComboDistrito(cboProvincia);
            cboDepartamento.DataSource = agendaBl_.LlenarComboDistrito(cboDepartamento);
            cboGrupo.DataSource = agendaBl_.LlenarComboGrupo(cboGrupo);
            cboFactorSan.DataSource = agendaBl_.LlenarComboFactor(cboFactorSan);
            cboTipoDocumento.SelectedValue = 1;
            cboGenero.SelectedValue = 1;
            cboEstadoCivil.SelectedValue = 1;
            cboResidencia.SelectedValue = 0;
            cboNivelEstudio.SelectedValue = 5;
            cboTipoSeguro.SelectedValue = 1;
            cboTipoServicio.SelectedValue = 1;
            cboServicio.SelectedValue = 2;
            var lista = AgendaBl.ObtenerPuestos();
            txtPuesto.DataSource = lista;
            txtPuesto.DisplayMember = "Puesto";
            txtPuesto.ValueMember = "Puesto";

            txtPuesto.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            txtPuesto.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
            this.txtPuesto.DropDownWidth = 250;
            txtPuesto.DisplayLayout.Bands[0].Columns[0].Width = 10;
            txtPuesto.DisplayLayout.Bands[0].Columns[1].Width = 250;
            if (!string.IsNullOrEmpty(""))
            {
                txtPuesto.Value = "";
            }


        }

        private void btnschedule_Click(object sender, EventArgs e)
        {
            AgendarServicio();
        }

        public BindingList<ventadetalleDto> ListadoVentaDetalle = new BindingList<ventadetalleDto>();
        private void AgendarServicio()
        {
            var oServiceDto = OServiceDto();
            Task.Factory.StartNew(() =>
            {
                ListadoVentaDetalle = AgendaBl.SheduleService(oServiceDto, 11);
            }).ContinueWith(t =>
            {
                if (!ListadoVentaDetalle.Any()) return;
                DialogResult = DialogResult.OK;
                Close();
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }

        private ServiceDto OServiceDto()
        {
            var oServiceDto = new ServiceDto
            {
                ProtocolId = cboProtocolo.SelectedValue.ToString(),
                PersonId = _personId,
                MasterServiceId = int.Parse(cboServicio.SelectedValue.ToString()),
                ServiceStatusId = (int) ServiceStatus.Iniciado,
                AptitudeStatusId = (int) AptitudeStatus.SinAptitud,
                ServiceDate = null,
                GlobalExpirationDate = null,
                ObsExpirationDate = null,
                FlagAgentId = 1,
                Motive = string.Empty,
                IsFac = 0,
                FechaNacimiento =_fechaNacimiento,
                GeneroId =_sexTypeId,
                PacienteHospSala = "0"
            };
            return oServiceDto;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

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
                                txtNroDocumento.Text = txtSearchNroDocument.Text;
                                txtNombres.Text = datos.Nombre;
                                txtApellidoPaterno.Text = datos.ApellidoPaterno;
                                txtApellidoMaterno.Text = datos.ApellidoMaterno;
                                dtpBirthdate.Value = datos.FechaNacimiento;
                            }
                        }
                        break;
                }
            }
            else
                MessageBox.Show("No se pudo conectar la página", "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        private void txtSearchNroDocument_TextChanged(object sender, EventArgs e)
        {
            btnBuscarTrabajador.Enabled = (txtSearchNroDocument.TextLength > 0);
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
        
        private void btnBuscarTrabajador_Click(object sender, EventArgs e)
        {
            var datosTrabajador = AgendaBl.GetDatosTrabajador(txtSearchNroDocument.Text);
            if (datosTrabajador != null)
            {
                LlenarCampos(datosTrabajador);
                _sexTypeId = Convert.ToInt32(datosTrabajador.GeneroId);
                _fechaNacimiento = datosTrabajador.FechaNacimiento;
            }
            else ObtenerDatosDNI(txtSearchNroDocument.Text.Trim()); 
                //TrabajadorNoEncontrado(txtSearchNroDocument.Text);
        }

        private void TrabajadorNoEncontrado(string nrodocumento)
        {
            MessageBox.Show(@"El trabajador con el nro. documento " + nrodocumento + @" no se encuentra en registrado",
                @"Información");
            LimparControlesTrabajador();
            txtNroDocumento.Text = txtSearchNroDocument.Text;
            txtNombres.Focus();
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
            txtNombres.Text = datosTrabajador.Nombres;
            txtApellidoPaterno.Text = datosTrabajador.ApellidoPaterno;
            txtApellidoMaterno.Text = datosTrabajador.ApellidoMaterno;
            cboTipoDocumento.SelectedValue = datosTrabajador.TipoDocumentoId;
            txtNroDocumento.Text = datosTrabajador.NroDocumento;
            cboGenero.SelectedValue = datosTrabajador.GeneroId;
            dtpBirthdate.Value = datosTrabajador.FechaNacimiento.Value;

            cboEstadoCivil.SelectedValue = datosTrabajador.EstadoCivil;
            txtBirthPlace.Text = datosTrabajador.LugarNacimiento;
            cboDistrito.SelectedValue = datosTrabajador.Distrito;
            cboProvincia.SelectedValue = datosTrabajador.Provincia;
            cboDepartamento.SelectedValue = datosTrabajador.Departamento;
            cboResidencia.SelectedValue = datosTrabajador.Reside;
            txtMail.Text = datosTrabajador.Email;
            txtAdressLocation.Text = datosTrabajador.Direccion;
            //txtPuesto.Text = datosTrabajador.Puesto;
            var lista = AgendaBl.ObtenerPuestos();
            txtPuesto.DataSource = lista;
            txtPuesto.DisplayMember = "Puesto";
            txtPuesto.ValueMember = "Puesto";

            txtPuesto.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            txtPuesto.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
            this.txtPuesto.DropDownWidth = 250;
            txtPuesto.DisplayLayout.Bands[0].Columns[0].Width = 10;
            txtPuesto.DisplayLayout.Bands[0].Columns[1].Width = 250;
            if (!string.IsNullOrEmpty(datosTrabajador.Puesto))
            {
                txtPuesto.Value = datosTrabajador.Puesto;
            }

            cboAltitud.SelectedValue = datosTrabajador.Altitud;
            txtExploitedMineral.Text = datosTrabajador.Minerales;
            cboNivelEstudio.SelectedValue = datosTrabajador.Estudios;
            cboGrupo.SelectedValue = datosTrabajador.Grupo;
            cboFactorSan.SelectedValue = datosTrabajador.Factor;
            txtResidenceTimeInWorkplace.Text = datosTrabajador.TiempoResidencia;
            cboTipoSeguro.SelectedValue = datosTrabajador.TipoSeguro;
            txtNumberLivingChildren.Text = datosTrabajador.Vivos.ToString();
            txtNumberDependentChildren.Text = datosTrabajador.Muertos.ToString();
            txtNroHermanos.Text = datosTrabajador.Hermanos.ToString();
            txtTelephoneNumber.Text = datosTrabajador.Telefono;
            cboParentesco.SelectedValue = datosTrabajador.Parantesco;
            cboLugarLabor.SelectedValue = datosTrabajador.Labor;
            
            _personId = datosTrabajador.PersonId;
        }

        private void cboTipoServicio_SelectedIndexChanged(object sender, EventArgs e)
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
            }
        }
        
        private void cboServicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboServicio.SelectedIndex == 0 || cboServicio.SelectedIndex == -1)
            {
                cboProtocolo.DataSource = new AgendaBl().LlenarComboProtocolo(cboProtocolo, 1000, 1000);
                cboProtocolo.SelectedIndex = 0;
            }
            else{
                cboProtocolo.DataSource = new AgendaBl().LlenarComboProtocolo(cboProtocolo, int.Parse(cboTipoServicio.SelectedValue.ToString()), int.Parse(cboServicio.SelectedValue.ToString()));
                cboProtocolo.SelectedIndex = 0;
            }
        }
        
        private void cboProtocolo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProtocolo.SelectedIndex == 0 || cboProtocolo.SelectedIndex == -1)
                LimpiarControlesProtocolo();
            else
                LlenarControlesProtocolo();
        }

        private void LimpiarControlesProtocolo()
        {
            txtGeso.Text = "";
            txtTipoEso.Text = "";
            txtEmpresaCliente.Text = "";
            txtEmpresaEmpleadora.Text = "";
            txtEmpresaTrabajo.Text = "";
        }

        private void LlenarControlesProtocolo()
        {
            var datosProtocolo = AgendaBl.GetDatosProtocolo(cboProtocolo.SelectedValue.ToString());
            txtGeso.Text = datosProtocolo.Geso;
            txtTipoEso.Text = datosProtocolo.TipoEso;
            txtEmpresaCliente.Text = datosProtocolo.EmpresaCliente;
            txtEmpresaEmpleadora.Text = datosProtocolo.EmpresaEmpleadora;
            txtEmpresaTrabajo.Text = datosProtocolo.EmpresaTrabajo;
        }

        private void btnSavePacient_Click(object sender, EventArgs e)
        {
            GrabarTrabajadorNuevo();
        }

        private void GrabarTrabajadorNuevo()
        {
                PersonDto oPersonDto = new PersonDto();

                oPersonDto.Nombres = txtNombres.Text;
                oPersonDto.TipoDocumento = int.Parse(cboTipoDocumento.SelectedValue.ToString());
                oPersonDto.NroDocumento = txtNroDocumento.Text;
                oPersonDto.ApellidoPaterno = txtApellidoPaterno.Text;
                oPersonDto.ApellidoMaterno = txtApellidoMaterno.Text;
                oPersonDto.GeneroId = int.Parse(cboGenero.SelectedValue.ToString());
                oPersonDto.FechaNacimiento = dtpBirthdate.Value;

                oPersonDto.EstadoCivil = cboEstadoCivil.SelectedValue == null ? -1: int.Parse(cboEstadoCivil.SelectedValue.ToString());
                oPersonDto.LugarNacimiento = txtBirthPlace.Text;
                oPersonDto.Distrito = cboDistrito.SelectedValue == null ? -1: int.Parse(cboDistrito.SelectedValue.ToString());
                oPersonDto.Provincia = int.Parse(cboProvincia.SelectedValue.ToString());
                oPersonDto.Departamento = int.Parse(cboDepartamento.SelectedValue.ToString());
                oPersonDto.Reside = int.Parse(cboResidencia.SelectedValue.ToString());
                oPersonDto.Email = txtMail.Text;
                oPersonDto.Direccion = txtAdressLocation.Text;
                oPersonDto.Puesto = txtPuesto.Text;
                oPersonDto.Altitud = cboAltitud.SelectedValue == null ? -1: int.Parse(cboAltitud.SelectedValue.ToString());
                oPersonDto.Minerales = txtExploitedMineral.Text;

                oPersonDto.Estudios = cboNivelEstudio.SelectedValue  == null ? -1 : int.Parse(cboNivelEstudio.SelectedValue.ToString());
                oPersonDto.Grupo = int.Parse(cboGrupo.SelectedValue.ToString());
                oPersonDto.Factor = int.Parse(cboFactorSan.SelectedValue.ToString());
                oPersonDto.TiempoResidencia = txtResidenceTimeInWorkplace.Text;
                oPersonDto.TipoSeguro = cboTipoSeguro.SelectedValue== null ?-1: int.Parse(cboTipoSeguro.SelectedValue.ToString());
                oPersonDto.Vivos = int.Parse(txtNumberLivingChildren.Text.ToString());
                oPersonDto.Muertos = int.Parse(txtNumberDependentChildren.Text.ToString());
                oPersonDto.Hermanos = int.Parse(txtNroHermanos.Text.ToString());
                oPersonDto.Telefono = txtTelephoneNumber.Text;
                oPersonDto.Parantesco = cboParentesco.SelectedValue == null?-1: int.Parse(cboParentesco.SelectedValue.ToString());
                oPersonDto.Labor = cboLugarLabor.SelectedValue == null?-1: int.Parse(cboLugarLabor.SelectedValue.ToString());

                if (_personId != null)
                {
                    _personId = AgendaBl.UpdatePerson(oPersonDto, _personId);
                }
                else
                {
                    _personId = AgendaBl.AddPerson(oPersonDto);
                    if (_personId == "El paciente ya se encuentra registrado")
                {
                    MessageBox.Show(@"Este paciente ya se encu", @"Información");
                    return;
                } 
            }
            _sexTypeId = oPersonDto.GeneroId;
            _fechaNacimiento = oPersonDto.FechaNacimiento;
            MessageBox.Show(@"Se grabó correctamente", @"Información");
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
    }
}
