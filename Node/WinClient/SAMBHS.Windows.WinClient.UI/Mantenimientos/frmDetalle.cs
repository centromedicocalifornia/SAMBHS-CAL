using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmDetalle : Form
    {
        public string detalle = "";
        string edicion = "";
        public int estadoVentana = 0;
        public frmDetalle(string detalle_)
        {
            edicion = detalle_;
            InitializeComponent();
        }
        private void frmDetalle_Load(object sender, EventArgs e)
        {
            txtDetalle.Text = edicion;

        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (txtDetalle.Text == string.Empty)
            {
                MessageBox.Show("ERROR EN DESCRICIÓN", "VALIDACIÓN", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            detalle = txtDetalle.Text;
            estadoVentana = 1;
            this.Close();
        }
    }
}
