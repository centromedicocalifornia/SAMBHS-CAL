using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using System.Text.RegularExpressions;
using Infragistics.Win.UltraWinMaskedEdit;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.Sql;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Configuration;
using SAMBHS.Security.BL;
using CrystalDecisions.Shared;
using System.Reflection;
namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmParteDiarioVentasCrystal : Form
    {
        public frmParteDiarioVentasCrystal(string IdVenta)
        {
            InitializeComponent();
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            var rp = new Reportes.Ventas.crRegistroVenta();

            var aptitudeCertificate = new VentaBL().ReporteRegistroVenta(ref objOperationResult, 0, DateTime.Today, DateTime.Today, 0, "0", "", "SIN AGRUPAR");
            DataSet ds1 = new DataSet();

            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

            dt.TableName = "dsRegistroVenta";

            ds1.Tables.Add(dt);
            rp.SetDataSource(ds1);

            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
        }
    }
}
