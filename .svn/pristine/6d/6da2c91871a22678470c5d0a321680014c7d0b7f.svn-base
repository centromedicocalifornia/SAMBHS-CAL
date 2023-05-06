using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class frmConfigurarDbfConnection : Form
    {
        public frmConfigurarDbfConnection()
        {
            InitializeComponent();
        }

        private void txtTablaKardex_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var choofdlog = new OpenFileDialog
            {
                Filter = string.Format("Archivos Backup (*.{0}) | *.{0}", "dbf"),
                FilterIndex = 1,
                Multiselect = false
            };

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                NBS_DBF_PathSettings.Default.dbf_data_path = choofdlog.FileName;
                if (choofdlog.SafeFileName != null)
                {
                    NBS_DBF_PathSettings.Default.dbf_data_filename = choofdlog.SafeFileName.Split('.')[0];
                    txtTablaKardex.Text = NBS_DBF_PathSettings.Default.dbf_data_path;
                }
                NBS_DBF_PathSettings.Default.Save();
            }
        }

        private void txtTablaKardexCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var choofdlog = new OpenFileDialog
            {
                Filter = string.Format("Archivos Backup (*.{0}) | *.{0}", "dbf"),
                FilterIndex = 1,
                Multiselect = false
            };

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                NBS_DBF_PathSettings.Default.dbf_clientref_path = choofdlog.FileName;
                if (choofdlog.SafeFileName != null)
                {
                    NBS_DBF_PathSettings.Default.dbf_clientref_filename = choofdlog.SafeFileName.Split('.')[0];
                    txtTablaKardexCliente.Text = NBS_DBF_PathSettings.Default.dbf_clientref_path;
                }
                NBS_DBF_PathSettings.Default.Save();
            }
        }

        private void frmConfigurarDbfConnection_Load(object sender, EventArgs e)
        {
            txtTablaKardex.Text = NBS_DBF_PathSettings.Default.dbf_data_path;
            txtTablaKardexCliente.Text = NBS_DBF_PathSettings.Default.dbf_clientref_path;
            txtCabeceraSincro.Text = NBS_DBF_PathSettings.Default.dbfSincro_Cabecera;
            txtDetalle.Text = NBS_DBF_PathSettings.Default.dbfSincro_Detalle;
        }

        private void txtCabeceraSincro_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var choofdlog = new OpenFileDialog
            {
                Filter = string.Format("Archivos Backup (*.{0}) | *.{0}", "dbf"),
                FilterIndex = 1,
                Multiselect = false
            };

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                txtCabeceraSincro.Text = NBS_DBF_PathSettings.Default.dbfSincro_Cabecera = choofdlog.FileName;
                NBS_DBF_PathSettings.Default.Save();
            }
        }

        private void txtDetalle_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var choofdlog = new OpenFileDialog
            {
                Filter = string.Format("Archivos Backup (*.{0}) | *.{0}", "dbf"),
                FilterIndex = 1,
                Multiselect = false
            };

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                txtDetalle.Text = NBS_DBF_PathSettings.Default.dbfSincro_Detalle = choofdlog.FileName;
                NBS_DBF_PathSettings.Default.Save();
            }
        }
    }
}
