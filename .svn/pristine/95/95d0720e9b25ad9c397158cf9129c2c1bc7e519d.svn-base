#region Referencias
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
using SAMBHS.Compra.BL;
using Infragistics.Win.UltraWinGrid;
using System.Text.RegularExpressions;
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Security.BL;
using SAMBHS.Contabilidad.BL;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using Infragistics.Win;
using LoadingClass;
using SAMBHS.Letras.BL;
using SAMBHS.Windows.WinClient.UI.Mantenimientos.UserControls;
using SAMBHS.Requerimientos.NBS;
#endregion

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class frmRegistroVentaNBS : Form
    {
        #region Declaraciones / Referencias
        CierreMensualBL _objCierreMensualBL = new CierreMensualBL();
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
        Boolean GlosaEditada = false;
        Boolean CargarFormularioKardex = true;
        Boolean Loading = true;
        #endregion

        int _MaxV, _ActV;
        string _Mode, _IdVentaGuardada;
        public string _pstrIdMovimiento_Nuevo;
        string strModo = "Nuevo", strIdVenta;
        bool _EsNotadeCredito;
        string _idVenta, CtaRubroMercaderia = "-1", CtaRubroServicio = "-1";
        decimal Total, Redondeado, Residuo;
        bool _Redondeado = false;
        public int IdIdentificacion = 0;
        bool _guardadoSinProceso = false;
        bool EditarTodo = true;
        private bool EsDocumentoInternoPermiteMontoCero
        {
            get
            {
                if (cboDocumento.Value == null) return false;
                int i;
                var permiteCero = Globals.ClientSession.i_PermiteIncluirPreciosCeroPedido == 1;
                i = int.TryParse(cboDocumento.Value.ToString(), out i) ? i : -1;
                var esDocInterno = !new DocumentoBL().DocumentoEsContable(i);
                return permiteCero && esDocInterno;
            }
        }

        BindingList<nbs_ventakardexDto> ListaGrilla = new BindingList<nbs_ventakardexDto>();
        List<nbs_ventakardexDto> _TempDetalle_EliminarKardexDto = new List<nbs_ventakardexDto>();
        List<nbs_ventakardexDto> _TempDetalle_AgregarKardexDto = new List<nbs_ventakardexDto>();
        List<nbs_ventakardexDto> _TempDetalle_ModificarKardexDto = new List<nbs_ventakardexDto>();
        List<string> ListaTipoKardex = new List<string>();
        #region Temporales Detalles de Venta
        List<ventadetalleDto> _TempDetalle_AgregarDto = new List<ventadetalleDto>();
        List<ventadetalleDto> _TempDetalle_ModificarDto = new List<ventadetalleDto>();
        List<ventadetalleDto> _TempDetalle_EliminarDto = new List<ventadetalleDto>();
        #endregion

        #region Temporales NotaSalidaDetalle
        List<movimientodetalleDto> __TempDetalle_AgregarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> __TempDetalle_ModificarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> __TempDetalle_EliminarDto = new List<movimientodetalleDto>();
        #endregion

        #region Permisos Botones
        bool _btnGuardar = false;
        bool _btnImprimir = false;
        #endregion

        public frmRegistroVentaNBS(string Modo, string IdVenta)
        {
            strModo = Modo;
            _IdVentaGuardada = strIdVenta = IdVenta;
            InitializeComponent();
        }

        private void frmRegistroVentaNBS_Load(object sender, EventArgs e)
        {
            using (new PleaseWait(this.Location, "Por favor espere..."))
            {
                UltraStatusbarManager.Inicializar(ultraStatusBar1);
                OperationResult objOperationResult = new OperationResult();
                #region ControlAcciones
                //if (_objCierreMensualBL.VerificarMesCerrado(Globals.ClientSession.i_Periodo.ToString().Trim(), DateTime.Now.Month.ToString("00").Trim(), "VENTAS/FACTURACIÓN"))
                //{
                //    btnGuardar.Visible = false;
                //    this.Text = "Registro de Venta [MES CERRADO]";
                //}
                //else
                //{

                //    btnGuardar.Visible = true;
                //    this.Text = "Registro de Venta";
                //}
                //var _formActions = _objSecurityBL.GetFormAction(ref objOperationResult, Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId, "frmRegistroVenta", Globals.ClientSession.i_RoleId);

                //_btnImprimir = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmRegistroVenta_PRINT", _formActions);
                //_btnGuardar = SAMBHS.Common.Resource.Utils.Windows.IsActionEnabled("frmRegistroVenta_SAVE", _formActions);
                _btnImprimir = true;
                _btnGuardar = true;

                btnGuardar.Enabled = _btnGuardar;
                #endregion
                this.BackColor = new GlobalFormColors().FormColor;
                panel1.BackColor = new GlobalFormColors().BannerColor;
                txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
                txtMes.Text = DateTime.Now.Month.ToString("00");
                #region Cargar Combos
                cboDocumento.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
                _ListadoComboDocumentos = Globals.CacheCombosVentaDto.cboDocumentos;
                _ListadoComboDocumentosRef = Globals.CacheCombosVentaDto.cboDocumentosRef;
                Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", Globals.CacheCombosVentaDto.cboDocumentos, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboList(cboDocumentoRef, "Value2", "Id", Globals.CacheCombosVentaDto.cboDocumentosRef.Where(x => !x.EsDocInterno).ToList(), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboEstado, "Value1", "Id", Globals.CacheCombosVentaDto.cboEstado, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboCondicionPago, "Value1", "Id", Globals.CacheCombosVentaDto.cboCondicionPago, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", Globals.CacheCombosVentaDto.cboEstablecimiento, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboIGV, "Value1", "Id", Globals.CacheCombosVentaDto.cboIGV, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboTipoVenta, "Value1", "Id", Globals.CacheCombosVentaDto.cboTipoVenta, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboMVenta, "Value1", "Id", Globals.CacheCombosVentaDto.cboMVenta, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboPuntoDestino, "Value1", "Id", Globals.CacheCombosVentaDto.cboPuntoDestino, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboPuntoEmbarque, "Value1", "Id", Globals.CacheCombosVentaDto.cboPuntoEmbarque, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboTipoEmbarque, "Value1", "Id", Globals.CacheCombosVentaDto.cboTipoEmbarque, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboTipoOperacion, "Value1", "Id", Globals.CacheCombosVentaDto.cboTipoOperacion, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id", Globals.CacheCombosVentaDto.cboVendedor, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboVendedorRef, "Value1", "Id", Globals.CacheCombosVentaDto.cboVendedorRef, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboTipoKardex, "Value1", "Value2", Globals.CacheCombosVentaDto.cboTiposKardex, DropDownListAction.Select);
                CargarCombosDetalle();
                #endregion

                if (Globals.ClientSession.i_VentasMostrarEmpaque == null || Globals.ClientSession.i_VentasMostrarEmpaque == 0)
                {
                    this.grdData.DisplayLayout.Bands[0].Columns["Empaque"].Hidden = true;
                    this.grdData.DisplayLayout.Bands[0].Columns["UMEmpaque"].Hidden = true;
                }

                if (Globals.ClientSession.i_VentasIscOtrosTributos == null || Globals.ClientSession.i_VentasIscOtrosTributos.Value == 0)
                {
                    this.grdData.DisplayLayout.Bands[0].Columns["d_isc"].Hidden = true;
                    this.grdData.DisplayLayout.Bands[0].Columns["d_otrostributos"].Hidden = true;
                }
                cboDocumento.Value = "-1";
                cboDocumentoRef.Value = "-1";
                cboTipoKardex.Value = "-1";
                Loading = false;
                ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);
                dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim())))));
                FormatoDecimalesGrilla((int)Globals.ClientSession.i_CantidadDecimales, (int)Globals.ClientSession.i_PrecioDecimales);
                LlenarCuentasRubros();
                Utils.Windows.SetLimitesPeriodo(dtpFechaRegistro);
                cboEstado.Enabled = VentaBL.TienePermisoAnular;
            }
            Activate();
            FilasAnticipioActivacion(chkDeduccionAnticipio.Checked);

            if (string.IsNullOrEmpty(txtSerieDoc.Text) && string.IsNullOrEmpty(txtCorrelativoDocIni.Text))
            {
                if (cboDocumento.Text.Trim() == "")
                {
                    cboDocumento.Value = "-1";
                }
                else
                {
                    var x = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento.Value.ToString() || p.Id == cboDocumento.Text);
                    if (x == null)
                    {
                        cboDocumento.Value = "-1";
                    }
                }

                txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).Trim();
                txtCorrelativoDocIni.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000").Trim();
                PredeterminarEstablecimiento(txtSerieDoc.Text);
                if (cboDocumento.Value != null && int.Parse(cboDocumento.Value.ToString()) != (int)DocumentType.Irpe && cboDocumento.Value.ToString() != "-1" && !string.IsNullOrEmpty(UserConfig.Default.SerieCaja))
                {
                    txtSerieDoc.Text = UserConfig.Default.SerieCaja;
                }
                CancelEventArgs e1 = new CancelEventArgs();
                txtSerieDoc_Validating(sender, e1);
                PredeterminarEstablecimiento(txtSerieDoc.Text.Trim());
            }

            ListaTipoKardex.Add("K");
            ListaTipoKardex.Add("C");
            ListaTipoKardex.Add("P");
            ListaTipoKardex.Add("D");
            ListaTipoKardex.Add("L");
            ListaTipoKardex.Add("E");
            ListaTipoKardex.Add("N");
            ListaTipoKardex.Add("H");
            ListaTipoKardex.Add("M");
            ListaTipoKardex.Add("T");
            if (strModo == "Nuevo")
            {
                if (cboDocumento.Value != null && int.Parse(cboDocumento.Value.ToString()) != (int)DocumentType.Irpe && cboDocumento.Value.ToString() != "-1" && !string.IsNullOrEmpty(UserConfig.Default.SerieCaja) && UserConfig.Default.SerieCaja != "--Seleccionar--")
                {
                    txtSerieDoc.Text = UserConfig.Default.SerieCaja;
                }
                CancelEventArgs e1 = new CancelEventArgs();
                txtSerieDoc_Validating(sender, e1);
                PredeterminarEstablecimiento(txtSerieDoc.Text.Trim());
            }
        }

        #region Eventos Generales

        private void btnAgregar_Click(object sender, EventArgs e)
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

            var ultimaFila = grdData.Rows.LastOrDefault();
            if (ultimaFila == null || (ultimaFila.Cells["i_Anticipio"].Value != null && ultimaFila.Cells["i_Anticipio"].Value.ToString() == "1") || ultimaFila.Cells["v_IdProductoDetalle"].Value != null)
            {
                UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Agregado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["i_IdUnidadMedida"].Value = "-1";
                row.Cells["d_Cantidad"].Value = "0";
                row.Cells["d_Precio"].Value = "0";
                row.Cells["i_IdUnidadMedida"].Value = "-1";
                row.Cells["i_Anticipio"].Value = "0";
                row.Cells["d_isc"].Value = "0";
                row.Cells["d_otrostributos"].Value = "0";
                row.Cells["d_Descuento"].Value = "0";
                row.Cells["d_Valor"].Value = "0";
                row.Cells["v_NroCuenta"].Value = "-1";
                row.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null && cboTipoOperacion.Value.ToString() != "5" ? cboTipoOperacion.Value.ToString() : "1";
                row.Cells["i_IdCentroCosto"].Value = "0";
                row.Cells["d_Percepcion"].Value = "0";
                row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                row.Cells["i_Anticipio"].Activation = chkDeduccionAnticipio.Checked ? Activation.AllowEdit : Activation.Disabled;
            }

            UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_NroCuenta"];
            this.grdData.ActiveCell = aCell;
            grdData.Focus();
            grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (_Mode == "New" && grdData.Rows.Any(fila => fila.Cells["v_NroCuenta"].Value != null && fila.Cells["v_NroCuenta"].Value.ToString() != "-1") &&
                grdData.Rows.Any(fila => fila.Cells["v_NroCuenta"].Value == null || fila.Cells["v_NroCuenta"].Value.ToString() == "-1"))
            {
                var filasVacias = grdData.Rows.Where(fila => fila.Cells["v_NroCuenta"].Value == null || fila.Cells["v_NroCuenta"].Value.ToString() == "-1").ToList();
                filasVacias.ForEach(fila => fila.Delete(false));
            }

            int _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            var filasActuales = grdData.Rows.Count;
            if (filasActuales > _limiteDocumento)
            {
                UltraMessageBox.Show("Imposible Guardar , Máximo número de items por documento alcanzado, elimine un Fila de los detalles", "Advertencia");
                return;
            }

            OperationResult objOperationResult = new OperationResult();

            if (uvDatos.Validate(true, false).IsValid)
            {
                if (grdData.Rows.Count(x => x.Cells["v_NroCuenta"].Value == null) > 0 || grdData.Rows.Any(x => x.Cells["v_NroCuenta"].Value.ToString() == "-1"))
                {
                    UltraMessageBox.Show("Porfavor ingrese correctamente los Rubros", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    grdData.Rows.First(x => x.Cells["v_NroCuenta"].Value == null || x.Cells["v_NroCuenta"].Value.ToString() == "-1").Selected = true;
                    return;
                }

                #region ValidacionKardex
                if (cboTipoKardex.Value != null)
                {
                    if (cboTipoKardex.Value.ToString() == "V" && ListaGrilla.Any())
                    {

                        if (!ValidaCamposNulosVaciosKardex())
                        {
                        }
                    }
                    else if (cboTipoKardex.Value.ToString() == "V" && !ListaGrilla.Any())
                    {
                    }
                    else if (cboTipoKardex.Value.ToString() != "V" && string.IsNullOrEmpty(txtNroKardex.Text.Trim()))
                    {

                        UltraMessageBox.Show("Por favor registre Número de Kardex", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtNroKardex.Focus();
                        return;
                    }
                    else if (string.IsNullOrEmpty(txtNroKardex.Text.Trim()) && int.Parse(cboTipoKardex.Value.ToString()) == -1)
                    {

                        UltraMessageBox.Show("Por favor registre el Tipo de Kardex", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        cboTipoKardex.Focus();
                        return;
                    }
                    else if (!EsDocumentoInternoPermiteMontoCero && ListaTipoKardex.Contains(cboTipoKardex.Value.ToString()))
                    {

                        if (string.IsNullOrEmpty(txtMonto.Text.Trim()) || decimal.Parse(txtMonto.Text) == 0)
                        {
                            UltraMessageBox.Show("Por favor registre el Monto", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtMonto.Focus();
                            return;
                        }

                    }

                    if (!EsDocumentoInternoPermiteMontoCero && !string.IsNullOrEmpty(txtMonto.Text.Trim()))
                    {
                        if (decimal.Parse(txtMonto.Text) == 0)
                        {
                            UltraMessageBox.Show("Por favor registre el Monto", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtMonto.Focus();
                            return;
                        }
                    }

                    if (decimal.Parse(txtMonto.Text) != decimal.Parse(txtTotal.Text))
                    {
                        if (UltraMessageBox.Show("El monto de los Kardex es diferente al Total de la Venta, ¿Desea Continuar?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.No)
                        {
                            txtMonto.Focus();
                            return;
                        }
                    }
                }
                #endregion






                if (_Mode != "New")
                {
                    if (new LetrasBL().VentaFueCanjeadaALetras(ref objOperationResult, _ventaDto.v_IdVenta))
                    {
                        UltraMessageBox.Show("Esta venta fue canjeada en letras, no se puede editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                    string ubicacion;
                    if (_objVentasBL.TieneCobranzasRealizadas(_ventaDto.v_IdVenta, out ubicacion) == true && EditarTodo)
                    {
                        UltraMessageBox.Show("Imposible Guardar Cambios a un Documento con Cobranzas Realizadas \r" + ubicacion, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    var GuiasRemision = _objVentasBL.ObtenerDetalleGuiaRemisionporDocumentoRef(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim(), txtCorrelativoDocIni.Text.Trim());
                    if (GuiasRemision.Any())
                    {
                        var ProdVenta = grdData.Rows.Where(p => p.Cells["i_RegistroEstado"].Value != null && p.Cells["i_RegistroEstado"].Value.ToString() == "Modificado").Select(p => p.Cells["v_IdProductoDetalle"].Value.ToString()).ToList();

                        if (ProdVenta.Any() || _TempDetalle_EliminarDto.Any())
                        {
                            UltraMessageBox.Show("Imposible Guardar Cambios a un Documento con Guia de Remisión Generada \n" + "Nro Guia Generada :" + GuiasRemision.FirstOrDefault().v_Observacion, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }

                if (EsVentaAfectaDetraccion())
                {
                    if (UltraMessageBox.Show("El documento es Afecto Detracción, ¿Desea continuar?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                }

                #region Validaciones Generales
                if (int.Parse(cboEstado.Value.ToString()) == 1)
                {
                    if (_ventaDto.i_IdEstado == 0)
                    {
                        UltraMessageBox.Show("No es posible Activar un documento Anulado anteriormente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                if (cboDocumento.Value.ToString() != "-1")
                {
                    if (cboEstado.Value.ToString() != "0" && (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) || cboDocumento.Value.ToString() == "8"))
                    {
                        if (cboDocumentoRef.Value.ToString() == "-1" || txtSerieDocRef.Text.Trim() == "" || txtCorrelativoDocRef.Text.Trim() == "")
                        {
                            UltraMessageBox.Show("Por favor ingrese un documento de referencia válido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            cboDocumentoRef.Focus();
                            return;
                        }
                    }
                }
                else
                {
                    UltraMessageBox.Show("Por favor ingrese un documento válido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    cboDocumento.Focus();
                    return;
                }

                if (decimal.Parse(txtTipoCambio.Text.Trim()) <= 0)
                {
                    UltraMessageBox.Show("Por favor ingrese un tipo de cambio válido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (grdData.Rows.Any(p => p.Cells["v_IdProductoDetalle"].Value == null && (!string.IsNullOrEmpty(p.Cells["v_NroCuenta"].Text) && !p.Cells["v_NroCuenta"].Text.Contains("ANTICIPIO"))) && !_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())))
                {
                    if (cboEstado.Value.ToString() != "0")
                    {
                        UltraMessageBox.Show("Uno de los artículos esta incorrectamente ingresado", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        grdData.Rows.First(p => p.Cells["v_IdProductoDetalle"].Value == null).Selected = true;
                        return;
                    }
                }

                if (!_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) && (!grdData.Rows.Any() || grdData.Rows.Count(p => p.Cells["v_IdProductoDetalle"].Value != null && p.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000") == 0))
                {
                    if (cboEstado.Value.ToString() != "0")
                    {
                        UltraMessageBox.Show("No se permite guardar mientras el detalle está vacío", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else
                {
                    if (ValidaCamposNulosVacios())
                    {
                        foreach (UltraGridRow Fila in grdData.Rows)
                        {
                            Fila.Cells["i_ValidarStock"].Value = Fila.Cells["i_ValidarStock"].Value == null ? "0" : Fila.Cells["i_ValidarStock"].Value;

                            if (Fila.Cells["v_NroCuenta"].Value == null)
                            {
                                UltraMessageBox.Show("Por favor ingrese correctamente todas las cuentas al detalle.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            if (!EsDocumentoInternoPermiteMontoCero && Fila.Cells["EsRedondeo"].Value == null && decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString()) <= 0)
                            {
                                UltraMessageBox.Show("Todos los Totales no están calculados.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Fila.Activate();
                                return;
                            }

                            if (Fila.Cells["i_IdAlmacen"].Value == null && Fila.Cells["v_IdProductoDetalle"].Value != null)
                            {
                                UltraMessageBox.Show("Porfavor especifique los Almacenes correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            string IdVentaDetalle = Fila.Cells["v_IdVentaDetalle"].Value != null ? Fila.Cells["v_IdVentaDetalle"].Value.ToString() : null;

                            // if (Fila.Cells["i_EsServicio"].Value.ToString() != "1" && cboDocumento.Value.ToString() != "7" && cboDocumento.Value.ToString() != "8" && Fila.Cells["i_ValidarStock"].Value.ToString() == "1")

                            if (!_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) && Fila.Cells["i_EsServicio"].Value.ToString() != "1" && cboDocumento.Value.ToString() != "8" && Fila.Cells["i_ValidarStock"].Value.ToString() == "1")
                            {
                                decimal Excedente;
                                Excedente = _objVentasBL.CantidadExcedentePorVentaDetalle(int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()), Fila.Cells["v_IdProductoDetalle"].Value.ToString(), decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()), IdVentaDetalle, int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString()), Fila.Cells["v_PedidoExportacion"].Value == null ? null : Fila.Cells["v_PedidoExportacion"].Value.ToString(), Fila.Cells["v_NroSerie"].Value == null ? null : Fila.Cells["v_NroSerie"].Value.ToString(), Fila.Cells["v_NroLote"].Value == null ? null : Fila.Cells["v_NroLote"].Value.ToString());

                                if (Excedente < 0)
                                {
                                    UltraMessageBox.Show("La cantidad ingresada sobrepasa el Saldo disponible en stock", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    UltraGridCell aCell = Fila.Cells["d_Cantidad"];
                                    grdData.ActiveRow = Fila;
                                    this.grdData.ActiveCell = aCell;
                                    grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                                    grdData.Focus();
                                    return;
                                }
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
                    }
                    else
                    {
                        return;
                    }
                }

                CalcularValoresDetalle();

                if (!VerificarLineaCreditoCliente(_ventaDto.v_IdCliente))
                {
                    return;
                }
                #endregion

                if (_Mode == "New")
                {
                    using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                    {
                        while (_objVentasBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text) == false)
                        {
                            txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                        }

                        while (_objVentasBL.ExisteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text, txtCorrelativoDocIni.Text) == false)
                        {
                            txtCorrelativoDocIni.Text = (int.Parse(txtCorrelativoDocIni.Text) + 1).ToString("00000000");
                        }
                        LlenarTemporalesKardex_();
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
                        _ventaDto.t_FechaRef = dtpFechaRef.Value;
                        _ventaDto.t_FechaVencimiento = dtpFechaVencimiento.Value;
                        _ventaDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                        _ventaDto.i_IdCondicionPago = int.Parse(cboCondicionPago.Value.ToString());
                        _ventaDto.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());
                        _ventaDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                        _ventaDto.i_IdIgv = int.Parse(cboIGV.Value.ToString()); //evaluar
                        _ventaDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                        _ventaDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                        _ventaDto.i_IdTipoDocumentoRef = int.Parse(cboDocumentoRef.Value.ToString());
                        _ventaDto.i_PreciosIncluyenIgv = chkPrecInIGV.Checked == true ? 1 : 0;
                        _ventaDto.v_CorrelativoDocumentoRef = txtCorrelativoDocRef.Text;
                        _ventaDto.v_Mes = txtMes.Text;
                        _ventaDto.v_Periodo = txtPeriodo.Text;
                        if (!_guardadoSinProceso)
                        {
                            _ventaDto.v_SerieDocumento = txtSerieDoc.Text;
                            _ventaDto.v_CorrelativoDocumento = txtCorrelativoDocIni.Text.Trim();
                            _ventaDto.v_CorrelativoDocumentoFin = txtCorrelativoDocFin.Text.Trim();
                        }
                        _ventaDto.v_Concepto = txtConcepto.Text.Trim();
                        _ventaDto.v_SerieDocumentoRef = txtSerieDocRef.Text;
                        _ventaDto.d_PorcDescuento = txtPorcDescuento.Text == string.Empty ? 0 : decimal.Parse(txtPorcDescuento.Text);
                        _ventaDto.d_PocComision = txtPorcComision.Text == string.Empty ? 0 : decimal.Parse(txtPorcComision.Text);
                        _ventaDto.d_Descuento = txtDescuento.Text == string.Empty ? 0 : decimal.Parse(txtDescuento.Text);
                        _ventaDto.v_BultoDimensiones = txtDimensiones.Text;
                        _ventaDto.v_NroGuiaRemisionCorrelativo = txtGuiaRemisionCorrelativo.Text.Trim();
                        _ventaDto.v_NroGuiaRemisionSerie = txtGuiaRemisionSerie.Text.Trim();
                        _ventaDto.d_IGV = txtIGV.Text == string.Empty ? 0 : decimal.Parse(txtIGV.Text.Trim());
                        _ventaDto.v_Marca = txtMarca.Text.Trim();
                        _ventaDto.v_NroBulto = txtNroBulto.Text.Trim();
                        _ventaDto.i_NroDias = txtNroDias.Text == string.Empty ? 0 : int.Parse(txtNroDias.Text.Trim());
                        _ventaDto.v_NroPedido = txtNroPedido.Text;
                        _ventaDto.v_OrdenCompra = txtOrdenCompra.Text.Trim();
                        _ventaDto.d_PesoBrutoKG = txtPesoBrutoKg.Text == string.Empty ? 0 : decimal.Parse(txtPesoBrutoKg.Text.Trim());
                        _ventaDto.d_PesoNetoKG = txtPesoNetoKg.Text == string.Empty ? 0 : decimal.Parse(txtPesoNetoKg.Text.Trim());
                        _ventaDto.t_FechaOrdenCompra = dtpFechaOrden.Value;
                        _ventaDto.i_IdMedioPagoVenta = int.Parse(cboMVenta.Value.ToString());
                        _ventaDto.i_IdPuntoDestino = int.Parse(cboPuntoDestino.Value.ToString());
                        _ventaDto.i_IdPuntoEmbarque = int.Parse(cboPuntoEmbarque.Value.ToString());
                        _ventaDto.i_IdTipoEmbarque = int.Parse(cboTipoEmbarque.Value.ToString());
                        _ventaDto.i_IdTipoOperacion = int.Parse(cboTipoOperacion.Value.ToString());
                        _ventaDto.i_IdTipoVenta = int.Parse(cboTipoVenta.Value.ToString());
                        _ventaDto.i_DrawBack = chkDrawnBack.Checked == true ? 1 : 0;
                        _ventaDto.v_IdVendedor = cboVendedor.Value.ToString();
                        _ventaDto.v_IdVendedorRef = cboVendedorRef.Value.ToString();
                        _ventaDto.v_NombreClienteTemporal = _ventaDto.v_IdCliente == "N002-CL000000000" ? txtRazonSocial.Text.ToString() : string.Empty;
                        // _ventaDto.v_DireccionClienteTemporal = _ventaDto.v_IdCliente == "N002-CL000000000" ? txtDireccion.Text.ToString() : string.Empty;
                        _ventaDto.v_DireccionClienteTemporal = txtDireccion.Text.ToString();
                        _ventaDto.NombreCliente = txtRazonSocial.Text;
                        _ventaDto.CodigoCliente = txtCodigoCliente.Text;
                        _ventaDto.d_total_isc = decimal.Parse(txtISC.Text);
                        _ventaDto.d_total_otrostributos = decimal.Parse(txtOtrosTributos.Text);
                        _ventaDto.d_ValorFOB = !string.IsNullOrEmpty(txtValorFOB.Text) ? decimal.Parse(txtValorFOB.Text.Trim()) : 0;
                        _ventaDto.v_PlacaVehiculo = txtPlacaVehiculo.Text.Trim();

                        _ventaDto.v_NroFUF = txtNroFuf.Text.Trim();
                        //_ventaDto.v_IdTipoKardex = txtTipoKardex.Text.Trim();
                        _ventaDto.v_IdTipoKardex = cboTipoKardex.Value == null || cboTipoKardex.Value.ToString() == "-1" ? "" : cboTipoKardex.Value.ToString();
                        LlenarTemporalesVenta();

                        _IdVentaGuardada = _objVentasBL.InsertarVenta(ref objOperationResult, _ventaDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_AgregarKardexDto, _guardadoSinProceso);
                        #endregion

                        var objDbfSincro = new DbfSincronizador();
                        var objOperationResult2 = new OperationResult();
                        _ventaDto.v_IdVenta = _IdVentaGuardada;
                        objDbfSincro.RutaDbfCabecera = NBS_DBF_PathSettings.Default.dbfSincro_Cabecera;
                        objDbfSincro.RutaDbfDetalle = NBS_DBF_PathSettings.Default.dbfSincro_Detalle;
                        objDbfSincro.ActualizarDatosVenta(ref objOperationResult2, _IdVentaGuardada, DbfSincronizador.TipoAccion.Venta);
                        if (objOperationResult2.Success == 0)
                        {
                            MessageBox.Show(objOperationResult2.ErrorMessage, @"Error al sincronizar DBF", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
                else if (_Mode == "Edit")
                {
                    using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                    {
                        LlenarTemporalesKardex_();
                        #region Actualiza Entidad Venta
                        _ventaDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                        _ventaDto.d_Anticipio = txtAnticipio.Text != string.Empty ? decimal.Parse(txtAnticipio.Text) : 0;
                        _ventaDto.d_IGV = txtIGV.Text != string.Empty ? decimal.Parse(txtIGV.Text) : 0;
                        _ventaDto.d_TipoCambio = txtTipoCambio.Text != string.Empty ? decimal.Parse(txtTipoCambio.Text) : 0;
                        _ventaDto.d_Valor = txtValor.Text != string.Empty ? decimal.Parse(txtValor.Text) : 0;
                        _ventaDto.d_Total = txtTotal.Text != string.Empty ? decimal.Parse(txtTotal.Text) : 0;
                        _ventaDto.d_ValorVenta = txtValorVenta.Text != string.Empty ? decimal.Parse(txtValorVenta.Text) : 0;
                        _ventaDto.i_DeduccionAnticipio = chkDeduccionAnticipio.Checked == true ? 1 : 0;
                        _ventaDto.i_EsAfectoIgv = chkAfectoIGV.Checked == true ? 1 : 0;
                        _ventaDto.t_FechaRef = dtpFechaRef.Value;
                        _ventaDto.t_FechaVencimiento = dtpFechaVencimiento.Value;
                        _ventaDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                        _ventaDto.i_IdCondicionPago = int.Parse(cboCondicionPago.Value.ToString());
                        _ventaDto.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());
                        _ventaDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                        _ventaDto.i_IdIgv = int.Parse(cboIGV.Value.ToString()); //evaluar
                        _ventaDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                        _ventaDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                        _ventaDto.i_IdTipoDocumentoRef = int.Parse(cboDocumentoRef.Value.ToString());
                        _ventaDto.i_PreciosIncluyenIgv = chkPrecInIGV.Checked == true ? 1 : 0;
                        _ventaDto.v_CorrelativoDocumentoRef = txtCorrelativoDocRef.Text;
                        _ventaDto.v_Mes = int.Parse(txtMes.Text.Trim()).ToString("00");
                        _ventaDto.v_Periodo = txtPeriodo.Text;
                        _ventaDto.v_Concepto = txtConcepto.Text;
                        if (!_guardadoSinProceso)
                        {
                            _ventaDto.v_SerieDocumento = txtSerieDoc.Text;
                            _ventaDto.v_CorrelativoDocumento = txtCorrelativoDocIni.Text.Trim();
                            _ventaDto.v_CorrelativoDocumentoFin = txtCorrelativoDocFin.Text.Trim();
                        }
                        _ventaDto.v_SerieDocumentoRef = txtSerieDocRef.Text;
                        _ventaDto.d_PorcDescuento = txtPorcDescuento.Text == string.Empty ? 0 : decimal.Parse(txtPorcDescuento.Text);
                        _ventaDto.d_PocComision = txtPorcComision.Text == string.Empty ? 0 : decimal.Parse(txtPorcComision.Text);
                        _ventaDto.d_Descuento = txtDescuento.Text == string.Empty ? 0 : decimal.Parse(txtDescuento.Text);
                        _ventaDto.v_BultoDimensiones = txtDimensiones.Text;
                        _ventaDto.v_NroGuiaRemisionCorrelativo = txtGuiaRemisionCorrelativo.Text.Trim();
                        _ventaDto.v_NroGuiaRemisionSerie = txtGuiaRemisionSerie.Text.Trim();
                        _ventaDto.d_IGV = txtIGV.Text == string.Empty ? 0 : decimal.Parse(txtIGV.Text.Trim());
                        _ventaDto.v_Marca = txtMarca.Text.Trim();
                        _ventaDto.v_NroBulto = txtNroBulto.Text.Trim();
                        _ventaDto.i_NroDias = txtNroDias.Text == string.Empty ? 0 : int.Parse(txtNroDias.Text.Trim());
                        _ventaDto.v_NroPedido = txtNroPedido.Text;
                        _ventaDto.v_OrdenCompra = txtOrdenCompra.Text.Trim();
                        _ventaDto.d_PesoBrutoKG = txtPesoBrutoKg.Text == string.Empty ? 0 : decimal.Parse(txtPesoBrutoKg.Text.Trim());
                        _ventaDto.d_PesoNetoKG = txtPesoNetoKg.Text == string.Empty ? 0 : decimal.Parse(txtPesoNetoKg.Text.Trim());
                        _ventaDto.t_FechaOrdenCompra = dtpFechaOrden.Value;
                        _ventaDto.i_DrawBack = chkDrawnBack.Checked == true ? 1 : 0;
                        _ventaDto.i_IdMedioPagoVenta = int.Parse(cboMVenta.Value.ToString());
                        _ventaDto.i_IdPuntoDestino = int.Parse(cboPuntoDestino.Value.ToString());
                        _ventaDto.i_IdPuntoEmbarque = int.Parse(cboPuntoEmbarque.Value.ToString());
                        _ventaDto.i_IdTipoEmbarque = int.Parse(cboTipoEmbarque.Value.ToString());
                        _ventaDto.i_IdTipoOperacion = int.Parse(cboTipoOperacion.Value.ToString());
                        _ventaDto.i_IdTipoVenta = int.Parse(cboTipoVenta.Value.ToString());
                        _ventaDto.v_IdVendedor = cboVendedor.Value.ToString();
                        _ventaDto.v_IdVendedorRef = cboVendedorRef.Value.ToString();
                        _ventaDto.v_NombreClienteTemporal = _ventaDto.v_IdCliente == "N002-CL000000000" ? txtRazonSocial.Text.ToString() : string.Empty;
                        // _ventaDto.v_DireccionClienteTemporal = _ventaDto.v_IdCliente == "N002-CL000000000" ? txtDireccion.Text.ToString() : string.Empty;
                        _ventaDto.v_DireccionClienteTemporal = txtDireccion.Text.ToString();
                        _ventaDto.NombreCliente = txtRazonSocial.Text;
                        _ventaDto.CodigoCliente = txtCodigoCliente.Text;
                        _ventaDto.d_total_isc = decimal.Parse(txtISC.Text);
                        _ventaDto.d_total_otrostributos = decimal.Parse(txtOtrosTributos.Text);
                        _ventaDto.d_ValorFOB = !string.IsNullOrEmpty(txtValorFOB.Text) ? decimal.Parse(txtValorFOB.Text.Trim()) : 0;
                        _ventaDto.v_PlacaVehiculo = txtPlacaVehiculo.Text.Trim();
                        _ventaDto.v_NroFUF = txtNroFuf.Text.Trim();
                        _ventaDto.v_IdTipoKardex = cboTipoKardex.Value == null || cboTipoKardex.Value.ToString() == "-1" ? "" : cboTipoKardex.Value.ToString();
                        LlenarTemporalesVenta();

                        _objVentasBL.ActualizarVenta(ref objOperationResult, _ventaDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto,
                            _TempDetalle_ModificarDto, _TempDetalle_EliminarDto, _TempDetalle_AgregarKardexDto, _TempDetalle_ModificarKardexDto,
                            _TempDetalle_EliminarKardexDto, _guardadoSinProceso);
                        #endregion

                        var objDbfSincro = new DbfSincronizador();
                        var objOperationResult2 = new OperationResult();
                        _ventaDto.v_IdVenta = _IdVentaGuardada;
                        objDbfSincro.RutaDbfCabecera = NBS_DBF_PathSettings.Default.dbfSincro_Cabecera;
                        objDbfSincro.RutaDbfDetalle = NBS_DBF_PathSettings.Default.dbfSincro_Detalle;
                        objDbfSincro.ActualizarDatosVenta(ref objOperationResult2, _ventaDto.v_IdVenta, DbfSincronizador.TipoAccion.Venta);
                        if (objOperationResult2.Success == 0)
                        {
                            MessageBox.Show(objOperationResult2.ErrorMessage, @"Error al sincronizar DBF", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }

                if (objOperationResult.Success == 1)
                {
                    using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                    {
                        strModo = "Guardado";
                        EdicionBarraNavegacion(true);
                        strIdVenta = _ventaDto.v_IdVenta;
                        BtnImprimir.Enabled = _btnImprimir;
                        CargarCabecera(_IdVentaGuardada);
                        _pstrIdMovimiento_Nuevo = _ventaDto.v_IdVenta;
                        _idVenta = _pstrIdMovimiento_Nuevo;
                        ListaGrilla = new BindingList<nbs_ventakardexDto>();
                        EditarTodo = true;

                    }
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                _TempDetalle_AgregarDto = new List<ventadetalleDto>();
                _TempDetalle_ModificarDto = new List<ventadetalleDto>();
                _TempDetalle_EliminarDto = new List<ventadetalleDto>();
                _TempDetalle_ModificarKardexDto = new List<nbs_ventakardexDto>();
                _TempDetalle_AgregarKardexDto = new List<nbs_ventakardexDto>();
                _TempDetalle_EliminarKardexDto = new List<nbs_ventakardexDto>();
            }
            else
            {
                UltraMessageBox.Show("Por favor llene los campos requeridos antes de guardar", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        private bool ValidaCamposNulosVaciosKardex()
        {
            if (ListaGrilla.Where(p => p.v_TipoKardex == null || p.v_TipoKardex.Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente tipo de Kardex", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (ListaGrilla.Where(p => p.v_NroKardex == null || p.v_NroKardex.Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente Número de Kardex", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (ListaGrilla.Where(p => p.d_Monto == null || p.d_Monto == 0).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente Monto del Kardex", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }


        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string TipoCambio = _objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
            txtTipoCambio.Text = TipoCambio;
            dtpFechaVencimiento.MinDate = dtpFechaRegistro.Value;

            txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
            txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Venta, _ventaDto.t_FechaRegistro, dtpFechaRegistro.Value, _ventaDto.v_Correlativo, 0);

            if (_objCierreMensualBL.VerificarMesCerrado(txtPeriodo.Text, txtMes.Text, (int)ModulosSistema.VentasFacturacion))
            {
                btnGuardar.Visible = false;
                this.Text = "Registro de Venta [MES CERRADO]";
            }
            else
            {

                btnGuardar.Visible = true;
                this.Text = "Registro de Venta";
            }
            dtpFechaOrden.Value = dtpFechaRegistro.Value;
            dtpFechaRef.Value = dtpFechaRegistro.Value;
            dtpFechaVencimiento.Value = dtpFechaRegistro.Value;
        }

        private void btnBuscarDetraccion_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtRucCliente.Text.Trim());
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {
                if (frm._IdCliente == "N002-CL000000000" && cboDocumento.Value != null && (int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.Boleta || int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.Irpe))
                {
                    _ventaDto.v_IdCliente = frm._IdCliente;
                    _ventaDto.i_IdDireccionCliente = frm._IdDireccionCliente;
                    txtCodigoCliente.Text = frm._CodigoCliente;
                    txtRazonSocial.Text = frm._RazonSocial;
                    txtRucCliente.Text = frm._NroDocumento;
                    txtDireccion.Text = frm._Direccion;
                    txtRazonSocial.Enabled = false;
                    txtRucCliente.Enabled = true;
                    txtDireccion.Enabled = false;
                    VerificarLineaCreditoCliente(_ventaDto.v_IdCliente);
                    IdIdentificacion = frm._TipoDocumento;
                }

                else if (frm._IdCliente != "N002-CL000000000" && cboDocumento.Value != null && (int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.Factura || int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.Irpe))
                {
                    _ventaDto.v_IdCliente = frm._IdCliente;
                    _ventaDto.i_IdDireccionCliente = frm._IdDireccionCliente;
                    txtCodigoCliente.Text = frm._CodigoCliente;
                    txtRazonSocial.Text = frm._RazonSocial;
                    txtRucCliente.Text = frm._NroDocumento;
                    txtDireccion.Text = frm._Direccion;
                    txtRazonSocial.Enabled = false;
                    txtRucCliente.Enabled = true;
                    txtDireccion.Enabled = false;
                    VerificarLineaCreditoCliente(_ventaDto.v_IdCliente);
                    IdIdentificacion = frm._TipoDocumento;
                }
                else
                {
                    UltraMessageBox.Show("Para este Documento no se puede elegir este cliente.", "Error de validación.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

            }
        }

        private void chkAfectoIGV_CheckedChanged(object sender, EventArgs e)
        {
            chkPrecInIGV.Enabled = chkAfectoIGV.Checked;
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

            FilasAnticipioActivacion(chkDeduccionAnticipio.Checked);

        }

        private void frmRegistroVentaNBS_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (strModo == "Nuevo" | strModo == "Edicion")
            //{
            //    if (UltraMessageBox.Show("¿Seguro de Salir del Formulario?", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            //    {
            //        e.Cancel = false;
            //    }
            //    else
            //    {
            //        e.Cancel = true;
            //    }
            //}
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
            //dtpFechaRegistro.MinDate = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/01").ToString());
            //dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/01").ToString()); ;
            //dtpFechaRegistro.MaxDate = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim()))).ToString()).ToString());
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
            if (_ListadoVentas.Count > 0)
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
            OperationResult objOperationResult = new OperationResult();
            LimpiarCabecera();
            CargarDetalle("");
            txtCorrelativo.Text = (int.Parse(_ListadoVentas[_MaxV].Value1) + 1).ToString("00000000");
            _Mode = "New";
            _ventaDto = new ventaDto();
            EdicionBarraNavegacion(false);
            txtTipoCambio.Text = _objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
        }

        private void cboTipoOperacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var c = grdData.DisplayLayout.Bands[0].Columns["i_IdTipoOperacion"];
                if (cboTipoOperacion.Value.ToString() == "5")
                {
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.EditAndSelectText;

                    foreach (var Fila in grdData.Rows)
                        Fila.Cells["i_RegistroEstado"].Value = "Modificado";
                }
                else
                {
                    if (cboTipoOperacion.Value.ToString().StartsWith("1"))
                    {
                        chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1 ? true : false;
                        chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvVentas == 1 ? true : false;
                    }
                    else if (cboTipoOperacion.Value.ToString() == "4")
                    {
                        cboTipoVenta.Value = "05";
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

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _ventadetalleDto = new ventadetalleDto();
                    _ventadetalleDto.v_IdVentaDetalle = grdData.ActiveRow.Cells["v_IdVentaDetalle"].Value.ToString();
                    _ventadetalleDto.v_IdFormatoUnicoFacturacionDetalle = grdData.ActiveRow.Cells["v_IdFormatoUnicoFacturacionDetalle"].Value == null ? null : grdData.ActiveRow.Cells["v_IdFormatoUnicoFacturacionDetalle"].Value.ToString();
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

        private void cboDocumento_ValueChanged(object sender, EventArgs e)
        {
            if (cboDocumento.Value != null)
            {
                if (cboDocumento.Value.ToString() == "3" | cboDocumento.Value.ToString() == "12")
                {
                    if (_Mode == "New")
                    {
                        txtCorrelativoDocFin.Enabled = true;
                    }

                }
                else
                {
                    txtCorrelativoDocFin.Enabled = false;
                    txtCorrelativoDocFin.Clear();
                }

            }
        }

        private void txtNroDias_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNroDias.Text))
            {
                dtpFechaVencimiento.Value = dtpFechaRegistro.Value.AddDays(int.Parse(txtNroDias.Text.Trim()));
            }
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
                        txtCodigoCliente.Clear();
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
                            txtCodigoCliente.Clear();
                            txtRazonSocial.Clear();
                            txtRucCliente.Focus();
                            txtDireccion.Clear();
                        }
                    }
                }
            }
        }

        private void txtCorrelativoDocFin_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCorrelativoDocFin.Text.Trim()))
            {
                int DocIni, DocFin;
                DocIni = int.Parse(txtCorrelativoDocIni.Text.Trim());
                DocFin = int.Parse(txtCorrelativoDocFin.Text.Trim());
                if (DocFin < DocIni)
                {
                    UltraMessageBox.Show("Número de correlativo inválido", "Error de Validación.", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    txtCorrelativoDocFin.Focus();
                }
            }

        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            List<string> IdVentas = grdData.Rows.Select(p => p.Cells["v_IdVenta"].Value.ToString()).Distinct().ToList();

            bool ImpresionVistaPrevia = _objDocumentoBL.ImpresionVistaPrevia(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());
            bool SeImprimio = false;
            if (int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.Factura)
            {
                using (Reportes.Ventas.frmDocumentoFactura frm = new Reportes.Ventas.frmDocumentoFactura(IdVentas, ImpresionVistaPrevia))
                {
                    if (ImpresionVistaPrevia)
                    {
                        frm.ShowDialog();
                    }
                    else
                    {
                        BtnImprimir.Enabled = false;
                    }
                }
            }
            if (int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.Boleta)
            {
                using (Reportes.Ventas.frmDocumentoBoleta frm = new Reportes.Ventas.frmDocumentoBoleta(IdVentas, ImpresionVistaPrevia))
                {
                    if (ImpresionVistaPrevia)
                    {
                        frm.ShowDialog();
                    }
                    else
                    {
                        BtnImprimir.Enabled = false;
                    }
                }
            }
            if (int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.Irpe)
            {
                //using (Reportes.Ventas.frmDocumentoIrpe frm = new Reportes.Ventas.frmDocumentoIrpe(IdVentas, ImpresionVistaPrevia))
                //{
                //    if (ImpresionVistaPrevia)
                //    {
                //        frm.ShowDialog();
                //    }
                //    else
                //    {
                //        BtnImprimir.Enabled = false;
                //    }
                //}
                new Reportes.Ventas.Ablimatex.TicketIrpe(IdVentas).Print();
                BtnImprimir.Enabled = false;

            }
            if (int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.NotaCredito)
            {
                using (Reportes.Ventas.frmDocumentoNotaCredito frm = new Reportes.Ventas.frmDocumentoNotaCredito(IdVentas, ImpresionVistaPrevia))
                {
                    if (ImpresionVistaPrevia)
                    {
                        frm.ShowDialog();
                    }
                    else
                    {
                        BtnImprimir.Enabled = false;
                    }
                }
            }

            if (int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.NotaDebito)
            {
                using (Reportes.Ventas.frmDocumentoNotaDebito frm = new Reportes.Ventas.frmDocumentoNotaDebito(IdVentas, ImpresionVistaPrevia))
                {
                    if (ImpresionVistaPrevia)
                    {
                        frm.ShowDialog();
                    }
                    else
                    {
                        BtnImprimir.Enabled = false;
                    }
                }

            }
            if (int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.TicketBoleta)
            {
                Reportes.Ventas.Ablimatex.frmDocumentoTicketSinRuc frm = new Reportes.Ventas.Ablimatex.frmDocumentoTicketSinRuc(IdVentas, IdIdentificacion, ImpresionVistaPrevia);
                if (ImpresionVistaPrevia)
                {
                    frm.ShowDialog();
                }
                else
                {
                    BtnImprimir.Enabled = false;
                }
            }


        }

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
        }

        #endregion

        #region Comportamiento de Controles
        private void txtRucCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtRucCliente, e);
        }

        private void txtGuiaRemisionSerie_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtGuiaRemisionSerie, e);
        }

        private void txtGuiaRemisionCorrelativo_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroEntero(txtGuiaRemisionCorrelativo, e);
        }

        private void txtSerieDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc, e);
        }

        private void txtCorrelativoDocIni_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDocIni, e);
        }

        private void txtCorrelativoDocFin_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDocFin, e);
        }

        private void txtCorrelativoDocIni_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDocIni, "{0:00000000}");
            ComprobarExistenciaCorrelativoDocumento();
            if (cboDocumento.Value != null)
            {
                if (cboDocumento.Value.ToString() == "3" | cboDocumento.Value.ToString() == "12")
                {
                    txtCorrelativoDocFin.Text = txtCorrelativoDocIni.Text;
                }
            }
        }

        private void txtCorrelativoDocFin_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDocFin, "{0:00000000}");
        }

        private void txtTipoCambio_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtTipoCambio, e);
        }

        private void txtSerieDocRef_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDocRef, e);
        }

        private void txtCorrelativoDocRef_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDocRef, e);
        }

        private void txtSerieDocRef_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerieDocRef, "{0:0000}");

        }

        private void txtCorrelativoDocRef_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDocRef, "{0:00000000}");
            if (grdData.Rows.Count() == 0)
            {
                DevuelveVentaReferencia(true);
            }
            else
            {
                if (UltraMessageBox.Show("Ya se ha cargado un documento previamente, ¿Desea cargar este otro documento?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.No)
                {
                    DevuelveVentaReferencia(false);
                }
                else
                {
                    LimpiarDetalle();
                    DevuelveVentaReferencia(true);
                }
            }
        }

        private void txtSerieDoc_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSerieDoc.Text) && cboDocumento.Value != null && cboDocumento.Value.ToString() != "-1")
            {
                Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc, "{0:0000}");
                ComprobarExistenciaCorrelativoDocumento();
                string Correlativo = _objDocumentoBL.CorrelativoxSerie(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text);
                if (Correlativo != null)
                {
                    txtCorrelativoDocIni.Text = Correlativo;

                }
                else
                {
                    UltraMessageBox.Show("No se encuentra registrado la serie en la configuración del Establecimiento", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    txtSerieDoc.Focus();
                    return;
                }
            }
        }

        private void txtNroDias_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDocRef, e);
        }

        private void txtDescuento_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtPorcDescuento, e);
        }

        private void txtComision_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtPorcComision, e);
        }

        private void cboDocumento_AfterDropDown(object sender, EventArgs e)
        {
            if (cboDocumento.Value == null) return;
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento.Rows)
            {
                if (cboDocumento.Value != null && cboDocumento.Value.ToString() == "-1") cboDocumento.Text = string.Empty;
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
                        var x = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento.Value.ToString() | p.Id == cboDocumento.Text);
                        if (x == null)
                        {
                            cboDocumento.Value = "-1";
                        }
                    }

                    if (cboDocumento.Value.ToString() == "3" || cboDocumento.Value.ToString() == "12" || cboDocumento.Value.ToString() == "438")
                    {
                        ClientePublicoGeneral();


                        if (cboDocumento.Value.ToString() != "438")
                        {
                            label34.Visible = true;
                            txtCorrelativoDocFin.Visible = true;
                            txtCorrelativoDocFin.Enabled = true;
                            txtCorrelativoDocFin.Text = txtCorrelativoDocIni.Text;
                        }

                    }
                    else if (_ventaDto.v_IdCliente == "N002-CL000000000")
                    {
                        _ventaDto.v_IdCliente = string.Empty;
                        _ventaDto.i_IdDireccionCliente = -1;
                        txtRucCliente.Clear();
                        txtRazonSocial.Clear();
                        txtCodigoCliente.Clear();
                        txtDireccion.Clear();
                        label34.Visible = false;
                        txtCorrelativoDocFin.Visible = false;
                        txtCorrelativoDocFin.Enabled = false;
                        txtCorrelativoDocFin.Text = string.Empty;
                    }

                    txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).Trim();
                    txtCorrelativoDocIni.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000").Trim();
                    PredeterminarEstablecimiento(txtSerieDoc.Text);


                    if (cboDocumento.Value != null && cboDocumento.Value.ToString() != "-1" && int.Parse(cboDocumento.Value.ToString()) != (int)DocumentType.Irpe && !string.IsNullOrEmpty(UserConfig.Default.SerieCaja) && UserConfig.Default.SerieCaja != "--Seleccionar--")
                    {
                        txtSerieDoc.Text = UserConfig.Default.SerieCaja;
                    }
                    CancelEventArgs e1 = new CancelEventArgs();
                    txtSerieDoc_Validating(sender, e1);
                    PredeterminarEstablecimiento(txtSerieDoc.Text.Trim());

                    if (!_objDocumentoBL.DocumentoEsContable(int.Parse(cboDocumento.Value.ToString()))) //Validación agregada por GUI
                    {
                        chkPrecInIGV.Checked = false;
                        chkAfectoIGV.Checked = false;
                        //chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1 ? true : false;
                        //chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvVentas == 1 ? true : false;
                    }
                    else
                    {
                        chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1;
                        chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvVentas == 1;
                    }
                }
                var y = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento.Value.ToString() || p.Id == cboDocumento.Text);
                if (y == null)
                {
                    cboDocumento.Value = "-1";
                }

                if (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) || cboDocumento.Value.ToString() == "8")
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
                    dtpFechaRef.Enabled = false;
                    chkDeduccionAnticipio.Enabled = true;
                }
                _EsNotadeCredito = _objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) ? true : false;
                if (_EsNotadeCredito == true)
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

        private void cboDocumentoRef_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumentoRef.Rows)
            {
                if (cboDocumentoRef.Value == "-1") cboDocumentoRef.Text = string.Empty;
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

        private void cboDocumentoRef_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumentoRef.Rows)
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
                    var x = _ListadoComboDocumentosRef.Find(p => p.Id == cboDocumentoRef.Value.ToString() | p.Id == cboDocumentoRef.Text);
                    if (x == null)
                    {
                        cboDocumentoRef.Value = "-1";
                    }

                    txtSerieDocRef.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumentoRef.Value.ToString())).Trim();

                    if (cboDocumento.Value != null && !string.IsNullOrEmpty(UserConfig.Default.SerieCaja) && UserConfig.Default.SerieCaja != "--Seleccionar--")
                    {
                        txtSerieDocRef.Text = UserConfig.Default.SerieCaja;
                    }
                }

                if (cboDocumentoRef.Value.ToString() == "50" || cboDocumentoRef.Value.ToString() == "52")
                {
                    label54.Visible = true;
                    txtValorFOB.Visible = true;
                }
                else
                {
                    label54.Visible = false;
                    txtValorFOB.Visible = false;
                }

            }
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanged(object sender, EventArgs e)
        {
            if (ultraExpandableGroupBox1.Expanded == true)
            {
                gpDetalle.Location = new Point(gpDetalle.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height);
                gpDetalle.Height = this.Height - gpDetalle.Location.Y - 80 - ultraStatusBar1.Height;
            }
            else
            {
                gpDetalle.Location = new Point(gpDetalle.Location.X, ultraExpandableGroupBox1.Location.Y + ultraExpandableGroupBox1.Height);
                gpDetalle.Height = this.Height - gpDetalle.Location.Y - 80 - ultraStatusBar1.Height;
            }

        }

        private void txtNroDias_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNroDias.Text))
            {
                if (int.Parse(txtNroDias.Text) > 365)
                {
                    UltraMessageBox.Show("El número de días no puede exceder de 365(1 año)", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //if (txtNroDias.CanUndo)
                    //{
                    //    txtNroDias.Undo();
                    //    txtNroDias.ClearUndo();
                    //    txtNroDias.Focus();
                    //}
                }
            }
        }

        private void txtPorcDescuento_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPorcDescuento.Text))
            {
                if (decimal.Parse(txtPorcDescuento.Text) > 100)
                {
                    UltraMessageBox.Show("Cantidad Inválida", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                }
                else
                {
                    CalcularTotales();
                }
            }
        }

        private void txtPorcComision_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPorcComision.Text))
            {
                if (decimal.Parse(txtPorcComision.Text) > 100)
                {
                    UltraMessageBox.Show("Cantidad Inválida", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                }
            }
        }

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            PredeterminarEstablecimiento(txtSerieDoc.Text);
        }


        private void txtSerieDoc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtSerieDoc.CanUndo)
                //{
                //    txtSerieDoc.Undo();
                //}
            }
        }

        private void txtCorrelativoDocIni_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtCorrelativoDocIni.CanUndo)
                //{
                //    txtCorrelativoDocIni.Undo();
                //}
            }
        }

        private void txtCorrelativoDocFin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtCorrelativoDocFin.CanUndo)
                //{
                //    txtCorrelativoDocFin.Undo();
                //}
            }
        }

        private void txtSerieDocRef_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtSerieDocRef.CanUndo)
                //{
                //    txtSerieDocRef.Undo();
                //}
            }
        }

        private void txtCorrelativoDocRef_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtCorrelativoDocRef.CanUndo)
                //{
                //    txtCorrelativoDocRef.Undo();
                //}
            }
        }

        private void txtTipoCambio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtTipoCambio.CanUndo)
                //{
                //    txtTipoCambio.Undo();
                //}
            }
        }

        private void txtNroDias_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtNroDias.CanUndo)
                //{
                //    txtNroDias.Undo();
                //}
            }
        }

        private void txtGuiaRemisionCorrelativo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtGuiaRemisionCorrelativo.CanUndo)
                //{
                //    txtGuiaRemisionCorrelativo.Undo();
                //}
            }
        }

        private void txtGuiaRemisionSerie_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtGuiaRemisionSerie.CanUndo)
                //{
                //    txtGuiaRemisionSerie.Undo();
                //}
            }
        }

        private void txtOrdenCompra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtOrdenCompra.CanUndo)
                //{
                //    txtOrdenCompra.Undo();
                //}
            }
        }

        private void txtNroBulto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtNroBulto.CanUndo)
                //{
                //    txtNroBulto.Undo();
                //}
            }
        }

        private void txtDimensiones_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtDimensiones.CanUndo)
                //{
                //    txtDimensiones.Undo();
                //}
            }
        }

        private void txtPesoBrutoKg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtPesoBrutoKg.CanUndo)
                //{
                //    txtPesoBrutoKg.Undo();
                //}
            }
        }

        private void txtPesoNetoKg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtPesoNetoKg.CanUndo)
                //{
                //    txtPesoNetoKg.Undo();
                //}
            }
        }

        private void txtMarca_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                //if (txtMarca.CanUndo)
                //{
                //    txtMarca.Undo();
                //}
            }
        }
        #endregion

        #region Clases/Validaciones
        private void ObtenerListadoVentas(string pstrPeriodo, string pstrMes)
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoVentas = _objVentasBL.ObtenerListadoVentas(ref objOperationResult, pstrPeriodo, pstrMes);
            switch (strModo)
            {
                case "Edicion":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdVenta);
                    _idVenta = strIdVenta;
                    break;

                case "Nuevo":
                    if (_ListadoVentas.Count != 0)
                    {
                        IdIdentificacion = 0;
                        _MaxV = _ListadoVentas.Count() - 1;
                        _ActV = _MaxV;
                        LimpiarCabecera();
                        CargarDetalle("");
                        _Mode = "New";
                        _ventaDto = new ventaDto();
                        _movimientoDto = new movimientoDto();
                        EdicionBarraNavegacion(true);
                        cboDocumento.Enabled = true;
                    }
                    else
                    {
                        IdIdentificacion = 0;
                        _Mode = "New";
                        LimpiarCabecera();
                        _MaxV = 1;
                        _ActV = 1;
                        _ventaDto = new ventaDto();
                        _movimientoDto = new movimientoDto();
                        btnNuevoMovimiento.Enabled = false;
                        EdicionBarraNavegacion(true);
                    }
                    txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Venta, _ventaDto.t_FechaRegistro, dtpFechaRegistro.Value, _ventaDto.v_Correlativo, 0);
                    txtTipoCambio.Text = _objVentasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
                    cboCondicionPago.Value = "1";
                    cboEstado.Value = "1";
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
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdVenta);
                    _idVenta = strIdVenta;
                    cboDocumento.Enabled = false;
                    txtSerieDoc.Enabled = false;
                    RestringirEdicion();
                    break;
            }
        }

        private void EdicionBarraNavegacion(bool ON_OFF)
        {
            cboDocumento.Enabled = ON_OFF;
            txtSerieDoc.Enabled = ON_OFF;
            cboDocumentoRef.Enabled = ON_OFF;
            btnBuscarDetraccion.Enabled = ON_OFF;
            txtSerieDocRef.Enabled = ON_OFF;
            txtCorrelativoDocFin.Enabled = ON_OFF;
            txtRucCliente.Enabled = ON_OFF;
        }

        private void CargarCabecera(string idmovimiento)
        {
            OperationResult objOperationResult = new OperationResult();
            _ventaDto = new ventaDto();
            _ventaDto = _objVentasBL.ObtenerVentaCabecera(ref objOperationResult, idmovimiento);
            if (_ventaDto != null)
            {

                IdIdentificacion = int.Parse(_ventaDto.i_IdTipoIdentificacion.ToString());
                cboMoneda.Value = _ventaDto.i_IdMoneda.ToString();
                txtPeriodo.Text = _ventaDto.v_Periodo;
                txtAnticipio.Text = _ventaDto.d_Anticipio.ToString();
                txtCodigoCliente.Text = _ventaDto.CodigoCliente;
                txtPorcComision.Text = _ventaDto.d_PocComision.ToString();
                txtCorrelativo.Text = _ventaDto.v_Correlativo;
                txtCorrelativoDocFin.Text = _ventaDto.v_CorrelativoDocumentoFin;
                txtCorrelativoDocIni.Text = _ventaDto.v_CorrelativoDocumento;
                txtCorrelativoDocRef.Text = _ventaDto.v_CorrelativoDocumentoRef;
                txtPorcDescuento.Text = _ventaDto.d_PorcDescuento.ToString();
                txtDescuento.Text = _ventaDto.d_Descuento.ToString();
                txtDimensiones.Text = _ventaDto.v_BultoDimensiones;
                txtGuiaRemisionCorrelativo.Text = _ventaDto.v_NroGuiaRemisionCorrelativo;
                txtGuiaRemisionSerie.Text = _ventaDto.v_NroGuiaRemisionSerie;
                txtIGV.Text = _ventaDto.d_IGV.ToString();
                txtMarca.Text = _ventaDto.v_Marca;
                txtMes.Text = int.Parse(_ventaDto.v_Mes).ToString("00");
                txtConcepto.Text = _ventaDto.v_Concepto;
                txtNroBulto.Text = _ventaDto.v_NroBulto;
                txtNroDias.Text = _ventaDto.i_NroDias.ToString();
                txtNroPedido.Text = _ventaDto.v_NroPedido;
                txtOrdenCompra.Text = _ventaDto.v_OrdenCompra;
                txtPesoBrutoKg.Text = _ventaDto.d_PesoBrutoKG.ToString();
                txtPesoNetoKg.Text = _ventaDto.d_PesoNetoKG.ToString();
                txtRazonSocial.Text = _ventaDto.NombreCliente;
                txtRucCliente.Text = _ventaDto.NroDocCliente;
                txtSerieDoc.Text = _ventaDto.v_SerieDocumento;
                txtSerieDocRef.Text = _ventaDto.v_SerieDocumentoRef;
                txtTotal.Text = _ventaDto.d_Total.ToString();
                txtValorVenta.Text = _ventaDto.d_ValorVenta.ToString();
                txtDireccion.Text = _ventaDto.Direccion;
                dtpFechaOrden.Value = _ventaDto.t_FechaOrdenCompra.Value;
                dtpFechaRef.Value = _ventaDto.t_FechaRef.Value;
                dtpFechaRegistro.Value = _ventaDto.t_FechaRegistro.Value;
                dtpFechaVencimiento.Value = _ventaDto.t_FechaVencimiento.Value;
                cboCondicionPago.Value = _ventaDto.i_IdCondicionPago.ToString();
                cboDocumento.Value = _ventaDto.i_IdTipoDocumento.ToString();
                cboDocumentoRef.Value = _ventaDto.i_IdTipoDocumentoRef.ToString();
                cboEstablecimiento.Value = _ventaDto.i_IdEstablecimiento.ToString();
                cboEstado.Value = _ventaDto.i_IdEstado.ToString();
                cboIGV.Value = _ventaDto.i_IdIgv.ToString();
                cboMoneda.Value = _ventaDto.i_IdMoneda.ToString();
                cboMVenta.Value = _ventaDto.i_IdMedioPagoVenta.ToString();
                cboPuntoDestino.Value = _ventaDto.i_IdPuntoDestino.ToString();
                cboPuntoEmbarque.Value = _ventaDto.i_IdPuntoEmbarque.ToString();
                cboTipoEmbarque.Value = _ventaDto.i_IdTipoEmbarque.ToString();
                chkAfectoIGV.Checked = _ventaDto.i_EsAfectoIgv == 1 ? true : false;

                cboTipoVenta.Value = int.Parse(_ventaDto.i_IdTipoVenta.ToString()).ToString("00");
                cboVendedor.Value = _ventaDto.v_IdVendedor != null ? _ventaDto.v_IdVendedor.ToString() : "-1";
                cboVendedorRef.Value = _ventaDto.v_IdVendedorRef.ToString();

                chkDeduccionAnticipio.Checked = _ventaDto.i_DeduccionAnticipio == 1 ? true : false;
                chkDrawnBack.Checked = _ventaDto.i_DrawBack == 1 ? true : false;
                chkPrecInIGV.Checked = _ventaDto.i_PreciosIncluyenIgv == 1 ? true : false;
                txtTipoCambio.Text = _ventaDto.d_TipoCambio.ToString();
                txtRucCliente.Enabled = _ventaDto.v_IdCliente == "N002-CL000000000" ? false : txtRucCliente.Enabled;
                txtRazonSocial.Enabled = _ventaDto.v_IdCliente != "N002-CL000000000" ? false : true;
                txtDireccion.Enabled = _ventaDto.v_IdCliente != "N002-CL000000000" ? false : true;
                txtPlacaVehiculo.Text = _ventaDto.v_PlacaVehiculo;
                txtNroFuf.Text = _ventaDto.v_NroFUF;
                cboTipoOperacion.Value = _ventaDto.i_IdTipoOperacion.ToString();
                //txtTipoKardex.Text = _ventaDto.v_IdTipoKardex.Trim();
                //txtTipoKardex.Text = _ventaDto.v_IdTipoKardex;
                CargarFormularioKardex = false;
                cboTipoKardex.Value = string.IsNullOrEmpty(_ventaDto.v_IdTipoKardex) ? "-1" : _ventaDto.v_IdTipoKardex.ToString();
                CargarFormularioKardex = true;
                //if (txtTipoKardex.Text.Trim() != "V" && txtTipoKardex.Text.Trim() != string.Empty)
                if (cboTipoKardex.Value == null)
                {
                    txtNroKardex.Visible = false;
                    txtNroKardex.Text = string.Empty;
                    lblNumeroKardex.Visible = false;

                    lblMonto.Visible = false;
                    txtMonto.Visible = false;
                    btnActualizarKardex.Visible = false;

                }
                else
                {
                    btnActualizarKardex.Visible = true;
                    if (cboTipoKardex.Value.ToString() != "V" && cboTipoKardex.Value.ToString() != string.Empty)
                    {
                        var NroKardex = _objVentasBL.ObtenerDetalleKardex(ref   objOperationResult, _ventaDto.v_IdVenta).FirstOrDefault();
                        txtNroKardex.Text = NroKardex != null ? NroKardex.v_NroKardex.Trim() : "";

                        txtMonto.Text = NroKardex != null ? NroKardex.d_Monto.Value.ToString() : "0";

                        txtNroKardex.Visible = true;
                        lblNumeroKardex.Visible = true;
                        lblMonto.Visible = true;
                        txtMonto.Visible = true;
                        txtMonto.Enabled = false;
                    }
                    else
                    {
                       var ListaKardex= _objVentasBL.ObtenerDetalleKardex(ref objOperationResult, _ventaDto.v_IdVenta);
                        txtNroKardex.Visible = false;
                        txtNroKardex.Text = string.Empty;
                        lblNumeroKardex.Visible = false;

                        lblMonto.Visible = true;
                        txtMonto.Visible = true;
                        txtMonto.Enabled = false;
                        txtMonto.Text = ListaKardex != null && ListaKardex.Any() ? ListaKardex.Sum(o => o.d_Monto.Value).ToString () : "0.00";
                    }
                }

                if (txtNroFuf.Text != string.Empty)
                {
                    btnBuscarDetraccion.Enabled = false;
                }


                _Mode = "Edit";
                BtnImprimir.Enabled = _btnImprimir == true ? true : false;
                CargarDetalle(_ventaDto.v_IdVenta);
                if (_ventaDto.v_NroPedido != string.Empty) RestringirEdicion(); //SI ES UNA VENTA ORIGINADA DE UN PEDIDO NO SE PUEDE EDITAR.
                btnGuardarSinProcesar.Enabled = string.IsNullOrEmpty(_ventaDto.v_CorrelativoDocumento) && string.IsNullOrEmpty(_ventaDto.v_SerieDocumento);
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
            var detail = _objVentasBL.ObtenerVentaDetalles(ref objOperationResult, pstringIdVenta);
            grdData.DataSource = detail;
            if (objOperationResult.Success == 0) return;
            var blIsc = new ProductoIscBL();
            _productoiscDtos.Clear();
            foreach (var dto in detail)
                if (dto.d_isc != null && dto.d_isc.Value != 0)
                {
                    var item = blIsc.FromProductDetail(ref objOperationResult, dto.v_IdProductoDetalle, Globals.ClientSession.i_Periodo.ToString());
                    if (item != null)
                        _productoiscDtos.Add(dto.v_IdProductoDetalle, item);
                }
            if (grdData.Rows.Count > 0)
            {
                for (int i = 0; i < grdData.Rows.Count(); i++)
                {
                    grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
                    FilaAnticipio(grdData.Rows[i]);
                }
                CalcularTotales();
            }
        }

        private void LimpiarCabecera()
        {
            txtAnticipio.Clear();
            txtCodigoCliente.Clear();

            if (string.IsNullOrEmpty(txtCorrelativoDocRef.Text))
            {
                txtCorrelativoDocFin.Clear();
                txtCorrelativoDocIni.Clear();
                txtCorrelativoDocRef.Clear();
                txtSerieDoc.Clear();
                txtSerieDocRef.Clear();
                cboDocumento.Value = "-1";
                cboDocumentoRef.Value = "-1";
            }

            txtPorcDescuento.Clear();
            txtDescuento.Clear();
            txtDimensiones.Clear();
            txtGuiaRemisionCorrelativo.Clear();
            txtGuiaRemisionSerie.Clear();
            txtIGV.Clear();
            txtMarca.Clear();
            txtConcepto.Text = @"VENTA DE MERCADERÍA";
            txtNroBulto.Clear();
            txtNroDias.Text = @"0";
            txtNroPedido.Clear();
            txtOrdenCompra.Clear();
            txtPesoBrutoKg.Clear();
            txtPesoNetoKg.Clear();
            txtRazonSocial.Clear();
            txtRucCliente.Clear();
            txtTotal.Clear();
            txtValorVenta.Clear();
            dtpFechaOrden.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim())))));
            dtpFechaRef.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim())))));
            dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Day));
            dtpFechaVencimiento.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Day));
            cboCondicionPago.Value = "-1";
            cboEstablecimiento.Value = "-1";
            cboEstado.Value = "-1";
            cboMVenta.Value = "-1";
            cboPuntoDestino.Value = "-1";
            cboPuntoEmbarque.Value = "-1";
            cboTipoEmbarque.Value = "-1";
            cboTipoVenta.Value = "03"; // predetermina como venta con moneda nacional
            cboVendedor.Value = Globals.ClientSession.v_IdVendedor;
            cboVendedorRef.Value = "-1";
            chkDeduccionAnticipio.Checked = false;
            chkDrawnBack.Checked = false;
            cboTipoOperacion.Value = Globals.ClientSession.i_IdTipoOperacionVentas.ToString();
            cboIGV.Value = Globals.ClientSession.i_IdIgv.ToString();
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1 ? true : false;
            chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvVentas == 1 ? true : false;
            txtPorcComision.Text = Globals.ClientSession.d_ComisionVendedor.ToString();
            txtCorrelativoDocIni.Enabled = true;

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
            ultraGridColumnaID.Hidden = false;
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
            UltraGridColumn __ultraGridColumnaDescripcion_ = new UltraGridColumn("Value1");
            __ultraGridColumnaDescripcion_.Header.Caption = "Descripción";
            __ultraGridColumnaDescripcion_.Header.VisiblePosition = 0;
            __ultraGridColumnaDescripcion_.Width = 270;
            __ultraGridBanda_.Columns.AddRange(new object[] { __ultraGridColumnaDescripcion_ });
            ucTipoOperacion.DisplayLayout.BandsSerializer.Add(__ultraGridBanda_);
            ucTipoOperacion.DropDownWidth = 270;
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
            Utils.Windows.LoadUltraComboList(ucTipoOperacion, "Value1", "Id", Globals.CacheCombosVentaDto.cboTipoOperacion.Where(r => r.Id != "5").ToList(), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucRubro, "Value1", "Id", _objVentasBL.ObtenRubrosParaComboGridVenta(ref objOperationResult, null), DropDownListAction.Select);
        }
        private void LlenarTemporalesKardex()
        {
            OperationResult objOperationResult = new OperationResult();
            nbs_ventakardexDto objKardex = new nbs_ventakardexDto();
            try
            {
                bool Eliminado = false;
                if (cboTipoKardex.Value == null) return;
                if (_Mode == "New")
                {
                    //if (txtTipoKardex.Text == "V")
                    if (cboTipoKardex.Value.ToString() == "V")
                    {
                        if (!ListaGrilla.Any()) return;
                        foreach (var fila in ListaGrilla)
                        {
                            switch (fila.i_RegistroTipo.ToString())
                            {
                                case "Temporal":
                                    if (fila.i_RegistroEstado == "Modificado")
                                    {
                                        var _objKardex = fila;
                                        _TempDetalle_AgregarKardexDto.Add(_objKardex);
                                    }
                                    break;

                                case "NoTemporal":
                                    if (fila.i_RegistroEstado == "Modificado")
                                    {
                                        var _objKardex = fila;
                                        _TempDetalle_ModificarKardexDto.Add(_objKardex);
                                    }
                                    break;
                            }
                        }
                    }
                    else if (ListaTipoKardex.Contains(cboTipoKardex.Value.ToString()))
                    {
                        objKardex = new nbs_ventakardexDto();
                        //objKardex.v_TipoKardex = txtTipoKardex.Text.Trim();
                        objKardex.v_TipoKardex = cboTipoKardex.Value.ToString();
                        objKardex.v_NroKardex = txtNroKardex.Text.Trim();
                        objKardex.d_Monto = txtMonto.Text.Trim() == "" ? 0 : decimal.Parse(txtMonto.Text.Trim());
                        _TempDetalle_AgregarKardexDto.Add(objKardex);

                    }

                }
                else
                {
                    if (_ventaDto.v_IdTipoKardex != null && _ventaDto.v_IdTipoKardex != cboTipoKardex.Value.ToString())
                    {
                        //Elimino todos los ventakardex asociados a IdVenta
                        var KardexEliminar = _objVentasBL.ObtenerDetalleKardex(ref objOperationResult, _ventaDto.v_IdVenta).ToList();
                        foreach (var item in KardexEliminar)
                        {
                            if (!_TempDetalle_EliminarKardexDto.Select(x => x.v_IdVentaKardex).Contains(item.v_IdVentaKardex))
                            {
                                _TempDetalle_EliminarKardexDto.Add(item);
                                Eliminado = true;
                            }

                        }
                    }


                    //if (txtTipoKardex.Text == "V")
                    if (cboTipoKardex.Value.ToString() == "V")
                    {
                        if (!ListaGrilla.Any()) return;
                        foreach (var fila in ListaGrilla)
                        {
                            switch (fila.i_RegistroTipo.ToString())
                            {
                                case "Temporal":
                                    if (fila.i_RegistroEstado == "Modificado")
                                    {
                                        var _objKardex = fila;
                                        _TempDetalle_AgregarKardexDto.Add(_objKardex);
                                    }
                                    break;

                                case "NoTemporal":
                                    if (fila.i_RegistroEstado == "Modificado")
                                    {
                                        var _objKardex = fila;
                                        _TempDetalle_ModificarKardexDto.Add(_objKardex);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        var objNroKardex = _objVentasBL.ObtenerDetalleKardex(ref   objOperationResult, _ventaDto.v_IdVenta).FirstOrDefault();

                        if (objNroKardex != null && objNroKardex.v_TipoKardex != null && objNroKardex.v_TipoKardex == "H")
                        {

                            objKardex = new nbs_ventakardexDto();
                            objKardex = objNroKardex;
                            //objKardex.v_TipoKardex = txtTipoKardex.Text.Trim();
                            objKardex.v_TipoKardex = cboTipoKardex.Value.ToString();
                            objKardex.v_NroKardex = txtNroKardex.Text.Trim();
                            objKardex.d_Monto = txtMonto.Text.Trim() == "" ? 0 : decimal.Parse(txtMonto.Text.Trim());
                            if (Eliminado)
                            {
                                _TempDetalle_AgregarKardexDto.Add(objKardex);
                            }
                            else
                            {
                                _TempDetalle_ModificarKardexDto.Add(objKardex);
                            }

                        }
                        else if (objNroKardex != null && objNroKardex.v_NroKardex != null && objNroKardex.v_NroKardex.Trim() != txtNroKardex.Text.Trim())
                        {
                            objKardex = new nbs_ventakardexDto();
                            objKardex = objNroKardex;
                            //objKardex.v_TipoKardex = txtTipoKardex.Text.Trim();
                            objKardex.v_TipoKardex = cboTipoKardex.Value.ToString();
                            objKardex.v_NroKardex = txtNroKardex.Text.Trim();
                            objKardex.d_Monto = txtMonto.Text.Trim() == "" ? 0 : decimal.Parse(txtMonto.Text.Trim());

                            if (Eliminado)
                            {
                                _TempDetalle_AgregarKardexDto.Add(objKardex);
                            }
                            else
                            {
                                _TempDetalle_ModificarKardexDto.Add(objKardex);
                            }
                        }
                        else if (objNroKardex != null && objNroKardex.v_NroKardex != null && txtNroKardex.Text.Trim() == string.Empty && objNroKardex.v_NroKardex != string.Empty)
                        {

                            objKardex = new nbs_ventakardexDto();
                            objKardex = objNroKardex;
                            objKardex.v_IdVentaKardex = objNroKardex.v_IdVentaKardex;
                            _TempDetalle_EliminarKardexDto.Add(objKardex);

                        }
                        else if (objNroKardex == null)
                        {
                            objKardex = new nbs_ventakardexDto();
                            objKardex.v_TipoKardex = cboTipoKardex.Value.ToString();
                            objKardex.v_NroKardex = txtNroKardex.Text.Trim();
                            objKardex.d_Monto = txtMonto.Text.Trim() == "" ? 0 : decimal.Parse(txtMonto.Text.Trim());
                            _TempDetalle_AgregarKardexDto.Add(objKardex);

                        }

                    }
                }
            }

            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message + "Linea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }

        private void LlenarTemporalesKardex_()
        {
            OperationResult objOperationResult = new OperationResult();
            nbs_ventakardexDto objKardex = new nbs_ventakardexDto();
            try
            {
                //bool Eliminado = false;
                if (cboTipoKardex.Value == null) return;
                if (_Mode == "New")
                {
                    //if (txtTipoKardex.Text == "V")
                    if (cboTipoKardex.Value.ToString() == "V")
                    {
                        if (!ListaGrilla.Any()) return;
                        foreach (var fila in ListaGrilla)
                        {
                            switch (fila.i_RegistroTipo.ToString())
                            {
                                case "Temporal":
                                    if (fila.i_RegistroEstado == "Modificado")
                                    {
                                        var _objKardex = fila;
                                        _TempDetalle_AgregarKardexDto.Add(_objKardex);
                                    }
                                    break;

                                case "NoTemporal":
                                    if (fila.i_RegistroEstado == "Modificado")
                                    {
                                        var _objKardex = fila;
                                        _TempDetalle_ModificarKardexDto.Add(_objKardex);
                                    }
                                    break;
                            }
                        }
                    }
                    else if (ListaTipoKardex.Contains(cboTipoKardex.Value.ToString()))
                    {
                        objKardex = new nbs_ventakardexDto();
                        //objKardex.v_TipoKardex = txtTipoKardex.Text.Trim();
                        objKardex.v_TipoKardex = cboTipoKardex.Value.ToString();
                        objKardex.v_NroKardex = txtNroKardex.Text.Trim();
                        objKardex.d_Monto = txtMonto.Text.Trim() == "" ? decimal.Parse(txtTotal.Text) : decimal.Parse(txtMonto.Text.Trim());
                        _TempDetalle_AgregarKardexDto.Add(objKardex);

                    }

                }
                else
                {
                    if (cboTipoKardex.Value.ToString() == "V")
                    {
                        //if (_ventaDto.v_IdTipoKardex != null && _ventaDto.v_IdTipoKardex != cboTipoKardex.Value.ToString())
                        //{
                        //    //Elimino todos los ventakardex asociados a IdVenta
                        //    var KardexEliminar = _objVentasBL.ObtenerDetalleKardex(ref objOperationResult, _ventaDto.v_IdVenta).ToList();
                        //    foreach (var item in KardexEliminar)
                        //    {
                        //        if (!_TempDetalle_EliminarKardexDto.Select(x => x.v_IdVentaKardex).Contains(item.v_IdVentaKardex))
                        //        {
                        //            _TempDetalle_EliminarKardexDto.Add(item);
                        //            Eliminado = true;
                        //        }

                        //    }
                        //}
                        if (!ListaGrilla.Any()) return;
                        foreach (var fila in ListaGrilla)
                        {
                            switch (fila.i_RegistroTipo.ToString())
                            {
                                case "Temporal":
                                    if (fila.i_RegistroEstado == "Modificado")
                                    {
                                        var _objKardex = fila;
                                        _TempDetalle_AgregarKardexDto.Add(_objKardex);
                                    }
                                    break;

                                case "NoTemporal":
                                    if (fila.i_RegistroEstado == "Modificado")
                                    {
                                        var _objKardex = fila;
                                        _TempDetalle_ModificarKardexDto.Add(_objKardex);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        //Elimino todos los ventakardex asociados a IdVenta
                        if (_ventaDto.v_IdTipoKardex != null && _ventaDto.v_IdTipoKardex.Trim() == "V")
                        {
                            var KardexEliminar = _objVentasBL.ObtenerDetalleKardex(ref objOperationResult, _ventaDto.v_IdVenta).ToList();
                            foreach (var item in KardexEliminar)
                            {
                                if (!_TempDetalle_EliminarKardexDto.Select(x => x.v_IdVentaKardex).Contains(item.v_IdVentaKardex))
                                {
                                    _TempDetalle_EliminarKardexDto.Add(item);
                                }

                            }

                            objKardex = new nbs_ventakardexDto();
                            objKardex.v_TipoKardex = cboTipoKardex.Value.ToString();
                            objKardex.v_NroKardex = txtNroKardex.Text.Trim();
                            objKardex.d_Monto = txtMonto.Text.Trim() == "" ? decimal.Parse(txtTotal.Text) : decimal.Parse(txtMonto.Text.Trim());
                            _TempDetalle_AgregarKardexDto.Add(objKardex);
                        }
                        else
                        {

                            var objNroKardex = _objVentasBL.ObtenerDetalleKardex(ref   objOperationResult, _ventaDto.v_IdVenta).FirstOrDefault();
                            if (objNroKardex != null)
                            {

                                objKardex = new nbs_ventakardexDto();
                                objKardex = objNroKardex;
                                objKardex.v_TipoKardex = cboTipoKardex.Value.ToString();
                                objKardex.v_NroKardex = txtNroKardex.Text.Trim();
                                objKardex.d_Monto = txtMonto.Text.Trim() == "" ? decimal.Parse(txtTotal.Text) : decimal.Parse(txtMonto.Text.Trim());
                                _TempDetalle_ModificarKardexDto.Add(objKardex);

                            }
                            else
                            {
                                objKardex = new nbs_ventakardexDto();
                                objKardex.v_TipoKardex = cboTipoKardex.Value.ToString();
                                objKardex.v_NroKardex = txtNroKardex.Text.Trim();
                                objKardex.d_Monto = txtMonto.Text.Trim() == "" ? decimal.Parse(txtTotal.Text) : decimal.Parse(txtMonto.Text.Trim());
                                _TempDetalle_AgregarKardexDto.Add(objKardex);
                            }
                        }





                    }
                }
            }

            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message + "Linea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }


        }




        private void LlenarTemporalesVenta()
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
                                _ventadetalleDto = new ventadetalleDto();
                                _ventadetalleDto.v_IdVenta = _ventaDto.v_IdVenta;
                                _ventadetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _ventadetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _ventadetalleDto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _ventadetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                _ventadetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _ventadetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());

                                _ventadetalleDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Text.Replace("_", ""));
                                _ventadetalleDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Text);
                                _ventadetalleDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioVenta"].Text);
                                _ventadetalleDto.d_Valor = Fila.Cells["d_Valor"].Value == null || string.IsNullOrEmpty(Fila.Cells["d_Valor"].Text) ? 0 : decimal.Parse(Fila.Cells["d_Valor"].Text.Replace("_", ""));
                                _ventadetalleDto.d_Descuento = Fila.Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Descuento"].Text);
                                _ventadetalleDto.d_isc = Fila.Cells["d_isc"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_isc"].Value.ToString());
                                _ventadetalleDto.d_otrostributos = Fila.Cells["d_otrostributos"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString());
                                _ventadetalleDto.i_Anticipio = Fila.Cells["i_Anticipio"].Value == null ? 0 : int.Parse(Fila.Cells["i_Anticipio"].Value.ToString());
                                _ventadetalleDto.i_InsertaIdUsuario = Fila.Cells["i_InsertaIdUsuario"].Value == null ? 0 : int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _ventadetalleDto.v_NroCuenta = Fila.Cells["v_NroCuenta"].Value == null ? null : Fila.Cells["v_NroCuenta"].Value.ToString().Trim();
                                _ventadetalleDto.i_IdCentroCosto = Fila.Cells["i_IdCentroCosto"].Value == null ? null : Fila.Cells["i_IdCentroCosto"].Value.ToString();
                                _ventadetalleDto.i_IdTipoOperacion = Fila.Cells["i_IdTipoOperacion"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoOperacion"].Value.ToString());
                                _ventadetalleDto.i_NroUnidades = Fila.Cells["i_NroUnidades"].Value == null ? 0 : int.Parse(Fila.Cells["i_NroUnidades"].Value.ToString());
                                _ventadetalleDto.d_PorcentajeComision = Fila.Cells["d_PorcentajeComision"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PorcentajeComision"].Value.ToString());
                                _ventadetalleDto.i_NroUnidades = Fila.Cells["i_NroUnidades"].Value == null ? 0 : int.Parse(Fila.Cells["i_NroUnidades"].Value.ToString());
                                _ventadetalleDto.v_Observaciones = Fila.Cells["v_Observaciones"].Value == null ? null : Fila.Cells["v_Observaciones"].Value.ToString();
                                _ventadetalleDto.v_PedidoExportacion = Fila.Cells["v_PedidoExportacion"].Value == null ? null : Fila.Cells["v_PedidoExportacion"].Value.ToString();
                                _ventadetalleDto.v_FacturaRef = Fila.Cells["v_FacturaRef"].Value == null ? null : Fila.Cells["v_FacturaRef"].Value.ToString();
                                _ventadetalleDto.v_DescripcionProducto = Fila.Cells["v_DescripcionProducto"].Value == null ? null : Fila.Cells["v_DescripcionProducto"].Value.ToString();
                                _ventadetalleDto.EsServicio = Fila.Cells["i_EsServicio"].Value == null || Fila.Cells["i_EsServicio"].Value.ToString() == "0" ? 0 : 1;
                                _ventadetalleDto.d_Percepcion = Fila.Cells["d_Percepcion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Percepcion"].Value.ToString());
                                _ventadetalleDto.i_IdMonedaLP = Fila.Cells["i_IdMonedaLP"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdMonedaLP"].Value.ToString());
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

                                _ventadetalleDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Text.Replace("_", ""));
                                _ventadetalleDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Text);
                                _ventadetalleDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioVenta"].Text);
                                _ventadetalleDto.d_Valor = Fila.Cells["d_Valor"].Value == null || string.IsNullOrEmpty(Fila.Cells["d_Valor"].Text) ? 0 : decimal.Parse(Fila.Cells["d_Valor"].Text);
                                _ventadetalleDto.d_Descuento = Fila.Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Descuento"].Text);
                                _ventadetalleDto.d_isc = Fila.Cells["d_isc"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_isc"].Value.ToString());
                                _ventadetalleDto.d_otrostributos = Fila.Cells["d_otrostributos"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString());
                                _ventadetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _ventadetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _ventadetalleDto.i_Anticipio = Fila.Cells["i_Anticipio"].Value == null ? 0 : int.Parse(Fila.Cells["i_Anticipio"].Value.ToString());
                                _ventadetalleDto.i_InsertaIdUsuario = Fila.Cells["i_InsertaIdUsuario"].Value == null ? 0 : int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _ventadetalleDto.v_NroCuenta = Fila.Cells["v_NroCuenta"].Value == null ? null : Fila.Cells["v_NroCuenta"].Value.ToString().Trim();
                                _ventadetalleDto.i_IdCentroCosto = Fila.Cells["i_IdCentroCosto"].Value == null ? null : Fila.Cells["i_IdCentroCosto"].Value.ToString();
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
                                _ventadetalleDto.i_IdMonedaLP = Fila.Cells["i_IdMonedaLP"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdMonedaLP"].Value.ToString());
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
                                        _movimientodetalleDto.v_NroPedido = Fila.Cells["v_PedidoExportacion"].Value == null ? null : Fila.Cells["v_PedidoExportacion"].Value.ToString();
                                        _movimientodetalleDto.d_CantidadEmpaque = decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
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
                                        _movimientodetalleDto.v_NroPedido = Fila.Cells["v_PedidoExportacion"].Value == null ? null : Fila.Cells["v_PedidoExportacion"].Value.ToString();
                                        _movimientodetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                        _movimientodetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                        _movimientodetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                        _movimientodetalleDto.d_CantidadEmpaque = decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                        __TempDetalle_ModificarDto.Add(_movimientodetalleDto);
                                    }
                                    break;
                            }
                        }
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

        private void CalcularValoresFila(UltraGridRow fila)
        {
            try
            {

                if (cboIGV.Value.ToString() == "-1")
                {
                    UltraMessageBox.Show("Porfavor seleccione el IGV", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var detail = (ventadetalleDto)fila.ListObject;
                if (detail.i_Anticipio == null) return;
                if (detail.v_IdProductoDetalle != null && detail.v_IdProductoDetalle == "N002-PE000000000") return;

                if (detail.d_Cantidad == null) detail.d_Cantidad = 0M;
                if (detail.d_Precio == null) detail.d_Precio = 0M;
                if (detail.v_FacturaRef == null) detail.v_FacturaRef = "0";
                if (detail.d_otrostributos == null) detail.d_otrostributos = 0;
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
                    var isc = CalcularIsc(detail.v_IdProductoDetalle ?? "", price);
                    price -= isc;
                    detail.d_isc = Utils.Windows.DevuelveValorRedondeado(isc * detail.d_Cantidad.Value, 2);
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
                    if (detail.d_otrostributos > 0)
                    {
                        detail.d_PrecioVenta += detail.d_otrostributos;
                        detail.d_PrecioVenta = Utils.Windows.DevuelveValorRedondeado(detail.d_PrecioVenta.Value, 2);
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
            try
            {
                if (grdData.Rows.Any())
                {

                    decimal SumAntValVenta = 0, SumAntIgv = 0, SumAntTotal = 0, SumAntISC = 0, SumAntOG = 0;
                    decimal SumDescuento = 0, SumValVenta = 0, SumIgv = 0, SumTotal = 0, SumVal = 0, SumISC = 0, SumOG = 0;

                    foreach (UltraGridRow Fila in grdData.Rows)
                    {
                        var objRow = (ventadetalleDto)Fila.ListObject;
                        if (objRow.i_IdTipoOperacion > 10) continue; // Operaciones no Onerosas
                        if (Fila.Cells["d_ValorVenta"].Value == null) { Fila.Cells["d_ValorVenta"].Value = "0"; }
                        if (Fila.Cells["d_Igv"].Value == null) { Fila.Cells["d_Igv"].Value = "0"; }
                        if (Fila.Cells["d_PrecioVenta"].Value == null) { Fila.Cells["d_PrecioVenta"].Value = "0"; }
                        if (Fila.Cells["d_Descuento"].Value == null) { Fila.Cells["d_Descuento"].Value = "0"; }
                        if (Fila.Cells["d_isc"].Value == null) { Fila.Cells["d_isc"].Value = "0"; }
                        if (Fila.Cells["d_otrostributos"].Value == null) { Fila.Cells["d_otrostributos"].Value = "0"; }
                        if (Fila.Cells["i_Anticipio"].Value == null) { Fila.Cells["i_Anticipio"].Value = "0"; }

                        switch (Fila.Cells["i_Anticipio"].Value.ToString())
                        {
                            case "1":
                                SumVal = SumVal + (Fila.Cells["d_Valor"].Value == null || string.IsNullOrEmpty(Fila.Cells["d_Valor"].Text) ? 0 : decimal.Parse(Fila.Cells["d_Valor"].Text));
                                SumAntValVenta = SumAntValVenta + decimal.Parse(Fila.Cells["d_ValorVenta"].Text);
                                SumAntIgv = SumAntIgv + decimal.Parse(Fila.Cells["d_Igv"].Text);
                                SumAntTotal = SumAntTotal + decimal.Parse(Fila.Cells["d_PrecioVenta"].Text);
                                SumDescuento = SumDescuento + decimal.Parse(Fila.Cells["d_Descuento"].Text);
                                SumAntISC = SumAntISC + decimal.Parse(Fila.Cells["d_isc"].Value.ToString());
                                SumAntOG = SumAntOG + decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString());
                                break;

                            case "0":
                                SumVal = SumVal + (Fila.Cells["d_Valor"].Value == null || string.IsNullOrEmpty(Fila.Cells["d_Valor"].Text.Trim()) ? 0 : decimal.Parse(Fila.Cells["d_Valor"].Text.Replace("_", "")));
                                SumValVenta = SumValVenta + decimal.Parse(Fila.Cells["d_ValorVenta"].Text);
                                SumIgv = SumIgv + decimal.Parse(Fila.Cells["d_Igv"].Text);
                                SumTotal = SumTotal + decimal.Parse(Fila.Cells["d_PrecioVenta"].Text);
                                SumDescuento = SumDescuento + decimal.Parse(Fila.Cells["d_Descuento"].Text);
                                SumISC = SumISC + decimal.Parse(Fila.Cells["d_isc"].Value.ToString());
                                SumOG = SumOG + decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString());
                                break;
                        }
                    }
                    //Descuento Global si existe: 
                    decimal porcDecuento = decimal.TryParse(txtPorcDescuento.Text, out porcDecuento) ? porcDecuento / 100 : 0;
                    var factDesc = 1 - porcDecuento;

                    txtAnticipio.Text = SumAntValVenta.ToString("0.00");
                    txtValor.Text = SumVal.ToString("0.00");
                    var ValVenta = SumValVenta - SumAntValVenta;
                    txtDescuento.Text = (SumDescuento + Utils.Windows.DevuelveValorRedondeado(ValVenta * porcDecuento, 2)).ToString("0.00");
                    txtValorVenta.Text = (ValVenta * factDesc).ToString("0.00");
                    txtIGV.Text = ((SumIgv - SumAntIgv) * factDesc).ToString("0.00");
                    txtISC.Text = ((SumISC - SumAntISC) * factDesc).ToString("0.00");

                    var total = (SumTotal - SumAntTotal) * factDesc;
                    txtTotal.Text = total.ToString("0.00");
                    txtOtrosTributos.Text = (SumOG - SumAntOG).ToString("0.00");

                    Total = decimal.Parse(txtTotal.Text);
                    Redondeado = decimal.Parse(Math.Round(Total, 1, MidpointRounding.AwayFromZero).ToString("0.00"));
                    Residuo = (Total - Redondeado) * -1;
                    _Redondeado = Residuo == 0 ? true : false;
                    btnRedondear.Enabled = Residuo != 0;



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
                            FilasPercepcion.ForEach(o => o.Cells["d_Percepcion"].Value = "0");
                    }
                    else
                    {
                        List<UltraGridRow> FilasPercepcion = grdData.Rows.Where(p => p.Cells["i_EsAfectoPercepcion"].Value != null && p.Cells["i_EsAfectoPercepcion"].Value.ToString() == "1").ToList();
                        FilasPercepcion.ForEach(o => o.Cells["d_Percepcion"].Value = "0");
                        txtTotal.Text = (decimal.Parse(txtTotal.Text) + FilasPercepcion.Sum(p => decimal.Parse(p.Cells["d_Percepcion"].Value.ToString()))).ToString("0.00");
                    }

                    txtMonto.Text = cboTipoKardex.Value != null && cboTipoKardex.Value.ToString() != "V" ? txtTotal.Text : txtMonto.Text;
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }

        private void ComprobarExistenciaCorrelativoDocumento()
        {
            OperationResult objOperationResult = new OperationResult();
            if (_objVentasBL.ComprobarExistenciaCorrelativoDocumento(ref objOperationResult, int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text, txtCorrelativoDocIni.Text, _ventaDto.v_IdVenta) == true)
            {

                UltraMessageBox.Show("El documento ya ha sido registrado anteriormente", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtRucCliente.Enabled = false;

                btnGuardar.Enabled = false;
            }
            else
            {
                txtRucCliente.Enabled = true;
                btnGuardar.Enabled = true;
            }
        }

        private void DevuelveVentaReferencia(bool Respuesta)
        {
            if (Respuesta)
            {
                if (txtSerieDocRef.Text.Trim() != "" && txtCorrelativoDocRef.Text.Trim() != "" && cboDocumentoRef.Value.ToString() != "-1")
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
                        MessageBox.Show(@"Esta venta ya cuenta con una nota de crédito activa.", @"Validación",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    string IdCompraRef = _objVentasBL.DevolverIdVenta(ref objOperationResult, int.Parse(cboDocumentoRef.Value.ToString()), txtSerieDocRef.Text, txtCorrelativoDocRef.Text);
                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage,
                            @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    ventaRefDto = _objVentasBL.ObtenerVentaCabecera(ref objOperationResult, IdCompraRef);
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
                        txtCodigoCliente.Text = ventaRefDto.CodigoCliente;
                        txtPorcComision.Text = ventaRefDto.d_PocComision.ToString();
                        txtPorcDescuento.Text = ventaRefDto.d_PorcDescuento.ToString();
                        txtDescuento.Text = ventaRefDto.d_Descuento.ToString();
                        txtDimensiones.Text = ventaRefDto.v_BultoDimensiones;
                        txtGuiaRemisionCorrelativo.Text = ventaRefDto.v_NroGuiaRemisionCorrelativo;
                        txtGuiaRemisionSerie.Text = ventaRefDto.v_NroGuiaRemisionSerie;
                        txtIGV.Text = ventaRefDto.d_IGV.ToString();
                        txtMarca.Text = ventaRefDto.v_Marca;
                        txtConcepto.Text = ventaRefDto.v_Concepto;
                        txtNroBulto.Text = ventaRefDto.v_NroBulto;
                        txtNroDias.Text = ventaRefDto.i_NroDias.ToString();
                        txtNroPedido.Text = ventaRefDto.v_NroPedido;
                        txtOrdenCompra.Text = ventaRefDto.v_OrdenCompra;
                        txtPesoBrutoKg.Text = ventaRefDto.d_PesoBrutoKG.ToString();
                        txtPesoNetoKg.Text = ventaRefDto.d_PesoNetoKG.ToString();
                        txtTotal.Text = ventaRefDto.d_Total.ToString();
                        txtValorVenta.Text = ventaRefDto.d_ValorVenta.ToString();
                        txtRucCliente.Text = ventaRefDto.NroDocCliente.ToString();
                        txtRazonSocial.Text = ventaRefDto.NombreCliente;
                        dtpFechaOrden.Value = ventaRefDto.t_FechaOrdenCompra.Value;
                        dtpFechaRef.Value = ventaRefDto.t_FechaRegistro.Value;
                        cboCondicionPago.Value = ventaRefDto.i_IdCondicionPago.ToString();
                        cboEstablecimiento.Value = ventaRefDto.i_IdEstablecimiento.ToString();
                        cboEstado.Value = ventaRefDto.i_IdEstado.ToString();
                        cboIGV.Value = ventaRefDto.i_IdIgv.ToString();
                        cboMoneda.Value = ventaRefDto.i_IdMoneda.ToString();
                        cboMVenta.Value = ventaRefDto.i_IdMedioPagoVenta.ToString();
                        cboPuntoDestino.Value = ventaRefDto.i_IdPuntoDestino.ToString();
                        cboPuntoEmbarque.Value = ventaRefDto.i_IdPuntoEmbarque.ToString();
                        cboTipoEmbarque.Value = ventaRefDto.i_IdTipoEmbarque.ToString();
                        cboTipoOperacion.Value = ventaRefDto.i_IdTipoOperacion.ToString();
                        cboTipoVenta.Value = int.Parse(ventaRefDto.i_IdTipoVenta.ToString()).ToString("00");
                        cboVendedor.Value = ventaRefDto.v_IdVendedor.ToString();
                        cboVendedorRef.Value = ventaRefDto.v_IdVendedorRef.ToString();
                        chkAfectoIGV.Checked = ventaRefDto.i_EsAfectoIgv == 1 ? true : false;
                        chkDeduccionAnticipio.Checked = ventaRefDto.i_DeduccionAnticipio == 1 ? true : false;
                        chkDrawnBack.Checked = ventaRefDto.i_DrawBack == 1 ? true : false;
                        chkPrecInIGV.Checked = ventaRefDto.i_PreciosIncluyenIgv == 1 ? true : false;
                        _ventaDto.v_IdCliente = ventaRefDto.v_IdCliente;
                        CargarDetalle(IdCompraRef);
                        if (grdData.Rows.Any())
                        {
                            foreach (UltraGridRow Fila in grdData.Rows)
                            {
                                //cambio los estados de las filas devueltas como temporales y modificadas para que al llenarse los temporales se inserten en el presente doc.
                                Fila.Cells["i_RegistroTipo"].Value = "Temporal";
                                Fila.Cells["i_RegistroEstado"].Value = "Modificado";
                            }
                        }
                        btnGuardar.Enabled = true;
                    }
                    else
                    {
                        UltraMessageBox.Show("No se ha registrado el documento de referencia", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LimpiarCabecera();
                        CargarDetalle("");
                        btnGuardar.Enabled = false;
                    }
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
                FormatoCantidad = "nnnnnn.";
                for (int i = 0; i < DecimalesCantidad; i++)
                {
                    FormatoCantidad = FormatoCantidad + sharp;
                }
            }
            else
            {
                FormatoCantidad = "nnnnnn";
            }

            if (DecimalesPrecio > 0)
            {
                string sharp = "n";
                FormatoPrecio = "nnnnnn.";
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

            c = grdData.DisplayLayout.Bands[0].Columns["v_CodigoInterno"];
            c.CellActivation = Activation.ActivateOnly;
            c.CellClickAction = CellClickAction.CellSelect;

            btnAgregar.Enabled = false;
            btnEliminar.Enabled = false;
            btnBuscarDetraccion.Enabled = false;
            txtRucCliente.Enabled = false;
            cboCondicionPago.Enabled = false;
            cboDocumento.Enabled = false;
            cboDocumentoRef.Enabled = false;
            cboEstablecimiento.Enabled = false;
            //cboEstado.Enabled = false;
            cboIGV.Enabled = false;
            cboMoneda.Enabled = false;
            cboMVenta.Enabled = false;
            cboPuntoDestino.Enabled = false;
            cboPuntoEmbarque.Enabled = false;
            cboTipoEmbarque.Enabled = false;
            cboTipoOperacion.Enabled = false;
            cboTipoVenta.Enabled = false;
            cboVendedor.Enabled = false;
            cboVendedorRef.Enabled = false;
            txtAnticipio.Enabled = false;
            txtCodigoCliente.Enabled = false;
            txtConcepto.Enabled = false;
            txtCorrelativo.Enabled = false;
            txtCorrelativoDocFin.Enabled = false;
            txtCorrelativoDocIni.Enabled = false;
            txtCorrelativoDocRef.Enabled = false;
            txtDescripcionColumnas.Enabled = false;
            txtDimensiones.Enabled = false;
            txtDireccion.Enabled = false;
            txtGuiaRemisionSerie.Enabled = false;
            txtIGV.Enabled = false;
            txtMarca.Enabled = false;
            txtMes.Enabled = false;
            txtNroBulto.Enabled = false;
            txtNroDias.Enabled = false;
            txtNroPedido.Enabled = false;
            txtOrdenCompra.Enabled = false;
            txtPeriodo.Enabled = false;
            txtPesoBrutoKg.Enabled = false;
            txtPesoNetoKg.Enabled = false;
            txtPorcComision.Enabled = false;
            txtPorcDescuento.Enabled = false;
            txtRazonSocial.Enabled = false;
            txtRucCliente.Enabled = false;
            txtSerieDoc.Enabled = false;
            txtTipoCambio.Enabled = false;
            chkAfectoIGV.Enabled = false;
            chkDeduccionAnticipio.Enabled = false;
            chkDrawnBack.Enabled = false;
            chkPrecInIGV.Enabled = false;
            txtGuiaRemisionCorrelativo.Enabled = false;
            dtpFechaOrden.Enabled = false;
            dtpFechaRef.Enabled = false;
            dtpFechaRegistro.Enabled = false;
            dtpFechaVencimiento.Enabled = false;
            //btnGuardar.Enabled = false;

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
                _ventaDto.i_IdDireccionCliente = int.Parse(Cadena[4]);
                txtCodigoCliente.Text = Cadena[1];
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
                _Redondeado = true;
                UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["d_Cantidad"].Value = "1";
                row.Cells["d_Precio"].Value = Residuo.ToString();
                row.Cells["i_IdUnidadMedida"].Value = "-1";
                row.Cells["i_Anticipio"].Value = "0";
                row.Cells["d_Descuento"].Value = "0";
                row.Cells["i_IdCentroCosto"].Value = "0";
                row.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString() == "5" ? "1" : cboTipoOperacion.Value.ToString();
                row.Cells["d_Valor"].Value = Residuo.ToString();
                row.Cells["EsRedondeo"].Value = "1";
                row.Cells["d_PrecioVenta"].Value = Residuo.ToString();
                row.Cells["v_NroCuenta"].Value = Residuo < 0 ? Globals.ClientSession.v_NroCuentaRedondeoPerdida : Globals.ClientSession.v_NroCuentaRedondeoGanancia;
                row.Cells["i_IdUnidadMedida"].Value = "15";
                row.Cells["v_IdProductoDetalle"].Value = "N002-PE000000000";
                row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                row.Cells["v_CodigoInterno"].Value = "00000000";
                row.Cells["v_DescripcionProducto"].Value = "REDONDEO";
                row.Cells["i_EsServicio"].Value = "1";
                row.Cells["i_EsAfectoDetraccion"].Value = "0";
                row.Cells["d_isc"].Value = "0";
                row.Cells["d_otrostributos"].Value = "0";
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
                int IdAlmacen = int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString());
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                    {
                        if (Filas[i].Cells["v_IdProductoDetalle"].Value.ToString() == Fila.Cells["v_IdProductoDetalle"].Value.ToString() && IdAlmacen == int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()))
                        {
                            UltraMessageBox.Show("Uno de los productos seleccionados ya existe en el detalle", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }
                if (i == 0)
                {
                    grdData.ActiveRow.Cells["v_DescripcionProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                    grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                    grdData.ActiveRow.Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                    grdData.ActiveRow.Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                    grdData.ActiveRow.Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                    grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_EsAfectoDetraccion"].Value = Filas[i].Cells["i_EsAfectoDetraccion"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsAfectoDetraccion"].Value.ToString());
                    grdData.ActiveRow.Cells["d_Precio"].Value = DevolverPrecioProducto(Filas[i].Cells["IdMoneda"].Value != null ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString()) : -1, Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()), int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);
                    grdData.ActiveRow.Cells["i_EsNombreEditable"].Value = Filas[i].Cells["i_NombreEditable"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_NombreEditable"].Value.ToString());
                    grdData.ActiveRow.Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                    grdData.ActiveRow.Cells["d_Cantidad"].Value = "1.0000";
                    grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = "0";
                    grdData.ActiveRow.Cells["d_isc"].Value = "0";
                    grdData.ActiveRow.Cells["d_otrostributos"].Value = "0";
                    grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                    grdData.ActiveRow.Cells["v_NroCuenta"].Value = Filas[i].Cells["NroCuentaVenta"].Value != null ? Filas[i].Cells["NroCuentaVenta"].Value.ToString() : "-1";
                    grdData.ActiveRow.Cells["i_ValidarStock"].Value = Filas[i].Cells["i_ValidarStock"].Value != null ? Filas[i].Cells["i_ValidarStock"].Value.ToString() : "0";
                    grdData.ActiveRow.Cells["i_EsAfectoPercepcion"].Value = Filas[i].Cells["i_EsAfectoPercepcion"].Value != null ? Filas[i].Cells["i_EsAfectoPercepcion"].Value.ToString() : "0";
                    grdData.ActiveRow.Cells["d_TasaPercepcion"].Value = Filas[i].Cells["d_TasaPercepcion"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_TasaPercepcion"].Value.ToString());
                    grdData.ActiveRow.Cells["i_IdMonedaLP"].Value = Filas[i].Cells["IdMoneda"].Value != null ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString()) : -1;
                }
                else
                {
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Agregado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_IdUnidadMedida"].Value = "-1";
                    row.Cells["d_Cantidad"].Value = "0";
                    row.Cells["d_Precio"].Value = "0";
                    row.Cells["i_IdUnidadMedida"].Value = "-1";
                    row.Cells["i_Anticipio"].Value = "0";
                    row.Cells["d_Descuento"].Value = "0";
                    row.Cells["d_Percepcion"].Value = "0";
                    row.Cells["d_Valor"].Value = "0";
                    row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    row.Activate();
                    grdData.ActiveRow.Cells["v_DescripcionProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                    grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                    grdData.ActiveRow.Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                    grdData.ActiveRow.Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                    grdData.ActiveRow.Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                    grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                    grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_EsAfectoDetraccion"].Value = Filas[i].Cells["i_EsAfectoDetraccion"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsAfectoDetraccion"].Value.ToString());
                    grdData.ActiveRow.Cells["d_Precio"].Value = DevolverPrecioProducto(Filas[i].Cells["IdMoneda"].Value != null ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString()) : -1, Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString()), int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);
                    grdData.ActiveRow.Cells["i_EsNombreEditable"].Value = Filas[i].Cells["i_NombreEditable"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_NombreEditable"].Value.ToString());
                    grdData.ActiveRow.Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                    grdData.ActiveRow.Cells["d_Cantidad"].Value = "1.0000";
                    grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = "0";
                    grdData.ActiveRow.Cells["d_isc"].Value = "0";
                    grdData.ActiveRow.Cells["d_otrostributos"].Value = "0";
                    grdData.ActiveRow.Cells["v_NroCuenta"].Value = Filas[i].Cells["NroCuentaVenta"].Value != null ? Filas[i].Cells["NroCuentaVenta"].Value.ToString() : "-1";
                    grdData.ActiveRow.Cells["i_ValidarStock"].Value = Filas[i].Cells["i_ValidarStock"].Value != null ? Filas[i].Cells["i_ValidarStock"].Value.ToString() : "0";
                    grdData.ActiveRow.Cells["i_EsAfectoPercepcion"].Value = Filas[i].Cells["i_EsAfectoPercepcion"].Value != null ? Filas[i].Cells["i_EsAfectoPercepcion"].Value.ToString() : "0";
                    grdData.ActiveRow.Cells["d_TasaPercepcion"].Value = Filas[i].Cells["d_TasaPercepcion"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_TasaPercepcion"].Value.ToString());
                    grdData.ActiveRow.Cells["i_IdMonedaLP"].Value = Filas[i].Cells["IdMoneda"].Value != null ? int.Parse(Filas[i].Cells["IdMoneda"].Value.ToString()) : -1;
                }
                if ((bool)Filas[i].Cells["EsAfectoIsc"].Value)
                    AddIsc(Filas[i].Cells["v_IdProductoDetalle"].Value.ToString());
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

            //if (cboDocumento.Value.ToString() != "7" && cboDocumento.Value.ToString() != "8")
            if (!_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) && cboDocumento.Value.ToString() != "8")
            {
                if (grdData.Rows.Count(p => (p.Cells["i_Anticipio"].Value.ToString() == "0") && (p.Cells["d_Cantidad"].Value == null || decimal.Parse(p.Cells["d_Cantidad"].Value.ToString().Trim()) <= 0)) != 0)
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente la Cantidad.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UltraGridRow Row = grdData.Rows.FirstOrDefault(p => (p.Cells["i_Anticipio"].Value.ToString() == "0") && (p.Cells["d_Cantidad"].Value == null || decimal.Parse(p.Cells["d_Cantidad"].Value.ToString().Trim()) <= 0));
                    grdData.Selected.Cells.Add(Row.Cells["d_Cantidad"]);
                    grdData.Focus();
                    Row.Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_Cantidad"];
                    this.grdData.ActiveCell = aCell;
                    return false;
                }
            }

            if (!EsDocumentoInternoPermiteMontoCero &&  grdData.Rows.Count(p => (p.Cells["i_Anticipio"].Value.ToString() == "0") && 
                (p.Cells["d_Precio"].Value == null || decimal.Parse(p.Cells["d_Precio"].Value.ToString().Trim()) == 0)) != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente el Precio.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(p => (p.Cells["i_Anticipio"].Value.ToString() == "0") && (p.Cells["d_Precio"].Value == null || decimal.Parse(p.Cells["d_Precio"].Value.ToString().Trim()) == 0));
                grdData.Selected.Cells.Add(Row.Cells["d_Precio"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_Precio"];
                this.grdData.ActiveCell = aCell;
                return false;
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

            if (grdData.Rows.Where(p => p.Cells["i_IdTipoOperacion"].Value == null || p.Cells["i_IdTipoOperacion"].Value.ToString() == "-1" || p.Cells["i_IdTipoOperacion"].Value.ToString() == "5").Count() != 0)
            {
                UltraMessageBox.Show("Porfavor especifique los tipos de operación correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(p => p.Cells["i_IdTipoOperacion"].Value == null || p.Cells["i_IdTipoOperacion"].Value.ToString() == "-1" || p.Cells["i_IdTipoOperacion"].Value.ToString() == "5").FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["i_IdTipoOperacion"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdTipoOperacion"];
                this.grdData.ActiveCell = aCell;
                grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                return false;
            }


            if (!_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString()))
                &&
                grdData.Rows.Any(
                    p =>
                        p.Cells["v_IdProductoDetalle"].Value == null
                        &&
                        (!string.IsNullOrEmpty(p.Cells["v_NroCuenta"].Text) && !p.Cells["v_NroCuenta"].Text.Contains("ANTICIPIO"))))
            {
                UltraMessageBox.Show("Por favor ingrese un Producto al detalle.", "Sistema", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                UltraGridRow Row =
                    grdData.Rows.FirstOrDefault(
                        p =>
                            p.Cells["v_IdProductoDetalle"].Value == null
                            &&
                            (!string.IsNullOrEmpty(p.Cells["v_NroCuenta"].Text) &&
                             !p.Cells["v_NroCuenta"].Text.Contains("ANTICIPIO")));

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

            if (grdData.ActiveCell.Column.Key == "v_DescripcionProducto" && grdData.ActiveRow.Cells["EsNombreEditable"].Value != null && grdData.ActiveRow.Cells["i_EsNombreEditable"].Value.ToString() == "0")
            {
                if (char.IsLetterOrDigit(Convert.ToChar(e.KeyCode)))
                    e.SuppressKeyPress = true;
                else if (char.IsSymbol(Convert.ToChar(e.KeyCode)))
                    e.SuppressKeyPress = true;
                else if (Convert.ToChar(e.KeyCode) < 200)
                    e.SuppressKeyPress = true;
                else
                    e.SuppressKeyPress = true;
            }

            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
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
                {
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "v_CodigoInterno":
                    if (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) || cboDocumento.Value.ToString() == "8" || e.Cell.Activation == Activation.Disabled) return;

                    if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "Temporal")
                    {
                        if (grdData.ActiveRow.Cells["i_IdAlmacen"].Value == null) return;

                        if (_objVentasBL.EsValidoCodProducto(e.Cell.Text))
                        {
                            productoshortDto prod = Globals.ClientSession.i_UsaListaPrecios == 1 ?
                                _objVentasBL.DevolverArticuloPorCodInterno(e.Cell.Text, _ventaDto.v_IdCliente, int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()))
                            : _objVentasBL.DevolverArticuloPorCodInterno(e.Cell.Text, int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()));
                            if (prod != null)
                            {
                                grdData.ActiveRow.Cells["v_DescripcionProducto"].Value = prod.v_Descripcion;
                                grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = prod.v_IdProductoDetalle;
                                grdData.ActiveRow.Cells["v_CodigoInterno"].Value = prod.v_CodInterno;
                                grdData.ActiveRow.Cells["Empaque"].Value = prod.d_Empaque;
                                grdData.ActiveRow.Cells["UMEmpaque"].Value = prod.EmpaqueUnidadMedida;
                                grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                                grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = prod.i_IdUnidadMedida;
                                grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = "0";
                                grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = prod.i_IdUnidadMedida;
                                grdData.ActiveRow.Cells["i_EsAfectoDetraccion"].Value = prod.i_EsAfectoDetraccion;
                                grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = (string)cboTipoOperacion.Value != "5" ? cboTipoOperacion.Value.ToString() : "1";
                                grdData.ActiveRow.Cells["d_isc"].Value = "0";
                                grdData.ActiveRow.Cells["d_otrostributos"].Value = "0";
                                grdData.ActiveRow.Cells["d_Precio"].Value = DevolverPrecioProducto(prod.IdMoneda, prod.d_Precio ?? 0, int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);
                                grdData.ActiveRow.Cells["i_EsNombreEditable"].Value = prod.i_NombreEditable;
                                grdData.ActiveRow.Cells["i_EsServicio"].Value = prod.i_EsServicio;
                                grdData.ActiveRow.Cells["v_NroCuenta"].Value = prod.NroCuentaVenta;
                                grdData.ActiveRow.Cells["i_ValidarStock"].Value = prod.i_ValidarStock ?? 0;
                                grdData.ActiveRow.Cells["i_EsAfectoPercepcion"].Value = prod.i_EsAfectoPercepcion ?? 0;
                                grdData.ActiveRow.Cells["d_TasaPercepcion"].Value = prod.d_TasaPercepcion;
                                grdData.ActiveRow.Cells["i_IdMonedaLP"].Value = prod.IdMoneda;
                                grdData.ActiveRow.Cells["d_Cantidad"].Value = "1.0000";
                                if (prod.EsAfectoIsc)
                                    AddIsc(prod.v_IdProductoDetalle);
                            }
                            else
                            {
                                UltraMessageBox.Show("El Artículo existe Pero no tuvo ingresos a este almacén", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                btnEliminar_Click(sender, e);
                            }
                        }
                        else
                        {
                            Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()), "PedidoVenta", _ventaDto.v_IdCliente, grdData.ActiveRow.Cells["v_CodigoInterno"].Text ?? string.Empty, UserConfig.Default.appTipoBusquedaPredeterminadaProducto);
                            frm.ShowDialog();
                            if (frm._NombreProducto != null)
                            {
                                int IdAlmacen = int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString());
                                foreach (UltraGridRow Fila in grdData.Rows)
                                {
                                    if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                                    {
                                        if (frm._IdProducto == Fila.Cells["v_IdProductoDetalle"].Value.ToString() && IdAlmacen == int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()))
                                        {
                                            UltraMessageBox.Show("El producto ya existe en el detalle", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            return;
                                        }
                                    }
                                }
                                grdData.ActiveRow.Cells["v_DescripcionProducto"].Value = frm._NombreProducto.ToString();
                                grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = frm._IdProducto.ToString();
                                grdData.ActiveRow.Cells["v_CodigoInterno"].Value = frm._CodigoInternoProducto.ToString();
                                grdData.ActiveRow.Cells["Empaque"].Value = frm._Empaque != null ? frm._Empaque.ToString() : null;
                                grdData.ActiveRow.Cells["UMEmpaque"].Value = frm._UnidadMedida != null ? frm._UnidadMedida.ToString() : string.Empty;
                                grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                                grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = "0";
                                grdData.ActiveRow.Cells["d_isc"].Value = "0";
                                grdData.ActiveRow.Cells["d_otrostributos"].Value = "0";
                                grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = frm._UnidadMedidaEmpaque;
                                grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = (string)cboTipoOperacion.Value != "5" ? cboTipoOperacion.Value.ToString() : "1";
                                grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = frm._UnidadMedidaEmpaque;
                                grdData.ActiveRow.Cells["i_EsAfectoDetraccion"].Value = frm._EsAfectoDetraccion;
                                grdData.ActiveRow.Cells["d_Precio"].Value = DevolverPrecioProducto(frm._IdMoneda, frm._PrecioUnitario, int.Parse(cboMoneda.Value.ToString()), !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);
                                grdData.ActiveRow.Cells["i_EsNombreEditable"].Value = frm._NombreEditable;
                                grdData.ActiveRow.Cells["i_EsServicio"].Value = frm._EsServicio.ToString();
                                grdData.ActiveRow.Cells["v_NroCuenta"].Value = frm._NroCuentaVenta;
                                grdData.ActiveRow.Cells["i_ValidarStock"].Value = frm._ValidarStock == null ? 0 : frm._ValidarStock;
                                grdData.ActiveRow.Cells["i_EsAfectoPercepcion"].Value = frm._EsAfectoPercepcion == null ? 0 : frm._EsAfectoPercepcion;
                                grdData.ActiveRow.Cells["d_TasaPercepcion"].Value = frm._TasaPercepcion;
                                grdData.ActiveRow.Cells["i_IdMonedaLP"].Value = frm._IdMoneda;
                                grdData.ActiveRow.Cells["d_Cantidad"].Value = "1.0000";
                                if (frm._EsAfectoisc)
                                    AddIsc(frm._IdProducto);
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

                return pdecPrecio;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            grdData.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";

            if (e.Cell.Column.Key == "i_Anticipio")
            {
                EmbeddableEditorBase editor = e.Cell.EditorResolved;
                e.Cell.Value = editor.Value;
                CalcularValoresFila(grdData.ActiveRow);
                FilaAnticipio(e.Cell.Row);
            }

            if (e.Cell.Column.Key == "v_NroCuenta")
            {
                EmbeddableEditorBase editor = e.Cell.EditorResolved;
                e.Cell.Value = editor.Value;
                if (!string.IsNullOrEmpty(e.Cell.Text.Trim()) && e.Cell.Text.ToUpper().Contains("ANTICIPIO"))
                {
                    if (!chkDeduccionAnticipio.Checked)
                    {
                        var mensaje =
                        MessageBox.Show(
                            "Al escoger un rubro anticipio sólo podrá estar este único como detalle, ¿Desea Continuar?",
                            "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (mensaje == DialogResult.Yes)
                        {
                            var filaAnticipio = e.Cell.Row;

                            grdData.Rows.Where(f => f != filaAnticipio)
                                .ToList().ForEach(fila => fila.Delete(false));
                            btnAgregar.Enabled = false;

                            return;
                        }

                        e.Cell.Value = "-1";
                    }
                    e.Cell.Row.Cells["i_EsServicio"].Value = "1";
                }
            }

            if (e.Cell.Column.Key == "i_IdTipoOperacion")
            {
                e.Cell.Value = e.Cell.EditorResolved.Value;
                CalcularValoresFila(e.Cell.Row);
            }

            if (e.Cell.Column.Key == "d_Cantidad")
                CalcularTotales();
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

                    case "v_FacturaRef":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDocumentoCelda(Celda, e);
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
                        txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_NombreCuenta"].Value != null ? grdData.ActiveRow.Cells["_NombreCuenta"].Value.ToString() : string.Empty;
                        if (grdData.ActiveRow.Cells["EsRedondeo"].Value != null)
                            e.Cell.CancelUpdate();
                        break;

                    case "i_IdDestino":
                        txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_Destino"].Value != null ? grdData.ActiveRow.Cells["_Destino"].Value.ToString() : string.Empty;
                        break;

                    case "i_IdCentroCostos":
                        txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_CentroCosto"].Value != null ? grdData.ActiveRow.Cells["_CentroCosto"].Value.ToString() : string.Empty;
                        break;

                    case "i_IdAlmacen":
                        txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_Almacen"].Value != null ? grdData.ActiveRow.Cells["_Almacen"].Value.ToString() : string.Empty;
                        if (grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null)
                        {
                            e.Cell.CancelUpdate();
                        }
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
                txtDescripcionColumnas.Text = string.Empty;
            }
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (strModo != "Consulta")
            {
                if (!string.IsNullOrEmpty(_ventaDto.v_NroPedido)) return;
                if (grdData.Rows.ToList().Any(f => f.Cells["v_NroCuenta"].Value != null && f.Cells["v_NroCuenta"].Text.Contains("ANTICIPIO"))) return;

                btnAgregar.Enabled = true;
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

        }

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            if (grdData.ActiveCell.Column.Key == "v_FacturaRef")
            {
                if (grdData.ActiveRow.Cells["v_FacturaRef"].Value != null)
                {
                    string NroGuia;
                    NroGuia = grdData.ActiveRow.Cells["v_FacturaRef"].Value.ToString();
                    if (NroGuia.Where(x => x.ToString() == "-").Count() == 1)
                    {
                        string[] SerieCorrelativo = new string[2];
                        SerieCorrelativo = NroGuia.Split(new Char[] { '-' });
                        grdData.ActiveRow.Cells["v_FacturaRef"].Value = int.Parse(SerieCorrelativo[0]).ToString("0000") + "-" + int.Parse(SerieCorrelativo[1]).ToString("00000000");
                    }
                }
            }

            if (grdData.ActiveCell.Column.Key == "d_isc" || grdData.ActiveCell.Column.Key == "d_otrostributos" || grdData.ActiveCell.Column.Key == "d_Cantidad" || grdData.ActiveCell.Column.Key == "d_Precio" || grdData.ActiveCell.Column.Key == "d_PrecioVenta" || grdData.ActiveCell.Column.Key == "d_isc" || grdData.ActiveCell.Column.Key == "d_otrostributos")
            {
                CalcularValoresFila(grdData.ActiveRow);
            }
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");
            if (e.Row.Cells["EsRedondeo"].Value != null)
            {
                e.Row.Appearance.ForeColor = Color.Red;
            }
            else if (e.Row.Cells["i_EsAfectoPercepcion"].Value != null && e.Row.Cells["i_EsAfectoPercepcion"].Value.ToString() == "1")
            {
                e.Row.Appearance.ForeColor = Color.Black;
                e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
            }
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (_ventaDto.v_NroPedido == string.Empty | _ventaDto.v_NroPedido == null)
            {
                if (strModo != "Consulta")
                {
                    if (grdData.ActiveRow == null)
                    {
                        btnEliminar.Enabled = false;
                    }
                    else
                    {
                        btnEliminar.Enabled = true;
                    }
                }
                else
                {
                    btnEliminar.Enabled = false;
                }
            }
        }
        #endregion

        private void txtRucCliente_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtRucCliente, e);
        }

        private void txtRucCliente_Validating_1(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRucCliente.Text.Trim()) && _ventaDto.v_IdCliente != "N002-CL000000000" && _ventaDto.v_IdCliente != "N002-CL999999999")
            {
                if (cboDocumento.Value.ToString() == "1")
                {
                    if (txtRucCliente.TextLength != 11)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _ventaDto.v_IdCliente = null;
                        _ventaDto.i_IdDireccionCliente = -1;
                        txtCodigoCliente.Clear();
                        txtRazonSocial.Clear();
                        txtRucCliente.Focus();
                        txtDireccion.Clear();
                        lblRevisionRUC.Text = string.Empty;
                    }
                    else
                    {
                        if (Utils.Windows.ValidarRuc(txtRucCliente.Text.Trim()) != true)
                        {
                            UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _ventaDto.v_IdCliente = null;
                            _ventaDto.i_IdDireccionCliente = -1;
                            txtCodigoCliente.Clear();
                            txtRazonSocial.Clear();
                            txtRucCliente.Focus();
                            txtDireccion.Clear();
                            lblRevisionRUC.Text = string.Empty;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(txtRucCliente.Text.Trim())) lblRevisionRUC.Text = string.Empty;
        }

        private void txtRucCliente_TextChanged(object sender, EventArgs e)
        {
            lblRevisionRUC.Text = string.Empty;
        }

        private void txtRucCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnConsultar")
            {
                if (txtRucCliente.Text.Trim().Length == 11 && Utils.Windows.ValidarRuc(txtRucCliente.Text.Trim()))
                {
                    string[] _Contribuyente = new string[10];

                    frmCustomerCapchaSUNAT frm = new frmCustomerCapchaSUNAT(txtRucCliente.Text.Trim());
                    frm.ShowDialog();
                    if (frm.ConectadoRecibido == true)
                    {
                        _Contribuyente = frm.DatosContribuyente;

                        lblRevisionRUC.Text = string.Format("Estado: {0} | Condición: {1}", _Contribuyente[3], _Contribuyente[4]);
                    }
                }
            }
        }

        private void cboTipoVenta_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cboTipoVenta.Value != null && int.Parse(cboTipoVenta.Value.ToString()) == 5)
            {
                List<KeyValueDTO> xx = (List<KeyValueDTO>)cboTipoOperacion.DataSource;

                if (xx != null && xx.Any(p => p.Id == "4"))
                {
                    cboTipoOperacion.Value = "4";
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

        private void cboTipoVenta_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtValorFOB_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtValorFOB, e);
        }

        private void txtRucCliente_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (txtRucCliente.Text.Trim() == "" && cboDocumento.Value != null && int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.Factura)
                {
                    EventArgs e1 = new EventArgs();
                    btnBuscarDetraccion_Click(sender, e1);

                }

                else if (txtRucCliente.Text.Trim() != "" && txtRucCliente.TextLength <= 5)
                {
                    Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtRucCliente.Text);
                    frm.ShowDialog();
                    if (frm._IdCliente != null)
                    {
                        _ventaDto.v_IdCliente = frm._IdCliente;
                        _ventaDto.i_IdDireccionCliente = frm._IdDireccionCliente;
                        txtCodigoCliente.Text = frm._CodigoCliente;
                        txtRazonSocial.Text = frm._RazonSocial;
                        txtRucCliente.Text = frm._NroDocumento;
                        IdIdentificacion = frm._TipoDocumento;
                        VerificarLineaCreditoCliente(_ventaDto.v_IdCliente);

                    }
                }
                else if (txtRucCliente.TextLength == 11 || txtRucCliente.TextLength == 8)
                {
                    if (cboDocumento.Value.ToString() == "1" && txtRucCliente.TextLength == 8)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _ventaDto.v_IdCliente = null;
                        _ventaDto.i_IdDireccionCliente = -1;
                        txtCodigoCliente.Clear();
                        txtRazonSocial.Clear();
                        txtDireccion.Clear();
                        return;
                    }
                    if (txtRucCliente.TextLength == 11 && Utils.Windows.ValidarRuc(txtRucCliente.Text) == false)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _ventaDto.v_IdCliente = null;
                        _ventaDto.i_IdDireccionCliente = -1;
                        txtCodigoCliente.Clear();
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
                        txtCodigoCliente.Text = DatosProveedor[1];
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
                                txtCodigoCliente.Text = DatosProveedor[1];
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
                    txtCodigoCliente.Text = string.Empty;
                    txtRazonSocial.Text = string.Empty;
                    txtDireccion.Clear();
                }
            }
        }

        private void txtGuiaRemisionSerie_Validating(object sender, CancelEventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtGuiaRemisionSerie, "{0:0000}");
        }

        private void txtGuiaRemisionCorrelativo_Validating(object sender, CancelEventArgs e)
        {

        }

        private void cboEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (strModo == "Nuevo" && cboEstado.Value.ToString() == "0" && _ventaDto.v_IdCliente == null)
            {
                if (UltraMessageBox.Show("¿Desea Realizar un comprobante Anulado?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    OperationResult objOperationResult = new OperationResult();
                    var ClienteAnulado = new ClienteBL().CreaVerificaClienteAnulado(ref objOperationResult);

                    if (objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    _ventaDto.v_IdCliente = ClienteAnulado.v_IdCliente;
                    txtRazonSocial.Text = ClienteAnulado.v_RazonSocial;
                    txtDireccion.Text = ClienteAnulado.v_DirecPrincipal;
                    txtCodigoCliente.Text = ClienteAnulado.v_CodCliente;
                    txtRucCliente.Text = ClienteAnulado.v_NroDocIdentificacion;
                    cboDocumento.Value = cboDocumento.Value ?? "1";
                    txtSerieDoc.Text = _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).Trim();
                    txtCorrelativoDocIni.Text = _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento, int.Parse(cboDocumento.Value.ToString())).ToString("00000000").Trim();
                    PredeterminarEstablecimiento(txtSerieDoc.Text);

                    txtConcepto.Text = "FACTURA ANULADA";

                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_IdUnidadMedida"].Value = "-1";
                    row.Cells["d_Cantidad"].Value = "1";
                    row.Cells["d_Precio"].Value = "1";
                    row.Cells["i_IdCentroCosto"].Value = "0";
                    row.Cells["v_Observaciones"].Value = "FACTURA ANULADA";
                    row.Cells["i_IdUnidadMedida"].Value = "-1";
                    row.Cells["i_Anticipio"].Value = "0";
                    row.Cells["d_Descuento"].Value = "0";
                    row.Cells["i_EsServicio"].Value = "1";
                    row.Cells["d_CantidadEmpaque"].Value = "1";
                    row.Cells["d_Valor"].Value = "0";
                    row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    row.Cells["v_NroCuenta"].Activate();
                    CalcularValoresDetalle();
                    grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown, true, true);
                }
            }

        }

        public bool VerificarLineaCreditoCliente(string IdCliente)
        {
            OperationResult objOperationResult = new OperationResult();
            decimal MontoOriginalVenta = 0;

            decimal MontoACargar = 0;

            MontoACargar = decimal.TryParse(txtTotal.Text.Trim(), out MontoACargar) ? MontoACargar : 0;
            MontoOriginalVenta = _ventaDto.d_Total != null ? _ventaDto.d_Total.Value : 0;

            var LineaCredito = new ClienteBL().DevuelveLineaCreditoCliente(ref objOperationResult, IdCliente);

            if (LineaCredito != null)
            {
                if ((LineaCredito.d_Saldo + MontoOriginalVenta) == 0 || (LineaCredito.d_Saldo + MontoOriginalVenta) < MontoACargar)
                {
                    UltraStatusbarManager.MarcarError(ultraStatusBar1, "El cliente no dispone de crédito suficiente para realizar esta compra.", timer1);
                    txtSerieDoc.Enabled = false; //se deshabilita la serie y correlativo porque al salir de sus focos tmb validan y pueden deshabilitar o habilitar el btnGuardar tambien
                    txtCorrelativoDocIni.Enabled = false;
                    btnGuardar.Enabled = false;
                    btnConsultarLineaCredito.Visible = true;
                    btnConsultarLineaCredito.Enabled = true;
                    return false;
                }
                else
                {
                    UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
                    txtSerieDoc.Enabled = true;
                    txtCorrelativoDocIni.Enabled = true;
                    btnGuardar.Enabled = true;
                    btnConsultarLineaCredito.Visible = false;
                    btnConsultarLineaCredito.Enabled = false;
                    return true;
                }
            }
            else
            {
                UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
                txtSerieDoc.Enabled = true;
                txtCorrelativoDocIni.Enabled = true;
                btnGuardar.Enabled = true;
                btnConsultarLineaCredito.Visible = false;
                btnConsultarLineaCredito.Enabled = false;
                return true;
            }
        }

        private void ultraExpandableGroupBox1_ExpandedStateChanging(object sender, CancelEventArgs e)
        {

        }

        private void btnConsultarLineaCredito_Click(object sender, EventArgs e)
        {

        }

        private void grdData_ClickCellButton(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "d_Precio")
            {
                string Id = grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null ? grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value.ToString() : string.Empty;
                int IdAlmacen = grdData.ActiveRow.Cells["i_IdAlmacen"].Value != null ? int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()) : -1;
                if (!string.IsNullOrEmpty(Id))
                {
                    var PopUp = new ucListaPreciosPopUp(ucListaPreciosPopUp.TipoVenta.VentaNormal, Id, IdAlmacen);
                    panelListaPrecios.ClientArea.Controls.Clear();
                    panelListaPrecios.ClientArea.Controls.Add(PopUp);
                    ultraPopupControlContainer1.Show();
                }
            }
        }

        public void FijarPrecio(string pstrPrecio, int IdMoneda)
        {
            if (grdData.ActiveRow != null)
            {
                //  grdData.ActiveRow.Cells["d_Precio"].Value = pstrPrecio;
                grdData.ActiveRow.Cells["d_Precio"].Value = cboMoneda.Value == null || decimal.Parse(txtTipoCambio.Text) <= 0 ? "0.0" : IdMoneda == int.Parse(cboMoneda.Value.ToString()) ? pstrPrecio : int.Parse(cboMoneda.Value.ToString()) == (int)Currency.Soles ? (Utils.Windows.DevuelveValorRedondeado(decimal.Parse(pstrPrecio) * decimal.Parse(txtTipoCambio.Text), 2).ToString()) : (Utils.Windows.DevuelveValorRedondeado(decimal.Parse(pstrPrecio) / decimal.Parse(txtTipoCambio.Text), 2).ToString());
                ultraPopupControlContainer1.Close();
                CalcularValoresFila(grdData.ActiveRow);
            }
        }

        private void txtGuiaRemisionCorrelativo_Validated(object sender, EventArgs e)
        {
            int EsNumeroEntero;
            if (int.TryParse(txtGuiaRemisionCorrelativo.Text.Trim(), out EsNumeroEntero))
            {
                Utils.Windows.FijarFormatoUltraTextBox(txtGuiaRemisionCorrelativo, "{0:00000000}");
            }
        }

        private void txtConcepto_KeyPress(object sender, KeyPressEventArgs e)
        {
            GlosaEditada = true;
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

        private void txtCodigoCliente_ValueChanged(object sender, EventArgs e)
        {

        }

        void FilaAnticipio(UltraGridRow fila)
        {
            try
            {
                if (fila == null) return;
                var activado = fila.Cells["i_Anticipio"].Value != null && fila.Cells["i_Anticipio"].Value.ToString() == "1";

                List<UltraGridCell> cellsCollection = new List<UltraGridCell> { 
                    fila.Cells["d_Cantidad"],
                    fila.Cells["i_IdAlmacen"],
                    fila.Cells["i_IdUnidadMedida"],
                    fila.Cells["d_Precio"],
                    fila.Cells["d_ValorVenta"],
                    fila.Cells["d_Igv"],
                    fila.Cells["d_Valor"],
                    fila.Cells["v_DescripcionProducto"],
                    fila.Cells["d_Descuento"],
                    fila.Cells["v_CodigoInterno"]
                };

                cellsCollection.ForEach(cell =>
                {
                    cell.Activation = activado ? Activation.Disabled : Activation.AllowEdit;
                });

                if (activado)
                {
                    fila.Cells["i_IdUnidadMedida"].Value = "5";
                    fila.Cells["d_PrecioVenta"].Activate();
                    grdData.PerformAction(UltraGridAction.EnterEditMode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void FilasAnticipioActivacion(bool activado)
        {
            var filas = grdData.Rows.ToList();
            filas.ForEach(f => f.Cells["i_Anticipio"].Activation = activado ? Activation.AllowEdit : Activation.Disabled);
        }

        private void frmBuscarFu_Click(object sender, EventArgs e)
        {
            if (cboDocumento.Value == null || cboDocumento.Value.ToString() == "-1")
            {
                cboDocumento.PerformAction(UltraComboAction.Dropdown);
                return;
            }

            OperationResult objOperationResult = new OperationResult();
            frmBandejaFormatoUnicoFacturacion frm = new frmBandejaFormatoUnicoFacturacion(Constants.TipoUsoFu);
            frm.ShowDialog();

            if (frm.v_IdFormatoUnicoFacturacion != null)
            {
                var CabeceraFuf = FormatoUnicoFacturacionBl.ObtenerCabecera(ref objOperationResult, frm.v_IdFormatoUnicoFacturacion);

                _ventaDto.v_IdCliente = CabeceraFuf.v_IdClienteFacturar;
                txtRazonSocial.Text = CabeceraFuf.NombreClienteFacturacion;
                txtRucCliente.Text = CabeceraFuf.NroDocumentoClienteFacturacion;
                txtNroFuf.Text = CabeceraFuf.v_NroFormato;
                txtDireccion.Text = CabeceraFuf.DireccionClienteFacturacion;
                txtNroFuf.Enabled = false;

                btnBuscarDetraccion.Enabled = false;

                btnBuscarDetraccion.Enabled = false;
                _ventaDto.v_IdFormatoUnicoFacturacion = CabeceraFuf.v_IdFormatoUnicoFacturacion;
                var DetallesFuf = FormatoUnicoFacturacionBl.ObtenerDetalleFUFparaVenta(ref objOperationResult, frm.v_IdFormatoUnicoFacturacion);
                if (cboDocumento.Value.ToString() != "438")
                {
                    DetallesFuf = new BindingList<nbs_formatounicofacturaciondetalleDto>(DetallesFuf.Where(p => p.i_FacturadoContabilidad == 0).ToList());
                }
                else
                {
                    DetallesFuf = new BindingList<nbs_formatounicofacturaciondetalleDto>(DetallesFuf.Where(p => p.i_FacturadoContabilidad == 1).ToList());
                }
                if (DetallesFuf.Count > 0)
                {
                    foreach (nbs_formatounicofacturaciondetalleDto Fila in DetallesFuf)
                    {
                        btnAgregar_Click(sender, e);

                        grdData.ActiveRow.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado;
                        grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = Fila.v_IdProductoDetalle;
                        grdData.ActiveRow.Cells["v_DescripcionProducto"].Value = Fila.DescripcionServicio;
                        grdData.ActiveRow.Cells["v_CodigoInterno"].Value = Fila.CodigoServicio;
                        grdData.ActiveRow.Cells["Empaque"].Value = "0";
                        grdData.ActiveRow.Cells["d_Cantidad"].Value = Fila.i_Cantidad.ToString();
                        grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = "-1";
                        grdData.ActiveRow.Cells["v_NroCuenta"].Value = Fila.NroCuenta;
                        grdData.ActiveRow.Cells["v_NroCuenta"].Value = Fila.NroCuenta;
                        grdData.ActiveRow.Cells["d_Precio"].Value = Fila.d_Importe.ToString();
                        grdData.ActiveRow.Cells["i_Eliminado"].Value = "0";
                        grdData.ActiveRow.Cells["i_InsertaIdUsuario"].Value = Fila.i_InsertaIdUsuario.ToString();
                        grdData.ActiveRow.Cells["t_InsertaFecha"].Value = Fila.t_InsertaFecha.Value.ToString();
                        grdData.ActiveRow.Cells["i_EsServicio"].Value = Fila.i_EsServicio.ToString();
                        grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = Fila.i_IdUnidadMedida.ToString();
                        grdData.ActiveRow.Cells["i_RegistroTipo"].Value = "Temporal";
                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = Fila.i_IdUnidadMedida.ToString();
                        CalcularValoresFila(grdData.ActiveRow);

                    }

                    btnAgregar.Enabled = false;
                    btnEliminar.Enabled = false;
                    cboDocumento.Enabled = false;

                }
            }
        }

        private void txtTipoKardex_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnBuscarKardex_Click(sender, e);
            }
        }

        private void btnBuscarKardex_Click(object sender, EventArgs e)
        {
            //if (txtTipoKardex.Text.Trim() == string.Empty) return;
            if (Loading) return;
            if (ValidarKardex.Validate(true, false).IsValid)
            {
                if (cboTipoKardex.Value != null && cboTipoKardex.Value.ToString() == "V")
                //if (txtTipoKardex.Text == "V")
                {
                    txtNroKardex.Visible = false;
                    lblNumeroKardex.Visible = false;

                    lblMonto.Visible = false;
                    txtMonto.Visible = false;

                    frmRegistroVentaKardex frm = new frmRegistroVentaKardex(_ventaDto.v_IdVenta, ListaGrilla, strModo, _TempDetalle_EliminarKardexDto);
                    frm.ShowDialog();
                    ListaGrilla = frm.ListaGrilla;
                    _TempDetalle_EliminarKardexDto = frm._TempDetalle_EliminarDto;
                    txtMonto.Text = frm.Total.ToString();
                    lblMonto.Visible = true;
                    txtMonto.Visible = true;

                }
                else if (cboTipoKardex.Value != null)
                {
                    txtNroKardex.Visible = true;
                    lblNumeroKardex.Visible = true;

                    lblMonto.Visible = true;
                    txtMonto.Visible = true;
                }

            }
            else
            {

                txtNroKardex.Visible = false;
                lblNumeroKardex.Visible = false;

                lblMonto.Visible = false;
                txtMonto.Visible = false;
            }

        }

        private void txtTipoKardex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'P' || e.KeyChar == 'p' || e.KeyChar == 'H' || e.KeyChar == 'h' || e.KeyChar == 'v' || e.KeyChar == 8 || e.KeyChar == 'E' || e.KeyChar == 'e' || e.KeyChar == 'M' || e.KeyChar == 'm' || e.KeyChar == 'K' || e.KeyChar == 'k' || e.KeyChar == 'R' || e.KeyChar == 'r')
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }

        }

        private void btnGuardarSinProcesar_Click(object sender, EventArgs e)
        {
            var resp = MessageBox.Show("¿Seguro de guardar sin procesar la venta?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == System.Windows.Forms.DialogResult.Yes)
            {
                _guardadoSinProceso = true;
                btnGuardar_Click(sender, e);
                btnGuardarSinProcesar.Enabled = false;
            }
        }

        private void txtMonto_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtMonto, e);
        }

        private void txtNroKardex_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtNroKardex, e);
            try
            {
                #region Obtiene la data de los DBFs de la notaría
                if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
                var dbfCon = new DbfConnector
                {
                    DataClientPath = NBS_DBF_PathSettings.Default.dbf_clientref_path,
                    DataPath = NBS_DBF_PathSettings.Default.dbf_data_path,
                    DataClientFileName = NBS_DBF_PathSettings.Default.dbf_clientref_filename,
                    DataFileName = NBS_DBF_PathSettings.Default.dbf_data_filename
                };
                dbfCon.ErrorEvent += dbfCon_ErrorEvent;

                // var data = dbfCon.ObtenerInfo(txtNroKardex.Text.Trim(), txtTipoKardex.Text.Trim());
                var data = dbfCon.ObtenerInfo(txtNroKardex.Text.Trim(), cboTipoKardex.Value.ToString());
                #endregion

                #region Registrar Cliente
                if (data == null) return;
                var f = new FrmClientesPorKardex(data);
                f.ShowDialog();
                if (f.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    _ventaDto.v_IdCliente = f.ClienteDto.v_IdCliente;
                    txtRucCliente.Text = f.ClienteDto.v_NroDocIdentificacion;
                    txtRazonSocial.Text = string.Join(" ", f.ClienteDto.v_ApePaterno.Trim(),
                        f.ClienteDto.v_ApeMaterno.Trim(), f.ClienteDto.v_PrimerNombre.Trim(),
                        f.ClienteDto.v_RazonSocial.Trim());

                    txtDireccion.Text = f.ClienteDto.v_DirecPrincipal;
                }
                else if (f.DialogResult == DialogResult.Abort)
                {
                    UltraStatusbarManager.MarcarError(ultraStatusBar1, f.ErrorMessage, timer1);
                    var objOperationResult = new OperationResult();
                    Mantenimientos.frmRegistroRapidoCliente frm = new Mantenimientos.frmRegistroRapidoCliente(f.RucCliente, "C");
                    frm.ShowDialog();
                    if (frm._Guardado)
                    {
                        txtRucCliente.Text = frm._NroDocumentoReturn;
                        var datosProveedor = _objVentasBL.DevolverClientePorNroDocumento(ref objOperationResult, txtRucCliente.Text.Trim());
                        if (datosProveedor != null)
                        {
                            _ventaDto.v_IdCliente = datosProveedor[0];
                            _ventaDto.i_IdDireccionCliente = int.Parse(datosProveedor[6]);
                            txtRazonSocial.Text = datosProveedor[2];
                            txtDireccion.Text = datosProveedor[3];
                            VerificarLineaCreditoCliente(_ventaDto.v_IdCliente);
                        }
                    }
                    txtRazonSocial.Enabled = false;
                    txtRucCliente.Enabled = true;
                    txtDireccion.Enabled = false;
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dbfCon_ErrorEvent(string error)
        {
            UltraStatusbarManager.MarcarError(ultraStatusBar1, error, timer1);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
        }

        private void cboTipoKardex_ValueChanged(object sender, EventArgs e)
        {
            if (CargarFormularioKardex)
            {
                btnBuscarKardex_Click(sender, e);
            }
            if (cboTipoKardex.Value != null && cboTipoKardex.Value.ToString() != "-1")
            {
                btnActualizarKardex.Visible = true;
            }

            txtMonto.Text = cboTipoKardex.Value != null && cboTipoKardex.Value.ToString() != "V" ? txtTotal.Text : txtMonto.Text;




        }

        #region Calculo ISC
        private readonly Dictionary<string, productoiscDto> _productoiscDtos = new Dictionary<string, productoiscDto>();

        private void AddIsc(string idProdDetalle)
        {
            if (_productoiscDtos.ContainsKey(idProdDetalle)) return;
            var objresult = new OperationResult();
            var productIsc = new ProductoIscBL().FromProductDetail(ref objresult, idProdDetalle, Globals.ClientSession.i_Periodo.ToString());
            if (productIsc != null)
                _productoiscDtos.Add(idProdDetalle, productIsc);
        }

        private decimal CalcularIsc(string idProdDetalle, decimal precioUnitario)
        {
            decimal valorIsc = 0;
            if (_productoiscDtos.ContainsKey(idProdDetalle))
            {
                var productIsc = _productoiscDtos[idProdDetalle];
                switch (productIsc.i_IdSistemaIsc)
                {
                    case 1:
                        precioUnitario = precioUnitario / (1M + productIsc.d_Porcentaje ?? 0);
                        valorIsc = precioUnitario * productIsc.d_Porcentaje ?? 0;
                        break;
                    case 2:
                        valorIsc = productIsc.d_Monto ?? 0;
                        break;
                    case 3:
                        valorIsc = (productIsc.d_Porcentaje * productIsc.d_Monto) ?? 0;
                        break;
                }
            }
            return valorIsc;
        }
        #endregion

        private void txtNroKardex_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtNroKardex, "{0:000000}");
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnActualizarKardex_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtMonto.Text.Trim()) && decimal.Parse(txtMonto.Text.Trim()) == 0)
            {
                txtMonto.Text = txtTotal.Text.Trim();
            }

            EditarTodo = false;
            OperationResult objOperationResult = new OperationResult();
            LlenarTemporalesKardex_();
            _ventaDto.v_IdTipoKardex = cboTipoKardex.Value == null || cboTipoKardex.Value.ToString() == "-1" ? "" : cboTipoKardex.Value.ToString();
            _objVentasBL.ActualizarVenta(ref objOperationResult, _ventaDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto, _TempDetalle_AgregarKardexDto, _TempDetalle_ModificarKardexDto, _TempDetalle_EliminarKardexDto, _guardadoSinProceso, false);
            var objDbfSincro = new DbfSincronizador();
            var objOperationResult2 = new OperationResult();
            _ventaDto.v_IdVenta = _IdVentaGuardada;
            objDbfSincro.RutaDbfCabecera = NBS_DBF_PathSettings.Default.dbfSincro_Cabecera;
            objDbfSincro.RutaDbfDetalle = NBS_DBF_PathSettings.Default.dbfSincro_Detalle;
            objDbfSincro.ActualizarDatosVenta(ref objOperationResult2, _ventaDto.v_IdVenta, DbfSincronizador.TipoAccion.Venta);
            if (objOperationResult2.Success == 0)
            {
                MessageBox.Show(objOperationResult2.ErrorMessage, @"Error al sincronizar DBF", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            if (objOperationResult.Success == 1)
            {
                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                {
                    strModo = "Guardado";
                    EdicionBarraNavegacion(true);
                    strIdVenta = _ventaDto.v_IdVenta;
                    BtnImprimir.Enabled = _btnImprimir;
                    CargarCabecera(_IdVentaGuardada);
                    _pstrIdMovimiento_Nuevo = _ventaDto.v_IdVenta;
                    _idVenta = _pstrIdMovimiento_Nuevo;
                    ListaGrilla = new BindingList<nbs_ventakardexDto>();
                    EditarTodo = true;

                }
                UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
    }
}

