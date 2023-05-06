using System;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class FrmEditarDescripcion : Form
    {
        public string Descripcion { get; set; }
        public FrmEditarDescripcion(string descripcion)
        {
            InitializeComponent();
            txtDescripcion.Text = descripcion;
        }

        private void frmEditarDescripcion_Load(object sender, EventArgs e)
        {
            txtDescripcion.Focus();
            txtDescripcion.SelectionStart = txtDescripcion.TextLength;
        }

        private void FrmEditarDescripcion_FormClosing(object sender, FormClosingEventArgs e)
        {
            Descripcion = txtDescripcion.Text.Trim();
        }
    }
}
