using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Venta.BL;
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Common.Resource;
using System.Threading;
using SAMBHS.CommonWIN.BL;
using System.Threading.Tasks;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Almacen.BL;
using System.IO;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmRegistroVentaVendedorAnalitico : Form
    {
        VendedorBL _objVendedorBL = new VendedorBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        CancellationTokenSource _cts = new CancellationTokenSource();
        VentaBL _objVentaBL = new VentaBL();
        LineaBL _objLineaBL = new LineaBL();
        MarcaBL _objMarcaBL = new MarcaBL();
        public frmRegistroVentaVendedorAnalitico(string IdVenta)
        {

            InitializeComponent();
        }

        private void frmRegistroVentaVendedorAnalitico_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
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
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            cboOrden.SelectedIndex = 4;
            cboVendedor.Value = "-1";
            cboDetalleDocumento.Value = "-1";
            cboAgrupado.Value = "SINAGRUPAR";
            cboLinea.Value = "-1";
            cboMarca.Value = "-1";
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.Value.ToString();
            cboEstablecimiento.Enabled = Globals.ClientSession.UsuarioEsContable == 1 ? true : false;
            cboTipoDocumento.Value = "-1";
        }

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Ventas Por Vendedor Analítico"
                : @"Reporte de Ventas Por Vendedor Analítico";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = !estado;
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            Reporte(false);
        }

        private void Reporte(bool Export)
        {


            if (Validar.Validate(true, false).IsValid)
            {

                List<ReporteVendedorAnalitico> VendedorAnalitco = new List<ReporteVendedorAnalitico>();
                OperationResult objOperationResult = new OperationResult();
                OcultarMostrarBuscar(true);
                Cursor.Current = Cursors.WaitCursor;

                var rp = new Reportes.Ventas.crRegistroVendedorAnalitico();

                Task.Factory.StartNew(() =>
                {


                    VendedorAnalitco = _objVentaBL.ReporteVendedorAnalitico(ref objOperationResult, DateTime.Parse(dtpFechaRegistroDe.Text + " 00:00"), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), int.Parse(cboDetalleDocumento.Value.ToString()), cboVendedor.Value.ToString(), cboAgrupado.Value.ToString(), int.Parse(cboMoneda.Value.ToString()), cboMarca.Value.ToString(), cboLinea.Value.ToString(), txtArtIni.Text.Trim(), cboOrden.Value.ToString(),int.Parse ( cboEstablecimiento.Value.ToString ()),int.Parse (cboTipoDocumento.Value.ToString ()));

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


                    DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(VendedorAnalitco);


                    if (Export)
                    {
                        VendedorAnalitco = VendedorAnalitco.OrderBy(l => l.Grupo1).ThenBy(l => l.Grupo2).ToList();
                        dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(VendedorAnalitco);
                        #region Headers

                        var columnas = new[]
                    {
                        "NroRegistro", "Fecha", "NroDocumento","DocumentoContable" ,"Producto","Cantidad","PrecioUnitario","ValorVenta","PrecioVenta"
                    };


                        var heads = new[]
                    {
                        new ExcelHeader{
                            Title = "", 
                            Headers = new ExcelHeader[]
                            {
                                 "NRO. REGISTRO", "FECHA", "DOCUMENTO","TIPO DOCUMENTO","PRODUCTO","CANTIDAD","PRECIO UNIT.","V. VENTA","P. VENTA"
                            }
                        },
                       

                    };

                        #endregion

                        var excel = new ExcelReport(dt) { Headers = heads };
                        excel.AutoSizeColumns(1, 20, 20, 20,20, 50, 25, 25, 25, 25);
                        excel.SetTitle("VENTAS POR VENDEDOR ANALÌTICO");

                        excel[2].Cells[4].Value = "DEL " + dtpFechaRegistroDe.Text + " AL " + dtpFechaRegistroAl.Text;
                        excel.SetHeaders();
                        var last = new int[0];
                        var group = 0;
                        excel.EndSection += excel_EndSection;


                        excel.EndSectionGroup += (sender, e) =>
                        {
                            if (cboAgrupado.Text != "SIN AGRUPAR")
                            {
                                var obj = (ExcelReport)sender;
                                if (e.EndSections.Length == 0) return;
                                if (group == 0)
                                {

                                    obj.SetFormulas(4, "TOTAL VENDEDOR", "=" + string.Join("+", e.EndSections.Select(i => "F" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "G" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "H" + i).ToArray()), "=" + string.Join("+", e.EndSections.Select(i => "I" + i).ToArray()));
                                    obj.CurrentPosition++;
                                }
                                last = e.EndSections;
                                group++;
                            }
                        };
                        var filtros = new[] { "Grupo1", "Grupo2" };
                        excel.SetData( ref objOperationResult ,columnas, filtros);
                        // InsertTable(excel, last);
                        var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                        excel.Generate(path);
                        System.Diagnostics.Process.Start(path);


                    }
                    else
                    {


                        // DataTable dt2 = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate2);

                        //ds1.Tables.Add(dt2);
                        ds1.Tables.Add(dt);

                        // ds1.Tables[0].TableName = "dsEmpresa";
                        ds1.Tables[0].TableName = "dsRegistroVendedorAnalitico";
                        rp.SetDataSource(ds1);
                        rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                        rp.SetParameterValue("RucEmpresa", "R.U.C. : " + aptitudeCertificate2.FirstOrDefault().RucEmpresaPropietaria.Trim());

                        rp.SetParameterValue("NroRegistros", VendedorAnalitco.Count());
                        rp.SetParameterValue("FechaImpresion", "DEL " + dtpFechaRegistroDe.Text + " AL " + dtpFechaRegistroAl.Text);
                        rp.SetParameterValue("Agrupamiento", cboAgrupado.Value.ToString());
                        rp.SetParameterValue("MonedaReporte", "MONEDA REPORTE : " + cboMoneda.Text);
                        crystalReportViewer1.ReportSource = rp;
                        crystalReportViewer1.Show();
                    }
                }
                    , TaskScheduler.FromCurrentSynchronizationContext());

            }
        }


        private void excel_EndSection(object sender, ExcelReportSectionEventArgs e)
        {
            if (e.StartPosition == e.EndPosition) return;
            var obj = (ExcelReport)sender;

            //obj[e.StartPosition - 1].Cells[1].Value = e.FirsRow["v_NombreProducto"];
            obj.SetFormulas(4, cboAgrupado.Text == "SIN AGRUPAR" ? "TOTAL VENDEDOR : " : cboAgrupado.Text == "PRODUCTO" ? "TOTAL POR PRODUCTO" : cboAgrupado.Text == "CLIENTE" ? "TOTAL POR CLIENTE" : "TOTAL POR DOCUMENTO", string.Format("=SUM(F{0}:F{1})", e.StartPosition + 1, e.EndPosition), "", string.Format("=SUM(H{0}:H{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(I{0}:I{1})", e.StartPosition + 1, e.EndPosition));
            obj.CurrentPosition++;



        }


        private void cboLinea_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (cboLinea.Value == null || cboLinea.Value.ToString() == "-1") return;

            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", cboLinea.Value.ToString()), DropDownListAction.Select);
            cboMarca.Value = "-1";
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
