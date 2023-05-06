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
    public partial class frmEditarEstadoDigitalContact : Form
    {
        private string _estadoId;
        private string _comentariosDescp;
        private string _DigitalId;
        private string _modo;
        private int _usuarioGraba;
        public frmEditarEstadoDigitalContact(string estadoId, string comentariosDescp, string modo, string DigitalId, int usuarioGraba)
        {
            _estadoId = estadoId;
            _comentariosDescp = comentariosDescp;
            _DigitalId = DigitalId;
            _usuarioGraba = usuarioGraba;
            _modo = modo;
            InitializeComponent();
        }

        private void frmEditarEstadoDigitalContact_Load(object sender, EventArgs e)
        {
            cboEstadoAtencion.DataSource = new AgendaBl().LlenarComboSystemParametro(cboEstadoAtencion, 95);
            cboEstadoAtencion.SelectedIndex = 0;

            if (_modo == "EDITAR")
            {
                cboEstadoAtencion.SelectedIndex = int.Parse(_estadoId);
                txtMotivo.Text = _comentariosDescp;
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            if (cboEstadoAtencion.SelectedIndex == 0)
            {
                MessageBox.Show("Seleccione correctamente el estado de REGISTRO.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtMotivo.Text == string.Empty)
            {
                MessageBox.Show("Ingrese un motivo contundente para editar estado de REGISTRO.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int estado = cboEstadoAtencion.SelectedIndex;
            string comentarios = txtMotivo.Text;

            if (estado == 4 || estado == 5 || estado == 6)
            {
               var cancelar = DigitalContactCenterBl.CancelarCita(_usuarioGraba, _DigitalId);

               if (cancelar)
               {
                   //btnFilter_Click(sender, e);
               }
               else
               {
                   MessageBox.Show("Sucedió un error, por favor vuelva a intentar.", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   return;
               }

            }

            DigitalContactCenterBl.UpdateCalendar(estado, comentarios, _usuarioGraba, _DigitalId);

            this.Close();
        }
    }
}
