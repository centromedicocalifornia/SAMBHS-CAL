using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Compra.BL;
using SAMBHS.Contabilidad.BL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaPagos : Form
    {
        #region Fields
        string _strFilterExpression = null;
        private Task _tarea;
        private CancellationTokenSource cts = new CancellationTokenSource();
        PagoBL _objPagoBL = new PagoBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        CierreMensualBL _objCierreMensualBL = new CierreMensualBL();
        #endregion

        public frmBandejaPagos(string pstr)
        {
            InitializeComponent();
        }
        private void frmBandejaPagos_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            CargarCombos();
            var fecha = new DateTime(Globals.ClientSession.i_Periodo ?? 2016, DateTime.Today.Month, DateTime.Today.Day);
            dtpFechaInicio.Value = fecha;
            dtpFechaFin.Value = fecha;
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void CargarCombos()
        {
            var objOperationResult = new OperationResult();
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.All);
            cboTipoDocumento.Value = "-1";
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult);

            Utils.Windows.LoadUltraComboList(cboDocCompras, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            cboDocCompras.Value = "-1";
            cboMoneda.Value = "-1";
        }
        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        #region Busqueda
        private void BindGrid()
        {
            var objData = GetData("NroRegistro ASC", _strFilterExpression);
            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);

            if (objData != null)
            {
                if (objData.Count > 0)
                {

                    btnEditar.Enabled = true;
                    btnEliminar.Enabled = true;
                }
                else
                {
                    btnEditar.Enabled = false;
                    btnEliminar.Enabled = false;
                }
                if (Globals.ClientSession.UsuarioEsContable == 0) CierraOperacionesContables();
            }
        }
        private void CierraOperacionesContables()
        {
            //var filas = grdData.Rows.ToList();
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            filas.ForEach(fila =>
            {
                var opContable = Globals.UsuariosContables.Contains(fila.Cells["v_UsuarioCreacion"].Value.ToString());
                fila.Activation = opContable ? Activation.Disabled : Activation.ActivateOnly;
            });
        }

        private List<pagoDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objPagoBL.ListarBusquedaPagos(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Value.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59"),int.Parse ( cboDocCompras.Value.ToString ()),txtSerieDoc.Text.Trim (),txtCorrelativoDoc.Text.Trim ());
            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }
        #endregion

        #region Comportamiento Controles
        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Bandeja Pedidos";
            UltraGridExcelExporter ultraGridExcelExporter1 = new UltraGridExcelExporter();
            SaveFileDialog sf = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = @"xlsx files (*.xlsx)|*.xlsx",
                FileName = dummyFileName
            };

            if (sf.ShowDialog() != DialogResult.OK) return;
            btnExportarBandeja.Appearance.Image = Resource.loadingfinal1;

            _tarea = Task.Factory.StartNew(() => { ultraGridExcelExporter1.Export(grdData, sf.FileName); }, cts.Token)
                                 .ContinueWith(t => ActualizarLabel("Bandeja Exportada a Excel."), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void chkBandejaAgrupable_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.HacerGrillaAgrupable(grdData, chkBandejaAgrupable.Checked);
        }

        private void cboTipoDocumento_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboTipoDocumento.Rows)
            {
                if (cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()))
                        {
                            filterRow = false;
                            break;
                        }
                    }
                }
                if (filterRow)
                {
                    row.Hidden = true;
                }
                else
                {
                    row.Hidden = false;
                }

            }
        }
        private void cboTipoDocumento_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboTipoDocumento.Rows)
            {
                if (cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()))
                        {
                            filterRow = false;
                            break;
                        }
                    }
                }
                if (filterRow)
                {
                    row.Hidden = true;
                }
                else
                {
                    row.Hidden = false;
                }

            }
        }
        private void cboTipoDocumento_Leave(object sender, EventArgs e)
        {
            if (cboTipoDocumento.Text.Trim() == "")
            {
                cboTipoDocumento.Value = "-1";
            }
            else
            {
                var x = _ListadoComboDocumentos.Find(p => p.Id == cboTipoDocumento.Value | p.Id == cboTipoDocumento.Text);
                if (x == null)
                {
                    cboTipoDocumento.Value = "-1";
                }
            }
        }

        #endregion

        #region CRUD
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow ||
               grdData.ActiveRow.Activation == Activation.Disabled) return;
            if (grdData.ActiveRow != null)
            {
                if (grdData.ActiveRow.Index < 0) return;
                if (Utils.Windows.HaveFormChild(this, typeof(frmPago), true)) return;
                new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
                string pstrIdPago;
                pstrIdPago = grdData.ActiveRow.Cells["v_IdPago"].Value.ToString();
                frmPago frm = new frmPago("Edicion", pstrIdPago, null, false);
                frm.FormClosed += delegate
                {
                    BindGrid();
                    Utils.Windows.SelectRowForKeyValue(grdData, "v_IdPago", pstrIdPago);
                };
                (this.MdiParent as frmMaster).RegistrarForm(this, frm);
            }
        }
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmPago))) return;
            new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
            frmPago frm = new frmPago("Nuevo", null, null);
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow || grdData.ActiveRow.Activation == Activation.Disabled) return;

            if (grdData.ActiveRow.Index < 0) return;
            OperationResult _objOperationResult = new OperationResult();
            if (UltraMessageBox.Show("¿Está seguro de Eliminar este pago  de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                {
                    string pstrIdPago;
                    pstrIdPago = grdData.ActiveRow.Cells["v_IdPago"].Value.ToString();
                    _objPagoBL.EliminarPago(ref _objOperationResult, pstrIdPago, Globals.ClientSession.GetAsList());
                    if (_objOperationResult.Success == 0)
                    {

                        UltraMessageBox.Show("Ocurrió un Error al Eliminar Pago", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }
                    btnBuscar_Click(sender, e);
                }
            }
        }
        private void btnBuscar_Click(object sender, EventArgs e)
        {

            if (cboDocCompras.Value != null && cboDocCompras.Value.ToString() != "-1")
            {
                if (!validarDocCompra.Validate(true, false).IsValid)
                {
                    return;

                }
            }
            List<string> Filters = new List<string>();
            _strFilterExpression = null;
            if (cboTipoDocumento.Value.ToString() != "-1")
            {
                Filters.Add("i_IdTipoDocumento == " + cboTipoDocumento.Value.ToString());
            }

            if (txtCorrelativoMes.Text.Trim() != string.Empty && txtMes.Text.Trim() != string.Empty)
            {
                Filters.Add("v_Mes == \"" + txtMes.Text + "\" && " + "v_Correlativo == \"" + txtCorrelativoMes.Text + "\"");
            }
            if (cboMoneda.Value.ToString() != "-1") Filters.Add("Moneda==\"" + (cboMoneda.Value.ToString().Equals("1") ? "S" : "D") + "\"");
            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }

            using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
            {
                this.BindGrid();
            }
            if (grdData.Rows.Count() == 0)
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
            }
        }
        #endregion

        #region Clases/Validaciones
        private void ActualizarLabel(string Texto)
        {
            lblDocumentoExportado.Text = Texto;
            btnExportarBandeja.Enabled = false;
            btnExportarBandeja.Appearance.Image = Resource.accept;
        }
        #endregion

        #region Grilla
        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            if (grdData.ActiveRow != null)
            {
                //btnEditar.Enabled = _btnEditar == true ? true : false;
                //btnEliminar.Enabled = _btnEliminar == true ? true : false;

                btnEditar.Enabled = true;
                btnEliminar.Enabled = true;
                btnEliminar.Enabled = _objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_Mes"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Tesoreria) ? false : true;

            }
            else
            {
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
        #endregion

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;

            var point = new Point(e.X, e.Y);
            var uiElement = ((UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            var row = (UltraGridRow)uiElement.GetContext(typeof(UltraGridRow));

            if (row == null)
            {
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
            else
            {
                btnEditar.Enabled = true;
                if (btnEliminar.Enabled)
                {
                    btnEliminar.Enabled = !_objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_Mes"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Tesoreria);

                }
            }
        }

        private void txtMes_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int serie;
            txtMes.Text = int.TryParse(txtMes.Text.Trim(), out serie) ? serie.ToString("0000") : string.Empty;
        }

        private void txtCorrelativoMes_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int correlativo;
            txtCorrelativoMes.Text = int.TryParse(txtCorrelativoMes.Text.Trim(), out correlativo) ? correlativo.ToString("00000000") : string.Empty;
        }

        private void txtSerieDoc_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int serieDoc;
            txtSerieDoc.Text = int.TryParse(txtSerieDoc.Text.Trim(), out serieDoc) ? serieDoc.ToString("0000") : string.Empty;
        }

        private void txtCorrelativoDoc_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int correlativoDoc;
            txtCorrelativoDoc.Text = int.TryParse(txtCorrelativoDoc.Text.Trim(), out correlativoDoc) ? correlativoDoc.ToString("00000000") : string.Empty;
        }

        private void btnEliminarPagos_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow || grdData.ActiveRow.Activation == Activation.Disabled) return;

            if (grdData.ActiveRow.Index < 0) return;
            var objOperationResult = new OperationResult();
            if (UltraMessageBox.Show("¿Está seguro de Eliminar TODOS los pago  de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                {
                  
                    _objPagoBL.EliminarTodosPagos(ref objOperationResult, Globals.ClientSession.GetAsList());
                    if (objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show("Ocurrió un Error al Eliminar Pago", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    btnBuscar_Click(sender, e);
                }
            }
        }

       

    }
}
