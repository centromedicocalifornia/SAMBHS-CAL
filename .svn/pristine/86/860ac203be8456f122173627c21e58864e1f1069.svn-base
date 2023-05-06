using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.Resource;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using SAMBHS.Venta.BL;
using SAMBHS.Almacen.BL;
using SAMBHS.Security.BL;
using SAMBHS.Contabilidad.BL;


namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmBandejaPedido : Form
    {
        private Task _tarea;
        private CancellationTokenSource cts = new CancellationTokenSource();
        public string _strFilterExpression, pstrIdPedido;
        PedidoBL objPedidoBL = new PedidoBL();
        MovimientoBL objMovimientoBL = new MovimientoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        PedidoBL _objPedidoBL = new PedidoBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        SecurityBL _obSecurityBL = new SecurityBL();
        CierreMensualBL _objCierreMensualBL = new CierreMensualBL();
        ClienteBL objClienteBL = new ClienteBL();
        pedidoDto _objPedido = new pedidoDto();
        bool _btnNuevo, _btnEliminar, _btnModificar, _btnGenerar, _btnEliminarCaducado;
        public string v_IdCliente = string.Empty;
        public frmBandejaPedido(string cadena)
        {
            InitializeComponent();
        }
        private void frmBandejaPedido_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            OperationResult objOperationResult = new OperationResult();
            #region ControlAcciones

            var _formActions = _obSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmBandejaPedido", Globals.ClientSession.i_RoleId);
            _btnNuevo = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaPedido_New", _formActions);
            _btnEliminar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaPedido_Delete", _formActions);
            _btnModificar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaPedido_Edit", _formActions);
            _btnGenerar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaPedido_Generate", _formActions);
            _btnEliminarCaducado = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaPedido_DeleteCaduc", _formActions);

            btnAgregar.Enabled = _btnNuevo;
            btnEliminar.Enabled = _btnEliminar;
            btnEditar.Enabled = _btnModificar;
            btnGenerar.Enabled = _btnGenerar;
           // btnEliminarCaducados.Enabled = _btnEliminarCaducado;
            #endregion
            #region CargarCombos
            // Utils.Windows.LoadDropDownList(cboCliente, "Value1", "Id", objMovimientoBL.BuscarProveedoresParaCombo(ref objOperationResult, txtCliente.Text, "C"), DropDownListAction.All);
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosPedidosParaComboGrid(ref objOperationResult);
            Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);

            Utils.Windows.LoadUltraComboEditorList(cboEstados, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 30, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", Globals.CacheCombosVentaDto.cboVendedor, DropDownListAction.Select); cboVendedor.Value = "-1";
            cboDocumento.Value = "-1";
            cboEstados.Value = "-1";
            #endregion
            btnEditar.Enabled = false;
            btnEliminar.Enabled = false;
            btnGenerar.Enabled = false;
          //  ValidarFechas();

            if (Globals.ClientSession.i_SystemUserId == 1)
            {
                // btnRecalcularSeparacion.Visible = true;
                //btnActualizarSeparacion.Visible = true;
                //  btnAcualizarDescuento.Visible = true;

            }
            else
            {
                //btnRecalcularSeparacion.Visible = false ;
                // btnActualizarSeparacion.Visible = false;
                //  btnAcualizarDescuento.Visible = false;
            }
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
            cboMoneda.Value = "-1";
            var banda = grdData.DisplayLayout.Bands[0];
            banda.Summaries["Total"].CustomSummaryCalculator = new TotalCustomSoles("d_PrecioVenta");
            ConfiguracionGrilla();
            RecalcularStock();
        }

        private void RecalcularStock()
        {
            if (Globals.ActualizadoStocks) return;
            var op = new OperationResult();
            pbRecalculandoStock.Visible = true;
            var objStockBl = new StockSeparacionBl();
            objStockBl.Terminado += delegate { pbRecalculandoStock.Visible = false; };
            objStockBl.IniciarProceso(ref op, Globals.ClientSession.i_Periodo.ToString(), Globals.ClientSession.i_IdAlmacenPredeterminado ?? -1);
        }

        private void ConfiguracionGrilla()
        {
            
            UltraGridColumn Glosa= grdData.DisplayLayout.Bands[0].Columns["v_Glosa"];
            
            if (Globals.ClientSession.v_RucEmpresa ==Constants.RucCMR) //
            {
                Glosa.Hidden = false;
               
            }
            else
            {
                Glosa.Hidden = true;
               
            }
        }

        #region Búsqueda
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<string> Filters = new List<string>();
            OperationResult objOperationResult = new OperationResult();
            using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                this.BindGrid();
        }

        private void BindGrid()
        {
            var objData = GetData("NroRegistro ASC", _strFilterExpression);
            grdData.DataSource = objData;
            if (objData != null)
            {
                if (objData.Count > 0)
                {

                    btnEditar.Enabled = _btnModificar;
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
                var opContable = fila.Cells ["v_UsuarioCreacion"].Value ==null ?false : Globals.UsuariosContables.Contains (fila.Cells["v_UsuarioCreacion"].Value.ToString());
                fila.Activation = opContable ? Activation.Disabled : Activation.ActivateOnly;
            });
        }
        private List<pedidoDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = objPedidoBL.ListarBusquedaPedidos(ref objOperationResult, pstrSortExpression, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Value.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59"), int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim(), txtCorrelativoDoc.Text.Trim(), txtMes.Text.Trim(), txtCorrelativoMes.Text.Trim(), v_IdCliente, int.Parse(cboEstados.Value.ToString()), int.Parse(cboMoneda.Value.ToString()), cboVendedor.Value.ToString());
            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return _objData;
        }


        private void MantenerSeleccion(string ValorSeleccionado)
        {

            if (string.IsNullOrEmpty(ValorSeleccionado)) return;
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            var fila = filas.FirstOrDefault(f => f.Cells["v_IdPedido"].Value.ToString().Contains(ValorSeleccionado));
            if (fila != null) fila.Activate();
        }
        #endregion

        #region ComportamientoControles

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

        private void txtMes_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
        }

        private void txtCorrelativoMes_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoMes, "{0:00000000}");
        }

        private void txtSerieDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc, e);
        }

        private void txtCorrelativoDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDoc, e);
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
            //if (strModo == "Nuevo")
            //{
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
            //txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString()));
            //txtNumeroDoc.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000");
            //ComprobarExistenciaCorrelativoDocumento();
            //}
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
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
                btnGenerar.Enabled = false;
            }
            else
            {

                btnEditar.Enabled = _btnModificar;
                if (grdData.ActiveRow.Cells["i_IdEstado"].Value.ToString() == "1")
                {
                    btnEliminar.Enabled = false;
                    btnGenerar.Enabled = false;
                }
                else
                {

                   
                    btnEliminar.Enabled = _btnEliminar;
                    btnGenerar.Enabled = _btnGenerar;
                    btnGenerar.Enabled = !_objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["NroRegistro"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.VentasFacturacion );
                    btnEliminar.Enabled = !_objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["NroRegistro"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.VentasFacturacion);


                    if (!string.IsNullOrEmpty(grdData.ActiveRow.Cells["NroFactura"].Value.ToString()))
                    {
                        btnEliminar.Enabled = false;
                        btnGenerar.Enabled = false;

                    }
                }

                if (grdData.ActiveRow.Cells["i_IdTipoDocumento"].Value.ToString() == "431")
                {

                    btnGenerar.Enabled = _btnGenerar;
                    btnGenerar.Enabled = !_objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["NroRegistro"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.VentasFacturacion);

                }
                else
                {
                    btnGenerar.Enabled = false;
                }

            }

        }
        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].Columns["i_IdEstado"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;


        }
        private void grdData_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;

            if (grdData.Rows.Count == 0) return;

            if (grdData.ActiveRow.Cells["i_IdEstado"].Value.ToString() == "1") //Facturado
            {
                btnEliminar.Enabled = false;
                btnGenerar.Enabled = false;
            }
            else
            {
               
                
                btnEliminar.Enabled = _btnEliminar;
                btnGenerar.Enabled = _btnGenerar;
                btnEliminar.Enabled = !_objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["NroRegistro"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.VentasFacturacion);
                btnGenerar.Enabled = !_objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["NroRegistro"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.VentasFacturacion);

                if (!string.IsNullOrEmpty(grdData.ActiveRow.Cells["NroFactura"].Value.ToString()))
                {
                    btnEliminar.Enabled = false;
                    btnGenerar.Enabled = false;

                }
            }
            if (grdData.ActiveRow.Cells["i_IdTipoDocumento"].Value.ToString() == "431")
            {

                btnGenerar.Enabled = _btnGenerar;
                btnGenerar.Enabled = !_objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["NroRegistro"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.VentasFacturacion);

            }
            else
            {
                btnGenerar.Enabled = false;
            }

        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;

            if (grdData.ActiveRow.Cells["i_IdEstado"].Value.ToString() == "1") // Factura 
            {
                btnEliminar.Enabled = false;
                btnGenerar.Enabled = false;
            }
            else
            {


               
                
                btnEliminar.Enabled = _btnEliminar;
                btnGenerar.Enabled = _btnGenerar;
                btnGenerar.Enabled =
                    !_objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(),
                        grdData.ActiveRow.Cells["NroRegistro"].Value.ToString().Trim().Substring(0, 2),
                        (int)ModulosSistema.VentasFacturacion);
                btnEliminar.Enabled =
                    !_objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(),
                        grdData.ActiveRow.Cells["NroRegistro"].Value.ToString().Trim().Substring(0, 2),
                        (int)ModulosSistema.VentasFacturacion);


                if (!string.IsNullOrEmpty(grdData.ActiveRow.Cells["NroFactura"].Value.ToString()))
                {
                    btnEliminar.Enabled = false;
                    btnGenerar.Enabled = false;

                }


            }
            if (grdData.ActiveRow.Cells["i_IdTipoDocumento"].Value.ToString() == "431")
            {
                btnGenerar.Enabled = _btnGenerar;
                btnGenerar.Enabled =
                    !_objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(),
                        grdData.ActiveRow.Cells["NroRegistro"].Value.ToString().Trim().Substring(0, 2),
                        (int)ModulosSistema.VentasFacturacion);
            }
            else
            {
                btnGenerar.Enabled = false;
            }

            btnEditar.Enabled = _btnModificar;


        }

        #endregion

        #region CRUD
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmPedido))) return;
            new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
            frmPedido frm = new frmPedido("Nuevo", "");
            frm.FormClosed += (_, ev) =>
            {
                MantenerSeleccion((_ as frmPedido)._pstrIdMovimiento_Nuevo);
                BindGrid();
            };
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
        }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow || grdData.ActiveRow.Activation == Activation.Disabled) return;
            if (Utils.Windows.HaveFormChild(this, typeof(frmPedido), true)) return;
            pstrIdPedido = grdData.ActiveRow.Cells["v_IdPedido"].Value.ToString();
            string NroFactura = grdData.ActiveRow.Cells["NroFactura"].Value == null ? "" : grdData.ActiveRow.Cells["NroFactura"].Value.ToString();
            frmPedido frm = new frmPedido("Edicion", pstrIdPedido, NroFactura);
            frm.FormClosed += (_, ev) =>
            {
                btnBuscar_Click(sender, e);
                MantenerSeleccion(pstrIdPedido);
                btnEditar.Enabled = _btnModificar;
            };
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow ||
               grdData.ActiveRow.Activation == Activation.Disabled) return;

            OperationResult _objOperationResult = new OperationResult();

            if (!_objPedidoBL.VerificarSiTieneFacturas(int.Parse(grdData.ActiveRow.Cells["i_IdTipoDocumento"].Value.ToString()), grdData.ActiveRow.Cells["v_SerieDocumento"].Value.ToString(), grdData.ActiveRow.Cells["v_CorrelativoDocumento"].Value.ToString()))
            {

                if (UltraMessageBox.Show("¿Está seguro de Eliminar este pedido de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    pstrIdPedido = grdData.ActiveRow.Cells["v_IdPedido"].Value.ToString();


                    objPedidoBL.EliminarPedido(ref _objOperationResult, pstrIdPedido, Globals.ClientSession.GetAsList());
                    btnBuscar_Click(sender, e);
                }
            }
            else
            {
                UltraMessageBox.Show("No se puede eliminar,Pedido ya se encuentra facturado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow ||
               grdData.ActiveRow.Activation == Activation.Disabled) return;
            if (Utils.Windows.HaveFormChild(this, typeof(frmPedido), true)) return;
            var pstrIdCotizacion = grdData.ActiveRow.Cells["v_IdPedido"].Value.ToString();
            frmPedido frm = new frmPedido("Cotizacion", pstrIdCotizacion);
            frm.FormClosed += (_, ev) =>
            {
                BindGrid();
                MantenerSeleccion(pstrIdCotizacion);
            };
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
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



        #endregion

        private void grdData_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;

            if (btnEditar.Enabled) btnEditar_Click(sender, e);

        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                grdData.DisplayLayout.Override.FilterUIType = grdData.DisplayLayout.Override.FilterUIType == FilterUIType.Default ? FilterUIType.FilterRow : FilterUIType.Default;
            }
        }



        private void txtCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarCliente")
            {
                using (var frm = new Mantenimientos.frmBuscarCliente("VV", txtCliente.Text.Trim()))
                {
                    frm.ShowDialog();
                    if (frm._IdCliente != null)
                    {
                        txtCliente.Text = string.Format("{0} - {1}", frm._NroDocumento, frm._RazonSocial);
                        v_IdCliente = frm._IdCliente;
                        txtCliente.ButtonsRight["btnEliminar"].Enabled = true;
                    }
                }
            }
            else
            {
                txtCliente.Clear();
                v_IdCliente = string.Empty;
                txtCliente.ButtonsRight["btnEliminar"].Enabled = false;
            }
        }

        private void chkBandejaAgrupable_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.HacerGrillaAgrupable(grdData, chkBandejaAgrupable.Checked);
        }

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Bandeja Pedidos";

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
                    using (var gExplorer = new UltraGridExcelExporter())
                        gExplorer.Export(grdData, filename);
                }, cts.Token)
                .ContinueWith(t => ActualizarLabel("Bandeja Exportada a Excel."),
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void ActualizarLabel(string Texto)
        {
            lblDocumentoExportado.Text = Texto;
            btnExportarBandeja.Enabled = false;
            btnExportarBandeja.Appearance.Image = Resource.accept;
        }

        private void frmBandejaPedido_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_tarea != null && !_tarea.IsCompleted)
                cts.Cancel();
        }

        private void btnClonarPedido_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            var doc = grdData.ActiveRow.Cells["TipoDocumento"].Value.ToString();

            if (grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow || grdData.ActiveRow.Activation == Activation.Disabled) return;

            var cliente = grdData.ActiveRow.Cells["NombreCliente"].Value.ToString();
            var monto = grdData.ActiveRow.Cells["d_PrecioVenta"].Value.ToString();

            var resp = MessageBox.Show(@"¿Seguro de clonar el " + doc + @" del cliente " + cliente + @" por S/" + monto + @"?", @"Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp == DialogResult.No) return;
            pstrIdPedido = grdData.ActiveRow.Cells["v_IdPedido"].Value.ToString();

            var objOperationResult = new OperationResult();

            var id = _objPedidoBL.ClonarPedido(ref objOperationResult, pstrIdPedido, Globals.ClientSession.GetAsList());
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dtpFechaFin.Value = DateTime.Now.Date;
            btnBuscar_Click(sender, e);
            MantenerSeleccion(id);
        }

        private void btnEliminarCaducados_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            var nombreAlmacen = new AlmacenBL().ObtenerAlmacen(ref objOperationResult, Globals.ClientSession.i_IdAlmacenPredeterminado ?? 1).v_Nombre;
            var fecha = DateTime.Today.AddDays(-2);
            if (!((frmMaster)Application.OpenForms["frmMaster"]).IsBussy())
            {
                if (UltraMessageBox.Show("¿Está seguro de Eliminar TODOS los pedidos Pendientes de " + nombreAlmacen + " Hasta " + fecha.ToShortDateString() + " ?", "Mensaje de Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    Globals.ProgressbarStatus.i_Progress = 1;
                    Globals.ProgressbarStatus.i_TotalProgress = 1;
                    Globals.ProgressbarStatus.b_Cancelado = false;
                    bwkProcesoBL.RunWorkerAsync();
                    ((frmMaster)Application.OpenForms["frmMaster"]).ComenzarBackGroundProcess();
                }
            }

            // btnBuscar_Click(sender, e);

        }
        private void IniciarProceso(object sender, DoWorkEventArgs e)
        {


            DateTime Fecha = DateTime.Today.AddDays(-2);
            OperationResult _objOperationResult = new OperationResult();

            objPedidoBL.EliminarPedidosCaducados(ref _objOperationResult, Globals.ClientSession.GetAsList(), Fecha, Globals.ClientSession.i_IdAlmacenPredeterminado.Value);

            if (_objOperationResult.Success == 1)
            {
                UltraMessageBox.Show("Pedidos Eliminados", "Sistema");
                Globals.ProgressbarStatus.b_Cancelado = false;

            }
            else
            {
                Globals.ProgressbarStatus.b_Cancelado = true;
                UltraMessageBox.Show("Ocurrió un Error al Eliminar Pedidos", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }
        private void btnActualizarSeparacion_Click(object sender, EventArgs e)
        {
            if (UltraMessageBox.Show("¿Está seguro de Actualizar los nulos de la Separacion Producto", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {


                OperationResult objOperationResult = new OperationResult();
                objPedidoBL.ActualizarNulosSeparacionProducto(ref objOperationResult);
                if (objOperationResult.Success == 1)
                {
                    UltraMessageBox.Show(" Nulos Separación Actualizada", "Sistema");
                    btnRecalcularSeparacion.Enabled = true;

                }
                else
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }

        private void btnRecalcularSeparacion_Click(object sender, EventArgs e)
        {


            if (UltraMessageBox.Show("¿Está seguro de Recalcular la Separacion del Producto Almacén", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                OperationResult objOperationResult = new OperationResult();
                _objPedidoBL.RecalcularSeparacionProductoAlmacen(ref objOperationResult, -1, Globals.ClientSession.GetAsList(), Globals.ClientSession.i_Periodo.ToString());
                if (objOperationResult.Success == 1)
                {
                    UltraMessageBox.Show("Separación Actualizada", "Sistema");

                }

                else
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }

        private void btnAcualizarDescuento_Click(object sender, EventArgs e)
        {



            if (UltraMessageBox.Show("¿Está seguro de Actualizar los Descuentos de los Pedidos?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                OperationResult objOperationResult = new OperationResult();
                _objPedidoBL.ActualizarDescuentos(ref objOperationResult);
                if (objOperationResult.Success == 1)
                {
                    UltraMessageBox.Show("Descuentos Actualizados", "Sistema");
                }
                else
                {
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }


        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells["i_IdEstado"].Value.ToString() == "3")
            {
                e.Row.Appearance.BackColor = Color.Salmon;
                e.Row.Appearance.BackColor2 = Color.Salmon;
                e.Row.Appearance.ForeColor = Color.Black;
            }
        }

        class TotalCustomSoles : ICustomSummaryCalculator
        {
            private decimal _total;
            private readonly string _campo;
            internal TotalCustomSoles(string field)
            {
                _campo = field;
            }

            public void AggregateCustomSummary(SummarySettings summarySettings, UltraGridRow row)
            {
                var estado = row.Cells["i_IdEstado"].Value.ToString();
                var tc = decimal.Parse(row.GetCellValue("d_TipoCambio").ToString());
                var idMoneda = row.Cells["Moneda"].Value.ToString() == "D" ? 2 : 1;

                if (estado == "3") return;
                var monto = DevuelveMontoPorCobrar(idMoneda, 1, decimal.Parse(row.Cells[_campo].Value.ToString()), tc);
                //if ((int)row.Cells["i_IdTipoDocumento"].Value == (int)TiposDocumentos.NotaCredito)
                //    _total -= monto;
                //else
                _total += monto;
            }

            public void BeginCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {
                _total = 0M;
            }

            public object EndCustomSummary(SummarySettings summarySettings, RowsCollection rows)
            {
                return _total;
            }

            private decimal DevuelveMontoPorCobrar(int idMonedaVenta, int idMonedaCobranza, decimal saldoVenta, decimal tipoCambioVenta)
            {
                try
                {
                    if (idMonedaVenta == idMonedaCobranza) return saldoVenta;
                    switch (idMonedaCobranza)
                    {
                        case 1: return Utils.Windows.DevuelveValorRedondeado(saldoVenta * tipoCambioVenta, 2);
                        case 2: return Utils.Windows.DevuelveValorRedondeado(saldoVenta / tipoCambioVenta, 2);
                    }

                    return saldoVenta;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return 0;
                }
            }
        }
    }
}
