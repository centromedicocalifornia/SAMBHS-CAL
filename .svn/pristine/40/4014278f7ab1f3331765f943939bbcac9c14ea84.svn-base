using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using SAMBHS.Cobranza.BL;
using CrystalDecisions.CrystalReports.Engine;
using System.Threading;
using System.IO;
using System.Drawing;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Cobranza
{
    public partial class frmReporteCuentasPorCobrar : Form
    {
        #region Fields
        private readonly VendedorBL _objVendedorBl = new VendedorBL();
        private readonly DocumentoBL _objDocumentoBl = new DocumentoBL();
        private string _strFilterExpression;
        private readonly DatahierarchyBL _objDatahierarchyBl = new DatahierarchyBL();
        private string _strOrderExpression, _strGrupollave, _strGrupollave2;
        private List<GridKeyValueDTO> _listadoComboDocumentos = new List<GridKeyValueDTO>();
        private readonly ClienteBL _objClienteBl = new ClienteBL();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        #region Init & Load
        public frmReporteCuentasPorCobrar(string parameter)
        {
            InitializeComponent();
        }
        private void frmReporteCuentasPorCobrar_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            //ValidarFechas();
            CargarCombos();
            chkIncluirLetrasCambio.Checked = Globals.ClientSession.v_RucEmpresa != Constants.RucRollavel && Globals.ClientSession.v_RucEmpresa != Constants.RucJorplast;
            uckConsiderarRangoFechas.Checked = false;
            object senderFechas = new object ();
            EventArgs eFechas= new EventArgs ();
            uckConsiderarRangoFechas_CheckedChanged(senderFechas, eFechas);
        }
        #endregion

        #region Methods
        private void CargarReporte(DateTime fechacobranza, DateTime fechacobranzaAl, string filtro, bool export)
        {
            var objOperationResult = new OperationResult();
            var retonar = new List<string>();
            var retonar2 = new List<string>();
            ReportDocument rp;
            if (chkModelo2.Checked)
            {
                rp = new crReporteCuentasPorCobrar2();
            }
            else
            {
                rp = new crReporteCuentasPorCobrar1();
                
            }
            _strOrderExpression = "";
            var listadoGrupos = _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 82, null);
            var seccion = cboAgrupar.Value == null ? "" : cboAgrupar.Value.ToString().ToLower();

            seccion = seccion.Replace("g", "G");
            seccion = seccion.Replace("s", "S");
            seccion = seccion.Replace("h", "H");

            _strGrupollave = "";
            var strNombreGrupollave = "";
            _strGrupollave2 = null;
            var nombreGrupollave = new List<string>();

            for (var i = 0; i <= listadoGrupos.Count - 1; i++)
            {
                if (cboAgrupar.Value == null || listadoGrupos[i].Value2 == null || listadoGrupos[i].Value3 == null)
                    continue;
                if (cboAgrupar.Value.ToString().Trim() == listadoGrupos[i].Value2.Trim() && listadoGrupos[i].Value3.Trim() != "")
                {
                    if (cboAgrupar.Text.Trim() == listadoGrupos[i].Value1)
                    {
                        strNombreGrupollave = listadoGrupos[i].Value1;
                        var splitNombreGrupollave = strNombreGrupollave.Split('/');
                        nombreGrupollave.AddRange(splitNombreGrupollave.Where(s => s.Trim() != ""));

                        strNombreGrupollave = nombreGrupollave[0];
                        _strOrderExpression = listadoGrupos[i].Value3;
                        _strGrupollave = listadoGrupos[i].Value3;
                    }
                }
                var split = listadoGrupos[i].Value2.Split(',');
                retonar.AddRange(split.Where(s => s.Trim() != ""));
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
                foreach (var s in cboAgrupar.Value.ToString().Split(','))
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

            var splitGrupollave = _strGrupollave.Split(',');
            var grupollave = splitGrupollave.Where(s => s.Trim() != "").ToList();
            if (grupollave.Count > 0)
            {
                if (grupollave.Count == 2)
                {
                    _strGrupollave = grupollave[0];
                    _strGrupollave2 = grupollave[1];

                }
                else
                {
                    _strGrupollave = grupollave[0];
                }
            }

            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            var incluirLetrasCambio = chkIncluirLetrasCambio.Checked;
            List<ReporteCuentasPorCobrar> aptitudeCertificate = null;

            Task.Factory.StartNew(() =>
            {
                var cobranzaBl = new CobranzaBL();
                aptitudeCertificate = chkModelo2.Checked ? cobranzaBl.ReporteCuentaspoCobrar2(ref objOperationResult, fechacobranza, fechacobranzaAl, _strOrderExpression + " ASC", _strGrupollave, strNombreGrupollave, filtro) : cobranzaBl.ReporteCuentaspoCobrar(ref objOperationResult, fechacobranza, fechacobranzaAl, _strOrderExpression + " ASC", _strGrupollave, strNombreGrupollave, filtro, incluirLetrasCambio, uckConsiderarRangoFechas.Checked, int.Parse ( cboEstablecimiento.Value.ToString ()),int.Parse (cboCondicionPago.Value.ToString ()));
            }, _cts.Token)
            .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;
                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show("Ocurrió un Error al realizar Reporte de Cuentas por Cobrar", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var dt = Utils.ConvertToDatatable(aptitudeCertificate);

                if (export)
                {
                    #region Headers

                    var columnas = new[]
                    {
                        "NombreCliente", "NroDocCliente", "MedioPago", "FechaEmision", "TipoDocumento", "GuiaRemision",
                        "FechaVencimiento","DiasFaltantesVencimiento", "Vendedor", "Moneda", "TotalFacturado", "Acuenta",
                        "Saldo", "SaldoDolares","Departamento","Provincia","Distrito"
                    };
                    var heads = new []
                    {
                        new ExcelHeader{
                            Title = "DETALLE", 
                            Headers = new ExcelHeader[]
                            {
                                "CLIENTE", "N° DOC CLIENTE", "COND. PAGO", "FECHA EMISION", "DOCUMENTO", "GUIA REM", "FECHA VENC.","DIAS FALTANTES",
                                "VENDEDOR", "M", "TOTAL DEUDA", "A CUENTA"
                            }
                        },
                        new ExcelHeader
                        {
                            Title = "DEUDA", Headers = new ExcelHeader[]{"SOLES", "DOLARES"}
                        }
                        ,
                        new ExcelHeader 
                        {
                            Title = "UBIGEO CLIENTE", Headers = new ExcelHeader[]{"DEPARTAMENTO", "PROVINCIA","DISTRITO"}
                        }
                    };
                    #endregion

                    var objexcel = new ExcelReport(dt) {Headers = heads};
                    objexcel.AutoSizeColumns(1, 50, 20, 15, 12, 20, 20, 12,15, 15, 6, 15, 15, 15, 15,30,30,30);
                    objexcel.SetTitle("CUENTAS POR COBRAR (CLIENTES)");
                    objexcel.SetHeaders();
                    objexcel.EndSection += (sender, e) =>
                    {
                        if (e.StartPosition == e.EndPosition) return;
                        ((ExcelReport)sender).SetFormulas(10, "SUB TOTAL:",
                            Enumerable.Range(0, 3)
                            .Select(i => string.Format("=SUM(${2}{0}:${2}{1})", e.StartPosition + 1, e.EndPosition, (char)('L' + i)))
                            .ToArray());
                        ((ExcelReport) sender).CurrentPosition++;
                    };
                    var filters = new Queue<string>();
                    if (cboAgrupar.Text != @"SIN AGRUPAR") filters.Enqueue("Grupollave");             
                    objexcel.SetData(  ref objOperationResult ,columnas, filters.ToArray());
                    var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                    objexcel.Generate(path);
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                    var ds1 = new DataSet();
                    var dt2 = Utils.ConvertToDatatable(aptitudeCertificate2);
                    dt.TableName = "dsReporteCuentasPorCobrar";
                    dt2.TableName = "dsEmpresa";
                    ds1.Tables.Add(dt);
                    ds1.Tables.Add(dt2);

                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("Fecha",
                        "DEL " + fechacobranza.Date.Day.ToString("00") + "/" + fechacobranza.Date.Month.ToString("00") +
                        "/" + fechacobranza.Date.Year.ToString() + " AL " + fechacobranzaAl.Date.Day.ToString("00") +
                        "/" + fechacobranzaAl.Date.Month.ToString("00") + "/" + fechacobranzaAl.Date.Year.ToString());
                    rp.SetParameterValue("CantidadDecimalPrecio", Globals.ClientSession.i_PrecioDecimales ?? 2);
                    rp.SetParameterValue("RangoFecha", uckConsiderarRangoFechas.Checked ? true : false);
                    rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count);
                    rp.SetParameterValue("Agrupado", cboAgrupar.Value == null ? "" : cboAgrupar.Value.ToString());
                    rp.SetParameterValue("KeyGrupo", cboAgrupar.Text.Trim());
                    rp.SetParameterValue("NombreEmpresa",
                        aptitudeCertificate2.First().NombreEmpresaPropietaria.Trim());
                    rp.SetParameterValue("RucEmpresa",
                        "R.U.C. : " + aptitudeCertificate2.First().RucEmpresaPropietaria.Trim());

                    #region Redimensionando las columas deacuerdo al agrupamiento

                    ReportObject columnaOculta;
                    ReportObject columnaRedimensionada;
                    switch (cboAgrupar.Text)
                    {
                        case "CLIENTE":
                            var header = rp.ReportDefinition.ReportObjects["Text10"];
                            columnaOculta = rp.ReportDefinition.ReportObjects["NombreCliente1"];
                            columnaRedimensionada = rp.ReportDefinition.ReportObjects["Correlativo1"];
                            columnaRedimensionada.Width = columnaRedimensionada.Width + columnaOculta.Width;
                            header.Width = columnaRedimensionada.Width;
                            break;

                        case "COND. PAGO":
                            var header1 = rp.ReportDefinition.ReportObjects["Text9"];
                            columnaOculta = rp.ReportDefinition.ReportObjects["MedioPago1"];
                            columnaRedimensionada = rp.ReportDefinition.ReportObjects["NombreCliente1"];
                            columnaRedimensionada.Width = columnaRedimensionada.Width + columnaOculta.Width;
                            header1.Width = columnaRedimensionada.Width;
                            break;

                        case "VENDEDOR":
                            //var Header2 = rp.ReportDefinition.ReportObjects["Text20"];
                            //ColumnaOculta = rp.ReportDefinition.ReportObjects["Vendedor1"];
                            //ColumnaRedimensionada = rp.ReportDefinition.ReportObjects["Moneda1"];
                            //ColumnaRedimensionada.Width = ColumnaRedimensionada.Width + ColumnaOculta.Width;
                            //Header2.Width = ColumnaRedimensionada.Width;
                            break;
                    }
                    #endregion

                    crystalReportViewer1.ReportSource = rp;
                    crystalReportViewer1.Show();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = (estado ? @"Generando... " : "" ) + @"Reporte de Cuentas por Cobrar";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = btnExcel.Enabled = !estado;
        }

        private void CargarCombos()
        {
            var objOperationResult = new OperationResult();
            _listadoComboDocumentos = _objDocumentoBl.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _listadoComboDocumentos, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", _objVendedorBl.ObtenerListadoVendedorParaCombo(ref objOperationResult, null), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboAgrupar, "Value1", "Value2", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 82, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboOrden, "Value1", "Value2", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 81, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboCondicionPago, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 23, null), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL ().ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.All);
            cboAgrupar.SelectedIndex = Globals.ClientSession.v_RucEmpresa == Constants.RucRollavel || Globals.ClientSession.v_RucEmpresa == Constants.RucJorplast ? 3 : 1;
            cboTipoDocumento.Value = "-1";
            cboOrden.SelectedIndex = 3;
            cboVendedor.Value = "-1";
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.Value.ToString();
            cboCondicionPago.Value = "-1";
        }

        private void ValidarFechas()
        {
            var periodo = Globals.ClientSession.i_Periodo.ToString();
            var mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == periodo)
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

            dtpFecha.Value = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo);
        }
        #endregion

        #region Events UI
        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (uvDatos.Validate(true, false).IsValid)
                {
                    if (cboAgrupar.SelectedIndex == 0)
                    {
                        UltraMessageBox.Show("Elija Agrupado por ", "SISTEMA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cboAgrupar.Focus();
                        return;
                    }
                    var filters = new Queue<string>();
                    if (txtCliente.Text != string.Empty) filters.Enqueue("v_CodigoCliente==\"" + txtCliente.Text.Trim() + "\"");
                    if (cboVendedor.Value.ToString() != "-1") filters.Enqueue("v_IdVendedor==\"" + cboVendedor.Value + "\"");
                    if (cboTipoDocumento.Value.ToString() != "-1") filters.Enqueue("(" + "idTipoDocumento==" + cboTipoDocumento.Value + ")");
                    if (txtSerie.Text.Trim() != string.Empty) filters.Enqueue("v_SerieDocumento==\"" + txtSerie.Text.Trim() + "\"");
                    if (txtCorrelativo.Text.Trim() != string.Empty) filters.Enqueue("v_CorrelativoDocumento==\"" + txtCorrelativo.Text.Trim() + "\"");
                    _strFilterExpression = filters.Count > 0 ? string.Join(" && ", filters)  : null;
                    CargarReporte(dtpFecha.Value.Date, DateTime.Parse(dtpFechaAl.Text + " 23:59"), _strFilterExpression, sender == btnExcel);
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

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key != "btnBuscarCliente") return;
            var frm = new Mantenimientos.frmBuscarCliente("VV", txtCliente.Text.Trim());
            frm.ShowDialog();

            if (frm._IdCliente != null)
            {
                txtCliente.Text = frm._CodigoCliente.Trim().ToUpper();
              //  txtRazonSocial.Text = frm._RazonSocial.Trim().ToUpper();
            }
        }

        private void txtCliente_Validated(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            if (txtCliente.Text.Trim() != string.Empty)
            {
                var client = _objClienteBl.ObtenerClienteCodigoBandejasCodigo(ref objOperationResult, txtCliente.Text.Trim(), "C");
                //txtRazonSocial.Text = client != null ? string.Join(" ", client.v_ApePaterno, client.v_ApeMaterno.Trim(), client.v_PrimerNombre.Trim(), client.v_SegundoNombre.Trim(), client.v_RazonSocial.Trim()).ToUpper() : string.Empty;
            }
            else
            {
                //txtRazonSocial.Clear();
            }
        }

        private void dtpFechaAl_ValueChanged(object sender, EventArgs e)
        {
            dtpFecha.MaxDate = dtpFechaAl.Value;
        }

        private void cboTipoDocumento_AfterDropDown(object sender, EventArgs e)
        {
            foreach (var row in cboTipoDocumento.Rows)
            {
                if ((string) cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
                var filterRow = true;
                foreach (var column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (!column.IsVisibleInLayout) continue;
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
                    if (!column.IsVisibleInLayout) continue;
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
            {
                cboTipoDocumento.Value = "-1";
            }
            else
            {
                var x = _listadoComboDocumentos.Find(p => p.Id == cboTipoDocumento.Value.ToString() | p.Id == cboTipoDocumento.Text.Trim());
                if (x == null)
                {
                    cboTipoDocumento.Value = "-1";
                }
            }
        }

        private void txtSerie_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerie, "{0:0000}");
        }

        private void txtCorrelativo_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativo, "{0:00000000}");
        }

        private void frmReporteCuentasPorCobrar_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void uckConsiderarRangoFechas_CheckedChanged(object sender, EventArgs e)
        {
            dtpFecha.Enabled = uckConsiderarRangoFechas.Checked;
            dtpFechaAl.Enabled = uckConsiderarRangoFechas.Checked;
        }
        #endregion

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded == true)
            {
                groupBox4.Location = new Point(groupBox4.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox4.Height = this.Height - groupBox4.Location.Y - 7;
            }
            else
            {
                groupBox4.Location = new Point(groupBox4.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                groupBox4.Height = this.Height - groupBox4.Location.Y - 7;
            }
        }

    }
}
