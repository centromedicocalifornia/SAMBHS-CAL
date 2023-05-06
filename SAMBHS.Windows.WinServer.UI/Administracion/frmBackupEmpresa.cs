using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    public partial class frmBackupEmpresa : Form
    {
        private string empresaRuc;
        public frmBackupEmpresa(string _empresaRuc)
        {
            InitializeComponent();
            empresaRuc = _empresaRuc;
        }

        private void frmBackupEmpresa_Load(object sender, EventArgs e)
        {
            UltraStatusbarManager.Inicializar(ultraStatusBar1);
            BackColor = new GlobalFormColors().FormColor;
            if (!string.IsNullOrEmpty(UserConfig.Default.appRutaCrearBackupPredeterminada)) {
                txtRutaBackup.Text = UserConfig.Default.appRutaCrearBackupPredeterminada + empresaRuc + (UserConfig.Default.csTipoMotorBD == TipoMotorBD.PostgreSQL ? ".backup" : ".bak");
            }
        }

        private void txtRutaBackup_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            switch (e.Button.Key)
            {
                case "btnBuscarRutaBackup":
                    FolderBrowserDialog fbd = new FolderBrowserDialog();
                    DialogResult result = fbd.ShowDialog();

                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        txtRutaBackup.Text = fbd.SelectedPath + @"\" + empresaRuc + (UserConfig.Default.csTipoMotorBD == TipoMotorBD.PostgreSQL ? ".backup" : ".bak");
                        UserConfig.Default.appRutaCrearBackupPredeterminada = fbd.SelectedPath + @"\";
                        UserConfig.Default.Save();
                    }
                    break;

                case "btnBuscarRutaBinPostgres":
                    frmBuscarBinPostgres f = new frmBuscarBinPostgres();
                    f.ShowDialog();
                    break;
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            string Password = Crypto.DecryptStringAES(UserConfig.Default.csPassword, "TiSolUciOnEs");
            if (UserConfig.Default.csTipoMotorBD == TipoMotorBD.PostgreSQL)
                PostgreSqlDump.BackupBD(UserConfig.Default.appBinPostgresLocation + "pg_dump.exe", txtRutaBackup.Text.Trim(), UserConfig.Default.csServidor, "5432", empresaRuc, UserConfig.Default.csUsuario, Password);
            else
                SqlServerDump.Backup(empresaRuc, txtRutaBackup.Text.Trim());
            UltraStatusbarManager.Mensaje(ultraStatusBar1, "Backup Finalizado.!", timer1);
            ultraButton1.Enabled = false;
            txtRutaBackup.Enabled = false;
        }
    }
}
