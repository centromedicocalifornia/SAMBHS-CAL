using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Venta.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmMonitoreoVentas : Form
    {
        #region Fields
        EstablecimientoBL _objEstablecimientoBL = new EstablecimientoBL();
        VendedorBL _objVendedorBL = new VendedorBL();
        VentaBL _objVentaBL = new VentaBL();
        string _strFilterExpression = "", pstrIdVenta;
        #endregion

        public frmMonitoreoVentas(string p)
        {
            InitializeComponent();
        }

        private void frmMonitoreoVentas_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;      
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboList(cboEstablecimiento, "Value2", "Id", _objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null, true), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", _objVendedorBL.ObtenerListadoVendedorParaCombo(ref objOperationResult, null), DropDownListAction.All);
            dtpFechaInicio.Value = DateTime.Today;
            cboEstablecimiento.Value = "-1";
            cboVendedor.Value = "-1";

            #region Events Combos
            cbOpcionesGraph.SelectedIndex = 0;
            cboTipo1.SelectedIndex = 0; // Almacen
            cboTipo2.SelectedIndex = 3; // Fecha
            cboFiltroFecha.SelectedIndex = 2; // Dia
            cboMoneda.SelectedIndex = 0;// Soles
            cboFormula.SelectedIndex = 0;
            cbOpcionesGraph.SelectionChanged += cbOpcionesGraph_SelectionChanged;
            cboTipo1.SelectionChanged += cboTipo_SelectionChanged;
            cboTipo2.SelectionChanged += cboTipo2_SelectionChanged;
            cboFiltroFecha.SelectionChanged += cboFiltroFechaAndMoneda_SelectionChanged;
            cboMoneda.SelectionChanged += cboFiltroFechaAndMoneda_SelectionChanged;
            cboFormula.SelectionChanged += cboFiltroFechaAndMoneda_SelectionChanged;
            #endregion

            #region Total Summary
            SummarySettings summarySettings1 = grdData.DisplayLayout.Bands[0].Summaries.Add(
                "SumaTotal",
                SummaryType.Custom,
                new TotalesCustom(),
                grdData.DisplayLayout.Bands[0].Columns["Total"],
                SummaryPosition.Right,
                null
                );
            summarySettings1.DisplayFormat = "Total: {0}";
            summarySettings1.SummaryDisplayArea = SummaryDisplayAreas.BottomFixed | SummaryDisplayAreas.GroupByRowsFooter;
            grdData.DisplayLayout.Bands[0].Summaries["group"].CustomSummaryCalculator = new TotalesCustom();
            #endregion
        }

        #region Methods
        void CargarDatos()
        { 
            OperationResult objOperationResult = new OperationResult();
            var objs = _objVentaBL.ListarMonitoreoVentas(ref objOperationResult, dtpFechaInicio.DateTime, DateTime.Parse(dtpFechaFin.DateTime.ToShortDateString() + " 23:59"), _strFilterExpression);
            foreach (var dto in objs)
            {
                if (dto.Documento == "NCR" || dto.Documento == "NDB")
                    dto.Total *= -1;
            }
            grdData.DataSource = objs;
            ReadyView = false;
            Tbs.Tabs["grafico"].Visible = grdData.Rows.Any();
            if (Tbs.Tabs["grafico"].Selected) this.ViewGraph();
            grdData.DisplayLayout.Bands[0].SortedColumns.Add("Almacen", false, true);
        }
        private Func<UltraGridRow, double> FuncConvert;
        private void SetData()
        {
            var columnCalc = "Total";
            #region Funcion Converter
            if (FuncConvert == null) FuncConvert = r =>
            {
                var num = decimal.Parse(r.Cells[columnCalc].Text);
                var row = (r.ListObject as MonitoreoVentasDto);
                if (row.Moneda != (string)cboMoneda.SelectedItem.DataValue)
                {
                    if (((string)cboMoneda.SelectedItem.DataValue).Equals("D"))
                        num /= row.TipoCambio;
                    else
                        num *= row.TipoCambio;
                }
                return (double)num;
            };
            #endregion

            List<NumericSeries> series = new List<NumericSeries>();
            var group1 = (from g1 in grdData.Rows
                            group g1 by g1.Cells[(string)cboTipo1.SelectedItem.DataValue].Text);
            foreach (var i1 in group1)
            {
                var serie = new NumericSeries();
                //serie.Key = item.Key;
                serie.Label = i1.Key;
                if (cboTipo2.SelectedIndex > 0)
                {
                    var group2 = (from g2 in i1
                                  group g2 by getTextGroup(g2.Cells[(string)cboTipo2.SelectedItem.DataValue]) into groups
                                  orderby groups.Key ascending
                                  select groups);
                    var points = new List<NumericDataPoint>(group2.Count());
                    foreach (var i2 in group2)
                        points.Add(getDataPoint(i2));
                    serie.Points.AddRange(points.ToArray());
                }
                else
                {
                    serie.Points.Add(getDataPoint(i1));
                }
                series.Add(serie);
            }

            chartAlmacen.Series.AddRange(series.ToArray());

        }
        private NumericDataPoint getDataPoint(IGrouping<string, UltraGridRow> group)
        {
            double valor;
            switch ((byte)cboFormula.SelectedItem.DataValue)
            {
                case 1: valor = group.Count();
                    break;
                case 2: valor = group.Average(r => FuncConvert(r));
                    break;
                case 3: valor = group.Max(r => FuncConvert(r));
                    break;
                case 4: valor = group.Min(r => FuncConvert(r));
                    break;
                default: valor = group.Sum(r => FuncConvert(r));
                    break;
            }
            return new NumericDataPoint(valor, group.Key, false);
        }
        private string getTextGroup(UltraGridCell celda)
        {
            if (cboFiltroFecha.Enabled && cboFiltroFecha.SelectedIndex > 0)
            {
                return DateTime.Parse(celda.Text).ToString((string)cboFiltroFecha.SelectedItem.DataValue);
            }
            else
            {
                return celda.Text;
            }
        }
        #endregion

        #region Events UI
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            _strFilterExpression = string.Empty;
            List<string> Filters = new List<string>();

            if (cboEstablecimiento.Value != null && cboEstablecimiento.Value.ToString() != "-1")
            {
                Filters.Add("i_IdEstablecimiento == " + cboEstablecimiento.Value.ToString());
            }

            if (cboVendedor.Value != null && cboVendedor.Value.ToString() != "-1")
            {
                Filters.Add("v_IdVendedor == \"" + cboVendedor.Value.ToString() + "\"");
            }

            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }
            CargarDatos();
        }
   
        private void grdData_ClickCellButton(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            if (e.Cell.Column.Key == "Consultar")
            {
                if (grdData.ActiveRow != null)
                {
                    pstrIdVenta = grdData.ActiveRow.Cells["v_IdVenta"].Value.ToString();
                    frmRegistroVenta frm = new frmRegistroVenta("Consulta", pstrIdVenta);
                    frm.ShowDialog();
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                // Prepare a dummy string, thos would appear in the dialog
                string dummyFileName = "REPORTE ALMACEN " + DateTime.Today.ToString("dd-MM-yyyy");
                SaveFileDialog sf = new SaveFileDialog();
                sf.DefaultExt = "xlsx";
                sf.Filter = "xlsx files (*.xlsx)|*.xlsx";
                // Feed the dummy name to the save dialog
                sf.FileName = dummyFileName;

                if (sf.ShowDialog() == DialogResult.OK)
                {
                    using (new LoadingClass.PleaseWait(this.Location, "Exportando excel..."))
                    {
                        ultraGridExcelExporter1.Export(grdData, sf.FileName);
                    }
                    System.Diagnostics.Process.Start(sf.FileName);
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog f = new SaveFileDialog();
            f.Filter = "JPG Image (*.jpg)|*.jpg|BMP Image(*.bmp)|*.bmp";
            f.DefaultExt = "jpg";
            f.FileName = "Grafico " + cboTipo1.SelectedItem.DisplayText + ".jpg";
            f.Title = "Guardar Gráfico";
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (var bmp = new Bitmap(chartAlmacen.Width, chartAlmacen.Height))
                {
                    chartAlmacen.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    bmp.Save(f.FileName);
                }
                if(UltraMessageBox.Show("Imagen Guardada!" + System.Environment.NewLine + f.FileName + System.Environment.NewLine + "¿Desea Abrir Imagen?", "Guardar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) 
                    == System.Windows.Forms.DialogResult.Yes)
                    System.Diagnostics.Process.Start(f.FileName);
            }
        }

        private void Tbs_SelectedTabChanged(object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e)
        {
            if (e.Tab.Key == "grafico")
                ViewGraph();
        }

        void cbOpcionesGraph_SelectionChanged(object sender, EventArgs e)
        {
            if (cbOpcionesGraph.SelectedItem != null)
            {
                var texto = cbOpcionesGraph.SelectedItem.DisplayText;
                Infragistics.UltraChart.Shared.Styles.ChartType tipoChart;
                if (Enum.TryParse<Infragistics.UltraChart.Shared.Styles.ChartType>(texto, out tipoChart))
                {
                    chartAlmacen.ChartType = tipoChart;
                    var is3D = cbOpcionesGraph.SelectedItem.DataValue.ToString() == "1";
                    Events3D(is3D);
                    gb2D.Visible = !is3D;
                    gb3D.Visible = is3D;
                }
            }
        }

        private void cboTipo_SelectionChanged(object sender, EventArgs e)
        {
            if (cboTipo1.SelectedItem != null)
            {
                ReadyView = false;
                this.ViewGraph();
            }
        }

        void cboTipo2_SelectionChanged(object sender, EventArgs e)
        {
            if (cboTipo2.SelectedItem != null)
            {
                cboFiltroFecha.Enabled = (cboTipo2.SelectedIndex == 3);
                ReadyView = false;
                this.ViewGraph();
           
            }
        }

        private void cboFiltroFechaAndMoneda_SelectionChanged(object sender, EventArgs e)
        {
            if (cboTipo1.SelectedItem != null)
            {
                ReadyView = false;
                ViewGraph();
            }
        }

        #region ConextMenu Chart
        private void lblExpand_Click(object sender, EventArgs e)
        {
            splitRight.Collapsed = splitTop.Collapsed = !splitTop.Collapsed;
            popupMenu.Close();
            (sender as Infragistics.Win.Misc.UltraLabel).Text = (splitTop.Collapsed) ? "Contraer" : "Expandir";
        }

        private void lblSave_Click(object sender, EventArgs e)
        {
            this.InvokeOnClick(this.btnSave, new EventArgs());
            popupMenu.Close();
        }

        private void chartAlmacen_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                popupMenu.Show();
        }
        #endregion

        #endregion

        #region Graphs
        private bool ReadyView = false;
        private bool ReadyEvent = false;

        private void ViewGraph()
        {
            if (ReadyView) return;
            grdData.DisplayLayout.Bands[0].SortedColumns.Clear();
            chartAlmacen.Series.Clear();
            SetData();
            ReadyView = true;
        }

        private void Events3D(bool set)
        {
            if (set)
            {
                if (ReadyEvent) return;
                chartAlmacen.MouseDown += chart1_MouseDown;
                chartAlmacen.MouseUp += chart1_MouseUp;
                chartAlmacen.MouseWheel += chart1_MouseWheel;
                ReadyEvent = true;
            }
            else
            {
                if (!ReadyEvent) return;
                chartAlmacen.MouseDown -= chart1_MouseDown;
                chartAlmacen.MouseUp -= chart1_MouseUp;
                chartAlmacen.MouseWheel -= chart1_MouseWheel;
                ReadyEvent = false;
            }
        }

        #region Events Grapfh
        private Point StartPoint = Point.Empty;
        private void chart1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                chartAlmacen.MouseMove += chart1_MouseMove;
                StartPoint = new Point(e.Location.X + (int)chartAlmacen.Transform3D.YRotation, e.Location.Y - (int)chartAlmacen.Transform3D.XRotation);
            }
        }

        private void chart1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                chartAlmacen.MouseMove -= chart1_MouseMove;
        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            chartAlmacen.Transform3D.XRotation = e.Location.Y - StartPoint.Y;
            chartAlmacen.Transform3D.YRotation = StartPoint.X - e.Location.X;
            System.Diagnostics.Debug.WriteLine(e.Location);
        }

        void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                var newScale = chartAlmacen.Transform3D.Scale + 7;
                chartAlmacen.Transform3D.Scale = (newScale > 100) ? 100 : newScale;
            }
            else
            {
                var newScale = chartAlmacen.Transform3D.Scale - 7;
                chartAlmacen.Transform3D.Scale = (newScale < 10) ? 10 : newScale;
            }
        }
        #endregion

        private void ultraTrackBar1_ValueChanged(object sender, EventArgs e)
        {
            chartAlmacen.Transform3D.Perspective = ultraTrackBar1.Value;
        }

        private void chkLight_CheckedChanged(object sender, EventArgs e)
        {
            chartAlmacen.Transform3D.Light = chkLight.Checked;
        }

        private void chkOutline_CheckedChanged(object sender, EventArgs e)
        {
            chartAlmacen.Transform3D.Outline = chkOutline.Checked;
        }

        private void chkScale2D_CheckedChanged(object sender, EventArgs e)
        {
            chartAlmacen.Axis.X.ScrollScale.Visible = chkScale2D.Checked;
            chartAlmacen.Axis.Y.ScrollScale.Visible = chkScale2D.Checked;
        }
        #endregion

    }
    /// <summary>
    /// Calcula la Suma de Totales de estado = 1
    /// </summary>
    public class TotalesCustom : ICustomSummaryCalculator
    {
        private decimal totalD;
        private decimal totalS;
        internal TotalesCustom()
        {

        }

        public void AggregateCustomSummary(SummarySettings summarySettings, UltraGridRow row)
        {
            var obj = (row.ListObject as SAMBHS.Common.BE.MonitoreoVentasDto);
            if(obj.Total.HasValue){
                this.totalS += (obj.Moneda.Equals("S")) ? obj.Total.Value : obj.Total.Value * obj.TipoCambio;
                this.totalD += (obj.Moneda.Equals("D")) ? obj.Total.Value : obj.Total.Value / obj.TipoCambio;
            }
        }

        public void BeginCustomSummary(SummarySettings summarySettings, RowsCollection rows)
        {
            this.totalS = 0M;
            this.totalD = 0M;
        }

        public object EndCustomSummary(SummarySettings summarySettings, RowsCollection rows)
        {
            return string.Concat("Soles-> ", totalS.ToString("#.000"), " │ Dólares -> ", totalD.ToString("#.000"));
        }
    }
}
