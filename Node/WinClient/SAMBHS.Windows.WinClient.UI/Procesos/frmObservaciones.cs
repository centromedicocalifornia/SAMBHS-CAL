using SAMBHS.Windows.SigesoftIntegration.UI;
//using SAMBHS.Windows.SigesoftIntegration.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmObservaciones : Form
    {
        public string _ServicioId;
        public string _Observacion;
        public frmObservaciones(string ServicioId, string Observacion)
        {
            _ServicioId = ServicioId;
            _Observacion = Observacion;
            InitializeComponent();
        }

        private void frmObservaciones_Load(object sender, EventArgs e)
        {
            txtDetalleAtencion.Text = _Observacion;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var DialogResult =
                        MessageBox.Show("¿Desea actualizar obsrvaciones de atención?", "INFORMACIÓN!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (DialogResult == DialogResult.Yes)
            {

                if (txtDetalleAtencion.Text == "- - -" || string.IsNullOrEmpty(txtDetalleAtencion.Text))
                {
                    MessageBox.Show("Ingrese la información correctamente", "CORRECTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    AgendaBl.UpdateObservacionServicio(txtDetalleAtencion.Text.Trim(), _ServicioId);

                    MessageBox.Show("GUARDADO CORRECTAMENTE", "CORRECTO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.Close();
                }

                
            }
            else
            {
                this.Close();
            }
        }
    }
}
