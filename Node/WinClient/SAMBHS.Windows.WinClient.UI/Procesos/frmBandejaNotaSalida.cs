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
using SAMBHS.Security.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Contabilidad.BL;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using System.Threading;



namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaNotaSalida : Form
    {

        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        SecurityBL _obSecurityBL = new SecurityBL();
        CierreMensualBL _objCierreMensualBL = new CierreMensualBL();
        ClienteBL _objClienteBL = new ClienteBL();
        private Task _tarea;
        private CancellationTokenSource cts = new CancellationTokenSource();
        string _strFilterExpression;

        string pstrIdMovimiento, v_IdCliente = string.Empty;
        bool _btnNuevo, _btnEliminar, _btnEditar;

        public frmBandejaNotaSalida(string Parametro)
        {
            InitializeComponent();
        }

        private void frmBandejaNotaSalida_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            #region ControlAcciones
            var _formActions = _obSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmBandejaNotaSalida", Globals.ClientSession.i_RoleId);

            _btnNuevo = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaNotaSalida_Add", _formActions);
            _btnEliminar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaNotaSalida_Delete", _formActions);
            _btnEditar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaNotaSalida_Edit", _formActions);

            btnNuevo.Enabled = _btnNuevo;
            btnEliminar.Enabled = _btnEliminar;
            btnEditar.Enabled = _btnEditar;
            #endregion
            #region Cargar Combos
            Utils.Windows.LoadUltraComboEditorList(cboMotivo, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 20, null), DropDownListAction.All);//20 numero del id Del motivo
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.All);
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            cboMotivo.Value= "-1";
          //  cboAlmacen.Enabled = false;
            #endregion
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            ValidarFechas();
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            cboMoneda.Value = "-1";
        }

        #region Busqueda
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            if (cboMotivo.Value.ToString() != "-1") Filters.Add("i_IdTipoMotivo ==" + cboMotivo.Value.ToString());
            if (cboAlmacen.Value.ToString() != "-1") Filters.Add("i_IdAlmacenOrigen ==" + cboAlmacen.Value.ToString());
            if (v_IdCliente != string.Empty) Filters.Add(string.Format("v_IdCliente==\"{0}\"", v_IdCliente));
            //Filters.Add("i_IdEstablecimiento=="+Globals.ClientSession.i_IdEstablecimiento.Value.ToString ());
            if (cboMoneda.Value.ToString() != "-1") Filters.Add(string.Format("Moneda==\"{0}\"", cboMoneda.Value.ToString().Equals("1") ? "S" : "D"));

            _strFilterExpression = null;
            if (Filters.Count > 0)
            {
                foreach (string item in Filters)
                {
                    _strFilterExpression = _strFilterExpression + item + " && ";
                }
                _strFilterExpression = _strFilterExpression.Substring(0, _strFilterExpression.Length - 4);
            }
            if (tbCBandejaNotasSalida.SelectedTab.Name == "tbVistaGeneral")
            {
                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                    this.BindGrid();

                if (grdData.Rows.Count() == 0)
                {
                    btnEliminar.Enabled = false;
                    btnEditar.Enabled = false;
                }
            }
            else
            {
                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                    this.BindGridDetallado();

                if (grdDataD.Rows.Count() == 0)
                {
                    btnEliminar.Enabled = false;
                    btnEditar.Enabled = false;
                }
            }
           
        }

        private void BindGrid()
        {
            var objData = GetData("v_MesCorrelativo ASC", _strFilterExpression);
            grdData.DataSource = objData;
            grdData.Focus();
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
        private void BindGridDetallado()
        {
            var objData = GetDataDetallado("v_MesCorrelativo ASC", _strFilterExpression);
            grdDataD.DataSource = objData;
            grdDataD.Focus();
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
            var _objData = _objMovimientoBL.ListarBusquedaMovimientos(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, dtpFechaFin.Value, (int)Common.Resource.TipoDeMovimiento.NotadeSalida,Globals.ClientSession.i_IdEstablecimiento.Value);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }

        private List<movimientoDto> GetDataDetallado(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = _objMovimientoBL.ListarBusquedaMovimientosDetallado(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, dtpFechaFin.Value, (int)Common.Resource.TipoDeMovimiento.NotadeSalida, Globals.ClientSession.i_IdEstablecimiento.Value);

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData;
        }


        #endregion
        #region Grilla

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
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
                btnEliminar.Enabled = _objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_MesCorrelativo"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Almacen) ? false : true; //NO DEPENDE DE ACCIONES DE CONTROL
                if (btnEliminar.Enabled == true )
                {
                  //  btnEliminar.Enabled = grdData.Rows[grdData.ActiveRow.Index].Cells["RegistroOrigen"].Value == null ? _btnEliminar : false;
                    btnEliminar.Enabled = grdData.ActiveRow.Cells["RegistroOrigen"].Value == null ? _btnEliminar : false;
                }

            }
        }
        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;
            if (grdData.ActiveRow != null)
            {
                btnEliminar.Enabled = _objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["v_MesCorrelativo"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Almacen) ? false : true;
                if (btnEliminar.Enabled == true)
                {
                   // btnEliminar.Enabled = grdData.Rows[grdData.ActiveRow.Index].Cells["RegistroOrigen"].Value == null ? _btnEliminar : false;
                    btnEliminar.Enabled = grdData.ActiveRow.Cells["RegistroOrigen"].Value == null ? _btnEliminar : false;

                }
                btnEditar.Enabled = _btnEditar;

            }
            else
            {
                btnEliminar.Enabled = false;
                btnEditar.Enabled = false;
            }
        }
        #endregion
        #region CRUD
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (tbCBandejaNotasSalida.SelectedTab.Name == "tbVistaGeneral")
            {
                if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow ||
                    grdData.ActiveRow.Activation == Activation.Disabled) return;

                if (grdData.ActiveRow.Index < 0) return;
                if (grdData.ActiveRow != null)
                {
                    if (Utils.Windows.HaveFormChild(this, typeof(frmNotaSalida), true)) return;
                    new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
                    if (grdData.ActiveRow.Index < 0) return;
                    pstrIdMovimiento = grdData.ActiveRow.Cells["v_IdMovimiento"].Value.ToString();
                    frmNotaSalida frm = new frmNotaSalida("Edicion", pstrIdMovimiento);
                    frm.FormClosed += delegate
                    {
                        BindGrid();
                        MantenerSeleccion(pstrIdMovimiento);
                    };
                    (this.MdiParent as frmMaster).RegistrarForm(this, frm);
                }
            }
            else
            {
                if (grdDataD.ActiveRow == null || grdDataD.ActiveRow.IsFilterRow || grdDataD.ActiveRow.IsGroupByRow ||
                    grdDataD.ActiveRow.Activation == Activation.Disabled) return;

                if (grdDataD.ActiveRow.Index < 0) return;
                if (grdDataD.ActiveRow != null)
                {
                    if (Utils.Windows.HaveFormChild(this, typeof(frmNotaSalida), true)) return;
                    new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
                    if (grdDataD.ActiveRow.Index < 0) return;
                    pstrIdMovimiento = grdDataD.ActiveRow.Cells["v_IdMovimiento"].Value.ToString();
                    frmNotaSalida frm = new frmNotaSalida("Edicion", pstrIdMovimiento);
                    frm.FormClosed += delegate
                    {
                        BindGridDetallado();
                        MantenerSeleccion(pstrIdMovimiento);
                    };
                    (this.MdiParent as frmMaster).RegistrarForm(this, frm);
                }
            }    
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow ||
             grdData.ActiveRow.Activation == Activation.Disabled) return;
            if (grdData.ActiveRow.Index < 0) return;
            OperationResult _objOperationResult = new OperationResult();
            if (UltraMessageBox.Show("¿Está seguro de Eliminar este movimiento de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                pstrIdMovimiento = grdData.ActiveRow.Cells["v_IdMovimiento"].Value.ToString();
                _objMovimientoBL.EliminarMovimiento(ref _objOperationResult, pstrIdMovimiento, Globals.ClientSession.GetAsList());
                btnBuscar_Click(sender, e);
            }
        }
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmNotaSalida))) return;
            frmNotaSalida frm = new frmNotaSalida("Nuevo", "");
            frm.FormClosed += (_, ev) =>
            {
                BindGrid();
                MantenerSeleccion((_ as frmNotaSalida)._pstrIdMovimiento_Nuevo);
            };
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
        }

        #endregion
        #region Clases/Validaciones
        private void LimpiaDetalle()
        {
            txtCliente.Text = string.Empty; 
        }
        private void MantenerSeleccion(string ValorSeleccionado)
        {
           
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
        private void ActualizarLabel(string Texto)
        {
            lblDocumentoExportado.Text = Texto;
            //btnExportarBandeja.Enabled = false;
            btnExportarBandeja.Appearance.Image = Resource.accept;
        }
        #endregion
        #region ComportamientoControles
        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            if (tbCBandejaNotasSalida.SelectedTab.Name == "tbVistaGeneral")
            {
                Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
            }
            else
            {
                Utils.Windows.MostrarOcultarFiltrosGrilla(grdDataD);      
            }
        }

        private void chkBandejaAgrupable_CheckedChanged(object sender, EventArgs e)
        {
            if (tbCBandejaNotasSalida.SelectedTab.Name == "tbVistaGeneral")
            {
                Utils.Windows.HacerGrillaAgrupable(grdData, chkBandejaAgrupable.Checked);
            }
            else
            {
                Utils.Windows.HacerGrillaAgrupable(grdDataD, chkBandejaAgrupable.Checked);
            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                if (grdData.DisplayLayout.Override.FilterUIType == FilterUIType.Default)
                {
                    grdData.DisplayLayout.Override.FilterUIType = FilterUIType.FilterRow;
                }
                else
                {
                    grdData.DisplayLayout.Override.FilterUIType = FilterUIType.Default;
                }
            }
        }
        private void txtCliente_TextChanged(object sender, EventArgs e)
        {
            if (txtCliente.Text   == "")
            {
                OperationResult objOperationResult = new OperationResult();
                // Utils.Windows.LoadDropDownList(cboCliente, "Value1", "Id", objMovimientoBL.BuscarProveedoresParaCombo(ref objOperationResult, txtCliente.Text, "C"), DropDownListAction.All);
                v_IdCliente = string.Empty;
                txtRazonSocial.Clear();

            }
        }

        private void txtCliente_KeyDown(object sender, KeyEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (e.KeyCode == Keys.Enter)
            {
                if (txtCliente.Text.Trim() != "" & txtCliente.TextLength <= 7)
                {

                    Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtCliente.Text.Trim());
                    frm.ShowDialog();

                    if (frm._IdCliente != null)
                    {

                        v_IdCliente = frm._IdCliente;
                        txtCliente.Text = frm._NroDocumento;
                        txtRazonSocial.Text = frm._RazonSocial;

                    }
                    else
                    {
                        txtRazonSocial.Clear();

                    }
                }

                else
                {

                    if (txtCliente.TextLength == 8 | txtCliente.TextLength == 11)
                    {
                        string[] DatosCliente = new string[3];
                        DatosCliente = _objMovimientoBL.DevolverClientePorNroDocumento(ref objOperationResult, txtCliente.Text.Trim());
                        if (DatosCliente != null)
                        {
                            v_IdCliente = DatosCliente[0];
                            txtRazonSocial.Text = DatosCliente[2];

                        }
                    }
                    else
                    {

                        Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtCliente.Text.Trim());
                        frm.ShowDialog();

                        if (frm._IdCliente != null)
                        {

                            v_IdCliente = frm._IdCliente;
                            txtCliente.Text = frm._NroDocumento;
                            txtRazonSocial.Text = frm._RazonSocial;

                        }
                        else
                        {
                            txtRazonSocial.Clear();
                            v_IdCliente = string.Empty;

                        }

                    }
                }


            }


        }
        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void cboCliente_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void txtCliente_KeyPress(object sender, KeyPressEventArgs e)
        {

            Utils.Windows.NumeroEnteroUltraTextBox(txtCliente, e);
        }
        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            
        }

        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarCliente")
            {
                OperationResult objOperationResult = new OperationResult();
                Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtCliente.Text.Trim());
                frm.ShowDialog();

                if (frm._IdCliente != null)
                {

                    // txtArtIni.Text = frm._CodigoInternoProducto.Trim();
                    txtCliente.Text = frm._NroDocumento;
                    txtRazonSocial.Text = frm._RazonSocial;
                    v_IdCliente = frm._IdCliente;
                }
                else
                {
                    //txtArtIni.Text = string.Empty;
                }
            }
        }

        private void txtCliente_Validated(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (txtCliente.Text.Trim() != string.Empty)
            {
                var Cliente = _objClienteBL.ObtenerClienteCodigoBandejas(ref objOperationResult, txtCliente.Text.Trim(), "C");
                if (Cliente != null)
                {
                    txtRazonSocial.Text = (Cliente.v_ApePaterno + " " + Cliente.v_ApeMaterno.Trim() + " " + Cliente.v_PrimerNombre.Trim() + " " + Cliente.v_SegundoNombre.Trim() + " " + Cliente.v_RazonSocial.Trim()).Trim();
                    v_IdCliente = Cliente.v_IdCliente;
                }
                else
                {
                    txtRazonSocial.Clear();
                    v_IdCliente = "-1";
                }

            }
            else
            {

                txtRazonSocial.Clear();
                v_IdCliente = string.Empty;
            }
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (tbCBandejaNotasSalida.SelectedTab.Name == "tbVistaGeneral")
            {
                if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

                const string dummyFileName = "Bandeja Notas de Salida";
                UltraGridExcelExporter ultraGridExcelExporter1 = new UltraGridExcelExporter();
                SaveFileDialog sf = new SaveFileDialog
                {
                    DefaultExt = "xlsx",
                    Filter = @"xlsx files (*.xlsx)|*.xlsx",
                    FileName = dummyFileName
                };

                if (sf.ShowDialog() != DialogResult.OK) return;
                btnExportarBandeja.Appearance.Image = Resource.loadingfinal1;
                _tarea = Task.Factory.StartNew(() => { ultraGridExcelExporter1.Export(grdData, sf.FileName); },
                        cts.Token)
                    .ContinueWith(t => ActualizarLabel("Bandeja Exportada a Excel."),
                        TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                if (!grdDataD.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

                const string dummyFileName = "Bandeja Notas de Salida Vista Detallada";
                UltraGridExcelExporter ultraGridExcelExporter1 = new UltraGridExcelExporter();
                SaveFileDialog sf = new SaveFileDialog
                {
                    DefaultExt = "xlsx",
                    Filter = @"xlsx files (*.xlsx)|*.xlsx",
                    FileName = dummyFileName
                };

                if (sf.ShowDialog() != DialogResult.OK) return;
                btnExportarBandeja.Appearance.Image = Resource.loadingfinal1;
                _tarea = Task.Factory.StartNew(() => { ultraGridExcelExporter1.Export(grdDataD, sf.FileName); },
                        cts.Token)
                    .ContinueWith(t => ActualizarLabel("Bandeja Exportada a Excel."),
                        TaskScheduler.FromCurrentSynchronizationContext());
            }    
        }

        private void frmBandejaNotaSalida_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_tarea != null && !_tarea.IsCompleted)
                cts.Cancel();
        }
       
       
        #endregion

        private void grdDataD_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (grdDataD.ActiveRow == null || grdDataD.ActiveRow.IsFilterRow || grdDataD.ActiveRow.IsGroupByRow) return;
            if (grdDataD.ActiveRow != null)
            {
                if (btnEditar.Enabled) btnEditar_Click(sender, e);
            }
        }



    }
}
