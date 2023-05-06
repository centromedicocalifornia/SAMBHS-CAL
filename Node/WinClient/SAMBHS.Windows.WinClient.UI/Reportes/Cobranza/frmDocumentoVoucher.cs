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
    public partial class frmDocumentoVoucher : Form
    {
        string _pstrIdVenta;
        string _pstrOrigen;
        public frmDocumentoVoucher(string pstrIdVenta, int numeroDecimales,string Origen)
        {
            InitializeComponent();
            _pstrIdVenta = pstrIdVenta;
            _pstrOrigen = Origen;
        }

        private void frmDocumentoVoucher_Load(object sender, EventArgs e)
        {

            try
            {
                ParameterFieldDefinitions crParameterFieldDefinitions;
                ParameterFieldDefinition crParameterFieldDefinition;
                ParameterValues crParameterValues;
                ParameterDiscreteValue crParameterDiscreteValue;

                List<ReporteDocumentoVoucher> x = new List<ReporteDocumentoVoucher>();


                //ReportDocument rp = new ReportDocument();
                CobranzaBL objCobranzaBL = new CobranzaBL();

                OperationResult objOperationResult = new OperationResult();
                DataSet ds = new DataSet();

               ReportDocument rp= new ReportDocument();

                //var _ocrPaymentVoucher = new Reportes.Cobranza.crDocumentoVoucher();


                var Empresa = new NodeBL().ReporteEmpresa();
              //  rp = _ocrPaymentVoucher;
                var fullPathApp = Application.StartupPath;




                if (_pstrOrigen == "COBRANZA RAPIDA")
                {
                    rp = new Reportes.Cobranza.crDocumentoVoucherRapida();
                    x = objCobranzaBL.ReporteDocumentoVoucherCobranzaRapida(_pstrIdVenta);
                }
                else
                {
                   rp = new Reportes.Cobranza.crDocumentoVoucher();
                    x = objCobranzaBL.ReporteDocumentoVoucherCobranza(_pstrIdVenta);
                }

                var dt = ToDataTable(x);
                dt.TableName = "dsDocumentoVoucher";
                ds.Tables.Add(dt);
           
                rp.SetDataSource(ds);
                crParameterValues = new ParameterValues();
                crParameterDiscreteValue = new ParameterDiscreteValue();
                crParameterDiscreteValue.Value = _pstrOrigen;  // TextBox con el valor del Parametro
                crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                crParameterFieldDefinition = crParameterFieldDefinitions["Origen"];
                crParameterValues = crParameterFieldDefinition.CurrentValues;
                crParameterValues.Clear();
                crParameterValues.Add(crParameterDiscreteValue);
                crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

                rp.SetParameterValue("NombreEmpresa",Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim ());
                rp.SetParameterValue("RucEmpresa","R.U.C. : " + Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());

                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
            catch (Exception g)
            {
                UltraMessageBox.Show("Se generó un error al Imprimir ", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }

            
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

        private void frmDocumentoVoucher_Load_1(object sender, EventArgs e)
        {

        }
        
    }
}
