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
using SAMBHS.Almacen.BL;
using CrystalDecisions.CrystalReports.Engine;
using System.Globalization;
using System.Threading;
using System.IO;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmKardex : Form
    {
        #region Field
        private string _strFilterExpression;
        private string _whereAlmacenesConcatenados;
        private string _whereAlmacenesTemporales;
        private string _almacenesConcatenados;
        public string Modulo = string.Empty;
        private readonly int _considerarDocumentosInternos = -1;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private List<KardexList> _result = new List<KardexList>();
        private readonly List<KeyValueDTO> _listaAlmacenesUnificados = new List<KeyValueDTO>();
        private readonly DatahierarchyBL _objDatahierarchyBl = new DatahierarchyBL();
        MarcaBL _objMarcaBL = new MarcaBL();
        #endregion

        #region Init
        public frmKardex(string modo)
        {
            InitializeComponent();
            _considerarDocumentosInternos = modo == "C" ? 0 : 1;

        }
        private void frmKardex_Load(object sender, EventArgs e)
        {

            BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            chkNroPedido.Checked = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec?true   : false ;

        }
        #endregion


        private void CargarCombos()
        {
            var objEstablecimientoBl = new EstablecimientoBL();
            var objLineaBl = new LineaBL();
            var objOperationResult = new OperationResult();
            var obj1 = new KeyValueDTO
            {
                Value1 = "Almacenes Unificados",
                Id = "1000"
            };
            _listaAlmacenesUnificados.Add(obj1);

            var listaAlmacenes = objEstablecimientoBl.GetAlmacenesXEstablecimiento(-1).Concat(_listaAlmacenesUnificados).ToList();

            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", objEstablecimientoBl.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", objLineaBl.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", listaAlmacenes, DropDownListAction.All);

            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", new MarcaBL().LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.All);
            cboFormato.Value = ((int)FormatoCantidad.UnidadMedidaProducto).ToString();
            ValidarMes();
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboLinea.Value = "-1";
            cboMarca.Value = "-1";
            cboModalidad.Value = "1";
            cboAgrupar.SelectedIndex = 0;
            chkNoConsideraGuiaCompra.Visible = false;
            cboMoneda.Value = "1";
            cboModelo.Visible = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec;
            lblModelo.Visible = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec;
            List<KeyValueDTO> ListaTiposModalidad = new List<KeyValueDTO>();
            KeyValueDTO objDetallado = new KeyValueDTO();
            if (Globals.ClientSession.v_RucEmpresa == Constants.RucWortec)
            {
                objDetallado = new KeyValueDTO();
                objDetallado.Id = "4";
                objDetallado.Value1 = "FISICO AUXILIAR";
                ListaTiposModalidad.Add(objDetallado);

                objDetallado = new KeyValueDTO();
                objDetallado.Id = "5";
                objDetallado.Value1 = "VALORIZADO AUXILIAR";
                ListaTiposModalidad.Add(objDetallado);
            }
            objDetallado = new KeyValueDTO();
            objDetallado.Id = "1";
            objDetallado.Value1 = "FISICO";
            ListaTiposModalidad.Add(objDetallado);

            objDetallado = new KeyValueDTO();
            objDetallado.Id = "2";
            objDetallado.Value1 = "VALORIZADO";
            ListaTiposModalidad.Add(objDetallado);

            objDetallado = new KeyValueDTO();
            objDetallado.Id = "3";
            objDetallado.Value1 = "VALORIZADO-SUNAT";
            ListaTiposModalidad.Add(objDetallado);

            objDetallado = new KeyValueDTO();
            objDetallado.Id = "7";
            objDetallado.Value1 = "FISICO-SUNAT";
            ListaTiposModalidad.Add(objDetallado);

            Utils.Windows.LoadUltraComboEditorList(cboModalidad, "Value1", "Id", ListaTiposModalidad, DropDownListAction.Select);
            if (Globals.ClientSession.v_RucEmpresa == Constants.RucWortec)
            {
                Utils.Windows.LoadUltraComboEditorList(cboModelo, "Value1", "Id", _objMarcaBL.LlenarComboModelo(ref objOperationResult), DropDownListAction.All);

            }
            cboModalidad.Value = "1";
            cboModelo.Value = "-1";
        
        
        }
        #region Methods
        private void ValidarMes()
        {
            lblPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            if (Globals.ClientSession.i_Periodo == DateTime.Now.Year)
            {
                nupMesInicio.Value = DateTime.Now.Month;
                nupMesInicio.Maximum = DateTime.Now.Month;
                nupMesInicio.Minimum = 1;
                nupMesFin.Value = DateTime.Now.Month;
                nupMesFin.Maximum = DateTime.Now.Month;
                nupMesFin.Minimum = 1;
            }
            else
            {
                nupMesInicio.Value = DateTime.Now.Month;
                nupMesInicio.Maximum = 12;
                nupMesInicio.Minimum = 1;
                nupMesFin.Value = 12;
                nupMesFin.Maximum = 12;
                nupMesFin.Minimum = 1;

            }
        }
        private void VisualizarReporte(bool export)
        {
            try
            {
                if (uvValidar.Validate(true, false).IsValid)
                {
                    var fecha1 = DateTime.Parse("01/" + nupMesInicio.Value.ToString(CultureInfo.InvariantCulture) + "/" + Globals.ClientSession.i_Periodo + " 00:00");
                    var fecha2 = DateTime.Parse(DateTime.DaysInMonth(int.Parse(Globals.ClientSession.i_Periodo.ToString()), int.Parse(nupMesFin.Value.ToString())).ToString() + "/" + nupMesFin.Value + "/" + Globals.ClientSession.i_Periodo.ToString() + " 23:59");
                    int idMoneda;
                    string moneda, nroPedido = string.Empty, codProducto = string.Empty;
                    var filters = new List<string>();
                    if (cboAlmacen.Value.ToString() != "-1" && cboAlmacen.Value.ToString() != "1000")
                    {
                        filters.Add("IdAlmacen==" + cboAlmacen.Value);
                    }
                    else if (cboAlmacen.Value.ToString() != "1000")
                    {
                        filters.Add("(" + (_whereAlmacenesConcatenados ?? _whereAlmacenesTemporales) + ")");
                    }

                    if (int.Parse(cboMoneda.Value.ToString()) == (int)Currency.Soles)
                    {
                        idMoneda = 1;
                        moneda = "Soles";
                    }
                    else
                    {
                        idMoneda = 2;
                        moneda = "Dolares";
                    }

                    _strFilterExpression = string.Join(" && ", filters);
                    _whereAlmacenesConcatenados = null;

                    var empresa = new NodeBL().ReporteEmpresa();
                    var incluirPedido = chkNroPedido.Checked ? 1 : 0;
                    var articulosStock0 = chkStockArticulosStock.Checked ? 1 : 0;
                    var articulosMovimiento = chkSoloArticulosMovimiento.Checked ? 1 : 0;
                    if (txtNroPedido.Text != string.Empty) nroPedido = txtNroPedido.Text.Trim();
                    if (txtArtIni.Text != string.Empty) codProducto = txtArtIni.Text.Trim();

                    CargarReporte(fecha1, fecha2, _strFilterExpression, idMoneda, empresa[0].NombreEmpresaPropietaria, empresa[0].RucEmpresaPropietaria, moneda, incluirPedido, codProducto, nroPedido, articulosStock0, articulosMovimiento, export);
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show("Ocurrió un error al realizar Reporte Kardex Físico / Valorizado.\n Información Adicional :" + ex.Message, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte Kardex Físico / Valorizado"
                : @"Reporte Kardex Físico / Valorizado";
            pBuscando.Visible = estado;

            BtnImprimir.Enabled = btnExcel.Enabled = !estado;
        }
        private void CargarReporte(DateTime fecha1, DateTime fecha2, string _strFilterExpression, int IdMoneda, string NombreEmpresas, string RucEmpresa, string Moneda, int IncluirPedido, string CodProducto, string NroPedido, int ArticulosStock0, int ArticulosMovimiento, bool Export)
        {

            try
            {
                var objOperationResult = new OperationResult();
                OcultarMostrarBuscar(true);
                
                Cursor.Current = Cursors.WaitCursor;

                Task.Factory.StartNew(() =>
                {
                 _result = new AlmacenBL().ReporteKardexValorizadoSunat(ref objOperationResult, DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo.ToString()), fecha2, _strFilterExpression, IdMoneda, NombreEmpresas, RucEmpresa, Moneda, int.Parse(cboEstablecimiento.Value.ToString()), IncluirPedido, CodProducto, NroPedido, ArticulosStock0, ArticulosMovimiento, _considerarDocumentosInternos, int.Parse(cboFormato.Value.ToString()), cboMarca.Value.ToString(), chkNoConsideraGuiaCompra.Checked, chkStockArticulosNegativo.Checked ? 1 : 0, chkStockArticulosStock0.Checked ? 1 : 0, cboLinea.Value.ToString(), fecha1,false,"",(cboModelo.Value ==null || cboModelo.Value.ToString ()=="-1") ?"": cboModelo.Text.Trim (),cboAgrupar.Value.ToString ());
                      
                }, _cts.Token)
                .ContinueWith(t =>
                {
                    if (_cts.IsCancellationRequested) return;
                    OcultarMostrarBuscar(false);
                    Cursor.Current = Cursors.Default;
                    if (objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte Kardex Físico / Valorizado.\n Información Adicional :" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);  
                        return;
                    }


                    var empresa = new NodeBL().ReporteEmpresa();
                    string fecha;
                    if (nupMesInicio.Value == nupMesFin.Value)
                    {

                        fecha = "MES :" + new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(nupMesInicio.Value.ToString(CultureInfo.InvariantCulture))).ToUpper() + " DEL " + Globals.ClientSession.i_Periodo.ToString();
                    }
                    else
                    {
                        fecha = "MES :" + new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(nupMesInicio.Value.ToString(CultureInfo.InvariantCulture))).ToUpper() + " A " + new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(nupMesFin.Value.ToString(CultureInfo.InvariantCulture))).ToUpper() + " DEL " + Globals.ClientSession.i_Periodo.ToString();
                    }
                    if (int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.Fisico || int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.FisicoAuxiliar || int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.FisicoSunat)
                    {
                        ReportDocument rp = new ReportDocument();
                        if (int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.Fisico)
                        {
                            rp = new crKardexFisico();
                        }
                        else if (int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.FisicoSunat)
                        {
                            rp = new crKardexFisicoSunatt();
                        }
                        else 
                        {
                         rp = new crKardexFisicoAuxiliar();
                        }

                       
                        var dt = Utils.ConvertToDatatable(_result);

                        if (Export)
                        {
                            #region Headers

                            var columnas = new[]
                            {
                                "v_Fecha", "v_NombreTipoMovimiento", "Guia", "Documento", "ClienteProveedor","Marca","Modelo","Ubicacion","NroParte","Ingreso_CantidadInicial","Salida_Cantidad","Saldo_Cantidad"
                            };


                            var heads = new[]
                            {
                                new ExcelHeader{
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                         "FECHA", "MOVIMIENTO", "GUÍA", "DOCUMENTO", "CLIENTE","MARCA","MODELO","UBICACIÓN","NRO. PARTE"
                                    }
                                },
                                new ExcelHeader
                                {
                                    Title = "ENTRADAS", Headers = new ExcelHeader[]{"CANTIDAD"}
                                }
                                ,
                                new ExcelHeader 
                                {
                                    Title = "SALIDAS", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "CANTIDAD"
                                    }
                                },

                                 new ExcelHeader 
                                {
                                    Title = "SALDOS", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "CANTIDAD"
                                    }
                                }
                            };
                            #endregion

                            var excel = new ExcelReport(dt) { Headers = heads };
                            excel.AutoSizeColumns(1, 20, 25, 25, 25, 50,25, 25, 25,25, 25, 25, 25);
                            excel.SetTitle("KARDEX FÍSICO");

                            excel[2].Cells[4].Value = fecha;
                            excel.SetHeaders();
                            excel.EndSection += excel_EndSection;
                            excel.EndSectionGroup += excel_EndSectionGroup;
                            var filtros = new[] { "Almacen", "v_NombreProducto" };
                            excel.SetData(ref  objOperationResult ,columnas, filtros);
                            
                           if ( objOperationResult.Success ==0)
                            {
                                UltraMessageBox.Show("Ocurrió un error al exportar excel ", "Sistema");
                                return;
                            }

                            var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                            excel.Generate(path);
                            System.Diagnostics.Process.Start(path);

                        }
                        else
                        {
                            dt.TableName = "dtKardex";
                            using (var ds1 = new DataSet())
                            {
                                ds1.Tables.Add(dt);
                                rp.SetDataSource(ds1); 
                            }

                            rp.SetParameterValue("CantidadDecimal", Globals.ClientSession.i_CantidadDecimales ?? 2);
                            rp.SetParameterValue("Fecha", fecha);
                            rp.SetParameterValue("Establecimiento", "ESTABLECIMIENTO : " + cboEstablecimiento.Text.Trim().ToUpper());
                            rp.SetParameterValue("PrecioDecimal", Globals.ClientSession.i_PrecioDecimales ?? 2);
                            rp.SetParameterValue("NroRegistros", _result.Count());
                            rp.SetParameterValue("NombreEmpresaPropietaria", Globals.ClientSession.v_CurrentExecutionNodeName);
                            rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Globals.ClientSession.v_RucEmpresa);
                            rp.SetParameterValue("LetrasTotalProducto", "TOTAL POR PRODUCTO : ");
                            rp.SetParameterValue("LetrasTotalAlmacen", "TOTAL POR ALMACÉN : ");
                            rp.SetParameterValue("LetrasTotal", "TOTAL GENERAL : ");
                            rp.SetParameterValue("ImprimirFechaHora", uckImprimirFechaHora.Checked ? true : false);
                            if ((int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.FisicoSunat))
                            {
                                rp.SetParameterValue("MetodoEvaluacion", "MÉTODO DE EVALUACIÓN  : PROMEDIO ");
                                rp.SetParameterValue("DireccionEstablecimiento", "ESTABLECIMIENTO : " + empresa.FirstOrDefault().Direccion);
                                rp.SetParameterValue("GuiasNoEncontradas", "");
                                rp.SetParameterValue("NroRegistrosGuiasNoEncontradas", 0);
                            }
                            crystalReportViewer1.ReportSource = rp;
                            crystalReportViewer1.Show();
                            crystalReportViewer1.Zoom(110);
                        }
                    }
                    else
                    {
                        ReportDocument rp;
                        if (int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.Valorizado)
                        {
                            rp = new crKardex();
                        }
                        else if (int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.ValorizadoAuxiliar)
                        {
                            rp = new crKardexAuxiliar();
                        }
                        else 
                        {
                        
                         rp = new crKardexSunatt();
                        }

                        var dt = Utils.ConvertToDatatable(_result);


                        if (Export)
                        {

                            #region Headers

                            var columnas = new[]
                            {
                                "v_Fecha", "v_NombreTipoMovimiento", "Guia", "Documento", "ClienteProveedor","Marca","Modelo","Ubicacion","NroParte","Ingreso_CantidadInicial","Ingreso_PrecioInicial","Ingreso_TotalInicial","Salida_Cantidad","Salida_Precio","Salida_Total","Saldo_Cantidad","Saldo_Precio","Saldo_Total"
                            };

                            var heads = new[]
                            {
                                new ExcelHeader{
                                    Title = "", 
                                    Headers = new ExcelHeader[]
                                    {
                                         "FECHA", "MOVIMIENTO", "GUÍA", "DOCUMENTO", "CLIENTE","MARCA","MODELO","UBICACIÓN","NRO. PARTE"
                                    }
                                },
                                new ExcelHeader
                                {
                                    Title = "ENTRADAS", Headers = new ExcelHeader[]{"CANTIDAD","P. UNITARIO","TOTAL"}
                                }
                                ,
                                new ExcelHeader 
                                {
                                    Title = "SALIDAS", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "CANTIDAD","P. UNITARIO","TOTAL"
                                    }
                                },

                                 new ExcelHeader 
                                {
                                    Title = "SALDOS", 
                                    Headers = new ExcelHeader[]
                                    {
                                        "CANTIDAD","P. UNITARIO","TOTAL"
                                    }
                                }

                            };

                            #endregion

                            var excel = new ExcelReport(dt) { Headers = heads };
                            excel.AutoSizeColumns(1, 20, 25, 25, 25, 50, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20);
                            excel.SetTitle("KARDEX VALORIZADO");

                            excel[2].Cells[4].Value = fecha;
                            excel.SetHeaders();
                            excel.EndSection += excel_EndSection;
                            excel.EndSectionGroup += excel_EndSectionGroup;
                            var filtros = new[] { "Almacen", "v_NombreProducto" };
                            excel.SetData( ref objOperationResult ,columnas, filtros);
                            if (objOperationResult.Success == 0)
                            {
                                UltraMessageBox.Show("Ocurrió un error al exportar excel ", "Sistema");
                                return;
                            }
                            var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                            excel.Generate(path);
                            System.Diagnostics.Process.Start(path);
                        }
                        else
                        {
                            dt.TableName = "dtKardex";
                            using (var ds1 = new DataSet())
                            {
                                ds1.Tables.Add(dt);
                                rp.SetDataSource(ds1); 
                            }

                            rp.SetParameterValue("CantidadDecimal", Globals.ClientSession.i_CantidadDecimales ?? 2);
                            rp.SetParameterValue("Fecha", fecha);
                            rp.SetParameterValue("Establecimiento", "ESTABLECIMIENTO : " + cboEstablecimiento.Text.Trim().ToUpper());
                            rp.SetParameterValue("PrecioDecimal", Globals.ClientSession.i_PrecioDecimales ?? 2);
                            rp.SetParameterValue("NroRegistros", _result.Count);
                            rp.SetParameterValue("NombreEmpresaPropietaria", Globals.ClientSession.v_CurrentExecutionNodeName);
                            rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Globals.ClientSession.v_RucEmpresa);
                            var guiasIdentificadas = new List<string>();
                            var guias = string.Join(" ", guiasIdentificadas);
                            rp.SetParameterValue("GuiasNoEncontradas", "LAS SIGUIENTES GUIAS DE COMPRAS NO FUERON CONSIDERADAS : \n" + guias);
                            rp.SetParameterValue("NroRegistrosGuiasNoEncontradas", guiasIdentificadas.Count());
                            rp.SetParameterValue("LetrasTotalProducto", "TOTAL POR PRODUCTO : ");
                            rp.SetParameterValue("LetrasTotalAlmacen", "TOTAL POR ALMACÉN : ");
                            rp.SetParameterValue("LetrasTotal", "TOTAL GENERAL : ");
                            rp.SetParameterValue("ImprimirFechaHora", uckImprimirFechaHora.Checked ? true : false);
                            if ((int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.ValorizadoSunat))
                            {
                                rp.SetParameterValue("MetodoEvaluacion", "MÉTODO DE EVALUACIÓN  : PROMEDIO ");
                                rp.SetParameterValue("DireccionEstablecimiento", "ESTABLECIMIENTO : " + empresa.FirstOrDefault().Direccion);
                            }
                            crystalReportViewer1.ReportSource = rp;
                            crystalReportViewer1.Show();
                            crystalReportViewer1.Zoom(110);
                        }
                    }

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show("Ocurrió un error al realizar Reporte Kardex Físico / Valorizado ,Data OK ,Asignación Crystal.\n Información Adicional :" + ex.Message, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void excel_EndSectionGroup(object sender, ExcelReportSectionGroupEventArgs e)
        {
            //if (e.EndSections.Length == 0) return;
            //var obj = (ExcelReport)sender;
            //var totalLabel = "TOTAL " + (e.Group == 1 ? "POR ALMACEN" : "GENERAL") + ":";
            //if (int.Parse(cboModalidad.Value.ToString()) == (int) TipoKardex.Fisico)
            //{
            //    obj.SetFormulas(5, totalLabel,
            //        Enumerable.Range(0, 2)
            //        .Select(  i => string.Format("=" + string.Join("+", e.EndSections.Select(n => "${0}" + n)), (char) ('G' + i)))
            //        .ToArray());
            //}
            //else
            //{
            //    obj.SetFormulas(5, totalLabel, "=" + string.Join("+", e.EndSections.Select(n => "$G" + n)));
            //    obj.SetFormulas(8, totalLabel, "=" + string.Join("+", e.EndSections.Select(n => "$J" + n)));           
            //}
        }

        #endregion

        #region Events UI
        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            VisualizarReporte(btnExcel == sender);
        }

        private void excel_EndSection(object sender, ExcelReportSectionEventArgs e)
        {
            if (int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.Fisico || int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.FisicoAuxiliar)
            {
                //if (e.StartPosition == e.EndPosition) return;
                //var obj = (ExcelReport)sender;
                //obj[e.StartPosition - 1].Cells[1].Value = e.FirsRow["v_NombreProducto"];
               // obj.SetFormulas(5, "TOTAL POR PRODUCTO : ", string.Format("=SUM(G{0}:G{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(H{0}:H{1})", e.StartPosition + 1, e.EndPosition), string.Format("=I{0}", e.EndPosition));
               // obj.CurrentPosition++;

              
            }
            else
            {

                //if (e.StartPosition == e.EndPosition) return;
                //var obj = (ExcelReport)sender;
                //obj[e.StartPosition - 1].Cells[1].Value = e.FirsRow["v_NombreProducto"];
                //obj.SetFormulas(5, "TOTAL POR PRODUCTO : ", string.Format("=SUM(G{0}:G{1})", e.StartPosition + 1, e.EndPosition), "", string.Format("=SUM(I{0}:I{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(J{0}:J{1})", e.StartPosition + 1, e.EndPosition), "", string.Format("=SUM(L{0}:L{1})", e.StartPosition + 1, e.EndPosition));
                //obj.CurrentPosition++;
            }

        }

        private void InsertTable(ExcelReport rp, IList<int> gr)
        {
            rp.CurrentPosition++;
            var row = rp[rp.CurrentPosition++];
            row.Cells[1].Value = "TOTAL DOCUMENTOS DE VENTA EMITIDOS :";
            row.Cells[5].Value = "TOTAL MONTO COBRADO :";
            if (gr.Count > 0)
            {
                row.Cells[4].ApplyFormula("=I" + gr[0]);
                row.Cells[7].ApplyFormula("=I" + gr[0]);
            }
            row = rp[rp.CurrentPosition++];
            row.Cells[1].Value = "TOTAL DOCUMENTOS DE VENTA CRÉDITO :";
            row.Cells[5].Value = "TOTAL MONTO PAGADO :";
            if (gr.Count > 1)
            {
                row.Cells[4].ApplyFormula("=I" + (gr[1] - 3));
                if (gr.Count > 2)
                    row.Cells[7].ApplyFormula("=K" + (gr[2] - 3));
            }
            row = rp[rp.CurrentPosition];
            row.Cells[1].Value = "TOTAL DOCUMENTOS DE COMPRA REGISTRADOS :";
            row.Cells[5].Value = "TOTAL ENVIO EFECTIVO  : *:";
            if (gr.Count > 2)
            {
                row.Cells[4].ApplyFormula("=I" + (gr[2] - 3));
                decimal caja;
                //decimal.TryParse(TxtDineroCaja.Text ?? "0", out caja);
                // row.Cells[7].ApplyFormula(string.Format("={0:N} + $H{1}-$H{2}", caja, rp.CurrentPosition - 1, rp.CurrentPosition));
            }
            rp[rp.CurrentPosition + 1].Cells[1].Value = "TOTAL EN EFECTIVO = DINERO EN CAJA +(TOTAL MONTO COBRADO - TOTAL MONTO PAGADO)";
        }

        private void cboEstablecimiento_SelectedIndexChanged(object sender, EventArgs e)
        {
            var objEstablecimientoBl = new EstablecimientoBL();
            var x = new List<KeyValueDTO>();
            _whereAlmacenesConcatenados = string.Empty;
            _almacenesConcatenados = string.Empty;

            if (cboEstablecimiento.Value == null || cboEstablecimiento.Value.ToString ()=="-1") return;

            x = objEstablecimientoBl.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString())).Concat(_listaAlmacenesUnificados).ToList();

            if (x.Count > 0)
            {
                foreach (var item in x.Where(l => l.Id != "1000"))
                {
                    _whereAlmacenesConcatenados = _whereAlmacenesConcatenados + "IdAlmacen==" + item.Id + " || ";
                    _almacenesConcatenados = _almacenesConcatenados + item.Value1 + ", ";
                }
                _whereAlmacenesConcatenados = _whereAlmacenesConcatenados.Substring(0, _whereAlmacenesConcatenados.Length - 4);
                _almacenesConcatenados = _almacenesConcatenados.Substring(0, _almacenesConcatenados.Length - 2);
                _whereAlmacenesTemporales = _whereAlmacenesConcatenados;
            }
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.All);
            cboAlmacen.Value = "-1";
        }

        private void nupMesFin_ValueChanged(object sender, EventArgs e)
        {
            nupMesInicio.Maximum = nupMesFin.Value;
        }

        private void chkNroPedido_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNroPedido.Checked)
            {
                txtNroPedido.Enabled = true;
            }
            else
            {
                txtNroPedido.Enabled = false;
                txtNroPedido.Clear();
            }
        }

        private void txtArtIni_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key != "btnBuscarArticulo") return;
            using (var frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null))
            {
                frm.ShowDialog();
                txtArtIni.Text = frm._IdProducto != null ? frm._CodigoInternoProducto.Trim() : string.Empty;
            }
        }

        private void frmKardex_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void cboLinea_ValueChanged(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            if (cboLinea.Value == null || cboLinea.Value.ToString() == "-1") return;

            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", new MarcaBL().LlenarComboMarca(ref objOperationResult, "v_CodMarca", cboLinea.Value.ToString()), DropDownListAction.Select);
            cboMarca.Value = "-1";
        }


        private void cboModalidad_ValueChanged(object sender, EventArgs e)
        {
            if (int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.Fisico || int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.FisicoAuxiliar)
            {
                chkNoConsideraGuiaCompra.Checked = false;
                chkNoConsideraGuiaCompra.Visible = true;
                cboFormato.Enabled = true;
                cboAlmacen.Enabled = true;
                //chkNroPedido.Checked = true;
                chkNroPedido.Checked = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec;
                chkNroPedido.Enabled = true;
                chkSoloArticulosMovimiento.Enabled = true;
                chkStockArticulosNegativo.Enabled = true;
                chkStockArticulosStock.Enabled = true;
                chkStockArticulosStock0.Enabled = true;
                chkNoConsideraGuiaCompra.Enabled = true;
            }
            else if (int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.ValorizadoSunat || int.Parse(cboModalidad.Value.ToString()) == (int)TipoKardex.FisicoSunat )
            {
                cboFormato.Value = ((int)FormatoCantidad.Unidades).ToString();
                chkNoConsideraGuiaCompra.Visible = true;
                cboFormato.Enabled = false;
                cboAlmacen.Value = "1000";
                cboAlmacen.Enabled = false;
                chkNroPedido.Checked =  Globals.ClientSession.v_RucEmpresa ==Constants.RucWortec ?true: false;
                chkNroPedido.Enabled = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec;
                chkSoloArticulosMovimiento.Checked = false;
                chkSoloArticulosMovimiento.Enabled = true;
                chkStockArticulosNegativo.Checked = false;
                chkStockArticulosNegativo.Enabled = false;
                chkStockArticulosStock.Checked = false;
                chkStockArticulosStock.Enabled = false;
                chkStockArticulosStock0.Checked = false;
                chkStockArticulosStock0.Enabled = false;
                chkNoConsideraGuiaCompra.Checked = false;
                chkNoConsideraGuiaCompra.Enabled = false;
            }
            else
            {
                chkNoConsideraGuiaCompra.Visible = true;
                cboFormato.Enabled = true;
                cboAlmacen.Enabled = true;
               // chkNroPedido.Checked = true;
                chkNroPedido.Checked = Globals.ClientSession.v_RucEmpresa == Constants.RucWortec ? true : false;
                chkNroPedido.Enabled = true;
                chkSoloArticulosMovimiento.Enabled = true;
                chkStockArticulosNegativo.Enabled = true;
                chkStockArticulosStock.Enabled = true;
                chkStockArticulosStock0.Enabled = true;
                chkNoConsideraGuiaCompra.Enabled = true;
            }

            

        }

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

        private void chkStockArticulosStock_CheckedChanged(object sender, EventArgs e)
        {
            chkStockArticulosNegativo.Enabled = chkStockArticulosStock0.Enabled = !chkStockArticulosStock.Checked;
        }

        private void chkStockArticulosStock0_CheckedChanged(object sender, EventArgs e)
        {
            chkStockArticulosNegativo.Enabled = chkStockArticulosStock.Enabled = !chkStockArticulosStock0.Checked;
        }

        private void chkStockArticulosNegativo_CheckedChanged(object sender, EventArgs e)
        {
            chkStockArticulosStock.Enabled = chkStockArticulosStock0.Enabled = !chkStockArticulosNegativo.Checked;
        }
        #endregion

    }
}
