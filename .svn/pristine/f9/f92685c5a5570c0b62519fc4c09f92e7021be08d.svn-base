using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinTabControl;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Venta.BL;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmImportarVentasPlantillas : Form
    {
        private readonly StringBuilder _sbErrores = new StringBuilder();
        private readonly StringBuilder _sbErroresDetalle = new StringBuilder();
        private readonly Image _esperandoImage = Resource.time_green;
        private readonly Image _procesandoImage = Resource.loadingfinal1;
        private readonly Image _terminadoImage = Resource.accept;
        private readonly Image _errorImage = Resource.cancel;
        private int _rowIndex, _rowCount, _rowWithErrors;
        private ImportacionVentasBl _objImportador;

        public frmImportarVentasPlantillas()
        {
            InitializeComponent();
#if DEBUG
            txtRuta.Text = @"C:\Users\sistemas3\Desktop\plantilla_venta_contable.xlsx";
#endif
        }

        private void txtRuta_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                var choofdlog = new OpenFileDialog
                {
                    Filter = @"Archivos Excel (*.*)| *.*",
                    FilterIndex = 1,
                    Multiselect = false
                };

                if (choofdlog.ShowDialog() == DialogResult.OK)
                {
                    txtRuta.Text = choofdlog.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExtraerInfo_Click(object sender, EventArgs e)
        {
                Invoke((MethodInvoker) delegate
                {
                    if (!ExtraerCabecera(txtRuta.Text, grdData)) return;
                    var extraccionExitosa = ExtraerDetalle(txtRuta.Text, ultraGrid1);
                    if (!extraccionExitosa) return;
                    var detalles = (List<ImportacionRegistroVentaDetalleDto>)ultraGrid1.DataSource;
                    uTabControl.Tabs[1].Visible = detalles != null;

                    if (detalles != null)
                    {
                        var dic = detalles
                        .GroupBy(g => g.NroDocumento)
                        .ToDictionary(k => k.Key, o => o.ToList());

                        foreach (var row in grdData.Rows)
                        {
                            List<ImportacionRegistroVentaDetalleDto> det;
                            var cab = (ImportacionRegistroVentaDto)row.ListObject;
                            if (cab == null) continue;
                            //if (dic.TryGetValue(cab.NroDocumento, out det))
                            //    cab.DetalleVenta.AddRange(det);
                        }
                    }

                    grdData.Rows.ToList().ForEach(r =>
                    {
                        var error = r.Cells["Errores"].Value.ToString();
                        if (!string.IsNullOrWhiteSpace(error))
                        {
                            r.ToolTipText = error;
                            _sbErrores.AppendLine(string.Format("Fila: {0}", r.Index + 1));
                            _sbErrores.AppendLine(error);
                        }
                        r.Cells["_Progreso"].Value = _esperandoImage;
                    });
                    var ok = grdData.Rows.Count(r => Convert.ToBoolean(r.Cells["Valido"].Value.ToString()));
                    var notOk = grdData.Rows.Count(r => !Convert.ToBoolean(r.Cells["Valido"].Value.ToString()));
                    linkVerErrores.Visible = notOk > 0;
                    pbEstados.Visible = true;
                    lblEstados.Text = string.Format("Registros: {0} | Correctos: {1} | Con Errores: {2}", grdData.Rows.Count, ok, notOk);
                    pbEstados.Image = linkVerErrores.Visible ? Resource.alerta : Resource.tick;
                    btnGuardar.Enabled = true;
                });
           
        }

        private static bool ExtraerCabecera(string sFileName, UltraGridBase grid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sFileName)) return false;
                const int colTipoDocumento = 0;
                const int colSerie = 1;
                const int colCorrelativo = 2;
                const int colFechaEmision = 3;
                const int colTipoCambio = 4;
                const int colMoneda = 5;
                const int colAnulado = 6;
                const int colTipoDocumentoRef = 7;
                const int colSerieRef = 8;
                const int colCorrelativoRef = 9;
                const int colNroDocumentoIdentidad = 10;
                const int colRazonSocialNombre = 11;
                const int colDireccion = 12;
                const int colImporteVenta = 13;
                const int colCuentaMercaderia = 14;
                const int colGlosa = 15;

                var libroExcel = Workbook.Load(sFileName);
                var hojaCabecera = libroExcel.Worksheets[0];
                var dataSource = new List<ImportacionRegistroVentaDto>();

                foreach (var fila in hojaCabecera.Rows)
                {
                    if (fila.Index <= 1) continue;
                    if (string.IsNullOrWhiteSpace(fila.GetCellText(colTipoDocumento))) continue;
                    decimal d;
                    int i;
                    dataSource.Add(new ImportacionRegistroVentaDto
                    {
                        Anulado = fila.GetCellText(colAnulado),
                        Correlativo = fila.GetCellText(colCorrelativo),
                        CorrelativoRef = fila.GetCellText(colCorrelativoRef),
                        CuentaMercaderia = fila.GetCellText(colCuentaMercaderia),
                        Direccion = fila.GetCellText(colDireccion),
                        FechaEmision = fila.GetCellText(colFechaEmision),
                        Glosa = fila.GetCellText(colGlosa),
                        IdTipoDoc = int.TryParse(fila.GetCellText(colTipoDocumento), out i) ? i : -1,
                        IdTipoDocRef = int.TryParse(fila.GetCellText(colTipoDocumentoRef), out i) ? i : -1,
                        ImporteVenta = decimal.TryParse(fila.GetCellText(colImporteVenta), out d) ? d : 0M,
                        Moneda = fila.GetCellText(colMoneda),
                        NroDocumentoIdentidad = fila.GetCellText(colNroDocumentoIdentidad),
                        RazonSocialNombre = fila.GetCellText(colRazonSocialNombre),
                        Serie = fila.GetCellText(colSerie),
                        SerieRef = fila.GetCellText(colSerieRef),
                        TipoCambio = decimal.TryParse(fila.GetCellText(colTipoCambio), out d) ? d : 0M
                    });
                }

                grid.DataSource = dataSource;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool ExtraerDetalle(string sFileName, UltraGridBase grid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sFileName)) return false;
                const int colTipoDocumento = 0;
                const int colSerie = 1;
                const int colCorrelativo = 2;
                const int colCodigo = 3;
                const int colDescripcion = 4;
                const int colCantidad = 5;
                const int colTotal = 6;
                const int colCuenta = 7;

                var libroExcel = Workbook.Load(sFileName);
                var hojaDetalle = libroExcel.Worksheets[1];
                var dataSource = new List<ImportacionRegistroVentaDetalleDto>();

                foreach (var fila in hojaDetalle.Rows)
                {
                    if (fila.Index <= 1) continue;
                    if (string.IsNullOrWhiteSpace(fila.GetCellText(colTipoDocumento))) continue;
                    decimal d;
                    int i;
                    dataSource.Add(new ImportacionRegistroVentaDetalleDto
                    {
                        Correlativo = fila.GetCellText(colCorrelativo),
                        IdTipoDoc = int.TryParse(fila.GetCellText(colTipoDocumento), out i) ? i : -1,
                        Serie = fila.GetCellText(colSerie),
                        CodArticulo = fila.GetCellText(colCodigo),
                        Total = decimal.TryParse(fila.GetCellText(colTotal), out d) ? d : 0m,
                        Cantidad = decimal.TryParse(fila.GetCellText(colCantidad), out d) ? d : 0m,
                        Cuenta = fila.GetCellText(colCuenta),
                        DescripionArticulo = fila.GetCellText(colDescripcion)
                    });
                }

                if (!dataSource.Any()) return true;
                grid.DataSource = dataSource;
                grid.Rows.ToList().ForEach(r =>
                {
                    var error = r.Cells["Errores"].Value.ToString();
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        r.ToolTipText = error;
                        _sbErroresDetalle.AppendLine(string.Format("Fila: {0}", r.Index + 1));
                        _sbErroresDetalle.AppendLine(error);
                    }
                });
                var ok = grid.Rows.Count(r => Convert.ToBoolean(r.Cells["Valido"].Value.ToString()));
                var notOk = grid.Rows.Count(r => !Convert.ToBoolean(r.Cells["Valido"].Value.ToString()));
                linkVerErroresDetalle.Visible = notOk > 0;
                pbEstadosDetalle.Visible = true;
                lblEstadosDetalle.Text = string.Format("Registros: {0} | Correctos: {1} | Con Errores: {2}", grid.Rows.Count, ok, notOk);
                pbEstadosDetalle.Image = notOk > 0 ? Resource.alerta : Resource.tick;
                btnGuardar.Enabled = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ClientePublicoGeneral()
        {
            var cadena = new VentaBL().PublicoGeneral();
            if (cadena == null)
            {
                new VentaBL().InsertaClientePublicoGeneralSiEsNecesario();
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                ClientePublicoGeneral();
                if (!grdData.Rows.Any()) return;
                _rowIndex = 0;
                _objImportador = new ImportacionVentasBl();
                var ok = grdData.Rows.Count(r => Convert.ToBoolean(r.Cells["Valido"].Value.ToString()));

                if (ok > 0)
                {
                    if (_sbErrores.ToString().Trim().Length > 0)
                    {
                        var msg = MessageBox.Show(@"Los registros con errores serán eliminados antes de empezar la carga, ¿Desea Continuar?", @"Confirmación",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (msg == DialogResult.No) return;

                        var notOk = grdData.Rows.Where(r => !Convert.ToBoolean(r.Cells["Valido"].Value.ToString())).ToList();
                        notOk.ForEach(f => f.Delete(false));
                    }
                    uTabControl.PerformAction(UltraTabControlAction.NavigateToFirstTab);
                    btnExtraerInfo.Enabled = false;
                    pbProgreso.Maximum = 100;
                    _rowCount = grdData.Rows.Count();
                    grdData.DisplayLayout.Bands[0].Columns["EstadoRegistro"].Hidden = true;
                    grdData.DisplayLayout.Bands[0].Columns["_Progreso"].Hidden = false;
                    var clientes = new ClienteBL().DevuelveClientes().ToDictionary(k => k.Value1, o => o);
                    var productos = new ProductoBL().DevolverProductos().ToDictionary(k => k.Value1, o => o.Value2);
                    _objImportador.ListaClientes = clientes;
                    _objImportador.ListaProductos = productos;
                    _objImportador.ClientSession = Common.Resource.Globals.ClientSession.GetAsList();
                    _objImportador.TerminadoEvent += _objImportador_TerminadoEvent;
                    ImportarVenta();
                    btnExtraerInfo.Enabled = false;
                    btnGuardar.Enabled = false;
                }
                else
                {
                    MessageBox.Show(
                   @"Todas las filas presentan errores, imposible iniciar carga, por favor revise el log de errores",
                   @"Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void _objImportador_TerminadoEvent(bool terminadoConExito, string mensaje)
        {
            try
            {
                var fila = grdData.Rows[_rowIndex - 1];
                if (fila == null) return;
                fila.Cells["_Progreso"].Value = terminadoConExito ? _terminadoImage : _errorImage;
                if (!terminadoConExito)
                {
                    fila.ToolTipText = mensaje;
                    _rowWithErrors++;
                    pictureBox1.Visible = true;
                    lblErrores.Text = string.Format("Hay {0} errores.", _rowWithErrors);
                }
                
                pbProgreso.Value = (_rowIndex * 100) / _rowCount;
                if (_rowIndex == grdData.Rows.Count)
                {
                    btnExtraerInfo.Enabled = true;
                    btnGuardar.Enabled = true;
                    _objImportador.TerminadoEvent -= _objImportador_TerminadoEvent;
                    MessageBox.Show(@"Importación de Registros Finalizada", @"Sistema", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else if (_rowIndex < grdData.Rows.Count) ImportarVenta();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImportarVenta()
        {
            var fila = grdData.Rows[_rowIndex];
            if (fila == null) return;
            try
            {
                if (!Convert.ToBoolean(fila.Cells["Valido"].Value.ToString())) return;
                var venta = (ImportacionRegistroVentaDto)fila.ListObject;
                _objImportador.Comenzar(venta);
                _rowIndex++;
            }
            catch (Exception ex)
            {
                fila.ToolTipText = ex.Message;
                _rowWithErrors++;
                pictureBox1.Visible = true;
                lblErrores.Text = string.Format("Hay {0} errores.", _rowWithErrors);
            }
        }

        private void linkVerErrores_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void frmImportarVentasPlantillas_Load(object sender, EventArgs e)
        {

        }

        private void linkVerErroresDetalle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
  
        }
    }
}
