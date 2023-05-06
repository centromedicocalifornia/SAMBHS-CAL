using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using System.Drawing.Imaging;
using Sigesoft.Node.WinClient.UI;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Windows.SigesoftIntegration.UI.BLL;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmEditarTrabajador : Form
    {
        private string _nroDocumento;
        private string _serviceId;
        private string _personId;
        string Mode;
        private string _fileName;
        private string _filePath;
        private string _empresaFacturacion;

        private AgendaBl agendaBl_ = new AgendaBl();
        private EmpresaBl empresaBl_ = new EmpresaBl();
        #region Properties


        public byte[] FingerPrintTemplate { get; set; }

        public byte[] FingerPrintImage { get; set; }

        public byte[] RubricImage { get; set; }

        public string RubricImageText { get; set; }

        #endregion

        #region GetChanges
        public class Campos
        {
            public string NombreCampo { get; set; }
            public string ValorCampo { get; set; }
        }
        string[] nombreCampos =
        {

            "txtNombres", "txtApellidoPaterno", "txtApellidoMaterno", "dtpBirthdate", "cboTipoDocumento", "txtNroDocumento", "cboGenero", "cboEstadoCivil",
            "cboNivelEstudio", "txtBirthPlace", "cboDistrito", "cboProvincia", "txtResidenceTimeInWorkplace", "cboDepartamento", "cboTipoSeguro", "cboResidencia",
            "txtNumberLivingChildren", "txtNumberDependentChildren", "txtMail", "txtNroHermanos", "txtTelephoneNumber", "txtAdressLocation",
            "txtPuesto", "cboParentesco", "cboAltitud", "cboLugarLabor", "txtExploitedMineral", "txtNacionalidad", "txtResidenciaAnte", "txtReligion",

        };

        private List<Campos> ListValuesCampo = new List<Campos>();

        private string GetChanges()
        {
            string cadena = new PacientBL().GetComentaryUpdateByPersonId(_personId);
            string oldComentary = cadena;
            cadena += "<FechaActualiza:" + DateTime.Now.ToString() + "|UsuarioActualiza:" + Globals.ClientSession.v_UserName + "|";
            bool change = false;
            foreach (var item in nombreCampos)
            {
                var fields = this.Controls.Find(item, true);
                string keyTagControl;
                string value1;

                if (fields.Length > 0)
                {
                    keyTagControl = fields[0].GetType().Name;
                    value1 = GetValueControl(keyTagControl, fields[0]);

                    var ValorCampo = ListValuesCampo.Find(x => x.NombreCampo == item).ValorCampo;
                    if (ValorCampo != value1)
                    {
                        cadena += item + ":" + ValorCampo + "|";
                        change = true;
                    }
                }
            }
            if (change)
            {
                return cadena;
            }

            return oldComentary;
        }

        private void SetOldValues()
        {
            ListValuesCampo = new List<Campos>();
            string keyTagControl = null;
            string value1 = null;
            foreach (var item in nombreCampos)
            {

                var fields = this.Controls.Find(item, true);

                if (fields.Length > 0)
                {
                    keyTagControl = fields[0].GetType().Name;
                    value1 = GetValueControl(keyTagControl, fields[0]);

                    Campos _Campo = new Campos();
                    _Campo.NombreCampo = item;
                    _Campo.ValorCampo = value1;
                    ListValuesCampo.Add(_Campo);
                }
            }
        }

        private string GetValueControl(string ControlId, Control ctrl)
        {
            string value1 = null;

            switch (ControlId)
            {
                case "TextBox":
                    value1 = ((TextBox)ctrl).Text;
                    break;
                case "ComboBox":
                    value1 = ((ComboBox)ctrl).Text;
                    break;
                case "CheckBox":
                    value1 = Convert.ToInt32(((CheckBox)ctrl).Checked).ToString();
                    break;
                case "RadioButton":
                    value1 = Convert.ToInt32(((RadioButton)ctrl).Checked).ToString();
                    break;
                case "DateTimePicker":
                    value1 = ((DateTimePicker)ctrl).Text; ;
                    break;
                case "UltraCombo":
                    value1 = ((UltraCombo)ctrl).Text; ;
                    break;
                default:
                    break;
            }

            return value1;
        }

        #endregion
        
        public frmEditarTrabajador(string pstrNorDoc, string pstrPersonId, string pstrServiceId)
        {
            _serviceId = pstrServiceId;
            _nroDocumento = pstrNorDoc;
            _personId = pstrPersonId;
            InitializeComponent();
        }

        private void btnSavePacient_Click(object sender, EventArgs e)
        {
            if (cboDistrito.SelectedValue.ToString() == "-1")
            {
                MessageBox.Show(@"Debe Seleccionar Distrito", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cboProvincia.SelectedValue.ToString() == "-1")
            {
                MessageBox.Show(@"Debe Seleccionar Provincia", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cboDepartamento.SelectedValue.ToString() == "-1")
            {
                MessageBox.Show(@"Debe Seleccionar Departamentoo", @"Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            GrabarTrabajadorNuevo();
            DialogResult = DialogResult.OK;
        }

        private void GrabarTrabajadorNuevo()
        {

            PersonDto oPersonDto = new PersonDto();
            ServiceDto oService = new ServiceDto();
            
            oPersonDto.Nombres = txtNombres.Text;
            oPersonDto.TipoDocumento = int.Parse(cboTipoDocumento.SelectedValue.ToString());
            oPersonDto.NroDocumento = txtNroDocumento.Text;
            oPersonDto.ApellidoPaterno = txtApellidoPaterno.Text;
            oPersonDto.ApellidoMaterno = txtApellidoMaterno.Text;
            oPersonDto.GeneroId = int.Parse(cboGenero.SelectedValue.ToString());
            oPersonDto.FechaNacimiento = dtpBirthdate.Value;
            oPersonDto.CommentaryUpdate = GetChanges();
            oPersonDto.EstadoCivil = cboEstadoCivil.SelectedValue == null ? -1 : int.Parse(cboEstadoCivil.SelectedValue.ToString());
            oPersonDto.LugarNacimiento = txtBirthPlace.Text;
            oPersonDto.Distrito = cboDistrito.SelectedValue == null ? -1 : int.Parse(cboDistrito.SelectedValue.ToString());
            oPersonDto.Provincia = int.Parse(cboProvincia.SelectedValue.ToString());
            oPersonDto.Departamento = int.Parse(cboDepartamento.SelectedValue.ToString());
            oPersonDto.Reside = int.Parse(cboResidencia.SelectedValue.ToString());
            oPersonDto.Email = txtMail.Text;
            oPersonDto.Direccion = txtAdressLocation.Text;
            oPersonDto.Puesto = txtPuesto.Text;
            oPersonDto.Altitud = cboAltitud.SelectedValue == null ? -1 : int.Parse(cboAltitud.SelectedValue.ToString());
            oPersonDto.Minerales = txtExploitedMineral.Text;
            oPersonDto.ContactoEmergencia = txtContactEmergency.Text;
            oPersonDto.CelularEmergencia = txtPhoneEmergency.Text;

            oPersonDto.Estudios = cboNivelEstudio.SelectedValue == null ? -1 : int.Parse(cboNivelEstudio.SelectedValue.ToString());
            oPersonDto.Grupo = int.Parse(cboGrupo.SelectedValue.ToString());
            oPersonDto.Factor = int.Parse(cboFactorSan.SelectedValue.ToString());
            oPersonDto.TiempoResidencia = txtResidenceTimeInWorkplace.Text;
            oPersonDto.TipoSeguro = cboTipoSeguro.SelectedValue == null ? -1 : int.Parse(cboTipoSeguro.SelectedValue.ToString());
            oPersonDto.Vivos = int.Parse(txtNumberLivingChildren.Text.ToString());
            oPersonDto.Muertos = int.Parse(txtNumberDependentChildren.Text.ToString());
            oPersonDto.Hermanos = int.Parse(txtNroHermanos.Text.ToString());
            oPersonDto.Telefono = txtTelephoneNumber.Text;
            oPersonDto.Parantesco = cboParentesco.SelectedValue == null ? -1 : int.Parse(cboParentesco.SelectedValue.ToString());
            oPersonDto.Labor = cboLugarLabor.SelectedValue == null ? -1 : int.Parse(cboLugarLabor.SelectedValue.ToString());
            oPersonDto.ResidenciaAnte = txtResidenciaAnte.Text;
            oPersonDto.Religion = txtReligion.Text;
            oPersonDto.Nacionalidad = txtNacionalidad.Text;
            oService.OrganizationId = cboEmpresaFacturacion.SelectedValue.ToString();
            
            string commentary = ProtocoloBl.GetCommentaryUpdateByserviceId(_serviceId);
            
            if (_empresaFacturacion != cboEmpresaFacturacion.Text)
            {

                commentary += "<FechaActualiza:" + DateTime.Now.ToString() + "|UsuarioActualiza:" + Globals.ClientSession.v_UserName + "|";
                commentary += "Empresa Facturación: " + _empresaFacturacion;
            }

            oService.CommentaryUpdate = commentary;
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

            if (_personId != null)
            {
                _personId = AgendaBl.UpdatePerson(oPersonDto, _personId);

                _serviceId = AgendaBl.UpdateService(oService, _serviceId);

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
            MessageBox.Show(@"Se grabó correctamente", @"Información");
        }

        private void frmEditarTrabajador_Load(object sender, EventArgs e)
        {
            cboTipoDocumento.DataSource = agendaBl_.LlenarComboTipoDocumento(cboTipoDocumento);
            cboGenero.DataSource = agendaBl_.LlenarComboGennero(cboGenero);
            cboEstadoCivil.DataSource = agendaBl_.LlenarComboEstadoCivil(cboEstadoCivil);
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
            cboEmpresaFacturacion.DataSource = empresaBl_.GetOrganizationFacturacion(cboEmpresaFacturacion, 9);
            cboTipoDocumento.SelectedValue = 1;
            cboGenero.SelectedValue = 1;
            cboEstadoCivil.SelectedValue = 1;
            cboResidencia.SelectedValue = 0;
            cboNivelEstudio.SelectedValue = 5;
            cboTipoSeguro.SelectedValue = 1;
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
            var datosTrabajador = AgendaBl.GetDatosTrabajador(_nroDocumento);
            if (datosTrabajador != null)
            {
                Mode = "Edit";
                LlenarCampos(datosTrabajador);
            }
            var datosServico = AgendaBl.GetDatosServicio(_serviceId);
            cboEmpresaFacturacion.SelectedValue = datosServico.OrganizationId == null ? "-1" : datosServico.OrganizationId.ToString();

            SetOldValues();
            _empresaFacturacion = cboEmpresaFacturacion.Text;
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

            //cboProvincia.DataSource = new AgendaBl().ObtenerTodasProvincia(cboProvincia, 113);
            cboProvincia.SelectedValue = datosTrabajador.Provincia == null ? "-1" : datosTrabajador.Provincia.ToString();

            //cboDepartamento.DataSource = new AgendaBl().ObtenerTodosDepartamentos(cboDepartamento, 113);
            cboDepartamento.SelectedValue = datosTrabajador.Departamento == null ? "-1" : datosTrabajador.Departamento.ToString();

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

            var contact = AgendaBl.ObtenerContactoEmergencia();
            txtContactEmergency.DataSource = contact;
            txtContactEmergency.DisplayMember = "ContactoEmergencia";
            txtContactEmergency.ValueMember = "ContactoEmergenciaId";

            txtContactEmergency.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Suggest;
            txtContactEmergency.AutoSuggestFilterMode = Infragistics.Win.AutoSuggestFilterMode.Contains;
            this.txtPuesto.DropDownWidth = 250;
            txtContactEmergency.DisplayLayout.Bands[0].Columns[0].Width = 10;
            txtContactEmergency.DisplayLayout.Bands[0].Columns[1].Width = 300;
            if (!string.IsNullOrEmpty(datosTrabajador.ContactoEmergencia))
            {
                txtContactEmergency.Value = datosTrabajador.ContactoEmergencia;
            }
            txtPhoneEmergency.Text = datosTrabajador.CelularEmergencia;
            cboAltitud.SelectedValue = datosTrabajador.Altitud;
            txtExploitedMineral.Text = datosTrabajador.Minerales;
            cboNivelEstudio.SelectedValue = datosTrabajador.Estudios;
            cboGrupo.SelectedValue = datosTrabajador.Grupo == null ? -1 : datosTrabajador.Grupo == 0?-1:datosTrabajador.Grupo;
            cboFactorSan.SelectedValue = datosTrabajador.Factor == null ? -1 : datosTrabajador.Factor == 0 ? -1 : datosTrabajador.Factor; ;
            txtResidenceTimeInWorkplace.Text = datosTrabajador.TiempoResidencia;
            cboTipoSeguro.SelectedValue = datosTrabajador.TipoSeguro;
            txtNumberLivingChildren.Text = datosTrabajador.Vivos.ToString();
            txtNumberDependentChildren.Text = datosTrabajador.Muertos.ToString();
            txtNroHermanos.Text = datosTrabajador.Hermanos.ToString();
            txtTelephoneNumber.Text = datosTrabajador.Telefono;
            cboParentesco.SelectedValue = datosTrabajador.Parantesco;
            cboLugarLabor.SelectedValue = datosTrabajador.Labor;
            txtNacionalidad.Text = datosTrabajador.Nacionalidad;
            txtResidenciaAnte.Text = datosTrabajador.ResidenciaAnterior;
            txtReligion.Text = datosTrabajador.Religion;

            FingerPrintTemplate = datosTrabajador.b_FingerPrintTemplate;
            FingerPrintImage = datosTrabajador.b_FingerPrintImage;
            RubricImage = datosTrabajador.b_RubricImage;
            RubricImageText = datosTrabajador.t_RubricImageText;
            pbPersonImage.Image = UtilsSigesoft.BytesArrayToImage(datosTrabajador.b_PersonImage, pbPersonImage);

            _personId = datosTrabajador.PersonId;
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

        private void btnArchivo1_Click(object sender, EventArgs e)
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
