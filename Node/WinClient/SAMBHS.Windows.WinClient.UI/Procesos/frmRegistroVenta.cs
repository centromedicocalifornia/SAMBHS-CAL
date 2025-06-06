﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Security.BL;
using SAMBHS.Contabilidad.BL;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;
using Infragistics.Win;
using LoadingClass;
using SAMBHS.Windows.WinClient.UI.Mantenimientos.UserControls;
using SAMBHS.Cobranza.BL;
using SAMBHS.Windows.NubefactIntegration;
using SAMBHS.Windows.WinClient.UI.Reportes.Ventas;

using SAMBHS.Windows.WinClient.UI.Reportes;
using System.IO;
using System.Text;
using SAMBHS.Windows.NubefactIntegration.Modelos;
using SAMBHS.Windows.SigesoftIntegration.UI.BLL;
using NetPdf;
using System.Configuration;
using Infragistics.Win.UltraWinEditors;
using SAMBHS.Windows.SigesoftIntegration.UI;
using SAMBHS.Common.BE.Custom;
using System.Data.SqlClient;


namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmRegistroVenta : Form
    {
        #region Declaraciones / Referencias

        SecurityBL _objSecurityBL = new SecurityBL();
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
        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        ventaDto _ventaDto = new ventaDto();
        movimientoDto _movimientoDto = new movimientoDto();
        ventadetalleDto _ventadetalleDto = new ventadetalleDto();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        List<KeyValueDTO> _ListadoVentas = new List<KeyValueDTO>();
        List<string> idVentasImpresion = new List<string>();
        bool GlosaEditada;
        bool impresionVistaPrevia;
        private string centroDeCosto;

        #endregion
        private bool EsDocumentoElectronico
        {
            get
            {
                int i;
                return !int.TryParse(txtSerieDoc.Text, out i);
            }
        }

        int _MaxV, _ActV;
        string _Mode, _IdVentaGuardada;
        public string _pstrIdMovimiento_Nuevo;
        string strModo = "Nuevo", strIdVenta;
        private readonly string _utilizado;
        private bool _EsNotadeCredito;
        public bool _modificada, Extraccion = false, ExtraccionPedido = false;
        string _idVenta, CtaRubroMercaderia = "-1", _ctaRubroServicio = "-1";
        decimal Redondeado, Residuo;
        bool _Redondeado;
        public int IdIdentificacion = 0;
        private List<string> listaGuiasRemisionPorAnular = new List<string>();
        bool EliminarVentas;
        private decimal valorOriginalCelda;
        private int _limiteDocumento;
        private string _idPedidoExtraccion;
        int ValidarStockAlmacen;
        AlmacenBL _objAlmacenBL = new AlmacenBL();
        bool enviandoNubefact = false;
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

        #region Temporales Detalles de Venta

        List<ventadetalleDto> _TempDetalle_AgregarDto = new List<ventadetalleDto>();
        List<ventadetalleDto> _TempDetalle_ModificarDto = new List<ventadetalleDto>();
        List<ventadetalleDto> _TempDetalle_EliminarDto = new List<ventadetalleDto>();

        #endregion

        #region Permisos Botones

        bool _btnGuardar;
        bool _btnImprimir;

        #endregion


        #region GetChanges

        public class Campos
        {
            public string NombreCampo { get; set; }
            public string ValorCampo { get; set; }
        }

        string[] nombreCampos =
        {

            "cboCondicionPago", "cboMoneda", "cboVendedor", "cboTipoVenta", "dtpFechaRegistro", 
            "txtTipoCambio", "txtNroDias", "dtpFechaVencimiento", "rbPercepcionSI", "rbPercepcionNO", 
            "rbPercepcionAgente", "rbPercepcioNoagente", "txtPercepcionPorcentaje", "txtConcepto", 
            "txtPlacaVehiculo", "cboEstado", "chkAfectoIGV", "chkPrecInIGV", "chkDeduccionAnticipio", 
            "chkEsGratuito", "cboTipoOperacion", "txtPorcDescuento", "txtPorcComision", "cboVendedorRef", 
            "chkAnticipo", "cboEstablecimiento", "ucTipoNota", "txtValorFOB", "txtNroPedido", "txtGuiaRemisionSerie", 
            "txtGuiaRemisionCorrelativo", "txtOrdenCompra", "dtpFechaOrden", 
            "cboPuntoEmbarque", "cboPuntoDestino", 
            "cboTipoEmbarque", "chkDrawnBack", "txtNroBulto", "txtNroBultoIngles", "txtMarca", "txtDimensiones", 
            "txtPesoBrutoKg", "txtPesoNetoKg", "txtBanco", "txtCantidadTotal", "cboMVenta", "txtNroBL", "dtpFechaBL", 
            "txtContenedor", "txtNaviera"
        };

        private List<Campos> ListValuesCampo = new List<Campos>();

        private string GetChanges()
        {
            string cadena = new UpdateCommentaryVentaBL().GetCommentaryUpdateVentaId(_IdVentaGuardada);
            string oldComentary = cadena;
            cadena += "<FechaActualiza:" + DateTime.Now.ToString() + "|UsuarioActualiza:" + Globals.ClientSession.v_UserName + "|";
            bool change = false;
            foreach (var item in nombreCampos)
            {
                var fields = this.Controls.Find(item, true);
                string keyTagControl;
                string value1;

                if (fields.Length > 0)
                {
                    keyTagControl = fields[0].GetType().Name;
                    value1 = GetValueControl(keyTagControl, fields[0]);

                    var ValorCampo = ListValuesCampo.Find(x => x.NombreCampo == item).ValorCampo;
                    if (ValorCampo != value1)
                    {
                        cadena += item + ":" + ValorCampo + "|";
                        change = true;
                    }
                }
            }
            if (change)
            {
                return cadena;
            }

            return oldComentary;
        }

        private void SetOldValues()
        {
            ListValuesCampo = new List<Campos>();
            string keyTagControl = null;
            string value1 = null;
            foreach (var item in nombreCampos)
            {

                var fields = this.Controls.Find(item, true);

                if (fields.Length > 0)
                {
                    keyTagControl = fields[0].GetType().Name;
                    value1 = GetValueControl(keyTagControl, fields[0]);

                    Campos _Campo = new Campos();
                    _Campo.NombreCampo = item;
                    _Campo.ValorCampo = value1;
                    ListValuesCampo.Add(_Campo);
                }
            }
        }

        private string GetValueControl(string ControlId, Control ctrl)
        {
            string value1 = null;

            switch (ControlId)
            {
                case "TextBox":
                    value1 = ((TextBox)ctrl).Text;
                    break;
                case "ComboBox":
                    value1 = ((ComboBox)ctrl).Text;
                    break;
                case "CheckBox":
                    value1 = Convert.ToInt32(((CheckBox)ctrl).Checked).ToString();
                    break;
                case "RadioButton":
                    value1 = Convert.ToInt32(((RadioButton)ctrl).Checked).ToString();
                    break;
                case "DateTimePicker":
                    value1 = ((DateTimePicker)ctrl).Text; ;
                    break;
                case "UltraCombo":
                    value1 = ((UltraCombo)ctrl).Text; ;
                    break;
                case "UltraComboEditor":
                    value1 = ((UltraComboEditor)ctrl).Text; 
                    break;
                case "UltraTextEditor":
                    value1 = ((UltraTextEditor)ctrl).Text;
                    break;
                default:
                    break;
            }

            return value1;
        }

        #endregion
        


        public frmRegistroVenta(string Modo, string IdVenta, string UtilizadoEn = null)
        {
            strModo = Modo;
            _IdVentaGuardada = strIdVenta = IdVenta;
            _utilizado = UtilizadoEn;
            _modificada = false;
            InitializeComponent();
        }

        private void frmRegistroVenta_Load(object sender, EventArgs e)
        {
            Utils.Windows.SetearLimiteFechas(new[] { dtpFechaRegistro });
            dtpFechaRegistro.MinDate = new DateTime(1999, 1, 1);
            using (new PleaseWait(Location, "Por favor espere..."))
            {
                UltraStatusbarManager.Inicializar(ultraStatusBar1);
                var objOperationResult = new OperationResult();

                #region ControlAcciones

                if (
                    _objCierreMensualBL.VerificarMesCerrado(Globals.ClientSession.i_Periodo.ToString().Trim(),
                        DateTime.Now.Month.ToString("00").Trim(), (int)ModulosSistema.VentasFacturacion) || _utilizado == "KARDEX")
                {
                    btnGuardar.Visible = false;

                    this.Text = _utilizado == "KARDEX" ? "Registro de Venta" : "Registro de Venta [MES CERRADO]";
                    if (_utilizado == "KARDEX")
                    {
                        btnSalir.Visible = false;
                        btnAgregar.Visible = false;
                        btnEliminar.Visible = false;
                        BtnImprimir.Visible = false;

                    }
                }
                else
                {

                    btnGuardar.Visible = true;
                    Text = @"Registro de Venta";
                }
                var _formActions = _objSecurityBL.GetFormAction(ref objOperationResult,
                    Globals.ClientSession.i_CurrentExecutionNodeId, Globals.ClientSession.i_SystemUserId,
                    "frmRegistroVenta", Globals.ClientSession.i_RoleId);

                _btnImprimir = Utils.Windows.IsActionEnabled("frmRegistroVenta_PRINT", _formActions);
                _btnGuardar = Utils.Windows.IsActionEnabled("frmRegistroVenta_SAVE", _formActions);
                _btnImprimir = true;
                _btnGuardar = true;

                btnGuardar.Enabled = _btnGuardar;

                #endregion

                BackColor = new GlobalFormColors().FormColor;
                panel1.Appearance.BackColor = new GlobalFormColors().BannerColor;
                panelPercepcion.BackColor = panelEmbarque.BackColor = new GlobalFormColors().FormColor;
                txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
                txtMes.Text = DateTime.Now.Month.ToString("00");

                #region Cargar Combos

                cboDocumento.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
                _ListadoComboDocumentos = new DocumentoBL().ObtenDocumentosParaComboGrid(ref objOperationResult, null, 0, 1);
                Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", _ListadoComboDocumentos,
                    DropDownListAction.Select);
                Utils.Windows.LoadUltraComboList(cboDocumentoRef, "Value2", "Id", _ListadoComboDocumentos.Where(x => !x.EsDocInterno).ToList(),
                    DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda,
                    DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboEstado, "Value1", "Id", Globals.CacheCombosVentaDto.cboEstado,
                    DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboCondicionPago, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboCondicionPago, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboEstablecimiento, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboIGV, "Value1", "Id", Globals.CacheCombosVentaDto.cboIGV,
                    DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboTipoVenta, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboTipoVenta, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboMVenta, "Value1", "Id", Globals.CacheCombosVentaDto.cboMVenta,
                    DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboPuntoDestino, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboPuntoDestino, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboPuntoEmbarque, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboPuntoEmbarque, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboTipoEmbarque, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboTipoEmbarque, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboTipoOperacion, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboTipoOperacion, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboVendedor, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboVendedorRef, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboVendedorRef, DropDownListAction.Select);
                CargarCombosDetalle();

                #endregion

                ConfigurarGrilla();

                cboDocumento.Value = "-1";
                cboDocumentoRef.Value = "-1";
                ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);
                FormatoDecimalesGrilla(Globals.ClientSession.i_CantidadDecimales ?? 2,
                    Globals.ClientSession.i_PrecioDecimales ?? 2);
                LlenarCuentasRubros();

                #region txtRucCLiente

                txtRucCliente.LoadConfig("C");
                txtRucCliente.ItemSelectedAfterDropClosed += delegate
                {
                    txtRucCliente_KeyDown_1(null, new KeyEventArgs(Keys.Enter));
                };
                txtRucCliente.TextChanged += delegate
                {
                    lblRevisionRUC.Text = string.Empty;
                };

                #endregion

                centroDeCosto = _objVentasBL.CentroCostoDeEstablecimiento(ref objOperationResult);
            }

            Activate();
            FilasAnticipioActivacion(chkDeduccionAnticipio.Checked);
            if (cboDocumento.Value != null && txtSerieDoc.Text.Trim() != string.Empty)
                _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()),
                    txtSerieDoc.Text.Trim());
            cboEstado.Enabled = VentaBL.TienePermisoAnular;
            btnAgregar.Enabled = !ExtraccionPedido && string.IsNullOrEmpty(txtNroPedido.Text);
            SetOldValues();
        }


        private void ConfigurarGrilla()
        {
            #region Configurar Grilla

            if (Globals.ClientSession.i_VentasMostrarEmpaque == null ||
                Globals.ClientSession.i_VentasMostrarEmpaque == 0)
            {
                grdData.DisplayLayout.Bands[0].Columns["Empaque"].Hidden = true;
                grdData.DisplayLayout.Bands[0].Columns["UMEmpaque"].Hidden = true;
            }

            if (Globals.ClientSession.i_VentasIscOtrosTributos == null ||
                Globals.ClientSession.i_VentasIscOtrosTributos.Value == 0)
            {
                grdData.DisplayLayout.Bands[0].Columns["d_isc"].Hidden = true;
                grdData.DisplayLayout.Bands[0].Columns["d_otrostributos"].Hidden = true;
            }
            if (Globals.ClientSession.i_IncluirNingunoCompraVenta == 1 && Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic)
            {
                grdData.DisplayLayout.Bands[0].Columns["v_PedidoExportacion"].Hidden = false;
                grdData.DisplayLayout.Bands[0].Columns["v_PedidoExportacion"].CellActivation = Activation.AllowEdit;
                grdData.DisplayLayout.Bands[0].Columns["v_PedidoExportacion"].CellClickAction = CellClickAction.Edit;
            }
            else
            {
                // grdData.DisplayLayout.Bands[0].Columns["v_PedidoExportacion"].Hidden = Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta != 1;

                grdData.DisplayLayout.Bands[0].Columns["v_PedidoExportacion"].Hidden = Globals.ClientSession.i_IncluirNingunoCompraVenta == 1 && Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic ? false : Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta != 1;

                grdData.DisplayLayout.Bands[0].Columns["v_PedidoExportacion"].CellActivation = Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta == 1 ? Activation.AllowEdit : Activation.ActivateOnly;
                grdData.DisplayLayout.Bands[0].Columns["v_PedidoExportacion"].CellClickAction = Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta == 1 ? CellClickAction.Edit : CellClickAction.RowSelect;


            }




            var activarUm = Globals.ClientSession.i_CambiarUnidadMedidaVentaPedido == 1;
            grdData.DisplayLayout.Bands[0].Columns["i_IdUnidadMedida"].CellActivation = activarUm
                ? Activation.AllowEdit
                : Activation.ActivateOnly;
            grdData.DisplayLayout.Bands[0].Columns["i_IdUnidadMedida"].CellClickAction = activarUm
                ? CellClickAction.Edit
                : CellClickAction.RowSelect;

            grdData.DisplayLayout.Bands[0].Columns["v_Observaciones"].CellActivation = Globals.ClientSession.v_RucEmpresa != Constants.RucAgrofergic ? Activation.AllowEdit : Activation.ActivateOnly;
            UltraGridColumn v_Observaciones = grdData.DisplayLayout.Bands[0].Columns["v_Observaciones"];
            v_Observaciones.Header.Caption = Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic ? "Anexos" : "Observaciones";
            v_Observaciones.MaxLength = Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic ? 3000 : 100;

            #endregion

        }
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (decimal.Parse(txtTipoCambio.Text.Trim()) <= 0)
            {
                UltraMessageBox.Show("Por favor ingrese un tipo de cambio válido", "Sistema", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                txtTipoCambio.Focus();
                return;
            }

            if (cboIGV.Value.ToString() == "-1")
            {
                UltraMessageBox.Show("Por favor seleccione un valor para el IGV", "Sistema", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                cboIGV.Focus();
                return;
            }

            if (
                grdData.Rows.Count(
                    p =>
                        p.Cells["v_IdProductoDetalle"].Value != null &&
                        p.Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000") != 0)
            {
                UltraMessageBox.Show("Antes de continuar por favor elimine el redondeo", "Sistema", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            var ultimaFila = grdData.Rows.LastOrDefault();

            if (ultimaFila == null ||
                (ultimaFila.Cells["i_Anticipio"].Value != null &&
                 ultimaFila.Cells["i_Anticipio"].Value.ToString() == "1") ||
                ultimaFila.Cells["v_IdProductoDetalle"].Value != null)
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
                row.Cells["d_isc"].Value = "0";
                row.Cells["d_otrostributos"].Value = "0";
                row.Cells["d_Descuento"].Value = "0";
                row.Cells["d_Valor"].Value = "0";
                row.Cells["v_NroCuenta"].Value = "-1";
                row.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null &&
                                                       cboTipoOperacion.Value.ToString() != "5"
                    ? cboTipoOperacion.Value.ToString()
                    : "1";
                row.Cells["i_IdCentroCosto"].Value = centroDeCosto;
                row.Cells["d_Percepcion"].Value = "0";
                row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                row.Cells["i_Anticipio"].Activation = chkDeduccionAnticipio.Checked
                    ? Activation.AllowEdit
                    : Activation.Disabled;
                row.Cells["t_FechaCaducidad"].Value = (DateTime?)null;
            }

            var aCell = grdData.ActiveRow.Cells["v_CodigoInterno"];
            grdData.Focus();
            grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
            grdData.ActiveCell = aCell;
            grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCorrelativo.Text))
            {
                UltraMessageBox.Show("No se obtuvo el Nro. de registro de la venta de forma correcta", "Advertencia");
                return;
            }

            if (cboDocumento.Value.ToString().Equals("1") && IdIdentificacion == 1)
            {
                UltraMessageBox.Show("En Factura no se puede elegir este cliente con DNI", "Error de validación.",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            // Cuando es CIF validar si se bajo el seguro
            if (cboMVenta.Value != null && cboMVenta.Value.ToString() == "3" || cboMVenta.Value != null && cboMVenta.Value.ToString() == "2")
            {
                var filasFlete =
                        grdData.Rows.Where(
                            fila =>
                                   fila.Cells["v_IdProductoDetalle"].Value != null && fila.Cells["v_IdProductoDetalle"].Value.ToString() == Globals.ClientSession.v_IdProductoDetalleFlete).ToList();


                if (!filasFlete.Any())
                {
                    UltraMessageBox.Show("Es necesario registrar el Flete para la Venta", "Error de validación.",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (cboMVenta.Value != null && cboMVenta.Value.ToString() == "3")
            {
                var filasSeguro =
                       grdData.Rows.Where(
                           fila =>
                            fila.Cells["v_IdProductoDetalle"].Value != null && fila.Cells["v_IdProductoDetalle"].Value.ToString() == Globals.ClientSession.v_IdProductoDetalleSeguro).ToList();

                if (!filasSeguro.Any())
                {
                    UltraMessageBox.Show("Es necesario registrar el Seguro para la Venta", "Error de validación.",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (_Mode == "New" &&
                grdData.Rows.Any(
                    fila =>
                        fila.Cells["v_NroCuenta"].Value != null && fila.Cells["v_NroCuenta"].Value.ToString() != "-1") &&
                grdData.Rows.Any(
                    fila =>
                        fila.Cells["v_NroCuenta"].Value == null || fila.Cells["v_NroCuenta"].Value.ToString() == "-1"))
            {
                var filasVacias =
                    grdData.Rows.Where(
                        fila =>
                            fila.Cells["v_NroCuenta"].Value == null ||
                            fila.Cells["v_NroCuenta"].Value.ToString() == "-1").ToList();
                filasVacias.ForEach(fila => fila.Delete(false));
            }

            if (cboDocumento.Value != null && (cboDocumento.Value.ToString() == "7" || cboDocumento.Value.ToString() == "1") && _ventaDto.v_IdCliente == "N002-CL000000000")
            {

                UltraMessageBox.Show("Este tipo documento no puede estar asociado a Público general", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var objOperationResult = new OperationResult();

            if (uvDatos.Validate(true, false).IsValid)
            {
                if (grdData.Rows.Count(x => x.Cells["v_NroCuenta"].Value == null) > 0 ||
                    grdData.Rows.Any(x => x.Cells["v_NroCuenta"].Value.ToString() == "-1"))
                {
                    UltraMessageBox.Show("Porfavor ingrese correctamente los Rubros", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    grdData.Rows.First(
                        x => x.Cells["v_NroCuenta"].Value == null || x.Cells["v_NroCuenta"].Value.ToString() == "-1")
                        .Selected = true;
                    return;
                }

                if (_Mode != "New")
                {
                    var ventaComparativa = new ventaDto
                    {
                        v_IdVenta = _idVenta,
                        i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString()),
                        v_SerieDocumento = txtSerieDoc.Text,
                        v_CorrelativoDocumento = txtCorrelativoDocIni.Text,
                        i_IdEstado = int.Parse(cboEstado.Value.ToString()),
                        d_Total = decimal.Parse(txtTotal.Text)
                    };

                    new VentaBL().ReGuardadoValido(ref objOperationResult, ventaComparativa);
                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(objOperationResult.ErrorMessage, @"Error de validación", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    var guiasRemision =
                        _objVentasBL.ObtenerDetalleGuiaRemisionporDocumentoRef(
                            int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim(),
                            txtCorrelativoDocIni.Text.Trim());
                    if (guiasRemision.Any())
                    {
                        var prodVenta =
                            grdData.Rows.Where(
                                p =>
                                    p.Cells["i_RegistroEstado"].Value != null &&
                                    p.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                                .Select(p => p.Cells["v_IdProductoDetalle"].Value.ToString())
                                .ToList();

                        if (prodVenta.Any() || _TempDetalle_EliminarDto.Any() || guiasRemision.FirstOrDefault().v_IdMovimientoDetalle != _ventaDto.v_IdCliente)
                        {
                            UltraMessageBox.Show(
                                "Imposible Guardar Cambios a un Documento con Guia de Remisión \n" +
                                "Nro. Guia(s):" + string.Join(",", guiasRemision.Select(o => o.v_Observacion)), "Advertencia",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }
                else

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

                if (EsVentaAfectaDetraccion())
                {
                    if (
                        UltraMessageBox.Show("El documento es Afecto Detracción, ¿Desea continuar?", "Advertencia",
                            MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        return;
                    }
                }

                #region Validaciones Generales

                if (int.Parse(cboEstado.Value.ToString()) == 1)
                {
                    if (_ventaDto.i_IdEstado == 0)
                    {
                        UltraMessageBox.Show("No es posible Activar un documento Anulado anteriormente", "Aviso",
                            MessageBoxButtons.OK);
                        return;
                    }
                }

                if (cboDocumento.Value.ToString() != "-1")
                {
                    var id = (string)cboDocumento.Value;
                    if ((id.Equals("7") || id.Equals("8")) && (ucTipoNota.Value == null || (string)ucTipoNota.Value == "-1"))
                    {
                        UltraMessageBox.Show("Seleccione un Tipo de Nota");
                        ucTipoNota.Focus();
                        return;
                    }

                    if (cboEstado.Value.ToString() != "0" &&
                        (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) ||
                         cboDocumento.Value.ToString() == "8"))
                    {
                        if (cboDocumentoRef.Value.ToString() == "-1" || txtSerieDocRef.Text.Trim() == "" ||
                            txtCorrelativoDocRef.Text.Trim() == "")
                        {
                            UltraMessageBox.Show("Por favor ingrese un documento de referencia válido", "Sistema",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            cboDocumentoRef.Focus();
                            return;
                        }
                    }
                }
                else
                {
                    UltraMessageBox.Show("Por favor ingrese un documento válido", "Sistema", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    cboDocumento.Focus();
                    return;
                }

                if (decimal.Parse(txtTipoCambio.Text.Trim()) <= 0)
                {
                    UltraMessageBox.Show("Por favor ingrese un tipo de cambio válido", "Sistema", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }
                decimal TipoCambioFecha = decimal.Parse(_objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date));

                if (TipoCambioFecha != decimal.Parse(txtTipoCambio.Text) && TipoCambioFecha > 0)
                {
                    if (UltraMessageBox.Show("El tipo de cambio es diferente al de la Fecha Registro ¿Desea continuar?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                }
                if (
                    grdData.Rows.Any(
                        p =>
                            p.Cells["v_IdProductoDetalle"].Value == null &&
                            (!string.IsNullOrEmpty(p.Cells["v_NroCuenta"].Text) &&
                             !p.Cells["v_NroCuenta"].Text.Contains("ANTICIPIO"))) &&
                    !_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())))
                {
                    if (cboEstado.Value.ToString() != "0")
                    {
                        UltraMessageBox.Show("Uno de los artículos esta incorrectamente ingresado", "Sistema",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        grdData.Rows.First(p => p.Cells["v_IdProductoDetalle"].Value == null).Selected = true;
                        return;
                    }
                }

                if (!_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) &&
                    (!grdData.Rows.Any() ||
                     grdData.Rows.Count(
                         p =>
                             p.Cells["v_IdProductoDetalle"].Value != null &&
                             p.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000") == 0))
                {
                    if (cboEstado.Value.ToString() != "0")
                    {
                        UltraMessageBox.Show("No se permite guardar mientras el detalle está vacío",
                            "Error de Validación");
                        return;
                    }
                }
                else
                {
                    if (ValidaCamposNulosVacios())
                    {
                        foreach (var fila in grdData.Rows)
                        {
                            var obj = (ventadetalleDto)fila.ListObject;
                            if (false && string.IsNullOrEmpty(obj.i_IdCentroCosto))
                            {
                                UltraMessageBox.Show("Necista configurar un centro de costo para el establecimiento",
                                    "Sistema", Icono: MessageBoxIcon.Error);
                                return;
                            }
                            if (obj.v_NroCuenta == null)
                            {
                                UltraMessageBox.Show("Por favor ingrese correctamente todas las cuentas al detalle.",
                                    "Sistema", Icono: MessageBoxIcon.Error);
                                return;
                            }

                            if (obj.EsRedondeo == null && obj.d_PrecioVenta < 0)
                            {
                                UltraMessageBox.Show("Todos los Totales no están calculados.", "Sistema",
                                    Icono: MessageBoxIcon.Error);
                                fila.Activate();
                                return;
                            }

                            if (chkEsGratuito.Checked && obj.i_IdTipoOperacion < 10)
                            {
                                UltraMessageBox.Show("Una Venta Gratuita no puede tener Operaciones Onerosas", "Sistema",
                                    Icono: MessageBoxIcon.Error);
                                return;
                            }
                            if (fila.Cells["i_IdAlmacen"].Value == null &&
                                fila.Cells["v_IdProductoDetalle"].Value != null)
                            {
                                UltraMessageBox.Show("Porfavor especifique los Almacenes correctamente", "Sistema",
                                    Icono: MessageBoxIcon.Error);
                                return;
                            }
                        }

                        #region ValidarStock
                        if (!_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString()))
                                            && cboDocumento.Value.ToString() != "8" && cboEstado.Value.ToString() == "1")
                        {
                            var detalles = grdData.Rows.Select(r => (ventadetalleDto)r.ListObject).ToList();
                            var resultado = new VentaBL().VerificarStockDisponible(ref objOperationResult, detalles, _idPedidoExtraccion);
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

                        if (Residuo != 0 && grdData.Rows.Count() != 0 &&
                            grdData.Rows.Count(
                                p =>
                                    p.Cells["v_IdProductoDetalle"].Value != null &&
                                    p.Cells["v_IdProductoDetalle"].Value.ToString() == "N002-PE000000000") != 0)
                        {
                            UltraMessageBox.Show("Por favor elimine el anterior redondeo para continuar.", "Sistema",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    using (new PleaseWait(this.Location, "Por favor espere..."))
                    {
                        int i;
                        decimal d;
                        while (_objVentasBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text) ==
                               false)
                        {
                            txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                        }

                        while (
                            _objVentasBL.ExisteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text,
                                txtCorrelativoDocIni.Text) == false)
                        {
                            txtCorrelativoDocIni.Text = (int.Parse(txtCorrelativoDocIni.Text) + 1).ToString("00000000");
                        }

                        #region Guarda Entidad Venta

                        _ventaDto.i_IdMoneda = int.TryParse(cboMoneda.Value.ToString(), out i) ? i : -1;
                        _ventaDto.v_Mes = (int.TryParse(txtMes.Text.Trim(), out i) ? i : 1).ToString("00");
                        _ventaDto.v_Periodo = txtPeriodo.Text.Trim();
                        _ventaDto.v_Correlativo = txtCorrelativo.Text;
                        _ventaDto.d_Anticipio = txtAnticipio.Text != string.Empty ? decimal.Parse(txtAnticipio.Text) : 0;
                        _ventaDto.d_IGV = txtIGV.Text != string.Empty ? decimal.Parse(txtIGV.Text) : 0;
                        _ventaDto.d_TipoCambio = txtTipoCambio.Text != string.Empty
                            ? decimal.Parse(txtTipoCambio.Text)
                            : 0;
                        _ventaDto.d_Total = txtTotal.Text != string.Empty ? decimal.Parse(txtTotal.Text) : 0;
                        _ventaDto.d_Valor = txtValor.Text != string.Empty ? decimal.Parse(txtValor.Text) : 0;
                        _ventaDto.d_ValorVenta = txtValorVenta.Text != string.Empty
                            ? decimal.Parse(txtValorVenta.Text)
                            : 0;
                        _ventaDto.i_DeduccionAnticipio = chkDeduccionAnticipio.Checked ? 1 : 0;
                        _ventaDto.i_EsAfectoIgv = chkAfectoIGV.Checked ? 1 : 0;
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
                        _ventaDto.i_PreciosIncluyenIgv = chkPrecInIGV.Checked ? 1 : 0;
                        _ventaDto.v_CorrelativoDocumentoRef = txtCorrelativoDocRef.Text;
                        _ventaDto.v_Mes = txtMes.Text;
                        _ventaDto.v_Periodo = txtPeriodo.Text;
                        _ventaDto.v_SerieDocumento = txtSerieDoc.Text.Trim();
                        _ventaDto.v_CorrelativoDocumento = txtCorrelativoDocIni.Text.Trim();
                        _ventaDto.v_CorrelativoDocumentoFin = txtCorrelativoDocFin.Text.Trim();
                        _ventaDto.v_Concepto = txtConcepto.Text.Trim();
                        _ventaDto.v_SerieDocumentoRef = txtSerieDocRef.Text.Trim();
                        _ventaDto.d_PorcDescuento = txtPorcDescuento.Text == string.Empty
                            ? 0
                            : decimal.Parse(txtPorcDescuento.Text);
                        _ventaDto.d_PocComision = txtPorcComision.Text == string.Empty
                            ? 0
                            : decimal.Parse(txtPorcComision.Text);
                        _ventaDto.d_Descuento = txtDescuento.Text == string.Empty ? 0 : decimal.Parse(txtDescuento.Text);
                        _ventaDto.v_BultoDimensiones = txtDimensiones.Text;
                        _ventaDto.v_NroGuiaRemisionCorrelativo = txtGuiaRemisionCorrelativo.Text.Trim();
                        _ventaDto.v_NroGuiaRemisionSerie = txtGuiaRemisionSerie.Text.Trim();
                        _ventaDto.d_IGV = txtIGV.Text == string.Empty ? 0 : decimal.Parse(txtIGV.Text.Trim());
                        _ventaDto.v_Marca = txtMarca.Text.Trim();
                        _ventaDto.v_NroBulto = txtNroBulto.Text.Trim();
                        _ventaDto.i_NroDias = int.TryParse(txtNroDias.Text.Trim(), out i) ? i : 0;
                        _ventaDto.v_NroPedido = txtNroPedido.Text;
                        _ventaDto.v_OrdenCompra = txtOrdenCompra.Text.Trim();
                        _ventaDto.d_PesoBrutoKG = decimal.TryParse(txtPesoBrutoKg.Text.Trim(), out d) ? d : 0;
                        _ventaDto.d_PesoNetoKG = decimal.TryParse(txtPesoNetoKg.Text.Trim(), out d) ? d : 0;
                        _ventaDto.t_FechaOrdenCompra = dtpFechaOrden.Value;
                        _ventaDto.i_IdMedioPagoVenta = cboMVenta.Value != null ? int.Parse(cboMVenta.Value.ToString()) : -1;
                        _ventaDto.i_IdPuntoDestino = cboPuntoDestino.Value != null ? int.Parse(cboPuntoDestino.Value.ToString()) : -1;
                        _ventaDto.i_IdPuntoEmbarque = cboPuntoEmbarque.Value != null ? int.Parse(cboPuntoEmbarque.Value.ToString()) : -1;
                        _ventaDto.i_IdTipoEmbarque = cboTipoEmbarque.Value != null ? int.Parse(cboTipoEmbarque.Value.ToString()) : -1;
                        _ventaDto.i_IdTipoOperacion = cboTipoOperacion.Value != null ? int.Parse(cboTipoOperacion.Value.ToString()) : -1;
                        _ventaDto.i_IdTipoVenta = cboTipoVenta.Value != null ? int.Parse(cboTipoVenta.Value.ToString()) : -1;
                        _ventaDto.i_DrawBack = chkDrawnBack.Checked ? 1 : 0;
                        _ventaDto.v_IdVendedor = cboVendedor.Value.ToString();
                        _ventaDto.v_IdVendedorRef = cboVendedorRef.Value.ToString();
                        _ventaDto.v_NombreClienteTemporal = _ventaDto.v_IdCliente == "N002-CL000000000"
                            ? txtRazonSocial.Text
                            : string.Empty;
                        _ventaDto.v_DireccionClienteTemporal = txtDireccion.Text;
                        _ventaDto.NombreCliente = txtRazonSocial.Text;
                        _ventaDto.CodigoCliente = txtCodigoCliente.Text;
                        _ventaDto.d_total_isc = decimal.Parse(txtISC.Text);
                        _ventaDto.d_total_otrostributos = decimal.Parse(txtOtrosTributos.Text);
                        _ventaDto.d_ValorFOB = !string.IsNullOrEmpty(txtValorFOB.Text)
                            ? decimal.Parse(txtValorFOB.Text.Trim())
                            : 0;
                        _ventaDto.v_PlacaVehiculo = txtPlacaVehiculo.Text.Trim();
                        _ventaDto.i_IdTipoNota = int.Parse((string)ucTipoNota.Value ?? "-1");
                        _ventaDto.i_EsGratuito = (short)(chkEsGratuito.Checked ? 1 : 0);
                        _ventaDto.i_EstadoSunat = (short)EstadoSunat.PENDIENTE;
                        _ventaDto.i_IdTipoBulto = -1;
                        _ventaDto.d_FleteTotal = decimal.TryParse(txtFlete.Text, out d) ? d : 0m;
                        _ventaDto.d_SeguroTotal = decimal.TryParse(txtSeguro.Text, out d) ? d : 0m;
                        _ventaDto.d_CantidaTotal = decimal.TryParse(txtCantidadTotal.Text, out d) ? d : 0m;
                        _ventaDto.v_NroBL = string.IsNullOrEmpty(txtNroBL.Text) ? "" : txtNroBL.Text.Trim();
                        _ventaDto.t_FechaPagoBL = dtpFechaBL.Value.Date;
                        _ventaDto.v_Contenedor = string.IsNullOrEmpty(txtContenedor.Text) ? "" : txtContenedor.Text.Trim();
                        _ventaDto.v_Banco = string.IsNullOrEmpty(txtBanco.Text) ? "" : txtBanco.Text.Trim();
                        _ventaDto.v_Naviera = string.IsNullOrEmpty(txtNaviera.Text) ? "" : txtNaviera.Text.Trim();
                        _ventaDto.d_Percepcion = decimal.TryParse(txtPercepcion.Text, out d) ? d : 0m;
                        _ventaDto.i_ItemsAfectosPercepcion = chkArticulosAfectosPercepcion.Checked ? 1 : 0;
                        _ventaDto.i_AplicaPercepcion = rbPercepcionSI.Checked ? 1 : 0;
                        _ventaDto.d_PorcentajePercepcion = decimal.TryParse(txtPercepcionPorcentaje.Text, out d) ? d : 0;
                        _ventaDto.v_NroBultoIngles = txtNroBultoIngles.Text;
                        _ventaDto.i_EsAnticipo = chkAnticipo.Checked ? 1 : 0;
                        _ventaDto.v_IdDocAnticipo = (string)txtDocAnticipo.Tag;
                        _ventaDto.i_ClienteEsAgente = int.Parse(cboTipoServicio.Value.ToString());
                        LlenarTemporalesVenta();
                        _ventaDto.v_IdVenta = _IdVentaGuardada = _objVentasBL.InsertarVenta(ref objOperationResult, _ventaDto,
                           Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, null, false, false,
                           listaGuiasRemisionPorAnular, EliminarVentas, Extraccion);

                        #endregion


                    }
                }
                else if (_Mode == "Edit")
                {
                    if (_ventaDto.i_IdEstado == 1 && int.Parse(cboEstado.Value.ToString()) != 1)
                    {
                        if (
                            UltraMessageBox.Show(
                                "Está por anular el Comprobante, esto restaurará el Stock, ¿Desea continuar?",
                                "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                            DialogResult.Yes)
                        {
                            if (!string.IsNullOrEmpty(_ventaDto.v_NroPedido) &&
                                !_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) &&
                                int.Parse(cboDocumento.Value.ToString()) != 8)
                            {
                                _objVentasBL.RestauraPedido(ref objOperationResult, _ventaDto.v_NroPedido,
                                    Globals.ClientSession.GetAsList(), _ventaDto.v_IdVenta);
                            }
                        }
                        else
                        {
                            return;
                        }
                        var guiasRemision =
                            _objVentasBL.ObtenerDetalleGuiaRemisionporDocumentoRef(
                                int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim(),
                                txtCorrelativoDocIni.Text.Trim());
                        if (guiasRemision.Any())
                        {
                            UltraMessageBox.Show("Imposible Anular un Documento con Guia de Remisión Generada.\n Nro Guia(s) : " + string.Join(",", guiasRemision.Select(o => o.v_Observacion)),
                                "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    using (new PleaseWait(Location, "Por favor espere..."))
                    {
                        #region Actualiza Entidad Venta
                        decimal d;
                        _ventaDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                        _ventaDto.d_Anticipio = txtAnticipio.Text != string.Empty ? decimal.Parse(txtAnticipio.Text) : 0;
                        _ventaDto.d_IGV = txtIGV.Text != string.Empty ? decimal.Parse(txtIGV.Text) : 0;
                        _ventaDto.d_TipoCambio = txtTipoCambio.Text != string.Empty
                            ? decimal.Parse(txtTipoCambio.Text)
                            : 0;
                        _ventaDto.d_Valor = txtValor.Text != string.Empty ? decimal.Parse(txtValor.Text) : 0;
                        _ventaDto.d_Total = txtTotal.Text != string.Empty ? decimal.Parse(txtTotal.Text) : 0;
                        _ventaDto.d_ValorVenta = txtValorVenta.Text != string.Empty
                            ? decimal.Parse(txtValorVenta.Text)
                            : 0;
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
                        _ventaDto.v_Correlativo = txtCorrelativo.Text;
                        _ventaDto.v_Concepto = txtConcepto.Text;
                        _ventaDto.v_SerieDocumento = txtSerieDoc.Text.Trim();
                        _ventaDto.v_CorrelativoDocumento = txtCorrelativoDocIni.Text.Trim();
                        _ventaDto.v_CorrelativoDocumentoFin = txtCorrelativoDocFin.Text.Trim();
                        _ventaDto.v_SerieDocumentoRef = txtSerieDocRef.Text.Trim();
                        _ventaDto.d_PorcDescuento = txtPorcDescuento.Text == string.Empty
                            ? 0
                            : decimal.Parse(txtPorcDescuento.Text);
                        _ventaDto.d_PocComision = txtPorcComision.Text == string.Empty
                            ? 0
                            : decimal.Parse(txtPorcComision.Text);
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
                        _ventaDto.d_PesoBrutoKG = txtPesoBrutoKg.Text == string.Empty
                            ? 0
                            : decimal.Parse(txtPesoBrutoKg.Text.Trim());
                        _ventaDto.d_PesoNetoKG = txtPesoNetoKg.Text == string.Empty
                            ? 0
                            : decimal.Parse(txtPesoNetoKg.Text.Trim());
                        _ventaDto.t_FechaOrdenCompra = dtpFechaOrden.Value;
                        _ventaDto.i_DrawBack = chkDrawnBack.Checked == true ? 1 : 0;
                        _ventaDto.i_IdMedioPagoVenta = cboMVenta.Value == null ? -1 : int.Parse(cboMVenta.Value.ToString());
                        _ventaDto.i_IdPuntoDestino = cboPuntoDestino.Value == null ? -1 : int.Parse(cboPuntoDestino.Value.ToString());
                        _ventaDto.i_IdPuntoEmbarque = cboPuntoEmbarque.Value == null ? -1 : int.Parse(cboPuntoEmbarque.Value.ToString());
                        _ventaDto.i_IdTipoEmbarque = cboTipoEmbarque.Value == null ? -1 : int.Parse(cboTipoEmbarque.Value.ToString());
                        _ventaDto.i_IdTipoOperacion = int.Parse(cboTipoOperacion.Value.ToString());
                        _ventaDto.i_IdTipoVenta = int.Parse(cboTipoVenta.Value.ToString());
                        _ventaDto.v_IdVendedor = cboVendedor.Value.ToString();
                        _ventaDto.v_IdVendedorRef = cboVendedorRef.Value.ToString();
                        _ventaDto.v_NombreClienteTemporal = _ventaDto.v_IdCliente == "N002-CL000000000"
                            ? txtRazonSocial.Text.ToString()
                            : string.Empty;
                        _ventaDto.v_DireccionClienteTemporal = txtDireccion.Text;
                        _ventaDto.NombreCliente = txtRazonSocial.Text;
                        _ventaDto.CodigoCliente = txtCodigoCliente.Text;
                        _ventaDto.d_total_isc = decimal.Parse(txtISC.Text);
                        _ventaDto.d_total_otrostributos = decimal.Parse(txtOtrosTributos.Text);
                        _ventaDto.d_ValorFOB = !string.IsNullOrEmpty(txtValorFOB.Text)
                            ? decimal.Parse(txtValorFOB.Text.Trim())
                            : 0;
                        _ventaDto.v_PlacaVehiculo = txtPlacaVehiculo.Text.Trim();
                        _ventaDto.i_EsGratuito = (short)(chkEsGratuito.Checked ? 1 : 0);
                        _ventaDto.i_IdTipoBulto = -1;
                        _ventaDto.i_IdTipoNota = int.Parse((string)ucTipoNota.Value ?? "-1");
                        _ventaDto.d_FleteTotal = decimal.TryParse(txtFlete.Text, out d) ? d : 0m;
                        _ventaDto.d_SeguroTotal = decimal.TryParse(txtSeguro.Text, out d) ? d : 0m;
                        _ventaDto.d_CantidaTotal = decimal.TryParse(txtCantidadTotal.Text, out d) ? d : 0m;
                        _ventaDto.i_ClienteEsAgente = int.Parse(cboTipoServicio.Value.ToString());
                        _ventaDto.v_NroBL = string.IsNullOrEmpty(txtNroBL.Text) ? "" : txtNroBL.Text.Trim();
                        _ventaDto.t_FechaPagoBL = dtpFechaBL.Value.Date;
                        _ventaDto.v_Contenedor = string.IsNullOrEmpty(txtContenedor.Text) ? "" : txtContenedor.Text.Trim();
                        _ventaDto.v_Banco = string.IsNullOrEmpty(txtBanco.Text) ? "" : txtBanco.Text.Trim();
                        _ventaDto.v_Naviera = string.IsNullOrEmpty(txtNaviera.Text) ? "" : txtNaviera.Text.Trim();
                        _ventaDto.d_Percepcion = decimal.TryParse(txtPercepcion.Text, out d) ? d : 0m;
                        _ventaDto.i_ItemsAfectosPercepcion = chkArticulosAfectosPercepcion.Checked ? 1 : 0;
                        _ventaDto.i_AplicaPercepcion = rbPercepcionSI.Checked ? 1 : 0;
                        _ventaDto.d_PorcentajePercepcion = decimal.TryParse(txtPercepcionPorcentaje.Text, out d) ? d : 0;
                        _ventaDto.v_NroBultoIngles = txtNroBultoIngles.Text;
                        _ventaDto.i_EsAnticipo = chkAnticipo.Checked ? 1 : 0;
                        _ventaDto.v_IdDocAnticipo = (string)txtDocAnticipo.Tag;
                        
                        LlenarTemporalesVenta();
                        _objVentasBL.ActualizarVenta(ref objOperationResult, _ventaDto,
                            Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_ModificarDto,
                            _TempDetalle_EliminarDto);
                        if (objOperationResult.Success == 1 &&
                            _objDocumentoBL.DocumentoEsInverso(_ventaDto.i_IdTipoDocumento.Value))
                            new CobranzaBL().RecalcularSaldoVenta(ref objOperationResult, _ventaDto.v_IdVenta,
                                Globals.ClientSession.GetAsList(), false, true);

                        #endregion

                        //joseph
                        //guardo los campos modificados
                        
                    }
                    if (objOperationResult.Success == 1)
                    {

                    }
                }

                if (objOperationResult.Success == 1)
                {
                    objOperationResult = new OperationResult();
                    new CobranzaBL().RecalcularSaldoVenta(ref objOperationResult, _ventaDto.v_IdVenta,
                        Globals.ClientSession.GetAsList(), false, new DocumentoBL().DocumentoEsInverso(_ventaDto.i_IdTipoDocumento ?? 0));

                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(
                            objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                            objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    _modificada = true;
                    using (new PleaseWait(this.Location, "Por favor espere..."))
                    {
                        string comentarioUpdate = GetChanges();
                        new UpdateCommentaryVentaBL().UpdateCommentaryVenta(_idVenta, comentarioUpdate);

                        strModo = "Guardado";
                        EdicionBarraNavegacion(true);
                        strIdVenta = _ventaDto.v_IdVenta;
                        BtnImprimir.Enabled = _btnImprimir;
                        CargarCabecera(_IdVentaGuardada);
                        _pstrIdMovimiento_Nuevo = _ventaDto.v_IdVenta;
                        _idVenta = _pstrIdMovimiento_Nuevo;

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

                    if (
                        UltraMessageBox.Show("El registro se ha guardado correctamente, ¿Desea Generar uno Nuevo?",
                            "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) ==
                        System.Windows.Forms.DialogResult.Yes)
                    {
                        btnNuevoMovimiento_Click(sender, e);
                    }
                }
                else
                {
                    UltraMessageBox.Show(
                        string.Format("{0}\n\n{1}\n\nTARGET: {2}", objOperationResult.ErrorMessage,
                            objOperationResult.ExceptionMessage, objOperationResult.AdditionalInformation), "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                _TempDetalle_AgregarDto = new List<ventadetalleDto>();
                _TempDetalle_ModificarDto = new List<ventadetalleDto>();
                _TempDetalle_EliminarDto = new List<ventadetalleDto>();
            }
            else
            {
                UltraMessageBox.Show("Por favor llene los campos requeridos antes de guardar", "Sistema",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            
        }

        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            var tipoCambio = _objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult,
                dtpFechaRegistro.Value.Date);
            txtTipoCambio.Text = tipoCambio;
            dtpFechaVencimiento.MinDate = dtpFechaRegistro.Value;

            txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
            txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Venta,
                _ventaDto.t_FechaRegistro, dtpFechaRegistro.Value, _ventaDto.v_Correlativo, 0);

            if (objOperationResult.Success == 0)
            {
                MessageBox.Show(objOperationResult.ErrorMessage + "\n" + objOperationResult.ExceptionMessage,
                    objOperationResult.AdditionalInformation, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_objCierreMensualBL.VerificarMesCerrado(txtPeriodo.Text, txtMes.Text, (int)ModulosSistema.VentasFacturacion) ||
                _utilizado == "KARDEX")
            {
                btnGuardar.Visible = false;
                Text = _utilizado == "KARDEX" ? "Registro de Venta" : "Registro de Venta [MES CERRADO]";
                if (_utilizado == "KARDEX")
                {

                    btnSalir.Visible = false;
                    btnAgregar.Visible = false;
                    btnEliminar.Visible = false;
                    BtnImprimir.Visible = false;

                }
            }
            else
            {

                btnGuardar.Visible = true;
                Text = "Registro de Venta";
            }
            dtpFechaOrden.Value = dtpFechaRegistro.Value;
            //dtpFechaRef.Value = dtpFechaRegistro.Value;
            dtpFechaVencimiento.Value = dtpFechaRegistro.Value;
        }

        private void btnBuscarDetraccion_Click(object sender, EventArgs e)
        {
            frmBuscarCliente frm = new frmBuscarCliente("VV", txtRucCliente.Text.Trim());
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {
                if (frm._IdCliente != "N002-CL000000000")
                {

                    if (cboDocumento.Value.ToString().Equals("1") && !Utils.Windows.ValidarRuc(frm._NroDocumento) &&
                        frm._TipoPersona != 3)
                    {
                        UltraMessageBox.Show("En Factura no se puede elegir este cliente.", "Error de validación.",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    //TipoPersona = frm._TipoPersona;

                    IdIdentificacion = frm._TipoDocumento;
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

                }
                else
                {

                    UltraMessageBox.Show("En Factura no se puede elegir este cliente.", "Error de validación.",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //TipoPersona = -1;


                }

            }
        }

        private void chkAfectoIGV_CheckedChanged(object sender, EventArgs e)
        {
            chkPrecInIGV.Enabled = chkAfectoIGV.Checked;
            if (!chkAfectoIGV.Checked) chkPrecInIGV.Checked = false;
            //CalcularValoresDetalle(); // No necesario ya que se ejecuta al cambiar el Value de cboTipoOperacion
            foreach (var row in grdData.Rows)
            {
                row.Cells["i_RegistroEstado"].Value = "Modificado";
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
                var c = grdData.DisplayLayout.Bands[0].Columns["i_Anticipio"];
                c.CellActivation = Activation.AllowEdit;
                c.CellClickAction = CellClickAction.Edit;
                puDocAntipo.Show((Control)sender);
            }
            else
            {
                foreach (var row in grdData.Rows)
                {
                    row.Cells["i_Anticipio"].Value = 0;
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                }
                var c = grdData.DisplayLayout.Bands[0].Columns["i_Anticipio"];
                c.CellActivation = Activation.NoEdit;
                c.CellClickAction = CellClickAction.CellSelect;
                CalcularValoresDetalle();
            }
            FilasAnticipioActivacion(chkDeduccionAnticipio.Checked);
        }

        private void txtMes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
                if (txtMes.Text != "")
                {
                    int Mes = int.Parse(txtMes.Text);
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
                        UltraMessageBox.Show("Mes inválido", "Error de Validación", MessageBoxButtons.OK,
                            MessageBoxIcon.Hand);
                    }
                }
            }
        }

        private void txtMes_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtMes, e);
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_ListadoVentas.Count() > 0)
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
                OperationResult objOperationResult = new OperationResult();
                CargarDetalle("");
                LimpiarCabecera();

                _ListadoVentas = _objVentasBL.ObtenerListadoVentas(ref objOperationResult, txtPeriodo.Text.Trim(),
                    txtMes.Text.Trim());
                _MaxV = _ListadoVentas.Count() - 1;

                if (_ListadoVentas.Any())
                    txtCorrelativo.Text = (int.Parse(_ListadoVentas[_MaxV].Value1) + 1).ToString("00000000");
                else
                    txtCorrelativo.Text = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString("00") +
                                          @"000001";

                _Mode = "New";
                _ventaDto = new ventaDto();

                strModo = "Nuevo";
                EdicionBarraNavegacion(true);
                txtTipoCambio.Text = _objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult,
                    dtpFechaRegistro.Value.Date);
                cboDocumento.PerformAction(UltraComboAction.Dropdown);
                BtnImprimir.Enabled = false;
                btnAgregar.Enabled = true;
                Extraccion = false;
                ExtraccionPedido = false;
                HabilitarLotesSerie(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _TempDetalle_AgregarDto = new List<ventadetalleDto>();
                _TempDetalle_ModificarDto = new List<ventadetalleDto>();
                _TempDetalle_EliminarDto = new List<ventadetalleDto>();
            }
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
                UltraMessageBox.Show(string.Format("{0}\nLinea: {1}", ex.Message,
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '))));
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (
                    UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _ventadetalleDto = new ventadetalleDto()
                    {
                        v_IdVentaDetalle = grdData.ActiveRow.Cells["v_IdVentaDetalle"].Value.ToString()
                    };
                    _TempDetalle_EliminarDto.Add(_ventadetalleDto);


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
            if (cboDocumento.Value == null) return;

            var id = cboDocumento.Value.ToString();
            txtSerieDoc.MaxLength = id.Equals("12") ? 20 : 4;

            if (!id.Equals("12") && txtSerieDoc.TextLength > 4)
                txtSerieDoc.Text = txtSerieDoc.Text.Substring(0, 4);


            if (id == "3" || id == "12")
            {
                if (_Mode == "New")
                    txtCorrelativoDocFin.Enabled = true;
            }
            else
            {
                txtCorrelativoDocFin.Enabled = false;
                txtCorrelativoDocFin.Clear();
            }
            //if (!Globals.ClientSession.EsEmisorElectronico) return;
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
                    if (txtRucCliente.Text.Length != 11)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema");
                        _ventaDto.v_IdCliente = null;
                        _ventaDto.i_IdDireccionCliente = -1;
                        txtCodigoCliente.Clear();
                        txtRazonSocial.Clear();
                        txtRucCliente.Focus();
                        txtDireccion.Clear();
                        //TipoPersona = -1;
                    }
                    else
                    {
                        if (Utils.Windows.ValidarRuc(txtRucCliente.Text.Trim()) != true)
                        {
                            UltraMessageBox.Show("RUC Inválido", "Sistema");
                            _ventaDto.v_IdCliente = null;
                            _ventaDto.i_IdDireccionCliente = -1;
                            txtCodigoCliente.Clear();
                            txtRazonSocial.Clear();
                            txtRucCliente.Focus();
                            txtDireccion.Clear();
                            //TipoPersona = -1;
                        }
                    }
                }
            }
        }

        private void txtCorrelativoDocFin_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCorrelativoDocFin.Text.Trim()))
            {
                var docIni = int.Parse(txtCorrelativoDocIni.Text.Trim());
                var docFin = int.Parse(txtCorrelativoDocFin.Text.Trim());
                if (docFin < docIni)
                {
                    UltraMessageBox.Show("Número de correlativo inválido", "Error de Validación.", MessageBoxButtons.OK,
                        MessageBoxIcon.Hand);
                    txtCorrelativoDocFin.Focus();
                }
            }

        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {

            #region Comentado
            //idVentasImpresion = new List<string>();
            //idVentasImpresion = grdData.Rows.Select(p => p.Cells["v_IdVenta"].Value.ToString()).Distinct().ToList();
            //impresionVistaPrevia = _objDocumentoBL.ImpresionVistaPrevia(int.Parse(cboDocumento.Value.ToString()),
            //    txtSerieDoc.Text.Trim());
            //var establecimientoDetalle =
            //    _objDocumentoBL.ConfiguracionEstablecimiento(int.Parse(cboDocumento.Value.ToString()),
            //        txtSerieDoc.Text.Trim());
            //if (establecimientoDetalle == null)
            //{
            //    UltraMessageBox.Show(
            //        string.Format("Documento {0} {1} no se encuentra asignado en Establecimiento ", cboDocumento.Text,
            //            txtSerieDoc.Text), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;

            //}

            //if (Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic || Globals.ClientSession.v_RucEmpresa == Constants.RucDemo || Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic2)
            //{
            //    pnlImpresionPrefactura.Visible = true;
            //    BtnImprimir.Enabled = false;
            //    btnGuardar.Enabled = false;


            //}
            //else
            //{

            //    var idDoc = int.Parse(cboDocumento.Value.ToString());

            //    if (idDoc == (int)DocumentType.Factura)
            //    {
            //        if (Globals.ClientSession.v_RucEmpresa == Constants.RucChayna)
            //        {
            //            var doc = new DocFactura(idVentasImpresion);
            //            doc.Print();
            //        }
            //        else
            //            if (Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic || Globals.ClientSession.v_RucEmpresa == Constants.RucDemo || Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic2)
            //            {
            //                using (var frm = new frmDocumentoFactura(idVentasImpresion, impresionVistaPrevia))
            //                {
            //                    if (impresionVistaPrevia)
            //                    {
            //                        frm.ShowDialog();
            //                        frm.Activate();
            //                    }
            //                    else
            //                    {

            //                        BtnImprimir.Enabled = false;


            //                    }
            //                }


            //            }
            //            else
            //            {

            //                using (var frm = new frmDocumentoFactura(idVentasImpresion, impresionVistaPrevia))
            //                {
            //                    if (impresionVistaPrevia)
            //                    {
            //                        frm.ShowDialog();
            //                        frm.Activate();
            //                    }
            //                    else
            //                    {

            //                        BtnImprimir.Enabled = false;


            //                    }
            //                }
            //            }
            //    }
            //    if (idDoc == (int)DocumentType.Boleta)
            //    {

            //        if (Globals.ClientSession.v_RucEmpresa == Constants.RucChayna)
            //        {
            //            var doc = new DocBoleta(idVentasImpresion);
            //            doc.Print();
            //        }
            //        else

            //            using (var frm = new frmDocumentoBoleta(idVentasImpresion, impresionVistaPrevia))
            //            {
            //                if (impresionVistaPrevia)
            //                {
            //                    frm.ShowDialog();
            //                    frm.Activate();
            //                }
            //                else
            //                {
            //                    BtnImprimir.Enabled = false;

            //                }
            //            }
            //    }
            //    if (idDoc == (int)DocumentType.NotaCredito)
            //    {
            //        if (Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita)
            //        {
            //            var tk = new Reportes.Ventas.Ablimatex.TicketNcr(idVentasImpresion);
            //            tk.Print();
            //        }
            //        else using (var frm = new frmDocumentoNotaCredito(idVentasImpresion, impresionVistaPrevia))
            //                if (impresionVistaPrevia)
            //                {
            //                    frm.ShowDialog();
            //                    frm.Activate();
            //                }
            //                else
            //                {
            //                    BtnImprimir.Enabled = false;

            //                }
            //    }

            //    if (idDoc == (int)DocumentType.NotaDebito)
            //    {
            //        using (var frm = new frmDocumentoNotaDebito(idVentasImpresion,
            //                impresionVistaPrevia))
            //        {
            //            if (impresionVistaPrevia)
            //            {
            //                frm.ShowDialog();
            //                frm.Activate();
            //            }
            //            else
            //            {
            //                BtnImprimir.Enabled = false;

            //            }
            //        }

            //    }
            //    if (idDoc == (int)DocumentType.TicketBoleta)
            //    {
            //        var idTickets = grdData.Rows.Select(p => p.Cells["v_IdVenta"].Value.ToString()).Distinct().ToArray();
            //        if (idTickets.Length > 0)
            //        {
            //            var printer = new Reportes.Ventas.Ablimatex.Ticket(idTickets);
            //            printer.Print();
            //            if (!impresionVistaPrevia)
            //                BtnImprimir.Enabled = false;

            //            // res = true;
            //        }
            //    }




            //    if ((Constants.RucManguifajas == Globals.ClientSession.v_RucEmpresa ||
            //         Constants.RucMultimangueras == Globals.ClientSession.v_RucEmpresa) &&
            //        (idDoc == (int)TiposDocumentos.GuiaInterna || idDoc == (int)TiposDocumentos.GuiaTraslado))
            //    {
            //        using (var frm = new frmDocumentoGuiaOtroFormato(idVentasImpresion, impresionVistaPrevia, true))
            //        {
            //            if (impresionVistaPrevia)
            //            {
            //                frm.ShowDialog();
            //                frm.Activate();
            //            }
            //            else
            //            {
            //                BtnImprimir.Enabled = false;

            //            }
            //        }

            //    }

            //    else
            //    {

            //        //if ((Constants.RucRollavel == Globals.ClientSession.v_RucEmpresa ||
            //        //     Constants.RucJorplast == Globals.ClientSession.v_RucEmpresa) &&
            //        //    idDoc == (int)DocumentType.Proforma)

            //        if (idDoc == (int)DocumentType.Proforma)
            //        {
            //            using (var frm = new frmDocumentoProforma(idVentasImpresion,
            //                    impresionVistaPrevia))
            //            {
            //                if (impresionVistaPrevia)
            //                {
            //                    frm.ShowDialog();
            //                    frm.Activate();

            //                }
            //                else
            //                {
            //                    BtnImprimir.Enabled = false;

            //                }
            //            }
            //        }
            //    }
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

                    string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                    string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                    Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
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

                    string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                    string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                    Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
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

                    string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                    string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                    Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
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

                    string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                    string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                    Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                    this.Enabled = true;
                }
            }
            else if (idDoc == (int)DocumentType.EGRESO_CAJA_ASISTENCIAL)
            {
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                this.Enabled = true;
            }
            else if (idDoc == (int)DocumentType.INGRESO_CAJA_OCUPACIONAL)
            {
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                this.Enabled = true;
            }
            else if (idDoc == (int)DocumentType.INGRESO_CAJA_ASISTENCIAL)
            {
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                this.Enabled = true;
            }
            else if (idDoc == (int)DocumentType.EGRESO_CAJA_FARMACIA)
            {
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                this.Enabled = true;
            }
            else if (idDoc == (int)DocumentType.INGRESO_CAJA_FARMACIA)
            {
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                var datosP = new VentaBL().GetIngresosEgresos(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);
                this.Enabled = true;
            }
            else if (idDoc == (int)DocumentType.RECIBO_SAN_LORENZO)
            {
                try
                {
                    string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                    string nroRecigo = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text;
                    string pdfPath = Path.Combine(Application.StartupPath, ruta + nroRecigo + ".pdf");
                    Process.Start(pdfPath);
                    this.Enabled = true;
                }
                catch (Exception)
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
                        //string calendar = listaServicios.Count() == 0 ? "CC" : ObtenerCalendar(listaServicios[0].ToString());
                        //string person = listaServicios.Count() == 0 ? "PP" : ObtenerPersonId(listaServicios[0].ToString());
                        //DateTime fechaNacimiento = listaServicios.Count() == 0 ? DateTime.Now : ObtenerFechaNac(person);
                        //Recibo_San_Lorenzo.CreateRecibo_San_Lorenzo(ruta + nombre + ".pdf", MedicalCenter, datosP);
                        //string service = GetHistoryCLinic(person);//listaServicios.Count() == 0 ? "SS" : listaServicios[0].ToString();
                        string DatosPaciente = ObtenerDatosPaciente(txtRucCliente.Text);
                        DatosPaciente = DatosPaciente + "|SS";
                        Recibo_Interno2.CreateRecibo(ruta, DatosPaciente, "VENTA DIRECTA", DateTime.Now, serie_correlativo, datosP);
                    }
                    else
                    {
                        var msj = string.Format("Por favor registre un Recibo para Imprimir comprobante.");
                        MessageBox.Show(msj, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    this.Enabled = true;
                }

                //var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();

                //var datosP = new VentaBL().GetReciboSanLorenzo(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDocIni.Text);

                //string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                //string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                //Recibo_San_Lorenzo.CreateRecibo_San_Lorenzo(ruta + nombre + ".pdf", MedicalCenter, datosP);
                //this.Enabled = true;
            }
            else if (idDoc == (int)DocumentType.TICKET_MEDICINAS)
            {
                var MedicalCenter = new ServiceBL().GetInfoMedicalCenter();



                var datosP = new VentaBL().TicketReceta(txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text);

                string ruta = GetApplicationConfigValue("rutaEgresos").ToString();
                string nombre = txtSerieDoc.Text + "-" + txtCorrelativoDocIni.Text + " - CSL";
                //Reporte_Egresos.CreateReporte_Egresos(ruta + nombre + ".pdf", MedicalCenter, datosP);

                TicketMedicina.CreateTicket_Medicina(ruta + nombre + ".pdf", MedicalCenter, datosP);

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

        public static string GetApplicationConfigValue(string nombre)
        {
            return Convert.ToString(ConfigurationManager.AppSettings[nombre]);
        }
        private void btnRedondear_Click(object sender, EventArgs e)
        {
            Redondeo();
        }

        private void cboMoneda_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMoneda.Value == null) return;
            switch (cboMoneda.Value.ToString())
            {
                case "1":
                    cboTipoVenta.Value = "03";
                    break;

                case "2":
                    cboTipoVenta.Value = "04";
                    //Se agrego el Tipo de Venta Predeterminado
                    //cboTipoVenta.Value = Globals.ClientSession.i_TipoVentaVentas !=-1 ? Globals.ClientSession.i_TipoVentaVentas.ToString("00") :"04";
                    break;
            }

            if (!grdData.Rows.Any()) return;
            var resp =
                MessageBox.Show(
                    string.Format("¿Desea aplicar el cambio [{0}] a los precios del detalle?", txtTipoCambio.Text),
                    @"Sistema",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.No) return;
            AplicarConversion(cboMoneda.Value.ToString(), decimal.Parse(txtTipoCambio.Text), grdData);

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

        //  #endregion

        #region Comportamiento de Controles

        private void txtGuiaRemisionSerie_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtGuiaRemisionSerie, e);
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

        private void txtCorrelativoDocRef_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDocRef, e);
        }

        private void txtSerieDocRef_Validated(object sender, EventArgs e)
        {
            //Utils.Windows.FijarFormatoUltraTextBox(txtSerieDocRef, "{0:0000}");
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

        private void txtSerieDoc_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSerieDoc.Text))
            {
                int Serie = 0;
                if (int.TryParse(txtSerieDoc.Text.Trim(), out Serie))
                {
                    Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc, "{0:0000}");
                }
                else
                    txtSerieDoc.Text = txtSerieDoc.Text.Trim();

                ComprobarExistenciaCorrelativoDocumento();
                string Correlativo = _objDocumentoBL.CorrelativoxSerie(int.Parse(cboDocumento.Value.ToString()),
                    txtSerieDoc.Text);
                if (Correlativo != null)
                {
                    txtCorrelativoDocIni.Text = Correlativo;
                }
                else
                {
                    UltraMessageBox.Show("No se encuentra registrado la serie en la configuración del Establecimiento",
                        "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    txtSerieDoc.Focus();
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
            foreach (UltraGridRow row in cboDocumento.Rows)
            {
                if (cboDocumento.Value != null && cboDocumento.Value.ToString() == "-1")
                    cboDocumento.Text = string.Empty;
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
                        var x =
                            _ListadoComboDocumentos.Find(
                                p => p.Id == cboDocumento.Value.ToString() | p.Id == cboDocumento.Text);
                        if (x == null)
                        {
                            cboDocumento.Value = "-1";
                        }
                    }

                    if (cboDocumento.Value != null && txtSerieDoc.Text.Trim() != string.Empty)
                        _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()),
                            txtSerieDoc.Text.Trim());

                    if (cboDocumento.Value.ToString() == "3" || cboDocumento.Value.ToString() == "12")
                    {
                        ClientePublicoGeneral();
                        label34.Visible = true;
                        txtCorrelativoDocFin.Visible = true;
                        txtCorrelativoDocFin.Enabled = true;
                        txtCorrelativoDocFin.Text = txtCorrelativoDocIni.Text;
                        txtConcepto.Text = @"ATENCIÓN MÉDICA";
                    }
                    else if (_ventaDto.v_IdCliente == "N002-CL000000000")
                    {
                        _ventaDto.v_IdCliente = string.Empty;
                        _ventaDto.i_IdDireccionCliente = -1;
                        txtRucCliente.Text = "";
                        txtRazonSocial.Clear();
                        txtCodigoCliente.Clear();
                        txtDireccion.Clear();
                        label34.Visible = false;
                        txtCorrelativoDocFin.Visible = false;
                        txtCorrelativoDocFin.Enabled = false;
                        txtCorrelativoDocFin.Text = string.Empty;
                    }

                    txtSerieDoc.Text =
                        _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento,
                            int.Parse(cboDocumento.Value.ToString())).Trim();

                    if (cboDocumento.Value != null && cboDocumento.Value.ToString() != "-1" && !string.IsNullOrEmpty(UserConfig.Default.SerieCaja) && UserConfig.Default.SerieCaja != "--Seleccionar--")
                    {
                        txtSerieDoc.Text = UserConfig.Default.SerieCaja;
                    }
                    CancelEventArgs e1 = new CancelEventArgs();
                    txtSerieDoc_Validating(sender, e1);
                    PredeterminarEstablecimiento(txtSerieDoc.Text);
                    if (!_objDocumentoBL.DocumentoEsContable(int.Parse(cboDocumento.Value.ToString())))
                    //Validación agregada por GUI
                    {

                        chkPrecInIGV.Checked = false;
                        chkAfectoIGV.Checked = false;

                    }
                    else
                    {
                        chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1;
                        chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvVentas == 1;
                        //Se cambió el orden 26 abril del 2017
                        //chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvVentas == 1;
                        //chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1;

                    }


                }

                var y = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento.Value.ToString() | p.Id == cboDocumento.Text);
                if (y == null)
                {
                    cboDocumento.Value = "-1";
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
                    dtpFechaRef.Enabled = false;
                    chkDeduccionAnticipio.Enabled = true;
                }
                _EsNotadeCredito = _objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString()))
                    ? true
                    : false;
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
                if (cboDocumento.Value.ToString() == "7") cboDocumentoRef.Focus();
            }
            else
            {
                cboDocumento.Value = "-1";
            }
        }

        private void cboDocumentoRef_AfterDropDown(object sender, EventArgs e)
        {
            foreach (UltraGridRow row in cboDocumentoRef.Rows)
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
            foreach (UltraGridRow row in cboDocumentoRef.Rows)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    return;
                }
                var filterRow =
                    cboDocumentoRef.DisplayLayout.Bands[0].Columns.Cast<UltraGridColumn>()
                        .Where(column => column.IsVisibleInLayout)
                        .All(column => !row.Cells[column].Text.Contains(cboDocumentoRef.Text.ToUpper()));
                row.Hidden = filterRow;
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
                    var x = _ListadoComboDocumentos.Find(p => p.Id == cboDocumentoRef.Value.ToString() | p.Id == cboDocumentoRef.Text);
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

        private void txtNroDias_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNroDias.Text))
            {
                if (int.Parse(txtNroDias.Text) > 365)
                {
                    UltraMessageBox.Show("El número de días no puede exceder de 365(1 año)", "Error de Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    UltraMessageBox.Show("Cantidad Inválida", "Error de Validación", Icono: MessageBoxIcon.Exclamation);
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
                    UltraMessageBox.Show("Cantidad Inválida", "Error de Validación", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                }
            }
        }

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            try
            {
                PredeterminarEstablecimiento(txtSerieDoc.Text);
                _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()),
                    txtSerieDoc.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Clases/Validaciones


        private void HabilitarLotesSerie(bool Habilitar)
        {
            grdData.DisplayLayout.Bands[0].Columns["v_NroSerie"].CellActivation = !Habilitar ? Activation.NoEdit : Activation.AllowEdit;
            grdData.DisplayLayout.Bands[0].Columns["v_NroSerie"].CellClickAction = !Habilitar ? CellClickAction.CellSelect : CellClickAction.EditAndSelectText;
            grdData.DisplayLayout.Bands[0].Columns["v_NroLote"].CellActivation = !Habilitar ? Activation.NoEdit : Activation.AllowEdit;
            grdData.DisplayLayout.Bands[0].Columns["v_NroLote"].CellClickAction = !Habilitar ? CellClickAction.CellSelect : CellClickAction.EditAndSelectText;
            grdData.DisplayLayout.Bands[0].Columns["t_FechaCaducidad"].CellActivation = !Habilitar ? Activation.NoEdit : Activation.AllowEdit;
            grdData.DisplayLayout.Bands[0].Columns["t_FechaCaducidad"].CellClickAction = !Habilitar ? CellClickAction.CellSelect : CellClickAction.EditAndSelectText;
        }


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
                    HabilitarLotesSerie(false);
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
                        txtCorrelativo.ButtonsRight[0].Enabled = false;
                        txtCorrelativo.ButtonsRight[1].Enabled = false;
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
                    txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult,
                        ListaProcesos.Venta, _ventaDto.t_FechaRegistro, dtpFechaRegistro.Value, _ventaDto.v_Correlativo,
                        0);

                    if (objOperationResult.Success == 0)
                    {
                        MessageBox.Show(objOperationResult.ErrorMessage + "\n" + objOperationResult.ExceptionMessage,
                            objOperationResult.AdditionalInformation, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    txtTipoCambio.Text = _objVentasBL.DevolverTipoCambioPorFecha(ref objOperationResult,
                        dtpFechaRegistro.Value.Date);
                    //cboCondicionPago.Value = "1";
                    cboCondicionPago.Value = Globals.ClientSession.i_IdCondicionPagoVenta == -1 ? 1 : Globals.ClientSession.i_IdCondicionPagoVenta;
                    cboEstado.Value = "1";
                    CargarDetalle("");
                    btnExtraer.Enabled = true;
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
                    HabilitarLotesSerie(false);
                    break;

                case "Consulta":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdVenta);
                    _idVenta = strIdVenta;
                    cboDocumento.Enabled = false;
                    txtSerieDoc.Enabled = false;
                    RestringirEdicion();
                    HabilitarLotesSerie(false);
                    break;
            }
        }

        private void EdicionBarraNavegacion(bool ON_OFF)
        {
            cboDocumento.Enabled = ON_OFF;
            txtSerieDoc.Enabled = ON_OFF;
            cboDocumentoRef.Enabled = ON_OFF;
            //btnBuscarDetraccion.Enabled = ON_OFF;
            txtSerieDocRef.Enabled = ON_OFF;
            txtCorrelativoDocFin.Enabled = ON_OFF;
        }

        private void CargarCabecera(string idmovimiento)
        {
            var objOperationResult = new OperationResult();
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
                cboTipoServicio.Value = _ventaDto.i_ClienteEsAgente;
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
                cboMVenta.Value = _ventaDto.i_IdMedioPagoVenta != null ? _ventaDto.i_IdMedioPagoVenta.ToString() : "-1";
                cboPuntoDestino.Value = _ventaDto.i_IdPuntoDestino != null ? _ventaDto.i_IdPuntoDestino.ToString() : "-1";
                cboPuntoEmbarque.Value = _ventaDto.i_IdPuntoEmbarque != null ? _ventaDto.i_IdPuntoEmbarque.ToString() : "-1";
                cboTipoEmbarque.Value = _ventaDto.i_IdTipoEmbarque != null ? _ventaDto.i_IdTipoEmbarque.ToString() : "-1";
                cboTipoVenta.Value = int.Parse(_ventaDto.i_IdTipoVenta.ToString()).ToString("00");
                cboVendedor.Value = _ventaDto.v_IdVendedor ?? "-1";
                cboVendedorRef.Value = _ventaDto.v_IdVendedorRef;
                chkAfectoIGV.Checked = _ventaDto.i_EsAfectoIgv == 1;
                chkDeduccionAnticipio.Checked = _ventaDto.i_DeduccionAnticipio == 1;
                chkDrawnBack.Checked = _ventaDto.i_DrawBack == 1;
                chkPrecInIGV.Checked = _ventaDto.i_PreciosIncluyenIgv == 1;
                txtTipoCambio.Text = _ventaDto.d_TipoCambio.ToString();
                txtRucCliente.Enabled = _ventaDto.v_IdCliente != "N002-CL000000000" && txtRucCliente.Enabled;
                txtRazonSocial.Enabled = _ventaDto.v_IdCliente == "N002-CL000000000";
                txtDireccion.Enabled = _ventaDto.v_IdCliente == "N002-CL000000000";
                txtPlacaVehiculo.Text = _ventaDto.v_PlacaVehiculo;
                ucTipoNota.Value = _ventaDto.i_IdTipoNota;
                _Mode = "Edit";
                BtnImprimir.Enabled = _btnImprimir;
                cboTipoOperacion.Value = _ventaDto.i_IdTipoOperacion.ToString();
                chkEsGratuito.Checked = _ventaDto.i_EsGratuito.HasValue && _ventaDto.i_EsGratuito == 1;
                txtFlete.Text = (_ventaDto.d_FleteTotal ?? 0).ToString(CultureInfo.InvariantCulture);
                txtCantidadTotal.Text = (_ventaDto.d_CantidaTotal ?? 0).ToString(CultureInfo.InvariantCulture);
                txtSeguro.Text = (_ventaDto.d_SeguroTotal ?? 0).ToString(CultureInfo.InvariantCulture);
                txtNroBL.Text = _ventaDto.v_NroBL;
                dtpFechaBL.Value = _ventaDto.t_FechaPagoBL.Value;
                txtContenedor.Text = _ventaDto.v_Contenedor;
                txtBanco.Text = _ventaDto.v_Banco;
                txtNaviera.Text = _ventaDto.v_Naviera;
                rbPercepcionSI.Checked = (_ventaDto.i_AplicaPercepcion ?? 0) == 1;
                rbPercepcionNO.Checked = (_ventaDto.i_AplicaPercepcion ?? 0) == 0;
                rbPercepcionAgente.Checked = (_ventaDto.i_ClienteEsAgente ?? 0) == 1;
                rbPercepcioNoagente.Checked = (_ventaDto.i_ClienteEsAgente ?? 0) == 0;
                txtPercepcionPorcentaje.Text = string.Format("{0:##.00}", (_ventaDto.d_PorcentajePercepcion ?? 0));
                chkArticulosAfectosPercepcion.Checked = (_ventaDto.i_ItemsAfectosPercepcion ?? 0) == 1;
                txtNroBultoIngles.Text = _ventaDto.v_NroBultoIngles;
                CargarDetalle(_ventaDto.v_IdVenta);
                if (_ventaDto.v_NroPedido != string.Empty)
                    RestringirEdicion(); //SI ES UNA VENTA ORIGINADA DE UN PEDIDO NO SE PUEDE EDITAR.
                txtNroPedido.Enabled = string.IsNullOrWhiteSpace(_ventaDto.v_NroPedido);
                chkAnticipo.Checked = _ventaDto.i_EsAnticipo == 1;
                txtDocAnticipo.Tag = _ventaDto.v_IdDocAnticipo;
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar la compra", "Sistema", Icono: MessageBoxIcon.Error);
            }
        }

        private void CargarDetalle(string pstringIdVenta)
        {
            var objOperationResult = new OperationResult();
            var detail = _objVentasBL.ObtenerVentaDetalles(ref objOperationResult, pstringIdVenta);
            grdData.DataSource = detail;
            if (objOperationResult.Success == 0) return;
            var blIsc = new ProductoIscBL();
            _productoiscDtos.Clear();
            foreach (var dto in detail)
                if (dto.d_isc != null && dto.d_isc.Value != 0)
                {
                    var item = blIsc.FromProductDetail(ref objOperationResult, dto.v_IdProductoDetalle,
                        Globals.ClientSession.i_Periodo.ToString());
                    if (item != null)
                        _productoiscDtos.Add(dto.v_IdProductoDetalle, item);
                }
            if (grdData.Rows.Count > 0)
            {
                foreach (var row in grdData.Rows)
                {
                    row.Cells["i_RegistroTipo"].Value = "NoTemporal";
                    FilaAnticipio(row);
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
            txtConcepto.Text = @"ATENCIÓN MÉDICA";
            txtNroBulto.Clear();
            txtNroDias.Text = @"0";
            txtNroPedido.Clear();
            txtOrdenCompra.Clear();
            txtPesoBrutoKg.Clear();
            txtPesoNetoKg.Clear();
            txtRazonSocial.Clear();
            txtRucCliente.Text = "";
            txtTotal.Clear();
            txtValorVenta.Clear();
            chkEsGratuito.Checked = false;
            //dtpFechaOrden.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim())))));
            //dtpFechaRef.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim())))));
            //dtpFechaRegistro.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Day));
            //dtpFechaVencimiento.Value = DateTime.Parse((txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" + DateTime.Now.Day));
            dtpFechaOrden.Value = new DateTime(Globals.ClientSession.i_Periodo ?? 1, DateTime.Today.Month, DateTime.DaysInMonth(Globals.ClientSession.i_Periodo ?? 1, DateTime.Today.Month));
            dtpFechaRef.Value = dtpFechaOrden.Value;
            dtpFechaRegistro.Value = new DateTime(Globals.ClientSession.i_Periodo ?? 1, DateTime.Today.Month, DateTime.Now.Day);
            dtpFechaVencimiento.Value = dtpFechaOrden.Value;
            cboEstablecimiento.Value = "-1";
            cboMVenta.Value = "-1";
            cboPuntoDestino.Value = "-1";
            cboPuntoEmbarque.Value = "-1";
            cboTipoEmbarque.Value = "-1";
            cboTipoVenta.Value = "03"; // predetermina como venta con moneda nacional
            cboVendedor.Value = Globals.ClientSession.v_IdVendedor;
            cboVendedorRef.Value = "-1";
            chkDeduccionAnticipio.Checked = false;
            chkDrawnBack.Checked = false;

            cboIGV.Value = Globals.ClientSession.i_IdIgv.ToString();
            cboMoneda.Value = Globals.ClientSession.i_IdMoneda.ToString();
            chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1;
            chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvVentas == 1;
            txtPorcComision.Text = Globals.ClientSession.d_ComisionVendedor.ToString();
            txtCorrelativoDocIni.Enabled = true;

            cboDocumento.Value = "-1";
            txtSerieDoc.Clear();
            txtCorrelativoDocIni.Clear();
            txtDireccion.Clear();
            // cboCondicionPago.Value = "1";
            cboCondicionPago.Value = Globals.ClientSession.i_IdCondicionPagoVenta == -1 ? 1 : Globals.ClientSession.i_IdCondicionPagoVenta;
            cboEstado.Value = "1";
            cboTipoOperacion.Value = Globals.ClientSession.i_IdTipoOperacionVentas.ToString();
        }

        private void CargarCombosDetalle()
        {
            var objOperationResult = new OperationResult();

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

            var __ultraGridBanda_ = new UltraGridBand("Band 0", -1);
            //var __ultraGridColumnaID_ = new UltraGridColumn("Value2");
            //__ultraGridColumnaID_.Header.Caption = @"Cod.";
            //__ultraGridColumnaID_.Header.VisiblePosition = 0;
            //__ultraGridColumnaID_.Width = 50;
            var __ultraGridColumnaDescripcion_ = new UltraGridColumn("Value1");
            __ultraGridColumnaDescripcion_.Header.Caption = @"Descripción";
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

            Utils.Windows.LoadUltraComboList(ucDestino, "Id", "Id",
                _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 24, null),
                DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucUnidadMedida, "Value1", "Id",
                _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 17, null),
                DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucAlmacen, "Id", "Id",
                _objNodeWarehouseBL.ObtenerAlmacenesParaComboGrid(ref objOperationResult, null,
                    Globals.ClientSession.i_IdEstablecimiento ?? 1), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucTipoOperacion, "Value1", "Id",
                Globals.CacheCombosVentaDto.cboTipoOperacion.Where(r => r.Id != "5").ToList(), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucRubro, "Value1", "Id",
                _objVentasBL.ObtenRubrosParaComboGridVenta(ref objOperationResult, null), DropDownListAction.Select);
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
                                _ventadetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null
                                    ? null
                                    : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _ventadetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _ventadetalleDto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _ventadetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                _ventadetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _ventadetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());

                                _ventadetalleDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_ValorVenta"].Text.Replace("_", ""));
                                _ventadetalleDto.d_Igv = Fila.Cells["d_Igv"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_Igv"].Text);
                                _ventadetalleDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_PrecioVenta"].Text);
                                _ventadetalleDto.d_Valor = Fila.Cells["d_Valor"].Value == null ||
                                                           string.IsNullOrEmpty(Fila.Cells["d_Valor"].Text)
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_Valor"].Text.Replace("_", ""));
                                _ventadetalleDto.d_Descuento = Fila.Cells["d_Descuento"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_Descuento"].Text);
                                _ventadetalleDto.d_isc = Fila.Cells["d_isc"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_isc"].Value.ToString());
                                _ventadetalleDto.d_otrostributos = Fila.Cells["d_otrostributos"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString());
                                _ventadetalleDto.i_Anticipio = Fila.Cells["i_Anticipio"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_Anticipio"].Value.ToString());
                                _ventadetalleDto.i_InsertaIdUsuario = Fila.Cells["i_InsertaIdUsuario"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _ventadetalleDto.v_NroCuenta = Fila.Cells["v_NroCuenta"].Value == null
                                    ? null
                                    : Fila.Cells["v_NroCuenta"].Value.ToString().Trim();
                                _ventadetalleDto.i_IdCentroCosto = Fila.Cells["i_IdCentroCosto"].Value == null
                                    ? null
                                    : Fila.Cells["i_IdCentroCosto"].Value.ToString();
                                _ventadetalleDto.i_IdTipoOperacion = Fila.Cells["i_IdTipoOperacion"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_IdTipoOperacion"].Value.ToString());
                                _ventadetalleDto.i_NroUnidades = Fila.Cells["i_NroUnidades"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_NroUnidades"].Value.ToString());
                                _ventadetalleDto.d_PorcentajeComision = Fila.Cells["d_PorcentajeComision"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_PorcentajeComision"].Value.ToString());
                                _ventadetalleDto.i_NroUnidades = Fila.Cells["i_NroUnidades"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_NroUnidades"].Value.ToString());

                                _ventadetalleDto.v_PedidoExportacion = Fila.Cells["v_PedidoExportacion"].Value == null
                                    ? null
                                    : Fila.Cells["v_PedidoExportacion"].Value.ToString().Trim();
                                _ventadetalleDto.v_FacturaRef = Fila.Cells["v_FacturaRef"].Value == null
                                    ? null
                                    : Fila.Cells["v_FacturaRef"].Value.ToString();
                                _ventadetalleDto.v_DescripcionProducto = Fila.Cells["v_DescripcionProducto"].Value ==
                                                                         null
                                    ? null
                                    : Fila.Cells["v_DescripcionProducto"].Value.ToString();
                                _ventadetalleDto.EsServicio = Fila.Cells["i_EsServicio"].Value == null ||
                                                              Fila.Cells["i_EsServicio"].Value.ToString() == "0"
                                    ? 0
                                    : 1;
                                _ventadetalleDto.d_Percepcion = Fila.Cells["d_Percepcion"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_Percepcion"].Value.ToString());
                                _ventadetalleDto.i_IdMonedaLP = Fila.Cells["i_IdMonedaLP"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_IdMonedaLP"].Value.ToString());

                                _ventadetalleDto.d_PrecioPactado = Fila.Cells["d_PrecioPactado"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_PrecioPactado"].Value.ToString());

                                _ventadetalleDto.d_SeguroXProducto = Fila.Cells["d_SeguroXProducto"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_SeguroXProducto"].Value.ToString());

                                _ventadetalleDto.d_FleteXProducto = Fila.Cells["d_FleteXProducto"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_FleteXProducto"].Value.ToString());

                                _ventadetalleDto.v_Observaciones = Fila.Cells["v_Observaciones"].Value == null ? null : Fila.Cells["v_Observaciones"].Value.ToString();
                                // _ventadetalleDto.v_Observaciones = Fila.Cells["i_IdVentaDetalleAnexo"].Value != null && Fila.Cells["i_IdVentaDetalleAnexo"].Value.ToString() != "-1" ? null : Fila.Cells["v_Observaciones"].Value == null ? null : Fila.Cells["v_Observaciones"].Value.ToString();
                                _ventadetalleDto.i_IdVentaDetalleAnexo = Fila.Cells["i_IdVentaDetalleAnexo"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdVentaDetalleAnexo"].Value.ToString());




                                _ventadetalleDto.v_NroSerie = Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim();
                                _ventadetalleDto.v_NroLote = Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim();
                                _ventadetalleDto.t_FechaCaducidad = Fila.Cells["t_FechaCaducidad"].Value == null ? (DateTime?)null : DateTime.Parse(Fila.Cells["t_FechaCaducidad"].Value.ToString());



                                _TempDetalle_AgregarDto.Add(_ventadetalleDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null &&
                                Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _ventadetalleDto = new ventadetalleDto();
                                _ventadetalleDto.v_IdVentaDetalle = Fila.Cells["v_IdVentaDetalle"].Value == null
                                    ? null
                                    : Fila.Cells["v_IdVentaDetalle"].Value.ToString();
                                _ventadetalleDto.v_IdMovimientoDetalle = Fila.Cells["v_IdMovimientoDetalle"].Value ==
                                                                         null
                                    ? null
                                    : Fila.Cells["v_IdMovimientoDetalle"].Value.ToString();
                                _ventadetalleDto.v_IdVenta = Fila.Cells["v_IdVenta"].Value == null
                                    ? null
                                    : Fila.Cells["v_IdVenta"].Value.ToString();
                                _ventadetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null
                                    ? null
                                    : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _ventadetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _ventadetalleDto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _ventadetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());

                                _ventadetalleDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_ValorVenta"].Text.Replace("_", ""));
                                _ventadetalleDto.d_Igv = Fila.Cells["d_Igv"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_Igv"].Text);
                                _ventadetalleDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_PrecioVenta"].Text);
                                _ventadetalleDto.d_Valor = Fila.Cells["d_Valor"].Value == null ||
                                                           string.IsNullOrEmpty(Fila.Cells["d_Valor"].Text)
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_Valor"].Text);
                                _ventadetalleDto.d_Descuento = Fila.Cells["d_Descuento"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_Descuento"].Text);
                                _ventadetalleDto.d_isc = Fila.Cells["d_isc"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_isc"].Value.ToString());
                                _ventadetalleDto.d_otrostributos = Fila.Cells["d_otrostributos"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString());
                                _ventadetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _ventadetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _ventadetalleDto.i_Anticipio = Fila.Cells["i_Anticipio"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_Anticipio"].Value.ToString());
                                _ventadetalleDto.i_InsertaIdUsuario = Fila.Cells["i_InsertaIdUsuario"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _ventadetalleDto.v_NroCuenta = Fila.Cells["v_NroCuenta"].Value == null
                                    ? null
                                    : Fila.Cells["v_NroCuenta"].Value.ToString().Trim();
                                _ventadetalleDto.i_IdCentroCosto = Fila.Cells["i_IdCentroCosto"].Value == null
                                    ? null
                                    : Fila.Cells["i_IdCentroCosto"].Value.ToString();
                                _ventadetalleDto.i_IdTipoOperacion = Fila.Cells["i_IdTipoOperacion"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_IdTipoOperacion"].Value.ToString());
                                _ventadetalleDto.i_NroUnidades = Fila.Cells["i_NroUnidades"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_NroUnidades"].Value.ToString());
                                _ventadetalleDto.d_PorcentajeComision = Fila.Cells["d_PorcentajeComision"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_PorcentajeComision"].Value.ToString());
                                _ventadetalleDto.i_NroUnidades = Fila.Cells["i_NroUnidades"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_NroUnidades"].Value.ToString());

                                _ventadetalleDto.v_PedidoExportacion = Fila.Cells["v_PedidoExportacion"].Value == null
                                    ? null
                                    : Fila.Cells["v_PedidoExportacion"].Value.ToString().Trim();
                                _ventadetalleDto.v_FacturaRef = Fila.Cells["v_FacturaRef"].Value == null
                                    ? null
                                    : Fila.Cells["v_FacturaRef"].Value.ToString();
                                _ventadetalleDto.v_DescripcionProducto = Fila.Cells["v_DescripcionProducto"].Value ==
                                                                         null
                                    ? null
                                    : Fila.Cells["v_DescripcionProducto"].Value.ToString();
                                _ventadetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _ventadetalleDto.i_InsertaIdUsuario =
                                    int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _ventadetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _ventadetalleDto.d_Percepcion = Fila.Cells["d_Percepcion"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_Percepcion"].Value.ToString());
                                _ventadetalleDto.i_IdMonedaLP = Fila.Cells["i_IdMonedaLP"].Value == null
                                    ? 0
                                    : int.Parse(Fila.Cells["i_IdMonedaLP"].Value.ToString());

                                _ventadetalleDto.d_PrecioPactado = Fila.Cells["d_PrecioPactado"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_PrecioPactado"].Value.ToString());

                                _ventadetalleDto.d_SeguroXProducto = Fila.Cells["d_SeguroXProducto"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_SeguroXProducto"].Value.ToString());

                                _ventadetalleDto.d_FleteXProducto = Fila.Cells["d_FleteXProducto"].Value == null
                                    ? 0
                                    : decimal.Parse(Fila.Cells["d_FleteXProducto"].Value.ToString());


                                _ventadetalleDto.v_Observaciones = Fila.Cells["v_Observaciones"].Value == null ? null : Fila.Cells["v_Observaciones"].Value.ToString();

                                _ventadetalleDto.v_NroSerie = Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim();
                                _ventadetalleDto.v_NroLote = Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim();
                                _ventadetalleDto.t_FechaCaducidad = Fila.Cells["t_FechaCaducidad"].Value == null ? (DateTime?)null : DateTime.Parse(Fila.Cells["t_FechaCaducidad"].Value.ToString());

                                // _ventadetalleDto.v_Observaciones = Fila.Cells["i_IdVentaDetalleAnexo"].Value != null && Fila.Cells["i_IdVentaDetalleAnexo"].Value.ToString() != "-1" ? null : Fila.Cells["v_Observaciones"].Value == null ? null : Fila.Cells["v_Observaciones"].Value.ToString();
                                _ventadetalleDto.i_IdVentaDetalleAnexo = Fila.Cells["i_IdVentaDetalleAnexo"].Value == null ? -1 : int.Parse(Fila.Cells["i_IdVentaDetalleAnexo"].Value.ToString());
                                _TempDetalle_ModificarDto.Add(_ventadetalleDto);
                            }
                            break;
                    }
                }
            }

        }

        private void CalcularValoresDetalle()
        {
            foreach (var fila in grdData.Rows)
            {
                CalcularValoresFila(fila);
            }

            CalcularTotales();
        }


        private void CalcularValoresFila(UltraGridRow fila)
        {
            try
            {
                if (cboIGV.Value.ToString() == "-1")
                {
                    UltraMessageBox.Show("Porfavor seleccione el IGV", "Sistema");
                    return;
                }

                var articulosExportacion = new string[]{Globals.ClientSession.v_IdProductoDetalleFlete,
                    Globals.ClientSession.v_IdProductoDetalleSeguro};

                var detail = (ventadetalleDto)fila.ListObject;
                if (detail.i_Anticipio == null) return;

                if (detail.v_IdProductoDetalle != null && detail.v_IdProductoDetalle == "N002-PE000000000") return;

                var cantOriginal = detail.d_Cantidad ?? 0;
                if (detail.d_Cantidad == null)
                    detail.d_Cantidad = cboDocumento.Value.ToString().Equals("7") ? 1M : 0M;
                else if (detail.d_Cantidad == 0 && cboDocumento.Value.ToString().Equals("7"))
                    detail.d_Cantidad = 1M;

                if (detail.d_Precio == null) detail.d_Precio = 0M;
                if (detail.v_FacturaRef == null) detail.v_FacturaRef = "0";
                if (detail.d_otrostributos == null) detail.d_otrostributos = 0;

                if (detail.d_PrecioPactado == null) detail.d_PrecioPactado = 0;
                if (detail.d_SeguroXProducto == null) detail.d_SeguroXProducto = 0;
                if (detail.d_FleteXProducto == null) detail.d_FleteXProducto = 0;

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
                    #region Calcular flete y seguro

                    if (cboMVenta.Value.ToString() != "-1" && cboMVenta.Value.ToString() != "1" &&
                        !articulosExportacion.Contains(detail.v_IdProductoDetalle))
                    {
                        decimal d;
                        var flete = decimal.TryParse(txtFlete.Text, out d) ? d : 0m;
                        var cantTotal = decimal.TryParse(txtCantidadTotal.Text, out d) ? d : 0m;
                        var seguro = decimal.TryParse(txtSeguro.Text, out d) ? d : 0m;

                        var cantU = detail.d_Cantidad ?? 0;

                        var filasExceptas = grdData.Rows.Where(f => f.Cells["v_IdProductoDetalle"].Value != null &&
                                    (f.Cells["v_IdProductoDetalle"].Value.ToString().Equals(Globals.ClientSession.v_IdProductoDetalleFlete) ||
                                    f.Cells["v_IdProductoDetalle"].Value.ToString().Equals(Globals.ClientSession.v_IdProductoDetalleSeguro)));

                        var esUltimaFila = fila == grdData.Rows.LastOrDefault(f => !filasExceptas.Contains(f));

                        detail.d_FleteXProducto = !esUltimaFila ? Utils.Windows.DevuelveValorRedondeado(cantTotal > 0 ? (flete / cantTotal) * cantU : 0m, 2) :
                                                  flete - grdData.Rows.Where(f => f != fila && !filasExceptas.Contains(f)).Select(p => p.Cells["d_FleteXProducto"].Value == null ? 0m :
                                                      decimal.Parse(p.Cells["d_FleteXProducto"].Value.ToString())).Sum();

                        detail.d_SeguroXProducto = txtSeguro.Enabled ? Utils.Windows.DevuelveValorRedondeado((detail.d_PrecioPactado ?? 0) * (detail.d_Cantidad ?? 0) * 0.0015m, 2) : 0m;
                        detail.d_Precio = cantU > 0 ? ((cantU * (detail.d_PrecioPactado ?? 0)) - detail.d_FleteXProducto - detail.d_SeguroXProducto) / cantU : 0m;
                    }
                    else
                    {
                        detail.d_FleteXProducto = 0m;
                        detail.d_SeguroXProducto = 0m;
                    }

                    #endregion

                    var price = detail.d_Precio.Value;
                    var porcIgv = decimal.Parse(cboIGV.Text) / 100;
                    var esGravado = detail.i_IdTipoOperacion == 1 ||
                                    (detail.i_IdTipoOperacion > 10 && detail.i_IdTipoOperacion < 20);
                    if (esGravado && chkPrecInIGV.Checked) price /= 1 + porcIgv;
                    var isc = CalcularIsc(detail.v_IdProductoDetalle ?? "", price);
                    price -= isc;
                    detail.d_isc = Utils.Windows.DevuelveValorRedondeado(isc * detail.d_Cantidad.Value, 2);
                    detail.d_Valor = Utils.Windows.DevuelveValorRedondeado(detail.d_Cantidad.Value * price, 2);
                    detail.d_Descuento = Utils.Windows.CalcularDescuentosSucesivos(descuentos, detail.d_Valor.Value);

                    if (esGravado && chkPrecInIGV.Checked)
                    {
                        detail.d_PrecioVenta =
                            Utils.Windows.DevuelveValorRedondeado(detail.d_Precio.Value * detail.d_Cantidad.Value, 2);
                        detail.d_PrecioVenta -= Utils.Windows.CalcularDescuentosSucesivos(descuentos,
                            detail.d_PrecioVenta - (detail.d_isc * (1 + porcIgv)) ?? 0);
                        detail.d_PrecioVenta = Utils.Windows.DevuelveValorRedondeado(detail.d_PrecioVenta ?? 0, 2);
                        detail.d_ValorVenta =
                            Utils.Windows.DevuelveValorRedondeado(
                                detail.d_PrecioVenta.Value / (1 + porcIgv) - detail.d_isc.Value, 2);
                        detail.d_Igv =
                            Utils.Windows.DevuelveValorRedondeado(
                                detail.d_PrecioVenta - detail.d_ValorVenta - detail.d_isc ?? 0, 2);

                        detail.d_Valor =
                            Utils.Windows.DevuelveValorRedondeado(detail.d_ValorVenta.Value + detail.d_Descuento.Value,
                                2);
                    }
                    else
                    {
                        detail.d_ValorVenta =
                            Utils.Windows.DevuelveValorRedondeado(detail.d_Valor - detail.d_Descuento ?? 0, 2);
                        detail.d_Igv = esGravado
                            ? Utils.Windows.DevuelveValorRedondeado(
                                (detail.d_ValorVenta.Value + detail.d_isc.Value) * porcIgv, 2)
                            : 0M;
                        detail.d_PrecioVenta =
                            Utils.Windows.DevuelveValorRedondeado(
                                detail.d_ValorVenta + detail.d_Igv + detail.d_otrostributos + detail.d_isc ?? 0, 2);
                    }
                    if (detail.d_otrostributos > 0)
                    {
                        detail.d_PrecioVenta += detail.d_otrostributos;
                        detail.d_PrecioVenta = Utils.Windows.DevuelveValorRedondeado(detail.d_PrecioVenta.Value, 2);
                    }
                }
                detail.d_CantidadEmpaque = GetCantidadEmpaque(detail);
                CalcularTotales();
                if (cboDocumento.Value.ToString().Equals("7"))
                    detail.d_Cantidad = cantOriginal;

                grdData.UpdateData();
                fila.Cells["i_RegistroEstado"].Value = "Modificado";
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(string.Format("{0}\nLinea: {1}", ex.Message,
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '))));
            }

        }

        private decimal GetCantidadEmpaque(ventadetalleDto detail)
        {
            if (detail.i_IdUnidadMedida == null) return 0M;
            if (detail.Empaque == null) detail.d_CantidadEmpaque = 0M;

            if (detail.v_IdProductoDetalle != null && detail.v_IdProductoDetalle != "N002-PE000000000" &&
                detail.i_IdUnidadMedida != -1)
            {
                var totalEmpaque = 0M;
                var empaque = detail.Empaque.Value;
                var cantidad = detail.d_Cantidad.Value;

                var umProducto =
                    ((List<GridKeyValueDTO>)ucUnidadMedida.DataSource).FirstOrDefault(
                        p => p.Id == detail.i_IdUnidadMedidaProducto.ToString());
                var um =
                    ((List<GridKeyValueDTO>)ucUnidadMedida.DataSource).FirstOrDefault(
                        p => p.Id == detail.i_IdUnidadMedida.ToString());

                if (um != null)
                {
                    switch (um.Value1)
                    {
                        case "CAJA":
                            var caja = empaque *
                                       (!string.IsNullOrEmpty(umProducto.Value2) ? decimal.Parse(umProducto.Value2) : 0);
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
                if (grdData.Rows.Count == 0) return;
                decimal SumAntValVenta = 0, SumAntIgv = 0, SumAntTotal = 0, SumAntISC = 0, SumAntOG = 0;
                decimal SumDescuento = 0, SumValVenta = 0, SumIgv = 0, SumTotal = 0, SumVal = 0, SumISC = 0, SumOG = 0;
                decimal sumSeguro = 0;

                foreach (UltraGridRow fila in grdData.Rows)
                {
                    var objRow = (ventadetalleDto)fila.ListObject;
                    if (objRow.i_IdTipoOperacion > 10) continue; // Operaciones no Onerosas
                    if (objRow.d_Valor == null) objRow.d_Valor = 0;
                    if (objRow.d_ValorVenta == null) objRow.d_ValorVenta = 0;
                    if (objRow.d_Igv == null) objRow.d_Igv = 0;
                    if (objRow.d_PrecioVenta == null) objRow.d_PrecioVenta = 0;
                    if (objRow.d_Descuento == null) objRow.d_Descuento = 0;
                    if (objRow.d_isc == null) objRow.d_isc = 0;
                    if (objRow.d_otrostributos == null) objRow.d_otrostributos = 0;
                    if (objRow.i_Anticipio == null) objRow.i_Anticipio = 0;

                    SumVal = SumVal + objRow.d_Valor.Value;
                    sumSeguro += objRow.d_SeguroXProducto ?? 0m;

                    switch (fila.Cells["i_Anticipio"].Value.ToString())
                    {
                        case "1":
                            SumAntValVenta += objRow.d_ValorVenta.Value;
                            SumAntIgv += objRow.d_Igv.Value;
                            SumAntTotal += objRow.d_PrecioVenta.Value;
                            SumDescuento += objRow.d_Descuento.Value;
                            SumAntISC += objRow.d_isc.Value;
                            SumAntOG += objRow.d_otrostributos.Value;
                            break;

                        case "0":
                            SumValVenta += objRow.d_ValorVenta.Value;
                            SumIgv += objRow.d_Igv.Value;
                            SumTotal += objRow.d_PrecioVenta.Value;
                            SumDescuento += objRow.d_Descuento.Value;
                            SumISC += objRow.d_isc.Value;
                            SumOG += objRow.d_otrostributos.Value;
                            break;
                    }
                }


                //Descuento Global si existe: 
                decimal porcDecuento = decimal.TryParse(txtPorcDescuento.Text, out porcDecuento) ? porcDecuento / 100 : 0;
                decimal d;
                var factDesc = 1 - porcDecuento;
                txtAnticipio.Text = SumAntValVenta.ToString("0.00");
                txtValor.Text = SumVal.ToString("0.00");
                var ValVenta = SumValVenta - SumAntValVenta;
                txtDescuento.Text = (SumDescuento + Utils.Windows.DevuelveValorRedondeado(ValVenta * porcDecuento, 2)).ToString("0.00");
                txtValorVenta.Text = (ValVenta * factDesc).ToString("0.00");
                txtIGV.Text = ((SumIgv - SumAntIgv) * factDesc).ToString("0.00");
                txtISC.Text = ((SumISC - SumAntISC) * factDesc).ToString("0.00");
                var porcentajePercepcion = decimal.TryParse(txtPercepcionPorcentaje.Text, out d) ? d : 0;
                var percepcion = porcentajePercepcion > 0 ? ((SumTotal - SumAntTotal) * factDesc) * porcentajePercepcion / 100 : 0m;
                var total = ((SumTotal - SumAntTotal) * factDesc) + percepcion;
                txtPercepcion.Text = string.Format("{0:##,###.00}", percepcion);
                txtTotal.Text = total.ToString("0.00");
                txtSeguro.Text = sumSeguro.ToString("0.00");
                txtOtrosTributos.Text = (SumOG - SumAntOG).ToString("0.00");
                Redondeado = decimal.Parse(Math.Round(total, 1, MidpointRounding.AwayFromZero).ToString("0.00"));
                Residuo = (total - Redondeado) * -1;
                _Redondeado = Residuo == 0;
                btnRedondear.Enabled = Residuo != 0;

                var filasPercepcion =
                    grdData.Rows.Where(
                        p =>
                            p.Cells["i_EsAfectoPercepcion"].Value != null &&
                            p.Cells["i_EsAfectoPercepcion"].Value.ToString() == "1").ToArray();
                if (cboDocumento.Value.ToString() == "3" || cboDocumento.Value.ToString() == "1")
                {
                    filasPercepcion =
                        filasPercepcion.Where(
                            p =>
                                cboDocumento.Value.ToString() == "3"
                                    ? decimal.Parse(p.Cells["d_PrecioVenta"].Value.ToString()) > 700
                                    : decimal.Parse(p.Cells["d_PrecioVenta"].Value.ToString()) > 0).ToArray();
                    if (filasPercepcion.Length > 0)
                    {
                        Array.ForEach(filasPercepcion,
                            o =>
                                o.Cells["d_Percepcion"].Value =
                                    o.Cells["d_PrecioVenta"].Value != null
                                        ? (decimal.Parse(o.Cells["d_PrecioVenta"].Value.ToString()) *
                                           (decimal.Parse(o.Cells["d_TasaPercepcion"].Value.ToString()) / 100)).ToString()
                                        : "0");
                        txtTotal.Text =
                            (decimal.Parse(txtTotal.Text) +
                             filasPercepcion.Sum(p => decimal.Parse(p.Cells["d_Percepcion"].Value.ToString()))).ToString
                                ("0.00");
                    }
                    else
                        Array.ForEach(filasPercepcion, o => o.Cells["d_Percepcion"].Value = "0");
                }
                else
                {
                    Array.ForEach(filasPercepcion, o => o.Cells["d_Percepcion"].Value = "0");
                    txtTotal.Text = (decimal.Parse(txtTotal.Text)).ToString("0.00");
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(string.Format("{0}\nLinea: {1}", ex.Message,
                    ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' '))));
            }
        }

        private void ComprobarExistenciaCorrelativoDocumento()
        {
            var objOperationResult = new OperationResult();
            if (
                _objVentasBL.ComprobarExistenciaCorrelativoDocumento(ref objOperationResult,
                    int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text, txtCorrelativoDocIni.Text,
                    _ventaDto.v_IdVenta) == true)
            {

                UltraMessageBox.Show("El documento ya ha sido registrado anteriormente", "Error de Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        txtRucCliente.Text = ventaRefDto.NroDocCliente;
                        txtRazonSocial.Text = ventaRefDto.NombreCliente;
                        dtpFechaOrden.Value = ventaRefDto.t_FechaOrdenCompra.Value;
                        dtpFechaRef.Value = ventaRefDto.t_FechaRegistro.Value;
                        cboCondicionPago.Value = ventaRefDto.i_IdCondicionPago.ToString();
                        cboEstablecimiento.Value = ventaRefDto.i_IdEstablecimiento.ToString();
                        cboEstado.Value = ventaRefDto.i_IdEstado.ToString();
                        cboIGV.Value = ventaRefDto.i_IdIgv.ToString();
                        cboMoneda.Value = ventaRefDto.i_IdMoneda.ToString();
                        cboMVenta.Value = "-1";
                        cboPuntoDestino.Value = ventaRefDto.i_IdPuntoDestino.ToString();
                        cboPuntoEmbarque.Value = ventaRefDto.i_IdPuntoEmbarque.ToString();
                        cboTipoEmbarque.Value = ventaRefDto.i_IdTipoEmbarque.ToString();
                        cboTipoOperacion.Value = ventaRefDto.i_IdTipoOperacion.ToString();
                        cboTipoVenta.Value = int.Parse(ventaRefDto.i_IdTipoVenta.ToString()).ToString("00");
                        if (!string.IsNullOrWhiteSpace(ventaRefDto.v_IdVendedor))
                            cboVendedor.Value = ventaRefDto.v_IdVendedor.ToString();
                        cboVendedorRef.Value = ventaRefDto.v_IdVendedorRef;
                        chkAfectoIGV.Checked = ventaRefDto.i_EsAfectoIgv == 1;
                        chkDeduccionAnticipio.Checked = ventaRefDto.i_DeduccionAnticipio == 1;
                        chkDrawnBack.Checked = ventaRefDto.i_DrawBack == 1;
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
                        var msg = MessageBox.Show("No se ha registrado el documento de referencia, ¿Desea continuar?",
                            "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                        if (msg == DialogResult.Yes)
                        {
                            //LimpiarCabecera();
                            CargarDetalle("");
                            btnGuardar.Enabled = false;
                        }
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

            //UltraGridColumn c;
            //c = grdData.DisplayLayout.Bands[0].Columns["i_IdAlmacen"];
            //c.CellActivation = Activation.ActivateOnly;
            //c.CellClickAction = CellClickAction.CellSelect;

            //c = grdData.DisplayLayout.Bands[0].Columns["d_Cantidad"];
            //c.CellActivation = Activation.ActivateOnly;
            //c.CellClickAction = CellClickAction.CellSelect;

            //c = grdData.DisplayLayout.Bands[0].Columns["d_Precio"];
            //c.CellActivation = Activation.ActivateOnly;
            //c.CellClickAction = CellClickAction.CellSelect;

            //c = grdData.DisplayLayout.Bands[0].Columns["i_IdUnidadMedida"];
            //c.CellActivation = Activation.ActivateOnly;
            //c.CellClickAction = CellClickAction.CellSelect;

            //c = grdData.DisplayLayout.Bands[0].Columns["d_PrecioVenta"];
            //c.CellActivation = Activation.ActivateOnly;
            //c.CellClickAction = CellClickAction.CellSelect;

            //c = grdData.DisplayLayout.Bands[0].Columns["i_NroUnidades"];
            //c.CellActivation = Activation.ActivateOnly;
            //c.CellClickAction = CellClickAction.CellSelect;

            //c = grdData.DisplayLayout.Bands[0].Columns["v_CodigoInterno"];
            //c.CellActivation = Activation.ActivateOnly;
            //c.CellClickAction = CellClickAction.CellSelect;

            //if (Globals.ClientSession.UsuarioEsContable == 1) return;
            //btnAgregar.Enabled = false;
            //btnEliminar.Enabled = false;
            //btnBuscarDetraccion.Enabled = false;
            //txtRucCliente.Enabled = false;
            //cboCondicionPago.Enabled = false;
            //cboDocumento.Enabled = false;
            //cboDocumentoRef.Enabled = false;
            //cboEstablecimiento.Enabled = false;
            ////cboEstado.Enabled = false;
            //cboIGV.Enabled = false;
            //cboMoneda.Enabled = false;
            //cboMVenta.Enabled = false;
            //cboPuntoDestino.Enabled = false;
            //cboPuntoEmbarque.Enabled = false;
            //cboTipoEmbarque.Enabled = false;
            //cboTipoOperacion.Enabled = false;
            //cboTipoVenta.Enabled = false;
            //cboVendedor.Enabled = false;
            //cboVendedorRef.Enabled = false;
            //txtAnticipio.Enabled = false;
            //txtCodigoCliente.Enabled = false;
            //txtConcepto.Enabled = false;
            //txtCorrelativo.Enabled = false;
            //txtCorrelativoDocFin.Enabled = false;
            //txtCorrelativoDocIni.Enabled = false;
            //txtCorrelativoDocRef.Enabled = false;
            //txtDescripcionColumnas.Enabled = false;
            //txtDimensiones.Enabled = false;
            //txtDireccion.Enabled = false;
            //txtGuiaRemisionSerie.Enabled = false;
            //txtIGV.Enabled = false;
            //txtMarca.Enabled = false;
            //txtMes.Enabled = false;
            //txtNroBulto.Enabled = false;
            //txtNroDias.Enabled = false;
            //txtNroPedido.Enabled = false;
            //txtOrdenCompra.Enabled = false;
            //txtPeriodo.Enabled = false;
            //txtPesoBrutoKg.Enabled = false;
            //txtPesoNetoKg.Enabled = false;
            //txtPorcComision.Enabled = false;
            //txtPorcDescuento.Enabled = false;
            //txtRazonSocial.Enabled = false;
            //txtRucCliente.Enabled = false;
            //txtSerieDoc.Enabled = false;
            //txtTipoCambio.Enabled = false;
            //chkAfectoIGV.Enabled = false;
            //chkDeduccionAnticipio.Enabled = false;
            //chkDrawnBack.Enabled = false;
            //chkPrecInIGV.Enabled = false;
            //txtGuiaRemisionCorrelativo.Enabled = false;
            //dtpFechaOrden.Enabled = false;
            //dtpFechaRef.Enabled = false;
            //dtpFechaRegistro.Enabled = false;
            //dtpFechaVencimiento.Enabled = false;
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
                _ventaDto.i_IdDireccionCliente = -1;
                txtCodigoCliente.Text = Cadena[1];
                txtRazonSocial.Text = Cadena[2];
                txtRucCliente.Text = Cadena[3];
                txtRazonSocial.Enabled = true;
                txtDireccion.Clear();
                txtDireccion.Enabled = true;
            }
            else
            {
                if (
                    UltraMessageBox.Show(
                        "El Cliente Público General no está definido en la Base de Datos, ¿Desea Agregarlo?", "Sistema",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
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
                var row = grdData.DisplayLayout.Bands[0].AddNew();
                if (row == null) return;
                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["d_Cantidad"].Value = "1";
                row.Cells["d_Precio"].Value = Residuo.ToString();
                row.Cells["i_IdUnidadMedida"].Value = "-1";
                row.Cells["i_Anticipio"].Value = "0";
                row.Cells["d_Descuento"].Value = "0";
                row.Cells["i_IdCentroCosto"].Value = "0";
                row.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString() == "5"
                    ? "1"
                    : cboTipoOperacion.Value.ToString();
                row.Cells["d_Valor"].Value = Residuo.ToString();
                row.Cells["d_ValorVenta"].Value = Residuo.ToString();
                row.Cells["EsRedondeo"].Value = "1";
                row.Cells["d_PrecioVenta"].Value = Residuo.ToString();
                row.Cells["v_NroCuenta"].Value = Residuo < 0
                    ? Globals.ClientSession.v_NroCuentaRedondeoPerdida
                    : Globals.ClientSession.v_NroCuentaRedondeoGanancia;
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
                row.Cells["t_FechaCaducidad"].Value = (DateTime?)null;
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
                    grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = centroDeCosto;
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
                    grdData.ActiveRow.Cells["t_FechaCaducidad"].Value = Filas[i].Cells["t_FechaCaducidad"].Value == null ? (DateTime?)null : Filas[i].Cells["t_FechaCaducidad"].Value;

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
                    grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = centroDeCosto;
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
                    grdData.ActiveRow.Cells["t_FechaCaducidad"].Value = Filas[i].Cells["t_FechaCaducidad"].Value == null ? (DateTime?)null : Filas[i].Cells["t_FechaCaducidad"].Value;

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
            if (
                grdData.Rows.Count(
                    p => p.Cells["v_NroCuenta"].Value == null || p.Cells["v_NroCuenta"].Value.ToString().Trim() == "-1") !=
                0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente todas las cuentas al detalle.", "Sistema",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row =
                    grdData.Rows.FirstOrDefault(
                        x =>
                            x.Cells["v_NroCuenta"].Value == null ||
                            x.Cells["v_NroCuenta"].Value.ToString().Trim() == "-1");
                grdData.Selected.Cells.Add(Row.Cells["v_NroCuenta"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_NroCuenta"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            //if (cboDocumento.Value.ToString() != "7" && cboDocumento.Value.ToString() != "8")
            if (!_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) &&
                cboDocumento.Value.ToString() != "8")
            {
                if (
                    grdData.Rows.Count(
                        p =>
                            (p.Cells["i_Anticipio"].Value.ToString() == "0") &&
                            (p.Cells["d_Cantidad"].Value == null ||
                             decimal.Parse(p.Cells["d_Cantidad"].Value.ToString().Trim()) <= 0)) != 0)
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente la Cantidad.", "Sistema", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    UltraGridRow Row =
                        grdData.Rows.FirstOrDefault(
                            p =>
                                (p.Cells["i_Anticipio"].Value.ToString() == "0") &&
                                (p.Cells["d_Cantidad"].Value == null ||
                                 decimal.Parse(p.Cells["d_Cantidad"].Value.ToString().Trim()) <= 0));
                    grdData.Selected.Cells.Add(Row.Cells["d_Cantidad"]);
                    grdData.Focus();
                    Row.Activate();
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_Cantidad"];
                    this.grdData.ActiveCell = aCell;
                    return false;
                }
            }

            if (
                grdData.Rows.Count(
                    p =>
                        (p.Cells["i_Anticipio"].Value.ToString() == "0") &&
                        (p.Cells["d_Precio"].Value == null ||
                         decimal.Parse(p.Cells["d_Precio"].Value.ToString().Trim()) == 0)) != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente el Precio.", "Sistema", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                UltraGridRow Row =
                    grdData.Rows.FirstOrDefault(
                        p =>
                            (p.Cells["i_Anticipio"].Value.ToString() == "0") &&
                            (p.Cells["d_Precio"].Value == null ||
                             decimal.Parse(p.Cells["d_Precio"].Value.ToString().Trim()) == 0));
                grdData.Selected.Cells.Add(Row.Cells["d_Precio"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_Precio"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (
                grdData.Rows.Count(
                    p => p.Cells["i_IdAlmacen"].Value == null || p.Cells["i_IdAlmacen"].Value.ToString().Trim() == "-1") !=
                0)
            {
                UltraMessageBox.Show("Por favor un almacén.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row =
                    grdData.Rows.FirstOrDefault(
                        x =>
                            x.Cells["i_IdAlmacen"].Value == null ||
                            x.Cells["i_IdAlmacen"].Value.ToString().Trim() == "-1");
                grdData.Selected.Cells.Add(Row.Cells["i_IdAlmacen"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdAlmacen"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (
                grdData.Rows.Count(
                    p =>
                        p.Cells["i_IdUnidadMedida"].Value == null ||
                        p.Cells["i_IdUnidadMedida"].Value.ToString().Trim() == "-1") != 0)
            {
                UltraMessageBox.Show("Por favor una Unidad de Medida.", "Sistema", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                UltraGridRow Row =
                    grdData.Rows.FirstOrDefault(
                        x =>
                            x.Cells["i_IdUnidadMedida"].Value == null ||
                            x.Cells["i_IdUnidadMedida"].Value.ToString().Trim() == "-1");
                grdData.Selected.Cells.Add(Row.Cells["i_IdUnidadMedida"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdUnidadMedida"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (
                grdData.Rows.Where(
                    p =>
                        p.Cells["v_IdProductoDetalle"].Value != null &&
                        p.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000" &&
                        (p.Cells["i_IdTipoOperacion"].Value == null ||
                         p.Cells["i_IdTipoOperacion"].Value.ToString() == "-1" ||
                         p.Cells["i_IdTipoOperacion"].Value.ToString() == "5")).Count() != 0)
            {
                UltraMessageBox.Show("Porfavor especifique los tipos de operación correctamente", "Sistema",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row =
                    grdData.Rows.Where(
                        p =>
                            p.Cells["v_IdProductoDetalle"].Value != null &&
                            p.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000" &&
                            (p.Cells["i_IdTipoOperacion"].Value == null ||
                             p.Cells["i_IdTipoOperacion"].Value.ToString() == "-1" ||
                             p.Cells["i_IdTipoOperacion"].Value.ToString() == "5")).FirstOrDefault();
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
                        (!string.IsNullOrEmpty(p.Cells["v_NroCuenta"].Text) &&
                         !p.Cells["v_NroCuenta"].Text.Contains("ANTICIPIO"))))
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

            var FilasSinFechaVencimientoLote = grdData.Rows.Where(p => p.Cells["i_SolicitarNroLoteSalida"].Value != null && p.Cells["i_SolicitarNroLoteSalida"].Value.ToString() == "1" && p.Cells["t_FechaCaducidad"].Value != null && (DateTime.Parse(p.Cells["t_FechaCaducidad"].Value.ToString()) == DateTime.MinValue || DateTime.Parse(p.Cells["t_FechaCaducidad"].Value.ToString()).ToShortDateString() == Constants.FechaNula)).ToList();

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


            var FilasSinFechaVencimientoLote2 = grdData.Rows.Where(p => p.Cells["i_SolicitarNroLoteSalida"].Value != null && p.Cells["i_SolicitarNroLoteSalida"].Value.ToString() == "1" && p.Cells["t_FechaCaducidad"].Value == null).ToList();

            if (FilasSinFechaVencimientoLote2.Any())
            {
                UltraMessageBox.Show("Por favor registre la fecha de vencimiento para producto", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                UltraGridRow Fila = FilasSinFechaVencimientoLote2.FirstOrDefault();
                grdData.Focus();
                Fila.Activate();
                Fila.Cells["t_FechaCaducidad"].Activate();
                grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown, true, false);
                return false;
            }



            return true;
        }

        #endregion

        #region Eventos de la Grilla

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].Columns["i_Anticipio"].Style =
                Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].EditorComponent = ucUnidadMedida;
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].EditorComponent = ucAlmacen;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdTipoOperacion"].EditorComponent = ucTipoOperacion;
            e.Layout.Bands[0].Columns["i_IdTipoOperacion"].Style =
                Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
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

            #region Ctrl + H

            if (e.Control && e.KeyCode == Keys.H)
            {
                frmBuscarProducto frm =
                    new frmBuscarProducto(int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()),
                        "PedidoVenta", _ventaDto.v_IdCliente,
                        grdData.ActiveRow.Cells["v_CodigoInterno"].Text ?? string.Empty,
                        UserConfig.Default.appTipoBusquedaPredeterminadaProducto, true);
                frm.ShowDialog();
                e.Handled = true;
                return;
            }

            #endregion

            if (grdData.ActiveCell.Column.Key == "v_DescripcionProducto" &&
                grdData.ActiveRow.Cells["EsNombreEditable"].Value != null &&
                grdData.ActiveRow.Cells["i_EsNombreEditable"].Value.ToString() == "0")
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

            if (e.KeyCode == Keys.Up | e.KeyCode == Keys.Down)
            {
                if (grdData.ActiveCell.Column.Key == "i_IdAlmacen" &&
                    grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null)
                {
                    e.SuppressKeyPress = true;
                    return;
                }
            }

            if (grdData.ActiveCell.Column.Key != "i_IdAlmacen" && grdData.ActiveCell.Column.Key != "i_IdUnidadMedida" &&
                grdData.ActiveCell.Column.Key != "i_IdTipoOperacion" && grdData.ActiveCell.Column.Key != "v_NroCuenta")
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
            if (grdData.ActiveCell != null && grdData.ActiveCell.Column.Key == "v_NroLote")
            {
                if (grdData.ActiveRow.Cells["i_SolicitarNroLoteSalida"].Value != null && grdData.ActiveRow.Cells["i_SolicitarNroLoteSalida"].Value.ToString() == "1")
                    e.SuppressKeyPress = true;
            }
            if (grdData.ActiveCell != null && grdData.ActiveCell.Column.Key == "v_NroSerie")
            {
                if (grdData.ActiveRow.Cells["i_SolicitarNroSerieSalida"].Value != null && grdData.ActiveRow.Cells["i_SolicitarNroSerieSalida"].Value.ToString() == "1")
                    e.SuppressKeyPress = true;
            }
        }

        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            if (grdData.ActiveRow == null) return;
            switch (e.Cell.Column.Key)
            {
                case "v_CodigoInterno":
                    //if (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) || cboDocumento.Value.ToString() == "8" || e.Cell.Activation == Activation.Disabled) return;

                    if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "Temporal")
                    {
                        if (grdData.ActiveRow.Cells["i_IdAlmacen"].Value == null) return;
                        var row = grdData.ActiveRow;
                        if (e.Cell.Text != "" && _objVentasBL.EsValidoCodProducto(e.Cell.Text))
                        {
                            productoshortDto prod = Globals.ClientSession.i_UsaListaPrecios == 1
                                ? string.IsNullOrEmpty(_ventaDto.v_IdCliente) ? _objVentasBL.DevolverArticuloPorCodInternoLista1(e.Cell.Text, _ventaDto.v_IdCliente,
                                    int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString())) : _objVentasBL.DevolverArticuloPorCodInterno(e.Cell.Text, _ventaDto.v_IdCliente,
                                    int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()))
                                : _objVentasBL.DevolverArticuloPorCodInterno(e.Cell.Text,
                                    int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()));

                            if (prod != null)
                            {
                                row.Cells["v_DescripcionProducto"].Value = prod.v_Descripcion;
                                row.Cells["v_IdProductoDetalle"].Value = prod.v_IdProductoDetalle;
                                row.Cells["v_CodigoInterno"].Value = prod.v_CodInterno;
                                row.Cells["Empaque"].Value = prod.d_Empaque;
                                row.Cells["UMEmpaque"].Value = prod.EmpaqueUnidadMedida;
                                row.Cells["i_RegistroEstado"].Value = "Modificado";
                                row.Cells["i_IdUnidadMedida"].Value = prod.i_IdUnidadMedida;
                                row.Cells["i_IdCentroCosto"].Value = centroDeCosto;
                                row.Cells["i_IdUnidadMedidaProducto"].Value = prod.i_IdUnidadMedida;
                                row.Cells["i_EsAfectoDetraccion"].Value = prod.i_EsAfectoDetraccion;
                                row.Cells["i_IdTipoOperacion"].Value = (string)cboTipoOperacion.Value != "5"
                                    ? cboTipoOperacion.Value.ToString()
                                    : "1";
                                row.Cells["d_isc"].Value = "0";
                                row.Cells["d_otrostributos"].Value = "0";
                                row.Cells["d_Precio"].Value = DevolverPrecioProducto(prod.IdMoneda, prod.d_Precio ?? 0,
                                    int.Parse(cboMoneda.Value.ToString()),
                                    !string.IsNullOrEmpty(txtTipoCambio.Text.Trim())
                                        ? decimal.Parse(txtTipoCambio.Text.Trim())
                                        : 0);

                                row.Cells["i_EsNombreEditable"].Value = prod.i_NombreEditable;
                                row.Cells["i_EsServicio"].Value = prod.i_EsServicio;
                                row.Cells["v_NroCuenta"].Value = prod.NroCuentaVenta;
                                row.Cells["i_ValidarStock"].Value = prod.i_ValidarStock ?? 0;
                                row.Cells["i_EsAfectoPercepcion"].Value = prod.i_EsAfectoPercepcion ?? 0;
                                row.Cells["d_TasaPercepcion"].Value = prod.d_TasaPercepcion;
                                row.Cells["i_IdMonedaLP"].Value = prod.IdMoneda;
                                row.Cells["d_Cantidad"].Value = UserConfig.Default.CantVenta;
                                row.Cells["v_PedidoExportacion"].Value = prod.v_NroPedidoExportacion;

                                row.Cells["i_SolicitarNroSerieSalida"].Value = prod.i_SolicitarNroSerieSalida;
                                row.Cells["i_SolicitarNroLoteSalida"].Value = prod.i_SolicitarNroLoteSalida;

                                if (prod.EsAfectoIsc)
                                    AddIsc(prod.v_IdProductoDetalle);
                            }
                            else
                            {
                                UltraMessageBox.Show("El Artículo existe Pero no tuvo ingresos a este almacén o está inactivo", "",
                                    MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                btnEliminar_Click(sender, e);
                            }
                        }
                        else
                        {
                            var frm =
                                new frmBuscarProducto(
                                    int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()), "PedidoVenta",
                                    _ventaDto.v_IdCliente,
                                    grdData.ActiveRow.Cells["v_CodigoInterno"].Text ?? string.Empty,
                                    UserConfig.Default.appTipoBusquedaPredeterminadaProducto);
                            frm.ShowDialog();
                            if (frm._NombreProducto != null)
                            {
                                var IdAlmacen = int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString());

                                row.Cells["v_DescripcionProducto"].Value = frm._NombreProducto;
                                row.Cells["v_IdProductoDetalle"].Value = frm._IdProducto;
                                row.Cells["v_CodigoInterno"].Value = frm._CodigoInternoProducto;
                                row.Cells["Empaque"].Value = frm._Empaque.ToString(CultureInfo.CurrentCulture);
                                row.Cells["UMEmpaque"].Value = frm._UnidadMedida ?? string.Empty;
                                row.Cells["i_RegistroEstado"].Value = "Modificado";
                                row.Cells["i_IdCentroCosto"].Value = centroDeCosto;
                                row.Cells["d_isc"].Value = "0";
                                row.Cells["d_otrostributos"].Value = "0";
                                row.Cells["i_IdUnidadMedida"].Value = frm._UnidadMedidaEmpaque;
                                row.Cells["i_IdTipoOperacion"].Value = (string)cboTipoOperacion.Value != "5"
                                    ? cboTipoOperacion.Value.ToString()
                                    : "1";
                                row.Cells["i_IdUnidadMedidaProducto"].Value = frm._UnidadMedidaEmpaque;
                                row.Cells["i_EsAfectoDetraccion"].Value = frm._EsAfectoDetraccion;
                                row.Cells["d_Precio"].Value = DevolverPrecioProducto(frm._IdMoneda, frm._PrecioUnitario,
                                    int.Parse(cboMoneda.Value.ToString()),
                                    !string.IsNullOrEmpty(txtTipoCambio.Text.Trim())
                                        ? decimal.Parse(txtTipoCambio.Text.Trim())
                                        : 0);
                                row.Cells["d_Cantidad"].Value = UserConfig.Default.CantVenta;
                                row.Cells["i_EsNombreEditable"].Value = frm._NombreEditable;
                                row.Cells["i_EsServicio"].Value = frm._EsServicio.ToString();
                                row.Cells["v_NroCuenta"].Value = frm._NroCuentaVenta;
                                row.Cells["i_ValidarStock"].Value = frm._ValidarStock;
                                row.Cells["i_EsAfectoPercepcion"].Value = frm._EsAfectoPercepcion;
                                row.Cells["d_TasaPercepcion"].Value = frm._TasaPercepcion;
                                row.Cells["i_IdMonedaLP"].Value = frm._IdMoneda;
                                row.Cells["v_PedidoExportacion"].Value = frm._NroPedidoExportacion;
                                if (frm._EsAfectoisc)
                                    AddIsc(frm._IdProducto);

                            }
                        }

                        if (grdData.Rows.Any())
                        {
                            if (grdData.ActiveRow == null) return;
                            var aCell = grdData.ActiveRow.Cells["d_Cantidad"];
                            grdData.Rows[e.Cell.Row.Index].Activate();
                            grdData.ActiveCell = aCell;
                            grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                            grdData.Focus();
                        }
                    }
                    break;

                case "i_IdCentroCosto":
                    var frm2 = new frmBuscarDatahierarchy(31, "Buscar Centro de Costos");
                    frm2.ShowDialog();
                    if (frm2._itemId != null)
                    {
                        grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = frm2._value2;
                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                    }
                    break;
            }
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
                                @"Al escoger un rubro anticipio sólo podrá estar este único como detalle, ¿Desea Continuar?",
                                @"Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
                var editor = e.Cell.EditorResolved;
                e.Cell.Value = editor.Value;
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

                    case "d_PorcentajeComision":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDecimalCelda(Celda, e);
                        break;

                    case "v_FacturaRef":
                        Celda = grdData.ActiveCell;
                        //Utils.Windows.NumeroDocumentoCelda(Celda, e);
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

            if (e.Cell.Column.Key == "d_PrecioVenta" || e.Cell.Column.Key == "d_Cantidad" ||
                e.Cell.Column.Key == "d_Precio" || e.Cell.Column.Key == "v_DescripcionProducto" ||
                e.Cell.Column.Key == "v_NroCuenta" || e.Cell.Column.Key == "i_IdDestino" ||
                e.Cell.Column.Key == "i_IdCentroCostos" || e.Cell.Column.Key == "i_IdAlmacen")
            {
                switch (e.Cell.Column.Key)
                {
                    case "v_NroCuenta":
                        txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_NombreCuenta"].Value != null
                            ? grdData.ActiveRow.Cells["_NombreCuenta"].Value.ToString()
                            : string.Empty;
                        if (grdData.ActiveRow.Cells["EsRedondeo"].Value != null)
                            e.Cell.CancelUpdate();
                        break;

                    case "i_IdDestino":
                        txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_Destino"].Value != null
                            ? grdData.ActiveRow.Cells["_Destino"].Value.ToString()
                            : string.Empty;
                        break;

                    case "i_IdCentroCostos":
                        txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_CentroCosto"].Value != null
                            ? grdData.ActiveRow.Cells["_CentroCosto"].Value.ToString()
                            : string.Empty;
                        break;

                    case "i_IdAlmacen":
                        txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_Almacen"].Value != null
                            ? grdData.ActiveRow.Cells["_Almacen"].Value.ToString()
                            : string.Empty;
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
            if (strModo == "Edicion")
            {
                btnAgregar.Enabled = ExtraccionPedido || !string.IsNullOrEmpty(txtNroPedido.Text) ? false : true;
            }
            if (strModo != "Consulta")
            {
                if (!string.IsNullOrEmpty(_ventaDto.v_NroPedido)) return;
                if (
                    grdData.Rows.ToList()
                        .Any(
                            f =>
                                f.Cells["v_NroCuenta"].Value != null &&
                                f.Cells["v_NroCuenta"].Text.Contains("ANTICIPIO"))) return;

                btnAgregar.Enabled = ExtraccionPedido || !string.IsNullOrEmpty(txtNroPedido.Text) ? false : true;
                //btnAgregar.Enabled =  true;
                Point point = new System.Drawing.Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement =
                    ((UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null) return;

                Infragistics.Win.UltraWinGrid.UltraGridRow row =
                    (Infragistics.Win.UltraWinGrid.UltraGridRow)
                        uiElement.GetContext(typeof(Infragistics.Win.UltraWinGrid.UltraGridRow));

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
            var columnasAfectasCalculo = new string[] { "d_isc", "d_otrostributos", "d_Cantidad", "d_Precio", "d_PrecioPactado" };
            //var columnasAfectasCalculo = new string[] { "d_isc", "d_otrostributos", "d_Cantidad", "d_Precio", "d_PrecioPactado", "CantidadExportacion", "KgExportacion" };
            if (columnasAfectasCalculo.Contains(grdData.ActiveCell.Column.Key))
            {
                CalcularValoresFila(grdData.ActiveRow);
            }

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

                    if (grdData.ActiveCell.Value == null ||
                        !decimal.TryParse(grdData.ActiveCell.Value.ToString(), out precioVenta)) return;
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            if (e.Row.Cells["EsRedondeo"].Value != null)
            {
                e.Row.Appearance.ForeColor = Color.Red;
            }
            else if (e.Row.Cells["i_EsAfectoPercepcion"].Value != null &&
                     e.Row.Cells["i_EsAfectoPercepcion"].Value.ToString() == "1")
            {
                e.Row.Appearance.ForeColor = Color.Black;
                e.Row.Appearance.FontData.Bold = Infragistics.Win.DefaultableBoolean.True;
            }
            if (e.Row.Band.Index == 0 && e.Row.Cells["t_FechaCaducidad"].Value != null && DateTime.Parse(e.Row.Cells["t_FechaCaducidad"].Value.ToString()).Date.ToShortDateString() == Constants.FechaNula)
            {
                e.Row.Cells["t_FechaCaducidad"].Appearance.BackColor = Color.White;
                e.Row.Cells["t_FechaCaducidad"].Appearance.ForeColor = Color.White;
            }

        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(_ventaDto.v_NroPedido)) return;
            if (strModo != "Consulta")
                btnEliminar.Enabled = grdData.ActiveRow != null;
            else
                btnEliminar.Enabled = false;
        }

        #endregion

        private void txtRucCliente_EditorButtonClick(object sender,
            Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnConsultar")
            {
                if (txtRucCliente.Text.Trim().Length == 11 && Utils.Windows.ValidarRuc(txtRucCliente.Text.Trim()))
                {
                    string[] _Contribuyente = new string[10];

                    frmCustomerCapchaSUNAT frm = new frmCustomerCapchaSUNAT(txtRucCliente.Text.Trim());
                    frm.ShowDialog();
                    if (frm.ConectadoRecibido)
                    {
                        _Contribuyente = frm.DatosContribuyente;

                        lblRevisionRUC.Text = string.Format("Estado: {0} | Condición: {1}", _Contribuyente[3],
                            _Contribuyente[4]);
                        ClienteBL.ActualizarDireccionCliente(_ventaDto.v_IdCliente, _Contribuyente[5]);
                        txtDireccion.Text = _Contribuyente[5].Trim();
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
                    // cboTipoOperacion.Enabled = false;
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

        private void txtValorFOB_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtValorFOB, e);
        }

        private void txtRucCliente_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (!txtRucCliente.IsDroppedDown && e.KeyCode == Keys.Enter)
            {
                if (txtRucCliente.Text.Trim() != "" && txtRucCliente.Text.Length <= 5)
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
                        //TipoPersona = frm._TipoPersona;
                        VerificarLineaCreditoCliente(_ventaDto.v_IdCliente);

                    }
                }
                else if (txtRucCliente.Text.Length == 11 || txtRucCliente.Text.Length == 8)
                {
                    if (cboDocumento.Value.ToString() == "1" && txtRucCliente.Text.Length == 8)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _ventaDto.v_IdCliente = null;
                        _ventaDto.i_IdDireccionCliente = -1;

                        txtCodigoCliente.Clear();
                        txtRazonSocial.Clear();
                        txtDireccion.Clear();
                        IdIdentificacion = 0;

                        return;
                    }
                    if (txtRucCliente.Text.Length == 11 && Utils.Windows.ValidarRuc(txtRucCliente.Text) == false)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema");
                        _ventaDto.v_IdCliente = null;
                        _ventaDto.i_IdDireccionCliente = -1;
                        txtCodigoCliente.Clear();
                        txtRazonSocial.Clear();
                        txtDireccion.Clear();
                        IdIdentificacion = 0;
                        return;
                    }
                    var objOperationResult = new OperationResult();
                    string[] DatosProveedor = new string[4];
                    DatosProveedor = _objVentasBL.DevolverClientePorNroDocumento(ref objOperationResult,
                        txtRucCliente.Text.Trim());
                    if (DatosProveedor != null)
                    {
                        _ventaDto.v_IdCliente = DatosProveedor[0];
                        _ventaDto.i_IdDireccionCliente = int.Parse(DatosProveedor[6]);
                        txtCodigoCliente.Text = DatosProveedor[1];
                        txtRazonSocial.Text = DatosProveedor[2];
                        txtDireccion.Text = DatosProveedor[3];
                        IdIdentificacion = int.Parse(DatosProveedor[5]);
                        VerificarLineaCreditoCliente(_ventaDto.v_IdCliente);
                    }
                    else
                    {
                        using (var frm = new frmRegistroRapidoCliente(txtRucCliente.Text.Trim(), "C"))
                        {
                            frm.ShowDialog();
                            if (frm._Guardado)
                            {
                                txtRucCliente.Text = frm._NroDocumentoReturn;
                                DatosProveedor = _objVentasBL.DevolverClientePorNroDocumento(ref objOperationResult,
                                    txtRucCliente.Text.Trim());
                                if (DatosProveedor != null)
                                {
                                    _ventaDto.v_IdCliente = DatosProveedor[0];
                                    _ventaDto.i_IdDireccionCliente = int.Parse(DatosProveedor[6]);
                                    txtCodigoCliente.Text = DatosProveedor[1];
                                    txtRazonSocial.Text = DatosProveedor[2];
                                    txtDireccion.Text = DatosProveedor[3];
                                    IdIdentificacion = int.Parse(DatosProveedor[5]);
                                    VerificarLineaCreditoCliente(_ventaDto.v_IdCliente);
                                }
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
                    IdIdentificacion = 0;
                    //TipoPersona = -1;
                    //TipoDocumentCliente =-1;
                }
            }
        }

        private void txtGuiaRemisionSerie_Validating(object sender, CancelEventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtGuiaRemisionSerie, "{0:0000}");
        }

        private void cboEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (strModo == "Nuevo" && cboEstado.Value.ToString() == "0" && _ventaDto.v_IdCliente == null)
            {
                if (
                    UltraMessageBox.Show("¿Desea Realizar un comprobante Anulado?", "Confirmación",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    OperationResult objOperationResult = new OperationResult();
                    var ClienteAnulado = new ClienteBL().CreaVerificaClienteAnulado(ref objOperationResult);

                    if (objOperationResult.Success == 0)
                    {
                        UltraMessageBox.Show(
                            string.Format("{0}\n\n{1}\n\nTARGET: {2}", objOperationResult.ErrorMessage,
                                objOperationResult.ExceptionMessage, objOperationResult.AdditionalInformation), "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    _ventaDto.v_IdCliente = ClienteAnulado.v_IdCliente;
                    txtRazonSocial.Text = ClienteAnulado.v_RazonSocial;
                    txtDireccion.Text = ClienteAnulado.v_DirecPrincipal;
                    txtCodigoCliente.Text = ClienteAnulado.v_CodCliente;
                    txtRucCliente.Text = ClienteAnulado.v_NroDocIdentificacion;
                    cboDocumento.Value = cboDocumento.Value ?? "1";
                    txtSerieDoc.Text =
                        _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento,
                            int.Parse(cboDocumento.Value.ToString())).Trim();
                    txtCorrelativoDocIni.Text =
                        _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento,
                            int.Parse(cboDocumento.Value.ToString())).ToString("00000000").Trim();
                    PredeterminarEstablecimiento(txtSerieDoc.Text);

                    txtConcepto.Text = "FACTURA ANULADA";

                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    if (row == null) return;
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_IdUnidadMedida"].Value = "-1";
                    row.Cells["d_Cantidad"].Value = "1";
                    row.Cells["d_Precio"].Value = "1";
                    row.Cells["i_IdCentroCosto"].Value = centroDeCosto;
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
            decimal MontoACargar = 0;

            MontoACargar = decimal.TryParse(txtTotal.Text.Trim(), out MontoACargar) ? MontoACargar : 0;
            var MontoOriginalVenta = _ventaDto.d_Total != null ? _ventaDto.d_Total.Value : 0;

            var LineaCredito = new ClienteBL().DevuelveLineaCreditoCliente(ref objOperationResult, IdCliente);

            if (LineaCredito != null)
            {
                if ((LineaCredito.d_Saldo + MontoOriginalVenta) == 0 ||
                    (LineaCredito.d_Saldo + MontoOriginalVenta) < MontoACargar)
                {
                    UltraStatusbarManager.MarcarError(ultraStatusBar1,
                        "El cliente no dispone de crédito suficiente para realizar esta compra.", timer1);
                    txtSerieDoc.Enabled = false;
                    //se deshabilita la serie y correlativo porque al salir de sus focos tmb validan y pueden deshabilitar o habilitar el btnGuardar tambien
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

        private void grdData_ClickCellButton(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "d_Precio")
            {
                string Id = grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null
                    ? grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value.ToString()
                    : string.Empty;
                int IdAlmacen = grdData.ActiveRow.Cells["i_IdAlmacen"].Value != null
                    ? int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString())
                    : -1;
                if (!string.IsNullOrEmpty(Id))
                {
                    var PopUp = new ucListaPreciosPopUp(ucListaPreciosPopUp.TipoVenta.VentaNormal, Id, IdAlmacen);
                    panelListaPrecios.ClientArea.Controls.Clear();
                    panelListaPrecios.ClientArea.Controls.Add(PopUp);
                    ultraPopupControlContainer1.Show();
                }
            }

            if (grdData.ActiveRow != null && grdData.ActiveCell.Column.Key == "v_Observaciones")
            {

            }

            if (grdData.ActiveRow != null && grdData.ActiveCell.Column.Key == "DetalleAnexo")
            {
                var f = new frmTablaAnexos();
                f.ShowDialog();


                grdData.ActiveCell.Row.Cells["i_IdVentaDetalleAnexo"].Value = f._IdAnexo;
                grdData.ActiveCell.Value = f._Anexo;
                grdData.ActiveCell.Row.Cells["i_RegistroEstado"].Value = "Modificado";
            }
        }

        public void FijarPrecio(string pstrPrecio, int IdMoneda)
        {
            if (grdData.ActiveRow != null)
            {
                //  grdData.ActiveRow.Cells["d_Precio"].Value = pstrPrecio;
                grdData.ActiveRow.Cells["d_Precio"].Value = cboMoneda.Value == null ||
                                                            decimal.Parse(txtTipoCambio.Text) <= 0
                    ? "0.0"
                    : IdMoneda == int.Parse(cboMoneda.Value.ToString())
                        ? pstrPrecio
                        : int.Parse(cboMoneda.Value.ToString()) == (int)Currency.Soles
                            ? (Utils.Windows.DevuelveValorRedondeado(
                                decimal.Parse(pstrPrecio) * decimal.Parse(txtTipoCambio.Text), 2).ToString())
                            : (Utils.Windows.DevuelveValorRedondeado(
                                decimal.Parse(pstrPrecio) / decimal.Parse(txtTipoCambio.Text), 2).ToString());
                ultraPopupControlContainer1.Close();
                CalcularValoresFila(grdData.ActiveRow);
            }
        }

        private void txtGuiaRemisionCorrelativo_Validated(object sender, EventArgs e)
        {
            if (txtGuiaRemisionCorrelativo.Text.Trim().Length == 0) return;
            int EsNumeroEntero;
            var correlativos = txtGuiaRemisionCorrelativo.Text.Split(',');
            for (int i = 0; i < correlativos.Length; i++)
            {
                if (int.TryParse(correlativos[i], out EsNumeroEntero))
                {
                    correlativos[i] = EsNumeroEntero.ToString("0000000");
                }
            }

            txtGuiaRemisionCorrelativo.Text = string.Join(", ", correlativos);
        }

        private void txtConcepto_KeyPress(object sender, KeyPressEventArgs e)
        {
            GlosaEditada = true;
            Utils.Windows.CaracteresValidos((sender as Infragistics.Win.UltraWinEditors.UltraTextEditor), e);
        }

        private void LlenarCuentasRubros()
        {
            var objOperationResult = new OperationResult();
            Task.Factory.StartNew(() =>
            {
                var objDatahierarchyDto = new datahierarchyDto();
                objDatahierarchyDto = _objDatahierarchyBL.ObtenerDataHierarchyPorValue1(ref objOperationResult, 51,
                    "mercadería");
                if (objDatahierarchyDto != null)
                {
                    CtaRubroMercaderia = objDatahierarchyDto.v_Value2;
                }

                objDatahierarchyDto = _objDatahierarchyBL.ObtenerDataHierarchyPorValue1(ref objOperationResult, 51,
                    "servicio");
                if (objDatahierarchyDto != null)
                {
                    _ctaRubroServicio = objDatahierarchyDto.v_Value2;
                }
            }
                );
        }

        private void FilaAnticipio(UltraGridRow fila)
        {
            if (fila == null) return;
            var activado = fila.Cells["i_Anticipio"].Value != null && fila.Cells["i_Anticipio"].Value.ToString() == "1";

            var cellsCollection = new[]
            {
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
            var cellState = activado ? Activation.Disabled : Activation.AllowEdit;
            Array.ForEach(cellsCollection, cell => cell.Activation = cellState);

            if (activado)
            {
                fila.Cells["i_IdUnidadMedida"].Value = "5";
                fila.Cells["d_PrecioVenta"].Activate();
                grdData.PerformAction(UltraGridAction.EnterEditMode);
            }
        }

        private void FilasAnticipioActivacion(bool activado)
        {
            var state = activado ? Activation.AllowEdit : Activation.Disabled;
            foreach (var row in grdData.Rows)
                row.Cells["i_Anticipio"].Activation = state;
        }

        private void btnExtraer_Click(object sender, EventArgs e)
        {
            try
            {
                var f = new FrmExtraerDetallesMovimientos();
                f.ShowDialog();
                _idPedidoExtraccion = string.Empty;
                if (f.DialogResult == DialogResult.OK)
                {
                    if (f.AnularGuias)
                    {
                        listaGuiasRemisionPorAnular = f.ListaGuiasPorAnular;
                        EliminarVentas = false;
                    }
                    else if (f.AnularVentas)
                    {
                        listaGuiasRemisionPorAnular = f.ListaGuiasPorAnular;
                        EliminarVentas = true;
                    }

                    var objOperationResult = new OperationResult();
                    if (!string.IsNullOrEmpty(f.IdPedido))
                    {
                        _idPedidoExtraccion = f.IdPedido;
                        var pedido = new PedidoBL().ObtenerPedidoCabecera(ref objOperationResult, f.IdPedido);
                        if (objOperationResult.Success != 1) return;
                        txtNroPedido.Text = pedido.i_IdTipoDocumento == (int)TiposDocumentos.Pedido ? string.Format(@"{0}-{1}", pedido.v_SerieDocumento, pedido.v_CorrelativoDocumento) : "";
                        _ventaDto.v_IdCliente = pedido.v_IdCliente;
                        cboCondicionPago.Value = pedido.i_IdCondicionPago.ToString();
                        cboVendedor.Value = pedido.v_IdVendedor;
                        txtRucCliente.Text = pedido.RucCliente;
                        txtRazonSocial.Text = pedido.RazonSocial;
                        txtDireccion.Text = pedido.Direccion;
                        cboMoneda.Value = pedido.i_IdMoneda.ToString();
                        chkAfectoIGV.Checked = pedido.i_AfectoIgv == 1;
                        chkPrecInIGV.Checked = pedido.i_PrecionInclIgv == 1;
                        txtNroPedido.Enabled = false;
                        IdIdentificacion = pedido.TipoDocumentoCliente;
                        if (!string.IsNullOrEmpty(pedido.v_Glosa))
                        {
                            txtConcepto.Text = pedido.v_Glosa;
                        }
                        _ventaDto.i_IdDireccionCliente = pedido.i_IdDireccionCliente;
                        btnAgregar.Enabled = pedido.i_IdTipoDocumento != (int)TiposDocumentos.Pedido;
                        ExtraccionPedido = true;
                    }
                    else
                    {
                        var firstOrDefault = f.ListaRetornoVentas.FirstOrDefault();
                        if (firstOrDefault != null && !string.IsNullOrWhiteSpace(firstOrDefault.v_IdVenta))
                        {
                            var cabecera = new VentaBL().ObtenerVentaCabecera(ref objOperationResult, firstOrDefault.v_IdVenta);
                            if (cabecera != null)
                            {
                                _ventaDto.v_IdCliente = cabecera.v_IdCliente;
                                cboCondicionPago.Value = cabecera.i_IdCondicionPago.ToString();
                                cboVendedor.Value = cabecera.v_IdVendedor;
                                txtRucCliente.Text = cabecera.NroDocCliente;
                                txtRazonSocial.Text = cabecera.NombreCliente;
                                txtDireccion.Text = cabecera.Direccion;
                                txtCodigoCliente.Text = cabecera.CodigoCliente;
                                cboMoneda.Value = cabecera.i_IdMoneda.ToString();
                                cboCondicionPago.Value = cabecera.i_IdCondicionPago ?? -1;
                                cboMoneda.Value = cabecera.i_IdMoneda ?? -1;
                                txtTipoCambio.Text = (cabecera.d_TipoCambio ?? 0).ToString();
                                txtFlete.Text = (cabecera.d_FleteTotal ?? 0).ToString();
                                txtSeguro.Text = (cabecera.d_SeguroTotal ?? 0).ToString();
                                txtCantidadTotal.Text = (cabecera.d_CantidaTotal ?? 0).ToString();
                                _ventaDto.i_IdDireccionCliente = cabecera.i_IdDireccionCliente;
                                cboMVenta.Value = cabecera.i_IdMedioPagoVenta ?? -1;
                                cboPuntoDestino.Value = cabecera.i_IdPuntoDestino.ToString();
                                cboPuntoEmbarque.Value = cabecera.i_IdPuntoEmbarque.ToString();
                                cboTipoEmbarque.Value = cabecera.i_IdTipoEmbarque.ToString();
                                txtOrdenCompra.Text = cabecera.v_OrdenCompra;
                                txtPesoBrutoKg.Text = cabecera.d_PesoBrutoKG.ToString();
                                txtPesoNetoKg.Text = cabecera.d_PesoNetoKG.ToString();
                                cboVendedor.Value = cabecera.v_IdVendedor;
                                chkDrawnBack.Checked = cabecera.i_DrawBack == 1 ? true : false;
                                cboTipoVenta.Value = cabecera.i_IdTipoVenta.Value.ToString("00");
                                txtNroBulto.Text = cabecera.v_NroBulto;
                                txtDimensiones.Text = cabecera.v_BultoDimensiones;
                                txtMarca.Text = cabecera.v_Marca;
                                txtFlete.Text = cabecera.d_FleteTotal.ToString();
                                txtSeguro.Text = cabecera.d_SeguroTotal.ToString();
                                txtNroBL.Text = cabecera.v_NroBL;
                                dtpFechaBL.Value = cabecera.t_FechaPagoBL.Value;
                                txtContenedor.Text = cabecera.v_Contenedor;
                                txtBanco.Text = cabecera.v_Banco;
                                txtNaviera.Text = cabecera.v_Naviera;
                                txtNroBultoIngles.Text = cabecera.v_NroBultoIngles;
                                if (Globals.ClientSession.v_RucEmpresa.Equals(Constants.RucAgrofergic) || Globals.ClientSession.v_RucEmpresa.Equals(Constants.RucDemo) || Globals.ClientSession.v_RucEmpresa.Equals(Constants.RucAgrofergic2))
                                {
                                    chkPrecInIGV.Checked = cabecera.i_PreciosIncluyenIgv == 1;
                                    chkAfectoIGV.Checked = cabecera.i_EsAfectoIgv == 1;
                                    cboTipoOperacion.Value = cabecera.i_IdTipoOperacion;
                                }
                                ExtraccionPedido = false;
                            }
                        }
                    }
                    if (f.ListaRetornoVentas.Any())
                    {
                        _TempDetalle_EliminarDto.AddRange(grdData.Rows.Select(r => (ventadetalleDto)r.ListObject));
                        grdData.DataSource = f.ListaRetornoVentas;
                    }

                    for (int i = 0; i < grdData.Rows.Count(); i++)
                    {
                        grdData.Rows[i].Cells["i_RegistroTipo"].Value = "Temporal";
                        grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";

                        if (Globals.ClientSession.v_RucEmpresa.Equals(Constants.RucAgrofergic))
                        {
                            if (grdData.Rows[i].Cells["i_IdTipoOperacion"].Value == null || grdData.Rows[i].Cells["i_IdTipoOperacion"].Value.ToString() == "-1")
                                grdData.Rows[i].Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                        }
                        else //<- esta para que las ventas internas de manguifajas bajen con igv en el detalle al extraerse en factura.
                            grdData.Rows[i].Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();

                    }

                    CalcularValoresDetalle();
                    Extraccion = true;
                }
                else
                {
                    Extraccion = false;
                    btnAgregar.Enabled = true;
                    ExtraccionPedido = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtNroPedido_Validated(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtNroPedido.Text.Trim()))
                {
                    var serieCorrelativo = txtNroPedido.Text.Split('-');
                    if (serieCorrelativo.Length == 2)
                    {
                        int s, c;
                        if (int.TryParse(serieCorrelativo[0], out s) && int.TryParse(serieCorrelativo[1], out c))
                            txtNroPedido.Text = string.Format(@"{0:0000} - {1:00000000}", s, c);
                        else
                            txtNroPedido.Clear();
                    }
                    else
                        txtNroPedido.Clear();
                }
            }
            catch
            {
                txtNroPedido.Clear();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void txtCorrelativo_EditorButtonClick(object sender,
            Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            switch (e.Button.Key)
            {
                case "btnAnterior":
                    if (OnAnterior == null) return;
                    OnAnterior();
                    if (!string.IsNullOrWhiteSpace(_idRecibido))
                    {
                        CargarCabecera(_idRecibido);
                    }

                    break;
                default:
                    if (OnSiguiente == null) return;
                    OnSiguiente();
                    if (!string.IsNullOrWhiteSpace(_idRecibido))
                    {
                        CargarCabecera(_idRecibido);
                    }
                    break;
            }
        }

        private void grdData_AfterEnterEditMode(object sender, EventArgs e)
        {
            if (grdData.ActiveCell != null && grdData.ActiveCell.Column.Key.Equals("d_PrecioVenta"))
            {
                decimal.TryParse(grdData.ActiveCell.Value.ToString(), out valorOriginalCelda);
            }
        }

        private void grdData_BeforeRowInsert(object sender, BeforeRowInsertEventArgs e)
        {
            var filasActuales = grdData.Rows.Count;
            if (_limiteDocumento == 0 || _limiteDocumento == null)
            {
                if (cboDocumento.Value != null && txtSerieDoc.Text.Trim() != string.Empty)
                    _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()),
                        txtSerieDoc.Text.Trim());
            }

            if (filasActuales > _limiteDocumento - 1)
            {
                e.Cancel = true;
                UltraStatusbarManager.MarcarError(ultraStatusBar1, "Máximo de items por documento alcanzado!", timer1);
            }

            else
                UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
        }

        #region Calculo ISC

        private readonly Dictionary<string, productoiscDto> _productoiscDtos = new Dictionary<string, productoiscDto>();

        private void AddIsc(string idProdDetalle)
        {
            if (_productoiscDtos.ContainsKey(idProdDetalle)) return;
            var objresult = new OperationResult();
            var productIsc = new ProductoIscBL().FromProductDetail(ref objresult, idProdDetalle,
                Globals.ClientSession.i_Periodo.ToString());
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

        public void GestionarComportamientoNcr(int idMotivo)
        {
            try
            {
                if (_ventaDto.v_IdVenta != null) return;
                switch (idMotivo)
                {
                    case 1: //Anulacion operacion
                        DevuelveVentaReferencia(true);
                        grdData.DisplayLayout.Bands[0].Columns["d_Cantidad"].CellActivation = Activation.AllowEdit;
                        break;

                    case 4: //descuento global
                        FijarServicioPorDescuento();
                        grdData.DisplayLayout.Bands[0].Columns["d_Cantidad"].CellActivation = Activation.AllowEdit;
                        break;

                    case 5: //descuento por item
                        DevuelveVentaReferencia(true);
                        grdData.Rows.ToList().ForEach(c => c.Cells["d_Cantidad"].Value = "0");
                        grdData.Rows.ToList().ForEach(c => c.Cells["d_CantidadEmpaque"].Value = "0");
                        grdData.DisplayLayout.Bands[0].Columns["d_Cantidad"].CellActivation = Activation.Disabled;
                        break;

                    case 7: //devolucion por item
                        DevuelveVentaReferencia(true);
                        grdData.DisplayLayout.Bands[0].Columns["d_Cantidad"].CellActivation = Activation.AllowEdit;
                        break;

                    case 8: //bonificacion
                        FijarServicioPorDescuento();
                        grdData.DisplayLayout.Bands[0].Columns["d_Cantidad"].CellActivation = Activation.AllowEdit;
                        break;

                    case 9: //disminucion valor
                        FijarServicioPorDescuento();
                        grdData.DisplayLayout.Bands[0].Columns["d_Cantidad"].CellActivation = Activation.AllowEdit;
                        break;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FijarServicioPorDescuento()
        {
            _TempDetalle_AgregarDto = new List<ventadetalleDto>();
            _TempDetalle_ModificarDto = new List<ventadetalleDto>();
            _TempDetalle_EliminarDto = new List<ventadetalleDto>();
            CargarDetalle("");
            btnAgregar_Click(this, new EventArgs());
            var row = grdData.ActiveRow;
            var prod = _objVentasBL.DevolverArticuloPorCodInterno("SERV001",
                int.Parse(row.Cells["i_IdAlmacen"].Value.ToString()));
            if (prod != null)
            {
                row.Cells["v_DescripcionProducto"].Value = prod.v_Descripcion;
                row.Cells["v_IdProductoDetalle"].Value = prod.v_IdProductoDetalle;
                row.Cells["v_CodigoInterno"].Value = prod.v_CodInterno;
                row.Cells["Empaque"].Value = prod.d_Empaque;
                row.Cells["UMEmpaque"].Value = prod.EmpaqueUnidadMedida;
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_IdUnidadMedida"].Value = prod.i_IdUnidadMedida;
                row.Cells["i_IdCentroCosto"].Value = centroDeCosto;
                row.Cells["i_IdUnidadMedidaProducto"].Value = prod.i_IdUnidadMedida;
                row.Cells["i_EsAfectoDetraccion"].Value = prod.i_EsAfectoDetraccion;
                row.Cells["i_IdTipoOperacion"].Value = (string)cboTipoOperacion.Value != "5"
                    ? cboTipoOperacion.Value.ToString()
                    : "1";
                row.Cells["d_isc"].Value = "0";
                row.Cells["d_otrostributos"].Value = "0";
                row.Cells["d_Precio"].Value = DevolverPrecioProducto(prod.IdMoneda, prod.d_Precio ?? 0,
                    int.Parse(cboMoneda.Value.ToString()),
                    !string.IsNullOrEmpty(txtTipoCambio.Text.Trim()) ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0);
                row.Cells["i_EsNombreEditable"].Value = prod.i_NombreEditable;
                row.Cells["i_EsServicio"].Value = prod.i_EsServicio;
                row.Cells["v_NroCuenta"].Value = prod.NroCuentaVenta;
                row.Cells["i_ValidarStock"].Value = prod.i_ValidarStock ?? 0;
                row.Cells["i_EsAfectoPercepcion"].Value = prod.i_EsAfectoPercepcion ?? 0;
                row.Cells["d_TasaPercepcion"].Value = prod.d_TasaPercepcion;
                row.Cells["i_IdMonedaLP"].Value = prod.IdMoneda;
                row.Cells["v_PedidoExportacion"].Value = prod.v_NroPedidoExportacion;
                if (prod.EsAfectoIsc)
                    AddIsc(prod.v_IdProductoDetalle);

                foreach (var fila in grdData.Rows)
                {
                    fila.Cells["i_RegistroTipo"].Value = "Temporal";
                    fila.Cells["i_RegistroEstado"].Value = "Modificado";
                }
            }

        }

        private void ucTipoNota_ValueChanged(object sender, EventArgs e)
        {
            if (ucTipoNota.Value == null) return;
            txtConcepto.NullText = @"Ingrese motivo de la Nota Electronica";
            txtConcepto.Text = ((Infragistics.Win.UltraWinEditors.UltraComboEditor)sender).Text;
            _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()),
                    txtSerieDoc.Text.Trim());
            int id;
            if (int.TryParse(ucTipoNota.Value.ToString(), out id))
            {
                GestionarComportamientoNcr(id);
            }
        }

        private void cboDocumento_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var x = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
            var y = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
            puEmbarque.Show(this.ParentForm, new Point(x + (Width - panelEmbarque.Width) / 2, y + (Height - panelEmbarque.Height) / 2));
            // CalcularValoresDetalle();
        }

        private void btnImprimirFactura_Click(object sender, EventArgs e)
        {
            idVentasImpresion = new List<string>();
            idVentasImpresion = grdData.Rows.Select(p => p.Cells["v_IdVenta"].Value.ToString()).Distinct().ToList();
            impresionVistaPrevia = _objDocumentoBL.ImpresionVistaPrevia(int.Parse(cboDocumento.Value.ToString()),
                txtSerieDoc.Text.Trim());
            var establecimientoDetalle =
                _objDocumentoBL.ConfiguracionEstablecimiento(int.Parse(cboDocumento.Value.ToString()),
                    txtSerieDoc.Text.Trim());
            if (establecimientoDetalle == null)
            {
                UltraMessageBox.Show(
                    string.Format("Documento {0} {1} no se encuentra asignado en Establecimiento ", cboDocumento.Text,
                        txtSerieDoc.Text), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            if (Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic)
            {

                //pnlImpresionFactura.Visible = true;
                BtnImprimir.Enabled = false;
                btnGuardar.Enabled = false;


            }
        }

        private void btnImprimirTipoFormato_Click(object sender, EventArgs e)
        {
            //using (var frm = new frmDocumentoFactura(idVentasImpresion, impresionVistaPrevia, Reportes.TiposReportes.PreFactura, rbtnFormatoIngles.Checked ? Reportes.Idioma.Ingles : Reportes.Idioma.Espaniol))
            using (var frm = new frmDocumentoFactura(idVentasImpresion, impresionVistaPrevia, int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.Proforma ? Reportes.TiposReportes.Proforma : Reportes.TiposReportes.Factura, rbtnFormatoIngles.Checked ? Reportes.Idioma.Ingles : Reportes.Idioma.Espaniol, null, uckConsiderarAliasCliente.Checked))
            {
                if (impresionVistaPrevia)
                {
                    frm.ShowDialog();
                    frm.Activate();
                }
                else
                {

                    BtnImprimir.Enabled = false;

                }
            }
        }

        private void btnSalirPanel_Click(object sender, EventArgs e)
        {
            pnlImpresionPrefactura.Visible = false;
            BtnImprimir.Enabled = true;
            btnGuardar.Enabled = true;
            rbtnFormatoEspaniol.Checked = true;
            uckConsiderarAliasCliente.Checked = false;
            uckConsiderarFlete.Checked = true;
            uckConsiderarObservacionesDetalle.Checked = false;
        }

        private void btnImprimirFormatoFactura_Click(object sender, EventArgs e)
        {
            //using (var frm = new frmDocumentoFactura(idVentasImpresion, impresionVistaPrevia, Reportes.TiposReportes.Factura, rbtnFormatoIngles1.Checked ? Reportes.Idioma.Ingles : Reportes.Idioma.Espaniol))
            //{
            //    if (impresionVistaPrevia)
            //    {
            //        frm.ShowDialog();
            //        frm.Activate();
            //    }
            //    else
            //    {

            //        BtnImprimir.Enabled = false;


            //    }
            //}
        }

        private void btnSalirPanelFactura_Click(object sender, EventArgs e)
        {
            //pnlImpresionFactura.Visible = false;
            BtnImprimir.Enabled = true;
            btnGuardar.Enabled = true;
        }

        private void cboMVenta_ValueChanged(object sender, EventArgs e)
        {
            int i;
            var id = cboMVenta.Value != null && int.TryParse(cboMVenta.Value.ToString(), out i) ? i : -1;
            switch (id)
            {
                case 2: //CFR
                    txtFlete.Enabled = true;
                    txtSeguro.Enabled = false;
                    txtCantidadTotal.Enabled = true;
                    txtSeguro.Text = @"0.0";
                    grdData.DisplayLayout.Bands[0].Columns["d_PrecioPactado"].Hidden = false;
                    grdData.DisplayLayout.Bands[0].Columns["d_FleteXProducto"].Hidden = false;
                    grdData.DisplayLayout.Bands[0].Columns["d_SeguroXProducto"].Hidden = true;
                    break;
                case 3: //CIF
                    txtFlete.Enabled = true;
                    txtSeguro.Enabled = true;
                    txtCantidadTotal.Enabled = true;
                    grdData.DisplayLayout.Bands[0].Columns["d_PrecioPactado"].Hidden = false;
                    grdData.DisplayLayout.Bands[0].Columns["d_FleteXProducto"].Hidden = false;
                    grdData.DisplayLayout.Bands[0].Columns["d_SeguroXProducto"].Hidden = false;
                    break;
                default:
                    txtFlete.Enabled = false;
                    txtSeguro.Enabled = false;
                    txtCantidadTotal.Enabled = false;
                    txtFlete.Text = @"0.0";
                    txtSeguro.Text = @"0.0";
                    txtCantidadTotal.Text = @"0.0";
                    grdData.DisplayLayout.Bands[0].Columns["d_PrecioPactado"].Hidden = true;
                    grdData.DisplayLayout.Bands[0].Columns["d_FleteXProducto"].Hidden = true;
                    grdData.DisplayLayout.Bands[0].Columns["d_SeguroXProducto"].Hidden = true;
                    break;
            }
            CalcularValoresDetalle();
        }

        private void txtFlete_Validating(object sender, CancelEventArgs e)
        {
            decimal d;
            if (!decimal.TryParse(txtFlete.Text, out d))
                txtFlete.Text = @"0.0";

            CalcularValoresDetalle();
            grdData.Rows.ToList().ForEach(r => r.Cells["i_RegistroEstado"].Value = "Modificado");
        }

        private void txtSeguro_Validating(object sender, CancelEventArgs e)
        {
            decimal d;
            if (!decimal.TryParse(txtSeguro.Text, out d))
                txtSeguro.Text = @"0.0";

            CalcularValoresDetalle();
            grdData.Rows.ToList().ForEach(r => r.Cells["i_RegistroEstado"].Value = "Modificado");
        }

        private void txtCantidadTotal_Validating(object sender, CancelEventArgs e)
        {
            decimal d;
            if (!decimal.TryParse(txtCantidadTotal.Text, out d))
                txtCantidadTotal.Text = @"0.0";

            CalcularValoresDetalle();
            grdData.Rows.ToList().ForEach(r => r.Cells["i_RegistroEstado"].Value = "Modificado");
        }

        private void puEmbarque_Closed(object sender, EventArgs e)
        {
            CalcularValoresDetalle();
            grdData.Refresh();
        }

        private void txtFlete_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            decimal d;
            var flete = decimal.TryParse(txtFlete.Text, out d) ? d : 0;
            if (flete <= 0) return;
            if (!string.IsNullOrWhiteSpace(Globals.ClientSession.v_IdProductoDetalleFlete))
            {
                var objOp = new OperationResult();
                var prod = new SAMBHS.Almacen.BL.ProductoBL().ObtenerProductoDesdeProdDetalle(ref objOp, Globals.ClientSession.v_IdProductoDetalleFlete);
                if (prod == null) return;
                var filaExistente = grdData.Rows
                    .FirstOrDefault(f => f.Cells["v_IdProductoDetalle"].Value != null && f.Cells["v_IdProductoDetalle"]
                                    .Value.ToString()
                                    .Equals(Globals.ClientSession.v_IdProductoDetalleFlete));

                if (filaExistente == null)
                {
                    var row = grdData.DisplayLayout.Bands[0].AddNew();
                    if (row == null) return;
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_IdUnidadMedida"].Value = "-1";
                    row.Cells["d_Cantidad"].Value = "1";
                    row.Cells["d_Precio"].Value = flete;
                    row.Cells["i_IdUnidadMedida"].Value = "15";
                    row.Cells["i_Anticipio"].Value = "0";
                    row.Cells["d_isc"].Value = "0";
                    row.Cells["d_otrostributos"].Value = "0";
                    row.Cells["d_Descuento"].Value = "0";
                    row.Cells["d_Valor"].Value = "0";
                    row.Cells["v_NroCuenta"].Value = "-1";
                    //row.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null &&
                    //                                       cboTipoOperacion.Value.ToString() != "5"
                    //    ? cboTipoOperacion.Value.ToString()
                    //    : "1";

                    row.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null &&
                                                         cboTipoOperacion.Value.ToString() != "5"
                      ? cboTipoOperacion.Value.ToString()
                      : "3";
                    row.Cells["i_IdCentroCosto"].Value = centroDeCosto;
                    row.Cells["d_Percepcion"].Value = "0";
                    row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    row.Cells["i_Anticipio"].Activation = chkDeduccionAnticipio.Checked
                        ? Activation.AllowEdit
                        : Activation.Disabled;

                    row.Cells["v_DescripcionProducto"].Value = prod.v_Descripcion;
                    row.Cells["v_IdProductoDetalle"].Value = prod.v_IdProductoDetalle;
                    row.Cells["v_CodigoInterno"].Value = prod.v_CodInterno;
                    row.Cells["Empaque"].Value = prod.d_Empaque;
                    row.Cells["v_NroCuenta"].Value = prod.NroCuenta;
                    row.Cells["i_EsServicio"].Value = 0;
                }
                else
                {
                    filaExistente.Cells["i_RegistroEstado"].Value = "Modificado";
                    filaExistente.Cells["i_IdUnidadMedida"].Value = "-1";
                    filaExistente.Cells["d_Cantidad"].Value = "1";
                    filaExistente.Cells["d_Precio"].Value = flete;
                    filaExistente.Cells["i_IdUnidadMedida"].Value = "15";
                    filaExistente.Cells["i_Anticipio"].Value = "0";
                    filaExistente.Cells["d_isc"].Value = "0";
                    filaExistente.Cells["d_otrostributos"].Value = "0";
                    filaExistente.Cells["d_Descuento"].Value = "0";
                    filaExistente.Cells["d_Valor"].Value = "0";
                    filaExistente.Cells["v_NroCuenta"].Value = "-1";
                    //filaExistente.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null &&
                    //                                       cboTipoOperacion.Value.ToString() != "5"
                    //    ? cboTipoOperacion.Value.ToString()
                    //    : "1";

                    filaExistente.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null &&
                                                         cboTipoOperacion.Value.ToString() != "5"
                      ? cboTipoOperacion.Value.ToString()
                      : "3";
                    filaExistente.Cells["i_IdCentroCosto"].Value = centroDeCosto;
                    filaExistente.Cells["d_Percepcion"].Value = "0";
                    filaExistente.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    filaExistente.Cells["i_Anticipio"].Activation = chkDeduccionAnticipio.Checked
                        ? Activation.AllowEdit
                        : Activation.Disabled;

                    filaExistente.Cells["v_DescripcionProducto"].Value = prod.v_Descripcion;
                    filaExistente.Cells["v_IdProductoDetalle"].Value = prod.v_IdProductoDetalle;
                    filaExistente.Cells["v_CodigoInterno"].Value = prod.v_CodInterno;
                    filaExistente.Cells["Empaque"].Value = prod.d_Empaque;
                    filaExistente.Cells["v_NroCuenta"].Value = prod.NroCuenta;
                }

            }
            else
            {
                MessageBox.Show(@"Por favor, configure el artículo Flete en la configuración de Empresa", @"Sistema",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void txtSeguro_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            decimal d;
            var seguro = decimal.TryParse(txtSeguro.Text, out d) ? d : 0;
            if (seguro <= 0) return;
            if (!string.IsNullOrWhiteSpace(Globals.ClientSession.v_IdProductoDetalleSeguro))
            {
                var objOp = new OperationResult();
                var prod = new ProductoBL().ObtenerProductoDesdeProdDetalle(ref objOp, Globals.ClientSession.v_IdProductoDetalleSeguro);
                if (prod == null) return;
                var filaExistente = grdData.Rows
                    .FirstOrDefault(f => f.Cells["v_IdProductoDetalle"].Value != null && f.Cells["v_IdProductoDetalle"]
                                             .Value.ToString()
                                             .Equals(Globals.ClientSession.v_IdProductoDetalleSeguro));

                if (filaExistente == null)
                {
                    var row = grdData.DisplayLayout.Bands[0].AddNew();
                    if (row == null) return;
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_IdUnidadMedida"].Value = "-1";
                    row.Cells["d_Cantidad"].Value = "1";
                    row.Cells["d_Precio"].Value = seguro;
                    row.Cells["i_IdUnidadMedida"].Value = "15";
                    row.Cells["i_Anticipio"].Value = "0";
                    row.Cells["d_isc"].Value = "0";
                    row.Cells["d_otrostributos"].Value = "0";
                    row.Cells["d_Descuento"].Value = "0";
                    row.Cells["d_Valor"].Value = "0";
                    row.Cells["v_NroCuenta"].Value = "-1";
                    //row.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null &&
                    //                                       cboTipoOperacion.Value.ToString() != "5"
                    //    ? cboTipoOperacion.Value.ToString()
                    //    : "1";

                    row.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null &&
                                                          cboTipoOperacion.Value.ToString() != "5"
                       ? cboTipoOperacion.Value.ToString()
                       : "3";
                    row.Cells["i_IdCentroCosto"].Value = centroDeCosto;
                    row.Cells["d_Percepcion"].Value = "0";
                    row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    row.Cells["i_Anticipio"].Activation = chkDeduccionAnticipio.Checked
                        ? Activation.AllowEdit
                        : Activation.Disabled;

                    row.Cells["v_DescripcionProducto"].Value = prod.v_Descripcion;
                    row.Cells["v_IdProductoDetalle"].Value = prod.v_IdProductoDetalle;
                    row.Cells["v_CodigoInterno"].Value = prod.v_CodInterno;
                    row.Cells["Empaque"].Value = prod.d_Empaque;
                    row.Cells["v_NroCuenta"].Value = prod.NroCuenta;
                    row.Cells["i_EsServicio"].Value = 0;
                }
                else
                {
                    filaExistente.Cells["i_RegistroEstado"].Value = "Modificado";
                    filaExistente.Cells["i_IdUnidadMedida"].Value = "-1";
                    filaExistente.Cells["d_Cantidad"].Value = "1";
                    filaExistente.Cells["d_Precio"].Value = seguro;
                    filaExistente.Cells["i_IdUnidadMedida"].Value = "15";
                    filaExistente.Cells["i_Anticipio"].Value = "0";
                    filaExistente.Cells["d_isc"].Value = "0";
                    filaExistente.Cells["d_otrostributos"].Value = "0";
                    filaExistente.Cells["d_Descuento"].Value = "0";
                    filaExistente.Cells["d_Valor"].Value = "0";
                    filaExistente.Cells["v_NroCuenta"].Value = "-1";
                    //filaExistente.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null &&
                    //                                                 cboTipoOperacion.Value.ToString() != "5"
                    //    ? cboTipoOperacion.Value.ToString()
                    //    : "1";
                    filaExistente.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null &&
                                                         cboTipoOperacion.Value.ToString() != "5"
                      ? cboTipoOperacion.Value.ToString()
                      : "3";
                    filaExistente.Cells["i_IdCentroCosto"].Value = centroDeCosto;
                    filaExistente.Cells["d_Percepcion"].Value = "0";
                    filaExistente.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    filaExistente.Cells["i_Anticipio"].Activation = chkDeduccionAnticipio.Checked
                        ? Activation.AllowEdit
                        : Activation.Disabled;

                    filaExistente.Cells["v_DescripcionProducto"].Value = prod.v_Descripcion;
                    filaExistente.Cells["v_IdProductoDetalle"].Value = prod.v_IdProductoDetalle;
                    filaExistente.Cells["v_CodigoInterno"].Value = prod.v_CodInterno;
                    filaExistente.Cells["Empaque"].Value = prod.d_Empaque;
                    filaExistente.Cells["v_NroCuenta"].Value = prod.NroCuenta;
                }
            }
            else
            {
                MessageBox.Show(@"Por favor, configure el artículo Seguro Marítimo en la configuración de Empresa", @"Sistema",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private List<ReporteDocumentoFactura> LlenarDetallesImpresionDirecto()
        {
            List<ReporteDocumentoFactura> ListaDetalles = new List<ReporteDocumentoFactura>();
            ReporteDocumentoFactura objDetalle = new ReporteDocumentoFactura();
            OperationResult objOperationResult = new OperationResult();
            var Cliente = new ClienteBL().ObtenerClientePorID(ref objOperationResult, _ventaDto.v_IdCliente);
            if (Cliente != null)
            {
                objDetalle.Alias = Cliente.v_Alias;
            }
            objDetalle.TelefonoCliente = Cliente.v_TelefonoFijo + "-" + Cliente.v_TelefonoMovil;
            objDetalle.FechaRegistro = dtpFechaRegistro.Value.Date;
            objDetalle.NroDocCliente = txtRucCliente.Text;
            objDetalle.NombreCliente = txtRazonSocial.Text;
            objDetalle.Vendedor = cboVendedor.Text.Trim();
            objDetalle.Direccion = txtDireccion.Text.Trim();
            objDetalle.CondicionPago = cboCondicionPago.Text.Trim();
            objDetalle.Documento = txtSerieDoc.Text + " " + txtCorrelativoDocIni.Text;
            objDetalle.TipoDocumento = cboDocumento.Text;
            objDetalle.ValorVenta = decimal.Parse(txtValorVenta.Text);
            objDetalle.Igv = decimal.Parse(txtIGV.Text);
            objDetalle.Total = decimal.Parse(txtTotal.Text);
            objDetalle.Descuento = decimal.Parse(txtDescuento.Text);
            objDetalle.SerieDocumento = txtSerieDoc.Text.Trim();
            objDetalle.SFechaOC = dtpFechaOrden.Value.Date.ToShortDateString();  //A.DFechaOC.Date.Day.ToString("00") + "/" + A.DFechaOC.Date.Month.ToString("00") + "/" + A.DFechaOC.Date.Year.ToString(),
            objDetalle.SFechaVencimiento = dtpFechaVencimiento.Value.Date.ToShortDateString();  //A.DFechaVencimiento.Date.Day.ToString("00") + "/" + A.DFechaVencimiento.Date.Month.ToString("00") + "/" + A.DFechaVencimiento.Date.Year.ToString(),
            objDetalle.NroOrdenCompra = txtOrdenCompra.Text;
            objDetalle.Valor = decimal.Parse(txtValor.Text);
            objDetalle.TipoCambio = decimal.Parse(txtTipoCambio.Text.Trim());
            objDetalle.valorigv = decimal.Parse(cboIGV.Text.ToString());
            objDetalle.Moneda = int.Parse(cboMoneda.Value.ToString()) == 1 ? "S/" : "USD";
            objDetalle.d_Igv = cboIGV.Text.ToString().Substring(0, 2) + "%";
            objDetalle.Pedido = txtNroPedido.Text.Trim();
            objDetalle.GuiaRemision = txtGuiaRemisionSerie.Text + "-" + txtGuiaRemisionCorrelativo.Text;
            objDetalle.NroPlaca = txtPlacaVehiculo.Text;
            objDetalle.i_PreciosIncluyenIgv = chkPrecInIGV.Checked ? 1 : 0;// A.i_PreciosIncluyenIgv ?? 0,
            objDetalle.Dia = objDetalle.FechaRegistro.Date.Day.ToString("00");
            objDetalle.Mess = objDetalle.FechaRegistro.Date.Month.ToString("00");
            objDetalle.Anio = objDetalle.FechaRegistro.Date.Year.ToString();
            objDetalle.FechaLetras = objDetalle.FechaRegistro.Day.ToString("00") + " DE " + new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(objDetalle.FechaRegistro.Month.ToString())).ToUpper() + " DEL " + objDetalle.FechaRegistro.Year.ToString();
            objDetalle.MesLetras = new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(int.Parse(objDetalle.FechaRegistro.Month.ToString())).ToUpper();
            objDetalle.iTipoCfr = cboMVenta.Value == null ? -1 : int.Parse(cboMVenta.Value.ToString());
            objDetalle.PuntoEmbarque = cboPuntoEmbarque.Value == null || int.Parse(cboPuntoEmbarque.Value.ToString()) == -1 ? "" : cboPuntoEmbarque.Text.Trim();
            objDetalle.PuntoDestinoEmbarque = cboPuntoDestino.Value == null || int.Parse(cboPuntoDestino.Value.ToString()) == -1 ? "" : cboPuntoDestino.Text.Trim();
            objDetalle.TipoCfr = cboMVenta.Value == null || int.Parse(cboMVenta.Value.ToString()) == -1 ? "" : cboMVenta.Text.Trim();
            objDetalle.TipoCfr = objDetalle.TipoCfr == "" ? "" : objDetalle.iTipoCfr == 1 ? "TOTAL " + objDetalle.TipoCfr + " " + objDetalle.PuntoEmbarque : "TOTAL  " + objDetalle.TipoCfr + " " + objDetalle.PuntoDestinoEmbarque;
            objDetalle.TipoEmbarque = cboTipoEmbarque.Value == null || int.Parse(cboTipoEmbarque.Value.ToString()) == -1 ? "" : cboTipoEmbarque.Text.Trim();
            objDetalle.MarcaEmbarque = txtMarca.Text.Trim();
            objDetalle.PesoBruto = string.IsNullOrEmpty(txtPesoBrutoKg.Text) ? 0 : decimal.Parse(txtPesoBrutoKg.Text);
            objDetalle.PesoNeto = string.IsNullOrEmpty(txtPesoNetoKg.Text) ? 0 : decimal.Parse(txtPesoNetoKg.Text); ;
            objDetalle.NroBultos = txtNroBulto.Text;
            // Kardex = A.Kardex == "V" ? "VARIOS" : NroKardex,
            // AmbasDescripcionesProducto = A.AmbasDescripcionesProducto,
            // FormaPagoCobranza=FormaPagoCobranza ,
            //AmbasDescripcionesProducto = H == null ? "" : string.IsNullOrEmpty(H.v_Descripcion2) ? "" : H.v_Descripcion2,
            foreach (var Fila in grdData.Rows)
            {
                ReporteDocumentoFactura objDetalle1 = new ReporteDocumentoFactura();
                objDetalle1 = (ReporteDocumentoFactura)objDetalle.Clone();
                var IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value != null ? Fila.Cells["v_IdProductoDetalle"].Value.ToString() : null;
                var prod = new ProductoBL().ObtenerProductoDesdeProdDetalle(ref  objOperationResult, IdProductoDetalle);
                objDetalle1.CodigoArticulo = Fila.Cells["v_CodigoInterno"].Value == null ? null : Fila.Cells["v_CodigoInterno"].Text.ToString();
                objDetalle1.Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Text.ToString());
                objDetalle1.UnidadSiglas = Fila.Cells["i_IdUnidadMedida"].Value == null ? "" : Fila.Cells["i_IdUnidadMedida"].Text.ToString();
                objDetalle1.AmbasDescripcionesProducto = Fila.Cells["v_DescripcionProducto"].Value == null ? "" : Fila.Cells["v_DescripcionProducto"].Text.ToString();

                if (Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic)
                {
                    if (rbtnFormatoIngles.Checked)
                    {
                        objDetalle1.Descripcion = prod != null ? prod.v_Descripcion2 : "";
                    }
                    else
                    {
                        var DescripcionProducto = Fila.Cells["v_DescripcionProducto"].Value == null ? "" : Fila.Cells["v_DescripcionProducto"].Text.ToString();
                        var DesDesglosada = DescripcionProducto.Split('-');
                        objDetalle1.Descripcion = DesDesglosada.Count() == 2 ? DesDesglosada[0] : DescripcionProducto;
                    }
                }
                else
                {
                    objDetalle1.Descripcion = objDetalle1.AmbasDescripcionesProducto;
                }
                objDetalle1.Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Text.ToString());
                objDetalle1.d_Valor = Fila.Cells["d_Valor"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Valor"].Text.ToString());
                objDetalle1.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Text.ToString());
                objDetalle1.d_Descuento = Fila.Cells["d_Descuento"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Descuento"].Text.ToString());
                objDetalle1.Unidad = Fila.Cells["i_IdUnidadMedida"].Value == null ? "" : Fila.Cells["i_IdUnidadMedida"].Text.ToString();
                objDetalle1.d_Igvdetalle = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Text.ToString());
                objDetalle1.PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioVenta"].Text.ToString());
                objDetalle1.d_PrecioImpresion = Fila.Cells["d_PrecioImpresion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioImpresion"].Text.ToString());
                objDetalle1.Observacion = Fila.Cells["v_Observaciones"].Value == null ? "" : Fila.Cells["v_Observaciones"].Text.ToString();
                objDetalle1.Descuentos = Fila.Cells["v_FacturaRef"].Value == null ? "" : Fila.Cells["v_FacturaRef"].Text.ToString();
                objDetalle1.PrecioDscto = objDetalle1.Cantidad == 0 ? 0 : Utils.Windows.DevuelveValorRedondeado(objDetalle1.PrecioVenta / objDetalle1.Cantidad, 6);
                objDetalle1.EsServicio = prod != null ? prod.i_EsServicio ?? 0 : 0;
                objDetalle1.FormaParteOtrosTributos = prod != null ? prod.i_IndicaFormaParteOtrosTributos ?? 0 : 0;
                objDetalle1.NroPartidaArancelaria = prod != null ? prod.v_NroPartidaArancelaria : "";
                objDetalle1.CantidadLetras = SAMBHS.Common.Resource.Utils.ConvertirenLetras(objDetalle1.Cantidad.ToString()) + "(" + objDetalle1.Cantidad + ")";
                ListaDetalles.Add(objDetalle1);
            }

            return ListaDetalles;

        }

        private void btnExportarImpresion_Click(object sender, EventArgs e)
        {
            string idVentasImpresion = grdData.Rows.Select(p => p.Cells["v_IdVenta"].Value.ToString()).FirstOrDefault();
            if (string.IsNullOrEmpty(idVentasImpresion)) return;
            OperationResult objOperationResult = new OperationResult();

            //private List<ReporteDocumentoFactura>  LlenarDetallesImpresionDirecto()
            var aptitudeCertificate = LlenarDetallesImpresionDirecto(); //  new VentaBL().ReporteDocumentoVenta(ref objOperationResult, idVentasImpresion);
            var first = aptitudeCertificate.FirstOrDefault();
            #region Headers
            string[] columnas = new string[8];
            if (rbtnFormatoIngles.Checked)
            {
                columnas = new[]
                    { 
                        "UnidadSiglas","Cantidad", "Descripcion", "NroPartidaArancelaria","Precio", "d_ValorVenta"
                    };
            }
            else
            {
                columnas = new[]
                    { 
                        "UnidadSiglas","Cantidad", "Descripcion", "NroPartidaArancelaria","Precio", "d_ValorVenta"
                    };
            }
            #endregion
            string Mes = new CultureInfo("es-ES", false).DateTimeFormat.GetMonthName(
                        int.Parse(first.FechaRegistro.Month.ToString("00"))).ToUpper();
            List<ReporteDocumentoFactura> ServicioFlete = new List<ReporteDocumentoFactura>();
            List<ReporteDocumentoFactura> ServicioSeguro = new List<ReporteDocumentoFactura>();
            string AnexoDetalle = "";
            List<ReporteDocumentoFactura> Reporte = new List<ReporteDocumentoFactura>();
            var moneda = first.Moneda == "S/" ? Currency.Soles : Currency.Dolares;
            string TotalEnNumero = first.Total.ToString("0.00");
            string TotalesenLetras = "";
            Idioma _Idioma = rbtnFormatoIngles.Checked ? Idioma.Ingles : Idioma.Espaniol;
            if (moneda == Currency.Soles)
            {

                TotalesenLetras = _Idioma == Idioma.Espaniol ? Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? "SON :" + Utils.ConvertirenLetras(TotalEnNumero) + " SOLES " : "SON :" + Utils.ConvertirenLetras(TotalEnNumero) + " SOLES " + "S.E.U.O."
                    : Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? "AMOUNT :" + Utils.DecimalToWords(decimal.Parse(TotalEnNumero)) + " SOLES " : "AMOUNT:" + Utils.DecimalToWords(decimal.Parse(TotalEnNumero)) + " SOLES " + "S.E.U.O.";

            }
            if (moneda == Currency.Dolares)
            {

                TotalesenLetras = _Idioma == Idioma.Espaniol ? Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? "SON :" + Utils.ConvertirenLetras(TotalEnNumero) + " DOLARES AMERICANOS " : "SON :" + Utils.ConvertirenLetras(TotalEnNumero) + " DOLARES AMERICANOS " + "S.E.U.O."
                    : Globals.ClientSession.i_IncluirSEUOImpresionDocumentos == 0 ? "AMOUNT:" + Utils.DecimalToWords(decimal.Parse(TotalEnNumero)) + " DOLARES AMERICANOS " : "AMOUNT:" + Utils.DecimalToWords(decimal.Parse(TotalEnNumero)) + " DOLARES AMERICANOS " + "S.E.U.O.";

            }

            if (Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic || Globals.ClientSession.v_RucEmpresa == Constants.RucDemo)
            {

                Reporte = aptitudeCertificate;
                aptitudeCertificate = Reporte.Where(o => o.FormaParteOtrosTributos == 0).ToList();
                ServicioFlete = Reporte.Where(o => o.FormaParteOtrosTributos == 1 && o.AmbasDescripcionesProducto.ToUpper().Contains("FLETE")).ToList();
                ServicioSeguro = Reporte.Where(o => o.FormaParteOtrosTributos == 1 && o.AmbasDescripcionesProducto.ToUpper().Contains("SEGURO")).ToList();
                var anexdetalle = Reporte.Where(o => !string.IsNullOrEmpty(o.ObservacionDetalle)).FirstOrDefault();
                AnexoDetalle = anexdetalle != null ? anexdetalle.ObservacionDetalle : "";

            }
            var dt = Utils.ConvertToDatatable(aptitudeCertificate);
            var excel = new ExcelReport(dt, "Reporte", true);//{ Headers = heads };
            excel.AutoSizeColumns(1, 10, 10, 25, 15, 10, 10);
            //excel.SetTitle("");
            excel.SetHeaders();
            excel[2].Cells[6].Value = first.Documento;

            excel[3].Cells[2].Value = Mes + " " + first.Dia + "    " + first.Anio;
            excel[3].Cells[5].Value = first.PuntoEmbarque;


            excel[4].Cells[2].Value = uckConsiderarAliasCliente.Checked ? first.Alias : first.NombreCliente;
            excel[4].Cells[5].Value = first.PuntoDestinoEmbarque;

            excel[5].Cells[2].Value = first.NroDocCliente;
            excel[5].Cells[5].Value = first.TipoEmbarque;

            excel[6].Cells[2].Value = first.Direccion;
            excel[6].Cells[5].Value = first.CondicionPago;

            excel[36].Cells[5].Value = AnexoDetalle;

            excel[37].Cells[5].Value = first.iTipoCfr == (int)TiposMediosPagoVenta.Cfr || first.iTipoCfr == (int)TiposMediosPagoVenta.Cif ? ServicioFlete == null ? "" : "FLETE : " : "";
            excel[37].Cells[6].Value = first.iTipoCfr == (int)TiposMediosPagoVenta.Cfr || Reporte.FirstOrDefault().iTipoCfr == (int)TiposMediosPagoVenta.Cif ? !ServicioFlete.Any() ? "" : ServicioFlete.Sum(o => o.PrecioVenta).ToString() : "";


            excel[38].Cells[5].Value = first.iTipoCfr == (int)TiposMediosPagoVenta.Cif ? ServicioSeguro == null ? "" : "SEGURO :" : "";
            excel[38].Cells[6].Value = first.iTipoCfr == (int)TiposMediosPagoVenta.Cif ? !ServicioSeguro.Any() ? "" : ServicioSeguro.Sum(o => o.PrecioVenta).ToString() : "";

            excel[39].Cells[1].Value = TotalesenLetras;

            excel[40].Cells[5].Value = first.TipoCfr;
            excel[40].Cells[6].Value = first.Total;

            excel[41].Cells[5].Value = first.Moneda;
            excel[41].Cells[6].Value = first.ValorVenta;

            excel[42].Cells[5].Value = first.Moneda;
            excel[42].Cells[6].Value = first.Total;

            excel[41].Cells[1].Value = first.MarcaEmbarque;
            excel[41].Cells[3].Value = first.NroBultos + " " + first.TipoBulto;

            excel[41].Cells[4].Value = first.PesoBruto;
            excel[42].Cells[4].Value = first.PesoNeto;



            var filters = new Queue<string>();
            excel.SetData(ref objOperationResult, columnas, filters.ToArray());
            var path = Path.Combine(Path.GetTempPath(), DateTime.Now.Ticks + @".xlsx");
            excel.Generate(path);
            System.Diagnostics.Process.Start(path);
        }

        private void btnImpresionDirecta_Click(object sender, EventArgs e)
        {
            idVentasImpresion = new List<string>();
            idVentasImpresion = grdData.Rows.Select(p => p.Cells["v_IdVenta"].Value.ToString()).Distinct().ToList();
            impresionVistaPrevia = _objDocumentoBL.ImpresionVistaPrevia(int.Parse(cboDocumento.Value.ToString()),
            txtSerieDoc.Text.Trim());
            var ListaDetalles = LlenarDetallesImpresionDirecto();
            using (var frm = new frmDocumentoFactura(idVentasImpresion, impresionVistaPrevia, TiposReportes.Factura, Idioma.Espaniol, ListaDetalles, uckConsiderarAliasCliente.Checked))
            {
                if (impresionVistaPrevia)
                {
                    frm.ShowDialog();
                    frm.Activate();
                }
                else
                {

                    BtnImprimir.Enabled = false;


                }
            }

        }

        private void rbPercepcionSI_CheckedChanged(object sender, EventArgs e)
        {
            var check = rbPercepcionSI.Checked;
            gbClientePercepcion.Enabled = check;
            txtPercepcionPorcentaje.Enabled = check;
            chkArticulosAfectosPercepcion.Enabled = check;
            if (!check)
                txtPercepcionPorcentaje.Text = @"0.00";
            txtPercepcionPorcentaje.Text = string.Format("{0:##.00}", CalcularPorcentajePercepcion(rbPercepcionSI.Checked,
                rbPercepcionAgente.Checked, chkArticulosAfectosPercepcion.Checked));
            cboPercepcion.Text = string.Format("Percepción: {0}", check ? "SI" : "NO");
            CalcularTotales();
        }

        private void txtPercepcionPorcentaje_Validating(object sender, CancelEventArgs e)
        {
            decimal d;
            txtPercepcionPorcentaje.Text = decimal.TryParse(txtPercepcionPorcentaje.Text, out d) ? string.Format("{0:##.00}", d) : @"0.00";
            CalcularTotales();
        }

        private static decimal CalcularPorcentajePercepcion(bool aplicaPercepcion, bool clienteEsAgente, bool itemsAplicanPercepcion)
        {
            decimal porcentaje;
            if (aplicaPercepcion)
                if (Globals.ClientSession.i_EmpresaAfectaPercepcionVenta == 1)
                    if (clienteEsAgente)
                        porcentaje = itemsAplicanPercepcion ? 0.5m : 0m;
                    else if (itemsAplicanPercepcion)
                        porcentaje = 2m;
                    else
                        porcentaje = 0.5m;
                else
                    if (clienteEsAgente)
                        porcentaje = itemsAplicanPercepcion ? 0.5m : 0m;
                    else if (itemsAplicanPercepcion)
                        porcentaje = 2m;
                    else
                        porcentaje = 0.5m;
            else
                porcentaje = 0m;
            return porcentaje;
        }

        private void chkArticulosAfectosPercepcion_CheckStateChanged(object sender, EventArgs e)
        {
            chkArticulosAfectosPercepcion.Text = chkArticulosAfectosPercepcion.Checked ? @"Items Afectos" : @"Items No Afectos";
            CalcularTotales();
        }

        private void rbPercepcionAgente_CheckedChanged(object sender, EventArgs e)
        {
            txtPercepcionPorcentaje.Text = string.Format("{0:##.00}", CalcularPorcentajePercepcion(rbPercepcionSI.Checked,
                rbPercepcionAgente.Checked, chkArticulosAfectosPercepcion.Checked));
            CalcularTotales();
        }

        private void chkArticulosAfectosPercepcion_CheckedChanged(object sender, EventArgs e)
        {
            txtPercepcionPorcentaje.Text = string.Format("{0:##.00}", CalcularPorcentajePercepcion(rbPercepcionSI.Checked,
                rbPercepcionAgente.Checked, chkArticulosAfectosPercepcion.Checked));
            CalcularTotales();
        }

        private void cboDocumentoRef_ValueChanged(object sender, EventArgs e)
        {
            if (cboDocumentoRef.Value != null)
            {
                var idRef = cboDocumentoRef.Value.ToString();
                txtSerieDocRef.MaxLength = idRef.Equals("12") ? 20 : 4;

                if (!idRef.Equals("12") && txtSerieDocRef.TextLength > 4)
                    txtSerieDocRef.Text = txtSerieDocRef.Text.Substring(0, 4);
            }
        }

        private string CreateTxt(string idVenta)
        {
            var opeResult = new OperationResult();
            var head = _objVentasBL.ObtenerVentaCabecera(ref opeResult, idVenta);
            var details = _objVentasBL.ObtenerVentaDetalles(ref opeResult, idVenta);

            var builder = new StringBuilder();
            string[] arr =
            {
                "CAB", string.Empty,head.i_IdTipoDocumento.Value.ToString("00"), head.v_SerieDocumento, head.v_CorrelativoDocumento, head.t_FechaRegistro.Value.ToString("yyyy-MM-dd"),
                head.NroDocCliente.Length == 11 ? "6": "1", head.NroDocCliente, head.NombreCliente, "admin@company.com", head.i_IdMoneda == 1 ? "PEN" : "USD",
                "","","", head.d_Total.Value.ToString("0.00"), "0.00", "0.00", "", head.d_IGV.Value.ToString("0.00"), "", "", head.d_Total.Value.ToString("0.00"), "", "", "", "",
                "", "", "", "", "", ""
            };

            builder.AppendLine(string.Join("|", arr));

            foreach (var detail in details)
            {
                var singIgv = detail.d_Precio.Value / 1.18M;
                string[] dets =
                {
                    "DET", "NIU", detail.d_Cantidad.Value.ToString("0.00"), detail.v_CodigoInterno, "",
                    detail.v_DescripcionProducto, singIgv.ToString("0.00"), "", detail.d_Igv.Value.ToString("0.00"),
                    "10", "", "", detail.d_Precio.Value.ToString("0.00"), detail.d_ValorVenta.Value.ToString("0.00"), ""
                };

                builder.AppendLine(string.Join("|", dets));
            }

            return builder.ToString();
        }

        private void txtDocAnticipo_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            //var frm = new FrmBuscarDocumentosParaAntipo();
            //if (frm.ShowDialog() != DialogResult.OK)
            //{
            //    return;
            //}
            //var dto = frm.SelectedVenta;
            //txtDocAnticipo.Text = dto.v_SerieDocumento + "-" + dto.v_CorrelativoDocumento;
            //txtDocAnticipo.Tag = dto.v_IdVenta;
        }

        private void frmRegistroVenta_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = enviandoNubefact;
        }

        private void txtGuiaRemisionCorrelativo_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = !((short)e.KeyCode >= 48 && (short)e.KeyCode <= 57) && ((short)e.KeyCode != 188) && ((short)e.KeyCode != 9);
        }

    }
}