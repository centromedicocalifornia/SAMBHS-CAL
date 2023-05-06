using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmPLE : Form
    {
        public frmPLE( string pstrParametro)
        {
            InitializeComponent();
        }

        private void frmPLE_Load(object sender, EventArgs e)
        {
            dtpFechaRegistroDe.Format = DateTimePickerFormat.Custom;
            dtpFechaRegistroDe.CustomFormat = "MMMM";
            dtpFechaRegistroDe.ShowUpDown = true;
        }

        private void btnRuta_Click(object sender, EventArgs e)
        {
            // Elegir la ruta para guardar el PDF combinado
            saveFileDialog1.FileName = string.Format(" Archivo PLE {0}", DateTime.Now.ToString("ddMMYYYY"));
            saveFileDialog1.Filter = "Files (*.txt;)|*.txt;";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtRuta.Text = saveFileDialog1.FileName;
            }
        }
    }
}
