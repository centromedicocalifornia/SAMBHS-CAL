using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmProductoReposicion : Form
    {
        #region Fields
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        #region Construct
        public frmProductoReposicion(string arg)
        {
            InitializeComponent();
            Load += frmProductoReposicion_Load;
        }

        private void frmProductoReposicion_Load(object sender, System.EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            var objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", new LineaBL().LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.All);
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboLinea.Value = "-1";
            txtCodigoProducto.Tag = string.Empty;
        }
        #endregion

        #region Methods
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = (estado ? @"Generando... " : string.Empty) + @"Reporte de Consolidado Almacenes";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = btnExcel.Enabled = !estado;
        }

        private void LoadReport(bool export)
        {
            OcultarMostrarBuscar(true);
            var objOperationResult = new OperationResult();
            List<ReporteStockConsolidado> content = null;
            Task.Factory.StartNew(() =>
            {
                //var fechaInicial = new DateTime(Globals.ClientSession.i_Periodo ?? DateTime.Now.Year, 1, 1);
                var fechaInicial = dtpFechaRegistroDe.Value.Date;
                var fechaFinal = dtpFechaRegistroAl.Value.Date.AddDays(1);
                var idEstab = int.Parse(cboEstablecimiento.Value.ToString());
                content = new AlmacenBL().GetReporteReposicion(ref objOperationResult, fechaInicial, fechaFinal, cboLinea.Value.ToString(), idEstab, (string)txtCodigoProducto.Tag);
            }, _cts.Token).ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;

                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show("Ocurrió un error al realizar Reporte Stock Consolidado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var dt = Utils.ConvertToDatatable(content);
                if (export)
                    ExportExcel(dt);
                else
                    Report(dt);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void Report(DataTable dt)
        {
            var aptitudeCertificate1 = new NodeBL().ReporteEmpresa();
            using (var ds1 = new DataSet())
            {
                var dt2 = Utils.ConvertToDatatable(aptitudeCertificate1);
                ds1.Tables.Add(dt);
                ds1.Tables.Add(dt2);
                ds1.Tables[0].TableName = "dsReporteStockConsolidado";
                ds1.Tables[1].TableName = "dsEmpresa";
                var rp = new crReporteReposicion();
                rp.SetDataSource(ds1);
                rp.SetParameterValue("Fecha", string.Format("AL {0:dd/MM/yy}", dtpFechaRegistroDe.Value.Date));
                rp.SetParameterValue("Establecimiento", cboEstablecimiento.Text.ToUpper());
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
        }

        private void ExportExcel(DataTable dt)
        {
            #region Headers
            if (dt.Rows.Count == 0)
            {
                UltraMessageBox.Show("No se encontro informacion!", "Export To Excel");
                return;
            }
            var cols = new List<string> { "codigoProducto", "descripcionProducto", "unidad", "cantidadAlmacen2", "cantidad", "cantidadAlmacen1" };
            var heads = new List<ExcelHeader> { "CODIGO", "DESCRIPCION", "U.M." , "STOCK ITALIA", "CANTIDAD", "STOCK ACTUAL"};
            #endregion
            OperationResult objOperationResult = new OperationResult();
            var objexcel = new ExcelReport(dt)
            {
                Headers = heads.ToArray(),
                FormaterHeader = cell =>
                {
                    cell.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(Color.Black);
                    cell.Font.ColorInfo = Color.WhiteSmoke;
                },
                StartSheet = new Point(0, 0)
            };

            objexcel.AutoSizeColumns(0, 20, 50, 15, 20, 15, 15);
            objexcel.CurrentPosition = 0;
            objexcel.SetHeaders();
            objexcel.SetData( ref objOperationResult ,cols.ToArray());
            var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
            objexcel.Generate(path);
            System.Diagnostics.Process.Start(path);
        }
        #endregion

        #region Events
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            LoadReport(btnExcel == sender);
        }
        private void TxtProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            using (var frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null))
            {
                frm.ShowDialog();
                txtCodigoProducto.Tag = txtCodigoProducto.Text = frm._IdProducto != null ? frm._CodigoInternoProducto.Trim() : string.Empty;
            }
        }

        private void frmProductoReposicion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }
        #endregion
    }
}
