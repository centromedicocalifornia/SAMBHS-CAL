using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.Common.BL;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Almacen.BL;
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Security.BL;
using System.Reflection;
using SAMBHS.Contabilidad.BL;
using LoadingClass;
using SAMBHS.Windows.WinClient.UI.Mantenimientos.UserControls;
using Infragistics.Win;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using SAMBHS.Common.DataModel;
namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmPedido : Form
    {
        #region Fields
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        VendedorBL _objVendedorBL = new VendedorBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        ClienteBL _objClienteBL = new ClienteBL();
        PedidoBL _objPedidoBL = new PedidoBL();
        AlmacenBL _objAlmacenBL = new AlmacenBL();
        VentaBL _objVentaBL = new VentaBL();
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        UltraCombo ucbUnidadMedida = new UltraCombo();
        UltraCombo ucbAlmacen = new UltraCombo();
        List<KeyValueDTO> _ListadoPedidos = new List<KeyValueDTO>();
        List<KeyValueDTO> _ListadoPedidosCambioFecha = new List<KeyValueDTO>();
        List<KeyValueDTO> _ListadoTransportistas = new List<KeyValueDTO>();
        public string strModo = "Nuevo", strIdPedido, _Mode, _pstrIdMovimiento_Nuevo, strIdTransportista, Agregado,NroFactura ="";
        CierreMensualBL _objCierreMensualBL = new CierreMensualBL();
        int _MaxV, _ActV, idLista;
        public decimal SumaValoresVenta = 0;
        public string Zona;
        public decimal SumaValoresVentaR = 0;
        EstablecimientoBL _objEstablecimientoBL = new EstablecimientoBL();
        int Scroll = 0;
        pedidoDto _objPedido = new pedidoDto();
        pedidodetalleDto _objPedidoDetalleDto = new pedidodetalleDto();
        separacionproductoDto _objSeparacionProducto = new separacionproductoDto();
        string Mensaje, MENSAJE = "La lista de Producto(s) tienen la cantidad ingresada superior al saldo del almacen: \n";
        string MensajeDsc = string.Empty, MENSAJEDSC = "La lista de Producto(s) tienen el descuento superior al permitido en SISTEMA: \n";
        string PubligoGen, DireccionGen;
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        AgenciaTransporteBL _objAgenciaTransporteBL = new AgenciaTransporteBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        ListaPreciosBL _objListaPrecioBL = new ListaPreciosBL();
        SecurityBL _obSecurityBL = new SecurityBL();
        string _idPedido, newIdPedido;
        decimal Total, Redondeado, Residuo;
        bool _Redondeado = false,Liberacion=false;
        bool _btnGuardar, _btnImprimir;
        private decimal valorOriginalCelda,  _limiteDocumento;
        //bool EliminarRegistroPedido=false;
        #region Temporales Detalles de Pedido
        List<pedidodetalleDto> _TempDetalle_AgregarDto = new List<pedidodetalleDto>();
        List<pedidodetalleDto> _TempDetalle_ModificarDto = new List<pedidodetalleDto>();
        List<pedidodetalleDto> _TempDetalle_EliminarDto = new List<pedidodetalleDto>();

        #endregion
        #region Temporales Detalle Separacion Producto
        List<separacionproductoDto> _TempDetalle_AgregarDtoSeparacion = new List<separacionproductoDto>();
        List<separacionproductoDto> _TempDetalle_ModificarDtoSeparacion = new List<separacionproductoDto>();
        List<separacionproductoDto> _TempDetalle_EliminarDtoSeparacion = new List<separacionproductoDto>();
        #endregion
        #endregion

        public frmPedido(string Modo, string idPedido,string  sNroFactura="")
        {
            InitializeComponent();
            strModo = Modo;
            strIdPedido = idPedido;
            _idPedido = idPedido;
            groupBox2.Height = strModo != "Cobranza" ? groupBox2.Height + 26 : groupBox2.Height;
            panel3.Location = new Point(panel3.Location.X, groupBox2.Location.Y + groupBox2.Height + 3);
            panel2.Visible = strModo == "Cobranza" ? true : false;
            NroFactura = sNroFactura;
        }

        private void frmPedido_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.Appearance.BackColor = new GlobalFormColors().BannerColor;
            OperationResult objOperationResult = new OperationResult();
            #region ControlAcciones
            var _formActions = _obSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmPedido", Globals.ClientSession.i_RoleId);
            if (_objCierreMensualBL.VerificarMesCerrado(Globals.ClientSession.i_Periodo.ToString().Trim(), DateTime.Now.Month.ToString("00").Trim(), (int)ModulosSistema.VentasFacturacion))
            {
                btnGuardar.Visible = false;
                this.Text = "Pedido/Cotización [MES CERRADO]";
            }
            else
            {

                btnGuardar.Visible = true;
                this.Text = "Pedido/Cotización";
            }
            _btnGuardar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmPedido_Save", _formActions);
            _btnImprimir = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmPedido_Print", _formActions);
            btnGuardar.Enabled = _btnGuardar;
            BtnImprimir.Enabled = _btnImprimir;
            btnEliminar.Enabled = false;
            #endregion
            txtPeriodo.Text = txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            txtMes.Text = DateTime.Now.Month.ToString();
            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
            CargarCombos();
            CargarCombosDetalle();
            ObtenerListadoPedidos(txtPeriodo.Text, txtMes.Text);
            FormatoDecimalesGrilla((int)Globals.ClientSession.i_CantidadDecimales, (int)Globals.ClientSession.i_PrecioDecimales);
            FormatoDecimalesTotales(3);
            if ((_objPedido.i_IdEstado == 1 && _objPedido.i_IdTipoDocumento == (int)TiposDocumentos.Pedido) || (_objPedido.i_IdTipoDocumento == (int)TiposDocumentos.Pedido && !string.IsNullOrEmpty(NroFactura)))
            {
               if (Globals.ClientSession.i_PermiteEditarPedidoFacturado ==0)
               // BotonesSinEdicionPorFactura(false);
                   BotonesSinEdicion(false);
            }
            if (Globals.ClientSession.v_RucEmpresa == Constants.RucCMR && _Mode == "Edit" && Globals.ClientSession.UsuarioEsContable == 0  && (cboVerificacion.Value != null   && (cboVerificacion.Value.ToString() == ((int)TipoVerificacionPedido.Atendido).ToString() || cboVerificacion.Value.ToString() == ((int)TipoVerificacionPedido.Reenviado).ToString())))
            {
                BotonesSinEdicion(false);
            }

            if (_Mode == "New")
            {
                BtnImprimir.Enabled = false;
                cboDocumento.Value = ((int)TiposDocumentos.Pedido).ToString();
                cboDocumento_Leave(sender, e);
            }
            ValidarFechas();
            Scroll = int.Parse(dtpFechaEmision.Value.Month.ToString());
            ConfiguracionGrilla();
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();
            if (Application.OpenForms["frmMaster"] != null) ((frmMaster)Application.OpenForms["frmMaster"]).Activate();

            #region txtRucCLiente
            txtRucCliente.LoadConfig("C");
            txtRucCliente.ItemSelectedAfterDropClosed += delegate
            {
                txtRucCliente_KeyDown(null, new KeyEventArgs(Keys.Enter));
            };
            #endregion
            if (cboDocumento.Value != null && txtSerieDoc.Text.Trim() != string.Empty)
                _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()),
                    txtSerieDoc.Text.Trim());
        }
        private void LimpiarTotales()
        {
            txtValor.Clear();
            txtValorVenta.Clear();
            txtIgv.Clear();
            txtCantidadTotal.Clear();
            txtPrecioVenta.Clear();
        }
        private void CargarCombos()
        {
            OperationResult objOperationResult = new OperationResult();
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboCondicionPago, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 41, null), DropDownListAction.Select);
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosPedidosParaComboGrid(ref objOperationResult);
            Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboTipoOperacion, "Value1", "Id", Globals.CacheCombosVentaDto.cboTipoOperacion, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(ucTipoOperacion, "Value1", "Id", Globals.CacheCombosVentaDto.cboTipoOperacion.Where(r => r.Id != "5").ToList(), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstados, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 43, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", _objVendedorBL.ObtenerListadoVendedorParaCombo(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboVendedorRef, "Value1", "Id", _objVendedorBL.ObtenerListadoVendedorParaCombo(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboIGV, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 27, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", _objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboVerificacion, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 169, null), DropDownListAction.Select);
            cboEstablecimiento.Enabled = false;
            _ListadoTransportistas = _objAgenciaTransporteBL.ObtenerListadoAgenciaTransporteParaCombo(ref objOperationResult, null, null);
            Utils.Windows.LoadUltraComboEditorList(cboAgenciaTransporte, "Value1", "Id", _objAgenciaTransporteBL.ObtenerListadoAgenciaTransporteParaCombo(ref objOperationResult, null, null), DropDownListAction.Select);
            cboAgenciaTransporte.Value = "-1";
            cboVerificacion.Value = "-1";
            lblAccion.Visible = Globals.ClientSession.v_RucEmpresa == Constants.RucCMR;
            cboVerificacion.Visible = Globals.ClientSession.v_RucEmpresa == Constants.RucCMR;
        }
        private void ConfiguracionGrilla()
        {
            var Empresa = new NodeBL().ReporteEmpresa();
            UltraGridColumn Empaque = grdData.DisplayLayout.Bands[0].Columns["Empaque"];
            UltraGridColumn UMEmpaque = grdData.DisplayLayout.Bands[0].Columns["UMEmpaque"];


            UltraGridColumn d_Descuento = grdData.DisplayLayout.Bands[0].Columns["d_Descuento"];
            UltraGridColumn d_ValorVenta = grdData.DisplayLayout.Bands[0].Columns["d_ValorVenta"];
            UltraGridColumn d_Igv = grdData.DisplayLayout.Bands[0].Columns["d_Igv"];
            UltraGridColumn d_Valor = grdData.DisplayLayout.Bands[0].Columns["d_Valor"];
            UltraGridColumn v_Descuento = grdData.DisplayLayout.Bands[0].Columns["v_Descuento"];

            if (Globals.ClientSession.i_VentasMostrarEmpaque.ToString() == "1") //
            {
                Empaque.Hidden = false;
                UMEmpaque.Hidden = false;
            }
            else
            {
                Empaque.Hidden = true;
                UMEmpaque.Hidden = true;
            }
            d_Descuento.Hidden = Globals.ClientSession.i_VisualizarColumnasBasicasPedido == 1 ? true : false;
            d_ValorVenta.Hidden = Globals.ClientSession.i_VisualizarColumnasBasicasPedido == 1 ? true : false;
            d_Igv.Hidden = Globals.ClientSession.i_VisualizarColumnasBasicasPedido == 1 ? true : false;
            d_Valor.Hidden = Globals.ClientSession.i_VisualizarColumnasBasicasPedido == 1 ? true : false;
            v_Descuento.Hidden = Globals.ClientSession.i_VisualizarColumnasBasicasPedido == 1 ? true : false;

            var activarUM = Globals.ClientSession.i_CambiarUnidadMedidaVentaPedido == 1 ? true : false;
            var activarPrecioVenta = Globals.ClientSession.i_EditarPrecioVentaPedido == 1;
            grdData.DisplayLayout.Bands[0].Columns["i_IdUnidadMedida"].CellActivation = activarUM ? Activation.AllowEdit : Activation.ActivateOnly;
            grdData.DisplayLayout.Bands[0].Columns["i_IdUnidadMedida"].CellClickAction = activarUM ? CellClickAction.Edit : CellClickAction.RowSelect;

            grdData.DisplayLayout.Bands[0].Columns["d_PrecioVenta"].CellActivation = activarPrecioVenta ? Activation.AllowEdit : Activation.NoEdit;
            grdData.DisplayLayout.Bands[0].Columns["d_PrecioVenta"].CellClickAction = activarPrecioVenta ? CellClickAction.Edit : CellClickAction.RowSelect;

            cboAgenciaTransporte.Visible = Globals.ClientSession.i_IncluirAgenciaTransportePedido == 1 ? true : false;
            lblAgenciaTransporte.Visible = Globals.ClientSession.i_IncluirAgenciaTransportePedido == 1 ? true : false;

            if (Globals.ClientSession.i_IncluirNingunoCompraVenta == 1 && Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic)
            {
                grdData.DisplayLayout.Bands[0].Columns["v_NroPedido"].Hidden = false;
                grdData.DisplayLayout.Bands[0].Columns["v_NroPedido"].CellActivation = Activation.AllowEdit;
                grdData.DisplayLayout.Bands[0].Columns["v_NroPedido"].CellClickAction = CellClickAction.Edit;
            }
            else
            {
                grdData.DisplayLayout.Bands[0].Columns["v_NroPedido"].Hidden = Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta != 1;

            }
        }

        private void ObtenerListadoPedidos(string pstrPeriodo, string pstrMes)
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoPedidos = _objPedidoBL.ObtenerListadoPedidos(ref objOperationResult, pstrPeriodo, pstrMes);

            switch (strModo)
            {
                case "Edicion":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdPedido);
                    cboDocumento.Enabled = false;
                    btnGenerar.Enabled = false;
                    BtnImprimir.Enabled = _btnImprimir;
                    break;

                case "Cotizacion":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdPedido);
                    cboDocumento.Enabled = true;
                    txtRucCliente.Enabled = true;
                    btnBuscarCliente.Enabled = true;
                    dtpFechaEmision.MinDate = DateTime.Parse((string.Format("{0}/{1}/01", txtPeriodo.Text, txtMes.Text)));
                    dtpFechaEmision.MaxDate = DateTime.Parse((string.Format("{0}/{1}/{2}", txtPeriodo.Text, txtMes.Text.Trim(), DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim())))));
                    cboDocumento.Enabled = false;
                    this.Text = "Pedido";
                    btnGenerar.Enabled = false;
                    
                    break;

                case "Clonacion":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdPedido);
                    cboDocumento.Enabled = true;
                    txtRucCliente.Enabled = true;
                    btnBuscarCliente.Enabled = true;
                    dtpFechaEmision.MinDate = DateTime.Parse((string.Format("{0}/{1}/01", txtPeriodo.Text, txtMes.Text)));
                    dtpFechaEmision.MaxDate = DateTime.Parse((string.Format("{0}/{1}/{2}", txtPeriodo.Text, txtMes.Text.Trim(), DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim())))));
                    cboDocumento.Enabled = false;
                    this.Text = "Pedido";
                    btnGenerar.Enabled = false;
                    cboVendedor.Enabled = Globals.ClientSession.v_RucEmpresa == Constants.RucCMR;
                    break;

                case "Nuevo":
                    if (_ListadoPedidos.Count != 0)
                    {
                        _MaxV = _ListadoPedidos.Count() - 1;
                        _ActV = _MaxV;
                        _objPedido = new pedidoDto();
                        LimpiarCabecera();
                        CargarDetalle("");
                        txtCorrelativo.Text = (int.Parse(_ListadoPedidos[_MaxV].Value1) + 1).ToString("00000000");
                        _Mode = "New";
                        EdicionBarraNavegacion(false);


                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        _objPedido = new pedidoDto();
                        LimpiarCabecera();
                        CargarDetalle("");
                        _MaxV = 1;
                        _ActV = 1;
                        btnNuevoMovimiento.Enabled = false;
                        EdicionBarraNavegacion(false);

                    }
                    cboEstados.Value = "0";
                    txtTipoCambio.Text = _objPedidoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
                    cboDocumento.Enabled = true;
                    cboCondicionPago.Value = Globals.ClientSession.i_IdCondicionPagoPedido != -1 ? Globals.ClientSession.i_IdCondicionPagoPedido.ToString() : "1";
                    break;

                case "Guardado":
                    _MaxV = _ListadoPedidos.Count() - 1;
                    _ActV = _MaxV;
                    CargarCabecera(strIdPedido == "" | strIdPedido == null ? _ListadoPedidos[_MaxV].Value2 : strIdPedido);
                    btnNuevoMovimiento.Enabled = true;
                    cboDocumento.Enabled = false;
                    btnGenerar.Enabled = false;
                    txtMes.Enabled = false;
                    txtCorrelativo.Enabled = false;
                    BtnImprimir.Enabled = _btnImprimir;
                    
                    break;

                case "Consulta":
                    if (_ListadoPedidos.Count != 0)
                    {
                        _MaxV = _ListadoPedidos.Count() - 1;
                        _ActV = _MaxV;
                        txtCorrelativo.Text = (int.Parse(_ListadoPedidos[_MaxV].Value1)).ToString("00000000");
                        CargarCabecera(_ListadoPedidos[_MaxV].Value2);
                        _Mode = "Edit";
                        EdicionBarraNavegacion(true);
                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        LimpiarCabecera();
                        CargarDetalle("");
                        _MaxV = 1;
                        _ActV = 1;
                        _objPedido = new pedidoDto();
                        btnNuevoMovimiento.Enabled = false;
                        _objPedidoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
                        EdicionBarraNavegacion(false);
                        txtMes.Enabled = true;
                    }
                    cboDocumento.Enabled = false;
                    btnGenerar.Enabled = false;
                    BtnImprimir.Enabled = _btnImprimir;
                    
                    break;
                case "Cobranza":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdPedido);
                    cboDocumento.Enabled = false;
                    txtRucCliente.Enabled = false;
                    btnBuscarCliente.Enabled = false;
                    btnGenerar.Enabled = true;
                    BotonesSinEdicion(false);
                    BtnImprimir.Enabled = true;
                    lblNotificacion.Text = _objVentaBL.ObtenerFuturosCorrelativos(_objPedido.v_IdPedido, txtRucCliente.Text.Length == 11 && txtRucCliente.Text != "00000000000");
                    break;
            }
        }

        #region Cabecera-Detalle

        private void ClientePublicoGeneral()
        {
            string[] Cadena = new string[4];
            Cadena = _objPedidoBL.PublicoGeneral();

            if (Cadena != null)
            {
                txtDireccion.Clear();
                _objPedido.v_IdCliente = Cadena[0];
                txtCodigoCliente.Text = Cadena[1];
                txtRazonCliente.Text = Cadena[2];
                txtRucCliente.Text = Cadena[3];
                txtDireccion.Text = Cadena[4];
                txtRazonCliente.Enabled = true;
                txtDireccion.Enabled = true;
                idLista = int.Parse(Cadena[5]);
                _objPedido.i_IdDireccionCliente = int.Parse(Cadena[6]);
                PubligoGen = txtRazonCliente.Text.Trim().Replace(" ", "");
                DireccionGen = txtDireccion.Text.Trim().Replace(" ", "");

            }
            else
            {
                if (UltraMessageBox.Show("El Cliente Público General no está definido en la Base de Datos, ¿Desea Agregarlo?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    _objVentaBL.InsertaClientePublicoGeneralSiEsNecesario();
                    ClientePublicoGeneral();
                }
            }
        }


        private void ClienteAsociadoUsuario(string IdCliente, int IdDireccion)
        {
            OperationResult objOperationResult = new OperationResult();
            string[] Cadena = new string[4];
            Cadena = _objPedidoBL.ObtenerClientePorID(ref objOperationResult, IdCliente, IdDireccion);
            if (Cadena != null)
            {
                txtDireccion.Clear();
                _objPedido.v_IdCliente = Cadena[0];
                txtCodigoCliente.Text = Cadena[1];
                txtRazonCliente.Text = Cadena[2];
                txtRucCliente.Text = Cadena[3];
                txtDireccion.Text = Cadena[4];
                txtRazonCliente.Enabled = true;
                txtDireccion.Enabled = true;
                idLista = int.Parse(Cadena[5]);
                _objPedido.i_IdDireccionCliente = int.Parse(Cadena[6]);
                PubligoGen = txtRazonCliente.Text.Trim().Replace(" ", "");
                DireccionGen = txtDireccion.Text.Trim().Replace(" ", "");
                Zona = Cadena[7];

            }
            else
            {
                if (UltraMessageBox.Show("No Existe datos asociados a este usuario en la Base de Datos, ¿Desea Agregarlo?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }
            }
        }


        private void CargarCabecera(string idPedido)
        {
            OperationResult objOperationResult = new OperationResult();
            _objPedido = new pedidoDto();
            _objPedido = _objPedidoBL.ObtenerPedidoCabecera(ref objOperationResult, idPedido);

            if (_objPedido != null)
            {
                _Mode = "Edit";
                cboVendedor.Value = _objPedido.v_IdVendedor;
                switch (strModo)
                {
                    case "Cotizacion":
                        txtGlosa.Text = string.Format("Referencia de : COT-{0}-{1}", _objPedido.v_SerieDocumento, _objPedido.v_CorrelativoDocumento);
                       
                        _objPedido.i_IdTipoDocumento = (int)TiposDocumentos.Pedido;
                        cboDocumento.Value = _objPedido.i_IdTipoDocumento;
                        //txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString()));
                        //txtNumeroDoc.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000");
                         txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento( cboEstablecimiento.Value !=null && cboEstablecimiento.Value.ToString ()!="-1" ?  int.Parse ( cboEstablecimiento.Value.ToString ())   : Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString()));
                        txtNumeroDoc.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento( cboEstablecimiento.Value !=null && cboEstablecimiento.Value.ToString ()!="-1" ?  int.Parse ( cboEstablecimiento.Value.ToString ()) : Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000");
                        ComprobarExistenciaCorrelativoDocumento();
                        _objPedido.v_SerieDocumento = txtSerieDoc.Text;
                        _objPedido.v_CorrelativoDocumento = txtNumeroDoc.Text;
                        dtpFechaEmision.Value = DateTime.Today;
                        dtpFechaVencimiento.Value = DateTime.Today;
                        txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
                        txtMes.Text = DateTime.Now.Month.ToString("00");
                        if (_ListadoPedidos.Count != 0)
                        {
                            _MaxV = _ListadoPedidos.Count() - 1;
                            _ActV = _MaxV;
                            txtCorrelativo.Text = (int.Parse(_ListadoPedidos[_MaxV].Value1) + 1).ToString("00000000");
                        }
                        else
                        {
                            txtCorrelativo.Text = "00000001";

                        }
                        txtTipoCambio.Text = _objPedidoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
                        _Mode = "New";
                        break;
                    case "Clonacion":
                        txtGlosa.Text = _objPedido.v_Glosa;
                        cboDocumento.Value = _objPedido.i_IdTipoDocumento.Value;
                        //txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString()));
                        //txtNumeroDoc.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000");
                        txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento( cboEstablecimiento.Value !=null && cboEstablecimiento.Value.ToString ()!="-1" ?  int.Parse ( cboEstablecimiento.Value.ToString ()) :Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString()));
                        txtNumeroDoc.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(cboEstablecimiento.Value !=null && cboEstablecimiento.Value.ToString ()!="-1" ?  int.Parse ( cboEstablecimiento.Value.ToString ()):Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000");
                        ComprobarExistenciaCorrelativoDocumento();
                        _objPedido.v_SerieDocumento = txtSerieDoc.Text;
                        _objPedido.v_CorrelativoDocumento = txtNumeroDoc.Text;
                        dtpFechaEmision.Value = DateTime.Today;
                        dtpFechaVencimiento.Value = DateTime.Today;
                        txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
                        txtMes.Text = DateTime.Now.Month.ToString("00");
                        if (_ListadoPedidos.Count != 0)
                        {
                            _MaxV = _ListadoPedidos.Count() - 1;
                            _ActV = _MaxV;
                            txtCorrelativo.Text = (int.Parse(_ListadoPedidos[_MaxV].Value1) + 1).ToString("00000000");
                        }
                        else
                        {
                            txtCorrelativo.Text = "00000001";

                        }
                        txtTipoCambio.Text = _objPedidoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
                        _Mode = "New";
                        break;
                    default:
                        cboDocumento.Value = _objPedido.i_IdTipoDocumento.ToString();
                        txtSerieDoc.Text = _objPedido.v_SerieDocumento;
                        txtNumeroDoc.Text = _objPedido.v_CorrelativoDocumento;
                        dtpFechaEmision.Value = _objPedido.t_FechaEmision.Value;
                        dtpFechaVencimiento.Value = _objPedido.t_FechaVencimiento.Value;
                        txtGlosa.Text = _objPedido.v_Glosa;
                        txtPeriodo.Text = _objPedido.v_Periodo;
                        txtMes.Text = _objPedido.v_Mes;
                        txtCorrelativo.Text = _objPedido.v_Correlativo;
                        txtTipoCambio.Text = _objPedido.d_TipoCambio.ToString();

                        break;
                }

               

                txtDiasVigencia.Text = (_objPedido.i_DiasVigencia).ToString();
                txtRucCliente.Text = _objPedido.RucCliente;
                txtCodigoCliente.Text = _objPedido.CodCliente;

                if (_objPedido.v_IdCliente == "N002-CL000000000")
                {
                    txtRazonCliente.Enabled = true;
                    txtDireccion.Enabled = true;

                    if (_objPedido.v_NombreClienteTemporal != "")
                    {
                        txtRazonCliente.Text = _objPedido.v_NombreClienteTemporal;
                    }
                    else
                    {
                        txtRazonCliente.Text = _objPedido.RazonSocial;
                    }
                    PubligoGen = txtRazonCliente.Text.Trim().Replace(" ", "");
                    txtDireccion.Text = _objPedido.v_DireccionClienteTemporal;
                    DireccionGen = txtDireccion.Text;

                }
                else
                {

                    txtRazonCliente.Enabled = false;
                    txtDireccion.Enabled = false;
                    txtRazonCliente.Text = _objPedido.RazonSocial;
                    txtDireccion.Text = _objPedido.Direccion;
                    PubligoGen = string.Empty;
                    DireccionGen = string.Empty;
                }
                var RelacionesClientesUsuario = new ClienteBL().ObtenerRelacionesClientesUsuario(ref objOperationResult, Globals.ClientSession.i_SystemUserId);
                if (RelacionesClientesUsuario.Any() && Globals.ClientSession.i_SystemUserId.Equals(RelacionesClientesUsuario.FirstOrDefault().i_SystemUser.Value))
                {
                    BloquearControlesCliente();
                }
                cboCondicionPago.Value = _objPedido.i_IdCondicionPago.ToString();
                cboMoneda.Value = _objPedido.i_IdMoneda.ToString();
                cboEstados.Value = _objPedido.i_IdEstado.ToString();
                chkAfectoIGV.Checked = _objPedido.i_AfectoIgv == 1 ? true : false;
                chkPrecInIGV.Checked = _objPedido.i_PrecionInclIgv == 1 ? true : false;
                
                cboVendedorRef.Value = _objPedido.v_IdVendedorRef;
                cboIGV.Value = _objPedido.i_IdIgv.ToString();
                txtPrecioVenta.Text = _objPedido.d_PrecioVenta.ToString();
                txtDescuento.Text = _objPedido.d_Dscto.ToString();
                txtValor.Text = _objPedido.d_Valor.ToString();
                txtValorVenta.Text = _objPedido.d_VVenta.ToString();
                txtCantidadTotal.Text = _objPedido.d_CantidadTotal.ToString();
                txtDscto.Text = _objPedido.d_Descuento.ToString();
                txtIgv.Text = _objPedido.d_Igv.ToString();
                idLista = _objPedido.IdLista.Value;
                dtpFechaDespacho.Value = _objPedido.t_FechaDespacho.Value;
                cboEstablecimiento.Value = _objPedido.i_IdEstablecimiento.Value.ToString();
                cboAgenciaTransporte.Value = _objPedido.v_IdAgenciaTransporte;
                cboTipoOperacion.Value = _objPedido.i_IdTipoOperacion.ToString();
                if (Globals.ClientSession.v_RucEmpresa == Constants.RucCMR)
                {
                    btnGuardar.Enabled = Globals.ClientSession.UsuarioEsContable ==0? _objPedido.i_IdTipoVerificacion == (int)TipoVerificacionPedido.Atendido ? false : _btnGuardar :_btnGuardar;
                }
                cboVerificacion.Value = _objPedido.i_IdTipoVerificacion.ToString();
                cboVerificacion.Enabled  = Globals.ClientSession.UsuarioEsContable == 1;
                CargarDetalle(_objPedido.v_IdPedido);

            }

            else
            {
                UltraMessageBox.Show("Hubo un error al cargar la pedido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


        }

        private void CargarDetalle(string pstridPedido)
        {
            OperationResult objOperationResult = new OperationResult();
            grdData.DataSource = _objPedidoBL.ObtenerPedidoDetalles(ref objOperationResult, pstridPedido);
            if (grdData.Rows.Any())
            {
                for (int i = 0; i < grdData.Rows.Count(); i++)
                {
                    if (grdData.Rows[i].Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000") // Cuando los servicios se jalan de Tabla producto y no de Tabla ProductoAlmacen
                    {
                        if (grdData.Rows[i].Cells["i_EsServicio"].Value.ToString() == "1" && grdData.Rows[i].Cells["v_IdProductoAlmacen"].Value == null)
                        {
                            grdData.Rows[i].Cells["d_DescuentoLP"].Value = "0";

                        }
                        else // antes de 12 febrero
                        {
                            var ListaPrecio = grdData.Rows[i].Cells["v_IdProductoDetalle"].Value != null ? _objListaPrecioBL.ObtenerListaPrecioDetalle(ref objOperationResult, int.Parse(grdData.Rows[i].Cells["i_IdAlmacen"].Value.ToString()), idLista, grdData.Rows[i].Cells["v_IdProductoDetalle"].Value.ToString()) : null;
                            if (ListaPrecio != null)
                            {
                               
                                grdData.Rows[i].Cells["d_DescuentoLP"].Value = ListaPrecio.d_Descuento.Value == null ? 0 : ListaPrecio.d_Descuento.Value;
                            }
                            else
                            {
                                grdData.Rows[i].Cells["d_DescuentoLP"].Value = "0";
                            }
                        }
                    }
                    grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
                    if (strModo == "Cotizacion" || strModo == "Clonacion")
                    {
                        grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
                    }
                }
                
                CalcularTotales();
            }
            if (Globals.ClientSession.i_CambiarAlmacenVentasDesdeVendedor == 1)
            {
                //cboVendedor.Enabled =  Globals.ClientSession.UsuarioEsContable ==1 ? true : !grdData.Rows.Any();
                cboVendedor.Enabled =!grdData.Rows.Any();
            }
        }

        private void FormatoDecimalesTotales(int DecimalesCantidad)
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

            txtDescuento.Text = txtDescuento.Text == string.Empty ? decimal.Parse("0").ToString(FormatoCantidad) : decimal.Parse(txtDescuento.Text).ToString(FormatoCantidad);
        }

        private void LimpiarCabecera()
        {
            cboDocumento.Value = "-1";
            txtSerieDoc.Clear();
            txtNumeroDoc.Clear();
            dtpFechaEmision.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            txtTipoCambio.Text = "";
            txtDiasVigencia.Text = "0";
            dtpFechaVencimiento.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
            txtRucCliente.Clear();
            txtCodigoCliente.Clear();
            txtRazonCliente.Clear();
            cboCondicionPago.Value = "-1";
            cboVendedor.Value = "-1";
            cboVendedorRef.Value = "-1";
            cboEstados.Value = "-1";
            txtDescuento.Text = "0";
            //txtGlosa.Clear();
            txtGlosa.Text = !string.IsNullOrEmpty(Globals.ClientSession.v_GlosaPedido) ? Globals.ClientSession.v_GlosaPedido : "";
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1 ? true : false;
            chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvVentas == 1 ? true : false;
            cboIGV.Value = Globals.ClientSession.i_IdIgv.ToString();
            txtValor.Clear();
            txtValorVenta.Clear();
            txtIgv.Clear();
            txtCantidadTotal.Clear();
            txtPrecioVenta.Clear();
            OperationResult objOperationResult = new OperationResult();
            var RelacionesClientesUsuario = new ClienteBL().ObtenerRelacionesClientesUsuario(ref objOperationResult, Globals.ClientSession.i_SystemUserId);
            if (RelacionesClientesUsuario.Any() && Globals.ClientSession.i_SystemUserId.Equals(RelacionesClientesUsuario.FirstOrDefault().i_SystemUser.Value))
            {
                ClienteAsociadoUsuario(RelacionesClientesUsuario.FirstOrDefault().v_IdCliente, RelacionesClientesUsuario.FirstOrDefault().i_IdDireccionCliente.Value);
                BloquearControlesCliente();
            }
            else
            {
                ClientePublicoGeneral();
            }
            cboVendedor.Value = Globals.ClientSession.v_IdVendedor;
            cboVendedor.Value = cboVendedor.Value == null ? "-1" : cboVendedor.Value.ToString();

            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.Value.ToString();
            cboTipoOperacion.Value = Globals.ClientSession.i_IdTipoOperacionVentas.ToString();
            
            cboVerificacion.Value = Globals.ClientSession.v_RucEmpresa ==Constants.RucCMR ?  Globals.ClientSession.UsuarioEsContable  ==1?((int)TipoVerificacionPedido.Atendido).ToString ():  ((int)TipoVerificacionPedido.PorAtender).ToString () :"-1";

            cboVerificacion.Enabled = Globals.ClientSession.UsuarioEsContable == 1;
        }

        private void BloquearControlesCliente()
        {
            txtRucCliente.Enabled = false;
            btnBuscarCliente.Enabled = false;
            txtRazonCliente.Enabled = false;
            txtDireccion.Enabled = false;
            btnBuscarDirecciones.Enabled = false;
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
                                _objPedidoDetalleDto = new pedidodetalleDto();
                                _objPedidoDetalleDto.v_IdPedido = _objPedido.v_IdPedido;
                                _objPedidoDetalleDto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _objPedidoDetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _objPedidoDetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _objPedidoDetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _objPedidoDetalleDto.d_PrecioUnitario = Fila.Cells["d_PrecioUnitario"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioUnitario"].Value.ToString());
                                _objPedidoDetalleDto.d_Valor = Fila.Cells["d_Valor"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Valor"].Value.ToString());
                                _objPedidoDetalleDto.d_Descuento = Fila.Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Descuento"].Value.ToString());
                                _objPedidoDetalleDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                                _objPedidoDetalleDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());
                                _objPedidoDetalleDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                                _objPedidoDetalleDto.i_NroUnidades = Fila.Cells["i_NroUnidades"].Value == null ? 0 : int.Parse(Fila.Cells["i_NroUnidades"].Value.ToString());
                                _objPedidoDetalleDto.v_Observacion = Fila.Cells["v_Observacion"].Value == null ? null : Fila.Cells["v_Observacion"].Value.ToString();
                                _objPedidoDetalleDto.v_NombreProducto = Fila.Cells["v_NombreProducto"].Value == null ? null : Fila.Cells["v_NombreProducto"].Value.ToString();
                                _objPedidoDetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _objPedidoDetalleDto.v_Descuento = Fila.Cells["v_Descuento"].Value == null ? "" : Fila.Cells["v_Descuento"].Value.ToString();
                                _objPedidoDetalleDto.i_IdTipoOperacion = Fila.Cells["i_IdTipoOperacion"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoOperacion"].Value.ToString());
                                _objPedidoDetalleDto.t_FechaLiberacion = Fila.Cells["t_FechaLiberacion"].Value == null ? (DateTime?)null :DateTime.Parse ( Fila.Cells["t_FechaLiberacion"].Value.ToString());
                                _objPedidoDetalleDto.i_LiberacionUsuario = Fila.Cells["i_LiberacionUsuario"].Value == null ? (int?)null : int.Parse(Fila.Cells["i_LiberacionUsuario"].Value.ToString());

                                //_objPedidoDetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                _objPedidoDetalleDto.v_NroSerie = Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim();
                                _objPedidoDetalleDto.v_NroLote = Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim();

                                _objPedidoDetalleDto.t_FechaCaducidad = Fila.Cells["t_FechaCaducidad"].Value == null ? (DateTime?)null : DateTime.Parse(Fila.Cells["t_FechaCaducidad"].Value.ToString());
                                if (int.Parse(cboDocumento.Value.ToString()) == (int)TiposDocumentos.Pedido)
                                {
                                    if (_objPedidoDetalleDto.v_IdProductoDetalle != "N002-PE000000000" & Fila.Cells["i_EsServicio"].Value.ToString() == "0")
                                    {
                                        _objSeparacionProducto = new separacionproductoDto();
                                        _objSeparacionProducto.v_IdPedido = _objPedido.v_IdPedido;
                                        _objSeparacionProducto.v_IdProductoAlmacen = Fila.Cells["v_IdProductoAlmacen"].Value == null ? null : Fila.Cells["v_IdProductoAlmacen"].Value.ToString();
                                        _objSeparacionProducto.d_Separacion_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                        _objSeparacionProducto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                        _objSeparacionProducto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                        _objSeparacionProducto.d_Separacion_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                        _objSeparacionProducto.v_IdProductoAlmacen = Fila.Cells["v_IdProductoAlmacen"].Value == null ? null : Fila.Cells["v_IdProductoAlmacen"].Value.ToString();

                                        _objSeparacionProducto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                        _objSeparacionProducto.v_NroSerie = Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim();
                                        _objSeparacionProducto.v_NroLote = Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim();
                                        _TempDetalle_AgregarDtoSeparacion.Add(_objSeparacionProducto);
                                    }
                                }
                                _TempDetalle_AgregarDto.Add(_objPedidoDetalleDto);

                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _objPedidoDetalleDto = new pedidodetalleDto();
                                _objPedidoDetalleDto.v_IdPedido = Fila.Cells["v_IdPedido"].Value == null ? null : Fila.Cells["v_IdPedido"].Value.ToString();
                                _objPedidoDetalleDto.v_IdPedidoDetalle = Fila.Cells["v_IdPedidoDetalle"].Value == null ? null : Fila.Cells["v_IdPedidoDetalle"].Value.ToString();
                                _objPedidoDetalleDto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _objPedidoDetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _objPedidoDetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _objPedidoDetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _objPedidoDetalleDto.d_PrecioUnitario = Fila.Cells["d_PrecioUnitario"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioUnitario"].Value.ToString());
                                _objPedidoDetalleDto.d_Valor = Fila.Cells["d_Valor"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Valor"].Value.ToString());
                                _objPedidoDetalleDto.d_Descuento = Fila.Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Descuento"].Value.ToString());
                                _objPedidoDetalleDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                                _objPedidoDetalleDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());
                                _objPedidoDetalleDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                                _objPedidoDetalleDto.i_NroUnidades = Fila.Cells["i_NroUnidades"].Value == null ? 0 : int.Parse(Fila.Cells["i_NroUnidades"].Value.ToString());
                                _objPedidoDetalleDto.v_Observacion = Fila.Cells["v_Observacion"].Value == null ? null : Fila.Cells["v_Observacion"].Value.ToString();
                                _objPedidoDetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _objPedidoDetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _objPedidoDetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _objPedidoDetalleDto.v_NombreProducto = Fila.Cells["v_NombreProducto"].Value == null ? null : Fila.Cells["v_NombreProducto"].Value.ToString();
                                _objPedidoDetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _objPedidoDetalleDto.v_Descuento = Fila.Cells["v_Descuento"].Value == null ? "" : Fila.Cells["v_Descuento"].Value.ToString();
                                _objPedidoDetalleDto.i_IdTipoOperacion = Fila.Cells["i_IdTipoOperacion"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoOperacion"].Value.ToString());
                                _objPedidoDetalleDto.t_FechaLiberacion = Fila.Cells["t_FechaLiberacion"].Value == null ? (DateTime?)null : DateTime.Parse(Fila.Cells["t_FechaLiberacion"].Value.ToString());
                                _objPedidoDetalleDto.i_LiberacionUsuario = Fila.Cells["i_LiberacionUsuario"].Value == null ? (int?)null : int.Parse(Fila.Cells["i_LiberacionUsuario"].Value.ToString());


                               // _objPedidoDetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                _objPedidoDetalleDto.v_NroSerie  = Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim();
                                _objPedidoDetalleDto.v_NroLote  = Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim();
                                _objPedidoDetalleDto.t_FechaCaducidad = Fila.Cells["t_FechaCaducidad"].Value == null ? (DateTime ?)null : DateTime.Parse(Fila.Cells["t_FechaCaducidad"].Value.ToString());
                                
                                if (int.Parse(cboDocumento.Value.ToString()) == (int)TiposDocumentos.Pedido)
                                {
                                    if (_objPedidoDetalleDto.v_IdProductoDetalle != "N002-PE000000000" & Fila.Cells["i_EsServicio"].Value.ToString() == "0")
                                    {
                                        _objSeparacionProducto = new separacionproductoDto();
                                        _objSeparacionProducto.v_IdPedido = Fila.Cells["v_IdPedido"].Value == null ? null : Fila.Cells["v_IdPedido"].Value.ToString();
                                        _objSeparacionProducto.v_IdProductoAlmacen = Fila.Cells["v_IdProductoAlmacen"].Value == null ? null : Fila.Cells["v_IdProductoAlmacen"].Value.ToString();
                                        _objSeparacionProducto.d_Separacion_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                        _objSeparacionProducto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                        _objSeparacionProducto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                        _objSeparacionProducto.d_Separacion_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                        _objSeparacionProducto.v_IdProductoAlmacen = Fila.Cells["v_IdProductoAlmacen"].Value == null ? null : Fila.Cells["v_IdProductoAlmacen"].Value.ToString();
                                        _objSeparacionProducto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                        _objSeparacionProducto.v_NroSerie = Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim();
                                        _objSeparacionProducto.v_NroLote = Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim();
                                        
                                        _TempDetalle_ModificarDtoSeparacion.Add(_objSeparacionProducto);
                                    }
                                }

                                _TempDetalle_ModificarDto.Add(_objPedidoDetalleDto);
                            }
                            break;
                    }
                }
            }

        }

        private void ComprobarExistenciaDocumento()
        {
            OperationResult objOperationResult = new OperationResult();
            if (_objPedidoBL.ComprobarExistenciaDocumento(ref objOperationResult, int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text, txtNumeroDoc.Text, _objPedido.v_IdPedido) == true)
            {
                UltraMessageBox.Show("El documento ya ha sido registrado anteriormente", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGuardar.Enabled = false;
            }
            else
            {
                //btnGuardar.Enabled = true;
                btnGuardar.Enabled = _btnGuardar;
            }
        }

        #endregion

        # region BarraNavegacion
        private void EdicionBarraNavegacion(bool ON_OFF)
        {
            txtCorrelativo.Enabled = ON_OFF;
            btnAnterior.Enabled = ON_OFF;
            btnSiguiente.Enabled = ON_OFF;
            txtMes.Enabled = ON_OFF;
            btnNuevoMovimiento.Enabled = ON_OFF;
        }


        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_ListadoPedidos.Count() > 0)
            {
                if (_MaxV == 0) CargarCabecera(_ListadoPedidos[0].Value2);

                if (_ActV > 0 && _ActV <= _MaxV)
                {
                    _ActV = _ActV - 1;
                    txtCorrelativo.Text = _ListadoPedidos[_ActV].Value1;
                    CargarCabecera(_ListadoPedidos[_ActV].Value2);
                }
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_ActV >= 0 && _ActV < _MaxV)
            {
                _ActV = _ActV + 1;
                txtCorrelativo.Text = _ListadoPedidos[_ActV].Value1;
                CargarCabecera(_ListadoPedidos[_ActV].Value2);
            }
        }

        private void btnNuevoMovimiento_Click(object sender, EventArgs e)
        {
            strModo = "Nuevo";
            _objPedido = new pedidoDto();
            OperationResult objOperationResult = new OperationResult();
            LimpiarCabecera();
            CargarDetalle("");
            txtCorrelativo.Text = (int.Parse(_ListadoPedidos[_MaxV].Value1) + 1).ToString("00000000");
            _Mode = "New";
            EdicionBarraNavegacion(false);
            cboEstados.Value = "0";
            txtTipoCambio.Text = _objPedidoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
            cboDocumento.Value = ((int)TiposDocumentos.Pedido).ToString();
            cboDocumento_Leave(sender, e);
            // cboCondicionPago.Value = "1";
            cboCondicionPago.Value = Globals.ClientSession.i_IdCondicionPagoPedido != -1 ? Globals.ClientSession.i_IdCondicionPagoPedido.ToString() : "1";
            txtRucCliente.Focus();
            cboDocumento.Enabled = true;
            cboMoneda.Enabled = true;
            cboCondicionPago.Enabled = true;

        }

        #endregion

        #region Grilla

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].EditorComponent = ucbUnidadMedida;
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].EditorComponent = ucbAlmacen;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            if (Globals.ClientSession.i_PermiteIntercambiarListasPrecios == 0)
            {
                e.Layout.Bands[0].Columns["d_PrecioUnitario"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
            }
        }
        private void FormatoDecimalesGrilla(int DecimalesCantidad, int DecimalesPrecio)
        {
            string FormatoCantidad, FormatoPrecio;
            UltraGridColumn _Cantidad = this.grdData.DisplayLayout.Bands[0].Columns["d_Cantidad"];
            _Cantidad.MaskDataMode = MaskMode.IncludeLiterals;
            _Cantidad.MaskDisplayMode = MaskMode.IncludeLiterals;

            UltraGridColumn _Precio = this.grdData.DisplayLayout.Bands[0].Columns["d_PrecioUnitario"];
            _Precio.MaskDataMode = MaskMode.IncludeLiterals;
            _Precio.MaskDisplayMode = MaskMode.IncludeLiterals;

            //UltraGridColumn _Descuento = this.grdData.DisplayLayout.Bands[0].Columns["d_Descuento"];
            //_Descuento.MaskDataMode = MaskMode.IncludeLiterals;
            //_Descuento.MaskDisplayMode = MaskMode.IncludeLiterals;


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
            //   _Descuento.MaskInput = FormatoPrecio;
            _Precio.MaskInput = "-" + FormatoPrecio;
        }
        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            int Index = grdData != null && grdData.ActiveRow != null ? grdData.ActiveRow.Index : 1;
            bool NoTieneStock = false;
            int idListaS = idLista;
            int idAlmacen;
            OperationResult objOperationResult = new OperationResult();


            if (Globals.ClientSession.v_RucEmpresa == Constants.RucCMR || grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "Temporal")
            {

                if (e.Cell.Column.Key == "v_CodigoInterno")
                {
                    idAlmacen = Convert.ToInt32(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdAlmacen"].Value);
                    if (idAlmacen == -1)
                    {
                        UltraMessageBox.Show("Porfavor seleccione un almacén antes de buscar un producto ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    else
                    {
                        if (_objPedidoBL.EsValidoCodProducto(e.Cell.Text))
                        {
                            productoshortDto prod = Globals.ClientSession.i_UsaListaPrecios == 1 ?
                                _objPedidoBL.DevolverArticuloPorCodInternoNuevo(ref objOperationResult, null, null, int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()), _objPedido.v_IdCliente, e.Cell.Text.Trim().ToUpper()).FirstOrDefault()
                            : _objPedidoBL.DevolverArticuloPorCodInternoNuevo(ref objOperationResult, int.Parse(grdData.ActiveRow.GetCellValue("i_IdAlmacen").ToString()), e.Cell.Text.Trim().ToUpper()).FirstOrDefault();
                            if (prod != null)
                            {


                                foreach (UltraGridRow Fila in grdData.Rows)
                                {
                                    if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                                    {

                                        if (Index != Fila.Index && prod.v_IdProductoDetalle == Fila.Cells["v_IdProductoDetalle"].Value.ToString() && idAlmacen == int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()))
                                        {
                                            UltraMessageBox.Show("El producto ya existe para este almacén ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return;
                                        }
                                    }
                                }

                                 

                                grdData.ActiveRow.Cells["v_NombreProducto"].Value = prod.v_Descripcion.Trim();
                                grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = prod.v_IdProductoDetalle;
                                grdData.ActiveRow.Cells["v_CodigoInterno"].Value = prod.v_CodInterno;
                                grdData.ActiveRow.Cells["Empaque"].Value = prod.d_Empaque;
                                grdData.ActiveRow.Cells["UMEmpaque"].Value = prod.EmpaqueUnidadMedida;
                                grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                                grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = prod.i_IdUnidadMedida;
                                grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = prod.i_IdUnidadMedida;
                                grdData.ActiveRow.Cells["i_EsAfectoDetraccion"].Value = prod.i_EsAfectoDetraccion;
                                grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = (string)cboTipoOperacion.Value != "5" ? cboTipoOperacion.Value.ToString() : "1";
                                grdData.ActiveRow.Cells["d_PrecioUnitario"].Value = DevolverPrecioProducto(prod.IdMoneda, prod.d_Precio ?? 0, int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);
                                grdData.ActiveRow.Cells["EsNombreEditable"].Value = prod.i_NombreEditable;
                                grdData.ActiveRow.Cells["i_EsServicio"].Value = prod.i_EsServicio;
                                grdData.ActiveRow.Cells["i_ValidarStock"].Value = prod.i_ValidarStock ?? 0;
                                grdData.ActiveRow.Cells["v_IdProductoAlmacen"].Value = prod.v_IdProductoAlmacen;
                                grdData.ActiveRow.Cells["d_StockActual"].Value = prod.stockActual;
                                grdData.ActiveRow.Cells["d_SeparacionTotal"].Value = prod.d_separacion;
                                grdData.ActiveRow.Cells["d_DescuentoLP"].Value = prod.d_Descuento;
                                grdData.ActiveRow.Cells["d_Cantidad"].Value = "1";
                                grdData.ActiveRow.Cells["i_PrecioEditable"].Value = prod.i_PrecioEditable ?? 0;
                                grdData.ActiveRow.Cells["i_SolicitarNroSerieSalida"].Value = prod.i_SolicitarNroSerieSalida;
                                grdData.ActiveRow.Cells["i_SolicitarNroLoteSalida"].Value = prod.i_SolicitarNroLoteSalida;

                            }
                            else
                            {
                                UltraMessageBox.Show("El Artículo existe Pero no tuvo ingresos a este almacén", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                btnEliminar_Click(sender, e);
                                NoTieneStock = true;
                            }
                        }
                        else
                        {




                            Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(idAlmacen, "PedidoVenta", _objPedido.v_IdCliente, grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Text == null ? string.Empty : grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Text);
                            frm.ShowDialog();

                            if (frm._NombreProducto != null)
                            {
                                if (int.Parse(cboDocumento.Value.ToString()) == (int)TiposDocumentos.Pedido)
                                {
                                    int ValidarStockAlmacen = _objAlmacenBL.ObtenerAlmacen(ref objOperationResult, int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString())).i_ValidarStockAlmacen ?? 0;
                                    if (frm._ValidarStock == 1 && ValidarStockAlmacen == 1)
                                    {
                                        if (frm._stockActual - frm._SeparaccionTotal <= 0 && frm._EsServicio == 0)
                                        {

                                            UltraMessageBox.Show("El producto tiene un Saldo de Cero Unidades en éste Almacén ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return;

                                        }
                                    }
                                }

                                foreach (UltraGridRow Fila in grdData.Rows)
                                {
                                    if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                                    {

                                        if (frm._IdProducto == Fila.Cells["v_IdProductoDetalle"].Value.ToString() && idAlmacen == int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()))
                                        {
                                            UltraMessageBox.Show("El producto ya existe para este almacén ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return;
                                        }
                                    }
                                }

                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreProducto"].Value = frm._NombreProducto.Trim();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = frm._IdProducto;
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value = frm._CodigoInternoProducto;
                                grdData.Rows[grdData.ActiveRow.Index].Cells["Empaque"].Value = frm._Empaque.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = frm._UnidadMedida ?? string.Empty;
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = frm._UnidadMedidaEmpaque ?? string.Empty;  //Por defecto ,pero si desea el usuario lo puede cambiar
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = frm._UnidadMedidaEmpaque ?? string.Empty; ;
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value = frm._IdProductoAlmacen ?? string.Empty;
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_SeparacionTotal"].Value = frm._SeparaccionTotal.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_StockActual"].Value = frm._stockActual.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsAfectoDetraccion"].Value = frm._EsAfectoDetraccion;
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value = frm._EsServicio;
                                grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = (string)cboTipoOperacion.Value != "5" ? cboTipoOperacion.Value.ToString() : "1";
                                //grdData.Rows[grdData.ActiveRow.Index].Cells["d_PrecioUnitario"].Value = frm._PrecioUnitario;
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_PrecioUnitario"].Value = DevolverPrecioProducto(frm._IdMoneda, frm._PrecioUnitario, int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);

                                grdData.Rows[grdData.ActiveRow.Index].Cells["EsNombreEditable"].Value = frm._NombreEditable;
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_PrecioEditable"].Value = frm._PrecioEditable == null ? 0 : frm._PrecioEditable;
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_DescuentoLP"].Value = frm._Descuento;
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value = frm._ValidarStock == null ? null : frm._ValidarStock.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = "1";
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                            }
                        }
                        if (grdData.Rows.Any() && !NoTieneStock)
                        {

                            UltraGridCell aCell = grdData.Rows[e.Cell.Row.Index].Cells["d_Cantidad"];
                            grdData.Rows[e.Cell.Row.Index].Activate();
                            grdData.ActiveCell = aCell;
                            grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                            grdData.Focus();
                        }

                    }
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

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            decimal Saldo = 0;
            OperationResult objOperationResult = new OperationResult();

            if (grdData.ActiveCell.Column.Key == "d_Cantidad" || grdData.ActiveCell.Column.Key == "d_PrecioUnitario")
            {
                CalcularValoresFila(grdData.Rows[grdData.ActiveRow.Index]);

            }

            if (cboDocumento.Value == null || grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value == null || grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value == null) return;

            if (int.Parse(cboDocumento.Value.ToString()) == (int)TiposDocumentos.Pedido)
            {

                int ValidarStockAlmacen = _objAlmacenBL.ObtenerAlmacen(ref objOperationResult, int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdAlmacen"].Value.ToString())).i_ValidarStockAlmacen ?? 0;
                switch (this.grdData.ActiveCell.Column.Key)
                {
                    case "d_Cantidad":

                        if (strModo == "Guardado" || strModo == "Edicion")
                        {

                            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "Temporal" & grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value.ToString() == "0") // Es como si fuera Nuevo
                            {
                                decimal StockActual = grdData.Rows[grdData.ActiveRow.Index].Cells["d_StockActual"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_StockActual"].Value.ToString());
                                decimal SeparacionTotal = grdData.Rows[grdData.ActiveRow.Index].Cells["d_SeparacionTotal"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_SeparacionTotal"].Value.ToString());
                                decimal SeparacionActual = grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value.ToString());
                                Saldo = StockActual - (SeparacionTotal + SeparacionActual);
                                if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value.ToString() == "1" && ValidarStockAlmacen == 1)
                                {
                                    if (Saldo < 0)
                                    {
                                        UltraMessageBox.Show("Cantidad Ingresada supera el saldo en Almacén", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        return;

                                    }
                                }

                            }
                            else  // Editado
                            {
                                if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value.ToString() == "0")
                                {
                                    decimal StockActual = grdData.Rows[grdData.ActiveRow.Index].Cells["d_StockActual"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_StockActual"].Value.ToString());
                                    decimal SeparacionAnterior = _objPedidoBL.ObtenerCantidadSeparacionProducto(ref objOperationResult, grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdPedido"].Value.ToString(), grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value.ToString());
                                    decimal SeparacionTotal = grdData.Rows[grdData.ActiveRow.Index].Cells["d_SeparacionTotal"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_SeparacionTotal"].Value.ToString());
                                    decimal SeparacionActual = grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value.ToString());
                                    Saldo = StockActual - ((SeparacionTotal - SeparacionAnterior) + SeparacionActual);
                                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value.ToString() == "0")
                                    {
                                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value.ToString() == "1" && ValidarStockAlmacen == 1)
                                        {
                                            if (Saldo < 0)
                                            {
                                                UltraMessageBox.Show("Cantidad Ingresada supera el saldo en Almacén", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                return;
                                            }
                                        }
                                    }
                                }

                            }

                        }
                        else
                        {
                            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value == null) return;
                            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value.ToString() == "0")
                            {
                                decimal StockActual = grdData.Rows[grdData.ActiveRow.Index].Cells["d_StockActual"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_StockActual"].Value.ToString());
                                decimal SeparacionTotal = grdData.Rows[grdData.ActiveRow.Index].Cells["d_SeparacionTotal"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_SeparacionTotal"].Value.ToString());
                                decimal SeparacionActual = grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value.ToString());
                                Saldo = StockActual - (SeparacionTotal + SeparacionActual);

                                if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value.ToString() == "0")
                                {
                                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value.ToString() == "1" && ValidarStockAlmacen == 1)
                                    {
                                        if (Saldo < 0)
                                        {
                                            UltraMessageBox.Show("Cantidad Ingresada supera el saldo en Almacén", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return;

                                        }
                                    }
                                }
                            }
                        }
                        break;


                }

            }
            switch (this.grdData.ActiveCell.Column.Key)
            {
                case "v_CodigoInterno":
                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value != null && grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value.ToString().Trim() == "")
                    {
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = null;
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreProducto"].Value = string.Empty;
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = "-1";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = "0";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_PrecioUnitario"].Value = "0";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = "-1";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Descuento"].Value = "0";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Valor"].Value = "0";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_ValorVenta"].Value = "0";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_PrecioVenta"].Value = "0";
                    }
                    break;

                case "v_Descuento":
                    var Empresa = new NodeBL().ReporteEmpresa();

                    //Empresa.FirstOrDefault().RucEmpresaPropietaria = Constants.RucChayna;
                    if (Empresa != null && Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim() == Constants.RucChayna)
                    {
                        decimal descuento1 = grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descuento"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["v_Descuento"].Text);
                        decimal descuentolp1 = grdData.Rows[grdData.ActiveRow.Index].Cells["d_DescuentoLP"].Value == null ? 0 : decimal.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["d_DescuentoLP"].Value.ToString());
                        if (descuento1 > descuentolp1)
                        {
                            UltraMessageBox.Show("El descuento ingresado supera al descuento permitido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        else
                        {
                            CalcularValoresFila(grdData.ActiveRow);
                        }

                    }
                    else
                    {

                        if (grdData.ActiveCell.Value != null)
                        {
                            grdData.ActiveCell.Value = Utils.Windows.DarFormatoDescuentoUnDecimal(grdData.ActiveCell.Value.ToString());
                            CalcularValoresFila(grdData.ActiveRow);
                        }
                        else
                            grdData.ActiveCell.Value = "0";
                    }

                    break;

                case "d_PrecioVenta":
                    try
                    {
                        if (Globals.ClientSession.i_EditarPrecioVentaPedido == 1)
                        {
                            #region Validaciones
                            if (grdData.ActiveCell.Value == null) grdData.ActiveCell.SetValue(0, false);
                            decimal v1;
                            decimal.TryParse(grdData.ActiveCell.Value.ToString(), out v1);
                            v1 = Utils.Windows.DevuelveValorRedondeado(v1, 2);
                            var v2 = Utils.Windows.DevuelveValorRedondeado(valorOriginalCelda, 2);
                            if (v1.Equals(v2)) return;
                            #endregion

                            #region verificar si tiene dcto
                            int dcto;
                            var celdaDscto = grdData.ActiveCell.Row.Cells["v_Descuento"];
                            var tieneDscto = celdaDscto.Value != null && !string.IsNullOrWhiteSpace(celdaDscto.Value.ToString()) &&
                                                ((int.TryParse(celdaDscto.Value.ToString(), out dcto) &&
                                                dcto != 0) || celdaDscto.Value.ToString().Contains("+"));
                            if (tieneDscto)
                            {
                                grdData.ActiveCell.Value = grdData.ActiveCell.OriginalValue;
                                return;
                            }
                            #endregion

                            decimal precioVenta;
                            Func<decimal, decimal, bool> validar = (n1, n2) => n1 != 0 && n2 != 0;
                            var cantidad = decimal.Parse(grdData.ActiveCell.Row.Cells["d_Cantidad"].Value.ToString());
                            var celdaPrecio = grdData.ActiveCell.Row.Cells["d_PrecioUnitario"];

                            if (grdData.ActiveCell.Value == null || !decimal.TryParse(grdData.ActiveCell.Value.ToString(), out precioVenta)) return;
                            if (!validar(cantidad, precioVenta)) return;
                            decimal precio;
                            if (chkAfectoIGV.Checked)
                            {
                                if (chkPrecInIGV.Checked)
                                    precio = precioVenta / cantidad;
                                else
                                    precio = (precioVenta / cantidad) / (1 + (decimal.Parse(cboIGV.Text) / 100));
                            }
                            else
                                precio = precioVenta / cantidad;

                            celdaPrecio.SetValue(Utils.Windows.DevuelveValorRedondeado(precio, 4), true);
                            CalcularValoresFila(grdData.ActiveRow);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    break;
            }


        }

        private void CalcularValoresFila(UltraGridRow fila)
        {
            if (cboIGV.Value.ToString() == "-1")
            {
                UltraMessageBox.Show("Porfavor seleccione el IGV", "Sistema", Icono: MessageBoxIcon.Information);
                return;
            }
            var detail = (BindingPedidoDetalle)fila.ListObject;
            if (detail.d_Cantidad == null) detail.d_Cantidad = 0;
            if (detail.d_PrecioUnitario == null) detail.d_PrecioUnitario = 0;
            if (string.IsNullOrWhiteSpace(detail.v_Descuento)) detail.v_Descuento = "0";

            var descuentos = detail.v_Descuento;

            var price = detail.d_PrecioUnitario.Value;
            var porcIgv = decimal.Parse(cboIGV.Text) / 100;
            var esGravado = detail.i_IdTipoOperacion == 1 || (detail.i_IdTipoOperacion > 10 && detail.i_IdTipoOperacion < 20);
            if (esGravado && chkPrecInIGV.Checked) price /= 1 + porcIgv;

            detail.d_Valor = Utils.Windows.DevuelveValorRedondeado(detail.d_Cantidad.Value * price, 2);
            detail.d_Descuento = Utils.Windows.CalcularDescuentosSucesivos(descuentos, detail.d_Valor.Value);

            if (detail.EsRedondeo == 1) porcIgv = 0; // Quita el IGV al redondeo
            if (esGravado && chkPrecInIGV.Checked)
            {
                detail.d_PrecioVenta = detail.d_PrecioUnitario * detail.d_Cantidad;
                detail.d_PrecioVenta -= Utils.Windows.CalcularDescuentosSucesivos(descuentos, detail.d_PrecioVenta ?? 0);
                detail.d_PrecioVenta = Utils.Windows.DevuelveValorRedondeado(detail.d_PrecioVenta ?? 0, 2);
                detail.d_ValorVenta = Utils.Windows.DevuelveValorRedondeado(detail.d_PrecioVenta.Value / (1 + porcIgv), 2);
                detail.d_Igv = Utils.Windows.DevuelveValorRedondeado(detail.d_PrecioVenta - detail.d_ValorVenta ?? 0, 2);

                detail.d_Valor = Utils.Windows.DevuelveValorRedondeado(detail.d_ValorVenta.Value + detail.d_Descuento.Value, 2);
            }
            else
            {
                detail.d_ValorVenta = Utils.Windows.DevuelveValorRedondeado(detail.d_Valor - detail.d_Descuento ?? 0, 2);
                detail.d_Igv = esGravado ? Utils.Windows.DevuelveValorRedondeado(detail.d_ValorVenta.Value * porcIgv, 2) : 0M;
                detail.d_PrecioVenta = Utils.Windows.DevuelveValorRedondeado(detail.d_ValorVenta + detail.d_Igv ?? 0, 2);
            }
            detail.d_CantidadEmpaque = GetCantidadEmpaque(detail);
            CalcularTotales();
            grdData.UpdateData();
        }

        private decimal GetCantidadEmpaque(BindingPedidoDetalle detail)
        {
            if (detail.i_IdUnidadMedida == null) return 0M;
            if (detail.Empaque == null) detail.d_CantidadEmpaque = 0M;

            if (detail.v_IdProductoDetalle != null && detail.v_IdProductoDetalle != "N002-PE000000000" && detail.i_IdUnidadMedida != -1)
            {
                var totalEmpaque = 0M;
                var empaque = detail.Empaque.Value;
                var cantidad = detail.d_Cantidad.Value;

                var umProducto = ((List<GridKeyValueDTO>)ucbUnidadMedida.DataSource).FirstOrDefault(p => p.Id == detail.i_IdUnidadMedidaProducto.ToString());
                var um = ((List<GridKeyValueDTO>)ucbUnidadMedida.DataSource).FirstOrDefault(p => p.Id == detail.i_IdUnidadMedida.ToString());

                if (um != null)
                {
                    switch (um.Value1)
                    {
                        case "CAJA":
                            var caja = empaque * (!string.IsNullOrEmpty(umProducto.Value2) ? decimal.Parse(umProducto.Value2) : 0);
                            totalEmpaque = cantidad * caja;
                            break;

                        default:
                            totalEmpaque = cantidad * (!string.IsNullOrEmpty(um.Value2) ? decimal.Parse(um.Value2) : 0);
                            break;
                    }
                }
                return totalEmpaque;
            }
            else
                return 0M;
        }

        private void CalcularTotales()
        {

            if (grdData.Rows.Count() > 0)
            {

                decimal SumDescuentoR = 0, SumValVentaR = 0, SumIgvR = 0, SumTotalR = 0, SumValR = 0, CantidadTotalR = 0;

                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    var objRow = (BindingPedidoDetalle)Fila.ListObject;
                    if (objRow.i_IdTipoOperacion > 10) continue; // Operaciones no Onerosas
                    if (Fila.Cells["d_ValorVenta"].Value == null) { Fila.Cells["d_ValorVenta"].Value = "0"; }
                    if (Fila.Cells["d_Igv"].Value == null) { Fila.Cells["d_Igv"].Value = "0"; }
                    if (Fila.Cells["d_PrecioVenta"].Value == null) { Fila.Cells["d_PrecioVenta"].Value = "0"; }
                    if (Fila.Cells["d_Descuento"].Value == null) { Fila.Cells["d_Descuento"].Value = "0"; }
                    if (Fila.Cells["d_Valor"].Value == null) { Fila.Cells["d_Valor"].Value = "0"; }
                    if (Fila.Cells["d_Cantidad"].Value == null) { Fila.Cells["d_Cantidad"].Value = "0"; }

                    if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                    {
                        SumValR = Utils.Windows.DevuelveValorRedondeado(SumValR + decimal.Parse(Fila.Cells["d_Valor"].Text), 2);
                        SumValVentaR = Utils.Windows.DevuelveValorRedondeado(SumValVentaR + decimal.Parse(Fila.Cells["d_ValorVenta"].Text), 2);
                        SumIgvR = Utils.Windows.DevuelveValorRedondeado(SumIgvR + decimal.Parse(Fila.Cells["d_Igv"].Text), 2);
                        SumTotalR = Utils.Windows.DevuelveValorRedondeado(SumTotalR + decimal.Parse(Fila.Cells["d_PrecioVenta"].Text), 2);
                        SumaValoresVentaR = SumTotalR;
                        SumDescuentoR = SumDescuentoR + decimal.Parse(Fila.Cells["d_Descuento"].Text);

                        if (Fila.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000")
                        {
                            CantidadTotalR = CantidadTotalR + decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                        }
                    }
                    //else
                    //{

                    //    SumVal = SumVal + decimal.Parse(Fila.Cells["d_Valor"].Text);
                    //    SumValVenta = SumValVenta + decimal.Parse(Fila.Cells["d_ValorVenta"].Text);
                    //    SumIgv = SumIgv + decimal.Parse(Fila.Cells["d_Igv"].Text);
                    //    SumTotal = SumTotal + decimal.Parse(Fila.Cells["d_PrecioVenta"].Text);
                    //    SumaValoresVenta = SumTotal;
                    //    SumDescuento = SumDescuento + decimal.Parse(Fila.Cells["d_Descuento"].Text);
                    //    CantidadTotal = CantidadTotal + decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());

                    //}
                }

                decimal descuentoCab = string.IsNullOrEmpty(txtDescuento.Text.Trim()) ? 0 : decimal.Parse(txtDescuento.Text);
                decimal porcDecuento = decimal.TryParse(descuentoCab.ToString(), out porcDecuento) ? porcDecuento / 100 : 0;
                var factDesc = 1 - porcDecuento;
                txtValor.Text = (SumValR).ToString("0.00");

                txtValorVenta.Text = (SumValVentaR * factDesc).ToString("0.00");
                txtIgv.Text = (SumIgvR * factDesc).ToString("0.00");
                txtDscto.Text = (SumDescuentoR + SumValVentaR * porcDecuento).ToString("0.00");
                txtCantidadTotal.Text = (CantidadTotalR).ToString("0.00");
                Total = SumTotalR * factDesc;
                txtPrecioVenta.Text = Total.ToString("0.00");

                Redondeado = decimal.Parse(Math.Round(Total, 1, MidpointRounding.AwayFromZero).ToString("0.00"));
                Residuo = (Total - Redondeado) * -1;
                _Redondeado = Residuo == 0 ? true : false;
                btnRedondear.Enabled = Residuo == 0 ? false : true;
            }
        }
        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdData.ActiveCell == null) return;

            switch (grdData.ActiveCell.Column.Key)
            {

                case "v_CodigoInterno":
                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null && Globals.ClientSession.v_RucEmpresa != Constants.RucCMR)
                    {
                        e.SuppressKeyPress = true;
                    }
                    break;
                case "d_PrecioUnitario":

                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null)
                    {
                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000")
                        {
                            if (char.IsLetterOrDigit(Convert.ToChar(e.KeyCode)))
                            {
                                e.SuppressKeyPress = true;
                            }
                            else if (char.IsSymbol(Convert.ToChar(e.KeyCode)))
                            {
                                e.SuppressKeyPress = true;
                            }
                            else if (Convert.ToChar(e.KeyCode) < 200)
                            {
                                e.SuppressKeyPress = true;
                            }
                            else
                            {
                                e.SuppressKeyPress = true;
                            }

                        }

                        if (Globals.ClientSession.i_PrecioEditableTodosProductosPedido == 0)
                        {

                            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_PrecioEditable"].Value != null && grdData.Rows[grdData.ActiveRow.Index].Cells["i_PrecioEditable"].Value.ToString() == "0")
                            {
                                if (char.IsLetterOrDigit(Convert.ToChar(e.KeyCode)))
                                {
                                    e.SuppressKeyPress = true;
                                }
                                else if (char.IsSymbol(Convert.ToChar(e.KeyCode)))
                                {
                                    e.SuppressKeyPress = true;
                                }
                                else if (Convert.ToChar(e.KeyCode) < 200)
                                {
                                    e.SuppressKeyPress = true;
                                }
                                else
                                {
                                    e.SuppressKeyPress = true;
                                }

                            }
                        }
                    }

                    break;

                case "v_NombreProducto":

                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null)
                    {
                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000")
                        {
                            if (char.IsLetterOrDigit(Convert.ToChar(e.KeyCode)))
                            {
                                e.SuppressKeyPress = true;
                            }
                            else if (char.IsSymbol(Convert.ToChar(e.KeyCode)))
                            {
                                e.SuppressKeyPress = true;
                            }
                            else if (Convert.ToChar(e.KeyCode) < 200)
                            {
                                e.SuppressKeyPress = true;
                            }
                            else
                            {
                                e.SuppressKeyPress = true;
                            }

                        }

                        if (Globals.ClientSession.i_PrecioEditableTodosProductosPedido == 0)
                        {

                            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_PrecioEditable"].Value != null && grdData.Rows[grdData.ActiveRow.Index].Cells["EsNombreEditable"].Value.ToString() == "0")
                            {
                                if (char.IsLetterOrDigit(Convert.ToChar(e.KeyCode)))
                                {
                                    e.SuppressKeyPress = true;
                                }
                                else if (char.IsSymbol(Convert.ToChar(e.KeyCode)))
                                {
                                    e.SuppressKeyPress = true;
                                }
                                else if (Convert.ToChar(e.KeyCode) < 200)
                                {
                                    e.SuppressKeyPress = true;
                                }
                                else
                                {
                                    e.SuppressKeyPress = true;
                                }

                            }
                        }
                    }
                    break;
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
                    //e.SuppressKeyPress = true;
                    //return;
                }

            }

            if (e.KeyCode >= 0 && (grdData.ActiveCell.Column.Key == "i_IdUnidadMedida" && grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null))
            {
                // e.SuppressKeyPress = true;
            }


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

                    case Keys.Back | Keys.MButton | Keys.Space | Keys.RButton:

                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
                        {

                            if (Globals.ClientSession.i_PrecioEditableTodosProductosPedido == 0)
                            {
                                if (grdData.Rows[grdData.ActiveRow.Index].Cells["EsNombreEditable"].Value.ToString() == "0")
                                {

                                    switch (this.grdData.ActiveCell.Column.Key)
                                    {
                                        case "v_NombreProducto": e.SuppressKeyPress = true;
                                            break;
                                        case "d_PrecioUnitario": e.SuppressKeyPress = true;
                                            break;
                                    }


                                }
                                else
                                {
                                    e.SuppressKeyPress = false;
                                }
                            }
                        }
                        break;
                    case Keys.LButton | Keys.Back:
                        e.Handled = false;
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

                    case "d_PrecioUnitario":
                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null)
                        {
                            if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000")
                            {
                                e.Handled = false;
                            }
                            else
                            {

                                if (Globals.ClientSession.i_PrecioEditableTodosProductosPedido == 0)
                                {
                                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_PrecioEditable"].Value == null || grdData.Rows[grdData.ActiveRow.Index].Cells["i_PrecioEditable"].Value.ToString() == "0")
                                    //if (grdData.Rows[grdData.ActiveRow.Index].Cells["EsNombreEditable"].Value.ToString() == "0")
                                    {
                                        if (char.IsLetterOrDigit(e.KeyChar))
                                        {
                                            e.Handled = true;
                                        }
                                        else if (char.IsSymbol(e.KeyChar))
                                        {
                                            e.Handled = true;
                                        }
                                        else if (e.KeyChar < 200)
                                        {
                                            e.Handled = true;
                                        }
                                        else
                                        {
                                            e.Handled = true;
                                        }

                                    }
                                    else
                                    {
                                        Celda = grdData.ActiveCell;
                                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                                    }
                                }
                            }
                        }
                        break;

                    case "d_Valor":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;

                    case "d_ValorVenta":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;

                    case "d_Igv":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;

                    case "d_PrecioVenta":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);

                        break;

                    case "v_Descuento":
                        var Empresa = new NodeBL().ReporteEmpresa();

                        if (Empresa != null && Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim() == Constants.RucChayna)
                        {
                            if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null)
                            {
                                if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000")
                                {
                                    e.Handled = true;
                                }
                                else
                                {
                                    Celda = grdData.ActiveCell;
                                    Utils.Windows.NumeroDecimalCelda(Celda, e);

                                }
                            }

                        }


                        break;


                    case "i_NroUnidades":
                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null)
                        {
                            if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000")
                            {
                                e.Handled = true;
                            }
                            else
                            {
                                Celda = grdData.ActiveCell;
                                Utils.Windows.NumeroEnteroCelda(Celda, e);


                            }
                        }
                        break;

                    case "v_Observacion":
                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null)
                        {
                            if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000")
                            {
                                e.Handled = true;
                            }
                        }
                        break;
                    case "v_NombreProducto":
                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null)
                        {
                            //if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
                            //{

                            if (Globals.ClientSession.i_PrecioEditableTodosProductosPedido == 0)
                            {
                                if (grdData.Rows[grdData.ActiveRow.Index].Cells["EsNombreEditable"].Value == null || grdData.Rows[grdData.ActiveRow.Index].Cells["EsNombreEditable"].Value.ToString() == "0")
                                {
                                    if (char.IsLetterOrDigit(e.KeyChar))
                                    {
                                        e.Handled = true;
                                    }
                                    else if (char.IsSymbol(e.KeyChar))
                                    {
                                        e.Handled = true;
                                    }
                                    else if (e.KeyChar < 200)
                                    {
                                        e.Handled = true;
                                    }
                                    else
                                    {
                                        e.Handled = true;
                                    }

                                }
                            }
                            //}
                        }

                        break;
                }
            }
        }
        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            Point point = new Point(e.X, e.Y);
            UIElement uiElement = ((UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

            if (uiElement == null || uiElement.Parent == null) return;

            UltraGridRow row = (UltraGridRow)uiElement.GetContext(typeof(UltraGridRow));

            if (strModo != "Cobranza")
            {
                if (row == null)
                {

                    btnEliminar.Enabled = false;

                }
                else
                {
                    btnEliminar.Enabled = true;

                    //btnEliminar.Enabled = _btnEliminarDetalle;
                }

            }
        }
        private void CalcularValoresDetalle()
        {
            if (grdData.Rows.Count() == 0)
            {
                LimpiarTotales();
                return;
            }
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                CalcularValoresFila(Fila);
               
            }
        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            EmbeddableEditorBase editor = e.Cell.EditorResolved;

            if (editor.Value != DBNull.Value)
            {
                e.Cell.Value = editor.Value;
            }
            grdData.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
            if (e.Cell.Column.Key == "i_IdTipoOperacion")
                CalcularValoresFila(e.Cell.Row);
        }

        private void grdData_ClickCell(object sender, ClickCellEventArgs e)
        {

            if (grdData.ActiveRow == null) return;

            switch (e.Cell.Column.Key)
            {

                case "i_IdAlmacen":

                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null || Globals.ClientSession.i_CambiarAlmacenVentasDesdeVendedor == 1)
                    {
                        e.Cell.CancelUpdate();
                    }

                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null)
                    {
                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000")
                        {
                            e.Cell.CancelUpdate();
                        }
                    }

                    break;
                case "i_IdUnidadMedida":
                    {
                        //if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
                        //{
                        //    e.Cell.CancelUpdate();
                        //}

                    }
                    break;

                case "v_NombreProducto":

                    if (Globals.ClientSession.i_PrecioEditableTodosProductosPedido == 0)
                    {
                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
                        {

                            if (grdData.Rows[grdData.ActiveRow.Index].Cells["EsNombreEditable"].Value == null || grdData.Rows[grdData.ActiveRow.Index].Cells["EsNombreEditable"].Value.ToString() == "0")
                            {
                                e.Cell.CancelUpdate();
                            }

                        }
                    }
                    break;

                case "d_PrecioUnitario":
                    if (Globals.ClientSession.i_PrecioEditableTodosProductosPedido == 0)
                    {

                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
                        {
                            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_PrecioEditable"].Value == null || grdData.Rows[grdData.ActiveRow.Index].Cells["i_PrecioEditable"].Value.ToString() == "0")
                            {
                                e.Cell.CancelUpdate();
                            }
                        }
                    }
                    break;
                case "d_Descuento":
                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value != null)
                    {
                        if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000")
                        {

                            e.Cell.CancelUpdate();
                        }
                    }

                    break;

            }
        }

        private void grdData_KeyUp(object sender, KeyEventArgs e)
        {
            UltraGridCell c;
            c = grdData.ActiveCell;

            if (grdData.ActiveCell == null) return;

            switch (e.KeyCode)
            {


                case Keys.Up: if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value != null)
                    {
                        if (grdData.ActiveCell.Column.Key == "i_IdAlmacen")
                        {
                            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal" && grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
                            {
                                c.CancelUpdate();
                                return;
                            }
                            else if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "Temporal" && grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
                            {
                                c.CancelUpdate();
                                return;

                            }
                        }
                    }
                    break;

                case Keys.Down:
                    {
                        if (grdData.ActiveCell.Column.Key == "i_IdAlmacen")
                        {
                            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal" & grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
                            {
                                c.CancelUpdate();
                                return;
                            }
                            else if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "Temporal" & grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value != null)
                            {
                                c.CancelUpdate();
                                return;

                            }
                        }
                    }
                    break;
            }


        }
        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {

            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");
            if (e.Row.Cells["EsRedondeo"].Value != null)
            {
                e.Row.Appearance.ForeColor = Color.Red;
            }
            if (e.Row.Band.Index == 0 && e.Row.Cells["t_FechaCaducidad"].Value != null && DateTime.Parse(e.Row.Cells["t_FechaCaducidad"].Value.ToString()).Date.ToShortDateString() == Constants.FechaNula)
            {
                e.Row.Cells["t_FechaCaducidad"].Appearance.BackColor = Color.White;
                e.Row.Cells["t_FechaCaducidad"].Appearance.ForeColor = Color.White;
            }
        }

        public void CargarCombosDetalle()
        {

            OperationResult objOperationResult = new OperationResult();

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

            #region Configura Combo Almacen

            UltraGridBand _ultraGridBanda1 = new UltraGridBand("Band 0", -1);
            UltraGridColumn _ultraGridColumnaID1 = new UltraGridColumn("Id");
            UltraGridColumn _ultraGridColumnaDescripcion1 = new UltraGridColumn("Value1");
            _ultraGridColumnaID1.Header.Caption = "Cod.";
            _ultraGridColumnaID1.Width = 50;
            _ultraGridColumnaDescripcion1.Header.Caption = "Descripción";
            _ultraGridColumnaDescripcion1.Header.VisiblePosition = 0;
            _ultraGridColumnaDescripcion1.Width = 280;


            _ultraGridBanda1.Columns.AddRange(new object[] { _ultraGridColumnaDescripcion1, _ultraGridColumnaID1 });
            ucbAlmacen.DisplayLayout.BandsSerializer.Add(_ultraGridBanda1);
            ucbAlmacen.DropDownWidth = 270;
            ucbAlmacen.DropDownStyle = UltraComboStyle.DropDownList;
            #endregion

            ucbUnidadMedida.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            ucbAlmacen.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucbUnidadMedida, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 17, null), DropDownListAction.Select); //17-Unidad Medida 
            Utils.Windows.LoadUltraComboList(ucbAlmacen, "Id", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaComboGrid(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);


        }

        public void RecibirItems(List<UltraGridRow> Filas)
        {
            Boolean Saldo0 = false;
            int cantMensajes = 0, cantidadMensajesExistencia = 0;
            Boolean anteriorRegistro = false;
            Boolean ExistenciaGrilla = false;
            OperationResult objOperationResult = new OperationResult();
            for (int i = 0; i < Filas.Count; i++)
            {
                Saldo0 = false;
                ExistenciaGrilla = false;
                int IdAlmacen = int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdAlmacen"].Value.ToString());


                if (int.Parse(cboDocumento.Value.ToString()) == (int)TiposDocumentos.Pedido)
                {

                    int ValidarStockAlmacen = _objAlmacenBL.ObtenerAlmacen(ref objOperationResult, int.Parse(grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdAlmacen"].Value.ToString())).i_ValidarStockAlmacen ?? 0;
                    if (Filas[i].Cells["v_CodInterno"].Value != null)
                    {
                        if (Filas[i].Cells["i_EsServicio"].Value.ToString() == "0" && Filas[i].Cells["i_ValidarStock"].Value.ToString() == "1" && ValidarStockAlmacen == 1)
                        {

                            if (decimal.Parse(Filas[i].Cells["Saldo"].Value.ToString()) <= 0)
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
                    }

                }
                foreach (UltraGridRow Fila in grdData.Rows)
                {

                    if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                    {

                        if (Filas[i].Cells["v_IdProductoDetalle"].Value.ToString() == Fila.Cells["v_IdProductoDetalle"].Value.ToString() && IdAlmacen == int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()))
                        {
                            if (cantidadMensajesExistencia == 0)
                            {
                                UltraMessageBox.Show("Uno de los productos seleccionados ya existe en el detalle", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                ExistenciaGrilla = true;
                                cantidadMensajesExistencia = cantidadMensajesExistencia + 1;
                                //return;
                            }
                            else
                            {
                                ExistenciaGrilla = true;
                                cantidadMensajesExistencia = cantidadMensajesExistencia + 1;
                            }
                        }


                    }

                }
                var tipoOperacion = (string)cboTipoOperacion.Value != "5" ? cboTipoOperacion.Value.ToString() : "1";
                if (i == 0)
                {
                    if (!Saldo0)
                    {
                        if (!ExistenciaGrilla)
                        {
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString().Trim();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value = Filas[i].Cells["v_IdProductoAlmacen"].Value == null ? null : Filas[i].Cells["v_IdProductoAlmacen"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["d_SeparacionTotal"].Value = Filas[i].Cells["d_separacion"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_separacion"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["d_StockActual"].Value = Filas[i].Cells["StockActual"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["StockActual"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsAfectoDetraccion"].Value = Filas[i].Cells["i_EsAfectoDetraccion"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsAfectoDetraccion"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["d_PrecioUnitario"].Value = DevolverPrecioProducto(Filas[i].Cells["IdMoneda"].Value != null ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString()) : -1, Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()), int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);
                            grdData.Rows[grdData.ActiveRow.Index].Cells["EsNombreEditable"].Value = Filas[i].Cells["i_NombreEditable"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_NombreEditable"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_PrecioEditable"].Value = Filas[i].Cells["i_PrecioEditable"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_PrecioEditable"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["d_DescuentoLP"].Value = Filas[i].Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Descuento"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value = Filas[i].Cells["i_ValidarStock"] == null ? 0 : int.Parse(Filas[i].Cells["i_ValidarStock"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";

                           // grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroPedido"].Value = Filas[i].Cells["v_NroPedido"].Value == null ? null : Filas[i].Cells["v_NroPedido"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_SolicitarNroSerieSalida"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroSerieSalida"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_SolicitarNroLoteSalida"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroLoteSalida"].Value.ToString());
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroSerie"].Value = Filas[i].Cells["v_NroSerie"].Value == null || Filas[i].Cells["v_NroSerie"].Value == "" ? null : Filas[i].Cells["v_NroSerie"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroLote"].Value = Filas[i].Cells["v_NroLote"].Value == null || Filas[i].Cells["v_NroLote"].Value == "" ? null : Filas[i].Cells["v_NroLote"].Value.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["t_FechaCaducidad"].Value = Filas[i].Cells["t_FechaCaducidad"].Value == null ? null : Filas[i].Cells["t_FechaCaducidad"].Value.ToString();


                           
                            grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = tipoOperacion;
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
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString().Trim();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value = Filas[i].Cells["v_IdProductoAlmacen"].Value == null ? null : Filas[i].Cells["v_IdProductoAlmacen"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_SeparacionTotal"].Value = Filas[i].Cells["d_separacion"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_separacion"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_StockActual"].Value = Filas[i].Cells["StockActual"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["StockActual"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsAfectoDetraccion"].Value = Filas[i].Cells["i_EsAfectoDetraccion"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsAfectoDetraccion"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                                // grdData.Rows[grdData.ActiveRow.Index].Cells["d_PrecioUnitario"].Value = Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_PrecioUnitario"].Value = DevolverPrecioProducto(Filas[i].Cells["IdMoneda"].Value != null ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString()) : -1, Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()), int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);
                                grdData.Rows[grdData.ActiveRow.Index].Cells["EsNombreEditable"].Value = Filas[i].Cells["i_NombreEditable"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_NombreEditable"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_PrecioEditable"].Value = Filas[i].Cells["i_PrecioEditable"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_PrecioEditable"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_DescuentoLP"].Value = Filas[i].Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Descuento"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value = Filas[i].Cells["i_ValidarStock"] == null ? 0 : int.Parse(Filas[i].Cells["i_ValidarStock"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value = "Temporal";

                                //grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroPedido"].Value = Filas[i].Cells["v_NroPedido"].Value == null ? null : Filas[i].Cells["v_NroPedido"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_SolicitarNroSerieSalida"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroSerieSalida"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_SolicitarNroLoteSalida"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroLoteSalida"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroSerie"].Value = Filas[i].Cells["v_NroSerie"].Value == null || Filas[i].Cells["v_NroSerie"].Value == "" ? null : Filas[i].Cells["v_NroSerie"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroLote"].Value = Filas[i].Cells["v_NroLote"].Value == null || Filas[i].Cells["v_NroLote"].Value == "" ? null : Filas[i].Cells["v_NroLote"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["t_FechaCaducidad"].Value = Filas[i].Cells["t_FechaCaducidad"].Value == null ? null : Filas[i].Cells["t_FechaCaducidad"].Value.ToString();
                                grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = tipoOperacion;
                                anteriorRegistro = false;
                            }

                            else
                            {
                                UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                                if (row == null) return;
                                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                                this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                                row.Cells["i_RegistroEstado"].Value = "Agregado";
                                row.Cells["i_RegistroTipo"].Value = "Temporal";
                                row.Cells["i_IdUnidadMedida"].Value = "-1";
                                row.Cells["d_Cantidad"].Value = "0";
                                row.Cells["d_PrecioUnitario"].Value = "0";
                                row.Cells["i_IdUnidadMedida"].Value = "-1";
                                row.Cells["i_IdAlmacen"].Value = "-1";
                                row.Cells["d_Descuento"].Value = "0";
                                row.Cells["d_Valor"].Value = "0";
                                row.Cells["d_ValorVenta"].Value = "0";
                                row.Cells["d_PrecioVenta"].Value = "0";
                                if (Globals.ClientSession.i_CambiarAlmacenVentasDesdeVendedor == 1)
                                {
                                    OperationResult pobjOperationResult = new OperationResult();
                                    var Vendedor = _objVendedorBL.ObtenerVendedor(ref  pobjOperationResult, cboVendedor.Value.ToString());
                                    row.Cells["i_IdAlmacen"].Value = Vendedor != null && Vendedor.i_IdAlmacen != -1 && Vendedor.i_IdAlmacen != null ? Vendedor.i_IdAlmacen.Value.ToString() : Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                                    cboVendedor.Enabled = !grdData.Rows.Any();
                                }
                                else
                                {
                                    row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                                }
                                row.Activate();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString().Trim();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value = Filas[i].Cells["v_IdProductoAlmacen"].Value == null ? null : Filas[i].Cells["v_IdProductoAlmacen"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_SeparacionTotal"].Value = Filas[i].Cells["d_separacion"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_separacion"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_StockActual"].Value = Filas[i].Cells["StockActual"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["StockActual"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsAfectoDetraccion"].Value = Filas[i].Cells["i_EsAfectoDetraccion"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsAfectoDetraccion"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                                //grdData.Rows[grdData.ActiveRow.Index].Cells["d_PrecioUnitario"].Value = Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_PrecioUnitario"].Value = DevolverPrecioProducto(Filas[i].Cells["IdMoneda"].Value != null ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString()) : -1, Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()), int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);
                                grdData.Rows[grdData.ActiveRow.Index].Cells["EsNombreEditable"].Value = Filas[i].Cells["i_NombreEditable"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_NombreEditable"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_PrecioEditable"].Value = Filas[i].Cells["i_PrecioEditable"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_PrecioEditable"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_DescuentoLP"].Value = Filas[i].Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Descuento"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_ValidarStock"].Value = Filas[i].Cells["i_ValidarStock"] == null ? 0 : int.Parse(Filas[i].Cells["i_ValidarStock"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                                grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = tipoOperacion;
                                //grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroPedido"].Value = Filas[i].Cells["v_NroPedido"].Value == null ? null : Filas[i].Cells["v_NroPedido"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_SolicitarNroSerieSalida"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroSerieSalida"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["i_SolicitarNroLoteSalida"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroLoteSalida"].Value.ToString());
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroSerie"].Value = Filas[i].Cells["v_NroSerie"].Value == null || Filas[i].Cells["v_NroSerie"].Value == "" ? null : Filas[i].Cells["v_NroSerie"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroLote"].Value = Filas[i].Cells["v_NroLote"].Value == null || Filas[i].Cells["v_NroLote"].Value == "" ? null : Filas[i].Cells["v_NroLote"].Value.ToString();
                                grdData.Rows[grdData.ActiveRow.Index].Cells["t_FechaCaducidad"].Value = Filas[i].Cells["t_FechaCaducidad"].Value == null ? null : Filas[i].Cells["t_FechaCaducidad"].Value.ToString();

                            }
                        }
                    }
                }
            }
            CalcularValoresDetalle();
        }

        private void grdData_ClickCellButton(object sender, CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "d_PrecioUnitario":
                    string Id = grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null ? grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value.ToString() : string.Empty;
                    int IdAlmacen = grdData.ActiveRow.Cells["i_IdAlmacen"].Value != null ? int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()) : -1;

                    if (!string.IsNullOrEmpty(Id))
                    {
                        var PopUp = new ucListaPreciosPopUp(ucListaPreciosPopUp.TipoVenta.Pedido, Id, IdAlmacen);// idLista;
                        panelListaPrecios.ClientArea.Controls.Clear();
                        panelListaPrecios.ClientArea.Controls.Add(PopUp);
                        ultraPopupControlContainer1.Show();
                    }
                    break;

                case "v_CodigoInterno":
                    UltraGridCell celda = this.grdData.ActiveRow.Cells["v_CodigoInterno"];
                    DoubleClickCellEventArgs f = new DoubleClickCellEventArgs(celda);
                    grdData_DoubleClickCell(sender, f);
                    break;

            }


        }
        #endregion

        #region CRUD
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int CantidadMensajes = 0;
            bool Eliminar = false;
            List<UltraGridRow> Filas = new List<UltraGridRow>();
            List<pedidodetalleDto> Filita = new List<pedidodetalleDto>();

            if (grdData.ActiveRow == null) return;
            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _objPedidoDetalleDto = new pedidodetalleDto()
                    {
                        v_IdPedidoDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdPedidoDetalle"].Value.ToString(),
                        v_IdProductoDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value.ToString()
                    };
                    _TempDetalle_EliminarDto.Add(_objPedidoDetalleDto);

                    //if (cboDocumento.Value.ToString() == "430")
                    if (int.Parse(cboDocumento.Value.ToString()) == (int)TiposDocumentos.Pedido)
                    {
                        if (_objPedidoDetalleDto.v_IdProductoDetalle != "N002-PE000000000")
                        {
                            _objSeparacionProducto = new separacionproductoDto()
                            {
                                v_IdPedido = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdPedido"].Value.ToString(),
                                v_IdProductoAlmacen = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoAlmacen"].Value.ToString()
                            };

                            _TempDetalle_EliminarDtoSeparacion.Add(_objSeparacionProducto);
                        }
                    }

                    grdData.Rows[grdData.ActiveRow.Index].Delete(false);
                }
            }
            else
            {
               
                grdData.Rows[grdData.ActiveRow.Index].Delete(false);
               
            }
            CalcularValoresDetalle();
            int Posicion = 1;
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                Fila.Cells["Index"].Value = (Posicion).ToString("00");
                Posicion++;
            }
            if (Globals.ClientSession.i_CambiarAlmacenVentasDesdeVendedor == 1)
            {
                cboVendedor.Enabled = !grdData.Rows.Any();
            }
        }

        public void btnGuardar_Click(object sender, EventArgs e)
        {
            if (_Mode == "New" && grdData.Rows.Any(fila => fila.Cells["v_IdProductoDetalle"].Value != null) &&
                grdData.Rows.Any(fila => fila.Cells["v_IdProductoDetalle"].Value == null))
            {
                var filasVacias = grdData.Rows.Where(fila => fila.Cells["v_IdProductoDetalle"].Value == null).ToList();
                filasVacias.ForEach(fila => fila.Delete(false));
            }

            OperationResult objOperationResult = new OperationResult();

            if (uvPedido.Validate(true, false).IsValid)
            {


                if (Globals.ClientSession.i_CambiarAlmacenVentasDesdeVendedor != 1 || Globals.ClientSession.UsuarioEsContable == 0)
                {
                    if (!ValidarVendedor.Validate(true, false).IsValid)
                    {
                        return;
                    }
                }
                else if (Globals.ClientSession.i_CambiarAlmacenVentasDesdeVendedor == 1)
                {
                    if (Globals.ClientSession.UsuarioEsContable == 0)
                    {
                        if (!ValidarVendedor.Validate(true, false).IsValid)
                        {
                            return;
                        }
                    }
                }

                if (cboDocumento.Value.ToString() == "-1")
                {
                    UltraMessageBox.Show("Por favor ingrese un Documento de referencia válido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (txtTipoCambio.Text == "")
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTipoCambio.Focus();
                    return;

                }
                else if (decimal.Parse(txtTipoCambio.Text) <= 0)
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTipoCambio.Focus();
                    return;

                }
                if (_objPedido.v_IdCliente == null)
                {
                    UltraMessageBox.Show("Por favor ingrese un cliente que exista  ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtRucCliente.Focus();
                    return;

                }

                if (grdData.Rows.Count() == 0)
                {
                    UltraMessageBox.Show("No se permite guardar mientras el detalle está vacío", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    if (ValidaCamposNulosVacios() == true)
                    {

                        if (EsVentaAfectaDetraccion() == true)
                        {
                            if (UltraMessageBox.Show("El documento es Afecto Detracción, ¿Desea continuar?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.No)
                            {
                                return;
                            }
                        }

                        var Empresa = new NodeBL().ReporteEmpresa();
                        //Empresa.FirstOrDefault ().RucEmpresaPropietaria  = Constants.RucChayna;
                        if (Empresa != null && Empresa.FirstOrDefault().RucEmpresaPropietaria.Trim() == Constants.RucChayna)
                        {
                            MensajeDsc = VerificarDescuento(); // Se verifica en Pedido-Cotizacion - Para cualquier tipo de Producto
                            if (MensajeDsc != "")
                            {
                                UltraMessageBox.Show(MENSAJEDSC + MensajeDsc, "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;

                            }
                        }
                        if (int.Parse(cboDocumento.Value.ToString()) == (int)TiposDocumentos.Pedido)
                        {
                            Mensaje = VerificarSaldoProductoAlmacen();
                            if (Mensaje != "")
                            {
                                UltraMessageBox.Show(MENSAJE + Mensaje, "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;

                            }
                            if (_objPedido.v_IdCliente == "N002-CL000000000" & _objPedido.d_Valor == Globals.ClientSession.d_ValorMaximoBoletas)
                            {
                                UltraMessageBox.Show("Es necesario que registre al Cliente", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;

                            }

                        }
                        if (Residuo != 0 && grdData.Rows.Count() != 0 && grdData.Rows.Where(p => p.Cells["v_IdProductoDetalle"].Value != null && p.Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000").Count() != 0)
                        {
                            UltraMessageBox.Show("Por favor elimine el anterior redondeo para continuar.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (Residuo != 0 && Globals.ClientSession.i_RedondearVentas == 1)
                        {
                            btnRedondear_Click(sender, e);
                        }
                            CalcularValoresDetalle();
                        
                        if (cboEstados.Value != null && cboEstados.Value.ToString() == "3" && _objPedidoBL.VerificarSiTieneFacturas(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim(), txtNumeroDoc.Text.Trim()))
                        {
                            UltraMessageBox.Show("Este registro no se puede anular , ya tiene ventas generadas", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (_Mode == "New")
                        {
                            while (_objPedidoBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text) == false)
                            {
                                txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                            }
                            while (_objPedidoBL.ExisteNroDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text, txtNumeroDoc.Text) == false)
                            {
                                txtNumeroDoc.Text = (int.Parse(txtNumeroDoc.Text) + 1).ToString("00000000");
                            }
                            _objPedido.v_Periodo = txtPeriodo.Text;
                            _objPedido.v_Mes = txtMes.Text;
                            _objPedido.v_Correlativo = txtCorrelativo.Text;
                            _objPedido.i_IdIgv = int.Parse(cboIGV.Value.ToString());
                            _objPedido.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                            _objPedido.v_SerieDocumento = txtSerieDoc.Text.Trim();
                            _objPedido.v_CorrelativoDocumento = txtNumeroDoc.Text.Trim();
                            _objPedido.t_FechaEmision = dtpFechaEmision.Value.Date;
                            _objPedido.i_DiasVigencia = int.Parse(txtDiasVigencia.Text);
                            _objPedido.t_FechaVencimiento = dtpFechaVencimiento.Value.Date;
                            _objPedido.d_TipoCambio = decimal.Parse(txtTipoCambio.Text);
                            _objPedido.i_IdCondicionPago = int.Parse(cboCondicionPago.Value.ToString());
                            _objPedido.v_Glosa = txtGlosa.Text;
                            _objPedido.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                            _objPedido.i_IdEstado = int.Parse(cboEstados.Value.ToString());
                            _objPedido.i_AfectoIgv = chkAfectoIGV.Checked == true ? 1 : 0;
                            _objPedido.i_PrecionInclIgv = chkPrecInIGV.Checked == true ? 1 : 0;
                            _objPedido.d_Dscto = decimal.Parse(txtDescuento.Text.Trim());
                            _objPedido.v_IdVendedor = cboVendedor.Value.ToString();
                            _objPedido.v_IdVendedorRef = cboVendedorRef.Value.ToString();
                            _objPedido.d_Valor = decimal.Parse(txtValor.Text);
                            _objPedido.d_VVenta = decimal.Parse(txtValorVenta.Text);
                            _objPedido.d_Igv = decimal.Parse(txtIgv.Text);
                            _objPedido.d_CantidadTotal = decimal.Parse(txtCantidadTotal.Text);
                            _objPedido.d_PrecioVenta = decimal.Parse(txtPrecioVenta.Text);
                            _objPedido.d_Descuento = decimal.Parse(txtDscto.Text);
                            _objPedido.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());
                            _objPedido.t_FechaDespacho = dtpFechaDespacho.Value.Date;
                            _objPedido.i_IdTipoOperacion = int.Parse(cboTipoOperacion.Value.ToString());
                            _objPedido.v_IdAgenciaTransporte = cboAgenciaTransporte.Value == null ? "-1" : cboAgenciaTransporte.Value.ToString();
                            _objPedido.i_IdTipoVerificacion =int.Parse (cboVerificacion.Value.ToString ());
                            if (_objPedido.v_IdCliente == "N002-CL000000000")
                            {
                                if (PubligoGen.Trim() != txtRazonCliente.Text.Trim().Replace(" ", ""))
                                {
                                    _objPedido.v_NombreClienteTemporal = txtRazonCliente.Text;
                                }
                                else
                                {
                                    _objPedido.v_NombreClienteTemporal = string.Empty;
                                }
                                if (DireccionGen.Trim() != txtDireccion.Text.Trim().Replace(" ", ""))
                                {
                                    _objPedido.v_DireccionClienteTemporal = txtDireccion.Text;
                                }
                                else
                                {
                                    _objPedido.v_DireccionClienteTemporal = string.Empty;
                                }

                            }
                            else
                            {
                                _objPedido.v_DireccionClienteTemporal = txtDireccion.Text.Trim();
                                _objPedido.v_NombreClienteTemporal = string.Empty;
                            }

                            LlenarTemporales();
                            newIdPedido = _objPedidoBL.InsertarPedido(ref objOperationResult, _objPedido, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_AgregarDtoSeparacion);



                        }

                        else if (_Mode == "Edit")
                        {


                            _objPedido.v_Periodo = txtPeriodo.Text;
                            _objPedido.v_Mes = txtMes.Text;
                            _objPedido.v_Correlativo = txtCorrelativo.Text;
                            _objPedido.i_IdIgv = int.Parse(cboIGV.Value.ToString());
                            _objPedido.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                            _objPedido.v_SerieDocumento = txtSerieDoc.Text.Trim();
                            _objPedido.v_CorrelativoDocumento = txtNumeroDoc.Text.Trim();
                            _objPedido.t_FechaEmision = dtpFechaEmision.Value.Date;
                            _objPedido.i_DiasVigencia = int.Parse(txtDiasVigencia.Text);
                            _objPedido.t_FechaVencimiento = dtpFechaVencimiento.Value.Date;
                            _objPedido.d_TipoCambio = decimal.Parse(txtTipoCambio.Text);
                            _objPedido.i_IdCondicionPago = int.Parse(cboCondicionPago.Value.ToString());
                            _objPedido.v_Glosa = txtGlosa.Text;
                            _objPedido.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                            _objPedido.i_IdEstado = int.Parse(cboEstados.Value.ToString());
                            _objPedido.i_AfectoIgv = chkAfectoIGV.Checked == true ? 1 : 0;
                            _objPedido.i_PrecionInclIgv = chkPrecInIGV.Checked == true ? 1 : 0;
                            _objPedido.d_Dscto = decimal.Parse(txtDescuento.Text.Trim());
                            _objPedido.v_IdVendedor = cboVendedor.Value.ToString();
                            _objPedido.v_IdVendedorRef = cboVendedorRef.Value.ToString();
                            _objPedido.d_Valor = decimal.Parse(txtValor.Text);
                            _objPedido.d_VVenta = decimal.Parse(txtValorVenta.Text);
                            _objPedido.d_Igv = decimal.Parse(txtIgv.Text);
                            _objPedido.d_CantidadTotal = decimal.Parse(txtCantidadTotal.Text);
                            _objPedido.d_PrecioVenta = decimal.Parse(txtPrecioVenta.Text);
                            _objPedido.d_Descuento = decimal.Parse(txtDscto.Text);
                            _objPedido.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());
                            _objPedido.t_FechaDespacho = dtpFechaDespacho.Value.Date;
                            _objPedido.i_IdTipoOperacion = int.Parse(cboTipoOperacion.Value.ToString());
                            _objPedido.v_IdAgenciaTransporte = cboAgenciaTransporte.Value == null ? "-1" : cboAgenciaTransporte.Value.ToString();
                            if (_objPedido.v_IdCliente == "N002-CL000000000")
                            {
                                if (PubligoGen != txtRazonCliente.Text.Trim().Replace(" ", ""))
                                {
                                    _objPedido.v_NombreClienteTemporal = txtRazonCliente.Text;
                                }
                                else
                                {
                                }
                                if (DireccionGen.Trim() != txtDireccion.Text.Trim().Replace(" ", ""))
                                {
                                    _objPedido.v_DireccionClienteTemporal = txtDireccion.Text;
                                }
                                else
                                { 
                                }
                            }
                            else
                            {
                                _objPedido.v_NombreClienteTemporal = string.Empty;
                                _objPedido.v_DireccionClienteTemporal = txtDireccion.Text.Trim();
                            }
                            _objPedido.i_IdTipoVerificacion = int.Parse(cboVerificacion.Value.ToString());
                            LlenarTemporales();
                            newIdPedido = _objPedidoBL.ActualizarPedido(ref objOperationResult, _objPedido, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto, _TempDetalle_AgregarDtoSeparacion, _TempDetalle_ModificarDtoSeparacion, _TempDetalle_EliminarDtoSeparacion);
                        }

                        if (objOperationResult.Success == 1)
                        {
                            strModo = "Guardado";
                            Liberacion = false;
                            EdicionBarraNavegacion(true);
                            strIdPedido = newIdPedido;
                            ObtenerListadoPedidos(txtPeriodo.Text, txtMes.Text);
                            _pstrIdMovimiento_Nuevo = newIdPedido;
                            _idPedido = _pstrIdMovimiento_Nuevo;
                            BtnImprimir.Enabled = _btnImprimir;
                           
                            if (UltraMessageBox.Show("El Pedido se ha guardado correctamente ,¿Desea Generar  Nuevo Pedido?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                            {
                                strModo = "Nuevo";
                                btnNuevoMovimiento_Click(sender, e);
                                btnGuardar.Enabled = true;
                                btnAgregar.Enabled = true;
                                grdData.Enabled = true;
                            }

                        }
                        else
                        {
                            UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        _TempDetalle_AgregarDto = new List<pedidodetalleDto>();
                        _TempDetalle_ModificarDto = new List<pedidodetalleDto>();
                        _TempDetalle_EliminarDto = new List<pedidodetalleDto>();
                        _TempDetalle_AgregarDtoSeparacion = new List<separacionproductoDto>();
                        _TempDetalle_ModificarDtoSeparacion = new List<separacionproductoDto>();
                        _TempDetalle_EliminarDtoSeparacion = new List<separacionproductoDto>();
                    }
                }
            }
            else
            {

                UltraMessageBox.Show("Por favor llene los campos requeridos antes de guardar", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (ValidarAgregarDetalle.Validate(true, false).IsValid)
            {
                if (Globals.ClientSession.i_CambiarAlmacenVentasDesdeVendedor == 1)
                {
                    if (!ValidarVendedor.Validate(true, false).IsValid)
                    {
                        return;
                    }
                }
                if (txtTipoCambio.Text.Trim() == "")
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTipoCambio.Focus();
                    return;

                }
                else if (decimal.Parse(txtTipoCambio.Text) <= 0)
                {
                    UltraMessageBox.Show("Por Favor ingrese un Tipo de cambio válido ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTipoCambio.Focus();
                    return;

                }
                if (cboIGV.Value.ToString() == "-1")
                {
                    UltraMessageBox.Show("Por favor seleccione un valor para el IGV", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cboIGV.Focus();
                    return;
                }

                if (cboDocumento.Value.ToString() == "-1")
                {
                    UltraMessageBox.Show("Por favor seleccione el tipo de Documento ", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cboDocumento.Focus();
                    return;

                }
                if (grdData.Rows.Where(p => p.Cells["v_IdProductoDetalle"].Value != null && p.Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000").Count() != 0)
                {
                    UltraMessageBox.Show("Antes de continuar por favor elimine el redondeo", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_objPedido.v_IdCliente == null)
                {
                    UltraMessageBox.Show("Por favor ingrese un Cliente existente ", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtRucCliente.Focus();
                    return;

                }
                var ultimaFila = grdData.Rows.LastOrDefault();
                if (ultimaFila == null || (ultimaFila.Cells["i_IdAlmacen"].Value != null && ultimaFila.Cells["v_IdProductoDetalle"].Value != null))
                {

                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    if (row == null) return;
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Agregado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_IdUnidadMedida"].Value = "-1";
                    row.Cells["d_Cantidad"].Value = "0";
                    row.Cells["d_PrecioUnitario"].Value = "0";
                    row.Cells["i_IdUnidadMedida"].Value = "-1";
                    row.Cells["i_IdAlmacen"].Value = "-1";
                    row.Cells["d_Descuento"].Value = "0";
                    row.Cells["d_Valor"].Value = "0";
                    row.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null && cboTipoOperacion.Value.ToString() != "5" ? cboTipoOperacion.Value.ToString() : "1";
                    row.Cells["d_ValorVenta"].Value = "0";
                    row.Cells["d_PrecioVenta"].Value = "0";
                    if (Globals.ClientSession.i_CambiarAlmacenVentasDesdeVendedor == 1)
                    {
                        //OperationResult pobjOperationResult = new OperationResult();
                        //var Vendedor = _objVendedorBL.ObtenerVendedor(ref  pobjOperationResult, cboVendedor.Value.ToString());
                        //row.Cells["i_IdAlmacen"].Value = Vendedor != null && Vendedor.i_IdAlmacen != -1 && Vendedor.i_IdAlmacen != null ? Vendedor.i_IdAlmacen.Value.ToString() : Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                        //cboVendedor.Enabled = !grdData.Rows.Any();
                        OperationResult pobjOperationResult = new OperationResult();
                        var Almacen = _objEstablecimientoBL.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimiento.Value.ToString()));
                        row.Cells["i_IdAlmacen"].Value = Almacen != null && Almacen.FirstOrDefault() != null && Almacen.FirstOrDefault().Id != "-1" ? Almacen.FirstOrDefault().Id.ToString() : Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                        cboVendedor.Enabled = !grdData.Rows.Any();


                    }
                    else
                    {
                        row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    }
                }

                grdData.Focus();
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_CodigoInterno"];
                this.grdData.ActiveCell = aCell;
                grdData.ActiveRow = aCell.Row;
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);


            }

        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                OperationResult pOperationResult = new OperationResult();
                decimal TipoCambio = 0;
                TipoCambio = decimal.Parse(_objVentaBL.DevolverTipoCambioPorFecha(ref pOperationResult, DateTime.Today.Date));

                if (TipoCambio == 0)
                {
                    UltraMessageBox.Show("No se ha registrado ningún tipo de cambio para el día de hoy.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                using (new PleaseWait(this.Location, "Facturando..."))
                {
                    _objVentaBL.ConvertOrderToSale(ref pOperationResult, _objPedido.v_IdPedido, Globals.ClientSession.GetAsList());
                }

                if (pOperationResult.Success == 0)
                {
                    UltraMessageBox.Show(pOperationResult.ErrorMessage + "\n\n" + pOperationResult.ExceptionMessage + "\n\nTARGET: " + pOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                frmBandejaPedidosFacturados frm = new frmBandejaPedidosFacturados(_objPedido.v_SerieDocumento + " - " + _objPedido.v_CorrelativoDocumento);
                frm.ShowDialog();
                btnGenerar.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnRedondear_Click(object sender, EventArgs e)
        {
            Redondeo();
        }

        public void Redondeo()
        {
            if (Residuo != 0 && _Redondeado == false)
            {
                _limiteDocumento++;
                _Redondeado = true;
                UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["d_Cantidad"].Value = "1";
                row.Cells["d_PrecioUnitario"].Value = Residuo.ToString();
                row.Cells["i_IdUnidadMedida"].Value = "-1";
                row.Cells["d_Descuento"].Value = "0";
                row.Cells["d_Valor"].Value = "0";
                row.Cells["EsRedondeo"].Value = "1";
                row.Cells["i_IdUnidadMedida"].Value = "15";
                row.Cells["v_IdProductoDetalle"].Value = "N002-PE000000000";
                row.Cells["t_FechaCaducidad"].Value = DateTime.Parse(Constants.FechaNula);
                //row.Cells["v_IdProductoAlmacen"].Value = "N002-PA000000000";
                row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                string[] Cadena = new string[12];

                Cadena = _objPedidoBL.DevolverProductos(row.Cells["v_IdProductoDetalle"].Value.ToString());
                row.Cells["v_CodigoInterno"].Value = Cadena[0];
                row.Cells["v_NombreProducto"].Value = Cadena[1];
                row.Cells["i_EsServicio"].Value = Cadena[8];
                row.Cells["EsNombreEditable"].Value = Cadena[10];

                txtPrecioVenta.Text = Redondeado.ToString("0.00");
                row.Appearance.ForeColor = Color.Red;
                CalcularValoresDetalle();
                btnRedondear.Enabled = false;
            }
            else if (Residuo == 0)
            {
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["EsRedondeo"].Value != null)
                    {
                        Fila.Delete();
                    }
                }
            }
        }
        #endregion

        #region Comportamiento de Controles
        private void txtTipoCambio_KeyPress(object sender, KeyPressEventArgs e)
        {
            //  Utils.Windows.NumeroDecimal(txtTipoCambio, e);
        }

        private void txtSerieDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc, e);
        }

        private void txtNumeroDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtNumeroDoc, e);
        }

        private void txtDiasVigencia_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtDiasVigencia, e);
        }

        private void txtDescuento_KeyPress(object sender, KeyPressEventArgs e)
        {

            Utils.Windows.NumeroDecimal2UltraTextBox(txtDescuento, e);
        }

        private void cboDocumento_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnBuscarCliente_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtRucCliente.Text.Trim());
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {
                if (idLista != frm._IdLista)
                {
                    if (grdData.Rows.Count() > 0)
                    {
                        if (UltraMessageBox.Show("¿Al realizar la edición del Cliente perderá todos los detalles de este pedido,Desea Continuar?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (_Mode == "Edit")
                            {
                                Eliminar();
                            }
                            grdData.Selected.Rows.AddRange((UltraGridRow[])this.grdData.Rows.All);
                            grdData.DeleteSelectedRows(false);

                        }
                        else
                        {
                            //txtRucCliente.Undo();
                            //txtRucCliente.ClearUndo();
                            return;
                        }
                    }
                }

                _objPedido.v_IdCliente = frm._IdCliente;
                _objPedido.i_IdDireccionCliente = frm._IdDireccionCliente;
                txtRucCliente.Text = frm._NroDocumento;
                txtCodigoCliente.Text = frm._CodigoCliente;
                txtRazonCliente.Text = frm._RazonSocial;
                txtDireccion.Text = frm._Direccion;
                idLista = frm._IdLista;

                if (_objPedido.v_IdCliente == "N002-CL000000000")
                {
                    PubligoGen = txtRazonCliente.Text.Trim().Replace(" ", "");
                    DireccionGen = txtDireccion.Text.Trim().Replace(" ", "");
                    txtRazonCliente.Enabled = true;
                    txtDireccion.Enabled = true;
                    txtRazonCliente.Focus();
                }
                else
                {
                    DireccionGen = string.Empty;
                    PubligoGen = string.Empty;
                    txtRazonCliente.Enabled = false;
                    txtDireccion.Enabled = false;
                }

            }
            else
            {
                idLista = 0;
            }
        }

        private void txtRucCliente_KeyDown(object sender, KeyEventArgs e)
        {
            //List<string > Lista 

            OperationResult objOperationResult = new OperationResult();
            if (!txtRucCliente.IsDroppedDown && e.KeyCode == Keys.Enter)
            {
                //txtRazonCliente.Clear();
                //txtDireccion.Clear();
                //txtCodigoCliente.Clear();
                #region Pop-Pup
                if (txtRucCliente.Text.Trim() != "" & txtRucCliente.TextLength <= 7)
                {

                    Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtRucCliente.Text.Trim());
                    frm.ShowDialog();
                    if (frm._IdCliente != null)
                    {
                        if (idLista != frm._IdLista)
                        {
                            if (grdData.Rows.Count() > 0)
                            {

                                if (UltraMessageBox.Show("¿Al realizar la edición del Cliente perderá todos los detalles de este pedido,Cliente pertenece a Lista Diferente al Cliente Anterior ,Desea Continuar?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    if (_Mode == "Edit")
                                    {
                                        Eliminar();
                                    }
                                    grdData.Selected.Rows.AddRange((UltraGridRow[])this.grdData.Rows.All);
                                    grdData.DeleteSelectedRows(false);

                                }
                                else
                                {
                                    //txtRucCliente.Undo();
                                    //txtRucCliente.ClearUndo();
                                    return;
                                }
                            }
                        }
                        _objPedido.v_IdCliente = frm._IdCliente;
                        _objPedido.i_IdDireccionCliente = frm._IdDireccionCliente;
                        txtCodigoCliente.Text = frm._CodigoCliente;
                        txtRazonCliente.Text = frm._RazonSocial;
                        txtRucCliente.Text = frm._NroDocumento;
                        txtDireccion.Text = frm._Direccion;
                        idLista = frm._IdLista;

                        if (_objPedido.v_IdCliente == "N002-CL000000000")
                        {
                            PubligoGen = txtRazonCliente.Text.Trim().Replace(" ", "");
                            DireccionGen = txtDireccion.Text.Trim().Replace(" ", "");
                            txtRazonCliente.Enabled = true;
                            txtDireccion.Enabled = true;
                            txtRazonCliente.Focus();

                        }
                        else
                        {
                            PubligoGen = string.Empty;
                            DireccionGen = string.Empty;
                            txtRazonCliente.Enabled = false;
                            txtDireccion.Enabled = false;
                        }

                    }
                    else
                    {
                        txtCodigoCliente.Clear();
                        txtRazonCliente.Clear();
                        txtDireccion.Clear();
                        idLista = 0;

                    }
                }

                #endregion
                else
                {
                    #region BusquedaCliente
                    if (txtRucCliente.TextLength == 8 | txtRucCliente.TextLength == 10 | txtRucCliente.TextLength == 11)
                    {
                        string[] DatosCliente = new string[4];
                        DatosCliente = _objPedidoBL.DevolverClientePorNroDocumento(ref objOperationResult, txtRucCliente.Text.Trim());
                        if (DatosCliente != null)
                        {
                            if (grdData.Rows.Count() > 0)
                            {
                                if (int.Parse(DatosCliente[3]) != idLista)
                                {
                                    if (UltraMessageBox.Show("¿Al realizar la edición del Cliente perderá todos los detalles de este pedido,Cliente pertenece a Lista Diferente al Cliente Anterior ,Desea Continuar?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {

                                        if (_Mode == "Edit")
                                        {
                                            Eliminar();
                                        }
                                        grdData.Selected.Rows.AddRange((UltraGridRow[])this.grdData.Rows.All);
                                        grdData.DeleteSelectedRows(false);

                                    }
                                    else
                                    {
                                        //txtRucCliente.Undo();
                                        //txtRucCliente.ClearUndo();

                                        return;
                                    }
                                }

                            }
                            _objPedido.v_IdCliente = DatosCliente[0];
                            _objPedido.i_IdDireccionCliente = int.Parse(DatosCliente[5]);
                            txtCodigoCliente.Text = DatosCliente[1];
                            txtRazonCliente.Text = DatosCliente[2];
                            txtDireccion.Text = DatosCliente[4];
                            idLista = Convert.ToInt32(DatosCliente[3]);

                            if (_objPedido.v_IdCliente == "N002-CL000000000")
                            {
                                PubligoGen = txtRazonCliente.Text.Trim().Replace(" ", "");
                                DireccionGen = txtDireccion.Text.Trim().Replace(" ", "");
                                txtRazonCliente.Enabled = true;
                                txtDireccion.Enabled = true;
                                txtRazonCliente.Focus();
                            }
                            else
                            {
                                PubligoGen = string.Empty;
                                DireccionGen = string.Empty;
                                txtRazonCliente.Enabled = false;
                                txtDireccion.Enabled = false;
                            }

                        }
                        else // LLamo a Cliente rápido
                        {
                            #region Cliente Rapido

                            Mantenimientos.frmRegistroRapidoCliente frm = new Mantenimientos.frmRegistroRapidoCliente(txtRucCliente.Text.Trim(), "C");
                            frm.ShowDialog();
                            if (frm._Guardado == true)
                            {
                                txtRucCliente.Text = frm._NroDocumentoReturn;
                                DatosCliente = _objPedidoBL.DevolverClientePorNroDocumento(ref objOperationResult, txtRucCliente.Text.Trim());
                                if (DatosCliente != null)
                                {
                                    if (idLista != int.Parse(DatosCliente[3]))
                                    {

                                        if (grdData.Rows.Count() > 0)
                                        {

                                            if (UltraMessageBox.Show("¿Al realizar la edición del Cliente perderá todos los detalles de este pedido,Cliente pertenece a Lista Diferente al Cliente Anterior ,Desea Continuar?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                            {
                                                if (_Mode == "Edit")
                                                {
                                                    Eliminar();
                                                }
                                                grdData.Selected.Rows.AddRange((UltraGridRow[])this.grdData.Rows.All);
                                                grdData.DeleteSelectedRows(false);
                                            }
                                        }


                                        else
                                        {
                                            //txtRucCliente.Undo();
                                            //txtRucCliente.ClearUndo();

                                            return;
                                        }
                                    }
                                    _objPedido.v_IdCliente = DatosCliente[0];
                                    _objPedido.i_IdDireccionCliente = int.Parse(DatosCliente[5]);
                                    txtCodigoCliente.Text = DatosCliente[1];
                                    txtRazonCliente.Text = DatosCliente[2];
                                    idLista = Convert.ToInt32(DatosCliente[3]);
                                    txtDireccion.Text = DatosCliente[4];
                                    if (_objPedido.v_IdCliente == "N002-CL000000000")
                                    {
                                        PubligoGen = txtRazonCliente.Text.Trim().Replace(" ", "");
                                        DireccionGen = txtDireccion.Text.Trim().Replace(" ", "");
                                        txtRazonCliente.Enabled = true;
                                        txtDireccion.Enabled = true;
                                        txtRazonCliente.Focus();
                                    }
                                    else
                                    {
                                        PubligoGen = string.Empty;
                                        DireccionGen = string.Empty;
                                        txtRazonCliente.Enabled = false;
                                        txtDireccion.Enabled = false;
                                    }


                                }
                                else
                                {
                                    idLista = 0;
                                    PubligoGen = string.Empty;
                                    DireccionGen = string.Empty;
                                }

                            }

                            #endregion
                        }

                    }
                    #endregion

                }

            }
        }


        private void dtpFechaEmision_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string TipoCambio = _objPedidoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);

            txtTipoCambio.Text = TipoCambio;
            if (Scroll >= int.Parse(dtpFechaEmision.Value.Month.ToString()))
            {
                dtpFechaVencimiento.MinDate = dtpFechaEmision.Value;
                dtpFechaVencimiento.Value = dtpFechaEmision.Value; // Solo esto,antes de agregar periodo
            }
            else
            {
                dtpFechaVencimiento.ResetText();
                dtpFechaVencimiento.MinDate = DateTime.Parse("01" + "/" + "01" + "/" + "1753");
                dtpFechaVencimiento.MaxDate = DateTime.Parse("31" + "/" + "12" + "/" + "9998");
                dtpFechaVencimiento.Value = dtpFechaEmision.Value;

            }
            Scroll = int.Parse(dtpFechaEmision.Value.Month.ToString());
            if (strModo == "Nuevo")
            {
                GenerarNumeroRegistro();
            }

            else
            {
                string AnioCambiado = dtpFechaEmision.Value.Year.ToString().Trim();
                string MesCambiado = dtpFechaEmision.Value.Month.ToString("00");     //int.Parse(dtpFechaEmision.Value.Month.ToString()) <= 9 ? ("0" + dtpFechaEmision.Value.Month.ToString()).Trim() : dtpFechaEmision.Value.Month.ToString();
                if (MesCambiado.Trim() != _objPedido.v_Mes.Trim() || AnioCambiado != _objPedido.v_Periodo.Trim())
                {
                    GenerarNumeroRegistro();
                }
                else
                {
                    txtPeriodo.Text = _objPedido.v_Periodo.Trim();
                    txtMes.Text = _objPedido.v_Mes.Trim();
                    txtCorrelativo.Text = _objPedido.v_Correlativo.Trim();

                }
            }
            if (_objCierreMensualBL.VerificarMesCerrado(txtPeriodo.Text.Trim(), txtMes.Text.Trim(), (int)ModulosSistema.VentasFacturacion))
            {
                btnGuardar.Visible = false;
                this.Text = "Pedido/Cotización [MES CERRADO]";
            }
            else
            {
                btnGuardar.Visible = true;
                this.Text = "Pedido/Cotización";
            }
        }

        private void txtDiasVigencia_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDiasVigencia.Text))
            {
                dtpFechaVencimiento.Value = dtpFechaEmision.Value.AddDays(int.Parse(txtDiasVigencia.Text.Trim()));
            }
        }

        private void txtRucCliente_TextChanged(object sender, EventArgs e)
        {
            if (txtRucCliente.Text == string.Empty)
            {
                _objPedido.v_IdCliente = null;
                _objPedido.i_IdDireccionCliente = -1;
                txtCodigoCliente.Clear();
                txtRazonCliente.Clear();

            }
        }


        private void txtNumeroDoc_Validated(object sender, EventArgs e)
        {

            Utils.Windows.FijarFormatoUltraTextBox(txtNumeroDoc, "{0:00000000}");
            ComprobarExistenciaCorrelativoDocumento();


        }

        private void cboDocumento_AfterDropDown(object sender, EventArgs e)
        {
            foreach (UltraGridRow row in cboDocumento.Rows)
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
            foreach (UltraGridRow row in cboDocumento.Rows)
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
            if (strModo == "Nuevo")
            {
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
                txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento( cboEstablecimiento.Value !=null && cboEstablecimiento.Value.ToString ()!="-1" ?  int.Parse ( cboEstablecimiento.Value.ToString ()) : Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString()));
                txtNumeroDoc.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(cboEstablecimiento.Value !=null && cboEstablecimiento.Value.ToString ()!="-1" ?  int.Parse ( cboEstablecimiento.Value.ToString ())  :       Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000");
                ComprobarExistenciaCorrelativoDocumento();
            }
            
            _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(cboDocumento.Value ==null ?-1: int.Parse(cboDocumento.Value.ToString()),
                     txtSerieDoc.Text.Trim());


        }

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc, "{0:0000}");
            ComprobarExistenciaCorrelativoDocumento();
        }

        private void chkAfectoIGV_CheckedChanged(object sender, EventArgs e)
        {
            chkPrecInIGV.Enabled = chkAfectoIGV.Checked; if (!chkAfectoIGV.Checked) chkPrecInIGV.Checked = false;

            //CalcularValoresDetalle(); // No necesario ya que se ejecuta al cambiar el Value de cboTipoOperacion
            foreach (var row in grdData.Rows)
                row.Cells["i_RegistroEstado"].Value = "Modificado";

            if (chkAfectoIGV.Checked)
            {
                if (cboTipoOperacion.Value == null || !cboTipoOperacion.Value.ToString().StartsWith("1"))
                    cboTipoOperacion.Value = "1";
            }
            else
                cboTipoOperacion.Value = "3";
        }

        private void chkPrecInIGV_CheckedChanged(object sender, EventArgs e)
        {
            CalcularValoresDetalle();
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
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
                    if (_objPedido.v_IdCliente != "N002-CL000000000") // Solo valida cuando son diferentes de Publico general
                    {

                        if (txtRucCliente.TextLength != 11)
                        {
                            UltraMessageBox.Show("RUC Inválido", "Error de  Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            _objPedido.v_IdCliente = null;
                            _objPedido.i_IdDireccionCliente = -1;
                            txtCodigoCliente.Clear();
                            txtRazonCliente.Clear();
                            txtRucCliente.Focus();
                        }
                        else
                        {

                            if (Utils.Windows.ValidarRuc(txtRucCliente.Text.Trim()) != true)
                            {
                                UltraMessageBox.Show("RUC Inválido", "Error de  Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                _objPedido.v_IdCliente = null;
                                _objPedido.i_IdDireccionCliente = -1;
                                txtCodigoCliente.Clear();
                                txtRazonCliente.Clear();
                                txtRucCliente.Focus();
                            }

                        }
                    }

                    if (_objPedido.v_IdCliente == null)
                    {
                        _objPedido.v_IdCliente = null;
                        _objPedido.i_IdDireccionCliente = -1;
                        txtCodigoCliente.Clear();
                        txtRazonCliente.Clear();
                        txtRucCliente.Focus();

                    }
                }
            }
        }


        #endregion

        #region Validaciones/ Otros
        private void Eliminar()
        {

            foreach (var Fila in grdData.Rows)
            {
                if (Fila.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
                {

                    _objPedidoDetalleDto = new pedidodetalleDto()
                    {
                        v_IdPedidoDetalle = Fila.Cells["v_IdPedidoDetalle"].Value.ToString(),
                        v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value.ToString()
                    };
                    _TempDetalle_EliminarDto.Add(_objPedidoDetalleDto);


                    if (int.Parse(cboDocumento.Value.ToString()) == (int)TiposDocumentos.Pedido)
                    {
                        if (_objPedidoDetalleDto.v_IdProductoDetalle != "N002-PE000000000")
                        {
                            _objSeparacionProducto = new separacionproductoDto()
                            {
                                v_IdPedido = Fila.Cells["v_IdPedido"].Value.ToString(),
                                v_IdProductoAlmacen = Fila.Cells["v_IdProductoAlmacen"].Value.ToString()
                            };

                            _TempDetalle_EliminarDtoSeparacion.Add(_objSeparacionProducto);
                        }
                    }

                    //Fila.Delete(false);
                }

                CalcularValoresDetalle();
            }

        }

        private void ComprobarExistenciaCorrelativoDocumento()
        {
            OperationResult objOperationResult = new OperationResult();
            if (_objPedidoBL.ComprobarExistenciaCorrelativoDocumento(ref objOperationResult, int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text, txtNumeroDoc.Text, _objPedido.v_IdPedido) == true)
            {
                UltraMessageBox.Show("El documento ya ha sido registrado anteriormente", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGuardar.Enabled = false;
            }
            else
            {
                btnGuardar.Enabled = true;
                //btnGuardar.Enabled = _btnGuardar;
            }
        }

        public void BotonesSinEdicion(bool Off)
        {
            cboDocumento.Enabled = Off;
            cboMoneda.Enabled = Off;
            cboVendedor.Enabled = Off;
            cboVendedorRef.Enabled = Off;
            cboCondicionPago.Enabled = Off;
            btnGuardar.Enabled = Off;
            btnEliminar.Enabled = Off;
            cboCondicionPago.Enabled = Off;
            btnAgregar.Enabled = Off;
            groupBox1.Enabled = Off;
           

        }

        public void BotonesSinEdicionPorFactura(bool Off)
        {
            cboDocumento.Enabled = Off;
            cboMoneda.Enabled = Off;
            cboVendedor.Enabled = Off;
            btnGuardar.Enabled = !Off;
            btnEliminar.Enabled = Off;
            cboCondicionPago.Enabled = Off;
            btnAgregar.Enabled = Off;
            grdData.Enabled = Off;
            btnLiberarSeparacion.Enabled = !Off;

        }

        private bool EsVentaAfectaDetraccion()
        {
            CalcularTotales();

            foreach (UltraGridRow Fila in grdData.Rows)
            {
                if (Fila.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000")
                {
                    if (Fila.Cells["i_EsAfectoDetraccion"].Value.ToString() == "1")
                    {
                        //Redodndeo

                        if (txtRucCliente.TextLength == 11 && decimal.Parse(txtPrecioVenta.Text) >= Globals.ClientSession.d_ValorMaximoBoletas)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private string VerificarSaldoProductoAlmacen()
        {
            decimal Saldo, StockActual, SeparacionTotal, SeparacionAnterior, SeparacionActual;
            OperationResult objOperationResult = new OperationResult();
            OperationResult objOperationResultValidarStock = new OperationResult();
            string mensaje = "";
            int ValidarStock;
            List<productoalmacen> ListaProductoAlmacen = _objMovimientoBL.ListaProductoAlmacen(Globals.ClientSession.i_Periodo.ToString());
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                Saldo = 0;
                int ValidarStockAlmacen = _objAlmacenBL.ObtenerAlmacen(ref objOperationResult, int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString())).i_ValidarStockAlmacen ?? 0;
                if (Fila.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal") //Porque esta siendo editado
                {
                    if (Fila.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000" && Fila.Cells["i_EsServicio"].Value.ToString() == "0") // cuando sea diferente de redondeo
                    {
                        productoalmacenDto objProductoAlmacenDto = new productoalmacenDto();
                        if (Fila.Cells["v_IdProductoAlmacen"].Value != null)
                        {

                           // objProductoAlmacenDto = _objPedidoBL.ObtenerProductoAlmacen(ref objOperationResult, int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()), Fila.Cells["v_IdProductoDetalle"].Value.ToString());
                            objProductoAlmacenDto = _objMovimientoBL.ObtenerStockProductoAlmacen(ref objOperationResult, Fila.Cells["v_IdProductoDetalle"].Value.ToString(), int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()), Fila.Cells["v_NroPedido"].Value == null || Fila.Cells["v_NroPedido"].Value == "" ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim(), ListaProductoAlmacen, Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim(), Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim());
                            ValidarStock = _objPedidoBL.ValidarStock(ref objOperationResultValidarStock, Fila.Cells["v_IdProductoDetalle"].Value.ToString());
                            if (objOperationResultValidarStock.Success == 0)
                            {
                                mensaje = string.Format("{0}Error Validar Stock{1}-{2}\n", mensaje, Fila.Cells["v_CodigoInterno"].Value.ToString().Trim(), Fila.Cells["v_NombreProducto"].Value);
                            }
                            StockActual = objProductoAlmacenDto == null ? 0 : objProductoAlmacenDto.d_StockActual == null ? 0 : objProductoAlmacenDto.d_StockActual.Value;
                            SeparacionTotal = objProductoAlmacenDto == null ? 0 : objProductoAlmacenDto.d_SeparacionTotal == null ? 0 : objProductoAlmacenDto.d_SeparacionTotal.Value;
                            SeparacionAnterior = _objPedidoBL.ObtenerCantidadSeparacionProducto(ref objOperationResult, Fila.Cells["v_IdPedido"].Value.ToString(), Fila.Cells["v_IdProductoAlmacen"].Value.ToString());
                            SeparacionActual = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                            Saldo = StockActual - ((SeparacionTotal - SeparacionAnterior) + SeparacionActual);

                            if (ValidarStock == 1 && ValidarStockAlmacen == 1)
                            {
                                if (Saldo < 0 & decimal.Parse(Fila.Cells["i_EsServicio"].Value.ToString()) == 0)
                                {

                                    mensaje = string.Format("{0}{1}-{2}\n", mensaje, Fila.Cells["v_CodigoInterno"].Value.ToString().Trim(), Fila.Cells["v_NombreProducto"].Value);

                                }
                            }
                        }
                    }
                }
                else //temporal -NUEVO
                {
                    if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                    {
                        if (Fila.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000" && Fila.Cells["i_EsServicio"].Value.ToString() == "0") // cuando sea diferente de redondeo
                        {
                            productoalmacenDto objProductoAlmacenDto = new productoalmacenDto();

                            if (Fila.Cells["v_IdProductoAlmacen"].Value != null)
                            {
                               // objProductoAlmacenDto = _objPedidoBL.ObtenerProductoAlmacen(ref objOperationResult, int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()), Fila.Cells["v_IdProductoDetalle"].Value.ToString());
                                objProductoAlmacenDto = _objMovimientoBL.ObtenerStockProductoAlmacen(ref objOperationResult, Fila.Cells["v_IdProductoDetalle"].Value.ToString(), int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()), Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim(), ListaProductoAlmacen, Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim(), Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim());
                                ValidarStock = _objPedidoBL.ValidarStock(ref objOperationResultValidarStock, Fila.Cells["v_IdProductoDetalle"].Value.ToString());
                                if (objOperationResultValidarStock.Success == 0)
                                {
                                    mensaje = mensaje + "Error Validar Stock" + Fila.Cells["v_CodigoInterno"].Value.ToString().Trim() + "-" + Fila.Cells["v_NombreProducto"].Value.ToString() + "\n";
                                }
                                StockActual = objProductoAlmacenDto == null ? 0 : objProductoAlmacenDto.d_StockActual == null ? 0 : objProductoAlmacenDto.d_StockActual.Value;
                                SeparacionTotal = objProductoAlmacenDto == null ? 0 : objProductoAlmacenDto.d_SeparacionTotal == null ? 0 : objProductoAlmacenDto.d_SeparacionTotal.Value;
                                SeparacionActual = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                Saldo = StockActual - (SeparacionTotal + SeparacionActual);
                                if (ValidarStock == 1 && ValidarStockAlmacen == 1)
                                {
                                    if (Saldo < 0 & decimal.Parse(Fila.Cells["i_EsServicio"].Value.ToString()) == 0)
                                    {
                                        mensaje = mensaje + Fila.Cells["v_CodigoInterno"].Value.ToString().Trim() + "-" + Fila.Cells["v_NombreProducto"].Value + "\n";
                                    }
                                }
                            }
                            else
                            {
                                mensaje = mensaje + Fila.Cells["v_CodigoInterno"].Value.ToString().Trim() + "-" + Fila.Cells["v_NombreProducto"].Value.ToString() + "\n";

                            }
                        }
                    }


                }

            }
            return mensaje;


        }

        private string VerificarDescuento()
        {
            string mensaje = "";
            foreach (UltraGridRow fila in grdData.Rows)
            {


                if (fila.Cells["v_Descuento"].Value == null || fila.Cells["v_Descuento"].Value == string.Empty) { fila.Cells["v_Descuento"].Value = "0"; }

                if (fila.Cells["v_IdProductoDetalle"].Value != null)
                {
                    if (fila.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000") // cuando sea diferente de redondeo
                    {
                        var cc = decimal.Parse(fila.Cells["v_Descuento"].Value.ToString());
                        if (fila.Cells["d_DescuentoLP"].Value == null)
                        {
                            if (decimal.Parse(fila.Cells["v_Descuento"].Value.ToString()) > 0)
                            {
                                mensaje = string.Format("{0}{1}-{2}\n", mensaje, fila.Cells["v_CodigoInterno"].Value.ToString().Trim(), fila.Cells["v_NombreProducto"].Value);
                            }

                        }

                        else if (decimal.Parse(fila.Cells["v_Descuento"].Value.ToString()) > decimal.Parse(fila.Cells["d_DescuentoLP"].Value.ToString()))
                        {
                            mensaje = mensaje + fila.Cells["v_CodigoInterno"].Value.ToString().Trim() + "-" + fila.Cells["v_NombreProducto"].Value.ToString() + "\n";

                        }
                    }
                }
            }
            return mensaje;

        }

        private bool ValidaCamposNulosVacios()
        {
            if (grdData.Rows.Where(p => p.Cells["v_IdProductoDetalle"].Value == null || p.Cells["v_IdProductoDetalle"].Value.ToString().Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente todas los productos", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(x => x.Cells["v_IdProductoDetalle"].Value == null || x.Cells["v_IdProductoDetalle"].Value.ToString().Trim() == string.Empty).FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["v_CodigoInterno"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_CodigoInterno"];
                this.grdData.ActiveCell = aCell;
                return false;
            }
            if (Globals.ClientSession.i_PermiteIncluirPreciosCeroPedido == 0 || (Globals.ClientSession.i_PermiteIncluirPreciosCeroPedido ==1 && Globals.ClientSession.UsuarioEsContable==1))
            {
                if ( !Liberacion && grdData.Rows.Where(p => p.Cells["EsRedondeo"].Value == null && decimal.Parse(p.Cells["d_PrecioUnitario"].Value.ToString()) <= 0).Count() != 0)
                {
                    UltraMessageBox.Show("Todos los totales no están calculados", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdData.Rows.Where(x => x.Cells["EsRedondeo"].Value == null && decimal.Parse(x.Cells["d_PrecioUnitario"].Value.ToString()) <= 0).FirstOrDefault();
                    grdData.Selected.Cells.Add(Row.Cells["d_PrecioUnitario"]);
                    grdData.Focus();
                    Row.Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_PrecioUnitario"];
                    this.grdData.ActiveCell = aCell;
                    return false;
                }
            }
            if (Globals.ClientSession.i_PermiteIncluirPreciosCeroPedido == 0)
            {
                if ( !Liberacion && grdData.Rows.Where(p => p.Cells["d_PrecioVenta"].Value == null || decimal.Parse(p.Cells["d_PrecioVenta"].Value.ToString()) <= 0).Count() != 0)
                {
                    UltraMessageBox.Show("Todos los totales no están calculados", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdData.Rows.Where(x => x.Cells["d_PrecioVenta"].Value == null || decimal.Parse(x.Cells["d_PrecioVenta"].Value.ToString()) <= 0).FirstOrDefault();
                    grdData.Selected.Cells.Add(Row.Cells["d_PrecioVenta"]);
                    grdData.Focus();
                    Row.Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_PrecioVenta"];
                    this.grdData.ActiveCell = aCell;
                    return false;
                }
            }

            if ( !Liberacion && grdData.Rows.Where(p => p.Cells["d_Cantidad"].Value == null || decimal.Parse(p.Cells["d_Cantidad"].Value.ToString().Trim()) == 0).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese una cantidad válida", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(p => p.Cells["d_Cantidad"].Value == null || decimal.Parse(p.Cells["d_Cantidad"].Value.ToString().Trim()) <= 0).FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["d_Cantidad"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_Cantidad"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (grdData.Rows.Where(p => p.Cells["i_IdAlmacen"].Value.ToString().Trim() == "-1").Count() != 0)
            {
                UltraMessageBox.Show("Por favor elija un almacén", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(x => x.Cells["i_IdAlmacen"].Value.ToString().Trim() == "-1").FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["i_IdAlmacen"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdAlmacen"];
                this.grdData.ActiveCell = aCell;
                return false;
            }


            var FilasSinLote = grdData.Rows.Where(p => p.Cells["i_SolicitarNroLoteSalida"].Value != null && p.Cells["i_SolicitarNroLoteSalida"].Value.ToString() == "1" && (p.Cells["v_NroLote"].Value == null || p.Cells["v_NroLote"].Value.ToString() == "")).ToList();

            if (FilasSinLote.Any())
            {
                UltraMessageBox.Show("Por favor registre el Nro. de Lote para el producto.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                UltraGridRow Fila = FilasSinLote.FirstOrDefault();
                grdData.Focus();
                Fila.Activate();
                Fila.Cells["v_NroLote"].Activate();
                grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown, true, false);
                return false;
            }


            var FilasConLoteSinSerRequeridas = grdData.Rows.Where(p => p.Cells["i_SolicitarNroLoteSalida"].Value != null && p.Cells["i_SolicitarNroLoteSalida"].Value.ToString() == "0" && (p.Cells["v_NroLote"].Value != null && p.Cells["v_NroLote"].Value != "")).ToList();

            if (FilasConLoteSinSerRequeridas.Any())
            {
                UltraMessageBox.Show("No es necesario registrar el Numero de Lote para este producto", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                UltraGridRow Fila = FilasConLoteSinSerRequeridas.FirstOrDefault();
                grdData.Focus();
                Fila.Activate();
                Fila.Cells["v_NroLote"].Activate();
                grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown, true, false);
                return false;
            }
            var FilasConSerieSinSerRequeridas = grdData.Rows.Where(p => p.Cells["i_SolicitarNroSerieSalida"].Value != null && p.Cells["i_SolicitarNroSerieSalida"].Value.ToString() == "0" && (p.Cells["v_NroSerie"].Value != null && p.Cells["v_NroSerie"].Value != "")).ToList();

            if (FilasConSerieSinSerRequeridas.Any())
            {
                UltraMessageBox.Show("No es necesario registrar el Numero de Serie para este producto", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                UltraGridRow Fila = FilasConSerieSinSerRequeridas.FirstOrDefault();
                grdData.Focus();
                Fila.Activate();
                Fila.Cells["v_NroSerie"].Activate();
                grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown, true, false);
                return false;
            }

            var FilasSinFechaVencimientoLote = grdData.Rows.Where(p => p.Cells["i_SolicitarNroLoteSalida"].Value != null && p.Cells["i_SolicitarNroLoteSalida"].Value.ToString() == "1" && (DateTime.Parse(p.Cells["t_FechaCaducidad"].Value.ToString()) == DateTime.MinValue)).ToList();

            if (FilasSinFechaVencimientoLote.Any())
            {
                UltraMessageBox.Show("Por favor registre la fecha de vencimiento para producto", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                UltraGridRow Fila = FilasSinFechaVencimientoLote.FirstOrDefault();
                grdData.Focus();
                Fila.Activate();
                Fila.Cells["t_FechaCaducidad"].Activate();
                grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown, true, false);
                return false;
            }



            return true;
        }
        private void ValidarFechas()
        {

            if (DateTime.Now.Year.ToString().Trim() == txtPeriodo.Text.Trim())
            {
                if (strModo == "Nuevo")
                {
                    dtpFechaEmision.Value = DateTime.Parse((string.Format("{0}/{1}/{2}", txtPeriodo.Text, txtMes.Text.Trim(), DateTime.Now.Date.Day)).ToString());
                    dtpFechaEmision.MinDate = DateTime.Parse(txtPeriodo.Text + "/01/01");
                    dtpFechaEmision.MaxDate = DateTime.Parse((string.Format("{0}/{1}/{2}", txtPeriodo.Text, txtMes.Text.Trim(), DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim())))).ToString());
                    dtpFechaVencimiento.Value = DateTime.Parse((string.Format("{0}/{1}/{2}", txtPeriodo.Text, txtMes.Text.Trim(), DateTime.Now.Date.Day)).ToString());
                    dtpFechaVencimiento.MinDate = dtpFechaEmision.Value;
                }
                else
                {
                    if (int.Parse(_objPedido.v_Mes.Trim()) <= int.Parse(DateTime.Now.Month.ToString()))
                    {
                        dtpFechaEmision.MaxDate = DateTime.Parse(string.Format("{0}/{1}/{2}", txtPeriodo.Text, DateTime.Now.Month, DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(DateTime.Now.Month.ToString()))));

                    }

                    dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/01/01").ToString());
                    dtpFechaVencimiento.MinDate = dtpFechaEmision.Value;
                }
            }
            else
            {
                if (strModo == "Nuevo")
                {
                    dtpFechaEmision.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/" + "01").ToString());
                    dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                    dtpFechaVencimiento.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Date.Day.ToString()).ToString());
                    dtpFechaVencimiento.MinDate = dtpFechaEmision.Value;
                    dtpFechaVencimiento.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                }
                else
                {

                    dtpFechaEmision.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + "01" + "/01").ToString());
                    dtpFechaEmision.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                    dtpFechaVencimiento.MinDate = dtpFechaEmision.Value;
                    dtpFechaVencimiento.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + " 12 " + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), 12)).ToString()).ToString());
                }

            }
        }

        private void GenerarNumeroRegistro()
        {
            OperationResult objOperationResult = new OperationResult();
            string Mes = int.Parse(dtpFechaEmision.Value.Month.ToString()) <= 9 ? ("0" + dtpFechaEmision.Value.Month).Trim() : dtpFechaEmision.Value.Month.ToString();
            _ListadoPedidosCambioFecha = _objPedidoBL.ObtenerListadoPedidos(ref objOperationResult, txtPeriodo.Text.Trim(), Mes);

            if (_ListadoPedidosCambioFecha.Count != 0)
            {
                int MaxMovimiento = _ListadoPedidosCambioFecha.Count() > 0 ? int.Parse(_ListadoPedidosCambioFecha[_ListadoPedidosCambioFecha.Count() - 1].Value1) : 0;
                MaxMovimiento++;
                txtCorrelativo.Text = MaxMovimiento.ToString("00000000");
                txtMes.Text = int.Parse(dtpFechaEmision.Value.Month.ToString()) <= 9 ? 0 + dtpFechaEmision.Value.Month.ToString() : dtpFechaEmision.Value.Month.ToString();
                txtPeriodo.Text = dtpFechaEmision.Value.Year.ToString();
            }
            else
            {
                txtCorrelativo.Text = "00000001";
                txtMes.Text = int.Parse(dtpFechaEmision.Value.Month.ToString()) <= 9 ? 0 + dtpFechaEmision.Value.Month.ToString() : dtpFechaEmision.Value.Month.ToString();
                txtPeriodo.Text = dtpFechaEmision.Value.Year.ToString();
            }

        }

        public void FijarPrecio(string pstrPrecio, int IdMoneda)
        {
            if (grdData.ActiveRow != null)
            {
                //grdData.ActiveRow.Cells["d_PrecioUnitario"].Value = pstrPrecio;
                grdData.ActiveRow.Cells["d_PrecioUnitario"].Value = cboMoneda.Value == null || decimal.Parse(txtTipoCambio.Text) <= 0 ? "0.0" : IdMoneda == int.Parse(cboMoneda.Value.ToString()) ? pstrPrecio : int.Parse(cboMoneda.Value.ToString()) == (int)Currency.Soles ? (Utils.Windows.DevuelveValorRedondeado(decimal.Parse(pstrPrecio) * decimal.Parse(txtTipoCambio.Text), 2).ToString()) : (Utils.Windows.DevuelveValorRedondeado(decimal.Parse(pstrPrecio) / decimal.Parse(txtTipoCambio.Text), 2).ToString());
                ultraPopupControlContainer1.Close();
            }
        }


        private void cboIGV_ValueChanged(object sender, EventArgs e)
        {


            if (grdData.Rows.Count() == 0)
            {

                txtDescuento.Text = "0.00";
                txtValor.Text = "0.00";
                txtValorVenta.Text = "0.00";
                txtIgv.Text = "0.00";
                txtPrecioVenta.Text = "0.00";
            }

            foreach (UltraGridRow Fila in grdData.Rows)
            {
                CalcularValoresFila(Fila);
                Fila.Cells["i_RegistroEstado"].Value = "Modificado";
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cboAgenciaTransporte_Leave(object sender, EventArgs e)
        {
            if (cboAgenciaTransporte.Value != null)
            {
                if (cboAgenciaTransporte.Text.Trim() == "")
                {
                    cboAgenciaTransporte.Value = "-1";
                }
                else
                {
                    var x = _ListadoTransportistas.Find(p => p.Id == cboAgenciaTransporte.Value.ToString() || p.Id == cboAgenciaTransporte.Text);
                    if (x == null)
                    {
                        cboAgenciaTransporte.Value = "-1";
                    }
                }

            }
        }

        private void cboAgenciaTransporte_Validated(object sender, EventArgs e)
        {
            if (cboAgenciaTransporte.Value == null)
            {
                cboAgenciaTransporte.Value = "-1";
            }
        }

        private void cboAgenciaTransporte_ValueChanged(object sender, EventArgs e)
        {
            if (cboAgenciaTransporte.Value == null)
            {
                return;
            }
            else if (cboAgenciaTransporte.Value.ToString() != "-1" && cboAgenciaTransporte.SelectedItem != null)
            {
                var x = (KeyValueDTO)cboAgenciaTransporte.SelectedItem.ListObject;
                if (x == null) return;
            }

        }

        private void txtDescuento_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDescuento.Text))
            {
                if (decimal.Parse(txtDescuento.Text) > 100)
                {
                    UltraMessageBox.Show("Cantidad Inválida", "Error de Validación", Icono: MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                }
                else
                {
                    CalcularTotales();
                }
            }
            else
            {
                CalcularTotales();
            }
        }

        private void grdData_AfterEnterEditMode(object sender, EventArgs e)
        {
            if (Globals.ClientSession.i_EditarPrecioVentaPedido == 1)
            {
                if (grdData.ActiveCell != null && grdData.ActiveCell.Column.Key.Equals("d_PrecioVenta"))
                {
                    decimal.TryParse(grdData.ActiveCell.Value.ToString(), out valorOriginalCelda);
                }
            }
        }

        private void cboTipoOperacion_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                var c = grdData.DisplayLayout.Bands[0].Columns["i_IdTipoOperacion"];
                if (cboTipoOperacion.Value.ToString() == "5")
                {
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.EditAndSelectText;
                    foreach (var fila in grdData.Rows)
                        fila.Cells["i_RegistroEstado"].Value = "Modificado";
                }
                else
                {
                    if (cboTipoOperacion.Value.ToString().StartsWith("1"))
                    {
                        chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1 ? true : false;
                        chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvVentas == 1 ? true : false;
                    }

                    c.CellActivation = Activation.Disabled;
                    c.CellClickAction = CellClickAction.RowSelect;
                    foreach (var fila in grdData.Rows)
                    {
                        fila.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                        fila.Cells["i_RegistroEstado"].Value = "Modificado";
                        CalcularValoresFila(fila);
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(string.Format("{0}\nLinea: {1}", ex.Message, ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '))));
            }
        }



        #endregion

        #region Imprimir

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {

            var ImpresionVistaPrevia = _objDocumentoBL.ImpresionVistaPrevia(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            var frm = new Reportes.Ventas.frmDocumentoPedido(Globals.ClientSession.i_CurrentExecutionNodeId, _idPedido, cboVendedor.Value.ToString(), ImpresionVistaPrevia);

            if (ImpresionVistaPrevia)
            {
                frm.ShowDialog();
            }
            else
            {
                BtnImprimir.Enabled = false;
            }
            if (strModo != "Cobranza")
            {
                if (UltraMessageBox.Show(" ¿Desea Generar uno Nuevo Pedido?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
                {
                    btnNuevoMovimiento_Click(sender, e);
                }
            }



        }

        #endregion

        private void cboMoneda_ValueChanged(object sender, EventArgs e)
        {
            if (cboMoneda.Value == null) return;
            if (!grdData.Rows.Any()) return;
            var resp =
                MessageBox.Show(
                    string.Format("¿Desea aplicar el cambio [{0}] a los precios del detalle?", txtTipoCambio.Text),
                    @"Sistema",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.No) return;
            AplicarConversion(cboMoneda.Value.ToString(), decimal.Parse(txtTipoCambio.Text), grdData);

        }

        private void AplicarConversion(string idMoneda, decimal tipoCambio, UltraGrid grid)
        {
            try
            {
                foreach (var row in grid.Rows)
                {
                    var precioActual = decimal.Parse(row.Cells["d_PrecioUnitario"].Value.ToString());
                    row.Cells["d_PrecioUnitario"].Value = idMoneda.Equals("1")
                        ? Utils.Windows.DevuelveValorRedondeado(precioActual * tipoCambio, Globals.ClientSession.i_PrecioDecimales.Value)
                        : Utils.Windows.DevuelveValorRedondeado(precioActual / tipoCambio, Globals.ClientSession.i_PrecioDecimales.Value);
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                }

                CalcularValoresDetalle();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBuscarDirecciones_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_objPedido.v_IdCliente))
            {
                frmClienteDirecciones frm = new frmClienteDirecciones(_objPedido.v_IdCliente, true);
                frm.ShowDialog();
                if (!string.IsNullOrEmpty(frm.v_DireccionCliente))
                {
                    _objPedido.i_IdDireccionCliente = frm.i_IdDireccionCliente;
                    txtDireccion.Text = frm.v_DireccionCliente;
                    Zona = frm.Zona;
                    txtGlosa.Text = !string.IsNullOrEmpty(Globals.ClientSession.v_GlosaPedido) ? Globals.ClientSession.v_GlosaPedido : Globals.ClientSession.i_CambiarAlmacenVentasDesdeVendedor == 1 ? cboVendedor.Value == null || cboVendedor.Value.ToString() == "-1" ? Zona : Zona + "-" + cboVendedor.Text : txtGlosa.Text;
                }

            }


        }

        private void cboVendedor_ValueChanged(object sender, EventArgs e)
        {
            txtGlosa.Text = !string.IsNullOrEmpty(Globals.ClientSession.v_GlosaPedido) ? Globals.ClientSession.v_GlosaPedido : Globals.ClientSession.i_CambiarAlmacenVentasDesdeVendedor == 1 ? cboVendedor.Value == null || cboVendedor.Value.ToString() == "-1" ? txtGlosa.Text : Zona + "-" + cboVendedor.Text : txtGlosa.Text;
            if (Globals.ClientSession.i_CambiarAlmacenVentasDesdeVendedor == 1)
            {
                OperationResult pobjOperationResult = new OperationResult();
                var Vendedor = _objVendedorBL.ObtenerVendedor(ref  pobjOperationResult, cboVendedor.Value.ToString());
                cboEstablecimiento.Value = Vendedor !=null && Vendedor.i_IdAlmacen != -1 ? Vendedor.i_IdAlmacen.ToString() : Globals.ClientSession.i_IdEstablecimiento.ToString();
                cboDocumento_Leave( sender, e);
            }
       }

        private void grdData_BeforeRowInsert(object sender, BeforeRowInsertEventArgs e)
        {
            if (Globals.ClientSession.v_RucEmpresa != Constants.RucChayna)
            {
                var filasActuales = grdData.Rows.Count;
                if (filasActuales > _limiteDocumento - 1)
                {
                    e.Cancel = true;
                    UltraMessageBox.Show("Máximo de items por documento alcanzado!");
                    return;

                }
            }

            
        }

        private void btnLiberarSeparacion_Click(object sender, EventArgs e)
        {
            Liberacion = true;
            int NumeroProdLiberados = 0;
            OperationResult objOperationResult = new OperationResult();
            var ProductosFaltantes = new PedidoBL().ReportePedidosFaltantes(ref objOperationResult, dtpFechaEmision.Value.Date, DateTime.Parse("31/12/" + Globals.ClientSession.i_Periodo.ToString() + " 23:59"), (int)EstadoPedido.Pendientes, (int)Currency.Soles, "-1", "",txtSerieDoc.Text.Trim (),txtNumeroDoc.Text.Trim (), "", "-1", (int)FormatoCantidad.UnidadMedidaProducto);
            if (objOperationResult.Success == 1)
            {
                var PedidosKey = ProductosFaltantes.ToDictionary(o => o.v_IdPedidoDetalle, o => o);
                foreach (var Fila in grdData.Rows)
                {
                    ReporteRegistroPedidoPendientes pd = PedidosKey.TryGetValue(Fila.Cells["v_IdPedidoDetalle"].Value.ToString(), out pd) ? pd : new ReporteRegistroPedidoPendientes();
                    if (pd != null && pd.v_IdPedidoDetalle != null)
                    {
                        Fila.Cells["d_Cantidad"].Value = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) - pd.Saldo;
                        Fila.Cells["i_RegistroEstado"].Value = "Modificado";
                        Fila.Cells["d_CantidadEmpaque"].Value = GetCantidadEmpaque((BindingPedidoDetalle)Fila.ListObject);
                        Fila.Cells["i_LiberacionUsuario"].Value = Int32.Parse(Globals.ClientSession.GetAsList()[2]);
                        Fila.Cells["t_FechaLiberacion"].Value = DateTime.Now;
                        CalcularValoresFila(Fila);
                        NumeroProdLiberados++;
                    }
                }

                UltraMessageBox.Show("Liberación de separación terminada correctamente.\n"+string.Format("Se liberaron {0} registros.", NumeroProdLiberados), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al realizar liberación de separación", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



        }





    }
}
