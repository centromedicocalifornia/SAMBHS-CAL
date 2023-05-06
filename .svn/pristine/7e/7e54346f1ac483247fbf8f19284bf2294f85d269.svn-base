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
using SAMBHS.Compra.BL;
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

namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    public partial class frmReporteRegistroCompra : Form
    {
        //VendedorBL _objVendedorBL = new VendedorBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        string _strFilterExpression;
        public int ConsiderarDocInternos = -1;
        public frmReporteRegistroCompra(string Modalidad)
        {
            InitializeComponent();
            ConsiderarDocInternos = Modalidad == "C" ? 0 : 1;
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            int Moneda;
            string ValorIdTipoDocumento, ValorIdVendedor, strOrderExpression;
            var rp = new Reportes.Ventas.crRegistroVenta();
            List<string> Filters = new List<string>();
            //ValorIdTipoDocumento = cboDetalleDocumento.SelectedValue.ToString() == Constants.SelectValue ? cboDetalleDocumento.SelectedValue.ToString() : cboDetalleDocumento.SelectedValue.ToString();
            //ValorIdVendedor = cboVendedor.SelectedValue.ToString() == Constants.SelectValue.ToString() ? Constants.SelectValue.ToString() : cboVendedor.SelectedValue.ToString();
            Moneda = RbtnSoles.Checked == true ? (int)Currency.Soles : (int)Currency.Dolares;
            _strFilterExpression = null;
            strOrderExpression = RbtnDocumento.Checked == true ? "TipoDocumento" : "";
            strOrderExpression += RbtnRegistro.Checked == true ? "CorrelativoDocumento" : "";
            strOrderExpression += RbtnFecha.Checked == true ? "FechaRegistro" : "";

            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }
            using (new LoadingClass.PleaseWait(this.Location, "Generando Reporte..."))
           CargarReporte(DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text+" 23:59"), Moneda, -1, chkHoraimpresion.Checked == true ? "1" : "0", "", "",strOrderExpression);

        }
        private void CargarReporte(DateTime FechaRegistroIni, DateTime FechaRegistroFin, int IdMoneda, int IdTipoDocumento, string FechaHoraImpresion, string IdVendedor, string NroCuenta, string Orden)
        {
            var rp = new Reportes.Ventas.crRegistroVenta();
          

            ParameterFieldDefinitions crParameterFieldDefinitions;
            ParameterFieldDefinition crParameterFieldDefinition;
            ParameterValues crParameterValues;
            ParameterDiscreteValue crParameterDiscreteValue;

            OperationResult objOperationResult = new OperationResult();
            //Order = "ASC" == "ASC" ? "ASC" : "DESC";

            var aptitudeCertificate = new ComprasBL().ReporteRegistroCompraProveedorResumen(ref objOperationResult ,  0, FechaRegistroIni, FechaRegistroFin, IdTipoDocumento, IdVendedor,NroCuenta, Orden,"","",ConsiderarDocInternos,"");

            DataSet ds1 = new DataSet();

            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

            dt.TableName = "dsRegistroCompra.";
            ds1.Tables.Add(dt);
            rp.SetDataSource(ds1);

            crParameterValues = new ParameterValues();
            crParameterDiscreteValue = new ParameterDiscreteValue();
            crParameterDiscreteValue.Value = FechaHoraImpresion;  // TextBox con el valor del Parametro
            crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
            crParameterFieldDefinition = crParameterFieldDefinitions["FechaHoraImpresion"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;
            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            crParameterValues = new ParameterValues();
            crParameterDiscreteValue = new ParameterDiscreteValue();
            crParameterDiscreteValue.Value = IdMoneda;  // TextBox con el valor del Parametro
            crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
            crParameterFieldDefinition = crParameterFieldDefinitions["IdMoneda"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;
            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            crParameterValues = new ParameterValues();
            crParameterDiscreteValue = new ParameterDiscreteValue();
            crParameterDiscreteValue.Value = FechaRegistroIni;  // TextBox con el valor del Parametro
            crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
            crParameterFieldDefinition = crParameterFieldDefinitions["FechaRegistroIni"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;
            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            crParameterValues = new ParameterValues();
            crParameterDiscreteValue = new ParameterDiscreteValue();
            crParameterDiscreteValue.Value = FechaRegistroFin;  // TextBox con el valor del Parametro
            crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
            crParameterFieldDefinition = crParameterFieldDefinitions["FechaRegistroFin"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;
            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();

            crystalReportViewer1.Zoom(110);
        }

        private void frmRegistroCompra_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
        }     
        
    }
}
