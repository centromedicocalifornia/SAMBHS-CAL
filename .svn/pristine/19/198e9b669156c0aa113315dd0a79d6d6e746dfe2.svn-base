using System;
using System.Windows.Forms;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmConfirmarMotivoEliminacion : Form
    {
        public string MotivoEliminacion { get { return ultraTextEditor1.Text.Trim(); } }

        public frmConfirmarMotivoEliminacion()
        {
            InitializeComponent();
        }

        private void frmConfirmarMotivoEliminacion_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ultraTextEditor1.Text) || ultraTextEditor1.Text.Trim().Length <= 5)
            {
                lblAlerta.Text = @"Ingrese un motivo...";
                ultraTextEditor1.Focus();

                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
