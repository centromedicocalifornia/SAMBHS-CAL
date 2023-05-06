using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI
{

    // ReSharper disable once InconsistentNaming
    public partial class frmPreferenciasConexion : Form
    {
        #region Fields
        public bool Guardado { get { return _guardado; } }
        private bool _guardado;
        private readonly SecureString _password;
        #endregion

        #region Construct & Load
        public frmPreferenciasConexion()
        {
            InitializeComponent();
            _password = new SecureString();
            _password.AppendChar('s');
            _password.AppendChar('a');
            _password.AppendChar('m');
            _password.AppendChar('b');
            _password.AppendChar('h');
            _password.AppendChar('s');
            _password.AppendChar('2');
            _password.AppendChar('0');
            _password.AppendChar('1');
            _password.AppendChar('5');
        }

        private void frmPreferenciasConexion_Load(object sender, EventArgs e)
        {
            UltraStatusbarManager.Inicializar(ultraStatusBar1);
            BackColor = new GlobalFormColors().FormColor;
            try
            {
                var cboDs = new List<MotorBd> 
                {
                    new MotorBd { IdMotor = TipoMotorBD.MSSQLServer, NombreMotor = "MSSQLServer"},
                    new MotorBd { IdMotor = TipoMotorBD.PostgreSQL, NombreMotor = "PostgreSQL"}
                };

                cboMotorBD.DataSource = cboDs;
                cboMotorBD.ValueMember = "IdMotor";
                cboMotorBD.DisplayMember = "NombreMotor";

                txtPassword.Text = !UserConfig.Default.csPassword.Equals("123456") ? Crypto.DecryptStringAES(UserConfig.Default.csPassword, "TiSolUciOnEs") : string.Empty;
                txtServidor.Text = UserConfig.Default.csServidor;
                txtUsuario.Text = UserConfig.Default.csUsuario;
                cboMotorBD.Value = UserConfig.Default.csTipoMotorBD;
                txtReplicacionID.Text = UserConfig.Default.repCurrentNode;
                ultraTextEditor1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region DB
        private void ultraButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvDatos.Validate("DB", true, false).IsValid)
                {
                    UserConfig.Default.csServidor = txtServidor.Text.Trim();
                    UserConfig.Default.csUsuario = txtUsuario.Text.Trim();
                    UserConfig.Default.csPassword = Crypto.EncryptStringAES(txtPassword.Text.Trim(), "TiSolUciOnEs");
                    UserConfig.Default.csTipoMotorBD = (TipoMotorBD)cboMotorBD.Value;
                    UserConfig.Default.repCurrentNode = txtReplicacionID.Text;
                    UserConfig.Default.Save();
                    _guardado = true;
                    UltraStatusbarManager.Mensaje(ultraStatusBar1, "Configuración guardada correctamente.", timer1);
                }
                else
                {
                    UltraStatusbarManager.MarcarError(ultraStatusBar1, "Ingrese todos los campos requeridos.", timer1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class MotorBd
        {
            public TipoMotorBD IdMotor { get; set; }
            public string NombreMotor { get; set; }
        }

        private void ultraTextEditor1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                panel1.Visible = ultraTextEditor1.Text.Trim() != SecureStringToString(_password);
                if (!panel1.Visible) txtServidor.Focus();
            }
        }

        private static string SecureStringToString(SecureString value)
        {
            var bstr = Marshal.SecureStringToBSTR(value);

            try
            {
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                Marshal.FreeBSTR(bstr);
            }
        }
        #endregion

    }
}
