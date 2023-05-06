
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
    public partial class frmReporteVentasCobranzasCrystal : Form
    {
        public DateTime FechaCobranza;
        public int IdTipoDocCobranza;
        public string Orden_,strIdCliente;
        //public int strFilterExpression;
        public frmReporteVentasCobranzasCrystal(DateTime dtFechaCobranza, int IdTipoDocumentoCobranza,string Orden,string pstrIdCliente)
        {
            InitializeComponent();
            FechaCobranza = dtFechaCobranza;
            strIdCliente = pstrIdCliente;
            IdTipoDocCobranza = IdTipoDocumentoCobranza;
            Orden_=Orden;
        }

        private void frmReporteVentasCobranzasCrystal_Load(object sender, EventArgs e)
        {
            ParameterFieldDefinitions crParameterFieldDefinitions;
            ParameterFieldDefinition crParameterFieldDefinition;
            ParameterValues crParameterValues;
            ParameterDiscreteValue crParameterDiscreteValue;

            OperationResult objOperationResult = new OperationResult();
            var rp = new Reportes.Cobranza.crReporteVentaCobranza_();

        
            //Order = "ASC" == "ASC" ? "ASC" : "DESC";

            var aptitudeCertificate = new CobranzaBL().ReporteVentasCobranzas(ref objOperationResult , FechaCobranza, IdTipoDocCobranza,Orden_,strIdCliente );

            DataSet ds1 = new DataSet();

            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

            dt.TableName = "dsReporteVentaCobranza";
            ds1.Tables.Add(dt);
            rp.SetDataSource(ds1);

     
            //crParameterValues = new ParameterValues();
            //crParameterDiscreteValue = new ParameterDiscreteValue();
            //crParameterDiscreteValue.Value = FechaRegistroFin;  // TextBox con el valor del Parametro
            //crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
            //crParameterFieldDefinition = crParameterFieldDefinitions["FechaRegistroFin"];
            //crParameterValues = crParameterFieldDefinition.CurrentValues;
            //crParameterValues.Clear();
            //crParameterValues.Add(crParameterDiscreteValue);
            //crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);



            crParameterValues = new ParameterValues();
            crParameterDiscreteValue = new ParameterDiscreteValue();
            crParameterDiscreteValue.Value = (int)Globals.ClientSession.i_PrecioDecimales;  // TextBox con el valor del Parametro
            crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
            crParameterFieldDefinition = crParameterFieldDefinitions["CantidadDecimalPrecio"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;
            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

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

    }
}
