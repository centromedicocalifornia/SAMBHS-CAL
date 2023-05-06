using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinGrid;
using Microsoft.VisualBasic;
using SAMBHS.Cobranza.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmMigrarCobranzasAdministrativas : Form
    {
        public frmMigrarCobranzasAdministrativas()
        {
            InitializeComponent();
        }

        private void frmMigrarCobranzasAdministrativas_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
        }

        private void ultraGroupBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnCabecera_Click(object sender, EventArgs e)
        {
            if (grdDataCabecera.Rows.Any())
            {
                grdDataCabecera.Selected.Rows.AddRange((UltraGridRow[])grdDataCabecera.Rows.All);
                grdDataCabecera.DeleteSelectedRows(false);
            }

            try
            {
                string sFileName;
                var choofdlog = new OpenFileDialog
                {
                    Filter = "Archivos Excel (*.*)| *.*",
                    FilterIndex = 1,
                    Multiselect = false
                };

                if (choofdlog.ShowDialog() == DialogResult.OK)
                {
                    sFileName = choofdlog.FileName;

                }
                else
                {
                    return;
                }

                using (new LoadingClass.PleaseWait(this.Location, "Importando Cabeceras..."))
                {
                    Workbook workbook = Workbook.Load(sFileName);
                    Worksheet worksheet = workbook.Worksheets[0];

                    this.ultraDataSource1.Reset();

                    bool isHeaderRow = true;
                    foreach (WorksheetRow row in worksheet.Rows)
                    {
                        if (isHeaderRow)
                        {
                            foreach (WorksheetCell cell in row.Cells)
                            {
                                string columnKey = cell.GetText();

                                var column = this.ultraDataSource1.Band.Columns.Add(columnKey);

                                switch (columnKey)
                                {
                                    default:
                                        column.DataType = typeof(string);
                                        break;
                                }
                            }

                            isHeaderRow = false;
                        }
                        else
                        {
                            this.ultraDataSource1.Rows.Add(row.Cells.Select(cell => cell.Value).ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return;
            }

            grdDataCabecera.SetDataBinding(this.ultraDataSource1, "Band 0");

            grdDataCabecera.Text = string.Format("Se encontraron {0} registros", grdDataCabecera.Rows.Count());

            foreach (string ColumnKey in grdDataCabecera.Rows[0].Cells.Cast<UltraGridCell>().Select(cell => cell.Column.Key).Where(columnKey => columnKey != "Nuevo"))
            {
                cboCabCorrelativoC.Items.Add(ColumnKey);
                cboCabFechaReg.Items.Add(ColumnKey);
                cboCabGlosa.Items.Add(ColumnKey);
                cboCabTipoCambio.Items.Add(ColumnKey);
                cboCabTipoDocumentoC.Items.Add(ColumnKey);
            }
            var columnas = cboCabTipoDocumentoC.Items.Cast<string>().ToList();
            cboCabCorrelativoC.SelectedIndex = columnas.FindIndex(p => p == "cncom_3");
            cboCabFechaReg.SelectedIndex = columnas.FindIndex(p => p == "dfcom_3");
            cboCabGlosa.SelectedIndex = columnas.FindIndex(p => p == "cglosaa_3");
            cboCabTipoCambio.SelectedIndex = columnas.FindIndex(p => p == "ntcam_3");
            cboCabTipoDocumentoC.SelectedIndex = columnas.FindIndex(p => p == "cccom_3");
        }

        private bool ConvertirFechas(string strCampo)
        {
            if (grdDataCabecera.Rows.Any() && strCampo != "--Seleccionar--")
            {

                try
                {
                    var filas =
                        grdDataCabecera.Rows.Where(
                            p =>
                                p.Cells[strCampo].Value != null &&
                                !string.IsNullOrEmpty(p.Cells[strCampo].Value.ToString())).ToList();


                    foreach (var o in filas)
                        o.Cells[strCampo].Value = RetornaFecha(o.Cells[strCampo].Value.ToString());
                    return true;
                }
                catch (Exception ex)
                {
                    UltraMessageBox.Show(ex.Message);
                    return false;
                }
            }
            return false;
        }

        private static DateTime RetornaFecha(string pstrFecha)
        {
            try
            {
                if (!pstrFecha.Count(p => p == '-').Equals(3))
                {
                    try
                    {
                        DateTime fecha = DateTime.TryParse(pstrFecha, out fecha) ? fecha : DateTime.FromOADate(double.Parse(pstrFecha));
                        return fecha;
                    }
                    catch { return DateTime.Today; }
                }
                var fechaArray = pstrFecha.Split('-');
                return new DateTime(int.Parse(fechaArray[0]), int.Parse(fechaArray[1]), int.Parse(fechaArray[2]));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return DateTime.Today;
            }

        }

        private void btnDetalle_Click(object sender, EventArgs e)
        {
            if (grdDataDetalle.Rows.Any())
            {
                grdDataDetalle.Selected.Rows.AddRange((UltraGridRow[])grdDataDetalle.Rows.All);
                grdDataDetalle.DeleteSelectedRows(false);
            }

            try
            {
                string sFileName;
                var choofdlog = new OpenFileDialog
                {
                    Filter = "Archivos Excel (*.*)| *.*",
                    FilterIndex = 1,
                    Multiselect = false
                };

                if (choofdlog.ShowDialog() == DialogResult.OK)
                {
                    sFileName = choofdlog.FileName;

                }
                else
                {
                    return;
                }

                using (new LoadingClass.PleaseWait(this.Location, "Importando Detalles..."))
                {
                    Workbook workbook = Workbook.Load(sFileName);
                    Worksheet worksheet = workbook.Worksheets[0];

                    this.ultraDataSource2.Reset();

                    bool isHeaderRow = true;
                    foreach (WorksheetRow row in worksheet.Rows)
                    {
                        if (isHeaderRow)
                        {
                            foreach (WorksheetCell cell in row.Cells)
                            {
                                string columnKey = cell.GetText();

                                var column = this.ultraDataSource2.Band.Columns.Add(columnKey);

                                switch (columnKey)
                                {
                                    default:
                                        column.DataType = typeof(string);
                                        break;
                                }
                            }

                            isHeaderRow = false;
                        }
                        else
                        {
                            this.ultraDataSource2.Rows.Add(row.Cells.Select(cell => cell.Value).ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return;
            }

            grdDataDetalle.SetDataBinding(this.ultraDataSource2, "Band 0");

            grdDataDetalle.Text = string.Format("Se encontraron {0} registros", grdDataDetalle.Rows.Count());

            foreach (string ColumnKey in grdDataDetalle.Rows[0].Cells.Cast<UltraGridCell>().Select(cell => cell.Column.Key).Where(columnKey => columnKey != "Nuevo"))
            {
                cboDetNetoXCobrar.Items.Add(ColumnKey);
                cboDetTDventa.Items.Add(ColumnKey);
                cboDetTipoDocumentoC.Items.Add(ColumnKey);
                cboDetCorrelativoC.Items.Add(ColumnKey);
                cboDetCorrelativoVenta.Items.Add(ColumnKey);
                cboDetMontoCobranza.Items.Add(ColumnKey);
            }
            var columnas = cboDetNetoXCobrar.Items.Cast<string>().ToList();
            cboDetNetoXCobrar.SelectedIndex = columnas.FindIndex(p => p == "nimpo_3a");
            cboDetTDventa.SelectedIndex = columnas.FindIndex(p => p == "ctdoc_3a");
            cboDetTipoDocumentoC.SelectedIndex = columnas.FindIndex(p => p == "cccom_3a");
            cboDetCorrelativoC.SelectedIndex = columnas.FindIndex(p => p == "cncom_3a");
            cboDetCorrelativoVenta.SelectedIndex = columnas.FindIndex(p => p == "cndoc_3a");
            cboDetMontoCobranza.SelectedIndex = columnas.FindIndex(p => p == "nimp1_3a");

        }

        private void cboImportarDaTA_Click(object sender, EventArgs e)
        {
            Start();
        }


        private void grdDataCabecera_AfterRowActivate(object sender, EventArgs e)
        {
            if (cboCabTipoDocumentoC.SelectedValue == null || cboCabCorrelativoC.SelectedValue == null ||
                cboDetCorrelativoC.SelectedValue == null || cboDetTipoDocumentoC.SelectedValue == null) return;

            grdDataDetalle.Selected.Rows.Clear();
            var tipoDoc = grdDataCabecera.ActiveRow.Cells[cboCabTipoDocumentoC.Text].Value.ToString().Trim();
            var serie = grdDataCabecera.ActiveRow.Cells[cboCabCorrelativoC.Text].Value.ToString().Trim();

            var detTipoDoc = cboDetTipoDocumentoC.Text;
            var detSerie = cboDetCorrelativoC.Text;

            var listaDetalles = grdDataDetalle.Rows.AsParallel().Where(p => p.Cells[detTipoDoc].Value.ToString().Trim() == (tipoDoc) &&
                                                                            p.Cells[detSerie].Value.ToString().Trim() == (serie)).ToList();

            if (!listaDetalles.Any()) return;
            grdDataDetalle.Selected.Rows.AddRange(listaDetalles.ToArray());
            var firstOrDefault = listaDetalles.FirstOrDefault();
            if (firstOrDefault != null) firstOrDefault.Activate();
        }

        public void Start()
        {
            try
            {
                #region Valiadaciones

                var resp = MessageBox.Show(@"¿Seguro de proseguir?", @"Sistema", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (resp == DialogResult.No) return;
                lblEstadoMigracion.Visible = true;
                if (!ConvertirFechas(cboCabFechaReg.Text)) return;
                var listasCobranzasToInsert = new List<cobranzaDto>();
                var ventasDiccionario =
                    new VentaBL().ListarBusquedaVentas()
                        .ToDictionary(
                            p =>
                                new
                                {
                                    tipo = p.i_IdTipoDocumento ?? -1,
                                    serie = p.v_SerieDocumento.Trim(),
                                    correlativo = p.v_CorrelativoDocumento.Trim()
                                }, o => o.v_IdVenta);

                var _cboCabTipoDocumentoC = cboCabTipoDocumentoC.Text;
                var _cboDetTipoDocumentoC = cboDetTipoDocumentoC.Text;
                var _cboDetCorrelativoC = cboDetCorrelativoC.Text;
                var _cboCabCorrelativoC = cboCabCorrelativoC.Text;
                var _cboCabFechaReg = cboCabFechaReg.Text;
                var _cboCabTipoCambio = cboCabTipoCambio.Text;
                var _cboDetNetoXCobrar = cboDetNetoXCobrar.Text;
                var _cboDetMontoCobranza = cboDetMontoCobranza.Text;

                var cabecerasValidas =
                    grdDataCabecera.Rows.Where(f => !string.IsNullOrWhiteSpace(f.Cells[_cboCabTipoDocumentoC].Value.ToString())
                        && !string.IsNullOrWhiteSpace(f.Cells[_cboCabCorrelativoC].Value.ToString())).ToList();

                var detallesValidos =
                    grdDataDetalle.Rows.Where(f => !string.IsNullOrWhiteSpace(f.Cells[_cboDetTipoDocumentoC].Value.ToString())
                                    && !string.IsNullOrWhiteSpace(f.Cells[_cboDetCorrelativoC].Value.ToString())).ToList();

                #endregion

                Task.Factory.StartNew(() =>
                {
                    Invoke((MethodInvoker)delegate { lblEstadoMigracion.Text = @"Recolectando información..."; });
                    var total = cabecerasValidas.Count;
                    var progreso = 0;
                    foreach (var fila in cabecerasValidas.AsParallel())
                    {
                        #region Cabecera
                        var cabeceraDto = new cobranzaDto { CobranzadetalleDtos = new List<cobranzadetalleDto>() };
                        cabeceraDto.t_FechaRegistro = DateTime.Parse(fila.Cells[_cboCabFechaReg].Value.ToString());
                        cabeceraDto.v_Periodo = cabeceraDto.t_FechaRegistro.Value.Year.ToString();
                        cabeceraDto.i_IdEstablecimiento = 1;
                        cabeceraDto.i_IdTipoDocumento = int.Parse(fila.Cells[_cboCabTipoDocumentoC].Value.ToString());
                        cabeceraDto.v_Mes = cabeceraDto.t_FechaRegistro.Value.Month.ToString("00");
                        cabeceraDto.v_Correlativo = "00000001";
                        cabeceraDto.d_TipoCambio = decimal.Parse(fila.Cells[_cboCabTipoCambio].Value.ToString());
                        cabeceraDto.i_IdMedioPago = 1;
                        cabeceraDto.v_Glosa = "COBRANZA DEL DÍA";
                        cabeceraDto.i_IdMoneda = decimal.Parse(fila.Cells["ntot1_3"].Value.ToString()) > 0 ? 1 : 2;
                        cabeceraDto.i_IdEstado = 1;
                        cabeceraDto.d_TotalSoles = decimal.Parse(fila.Cells["ntot1_3"].Value.ToString());
                        cabeceraDto.d_TotalDolares = /*decimal.Parse(fila.Cells["ntot2_3"].Value.ToString())*/ 0M;
                        #endregion

                        #region Detalles
                        var filasDependientes =
                                    detallesValidos.Where(
                                        p => p.Cells[_cboDetTipoDocumentoC].Value.ToString().Equals(fila.Cells[_cboCabTipoDocumentoC].Value.ToString().Trim()) &&
                                             p.Cells[_cboDetCorrelativoC].Value.ToString().Equals(fila.Cells[_cboCabCorrelativoC].Value.ToString().Trim())).ToList();

                        foreach (var filaDetalle in filasDependientes)
                        {
                            var detalle = new cobranzadetalleDto();
                            var idDocumento = int.Parse(filaDetalle.Cells["ctdoc_3a"].Value.ToString());
                            var serieCorrelativo = filaDetalle.Cells["cndoc_3a"].Value.ToString().Split('-');
                            var serie = serieCorrelativo[0];
                            var correlativo = serieCorrelativo[1];
                            string idVenta = ventasDiccionario.TryGetValue(new { tipo = idDocumento, serie, correlativo }, out idVenta) ? idVenta : string.Empty;
                            detalle.v_IdVenta = idVenta;
                            if (string.IsNullOrWhiteSpace(detalle.v_IdVenta)) continue;
                            detalle.i_IdFormaPago = -1;
                            detalle.i_IdTipoDocumentoRef = -1;
                            detalle.v_DocumentoRef = string.Empty;
                            detalle.d_NetoXCobrar = decimal.Parse(filaDetalle.Cells[_cboDetNetoXCobrar].Value.ToString());
                            detalle.d_ImporteSoles = decimal.Parse(filaDetalle.Cells[_cboDetMontoCobranza].Value.ToString());
                            detalle.i_EsLetra = 0;
                            cabeceraDto.CobranzadetalleDtos.Add(detalle);
                        }
                        listasCobranzasToInsert.Add(cabeceraDto);
                        #endregion

                        progreso++;
                        Invoke((MethodInvoker)delegate { lblEstadoMigracion.Text = string.Format(@"Recolectando información...{0}", (progreso * 100) / total + "%"); });
                    }
                }, TaskCreationOptions.LongRunning).ContinueWith(delegate
                {
                    var objCobranzaMigracion = new CobranzaMigracion { CobranzasParaInsertar = listasCobranzasToInsert };
                    objCobranzaMigracion.OnInsercionEvent += objCobranzaMigracion_OnInsercionEvent;
                    objCobranzaMigracion.OnErrorEvent += objCobranzaMigracion_OnErrorEvent;
                    objCobranzaMigracion.ComenzarAsync();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + '\n' + @"Linea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }

        private static void objCobranzaMigracion_OnErrorEvent(OperationResult objOperationResult)
        {
            MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage,
                @"Error en el proceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void objCobranzaMigracion_OnInsercionEvent(string pstrCobranza)
        {
            lblEstadoMigracion.Text = string.Format("Completado: {0}", pstrCobranza);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var input = Interaction.InputBox("Ingrese los ids de las cobranzas", "Eliminar Cobranzas");
            if (input == null || !input.Contains(','))
            {
                MessageBox.Show(@"Los ids estan incorrectamente ingresados.");
                return;
            }
            lblEstadoMigracion.Visible = true;
            var objCobranzaMigracion = new CobranzaMigracion { IdsEliminar = input.Split(',') };
            objCobranzaMigracion.OnInsercionEvent += objCobranzaMigracion_OnInsercionEvent;
            objCobranzaMigracion.OnErrorEvent += objCobranzaMigracion_OnErrorEvent;
            objCobranzaMigracion.ComenzarEliminacionAsync();
        }
    }
}
