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
using SAMBHS.Cobranza.BL;
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
namespace SAMBHS.Windows.WinClient.UI.Reportes.Cobranza
{
    public partial class frmDocumentoVoucherAdelantos : Form
    {
        public string IdAdelanto;
        public frmDocumentoVoucherAdelantos(string pstrIdAdelanto)
        {
            InitializeComponent();
            IdAdelanto = pstrIdAdelanto;
        }

        private void frmDocumentoVoucherAdelantos_Load(object sender, System.EventArgs e)
        {

            var rp = new Reportes.Cobranza.crDocumentoVoucherAdelantos();
            var aptitudeCertificate = new AdelantoBL().ReporteDocumentoVoucherAdelanto(IdAdelanto);

            DataSet ds1 = new DataSet();

            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

            dt.TableName = "dsDocumentoVoucherAdelantos";
            ds1.Tables.Add(dt);
            rp.SetDataSource(ds1);


            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        private void frmDocumentoVoucherAdelantos_Load_1(object sender, EventArgs e)
        {

        }




    }
}
