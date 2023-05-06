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
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    public partial class frmCompararSaldosMigracionCuentas : Form
    {
        public frmCompararSaldosMigracionCuentas()
        {
            InitializeComponent();
        }

        private void btnCargarSaldosSistema_Click(object sender, EventArgs e)
        {
            ultraGrid1.DataSource = SaldoContableBL.CargarSaldosParaComparacion(rbCuenta12.Checked ? "12" : "42");
            ultraGrid1.DisplayLayout.Bands[0].Columns.Add("_Estado", "Estado");
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

            grdData.Text = string.Format("Se encontraron {0} registros", grdData.Rows.Count());
        }

        private void ObtenerEstado(UltraGridRow filaActiva, bool activar = false)
        {
            try
            {
                decimal saldoSoles;
                decimal saldoDolares;
                if (filaActiva == null) return;
                var nroCuenta = filaActiva.Cells["Cuenta"].Value.ToString().Trim();
                var rucAnexo = filaActiva.Cells["RucAnexo"].Value.ToString().Trim();
                var nroDocumento = filaActiva.Cells["Documento"].Value.ToString().Trim();
                decimal.TryParse(filaActiva.Cells["SaldoSoles"].Value.ToString(), out saldoSoles);
                decimal.TryParse(filaActiva.Cells["SaldoDolares"].Value.ToString(), out saldoDolares);

                var filaRelacionada =
                    grdData.Rows.FirstOrDefault(f => f.Cells["CUEN_3"].Value.ToString().Contains(nroCuenta) &&
                                                     f.Cells["DOCUMENTO"].Value.ToString().Contains(nroDocumento) &&
                                                     f.Cells["RUCANEXO"].Value.ToString().Contains(rucAnexo));
                if (filaRelacionada != null)
                {
                    decimal saldoSolesFox;
                    decimal saldoDolaresFox;
                    decimal.TryParse(filaRelacionada.Cells["IMP1_3"].Value.ToString(), out saldoSolesFox);
                    decimal.TryParse(filaRelacionada.Cells["IMP2_3"].Value.ToString(), out saldoDolaresFox);
                    filaActiva.Cells["_Estado"].SetValue(saldoSoles != saldoSolesFox ? "SALDO_DIFERENTE" : "OK", false);
                    if (activar) filaRelacionada.Activate();
                }
                else
                    filaActiva.Cells["_Estado"].SetValue("SALDO_NO_ENCONTRADO", false);
            }
            catch 
            {
                if (filaActiva != null) filaActiva.Cells["_Estado"].SetValue("*ERROR*", false);
            }
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            var filas = ultraGrid1.Rows.ToList();
            filas.ForEach(f => ObtenerEstado(f));
        }

        private void ultraGrid1_AfterRowActivate(object sender, EventArgs e)
        {
            if (ultraGrid1.ActiveRow != null)
                ObtenerEstado(ultraGrid1.ActiveRow, true);
        }

        private void ultraButton3_Click(object sender, EventArgs e)
        {
            try
            {
                const string dummyFileName = "Diferencias de Saldos";

                SaveFileDialog sf = new SaveFileDialog();
                sf.DefaultExt = "xlsx";
                sf.Filter = @"xlsx files (*.xlsx)|*.xlsx";
                sf.FileName = dummyFileName;

                if (sf.ShowDialog() != DialogResult.OK) return;
                using (new PleaseWait(this.Location, "Exportando excel..."))
                {
                    ultraGridExcelExporter1.Export(ultraGrid1, sf.FileName);
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
            }
        }
    }
}
