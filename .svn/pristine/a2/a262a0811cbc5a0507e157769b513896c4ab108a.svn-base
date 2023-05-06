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
    public partial class frmLicenseInput : Form
    {
        public string LicenciaInput {
            get { return ultraTextEditor1.Text.Trim(); }
        }

        public frmLicenseInput()
        {
            InitializeComponent();
        }

        private void frmLicenseInput_Load(object sender, EventArgs e)
        {

        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ultraTextEditor1.Text))
            {
                ultraTextEditor1.Focus();
                MessageBox.Show(@"Ingrese la licencia");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
