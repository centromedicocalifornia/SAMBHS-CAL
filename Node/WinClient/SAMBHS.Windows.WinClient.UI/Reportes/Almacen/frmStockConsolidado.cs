using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BL;
using SAMBHS.Common.BE;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmStockConsolidado : Form
    {
        #region Fields
        private readonly EstablecimientoBL _objEstablecimientoBl = new EstablecimientoBL();
        private readonly DatahierarchyBL _objDatahierarchyBl = new DatahierarchyBL();
        private string _whereAlmacenesConcatenados, _almacenesConcatenados;
        readonly LineaBL _objLineaBl = new LineaBL();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        readonly MarcaBL _objMarcaBl = new MarcaBL();
        public string VCodigoInterno = string.Empty;
        #endregion

        #region Init & Load
        public frmStockConsolidado(string args)
        {
            InitializeComponent();
        }

        private void frmStockConsolidado_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            var objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", _objEstablecimientoBl.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboOrdenar, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 96, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", _objLineaBl.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBl.LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.All);
            
            cboStock.Value = "-1";
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboOrdenar.Value = "1";
            ValidarFechas();
            cboLinea.Value = "-1";
            cboMarca.Value = "-1";
            cboModelo.Value = "2";
            cboEstadoProducto.Value = "-1";
        }
        #endregion

        #region Methods
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = (estado ? @"Generando... " : string.Empty) + @"Reporte de Consolidado Almacenes";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = btnExcel.Enabled = !estado;
        }

        private void CargarReporte(string almacenes, int stock, int pstrModelo, int pstrEstablecimiento, bool exportExcel)
        {
            var objOperationResult = new OperationResult();
            var strOrderExpression = string.Empty;
            var listGroupsOrder = _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 96, null);

            foreach (var listGroup in listGroupsOrder)
            {
                if (cboOrdenar.Text.Trim() == listGroup.Value1.Trim())
                    strOrderExpression = listGroup.Value3;
            }

            Cursor.Current = Cursors.WaitCursor;
            OcultarMostrarBuscar(true);

            if (pstrModelo == 0)
            {
                List<ReporteStockConsolidado> aptitudeCertificate = null;
                //Add color, talla , material, temporada
                var filtros = (from Control ctr in pnOpciones.ClientArea.Controls
                               select ctr as Infragistics.Win.UltraWinEditors.UltraCheckEditor into chk
                               where chk != null && chk.Checked && chk.Text != "Marcar Todos"
                               select chk.Text).ToList();
             
                
               

                Task.Factory.StartNew(() =>
                {
                    var fechaInicial = new DateTime(Globals.ClientSession.i_Periodo ?? DateTime.Now.Year, 1, 1);
                    var fechaFinal = dtpFechaFin.Value.AddHours(23).AddMinutes(59);
                    var bl = new AlmacenBL();
                    aptitudeCertificate = exportExcel && filtros.Count > 0 ? bl.ReporteStockConsolidado2(ref objOperationResult, fechaInicial, fechaFinal, cboLinea.Value.ToString(), pstrEstablecimiento, VCodigoInterno, cboMarca.Value.ToString(), cboStock.Value == "1"  , cboStock.Value =="2",cboStock.Value =="3",cboStock.Value =="4" ,int.Parse ( cboEstadoProducto.Value.ToString ()) )
                        : bl.ReporteStockConsolidado(ref objOperationResult, fechaInicial, fechaFinal, strOrderExpression, cboLinea.Value.ToString(), pstrEstablecimiento, VCodigoInterno, cboMarca.Value.ToString(), cboStock.Value == "1", cboStock.Value == "2", cboStock.Value =="3", cboStock.Value =="4"?true:false ,int.Parse(cboEstadoProducto.Value.ToString()));
                }, _cts.Token)
                .ContinueWith(t =>
                {
                    if (_cts.IsCancellationRequested) return;
                    OcultarMostrarBuscar(false);
                    Cursor.Current = Cursors.Default;

                    if (objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte Stock Consolidado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    using (var dt = Utils.ConvertToDatatable(aptitudeCertificate))
                    {
                        if (exportExcel)
                            ExportExcel(dt, filtros);
                        else
                            Report1(dt, stock);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {          

                List<ReporteStockConsolidadoCajasUnidades> aptitudeCertificateCu = null;
                Task.Factory.StartNew(() =>
                    {
                        var fechaInicial = new DateTime(Globals.ClientSession.i_Periodo ?? DateTime.Now.Year, 1, 1);
                        var fechaFinal = dtpFechaFin.Value.AddHours(23).AddMinutes(59);
                        aptitudeCertificateCu =
                        new AlmacenBL().ReporteStockConsolidadoCajasUnidades(ref objOperationResult, fechaInicial, fechaFinal, almacenes, strOrderExpression, stock, cboLinea.Value.ToString(), pstrEstablecimiento, stock, VCodigoInterno, pstrModelo,int.Parse ( cboEstadoProducto.Value.ToString ()) );
                    }, _cts.Token)
                .ContinueWith(t =>
                {
                    if (_cts.IsCancellationRequested) return;
                    OcultarMostrarBuscar(false);
                    Cursor.Current = Cursors.Default;
                    if (objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte Stock Consolidado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var dt = Utils.ConvertToDatatable(aptitudeCertificateCu);
                    Report2(dt, stock);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void Report2(DataTable dt, int stock)
        {
            var rp = new crReporteStockConsolidadoCajasUnidades();
            var aptitudeCertificate1 = new NodeBL().ReporteEmpresa();
            using (var ds1 = new DataSet())
            {
                var dt2 = Utils.ConvertToDatatable(aptitudeCertificate1);
                ds1.Tables.Add(dt);
                ds1.Tables.Add(dt2);
                ds1.Tables[0].TableName = "dsReporteStockConsolidadoCajasUnidades";
                ds1.Tables[1].TableName = "dsEmpresa";
                rp.SetDataSource(ds1);

                rp.SetParameterValue("MostrarStockCero", stock);
                rp.SetParameterValue("Fecha", string.Format("AL  {0:dd/MM/yyyy}", dtpFechaFin.Value.Date));
                rp.SetParameterValue("Establecimiento", cboEstablecimiento.Text.ToUpper());
                rp.SetParameterValue("NroRegistros", dt.Rows.Count);
                rp.SetParameterValue("NombreEmpresa", Globals.ClientSession.v_CurrentExecutionNodeName);
                rp.SetParameterValue("RucEmpresa","R.U.C. : "+ Globals.ClientSession.v_RucEmpresa);
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
        }        
        
        private void Report1(DataTable dt, int stock)
        {
            var aptitudeCertificate1 = new NodeBL().ReporteEmpresa();
            using (var ds1 = new DataSet())
            {
                var dt2 = Utils.ConvertToDatatable(aptitudeCertificate1);
                ds1.Tables.Add(dt);
                ds1.Tables.Add(dt2);
                ds1.Tables[0].TableName = "dsReporteStockConsolidado";
                ds1.Tables[1].TableName = "dsEmpresa";
                var rp = new crReporteStockConsolidado();
                rp.SetDataSource(ds1);
                rp.SetParameterValue("MostrarStockCero", stock);
                rp.SetParameterValue("Fecha", string.Format("AL {0:dd/MM/yy}", dtpFechaFin.Value.Date));
                rp.SetParameterValue("Establecimiento", cboEstablecimiento.Text.ToUpper());
                rp.SetParameterValue("NroRegistros", dt.Rows.Count);
                rp.SetParameterValue("NombreEmpresa", Globals.ClientSession.v_CurrentExecutionNodeName);
                rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Globals.ClientSession.v_RucEmpresa);
                crystalReportViewer1.ReportSource = rp;
                crystalReportViewer1.Show();
            }
        }

        private void ExportExcel(DataTable dt, IEnumerable<string> filters)
        {
            #region Headers
            if (dt.Rows.Count == 0)
            {
                UltraMessageBox.Show("No se encontro informacion!", "Report To Excel");
                return;
            }
            var cols = new List<string> { "codigoProducto", "descripcionProducto", "unidad", "PrecioMayorista" };
            var heads = new List<ExcelHeader> { "CODIGO", "DESCRIPCION", "U.M.", "Precio Mayorista" };

            foreach (var ctr in filters)
            {
                cols.Add(ctr);
                heads.Add(ctr);
            }
            var r = dt.Rows[0];

            for (byte i = 1; i <= 11; i++)
            {
                var str = r["almacen" + i].ToString();
                if (string.IsNullOrEmpty(str)) continue;
                cols.Add("cantidadAlmacen" + i);
                heads.Add(str);
            }
            cols.Add("cantidad");
            heads.Add("TOTAL");
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
                StartSheet = new Point(0,0)
            };
            
            objexcel.AutoSizeColumns(0, 20, 50);
            objexcel.AutoSizeColumns(2, Enumerable.Range(0, cols.Count - 1).Select(n => 15).ToArray());
            objexcel.SetTitle("CONSOLIDADO DE ALMACENES (" + dtpFechaFin.Value.ToString("dd/MM/yyyy") + ")");
            objexcel.SetHeaders();
            objexcel.SetData( ref objOperationResult ,cols.ToArray());
            if (objOperationResult.Success == 0)
            {
                UltraMessageBox.Show("Ocurrió un error al exportar excel", "Sistema");
                return;
            }
            var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
            objexcel.Generate(path);
            System.Diagnostics.Process.Start(path);
        }

        private void ValidarFechas()
        {
            var periodo = Globals.ClientSession.i_Periodo ?? DateTime.Now.Year;
            dtpFechaFin.MinDate = new DateTime(periodo, 1, 1);
            if (DateTime.Now.Year == periodo)
            {
                dtpFechaFin.MaxDate = DateTime.Now.Date;
                dtpFechaFin.Value = dtpFechaFin.MaxDate;
            }
            else
            {
                dtpFechaFin.MaxDate = new DateTime(periodo, 12, 31);
                dtpFechaFin.Value = new DateTime(periodo, DateTime.Today.Month, DateTime.Today.Day);
            }
        }
        #endregion

        #region Events
        private void cboEstablecimiento_SelectedIndexChanged(object sender, EventArgs e)
        {
            _whereAlmacenesConcatenados = string.Empty;
            _almacenesConcatenados = string.Empty;
            if (cboEstablecimiento.Value == null) return;

            var x = _objEstablecimientoBl.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));
            if (x.Count <= 0) return;
            foreach (var item in x)
            {
                _whereAlmacenesConcatenados = string.Format("IdAlmacen=={0}{1} || ", _whereAlmacenesConcatenados, item.Id);
                _almacenesConcatenados = string.Format("{0}{1}, ", _almacenesConcatenados, item.Value1);
            }
            _whereAlmacenesConcatenados = _whereAlmacenesConcatenados.Substring(0, _whereAlmacenesConcatenados.Length - 4);
            _almacenesConcatenados = string.Format("{0} / {1}", cboEstablecimiento.Text, _almacenesConcatenados.Substring(0, _almacenesConcatenados.Length - 2));
        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvValidar.Validate(true, false).IsValid)
                {
                    int stock = 0, modelo = 0;
                    var filters = new Queue<string>();
                    filters.Enqueue(_whereAlmacenesConcatenados);
                    string strFilter = filters.Count > 0 ? string.Join(" && ",filters) :string.Empty;
                    
                   // if (rdbCajas.Checked) modelo = 1;
                    modelo =cboModelo.Value =="1"?1:0;
                    if (txtCodigoProducto.Text != string.Empty) VCodigoInterno = txtCodigoProducto.Text.Trim();
                    CargarReporte(strFilter, stock, modelo, int.Parse(cboEstablecimiento.Value.ToString()), sender == btnExcel);
                }
            }
            catch (Exception)
            {
                UltraMessageBox.Show("Ocurrió un error al realizar Reporte Stock Consolidado", "Sistema");
            }
        }

        private void txtCodigoProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            using (var frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null))
            {
                frm.ShowDialog();
                if (frm._IdProducto != null)
                {
                    txtCodigoProducto.Text = frm._CodigoInternoProducto.Trim();
                    // txtNombreProducto.Text = frm._NombreProducto.Trim();
                    VCodigoInterno = frm._CodigoInternoProducto.Trim();
                }
                else
                {
                    txtCodigoProducto.Text = string.Empty;
                    // txtNombreProducto.Text = string.Empty;
                    VCodigoInterno = string.Empty;
                }
            }
        }

        private void frmStockConsolidado_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }


        private void ultraExpandableGroupBox1_ExpandedStateChanging(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded == true)
            {
                groupBox2.Location = new Point(groupBox2.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox2.Height = this.Height - groupBox2.Location.Y - 7;
            }
            else
            {
                groupBox2.Location = new Point(groupBox2.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox2.Height = this.Height - groupBox2.Location.Y - 7;
            }
        }
        #endregion

        private void ddbtnOther_Click(object sender, EventArgs e)
        {

        }

        private void uckMarcarTodos_CheckedChanged(object sender, EventArgs e)
        {

            var MarcarTodos = (from Control ctr in pnOpciones.ClientArea.Controls
                           select ctr as Infragistics.Win.UltraWinEditors.UltraCheckEditor into chk
                           where chk != null && chk.Text == "Marcar Todos"
                           select chk).FirstOrDefault();
            var ControlesMarcar = (from Control ctr in pnOpciones.ClientArea.Controls
                            select ctr as Infragistics.Win.UltraWinEditors.UltraCheckEditor into chk
                            where chk != null && chk.Text != "Marcar Todos"
                            select chk).ToList();
            if (MarcarTodos.Checked)
            {
                ControlesMarcar.ForEach(o => o.Checked = true);
            }
            else
            {
                ControlesMarcar.ForEach(o => o.Checked = false);
            }
        }

       

    }
}
