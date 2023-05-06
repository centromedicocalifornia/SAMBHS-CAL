using SAMBHS.Common.Resource;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinServer.UI.Administracion
{
    public partial class frmRestaurarBd : Form
    {
        private string nombreEmpresa;
        public frmRestaurarBd(string _nombreempresa)
        {
            InitializeComponent();
            nombreEmpresa = _nombreempresa;
        }

        private void ultraTextEditor1_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = string.Format("Archivos Backup (*.{0})| *.{0}", (UserConfig.Default.csTipoMotorBD == TipoMotorBD.PostgreSQL ? "backup" : "bak"));
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;

            if (choofdlog.ShowDialog() == DialogResult.OK) ultraTextEditor1.Text = choofdlog.FileName;
        }

        private void frmRestaurarBd_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ultraTextEditor1.Text.Trim()) || !File.Exists(ultraTextEditor1.Text))
            {
                MessageBox.Show("La ruta del archivo backup no es válida!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            ultraButton1.Enabled = false;
            if (UserConfig.Default.csTipoMotorBD == TipoMotorBD.PostgreSQL)
            {
                if (PostgreSqlDump.DropDB(nombreEmpresa, -1))
                    if (PostgreSqlDump.CreateDB(nombreEmpresa))
                        if (PostgreSqlDump.RestoreDB(ultraTextEditor1.Text, nombreEmpresa, UserConfig.Default.appBinPostgresLocation, UserConfig.Default.csServidor, "5432"))
                        {
                            MessageBox.Show("Empresa Restaurada Correctamente!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                            return;
                        }

                MessageBox.Show("Ocurrió un error al restaurar la Empresa", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (SqlServerDump.RestoreDB(ultraTextEditor1.Text, nombreEmpresa))
                    MessageBox.Show("Empresa Restaurada Correctamente!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                else
                    MessageBox.Show("Ocurrió un error al restaurar la Empresa", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ultraButton1.Enabled = true;
        }
    }
}
