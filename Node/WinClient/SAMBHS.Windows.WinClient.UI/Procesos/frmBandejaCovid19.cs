using Infragistics.Win.UltraWinGrid.ExcelExport;
using LoadingClass;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI.BLL;
using SAMBHS.Windows.SigesoftIntegration.UI.Dtos;
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
    public partial class frmBandejaCovid19 : Form
    {
        public frmBandejaCovid19()
        {
            InitializeComponent();
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            BindingGrid();
        }

        private void BindingGrid()
        {
            List<ListadoCovid> DataSource = new List<ListadoCovid>();
            if (checkPruebaRapida.Checked == true)
            {
                DataSource = new ServiceBL().GetServicesByCovid(txtValue.Text, dtInicio.Value.Date, dtFin.Value); 
            }
            else if (checkPruebaAntigeno.Checked == true)
            {
                DataSource = new ServiceBL().GetServicesByCovidAntigeno(txtValue.Text, dtInicio.Value.Date, dtFin.Value);
            }
            else if (checkPruebaMolecular.Checked == true)
            {
                DataSource = new ServiceBL().GetServicesByCovidMolecular(txtValue.Text, dtInicio.Value.Date, dtFin.Value);
            }
            

            grdServices.DataSource = DataSource;
            grdServices.DataBind();

            if (DataSource == null) return;
            if (DataSource.Count >= 0)
            {
                lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", DataSource.Count);
            }
            else
            {
                lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", DataSource.Count);
            }
            
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {

            string dummyFileName = "PRUEBAS COVID-19 DESDE " + dtInicio.Value.ToShortDateString().Split('/')[0] + "-" + dtInicio.Value.ToShortDateString().Split('/')[1] + "-" + dtInicio.Value.ToShortDateString().Split('/')[2] + " HASTA " + dtFin.Value.ToShortDateString().Split('/')[0] + "-" + dtFin.Value.ToShortDateString().Split('/')[1] + "-" + dtFin.Value.ToShortDateString().Split('/')[2];
                SaveFileDialog sf = new SaveFileDialog();
                var ultraGridExcelExporter1 = new UltraGridExcelExporter();
                sf.DefaultExt = "xlsx";
                sf.Filter = "xlsx files (*.xlsx)|*.xlsx";
                // Feed the dummy name to the save dialog
                sf.FileName = dummyFileName;


                if (sf.ShowDialog() == DialogResult.OK)
                {
                    using (new PleaseWait(this.Location, "Exportando excel..."))
                    {
                        ultraGridExcelExporter1.Export(grdServices, sf.FileName);

                    }
                    UltraMessageBox.Show("Exportación Finalizada", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dtInicio_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dtFin_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkPruebaRapida_CheckedChanged(object sender, EventArgs e)
        {
            if (checkPruebaRapida.Checked == true)
            {
                checkPruebaAntigeno.Checked = false;
                checkPruebaMolecular.Checked = false;
            }
        }

        private void checkPruebaAntigeno_CheckedChanged(object sender, EventArgs e)
        {
            if (checkPruebaAntigeno.Checked == true)
            {
                checkPruebaRapida.Checked = false;
                checkPruebaMolecular.Checked = false;
            }
        }

        private void checkPruebaMolecular_CheckedChanged(object sender, EventArgs e)
        {
            if (checkPruebaMolecular.Checked == true)
            {
                checkPruebaRapida.Checked = false;
                checkPruebaAntigeno.Checked = false;
            }
        }
        
    }
}
