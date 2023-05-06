using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmElegirTipoDetalle : Form
    {
        public string _tipo { get; set; }
        public frmElegirTipoDetalle()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cboTipo.Text == "--Seleccionar--")
            {
                MessageBox.Show("Debe Seleccionar una opción", " ¡ INFORMACIÓN !", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            _tipo = cboTipo.Text;

            this.DialogResult = DialogResult.OK;

        }
    }
}
