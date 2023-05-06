using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Windows.WinServer.UI;


namespace SAMBHS.Windows.WinClient.UI
{

    public partial class frmPreferenciasConexion : Form
    {
        public bool Guardado { get { return _guardado; } }
        private bool _guardado = false;

        public frmPreferenciasConexion()
        {
            InitializeComponent();
        }

        private void frmPreferenciasConexion_Load(object sender, EventArgs e)
        {
            UltraStatusbarManager.Inicializar(ultraStatusBar1);
            this.BackColor = new GlobalFormColors().FormColor;
            try
            {
                List<MotorBD> cboDS = new List<MotorBD> 
                {
                    new MotorBD { IdMotor = TipoMotorBD.MSSQLServer, NombreMotor = "MSSQLServer"},
                    new MotorBD { IdMotor = TipoMotorBD.PostgreSQL, NombreMotor = "PostgreSQL"}
                };

                cboMotorBD.DataSource = cboDS;
                cboMotorBD.ValueMember = "IdMotor";
                cboMotorBD.DisplayMember = "NombreMotor";

                txtPassword.Text = Crypto.DecryptStringAES(UserConfig.Default.csPassword, "TiSolUciOnEs");
                txtServidor.Text = UserConfig.Default.csServidor;
                txtUsuario.Text = UserConfig.Default.csUsuario;
                chkMultiEmpresas.Checked = UserConfig.Default.appSistemaMultiEmpresas;
                cboMotorBD.SelectedValue = UserConfig.Default.csTipoMotorBD;
                ultraTextEditor1.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvDatos.Validate(true, false).IsValid)
                {
                    UserConfig.Default.csServidor = txtServidor.Text.Trim();
                    UserConfig.Default.csUsuario = txtUsuario.Text.Trim();
                    UserConfig.Default.csPassword = Crypto.EncryptStringAES(txtPassword.Text.Trim(), "TiSolUciOnEs");
                    UserConfig.Default.csTipoMotorBD = (TipoMotorBD)cboMotorBD.SelectedValue;
                    UserConfig.Default.appSistemaMultiEmpresas = chkMultiEmpresas.Checked;
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

        private void txtServidor_Validated(object sender, EventArgs e)
        {
            string DataSource = txtServidor.Text.Trim();

            if (DataSource.Contains(@"\"))
            {
                int IndexSlash = DataSource.IndexOf(@"\");
                DataSource = DataSource.Substring(0, IndexSlash);
            }

            if (!Utils.PingNetwork(DataSource, 500, 2))
            {
                UltraStatusbarManager.MarcarError(ultraStatusBar1, "Servidor no disponible", timer1);
                ultraButton1.Enabled = false;
            }
            else
            {
                UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
                ultraButton1.Enabled = true;
            }
        }

        class MotorBD
        {
            public TipoMotorBD IdMotor { get; set; }
            public string NombreMotor { get; set; }
        }

        private void ultraTextEditor1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void ultraTextEditor1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                panel1.Visible = ultraTextEditor1.Text.Trim() != "sambhs2015";
                if (!panel1.Visible) txtServidor.Focus();
            }
        }
    }
}
