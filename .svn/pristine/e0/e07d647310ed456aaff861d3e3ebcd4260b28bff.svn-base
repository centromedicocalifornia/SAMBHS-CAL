using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Venta.BL;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmRegistroVentaCrystal : Form
    {

        string Periodo,  FechaHoraImpresion, IdVendedor, Orden;
        DateTime? FechaRegistroIni, FechaRegistroFin;
        int IdMoneda, IdTipoDocumento;
        public frmRegistroVentaCrystal(string _Periodo, DateTime? _FechaRegistroIni, DateTime? _FechaRegistroFin, int _IdMoneda, int _IdTipoDocumento,string _IdVendedor,  string _FechaHoraImpresion,string _Orden)
        {
            InitializeComponent();

            Periodo = _Periodo;
            FechaRegistroIni = _FechaRegistroIni;
            FechaRegistroFin = _FechaRegistroFin;      
            IdMoneda = _IdMoneda;
            IdTipoDocumento = _IdTipoDocumento;
            FechaHoraImpresion = _FechaHoraImpresion;
            IdVendedor = _IdVendedor;
            Orden = _Orden;
        }
        #region Cargar Reporte
        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {
            Reporte();
        }
        #endregion
        #region Clases
        private void Reporte()
        {
            var rp = new Reportes.Ventas.crRegistroVenta();
            ParameterFieldDefinitions crParameterFieldDefinitions;
            ParameterFieldDefinition crParameterFieldDefinition;
            ParameterValues crParameterValues;
            ParameterDiscreteValue crParameterDiscreteValue;
            OperationResult objOperationResult = new OperationResult();
            //Order = "ASC" == "ASC" ? "ASC" : "DESC";

            var aptitudeCertificate = new VentaBL().ReporteRegistroVenta(ref  objOperationResult, 0, FechaRegistroIni, FechaRegistroFin, IdTipoDocumento, IdVendedor, Orden, "SIN AGRUPAR");

            DataSet ds1 = new DataSet();

            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

            dt.TableName = "dsRegistroVenta";
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
        }     
        #endregion
    }
}
