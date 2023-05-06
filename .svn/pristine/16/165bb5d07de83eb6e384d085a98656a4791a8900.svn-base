using System;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Security.BL;
using SAMBHS.CommonWIN.BL;
using System.Deployment.Application;
using System.Runtime.InteropServices;
using SAMBHS.Windows.WinServer.UI;

namespace SAMBHS.Windows.WinClient.UI
{
    public partial class frmLogin : Form
    {
        private int _intNodeId, _intIdEstablecimiento;
        string DataSource;

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }

        private void AuthenticateUser()
        {
            ConfiguracionEmpresaBL objConfiguracionEmpresaBL = new ConfiguracionEmpresaBL();
            OperationResult objOperationResult = new OperationResult();
            SecurityBL objSecurityBL = new SecurityBL();
            var objUserDto = objSecurityBL.ValidateSystemUser(ref objOperationResult, _intNodeId, txtUserName.Text, Utils.Encrypt(txtPassword.Text));
            ClientSession objClientSession = new ClientSession();

            try
            {
                if (objUserDto != null)
                {

                    objClientSession.i_SystemUserId = objUserDto.i_SystemUserId;
                    objClientSession.v_UserName = objUserDto.v_PersonName + " (" + objUserDto.v_UserName + ")";
                    objClientSession.i_PersonId = objUserDto.i_PersonId;
                    objClientSession.i_RoleId = (int)objUserDto.i_RoleId;

                    if (objOperationResult.Success == 0)
                    {
                        if (!string.IsNullOrEmpty(objOperationResult.ExceptionMessage))
                        {
                            UltraMessageBox.Show(objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return;
                    }
                }
                else
                {
                    UltraMessageBox.Show((objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage).Trim(), "Error de Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning, "Si no recuerda su usuario o contraseña, póngase en contacto con sistemas.");
                    txtUserName.Focus();
                    return;
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
            }

            // Pasar el objeto de sesión al gestor de objetos globales
            Globals.ClientSession = objClientSession;
            this.Hide();
            frmMaster frm = new frmMaster();
            ShowDesktop();
            frm.ShowDialog();
            this.Close();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            #region Actualizacion ClickOnce
            try
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
            }
            catch { }
            #endregion

            OperationResult objOperationResult = new OperationResult();
            this.BackColor = new GlobalFormColors().FormColor;

            txtUserName.Focus();

        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK_Click(sender, e);
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string providerString = string.Empty;

            #region Propiedades de la ConnectionString
            DataSource = UserConfig.Default.csServidor;
            string InitialCatalog = "TIS_INTEGRADO";
            string UserID = UserConfig.Default.csUsuario;
            string Password = Crypto.DecryptStringAES(UserConfig.Default.csPassword, "TiSolUciOnEs");

            switch (UserConfig.Default.csTipoMotorBD)
            {
                case TipoMotorBD.MSSQLServer:
                   // providerString = "Data Source=" + DataSource + ";Initial Catalog=" + InitialCatalog + ";Integrated Security=False;Persist Security Info=True;User ID=" + UserID + ";Password=" + Password + "";
                    providerString = "Data Source=" + DataSource + ";Initial Catalog=" + InitialCatalog + ";Integrated Security=False;Persist Security Info=True;User ID=" + UserID + ";Password=" + Password +  ";Connection Timeout=0" + "";
                    break;

                case TipoMotorBD.PostgreSQL:
                    providerString = "User Id=" + UserID + "; password=" + Password + ";Host=" + DataSource + ";Database=" + InitialCatalog + ";Initial Schema=public;default command timeout=0";
                    break;
            }

            LocalGlobals.ConnectionString = providerString;
            string cs = ConnectionStringManager.GetConnectionString("SAMBHSConnectionStringWin");
            string newConStr = ConnectionStringManager.SetConnectionStringDatabaseName(cs, providerString);
            ConnectionStringManager.SaveConnectionString(ref objOperationResult, "SAMBHSConnectionStringWin", newConStr);

            if (objOperationResult.Success == 0)
            {
                UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion

            this.AuthenticateUser();

        }

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        /// <summary>
        /// Shows the desktop.
        /// </summary>
        public static void ShowDesktop()
        {
            keybd_event(0x5B, 0, 0, 0);
            keybd_event(0x4D, 0, 0, 0);
            keybd_event(0x5B, 0, 0x2, 0);
        }

        private void btnConfigurarConexión_Click(object sender, EventArgs e)
        {
            frmPreferenciasConexion cnx = new frmPreferenciasConexion();
            cnx.ShowDialog();
        }

        private void txtUserName_Validated(object sender, EventArgs e)
        {
            var btnconfigEstado = txtUserName.Text.Trim().ToLower() == "sa";
            btnConfigurarConexión.Visible = btnconfigEstado;
        }

    }
}
