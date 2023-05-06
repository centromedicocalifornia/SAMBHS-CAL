#region Name Space
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using SAMBHS.Almacen.BL;
using System.Threading;
#endregion

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmRegistroVentaProductoResumen : Form
    {
        #region Declaraciones / Referencias
        VendedorBL _objVendedorBL = new VendedorBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        LineaBL _objLineaBL = new LineaBL();
        string _whereAlmacenesConcatenados, _AlmacenesConcatenados;
        List<string> Grupollave = new List<string>();
        List<string> NombreGrupollave = new List<string>();
        MarcaBL _objMarcaBL = new MarcaBL();
        public int consideraDocumentosContables = -1;
        public string _Modalidad = "";
        CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        #region Carga de inicializacion
        public frmRegistroVentaProductoResumen(string Modalidad)
        {
            InitializeComponent();
            _Modalidad = Modalidad;
            consideraDocumentosContables = Modalidad == Constants.ModuloContable ? 0 : 1;
        }
        #endregion

        #region Cargar Reporte

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Reporte de Ventas por Producto (Resumen)"
                : @"Reporte de Ventas por Producto (Resumen)";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = btnExcel.Enabled = !estado;
        }
        private void CargarReporte(DateTime FechaRegistroIni, DateTime FechaRegistroFin, int IdMoneda, string FechaHoraImpresion, string IdCliente, string IdProducto, string Orden, int pIntEstablecimiento, string strAlmacenes, string IdLinea, bool export)
        {

            var objOperationResult = new OperationResult();
            List<ReporteRegistroVentaProductoResumen> aptitudeCertificate = null;
            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;

            Task.Factory.StartNew(() =>
            {

                //aptitudeCertificate = new VentaBL().ReporteRegistroVentaProductoResumenII(ref objOperationResult, 0, FechaRegistroIni, FechaRegistroFin, IdMoneda, IdCliente, IdProducto, Orden, pIntEstablecimiento, strAlmacenes, ConsideraDocumentosInternos, cboAgrupar.Text.Trim(), IdLinea, cboMarca.Value.ToString());
                aptitudeCertificate = new VentaBL().ReporteRegistroVentaProductoResumenII(ref objOperationResult, 0, FechaRegistroIni, FechaRegistroFin, IdMoneda, IdCliente, IdProducto, Orden, pIntEstablecimiento, strAlmacenes, int.Parse ( cboTipoDocumento.Value.ToString ()), cboAgrupar.Text.Trim(), IdLinea, cboMarca.Value.ToString());
            
            }, _cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show("Ocurrió un error al realizar Reporte de Ventas por Producto (Resumen)", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var dt = Utils.ConvertToDatatable(aptitudeCertificate);
                if (export)
                {
                    #region Headers
                    if (dt.Rows.Count == 0)
                    {
                        UltraMessageBox.Show("No se encontro informacion!", "Report To Excel");
                        return;
                    }


                    var cols = cboAgrupar.Text == "SIN AGRUPAR" ? cboEstablecimiento.Value == "-1" ? new List<string> { "IdProducto", "NombreProducto", "CantidadDetalle",  "ValorVentaDetalle", "PrecioVentaDetalle", "NombreMoneda" } : new List<string> { "v_Almacen", "IdProducto", "NombreProducto", "CantidadDetalle", "ValorVentaDetalle", "PrecioVentaDetalle", "NombreMoneda" } : cboEstablecimiento.Value == "-1" ? new List<string> { "IdProducto", "NombreProducto", "CantidadDetalle", "ValorVentaDetalle", "PrecioVentaDetalle", "NombreMoneda", "GrupoLLave" } : new List<string> { "v_Almacen", "IdProducto", "NombreProducto", "CantidadDetalle", "ValorVentaDetalle", "PrecioVentaDetalle", "NombreMoneda", "GrupoLLave" };
                    var heads = cboAgrupar.Text == "SIN AGRUPAR" ? cboEstablecimiento.Value == "-1" ? new List<ExcelHeader> { "CODIGO", "DESCRIPCION", "CANTIDAD","VALOR VENTA", "PRECIO VENTA", "MONEDA" } : new List<ExcelHeader> { "ALMACÉN", "CODIGO", "DESCRIPCION", "CANTIDAD", "VALOR VENTA", "PRECIO VENTA", "MONEDA" } : cboEstablecimiento.Value == "-1" ? new List<ExcelHeader> { "CODIGO", "DESCRIPCION", "CANTIDAD", "VALOR VENTA", "PRECIO VENTA", "MONEDA", "AGRUPAMIENTO" } : new List<ExcelHeader> { "ALMACEN", "CODIGO", "DESCRIPCION", "CANTIDAD", "VALOR VENTA", "PRECIO VENTA", "MONEDA", "AGRUPAMIENTO" };

                    #endregion

                    var objexcel = new ExcelReport(dt)
                    {
                        Headers = heads.ToArray(),
                        FormaterHeader = (cell) =>
                        {
                            cell.Fill = Infragistics.Documents.Excel.CellFill.CreateSolidFill(Color.Black);
                            cell.Font.ColorInfo = Color.WhiteSmoke;
                        },
                        StartSheet = new Point(0, 0)
                    };

                    objexcel.AutoSizeColumns(0, 20, 20, 50, 10,10,10,10, 10, 10, 10, 50);
                    objexcel.CurrentPosition = 0;
                    objexcel.SetHeaders();
                    objexcel.SetData(ref objOperationResult ,cols.ToArray());
                    if (objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show("Ocurrió un error al exportar excel", "Sistema");
                        return;

                    }
                    var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                    objexcel.Generate(path);
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                    var dt2 = Utils.ConvertToDatatable(aptitudeCertificate2);
                    dt2.TableName = "dsEmpresa";
                    dt.TableName = "dsRegistroVentaProductoResumen";
                    using (DataSet ds1 = new DataSet())
                    {
                        ds1.Tables.AddRange(new[] { dt, dt2 });
                        var rp = new crRegistroVentaProductoResumen();
                        rp.SetDataSource(ds1);
                        rp.SetParameterValue("FechaHoraImpresion", FechaHoraImpresion);
                        rp.SetParameterValue("IdMoneda", IdMoneda);
                        rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                        rp.SetParameterValue("Cliente", txtCodigo.Text.Trim() == string.Empty ? 0 : 1);
                        rp.SetParameterValue("Establecimiento", cboEstablecimiento.Text.Trim().ToUpper());
                        rp.SetParameterValue("Fecha", "DEL  " + dtpFechaRegistroDe.Text + " AL " + dtpFechaRegistroAl.Text);
                        rp.SetParameterValue("Agrupar", cboAgrupar.Text);
                        crystalReportViewer1.ReportSource = rp;
                        crystalReportViewer1.Show();
                    }
                }
            }
                , TaskScheduler.FromCurrentSynchronizationContext());

        }
        #endregion

        #region Controles Botones
        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", "");
            frm.ShowDialog();

            if (frm._IdCliente != null)
            {
                // TxtCliente.Text = frm._RazonSocial.Trim ();
                txtCodigo.Text = frm._CodigoCliente.Trim();
            }
        }
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            string _strFilterExpression = string.Empty;
            try
            {
                if (uvDatos.Validate(true, false).IsValid)
                {
                    string IdLinea = string.Empty;
                    var Filters = new List<string>();
                    var ValorIdCliente = txtCodigo.Text.Trim() == "" ? "" : txtCodigo.Text.Trim();
                    var ValorIdProducto = txtProducto.Text.Trim() == "" ? "" : txtProducto.Text.Trim();
                    IdLinea = cboLinea.Value.ToString();
                    var moneda = int.Parse(cboMoneda.Value.ToString());
                    var strOrderExpression = cboOrden.Value.ToString() == "-1" ? "" : cboOrden.Value.ToString();
                    if (cboAlmacen.Value.ToString() != "-1")
                    {
                        Filters.Add("pintAlmacen==" + cboAlmacen.Value);
                    }
                    else
                    {
                        Filters.Add(_whereAlmacenesConcatenados);
                    }
                    _strFilterExpression = Filters.Count > 0 ? string.Join(" && ", Filters) : null;
                    CargarReporte(DateTime.Parse(dtpFechaRegistroDe.Text), DateTime.Parse(dtpFechaRegistroAl.Text + " 23:59"), moneda, chkHoraimpresion.Checked ? "1" : "0", ValorIdCliente, ValorIdProducto, strOrderExpression, int.Parse(cboEstablecimiento.Value.ToString()), _strFilterExpression, IdLinea, btnExcel == sender);

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
        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            var frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
            frm.ShowDialog();

            txtProducto.Text = frm._IdProducto != null ? frm._CodigoInternoProducto.Trim() : string.Empty;

        }
        #endregion

        #region Cargar Load
        private void frmRegistroVentaProductoResumen_Load(object sender, EventArgs e)
        {

            this.BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            if (_Modalidad == Constants.ModuloContable)
            {
                consideraDocumentosContables = 1;
                cboTipoDocumento.Value = consideraDocumentosContables == 1 ? "1" : "-1";
            }

            else consideraDocumentosContables = int.Parse(cboTipoDocumento.Value.ToString());
            cboTipoDocumento.Visible = consideraDocumentosContables != 1;
            lblTipoDocumento.Visible = consideraDocumentosContables != 1;



        }
        #endregion

        private void CargarCombos()
        {

            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            List<KeyValueDTO> ListaDetallados = new List<KeyValueDTO>();
            KeyValueDTO objDetallado = new KeyValueDTO();
            objDetallado.Id = "99";
            objDetallado.Value1 = "Detallados";
            objDetallado.Value2 = "Detallados";
            ListaDetallados.Add(objDetallado);
            var Establecimiento = Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita ? objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null).Concat(ListaDetallados).ToList() : objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", Establecimiento, DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", _objLineaBL.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMarca, "Value1", "Id", _objMarcaBL.LlenarComboMarca(ref objOperationResult, "v_CodMarca", "-1"), DropDownListAction.All);

            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            cboLinea.Value = "-1";
            cboMarca.Value = "-1";
            this.cboAgrupar.Items.Add("-1", "--Seleccionar--");
            this.cboAgrupar.Items.Add("1", "SIN AGRUPAR");
            this.cboAgrupar.Items.Add("2", "LINEA");

            if (Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita)
            {

                this.cboAgrupar.Items.Add("3", "MARCA");
                this.cboAgrupar.Items.Add("4", "MODELO");
                this.cboAgrupar.Items.Add("5", "TEMPORADA");
                this.cboAgrupar.Items.Add("6", "COLECCIÓN");
                this.cboAgrupar.Items.Add("7", "LINEA ,MARCA , MODELO");
                this.cboAgrupar.Items.Add("8", "LINEA ,MARCA , MODELO , TALLA");
                this.cboAgrupar.Items.Add("9", "MODELO Y TALLA");
                this.cboAgrupar.Items.Add("10", "MODELO , TALLA Y COLOR");


            }
            cboOrden.Value = "IdProducto";
            cboAgrupar.Value = "1";
            cboAlmacen.Value = "-1";
            cboTipoDocumento.Value = "-1";
        
        }
        private void cboEstablecimiento_SelectedIndexChanged(object sender, EventArgs e)
        {
            _whereAlmacenesConcatenados = string.Empty;
            _AlmacenesConcatenados = string.Empty;
            var x = new List<KeyValueDTO>();
            if (cboEstablecimiento.Value == null) return;

            x = objEstablecimientoBL.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));

            if (x.Count > 0)
            {
                foreach (var item in x)
                {
                    _whereAlmacenesConcatenados = _whereAlmacenesConcatenados + "pintAlmacen==" + item.Id + " || ";
                    _AlmacenesConcatenados = _AlmacenesConcatenados + item.Value1 + ", ";
                }
                _whereAlmacenesConcatenados = _whereAlmacenesConcatenados.Substring(0, _whereAlmacenesConcatenados.Length - 4);
                _AlmacenesConcatenados = cboEstablecimiento.Text + " / " + _AlmacenesConcatenados.Substring(0, _AlmacenesConcatenados.Length - 2);
            }
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.All);
            cboAlmacen.Value = "-1";
        }

        private void frmRegistroVentaProductoResumen_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void txtProducto_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarArticulo")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "REPORTE", null, null);
                frm.ShowDialog();

                if (frm._IdProducto != null)
                {

                    txtProducto.Text = frm._CodigoInternoProducto.Trim();
                }
                else
                {
                    txtProducto.Text = string.Empty;
                }
            }
        }

        private void txtCodigo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtCodigo.Text.Trim());
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {

                txtCodigo.Text = frm._CodigoCliente;
            }
            
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanging(object sender, System.ComponentModel.CancelEventArgs e)
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

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }
    }
}
