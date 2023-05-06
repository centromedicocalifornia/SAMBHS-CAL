//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Contabilidad.BL;
using SAMBHS.Security.BL;
using SAMBHS.Venta.BL;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using System.Threading;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaReciboHonoarios : Form
    {

        string _strFilterExpression, pstrIdReciboHonorarios;
        ReciboHonorarioBL _objReciboHonoraioBL = new ReciboHonorarioBL();
        SecurityBL _obSecurityBL = new SecurityBL();
        public string v_IdProveedor = string.Empty;
        bool _btnNuevo, _btnEliminar, _btnEditar;
        CierreMensualBL _objCierreMensualBL = new CierreMensualBL();
        ClienteBL _objClienteBL = new ClienteBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private Task _tarea;
        public int _TipoDoc = -1;
        public string _Serie = string.Empty ,_IdReciboHonorario;
        public string _Correlativo = string.Empty;
        public string Referencia = string.Empty;
        private bool _edicionNroRegistros = false;
        public frmBandejaReciboHonoarios(string cadena)
        {
            InitializeComponent();
            Referencia = cadena;
        }
        private void frmBandejaReciboHonoarios_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            _ListadoComboDocumentos = _objReciboHonoraioBL.ObtenerDocumentosParaComboGridReciboHonorarios(ref objOperationResult, null);
            Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", _objReciboHonoraioBL.ObtenerDocumentosParaComboGridReciboHonorarios(ref objOperationResult, null), DropDownListAction.Select);
            cboDocumento.Value = "2";
            #region ControlAcciones
            var _formActions = _obSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmBandejaReciboHonoarios", Globals.ClientSession.i_RoleId);

            _btnNuevo = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaReciboHonoarios_New", _formActions);
            _btnEliminar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaReciboHonoarios_Delete", _formActions);
            _btnEditar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaReciboHonoarios_Edit", _formActions);
            btnAgregar.Enabled = _btnNuevo;
            btnEliminar.Enabled = _btnEliminar;
            btnEditar.Enabled = _btnEditar;
            #endregion
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            ValidarFechas();
            if (Referencia == "DocRef")
            {
                this.Text = "Recibos por Honorarios";
                this.Size = new Size(970, 540);
                btnAgregar.Visible = false;
                btnEliminar.Visible = false;
                btnEditar.Visible = false;
                this.StartPosition = FormStartPosition.CenterParent;
            
            }
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            cboMoneda.Value = "-1";
        }
        #region Busqueda
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            OperationResult objOperationResult = new OperationResult();
            if (v_IdProveedor != string.Empty) Filters.Add("v_IdProveedor==\"" + v_IdProveedor + "\"");
            if (txtMes.Text !="" ) Filters.Add("v_Mes==\""+txtMes.Text+"\"");
            if(txtCorrelativo.Text!="") Filters.Add ("v_Correlativo==\"" + txtCorrelativo.Text+"\"");
            if (txtSerieDoc.Text != "") Filters.Add("v_SerieDocumento==\"" + txtSerieDoc.Text + "\"");
            if(txtCorrelativoDoc.Text !="") Filters.Add ("v_CorrelativoDocumento==\"" +txtCorrelativoDoc.Text+"\"");
            if (Referencia =="DocRef") Filters.Add ("i_IdTipoDocumento!=" + (int) TiposDocumentos.NotaCredito);
            if (cboMoneda.Value.ToString() != "-1") Filters.Add("Moneda==\"" + (cboMoneda.Value.ToString().Equals("1") ? "S" : "D") + "\"");
            _strFilterExpression = null;

            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }
            using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
            this.BindGrid();
        }

        private void BindGrid()
        {
            var objData = GetData("v_NumeroRegistro ASC", _strFilterExpression);

            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<recibohonorarioDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objReciboHonoraioBL.ListarBusquedaReciboHonorarios(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Value.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59"));

            
            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }
        #endregion
        #region ComportamientoControles
        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void chkBandejaAgrupable_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.HacerGrillaAgrupable(grdData, chkBandejaAgrupable.Checked);
        }


        private void txtCorrelativo_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativo, e);

        }

        private void txtSerieDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc, e);
        }

        private void txtCorrelativoDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDoc, e);
        }

        private void txtCorrelativo_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativo, "{0:00000000}");

        }

        private void txtCorrelativoDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDoc, "{0:00000000}");
            
        }

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc, "{0:0000}");
        }

        private void txtMes_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
        }

        private void txtSerieDoc_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                txtCorrelativoDoc.Focus();

            }
        }

        private void txtMes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
                if (txtMes.Text != "")
                {
                    int Mes;
                    Mes = int.Parse(txtMes.Text);
                    if (Mes < 1 | Mes > 12)
                    {
                        UltraMessageBox.Show("Mes inválido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Hand);

                    }
                    else
                    {
                        txtCorrelativo.Focus();
                    }

                }
            }

        }

        private void txtMes_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtMes, e);
        }
        private void txtCorrelativo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                txtSerieDoc.Focus();

            }
        }

        private void txtCorrelativoDoc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {



            }
        }
        #endregion

        #region CRUD

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmReciboHonorario))) return;
            frmReciboHonorario frm = new frmReciboHonorario("Nuevo", "");
            frm.FormClosed += (_, ev) =>
            {
                BindGrid();
                MantenerSeleccion((_ as frmReciboHonorario)._pstrIdMovimiento_Nuevo);
            };
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
        }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow || _edicionNroRegistros) return;
            if (grdData.ActiveRow != null)
            {
                if (Utils.Windows.HaveFormChild(this, typeof(frmReciboHonorario), true)) return;
                new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
                pstrIdReciboHonorarios = grdData.ActiveRow.Cells["v_IdReciboHonorario"].Value.ToString();
                frmReciboHonorario frm = new frmReciboHonorario("Edicion", pstrIdReciboHonorarios);
                #region Eventos
                frm.OnSiguiente += delegate
                {
                    grdData.PerformAction(UltraGridAction.BelowRow);
                    frm.IdRecibido = grdData.ActiveRow.Cells["v_IdReciboHonorario"].Value.ToString();
                };

                frm.OnAnterior += delegate
                {
                    grdData.PerformAction(UltraGridAction.AboveRow);
                    frm.IdRecibido = grdData.ActiveRow.Cells["v_IdReciboHonorario"].Value.ToString();
                };
                frm.FormClosed += (_, ev) =>
                {
                    BindGrid();
                    MantenerSeleccion(pstrIdReciboHonorarios);
                };
                #endregion
                (this.MdiParent as frmMaster).RegistrarForm(this, frm);
            }

        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow || _edicionNroRegistros) return;
            OperationResult _objOperationResult = new OperationResult();
            if (_objReciboHonoraioBL.TieneDocReferenciasAsociados(grdData.ActiveRow.Cells["i_IdTipoDocumento"].Value == null ? -1 : int.Parse(grdData.ActiveRow.Cells["i_IdTipoDocumento"].Value.ToString()), grdData.ActiveRow.Cells["v_SerieDocumento"].Value.ToString(), grdData.ActiveRow.Cells["v_CorrelativoDocumento"].Value.ToString()))
            {
                UltraMessageBox.Show("Este documento tiene asociados Notas de Crédito , no se puede eliminar.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            else
            {
                if (UltraMessageBox.Show("¿Está seguro de Eliminar este recibo de honorarios de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    pstrIdReciboHonorarios = grdData.ActiveRow.Cells["v_IdReciboHonorario"].Value.ToString();
                    _objReciboHonoraioBL.EliminarReciboHonorarios(ref _objOperationResult, pstrIdReciboHonorarios, Globals.ClientSession.GetAsList());
                    btnBuscar_Click(sender, e);
                }
            }
        }
        #endregion

        #region Grilla
        private void MantenerSeleccion(string ValorSeleccionado)
        {
            //foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            //{
            //    if (row.Cells["v_IdReciboHonorario"].Text == ValorSeleccionado)
            //    {
            //        grdData.ActiveRow = row;
            //        grdData.ActiveRow.Selected = true;
            //        break;
            //    }
            //}
            if (string.IsNullOrEmpty(ValorSeleccionado)) return;
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            var fila = filas.FirstOrDefault(f => f.Cells["v_IdReciboHonorario"].Value.ToString().Contains(ValorSeleccionado));
            if (fila != null) fila.Activate();
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].Columns["i_IdEstado"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
        }
        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow || _edicionNroRegistros) return;
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row == null)
            {
                btnEditar.Enabled = false;
                btnEliminar.Enabled = false;
            }
            else
            {
                btnEditar.Enabled = _btnEditar;
                btnEliminar.Enabled = _btnEliminar;
                btnEliminar.Enabled = _objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_Mes"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Compras) ? false : true;

            }
        }
        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow || _edicionNroRegistros) return;
            
            if (grdData.ActiveRow != null)
            {

                btnEliminar.Enabled = _btnEliminar;
                btnEditar.Enabled = _btnEditar;
                btnEliminar.Enabled = _objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_Mes"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Compras) ? false : true;


            }
            else
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
            }
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Bandeja Recibo Honorarios";
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

       
        private void ActualizarLabel(string Texto)
        {
            lblDocumentoExportado.Text = Texto;
            btnExportarBandeja.Enabled = false;
            btnExportarBandeja.Appearance.Image = Resource.accept;
        }

        
        #endregion
        #region Validaciones


        private void ValidarFechas()
        {
            string Periodo = Globals.ClientSession.i_Periodo.ToString();
            string Mes = DateTime.Today.Month.ToString();
            if (DateTime.Now.Year.ToString().Trim() == Periodo)
            {
                dtpFechaInicio.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaInicio.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), int.Parse(Mes))).ToString()).ToString());
                dtpFechaInicio.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

                dtpFechaFin.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/" + "01").ToString());
                dtpFechaFin.MaxDate = DateTime.Parse((Periodo + "/" + Mes + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), int.Parse(Mes))).ToString()).ToString());
                dtpFechaFin.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

            }
            else
            {
                dtpFechaInicio.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaInicio.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaInicio.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                dtpFechaFin.MinDate = DateTime.Parse((Periodo + "/" + "01" + "/01").ToString());
                dtpFechaFin.MaxDate = DateTime.Parse((Periodo + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(Periodo), 12)).ToString()).ToString());
                dtpFechaFin.Value = DateTime.Parse((Periodo + "/" + Mes + "/" + DateTime.Now.Date.Day.ToString()).ToString());

            }
        }
        private void grdData_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                if (Referencia == "DocRef")
                {
                    _TipoDoc = int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value.ToString());
                    _Serie = grdData.Rows[grdData.ActiveRow.Index].Cells["v_SerieDocumento"].Value.ToString();
                    _Correlativo = grdData.Rows[grdData.ActiveRow.Index].Cells["v_CorrelativoDocumento"].Value.ToString();
                    _IdReciboHonorario = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdReciboHonorario"].Value.ToString();
                    this.Close();
                }
                else
                {

                    if (btnEditar.Enabled) btnEditar_Click(sender, e);
                }
            }
        }

        private void txtProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarCliente")
            {
                Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor("", "");
                frm.ShowDialog();
                if (frm._IdProveedor != null)
                {
                    txtProveedor.Text = string.Format("{0} - {1}", frm._NroDocumento, frm._RazonSocial);
                    v_IdProveedor = frm._IdProveedor;
                    txtProveedor.ButtonsRight["btnEliminar"].Enabled = true;
                }
            }
            else
            {
                txtProveedor.Clear();
                v_IdProveedor = string.Empty;
                txtProveedor.ButtonsRight["btnEliminar"].Enabled = false;
            }
        }

        private void cboDocumento_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento.Rows)
            {
                if (cboDocumento.Value == null) return;
                if (cboDocumento.Value.ToString() == "-1") cboDocumento.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumento.Text.ToUpper()))
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

        private void cboDocumento_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento.Rows)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    return;
                }
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumento.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumento.Text.ToUpper()))
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

        private void cboDocumento_Leave(object sender, EventArgs e)
        {
            if (cboDocumento.Text.Trim() == "")
            {
                cboDocumento.Value = "-1";
            }
            else
            {
                var x = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento.Value.ToString() | p.Id == cboDocumento.Text | p.Value2 == cboDocumento.Text);
                if (x == null)
                {
                    cboDocumento.Value = "-1";
                }
            }
        }
        #endregion

        private void btnEditarNroRegistros_Click(object sender, EventArgs e)
        {
            if (!_edicionNroRegistros)
            {
                var g = new FrmRegenerarAsientosConfig();
                g.ShowDialog();
                if (g.DialogResult == DialogResult.OK)
                {
                    var banda = grdData.DisplayLayout.Bands[0];
                    _edicionNroRegistros = true;
                    var periodo = Globals.ClientSession.i_Periodo ?? DateTime.Now.Year;
                    var mesEscogido = g.MesProceso;
                    if (mesEscogido < dtpFechaInicio.Value.Month)
                    {
                        dtpFechaInicio.Value = new DateTime(periodo, mesEscogido, 1);
                        dtpFechaFin.Value = new DateTime(periodo, mesEscogido, DateTime.DaysInMonth(periodo, mesEscogido));
                    }
                    else
                    {
                        dtpFechaFin.Value = new DateTime(periodo, mesEscogido, DateTime.DaysInMonth(periodo, mesEscogido));
                        dtpFechaInicio.Value = new DateTime(periodo, mesEscogido, 1);
                    }

                    btnBuscar_Click(sender, e);
                    btnBuscar.Enabled = false;
                    dtpFechaInicio.Enabled = false;
                    dtpFechaFin.Enabled = false;
                    btnAgregar.Enabled = false;
                    btnEditar.Enabled = false;
                    btnEliminar.Enabled = false;
                    btnEditarNroRegistros.Enabled = false;
                    btnOK.Visible = true;
                    btnNO.Visible = true;
                    foreach (var column in banda.Columns)
                    {
                        column.CellActivation = !column.Key.Equals("v_NumeroRegistro") ? Activation.Disabled : Activation.AllowEdit;
                    }
                    banda.Columns["v_NumeroRegistro"].CellClickAction = CellClickAction.Edit;
                }
            }
        }

        private void grdData_BeforeExitEditMode(object sender, BeforeExitEditModeEventArgs e)
        {
            if (grdData.ActiveCell != null && grdData.ActiveCell.Column.Key.Equals("v_NumeroRegistro"))
            {

                var entidadOriginal = (recibohonorarioDto)grdData.ActiveRow.ListObject;
                if (entidadOriginal != null)
                {
                    if (grdData.ActiveCell.Text.Contains('-'))
                    {
                        grdData.ActiveCell.SetValue(entidadOriginal.v_NumeroRegistro, true);
                        return;
                    }
                    int n;
                    if (int.TryParse(grdData.ActiveCell.Text, out n))
                    {
                        var mesOriginal = entidadOriginal.v_Mes.Trim();
                        var almacenOriginal = entidadOriginal.v_NumeroRegistro.Split('-')[1].Substring(0, 2);
                        var regNuevo = string.Format("{0}-{1}{2}", mesOriginal, almacenOriginal, n.ToString("000000"));
                        grdData.ActiveCell.SetValue(regNuevo, true);
                    }
                    else
                        grdData.ActiveCell.SetValue(entidadOriginal.v_NumeroRegistro, true);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                var objOperationResult = new OperationResult();

                var listaEnvio = ((List<recibohonorarioDto>)grdData.DataSource).ToDictionary(p => p.v_NumeroRegistro, o => o.v_IdReciboHonorario);
                ReciboHonorarioBL.ModificarNroRegistrosReciboHonorario(ref objOperationResult, listaEnvio,Globals.ClientSession.GetAsList ());
                if (objOperationResult.Success == 0)
                {
                    if (objOperationResult.Success != 1)
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + Environment.NewLine +
                            objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                MessageBox.Show(@"Registros Actualizados");
                btnBuscar_Click(sender, e);
                btnNO_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Hay registros que se repiten, por favor intente de nuevo.", @"Error en el proceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNO_Click(object sender, EventArgs e)
        {
            _edicionNroRegistros = false;
            btnBuscar.Enabled = true;
            dtpFechaInicio.Enabled = true;
            dtpFechaFin.Enabled = true;
            btnAgregar.Enabled = true;
            btnEditar.Enabled = true;
            btnEliminar.Enabled = true;
            btnOK.Visible = false;
            btnNO.Visible = false;
            btnEditarNroRegistros.Enabled = true;
            var banda = grdData.DisplayLayout.Bands[0];
            foreach (var column in banda.Columns)
            {
                column.CellActivation = Activation.ActivateOnly;
            }
            banda.Columns["v_NumeroRegistro"].CellClickAction = CellClickAction.RowSelect;
        }

      

      

    }
}