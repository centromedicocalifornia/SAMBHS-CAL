#region Name Space
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
#endregion

namespace SAMBHS.Windows.WinClient.UI.Reportes.Compras
{
    public partial class frmProveedor : Form
    {
        #region Declaraciones / Referencias
        ClienteBL _objLineaBL = new ClienteBL();
        string strOrderExpression;
        string _strFilterExpression;
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        #endregion 
        #region Carga de inicializacion
        public frmProveedor(string parametro)
        {
            InitializeComponent();
        }
        #endregion
        #region Cargar Load
        private void frmProveedor_Load(object sender, EventArgs e)
        {

            this.BackColor = new GlobalFormColors().FormColor;

            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadDropDownList(cboTipoPersona, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 2, null), DropDownListAction.All);
            Utils.Windows.LoadDropDownList(cboOrden, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 63, null), DropDownListAction.Select);
            cboOrden.SelectedIndex = 2;
        
        }
        #endregion
        #region Cargar Reporte
        private void CargarReporte()
        {

            ParameterFieldDefinitions crParameterFieldDefinitions;
            ParameterFieldDefinition crParameterFieldDefinition;
            ParameterValues crParameterValues;
            ParameterDiscreteValue crParameterDiscreteValue;
            OperationResult objOpertionResult = new OperationResult();
            var rp = new Reportes.Compras.crProveedor();
            strOrderExpression = cboOrden.SelectedValue.ToString();

            List<string> Filters = new List<string>();
            Filters.Add("v_FlagPantalla.Contains(\"V\")");

            _strFilterExpression = null;
            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }
            var aptitudeCertificate = new ClienteBL().ReporteCliente( ref objOpertionResult,"" + strOrderExpression + " ASC", int.Parse(cboTipoPersona.SelectedValue.ToString()), _strFilterExpression,-1,-1,-1,-1);
            var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();


            DataSet ds1 = new DataSet();
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);


            ds1.Tables.Add(dt);
            ds1.Tables.Add(dt2);
            ds1.Tables[0].TableName = "dsCliente";
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

            rp.SetParameterValue("NroRegistros",aptitudeCertificate.Count ());

            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();
            crystalReportViewer1.Zoom(110);
        }
        #endregion
        #region Controles Botones
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            try
            {

                if (uvDatos.Validate(true, false).IsValid)
                {
                    using (new LoadingClass.PleaseWait(this.Location, "Generando Reporte..."))
                    CargarReporte();
                }
                else
                {
                    UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }


            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
