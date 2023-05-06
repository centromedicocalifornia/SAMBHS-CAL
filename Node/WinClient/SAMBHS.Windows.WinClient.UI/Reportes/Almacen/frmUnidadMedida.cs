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
    public partial class frmUnidadMedida : Form
    {
        #region Declaraciones / Referencias
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        LineaBL _objLineaBL = new LineaBL();
        string _strFilterExpression;
        string _whereAlmacenesConcatenados;
        string _AlmacenesConcatenados;
        string strOrderExpression;
        string strGrupollave, strGrupollave2;
        string strNombreGrupollave, strNombreGrupollave2;
        List<string> Grupollave = new List<string>();
        List<string> NombreGrupollave = new List<string>();
        #endregion     
        #region Carga de inicializacion
        public frmUnidadMedida(string _IdUnidadMedida)
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

            OperationResult objOperationResult = new OperationResult();

            datahierarchyDto __datahierarchyDto = new datahierarchyDto();
            List<KeyValueDTO> _ListadoGrupos = new List<KeyValueDTO>();
            List<datahierarchyDto> _datahierarchyDto = new List<datahierarchyDto>();
            List<string> Retonar = new List<string>();
            List<string> Retonar2 = new List<string>();

            var rp = new Reportes.Almacen.crUnidadMedida();
            strOrderExpression = "";


            var aptitudeCertificate = new ProductoBL().ReporteUnidadMedida("" + strOrderExpression + " ASC");
            var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();

            DataSet ds1 = new DataSet();
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);


            ds1.Tables.Add(dt);
            ds1.Tables.Add(dt2);
            ds1.Tables[0].TableName = "dsUnidadMedida";
            ds1.Tables[1].TableName = "dsEmpresa";

            rp.SetDataSource(ds1);

            crParameterValues = new ParameterValues();
            crParameterDiscreteValue = new ParameterDiscreteValue();
            crParameterDiscreteValue.Value = chkHoraimpresion.Checked == true ? "1" : "0";  // TextBox con el valor del Parametro
            crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
            crParameterFieldDefinition = crParameterFieldDefinitions["FechaHoraImpresion"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;
            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());

            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
            crystalReportViewer1.Zoom(110);
        }
        #endregion
        #region Controles Botones
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            using (new LoadingClass.PleaseWait(this.Location, "Generando Reporte..."))
            CargarReporte();
        }
        #endregion

        private void frmUnidadMedida_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
        }
    }
}
