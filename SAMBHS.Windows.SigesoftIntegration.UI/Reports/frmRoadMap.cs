using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Windows.SigesoftIntegration.UI;

namespace Sigesoft.Node.WinClient.UI.Reports
{
    public partial class frmRoadMap : Form
    {
        private string _serviceId;
        private string _calendarId;
        private DateTime _fechaNacimiento;

        public frmRoadMap()
        {
            InitializeComponent();        
        }

        public frmRoadMap(string serviceId, string calendarId, DateTime fechaNacimiento)
        {
            InitializeComponent();
            _serviceId = serviceId;
            _calendarId = calendarId;
            _fechaNacimiento = fechaNacimiento;
        }

        private void ShowReport()
        {
            // Cabecera
            var headerRoadMap = AgendaBl.GetHeaderRoadMap(_calendarId, _fechaNacimiento);
            // Detalle
            var detailRoadMap = AgendaBl.GetServiceComponentsByCategoryExceptLab(_serviceId);
            var rp = new SAMBHS.Windows.SigesoftIntegration.UI.Reports.crRoadMap();

            DataSet ds = new DataSet();

            DataTable dtHeader = UtilsSigesoft.ConvertToDatatable(headerRoadMap);
            DataTable dtDetail = UtilsSigesoft.ConvertToDatatable(detailRoadMap);
            
            dtHeader.TableName = "dtHeaderRoadMap";
            dtDetail.TableName = "dtDetailRoadMap";

            ds.Tables.Add(dtHeader);
            ds.Tables.Add(dtDetail);
            rp.SetDataSource(ds);

            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
        }

        private void frmRoadMap_Load(object sender, EventArgs e)
        {
            ShowReport();
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }  

    }
}
