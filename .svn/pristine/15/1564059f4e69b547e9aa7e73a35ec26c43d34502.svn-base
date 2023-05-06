using System;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using System.Threading;
using System.Windows.Forms;
using System.Net;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{

    // ReSharper disable once InconsistentNaming
    public partial class frmCustomerCapchaRENIEC : Form
    {
        #region Fields
        private ConsultaPersona _myInfo;
        private Thread _thread;
        private readonly string _dni;
        #endregion

        #region Properties
        public string[] DatosPersona = new string[3];
        public bool ConectadoRecibido;
        #endregion

        #region Construct
        public frmCustomerCapchaRENIEC(string dni)
        {
            _dni = dni;
            InitializeComponent();
        }
        #endregion

        #region Event UI
        private void frmCustomerCapchaRENIEC_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            txtCapcha.Focus();
            _thread = new Thread(t => CargarImagen()) { IsBackground = true };
            _thread.Start();
            ConectadoRecibido = false;
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCapcha.Text.Trim())) return;

            _thread = new Thread(t =>
            {
                Invoke(new MethodInvoker(() => SetStatusBar(true)));
                _myInfo.GetInfo(_dni, txtCapcha.Text);
                Invoke(new MethodInvoker(CaptionResul));
            }) { IsBackground = true };

            _thread.Start();
        }
        private void txtCapcha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
                InvokeOnClick(btnOK, e);
        }

        private void frmCustomerCapchaRENIEC_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            CargarImagen();
            txtCapcha.SelectAll();
            txtCapcha.Focus();
            txtCapcha.Clear();
        }

        private void frmCustomerCapchaRENIEC_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_thread != null && _thread.IsAlive) _thread.Abort();
        }
        #endregion

        #region Methods
        private void SetStatusBar(bool buscando)
        {
            if (buscando)
            {
                uStatusBar.Panels["pMensaje"].Visible = false;
                uStatusBar.Panels["pMarquee"].Visible = true;
            }
            else
            {
                uStatusBar.Panels["pMensaje"].Visible = true;
                uStatusBar.Panels["pMarquee"].Visible = false;
            }
            Invoke(new MethodInvoker(() =>
            {
                txtCapcha.Enabled = !buscando;
                btnOK.Enabled = txtCapcha.Enabled;
                btnReload.Enabled = txtCapcha.Enabled;
            }));
        }

        private void CargaImagenCompletada()
        {
            lblObteniendoCapcha.Visible = false;
            btnOK.Enabled = true;
            btnReload.Enabled = true;
            pbLoadingCapcha.Visible = false;
        }

        private void CargarImagen()
        {
            try
            {
                if (_myInfo == null)
                    _myInfo = new ConsultaPersona();

                pbCapcha.Image = _myInfo.GetCapcha;
                Invoke(new MethodInvoker(CargaImagenCompletada));
            }
            catch (WebException ex)
            {
                var httpResponse = (HttpWebResponse)ex.Response;
                if (httpResponse != null)
                {
                    if (httpResponse.StatusCode == HttpStatusCode.InternalServerError)
                            CargarImagen();
                }
                else
                    Invoke(new MethodInvoker(() =>
                    {
                        lblObteniendoCapcha.Text = @"Error de Conexión";
                        pbLoadingCapcha.Image = Resource.delete;
                    }));
            }
        }

        private void CaptionResul()
        {
            switch (_myInfo.GetResul)
            {
                case ConsultaPersona.Resul.Ok:
                    DatosPersona[0] = _myInfo.ApePaterno;
                    DatosPersona[1] = _myInfo.ApeMaterno;
                    DatosPersona[2] = _myInfo.Nombres;
                    ConectadoRecibido = true;
                    Close();
                    break;
                case ConsultaPersona.Resul.NoResul:
                    UltraMessageBox.Show("No existe DNI", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SetStatusBar(false);
                    break;
                case ConsultaPersona.Resul.ErrorCapcha:
                    CargarImagen();
                    UltraMessageBox.Show("Ingrese el capcha correctamente", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SetStatusBar(false);
                    break;
                case ConsultaPersona.Resul.Error:
                    UltraMessageBox.Show("Error desconocido", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SetStatusBar(false);
                    break;
            }
        }
        #endregion
    }
}
