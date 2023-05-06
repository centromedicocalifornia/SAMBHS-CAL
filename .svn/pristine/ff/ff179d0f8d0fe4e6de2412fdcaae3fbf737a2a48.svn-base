using SAMBHS.Common.BE.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.BE;

namespace SAMBHS.Windows.WinClient.UI.Reportes
{
    public partial class frmViewRawReportWindow : Form
    {
        dynamic ListReport;

        public frmViewRawReportWindow(object Report, string formSource)
        {
            InitializeComponent();
            switch (formSource)
            {
                case "frmReporteAnalisisCuentas":
                    ListReport = (List<ReporteAnalisisCuentas>)Report;
                    break;

                case "frmReporteVentaSunat":
                    ListReport = (List<ReporteRegistroVentaContable>)Report;
                    break;

                case "frmReporteAnalisisCuentasCteAnalitico":
                    ListReport = (List<ReporteAnalisisCuentasCteAnalitico>)Report;
                    break;

                case "frmReporteBoletas":
                    ListReport = (List<ReportePlanillaBoleta>)Report;
                    break;

                default:
                    ListReport = new List<string>();
                    break;
            }
            ListReport = Report;
        }

        private void frmViewRawReportWindow_Load(object sender, EventArgs e)
        {
            dataGridView1.DataSource = ListReport;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
        }
    }
}
