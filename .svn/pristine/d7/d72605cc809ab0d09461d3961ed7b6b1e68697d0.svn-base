using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.BE;
using SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones;

namespace SAMBHS.Windows.WinClient.UI.Migraciones
{
    public partial class frmMigraciones : Form
    {
        MigracionesBL _objMigracionesBL = new MigracionesBL();
        LineaBL _objLineaBL = new LineaBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        ProductoBL _objProductoBL = new ProductoBL();
        string _modeDatos = "New";
        public frmMigraciones(string parametro)
        {
            InitializeComponent();
        }

        private void frmMigraciones_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            //Utils.Windows.LoadDropDownList(cboLinea, "Value1", "Id", _objLineaBL.LlenarComboLinea(ref objOperationResult, "v_CodLinea"), DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboTipoProducto, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 6, null), DropDownListAction.Select);
            cboCodigo.SelectedIndex = 0;
            cboDescripcion.SelectedIndex = 0;
            cboCosto.SelectedIndex = 0;
            cboUnidadMedida.SelectedIndex = 0;
            cboLinea.SelectedIndex = 0;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Any())
            {
                grdData.Selected.Rows.AddRange((UltraGridRow[])grdData.Rows.All);
                grdData.DeleteSelectedRows(false);
            }

            try
            {
                string sFileName = "";
                OpenFileDialog choofdlog = new OpenFileDialog();
                choofdlog.Filter = @"Archivos Excel (*.*)| *.*";
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

                Workbook workbook = Workbook.Load(sFileName);
                Worksheet worksheet = workbook.Worksheets[0];

                this.ultraDataSource1.Reset();

                bool isHeaderRow = true;
                foreach (WorksheetRow row in worksheet.Rows)
                {
                    // Assume that the first row is the column headers. 
                    if (isHeaderRow)
                    {
                        foreach (WorksheetCell cell in row.Cells)
                        {
                            // Get the text of the cell. 
                            string columnKey = cell.GetText();

                            // Adda column
                            UltraDataColumn column = this.ultraDataSource1.Band.Columns.Add(columnKey);

                            // Set the DataType. 
                            switch (columnKey)
                            {
                                default:
                                    column.DataType = typeof(string);
                                    break;
                            }
                        }

                        // All following rows will not be headers. 
                        isHeaderRow = false;
                    }
                    else
                    {
                        try
                        {
                            this.ultraDataSource1.Rows.Add(row.Cells.Select(cell => cell.Value).ToArray());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            continue;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            grdData.SetDataBinding(this.ultraDataSource1, "Band 0");

            if (grdData.Rows.Count > 0)
            {
                if (RevisarDuplicidad() != 0)
                {
                    btnImportar.Enabled = true;
                }
            }
            else
            {
                btnImportar.Enabled = true;
            }
            foreach (UltraGridCell cell in grdData.Rows[0].Cells)
            {
                string ColumnKey = cell.Column.Key;
                if (ColumnKey != "Nuevo")
                {
                    cboCodigo.Items.Add(ColumnKey);
                    cboDescripcion.Items.Add(ColumnKey);
                    cboCosto.Items.Add(ColumnKey);
                    cboUnidadMedida.Items.Add(ColumnKey);
                    cboLinea.Items.Add(ColumnKey);
                }
            }
        }

        private int RevisarDuplicidad()
        {
            List<string> Resultado = new List<string>();
            Resultado = _objMigracionesBL.DevuelveDuplicados(grdData.Rows.Select(p => p.Cells[0].Value.ToString().Trim()).ToList());
            int RegNuevos = 0;

            foreach (UltraGridRow Fila in grdData.Rows.Where(p => Resultado.Contains(p.Cells[0].Value.ToString().Trim())))
            {
                Fila.Cells["Nuevo"].Value = "1";
            }
            RegNuevos = grdData.Rows.Count(p => p.Cells["Nuevo"].Value != null);

            lblContadorFilas.Text = string.Format("Se importaron {0} Registros, Nuevos: {1}", grdData.Rows.Count(), RegNuevos);
            grdData.DisplayLayout.Bands[0].SortedColumns.Add("Nuevo", true, true);
            return RegNuevos;

        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells["Nuevo"].Value != null && e.Row.Cells["Nuevo"].Value.ToString() == "1")
            {
                e.Row.Appearance.BackColor = Color.Honeydew;
                e.Row.Appearance.BackColor2 = Color.Honeydew;
                e.Row.Appearance.ForeColor = Color.Black;
            }
        }

        private void ultraButton1_Click_1(object sender, EventArgs e)
        {
            var ProductosEnBD = _objProductoBL.DevolverProductos();

            if (uvDatos.Validate(true, false).IsValid)
            {
                var Filas = grdData.Rows.Where(p => p.Cells["Nuevo"].Value != null && p.Cells["Nuevo"].Value.ToString() == "1" && !string.IsNullOrEmpty(p.Cells[cboCodigo.Text].Value.ToString()) && !string.IsNullOrEmpty(p.Cells[cboDescripcion.Text].Value.ToString()));

                var _productoDto = new productoDto();
                var objOperationResult = new OperationResult();

                if (_modeDatos == "New")
                {
                    var listaProductos = new List<productoDto>();
                    var filasDepuradas = Filas.ToList().GroupBy(o => o.Cells[cboCodigo.Text].Value.ToString()).ToList().Select(group => group.First()).ToList();
                    var existeEmpaque = grdData.DisplayLayout.Bands[0].Columns.Contains("Empaque");
                    var existeTipoArt = grdData.DisplayLayout.Bands[0].Columns.Contains("Tipo_Art");
                    foreach (UltraGridRow Fila in filasDepuradas)
                    {
                        var vCodInterno = Fila.Cells[cboCodigo.Text].Value.ToString().Trim();

                        if (!ProductosEnBD.Select(p => p.Value1.Trim()).ToList().Contains(vCodInterno))
                        {
                            decimal costo;
                            _productoDto = new productoDto();
                            _productoDto.v_CodInterno = vCodInterno;
                            _productoDto.v_IdLinea = cboLinea.Text == "--Seleccionar--" ? "-1" : Fila.Cells[cboLinea.Text].Value.ToString();
                            _productoDto.v_Descripcion = Fila.Cells[cboDescripcion.Text].Value.ToString();
                            _productoDto.d_Empaque = existeEmpaque ? int.Parse(Fila.Cells["Empaque"].Value.ToString()) : 1;
                            _productoDto.i_IdUnidadMedida = cboUnidadMedida.Text == "--Seleccionar--" ? 15 : Fila.Cells[cboUnidadMedida.Text].Value == null ? 15 : int.Parse(Fila.Cells[cboUnidadMedida.Text].Value.ToString());
                            _productoDto.d_Peso = 0;
                            _productoDto.v_Ubicacion = string.Empty;
                            _productoDto.v_Caracteristica = string.Empty;
                            _productoDto.v_CodProveedor = string.Empty;
                            _productoDto.v_Descripcion2 = string.Empty;
                            _productoDto.i_IdTipoProducto = cboTipoProducto.SelectedValue.ToString() == "-1" ? -1 : int.Parse(cboTipoProducto.SelectedValue.ToString());
                            _productoDto.i_EsServicio = RevisaSiEsServicio(vCodInterno) ? 1 : 0;
                            _productoDto.i_EsLote = 0;
                            _productoDto.i_EsAfectoDetraccion = 0;
                            _productoDto.i_EsActivo = 1;
                            _productoDto.i_NombreEditable = 0;
                            _productoDto.d_PrecioCosto = cboCosto.Text == "--Seleccionar--" ? 0 : Fila.Cells[cboCosto.Text].Value == null ? 0 : decimal.TryParse(Fila.Cells[cboCosto.Text].Value.ToString(), out costo) ? costo : 0;
                            _productoDto.i_ValidarStock = 0;
                            _productoDto.i_IdProveedor = -1;
                            _productoDto.i_IdTipo = -1;
                            _productoDto.d_PrecioVenta = 0;
                            _productoDto.d_separacion = 0;
                            _productoDto.i_EsActivoFijo = 0;
                            _productoDto.d_StockMaximo = 0;
                            _productoDto.d_StockMinimo = 0;
                            _productoDto.i_IdUsuario = -1;
                            _productoDto.i_IdTela = -1;
                            _productoDto.i_IdEtiqueta = -1;
                            _productoDto.i_IdCuello = -1;
                            _productoDto.i_IdAplicacion = -1;
                            _productoDto.i_IdArte = -1;
                            _productoDto.i_IdColeccion = -1;
                            _productoDto.i_IdTemporada = -1;
                            _productoDto.i_EsAfectoPercepcion = 0;
                            _productoDto.d_TasaPercepcion = 0;
                            _productoDto.i_Anio = 0;
                            _productoDto.i_IdTipoProducto =existeTipoArt ? int.Parse(Fila.Cells["Tipo_Art"].Value.ToString()) : -1;
                            listaProductos.Add(_productoDto);
                            var prodIngresado = new KeyValueDTO();
                            prodIngresado.Value1 = _productoDto.v_CodInterno;
                            ProductosEnBD.Add(prodIngresado);
                        }
                    }
                    if (UltraMessageBox.Show("Se importarán sólo los nuevos productos encontrados (" + listaProductos.Count().ToString() + "), ¿Desea Continuar...?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        _objProductoBL.InsertarProductos(ref objOperationResult, listaProductos, Globals.ClientSession.GetAsList());

                        if (objOperationResult.Success == 1)
                        {
                            UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            if (objOperationResult.ErrorMessage == null)
                            {
                                UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            if (cboUnidadMedida.Text != "--Seleccionar--" && grdData.Rows.Any())
            {
                List<string> UnidadesMedidas = new List<string>();

                UnidadesMedidas = grdData.Rows.GroupBy(p => p.Cells[cboUnidadMedida.Text].Value.ToString()).Select(x => x.First().Cells[cboUnidadMedida.Text].Value.ToString().Trim()).ToList();

                frmIgualarUM f = new frmIgualarUM(UnidadesMedidas);
                f.ShowDialog();
            }
        }

        public void RecibirListaUM(List<string[]> ListaUM)
        {
            ListaUM.ForEach(x => IgualarUMdesdeLista(x[0], x[1]));
        }

        public void IgualarUMdesdeLista(string UM, string IdUnidad)
        {
            foreach (UltraGridRow Fila in grdData.Rows.Where(p => p.Cells[cboUnidadMedida.Text].Value.ToString().Trim() == UM).ToList())
            {
                Fila.Cells[cboUnidadMedida.Text].Value = IdUnidad;
            }
        }

        public void RecibirListaLineas(List<string[]> ListaLinea)
        {
            ListaLinea.ForEach(x => IgualarLineasdesdeLista(x[0], x[1]));
        }

        public void IgualarLineasdesdeLista(string Linea, string IdLinea)
        {
            string ColumnaLinea = cboLinea.Text;
            foreach (UltraGridRow Fila in grdData.Rows.Where(p => p.Cells[ColumnaLinea].Value.ToString().Trim() == Linea).ToList().AsParallel())
            {
                Fila.Cells[ColumnaLinea].Value = IdLinea;
            }
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded == true)
            {
                gpDetalle.Location = new Point(ultraExpandableGroupBox1.Location.X + ultraExpandableGroupBox1.Height + 5, gpDetalle.Location.Y);
                gpDetalle.Width = 831;
            }
            else
            {
                gpDetalle.Location = new Point(ultraExpandableGroupBox1.Location.X + ultraExpandableGroupBox1.Height + 5, gpDetalle.Location.Y);
                gpDetalle.Width = 1089;
            }
        }

        bool RevisaSiEsServicio(string CodigoProducto)
        {
            try
            {
                List<string> CodigoServicios = txtDetCodServicios.Text.Split(new Char[] { ',' }).ToList();
                foreach (string CodServicio in CodigoServicios)
                {
                    if (CodigoProducto.Contains(CodServicio))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return false;
            }
        }

        private void ultraButton3_Click(object sender, EventArgs e)
        {
            if (cboLinea.Text != "--Seleccionar--" && grdData.Rows.Any())
            {
                List<string> Lineas = new List<string>();

                Lineas = grdData.Rows.GroupBy(p => p.Cells[cboLinea.Text].Value.ToString()).Select(x => x.First().Cells[cboLinea.Text].Value.ToString().Trim()).ToList();

                frmIgualarLineas f = new frmIgualarLineas(Lineas);
                f.ShowDialog();
            }
        }

        private void ultraButton4_Click(object sender, EventArgs e)
        {
            ///frmMigrarProductoUtilidadesDescuentos f = new frmMigrarProductoUtilidadesDescuentos();
            frmMigrarProductoUtilidadesDescuentos2 f = new frmMigrarProductoUtilidadesDescuentos2();
            f.ShowDialog();
        }

        private void ultraButton5_Click(object sender, EventArgs e)
        {
            if (cboLinea.Text != string.Empty && !cboLinea.Text.Equals("--Seleccionar--"))
            {
                var lineas = grdData.Rows
                    .Where(o => !string.IsNullOrWhiteSpace(o.Cells[cboLinea.Text].Value.ToString()))
                    .Select(p => p.Cells[cboLinea.Text].Value.ToString())
                    .Distinct()
                    .Select(r => new lineaDto
                    {
                        v_Nombre = r.Trim()
                    });

                var f = new frmMigrarLineas(lineas);
                f.ShowDialog();
            }
        }
    }
}
