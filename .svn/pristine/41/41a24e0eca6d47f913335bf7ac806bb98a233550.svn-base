using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinDataSource;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmMovimientosImportacionExcel : Form
    {
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        public BindingList<GridmovimientodetalleDto> ListaRetorno { get; set; }

        public frmMovimientosImportacionExcel()
        {
            InitializeComponent();
        }

        private void frmMovimientosImportacionExcel_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            cboColumnaPrecio.SelectedIndex = 0;
            cboColumnaNombre.SelectedIndex = 0;
            cboColumnaCodigo.SelectedIndex = 0;
            cboColumnaCantidad.SelectedIndex = 0;
            cboColumnaPedido.SelectedIndex = 0;

            #region OpenFile
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "Archivos Excel (*.*)| *.*";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;
            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                ProcesarExcel(choofdlog.FileName);
                Button btnCancel = new Button();
                this.CancelButton = btnCancel;
                btnCancel.Click += delegate { this.Close(); };
                cboColumnaCodigo.Focus();
            }
            else
                this.Close();
            #endregion
        }

        private void ProcesarExcel(string filename)
        {
            try
            {
                Workbook workbook = Workbook.Load(filename);
                Worksheet worksheet = workbook.Worksheets[0];
                ultraGrid1.Text = System.IO.Path.GetFileName(filename);
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
                                case "Codigo":
                                    column.DataType = typeof(string);
                                    break;
                                case "Descripcion":
                                    column.DataType = typeof(string);
                                    break;
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
                        // Get the data fom the excel row cells. 
                        List<object> rowData = new List<object>();
                        foreach (WorksheetCell cell in row.Cells)
                        {
                            rowData.Add(cell.Value);
                        }
                        try
                        {
                            ultraDataSource1.Rows.Add(rowData.ToArray());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        // Add a row to the UltraDataSource

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            // Bind the UltraDataSource to the grid. 
            ultraGrid1.SetDataBinding(ultraDataSource1, "Band 0");

            foreach (UltraGridCell cell in ultraGrid1.Rows[0].Cells)
            {
                string ColumnKey = cell.Column.Key;
                if (ColumnKey != "Nuevo")
                {
                    cboColumnaCantidad.Items.Add(ColumnKey);
                    cboColumnaCodigo.Items.Add(ColumnKey);
                    cboColumnaNombre.Items.Add(ColumnKey);
                    cboColumnaPrecio.Items.Add(ColumnKey);
                    cboColumnaPedido.Items.Add(ColumnKey);
                }
            }

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string ColumaCodigo = cboColumnaCodigo.Text;
                string txtColumnaNombre = cboColumnaNombre.Text;

                var Filas = ultraGrid1.Rows.Where(p => p.Cells[ColumaCodigo].Value.ToString() != string.Empty && p.Cells[txtColumnaNombre].Value.ToString() != string.Empty).ToList();
                List<GridmovimientodetalleDto> listaReturn = new List<GridmovimientodetalleDto>();
                var listaProductosTemporal = new ProductoBL().DevolverProductos();

                var compara1 = listaProductosTemporal.Select(p => p.Value1.Trim());
                var compara2 = ultraGrid1.Rows.Select(p => p.Cells[cboColumnaCodigo.Text].Value.ToString().Trim());

                var diferencias = compara2.Except(compara1);


                if (diferencias.Any())
                {
                    if (diferencias.Count() <= 16)
                    {
                        var listado = new StringBuilder();
                        diferencias.ToList().ForEach(p => listado.AppendLine("- " + p));
                        var resp = MessageBox.Show("Productos Que no Existen en la Base de datos:" + '\n' + listado.ToString() + '\n' + "¿Desea Continuar?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (resp == System.Windows.Forms.DialogResult.No) return;
                    }

                    var _resp =
                        MessageBox.Show(
                            "Hay demasiados artículos que no existen (" + diferencias.Count() + ") en la base de datos, ¿desea proseguir?", "Sistema",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

                    if (_resp == DialogResult.No) return;
                }

                foreach (UltraGridRow fila in Filas)
                {
                    var filaGrid = new GridmovimientodetalleDto();
                    int idUm;
                    decimal cantidad;
                    if(fila.Cells[ColumaCodigo].Value.ToString() == string.Empty) continue;
                    string codigo = fila.Cells[ColumaCodigo].Value.ToString().Trim();
                    var productoTemp = listaProductosTemporal.FirstOrDefault(p => p.Value1.Trim().Equals(codigo.Trim()));
                    if (productoTemp == null) continue;

                    filaGrid.i_IdUnidad = int.TryParse(productoTemp.Value5, out idUm) ? idUm : 5;
                    filaGrid.i_IdTipoDocumento = -1;
                    filaGrid.v_IdProductoDetalle = productoTemp.Value2;
                    filaGrid.d_Cantidad = decimal.TryParse(fila.Cells[cboColumnaCantidad.Text].Value.ToString(), out cantidad) ? cantidad : 0;
                    filaGrid.d_CantidadEmpaque = filaGrid.d_Cantidad;
                    filaGrid.d_Precio = decimal.TryParse(fila.Cells[cboColumnaPrecio.Text].Value.ToString(), out cantidad) ? cantidad : 0;
                    filaGrid.d_Total = filaGrid.d_Cantidad.Value * filaGrid.d_Precio.Value;
                    filaGrid.v_CodigoInterno = codigo;
                    filaGrid.v_NombreProducto = productoTemp.Value3;
                    filaGrid.i_IdUnidadMedidaProducto = idUm;
                    filaGrid.Empaque = productoTemp.Value6;
                    listaReturn.Add(filaGrid);
                }
                ListaRetorno = new BindingList<GridmovimientodetalleDto>(listaReturn);
                DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }


    }
}
