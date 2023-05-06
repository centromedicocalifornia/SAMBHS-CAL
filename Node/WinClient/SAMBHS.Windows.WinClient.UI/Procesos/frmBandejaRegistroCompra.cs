using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Compra.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Contabilidad.BL;
using SAMBHS.Letras.BL;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using System.Threading;
namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaRegistroCompra : Form
    {
        #region Fields
        private Task _tarea;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private readonly ComprasBL _objComprasBl = new ComprasBL();
        private readonly DocumentoBL _objDocumentoBl = new DocumentoBL();
        private List<GridKeyValueDTO> _listadoComboDocumentos = new List<GridKeyValueDTO>();
        readonly CierreMensualBL _objCierreMensualBl = new CierreMensualBL();
        private string _strFilterExpression, _pstrIdCompra, _vIdProveedor = string.Empty;
        #endregion


        public frmBandejaRegistroCompra(string arg)
        {
            InitializeComponent();
        }

        private void frmBandejaRegistroCompra_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            _listadoComboDocumentos = _objDocumentoBl.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 1, 0);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _listadoComboDocumentos, DropDownListAction.All);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            cboMoneda.Value = "-1";

            cboTipoDocumento.Value = "-1";
            var fecha = new DateTime(Globals.ClientSession.i_Periodo ?? DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            dtpFechaInicio.Value = fecha;
            dtpFechaFin.Value = fecha;
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
            regularRegistroCompras.Visible = Globals.ClientSession.i_SystemUserId.Equals(1);
            var banda = grdData.DisplayLayout.Bands[0];
            banda.Summaries["Total"].CustomSummaryCalculator = new TotalCustomSoles("d_Total");
        }

        private void BindGrid()
        {
            var objData = GetData("NroRegistro ASC", _strFilterExpression);

            grdData.DataSource = objData;
            if (objData != null)
            {
                if (objData.Count > 0)
                {
                    lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
                    btnEditar.Enabled = true;
                }
                else
                {
                    btnEditar.Enabled = false;
                    btnEliminar.Enabled = false;
                }

                if (Globals.ClientSession.UsuarioEsContable == 0) CierraOperacionesContables();
            }
        }

        private List<compraDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            var objOperationResult = new OperationResult();
            var objData = _objComprasBl.ListarBusquedaCompras(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, dtpFechaFin.Value,false , Globals.ClientSession.i_IdEstablecimiento?? -1);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return objData;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid)
            {
                var filters = new Queue<string>();
                if (txtMes.Text.Trim() != "" && txtCorrelativoMes.Text.Trim() != "")
                {
                    filters.Enqueue("v_Mes==\"" + txtMes.Text.Trim() + "\"");
                    filters.Enqueue("v_Correlativo==\"" + txtCorrelativoMes.Text.Trim() + "\"");
                }

                if (cboTipoDocumento.Value.ToString() != "-1")
                {
                    filters.Enqueue("i_IdTipoDocumento==" + cboTipoDocumento.Value);
                    filters.Enqueue("v_SerieDocumento==\"" + txtSerieDoc.Text.Trim() + "\"");
                    filters.Enqueue("v_CorrelativoDocumento==\"" + txtCorrelativoDoc.Text.Trim() + "\"");
                }

                if (_vIdProveedor != string.Empty) filters.Enqueue("v_IdProveedor==\"" + _vIdProveedor + "\"");
                if (cboMoneda.Value.ToString() != "-1") filters.Enqueue("Moneda==\"" + (cboMoneda.Value.ToString().Equals("1") ? "S" : "D") + "\"");
                _strFilterExpression = string.Join(" && ", filters);

                using (new LoadingClass.PleaseWait(Location, "Por favor espere..."))
                {
                    BindGrid();
                }
                grdData.Focus();
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmRegistroCompra))) return;
            var frm = new frmRegistroCompra("Nuevo", "");
            frm.FormClosed += (_, ev) =>
            {
                BindGrid();
                MantenerSeleccion(((frmRegistroCompra)_)._pstrIdMovimiento_Nuevo);
            };
            ((frmMaster)MdiParent).RegistrarForm(this, frm);
            
        }

        private void MantenerSeleccion(string valorSeleccionado)
        {
            if (string.IsNullOrEmpty(valorSeleccionado)) return;
            var filas = grdData.Rows.ToList();
            var fila = filas.FirstOrDefault(f => f.Cells["v_IdCompra"].Value.ToString().Contains(valorSeleccionado));
            if (fila != null) fila.Activate();
        }
                                          
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow ||
                grdData.ActiveRow.Activation == Activation.Disabled) return;
            if (Utils.Windows.HaveFormChild(this, typeof(frmRegistroCompra), true)) return;
            _pstrIdCompra = grdData.ActiveRow.Cells["v_IdCompra"].Value.ToString();

            var frm = new frmRegistroCompra("Edicion", _pstrIdCompra);            
            frm.FormClosed += (_, ev) =>
                {
                    BindGrid();
                    MantenerSeleccion(_pstrIdCompra);
                    btnEditar.Enabled = true;
                };
            ((frmMaster)MdiParent).RegistrarForm(this, frm); 
            
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow ||
               grdData.ActiveRow.Activation == Activation.Disabled) return;

            var objOperationResult = new OperationResult();
            _pstrIdCompra = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdCompra"].Value.ToString();
            
            if (new LetrasBL().CompraFueCanjeadaALetras(ref objOperationResult, _pstrIdCompra))
            {
                UltraMessageBox.Show("Esta compra fue canjeada en letras, no se puede eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            if (_objComprasBl.TienePagosRealizados(_pstrIdCompra))
            {
                UltraMessageBox.Show("Imposible Eliminar un Documento con Pagos Realizados", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            string nroTesoreria;
            if (!_objComprasBl.ComprobarSiFueLlamadoEnTesoreria(_pstrIdCompra, out nroTesoreria))
            {
                if (UltraMessageBox.Show("¿Está seguro de Eliminar esta compra de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_objComprasBl.CompraTieneGuiasRemisionEnlazadas(ref objOperationResult, _pstrIdCompra))
                    {
                        var resp =
                            UltraMessageBox.Show(
                                "La compra tiene una o más guías de remisión de compra enlazadas que se borrarán también, ¿Desea Continuar?",
                                "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (resp == DialogResult.No) return;
                    }

                    using (new LoadingClass.PleaseWait(Location, "Por favor espere..."))
                    {
                        _objComprasBl.EliminarCompra(ref objOperationResult, _pstrIdCompra, Globals.ClientSession.GetAsList());
                    }
                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    btnBuscar_Click(sender, e);
                }
            }
            else
                UltraMessageBox.Show(string.Format("El registro ya tiene registrado un pago en Tesorería [{0}], No se puede Eliminar", nroTesoreria), "Sistema");

        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            var point = new Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            if (grdData.ActiveRow != null)
            {
                if (grdData.ActiveRow.Index < 0) return;

              //  grdData.ActiveRow.Cells["v_Periodo"].Value.ToString()
                btnEliminar.Enabled = !_objCierreMensualBl.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_Mes"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Compras);
 
                if (btnEliminar.Enabled)
                {
                    //btnEliminar.Enabled = grdData.Rows[grdData.ActiveRow.Index].Cells["RegistroOrigen"].Value == null ? true : false;
                    btnEliminar.Enabled = grdData.ActiveRow.Cells["RegistroOrigen"].Value == null;
                }
                btnEditar.Enabled = true;

            }
            else
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
            }
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdData.DisplayLayout.Bands[0].Columns["i_IdEstado"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            try
            {
                if (!e.Row.IsFilterRow && !e.Row.IsGroupByRow && e.Row.Cells["i_IdEstado"].Value.ToString() == "0")
                {
                    e.Row.Appearance.BackColor = Color.Salmon;
                    e.Row.Appearance.BackColor2 = Color.Salmon;
                    e.Row.Appearance.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtCorrelativoMes_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoMes, "{0:00000000}");
        }

        private void txtMes_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
        }

        private void txtMes_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtMes, e);
        }

        private void txtCorrelativoMes_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoMes, e);
        }

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc, "{0:0000}");
        }

        private void txtCorrelativoDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDoc, "{0:00000000}");
        }

        private void txtSerieDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc, e);
        }

        private void txtCorrelativoDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDoc, e);
        }

        private void cboTipoDocumento_AfterDropDown(object sender, EventArgs e)
        {
            foreach (var row in cboTipoDocumento.Rows)
            {
                if ((string) cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
                bool filterRow = true;
                foreach (var column in cboTipoDocumento.DisplayLayout.Bands[0].Columns)
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
                row.Hidden = filterRow;

            }
        }

        private void cboTipoDocumento_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (var row in cboTipoDocumento.Rows)
            {
                if ((string) cboTipoDocumento.Value == "-1") cboTipoDocumento.Text = string.Empty;
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
                row.Hidden = filterRow;

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
                var x = _listadoComboDocumentos.Find(p => p.Id == (string) cboTipoDocumento.Value | p.Id == cboTipoDocumento.Text);
                if (x == null)
                {
                    cboTipoDocumento.Value = "-1";
                }
            }
        }

        private void cboTipoDocumento_ValueChanged(object sender, EventArgs e)
        {
            if ((string) cboTipoDocumento.Value != "-1")
            {
                txtSerieDoc.Enabled = true;
                txtCorrelativoDoc.Enabled = true;
            }
            else
            {
                txtSerieDoc.Clear();
                txtCorrelativoDoc.Clear();
                txtSerieDoc.Enabled = false;
                txtCorrelativoDoc.Enabled = false;
            }
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            if (grdData.ActiveRow != null)
            {
                if (grdData.ActiveRow.Index < 0) return;
               // btnEliminar.Enabled = _objCierreMensualBL.VerificarMesCerrado(grdData.Rows[grdData.ActiveRow.Index].Cells["v_Periodo"].Value.ToString(), grdData.Rows[grdData.ActiveRow.Index].Cells["v_Mes"].Value.ToString().Trim().Substring(0, 2), "COMPRAS") ? false : true;
                btnEliminar.Enabled = !_objCierreMensualBl.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_Mes"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Compras);
                if (btnEliminar.Enabled)
                {
                    //btnEliminar.Enabled = grdData.Rows[grdData.ActiveRow.Index].Cells["RegistroOrigen"].Value == null ? true : false;
                    btnEliminar.Enabled = grdData.ActiveRow.Cells["RegistroOrigen"].Value == null;
                }
                btnEditar.Enabled = true;

            }
            else
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
            }
        }

        #region Buscar Proveedor
        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarProv")
            {
                using (var frm = new Mantenimientos.frmBuscarProveedor(txtRucProveedor.Text.Trim(), "RUC"))
                {
                    frm.ShowDialog();
                    if (frm._IdProveedor != null)
                    {
                        txtRucProveedor.Text = frm._NroDocumento + @" - " + frm._RazonSocial;
                        _vIdProveedor = frm._IdProveedor;
                        txtRucProveedor.ButtonsRight["btnEliminar"].Enabled = true;
                    } 
                }
            }
            else
            {
                txtRucProveedor.Clear();
                _vIdProveedor = string.Empty;
                txtRucProveedor.ButtonsRight["btnEliminar"].Enabled = false;
            }
        }
        #endregion

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

        private void CierraOperacionesContables()
        {
            var filas = grdData.Rows.ToList();
            filas.ForEach(fila =>
            {
                if (fila.Cells["v_UsuarioCreacion"].Value != null)
                {
                    var opContable = Globals.UsuariosContables.Contains(fila.Cells["v_UsuarioCreacion"].Value.ToString());
                    fila.Activation = opContable ? Activation.Disabled : Activation.ActivateOnly;
                }  
            } );
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void dtpFechaInicio_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Bandeja de Compras";

            using (var sf = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = @"xlsx files (*.xlsx)|*.xlsx",
                FileName = dummyFileName
            })
            {
                if (sf.ShowDialog() != DialogResult.OK) return;
                btnExportarBandeja.Appearance.Image = Resource.loadingfinal1;
                var filename = sf.FileName;
                _tarea = Task.Factory.StartNew(() =>
                {
                    using (var ultraGridExcelExporter1 = new UltraGridExcelExporter())
                        ultraGridExcelExporter1.Export(grdData, filename);
                }, _cts.Token)
                    .ContinueWith(t => ActualizarLabel("Bandeja Exportada a Excel."), TaskScheduler.FromCurrentSynchronizationContext());
            }

        }

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void chkBandejaAgrupable_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.HacerGrillaAgrupable(grdData, chkBandejaAgrupable.Checked);
        }
        private void ActualizarLabel(string texto)
        {
            lblDocumentoExportado.Text = texto;
            btnExportarBandeja.Enabled = false;
            btnExportarBandeja.Appearance.Image = Resource.accept;
        }

        private void regularRegistroCompras_Click(object sender, EventArgs e)
        {
            using(var f = new frmRegularRegistroCompras())
                f.ShowDialog();
        }

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            var idCompra = grdData.ActiveRow.Cells["v_IdCompra"].Value.ToString();
            var f = new frmConsultaHistorialPagosCompra(idCompra, frmConsultaHistorialPagosCompra.TipoBusqueda.Venta);
            f.ShowDialog();
        }
    }
}
