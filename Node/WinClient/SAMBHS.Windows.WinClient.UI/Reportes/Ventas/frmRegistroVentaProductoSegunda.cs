#region References
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using SAMBHS.Almacen.BL;
using System.Threading;
using System.IO;
#endregion

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmRegistroVentaProductoSegunda : Form
    {
        #region Declaraciones / Referencias
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private string _strGrupollave, _strNombreGrupollave;
        private readonly int _consideraDocumentosInternos = -1;
        DatahierarchyBL _objDatahierarchyBl = new DatahierarchyBL();
        string _whereAlmacenesConcatenados, _AlmacenesConcatenados;
        #endregion

        #region Carga de inicializacion
        public frmRegistroVentaProductoSegunda(string Modalidad)
        {
            InitializeComponent();
            _consideraDocumentosInternos = Modalidad == Constants.ModuloContable ? 0 : 1;
            BackColor = new GlobalFormColors().FormColor;
        }
        #endregion

        #region Cargar Reporte
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Ventas por Productos de Segunda"
                : @"Reporte de Ventas por Productos de Segunda";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = !estado;
        }
        private void CargarReporte(bool Excel)
        {
            OperationResult objOperationResult = new OperationResult();
            List<string> Retonar = new List<string>();
            List<string> Retonar2 = new List<string>();
            List<ReporteVentaProductoSegunda> aptitudeCertificate = null;
            var rp = new crRegistroVentasProductosSegunda();
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {

                aptitudeCertificate = new VentaBL().ReporteRegistroVentaProductosSegunda(ref objOperationResult, int.Parse(cboEstablecimiento.Value.ToString()), DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), TxtProductoCodigo.Text.Trim(), int.Parse(cboTipoDocumento.Value.ToString()), int.Parse(cboTipoProducto.Value.ToString()), int.Parse(cboEstadoCobranza.Value.ToString()), int.Parse(cboAlmacen.Value.ToString()));

            }, _cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show("Error  Reporte de Ventas por Producto (Analítico)", "Sistema ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var ds1 = new DataSet())
                {
                    using (var dt = Utils.ConvertToDatatable(aptitudeCertificate))

                        if (Excel)
                        {
                            #region Headers
                            if (dt.Rows.Count == 0)
                            {
                                UltraMessageBox.Show("No se encontro informacion!", "Report To Excel");
                                return;
                            }
                            var cols = new List<string> { "CodigoProducto", "Producto", "Cantidad", "MontoVendido", "Acuenta", "Saldo" };
                            var heads = new List<ExcelHeader> { "CÓDIGO PRODUCTO", "DESCRIPCIÓN", "CANTIDAD", "MONTO VENDIDO (S/)", "A CUENTA(S/)", "SALDO(S/)" };
                            #endregion

                            var objexcel = new ExcelReport(dt)
                            {
                                Headers = heads.ToArray(),
                                //FormaterHeader = (cell) =>
                                //{
                                //    cell.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(Color.Black);
                                //    cell.Font.ColorInfo = Color.WhiteSmoke;
                                //},
                                //StartSheet = new Point(0, 0)
                            };

                            objexcel.AutoSizeColumns(1, 20, 50, 10,20,20,20);
                            objexcel.SetTitle("VENTAS Y COBRANZAS DE PRODUCTOS DE SEGUNDA");
                            objexcel[2].Cells[4].Value = "DEL " + dtpFechaRegistroDe.Text + " AL " + dtpFechaRegistroAl.Text;
                            objexcel.CurrentPosition = 0;
                            objexcel.SetHeaders();

                            objexcel.EndSection += excel_EndSection;

                            objexcel.SetData(ref objOperationResult ,cols.ToArray());
                            var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                            objexcel.Generate(path);
                            System.Diagnostics.Process.Start(path);
                        }
                        else
                        {

                            ds1.Tables.Add(dt);
                            ds1.Tables[0].TableName = "dsReporteVentaProductoSegunda";
                            rp.SetDataSource(ds1);




                            rp.SetParameterValue("Fecha", "DEL " + dtpFechaRegistroDe.Text + " AL " + dtpFechaRegistroAl.Text);
                            rp.SetParameterValue("NombreEmpresa", Globals.ClientSession.v_CurrentExecutionNodeName.ToUpper());
                            rp.SetParameterValue("RucEmpresa", "R.U.C.: " + Globals.ClientSession.v_RucEmpresa);
                            rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());

                            crystalReportViewer1.ReportSource = rp;
                            crystalReportViewer1.Show();
                        }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }


        private void excel_EndSection(object sender, ExcelReportSectionEventArgs e)
        {
            //if (int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.Fisico)
            //{
                if (e.StartPosition == e.EndPosition) return;
                var obj = (ExcelReport)sender;
                //obj[e.StartPosition - 1].Cells[1].Value = e.FirsRow["v_NombreProducto"];
                obj.SetFormulas(2, "TOTALES : ", string.Format("=SUM(D{0}:D{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(E{0}:E{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(F{0}:F{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(G{0}:G{1})", e.StartPosition + 1, e.EndPosition));
                obj.CurrentPosition++;
            //}
            //else
            //{

            //    if (e.StartPosition == e.EndPosition) return;
            //    var obj = (ExcelReport)sender;
            //    obj[e.StartPosition - 1].Cells[1].Value = e.FirsRow["v_NombreProducto"];
            //    obj.SetFormulas(5, "TOTAL POR PRODUCTO : ", string.Format("=SUM(G{0}:G{1})", e.StartPosition + 1, e.EndPosition), "", string.Format("=SUM(I{0}:I{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(J{0}:J{1})", e.StartPosition + 1, e.EndPosition), "", string.Format("=SUM(L{0}:L{1})", e.StartPosition + 1, e.EndPosition));
            //    obj.CurrentPosition++;
            //}

        }

        #endregion

        #region Controles Botones
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            try
            {

                if (uvDatos.Validate(true, false).IsValid)
                {
                    var strOrderExpression = cboTipoDocumento.Value.ToString() == "-1" ? "" : cboTipoDocumento.Value.ToString();
                    CargarReporte(false);
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

        private void frmRegistroVentaProductoSegunda_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
        }

        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            var dthierarchy = new DatahierarchyBL();
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboTipoDocumento.Text = @"DOCUMENTO";
            Utils.Windows.LoadUltraComboEditorList(cboTipoProducto, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 6, null), DropDownListAction.Select);
            cboTipoProducto.Value = "10";
            cboTipoDocumento.Value = "-1";
            cboEstadoCobranza.Value = "-1";
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString();
        }

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


        private void TxtProductoCodigo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarArticulo")
            {

                using (var frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null))
                {
                    frm.ShowDialog();
                    TxtProductoCodigo.Text = frm._IdProducto != null ? frm._CodigoInternoProducto.Trim() : string.Empty;
                }
            }
        }



        private void cboEstablecimiento_ValueChanged(object sender, EventArgs e)
        {
            EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
            List<KeyValueDTO> x = new List<KeyValueDTO>();
            _whereAlmacenesConcatenados = string.Empty;
            _AlmacenesConcatenados = string.Empty;
            if (cboEstablecimiento.Value == null) return;
            x = objEstablecimientoBL.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString())).ToList();
            if (x.Count > 0)
            {
                foreach (var item in x)
                {
                    _whereAlmacenesConcatenados = _whereAlmacenesConcatenados + "IdAlmacen==" + item.Id + " || ";
                    _AlmacenesConcatenados = _AlmacenesConcatenados + item.Value1 + ", ";
                }
                _whereAlmacenesConcatenados = _whereAlmacenesConcatenados.Substring(0, _whereAlmacenesConcatenados.Length - 4);
                _AlmacenesConcatenados = _AlmacenesConcatenados.Substring(0, _AlmacenesConcatenados.Length - 2);
                // _whereAlmacenesTemporales = _whereAlmacenesConcatenados;
            }
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.Select);
            cboAlmacen.Value = "-1";
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            CargarReporte(true);
        }

    }
}
