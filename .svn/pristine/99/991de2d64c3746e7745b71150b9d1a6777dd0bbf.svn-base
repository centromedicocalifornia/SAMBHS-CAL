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
    public partial class frmRegistroVentaProductoAnalitico : Form
    {
        #region Declaraciones / Referencias
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private string _strGrupollave, _strNombreGrupollave;
        public  int _consideraDocumentosContables = -1;
        private string _Modalidad;
        string _whereAlmacenesConcatenados, _AlmacenesConcatenados ;
        #endregion

        #region Carga de inicializacion
        public frmRegistroVentaProductoAnalitico(string Modalidad)
        {
            InitializeComponent();
            _Modalidad = Modalidad;
         // _consideraDocumentosInternos = Modalidad == Constants.ModuloContable ? 0 : 1;
            BackColor = new GlobalFormColors().FormColor;
        }

        private void frmRegistroVentaProductoAnalitico_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            if (_Modalidad == Constants.ModuloContable)
            {
                _consideraDocumentosContables = 1;
                cboTipoDocumento.Value = _consideraDocumentosContables == 1 ? "1" : "-1";
            }
            else _consideraDocumentosContables = int.Parse(cboTipoDocumento.Value.ToString());
            cboTipoDocumento.Visible = _consideraDocumentosContables != 1;
            lblTipoDocumento.Visible = _consideraDocumentosContables != 1;
            
        }

        #endregion

        #region Cargar Reporte
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Ventas por Producto (Analítico)"
                : @"Reporte de Ventas por Producto (Analítico)";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = !estado;
        }
        private void CargarReporte(DateTime FechaRegistroIni, DateTime FechaRegistroFin, string FechaHoraImpresion, string IdCliente, string IdProducto, string Orden , bool Excel)
        {
            OperationResult objOperationResult = new OperationResult();
            List<string> Retonar = new List<string>();
            List<string> Retonar2 = new List<string>();

            List<ReporteRegistroVentaProductoAnalitico> aptitudeCertificate = null;
            var rp = new crRegistroVentaProductoAnalitico_();

            var _ListadoGrupos = (List<KeyValueDTO>)cboAgrupar.DataSource;
            _strGrupollave = "";
            _strNombreGrupollave = "";

            var Grupollave = new List<string>();
            var NombreGrupollave = new List<string>();

            for (int i = 0; i <= _ListadoGrupos.Count - 1; i++)
            {
                if (cboAgrupar.Value != null && _ListadoGrupos[i].Value2 != null && _ListadoGrupos[i].Value3 != null)
                {
                    if (cboAgrupar.Value.ToString().Trim() == _ListadoGrupos[i].Value2.Trim() && _ListadoGrupos[i].Value3.Trim() != "")
                    {
                        if (cboAgrupar.Text.Trim() == _ListadoGrupos[i].Value1)
                        {
                            _strNombreGrupollave = _ListadoGrupos[i].Value1;
                            var splitNombreGrupollave = _strNombreGrupollave.Split('/');
                            foreach (var s in splitNombreGrupollave)
                            {
                                if (s.Trim() != "")
                                    NombreGrupollave.Add(s);

                            }
                            _strNombreGrupollave = NombreGrupollave[0];
                            _strGrupollave = _ListadoGrupos[i].Value3;
                        }
                    }
                    var split = _ListadoGrupos[i].Value2.Split(',');
                    foreach (var s in split)
                    {
                        if (s.Trim() != "")
                            Retonar.Add(s);

                    }
                }
            }
            Retonar = Retonar.Distinct().ToList();
            for (var i = 0; i <= Retonar.Count - 1; i++)
            {
                Retonar2.Add(Retonar[0]);
            }

           
            string[] splitGrupollave = _strGrupollave.Split(',');
            foreach (string s in splitGrupollave)
            {
                if (s.Trim() != "")
                    Grupollave.Add(s);

            }
            if (Grupollave.Count > 0)
            {
                _strGrupollave = Grupollave[0];
            }
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {
                aptitudeCertificate = new VentaBL().ReporteRegistroVentaProductoAnalitico(ref objOperationResult, int.Parse(cboEstablecimiento.Value.ToString()), FechaRegistroIni, FechaRegistroFin, IdCliente, IdProducto, Orden, _strGrupollave, _strNombreGrupollave, cboLinea.Value.ToString(), cboMarca.Value.ToString(), int.Parse(cboTipoDocumento.Value.ToString()));
            
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
                    aptitudeCertificate = aptitudeCertificate.OrderBy(o => o.Grupollave).ThenBy(o => o.GrupoLlave2).ToList();
                    using (var dt = Utils.ConvertToDatatable(aptitudeCertificate))
                        if (Excel)
                        {
                            #region Headers
                            var columnas = new[]
                    {
                        "FechaRegistro","TipoDocumento", "Documento","CantidadDetalle","PrecioDetalle","ValorVentaDetalle","ValorVentaDetalleD","PrecioVenta","DocumentoContable"
                    };
                            var heads = new ExcelHeader[]
                    {
                        "FECHA","T. DOC", "DOC.", "CANTIDAD" , "PRECIO UNITARIO S/.","VALOR VENTA S/.","VALOR VENTA $." ,"P. VENTA S/." ,"TIPO DOCUMENTO"
                    };
                            #endregion

                            var objexcel = new ExcelReport(dt) { Headers = heads };
                            objexcel.AutoSizeColumns(1, 12,8, 20, 12, 12, 12, 12,12,15);
                            objexcel.SetTitle("VENTA-PRODUCTO ANALITICO");
                            objexcel.SetHeaders();
                            objexcel.EndSection += (_, e) =>
                            {
                                var obj = (ExcelReport)_;
                                obj.SetFormulas(3, "SUB TOTAL:", Enumerable.Range(0, 5).Select(i => string.Format("=SUM(${2}{0}:${2}{1})", e.StartPosition + 1, e.EndPosition, (char)('E' + i))).ToArray());
                                obj.CurrentPosition++;
                            };
                            objexcel.EndSectionGroup += (_, e) =>
                            {
                                if (e.EndSections.Length == 0) return;
                                var obj = (ExcelReport)_;
                                //obj.SetFormulas(3, "TOTAL:", Enumerable.Range(0, 5).Select(i => string.Format("=SUM(" + string.Join(",", e.EndSections.Select(n => "${0}" + n)) + ")", (char)('E' + i))).ToArray());
                                obj.SetFormulas(3, "TOTAL:", Enumerable.Range(0, 5).Select(i => string.Format("=" + string.Join("+", e.EndSections.Select(n => "${0}" + (n))), (char)('E' + i))).ToArray());


                                obj.CurrentPosition++;
                            };
                            var filtros = cboAgrupar.Text == @"SIN AGRUPAR" ? new[] { "GrupoLlave" } : new[] { "GrupoLlave", "Grupollave2" };
                            objexcel.SetData( ref objOperationResult ,columnas, filtros);
                            var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                            objexcel.Generate(path);
                            System.Diagnostics.Process.Start(path);
                        }
                        else
                        {
                            ds1.Tables.Add(dt);
                            ds1.Tables[0].TableName = "dsRegistroVentaProductoAnalitico";
                            rp.SetDataSource(ds1);
                            rp.SetParameterValue("FechaHoraImpresion", FechaHoraImpresion);
                            rp.SetParameterValue("IdMoneda", 1);
                            rp.SetParameterValue("CantidadDecimal", Globals.ClientSession.i_CantidadDecimales ?? 2);
                            rp.SetParameterValue("CantidadDeciamlPrecio", Globals.ClientSession.i_PrecioDecimales ?? 2);
                            rp.SetParameterValue("Fecha", "DEL " + FechaRegistroIni.Date.Day.ToString("00") + "/" + FechaRegistroIni.Date.Month.ToString("00") + "/" + FechaRegistroIni.Date.Year.ToString() + " AL " + FechaRegistroFin.Date.Day.ToString("00") + "/" + FechaRegistroFin.Date.Month.ToString("00") + FechaRegistroFin.Date.Year.ToString());
                            rp.SetParameterValue("NombreEmpresa", Globals.ClientSession.v_CurrentExecutionNodeName.ToUpper());
                            rp.SetParameterValue("RucEmpresa", Globals.ClientSession.v_RucEmpresa);
                            rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                            rp.SetParameterValue("Linea", cboAgrupar.Text.Trim());
                            crystalReportViewer1.ReportSource = rp;
                            crystalReportViewer1.Show();
                        }
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            
        }

        #endregion

        #region Controles Botones
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            ReporteExcel(false);

        }

        private void ReporteExcel(bool Excel)
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
                    var ValorIdCliente = TxtCodigo.Text.Trim() == "" ? "" : TxtCodigo.Text.Trim();
                    var ValorIdProducto = TxtProductoCodigo.Text.Trim() == "" ? "" : TxtProductoCodigo.Text.Trim();
                    //Moneda = int.Parse(cboMoneda.Value.ToString());
                    var strOrderExpression = cboOrden.Value.ToString() == "-1" ? "" : cboOrden.Value.ToString();
                    CargarReporte(DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), chkHoraimpresion.Checked == true ? "1" : "0", ValorIdCliente, ValorIdProducto, strOrderExpression,Excel);
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

       
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            // Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            var dthierarchy = new DatahierarchyBL();
            Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", dthierarchy.GetDataHierarchiesForCombo(ref objOperationResult, 70, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAgrupar, "Value1", "Value2", dthierarchy.GetDataHierarchiesForCombo(ref objOperationResult, 86, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", new LineaBL().LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", new MarcaBL().LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboAgrupar.SelectedIndex = 2;
            cboOrden.Text = @"DOCUMENTO";
            cboLinea.Value = "-1";
            cboMarca.Value = "-1";
            cboTipoDocumento.Value  = "-1";
            
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

        private void TxtCodigo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var frm = new Mantenimientos.frmBuscarCliente("V", TxtCodigo.Text.Trim());
            frm.ShowDialog();

            if (frm._IdCliente == null) return;
            TxtCodigo.Text = frm._CodigoCliente;
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
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.All);
            cboAlmacen.Value = "-1";
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            ReporteExcel(true);
        }

    }
}
