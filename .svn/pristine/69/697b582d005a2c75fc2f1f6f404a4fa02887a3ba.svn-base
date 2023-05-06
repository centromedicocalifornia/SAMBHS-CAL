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
using SAMBHS.Cobranza.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using System.Text.RegularExpressions;
using Infragistics.Win.UltraWinMaskedEdit;
using CrystalDecisions.CrystalReports.Engine;
using System.Data.Sql;
using System.Linq.Dynamic;
using System.Data.SqlClient;
using System.Configuration;
using SAMBHS.Security.BL;
using CrystalDecisions.Shared;
using System.Reflection;
using SAMBHS.Contabilidad.BL;
using LoadingClass;
using SAMBHS.Common.DataModel;
using SAMBHS.Requerimientos.NBS;
using SAMBHS.Tesoreria.BL;
using SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya;



namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmCobranza : Form
    {
        readonly CobranzaBL _objCobranzaBl = new CobranzaBL();
        readonly DocumentoBL _objDocumentoBl = new DocumentoBL();
        readonly SecurityBL _objSecurityBl = new SecurityBL();
        readonly DatahierarchyBL _objDatahierarchyBl = new DatahierarchyBL();
        cobranzaDto _cobranzaDto = new cobranzaDto();
        cobranzadetalleDto _cobranzadetalleDto = new cobranzadetalleDto();
        readonly UltraCombo _ucTipoDocumento = new UltraCombo();
        readonly UltraCombo _ucFormasPago = new UltraCombo();
        List<KeyValueDTO> _listadoCobranzas = new List<KeyValueDTO>();
        List<GridKeyValueDTO> _listadoComboDocumentos = new List<GridKeyValueDTO>();
        readonly List<string[]> _listaVentas;
        string _mode, _strModo, _strIdCobranza;
        int _maxV;
        bool _documentoImpreso = false;
        readonly bool _sePuedeEliminar;

        #region Temporales Detalles de Cobranza
        List<cobranzadetalleDto> _tempDetalleAgregarDto = new List<cobranzadetalleDto>();
        List<cobranzadetalleDto> _tempDetalleModificarDto = new List<cobranzadetalleDto>();
        List<cobranzadetalleDto> _tempDetalleEliminarDto = new List<cobranzadetalleDto>();
        #endregion
        #region Permisos Botones
        bool _btnBuscarPendientes = false;
        bool _btnGuardar = false;
        bool _btnImprimir = false;
        #endregion

        public frmCobranza(string Modo, string idVenta, List<string[]> listaVentas, bool sePuedeEliminar = false)
        {
            _strModo = Modo;
            _strIdCobranza = idVenta;
            _listaVentas = listaVentas;
            InitializeComponent();
            _sePuedeEliminar = sePuedeEliminar;
        }

        private void frmCobranza_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            UltraStatusbarManager.Inicializar(ultraStatusBar1);
            #region Controla Cierre de Mes
            if (new CierreMensualBL().VerificarMesCerrado(Globals.ClientSession.i_Periodo.ToString(), DateTime.Now.Month.ToString(), (int)ModulosSistema.CtasPagar))
            {
                btnGuardar.Visible = false;
                this.Text = "Cobranza [MES CERRADO]";
            }
            else
            {

                btnGuardar.Visible = true;
                this.Text = "Cobranza";
            }
            #endregion
            #region ControlAcciones
            var _formActions = _objSecurityBl.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmCobranza", Globals.ClientSession.i_RoleId);

            _btnBuscarPendientes = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmCobranza_PENDIENTES", _formActions);
            _btnGuardar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmCobranza_SAVE", _formActions);
            _btnImprimir = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmCobranza_PRINT", _formActions);
            btnGuardar.Enabled = _btnGuardar;
            btnBuscarPendientes.Enabled = _btnBuscarPendientes;
            BtnImprimir.Enabled = _btnImprimir;
            #endregion

            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            linkAsiento.BackColor = new GlobalFormColors().BannerColor;
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            txtMes.Text = DateTime.Now.Month.ToString("00");
            _listadoComboDocumentos = _objDocumentoBl.ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _listadoComboDocumentos, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMedioPago, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForComboWithValue2(ref objOperationResult, 44, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstado, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 26, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMedioPago, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForComboWithValue2(ref objOperationResult, 44, null), DropDownListAction.Select);
            //Utils.Windows.LoadUltraComboEditorList(cboFormaPagoDetalle, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 46, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboFormaPagoDetalle, "Value1", "Id", _objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 46, null, UserConfig.Default.MostrarSoloFormasPagoAlmacenActual), DropDownListAction.Select);

            cboFormaPagoDetalle.Value = "-1";
            cboTipoDocumento.Value = "-1";
            CargarCombosDetalle();
            cboMoneda.SelectedIndex = 2;
            ObtenerListadoCobranzas(txtPeriodo.Text, txtMes.Text.Trim());
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();
            if (Application.OpenForms["frmMaster"] != null) ((frmMaster)Application.OpenForms["frmMaster"]).Activate();
            btnEliminarCobranza.Visible = btnGuardar.Visible && _sePuedeEliminar;
            if (cboEstado.Value != null && cboEstado.Value.ToString() == "0")
                cboEstado.Enabled = false;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvDatos.Validate(true, false).IsValid)
            {
                if (CalcularTotales() == -1)
                {
                    grdData.Focus();
                    return;
                }

                if (cboTipoDocumento.Value.ToString() == "-1")
                {
                    UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por Favor seleccione un tipo de documento.", timer1);
                    cboTipoDocumento.PerformAction(UltraComboAction.Dropdown);
                    return;
                }

                if (!grdData.Rows.Any())
                {
                    UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por Favor ingrese almenos una fila al detalle del documento.", timer1);
                    return;
                }
                if (ValidaCamposNulosVacios())
                {
                    if (grdData.Rows.Any(Fila => Fila.Cells["d_ImporteSoles"].Value == null))
                    {
                        UltraStatusbarManager.MarcarError(ultraStatusBar1, "Todos los Totales no están calculados.", timer1);
                        return;
                    }
                }
                else
                {
                    return;
                }
                grdData.Rows.ToList().ForEach(f => f.Cells["i_RegistroEstado"].Value = "Modificado");

                switch (_mode)
                {
                    case "New":
                        while (_objCobranzaBl.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text, int.Parse(cboTipoDocumento.Value.ToString())) == false)
                        {
                            txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                        }
                        _cobranzaDto.i_IdTipoDocumento = int.Parse(cboTipoDocumento.Value.ToString());
                        _cobranzaDto.v_Correlativo = txtCorrelativo.Text;
                        _cobranzaDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                        _cobranzaDto.v_Nombre = txtNombre.Text;
                        _cobranzaDto.v_Mes = txtMes.Text;
                        _cobranzaDto.v_Periodo = txtPeriodo.Text;
                        _cobranzaDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                        _cobranzaDto.v_Glosa = txtGlosa.Text.Trim();
                        _cobranzaDto.v_Mes = txtMes.Text.Trim();
                        _cobranzaDto.v_Periodo = txtPeriodo.Text.Trim();
                        _cobranzaDto.v_Correlativo = txtCorrelativo.Text;
                        _cobranzaDto.d_TotalDolares = decimal.Parse(txtTotalDolares.Text.Trim());
                        _cobranzaDto.d_TotalSoles = decimal.Parse(txtTotalSoles.Text.Trim());
                        _cobranzaDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                        _cobranzaDto.i_IdMedioPago = int.Parse(cboMedioPago.Value.ToString());
                        _cobranzaDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                        LlenarTemporalesCobranza();
                        _objCobranzaBl.InsertarCobranza(ref objOperationResult, _cobranzaDto, Globals.ClientSession.GetAsList(), _tempDetalleAgregarDto);
                        break;

                    case "Edit":
                        var letrasMantenimiento = CobranzaBL.LetrasEnCobranzaFueronCanceladasProtestadas(_cobranzaDto.v_IdCobranza);
                        if (letrasMantenimiento.Any())
                        {
                            MessageBox.Show(
                                string.Format("Las siguientes letras tienen mantenimiento: \n - LEC {0}", string.Join("\n - LEC ", letrasMantenimiento)),
                                @"No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return;
                        }
                        _cobranzaDto.i_IdTipoDocumento = int.Parse(cboTipoDocumento.Value.ToString());
                        _cobranzaDto.v_Correlativo = txtCorrelativo.Text;
                        _cobranzaDto.v_Nombre = txtNombre.Text;
                        _cobranzaDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                        _cobranzaDto.v_Mes = txtMes.Text;
                        _cobranzaDto.v_Periodo = txtPeriodo.Text;
                        _cobranzaDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                        _cobranzaDto.v_Glosa = txtGlosa.Text.Trim();
                        _cobranzaDto.v_Mes = txtMes.Text.Trim();
                        _cobranzaDto.v_Periodo = txtPeriodo.Text.Trim();
                        _cobranzaDto.v_Correlativo = txtCorrelativo.Text;
                        _cobranzaDto.d_TotalDolares = decimal.Parse(txtTotalDolares.Text.Trim());
                        _cobranzaDto.d_TotalSoles = decimal.Parse(txtTotalSoles.Text.Trim());
                        _cobranzaDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                        _cobranzaDto.i_IdMedioPago = int.Parse(cboMedioPago.Value.ToString());
                        _cobranzaDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                        LlenarTemporalesCobranza();
                        _objCobranzaBl.ActualizarCobranza(ref objOperationResult, _cobranzaDto, Globals.ClientSession.GetAsList(), _tempDetalleAgregarDto, _tempDetalleModificarDto, _tempDetalleEliminarDto);
                        break;
                }

                if (objOperationResult.Success == 1)
                {
                    _strModo = "Guardado";
                    _strIdCobranza = _cobranzaDto.v_IdCobranza;
                    ObtenerListadoCobranzas(txtPeriodo.Text, txtMes.Text);
                    cboTipoDocumento.Enabled = false;

                    linkAsiento.Visible = Globals.ClientSession.UsuarioEsContable == 1;
                    UltraStatusbarManager.Mensaje(ultraStatusBar1, "El registro se ha guardado correctamente", timer1);

                    #region Requerimientos para Notaria Becerra Sosaya
                    if (Globals.ClientSession.v_RucEmpresa.Equals(Constants.RucNotariaBecerrSosaya))
                    {
                        var objDbfSincro = new DbfSincronizador();
                        var objOperationResult2 = new OperationResult();
                        objDbfSincro.RutaDbfCabecera = NBS_DBF_PathSettings.Default.dbfSincro_Cabecera;
                        objDbfSincro.RutaDbfDetalle = NBS_DBF_PathSettings.Default.dbfSincro_Detalle;
                        foreach (var row in grdData.Rows)
                        {
                            var idVenta = row.Cells["v_IdVenta"].Value.ToString();
                            var idformaPago = int.Parse(row.Cells["i_IdFormaPago"].Value.ToString());
                            var idCobranzaDetalle = row.Cells["v_IdCobranzaDetalle"].Value.ToString();
                            decimal importe;
                            decimal.TryParse(row.Cells["d_ImporteSoles"].Value.ToString(), out importe);
                            objDbfSincro.ActualizarDatosVenta(ref objOperationResult2, idVenta, DbfSincronizador.TipoAccion.Cobranza,
                                new DbfSincronizador.DatosCobranza
                                {
                                    FechaCobranza = dtpFechaRegistro.Value,
                                    IdFormaPago = idformaPago,
                                    MontoCobrado = importe,
                                    IdCobranzaDetalle = idCobranzaDetalle
                                });

                            if (objOperationResult2.Success == 0)
                            {
                                MessageBox.Show(objOperationResult2.ErrorMessage,
                                                @"Error al sincronizar DBF", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }

                        foreach (var cobranzadetalleDto in _tempDetalleEliminarDto)
                        {
                            objDbfSincro.EliminarCobranzaDetalle(ref objOperationResult,
                                cobranzadetalleDto.v_IdCobranzaDetalle, dtpFechaRegistro.Value);
                            if (objOperationResult2.Success == 0)
                            {
                                MessageBox.Show(objOperationResult2.ErrorMessage,
                                                @"Error al sincronizar DBF", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                _tempDetalleAgregarDto = new List<cobranzadetalleDto>();
                _tempDetalleModificarDto = new List<cobranzadetalleDto>();
                _tempDetalleEliminarDto = new List<cobranzadetalleDto>();

                var resp = MessageBox.Show(@"¿Desea Continuar con un registro nuevo?", @"Sistema", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                if (resp == System.Windows.Forms.DialogResult.Yes)
                    PrepararNuevo();
            }
            else
            {
                UltraMessageBox.Show("Por favor llene los campos requeridos antes de guardar", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void linkAsiento_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           
        }

        private void PrepararNuevo()
        {
            _strModo = "Nuevo";
            frmCobranza_Load(this, new EventArgs());
            cboTipoDocumento.Enabled = true;
            cboTipoDocumento.PerformAction(UltraComboAction.Dropdown);
            linkAsiento.Visible = false;
        }

        private void btnBuscarDetraccion_Click(object sender, EventArgs e)
        {
            if (cboTipoDocumento.Value != null && cboTipoDocumento.Value.ToString() != "-1" && cboMoneda.Value.ToString() != "-1")
            {
                txtTipoCambio.Text = string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? "0" : txtTipoCambio.Text;

                if (decimal.Parse(txtTipoCambio.Text) <= 0)
                {
                    UltraMessageBox.Show("Por favor elija un tipo de cambio primero", "Sistema", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                    txtTipoCambio.Focus();
                }
                else
                {
                    frmCobranzaPendienteConsulta frm = new frmCobranzaPendienteConsulta(null, true, false, cboTipoDocumento.Text);
                    frm.ShowDialog();
                }
            }
            var objOperationResult = new OperationResult();
            var existenLetras = grdData.Rows.Any(r => r.Cells["i_EsLetra"].Value.ToString() == "1");

            if (existenLetras)
            {
                if (cboTipoDocumento.Value == null) return;
                var cadena = new TesoreriaBL().DevuelveCuentaCajaBanco(ref objOperationResult, int.Parse(cboTipoDocumento.Value.ToString()));
                var estado = !string.IsNullOrWhiteSpace(cadena.NroCuenta) && !cadena.NroCuenta.StartsWith("104");
                grdData.DisplayLayout.Bands[0].Columns["d_IngresosFinancieros"].Hidden = estado;
                grdData.DisplayLayout.Bands[0].Columns["d_GastosFinancieros"].Hidden = estado;
            }

        }

        private void cboTipoDocumento_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboTipoDocumento.Rows)
            {
                if (cboTipoDocumento.Value != null && cboTipoDocumento.Value.ToString() == "-1") cboTipoDocumento.Text = string.Empty;
                bool filterRow = cboTipoDocumento.DisplayLayout.Bands[0].Columns.Cast<UltraGridColumn>().Where(column => column.IsVisibleInLayout).All(column => !row.Cells[column].Text.Contains(cboTipoDocumento.Text.ToUpper()));
                row.Hidden = filterRow;
            }
        }

        private void cboTipoDocumento_Leave(object sender, EventArgs e)
        {
            //
            var objOperationResult = new OperationResult();
            if (cboTipoDocumento.Text.Trim() == "")
            {
                cboTipoDocumento.Value = "-1";
            }
            else
            {
                var x = _listadoComboDocumentos.Find(p => p.Id == cboTipoDocumento.Value.ToString() || p.Id == cboTipoDocumento.Text);
                if (x == null)
                {
                    cboTipoDocumento.Value = "-1";
                }
            }
            ObtenerCorrelativoCobranza();
            if (cboTipoDocumento.Value != null)
            {


                bool tieneCuentaContable;
                var idMoneda = _objCobranzaBl.DevuelveMonedaPorDocumento(ref objOperationResult, int.Parse(cboTipoDocumento.Value.ToString()), out tieneCuentaContable);

                if (objOperationResult.Success == 1)
                {
                    cboMoneda.Value = idMoneda.ToString();
                }
                else
                {
                    UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!tieneCuentaContable)
                {
                    UltraStatusbarManager.MarcarError(ultraStatusBar1, "Este documento no tiene una cuenta contable relacionada", timer1);
                    btnGuardar.Enabled = false;
                }
                else
                {
                    UltraStatusbarManager.Inicializar(ultraStatusBar1);
                    btnGuardar.Enabled = true;
                }
            }
            if (_strModo == "Edicion") return;
            if (grdData.Rows.Any() && cboTipoDocumento.Value.ToString() == "300") //300 = caja soles
                grdData.Rows.ToList().ForEach(fila =>
                {
                    fila.Cells["i_IdFormaPago"].Value = "1"; // si es caja soles coloca la forma de pago como efectivo de manera predeterminada.
                });

            int i;
            cboFormaPagoDetalle.Value = CobranzaBL.ObtenerFormaPagoPorDocumento(ref objOperationResult,
                int.TryParse((cboTipoDocumento.Value ?? "-1").ToString(), out i) ? i : -1);
            _ucFormasPago.DataSource = new List<GridKeyValueDTO>();
            Utils.Windows.LoadUltraComboList(_ucFormasPago, "Value1", "Id", _objCobranzaBl.ListadoFormasPago(i), DropDownListAction.Select);
            btnMarcarTodos_Click(sender, e);
        }

        private void btnEliminarDetalle_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _cobranzadetalleDto = new cobranzadetalleDto
                    {
                        v_IdCobranzaDetalle =
                            grdData.ActiveRow.Cells["v_IdCobranzaDetalle"].Value.ToString()
                    };
                    _tempDetalleEliminarDto.Add(_cobranzadetalleDto);
                    grdData.ActiveRow.Delete(false);
                }
            }
            else
            {
                grdData.ActiveRow.Delete(false);
            }
            CalcularValoresDetalle();
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            Reportes.Cobranza.frmDocumentoVoucher frm = new Reportes.Cobranza.frmDocumentoVoucher(_cobranzaDto.v_IdCobranza, (int)Globals.ClientSession.i_PrecioDecimales, "COBRANZA");
            frm.ShowDialog();
            _documentoImpreso = true;
        }

        private void txtNombre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (txtNombre.CanUndo())
                {
                    txtNombre.Undo();
                }
            }
        }

        private void txtTipoCambio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (txtTipoCambio.CanUndo())
                {
                    txtTipoCambio.Undo();
                }
            }
        }

        private void dtpFechaRegistro_Leave(object sender, EventArgs e)
        {
            if (_mode == "New")
            {
                var objOperationResult = new OperationResult();
                txtTipoCambio.Text = _objCobranzaBl.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
            }
        }

        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
            txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Cobranza, _cobranzaDto.t_FechaRegistro, dtpFechaRegistro.Value, _cobranzaDto.v_Correlativo, int.Parse(cboTipoDocumento.Value.ToString()));
            #region Controla Cierre de Mes
            if (new CierreMensualBL().VerificarMesCerrado(txtPeriodo.Text, txtMes.Text, (int)ModulosSistema.CtasPagar))
            {
                btnGuardar.Visible = false;
                this.Text = "Cobranza [MES CERRADO]";
            }
            else
            {

                btnGuardar.Visible = true;
                this.Text = "Cobranza";
            }
            #endregion
            ObtenerCorrelativoCobranza();
        }

        #region Clases / Validaciones
        private void ObtenerListadoCobranzas(string pstrPeriodo, string pstrMes)
        {
            var objOperationResult = new OperationResult();
            _listadoCobranzas = _objCobranzaBl.ObtenerListadoCobranzas(ref objOperationResult, pstrPeriodo, pstrMes, int.Parse(cboTipoDocumento.Value.ToString()));
            switch (_strModo)
            {
                case "Edicion":
                    CargarCabecera(_strIdCobranza);
                    if (_cobranzaDto.i_IdEstado == 0) btnGuardar.Enabled = false;
                    cboTipoDocumento.Enabled = false;
                    cboMoneda.Enabled = false;
                    txtTipoCambio.Enabled = false;
                    BtnImprimir.Enabled = true;
                    cboTipoDocumento.Enabled = true;
                    linkAsiento.Visible = Globals.ClientSession.UsuarioEsContable == 1;
                    break;

                case "Nuevo":
                    _mode = "New";
                    LimpiarCabecera();
                    _cobranzaDto = new cobranzaDto();
                    txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Cobranza, _cobranzaDto.t_FechaRegistro, dtpFechaRegistro.Value, _cobranzaDto.v_Correlativo, int.Parse(cboTipoDocumento.Value.ToString()));
                    txtTipoCambio.Text = _objCobranzaBl.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
                    cboMedioPago.Value = "1";
                    cboEstado.Value = "1";
                    cboEstado.Enabled = false;
                    if (_listaVentas == null)
                    {
                        CargarDetalle("");
                    }
                    else
                    {
                        CargarDetalle("");
                        RecibirPedidosFacturados(_listaVentas);
                    }
                    BtnImprimir.Enabled = false;
                    linkAsiento.Visible = false;
                    break;

                case "Guardado":
                    _maxV = _listadoCobranzas.Count - 1;
                    if (_strIdCobranza == "" | _strIdCobranza == null)
                    {
                        CargarCabecera(_listadoCobranzas[_maxV].Value2);
                    }
                    else
                    {
                        CargarCabecera(_strIdCobranza);
                    }
                    BtnImprimir.Enabled = true;
                    linkAsiento.Visible = Globals.ClientSession.UsuarioEsContable == 1;
                    break;

                case "Consulta":
                    if (_listadoCobranzas.Count != 0)
                    {
                        _maxV = _listadoCobranzas.Count - 1;
                        txtCorrelativo.Text = (int.Parse(_listadoCobranzas[_maxV].Value1)).ToString("00000000");
                        CargarCabecera(_listadoCobranzas[_maxV].Value2);
                        _mode = "Edit";
                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _mode = "New";
                        LimpiarCabecera();
                        CargarDetalle("");
                        _maxV = 1;
                        _cobranzaDto = new cobranzaDto();
                        txtTipoCambio.Text = _objCobranzaBl.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
                        txtMes.Enabled = true;
                    }

                    linkAsiento.Visible = Globals.ClientSession.UsuarioEsContable == 1;
                    break;
            }
        }

        private void ObtenerCorrelativoCobranza()
        {
            int i;
            var objOperationResult = new OperationResult();
            var docOriginal = _cobranzaDto.i_IdTipoDocumento ?? -1;
            var periodoOriginal = (_cobranzaDto.t_FechaRegistro ?? DateTime.Now).Year;
            var mesOriginal = (_cobranzaDto.t_FechaRegistro ?? DateTime.Now).Month;
            var docNuevo = int.TryParse(cboTipoDocumento.Value.ToString(), out i) ? i : -1;
            var periodoNuevo = dtpFechaRegistro.Value.Year;
            var mesNuevo = dtpFechaRegistro.Value.Month;

            if (docOriginal != docNuevo || (periodoNuevo != periodoOriginal) || (mesOriginal != mesNuevo))
                txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Cobranza, _cobranzaDto.t_FechaRegistro, dtpFechaRegistro.Value, _cobranzaDto.v_Correlativo, int.Parse(cboTipoDocumento.Value.ToString()));
            else
                txtCorrelativo.Text = _cobranzaDto.v_Correlativo;
        }

        private void CargarCabecera(string IdPago)
        {
            OperationResult objOperationResult = new OperationResult();
            _cobranzaDto = new cobranzaDto();
            _cobranzaDto = _objCobranzaBl.ObtenerCobranzaCabecera(ref objOperationResult, IdPago);
            if (_cobranzaDto != null)
            {
                txtPeriodo.Text = _cobranzaDto.v_Periodo;
                txtMes.Text = int.Parse(_cobranzaDto.v_Mes.ToString().Trim()).ToString("00");
                txtCorrelativo.Text = _cobranzaDto.v_Correlativo;
                cboTipoDocumento.Value = _cobranzaDto.i_IdTipoDocumento.ToString();
                cboEstado.Value = _cobranzaDto.i_IdEstado.ToString();
                cboMedioPago.Value = _cobranzaDto.i_IdMedioPago.ToString();
                txtGlosa.Text = _cobranzaDto.v_Glosa;
                txtNombre.Text = _cobranzaDto.v_Nombre;
                if (_cobranzaDto.t_FechaRegistro != null) dtpFechaRegistro.Value = _cobranzaDto.t_FechaRegistro.Value;
                txtTipoCambio.Text = _cobranzaDto.d_TipoCambio.ToString();
                txtTipoCambio.Text = _cobranzaDto.d_TipoCambio.ToString();
                txtTotalDolares.Text = _cobranzaDto.d_TotalDolares.ToString();
                txtTotalSoles.Text = _cobranzaDto.d_TotalSoles.ToString();
                cboMoneda.Value = _cobranzaDto.i_IdMoneda.ToString();
                _mode = "Edit";
                CargarDetalle(_cobranzaDto.v_IdCobranza);
                Utils.Windows.LoadUltraComboList(_ucFormasPago, "Value1", "Id", _objCobranzaBl.ListadoFormasPago(_cobranzaDto.i_IdTipoDocumento ?? -1), DropDownListAction.Select);
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar la cobranza", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDetalle(string pstringIdCobranza)
        {
            var objOperationResult = new OperationResult();
            try
            {
                var ds = _objCobranzaBl.ObtenerCobranzaDetalle(ref objOperationResult, pstringIdCobranza);
                grdData.DataSource = ds;
                var aplicaret = ds.Cast<cobranzadetalle>().ToList().Any(p => p.i_AplicaRetencion == 1);
                grdData.DisplayLayout.Bands[0].Columns["d_MontoRetencion"].Hidden = !aplicaret;
                grdData.DisplayLayout.Bands[0].Columns["v_NroRetencion"].Hidden = !aplicaret;
                var existenLetras = grdData.Rows.Any(r => r.Cells["i_EsLetra"].Value != null && r.Cells["i_EsLetra"].Value.ToString() == "1");
                if (existenLetras)
                {
                    if (cboTipoDocumento.Value == null) return;
                    var cadena = new TesoreriaBL().DevuelveCuentaCajaBanco(ref objOperationResult, int.Parse(cboTipoDocumento.Value.ToString()));
                    var estado = !string.IsNullOrWhiteSpace(cadena.NroCuenta) && !cadena.NroCuenta.StartsWith("104");
                    grdData.DisplayLayout.Bands[0].Columns["d_IngresosFinancieros"].Hidden = estado;
                    grdData.DisplayLayout.Bands[0].Columns["d_GastosFinancieros"].Hidden = estado;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            DevolverNombreRelaciones();
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
            }
            CalcularTotales();

        }

        private void LimpiarCabecera()
        {

        }

        private void CargarCombosDetalle()
        {
            OperationResult objOperationResult = new OperationResult();

            #region Configura Combo Tipo Documento
            UltraGridBand _ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn _ultraGridColumnaID = new UltraGridColumn("Id");
            UltraGridColumn _ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            UltraGridColumn _ultraGridColumnaSiglas = new UltraGridColumn("Value2");
            _ultraGridColumnaID.Header.Caption = "Cod.";
            _ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            _ultraGridColumnaSiglas.Header.Caption = "Siglas";
            _ultraGridColumnaID.Width = 30;
            _ultraGridColumnaDescripcion.Width = 200;
            _ultraGridColumnaSiglas.Width = 80;
            _ultraGridBanda.Columns.AddRange(new object[] { _ultraGridColumnaID, _ultraGridColumnaDescripcion, _ultraGridColumnaSiglas });
            _ucTipoDocumento.DisplayLayout.BandsSerializer.Add(_ultraGridBanda);
            _ucTipoDocumento.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            _ucTipoDocumento.DropDownWidth = 330;
            #endregion

            #region Configura Combo Formas de Pago
            UltraGridBand ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumnaID = new UltraGridColumn("Id");
            UltraGridColumn ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            ultraGridColumnaID.Header.Caption = "Cod.";
            ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            ultraGridColumnaID.Width = 30;
            ultraGridColumnaDescripcion.Width = 200;
            ultraGridBanda.Columns.AddRange(new object[] { ultraGridColumnaID, ultraGridColumnaDescripcion });
            _ucFormasPago.DisplayLayout.BandsSerializer.Add(ultraGridBanda);
            _ucFormasPago.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            _ucFormasPago.DropDownWidth = 330;
            #endregion

            _ucFormasPago.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(_ucTipoDocumento, "Value2", "Id", _objDocumentoBl.ObtenDocumentosParaComboGridTesoreria(ref objOperationResult, null), DropDownListAction.Select);

        }

        private void LlenarTemporalesCobranza()
        {
            if (grdData.Rows.Count() != 0)
            {
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    int i;
                    switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                    {
                        case "Temporal":
                            if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _cobranzadetalleDto = new cobranzadetalleDto
                                {
                                    v_IdCobranza = _cobranzaDto.v_IdCobranza,
                                    d_NetoXCobrar =
                                        Fila.Cells["d_NetoXCobrar"].Value == null
                                            ? 0
                                            : decimal.Parse(Fila.Cells["d_NetoXCobrar"].Value.ToString()),


                                    v_IdVenta =
                                        Fila.Cells["v_IdVenta"].Value == null
                                            ? null
                                            : Fila.Cells["v_IdVenta"].Value.ToString(),
                                    d_ImporteDolares =
                                        Fila.Cells["d_ImporteDolares"].Value == null
                                            ? 0
                                            : decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString()),

                                    d_ImporteSoles =
                                                       Fila.Cells["d_ImporteSoles"].Value == null
                                                           ? 0
                                                           : decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString()),



                                    d_Redondeo = Fila.Cells["d_Redondeo"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Redondeo"].Value.ToString()),




                                    i_IdTipoDocumentoRef =
                                        Fila.Cells["i_IdTipoDocumentoRef"].Value == null
                                            ? 0
                                            : int.Parse(Fila.Cells["i_IdTipoDocumentoRef"].Value.ToString()),
                                    i_IdFormaPago =
                                        Fila.Cells["i_IdFormaPago"].Value == null
                                            ? 0
                                            : int.Parse(Fila.Cells["i_IdFormaPago"].Value.ToString()),
                                    v_DocumentoRef =
                                        Fila.Cells["v_DocumentoRef"].Value == null
                                            ? null
                                            : Fila.Cells["v_DocumentoRef"].Value.ToString(),
                                    v_Observacion =
                                        Fila.Cells["v_Observacion"].Value == null
                                            ? null
                                            : Fila.Cells["v_Observacion"].Value.ToString(),
                                    Moneda = Fila.Cells["Moneda"].Value.ToString(),
                                    i_EsLetra =
                                        Fila.Cells["i_EsLetra"].Value != null
                                            ? int.Parse(Fila.Cells["i_EsLetra"].Value.ToString())
                                            : 0,
                                    i_AplicaRetencion = Fila.Cells["i_AplicaRetencion"].Value != null
                                            ? int.TryParse(Fila.Cells["i_AplicaRetencion"].Value.ToString(), out i) ? i : 0 : 0,

                                    d_MontoRetencion = Fila.Cells["d_MontoRetencion"].Value == null
                                           ? 0 : decimal.Parse(Fila.Cells["d_MontoRetencion"].Value.ToString()),

                                    v_NroRetencion = Fila.Cells["v_NroRetencion"].Value == null
                                            ? null : Fila.Cells["v_NroRetencion"].Value.ToString(),

                                    i_EsAbonoLetraDescuento =
                                Fila.Cells["i_EsAbonoLetraDescuento"].Value != null
                                    ? int.Parse(Fila.Cells["i_EsAbonoLetraDescuento"].Value.ToString())
                                    : 0,

                                    d_GastosFinancieros = Fila.Cells["d_GastosFinancieros"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_GastosFinancieros"].Value.ToString()),
                                    d_IngresosFinancieros = Fila.Cells["d_IngresosFinancieros"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_IngresosFinancieros"].Value.ToString()),

                                };
                                _tempDetalleAgregarDto.Add(_cobranzadetalleDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _cobranzadetalleDto = new cobranzadetalleDto
                                {
                                    v_IdCobranza =
                                        Fila.Cells["v_IdCobranza"].Value == null
                                            ? null
                                            : Fila.Cells["v_IdCobranza"].Value.ToString(),
                                    v_IdCobranzaDetalle =
                                        Fila.Cells["v_IdCobranzaDetalle"].Value == null
                                            ? null
                                            : Fila.Cells["v_IdCobranzaDetalle"].Value.ToString(),
                                    v_IdVenta =
                                        Fila.Cells["v_IdVenta"].Value == null
                                            ? null
                                            : Fila.Cells["v_IdVenta"].Value.ToString(),
                                    d_NetoXCobrar =
                                        Fila.Cells["d_NetoXCobrar"].Value == null
                                            ? 0
                                            : decimal.Parse(Fila.Cells["d_NetoXCobrar"].Value.ToString()),




                                    d_ImporteDolares =
                                        Fila.Cells["d_ImporteDolares"].Value == null
                                            ? 0
                                            : decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString()),
                                    d_ImporteSoles =
                                        Fila.Cells["d_ImporteSoles"].Value == null
                                            ? 0
                                            : decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString()),



                                    d_Redondeo = Fila.Cells["d_Redondeo"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Redondeo"].Value.ToString()),

                                    i_IdTipoDocumentoRef =
                                        Fila.Cells["i_IdTipoDocumentoRef"].Value == null
                                            ? 0
                                            : int.Parse(Fila.Cells["i_IdTipoDocumentoRef"].Value.ToString()),
                                    v_DocumentoRef =
                                        Fila.Cells["v_DocumentoRef"].Value == null
                                            ? null
                                            : Fila.Cells["v_DocumentoRef"].Value.ToString(),
                                    i_IdFormaPago =
                                        Fila.Cells["i_IdFormaPago"].Value == null
                                            ? 0
                                            : int.Parse(Fila.Cells["i_IdFormaPago"].Value.ToString()),
                                    v_Observacion =
                                        Fila.Cells["v_Observacion"].Value == null
                                            ? null
                                            : Fila.Cells["v_Observacion"].Value.ToString(),
                                    i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString()),
                                    i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString()),
                                    t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value),
                                    i_AplicaRetencion = Fila.Cells["i_AplicaRetencion"].Value != null
                                           ? int.TryParse(Fila.Cells["i_AplicaRetencion"].Value.ToString(), out i) ? i : 0 : 0,

                                    d_MontoRetencion = Fila.Cells["d_MontoRetencion"].Value == null
                                           ? 0 : decimal.Parse(Fila.Cells["d_MontoRetencion"].Value.ToString()),

                                    v_NroRetencion = Fila.Cells["v_NroRetencion"].Value == null
                                            ? null : Fila.Cells["v_NroRetencion"].Value.ToString(),
                                    i_EsLetra =
                                        Fila.Cells["i_EsLetra"].Value != null
                                            ? int.Parse(Fila.Cells["i_EsLetra"].Value.ToString())
                                            : 0,
                                    i_EsAbonoLetraDescuento =
                        Fila.Cells["i_EsAbonoLetraDescuento"].Value != null
                            ? int.Parse(Fila.Cells["i_EsAbonoLetraDescuento"].Value.ToString())
                            : 0,
                                    d_GastosFinancieros = Fila.Cells["d_GastosFinancieros"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_GastosFinancieros"].Value.ToString()),
                                    d_IngresosFinancieros = Fila.Cells["d_IngresosFinancieros"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_IngresosFinancieros"].Value.ToString()),
                                    Moneda = Fila.Cells["Moneda"].Value.ToString()
                                };
                                _tempDetalleModificarDto.Add(_cobranzadetalleDto);
                            }
                            break;
                    }
                }
            }

        }

        private void CalcularValoresDetalle()
        {
            if (!grdData.Rows.Any()) return;
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                CalcularValoresFila(Fila);
            }
        }

        private void CalcularValoresFila(UltraGridRow Fila)
        {
            if (Fila.Cells["i_IdMoneda"].Value == null) return;
            if (Fila.Cells["d_NetoXCobrar"].Value == null) { Fila.Cells["d_NetoXCobrar"].Value = "0"; }
            decimal TipoCambio = decimal.Parse(txtTipoCambio.Text.Trim());
            CalcularTotales();
        }

        private int CalcularTotales()
        {
            if (grdData.Rows.Any())
            {
                decimal SumSoles = 0, SumDolares = 0;

                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["d_ImporteSoles"].Value == null) { Fila.Cells["d_ImporteSoles"].Value = "0"; }
                    if (Fila.Cells["d_ImporteDolares"].Value == null) { Fila.Cells["d_ImporteDolares"].Value = "0"; }

                    if (Globals.ClientSession.i_RedondearVentas == 1 && Fila.Cells["Moneda"].Value.ToString() == Fila.Cells["MonedaOriginal"].Value.ToString())
                    {
                        if (decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString()) > decimal.Parse(Fila.Cells["d_NetoXCobrar"].Value.ToString()))
                        {
                            //UltraStatusbarManager.MarcarError(ultraStatusBar1, "El Importe no debe ser mayor que el Monto por Cobrar", timer1); // REDONDEOCOBRANZA

                            //UltraGridCell aCell = Fila.Cells["d_ImporteSoles"];
                            //grdData.ActiveRow = grdData.ActiveRow;
                            //grdData.ActiveCell = aCell;
                            //grdData.PerformAction(UltraGridAction.EnterEditMode, true, false);

                            //return -1;
                        }
                    }

                    SumSoles = SumSoles + decimal.Parse(Fila.Cells["d_ImporteSoles"].Value.ToString());
                    SumDolares = SumDolares + decimal.Parse(Fila.Cells["d_ImporteDolares"].Value.ToString());
                }
                txtTotalSoles.Text = SumSoles.ToString("0.00");
                txtTotalDolares.Text = SumDolares.ToString("0.00");
            }
            return 0;
        }

        private void DevolverNombreRelaciones()
        {
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                if (grdData.Rows[i].Cells["i_EsLetra"].Value == null || grdData.Rows[i].Cells["i_EsLetra"].Value.ToString() == "0")
                {
                    string[] Cadena = new string[6];
                    Cadena = _objCobranzaBl.DevolverNombres(grdData.Rows[i].Cells["v_IdVenta"].Value.ToString());
                    if (Cadena != null)
                    {
                        grdData.Rows[i].Cells["TipoDocumento"].Value = Cadena[0];
                        grdData.Rows[i].Cells["NroDocumento"].Value = Cadena[1];
                        grdData.Rows[i].Cells["Cliente"].Value = Cadena[2];
                        grdData.Rows[i].Cells["i_IdMoneda"].Value = Cadena[3];
                        grdData.Rows[i].Cells["Moneda"].Value = cboMoneda.Value.ToString() == "1" ? "S/." : "US$.";
                        grdData.Rows[i].Cells["MonedaOriginal"].Value = Cadena[4];
                    }
                }
                else
                {
                    string[] Cadena = new string[6];
                    Cadena = _objCobranzaBl.DevolverNombresLetras(grdData.Rows[i].Cells["v_IdVenta"].Value.ToString());
                    if (Cadena != null)
                    {
                        grdData.Rows[i].Cells["TipoDocumento"].Value = Cadena[0];
                        grdData.Rows[i].Cells["NroDocumento"].Value = Cadena[1];
                        grdData.Rows[i].Cells["Cliente"].Value = Cadena[2];
                        grdData.Rows[i].Cells["i_IdMoneda"].Value = Cadena[3];
                        grdData.Rows[i].Cells["Moneda"].Value = cboMoneda.Value.ToString() == "1" ? "S/." : "US$.";
                        grdData.Rows[i].Cells["MonedaOriginal"].Value = Cadena[4];
                    }
                }
            }
        }

        private bool ValidaCamposNulosVacios()
        {
            if (!UserConfig.Default.PermiteRealizarCobranzasImporteCero)
            {
                if (grdData.Rows.Count(p => p.Cells["d_ImporteSoles"].Value == null || decimal.Parse(p.Cells["d_ImporteSoles"].Value.ToString().Trim()) <= 0) != 0)
                {
                    UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por favor ingrese correctamente el Importe.", timer1);
                    UltraGridRow Row = grdData.Rows.FirstOrDefault(p => p.Cells["d_ImporteSoles"].Value == null || decimal.Parse(p.Cells["d_ImporteSoles"].Value.ToString().Trim()) <= 0);
                    grdData.Selected.Cells.Add(Row.Cells["d_ImporteSoles"]);
                    grdData.Focus();
                    Row.Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_ImporteSoles"];
                    this.grdData.ActiveCell = aCell;
                    grdData.PerformAction(UltraGridAction.EnterEditMode);
                    return false;
                }
            }

            if (grdData.Rows.Any(p => p.Cells["i_IdFormaPago"].Value != null && p.Cells["i_IdFormaPago"].Value.ToString() == "-1"))
            {
                UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por favor ingrese correctamente las formas de pago.", timer1);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(p => p.Cells["i_IdFormaPago"].Value.ToString() == "-1");
                grdData.Selected.Cells.Add(Row.Cells["i_IdFormaPago"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdFormaPago"];
                this.grdData.ActiveCell = aCell;
                grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown);
                return false;
            }




            if (grdData.Rows.Any(p => p.Cells["d_Redondeo"].Value != null && decimal.Parse(p.Cells["d_Redondeo"].Value.ToString()) != 0) && (string.IsNullOrEmpty(Globals.ClientSession.v_NroCuentaCobranzaRedondeoPerdida) || string.IsNullOrEmpty(Globals.ClientSession.v_NroCuentaCobranzaRedondeoGanancia)))
            {
                UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por favor ingrese correctamente las cuentas Redondeo Ganancia y Pérdida para la Cobranza", timer1);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(p => p.Cells["d_Redondeo"].Value != null && decimal.Parse(p.Cells["d_Redondeo"].Value.ToString()) != 0);
                grdData.Selected.Cells.Add(Row.Cells["d_Redondeo"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_Redondeo"];
                this.grdData.ActiveCell = aCell;
                grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown);
                return false;
            }
            if (grdData.Rows.Any(p => !p.Cells["i_IdFormaPago"].Text.Contains("EFECTIVO") && p.Cells["i_IdTipoDocumentoRef"].Value.ToString() == "-1"))
            {
                UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por favor ingrese los documentos de referencia.", timer1);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(p => p.Cells["i_IdFormaPago"].Text != "EFECTIVO" && p.Cells["i_IdTipoDocumentoRef"].Value.ToString() == "-1");
                grdData.Selected.Cells.Add(Row.Cells["i_IdTipoDocumentoRef"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdTipoDocumentoRef"];
                this.grdData.ActiveCell = aCell;
                grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown);
                return false;
            }


            foreach (string IdVenta in grdData.Rows.AsParallel().ToList().Select(p => p.Cells["v_IdVenta"].Value.ToString()).Distinct().ToList())
            {
                List<UltraGridRow> Filas = new List<UltraGridRow>();
                Filas = grdData.Rows.Where(p => p.Cells["v_IdVenta"].Value.ToString() == IdVenta).ToList();

                if (Filas.Count() > 1)
                {
                    foreach (var item in Filas)
                    {
                        if (Filas.Count(p => p.Cells["i_IdTipoDocumentoRef"].Value.ToString() == item.Cells["i_IdTipoDocumentoRef"].Value.ToString()) > 1)
                        {
                            UltraStatusbarManager.MarcarError(ultraStatusBar1, "No se pueden registrar el mismo documento varias veces con la misma modalidad de pago.", timer1);
                            grdData.Selected.Rows.AddRange(Filas.Where(p => p.Cells["i_IdTipoDocumentoRef"].Value.ToString() == item.Cells["i_IdTipoDocumentoRef"].Value.ToString()).ToArray());
                            grdData.Focus();
                            grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                            return false;
                        }
                    }
                }
            }

            foreach (string IdVenta in grdData.Rows.AsParallel().ToList().Select(p => p.Cells["v_IdVenta"].Value.ToString()).Distinct().ToList())
            {
                if (Globals.ClientSession.i_RedondearVentas == 1 && grdData.Rows.Where(p => p.Cells["v_IdVenta"].Value.ToString() == IdVenta && p.Cells["Moneda"].Value.ToString() == p.Cells["MonedaOriginal"].Value.ToString()).Sum(o => decimal.Parse(o.Cells["d_ImporteSoles"].Value.ToString())) > decimal.Parse(grdData.Rows.Where(q => q.Cells["v_IdVenta"].Value.ToString() == IdVenta).Select(p => p.Cells["d_NetoXCobrar"].Value.ToString()).Distinct().First()))
                {
                    //UltraStatusbarManager.MarcarError(ultraStatusBar1, "La suma de los importes superan el monto por cobrar de este documento.", timer1);// REDONDEOCOBRANZA
                    //List<UltraGridCell> cells = new List<UltraGridCell>();
                    //grdData.Rows.Where(p => p.Cells["v_IdVenta"].Value.ToString() == IdVenta).ToList().ForEach(o => cells.Add(o.Cells["d_ImporteSoles"]));
                    //grdData.Selected.Cells.AddRange(cells.ToArray());
                    //grdData.Focus();
                    //grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);

                    //return false;
                }
            }



            foreach (var row in grdData.Rows)
            {
                decimal i;
                var retenciones = decimal.TryParse((row.Cells["d_MontoRetencion"].Value ?? 0).ToString(), out i) ? i : 0;
                if (retenciones > 0 && (row.Cells["v_NroRetencion"].Value == null || string.IsNullOrWhiteSpace(row.Cells["v_NroRetencion"].Value.ToString())))
                {
                    UltraStatusbarManager.MarcarError(ultraStatusBar1, "Por favor especificar el documento de retención.", timer1);
                    row.Cells["v_NroRetencion"].Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    return false;
                }
            }

            return true;
        }

        public void RecibirPedidosFacturados(List<string[]> ListaVentas)
        {
            foreach (string[] Venta in ListaVentas)
            {
                txtGlosa.Text = @"COBRANZA DEL DÍA " + DateTime.Today.ToShortDateString();
                UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["v_IdVenta"].Value = Venta[0];
                row.Cells["i_IdFormaPago"].Value = "-1";
                row.Cells["i_IdTipoDocumentoRef"].Value = "-1";
                string[] Cadena = new string[5];
                Cadena = _objCobranzaBl.DevolverNombres(Venta[0]);
                if (Cadena != null)
                {
                    row.Cells["TipoDocumento"].Value = Cadena[0];
                    row.Cells["NroDocumento"].Value = Cadena[1];
                    row.Cells["Cliente"].Value = Cadena[2];
                    row.Cells["i_IdMoneda"].Value = Cadena[3];
                    row.Cells["Moneda"].Value = Cadena[4];
                    row.Cells["d_NetoXCobrar"].Value = decimal.Parse(Math.Round(decimal.Parse(Venta[1].ToString()), 2, MidpointRounding.AwayFromZero).ToString("0.00"));
                    row.Cells["d_ImporteSoles"].Value = row.Cells["d_NetoXCobrar"].Value;
                    row.Cells["MonedaOriginal"].Value = Cadena[4];
                }
            }
        }

        public void RecibirItems(List<UltraGridRow> Filas)
        {
            foreach (UltraGridRow t in Filas)
            {

                {
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    decimal TCambio = decimal.Parse(txtTipoCambio.Text.Trim());
                    if (TCambio != 0)
                    {
                        switch (cboMoneda.Value.ToString())
                        {
                            case "1":

                                switch (t.Cells["Moneda"].Value.ToString())
                                {
                                    case "S/.":
                                        grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                                        grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                        row.Cells["i_RegistroEstado"].Value = "Agregado";
                                        row.Cells["i_RegistroTipo"].Value = "Temporal";
                                        row.Cells["i_IdTipoDocumentoRef"].Value = "-1";
                                        row.Cells["i_IdFormaPago"].Value = cboFormaPagoDetalle.Value.ToString();
                                        row.Cells["i_AplicaRetencion"].Value = "0";
                                        row.Cells["d_Redondeo"].Value = "0";
                                        row.Activate();
                                        grdData.ActiveRow.Cells["v_IdVenta"].Value = t.Cells["v_IdVenta"].Value.ToString();
                                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                                        grdData.ActiveRow.Cells["TipoDocumento"].Value = t.Cells["TipoDocumento"].Value.ToString();
                                        grdData.ActiveRow.Cells["NroDocumento"].Value = t.Cells["NroDocumento"].Value.ToString();
                                        grdData.ActiveRow.Cells["Cliente"].Value = t.Cells["NombreCliente"].Value.ToString();
                                        grdData.ActiveRow.Cells["d_NetoXCobrar"].Value = t.Cells["d_Saldo"].Value.ToString();

                                        grdData.ActiveRow.Cells["d_ImporteSoles"].Value = grdData.ActiveRow.Cells["d_NetoXCobrar"].Value.ToString();
                                        grdData.ActiveRow.Cells["Moneda"].Value = "S/.";
                                        grdData.ActiveRow.Cells["i_EsLetra"].Value = (bool)t.Cells["EsLetra"].Value ? 1 : 0;
                                        grdData.ActiveRow.Cells["MonedaOriginal"].Value = t.Cells["Moneda"].Value.ToString();
                                        grdData.ActiveRow.Cells["d_Redondeo"].Value = "0.00";
                                        break;

                                    case "US$.":
                                        grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                                        grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                        row.Cells["i_RegistroEstado"].Value = "Agregado";
                                        row.Cells["i_RegistroTipo"].Value = "Temporal";
                                        row.Cells["i_IdTipoDocumentoRef"].Value = "-1";
                                        row.Cells["i_IdFormaPago"].Value = cboFormaPagoDetalle.Value.ToString();
                                        row.Cells["i_AplicaRetencion"].Value = "0";
                                        row.Cells["d_Redondeo"].Value = "0";
                                        row.Activate();
                                        grdData.ActiveRow.Cells["v_IdVenta"].Value = t.Cells["v_IdVenta"].Value.ToString();
                                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                                        grdData.ActiveRow.Cells["TipoDocumento"].Value = t.Cells["TipoDocumento"].Value.ToString();
                                        grdData.ActiveRow.Cells["NroDocumento"].Value = t.Cells["NroDocumento"].Value.ToString();
                                        grdData.ActiveRow.Cells["Cliente"].Value = t.Cells["NombreCliente"].Value.ToString();
                                        grdData.ActiveRow.Cells["d_NetoXCobrar"].Value = Math.Round((decimal.Parse(t.Cells["d_Saldo"].Value.ToString()) * TCambio), 2, MidpointRounding.AwayFromZero).ToString("0.00");
                                        grdData.ActiveRow.Cells["d_ImporteSoles"].Value = grdData.ActiveRow.Cells["d_NetoXCobrar"].Value.ToString();
                                        grdData.ActiveRow.Cells["Moneda"].Value = "S/.";
                                        grdData.ActiveRow.Cells["i_EsLetra"].Value = (bool)t.Cells["EsLetra"].Value ? 1 : 0;
                                        grdData.ActiveRow.Cells["MonedaOriginal"].Value = t.Cells["Moneda"].Value.ToString();
                                        grdData.ActiveRow.Cells["d_Redondeo"].Value = "0.00";
                                        break;
                                }

                                break;

                            case "2":

                                switch (t.Cells["Moneda"].Value.ToString())
                                {
                                    case "S/.":
                                        grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                                        grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                        row.Cells["i_RegistroEstado"].Value = "Agregado";
                                        row.Cells["i_RegistroTipo"].Value = "Temporal";
                                        row.Cells["i_IdTipoDocumentoRef"].Value = "-1";
                                        row.Cells["i_IdFormaPago"].Value = cboFormaPagoDetalle.Value.ToString();
                                        row.Cells["i_AplicaRetencion"].Value = "0";
                                        row.Cells["d_Redondeo"].Value = "0";
                                        row.Activate();
                                        grdData.ActiveRow.Cells["v_IdVenta"].Value = t.Cells["v_IdVenta"].Value.ToString();
                                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                                        grdData.ActiveRow.Cells["TipoDocumento"].Value = t.Cells["TipoDocumento"].Value.ToString();
                                        grdData.ActiveRow.Cells["NroDocumento"].Value = t.Cells["NroDocumento"].Value.ToString();
                                        grdData.ActiveRow.Cells["Cliente"].Value = t.Cells["NombreCliente"].Value.ToString();
                                        grdData.ActiveRow.Cells["d_NetoXCobrar"].Value = Math.Round((decimal.Parse(t.Cells["d_Saldo"].Value.ToString()) / TCambio), 2, MidpointRounding.AwayFromZero).ToString("0.00");
                                        grdData.ActiveRow.Cells["Moneda"].Value = "US$.";
                                        grdData.ActiveRow.Cells["i_EsLetra"].Value = (bool)t.Cells["EsLetra"].Value ? 1 : 0;
                                        grdData.ActiveRow.Cells["MonedaOriginal"].Value = t.Cells["Moneda"].Value.ToString();
                                        grdData.ActiveRow.Cells["d_ImporteSoles"].Value = grdData.ActiveRow.Cells["d_NetoXCobrar"].Value.ToString();
                                        grdData.ActiveRow.Cells["d_Redondeo"].Value = "0.00";
                                        break;

                                    case "US$.":
                                        grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                                        grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                        row.Cells["i_RegistroEstado"].Value = "Agregado";
                                        row.Cells["i_RegistroTipo"].Value = "Temporal";
                                        row.Cells["i_IdTipoDocumentoRef"].Value = "-1";
                                        row.Cells["i_IdFormaPago"].Value = cboFormaPagoDetalle.Value.ToString();
                                        row.Cells["i_AplicaRetencion"].Value = "0";
                                        row.Cells["d_Redondeo"].Value = "0";
                                        row.Activate();
                                        grdData.ActiveRow.Cells["v_IdVenta"].Value = t.Cells["v_IdVenta"].Value.ToString();
                                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                                        grdData.ActiveRow.Cells["TipoDocumento"].Value = t.Cells["TipoDocumento"].Value.ToString();
                                        grdData.ActiveRow.Cells["NroDocumento"].Value = t.Cells["NroDocumento"].Value.ToString();
                                        grdData.ActiveRow.Cells["Cliente"].Value = t.Cells["NombreCliente"].Value.ToString();
                                        grdData.ActiveRow.Cells["d_NetoXCobrar"].Value = t.Cells["d_Saldo"].Value.ToString();
                                        grdData.ActiveRow.Cells["Moneda"].Value = "US$.";
                                        grdData.ActiveRow.Cells["i_EsLetra"].Value = (bool)t.Cells["EsLetra"].Value ? 1 : 0;
                                        grdData.ActiveRow.Cells["MonedaOriginal"].Value = t.Cells["Moneda"].Value.ToString();
                                        grdData.ActiveRow.Cells["d_ImporteSoles"].Value = grdData.ActiveRow.Cells["d_NetoXCobrar"].Value.ToString();
                                        grdData.ActiveRow.Cells["d_Redondeo"].Value = "0.00";
                                        break;
                                }

                                break;

                        }

                    }
                    else
                    {
                        UltraMessageBox.Show("Por favor, ingrese un tipo de cambio válido", "Error de validación");
                    }
                }
            }

            CalcularValoresDetalle();
        }
        #endregion

        #region Eventos de la Grilla
        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdTipoDocumentoRef"].EditorComponent = _ucTipoDocumento;
            e.Layout.Bands[0].Columns["i_IdTipoDocumentoRef"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdFormaPago"].EditorComponent = _ucFormasPago;
            e.Layout.Bands[0].Columns["i_IdFormaPago"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {

            if (grdData.ActiveCell == null) return;
            if (grdData.ActiveCell.Column.Key == "d_ImporteSoles")
            {
                if (grdData.ActiveRow.Cells["i_IdTipoDocumentoRef"].Value.ToString() == "7")
                {
                    //e.SuppressKeyPress = true;
                }
            }
            switch (grdData.ActiveCell.Column.Key)
            {

                case "d_Redondeo":
                    //if ((grdData.ActiveRow.Cells["i_AplicaRetencion"].Value != null && grdData.ActiveRow.Cells["i_AplicaRetencion"].Value.ToString() == "1") || (grdData.ActiveRow.Cells["i_EsLetra"].Value != null && grdData.ActiveRow.Cells["i_EsLetra"].Value.ToString()=="1"))
                    //{
                    //    e.SuppressKeyPress = true;
                    //}
                    //break;
                    if ((grdData.ActiveRow.Cells["i_AplicaRetencion"].Value != null && grdData.ActiveRow.Cells["i_AplicaRetencion"].Value.ToString() == "1"))
                    {
                        e.SuppressKeyPress = true;
                    }
                    break;

            }




            if (grdData.ActiveCell.Column.Key != "i_IdTipoDocumentoRef")
            {
                switch (e.KeyCode)
                {
                    case Keys.Up:
                        grdData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdData.PerformAction(UltraGridAction.AboveCell, false, false);
                        e.Handled = true;
                        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Down:
                        grdData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdData.PerformAction(UltraGridAction.BelowCell, false, false);
                        e.Handled = true;
                        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Right:
                        grdData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdData.PerformAction(UltraGridAction.NextCellByTab, false, false);
                        e.Handled = true;
                        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Left:
                        grdData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdData.PerformAction(UltraGridAction.PrevCellByTab, false, false);
                        e.Handled = true;
                        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Right:
                        grdData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdData.PerformAction(UltraGridAction.NextCellByTab, false, false);
                        e.Handled = true;
                        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                    case Keys.Left:
                        grdData.PerformAction(UltraGridAction.ExitEditMode, false, false);
                        grdData.PerformAction(UltraGridAction.PrevCellByTab, false, false);
                        e.Handled = true;
                        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        break;
                }
            }
        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            try
            {
                grdData.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                if (!e.Cell.Column.Key.Equals("i_AplicaRetencion")) return;
                if (e.Cell.EditorResolved == null) return;
                int i;
                var editor = e.Cell.EditorResolved.Value;
                if (!int.TryParse(editor.ToString(), out i)) return;
                grdData.DisplayLayout.Bands[0].Columns["d_MontoRetencion"].Hidden = i != 1;
                grdData.DisplayLayout.Bands[0].Columns["v_NroRetencion"].Hidden = i != 1;
                grdData.ActiveRow.Cells["d_Redondeo"].Activation = i == 1 ? Activation.NoEdit : Activation.AllowEdit;
                grdData.ActiveRow.Cells["d_Redondeo"].Value = "0.00";
                grdData.ActiveRow.Cells["d_MontoRetencion"].Activate();
                grdData.PerformAction(UltraGridAction.EnterEditMode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grdData_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (grdData.ActiveCell != null)
            {
                switch (grdData.ActiveCell.Column.Key)
                {
                    case "d_NetoXCobrar":
                        var celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(celda, e);
                        break;


                    case "d_Redondeo":

                        var Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalNegativoCelda(Celda, e);
                        break;

                }
            }
        }

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            if (grdData.ActiveCell == null) return;

            string IdTipoDocRef = grdData.ActiveRow.Cells["i_IdTipoDocumentoRef"].Value.ToString();

            if (grdData.ActiveCell != null && grdData.ActiveCell.Column.Key == "d_ImporteSoles")
                txtTotalSoles.Text = grdData.Rows.Where(p => p.Cells["d_ImporteSoles"].Value != null)
                        .Sum(c => decimal.Parse(c.Cells["d_ImporteSoles"].Value.ToString()))
                        .ToString("0.00");


            if (grdData.ActiveCell != null && grdData.ActiveCell.Column.Key == "d_Redondeo")
            {
                //decimal neto = grdData.ActiveRow.Cells["d_NetoXCobrar"].Value == null ? 0 : decimal.Parse(grdData.ActiveRow.Cells["d_NetoXCobrar"].Value.ToString());
                //decimal importe = grdData.ActiveRow.Cells["d_ImporteSoles"].Value == null ? 0 : decimal.Parse(grdData.ActiveRow.Cells["d_ImporteSoles"].Value.ToString());
                //decimal redondeo = grdData.ActiveRow.Cells["d_Redondeo"].Value == null ? 0 : decimal.Parse(grdData.ActiveRow.Cells["d_Redondeo"].Value.ToString());
                //grdData.ActiveRow.Cells["Saldo"].Value = neto - importe + redondeo;
            }


            if (IdTipoDocRef != "-1")
            {
                if (grdData.ActiveCell.Column.Key == "v_DocumentoRef")
                {
                    if (grdData.ActiveCell.Text.Contains("-"))
                    {
                        decimal Importe;

                        switch (IdTipoDocRef.Trim())
                        {
                            case "7":
                                int i;
                                var SerieCorrelativo = grdData.ActiveCell.Text.Split('-');
                                var serie = int.TryParse(SerieCorrelativo[0], out i) ? i.ToString("0000") : SerieCorrelativo[0];
                                var correlativo = int.TryParse(SerieCorrelativo[1], out i) ? i.ToString("00000000") : "No formated";
                                grdData.ActiveCell.Value = string.Format("{0}-{1}", serie.ToUpper(), correlativo.ToUpper());
                                Importe = _objCobranzaBl.DevuelveMontoNotaCredito(grdData.ActiveCell.Text.Trim());
                                if (Importe == 0)
                                {
                                    UltraMessageBox.Show("El documento no existe o ya fue usado en una cobranza", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                                    grdData.ActiveRow.Cells["v_DocumentoRef"].Value = "";
                                    return;
                                }
                                grdData.ActiveRow.Cells["d_ImporteSoles"].Value = Importe.ToString();
                                break;

                            case "433":
                                var serieCorrelativo = grdData.ActiveCell.Text.Split('-');
                                if (serieCorrelativo.Length == 2)
                                {
                                    var nroDocumento = int.Parse(serieCorrelativo[0]).ToString("0000") + "-" + int.Parse(serieCorrelativo[1]).ToString("00000000");
                                    grdData.ActiveCell.Value = nroDocumento;

                                    Importe = _objCobranzaBl.DevuelveSaldoAdelanto(nroDocumento);
                                    if (Importe == 0)
                                    {
                                        UltraMessageBox.Show("El documento no existe o ya fue usado en una cobranza", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                                        grdData.ActiveRow.Cells["v_DocumentoRef"].Value = "";
                                        return;
                                    }
                                    grdData.ActiveRow.Cells["d_ImporteSoles"].Value = Importe.ToString();
                                    grdData.ActiveRow.Cells["_MaxValor"].Value = Importe.ToString();
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void grdData_BeforeCellUpdate(object sender, BeforeCellUpdateEventArgs e)
        {
            if (e.Cell.Value == null) return;

            if (grdData.ActiveCell == null) return;

            if (grdData.ActiveCell.Column.Key != "d_ImporteSoles") return;
            if (grdData.ActiveRow.Cells["i_IdTipoDocumentoRef"].Value.ToString() == "433")
            {
                if (grdData.ActiveRow.Cells["_MaxValor"].Value == null) return;
                if (decimal.Parse(e.NewValue.ToString()) > decimal.Parse(grdData.ActiveRow.Cells["_MaxValor"].Value.ToString()))
                {
                    //UltraMessageBox.Show("No se puede actualizar el importe a un valor superior.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk); REDONDEOCOBRANZA
                    //e.Cancel = true; 
                }
            }
            else
            {
                if (grdData.ActiveRow.Cells["Moneda"].Value.ToString() !=
                    grdData.ActiveRow.Cells["MonedaOriginal"].Value.ToString() ||
                    Globals.ClientSession.i_RedondearVentas != 1) return;
                if (decimal.Parse(e.NewValue.ToString()) > decimal.Parse(grdData.ActiveRow.Cells["d_NetoXCobrar"].Value.ToString()))
                {
                    //UltraMessageBox.Show("No se puede actualizar el importe a un valor superior.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);REDONDEOCOBRANZA
                    //e.Cancel = true;
                }
            }
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row == null)
            {
                btnEliminarDetalle.Enabled = false;
            }
            else
            {
                btnEliminarDetalle.Enabled = true;
            }
        }
        private void grdData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "i_IdFormaPago")
            {
                var xx = (List<GridKeyValueDTO>)_ucFormasPago.DataSource;

                if (xx != null)
                {
                    var doc = xx.FirstOrDefault(p => p.Id == e.Cell.Value.ToString());
                    if (doc == null) return;
                    grdData.ActiveRow.Cells["i_IdTipoDocumentoRef"].Value = doc.Value3 != null && !string.IsNullOrEmpty(doc.Value3.ToString()) ? int.Parse(doc.Value3.ToString()) : -1;
                }
            }
        }

        private void grdData_ClickCellButton(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "")
            {

            }
            switch (e.Cell.Column.Key)
            {
                case "Saldo":
                    // if ((grdData.ActiveRow.Cells["i_AplicaRetencion"].Value != null && grdData.ActiveRow.Cells["i_AplicaRetencion"].Value.ToString() == "0") &&grdData.ActiveRow.Cells["i_EsLetra"].Value != null && grdData.ActiveRow.Cells["i_EsLetra"].Value.ToString ()=="0" )
                    // {
                    //     if(grdData.ActiveRow.Cells["Saldo"].Value  == null || grdData.ActiveRow.Cells["Saldo"].Value.ToString() == "0" ) return;
                    //     grdData.ActiveRow.Cells["d_Redondeo"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(grdData.ActiveRow.Cells["Saldo"].Value.ToString()), 2);
                    //}

                    if ((grdData.ActiveRow.Cells["i_AplicaRetencion"].Value != null && grdData.ActiveRow.Cells["i_AplicaRetencion"].Value.ToString() == "0"))
                    {
                        if (grdData.ActiveRow.Cells["Saldo"].Value == null || grdData.ActiveRow.Cells["Saldo"].Value.ToString() == "0") return;
                        grdData.ActiveRow.Cells["d_Redondeo"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(grdData.ActiveRow.Cells["Saldo"].Value.ToString()), 2);
                    }

                    break;
            }
        }

        private void grdData_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (e.Cell.Column.Key == "d_ImporteSoles")
            {
                if (grdData.ActiveRow != null && grdData.ActiveRow.Cells["i_IdTipoDocumentoRef"].Value.ToString() == "7")
                {
                    //e.Cell.CancelUpdate();
                }
            }
        }
        #endregion

        private void frmCobranza_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (BtnImprimir.Enabled && _listaVentas != null && _listaVentas.Any() && !_documentoImpreso)
                e.Cancel =
                    MessageBox.Show(@"¿Seguro de salir sin imprimir el voucher?", @"Confirmación", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No;

            if (Application.OpenForms["frmBandejaPedidosFacturados"] == null) return;
            var bandejaPendidosFacturados = (frmBandejaPedidosFacturados)Application.OpenForms["frmBandejaPedidosFacturados"];
            bandejaPendidosFacturados.Close();
        }

        private void btnEliminarCobranza_Click(object sender, EventArgs e)
        {
            OperationResult _objOperationResult = new OperationResult();
            if (UltraMessageBox.Show("¿Está seguro de Eliminar esta cobranza  de los registros?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                {
                    string pstrIdCobranza = _cobranzaDto.v_IdCobranza;
                    _objCobranzaBl.EliminarCobranza(ref _objOperationResult, pstrIdCobranza, Globals.ClientSession.GetAsList());
                    if (_objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show(_objOperationResult.ErrorMessage + "\n\n" + _objOperationResult.ExceptionMessage + "\n\nTARGET: " + _objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        #region Requerimientos para Notaria Becerra Sosaya

                        if (Globals.ClientSession.v_RucEmpresa.Equals(Constants.RucNotariaBecerrSosaya))
                        {
                            var objDbfSincro = new DbfSincronizador();
                            var objOperationResult2 = new OperationResult();
                            objDbfSincro.RutaDbfCabecera = NBS_DBF_PathSettings.Default.dbfSincro_Cabecera;
                            objDbfSincro.RutaDbfDetalle = NBS_DBF_PathSettings.Default.dbfSincro_Detalle;

                            objDbfSincro.EliminarCobranza(ref objOperationResult2, pstrIdCobranza);

                            if (objOperationResult2.Success == 0)
                            {
                                MessageBox.Show(objOperationResult2.ErrorMessage,
                                    @"Error al sincronizar DBF", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        #endregion
                    }
                    Close();
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboEstado_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnMarcarTodos_Click(object sender, EventArgs e)
        {
            if (ValidarFormaPagoDetalle.Validate(true, false).IsValid)
            {
                foreach (var fila in grdData.Rows)
                {
                    // if (cboFormaPagoDetalle.Value != null && cboFormaPagoDetalle.Value.ToString() != "-1" && (fila.Cells["i_IdFormaPago"].Value == null || fila.Cells["i_IdFormaPago"].Value.ToString() == "-1"))
                    if (cboFormaPagoDetalle.Value != null && cboFormaPagoDetalle.Value.ToString() != "-1")
                    {
                        fila.Cells["i_RegistroEstado"].Value = "Modificado";
                        fila.Cells["i_IdFormaPago"].Value = cboFormaPagoDetalle.Value.ToString();
                        var xx = (List<GridKeyValueDTO>)_ucFormasPago.DataSource;
                        if (xx != null)
                        {
                            var doc = xx.FirstOrDefault(p => p.Id == fila.Cells["i_IdFormaPago"].Value.ToString());
                            if (doc == null) return;
                            fila.Cells["i_IdTipoDocumentoRef"].Value = doc.Value3 != null && !string.IsNullOrEmpty(doc.Value3.ToString()) ? int.Parse(doc.Value3.ToString()) : -1;
                        }
                    }

                    //UltraMessageBox.Show("Datos Actualizados", "Sistema");
                }
            }
        }

        private void txtGlosa_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.CaracteresValidos((sender as Infragistics.Win.UltraWinEditors.UltraTextEditor), e);
        }


    }
}
