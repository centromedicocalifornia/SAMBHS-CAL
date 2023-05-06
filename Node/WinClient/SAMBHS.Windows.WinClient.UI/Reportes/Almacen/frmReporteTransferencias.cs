using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Almacen.BL;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmReporteTransferencias : Form
    {

        #region Declaraciones / Referencias
        LineaBL _objLineaBL = new LineaBL();
        readonly NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        string strOrderExpression;
        #endregion     
        #region Carga de inicializacion
        public frmReporteTransferencias(string _IdLinea)
        {
            InitializeComponent();
        }
        #endregion
        #region Cargar Reporte
        private void CargarReporte()
        {

            ParameterFieldDefinitions crParameterFieldDefinitions;
            ParameterFieldDefinition crParameterFieldDefinition;
            ParameterValues crParameterValues;
            ParameterDiscreteValue crParameterDiscreteValue;
            OperationResult objOperationResult= new OperationResult ();
            var rp = new Reportes.Almacen.crReporteTransferencias();
            strOrderExpression = "";


            //var aptitudeCertificate = new LineaBL().ReporteLinea( "" + strOrderExpression + " ASC" )
                 var aptitudeCertificate = new MovimientoBL ().ReporteTransferencias (ref objOperationResult,dtpFechaInicio.Value.Date ,DateTime.Parse (dtpFechaFin.Text +" 23:59"),  int.Parse ( cboAlmacen.Value.ToString ()));
            var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();

            DataSet ds1 = new DataSet();
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);


            ds1.Tables.Add(dt);
           // ds1.Tables.Add(dt2);
            ds1.Tables[0].TableName = "dsReporteTransferencias";
            //ds1.Tables[1].TableName = "dsEmpresa";

            rp.SetDataSource(ds1);

            crParameterValues = new ParameterValues();
            crParameterDiscreteValue = new ParameterDiscreteValue();
            //crParameterDiscreteValue.Value = chkHoraimpresion.Checked==true? "1" : "0";  // TextBox con el valor del Parametro
            //crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
            //crParameterFieldDefinition = crParameterFieldDefinitions["FechaHoraImpresion"];
            //crParameterValues = crParameterFieldDefinition.CurrentValues;
            //crParameterValues.Clear();
            //crParameterValues.Add(crParameterDiscreteValue);
            //crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            //rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());

            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
            crystalReportViewer1.Zoom(110);
        }
        #endregion
        #region Controles Botones
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            if (Validaciones.Validate(true, false).IsValid)
            {
                CargarReporte();
            }
        }
        #endregion

        private void frmReporteTransferencias_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null,-1), DropDownListAction.Select);
            cboAlmacen.Value = "-1";
        }

    }
}
