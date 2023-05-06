using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Tesoreria.BL;
using SAMBHS.Common.BL;
using CrystalDecisions.CrystalReports.Engine;
namespace SAMBHS.Windows.WinClient.UI.Reportes.Tesoreria
{
    public partial class frmComprobanteTesoreria : Form
    {

        public string pstrIdTesoreria = string.Empty;
        public bool esIngreso = false;
        public frmComprobanteTesoreria(string IdTesoreria, bool Ingreso)
        {
            InitializeComponent();
            pstrIdTesoreria = IdTesoreria;
            esIngreso = Ingreso;
        }

        private void frmComprobanteTesoreria_Load(object sender, EventArgs e)
        {
            ReportDocument rp = new ReportDocument();

            if (esIngreso)
            {

                rp =  new Reportes.Tesoreria.crComprobanteTesoreria();
            }
            else
            {
                rp = new Reportes.Tesoreria.crComprobanteTesoreriaEgreso();
            }
            var aptitudeCertificate = new TesoreriaBL().ReporteComprobanteTesoreria(pstrIdTesoreria);
            var aptitudeCertificate1 = new NodeBL().ReporteEmpresa();

            DataSet ds1 = new DataSet();

            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            DataTable dt1 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate1);

            ds1.Tables.Add(dt);
            ds1.Tables.Add(dt1);
            ds1.Tables[0].TableName = "dsReporteComprobanteTesoreria";
            ds1.Tables[1].TableName = "dsEmpresa";




            rp.SetDataSource(ds1);
            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
        }



    }
}
