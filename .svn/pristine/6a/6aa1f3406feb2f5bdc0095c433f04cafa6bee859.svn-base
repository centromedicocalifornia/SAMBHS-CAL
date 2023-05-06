using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SAMBHS.Common.Resource
{
    public partial class ConexionPerdida : Form
    {
        public ConexionPerdida(string nombreSuscriptor)
        {
            InitializeComponent();
            lblNombreSucursal.Text = nombreSuscriptor;
        }

        private void ConexionPerdida_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            this.TopMost = true;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
