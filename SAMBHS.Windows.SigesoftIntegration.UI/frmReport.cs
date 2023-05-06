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
    public partial class frmReport : Form
    {
        public frmReport()
        {
            InitializeComponent();
        }

        private void ShowReport()
        {
            // Cabecera
            //var headerRoadMap = AgendaBl.Report();

            //var rp = new SAMBHS.Windows.SigesoftIntegration.UI.Reports.CrystalReport1();

            //DataSet ds = new DataSet();

            //DataTable dtHeader = UtilsSigesoft.ConvertToDatatable(headerRoadMap);

            //dtHeader.TableName = "dtReport";

            //ds.Tables.Add(dtHeader);
            //rp.SetDataSource(ds);

            //crystalReportViewer1.ReportSource = rp;
            //crystalReportViewer1.Show();
        }

        private void frmReport_Load(object sender, EventArgs e)
        {
            ShowReport();
        }
    }
}
