using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using LoadingClass;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Compra.BL;
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
    public partial class frmMigrarCompras : Form
    {
        ClienteBL _objClienteBL = new ClienteBL();
        compraDto _compraDto = new compraDto();
        ProductoBL _objProductoBL = new ProductoBL();
        List<compraDto> VentasConDetalle = new List<compraDto>();
        List<compraDto> Compras = new List<compraDto>();
        List<compradetalleDto> compradetalles = new List<compradetalleDto>();

        public frmMigrarCompras(string N)
        {
            InitializeComponent();
        }

        private void frmMigrarCompras_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel3.BackColor = new GlobalFormColors().FormColor;
            cboCabTipoDoc.SelectedIndex = 0;
            cboCabSerie.SelectedIndex = 0;
            cboCabCorrelativo.SelectedIndex = 0;
            cboCabCodCliente.SelectedIndex = 0;
            cboCabGlosa.SelectedIndex = 0;
            cboCabFechaEmision.SelectedIndex = 0;
            cboCabDestino.SelectedIndex = 0;
            cboCabTipoCambio.SelectedIndex = 0;
            cboDetTipoDoc.SelectedIndex = 0;
            cboDetSerie.SelectedIndex = 0;
            cboDetCorrelativo.SelectedIndex = 0;
            cboDetCodigoProd.SelectedIndex = 0;
            cboDetNroPedido.SelectedIndex = 0;
            cboDetUM.SelectedIndex = 0;
            cboDetCantidad.SelectedIndex = 0;
            cboDetNroCuenta.SelectedIndex = 0;
            cboDetGlosa.SelectedIndex = 0;
            cboDetTotalSoles.SelectedIndex = 0;
            cboDetTotalDolares.SelectedIndex = 0;
            cboCabMoneda.SelectedIndex = 0;
            cboCabIncluyeIGV.SelectedIndex = 0;
            cboCabAfectoIGV.SelectedIndex = 0;
            cboDetDestino.SelectedIndex = 0;
            cboCabFechaDetraccion.SelectedIndex = 0;
            cboCabDetraccion.SelectedIndex = 0;
            cboCabCodDetraccion.SelectedIndex = 0;
            cboCabNroDetraccion.SelectedIndex = 0;
            cboCabPorcientoDetraccion.SelectedIndex = 0;
            cboDetValorDolaresDetraccion.SelectedIndex = 0;
            cboDetValorSolesDetraccion.SelectedIndex = 0;
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
                    cboCabTipoDoc.Items.Add(ColumnKey);
                    cboCabSerie.Items.Add(ColumnKey);
                    cboCabCorrelativo.Items.Add(ColumnKey);
                    cboCabCodCliente.Items.Add(ColumnKey);
                    cboCabGlosa.Items.Add(ColumnKey);
                    cboCabFechaEmision.Items.Add(ColumnKey);
                    cboCabDestino.Items.Add(ColumnKey);
                    cboCabMoneda.Items.Add(ColumnKey);
                    cboCabTipoCambio.Items.Add(ColumnKey);
                    cboCabAfectoIGV.Items.Add(ColumnKey);
                    cboCabIncluyeIGV.Items.Add(ColumnKey);
                    cboCabDetraccion.Items.Add(ColumnKey);
                    cboCabNroDetraccion.Items.Add(ColumnKey);
                    cboCabFechaDetraccion.Items.Add(ColumnKey);
                    cboCabPorcientoDetraccion.Items.Add(ColumnKey);
                    cboCabCodDetraccion.Items.Add(ColumnKey);
                    cboCabNroRegistroCabecera.Items.Add(ColumnKey);
                    cboCabFechaRegistro.Items.Add(ColumnKey);
                }
            }
            List<string> Columnas = cboCabTipoDoc.Items.Cast<string>().ToList();

            cboCabTipoDoc.SelectedIndex = Columnas.FindIndex(p => p == "TIDOC_3"); //4;
            cboCabSerie.SelectedIndex = Columnas.FindIndex(p => p == "SFACTU_3"); //5;
            cboCabCorrelativo.SelectedIndex = Columnas.FindIndex(p => p == "NFACTU_3"); //6;
            cboCabCodCliente.SelectedIndex = Columnas.FindIndex(p => p == "FICH_3"); //2;
            cboCabGlosa.SelectedIndex = Columnas.FindIndex(p => p == "GLOA_3"); //19;
            cboCabFechaEmision.SelectedIndex = Columnas.FindIndex(p => p == "FCOME_3"); //7;
            cboCabFechaRegistro.SelectedIndex = Columnas.FindIndex(p => p == "FCOM_3"); //7;
            cboCabDestino.SelectedIndex = Columnas.FindIndex(p => p == "DEST_3"); //18;
            cboCabMoneda.SelectedIndex = Columnas.FindIndex(p => p == "MONE_3"); //10;
            cboCabTipoCambio.SelectedIndex = Columnas.FindIndex(p => p == "TCAM_3"); //11;
            cboCabAfectoIGV.SelectedIndex = Columnas.FindIndex(p => p == "AIGV_3"); //24;
            cboCabIncluyeIGV.SelectedIndex = Columnas.FindIndex(p => p == "IIGV_3"); //25;
            cboCabDetraccion.SelectedIndex = Columnas.FindIndex(p => p == "DERETE_3"); //40;
            cboCabNroDetraccion.SelectedIndex = Columnas.FindIndex(p => p == "DENREG_3"); //41;
            cboCabFechaDetraccion.SelectedIndex = Columnas.FindIndex(p => p == "DEFCOM_3"); //42;
            cboCabPorcientoDetraccion.SelectedIndex = Columnas.FindIndex(p => p == "VDERETE_3"); //45;
            cboCabCodDetraccion.SelectedIndex = Columnas.FindIndex(p => p == "DETRA_3"); //49;
            cboCabNroRegistroCabecera.SelectedIndex = Columnas.FindIndex(p => p.Equals("NCOM_3"));

        }

        private void btnImportarDetalle_Click(object sender, EventArgs e)
        {
            if (grdDataDetalles.Rows.Any())
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
                    cboDetTipoDoc.Items.Add(ColumnKey);
                    cboDetSerie.Items.Add(ColumnKey);
                    cboDetCorrelativo.Items.Add(ColumnKey);
                    cboDetCodigoProd.Items.Add(ColumnKey);
                    cboDetNroPedido.Items.Add(ColumnKey);
                    cboDetUM.Items.Add(ColumnKey);
                    cboDetCantidad.Items.Add(ColumnKey);
                    cboDetNroCuenta.Items.Add(ColumnKey);
                    cboDetGlosa.Items.Add(ColumnKey);
                    cboDetTotalDolares.Items.Add(ColumnKey);
                    cboDetTotalSoles.Items.Add(ColumnKey);
                    cboDetDestino.Items.Add(ColumnKey);
                    cboDetValorDolaresDetraccion.Items.Add(ColumnKey);
                    cboDetValorSolesDetraccion.Items.Add(ColumnKey);
                    cboDetNroRegistroCabecera.Items.Add(ColumnKey);
                }
            }

            List<string> Columnas = cboDetTipoDoc.Items.Cast<string>().ToList();
            cboDetTipoDoc.SelectedIndex = Columnas.FindIndex(p => p == "TDOC_3A"); // 37;
            cboDetSerie.SelectedIndex = Columnas.FindIndex(p => p == "SERIE_3A"); // 38;
            cboDetCorrelativo.SelectedIndex = Columnas.FindIndex(p => p == "NRO_3A"); // 39;
            cboDetCodigoProd.SelectedIndex = Columnas.FindIndex(p => p == "LINEA_3A"); // 4;
            cboDetNroPedido.SelectedIndex = Columnas.FindIndex(p => p == "NPEDI_3A"); // 24;
            cboDetUM.SelectedIndex = Columnas.FindIndex(p => p == "UNID_3A"); // 0;
            cboDetCantidad.SelectedIndex = Columnas.FindIndex(p => p == "CANT_3A"); // 5;
            cboDetNroCuenta.SelectedIndex = Columnas.FindIndex(p => p == "CUEN_3A"); // 2;
            cboDetGlosa.SelectedIndex = Columnas.FindIndex(p => p == "GLOSA_3A"); // 36;
            cboDetTotalSoles.SelectedIndex = Columnas.FindIndex(p => p == "TOT1_3A"); // 10;
            cboDetTotalDolares.SelectedIndex = Columnas.FindIndex(p => p == "TOT2_3A"); // 14;
            cboDetDestino.SelectedIndex = Columnas.FindIndex(p => p == "DEST_3A"); // 15;
            cboDetValorDolaresDetraccion.SelectedIndex = Columnas.FindIndex(p => p == "VDETRA2_3A"); // 22;
            cboDetValorSolesDetraccion.SelectedIndex = Columnas.FindIndex(p => p == "VDETRA1_3A"); // 23;
            cboDetNroRegistroCabecera.SelectedIndex = Columnas.FindIndex(p => p == "NCOM_3A"); // 23;
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (cboCabTipoDoc.SelectedIndex != 0 && cboCabSerie.SelectedIndex != 0 && cboCabCorrelativo.SelectedIndex != 0 && cboDetTipoDoc.SelectedIndex != 0 && cboDetSerie.SelectedIndex != 0 && cboDetCorrelativo.SelectedIndex != 0)
            {
                grdDataDetalles.Selected.Rows.Clear();
                string TipoDoc = grdData.ActiveRow.Cells[cboCabNroRegistroCabecera.Text].Value.ToString().Trim();

                string DetTipoDoc = cboDetNroRegistroCabecera.Text;

                List<UltraGridRow> ListaDetalles = grdDataDetalles.Rows.AsParallel().Where(p => p.Cells[DetTipoDoc].Value.ToString().Trim() == (TipoDoc)).ToList();

                if (ListaDetalles.Any())
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
                        grdData.Rows.Where(p => p.Cells[cboCabFechaEmision.Text].Value != null && !string.IsNullOrEmpty(p.Cells[cboCabFechaEmision.Text].Value.ToString())).ToList()
                                    .ForEach(o => o.Cells[cboCabFechaEmision.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboCabFechaEmision.Text].Value.ToString())));
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
                    List<KeyValueDTO> Proveedores = _objClienteBL.DevuelveProveedores();

                    string ColumnaTipoDoc = cboCabTipoDoc.Text;

                    try
                    {
                        #region Procesar Cabeceras
                        string ColumnaFechaEmision = cboCabFechaEmision.Text;
                        List<UltraGridRow> BoletasYFacturas = grdData.Rows.AsParallel().Where(p => p.Cells[ColumnaFechaEmision].Value.ToString().Trim() != string.Empty)
                                                                .OrderBy(x => x.Cells[ColumnaFechaEmision].Value.ToString()).ToList();

                        foreach (UltraGridRow Fila in BoletasYFacturas)
                        {
                            compraDto Compra = new compraDto();
                            try
                            {
                                DateTime FechaEmision = DateTime.Parse(Fila.Cells[cboCabFechaEmision.Text].Value.ToString());
                                DateTime FechaRegistro = DateTime.Parse(Fila.Cells[cboCabFechaRegistro.Text].Value.ToString());
                                Compra.v_Periodo = FechaRegistro.Year.ToString("00");
                                Compra.v_Mes = FechaRegistro.Month.ToString("00");
                                Compra.i_IdIgv = 1; //Actual 18%
                                Compra.i_IdTipoCompra = Fila.Cells[cboCabMoneda.Text].Value != null ? int.Parse(Fila.Cells[cboCabMoneda.Text].Value.ToString()) : 1;
                                Compra.i_IdTipoDocumento = int.Parse(Fila.Cells[ColumnaTipoDoc].Value.ToString());
                                Compra.v_SerieDocumento = Fila.Cells[cboCabSerie.Text].Value.ToString().Trim().ToUpper ();
                                Compra.v_CorrelativoDocumento = int.Parse(Fila.Cells[cboCabCorrelativo.Text].Value.ToString()).ToString("00000000").ToUpper ();
                                Compra.i_IdTipoDocumentoRef = !string.IsNullOrWhiteSpace(Fila.Cells["TIDOCR_3"].Value.ToString()) ? int.Parse(Fila.Cells["TIDOCR_3"].Value.ToString()) : -1;
                                Compra.v_SerieDocumentoRef = !string.IsNullOrWhiteSpace(Fila.Cells["TIDOCR_3"].Value.ToString()) ? Fila.Cells["SFACTUR_3"].Value.ToString().Trim() : string.Empty;
                                Compra.v_CorrelativoDocumentoRef = !string.IsNullOrWhiteSpace(Fila.Cells["TIDOCR_3"].Value.ToString()) ? Fila.Cells["NFACTUR_3"].Value.ToString().Trim() : string.Empty; ;
                                Compra.t_FechaRef = FechaRegistro;
                                Compra.v_Glosa = Fila.Cells[cboCabGlosa.Text].Value.ToString();
                                Compra.t_FechaEmision = FechaEmision;
                                Compra.i_EsDetraccion = int.Parse(Fila.Cells[cboCabDetraccion.Text].Value.ToString().Trim()) == 1 ? 1 : 0;

                                if (Compra.i_EsDetraccion == 1)
                                {
                                    Compra.i_CodigoDetraccion = Fila.Cells[cboCabCodDetraccion.Text].Value.ToString().Trim().Length >= 1 ? int.Parse(Fila.Cells[cboCabCodDetraccion.Text].Value.ToString().Trim().Substring(1, 2)) : 0;
                                    Compra.t_FechaDetraccion = Fila.Cells[cboCabFechaDetraccion.Text].Value.ToString().Trim() != "/  /" ? DateTime.Parse(Fila.Cells[cboCabFechaDetraccion.Text].Value.ToString().Trim()) : DateTime.Today;
                                    Compra.v_NroDetraccion = Fila.Cells[cboCabNroDetraccion.Text].Value.ToString().Trim();
                                    Compra.d_PorcentajeDetraccion = int.Parse(Fila.Cells[cboCabPorcientoDetraccion.Text].Value.ToString().Trim());
                                }
                                else
                                {
                                    Compra.i_CodigoDetraccion = 0;
                                    Compra.t_FechaDetraccion = FechaRegistro;
                                    Compra.v_NroDetraccion = string.Empty;
                                    Compra.d_PorcentajeDetraccion = 0;
                                }

                                string CodCliente = Fila.Cells[cboCabCodCliente.Text].Value.ToString().Trim();

                                if (!string.IsNullOrEmpty(CodCliente))
                                {
                                    KeyValueDTO Cliente = Proveedores.FirstOrDefault(p => p.Value1.Trim() == CodCliente);

                                    if (Cliente != null)
                                    {
                                        Compra.v_IdProveedor = Cliente.Id;
                                        Compra.NombreProveedor = Cliente.Value3;
                                    }
                                    else
                                        Compra.v_IdProveedor = "N002-CL000000000";

                                }
                                else
                                    Compra.v_IdProveedor = "N002-CL000000000";


                                Compra.t_FechaRegistro = FechaRegistro;
                                Compra.d_TipoCambio = Fila.Cells[cboCabTipoCambio.Text].Value != null ? decimal.Parse(Fila.Cells[cboCabTipoCambio.Text].Value.ToString()) : 3;
                                Compra.t_FechaVencimiento = FechaRegistro;
                                Compra.i_IdCondicionPago = 1;
                                Compra.i_IdEstado = 1;
                                Compra.i_EsAfectoIgv = int.Parse(Fila.Cells[cboCabAfectoIGV.Text].Value.ToString());
                                Compra.i_PreciosIncluyenIgv = int.Parse(Fila.Cells[cboCabIncluyeIGV.Text].Value.ToString()); ;
                                Compra.d_Anticipio = 0;
                                Compra.i_IdDestino = int.Parse(Fila.Cells[cboCabDestino.Text].Value.ToString());
                                Compra.i_DeduccionAnticipio = 0;
                                Compra.i_IdEstablecimiento = Globals.ClientSession.i_IdEstablecimiento.Value;
                                Compra.i_IdMoneda = Fila.Cells[cboCabMoneda.Text].Value != null ? int.Parse(Fila.Cells[cboCabMoneda.Text].Value.ToString()) : 1;
                                Compra.RegistroKey = Fila.Cells[cboCabNroRegistroCabecera.Text].Value.ToString().Trim();
                                Compras.Add(Compra);
                            }
                            catch (Exception ex)
                            {
                                UltraMessageBox.Show(ex.Message + " " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                            }
                        }

                        #endregion

                        #region Procesar Detalles

                        List<KeyValueDTO> Productos = _objProductoBL.DevolverProductos();
                        List<string> ProductosNoEncontrados = new List<string>();

                        ColumnaTipoDoc = cboDetTipoDoc.Text;
                        var ColumnaNroCuenta = cboDetNroCuenta.Text;

                        List<UltraGridRow> DetallesBoletasYFacturas = grdDataDetalles.Rows.Where(p => !string.IsNullOrEmpty(p.Cells[cboDetNroCuenta.Text].Value.ToString())).ToList();

                        foreach (UltraGridRow Fila in DetallesBoletasYFacturas)
                        {
                            string CodProducto = Fila.Cells[cboDetCodigoProd.Text].Value.ToString().Trim();
                            decimal Cantidad, Precio;

                            compradetalleDto compradetalle = new compradetalleDto();
                            compradetalle.i_IdAlmacen = (int?)Globals.ClientSession.i_IdAlmacenPredeterminado.Value;
                            compradetalle.v_NroCuenta = Fila.Cells[ColumnaNroCuenta].Value.ToString().Trim();
                            compradetalle.i_Anticipio = 0;
                            compradetalle.v_Glosa = Fila.Cells[cboDetGlosa.Text].Value.ToString();
                            compradetalle.v_NroPedido = Fila.Cells[cboDetNroPedido.Text].Value.ToString();

                            if (!string.IsNullOrEmpty(CodProducto))
                            {
                                if (Productos.Select(p => p.Value1.Trim()).ToList().Contains(CodProducto))
                                {
                                    compradetalle.v_IdProductoDetalle = Productos.FirstOrDefault(p => p.Value1 == CodProducto).Value2;
                                }
                                else
                                {
                                    if (!ProductosNoEncontrados.Contains(CodProducto))
                                    {
                                        ProductosNoEncontrados.Add(CodProducto);
                                    }
                                }
                            }

                            Cantidad = decimal.Parse(Fila.Cells[cboDetCantidad.Text].Value.ToString());
                            compradetalle.d_Cantidad = Cantidad;

                            compradetalle.d_PrecioVenta = decimal.Parse(Fila.Cells[cboDetTotalSoles.Text].Value.ToString());
                            compradetalle.TotalDolares = decimal.Parse(Fila.Cells[cboDetTotalDolares.Text].Value.ToString());

                            compradetalle.i_IdUnidadMedida = /*cboDetUM.Text != "--Seleccionar--" ? Fila.Cells[cboDetUM.Text].Value != null && !string.IsNullOrEmpty(Fila.Cells[cboDetUM.Text].Value.ToString()) ? int.Parse(Fila.Cells[cboDetUM.Text].Value.ToString()) : 15 : */15;
                            compradetalle.SerieDocumento = Fila.Cells[cboDetSerie.Text].Value.ToString().Trim();
                            compradetalle.CorrelativoDocumento = Fila.Cells[cboDetCorrelativo.Text].Value.ToString();
                            compradetalle.TipoDocumento = int.Parse(Fila.Cells[cboDetTipoDoc.Text].Value.ToString());
                            compradetalle.i_IdCentroCostos = "";
                            compradetalle.i_IdDestino = !string.IsNullOrEmpty(Fila.Cells[cboDetDestino.Text].Value.ToString().Trim()) ? int.Parse(Fila.Cells[cboDetDestino.Text].Value.ToString()) : 0;
                            compradetalle.d_ValorDolaresDetraccion = decimal.Parse(Fila.Cells[cboDetValorDolaresDetraccion.Text].Value.ToString().Trim());
                            compradetalle.d_ValorSolesDetraccion = decimal.Parse(Fila.Cells[cboDetValorSolesDetraccion.Text].Value.ToString().Trim());
                            compradetalle.RegistroKey = Fila.Cells[cboDetNroRegistroCabecera.Text].Value.ToString().Trim();
                            compradetalles.Add(compradetalle);
                        }

                        if (ProductosNoEncontrados.Count() != 0)
                        {
                            var resp = UltraMessageBox.Show(string.Format("Hay {0} productos no registrados, ¿Desea Continuar?", ProductosNoEncontrados.Distinct().Count()), "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            var productos = string.Join(", ", ProductosNoEncontrados);
                            Clipboard.SetText(productos);
                            if (resp == DialogResult.No) return;
                        }
                        #endregion

                        #region Guardar

                        #region Separa las cabeceras que cuentan con detalle para procesarlas
                        VentasConDetalle = new List<compraDto>();

                        var compradetallesKeys = compradetalles.Select(p => p.RegistroKey).Distinct().ToList();

                        foreach (compraDto Venta in Compras.AsParallel())
                        {
                            if (compradetallesKeys.Contains(Venta.RegistroKey))
                            {
                                VentasConDetalle.Add(Venta);
                            }
                        }
                        #endregion

                        #region Asigna los correlativos de las ventas filtradas de acuerdo a su mes
                        List<string> Meses = Compras.Select(p => p.t_FechaRegistro.Value.Month.ToString()).Distinct().ToList();
                        foreach (string Mes in Meses)
                        {
                            int reg = 1;
                            var VentasDelMes = VentasConDetalle.Where(p => p.t_FechaRegistro.Value.Month.ToString() == Mes).ToList();
                            foreach (var VentaM in VentasDelMes)
                            {
                                VentaM.v_Correlativo = reg.ToString("00000000");
                                reg++;
                            }
                        }
                        #endregion
                        (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();
                        bwkProceso.RunWorkerAsync();
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message + " " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                        return;
                    }
                }
            }
        }

        private void ultraButton3_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Any() && cboCabFechaDetraccion.Text != "--Seleccionar--")
            {
                if (UltraMessageBox.Show("¿Seguro de Convertir la Fecha?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        grdData.Rows.Where(p => p.Cells[cboCabFechaDetraccion.Text].Value != null && !string.IsNullOrEmpty(p.Cells[cboCabFechaDetraccion.Text].Value.ToString()) && p.Cells[cboCabFechaDetraccion.Text].Value.ToString().Trim() != "/  /").ToList()
                                    .ForEach(o => o.Cells[cboCabFechaDetraccion.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboCabFechaDetraccion.Text].Value.ToString())));
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message + " " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                    }
                }
            }
        }

        private void GeneraNotaIngreso(compraDto _compraDto, List<compradetalleDto> _compradetalleDto)
        {
            OperationResult objOperationResult = new OperationResult();
            movimientoDto _movimientoDto = new movimientoDto();
            List<string> Almacenes = new List<string>();
            List<movimientodetalleDto> __TempDetalle_AgregarDto = new List<movimientodetalleDto>();
            string pstrAlmacen;
            bool _EsNotadeCredito = _compraDto.i_IdTipoDocumento == 7 ? true : false;

            foreach (compradetalleDto Fila in _compradetalleDto)
            {
                if (Fila.v_IdProductoDetalle != null)
                {
                    pstrAlmacen = Fila.i_IdAlmacen.ToString();
                    if (!Almacenes.Contains(pstrAlmacen))
                    {
                        Almacenes.Add(pstrAlmacen);
                    }
                }
            }

            List<KeyValueDTO> ListaMovimientos = new List<KeyValueDTO>();
            ListaMovimientos = new MovimientoBL().ObtenerListadoMovimientos(ref objOperationResult, _compraDto.v_Periodo, _compraDto.v_Mes, (int)TipoDeMovimiento.NotadeIngreso);

            int MaxMovimiento;
            MaxMovimiento = ListaMovimientos.Count() > 0 ? int.Parse(ListaMovimientos[ListaMovimientos.Count() - 1].Value1.ToString()) : 0;

            foreach (String Almacen in Almacenes)
            {
                MaxMovimiento++;
                _movimientoDto.d_TipoCambio = _compraDto.d_TipoCambio;
                _movimientoDto.i_IdAlmacenOrigen = int.Parse(Almacen);
                _movimientoDto.i_IdMoneda = _compraDto.i_IdMoneda;
                _movimientoDto.i_IdTipoMotivo = 1;//Compra nacional
                _movimientoDto.t_Fecha = _compraDto.t_FechaRegistro;
                _movimientoDto.v_Glosa = _compraDto.v_Glosa;
                _movimientoDto.v_Mes = _compraDto.v_Mes;
                _movimientoDto.v_Periodo = _compraDto.v_Periodo;
                _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                _movimientoDto.v_Correlativo = MaxMovimiento.ToString("00000000");
                _movimientoDto.v_IdCliente = _compraDto.v_IdProveedor;
                _movimientoDto.v_OrigenTipo = "C";
                _movimientoDto.i_EsDevolucion = _EsNotadeCredito == true ? 1 : 0;
                _movimientoDto.v_OrigenRegCorrelativo = _compraDto.v_Correlativo;
                _movimientoDto.v_OrigenRegMes = _compraDto.v_Mes;
                _movimientoDto.v_OrigenRegPeriodo = _compraDto.v_Periodo;
                _movimientoDto.d_TotalPrecio = _compraDto.d_Total;

                _movimientoDto.v_Glosa = _compraDto.v_Glosa;
                _movimientoDto.v_NroOrdenCompra = _compraDto.v_ODCSerie + "-" + _compraDto.v_ODCCorrelativo;
                if (_EsNotadeCredito == true)
                {
                    for (int i = 0; i < grdData.Rows.Count(); i++)
                    {
                        grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
                    }
                }

                foreach (var Detalle in _compradetalleDto.Where(p => p.i_IdAlmacen == Int32.Parse(Almacen)))
                {
                    movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
                    _movimientodetalleDto.v_IdMovimiento = _movimientoDto.v_IdMovimiento;
                    _movimientodetalleDto.v_IdProductoDetalle = Detalle.v_IdProductoDetalle;
                    _movimientodetalleDto.v_NroGuiaRemision = Detalle.v_NroGuiaRemision;
                    _movimientodetalleDto.i_IdTipoDocumento = _compraDto.i_IdTipoDocumento.Value;
                    _movimientodetalleDto.v_NumeroDocumento = _compraDto.v_SerieDocumento + "-" + _compraDto.v_CorrelativoDocumento;
                    _movimientodetalleDto.d_Cantidad = Detalle.d_Cantidad.Value;
                    _movimientodetalleDto.i_IdUnidad = Detalle.i_IdUnidadMedida.Value;
                    _movimientodetalleDto.d_Precio = Detalle.d_Precio;
                   // _movimientodetalleDto.d_Total = Detalle.d_Cantidad.Value * Detalle.d_Precio;
                    _movimientodetalleDto.d_Total = Utils.Windows.DevuelveValorRedondeado(Detalle.d_PrecioVenta ?? 0, 2);
                    _movimientodetalleDto.d_CantidadEmpaque = Detalle.d_Cantidad.Value;
                    _movimientodetalleDto.v_NroPedido = Detalle.v_NroPedido;
                    __TempDetalle_AgregarDto.Add(_movimientodetalleDto);
                }

                _movimientoDto.d_TotalCantidad = __TempDetalle_AgregarDto.Sum(p => p.d_Cantidad.Value);
                _movimientoDto.d_TotalPrecio = __TempDetalle_AgregarDto.Sum(p => p.d_Total.Value);
                new MovimientoBL().InsertarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), __TempDetalle_AgregarDto);

                if (objOperationResult.Success == 0)
                {
                    UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                __TempDetalle_AgregarDto = new List<movimientodetalleDto>();
            }
        }

        private void bwkProceso_DoWork(object sender, DoWorkEventArgs e)
        {
            #region Empieza el guardado de las ventas filtradas
            var listaComprasProcesadas = new List<string>();
            Globals.ProgressbarStatus.i_TotalProgress = VentasConDetalle.Count;

            foreach (compraDto Compra in VentasConDetalle)
            {
                try
                {
                    var objOperationResult = new OperationResult();
                    var DetallesDeCabecera = compradetalles.Where(p => p.RegistroKey.Equals(Compra.RegistroKey)).ToList();

                    foreach (var Detalle in DetallesDeCabecera)
                    {
                        if (Compra.i_IdMoneda == 1)
                        {
                            if (Compra.i_EsAfectoIgv == 1)
                            {
                                if (Detalle.i_IdDestino != 4)
                                {
                                    Detalle.d_ValorVenta = (decimal?)(double.Parse(Detalle.d_PrecioVenta.Value.ToString()) / 1.18);
                                    Detalle.d_Igv = Detalle.d_PrecioVenta.Value - Detalle.d_ValorVenta.Value;
                                    Detalle.d_Precio = Detalle.d_Cantidad != 0 ? (Detalle.d_PrecioVenta - Detalle.d_Igv) / Detalle.d_Cantidad : 0;
                                }
                                else
                                {
                                    Detalle.d_ValorVenta = Detalle.d_PrecioVenta ?? 0;
                                    Detalle.d_Igv = 0;
                                    Detalle.d_Precio = Detalle.d_Cantidad != 0 ? (Detalle.d_PrecioVenta - Detalle.d_Igv) / Detalle.d_Cantidad : 0;
                                }
                            }
                            else
                            {
                                Detalle.d_ValorVenta = Detalle.d_PrecioVenta;
                                Detalle.d_Igv = 0;
                                Detalle.d_Precio = Detalle.d_Cantidad != 0 ? (Detalle.d_PrecioVenta - Detalle.d_Igv) / Detalle.d_Cantidad : 0;
                            }
                        }
                        else
                        {
                            if (Compra.i_EsAfectoIgv == 1)
                            {
                                if (Detalle.i_IdDestino != 4)
                                {
                                    Detalle.d_PrecioVenta = Detalle.TotalDolares;
                                    Detalle.d_ValorVenta = (decimal?) (double.Parse(Detalle.d_PrecioVenta.Value.ToString())/1.18);
                                    Detalle.d_Igv = Detalle.d_PrecioVenta.Value - Detalle.d_ValorVenta.Value;
                                    Detalle.d_Precio = Detalle.d_Cantidad != 0 ? (Detalle.d_PrecioVenta - Detalle.d_Igv)/Detalle.d_Cantidad : 0;
                                }
                                else
                                {
                                    Detalle.d_PrecioVenta = Detalle.TotalDolares;
                                    Detalle.d_ValorVenta = Detalle.d_PrecioVenta ?? 0;
                                    Detalle.d_Igv = 0;
                                    Detalle.d_Precio = Detalle.d_Cantidad != 0 ? (Detalle.d_PrecioVenta - Detalle.d_Igv) / Detalle.d_Cantidad : 0;
                                }
                            }
                            else
                            {
                                Detalle.d_PrecioVenta = Detalle.TotalDolares;
                                Detalle.d_ValorVenta = Detalle.d_PrecioVenta;
                                Detalle.d_Igv = 0;
                                Detalle.d_Precio = Detalle.d_Cantidad != 0 ? (Detalle.d_PrecioVenta - Detalle.d_Igv) / Detalle.d_Cantidad : 0;
                            }
                        }
                    }

                    Compra.d_Total = DetallesDeCabecera.Sum(p => p.d_PrecioVenta.Value);
                    Compra.d_ValorVenta = DetallesDeCabecera.Sum(p => p.d_ValorVenta.Value);
                    Compra.d_IGV = DetallesDeCabecera.Sum(p => p.d_Igv.Value);

                    if (!ComprasBL.ExisteDocumento2(Compra.i_IdTipoDocumento.Value, Compra.v_IdProveedor, Compra.v_SerieDocumento.ToUpper (), Compra.v_CorrelativoDocumento))
                    {
                        if (DetallesDeCabecera.Any(p => p.v_IdProductoDetalle != null) && chkGenerarNI.Checked)
                        {
                            //GeneraNotaIngreso(Compra, DetallesDeCabecera.Where(p => p.v_IdProductoDetalle != null).ToList());
                        }

                        if (Compra.i_IdTipoDocumento == 7)
                        {
                            string x = "";
                        }

                        listaComprasProcesadas.Add(new ComprasBL().InsertarCompra(ref objOperationResult, Compra, Globals.ClientSession.GetAsList(), DetallesDeCabecera, null));

                        if (objOperationResult.Success == 0)
                        {
                            UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation +" " +Compra.i_IdTipoDocumento +" " + Compra.v_SerieDocumento +" "+ Compra.v_CorrelativoDocumento, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    Globals.ProgressbarStatus.i_Progress++;

                }
                catch (Exception ex)
                {
                    UltraMessageBox.Show(ex.Message + " " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                    return;
                }

            }
            UltraMessageBox.Show("Importación Finalizada", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            #endregion
        }

        private void ultraButton4_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Any() && cboCabFechaRegistro.Text != @"--Seleccionar--")
            {
                if (UltraMessageBox.Show("¿Seguro de Convertir la Fecha?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        grdData.Rows.Where(p => p.Cells[cboCabFechaRegistro.Text].Value != null && !string.IsNullOrEmpty(p.Cells[cboCabFechaRegistro.Text].Value.ToString())).ToList()
                                    .ForEach(o => o.Cells[cboCabFechaRegistro.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboCabFechaRegistro.Text].Value.ToString())));
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
