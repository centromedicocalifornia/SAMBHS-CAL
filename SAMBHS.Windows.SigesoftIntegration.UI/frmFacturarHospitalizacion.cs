using SAMBHS.Common.BE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmFacturarHospitalizacion : Form
    {
        public frmFacturarHospitalizacion()
        {
            InitializeComponent();
        }

        private void frmFacturarHospitalizacion_Load(object sender, EventArgs e)
        {

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var cobarA = "Paciente";
            if (rbMedicoTratante.Checked)
                cobarA = "Medico";

           var obj = FacturacionServiciosBl.FacturarHospitalizacion(txtDni.Text.Trim(), dtpF1.Value, dtpF2.Value, cobarA);

           if (rbMedicoTratante.Checked)
           {
               var conDeuda = obj.FindAll(p => p.i_MedicoPago != 1).ToList();
               grdData.DataSource = conDeuda;
           }
           else
           {
               var conDeuda = obj.FindAll(p => p.i_PacientePago != 1).ToList();
               grdData.DataSource = conDeuda;
           }
           
        }

        public BindingList<ventadetalleDto> ListadoVentaDetalle = new BindingList<ventadetalleDto>();
        private void cboAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    ListadoVentaDetalle = FacturacionServiciosBl.ObtenerDetalleVenta((List<FacturarHospitalizacion>)grdData.DataSource);
                }).ContinueWith(t =>
                {
                    if (ListadoVentaDetalle.Any())
                    {
                        DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                    ultraGroupBox1.Enabled = true;
                    cboAceptar.Enabled = true;
                    Close();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
