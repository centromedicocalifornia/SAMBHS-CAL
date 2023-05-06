using Infragistics.Documents.Excel;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.Contabilidad.BL;
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
    public partial class frmMigrarPlanCuentas : Form
    {
        AsientosContablesBL _objAsientosContablesBL = new AsientosContablesBL();
        public frmMigrarPlanCuentas(string N)
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
                    cboNroCuenta.Items.Add(ColumnKey);
                    cboNombre.Items.Add(ColumnKey);
                    cboNivel.Items.Add(ColumnKey);
                    cboDetalle.Items.Add(ColumnKey);
                    cboMoneda.Items.Add(ColumnKey);
                    cboNaturaleza.Items.Add(ColumnKey);
                    cboAnalisis.Items.Add(ColumnKey);

                    cboCuentaDestino.Items.Add(ColumnKey);
                    cboCuentaOrigen.Items.Add(ColumnKey);
                    cboCuentaTransferencia.Items.Add(ColumnKey);
                    cboPorcentaje.Items.Add(ColumnKey);
                }
            }
        }

        private void frmMigrarPlanCuentas_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            cboNroCuenta.SelectedIndex = 0;
            cboNombre.SelectedIndex = 0;
            cboNivel.SelectedIndex = 0;
            cboDetalle.SelectedIndex = 0;
            cboMoneda.SelectedIndex = 0;
            cboNaturaleza.SelectedIndex = 0;
            cboAnalisis.SelectedIndex = 0;

            cboPorcentaje.SelectedIndex = 0;
            cboCuentaTransferencia.SelectedIndex = 0;
            cboCuentaOrigen.SelectedIndex = 0;
            cboCuentaDestino.SelectedIndex = 0;
            EsPlanContable(true);
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {

            if (UltraMessageBox.Show("¿Seguro de Insertar?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                if (rbPlanContable.Checked)
                {
                    if (uvDatos.Validate(true, false).IsValid && grdData.Rows.Count() > 0)
                    {
                        try
                        {
                            OperationResult objOperationResult = new OperationResult();

                            using (new LoadingClass.PleaseWait(this.Location, "Insertando Plan Contable"))
                            {
                                string ColumnaNroCuenta = cboNroCuenta.Text;
                                string ColumnaNombre = cboNombre.Text;
                                string ColumnaNivel = cboNivel.Text;
                                string ColumnaDetalle = cboDetalle.Text;
                                string ColumnaAnalisis = cboAnalisis.Text;
                                string ColumnaMoneda = cboMoneda.Text;
                                string ColumnaNaturaleza = cboNaturaleza.Text;
                                var Filas = grdData.Rows.Where(p => p.Cells[ColumnaNombre].Value != null && !string.IsNullOrEmpty(p.Cells[ColumnaNombre].Value.ToString())).ToList();

                                Filas = Filas.GroupBy(p => p.Cells[ColumnaNroCuenta].Value.ToString().Trim()).Select(p => p.FirstOrDefault()).ToList() ;

                                List<asientocontableDto> ListaXIngresar = new List<asientocontableDto>();

                                foreach (UltraGridRow Fila in Filas.AsParallel())
                                {
                                    asientocontableDto _asientocontableDto = new asientocontableDto();
                                    _asientocontableDto.v_NombreCuenta = Fila.Cells[ColumnaNombre].Value.ToString().Trim();
                                    _asientocontableDto.v_NroCuenta = Fila.Cells[ColumnaNroCuenta].Value.ToString().Trim();
                                    _asientocontableDto.i_LongitudJerarquica = Fila.Cells[ColumnaNroCuenta].Value.ToString().Trim().Length;
                                    _asientocontableDto.i_IdentificaCtaBancos = 0;
                                    _asientocontableDto.i_Imputable = Fila.Cells[ColumnaNivel].Value != null && Fila.Cells[ColumnaNivel].Value.ToString().Trim() == "S" ? 1 : 0;
                                    _asientocontableDto.i_Naturaleza = Fila.Cells[ColumnaNaturaleza].Value != null && Fila.Cells[ColumnaNaturaleza].Value.ToString().Trim() == "A" ? 2 : 1;
                                    _asientocontableDto.i_Detalle = Fila.Cells[ColumnaDetalle].Value != null && Fila.Cells[ColumnaDetalle].Value.ToString().Trim() == "S" ? 1 : 0;
                                    _asientocontableDto.i_Analisis = Fila.Cells[ColumnaAnalisis].Value != null && Fila.Cells[ColumnaAnalisis].Value.ToString().Trim() == "S" ? 1 : 0;
                                    _asientocontableDto.i_IdMoneda = _asientocontableDto.i_Imputable == 1 ? Fila.Cells[ColumnaMoneda].Value != null && Fila.Cells[ColumnaMoneda].Value.ToString().Trim() == "S" ? 1 : 2 : -1;
                                    _asientocontableDto.i_ACM = 0;
                                    _asientocontableDto.i_AplicacionDestino = 0;
                                    _asientocontableDto.i_CentroCosto = 0;
                                    _asientocontableDto.i_RendicionFondos = 0;
                                    _asientocontableDto.i_PermiteItem = 0;
                                    _asientocontableDto.i_EntFinanciera = 0;
                                    _asientocontableDto.i_EsActivo = 1;
                                    _asientocontableDto.i_ActivarRubro = 0;
                                    ListaXIngresar.Add(_asientocontableDto);
                                }

                                _objAsientosContablesBL.AsientosNuevos(ref objOperationResult, ListaXIngresar, Globals.ClientSession.GetAsList());
                            }

                            if (objOperationResult.Success == 1)
                            {
                                UltraMessageBox.Show("Plan de Cuentas insertado correctamente.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            UltraMessageBox.Show(ex.Message);
                        }
                    }
                }
                else
                {
                    if (cboCuentaDestino.Text == "--Seleccionar--" || cboCuentaOrigen.Text == "--Seleccionar--" || cboCuentaTransferencia.Text == "--Seleccionar--" || cboPorcentaje.Text == "--Seleccionar--")
                    {
                        UltraMessageBox.Show("Por favor eliga correctamente las columnas equivalentes", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    List<destinoDto> ListaXInsertar = new List<destinoDto>();
                    AsientosContablesBL _objAsientosContablesBL = new AsientosContablesBL();
                    List<UltraGridRow> FilasDestinos = grdData.Rows.Where(p => Utils.Windows.EsCuentaImputable(p.Cells[cboCuentaDestino.Text].Value.ToString()) && Utils.Windows.EsCuentaImputable(p.Cells[cboCuentaOrigen.Text].Value.ToString()) && Utils.Windows.EsCuentaImputable(p.Cells[cboCuentaTransferencia.Text].Value.ToString())).ToList();
                    OperationResult objOperationResult = new OperationResult();

                    foreach (UltraGridRow FilaDestino in FilasDestinos)
                    {
                        destinoDto _destinoDto = new destinoDto();
                        _destinoDto.v_CuentaDestino = FilaDestino.Cells[cboCuentaDestino.Text].Value.ToString().Trim();
                        _destinoDto.v_CuentaOrigen = FilaDestino.Cells[cboCuentaOrigen.Text].Value.ToString().Trim();
                        _destinoDto.v_CuentaTransferencia = FilaDestino.Cells[cboCuentaTransferencia.Text].Value.ToString().Trim();
                        _destinoDto.i_Porcentaje = int.Parse(FilaDestino.Cells[cboPorcentaje.Text].Value.ToString());
                        ListaXInsertar.Add(_destinoDto);
                    }

                    _objAsientosContablesBL.DestinoNuevoMigracion(ref objOperationResult, Globals.ClientSession.GetAsList(), ListaXInsertar);

                    if (objOperationResult.Success == 1)
                    {
                        UltraMessageBox.Show("Importación finalizada");
                    }
                    else
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void rbPlanContable_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPlanContable.Checked)
            {
                EsPlanContable(true);
            }
        }

        void EsPlanContable(bool Estado)
        {
            if (!Estado)
            {
                cboAnalisis.Enabled = false;
                cboDetalle.Enabled = false;
                cboMoneda.Enabled = false;
                cboNaturaleza.Enabled = false;
                cboNivel.Enabled = false;
                cboNombre.Enabled = false;
                cboNroCuenta.Enabled = false;

                cboCuentaDestino.Enabled = true;
                cboCuentaOrigen.Enabled = true;
                cboCuentaTransferencia.Enabled = true;
                cboPorcentaje.Enabled = true;
            }
            else
            {
                cboAnalisis.Enabled = true;
                cboDetalle.Enabled = true;
                cboMoneda.Enabled = true;
                cboNaturaleza.Enabled = true;
                cboNivel.Enabled = true;
                cboNombre.Enabled = true;
                cboNroCuenta.Enabled = true;

                cboCuentaDestino.Enabled = false;
                cboCuentaOrigen.Enabled = false;
                cboCuentaTransferencia.Enabled = false;
                cboPorcentaje.Enabled = false;
            }
        }

        private void rbDestinos_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDestinos.Checked)
            {
                EsPlanContable(false);
            }
        }

    }
}
 