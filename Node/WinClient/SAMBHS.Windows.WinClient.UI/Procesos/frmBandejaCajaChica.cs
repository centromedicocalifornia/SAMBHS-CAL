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
    public partial class frmBandejaCajaChica : Form
    {
        private Task _tarea;
        private CancellationTokenSource cts = new CancellationTokenSource();
        public string _strFilterExpression, pstrIdCajaChica;
        PedidoBL objPedidoBL = new PedidoBL();
        MovimientoBL objMovimientoBL = new MovimientoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        CajaChicaBL _objCajaChicaBL = new CajaChicaBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        SecurityBL _obSecurityBL = new SecurityBL();
        CierreMensualBL _objCierreMensualBL = new CierreMensualBL();
        ClienteBL objClienteBL = new ClienteBL();
        pedidoDto _objPedido = new pedidoDto();
        bool _btnNuevo = true, _btnEliminar = true, _btnModificar = true, _btnEliminarCaducado = true;
        public string v_IdCliente = string.Empty;
        public frmBandejaCajaChica(string cadena)
        {
            InitializeComponent();
        }
        private void frmBandejaCajaChica_Load(object sender, EventArgs e)
        {
            #region ControlAcciones

            // var _formActions = _obSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmBandejaPedido", Globals.ClientSession.i_RoleId);
            //_btnNuevo = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaPedido_New", _formActions);
            //_btnEliminar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaPedido_Delete", _formActions);
            //_btnModificar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaPedido_Edit", _formActions);
            //_btnGenerar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaPedido_Generate", _formActions);
            //_btnEliminarCaducado = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaPedido_DeleteCaduc", _formActions);
            this.BackColor = new GlobalFormColors().FormColor;
            //_btnEliminar =Globals.ClientSession.i_SystemUserId == 1 ?true :false ;
            btnAgregar.Enabled = _btnNuevo;
            btnEliminar.Enabled = _btnEliminar;
            btnEditar.Enabled = _btnModificar;
            
            #endregion
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

            if (Globals.ClientSession.i_SystemUserId != 1)
            {
                objData = objData.FindAll(p => p.v_IdVendedor == Globals.ClientSession.v_IdVendedor);
            }


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
                var opContable = Globals.UsuariosContables.Contains(fila.Cells["v_UsuarioCreacion"].Value.ToString());
                fila.Activation = opContable ? Activation.Disabled : Activation.ActivateOnly;
            });
        }
        private List<cajachicaDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();

            var _objData = _objCajaChicaBL.ListarBusquedaCajaChica(ref objOperationResult, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Text + " 23:59"), Globals.ClientSession.i_SystemUserId);//objPedidoBL.ListarBusquedaPedidos(ref objOperationResult, pstrSortExpression, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Value.Day.ToString() + "/" + dtpFechaFin.Value.Month.ToString() + "/" + dtpFechaFin.Value.Year.ToString() + " 23:59"), int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim(), txtCorrelativoDoc.Text.Trim(), txtMes.Text.Trim(), txtCorrelativoMes.Text.Trim(), v_IdCliente, int.Parse(cboEstados.Value.ToString()), int.Parse(cboMoneda.Value.ToString()), cboVendedor.Value.ToString());
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
            var fila = filas.FirstOrDefault(f => f.Cells["v_IdCajaChica"].Value.ToString().Contains(ValorSeleccionado));
            if (fila != null) fila.Activate();
        }
        #endregion

        #region ComportamientoControles
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

            }
            else
            {

                btnEditar.Enabled = _btnModificar;
                btnEliminar.Enabled = _btnEliminar;
                btnEliminar.Enabled = !_objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["NroRegistro"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Tesoreria);

                

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

            //if (grdData.ActiveRow.Cells["i_IdEstado"].Value.ToString() == "1") //Facturado
            //{
            //    btnEliminar.Enabled = false;

            //}
            //else
            //{
                btnEliminar.Enabled = _btnEliminar;
                btnEliminar.Enabled = !_objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(), grdData.ActiveRow.Cells["NroRegistro"].Value.ToString().Trim().Substring(0, 2), (int)ModulosSistema.Tesoreria);
                btnEditar.Enabled = _btnModificar;
            //}


        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow) return;

            //if (grdData.ActiveRow.Cells["i_IdEstado"].Value.ToString() == "1") // Factura 
            //{
            //    btnEliminar.Enabled = false;

            //}
            //else
            //{
                btnEliminar.Enabled = _btnEliminar;
                btnEliminar.Enabled =
                    !_objCierreMensualBL.VerificarMesCerrado(grdData.ActiveRow.Cells["v_Periodo"].Value.ToString(),
                        grdData.ActiveRow.Cells["NroRegistro"].Value.ToString().Trim().Substring(0, 2),
                        (int)ModulosSistema.Tesoreria);
            //}
            btnEditar.Enabled = _btnModificar;


        }

        #endregion

        #region CRUD
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmCajaChica))) return;
            new LoadingClass.PleaseWait(this.Location, "Por favor espere...");
            frmCajaChica frm = new frmCajaChica("Nuevo", "");
            frm.FormClosed += (_, ev) =>
            {
                MantenerSeleccion((_ as frmCajaChica)._pstrIdCajaChica_Nuevo);
                BindGrid();
            };
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
        }
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow || grdData.ActiveRow.Activation == Activation.Disabled) return;
            if (Utils.Windows.HaveFormChild(this, typeof(frmCajaChica), true)) return;
            pstrIdCajaChica = grdData.ActiveRow.Cells["v_IdCajaChica"].Value.ToString();
            frmCajaChica frm = new frmCajaChica("Edicion", pstrIdCajaChica);
            frm.FormClosed += (_, ev) =>
            {
                btnBuscar_Click(sender, e);
                MantenerSeleccion(pstrIdCajaChica);
                btnEditar.Enabled = _btnModificar;
            };
            (this.MdiParent as frmMaster).RegistrarForm(this, frm);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow ||
               grdData.ActiveRow.Activation == Activation.Disabled) return;

            OperationResult _objOperationResult = new OperationResult();
            if (UltraMessageBox.Show("¿Está seguro de Eliminar esta caja chica de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                pstrIdCajaChica = grdData.ActiveRow.Cells["v_IdCajaChica"].Value.ToString();
                _objCajaChicaBL.EliminarCajaChica(ref _objOperationResult, pstrIdCajaChica, Globals.ClientSession.GetAsList());
                if (_objOperationResult.Success == 1)
                {
                    UltraMessageBox.Show("El registro se ha eliminado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnBuscar_Click(sender, e);
                }
                else
                {
                    UltraMessageBox.Show("Ocurrió un error al eliminar", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
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

            const string dummyFileName = "Bandeja Caja Chica";

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

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells["i_IdEstado"].Value.ToString() == "1")
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



        private void frmBandejaCajaChica_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_tarea != null && !_tarea.IsCompleted)
                cts.Cancel();
        }
    }
}
