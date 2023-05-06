using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Letras.BL;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Infragistics.Documents.Excel;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Venta.BL;


namespace SAMBHS.Windows.WinClient.UI.Reportes.Letras.LetrasPagar
{
    public partial class frmLetrasPendientesPagos : Form
    {
        #region Fields
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        VendedorBL _objVendedorBL = new VendedorBL();
        #endregion

        #region Init & Load
        public frmLetrasPendientesPagos(string arg)
        {
            InitializeComponent();
        }
        private void frmLetrasPendientesPagos_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            cboAgrupar.SelectedIndex = 0;
            dtpFechaRegistroAl.Value = DateTime.Parse("31/12/" + Globals.ClientSession.i_Periodo);
            dtpFechaRegistroDe.Value = DateTime.Parse("01/01/" + Globals.ClientSession.i_Periodo);
            CargarCombos();
        }
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboEstadoLetra, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 110, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboUbicacionLetra, "Value1", "Id", new DocumentoBL().ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null), DropDownListAction.Select);
            cboEstadoLetra.Value = "-1";
            cboUbicacionLetra.Value = "-1";
        
        }
        #endregion

        #region Methods
        private void OcultarMostrarBuscar(bool estado)
        {
            Text = (estado ? @"Generando... " : "") + @"Letras Pendientes de Pagos";
            pBuscando.Visible = estado;
            BtnVuisualizar.Enabled = btnExcel.Enabled = !estado;
        }
        private void CargarReporte(bool export)
        {

            var filterExpression = string.Empty;
            string clienteFiltro = string.Empty;
            #region Armar la expresion de filtro para la consulta
            if (txtDetalleEspecifico.Tag != null)
            {
                clienteFiltro = txtDetalleEspecifico.Tag.ToString () ; // "v_IdCliente == \"" + txtDetalleEspecifico.Tag + "\"";

                //filterExpression = string.IsNullOrEmpty(filterExpression) ? clienteFiltro : filterExpression + " && " + clienteFiltro;
            }
           
            #endregion

            OperationResult objOperationResult = new OperationResult();

            OcultarMostrarBuscar(true);
            Cursor.Current = Cursors.WaitCursor;
            List<ReporteLetrasPendientesPagoDto> reporte = null;
            string Agrupar = cboAgrupar.Text.Trim();


            Task.Factory.StartNew(() => reporte = new LetrasBL().ReporteLetrasPendientesPago(ref objOperationResult, dtpFechaRegistroDe.Value, dtpFechaRegistroAl.Value, clienteFiltro, Agrupar, cboEstadoLetra.Value.ToString() != "-1" ? cboEstadoLetra.Text : "-1", cboUbicacionLetra.Value.ToString() != "-1" ? cboUbicacionLetra.Text : "-1"), _cts.Token) 
                .ContinueWith(t =>
            {
                if (_cts.IsCancellationRequested) return;
                OcultarMostrarBuscar(false);
                Cursor.Current = Cursors.Default;

                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show("Ocurrió un Error en Letras Pendientes de Pagos" + Environment.NewLine + objOperationResult.ExceptionMessage, "Sistema", Icono:MessageBoxIcon.Error);
                    return;
                }
                var dt = Utils.ConvertToDatatable(reporte);
                if (export)
                {
                    #region Headers
                    var columnas = new[]
                    {
                        "NombreRazonSocial","RucCliente", "Letra", "FechaEmision", "FechaVencimiento", "ImporteSoles","ImporteDolares", 
                        "Estado","UbicacionSoloLetras", "NroUnico","Facturas"
                    };
                    var heads = new ExcelHeader[]
                    {
                        "NOMBRE/RAZON SOCIAL ", "RUC","LETRA", "F. EMISIÓN", "F. VENC", "SOLES", "DÓLARES", "ESTADO","UBICACIÓN", "NRO. UNICO","FACTURAS"
                    };
                    #endregion

                    var excel = new ExcelReport(dt){ Headers = heads};
                    excel.AutoSizeColumns(1, 50,20 ,20, 15, 15, 20, 20, 20, 15, 20,100);
                    excel.SetTitle("RELACIÓN DE LETRAS PENDIENTES DE PAGO");
                    excel.SetHeaders();
                    excel.EndSection += (sender, e) =>
                    {
                        if (e.StartPosition == e.EndPosition) return;
                        var obj = (ExcelReport)sender;
                     
                        obj.SetFormulas(5, "TOTALES: ", string.Format("=SUM(G{0}:G{1})", e.StartPosition + 1, e.EndPosition), string.Format("=SUM(H{0}:H{1})", e.StartPosition + 1, e.EndPosition));
                        obj.CurrentPosition++;
                    };
                    excel.SetData(ref objOperationResult ,columnas);
                    var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                    excel.Generate(path);
                    System.Diagnostics.Process.Start(path);
                }
                else
                {
                    var rp = new crLetrasPendientesPago();
                    var aptitudeCertificate2 = new NodeBL().ReporteEmpresa();
                    var ds1 = new DataSet();
                    dt.TableName = "dsReporteLetrasPorPagar";
                    ds1.Tables.Add(dt);

                    string rangoFechas = string.Format("FECHA : DEL  {0} AL {1}", dtpFechaRegistroDe.Value.Date.ToShortDateString(), dtpFechaRegistroAl.Value.Date.ToShortDateString());

                    rp.SetDataSource(ds1);
                    rp.SetParameterValue("Fecha", rangoFechas);
                    rp.SetParameterValue("MostrarFechaImpresion", chkFechaImpresion.Checked);
                    rp.SetParameterValue("NombreEmpresa", aptitudeCertificate2.First().NombreEmpresaPropietaria);
                    rp.SetParameterValue("RucEmpresa", string.Format("RUC: {0}", aptitudeCertificate2.First().RucEmpresaPropietaria));
                   rp.SetParameterValue("GrupoKey", cboAgrupar.Text);
                    rp.SetParameterValue("NroRegistros", reporte.Count());
                    #region Redimensionando las columas deacuerdo al agrupamiento
                    
                    ReportObject columnaOculta;
                    ReportObject columnaRedimensionada;

                    switch (cboAgrupar.Text)
                    {
                        case "CLIENTE":
                            columnaOculta = rp.ReportDefinition.ReportObjects["NombreRazonSocial1"];
                            columnaRedimensionada = rp.ReportDefinition.ReportObjects["Letra1"];
                            columnaRedimensionada.Width = columnaRedimensionada.Width + columnaOculta.Width;
                            break;

                        case "ESTADO":
                            var header = rp.ReportDefinition.ReportObjects["Text7"];
                            columnaOculta = rp.ReportDefinition.ReportObjects["Estado1"];
                            columnaRedimensionada = rp.ReportDefinition.ReportObjects["UbicacionSoloLetras1"];
                            columnaRedimensionada.Width = columnaRedimensionada.Width + columnaOculta.Width;
                            header.Width = columnaRedimensionada.Width;
                            break;

                        case "UBICACIÓN":
                            columnaOculta = rp.ReportDefinition.ReportObjects["UbicacionSoloLetras1"];
                            columnaRedimensionada = rp.ReportDefinition.ReportObjects["Estado1"];
                            columnaRedimensionada.Width = columnaRedimensionada.Width + columnaOculta.Width;
                            break;
                    }
                    #endregion

                    crystalReportViewer1.ReportSource = rp;
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        #endregion

        #region Events
        private void txtDetalleEspecifico_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmBuscarCliente("V", "");
            f.ShowDialog();
            if (f._RazonSocial == null) return;
            txtDetalleEspecifico.Text = f._RazonSocial;
            txtDetalleEspecifico.Tag = f._IdCliente;
        }

        private void frmLetrasPendientesCobranza_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }

        private void txtDetalleEspecifico_ValueChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDetalleEspecifico.Text.Trim()))
            {
                txtDetalleEspecifico.Tag = null;
            }
        }

        private void BtnVuisualizar_Click(object sender, EventArgs e)
        {
                  CargarReporte(sender == btnExcel);
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
                groupBox2.Location = new Point(groupBox2.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height + 5);
                 groupBox2.Height = Height - groupBox2.Location.Y - 7;
        }
        #endregion
    }
}
