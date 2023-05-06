using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using System.Threading;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using System.Drawing;


namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmRegistroVenta : Form
    {
        #region Fields
        private readonly VendedorBL _objVendedorBl = new VendedorBL();
        private readonly DocumentoBL _objDocumentoBl = new DocumentoBL();
        private readonly DatahierarchyBL _objDatahierarchyBl = new DatahierarchyBL();
        private readonly EstablecimientoBL _objEstablecimientoBl = new EstablecimientoBL();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        #region Init & Load
        public frmRegistroVenta(string periodo)
        {
            InitializeComponent();
        }

        private void frmRegistroVenta_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            ValidarFecha();
            cboAgrupar.Text =Constants.RucDetec ==Globals.ClientSession.v_RucEmpresa?"MONEDA" : "SIN AGRUPAR";
        }
        #endregion

        #region Events
        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvDatos.Validate(true, false).IsValid)
                {
                    var valorIdVendedor = cboVendedor.Value.ToString() == Constants.SelectValue ? Constants.SelectValue : cboVendedor.Value.ToString();
                    var moneda = int.Parse(cboMoneda.Value.ToString());
                    var strOrderExpression = cboOrden.Value.ToString() == "-1" ? "" : cboOrden.Value.ToString();
                    CargarReporte(DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text+ " 23:59"), moneda, int.Parse(cboDetalleDocumento.Value.ToString()),
                        chkHoraimpresion.Checked ? "1" : "0", valorIdVendedor, strOrderExpression, sender == btnExcel);
                }
                else
                {
                    UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {

                UltraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dtpFechaRegistroAl_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaRegistroDe.MaxDate = dtpFechaRegistroAl.Value;
        }

        private void frmRegistroVenta_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }
        private void cboEstablecimiento_ValueChanged(object sender, EventArgs e)
        {
            if (cboEstablecimiento.Value == null) return;
            var x = new EstablecimientoBL().GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.All);
            cboAlmacen.Value = "-1";
        }
        #endregion

        #region Methods
        private void CargarCombos()
        {
            var objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", _objVendedorBl.ObtenerListadoVendedorParaCombo(ref objOperationResult, null), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboDetalleDocumento, "Value1", "Id", _objDocumentoBl.ObtenDocumentosParaCombo(ref objOperationResult, null, 0, 1), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 67, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null).OrderBy(r => r.Id).ToList(), DropDownListAction.Select);
            cboEstablecimiento.Value = (Globals.ClientSession.i_IdEstablecimiento ?? 1).ToString();
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            cboOrden.SelectedIndex = 1;
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objEstablecimientoBl.GetAlmacenesXEstablecimiento(Globals.ClientSession.i_IdEstablecimiento ?? 1), DropDownListAction.All);
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            cboVendedor.Value = "-1";
            cboDetalleDocumento.Value = "-1";

        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = (estado ? @"Generando... "  : "") + @"Reporte de Ventas";
            pBuscando.Visible = estado;

            BtnImprimir.Enabled = btnExcel.Enabled = !estado;
        }
        private void CargarReporte( DateTime fechaRegistroIni, DateTime fechaRegistroFin, int idMoneda, int idTipoDocumento, string fechaHoraImpresion, string idVendedor, string orden, bool export)
        {

            ReportDocument rp = new ReportDocument();
            if (cboAgrupar.Text == "SIN AGRUPAR")
                rp = new crRegistroVenta();
            else
                rp = new crRegistroVentaAmbasMonedas();
            var objOperationResult= new OperationResult ();
            var aptitudeCertificate= new List<ReporteRegistroVenta>();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() => aptitudeCertificate = new VentaBL().ReporteRegistroVenta( ref objOperationResult, int.Parse(cboEstablecimiento.Value.ToString()), fechaRegistroIni, fechaRegistroFin, idTipoDocumento, idVendedor, orden,cboAgrupar.Text), _cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show((objOperationResult.ExceptionMessage ?? objOperationResult.ErrorMessage) + "\n\nTARGET: " + 
                        objOperationResult.AdditionalInformation, "Error " + " Reporte Kardex Físico / Valorizado", Icono:MessageBoxIcon.Error);
                    return;
                }


                var dt = Utils.ConvertToDatatable(aptitudeCertificate);
                if (export)
                {
                    #region Headers
                    var columnas = new[]
                    {
                        "FechaRegistro", "TipoDocumento", "NombreDocumento", "NombreCliente", "NroDocCliente",
                        "NombreVendedor", "ValorVenta", "Igv", "Total", "Moneda"
                    };
                    var heads = new ExcelHeader[]
                    {
                        "FECHA", "DOC.", "NUMERO", "CLIENTE", "RUC", "VENDEDOR", "SUBTOTAL", "IGV", "TOTAL", "M"
                    };
                    #endregion

                    var objexcel = new ExcelReport(dt) { Headers = heads };
                    objexcel.AutoSizeColumns(1, 12, 10, 25, 50, 15, 15, 20, 20, 20, 8);
                    objexcel.SetTitle("REPORTE DE VENTAS");
                    objexcel.SetHeaders();
                    objexcel.EndSection += (_, e) =>
                    {
                        var obj = (ExcelReport)_;
                        obj.SetFormulas(6, "SUB TOTAL:", Enumerable.Range(0, 3).Select(i => string.Format("=SUM(${2}{0}:${2}{1})", e.StartPosition + 1, e.EndPosition, (char)('H' + i))).ToArray());
                        obj.CurrentPosition++;
                    };
                    objexcel.SetData(ref objOperationResult ,columnas);
                    var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                    objexcel.Generate(path);
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                    var ds1 = new DataSet();
                    var dt2 = Utils.ConvertToDatatable(aptitudeCertificate2);

                    dt.TableName = "dsRegistroVenta";
                    dt2.TableName = "dsEmpresa";
                    ds1.Tables.Add(dt);
                    ds1.Tables.Add(dt2);
                    rp.SetDataSource(ds1);

                    rp.SetParameterValue("FechaHoraImpresion", fechaHoraImpresion);
                    rp.SetParameterValue("IdMoneda", idMoneda);
                    rp.SetParameterValue("Fecha", "DEL " + fechaRegistroIni.Date.Day.ToString("00") + "/" + fechaRegistroIni.Date.Month.ToString("00") + "/" + fechaRegistroIni.Date.Year.ToString() + " AL " + fechaRegistroFin.Date.Day.ToString("00") + "/" + fechaRegistroFin.Date.Month.ToString("00") + "/" + fechaRegistroFin.Date.Year.ToString());
                    rp.SetParameterValue("NombreEmpresaPropietaria", aptitudeCertificate2.First().NombreEmpresaPropietaria);
                    rp.SetParameterValue("RucEmpresaPropietaria", aptitudeCertificate2.First().RucEmpresaPropietaria);
                    rp.SetParameterValue("NumeroRegistros", aptitudeCertificate.Count);

                    crystalReportViewer1.ReportSource = rp;
                    crystalReportViewer1.Show();

                    crystalReportViewer1.Zoom(110);
                }

            }
                , TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void ValidarFecha()
        {
            var periodo = Globals.ClientSession.i_Periodo.ToString();
            var mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == periodo)
            {
                dtpFechaRegistroDe.MinDate = DateTime.Parse(periodo + "/01/01");
                dtpFechaRegistroDe.MaxDate = DateTime.Parse(periodo + "/" + mes + "/" + DateTime.Now.Date.Day);
                dtpFechaRegistroDe.Value = DateTime.Parse(periodo + "/" + mes + "/" + DateTime.Now.Date.Day);
                dtpFechaRegistroAl.MinDate = DateTime.Parse(periodo + "/01/01");
                dtpFechaRegistroAl.MaxDate = DateTime.Parse(periodo + "/" + mes + "/" + DateTime.Now.Date.Day);
                dtpFechaRegistroAl.Value = DateTime.Parse(periodo + "/" + mes + "/" + DateTime.Now.Date.Day);

            }
            else
            {
                dtpFechaRegistroDe.MinDate = DateTime.Parse(periodo + "/01/01");
                dtpFechaRegistroDe.MaxDate = DateTime.Parse(periodo + "/12/" + DateTime.DaysInMonth(int.Parse(periodo), 12));
                dtpFechaRegistroDe.Value = DateTime.Parse(periodo + "/" + mes + "/" + DateTime.Now.Date.Day);
                dtpFechaRegistroAl.MinDate = DateTime.Parse(periodo + "/01/01");
                dtpFechaRegistroAl.MaxDate = DateTime.Parse(periodo + "/12/" + DateTime.DaysInMonth(int.Parse(periodo), 12));
                dtpFechaRegistroAl.Value = DateTime.Parse(periodo + "/" + mes + "/" + DateTime.Now.Date.Day);
            }
        }
        #endregion

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded)
            {
                groupBox1.Location = new Point(groupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox1.Height = Height - groupBox1.Location.Y - 7;
            }
            else
            {
                groupBox1.Location = new Point(groupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox1.Height = Height - groupBox1.Location.Y - 7;
            }
        }
    }
}
