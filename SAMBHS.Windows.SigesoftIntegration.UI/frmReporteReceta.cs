using CrystalDecisions.Shared;
using SAMBHS.Common.Resource;
using SAMBHS.Windows.SigesoftIntegration.UI.Reports;
//using Sigesoft.Node.Contasol.Integration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAMBHS.Windows.SigesoftIntegration.UI
{
    public partial class frmReporteReceta : Form
    {
        private readonly string _serviceId;
        private OperationResult _objOperationResult;
        private List<recetadespachoDto> _dataReporte;
        private readonly string _recomendaciones;
        private readonly string _restricciones;
        private readonly string _v_DiagnosticRepositoryId;

        public frmReporteReceta(string serviceId, string recomendaciones, string restricciones, string v_DiagnosticRepositoryId)
        {
            _serviceId = serviceId;
            _recomendaciones = recomendaciones;
            _restricciones = restricciones;
            InitializeComponent();
            _v_DiagnosticRepositoryId = v_DiagnosticRepositoryId;
            _objOperationResult = new OperationResult();
        }

        private void frmReporteReceta_Load(object sender, EventArgs e)
        {
            Cargar();
        }
        private void Cargar()
        {
            var objRecetaBl = new AgendaBl();

            string _ruta = GetApplicationConfigValue("Receta").ToString();

            DiskFileDestinationOptions objDiskOpt = new DiskFileDestinationOptions();
            try
            {
                Task.Factory.StartNew(() =>
                {
                    _dataReporte = objRecetaBl.GetRecetaToReport(_serviceId, _v_DiagnosticRepositoryId);

                }, TaskCreationOptions.LongRunning).ContinueWith(t =>
                {
                    var rp = new crRecetaPresentacion();
                    var ds = new DataSet();
                    var dsReport = UtilsSigesoft.ConvertToDatatable(_dataReporte);
                    ds.Tables.Add(dsReport);
                    ds.Tables[0].TableName = "dsReporteReceta";
                    rp.SetDataSource(dsReport);
                    rp.SetParameterValue("_Recomendaciones", _recomendaciones);
                    rp.SetParameterValue("_Restricciones", _restricciones);
                    crystalReportViewer1.ReportSource = rp;

                    rp.ExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    rp.ExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    objDiskOpt = new DiskFileDestinationOptions();
                    objDiskOpt.DiskFileName = _ruta + _serviceId + "-" + _v_DiagnosticRepositoryId + ".pdf";
                    rp.ExportOptions.DestinationOptions = objDiskOpt;
                    //rp.Export();
                    //rp.Close();
                    //rp.Dispose();
                },
                TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static string GetApplicationConfigValue(string nombre)
        {
            return Convert.ToString(ConfigurationManager.AppSettings[nombre]);
        }
    }
}
