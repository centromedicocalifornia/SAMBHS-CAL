using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Venta.BL;
using System.Threading;
using System.Threading.Tasks;
using SAMBHS.Common.BE;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using SAMBHS.Almacen.BL;

namespace SAMBHS.Windows.WinClient.UI.Reportes.Ventas
{
    public partial class frmEstadisticaVentas : Form
    {
        #region Fields
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly VentaBL _objVentaBl = new VentaBL();
        private List<MesesEstadisticas> _listaMesesEstadisticasVentas = new List<MesesEstadisticas>();
        private List<MesesEstadisticas> _listaMesesEstadisticasCompras = new List<MesesEstadisticas>();
        private List<EstadisticasVentas> _listaEstadisticas = new List<EstadisticasVentas>();
        private Task _tarea;
        #endregion

        #region Init & Load
        public frmEstadisticaVentas(string arg)
        {
            InitializeComponent();
        }

        private void frmEstadisticaVentas__Load(object sender, EventArgs e)
        {
            CargarCombos();
            cboFormato.Value = "2";
            cboPeriodo.Value = Globals.ClientSession.i_Periodo.ToString();



            


        }
        #endregion

        #region Methods
        private void CargarCombos()
        {
            BackColor = new GlobalFormColors().FormColor;
            var objEstablecimientoBl = new EstablecimientoBL();
            var objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", objEstablecimientoBl.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", objEstablecimientoBl.GetAlmacenesXEstablecimiento(-1), DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", Globals.CacheCombosVentaDto.cboVendedor, DropDownListAction.All);
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            cboVendedor.Value = "-1";
            //cboAlmacen1.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            //Utils.Windows.LoadUltraComboList(cboAlmacen1, "Value2", "Id", objEstablecimientoBl.GetAlmacenesXEstablecimiento( int.Parse (cboEstablecimiento.Value.ToString ())),
            //    DropDownListAction.Select);
            //cboAlmacen1.Value = "-1";
            //this.cboAlmacen1.CheckedListSettings.CheckStateMember = "IsActive";
            var almacenes = objEstablecimientoBl.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));
        }

        private void OcultarMostrarBuscar(bool estado)
        {
            Text = estado
                ? @"Generando... " + "Estadistica de Ventas"
                : @"Estadistica de Ventas";
            pBuscando.Visible = estado;
            btnVisualizar.Enabled = !estado;

        }

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Estadistica de Ventas";

            using (var sf = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = @"xlsx files (*.xlsx)|*.xlsx",
                FileName = dummyFileName
            })
            {
                if (sf.ShowDialog() != DialogResult.OK) return;
                btnExportarBandeja.Appearance.Image = Resource.loadingfinal1;

                var filename = sf.FileName;
                try
                {
                    _tarea = Task.Factory.StartNew(() =>
                    {
                        using (var exporter = new UltraGridExcelExporter())
                            exporter.Export(grdData, filename);
                    }).ContinueWith(t => ActualizarLabel("Estadistica de Ventas  Exportada a Excel."), TaskScheduler.FromCurrentSynchronizationContext());
                }
                catch (Exception)
                {
                    UltraMessageBox.Show("Ocurrió un Error , posiblemte archivo esté siendo utilizado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ConfiguracionGrilla()
        {
            UltraGridColumn dCantidadEnero = grdData.DisplayLayout.Bands[0].Columns["dCantidad1"];
            UltraGridColumn dCantidadFebrero = grdData.DisplayLayout.Bands[0].Columns["dCantidad2"];
            UltraGridColumn dCantidadMarzo = grdData.DisplayLayout.Bands[0].Columns["dCantidad3"];
            UltraGridColumn dCantidadAbril = grdData.DisplayLayout.Bands[0].Columns["dCantidad4"];
            UltraGridColumn dCantidadMayo = grdData.DisplayLayout.Bands[0].Columns["dCantidad5"];
            UltraGridColumn dCantidadJunio = grdData.DisplayLayout.Bands[0].Columns["dCantidad6"];
            UltraGridColumn dCantidadJulio = grdData.DisplayLayout.Bands[0].Columns["dCantidad7"];
            UltraGridColumn dCantidadAgosto = grdData.DisplayLayout.Bands[0].Columns["dCantidad8"];
            UltraGridColumn dCantidadSetiembre = grdData.DisplayLayout.Bands[0].Columns["dCantidad9"];
            UltraGridColumn dCantidadOctubre = grdData.DisplayLayout.Bands[0].Columns["dCantidad10"];
            UltraGridColumn dCantidadNoviembre = grdData.DisplayLayout.Bands[0].Columns["dCantidad11"];
            UltraGridColumn dCantidadDiciembre = grdData.DisplayLayout.Bands[0].Columns["dCantidad12"];
            UltraGridColumn dCantidadEneroCompras = grdData.DisplayLayout.Bands[0].Columns["dCantidadC1"];
            UltraGridColumn dCantidadFebreroCompras = grdData.DisplayLayout.Bands[0].Columns["dCantidadC2"];
            UltraGridColumn dCantidadMarzoCompras = grdData.DisplayLayout.Bands[0].Columns["dCantidadC3"];
            UltraGridColumn dCantidadAbrilCompras = grdData.DisplayLayout.Bands[0].Columns["dCantidadC4"];
            UltraGridColumn dCantidadMayoCompras = grdData.DisplayLayout.Bands[0].Columns["dCantidadC5"];
            UltraGridColumn dCantidadJunioCompras = grdData.DisplayLayout.Bands[0].Columns["dCantidadC6"];
            UltraGridColumn dCantidadJulioCompras = grdData.DisplayLayout.Bands[0].Columns["dCantidadC7"];
            UltraGridColumn dCantidadAgostoCompras = grdData.DisplayLayout.Bands[0].Columns["dCantidadC8"];
            UltraGridColumn dCantidadSetiembreCompras = grdData.DisplayLayout.Bands[0].Columns["dCantidadC9"];
            UltraGridColumn dCantidadOctubreCompras = grdData.DisplayLayout.Bands[0].Columns["dCantidadC10"];
            UltraGridColumn dCantidadNoviembreCompras = grdData.DisplayLayout.Bands[0].Columns["dCantidadC11"];
            UltraGridColumn dCantidadDiciembreCompras = grdData.DisplayLayout.Bands[0].Columns["dCantidadC12"];
            dCantidadEnero.Hidden = _listaMesesEstadisticasVentas.Where(x => x.iMes == "01").Any() ? false : true;
            dCantidadFebrero.Hidden = _listaMesesEstadisticasVentas.Where(x => x.iMes == "02").Any() ? false : true;
            dCantidadMarzo.Hidden = _listaMesesEstadisticasVentas.Where(x => x.iMes == "03").Any() ? false : true;
            dCantidadAbril.Hidden = _listaMesesEstadisticasVentas.Where(x => x.iMes == "04").Any() ? false : true;
            dCantidadMayo.Hidden = _listaMesesEstadisticasVentas.Where(x => x.iMes == "05").Any() ? false : true;
            dCantidadJunio.Hidden = _listaMesesEstadisticasVentas.Where(x => x.iMes == "06").Any() ? false : true;
            dCantidadJulio.Hidden = _listaMesesEstadisticasVentas.Where(x => x.iMes == "07").Any() ? false : true;
            dCantidadAgosto.Hidden = _listaMesesEstadisticasVentas.Where(x => x.iMes == "08").Any() ? false : true;
            dCantidadSetiembre.Hidden = _listaMesesEstadisticasVentas.Where(x => x.iMes == "09").Any() ? false : true;
            dCantidadOctubre.Hidden = _listaMesesEstadisticasVentas.Where(x => x.iMes == "10").Any() ? false : true;
            dCantidadNoviembre.Hidden = _listaMesesEstadisticasVentas.Where(x => x.iMes == "11").Any() ? false : true;
            dCantidadDiciembre.Hidden = _listaMesesEstadisticasVentas.Where(x => x.iMes == "12").Any() ? false : true;




            dCantidadEneroCompras.Hidden = _listaMesesEstadisticasCompras.Where(x => x.iMes == "01").Any() ? false : true;
            dCantidadFebreroCompras.Hidden = _listaMesesEstadisticasCompras.Where(x => x.iMes == "02").Any() ? false : true;
            dCantidadMarzoCompras.Hidden = _listaMesesEstadisticasCompras.Where(x => x.iMes == "03").Any() ? false : true;
            dCantidadAbrilCompras.Hidden = _listaMesesEstadisticasCompras.Where(x => x.iMes == "04").Any() ? false : true;
            dCantidadMayoCompras.Hidden = _listaMesesEstadisticasCompras.Where(x => x.iMes == "05").Any() ? false : true;
            dCantidadJunioCompras.Hidden = _listaMesesEstadisticasCompras.Where(x => x.iMes == "06").Any() ? false : true;
            dCantidadJulioCompras.Hidden = _listaMesesEstadisticasCompras.Where(x => x.iMes == "07").Any() ? false : true;
            dCantidadAgostoCompras.Hidden = _listaMesesEstadisticasCompras.Where(x => x.iMes == "08").Any() ? false : true;
            dCantidadSetiembreCompras.Hidden = _listaMesesEstadisticasCompras.Where(x => x.iMes == "09").Any() ? false : true;
            dCantidadOctubreCompras.Hidden = _listaMesesEstadisticasCompras.Where(x => x.iMes == "10").Any() ? false : true;
            dCantidadNoviembreCompras.Hidden = _listaMesesEstadisticasCompras.Where(x => x.iMes == "11").Any() ? false : true;
            dCantidadDiciembreCompras.Hidden = _listaMesesEstadisticasCompras.Where(x => x.iMes == "12").Any() ? false : true;

        }

        private void ActualizarLabel(string texto)
        {
            lblDocumentoExportado.Text = texto;
            btnExportarBandeja.Enabled = false;
            btnExportarBandeja.Appearance.Image = Resource.accept;
        }
        #endregion

        #region Events UI
        private void cboEstablecimiento_ValueChanged(object sender, EventArgs e)
        {
            if (cboEstablecimiento.Value == null) return;

            var x = new EstablecimientoBL().GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.All);
            cboAlmacen.Value = "-1";

            //cboAlmacen1.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            //var xx = new EstablecimientoBL().GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));
            //Utils.Windows.LoadUltraComboList(cboAlmacen1, "Value2", "Id", xx,
            //    DropDownListAction.Select);

            //cboAlmacen1.Value = "-1";

        }

        private void btnVisualizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Validar.Validate(true, false).IsValid) return;
                string strFilterExpression = null;
                if (cboAlmacen.Value.ToString() != "-1")
                {
                    strFilterExpression = "IdAlmacen==" + cboAlmacen.Value;
                }
                else
                {
                    var ListaAlmacenes = (((List<KeyValueDTO>)cboAlmacen.DataSource).Where(o => o.Id != "-1")).ToList();
                    //var concat = string.Empty;
                    int cont = 1;
                    foreach (var item in ListaAlmacenes.Where(o => o.Id != "-1"))
                    {
                        if (cont == ListaAlmacenes.Count())
                        {
                            strFilterExpression = strFilterExpression + "idAlmacen==" + item.Id;
                        }
                        else
                        {
                            strFilterExpression = strFilterExpression + "idAlmacen==" + item.Id + " || ";
                        }
                        //concat = " || ";
                        cont += 1;
                    }
                    if (ListaAlmacenes.Count > 0)
                        strFilterExpression = "(" + strFilterExpression + ")";
                }
                OcultarMostrarBuscar(true);
                var objOperationResult = new OperationResult();
                Cursor.Current = Cursors.WaitCursor;

                Task.Factory.StartNew(() =>
                    {
                        _listaMesesEstadisticasVentas = _objVentaBl.ObtenerMesesEstadisticasVentas(int.Parse(cboEstablecimiento.Value.ToString()),cboPeriodo.Value.ToString ());
                        _listaMesesEstadisticasCompras = _objVentaBl.ObtenerMesesEstadisticasCompras(int.Parse(cboEstablecimiento.Value.ToString()),cboPeriodo.Value.ToString ());
                        //  VentasTotales = _objVentaBL.VentasTotales(ref objOperationResult);
                        _listaEstadisticas = new VentaBL().EstadisticasVentas(ref objOperationResult, chkIncluirProductosNoVendidos.Checked, int.Parse(cboEstablecimiento.Value.ToString()), strFilterExpression, "-1", "-1", cboVendedor.Value.ToString(), _listaMesesEstadisticasVentas, txtArtIni.Text.Trim() , int.Parse ( cboAlmacen.Value.ToString ()), cboPeriodo.Value.ToString (), int.Parse (cboFormato.Value.ToString ()));
                    }, _cts.Token)
                    .ContinueWith(t =>
                    {
                        if (_cts.IsCancellationRequested) return;
                        OcultarMostrarBuscar(false);
                        Cursor.Current = Cursors.Default;
                        if (objOperationResult.Success == 0)
                        {
                            UltraMessageBox.Show("Error al realizar Estadística", "Sistema", Icono: MessageBoxIcon.Error);
                            return;
                        }
                        ConfiguracionGrilla();
                        grdData.DataSource = _listaEstadisticas;
                        lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", grdData.Rows.Count);
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception)
            {
                UltraMessageBox.Show("Ocurrió un error al realizar Reporte", "Sistema", Icono: MessageBoxIcon.Error);
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
        #endregion


       
    }
}
