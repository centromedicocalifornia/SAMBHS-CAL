using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
using ScrapperReniecSunat;
using Sigesoft.Node.WinClient.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmNuevoRegistro : Form
    {
        #region Properties


        public byte[] FingerPrintTemplate { get; set; }

        public byte[] FingerPrintImage { get; set; }

        public byte[] RubricImage { get; set; }

        public string RubricImageText { get; set; }

        #endregion

        #region Variables Globales
        private string _personId;
        private string Mode;
        private string _fileName;
        private string _filePath;
        private AgendaBl agendaBl_ = new AgendaBl();
        #endregion
        public frmNuevoRegistro()
        {
            InitializeComponent();
        }

        private void btnBuscarTrabajador_Click(object sender, EventArgs e)
        {
            #region Validaciones
            if (txtSearchNroDocument.Text == "" || txtSearchNroDocument.Text.Length != 8)
            {
                if (txtSearchNroDocument.Text == "")
                {
                    MessageBox.Show(@"No hay nro. de documento para buscar", @"Información");
                }
                else if (txtSearchNroDocument.Text.Length != 8)
                {
                    MessageBox.Show(@"Nro. de dígitos incorrecto (DNI=8 dígitos)", @"Información");
                }
                return;
            }
            #endregion
            //Buscar paciente
            var datosTrabajador = AgendaBl.GetDatosTrabajador(txtSearchNroDocument.Text);
            if (datosTrabajador != null)
            {
                LlenarCampos(datosTrabajador);
                Mode = "Edit";
            }
            else 
            {
                ObtenerDatosDNI(txtSearchNroDocument.Text.Trim());
                cboTipoDocumento.SelectedValue = 1;
                cboGenero.SelectedValue = 1;
                Mode = "New";
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
                                txtNroDocumento.Text = txtSearchNroDocument.Text;
                                txtNombres.Text = datos.Nombre;
                                txtApellidoPaterno.Text = datos.ApellidoPaterno;
                                txtApellidoMaterno.Text = datos.ApellidoMaterno;
                                dtpBirthdate.Value = datos.FechaNacimiento;
                                _personId = null;
                            }
                        }
                        break;
                }
            }
            else
                MessageBox.Show("No se pudo conectar la página", "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            FingerPrintTemplate = datosTrabajador.b_FingerPrintTemplate;
            FingerPrintImage = datosTrabajador.b_FingerPrintImage;
            RubricImage = datosTrabajador.b_RubricImage;
            RubricImageText = datosTrabajador.t_RubricImageText;
            pbPersonImage.Image = null;
            pbPersonImage.ImageLocation = null;
            pbPersonImage.Image = UtilsSigesoft.BytesArrayToImage(datosTrabajador.b_PersonImage, pbPersonImage);
            _personId = datosTrabajador.PersonId;
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

        private void frmNuevoRegistro_Load(object sender, EventArgs e)
        {
            cboTipoDocumento.DataSource = agendaBl_.LlenarComboTipoDocumento(cboTipoDocumento);
            cboTipoDocumento.SelectedIndex = 0;

            cboGenero.DataSource = agendaBl_.LlenarComboGennero(cboGenero);
            cboGenero.SelectedIndex = 0;
            
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
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            pbPersonImage.Image = null;
        }

        private void btnSavePacient_Click(object sender, EventArgs e)
        {
            GrabarTrabajadorNuevo();
            btnSavePacient.Enabled = false;
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
            oPersonDto.EstadoCivil = -1;
            oPersonDto.LugarNacimiento = "---";
            oPersonDto.Distrito = -1 ;
            oPersonDto.Provincia = -1;
            oPersonDto.Departamento = -1;
            oPersonDto.Reside = -1;
            oPersonDto.Email = "---";
            oPersonDto.Direccion = "---";
            oPersonDto.Puesto = "---";
            oPersonDto.Altitud = -1;
            oPersonDto.Minerales = "---";
            oPersonDto.Estudios =  -1 ;
            oPersonDto.Grupo = -1;
            oPersonDto.Factor = -1;
            oPersonDto.TiempoResidencia = "---";
            oPersonDto.TipoSeguro = -1 ;
            oPersonDto.Vivos = 0;
            oPersonDto.Muertos = 0 ;
            oPersonDto.Hermanos = 0;
            oPersonDto.Telefono = "---";
            oPersonDto.Parantesco = -1 ;
            oPersonDto.Labor =  -1;
            oPersonDto.Nacionalidad = "---";
            oPersonDto.ResidenciaAnte = "---";
            oPersonDto.Religion = "---";
            oPersonDto.titular = "---";

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
            }
            else
            {
                _personId = AgendaBl.AddPerson(oPersonDto);
                if (_personId == "El paciente ya se encuentra registrado")
                {
                    MessageBox.Show(@"El paciente ya se encuentra registrado", @"Información");
                    return;
                }
            }
            
            MessageBox.Show(@"Se grabó correctamente", @"Información");
            this.Close();
        }
    }
}
