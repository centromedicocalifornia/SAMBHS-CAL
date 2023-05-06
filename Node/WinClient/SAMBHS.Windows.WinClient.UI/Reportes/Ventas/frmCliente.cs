using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using CrystalDecisions.Shared;
using System.Threading;
using System.IO;
using Infragistics.Documents.Excel;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmCliente : Form
    {
        #region Fields
        private readonly SystemParameterBL _objSystemParameterBl = new SystemParameterBL();
        private readonly DatahierarchyBL _objDatahierarchyBl = new DatahierarchyBL();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        #region Init & Load
        public frmCliente(string arg)
        {
            InitializeComponent();
        }

        private void frmCliente_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            var objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboTipoPersona, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 2, null), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 63, null), DropDownListAction.Select);
            cboOrden.SelectedIndex = 1;
            cboTipoPersona.Value = "-1";

            var listaPaises = (_objSystemParameterBl.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 112, null)).FindAll(p => p.Value3 == "-1");
            Utils.Windows.LoadUltraComboEditorList(ddlPais, "Value1", "Id", listaPaises, DropDownListAction.Select);
            //Combo Departamento
            Utils.Windows.LoadUltraComboEditorList(ddlDepartamento, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            //Combo Provincia
            Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);
            //Combo Distrito
            Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, null, 112, ""), DropDownListAction.Select);

            Utils.Windows.LoadUltraComboEditorList(cboListaPrecios, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 47, null), DropDownListAction.Select);
            ddlPais.Value = "1";
            ddlDepartamento.Value = "-1";
            ddlProvincia.Value = "-1";
            ddlDistrito.Value = "-1";
            cboListaPrecios.Value = "-1";
        }
        #endregion

        #region Cargar Reporte
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = (estado ? @"Generando... " : "") + @"Reporte Cliente";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = btnExcel.Enabled = !estado;
        }

        private void CargarReporte(bool export)
        {
            var rp = new crCliente();
            var strOrderExpression = cboOrden.Value.ToString();
            var filters = new Queue<string>();
            filters.Enqueue("v_FlagPantalla.Contains(\"C\")");
            var strFilterExpression = filters.Count > 0 ? string.Join(" && ", filters) : null;
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            List<ReporteCliente> aptitudeCertificate = null;
            var objOperationResult = new OperationResult();
            Task.Factory.StartNew(() =>
            {
                lock (objOperationResult)
                    aptitudeCertificate = new ClienteBL().ReporteCliente(ref objOperationResult, "" + strOrderExpression + " ASC", int.Parse(cboTipoPersona.Value.ToString()), strFilterExpression, int.Parse(ddlDepartamento.Value.ToString()), int.Parse(ddlProvincia.Value.ToString()), int.Parse(ddlDistrito.Value.ToString()),int.Parse ( cboListaPrecios.Value.ToString ()));
            }, _cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                lock (objOperationResult)
                    if (objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show("Sistema", "Error al Realizar Reporte Clientes", Icono: MessageBoxIcon.Error);
                        return;
                    }

                var dt = Utils.ConvertToDatatable(aptitudeCertificate);
                if (export)
                {
                    #region Headers
                    var columnas = new[]
                    {
                        "v_CodCliente", "NombreRazonSocial", "v_NroDocIdentificacion", "v_TipoPersona",
                        "v_Direccion", "Departamento", "Provincia", "Distrito","v_TelefonoFijo"
                    };
                    var heads = new ExcelHeader[]
                    {
                       "CODIGO", "CLIENTE", "RUC", "PERSONA", "DIRECCION", "DEPARTAMENTO", "PROVINCIA", "DISTRITO", "TELEFONO"
                    };
                    #endregion

                    var objexcel = new ExcelReport(dt)
                    {
                        Headers = heads,
                        FormaterHeader = format =>
                        {
                            format.Fill = CellFill.CreateSolidFill(Color.DodgerBlue);
                            format.Font.ColorInfo = Color.White;
                        }
                    };
                    objexcel.AutoSizeColumns(1, 10, 30, 15, 20, 30, 30, 30, 30, 20);
                    objexcel.SetTitle("CLIENTES");
                    objexcel.SetHeaders();
                    objexcel.SetData(ref objOperationResult ,columnas);
                    var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                    objexcel.Generate(path);
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                    var dt2 = Utils.ConvertToDatatable(aptitudeCertificate2);
                    dt.TableName = "dsCliente";
                    dt2.TableName = "dsEmpresa";
                    var ds1 = new DataSet();
                    ds1.Tables.Add(dt);
                    ds1.Tables.Add(dt2);
                    rp.SetDataSource(ds1);

                    var crParameterDiscreteValue = new ParameterDiscreteValue
                    {
                        Value = chkHoraimpresion.Checked ? "1" : "0"
                    };
                    // TextBox con el valor del Parametro
                    var crParameterFieldDefinitions = rp.DataDefinition.ParameterFields;
                    var crParameterFieldDefinition = crParameterFieldDefinitions["FechaHoraImpresion"];
                    var crParameterValues = crParameterFieldDefinition.CurrentValues;
                    crParameterValues.Clear();
                    crParameterValues.Add(crParameterDiscreteValue);
                    crParameterFieldDefinition.ApplyCurrentValues(crParameterValues);

                    rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count);
                    rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.First().NombreEmpresaPropietaria.Trim());
                    rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate2.First().RucEmpresaPropietaria.Trim());
                    crystalReportViewer1.ReportSource = rp;
                    crystalReportViewer1.Show();
                    crystalReportViewer1.Zoom(110);
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion

        #region Events
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvDatos.Validate(true, false).IsValid)
                    CargarReporte(sender == btnExcel);
                else
                    UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void frmCliente_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void ddlDepartamento_ValueChanged(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            if (ddlDepartamento.Value == null) return;
            Utils.Windows.LoadUltraComboEditorList(ddlProvincia, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult,
                (string)ddlDepartamento.Value == "-1" ? (int?)null : int.Parse(ddlDepartamento.Value.ToString()), 112, ""), DropDownListAction.Select);
            ddlProvincia.Value = "-1";
        }

        private void ddlProvincia_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlProvincia.Value == null) return;
            Utils.Windows.LoadUltraComboEditorList(ddlDistrito, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult,
                (string)ddlProvincia.Value == "-1" ? (int?)null : int.Parse(ddlProvincia.Value.ToString()), 112, ""), DropDownListAction.Select);
            ddlDistrito.Value = "-1";
        }

        private void ddlPais_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (ddlPais.Value == null) return;
            //si el combo esta en seleccione tengo que reiniciar el combo departamento
            Utils.Windows.LoadUltraComboEditorList(ddlDepartamento, "Value1", "Id", _objSystemParameterBl.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult,
                (string)ddlPais.Value == "-1" ? (int?)null : int.Parse(ddlPais.Value.ToString()), 112, ""), DropDownListAction.Select);
            ddlDepartamento.Value = "-1";
        }
        #endregion
    }
}
