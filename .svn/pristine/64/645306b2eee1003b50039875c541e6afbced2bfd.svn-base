using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Venta.BL;
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
    public partial class frmRegistroVentaVendedorResumen : Form
    {

        VendedorBL _objVendedorBL = new VendedorBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        VentaBL _objVentaBL = new VentaBL();
        LineaBL _objLineaBL = new LineaBL();
        MarcaBL _objMarcaBL = new MarcaBL();
        public frmRegistroVentaVendedorResumen(string IdVenta)
        {
            InitializeComponent();
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            Reporte(false);

        }

        private void Reporte(bool Excel)
        {


            if (Validar.Validate(true, false).IsValid)
            {
                string FechaMostrar = "DEL " + dtpFechaRegistroDe.Text + " AL " + dtpFechaRegistroAl.Text;
                List<ReporteVendedorResumen> VendedorResumen = new List<ReporteVendedorResumen>();
                OperationResult objOperationResult = new OperationResult();
                OcultarMostrarBuscar(true);
                Cursor.Current = Cursors.WaitCursor;

                var rp = new Reportes.Ventas.crRegistroVendedorResumen();

                Task.Factory.StartNew(() =>
                {


                    VendedorResumen = _objVentaBL.ReporteVendedorResumen(ref objOperationResult, DateTime.Parse(dtpFechaRegistroDe.Text + " 00:00"), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), cboAgrupado.Value.ToString(), int.Parse(cboMoneda.Value.ToString()), int.Parse(cboDetalleDocumento.Value.ToString()), cboLinea.Value.ToString(), cboMarca.Value.ToString(), cboVendedor.Value.ToString(), txtArtIni.Text.Trim(), int.Parse(cboEstablecimiento.Value.ToString()),cboOrden.Value.ToString (), int.Parse (cboTipoDocumento.Value.ToString ()));

                }, _cts.Token)
                .ContinueWith(t =>
                {


                    if (_cts.IsCancellationRequested) return;
                    OcultarMostrarBuscar(false);
                    Cursor.Current = Cursors.Default;
                    if (objOperationResult.Success == 0)
                    {
                        if (!string.IsNullOrEmpty(objOperationResult.ExceptionMessage))
                        {
                            UltraMessageBox.Show(objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error " + " Reporte de Ventas Por Vendedor Analítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error" + " Reporte de Ventas Por Vendedor Analítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return;
                    }

                    var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                    DataSet ds1 = new DataSet();
                    //ExcelHeader[] heads= new ExcelHeader [10];
                    //string [] columnas= new string [10];
                    DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(VendedorResumen);

                    if (Excel)
                    {

                        #region ExcelSinAgrupar
                        if (cboAgrupado.Value.ToString() == "SINAGRUPAR" )
                        {
                            var columnas = new[]
                            {
                           
                               "Grupo2", "Cantidad","ValorVenta", "PrecioVenta",
                            };
                            var heads = new[]
                            {
                                new ExcelHeader{
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                         "VENDEDOR", "CANTIDAD" ,"VALOR VENTA", "PRECIO VENTA",
                                    }
                                },
                                
                            };


                            var excel = new ExcelReport(dt) { Headers = heads };
                            excel.AutoSizeColumns(1, 20, 25, 25, 25, 50, 25, 25, 25);
                            excel.SetTitle("VENTAS POR VENDEDOR RESUMEN");

                            excel[2].Cells[2].Value = FechaMostrar;
                            excel.SetHeaders();
                            //excel.EndSection += excel_EndSection;
                            //excel.EndSectionGroup += excel_EndSectionGroup;
                            //var filtros = new[] { "Almacen", "v_NombreProducto" };


                            excel.SetData(ref objOperationResult ,columnas);
                            var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                            excel.Generate(path);
                            System.Diagnostics.Process.Start(path);


                        #endregion
                        }
                        else if (cboAgrupado.Value.ToString() == "PRODUCTO" || cboAgrupado.Value.ToString() == "CLIENTE")
                        {
                            #region ExcelProductoCliente
                            var columnas = new[]
                            {
                           
                               "Grupo2","Cantidad", "ValorVenta", "PrecioVenta",
                            };
                            var heads = new[]
                            {
                                new ExcelHeader{
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                         "VENDEDOR","CANTIDAD", "VALOR VENTA", "PRECIO VENTA",
                                    }
                                },
                                
                            };


                            var excel = new ExcelReport(dt) { Headers = heads };
                            excel.AutoSizeColumns(1, 50,25,25);
                            excel.SetTitle("VENTAS POR VENDEDOR RESUMEN");

                            excel[2].Cells[2].Value = FechaMostrar;
                            excel.SetHeaders();
                            //excel.EndSection += excel_EndSection;
                           // excel.EndSectionGroup += excel_EndSectionGroup;
                            var filtros = new[] { "Grupo1"};
                            excel.SetData(ref objOperationResult ,columnas, filtros);

                            var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                            excel.Generate(path);
                            System.Diagnostics.Process.Start(path);

                            #endregion
                        }

                        else if (cboAgrupado.Value.ToString() == "DOCUMENTO")
                        {
                            #region ExcelDocumento
                            var columnas = new[]
                            {
                           
                               "Grupo2","Fecha","NombreCliente","CondicioPago","Cantidad", "ValorVenta", "PrecioVenta",
                            };
                            var heads = new[]
                            {
                                new ExcelHeader{
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                         "VENDEDOR", "FECHA", "CLIENTE","CONDICIÓN DE PAGO","CANTIDAD", "VALOR DE VENTA","PRECIO DE VENTA"
                                    }
                                },
                                
                            };


                            var excel = new ExcelReport(dt) { Headers = heads };
                            excel.AutoSizeColumns(1, 50, 25, 50,25,25,25);
                            excel.SetTitle("VENTAS POR VENDEDOR RESUMEN");

                            excel[2].Cells[2].Value = FechaMostrar;
                            excel.SetHeaders();
                            //excel.EndSection += excel_EndSection;
                            excel.EndSectionGroup += excel_EndSectionGroup;

                            var filtros = new[] { "Grupo1" };
                            excel.SetData( ref objOperationResult ,columnas, filtros);

                            var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                            excel.Generate(path);
                            System.Diagnostics.Process.Start(path);

                            #endregion
                        
                        }
                    }


                    else
                    {


                        ds1.Tables.Add(dt);
                        ds1.Tables[0].TableName = "dsRegistroVendedorResumen";
                        rp.SetDataSource(ds1);
                        rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                        rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());

                        rp.SetParameterValue("NroRegistros", VendedorResumen.Count());
                        rp.SetParameterValue("FechaImpresion", FechaMostrar);
                        rp.SetParameterValue("Agrupamiento", cboAgrupado.Value.ToString());
                        rp.SetParameterValue("MonedaReporte", "MONEDA REPORTE : " + cboMoneda.Text);
                        crystalReportViewer1.ReportSource = rp;
                        crystalReportViewer1.Show();
                    }
                }
                    , TaskScheduler.FromCurrentSynchronizationContext());
            }

        }

        private void excel_EndSectionGroup(object sender, ExcelReportSectionGroupEventArgs e)
        {
            if (e.EndSections.Length == 0) return;
            var obj = (ExcelReport)sender;
            var totalLabel = "TOTAL POR VENDEDOR :";
            if (cboAgrupado.Value.ToString() == "PRODUCTO" || cboAgrupado.Value.ToString() == "CLIENTE")
            {
                obj.SetFormulas(1, totalLabel,
                    Enumerable.Range(2, 2)
                    .Select(i => string.Format("=" + string.Join("+", e.EndSections.Select(n => "${0}" + n)), (char)('C' + i)))
                    .ToArray());
            }
            else
            {
                //obj.SetFormulas(5, totalLabel, "=" + string.Join("+", e.EndSections.Select(n => "$G" + n)));
                //obj.SetFormulas(8, totalLabel, "=" + string.Join("+", e.EndSections.Select(n => "$J" + n)));
            }
        }

        private void excel_EndSection(object sender, ExcelReportSectionEventArgs e)
        {
            //if (int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.Fisico)
            //{
            //    if (e.StartPosition == e.EndPosition) return;
            //    var obj = (ExcelReport)sender;
            //    obj[e.StartPosition - 1].Cells[1].Value = e.FirsRow["v_NombreProducto"];
            //    obj.SetFormulas(5, "TOTAL POR PRODUCTO : ", string.Format("=SUM(G{0}:G{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(H{0}:H{1})", e.StartPosition + 1, e.EndPosition), string.Format("=I{0}", e.EndPosition));
            //    obj.CurrentPosition++;


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
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", _objVendedorBL.ObtenerListadoVendedorParaCombo(ref objOperationResult, null), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboDetalleDocumento, "Value1", "Id", _objDocumentoBL.ObtenDocumentosParaCombo(ref objOperationResult, null, 0, 1), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            //Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 67, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", _objLineaBL.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.Value.ToString();
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            cboOrden.SelectedIndex = 1;
            cboVendedor.Value = "-1";
            cboDetalleDocumento.Value = "-1";
            cboAgrupado.Value = "DOCUMENTO";
            cboLinea.Value = "-1";
            cboMarca.Value = "-1";
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.Value.ToString();
            cboEstablecimiento.Enabled = Globals.ClientSession.UsuarioEsContable == 1 ? true : false;
            cboTipoDocumento.Value = "-1";

        }

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Ventas Por Vendedor Resumen"
                : @"Reporte de Ventas Por Vendedor Resumen";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = !estado;
        }

        private void frmRegistroVentaVendedorResumen_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
        }

        private void txtArtIni_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarArticulo")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
                frm.ShowDialog();

                if (frm._IdProducto != null)
                {

                    txtArtIni.Text = frm._CodigoInternoProducto.Trim();
                }
                else
                {
                    txtArtIni.Text = string.Empty;
                }
            }
        }

        private void frmRegistroVentaVendedorResumen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded == true)
            {
                groupBox1.Location = new Point(groupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox1.Height = this.Height - groupBox1.Location.Y - 7;
            }
            else
            {
                groupBox1.Location = new Point(groupBox1.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox1.Height = this.Height - groupBox1.Location.Y - 7;
            }
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            Reporte(true);
        }


    }
}
