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
using SAMBHS.Venta.BL;
using SAMBHS.Almacen.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Security.BL;
using SAMBHS.Contabilidad.BL;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using System.Threading;

namespace SAMBHS.Windows.WinClient.UI.Procesos    
{
    public partial class frmBandejaTransferencia : Form
    {
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        SecurityBL _obSecurityBL = new SecurityBL();
        CierreMensualBL _objCierreMensualBL = new CierreMensualBL();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private Task _tarea;
        string _strFilterExpression;
        string pstrIdMovimiento;
        bool _btnNuevo, _btnEliminar, _btnEditar;
        public frmBandejaTransferencia(string Parametro)
        {
            InitializeComponent();
        }
        private void frmBandejaTransferencia_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            #region ControlAcciones
            var _formActions = _obSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmBandejaTransferencia", Globals.ClientSession.i_RoleId);

            _btnNuevo = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaTransferencia_Add", _formActions);
            _btnEliminar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaTransferencia_Delete", _formActions);
            _btnEditar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaTransferencia_Edit", _formActions);
            btnAgregar.Enabled = _btnNuevo;
            btnEliminar.Enabled = _btnEliminar;
            btnEditar.Enabled = _btnEditar;
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            #endregion
            CargarCombos();
            ValidarFechas();
            btnRegenerarT.Visible = Globals.ClientSession.i_SystemUserId == 1;
            btnActualizarGuiasRemision.Visible = Globals.ClientSession.i_SystemUserId == 1;
            btnRegenerarIDs.Visible = Globals.ClientSession.i_SystemUserId == 1;
            
        }
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboMotivo, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 22, null), DropDownListAction.All);//20 numero del id Del motivo
            Utils.Windows.LoadUltraComboEditorList(cboAlmacenOrigen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            cboAlmacenOrigen.Value  = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString();
            if (cboAlmacenOrigen.Value == null)
            {

            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(cboAlmacenDestino, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaComboAll(ref objOperationResult, "i_IdAlmacen!=" + cboAlmacenOrigen.Value.ToString()), DropDownListAction.Select);
            }
           // cboAlmacenOrigen.Enabled = false;
            cboMotivo.Value = "-1";
            cboAlmacenDestino.Value = "-1";
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            cboMoneda.Value = "-1";
        }
        #region Comportamiento Controles
        private void cboAlmacenOrigen_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboAlmacenOrigen.Value == null) return;
            OperationResult objOperationResult = new OperationResult();
            var listaCompleta = _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_CurrentExecutionNodeId);
            var ListaFiltrada = listaCompleta.ToList();
            ListaFiltrada.RemoveAll(item => item.Id == cboAlmacenOrigen.Value.ToString());
            Utils.Windows.LoadUltraComboEditorList(cboAlmacenDestino, "Value1", "Id", ListaFiltrada, DropDownListAction.All);
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }
        

        private void cboAlmacenOrigen_ValueChanged(object sender, EventArgs e)
        {
            if (cboAlmacenOrigen.Value == null) return;
            OperationResult objOperationResult = new OperationResult();
            var listaCompleta = _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null,Globals.ClientSession.i_IdEstablecimiento.Value );
            //var listaCompleta = _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_CurrentExecutionNodeId);
            var ListaFiltrada = listaCompleta.ToList();
            ListaFiltrada.RemoveAll(item => item.Id == cboAlmacenOrigen.Value.ToString());
            Utils.Windows.LoadUltraComboEditorList(cboAlmacenDestino, "Value1", "Id", ListaFiltrada, DropDownListAction.All);
            cboAlmacenDestino.Value = "-1";
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Bandeja Transferencias";
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

        private void frmBandejaTransferencia_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_tarea != null && !_tarea.IsCompleted)
                cts.Cancel();
        }

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void chkBandejaAgrupable_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.HacerGrillaAgrupable(grdData, chkBandejaAgrupable.Checked);
        }
        #endregion
        #region Busqueda
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            if (cboMotivo.Value.ToString() != "-1") Filters.Add("i_IdTipoMotivo==" + cboMotivo.Value.ToString());
            if (cboAlmacenOrigen.Value.ToString() != "-1") Filters.Add("i_IdAlmacenOrigen==" + cboAlmacenOrigen.Value.ToString());
            if (cboAlmacenDestino.Value.ToString() != "-1") Filters.Add("i_IdAlmacenDestino==" + cboAlmacenDestino.Value.ToString());
            Filters.Add("i_IdEstablecimiento==" + Globals.ClientSession.i_IdEstablecimiento.Value.ToString ());
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


            var objData = GetData("v_MesCorrelativo ASC", _strFilterExpression);
            grdData.DataSource = objData;
            lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);

            if (objData != null)
            {
                if (objData.Count > 0)
                {
                    
                    btnEditar.Enabled = _btnEditar;
                    btnEliminar.Enabled = _btnEliminar;
                }
                else
                {
                    btnEditar.Enabled = false;
                    btnEliminar.Enabled = false;
                }
                lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
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
        private List<movimientoDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objMovimientoBL.ListarBusquedaMovimientos(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, dtpFechaFin.Value, (int)Common.Resource.TipoDeMovimiento.Transferencia,Globals.ClientSession.i_IdEstablecimiento.Value);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }
        #endregion
        #region CRUD
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow ||
              grdData.ActiveRow.Activation == Activation.Disabled) return;
            
            
            if (grdData.ActiveRow != null)
            {
                if (Utils.Windows.HaveFormChild(this, typeof(frmTransferencia), true)) return;
                new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
                pstrIdMovimiento = grdData.ActiveRow.Cells["v_IdMovimiento"].Value.ToString();
                frmTransferencia frm = new frmTransferencia("Edicion", pstrIdMovimiento);
                frm.FormClosed += delegate
                {
                    BindGrid();
                    MantenerSeleccion(pstrIdMovimiento);
                };
                (this.MdiParent as frmMaster).RegistrarForm(this, frm);
                
            }
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmTransferencia))) return;
            frmTransferencia frm = new frmTransferencia("Nuevo", "");
            frm.FormClosed += (_, ev) =>
            {
                BindGrid();
                MantenerSeleccion((_ as frmTransferencia)._pstrIdMovimiento_Nuevo);
            };
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
        }
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow ||
              grdData.ActiveRow.Activation == Activation.Disabled) return;

            OperationResult _objOperationResult = new OperationResult();
            if (UltraMessageBox.Show("¿Está seguro de Eliminar este movimiento de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                pstrIdMovimiento = grdData.ActiveRow.Cells["v_IdMovimiento"].Value.ToString();
                #region Elimina Notas de Ingresos y Salidas Relacionadas
                var MovIS = _objMovimientoBL.ObtenerCabeceraNotaISEmitidasTransferencia(ref _objOperationResult,pstrIdMovimiento);
                foreach (var Fila in MovIS)
                {
                    _objMovimientoBL.EliminarMovimiento(ref _objOperationResult, Fila, Globals.ClientSession.GetAsList());
                }
                #endregion

                _objMovimientoBL.EliminarMovimiento(ref _objOperationResult, pstrIdMovimiento, Globals.ClientSession.GetAsList());
                btnBuscar_Click(sender, e);
            }
        }
        #endregion
        #region Grilla
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
                btnEditar.Enabled = _btnEditar;
                btnEliminar.Enabled = _btnEliminar;
                btnEliminar.Enabled = _objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_MesCorrelativo"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Almacen) ? false : true;
            }
        }
        private void grdData_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                if (btnEditar.Enabled) btnEditar_Click(sender, e);
            }
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }
        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            if (grdData.ActiveRow != null)
            {

                btnEliminar.Enabled = _btnEliminar;
                btnEditar.Enabled = _btnEditar;
                btnEliminar.Enabled = _objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_MesCorrelativo"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Almacen) ? false : true;
            }
            else
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
            }
        }

        
        #endregion
        #region Clases/Validaciones
        private void MantenerSeleccion(string ValorSeleccionado)
        {
            //foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in grdData.Rows)
            //{
            //    if (row.Cells["v_IdMovimiento"].Text == ValorSeleccionado)
            //    {
            //        grdData.ActiveRow = row;
            //        grdData.ActiveRow.Selected = true;
            //        break;
            //    }
            //}
            if (string.IsNullOrEmpty(ValorSeleccionado)) return;
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            var fila = filas.FirstOrDefault(f => f.Cells["v_IdMovimiento"].Value.ToString().Contains(ValorSeleccionado));
            if (fila != null) fila.Activate();
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

        private void btnRegenerarT_Click(object sender, EventArgs e)
        {
            AlmacenBL _objAlmacenBL= new AlmacenBL ();
            OperationResult objOperationResult = new OperationResult ();
            var NombreAlmacen=_objAlmacenBL.ObtenerAlmacen(ref objOperationResult ,Globals.ClientSession.i_IdAlmacenPredeterminado.Value ).v_Nombre;
            if (!(System.Windows.Forms.Application.OpenForms["frmMaster"] as frmMaster).IsBussy())
            {
                if (UltraMessageBox.Show("¿Seguro de Actualizar las transferencias de  " + NombreAlmacen, "Mensaje de Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
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
            string Error="";
          //  _objMovimientoBL.RegenerarTransferencias(ref objOperationResult,ref Error,Globals.ClientSession.i_IdAlmacenPredeterminado.Value ,1,true);
            _objMovimientoBL.RegeraTransferenciaMetodo2(ref objOperationResult, Globals.ClientSession.i_Periodo.Value.ToString(), Globals.ClientSession.GetAsList());
            if (objOperationResult.Success == 1)
            {
                UltraMessageBox.Show("Proceso terminado correctamente!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Globals.ProgressbarStatus.b_Cancelado = true;
                UltraMessageBox.Show("Ocurrió un Error al regenerar Transferencias  en " + Error + " Adicional :\n\n"+objOperationResult.ErrorMessage+"\n\n"+ objOperationResult.AdditionalInformation, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

                
            }
        }

        private void btnActualizarGuiasRemision_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            _objMovimientoBL.RegenerarGuiasRemisionDetalles(ref objOperationResult);
            if (objOperationResult.Success == 1)
            {
                UltraMessageBox.Show("Proceso culminado exitosamente");
            }
            else
            {
                UltraMessageBox.Show("Ocurriò un error al realizado Proceso");
            }
        }

        private void btnRegenerarIDs_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            _objMovimientoBL.AsociarIds(ref objOperationResult);
            if (objOperationResult.Success == 1)
            {
                UltraMessageBox.Show("Proceso culminado exitosamente");
            }
            else
            {
                UltraMessageBox.Show("Ocurriò un error al realizado Proceso");
            }
        }

    }
}
