using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Almacen
{
    public partial class frmCruceInventario : Form
    {
        #region Fields
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        #endregion

        #region Initialize
        public frmCruceInventario(string arg)
        {
            InitializeComponent();
            Load += frmCruceInventario_Load;
        }

        private void frmCruceInventario_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            var objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null).OrderBy(r => r.Id).ToList(), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboLinea, "Value1", "Id", new LineaBL().LlenarComboLinea(ref objOperationResult, null), DropDownListAction.All );
            cboEstablecimiento.Value = (Globals.ClientSession.i_IdEstablecimiento ?? 1).ToString();
            cboLinea.Value = "-1";
            dtpFechaInicio.MaxDate = dtpFechaInicio.DateTime;
            dtpFechaInicio.DateTime = Convert.ToDateTime("01/01/" + (Globals.ClientSession.i_Periodo ?? DateTime.Now.Year) + " 00:00");
        }
        #endregion

        #region Events
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (!uvFiltro.Validate().IsValid) return;
            LoadReporte(sender == btnExcel);
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.DateTime;
            if (dtpFechaInicio.DateTime > dtpFechaInicio.MaxDate)
                dtpFechaInicio.DateTime = dtpFechaInicio.MaxDate;
        }

        private void cboEstablecimiento_ValueChanged(object sender, EventArgs e)
        {
            if (cboEstablecimiento.Value == null) return;
            var x = new EstablecimientoBL().GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.Select);
            cboAlmacen.Value = "-1";
        }

        private void frmCruceInventario_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_cts.Token.CanBeCanceled) _cts.Cancel();
        }
        #endregion

        #region Methods
        private void OcultarMostrarBuscar(bool estado)
        {
            pBuscando.Visible = estado;
            btnBuscar.Enabled = btnExcel.Enabled = !estado;
        }

        private void LoadReporte(bool export)
        {
            try
            {
                #region Args
                DateTime fecha1 = dtpFechaInicio.DateTime.Date;
                DateTime fecha2 = Convert.ToDateTime(dtpFechaFin.DateTime.ToString("dd/MM/yyyy") + " 23:59");

                #endregion

                var objOperationResult = new OperationResult();
                OcultarMostrarBuscar(true);
                List<KardexList> aptitudeCertificate = null;
                Task.Factory.StartNew(() =>
                    {
                    var line= cboLinea.Value.ToString();
                    if (line == "-1") line = null;
                    aptitudeCertificate = new AlmacenBL().ReporteCruceInventario(ref objOperationResult, fecha1, fecha2, line,
                        int.Parse(cboEstablecimiento.Value.ToString()),int.Parse(cboAlmacen.Value.ToString()));
                }, _cts.Token)
                .ContinueWith(t =>
                {
                    if (_cts.IsCancellationRequested) return;
                    OcultarMostrarBuscar(false);
                    if (objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show("Ocurrió un error al realizar Reporte Stock Consolidado.\n Información Adicional :" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var dt = Utils.ConvertToDatatable(aptitudeCertificate);
                    if (export)
                    {
                        var cols = new[]{
                            "CodProducto", "v_NombreProducto", "Saldo_Cantidad", "StockFisico"
                        };
                        var heads = new ExcelHeader[]{
                            "CODIGO", "DESCRIPCION", "CANTIDAD LOGICA", "CANTIDAD FISICA", "DIFERENCIA"
                        };
                        var xlxsReport = new ExcelReport(dt, "CRUCE INVENTARIO")
                        {
                            StartSheet = new Point(0,0),
                            Headers = heads
                        };
                        xlxsReport.AutoSizeColumns(0, 15, 40, 20, 20, 20);
                        xlxsReport.SetTitle("CRUCE DE INVENTARIO");
                        xlxsReport.SetHeaders();
                        xlxsReport.EndSection += xlxsReport_EndSection;
                        xlxsReport.SetData(ref objOperationResult,cols);
                        var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
                        xlxsReport.Generate(path);
                        System.Diagnostics.Process.Start(path);
                    }
                    else
                    {  
                        dt.TableName = "dtKardex";
                        using (var ds1 = new DataSet())
                        {
                            ds1.Tables.Add(dt);
                            var rp = new crCruceInventario();
                            rp.SetDataSource(ds1);
                            rp.SetParameterValue("Fecha", "AL  " + dtpFechaInicio.DateTime.ToString("dd/MM/yyyy") + "/" + dtpFechaFin.DateTime.ToString("dd/MM/yyyy"));
                            rp.SetParameterValue("Establecimiento", "ESTABLECIMIENTO : " + cboEstablecimiento.Text.Trim().ToUpper());
                            rp.SetParameterValue("DecimalesCantidad", Globals.ClientSession.i_CantidadDecimales);
                            rp.SetParameterValue("NroRegistros", aptitudeCertificate.Count());
                            rp.SetParameterValue("NombreEmpresa", Globals.ClientSession.v_CurrentExecutionNodeName);
                            rp.SetParameterValue("RucEmpresa", "R.U.C. : " + Globals.ClientSession.v_RucEmpresa);
                            rp.SetParameterValue("AlmacenElegido", int.Parse(cboAlmacen.Value.ToString()));
                            rp.SetParameterValue("MonedaReporte", "MONEDA : SOLES");
                            crystalReportViewer1.ReportSource = rp;
                            crystalReportViewer1.Show(); 
                        }
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception er)
            {
                UltraMessageBox.Show(er.Message, "Error", Icono: MessageBoxIcon.Error);
            }
        }

        private void xlxsReport_EndSection(object sender, ExcelReportSectionEventArgs e)
        {
            if (e.StartPosition == e.EndPosition) return;
            for (int i = e.StartPosition; i < e.EndPosition; i++)
            {
                e.Wsheet.Rows[i].Cells[4].ApplyFormula(string.Format("=$D{0}-$C{0}", i+1));
            }
        }
        #endregion

    }
}
