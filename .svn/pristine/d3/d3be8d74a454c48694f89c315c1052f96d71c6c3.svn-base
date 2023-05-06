using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using SAMBHS.Cobranza.BL;
using System.Threading;
using Infragistics.Documents.Excel;
using CrystalDecisions.CrystalReports.Engine;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Cobranza
{
    public partial class frmReportePlanillaCobranza : Form
    {
        #region Fields
        private readonly VendedorBL _objVendedorBl = new VendedorBL();
        private readonly DocumentoBL _objDocumentoBl = new DocumentoBL();
        private readonly DatahierarchyBL _objDatahierarchyBl = new DatahierarchyBL();
        private readonly ClienteBL _objClienteBl = new ClienteBL();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private string _strOrderExpression;
        private List<GridKeyValueDTO> _listadoComboDocumentosCobranza = new List<GridKeyValueDTO>();
        private List<GridKeyValueDTO> _listadoComboDocumentos = new List<GridKeyValueDTO>();
        #endregion

        #region Init
        public frmReportePlanillaCobranza(string parameter)
        {
            InitializeComponent();
        }
        private void frmReportePlanillaCobranza_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            //ValidarFechas();
            CargarCombos();
            uckSoloResumen.Visible = Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya || Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita;
            uckSoloResumen.Checked = Globals.ClientSession.v_RucEmpresa == Constants.RucNotariaBecerrSosaya || Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita;
        }
        #endregion

        #region Methods
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = (estado ? @"Generando... " : string.Empty)
                    + @"Reporte Planilla de Cobranza";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = btnExcel.Enabled = !estado;
        }

        private void CargarReporte(DateTime fechacobranza, DateTime fechacobranzaAl, int? idTipoDocumentoCobranzaDetalle, string idVendedor, string serie, bool excel)
        {
            var objOperationResult = new OperationResult();
            var retonar = new List<string>();
            // ReSharper disable once CollectionNeverQueried.Local
            var retonar2 = new List<string>();
            ReportDocument rp = new ReportDocument();
            if (uckSoloResumen.Checked)
            {
                if (cboAgrupar.Text =="SIN AGRUPAR")
                {
                    rp = new crReportePlanillaCobranzaResumen();
                }
                else
                {
                    rp = new crReportePlanillaCobranzaResumenAgrupado();
                }
            }
            else
            {
                rp = new crReportePlanillaCobranza();
            }
             
            _strOrderExpression = "";
            var listadoGrupos = _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 79, null);
            var seccion = cboAgrupar.Value == null ? "" : cboAgrupar.Value.ToString().ToLower();
            seccion = seccion.Replace("g", "G");
            seccion = seccion.Replace("s", "S");
            seccion = seccion.Replace("h", "H");

            var strGrupollave = "";
            var strNombreGrupollave = "";
            var nombreGrupollave = new List<string>();

            for (var i = 0; i <= listadoGrupos.Count - 1; i++)
            {
                if (cboAgrupar.Value != null && listadoGrupos[i].Value2 != null && listadoGrupos[i].Value3 != null)
                {
                    if (cboAgrupar.Value.ToString().Trim() == listadoGrupos[i].Value2.Trim() && listadoGrupos[i].Value3.Trim() != "")
                    {
                        if (cboAgrupar.Text.Trim() == listadoGrupos[i].Value1)
                        {
                            strNombreGrupollave = listadoGrupos[i].Value1;
                            var splitNombreGrupollave = strNombreGrupollave.Split('/');
                            nombreGrupollave.AddRange(splitNombreGrupollave.Where(s => s.Trim() != ""));
                            strNombreGrupollave = nombreGrupollave[0];
                            // strOrderExpression = _ListadoGrupos[i].Value3.ToString();
                            strGrupollave = listadoGrupos[i].Value3;
                        }
                    }
                    var split = listadoGrupos[i].Value2.Split(',');
                    retonar.AddRange(split.Where(s => s.Trim() != ""));
                }
            }
            retonar = retonar.Distinct().ToList();
            for (var i = 0; i <= retonar.Count - 1; i++)
            {

                retonar2.Add(retonar[0]);
                seccion = retonar[i].ToLower();
                seccion = seccion.Replace("g", "G");
                seccion = seccion.Replace("s", "S");
                seccion = seccion.Replace("h", "H");
                seccion = seccion.Replace("f", "F");
            }

            if (cboAgrupar.Value != null && cboAgrupar.Value.ToString().Trim() != "")
            {

                var split = cboAgrupar.Value.ToString().Split(',');
                foreach (var s in split)
                {
                    if (s.Trim() != "")
                        seccion = s.ToLower();
                    seccion = seccion.Replace("g", "G");
                    seccion = seccion.Replace("s", "S");
                    seccion = seccion.Replace("h", "H");
                    seccion = seccion.Replace("f", "F");
                    rp.ReportDefinition.Sections[seccion].SectionFormat.EnableSuppress = cboAgrupar.Text == @"SIN AGRUPAR";
                }
            }
            _strOrderExpression += _strOrderExpression != "" ? _strOrderExpression != cboOrden.Value.ToString().Trim() ? "," + cboOrden.Value.ToString().Trim() : "" : cboOrden.Value.ToString().Trim();

            var splitGrupollave = strGrupollave.Split(',');
            var grupollave = splitGrupollave.Where(s => s.Trim() != "").ToList();
            if (grupollave.Count > 0)
                strGrupollave = grupollave[0];
          

            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            List<ReportePlanillaCobranza> aptitudeCertificate = null;

            Task.Factory
                .StartNew(() => aptitudeCertificate = new CobranzaBL().ReportePlanillaCobranzas(ref objOperationResult, fechacobranza, fechacobranzaAl, idTipoDocumentoCobranzaDetalle, _strOrderExpression + " ASC", txtCodigo.Text.Trim(), idVendedor, serie, strGrupollave, strNombreGrupollave, txtCorrelativo.Text.Trim(), int.Parse(cboTipoDocumento.Value.ToString()),uckSoloResumen.Checked,int.Parse ( cboUsuarios.Value.ToString ()),int.Parse ( cboEstablecimiento.Value.ToString ())), _cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show(objOperationResult.ExceptionMessage ?? objOperationResult.ErrorMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var dt = Utils.ConvertToDatatable(aptitudeCertificate);
                if (excel)
                {
                    #region Headers
                    var columnas = new[]
                    {
                        "TipoDocumentoCobranza", "NroDocumentoCobranza", "FechaEmision","Vendedor" ,"TipoDocumento", "NroDocumento", "FechaOrigen",
                        "NumeroDocumentoReferenciaCobranzaDetalle","NombreCliente", "Moneda", "TotalFacturado", "MontoCobrar", "MontoPagado", 
                        "MontoPagadoDolares"
                    };
                    var heads = new []
                    {
                        new ExcelHeader
                        {
                            Title = "COBRANZA", Headers = new ExcelHeader[]{"TD", "DOCUMENTO", "FECHA"}
                        }, 
                        new ExcelHeader
                        {
                           Title = "", Headers = new ExcelHeader[]{"VENDEDOR","TD", "DOC.EMISION", "FECHA EMISION", "DOC. REF", "CLIENTE", "", "IMPORTE DOC.", "DEUDA"}
                        },
                        new ExcelHeader
                        {
                            Title = "COBRADO", Headers = new ExcelHeader[]{"SOLES", "DOLARES"}
                        } 
                    };
                    #endregion

                    var objexcel = new ExcelReport(dt) { Headers = heads };
                    objexcel.AutoSizeColumns(1, 5, 20, 15, 25,5, 20, 20, 20, 50, 5, 15, 15, 15, 15);
                    objexcel.SetTitle("PLANILLA DE COBRANZAS");
                    objexcel.SetHeaders();
                    objexcel.EndSection += (sender, e) =>
                    {
                        var obj = (ExcelReport) sender;
                        if (e.StartPosition == e.EndPosition) return;
                        obj.SetFormulas(11, "SUB TOTAL:", Enumerable.Range(0, 2).Select(i => string.Format("=SUM(${2}{0}:${2}{1})", e.StartPosition + 1, e.EndPosition, (char)('M' + i))).ToArray());
                        obj.CurrentPosition++;
                    };
                    objexcel.SetData( ref objOperationResult ,  columnas, "Grupollave", "FormaPago");
                    var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                    objexcel.Generate(path);
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                    var ds1 = new DataSet();
                    var dt2 = Utils.ConvertToDatatable(aptitudeCertificate2);

                    dt.TableName = "dsReportePlanillaCobranza";
                    dt2.TableName = "dsEmpresa";
                    ds1.Tables.Add(dt);
                    ds1.Tables.Add(dt2);
                    var totalGrupollave = cboAgrupar.Text == @"TIPO DE COMPROBANTE CAJA"
                        ? "DOCUMENTO DE COBRANZA : "
                        : cboAgrupar.Text == @"TIPO DE COMPROBANTE VENTA" ? "TIPO DE COMPROBANTE : " : "";
                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("Fecha",
                        "DEL " + fechacobranza.Date.Day.ToString("00") + "/" + fechacobranza.Date.Month.ToString("00") +
                        "/" + fechacobranza.Date.Year.ToString() + " AL " + fechacobranzaAl.Date.Day.ToString("00") +
                        "/" + fechacobranzaAl.Date.Month.ToString("00") + "/" + fechacobranzaAl.Date.Year.ToString());
                        // TextBox con el valor del Parametro);
                    rp.SetParameterValue("CantidadDecimalPrecio", Globals.ClientSession.i_PrecioDecimales);
                    rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count);
                    rp.SetParameterValue("SubTotalGrupo", "SUB-TOTAL " + totalGrupollave);
                    rp.SetParameterValue("Agrupar", strNombreGrupollave);
                    crystalReportViewer1.ReportSource = rp;
                    crystalReportViewer1.Show();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ValidarFechas()
        {
            var periodo = Globals.ClientSession.i_Periodo.ToString();
            var mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString() == periodo)
            {
                dtpFecha.MinDate = DateTime.Parse((periodo + "/01/01"));
                dtpFecha.MaxDate = DateTime.Parse((periodo + "/" + mes + "/" + (DateTime.DaysInMonth(int.Parse(periodo), int.Parse(mes)))));
                dtpFecha.Value = DateTime.Parse((periodo + "/" + mes + "/" + DateTime.Now.Date.Day));
                dtpFechaAl.MinDate = DateTime.Parse((periodo + "/01/01"));
                dtpFechaAl.MaxDate = DateTime.Parse((periodo + "/" + mes + "/" + (DateTime.DaysInMonth(int.Parse(periodo), int.Parse(mes)))));
                dtpFechaAl.Value = DateTime.Parse((periodo + "/" + mes + "/" + DateTime.Now.Date.Day));

            }
            else
            {
                dtpFecha.MinDate = DateTime.Parse((periodo + "/01/01"));
                dtpFecha.MaxDate = DateTime.Parse((periodo + "/12/" + (DateTime.DaysInMonth(int.Parse(periodo), 12))));
                dtpFecha.Value = DateTime.Parse((periodo + "/" + mes + "/" + DateTime.Now.Date.Day));
                dtpFechaAl.MinDate = DateTime.Parse((periodo + "/01/01"));
                dtpFechaAl.MaxDate = DateTime.Parse((periodo + "/12/" + (DateTime.DaysInMonth(int.Parse(periodo), 12))));
                dtpFechaAl.Value = DateTime.Parse((periodo + "/" + mes + "/" + DateTime.Now.Date.Day));
            }
        }

        private void CargarCombos()
        {

            var objOperationResult = new OperationResult();
            _listadoComboDocumentosCobranza = _objDocumentoBl.ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null);
            _listadoComboDocumentos = _objDocumentoBl.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
            _listadoComboDocumentos = _objDocumentoBl.ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null);
            Utils.Windows.LoadUltraComboList(cboComprobante, "Value2", "Id", _listadoComboDocumentosCobranza, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", _objVendedorBl.ObtenerListadoVendedorParaCombo(ref objOperationResult, null), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboAgrupar, "Value1", "Value2", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 79, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 80, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _listadoComboDocumentos, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList (cboUsuarios ,"Value1","Id",_objVendedorBl.ObtenerUsuariosParaCombo (ref objOperationResult,null ),DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            cboUsuarios.Value = "-1";
            cboAgrupar.SelectedIndex = Globals.ClientSession.v_RucEmpresa ==Constants.RucNotariaBecerrSosaya ? 1 : 3;
            cboTipoDocumento.Value = "-1";
            cboOrden.SelectedIndex = 3;
            cboComprobante.Value = "-1";
            cboVendedor.Value = "-1";
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboEstablecimiento.Enabled = Globals.ClientSession.UsuarioEsContable == 1 ? true : false;

            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _listadoComboDocumentos, DropDownListAction.Select);
        }
        #endregion

        #region Events UI
        private void txtCodigo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key != "btnBuscarCliente") return;
            var frm = new Mantenimientos.frmBuscarCliente("VV", txtCodigo.Text.Trim());
            frm.ShowDialog();

            if (frm._IdCliente != null)
            {
                txtCodigo.Text = frm._CodigoCliente.Trim().ToUpper();
                
            }
        }

        private void txtCodigo_Validated(object sender, EventArgs e)
        {

            if (txtCodigo.Text.Trim() != string.Empty)
            {
                var objOperationResult = new OperationResult();
                var cliente = _objClienteBl.ObtenerClienteCodigoBandejasCodigo(ref objOperationResult, txtCodigo.Text.Trim(), "C");
            }
               
        }

        private void cboTipoDocumento_AfterDropDown(object sender, EventArgs e)
        {
            foreach (var row in cboTipoDocumento.Rows)
            {
                if ((string) cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
                var filterRow = true;
                foreach (var column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                        if (row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()))
                        {
                            filterRow = false;
                            break;
                        }
                }
                row.Hidden = filterRow;
            }
        }

        private void cboTipoDocumento_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (var row in cboTipoDocumento.Rows)
            {
                if ((string) cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
                var filterRow = true;
                foreach (var column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                        if (row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()))
                        {
                            filterRow = false;
                            break;
                        }
                }
                row.Hidden = filterRow;
            }
        }

        private void cboTipoDocumento_Leave(object sender, EventArgs e)
        {
            if (cboTipoDocumento.Text.Trim() == "")
                cboTipoDocumento.Value = "-1";
            else
            {
                var x = _listadoComboDocumentos.Find(p => p.Id == cboTipoDocumento.Value.ToString() | p.Id == cboTipoDocumento.Text.Trim());
                if (x == null)
                    cboTipoDocumento.Value = "-1";
            }
        }

        private void cboComprobante_AfterDropDown(object sender, EventArgs e)
        {
            foreach (var row in cboComprobante.Rows)
            {
                if ((string) cboComprobante.Value == "-1") cboComprobante.Text = string.Empty;
                var filterRow = true;
                foreach (var column in cboComprobante.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                        if (row.Cells[column].Text.Contains(cboComprobante.Text.ToUpper()))
                        {
                            filterRow = false;
                            break;
                        }
                }
                row.Hidden = filterRow;
            }
        }

        private void cboComprobante_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (var row in cboComprobante.Rows)
            {
                if ((string) cboComprobante.Value == "-1") cboComprobante.Text = string.Empty;
                var filterRow = true;
                foreach (var column in cboComprobante.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                        if (row.Cells[column].Text.Contains(cboComprobante.Text.ToUpper()))
                        {
                            filterRow = false;
                            break;
                        }
                }
                row.Hidden = filterRow;

            }
        }

        private void cboComprobante_Leave(object sender, EventArgs e)
        {
            if (cboComprobante.Text.Trim() == "")
                cboComprobante.Value = "-1";
            else
            {
                var x = _listadoComboDocumentosCobranza.Find(p => p.Id == cboComprobante.Value.ToString() | p.Id == cboComprobante.Text.Trim());
                if (x == null)
                {
                    cboComprobante.Value = "-1";
                }
            }
        }
        private void dtpFechaAl_ValueChanged(object sender, EventArgs e)
        {
            dtpFecha.MaxDate = dtpFechaAl.Value;
        }

        private void txtSerie_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerie, "{0:0000}");
        }

        private void txtCorrelativo_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativo, "{0:00000000}");
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanging(object sender, CancelEventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded)
            {
                grpReporte.Location = new Point(grpReporte.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                grpReporte.Height = Height - grpReporte.Location.Y - 7;
            }
            else
            {
                grpReporte.Location = new Point(grpReporte.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                grpReporte.Height = Height - grpReporte.Location.Y - 7;
            }
        }

        private void frmReportePlanillaCobranza_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboAgrupar.SelectedIndex == 0)
                {
                    UltraMessageBox.Show("Elija Agrupado por ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cboAgrupar.Focus();
                    return;
                }

                if (uvDatos.Validate(true, false).IsValid)
                {
                    var idTipoDocumentoCobranzaDetalle = int.Parse(cboComprobante.Value.ToString());
                    CargarReporte(dtpFecha.Value.Date, DateTime.Parse(dtpFechaAl.Text + " 23:59"), idTipoDocumentoCobranzaDetalle, cboVendedor.Value.ToString(), txtSerie.Text.Trim(), sender == btnExcel);
                }
                else
                    UltraMessageBox.Show("Por favor llene los campos requeridos para visualizar el reporte", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch
            {
                UltraMessageBox.Show("Se produjo un error  al mostrar el reporte ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}

