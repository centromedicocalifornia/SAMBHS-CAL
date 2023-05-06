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

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmRegistroPedido : Form
    {
        VendedorBL _objVendedorBL = new VendedorBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        string _strFilterExpression;
        public frmRegistroPedido(string Periodo)
        {
            InitializeComponent();
        }
        #region Cargar Load
        private void frmRegistroPedido_Load(object sender, EventArgs e)
        {
            #region Declaraciones / Referencias
            DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
            #endregion

            this.BackColor = new GlobalFormColors().FormColor;
            int Mes = int.Parse(DateTime.Today.Month.ToString());
            OperationResult objOperationResult = new OperationResult();
            dtpFechaRegistroDe.MinDate = DateTime.Parse((Globals.ClientSession.i_Periodo.ToString() + "/" + 01 + "/01").ToString());
            dtpFechaRegistroDe.MaxDate = DateTime.Parse((Globals.ClientSession.i_Periodo.ToString() + "/" + 12 + "/" + (DateTime.DaysInMonth(int.Parse(Globals.ClientSession.i_Periodo.ToString()),Mes)).ToString()).ToString());
            dtpFechaRegistroAl.MinDate = DateTime.Parse((Globals.ClientSession.i_Periodo.ToString() + "/" + 01 + "/01").ToString());
            dtpFechaRegistroAl.MaxDate = DateTime.Parse((Globals.ClientSession.i_Periodo.ToString() + "/" + 12 + "/" + (DateTime.DaysInMonth(int.Parse(Globals.ClientSession.i_Periodo.ToString()), Mes)).ToString()).ToString());
            Utils.Windows.LoadUltraComboEditorList (cboVendedor, "Value1", "Id", _objVendedorBL.ObtenerListadoVendedorParaCombo(ref objOperationResult, null), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboDetalleDocumento, "Value1", "Id", _objDocumentoBL.ObtenDocumentosParaCombo(ref objOperationResult, null, 0, 3), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 72, null), DropDownListAction.Select);
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            cboOrden.SelectedIndex = 1;
            cboVendedor.Value = "-1";
            cboDetalleDocumento.Value = "-1";

            //_ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosPedidosParaComboGrid(ref objOperationResult);
            //Utils.Windows.LoadUltraComboList(cboDetalleDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
        }
        #endregion

        #region Controles Botones
        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvDatos.Validate(true, false).IsValid)
                {
                    int Moneda;
                string ValorIdTipoDocumento, ValorIdVendedor, strOrderExpression;
                var rp = new Reportes.Ventas.crRegistroVenta();
                List<string> Filters = new List<string>();
                ValorIdTipoDocumento = cboDetalleDocumento.Value.ToString() == Constants.SelectValue ? cboDetalleDocumento.Value.ToString() : cboDetalleDocumento.Value.ToString();
                ValorIdVendedor = cboVendedor.Value.ToString() == Constants.SelectValue.ToString() ? Constants.SelectValue.ToString() : cboVendedor.Value.ToString();

                _strFilterExpression = null;
                Moneda = int.Parse(cboMoneda.Value.ToString());
                strOrderExpression = cboOrden.Value.ToString() == "-1" ? "" : cboOrden.Value.ToString();

                //strOrderExpression = RbtnDocumento.Checked == true ? "TipoDocumento" : "";
                //strOrderExpression += RbtnRegistro.Checked == true ? "CorrelativoDocumento" : "";
                //strOrderExpression += RbtnFecha.Checked == true ? "FechaRegistro" : "";

                if (Filters.Count > 0)
                {
                    foreach (string item in Filters)
                    {
                        _strFilterExpression = _strFilterExpression + item + " && ";
                    }
                    _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                }
                using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                    CargarReporte(DateTime.Today.Year.ToString(), DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text), Moneda, int.Parse(cboDetalleDocumento.Value.ToString()), chkHoraimpresion.Checked == true ? "1" : "0", ValorIdVendedor, strOrderExpression);

                }
                else {
                    UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            

            }
            catch
            {

                UltraMessageBox.Show("Se produjo un error en el reporte", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        #endregion
        #region Cargar Reporte

        private void CargarReporte(string Periodo, DateTime FechaRegistroIni, DateTime FechaRegistroFin, int IdMoneda, int IdTipoDocumento, string FechaHoraImpresion, string IdVendedor, string Orden)
        {
            var rp = new Reportes.Ventas.crRegistroPedido();
            ParameterFieldDefinitions crParameterFieldDefinitions;
            ParameterFieldDefinition crParameterFieldDefinition;
            ParameterValues crParameterValues;
            ParameterDiscreteValue crParameterDiscreteValue;

            var aptitudeCertificate = new PedidoBL().ReporteRegistroPedido(Periodo, 0, FechaRegistroIni, FechaRegistroFin, IdTipoDocumento, IdVendedor, Orden);
            var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();

            DataSet ds1 = new DataSet();
            DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
            DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);

            ds1.Tables.Add(dt2);
            ds1.Tables.Add(dt);

            ds1.Tables[0].TableName = "dsEmpresa";
            ds1.Tables[1].TableName = "dsRegistroPedido";
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
            crParameterDiscreteValue.Value = "Del " + FechaRegistroIni.Date.Day.ToString("00") + "/" + FechaRegistroIni.Date.Month.ToString("00") + "/" + FechaRegistroIni.Date.Year.ToString() + " Al " + FechaRegistroFin.Date.Day.ToString("00") + "/" + FechaRegistroFin.Date.Month.ToString("00") + "/"+FechaRegistroFin.Date.Year.ToString();  // TextBox con el valor del Parametro
            crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
            crParameterFieldDefinition = crParameterFieldDefinitions["Fecha"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;
            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            crParameterValues = new ParameterValues();
            crParameterDiscreteValue = new ParameterDiscreteValue();
            crParameterDiscreteValue.Value = aptitudeCertificate.Count () ;  // TextBox con el valor del Parametro
            crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
            crParameterFieldDefinition = crParameterFieldDefinitions["NroRegistros"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;
            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            crParameterValues = new ParameterValues();
            crParameterDiscreteValue = new ParameterDiscreteValue();
            crParameterDiscreteValue.Value =aptitudeCertificate2.FirstOrDefault ().NombreEmpresaPropietaria.Trim ()  ;  // TextBox con el valor del Parametro
            crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
            crParameterFieldDefinition = crParameterFieldDefinitions["NombreEmpresa"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;
            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

            crParameterValues = new ParameterValues();
            crParameterDiscreteValue = new ParameterDiscreteValue();
            crParameterDiscreteValue.Value =aptitudeCertificate2.FirstOrDefault ().RucEmpresaPropietaria.Trim () ;  // TextBox con el valor del Parametro
            crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
            crParameterFieldDefinition = crParameterFieldDefinitions["RucEmpresa"];
            crParameterValues = crParameterFieldDefinition.CurrentValues;
            crParameterValues.Clear();
            crParameterValues.Add(crParameterDiscreteValue);
            crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);


            crystalReportViewer1.ReportSource = rp;
            crystalReportViewer1.Show();

            crystalReportViewer1.Zoom(110);
        }

        #endregion


    }
}
