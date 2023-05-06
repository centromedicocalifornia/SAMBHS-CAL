using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using SAMBHS.Common.BE;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Venta.BL;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmClienteVendedor : Form
    {
        public frmClienteVendedor(string N)
        {
            InitializeComponent();
            this.Load += frmClienteVendedor_Load;
        }

        private void frmClienteVendedor_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            cboAgrupar.SelectedIndex = 0;
            this.Load -= frmClienteVendedor_Load;
        }

        #region Eventos UI
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            var aptitudeCertificate = CargarReporte();

            var rp = new crClienteVendedor();

            DataSet ds1 = new DataSet();
            WinClient.UI.Dataset.dsClienteVendedor f = new Dataset.dsClienteVendedor();

            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            dt.TableName = "dsClienteVendedor";
            ds1.Tables.Add(dt);
            rp.SetDataSource(ds1);
            rp.SetParameterValue("ShowDate", chkVisibleDate.Checked);
            crystalReportViewer1.ReportSource = rp;
        }

        private void txtDetalleEspecifico_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmBuscarVendedor();
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtDetalleEspecifico.Text = f.Vendedor.v_NombreCompleto;
                txtDetalleEspecifico.Tag = f.Vendedor.v_IdVendedor;
            }
        }

        private void cboAgrupar_ValueChanged(object sender, EventArgs e)
        {
            txtDetalleEspecifico.Enabled = (cboAgrupar.SelectedIndex == 1);
        }
        #endregion

        #region Methods
        List<ReporteClienteVendedorDto> CargarReporte()
        {
            try
            {
                List<ReporteClienteVendedorDto> Reporte;
                string FilterExpression = string.Empty;

                #region Armar la expresion de filtro para la consulta
                if (cboAgrupar.SelectedIndex == 1 && txtDetalleEspecifico.Tag != null)
                    FilterExpression = "v_IdVendedor == \"" + txtDetalleEspecifico.Tag.ToString() + "\"";
                #endregion

                OperationResult objOperationResult = new OperationResult();

                Cursor.Current = Cursors.WaitCursor;
                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                {
                    Reporte = new CarteraClienteBL().ObtenerReporteCarteraClientePorVendedor(ref objOperationResult, null, FilterExpression);
                }
                Cursor.Current = Cursors.Default;

                if (objOperationResult.Success == 0)
                {
                    if (!string.IsNullOrEmpty(objOperationResult.ExceptionMessage))
                    {
                        UltraMessageBox.Show(objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return null;
                }

                return Reporte;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        #endregion
    }
}
