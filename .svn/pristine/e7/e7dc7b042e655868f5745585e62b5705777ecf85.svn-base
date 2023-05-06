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
using SAMBHS.CommonWIN.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    public partial class frmMigrarDocumentos : Form
    {
        public frmMigrarDocumentos()
        {
            InitializeComponent();
        }

        private void btnImportarExcel_Click(object sender, EventArgs e)
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

            grdData.Text = string.Format("Se encontraron {0} registros", grdData.Rows.Count());

            foreach (UltraGridCell cell in grdData.Rows[0].Cells)
            {
                string ColumnKey = cell.Column.Key;
                if (ColumnKey != "Nuevo")
                {
                    cboCodigo.Items.Add(ColumnKey);
                    cboNombre.Items.Add(ColumnKey);
                    cboNroCuenta.Items.Add(ColumnKey);
                    cboSiglas.Items.Add(ColumnKey);
                    cboUsadoCompras.Items.Add(ColumnKey);
                    cboUsadoContabilidad.Items.Add(ColumnKey);
                    cboUsadoTesoreria.Items.Add(ColumnKey);
                    cboUsadoVentas.Items.Add(ColumnKey);
                }
            }
            List<string> Columnas = cboCodigo.Items.Cast<string>().ToList();
            cboCodigo.SelectedIndex = Columnas.FindIndex(p => p.Equals("CODIGO"));
            cboNombre.SelectedIndex = Columnas.FindIndex(p => p.Equals("NOMBRE"));
            cboNroCuenta.SelectedIndex = Columnas.FindIndex(p => p.Equals("CUENTA"));
            cboSiglas.SelectedIndex = Columnas.FindIndex(p => p.Equals("SIGLA"));
            cboUsadoCompras.SelectedIndex = Columnas.FindIndex(p => p.Equals("COMPRAS"));
            cboUsadoContabilidad.SelectedIndex = Columnas.FindIndex(p => p.Equals("CONTABILIDAD"));
            cboUsadoTesoreria.SelectedIndex = Columnas.FindIndex(p => p.Equals("TESORERIA"));
            cboUsadoVentas.SelectedIndex = Columnas.FindIndex(p => p.Equals("VENTAS"));

            grdData.DisplayLayout.Bands[0].Columns.Add("EsCtaValida", "Cta. Válida");
            foreach (var row in grdData.Rows)
            {
                if (row.Cells[cboNroCuenta.Text].Value != null && !row.Cells[cboNroCuenta.Text].Value.ToString().Trim().Equals(""))
                {
                    row.Cells["EsCtaValida"].Value = Utils.Windows.EsCuentaImputable(row.Cells[cboNroCuenta.Text].Value.ToString().Trim());
                }
            }
        }

        private void frmMigrarDocumentos_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;

        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            var filasValidas = grdData.Rows.Where(p => !string.IsNullOrWhiteSpace(p.Cells[cboSiglas.Text].Value.ToString()));
            var objOperationResult = new OperationResult();
            using (var ts = TransactionUtils.CreateTransactionScope())
            {
                if (checkBox1.Checked && !new DocumentoBL().EliminaDocumentosParaMigracion(ref objOperationResult))
                {
                    if (objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show(
                            objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage +
                            "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    foreach (var fila in filasValidas)
                    {
                        var documento = new documentoDto();
                        int codDoc;
                        int.TryParse(fila.Cells[cboCodigo.Text].Value.ToString(), out codDoc);
                        documento.i_CodigoDocumento = codDoc;
                        documento.v_Siglas = fila.Cells[cboSiglas.Text].Value.ToString().Trim().ToUpper();
                        documento.v_Nombre = fila.Cells[cboNombre.Text].Value.ToString().Trim().ToUpper();
                        documento.v_NroCuenta = fila.Cells[cboNroCuenta.Text].Value.ToString().Trim();
                        documento.i_OperacionTransitoria = 0;
                        documento.i_UsadoCompras =
                            !string.IsNullOrWhiteSpace(fila.Cells[cboUsadoCompras.Text].Value.ToString()) && fila.Cells[cboUsadoCompras.Text].Value.ToString().Trim().Equals("S") ? 1 : 0;
                        documento.i_UsadoContabilidad =
                            !string.IsNullOrWhiteSpace(fila.Cells[cboUsadoContabilidad.Text].Value.ToString()) && fila.Cells[cboUsadoContabilidad.Text].Value.ToString().Trim().Equals("S") ? 1 : 0;
                        documento.i_UsadoVentas =
                            !string.IsNullOrWhiteSpace(fila.Cells[cboUsadoVentas.Text].Value.ToString()) && fila.Cells[cboUsadoVentas.Text].Value.ToString().Trim().Equals("S") ? 1 : 0;
                        documento.i_UsadoPedidoCotizacion = 0;
                        documento.i_UsadoDocumentoContable = 1;
                        documento.i_UsadoDocumentoInterno = 0;
                        documento.i_Naturaleza = 1;
                        documento.i_RequiereSerieNumero = 0;
                        documento.i_UsadoTesoreria =
                            !string.IsNullOrWhiteSpace(fila.Cells[cboUsadoTesoreria.Text].Value.ToString()) && fila.Cells[cboUsadoTesoreria.Text].Value.ToString().Trim().Equals("S") ? 1 : 0;
                        documento.i_UsadoLibroDiario = documento.i_CodigoDocumento >= 200 &&
                                                       documento.i_CodigoDocumento < 300
                            ? 1
                            : 0;

                        documento.i_UsadoDocumentoInverso = documento.i_CodigoDocumento == 7 ? 1 : 0;
                        documento.i_Destino = 0;
                        new DocumentoBL().DocumentoNuevo(ref objOperationResult, documento,
                            Globals.ClientSession.GetAsList());
                        if (objOperationResult.Success == 0)
                        {
                            UltraMessageBox.Show(
                                objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage +
                                "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                    }
                }


                ts.Complete();
            }
        }
    }
}
