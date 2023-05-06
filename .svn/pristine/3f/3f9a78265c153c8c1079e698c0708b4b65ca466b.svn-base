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
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Security.BL;
using SAMBHS.Contabilidad.BL;
using LoadingClass;
using System.Globalization;
using SAMBHS.Common.DataModel;
namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmTransferencia : Form
    {
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        CierreMensualBL _objCierreMensualBL = new CierreMensualBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        movimientoDto _movimientoDto = new movimientoDto();
        movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
        List<KeyValueDTO> _ListadoMovimientos = new List<KeyValueDTO>();
        List<KeyValueDTO> _ListadoMovimientosCambioFecha = new List<KeyValueDTO>();
        UltraCombo ucUnidadMedida = new UltraCombo();
        UltraCombo ucTipoDocumento = new UltraCombo();
        SecurityBL _obSecurityBL = new SecurityBL();
        PedidoBL _objPedidoBL = new PedidoBL();
        int _MaxV, _ActV;
        string _Mode;
        public string _pstrIdMovimiento_Nuevo, Agregado = string.Empty, Mensaje = string.Empty, MENSAJE = "La lista de Producto(s) tienen la cantidad ingresada superior al stock de almacen: \n";
        string strModo, strIdmovimiento;
        public movimientoDto objMovimientoDto = new movimientoDto();
        bool _btnGuardar, _btnImprimir;
        #region Temporales DetalleVenta
        List<movimientodetalleDto> _TempDetalle_AgregarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> _TempDetalle_ModificarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> _TempDetalle_EliminarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> _TempDetalle_ModificarDocumentosDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> _TempDetalle_AgregarDocumentosDto = new List<movimientodetalleDto>();
        #endregion

        public frmTransferencia(string Modo, string IdMovimiento)
        {
            strModo = Modo;
            strIdmovimiento = IdMovimiento;
            InitializeComponent();
        }
        private void frmTransferencia_Load(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            txtNroRegSerie.Text = DateTime.Now.Month.ToString();
            Utils.Windows.FijarFormatoUltraTextBox(txtNroRegSerie, "{0:00}");
            #region ControlAcciones
            if (_objCierreMensualBL.VerificarMesCerrado(Globals.ClientSession.i_Periodo.ToString(), DateTime.Now.Month.ToString("00"), (int)ModulosSistema.Almacen))
            {
                btnGuardar.Visible = false;
                this.Text = "Transferencia entre Almacenes [MES CERRADO]";
            }
            else
            {
                btnGuardar.Visible = true;
                this.Text = "Transferencia entre Almacenes";
            }
            var _formActions = _obSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmTransferencia", Globals.ClientSession.i_RoleId);
            _btnGuardar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmTransferencia_Save", _formActions);
            _btnImprimir = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmTransferencia_Print", _formActions);
            btnGuardar.Enabled = _btnGuardar;
            btnImprimir.Enabled = false;
            btnEliminar.Enabled = false;
            #endregion
            CargarCombos();
            CargarCombosDetalle();
            ObtenerListadoMovimientos(txtPeriodo.Text, txtNroRegSerie.Text);
            FormatoDecimalesGrilla(Globals.ClientSession.i_CantidadDecimales ?? 2, Globals.ClientSession.i_PrecioDecimales ?? 2);
            FormatoDecimalesTotales(Globals.ClientSession.i_CantidadDecimales ?? 2, Globals.ClientSession.i_PrecioDecimales ?? 2);
            ValidarFechas();
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();
            if (Application.OpenForms["frmMaster"] != null) ((frmMaster)Application.OpenForms["frmMaster"]).Activate();
            ConfigurarGrilla();
            btnMigracion.Visible = Globals.ClientSession.i_SystemUserId.Equals(1);
            btnRegenerarTransferencia.Visible = Globals.ClientSession.i_SystemUserId.Equals(1);
        }

        private void ConfigurarGrilla()
        {
            grdData.DisplayLayout.Bands[0].Columns["v_NroPedido"].Hidden =
            Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta != 1;
        }
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboMotivo, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 22, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacenOrigen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            cboAlmacenOrigen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString();
            Utils.Windows.LoadUltraComboEditorList(cboAlmacenDestino, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaComboAll(ref objOperationResult, "i_IdAlmacen!=" + cboAlmacenOrigen.Value.ToString()), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboTipoDocumento, "Value2", "Id", _objDocumentoBL.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 1, 0), DropDownListAction.Select);
            cboAlmacenOrigen.Enabled = false;
            cboTipoDocumento.Value = "-1";
        }

        #region CRUD
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _movimientodetalleDto = new movimientodetalleDto();
                    _movimientodetalleDto.v_IdMovimientoDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdMovimientoDetalle"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(_movimientodetalleDto);
                    grdData.Rows[grdData.ActiveRow.Index].Delete(false);
                }
            }
            else
            {
                grdData.Rows[grdData.ActiveRow.Index].Delete(false);
            }
        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (ValidarAlmacen.Validate(true, false).IsValid)
            {
                var ultimaFila = grdData.Rows.LastOrDefault();
                if (ultimaFila == null || ultimaFila.Cells["v_IdProductoDetalle"].Value != null)
                {
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Agregado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_IdUnidad"].Value = "-1";
                    row.Cells["i_IdTipoDocumento"].Value = "-1";
                    row.Cells["d_Cantidad"].Value = "1";
                    row.Cells["d_Precio"].Value = "0";
                    row.Cells["i_IdTipoDocumento"].Value = cboTipoDocumento.Value == null ? "-1" : cboTipoDocumento.Value.ToString();
                    row.Cells["v_NumeroDocumento"].Value = !string.IsNullOrEmpty(txtSerieDoc.Text.Trim()) && !string.IsNullOrEmpty(txtNroDoc.Text.Trim()) ? txtSerieDoc.Text.Trim() + "-" + txtNroDoc.Text.Trim() : "";
                    row.Cells["v_NroGuiaRemision"].Value = !string.IsNullOrEmpty(txtSerieDoc.Text.Trim()) && !string.IsNullOrEmpty(txtNroDoc.Text.Trim()) ? cboTipoDocumento.Value != null && int.Parse(cboTipoDocumento.Value.ToString()) == (int)TiposDocumentos.GuiaRemision ? txtSerieDoc.Text.Trim() + "-" + txtNroDoc.Text.Trim() : "":"";
                }
               
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_CodigoInterno"];
                this.grdData.ActiveCell = aCell;
                grdData.Focus();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                Agregado = "Agregado";

            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvDatos.Validate(true, false).IsValid)
            {
                if (grdData.Rows.Count() == 0)
                {
                    UltraMessageBox.Show("No se permite guardar mientras el detalle está vacío", "Error de Validación", Icono: MessageBoxIcon.Error);
                    return;
                }

                if (txtTipoCambio.Text == "")
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", Icono: MessageBoxIcon.Error);
                    txtTipoCambio.Focus();
                    return;

                }
                else if (decimal.Parse(txtTipoCambio.Text) <= 0)
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", Icono: MessageBoxIcon.Error);
                    txtTipoCambio.Focus();
                    return;

                }
                if (cboAlmacenOrigen.Value.ToString() == cboAlmacenDestino.Value.ToString())
                {
                    UltraMessageBox.Show("El almacén Origen y Almacén Destino no pueden ser el mismo ", "Error de Validación", Icono: MessageBoxIcon.Error);
                    cboAlmacenDestino.Focus();
                    return;

                }

                if (ValidaCamposNulosVacios())
                {
                    // CalcularTotal();
                    if (_Mode == "New")
                    {
                        Mensaje = VerificarStockProductoAlmacen();
                        if (Mensaje != "")
                        {
                            UltraMessageBox.Show(MENSAJE + Mensaje, "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else
                        {
                            while (_objMovimientoBL.ExisteNroRegistro(txtPeriodo.Text, txtNroRegSerie.Text, txtNroRegCorrelativo.Text, (int)TipoDeMovimiento.Transferencia) == false)
                            {

                                txtNroRegCorrelativo.Text = (int.Parse(txtNroRegCorrelativo.Text) + 1).ToString("00000000");
                            }
                            _movimientoDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                            _movimientoDto.i_IdAlmacenOrigen = int.Parse(cboAlmacenOrigen.Value.ToString());
                            _movimientoDto.i_IdAlmacenDestino = int.Parse(cboAlmacenDestino.Value.ToString());
                            _movimientoDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                            _movimientoDto.i_IdTipoMotivo = int.Parse(cboMotivo.Value.ToString());
                            _movimientoDto.t_Fecha = dtpFecha.Value;
                            _movimientoDto.v_Glosa = txtGlosa.Text.Trim();
                            _movimientoDto.v_Mes = txtNroRegSerie.Text.Trim();
                            _movimientoDto.v_Periodo = txtPeriodo.Text.Trim();
                            _movimientoDto.i_IdTipoMovimiento = (int)Common.Resource.TipoDeMovimiento.Transferencia;
                            _movimientoDto.v_Correlativo = txtNroRegCorrelativo.Text;
                            _movimientoDto.d_TotalCantidad = txtCantidad.Text == "" ? 0 : decimal.Parse(txtCantidad.Text);
                            _movimientoDto.d_TotalPrecio = txtTotal.Text == "" ? 0 : decimal.Parse(txtTotal.Text);
                            _movimientoDto.i_EsDevolucion = 0;
                            _movimientoDto.i_IdEstablecimiento = int.Parse(Globals.ClientSession.i_IdEstablecimiento.Value.ToString());
                            _movimientoDto.i_IdTipoDocumento = cboTipoDocumento.Value != null ? int.Parse(cboTipoDocumento.Value.ToString()) : -1;
                            _movimientoDto.v_SerieDocumento = txtSerieDoc.Text;
                            _movimientoDto.v_CorrelativoDocumento = txtNroDoc.Text;
                           
                            LlenarTemporales();
                            _objMovimientoBL.InsertarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto);
                        }
                    }
                    else if (_Mode == "Edit")
                    {
                        Mensaje = VerificarStockProductoAlmacenEditado();

                        if (Mensaje != "")
                        {
                            UltraMessageBox.Show(MENSAJE + Mensaje, "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error); return;
                        }
                        else
                        {
                            _movimientoDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                            _movimientoDto.i_IdAlmacenOrigen = int.Parse(cboAlmacenOrigen.Value.ToString());
                            _movimientoDto.i_IdAlmacenDestino = int.Parse(cboAlmacenDestino.Value.ToString());
                            _movimientoDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                            _movimientoDto.i_IdTipoMotivo = int.Parse(cboMotivo.Value.ToString());
                            _movimientoDto.t_Fecha = dtpFecha.Value.Date;
                            _movimientoDto.v_Glosa = txtGlosa.Text.Trim();
                            _movimientoDto.v_Mes = txtNroRegSerie.Text.Trim();
                            _movimientoDto.v_Periodo = txtPeriodo.Text.Trim();
                            _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.Transferencia;
                            _movimientoDto.v_Correlativo = txtNroRegCorrelativo.Text;
                            _movimientoDto.d_TotalCantidad = txtCantidad.Text == "" ? 0 : decimal.Parse(txtCantidad.Text);
                            _movimientoDto.d_TotalPrecio = txtTotal.Text == "" ? 0 : decimal.Parse(txtTotal.Text);
                            _movimientoDto.i_IdEstablecimiento = int.Parse(Globals.ClientSession.i_IdEstablecimiento.Value.ToString());

                            _movimientoDto.i_IdTipoDocumento = cboTipoDocumento.Value != null ? int.Parse(cboTipoDocumento.Value.ToString()) : -1;
                            _movimientoDto.v_SerieDocumento = txtSerieDoc.Text;
                            _movimientoDto.v_CorrelativoDocumento = txtNroDoc.Text;
                            LlenarTemporales();
                            _objMovimientoBL.ActualizarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto);
                            objMovimientoDto = _movimientoDto;
                        }

                    }
                    if (objOperationResult.Success == 1)
                    {
                        strModo = "Guardado";
                        EdicionBarraNavegacion(true);
                        _pstrIdMovimiento_Nuevo = strIdmovimiento;
                        ObtenerListadoMovimientos(txtPeriodo.Text, txtNroRegSerie.Text);
                        UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Agregado = string.Empty;
                        btnImprimir.Enabled = _btnImprimir;
                    }
                    else
                    {
                        UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    _TempDetalle_AgregarDto = new List<movimientodetalleDto>();
                    _TempDetalle_ModificarDto = new List<movimientodetalleDto>();
                    _TempDetalle_EliminarDto = new List<movimientodetalleDto>();
                }
            }
        }
        #endregion

        #region ComportamientoControles
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string TipoCambio = _objMovimientoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFecha.Value.Date);

            if (TipoCambio == "0.0000")
            {
                txtTipoCambio.Text = string.Empty;
            }

            txtTipoCambio.Text = TipoCambio;

            if (strModo == "Nuevo")
            {
                GenerarNumeroRegistro();
            }

            else
            {
                string MesCambiado = int.Parse(dtpFecha.Value.Month.ToString()) <= 9 ? ("0" + dtpFecha.Value.Month.ToString()).Trim() : dtpFecha.Value.Month.ToString();
                string AnioCambiado = dtpFecha.Value.Year.ToString().Trim();
                if (MesCambiado.Trim() != _movimientoDto.v_Mes.Trim() || AnioCambiado != _movimientoDto.v_Periodo.Trim())
                {
                    GenerarNumeroRegistro();

                }
                else
                {
                    txtPeriodo.Text = _movimientoDto.v_Periodo.Trim();
                    txtNroRegSerie.Text = _movimientoDto.v_Mes.Trim();
                    txtNroRegCorrelativo.Text = _movimientoDto.v_Correlativo.Trim();


                }
            }

            if (_objCierreMensualBL.VerificarMesCerrado(txtPeriodo.Text.Trim(), txtNroRegSerie.Text.Trim(), (int)ModulosSistema.Almacen))
            {
                btnGuardar.Visible = false;
                this.Text = "Transferencia entre Almacenes [MES CERRADO]";
            }
            else
            {
                btnGuardar.Visible = true;
                this.Text = "Transferencia entre Almacenes";

            };
        }
        private void txtTipoCambio_ValueChanged(object sender, EventArgs e)
        {

        }
        #endregion

        #region Barra de Navegación
        private void ObtenerListadoMovimientos(string pstrPeriodo, string pstrMes)
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoMovimientos = _objMovimientoBL.ObtenerListadoMovimientos(ref objOperationResult, pstrPeriodo, pstrMes, (int)Common.Resource.TipoDeMovimiento.Transferencia);
            switch (strModo)
            {
                case "Edicion":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdmovimiento);
                    cboAlmacenOrigen.Enabled = false;
                    cboAlmacenDestino.Enabled = false;
                    btnImprimir.Enabled = _btnImprimir;
                    break;

                case "Nuevo":
                    if (_ListadoMovimientos.Count != 0)
                    {
                        _MaxV = _ListadoMovimientos.Count() - 1;
                        _ActV = _MaxV;
                        LimpiarCabecera();
                        CargarDetalle("");
                        txtNroRegCorrelativo.Text = (int.Parse(_ListadoMovimientos[_MaxV].Value1) + 1).ToString("00000000");
                        _Mode = "New";
                        _movimientoDto = new movimientoDto();
                        //EdicionBarraNavegacion(true);
                    }
                    else
                    {
                        txtNroRegCorrelativo.Text = "00000001";
                        _Mode = "New";
                        LimpiarCabecera();
                        CargarDetalle("");
                        _MaxV = 1;
                        _ActV = 1;
                        _movimientoDto = new movimientoDto();
                        btnNuevoMovimiento.Enabled = false;
                        //EdicionBarraNavegacion(false);
                    }
                    txtNroRegSerie.Enabled = true;
                    txtTipoCambio.Text = _objMovimientoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFecha.Value.Date);
                    EdicionBarraNavegacion(false);
                    break;

                case "Guardado":
                    _MaxV = _ListadoMovimientos.Count() - 1;
                    _ActV = _MaxV;
                    if (strIdmovimiento == "" | strIdmovimiento == null)
                    {
                        CargarCabecera(_ListadoMovimientos[_MaxV].Value2);
                    }
                    else
                    {
                        CargarCabecera(strIdmovimiento);
                    }
                    btnNuevoMovimiento.Enabled = true;
                    EdicionBarraNavegacion(false);
                    cboAlmacenOrigen.Enabled = false;
                    cboAlmacenDestino.Enabled = false;
                    btnImprimir.Enabled = _btnImprimir;
                    break;

                case "Consulta":
                    if (_ListadoMovimientos.Count != 0)
                    {
                        _MaxV = _ListadoMovimientos.Count() - 1;
                        _ActV = _MaxV;
                        txtNroRegCorrelativo.Text = (int.Parse(_ListadoMovimientos[_MaxV].Value1)).ToString("00000000");
                        CargarCabecera(_ListadoMovimientos[_MaxV].Value2);
                        _Mode = "Edit";
                        EdicionBarraNavegacion(true);
                    }
                    else
                    {
                        txtNroRegCorrelativo.Text = "00000001";
                        _Mode = "New";
                        LimpiarCabecera();
                        CargarDetalle("");
                        _MaxV = 1;
                        _ActV = 1;
                        _movimientoDto = new movimientoDto();
                        btnNuevoMovimiento.Enabled = false;
                        txtTipoCambio.Text = _objMovimientoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFecha.Value.Date);
                        EdicionBarraNavegacion(false);
                        txtNroRegSerie.Enabled = true;
                    }
                    EdicionBarraNavegacion(false);
                    cboAlmacenOrigen.Enabled = false;
                    cboAlmacenDestino.Enabled = false;
                    btnImprimir.Enabled = _btnImprimir;
                    break;
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {

        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {

        }

        private void btnNuevoMovimiento_Click(object sender, EventArgs e)
        {

        }

        private void txtNroRegCorrelativo_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtNroRegSerie_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtNroRegSerie_Leave(object sender, EventArgs e)
        {

        }

        private void txtNroRegSerie_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtNroRegSerie, e);
        }
        #endregion

        #region Clases / Validaciones

        private void GenerarNumeroRegistro()
        {
            OperationResult objOperationResult = new OperationResult();
            string Mes;
            Mes = int.Parse(dtpFecha.Value.Month.ToString()) <= 9 ? ("0" + dtpFecha.Value.Month.ToString()).Trim() : dtpFecha.Value.Month.ToString();
            _ListadoMovimientosCambioFecha = _objMovimientoBL.ObtenerListadoMovimientos(ref objOperationResult, txtPeriodo.Text.Trim(), Mes, (int)Common.Resource.TipoDeMovimiento.Transferencia);
            if (_ListadoMovimientosCambioFecha.Count != 0)
            {
                int MaxMovimiento;
                MaxMovimiento = _ListadoMovimientosCambioFecha.Count() > 0 ? int.Parse(_ListadoMovimientosCambioFecha[_ListadoMovimientosCambioFecha.Count() - 1].Value1.ToString()) : 0;
                MaxMovimiento++;
                txtNroRegCorrelativo.Text = MaxMovimiento.ToString("00000000");
                txtNroRegSerie.Text = int.Parse(dtpFecha.Value.Month.ToString()) <= 9 ? 0 + dtpFecha.Value.Month.ToString() : dtpFecha.Value.Month.ToString();
                txtPeriodo.Text = dtpFecha.Value.Year.ToString();

            }
            else
            {
                txtNroRegCorrelativo.Text = "00000001";
                txtNroRegSerie.Text = int.Parse(dtpFecha.Value.Month.ToString()) <= 9 ? 0 + dtpFecha.Value.Month.ToString() : dtpFecha.Value.Month.ToString();
                txtPeriodo.Text = dtpFecha.Value.Year.ToString();
            }

        }

        private void ValidarFechas()
        {

            if (DateTime.Now.Year.ToString().Trim() == txtPeriodo.Text.Trim())
            {
                if (strModo == "Nuevo")
                {
                    dtpFecha.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtNroRegSerie.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFecha.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFecha.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());

                }
                else
                {
                    if (int.Parse(_movimientoDto.v_Mes.Trim()) <= int.Parse(DateTime.Now.Month.ToString()))
                    {
                        dtpFecha.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString());


                    }

                    dtpFecha.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                }


            }
            else
            {
                if (strModo == "Nuevo")
                {
                    dtpFecha.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtNroRegSerie.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFecha.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFecha.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());

                }
                else
                {

                    dtpFecha.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/01").ToString());
                    dtpFecha.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());

                }

            }
        }

        private void FormatoDecimalesTotales(int DecimalesCantidad, int DecimalesPrecio)
        {


            string FormatoCantidad;

            if (DecimalesCantidad > 0)
            {
                string sharp = "0";
                FormatoCantidad = "0.";
                for (int i = 0; i < DecimalesCantidad; i++)
                {
                    FormatoCantidad = FormatoCantidad + sharp;
                }
            }
            else
            {
                FormatoCantidad = "0";
            }
            if (txtCantidad.Text != "")
            {
                txtCantidad.Text = decimal.Parse(txtCantidad.Text).ToString(FormatoCantidad);
            }

        }
        private void EdicionBarraNavegacion(bool ON_OFF)
        {
            txtNroRegCorrelativo.Enabled = ON_OFF;
            btnNuevoMovimiento.Enabled = ON_OFF;
            btnAnterior.Enabled = ON_OFF;
            btnSiguiente.Enabled = ON_OFF;
            txtNroRegSerie.Enabled = ON_OFF;
            txtPeriodo.Enabled = ON_OFF;
        }

        private void LimpiarCabecera()
        {
            //cboAlmacenOrigen.SelectedValue = "-1";
            cboAlmacenDestino.Value = "-1";
            cboMoneda.Value = "-1";
            cboMotivo.Value = "-1";
            dtpFecha.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtNroRegSerie.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            txtTipoCambio.Text = string.Empty;
            txtGlosa.Clear();
            txtTotal.Clear();
            txtCantidad.Clear();
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            // cboAlmacenOrigen.SelectedValue = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
        }

        private void CargarCabecera(string idmovimiento)
        {
            OperationResult objOperationResult = new OperationResult();
            _movimientoDto = new movimientoDto();
            _movimientoDto = _objMovimientoBL.ObtenerMovimientoCabecera(ref objOperationResult, idmovimiento);
            _pstrIdMovimiento_Nuevo = _movimientoDto.v_IdMovimiento;
            if (_movimientoDto != null)
            {
                _Mode = "Edit";
                cboAlmacenOrigen.Value = _movimientoDto.i_IdAlmacenOrigen.ToString();
                cboAlmacenDestino.Value = _movimientoDto.i_IdAlmacenDestino.ToString();
                cboMoneda.Value = _movimientoDto.i_IdMoneda.ToString();
                cboMotivo.Value = _movimientoDto.i_IdTipoMotivo.ToString();
                txtGlosa.Text = _movimientoDto.v_Glosa;
                dtpFecha.Value = _movimientoDto.t_Fecha.Value;
                txtTipoCambio.Text = _movimientoDto.d_TipoCambio.ToString();
                txtNroRegCorrelativo.Text = _movimientoDto.v_Correlativo;
                txtPeriodo.Text = _movimientoDto.v_Periodo;
                txtNroRegSerie.Text = _movimientoDto.v_Mes;
                txtTotal.Text = _movimientoDto.d_TotalPrecio.ToString();
                txtCantidad.Text = _movimientoDto.d_TotalCantidad.ToString();
                _movimientoDto.v_MesGuardado = _movimientoDto.v_Mes;
                _movimientoDto.v_AnioGuardado = _movimientoDto.v_Periodo;
                _movimientoDto.v_CorrelativoGuardado = _movimientoDto.v_Correlativo;
                cboTipoDocumento.Value = (_movimientoDto.i_IdTipoDocumento ?? -1).ToString();
                txtSerieDoc.Text = _movimientoDto.v_SerieDocumento;
                txtNroDoc.Text = _movimientoDto.v_CorrelativoDocumento;

                CargarDetalle(_movimientoDto.v_IdMovimiento);
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar el movimiento", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void CargarDetalle(string pstringIdMovimiento)
        {
            OperationResult objOperationResult = new OperationResult();
            grdData.DataSource = _objMovimientoBL.ObtenerMovimientoDetalles(ref objOperationResult, pstringIdMovimiento);
            //BuscarNombresArticulos();
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
                grdData.Rows[i].Cells["i_RegistroEstado"].Value = "NoModificado";
            }
        }

        private void CargarCombosDetalle()
        {
            OperationResult objOperationResult = new OperationResult();

            #region Configura Combo Unidad Medida
            UltraGridBand ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumnaID = new UltraGridColumn("Id");
            UltraGridColumn ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            UltraGridColumn ultraGridColumnaValue2 = new UltraGridColumn("Value2");
            ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion.Width = 220;
            ultraGridColumnaID.Hidden = true;
            ultraGridColumnaValue2.Hidden = true;
            ultraGridBanda.Columns.AddRange(new object[] { ultraGridColumnaDescripcion, ultraGridColumnaID, ultraGridColumnaValue2 });
            ucUnidadMedida.DisplayLayout.BandsSerializer.Add(ultraGridBanda);
            ucUnidadMedida.DropDownWidth = 223;
            #endregion

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
            ucTipoDocumento.DisplayLayout.BandsSerializer.Add(_ultraGridBanda);
            ucTipoDocumento.DropDownStyle = Infragistics.Win.UltraWinGrid.UltraComboStyle.DropDownList;
            ucTipoDocumento.DropDownWidth = 330;
            #endregion

            Utils.Windows.LoadUltraComboList(ucTipoDocumento, "Value2", "Id", _objDocumentoBL.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 1, 0), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucUnidadMedida, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 17, null), DropDownListAction.Select);
        }

        private void LlenarTemporales()
        {
            if (grdData.Rows.Count() != 0)
            {
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                    {
                        case "Temporal":
                            if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _movimientodetalleDto = new movimientodetalleDto();
                                _movimientodetalleDto.v_IdMovimiento = _movimientoDto.v_IdMovimiento;
                                _movimientodetalleDto.v_IdMovimientoDetalleTransferencia = string.Empty;
                                _movimientodetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _movimientodetalleDto.v_NroGuiaRemision = Fila.Cells["v_NroGuiaRemision"].Value == null ? string.Empty : Fila.Cells["v_NroGuiaRemision"].Value.ToString();
                                _movimientodetalleDto.i_IdTipoDocumento = Fila.Cells["i_IdTipoDocumento"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoDocumento"].Value.ToString());
                                _movimientodetalleDto.v_NumeroDocumento = Fila.Cells["v_NumeroDocumento"].Value == null ? string.Empty : Fila.Cells["v_NumeroDocumento"].Value.ToString();
                                _movimientodetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _movimientodetalleDto.i_IdUnidad = Fila.Cells["i_IdUnidad"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidad"].Value.ToString());
                                _movimientodetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                _movimientodetalleDto.d_Total = Fila.Cells["d_Total"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Total"].Value.ToString());
                                _movimientodetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                _movimientodetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _movimientodetalleDto.CodigoProducto = Fila.Cells["v_CodigoInterno"].Value == null ? null : Fila.Cells["v_CodigoInterno"].Value.ToString();
                                _TempDetalle_AgregarDto.Add(_movimientodetalleDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _movimientodetalleDto = new movimientodetalleDto();
                                _movimientodetalleDto.v_IdMovimientoDetalle = Fila.Cells["v_IdMovimientoDetalle"].Value == null ? null : Fila.Cells["v_IdMovimientoDetalle"].Value.ToString();
                                _movimientodetalleDto.v_IdMovimiento = Fila.Cells["v_IdMovimiento"].Value == null ? null : Fila.Cells["v_IdMovimiento"].Value.ToString();
                                _movimientodetalleDto.v_IdMovimientoDetalleTransferencia = Fila.Cells["v_IdMovimientoDetalleTransferencia"].Value == null ? null : Fila.Cells["v_IdMovimientoDetalleTransferencia"].Value.ToString();
                                _movimientodetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _movimientodetalleDto.v_NroGuiaRemision = Fila.Cells["v_NroGuiaRemision"].Value == null ? string.Empty : Fila.Cells["v_NroGuiaRemision"].Value.ToString();
                                _movimientodetalleDto.i_IdTipoDocumento = Fila.Cells["i_IdTipoDocumento"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoDocumento"].Value.ToString());
                                _movimientodetalleDto.v_NumeroDocumento = Fila.Cells["v_NumeroDocumento"].Value == null ? string.Empty : Fila.Cells["v_NumeroDocumento"].Value.ToString();
                                _movimientodetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _movimientodetalleDto.i_IdUnidad = Fila.Cells["i_IdUnidad"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidad"].Value.ToString());
                                _movimientodetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                _movimientodetalleDto.d_Total = Fila.Cells["d_Total"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Total"].Value.ToString());
                                _movimientodetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                _movimientodetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _movimientodetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _movimientodetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _movimientodetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _movimientodetalleDto.CodigoProducto = Fila.Cells["v_CodigoInterno"].Value == null ? null : Fila.Cells["v_CodigoInterno"].Value.ToString();
                                _TempDetalle_ModificarDto.Add(_movimientodetalleDto);
                            }
                            break;
                    }
                }
            }

        }




        private void LlenarTemporalesDocumentos()
        {
            _TempDetalle_ModificarDocumentosDto = new List<movimientodetalleDto>();
            if (grdData.Rows.Count() != 0)
            {
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                    {
                        case "Temporal":
                            if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _movimientodetalleDto = new movimientodetalleDto();
                                _movimientodetalleDto.v_IdMovimiento = _movimientoDto.v_IdMovimiento;
                                _movimientodetalleDto.v_IdMovimientoDetalleTransferencia = string.Empty;
                                _movimientodetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _movimientodetalleDto.v_NroGuiaRemision = Fila.Cells["v_NroGuiaRemision"].Value == null ? string.Empty : Fila.Cells["v_NroGuiaRemision"].Value.ToString();
                                _movimientodetalleDto.i_IdTipoDocumento = Fila.Cells["i_IdTipoDocumento"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoDocumento"].Value.ToString());
                                _movimientodetalleDto.v_NumeroDocumento = Fila.Cells["v_NumeroDocumento"].Value == null ? string.Empty : Fila.Cells["v_NumeroDocumento"].Value.ToString();
                                _TempDetalle_AgregarDocumentosDto.Add(_movimientodetalleDto);
                                
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _movimientodetalleDto = new movimientodetalleDto();
                                _movimientodetalleDto.v_IdMovimientoDetalle = Fila.Cells["v_IdMovimientoDetalle"].Value == null ? null : Fila.Cells["v_IdMovimientoDetalle"].Value.ToString();
                                _movimientodetalleDto.v_IdMovimiento = Fila.Cells["v_IdMovimiento"].Value == null ? null : Fila.Cells["v_IdMovimiento"].Value.ToString();
                                _movimientodetalleDto.v_IdMovimientoDetalleTransferencia = Fila.Cells["v_IdMovimientoDetalleTransferencia"].Value == null ? null : Fila.Cells["v_IdMovimientoDetalleTransferencia"].Value.ToString();
                                _movimientodetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _movimientodetalleDto.v_NroGuiaRemision = Fila.Cells["v_NroGuiaRemision"].Value == null ? string.Empty : Fila.Cells["v_NroGuiaRemision"].Value.ToString();
                                _movimientodetalleDto.i_IdTipoDocumento = Fila.Cells["i_IdTipoDocumento"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoDocumento"].Value.ToString());
                                _movimientodetalleDto.v_NumeroDocumento = Fila.Cells["v_NumeroDocumento"].Value == null ? string.Empty : Fila.Cells["v_NumeroDocumento"].Value.ToString();
                              _TempDetalle_ModificarDocumentosDto.Add(_movimientodetalleDto);
                            }
                            break;
                    }
                }
            }

        }


        private void BuscarNombresArticulos()
        {
            int j = grdData.Rows.Count();

            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                string[] Cadena = new string[7];
                Cadena = _objMovimientoBL.DevolverNombresProductoAlmacen(grdData.Rows[i].Cells["v_IdProductoDetalle"].Value.ToString(), int.Parse(cboAlmacenOrigen.Value.ToString()));
                grdData.Rows[i].Cells["StockActual"].Value = Cadena[2];
                grdData.Rows[i].Cells["EsServicio"].Value = Cadena[5];
                grdData.Rows[i].Cells["i_IdUnidadMedidaProducto"].Value = Cadena[6];

            }
        }


        private void CalcularTotal()
        {
            decimal TotalPrecio = 0, TotalCantidad = 0;
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                if (Fila.Cells["d_Total"].Value == null || Fila.Cells["d_Cantidad"].Value == null) return;
                TotalPrecio = TotalPrecio + decimal.Parse(Fila.Cells["d_Total"].Value.ToString());
                TotalCantidad = TotalCantidad + decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
            }
            txtTotal.Text = TotalPrecio.ToString("0.00");
            txtCantidad.Text = TotalCantidad.ToString("0.00");
        }

        private void CalcularValoresFila(UltraGridRow Fila)
        {
            decimal precio, total, cantidad;

            if (Fila.Cells["d_Cantidad"].Value == null) { Fila.Cells["d_Cantidad"].Value = "0"; }
            if (Fila.Cells["d_Precio"].Value == null) { Fila.Cells["d_Precio"].Value = "0"; }

            cantidad = decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value.ToString());
            precio = decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value.ToString());
            total = cantidad * precio;
            Fila.Cells["d_Total"].Value = total;
            CalcularTotales();
            CalcularTotal();

        }

        private void CalcularTotales()
        {
            decimal Total = 0, TotalCantidad = 0;

            foreach (UltraGridRow Fila in grdData.Rows)
            {
                if (Fila.Cells["d_Cantidad"].Value == null) { Fila.Cells["d_Cantidad"].Value = "0"; }
                if (Fila.Cells["d_Precio"].Value == null) { Fila.Cells["d_Precio"].Value = "0"; }
                if (Fila.Cells["d_Total"].Value == null) { Fila.Cells["d_Total"].Value = "0"; }

                if (decimal.Parse(Fila.Cells["d_Total"].Value.ToString()) != 0)
                {
                    Total = Total + decimal.Parse(Fila.Cells["d_Total"].Value.ToString());
                }

                if (decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0)
                {
                    TotalCantidad = TotalCantidad + decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                }



                if (Fila.Cells["i_IdUnidad"].Value != null)
                {
                    if (Fila.Cells["v_IdProductoDetalle"].Value != null && Fila.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000" && Fila.Cells["i_IdUnidad"].Value.ToString() != "-1" && decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0)
                    {
                        decimal TotalEmpaque = 0;
                        decimal Empaque = decimal.Parse(Fila.Cells["Empaque"].Value.ToString());
                        string Producto = Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                        decimal Cantidad = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                        int UM = int.Parse(Fila.Cells["i_IdUnidad"].Value.ToString());
                        int UMProducto = int.Parse(Fila.Cells["i_IdUnidadMedidaProducto"].Value.ToString());

                        GridKeyValueDTO _UMProducto = ((List<GridKeyValueDTO>)ucUnidadMedida.DataSource).Where(p => p.Id == UMProducto.ToString()).FirstOrDefault();
                        GridKeyValueDTO _UM = ((List<GridKeyValueDTO>)ucUnidadMedida.DataSource).Where(p => p.Id == UM.ToString()).FirstOrDefault();

                        if (_UM != null)
                        {
                            switch (_UM.Value1)
                            {
                                case "CAJA":
                                    decimal Caja = Empaque * (!string.IsNullOrEmpty(_UMProducto.Value2) ? decimal.Parse(_UMProducto.Value2) : 0);
                                    TotalEmpaque = Cantidad * Caja;
                                    break;

                                default:
                                    TotalEmpaque = Cantidad * (!string.IsNullOrEmpty(_UM.Value2) ? decimal.Parse(_UM.Value2) : 0);
                                    break;
                            }
                        }
                        Fila.Cells["d_CantidadEmpaque"].Value = TotalEmpaque.ToString();
                    }
                    else
                    {
                        Fila.Cells["d_CantidadEmpaque"].Value = "0";
                    }
                }
            }
            txtTotal.Text = Total.ToString("0.00");
            txtCantidad.Text = TotalCantidad.ToString("0.00");

            FormatoDecimalesTotales((int)Globals.ClientSession.i_CantidadDecimales, (int)Globals.ClientSession.i_PrecioDecimales);

        }

        private void cboAlmacenOrigen_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cboAlmacenOrigen.Value == null || cboAlmacenOrigen.Value.ToString() == "-1") return;
            EstablecimientoBL objEstablecimientoBL = new EstablecimientoBL();
            List<KeyValueDTO> Almacenes = new List<KeyValueDTO>();
            if (cboAlmacenOrigen.Value == null) return;
            Almacenes = objEstablecimientoBL.GetAlmacenesXEstablecimiento(int.Parse(cboAlmacenOrigen.Value.ToString()));
            Utils.Windows.LoadUltraComboEditorList(cboAlmacenDestino, "Value1", "Id", Almacenes, DropDownListAction.Select);
        }
        private bool ValidaCamposNulosVacios()
        {
            if (grdData.Rows.Where(p => p.Cells["v_IdProductoDetalle"].Value == null || p.Cells["v_IdProductoDetalle"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente los productos", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(x => x.Cells["v_IdProductoDetalle"].Value == null || x.Cells["v_IdProductoDetalle"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["v_CodigoInterno"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_CodigoInterno"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (grdData.Rows.Where(p => p.Cells["d_Cantidad"].Value == null || p.Cells["d_Cantidad"].Value.ToString().Trim() == string.Empty || decimal.Parse(p.Cells["d_Cantidad"].Value.ToString()) <= 0).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente la Cantidad", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(x => x.Cells["d_Cantidad"].Value == null || x.Cells["d_Cantidad"].Value.ToString().Trim() == string.Empty || decimal.Parse(x.Cells["d_Cantidad"].Value.ToString()) <= 0).FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["d_Cantidad"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_Cantidad"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            return true;
        }
        private string VerificarStockProductoAlmacen()
        {

            string mensaje = "";
            OperationResult objOperation = new OperationResult();

            foreach (UltraGridRow Fila in grdData.Rows)
            {
                decimal cantidad = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                int ValidarStock = _objPedidoBL.ValidarStock(ref objOperation, Fila.Cells["v_IdProductoDetalle"].Value.ToString());
                List<productoalmacen> ListaProductoAlmacen = _objMovimientoBL.ListaProductoAlmacen(Globals.ClientSession.i_Periodo.ToString());
                var sa=_objMovimientoBL.ObtenerStockProductoAlmacen(ref objOperation, Fila.Cells["v_IdProductoDetalle"].Value.ToString(), int.Parse(cboAlmacenOrigen.Value.ToString()), Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim(), ListaProductoAlmacen, Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim(), Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim());
                decimal stockActual = sa != null ? sa.d_StockActual ?? 0 : 0;

                if (cantidad > stockActual && ValidarStock ==1)
                {
                    mensaje = mensaje + Fila.Cells["v_CodigoInterno"].Value.ToString().Trim() + "-" + Fila.Cells["v_NombreProducto"].Value.ToString() + "\n";
                }
            }
            return mensaje;

        }
        private string VerificarStockProductoAlmacenEditado()
        {
            OperationResult objOperation = new OperationResult();
            string mensaje = "";
            List<productoalmacen> ListaProductoAlmacen = _objMovimientoBL.ListaProductoAlmacen(Globals.ClientSession.i_Periodo.ToString());
            foreach (UltraGridRow Fila in grdData.Rows)
            {

                decimal cantidad = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                var sa =_objMovimientoBL.ObtenerStockProductoAlmacen(ref objOperation, Fila.Cells["v_IdProductoDetalle"].Value.ToString(), int.Parse(cboAlmacenOrigen.Value.ToString()), Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim(), ListaProductoAlmacen, Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim(), Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim());
                decimal stockActual = sa != null ? sa.d_StockActual ?? 0 : 0;
                int ValidarStock = _objPedidoBL.ValidarStock(ref objOperation, Fila.Cells["v_IdProductoDetalle"].Value.ToString());
                if (Fila.Cells["v_IdMovimientoDetalle"].Value != null && Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                {
                    decimal stockAnterior = _objMovimientoBL.ObtenerCantidadMovimientoDetalle(ref objOperation, Fila.Cells["v_IdMovimientoDetalle"].Value.ToString());
                    if ((cantidad > stockActual + stockAnterior) && ValidarStock ==1 )
                    {
                        mensaje = mensaje + Fila.Cells["v_CodigoInterno"].Value.ToString().Trim() + "-" + Fila.Cells["v_NombreProducto"].Value.ToString() + "\n";

                    }
                }
                else if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                {
                    if (cantidad > stockActual &&  ValidarStock ==1)
                    {
                        mensaje = mensaje + Fila.Cells["v_CodigoInterno"].Value.ToString().Trim() + "-" + Fila.Cells["v_NombreProducto"].Value.ToString() + "\n";
                    }

                }
            }
            return mensaje;

        }
        #endregion

        #region Eventos de la Grilla
        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdData.ActiveCell == null) return;

            if (this.grdData.ActiveCell.Column.Key != "i_IdTipoDocumento")
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

        private void grdData_InitializeLayout(object sender, Infragistics.Win.UltraWinGrid.InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdUnidad"].EditorComponent = ucUnidadMedida;
            e.Layout.Bands[0].Columns["i_IdUnidad"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdTipoDocumento"].EditorComponent = ucTipoDocumento;
            e.Layout.Bands[0].Columns["i_IdTipoDocumento"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }

        private void grdData_DoubleClickCell(object sender, Infragistics.Win.UltraWinGrid.DoubleClickCellEventArgs e)
        {
            if (int.Parse(cboAlmacenOrigen.Value.ToString()) == -1)
            {
                UltraMessageBox.Show("Por favor seleccione  almacén Origen antes de buscar un producto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (e.Cell.Column.Key == "v_CodigoInterno")
            {
                Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(int.Parse(cboAlmacenOrigen.Value.ToString()), "Salida", null, grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Text == null ? string.Empty : grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Text.ToString());
                frm.ShowDialog();
                if (frm._NombreProducto != null)
                {
                    foreach (UltraGridRow Fila in grdData.Rows)
                    {
                        if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                        {
                            if (frm._IdProducto == Fila.Cells["v_IdProductoDetalle"].Value.ToString())
                            {
                                UltraMessageBox.Show("El producto ya existe en el detalle", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                return;
                            }
                        }
                    }

                    if ((frm._stockActual - frm._SeparaccionTotal <= 0) && frm._ValidarStock ==1 )
                    {
                        UltraMessageBox.Show("El producto tiene Stock 0", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreProducto"].Value = frm._NombreProducto.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = frm._IdProducto.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value = frm._CodigoInternoProducto.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidad"].Value = frm._UnidadMedidaEmpaque != null ? frm._UnidadMedidaEmpaque.ToString() : string.Empty;  //Por defecto ,pero si desea el usuario lo puede cambiar
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = frm._UnidadMedidaEmpaque != null ? frm._UnidadMedidaEmpaque.ToString() : null;
                        grdData.Rows[grdData.ActiveRow.Index].Cells["Empaque"].Value = frm._Empaque.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = frm._UnidadMedida != null ? frm._UnidadMedida.ToString() : string.Empty;
                        grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value = frm._stockActual.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroPedido"].Value = frm._NroPedidoExportacion;
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value = frm._ValidarStock;
                        
                        CalcularTotales();
                    }
                }


                UltraGridCell aCell = grdData.Rows[e.Cell.Row.Index].Cells["d_Cantidad"];
                grdData.Selected.Cells.Add(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"]);
                grdData.Focus();
                grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                this.grdData.ActiveCell = aCell;

            }
        }

        public void RecibirItems(List<UltraGridRow> Filas)
        {
            bool ExistenciaGrilla = false, anteriorRegistro = false;
            bool Saldo0 = false;
            int mensajeEg = 0, cantMensajes = 0;

            for (int i = 0; i < Filas.Count; i++)
            {
                ExistenciaGrilla = false;
                Saldo0 = false;
                //int IdAlmacen = int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdAlmacen"].Value.ToString());
                if (Filas[i].Cells["v_CodInterno"].Value != null)
                {
                    if (decimal.Parse(Filas[i].Cells["Saldo"].Value.ToString()) <= 0 && Filas[i].Cells["i_ValidarStock"].Value.ToString() == "1")
                    {
                        if (cantMensajes == 0)
                        {
                            UltraMessageBox.Show("Uno de los productos seleccionados tiene stock  0", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Saldo0 = true;
                            cantMensajes = cantMensajes + 1;
                        }
                        else
                        {
                            Saldo0 = true;
                            cantMensajes = cantMensajes + 1;
                        }

                    }
                }


                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                    {
                        if (Filas[i].Cells["v_IdProductoDetalle"].Value.ToString() == Fila.Cells["v_IdProductoDetalle"].Value.ToString())
                        {
                            if (mensajeEg == 0)
                            {
                                UltraMessageBox.Show("Uno de los productos seleccionados ya existe en el detalle", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ExistenciaGrilla = true;
                                mensajeEg = mensajeEg + 1;
                            }
                            else
                            {
                                ExistenciaGrilla = true;
                                mensajeEg = mensajeEg + 1;
                            }

                        }

                    }
                }
                if (i == 0)
                {
                    if (!Saldo0)
                    {
                        if (!ExistenciaGrilla)
                        {
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidad"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();  //Por defecto ,pero si desea el usuario lo puede cambiar
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value = "0";
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                            grdData.Rows[grdData.ActiveRow.Index].Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? null : Filas[i].Cells["d_Empaque"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value = Filas[i].Cells["stockActual"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["stockActual"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value = "Temporal";
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroPedido"].Value = Filas[i].Cells["v_NroPedidoExportacion"].Value == null ? null : Filas[i].Cells["v_NroPedidoExportacion"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value = Filas[i].Cells["i_ValidarStock"] == null ? 0 : int.Parse(Filas[i].Cells["i_ValidarStock"].Value.ToString());
                        }
                        else
                        {
                            anteriorRegistro = true;
                        }
                    }
                    else
                    {
                        anteriorRegistro = true;
                    }

                }
                else
                {
                    if (!Saldo0)
                    {
                        if (!ExistenciaGrilla)
                        {

                            if (anteriorRegistro)
                            {
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? null : Filas[i].Cells["d_Empaque"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value = "0";
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value = "Temporal";
                                grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value = Filas[i].Cells["stockActual"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["stockActual"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidad"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();  //Por defecto ,pero si desea el usuario lo puede cambiar
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroPedido"].Value = Filas[i].Cells["v_NroPedidoExportacion"].Value == null ? null : Filas[i].Cells["v_NroPedidoExportacion"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value = Filas[i].Cells["i_ValidarStock"] == null ? 0 : int.Parse(Filas[i].Cells["i_ValidarStock"].Value.ToString());
                                anteriorRegistro = false;

                            }
                            else
                            {
                                UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                                this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                row.Cells["i_RegistroEstado"].Value = "Agregado";
                                row.Cells["i_RegistroTipo"].Value = "Temporal";
                                row.Cells["i_IdUnidad"].Value = "-1";
                                row.Cells["d_Cantidad"].Value = "1";
                                row.Cells["d_Precio"].Value = "0";
                                row.Cells["i_IdTipoDocumento"].Value = "-1";
                                row.Activate();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? null : Filas[i].Cells["d_Empaque"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidad"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();  //Por defecto ,pero si desea el usuario lo puede cambiar
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value = "0";
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                                grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value = Filas[i].Cells["stockActual"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["stockActual"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroPedido"].Value = Filas[i].Cells["v_NroPedidoExportacion"].Value == null ? null : Filas[i].Cells["v_NroPedidoExportacion"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value = Filas[i].Cells["i_ValidarStock"] == null ? 0 : int.Parse(Filas[i].Cells["i_ValidarStock"].Value.ToString());
                            }
                        }
                    }
                }
            }
            object o= new object ();
           EventArgs e= new EventArgs ();
           btnMarcarTodos_Click(o, e);
           CalcularTotales();
        }


        private void grdData_CellChange(object sender, Infragistics.Win.UltraWinGrid.CellEventArgs e)
        {
            grdData.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
        }

        private void grdData_AfterCellActivate(object sender, EventArgs e)
        {
            if (grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value == null || grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value == null) return;
            if (decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value.ToString()) == 0 | decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value.ToString()) == 0) return;
            decimal cantidad;
            decimal precio, total;
            cantidad = decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value.ToString());
            precio = decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value.ToString());
            total = cantidad * precio;
            grdData.Rows[grdData.ActiveRow.Index].Cells["d_Total"].Value = total;
            CalcularTotal();
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new System.Drawing.Point(e.X, e.Y);
            Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

            if (row == null)
            {
                btnEliminar.Enabled = false;
            }
            else
            {
                btnEliminar.Enabled = true;
            }
        }


        private void grdData_KeyPress(object sender, KeyPressEventArgs e)
        {
            UltraGridCell Celda;
            if (this.grdData.ActiveCell == null)
            {
                return;
            }

            switch (this.grdData.ActiveCell.Column.Key)
            {

                case "d_Cantidad":

                    Celda = grdData.ActiveCell;
                    Utils.Windows.NumeroDecimalCelda(Celda, e);
                    break;


                case "d_Precio":
                    Celda = grdData.ActiveCell;
                    Utils.Windows.NumeroDecimalCelda(Celda, e);
                    break;

                case "v_NroGuiaRemision":
                    Celda = grdData.ActiveCell;
                    Utils.Windows.NumeroDecimalCelda(Celda, e);
                    break;


            }

        }

        private void grdData_KeyUp(object sender, KeyEventArgs e)
        {

            //if (grdData.ActiveCell.Column.Key ==null) return ;

            //if (this.grdData.ActiveCell.Column.Key == "i_IdTipoDocumento")
            //{
            //    foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in ucTipoDocumento.Rows)
            //    {
            //        if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            //        {
            //            return;
            //        }
            //        bool filterRow = true;
            //        foreach (UltraGridColumn column in ucTipoDocumento.DisplayLayout.Bands[0].Columns)
            //        {
            //            if (column.IsVisibleInLayout)
            //            {
            //                if (row.Cells[column].Text.Contains(grdData.ActiveCell.Text.ToUpper()))
            //                {
            //                    filterRow = false;
            //                    break;
            //                }
            //            }
            //        }
            //        if (filterRow)
            //        {
            //            row.Hidden = true;
            //        }
            //        else
            //        {
            //            row.Hidden = false;
            //        }
            //    }
            //}
        }

        private void FormatoDecimalesGrilla(int DecimalesCantidad, int DecimalesPrecio)
        {
            string FormatoCantidad, FormatoPrecio;
            UltraGridColumn _Cantidad = this.grdData.DisplayLayout.Bands[0].Columns["d_Cantidad"];
            _Cantidad.MaskDataMode = MaskMode.IncludeLiterals;
            _Cantidad.MaskDisplayMode = MaskMode.IncludeLiterals;

            UltraGridColumn _Precio = this.grdData.DisplayLayout.Bands[0].Columns["d_Precio"];
            _Precio.MaskDataMode = MaskMode.IncludeLiterals;
            _Precio.MaskDisplayMode = MaskMode.IncludeLiterals;

            if (DecimalesCantidad > 0)
            {
                string sharp = "n";
                FormatoCantidad = "nnnnnnnnnn.";
                for (int i = 0; i < DecimalesCantidad; i++)
                {
                    FormatoCantidad = FormatoCantidad + sharp;
                }
            }
            else
            {
                FormatoCantidad = "nnnnnnnnnn";
            }

            if (DecimalesPrecio > 0)
            {
                string sharp = "n";
                FormatoPrecio = "nnnnnnnnnn.";
                for (int i = 0; i < DecimalesPrecio; i++)
                {
                    FormatoPrecio = FormatoPrecio + sharp;
                }
            }
            else
            {
                FormatoPrecio = "nnnnnn";
            }

            _Cantidad.MaskInput = FormatoCantidad;
            _Precio.MaskInput = FormatoPrecio;
        }
        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");
        }

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            decimal stockActual, cantidad, cantidadAnterior;
            OperationResult objOperation = new OperationResult();
            string stockIngresado = string.Empty;

            if (grdData.Rows[grdData.ActiveRow.Index] == null) return;
            if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value != null)
            {
                if (this.grdData.ActiveCell.Column.Key == "d_Cantidad" || this.grdData.ActiveCell.Column.Key == "d_Precio")
                {
                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value == null) return;

                    stockIngresado = grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value.ToString();

                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
                    {

                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                        {
                           
                            stockActual =  grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value ==null ? 0:decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value.ToString());
                            cantidad = decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value.ToString());
                            cantidadAnterior = _objMovimientoBL.ObtenerCantidadMovimientoDetalle(ref objOperation, grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdMovimientoDetalle"].Value.ToString());
                            if (cantidad > cantidadAnterior + stockActual  && grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value.ToString() == "1")
                            {
                               // UltraMessageBox.Show("La Cantidad Ingresada a sobrepasado el stock existente", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                
                            }
                            
                        }
                        else
                        {
                            stockIngresado = "0";
                            stockActual = grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value ==null ?0: decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value.ToString());
                            cantidad = decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value.ToString());
                            if (decimal.Parse(stockIngresado) > stockActual  && grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value.ToString() == "1")
                            {
                                //UltraMessageBox.Show("La Cantidad Ingresada a sobrepasado el stock existente", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                               

                            }

                        }

                    }
                    else
                    {
                        stockActual =  grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value ==null ?0: decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["StockActual"].Value.ToString());
                        if (decimal.Parse(stockIngresado) > stockActual && grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value.ToString() == "1")
                        {
                           // UltraMessageBox.Show("La Cantidad Ingresada a sobrepasado el stock existente", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            

                        }
                    }
                   
                }

                if (grdData.ActiveCell.Column.Key == "v_NroGuiaRemision")
                {
                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroGuiaRemision"].Value != null)
                    {
                        string NroGuia;
                        NroGuia = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroGuiaRemision"].Value.ToString();
                        if (NroGuia.Contains("-"))
                        {
                            string[] SerieCorrelativo = new string[2];
                            SerieCorrelativo = NroGuia.Split(new Char[] { '-' });
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroGuiaRemision"].Value = int.Parse(SerieCorrelativo[0]).ToString("0000") + "-" + int.Parse(SerieCorrelativo[1]).ToString("00000000");
                        }
                    }
                }

                if (grdData.ActiveCell.Column.Key == "v_NumeroDocumento")
                {
                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_NumeroDocumento"].Value != null)
                    {
                        string NroGuia;
                        NroGuia = grdData.Rows[grdData.ActiveRow.Index].Cells["v_NumeroDocumento"].Value.ToString();
                        if (NroGuia.Contains("-"))
                        {
                            var SerieCorrelativo = NroGuia.Split(new Char[] { '-' });
                            if(SerieCorrelativo.All(r => !string.IsNullOrWhiteSpace(r)))
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_NumeroDocumento"].Value = int.Parse(SerieCorrelativo[0]).ToString("0000") + "-" + int.Parse(SerieCorrelativo[1]).ToString("00000000");
                        }
                    }
                }

            }

            CalcularValoresFila(grdData.Rows[grdData.ActiveRow.Index]);
        }
        #endregion

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            var frm = new Reportes.Almacen.frmDocumentoTransferencia(_pstrIdMovimiento_Nuevo);
            frm.Show();
        }

        private void cboAlmacenOrigen_ValueChanged(object sender, EventArgs e)
        {
            if (cboAlmacenOrigen.Value == null || cboAlmacenOrigen.Value.ToString() == "-1") return;
            var objEstablecimientoBl = new EstablecimientoBL();
            if (cboAlmacenOrigen.Value == null) return;

            var almacenes = objEstablecimientoBl.GetAlmacenesXEstablecimiento(int.Parse(cboAlmacenOrigen.Value.ToString()));
            Utils.Windows.LoadUltraComboEditorList(cboAlmacenDestino, "Value1", "Id", almacenes, DropDownListAction.Select);
        }

        private void btnMigracion_Click(object sender, EventArgs e)
        {
            foreach (var fila in grdData.Rows)
                fila.Cells["i_RegistroEstado"].Value = "Modificado";
            UltraMessageBox.Show("Todos los Cáculos han finalizado", "Sistema");
        }

        private void txtProductoIngresar_KeyDown(object sender, KeyEventArgs e)
        {
                        if (e.KeyCode != Keys.Enter) return;
            if(txtProductoIngresar.Text.Trim() == "") return;

            var codProd = txtProductoIngresar.Text.Trim();
            var row = grdData.Rows.FirstOrDefault(r => (string)r.GetCellValue("v_CodigoInterno") == codProd);
            if (row != null)
            {
                row.Cells["d_Cantidad"].Value = decimal.Parse(row.GetCellValue("d_Cantidad").ToString()) + 1;
            }
            else
            {
                var prod = _objMovimientoBL.DevolverArticuloPorCodInterno(codProd, int.Parse(cboAlmacenOrigen.Value.ToString()));
                if (prod == null)
                {
                    UltraMessageBox.Show("¡Producto No encontrado!", "Error en Busqueda");
                    return;
                }
                InvokeOnClick(btnAgregar, e);
                row = grdData.ActiveRow;
                if (row == null) return;
                row.Cells["v_NombreProducto"].Value = prod.v_Descripcion.Trim();
                row.Cells["v_IdProductoDetalle"].Value = prod.v_IdProductoDetalle.Trim();
                row.Cells["v_CodigoInterno"].Value = prod.v_CodInterno.Trim();
                row.Cells["Empaque"].Value = prod.d_Empaque.HasValue
                    ? prod.d_Empaque.Value.ToString(CultureInfo.CurrentCulture)
                    : null;
                row.Cells["UMEmpaque"].Value = prod.EmpaqueUnidadMedida;
                row.Cells["d_Cantidad"].Value = 1M;
                row.Cells["d_Precio"].Value = 0M;
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["StockActual"].Value = prod.stockActual ?? 0M;
                row.Cells["i_IdUnidad"].Value = prod.i_IdUnidadMedida;
                row.Cells["i_IdUnidadMedidaProducto"].Value = prod.i_IdUnidadMedida;
                txtProductoIngresar.Focus();
            }
            txtProductoIngresar.Clear();
        }

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc, "{0:0000}");
        }

        private void txtNroDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtNroDoc, "{0:00000000}");
        }

        private void btnMarcarTodos_Click(object sender, EventArgs e)
        {
            if (ValidarMarcarTodos.Validate(true, false).IsValid)
            {
                foreach (var fila in grdData.Rows)
                {
                    fila.Cells["i_RegistroEstado"].Value = "Modificado";
                    fila.Cells["i_IdTipoDocumento"].Value = cboTipoDocumento.Value == null ? "-1" : fila.Cells["i_IdTipoDocumento"].Value.ToString() == "-1" ? cboTipoDocumento.Value.ToString() : fila.Cells["i_IdTipoDocumento"].Value.ToString();
                    fila.Cells["v_NumeroDocumento"].Value = !string.IsNullOrEmpty ( txtSerieDoc.Text.Trim ()) && !string.IsNullOrEmpty ( txtNroDoc.Text.Trim ()) ? txtSerieDoc.Text.Trim() + "-" + txtNroDoc.Text.Trim() : fila.Cells["v_NumeroDocumento"].Value.ToString();
                    fila.Cells["v_NroGuiaRemision"].Value =  cboTipoDocumento.Value != null && int.Parse(cboTipoDocumento.Value.ToString()) == (int)TiposDocumentos.GuiaRemision ? !string.IsNullOrEmpty ( txtSerieDoc.Text.Trim ()) && !string.IsNullOrEmpty ( txtNroDoc.Text.Trim ()) ?   txtSerieDoc.Text.Trim() + "-" + txtNroDoc.Text.Trim() : "" :  fila.Cells["v_NroGuiaRemision"].Value==null ?"":  fila.Cells["v_NroGuiaRemision"].Value.ToString();
                    //UltraMessageBox.Show("Datos Actualizados", "Sistema");
                }
            }
        }

        private void btnActualizarDocumentosIngresosSalidas_Click(object sender, EventArgs e)
        {
            
           
            OperationResult objOperationResult = new OperationResult ();
            LlenarTemporalesDocumentos();
            new MovimientoBL().ActualizarDocumentosTransferencias(ref objOperationResult, txtNroRegSerie.Text.Trim(), txtPeriodo.Text.Trim(), txtNroRegCorrelativo.Text.Trim(), _TempDetalle_ModificarDocumentosDto);
            if (objOperationResult.Success == 1)
            {

                UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                strModo = "Guardado";
                EdicionBarraNavegacion(true);
                _pstrIdMovimiento_Nuevo = strIdmovimiento;
                ObtenerListadoMovimientos(txtPeriodo.Text, txtNroRegSerie.Text);
                Agregado = string.Empty;
                btnImprimir.Enabled = _btnImprimir;

            }
            else
            {
                UltraMessageBox.Show("Ocurrió un error al guardar", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


        }

        private void btnRegenerarTransferencia_Click(object sender, EventArgs e)
        {
            
            
            
            OperationResult  objOperationResult = new OperationResult();
            _objMovimientoBL.RegenerarTransferencia(ref objOperationResult, strIdmovimiento, Globals.ClientSession.GetAsList());
            if (objOperationResult.Success == 1)
            {
                strModo = "Guardado";
                EdicionBarraNavegacion(true);
                _pstrIdMovimiento_Nuevo = strIdmovimiento;
                ObtenerListadoMovimientos(txtPeriodo.Text, txtNroRegSerie.Text);
                UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Agregado = string.Empty;
                btnImprimir.Enabled = _btnImprimir;
            }
            else
            {
                UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            _TempDetalle_AgregarDto = new List<movimientodetalleDto>();
            _TempDetalle_ModificarDto = new List<movimientodetalleDto>();
            _TempDetalle_EliminarDto = new List<movimientodetalleDto>();
        }

       
    }
}
