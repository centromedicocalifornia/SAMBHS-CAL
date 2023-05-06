using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LoadingClass;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos
{
    public partial class frmConsultaVentasDetraccion : Form
    {
        public frmConsultaVentasDetraccion()
        {
            InitializeComponent();
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            int cant;
            ultraGrid1.DataSource = new VentaBL().ObtenerMovimientosDeAnulados(out cant);
            ultraGrid1.Text = string.Format("Se encontraron {0} registros", cant);
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            try
            {
                const string dummyFileName = "Pendientes";

                SaveFileDialog sf = new SaveFileDialog();
                sf.DefaultExt = "xlsx";
                sf.Filter = @"xlsx files (*.xlsx)|*.xlsx";
                sf.FileName = dummyFileName;

                if (sf.ShowDialog() != DialogResult.OK) return;
                using (new PleaseWait(this.Location, "Exportando excel..."))
                {
                    ultraGridExcelExporter1.Export(ultraGrid1, sf.FileName);
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
            }
        }

        private void ultraButton3_Click(object sender, EventArgs e)
        {
            var filas = ultraGrid1.Rows.ToList();
            var lista = filas.Select(f => f.Cells["idMovimiento"].Value.ToString()).ToList();
            ultraGrid2.DataSource = new VentaBL().ObtenerListaArticulosPorAnulados(lista);
        }

        private void ultraButton4_Click(object sender, EventArgs e)
        {
            try
            {
                const string dummyFileName = "Pendientes Articulos";

                SaveFileDialog sf = new SaveFileDialog();
                sf.DefaultExt = "xlsx";
                sf.Filter = @"xlsx files (*.xlsx)|*.xlsx";
                sf.FileName = dummyFileName;

                if (sf.ShowDialog() != DialogResult.OK) return;
                using (new PleaseWait(this.Location, "Exportando excel..."))
                {
                    ultraGridExcelExporter1.Export(ultraGrid2, sf.FileName);
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
            }
        }

        private void ultraButton5_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            var filas = ultraGrid1.Rows.ToList();
            if (!filas.Any()) return;
            var msje = UltraMessageBox.Show("¿Seguro de Proseguir?", "Sistema", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (msje == DialogResult.No) return;

            var lista = filas.Select(f => f.Cells["idMovimiento"].Value.ToString()).ToList();
            new VentaBL().EliminarNotasSalidaPorAnulados(ref objOperationResult, lista);
            if (objOperationResult.Success == 0)
                UltraMessageBox.Show(
                    objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " +
                    objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                UltraMessageBox.Show("Proceso Terminado!");
        }
    }
}
