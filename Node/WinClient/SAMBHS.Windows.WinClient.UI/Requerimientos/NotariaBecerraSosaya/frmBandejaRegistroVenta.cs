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
using SAMBHS.Compra.BL;
using SAMBHS.Almacen.BL;
using Infragistics.Win.UltraWinGrid;
using System.Reflection;
using System.Threading;
using Infragistics.Win.UltraWinGrid.ExcelExport;
using LoadingClass;
using SAMBHS.Letras.BL;
using SAMBHS.Requerimientos.NBS;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using SAMBHS.Windows.WinClient.UI.Procesos;

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class frmBandejaRegistroVenta : Form
    {
        private Task _tarea;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private readonly Security.BL.SecurityBL _objSecurityBl = new Security.BL.SecurityBL();
        VentaBL _objVentaBL = new VentaBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        VentaBL _objVentasBL = new VentaBL();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        List<GridKeyValueDTO> _ListadoComboDocumentosGuiaRemision = new List<GridKeyValueDTO>();
        string _strFilterExpression, pstrIdVenta;
        public string v_IdCliente = string.Empty;
        public string Modo, _NroDocumento;
        public int _TipoDocumento;
        private bool SeleccionarVentaRapida = false;
        private bool Sincronizando { get; set; }

        #region Permisos Botones
        bool _btnAgregar = false;
        bool _btnEditar = false;
        bool _btnEliminar = false;
        bool _btnVentaRapida = false;
        #endregion

        public frmBandejaRegistroVenta(string modo)
        {
            InitializeComponent();
            Modo = modo;
        }

        private void frmBandejaRegistroVenta_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            #region ControlAcciones
            var _formActions = _objSecurityBl.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmBandejaRegistroVentaNotaria", Globals.ClientSession.i_RoleId);
            _btnAgregar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaRegistroVentaNotaria_ADD", _formActions);
            _btnEditar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaRegistroVentaNotaria_EDIT", _formActions);
            _btnEliminar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaRegistroVentaNotaria_DELETE", _formActions);
            _btnVentaRapida = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmBandejaRegistroVentaNotaria_VENTA_RAPIDA", _formActions);
            btnVentaRapida.Enabled = _btnVentaRapida;
            btnAgregar.Enabled = _btnAgregar;
            btnEliminar.Enabled = _btnEliminar;
            btnEditar.Enabled = _btnEditar;
            #endregion

            this.BackColor = new GlobalFormColors().FormColor;

            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
            _ListadoComboDocumentosGuiaRemision = _objDocumentoBL.ObtenDocumentosParaComboGridGuiaRemision(ref objOperationResult, null, 0, 1);
            dtpFechaInicio.Value = DateTime.Parse(DateTime.Today.Day.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + Globals.ClientSession.i_Periodo.ToString()); //DateTime.Parse("01/" + DateTime.Today.Month.ToString() + "/" + Globals.ClientSession.i_Periodo.ToString());
            dtpFechaFin.Value = DateTime.Parse(DateTime.Today.Day.ToString() + "/" + DateTime.Today.Month.ToString() + "/" + Globals.ClientSession.i_Periodo.ToString());
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
            if (Modo == "GuiaRemisión")
            {
                this.Text = "Documentos de Referencia de Ventas";
                btnEliminar.Visible = false;
                btnAgregar.Visible = false;
                btnEditar.Visible = false;
                btnVentaRapida.Visible = false;
                Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _ListadoComboDocumentosGuiaRemision, DropDownListAction.All);
                this.Size = new System.Drawing.Size(1010, 650);
                this.Location = new Point(178, 42);
            }
            else
            {
                Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.All);
            }
            cboTipoDocumento.Value = "-1";
            btnRegenerarDbf.Visible = Globals.ClientSession.i_SystemUserId == 1;

           
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {

            if (uvDatos.Validate(true, false).IsValid)
            {
                List<string> Filters = new List<string>();
                if (txtMes.Text.Trim() != "" && txtCorrelativoMes.Text.Trim() != "")
                {
                    Filters.Add("v_Mes==\"" + int.Parse(txtMes.Text.Trim()).ToString() + "\"");
                    Filters.Add("v_Correlativo==\"" + txtCorrelativoMes.Text.Trim() + "\"");
                }

                if (cboTipoDocumento.Value.ToString() != "-1") Filters.Add("i_IdTipoDocumento==" + cboTipoDocumento.Value.ToString());

                if (txtSerieDoc.Text != "") Filters.Add("v_SerieDocumento==\"" + txtSerieDoc.Text.Trim() + "\"");

                if (txtCorrelativoDoc.Text != "") Filters.Add("v_CorrelativoDocumento==\"" + txtCorrelativoDoc.Text.Trim() + "\"");

                if (v_IdCliente != string.Empty) Filters.Add("v_IdCliente==\"" + v_IdCliente + "\"");

                if (Modo == "GuiaRemisión") Filters.Add("i_IdEstado==1");

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
                {
                    this.BindGrid();
                }

                grdData.Focus();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null && !grdData.ActiveRow.IsGroupByRow && !grdData.ActiveRow.IsFilterRow && grdData.ActiveRow.Activation != Activation.Disabled)
            {
                if (Utils.Windows.HaveFormChild(this, typeof(frmRegistroVentaNBS), true)) return;
                pstrIdVenta = grdData.ActiveRow.Cells["v_IdVenta"].Value.ToString();

                SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya.frmRegistroVentaNBS frm = new frmRegistroVentaNBS("Edicion", pstrIdVenta);
                frm.FormClosed += delegate
                {
                    BindGrid();
                    MantenerSeleccion(pstrIdVenta);
                };
                ((frmMaster)MdiParent).RegistrarForm(this, frm);
            }
        }

        private void BindGrid()
        {
            var objData = GetData("NroRegistro ASC", _strFilterExpression);

            grdData.SetDataBinding(objData, "");
            if (objData != null)
            {
                if (objData.Count > 0)
                {
                    lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
                    btnEditar.Enabled = _btnEditar;
                    btnEliminar.Enabled = _btnEliminar;
                    btnEliminar.Enabled = VentaBL.TienePermisoEliminar;
                }
                else
                {
                    lblContadorFilas.Text = string.Format("Se encontraron {0} registros.", objData.Count);
                    btnEditar.Enabled = _btnEditar;
                    btnEliminar.Enabled = VentaBL.TienePermisoEliminar;
                }

                if (Globals.ClientSession.UsuarioEsContable == 0) CierraOperacionesContables();
            }
        }

        private List<ventaDto> GetData(string pstrSortExpression, string pstrFilterExpression)
        {
            OperationResult objOperationResult = new OperationResult();
            var _objData = VentaNbsBl.ListarBusquedaVentas(ref objOperationResult, pstrSortExpression, pstrFilterExpression, dtpFechaInicio.Value.Date, DateTime.Parse(dtpFechaFin.Text + " 23:59"));

            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show("Error en operación:" + System.Environment.NewLine + objOperationResult.ExceptionMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return _objData.ToList();
        }


        private void MantenerSeleccion(string ValorSeleccionado)
        {

            if (string.IsNullOrEmpty(ValorSeleccionado)) return;
            var filas = grdData.DisplayLayout.Bands[0].GetRowEnumerator(GridRowType.DataRow).Cast<UltraGridRow>();
            var fila = filas.FirstOrDefault(f => f.Cells["v_IdVenta"].Value.ToString().Contains(ValorSeleccionado));
            if (fila != null) fila.Activate();

        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            //btnEditar.Enabled = row != null;
            btnEditar.Enabled = _btnEditar;
            btnEliminar.Enabled = _btnEliminar;
            btnEliminar.Enabled = VentaBL.TienePermisoEliminar;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmRegistroVentaNBS))) return;
            SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya.frmRegistroVentaNBS frm = new frmRegistroVentaNBS("Nuevo", "");
            frm.FormClosed += delegate
            {
                BindGrid();
            };
            ((frmMaster)MdiParent).RegistrarForm(this, frm);
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].Columns["i_IdEstado"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
        }

        private void txtMes_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
        }

        private void txtMes_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtMes, e);
        }

        private void txtCorrelativoMes_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoMes, "{0:00000000}");
        }

        private void txtCorrelativoMes_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoMes, e);
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
                row.Hidden = filterRow;
            }
        }

        private void cboTipoDocumento_Leave(object sender, EventArgs e)
        {
            GridKeyValueDTO x = new GridKeyValueDTO();

            if (cboTipoDocumento.Text.Trim() == "")
            {
                cboTipoDocumento.Value = "-1";
            }
            else
            {
                if (Modo == "GuiaRemisión")
                {
                    x = _ListadoComboDocumentosGuiaRemision.Find(p => p.Id == cboTipoDocumento.Value | p.Id == cboTipoDocumento.Text);

                }
                else
                {
                    x = _ListadoComboDocumentos.Find(p => p.Id == cboTipoDocumento.Value | p.Id == cboTipoDocumento.Text);
                }
                if (x == null)
                {
                    cboTipoDocumento.Value = "-1";
                }
            }


        }

        private void txtSerieDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc, e);
        }

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc, "{0:0000}");
        }

        private void txtCorrelativoDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDoc, e);
        }

        private void txtCorrelativoDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDoc, "{0:00000000}");
        }

        private void cboTipoDocumento_ValueChanged(object sender, EventArgs e)
        {
            if (cboTipoDocumento.Value != null && cboTipoDocumento.Value.ToString() != "-1")
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

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (grdData.ActiveRow == null || grdData.ActiveRow.IsFilterRow || grdData.ActiveRow.IsGroupByRow ||
                grdData.ActiveRow.Activation == Activation.Disabled) return;

            pstrIdVenta = grdData.ActiveRow.Cells["v_IdVenta"].Value.ToString();
            if (new LetrasBL().VentaFueCanjeadaALetras(ref objOperationResult, pstrIdVenta))
            {
                UltraMessageBox.Show("Esta venta fue canjeada en letras, no se puede eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            string ubicacion;
            if (_objVentasBL.TieneCobranzasRealizadas(pstrIdVenta, out ubicacion))
            {
                UltraMessageBox.Show("Imposible Eliminar un Documento con Cobranzas Realizadas \r" + ubicacion, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            var ventaDto = (ventaDto)grdData.ActiveRow.ListObject;
            var TieneNotaCredito = _objVentasBL.VentaYaTieneNRC(ref objOperationResult, ventaDto.i_IdTipoDocumento.Value, ventaDto.v_SerieDocumento, ventaDto.v_CorrelativoDocumento);

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage,
                    @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (TieneNotaCredito)
            {
                MessageBox.Show(@"Esta venta cuenta con una nota de crédito activa.", @"Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                var nroDoc = grdData.ActiveRow.Cells["Documento"].Value.ToString();
                if (nroDoc.Contains('-'))
                {
                    string[] serieCorrelativo = nroDoc.Split('-');
                    string serie = serieCorrelativo[0].Trim();
                    string correlativo = serieCorrelativo[1].Trim();
                    var GuiasRemision =
                        _objVentasBL.ObtenerDetalleGuiaRemisionporDocumentoRef(
                            int.Parse(grdData.ActiveRow.Cells["i_IdTipoDocumento"].Value.ToString()), serie, correlativo);

                    if (GuiasRemision.Any())
                    {
                        UltraMessageBox.Show("Imposible Eliminar  un Documento con Guia de Remisión Generada",
                            "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            if (UltraMessageBox.Show("¿Está seguro de Eliminar esta venta de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var f = new frmConfirmarMotivoEliminacion();
                var mot = f.ShowDialog();
                if (mot == DialogResult.OK)
                {
                    _objVentaBL.EliminarVenta(ref objOperationResult, pstrIdVenta, Globals.ClientSession.GetAsList(), f.MotivoEliminacion);


                    btnBuscar_Click(sender, e);

                    if (objOperationResult.Success == 0)
                    {
                        if (objOperationResult.ErrorMessage == null)
                            UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        else
                            UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Band.Index == 0 &&
                e.Row.Cells["i_IdEstado"].Value.ToString() == "0")
            {
                e.Row.Appearance.BackColor = Color.Salmon;
                e.Row.Appearance.BackColor2 = Color.Salmon;
                e.Row.Appearance.ForeColor = Color.Black;
            }
        }

        private void dtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            dtpFechaInicio.MaxDate = dtpFechaFin.Value;
        }

        private void grdData_DoubleClick(object sender, EventArgs e)
        {
            if (Modo == "GuiaRemisión")
            {
                if (grdData.Rows.Count == 0) return; //se cambio

                if (grdData.ActiveRow != null)
                {

                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdVenta"].Value != null)
                    {
                        _TipoDocumento = int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdTipoDocumento"].Value.ToString());
                        _NroDocumento = grdData.Rows[grdData.ActiveRow.Index].Cells["Documento"].Value.ToString();
                        this.Close();
                    }
                }
            }
        }

        private void grdData_DoubleClickRow(object sender, DoubleClickRowEventArgs e)
        {
            if (grdData.ActiveRow != null && Modo != "GuiaRemisión")
            {
                if (btnEditar.Enabled) btnEditar_Click(sender, e);
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

        private void txtRucProveedor_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscarCliente")
            {
                frmBuscarCliente f = new frmBuscarCliente("VV", "");
                f.ShowDialog();
                if (f._IdCliente != null)
                {
                    txtCliente.Tag = f._IdCliente.ToString();
                    txtCliente.Text = string.Format("{0} - {1}", f._NroDocumento, f._RazonSocial);
                    v_IdCliente = f._IdCliente;
                    txtCliente.ButtonsRight["btnEliminar"].Enabled = true;
                }
            }
            else
            {
                txtCliente.Tag = null;
                txtCliente.Clear();
                v_IdCliente = string.Empty;
                txtCliente.ButtonsRight["btnEliminar"].Enabled = false;
            }
        }

        private void txtCliente_Validated(object sender, EventArgs e)
        {

        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (uvDatos.Validate(true, false).IsValid)
            {
                List<string> Filters = new List<string>();
                if (txtMes.Text.Trim() != "" && txtCorrelativoMes.Text.Trim() != "")
                {
                    Filters.Add("v_Mes==\"" + int.Parse(txtMes.Text.Trim()).ToString() + "\"");
                    Filters.Add("v_Correlativo==\"" + txtCorrelativoMes.Text.Trim() + "\"");
                }

                if (cboTipoDocumento.Value.ToString() != "-1") Filters.Add("i_IdTipoDocumento==" + cboTipoDocumento.Value.ToString());

                if (txtSerieDoc.Text != "") Filters.Add("v_SerieDocumento==\"" + txtSerieDoc.Text.Trim() + "\"");

                if (txtCorrelativoDoc.Text != "") Filters.Add("v_CorrelativoDocumento==\"" + txtCorrelativoDoc.Text.Trim() + "\"");

                if (v_IdCliente != string.Empty) Filters.Add("v_IdCliente==\"" + v_IdCliente + "\"");

                if (Modo == "GuiaRemisión") Filters.Add("i_IdEstado==1");

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
                {
                    this.BindGrid();
                }

                grdData.Focus();
            }
        }

        private void btnVentaRapida_Click(object sender, EventArgs e)
        {
            if (Utils.Windows.HaveFormChild(this, typeof(frmRegistroVentaRapidaNBS))) return;
            var ultimaFila = grdData.Rows.LastOrDefault();
            if (ultimaFila != null) ultimaFila.Activate();
            frmRegistroVentaRapidaNBS frm = new frmRegistroVentaRapidaNBS("Nuevo", "");
            frm.FormClosed += (_, ev) =>
            {
                BindGrid();
                MantenerSeleccion(((frmRegistroVentaRapidaNBS)_)._pstrIdMovimiento_Nuevo);
            };
            frm.VentaGuardadaEvent += frm_VentaGuardadaEvent;
            ((frmMaster)MdiParent).RegistrarForm(this, frm);
        }

        private void frm_VentaGuardadaEvent(string idVenta)
        {
            Invoke((MethodInvoker) delegate { lblSincronizando.Visible = true; });
            Task.Factory.StartNew(() =>
            {
                var objDbfSincro = new DbfSincronizador();
                var objOperationResult2 = new OperationResult();
                objDbfSincro.RutaDbfCabecera = NBS_DBF_PathSettings.Default.dbfSincro_Cabecera;
                objDbfSincro.RutaDbfDetalle = NBS_DBF_PathSettings.Default.dbfSincro_Detalle;
                objDbfSincro.ActualizarDatosVenta(ref objOperationResult2, idVenta, DbfSincronizador.TipoAccion.Venta);
                if (objOperationResult2.Success == 0) 
                    MessageBox.Show(objOperationResult2.ErrorMessage, @"Error al sincronizar DBF", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                
            }, TaskCreationOptions.LongRunning)
            .ContinueWith(t =>
            {
                Invoke((MethodInvoker)delegate { lblSincronizando.Visible = false; });
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (Application.OpenForms["frmRegistroVentaRapidaNBS"] != null && grdData.ActiveRow != null && SeleccionarVentaRapida)
            {
                var ventaRapidaForm = (frmRegistroVentaRapidaNBS)Application.OpenForms["frmRegistroVentaRapidaNBS"];
                var idVenta = grdData.ActiveRow.Cells["v_IdVenta"].Value.ToString();
                ventaRapidaForm.CargarCabecera(idVenta, false);
            }
        }

        public bool MarcarFilaAnterior()
        {
            SeleccionarVentaRapida = true;
            return grdData.PerformAction(UltraGridAction.AboveRow);
        }

        public bool MarcarFilaSiguiente()
        {
            SeleccionarVentaRapida = true;
            return grdData.PerformAction(UltraGridAction.BelowRow);
        }

        public void ActualizarBandeja()
        {
            SeleccionarVentaRapida = false;
            btnBuscar_Click(this, new EventArgs());
            SeleccionarVentaRapida = true;
            var ultimaFila = grdData.Rows.LastOrDefault();
            if (ultimaFila != null) ultimaFila.Activate();
        }

        private void chkFiltroPersonalizado_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.MostrarOcultarFiltrosGrilla(grdData);
        }

        private void chkBandejaAgrupable_CheckedChanged(object sender, EventArgs e)
        {
            Utils.Windows.HacerGrillaAgrupable(grdData, chkBandejaAgrupable.Checked);
        }

        private void btnExportarBandeja_Click(object sender, EventArgs e)
        {
            if (!grdData.Rows.Any() || (_tarea != null && !_tarea.IsCompleted)) return;

            const string dummyFileName = "Bandeja Ventas";
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

        private void frmBandejaRegistroVenta_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_tarea != null && !_tarea.IsCompleted)
                cts.Cancel();
        }

        private void CierraOperacionesContables()
        {
            var filas = grdData.Rows.GetRowEnumerator(GridRowType.DataRow, null, null).Cast<UltraGridRow>().ToList();
            filas.ForEach(fila =>
            {
                if (fila.Cells["v_UsuarioCreacion"].Value != null)
                {
                    var opContable = Globals.UsuariosContables.Contains(fila.Cells["v_UsuarioCreacion"].Value.ToString());
                    fila.Activation = opContable ? Activation.Disabled : Activation.ActivateOnly;
                }
            });
        }

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            var idVenta = grdData.ActiveRow.Cells["v_IdVenta"].Value.ToString();
            var saldo = decimal.Parse(grdData.ActiveRow.Cells["Saldo"].Value.ToString());
            var f = new frmConsultaHistorialPagosVenta(idVenta, frmConsultaHistorialPagosVenta.TipoBusqueda.Venta);
            f.ShowDialog();
        }

        private void btnRegenerarDbf_Click(object sender, EventArgs e)
        {
            var f = new FrmRegenerarDataDbf();
            f.ShowDialog();
        }
    }
}
