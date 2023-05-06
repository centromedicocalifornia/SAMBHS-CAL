using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using LoadingClass;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
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
    public partial class frmMigracionVentasFox : Form
    {
        ClienteBL _objClienteBL = new ClienteBL();
        ProductoBL _objProductoBL = new ProductoBL();
        List<ventaDto> VentasConDetalle = new List<ventaDto>();
        List<ventadetalleDto> VentaDetalles = new List<ventadetalleDto>();

        public frmMigracionVentasFox(string N)
        {
            InitializeComponent();
        }

        private void frmMigracionVentasFox_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().FormColor;
            panel2.BackColor = new GlobalFormColors().FormColor;
            panel3.BackColor = new GlobalFormColors().FormColor;
            cboCabTipoDoc.SelectedIndex = 0;
            cboCabSerie.SelectedIndex = 0;
            cboCabCorrelativo.SelectedIndex = 0;
            cboCabCodCliente.SelectedIndex = 0;
            cboCabCodVendedor.SelectedIndex = 0;
            cboCabFechaEmision.SelectedIndex = 0;
            cboCabEstado.SelectedIndex = 0;
            cboDetTipoDoc.SelectedIndex = 0;
            cboDetSerie.SelectedIndex = 0;
            cboDetCorrelativo.SelectedIndex = 0;
            cboDetCodigoProd.SelectedIndex = 0;
            cboDetCantidad.SelectedIndex = 0;

            cboCabCondPago.SelectedIndex = 0;
            cboCabCorrelativoGR.SelectedIndex = 0;
            cboCabFOrdenCompra.SelectedIndex = 0;
            cboCabAfectoIGV.SelectedIndex = 0;
            cboCabTipoVenta.SelectedIndex = 0;
            cboCabTipoCambio.SelectedIndex = 0;
            cboCabSerieGR.SelectedIndex = 0;
            cboCabOrdenCompra.SelectedIndex = 0;
            cboCabNroPedido.SelectedIndex = 0;
            cboCabNroDias.SelectedIndex = 0;
            cboCabMoneda.SelectedIndex = 0;
            cboCabGlosa.SelectedIndex = 0;
            cboCabIncluyeIGV.SelectedIndex = 0;

            cboCabCorrelativo.SelectedIndex = 0;
            cboDetObservaciones.SelectedIndex = 0;
            cboDetObservaciones2.SelectedIndex = 0;
            cboDetPrecioDolares.SelectedIndex = 0;
            cboDetPrecioSoles.SelectedIndex = 0;
            cboDetNroCuenta.SelectedIndex = 0;
            cboDetPedido.SelectedIndex = 0;
        }

        private void btnImportarCabecera_Click(object sender, EventArgs e)
        {

            if (grdData.Rows.Any())
            {
                grdData.Selected.Rows.AddRange((UltraGridRow[])grdData.Rows.All);
                grdData.DeleteSelectedRows(false);
            }

            try
            {
                string sFileName;
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
                    cboCabCodVendedor.Items.Add(ColumnKey);
                    cboCabFechaEmision.Items.Add(ColumnKey);
                    cboCabEstado.Items.Add(ColumnKey);
                    cboCabCondPago.Items.Add(ColumnKey);
                    cboCabCorrelativoGR.Items.Add(ColumnKey);
                    cboCabFOrdenCompra.Items.Add(ColumnKey);
                    cboCabAfectoIGV.Items.Add(ColumnKey);
                    cboCabTipoVenta.Items.Add(ColumnKey);
                    cboCabTipoCambio.Items.Add(ColumnKey);
                    cboCabSerieGR.Items.Add(ColumnKey);
                    cboCabOrdenCompra.Items.Add(ColumnKey);
                    cboCabNroPedido.Items.Add(ColumnKey);
                    cboCabNroDias.Items.Add(ColumnKey);
                    cboCabMoneda.Items.Add(ColumnKey);
                    cboCabGlosa.Items.Add(ColumnKey);
                    cboCabIncluyeIGV.Items.Add(ColumnKey);
                    cboCabCorrelativoRef.Items.Add(ColumnKey);
                    cboCabTipoDocRef.Items.Add(ColumnKey);
                    cboCabSerieRef.Items.Add(ColumnKey);
                    cboCabFechaRef.Items.Add(ColumnKey);
                    cboCabTipoOperacion.Items.Add(ColumnKey);
                    cboCabDeduccionAnticipo.Items.Add(ColumnKey);
                }
            }
            List<string> Columnas = cboCabTipoDoc.Items.Cast<string>().ToList();
            cboCabTipoDoc.SelectedIndex = Columnas.FindIndex(p => p == "TIDOC_3");
            cboCabSerie.SelectedIndex = Columnas.FindIndex(p => p == "SFACTU_3");
            cboCabCorrelativo.SelectedIndex = Columnas.FindIndex(p => p == "NFACTU_3");
            cboCabFechaEmision.SelectedIndex = Columnas.FindIndex(p => p == "FCOM_3");
            cboCabCodVendedor.SelectedIndex = Columnas.FindIndex(p => p == "FICHV_3");
            cboCabTipoCambio.SelectedIndex = Columnas.FindIndex(p => p == "TCAM_3");
            cboCabCodCliente.SelectedIndex = Columnas.FindIndex(p => p == "FICH_3");
            cboCabEstado.SelectedIndex = Columnas.FindIndex(p => p == "FLAG_3");
            cboCabMoneda.SelectedIndex = Columnas.FindIndex(p => p == "MONE_3");
            cboCabCondPago.SelectedIndex = Columnas.FindIndex(p => p == "CONDP_3");
            cboCabGlosa.SelectedIndex = Columnas.FindIndex(p => p == "GLOA_3");
            cboCabOrdenCompra.SelectedIndex = Columnas.FindIndex(p => p == "ORDC_3");
            cboCabNroPedido.SelectedIndex = Columnas.FindIndex(p => p == "PED_3");
            cboCabTipoVenta.SelectedIndex = Columnas.FindIndex(p => p == "TIPOV_3");
            cboCabNroDias.SelectedIndex = Columnas.FindIndex(p => p == "NDIAS_3");
            cboCabAfectoIGV.SelectedIndex = Columnas.FindIndex(p => p == "AIGV_3");
            cboCabIncluyeIGV.SelectedIndex = Columnas.FindIndex(p => p == "IIGV_3");
            cboCabFOrdenCompra.SelectedIndex = Columnas.FindIndex(p => p == "FCOMOC_3");
            cboCabSerieGR.SelectedIndex = Columnas.FindIndex(p => p == "SERIE_3");
            cboCabCorrelativoGR.SelectedIndex = Columnas.FindIndex(p => p == "NUMERO_3");
            cboCabTipoDocRef.SelectedIndex = Columnas.FindIndex(p => p == "TIDOCR_3");
            cboCabSerieRef.SelectedIndex = Columnas.FindIndex(p => p == "SFACTUR_3");
            cboCabCorrelativoRef.SelectedIndex = Columnas.FindIndex(p => p == "NFACTUR_3");
            cboCabFechaRef.SelectedIndex = Columnas.FindIndex(p => p == "FCOMREF_3");
            cboCabTipoOperacion.SelectedIndex = Columnas.FindIndex(p => p == "TIPOVTA_3");
            cboCabDeduccionAnticipo.SelectedIndex = Columnas.FindIndex(p => p.Equals("ANT_3"));
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
                    cboDetTipoDoc.Items.Add(ColumnKey);
                    cboDetSerie.Items.Add(ColumnKey);
                    cboDetCorrelativo.Items.Add(ColumnKey);
                    cboDetCodigoProd.Items.Add(ColumnKey);
                    cboDetCantidad.Items.Add(ColumnKey);
                    cboDetObservaciones.Items.Add(ColumnKey);
                    cboDetObservaciones2.Items.Add(ColumnKey);
                    cboDetPrecioDolares.Items.Add(ColumnKey);
                    cboDetPrecioSoles.Items.Add(ColumnKey);
                    cboDetNroCuenta.Items.Add(ColumnKey);
                    cboDetPedido.Items.Add(ColumnKey);
                    cboDetEsAnticipo.Items.Add(ColumnKey);

                }
            }
            List<string> Columnas = cboDetTipoDoc.Items.Cast<string>().ToList();

            cboDetTipoDoc.SelectedIndex = Columnas.FindIndex(p => p == "TDOC_3A");
            cboDetSerie.SelectedIndex = Columnas.FindIndex(p => p == "SERIE_3A");
            cboDetCorrelativo.SelectedIndex = Columnas.FindIndex(p => p == "NRO_3A");
            cboDetCodigoProd.SelectedIndex = Columnas.FindIndex(p => p == "LINEA_3A");
            cboDetCantidad.SelectedIndex = Columnas.FindIndex(p => p == "CANT_3A");
            cboDetPrecioSoles.SelectedIndex = Columnas.FindIndex(p => p == "TOT1_3A");
            cboDetPrecioDolares.SelectedIndex = Columnas.FindIndex(p => p == "TOT2_3A");
            cboDetNroCuenta.SelectedIndex = Columnas.FindIndex(p => p == "CUEN_3A");
            cboDetObservaciones.SelectedIndex = Columnas.FindIndex(p => p == "OBSER1_3A");
            cboDetObservaciones2.SelectedIndex = Columnas.FindIndex(p => p == "OBSER2_3A");
            cboDetPedido.SelectedIndex = Columnas.FindIndex(p => p == "PDE_3A");
            cboDetEsAnticipo.SelectedIndex = Columnas.FindIndex(p => p.Equals("ANT_3A"));

        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (cboCabTipoDoc.SelectedIndex != 0 && cboCabSerie.SelectedIndex != 0 && cboCabCorrelativo.SelectedIndex != 0 && cboDetTipoDoc.SelectedIndex != 0 && cboDetSerie.SelectedIndex != 0 && cboDetCorrelativo.SelectedIndex != 0)
            {
                grdDataDetalles.Selected.Rows.Clear();
                string TipoDoc = grdData.ActiveRow.Cells[cboCabTipoDoc.Text].Value.ToString();
                string Serie = grdData.ActiveRow.Cells[cboCabSerie.Text].Value.ToString();
                string Correlativo = grdData.ActiveRow.Cells[cboCabCorrelativo.Text].Value.ToString();

                string DetTipoDoc = cboDetTipoDoc.Text;
                string DetSerie = cboDetSerie.Text;
                string DetCorrelativo = cboDetCorrelativo.Text;

                List<UltraGridRow> ListaDetalles = grdDataDetalles.Rows.AsParallel().Where(p => p.Cells[DetTipoDoc].Value.ToString() == (TipoDoc) &&
                                                                                                p.Cells[DetSerie].Value.ToString() == (Serie) &&
                                                                                                p.Cells[DetCorrelativo].Value.ToString() == (Correlativo)).ToList();

                if (ListaDetalles.Any())
                {
                    grdDataDetalles.Selected.Rows.AddRange(ListaDetalles.ToArray());
                    ListaDetalles.FirstOrDefault().Activate();
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
                    var Clientes = _objClienteBL.DevuelveClientes();
                    var Vendedores = new VendedorBL().DevuelveVendedores();
                    var Ventas = new List<ventaDto>();

                    string ColumnaTipoDoc = cboCabTipoDoc.Text;

                    try
                    {
                        #region Procesar Cabeceras
                        string ColumnaFechaEmision = cboCabFechaEmision.Text;
                        List<UltraGridRow> BoletasYFacturas = grdData.Rows.Where(p => p.Cells[cboCabFechaEmision.Text].Value != null && p.Cells[cboCabFechaEmision.Text].Value.ToString() != string.Empty)
                                                                .OrderBy(x => x.Cells[ColumnaFechaEmision].Value.ToString()).ToList();


                        foreach (var Fila in BoletasYFacturas)
                        {
                            var venta = new ventaDto();
                            try
                            {
                                int integer;
                                var fechaRegistro = DateTime.Parse(Fila.Cells[cboCabFechaEmision.Text].Value.ToString());
                                DateTime fechaReferencia;
                                venta.v_Periodo = fechaRegistro.Year.ToString("00");
                                venta.v_Mes = fechaRegistro.Month.ToString("00");
                                venta.i_IdIgv = 1; //Actual 18%
                                venta.i_IdTipoDocumento = int.TryParse(Fila.Cells[cboCabTipoDoc.Text].Value.ToString().Trim(), out integer) ? integer : -1;
                                venta.v_SerieDocumento = Fila.Cells[cboCabSerie.Text].Value.ToString().Trim();
                                venta.v_CorrelativoDocumento = Fila.Cells[cboCabCorrelativo.Text].Value.ToString().Trim();
                                venta.v_CorrelativoDocumentoFin = string.Empty;
                                venta.i_IdTipoDocumentoRef = !string.IsNullOrEmpty(Fila.Cells[cboCabTipoDocRef.Text].Value.ToString().Trim()) ? int.TryParse(Fila.Cells[cboCabTipoDocRef.Text].Value.ToString().Trim(), out integer) ? integer : -1 : -1;
                                venta.v_SerieDocumentoRef = Fila.Cells[cboCabSerieRef.Text].Value.ToString().Trim();
                                venta.v_CorrelativoDocumentoRef = Fila.Cells[cboCabCorrelativoRef.Text].Value.ToString();
                                venta.t_FechaRef = !string.IsNullOrWhiteSpace(cboCabFechaRef.Text) ? DateTime.TryParse(Fila.Cells[cboCabFechaRef.Text].Value.ToString(), out fechaReferencia) ? fechaReferencia : fechaRegistro : fechaRegistro;
                                venta.i_IdEstado = int.TryParse(Fila.Cells[cboCabEstado.Text].Value.ToString().Trim(), out integer) ? integer : 1;

                                string codCliente = Fila.Cells[cboCabCodCliente.Text].Value.ToString().Trim();
                                string codVendedor = Fila.Cells[cboCabCodVendedor.Text].Value.ToString().Trim();

                                if (venta.i_IdEstado == 1)
                                {
                                    if (!string.IsNullOrEmpty(codCliente))
                                    {
                                        KeyValueDTO Cliente = Clientes.FirstOrDefault(p => p.Value1.Trim() == codCliente);

                                        if (Cliente != null)
                                        {
                                            venta.v_IdCliente = Cliente.Id;
                                            venta.NombreCliente = Cliente.Value3;
                                        }
                                        else
                                            venta.v_IdCliente = "N002-CL000000000";
                                    }
                                    else
                                        venta.v_IdCliente = "N002-CL000000000";
                                }
                                else
                                {
                                    var objOperationResult = new OperationResult();
                                    var clienteAnulado = new ClienteBL().CreaVerificaClienteAnulado(ref objOperationResult);
                                    venta.v_IdCliente = clienteAnulado.v_IdCliente;
                                }

                                if (!string.IsNullOrEmpty(codVendedor))
                                {
                                    var vendedor = Vendedores.FirstOrDefault(p => p.Value1.Trim() == codVendedor.Trim());
                                    venta.v_IdVendedor = vendedor != null ? vendedor.Id : null;
                                }
                                else
                                    venta.v_IdVendedor = null;

                                venta.v_NombreClienteTemporal = venta.i_IdTipoDocumento == 3 ? Fila.Cells[cboCabCodVendedor.Text.Trim()].Value.ToString() : string.Empty;
                                venta.v_DireccionClienteTemporal = string.Empty;
                                venta.t_FechaRegistro = fechaRegistro;
                                venta.d_TipoCambio = decimal.Parse(Fila.Cells[cboCabTipoCambio.Text].Value.ToString().Trim());
                                venta.i_NroDias = int.TryParse(Fila.Cells[cboCabNroDias.Text].Value.ToString(), out integer) ? integer : 0;

                                venta.t_FechaVencimiento = venta.t_FechaRegistro.Value.AddDays(venta.i_NroDias.Value);

                                venta.i_IdCondicionPago = Fila.Cells[cboCabCondPago.Text].Value != null ? int.TryParse(Fila.Cells[cboCabCondPago.Text].Value.ToString(), out integer) ? integer : -1 : -1;

                                venta.i_EsAfectoIgv = int.TryParse(Fila.Cells[cboCabAfectoIGV.Text].Value.ToString(), out integer) ? integer : 1;
                                venta.i_PreciosIncluyenIgv = int.TryParse(Fila.Cells[cboCabIncluyeIGV.Text].Value.ToString(), out integer) ? integer : 1;
                                venta.v_IdVendedorRef = "-1";
                                venta.d_PorcDescuento = 0;
                                venta.d_PocComision = 0;
                                venta.d_Descuento = 0;
                                venta.d_Percepcion = 0;
                                venta.d_Anticipio = 0;
                                venta.i_DeduccionAnticipio = !string.IsNullOrWhiteSpace(Fila.Cells[cboCabDeduccionAnticipo.Text].Value.ToString()) ? int.Parse(Fila.Cells[cboCabDeduccionAnticipo.Text].Value.ToString()) : 0;
                                venta.v_NroPedido = !string.IsNullOrWhiteSpace(cboCabNroPedido.Text) ? Fila.Cells[cboCabNroPedido.Text].Value.ToString().Trim() : string.Empty;
                                venta.v_NroGuiaRemisionSerie = Fila.Cells[cboCabSerieGR.Text].Value.ToString().Trim();
                                venta.v_NroGuiaRemisionCorrelativo = Fila.Cells[cboCabCorrelativoGR.Text].Value.ToString().Trim();
                                venta.v_NroBulto = string.Empty;
                                venta.t_FechaOrdenCompra = fechaRegistro;
                                venta.v_OrdenCompra = !string.IsNullOrWhiteSpace(cboCabOrdenCompra.Text.Trim()) ? Fila.Cells[cboCabOrdenCompra.Text].Value.ToString().Trim() : string.Empty;
                                venta.i_IdTipoVenta = int.TryParse(Fila.Cells[cboCabTipoVenta.Text].Value.ToString().Trim(), out integer) ? integer : 1;
                                venta.i_IdTipoVenta = venta.i_IdTipoVenta == 0 ? 3 : venta.i_IdTipoVenta;
                                if ((listBox1.Items.Count > 0 && listBox2.Items.Count > 0) && (listBox1.Items.Count == listBox2.Items.Count))
                                {
                                    if (!string.IsNullOrWhiteSpace(cboCabTipoOperacion.Text.Trim()))
                                    {
                                        var valorExcel = int.TryParse(Fila.Cells[cboCabTipoOperacion.Text].Value.ToString().Trim(), out integer) ? integer : 1;
                                        var listaExcel = listBox1.Items.Cast<string>().ToList();
                                        var index = listaExcel.FindIndex(p => p == valorExcel.ToString());

                                        if (index > 0)
                                        {
                                            int valorOperacion;
                                            venta.i_IdTipoOperacion = int.TryParse(listBox2.Items[index].ToString(), out valorOperacion) ? valorOperacion : -1;
                                        }
                                    }
                                    else
                                        venta.i_IdTipoOperacion = 1;
                                }
                                else
                                {
                                    UltraMessageBox.Show("Las reaciones de tipo de operacion entre el excel \ny el sistema no fue ingresada correctamente");
                                    return;
                                }
                                int almacen;
                                if (grdData.DisplayLayout.Bands[0].Columns.Exists("ALMACEN"))
                                    venta.Almacen = int.TryParse(Fila.Cells["ALMACEN"].Value.ToString().Trim(), out almacen) ? almacen : -1;
                                else
                                    venta.Almacen = -1;

                                venta.i_IdEstablecimiento = Globals.ClientSession.i_IdEstablecimiento.Value;
                                venta.i_IdMoneda = int.TryParse(Fila.Cells[cboCabMoneda.Text].Value.ToString().Trim(), out integer) ? integer : 1;
                                venta.v_Concepto = Fila.Cells[cboCabGlosa.Text].Value.ToString().Trim();
                                venta.d_PesoBrutoKG = 0;
                                venta.d_PesoNetoKG = 0;
                                venta.i_IdPuntoEmbarque = -1;
                                venta.i_IdPuntoDestino = -1;
                                venta.i_IdTipoEmbarque = -1;
                                venta.i_IdMedioPagoVenta = -1;
                                venta.v_Marca = string.Empty;
                                venta.i_DrawBack = 0;
                                venta.v_BultoDimensiones = string.Empty;
                                venta.RegistroKey = venta.i_IdTipoDocumento.ToString() + int.Parse(venta.v_SerieDocumento) + int.Parse(venta.v_CorrelativoDocumento);
                                Ventas.Add(venta);
                            }
                            catch (Exception ex)
                            {
                                UltraMessageBox.Show(ex.Message + "\n" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                                break;
                            }
                        }

                        #endregion

                        #region Procesar Detalles
                        var productos = _objProductoBL.DevolverProductos();
                        var productosNoEncontrados = new List<string>();

                        ColumnaTipoDoc = cboDetTipoDoc.Text;
                        var detallesBoletasYFacturas = grdDataDetalles.Rows.Where(p => !string.IsNullOrWhiteSpace(p.Cells[cboDetTipoDoc.Text].Value.ToString())).ToList();
                        foreach (var Fila in detallesBoletasYFacturas)
                        {
                            var codProducto = Fila.Cells[cboDetCodigoProd.Text].Value.ToString().Trim();
                            var ventadetalle = new ventadetalleDto();
                            //ventadetalle.i_IdAlmacen = !grdDataDetalles.DisplayLayout.Bands[0].Columns.Exists("ALMACEN") ? Globals.ClientSession.i_IdAlmacenPredeterminado.Value : int.Parse(Fila.Cells["ALMACEN"].Value.ToString().Trim());
                            ventadetalle.v_NroCuenta = Fila.Cells[cboDetNroCuenta.Text].Value.ToString().Trim();
                            ventadetalle.i_Anticipio = !string.IsNullOrWhiteSpace(cboDetEsAnticipo.Text.Trim()) ? int.Parse(Fila.Cells[cboDetEsAnticipo.Text].Value.ToString()) : 0;

                            if (productos.Select(p => p.Value1.Trim()).ToList().Contains(codProducto))
                            {
                                var productoDetalle = productos.FirstOrDefault(p => p.Value1 == codProducto);
                                ventadetalle.v_IdProductoDetalle = productoDetalle.Value2;
                                ventadetalle.v_DescripcionProducto = productoDetalle.Value3;
                            }

                            var cantidad = Fila.Cells[cboDetCantidad.Text].Value.ToString() != string.Empty ? decimal.Parse(Fila.Cells[cboDetCantidad.Text].Value.ToString()) : 0;
                            ventadetalle.v_NroCuenta = string.IsNullOrWhiteSpace(ventadetalle.v_NroCuenta) ? "-1" : ventadetalle.v_NroCuenta;
                            ventadetalle.d_Cantidad = cantidad > 0 ? cantidad : cantidad * -1;
                            ventadetalle.d_CantidadEmpaque = ventadetalle.d_Cantidad.Value;
                            ventadetalle.i_IdUnidadMedida = 15;
                            ventadetalle.d_Descuento = 0;
                            ventadetalle.i_IdTipoOperacion = 1;
                            ventadetalle.d_PrecioVenta = decimal.Parse(Fila.Cells[cboDetPrecioSoles.Text].Value.ToString().Trim());
                            ventadetalle.d_PrecioVenta = ventadetalle.d_PrecioVenta > 0
                                ? ventadetalle.d_PrecioVenta
                                : ventadetalle.d_PrecioVenta*-1;
                            ventadetalle.TotalDolares = decimal.Parse(Fila.Cells[cboDetPrecioDolares.Text].Value.ToString().Trim());
                            ventadetalle.v_Observaciones = Fila.Cells[cboDetObservaciones.Text].Value.ToString().Trim() + "\r\n" + Fila.Cells[cboDetObservaciones2.Text].Value.ToString().Trim();
                            ventadetalle.SerieDocumento = Fila.Cells[cboDetSerie.Text].Value.ToString();
                            ventadetalle.CorrelativoDocumento = Fila.Cells[cboDetCorrelativo.Text].Value.ToString();
                            ventadetalle.TipoDocumento = int.Parse(Fila.Cells[cboDetTipoDoc.Text].Value.ToString());
                            ventadetalle.RegistroKey = ventadetalle.TipoDocumento.ToString() + int.Parse(ventadetalle.SerieDocumento).ToString() + int.Parse(ventadetalle.CorrelativoDocumento).ToString();
                            VentaDetalles.Add(ventadetalle);
                        }

                        if (productosNoEncontrados.Count() != 0)
                        {
                            UltraMessageBox.Show(string.Format("Hay {0} productos no registrados, por favor regístrelos primero.", productosNoEncontrados.Distinct().Count()));
                            return;
                        }
                        #endregion

                        #region Separa las cabeceras que cuentan con detalle para procesarlas
                        VentasConDetalle = new List<ventaDto>();

                        var VentaDetallesKeys = VentaDetalles.Select(p => p.RegistroKey).Distinct().ToList();
                        foreach (ventaDto Venta in Ventas.AsParallel())
                        {
                            if (VentaDetallesKeys.Contains(Venta.RegistroKey))
                            {
                                VentasConDetalle.Add(Venta);
                            }
                        }

                        #endregion

                        #region Asigna los correlativos de las ventas filtradas de acuerdo a su mes
                        List<string> Meses = Ventas.Select(p => p.t_FechaRegistro.Value.Month.ToString()).Distinct().ToList();
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

                        var detallesSinCuenta = VentaDetalles.Where(p => p.v_NroCuenta == "-1").ToList();
                        if (detallesSinCuenta.Any())
                        {
                            var cabeceras = detallesSinCuenta.Select(p => p.RegistroKey).Distinct().ToList();
                            MessageBox.Show(string.Join(", ", cabeceras), "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        #region Empieza el guardado de las ventas filtradas
                        (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();
                        bwkProceso.RunWorkerAsync();
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        Globals.ProgressbarStatus.b_Cancelado = true;
                        UltraMessageBox.Show(ex.Message + "\n" + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
                        return;
                    }
                }
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {

            ConvertirFechas(cboCabFechaEmision.Text);

        }

        private void ultraButton3_Click(object sender, EventArgs e)
        {

            ConvertirFechas(cboCabFOrdenCompra.Text);

        }

        private void bwkProceso_DoWork(object sender, DoWorkEventArgs e)
        {
            Globals.ProgressbarStatus.i_TotalProgress = VentasConDetalle.Count;
            var ventasExistentes = new VentaBL().ObtenerVentasParaMigracion().ToList();
            var establecimientos = new VentaBL().ObtenerEstablecimientoalmacenDtos();
            foreach (ventaDto Venta in VentasConDetalle.AsParallel())
            {
                OperationResult objOperationResult = new OperationResult();
                List<ventadetalleDto> DetallesDeCabecera = VentaDetalles.Where(p => p.TipoDocumento == Venta.i_IdTipoDocumento && int.Parse(p.SerieDocumento) == int.Parse(Venta.v_SerieDocumento) && int.Parse(p.CorrelativoDocumento) == int.Parse(Venta.v_CorrelativoDocumento)).ToList();
                
                foreach (var Detalle in DetallesDeCabecera)
                {
                    if (Venta.Almacen != -1)
                        Detalle.i_IdAlmacen = Venta.Almacen;
                    if (Venta.i_IdMoneda == 1)
                    {
                        if (Venta.i_EsAfectoIgv == 1)
                        {
                            Detalle.d_ValorVenta = (decimal?)(double.Parse(Detalle.d_PrecioVenta.Value.ToString()) / 1.18);
                            Detalle.d_Igv = Detalle.d_PrecioVenta.Value - Detalle.d_ValorVenta.Value;
                            Detalle.d_Precio = Detalle.d_Cantidad != 0 ? Venta.i_PreciosIncluyenIgv.Value == 1 ? (Detalle.d_PrecioVenta) / Detalle.d_Cantidad : (Detalle.d_PrecioVenta - Detalle.d_Igv - Detalle.d_Descuento) / Detalle.d_Cantidad : 0;
                            Detalle.d_Valor = Detalle.d_Cantidad.Value * Detalle.d_Precio.Value;
                            Detalle.i_IdTipoOperacion = Venta.i_IdTipoOperacion;
                            if (Detalle.i_Anticipio == 1)
                            {
                                if (Venta.i_PreciosIncluyenIgv == 1)
                                {
                                    Detalle.d_Valor = Detalle.d_PrecioVenta;
                                }
                            }
                            Detalle.i_IdTipoOperacionAnexo = Venta.i_IdTipoOperacion == 2 ? 1 : 0;
                        }
                        else
                        {

                            Detalle.d_ValorVenta = Detalle.d_PrecioVenta;
                            Detalle.d_Igv = 0;
                            Detalle.d_Precio = Detalle.d_Cantidad != 0 ? Venta.i_PreciosIncluyenIgv.Value == 1 ? (Detalle.d_PrecioVenta) / Detalle.d_Cantidad : (Detalle.d_PrecioVenta - Detalle.d_Igv - Detalle.d_Descuento) / Detalle.d_Cantidad : 0;
                            Detalle.d_Valor = Detalle.d_Cantidad.Value * Detalle.d_Precio.Value;
                            Detalle.i_IdTipoOperacion = Venta.i_IdTipoOperacion;
                            if (Detalle.i_Anticipio == 1)
                            {
                                if (Venta.i_PreciosIncluyenIgv == 1)
                                {
                                    Detalle.d_Valor = Detalle.d_PrecioVenta;
                                }
                            }
                            Detalle.i_IdTipoOperacionAnexo = Venta.i_IdTipoOperacion == 2 ? 1 : 0;
                        }
                    }
                    else
                    {
                        if (Venta.i_EsAfectoIgv == 1)
                        {
                            Detalle.d_PrecioVenta = Detalle.TotalDolares;
                            Detalle.d_ValorVenta = (decimal?)(double.Parse(Detalle.d_PrecioVenta.Value.ToString()) / 1.18);
                            Detalle.d_Igv = Detalle.d_PrecioVenta.Value - Detalle.d_ValorVenta.Value;
                            Detalle.d_Precio = Detalle.d_Cantidad != 0 ? Venta.i_PreciosIncluyenIgv.Value == 1 ? (Detalle.d_PrecioVenta) / Detalle.d_Cantidad : (Detalle.d_PrecioVenta - Detalle.d_Igv - Detalle.d_Descuento) / Detalle.d_Cantidad : 0;
                            Detalle.d_Valor = Detalle.d_Cantidad.Value * Detalle.d_Precio.Value;
                            Detalle.i_IdTipoOperacion = Venta.i_IdTipoOperacion;
                            if (Detalle.i_Anticipio == 1)
                            {
                                if (Venta.i_PreciosIncluyenIgv == 1)
                                {
                                    Detalle.d_Valor = Detalle.d_PrecioVenta;
                                }
                            }
                            Detalle.i_IdTipoOperacionAnexo = Venta.i_IdTipoOperacion == 2 ? 1 : 0;
                        }
                        else
                        {
                            Detalle.d_PrecioVenta = Detalle.TotalDolares;
                            Detalle.d_ValorVenta = Detalle.d_PrecioVenta;
                            Detalle.d_Igv = 0;
                            Detalle.d_Precio = Detalle.d_Cantidad != 0 ? Venta.i_PreciosIncluyenIgv.Value == 1 ? (Detalle.d_PrecioVenta) / Detalle.d_Cantidad : (Detalle.d_PrecioVenta - Detalle.d_Igv - Detalle.d_Descuento) / Detalle.d_Cantidad : 0;
                            Detalle.d_Valor = Detalle.d_Cantidad.Value * Detalle.d_Precio.Value;
                            if (Detalle.i_Anticipio == 1)
                            {
                                if (Venta.i_PreciosIncluyenIgv == 1)
                                {
                                    Detalle.d_Valor = Detalle.d_PrecioVenta;
                                }
                            }
                            Detalle.i_IdTipoOperacion = Venta.i_IdTipoOperacion;
                            Detalle.i_IdTipoOperacionAnexo = Venta.i_IdTipoOperacion == 2 ? 1 : 0;
                        }
                    }
                }
                var filasAnticipo = DetallesDeCabecera.Where(p => p.i_Anticipio.HasValue && p.i_Anticipio.Value == 1).ToList();
                var filasSinAnticipo = DetallesDeCabecera.Where(p => p.i_Anticipio.HasValue && p.i_Anticipio.Value != 1).ToList();
                decimal AnticipoPV = 0, AnticipoVV = 0, AnticipoIgv = 0, AnticipoValor = 0;
                if (filasAnticipo.Any())
                {
                    AnticipoPV = filasAnticipo.Sum(o => o.d_PrecioVenta ?? 0);
                    AnticipoVV = filasAnticipo.Sum(o => o.d_ValorVenta ?? 0);
                    AnticipoIgv = filasAnticipo.Sum(o => o.d_Igv ?? 0);
                    AnticipoValor = filasAnticipo.Sum(o => o.d_Valor ?? 0);
                }
                var almacen = DetallesDeCabecera.FirstOrDefault().i_IdAlmacen ?? 0;
                var establecimiento = establecimientos.FirstOrDefault(p => p.i_IdAlmacen == almacen).i_IdEstablecimiento;
                Venta.i_IdEstablecimiento = establecimiento;
                Venta.d_Total = filasSinAnticipo.Sum(p => p.d_PrecioVenta ?? 0) - AnticipoPV;
                Venta.d_ValorVenta = filasSinAnticipo.Sum(p => p.d_ValorVenta ?? 0) - AnticipoVV;
                Venta.d_IGV = filasSinAnticipo.Sum(p => p.d_Igv ?? 0) - AnticipoIgv;
                Venta.d_Valor = filasSinAnticipo.Sum(p => p.d_Valor) - AnticipoValor;
                Venta.d_Anticipio = AnticipoValor;
                Venta.i_IdTipoOperacion = 1; //puesto para hormiga.
                Venta.v_IdVendedor = "-1"; // para hormiga.
                if (DetallesDeCabecera.Any(p => p.i_IdAlmacen == null))
                {
                    MessageBox.Show("Uno o más almacenes en el detalles es nulo");
                    return;
                }
                if (!ventasExistentes.Any(p => p.v_SerieDocumento.Equals(Venta.v_SerieDocumento) && p.v_CorrelativoDocumento.Equals(Venta.v_CorrelativoDocumento)))
                {
                    new VentaBL().InsertarVentaImportada(ref objOperationResult, Venta, Globals.ClientSession.GetAsList(), DetallesDeCabecera, chkGenerarNS.Checked, true);
                    if (objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                
                Globals.ProgressbarStatus.i_Progress++;
            }

            UltraMessageBox.Show("Importación Finalizada", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ultraButton4_Click(object sender, EventArgs e)
        {

            ConvertirFechas(cboCabFechaRef.Text);

        }

        private void ultraPopupControlContainer1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void ultraButton5_Click(object sender, EventArgs e)
        {
            ultraPopupControlContainer1.Show();
        }

        private void ultraTextEditor1_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (!String.IsNullOrEmpty(ultraTextEditor1.Text.Trim()))
            {
                listBox1.Items.Add(ultraTextEditor1.Text);
                ultraTextEditor1.Clear();
            }
        }

        private void ultraTextEditor2_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (!String.IsNullOrEmpty(ultraTextEditor2.Text.Trim()))
            {
                listBox2.Items.Add(ultraTextEditor2.Text);
                ultraTextEditor2.Clear();
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                }
            }
        }

        private void listBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (listBox2.SelectedIndex >= 0)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    listBox2.Items.RemoveAt(listBox2.SelectedIndex);
                }
            }
        }

        private void ultraPopupControlContainer1_Closed(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != listBox2.Items.Count)
            {
                UltraMessageBox.Show("Ambas listas deben tener la misma cantidad de elementos.!");
                ultraPopupControlContainer1.Show();
            }
        }

        private void panelPopUp_Validating(object sender, CancelEventArgs e)
        {

        }

        private void ultraTextEditor1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(ultraTextEditor1, e);
        }

        private void ultraTextEditor2_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(ultraTextEditor2, e);
        }

        private void ConvertirFechas(string strCampo)
        {
            if (grdData.Rows.Any() && strCampo != "--Seleccionar--")
            {
                if (UltraMessageBox.Show("¿Seguro de Convertir la Fecha?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        var filas =
                            grdData.Rows.Where(
                                p =>
                                    p.Cells[strCampo].Value != null &&
                                    !string.IsNullOrEmpty(p.Cells[strCampo].Value.ToString())).ToList();


                        foreach (var o in filas)
                            o.Cells[strCampo].Value = RetornaFecha(o.Cells[strCampo].Value.ToString());
                    }
                    catch (Exception ex)
                    {
                        UltraMessageBox.Show(ex.Message);
                    }
                }
            }
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

    }
}
