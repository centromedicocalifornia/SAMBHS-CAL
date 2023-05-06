using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Contabilidad.BL;
using SAMBHS.Compra.BL;
using SAMBHS.Almacen.BL;
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Security.BL;
using SAMBHS.Venta.BL;
using LoadingClass;
using System.Transactions;
using Infragistics.Win;

using SAMBHS.Windows.WinClient.UI.Mantenimientos;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmGuiaRemision : Form
    {
        #region Fields
        private readonly DocumentoBL _objDocumentoBl = new DocumentoBL();
        private readonly PedidoBL _objPedidoBl = new PedidoBL();
        private readonly GuiaRemisionBL _objGuiaRemisionBl = new GuiaRemisionBL();
        private readonly TransportistaChoferBL _objTransportistaChoferBl = new TransportistaChoferBL();
        private readonly TransportistaUnidadTransporteBL _objTransporteUnidadTransporte = new TransportistaUnidadTransporteBL();
        private readonly CierreMensualBL _objCierreMensualBl = new CierreMensualBL();
        private List<GridKeyValueDTO> _listadoComboDocumentos;
        private List<KeyValueDTO> _listadoTransportistas;
        private List<GridKeyValueDTO> _listadoComboGuias;
        private guiaremisionDto _guiaremisionDto = new guiaremisionDto();
        private readonly MovimientoBL _objMovimientoBl = new MovimientoBL();
        private guiaremisiondetalleDto _guiaremisiondetalleDto = new guiaremisiondetalleDto();
        UltraCombo ucbTipoBultos = new UltraCombo();
        UltraCombo ucbUnidadMedida = new UltraCombo();
        UltraCombo ucbUnidadEmpaque = new UltraCombo();
        UltraCombo ucbAlmacen = new UltraCombo();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        private List<KeyValueDTO> _listadoGuiaRemision = new List<KeyValueDTO>();
        private string _strModo, _strIdGuiaRemision, _mode, _strIdTransportista;
        public string PstrIdMovimientoNuevo;
        private int _maxV, _actV;
        private bool Temporal, _btnGuardar, _btnImprimir, EncontrarDocRef, IngresarValidating, FiltroVentas;
        private string newIdGuiaRemision = string.Empty;
        private string serieDocPrevio, correlativoDocPrevio;
        private int _tipoDocumentoPrevio, _tipoDocumentoPrevioChange, _scroll;
        private readonly string MENSAJEVS = "La lista de Producto(s) tienen la cantidad superior al permitido en SISTEMA: \n";
        private bool _eventoEnter = true, _extraidoDocumentoReferencia;
        movimientoDto _movimientoDto = new movimientoDto();
        movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
        private readonly string _utilizado;
        private decimal _redondeo;
        private ventaDto _idVenta;
        #endregion

        #region Temporales NotaSalidaDetalle
        List<movimientodetalleDto> __TempDetalle_AgregarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> __TempDetalle_ModificarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> __TempDetalle_EliminarDto = new List<movimientodetalleDto>();
        #endregion

        #region Evntos de navegacion
        public string IdRecibido
        {
            set { _idRecibido = value; }
        }
        private string _idRecibido;
        public delegate void OnSiguienteAnterior();
        public event OnSiguienteAnterior OnSiguiente;
        public event OnSiguienteAnterior OnAnterior;

        #endregion

        public frmGuiaRemision(string Modo, string idGuiaRemision, string utilizadoEn = null)
        {
            InitializeComponent();
            _strModo = Modo;
            _strIdGuiaRemision = idGuiaRemision;
            _utilizado = utilizadoEn;
        }

        private void frmGuiaRemision_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            pnlVariosTiposImpresion.Appearance.BackColor = panel1.BackColor;
            flowLayoutPanel1.BackColor = BackColor;
            var objOperationResult = new OperationResult();
            #region ControlAcciones
            if (_objCierreMensualBl.VerificarMesCerrado(Globals.ClientSession.i_Periodo.ToString().Trim(), DateTime.Now.Month.ToString("00").Trim(), (int)ModulosSistema.VentasFacturacion) || _utilizado == "KARDEX")
            {

                btnGuardar.Visible = false;
                Text = _utilizado == "KARDEX" ? "Guía de Remisión" : "Guía de Remisión [MES CERRADO]";

                if (_utilizado == "KARDEX")
                {

                    btnSalir.Visible = false;
                    btnAgregar.Visible = false;
                    btnEliminar.Visible = false;
                    btnImprimir.Visible = false;
                }

            }
            else
            {
                btnGuardar.Visible = true;
                Text = @"Guía de Remisión";
            };
            var formActions = new SecurityBL().GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmGuiaRemision", Globals.ClientSession.i_RoleId);
            _btnGuardar = Utils.Windows.IsActionEnabled("frmGuiaRemision_Save", formActions);
            _btnImprimir = Utils.Windows.IsActionEnabled("frmGuiaRemision_Print", formActions);
            btnGuardar.Enabled = _btnGuardar;
            btnImprimir.Enabled = _btnImprimir;
            btnEliminar.Enabled = false;
            #endregion
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            txtMes.Text = DateTime.Now.Month.ToString();
            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
            #region CargarCombos
            List<string> FiltersComboDocumento = new List<string>()
            {
                "i_CodigoDocumento== 1 ||i_CodigoDocumento== 3 || i_CodigoDocumento== 438"
            };
            var strFilterComboGuia = "i_CodigoDocumento==9";
            _listadoComboDocumentos = new DocumentoBL().ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
            _listadoComboGuias = _objGuiaRemisionBl.ObtenerDocumentosParaComboGridGuiaRemision(ref objOperationResult, null, strFilterComboGuia);
            var objDatahierarchyBl = new DatahierarchyBL();
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstado, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 158, null), DropDownListAction.Select);// UTILIZO EL MISMO ESTADO QUE PARA RECIBO HONORARIO DEL DATAHIE
            Utils.Windows.LoadUltraComboEditorList(cboMotivos, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 33, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboDocumentoRef, "Value2", "Id", _listadoComboDocumentos, DropDownListAction.Select);
            var objAgenciaTransporteBl = new AgenciaTransporteBL();
            _listadoTransportistas = objAgenciaTransporteBl.ObtenerListadoAgenciaTransporteParaCombo(ref objOperationResult, null, null);
            Utils.Windows.LoadUltraComboEditorList(cboRazonAgencia, "Value1", "Id", objAgenciaTransporteBl.ObtenerListadoAgenciaTransporteParaCombo(ref objOperationResult, null, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboPlaca, "Value1", "Id", _objTransporteUnidadTransporte.ObtenerListadoTransportistaUnidadTransporteParaCombo(ref objOperationResult, null, null, ""), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboChofer, "Value1", "Id", _objTransportistaChoferBl.ObtenerListadoTransportistaChoferParaCombo(ref objOperationResult, null, null, ""), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboTipoGuia, "Value2", "Id", _listadoComboGuias, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboIGV, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForCombo(ref objOperationResult, 27, null), DropDownListAction.Select);

            // Utils.Windows.LoadUltraComboEditorList(cboIGV, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 27, null), DropDownListAction.Select);
            var xx = ((int)TiposDocumentos.GuiaRemision).ToString();
            cboDocumentoRef.Value = "1";
            cboModalidad.Value = "1";
            cboEstablecimiento.Enabled = false;
            cboTipoGuia.Value = "9";
            //var almacenes = new EstablecimientoBL().GetAlmacenesXEstablecimiento(int.Parse(Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString ()));
            //Utils.Windows.LoadUltraComboEditorList(cboAlmacenLlegada, "Value1", "Id", almacenes, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacenLlegada, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaComboAll(ref objOperationResult, "i_IdAlmacen!=" + Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString()), DropDownListAction.Select);
            cboAlmacenLlegada.Value = "-1";
            #endregion
            CargarComboDetalle();
            ObtenerListadoGuiaRemision(txtPeriodo.Text, txtMes.Text);
            dtpFechaEmision.MaxDate = DateTime.Parse(txtPeriodo.Text + "/12/" + DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim())) + " 23:59");
            FormatoDecimalesGrilla(Globals.ClientSession.i_CantidadDecimales ?? 2, Globals.ClientSession.i_PrecioDecimales ?? 2);
            serieDocPrevio = txtSerieDoc.Text.Trim();
            _tipoDocumentoPrevio = int.Parse(cboDocumentoRef.Value.ToString());
            ConfiguracionGrilla();

            _scroll = int.Parse(dtpFechaEmision.Value.Month.ToString());
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();
            if (Application.OpenForms["frmMaster"] != null) ((frmMaster)Application.OpenForms["frmMaster"]).Activate();

            #region Txt Ruc
            txtRucCliente.LoadConfig("C");
            txtRucCliente.ItemSelectedAfterDropClosed += delegate
            {
                txtRucCliente_KeyDown(null, new KeyEventArgs(Keys.Enter));
            };
            txtRucTransportista.LoadConfig("", "TR");
            txtRucTransportista.ItemSelectedAfterDropClosed += delegate
            {
                txtRucTransportista_KeyDown(null, new KeyEventArgs(Keys.Enter));
            };
            #endregion
        }

        #region Temporales Guia Remision
        List<guiaremisiondetalleDto> _TempDetalle_AgregarDto = new List<guiaremisiondetalleDto>();
        List<guiaremisiondetalleDto> _TempDetalle_ModificarDto = new List<guiaremisiondetalleDto>();
        List<guiaremisiondetalleDto> _TempDetalle_EliminarDto = new List<guiaremisiondetalleDto>();
        #endregion

        # region  BarraNavegacion
        private void EdicionBarraNavegacion(bool ON_OFF)
        {
            // txtCorrelativo.Enabled = ON_OFF;
            btnAnterior.Enabled = ON_OFF;
            btnSiguiente.Enabled = ON_OFF;
            txtMes.Enabled = ON_OFF;
            txtPeriodo.Enabled = ON_OFF;
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_listadoGuiaRemision.Count() > 0)
            {
                if (_maxV == 0) CargarCabecera(_listadoGuiaRemision[0].Value2);

                if (_actV > 0 && _actV <= _maxV)
                {
                    _actV = _actV - 1;
                    txtCorrelativo.Text = _listadoGuiaRemision[_actV].Value1;
                    CargarCabecera(_listadoGuiaRemision[_actV].Value2);
                }
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_actV >= 0 && _actV < _maxV)
            {
                _actV = _actV + 1;
                txtCorrelativo.Text = _listadoGuiaRemision[_actV].Value1;
                CargarCabecera(_listadoGuiaRemision[_actV].Value2);
            }
        }
        #endregion

        #region Cabecera
        private void ObtenerListadoGuiaRemision(string pstrPeriodo, string pstrMes)
        {
            OperationResult objOperationResult = new OperationResult();
            _listadoGuiaRemision = _objGuiaRemisionBl.ObtenerListadoGuiaRemision(ref objOperationResult, pstrPeriodo, pstrMes);
            switch (_strModo)
            {
                case "Edicion":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(_strIdGuiaRemision);

                    break;

                case "Nuevo":
                    if (_listadoGuiaRemision.Count != 0)
                    {
                        _maxV = _listadoGuiaRemision.Count() - 1;
                        _actV = _maxV;
                        LimpiarCabecera();
                        CargarDetalle(""); //Le mando 1 solo acá porque es nuevo
                        // txtCorrelativo.Text = (int.Parse(_ListadoGuiaRemision[_MaxV].Value1) + 1).ToString("00000000");
                        _mode = "New";
                        _guiaremisionDto = new guiaremisionDto();

                    }
                    else
                    {
                        // txtCorrelativo.Text = "00000001";
                        _mode = "New";
                        LimpiarCabecera();
                        CargarDetalle("");
                        _maxV = 1;
                        _actV = 1;
                        _guiaremisionDto = new guiaremisionDto();
                        btnNuevoMovimiento.Enabled = false;


                    }
                    txtMes.Text = pstrMes;
                    cboEstado.Value = "1";
                    cboMotivos.Value = "1";
                    txtTipoCambio.Text = _objGuiaRemisionBl.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);

                    if (!string.IsNullOrEmpty(UserConfig.Default.SerieCaja) && UserConfig.Default.SerieCaja != "--Seleccionar--")
                    {
                        txtSerieGuia.Text = UserConfig.Default.SerieCaja;
                    }
                    else
                    {
                        txtSerieGuia.Text = txtSerieGuia.Text = _objDocumentoBl.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboTipoGuia.Value.ToString())).Trim();
                    }
                    txtNumeroGuia.Text = _objDocumentoBl.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboTipoGuia.Value.ToString())).ToString("D8");
                    btnImprimir.Enabled = false;
                    EdicionBarraNavegacion(false);
                    ComprobarExistenciaCorrelativoDocumento();
                    txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.GuiaRemision, _guiaremisionDto.t_FechaEmision, dtpFechaEmision.Value, _guiaremisionDto.v_Correlativo, 0);
                    object o1 = new object();
                    CancelEventArgs e1 = new CancelEventArgs();
                    txtSerieGuia_Validating(o1, e1);
                    break;

                case "Guardado":
                    _maxV = _listadoGuiaRemision.Count() - 1;
                    _actV = _maxV;
                    if (_strIdGuiaRemision == "" | _strIdGuiaRemision == null)
                    {
                        CargarCabecera(_listadoGuiaRemision[_maxV].Value2);
                    }
                    else
                    {
                        CargarCabecera(_strIdGuiaRemision);
                    }
                    btnNuevoMovimiento.Enabled = true;
                    EdicionBarraNavegacion(false);
                    break;

                case "Consulta":
                    if (_listadoGuiaRemision.Count != 0)
                    {
                        _maxV = _listadoGuiaRemision.Count() - 1;
                        _actV = _maxV;
                        txtCorrelativo.Text = (int.Parse(_listadoGuiaRemision[_maxV].Value1)).ToString("00000000");
                        CargarCabecera(_listadoGuiaRemision[_maxV].Value2);
                        _mode = "Edit";

                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _mode = "New";
                        LimpiarCabecera();
                        CargarDetalle("");
                        _maxV = 1;
                        _actV = 1;
                        _guiaremisionDto = new guiaremisionDto();
                        btnNuevoMovimiento.Enabled = false;
                        _objGuiaRemisionBl.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);

                        txtMes.Enabled = true;
                    }
                    EdicionBarraNavegacion(false);
                    break;
            }

        }

        private void ComprobarExistenciaCorrelativoDocumento()
        {
            OperationResult objOperationResult = new OperationResult();
            if (_objGuiaRemisionBl.ComprobarExistenciaCorrelativoDocumento(ref objOperationResult, 9, txtSerieDoc.Text, txtNumeroDoc.Text, _guiaremisionDto.v_IdGuiaRemision) == true)
            {
                UltraMessageBox.Show("El documento ya ha sido registrado anteriormente", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGuardar.Enabled = false;
            }
            else
            {

                btnGuardar.Enabled = _btnGuardar;
            }
        }

        private void LimpiarCabecera()
        {
            txtSerieGuia.Clear();
            txtNumeroGuia.Clear();
            txtSerieDoc.Clear();
            txtNumeroDoc.Clear();
            txtPartida.Clear();
            txtLlegada.Clear();
            txtNumeroCotizacion.Clear();
            txtTipoCambio.Text = "";
            cboEstado.Value = "-1";
            cboMotivos.Value = "-1";
            txtRucTransportista.Clear();
            txtRazonSocial.Clear();
            txtCodigo.Clear();
            cboPlaca.Value = "-1";
            cboChofer.Value = "-1";
            txtNumInscripcion.Clear();
            txtNombreChofer.Clear();
            txtRucAgencia.Clear();
            dtpFechaEmision.Value = DateTime.Now.Date;// DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            dtpFechaTraslado.Value = DateTime.Now.Date; // DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            cboRazonAgencia.Value = "-1";
            txtDireccion.Clear();
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            txtRucCliente.Clear();
            txtCodigoCliente.Clear();
            txtRazonCliente.Clear();
            txtLlegada.Clear();
            var partida = _objGuiaRemisionBl.ObtenerDatosEmpresa();
            txtPartida.Text = partida.Item1;
            txtUbigueoPartida.Text = partida.Item2;

            txtSerieGuia.Enabled = true;
            txtNumeroGuia.Enabled = true;
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.Value.ToString();
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            uckAfectoIgv.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1 ? true : false;
            uckPreciosIncluyenIgv.Checked = true;
            cboIGV.Value = Globals.ClientSession.i_IdIgv.ToString();
            _guiaremisionDto.v_IdCliente = null;



        }

        private void CargarCabecera(string idmovimiento)
        {
            var objOperationResult = new OperationResult();
            _guiaremisionDto = _objGuiaRemisionBl.ObtenerGuiaRemisionCabecera(ref objOperationResult, idmovimiento);
            if (_guiaremisionDto != null)
            {
                _mode = "Edit";
                txtCorrelativo.Text = _guiaremisionDto.v_Correlativo;
                txtMes.Text = _guiaremisionDto.v_Mes;
                txtSerieGuia.Text = _guiaremisionDto.v_SerieGuiaRemision;
                txtNumeroGuia.Text = _guiaremisionDto.v_NumeroGuiaRemision;
                dtpFechaEmision.Value = _guiaremisionDto.t_FechaEmision.Value;
                txtTipoCambio.Text = _guiaremisionDto.d_TipoCambio.ToString();
                dtpFechaTraslado.Value = _guiaremisionDto.t_FechaTraslado.Value;
                txtSerieDoc.Text = _guiaremisionDto.v_SerieDocumentoRef;
                txtNumeroDoc.Text = _guiaremisionDto.v_NumeroDocumentoRef.Trim();
                txtNumeroCotizacion.Text = _guiaremisionDto.v_NumeroPedidoCotizacion;
                cboEstado.Value = _guiaremisionDto.i_IdEstado.ToString();
                txtPartida.Text = _guiaremisionDto.v_PuntoPartida;
                txtLlegada.Text = _guiaremisionDto.v_PuntoLLegada;
                cboMotivos.Value = _guiaremisionDto.i_IdMotivoTraslado.ToString();
                cboMoneda.Value = _guiaremisionDto.i_IdMoneda.ToString();
                txtRazonSocial.Text = _guiaremisionDto.NombreTransportista;
                txtCodigo.Text = _guiaremisionDto.CodTransportista;
                txtRucTransportista.Text = _guiaremisionDto.RucTransportista;
                cboDocumentoRef.Value = _guiaremisionDto.i_IdTipoDocumento.ToString();
                if (_guiaremisionDto.v_IdTransportista != null)
                {
                    Utils.Windows.LoadUltraComboEditorList(cboPlaca, "Value1", "Id", _objTransporteUnidadTransporte.ObtenerListadoTransportistaUnidadTransporteParaCombo(ref objOperationResult, null, null, _guiaremisionDto.v_IdTransportista), DropDownListAction.Select);
                    Utils.Windows.LoadUltraComboEditorList(cboChofer, "Value1", "Id", _objTransportistaChoferBl.ObtenerListadoTransportistaChoferParaCombo(ref objOperationResult, null, null, _guiaremisionDto.v_IdTransportista), DropDownListAction.Select);
                }
                cboEstablecimiento.Value = _guiaremisionDto.i_IdEstablecimiento.Value.ToString();
                cboPlaca.Value = _guiaremisionDto.v_IdUnidadTransporte ?? "-1";
                cboChofer.Value = _guiaremisionDto.v_IdChofer ?? "-1";
                cboRazonAgencia.Value = _guiaremisionDto.v_IdAgenciaTransporte ?? "-1";
                txtRucCliente.Text = _guiaremisionDto.NroDocCliente;
                txtCodigoCliente.Text = _guiaremisionDto.CodigoCliente;
                txtRazonCliente.Text = _guiaremisionDto.NombreCliente;
                txtRedondeo.Text = decimal.Parse(_guiaremisionDto.d_Redondeo.ToString()).ToString("0.00");
                _redondeo = _guiaremisionDto.d_Redondeo.Value;
                txtSerieGuia.Enabled = false;
                txtNumeroGuia.Enabled = false;
                cboTipoGuia.Enabled = false;
                cboTipoGuia.Value = _guiaremisionDto.i_IdTipoGuia.ToString();
                _extraidoDocumentoReferencia = int.Parse(cboTipoGuia.Value.ToString()) != (int)TiposDocumentos.GuiaInterna;
                cboIGV.Value = _guiaremisionDto.i_IdIgv.ToString();
                uckAfectoIgv.Checked = _guiaremisionDto.i_AfectoIgv != null && _guiaremisionDto.i_AfectoIgv != 0;
                uckPreciosIncluyenIgv.Checked = _guiaremisionDto.i_PrecionInclIgv != null && _guiaremisionDto.i_PrecionInclIgv != 0;
                txtTotal.Text = decimal.Parse(_guiaremisionDto.d_Total.ToString()).ToString("0.00");
                cboAlmacenLlegada.Value = _guiaremisionDto.i_IdAlmacenDestino.Value.ToString() ?? "-1";
                txtUbigueoLlegada.Text = _guiaremisionDto.v_UbigueoLlegada;
                txtUbigueoPartida.Text = _guiaremisionDto.v_UbigueoPartida;
                cboModalidad.Value = (_guiaremisionDto.i_Modalidad ?? 1).ToString();
                txtTotalPeso.Text = (_guiaremisionDto.d_TotalPeso ?? 0).ToString(CultureInfo.InvariantCulture);
                CargarDetalle(_guiaremisionDto.v_IdGuiaRemision);
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar la guia", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void ExistenciaGuiaRemision()
        {
            OperationResult objOperationResult = new OperationResult();

            if (_objGuiaRemisionBl.ObtenerExistenciaGuiaRemision(objOperationResult, txtSerieGuia.Text.Trim(), txtNumeroGuia.Text.Trim(), _guiaremisionDto.v_IdGuiaRemision, int.Parse(cboTipoGuia.Value.ToString())))
            {
                UltraMessageBox.Show("El numero de Guia ya fue registrado anteriormente", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnGuardar.Enabled = false;
                return;
            }
            else
            {
                btnGuardar.Enabled = _btnGuardar;
            }
        }

        private bool ValidaCamposNulosVacios()
        {
            if (grdData.Rows.Where(p => p.Cells["v_IdProductoDetalle"].Value == null || p.Cells["v_IdProductoDetalle"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente los productos", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(x => x.Cells["v_IdProductoDetalle"].Value == null || x.Cells["v_IdProductoDetalle"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["v_CodInterno"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_CodInterno"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (Globals.ClientSession.i_IncluirPreciosGuiaRemision.ToString() == "1") //
            {
                if (grdData.Rows.Where(p => p.Cells["d_Precio"].Value == null || p.Cells["d_Precio"].Value.ToString().Trim() == string.Empty || decimal.Parse(p.Cells["d_Precio"].Value.ToString()) <= 0).Count() != 0)
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente los precios", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdData.Rows.Where(x => x.Cells["d_Precio"].Value == null || x.Cells["d_Precio"].Value.ToString().Trim() == string.Empty || decimal.Parse(x.Cells["d_Precio"].Value.ToString()) <= 0).FirstOrDefault();
                    grdData.Selected.Cells.Add(Row.Cells["d_Precio"]);
                    grdData.Focus();
                    Row.Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_Precio"];
                    this.grdData.ActiveCell = aCell;
                    return false;
                }
            }

            if (grdData.Rows.Where(p => p.Cells["d_Cantidad"].Value == null || p.Cells["d_Cantidad"].Value.ToString().Trim() == string.Empty || decimal.Parse(p.Cells["d_Cantidad"].Value.ToString()) <= 0).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente las cantidades.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        #endregion

        #region Detalles

        private void CargarComboDetalle()
        {
            OperationResult objOperationResult = new OperationResult();
            #region Configura Combo Tipo de Bultos
            UltraGridBand ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumnaID = new UltraGridColumn("Id");
            UltraGridColumn ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion.Width = 267;
            ultraGridColumnaID.Hidden = true;
            ultraGridBanda.Columns.AddRange(new object[] { ultraGridColumnaDescripcion, ultraGridColumnaID });
            ucbTipoBultos.DisplayLayout.BandsSerializer.Add(ultraGridBanda);
            ucbTipoBultos.DropDownWidth = 270;
            ucbTipoBultos.DropDownStyle = UltraComboStyle.DropDownList;
            #endregion

            #region Configura Combo Unidad Medida
            UltraGridBand ultraGridBanda_ = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumnaID_ = new UltraGridColumn("Id");
            UltraGridColumn ultraGridColumnaDescripcion_ = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion_.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion_.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion_.Width = 267;
            ultraGridColumnaID_.Hidden = true;
            ultraGridBanda_.Columns.AddRange(new object[] { ultraGridColumnaDescripcion_, ultraGridColumnaID_ });
            ucbUnidadMedida.DisplayLayout.BandsSerializer.Add(ultraGridBanda_);
            ucbUnidadMedida.DropDownWidth = 270;
            ucbUnidadMedida.DropDownStyle = UltraComboStyle.DropDownList;
            #endregion

            #region Configura Combo Unidad Empaque
            UltraGridBand _ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn _ultraGridColumnaID_ = new UltraGridColumn("Id");
            UltraGridColumn _ultraGridColumnaDescripcion_ = new UltraGridColumn("Value1");
            _ultraGridColumnaDescripcion_.Header.Caption = "Descripción";
            _ultraGridColumnaDescripcion_.Header.VisiblePosition = 0;
            _ultraGridColumnaDescripcion_.Width = 267;
            _ultraGridColumnaID_.Hidden = true;
            _ultraGridBanda.Columns.AddRange(new object[] { _ultraGridColumnaDescripcion_, _ultraGridColumnaID_ });
            ucbUnidadEmpaque.DisplayLayout.BandsSerializer.Add(_ultraGridBanda);
            ucbUnidadEmpaque.DropDownWidth = 270;
            ucbUnidadEmpaque.DropDownStyle = UltraComboStyle.DropDownList;
            #endregion


            #region Configura Combo Almacen

            UltraGridBand _ultraGridBanda1 = new UltraGridBand("Band 0", -1);
            UltraGridColumn _ultraGridColumnaID1 = new UltraGridColumn("Id");
            UltraGridColumn _ultraGridColumnaDescripcion1 = new UltraGridColumn("Value1");
            _ultraGridColumnaID1.Header.Caption = "Cod.";

            _ultraGridColumnaID1.Width = 50;
            _ultraGridColumnaDescripcion1.Header.Caption = "Descripción";
            _ultraGridColumnaDescripcion1.Header.VisiblePosition = 0;
            _ultraGridColumnaDescripcion1.Width = 280;
            //_ultraGridColumnaID1.Hidden = true;
            _ultraGridBanda1.Columns.AddRange(new object[] { _ultraGridColumnaDescripcion1, _ultraGridColumnaID1 });
            ucbAlmacen.DisplayLayout.BandsSerializer.Add(_ultraGridBanda1);
            ucbAlmacen.DropDownWidth = 270;
            ucbAlmacen.DropDownStyle = UltraComboStyle.DropDownList;
            #endregion

            ucbTipoBultos.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            ucbUnidadMedida.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            var objDatahierarchyBl = new DatahierarchyBL();
            Utils.Windows.LoadUltraComboList(ucbTipoBultos, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForComboGrid(ref objOperationResult, 34, null), DropDownListAction.Select); //34-Tipo de Bultos
            Utils.Windows.LoadUltraComboList(ucbUnidadMedida, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForComboGrid(ref objOperationResult, 17, null), DropDownListAction.Select); //17-Unidad Medida 
            Utils.Windows.LoadUltraComboList(ucbUnidadEmpaque, "Value1", "Id", objDatahierarchyBl.GetDataHierarchiesForComboGrid(ref objOperationResult, 17, null), DropDownListAction.Select); //17-Unidad Medida Empaque
            Utils.Windows.LoadUltraComboList(ucbAlmacen, "Id", "Id", new NodeWarehouseBL().ObtenerAlmacenesParaComboGrid(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);

        }

        private void CargarDetalle(string pstringIdGuiaRemision)
        {

            var objOperationResult = new OperationResult();
            grdData.DataSource = _objGuiaRemisionBl.ObtenerGuiaRemisionDetalles(ref objOperationResult, pstringIdGuiaRemision);

            BuscarProductos();

            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
                grdData.Rows[i].Cells["i_EsServicio"].Value = grdData.Rows[i].Cells["v_IdProductoDetalle"].Value == null ? 0 : _objGuiaRemisionBl.EsServicio(grdData.Rows[i].Cells["v_IdProductoDetalle"].Value.ToString());

            }
            CalcularValores();
            ConfiguracionGrilla();
        }


        private void BuscarProductos()
        {
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                if (grdData.Rows[i].Cells["v_IdProductoDetalle"].Value != null)
                {
                    string[] Cadena = new string[4];
                    Cadena = _objGuiaRemisionBl.DevolverProductos(grdData.Rows[i].Cells["v_IdProductoDetalle"].Value.ToString());
                    grdData.Rows[i].Cells["v_CodInterno"].Value = Cadena[0];
                    grdData.Rows[i].Cells["v_Descripcion"].Value = Cadena[1];
                    grdData.Rows[i].Cells["Empaque"].Value = Cadena[2];
                    grdData.Rows[i].Cells["UMEmpaque"].Value = Cadena[3];
                    grdData.Rows[i].Cells["i_IdUnidadMedidaProducto"].Value = Cadena[4];

                }
            }
        }

        private void BuscarProductosTemporal()
        {
            for (int i = 0; i < grdData.Rows.Count; i++)
            {
                if (grdData.Rows[i].Cells["v_IdProductoDetalle"].Value != null)
                {
                    var Cadena = _objGuiaRemisionBl.DevolverProductos(grdData.Rows[i].Cells["v_IdProductoDetalle"].Value.ToString());
                    grdData.Rows[i].Cells["v_CodInterno"].Value = Cadena[0];
                    grdData.Rows[i].Cells["v_Descripcion"].Value = Cadena[1];
                    grdData.Rows[i].Cells["Empaque"].Value = Cadena[2];
                    grdData.Rows[i].Cells["UMEmpaque"].Value = Cadena[3];
                    //  grdData.Rows[i].Cells["i_IdUnidadMedida"].Value = Cadena[4];
                    grdData.Rows[i].Cells["i_IdUnidadMedidaProducto"].Value = Cadena[4];
                    //grdData.Rows[i].Cells["d_CantidadEmpaque"].Value = Cadena[4];
                }
            }
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
                                _guiaremisiondetalleDto = new guiaremisiondetalleDto();
                                _guiaremisiondetalleDto.v_IdGuiaRemision = _guiaremisionDto.v_IdGuiaRemision;
                                _guiaremisiondetalleDto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _guiaremisiondetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _guiaremisiondetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _guiaremisiondetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _guiaremisiondetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                _guiaremisiondetalleDto.d_Total = Fila.Cells["d_Total"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Total"].Text);
                                _guiaremisiondetalleDto.d_CantidadBulto = Fila.Cells["d_CantidadBulto"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadBulto"].Value.ToString());
                                _guiaremisiondetalleDto.i_IdTipoBulto = Fila.Cells["i_IdTipoBulto"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdTipoBulto"].Value.ToString());
                                _guiaremisiondetalleDto.v_Observacion = Fila.Cells["v_Observacion"].Value == null ? null : Fila.Cells["v_Observacion"].Value.ToString();
                                _guiaremisiondetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _guiaremisiondetalleDto.v_IdMovimientoDetalle = Fila.Cells["v_IdMovimientoDetalle"].Value == null ? null : Fila.Cells["v_IdMovimientoDetalle"].Value.ToString();
                                _guiaremisiondetalleDto.i_EsServicio = Fila.Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Fila.Cells["i_EsServicio"].Value.ToString());
                                _guiaremisiondetalleDto.v_CodInterno = Fila.Cells["v_CodInterno"].Value == null ? "" : Fila.Cells["v_CodInterno"].Value.ToString();
                                _guiaremisiondetalleDto.v_Descripcion = Fila.Cells["v_Descripcion"].Value == null ? "" : Fila.Cells["v_Descripcion"].Value.ToString();
                                _guiaremisiondetalleDto.d_Valor = Fila.Cells["d_Valor"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Valor"].Text);

                                _guiaremisiondetalleDto.v_Descuento = Fila.Cells["v_Descuento"].Value == null ? "" : Fila.Cells["v_Descuento"].Value.ToString();
                                _guiaremisiondetalleDto.d_Descuento = Fila.Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Descuento"].Text);

                                _guiaremisiondetalleDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Text);

                                _guiaremisiondetalleDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Text);

                                _TempDetalle_AgregarDto.Add(_guiaremisiondetalleDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _guiaremisiondetalleDto = new guiaremisiondetalleDto();
                                _guiaremisiondetalleDto.v_IdGuiaRemision = Fila.Cells["v_IdGuiaRemision"].Value == null ? null : Fila.Cells["v_IdGuiaRemision"].Value.ToString();
                                _guiaremisiondetalleDto.v_IdGuiaRemisionDetalle = Fila.Cells["v_IdGuiaRemisionDetalle"].Value == null ? null : Fila.Cells["v_IdGuiaRemisionDetalle"].Value.ToString();
                                _guiaremisiondetalleDto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _guiaremisiondetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _guiaremisiondetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _guiaremisiondetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _guiaremisiondetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _guiaremisiondetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                _guiaremisiondetalleDto.d_Total = Fila.Cells["d_Total"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Total"].Text);
                                _guiaremisiondetalleDto.d_CantidadBulto = Fila.Cells["d_CantidadBulto"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadBulto"].Value.ToString());
                                _guiaremisiondetalleDto.i_IdTipoBulto = Fila.Cells["i_IdTipoBulto"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdTipoBulto"].Value.ToString());
                                _guiaremisiondetalleDto.v_Observacion = Fila.Cells["v_Observacion"].Value == null ? null : Fila.Cells["v_Observacion"].Value.ToString();
                                _guiaremisiondetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _guiaremisiondetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _guiaremisiondetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _guiaremisiondetalleDto.v_IdMovimientoDetalle = Fila.Cells["v_IdMovimientoDetalle"].Value == null ? null : Fila.Cells["v_IdMovimientoDetalle"].Value.ToString();
                                _guiaremisiondetalleDto.i_EsServicio = Fila.Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Fila.Cells["i_EsServicio"].Value.ToString());
                                _guiaremisiondetalleDto.v_CodInterno = Fila.Cells["v_CodInterno"].Value == null ? "" : Fila.Cells["v_CodInterno"].Value.ToString();
                                _guiaremisiondetalleDto.v_Descripcion = Fila.Cells["v_Descripcion"].Value == null ? "" : Fila.Cells["v_Descripcion"].Value.ToString();
                                _guiaremisiondetalleDto.d_Valor = Fila.Cells["d_Valor"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Valor"].Text);

                                _guiaremisiondetalleDto.d_Valor = Fila.Cells["d_Valor"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Valor"].Text);

                                _guiaremisiondetalleDto.v_Descuento = Fila.Cells["v_Descuento"].Value == null ? "" : Fila.Cells["v_Descuento"].Value.ToString();
                                _guiaremisiondetalleDto.d_Descuento = Fila.Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Descuento"].Text);

                                _guiaremisiondetalleDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Text);

                                _guiaremisiondetalleDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Text);


                                _TempDetalle_ModificarDto.Add(_guiaremisiondetalleDto);
                            }
                            break;
                    }
                }
            }

        }

        #endregion

        #region Grilla
        private void ConfiguracionGrilla()
        {
            UltraGridColumn d_Precio = grdData.DisplayLayout.Bands[0].Columns["d_Precio"];
            UltraGridColumn d_Total = grdData.DisplayLayout.Bands[0].Columns["d_Total"]; //PrecioVenta
            UltraGridColumn d_Valor = grdData.DisplayLayout.Bands[0].Columns["d_Valor"];
            UltraGridColumn v_Descuento = grdData.DisplayLayout.Bands[0].Columns["v_Descuento"];
            UltraGridColumn d_Descuento = grdData.DisplayLayout.Bands[0].Columns["d_Descuento"];
            UltraGridColumn d_ValorVenta = grdData.DisplayLayout.Bands[0].Columns["d_ValorVenta"];
            UltraGridColumn d_Igv = grdData.DisplayLayout.Bands[0].Columns["d_Igv"];

            if (Globals.ClientSession.i_IncluirPreciosGuiaRemision.ToString() == "1")
            {
                if ((_extraidoDocumentoReferencia || (cboDocumentoRef.Value != null && cboDocumentoRef.Value.ToString() != "-1")) && Globals.ClientSession.v_RucEmpresa != Constants.RucHormiguita)
                {

                    d_Precio.Hidden = true;
                    d_Total.Hidden = true;
                    d_Valor.Hidden = true;
                    v_Descuento.Hidden = true;
                    d_Descuento.Hidden = true;
                    d_ValorVenta.Hidden = true;
                    d_Igv.Hidden = true;
                    lblTotal.Visible = false;
                    txtTotal.Visible = false;
                    //uckAfectoIgv.Visible = false;
                    //uckPreciosIncluyenIgv.Visible = false;
                    cboIGV.Visible = false;
                    lblIgv.Visible = false;
                }
                else
                {

                    d_Total.Hidden = false;
                    d_Precio.Hidden = false;
                    d_Valor.Hidden = false;
                    v_Descuento.Hidden = false;
                    d_Descuento.Hidden = false;
                    d_ValorVenta.Hidden = false;
                    d_Igv.Hidden = false;
                    lblTotal.Visible = true;
                    txtTotal.Visible = true;
                    //uckAfectoIgv.Visible = true;
                    //uckPreciosIncluyenIgv.Visible = true;
                    cboIGV.Visible = true;
                    lblIgv.Visible = true;
                }

            }
            else
            {


                d_Precio.Hidden = true;
                d_Total.Hidden = true;

                d_Valor.Hidden = true;
                v_Descuento.Hidden = true;
                d_Descuento.Hidden = true;
                d_ValorVenta.Hidden = true;
                d_Igv.Hidden = true;

                lblTotal.Visible = false;
                txtTotal.Visible = false;
                //uckAfectoIgv.Visible = false;
                //uckPreciosIncluyenIgv.Visible = false;
                cboIGV.Visible = false;
                lblIgv.Visible = false;

            }
            grpDatosAgenciaTransporte.Visible = Globals.ClientSession.i_IncluirAgeciaTransporteGuiaRemision.ToString() == "1" ? true : false;
            lblAlmacenLlegada.Visible = cboAlmacenLlegada.Visible = Globals.ClientSession.i_IncluirAlmacenDestinoGuiaRemisionVenta.ToString() == "1" ? true : false;




        }
        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {

            Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(-1, "PedidoVenta", string.Empty, string.Empty, TipoBusquedaProductos.Ambos);
            var RucEmpresa = new NodeBL().ReporteEmpresa().FirstOrDefault().RucEmpresaPropietaria.Trim();
            int idAlmacen;
            if (e.Cell.Column.Key == "v_CodInterno")
            {
                idAlmacen = Convert.ToInt32(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdAlmacen"].Value);
                if (idAlmacen == -1)
                {
                    UltraMessageBox.Show("Porfavor seleccione un almacén antes de buscar un producto ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {


                    if (RucEmpresa == Constants.RucManguifajas)
                    {
                        frm = new Mantenimientos.frmBuscarProducto(idAlmacen, "PedidoVenta", null, grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Text == null ? string.Empty : grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Text.ToString());
                        frm.ShowDialog();
                    }
                    else
                    {

                        //Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(idAlmacen, "Salida", null, grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Text == null ? string.Empty : grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Text.ToString());
                        frm = new Mantenimientos.frmBuscarProducto(idAlmacen, "Salida", null, grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Text == null ? string.Empty : grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Text.ToString());
                        frm.ShowDialog();
                    }

                    if (frm._NombreProducto != null)
                    {
                        foreach (UltraGridRow Fila in grdData.Rows)
                        {
                            if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                            {

                                if (frm._IdProducto == Fila.Cells["v_IdProductoDetalle"].Value.ToString() & idAlmacen == int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()))
                                {
                                    UltraMessageBox.Show("El producto ya existe para este almacén ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return;
                                }
                            }
                        }



                        grdData.ActiveRow.Cells["i_EsServicio"].Value = int.Parse(frm._EsServicio.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value = frm._NombreProducto.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = frm._IdProducto.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Value = frm._CodigoInternoProducto.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["Empaque"].Value = frm._Empaque != null ? frm._Empaque.ToString() : string.Empty;
                        grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = frm._UnidadMedida != null ? frm._UnidadMedida.ToString() : string.Empty;
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = frm._UnidadMedidaEmpaque != null ? frm._UnidadMedidaEmpaque.ToString() : string.Empty;  //Por defecto ,pero si desea el usuario lo puede cambiar
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = frm._UnidadMedidaEmpaque != null ? frm._UnidadMedidaEmpaque.ToString() : null;
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = "1";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdTipoBulto"].Value = "-1";
                        // grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value = "0";

                        grdData.ActiveRow.Cells["d_Precio"].Value = DevolverPrecioProducto(frm._IdMoneda, frm._PrecioUnitario == null ? 0 : frm._PrecioUnitario, int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);


                    }

                    UltraGridCell aCell = grdData.Rows[e.Cell.Row.Index].Cells["d_Cantidad"];
                    grdData.Rows[e.Cell.Row.Index].Activate();
                    grdData.ActiveCell = aCell;
                    grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                    grdData.Focus();
                }

            }
        }
        private decimal DevolverPrecioProducto(int pintIdMonedaBusqueda, decimal pdecPrecio, int pintIdMonedaVenta, decimal pdecTipoCambioVenta)
        {
            try
            {
                if (pintIdMonedaBusqueda != -1 && pintIdMonedaBusqueda != pintIdMonedaVenta)
                {
                    switch (pintIdMonedaVenta)
                    {
                        case 1:
                            return Utils.Windows.DevuelveValorRedondeado(pdecPrecio * pdecTipoCambioVenta, 2);

                        case 2:
                            return Utils.Windows.DevuelveValorRedondeado(pdecPrecio / pdecTipoCambioVenta, 2);
                    }
                }
                else if (pintIdMonedaBusqueda == 0)
                {

                    return 0;
                }

                return pdecPrecio;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].EditorComponent = ucbUnidadMedida;
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].EditorComponent = ucbAlmacen;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdTipoBulto"].EditorComponent = ucbTipoBultos;
            e.Layout.Bands[0].Columns["i_IdTipoBulto"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            // e.Layout.Bands[0].Columns["i_IdUnidadEmpaque"].EditorComponent = ucbUnidadEmpaque;
        }

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            if (grdData.ActiveCell == null || grdData.ActiveCell.Column == null || grdData.ActiveCell.Column.Key == null) { return; }
            switch (this.grdData.ActiveCell.Column.Key)
            {

                case "v_Descuento":

                    if (grdData.ActiveCell.Value != null)
                    {
                        grdData.ActiveCell.Value = Utils.Windows.DarFormatoDescuentoUnDecimal(grdData.ActiveCell.Value.ToString());
                        CalcularValoresFila(grdData.ActiveRow);
                    }
                    else
                        grdData.ActiveCell.Value = "0";

                    break;
            }

            CalcularValoresFila(grdData.Rows[grdData.ActiveRow.Index]);


        }
        private void CalcularValoresFila(UltraGridRow Fila)
        {

            if (_extraidoDocumentoReferencia)
            {
                decimal cantidad;
                decimal precio, total;
                if (Fila.Cells["d_Cantidad"].Value == null) { Fila.Cells["d_Cantidad"].Value = "0"; }
                if (Fila.Cells["d_Precio"].Value == null) { Fila.Cells["d_Precio"].Value = "0"; }
                cantidad = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                precio = decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                total = cantidad * precio;
                Fila.Cells["d_Total"].Value = total;
                CalcularValores();
            }
            else
            {
                if (cboIGV.Value.ToString() != "-1")
                {
                    if (Fila.Cells["d_Cantidad"].Value == null) { Fila.Cells["d_Cantidad"].Value = "0"; }
                    if (Fila.Cells["d_Precio"].Value == null) { Fila.Cells["d_Precio"].Value = "0"; }
                    if (Fila.Cells["d_ValorVenta"].Value == null) { Fila.Cells["d_ValorVenta"].Value = "0"; }
                    if (Fila.Cells["d_Total"].Value == null) { Fila.Cells["d_Total"].Value = "0"; }
                    if (Fila.Cells["d_Descuento"].Value == null) { Fila.Cells["d_Descuento"].Value = "0"; }
                    if (Fila.Cells["v_Descuento"].Value == null || Fila.Cells["v_Descuento"].Value == string.Empty) { Fila.Cells["v_Descuento"].Value = "0"; }

                    var descuentos = Fila.Cells["v_Descuento"].Value.ToString();
                    if (uckAfectoIgv.Checked == false)
                    {

                        if (decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0 && decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) != 0)
                        {

                            Fila.Cells["d_Valor"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Precio"].Value.ToString())), 2).ToString("0.000");
                            Fila.Cells["d_Descuento"].Value = Fila.Cells["v_Descuento"].Value.ToString() != null ? Utils.Windows.CalcularDescuentosSucesivosDecimales(descuentos, decimal.Parse(Fila.Cells["d_Valor"].Value.ToString())) : Fila.Cells["d_Descuento"].Value;
                            Fila.Cells["d_ValorVenta"].Value = decimal.Parse(Fila.Cells["d_Valor"].Value.ToString()) - decimal.Parse(Fila.Cells["d_Descuento"].Value.ToString());
                            Fila.Cells["d_Total"].Value = Fila.Cells["d_ValorVenta"].Value.ToString();
                            Fila.Cells["d_Igv"].Value = "0";
                        }
                        else if (decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) == 0 || decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) == 0) //Agrego 24 febrero 2016
                        {
                            Fila.Cells["d_Valor"].Value = "0";
                            Fila.Cells["d_Igv"].Value = "0";
                            Fila.Cells["d_ValorVenta"].Value = "0";
                        }
                        else
                        {

                            Fila.Cells["d_Valor"].Value = Fila.Cells["d_Total"].Value.ToString();
                            Fila.Cells["d_Igv"].Value = "0";
                            Fila.Cells["d_ValorVenta"].Value = Fila.Cells["d_Total"].Value.ToString();

                        }

                    }
                    else
                    {
                        if (uckPreciosIncluyenIgv.Checked == false)
                        {


                            if (decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) == 0 || decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) == 0)
                            {
                                Fila.Cells["d_Valor"].Value = "0";
                                Fila.Cells["d_Igv"].Value = "0";
                                Fila.Cells["d_ValorVenta"].Value = "0";

                            }

                            if (decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0 && decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) != 0)
                            {
                                Fila.Cells["d_Valor"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Precio"].Value.ToString())), 2).ToString("0.000");
                                Fila.Cells["d_Descuento"].Value = Fila.Cells["v_Descuento"].Value != null ? Utils.Windows.CalcularDescuentosSucesivosDecimales(descuentos, decimal.Parse(Fila.Cells["d_Valor"].Value.ToString())).ToString("0.000") : Fila.Cells["d_Descuento"].Value;

                                Fila.Cells["d_ValorVenta"].Value = decimal.Parse(Fila.Cells["d_Valor"].Value.ToString()) - decimal.Parse(Fila.Cells["d_Descuento"].Value.ToString());
                                Fila.Cells["d_Igv"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString()) * (decimal.Parse(cboIGV.Text) / 100), 2).ToString("0.000");



                                if (Fila.Cells["d_Descuento"].Value.ToString() != "0")
                                {

                                    Fila.Cells["d_Total"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString()) + decimal.Parse(Fila.Cells["d_Igv"].Value.ToString())), 2).ToString("0.000");
                                    Fila.Cells["d_Total"].Value = decimal.Parse(Fila.Cells["d_Total"].Value.ToString());
                                }
                                else
                                {

                                    Fila.Cells["d_Total"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString()) + decimal.Parse(Fila.Cells["d_Igv"].Value.ToString())), 2).ToString("0.000");
                                }

                            }
                            else
                            {


                                Fila.Cells["d_Valor"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Total"].Value.ToString()), 2).ToString("0.000");
                                Fila.Cells["d_ValorVenta"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_Total"].Value.ToString()) / (1 + (decimal.Parse(cboIGV.Text) / 100))), 2).ToString("0.000");
                                Fila.Cells["d_Igv"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Total"].Value.ToString()) - (decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString())), 2).ToString("0.000");
                            }

                        }
                        else
                        {


                            if (decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) == 0 || decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) == 0)
                            {
                                Fila.Cells["d_Valor"].Value = "0";
                                Fila.Cells["d_Igv"].Value = "0";
                                Fila.Cells["d_ValorVenta"].Value = "0";

                            }


                            if (decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0 && decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) != 0)
                            {
                                Fila.Cells["d_Valor"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Precio"].Value.ToString())), 2).ToString("0.000");
                                Fila.Cells["d_Descuento"].Value = Fila.Cells["v_Descuento"].Value.ToString() != null ? Utils.Windows.CalcularDescuentosSucesivosDecimales(descuentos, decimal.Parse(Fila.Cells["d_Valor"].Value.ToString())).ToString("0.000") : Fila.Cells["d_Descuento"].Value;
                                if (decimal.Parse(Fila.Cells["d_Descuento"].Value.ToString()) != 0)
                                {
                                    Fila.Cells["d_Total"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Valor"].Value.ToString()) - decimal.Parse(Fila.Cells["d_Descuento"].Value.ToString()), 2).ToString("0.000");
                                }
                                else
                                {
                                    Fila.Cells["d_Total"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Precio"].Value.ToString())), 2).ToString("0.000");
                                }

                                Fila.Cells["d_ValorVenta"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_Total"].Value.ToString()) / (1 + (decimal.Parse(cboIGV.Text) / 100))), 2).ToString("0.000");
                                Fila.Cells["d_Igv"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Total"].Value.ToString()) - (decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString())), 2).ToString("0.000");
                            }
                            else
                            {


                                Fila.Cells["d_Valor"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Precio"].Value.ToString())), 2).ToString("0.000");
                                Fila.Cells["d_Descuento"].Value = Fila.Cells["v_Descuento"].Value.ToString() != null ? Utils.Windows.CalcularDescuentosSucesivosDecimales(descuentos, decimal.Parse(Fila.Cells["d_Valor"].Value.ToString())) : Fila.Cells["d_Descuento"].Value;

                                if (decimal.Parse(Fila.Cells["d_Descuento"].Value.ToString()) != 0)
                                {

                                    Fila.Cells["d_Total"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Valor"].Value.ToString()) - decimal.Parse(Fila.Cells["d_Descuento"].Value.ToString()), 2).ToString("0.000");
                                }
                                else
                                {


                                    // Fila.Cells["d_Total"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString()), 2);

                                    Fila.Cells["d_Total"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Valor"].Value.ToString()), 2);
                                }
                                Fila.Cells["d_ValorVenta"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_Total"].Value.ToString()) / (1 + (decimal.Parse(cboIGV.Text) / 100))), 2).ToString("0.000");
                                Fila.Cells["d_Igv"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Total"].Value.ToString()) - (decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString())), 2).ToString("0.000");

                            }

                        }
                    }
                    // CalcularTotales();
                    CalcularValores();
                }

                else
                {
                    UltraMessageBox.Show("Porfavor seleccione el IGV", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            EmbeddableEditorBase editor = e.Cell.EditorResolved;

            if (editor.Value != System.DBNull.Value)
            {
                e.Cell.Value = editor.Value;
            }
            grdData.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";

            if (this.grdData.ActiveCell.Column.Key == "i_IdAlmacen")
            {

                int idAlmacen = (grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdAlmacen"].Value != null) ? int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdAlmacen"].Value.ToString()) : -2;
                string codigoProducto = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null ? grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString() : null;

                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                    {
                        if (idAlmacen != -1)
                        {
                            if (idAlmacen == int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()) & Fila.Cells["v_IdProductoDetalle"].Value.ToString() == codigoProducto)
                            {
                                UltraMessageBox.Show("El producto ya existe para este almacén ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }

                    }
                }
            }
        }

        private void CalcularValores()
        {
            decimal Total = 0, CantidadBulto = 0, TotalIncluye = 0;
            if (_extraidoDocumentoReferencia)
            {
                decimal SumTotalR = 0;
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["d_Total"].Value == null) { Fila.Cells["d_Total"].Value = "0"; }
                    if (Fila.Cells["d_CantidadBulto"].Value == null) { Fila.Cells["d_CantidadBulto"].Value = "0"; }
                    SumTotalR = Utils.Windows.DevuelveValorRedondeado(SumTotalR + decimal.Parse(Fila.Cells["d_Total"].Text), 2);
                    CantidadBulto = CantidadBulto + decimal.Parse(Fila.Cells["d_CantidadBulto"].Value.ToString());

                    if (Fila.Cells["i_IdUnidadMedida"].Value != null)
                    {
                        if (Fila.Cells["v_IdProductoDetalle"].Value != null && Fila.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000" && Fila.Cells["i_IdUnidadMedida"].Value.ToString() != "-1" && decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0)
                        {
                            decimal TotalEmpaque = 0;
                            decimal Empaque = decimal.Parse(Fila.Cells["Empaque"].Value.ToString());
                            string Producto = Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                            decimal Cantidad = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                            int UM = int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                            int UMProducto = int.Parse(Fila.Cells["i_IdUnidadMedidaProducto"].Value.ToString());

                            GridKeyValueDTO _UMProducto = ((List<GridKeyValueDTO>)ucbUnidadMedida.DataSource).Where(p => p.Id == UMProducto.ToString()).FirstOrDefault();
                            GridKeyValueDTO _UM = ((List<GridKeyValueDTO>)ucbUnidadMedida.DataSource).Where(p => p.Id == UM.ToString()).FirstOrDefault();

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
                    }
                }
                if (Globals.ClientSession.i_IncluirPreciosGuiaRemision.ToString() == "0")
                {
                    txtTotal.Text = TotalIncluye.ToString("0.00");
                }
                else
                {
                    txtTotal.Text = (SumTotalR + _redondeo).ToString("0.00");
                }
                txtTotalBultos.Text = CantidadBulto.ToString("0.00");
                txtRedondeo.Text = _redondeo.ToString("0.00");

            }
            else
            {

                decimal SumDescuento = 0, SumValVenta = 0, SumIgv = 0, SumTotal = 0, SumVal = 0, CantidadTotal = 0;
                decimal SumDescuentoR = 0, SumValVentaR = 0, SumIgvR = 0, SumTotalR = 0, SumValR = 0, CantidadTotalR = 0;
                decimal SumaValoresVentaR = 0, SumaValoresVenta = 0;
                foreach (UltraGridRow Fila in grdData.Rows)
                {

                    if (Fila.Cells["d_ValorVenta"].Value == null) { Fila.Cells["d_ValorVenta"].Value = "0"; }
                    if (Fila.Cells["d_Igv"].Value == null) { Fila.Cells["d_Igv"].Value = "0"; }
                    if (Fila.Cells["d_Total"].Value == null) { Fila.Cells["d_Total"].Value = "0"; }
                    if (Fila.Cells["d_Descuento"].Value == null) { Fila.Cells["d_Descuento"].Value = "0"; }
                    if (Fila.Cells["d_Valor"].Value == null) { Fila.Cells["d_Valor"].Value = "0"; }
                    if (Fila.Cells["d_Cantidad"].Value == null) { Fila.Cells["d_Cantidad"].Value = "0"; }
                    if (Fila.Cells["d_CantidadBulto"].Value == null) { Fila.Cells["d_CantidadBulto"].Value = "0"; }
                    CantidadBulto = CantidadBulto + decimal.Parse(Fila.Cells["d_CantidadBulto"].Value.ToString());
                    if (Fila.Cells["v_IdProductoDetalle"].Value != null && Fila.Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000")
                    {
                        SumValR = Utils.Windows.DevuelveValorRedondeado(SumValR + decimal.Parse(Fila.Cells["d_Valor"].Text), 2);
                        SumValVentaR = Utils.Windows.DevuelveValorRedondeado(SumValVentaR + decimal.Parse(Fila.Cells["d_ValorVenta"].Text), 2);
                        SumIgvR = Utils.Windows.DevuelveValorRedondeado(SumIgvR + decimal.Parse(Fila.Cells["d_Igv"].Text), 2);
                        SumTotalR = Utils.Windows.DevuelveValorRedondeado(SumTotalR + decimal.Parse(Fila.Cells["d_Total"].Text), 2);
                        SumaValoresVentaR = SumTotalR;
                        SumDescuentoR = SumDescuentoR + decimal.Parse(Fila.Cells["d_Descuento"].Text);
                        CantidadTotalR = CantidadTotalR + decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                    }
                    else
                    {

                        SumVal = SumVal + decimal.Parse(Fila.Cells["d_Valor"].Text);
                        SumValVenta = SumValVenta + decimal.Parse(Fila.Cells["d_ValorVenta"].Text);
                        SumIgv = SumIgv + decimal.Parse(Fila.Cells["d_Igv"].Text);
                        SumTotal = SumTotal + decimal.Parse(Fila.Cells["d_Total"].Text);
                        SumaValoresVenta = SumTotal;
                        SumDescuento = SumDescuento + decimal.Parse(Fila.Cells["d_Descuento"].Text);
                        CantidadTotal = CantidadTotal + decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());

                    }

                    //txtValor.Text = (SumVal + SumValR).ToString("0.00");
                    //txtValorVenta.Text = (SumValVenta + SumValVentaR).ToString("0.00");
                    //txtDscto.Text = (SumDescuento + SumDescuentoR).ToString("0.00");
                    //txtIgv.Text = (SumIgv + SumIgvR).ToString("0.00");
                    //txtPrecioVenta.Text = (SumTotal + SumTotalR).ToString("0.00");
                    //txtCantidadTotal.Text = (CantidadTotal).ToString("0.00");

                    if (Fila.Cells["i_IdUnidadMedida"].Value != null)
                    {
                        if (Fila.Cells["v_IdProductoDetalle"].Value != null && Fila.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000" && Fila.Cells["i_IdUnidadMedida"].Value.ToString() != "-1" && decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0)
                        {
                            decimal TotalEmpaque = 0;
                            decimal Empaque = decimal.Parse(Fila.Cells["Empaque"].Value.ToString());
                            string Producto = Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                            decimal Cantidad = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                            int UM = int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                            int UMProducto = int.Parse(Fila.Cells["i_IdUnidadMedidaProducto"].Value.ToString());

                            GridKeyValueDTO _UMProducto = ((List<GridKeyValueDTO>)ucbUnidadMedida.DataSource).Where(p => p.Id == UMProducto.ToString()).FirstOrDefault();
                            GridKeyValueDTO _UM = ((List<GridKeyValueDTO>)ucbUnidadMedida.DataSource).Where(p => p.Id == UM.ToString()).FirstOrDefault();

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

                    }
                }

                txtTotal.Text = SumTotal.ToString("0.00");
                txtTotalBultos.Text = CantidadBulto.ToString("0.00");
            }

        }

        private void CargarDetalleTemporal()
        {
            var objOperationResult = new OperationResult();
            grdData.DataSource = _objGuiaRemisionBl.ObtenerGuiaRemisionDetallesTemporal(ref objOperationResult);
            BuscarProductosTemporal();
            foreach (var row in grdData.Rows)
            {
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_EsServicio"].Value = row.Cells["v_IdProductoDetalle"].Value == null ? 0 : _objGuiaRemisionBl.EsServicio(row.Cells["v_IdProductoDetalle"].Value.ToString());

                if (Globals.ClientSession.i_IncluirPreciosGuiaRemision.ToString() == "0")
                {

                    row.Cells["d_Precio"].Value = "0";
                    row.Cells["d_Total"].Value = "0";
                }
                _extraidoDocumentoReferencia = true;
            }
            CalcularValoresTemporales();
        }

        private void CalcularValoresTemporales()
        {
            foreach (var Fila in grdData.Rows)
            {
                var cantidad = Fila.Cells["d_Cantidad"].Value != null ? decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) : 0;
                var precio = Fila.Cells["d_Precio"].Value != null ? decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) : 0;
                var total = cantidad * precio;
                Fila.Cells["d_Total"].Value = total.ToString();


            }
            CalcularValores();
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            var point = new Point(e.X, e.Y);
            var uiElement = ((UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            var row = (UltraGridRow)uiElement.GetContext(typeof(UltraGridRow));

            btnEliminar.Enabled = row != null;
        }

        private void grdData_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (grdData.ActiveCell != null)
            {
                UltraGridCell Celda;
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

                    case "d_Total":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;

                    case "d_CantidadBulto":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;
                }
            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdData.ActiveCell == null) return;

            if (this.grdData.ActiveCell.Column.Key != "i_IdUnidadMedida" && grdData.ActiveCell.Column.Key != "i_IdAlmacen" && grdData.ActiveCell.Column.Key != "i_IdTipoBulto")
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
                    case Keys.Enter:
                        DoubleClickCellEventArgs eventos = new DoubleClickCellEventArgs(grdData.ActiveCell);
                        grdData_DoubleClickCell(sender, eventos);
                        e.Handled = true;
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

            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                if (grdData.ActiveCell.Column.Key == "i_IdAlmacen" && grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null)
                {
                    e.SuppressKeyPress = true;
                    return;
                }
                if (grdData.ActiveCell.Column.Key == "i_IdUnidadMedida" && grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null)
                {
                    e.SuppressKeyPress = true;
                    return;
                }

            }
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

        public void RecibirItems(List<UltraGridRow> Filas)
        {
            bool ExistenciaGrilla = false, anteriorRegistro = false; ;
            int mensajeEg = 0;
            for (int i = 0; i < Filas.Count; i++)
            {
                ExistenciaGrilla = false;

                int IdAlmacen = int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdAlmacen"].Value.ToString());
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                    {
                        if (Filas[i].Cells["v_IdProductoDetalle"].Value.ToString() == Fila.Cells["v_IdProductoDetalle"].Value.ToString() && IdAlmacen == int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()))
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
                            //return;
                        }
                    }
                }
                if (i == 0)
                {
                    if (!ExistenciaGrilla)
                    {
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value = "Temporal";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdTipoBulto"].Value = "-1";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value = DevolverPrecioProducto(Filas[i].Cells["IdMoneda"].Value != null ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString()) : -1, Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()), int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value = DevolverPrecioProducto(Filas[i].Cells["IdMoneda"].Value != null ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString()) : -1, Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()), int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);

                    }
                    else
                    {
                        anteriorRegistro = true;
                    }

                }
                else
                {
                    if (!ExistenciaGrilla)
                    {

                        if (anteriorRegistro)
                        {
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value = "Temporal";
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdTipoBulto"].Value = "-1";
                            grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value = DevolverPrecioProducto(Filas[i].Cells["IdMoneda"].Value != null ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString()) : -1, Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()), int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value = DevolverPrecioProducto(Filas[i].Cells["IdMoneda"].Value != null ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString()) : -1, Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()), int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);

                            anteriorRegistro = false;

                        }
                        else
                        {
                            UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                            grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                            this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                            row.Cells["i_RegistroEstado"].Value = "Agregado";
                            row.Cells["i_RegistroTipo"].Value = "Temporal";
                            row.Cells["i_IdUnidadMedida"].Value = "-1";
                            row.Cells["d_Cantidad"].Value = "1";
                            row.Cells["d_Precio"].Value = "0";
                            row.Cells["i_IdTipoBulto"].Value = "-1";
                            row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                            row.Activate();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descripcion"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value = "Temporal";
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["d_Precio"].Value = DevolverPrecioProducto(Filas[i].Cells["IdMoneda"].Value != null ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString()) : -1, Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()), int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);
                            grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdTipoBulto"].Value = "-1";
                        }
                    }
                }
            }
            CalcularValoresDetalle();
        }

        private void CalcularValoresDetalle()
        {
            if (grdData.Rows.Count() == 0)
            {
                txtTotal.Text = "0.00";
                txtTotalBultos.Text = "0.00";
                txtRedondeo.Text = "0.00";
            }

            foreach (UltraGridRow Fila in grdData.Rows)
            {
                CalcularValoresFila(Fila);
                //  Fila.Cells["i_RegistroEstado"].Value = "Modificado";
            }
        }


        private void CalcularValoresDetalleComboIgv()
        {
            if (grdData.Rows.Count() == 0)
            {
                txtTotal.Text = "0.00";
                txtTotalBultos.Text = "0.00";
                txtRedondeo.Text = "0.00";
            }

            foreach (UltraGridRow Fila in grdData.Rows)
            {
                CalcularValoresFila(Fila);
                Fila.Cells["i_RegistroEstado"].Value = "Modificado";
            }
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");


        }
        private void grdData_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            switch (e.Cell.Column.Key)
            {
                case "i_IdAlmacen":



                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null)
                    {

                        e.Cell.CancelUpdate();

                    }
                    break;
            }

        }

        #endregion

        #region CRUD

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            _movimientodetalleDto = new movimientodetalleDto();
            OperationResult objOperationResult = new OperationResult();
            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _guiaremisiondetalleDto = new guiaremisiondetalleDto();
                    _guiaremisiondetalleDto.v_IdGuiaRemisionDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdGuiaRemisionDetalle"].Value.ToString();
                    _guiaremisiondetalleDto.v_IdMovimientoDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdMovimientoDetalle"].Value == null ? null : grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdMovimientoDetalle"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(_guiaremisiondetalleDto);

                    if (_objDocumentoBl.DocumentoGeneraStock(int.Parse(cboTipoGuia.Value.ToString())))
                    {
                        var movimiento = _objMovimientoBl.ObtenerMovimientoCabeceraDesdeCompras(ref objOperationResult, int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdAlmacen"].Value.ToString()), Constants.OrigenGuiaInterna, txtPeriodo.Text.Trim(), txtMes.Text.Trim(), txtCorrelativo.Text.Trim());
                        if (movimiento != null && movimiento.v_IdMovimiento != null)
                        {
                            _movimientodetalleDto.v_IdMovimientoDetalle = _objMovimientoBl.ObtenerIdMovimientoDetalleGuiaRemision(movimiento.v_IdMovimiento, grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString());
                            _movimientodetalleDto.i_IdAlmacen = grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdAlmacen"].Value.ToString();
                            __TempDetalle_EliminarDto.Add(_movimientodetalleDto);
                        }
                    }
                    grdData.Rows[grdData.ActiveRow.Index].Delete(false);
                    CalcularValores();
                }


            }
            else
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    grdData.Rows[grdData.ActiveRow.Index].Delete(false);
                    CalcularValores();
                }


            }

        }

        private bool ValidarCantidadSegunVenta(ventaDto Venta, string Modo)
        {
            OperationResult pobjOperationResult = new OperationResult();
            string pstrIdVenta = String.Empty;
            if (cboDocumentoRef.Value == null || cboDocumentoRef.Value.ToString() == "-1" || txtSerieDoc.Text.Trim() == string.Empty || txtNumeroDoc.Text.Trim() == string.Empty) { return true; }

            if (Venta != null)
            {
                decimal CantidadVenta = 0, CantidadGuias = 0;
                decimal CantidadAnteriorGuia = 0;

                if (Modo == "Edit")
                {
                    int numCorrelativo;
                    if (int.TryParse(txtNumeroDoc.Text, out numCorrelativo))
                    {
                        var id = int.Parse(txtNumeroDoc.Text);
                        var objVenta = _objGuiaRemisionBl.ObtenerIdVenta(ref pobjOperationResult, Convert.ToInt32(cboDocumentoRef.Value), txtSerieDoc.Text, numCorrelativo.ToString("D8"));
                        pstrIdVenta = (objVenta == null) ? string.Empty : objVenta.v_IdVenta;
                    }
                    else
                        pstrIdVenta = string.Empty;
                }
                else
                {
                    pstrIdVenta = Venta.v_IdVenta;
                }
                if (pstrIdVenta == string.Empty) return true;
                var VentasDetalle = new VentaBL().ObtenerVentaDetalles(ref pobjOperationResult, pstrIdVenta);
                var GuiaRemisionDetalle = _objGuiaRemisionBl.ObtenerDetalleGuiaRemisionporDocumentoRef(int.Parse(cboDocumentoRef.Value.ToString()), txtSerieDoc.Text.Trim(), txtNumeroDoc.Text.Trim());
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    string IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                    string CodigoProducto = Fila.Cells["v_CodInterno"].Value.ToString();
                    decimal CantidadEmpaque = decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                    CantidadVenta = VentasDetalle.ToList().FindAll(x => x.v_IdProductoDetalle == IdProductoDetalle).Sum(x => x.d_CantidadEmpaque.Value);
                    if (Modo == "New")
                    {
                        CantidadGuias = (CantidadEmpaque + GuiaRemisionDetalle.FindAll(x => x.v_IdProductoDetalle == IdProductoDetalle).Sum(x => x.d_CantidadEmpaque.Value));
                    }
                    else
                    {
                        string v_IdGuiaRemisionDetalle = Fila.Cells["v_IdGuiaRemisionDetalle"].Value == null ? null : Fila.Cells["v_IdGuiaRemisionDetalle"].Value.ToString();
                        if (v_IdGuiaRemisionDetalle == null) // En caso de que se anulo la  guia , se modifico la venta y se regresa a ventana a la ventana a editar
                        {

                            CantidadGuias = CantidadEmpaque;
                        }
                        else
                        {
                            CantidadAnteriorGuia = GuiaRemisionDetalle.FindAll(x => x.v_IdGuiaRemisionDetalle == v_IdGuiaRemisionDetalle).Sum(x => x.d_CantidadEmpaque.Value);
                            CantidadGuias = (CantidadEmpaque + GuiaRemisionDetalle.FindAll(x => x.v_IdProductoDetalle == IdProductoDetalle).Sum(x => x.d_CantidadEmpaque.Value)) - CantidadAnteriorGuia;
                        }
                    }

                    int Servicio = VentasDetalle.ToList().FindAll(x => x.v_IdProductoDetalle == IdProductoDetalle).Sum(x => x.i_EsServicio.Value);
                    if ((Globals.ClientSession.i_IncluirServicioGuiaVenta == 1 && (Servicio == 1 || Servicio == 0)) || (Globals.ClientSession.i_IncluirServicioGuiaVenta == 0 && Servicio == 0))
                    {

                        if (CantidadGuias > CantidadVenta)
                        {
                            if (CantidadVenta == 0)
                            {
                                UltraMessageBox.Show("No se permite Guardar. \n El Producto : " + CodigoProducto + ", no está contenido en " + cboDocumentoRef.Text.Trim() + " " + txtSerieDoc.Text.Trim() + "-" + txtNumeroDoc.Text.Trim(), "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                UltraMessageBox.Show("No se permite Guardar. \n La cantidad del Producto : " + CodigoProducto + " es mayor que la registrada en " + cboDocumentoRef.Text.Trim() + " " + txtSerieDoc.Text.Trim() + "-" + txtNumeroDoc.Text.Trim(), "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            }
                            UltraGridRow Row = grdData.Rows.Where(x => x.Cells["v_IdProductoDetalle"].Value.ToString() == IdProductoDetalle).FirstOrDefault();
                            grdData.Selected.Cells.Add(Row.Cells["v_IdProductoDetalle"]);
                            grdData.Focus();
                            Row.Activate();
                            grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                            UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_IdProductoDetalle"];
                            this.grdData.ActiveCell = aCell;
                            return false;

                        }
                    }
                    else
                    {

                        UltraMessageBox.Show("No se permite Guardar. \n El Producto : " + CodigoProducto + " es un Servicio ,Sistema no permte Incluir Servicios en Guia Remisión", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;

                    }

                }
                return true;
            }
            else
            {

                return true;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {

                OperationResult objOperationResult = new OperationResult();
                string MensajeValidar = string.Empty;
                if (uvGuiaRemision.Validate(true, false).IsValid)
                {

                    if (Globals.ClientSession.i_IncluirPreciosGuiaRemision.Value.ToString() == "1" && int.Parse(cboTipoGuia.Value.ToString()) == (int)TiposDocumentos.GuiaInterna)
                    {
                        if (!ValidarIncluirPrecios.Validate(true, false).IsValid)
                        {
                            return;
                        }
                    }

                    //if (Globals.ClientSession.i_IncluirAlmacenDestinoGuiaRemisionVenta ==1 &&  (cboAlmacenLlegada.Value == null || cboAlmacenLlegada.Value.ToString () =="-1"))
                    //{
                    //    if (!ValidarAlmacenDestino.Validate(true, false).IsValid)
                    //    {
                    //        return;
                    //    }
                    //}

                    if (cboDocumentoRef.Value.ToString() != "-1" && txtSerieDoc.Text.Trim() != string.Empty && txtNumeroDoc.Text.Trim() != string.Empty)
                    {
                    }
                    else
                    {
                        if (cboDocumentoRef.Value.ToString() == "-1" && (txtSerieDoc.Text.Trim() == string.Empty && txtNumeroDoc.Text.Trim() == string.Empty))
                        {
                        }
                        else
                        {
                            UltraMessageBox.Show("Por Favor ingrese un Documento de referencia válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            cboDocumentoRef.Focus();
                            return;
                        }
                    }
                    if (_guiaremisionDto.v_IdCliente == null)
                    {

                        UltraMessageBox.Show("Por Favor ingrese un Cliente Válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cboDocumentoRef.Focus();
                        return;

                    }
                    if (decimal.Parse(txtTipoCambio.Text.Trim()) <= 0)
                    {
                        UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTipoCambio.Focus();
                        return;
                    }
                    if (_guiaremisionDto.v_IdCliente == null)
                    {
                        UltraMessageBox.Show("Por Favor ingrese un Cliente que exista  ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtRucCliente.Focus();
                        return;
                    }
                    if (grdData.Rows.Count == 0)
                    {
                        UltraMessageBox.Show("No se permite guardar mientras el detalle está vacío", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (_objDocumentoBl.DocumentoGeneraStock(int.Parse(cboTipoGuia.Value.ToString())))
                    {
                        MensajeValidar = ValidarStock();
                        if (MensajeValidar != "")
                        {

                            var resp =
                       MessageBox.Show(
                           string.Format(
                              MENSAJEVS + MensajeValidar + "¿Desea Guardarlo  de todas formas?"), "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (resp == DialogResult.No) return;
                        }
                    }
                    CalcularValoresDetalle();

                    if (ValidaCamposNulosVacios())
                    {
                        if (ValidarCantidadSegunVenta(_idVenta, _mode))
                        {
                            if (_mode == "New")
                            {
                                while (_objGuiaRemisionBl.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text) == false)
                                {
                                    txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                                }
                                while (_objGuiaRemisionBl.ExisteNroDocumento(int.Parse(cboTipoGuia.Value.ToString()), txtSerieGuia.Text, txtNumeroGuia.Text) == false)
                                {
                                    txtNumeroGuia.Text = (int.Parse(txtNumeroGuia.Text) + 1).ToString("00000000");
                                }
                                objOperationResult.Success = GenerarGuias() ? 1 : 0;
                            }
                            else if (_mode == "Edit")
                            {


                                _guiaremisionDto.i_IdTipoGuia = int.Parse(cboTipoGuia.Value.ToString());
                                _guiaremisionDto.v_Periodo = txtPeriodo.Text;
                                _guiaremisionDto.v_Mes = txtMes.Text;
                                _guiaremisionDto.v_Correlativo = txtCorrelativo.Text;
                                _guiaremisionDto.v_SerieGuiaRemision = txtSerieGuia.Text;
                                _guiaremisionDto.v_NumeroGuiaRemision = txtNumeroGuia.Text;
                                _guiaremisionDto.t_FechaEmision = dtpFechaEmision.Value;
                                _guiaremisionDto.d_TipoCambio = Convert.ToDecimal(txtTipoCambio.Text);
                                _guiaremisionDto.t_FechaTraslado = dtpFechaTraslado.Value;
                                _guiaremisionDto.v_SerieDocumentoRef = txtSerieDoc.Text;
                                _guiaremisionDto.v_NumeroDocumentoRef = txtNumeroDoc.Text;
                                _guiaremisionDto.v_NumeroPedidoCotizacion = txtNumeroCotizacion.Text;
                                _guiaremisionDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                                _guiaremisionDto.v_PuntoPartida = txtPartida.Text;
                                _guiaremisionDto.v_PuntoLLegada = txtLlegada.Text;
                                _guiaremisionDto.i_IdMotivoTraslado = int.Parse(cboMotivos.Value.ToString());
                                _guiaremisionDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                                _guiaremisionDto.d_Total = txtTotal.Text == "0.00" ? 0 : decimal.Parse(txtTotal.Text);
                                _guiaremisionDto.d_TotalBultos = txtTotalBultos.Text == "0.00" ? 0 : decimal.Parse(txtTotalBultos.Text);
                                _guiaremisionDto.i_IdTipoDocumento = int.Parse(cboDocumentoRef.Value.ToString());
                                _guiaremisionDto.v_IdUnidadTransporte = cboPlaca.Value.ToString() == "-1" ? null : cboPlaca.Value.ToString();
                                _guiaremisionDto.v_IdChofer = cboChofer.Value.ToString() == "-1" ? null : cboChofer.Value.ToString();
                                _guiaremisionDto.v_IdAgenciaTransporte = cboRazonAgencia.Value.ToString() == "-1" ? null : cboRazonAgencia.Value.ToString();
                                _guiaremisionDto.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());
                                _guiaremisionDto.i_IdIgv = int.Parse(cboIGV.Value.ToString());
                                _guiaremisionDto.i_AfectoIgv = uckAfectoIgv.Checked ? 1 : 0;
                                _guiaremisionDto.i_PrecionInclIgv = uckPreciosIncluyenIgv.Checked ? 1 : 0;
                                _guiaremisionDto.i_IdAlmacenDestino = cboAlmacenLlegada.Value == null ? -1 : int.Parse(cboAlmacenLlegada.Value.ToString());
                                _guiaremisionDto.i_Modalidad = cboModalidad.Value != null ? short.Parse(cboModalidad.Value.ToString()) : (short?)null;
                                _guiaremisionDto.v_UbigueoLlegada = txtUbigueoLlegada.Text;
                                _guiaremisionDto.v_UbigueoPartida = txtUbigueoPartida.Text;
                                _guiaremisionDto.d_TotalPeso = GetNumberFromText(txtTotalPeso.Text);

                                if (Globals.ClientSession.i_IncluirAlmacenDestinoGuiaRemisionVenta == 1 && _guiaremisionDto.i_IdAlmacenDestino == -1)
                                {
                                    _guiaremisionDto.i_IdEstado = 2;
                                }
                                LlenarTemporales();
                                using (TransactionScope ts = TransactionUtils.CreateTransactionScope())
                                {

                                    if (_objDocumentoBl.DocumentoGeneraStock(int.Parse(cboTipoGuia.Value.ToString())))
                                    {
                                        ActualizaNotaSalida(ref objOperationResult);
                                    }

                                    newIdGuiaRemision = _objGuiaRemisionBl.ActualizarGuiaRemision(ref objOperationResult, _guiaremisionDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto, _idVenta == null ? null : _idVenta.v_IdVenta);

                                    if (Temporal)
                                    {
                                        _objGuiaRemisionBl.EliminarTemporal();
                                        Temporal = false;
                                    }

                                    ts.Complete();
                                }
                            }

                            if (objOperationResult.Success == 1)
                            {
                                _strModo = "Guardado";
                                // EdicionBarraNavegacion(true);
                                PstrIdMovimientoNuevo = newIdGuiaRemision;
                                _strIdGuiaRemision = newIdGuiaRemision;
                                // ObtenerListadoGuiaRemision(txtPeriodo.Text, txtMes.Text);
                                btnImprimir.Enabled = _btnImprimir;
                                CargarCabecera(_strIdGuiaRemision);
                                EdicionBarraNavegacion(false);
                                if (UltraMessageBox.Show("La guia Remisión se ha guardado correctamente ,¿Desea Generar una nueva guia Remisión?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    _strModo = "Nuevo";
                                    btnNuevoMovimiento_Click(sender, e);


                                }
                            }
                            else
                            {
                                UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            _TempDetalle_AgregarDto = new List<guiaremisiondetalleDto>();
                            _TempDetalle_ModificarDto = new List<guiaremisiondetalleDto>();
                            _TempDetalle_EliminarDto = new List<guiaremisiondetalleDto>();
                            __TempDetalle_AgregarDto = new List<movimientodetalleDto>();
                            __TempDetalle_EliminarDto = new List<movimientodetalleDto>();
                            __TempDetalle_ModificarDto = new List<movimientodetalleDto>();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _TempDetalle_AgregarDto = new List<guiaremisiondetalleDto>();
                _TempDetalle_ModificarDto = new List<guiaremisiondetalleDto>();
                _TempDetalle_EliminarDto = new List<guiaremisiondetalleDto>();
                __TempDetalle_AgregarDto = new List<movimientodetalleDto>();
                __TempDetalle_EliminarDto = new List<movimientodetalleDto>();
                __TempDetalle_ModificarDto = new List<movimientodetalleDto>();

            }

        }

        private bool GenerarGuias()
        {
            var objOperationResult = new OperationResult();

            var local = new EstablecimientoBL().GetestablecimientoDetalle(ref objOperationResult, Globals.ClientSession.i_IdEstablecimiento ?? 1, int.Parse(cboTipoGuia.Value.ToString()));
            if (objOperationResult.Success == 0 || local == null)
            {
                UltraMessageBox.Show(string.Format("No se encontro informacion del establecimiento{0}{1}", Environment.NewLine, objOperationResult.ExceptionMessage), "Error", Icono: MessageBoxIcon.Error);
                return false;
            }
            var numItemsGuia = local.i_NumeroItems ?? 0;
            if (numItemsGuia == 0) numItemsGuia = grdData.Rows.Count;

            var totalItems = grdData.Rows.Count;
            var numGuias = totalItems / numItemsGuia;
            if (totalItems % numItemsGuia != 0) numGuias++;
            var NroGuia = int.Parse(txtNumeroGuia.Text);
            if (numGuias > 1)
            {
                if (UltraMessageBox.Show(string.Format("Se Generaran {0} Guias de Remision\nDesde {1}-{2:D8} hasta {1}-{3:D8}",
                    numGuias, txtSerieGuia.Text, NroGuia, NroGuia + numGuias - 1), Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    != System.Windows.Forms.DialogResult.Yes) return false;
            }

            _guiaremisionDto.i_IdTipoGuia = int.Parse(cboTipoGuia.Value.ToString());
            _guiaremisionDto.v_Periodo = txtPeriodo.Text;
            _guiaremisionDto.v_Mes = txtMes.Text;
            _guiaremisionDto.v_SerieGuiaRemision = txtSerieGuia.Text;
            _guiaremisionDto.v_NumeroGuiaRemision = txtNumeroGuia.Text;
            _guiaremisionDto.v_Correlativo = txtCorrelativo.Text;
            _guiaremisionDto.t_FechaEmision = dtpFechaEmision.Value;
            _guiaremisionDto.d_TipoCambio = Convert.ToDecimal(txtTipoCambio.Text);
            _guiaremisionDto.t_FechaTraslado = dtpFechaTraslado.Value;
            _guiaremisionDto.v_SerieDocumentoRef = txtSerieDoc.Text;
            _guiaremisionDto.v_NumeroDocumentoRef = txtNumeroDoc.Text;
            _guiaremisionDto.v_NumeroPedidoCotizacion = txtNumeroCotizacion.Text;
            _guiaremisionDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
            _guiaremisionDto.v_PuntoPartida = txtPartida.Text;
            _guiaremisionDto.v_PuntoLLegada = txtLlegada.Text;
            _guiaremisionDto.i_IdMotivoTraslado = int.Parse(cboMotivos.Value.ToString());
            _guiaremisionDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
            _guiaremisionDto.d_Total = txtTotal.Text == "0.00" ? 0 : decimal.Parse(txtTotal.Text);
            _guiaremisionDto.d_TotalBultos = txtTotalBultos.Text == "0.00" ? 0 : decimal.Parse(txtTotalBultos.Text);
            _guiaremisionDto.d_Redondeo = txtRedondeo.Text == "0.00" ? 0 : decimal.Parse(txtRedondeo.Text);
            _guiaremisionDto.v_IdUnidadTransporte = cboPlaca.Value.ToString() == "-1" ? null : cboPlaca.Value.ToString();
            _guiaremisionDto.v_IdChofer = cboChofer.Value.ToString() == "-1" ? null : cboChofer.Value.ToString();
            _guiaremisionDto.v_IdAgenciaTransporte = cboRazonAgencia.Value.ToString() == "-1" ? null : cboRazonAgencia.Value.ToString();
            _guiaremisionDto.i_IdTipoDocumento = int.Parse(cboDocumentoRef.Value.ToString());
            _guiaremisionDto.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());
            _guiaremisionDto.i_IdIgv = int.Parse(cboIGV.Value.ToString());
            _guiaremisionDto.i_AfectoIgv = uckAfectoIgv.Checked ? 1 : 0;
            _guiaremisionDto.i_PrecionInclIgv = uckPreciosIncluyenIgv.Checked ? 1 : 0;
            _guiaremisionDto.i_IdAlmacenDestino = cboAlmacenLlegada.Value == null ? -1 : int.Parse(cboAlmacenLlegada.Value.ToString());
            _guiaremisionDto.i_Modalidad = cboModalidad.Value != null ? short.Parse(cboModalidad.Value.ToString()) : (short?)null;
            _guiaremisionDto.v_UbigueoLlegada = txtUbigueoLlegada.Text;
            _guiaremisionDto.v_UbigueoPartida = txtUbigueoPartida.Text;
            _guiaremisionDto.d_TotalPeso = GetNumberFromText(txtTotalPeso.Text);
            if (Globals.ClientSession.i_IncluirAlmacenDestinoGuiaRemisionVenta == 1 && _guiaremisionDto.i_IdAlmacenDestino == -1)
            {
                _guiaremisionDto.i_IdEstado = 2;
            }
            LlenarTemporales();
            for (int i = 0; i < numGuias; i++)
            {
                while (_objGuiaRemisionBl.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, _guiaremisionDto.v_Correlativo) == false)
                    _guiaremisionDto.v_Correlativo = (int.Parse(_guiaremisionDto.v_Correlativo) + 1).ToString("D8");

                while (_objGuiaRemisionBl.ExisteNroDocumento(_guiaremisionDto.i_IdTipoGuia.Value, _guiaremisionDto.v_SerieGuiaRemision, _guiaremisionDto.v_NumeroGuiaRemision) == false)
                {
                    _guiaremisionDto.v_NumeroGuiaRemision = (int.Parse(_guiaremisionDto.v_NumeroGuiaRemision) + 1).ToString("D8");
                }
                var itemsDetail = _TempDetalle_AgregarDto.Skip(i * numItemsGuia).Take(numItemsGuia).ToList();
                newIdGuiaRemision = _objGuiaRemisionBl.InsertarGuiaRemision(ref objOperationResult, _guiaremisionDto, Globals.ClientSession.GetAsList(), itemsDetail);
                if (objOperationResult.Success != 1)
                {
                    UltraMessageBox.Show(objOperationResult.ExceptionMessage, "Error al Insertar Guias", Icono: MessageBoxIcon.Error);
                    return false;
                }
            }
            if (Temporal)
            {
                _objGuiaRemisionBl.EliminarTemporal();
                Temporal = false;
            }
            return true;
        }

        private void ActualizaNotaSalida(ref OperationResult objOperationResultF)
        {
            //Verifica primero que el detalle contenga almenos un producto, si contiene solo servicios no genera nota de salida.
            bool ExisteProducto = false;

            OperationResult objOperationResult = new OperationResult();
            bool _EsNotadeCredito = false;
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                if (Fila.Cells["i_EsServicio"].Value.ToString() == "0")
                {
                    ExisteProducto = true;
                    break;
                }
            }
            if (ExisteProducto == true)
            {
                List<string> Almacenes = new List<string>();
                string pstrAlmacen;

                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["v_IdProductoDetalle"].Value != null && Fila.Cells["i_EsServicio"].Value.ToString() == "0")
                    {
                        pstrAlmacen = Fila.Cells["i_IdAlmacen"].Value.ToString();
                        if (!Almacenes.Contains(pstrAlmacen))
                        {
                            Almacenes.Add(pstrAlmacen);
                        }
                    }
                }

                foreach (String Almacen in Almacenes)
                {
                    _movimientoDto = _objMovimientoBl.ObtenerMovimientoCabeceraDesdeCompras(ref objOperationResult, int.Parse(Almacen), Constants.OrigenGuiaInterna, _guiaremisionDto.v_Periodo, _guiaremisionDto.v_Mes.Trim(), _guiaremisionDto.v_Correlativo.Trim());

                    if (cboEstado.Value.ToString() != "0")
                    {
                        if (_movimientoDto != null)
                        {
                            //_EsNotadeCredito = cboDocumento.Value.ToString() == "7" | cboDocumento.Value.ToString() == "8" ? true : false;
                            _EsNotadeCredito = _objDocumentoBl.DocumentoEsInverso(int.Parse(cboDocumentoRef.Value.ToString())) ? true : false;
                            _movimientoDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                            _movimientoDto.i_IdAlmacenOrigen = int.Parse(Almacen);
                            _movimientoDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                            _movimientoDto.t_Fecha = dtpFechaEmision.Value;
                            _movimientoDto.v_Mes = txtMes.Text.Trim();
                            _movimientoDto.i_EsDevolucion = _EsNotadeCredito == true ? 1 : 0;
                            _movimientoDto.v_IdCliente = _guiaremisionDto.v_IdCliente;
                            _movimientoDto.d_TotalPrecio = txtTotal.Text == "" ? 0 : decimal.Parse(txtTotal.Text);
                            _movimientoDto.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());
                            LlenarTemporalesMovimiento(Almacen);
                            decimal Cantidad = 0, Total = 0, _Cantidad, _Total;
                            for (int i = 0; i < grdData.Rows.Count(); i++)
                            {
                                if (grdData.Rows[i].Cells["i_IdAlmacen"].Value.ToString() == Almacen)
                                {
                                    _Cantidad = grdData.Rows[i].Cells["d_Cantidad"].Value != null ? decimal.Parse(grdData.Rows[i].Cells["d_Cantidad"].Value.ToString()) : 0;
                                    Cantidad = Cantidad + _Cantidad;

                                    _Total = grdData.Rows[i].Cells["d_Precio"].Value != null ? decimal.Parse(grdData.Rows[i].Cells["d_Precio"].Value.ToString()) : 0;
                                    Total = (_Cantidad * _Total) + Total;
                                }
                            }

                            _movimientoDto.d_TotalCantidad = Cantidad;
                            _movimientoDto.d_TotalPrecio = Total;
                            _objMovimientoBl.ActualizarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), __TempDetalle_AgregarDto, __TempDetalle_ModificarDto, __TempDetalle_EliminarDto);
                            __TempDetalle_AgregarDto = new List<movimientodetalleDto>();
                            __TempDetalle_EliminarDto = new List<movimientodetalleDto>();
                            __TempDetalle_ModificarDto = new List<movimientodetalleDto>();
                        }
                        else
                        {
                            _movimientoDto = new movimientoDto();
                            List<KeyValueDTO> ListaMovimientos = new List<KeyValueDTO>();
                            ListaMovimientos = _objMovimientoBl.ObtenerListadoMovimientos(ref objOperationResult, txtPeriodo.Text, txtMes.Text, (int)TipoDeMovimiento.NotadeSalida);

                            int MaxMovimiento;
                            MaxMovimiento = ListaMovimientos.Count() > 0 ? int.Parse(ListaMovimientos[ListaMovimientos.Count() - 1].Value1.ToString()) : 0;
                            MaxMovimiento++;
                            //  _EsNotadeCredito = cboDocumento.Value.ToString() == "7" | cboDocumento.Value.ToString() == "8" ? true : false;
                            _EsNotadeCredito = _objDocumentoBl.DocumentoEsInverso(int.Parse(cboDocumentoRef.Value.ToString())) ? true : false;
                            _movimientoDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                            _movimientoDto.i_IdAlmacenOrigen = int.Parse(Almacen);
                            _movimientoDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                            _movimientoDto.i_IdTipoMotivo = 1;//Compra nacional
                            _movimientoDto.t_Fecha = dtpFechaEmision.Value;
                            _movimientoDto.v_Mes = txtMes.Text.Trim();
                            _movimientoDto.v_Periodo = txtPeriodo.Text.Trim();
                            _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeSalida;
                            _movimientoDto.v_Correlativo = MaxMovimiento.ToString("00000000");
                            _movimientoDto.v_IdCliente = _guiaremisionDto.v_IdCliente;
                            _movimientoDto.v_OrigenTipo = Constants.OrigenGuiaInterna;
                            _movimientoDto.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());
                            _movimientoDto.i_EsDevolucion = _EsNotadeCredito == true ? 1 : 0;
                            _movimientoDto.v_OrigenRegCorrelativo = txtCorrelativo.Text;
                            _movimientoDto.v_OrigenRegMes = txtMes.Text.Trim();
                            _movimientoDto.v_OrigenRegPeriodo = txtPeriodo.Text.Trim();
                            _movimientoDto.d_TotalPrecio = txtTotal.Text == "" ? 0 : decimal.Parse(txtTotal.Text);
                            LlenarTemporalesMovimiento(Almacen);
                            decimal Cantidad = 0, Total = 0;
                            for (int i = 0; i < grdData.Rows.Count(); i++)
                            {
                                if (grdData.Rows[i].Cells["i_IdAlmacen"].Value.ToString() == Almacen)
                                {
                                    var _Cantidad = grdData.Rows[i].Cells["d_Cantidad"].Value != null ? decimal.Parse(grdData.Rows[i].Cells["d_Cantidad"].Value.ToString()) : 0;
                                    Cantidad = Cantidad + _Cantidad;

                                    var _Total = grdData.Rows[i].Cells["d_Precio"].Value != null ? decimal.Parse(grdData.Rows[i].Cells["d_Precio"].Value.ToString()) : 0;
                                    //Total = Total + _Total;
                                    Total = (_Cantidad * _Total) + Total;
                                }
                            }

                            _movimientoDto.d_TotalCantidad = Cantidad;
                            _movimientoDto.d_TotalPrecio = Total;
                            _objMovimientoBl.InsertarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), __TempDetalle_AgregarDto);
                            __TempDetalle_AgregarDto = new List<movimientodetalleDto>();
                        }
                    }
                    else
                    {
                        if (_movimientoDto != null)
                        {
                            _objMovimientoBl.EliminarMovimiento(ref objOperationResult, _movimientoDto.v_IdMovimiento, Globals.ClientSession.GetAsList());
                        }
                    }
                }
            }
            objOperationResultF.Success = objOperationResult.Success;
        }

        private void LlenarTemporalesMovimiento(string Almacen)
        {
            OperationResult objOperationResult = new OperationResult();
            if (grdData.Rows.Count() != 0)
            {

                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["i_IdAlmacen"].Value != null && Fila.Cells["i_EsServicio"].Value.ToString() != "1")
                    {
                        if (Fila.Cells["i_IdAlmacen"].Value.ToString() == Almacen && Fila.Cells["v_IdProductoDetalle"].Value != null)
                        {
                            switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                            {
                                case "Temporal":
                                    if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                                    {
                                        _movimientodetalleDto = new movimientodetalleDto();
                                        _movimientodetalleDto.v_IdMovimiento = _movimientoDto.v_IdMovimiento;
                                        _movimientodetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();


                                        _movimientodetalleDto.v_NroGuiaRemision = txtSerieGuia.Text.Trim() == string.Empty || txtNumeroGuia.Text.Trim() == string.Empty ? "" : txtSerieGuia.Text.Trim() + "-" + txtNumeroGuia.Text.Trim();
                                        _movimientodetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                        _movimientodetalleDto.i_IdUnidad = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                        _movimientodetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                        _movimientodetalleDto.d_Total = decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                        _movimientodetalleDto.v_NroPedido = "";
                                        _movimientodetalleDto.d_CantidadEmpaque = decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());

                                        _movimientodetalleDto.i_IdTipoDocumento = int.Parse(cboTipoGuia.Value.ToString());
                                        _movimientodetalleDto.v_NumeroDocumento = _movimientodetalleDto.v_NroGuiaRemision.Trim();
                                        __TempDetalle_AgregarDto.Add(_movimientodetalleDto);
                                    }
                                    break;

                                case "NoTemporal":
                                    if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                                    {
                                        _movimientodetalleDto = new movimientodetalleDto();
                                        var movimiento = _objMovimientoBl.ObtenerMovimientoCabeceraDesdeCompras(ref objOperationResult, int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdAlmacen"].Value.ToString()), Constants.OrigenGuiaInterna, txtPeriodo.Text.Trim(), txtMes.Text.Trim(), txtCorrelativo.Text.Trim());
                                        if (movimiento != null && movimiento.v_IdMovimiento != null)
                                        {
                                            _movimientodetalleDto.v_IdMovimientoDetalle = _objMovimientoBl.ObtenerIdMovimientoDetalleGuiaRemision(movimiento.v_IdMovimiento, Fila.Cells["v_IdProductoDetalle"].Value.ToString());

                                        }

                                        _movimientodetalleDto.v_IdMovimiento = _movimientoDto.v_IdMovimiento;
                                        _movimientodetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();


                                        _movimientodetalleDto.v_NroGuiaRemision = txtSerieGuia.Text.Trim() == string.Empty || txtNumeroGuia.Text.Trim() == string.Empty ? "" : txtSerieGuia.Text.Trim() + "-" + txtNumeroGuia.Text.Trim();

                                        _movimientodetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                        _movimientodetalleDto.i_IdUnidad = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                        _movimientodetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                        _movimientodetalleDto.d_Total = decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                        _movimientodetalleDto.v_NroPedido = "";
                                        _movimientodetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                        _movimientodetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                        _movimientodetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                        _movimientodetalleDto.d_CantidadEmpaque = decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                        _movimientodetalleDto.i_IdTipoDocumento = int.Parse(cboTipoGuia.Value.ToString());
                                        _movimientodetalleDto.v_NumeroDocumento = _movimientodetalleDto.v_NroGuiaRemision.Trim();
                                        __TempDetalle_ModificarDto.Add(_movimientodetalleDto);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

        }

        private string ValidarStock()
        {
            OperationResult objOperationResult = new OperationResult();
            string mensaje = string.Empty;
            int ValidarStock;


            foreach (UltraGridRow Fila in grdData.Rows)
            {
                {
                    ValidarStock = _objPedidoBl.ValidarStock(ref objOperationResult, Fila.Cells["v_IdProductoDetalle"].Value.ToString());
                    if (!_objDocumentoBl.DocumentoEsInverso(int.Parse(cboDocumentoRef.Value.ToString())) && int.Parse(Fila.Cells["i_EsServicio"].Value.ToString()) != 1 && cboDocumentoRef.Value.ToString() != "8")
                    {
                        decimal Excedente;
                        string IdGuiaRemisionDetalle = Fila.Cells["v_IdGuiaRemisionDetalle"].Value != null ? Fila.Cells["v_IdGuiaRemisionDetalle"].Value.ToString() : null;
                        Excedente = _objGuiaRemisionBl.CantidadExcedentePorGuiaDetalle(ref objOperationResult, int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()), Fila.Cells["v_IdProductoDetalle"].Value.ToString(), decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()), IdGuiaRemisionDetalle, int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString()));
                        if (ValidarStock == 1)
                        {
                            if (Excedente < 0)
                            {
                                // UltraMessageBox.Show("La cantidad ingresada sobrepasa el Saldo disponible en stock", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                mensaje = mensaje + Fila.Cells["v_CodInterno"].Value.ToString().Trim() + " " + Fila.Cells["v_Descripcion"].Value.ToString().Trim() + "\n";
                                //UltraGridCell aCell = Fila.d_Cantidad.Value ;
                                // grdData.ActiveRow = Fila;
                                //this.grdData.ActiveCell = aCell;
                                //grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                                //grdData.Focus();

                            }
                        }
                    }
                }

            }

            return mensaje;


        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            //if (ValidarMonedaLP.Validate(true, false).IsValid)
            //{
            if (cboMoneda.Value.ToString() == "-1")
            {
                UltraMessageBox.Show("Por Favor ingrese la moneda ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cboMoneda.Focus();
                return;
            }
            if (txtTipoCambio.Text == "")
            {
                UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtTipoCambio.Focus();
                return;

            }
            else if (decimal.Parse(txtTipoCambio.Text) <= 0)
            {
                UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtTipoCambio.Focus();
                return;

            }
            var ultimaFila = grdData.Rows.LastOrDefault();
            if (ultimaFila == null || (ultimaFila.Cells["i_IdAlmacen"].Value != null && ultimaFila.Cells["v_IdProductoDetalle"].Value != null))
            {
                UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Agregado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["i_IdUnidadMedida"].Value = "-1";
                row.Cells["i_IdTipoBulto"].Value = "-1";
                row.Cells["d_Cantidad"].Value = "1";
                row.Cells["d_Precio"].Value = "0";
                row.Cells["i_IdTipoBulto"].Value = "-1";
                row.Cells["d_Valor"].Value = "0";
                row.Cells["d_Descuento"].Value = "0";
                row.Cells["d_ValorVenta"].Value = "0";
                row.Cells["d_Igv"].Value = "0";
                row.Cells["d_Total"].Value = "0"; //PrecioVenta

                row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();

                grdData.Focus();
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_CodInterno"];
                this.grdData.ActiveCell = aCell;
                grdData.ActiveRow = aCell.Row;

                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);

            }
            if (ultimaFila != null)
            {
                if (ultimaFila.Cells["i_IdAlmacen"].Value == null)
                {
                    grdData.Focus();
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdAlmacen"];
                    this.grdData.ActiveCell = aCell;
                    grdData.ActiveRow = aCell.Row;

                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                }
                else if (ultimaFila.Cells["v_IdProductoDetalle"].Value == null)
                {
                    grdData.Focus();
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_CodInterno"];
                    this.grdData.ActiveCell = aCell;
                    grdData.ActiveRow = aCell.Row;

                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);

                }
            }

            //}
        }
        #endregion

        #region Comportamiento Controles
        private void txtNumeroCotizacion_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc, e);
        }
        private void btnDocumentos_Click(object sender, EventArgs e)
        {
            EncontrarDocRef = false;
            serieDocPrevio = txtSerieDoc.Text.Trim();
            _tipoDocumentoPrevio = int.Parse(cboDocumentoRef.Value.ToString());
            correlativoDocPrevio = txtNumeroDoc.Text.Trim();
            var frm = new frmBuscarVenta();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                var r = frm.Result;
                cboDocumentoRef.Value = r.iTipoDocumento.ToString();
                if (r.NroDocumento.Contains('-'))
                {
                    var serCorr = r.NroDocumento.Split('-');
                    txtSerieDoc.Text = serCorr[0];
                    txtNumeroDoc.Text = serCorr[1];
                    if (frm.IsNotaSalida)
                    {
                        GetDetalleNotaSalida(r.ID);
                        ConfiguracionGrilla();
                    }
                    else
                    {

                        if (frm.IsPedido)
                        {
                            GetDetallePedido(r.ID);
                        }
                        else
                        {
                            FiltroVentas = _eventoEnter = true;
                            txtNumeroDoc_Validating(sender, new CancelEventArgs());
                            FiltroVentas = _eventoEnter = false;
                        }
                    }

                }
                else
                {
                    txtSerieDoc.Clear();
                    txtNumeroDoc.Clear();
                }
            }
        }
        private void txtRucCliente_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRucCliente.Text.Trim()))
            {

                if (txtRucCliente.TextLength == 8)
                {

                }
                else
                {
                    if (_guiaremisionDto.v_IdCliente != "N002-CL000000000") // Solo valida cuando son diferentes de Publico general
                    {

                        if (txtRucCliente.TextLength != 11)
                        {
                            UltraMessageBox.Show("RUC Inválido", "Error de  Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            _guiaremisionDto.v_IdCliente = null;
                            txtCodigoCliente.Clear();
                            txtRazonCliente.Clear();
                            txtRucCliente.Focus();
                        }
                        else
                        {
                            if (Utils.Windows.ValidarRuc(txtRucCliente.Text.Trim()) != true)
                            {
                                UltraMessageBox.Show("RUC Inválido", "Error de  Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                _guiaremisionDto.v_IdCliente = null;
                                txtCodigoCliente.Clear();
                                txtRazonCliente.Clear();
                                txtRucCliente.Focus();
                            }
                        }
                    }
                }
            }
        }
        private void txtSerieDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc, e);
        }

        private void txtNumeroDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtNumeroDoc, e);


        }

        private void cboPlaca_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cboPlaca.Value == null) return;
            if (cboPlaca.Value.ToString() != "-1")
            {

                var x = (KeyValueDTO)cboPlaca.SelectedItem.ListObject;

                if (x == null) return;
                txtMarca.Text = x.Value2;
                txtNumInscripcion.Text = x.Value3;

            }
            else
            {
                txtMarca.Clear();
                txtNumInscripcion.Clear();

            }
        }

        private void cboChofer_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cboChofer.Value == null) return;
            if (cboChofer.Value.ToString() != "-1")
            {
                var x = (KeyValueDTO)cboChofer.SelectedItem.ListObject;
                if (x == null) return;
                txtNombreChofer.Text = x.Value2;

            }
            else
            {
                txtNombreChofer.Clear();

            }


        }

        private void cboRazonAgencia_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboRazonAgencia.Value == null)
            {
                txtRucAgencia.Clear();
                txtDireccion.Clear();
            }
            else if (cboRazonAgencia.Value.ToString() != "-1" && cboRazonAgencia.SelectedItem != null)
            {
                var x = (KeyValueDTO)cboRazonAgencia.SelectedItem.ListObject;
                if (x == null) return;

                txtRucAgencia.Text = x.Value2;
                txtDireccion.Text = x.Value3;

            }
            else
            {
                txtRucAgencia.Clear();
                txtDireccion.Clear();

            }
        }

        private void txtRucTransportista_KeyDown(object sender, KeyEventArgs e)
        {
            var objOperationResult = new OperationResult();
            if (!txtRucTransportista.IsDroppedDown && e.KeyCode == Keys.Enter)
            {
                if (txtRucTransportista.Text.Trim() != "" && txtRucTransportista.TextLength <= 7)
                {
                    Mantenimientos.frmBuscarTransportista frm = new Mantenimientos.frmBuscarTransportista(txtRucTransportista.Text);
                    frm.ShowDialog();
                    if (frm.StrIdTransportista != null)
                    {
                        _guiaremisionDto.v_IdTransportista = frm.StrIdTransportista;
                        _strIdTransportista = frm.StrIdTransportista;
                        txtRucTransportista.Text = frm.StrNroDocumento;
                        txtRazonSocial.Text = frm.StrRazonSocial;
                        txtCodigo.Text = frm.StrCodigo;
                        var ListaPlacas = _objTransporteUnidadTransporte.ObtenerListadoTransportistaUnidadTransporteParaCombo(ref objOperationResult, null, null, _strIdTransportista);
                        var ListaChofer = _objTransportistaChoferBl.ObtenerListadoTransportistaChoferParaCombo(ref objOperationResult, null, null, _strIdTransportista);
                        Utils.Windows.LoadUltraComboEditorList(cboPlaca, "Value1", "Id", ListaPlacas, DropDownListAction.Select);
                        Utils.Windows.LoadUltraComboEditorList(cboChofer, "Value1", "Id", ListaChofer, DropDownListAction.Select);
                        cboChofer.Value = ListaChofer.Any() ? ListaChofer.Where(x => x.Id != "-1").ToList().Any() ? ListaChofer.Where(x => x.Id != "-1").FirstOrDefault().Id.ToString() : "-1" : "-1";
                        cboPlaca.Value = ListaPlacas.Any() ? ListaPlacas.Where(x => x.Id != "-1").ToList().Any() ? ListaPlacas.Where(x => x.Id != "-1").FirstOrDefault().Id.ToString() : "-1" : "-1";
                    }

                    else
                    {
                        txtRazonSocial.Clear();
                        txtCodigo.Clear();
                        txtMarca.Clear();
                        txtNumInscripcion.Clear();
                        txtNombreChofer.Clear();
                        Utils.Windows.LoadUltraComboEditorList(cboPlaca, "Value1", "Id", _objTransporteUnidadTransporte.ObtenerListadoTransportistaUnidadTransporteParaCombo(ref objOperationResult, null, null, ""), DropDownListAction.Select);
                        Utils.Windows.LoadUltraComboEditorList(cboChofer, "Value1", "Id", _objTransportistaChoferBl.ObtenerListadoTransportistaChoferParaCombo(ref objOperationResult, null, null, ""), DropDownListAction.Select);
                        cboPlaca.Value = "-1";
                        cboChofer.Value = "-1";
                        return;
                    }
                }
                else if (txtRucTransportista.TextLength == 8 || txtRucTransportista.TextLength == 11)
                {

                    var _objData = _objGuiaRemisionBl.ObtenerTransportistaPorNroDocumento(ref objOperationResult, txtRucTransportista.Text);

                    if (_objData != null)
                    {
                        _strIdTransportista = _objData.v_IdTransportista;
                        txtRucTransportista.Text = _objData.v_NumeroDocumento;
                        txtRazonSocial.Text = _objData.v_NombreRazonSocial;
                        txtCodigo.Text = _objData.v_Codigo;

                        var ListaPlacas = _objTransporteUnidadTransporte.ObtenerListadoTransportistaUnidadTransporteParaCombo(ref objOperationResult, null, null, _strIdTransportista);
                        var ListaChofer = _objTransportistaChoferBl.ObtenerListadoTransportistaChoferParaCombo(ref objOperationResult, null, null, _strIdTransportista);
                        Utils.Windows.LoadUltraComboEditorList(cboPlaca, "Value1", "Id", ListaPlacas, DropDownListAction.Select);
                        Utils.Windows.LoadUltraComboEditorList(cboChofer, "Value1", "Id", ListaChofer, DropDownListAction.Select);
                        ListaChofer = ListaChofer.Where(o => o.Id != "-1").ToList();
                        ListaPlacas = ListaPlacas.Where(o => o.Id != "-1").ToList();
                        var idChofer = ListaChofer.Any() ? ListaChofer.FirstOrDefault(x => x.Id != "-1").Id : "-1";
                        var idPlaca = ListaPlacas.Any() ? ListaPlacas.FirstOrDefault(x => x.Id != "-1").Id : "-1";
                        cboChofer.Value = ListaChofer.Any() ? ListaChofer.Where(x => x.Id != "-1").ToList().Any() ? idChofer : "-1" : "-1";
                        cboPlaca.Value = ListaPlacas.Any() ? ListaPlacas.Where(x => x.Id != "-1").ToList().Any() ? idPlaca : "-1" : "-1";
                    }
                    else
                    {
                        Mantenimientos.frmRegistroRapidoTransportista frm = new Mantenimientos.frmRegistroRapidoTransportista(txtRucTransportista.Text.Trim());
                        frm.ShowDialog();
                        if (frm._Guardado == true)
                        {
                            txtRucTransportista.Text = frm._NroDocumentoReturn;
                            var datosTransportista = new TransportistaBL().ObtenerTransportistaporNroDocumento(ref objOperationResult, txtRucTransportista.Text.Trim());
                            if (datosTransportista != null)
                            {


                                _guiaremisionDto.v_IdTransportista = datosTransportista.v_IdTransportista;
                                txtRazonSocial.Text = datosTransportista.v_NombreRazonSocial;
                                txtCodigo.Text = datosTransportista.v_Codigo;
                                _strIdTransportista = datosTransportista.v_IdTransportista;
                                txtRucTransportista.Text = datosTransportista.v_NumeroDocumento;

                                var listaPlacas = _objTransporteUnidadTransporte.ObtenerListadoTransportistaUnidadTransporteParaCombo(ref objOperationResult, null, null, _strIdTransportista);
                                var listaChofer = _objTransportistaChoferBl.ObtenerListadoTransportistaChoferParaCombo(ref objOperationResult, null, null, _strIdTransportista);

                                Utils.Windows.LoadUltraComboEditorList(cboPlaca, "Value1", "Id", listaPlacas, DropDownListAction.Select);
                                Utils.Windows.LoadUltraComboEditorList(cboChofer, "Value1", "Id", listaChofer, DropDownListAction.Select);

                                listaChofer = listaChofer.Where(o => o.Id != "-1").ToList();
                                listaPlacas = listaPlacas.Where(o => o.Id != "-1").ToList();
                                var idChofer = listaChofer.Any() ? listaChofer.FirstOrDefault(x => x.Id != "-1").Id : "-1";
                                var idPlaca = listaPlacas.Any() ? listaPlacas.FirstOrDefault(x => x.Id != "-1").Id : "-1";
                                cboChofer.Value = listaChofer.Any() ? listaChofer.Where(x => x.Id != "-1").ToList().Any() ? idChofer : "-1" : "-1";
                                cboPlaca.Value = listaPlacas.Any() ? listaPlacas.Where(x => x.Id != "-1").ToList().Any() ? idPlaca : "-1" : "-1";
                            }
                        }
                    }

                }
            }
        }

        private void dtpFechaEmision_ValueChanged(object sender, EventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();
            string TipoCambio = _objGuiaRemisionBl.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
            txtTipoCambio.Text = TipoCambio;
            if (_scroll >= int.Parse(dtpFechaEmision.Value.Month.ToString()))
            {
                dtpFechaTraslado.MinDate = dtpFechaEmision.Value;
                dtpFechaTraslado.Value = dtpFechaEmision.Value; // Solo esto,antes de agregar periodo
            }
            else
            {
                dtpFechaTraslado.ResetText();
                dtpFechaTraslado.MinDate = DateTime.Parse("01" + "/" + "01" + "/" + "1753");
                dtpFechaTraslado.MaxDate = DateTime.Parse("31" + "/" + "12" + "/" + "9998");
                dtpFechaTraslado.Value = dtpFechaEmision.Value;

            }
            _scroll = int.Parse(dtpFechaEmision.Value.Month.ToString());
            txtPeriodo.Text = dtpFechaEmision.Value.Year.ToString();
            txtMes.Text = dtpFechaEmision.Value.Month.ToString("00");

            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.GuiaRemision, _guiaremisionDto.t_FechaEmision, dtpFechaEmision.Value, _guiaremisionDto.v_Correlativo, 0);



            if (_objCierreMensualBl.VerificarMesCerrado(txtPeriodo.Text.Trim(), txtMes.Text.Trim(), (int)ModulosSistema.VentasFacturacion) || _utilizado == "KARDEX")
            {
                btnGuardar.Visible = false;
                this.Text = _utilizado == "KARDEX" ? "Guía de Remisión" : "Guía de Remisión [MES CERRADO]";
                if (_utilizado == "KARDEX")
                {

                    btnSalir.Visible = false;
                    btnAgregar.Visible = false;
                    btnEliminar.Visible = false;

                    btnImprimir.Visible = false;
                }
            }

            else
            {

                btnGuardar.Visible = true;
                this.Text = "Guía de Remisión";
            };

        }

        private void cboDocumentoRef_Leave(object sender, EventArgs e)
        {

            if (_strModo == "Nuevo")
            {
                if (cboDocumentoRef.Text.Trim() == "")
                {
                    cboDocumentoRef.Value = "-1";
                }
                else
                {
                    var x = _listadoComboDocumentos.Find(p => p.Id == cboDocumentoRef.Value.ToString() | p.Id == cboDocumentoRef.Text);
                    if (x == null)
                    {
                        cboDocumentoRef.Value = "-1";
                    }
                }

                if (cboDocumentoRef.Value == null)
                {
                    _extraidoDocumentoReferencia = false;

                }
                else
                {
                    if (cboDocumentoRef.Value.ToString() == "-1")
                    {
                        _extraidoDocumentoReferencia = false;
                    }
                }
            }
        }

        private void cboDocumentoRef_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (UltraGridRow row in cboDocumentoRef.Rows)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    return;
                }
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumentoRef.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumentoRef.Text.ToUpper()))
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

        private void cboDocumentoRef_AfterDropDown(object sender, EventArgs e)
        {
            foreach (UltraGridRow row in cboDocumentoRef.Rows)
            {
                if (cboDocumentoRef.Value == null) return;
                if (cboDocumentoRef.Value.ToString() == "-1") cboDocumentoRef.Text = string.Empty;
                bool filterRow = true;
                foreach (UltraGridColumn column in cboDocumentoRef.DisplayLayout.Bands[0].Columns)
                {
                    if (column.IsVisibleInLayout)
                    {
                        if (row.Cells[column].Text.Contains(cboDocumentoRef.Text.ToUpper()))
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

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc, "{0:0000}");

        }

        private void txtNumeroGuia_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtNumeroGuia, "{0:00000000}");
            ExistenciaGuiaRemision();
        }

        private void txtNumeroDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtNumeroDoc, "{0:00000000}");
        }

        private void txtNumeroDoc_Validating(object sender, CancelEventArgs e)
        {
            if (!_eventoEnter) return;
            OperationResult objOperationResult = new OperationResult();
            int correlativoDoc;
            if (!int.TryParse(txtNumeroDoc.Text, out correlativoDoc)) return;
            if (!FiltroVentas)
            {
                serieDocPrevio = txtSerieDoc.Text.Trim();
                _tipoDocumentoPrevio = int.Parse(cboDocumentoRef.Value.ToString());
                correlativoDocPrevio = txtNumeroDoc.Text.Trim();
            }
            _idVenta = _objGuiaRemisionBl.ObtenerIdVenta(ref objOperationResult, Convert.ToInt32(cboDocumentoRef.Value), txtSerieDoc.Text, correlativoDoc.ToString("00000000"));// Busco Factura
            if (_idVenta != null) // Si la Factura Existe
            {
                _redondeo = _objGuiaRemisionBl.ObtenerRedondeo(_idVenta.v_IdVenta);
                var guiaremision = _objGuiaRemisionBl.ObtenerFacturaenGuiaRemisionPrueba(ref objOperationResult, int.Parse(cboDocumentoRef.Value.ToString()), txtSerieDoc.Text, int.Parse(txtNumeroDoc.Text).ToString("00000000"), _strIdGuiaRemision); //agregagado 31 dic.
                EncontrarDocRef = true;
                if (guiaremision == true)
                {
                    if (UltraMessageBox.Show("¿El documento se ha registrado anteriormente, desea volver a registrarlo para esta guia de remisión?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        # region DocumentoRegistradoRespuestaSI
                        serieDocPrevio = txtSerieDoc.Text.Trim();
                        _tipoDocumentoPrevio = int.Parse(cboDocumentoRef.Value.ToString());
                        if (FiltroVentas) { correlativoDocPrevio = txtNumeroDoc.Text.Trim(); }
                        if (grdData.Rows.Count == 0) // Verifico la Grilla
                        {
                            if (_idVenta.v_IdCliente != null) // Si existe Cliente Factura
                            {
                                LlenarCliente(_idVenta.v_IdCliente, _idVenta.v_IdVenta);
                                txtNumeroCotizacion.Text = _idVenta.v_NroPedido;

                            }
                            else
                            {
                                LimpiarCliente();
                                txtNumeroCotizacion.Clear();
                            }
                            if (_idVenta.i_IdMoneda != -1 & _idVenta.i_IdMoneda != null)
                            {
                                cboMoneda.Value = _idVenta.i_IdMoneda.ToString();
                            }
                            else
                            {
                                cboMoneda.Value = "-1";
                            }

                            CargarComboDetalle();
                            _objGuiaRemisionBl.EliminarTemporal();
                            _objGuiaRemisionBl.InsertarDetalleVentaADetalleGuiaRemision(ref objOperationResult, _idVenta.v_IdVenta);
                            CargarDetalleTemporal();
                            _objGuiaRemisionBl.EliminarTemporal(); //Temporal=True
                            //#region Agregado recientemente
                            //RecibirDetallesDeFactura(IdVenta.v_IdVenta);
                            //#endregion

                        }
                        else  // Si no está vacia 
                        {

                            if (UltraMessageBox.Show("¿Al realizar la edición del Documento de referencia perderá todos los detalles de esta Guía de Remisión,Desea Continuar?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {

                                if (_strModo == "Edicion" | _strModo == "Guardado")
                                {
                                    foreach (UltraGridRow Fila in grdData.Rows)
                                    {
                                        _guiaremisiondetalleDto = new guiaremisiondetalleDto();
                                        // if (Fila.Cells["v_IdGuiaRemisionDetalle"].Value != null)
                                        //{
                                        if (Fila.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
                                        {
                                            _guiaremisiondetalleDto.v_IdGuiaRemisionDetalle = Fila.Cells["v_IdGuiaRemisionDetalle"].Value.ToString();
                                            _TempDetalle_EliminarDto.Add(_guiaremisiondetalleDto);
                                        }
                                        //}

                                    }
                                }

                                grdData.Selected.Rows.AddRange((UltraGridRow[])this.grdData.Rows.All);
                                grdData.DeleteSelectedRows(false);

                                if (_idVenta.v_IdCliente != null) // Si existe Cliente Factura
                                {
                                    LlenarCliente(_idVenta.v_IdCliente, _idVenta.v_IdVenta);
                                    txtNumeroCotizacion.Text = _idVenta.v_NroPedido;
                                }
                                else
                                {
                                    LimpiarCliente();
                                    txtNumeroCotizacion.Clear();

                                }
                                if (_idVenta.i_IdMoneda != -1 & _idVenta.i_IdMoneda != null)
                                {
                                    cboMoneda.Value = _idVenta.i_IdMoneda.ToString();
                                }
                                else
                                {
                                    cboMoneda.Value = "-1";
                                }
                                CargarComboDetalle();
                                _objGuiaRemisionBl.EliminarTemporal();
                                _objGuiaRemisionBl.InsertarDetalleVentaADetalleGuiaRemision(ref objOperationResult, _idVenta.v_IdVenta);
                                CargarDetalleTemporal();
                                _objGuiaRemisionBl.EliminarTemporal();
                                Temporal = true;
                                serieDocPrevio = txtSerieDoc.Text.Trim();
                                _tipoDocumentoPrevio = int.Parse(cboDocumentoRef.Value.ToString());
                                if (FiltroVentas)
                                {
                                    correlativoDocPrevio = txtNumeroDoc.Text.Trim();
                                }
                            }

                            else
                            {
                                if (FiltroVentas)
                                {
                                    txtNumeroDoc.Text = correlativoDocPrevio;
                                    txtSerieDoc.Text = serieDocPrevio;
                                    cboDocumentoRef.Value = _tipoDocumentoPrevio == 0 ? "-1" : _tipoDocumentoPrevio.ToString();
                                }
                                else
                                {
                                    txtNumeroDoc.Undo();
                                    txtNumeroDoc.ClearUndo();
                                    txtSerieDoc.Undo();
                                    txtSerieDoc.ClearUndo();
                                }
                            }


                        }//Termina respuesta Si
                        #endregion
                    }
                    else
                    {
                        if (FiltroVentas)
                        {
                            txtNumeroDoc.Text = correlativoDocPrevio;
                            txtSerieDoc.Text = serieDocPrevio;
                            cboDocumentoRef.Value = _tipoDocumentoPrevio == 0 ? "-1" : _tipoDocumentoPrevio.ToString();

                        }
                        else
                        {
                            txtNumeroDoc.Undo();
                            txtNumeroDoc.ClearUndo();
                            txtSerieDoc.Undo();
                            txtSerieDoc.ClearUndo();
                            cboDocumentoRef.Value = _tipoDocumentoPrevioChange == 0 ? "-1" : _tipoDocumentoPrevioChange.ToString();
                        }
                    }
                }
                else // Si la factura no ha sido registrada en la guia -Buscamos nuevamente la factura
                {
                    if (grdData.Rows.Count == 0) // Verifico la Grilla
                    {
                        if (_idVenta.v_IdCliente != null) // Si existe Cliente Factura
                        {
                            LlenarCliente(_idVenta.v_IdCliente, _idVenta.v_IdVenta);
                            txtNumeroCotizacion.Text = _idVenta.v_NroPedido;
                        }
                        else
                        {
                            LimpiarCliente();
                            txtNumeroCotizacion.Clear();

                        }
                        if (_idVenta.i_IdMoneda != null && _idVenta.i_IdMoneda != -1)
                        {
                            cboMoneda.Value = _idVenta.i_IdMoneda.ToString();
                        }
                        else
                        {
                            cboMoneda.Value = "-1";
                        }
                        CargarComboDetalle();
                        _objGuiaRemisionBl.EliminarTemporal();
                        _objGuiaRemisionBl.InsertarDetalleVentaADetalleGuiaRemision(ref objOperationResult, _idVenta.v_IdVenta);
                        CargarDetalleTemporal();
                        _objGuiaRemisionBl.EliminarTemporal();

                        Temporal = true;
                    }

                    else  // Si no está vacia 
                    {
                        if (UltraMessageBox.Show("¿Al realizar la edición del Documento de referencia perderá todos los detalles de esta Guia de Remisión,Desea Continuar?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (_strModo == "Edicion" || _strModo == "Guardado")
                            {
                                foreach (UltraGridRow Fila in grdData.Rows)
                                {
                                    _guiaremisiondetalleDto = new guiaremisiondetalleDto();

                                    if (Fila.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
                                    {
                                        _guiaremisiondetalleDto.v_IdGuiaRemisionDetalle = Fila.Cells["v_IdGuiaRemisionDetalle"].Value.ToString();
                                        _TempDetalle_EliminarDto.Add(_guiaremisiondetalleDto);
                                    }
                                }
                            }
                            grdData.Selected.Rows.AddRange((UltraGridRow[])this.grdData.Rows.All);
                            grdData.DeleteSelectedRows(false);
                            serieDocPrevio = txtSerieDoc.Text.Trim();
                            _tipoDocumentoPrevio = int.Parse(cboDocumentoRef.Value.ToString());
                            if (_idVenta.v_IdCliente != null) // Si existe Cliente Factura
                            {
                                LlenarCliente(_idVenta.v_IdCliente, _idVenta.v_IdVenta);
                                txtNumeroCotizacion.Text = _idVenta.v_NroPedido;
                            }
                            else
                            {
                                LimpiarCliente();
                                txtNumeroCotizacion.Clear();
                            }
                            if (_idVenta.i_IdMoneda != -1 & _idVenta.i_IdMoneda != null)
                            {
                                cboMoneda.Value = _idVenta.i_IdMoneda.ToString();
                            }
                            else
                            {
                                cboMoneda.Value = "-1";
                            }
                            CargarComboDetalle();
                            _objGuiaRemisionBl.EliminarTemporal();
                            _objGuiaRemisionBl.InsertarDetalleVentaADetalleGuiaRemision(ref objOperationResult, _idVenta.v_IdVenta);
                            CargarDetalleTemporal();
                            _objGuiaRemisionBl.EliminarTemporal();
                            Temporal = true;
                        } //Si la rpta es No-- Ya no se Busca Factura

                        else  // Si no está vacia 
                        {
                            if (FiltroVentas)
                            {
                                txtNumeroDoc.Text = correlativoDocPrevio;
                                txtSerieDoc.Text = serieDocPrevio;
                                cboDocumentoRef.Value = _tipoDocumentoPrevio == 0 ? "-1" : _tipoDocumentoPrevio.ToString();
                            }
                            else
                            {
                                txtNumeroDoc.Undo();
                                txtNumeroDoc.ClearUndo();
                                txtSerieDoc.Undo();
                                txtSerieDoc.ClearUndo();
                                cboDocumentoRef.Value = _tipoDocumentoPrevioChange == 0 ? "-1" : _tipoDocumentoPrevioChange.ToString();
                            }
                        }
                    }
                }
            }

            else // Si la Factura no Existe 
            {
                UltraMessageBox.Show("Este Documento no existe en el Sistema.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (grdData.Rows.Count != 0)
                {
                    if (UltraMessageBox.Show("¿Al realizar la edición del Documento de referencia perderá todos los detalles de esta Guia de Remisión,Desea Continuar?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (_strModo == "Edicion" | _strModo == "Guardado")
                        {
                            foreach (UltraGridRow Fila in grdData.Rows)
                            {
                                _guiaremisiondetalleDto = new guiaremisiondetalleDto();

                                if (Fila.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
                                {
                                    _guiaremisiondetalleDto.v_IdGuiaRemisionDetalle = Fila.Cells["v_IdGuiaRemisionDetalle"].Value.ToString();
                                    _TempDetalle_EliminarDto.Add(_guiaremisiondetalleDto);
                                }
                            }
                        }
                        grdData.Selected.Rows.AddRange((UltraGridRow[])this.grdData.Rows.All);
                        grdData.DeleteSelectedRows(false);
                        serieDocPrevio = txtSerieDoc.Text.Trim();
                        _tipoDocumentoPrevio = int.Parse(cboDocumentoRef.Value.ToString());
                        correlativoDocPrevio = txtNumeroDoc.Text.Trim();

                        if (_idVenta != null) // Si existe Cliente Factura
                        {
                            LlenarCliente(_idVenta.v_IdCliente, _idVenta.v_IdVenta);
                            txtNumeroCotizacion.Text = _idVenta.v_NroPedido;
                            cboMoneda.Value = _idVenta.i_IdMoneda.ToString();
                        }
                        else
                        {
                            LimpiarCliente();
                            txtNumeroCotizacion.Clear();
                        }
                    }
                    else
                    {
                        // Como no Existe Guia Remision , se llena antes Grilla
                        if (FiltroVentas)
                        {
                            txtNumeroDoc.Text = correlativoDocPrevio;
                            txtSerieDoc.Text = serieDocPrevio;
                            cboDocumentoRef.Value = _tipoDocumentoPrevio == 0 ? "-1" : _tipoDocumentoPrevio.ToString();
                        }
                        else
                        {
                            txtNumeroDoc.Undo();
                            txtNumeroDoc.ClearUndo();
                            txtSerieDoc.Undo();
                            txtSerieDoc.ClearUndo();
                            cboDocumentoRef.Value = _tipoDocumentoPrevioChange == 0 ? "-1" : _tipoDocumentoPrevioChange.ToString();
                        }
                        e.Cancel = true;
                    }
                }
            }
            _tipoDocumentoPrevioChange = _tipoDocumentoPrevio;
            ConfiguracionGrilla();


        }

        private void RecibirDetallesDeFactura(string idVenta)
        {
            var objOperationResult = new OperationResult();
            var ds = _objGuiaRemisionBl.ObtenerDetallesVentaParaGuiaRemision(ref objOperationResult, idVenta);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            grdData.DataSource = ds;

            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
                grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";

                if (Globals.ClientSession.i_IncluirPreciosGuiaRemision.ToString() == "0")
                {

                    grdData.Rows[i].Cells["d_Precio"].Value = "0";
                    grdData.Rows[i].Cells["d_Total"].Value = "0";
                }
            }
            CalcularValoresTemporales();
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
                    if (Mes >= 1 && Mes <= 12)
                    {
                        if (_strModo == "Nuevo")
                        {
                            ObtenerListadoGuiaRemision(txtPeriodo.Text, txtMes.Text);
                        }
                        else if (_strModo == "Guardado")
                        {
                            _strModo = "Consulta";
                            ObtenerListadoGuiaRemision(txtPeriodo.Text, txtMes.Text);
                        }
                        else
                        {
                            ObtenerListadoGuiaRemision(txtPeriodo.Text, txtMes.Text);
                        }
                    }
                    else
                    {
                        UltraMessageBox.Show("Mes inválido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
            }
        }

        private void txtMes_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtMes, e);
        }

        private void txtMes_Leave(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
            if (txtMes.Text != "")
            {
                int Mes;
                Mes = int.Parse(txtMes.Text);
                if (Mes >= 1 && Mes <= 12)
                {
                    if (_strModo == "Nuevo")
                    {
                        ObtenerListadoGuiaRemision(txtPeriodo.Text, txtMes.Text);
                    }
                    else if (_strModo == "Guardado")
                    {
                        _strModo = "Consulta";
                        ObtenerListadoGuiaRemision(txtPeriodo.Text, txtMes.Text);
                    }
                    else
                    {
                        ObtenerListadoGuiaRemision(txtPeriodo.Text, txtMes.Text);
                    }
                }
                else
                {
                    UltraMessageBox.Show("Mes inválido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void btnBuscarTransportista_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            Mantenimientos.frmBuscarTransportista frm = new Mantenimientos.frmBuscarTransportista(txtRucTransportista.Text.Trim());
            frm.ShowDialog();
            if (frm.StrIdTransportista != null)
            {

                _guiaremisionDto.v_IdTransportista = frm.StrIdTransportista;
                _strIdTransportista = frm.StrIdTransportista;
                txtRucTransportista.Text = frm.StrNroDocumento;
                txtRazonSocial.Text = frm.StrRazonSocial;
                txtCodigo.Text = frm.StrCodigo;
                var ListaChofer = _objTransportistaChoferBl.ObtenerListadoTransportistaChoferParaCombo(ref objOperationResult, null, null, _strIdTransportista);
                var ListaPlacas = _objTransporteUnidadTransporte.ObtenerListadoTransportistaUnidadTransporteParaCombo(ref objOperationResult, null, null, _strIdTransportista);

                Utils.Windows.LoadUltraComboEditorList(cboPlaca, "Value1", "Id", ListaPlacas, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboChofer, "Value1", "Id", ListaChofer, DropDownListAction.Select);

                cboChofer.Value = ListaChofer.Any() ? ListaChofer.Where(x => x.Id != "-1").ToList().Any() ? ListaChofer.Where(x => x.Id != "-1").FirstOrDefault().Id.ToString() : "-1" : "-1";
                cboPlaca.Value = ListaPlacas.Any() ? ListaPlacas.Where(x => x.Id != "-1").ToList().Any() ? ListaPlacas.Where(x => x.Id != "-1").FirstOrDefault().Id.ToString() : "-1" : "-1";

            }

            else
            {

                //UltraMessageBox.Show("El Codigo de Transportista no existe en el sistema", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtRazonSocial.Clear();
                txtCodigo.Clear();
                txtMarca.Clear();
                txtNumInscripcion.Clear();
                txtNombreChofer.Clear();
                Utils.Windows.LoadUltraComboEditorList(cboPlaca, "Value1", "Id", _objTransporteUnidadTransporte.ObtenerListadoTransportistaUnidadTransporteParaCombo(ref objOperationResult, null, null, ""), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboChofer, "Value1", "Id", _objTransportistaChoferBl.ObtenerListadoTransportistaChoferParaCombo(ref objOperationResult, null, null, ""), DropDownListAction.Select);
                cboPlaca.Value = "-1";
                cboChofer.Value = "-1";
                return;
            }
        }

        private void txtRucCliente_KeyDown(object sender, KeyEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (!txtRucCliente.IsDroppedDown && e.KeyCode == Keys.Enter)
            {
                if (txtRucCliente.Text.Trim() != "" & txtRucCliente.TextLength <= 5)
                {
                    var frm = new frmBuscarCliente("VV", txtRucCliente.Text.Trim());
                    frm.ShowDialog();

                    if (frm._IdCliente != null)
                    {
                        _guiaremisionDto.v_IdCliente = frm._IdCliente;
                        txtCodigoCliente.Text = frm._CodigoCliente;
                        txtRazonCliente.Text = frm._RazonSocial;
                        txtRucCliente.Text = frm._NroDocumento;
                        txtLlegada.Text = frm._Direccion;
                        LoadDirectionByClient(frm._IdCliente, frm._IdDireccionCliente);
                    }
                    else
                    {
                        txtCodigoCliente.Clear();
                        txtRazonCliente.Clear();
                        txtLlegada.Clear();
                        txtLlegada.DataSource = null;
                    }
                }
                else if (txtRucCliente.TextLength == 8 || txtRucCliente.TextLength == 11)
                {

                    string[] DatosCliente = new string[3];
                    DatosCliente = _objGuiaRemisionBl.DevolverClientePorNroDocumento(ref objOperationResult, txtRucCliente.Text.Trim());
                    if (DatosCliente != null)
                    {

                        _guiaremisionDto.v_IdCliente = DatosCliente[0];
                        txtCodigoCliente.Text = DatosCliente[1];
                        txtRazonCliente.Text = DatosCliente[2];

                        txtLlegada.Text = DatosCliente[3];
                        LoadDirectionByClient(DatosCliente[0], int.Parse(DatosCliente[4]));
                    }
                    else
                    {
                        // Cliente rápido
                        Mantenimientos.frmRegistroRapidoCliente frm = new Mantenimientos.frmRegistroRapidoCliente(txtRucCliente.Text.Trim(), "C");
                        frm.ShowDialog();
                        if (frm._Guardado == true)
                        {
                            txtRucCliente.Text = frm._NroDocumentoReturn;
                            DatosCliente = _objGuiaRemisionBl.DevolverClientePorNroDocumento(ref objOperationResult, txtRucCliente.Text.Trim());
                            if (DatosCliente != null)
                            {
                                _guiaremisionDto.v_IdCliente = DatosCliente[0];
                                txtCodigoCliente.Text = DatosCliente[1];
                                txtRazonCliente.Text = DatosCliente[2];
                                txtLlegada.Text = DatosCliente[3];
                                LoadDirectionByClient(DatosCliente[0], int.Parse(DatosCliente[4]));
                            }
                        }
                    }
                }
            }
        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            var frm = new frmBuscarCliente("VV", txtRucCliente.Text.Trim());
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {
                _guiaremisionDto.v_IdCliente = frm._IdCliente;
                txtRucCliente.Text = frm._NroDocumento;
                txtCodigoCliente.Text = frm._CodigoCliente;
                txtRazonCliente.Text = frm._RazonSocial;
                txtLlegada.Text = frm._Direccion;
                LoadDirectionByClient(frm._IdCliente, frm._IdDireccionCliente);
            }
        }

        private void LoadDirectionByClient(string id, int IdDireccionCliente)
        {
            //return;
            var objOperationResult = new OperationResult();
            var data = new ClienteBL().ObtenerDireccionClienteParaCombo(ref objOperationResult, id);
            txtLlegada.DataSource = data;
            if (data == null) return;

            //var pred = data.FirstOrDefault(r => r.i_EsDireccionPredeterminada == 1);
            var pred = data.FirstOrDefault(r => r.i_IdDireccionCliente == IdDireccionCliente);
            if (pred != null)
            {
                txtLlegada.Value = pred.i_IdDireccionCliente;
                SetUbigueoLlegada(pred);
            }
        }

        private void cboDirCliente_ValueChanged(object sender, EventArgs e)
        {
            var value = txtLlegada.SelectedItem;
            if (value == null)
            {
                txtUbigueoLlegada.Clear();
                return;
            }
            var obj = (clientedireccionesDto)value.ListObject;

            SetUbigueoLlegada(obj);
        }

        private void SetUbigueoLlegada(clientedireccionesDto obj)
        {
            if (obj == null)
            {
                txtUbigueoLlegada.Clear();
                return;
            }

            if (obj.i_IdDepartamento == null ||
                obj.i_IdProvincia == null ||
                obj.i_IdDistrito == null)
            {
                txtUbigueoLlegada.Clear();
                return;
            }

            var ubigueo = new SystemParameterBL().GetUbigueoByIds(
                obj.i_IdDepartamento ?? 1, obj.i_IdProvincia ?? 1, obj.i_IdDistrito ?? 1
            );
            txtUbigueoLlegada.Text = ubigueo;
        }

        private void txtRucTransportista_TextChanged(object sender, EventArgs e)
        {
            if (txtRucTransportista.Text == "")
            {
                txtRazonSocial.Clear();
                txtCodigo.Clear();
                cboPlaca.Value = "-1";
                cboChofer.Value = "-1";
                _guiaremisionDto.v_IdTransportista = null;
            }

        }

        private void txtRucCliente_TextChanged(object sender, EventArgs e)
        {
            if (txtRucCliente.Text == "")
            {
                txtRazonCliente.Clear();
                txtCodigoCliente.Clear();
                txtPartida.Clear();
                _guiaremisionDto.v_IdCliente = null;
            }
        }

        private void txtNumeroDoc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Utils.Windows.FijarFormatoUltraTextBox(txtNumeroDoc, "{0:00000000}");
                _eventoEnter = true;
                txtNumeroDoc_Validating(sender, new CancelEventArgs());
                _eventoEnter = false;
            }
        }
        private void txtSerieGuia_Validating(object sender, CancelEventArgs e)
        {
            // txtNumeroGuia.Text = _objDocumentoBL.CorrelativoxSerie(Globals.ClientSession.i_IdEstablecimiento, 9).ToString("00000000");

            if (txtSerieGuia.Text.Trim() != String.Empty)
            {
                int Serie = 0;
                if (int.TryParse(txtSerieGuia.Text.Trim(), out Serie))
                {
                    Utils.Windows.FijarFormatoUltraTextBox(txtSerieGuia, "{0:0000}");
                }

                string CorrelativoGuia = _objDocumentoBl.CorrelativoxSerie(int.Parse(cboTipoGuia.Value.ToString()), txtSerieGuia.Text.Trim());
                if (CorrelativoGuia != null)
                {
                    txtNumeroGuia.Text = CorrelativoGuia;
                }
                else
                {
                    UltraMessageBox.Show("No se encuentra registrado la serie en la configuración del Establecimiento", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtSerieDoc.Focus();
                    return;
                }
            }
            else
            {
                txtNumeroGuia.Text = string.Empty;

            }

        }

        private void cboTipoGuia_Leave(object sender, EventArgs e)
        {
            if (_strModo == "Nuevo")
            {
                if (cboTipoGuia.Text.Trim() == "")
                {
                    cboTipoGuia.Value = "-1";
                }
                else
                {
                    var x = _listadoComboGuias.Find(p => p.Id == cboTipoGuia.Value.ToString() || p.Id == cboTipoGuia.Text);
                    if (x == null)
                    {
                        cboTipoGuia.Value = "-1";
                    }
                }
                if (!string.IsNullOrEmpty(UserConfig.Default.SerieCaja) && UserConfig.Default.SerieCaja != "--Seleccionar--")
                {
                    txtSerieGuia.Text = UserConfig.Default.SerieCaja;
                }
                else
                {
                    txtSerieGuia.Text = _objDocumentoBl.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboTipoGuia.Value.ToString())).Trim();
                }
                // txtNumeroGuia.Text = _objDocumentoBl.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboTipoGuia.Value.ToString())).ToString("00000000").Trim();
                object s1 = new object();
                CancelEventArgs e1 = new CancelEventArgs();
                txtSerieGuia_Validating(s1, e1);
                ComprobarExistenciaCorrelativoDocumento();
                if (cboTipoGuia.Value != null && int.Parse(cboTipoGuia.Value.ToString()) == (int)TiposDocumentos.GuiaInterna)
                {
                    cboDocumentoRef.Value = "-1";
                    txtSerieDoc.Text = string.Empty;
                    txtNumeroDoc.Text = string.Empty;
                    _extraidoDocumentoReferencia = false;
                    ConfiguracionGrilla();
                    CalcularValoresDetalle();
                }

            }
        }

        private void btnSalirPanel_Click(object sender, EventArgs e)
        {
            pnlVariosTiposImpresion.Visible = false;
            btnImprimir.Enabled = true;
            btnGuardar.Enabled = true;
        }

        private void btnImprimirTipoFormato_Click(object sender, EventArgs e)
        {
            bool ImpresionVistaPrevia = _objDocumentoBl.ImpresionVistaPrevia(int.Parse(cboTipoGuia.Value.ToString()), txtSerieGuia.Text.Trim());
            if (rbtnFormatoGuia.Checked)
            {
                var frm = new Reportes.Ventas.frmDocumentoGuiaRemision(_strIdGuiaRemision, ImpresionVistaPrevia, Reportes.TiposReportes.GuiaRemisionVenta);
                if (ImpresionVistaPrevia)
                    frm.ShowDialog();
                else
                    btnImprimir.Enabled = false;
            }
            else
            {
                List<string> ListaGuiaRemision = new List<string>();
                ListaGuiaRemision.Add(_strIdGuiaRemision);
                Reportes.Ventas.frmDocumentoGuiaOtroFormato frm = new Reportes.Ventas.frmDocumentoGuiaOtroFormato(ListaGuiaRemision, ImpresionVistaPrevia, false);

                if (ImpresionVistaPrevia)
                {
                    frm.ShowDialog();
                }
                else
                {
                    btnImprimir.Enabled = false;
                }

            }


        }

        private void uckAfectoIgv_CheckedChanged(object sender, EventArgs e)
        {
            uckPreciosIncluyenIgv.Enabled = uckAfectoIgv.Checked == true ? true : false;
            if (!uckPreciosIncluyenIgv.Enabled)
            {
                uckPreciosIncluyenIgv.Checked = false;
            }
            CalcularValoresDetalle();
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
            }
        }

        private void uckPreciosIncluyenIgv_CheckedChanged(object sender, EventArgs e)
        {
            CalcularValoresDetalle();
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
            }
        }

        #endregion

        #region Validaciones-Otros Procedimientos
        //private void ValidarFechas()
        //{

        //    if (DateTime.Now.Year.ToString().Trim() == txtPeriodo.Text.Trim())
        //    {
        //        if (strModo == "Nuevo")
        //        {
        //            dtpFechaEmision.Value = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
        //            dtpFechaTraslado.Value = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
        //        }

        //        dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
        //        dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim()))).ToString()).ToString());
        //        dtpFechaTraslado.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
        //        dtpFechaTraslado.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim()))).ToString()).ToString());
        //    }
        //    else
        //    {
        //        if (strModo == "Nuevo")
        //        {
        //            dtpFechaEmision.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
        //            dtpFechaTraslado.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());

        //        }
        //        dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/01").ToString());
        //        dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
        //        dtpFechaTraslado.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/01").ToString());
        //        dtpFechaTraslado.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());

        //    }
        //}

        private void ValidarFechas()
        {

            if (DateTime.Now.Year.ToString().Trim() == txtPeriodo.Text.Trim())
            {
                if (_strModo == "Nuevo")
                {
                    dtpFechaEmision.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim()))).ToString()).ToString() + " 23:59");
                    dtpFechaTraslado.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaTraslado.MinDate = dtpFechaEmision.Value;


                }
                else
                {


                    if (int.Parse(_guiaremisionDto.v_Mes.Trim()) <= int.Parse(DateTime.Now.Month.ToString()))
                    {
                        dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + DateTime.Now.Month.ToString() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))).ToString()).ToString() + " 23:59");

                    }

                    dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaTraslado.MinDate = dtpFechaEmision.Value;

                }
            }
            else
            {
                if (_strModo == "Nuevo")
                {
                    dtpFechaEmision.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString() + " 23:59");
                    dtpFechaTraslado.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaTraslado.MinDate = dtpFechaEmision.Value;
                    dtpFechaTraslado.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString() + " 23:59");
                }
                else
                {

                    dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/01").ToString());
                    dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString() + " 23:59");
                    dtpFechaTraslado.MinDate = dtpFechaEmision.Value;
                    dtpFechaTraslado.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString() + " 23:59");
                }

            }
        }

        private void LimpiarCliente()
        {
            txtRucCliente.Clear();
            txtCodigoCliente.Clear();
            txtRazonCliente.Clear();
            txtLlegada.Clear();
            cboMoneda.Value = "-1";
            _guiaremisionDto.v_IdCliente = null;
            _guiaremisionDto.i_IdDireccionCliente = -1;

        }
        private void LlenarCliente(string IdCliente, string IdVenta)
        {
            OperationResult objOperationResult = new OperationResult();
            clienteDto Cliente = new clienteDto();
            _guiaremisionDto.v_IdCliente = IdCliente;
            Cliente = _objGuiaRemisionBl.ObtenerClienteporId(ref objOperationResult, IdCliente, IdVenta);
            txtRucCliente.Text = Cliente == null ? "" : Cliente.v_NroDocIdentificacion;
            txtCodigoCliente.Text = Cliente == null ? "" : Cliente.v_CodCliente;
            txtRazonCliente.Text = Cliente == null ? "" : string.Join(" ", Cliente.v_PrimerNombre, Cliente.v_SegundoNombre, Cliente.v_ApePaterno, Cliente.v_ApeMaterno, Cliente.v_RazonSocial);
            txtLlegada.Text = Cliente == null ? "" : Cliente.v_DirecPrincipal.Trim();
            //txtLlegada.Value = Cliente.i_IdDireccionCliente;
            if (Cliente != null) LoadDirectionByClient(IdCliente, Cliente.i_IdDireccionCliente);
            else txtLlegada.DataSource = null;
            _guiaremisionDto.i_IdDireccionCliente = Cliente.i_IdDireccionCliente;

        }

        #endregion

        #region Imprimir
        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (ValidarImprimir.Validate(true, false).IsValid)
            {
                bool ImpresionVistaPrevia = _objDocumentoBl.ImpresionVistaPrevia(int.Parse(cboTipoGuia.Value.ToString()), txtSerieGuia.Text.Trim());
                var EstablecimientoDetalle = _objDocumentoBl.ConfiguracionEstablecimiento(int.Parse(cboTipoGuia.Value.ToString()), txtSerieGuia.Text.Trim());
                if (EstablecimientoDetalle == null)
                {
                    UltraMessageBox.Show(string.Format("Documento {0} {1} no se encuentra asignado en Establecimiento ", cboTipoGuia.Text, txtSerieGuia.Text), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var frm = new Reportes.Ventas.frmDocumentoGuiaRemision(_guiaremisionDto.v_IdGuiaRemision, ImpresionVistaPrevia, Reportes.TiposReportes.GuiaRemisionVenta);
                if (ImpresionVistaPrevia)
                {
                    frm.ShowDialog();
                }
                else
                {
                    btnImprimir.Enabled = false;
                }

            }


        }
        #endregion

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                CalcularValoresFila(Fila);
                Fila.Cells["i_RegistroEstado"].Value = "Modificado";
            }
            UltraMessageBox.Show("Todos los Cáculos han finalizado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void cboIGV_ValueChanged(object sender, EventArgs e)
        {
            CalcularValoresDetalleComboIgv();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cboRazonAgencia_Validated(object sender, EventArgs e)
        {
            if (cboRazonAgencia.Value == null)
            {
                cboRazonAgencia.Value = "-1";
            }
        }

        private void cboRazonAgencia_Leave(object sender, EventArgs e)
        {

            if (cboRazonAgencia.Value != null)
            {
                if (cboRazonAgencia.Text.Trim() == "")
                {
                    cboRazonAgencia.Value = "-1";
                }
                else
                {
                    var x = _listadoTransportistas.Find(p => p.Id == cboRazonAgencia.Value.ToString() || p.Id == cboRazonAgencia.Text);
                    if (x == null)
                    {
                        cboRazonAgencia.Value = "-1";
                    }
                }

            }
        }

        private void btnNuevoMovimiento_Click(object sender, EventArgs e)
        {
            _strModo = "Nuevo";
            _mode = "New";
            _guiaremisionDto = new guiaremisionDto();
            OperationResult objOperationResult = new OperationResult();
            ObtenerListadoGuiaRemision(Globals.ClientSession.i_Periodo.ToString(), DateTime.Now.Month.ToString("00"));
            _objGuiaRemisionBl.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);


        }

        private void txtCorrelativo_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativo, "{0:00000000}");
        }

        private void txtCorrelativo_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativo, e);
        }

        private void GetDetalleNotaSalida(string idNota)
        {
            var objOperationResult = new OperationResult();
            var mov = _objMovimientoBl.ObtenerMovimientoCabecera(ref objOperationResult, idNota);
            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show(objOperationResult.ExceptionMessage, "Error en busqueda", Icono: MessageBoxIcon.Error);
                return;
            }
            if (mov.v_IdCliente != null)
                LlenarCliente(mov.v_IdCliente, _idVenta == null ? null : _idVenta.v_IdVenta);
            else
            {
                LimpiarCliente();
                txtNumeroCotizacion.Clear();
            }

            cboMoneda.Value = mov.i_IdMoneda != null ? mov.i_IdMoneda.ToString() : "-1";

            var details = _objMovimientoBl.ObtenerMovimientoDetalles(ref objOperationResult, idNota);
            grdData.DataSource = new BindingList<Gridtemporalventadetalle>(details.Select(n => new Gridtemporalventadetalle
                                {

                                    //i_IdAlmacen = mov.i_IdAlmacenOrigen ?? 1,
                                    i_IdAlmacen = Globals.ClientSession.i_IdAlmacenPredeterminado.Value,
                                    v_IdProductoDetalle = n.v_IdProductoDetalle,
                                    d_Cantidad = n.d_Cantidad ?? 0,
                                    i_IdUnidadMedida = n.i_IdUnidadMedidaProducto.Value,
                                    d_CantidadEmpaque = n.d_CantidadEmpaque ?? 0,
                                    d_Precio = n.d_Precio ?? 0,
                                    d_Total = n.d_Total ?? 0,
                                    v_IdMovimientoDetalle = n.v_IdMovimientoDetalle,
                                    d_Valor = 0,
                                    v_Descuento = "0",
                                    d_Descuento = 0,
                                    d_ValorVenta = 0,
                                    d_Igv = 0,
                                    v_IdGuiaRemisionDetalle = null,
                                }).ToList());
            BuscarProductosTemporal();
            foreach (var row in grdData.Rows)
            {
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_EsServicio"].Value = row.Cells["v_IdProductoDetalle"].Value == null ? 0 : _objGuiaRemisionBl.EsServicio(row.Cells["v_IdProductoDetalle"].Value.ToString());
                if (Globals.ClientSession.i_IncluirPreciosGuiaRemision.ToString() == "0")
                {
                    row.Cells["d_Precio"].Value = "0";
                    row.Cells["d_Total"].Value = "0";
                }
                _extraidoDocumentoReferencia = true;
            }
            CalcularValoresTemporales();
        }

        private void GetDetallePedido(string idPedido)
        {
            var objOperationResult = new OperationResult();
            var ped = new PedidoBL().ObtenerPedidoCabecera(ref objOperationResult, idPedido);
            if (objOperationResult.Success != 1)
            {
                UltraMessageBox.Show(objOperationResult.ExceptionMessage, "Error en busqueda", Icono: MessageBoxIcon.Error);
                return;
            }
            if (ped.v_IdCliente != null)
                LlenarCliente(ped.v_IdCliente, _idVenta == null ? null : _idVenta.v_IdVenta);
            else
            {
                LimpiarCliente();
                txtNumeroCotizacion.Clear();
            }

            cboMoneda.Value = ped.i_IdMoneda != null ? ped.i_IdMoneda.ToString() : "-1";
            var details = new PedidoBL().ObtenerPedidoDetalles(ref objOperationResult, idPedido);
            grdData.DataSource = new BindingList<Gridtemporalventadetalle>(details.Select(n => new Gridtemporalventadetalle
            {
                //v_IdTemporalVentaD = n.v_IdMovimientoDetalle,
                i_IdAlmacen = n.i_IdAlmacen ?? -1,
                v_IdProductoDetalle = n.v_IdProductoDetalle,
                //i_IdUnidadEmpaque = n.i_IdUnidadEmpaque == null ? -1 : n.i_IdUnidadEmpaque.Value,
                d_Cantidad = n.d_Cantidad ?? 0,
                i_IdUnidadMedida = n.i_IdUnidadMedidaProducto.Value,
                d_CantidadEmpaque = n.d_CantidadEmpaque ?? 0,
                d_Precio = n.d_PrecioUnitario ?? 0,
                d_Total = n.d_PrecioVenta ?? 0,
                v_IdMovimientoDetalle = n.v_IdPedidoDetalle,
                d_Valor = n.d_Valor,
                v_Descuento = n.v_Descuento,
                d_Descuento = n.d_Descuento,
                d_ValorVenta = n.d_ValorVenta,
                d_Igv = n.d_Igv,
                v_IdGuiaRemisionDetalle = null,
            }).ToList());
            BuscarProductosTemporal();
            foreach (var row in grdData.Rows)
            {
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_EsServicio"].Value = row.Cells["v_IdProductoDetalle"].Value == null ? 0 : _objGuiaRemisionBl.EsServicio(row.Cells["v_IdProductoDetalle"].Value.ToString());
                if (Globals.ClientSession.i_IncluirPreciosGuiaRemision.ToString() == "0")
                {
                    row.Cells["d_Precio"].Value = "0";
                    row.Cells["d_Total"].Value = "0";
                }
                _extraidoDocumentoReferencia = true;
            }
            CalcularValoresTemporales();
        }

        private void txtCorrelativo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            switch (e.Button.Key)
            {
                case "btnAnterior":
                    if (OnAnterior == null) return;
                    OnAnterior();
                    if (!string.IsNullOrWhiteSpace(_idRecibido))
                    {
                        CargarCabecera(_idRecibido);
                        btnImprimir.Enabled = _btnImprimir;
                    }

                    break;
                default:
                    if (OnSiguiente == null) return;
                    OnSiguiente();
                    if (!string.IsNullOrWhiteSpace(_idRecibido))
                    {
                        CargarCabecera(_idRecibido);
                        btnImprimir.Enabled = _btnImprimir;
                    }
                    break;
            }
        }

        private void txtUbigueoLlegada_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var textEditor = (Infragistics.Win.UltraWinEditors.UltraTextEditor)sender;
            var ubigueo = textEditor.Text;
            var frm = new frmBusquedaUbigueo(ubigueo);
            if (frm.ShowDialog() != DialogResult.OK)
                return;

            textEditor.Text = frm.Ubigueo;
        }

        private decimal GetNumberFromText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;
            decimal res;
            decimal.TryParse(value, out res);

            return res;
        }
        #region Guia Electronica

        #endregion
    }

}


