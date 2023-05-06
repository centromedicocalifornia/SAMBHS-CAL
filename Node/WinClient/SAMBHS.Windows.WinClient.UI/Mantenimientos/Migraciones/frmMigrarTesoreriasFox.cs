using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using LoadingClass;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Compra.BL;
using SAMBHS.Tesoreria.BL;
using SAMBHS.Venta.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    public partial class frmMigrarTesoreriasFox : Form
    {
        ClienteBL _objClienteBL = new ClienteBL();
        tesoreriaDto _tesoreriaDto = new tesoreriaDto();
        ProductoBL _objProductoBL = new ProductoBL();
        List<tesoreriaDto> TesoreriasConDetalle = new List<tesoreriaDto>();
        List<tesoreriaDto> _Tesorerias = new List<tesoreriaDto>();
        List<tesoreriadetalleDto> tesoreriadetalles = new List<tesoreriadetalleDto>();
        List<KeyValueDTO> Clientes = new List<KeyValueDTO>();

        public frmMigrarTesoreriasFox(string N)
        {
            InitializeComponent();
        }

        private void frmMigrarTesoreriasFox_Load(object sender, EventArgs e)
        {
            Clientes = _objClienteBL.DevuelveClientesProveedores();
            this.BackColor = new GlobalFormColors().FormColor;
            panel3.BackColor = new GlobalFormColors().FormColor;
            cboCabTipoDocT.SelectedIndex = 0;
            cboCabNroReg.SelectedIndex = 0;
            cboCabGlosa.SelectedIndex = 0;
            cboCabFechaEmision.SelectedIndex = 0;
            cboCabNombre.SelectedIndex = 0;
            cboCabTipoCambio.SelectedIndex = 0;
            cboDetTipoDocT.SelectedIndex = 0;
            cboDetNroReg.SelectedIndex = 0;
            cboDetNaturaleza.SelectedIndex = 0;
            cboDetCodCliente.SelectedIndex = 0;
            cboDetFechaRef.SelectedIndex = 0;
            cboDetTipoDocRef.SelectedIndex = 0;
            cboDetNroCuenta.SelectedIndex = 0;
            cboDetImporteSoles.SelectedIndex = 0;
            cboDetImporteDolares.SelectedIndex = 0;
            cboCabMoneda.SelectedIndex = 0;
            cboDetTipoDoc.SelectedIndex = 0;
            cboCabTotHSoles.SelectedIndex = 0;
            cboCabTotDSoles.SelectedIndex = 0;
            cboCabTotHDolares.SelectedIndex = 0;
            cboCabTotDDolares.SelectedIndex = 0;
            cboDetAnalisis.SelectedIndex = 0;
            cboDetNroDoc.SelectedIndex = 0;
            cboDetDestino.SelectedIndex = 0;
        }

        private void btnImportarCabecera_Click(object sender, EventArgs e)
        {

            if (grdData.Rows.Count() > 0)
            {
                grdData.Selected.Rows.AddRange((UltraGridRow[])grdData.Rows.All);
                grdData.DeleteSelectedRows(false);
            }

            try
            {
                string sFileName = "";
                OpenFileDialog choofdlog = new OpenFileDialog();
                choofdlog.Filter = "Archivos Excel (*.*)| *.*";
                choofdlog.FilterIndex = 1;
                choofdlog.Multiselect = false;

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

                                UltraDataColumn column = this.ultraDataSource1.Band.Columns.Add(columnKey);

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
                            List<object> rowData = new List<object>();
                            foreach (WorksheetCell cell in row.Cells)
                            {
                                rowData.Add(cell.Value);
                            }

                            this.ultraDataSource1.Rows.Add(rowData.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return;
            }

            grdData.SetDataBinding(this.ultraDataSource1, "Band 0");

            lblContadorFilas.Text = string.Format("Se encontraron {0} registros", grdData.Rows.Count());

            foreach (UltraGridCell cell in grdData.Rows[0].Cells)
            {
                string ColumnKey = cell.Column.Key;
                if (ColumnKey != "Nuevo")
                {
                    cboCabTipoDocT.Items.Add(ColumnKey);
                    cboCabNroReg.Items.Add(ColumnKey);
                    cboCabGlosa.Items.Add(ColumnKey);
                    cboCabFechaEmision.Items.Add(ColumnKey);
                    cboCabNombre.Items.Add(ColumnKey);
                    cboCabMoneda.Items.Add(ColumnKey);
                    cboCabTipoCambio.Items.Add(ColumnKey);
                    cboCabTotDSoles.Items.Add(ColumnKey);
                    cboCabTotDDolares.Items.Add(ColumnKey);
                    cboCabTotHSoles.Items.Add(ColumnKey);
                    cboCabTotHDolares.Items.Add(ColumnKey);
                    cboCabTipoMov.Items.Add(ColumnKey);
                    cboCabMedioPago.Items.Add(ColumnKey);
                    cboCabCuenta.Items.Add(ColumnKey);
                }
            }
            List<string> Columnas = cboCabTipoDocT.Items.Cast<string>().ToList();

            cboCabTipoDocT.SelectedIndex = Columnas.FindIndex(p => p == "ccom_3"); //4;
            cboCabNroReg.SelectedIndex = Columnas.FindIndex(p => p == "ncom_3"); //5;
            cboCabGlosa.SelectedIndex = Columnas.FindIndex(p => p == "glosa_3"); //19;
            cboCabFechaEmision.SelectedIndex = Columnas.FindIndex(p => p == "fcom_3"); //7;
            cboCabNombre.SelectedIndex = Columnas.FindIndex(p => p == "nombcp_3"); //18;
            cboCabMoneda.SelectedIndex = Columnas.FindIndex(p => p == "mone_3"); //10;
            cboCabTipoCambio.SelectedIndex = Columnas.FindIndex(p => p == "tcam_3"); //11;
            cboCabTotDSoles.SelectedIndex = Columnas.FindIndex(p => p == "totd1_3"); //40;
            cboCabTotDDolares.SelectedIndex = Columnas.FindIndex(p => p == "totd2_3"); //41;
            cboCabTotHSoles.SelectedIndex = Columnas.FindIndex(p => p == "toth1_3"); //42;
            cboCabTotHDolares.SelectedIndex = Columnas.FindIndex(p => p == "toth2_3"); //49;
            cboCabCuenta.SelectedIndex = Columnas.FindIndex(p => p == "cuenb_3"); //49;
            cboCabTipoMov.SelectedIndex = Columnas.FindIndex(p => p == "tmov_3"); //49;
            cboCabMedioPago.SelectedIndex = Columnas.FindIndex(p => p == "tpago_3"); //49;
        }

        private void btnImportarDetalle_Click(object sender, EventArgs e)
        {
            if (grdDataDetalles.Rows.Count() > 0)
            {
                grdDataDetalles.Selected.Rows.AddRange((UltraGridRow[])grdDataDetalles.Rows.All);
                grdDataDetalles.DeleteSelectedRows(false);
            }

            try
            {
                string sFileName = "";
                OpenFileDialog choofdlog = new OpenFileDialog();
                choofdlog.Filter = "Archivos Excel (*.*)| *.*";
                choofdlog.FilterIndex = 1;
                choofdlog.Multiselect = false;

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

                                UltraDataColumn column = this.ultraDataSource2.Band.Columns.Add(columnKey);

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
                            List<object> rowData = new List<object>();
                            foreach (WorksheetCell cell in row.Cells)
                            {
                                rowData.Add(cell.Value);
                            }

                            this.ultraDataSource2.Rows.Add(rowData.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return;
            }

            grdDataDetalles.SetDataBinding(this.ultraDataSource2, "Band 0");

            lblContadorFilasDetalle.Text = string.Format("Se encontraron {0} registros", grdDataDetalles.Rows.Count());

            foreach (UltraGridCell cell in grdDataDetalles.Rows[0].Cells)
            {
                string ColumnKey = cell.Column.Key;
                if (ColumnKey != "Nuevo")
                {
                    cboDetTipoDocT.Items.Add(ColumnKey);
                    cboDetNroReg.Items.Add(ColumnKey);
                    cboDetNaturaleza.Items.Add(ColumnKey);
                    cboDetCodCliente.Items.Add(ColumnKey);
                    cboDetFechaRef.Items.Add(ColumnKey);
                    cboDetTipoDocRef.Items.Add(ColumnKey);
                    cboDetNroCuenta.Items.Add(ColumnKey);
                    cboDetImporteDolares.Items.Add(ColumnKey);
                    cboDetImporteSoles.Items.Add(ColumnKey);
                    cboDetTipoDoc.Items.Add(ColumnKey);
                    cboDetAnalisis.Items.Add(ColumnKey);
                    cboDetNroDocRef.Items.Add(ColumnKey);
                    cboDetNroDoc.Items.Add(ColumnKey);
                    cboDetDestino.Items.Add(ColumnKey);
                }
            }

            List<string> Columnas = cboDetTipoDocT.Items.Cast<string>().ToList();
            cboDetTipoDocT.SelectedIndex = Columnas.FindIndex(p => p == "ccom_3a"); // 37;
            cboDetNroReg.SelectedIndex = Columnas.FindIndex(p => p == "ncom_3a"); // 38;
            cboDetNaturaleza.SelectedIndex = Columnas.FindIndex(p => p == "codh_3a"); // 39;
            cboDetFechaRef.SelectedIndex = Columnas.FindIndex(p => p == "fref_3a"); // 0;
            cboDetTipoDocRef.SelectedIndex = Columnas.FindIndex(p => p == "cref_3a"); // 5;
            cboDetNroCuenta.SelectedIndex = Columnas.FindIndex(p => p == "cuen_3a"); // 2;
            cboDetImporteSoles.SelectedIndex = Columnas.FindIndex(p => p == "imp1_3a"); // 10;
            cboDetImporteDolares.SelectedIndex = Columnas.FindIndex(p => p == "imp2_3a"); // 14;
            cboDetTipoDoc.SelectedIndex = Columnas.FindIndex(p => p == "cdoc_3a"); // 15;
            cboDetAnalisis.SelectedIndex = Columnas.FindIndex(p => p == "nomb_3a"); // 22;
            cboDetNroDoc.SelectedIndex = Columnas.FindIndex(p => p == "ndoc_3a"); // 23;
            cboDetNroDocRef.SelectedIndex = Columnas.FindIndex(p => p == "nref_3a");
            cboDetCodCliente.SelectedIndex = Columnas.FindIndex(p => p == "fich_3a");
            cboDetDestino.SelectedIndex = Columnas.FindIndex(p => p == "flagd_3a");
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (cboCabTipoDocT.SelectedIndex != 0 && cboCabNroReg.SelectedIndex != 0 && cboDetTipoDocT.SelectedIndex != 0 && cboDetNroReg.SelectedIndex != 0 && cboDetNaturaleza.SelectedIndex != 0)
            {
                grdDataDetalles.Selected.Rows.Clear();
                string TipoDoc = grdData.ActiveRow.Cells[cboCabTipoDocT.Text].Value.ToString().Trim();
                string Serie = grdData.ActiveRow.Cells[cboCabNroReg.Text].Value.ToString().Trim();

                string DetTipoDoc = cboDetTipoDocT.Text;
                string DetSerie = cboDetNroReg.Text;
                string DetCorrelativo = cboDetNaturaleza.Text;

                List<UltraGridRow> ListaDetalles = grdDataDetalles.Rows.AsParallel().Where(p => p.Cells[DetTipoDoc].Value.ToString().Trim() == (TipoDoc) &&
                                                                                                p.Cells[DetSerie].Value.ToString().Trim() == (Serie) ).ToList();

                if (ListaDetalles.Count() > 0)
                {
                    grdDataDetalles.Selected.Rows.AddRange(ListaDetalles.ToArray());
                    ListaDetalles.FirstOrDefault().Activate();
                }
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Count() > 0 && cboCabFechaEmision.Text != "--Seleccionar--")
            {
                if (UltraMessageBox.Show("¿Seguro de Convertir la Fecha?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        string columnaFecha = cboCabFechaEmision.Text;
                        grdData.Rows.AsParallel().Where(p => p.Cells[columnaFecha].Value != null && !string.IsNullOrEmpty(p.Cells[columnaFecha].Value.ToString())).ToList()
                                    .ForEach(o => o.Cells[columnaFecha].Value = DateTime.FromOADate(double.Parse(o.Cells[columnaFecha].Value.ToString())));
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid)
            {
                if (!grdData.Rows.Any() || !grdDataDetalles.Rows.Any())
                {
                    UltraMessageBox.Show("Por favor importe los excel con las cabeceras y los detalles...");
                }
                else
                {
                    string ColumnaTipoDoc = cboCabTipoDocT.Text;
                    string ColumnaNroCuenta;
                    _Tesorerias = new List<tesoreriaDto>();
                    try
                    {
                        #region Procesar Cabeceras
                        string ColumnaFechaEmision = cboCabFechaEmision.Text;
                        List<UltraGridRow> Tesorerias = grdData.Rows.AsParallel().Where(p => p.Cells[ColumnaFechaEmision].Value.ToString() != string.Empty)
                                                                                 .OrderBy(x => x.Cells[ColumnaFechaEmision].Value.ToString()).ToList();

                        foreach (UltraGridRow Fila in Tesorerias)
                        {
                            tesoreriaDto Tesoreria = new tesoreriaDto();
                            try
                            {
                                decimal TCambio, totalDSoles, totalDDolares, totalHSoles, totalHDolares;
                                int IdMoneda, IdMedioPago, IdTipoDocumento, IdTipoMovimiento;
                                DateTime FechaRegistro = DateTime.Parse(Fila.Cells[cboCabFechaEmision.Text].Value.ToString());
                                Tesoreria.v_Periodo = FechaRegistro.Year.ToString("00");
                                Tesoreria.v_Mes = FechaRegistro.Month.ToString("00");
                                Tesoreria.t_FechaRegistro = FechaRegistro;
                                Tesoreria.d_TipoCambio = decimal.TryParse(Fila.Cells[cboCabTipoCambio.Text].Value.ToString(), out TCambio) ? TCambio : 0;
                                Tesoreria.v_Nombre = Fila.Cells[cboCabNombre.Text].Value.ToString().Trim();
                                Tesoreria.v_Glosa = Fila.Cells[cboCabGlosa.Text].Value.ToString().Trim();
                                Tesoreria.i_IdMoneda = int.TryParse(Fila.Cells[cboCabMoneda.Text].Value.ToString(), out IdMoneda) ? IdMoneda : 1;
                                Tesoreria.i_AplicaRetencion = 0;
                                Tesoreria.v_NroCuentaCajaBanco = Fila.Cells[cboCabCuenta.Text].Value.ToString().Trim();
                                Tesoreria.d_TotalDebe_Importe = Tesoreria.i_IdMoneda == 1 ? decimal.TryParse(Fila.Cells[cboCabTotDSoles.Text].Value.ToString(), out totalDSoles) ? totalDSoles : 0 : decimal.TryParse(Fila.Cells[cboCabTotDDolares.Text].Value.ToString(), out totalDDolares) ? totalDDolares : 0;
                                Tesoreria.d_TotalDebe_Cambio = Tesoreria.i_IdMoneda == 2 ? decimal.TryParse(Fila.Cells[cboCabTotDSoles.Text].Value.ToString(), out totalDSoles) ? totalDSoles : 0 : decimal.TryParse(Fila.Cells[cboCabTotDDolares.Text].Value.ToString(), out totalDDolares) ? totalDDolares : 0;
                                Tesoreria.d_TotalHaber_Importe = Tesoreria.i_IdMoneda == 1 ? decimal.TryParse(Fila.Cells[cboCabTotHSoles.Text].Value.ToString(), out totalHSoles) ? totalHSoles : 0 : decimal.TryParse(Fila.Cells[cboCabTotHDolares.Text].Value.ToString(), out totalHDolares) ? totalHDolares : 0;
                                Tesoreria.d_TotalHaber_Cambio = Tesoreria.i_IdMoneda == 2 ? decimal.TryParse(Fila.Cells[cboCabTotHSoles.Text].Value.ToString(), out totalHSoles) ? totalHSoles : 0 : decimal.TryParse(Fila.Cells[cboCabTotHDolares.Text].Value.ToString(), out totalHDolares) ? totalHDolares : 0;
                                Tesoreria.d_Diferencia_Cambio = 0;
                                Tesoreria.d_Diferencia_Importe = 0;
                                Tesoreria.i_IdMedioPago = !string.IsNullOrWhiteSpace(cboCabMedioPago.Text) && !cboCabMedioPago.Text.Equals("--Seleccionar--") ? int.TryParse(Fila.Cells[cboCabMedioPago.Text].Value.ToString().Trim(), out IdMedioPago) ? IdMedioPago : 1 : 1;
                                Tesoreria.i_IdTipoDocumento = int.TryParse(Fila.Cells[cboCabTipoDocT.Text].Value.ToString().Trim(), out IdTipoDocumento) ? IdTipoDocumento : -1;
                                Tesoreria.i_TipoMovimiento = int.TryParse(Fila.Cells[cboCabTipoMov.Text].Value.ToString().Trim(), out IdTipoMovimiento) ? IdTipoMovimiento == 21 ? (int)TipoMovimientoTesoreria.Ingreso : (int)TipoMovimientoTesoreria.Egreso : 1;
                                Tesoreria.NroRegistro = Fila.Cells[cboCabNroReg.Text].Value.ToString().Trim();
                                Tesoreria.RegistroKey = Tesoreria.i_IdTipoDocumento.ToString() + "-" + Tesoreria.NroRegistro;
                                Tesoreria.i_IdEstado = 1;
                                _Tesorerias.Add(Tesoreria);
                            }
                            catch (Exception ex)
                            {
                                UltraMessageBox.Show(ex.Message);
                                break;
                            }
                        }

                        #endregion

                        #region Procesar Detalles

                        
                        List<string> ProductosNoEncontrados = new List<string>();

                        ColumnaTipoDoc = cboDetTipoDocT.Text;
                        ColumnaNroCuenta = cboDetNroCuenta.Text;
                        bool FaltanRegistrarClientes = false;
                        string ClienteFaltante = "\n";
                        List<UltraGridRow> DetallesTesorerias = grdDataDetalles.Rows.Where(p => !string.IsNullOrEmpty(p.Cells[cboDetNroCuenta.Text].Value.ToString()) && p.Cells[cboDetDestino.Text].Value.ToString().Trim() != "1").ToList();

                        foreach (UltraGridRow Fila in DetallesTesorerias)
                        {
                            int IdTipoDocumento, IdTipoDocumentoRef;
                            decimal ImporteSoles, ImporteDolares;

                            tesoreriadetalleDto tesoreriadetalle = new tesoreriadetalleDto();
                            tesoreriadetalle.RegistroKey = Fila.Cells[cboDetTipoDocT.Text].Value.ToString() + "-" + Fila.Cells[cboDetNroReg.Text].Value.ToString().Trim();

                            tesoreriaDto Cabecera = _Tesorerias.SingleOrDefault(p => p.RegistroKey == tesoreriadetalle.RegistroKey);

                            if (Cabecera != null)
                            {
                                tesoreriadetalle.v_NroCuenta = Fila.Cells[cboDetNroCuenta.Text].Value.ToString();
                                tesoreriadetalle.i_IdTipoDocumento = int.TryParse(Fila.Cells[cboDetTipoDoc.Text].Value.ToString(), out IdTipoDocumento) ? IdTipoDocumento : -1;
                                tesoreriadetalle.i_IdTipoDocumentoRef = int.TryParse(Fila.Cells[cboDetTipoDocRef.Text].Value.ToString(), out IdTipoDocumentoRef) ? IdTipoDocumentoRef : -1;
                                tesoreriadetalle.v_Analisis = Fila.Cells[cboDetAnalisis.Text].Value.ToString();
                                tesoreriadetalle.v_Naturaleza = Fila.Cells[cboDetNaturaleza.Text].Value.ToString().Trim();
                                tesoreriadetalle.d_Importe = Cabecera.i_IdMoneda == 1 ? decimal.TryParse(Fila.Cells[cboDetImporteSoles.Text].Value.ToString(), out ImporteSoles) ? ImporteSoles : 0 : decimal.TryParse(Fila.Cells[cboDetImporteDolares.Text].Value.ToString(), out ImporteDolares) ? ImporteDolares : 0;
                                tesoreriadetalle.d_Cambio = Cabecera.i_IdMoneda == 2 ? decimal.TryParse(Fila.Cells[cboDetImporteSoles.Text].Value.ToString(), out ImporteSoles) ? ImporteSoles : 0 : decimal.TryParse(Fila.Cells[cboDetImporteDolares.Text].Value.ToString(), out ImporteDolares) ? ImporteDolares : 0;
                                tesoreriadetalle.t_Fecha = DateTime.Parse(Fila.Cells[cboDetFechaRef.Text].Value.ToString());
                                tesoreriadetalle.i_IdCentroCostos = "0";
                                string codAnexo = Fila.Cells[cboDetCodCliente.Text].Value.ToString().Trim();

                                if (!string.IsNullOrEmpty(codAnexo))
                                {
                                    var anexo = Clientes.FirstOrDefault(p => p.Value1.Trim() == Fila.Cells[cboDetCodCliente.Text].Value.ToString().Trim());

                                    if (anexo != null)
                                    {
                                        tesoreriadetalle.v_IdCliente = anexo.Id;
                                    }
                                    else
                                    {
                                        FaltanRegistrarClientes = true;
                                        ClienteFaltante = ClienteFaltante + "\n -" + Fila.Cells[cboDetCodCliente.Text].Value.ToString().Trim();
                                    }
                                }
                                tesoreriadetalle.CodAnexo = Fila.Cells[cboDetCodCliente.Text].Value.ToString().Trim();
                                tesoreriadetalle.v_NroDocumento = Fila.Cells[cboDetNroDoc.Text].Value.ToString().Trim();
                                tesoreriadetalle.v_NroDocumentoRef = Fila.Cells[cboDetNroDocRef.Text].Value.ToString().Trim();
                                tesoreriadetalles.Add(tesoreriadetalle);
                            }
                        }

                        if (FaltanRegistrarClientes)
                        {
                          UltraMessageBox.Show(string.Format("Hay clientes no registrados por favor registrelos primero. {0}", ClienteFaltante));
                          return;
                        }
                        #endregion

                        #region Guardar

                        #region Separa las cabeceras que cuentan con detalle para procesarlas
                        TesoreriasConDetalle = new List<tesoreriaDto>();

                        var tesoreriadetallesKeys = tesoreriadetalles.Select(p => p.RegistroKey).Distinct().ToList();

                        foreach (tesoreriaDto Tesoreria in _Tesorerias.AsParallel())
                        {
                            if (tesoreriadetallesKeys.Contains(Tesoreria.RegistroKey))
                            {
                                var xx = tesoreriadetalles.Where(p => !p.CodAnexo.StartsWith("T") && p.RegistroKey == Tesoreria.RegistroKey).Count();
                                if (xx > 0) //<-- agregado pro mientras hasta implementar trabajadores
                                {
                                    TesoreriasConDetalle.Add(Tesoreria);
                                }
                                
                            }
                        }
                        #endregion

                        #region Asigna los correlativos de las Tesorerias filtradas de acuerdo a su mes
                        List<string> Meses = _Tesorerias.Select(p => p.t_FechaRegistro.Value.Month.ToString()).Distinct().ToList();
                        List<int> TiposDocumentos = _Tesorerias.Select(p => p.i_IdTipoDocumento.Value).Distinct().ToList();
                        foreach (string Mes in Meses)
                        {
                            foreach (var IdDocumento in TiposDocumentos)
                            {
                                int reg = 1;
                                var TesoreriasDelMes = TesoreriasConDetalle.Where(p => p.t_FechaRegistro.Value.Month.ToString() == Mes && p.i_IdTipoDocumento == IdDocumento).ToList();
                                foreach (var TesoreriaM in TesoreriasDelMes)
                                {
                                    TesoreriaM.v_Correlativo = reg.ToString("00000000");
                                    reg++;
                                }
                            }
                        }
                        #endregion

                        (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();
                        bwkProceso.RunWorkerAsync();
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                        return;
                    }
                }
            }
        }

        private void ultraButton3_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Count() > 0 && cboCabTotHSoles.Text != "--Seleccionar--")
            {
                if (UltraMessageBox.Show("¿Seguro de Convertir la Fecha?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        grdData.Rows.Where(p => p.Cells[cboCabTotHSoles.Text].Value != null && !string.IsNullOrEmpty(p.Cells[cboCabTotHSoles.Text].Value.ToString()) && p.Cells[cboCabTotHSoles.Text].Value.ToString().Trim() != "/  /").ToList()
                                    .ForEach(o => o.Cells[cboCabTotHSoles.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboCabTotHSoles.Text].Value.ToString())));
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }
                }
            }
        }


        private void bwkProceso_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Empieza el guardado de las Tesorerias filtradas
            Globals.ProgressbarStatus.i_TotalProgress = TesoreriasConDetalle.Count;
            foreach (tesoreriaDto Tesoreria in TesoreriasConDetalle.AsParallel())
            {
                try
                {
                    OperationResult objOperationResult = new OperationResult();
                    List<tesoreriadetalleDto> DetallesDeCabecera = tesoreriadetalles.Where(p => p.RegistroKey == Tesoreria.RegistroKey).ToList();

                    new TesoreriaBL().Insertartesoreria(ref objOperationResult, Tesoreria, Globals.ClientSession.GetAsList(), DetallesDeCabecera);

                    if (objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    Globals.ProgressbarStatus.i_Progress++;
                }
                catch (Exception ex)
                {
                    UltraMessageBox.Show(ex.Message);
                    return;
                }

            }
            UltraMessageBox.Show("Importación Finalizada", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            #endregion
        }

        private void ultraGroupBox3_Click(object sender, EventArgs e)
        {

        }

        private void ultraButton3_Click_1(object sender, EventArgs e)
        {
            if (grdDataDetalles.Rows.Any() && cboDetFechaRef.Text != "--Seleccionar--")
            {
                if (UltraMessageBox.Show("¿Seguro de Convertir la Fecha?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        grdDataDetalles.Rows.Where(p => p.Cells[cboDetFechaRef.Text].Value != null && !string.IsNullOrEmpty(p.Cells[cboDetFechaRef.Text].Value.ToString()) && p.Cells[cboDetFechaRef.Text].Value.ToString().Trim() != "/  /").ToList()
                                    .ForEach(o =>
                                    {
                                        try
                                        {
                                            o.Cells[cboDetFechaRef.Text].Value =
                                            DateTime.FromOADate(
                                                double.Parse(o.Cells[cboDetFechaRef.Text].Value.ToString()));
                                        }
                                        catch (Exception)
                                        {
                                            o.Cells[cboDetFechaRef.Text].Value = new DateTime(2016,01,01);
                                        }
                                        
                                    });
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }
                }
            }
        }

    }
}
