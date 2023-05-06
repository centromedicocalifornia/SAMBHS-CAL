using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Contabilidad.BL;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Tesoreria
{
    public partial class frmReporteDetracciones : Form
    {
        private readonly VentaDetraccionesBl _objVentaDetraccionesBl;
        private OperationResult _objOperationResult;
        private List<ReporteVentaDetracciones> _dataReporte;
        public frmReporteDetracciones(string N)
        {
            InitializeComponent();
            _objVentaDetraccionesBl = new VentaDetraccionesBl();
            _objOperationResult = new OperationResult();
            _dataReporte = new List<ReporteVentaDetracciones>();
        }

        private void frmReporteDetracciones_Load(object sender, EventArgs e) {
            BackColor = new GlobalFormColors().FormColor;
        }

        private void CargarReporte()
        {
            try
            {
                _dataReporte = _objVentaDetraccionesBl.ObtenerVentasDetraccion(ref _objOperationResult, 
                    dtpFechaRegistroDe.Value, DateTime.Parse (dtpFechaRegistroAl.Text + " 23:59"));
                if (uckDetraccionMayor0.Checked)
                {
                    _dataReporte = _dataReporte.Where(o => o.DepositoBancoDetraccion > 0).ToList ();
                }
                if (_objOperationResult.Success == 0)
                {
                    MessageBox.Show(_objOperationResult.ErrorMessage, @"Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                var rp = new crReporteDetraccionVentas();
                var ds = new DataSet();
                var dsReport = Utils.ConvertToDatatable(_dataReporte);
                ds.Tables.Add(dsReport);
                ds.Tables[0].TableName = "dsReporteVentaDetracciones";
                var dataEmpresa = new NodeBL().ReporteEmpresa().FirstOrDefault();
                var rango = string.Format("Del {0} al {1}", dtpFechaRegistroDe.Value.ToString("d"), dtpFechaRegistroAl.Value.ToString("d"));
                rp.SetDataSource(dsReport);
                if (dataEmpresa != null)
                {
                    rp.SetParameterValue("_NombreEmpresa", dataEmpresa.NombreEmpresaPropietaria);
                    rp.SetParameterValue("_RucEmpresa", string.Format("RUC: {0}", dataEmpresa.RucEmpresaPropietaria));
                }
                rp.SetParameterValue("_rangoFecha", rango);
                crystalReportViewer.ReportSource = rp;
                btnExcel.Enabled = _dataReporte.Any();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), @"Error al cargar reporte!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnVerReporte_Click(object sender, EventArgs e)
        {
            CargarReporte();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                const string dummyFileName = "Estado detracciones";
                var sf = new SaveFileDialog
                {
                    DefaultExt = "xlsx",
                    Filter = @"xlsx files (*.xlsx)|*.xlsx",
                    FileName = dummyFileName
                };
                var rango = string.Format("{0} al {1}", dtpFechaRegistroDe.Value.ToString("d"), dtpFechaRegistroAl.Value.ToString("d"));
                if (sf.ShowDialog() != DialogResult.OK) return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
