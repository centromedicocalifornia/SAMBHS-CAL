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
    public partial class frmBuscarBinPostgres : Form
    {
        public frmBuscarBinPostgres()
        {
            InitializeComponent();
        }

        private void frmBuscarBinPostgres_Load(object sender, EventArgs e)
        {
            ultraTextEditor1.Text = UserConfig.Default.appBinPostgresLocation;
        }

        private void ultraTextEditor1_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            var result = fbd.ShowDialog();

            if (result == DialogResult.OK) 
            {
                ultraTextEditor1.Text = fbd.SelectedPath + @"\";
            }
        }

        private void frmBuscarBinPostgres_FormClosing(object sender, FormClosingEventArgs e)
        {
            UserConfig.Default.appBinPostgresLocation = ultraTextEditor1.Text;
            UserConfig.Default.Save();
        }
    }
}
