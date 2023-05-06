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
using System.Threading;
using System.IO;
namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmStock : Form
    {

        string _whereAlmacenesConcatenados;
        string _AlmacenesConcatenados;
        public int ConsiderarDocumentosInternos = -1;
        List<KardexList> aptitudeCertificate = new List<KardexList>();
        CancellationTokenSource _cts = new CancellationTokenSource();
        MarcaBL _objMarcaBL = new MarcaBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        ReportDocument rp = new ReportDocument();
        public frmStock(string Modo)
        {
            InitializeComponent();
            ConsiderarDocumentosInternos = Modo == "C" ? 0 : 1;
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            //DestruirCrystal(rp);
            VisualizarReporte(false);

        }

        private void DestruirCrystal(ReportDocument doc1)
        {

            if (doc1 != null)
            {
                doc1.Close();
                doc1.Dispose();
                crystalReportViewer1.ReportSource = null;
                crystalReportViewer1.Dispose();
                crystalReportViewer1 = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void VisualizarReporte(bool ExcelExport)
        {
            try
            {
                string CodigoProducto = string.Empty, nroPedido = string.Empty;
                if (uvValidar.Validate(true, false).IsValid)
                {
                    DateTime fecha1 = Convert.ToDateTime("01/01/" + Globals.ClientSession.i_Periodo.ToString() + " 00:00");
                    DateTime fecha2 = Convert.ToDateTime(dtpFechaHasta.Text + " 23:59");
                    int IdMoneda;
                    int TipoKardexReporte;
                    string Moneda, _strFilterExpression = string.Empty;
                    List<string> Filters = new List<string>();
                    if (txtCodArticulo.Text.Trim() != string.Empty) CodigoProducto = txtCodArticulo.Text.Trim();
                    if (txtNroPedido.Text.Trim() != string.Empty) nroPedido = txtNroPedido.Text.Trim();
                    if (cboMarca.Value.ToString() != "-1") Filters.Add("v_IdMarca==" + "\"" + cboMarca.Value.ToString() + "\"");
                    if (cboAlmacen.Value.ToString() != "-1")
                    {
                        Filters.Add("IdAlmacen==" + cboAlmacen.Value.ToString());
                    }
                    else
                    {
                        Filters.Add("(" + _whereAlmacenesConcatenados + ")");
                    }
                    if (int.Parse(cboMoneda.Value.ToString()) == (int)Currency.Soles)
                    {
                        IdMoneda = 1;
                        Moneda = "SOLES";
                    }
                    else
                    {
                        IdMoneda = 2;
                        Moneda = "DOLARES";
                    }
                    TipoKardexReporte = int.Parse ( cboModalidad.Value.ToString()); 
                    if (Filters.Count > 0)
                    {
                        foreach (string item in Filters)
                        {
                            _strFilterExpression = _strFilterExpression + item + " && ";
                        }
                        _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
                    }
                    var Empresa = new NodeBL().ReporteEmpresa();
                    int SoloconstockNegativo = chkMostrarconstockNegativo.Checked == true ? 1 : 0;
                    int SoloStockMayor = chkMostrarconstock.Checked == true ? 1 : 0;
                    {

                        CargarReporte(fecha1, fecha2, _strFilterExpression, IdMoneda, Empresa[0].NombreEmpresaPropietaria, Empresa[0].RucEmpresaPropietaria, Moneda, cboAlmacen.Value.ToString() == "-1" ? _AlmacenesConcatenados : cboAlmacen.Text.ToString(), dtpFechaHasta.Value.ToString("dd-MMMMM"), dtpFechaHasta.Value.ToString("MMMM"), TipoKardexReporte, CodigoProducto, nroPedido, cboLinea.Value.ToString(), ConsiderarDocumentosInternos, int.Parse(cboFormato.Value.ToString()), ExcelExport);
                    }
                }
            }
            catch (Exception f)
            {

                UltraMessageBox.Show("Ocurrió un error al realizar Reporte Stock Consolidado" + f.InnerException, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }



        }

        private void LimpiarFiltros()
        {
            cboEstablecimiento.Value = "-1";
        }
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte Stock Físico / Valorizado"
                : @"Reporte Stock Físico / Valorizado";
            pBuscando.Visible = estado;

            BtnImprimir.Enabled = !estado;
        }
        private void CargarReporte(DateTime? pdtFechaInicio, DateTime? pdtFechaFin, string pstrFilterExpression, int pintIdMoneda, string pstrEmpresa, string pstrRUC, string pstrMoneda, string pstrAlmacen, string pstrMesIni, string pstrMesFin, int pintTipoKardex, string pstrCodigoProducto, string NroPedido, string Linea, int ConsideraDocInternos, int FormatoCantidad, bool ExcelExport)
        {
            OperationResult objOperationResult = new OperationResult();


            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {
                aptitudeCertificate = new AlmacenBL().ReporteStock(ref objOperationResult, int.Parse(cboEstablecimiento.Value.ToString()), pdtFechaInicio, pdtFechaFin, pstrFilterExpression, pintIdMoneda, pstrCodigoProducto, NroPedido, Linea, chkMostrarconstock0.Checked ? 1 : 0, chkMostrarconstock.Checked ? 1 : 0, chkMostrarconstockNegativo.Checked ? 1 : 0, chkMostrarSoloStockMinimo.Checked ? 1 : 0, ConsideraDocInternos, FormatoCantidad, (cboModelo.Value ==null ||  cboModelo.Value.ToString() == "-1")? "" : cboModelo.Text.Trim(),DateTime.Now, chkIncluirNroPedido.Checked ?1 :0 ,null,false,-1,false,false,false,cboAgrupar.Value.ToString ());

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
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Stock de Stock.\n Información Adicional :" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Stock.\n Información Adicional :" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
                var Empresa = new NodeBL().ReporteEmpresa();
                if (pintTipoKardex == 1 || pintTipoKardex ==3)
                {

                    if (pintTipoKardex == 1)
                    {
                        rp = new Reportes.Almacen.crStockFisico();
                    }
                    else
                    {
                        rp = new Reportes.Almacen.crStockFisicoAuxiliar();
                    }

                    DataSet ds1 = new DataSet();
                    DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);

                    if (ExcelExport)
                    {


                        aptitudeCertificate = aptitudeCertificate.OrderBy(l => l.CodProducto).ToList();
                        dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                        #region Headers

                        var columnas = new[]
                    {
                        "CodProducto", "v_NombreProducto", "Marca","Modelo","Ubicacion" ,"NroParte","NroPedido","Saldo_CantidadExcel", "UnidadMedida"
                    };


                        var heads = new[]
                    {
                        new ExcelHeader{
                            Title = "", 
                            Headers = new ExcelHeader[]
                            {
                                 "CÓDIGO", "DESCRIPCIÓN","MARCA" ,"MODELO","UBICACIÓN","NRO. PARTE","PEDIDO", "CANTIDAD", "UNIDAD"
                            }
                        },
                       

                    };

                        #endregion

                        var excel = new ExcelReport(dt) { Headers = heads };
                        excel.AutoSizeColumns(1, 20, 50, 25, 25,25,25,25,25, 15);
                        excel.SetTitle("STOCK FÍSICO");

                        excel[2].Cells[2].Value = "AL  " + dtpFechaHasta.Value.Day.ToString("00") + "/" + dtpFechaHasta.Value.Month.ToString("00") + "/" + dtpFechaHasta.Value.Year.ToString();

                        excel.SetHeaders();
                        //var last = new int[0];
                        //  var group = 0;
                        excel.EndSection += excel_EndSection;
                        // var filtros = new[] { "Almacen", "v_NombreProducto" };
                        excel.SetData( ref objOperationResult ,columnas);
                        // InsertTable(excel, last);
                        var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                        excel.Generate(path);
                        System.Diagnostics.Process.Start(path);

                    }
                    else
                    {


                        dt.TableName = "dtKardex";
                        ds1.Tables.Add(dt);
                        rp.SetDataSource(ds1);
                        rp.SetParameterValue("Fecha", "AL  " + dtpFechaHasta.Value.Day.ToString("00") + "/" + dtpFechaHasta.Value.Month.ToString("00") + "/" + dtpFechaHasta.Value.Year.ToString());
                        rp.SetParameterValue("Establecimiento", "ESTABLECIMIENTO : " + cboEstablecimiento.Text.Trim().ToUpper());
                        rp.SetParameterValue("DecimalesCantidad", Globals.ClientSession.i_CantidadDecimales);
                        rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                        rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                        rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
                        rp.SetParameterValue("AlmacenElegido", int.Parse(cboAlmacen.Value.ToString()));
                        rp.SetParameterValue("MonedaReporte", "MONEDA : " + cboMoneda.Text);
                        rp.SetParameterValue("StockMinimo", chkMostrarSoloStockMinimo.Checked);
                        crystalReportViewer1.ReportSource = rp;
                        crystalReportViewer1.Show();
                        crystalReportViewer1.Zoom(110);

                    }

                }
                else if (pintTipoKardex == 2 || pintTipoKardex ==4)
                {
                    if (pintTipoKardex == 2)
                    {
                        rp = new Reportes.Almacen.crStockValorizado();
                    }
                    else
                    {
                        rp = new Reportes.Almacen.crStockValorizadoAuxiliar();
                    }
                    DataSet ds1 = new DataSet();
                    DataTable dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                    if (ExcelExport)
                    {
                        aptitudeCertificate = aptitudeCertificate.OrderBy(l => l.CodProducto).ToList();
                        dt = SAMBHS.Common.Resource.Utils.ConvertToDatatable(aptitudeCertificate);
                        #region Headers

                        var columnas = new[]
                    {
                         "CodProducto", "v_NombreProducto", "Marca","Modelo","Ubicacion" ,"NroParte", "NroPedido", "Saldo_CantidadExcel", "UnidadMedida","Saldo_PrecioExcel","Saldo_TotalExcel"
                    };


                        var heads = new[]
                    {
                        new ExcelHeader{
                            Title = "", 
                            Headers = new ExcelHeader[]
                            {
                                 "CÓDIGO PRODUCTO", "DESCRIPCIÓN","MARCA" ,"MODELO","UBICACIÓN","NRO. PARTE", "NRO. PEDIDO", "CANTIDAD", "UNIDAD MEDIDA"
                            }
                        },
                      
                      new ExcelHeader{
                            Title =int.Parse ( cboMoneda.Value.ToString ())==(int)Currency.Soles ? "SOLES":"DÓLARES", 
                            Headers = new ExcelHeader[]
                            {
                                 "P. UNITARIO", "VALOR"
                            }
                        },

                        
                    };

                        #endregion


                        var excel = new ExcelReport(dt) { Headers = heads };
                        excel.AutoSizeColumns(1, 20, 50, 25, 25, 25, 25, 25,25,25,25,25);
                        excel.SetTitle("KARDEX VALORIZADO");

                        excel[2].Cells[4].Value = "AL  " + dtpFechaHasta.Value.Day.ToString("00") + "/" + dtpFechaHasta.Value.Month.ToString("00") + "/" + dtpFechaHasta.Value.Year.ToString();
                        excel.SetHeaders();
                        var last = new int[0];
                        var group = 0;
                        excel.EndSection += excel_EndSection;
                        //var filtros = new[] { "Almacen", "v_NombreProducto" };
                        excel.SetData( ref objOperationResult ,columnas);
                        // InsertTable(excel, last);
                        var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                        excel.Generate(path);
                        System.Diagnostics.Process.Start(path);
                    }
                    else
                    {
                        dt.TableName = "dtKardex";
                        ds1.Tables.Add(dt);
                        rp.SetDataSource(ds1);
                        rp.SetParameterValue("Fecha", "AL  " + dtpFechaHasta.Value.Day.ToString("00") + "/" + dtpFechaHasta.Value.Month.ToString("00") + "/" + dtpFechaHasta.Value.Year.ToString());  // TextBox con el valor del Parametro);
                        rp.SetParameterValue("Establecimiento", "ESTABLECIMIENTO : " + cboEstablecimiento.Text.Trim().ToUpper());
                        rp.SetParameterValue("DecimalesCantidad", Globals.ClientSession.i_CantidadDecimales);
                        rp.SetParameterValue("DecimalesPrecio", Globals.ClientSession.i_PrecioDecimales);
                        rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                        rp.SetParameterValue("NombreEmpresa", Empresa.FirstOrDefault().NombreEmpresaPropietaria.Trim());
                        rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim());
                        rp.SetParameterValue("Moneda", pstrMoneda);
                        rp.SetParameterValue("AlmacenElegido", int.Parse(cboAlmacen.Value.ToString()));
                        rp.SetParameterValue("MonedaReporte", "MONEDA : " + cboMoneda.Text);
                        rp.SetParameterValue("StockMinimo", chkMostrarSoloStockMinimo.Checked);
                        crystalReportViewer1.ReportSource = rp;
                        crystalReportViewer1.Show();
                        crystalReportViewer1.Zoom(110);
                    }
                }


            }
                 , TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void excel_EndSection(object sender, ExcelReportSectionEventArgs e)
        {
            if (int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.Fisico)
            {
                if (e.StartPosition == e.EndPosition) return;
                var obj = (ExcelReport)sender;
                //obj[e.StartPosition - 1].Cells[1].Value = e.FirsRow["v_NombreProducto"];
                obj.SetFormulas(3, "TOTAL : ", string.Format("=SUM(E{0}:E{1})", e.StartPosition + 1, e.EndPosition));
                obj.CurrentPosition++;
            }
            else
            {

                if (e.StartPosition == e.EndPosition) return;
                var obj = (ExcelReport)sender;
                // obj[e.StartPosition - 1].Cells[1].Value = e.FirsRow["v_NombreProducto"];
                obj.SetFormulas(3, "TOTAL : ", string.Format("=SUM(E{0}:E{1})", e.StartPosition + 1, e.EndPosition), "", "", string.Format("=SUM(H{0}:H{1})", e.StartPosition + 1, e.EndPosition));
                obj.CurrentPosition++;


            }
        }


        private void frmStock_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
            LineaBL _objLineaBL = new LineaBL();
            NodeWarehouseBL objNodeWarehouseBL = new NodeWarehouseBL();
            OperationResult objOperationResult = new OperationResult();

            List<KeyValueDTO> ListaTiposModalidad = new List<KeyValueDTO>();
            KeyValueDTO objDetallado = new KeyValueDTO();
            if (Globals.ClientSession.v_RucEmpresa == Constants.RucWortec)
            {
                objDetallado = new KeyValueDTO();
                objDetallado.Id = "3";
                objDetallado.Value1 = "FISICO AUXILIAR";
                ListaTiposModalidad.Add(objDetallado);

                objDetallado = new KeyValueDTO();
                objDetallado.Id = "4";
                objDetallado.Value1 = "VALORIZADO AUXILIAR";
                ListaTiposModalidad.Add(objDetallado);
            }
            cboModelo.Visible = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec;
            lblModelo.Visible = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec;

            objDetallado = new KeyValueDTO();
            objDetallado.Id = "1";
            objDetallado.Value1 = "FISICO";
            ListaTiposModalidad.Add(objDetallado);

            objDetallado = new KeyValueDTO();
            objDetallado.Id = "2";
            objDetallado.Value1 = "VALORIZADO";
            ListaTiposModalidad.Add(objDetallado);

            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", _objLineaBL.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", objEstablecimientoBL.GetAlmacenesXEstablecimiento(-1), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboModalidad, "Value1", "Id", ListaTiposModalidad, DropDownListAction.Select);
            
            if (Globals.ClientSession.v_RucEmpresa == Constants.RucWortec)
            {
                Utils.Windows.LoadUltraComboEditorList(cboModelo, "Value1", "Id", _objMarcaBL.LlenarComboModelo(ref objOperationResult), DropDownListAction.All);
             
            }
            
            ValidarFechas();
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            cboLinea.Value = "-1";
            cboMarca.Value = "-1";
            cboFormato.Value = ((int)FormatoCantidad.UnidadMedidaProducto).ToString();
            cboMoneda.Value = "1";
            cboModalidad.Value = "1";
            cboModelo.Value = "-1";
            chkIncluirNroPedido.Checked = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec;
            cboAgrupar.SelectedIndex = 0;

        }
        private void ValidarFechas()
        {
            string Periodo = Globals.ClientSession.i_Periodo.ToString();
            string Mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == Periodo)
            {
                dtpFechaHasta.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaHasta.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaHasta.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            }
            else
            {
                dtpFechaHasta.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaHasta.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaHasta.Value = DateTime.Parse((Periodo + "/12/31").ToString());
            }
        }
        private void cboEstablecimiento_SelectedIndexChanged(object sender, EventArgs e)
        {
            EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
            List<KeyValueDTO> x = new List<KeyValueDTO>();
            _whereAlmacenesConcatenados = string.Empty;
            _AlmacenesConcatenados = string.Empty;
            if (cboEstablecimiento.Value == null) return;

            x = objEstablecimientoBL.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));

            if (x.Count > 0)
            {
                foreach (var item in x)
                {
                    _whereAlmacenesConcatenados = _whereAlmacenesConcatenados + "IdAlmacen==" + item.Id + " || ";
                    _AlmacenesConcatenados = _AlmacenesConcatenados + item.Value1 + "/ ";
                }
                _whereAlmacenesConcatenados = _whereAlmacenesConcatenados.Substring(0, _whereAlmacenesConcatenados.Length - 4);
                //_AlmacenesConcatenados = cboEstablecimiento.Text + " / " + _AlmacenesConcatenados.Substring(0, _AlmacenesConcatenados.Length - 2);
                _AlmacenesConcatenados = _AlmacenesConcatenados.Substring(0, _AlmacenesConcatenados.Length - 2);
            }

            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.All);
            cboAlmacen.Value = "-1";

        }

        private void txtCodArticulo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarArticulo")
            {

                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
                frm.ShowDialog();
                if (frm._IdProducto != null)
                {
                    // txtNroCuenta.Text = frmPlanCuentasConsulta._NroSubCuenta.Trim();
                    txtCodArticulo.Text = frm._CodigoInternoProducto.Trim();
                }
                else
                {
                    // txtNroCuenta.Clear();
                    txtCodArticulo.Clear();
                }
            }

        }


        private void chkMostrarconstock_CheckedChanged(object sender, EventArgs e)
        {

            chkMostrarconstock0.Enabled = chkMostrarconstockNegativo.Enabled = !chkMostrarconstock.Checked;
            //chkMostrarconstock0.Checked = chkMostrarconstockNegativo.Checked  = !chkMostrarconstock.Checked;
        }

        private void frmStock_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void cboLinea_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (cboLinea.Value == null || cboLinea.Value.ToString() == "-1") return;

            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", cboLinea.Value.ToString()), DropDownListAction.Select);
            cboMarca.Value = "-1";
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
            VisualizarReporte(true);
        }

        private void chkMostrarconstock0_CheckedChanged(object sender, EventArgs e)
        {
            chkMostrarconstock.Enabled = chkMostrarconstockNegativo.Enabled = !chkMostrarconstock0.Checked;
            // chkMostrarconstock.Checked =  chkMostrarconstockNegativo.Checked =!chkMostrarconstock0.Checked;
        }

        private void chkMostrarconstockNegativo_CheckedChanged(object sender, EventArgs e)
        {
            chkMostrarconstock0.Enabled = chkMostrarconstock.Enabled = !chkMostrarconstockNegativo.Checked;
            // chkMostrarconstock0.Checked = chkMostrarconstock.Checked  =  !chkMostrarconstockNegativo.Checked;
        }

        private void chkMostrarSoloStockMinimo_CheckedChanged(object sender, EventArgs e)
        {

        }




    }
}
