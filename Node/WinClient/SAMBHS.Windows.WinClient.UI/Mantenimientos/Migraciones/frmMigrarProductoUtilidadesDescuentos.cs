using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using LoadingClass;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    public partial class frmMigrarProductoUtilidadesDescuentos : Form
    {
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        public frmMigrarProductoUtilidadesDescuentos()
        {
            InitializeComponent();
        }

        private void frmMigrarProductoUtilidadesDescuentos_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            cboCodProd.SelectedIndex = 0;
            cboMonto.SelectedIndex = 0;
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboListaPrecios, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 47, null), DropDownListAction.Select);
            cboListaPrecios.Value = "-1";
        }

        private void ultraButton4_Click(object sender, EventArgs e)
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

            foreach (UltraGridCell cell in grdData.Rows[0].Cells)
            {
                string ColumnKey = cell.Column.Key;
                cboCodProd.Items.Add(ColumnKey);
                cboMonto.Items.Add(ColumnKey);
            }
            ultraButton4.Enabled = !grdData.Rows.Any();

        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            try
            {
                ProductoBL objProductoBl = new ProductoBL();
                OperationResult objOperationResult = new OperationResult();

                var idListaPrecio = int.Parse(cboListaPrecios.Value.ToString());
                if ((!rbCostos.Checked && idListaPrecio == -1) || !grdData.Rows.Any()) return;
                var filas = grdData.Rows.Where(
                        p =>
                            p.Cells[cboCodProd.Text].Value != null &&
                            !string.IsNullOrEmpty(p.Cells[cboCodProd.Text].Value.ToString())
                            && p.Cells[cboMonto.Text].Value != null && 
                            !string.IsNullOrEmpty(p.Cells[cboMonto.Text].Value.ToString())).ToList();
                decimal mon = 0;

                var listaResult = filas.Select(fila => new SAMBHS.Almacen.BL.ProductoBL.ProductoListaUtilidadDto
                {
                    Monto = decimal.TryParse(fila.Cells[cboMonto.Text].Value.ToString().Trim(), out mon) ? mon : 0,
                    CodProducto = fila.Cells[cboCodProd.Text].Value.ToString()
                })
                .ToList();

                ultraButton1.Enabled = false;
                using (new PleaseWait(this.Location, "Por favor espere..."))
                {
                    if (!rbCostos.Checked)
                    {
                        objProductoBl.ActualizarListaPreciosUtilidadMigracion(ref objOperationResult, idListaPrecio,
                            listaResult,
                            radioButton1.Checked
                                ? ProductoBL.ProductoListaTipo.Utilidad
                                : ProductoBL.ProductoListaTipo.Descuento);
                    }
                    else
                    {
                        objProductoBl.ActualizarListaPreciosUtilidadMigracion(ref objOperationResult, idListaPrecio,
                            listaResult, ProductoBL.ProductoListaTipo.Costo);
                    }
                }

                if (objOperationResult.Success == 0)
                    MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage,
                        @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(@"Datos Actualizados", @"Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ultraButton1.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void rbCostos_CheckedChanged(object sender, EventArgs e)
        {
            cboListaPrecios.Enabled = !rbCostos.Checked;
        }
    }
}
