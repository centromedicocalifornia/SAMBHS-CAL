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
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmActualizarCostosNotaIngreso : Form
    {
        public List<ActualizarCostoProducto> Resultado {
            get { return _result; }
        }

        private List<ActualizarCostoProducto> _result;

        public frmActualizarCostosNotaIngreso()
        {
            InitializeComponent();
        }

        private void frmActualizarCostosNotaIngreso_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
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
                            this.ultraDataSource1.Rows.Add(rowData.ToArray());
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            continue;
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
            this.ultraGrid1.SetDataBinding(this.ultraDataSource1, "Band 0");

            foreach (UltraGridCell cell in ultraGrid1.Rows[0].Cells)
            {
                string ColumnKey = cell.Column.Key;
                if (ColumnKey != "Nuevo")
                {
                    cboColumnaCodigo.Items.Add(ColumnKey);
                    cboColumnaCosto.Items.Add(ColumnKey);
                }
            }

        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
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

        public class ActualizarCostoProducto
        {
            public string CodProducto { get; set; }
            public decimal Costo { get; set; }
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(cboColumnaCodigo.Text) && !string.IsNullOrWhiteSpace(cboColumnaCosto.Text))
                {
                    var filasValidas = ultraGrid1.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
                    filasValidas = filasValidas.Where(p => p.Cells[cboColumnaCodigo.Text].Value != null && 
                        p.Cells[cboColumnaCosto.Text].Value != null && 
                        !string.IsNullOrWhiteSpace(p.Cells[cboColumnaCodigo.Text].Value.ToString()) && 
                        !string.IsNullOrWhiteSpace(p.Cells[cboColumnaCosto.Text].Value.ToString())).ToList();

                    _result = filasValidas.Select(f =>
                    {
                        decimal costo;
                        return new ActualizarCostoProducto
                        {
                            CodProducto = f.Cells[cboColumnaCodigo.Text].Value.ToString().Trim(),
                            Costo = decimal.TryParse(f.Cells[cboColumnaCosto.Text].Value.ToString(), out costo) ? costo : 0
                        };
                    }).ToList();

                    DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
