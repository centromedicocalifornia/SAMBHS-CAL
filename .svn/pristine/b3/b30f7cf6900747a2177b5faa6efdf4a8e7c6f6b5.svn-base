#region Name Space
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
using CrystalDecisions.CrystalReports.Engine;
using System.Threading;
using System.IO;
using Infragistics.Documents.Excel;

#endregion

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    // ReSharper disable once InconsistentNaming
    public partial class frmRegistroVentasDetalle : Form
    {
        #region Fields
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private string _whereAlmacenesConcatenados = string.Empty;
        private string _whereAlmacenesTemporales = string.Empty;
        #endregion

        #region Init & Load
        // ReSharper disable once UnusedParameter.Local
        public frmRegistroVentasDetalle(string idVenta)
        {
            InitializeComponent();
        }
        private void frmRegistroVentaProductoAnalitico_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
        }
        #endregion

        #region Methods
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            var objEstablecimientoBl = new EstablecimientoBL();
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", objEstablecimientoBl.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", objEstablecimientoBl.GetAlmacenesXEstablecimiento(-1), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", new VendedorBL().ObtenerListadoVendedorParaCombo(ref objOperationResult, null), DropDownListAction.All);
            cboOrden.Value = "DocumentoCanjeado";
            cboEstablecimiento.Value = (Globals.ClientSession.i_IdAlmacenPredeterminado ?? -1).ToString();
            cboAgrupar.Value = "1";
            cboTipo.Value = "1";
            cboVendedor.Value = "-1";       
        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = (estado ? @"Generando... " : "") + @"Reporte de Ventas Detalle";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = btnExcel.Enabled = !estado;
        }
        private void CargarReporte(DateTime fechaRegistroIni, DateTime fechaRegistroFin, string fechaHoraImpresion, string filtro, string orden, bool export)
        {
            OperationResult objOperationResult = new OperationResult();
            List<ReporteVentasDetalles> aptitudeCertificate = new List<ReporteVentasDetalles>();
            var rp = (string)cboAgrupar.Value == "1" ? Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya ? (ReportDocument)new crReporteVentasyDetallesNotaria() : (ReportDocument)new crReporteVentasyDetalles() : Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya ? (ReportDocument) new crReporteVentasyDetallesAgrupadoNotaria() :(ReportDocument) new crReporteVentasyDetallesAgrupado();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {
                //aptitudeCertificate = new VentaBL().ReporteRegistroVentaProductoAnalitico(ref objOperationResult, 0, FechaRegistroIni, FechaRegistroFin, IdMoneda, IdCliente, IdProducto, Orden, strGrupollave, strNombreGrupollave, cboTipo.Value.ToString(), cboMarca.Value.ToString());
                aptitudeCertificate = new VentaBL().ReporteVentasDetalles(ref objOperationResult, fechaRegistroIni, fechaRegistroFin, txtCodigoProducto.Text.Trim(), txtCodigoCliente.Text.Trim(), filtro, orden, (string)cboAgrupar.Value == "2", cboVendedor.Value.ToString());
            }, _cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show("Error Reporte de Ventas Detalle" + Environment.NewLine + objOperationResult.ExceptionMessage, "Sistema ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var dt = Utils.ConvertToDatatable(aptitudeCertificate);

                if (export)
                {
                    #region Headers
                    
                    
                    var columnas = Globals.ClientSession.v_RucEmpresa ==Constants.RucNotariaBecerrSosaya ?  new[]
                    {
                        "TipoDocumento", "NumeroDocumento", "Vendedor", "sFecha", "Documento","NroDocCliente", "Cliente", "Cantidad", "UnidadMedida",
                        "Producto", "MonedaOperacion", "PrecioUnitario", "Total",
                        
                    } :  new[]
                    {
                        "DocumentoCanjeado", "Vendedor", "sFecha", "Documento","NroDocCliente", "Cliente", "Cantidad", "UnidadMedida",
                        "Producto", "MonedaOperacion", "PrecioUnitario", "Total",
                        "CantidadPorEntregar", "UnidadMedidaPorEntregar", "Estado"
                    } ;
                    var heads =Globals.ClientSession.v_RucEmpresa ==Constants.RucNotariaBecerrSosaya ? new ExcelHeader[]
                    {
                           new ExcelHeader
                        {
                            Title = "DOCUMENTO", Headers = new ExcelHeader[]{"TIPO","NÚMERO"}
                        }
                    , "VENDEDOR", "FECHA", "DOCUMENTO","NRO. DOC CLIENTE", "CLIENTE", "CANTIDAD", "U. MEDIDA", "PRODUCTO", "MONEDA" , "PRECIO", "TOTALES"
                    } : new ExcelHeader[]
                    {
                        "DOCUMENTO", "VENDEDOR.", "FECHA", "DOCUMENTO","NRO. DOC CLIENTE", "CLIENTE", "CANTIDAD", "U. MEDIDA", "PRODUCTO", "MONEDA" , "PRECIO", "TOTALES", "POR ENTREGAR","UM", "ESTADO"
                    };


                    #endregion

                    var excel = new ExcelReport(dt) {Headers = heads};
                    if (Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya)
                    {
                        excel.AutoSizeColumns(1,10, 20, 30, 15, 30, 25, 40, 15, 20, 100, 15, 20, 20);
                    }
                    else
                    {
                        excel.AutoSizeColumns(1, 20, 30, 15, 30, 25, 40, 15, 20, 30, 15, 20, 20, 20, 15, 20);
                    }
                    excel.SetTitle("REGISTRO DE VENTAS Y DETALLES");
                    excel.SetHeaders();
                    excel.EndSection += (_, e) =>
                    {
                        if (e.StartPosition == e.EndPosition) return;
                        var obj = (ExcelReport)_;

                        if (Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya)
                        {
                            obj.SetFormulas(6, "SUB TOTAL:", string.Format("=SUM($H{0}:$H{1})", e.StartPosition + 1, e.EndPosition), null, null,null ,
                                string.Format("=SUM($L{0}:$L{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM($M{0}:$M{1})", e.StartPosition + 1, e.EndPosition));
                            obj.CurrentPosition++;
                        }
                        else
                        {
                            obj.SetFormulas(6, "SUB TOTAL:", string.Format("=SUM($H{0}:$H{1})", e.StartPosition + 1, e.EndPosition), null, null,
                                    string.Format("=SUM($L{0}:$L{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM($M{0}:$M{1})", e.StartPosition + 1, e.EndPosition));
                            obj.CurrentPosition++;
                        }
                    };
                    if ((string)cboAgrupar.Value == "1")
                        excel.SetData( ref objOperationResult ,columnas);
                    else
                        excel.SetData( ref objOperationResult ,columnas, "Grupo");
                    var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                    excel.Generate(path);
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                    var ds1 = new DataSet();
                    dt.TableName = "dsVentayDetalles";
                    ds1.Tables.Add(dt);
                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("FechaHoraImpresion", fechaHoraImpresion);
                    rp.SetParameterValue("CantidadDecimal", Globals.ClientSession.i_CantidadDecimales ?? 2);
                    rp.SetParameterValue("CantidadDecimalPrecio", Globals.ClientSession.i_PrecioDecimales ?? 2);
                    rp.SetParameterValue("Fecha", "DEL " + fechaRegistroIni.Date.ToString("dd/MM/yyyy") + " AL " + fechaRegistroFin.Date.ToString("dd/MM/yyyy"));
                    rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.First().NombreEmpresaPropietaria.Trim().ToUpper());
                    rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate2.First().RucEmpresaPropietaria.ToUpper());
                    rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count);
                    crystalReportViewer1.ReportSource = rp;
                    crystalReportViewer1.Show();                    
                }

            }
                , TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion

        #region Methods UI
        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded)
            {
                groupBox1.Location = new Point(groupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox1.Height = Height - ultraExpandableGroupBox1.Height - 5;
            }
            else
            {
                groupBox1.Location = new Point(groupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox1.Height = Height - ultraExpandableGroupBox1.Height - 5;
            }
        }

        private void frmRegistroVentaProductoAnalitico_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }
        private void cboEstablecimiento_ValueChanged(object sender, EventArgs e)
        {
            _whereAlmacenesConcatenados = string.Empty;
            var almacenesConcatenados = string.Empty;
            if (cboEstablecimiento.Value == null) return;
            var x = new EstablecimientoBL().GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));

            if (x.Count > 0)
            {
                foreach (var item in x)
                {
                    _whereAlmacenesConcatenados = _whereAlmacenesConcatenados + "IdAlmacen==" + item.Id + " || ";
                    almacenesConcatenados = almacenesConcatenados + item.Value1 + ", ";
                }
                _whereAlmacenesConcatenados = _whereAlmacenesConcatenados.Substring(0, _whereAlmacenesConcatenados.Length - 4);
                //almacenesConcatenados = almacenesConcatenados.Substring(0, almacenesConcatenados.Length - 2);
                _whereAlmacenesTemporales = _whereAlmacenesConcatenados;
            }
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.All);
            cboAlmacen.Value = "-1";
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvDatos.Validate(true, false).IsValid)
                {
                    if (cboAgrupar.Value != null && cboAgrupar.Value.ToString() == "-1")
                    {
                        UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        cboAgrupar.Focus();
                        return;
                    }
                  
                    var filters = new Queue<string>();
                    if (cboAlmacen.Value.ToString() != "-1")
                    {
                        filters.Enqueue("IdAlmacen==" + cboAlmacen.Value);
                    }
                    else
                    {
                        filters.Enqueue("(" + (_whereAlmacenesConcatenados ?? _whereAlmacenesTemporales) + ")");
                    }
                    var strFilterExpression = filters.Count > 0 ? string.Join(" && ", filters) : null;

                    var strOrderExpression = cboOrden.Value.ToString() == "-1" ? "" : cboOrden.Value.ToString();
                    CargarReporte(DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), chkHoraimpresion.Checked ? "1" : "0", strFilterExpression, strOrderExpression, sender == btnExcel);
                }

                else
                    UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", Icono:MessageBoxIcon.Exclamation);
            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", Icono: MessageBoxIcon.Error);
            }
        }

        private void txtCodigoProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key != "btnBuscarArticulo") return;
            var frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
            frm.ShowDialog();
            txtCodigoProducto.Text = frm._IdProducto != null ? frm._CodigoInternoProducto.Trim() : string.Empty;
        }

        private void txtCodigoCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key != "btnBuscarCliente") return;
            var frm = new Mantenimientos.frmBuscarCliente("V", txtCodigoCliente.Text.Trim());
            frm.ShowDialog();

            if (frm._IdCliente == null) return;
            txtCodigoCliente.Text = frm._CodigoCliente.Trim();
            txtCodigoCliente.Text = frm._CodigoCliente;
        }
        #endregion
    }
}
