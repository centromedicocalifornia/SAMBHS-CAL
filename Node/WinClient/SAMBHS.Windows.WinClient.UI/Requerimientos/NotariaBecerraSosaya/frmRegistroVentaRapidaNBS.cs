#region Referencias
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
using SAMBHS.Compra.BL;
using Infragistics.Win.UltraWinGrid;
using System.Text.RegularExpressions;
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Security.BL;
using SAMBHS.Contabilidad.BL;
using LoadingClass;
using SAMBHS.Cobranza.BL;
using SAMBHS.Windows.WinClient.UI.Mantenimientos.UserControls;
using SAMBHS.Requerimientos.NBS;
using SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya;
using SAMBHS.Windows.WinClient.UI.Procesos;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;

#endregion

namespace SAMBHS.Windows.WinClient.UI.Requerimientos.NotariaBecerraSosaya
{
    public partial class frmRegistroVentaRapidaNBS : Form
    {
        #region Declaraciones / Referencias
        public delegate void VentaGuardada(string idVenta);
        public event VentaGuardada VentaGuardadaEvent;

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
        BindingList<nbs_ventakardexDto> ListaGrilla = new BindingList<nbs_ventakardexDto>();
        List<nbs_ventakardexDto> _TempDetalle_EliminarDtos = new List<nbs_ventakardexDto>();
        List<nbs_ventakardexDto> _TempDetalle_AgregarKardexDto = new List<nbs_ventakardexDto>();
        List<nbs_ventakardexDto> _TempDetalle_ModificarKardexDto = new List<nbs_ventakardexDto>();
        List<string> ListaTipoKardex = new List<string>();
        #endregion

        int _MaxV, _ActV, _idMonedaCobranza;
        string _Mode, _IdVentaGuardada;
        public string _pstrIdMovimiento_Nuevo;
        string strModo = "Nuevo", strIdVenta;
        public int IdIdentificacion = 0;
        public bool FUFUsadoenVenta = false;
        bool _EsNotadeCredito = false, NroFufKeyDown = false;

        string _idVenta, CtaRubroMercaderia = "-1", CtaRubroServicio = "-1";
        decimal Total, Redondeado, Residuo;
        bool _Redondeado = false;
        bool _guardadoSinProceso = false;
        public string IdFormatoUnicoFacturacion = null;
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

        #region PermisosBotones
        bool _btnImprimir = false;
        bool _btnGuardar = false;
        bool _btnCobrar = false;
        #endregion

        private bool EsDocumentoInternoPermiteMontoCero {
            get
            {
                if (cboDocumento.Value == null) return false;
                int i;
                var permiteCero = Globals.ClientSession.i_PermiteIncluirPreciosCeroPedido  == 1;
                i = int.TryParse(cboDocumento.Value.ToString(), out i) ? i : -1;
                var esDocInterno = !new DocumentoBL().DocumentoEsContable(i);
                return permiteCero && esDocInterno;
            }
        }

        public frmRegistroVentaRapidaNBS(string Modo, string IdVenta)
        {
            strModo = Modo;
            strIdVenta = IdVenta;
            InitializeComponent();
        }

        private void frmRegistroVentaRapidaNBS_Load(object sender, EventArgs e)
        {
            using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
            {
                cboDocumento.Enabled = !Globals.ClientSession.v_RucEmpresa.Equals("20601502535");
                UltraStatusbarManager.Inicializar(ultraStatusBar1);
                var objOperationResult = new OperationResult();
                IdIdentificacion = 0;

                this.BackColor = new GlobalFormColors().FormColor;
                panel1.BackColor = new GlobalFormColors().BannerColor;

                if (Globals.ClientSession.i_VentasMostrarEmpaque == null ||
                    Globals.ClientSession.i_VentasMostrarEmpaque == 0)
                {
                    this.grdData.DisplayLayout.Bands[0].Columns["Empaque"].Hidden = true;
                    this.grdData.DisplayLayout.Bands[0].Columns["UMEmpaque"].Hidden = true;
                }

                txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
                txtMes.Text = DateTime.Now.Month.ToString("00");

                #region Cargar Combos

                cboDocumento.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
                _ListadoComboDocumentos = Globals.CacheCombosVentaDto.cboDocumentos;
                _ListadoComboDocumentosRef = Globals.CacheCombosVentaDto.cboDocumentosRef;
                Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", Globals.CacheCombosVentaDto.cboDocumentos,
                    DropDownListAction.Select);

                Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda,
                    DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboCondicionPago, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboCondicionPago, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboEstablecimiento, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboIGV, "Value1", "Id", Globals.CacheCombosVentaDto.cboIGV,
                    DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboTipoVenta, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboTipoVenta, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboTipoOperacion, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboTipoOperacion, DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboVendedor, "Value1", "Id",
                    Globals.CacheCombosVentaDto.cboVendedor, DropDownListAction.Select);
                CargarCombosDetalle();
                Utils.Windows.LoadUltraComboEditorList(cboFormaPago, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 46, null), DropDownListAction.Select);
                Utils.Windows.LoadUltraComboEditorList(cboMonedaCobranza, "Value1", "Id", Globals.CacheCombosVentaDto.cboMoneda, DropDownListAction.Select);

                #endregion

                ObtenerListadoVentas(txtPeriodo.Text, txtMes.Text);
                dtpFechaRegistro.MaxDate =
                    DateTime.Parse(
                        (txtPeriodo.Text + "/" + txtMes.Text.Trim() + "/" +
                         (DateTime.DaysInMonth(int.Parse(txtPeriodo.Text), int.Parse(txtMes.Text.Trim()))).ToString())
                            .ToString());

                FormatoDecimalesGrilla((int)Globals.ClientSession.i_CantidadDecimales,
                    (int)Globals.ClientSession.i_PrecioDecimales);

            }


            txtSerieDoc.Text =
                _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento,
                    int.Parse(cboDocumento.Value.ToString())).Trim();
            txtCorrelativoDocIni.Text =
                _objDocumentoBL.DevolverCorrelativoPorDocumento(Globals.ClientSession.i_IdEstablecimiento,
                    int.Parse(cboDocumento.Value.ToString())).ToString("00000000").Trim();
            PredeterminarEstablecimiento(txtSerieDoc.Text);
            btnAgregar_Click(sender, e);

            LlenarCuentasRubros();
            if (grdData.Rows.Any())
            {
                grdData.Rows.First().Cells["v_CodigoInterno"].Activate();
                grdData.PerformAction(UltraGridAction.EnterEditMode);
            }

            if (cboDocumento.Value.ToString() == "1")
            {
                btnBuscarCliente.Focus();
            }
            else if (cboDocumento.Value.ToString() == "3")
            {
                ClientePublicoGeneral();
            }

            btnConfig.Visible = Globals.ClientSession.i_SystemUserId == 1;

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
            if (cboDocumento.Value != null && int.Parse(cboDocumento.Value.ToString()) != (int)DocumentType.Irpe && !string.IsNullOrEmpty(UserConfig.Default.SerieCaja) && UserConfig.Default.SerieCaja != "--Seleccionar--")
            {
                txtSerieDoc.Text = UserConfig.Default.SerieCaja;
            }
            CancelEventArgs e1 = new CancelEventArgs();
            txtSerieDoc_Validating(sender, e1);
            PredeterminarEstablecimiento(txtSerieDoc.Text.Trim());
            cboDocumento.Focus();
            cboDocumento.PerformAction(UltraComboAction.Dropdown);

        }

        #region Eventos Generales
        private void txtRucCliente_KeyDown(object sender, KeyEventArgs e)
        {


            switch (e.KeyCode)
            {
                case Keys.Enter:

                    foreach (var item in txtRucCliente.Text)
                    {
                        if (!Utils.Windows.NumeroEntero(item))
                        {
                            UltraMessageBox.Show("Número de RUC/DNI  deben ser solo números", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            _ventaDto.v_IdCliente = null;
                            _ventaDto.i_IdDireccionCliente = -1;
                            txtRazonSocial.Clear();
                            txtRucCliente.Focus();
                            txtDireccion.Clear();
                            return;
                        }

                    }

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
                            txtRazonSocial.Text = frm._RazonSocial;
                            txtRucCliente.Text = frm._NroDocumento;
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
                            txtRazonSocial.Clear();
                            txtDireccion.Clear();
                            return;
                        }
                        if (txtRucCliente.TextLength == 11 && Utils.Windows.ValidarRuc(txtRucCliente.Text) == false)
                        {
                            UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            _ventaDto.i_IdDireccionCliente =int.Parse ( DatosProveedor[6]);
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
                        //txtDireccion.Enabled = false;
                    }
                    else
                    {
                        txtRazonSocial.Text = string.Empty;
                        txtDireccion.Clear();
                    }
                    txtTipoKardex.Focus();

                    break;
                case Keys.Escape:

                    if (txtRucCliente.CanUndo())
                    {
                        txtRucCliente.Undo();
                    }

                    break;

                case Keys.F10:
                    //object sender1 = new object();
                    //EventArgs e1 = new EventArgs();
                    //btnBuscarDetraccion_Click(sender1, e1);

                    break;
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
                        row.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value != null && cboTipoOperacion.Value.ToString() != "5" ? cboTipoOperacion.Value.ToString() : "1";
                        row.Cells["i_IdCentroCosto"].Value = "0";
                        row.Cells["d_Percepcion"].Value = "0";
                        row.Cells["d_Valor"].Value = "0";
                        row.Cells["v_NroCuenta"].Value = "-1";
                        row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                        row.Cells["i_IdCentroCosto"].Value = _objVentasBL.CentroCostoDeEstablecimiento(ref objOperationResult);
                    }
                    else
                    {
                        var ultimaFila = grdData.Rows.LastOrDefault();
                        if (ultimaFila != null && !cboDocumento.Focused)
                        {
                            grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                            grdData.Focus();
                            ultimaFila.Cells["v_CodigoInterno"].Activate();
                            //ultimaFila.Cells["v_CodigoInterno"].Value = cboDocumento.Value != null && cboDocumento.Value.ToString() == "438" ? "I" : "";
                            grdData.PerformAction(UltraGridAction.EnterEditMode);

                        }
                    }
                }
                else
                {
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
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

                grdData.Focus();
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_CodigoInterno"];
                this.grdData.ActiveCell = aCell;
                //this.grdData.ActiveCell.Value = cboDocumento.Value != null && cboDocumento.Value.ToString() == "438" ? "I" : "";
                grdData.ActiveRow = aCell.Row;
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                aCell.Activate();
                grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
            }
            else
            {
                var ultimaFila = grdData.Rows.LastOrDefault();
                if (ultimaFila != null && !cboDocumento.Focused)
                {
                    grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                    grdData.Focus();
                    ultimaFila.Cells["v_CodigoInterno"].Activate();
                    // ultimaFila.Cells["v_CodigoInterno"].Value = cboDocumento.Value != null && cboDocumento.Value.ToString() == "438" ? "I" : "";
                    grdData.PerformAction(UltraGridAction.EnterEditMode);
                }
            }

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            int _limiteDocumento = DocumentoBL.ObtenerLimiteDocumento(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text.Trim());

            var filasActuales = grdData.Rows.Count;
            if (filasActuales > _limiteDocumento)
            {
                UltraMessageBox.Show("Imposible Guardar , Máximo número de items por documento alcanzado, elimine un Fila de los detalles", "Advertencia");
                return;
            }

            if (grdData.Rows.Any(fila => fila.Cells["v_NroCuenta"].Value != null && fila.Cells["v_NroCuenta"].Value.ToString() != "-1") &&
               grdData.Rows.Any(fila => fila.Cells["v_NroCuenta"].Value == null || fila.Cells["v_NroCuenta"].Value.ToString() == "-1"))
            {
                var filasVacias = grdData.Rows.Where(fila => fila.Cells["v_NroCuenta"].Value == null || fila.Cells["v_NroCuenta"].Value.ToString() == "-1").ToList();
                filasVacias.ForEach(fila => fila.Delete(false));
            }


            if (uvDatos.Validate(true, false).IsValid)
            {
                if (grdData.Rows.Any(x => x.Cells["v_NroCuenta"].Value == null) || grdData.Rows.Any(x => x.Cells["v_NroCuenta"].Value.ToString() == "-1"))
                {
                    UltraMessageBox.Show("Porfavor ingrese correctamente los Servicios", "Advertencia");
                    grdData.Rows.First(x => x.Cells["v_NroCuenta"].Value == null || x.Cells["v_NroCuenta"].Value.ToString() == "-1").Selected = true;
                    return;
                }

                string ubicacion;
                if (_objVentasBL.TieneCobranzasRealizadas(_ventaDto.v_IdVenta, out ubicacion))
                {
                    UltraMessageBox.Show("Imposible Guardar Cambios a un Documento con Cobranzas Realizadas \r" + ubicacion, "Advertencia");
                    return;
                }

                if (EsVentaAfectaDetraccion())
                {
                    if (UltraMessageBox.Show("El documento es Afecto Detracción, ¿Desea continuar?", "Advertencia", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                }

                #region ValidacionKardex


                if (txtTipoKardex.Text == "V" && ListaGrilla.Any())
                {

                    if (!ValidaCamposNulosVaciosKardex())
                    {
                        //UltraMessageBox.Show("Por favor registre los Tipos de Kardex", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //btnBuscarKardex.Focus();
                        return;
                    }
                }
                else if (txtTipoKardex.Text == "V" && !ListaGrilla.Any())
                {

                    UltraMessageBox.Show("Por favor registre los Tipos de Kardex", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnBuscarKardex.Focus();
                    return;


                }
                else if (txtTipoKardex.Text.Trim() != string.Empty && txtNroKardex.Text == string.Empty && txtTipoKardex.Text.Trim() != "V")
                {

                    UltraMessageBox.Show("Por favor registre Número de Kardex", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtNroKardex.Focus();
                    return;
                }
                else if (txtNroKardex.Text != string.Empty && txtTipoKardex.Text == string.Empty)
                {

                    UltraMessageBox.Show("Por favor registre el Tipo de Kardex", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtTipoKardex.Focus();
                    return;
                }
                else if (!EsDocumentoInternoPermiteMontoCero && ListaTipoKardex.Contains(txtTipoKardex.Text.Trim()) && 
                    (txtMonto.Text == string.Empty || decimal.Parse(txtMonto.Text) == 0))
                {

                    UltraMessageBox.Show("Por favor registre el Monto", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtMonto.Focus();
                    return;

                }
                if (txtTipoKardex.Text.Trim() != string.Empty)
                {


                    if (txtTipoKardex.Text.Trim().ToUpper() == "V" && (decimal.Parse(txtMontoVarios.Text.Trim()) != decimal.Parse(txtTotal.Text.Trim())))
                    {
                        if (UltraMessageBox.Show("El Monto del Kardex no es igual al Total ,¿Desea Continuar?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                            return;
                    }
                    else if (txtTipoKardex.Text.Trim().ToUpper() != "V" && (decimal.Parse(txtMontoVarios.Text.Trim()) != decimal.Parse(txtTotal.Text.Trim())))
                    {
                        if (decimal.Parse(txtMonto.Text.Trim()) != decimal.Parse(txtTotal.Text.Trim()))
                            if (UltraMessageBox.Show("El Monto del Kardex no es igual al Total ,¿Desea Continuar?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                                return;
                    }

                }
                #endregion

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

                if (!grdData.Rows.Any() || !grdData.Rows.Any(p => p.Cells["v_IdProductoDetalle"].Value != null && p.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000"))
                {
                    UltraMessageBox.Show("No se permite guardar mientras el detalle está vacío", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }




                if (txtNroOperacion.Enabled && string.IsNullOrEmpty(txtNroOperacion.Text.Trim()))
                {
                    MessageBox.Show("Por favor ingrese el número de operación para la cobranza con tarjeta.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtNroOperacion.Focus();
                    return;
                }

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
                        if (Fila.Cells["i_EsServicio"].Value.ToString() != "1" && !_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) && cboDocumento.Value.ToString() != "8" && Fila.Cells["i_ValidarStock"].Value.ToString() == "1")
                        {
                            var Excedente = _objVentasBL.CantidadExcedentePorVentaDetalle(int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()), Fila.Cells["v_IdProductoDetalle"].Value.ToString(), decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()), IdVentaDetalle, int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString()), Fila.Cells["v_PedidoExportacion"].Value == null ? null : Fila.Cells["v_PedidoExportacion"].Value.ToString(), Fila.Cells["v_NroSerie"].Value == null ? null : Fila.Cells["v_NroSerie"].Value.ToString(), Fila.Cells["v_NroLote"].Value == null ? null : Fila.Cells["v_NroLote"].Value.ToString());

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

                if (string.IsNullOrEmpty(_ventaDto.v_IdCliente))
                {
                    UltraMessageBox.Show("Porfavor especifique el RUC/DNI del Cliente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnBuscarCliente.Focus();
                    return;

                }
                #endregion

                using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
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
                        LlenarTemporalesKardex();
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
                        _ventaDto.t_FechaRef = DateTime.Today;
                        _ventaDto.t_FechaVencimiento = DateTime.Today;
                        _ventaDto.t_FechaRegistro = dtpFechaRegistro.DateTime;
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
                        if (!_guardadoSinProceso)
                        {
                            _ventaDto.v_SerieDocumento = txtSerieDoc.Text;
                            _ventaDto.v_CorrelativoDocumento = txtCorrelativoDocIni.Text.Trim();
                            _ventaDto.v_CorrelativoDocumentoFin = string.Empty;
                        }
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
                        _ventaDto.t_FechaOrdenCompra = DateTime.Today;
                        _ventaDto.i_IdMedioPagoVenta = -1;
                        _ventaDto.i_IdPuntoDestino = -1;
                        _ventaDto.i_IdPuntoEmbarque = -1;
                        _ventaDto.i_IdTipoEmbarque = -1;
                        _ventaDto.i_IdTipoOperacion = int.Parse(cboTipoOperacion.Value.ToString());
                        _ventaDto.i_IdTipoVenta = int.Parse(cboTipoVenta.Value.ToString());
                        _ventaDto.i_DrawBack = 0;
                        _ventaDto.v_IdVendedor = cboVendedor.Value.ToString();
                        _ventaDto.v_IdVendedorRef = "-1";
                        _ventaDto.v_NombreClienteTemporal = _ventaDto.v_IdCliente == "N002-CL000000000" ? txtRazonSocial.Text.ToString() : string.Empty;
                        //_ventaDto.v_DireccionClienteTemporal = _ventaDto.v_IdCliente == "N002-CL000000000" ? txtDireccion.Text.ToString() : string.Empty;
                        _ventaDto.v_DireccionClienteTemporal = txtDireccion.Text.ToString() ;
                        _ventaDto.NombreCliente = txtRazonSocial.Text;
                        _ventaDto.v_IdTipoKardex = txtTipoKardex.Text.Trim();
                        var x = _ventaDto.v_IdCliente;
                        LlenarTemporalesVenta();

                        _IdVentaGuardada = _objVentasBL.InsertarVenta(ref objOperationResult, _ventaDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_AgregarKardexDto, _guardadoSinProceso, FUFUsadoenVenta);
                        #endregion
                    } 
                    
                }

                if (objOperationResult.Success == 1)
                {
                    if (VentaGuardadaEvent != null) VentaGuardadaEvent(_IdVentaGuardada);
                    if (cboCondicionPago.Text.Contains("CONTADO") && !chkCobranzaMixta.Checked && !_guardadoSinProceso)
                    {
                        new CobranzaBL().RealizaCobranzaAlContado(ref objOperationResult,
                            int.Parse(cboMoneda.Value.ToString()), int.Parse(cboFormaPago.Value.ToString()),
                            txtNroOperacion.Text, decimal.Parse(txtMontoCobrar.Text), _IdVentaGuardada,
                            dtpFechaRegistro.DateTime, decimal.Parse(txtTipoCambio.Text));

                        if (objOperationResult.Success == 0)
                        {
                            UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage + "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    EdicionBarraNavegacion(true);
                    strIdVenta = _IdVentaGuardada;
                    btnGuardar.Enabled = false;
                    BtnImprimir.Enabled = true;
                    //btnCobrar.Enabled = _btnCobrar;
                    _pstrIdMovimiento_Nuevo = _IdVentaGuardada;
                    _idVenta = _pstrIdMovimiento_Nuevo;

                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // btnGenerarIrpe.Enabled = cboDocumento.Value.ToString() != "438";
                    btnNuevoMovimiento.Enabled = true;
                    btnGuardarSinProcesar.Enabled = false;
                    ListaGrilla = new BindingList<nbs_ventakardexDto>();
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
                _TempDetalle_ModificarKardexDto = new List<nbs_ventakardexDto>();
                _TempDetalle_AgregarKardexDto = new List<nbs_ventakardexDto>();
                _TempDetalle_EliminarDtos = new List<nbs_ventakardexDto>();
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
                UltraMessageBox.Show("Por favor ingrese correctamente tipo de Kardex en Detalle Kardex Varios", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (ListaGrilla.Where(p => p.v_NroKardex == null || p.v_NroKardex.Trim() == string.Empty).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente Número de Kardex Detalle Kardex Varios", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (ListaGrilla.Where(p => p.d_Monto == null || p.d_Monto == 0).Count() != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente Monto del Kardex Detalle Kardex Varios", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            int i = 0;
            foreach (var item in ListaGrilla)
            {
                item.Index = i;
                i = i + 1;
            }


            foreach (var fila1 in ListaGrilla)
            {
                foreach (var fila2 in ListaGrilla)
                {
                    if (fila2.v_TipoKardex != null && fila2.v_NroKardex != null && fila1.v_TipoKardex != null && fila1.v_NroKardex != null)


                        if (fila1.Index != fila2.Index && fila1.v_TipoKardex.ToString() == fila2.v_TipoKardex.ToString() && fila1.v_NroKardex.ToString() == fila2.v_NroKardex.ToString())
                        {
                            UltraMessageBox.Show("El Kardex no se puede repetir. : Items : " + (fila1.Index + 1).ToString("00") + "-" + (fila2.Index + 1).ToString("00"), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }

                }

            }

            return true;
        }

        private void btnBuscarDetraccion_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarCliente frm = new Mantenimientos.frmBuscarCliente("VV", txtRucCliente.Text.Trim());
            frm.ShowDialog();
            if (frm._IdCliente != null)
            {
                if (frm._IdCliente != "N002-CL000000000")
                {
                    _ventaDto.v_IdCliente = frm._IdCliente;
                    _ventaDto.i_IdDireccionCliente = frm._IdDireccionCliente;
                    txtRazonSocial.Text = frm._RazonSocial;
                    txtRucCliente.Text = frm._NroDocumento;
                    txtDireccion.Text = frm._Direccion;
                    txtRazonSocial.Enabled = false;
                    txtRucCliente.Enabled = true;
                    //txtDireccion.Enabled = false;
                    VerificarLineaCreditoCliente(_ventaDto.v_IdCliente);
                    IdIdentificacion = frm._TipoDocumento;
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
            if (btnCobrar.Enabled && btnCobrar.Visible)
            {
                var resp = MessageBox.Show("¿Seguro de Salir sin Cobrar la venta?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                e.Cancel = resp == DialogResult.No;
            }
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
                    CargarCabecera(x.Value2, false);
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
                        CargarCabecera(x.Value2, false);
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
                if (_MaxV == 0) CargarCabecera(_ListadoVentas[0].Value2, false);

                if (_ActV > 0 && _ActV <= _MaxV)
                {
                    _ActV = _ActV - 1;
                    txtCorrelativo.Text = _ListadoVentas[_ActV].Value1;
                    CargarCabecera(_ListadoVentas[_ActV].Value2, false);
                }
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_ActV >= 0 && _ActV < _MaxV)
            {
                _ActV = _ActV + 1;
                txtCorrelativo.Text = _ListadoVentas[_ActV].Value1;
                CargarCabecera(_ListadoVentas[_ActV].Value2, false);
            }
        }

        private void btnNuevoMovimiento_Click(object sender, EventArgs e)
        {
            _ventaDto = new ventaDto();

            LimpiarCabecera();
            CargarDetalle("");
            //cboDocumento.Value = UserConfig.Default.csTipoDocumentoVentaRapida;
            //cboDocumento_Leave(sender, e);
            OperationResult objOperationResult = new OperationResult();
            txtSerieDoc_Validating(sender, new CancelEventArgs());
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Venta, null, dtpFechaRegistro.DateTime, _ventaDto.v_Correlativo, 0);
            _Mode = "New";
            EdicionBarraNavegacion(false);
            txtTipoCambio.Text = _objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.DateTime);
            btnAgregar_Click(sender, e);
            btnGuardar.Enabled = true;

            gpbDatosAdicionales.Enabled = true;
            btnAgregar.Enabled = true;
            btnEliminar.Enabled = true;

            txtRucCliente.Enabled = true;

            //  txtDireccion.Enabled = false;
            txtRazonSocial.Enabled = false;
            btnBuscarCliente.Enabled = true;

            txtNroFuf.Enabled = true;
            txtNroFuf.Clear();
            cboCondicionPago.Value = Globals.CacheCombosVentaDto.cboCondicionPago.Any(p => p.Value1.Contains("CONTADO"))
                ? Globals.CacheCombosVentaDto.cboCondicionPago.FirstOrDefault(p => p.Value1.Contains("CONTADO")).Id
                : "-1";

            if (Globals.ClientSession.i_IdAlmacenPredeterminado != null)
                cboEstablecimiento.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString();

            if (cboDocumento.Value.ToString() == "3" || cboDocumento.Value.ToString() == "12")
            {
                ClientePublicoGeneral();
            }

            if (cboDocumento.Value.ToString() == "1")
            {
                btnBuscarCliente.Focus();
            }
            _guardadoSinProceso = false;
            cboDocumento.Enabled = true;
            btnGenerarIrpe.Enabled = false;
            BtnImprimir.Enabled = false;
            btnGuardarSinProcesar.Enabled = true;
            txtMontoVarios.Text = "0.00";
            txtMonto.Text = "0.00";
            if (cboDocumento.Value != null && int.Parse(cboDocumento.Value.ToString()) != (int)DocumentType.Irpe && !string.IsNullOrEmpty(UserConfig.Default.SerieCaja) && UserConfig.Default.SerieCaja != "--Seleccionar--")
            {
                txtSerieDoc.Text = UserConfig.Default.SerieCaja;
            }
            CancelEventArgs e1 = new CancelEventArgs();
            txtSerieDoc_Validating(sender, e1);
        }

        private void cboTipoOperacion_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTipoOperacion.Value.ToString() == "5")
                {
                    UltraGridColumn c = grdData.DisplayLayout.Bands[0].Columns["i_IdTipoOperacion"];
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.CellSelect;
                }
                else
                {
                    UltraGridColumn c = grdData.DisplayLayout.Bands[0].Columns["i_IdTipoOperacion"];
                    c.CellActivation = Activation.Disabled;
                    c.CellClickAction = CellClickAction.RowSelect;
                    foreach (UltraGridRow Fila in grdData.Rows)
                    {
                        Fila.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                        CalcularValoresFila(Fila);
                    }
                }

                if (cboTipoOperacion.Value.ToString() != "2")
                {
                    UltraGridColumn d = grdData.DisplayLayout.Bands[0].Columns["d_Igv"];
                    d.CellActivation = Activation.AllowEdit;
                    d.CellClickAction = CellClickAction.CellSelect;
                }
                else
                {
                    UltraGridColumn d = grdData.DisplayLayout.Bands[0].Columns["d_Igv"];
                    d.CellActivation = Activation.Disabled;
                    d.CellClickAction = CellClickAction.RowSelect;
                }
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



            foreach (var item in txtRucCliente.Text)
            {
                if (!Utils.Windows.NumeroEntero(item))
                {
                    UltraMessageBox.Show("Número de RUC/DNI  deben ser solo números", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _ventaDto.v_IdCliente = null;
                    _ventaDto.i_IdDireccionCliente = -1;
                    txtRazonSocial.Clear();
                    txtRucCliente.Focus();
                    txtDireccion.Clear();
                    return;
                }

            }

            if (string.IsNullOrEmpty(txtRucCliente.Text.Trim()))
            {
                _ventaDto.v_IdCliente = null;
                _ventaDto.i_IdDireccionCliente = -1;
                txtRazonSocial.Clear();
                txtDireccion.Clear();
            }

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

            List<string> IdVentas = new List<string> { strIdVenta };
            bool SeImprimio = false;
            if (int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.Factura)
            {
                using (Reportes.Ventas.frmDocumentoFactura frm = new Reportes.Ventas.frmDocumentoFactura(IdVentas, false))
                {
                    BtnImprimir.Enabled = false;
                }

                //frm.ShowDialog();
            }
            if (int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.Boleta)
            {
                using (Reportes.Ventas.frmDocumentoBoleta frm = new Reportes.Ventas.frmDocumentoBoleta(IdVentas, false))
                {
                    BtnImprimir.Enabled = false;
                }
                //frm.ShowDialog();
            }
            if (int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.NotaCredito)
            {
                using (Reportes.Ventas.frmDocumentoNotaCredito frm = new Reportes.Ventas.frmDocumentoNotaCredito(IdVentas, false))
                {
                    BtnImprimir.Enabled = false;
                }
                // frm.ShowDialog();
            }
            if (int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.TicketBoleta)
            {
                Reportes.Ventas.Ablimatex.frmDocumentoTicketSinRuc frm = new Reportes.Ventas.Ablimatex.frmDocumentoTicketSinRuc(IdVentas, IdIdentificacion, false);
                BtnImprimir.Enabled = false;
                // frm.ShowDialog();
            }

            if (int.Parse(cboDocumento.Value.ToString()) == (int)DocumentType.Irpe)
            {
                //using (Reportes.Ventas.frmDocumentoIrpe frm = new Reportes.Ventas.frmDocumentoIrpe(IdVentas, false))
                //{
                //    BtnImprimir.Enabled = false;
                //}
                new Reportes.Ventas.Ablimatex.TicketIrpe(IdVentas).Print();
                BtnImprimir.Enabled = false;

            }
            btnGenerarIrpe.Enabled = cboDocumento.Value.ToString() != "438";
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

            cboMonedaCobranza.Value = cboMoneda.Value;
            txtMontoCobrar.Text = CalcularMontoCobrar(int.Parse(cboMoneda.Value.ToString()),
                !string.IsNullOrEmpty(txtTotal.Text.Trim()) ? decimal.Parse(txtTotal.Text) : 0, int.Parse(cboMonedaCobranza.Value.ToString()),
                    decimal.Parse(txtTipoCambio.Text));
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

        #endregion

        #region Comportamiento de Controles
        private void txtRucCliente_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroEnteroUltraTextBox(txtRucCliente, e);
        }

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
            ComprobarExistenciaCorrelativoDocumento();
        }

        private void txtTipoCambio_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtTipoCambio, e);
        }

        private void txtSerieDoc_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSerieDoc.Text))
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
                    if (string.IsNullOrWhiteSpace(cboDocumento.Text.Trim()))
                        cboDocumento.Value = "-1";
                    else
                    {
                        var x = _ListadoComboDocumentos.Any(p => p.Id == cboDocumento.Value.ToString() || p.Id == cboDocumento.Text);
                        if (!x)
                        {
                            cboDocumento.Value = "-1";
                        }
                    }



                    #region Setea la forma de pago de las IRPES

                    if (cboDocumento.Value.ToString() == ((int)TiposDocumentos.Irpe).ToString())
                    {
                        var irpFormaPago =
                            ((List<KeyValueDTO>)cboFormaPago.DataSource).FirstOrDefault(
                                p => p.Value1.ToUpper().Contains("IRPE"));
                        cboFormaPago.Value = irpFormaPago != null ? irpFormaPago.Id : "-1";
                    }
                    else
                    {
                        var irpFormaPago =
                            ((List<KeyValueDTO>)cboFormaPago.DataSource).FirstOrDefault(
                                p => !p.Value1.ToUpper().Contains("IRPE") && p.Value1.ToUpper().Contains("EFECTIVO"));
                        cboFormaPago.Value = irpFormaPago != null ? irpFormaPago.Id : "-1";
                        cboFormaPago.Enabled = true;
                    }

                    #endregion

                    txtDireccion.Enabled = cboDocumento.Value.ToString() != "3";

                    if (cboDocumento.Value.ToString() == "3" || cboDocumento.Value.ToString() == "12" ||
                         cboDocumento.Value.ToString() == ((int)TiposDocumentos.Irpe).ToString())
                    {
                        if (txtRucCliente.Text.Trim() == string.Empty)
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

                    txtSerieDoc.Text =
                        _objDocumentoBL.DevolverSeriePorDocumento(Globals.ClientSession.i_IdEstablecimiento,
                            int.Parse(cboDocumento.Value.ToString())).Trim(); int.Parse(cboDocumento.Value.ToString()).ToString("00000000").Trim();

                    if (cboDocumento.Value != null && int.Parse(cboDocumento.Value.ToString()) != (int)DocumentType.Irpe && !string.IsNullOrEmpty(UserConfig.Default.SerieCaja) && UserConfig.Default.SerieCaja != "--Seleccionar--")
                    {
                        txtSerieDoc.Text = UserConfig.Default.SerieCaja;
                    }

                    txtCorrelativoDocIni.Text = _objDocumentoBL.CorrelativoxSerie(int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text);

                    CancelEventArgs e1 = new CancelEventArgs();
                    txtSerieDoc_Validating(sender, e1);
                    PredeterminarEstablecimiento(txtSerieDoc.Text);
                    if (!_objDocumentoBL.DocumentoEsContable(int.Parse(cboDocumento.Value.ToString())))
                    {
                        chkPrecInIGV.Checked = false;
                        chkAfectoIGV.Checked = false;
                    }
                    else
                    {
                        chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvVentas == 1;
                        chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvVentas == 1;
                    }
                }

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
                SetColorStatusBar(cboDocumento.Value as string);
                ComprobarExistenciaCorrelativoDocumento();
            }
            else
            {
                cboDocumento.Value = "-1";
            }

            if (cboDocumento.Value != null && cboDocumento.Value.ToString() != "-1" && cboDocumento.Value.ToString() != "1")
            {
                txtTipoKardex.Focus();
            }
            else if (cboDocumento.Value.ToString() == "1")
            {
                txtRucCliente.Focus();
            }
        }
        private void SetColorStatusBar(string id)
        {
            if (id == null) return;
            switch (id)
            {
                case "1":
                    ultraStatusBar1.Appearance.BackColor = Color.Green;
                    ultraStatusBar1.Appearance.BackColor2 = Color.Green;
                    break;
                case "3":
                    ultraStatusBar1.Appearance.BackColor = Color.Purple;
                    ultraStatusBar1.Appearance.BackColor2 = Color.Purple;
                    break;
                case "12":
                    ultraStatusBar1.Appearance.BackColor = Color.Yellow;
                    ultraStatusBar1.Appearance.BackColor2 = Color.Yellow;
                    break;
                default:
                    ultraStatusBar1.Appearance.BackColor = Color.FromArgb(195, 195, 195);
                    ultraStatusBar1.Appearance.BackColor2 = Color.FromArgb(195, 195, 195);
                    break;
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
            OperationResult objOperationResult = new OperationResult();
            _ListadoVentas = _objVentasBL.ObtenerListadoVentas(ref objOperationResult, pstrPeriodo, pstrMes);
            switch (strModo)
            {
                case "Edicion":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdVenta, false);
                    _idVenta = strIdVenta;
                    cboDocumento.Enabled = false;
                    txtSerieDoc.Enabled = false;
                    txtNroFuf.Enabled = false;

                    break;

                case "Nuevo":
                    if (_ListadoVentas.Count != 0)
                    {
                        _MaxV = _ListadoVentas.Count() - 1;
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
                        CargarDetalle("");
                        _MaxV = 1;
                        _ActV = 1;
                        _ventaDto = new ventaDto();
                        _movimientoDto = new movimientoDto();
                        btnNuevoMovimiento.Enabled = false;
                        EdicionBarraNavegacion(false);
                    }
                    txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Venta, _ventaDto.t_FechaRegistro, dtpFechaRegistro.DateTime, _ventaDto.v_Correlativo, 0);
                    txtTipoCambio.Text = _objVentasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.DateTime);
                    cboCondicionPago.Value = "1";



                    break;

                case "Guardado":
                    _MaxV = _ListadoVentas.Count() - 1;
                    _ActV = _MaxV;
                    if (strIdVenta == "" | strIdVenta == null)
                    {
                        CargarCabecera(_ListadoVentas[_MaxV].Value2, false);
                    }
                    else
                    {
                        CargarCabecera(strIdVenta, false);
                    }
                    btnNuevoMovimiento.Enabled = true;
                    txtNroFuf.Enabled = false;
                    break;

                case "Consulta":
                    if (_ListadoVentas.Count != 0)
                    {
                        _MaxV = _ListadoVentas.Count() - 1;
                        _ActV = _MaxV;
                        txtCorrelativo.Text = (int.Parse(_ListadoVentas[_MaxV].Value1)).ToString("00000000");
                        CargarCabecera(_ListadoVentas[_MaxV].Value2, false);
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
                    txtNroFuf.Enabled = false;
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

        public void CargarCabecera(string IdVenta, bool Irpe)
        {
            OperationResult objOperationResult = new OperationResult();
            _ventaDto = new ventaDto();
            _ventaDto = _objVentasBL.ObtenerVentaCabecera(ref objOperationResult, IdVenta);
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
                txtDireccion.Text = _ventaDto.Direccion;

                txtSerieDoc.Text = _ventaDto.v_SerieDocumento;
                txtTotal.Text = _ventaDto.d_Total.ToString();
                txtValorVenta.Text = _ventaDto.d_ValorVenta.ToString();

                dtpFechaRegistro.Value = _ventaDto.t_FechaRegistro.Value;
                cboCondicionPago.Value = _ventaDto.i_IdCondicionPago.ToString();
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
                // txtDireccion.Enabled = _ventaDto.v_IdCliente == "N002-CL000000000";
                BtnImprimir.Enabled = true;
                btnCobrar.Enabled = true;
                txtTipoKardex.Text = _ventaDto.v_IdTipoKardex;
                if (txtTipoKardex.Text.Trim() != "V" && txtTipoKardex.Text.Trim() != string.Empty)
                {
                    var NroKardex = _objVentasBL.ObtenerDetalleKardex(ref   objOperationResult, _ventaDto.v_IdVenta).FirstOrDefault();
                    txtNroKardex.Text = NroKardex.v_NroKardex;
                    txtMonto.Text = NroKardex.d_Monto.ToString();
                    txtNroKardex.Visible = true;
                    lblNumeroKardex.Visible = true;
                    lblMonto.Visible = true;
                    txtMonto.Visible = true;


                }
                else
                {
                    txtNroKardex.Visible = false;
                    txtNroKardex.Text = string.Empty;
                    lblNumeroKardex.Visible = false;

                    lblMonto.Visible = false;
                    txtMonto.Visible = false;

                }

                if (txtNroFuf.Text != string.Empty)
                {

                    btnBuscarCliente.Enabled = false;
                }

                if (!Irpe)
                {

                    _Mode = "Edit";
                    CargarDetalle(IdVenta);
                    cboDocumento.Value = _ventaDto.i_IdTipoDocumento.Value.ToString();
                    if (_ventaDto.v_NroPedido != string.Empty) RestringirEdicion(); //SI ES UNA VENTA ORIGINADA DE UN PEDIDO NO SE PUEDE EDITAR.
                    btnGuardar.Enabled = false;
                    gpbDatosAdicionales.Enabled = false;
                    grdData.Rows.ToList().ForEach(fila => fila.Activation = Activation.Disabled);
                    btnAgregar.Enabled = false;
                    btnEliminar.Enabled = false;

                }
                else
                {
                    object sender = new object();
                    EventArgs e = new EventArgs();
                    CargarDetalleDesdeFUT(_ventaDto.v_IdFormatoUnicoFacturacion);
                    txtSerieDoc.Clear();
                    txtCorrelativoDocIni.Clear();
                    cboDocumento.Value = (int)TiposDocumentos.Irpe;
                    cboDocumento_Leave(sender, e);
                    txtSerieDoc_Validating(sender, new CancelEventArgs());
                    txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Venta, null, dtpFechaRegistro.DateTime, _ventaDto.v_Correlativo, 0);
                    _Mode = "New";
                    btnGuardar.Enabled = true;
                    gpbDatosAdicionales.Enabled = true;
                    grdData.Rows.ToList().ForEach(fila => fila.Activation = Activation.AllowEdit);
                    txtRucCliente.Enabled = false;
                    // txtDireccion.Enabled = false;
                    txtRazonSocial.Enabled = false;
                    btnAgregar.Enabled = false;
                    btnEliminar.Enabled = false;
                    cboDocumento.Enabled = false;

                }

                txtRazonSocial.Text = _ventaDto.NombreCliente;
                txtRucCliente.Text = _ventaDto.NroDocCliente;
                txtDireccion.Text = _ventaDto.Direccion;
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar la compra", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void CargarDetalleDesdeFUT(string IdFormatoUnicoFacturacion)
        {
            OperationResult objOperationResult = new OperationResult();
            try
            {
                grdData.DataSource = _objVentasBL.ObtenerFormatoUnicoFacturacionDetalles(ref objOperationResult, IdFormatoUnicoFacturacion, int.Parse(cboTipoOperacion.Value.ToString()));

                foreach (UltraGridRow Fila in grdData.Rows)
                {


                    Fila.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                    Fila.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado;
                    Fila.Cells["i_RegistroTipo"].Value = "Temporal";
                    Fila.Cells["i_RegistroEstado"].Value = "Modificado";
                    CalcularValoresFila(Fila);

                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show("Hubo un error al cargar IRPE", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtSerieDoc.Clear();
            txtCorrelativoDocIni.Clear();
            txtDescuento.Clear();
            txtIGV.Clear();
            txtConcepto.Text = "VENTA DE MERCADERÍA";
            txtRazonSocial.Clear();
            txtRucCliente.Clear();
            txtDireccion.Clear();
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
            txtNroKardex.Clear();
            txtTipoKardex.Clear();
            cboDocumento.Value = Globals.ClientSession.v_RucEmpresa.Equals("20601502535") ? "438" : UserConfig.Default.csTipoDocumentoVentaRapida;
            object sender = new object();
            EventArgs e = new EventArgs();
            cboDocumento_Leave(sender, e);

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
                                _ventadetalleDto.i_IdCentroCosto = Fila.Cells["i_IdCentroCosto"].Value == null ? "" : Fila.Cells["i_IdCentroCosto"].Value.ToString();
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
                                _ventadetalleDto.v_IdFormatoUnicoFacturacionDetalle = Fila.Cells["v_IdFormatoUnicoFacturacionDetalle"].Value == null ? null : Fila.Cells["v_IdFormatoUnicoFacturacionDetalle"].Value.ToString();
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
                                _ventadetalleDto.i_IdCentroCosto = Fila.Cells["i_IdCentroCosto"].Value == null ? "" : Fila.Cells["i_IdCentroCosto"].Value.ToString();
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
                                _ventadetalleDto.v_IdFormatoUnicoFacturacionDetalle = Fila.Cells["v_IdFormatoUnicoFacturacionDetalle"].Value == null ? null : Fila.Cells["v_IdFormatoUnicoFacturacionDetalle"].Value.ToString();
                                _TempDetalle_ModificarDto.Add(_ventadetalleDto);
                            }
                            break;
                    }
                }
            }

        }


        private void LlenarTemporalesKardex()
        {
            try
            {
                if (txtTipoKardex.Text == "V")
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
                else if (ListaTipoKardex.Contains(txtTipoKardex.Text.Trim()))
                {
                    nbs_ventakardexDto objKardex = new nbs_ventakardexDto();
                    objKardex.v_TipoKardex = txtTipoKardex.Text.Trim();
                    objKardex.v_NroKardex = txtNroKardex.Text.Trim();
                    objKardex.d_Monto = txtMonto.Text.Trim() == "" ? 0 : decimal.Parse(txtMonto.Text.Trim());
                    _TempDetalle_AgregarKardexDto.Add(objKardex);

                }
            }

            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message + "Error: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
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

        private void CalcularValoresDetalle()
        {
            if (grdData.Rows.Count() == 0) return;
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                CalcularValoresFila(Fila);
            }
        }

        private void CalcularValoresFila(UltraGridRow fila)
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
            if (grdData.Rows.Count() > 0)
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
                if (!string.IsNullOrWhiteSpace(txtNroKardex.Text))
                    txtMonto.Text = txtTotal.Text;

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
                btnGuardar.Enabled = false;
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

            btnAgregar.Enabled = false;
            btnEliminar.Enabled = false;

            btnBuscarCliente.Enabled = false;
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
                _ventaDto.i_IdDireccionCliente = int.Parse(Cadena[4]);
                txtRazonSocial.Text = Cadena[2];
                txtRucCliente.Text = Cadena[3];
                txtRazonSocial.Enabled = true;
                txtDireccion.Clear();
                // txtDireccion.Enabled = true;
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
                int IdAlmacen = int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString());
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["v_IdProductoDetalle"].Value != null)
                    {
                        if (Filas[i].Cells["v_IdProductoDetalle"].Value.ToString() == Fila.Cells["v_IdProductoDetalle"].Value.ToString() && IdAlmacen == int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString()))
                        {
                            UltraMessageBox.Show("Uno de los servicios seleccionados ya existe en el detalle", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    if (txtMonto.Text.Trim() == string.Empty && txtMontoVarios.Text.Trim() == string.Empty)
                    {
                        grdData.ActiveRow.Cells["d_Precio"].Value = Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString());
                    }
                    grdData.ActiveRow.Cells["i_EsNombreEditable"].Value = Filas[i].Cells["i_NombreEditable"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_NombreEditable"].Value.ToString());
                    grdData.ActiveRow.Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                    grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                    grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = "0";
                    grdData.ActiveRow.Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                    grdData.ActiveRow.Cells["v_NroCuenta"].Value = Filas[i].Cells["NroCuentaVenta"].Value != null ? Filas[i].Cells["NroCuentaVenta"].Value.ToString() : "-1";
                    grdData.ActiveRow.Cells["i_ValidarStock"].Value = Filas[i].Cells["i_ValidarStock"].Value != null ? Filas[i].Cells["i_ValidarStock"].Value.ToString() : "0";
                    grdData.ActiveRow.Cells["i_EsAfectoPercepcion"].Value = Filas[i].Cells["i_EsAfectoPercepcion"].Value != null ? Filas[i].Cells["i_EsAfectoPercepcion"].Value.ToString() : "0";
                    grdData.ActiveRow.Cells["d_TasaPercepcion"].Value = Filas[i].Cells["d_TasaPercepcion"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_TasaPercepcion"].Value.ToString());
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
                    grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_EsAfectoDetraccion"].Value = Filas[i].Cells["i_EsAfectoDetraccion"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsAfectoDetraccion"].Value.ToString());
                    if (txtMonto.Text.Trim() == string.Empty && txtMontoVarios.Text.Trim() == string.Empty)
                    {
                        grdData.ActiveRow.Cells["d_Precio"].Value = Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString());
                    }
                    grdData.ActiveRow.Cells["i_EsNombreEditable"].Value = Filas[i].Cells["i_NombreEditable"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_NombreEditable"].Value.ToString());
                    grdData.ActiveRow.Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                    grdData.ActiveRow.Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                    grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                    grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = "0";
                    grdData.ActiveRow.Cells["v_NroCuenta"].Value = Filas[i].Cells["NroCuentaVenta"].Value != null ? Filas[i].Cells["NroCuentaVenta"].Value.ToString() : "-1";
                    grdData.ActiveRow.Cells["i_ValidarStock"].Value = Filas[i].Cells["i_ValidarStock"].Value != null ? Filas[i].Cells["i_ValidarStock"].Value.ToString() : "0";
                    grdData.ActiveRow.Cells["i_EsAfectoPercepcion"].Value = Filas[i].Cells["i_EsAfectoPercepcion"].Value != null ? Filas[i].Cells["i_EsAfectoPercepcion"].Value.ToString() : "0";
                    grdData.ActiveRow.Cells["d_TasaPercepcion"].Value = Filas[i].Cells["d_TasaPercepcion"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_TasaPercepcion"].Value.ToString());
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


            if (!EsDocumentoInternoPermiteMontoCero && grdData.Rows.Count(p => p.Cells["d_Cantidad"].Value == null || 
                decimal.Parse(p.Cells["d_Cantidad"].Value.ToString().Trim()) <= 0) != 0)
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

            if (!EsDocumentoInternoPermiteMontoCero && grdData.Rows.Count(p => p.Cells["d_Precio"].Value == null || 
                decimal.Parse(p.Cells["d_Precio"].Value.ToString().Trim()) == 0) != 0)
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

            if (grdData.ActiveCell != null && grdData.ActiveCell.Column != null && grdData.ActiveCell.Column.Key == "d_Precio")
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
                    //if (cboDocumento.Value.ToString() == "7" | cboDocumento.Value.ToString() == "8") return;
                    if (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) || cboDocumento.Value.ToString() == "8") return;
                    if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "Temporal")
                    {
                        if (grdData.ActiveRow.Cells["i_IdAlmacen"].Value == null) return;

                        if (!string.IsNullOrEmpty(e.Cell.Text) && cboDocumento.Value != null && cboDocumento.Value.ToString() != "-1" && cboDocumento.Value.ToString() == "438")
                        {
                            if (!e.Cell.Text.StartsWith("I"))
                            {
                                UltraMessageBox.Show("El Código del servicio empieza I", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }

                        if (!string.IsNullOrEmpty(e.Cell.Text) && _objVentasBL.EsValidoCodProducto(e.Cell.Text))
                        {
                            // productoshortDto Producto = _objVentasBL.DevolverArticuloPorCodInterno(e.Cell.Text, _ventaDto.v_IdCliente, int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()));
                            productoshortDto Producto = Globals.ClientSession.i_UsaListaPrecios == 1
                               ? _objVentasBL.DevolverArticuloPorCodInterno(e.Cell.Text, _ventaDto.v_IdCliente, int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()))
                               : _objVentasBL.DevolverArticuloPorCodInterno(e.Cell.Text, int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()));

                            if (Producto != null)
                            {
                                grdData.ActiveRow.Cells["v_DescripcionProducto"].Value = Producto.v_Descripcion;
                                grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = Producto.v_IdProductoDetalle;
                                grdData.ActiveRow.Cells["v_CodigoInterno"].Value = Producto.v_CodInterno;
                                grdData.ActiveRow.Cells["Empaque"].Value = Producto.d_Empaque;
                                grdData.ActiveRow.Cells["UMEmpaque"].Value = Producto.EmpaqueUnidadMedida;
                                grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                                grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                                grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = "0";
                                grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = Producto.i_IdUnidadMedida;
                                grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = Producto.i_IdUnidadMedida;
                                grdData.ActiveRow.Cells["i_EsAfectoDetraccion"].Value = Producto.i_EsAfectoDetraccion;
                                if (txtMonto.Text.Trim() == string.Empty && txtMontoVarios.Text.Trim() == string.Empty)
                                {
                                    grdData.ActiveRow.Cells["d_Precio"].Value = Producto.d_Precio;
                                }
                                grdData.ActiveRow.Cells["i_EsNombreEditable"].Value = Producto.i_NombreEditable;
                                grdData.ActiveRow.Cells["i_EsServicio"].Value = Producto.i_EsServicio;
                                grdData.ActiveRow.Cells["v_NroCuenta"].Value = Producto.NroCuentaVenta;
                                grdData.ActiveRow.Cells["i_ValidarStock"].Value = Producto.i_ValidarStock ?? 0;
                                grdData.ActiveRow.Cells["i_EsAfectoPercepcion"].Value = Producto.i_EsAfectoPercepcion ?? 0;
                                grdData.ActiveRow.Cells["d_TasaPercepcion"].Value = Producto.d_TasaPercepcion;
                                grdData.ActiveRow.Cells["d_Cantidad"].Value = "1.0000";
                            }
                            else
                            {
                                UltraMessageBox.Show("El Servicio existe Pero no tuvo ingresos a este almacén", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
                                grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = frm._UnidadMedidaEmpaque;
                                grdData.ActiveRow.Cells["i_IdTipoOperacion"].Value = cboTipoOperacion.Value.ToString();
                                grdData.ActiveRow.Cells["i_IdCentroCosto"].Value = "0";
                                grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = frm._UnidadMedidaEmpaque;
                                grdData.ActiveRow.Cells["i_EsAfectoDetraccion"].Value = frm._EsAfectoDetraccion;
                                if (txtMonto.Text.Trim() == string.Empty && txtMontoVarios.Text.Trim() == string.Empty)
                                {
                                    grdData.ActiveRow.Cells["d_Precio"].Value = frm._PrecioUnitario.ToString();//_objVentasBL.DevuelvePrecio(frm._IdProducto.ToString(), IdAlmacen, _ventaDto.v_IdCliente).ToString();
                                }
                                grdData.ActiveRow.Cells["i_EsNombreEditable"].Value = frm._NombreEditable;
                                grdData.ActiveRow.Cells["i_EsServicio"].Value = frm._EsServicio.ToString();
                                grdData.ActiveRow.Cells["v_NroCuenta"].Value = frm._NroCuentaVenta;
                                grdData.ActiveRow.Cells["i_ValidarStock"].Value = frm._ValidarStock == null ? 0 : frm._ValidarStock;
                                grdData.ActiveRow.Cells["i_EsAfectoPercepcion"].Value = frm._EsAfectoPercepcion == null ? 0 : frm._EsAfectoPercepcion;
                                grdData.ActiveRow.Cells["d_TasaPercepcion"].Value = frm._TasaPercepcion;
                                grdData.ActiveRow.Cells["d_Cantidad"].Value = "1.0000";
                            }
                        }
                        if (grdData.Rows.Count() > 0)
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
                case "d_Precio":
                    grdData.ActiveRow.Cells["d_Cantidad"].Activate();
                    grdData.PerformAction(UltraGridAction.EnterEditMode);

                    break;
                case "d_Cantidad":
                    btnAgregar_Click(sender, e);
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
            if (!grdData.Rows.Any()) return;

            if (e.Cell.Column.Key == "d_PrecioVenta" || e.Cell.Column.Key == "d_Cantidad" || e.Cell.Column.Key == "d_Precio" || e.Cell.Column.Key == "v_DescripcionProducto" || e.Cell.Column.Key == "v_NroCuenta" || e.Cell.Column.Key == "i_IdDestino" || e.Cell.Column.Key == "i_IdCentroCostos" || e.Cell.Column.Key == "i_IdAlmacen")
            {
                switch (e.Cell.Column.Key)
                {
                    case "v_NroCuenta":
                        //txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_NombreCuenta"].Value != null ? grdData.ActiveRow.Cells["_NombreCuenta"].Value.ToString() : string.Empty;
                        if (grdData.ActiveRow.Cells["EsRedondeo"].Value != null)
                            e.Cell.CancelUpdate();
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

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {

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

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(_ventaDto.v_NroPedido))
            //{
            //    if (grdData.ActiveRow == null || grdData.ActiveRow.Activation == Activation.Disabled)
            //    {
            //        btnEliminar.Enabled = false;
            //    }
            //    else
            //    {
            //        btnEliminar.Enabled = true;
            //    }
            //}
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
                if (_objVentasBL.TieneCobranzaPendiente(_ventaDto.v_IdVenta))
                {
                    btnCobrar.Enabled = true;
                }
                else
                {
                    btnCobrar.Enabled = false;
                }
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
                    txtSerieDoc.Enabled = false; //se deshabilita la serie y correlativo porque al salir de sus focos tmb validan y pueden deshabilitar o habilitar el btnGuardar tambien
                    txtCorrelativoDocIni.Enabled = false;
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
            if (e.Cell == null) return;
            if (e.Cell.Column.Key != "v_DescripcionProducto") return;
            var f = new FrmEditarDescripcion(e.Cell.Value != null ? e.Cell.Value.ToString() : string.Empty);
            f.FormClosing += delegate
            {
                e.Cell.Value = f.Descripcion;
            };

            f.ShowDialog();
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
        }

        private void frmBuscarFu_Click(object sender, EventArgs e)
        {
            NroFufKeyDown = true;
            frmBandejaFormatoUnicoFacturacion frm = new frmBandejaFormatoUnicoFacturacion("N");

            if (cboDocumento.Value == null || cboDocumento.Value.ToString() == "-1")
            {
                cboDocumento.PerformAction(UltraComboAction.Dropdown);
                return;
            }

            OperationResult objOperationResult = new OperationResult();
            frm = new frmBandejaFormatoUnicoFacturacion(cboDocumento.Value.ToString());
            frm.ShowDialog();
            if (frm.v_IdFormatoUnicoFacturacion != null || IdFormatoUnicoFacturacion != null)
            {
                if (grdData.Rows.Any())
                {
                    if (grdData.Rows.FirstOrDefault().Cells["v_IdProductoDetalle"].Value != null)
                    {
                        var resp =
                                 MessageBox.Show(
                                     string.Format(
                                         "Se perderán todos los datos contenidos en el Detalle de Venta ¿Desea Continuar?"), "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (resp == DialogResult.No)
                        {
                            return;
                        }
                        else
                        {
                            grdData.Selected.Rows.AddRange((UltraGridRow[])this.grdData.Rows.All);
                            grdData.DeleteSelectedRows(false); ;
                        }
                    }

                }

                FUFUsadoenVenta = true;
                var CabeceraFuf = frm.v_IdFormatoUnicoFacturacion != null ? FormatoUnicoFacturacionBl.ObtenerCabecera(ref objOperationResult, frm.v_IdFormatoUnicoFacturacion) : FormatoUnicoFacturacionBl.ObtenerCabecera(ref objOperationResult, IdFormatoUnicoFacturacion);
                _ventaDto.v_IdCliente = CabeceraFuf.v_IdClienteFacturar;
                txtRazonSocial.Text = CabeceraFuf.NombreClienteFacturacion;
                txtRucCliente.Text = CabeceraFuf.NroDocumentoClienteFacturacion;
                txtNroFuf.Text = CabeceraFuf.v_NroFormato;
                txtDireccion.Text = CabeceraFuf.DireccionClienteFacturacion;
                cboDocumento.Value = "-1";
                txtNroFuf.Enabled = false;
                btnBuscarCliente.Enabled = false;
                txtRucCliente.Enabled = false;
                txtRazonSocial.Enabled = false;
                _ventaDto.v_IdFormatoUnicoFacturacion = CabeceraFuf.v_IdFormatoUnicoFacturacion;
                var DetallesFuf = FormatoUnicoFacturacionBl.ObtenerDetalleFUFparaVenta(ref objOperationResult, frm.v_IdFormatoUnicoFacturacion != null ? frm.v_IdFormatoUnicoFacturacion : IdFormatoUnicoFacturacion);
                var DetallesFufAlt = new BindingList<nbs_formatounicofacturaciondetalleDto>(DetallesFuf.Where(p => p.i_FacturadoContabilidad == 0).ToList());
               // if (DetallesFufAlt.Any() && cboDocumento.Value != null && cboDocumento.Value.ToString() == "-1")
                if (DetallesFufAlt.Any())
                {
                    cboDocumento.Value =  CabeceraFuf.i_IdTipoDocumentoCliente ==6? "1":"3";
                    cboDocumento_Leave(sender, e);
                }
                else
                {
                    var DetallesFufAlt1 = new BindingList<nbs_formatounicofacturaciondetalleDto>(DetallesFuf.Where(p => p.i_FacturadoContabilidad == 1).ToList());
                    if (DetallesFufAlt1.Any())
                    {
                        cboDocumento.Value = "438";
                        cboDocumento_Leave(sender, e);
                    }
                }


                if (cboDocumento.Value.ToString() != "438")
                {
                    DetallesFuf = new BindingList<nbs_formatounicofacturaciondetalleDto>(DetallesFuf.Where(p => p.i_FacturadoContabilidad == 0).ToList());//facturas
                }
                else
                {
                    DetallesFuf = new BindingList<nbs_formatounicofacturaciondetalleDto>(DetallesFuf.Where(p => p.i_FacturadoContabilidad == 1).ToList()); //irpes
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

                        if (cboDocumento.Value.ToString() != "438")
                        {

                            grdData.ActiveRow.Cells["d_Precio"].Value = Fila.d_Importe.ToString();
                        }
                        else
                        {
                            grdData.ActiveRow.Cells["d_Precio"].Value = Fila.d_ImporteRegistral.ToString();
                        }
                        grdData.ActiveRow.Cells["i_Eliminado"].Value = "0";
                        grdData.ActiveRow.Cells["i_InsertaIdUsuario"].Value = Fila.i_InsertaIdUsuario.ToString();
                        grdData.ActiveRow.Cells["t_InsertaFecha"].Value = Fila.t_InsertaFecha.Value.ToString();
                        grdData.ActiveRow.Cells["i_EsServicio"].Value = Fila.i_EsServicio.ToString();
                        grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = Fila.i_IdUnidadMedida.ToString();
                        grdData.ActiveRow.Cells["i_RegistroTipo"].Value = "Temporal";
                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = Fila.i_IdUnidadMedida.ToString();
                        grdData.ActiveRow.Cells["v_IdFormatoUnicoFacturacionDetalle"].Value = Fila.v_IdFormatoUnicoFacturacionDetalle;
                        CalcularValoresFila(grdData.ActiveRow);

                    }

                    btnAgregar.Enabled = false;
                    btnEliminar.Enabled = false;
                    cboDocumento.Enabled = false;
                }
            }
            else
            {
                if (_ventaDto.v_IdCliente == null || frm.v_IdFormatoUnicoFacturacion == null)
                {
                    btnBuscarCliente.Enabled = true;
                    txtRucCliente.Enabled = true;
                }
                else
                {
                    btnBuscarCliente.Enabled = false;
                    txtRucCliente.Enabled = false;
                }
                IdFormatoUnicoFacturacion = null;
            }

            NroFufKeyDown = false;

        }

        private void LlenarDatosFUF()
        {
            OperationResult objOperationResult = new OperationResult();
            if (grdData.Rows.Any())
            {
                if (grdData.Rows.FirstOrDefault().Cells["v_IdProductoDetalle"].Value != null)
                {
                    var resp =
                             MessageBox.Show(
                                 string.Format(
                                     "Se perderán todos los datos contenidos en el Detalle de Venta ¿Desea Continuar?"), "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (resp == DialogResult.No)
                    {
                        return;
                    }
                    else
                    {
                        grdData.Selected.Rows.AddRange((UltraGridRow[])this.grdData.Rows.All);
                        grdData.DeleteSelectedRows(false); ;
                    }
                }

            }

            FUFUsadoenVenta = true;

            var CabeceraFuf = IdFormatoUnicoFacturacion != null ? FormatoUnicoFacturacionBl.ObtenerCabecera(ref objOperationResult, IdFormatoUnicoFacturacion) : FormatoUnicoFacturacionBl.ObtenerCabecera(ref objOperationResult, IdFormatoUnicoFacturacion);

            _ventaDto.v_IdCliente = CabeceraFuf.v_IdClienteFacturar;
            txtRazonSocial.Text = CabeceraFuf.NombreClienteFacturacion;
            txtRucCliente.Text = CabeceraFuf.NroDocumentoClienteFacturacion;
            txtNroFuf.Text = CabeceraFuf.v_NroFormato;
            txtDireccion.Text = CabeceraFuf.DireccionClienteFacturacion;
            txtNroFuf.Enabled = false;
            btnBuscarCliente.Enabled = false;
            txtRucCliente.Enabled = false;
            txtRazonSocial.Enabled = false;
            _ventaDto.v_IdFormatoUnicoFacturacion = CabeceraFuf.v_IdFormatoUnicoFacturacion;
            var DetallesFuf = FormatoUnicoFacturacionBl.ObtenerDetalleFUFparaVenta(ref objOperationResult, IdFormatoUnicoFacturacion != null ? IdFormatoUnicoFacturacion : IdFormatoUnicoFacturacion);
            object sender1 = new object();
            EventArgs e1 = new EventArgs();
            var DetallesFufAlt = new BindingList<nbs_formatounicofacturaciondetalleDto>(DetallesFuf.Where(p => p.i_FacturadoContabilidad == 0).ToList());
            if (DetallesFufAlt.Any())
            {
                cboDocumento.Value = CabeceraFuf.i_IdTipoDocumentoCliente == 6 ?  "1":"3";
                cboDocumento_Leave(sender1, e1);
            }
            else
            {
                var DetallesFufAlt1 = new BindingList<nbs_formatounicofacturaciondetalleDto>(DetallesFuf.Where(p => p.i_FacturadoContabilidad == 1).ToList());
                if (DetallesFufAlt1.Any())
                {
                    cboDocumento.Value = "438";
                    cboDocumento_Leave(sender1, e1);
                }
            }


            if (cboDocumento.Value.ToString() != "438")
            {
                DetallesFuf = new BindingList<nbs_formatounicofacturaciondetalleDto>(DetallesFuf.Where(p => p.i_FacturadoContabilidad == 0).ToList());//facturas
            }
            else
            {
                DetallesFuf = new BindingList<nbs_formatounicofacturaciondetalleDto>(DetallesFuf.Where(p => p.i_FacturadoContabilidad == 1).ToList()); //irpes
            }
            if (DetallesFuf.Count > 0)
            {
                foreach (nbs_formatounicofacturaciondetalleDto Fila in DetallesFuf)
                {
                    btnAgregar_Click(sender1, e1);

                    grdData.ActiveRow.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado;
                    grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = Fila.v_IdProductoDetalle;
                    grdData.ActiveRow.Cells["v_DescripcionProducto"].Value = Fila.DescripcionServicio;
                    grdData.ActiveRow.Cells["v_CodigoInterno"].Value = Fila.CodigoServicio;
                    grdData.ActiveRow.Cells["Empaque"].Value = "0";
                    grdData.ActiveRow.Cells["d_Cantidad"].Value = Fila.i_Cantidad.ToString();
                    grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = "-1";
                    grdData.ActiveRow.Cells["v_NroCuenta"].Value = Fila.NroCuenta;
                    grdData.ActiveRow.Cells["v_NroCuenta"].Value = Fila.NroCuenta;

                    if (cboDocumento.Value.ToString() != "438")
                    {

                        grdData.ActiveRow.Cells["d_Precio"].Value = Fila.d_Importe.ToString();
                    }
                    else
                    {
                        grdData.ActiveRow.Cells["d_Precio"].Value = Fila.d_ImporteRegistral.ToString();
                    }
                    grdData.ActiveRow.Cells["i_Eliminado"].Value = "0";
                    grdData.ActiveRow.Cells["i_InsertaIdUsuario"].Value = Fila.i_InsertaIdUsuario.ToString();
                    grdData.ActiveRow.Cells["t_InsertaFecha"].Value = Fila.t_InsertaFecha.Value.ToString();
                    grdData.ActiveRow.Cells["i_EsServicio"].Value = Fila.i_EsServicio.ToString();
                    grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = Fila.i_IdUnidadMedida.ToString();
                    grdData.ActiveRow.Cells["i_RegistroTipo"].Value = "Temporal";
                    grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                    grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = Fila.i_IdUnidadMedida.ToString();
                    grdData.ActiveRow.Cells["v_IdFormatoUnicoFacturacionDetalle"].Value = Fila.v_IdFormatoUnicoFacturacionDetalle;
                    CalcularValoresFila(grdData.ActiveRow);

                }

                btnAgregar.Enabled = false;
                btnEliminar.Enabled = false;
                cboDocumento.Enabled = false;
            }



        }

        private void txtTipoKardex_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTipoKardex.Text))
            {
                foreach (var item in ListaTipoKardex)
                {
                    if (e.KeyChar.ToString().ToUpper() == item || e.KeyChar.ToString().ToUpper() == "V" || e.KeyChar == 8)
                    {
                        e.Handled = false;
                        return;
                    }
                    e.Handled = true;
                }
            }
        }

        private void btnBuscarKardex_Click(object sender, EventArgs e)
        {

            if (txtTipoKardex.Text.Trim() == string.Empty) return;
            if (txtTipoKardex.Text == "V")
            {
                txtNroKardex.Visible = false;
                lblNumeroKardex.Visible = false;
                lblMonto.Visible = false;
                txtMonto.Visible = false;

                lblMontoVarios.Visible = true;
                txtMontoVarios.Visible = true;

                frmRegistroVentaKardex frm = new frmRegistroVentaKardex(_ventaDto.v_IdVenta, ListaGrilla, strModo, _TempDetalle_EliminarDtos);
                frm.ShowDialog();
                txtMontoVarios.Text = frm.Total.ToString();
                ListaGrilla = frm.ListaGrilla;
                _TempDetalle_EliminarDtos = frm._TempDetalle_EliminarDto;
                txtMontoVarios.Focus();

            }
            else
            {
                txtNroKardex.Visible = true;
                lblNumeroKardex.Visible = true;
                lblMonto.Visible = true;
                txtMonto.Visible = true;
                lblMontoVarios.Visible = false;
                txtMontoVarios.Visible = false;
                txtNroKardex.Focus();
            }
        }

        private void btnAnt_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["frmBandejaRegistroVenta"] != null)
            {
                var ventaBandejaForm = (frmBandejaRegistroVenta)Application.OpenForms["frmBandejaRegistroVenta"];
                var estado = ventaBandejaForm.MarcarFilaAnterior();
                if (!btnNuevoMovimiento.Enabled)
                    btnNuevoMovimiento.Enabled = estado;
            }
        }

        private void btnSig_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms["frmBandejaRegistroVenta"] != null)
            {
                var ventaBandejaForm = (frmBandejaRegistroVenta)Application.OpenForms["frmBandejaRegistroVenta"];
                var estado = ventaBandejaForm.MarcarFilaSiguiente();
                if (!btnNuevoMovimiento.Enabled)
                    btnNuevoMovimiento.Enabled = estado;
            }
        }

        private void btnGenerarIrpe_Click(object sender, EventArgs e)
        {
            btnNuevoMovimiento.Enabled = false;
            string IdVentaGuardada = _ventaDto.v_IdVenta;
            strModo = "Nuevo";

            if (IdVentaGuardada == null && _IdVentaGuardada == null)
            {
                UltraMessageBox.Show("No Existe Información para Generar Irpe", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            CargarCabecera(_IdVentaGuardada ?? IdVentaGuardada, true);

            _ventaDto.v_IdVenta = null;
            btnGenerarIrpe.Enabled = false;
            btnAgregar.Enabled = !grdData.Rows.Any();
            btnEliminar.Enabled = !grdData.Rows.Any();
            cboDocumento.Enabled = false;
            txtNroFuf.Enabled = false;
            btnBuscarCliente.Enabled = false;
            txtRucCliente.Enabled = false;
            BtnImprimir.Enabled = false;
            btnAgregar_Click(sender, e);
        }

        private void txtTipoKardex_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (!string.IsNullOrWhiteSpace(txtTipoKardex.Text))
                    {
                        btnBuscarKardex_Click(sender, e);
                    }
                    else
                    {
                        var ultimaFila = grdData.Rows.LastOrDefault();
                        if (ultimaFila != null)
                        {
                            grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                            grdData.Focus();
                            ultimaFila.Cells["v_CodigoInterno"].Activate();
                            grdData.PerformAction(UltraGridAction.EnterEditMode);
                        }
                    }

                    break;
                case Keys.F10:
                    btnBuscarKardex_Click(sender, e);
                    break;
            }


        }

        private void txtNroFuf_TextChanged(object sender, EventArgs e)
        {
            if (txtNroFuf.Text.Trim() == string.Empty)
            {
                txtNroFuf.Enabled = true;
                btnBuscarCliente.Enabled = true;
                txtRucCliente.Enabled = true;
                _ventaDto.v_IdFormatoUnicoFacturacion = null;
                FUFUsadoenVenta = false;
            }
        }

        private void btnGuardarSinProcesar_Click(object sender, EventArgs e)
        {
            var resp = MessageBox.Show("¿Seguro de guardar sin procesar la venta?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == System.Windows.Forms.DialogResult.Yes)
            {
                _guardadoSinProceso = true;
                btnGuardar_Click(sender, e);
            }
        }

        private void txtNroFuf_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtNroFuf_KeyDown(object sender, KeyEventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            object sender1 = new object();
            EventArgs e1 = new EventArgs();
            switch (e.KeyCode)
            {
                //case Keys.F10:
                //    frmBuscarFu_Click(sender1, e1);

                //    break;
                case Keys.Enter:
                    Utils.Windows.FijarFormatoUltraTextBox(txtNroFuf, "{0:00000000}");
                    if (string.IsNullOrEmpty(txtNroFuf.Text.Trim()))
                    {

                        frmBuscarFu_Click(sender1, e1);
                    }
                    else
                    {
                        nbs_formatounicofacturacionDto fuf = FormatoUnicoFacturacionBl.ObtenerFUFxNroFormato(ref objOperationResult, txtNroFuf.Text.Trim());

                        if (fuf != null)
                        {
                            NroFufKeyDown = true;
                            IdFormatoUnicoFacturacion = fuf.v_IdFormatoUnicoFacturacion;
                            //frmBuscarFu_Click(sender1, e1);
                            LlenarDatosFUF();
                        }
                        else
                        {
                            UltraMessageBox.Show("F.U.F no existe", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                    break;
            }
        }

        private void txtMonto_KeyPress(object sender, KeyPressEventArgs e)
        {

            Utils.Windows.NumeroDecimal2UltraTextBox(txtMonto, e);
        }

        private void cboMonedaCobranza_ValueChanged(object sender, EventArgs e)
        {
            if (cboMonedaCobranza.Value.ToString() == "-1") return;
            var ds = _objVentasBL.ObtenerFormasPagoPorMoneda(int.Parse(cboMonedaCobranza.Value.ToString()));
            if (ds.Any())
            {
                cboFormaPago.DataSource = ds;
                var predetEfectivo = ds.FirstOrDefault(p => p.Value1 != null && p.Value1.Contains("EFECTIVO"));
                cboFormaPago.Value = predetEfectivo != null ? predetEfectivo.Id : "-1";
            }

            txtMontoCobrar.Text = CalcularMontoCobrar(int.Parse(cboMoneda.Value.ToString()),
                !string.IsNullOrEmpty(txtTotal.Text.Trim()) ? decimal.Parse(txtTotal.Text) : 0, int.Parse(cboMonedaCobranza.Value.ToString()),
                    decimal.Parse(txtTipoCambio.Text));
        }

        private void cboFormaPago_ValueChanged(object sender, EventArgs e)
        {
            bool formaPagoRequiereNroDocumento;
            new FormaPagoDocumentoBL().DevuelveComprobantePorFormaPago(int.Parse(cboFormaPago.Value.ToString()), out formaPagoRequiereNroDocumento, out _idMonedaCobranza);

            txtNroOperacion.Enabled = formaPagoRequiereNroDocumento;
            txtPagaCon.Enabled = cboFormaPago.Text.Contains("EFECTIVO");
            chkCobranzaMixta.Checked = cboFormaPago.Text.Contains("MIXTO");
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
                txtNroOperacion.Enabled = false;
                txtNroOperacion.Clear();
                cboFormaPago.Visible = false;
                txtNroOperacion.Visible = false;
            }
            else
            {
                chkCobranzaMixta.Enabled = true;
                gbOpcionesCobranza.Enabled = true;
                btnCobrar.Visible = chkCobranzaMixta.Checked;
                cboFormaPago.Enabled = true;
                var ds = (List<KeyValueDTO>)cboFormaPago.DataSource;
                var predetEfectivo = ds.FirstOrDefault(p => p.Value1 != null && p.Value1.Contains("EFECTIVO"));
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

        private void txtNroFuf_Validating(object sender, CancelEventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtNroFuf, "{0:00000000}");
        }



        private void uvDatos_Validating(object sender, Infragistics.Win.Misc.ValidatingEventArgs e)
        {
            if (!e.Control.Visible && !e.IsValid)
            {
                UltraMessageBox.Show("Por favor llene los campos requeridos antes de guardar" + e.Control.Name, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
        }

        private void txtNroKardex_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtNroKardex, e);
            if (e.KeyChar != Convert.ToChar(Keys.Enter)) return;
            try
            {
                var data = new List<DbfConnector.KardexClienteDto>();
                Task.Factory.StartNew(() =>
                {
                    Invoke((MethodInvoker)delegate
                    {
                        txtNroKardex.Enabled = false;
                        #region Obtiene la data de los DBFs de la notaría
                        var dbfCon = new DbfConnector
                        {
                            DataClientPath = NBS_DBF_PathSettings.Default.dbf_clientref_path,
                            DataPath = NBS_DBF_PathSettings.Default.dbf_data_path,
                            DataClientFileName = NBS_DBF_PathSettings.Default.dbf_clientref_filename,
                            DataFileName = NBS_DBF_PathSettings.Default.dbf_data_filename
                        };
                        dbfCon.ErrorEvent += dbfCon_ErrorEvent;
                        UltraStatusbarManager.Mensaje(ultraStatusBar1, "Buscando clientes relacionados...", timer1);
                        var dataReturn = dbfCon.ObtenerInfo(txtNroKardex.Text.Trim(), txtTipoKardex.Text.Trim());
                        if (dataReturn != null)
                            data = dataReturn.ToList();
                        #endregion
                    });
                }).ContinueWith(t =>
                {
                    txtNroKardex.Enabled = true;
                    #region Registrar Cliente

                    UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
                    if (data == null || !data.Any())
                    {
                        UltraStatusbarManager.MarcarError(ultraStatusBar1, "No se encontró información de éste kardex.",
                            timer1);

                        //if (cboDocumento.Value.ToString().Equals("1"))
                        //    txtRucCliente.Focus();
                        //else
                        //{
                        //    if (_ventaDto.v_IdCliente != "N002-CL000000000")
                        //    {
                        //    }
                        //    else
                        //    {
                        //        ClientePublicoGeneral();
                        //        txtRazonSocial.Focus();
                        //    }
                        //}
                    }
                    else
                    {
                        var f = new FrmClientesPorKardex(data);
                        f.ShowDialog();
                        if (f.DialogResult == DialogResult.OK)
                        {
                            _ventaDto.v_IdCliente = f.ClienteDto.v_IdCliente;
                            txtRucCliente.Text = f.ClienteDto.v_NroDocIdentificacion;
                            txtRazonSocial.Text = string.Join(" ", f.ClienteDto.v_ApePaterno.Trim(),
                                f.ClienteDto.v_ApeMaterno.Trim(), f.ClienteDto.v_PrimerNombre.Trim(),
                                f.ClienteDto.v_RazonSocial.Trim());

                            txtDireccion.Text = f.ClienteDto.v_DirecPrincipal;
                            //if ((f.ClienteDto.v_NroDocIdentificacion.Length != 11 && cboDocumento.Value.ToString() == "1"))
                            //{
                            //    cboDocumento.Focus();
                            //    cboDocumento.Value = "3";
                            //    cboDocumento.PerformAction(UltraComboAction.Dropdown);
                            //}
                        }
                        else if (f.DialogResult == DialogResult.Abort)
                        {
                            UltraStatusbarManager.MarcarError(ultraStatusBar1, f.ErrorMessage, timer1);
                            var objOperationResult = new OperationResult();
                            var frm = new Mantenimientos.frmRegistroRapidoCliente(f.RucCliente, "C");
                            frm.ShowDialog();
                            if (frm._Guardado)
                            {
                                txtRucCliente.Text = frm._NroDocumentoReturn;
                                var datosProveedor = _objVentasBL.DevolverClientePorNroDocumento(ref objOperationResult, txtRucCliente.Text.Trim());
                                if (datosProveedor != null)
                                {
                                    _ventaDto.v_IdCliente = datosProveedor[0];
                                    _ventaDto.i_IdDireccionCliente =int.Parse (datosProveedor[6]);
                                    txtRazonSocial.Text = datosProveedor[2];
                                    txtDireccion.Text = datosProveedor[3];
                                    VerificarLineaCreditoCliente(_ventaDto.v_IdCliente);
                                }
                            }
                            txtRazonSocial.Enabled = false;
                            txtRucCliente.Enabled = true;
                            // txtDireccion.Enabled = false;
                        }
                        txtMonto.Focus();
                        txtMonto.Clear();
                    }

                    #endregion
                }, TaskScheduler.FromCurrentSynchronizationContext());
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

        private void btnConfig_Click(object sender, EventArgs e)
        {
            var f = new frmConfigurarDbfConnection();
            f.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UltraStatusbarManager.Reestablecer(ultraStatusBar1, timer1);
        }

        private void txtNroKardex_Validating(object sender, CancelEventArgs e)
        {
            txtNroKardex.Text = txtNroKardex.Text.Trim();
        }

        private void txtMonto_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (grdData.Rows.Any())
                    {
                        if (string.IsNullOrEmpty(txtNroFuf.Text))
                        {
                            var filasVacias = grdData.Rows.FirstOrDefault();
                            filasVacias.Cells["d_Precio"].Value = txtTipoKardex.Text.Trim() == "V" ? string.IsNullOrEmpty(txtMontoVarios.Text.Trim()) ? "0.00" : txtMontoVarios.Text.ToString() : string.IsNullOrEmpty(txtMonto.Text.Trim()) ? "0.00" : txtMonto.Text.ToString();
                        }
                        var ultimaFila = grdData.Rows.LastOrDefault();
                        if (ultimaFila != null)
                        {
                            grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                            grdData.Focus();
                            ultimaFila.Cells["v_CodigoInterno"].Activate();
                            grdData.PerformAction(UltraGridAction.EnterEditMode);
                        }
                    }
                    else
                    {
                        EventArgs e1 = new EventArgs();
                        btnAgregar_Click(sender, e1);
                    }
                    break;
            }
        }

        private void txtMontoVarios_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    txtMonto_KeyDown(sender, e);
                    break;
            }
        }

        private void txtNroKardex_Leave(object sender, EventArgs e)
        {
            txtMonto.Focus();
            txtMonto.Text = string.Empty;
        }

        private void txtTipoKardex_Leave(object sender, EventArgs e)
        {
            //if (txtTipoKardex.Text.Trim() == string.Empty)
            //{
            //    if (cboDocumento.Value.ToString().Equals("1"))
            //    {
            //        txtRucCliente.Focus();
            //    }
            //    else
            //    {
            //        txtRazonSocial.Focus();
            //    }
            //}
        }

        private void txtNroKardex_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtNroKardex, "{0:000000}");
        }

        private void txtRazonSocial_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (grdData.Rows.Any())
                {
                    var primeraFila = grdData.Rows.FirstOrDefault();
                    if (primeraFila != null)
                    {
                        primeraFila.Cells["v_CodigoInterno"].Activate();
                        grdData.PerformAction(UltraGridAction.EnterEditMode);
                    }
                }
            }
        }

        private void txtRucCliente_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {

            if (e.Button.Key == "btnActualizarCliente")
            {
                if (string.IsNullOrEmpty(_ventaDto.v_IdCliente))
                {
                    UltraMessageBox.Show("Solo se actualiza si  los datos del cliente ya han sido obtenidos en este formulario", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (txtRucCliente.Text.Trim().Length == 11 && Utils.Windows.ValidarRuc(txtRucCliente.Text.Trim()))
                {
                    string[] _Contribuyente = new string[10];

                    frmCustomerCapchaSUNAT frm = new frmCustomerCapchaSUNAT(txtRucCliente.Text.Trim());
                    frm.ShowDialog();
                    if (frm.ConectadoRecibido)
                    {
                        _Contribuyente = frm.DatosContribuyente;

                        var revision = string.Format("Estado: {0} | Condición: {1}", _Contribuyente[3], _Contribuyente[4]);
                        UltraStatusbarManager.Mensaje(ultraStatusBar1, revision, timer1);
                        ClienteBL.ActualizarDatosCliente(_ventaDto.v_IdCliente, _Contribuyente[5], _Contribuyente[0]);
                        KeyEventArgs e1 = new KeyEventArgs(Keys.Enter);
                        txtRucCliente_KeyDown(sender, e1);
                    }
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtNroOperacion_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (!string.IsNullOrEmpty(txtNroOperacion.Text.Trim()))
                    {
                        if (grdData.Rows.Any())
                        {
                            //var filasVacias = grdData.Rows.FirstOrDefault();
                            //filasVacias.Cells["v_CodigoInterno"].Value = txtTipoKardex.Text.Trim() == "V" ? string.IsNullOrEmpty(txtMontoVarios.Text.Trim()) ? "0.00" : txtMontoVarios.Text.ToString() : string.IsNullOrEmpty(txtMonto.Text.Trim()) ? "0.00" : txtMonto.Text.ToString();
                            var ultimaFila = grdData.Rows.LastOrDefault();
                            if (ultimaFila != null)
                            {
                                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                                grdData.Focus();
                                ultimaFila.Cells["v_CodigoInterno"].Activate();
                                grdData.PerformAction(UltraGridAction.EnterEditMode);
                            }

                        }
                        else
                        {
                            EventArgs e1 = new EventArgs();
                            btnAgregar_Click(sender, e1);

                        }
                    }
                    break;
            }


        }

        private void txtDireccion_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_ventaDto.v_IdCliente)) return;
                switch (e.Button.Key)
                {
                    case "btnEditar":
                        txtDireccion.ReadOnly = false;
                        txtDireccion.ButtonsRight[0].Enabled = false;
                        txtDireccion.ButtonsRight[1].Enabled = true;
                        txtDireccion.Focus();
                        break;

                    case "btnGuardar":
                        txtDireccion.ReadOnly = true;
                        txtDireccion.ButtonsRight[0].Enabled = true;
                        txtDireccion.ButtonsRight[1].Enabled = false;
                        ClienteBL.ActualizarDireccionCliente(_ventaDto.v_IdCliente, txtDireccion.Text.Trim());
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmRegistroVentaRapidaNBS_FormClosed(object sender, FormClosedEventArgs e)
        {
            uvDatos.Dispose();
        }

        protected virtual void OnVentaGuardadaEvent(string idventa)
        {
            var handler = VentaGuardadaEvent;
            if (handler != null) handler(idventa);
        }
    }
}

