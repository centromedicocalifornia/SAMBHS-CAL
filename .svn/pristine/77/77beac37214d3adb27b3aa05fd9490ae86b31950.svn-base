using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
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
    public partial class frmMigrarTipoCambio : Form
    {
        TipoCambioBL _objTipoCambioBL = new TipoCambioBL();

        public frmMigrarTipoCambio(string N)
        {
            InitializeComponent();
        }

        private void btnImportar_Click(object sender, EventArgs e)
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
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return;
            }

            grdData.SetDataBinding(this.ultraDataSource1, "Band 0");

            foreach (UltraGridCell cell in grdData.Rows[0].Cells)
            {
                string ColumnKey = cell.Column.Key;
                if (ColumnKey != "Nuevo")
                {
                    cboFecha.Items.Add(ColumnKey);
                    cboValorCompra.Items.Add(ColumnKey);
                    cboValorVenta.Items.Add(ColumnKey);
                }
            }
        }

        private void frmMigrarTipoCambio_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            cboFecha.SelectedIndex = 0;
            cboValorCompra.SelectedIndex = 0;
            cboValorVenta.SelectedIndex = 0;
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (UltraMessageBox.Show("¿Seguro de Continuar?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes )
            {
                try
                {
                    if (grdData.Rows.Count() > 0 && uvDatos.Validate(true, false).IsValid)
                    {
                        string ColumnaFecha = cboFecha.Text;
                        string ColumnaValorCompra = cboValorCompra.Text;
                        string ColumnaValorVenta = cboValorVenta.Text;

                        List<UltraGridRow> Filas = grdData.Rows.Where(p => p.Cells[ColumnaFecha].Value != null && !string.IsNullOrEmpty(p.Cells[ColumnaFecha].Value.ToString()) && p.Cells[ColumnaValorCompra].Value != null && p.Cells[ColumnaValorVenta].Value != null).ToList();

                        List<tipodecambioDto> ListaTipoCambiosXIngresar = new List<tipodecambioDto>();

                        foreach (UltraGridRow Fila in Filas.AsParallel())
                        {
                            Fila.Activate();
                            tipodecambioDto _tipodecambioDto = new tipodecambioDto();
                            _tipodecambioDto.d_FechaTipoC = DateTime.Parse(Fila.Cells[ColumnaFecha].Value.ToString());
                            _tipodecambioDto.d_ValorCompra = Decimal.Parse(Fila.Cells[ColumnaValorCompra].Value.ToString());
                            _tipodecambioDto.d_ValorVenta = Decimal.Parse(Fila.Cells[ColumnaValorVenta].Value.ToString());
                            _tipodecambioDto.d_ValorVentaContable = Decimal.Parse(Fila.Cells[ColumnaValorVenta].Value.ToString());
                            _tipodecambioDto.d_ValorCompraContable = Decimal.Parse(Fila.Cells[ColumnaValorCompra].Value.ToString());
                            ListaTipoCambiosXIngresar.Add(_tipodecambioDto);
                        }

                        OperationResult objOperationResult = new OperationResult();

                        _objTipoCambioBL.InsertarTiposCambios(ref objOperationResult, ListaTipoCambiosXIngresar, Globals.ClientSession.GetAsList());

                        if (objOperationResult.Success == 1)
                        {
                            UltraMessageBox.Show("Los Tipos de Cambio se Insertaron Correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    UltraMessageBox.Show(ex.Message);
                    return;
                }
            }
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Count() > 0 && cboFecha.Text != "--Seleccionar--")
            {
                if (UltraMessageBox.Show("¿Seguro de Convertir la Fecha?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        grdData.Rows.Where(p => p.Cells[cboFecha.Text].Value != null && !string.IsNullOrEmpty(p.Cells[cboFecha.Text].Value.ToString())).ToList()
                                    .ForEach(o => o.Cells[cboFecha.Text].Value = DateTime.FromOADate(double.Parse(o.Cells[cboFecha.Text].Value.ToString())));
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
