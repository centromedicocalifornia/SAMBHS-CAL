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
    public partial class frmEditMedic : Form
    {
        public int _medicsolicitante;
        public int _medictratante;
        public string _v_Servicecomponent;
        public frmEditMedic(string v_Servicecomponent, int medicsolicitante, int medictratante)
        {
            _medicsolicitante = medicsolicitante;
            _medictratante = medictratante;
            _v_Servicecomponent = v_Servicecomponent;
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

            new ProtocoloBl().UpdateServiceComponentMedicTratante(_v_Servicecomponent, int.Parse(cboMedicoTratante.SelectedValue.ToString()), int.Parse(cboMedicoIndica.SelectedValue.ToString()));
            
            MessageBox.Show("Se grabo correctamente", " ¡ INFORMACIÓN !", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;


        }

        private void frmEditMedic_Load(object sender, EventArgs e)
        {
            var datosServiceComponent = ProtocoloBl.GetServiceComponent(_v_Servicecomponent);

            cboMedicoIndica.DataSource = new AgendaBl().LlenarComboUsuarios(cboMedicoIndica);
            cboMedicoIndica.SelectedValue = "11";
            cboMedicoIndica.Enabled = true;

            cboMedicoIndica.SelectedValue = datosServiceComponent.MedicoIndica.ToString() == null || datosServiceComponent.MedicoIndica.ToString() == "-1" ? "11" : datosServiceComponent.MedicoIndica.ToString();

            cboMedicoTratante.DataSource = new AgendaBl().LlenarComboUsuarios(cboMedicoTratante);
            cboMedicoTratante.SelectedValue = "11";
            cboMedicoTratante.Enabled = true;

            cboMedicoTratante.SelectedValue = datosServiceComponent.MedicoTratante.ToString() == null || datosServiceComponent.MedicoTratante.ToString() == "-1" ? "11" : datosServiceComponent.MedicoTratante.ToString();

        }
    }
}
