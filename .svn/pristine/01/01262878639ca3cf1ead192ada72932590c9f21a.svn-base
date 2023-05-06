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
using SAMBHS.Common.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos.Migraciones
{
    public partial class frmUbigeoCliente : Form
    {
        private readonly SystemParameterBL _objSystemParameterBL = new SystemParameterBL();
        private List<KeyValueDTO> ListaDepartamentos = new List<KeyValueDTO>();
        private List<KeyValueDTO> ListaUbigeo = new List<KeyValueDTO>();

        public frmUbigeoCliente()
        {
            InitializeComponent();
        }

        private void ultraButton2_Click(object sender, EventArgs e)
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

                Workbook workbook = Workbook.Load(sFileName);
                Worksheet worksheet = workbook.Worksheets[0];

                this.ultraDataSource1.Reset();

                bool isHeaderRow = true;
                foreach (WorksheetRow row in worksheet.Rows)
                {
                    try
                    {
                        if (isHeaderRow)
                        {
                            foreach (WorksheetCell cell in row.Cells)
                            {

                                string columnKey = cell.GetText();

                                UltraDataColumn column = this.ultraDataSource1.Band.Columns.Add(columnKey);

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
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        continue;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            grdData.SetDataBinding(this.ultraDataSource1, "Band 0");


            foreach (UltraGridCell cell in grdData.Rows[0].Cells)
            {
                string ColumnKey = cell.Column.Key;
                if (ColumnKey != "Nuevo")
                {
                    cboDepartamento.Items.Add(ColumnKey);
                    cboDistrito.Items.Add(ColumnKey);
                    cboProvincia.Items.Add(ColumnKey);
                    cboCodigo.Items.Add(ColumnKey);
                }
            }
        }

        private void frmUbigeoCliente_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            ListaDepartamentos = _objSystemParameterBL.GetSystemParameterForComboUbigeoKeyValueDto(ref objOperationResult, 1, 112, null);
            ListaUbigeo = _objSystemParameterBL.GetSystemParameterForComboKeyValueDto(ref objOperationResult, 112, null);
        }

        public string ObtenerDetapartamentoId(List<KeyValueDTO> ListaDepartamentos, string NombreDepartamento)
        {
            try
            {
                if (string.IsNullOrEmpty(NombreDepartamento.Trim())) return "-1";
                var reslt = ListaDepartamentos.FirstOrDefault(p => !string.IsNullOrEmpty(p.Value1.Trim()) && p.Value1.Trim().Contains(NombreDepartamento.Trim()));
                return reslt != null ? reslt.Id : "-1";
            }
            catch (Exception)
            {
                return "-1";
            }
        }

        public string ObtenerProvincia(string idDepartamento, List<KeyValueDTO> ListaUbigeo, string NombreProvincia)
        {
            try
            {
                var reslt = ListaUbigeo.Where(x => x.Value3.Trim().Equals(idDepartamento.Trim())).FirstOrDefault(p => p.Value1.Contains(NombreProvincia.Trim()));
                return reslt != null ? reslt.Id : "-1";
            }
            catch (Exception)
            {
                return "-1";
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            filas.ForEach(fila =>
            {
                var celdaDepartamento = fila.Cells[cboDepartamento.Text];
                if (celdaDepartamento.Value != null)
                {
                    celdaDepartamento.Value = ObtenerDetapartamentoId(ListaDepartamentos, celdaDepartamento.Value.ToString());

                    var celdaProvincia = fila.Cells[cboProvincia.Text];
                    if (celdaProvincia.Value != null)
                    {
                        celdaProvincia.Value = ObtenerProvincia(celdaDepartamento.Value.ToString(), ListaUbigeo, celdaProvincia.Value.ToString());
                    }
                }
                else
                    celdaDepartamento.Value = "-1";
            });
        }

        private void ultraButton3_Click(object sender, EventArgs e)
        {
            try
            {
                var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
                var lista = filas.Select(fila => new KeyValueDTO
                {
                    Id = fila.Cells[cboCodigo.Text].Value.ToString(),
                    Value1 = !string.IsNullOrEmpty(cboDepartamento.Text) ? fila.Cells[cboDepartamento.Text].Value.ToString() : "-1",
                    Value3 = !string.IsNullOrEmpty(cboDistrito.Text) ? fila.Cells[cboDistrito.Text].Value.ToString() : "-1",
                    Value2 = !string.IsNullOrEmpty(cboProvincia.Text) ? fila.Cells[cboProvincia.Text].Value.ToString() : "-1"
                }).ToList();

                var objOperationResult = new OperationResult();
                ClienteBL.ActualizarUbigeoPorCliente(ref objOperationResult, lista);
                if (objOperationResult.Success == 0)
                {
                    MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
