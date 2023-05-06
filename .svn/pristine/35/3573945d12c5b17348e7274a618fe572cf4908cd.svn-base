using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using SAMBHS.Compra.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Almacen.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Security.BL;
using SAMBHS.Contabilidad.BL;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using System.Threading;
using System.Threading.Tasks;
using SAMBHS.Compra.BL;
using SAMBHS.Tesoreria.BL;
namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaImportaciones : Form
    {

        string _strFilterExpression,pstrIdImportacion;
        importacionDto _objImportacionDto = new importacionDto();
        ImportacionBL _objImportacionBL = new ImportacionBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        DiarioBL _objDiarioBL = new DiarioBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        SecurityBL _obSecurityBL = new SecurityBL();
        CierreMensualBL _objCierreMensualBL = new CierreMensualBL();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private Task _tarea;
        bool _btnNuevo, _btnEliminar, _btnEditar;
        public string Modulo = "";
        public frmBandejaImportaciones(string cadena)
        {
            InitializeComponent();
            Modulo = cadena;
        }
        private void frmBandejaImportaciones_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperatioResult = new OperationResult();
            if (Modulo == "C") // Acciones para modulo Contable
            {
                btnNuevo.Enabled = true;
            }
            else// Acciones para modulo Administrativo
            {

                #region ControlAcciones
                var _formActions = _obSecurityBL.GetFormAction(ref objOperatioResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmBandejaImportaciones", Globals.ClientSession.i_RoleId);

                _btnNuevo = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaImportaciones_New", _formActions);
                _btnEliminar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaImportaciones_Delete", _formActions);
                _btnEditar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaImportaciones_Edit", _formActions);

                btnNuevo.Enabled = _btnNuevo;
                btnEliminar.Enabled = _btnEliminar;
                btnEditar.Enabled = _btnEditar;
                #endregion
            }
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            CargarCombos();
            //btnRegenerarDiarios.Visible = Globals.ClientSession.i_SystemUserId == 1;
            //btnRegenerarNotasIngreso.Visible = Globals.ClientSession.i_SystemUserId == 1;
        }
        
        #region ComportamientoControles
        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }
        private void txtMes_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
        }

        private void txtCorrelativoMes_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoMes, "{0:00000000}");
        }

        private void txtNumeroDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtNumeroDoc, "{0:00000000}");
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
            OperationResult objOperationResult = new OperationResult();
            List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult);
            if (cboDocumento.Text.Trim() == "")
            {
                cboDocumento.Value = "-1";
            }
            else
            {
                var x = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento.Value.ToString() | p.Id == cboDocumento.Text);
                if (x == null)
                {
                    cboDocumento.Value = "-1";
                }

            }
        }


        private void cboSerieDoc_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboSerieDoc.Rows)
            {
                if (cboSerieDoc.Value == null) return;
                if (cboSerieDoc.Value.ToString() == "-1") cboSerieDoc.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboSerieDoc.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboSerieDoc.Text.ToUpper()))
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

        private void cboSerieDoc_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboSerieDoc.Rows)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    return;
                }
                bool filterRow = true;
                foreach (UltraGridColumn column in cboSerieDoc.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboSerieDoc.Text.ToUpper()))
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

        private void cboSerieDoc_Leave(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            List<GridKeyValueDTO> _ListadoSeries = new List<GridKeyValueDTO>();
            _ListadoSeries = _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 53, null);


            if (cboSerieDoc.Text.Trim() == "")
            {

                cboSerieDoc.Value = "-1";
            }
            else
            {
                var x = _ListadoSeries.Find(p => p.Id == cboSerieDoc.Value.ToString() | p.Id == cboSerieDoc.Text);
                if (x == null)
                {

                    cboSerieDoc.Value = "-1";
                }
            }


        }
        #endregion
        #region Búsqueda

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            OperationResult objOperationResult = new OperationResult();
           
            if (txtMes.Text != "") Filters.Add("v_Mes==\"" + txtMes.Text + "\"");
            if (txtCorrelativoMes.Text != "") Filters.Add("v_Correlativo==\"" + txtCorrelativoMes.Text + "\"");
            if (cboAlmacen.Value.ToString() != "-1") Filters.Add("i_IdAlmacen==" + cboAlmacen.Value.ToString());
            if (cboVia.Value.ToString() != "-1") Filters.Add("i_IdTipoVia==" + cboVia.Value.ToString());
            if (cboMoneda.Value.ToString() != "-1") Filters.Add("i_IdMoneda==" + cboMoneda.Value.ToString());           
            if (cboSerieDoc.Value.ToString() != "-1") Filters.Add("i_IdSerieDocumento==" + cboSerieDoc.Value.ToString());
            if (txtNumeroDoc.Text != "") Filters.Add("v_CorrelativoDocumento==\"" + txtNumeroDoc.Text + "\"");
            if (cboDocumento.Value.ToString() != "-1") Filters.Add("i_IdTipoDocumento==" +cboDocumento.Value.ToString ());
            Filters.Add("i_IdEstablecimiento==" + Globals.ClientSession.i_IdEstablecimiento.Value.ToString ());

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
            var objData = GetData("NroRegistro ASC", _strFilterExpression);
            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
        }

        private List<importacionDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();

            var _objData = _objImportacionBL.ListarBusquedaImportacion(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Value.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59"));

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private void MantenerSeleccion(string ValorSeleccionado)
        {
            //foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            //{
            //    if (row.Cells["v_IdImportacion"].Text == ValorSeleccionado)
            //    {
            //        grdData.ActiveRow = row;
            //        grdData.ActiveRow.Selected = true;
            //        break;
            //    }
            //}
            if (string.IsNullOrEmpty(ValorSeleccionado)) return;
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            var fila = filas.FirstOrDefault(f => f.Cells["v_IdImportacion"].Value.ToString().Contains(ValorSeleccionado));
            if (fila != null) fila.Activate();
        }

        #endregion
        #region Grilla
        private void grdData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].Columns["i_IdEstado"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {

            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
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
                if (Modulo == "C")
                {
                    btnEditar.Enabled = true;
                    btnEliminar.Enabled = true;
                    btnEliminar.Enabled = _objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_Mes"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Compras) ? false : true;

                }
                else
                {
                    btnEditar.Enabled = _btnEditar;
                    btnEliminar.Enabled = _btnEliminar;
                    btnEliminar.Enabled = _objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_Mes"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Compras) ? false : _btnEliminar;
                }
               // btnEliminar.Enabled = _objCierreMensualBL.VerificarMesCerrado(grdData.Rows[grdData.ActiveRow.Index].Cells["v_Periodo"].Value.ToString(), grdData.Rows[grdData.ActiveRow.Index].Cells["NroRegistro"].Value.ToString().Trim().Substring(0, 2), "COMPRAS") ? false : true;
               
            }
        }
        private void grdData_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                if (btnEditar.Enabled) btnEditar_Click(sender, e);
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
        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            if (grdData.ActiveRow != null)
            {
                if (Modulo == "C")
                {
                    btnEliminar.Enabled = true;
                    btnEditar.Enabled = true;
                    btnEliminar.Enabled = _objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_Mes"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Compras) ? false : true;
                }
                else
                {

                    btnEliminar.Enabled = _btnEliminar;
                    btnEditar.Enabled = _btnEditar;
                    btnEliminar.Enabled = _objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_Mes"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Compras) ? false : _btnEliminar;
                }
               
            }
            else
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
            }
        }
        #endregion
        #region CRUD
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmRegistroImportacion))) return;
            frmRegistroImportacion frm = new frmRegistroImportacion("Nuevo", "", Modulo);
            frm.FormClosed += (_, ev) =>
            {
                BindGrid();
                MantenerSeleccion((_ as frmRegistroImportacion)._pstrIdMovimiento_Nuevo);
            };
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
        }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;

            if (Utils.Windows.HaveFormChild(this, typeof(frmRegistroImportacion), true)) return;
            new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
            pstrIdImportacion = grdData.ActiveRow.Cells["v_IdImportacion"].Value.ToString();
            frmRegistroImportacion frm = new frmRegistroImportacion("Edicion", pstrIdImportacion,Modulo);
            frm.FormClosed += (_, ev) =>
            {
                BindGrid();
                MantenerSeleccion(pstrIdImportacion);
            };
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            OperationResult _objOperationResult = new OperationResult();
            if (UltraMessageBox.Show("¿Está seguro de Eliminar está importación de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                pstrIdImportacion = grdData.ActiveRow.Cells["v_IdImportacion"].Value.ToString();
                _objImportacionBL.EliminarImportacion(ref _objOperationResult, pstrIdImportacion, Globals.ClientSession.GetAsList());
                btnBuscar_Click(sender, e);
            }
        }

        #endregion
        #region Validaciones/Otros Procedimientos

        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();

            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboVia, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 49, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboSerieDoc, "Value2", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 53, null), DropDownListAction.Select);
            cboDocumento.Value = "-1";
            cboSerieDoc.Value = "-1";
            cboVia.Value = "-1";
            cboAlmacen.Value = "-1";
            cboMoneda.Value = "-1";
        }
        
        
        
        
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
        

        #endregion

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void chkBandejaAgrupable_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.HacerGrillaAgrupable(grdData, chkBandejaAgrupable.Checked);
        }

       

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells["i_IdEstado"].Value.ToString() == "0")
            {
                e.Row.Appearance.BackColor = Color.Salmon;
                e.Row.Appearance.BackColor2 = Color.Salmon;
                e.Row.Appearance.ForeColor = Color.Black;
            }
        }

        private void btnRegenerarDiarios_Click(object sender, EventArgs e)
        {
            AlmacenBL _objAlmacenBL = new AlmacenBL();
            OperationResult objOperationResult = new OperationResult();
          
            if (!(System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).IsBussy())
            {
                if (UltraMessageBox.Show("¿Seguro de Regenerar todos Diario Importación " , "Mensaje de Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.ProgressbarStatus.i_Progress = 1;
                    Globals.ProgressbarStatus.i_TotalProgress = 1;
                    Globals.ProgressbarStatus.b_Cancelado = false;
                    bwkProcesoBL.RunWorkerAsync();
                    (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();
                }
            }
        }

        private void bwkProcesoBL_DoWork(object sender, DoWorkEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            MovimientoBL _objMovimientoBL = new MovimientoBL();
            string Error = "";
          
            _objDiarioBL.RegeneraDiariosImportacion( ref objOperationResult);

            if (objOperationResult.Success == 1)
            {
                UltraMessageBox.Show("Proceso terminado correctamente!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Globals.ProgressbarStatus.b_Cancelado = true;
                UltraMessageBox.Show("Ocurrió un Error al regenerar Diarios  en " + Error + " Adicional :\n\n" + objOperationResult.ErrorMessage + "\n\n" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
        }

        private void btnRegenerarNotasIngreso_Click(object sender, EventArgs e)
        {
            AlmacenBL _objAlmacenBL = new AlmacenBL();
            OperationResult objOperationResult = new OperationResult();

            if (!(System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).IsBussy())
            {
                if (UltraMessageBox.Show("¿Seguro de Regenerar todaslas Notas Ingreso ", "Mensaje de Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.ProgressbarStatus.i_Progress = 1;
                    Globals.ProgressbarStatus.i_TotalProgress = 1;
                    Globals.ProgressbarStatus.b_Cancelado = false;
                    bwkProcesoIngresos.RunWorkerAsync();
                    (System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).ComenzarBackGroundProcess();
                }
            }
        }

        private void bwkProcesoIngresos_DoWork(object sender, DoWorkEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            
            ImportacionBL _objImportacionBL= new ImportacionBL ();
            string Error = "";


            _objImportacionBL.ActualizaNotadeIngresoDesdeImportacion(ref objOperationResult);
            if (objOperationResult.Success == 1)
            {
                UltraMessageBox.Show("Proceso terminado correctamente!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Globals.ProgressbarStatus.b_Cancelado = true;
                UltraMessageBox.Show("Ocurrió un Error al regenerar Ingresos Almacén  en " + Error + " Adicional :\n\n" + objOperationResult.ErrorMessage + "\n\n" + objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
        }
    }
}
