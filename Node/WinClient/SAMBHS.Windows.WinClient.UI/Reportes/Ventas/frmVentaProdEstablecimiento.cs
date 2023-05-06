using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmVentaProdEstablecimiento : Form
    {
        #region Fields
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        #region Init
        public frmVentaProdEstablecimiento(string arg)
        {
            InitializeComponent();
            Load += frmVentaProdEstablecimiento_Load;
            FormClosing += frmVentaProdEstablecimiento_FormClosing;
        }

        private void frmVentaProdEstablecimiento_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            var _objDatahierarchyBL = new DatahierarchyBL();
            var objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboConsiderar, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 84, null), DropDownListAction.Select);
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            cboConsiderar.Value = "1";
            dtpFechaRegistroDe.MinDate = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString());
            dtpFechaRegistroDe.MaxDate = dtpFechaRegistroAl.Value;
        }
        #endregion

        #region Methods
        private void OcultarMostrarBuscar(bool estado)
        {
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = !estado;
        }
        private void CargarReporte(int stock, bool exportExcel)
        {
            OperationResult objOperationResult = new OperationResult();             
            DateTime FechaInicial = dtpFechaRegistroDe.Value.Date;
            DateTime FechaFinal = DateTime.Parse(dtpFechaRegistroAl.Value.ToString("dd/MM/yyyy") + " 23:59");
            Cursor.Current = Cursors.WaitCursor;
            OcultarMostrarBuscar(true);
            List<ReporteStockConsolidado> aptitudeCertificate = null;

            Task.Factory.StartNew(() =>
            {
                var bl = new Venta.BL.VentaBL();
                aptitudeCertificate = bl.ReporteRegistroVentaProductoEmpresa(ref objOperationResult, FechaInicial, FechaFinal, int.Parse(cboMoneda.Value.ToString()), 
                    txtCodigoProducto.Text.Trim(), 1, int.Parse(cboConsiderar.Value.ToString()));
            }, _cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;

                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show("Ocurrió un error al realizar el Reporte" + Environment.NewLine + 
                    objOperationResult.ExceptionMessage, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                try
                {
                    var dt = Utils.ConvertToDatatable(aptitudeCertificate);
                    if (exportExcel)
                    {
                        #region Headers
                        if (dt.Rows.Count == 0)
                        {
                            UltraMessageBox.Show("No se encontro informacion!", "Report To Excel");
                            return;
                        }
                        var cols = new List<string> { "codigoProducto", "descripcionProducto" };
                        var heads = new List<ExcelHeader> { "CODIGO", "DESCRIPCION" };

                        DataRow r = dt.Rows[0];
                        var countAlmacen = 0;
                        for (byte i = 1; i <= 10; i++)
                        {
                            var str = r["almacen" + i].ToString();
                            if (!string.IsNullOrEmpty(str))
                            {
                                cols.Add("cantidadAlmacen" + i);
                                heads.Add(str);
                                countAlmacen++;
                            }
                        }
                        //heads.Add("TOTAL");
                        #endregion

                        var objexcel = new ExcelReport(dt)
                        {
                            Headers = heads.ToArray(),
                            FormaterHeader = (cell) =>
                            {
                                cell.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(Color.Black);
                                cell.Font.ColorInfo = Color.WhiteSmoke;
                            }
                        };

                        objexcel.AutoSizeColumns(1, 20, 50);
                        objexcel.AutoSizeColumns(3, Enumerable.Range(0, cols.Count - 1).Select(n => 20).ToArray());
                        objexcel.SetTitle("VENTA POR PRODUCTO - ESTABLECIMIENTO");
                        objexcel.SetHeaders();
                        objexcel.SetData(ref objOperationResult ,cols.ToArray());
                        var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                        objexcel.Generate(path);
                        System.Diagnostics.Process.Start(path);
                    }
                    else
                    {
                        var aptitudeCertificate1 = new NodeBL().ReporteEmpresa();
                        DataSet ds1 = new DataSet();
                        DataTable dt2 = Utils.ConvertToDatatable(aptitudeCertificate1);
                        ds1.Tables.Add(dt);
                        ds1.Tables.Add(dt2);
                        ds1.Tables[0].TableName = "dsReporteStockConsolidado";
                        ds1.Tables[1].TableName = "dsEmpresa";
                        var rp = new crReporteVentaProdEstablecimiento();
                        rp.SetDataSource(ds1);
                        rp.SetParameterValue("MostrarStockCero", 0);
                        rp.SetParameterValue("Fecha", string.Format("AL {0:00}/{1:00}/{2}", dtpFechaRegistroDe.Value.Date.Day, dtpFechaRegistroAl.Value.Date.Month, dtpFechaRegistroAl.Value.Date.Year));
                        //rp.SetParameterValue("Establecimiento", "IMM");
                        rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count);
                        //rp.SetParameterValue("NombreEmpresa", Globals.ClientSession.v_CurrentExecutionNodeName);
                        //rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Globals.ClientSession.v_RucEmpresa);
                        crystalReportViewer1.ReportSource = rp;
                        crystalReportViewer1.Show();
                    }
                }catch(Exception)
                {

                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            
        }

        #endregion
        
        #region Events
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            if(uvDatos.Validate().IsValid)
                CargarReporte(0, sender == btnExcel);
        }
        private void frmVentaProdEstablecimiento_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void txtCodigoProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
            frm.ShowDialog();

            if (!string.IsNullOrEmpty(frm._IdProducto))
            {
                txtCodigoProducto.Text = frm._CodigoInternoProducto.Trim();
            }
            else
            {
                txtCodigoProducto.Text = string.Empty;
            }
        }
        private void dtpFechaRegistroAl_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaRegistroDe.MaxDate = dtpFechaRegistroAl.Value;
            if (dtpFechaRegistroDe.Value > dtpFechaRegistroDe.MaxDate)
                dtpFechaRegistroDe.Value = dtpFechaRegistroDe.MaxDate;
        }
        #endregion
    }
}
