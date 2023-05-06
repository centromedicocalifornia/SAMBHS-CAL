using LoadingClass;
using SAMBHS.Common.Resource;
using SAMBHS.Requerimientos.NBS;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class frmConsultaMovimientos : Form
    {
        private string v_IdCliente;
        private string v_IdClienteDesdeFacturacion;
        public frmConsultaMovimientos(string N)
        {
            InitializeComponent();
        }

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarCliente")
            {
                frmBuscarCliente f = new frmBuscarCliente("VV", "");
                f.ShowDialog();
                if (f._IdCliente != null)
                {
                    txtCliente.Tag = f._IdCliente;
                    txtCliente.Text = string.Format("{0} - {1}", f._NroDocumento, f._RazonSocial);
                    v_IdCliente = f._IdCliente;
                    txtCliente.ButtonsRight["btnEliminar"].Enabled = true;
                }
            }
            else
            {
                txtCliente.Tag = null;
                txtCliente.Clear();
                v_IdCliente = string.Empty;
                txtCliente.ButtonsRight["btnEliminar"].Enabled = false;
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            //if (v_IdCliente != null)
            //{
                OperationResult objOperationResult = new OperationResult();
                var dsorden = ObtenerConsultaReporteFlujoBl.ObtenerConsultaFlujoProcesos(ref objOperationResult, v_IdCliente , DateTime.Parse ( dtpFechaDesdeO.Text  ),DateTime.Parse (dtpFechaHastaO.Text  +" 23:59") );

                if (objOperationResult.Success == 1)
                {
                    grdData.DataSource = dsorden;
                    btnExportarExcel.Enabled = true;
                }


                lblContadorFilasDesdeOrden.Text = string.Format("Se encontraron {0} registros.", dsorden.Count);
            //}
        }

        private void frmConsultaMovimientos_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string dummyFileName = "Flujo-OrdenTrabajo-" + txtCliente.Text.Trim();

                SaveFileDialog sf = new SaveFileDialog();
                sf.DefaultExt = "xlsx";
                sf.Filter = @"xlsx files (*.xlsx)|*.xlsx";
                sf.FileName = dummyFileName;

                if (sf.ShowDialog() != DialogResult.OK) return;
                using (new PleaseWait(this.Location, "Exportando excel..."))
                {
                    ultraGridExcelExporter1.Export(grdData, sf.FileName);
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
            }
        }

        private void ultraGroupBox1_Click(object sender, EventArgs e)
        {

        }

        private void txtTipoKardex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'H' || e.KeyChar == 'h' || e.KeyChar == 8 || e.KeyChar == 'E' || e.KeyChar == 'e' || e.KeyChar == 'M' || e.KeyChar == 'm' || e.KeyChar == 'K' || e.KeyChar == 'k' || e.KeyChar == 'R' || e.KeyChar == 'r')
           
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

        }

        private void btnBuscarDesdeFacturacion_Click(object sender, EventArgs e)
        {
            //if (v_IdCliente != null)
            //{
                OperationResult objOperationResult = new OperationResult();
                var ds = ObtenerConsultaReporteFlujoBl.ObtenerKardexDesdeFacturacion(ref objOperationResult, v_IdClienteDesdeFacturacion, dtpFechaRegistroDe.Value.Date, DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), txtTipoKardex.Text.Trim(), txtNroKardex.Text.Trim());

                if (objOperationResult.Success == 1)
                {
                    grDataFacturacion.DataSource = ds;
                    btnExportarExcel.Enabled = true;
                }
                lblContadorFilasDesdeFacturación.Text = string.Format("Se encontraron {0} registros.", ds.Count);
               
            //}
        }

        private void txtClienteDesdeFacturacion_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarCliente")
            {
                frmBuscarCliente f = new frmBuscarCliente("VV", "");
                f.ShowDialog();
                if (f._IdCliente != null)
                {
                    txtClienteDesdeFacturacion.Tag = f._IdCliente;
                    txtClienteDesdeFacturacion.Text = string.Format("{0} - {1}", f._NroDocumento, f._RazonSocial);
                    v_IdClienteDesdeFacturacion = f._IdCliente;
                    txtClienteDesdeFacturacion.ButtonsRight["btnEliminar"].Enabled = true;
                }
            }
            else
            {
                txtClienteDesdeFacturacion.Tag = null;
                txtClienteDesdeFacturacion.Clear();
                v_IdClienteDesdeFacturacion = string.Empty;
                txtClienteDesdeFacturacion.ButtonsRight["btnEliminar"].Enabled = false;
            }
        }

        private void ultraCheckEditor1_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grDataFacturacion);
        }
    }
}
