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
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Compra.BL;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmRegularRegistroCompras : Form
    {
        public frmRegularRegistroCompras()
        {
            InitializeComponent();
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

        }

        private void frmRegularRegistroCompras_Load(object sender, EventArgs e)
        {

        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            //var filas = grdData.Rows.Select(f => new compraDto
            //{
            //    v_IdCompra =  f.GetCellValue("v_IdCompra").ToString(),
            //    v_Mes = f.GetCellValue("v_Mes").ToString(),
            //    v_Correlativo = f.GetCellValue("v_Correlativo").ToString(),
            //}).ToList();

            //var objOperationResult = new OperationResult();
            //ComprasBL.RegularRegistrosCompras(ref objOperationResult, filas);

            //if (objOperationResult.Success == 0)
            //{
            //    MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            //MessageBox.Show(@"Operation Completada");

            try
            {
                var filas = grdData.Rows.Where(p => p.Cells["NCOM_3"].Value.ToString().Length > 0).AsParallel().Select(f => new compraDto
                {
                    NroRegistro = f.GetCellValue("NCOM_3").ToString(),
                    v_SerieDocumento = f.GetCellValue("SFACTU_3").ToString(),
                    i_IdTipoDocumento = int.Parse(f.GetCellValue("TIDOC_3").ToString()),
                    v_CorrelativoDocumento = f.GetCellValue("NFACTU_3").ToString(),
                    CodigoProveedor = f.GetCellValue("FICH_3").ToString()
                }).ToList();
                var objOperationResult = new OperationResult();
                ComprasBL.RegularRegistrosComprasRollavel(ref objOperationResult, filas);

                if (objOperationResult.Success == 0)
                {
                    MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage,
                        @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show(@"Operation Completada");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
