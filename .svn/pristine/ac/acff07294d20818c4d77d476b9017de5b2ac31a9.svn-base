using System;
using System.Windows.Forms;
using SAMBHS.Requerimientos.NBS;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class FrmRegenerarDataDbf : Form
    {
        private bool Bussy { get;set;}

        public FrmRegenerarDataDbf()
        {
            InitializeComponent();
        }

        private void frmRegenerarDataDbf_Load(object sender, EventArgs e)
        {
            txtRutaCabecera.Text = NBS_DBF_PathSettings.Default.dbfSincro_Cabecera;
            txtRutaDetalle.Text = NBS_DBF_PathSettings.Default.dbfSincro_Detalle;
            ultraTextEditor1.Focus();
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (ultraPanel1.Visible) return;
            var msg =
                MessageBox.Show(
                    @"Este proceso puede tardar un momento y se necesita correr cuando no se esté en horario laboral ¿Desea continuar?",
                    @"Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (msg == DialogResult.No) return;
            var f = new RegenerarDbf {RutaDbfCabecera = txtRutaCabecera.Text, RutaDbfDetalle = txtRutaDetalle.Text};
            f.EstadoEvent += delegate(int s) { Invoke((MethodInvoker)delegate { pbAvance.Value = s; }); };
            f.ErrorEvent += delegate(string s)
            {
                MessageBox.Show(s, @"Error en el proceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            f.ProcesoTerminadoEvent += f_ProcesoTerminadoEvent;

            f.ComenzarAsync();
            ultraButton1.Enabled = false;
            Bussy = true;
        }

        private void f_ProcesoTerminadoEvent()
        {
            ultraButton1.Enabled = true;
            Bussy = false;
            MessageBox.Show(@"Proceso completado!", @"Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void FrmRegenerarDataDbf_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = Bussy;
        }

        private void ultraTextEditor1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ultraPanel1.Visible = !ultraTextEditor1.Text.Trim().Equals("sambhs2015");
            }
        }

    }
}
