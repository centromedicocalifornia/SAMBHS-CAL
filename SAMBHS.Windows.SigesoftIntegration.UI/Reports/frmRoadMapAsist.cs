using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Windows.SigesoftIntegration.UI;

namespace SAMBHS.Windows.SigesoftIntegration.UI.Reports
{
    public partial class frmRoadMapAsist : Form
    {
        private string _serviceId;
        private string _calendarId;
        private DateTime _fechaNacimiento;

        public frmRoadMapAsist()
        {
            InitializeComponent();
        }

        public frmRoadMapAsist(string serviceId, string calendarId, DateTime fechaNacimiento)
        {
            InitializeComponent();
            _serviceId = serviceId;
            _calendarId = calendarId;
            _fechaNacimiento = fechaNacimiento;
        }

        //private void frmRoadMapAsist_Load(string serviceId, string calendarId, DateTime fechaNacimiento)
        //{
        //    InitializeComponent();
        //    _serviceId = serviceId;
        //    _calendarId = calendarId;
        //    _fechaNacimiento = fechaNacimiento;
        //}

        private void ShowReport()
        {
            // Cabecera
            var headerRoadMap = AgendaBl.GetHeaderRoadMap(_calendarId, _fechaNacimiento);
            // Detalle
            var detailRoadMap = AgendaBl.GetServiceComponentsByCategoryExceptLab_Asist(_serviceId);
            var rp = new SAMBHS.Windows.SigesoftIntegration.UI.Reports.crRoadMapAsist();

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

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            ShowReport();
        }

    }
}
