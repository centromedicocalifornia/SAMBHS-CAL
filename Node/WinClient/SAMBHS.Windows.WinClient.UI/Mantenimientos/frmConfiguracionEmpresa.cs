using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Almacen.BL;
using Infragistics.Win.UltraWinGrid;
using SAMBHS.Security.BL;
using System.Diagnostics;
using System.IO;
using SAMBHS.Cobranza.BL;
using SAMBHS.Compra.BL;
using SAMBHS.Tesoreria.BL;
using System.Collections.Generic;
using SAMBHS.Venta.BL;

using SAMBHS.Planilla.BL;

namespace SAMBHS.Windows.WinClient.UI.Mantenimientos
{
    public partial class frmConfiguracionEmpresa : Form
    {
        private readonly UltraCombo _ucDestino = new UltraCombo();
        private OperationResult _objOperationResult = new OperationResult();
        EstablecimientoBL _objEstablecimientoBL = new EstablecimientoBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        ConfiguracionEmpresaBL _objConfiguracionEmpresaBL = new ConfiguracionEmpresaBL();
        PedidoBL objPedidoBL = new PedidoBL();
        string _strFilterExpression, _mode;
        string IdRepresentanteLegal = null, ImagenPathLogo = string.Empty, ImagePathFirma;
        int i_IdConfiguracionEmpresa;
        configuracionempresaDto _configuracionempresaDto;
        SecurityBL _objSecurityBL = new SecurityBL();
        bool _btnGuardar;
        Byte[] Logo = null, Firma = null;
        public frmConfiguracionEmpresa(string arg)
        {
            InitializeComponent();
        }

        private void frmConfiguracionEmpresa_Load(object sender, EventArgs e)
        {
            BackColor = new GlobalFormColors().FormColor;

            #region ControlAcciones
            var _formActions = _objSecurityBL.GetFormAction(ref _objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmConfiguracionEmpresa", Globals.ClientSession.i_RoleId);
            _btnGuardar = Utils.Windows.IsActionEnabled("frmConfiguraciobEmpresa_Save", _formActions);
            #endregion

            try
            {
                OperationResult objOperationResult = new OperationResult();
                List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
                _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosCobranzaParaComboGrid(ref objOperationResult, null);
                #region Cargar Combos
                _ucDestino.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
                //ucAlmacen.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
                Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref _objOperationResult, 18, null), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cbMonedaCompras, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref _objOperationResult, 18, null), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboDestino, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchyForComboWithIDValueDto(ref _objOperationResult, 24, null), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboTipoOperacion, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchyForComboWithIDValueDto(ref _objOperationResult, 35, null), DropDownListAction.Select);
                cboTipoOperacion2.DataSource = _objDatahierarchyBL.GetDataHierarchyForComboKeyValueDto(ref _objOperationResult, 35, "v_Value2");
                Utils.Windows.LoadUltraComboEditorList(cboIGV, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref _objOperationResult, 27, null), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboPermiteStockNegativo, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref _objOperationResult, 40, null), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref _objOperationResult, null, Globals.ClientSession.i_CurrentExecutionNodeId), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboEstablecimientoPredet, "Value1", "Id", _objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref _objOperationResult, null), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboDocumento, "Value1", "Id", _objDocumentoBL.ObtenDocumentosParaCombo(ref _objOperationResult, null, 0, 1), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboMonedaImportacion, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref _objOperationResult, 18, null), DropDownListAction.Select);
                var libros = new DocumentoBL().ObtenDocumentosLibroDiarioParaComboGrid(ref _objOperationResult);
                Utils.Windows.LoadUltraComboList(cboDocumentoLetrasCobrar, "Value2", "Id", libros, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboList(cboDocumentoLetrasPagar, "Value2", "Id", libros, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboTipoRegimenEmpresa, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref _objOperationResult, 152, null).OrderBy(l => l.Id).ToList(), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboCodigoPlanContable, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref _objOperationResult, 154, null).OrderBy(l => l.Id).ToList(), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboSeriePredeterminada, "Value1", "Id", _objEstablecimientoBL.ObtenerSerieEstablecimiento(ref _objOperationResult, Globals.ClientSession.i_IdEstablecimiento.Value).OrderBy(l => l.Value1).ToList(), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboList(cboDocumentoCajaChica, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboTipoVentaVentas, "Value1", "Id", Globals.CacheCombosVentaDto.cboTipoVenta, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboCondicionPagoPedido, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 41, null), DropDownListAction.Select);


                Utils.Windows.LoadUltraComboEditorList(cboCondicionPagoVenta, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 23, null), DropDownListAction.Select);

                cboSeriePredeterminada.Value = "-1";
                cboDocumentoCajaChica.Value = "-1";
                cboTipoVentaVentas.Value = "-1";

                cboCondicionPagoVenta.Value = "-1";
                //cboTipoOperacion2.GetRow(ChildRow.Last).Hidden = true;
                foreach (var row in cboTipoOperacion2.Rows)
                    row.Cells["check"].Value = (string)row.Cells["Value5"].Value == "1";
                btnRegenerarTesoreriasDesdeCobranzaDetalle.Visible = Globals.ClientSession.i_SystemUserId == 1;
                #endregion

                #region Cargar Load
                Cargar();
                #endregion
            }
            catch
            {
                UltraMessageBox.Show("Hubo un error al cargar la Configuracion de Empresa", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        #region Cabecera
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            var numListaPrecios = new ListaPreciosBL().VerificarNumeroListaPrecios(ref objOperationResult);
            if (chkSeUsara1ListaPreciosEmpresa.Checked && numListaPrecios > 1)
            {
                UltraMessageBox.Show("No se puede marcar  " + "Se usará una sola Tarifario en la Empresa.\n" + "La empresa ya cuenta con varias Tarifario.\n", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                chkSeUsara1ListaPreciosEmpresa.Checked = false;
                return;
            }


            if (chkEmpresaAfectaRetencion.Checked)
            {
                if (string.IsNullOrEmpty(txtTasaRetencion.Text) || string.IsNullOrEmpty(txtNroCuentaRetencion.Text))
                {
                    UltraMessageBox.Show("Porfavor Ingrese la Tasa y la Cuenta para la Retención", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTasaRetencion.Focus();
                    return;
                }
            }

            if (chkRedondearVentas.Checked)
            {
                if (string.IsNullOrEmpty(txtRedondeoPerdida.Text.Trim()) || string.IsNullOrEmpty(txtRedondeoGanancia.Text.Trim()))
                {
                    UltraMessageBox.Show("Porfavor Ingrese las cuentas para ambos redondeos correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (uvConfiguracionEmpresa.Validate(true, false).IsValid)
            {
                UserConfig.Default.NroTicket = txtNroTicket.Text;
                UserConfig.Default.Save();
                SaveOperaciones();
                if (_mode == "New")
                {
                    _configuracionempresaDto = new configuracionempresaDto();

                    _configuracionempresaDto.i_IdMoneda = Globals.ClientSession.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                    _configuracionempresaDto.i_AfectoIgvCompras = Globals.ClientSession.i_AfectoIgvCompras = chkAfectoIGVCom.Checked ? 1 : 0;
                    _configuracionempresaDto.i_PrecioIncluyeIgvCompras = Globals.ClientSession.i_PrecioIncluyeIgvCompras = chkPrecInIGVCom.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IdDestinoCompras = Globals.ClientSession.i_IdDestinoCompras = int.Parse(cboDestino.Value.ToString());
                    _configuracionempresaDto.i_AfectoIgvVentas = Globals.ClientSession.i_AfectoIgvVentas = chkAfectoIGVVen.Checked ? 1 : 0;
                    _configuracionempresaDto.i_PrecioIncluyeIgvVentas = Globals.ClientSession.i_PrecioIncluyeIgvVentas = chkPrecInIGVVen.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IdTipoOperacionVentas = Globals.ClientSession.i_IdTipoOperacionVentas = int.Parse(cboTipoOperacion.Value.ToString());
                    _configuracionempresaDto.i_CantidadDecimales = Globals.ClientSession.i_CantidadDecimales = int.Parse(txtNumeroDecimalesCantidad.Text);
                    _configuracionempresaDto.i_PrecioDecimales = Globals.ClientSession.i_PrecioDecimales = int.Parse(txtNumeroDecimalesPrecio.Text);
                    _configuracionempresaDto.d_ValorMaximoBoletas = Globals.ClientSession.d_ValorMaximoBoletas = decimal.Parse(txtMontoMaximoBoletas.Text);
                    _configuracionempresaDto.i_IdPermiteStockNegativo = Globals.ClientSession.i_IdPermiteStockNegativo = int.Parse(cboPermiteStockNegativo.Value.ToString());
                    _configuracionempresaDto.i_IdIgv = Globals.ClientSession.i_IdIgv = int.Parse(cboIGV.Value.ToString());
                    _configuracionempresaDto.d_ComisionVendedor = Globals.ClientSession.d_ComisionVendedor = decimal.Parse(txtPorcentajeComisionVendedor.Text);
                    _configuracionempresaDto.v_IdCuentaContableSoles = Globals.ClientSession.v_IdCuentaContableSoles = TxtCuentaContableSoles.Text;
                    _configuracionempresaDto.v_IdCuentaContableDolares = Globals.ClientSession.v_IdCuentaContableDolares = TxtCuentaContableDolares.Text;
                    _configuracionempresaDto.i_IdAlmacenPredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado = int.Parse(cboAlmacen.Value.ToString());
                    _configuracionempresaDto.i_IncluirPreciosGuiaRemision = Globals.ClientSession.i_IncluirPreciosGuiaRemision = ChkIncluirPrecios.Checked ? 1 : 0;
                    _configuracionempresaDto.i_FechaRegistro = Globals.ClientSession.i_FechaRegistro = rbtnFechaRegistro.Checked ? 1 : 0;
                    _configuracionempresaDto.i_FechaEmision = Globals.ClientSession.i_FechaEmision = rbtnFechaEmision.Checked ? 1 : 0;
                    _configuracionempresaDto.i_EmpresaAfectaRetencion = Globals.ClientSession.i_EmpresaAfectaRetencion = chkEmpresaAfectaRetencion.Checked ? 1 : 0;
                    _configuracionempresaDto.v_NroCuentaRetencion = Globals.ClientSession.v_NroCuentaRetencion = txtNroCuentaRetencion.Text;
                    _configuracionempresaDto.i_VentasMostrarColumasEmpaque = Globals.ClientSession.i_VentasMostrarEmpaque = chkMostrarEmpaqueVentas.Checked ? 1 : 0;
                    _configuracionempresaDto.i_ComprasMostrarColumasEmpaque = Globals.ClientSession.i_ComprasMostrarEmpaque = chkMostrarEmpaqueCompras.Checked ? 1 : 0;
                    _configuracionempresaDto.d_TasaRetencion = Globals.ClientSession.d_TasaRetencion = decimal.Parse(txtTasaRetencion.Text.Trim());
                    _configuracionempresaDto.i_RedondearVentas = Globals.ClientSession.i_RedondearVentas = chkRedondearVentas.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IdMonedaCompra = Globals.ClientSession.i_IdMonedaCompra = int.Parse(cbMonedaCompras.Value.ToString());
                    _configuracionempresaDto.i_EditarPrecioPedido = Globals.ClientSession.i_PrecioEditableTodosProductosPedido = chkEditarPreciosUnitariosPedido.Checked ? 1 : 0;
                    _configuracionempresaDto.i_ComprasMostrarIscyOtrosTributos = Globals.ClientSession.i_ComprasIscOtrosTributos = chkMostrarComprasColIscOtrosTributos.Checked ? 1 : 0;
                    _configuracionempresaDto.i_VentasMostrarIscyOtrosTributos = Globals.ClientSession.i_VentasIscOtrosTributos = chkMostrarVentasColIscOtrosTributos.Checked ? 1 : 0;
                    _configuracionempresaDto.v_NroCuentaRedondeoPerdida = Globals.ClientSession.v_NroCuentaRedondeoGanancia = txtRedondeoPerdida.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaRedondeoGanancia = Globals.ClientSession.v_NroCuentaRedondeoPerdida = txtRedondeoGanancia.Text.Trim();
                    _configuracionempresaDto.v_DiferenciaCambioCtaGanancia = Globals.ClientSession.v_DiferenciaCambioCtaGanancia = txtDifCambioCtaGanancia.Text.Trim();
                    _configuracionempresaDto.v_DiferenciaCambioCtaPerdida = Globals.ClientSession.v_DiferenciaCambioCtaPerdida = txtDifCambioCtaPerdida.Text.Trim();
                    _configuracionempresaDto.i_EditarPrecioVenta = Globals.ClientSession.i_EditarPrecioVenta = chkEditarPreciosUnitariosVenta.Checked ? 1 : 0;
                    _configuracionempresaDto.i_PermiteIntercambiarListasPrecios = Globals.ClientSession.i_PermiteIntercambiarListasPrecios = chkIntercambiarListaPrecios.Checked ? 1 : 0;
                    _configuracionempresaDto.i_GenerarActDesdeCompras = Globals.ClientSession.i_GenerarActDesdeCompras = chkActivoFijo.Checked ? 1 : 0;
                    _configuracionempresaDto.v_NroCuentaUtilidadCierre = Globals.ClientSession.v_NroCuentaUtilidadCierre = txtCuentaUtilidad.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaPerdidaCierre = Globals.ClientSession.v_NroCuentaPerdidaCierre = txtCuentaPerdida.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaResul891Cierre = Globals.ClientSession.v_NroCuentaResul891Cierre = txtResultado891.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaResul892Cierre = Globals.ClientSession.v_NroCuentaResul892Cierre = txtResultado892.Text.Trim();
                    _configuracionempresaDto.v_ImpresionDirectoVentas = Globals.ClientSession.v_ImpresionDirectoVentas = chkImpresionDirectaVenta.Checked ? "1" : "0";
                    _configuracionempresaDto.v_NroCuentaAjuste = Globals.ClientSession.v_NroCuentaAjuste = txtCuentaAjuste.Text.Trim();
                    _configuracionempresaDto.i_IncluirServicioGuiaVenta = Globals.ClientSession.i_IncluirServicioGuiaVenta = chkIncluirServiciosGuiaRV.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IncluirTransportistaGuiaRemision = Globals.ClientSession.i_IncluirAgeciaTransporteGuiaRemision = uckIncluirAgenciaTransporte.Checked ? 1 : 0;    //AgenciaTransporte; 
                    _configuracionempresaDto.i_ActualizarCostoProductos = Globals.ClientSession.i_ActualizarCostosProductos = chkActualizarCostoCompra.Checked ? 1 : 0;
                   // _configuracionempresaDto.i_IncluirLotesCompraVenta = Globals.ClientSession.i_IncluirLotesCompraVenta = rbtnInlcuirLotesCompraVenta.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IncluirNingunoCompraVenta = Globals.ClientSession.i_IncluirNingunoCompraVenta = rbtnInlcuirNingunoCompraVenta.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IncluirPedidoExportacionCompraVenta = Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta = rbtnInlcuirPedidoExportacionCompraVenta.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IdMonedaImportacion = Globals.ClientSession.i_IdMonedaImportacion = int.Parse(cboMonedaImportacion.Value.ToString());
                    _configuracionempresaDto.v_NroCuentaISC = Globals.ClientSession.NroCtaISC = txtNroCuentaISC.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaPercepcion = Globals.ClientSession.NroCtaPercepcion = txtNroCuentaPercepcion.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaOtrosConsumos = Globals.ClientSession.NroCtaOtrosTributos = txtNroCuentaOtrosTributos.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaGastosFinancierosCobranza = Globals.ClientSession.NroCtaGastosFinancieros = txtNroCuentaGastosF.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaIngresosFinancierosCobranza = Globals.ClientSession.NroCtaIngresosFinancieros = txtNrCuentaIngresosF.Text.Trim();
                    _configuracionempresaDto.i_CambiarUnidadMedidaVentaPedido = Globals.ClientSession.i_CambiarUnidadMedidaVentaPedido = chkPermiteIntercambiarUnidadMedida.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IncluirSEUOImpresionDocumentos = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos = uckIncluirSEOUImpresionDocumentos.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IdDocumentoContableLEC = Globals.ClientSession.i_IdDocumentoContableLEC = int.Parse(cboDocumentoLetrasCobrar.Value.ToString());
                    _configuracionempresaDto.i_IdDocumentoContableLEP = Globals.ClientSession.i_IdDocumentoContableLEP = int.Parse(cboDocumentoLetrasPagar.Value.ToString());
                    _configuracionempresaDto.i_TipoRegimenEmpresa = Globals.ClientSession.i_IdTipoRegimenEmpresa = int.Parse(cboTipoRegimenEmpresa.Value.ToString());
                    _configuracionempresaDto.v_NroCuentaObligacionesFinancierosCobranza = Globals.ClientSession.NroCtaObligacionesFinancieros = txtNrCuentaObligacionesF.Text.Trim();
                    _configuracionempresaDto.i_ImprimirDniPNaturalesLetras = Globals.ClientSession.i_ImprimirDniPNaturales = uckImprimirDniPNaturales.Checked ? 1 : 0;
                    _configuracionempresaDto.i_UsaListaPrecios = Globals.ClientSession.i_UsaListaPrecios = (short)(chkListaPrecios.Checked ? 1 : 0);
                    _configuracionempresaDto.i_IncluirAgenciaTransportePedido = Globals.ClientSession.i_IncluirAgenciaTransportePedido = chkIncluirAgenciaTransportePedido.Checked ? 1 : 0;
                    _configuracionempresaDto.i_EditarPrecioVentaPedido = Globals.ClientSession.i_EditarPrecioVentaPedido = chkEditarPreciosVentaPedido.Checked ? 1 : 0;
                    _configuracionempresaDto.v_GlosaTicket = Globals.ClientSession.GlosaTicket = txtGlosaTicket.Text;
                    _configuracionempresaDto.v_NroCuentaAdelanto = Globals.ClientSession.NroCtaAdelanto = txtNroCuentaAnticipo.Text;
                    _configuracionempresaDto.i_CodigoPlanContable = Globals.ClientSession.i_CodigoPlanContable = int.Parse(cboCodigoPlanContable.Value.ToString());
                    _configuracionempresaDto.i_TckUseInfo = Globals.ClientSession.TckUsarInfo = (short)(chkTckUse.Checked ? 1 : 0);
                    _configuracionempresaDto.v_TckRuc = Globals.ClientSession.TckRuc = txtTckRuc.Text;
                    _configuracionempresaDto.v_TckRzs = Globals.ClientSession.TckRzs = txtTckRzs.Text;
                    _configuracionempresaDto.v_TckDireccion = Globals.ClientSession.TckDireccion = txtTckDir.Text;
                    _configuracionempresaDto.v_TckExt = Globals.ClientSession.TckTelf = txtTckTelf.Text;
                    _configuracionempresaDto.v_IdPlanillaConceptoFaltas = Globals.ClientSession.ConceptoPlanillaTardanzas = txtConceptoPlanillaFaltas.Tag != null ? txtConceptoPlanillaFaltas.Tag.ToString() : string.Empty;
                    _configuracionempresaDto.v_IdPlanillaConceptoTardanzas = Globals.ClientSession.ConceptoPlanillaTardanzas = txtConceptoPlanillaTardanzas.Tag != null ? txtConceptoPlanillaTardanzas.Tag.ToString() : string.Empty;
                    _configuracionempresaDto.i_ValidarStockMinimoProducto = Globals.ClientSession.i_ValidarStockMinimoProducto = chkValidarStockMinimo.Checked ? 1 : 0;
                    _configuracionempresaDto.i_CostoListaPreciosDiferentesxAlmacen = Globals.ClientSession.i_CostoListaPreciosDiferentesxAlmacen = chkListaPreciosDiferentosCostos.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IncluirAlmacenDestinoGuiaRemision = Globals.ClientSession.i_IncluirAlmacenDestinoGuiaRemisionVenta = uckIncluirAlmaceDestinoGuiaVenta.Checked ? 1 : 0;
                    _configuracionempresaDto.i_VisualizarColumnasBasicasPedido = Globals.ClientSession.i_VisualizarColumnasBasicasPedido = chkVisualizarColumnasBasicasPedido.Checked ? 1 : 0;
                    _configuracionempresaDto.v_IdRepresentanteLegal = Globals.ClientSession.v_IdRepresentanteLegal = IdRepresentanteLegal;
                    _configuracionempresaDto.v_NroLeyCuartaCategoria = Globals.ClientSession.v_NroLeyCuartaCategoria = txtNroLeyCuartaCategoria.Text;
                    _configuracionempresaDto.i_UsaDominicalCalculoDescuento = Globals.ClientSession.i_UsaDominicalCalculoDescuento = chkIncluirDominical.Checked ? 1 : 0;
                    _configuracionempresaDto.i_GenerarNotaSalidaDesdeVentaUltimoDiaMes = Globals.ClientSession.i_GenerarNotaSalidaDesdeVentaUltimoDiaMes = chkGenerarNotaSalidaFechaUltimoDiaMes.Checked ? 1 : 0;
                    _configuracionempresaDto.v_GlosaPedido = Globals.ClientSession.v_GlosaPedido = txtGlosaPedido.Text.Trim();
                    _configuracionempresaDto.i_IdCondicionPagoPedido = Globals.ClientSession.i_IdCondicionPagoPedido = int.Parse(cboCondicionPagoPedido.Value.ToString());
                    _configuracionempresaDto.i_TipoVentaVentas = Globals.ClientSession.i_TipoVentaVentas = cboTipoVentaVentas.Value == null ? -1 : int.Parse(cboTipoVentaVentas.Value.ToString());
                    _configuracionempresaDto.i_SeUsaraSoloUnaListaPrecioEmpresa = Globals.ClientSession.i_SeUsaraSoloUnaListaPrecioEmpresa = chkSeUsara1ListaPreciosEmpresa.Checked ? 1 : 0;
                    _configuracionempresaDto.i_VisualizarBusquedaProductos = Globals.ClientSession.i_VisualizarBusquedaProductos = chkVisualizarTodosProductosBúsquedaProductos.Checked ? 1 : 0; // Para visualizar todos los productos en busqueda de productos sin el check de marcar stock
                    _configuracionempresaDto.i_PermiteIncluirPreciosCeroPedido = Globals.ClientSession.i_PermiteIncluirPreciosCeroPedido = chkPermiteIngresoProductosCantidadCeroPedido.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IdCondicionPagoVenta = Globals.ClientSession.i_IdCondicionPagoVenta = int.Parse(cboCondicionPagoVenta.Value.ToString());
                    _configuracionempresaDto.v_NroCuentaCobranzaRedondeoPerdida = Globals.ClientSession.v_NroCuentaCobranzaRedondeoPerdida = txtCuentaCobranzaRedondeoPerdida.Text;
                    _configuracionempresaDto.v_NroCuentaCobranzaRedondeoGanancia = Globals.ClientSession.v_NroCuentaCobranzaRedondeoGanancia = txtCuentaCobranzaRedondeoGanancia.Text;
                    _configuracionempresaDto.i_CambiarAlmacenVentasDesdeVendedor = Globals.ClientSession.i_CambiarAlmacenVentasDesdeVendedor = chkCambiarAlmacenDesdeVendedor.Checked ? 1 : 0;
                    _configuracionempresaDto.i_TipoDepreciacionActivoFijo = Globals.ClientSession.i_TipoDepreciacionActivoFijo = rbtnMensual.Checked ? 1 : 2;
                    _configuracionempresaDto.i_PermiteEditarPedidoFacturado = Globals.ClientSession.i_PermiteEditarPedidoFacturado = chkPermiteEditarPedidoFacturado.Checked ? 1 : 0;
                    _configuracionempresaDto.b_LogoEmpresa = Logo;
                    _configuracionempresaDto.b_FirmaDigitalEmpresa = Firma;
                    _objConfiguracionEmpresaBL.InsertarConfiguracionEmpresa(ref objOperationResult, _configuracionempresaDto, Globals.ClientSession.GetAsList());
                }
                else if (_mode == "Edit")
                {
                    var objConfiguracionEmpresaBl = new ConfiguracionEmpresaBL();
                    _configuracionempresaDto = objConfiguracionEmpresaBl.ObtenerConfiguracionEmpresa(ref objOperationResult, Globals.ClientSession.i_SystemUserId);
                    i_IdConfiguracionEmpresa = _configuracionempresaDto.i_IdConfiguracionEmpresa;
                    _configuracionempresaDto.i_IdMoneda = Globals.ClientSession.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                    _configuracionempresaDto.i_AfectoIgvCompras = Globals.ClientSession.i_AfectoIgvCompras = chkAfectoIGVCom.Checked == true ? 1 : 0;
                    _configuracionempresaDto.i_PrecioIncluyeIgvCompras = Globals.ClientSession.i_PrecioIncluyeIgvCompras = chkPrecInIGVCom.Checked == true ? 1 : 0;
                    _configuracionempresaDto.i_IdDestinoCompras = Globals.ClientSession.i_IdDestinoCompras = int.Parse(cboDestino.Value.ToString());
                    _configuracionempresaDto.i_AfectoIgvVentas = Globals.ClientSession.i_AfectoIgvVentas = chkAfectoIGVVen.Checked == true ? 1 : 0;
                    _configuracionempresaDto.i_PrecioIncluyeIgvVentas = Globals.ClientSession.i_PrecioIncluyeIgvVentas = chkPrecInIGVVen.Checked == true ? 1 : 0;
                    _configuracionempresaDto.i_IdTipoOperacionVentas = Globals.ClientSession.i_IdTipoOperacionVentas = int.Parse(cboTipoOperacion.Value.ToString());
                    _configuracionempresaDto.i_CantidadDecimales = Globals.ClientSession.i_CantidadDecimales = int.Parse(txtNumeroDecimalesCantidad.Text);
                    _configuracionempresaDto.i_PrecioDecimales = Globals.ClientSession.i_PrecioDecimales = int.Parse(txtNumeroDecimalesPrecio.Text);
                    _configuracionempresaDto.d_ValorMaximoBoletas = Globals.ClientSession.d_ValorMaximoBoletas = decimal.Parse(txtMontoMaximoBoletas.Text);
                    _configuracionempresaDto.i_IdPermiteStockNegativo = Globals.ClientSession.i_IdPermiteStockNegativo = int.Parse(cboPermiteStockNegativo.Value.ToString());
                    _configuracionempresaDto.i_IdIgv = Globals.ClientSession.i_IdIgv = int.Parse(cboIGV.Value.ToString());
                    _configuracionempresaDto.d_ComisionVendedor = Globals.ClientSession.d_ComisionVendedor = decimal.Parse(txtPorcentajeComisionVendedor.Text);
                    _configuracionempresaDto.v_IdCuentaContableSoles = Globals.ClientSession.v_IdCuentaContableSoles = TxtCuentaContableSoles.Text;
                    _configuracionempresaDto.v_IdCuentaContableDolares = Globals.ClientSession.v_IdCuentaContableDolares = TxtCuentaContableDolares.Text;
                    _configuracionempresaDto.i_IdAlmacenPredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado = int.Parse(cboAlmacen.Value.ToString());
                    _configuracionempresaDto.i_IncluirPreciosGuiaRemision = Globals.ClientSession.i_IncluirPreciosGuiaRemision = ChkIncluirPrecios.Checked == true ? 1 : 0;
                    _configuracionempresaDto.i_FechaRegistro = Globals.ClientSession.i_FechaRegistro = rbtnFechaRegistro.Checked == true ? 1 : 0;
                    _configuracionempresaDto.i_FechaEmision = Globals.ClientSession.i_FechaEmision = rbtnFechaEmision.Checked == true ? 1 : 0;
                    _configuracionempresaDto.i_EmpresaAfectaRetencion = Globals.ClientSession.i_EmpresaAfectaRetencion = chkEmpresaAfectaRetencion.Checked == true ? 1 : 0;
                    _configuracionempresaDto.v_NroCuentaRetencion = Globals.ClientSession.v_NroCuentaRetencion = txtNroCuentaRetencion.Text;
                    _configuracionempresaDto.i_VentasMostrarColumasEmpaque = Globals.ClientSession.i_VentasMostrarEmpaque = chkMostrarEmpaqueVentas.Checked == true ? 1 : 0;
                    _configuracionempresaDto.i_ComprasMostrarColumasEmpaque = Globals.ClientSession.i_ComprasMostrarEmpaque = chkMostrarEmpaqueCompras.Checked == true ? 1 : 0;
                    _configuracionempresaDto.d_TasaRetencion = Globals.ClientSession.d_TasaRetencion = !string.IsNullOrEmpty(txtTasaRetencion.Text) ? decimal.Parse(txtTasaRetencion.Text.Trim()) : 0;
                    _configuracionempresaDto.i_RedondearVentas = Globals.ClientSession.i_RedondearVentas = chkRedondearVentas.Checked == true ? 1 : 0;
                    _configuracionempresaDto.i_IdMonedaCompra = Globals.ClientSession.i_IdMonedaCompra = int.Parse(cbMonedaCompras.Value.ToString());
                    _configuracionempresaDto.i_EditarPrecioPedido = Globals.ClientSession.i_PrecioEditableTodosProductosPedido = chkEditarPreciosUnitariosPedido.Checked == true ? 1 : 0;
                    _configuracionempresaDto.i_ComprasMostrarIscyOtrosTributos = Globals.ClientSession.i_ComprasIscOtrosTributos = chkMostrarComprasColIscOtrosTributos.Checked == true ? 1 : 0;
                    _configuracionempresaDto.i_VentasMostrarIscyOtrosTributos = Globals.ClientSession.i_VentasIscOtrosTributos = chkMostrarVentasColIscOtrosTributos.Checked == true ? 1 : 0;
                    _configuracionempresaDto.v_NroCuentaRedondeoPerdida = Globals.ClientSession.v_NroCuentaRedondeoGanancia = txtRedondeoPerdida.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaRedondeoGanancia = Globals.ClientSession.v_NroCuentaRedondeoPerdida = txtRedondeoGanancia.Text.Trim();
                    _configuracionempresaDto.v_DiferenciaCambioCtaGanancia = Globals.ClientSession.v_DiferenciaCambioCtaGanancia = txtDifCambioCtaGanancia.Text.Trim();
                    _configuracionempresaDto.v_DiferenciaCambioCtaPerdida = Globals.ClientSession.v_DiferenciaCambioCtaPerdida = txtDifCambioCtaPerdida.Text.Trim();
                    _configuracionempresaDto.i_EditarPrecioVenta = Globals.ClientSession.i_EditarPrecioVenta = chkEditarPreciosUnitariosVenta.Checked ? 1 : 0;
                    _configuracionempresaDto.i_PermiteIntercambiarListasPrecios = Globals.ClientSession.i_PermiteIntercambiarListasPrecios = chkIntercambiarListaPrecios.Checked ? 1 : 0;
                    _configuracionempresaDto.i_GenerarActDesdeCompras = Globals.ClientSession.i_GenerarActDesdeCompras = chkActivoFijo.Checked ? 1 : 0;
                    _configuracionempresaDto.v_NroCuentaUtilidadCierre = Globals.ClientSession.v_NroCuentaUtilidadCierre = txtCuentaUtilidad.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaPerdidaCierre = Globals.ClientSession.v_NroCuentaPerdidaCierre = txtCuentaPerdida.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaResul891Cierre = Globals.ClientSession.v_NroCuentaResul891Cierre = txtResultado891.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaResul892Cierre = Globals.ClientSession.v_NroCuentaResul892Cierre = txtResultado892.Text.Trim();
                    _configuracionempresaDto.v_ImpresionDirectoVentas = Globals.ClientSession.v_ImpresionDirectoVentas = chkImpresionDirectaVenta.Checked == true ? "1" : "0";
                    _configuracionempresaDto.v_NroCuentaAjuste = Globals.ClientSession.v_NroCuentaAjuste = txtCuentaAjuste.Text.Trim();
                    _configuracionempresaDto.i_IncluirServicioGuiaVenta = Globals.ClientSession.i_IncluirServicioGuiaVenta = chkIncluirServiciosGuiaRV.Checked == true ? 1 : 0;
                    _configuracionempresaDto.i_IncluirTransportistaGuiaRemision = Globals.ClientSession.i_IncluirAgeciaTransporteGuiaRemision = uckIncluirAgenciaTransporte.Checked ? 1 : 0;    //AgenciaTransporte; 
                    _configuracionempresaDto.i_ActualizarCostoProductos = Globals.ClientSession.i_ActualizarCostosProductos = chkActualizarCostoCompra.Checked ? 1 : 0;
                   // _configuracionempresaDto.i_IncluirLotesCompraVenta = Globals.ClientSession.i_IncluirLotesCompraVenta = rbtnInlcuirLotesCompraVenta.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IncluirNingunoCompraVenta = Globals.ClientSession.i_IncluirNingunoCompraVenta = rbtnInlcuirNingunoCompraVenta.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IncluirPedidoExportacionCompraVenta = Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta = rbtnInlcuirPedidoExportacionCompraVenta.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IdMonedaImportacion = Globals.ClientSession.i_IdMonedaImportacion = int.Parse(cboMonedaImportacion.Value.ToString());
                    _configuracionempresaDto.v_NroCuentaISC = Globals.ClientSession.NroCtaISC = txtNroCuentaISC.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaPercepcion = Globals.ClientSession.NroCtaPercepcion = txtNroCuentaPercepcion.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaOtrosConsumos = Globals.ClientSession.NroCtaOtrosTributos = txtNroCuentaOtrosTributos.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaGastosFinancierosCobranza = Globals.ClientSession.NroCtaGastosFinancieros = txtNroCuentaGastosF.Text.Trim();
                    _configuracionempresaDto.v_NroCuentaIngresosFinancierosCobranza = Globals.ClientSession.NroCtaIngresosFinancieros = txtNrCuentaIngresosF.Text.Trim();
                    _configuracionempresaDto.i_CambiarUnidadMedidaVentaPedido = Globals.ClientSession.i_CambiarUnidadMedidaVentaPedido = chkPermiteIntercambiarUnidadMedida.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IncluirSEUOImpresionDocumentos = Globals.ClientSession.i_IncluirSEUOImpresionDocumentos = uckIncluirSEOUImpresionDocumentos.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IdDocumentoContableLEC = Globals.ClientSession.i_IdDocumentoContableLEC = int.Parse(cboDocumentoLetrasCobrar.Value.ToString());
                    _configuracionempresaDto.i_IdDocumentoContableLEP = Globals.ClientSession.i_IdDocumentoContableLEP = int.Parse(cboDocumentoLetrasPagar.Value.ToString());
                    _configuracionempresaDto.i_TipoRegimenEmpresa = Globals.ClientSession.i_IdTipoRegimenEmpresa = int.Parse(cboTipoRegimenEmpresa.Value.ToString());
                    _configuracionempresaDto.v_NroCuentaObligacionesFinancierosCobranza = Globals.ClientSession.NroCtaObligacionesFinancieros = txtNrCuentaObligacionesF.Text.Trim();
                    _configuracionempresaDto.i_ImprimirDniPNaturalesLetras = Globals.ClientSession.i_ImprimirDniPNaturales = uckImprimirDniPNaturales.Checked ? 1 : 0;
                    _configuracionempresaDto.i_UsaListaPrecios = Globals.ClientSession.i_UsaListaPrecios = (short)(chkListaPrecios.Checked ? 1 : 0);
                    _configuracionempresaDto.i_IncluirAgenciaTransportePedido = Globals.ClientSession.i_IncluirAgenciaTransportePedido = chkIncluirAgenciaTransportePedido.Checked ? 1 : 0;
                    _configuracionempresaDto.i_EditarPrecioVentaPedido = Globals.ClientSession.i_EditarPrecioVentaPedido = chkEditarPreciosVentaPedido.Checked ? 1 : 0;
                    _configuracionempresaDto.v_GlosaTicket = Globals.ClientSession.GlosaTicket = txtGlosaTicket.Text;
                    _configuracionempresaDto.v_NroCuentaAdelanto = Globals.ClientSession.NroCtaAdelanto = txtNroCuentaAnticipo.Text;
                    _configuracionempresaDto.i_CodigoPlanContable = Globals.ClientSession.i_CodigoPlanContable = int.Parse(cboCodigoPlanContable.Value.ToString());
                    _configuracionempresaDto.i_TckUseInfo = Globals.ClientSession.TckUsarInfo = (short)(chkTckUse.Checked ? 1 : 0);
                    _configuracionempresaDto.v_TckRuc = Globals.ClientSession.TckRuc = txtTckRuc.Text;
                    _configuracionempresaDto.v_TckRzs = Globals.ClientSession.TckRzs = txtTckRzs.Text;
                    _configuracionempresaDto.v_TckDireccion = Globals.ClientSession.TckDireccion = txtTckDir.Text;
                    _configuracionempresaDto.v_TckExt = Globals.ClientSession.TckTelf = txtTckTelf.Text;
                    _configuracionempresaDto.i_ValidarStockMinimoProducto = Globals.ClientSession.i_ValidarStockMinimoProducto = chkValidarStockMinimo.Checked ? 1 : 0;
                    _configuracionempresaDto.v_NroCtaRetenciones = Globals.ClientSession.v_NroCtaRetenciones = txtNroCtaRetenciones.Text.Trim();
                    _configuracionempresaDto.i_CostoListaPreciosDiferentesxAlmacen = Globals.ClientSession.i_CostoListaPreciosDiferentesxAlmacen = chkListaPreciosDiferentosCostos.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IncluirAlmacenDestinoGuiaRemision = Globals.ClientSession.i_IncluirAlmacenDestinoGuiaRemisionVenta = uckIncluirAlmaceDestinoGuiaVenta.Checked ? 1 : 0;
                    _configuracionempresaDto.i_VisualizarColumnasBasicasPedido = Globals.ClientSession.i_VisualizarColumnasBasicasPedido = chkVisualizarColumnasBasicasPedido.Checked ? 1 : 0;
                    _configuracionempresaDto.v_IdPlanillaConceptoFaltas = Globals.ClientSession.ConceptoPlanillaTardanzas = txtConceptoPlanillaFaltas.Tag != null ? txtConceptoPlanillaFaltas.Tag.ToString() : string.Empty;
                    _configuracionempresaDto.v_IdPlanillaConceptoTardanzas = Globals.ClientSession.ConceptoPlanillaTardanzas = txtConceptoPlanillaTardanzas.Tag != null ? txtConceptoPlanillaTardanzas.Tag.ToString() : string.Empty;
                    _configuracionempresaDto.v_IdRepresentanteLegal = Globals.ClientSession.v_IdRepresentanteLegal = IdRepresentanteLegal;
                    _configuracionempresaDto.v_NroLeyCuartaCategoria = Globals.ClientSession.v_NroLeyCuartaCategoria = txtNroLeyCuartaCategoria.Text;
                    _configuracionempresaDto.i_UsaDominicalCalculoDescuento = Globals.ClientSession.i_UsaDominicalCalculoDescuento = chkIncluirDominical.Checked ? 1 : 0;
                    _configuracionempresaDto.v_IdProductoDetalleFlete = Globals.ClientSession.v_IdProductoDetalleFlete = txtArticuloFlete.Tag != null ? txtArticuloFlete.Tag.ToString() : null;
                    _configuracionempresaDto.v_IdProductoDetalleSeguro = Globals.ClientSession.v_IdProductoDetalleSeguro = txtSeguroMaritimo.Tag != null ? txtSeguroMaritimo.Tag.ToString() : null;
                    _configuracionempresaDto.i_GenerarNotaSalidaDesdeVentaUltimoDiaMes = Globals.ClientSession.i_GenerarNotaSalidaDesdeVentaUltimoDiaMes = chkGenerarNotaSalidaFechaUltimoDiaMes.Checked ? 1 : 0;
                    _configuracionempresaDto.i_TipoVentaVentas = Globals.ClientSession.i_TipoVentaVentas = cboTipoVentaVentas.Value == null ? -1 : int.Parse(cboTipoVentaVentas.Value.ToString());
                    _configuracionempresaDto.v_GlosaPedido = Globals.ClientSession.v_GlosaPedido = txtGlosaPedido.Text.Trim();
                    _configuracionempresaDto.i_IdCondicionPagoPedido = Globals.ClientSession.i_IdCondicionPagoPedido = int.Parse(cboCondicionPagoPedido.Value.ToString());
                    _configuracionempresaDto.i_SeUsaraSoloUnaListaPrecioEmpresa = Globals.ClientSession.i_SeUsaraSoloUnaListaPrecioEmpresa = chkSeUsara1ListaPreciosEmpresa.Checked ? 1 : 0;
                    _configuracionempresaDto.i_VisualizarBusquedaProductos = Globals.ClientSession.i_VisualizarBusquedaProductos = chkVisualizarTodosProductosBúsquedaProductos.Checked ? 1 : 0; // Para visualizar todos los productos en busqueda de productos sin el check de marcar stock
                    _configuracionempresaDto.i_PermiteIncluirPreciosCeroPedido = Globals.ClientSession.i_PermiteIncluirPreciosCeroPedido = chkPermiteIngresoProductosCantidadCeroPedido.Checked ? 1 : 0;
                    _configuracionempresaDto.i_IdCondicionPagoVenta = Globals.ClientSession.i_IdCondicionPagoVenta = int.Parse(cboCondicionPagoVenta.Value.ToString());
                    _configuracionempresaDto.v_NroCuentaCobranzaRedondeoPerdida = Globals.ClientSession.v_NroCuentaCobranzaRedondeoPerdida = txtCuentaCobranzaRedondeoPerdida.Text;
                    _configuracionempresaDto.v_NroCuentaCobranzaRedondeoGanancia = Globals.ClientSession.v_NroCuentaCobranzaRedondeoGanancia = txtCuentaCobranzaRedondeoGanancia.Text;
                    _configuracionempresaDto.i_CambiarAlmacenVentasDesdeVendedor = Globals.ClientSession.i_CambiarAlmacenVentasDesdeVendedor = chkCambiarAlmacenDesdeVendedor.Checked ? 1 : 0;
                    _configuracionempresaDto.i_EmpresaAfectaPercepcionVenta = Globals.ClientSession.i_EmpresaAfectaPercepcionVenta = chkEmpresaAfectaPercepcionVenta.Checked ? 1 : 0;
                    _configuracionempresaDto.i_TipoDepreciacionActivoFijo = Globals.ClientSession.i_TipoDepreciacionActivoFijo = rbtnMensual.Checked ? 1 : 2;
                    _configuracionempresaDto.i_PermiteEditarPedidoFacturado = Globals.ClientSession.i_PermiteEditarPedidoFacturado = chkPermiteEditarPedidoFacturado.Checked ? 1 : 0;
                    _configuracionempresaDto.b_LogoEmpresa = Logo;
                    _configuracionempresaDto.b_FirmaDigitalEmpresa = Firma;
                    _objConfiguracionEmpresaBL.ActualizarConfiguracionEmpresa(ref objOperationResult, _configuracionempresaDto, Globals.ClientSession.GetAsList());
                }
                int i;
                UserConfig.Default.appAlmacenPredeterminado = Globals.ClientSession.i_IdAlmacenPredeterminado ?? 1;
                Globals.ClientSession.i_IdEstablecimiento = int.Parse(cboEstablecimientoPredet.Value.ToString());
                ((frmMaster)MdiParent).SetEstablecimiento(cboEstablecimientoPredet.Text.Substring(cboEstablecimientoPredet.Text.IndexOf('|') + 1));
                UserConfig.Default.appEstablecimientoPredeterminado = int.Parse(cboEstablecimientoPredet.Value.ToString());
                UserConfig.Default.csTipoDocumentoVentaRapida = cboDocumento.Value.ToString();
                UserConfig.Default.appTipoBusquedaPredeterminadaProducto = chkPredeterminarServicios.Checked
                    ? TipoBusquedaProductos.Servicios
                    : TipoBusquedaProductos.Ambos;
                UserConfig.Default.appPermiteIntercambiarDocumentosVR = chkPermiteIntercambiarDocumentoVR.Checked;
                UserConfig.Default.ImpresionDirectaTesoreria = uckImprimirAutomatTesoreria.Checked;
                UserConfig.Default.MostrarSoloFormasPagoAlmacenActual = chkFormasPagoAlmacen.Checked;
                UserConfig.Default.SerieCaja = cboSeriePredeterminada.Text.Trim();
                UserConfig.Default.TipoDocumentoCajaChica = cboDocumentoCajaChica.Value == null ? "-1" : cboDocumentoCajaChica.Value.ToString();
                UserConfig.Default.CantVenta = int.TryParse(txtCantVentaPredet.Text, out i) ? i : 1;
                UserConfig.Default.CantCompra = int.TryParse(txtCantCompraPredet.Text, out i) ? i : 1;
                UserConfig.Default.PermiteRealizarCobranzasImporteCero = chkPermiteRealizarCobranzasImporteCero.Checked;
                UserConfig.Default.Save();

                if (objOperationResult.Success == 1)
                {
                    _mode = "Edit";
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema");
                }
                else
                    UltraMessageBox.Show(Constants.GenericErrorMessage, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {

                UltraMessageBox.Show("Ingresar los Datos solicitados", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Cargar()
        {
            var objConfiguracionEmpresaBl = new ConfiguracionEmpresaBL();

            _configuracionempresaDto = objConfiguracionEmpresaBl.ObtenerConfiguracionEmpresa(ref _objOperationResult, Globals.ClientSession.i_SystemUserId);

            if (_configuracionempresaDto == null)
            {
                _mode = "New";
            }
            else
            {
                _mode = "Edit";
                i_IdConfiguracionEmpresa = _configuracionempresaDto.i_IdConfiguracionEmpresa;
                cboMoneda.Value = _configuracionempresaDto.i_IdMoneda.ToString();
                chkAfectoIGVCom.Checked = _configuracionempresaDto.i_AfectoIgvCompras == 1;
                chkPrecInIGVCom.Checked = _configuracionempresaDto.i_PrecioIncluyeIgvCompras == 1;
                cboDestino.Value = _configuracionempresaDto.i_IdDestinoCompras.ToString();
                chkAfectoIGVVen.Checked = _configuracionempresaDto.i_AfectoIgvVentas == 1;
                chkPrecInIGVVen.Checked = _configuracionempresaDto.i_PrecioIncluyeIgvVentas == 1;
                cboTipoOperacion.Value = _configuracionempresaDto.i_IdTipoOperacionVentas.ToString();
                txtNumeroDecimalesCantidad.Text = _configuracionempresaDto.i_CantidadDecimales.ToString();
                txtNumeroDecimalesPrecio.Text = _configuracionempresaDto.i_PrecioDecimales.ToString();
                txtMontoMaximoBoletas.Text = _configuracionempresaDto.d_ValorMaximoBoletas.ToString();
                cboPermiteStockNegativo.Value = _configuracionempresaDto.i_IdPermiteStockNegativo.ToString();
                cboIGV.Value = _configuracionempresaDto.i_IdIgv.ToString();
                txtPorcentajeComisionVendedor.Text = _configuracionempresaDto.d_ComisionVendedor.ToString();
                TxtCuentaContableSoles.Text = _configuracionempresaDto.v_IdCuentaContableSoles;
                TxtCuentaContableDolares.Text = _configuracionempresaDto.v_IdCuentaContableDolares;
                ChkIncluirPrecios.Checked = _configuracionempresaDto.i_IncluirPreciosGuiaRemision == 1;
                rbtnFechaRegistro.Checked = _configuracionempresaDto.i_FechaRegistro == 1;
                rbtnFechaEmision.Checked = _configuracionempresaDto.i_FechaEmision == 1;
                chkEmpresaAfectaRetencion.Checked = _configuracionempresaDto.i_EmpresaAfectaRetencion == 1;
                txtNroCuentaRetencion.Enabled = _configuracionempresaDto.i_EmpresaAfectaRetencion == 1;
                txtTasaRetencion.Enabled = _configuracionempresaDto.i_EmpresaAfectaRetencion == 1;
                txtTasaRetencion.Text = _configuracionempresaDto.d_TasaRetencion.ToString();
                txtNroCuentaRetencion.Text = _configuracionempresaDto.v_NroCuentaRetencion;
                cboEstablecimientoPredet.Value = UserConfig.Default.appEstablecimientoPredeterminado.ToString();
                cboAlmacen.Value = UserConfig.Default.appAlmacenPredeterminado.ToString();
                cboDocumento.Value = UserConfig.Default.csTipoDocumentoVentaRapida;
                uckImprimirAutomatTesoreria.Checked = UserConfig.Default.ImpresionDirectaTesoreria;
                chkRedondearVentas.Checked = _configuracionempresaDto.i_RedondearVentas != null && _configuracionempresaDto.i_RedondearVentas.Value != 0;
                chkMostrarEmpaqueCompras.Checked = _configuracionempresaDto.i_ComprasMostrarColumasEmpaque != null && _configuracionempresaDto.i_ComprasMostrarColumasEmpaque.Value != 0;
                chkMostrarEmpaqueVentas.Checked = _configuracionempresaDto.i_VentasMostrarColumasEmpaque != null && _configuracionempresaDto.i_VentasMostrarColumasEmpaque.Value != 0;
                cbMonedaCompras.Value = _configuracionempresaDto.i_IdMonedaCompra != null ? _configuracionempresaDto.i_IdMonedaCompra.ToString() : "-1";
                chkEditarPreciosUnitariosPedido.Checked = _configuracionempresaDto.i_EditarPrecioPedido == 1;
                chkMostrarComprasColIscOtrosTributos.Checked = _configuracionempresaDto.i_ComprasMostrarIscyOtrosTributos == 1;
                chkMostrarVentasColIscOtrosTributos.Checked = _configuracionempresaDto.i_VentasMostrarIscyOtrosTributos == 1;
                txtRedondeoGanancia.Text = _configuracionempresaDto.v_NroCuentaRedondeoGanancia;
                txtRedondeoPerdida.Text = _configuracionempresaDto.v_NroCuentaRedondeoPerdida;
                txtDifCambioCtaPerdida.Text = _configuracionempresaDto.v_DiferenciaCambioCtaPerdida;
                txtDifCambioCtaGanancia.Text = _configuracionempresaDto.v_DiferenciaCambioCtaGanancia;
                chkEditarPreciosUnitariosVenta.Checked = _configuracionempresaDto.i_EditarPrecioVenta == 1;
                chkIntercambiarListaPrecios.Checked = _configuracionempresaDto.i_PermiteIntercambiarListasPrecios == 1;
                chkActivoFijo.Checked = _configuracionempresaDto.i_GenerarActDesdeCompras == 1;
                txtCuentaUtilidad.Text = _configuracionempresaDto.v_NroCuentaUtilidadCierre;
                txtCuentaPerdida.Text = _configuracionempresaDto.v_NroCuentaPerdidaCierre;
                txtResultado891.Text = _configuracionempresaDto.v_NroCuentaResul891Cierre;
                txtResultado892.Text = _configuracionempresaDto.v_NroCuentaResul892Cierre;
                chkImpresionDirectaVenta.Checked = _configuracionempresaDto.v_ImpresionDirectoVentas != null && _configuracionempresaDto.v_ImpresionDirectoVentas != "0";
                chkIncluirServiciosGuiaRV.Checked = _configuracionempresaDto.i_IncluirServicioGuiaVenta != null && _configuracionempresaDto.i_IncluirServicioGuiaVenta != 0;
                chkPredeterminarServicios.Checked = UserConfig.Default.appTipoBusquedaPredeterminadaProducto == TipoBusquedaProductos.Servicios;
                uckIncluirAgenciaTransporte.Checked = _configuracionempresaDto.i_IncluirTransportistaGuiaRemision != null && _configuracionempresaDto.i_IncluirTransportistaGuiaRemision != 0;
                chkActualizarCostoCompra.Checked = _configuracionempresaDto.i_ActualizarCostoProductos == 1;
                chkPermiteIntercambiarDocumentoVR.Checked = UserConfig.Default.appPermiteIntercambiarDocumentosVR;
                //rbtnInlcuirLotesCompraVenta.Checked = _configuracionempresaDto.i_IncluirLotesCompraVenta == 1;
                rbtnInlcuirNingunoCompraVenta.Checked = _configuracionempresaDto.i_IncluirNingunoCompraVenta == 1;
                rbtnInlcuirPedidoExportacionCompraVenta.Checked = _configuracionempresaDto.i_IncluirPedidoExportacionCompraVenta == 1;
                cboMonedaImportacion.Value = _configuracionempresaDto.i_IdMonedaImportacion.ToString();
                txtNroCuentaPercepcion.Text = _configuracionempresaDto.v_NroCuentaPercepcion ?? string.Empty;
                txtNroCuentaOtrosTributos.Text = _configuracionempresaDto.v_NroCuentaOtrosConsumos ?? string.Empty;
                txtNroCuentaISC.Text = _configuracionempresaDto.v_NroCuentaISC ?? string.Empty;
                txtNroCuentaGastosF.Text = _configuracionempresaDto.v_NroCuentaGastosFinancierosCobranza ?? string.Empty;
                txtNrCuentaIngresosF.Text = _configuracionempresaDto.v_NroCuentaIngresosFinancierosCobranza ?? string.Empty;
                chkPermiteIntercambiarUnidadMedida.Checked = _configuracionempresaDto.i_CambiarUnidadMedidaVentaPedido == 1;
                uckIncluirSEOUImpresionDocumentos.Checked = _configuracionempresaDto.i_IncluirSEUOImpresionDocumentos != null && _configuracionempresaDto.i_IncluirSEUOImpresionDocumentos != 0;
                cboDocumentoLetrasCobrar.Value = _configuracionempresaDto.i_IdDocumentoContableLEC.Value.ToString();
                cboDocumentoLetrasPagar.Value = _configuracionempresaDto.i_IdDocumentoContableLEP.Value.ToString();
                cboTipoRegimenEmpresa.Value = _configuracionempresaDto.i_TipoRegimenEmpresa.Value.ToString();
                txtNrCuentaObligacionesF.Text = _configuracionempresaDto.v_NroCuentaObligacionesFinancierosCobranza;
                uckImprimirDniPNaturales.Checked = _configuracionempresaDto.i_ImprimirDniPNaturalesLetras != 0;
                chkListaPrecios.Checked = _configuracionempresaDto.i_UsaListaPrecios == 1;
                chkIncluirAgenciaTransportePedido.Checked = _configuracionempresaDto != null && _configuracionempresaDto.i_IncluirAgenciaTransportePedido != 0;
                chkEditarPreciosVentaPedido.Checked = _configuracionempresaDto.i_EditarPrecioVentaPedido == 1;
                txtGlosaTicket.Text = _configuracionempresaDto.v_GlosaTicket;
                txtNroCuentaAnticipo.Text = _configuracionempresaDto.v_NroCuentaAdelanto;
                txtNroTicket.Text = UserConfig.Default.NroTicket;
                cboCodigoPlanContable.Value = _configuracionempresaDto.i_CodigoPlanContable;
                chkTckUse.Checked = _configuracionempresaDto.i_TckUseInfo == 1;
                txtTckRuc.Text = _configuracionempresaDto.v_TckRuc;
                txtTckRzs.Text = _configuracionempresaDto.v_TckRzs;
                txtTckDir.Text = _configuracionempresaDto.v_TckDireccion;
                txtTckTelf.Text = _configuracionempresaDto.v_TckExt;
                txtNroCtaRetenciones.Text = _configuracionempresaDto.v_NroCtaRetenciones == null ? "" : _configuracionempresaDto.v_NroCtaRetenciones.Trim();
                chkValidarStockMinimo.Checked = _configuracionempresaDto.i_ValidarStockMinimoProducto == 1;
                chkVisualizarColumnasBasicasPedido.Checked = _configuracionempresaDto.i_VisualizarColumnasBasicasPedido == 1;
                chkFormasPagoAlmacen.Checked = UserConfig.Default.MostrarSoloFormasPagoAlmacenActual;
                cboSeriePredeterminada.Value = UserConfig.Default.SerieCaja;
                chkListaPreciosDiferentosCostos.Checked = _configuracionempresaDto != null && _configuracionempresaDto.i_CostoListaPreciosDiferentesxAlmacen != 0;
                uckIncluirAlmaceDestinoGuiaVenta.Checked = _configuracionempresaDto != null && _configuracionempresaDto.i_IncluirAlmacenDestinoGuiaRemision != 0;
                cboDocumentoCajaChica.Value = UserConfig.Default.TipoDocumentoCajaChica;
                txtConceptoPlanillaFaltas.Text = _configuracionempresaDto.CodConceptoPlanillaFaltas;
                txtConceptoPlanillaFaltas.Tag = _configuracionempresaDto.v_IdPlanillaConceptoFaltas;
                txtConceptoPlanillaTardanzas.Text = _configuracionempresaDto.CodConceptoPlanillaTardanzas;
                txtConceptoPlanillaTardanzas.Tag = _configuracionempresaDto.v_IdPlanillaConceptoTardanzas;
                txtNroLeyCuartaCategoria.Text = _configuracionempresaDto.v_NroLeyCuartaCategoria;
                chkIncluirDominical.Checked = (_configuracionempresaDto.i_UsaDominicalCalculoDescuento ?? 0) == 1;
                var RepresentanteLegal = new PlanillaBL().ObtenerTrabajadorbyIdTrabajador(ref _objOperationResult, _configuracionempresaDto.v_IdRepresentanteLegal);
                txtRepresentateLegal.Text = RepresentanteLegal != null ? RepresentanteLegal.v_RazonSocial : "";
                txtCantCompraPredet.Text = UserConfig.Default.CantCompra.ToString();
                txtCantVentaPredet.Text = UserConfig.Default.CantVenta.ToString();
                txtArticuloFlete.Text = _configuracionempresaDto.CodArticuloFlete;
                txtArticuloFlete.Tag = _configuracionempresaDto.v_IdProductoDetalleFlete;
                txtSeguroMaritimo.Text = _configuracionempresaDto.CodArticuloSeguroMaritimo;
                txtSeguroMaritimo.Tag = _configuracionempresaDto.v_IdProductoDetalleSeguro;
                chkGenerarNotaSalidaFechaUltimoDiaMes.Checked = _configuracionempresaDto != null && _configuracionempresaDto.i_GenerarNotaSalidaDesdeVentaUltimoDiaMes != 0;
                cboTipoVentaVentas.Value = _configuracionempresaDto.i_TipoVentaVentas.Value != -1 ? _configuracionempresaDto.i_TipoVentaVentas.Value.ToString("00") : "-1";
                chkEmpresaAfectaPercepcionVenta.Checked = _configuracionempresaDto.i_EmpresaAfectaPercepcionVenta == 1;
                cboCondicionPagoPedido.Value = _configuracionempresaDto.i_IdCondicionPagoPedido ?? -1;
                txtGlosaPedido.Text = _configuracionempresaDto.v_GlosaPedido;
                chkSeUsara1ListaPreciosEmpresa.Checked = _configuracionempresaDto.i_SeUsaraSoloUnaListaPrecioEmpresa == 1;
                chkVisualizarTodosProductosBúsquedaProductos.Checked = _configuracionempresaDto.i_VisualizarBusquedaProductos == 1;
                chkPermiteIngresoProductosCantidadCeroPedido.Checked = _configuracionempresaDto.i_PermiteIncluirPreciosCeroPedido == 1;
                cboCondicionPagoVenta.Value = _configuracionempresaDto.i_IdCondicionPagoVenta ?? -1;
                txtCuentaCobranzaRedondeoGanancia.Text = _configuracionempresaDto.v_NroCuentaCobranzaRedondeoGanancia;
                txtCuentaCobranzaRedondeoPerdida.Text = _configuracionempresaDto.v_NroCuentaCobranzaRedondeoPerdida;
                chkCambiarAlmacenDesdeVendedor.Checked = _configuracionempresaDto.i_CambiarAlmacenVentasDesdeVendedor == 1;
                rbtnMensual.Checked = _configuracionempresaDto.i_TipoDepreciacionActivoFijo == 1;
                rbtnDiaria.Checked = _configuracionempresaDto.i_TipoDepreciacionActivoFijo == 2;
                chkPermiteEditarPedidoFacturado.Checked = _configuracionempresaDto.i_PermiteEditarPedidoFacturado == 1;
                FotoLogoEmpresa.Image = _configuracionempresaDto.b_LogoEmpresa != null ? Utils.Windows.BinaryToImage(_configuracionempresaDto.b_LogoEmpresa) : null;
                FotoFirmaEmpresa.Image = _configuracionempresaDto.b_FirmaDigitalEmpresa != null ? Utils.Windows.BinaryToImage(_configuracionempresaDto.b_FirmaDigitalEmpresa) : null;
                Logo = _configuracionempresaDto.b_LogoEmpresa;
                Firma = _configuracionempresaDto.b_FirmaDigitalEmpresa;

            }
        }

        #region Eventos
        private void txtNumeroDecimalesCantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Utils.Windows.NumeroEntero(txtNumeroDecimalesCantidad, e);
            Utils.Windows.NumeroEnteroMaxDecimalesUltraTextBox(txtNumeroDecimalesCantidad, e);
        }

        private void txtNumeroDecimalesPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Utils.Windows.NumeroEntero(txtNumeroDecimalesPrecio, e);
            Utils.Windows.NumeroEnteroMaxDecimalesUltraTextBox(txtNumeroDecimalesCantidad, e);
        }

        private void txtMontoMaximoBoletas_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox((Infragistics.Win.UltraWinEditors.UltraTextEditor)sender, e);
        }

        private void txtPorcentajeComisionVendedor_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtPorcentajeComisionVendedor, e);
        }
        #endregion

        private void chkEmpresaAfectaRetencion_CheckedChanged(object sender, EventArgs e)
        {
            txtNroCuentaRetencion.Enabled = chkEmpresaAfectaRetencion.Checked;
            txtTasaRetencion.Enabled = chkEmpresaAfectaRetencion.Checked;
            if (chkEmpresaAfectaRetencion.Checked == false)
            {
                txtNroCuentaRetencion.Clear();
                txtTasaRetencion.Clear();
            }
        }

        #endregion

        private void chkRedondearVentas_CheckedChanged(object sender, EventArgs e)
        {
            txtRedondeoGanancia.Enabled = chkRedondearVentas.Checked;
            txtRedondeoPerdida.Enabled = chkRedondearVentas.Checked;

            if (chkRedondearVentas.Checked) txtRedondeoPerdida.Focus();
        }

        private void txtRedondeoPerdida_DoubleClick(object sender, EventArgs e)
        {
            frmPlanCuentasConsulta f = new frmPlanCuentasConsulta(txtNroCuentaRetencion.Text.Trim());
            f.ShowDialog();
            if (!string.IsNullOrEmpty(f._NroSubCuenta))
            {
                txtRedondeoPerdida.Text = f._NroSubCuenta;
            }
        }

        private void txtRedondeoGanancia_DoubleClick(object sender, EventArgs e)
        {
            frmPlanCuentasConsulta f = new frmPlanCuentasConsulta(txtNroCuentaRetencion.Text.Trim());
            f.ShowDialog();
            if (!string.IsNullOrEmpty(f._NroSubCuenta))
            {
                txtRedondeoGanancia.Text = f._NroSubCuenta;
            }
        }

        private void txtRedondeoPerdida_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRedondeoPerdida.Text.Trim()) && !Utils.Windows.EsCuentaImputable(txtRedondeoPerdida.Text.Trim())) e.Cancel = true;
        }

        private void txtRedondeoGanancia_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRedondeoPerdida.Text.Trim()) && !Utils.Windows.EsCuentaImputable(txtRedondeoPerdida.Text.Trim())) e.Cancel = true;
        }

        private void txtTasaRetencion_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtTasaRetencion, e);
        }

        private void txtDifCambioCtaGanancia_DoubleClick(object sender, EventArgs e)
        {
            var f = new frmPlanCuentasConsulta("");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtDifCambioCtaGanancia.Text = f._NroSubCuenta;
            }
        }

        private void txtDifCambioCtaPerdida_DoubleClick(object sender, EventArgs e)
        {
            var f = new frmPlanCuentasConsulta("");
            f.ShowDialog();
            if (f._NroSubCuenta != null)
            {
                txtDifCambioCtaPerdida.Text = f._NroSubCuenta;
            }
        }

        private void cboEstablecimientoPredet_SelectedIndexChanged(object sender, EventArgs e)
        {
            var objEstablecimientoBl = new EstablecimientoBL();
            if (cboEstablecimientoPredet.Value == null) return;
            var x = objEstablecimientoBl.GetAlmacenesXEstablecimiento(int.Parse(cboEstablecimientoPredet.Value.ToString()));
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", x, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboSeriePredeterminada, "Value1", "Id", _objEstablecimientoBL.ObtenerSerieEstablecimiento(ref _objOperationResult, int.Parse(cboEstablecimientoPredet.Value.ToString())).OrderBy(l => l.Value1).ToList(), DropDownListAction.Select);
            cboSeriePredeterminada.Value = "-1";
        }

        private void txtNroCuentaRetencion_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnEditorEstilos_Click(object sender, EventArgs e)
        {
            var f = new frmRecalcularNroRegistros();
            f.ShowDialog();
        }

        private void txtCuentaUtilidad_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("591");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtCuentaUtilidad.Text = f._NroSubCuenta;
            }
        }

        private void txtCuentaUtilidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCuentaUtilidad, e);
        }

        private void txtCuentaPerdida_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCuentaPerdida, e);
        }

        private void txtResultado891_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtResultado891, e);
        }

        private void txtResultado892_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtResultado892, e);
        }

        private void txtCuentaPerdida_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("592");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtCuentaPerdida.Text = f._NroSubCuenta;
            }
        }

        private void txtResultado891_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("891");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtResultado891.Text = f._NroSubCuenta;
            }
        }

        private void txtResultado892_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("892");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtResultado892.Text = f._NroSubCuenta;
            }
        }

        private void txtCuentaAjuste_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("14");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtCuentaAjuste.Text = f._NroSubCuenta;
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var pobjOperationResult = new OperationResult();
            _objConfiguracionEmpresaBL.ActualizarUsuariosDesdeTIS_Integrado(ref pobjOperationResult);

            if (pobjOperationResult.Success == 0)
            {
                MessageBox.Show(
                    pobjOperationResult.ErrorMessage + '\n' + pobjOperationResult.ExceptionMessage + '\n' +
                    pobjOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblActualizacionUsuarios.Text = @"Usuarios Actualizados";
        }

        private void TxtCuentaContableSoles_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("70");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                TxtCuentaContableSoles.Text = f._NroSubCuenta;
            }
        }

        private void TxtCuentaContableDolares_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("70");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                TxtCuentaContableDolares.Text = f._NroSubCuenta;
            }
        }

        private void txtDifCambioCtaGanancia_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("77");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtDifCambioCtaGanancia.Text = f._NroSubCuenta;
            }
        }

        private void txtDifCambioCtaPerdida_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("67");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtDifCambioCtaPerdida.Text = f._NroSubCuenta;
            }
        }

        private void txtNroCuentaPercepcion_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("40");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtNroCuentaPercepcion.Text = f._NroSubCuenta;
            }
        }

        private void txtNroCuentaISC_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            frmPlanCuentasConsulta f = new frmPlanCuentasConsulta("4012");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtNroCuentaISC.Text = f._NroSubCuenta;
            }
        }

        private void txtNroCuentaOtrosTributos_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            frmPlanCuentasConsulta f = new frmPlanCuentasConsulta("40");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtNroCuentaOtrosTributos.Text = f._NroSubCuenta;
            }
        }

        private void txtNroCuentaGastosF_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("67");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtNroCuentaGastosF.Text = f._NroSubCuenta;
            }
        }

        private void txtNrCuentaIngresosF_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("77");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtNrCuentaIngresosF.Text = f._NroSubCuenta;
            }
        }

        private void btnActualizarDocumentosLetras_Click(object sender, EventArgs e)
        {
            var resp = MessageBox.Show(@"¿Seguro de reubicar los asientos de los canjes?", @"Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resp == DialogResult.No) return;

            ultraButton1.Enabled = false;
            var objOperationResult = new OperationResult();

            new DiarioBL().ReubicarAsientosCanjeLetras(ref objOperationResult,
                int.Parse(cboDocumentoLetrasCobrar.Value.ToString()), 1);

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            new DiarioBL().ReubicarAsientosCanjeLetras(ref objOperationResult,
                int.Parse(cboDocumentoLetrasPagar.Value.ToString()), 2);

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(
                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(@"Proceso Terminado Correctamente", @"Sistema", MessageBoxButtons.OK,
                MessageBoxIcon.Asterisk);
        }

        private static void DescargarPlantilla(string origen)
        {
            try
            {
                var fbd = new FolderBrowserDialog();
                var result = fbd.ShowDialog();
                if (result != DialogResult.OK) return;
                var destino = fbd.SelectedPath + @"\" + Path.GetFileName(origen);
                File.Copy(origen, destino);
                var proc = new Process
                {
                    StartInfo =
                    {
                        FileName = destino,
                        UseShellExecute = true
                    }
                };
                proc.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void linkAsientoDiario_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DescargarPlantilla(@"PlantillasExcel\plantilla_asiento_diario.xlsx");
        }

        private void txtNrCuentaObligacionesF_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("45");
            f.ShowDialog();
            if (f._NroSubCuenta != null) txtNrCuentaObligacionesF.Text = f._NroSubCuenta;
        }

        private void SaveOperaciones()
        {
            var anyChange = false;
            foreach (var row in cboTipoOperacion2.Rows)
            {
                var obj = (KeyValueDTO)row.ListObject;
                if (obj.Id == "-1") continue;
                string field = (bool)row.Cells["check"].Value ? "1" : "0";
                if (obj.Value5 != field)
                {
                    var dto = _objDatahierarchyBL.GetDataHierarchy(ref _objOperationResult, 35, int.Parse(obj.Id));
                    if (dto == null) continue;
                    obj.Value5 = dto.v_Field = field;
                    _objDatahierarchyBL.UpdateDataHierarchy(ref _objOperationResult, dto, Globals.ClientSession.GetAsList());
                    anyChange = true;
                }
            }
            if (anyChange)
                Globals.CacheCombosVentaDto.cboTipoOperacion = _objDatahierarchyBL.GetDataHierarchiesForComboWithValue2(ref _objOperationResult, 35, null, "v_Field == \"1\"");
        }

        private void uvConfiguracionEmpresa_Validating(object sender, Infragistics.Win.Misc.ValidatingEventArgs e)
        {
            if (e.IsValid) return;
            var ctr = e.Control.Parent;
            while (ctr != null)
            {
                var expand = ctr as Infragistics.Win.Misc.UltraExpandableGroupBox;
                if (expand != null)
                {
                    expand.Expanded = true;
                    break;
                }
                ctr = ctr.Parent;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(@"Backgrounds\AnyDesk.exe");
        }

        private void txtNroCuentaAnticipo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("122");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtNroCuentaAnticipo.Text = f._NroSubCuenta;
            }
        }

        private void btnRegenerarAsientosCompra_Click(object sender, EventArgs e)
        {
            var f = new FrmRegenerarAsientosConfig();
            f.ShowDialog();
            if (f.DialogResult != DialogResult.OK) return;
            progressBar1.Visible = true;
            btnRegenerarAsientosCompra.Enabled = false;
            ultraButton2.Enabled = false;
            btnRegenerarAsientosVenta.Enabled = false;
            btnRegenerarTesoreriasDesdeCobranzaDetalle.Enabled = false;
            var mes = f.MesProceso;
            var proceso = new RegenerarAsientosCompraWorker();
            proceso.ErrorEvent += proceso_ErrorEvent;
            proceso.ProcesoEvent += proceso_ProcesoEvent;
            proceso.FinalizadoEvent += proceso_FinalizadoEvent;
            proceso.ComenzarAsync(mes, Globals.ClientSession.i_Periodo ?? DateTime.Now.Year);
            lblEstadoRegeneracion.Text = "Regenerando Compras";
        }

        void proceso_FinalizadoEvent(object sender, EventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                btnRegenerarAsientosCompra.Enabled = true;
                ultraButton2.Enabled = true;
                btnRegenerarAsientosVenta.Enabled = true;
                progressBar1.Visible = false;
                lblEstadoRegeneracion.Text = "Terminado.";
            });
        }

        private void proceso_ProcesoEvent(int porcentaje)
        {
            Invoke((MethodInvoker)delegate { progressBar1.Value = porcentaje; });
        }

        private static void proceso_ErrorEvent(OperationResult pobjOperationResult)
        {
            MessageBox.Show(string.Format("{0}'\n'{1}'\n'{2}", pobjOperationResult.ErrorMessage,
                pobjOperationResult.ExceptionMessage, pobjOperationResult.AdditionalInformation));
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            var f = new FrmRegenerarAsientosConfig();
            f.ShowDialog();
            if (f.DialogResult != DialogResult.OK) return;
            progressBar1.Visible = true;
            btnRegenerarAsientosCompra.Enabled = false;
            ultraButton2.Enabled = false;
            btnRegenerarAsientosVenta.Enabled = false;
            btnRegenerarTesoreriasDesdeCobranzaDetalle.Enabled = false;
            var mes = f.MesProceso;
            var proceso = new RegenerarAsientosCobranzaWorker();
            proceso.ErrorEvent += proceso_ErrorEvent;
            proceso.ProcesoEvent += proceso_ProcesoEvent;
            proceso.FinalizadoEvent += proceso_FinalizadoEvent;
            proceso.ComenzarAsync(mes, Globals.ClientSession.i_Periodo ?? DateTime.Now.Year);
            lblEstadoRegeneracion.Text = @"Regenerando Cobranzas";
        }

        private void btnRegenerarAsientosVenta_Click(object sender, EventArgs e)
        {
            var f = new FrmRegenerarAsientosConfig();
            f.ShowDialog();
            if (f.DialogResult != DialogResult.OK) return;
            progressBar1.Visible = true;
            btnRegenerarAsientosCompra.Enabled = false;
            ultraButton2.Enabled = false;
            btnRegenerarAsientosVenta.Enabled = false;
            btnRegenerarTesoreriasDesdeCobranzaDetalle.Enabled = false;
            var mes = f.MesProceso;
            var proceso = new RegenerarAsientosVentaWorker();
            proceso.ErrorEvent += proceso_ErrorEvent;
            proceso.ProcesoEvent += proceso_ProcesoEvent;
            proceso.FinalizadoEvent += proceso_FinalizadoEvent;
            proceso.ComenzarAsync(mes, Globals.ClientSession.i_Periodo ?? DateTime.Now.Year);
            lblEstadoRegeneracion.Text = @"Regenerando Ventas";
        }

        private void txtNroCtaRetenciones_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmPlanCuentasConsulta("40");
            f.ShowDialog();

            if (f._NroSubCuenta != null)
            {
                txtNroCtaRetenciones.Text = f._NroSubCuenta;
            }
        }

        private void btnRegenerarTesoreriasDesdeCobranzaDetalle_Click(object sender, EventArgs e)
        {
            var f = new FrmRegenerarAsientosConfig();
            f.ShowDialog();
            if (f.DialogResult != DialogResult.OK) return;
            progressBar1.Visible = true;
            btnRegenerarAsientosCompra.Enabled = false;
            ultraButton2.Enabled = false;
            btnRegenerarAsientosVenta.Enabled = false;
            btnRegenerarTesoreriasDesdeCobranzaDetalle.Enabled = false;
            var mes = f.MesProceso;
            var proceso = new RegenerarAsientosCobranzaWorker();
            proceso.ErrorEvent += proceso_ErrorEvent;
            proceso.ProcesoEvent += proceso_ProcesoEvent;
            proceso.FinalizadoEvent += proceso_FinalizadoEvent;
            proceso.ComenzarAsyncCobranzasNotaria(mes, Globals.ClientSession.i_Periodo ?? DateTime.Now.Year);
            lblEstadoRegeneracion.Text = "Regenerando Cobranzas Desde Detalle";
        }

        private void chkListaPrecios_CheckedChanged(object sender, EventArgs e)
        {
            chkListaPreciosDiferentosCostos.Enabled = chkListaPrecios.Checked;
        }

        private void txtNroCuentaRetencion_ValueChanged(object sender, EventArgs e)
        {

        }

        private void txtNroCuentaRetencion_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtNroCuentaRetencion.Text) &&
                (!txtNroCuentaRetencion.Text.Trim().StartsWith("40") || !Utils.Windows.EsCuentaImputable(txtNroCuentaRetencion.Text)))
            {
                MessageBox.Show(@"Debe elegir una cuenta 40 imputable para el régimen de retención", @"Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtNroCuentaRetencion.Focus();
            }
        }

        private void txtNroCuentaRetencion_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void txtNroCuentaRetencion_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            frmPlanCuentasConsulta f = new frmPlanCuentasConsulta(txtNroCuentaRetencion.Text.Trim());
            f.ShowDialog();
            if (!string.IsNullOrEmpty(f._NroSubCuenta))
            {
                if (f._NroSubCuenta.StartsWith("40"))
                {
                    txtNroCuentaRetencion.Text = f._NroSubCuenta;
                }
                else
                {
                    MessageBox.Show(@"Debe elegir una cuenta 40 imputable para el régimen de retención", @"Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtNroCuentaRetencion.Focus();
                }
            }
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

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DescargarPlantilla(@"PlantillasExcel\plantilla_venta_contable.xlsx");
        }

        private void txtConceptoPlanillaTardanzas_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

        }

        private void txtConceptoPlanillaFaltas_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

        }

        private void txtRepresentateLegal_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmBuscarTrabajador();
            f.ShowDialog();
            if (f.TrabajadorconsultaDto == null) return;
            txtRepresentateLegal.Text = f.TrabajadorconsultaDto.NombreApellidos;
            IdRepresentanteLegal = f.TrabajadorconsultaDto.IdTrabajador;
            //txtTrabajador.Tag = f.TrabajadorconsultaDto.IdTrabajador;
        }

        private void ultraCheckEditor1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void txtCantVentaPredet_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCantVentaPredet, e);
        }

        private void txtCantCompraPredet_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCantCompraPredet, e);
        }

        private void ultraGroupBox11_Click(object sender, EventArgs e)
        {

        }

        private void txtArticuloFlete_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmBuscarProductoComprobante();
            f.ShowDialog();
            if (!string.IsNullOrWhiteSpace(f._IdProducto))
            {
                txtArticuloFlete.Text = f._CodigoInternoProducto;
                txtArticuloFlete.Tag = f._IdProducto;
            }
        }

        private void txtSeguroMaritimo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var f = new frmBuscarProductoComprobante();
            f.ShowDialog();
            if (!string.IsNullOrWhiteSpace(f._IdProducto))
            {
                txtSeguroMaritimo.Text = f._CodigoInternoProducto;
                txtSeguroMaritimo.Tag = f._IdProducto;
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DescargarPlantilla(@"PlantillasExcel\plantilla_registro_productos.xlsx");
        }

        private void txtCuentaCobranzaRedondeoGanancia_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox((Infragistics.Win.UltraWinEditors.UltraTextEditor)sender, e);
        }

        private void txtCuentaCobranzaRedondeoPerdida_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox((Infragistics.Win.UltraWinEditors.UltraTextEditor)sender, e);
        }

        private void txtNrCuentaIngresosF_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox((Infragistics.Win.UltraWinEditors.UltraTextEditor)sender, e);
        }

        private void txtNrCuentaObligacionesF_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox((Infragistics.Win.UltraWinEditors.UltraTextEditor)sender, e);
        }

        private void txtNroCuentaGastosF_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox((Infragistics.Win.UltraWinEditors.UltraTextEditor)sender, e);
        }

        private void txtNroCtaRetenciones_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox((Infragistics.Win.UltraWinEditors.UltraTextEditor)sender, e);
        }

        private void txtCuentaCobranzaRedondeoGanancia_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            frmPlanCuentasConsulta f = new frmPlanCuentasConsulta(txtCuentaCobranzaRedondeoGanancia.Text.Trim());
            f.ShowDialog();
            if (!string.IsNullOrEmpty(f._NroSubCuenta))
            {
                txtCuentaCobranzaRedondeoGanancia.Text = f._NroSubCuenta;
            }
        }

        private void txtCuentaCobranzaRedondeoPerdida_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            frmPlanCuentasConsulta f = new frmPlanCuentasConsulta(txtCuentaCobranzaRedondeoPerdida.Text.Trim());
            f.ShowDialog();
            if (!string.IsNullOrEmpty(f._NroSubCuenta))
            {
                txtCuentaCobranzaRedondeoPerdida.Text = f._NroSubCuenta;
            }
        }

        private void ultraLabel12_Click(object sender, EventArgs e)
        {

        }

        private void btnBuscarImagen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.Filter = "Image Files (*.jpg;*.gif;*.jpeg;*.png)|*.jpg;*.gif;*.jpeg;*.png";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                ImagenPathLogo = openFileDialog1.FileName;
                FotoLogoEmpresa.Load(ImagenPathLogo);
                if (!string.IsNullOrEmpty(ImagenPathLogo))
                {
                    Logo = null;
                    Logo = File.ReadAllBytes(ImagenPathLogo);
                }
            }
        }

        private void btnEliminarImagen_Click(object sender, EventArgs e)
        {

        }

        private void FotoLogoEmpresa_MouseEnter(object sender, EventArgs e)
        {
            PictureBox picture = (sender as PictureBox);
            if (picture.Image != null)
            {
                PictureExpandLogoEmpresa.Image = picture.Image;
                popupLogoEmpresa.Show();

            }

            btnBuscarImagen.Visible = true;
            btnEliminarImagenLogo.Visible = true;

            FotoLogoEmpresa.MouseEnter -= FotoLogoEmpresa_MouseEnter;
        }

        private void btnBuscarImagenFirma_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.Filter = "Image Files (*.jpg;*.gif;*.jpeg;*.png)|*.jpg;*.gif;*.jpeg;*.png";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                ImagenPathLogo = openFileDialog1.FileName;

                FotoFirmaEmpresa.Load(ImagenPathLogo);
                if (!string.IsNullOrEmpty(ImagenPathLogo))
                {
                    Firma = null;
                    Firma = File.ReadAllBytes(ImagenPathLogo);
                }
            }
        }



        private void btnEliminarImagenLogo_Click(object sender, EventArgs e)
        {
            FotoLogoEmpresa.Image = null;

            Logo = null;
        }

        private void btnEliminarImagenFirma_Click(object sender, EventArgs e)
        {

            FotoFirmaEmpresa.Image = null;
            Firma = null;
        }

        private void FotoFirmaEmpresa_MouseEnter(object sender, EventArgs e)
        {
            PictureBox picture = (sender as PictureBox);
            if (picture.Image != null)
            {


                PictureExpandFirmaEmpresa.Image = picture.Image;
                popupFirmaEmpresa.Show();


            }

            btnBuscarImagenFirma.Visible = true;
            btnEliminarImagenFirma.Visible = true;
            FotoFirmaEmpresa.MouseEnter -= FotoLogoEmpresa_MouseEnter;
        }

        private void ultraButton3_Click(object sender, EventArgs e)
        {
            var f = new FrmMigrarDataPeriodos();
            f.ShowDialog();
        }
    }
}
