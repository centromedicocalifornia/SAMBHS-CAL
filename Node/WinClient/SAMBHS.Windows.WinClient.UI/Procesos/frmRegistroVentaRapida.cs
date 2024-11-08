﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Venta.BL;
using SAMBHS.Almacen.BL;
using SAMBHS.Compra.BL;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Security.BL;
using SAMBHS.Contabilidad.BL;
using LoadingClass;
using SAMBHS.Windows.WinClient.UI.Mantenimientos.UserControls;
using SAMBHS.Cobranza.BL;


using SAMBHS.Windows.SigesoftIntegration.UI;
using SAMBHS.Windows.WinClient.UI.Sigesoft;
using SAMBHS.Windows.NubefactIntegration;
using SAMBHS.Windows.NubefactIntegration.Modelos;
using NetPdf;
using System.Configuration;
using SAMBHS.Windows.SigesoftIntegration.UI.BLL;
using SAMBHS.Windows.SigesoftIntegration.UI.Reports;
using System.Data.SqlClient;
using SAMBHS.Common.BE.Custom;
using Sigesoft.Node.WinClient.UI.Reports;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmRegistroVentaRapida : Form
    {
        #region Declaraciones / Referencias
        private bool EsDocumentoElectronico
        {
            get
            {
                int i;
                return !int.TryParse(txtSerieDoc.Text, out i);
            }
        }

        SecurityBL _objSecurityBL = new SecurityBL();
        UltraCombo ucUnidadMedida = new UltraCombo();
        UltraCombo ucDestino = new UltraCombo();
        UltraCombo ucAlmacen = new UltraCombo();
        UltraCombo ucTipoOperacion = new UltraCombo();
        UltraCombo ucRubro = new UltraCombo();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        EstablecimientoBL _objEstablecimientoBL = new EstablecimientoBL();
        VentaBL _objVentasBL = new VentaBL();
        ComprasBL _objComprasBL = new ComprasBL();
        VendedorBL _objVendedorBL = new VendedorBL();
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        ventaDto _ventaDto = new ventaDto();
        movimientoDto _movimientoDto = new movimientoDto();
        movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
        ventadetalleDto _ventadetalleDto = new ventadetalleDto();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        List<GridKeyValueDTO> _ListadoComboDocumentosRef = new List<GridKeyValueDTO>();
        List<KeyValueDTO> _ListadoVentas = new List<KeyValueDTO>();
        #endregion
        bool enviandoNubefact = false;
        int _MaxV, _ActV;
        string _Mode, _IdVentaGuardada;
        public string _pstrIdMovimiento_Nuevo;
        string strModo = "Nuevo", strIdVenta;
        public int IdIdentificacion, _idMonedaCobranza;
        bool _EsNotadeCredito = false;
        public bool _Modificado = false;
        string _idVenta, CtaRubroMercaderia = "-1", CtaRubroServicio = "-1";
        decimal Total, Redondeado, Residuo;
        bool _Redondeado = false;
        int ValidarStockAlmacen = 1;
        private decimal valorOriginalCelda;
        private int _limiteDocumento;
        private string _ServiciosSeleccionadosSigesoft;
        public int tipoFact;
        AlmacenBL _objAlmacenBL = new AlmacenBL();
        private List<string> listaServicios = new List<string>();
        private List<string> services = new List<string>();
        private string _nroLiquidacion;
        public BindingList<ventadetalleDto> ListadoVentaDetalle = new BindingList<ventadetalleDto>();
            #region Temporales Detalles de Venta
        List<ventadetalleDto> _TempDetalle_AgregarDto = new List<ventadetalleDto>();
        List<ventadetalleDto> _TempDetalle_ModificarDto = new List<ventadetalleDto>();
        List<ventadetalleDto> _TempDetalle_EliminarDto = new List<ventadetalleDto>();
        List<KeyValueDTO> ListaVendedores = new List<KeyValueDTO>();
        #endregion

        #region Temporales NotaSalidaDetalle
        List<movimientodetalleDto> __TempDetalle_AgregarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> __TempDetalle_ModificarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> __TempDetalle_EliminarDto = new List<movimientodetalleDto>();
        #endregion

        #region PermisosBotones
        bool _btnImprimir;
        bool _btnGuardar;
        bool _btnCobrar;
        #endregion

        public string servicio_;
        public string ticket_;
        public frmRegistroVentaRapida(string Modo, string IdVenta, List<string> servicios)
        {
            strModo = Modo;
            strIdVenta = IdVenta;
            InitializeComponent();
            listaServicios = servicios;
        }

        private void frmRegistroVenta_Load(object sender, EventArgs e)
        {

            OperationResult objOperationResult = new OperationResult();

            using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
            {
                UltraStatusbarManager.Inicializar(ultraStatusBar1);


                IdIdentificacion = 0;

                #region ControlAcciones
                //txtTipoCambio.Text = _objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.DateTime);
                if (new CierreMensualBL().VerificarMesCerrado(Globals.ClientSession.i_Periodo.ToString().Trim(),
                    DateTime.Now.Month.ToString("00").Trim(), (int)ModulosSistema.VentasFacturacion))
                {
                    btnGuardar.Visible = false;
                    this.Text = "Registro de Venta [MES CERRADO]";
                }
                else
                {

                    btnGuardar.Visible = true;
                    this.Text = "Registro de Venta";
                }
                var _formActions = _objSecurityBL.GetFormAction(ref objOperationResult,
                    Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId,
                    "frmRegistroVentaRapida", Globals.ClientSession.i_RoleId);

                _btnImprimir = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmRegistroVentaRapida_PRINT",
                    _formActions);
                _btnGuardar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmRegistroVentaRapida_SAVE",
                    _formActions);
                _btnCobrar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmRegistroVentaRapida_COBRAR",
                    _formActions);
                btnGuardar.Enabled = _btnGuardar;

                BtnImprimir.Visible = Globals.ClientSession.v_ImpresionDirectoVentas == "1" ? false : true;

                #endregion

                this.BackColor = new GlobalFormColors().FormColor;
                panel1.BackColor = new GlobalFormColors().BannerColor;

                #region ConfigurarGrilla

                if (Globals.ClientSession.i_VentasMostrarEmpaque == null || Globals.ClientSession.i_VentasMostrarEmpaque == 0)
                {
                    this.grdData.DisplayLayout.Bands[0].Columns["Empaque"].Hidden = true;
                    this.grdData.DisplayLayout.Bands[0].Columns["UMEmpaque"].Hidden = true;
                }
                var activarUM = Globals.ClientSession.i_CambiarUnidadMedidaVentaPedido == 1 ? true : false;
                grdData.DisplayLayout.Bands[0].Columns["i_IdUnidadMedida"].CellActivation = activarUM ? Activation.AllowEdit : Activation.ActivateOnly;
                grdData.DisplayLayout.Bands[0].Columns["i_IdUnidadMedida"].CellClickAction = activarUM ? CellClickAction.Edit : CellClickAction.RowSelect;
                #endregion

                txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
                txtMes.Text = DateTime.Now.Month.ToString("00");

                #region Cargar Combos
                cboDocumento.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
                _ListadoComboDocumentos = Globals.CacheCombosVentaDto.cboDocumentos;
                _ListadoComboDocumentosRef = Globals.CacheCombosVentaDto.cboDocumentosRef;
                Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", Globals.CacheCombosVentaDto.cboDocumentos, DropDownListAction.Select);

                Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboCondicionPago, "Value1", "Id", Globals.CacheCombosVentaDto.cboCondicionPago, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", Globals.CacheCombosVentaDto.cboEstablecimiento, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboIGV, "Value1", "Id", Globals.CacheCombosVentaDto.cboIGV, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboTipoVenta, "Value1", "Id", Globals.CacheCombosVentaDto.cboTipoVenta, DropDownListAction.Select);
                ListaVendedores = Globals.CacheCombosVentaDto.cboVendedor;
                Utils.Windows.LoadUltraComboEditorList(cboTipoOperacion, "Value1", "Id", Globals.CacheCombosVentaDto.cboTipoOperacion, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", ListaVendedores, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboFormaPago, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 46, null, UserConfig.Default.MostrarSoloFormasPagoAlmacenActual), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboMonedaCobranza, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboList(cboDocumentoRef, "Value2", "Id", Globals.CacheCombosVentaDto.cboDocumentosRef.Where(x => !x.EsDocInterno).ToList(),
                   DropDownListAction.Select);

                cboFormaPago.Value = "-1";
                cboMonedaCobranza.Value = "-1";
                cboDocumentoRef.Value = "-1";
                cboTipoServicio.Value = -1;
                CargarCombosDetalle();
                #endregion

                ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);
                dtpFechaRegistro.MaxDate =
                    DateTime.Parse(
                        (txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" +
                         (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim())))));
                FormatoDecimalesGrilla((int)Globals.ClientSession.i_CantidadDecimales,
                    (int)Globals.ClientSession.i_PrecioDecimales);


            }

            cboDocumento.Value = UserConfig.Default.csTipoDocumentoVentaRapida;
            cboDocumento.Enabled = UserConfig.Default.appPermiteIntercambiarDocumentosVR;
            // ClientePublicoGeneral();

            if (cboDocumento.Value.ToString() == "1")
            {
                btnBuscarDetraccion.Focus();
            }
            else
            {
                ClientePublicoGeneral();
            }
            txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).Trim();
            // se agrego para que las cajas usen diferentetes series 
            if (cboDocumento.Value != null && cboDocumento.Value.ToString() != "-1" && !string.IsNullOrEmpty(UserConfig.Default.SerieCaja) && UserConfig.Default.SerieCaja != "--Seleccionar--")
            {
                txtSerieDoc.Text = UserConfig.Default.SerieCaja;
            }
            CancelEventArgs e1 = new CancelEventArgs();
            txtSerieDoc_Validating(sender, e1);

            PredeterminarEstablecimiento(txtSerieDoc.Text);
            if (cboDocumento.Value != null && txtSerieDoc.Text.Trim() != string.Empty)
                _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());

            LlenarCuentasRubros();
            chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1;
            chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvVentas == 1;
            #region txtRucCLiente
            txtRucCliente.LoadConfig("C");
            txtRucCliente.ItemSelectedAfterDropClosed += delegate
            {
                txtRucCliente_KeyDown(null, new KeyEventArgs(Keys.Enter));
            };
            #endregion
            int systemUserId = Globals.ClientSession.i_SystemUserId;
            if (systemUserId == 2036)
            {
                btnAgendar.Enabled = false;
            }

            cboDocumento.Focus();
            chkImpresionDirecta.Checked = UserConfig.Default.ImpresioDirectaVentaRapida;
            //cboCondicionPago.Value = Globals.ClientSession.v_RucEmpresa == Constants.RucDetec ? "2" : "1";

            cboCondicionPago.Value = Globals.ClientSession.i_IdCondicionPagoVenta == -1 ? 1 : Globals.ClientSession.i_IdCondicionPagoVenta;
            

            btnAgregar_Click(sender, e);

            if (ListadoVentaDetalle.Count > 0)
            {
                grdData.DataSource = ListadoVentaDetalle;
                for (var i = 0; i < grdData.Rows.Count(); i++)
                {
                    grdData.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
                    grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
                    grdData.Rows[i].Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                }
            }
        }

        #region Eventos Generales
        private void txtRucCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (!txtRucCliente.IsDroppedDown && e.KeyCode == Keys.Enter)
            {
                if (txtRucCliente.Text.Trim() != "" && txtRucCliente.TextLength <= 5)
                {
                    Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtRucCliente.Text);
                    frm.ShowDialog();
                    if (frm._IdCliente != null)
                    {
                        _ventaDto.v_IdCliente = frm._IdCliente;
                        _ventaDto.i_IdDireccionCliente = frm._IdDireccionCliente;

                        txtRazonSocial.Text = frm._RazonSocial;
                        txtRucCliente.Text = frm._NroDocumento;
                        VerificarLineaCreditoCliente(_ventaDto.v_IdCliente);
                    }
                }
                else if (txtRucCliente.TextLength == 11 | txtRucCliente.TextLength == 8)
                {
                    if (cboDocumento.Value.ToString() == "1" && txtRucCliente.TextLength == 8)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema");
                        _ventaDto.v_IdCliente = null;
                        _ventaDto.i_IdDireccionCliente = -1;
                        txtRazonSocial.Clear();
                        txtDireccion.Clear();
                        return;
                    }
                    if (txtRucCliente.TextLength == 11 && Utils.Windows.ValidarRuc(txtRucCliente.Text) == false)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema");
                        _ventaDto.v_IdCliente = null;
                        _ventaDto.i_IdDireccionCliente = -1;
                        txtRazonSocial.Clear();
                        txtDireccion.Clear();
                        return;
                    }
                    OperationResult objOperationResult = new OperationResult();
                    string[] DatosProveedor = new string[4];
                    DatosProveedor = _objVentasBL.DevolverClientePorNroDocumento(ref objOperationResult, txtRucCliente.Text.Trim());
                    if (DatosProveedor != null)
                    {
                        _ventaDto.v_IdCliente = DatosProveedor[0];
                        _ventaDto.i_IdDireccionCliente = int.Parse(DatosProveedor[6]);
                        txtRazonSocial.Text = DatosProveedor[2];
                        txtDireccion.Text = DatosProveedor[3];
                        VerificarLineaCreditoCliente(_ventaDto.v_IdCliente);
                    }
                    else
                    {
                        Mantenimientos.frmRegistroRapidoCliente frm = new Mantenimientos.frmRegistroRapidoCliente(txtRucCliente.Text.Trim(), "C");
                        frm.ShowDialog();
                        if (frm._Guardado == true)
                        {
                            txtRucCliente.Text = frm._NroDocumentoReturn;
                            DatosProveedor = _objVentasBL.DevolverClientePorNroDocumento(ref objOperationResult, txtRucCliente.Text.Trim());
                            if (DatosProveedor != null)
                            {
                                _ventaDto.v_IdCliente = DatosProveedor[0];
                                _ventaDto.i_IdDireccionCliente = int.Parse(DatosProveedor[6]);
                                txtRazonSocial.Text = DatosProveedor[2];
                                txtDireccion.Text = DatosProveedor[3];
                                VerificarLineaCreditoCliente(_ventaDto.v_IdCliente);
                            }
                        }
                    }
                    txtRazonSocial.Enabled = false;
                    txtRucCliente.Enabled = true;
                    txtDireccion.Enabled = false;
                }
                else
                {
                    txtRazonSocial.Text = string.Empty;
                    txtDireccion.Clear();
                }
            }

            if (e.KeyCode == Keys.Escape)
            {
                //if (txtRucCliente.CanUndo())
                //{
                //    txtRucCliente.Undo();
                //}
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (!grdData.Rows.Any(p => p.Cells["v_IdProductoDetalle"].Value == null || p.Cells["v_IdProductoDetalle"].Value.ToString() == "-1" && p.Cells["v_NroCuenta"].Value == null))
            {
                if (decimal.Parse(txtTipoCambio.Text.Trim()) <= 0)
                {
                    UltraMessageBox.Show("Por favor ingrese un tipo de cambio válido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtTipoCambio.Focus();
                    return;
                }

                if (cboIGV.Value.ToString() == "-1")
                {
                    UltraMessageBox.Show("Por favor seleccione un valor para el IGV", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    cboIGV.Focus();
                    return;
                }

                if (grdData.Rows.Count(p => p.Cells["v_IdProductoDetalle"].Value != null && p.Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000") != 0)
                {
                    UltraMessageBox.Show("Antes de continuar por favor elimine el redondeo", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                OperationResult objOperationResult = new OperationResult();
                if (grdData.ActiveRow != null)
                {
                    if (grdData.ActiveRow.Cells["v_NroCuenta"].Value != null)
                    {
                        UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                        if (row == null) return;
                        grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                        grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                        row.Cells["i_RegistroEstado"].Value = "Agregado";
                        row.Cells["i_RegistroTipo"].Value = "Temporal";
                        row.Cells["i_IdUnidadMedida"].Value = "-1";
                        row.Cells["d_Cantidad"].Value = "0";
                        row.Cells["d_Precio"].Value = "0";
                        row.Cells["i_IdUnidadMedida"].Value = "-1";
                        row.Cells["i_Anticipio"].Value = "0";
                        row.Cells["d_Descuento"].Value = "0";
                        row.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null && cboTipoOperacion.Value.ToString() != "5" ? cboTipoOperacion.Value.ToString() : "1";
                        row.Cells["i_IdCentroCosto"].Value = "0";
                        row.Cells["d_Percepcion"].Value = "0";
                        row.Cells["d_Valor"].Value = "0";
                        row.Cells["v_NroCuenta"].Value = "-1";
                        row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                        row.Cells["i_IdCentroCosto"].Value = _objVentasBL.CentroCostoDeEstablecimiento(ref objOperationResult);
                    }
                }
                else
                {
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    if (row == null) return;
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Agregado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["d_Cantidad"].Value = "0";
                    row.Cells["d_Precio"].Value = "0";
                    row.Cells["i_IdUnidadMedida"].Value = "-1";
                    row.Cells["i_Anticipio"].Value = "0";
                    row.Cells["d_Descuento"].Value = "0";
                    row.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null && cboTipoOperacion.Value.ToString() != "5" ? cboTipoOperacion.Value.ToString() : "1";
                    row.Cells["i_IdCentroCosto"].Value = "0";
                    row.Cells["d_Valor"].Value = "0";
                    row.Cells["v_NroCuenta"].Value = "-1";
                    row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    row.Cells["i_IdCentroCosto"].Value = _objVentasBL.CentroCostoDeEstablecimiento(ref objOperationResult);
                }
            }
            EnterEditMode();
        }

        private void EnterEditMode()
        {
            var ultimaFila = grdData.Rows.LastOrDefault();
            if (ultimaFila != null && !cboDocumento.Focused)
            {
                grdData.Focus();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                grdData.ActiveCell = ultimaFila.Cells["v_CodigoInterno"];
                grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                if (!grdData.ActiveCell.IsInEditMode)
                {
                    grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (cboTipoServicio.Value.ToString() == "6")
            {
                if (txtCodigo.TextLength < 14 || txtCodigo.TextLength > 14)
                {
                    UltraMessageBox.Show("Debe igresar correctamente el N° de Liquidación", "Advertencia");
                    return;
                }
                string v_NroLiquidacion = txtCodigo.Text;
                var liquidacion = BuscarLiquidacion(v_NroLiquidacion);

                if (liquidacion.Count() == null || liquidacion.Count() == 0)
                {
                    UltraMessageBox.Show("No Existe Liquidación en la Base de Datos", "Advertencia");
                    return;
                }
            }

            if (cboTipoServicio.Value.ToString() == "-1")
            {
                 UltraMessageBox.Show("Debe Seleccionar un Tipo de Servicio", "Advertencia");
                 return;
            }
            //if (uvDatos.Validate(true, true).IsValid)
            //{
                if (string.IsNullOrWhiteSpace(txtCorrelativo.Text))
                {
                    UltraMessageBox.Show("No se obtuvo el Nro. de registro de la venta de forma correcta", "Advertencia");
                    return;
                }

                if (grdData.Rows.Any(x => x.Cells["v_NroCuenta"].Value == null) || grdData.Rows.Any(x => x.Cells["v_NroCuenta"].Value.ToString() == "-1"))
                {
                    UltraMessageBox.Show("Porfavor ingrese correctamente los Rubros", "Advertencia");
                    grdData.Rows.First(x => x.Cells["v_NroCuenta"].Value == null || x.Cells["v_NroCuenta"].Value.ToString() == "-1").Selected = true;
                    return;
                }
                string ubicacion;
                if (_objVentasBL.TieneCobranzasRealizadas(_ventaDto.v_IdVenta, out ubicacion))
                {
                    UltraMessageBox.Show("Imposible Guardar Cambios a un Documento con Cobranzas Realizadas \r" + ubicacion, "Advertencia");
                    return;
                }

                if ((!chkCobranzaMixta.Checked && chkCobranzaMixta.Enabled) && (cboFormaPago.Value == null || cboFormaPago.Value.ToString() == "-1"))
                {
                    MessageBox.Show("Seleccione la forma de pago del documento primero.!", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    cboFormaPago.Focus();
                    return;
                }

                if (EsVentaAfectaDetraccion() &&
                    UltraMessageBox.Show("El documento es Afecto Detracción, ¿Desea continuar?", "Advertencia",
                        MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }

                if (txtNroOperacion.Enabled && string.IsNullOrEmpty(txtNroOperacion.Text.Trim()))
                {
                    MessageBox.Show(@"Por favor ingrese el número de operación para la cobranza con tarjeta.", @"Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtNroOperacion.Focus();
                    return;
                }

                #region Validaciones Generales

                if (decimal.Parse(txtTipoCambio.Text.Trim()) <= 0)
                {
                    UltraMessageBox.Show("Por favor ingrese un tipo de cambio válido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (grdData.Rows.Count(p => p.Cells["v_IdProductoDetalle"].Value == null) != 0)
                {
                    UltraMessageBox.Show("Uno de los artículos esta incorrectamente ingresado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    grdData.Rows.First(p => p.Cells["v_IdProductoDetalle"].Value == null).Selected = true;
                    return;
                }


                if (cboDocumento.Value != null && (cboDocumento.Value.ToString() == "7" || cboDocumento.Value == "1") && _ventaDto.v_IdCliente == "N002-CL000000000")
                {

                    UltraMessageBox.Show("Este tipo documento no puede estar asociado a Público general", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!grdData.Rows.Any() || !grdData.Rows.Any(p => p.Cells["v_IdProductoDetalle"].Value != null && p.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000"))
                {
                    UltraMessageBox.Show("No se permite guardar mientras el detalle está vacío", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var id = (string)cboDocumento.Value;
                if ((id.Equals("7") || id.Equals("8")) && (ucTipoNota.Value == null || (string)ucTipoNota.Value == "-1"))
                {
                    UltraMessageBox.Show("Seleccione un Tipo de Nota");
                    ucTipoNota.Focus();
                    return;
                }


                #region Revision de la serie de la NCR y NDB
                if (EsDocumentoElectronico && cboDocumento.Value != null && cboDocumentoRef.Value != null && !cboDocumentoRef.Value.ToString().Equals("-1") &&
                    (cboDocumento.Value.ToString().Equals("7") || cboDocumento.Value.ToString().Equals("8")))
                {
                    var docRef = cboDocumentoRef.Value.ToString();

                    if ((docRef.Equals("1") && !txtSerieDoc.Text.StartsWith("F")) || (docRef.Equals("3") && !txtSerieDoc.Text.StartsWith("B")))
                    {
                        MessageBox.Show("Por favor revise coloque correctamente la serie del documento", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txtSerieDoc.Focus();
                        return;
                    }
                }
                #endregion

                

                if (ValidaCamposNulosVacios())
                {
                    foreach (UltraGridRow Fila in grdData.Rows)
                    {
                        Fila.Cells["i_ValidarStock"].Value = Fila.Cells["i_ValidarStock"].Value ?? "0";

                        if (Fila.Cells["v_NroCuenta"].Value == null)
                        {
                            UltraMessageBox.Show("Por favor ingrese correctamente todas las cuentas al detalle.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        if (txtSerieDoc.Text != "TFM" && txtSerieDoc.Text != "RSL" && txtSerieDoc.Text != "THM")
                        {
                            if (Fila.Cells["EsRedondeo"].Value == null && decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString()) <= 0)
                            {
                                UltraMessageBox.Show("Todos los Totales no están calculados.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Fila.Activate();
                                return;
                            }
                        }

                        if (Fila.Cells["i_IdAlmacen"].Value == null && Fila.Cells["v_IdProductoDetalle"].Value != null)
                        {
                            UltraMessageBox.Show("Porfavor especifique los Almacenes correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    #region ValidarStock
                    if (!_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString()))
                                        && cboDocumento.Value.ToString() != "8")
                    {
                        var detalles = grdData.Rows.Select(r => (ventadetalleDto)r.ListObject).ToList();
                        var resultado = new VentaBL().VerificarStockDisponible(ref objOperationResult, detalles);
                        if (objOperationResult.Success == 1)
                        {
                            if (!resultado)
                            {
                                MessageBox.Show(
                                    @"Los siguientes productos no cuentan con el Stock disponible:" + '\n' +
                                    objOperationResult.AdditionalInformation, @"Validación del Sistema",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show(objOperationResult.ErrorMessage, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    #endregion

                    if (Residuo != 0 && !grdData.Rows.Any() && grdData.Rows.Count(p => p.Cells["v_IdProductoDetalle"].Value != null && p.Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000") != 0)
                    {
                        UltraMessageBox.Show("Por favor elimine el anterior redondeo para continuar.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (Residuo != 0 && Globals.ClientSession.i_RedondearVentas == 1)
                    {
                        btnRedondear_Click(sender, e);
                    }
                }
                else
                {
                    return;
                }

                CalcularValoresDetalle();

                if (!VerificarLineaCreditoCliente(_ventaDto.v_IdCliente))
                {
                    return;
                }
                #endregion

                using (new PleaseWait(Location, "Por favor espere..."))
                {
                    if (_Mode == "New")
                    {
                        while (_objVentasBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text) == false)
                        {
                            txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                        }

                        while (_objVentasBL.ExisteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text, txtCorrelativoDocIni.Text) == false)
                        {
                            txtCorrelativoDocIni.Text = (int.Parse(txtCorrelativoDocIni.Text) + 1).ToString("00000000");
                        }
                        int i;
                        #region Guarda Entidad Venta
                        _ventaDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                        _ventaDto.v_Mes = int.Parse(txtMes.Text.Trim()).ToString("00");
                        _ventaDto.v_Periodo = txtPeriodo.Text.Trim();
                        _ventaDto.v_Correlativo = txtCorrelativo.Text;
                        _ventaDto.d_Anticipio = txtAnticipio.Text != string.Empty ? decimal.Parse(txtAnticipio.Text) : 0;
                        _ventaDto.d_IGV = txtIGV.Text != string.Empty ? decimal.Parse(txtIGV.Text) : 0;
                        _ventaDto.d_TipoCambio = txtTipoCambio.Text != string.Empty ? decimal.Parse(txtTipoCambio.Text) : 0;
                        _ventaDto.d_Total = txtTotal.Text != string.Empty ? decimal.Parse(txtTotal.Text) : 0;
                        _ventaDto.d_Valor = txtValor.Text != string.Empty ? decimal.Parse(txtValor.Text) : 0;
                        _ventaDto.d_ValorVenta = txtValorVenta.Text != string.Empty ? decimal.Parse(txtValorVenta.Text) : 0;
                        _ventaDto.i_DeduccionAnticipio = chkDeduccionAnticipio.Checked == true ? 1 : 0;
                        _ventaDto.i_EsAfectoIgv = chkAfectoIGV.Checked == true ? 1 : 0;
                        _ventaDto.t_FechaVencimiento = dtpFechaRef.Value;
                        _ventaDto.t_FechaRegistro = dtpFechaRegistro.DateTime;
                        if (chkFecha.Checked == true)
                        {
                            _ventaDto.t_FechaRef = dtpFechaRegistro.DateTime;
                            _ventaDto.t_FechaOrdenCompra = dtpFechaRegistro.DateTime;
                            _ventaDto.t_InsertaFecha = dtpFechaRegistro.DateTime;
                        }
                        else
                        {
                            _ventaDto.t_FechaRef = DateTime.Today;
                            _ventaDto.t_FechaOrdenCompra = DateTime.Today;
                            _ventaDto.t_InsertaFecha = DateTime.Now;
                        }
                        _ventaDto.i_IdCondicionPago = int.Parse(cboCondicionPago.Value.ToString());
                        _ventaDto.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());
                        _ventaDto.i_IdEstado = 1;
                        _ventaDto.i_IdIgv = int.Parse(cboIGV.Value.ToString()); //evaluar
                        _ventaDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                        _ventaDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                        _ventaDto.i_IdTipoDocumentoRef = -1;
                        _ventaDto.i_PreciosIncluyenIgv = chkPrecInIGV.Checked == true ? 1 : 0;
                        _ventaDto.v_CorrelativoDocumentoRef = string.Empty;
                        _ventaDto.v_Mes = txtMes.Text;
                        _ventaDto.v_Periodo = txtPeriodo.Text;
                        _ventaDto.v_SerieDocumento = txtSerieDoc.Text;
                        _ventaDto.v_CorrelativoDocumento = txtCorrelativoDocIni.Text.Trim();
                        _ventaDto.v_CorrelativoDocumentoFin = string.Empty;
                        _ventaDto.v_Concepto = txtConcepto.Text.Trim();
                        _ventaDto.v_SerieDocumentoRef = string.Empty;
                        _ventaDto.d_PorcDescuento = 0;
                        _ventaDto.d_PocComision = 0;
                        _ventaDto.d_Descuento = txtDescuento.Text == string.Empty ? 0 : decimal.Parse(txtDescuento.Text);
                        _ventaDto.v_BultoDimensiones = string.Empty;
                        _ventaDto.v_NroGuiaRemisionCorrelativo = string.Empty;
                        _ventaDto.v_NroGuiaRemisionSerie = string.Empty;
                        _ventaDto.d_IGV = txtIGV.Text == string.Empty ? 0 : decimal.Parse(txtIGV.Text.Trim());
                        _ventaDto.v_Marca = string.Empty;
                        _ventaDto.v_NroBulto = string.Empty;
                        _ventaDto.i_NroDias = 0;
                        _ventaDto.v_NroPedido = string.Empty;
                        _ventaDto.v_OrdenCompra = string.Empty;
                        _ventaDto.d_PesoBrutoKG = 0;
                        _ventaDto.d_PesoNetoKG = 0;
                        
                        _ventaDto.i_IdMedioPagoVenta = -1;
                        _ventaDto.i_IdPuntoDestino = -1;
                        _ventaDto.i_IdPuntoEmbarque = -1;
                        _ventaDto.i_IdTipoEmbarque = -1;
                        _ventaDto.i_IdTipoDocumentoRef = int.TryParse(cboDocumentoRef.Value.ToString(), out i) ? i : -1;
                        _ventaDto.v_SerieDocumentoRef = txtSerieDocRef.Text;
                        _ventaDto.v_CorrelativoDocumentoRef = txtCorrelativoDocRef.Text;
                        _ventaDto.i_IdTipoOperacion = int.Parse(cboTipoOperacion.Value.ToString());
                        _ventaDto.i_IdTipoNota = int.Parse((string)ucTipoNota.Value ?? "-1");
                        _ventaDto.i_IdTipoVenta = 3; //int.Parse(cboTipoVenta.Value.ToString());
                        _ventaDto.i_DrawBack = 0;
                        _ventaDto.v_IdVendedor = cboVendedor.Value.ToString();
                        _ventaDto.v_IdVendedorRef = "-1";
                        _ventaDto.v_NombreClienteTemporal = _ventaDto.v_IdCliente == "N002-CL000000000" ? txtRazonSocial.Text : string.Empty;
                        //_ventaDto.v_DireccionClienteTemporal = _ventaDto.v_IdCliente == "N002-CL000000000" ? txtDireccion.Text : string.Empty;
                        _ventaDto.v_DireccionClienteTemporal = txtDireccion.Text;
                        _ventaDto.NombreCliente = txtRazonSocial.Text;
                        _ventaDto.i_FacturacionCliente = rbAseguradora.Checked ? 0 : 1;
                        _ventaDto.v_SigesoftServiceId = _ServiciosSeleccionadosSigesoft;
                        _ventaDto.i_ClienteEsAgente = int.Parse(cboTipoServicio.Value.ToString());
                        LlenarTemporalesVenta();
                        _ventaDto.v_IdVenta = _IdVentaGuardada = _objVentasBL.InsertarVenta(ref objOperationResult, _ventaDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto);
                        #endregion

                        ActualizarFacturacionServicio(tipoFact, listaServicios);
                        if (servicio_!= null || servicio_ == "")
                        {
                            ActualizarFacturacionReceta(txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text.Trim(), servicio_);
                        }
                        if (ticket_ != null || ticket_ == "")
                        {
                            ActualizarTicketHosp(txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text.Trim(), ticket_);
                        }
                    }
                }

                if (objOperationResult.Success == 1)
                {
                    AgendaBl oAgendaBl = new AgendaBl();
                    #region Grabar es Sigesoft i_IsFac

                    if (servicios != null)
                    {
                        foreach (var item in servicios)
                        {
                            var serviceId = item.v_ServiceId;
                            oAgendaBl.UpdateIsFact(serviceId, 2);

                            oAgendaBl.GenerarLiquidacion(serviceId, item.EmpresaFacturacion, decimal.Parse(txtTotal.Text), _ventaDto.t_FechaVencimiento.Value.Date, _ventaDto.v_SerieDocumento + "-" + _ventaDto.v_CorrelativoDocumento);
                        }
                    }
                    else
                    {
                        //Actualizar Tabla Liquidacion Sigesoft
                        oAgendaBl.UpdateLiquidacion(_nroLiquidacion, _ventaDto.v_SerieDocumento +"-"+ _ventaDto.v_CorrelativoDocumento, _ventaDto.t_FechaVencimiento);
                    }

                    
                  

                    #endregion

                    #region Grabar Pago de Hopitalización en Sigesoft

                    if (rbHospitalizacion.Checked)
                    {
                       var hopitalizacionId =   grdData.Rows[0].Cells["v_HopitalizacionId"].Value.ToString();
                       var aCargo = grdData.Rows[0].Cells["ACargo"].Value.ToString();
                       oAgendaBl.UpdatePagoHospitalizacion(hopitalizacionId, aCargo, 1);
                    }

                    #endregion

                    _Modificado = true;
                    EdicionBarraNavegacion(true);
                    strIdVenta = _ventaDto.v_IdVenta;
                    btnGuardar.Enabled = false;
                    BtnImprimir.Enabled = _btnImprimir;
                    ///BtnImprimir.Enabled = true;
                    btnCobrar.Enabled = _btnCobrar;
                    _pstrIdMovimiento_Nuevo = _ventaDto.v_IdVenta;
                    _idVenta = _pstrIdMovimiento_Nuevo;
                    cboDocumento.Enabled = false;
                    txtSerieDoc.Enabled = false;
                    UltraStatusbarManager.Mensaje(ultraStatusBar1, "Venta guardada correctamente!", timer1);
                    if (cboCondicionPago.Text.Contains("CONTADO") && !chkCobranzaMixta.Checked)
                    {
                        new CobranzaBL().RealizaCobranzaAlContado(ref objOperationResult,
                            int.Parse(cboMonedaCobranza.Value.ToString()), int.Parse(cboFormaPago.Value.ToString()),
                            txtNroOperacion.Text, decimal.Parse(txtMontoCobrar.Text), _IdVentaGuardada,
                            dtpFechaRegistro.DateTime, decimal.Parse(txtTipoCambio.Text));

                        if (objOperationResult.Success == 0)
                        {
                            UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    if (UserConfig.Default.ImpresioDirectaVentaRapida != chkImpresionDirecta.Checked)
                    {
                        UserConfig.Default.ImpresioDirectaVentaRapida = chkImpresionDirecta.Checked;
                        UserConfig.Default.Save();
                    }

                    btnNuevoMovimiento.Enabled = true;

                    #region Fact Electronica

                    if (UserConfig.Default.NubefactEnviarAlGuardar)
                    {
                        enviandoNubefact = true;
                        var nubefactM = new NubeFacTManager
                        {
                            EnviarAutomaticamente = UserConfig.Default.NubefactAutoEnvio,
                            FormatoImpresion = UserConfig.Default.NubefactFormato,
                            Ruta = UserConfig.Default.NubefactRuta,
                            Token = UserConfig.Default.NubefactToken,
                            TipoAccionRealizar = NubeFacTManager.TipoAccion.EnviarComprobante
                        };

                        nubefactM.EstadoEvent += delegate(string msg, int rowIndex)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                UltraStatusbarManager.Mensaje(ultraStatusBar1, msg, timer1);
                            });
                        };

                        nubefactM.ErrorEvent += delegate(Exception ex, int rowIndex, SAMBHS.Windows.NubefactIntegration.NubeFacTManager.TipoAccion action)
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                UltraStatusbarManager.MarcarError(ultraStatusBar1, ex.Message, timer1);
                                enviandoNubefact = false;
                            });
                        };

                        nubefactM.TerminadoEvent += delegate(IRespuesta rpt, int rowIndex, EstadoSunat estadoSunat)
                        {
                            var error = rpt as RespuestaError;
                            enviandoNubefact = false;
                            if (error != null)
                                UltraStatusbarManager.MarcarError(ultraStatusBar1, error.Errors, timer1);
                            else
                                UltraStatusbarManager.Mensaje(ultraStatusBar1, estadoSunat.ToString(), timer1);
                        };

                        nubefactM.Comenzar(_ventaDto.v_IdVenta);
                    }
                    #endregion
                }
                else
                {
                    if (objOperationResult.ErrorMessage == null)
                    {
                        UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                _TempDetalle_AgregarDto = new List<ventadetalleDto>();
                _TempDetalle_ModificarDto = new List<ventadetalleDto>();
                _TempDetalle_EliminarDto = new List<ventadetalleDto>();
            //}
            //else
            //{
            //    UltraMessageBox.Show("Por favor llene los campos requeridos antes de guardar", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //}
                if (listaServicios.Count !=0 )
                {
                    #region GUARDAR COMPROBANTE Y ESTADO DE VENTA
                    #region Conexion SAM
                    ConexionSigesoft conectasam = new ConexionSigesoft();
                    conectasam.opensigesoft();
                    #endregion
                    if (listaServicios.Count == 1)
                    {
                        foreach (var item in listaServicios)
                        {
                            var cadena1 = "update service set i_IsFac = 2, v_ComprobantePago = CONCAT('" + txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " | ', v_ComprobantePago) where v_ServiceId='" + item + "'";
                            SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                            SqlDataReader lector = comando.ExecuteReader();
                            lector.Close();
                        }
                    }
                    else
                    {
                        var query = listaServicios.GroupBy(x => x).Where(g => g.Count() >= 1).ToDictionary(x => x.Key, y => y.Count());
                        foreach (var item in query)
                        {
                            var cadena1 = "update service set i_IsFac = 2, v_ComprobantePago = CONCAT('" + txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " | ', v_ComprobantePago) where v_ServiceId='" + item.Key + "'";
                            SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
                            SqlDataReader lector = comando.ExecuteReader();
                            lector.Close();
                        }
                    }
                    
                    #endregion
                }
                
                
                if (cboTipoServicio.Value.ToString() == "6")
                {
                    //if (txtCodigo.TextLength < 14 || txtCodigo.TextLength > 14)
                    //{
                    //    UltraMessageBox.Show("Verifique el núnero de liquidación no es correcto.", "Error de validación.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //    return;
                    //}

                    string FechavVencimiento = dtpFechaRef.Value.ToString();
                    string monto = txtTotal.Text;
                    string v_NroFactura = _ventaDto.v_SerieDocumento + "-" + _ventaDto.v_CorrelativoDocumento;
                    string v_NroLiquidacion = txtCodigo.Text;
                    UpdateLiquidacion(v_NroFactura, v_NroLiquidacion, FechavVencimiento, monto);
                }

                BtnImprimir.Enabled = true;
        }

        private void UpdateLiquidacion(string v_NroFactura, string v_NroLiquidacion, string fechaV, string monto)
        {
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            #endregion
            var cadena1 = "update liquidacion set v_NroFactura='" + v_NroFactura + "' , d_FechaVencimiento = '" + fechaV + "' , d_Monto = '" + monto + "' where v_NroLiquidacion='" + v_NroLiquidacion + "'";
            SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
            SqlDataReader lector = comando.ExecuteReader();
            lector.Close();
            conectasam.closesigesoft();
        }

        private string BuscarLiquidacion(string v_NroLiquidacion)
        {
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            var cadena1 = "select v_NroLiquidacion from liquidacion where v_NroLiquidacion = '" + v_NroLiquidacion + "'";
            SqlCommand comando = new SqlCommand(cadena1, connection: conectasam.conectarsigesoft);
            SqlDataReader lector = comando.ExecuteReader();
            string nroliq = "";
            while (lector.Read())
            {
                nroliq = lector.GetValue(0).ToString();
            }
            lector.Close();
            conectasam.closesigesoft();

            return nroliq;
        }

        private void ActualizarFacturacionServicio(int tipoFact, List<string> servicios)
        {
            FacturacionServiciosBl.ActualizarFacturacionServicio(tipoFact, servicios);
        }
        private void ActualizarFacturacionReceta(string comprobante, string servicios)
        {
            FacturacionServiciosBl.ActualizarFacturacionReceta(comprobante, servicios);
        }
        private void ActualizarTicketHosp(string comprobante, string Ticket)
        {
            FacturacionServiciosBl.ActualizarTicketHospi(comprobante, Ticket);
        }
        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnBuscarDetraccion_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtRucCliente.Text.Trim());
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {
                if (frm._IdCliente != "N002-CL000000000")
                {
                    if (cboDocumento.Value.ToString().Equals("1") && !Utils.Windows.ValidarRuc(frm._NroDocumento))
                    {
                        UltraMessageBox.Show("En Factura no se puede elegir este cliente.", "Error de validación.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    _ventaDto.v_IdCliente = frm._IdCliente;
                    _ventaDto.i_IdDireccionCliente = frm._IdDireccionCliente;
                    txtRazonSocial.Text = frm._RazonSocial;
                    txtRucCliente.Text = frm._NroDocumento;
                    txtDireccion.Text = frm._Direccion;
                    txtRazonSocial.Enabled = false;
                    txtRucCliente.Enabled = true;
                    txtDireccion.Enabled = false;
                    VerificarLineaCreditoCliente(_ventaDto.v_IdCliente);
                    IdIdentificacion = frm._TipoDocumento;
                    //if (cboDocumento.Value != null && !_objDocumentoBL.DocumentoEsContable(int.Parse(cboDocumento.Value.ToString())))
                    //{
                    //    txtRazonSocial.Enabled = true;
                    //}
                }
                else
                {
                    UltraMessageBox.Show("En Factura no se puede elegir este cliente.", "Error de validación.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

            }


        }

        private void chkAfectoIGV_CheckedChanged(object sender, EventArgs e)
        {
            chkPrecInIGV.Enabled = chkAfectoIGV.Checked;
            CalcularValoresDetalle();
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
            }
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

        private void chkDeduccionAnticipio_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDeduccionAnticipio.Checked)
            {
                UltraGridColumn c = grdData.DisplayLayout.Bands[0].Columns["i_Anticipio"];
                c.CellActivation = Activation.AllowEdit;
                c.CellClickAction = CellClickAction.Edit;
            }
            else
            {
                for (int i = 0; i < grdData.Rows.Count(); i++)
                {
                    grdData.Rows[i].Cells["i_Anticipio"].Value = 0;
                    grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
                }
                UltraGridColumn c = grdData.DisplayLayout.Bands[0].Columns["i_Anticipio"];
                c.CellActivation = Activation.NoEdit;
                c.CellClickAction = CellClickAction.CellSelect;
                CalcularValoresDetalle();
            }
        }

        private void frmRegistroVenta_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!enviandoNubefact)
            {
                if (btnCobrar.Enabled && btnCobrar.Visible)
                {
                    var resp = MessageBox.Show("¿Seguro de Salir sin Cobrar la venta?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    e.Cancel = resp == DialogResult.No;
                }
            }
            else
                e.Cancel = enviandoNubefact;
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
                    if (strModo == "Nuevo")
                    {
                        ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);
                    }
                    else if (strModo == "Guardado")
                    {
                        strModo = "Consulta";
                        ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);
                    }
                    else
                    {
                        ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);
                    }
                }
                else
                {
                    UltraMessageBox.Show("Mes inválido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/01").ToString());
            dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/01").ToString()); ;
            dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim()))).ToString()).ToString());
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
                        if (strModo == "Nuevo")
                        {
                            ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);
                        }
                        else if (strModo == "Guardado")
                        {
                            strModo = "Consulta";
                            ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);
                        }
                        else
                        {
                            ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);
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

        private void txtCorrelativo_Leave(object sender, EventArgs e)
        {
            txtCorrelativo.Text = txtCorrelativo.Text == "" ? "" : int.Parse(txtCorrelativo.Text).ToString("00000000");
            if (txtCorrelativo.Text != "")
            {
                var x = _ListadoVentas.Find(p => p.Value1 == txtCorrelativo.Text);
                if (x != null)
                {
                    CargarCabecera(x.Value2);
                }
                else
                {
                    UltraMessageBox.Show("No se encontró la compra", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);
                }
            }
        }

        private void txtCorrelativo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtCorrelativo.Text = txtCorrelativo.Text == "" ? "" : int.Parse(txtCorrelativo.Text).ToString("00000000");
                if (txtCorrelativo.Text != "")
                {
                    var x = _ListadoVentas.Find(p => p.Value1 == txtCorrelativo.Text);
                    if (x != null)
                    {
                        CargarCabecera(x.Value2);
                    }
                    else
                    {
                        UltraMessageBox.Show("No se encontró la compra", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);
                    }
                }
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_ListadoVentas.Any())
            {
                if (_MaxV == 0) CargarCabecera(_ListadoVentas[0].Value2);

                if (_ActV > 0 && _ActV <= _MaxV)
                {
                    _ActV = _ActV - 1;
                    txtCorrelativo.Text = _ListadoVentas[_ActV].Value1;
                    CargarCabecera(_ListadoVentas[_ActV].Value2);
                }
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_ActV >= 0 && _ActV < _MaxV)
            {
                _ActV = _ActV + 1;
                txtCorrelativo.Text = _ListadoVentas[_ActV].Value1;
                CargarCabecera(_ListadoVentas[_ActV].Value2);
            }
        }

        private void btnNuevoMovimiento_Click(object sender, EventArgs e)
        {
            try
            {
                _ventaDto = new ventaDto();
                CargarDetalle("");
                LimpiarCabecera();
                //cboDocumento.Value = UserConfig.Default.csTipoDocumentoVentaRapida;
                //cboDocumento_Leave(sender, e);
                OperationResult objOperationResult = new OperationResult();
                if (cboDocumento.Value != null && !string.IsNullOrEmpty(UserConfig.Default.SerieCaja) && UserConfig.Default.SerieCaja != "--Seleccionar--")
                {
                    txtSerieDoc.Text = UserConfig.Default.SerieCaja;
                }
                CancelEventArgs e1 = new CancelEventArgs();
                txtCorrelativoDocIni.Text = _objDocumentoBL.CorrelativoxSerie(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text);
                txtSerieDoc_Validating(sender, e1);
                _Mode = "New";
                EdicionBarraNavegacion(false);
                txtTipoCambio.Text = _objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.DateTime);
                btnAgregar_Click(sender, e);
                btnGuardar.Enabled = true;
                BtnImprimir.Enabled = false;
                //BtnImprimir.Enabled = true;
                groupBox3.Enabled = true;
                btnAgregar.Enabled = true;
                btnEliminar.Enabled = true;
                txtRucCliente.Enabled = true;
                cboDocumento.Enabled = true;
                txtSerieDoc.Enabled = true;
                cboCondicionPago.Value = Globals.CacheCombosVentaDto.cboCondicionPago.Any(p => p.Value1.Contains("CONTADO"))
                    ? Globals.CacheCombosVentaDto.cboCondicionPago.FirstOrDefault(p => p.Value1.Contains("CONTADO")).Id
                    : "-1";

                if (cboDocumento.Value.ToString() != "7" && cboDocumento.Value.ToString() != "1" && cboDocumento.Value.ToString() != "8")
                {
                    ClientePublicoGeneral();
                }
                PredeterminarEstablecimiento(txtSerieDoc.Text);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void cboTipoOperacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                UltraGridColumn c = grdData.DisplayLayout.Bands[0].Columns["i_IdTipoOperacion"];
                if (cboTipoOperacion.Value.ToString() == "5")
                {
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.CellSelect;
                }
                else
                {
                    c.CellActivation = Activation.Disabled;
                    c.CellClickAction = CellClickAction.RowSelect;
                    foreach (UltraGridRow Fila in grdData.Rows)
                    {
                        Fila.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                        CalcularValoresFila(Fila);
                    }
                }

                //if (cboTipoOperacion.Value.ToString() != "2")
                //{
                //    UltraGridColumn d = grdData.DisplayLayout.Bands[0].Columns["d_Igv"];
                //    d.CellActivation = Activation.AllowEdit;
                //    d.CellClickAction = CellClickAction.CellSelect;
                //}
                //else
                //{
                //    UltraGridColumn d = grdData.DisplayLayout.Bands[0].Columns["d_Igv"];
                //    d.CellActivation = Activation.Disabled;
                //    d.CellClickAction = CellClickAction.RowSelect;
                //}
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _ventadetalleDto = new ventadetalleDto();
                    _ventadetalleDto.v_IdVentaDetalle = grdData.ActiveRow.Cells["v_IdVentaDetalle"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(_ventadetalleDto);

                    if (grdData.ActiveRow.Cells["v_IdMovimientoDetalle"].Value != null)
                    {
                        _movimientodetalleDto = new movimientodetalleDto();
                        _movimientodetalleDto.v_IdMovimientoDetalle = grdData.ActiveRow.Cells["v_IdMovimientoDetalle"].Value.ToString();
                        _movimientodetalleDto.i_IdAlmacen = grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString();
                        __TempDetalle_EliminarDto.Add(_movimientodetalleDto);
                    }

                    grdData.ActiveRow.Delete(false);
                }
            }
            else
            {
                grdData.ActiveRow.Delete(false);
            }
            CalcularValoresDetalle();
        }

        private void cboIGV_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcularValoresDetalle();
        }

        private void txtRucCliente_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRucCliente.Text.Trim()) && _ventaDto.v_IdCliente != "N002-CL000000000")
            {
                if (cboDocumento.Value.ToString() == "1")
                {
                    if (txtRucCliente.TextLength != 11)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _ventaDto.v_IdCliente = null;
                        _ventaDto.i_IdDireccionCliente = -1;
                        txtRazonSocial.Clear();
                        txtRucCliente.Focus();
                        txtDireccion.Clear();
                    }
                    else
                    {
                        if (Utils.Windows.ValidarRuc(txtRucCliente.Text.Trim()) != true)
                        {
                            UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _ventaDto.v_IdCliente = null;
                            _ventaDto.i_IdDireccionCliente = -1;
                            txtRazonSocial.Clear();
                            txtRucCliente.Focus();
                            txtDireccion.Clear();
                        }
                    }
                }
            }
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            #region Comentado
            //var establecimientoDetalle = _objDocumentoBL.ConfiguracionEstablecimiento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            //if (establecimientoDetalle == null)
            //{
            //    UltraMessageBox.Show(string.Format("Documento {0} {1} no se encuentra asignado en Establecimiento ", cboDocumento.Text, txtSerieDoc.Text), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;

            //}

            //if (cboFormaPago.Text.Equals("Mixto", StringComparison.OrdinalIgnoreCase))
            //{
            //    return;
            //}
            //var impresionVistaPrevia = _objDocumentoBL.ImpresionVistaPrevia(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            //var idDoc = int.Parse(cboDocumento.Value.ToString());
            //var idVentas = new List<string> { _IdVentaGuardada };
            //if (idDoc == (int)DocumentType.Factura)
            //{
            //    Reportes.Ventas.frmDocumentoFactura frm = new Reportes.Ventas.frmDocumentoFactura(idVentas, impresionVistaPrevia);

            //    if (impresionVistaPrevia)
            //    {
            //        frm.ShowDialog();
            //        frm.Activate();
            //    }

            //}
            //if (idDoc == (int)DocumentType.Boleta)
            //{
            //    Reportes.Ventas.frmDocumentoBoleta frm = new Reportes.Ventas.frmDocumentoBoleta(idVentas, impresionVistaPrevia);

            //    if (impresionVistaPrevia)
            //    {
            //        frm.ShowDialog();
            //        frm.Activate();
            //    }

            //}
            //if (idDoc == (int)DocumentType.NotaCredito)
            //{
            //    if (Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita)
            //    {
            //        var tk = new Reportes.Ventas.Ablimatex.TicketNcr(idVentas);
            //        tk.Print();
            //    }
            //    else
            //        using (var frm = new Reportes.Ventas.frmDocumentoNotaCredito(idVentas, impresionVistaPrevia))
            //            if (impresionVistaPrevia)
            //            {
            //                frm.ShowDialog();
            //                frm.Activate();
            //            }
            //}
            //if (idDoc == (int)DocumentType.TicketBoleta) // 12 : TCK FAC o BOL
            //{
            //    //Reportes.Ventas.Ablimatex.frmDocumentoTicketSinRuc frm = new Reportes.Ventas.Ablimatex.frmDocumentoTicketSinRuc(idVentas, IdIdentificacion, impresionVistaPrevia);
            //    var pr = new Reportes.Ventas.Ablimatex.Ticket(idVentas);
            //    pr.Print();
            //    //frm.ShowDialog();
            //}




            //if (int.Parse(cboDocumento.Value.ToString()) == (int)TiposDocumentos.GuiaInterna)
            //{

            //    Reportes.Ventas.frmDocumentoGuiaOtroFormato frm = new Reportes.Ventas.frmDocumentoGuiaOtroFormato(idVentas, impresionVistaPrevia, true);
            //    if (impresionVistaPrevia)
            //    {
            //        frm.ShowDialog();
            //        frm.Activate();
            //    }


            //}

            //if (int.Parse(cboDocumento.Value.ToString()) == (int)TiposDocumentos.Proforma)
            //{

            //    using (Reportes.Ventas.frmDocumentoProforma frm = new Reportes.Ventas.frmDocumentoProforma(idVentas, impresionVistaPrevia))
            //    {
            //        if (impresionVistaPrevia)
            //        {
            //            frm.ShowDialog();
            //            frm.Activate();

            //        }
            //        else
            //        {
            //            BtnImprimir.Enabled = false;

            //        }
            //    }


            //}

            //BtnImprimir.Enabled = false;

            //if (MessageBox.Show(@"¿Desea continuar con una venta nueva?", @"Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //{
            //    btnNuevoMovimiento_Click(sender, e);
            //} 
            #endregion

            var idDoc = int.Parse(cboDocumento.Value.ToString());

            var objOperationResult = new OperationResult();

            if (idDoc == (int)DocumentType.EGRESO_CAJA_OCUPACIONAL)
            {
                BtnImprimir.Enabled = true;
                using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                {
                    this.Enabled = false;
                    var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                    var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                    if (datosP.Count() >= 1)
                    {
                        string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                        string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                        Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                    }
                    else
                    {
                        var msj = string.Format("Por favor registre un Egreso Ocupacional para Imprimir comprobante.");
                        MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    
                    this.Enabled = true;
                }
            }
            else if (idDoc == (int)DocumentType.EGRESO_CAJA_MTC)
            {
                BtnImprimir.Enabled = true;
                using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                {
                    this.Enabled = false;
                    var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                    var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                    if (datosP.Count() >= 1)
                    {
                        string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                        string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                        Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                    }
                    else
                    {
                        var msj = string.Format("Por favor registre un Egreso DE MTC para Imprimir comprobante.");
                        MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    this.Enabled = true;
                }
            }
            else if (idDoc == (int)DocumentType.EGRESO_CAJA_GINECOLOGIA)
            {
                BtnImprimir.Enabled = true;
                using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                {
                    this.Enabled = false;
                    var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                    var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                    if (datosP.Count() >= 1)
                    {
                        string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                        string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                        Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                    }
                    else
                    {
                        var msj = string.Format("Por favor registre un Egreso GINECOLOGICO para Imprimir comprobante.");
                        MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    this.Enabled = true;
                }
            }
            else if (idDoc == (int)DocumentType.EGRESO_CAJA_REHABILITACION)
            {
                BtnImprimir.Enabled = true;
                using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
                {
                    this.Enabled = false;
                    var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                    var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                    if (datosP.Count() >= 1)
                    {
                        string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                        string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                        Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                    }
                    else
                    {
                        var msj = string.Format("Por favor registre un Egreso REHABILITACION para Imprimir comprobante.");
                        MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    this.Enabled = true;
                }
            }
            else if (idDoc == (int)DocumentType.EGRESO_CAJA_ASISTENCIAL)
            {
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                if (datosP.Count() >= 1)
                {
                    string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                    string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                    Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                }
                else
                {
                    var msj = string.Format("Por favor registre un Egreso Asistencial para Imprimir comprobante.");
                    MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Enabled = true;
            }
            else if (idDoc == (int)DocumentType.INGRESO_CAJA_OCUPACIONAL)
            {
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                if (datosP.Count() >= 1)
                {
                    string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                    string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                    Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                }
                else
                {
                    var msj = string.Format("Por favor registre un Ingreso Ocupacional para Imprimir comprobante.");
                    MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Enabled = true;
            }
            else if (idDoc == (int)DocumentType.INGRESO_CAJA_ASISTENCIAL)
            {
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                if (datosP.Count() >= 1)
                {
                    string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                    string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                    Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                }
                else
                {
                    var msj = string.Format("Por favor registre un Ingreso Asistencial para Imprimir comprobante.");
                    MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Enabled = true;
            }
            else if (idDoc == (int)DocumentType.EGRESO_CAJA_FARMACIA)
            {
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                if (datosP.Count() >= 1)
                {
                    string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                    string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                    Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                }
                else
                {
                    var msj = string.Format("Por favor registre un Ingreso Asistencial para Imprimir comprobante.");
                    MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Enabled = true;
            }
            else if (idDoc == (int)DocumentType.INGRESO_CAJA_FARMACIA)
            {
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                if (datosP.Count() >= 1)
                {
                    string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                    string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                    Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                }
                else
                {
                    var msj = string.Format("Por favor registre un Ingreso Asistencial para Imprimir comprobante.");
                    MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Enabled = true;
            }
            else if (idDoc == (int)DocumentType.RECIBO_SAN_LORENZO)
            {
                if (txtRucCliente.Text == "00000000000")
                {
                    MessageBox.Show("Debe registrar al cliente con su documento.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new VentaBL().GetReciboSanLorenzo(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);
                string nroRecibo = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text;
                if (datosP.Count() >= 1)
                {
                    string serie_correlativo = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + "|" + cboMoneda.Text;
                    string ruta = GetApplicationConfigValue("rutaEgresos").ToString() + nroRecibo + ".pdf";
                    string calendar = listaServicios.Count() == 0 ? "CC" : ObtenerCalendar(listaServicios[0].ToString());
                    string person = listaServicios.Count() == 0 ? "PP" : ObtenerPersonId(listaServicios[0].ToString());
                    DateTime fechaNacimiento = listaServicios.Count() == 0 ? DateTime.Now : ObtenerFechaNac(person);
                    //Recibo_San_Lorenzo.CreateRecibo_San_Lorenzo(ruta + nombre + ".pdf", MedicalCenter, datosP);
                    string service = GetHistoryCLinic(person);//listaServicios.Count() == 0 ? "SS" : listaServicios[0].ToString();
                    string DatosPaciente = ObtenerDatosPaciente(txtRucCliente.Text);
                    DatosPaciente = DatosPaciente + "|" + service;
                    Recibo_Interno2.CreateRecibo(ruta, DatosPaciente, calendar, fechaNacimiento, serie_correlativo, datosP);

                }
                else
                {
                    var msj = string.Format("Por favor registre un Recibo para Imprimir comprobante.");
                    MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Enabled = true;
                //var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                //var datosP = new VentaBL().GetReciboSanLorenzo(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                //if (datosP.Count() >= 1)
                //{
                //    string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                //    string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                //    Recibo_San_Lorenzo.CreateRecibo_San_Lorenzo(ruta + nombre + ".pdf", MedicalCenter, datosP);
                //}
                //else
                //{
                //    var msj = string.Format("Por favor registre un Recibo para Imprimir comprobante.");
                //    MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //}
                //this.Enabled = true;
            }
            else if (idDoc == (int)DocumentType.TICKET_MEDICINAS)
            {
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new VentaBL().TicketReceta(txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text);
                
                if (datosP.Count() >= 1)
                {
                    string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                    string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                    TicketMedicina.CreateTicket_Medicina(ruta + nombre + ".pdf", MedicalCenter, datosP);
                }
                else
                {
                    var msj = string.Format("Por favor registre un Ticket para Imprimir comprobante.");
                    MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Enabled = true;
            }
            else if (idDoc == (int)DocumentType.TICKET_HOSPIT)
            {
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new VentaBL().TicketHospi(txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text);

                if (datosP.Count() >= 1)
                {
                    string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                    string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                    TicketHospit.CreateTicket_Hospi(ruta + nombre + ".pdf", MedicalCenter, datosP);
                }
                else
                {
                    var msj = string.Format("Por favor registre un Ticket para Imprimir comprobante.");
                    MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Enabled = true;
            }
            var link = VentaHelper.GetPdf(_ventaDto.v_IdVenta, VentaHelper.TipoConstancia.ENVIO, VentaHelper.TipoRepresentacion.PDF);
            if (!string.IsNullOrEmpty(link))
                System.Diagnostics.Process.Start(link); 
        }

        private string ObtenerCalendar(string servicioId)
        {
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            string fecha = DateTime.Now.ToShortDateString();
            var cadena = "select cc.v_CalendarId, su.v_UserName from calendar cc " +
                         "inner join service ss on cc.v_ServiceId=ss.v_ServiceId " +
                         "inner join servicecomponent sc on ss.v_ServiceId=sc.v_ServiceId " +
                         "inner join systemuser su on sc.i_MedicoTratanteId=su.i_SystemUserId " +
                         "where sc.v_ComponentId='N009-ME000001143' and ss.v_ServiceId='" + servicioId + "'";
            var comando = new SqlCommand(cadena, connection: conectasam.conectarsigesoft);
            var lector = comando.ExecuteReader();
            string v_CalendarId = "";
            while (lector.Read())
            {
                v_CalendarId = lector.GetValue(0).ToString() + '|' + lector.GetValue(1).ToString();
            }
            lector.Close();
            conectasam.closesigesoft();
            return v_CalendarId;
        }

        private string GetHistoryCLinic(string person)
        {
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            var cadena = "select v_nroHistoria from Historyclinics where v_PersonId = '" + person + "' ";
            var comando = new SqlCommand(cadena, connection: conectasam.conectarsigesoft);
            var lector = comando.ExecuteReader();
            string v_nroHistoria = "";
            while (lector.Read())
            {
                v_nroHistoria = lector.GetValue(0).ToString();
            }
            lector.Close();
            conectasam.closesigesoft();
            return v_nroHistoria == "" ? "---" : v_nroHistoria;
        }

        private string ObtenerPersonId(string servicioId)
        {
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            var cadena = "select v_PersonId from calendar where v_ServiceId = '" + servicioId + "' ";
            var comando = new SqlCommand(cadena, connection: conectasam.conectarsigesoft);
            var lector = comando.ExecuteReader();
            string v_PersonId = "";
            while (lector.Read())
            {
                v_PersonId = lector.GetValue(0).ToString();
            }
            lector.Close();
            conectasam.closesigesoft();
            return v_PersonId;
        }

        private DateTime ObtenerFechaNac(string person)
        {
            ConexionSigesoft conectasam = new ConexionSigesoft();
            conectasam.opensigesoft();
            string fecha = DateTime.Now.ToShortDateString();
            var cadena = "select d_Birthdate from person where v_PersonId = '" + person + "'";
            var comando = new SqlCommand(cadena, connection: conectasam.conectarsigesoft);
            var lector = comando.ExecuteReader();
            DateTime d_Birthdate = default(DateTime);
            while (lector.Read())
            {
                d_Birthdate = (DateTime)lector.GetValue(0);
            }
            lector.Close();
            conectasam.closesigesoft();
            return d_Birthdate;
        }

        private string ObtenerDatosPaciente(string dni)
        {
            ConexionSAM2 conectasam = new ConexionSAM2();
            conectasam.opensam();
            string fecha = DateTime.Now.ToShortDateString();
            var cadena = "select v_PrimerNombre + ' ' +	v_SegundoNombre + ' ' +	v_ApePaterno + ' ' +	v_ApeMaterno + ' ' +	v_RazonSocial as nombre, v_DirecPrincipal as direc from cliente where v_CodCliente='" + dni + "'";
            var comando = new SqlCommand(cadena, connection: conectasam.conectarsam);
            var lector = comando.ExecuteReader();
            string datos = dni;
            while (lector.Read())
            {
                datos = datos + "|" + lector.GetValue(0).ToString();
                datos = datos + "|" + lector.GetValue(1).ToString();
            }
            lector.Close();
            conectasam.closesam();
            return datos;
        }

        public static string GetApplicationConfigValue(string nombre)
        {
            return Convert.ToString(ConfigurationManager.AppSettings[nombre]);
        }

        //public void UpdateIsFact(string serviceId, int? value)
        //{
        //    using (var cnx = ConnectionHelper.GetNewSigesoftConnection)
        //    {
        //        if (cnx.State != System.Data.ConnectionState.Open) cnx.Open();

        //        var actualizar = "UPDATE service SET " +
        //                         "d_UpdateDate = GETDATE(), " +
        //                         "i_IsFac = " + value +
        //                         "WHERE v_ServiceId = '" + serviceId + "'";
        //        cnx.Execute(actualizar);

        //    }
        //}
        private void btnRedondear_Click(object sender, EventArgs e)
        {
            Redondeo();
        }

        private void cboMoneda_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cboMoneda.Value.ToString())
            {
                case "1":
                    cboTipoVenta.Value = "03";
                    break;

                case "2":
                    cboTipoVenta.Value = "04";
                    break;
            }
            cboMonedaCobranza.Value = cboMoneda.Value;

            if (Constants.RucDetec == Globals.ClientSession.v_RucEmpresa || !grdData.Rows.Any()) return;
            var resp = MessageBox.Show(string.Format("¿Desea aplicar el cambio [{0}] a los precios del detalle?", txtTipoCambio.Text), @"Sistema",
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
                    var precioActual = decimal.Parse(row.Cells["d_Precio"].Value.ToString());
                    row.Cells["d_Precio"].Value = idMoneda.Equals("1")
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

        

        #region Comportamiento de Controles

        private void txtSerieDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc, e);
        }

        private void txtCorrelativoDocIni_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDocIni, e);
        }

        private void txtCorrelativoDocIni_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDocIni, "{0:00000000}");
            //ComprobarExistenciaCorrelativoDocumento();
        }

        private void txtTipoCambio_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtTipoCambio, e);
        }

        private void txtSerieDoc_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSerieDoc.Text))
            {
                int i;
                if (int.TryParse(txtSerieDoc.Text, out i))
                    Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc, "{0:0000}");
                //ComprobarExistenciaCorrelativoDocumento();
                string Correlativo = _objDocumentoBL.CorrelativoxSerie(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text);
                if (Correlativo != null)
                {
                    txtCorrelativoDocIni.Text = Correlativo;
                }
                else
                {
                    UltraMessageBox.Show("No se encuentra registrado la serie en la configuración del Establecimiento", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    if (txtSerieDoc.CanUndo())
                    {
                        txtSerieDoc.Undo();
                        txtSerieDoc.ClearUndo();
                    }
                    txtSerieDoc.Focus();
                    return;
                }
            }
        }

        private void cboDocumento_AfterDropDown(object sender, EventArgs e)
        {
            //foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento.Rows)
            //{
            //    if (cboDocumento.Value == "-1") cboDocumento.Text = string.Empty;
            //    bool filterRow = true;
            //    foreach (UltraGridColumn column in cboDocumento.DisplayLayout.Bands[0].Columns)
            //    {
            //        if (column.IsVisibleInLayout)
            //        {
            //            if (row.Cells[column].Text.Contains(cboDocumento.Text.ToUpper()))
            //            {
            //                filterRow = false;
            //                break;
            //            }
            //        }
            //    }
            //    if (filterRow)
            //    {
            //        row.Hidden = true;
            //    }
            //    else
            //    {
            //        row.Hidden = false;
            //    }

            //}
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
                row.Hidden = filterRow;
            }
        }

        private void cboDocumento_Leave(object sender, EventArgs e)
        {
            if (cboDocumento.Value != null)
            {
                if (strModo == "Nuevo")
                {
                    if (cboDocumento.Text.Trim() == "")
                    {
                        cboDocumento.Value = "-1";
                    }
                    else
                    {
                        var x = _ListadoComboDocumentos.Any(p => p.Id == cboDocumento.Value.ToString() || p.Id == cboDocumento.Text);
                        if (!x)
                        {
                            cboDocumento.Value = "-1";
                        }
                    }
                    if (cboDocumento.Value != null && txtSerieDoc.Text.Trim() != string.Empty)
                        _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());

                    if (cboDocumento.Value.ToString() != "7" && cboDocumento.Value.ToString() != "1" && cboDocumento.Value.ToString() != "8")
                    {
                        ClientePublicoGeneral();
                    }
                    else if (_ventaDto.v_IdCliente == "N002-CL000000000")
                    {
                        _ventaDto.v_IdCliente = string.Empty;
                        _ventaDto.i_IdDireccionCliente = -1;
                        txtRucCliente.Clear();
                        txtRazonSocial.Clear();
                        txtDireccion.Clear();
                    }
                    txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).Trim();
                    // se agrego para que cada caja utiliza una serie 
                    if (cboDocumento.Value != null && cboDocumento.Value.ToString() != "-1" && !string.IsNullOrEmpty(UserConfig.Default.SerieCaja) && UserConfig.Default.SerieCaja != "--Seleccionar--")
                    {
                        txtSerieDoc.Text = UserConfig.Default.SerieCaja;
                    }

                    //txtCorrelativoDocIni.Text = _objDocumentoBL.CorrelativoxSerie(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text);
                    CancelEventArgs e1 = new CancelEventArgs();
                    txtSerieDoc_Validating(sender, e1);
                    //
                    //txtCorrelativoDocIni.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000").Trim();
                    PredeterminarEstablecimiento(txtSerieDoc.Text);
                    if (!_objDocumentoBL.DocumentoEsContable(int.Parse(cboDocumento.Value.ToString()))) //Validación agregada por GUI
                    {
                        chkPrecInIGV.Checked = false;
                        chkAfectoIGV.Checked = false;

                    }
                    else
                    {
                        chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1;
                        chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvVentas == 1;
                    }
                    var id = cboDocumento.Value;
                    if (id.Equals("7") || id.Equals("8"))
                    {
                        var objOperationResult = new OperationResult();
                        Utils.Windows.LoadUltraComboEditorList(ucTipoNota, "Value1", "Id",
                            _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult,
                                id.Equals("7") ? 156 : 157, null), DropDownListAction.Select);
                        ucTipoNota.Value = "7";
                        ucTipoNota.ValueList.DropDownResizeHandleStyle = DropDownResizeHandleStyle.DiagonalResize;
                        ucTipoNota.Visible = lblTipoNota.Visible = true;
                    }
                    else
                        ucTipoNota.Visible = lblTipoNota.Visible = false;
                }

                if (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) ||
               cboDocumento.Value.ToString() == "8")
                {
                    cboDocumentoRef.Enabled = true;
                    txtSerieDocRef.Enabled = true;
                    txtCorrelativoDocRef.Enabled = true;
                    dtpFechaRef.Enabled = true;
                    chkDeduccionAnticipio.Enabled = false;
                }
                else
                {
                    cboDocumentoRef.Value = "-1";
                    cboDocumentoRef.Enabled = false;
                    txtSerieDocRef.Enabled = false;
                    txtSerieDocRef.Clear();
                    txtCorrelativoDocRef.Clear();
                    txtCorrelativoDocRef.Enabled = false;
                    dtpFechaRef.Enabled = true;
                    chkDeduccionAnticipio.Enabled = true;
                }

                //_EsNotadeCredito = cboDocumento.Value.ToString().Trim() == "7";
                _EsNotadeCredito = _objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString().Trim()));
                if (_EsNotadeCredito)
                {
                    UltraGridColumn c = grdData.DisplayLayout.Bands[0].Columns["i_IdAlmacen"];
                    c.CellActivation = Activation.ActivateOnly;
                    c.CellClickAction = CellClickAction.CellSelect;
                }
                else
                {
                    UltraGridColumn c = grdData.DisplayLayout.Bands[0].Columns["i_IdAlmacen"];
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                }

                ComprobarExistenciaCorrelativoDocumento();

            }
            else
            {
                cboDocumento.Value = "-1";
            }
        }

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            try
            {
                PredeterminarEstablecimiento(txtSerieDoc.Text);
                _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtSerieDoc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (txtSerieDoc.CanUndo())
                {
                    txtSerieDoc.Undo();
                }
            }
        }

        private void txtCorrelativoDocIni_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (txtCorrelativoDocIni.CanUndo())
                {
                    txtCorrelativoDocIni.Undo();
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

        #endregion

        #region Clases/Validaciones
        private void ObtenerListadoVentas(string pstrPeriodo, string pstrMes)
        {
            var objOperationResult = new OperationResult();
            _ListadoVentas = _objVentasBL.ObtenerListadoVentas(ref objOperationResult, pstrPeriodo, pstrMes);
            switch (strModo)
            {
                case "Edicion":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdVenta);
                    _idVenta = strIdVenta;
                    cboDocumento.Enabled = false;
                    txtSerieDoc.Enabled = false;
                    break;

                case "Nuevo":
                    if (_ListadoVentas.Count != 0)
                    {
                        _MaxV = _ListadoVentas.Count - 1;
                        _ActV = _MaxV;
                        LimpiarCabecera();
                        CargarDetalle("");
                        _Mode = "New";
                        _ventaDto = new ventaDto();
                        _movimientoDto = new movimientoDto();
                        EdicionBarraNavegacion(false);
                    }
                    else
                    {
                        _Mode = "New";
                        LimpiarCabecera();
                        _MaxV = 1;
                        _ActV = 1;
                        _ventaDto = new ventaDto();
                        _movimientoDto = new movimientoDto();
                        btnNuevoMovimiento.Enabled = false;
                        EdicionBarraNavegacion(false);
                    }
                    txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Venta, _ventaDto.t_FechaRegistro, dtpFechaRegistro.DateTime, _ventaDto.v_Correlativo, 0);
                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(objOperationResult.ErrorMessage + "\n" + objOperationResult.ExceptionMessage,
                            objOperationResult.AdditionalInformation, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    txtTipoCambio.Text = _objVentasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.DateTime);
                    //cboCondicionPago.Value = "1";
                    CargarDetalle("");
                    break;

                case "Guardado":
                    _MaxV = _ListadoVentas.Count() - 1;
                    _ActV = _MaxV;
                    if (strIdVenta == "" | strIdVenta == null)
                    {
                        CargarCabecera(_ListadoVentas[_MaxV].Value2);
                    }
                    else
                    {
                        CargarCabecera(strIdVenta);
                    }
                    btnNuevoMovimiento.Enabled = true;
                    break;

                case "Consulta":
                    if (_ListadoVentas.Count != 0)
                    {
                        _MaxV = _ListadoVentas.Count - 1;
                        _ActV = _MaxV;
                        txtCorrelativo.Text = (int.Parse(_ListadoVentas[_MaxV].Value1)).ToString("00000000");
                        CargarCabecera(_ListadoVentas[_MaxV].Value2);
                        _Mode = "Edit";
                        EdicionBarraNavegacion(true);
                        cboDocumento.Enabled = false;
                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        LimpiarCabecera();
                        CargarDetalle("");
                        _MaxV = 1;
                        _ActV = 1;
                        _ventaDto = new ventaDto();
                        _movimientoDto = new movimientoDto();
                        btnNuevoMovimiento.Enabled = false;
                        txtTipoCambio.Text = _objVentasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.DateTime);
                        EdicionBarraNavegacion(false);
                        txtMes.Enabled = true;
                    }
                    break;
            }
        }

        private void EdicionBarraNavegacion(bool ON_OFF)
        {
            //txtCorrelativo.Enabled = ON_OFF;
            btnAnterior.Enabled = ON_OFF;
            btnSiguiente.Enabled = ON_OFF;
            btnNuevoMovimiento.Enabled = ON_OFF;
        }

        public void CargarCabecera(string idmovimiento)
        {
            OperationResult objOperationResult = new OperationResult();
            _ventaDto = new ventaDto();
            _ventaDto = _objVentasBL.ObtenerVentaCabecera(ref objOperationResult, idmovimiento);
            if (_ventaDto != null)
            {
                cboMoneda.Value = _ventaDto.i_IdMoneda.ToString();
                txtPeriodo.Text = _ventaDto.v_Periodo;
                txtAnticipio.Text = _ventaDto.d_Anticipio.ToString();
                txtCorrelativo.Text = _ventaDto.v_Correlativo;
                txtCorrelativoDocIni.Text = _ventaDto.v_CorrelativoDocumento;
                txtDescuento.Text = _ventaDto.d_Descuento.ToString();
                txtIGV.Text = _ventaDto.d_IGV.ToString();
                txtMes.Text = int.Parse(_ventaDto.v_Mes).ToString("00");
                txtConcepto.Text = _ventaDto.v_Concepto;
                txtRazonSocial.Text = _ventaDto.NombreCliente;
                txtRucCliente.Text = _ventaDto.NroDocCliente;
                txtSerieDoc.Text = _ventaDto.v_SerieDocumento;
                txtTotal.Text = _ventaDto.d_Total.ToString();
                txtValorVenta.Text = _ventaDto.d_ValorVenta.ToString();
                txtDireccion.Text = _ventaDto.Direccion;
                dtpFechaRegistro.Value = _ventaDto.t_FechaRegistro.Value;
                cboCondicionPago.Value = _ventaDto.i_IdCondicionPago.ToString();
                cboDocumento.Value = _ventaDto.i_IdTipoDocumento.ToString();
                cboEstablecimiento.Value = _ventaDto.i_IdEstablecimiento.ToString();
                cboIGV.Value = _ventaDto.i_IdIgv.ToString();
                cboMoneda.Value = _ventaDto.i_IdMoneda.ToString();
                cboTipoOperacion.Value = _ventaDto.i_IdTipoOperacion.ToString();
                cboTipoVenta.Value = int.Parse(_ventaDto.i_IdTipoVenta.ToString()).ToString("00");
                cboVendedor.Value = _ventaDto.v_IdVendedor != null ? _ventaDto.v_IdVendedor.ToString() : "-1";
                chkAfectoIGV.Checked = _ventaDto.i_EsAfectoIgv == 1 ? true : false;
                chkDeduccionAnticipio.Checked = _ventaDto.i_DeduccionAnticipio == 1 ? true : false;
                chkPrecInIGV.Checked = _ventaDto.i_PreciosIncluyenIgv == 1 ? true : false;
                txtTipoCambio.Text = _ventaDto.d_TipoCambio.ToString();
                txtRucCliente.Enabled = _ventaDto.v_IdCliente != "N002-CL000000000";
                txtRazonSocial.Enabled = _ventaDto.v_IdCliente == "N002-CL000000000";
                txtDireccion.Enabled = _ventaDto.v_IdCliente == "N002-CL000000000";
                _Mode = "Edit";
                BtnImprimir.Enabled = false;
                btnCobrar.Enabled = true;
                CargarDetalle(_ventaDto.v_IdVenta);
                if (_ventaDto.v_NroPedido != string.Empty) RestringirEdicion(); //SI ES UNA VENTA ORIGINADA DE UN PEDIDO NO SE PUEDE EDITAR.
                btnGuardar.Enabled = false;
                groupBox3.Enabled = false;
                grdData.Rows.ToList().ForEach(fila => fila.Activation = Activation.Disabled);
                btnAgregar.Enabled = false;
                btnEliminar.Enabled = false;
                txtPagaCon.Enabled = true;
                ucTipoNota.Value = _ventaDto.i_IdTipoNota;
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar la compra", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void CargarDetalle(string pstringIdVenta)
        {
            OperationResult objOperationResult = new OperationResult();
            try
            {
                grdData.DataSource = _objVentasBL.ObtenerVentaDetalles(ref objOperationResult, pstringIdVenta);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (grdData.Rows.Count > 0)
            {
                grdData.Rows.ToList().ForEach(p => p.Cells["i_RegistroTipo"].Value = "NoTemporal");
                CalcularTotales();
            }
        }

        private void LimpiarCabecera()
        {
            txtAnticipio.Clear();

            txtDescuento.Clear();
            txtIGV.Clear();
            txtConcepto.Text = "ATENCION MEDICA";
            txtRazonSocial.Clear();
            txtRucCliente.Clear();
            txtTotal.Clear();
            txtValorVenta.Clear();
            dtpFechaRegistro.Value = DateTime.Today;
            cboCondicionPago.Value = "-1";
            cboEstablecimiento.Value = "-1";
            cboMoneda.Value = "-1";
            cboTipoVenta.Value = "03"; // predetermina como venta con moneda nacional
            cboVendedor.Value = Globals.ClientSession.v_IdVendedor;
            chkDeduccionAnticipio.Checked = false;
            cboTipoOperacion.Value = Globals.ClientSession.i_IdTipoOperacionVentas.ToString();
            cboIGV.Value = Globals.ClientSession.i_IdIgv.ToString();
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1 ? true : false;
            chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvVentas == 1 ? true : false;
        }

        private void CargarCombosDetalle()
        {
            OperationResult objOperationResult = new OperationResult();

            #region Configura Combo Unidad Medida
            UltraGridBand ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn ultraGridColumnaID = new UltraGridColumn("Id");
            UltraGridColumn ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            ultraGridColumnaDescripcion.Header.VisiblePosition = 0;
            ultraGridColumnaDescripcion.Width = 267;
            ultraGridColumnaID.Hidden = true;
            ultraGridBanda.Columns.AddRange(new object[] { ultraGridColumnaDescripcion, ultraGridColumnaID });
            ucUnidadMedida.DisplayLayout.BandsSerializer.Add(ultraGridBanda);
            ucUnidadMedida.DropDownWidth = 270;
            ucUnidadMedida.DropDownStyle = UltraComboStyle.DropDownList;
            #endregion

            #region Configura Combo Destino
            UltraGridBand _ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn _ultraGridColumnaID = new UltraGridColumn("Id");
            UltraGridColumn _ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            _ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            _ultraGridColumnaDescripcion.Header.VisiblePosition = 0;
            _ultraGridColumnaDescripcion.Width = 327;
            _ultraGridColumnaID.Hidden = true;
            _ultraGridBanda.Columns.AddRange(new object[] { _ultraGridColumnaDescripcion, _ultraGridColumnaID });
            ucDestino.DisplayLayout.BandsSerializer.Add(_ultraGridBanda);
            ucDestino.DropDownWidth = 330;
            #endregion

            #region Configura Combo Almacén
            UltraGridBand __ultraGridBanda = new UltraGridBand("Band 0", -1);
            UltraGridColumn __ultraGridColumnaID = new UltraGridColumn("Id");
            UltraGridColumn __ultraGridColumnaDescripcion = new UltraGridColumn("Value1");
            __ultraGridColumnaID.Header.Caption = "Cod.";
            __ultraGridColumnaDescripcion.Header.Caption = "Descripción";
            __ultraGridColumnaDescripcion.Header.VisiblePosition = 0;
            __ultraGridColumnaID.Width = 50;
            __ultraGridColumnaDescripcion.Width = 327;
            __ultraGridBanda.Columns.AddRange(new object[] { __ultraGridColumnaDescripcion, __ultraGridColumnaID });
            ucAlmacen.DisplayLayout.BandsSerializer.Add(__ultraGridBanda);
            ucAlmacen.DropDownWidth = 380;
            ucAlmacen.DropDownStyle = UltraComboStyle.DropDownList;

            #endregion

            #region Configura Combo Tipo Operación
            UltraGridBand __ultraGridBanda_ = new UltraGridBand("Band 0", -1);
            UltraGridColumn __ultraGridColumnaID_ = new UltraGridColumn("Id");
            UltraGridColumn __ultraGridColumnaDescripcion_ = new UltraGridColumn("Value1");
            __ultraGridColumnaID_.Header.Caption = "Cod.";
            __ultraGridColumnaDescripcion_.Header.Caption = "Descripción";
            __ultraGridColumnaDescripcion_.Header.VisiblePosition = 0;
            __ultraGridColumnaID_.Width = 50;
            __ultraGridColumnaDescripcion_.Width = 327;
            __ultraGridBanda_.Columns.AddRange(new object[] { __ultraGridColumnaDescripcion_, __ultraGridColumnaID_ });
            ucTipoOperacion.DisplayLayout.BandsSerializer.Add(__ultraGridBanda_);
            ucTipoOperacion.DropDownWidth = 380;
            #endregion

            #region Configura Combo Rubros
            UltraGridBand __ultraGridBandaRubro_ = new UltraGridBand("Band 0", -1);
            UltraGridColumn __ultraGridColumnaIDRubro_ = new UltraGridColumn("Id");
            UltraGridColumn __ultraGridColumnaDescripcionRubro_ = new UltraGridColumn("Value1");
            __ultraGridColumnaIDRubro_.Header.Caption = "Cod.";
            __ultraGridColumnaDescripcionRubro_.Header.Caption = "Descripción";
            __ultraGridColumnaDescripcionRubro_.Header.VisiblePosition = 0;
            __ultraGridColumnaIDRubro_.Width = 50;
            __ultraGridColumnaDescripcionRubro_.Width = 327;
            __ultraGridBandaRubro_.Columns.AddRange(new object[] { __ultraGridColumnaDescripcionRubro_, __ultraGridColumnaIDRubro_ });
            ucRubro.DisplayLayout.BandsSerializer.Add(__ultraGridBandaRubro_);
            ucRubro.DropDownWidth = 380;
            #endregion

            ucDestino.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            ucAlmacen.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            ucTipoOperacion.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            ucUnidadMedida.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            ucRubro.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucDestino, "Id", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 24, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucUnidadMedida, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 17, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucAlmacen, "Id", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaComboGrid(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucTipoOperacion, "Id", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 35, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucRubro, "Value1", "Id", _objVentasBL.ObtenRubrosParaComboGridVenta(ref objOperationResult, null), DropDownListAction.Select);
        }

        private void LlenarTemporalesVenta()
        {
            if (grdData.Rows.Count() != 0)
            {
                var objOperationResult = new OperationResult();
                var centroCosto = _objVentasBL.CentroCostoDeEstablecimiento(ref objOperationResult);
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                    {
                        case "Temporal":
                            if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _ventadetalleDto = new ventadetalleDto();
                                _ventadetalleDto.v_IdVenta = _ventaDto.v_IdVenta;
                                _ventadetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _ventadetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _ventadetalleDto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _ventadetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                _ventadetalleDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                                _ventadetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _ventadetalleDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());
                                _ventadetalleDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                                _ventadetalleDto.i_Anticipio = Fila.Cells["i_Anticipio"].Value == null ? 0 : int.Parse(Fila.Cells["i_Anticipio"].Value.ToString());
                                _ventadetalleDto.d_Descuento = Fila.Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Descuento"].Value.ToString());
                                _ventadetalleDto.d_Valor = Fila.Cells["d_Valor"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Valor"].Value.ToString().Trim().Replace("_", ""));
                                _ventadetalleDto.i_InsertaIdUsuario = Fila.Cells["i_InsertaIdUsuario"].Value == null ? 0 : int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _ventadetalleDto.v_NroCuenta = Fila.Cells["v_NroCuenta"].Value == null ? null : Fila.Cells["v_NroCuenta"].Value.ToString().Trim();
                                _ventadetalleDto.i_IdCentroCosto = centroCosto ?? string.Empty;
                                _ventadetalleDto.i_IdTipoOperacion = Fila.Cells["i_IdTipoOperacion"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoOperacion"].Value.ToString());
                                _ventadetalleDto.i_NroUnidades = Fila.Cells["i_NroUnidades"].Value == null ? 0 : int.Parse(Fila.Cells["i_NroUnidades"].Value.ToString());
                                _ventadetalleDto.d_PorcentajeComision = Fila.Cells["d_PorcentajeComision"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PorcentajeComision"].Value.ToString());
                                _ventadetalleDto.i_NroUnidades = Fila.Cells["i_NroUnidades"].Value == null ? 0 : int.Parse(Fila.Cells["i_NroUnidades"].Value.ToString());
                                _ventadetalleDto.v_Observaciones = Fila.Cells["v_Observaciones"].Value == null ? null : Fila.Cells["v_Observaciones"].Value.ToString();
                                _ventadetalleDto.v_PedidoExportacion = Fila.Cells["v_PedidoExportacion"].Value == null ? null : Fila.Cells["v_PedidoExportacion"].Value.ToString();
                                _ventadetalleDto.v_FacturaRef = Fila.Cells["v_FacturaRef"].Value == null ? null : Fila.Cells["v_FacturaRef"].Value.ToString();
                                _ventadetalleDto.v_DescripcionProducto = Fila.Cells["v_DescripcionProducto"].Value == null ? null : Fila.Cells["v_DescripcionProducto"].Value.ToString();
                                _ventadetalleDto.EsServicio = Fila.Cells["i_EsServicio"].Value == null | Fila.Cells["i_EsServicio"].Value.ToString() == "0" ? 0 : 1;
                                _ventadetalleDto.d_Percepcion = Fila.Cells["d_Percepcion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Percepcion"].Value.ToString());
                                _ventadetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _ventadetalleDto.d_PrecioContraparte = Fila.Cells["d_PrecioContraparte"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioContraparte"].Value.ToString());
                                _ventadetalleDto.v_NroLote = Fila.Cells["v_NroLote"].Value == null ? null : Fila.Cells["v_NroLote"].Value.ToString();
                                _ventadetalleDto.t_FechaCaducidad = Fila.Cells["t_FechaCaducidad"].Value == null ? (DateTime?)null : (DateTime?)Fila.Cells["t_FechaCaducidad"].Value;
                                _TempDetalle_AgregarDto.Add(_ventadetalleDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _ventadetalleDto = new ventadetalleDto();
                                _ventadetalleDto.v_IdVentaDetalle = Fila.Cells["v_IdVentaDetalle"].Value == null ? null : Fila.Cells["v_IdVentaDetalle"].Value.ToString();
                                _ventadetalleDto.v_IdMovimientoDetalle = Fila.Cells["v_IdMovimientoDetalle"].Value == null ? null : Fila.Cells["v_IdMovimientoDetalle"].Value.ToString();
                                _ventadetalleDto.v_IdVenta = Fila.Cells["v_IdVenta"].Value == null ? null : Fila.Cells["v_IdVenta"].Value.ToString();
                                _ventadetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _ventadetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _ventadetalleDto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _ventadetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                _ventadetalleDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                                _ventadetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _ventadetalleDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());
                                _ventadetalleDto.d_Descuento = Fila.Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Descuento"].Value.ToString());
                                _ventadetalleDto.d_Valor = Fila.Cells["d_Valor"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Valor"].Value.ToString());
                                _ventadetalleDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                                _ventadetalleDto.i_Anticipio = Fila.Cells["i_Anticipio"].Value == null ? 0 : int.Parse(Fila.Cells["i_Anticipio"].Value.ToString());
                                _ventadetalleDto.i_InsertaIdUsuario = Fila.Cells["i_InsertaIdUsuario"].Value == null ? 0 : int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _ventadetalleDto.v_NroCuenta = Fila.Cells["v_NroCuenta"].Value == null ? null : Fila.Cells["v_NroCuenta"].Value.ToString().Trim();
                                _ventadetalleDto.i_IdCentroCosto = centroCosto ?? string.Empty;
                                _ventadetalleDto.i_IdTipoOperacion = Fila.Cells["i_IdTipoOperacion"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoOperacion"].Value.ToString());
                                _ventadetalleDto.i_NroUnidades = Fila.Cells["i_NroUnidades"].Value == null ? 0 : int.Parse(Fila.Cells["i_NroUnidades"].Value.ToString());
                                _ventadetalleDto.d_PorcentajeComision = Fila.Cells["d_PorcentajeComision"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PorcentajeComision"].Value.ToString());
                                _ventadetalleDto.i_NroUnidades = Fila.Cells["i_NroUnidades"].Value == null ? 0 : int.Parse(Fila.Cells["i_NroUnidades"].Value.ToString());
                                _ventadetalleDto.v_Observaciones = Fila.Cells["v_Observaciones"].Value == null ? null : Fila.Cells["v_Observaciones"].Value.ToString();
                                _ventadetalleDto.v_PedidoExportacion = Fila.Cells["v_PedidoExportacion"].Value == null ? null : Fila.Cells["v_PedidoExportacion"].Value.ToString();
                                _ventadetalleDto.v_FacturaRef = Fila.Cells["v_FacturaRef"].Value == null ? null : Fila.Cells["v_FacturaRef"].Value.ToString();
                                _ventadetalleDto.v_DescripcionProducto = Fila.Cells["v_DescripcionProducto"].Value == null ? null : Fila.Cells["v_DescripcionProducto"].Value.ToString();
                                _ventadetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _ventadetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _ventadetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _ventadetalleDto.d_Percepcion = Fila.Cells["d_Percepcion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Percepcion"].Value.ToString());
                                _ventadetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _ventadetalleDto.d_PrecioContraparte = Fila.Cells["d_PrecioContraparte"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioContraparte"].Value.ToString());
                                _ventadetalleDto.v_NroLote = Fila.Cells["v_NroLote"].Value == null ? null : Fila.Cells["v_NroLote"].Value.ToString();
                                _ventadetalleDto.t_FechaCaducidad = Fila.Cells["t_FechaCaducidad"].Value == null ? (DateTime?)null : (DateTime?)Fila.Cells["t_FechaCaducidad"].Value;
                                _TempDetalle_ModificarDto.Add(_ventadetalleDto);
                            }
                            break;
                    }
                }
            }

        }

        private void LlenarTemporalesMovimiento(string Almacen)
        {
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
                                        _movimientodetalleDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                                        _movimientodetalleDto.v_NumeroDocumento = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text;
                                        _movimientodetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                        _movimientodetalleDto.i_IdUnidad = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                        _movimientodetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                        _movimientodetalleDto.d_Total = decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                        __TempDetalle_AgregarDto.Add(_movimientodetalleDto);
                                    }
                                    break;

                                case "NoTemporal":
                                    if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                                    {
                                        _movimientodetalleDto = new movimientodetalleDto();
                                        _movimientodetalleDto.v_IdMovimientoDetalle = Fila.Cells["v_IdMovimientoDetalle"].Value == null ? null : Fila.Cells["v_IdMovimientoDetalle"].Value.ToString();
                                        _movimientodetalleDto.v_IdMovimiento = _movimientoDto.v_IdMovimiento;
                                        _movimientodetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                        _movimientodetalleDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                                        _movimientodetalleDto.v_NumeroDocumento = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text;
                                        _movimientodetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                        _movimientodetalleDto.i_IdUnidad = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                        _movimientodetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                        _movimientodetalleDto.d_Total = decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                        _movimientodetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                        _movimientodetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                        _movimientodetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                        __TempDetalle_ModificarDto.Add(_movimientodetalleDto);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

        }
        string Truncate(decimal value, int precision)
        {
            string result = value.ToString();

            int dot = result.IndexOf(',');
            if (dot < 0)
            {
                return result;
            }

            int newLength = dot + precision + 1;

            if (newLength == dot + 1)
            {
                newLength--;
            }

            if (newLength > result.Length)
            {
                newLength = result.Length;
            }

            return result.Substring(0, newLength);
        }

        private void CalcularValoresDetalle(bool truncate = false)
        {
            if (!grdData.Rows.Any()) return;
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                CalcularValoresFila(Fila, truncate);
            }
        }

        private void CalcularValoresFila(UltraGridRow fila, bool truncate = false)
        {
            try
            {
                if (cboIGV.Value.ToString() == "-1")
                {
                    UltraMessageBox.Show("Porfavor seleccione el IGV", "Sistema");
                    return;
                }

                if (fila.Cells["v_IdProductoDetalle"].Value != null && fila.Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000") return;

                var detail = (ventadetalleDto)fila.ListObject;
                if (detail.i_Anticipio == null) return;
                if (detail.v_IdProductoDetalle != null && detail.v_IdProductoDetalle == "N002-PE000000000") return;

                if (detail.d_Cantidad == null) detail.d_Cantidad = 0M;
                if (detail.d_Precio == null) detail.d_Precio = 0M;
                if (detail.v_FacturaRef == null) detail.v_FacturaRef = "0";
                if (detail.d_otrostributos == null) detail.d_otrostributos = 0;
                if (detail.i_IdTipoOperacion == null) detail.i_IdTipoOperacion = 1;

                var descuentos = detail.v_FacturaRef;

                if (detail.i_Anticipio.ToString() == "1")
                {
                    detail.d_Cantidad = 0M;
                    detail.d_Precio = 0M;
                    if (chkAfectoIGV.Checked)
                    {
                        if (chkPrecInIGV.Checked) detail.d_Valor = detail.d_PrecioVenta;
                    }
                    else
                        detail.d_Valor = 0M;
                }
                else
                {
                    var price = detail.d_Precio.Value;
                    var porcIgv = decimal.Parse(cboIGV.Text) / 100;
                    var esGravado = detail.i_IdTipoOperacion == 1 || (detail.i_IdTipoOperacion > 10 && detail.i_IdTipoOperacion < 20);
                    if (esGravado && chkPrecInIGV.Checked) price /= 1 + porcIgv;
                    detail.d_isc = 0.00M;
                    detail.d_Valor = Utils.Windows.DevuelveValorRedondeado(detail.d_Cantidad.Value * price, 2);
                    detail.d_Descuento = Utils.Windows.CalcularDescuentosSucesivos(descuentos, detail.d_Valor.Value);

                    if (esGravado && chkPrecInIGV.Checked)
                    {
                        detail.d_PrecioVenta = Utils.Windows.DevuelveValorRedondeado(detail.d_Precio.Value * detail.d_Cantidad.Value, 2);
                        detail.d_PrecioVenta -= Utils.Windows.CalcularDescuentosSucesivos(descuentos, detail.d_PrecioVenta - (detail.d_isc * (1 + porcIgv)) ?? 0);
                        detail.d_PrecioVenta = Utils.Windows.DevuelveValorRedondeado(detail.d_PrecioVenta ?? 0, 2);
                        detail.d_ValorVenta = Utils.Windows.DevuelveValorRedondeado(detail.d_PrecioVenta.Value / (1 + porcIgv) - detail.d_isc.Value, 2);
                        detail.d_Igv = Utils.Windows.DevuelveValorRedondeado(detail.d_PrecioVenta - detail.d_ValorVenta - detail.d_isc ?? 0, 2);

                        detail.d_Valor = Utils.Windows.DevuelveValorRedondeado(detail.d_ValorVenta.Value + detail.d_Descuento.Value, 2);
                    }
                    else
                    {
                        detail.d_ValorVenta = Utils.Windows.DevuelveValorRedondeado(detail.d_Valor - detail.d_Descuento ?? 0, 2);
                        detail.d_Igv = esGravado ? Utils.Windows.DevuelveValorRedondeado((detail.d_ValorVenta.Value + detail.d_isc.Value) * porcIgv, 2) : 0M;
                        detail.d_PrecioVenta = Utils.Windows.DevuelveValorRedondeado(detail.d_ValorVenta + detail.d_Igv + detail.d_otrostributos + detail.d_isc ?? 0, 2);
                    }
                }
                detail.d_CantidadEmpaque = GetCantidadEmpaque(detail);
                CalcularTotales();
                grdData.UpdateData();

            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }

        }

        private decimal GetCantidadEmpaque(ventadetalleDto detail)
        {
            if (detail.i_IdUnidadMedida == null) return 0M;
            if (detail.Empaque == null) detail.d_CantidadEmpaque = 0M;

            if (detail.v_IdProductoDetalle != null && detail.v_IdProductoDetalle != "N002-PE000000000" && detail.i_IdUnidadMedida != -1)
            {
                var totalEmpaque = 0M;
                var empaque = detail.Empaque.Value;
                var cantidad = detail.d_Cantidad.Value;

                var umProducto = ((List<GridKeyValueDTO>)ucUnidadMedida.DataSource).FirstOrDefault(p => p.Id == detail.i_IdUnidadMedidaProducto.ToString());
                var um = ((List<GridKeyValueDTO>)ucUnidadMedida.DataSource).FirstOrDefault(p => p.Id == detail.i_IdUnidadMedida.ToString());

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
            if (grdData.Rows.Any())
            {
                decimal SumAntValVenta = 0, SumAntIgv = 0, SumAntTotal = 0;
                decimal SumDescuento = 0, SumValVenta = 0, SumIgv = 0, SumTotal = 0, SumVal = 0;

                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["d_ValorVenta"].Value == null) { Fila.Cells["d_ValorVenta"].Value = "0"; }
                    if (Fila.Cells["d_Igv"].Value == null) { Fila.Cells["d_Igv"].Value = "0"; }
                    if (Fila.Cells["d_PrecioVenta"].Value == null) { Fila.Cells["d_PrecioVenta"].Value = "0"; }
                    if (Fila.Cells["d_Descuento"].Value == null) { Fila.Cells["d_Descuento"].Value = "0"; }

                    switch (Fila.Cells["i_Anticipio"].Value.ToString())
                    {
                        case "1":
                            SumVal = SumVal + decimal.Parse(Fila.Cells["d_Valor"].Value.ToString());
                            SumAntValVenta = SumAntValVenta + decimal.Parse(Fila.Cells["d_ValorVenta"].Text.Trim().Replace("_", ""));
                            SumAntIgv = SumAntIgv + decimal.Parse(Fila.Cells["d_Igv"].Text.Trim().Replace("_", ""));
                            SumAntTotal = SumAntTotal + decimal.Parse(Fila.Cells["d_PrecioVenta"].Text.Trim().Replace("_", ""));
                            SumDescuento = SumDescuento + decimal.Parse(Fila.Cells["d_Descuento"].Text.Trim().Replace("_", ""));
                            break;

                        case "0":
                            SumVal = SumVal + decimal.Parse(Fila.Cells["d_Valor"].Value.ToString());
                            SumValVenta = SumValVenta + decimal.Parse(Fila.Cells["d_ValorVenta"].Text.Trim().Replace("_", ""));
                            SumIgv = SumIgv + decimal.Parse(Fila.Cells["d_Igv"].Text.Replace("_", "").Trim());
                            SumTotal = SumTotal + decimal.Parse(Fila.Cells["d_PrecioVenta"].Text.Trim().Replace("_", ""));
                            SumDescuento = SumDescuento + decimal.Parse(Fila.Cells["d_Descuento"].Text.Trim().Replace("_", ""));
                            break;
                    }
                }

                txtAnticipio.Text = SumAntValVenta.ToString("0.00");
                txtValor.Text = SumVal.ToString("0.00");
                txtValorVenta.Text = (SumValVenta - SumAntValVenta).ToString("0.00");
                txtDescuento.Text = SumDescuento.ToString("0.00");
                txtIGV.Text = (SumIgv - SumAntIgv).ToString("0.00");
                txtTotal.Text = (SumTotal - SumAntTotal).ToString("0.00");

                Total = decimal.Parse(txtTotal.Text);
                Redondeado = decimal.Parse(Math.Round(Total, 1, MidpointRounding.AwayFromZero).ToString("0.00"));
                Residuo = (Total - Redondeado) * -1;
                _Redondeado = Residuo == 0;
                btnRedondear.Enabled = Residuo != 0;

                txtMontoCobrar.Text = CalcularMontoCobrar(int.Parse(cboMoneda.Value.ToString()),
                !string.IsNullOrEmpty(txtTotal.Text.Trim()) ? decimal.Parse(txtTotal.Text) : 0, int.Parse(cboMonedaCobranza.Value.ToString()),
                    decimal.Parse(txtTipoCambio.Text));

                if (cboDocumento.Value.ToString() == "3" || cboDocumento.Value.ToString() == "1")
                {
                    List<UltraGridRow> FilasPercepcion = grdData.Rows.Where(p => p.Cells["i_EsAfectoPercepcion"].Value != null && p.Cells["i_EsAfectoPercepcion"].Value.ToString() == "1").ToList();
                    if (FilasPercepcion.Where(p => cboDocumento.Value.ToString() == "3" ? decimal.Parse(p.Cells["d_PrecioVenta"].Value.ToString()) > 700 : decimal.Parse(p.Cells["d_PrecioVenta"].Value.ToString()) > 0).ToList().Any())
                    {
                        FilasPercepcion = FilasPercepcion.Where(p => cboDocumento.Value.ToString() == "3" ? decimal.Parse(p.Cells["d_PrecioVenta"].Value.ToString()) > 700 : decimal.Parse(p.Cells["d_PrecioVenta"].Value.ToString()) > 0).ToList();
                        FilasPercepcion.ForEach(o => o.Cells["d_Percepcion"].Value = o.Cells["d_PrecioVenta"].Value != null ? (decimal.Parse(o.Cells["d_PrecioVenta"].Value.ToString()) * (decimal.Parse(o.Cells["d_TasaPercepcion"].Value.ToString()) / 100)).ToString() : "0");
                        txtTotal.Text = (decimal.Parse(txtTotal.Text) + FilasPercepcion.Sum(p => decimal.Parse(p.Cells["d_Percepcion"].Value.ToString()))).ToString("0.00");
                    }
                    else
                    {
                        FilasPercepcion.ForEach(o => o.Cells["d_Percepcion"].Value = "0");
                    }
                }
                else
                {
                    List<UltraGridRow> FilasPercepcion = grdData.Rows.Where(p => p.Cells["i_EsAfectoPercepcion"].Value != null && p.Cells["i_EsAfectoPercepcion"].Value.ToString() == "1").ToList();
                    FilasPercepcion.ForEach(o => o.Cells["d_Percepcion"].Value = "0");
                    txtTotal.Text = (decimal.Parse(txtTotal.Text) + FilasPercepcion.Sum(p => decimal.Parse(p.Cells["d_Percepcion"].Value.ToString()))).ToString("0.00");
                }
            }
        }

        private void ComprobarExistenciaCorrelativoDocumento()
        {
            OperationResult objOperationResult = new OperationResult();
            if (_objVentasBL.ComprobarExistenciaCorrelativoDocumento(ref objOperationResult, int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text, txtCorrelativoDocIni.Text, _ventaDto.v_IdVenta) == true)
            {
                UltraMessageBox.Show("El documento ya ha sido registrado anteriormente", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGuardar.Enabled = true;
            }
            else
            {
                if (strModo != "Guardado")
                {
                    btnGuardar.Enabled = true;
                }
            }
        }

        private void LimpiarDetalle()
        {
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                if (Fila.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
                {
                    _ventadetalleDto = new ventadetalleDto();
                    _ventadetalleDto.v_IdVentaDetalle = Fila.Cells["v_IdVentaDetalle"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(_ventadetalleDto);

                    if (Fila.Cells["v_IdMovimientoDetalle"].Value != null)
                    {
                        _movimientodetalleDto = new movimientodetalleDto();
                        _movimientodetalleDto.v_IdMovimientoDetalle = Fila.Cells["v_IdMovimientoDetalle"].Value.ToString();
                        __TempDetalle_EliminarDto.Add(_movimientodetalleDto);
                    }
                }
            }
            grdData.Selected.Rows.AddRange((UltraGridRow[])this.grdData.Rows.All);
            grdData.DeleteSelectedRows(false);
            CalcularValoresDetalle();
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
                FormatoCantidad = "nnnnnnn.";
                for (int i = 0; i < DecimalesCantidad; i++)
                {
                    FormatoCantidad = FormatoCantidad + sharp;
                }
            }
            else
            {
                FormatoCantidad = "nnnnnnn";
            }

            if (DecimalesPrecio > 0)
            {
                string sharp = "n";
                FormatoPrecio = "nnnnnnn.";
                for (int i = 0; i < DecimalesPrecio; i++)
                {
                    FormatoPrecio = FormatoPrecio + sharp;
                }
            }
            else
            {
                FormatoPrecio = "nnnnnnn";
            }

            _Cantidad.MaskInput = FormatoCantidad;
            _Precio.MaskInput = "-" + FormatoPrecio;
        }

        private void RestringirEdicion()
        {
            UltraGridColumn c;
            c = grdData.DisplayLayout.Bands[0].Columns["i_IdAlmacen"];
            c.CellActivation = Activation.ActivateOnly;
            c.CellClickAction = CellClickAction.CellSelect;

            c = grdData.DisplayLayout.Bands[0].Columns["d_Cantidad"];
            c.CellActivation = Activation.ActivateOnly;
            c.CellClickAction = CellClickAction.CellSelect;

            c = grdData.DisplayLayout.Bands[0].Columns["d_Precio"];
            c.CellActivation = Activation.ActivateOnly;
            c.CellClickAction = CellClickAction.CellSelect;

            c = grdData.DisplayLayout.Bands[0].Columns["i_IdUnidadMedida"];
            c.CellActivation = Activation.ActivateOnly;
            c.CellClickAction = CellClickAction.CellSelect;

            c = grdData.DisplayLayout.Bands[0].Columns["d_PrecioVenta"];
            c.CellActivation = Activation.ActivateOnly;
            c.CellClickAction = CellClickAction.CellSelect;

            c = grdData.DisplayLayout.Bands[0].Columns["i_NroUnidades"];
            c.CellActivation = Activation.ActivateOnly;
            c.CellClickAction = CellClickAction.CellSelect;

            btnAgregar.Enabled = false;
            btnEliminar.Enabled = false;

            btnBuscarDetraccion.Enabled = false;
            txtRucCliente.Enabled = false;
        }

        private bool EsVentaAfectaDetraccion()
        {
            CalcularTotales();
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                if (Fila.Cells["i_EsAfectoDetraccion"].Value != null)
                {
                    if (Fila.Cells["i_EsAfectoDetraccion"].Value.ToString() == "1")
                    {
                        if (cboDocumento.Value.ToString() == "1" && decimal.Parse(txtTotal.Text) >= 700)
                        {
                            return true;
                        }
                    }
                }

            }
            return false;
        }

        private void ClientePublicoGeneral()
        {
            string[] Cadena = new string[4];
            Cadena = _objVentasBL.PublicoGeneral();
            if (Cadena != null)
            {
                _ventaDto.v_IdCliente = Cadena[0];
                txtRazonSocial.Text = Cadena[2];
                txtRucCliente.Text = Cadena[3];
                txtRazonSocial.Enabled = true;
                txtDireccion.Clear();
                txtDireccion.Enabled = true;
            }
            else
            {
                if (UltraMessageBox.Show("El Cliente Público General no está definido en la Base de Datos, ¿Desea Agregarlo?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    _objVentasBL.InsertaClientePublicoGeneralSiEsNecesario();
                    ClientePublicoGeneral();
                }
            }
        }

        private void Redondeo()
        {
            if (Residuo != 0 && _Redondeado == false)
            {
                _limiteDocumento++;
                _Redondeado = true;
                UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                if (row == null) return;
                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["d_Cantidad"].Value = "1";
                row.Cells["d_Precio"].Value = Residuo.ToString();
                row.Cells["i_IdUnidadMedida"].Value = "-1";
                row.Cells["i_Anticipio"].Value = "0";
                row.Cells["d_Descuento"].Value = "0";
                row.Cells["d_Valor"].Value = Residuo.ToString();
                row.Cells["d_ValorVenta"].Value = Residuo.ToString();
                row.Cells["EsRedondeo"].Value = "1";
                row.Cells["d_PrecioVenta"].Value = Residuo.ToString();
                row.Cells["i_IdCentroCosto"].Value = "0";
                row.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString() == "5" ? "1" : cboTipoOperacion.Value.ToString();
                row.Cells["i_IdTipoOperacionAnexo"].Value = row.Cells["i_IdTipoOperacion"].Value.ToString() == "2" ? 1 : 0;
                row.Cells["v_NroCuenta"].Value = Residuo < 0 ? Globals.ClientSession.v_NroCuentaRedondeoPerdida : Globals.ClientSession.v_NroCuentaRedondeoGanancia;
                row.Cells["i_IdUnidadMedida"].Value = "15";
                row.Cells["v_IdProductoDetalle"].Value = "N002-PE000000000";
                row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                row.Cells["v_CodigoInterno"].Value = "00000000";
                row.Cells["v_DescripcionProducto"].Value = "REDONDEO";
                row.Cells["i_EsServicio"].Value = "1";
                row.Cells["i_EsAfectoDetraccion"].Value = "0";
                row.Cells["i_EsNombreEditable"].Value = "0";
                txtTotal.Text = Redondeado.ToString("0.00");
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

        public void RecibirItems(List<UltraGridRow> Filas)
        {
            for (int i = 0; i < Filas.Count; i++)
            {
                var tipoOperacion = (string)cboTipoOperacion.Value != "5" ? cboTipoOperacion.Value.ToString() : "1";
                if (i == 0)
                {
                    grdData.ActiveRow.Cells["v_DescripcionProducto"].Value =
                        Filas[i].Cells["v_Descripcion"].Value.ToString();
                    grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value =
                        Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                    grdData.ActiveRow.Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                    grdData.ActiveRow.Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null
                        ? 0
                        : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                    grdData.ActiveRow.Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null
                        ? null
                        : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                    grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_EsAfectoDetraccion"].Value =
                        Filas[i].Cells["i_EsAfectoDetraccion"].Value == null
                            ? 0
                            : int.Parse(Filas[i].Cells["i_EsAfectoDetraccion"].Value.ToString());
                    grdData.ActiveRow.Cells["d_Precio"].Value =
                        DevolverPrecioProducto(
                            Filas[i].Cells["IdMoneda"].Value != null
                                ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString())
                                : -1,
                            Filas[i].Cells["d_Precio"].Value == null
                                ? 0
                                : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()),
                            int.Parse(cboMoneda.Value.ToString()),
                            !string.IsNullOrEmpty(txtTipoCambio.Text.Trim())
                                ? decimal.Parse(txtTipoCambio.Text.Trim())
                                : 0);
                    grdData.ActiveRow.Cells["i_EsNombreEditable"].Value = Filas[i].Cells["i_NombreEditable"].Value ==
                                                                          null
                        ? 0
                        : int.Parse(Filas[i].Cells["i_NombreEditable"].Value.ToString());
                    grdData.ActiveRow.Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null
                        ? 0
                        : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                    grdData.ActiveRow.Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null
                        ? UserConfig.Default.CantVenta
                        : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                    grdData.ActiveRow.Cells["d_isc"].Value = "0";
                    grdData.ActiveRow.Cells["d_otrostributos"].Value = "0";
                    grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = tipoOperacion;
                    grdData.ActiveRow.Cells["v_NroCuenta"].Value = Filas[i].Cells["NroCuentaVenta"].Value != null
                        ? Filas[i].Cells["NroCuentaVenta"].Value.ToString()
                        : "-1";
                    grdData.ActiveRow.Cells["i_ValidarStock"].Value = Filas[i].Cells["i_ValidarStock"].Value != null
                        ? Filas[i].Cells["i_ValidarStock"].Value.ToString()
                        : "0";
                    grdData.ActiveRow.Cells["i_EsAfectoPercepcion"].Value =
                        Filas[i].Cells["i_EsAfectoPercepcion"].Value != null
                            ? Filas[i].Cells["i_EsAfectoPercepcion"].Value.ToString()
                            : "0";
                    grdData.ActiveRow.Cells["d_TasaPercepcion"].Value = Filas[i].Cells["d_TasaPercepcion"].Value == null
                        ? 0
                        : decimal.Parse(Filas[i].Cells["d_TasaPercepcion"].Value.ToString());
                    grdData.ActiveRow.Cells["i_IdMonedaLP"].Value = Filas[i].Cells["IdMoneda"].Value != null
                        ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString())
                        : -1;
                    grdData.ActiveRow.Cells["v_PedidoExportacion"].Value =
                        Filas[i].Cells["v_NroPedidoExportacion"].Value == null
                            ? null
                            : Filas[i].Cells["v_NroPedidoExportacion"].Value.ToString();
                    grdData.ActiveRow.Cells["i_SolicitarNroSerieSalida"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroSerieSalida"].Value.ToString());
                    grdData.ActiveRow.Cells["i_SolicitarNroLoteSalida"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroLoteSalida"].Value.ToString());

                    grdData.ActiveRow.Cells["v_NroSerie"].Value = Filas[i].Cells["v_NroSerie"].Value == null || Filas[i].Cells["v_NroSerie"].Value == "" ? null : Filas[i].Cells["v_NroSerie"].Value.ToString();
                    grdData.ActiveRow.Cells["v_NroLote"].Value = Filas[i].Cells["v_NroLote"].Value == null || Filas[i].Cells["v_NroLote"].Value == "" ? null : Filas[i].Cells["v_NroLote"].Value.ToString();
                    grdData.ActiveRow.Cells["t_FechaCaducidad"].Value = Filas[i].Cells["t_FechaCaducidad"].Value == null ? null : Filas[i].Cells["t_FechaCaducidad"].Value.ToString();

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
                    row.Cells["d_Cantidad"].Value = UserConfig.Default.CantVenta;
                    row.Cells["d_Precio"].Value = "0";
                    row.Cells["i_IdUnidadMedida"].Value = "-1";
                    row.Cells["i_Anticipio"].Value = "0";
                    row.Cells["d_Descuento"].Value = "0";
                    row.Cells["d_Percepcion"].Value = "0";
                    row.Cells["d_Valor"].Value = "0";
                    row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    row.Activate();
                    grdData.ActiveRow.Cells["v_DescripcionProducto"].Value =
                        Filas[i].Cells["v_Descripcion"].Value.ToString();
                    grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value =
                        Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                    grdData.ActiveRow.Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                    grdData.ActiveRow.Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null
                        ? 0
                        : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                    grdData.ActiveRow.Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null
                        ? null
                        : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                    grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = tipoOperacion;
                    grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null
                        ? null
                        : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value =
                        Filas[i].Cells["i_IdUnidadMedida"].Value == null
                            ? null
                            : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_EsAfectoDetraccion"].Value =
                        Filas[i].Cells["i_EsAfectoDetraccion"].Value == null
                            ? 0
                            : int.Parse(Filas[i].Cells["i_EsAfectoDetraccion"].Value.ToString());
                    grdData.ActiveRow.Cells["d_Precio"].Value =
                        DevolverPrecioProducto(
                            Filas[i].Cells["IdMoneda"].Value != null
                                ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString())
                                : -1,
                            Filas[i].Cells["d_Precio"].Value == null
                                ? 0
                                : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()),
                            int.Parse(cboMoneda.Value.ToString()),
                            !string.IsNullOrEmpty(txtTipoCambio.Text.Trim())
                                ? decimal.Parse(txtTipoCambio.Text.Trim())
                                : 0);
                    grdData.ActiveRow.Cells["i_EsNombreEditable"].Value = Filas[i].Cells["i_NombreEditable"].Value ==
                                                                          null
                        ? 0
                        : int.Parse(Filas[i].Cells["i_NombreEditable"].Value.ToString());
                    grdData.ActiveRow.Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null
                        ? 0
                        : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                    grdData.ActiveRow.Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null
                        ? UserConfig.Default.CantVenta
                        : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                    grdData.ActiveRow.Cells["d_isc"].Value = "0";
                    grdData.ActiveRow.Cells["d_otrostributos"].Value = "0";
                    grdData.ActiveRow.Cells["v_NroCuenta"].Value = Filas[i].Cells["NroCuentaVenta"].Value != null
                        ? Filas[i].Cells["NroCuentaVenta"].Value.ToString()
                        : "-1";
                    grdData.ActiveRow.Cells["i_ValidarStock"].Value = Filas[i].Cells["i_ValidarStock"].Value != null
                        ? Filas[i].Cells["i_ValidarStock"].Value.ToString()
                        : "0";
                    grdData.ActiveRow.Cells["i_EsAfectoPercepcion"].Value =
                        Filas[i].Cells["i_EsAfectoPercepcion"].Value != null
                            ? Filas[i].Cells["i_EsAfectoPercepcion"].Value.ToString()
                            : "0";
                    grdData.ActiveRow.Cells["d_TasaPercepcion"].Value = Filas[i].Cells["d_TasaPercepcion"].Value == null
                        ? 0
                        : decimal.Parse(Filas[i].Cells["d_TasaPercepcion"].Value.ToString());
                    grdData.ActiveRow.Cells["i_IdMonedaLP"].Value = Filas[i].Cells["IdMoneda"].Value != null
                        ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString())
                        : -1;
                    grdData.ActiveRow.Cells["v_PedidoExportacion"].Value =
                        Filas[i].Cells["v_NroPedidoExportacion"].Value == null
                            ? null
                            : Filas[i].Cells["v_NroPedidoExportacion"].Value.ToString();

                    grdData.ActiveRow.Cells["i_SolicitarNroSerieSalida"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroSerieSalida"].Value.ToString());
                    grdData.ActiveRow.Cells["i_SolicitarNroLoteSalida"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroLoteSalida"].Value.ToString());

                    grdData.ActiveRow.Cells["v_NroSerie"].Value = Filas[i].Cells["v_NroSerie"].Value == null || Filas[i].Cells["v_NroSerie"].Value == "" ? null : Filas[i].Cells["v_NroSerie"].Value.ToString();
                    grdData.ActiveRow.Cells["v_NroLote"].Value = Filas[i].Cells["v_NroLote"].Value == null || Filas[i].Cells["v_NroLote"].Value == "" ? null : Filas[i].Cells["v_NroLote"].Value.ToString();
                    grdData.ActiveRow.Cells["t_FechaCaducidad"].Value = Filas[i].Cells["t_FechaCaducidad"].Value == null ? null : Filas[i].Cells["t_FechaCaducidad"].Value.ToString();

                }
            }
            CalcularValoresDetalle();
        }

        private void PredeterminarEstablecimiento(string serie)
        {
            int d;
            if (cboDocumento.Value != null && int.TryParse(cboDocumento.Value.ToString(), out d))
                cboEstablecimiento.Value = _objEstablecimientoBL.DevolverEstablecimientoXSerie(d, serie).ToString();
        }

        private bool ValidaCamposNulosVacios()
        {
            if (grdData.Rows.Count(p => p.Cells["v_NroCuenta"].Value == null || p.Cells["v_NroCuenta"].Value.ToString().Trim() == "-1") != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente todas las cuentas al detalle.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["v_NroCuenta"].Value == null || x.Cells["v_NroCuenta"].Value.ToString().Trim() == "-1");
                grdData.Selected.Cells.Add(Row.Cells["v_NroCuenta"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_NroCuenta"];
                this.grdData.ActiveCell = aCell;
                return false;
            }


            if (grdData.Rows.Count(p => p.Cells["d_Cantidad"].Value == null || decimal.Parse(p.Cells["d_Cantidad"].Value.ToString().Trim()) <= 0) != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente la Cantidad.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(p => p.Cells["d_Cantidad"].Value == null || decimal.Parse(p.Cells["d_Cantidad"].Value.ToString().Trim()) == 0);
                grdData.Selected.Cells.Add(Row.Cells["d_Cantidad"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_Cantidad"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (txtSerieDoc.Text != "TFM" && txtSerieDoc.Text != "RSL" && txtSerieDoc.Text != "THM")
            {
                if (grdData.Rows.Count(p => p.Cells["d_Precio"].Value == null || decimal.Parse(p.Cells["d_Precio"].Value.ToString().Trim()) == 0) != 0)
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente el Precio.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdData.Rows.FirstOrDefault(p => p.Cells["d_Precio"].Value == null || decimal.Parse(p.Cells["d_Precio"].Value.ToString().Trim()) == 0);
                    grdData.Selected.Cells.Add(Row.Cells["d_Precio"]);
                    grdData.Focus();
                    Row.Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_Precio"];
                    this.grdData.ActiveCell = aCell;
                    return false;
                }
            }

            if (grdData.Rows.Count(p => p.Cells["i_IdAlmacen"].Value == null || p.Cells["i_IdAlmacen"].Value.ToString().Trim() == "-1") != 0)
            {
                UltraMessageBox.Show("Por favor un almacén.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["i_IdAlmacen"].Value == null || x.Cells["i_IdAlmacen"].Value.ToString().Trim() == "-1");
                grdData.Selected.Cells.Add(Row.Cells["i_IdAlmacen"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdAlmacen"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (grdData.Rows.Count(p => p.Cells["i_IdUnidadMedida"].Value == null || p.Cells["i_IdUnidadMedida"].Value.ToString().Trim() == "-1") != 0)
            {
                UltraMessageBox.Show("Por favor una Unidad de Medida.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["i_IdUnidadMedida"].Value == null || x.Cells["i_IdUnidadMedida"].Value.ToString().Trim() == "-1");
                grdData.Selected.Cells.Add(Row.Cells["i_IdUnidadMedida"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdUnidadMedida"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (grdData.Rows.Count(p => p.Cells["v_IdProductoDetalle"].Value == null || p.Cells["v_IdProductoDetalle"].Value.ToString().Trim() == string.Empty) != 0)
            {
                UltraMessageBox.Show("Por favor ingrese un Producto al detalle.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(x => x.Cells["v_IdProductoDetalle"].Value == null || x.Cells["v_IdProductoDetalle"].Value.ToString().Trim() == string.Empty);
                grdData.Selected.Cells.Add(Row.Cells["v_IdProductoDetalle"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_IdProductoDetalle"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            return true;
        }

        #endregion

        #region Eventos de la Grilla
        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].Columns["i_Anticipio"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].EditorComponent = ucUnidadMedida;
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].EditorComponent = ucAlmacen;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdTipoOperacion"].EditorComponent = ucTipoOperacion;
            e.Layout.Bands[0].Columns["i_IdTipoOperacion"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["v_NroCuenta"].EditorComponent = ucRubro;
            e.Layout.Bands[0].Columns["v_NroCuenta"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            if (Globals.ClientSession.i_PermiteIntercambiarListasPrecios == 0)
            {
                e.Layout.Bands[0].Columns["d_Precio"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.Default;
            }
        }

        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdData.ActiveCell == null) return;

            if (grdData.ActiveCell.Column.Key == "v_DescripcionProducto" && grdData.ActiveRow.Cells["i_EsNombreEditable"].Value != null && grdData.ActiveRow.Cells["i_EsNombreEditable"].Value.ToString() == "0")
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

            if (e.KeyCode == Keys.Up | e.KeyCode == Keys.Down)
            {
                if (grdData.ActiveCell.Column.Key == "i_IdAlmacen" && grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null)
                {
                    e.SuppressKeyPress = true;
                    return;
                }
            }

            if (grdData.ActiveCell.Column.Key != "i_IdAlmacen" && grdData.ActiveCell.Column.Key != "i_IdUnidadMedida" && grdData.ActiveCell.Column.Key != "i_IdTipoOperacion" && grdData.ActiveCell.Column.Key != "v_NroCuenta")
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

            if (grdData.ActiveCell != null && grdData.ActiveCell.Column.Key == "d_Precio")
            {
                if (Globals.ClientSession.i_EditarPrecioVenta == 0)
                    e.SuppressKeyPress = true;
            }
        }

        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "v_CodigoInterno":
                    if (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) | cboDocumento.Value.ToString() == "8") return;
                    if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "Temporal")
                    {
                        if (grdData.ActiveRow.Cells["i_IdAlmacen"].Value == null) return;

                        if (!string.IsNullOrEmpty(e.Cell.Text) && _objVentasBL.EsValidoCodProducto(e.Cell.Text))
                        {
                            productoshortDto prod = Globals.ClientSession.i_UsaListaPrecios == 1
                                ? string.IsNullOrEmpty(_ventaDto.v_IdCliente) ? _objVentasBL.DevolverArticuloPorCodInternoLista1(e.Cell.Text, _ventaDto.v_IdCliente, int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString())) : _objVentasBL.DevolverArticuloPorCodInterno(e.Cell.Text, _ventaDto.v_IdCliente, int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()))
                                : _objVentasBL.DevolverArticuloPorCodInterno(e.Cell.Text, int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()));



                            if (prod != null)
                            {
                                grdData.ActiveRow.Cells["v_DescripcionProducto"].Value = prod.v_Descripcion.Trim();
                                grdData.ActiveRow.Cells["d_Cantidad"].Value = "1";
                                grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = prod.v_IdProductoDetalle.Trim();
                                grdData.ActiveRow.Cells["v_CodigoInterno"].Value = prod.v_CodInterno.Trim();
                                grdData.ActiveRow.Cells["Empaque"].Value = prod.d_Empaque;
                                grdData.ActiveRow.Cells["UMEmpaque"].Value = prod.EmpaqueUnidadMedida;
                                grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                                grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                                grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = "0";
                                grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = prod.i_IdUnidadMedida;
                                grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = prod.i_IdUnidadMedida;
                                grdData.ActiveRow.Cells["i_EsAfectoDetraccion"].Value = prod.i_EsAfectoDetraccion;
                                grdData.ActiveRow.Cells["d_Precio"].Value = DevolverPrecioProducto(prod.IdMoneda, prod.d_Precio ?? 0,
                                    int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ?
                                    decimal.Parse(txtTipoCambio.Text.Trim()) : 0);
                                grdData.ActiveRow.Cells["i_EsNombreEditable"].Value = prod.i_NombreEditable;
                                grdData.ActiveRow.Cells["i_EsServicio"].Value = prod.i_EsServicio;
                                grdData.ActiveRow.Cells["v_NroCuenta"].Value = prod.NroCuentaVenta;
                                grdData.ActiveRow.Cells["i_ValidarStock"].Value = prod.i_ValidarStock ?? 0;
                                grdData.ActiveRow.Cells["i_EsAfectoPercepcion"].Value = prod.i_EsAfectoPercepcion ?? 0;
                                grdData.ActiveRow.Cells["d_TasaPercepcion"].Value = prod.d_TasaPercepcion;
                                grdData.ActiveRow.Cells["v_CodigoInterno"].Activation = Activation.Disabled;
                                if (!string.IsNullOrWhiteSpace(prod.Observacion))
                                    grdData.ActiveRow.Cells["v_DescripcionProducto"].ToolTipText = prod.Observacion;
                            }
                            else
                            {
                                UltraMessageBox.Show("El Artículo existe Pero no tuvo ingresos a este almacén o está inactivo.", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                btnEliminar_Click(sender, e);
                            }
                        }
                        else
                        {
                            Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()), "PedidoVenta", _ventaDto.v_IdCliente, grdData.ActiveRow.Cells["v_CodigoInterno"].Text ?? string.Empty, UserConfig.Default.appTipoBusquedaPredeterminadaProducto);
                            frm.ShowDialog();
                            if (frm._NombreProducto != null)
                            {
                                //int IdAlmacen = int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString());
                                grdData.ActiveRow.Cells["v_DescripcionProducto"].Value = frm._NombreProducto.Trim();
                                grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = frm._IdProducto;
                                grdData.ActiveRow.Cells["v_CodigoInterno"].Value = frm._CodigoInternoProducto.Trim();
                                grdData.ActiveRow.Cells["Empaque"].Value = frm._Empaque != null ? frm._Empaque.ToString() : null;
                                grdData.ActiveRow.Cells["UMEmpaque"].Value = frm._UnidadMedida ?? string.Empty;
                                grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                                grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = frm._UnidadMedidaEmpaque;
                                grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                                grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = "0";
                                grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = frm._UnidadMedidaEmpaque;
                                grdData.ActiveRow.Cells["i_EsAfectoDetraccion"].Value = frm._EsAfectoDetraccion;
                                grdData.ActiveRow.Cells["d_Cantidad"].Value = "1";
                                //grdData.ActiveRow.Cells["d_Precio"].Value = frm._PrecioUnitario.ToString();
                                grdData.ActiveRow.Cells["d_Precio"].Value = DevolverPrecioProducto(frm._IdMoneda, frm._PrecioUnitario,
                                   int.Parse(cboMoneda.Value.ToString()),
                                   !string.IsNullOrEmpty(txtTipoCambio.Text.Trim())
                                       ? decimal.Parse(txtTipoCambio.Text.Trim())
                                       : 0);
                                grdData.ActiveRow.Cells["i_EsNombreEditable"].Value = frm._NombreEditable;
                                grdData.ActiveRow.Cells["i_EsServicio"].Value = frm._EsServicio.ToString();
                                grdData.ActiveRow.Cells["v_NroCuenta"].Value = frm._NroCuentaVenta;
                                grdData.ActiveRow.Cells["i_ValidarStock"].Value = frm._ValidarStock;
                                grdData.ActiveRow.Cells["i_EsAfectoPercepcion"].Value = frm._EsAfectoPercepcion;
                                grdData.ActiveRow.Cells["d_TasaPercepcion"].Value = frm._TasaPercepcion;
                                grdData.ActiveRow.Cells["v_CodigoInterno"].Activation = Activation.Disabled;
                                if (!string.IsNullOrWhiteSpace(frm._Observacion))
                                    grdData.ActiveRow.Cells["v_DescripcionProducto"].ToolTipText = frm._Observacion;
                            }
                        }
                        if (grdData.Rows.Any())
                        {
                            UltraGridCell aCell = grdData.Rows[e.Cell.Row.Index].Cells["d_Precio"];
                            grdData.Rows[e.Cell.Row.Index].Activate();
                            grdData.ActiveCell = aCell;
                            grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                            grdData.Focus();
                        }
                    }
                    break;

                case "i_IdCentroCosto":
                    Mantenimientos.frmBuscarDatahierarchy frm2 = new Mantenimientos.frmBuscarDatahierarchy(31, "Buscar Centro de Costos");
                    frm2.ShowDialog();
                    if (frm2._itemId != null)
                    {
                        grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = frm2._value2;
                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                    }
                    break;
            }
        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {

            grdData.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";


        }

        private void grdData_AfterCellActivate(object sender, EventArgs e)
        {
            CalcularValoresFila(grdData.ActiveRow);
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
                        try
                        {
                            Celda = grdData.ActiveCell;
                            Utils.Windows.NumeroDecimalCelda(Celda, e);

                            if (e.KeyChar == Convert.ToChar(Keys.Enter))
                            {
                                if (Celda.Row.Index == grdData.Rows.Count - 1)
                                    btnAgregar_Click(sender, e);
                                else
                                {
                                    grdData.Rows[Celda.Row.Index + 1].Cells["d_Precio"].Activate();
                                    grdData.PerformAction(UltraGridAction.EnterEditMode);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                        break;

                    case "d_PrecioVenta":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;

                    case "d_Descuento":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;

                    case "d_PorcentajeComision":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;

                    case "v_NroCuenta":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroEnteroCelda(Celda, e);
                        break;
                }
            }
        }

        private void grdData_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (e.Cell.Column.Key == "d_PrecioVenta" || e.Cell.Column.Key == "d_Cantidad" || e.Cell.Column.Key == "d_Precio" || e.Cell.Column.Key == "v_DescripcionProducto" || e.Cell.Column.Key == "v_NroCuenta" || e.Cell.Column.Key == "i_IdDestino" || e.Cell.Column.Key == "i_IdCentroCostos" || e.Cell.Column.Key == "i_IdAlmacen")
            {
                switch (e.Cell.Column.Key)
                {
                    case "v_NroCuenta":
                        //txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_NombreCuenta"].Value != null ? grdData.ActiveRow.Cells["_NombreCuenta"].Value.ToString() : string.Empty;
                        if (grdData.ActiveRow.Cells["EsRedondeo"].Value != null)
                            e.Cell.CancelUpdate();
                        break;

                    case "i_IdDestino":
                        //txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_Destino"].Value != null ? grdData.ActiveRow.Cells["_Destino"].Value.ToString() : string.Empty;
                        break;

                    case "i_IdCentroCostos":
                        //txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_CentroCosto"].Value != null ? grdData.ActiveRow.Cells["_CentroCosto"].Value.ToString() : string.Empty;
                        break;

                    case "i_IdAlmacen":
                        //txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_Almacen"].Value != null ? grdData.ActiveRow.Cells["_Almacen"].Value.ToString() : string.Empty;
                        if (grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null)
                        {
                            e.Cell.CancelUpdate();
                        }
                        //if (grdData.ActiveRow.Cells["EsRedondeo"].Value != null)
                        //    e.Cell.CancelUpdate();
                        break;

                    case "v_DescripcionProducto":
                        if (grdData.ActiveRow.Cells["i_EsNombreEditable"].Value != null)
                        {
                            if (grdData.ActiveRow.Cells["i_EsNombreEditable"].Value.ToString() == "0")
                            {
                                e.Cell.CancelUpdate();
                            }
                        }
                        break;

                    case "d_Precio":
                        if (grdData.ActiveRow.Cells["EsRedondeo"].Value != null)
                            e.Cell.CancelUpdate();
                        if (Globals.ClientSession.i_EditarPrecioVenta == 0)
                            e.Cell.CancelUpdate();

                        break;

                    case "d_Cantidad":
                        if (grdData.ActiveRow.Cells["EsRedondeo"].Value != null)
                            e.Cell.CancelUpdate();

                        break;

                    case "d_PrecioVenta":
                        if (grdData.ActiveRow.Cells["EsRedondeo"].Value != null)
                            e.Cell.CancelUpdate();

                        break;
                }
            }
            else
            {
                //txtDescripcionColumnas.Text = string.Empty;
            }
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (string.IsNullOrEmpty(_ventaDto.v_NroPedido) && grdData.ActiveRow.Activation != Activation.Disabled)
            {
                Point point = new System.Drawing.Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement = ((Infragistics.Win.UltraWinGrid.UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null) return;

                Infragistics.Win.UltraWinGrid.UltraGridRow row = (Infragistics.Win.UltraWinGrid.UltraGridRow)uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

                btnEliminar.Enabled = row != null;
            }
            else
            {
                btnEliminar.Enabled = false;
            }
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");
            if (e.Row.Cells["EsRedondeo"].Value != null)
                e.Row.Appearance.ForeColor = Color.Red;
            else if (e.Row.Cells["i_EsAfectoPercepcion"].Value != null && e.Row.Cells["i_EsAfectoPercepcion"].Value.ToString() == "1")
            {
                e.Row.Appearance.ForeColor = Color.Black;
                e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
            }
        }

        private void grdData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            try
            {
                if (e.Cell.Column.Key == "d_Cantidad" || e.Cell.Column.Key == "d_Precio")
                {
                    if (grdData.ActiveRow != null && e.Cell.Value != null)
                    {
                        UltraGridRow Fila = grdData.ActiveRow;
                        if (Fila.Cells["d_Precio"].Value == null || Fila.Cells["d_Cantidad"].Value == null) return;
                        CalcularValoresFila(Fila);
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_ventaDto.v_NroPedido))
            {
                if (grdData.ActiveRow == null || grdData.ActiveRow.Activation == Activation.Disabled)
                {
                    btnEliminar.Enabled = false;
                }
                else
                {
                    btnEliminar.Enabled = true;
                }
            }
        }
        #endregion

        private void btnCobrar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (_objVentasBL.DevolverTipoCambioPorFecha(ref objOperationResult, DateTime.Today.Date) == "0")
            {
                UltraMessageBox.Show("No se ha registrado ningún tipo de cambio para el día de hoy.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (_objVentasBL.TieneCobranzaPendiente(_ventaDto.v_IdVenta))
            {
                frmCobranzaRapida frm = new frmCobranzaRapida(_ventaDto.v_IdVenta, int.Parse(cboDocumento.Value.ToString()), IdIdentificacion);
                frm.ShowDialog();
                btnCobrar.Enabled = _objVentasBL.TieneCobranzaPendiente(_ventaDto.v_IdVenta);
            }
        }

        private void cboTipoVenta_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboTipoVenta.Value != null && int.Parse(cboTipoVenta.Value.ToString()) == 5)
            {
                List<KeyValueDTO> xx = (List<KeyValueDTO>)cboTipoOperacion.DataSource;

                if (xx != null && xx.Count(p => p.Id == "2") > 0)
                {
                    cboTipoOperacion.Value = "2";
                    cboTipoOperacion.Enabled = false;
                }
                else
                {
                    cboTipoOperacion.Enabled = true;
                }
            }
            else
            {
                cboTipoOperacion.Enabled = true;
            }
        }

        public bool VerificarLineaCreditoCliente(string IdCliente)
        {
            OperationResult objOperationResult = new OperationResult();
            decimal MontoOriginalVenta = 0;

            decimal MontoACargar = 0;

            MontoACargar = decimal.TryParse(txtTotal.Text.Trim(), out MontoACargar) ? MontoACargar : 0;
            MontoOriginalVenta = _ventaDto.d_Total ?? 0;

            var LineaCredito = new ClienteBL().DevuelveLineaCreditoCliente(ref objOperationResult, IdCliente);

            if (LineaCredito != null)
            {
                if ((LineaCredito.d_Saldo + MontoOriginalVenta) == 0 || (LineaCredito.d_Saldo + MontoOriginalVenta) < MontoACargar)
                {
                    UltraStatusbarManager.MarcarError(ultraStatusBar1, "El cliente no dispone de crédito suficiente para realizar esta compra.", timer1);
                    //txtSerieDoc.Enabled = false; //se deshabilita la serie y correlativo porque al salir de sus focos tmb validan y pueden deshabilitar o habilitar el btnGuardar tambien
                    //txtCorrelativoDocIni.Enabled = false;
                    btnGuardar.Enabled = false;
                    return false;
                }
                else
                {
                    UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
                    txtSerieDoc.Enabled = true;
                    btnGuardar.Enabled = true;
                    return true;
                }
            }
            else
            {
                UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
                txtSerieDoc.Enabled = true;
                btnGuardar.Enabled = true;
                return true;
            }
        }

        private void grdData_ClickCellButton(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "d_Precio")
            {
                string Id = grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null ? grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value.ToString() : string.Empty;
                int IdAlmacen = grdData.ActiveRow.Cells["i_IdAlmacen"].Value != null ? int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()) : -1;
                if (!string.IsNullOrEmpty(Id))
                {
                    var PopUp = new ucListaPreciosPopUp(ucListaPreciosPopUp.TipoVenta.VentaRapida, Id, IdAlmacen);
                    ultraPopupControlContainer1.Show();
                }
            }
        }

        public void FijarPrecio(string pstrPrecio, int IdMoneda)
        {
            if (grdData.ActiveRow != null)
            {
                grdData.ActiveRow.Cells["d_Precio"].Value = cboMoneda.Value == null || decimal.Parse(txtTipoCambio.Text) <= 0 ? "0.0" : IdMoneda == int.Parse(cboMoneda.Value.ToString()) ? pstrPrecio : int.Parse(cboMoneda.Value.ToString()) == (int)Currency.Soles ? (Utils.Windows.DevuelveValorRedondeado(decimal.Parse(pstrPrecio) * decimal.Parse(txtTipoCambio.Text), 2).ToString()) : (Utils.Windows.DevuelveValorRedondeado(decimal.Parse(pstrPrecio) / decimal.Parse(txtTipoCambio.Text), 2).ToString());

                ultraPopupControlContainer1.Close();
            }
        }

        void LlenarCuentasRubros()
        {
            OperationResult objOperationResult = new OperationResult();
            Task.Factory.StartNew(() =>
            {
                datahierarchyDto _datahierarchyDto = new datahierarchyDto();
                _datahierarchyDto = _objDatahierarchyBL.ObtenerDataHierarchyPorValue1(ref objOperationResult, 51, "mercadería");
                if (_datahierarchyDto != null)
                {
                    CtaRubroMercaderia = _datahierarchyDto.v_Value2;
                }

                _datahierarchyDto = _objDatahierarchyBL.ObtenerDataHierarchyPorValue1(ref objOperationResult, 51, "servicio");
                if (_datahierarchyDto != null)
                {
                    CtaRubroServicio = _datahierarchyDto.v_Value2;
                }
            }
            );
        }

        private void ultraDateTimeEditor1_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string TipoCambio = _objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.DateTime);
            txtTipoCambio.Text = TipoCambio;

            txtPeriodo.Text = dtpFechaRegistro.DateTime.Year.ToString();
            txtMes.Text = dtpFechaRegistro.DateTime.Month.ToString("00");
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Venta, _ventaDto.t_FechaRegistro, dtpFechaRegistro.DateTime, _ventaDto.v_Correlativo, 0);
            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(objOperationResult.ErrorMessage + "\n" + objOperationResult.ExceptionMessage,
                    objOperationResult.AdditionalInformation, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void ultraButton3_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["frmBandejaRegistroVenta"] != null)
            {
                var ventaBandejaForm = (frmBandejaRegistroVenta)Application.OpenForms["frmBandejaRegistroVenta"];
                var estado = ventaBandejaForm.MarcarFilaAnterior();
                if (!btnNuevoMovimiento.Enabled)
                    btnNuevoMovimiento.Enabled = estado;
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["frmBandejaRegistroVenta"] != null)
            {
                var ventaBandejaForm = (frmBandejaRegistroVenta)Application.OpenForms["frmBandejaRegistroVenta"];
                var estado = ventaBandejaForm.MarcarFilaSiguiente();
                if (!btnNuevoMovimiento.Enabled)
                    btnNuevoMovimiento.Enabled = estado;
            }
        }

        private void cboMonedaCobranza_ValueChanged(object sender, EventArgs e)
        {
            //if (cboMonedaCobranza.Value.ToString() == "-1") return;
            //var ds = _objVentasBL.ObtenerFormasPagoPorMoneda(int.Parse(cboMonedaCobranza.Value.ToString()), UserConfig.Default.MostrarSoloFormasPagoAlmacenActual);
            //if (ds.Any())
            //{
            //    cboFormaPago.DataSource = ds;
            //    var predetEfectivo = ds.FirstOrDefault(p => p.Value1 != null && p.Value1.Contains("EFECTIVO"));
            //    cboFormaPago.Value = predetEfectivo != null ? predetEfectivo.Id : "-1";
            //}

            //txtMontoCobrar.Text = CalcularMontoCobrar(int.Parse(cboMoneda.Value.ToString()),
            //    !string.IsNullOrEmpty(txtTotal.Text.Trim()) ? decimal.Parse(txtTotal.Text) : 0, int.Parse(cboMonedaCobranza.Value.ToString()),
            //        decimal.Parse(txtTipoCambio.Text));
        }

        private void cboCondicionPago_ValueChanged(object sender, EventArgs e)
        {
            if (!cboCondicionPago.Text.Contains("CONTADO"))
            {
                chkCobranzaMixta.Checked = false;
                chkCobranzaMixta.Enabled = false;
                gbOpcionesCobranza.Enabled = false;
                btnCobrar.Visible = false;
                cboFormaPago.Value = "-1";
                cboFormaPago.Enabled = false;
                txtNroOperacion.Enabled = true;
                txtNroOperacion.Clear();
                cboFormaPago.Visible = false;
                txtNroOperacion.Visible = true;
            }
            else
            {
                chkCobranzaMixta.Enabled = true;
                gbOpcionesCobranza.Enabled = true;
                btnCobrar.Visible = chkCobranzaMixta.Checked;
                cboFormaPago.Enabled = true;
                var ds = (List<KeyValueDTO>)cboFormaPago.DataSource;
                var predetEfectivo = ds.FirstOrDefault(p => p.Value1 != null && p.Value1.Contains("EFECTIVO SOLES"));
                cboFormaPago.Value = predetEfectivo != null ? predetEfectivo.Id : "-1";
                cboFormaPago.Visible = true;
                txtNroOperacion.Visible = true;
            }

        }

        private void chkCobranzaMixta_CheckedChanged(object sender, EventArgs e)
        {
            gbOpcionesCobranza.Enabled = !chkCobranzaMixta.Checked;
            btnCobrar.Visible = chkCobranzaMixta.Checked;
        }

        private void cboFormaPago_ValueChanged(object sender, EventArgs e)
        {
            bool formaPagoRequiereNroDocumento;
            new FormaPagoDocumentoBL().DevuelveComprobantePorFormaPago(int.Parse(cboFormaPago.Value.ToString()), out formaPagoRequiereNroDocumento, out _idMonedaCobranza);

            txtNroOperacion.Enabled = formaPagoRequiereNroDocumento;
            txtPagaCon.Enabled = cboFormaPago.Text.Contains("EFECTIVO");
            chkCobranzaMixta.Checked = cboFormaPago.Text.Contains("MIXTO");
        }

        public string CalcularMontoCobrar(int pintIdMonedaVenta, decimal pdecTotalVenta, int pintIdMonedaCobranza, decimal pdecTipoCambio)
        {
            try
            {
                if (pintIdMonedaCobranza != pintIdMonedaVenta)
                {
                    if (pintIdMonedaCobranza == 1)
                        return Utils.Windows.DevuelveValorRedondeado((pdecTotalVenta * pdecTipoCambio), 2).ToString("0.00");

                    return Utils.Windows.DevuelveValorRedondeado((pdecTotalVenta / pdecTipoCambio), 2).ToString("0.00");
                }

                return pdecTotalVenta.ToString("0.00");
            }
            catch
            {
                return "0.00";
            }
        }

        private void txtTipoCambio_ValueChanged(object sender, EventArgs e)
        {
            if (txtMontoCobrar.Text !="0.0")
            {
                txtMontoCobrar.Text = CalcularMontoCobrar(int.Parse(cboMoneda.Value.ToString()),
                !string.IsNullOrEmpty(txtTotal.Text.Trim()) ? decimal.Parse(txtTotal.Text) : 0, int.Parse(cboMonedaCobranza.Value.ToString()),
                !string.IsNullOrEmpty(txtTipoCambio.Text) ? decimal.Parse(txtTipoCambio.Text) : 0);
            }
            
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtPagaCon_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                decimal monto, pago;
                if (decimal.TryParse(txtMontoCobrar.Text, out monto) && decimal.TryParse(txtPagaCon.Text, out pago))
                    UltraStatusbarManager.Mensaje(ultraStatusBar1,
                        string.Format("Vuelto: {0}{1}", cboMonedaCobranza.Value.ToString() == "1" ? "S/" : "$",
                            Utils.Windows.DevuelveValorRedondeado((decimal)(float)(pago - monto), 2)), timer1);
                else
                    UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
            }
            catch
            {
                UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
            }
        }

        private void grdData_AfterEnterEditMode(object sender, EventArgs e)
        {
            if (grdData.ActiveCell != null && grdData.ActiveCell.Column.Key.Equals("d_PrecioVenta"))
            {
                decimal.TryParse(grdData.ActiveCell.Value.ToString(), out valorOriginalCelda);
            }
        }

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            if (grdData.ActiveCell.Column.Key == "v_FacturaRef")
            {
                if (grdData.ActiveCell.Value != null)
                {
                    grdData.ActiveCell.Value = Utils.Windows.DarFormatoDescuento(grdData.ActiveCell.Value.ToString());
                    CalcularValoresFila(grdData.ActiveRow);
                }
                else
                    grdData.ActiveCell.Value = "0";
            }

            if (grdData.ActiveCell != null && grdData.ActiveCell.Column.Key.Equals("d_PrecioVenta"))
            {
                try
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
                    var celdaDscto = grdData.ActiveCell.Row.Cells["v_FacturaRef"];
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
                    var celdaPrecio = grdData.ActiveCell.Row.Cells["d_Precio"];

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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void grdData_BeforeRowInsert(object sender, BeforeRowInsertEventArgs e)
        {
            var filasActuales = grdData.Rows.Count;
            if (filasActuales > _limiteDocumento - 1)
            {
                e.Cancel = true;
                UltraStatusbarManager.MarcarError(ultraStatusBar1, "Máximo de items por documento alcanzado!", timer1);
            }

            else
                UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
        }

        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            
            //if (e.KeyCode != Keys.Enter) return;
            //var code = txtCodigo.Text.Trim();
            //if (code == string.Empty) return;
            //var row = (from r in grdData.Rows
            //           let val = r.GetCellValue("v_CodigoInterno")
            //           where val != null && val.ToString() == code
            //           select r).FirstOrDefault();
            //if (row == null)
            //{
            //    try
            //    {
            //        InvokeOnClick(btnAgregar, e);
            //        row = grdData.GetRow(ChildRow.Last);
            //        row.Cells["v_CodigoInterno"].Value = code;
            //        grdData_DoubleClickCell(grdData, new DoubleClickCellEventArgs(row.Cells["v_CodigoInterno"]));
            //    }
            //    catch
            //    {
            //        return;
            //    }
            //}
            //else
            //{
            //    var cant = row.GetCellValue("d_Cantidad") ?? 0M;
            //    row.Cells["d_Cantidad"].Value = (decimal.Parse(cant.ToString()) + 1M).ToString(CultureInfo.InvariantCulture);
            //}

            //txtCodigo.Clear();
            //txtCodigo.Focus();
        }

        private void btnViewCode_Click(object sender, EventArgs e)
        {
            //var vs = !txtCodigo.Visible;
            //ulCodigo.Visible = txtCodigo.Visible = vs;
            //if (vs) txtCodigo.Focus();
        }


        private decimal DevolverPrecioProducto(int pintIdMonedaBusqueda, decimal pdecPrecio, int pintIdMonedaVenta,
           decimal pdecTipoCambioVenta)
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

                return pdecPrecio;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        private void cboDocumentoRef_Leave(object sender, EventArgs e)
        {
            if (strModo == "Nuevo")
            {
                if (cboDocumentoRef.Text.Trim() == "")
                {
                    cboDocumentoRef.Value = "-1";
                }
                else
                {
                    var x =
                        _ListadoComboDocumentosRef.Find(
                            p => p.Id == cboDocumentoRef.Value.ToString() || p.Id == cboDocumentoRef.Text);
                    if (x == null)
                    {
                        cboDocumentoRef.Value = "-1";
                    }

                    txtSerieDocRef.Text =
                        _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento,
                            int.Parse(cboDocumentoRef.Value.ToString())).Trim();

                    if (cboDocumento.Value != null && !string.IsNullOrEmpty(UserConfig.Default.SerieCaja) && UserConfig.Default.SerieCaja != "--Seleccionar--")
                    {
                        txtSerieDocRef.Text = UserConfig.Default.SerieCaja;
                    }
                }

            }
        }

        private void txtSerieDocRef_Validated(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSerieDocRef.Text))
            {
                int Serie = 0;
                if (int.TryParse(txtSerieDocRef.Text.Trim(), out Serie))
                {
                    Utils.Windows.FijarFormatoUltraTextBox(txtSerieDocRef, "{0:0000}");
                }
                else
                    txtSerieDocRef.Text = txtSerieDocRef.Text.Trim();
            }
        }

        private void txtCorrelativoDocRef_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDocRef, "{0:00000000}");
            if (!grdData.Rows.Any())
                DevuelveVentaReferencia(true);
            else if (
                UltraMessageBox.Show(
                    "Ya se ha cargado un documento previamente, ¿Desea cargar este otro documento?", "Sistema",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.No)
                DevuelveVentaReferencia(false);
            else
            {
                LimpiarDetalle();
                DevuelveVentaReferencia(true);
            }
        }

        private void DevuelveVentaReferencia(bool Respuesta)
        {
            if (Respuesta)
            {
                if (txtSerieDocRef.Text.Trim() != "" && txtCorrelativoDocRef.Text.Trim() != "" &&
                    cboDocumentoRef.Value.ToString() != "-1")
                {
                    ventaDto ventaRefDto = new ventaDto();
                    OperationResult objOperationResult = new OperationResult();
                    var TieneNotaCredito = _objVentasBL.VentaYaTieneNRC(ref objOperationResult,
                        int.Parse(cboDocumentoRef.Value.ToString()), txtSerieDocRef.Text, txtCorrelativoDocRef.Text);

                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage,
                            @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (TieneNotaCredito)
                    {
                        var resp =
                            MessageBox.Show(@"Esta venta ya cuenta con una nota de crédito activa. ¿Desea Continuar?",
                                @"Validación",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (resp == DialogResult.No) return;
                    }

                    var idCompraRef = _objVentasBL.DevolverIdVenta(ref objOperationResult,
                        int.Parse(cboDocumentoRef.Value.ToString()), txtSerieDocRef.Text, txtCorrelativoDocRef.Text);
                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage,
                            @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    ventaRefDto = _objVentasBL.ObtenerVentaCabecera(ref objOperationResult, idCompraRef);
                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage,
                            @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (ventaRefDto != null)
                    {
                        txtTipoCambio.Text = ventaRefDto.d_TipoCambio == null ? "0.000" : ventaRefDto.d_TipoCambio.Value.ToString();
                        cboMoneda.Value = ventaRefDto.i_IdMoneda.ToString();
                        txtAnticipio.Text = ventaRefDto.d_Anticipio.ToString();
                        txtDescuento.Text = ventaRefDto.d_Descuento.ToString();
                        txtIGV.Text = ventaRefDto.d_IGV.ToString();
                        txtConcepto.Text = ventaRefDto.v_Concepto;
                        txtTotal.Text = ventaRefDto.d_Total.ToString();
                        txtValorVenta.Text = ventaRefDto.d_ValorVenta.ToString();
                        txtRucCliente.Text = ventaRefDto.NroDocCliente;
                        txtRazonSocial.Text = ventaRefDto.NombreCliente;
                        dtpFechaRef.Value = ventaRefDto.t_FechaVencimiento.Value;
                        cboEstablecimiento.Value = ventaRefDto.i_IdEstablecimiento.ToString();
                        cboIGV.Value = ventaRefDto.i_IdIgv.ToString();
                        cboMoneda.Value = ventaRefDto.i_IdMoneda.ToString();
                        cboTipoOperacion.Value = ventaRefDto.i_IdTipoOperacion.ToString();
                        cboTipoVenta.Value = int.Parse(ventaRefDto.i_IdTipoVenta.ToString()).ToString("00");
                        if (!string.IsNullOrWhiteSpace(ventaRefDto.v_IdVendedor))
                            cboVendedor.Value = ventaRefDto.v_IdVendedor;
                        chkAfectoIGV.Checked = ventaRefDto.i_EsAfectoIgv == 1;
                        chkDeduccionAnticipio.Checked = ventaRefDto.i_DeduccionAnticipio == 1;
                        chkPrecInIGV.Checked = ventaRefDto.i_PreciosIncluyenIgv == 1;
                        _ventaDto.v_IdCliente = ventaRefDto.v_IdCliente;
                        CargarDetalle(idCompraRef);
                        if (grdData.Rows.Any())
                        {
                            foreach (var fila in grdData.Rows)
                            {
                                //cambio los estados de las filas devueltas como temporales y modificadas para que al llenarse los temporales se inserten en el presente doc.
                                fila.Cells["i_RegistroTipo"].Value = "Temporal";
                                fila.Cells["i_RegistroEstado"].Value = "Modificado";
                            }
                        }
                        btnGuardar.Enabled = true;
                    }
                    else
                    {
                        UltraMessageBox.Show("No se ha registrado el documento de referencia", "Sistema");
                        LimpiarCabecera();
                        CargarDetalle("");
                        btnGuardar.Enabled = false;
                    }
                }
            }
        }

        private void cboDocumento_Validated(object sender, EventArgs e)
        {
            var tipdoc = cboDocumento.Value as string;
            if (tipdoc == "7" || tipdoc == "8")
            {
                cboDocumentoRef.Focus();
                cboCondicionPago.Value = "2";
                cboCondicionPago.Enabled = false;
            }
            else
            {
                //cboCondicionPago.Value = "1";
                cboCondicionPago.Value = Globals.ClientSession.i_IdCondicionPagoVenta == -1 ? 1 : Globals.ClientSession.i_IdCondicionPagoVenta;
                cboCondicionPago.Enabled = true;
            }
        }

        private void cboVendedor_Leave(object sender, EventArgs e)
        {
            if (cboVendedor.Value != null)
            {
                if (cboVendedor.Text.Trim() == "")
                {
                    cboVendedor.Value = "-1";
                }
                else
                {
                    var x = ListaVendedores.Find(p => p.Id == cboVendedor.Value.ToString() || p.Id == cboVendedor.Text);
                    if (x == null)
                    {
                        cboVendedor.Value = "-1";
                    }
                }

            }
        }

        private void cboVendedor_Validated(object sender, EventArgs e)
        {
            if (cboVendedor.Value == null)
            {
                cboVendedor.Value = "-1";
            }
        }

        private void cboVendedor_ValueChanged(object sender, EventArgs e)
        {
            if (cboVendedor.Value == null)
            {

            }
            else if (cboVendedor.Value.ToString() != "-1" && cboVendedor.SelectedItem != null)
            {
                var x = (KeyValueDTO)cboVendedor.SelectedItem.ListObject;
                if (x == null) return;
            }
        }

        private void ucTipoNota_ValueChanged(object sender, EventArgs e)
        {
            if (ucTipoNota.Value == null) return;
            txtConcepto.NullText = @"Ingrese motivo de la Nota Electronica";
            txtConcepto.Text = ((Infragistics.Win.UltraWinEditors.UltraComboEditor)sender).Text;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.F1:
                    cboDocumento.Focus();
                    return true;

                case Keys.F2:
                    txtRucCliente.Focus();
                    return true;

                case Keys.F3:
                    cboVendedor.Focus();
                    return true;

                case Keys.F4:
                    cboFormaPago.Focus();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void txtCodigo_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnBuscarServicios_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbHospitalizacion.Checked)
                {
                    var frm = new frmFacturarHospitalizacion();
                    var r = frm.ShowDialog();

                    if (r == System.Windows.Forms.DialogResult.OK)
                    {
                        var detalle = frm.ListadoVentaDetalle;

                        grdData.DataSource = detalle;

                        for (int i = 0; i < grdData.Rows.Count(); i++)
                        {
                            grdData.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
                            grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
                            grdData.Rows[i].Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                        }

                        CalcularValoresDetalle();
                        ultraGroupBox1.Enabled = false;
                    }

                }
                #region ....
                if (rbAsistencial.Checked)
                {
                    var f = new frmBuscarServiciosPendientesAsistencial(txtRucCliente.Text, txtRazonSocial.Text, TipoFacturacion.Asistencial, txtRucCliente.Text);
                    var r = f.ShowDialog();
                    if (r == System.Windows.Forms.DialogResult.OK)
                    {
                        var detalle = f.ListadoVentaDetalle;
                        _ServiciosSeleccionadosSigesoft = VentaBL.ObtenerServicios(f.ServiciosSeleccionados);
                        grdData.DataSource = detalle;

                        for (int i = 0; i < grdData.Rows.Count(); i++)
                        {
                            grdData.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
                            grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
                            grdData.Rows[i].Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                        }

                        CalcularValoresDetalle();
                        ultraGroupBox1.Enabled = false;
                    }
                }
                else
                {
                    if (!Utils.Windows.ValidarRuc(txtRucCliente.Text)) return;
                    TipoFacturacion tipoFacturacion;
                    if (rbAseguradora.Checked)
                    {
                        tipoFacturacion = TipoFacturacion.Aseguradora;
                    }
                    else if (rbOcupacional.Checked)
                    {
                        tipoFacturacion = TipoFacturacion.Ocupacional;
                    }
                    else
                    {
                        tipoFacturacion = TipoFacturacion.Hospitalizacion;
                    }

                    var f = new frmBuscarServiciosPendientes(tipoFacturacion, txtRucCliente.Text, txtRazonSocial.Text);
                    var r = f.ShowDialog();
                    if (r == System.Windows.Forms.DialogResult.OK)
                    {
                        var detalle = f.ListadoVentaDetalle;
                        tipoFact = f.Tipofact;
                        listaServicios = f.ServiciosSeleccionados;
                        _ServiciosSeleccionadosSigesoft = VentaBL.ObtenerServicios(f.ServiciosSeleccionados);
                        _nroLiquidacion = f._nroLiquidacion;
                        grdData.DataSource = detalle;

                        for (int i = 0; i < grdData.Rows.Count(); i++)
                        {
                            grdData.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
                            grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
                            grdData.Rows[i].Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                        }

                        CalcularValoresDetalle();
                        ultraGroupBox1.Enabled = false;
                    }
                }


                //var f = new frmBuscarServiciosPendientes(txtRucCliente.Text, txtRazonSocial.Text);
                //var r = f.ShowDialog();
                //if (r == DialogResult.OK)
                //{
                //    var detalle = f.ListadoVentaDetalle;
                //    tipoFact = f.Tipofact;
                //    listaServicios = f.ServiciosSeleccionados;

                //    _ServiciosSeleccionadosSigesoft = VentaBL.ObtenerServicios(f.ServiciosSeleccionados);
                //    grdData.DataSource = detalle;

                //    for (int i = 0; i < grdData.Rows.Count(); i++)
                //    {
                //        grdData.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
                //        grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
                //        grdData.Rows[i].Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                //    }

                //    CalcularValoresDetalle();
                //    ultraGroupBox1.Enabled = false;
                //}
                #endregion
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private BindingList<ventadetalleDto> servicios;
        private void btnAgendar_Click(object sender, EventArgs e)
        {

            var frmBandejaAgenda = new SAMBHS.Windows.SigesoftIntegration.UI.frmBandejaAgenda(""); 
            if (frmBandejaAgenda.ShowDialog() != DialogResult.OK) return;
            servicios = frmBandejaAgenda.ListadoVentaDetalle;
            txtRucCliente.Text = servicios[0].RucEmpFacturacion;
            grdData.DataSource = frmBandejaAgenda.ListadoVentaDetalle;
            for (var i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
                grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
                grdData.Rows[i].Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
            }
            CalcularValoresDetalle();
            ultraGroupBox1.Enabled = false;
        }

        private void btnFarmacia_Click(object sender, EventArgs e)
        {
            
            var frm = new FrmBuscarMedicamento(txtRucCliente.Text);
            if (frm.ShowDialog() != DialogResult.OK) return;
            grdData.DataSource = frm.ListadoVentaDetalle;
            servicio_ = frm.service;
            ticket_ = frm.ticket;
            for (var i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
                grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
                grdData.Rows[i].Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
            }
            CalcularValoresDetalle();
            ultraGroupBox1.Enabled = false;
            MessageBox.Show(@"Se agregó correctamente.", @"Información");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (new LoadingClass.PleaseWait(this.Location, "Generando..."))
            {
                this.Enabled = false;

                string ruta = GetApplicationConfigValue("rutaLiquidacion").ToString();
                string nombre = "EGRESOS DE PRUEBA - CSL";
                //Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", );
                this.Enabled = true;
            }
        }

        private void txtCorrelativoDocIni_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtSerieDoc_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cboTipoServicio_ValueChanged(object sender, EventArgs e)
        {
            if (cboTipoServicio.Value.ToString() == "6")
            {
                ulCodigo.Visible = true;
                txtCodigo.Visible = true;
                btnViewCode.Visible = true;
            }
            else
            {
                ulCodigo.Visible = false;
                txtCodigo.Visible = false;
                btnViewCode.Visible = false;
            }
        }

        private void cboDocumento_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void txtCodigo_Click(object sender, EventArgs e)
        {
            
        }

        private void txtCodigo_Enter(object sender, EventArgs e)
        {
            txtCodigo.Text = "N009-0000";
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            cboDocumento.Value = "3";
            txtSerieDoc.Text = "B001";
            PredeterminarEstablecimiento(txtSerieDoc.Text);
            _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            CancelEventArgs e1 = new CancelEventArgs();
            this.txtSerieDoc_Validating(sender, e1);
            cboTipoServicio.Value = "2";
        }

        private void ultraButton4_Click(object sender, EventArgs e)
        {
            cboDocumento.Value = "1";
            txtSerieDoc.Text = "F001";
            PredeterminarEstablecimiento(txtSerieDoc.Text);
            _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            CancelEventArgs e1 = new CancelEventArgs();
            this.txtSerieDoc_Validating(sender, e1);
            cboTipoServicio.Value = "2";
        }

        private void ultraButton5_Click(object sender, EventArgs e)
        {
            cboDocumento.Value = "3";
            txtSerieDoc.Text = "B002";
            PredeterminarEstablecimiento(txtSerieDoc.Text);
            _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            CancelEventArgs e1 = new CancelEventArgs();
            this.txtSerieDoc_Validating(sender, e1);
            cboTipoServicio.Value = "1";
        }

        private void ultraButton6_Click(object sender, EventArgs e)
        {
            cboDocumento.Value = "1";
            txtSerieDoc.Text = "F002";
            PredeterminarEstablecimiento(txtSerieDoc.Text);
            _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            CancelEventArgs e1 = new CancelEventArgs();
            this.txtSerieDoc_Validating(sender, e1);
            cboTipoServicio.Value = "1";
        }

        private void ultraButton7_Click(object sender, EventArgs e)
        {
            cboDocumento.Value = "506";
            txtSerieDoc.Text = "RSL";
            PredeterminarEstablecimiento(txtSerieDoc.Text);
            _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            CancelEventArgs e1 = new CancelEventArgs();
            this.txtSerieDoc_Validating(sender, e1);
            cboTipoServicio.Value = "2";
        }

        private void ultraButton8_Click(object sender, EventArgs e)
        {
            cboDocumento.Value = "502";
            txtSerieDoc.Text = "ECA";
            PredeterminarEstablecimiento(txtSerieDoc.Text);
            _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            CancelEventArgs e1 = new CancelEventArgs();
            this.txtSerieDoc_Validating(sender, e1);
            cboTipoServicio.Value = "2";
        }

        private void ultraButton9_Click(object sender, EventArgs e)
        {
            cboDocumento.Value = "3";
            txtSerieDoc.Text = "B003";
            PredeterminarEstablecimiento(txtSerieDoc.Text);
            _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            CancelEventArgs e1 = new CancelEventArgs();
            this.txtSerieDoc_Validating(sender, e1);
            cboTipoServicio.Value = "3";
        }

        private void ultraButton10_Click(object sender, EventArgs e)
        {
            cboDocumento.Value = "1";
            txtSerieDoc.Text = "F003";
            PredeterminarEstablecimiento(txtSerieDoc.Text);
            _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            CancelEventArgs e1 = new CancelEventArgs();
            this.txtSerieDoc_Validating(sender, e1);
            cboTipoServicio.Value = "3";
        }
    }
}

