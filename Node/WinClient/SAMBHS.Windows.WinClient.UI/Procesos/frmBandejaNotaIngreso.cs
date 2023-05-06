using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using SAMBHS.Almacen.BL;
using Infragistics.Win.UltraWinGrid;
using LoadingClass;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using System.Threading;
namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaNotaIngreso : Form
    {
        #region Fields
        private readonly MovimientoBL _objMovimientoBl = new MovimientoBL();
        private Task _tarea;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private string _strFilterExpression;
        private string _pstrIdMovimiento, _vIdCliente = string.Empty;
        #endregion

        #region Init
        public frmBandejaNotaIngreso(string arg)
        {
            InitializeComponent();
        }

        private void frmBandejaNotaIngreso_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            var objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboMotivo, "Value1", "Id", new DatahierarchyBL().GetDataHierarchiesForCombo(ref objOperationResult, 19, null), DropDownListAction.All);
            if (Globals.ClientSession.UsuarioEsContable == 1)
            {
                Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", new NodeWarehouseBL().ObtenerAlmacenesParaCombo(ref objOperationResult, null,-1), DropDownListAction.All);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", new NodeWarehouseBL().ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento ?? 1), DropDownListAction.All);
            }
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            var fecha = DateTime.Parse(string.Format("{0}/{1}/{2}", DateTime.Today.Day, DateTime.Today.Month, Globals.ClientSession.i_Periodo));
            dtpFechaInicio.Value = fecha;
            dtpFechaFin.Value = fecha;
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
            cboMotivo.Value = "-1";

            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            cboMoneda.Value = "-1";
        }
        #endregion

        #region Event UI
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmNotaIngreso))) return;
            var frm = new frmNotaIngreso("Nuevo", "");
            frm.FormClosed += (_, ev) =>
            {
                BindGrid();
                MantenerSeleccion(((frmNotaIngreso)_)._pstrIdMovimiento_Nuevo);
            };
            ((frmMaster)MdiParent).RegistrarForm(this, frm);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var filters = new Queue<string>();
            if (cboMotivo.Value.ToString() != "-1") filters.Enqueue("i_IdTipoMotivo==" + cboMotivo.Value);
            if (cboAlmacen.Value.ToString() != "-1") filters.Enqueue("i_IdAlmacenOrigen==" + cboAlmacen.Value);

            //if (Globals.ClientSession.UsuarioEsContable == 0)
            //{
            //    filters.Enqueue("i_IdEstablecimiento==" + (Globals.ClientSession.i_IdEstablecimiento ?? 1));
            //}
            if (_vIdCliente != string.Empty) filters.Enqueue("v_IdCliente==\"" + _vIdCliente + "\"");
            if (cboMoneda.Value.ToString() != "-1") filters.Enqueue("Moneda==\"" + (cboMoneda.Value.ToString().Equals("1") ? "S" : "D") + "\"");
            _strFilterExpression = string.Join(" && ", filters);
            using (new PleaseWait(Location, "Por favor espere..."))
            {
                BindGrid();
            }

            if (!grdData.Rows.Any())
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
            }
        }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null && !grdData.ActiveRow.IsGroupByRow && !grdData.ActiveRow.IsFilterRow)
            {
                if (Utils.Windows.HaveFormChild(this, typeof(frmNotaIngreso), true)) return;
                _pstrIdMovimiento = grdData.ActiveRow.Cells["v_IdMovimiento"].Value.ToString();
                var frm = new frmNotaIngreso("Edicion", _pstrIdMovimiento);
                frm.FormClosed += delegate
                {
                    BindGrid();
                    MantenerSeleccion(_pstrIdMovimiento);
                };
                ((frmMaster)MdiParent).RegistrarForm(this, frm);
            }
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsGroupByRow || grdData.ActiveRow.IsFilterRow) return;
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
                btnEliminar.Enabled = grdData.Rows[grdData.ActiveRow.Index].Cells["RegistroOrigen"].Value == null || grdData.Rows[grdData.ActiveRow.Index].Cells["RegistroOrigen"].Value.ToString() == "CARGA PERIODO:2016 01-12";
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow.Index < 0) return;
            var objOperationResult = new OperationResult();
            if (UltraMessageBox.Show("¿Está seguro de Eliminar este movimiento de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _pstrIdMovimiento = grdData.ActiveRow.Cells["v_IdMovimiento"].Value.ToString();
                using (new PleaseWait(Location, "Por favor espere..."))
                {
                    using (var ts = TransactionUtils.CreateTransactionScope())
                    {
                        _objMovimientoBl.EliminarMovimiento(ref objOperationResult, _pstrIdMovimiento, Globals.ClientSession.GetAsList());
                        if (objOperationResult.Success == 0)
                        {
                            MessageBox.Show(
                                objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                                objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }

                        new ProductoBL().EliminarRecetaMovimiento(ref objOperationResult, _pstrIdMovimiento);
                        if (objOperationResult.Success == 0)
                        {
                            MessageBox.Show(
                                objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                                objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                        ts.Complete();
                    }
                }
                btnBuscar_Click(sender, e);
            }
        }
        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            if (grdData.ActiveRow != null)
            {
                btnEliminar.Enabled = grdData.ActiveRow.Cells["RegistroOrigen"].Value == null || grdData.ActiveRow.Cells["RegistroOrigen"].Value.ToString() == "CARGA PERIODO:2016 01-12";
                btnEditar.Enabled = true;
            }
            else
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
            }

        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void dtpFechaInicio_ValueChanged(object sender, EventArgs e)
        {

        }

        private void grdData_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            if (grdData.ActiveRow != null)
            {
                if (btnEditar.Enabled) btnEditar_Click(sender, e);
            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Control && e.KeyCode == Keys.F)
            //{
            //    if (grdData.DisplayLayout.Override.FilterUIType == FilterUIType.Default)
            //    {
            //        grdData.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;
            //    }
            //    else
            //    {
            //        grdData.DisplayLayout.Override.FilterUIType = FilterUIType.Default;
            //    }
            //}
        }

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            //if (e.Button.Key == "btnBuscarCliente")
            //{
            //    OperationResult objOperationResult = new OperationResult();
            //    Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("V", txtCliente.Text.Trim());
            //    frm.ShowDialog();

            //    if (frm._IdCliente != null)
            //    {

            //        txtCliente.Text = frm._NroDocumento;
            //        txtRazonSocial.Text = frm._RazonSocial;
            //        v_IdCliente = frm._IdCliente;
            //    }
            //    else
            //    {

            //    }
            //}

            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {

                txtCliente.Text = frm._NroDocumento;
                txtRazonSocial.Text = frm._RazonSocial;
                _vIdCliente = frm._IdProveedor;

            }
        }

        private void txtCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCliente, e);
        }

        private void txtCliente_Validated(object sender, EventArgs e)
        {
            if (txtCliente.Text.Trim() != string.Empty)
            {
                var objOperationResult = new OperationResult();
                var client = new ClienteBL().ObtenerClienteCodigoBandejas(ref objOperationResult, txtCliente.Text.Trim(), "V");
                if (client != null)
                {
                    txtRazonSocial.Text = string.Join(" ", client.v_ApePaterno, client.v_ApeMaterno.Trim(), client.v_PrimerNombre.Trim(), client.v_SegundoNombre.Trim(), client.v_RazonSocial.Trim()).Trim();
                    _vIdCliente = client.v_IdCliente;
                }
                else
                {
                    txtRazonSocial.Clear();
                    _vIdCliente = "-1";
                }

            }
            else
            {

                txtRazonSocial.Clear();
                _vIdCliente = string.Empty;
            }
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Bandeja Notas de Salida";

            using (var sf = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = @"xlsx files (*.xlsx)|*.xlsx",
                FileName = dummyFileName
            })
            {
                if (sf.ShowDialog() != DialogResult.OK) return;
                var filename = sf.FileName;
                btnExportarBandeja.Appearance.Image = Resource.loadingfinal1;
                _tarea = Task.Factory.StartNew(() =>
                {
                    using (var exporter = new UltraGridExcelExporter())
                        exporter.Export(grdData, filename);
                }, _cts.Token)
                .ContinueWith(t => ActualizarLabel("Bandeja Exportada a Excel."), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }
        #endregion

        #region Methods
        private void BindGrid()
        {
            var objData = GetData("v_MesCorrelativo ASC", _strFilterExpression);
            if (objData == null) return;
            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<movimientoDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            var objOperationResult = new OperationResult();
            var objData = _objMovimientoBl.ListarBusquedaMovimientos(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, dtpFechaFin.Value, (int)TipoDeMovimiento.NotadeIngreso, Globals.ClientSession.UsuarioEsContable == 0 ?Globals.ClientSession.i_IdEstablecimiento.Value : -1);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return objData;
        }

        private void MantenerSeleccion(string valorSeleccionado)
        {
            foreach (var row in grdData.Rows)
            {
                if (row.Cells["v_IdMovimiento"].Text != valorSeleccionado) continue;
                grdData.ActiveRow = row;
                grdData.ActiveRow.Selected = true;
                break;
            }
        }

        private void ActualizarLabel(string texto)
        {
            lblDocumentoExportado.Text = texto;
            btnExportarBandeja.Enabled = false;
            btnExportarBandeja.Appearance.Image = Resource.accept;
        }
        #endregion

    }
}
