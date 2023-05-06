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
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Contabilidad.BL;
using LoadingClass;
using SAMBHS.Letras.BL;
using SAMBHS.Windows.WinClient.UI.Requerimientos;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    /// <summary>
    /// Eduardo Quiroz Cosme 20/11/2014
    /// </summary>
    public partial class frmRegistroCompra : Form
    {
        readonly UltraCombo ucUnidadMedida = new UltraCombo();
        readonly UltraCombo ucDestino = new UltraCombo();
        readonly UltraCombo ucAlmacen = new UltraCombo();
        UltraCombo ucRubro = new UltraCombo();
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        readonly EstablecimientoBL _objEstablecimientoBL = new EstablecimientoBL();
        readonly DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        readonly DocumentoBL _objDocumentoBL = new DocumentoBL();
        readonly ComprasBL _objComprasBL = new ComprasBL();

        NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        movimientoDto _movimientoDto = new movimientoDto();
        movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
        compraDto _compraDto = new compraDto();
        compradetalleDto _compradetalleDto = new compradetalleDto();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        List<GridKeyValueDTO> _ListadoComboDocumentosRef = new List<GridKeyValueDTO>();
        List<KeyValueDTO> _ListadoCompras = new List<KeyValueDTO>();
        guiaremisioncompraDto _guiaremisioncompraDto = new guiaremisioncompraDto();

        int _MaxV, _ActV;
        string _Mode;
        public string _pstrIdMovimiento_Nuevo;
        string strModo = "Nuevo", strIdcompra, CtaRubroMercaderia = "-1", CtaRubroServicio = "-1";
        string _EstadoActual;
        bool _EsNotadeCredito = false;
        bool GuiaRemisionImportada = false;
        private decimal valorOriginalCelda;
        readonly string Utilizado = string.Empty;

        #region Temporales DetalleCompra
        List<compradetalleDto> _TempDetalle_AgregarDto = new List<compradetalleDto>();
        List<compradetalleDto> _TempDetalle_ModificarDto = new List<compradetalleDto>();
        List<compradetalleDto> _TempDetalle_EliminarDto = new List<compradetalleDto>();
        #endregion

        #region Temporales NotaIngresoDetalle
        List<movimientodetalleDto> __TempDetalle_EliminarDto = new List<movimientodetalleDto>();
        #endregion

        public frmRegistroCompra(string Modo, string IdCompra, string UtilizadoEn = null)
        {
            strModo = Modo;
            strIdcompra = IdCompra;
            Utilizado = UtilizadoEn;
            InitializeComponent();

        }

        private void frmRegistroCompra_Load(object sender, EventArgs e)
        {
            Utils.Windows.SetearLimiteFechas(new[] { dtpFechaRegistro, dtpFechaEmision });
            dtpFechaRegistro.MinDate = new DateTime(1999,1,1);
            dtpFechaEmision.MinDate = new DateTime(1999, 1, 1); 
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            OperationResult objOperationResult = new OperationResult();
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            txtMes.Text = DateTime.Now.Month.ToString("00");

            #region Control de Acciones
            if (new CierreMensualBL().VerificarMesCerrado(Globals.ClientSession.i_Periodo.ToString().Trim(), DateTime.Now.Month.ToString("00").Trim(), (int)ModulosSistema.Compras) || Utilizado == "KARDEX")
            {
                btnGuardar.Visible = false;
                this.Text = Utilizado == "KARDEX" ? "Registro de Compra" : "Registro de Compra [MES CERRADO]";
                if (Utilizado == "KARDEX")
                {

                    btnSalir.Visible = false;
                    btnAgregar.Visible = false;
                    btnEliminar.Visible = false;
                    btnPagar.Visible = false;

                }
            }
            else
            {
                btnGuardar.Visible = true;
                this.Text = "Registro de Compra";
            }
            #endregion
            #region Cargar Combos
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGridCompras(ref objOperationResult);
            //_ListadoComboDocumentosRef = _objDocumentoBL.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 1, 0);
            _ListadoComboDocumentosRef = _objDocumentoBL.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 1, 0).Where(x => !x.EsDocInterno).ToList();
            Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(cboDocumentoRef, "Value2", "Id", _ListadoComboDocumentosRef, DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstado, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 26, null), DropDownListAction.Select);
            //Utils.Windows.LoadUltraComboEditorList(cboCondicionPago, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 23, null), DropDownListAction.Select);
            if (Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic)
            {
                Utils.Windows.LoadUltraComboEditorList(cboCondicionPago, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 23, null).Where(o => o.Id == "2" || o.Id == "3").ToList(), DropDownListAction.Select);
            }
            else
            {
                Utils.Windows.LoadUltraComboEditorList(cboCondicionPago, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 23, null), DropDownListAction.Select);
            }

            Utils.Windows.LoadUltraComboEditorList(cboDestino, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchyForComboWithIDValueDto(ref objOperationResult, 24, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", _objEstablecimientoBL.ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboIGV, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 27, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboTipoCompra, "Value1", "Id", _objComprasBL.ObtenerConceptosParaCombo(ref objOperationResult, 1, null), DropDownListAction.Select);
            cboDocumento.Value = "-1";
            cboDocumentoRef.Value = "-1";
            CargarCombosDetalle();
            #endregion

            ConfigurarGrilla();
            ObtenerListadoCompras(txtPeriodo.Text, txtMes.Text);
            FormatoDecimalesGrilla((int)Globals.ClientSession.i_CantidadDecimales, (int)Globals.ClientSession.i_PrecioDecimales);
            if (_compraDto.v_IdDocumentoReferencia != null) btnGuardar.Enabled = false;
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();
            if (Application.OpenForms["frmMaster"] != null) ((frmMaster)Application.OpenForms["frmMaster"]).Activate();
            LlenarCuentasRubros();

            #region txtRucCLiente
            txtRucProveedor.LoadConfig("V");
            txtRucProveedor.ItemSelectedAfterDropClosed += delegate
            {
                txtRucProveedor_KeyDown(null, new KeyEventArgs(Keys.Enter));
            };
            #endregion

            ultraButton1.Visible = Globals.ClientSession.i_SystemUserId == 1;
        }

        #region Eventos Generales
        private void cboEstado_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtCorrelativoDocRef_Validating(object sender, CancelEventArgs e)
        {
            if (txtCorrelativoDocRef.CanUndo() == true)
            {
                if (grdData.Rows.Count() != 0)
                {
                    if (UltraMessageBox.Show("Ya se ha cargado un documento previamente, ¿Desea cargar este otro documento?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.No)
                    {
                        e.Cancel = true;
                        txtCorrelativoDocRef.Undo();
                        txtCorrelativoDocRef.ClearUndo();
                    }
                }
            }
        }

        private void txtCorrelativoDocRef_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDocRef, "{0:00000000}");
            if (_Mode == "New" || string.IsNullOrEmpty(txtCorrelativoDocRef.Text.Trim()))
            {
                DevuelveCompraReferencia();
            }
        }

        private void txtRucProveedor_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRucProveedor.Text.Trim()))
            {
                if (cboDocumento.Value.ToString() == "1")
                {
                    if (txtRucProveedor.TextLength != 11)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _compraDto.v_IdProveedor = null;
                        txtCodigoProveedor.Clear();
                        txtRazonSocial.Clear();
                        txtRucProveedor.Focus();
                    }
                    else
                    {
                        if (Utils.Windows.ValidarRuc(txtRucProveedor.Text.Trim()) != true)
                        {
                            UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _compraDto.v_IdProveedor = null;
                            txtCodigoProveedor.Clear();
                            txtRazonSocial.Clear();
                            txtRucProveedor.Focus();
                        }
                    }
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Any() && grdData.Rows[grdData.Rows.Count() - 1].Cells["v_NroCuenta"].Value == "-1") return;

            if (decimal.Parse(txtTipoCambio.Text.Trim()) <= 0)
            {
                UltraMessageBox.Show("Por favor ingrese un tipo de cambio válido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtTipoCambio.Focus();
                return;
            }
            if (cboIGV.Value == "-1")
            {
                UltraMessageBox.Show("Por favor seleccione un valor para el IGV", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboIGV.Focus();
                return;
            }
            if (grdData.ActiveRow != null)
            {
                if (chkDuplicar.Checked != true)
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
                        row.Cells["i_IdCentroCostos"].Value = "0";
                        row.Cells["i_IdCentroCostos"].Activation = Activation.Disabled;
                        row.Cells["i_IdDestino"].Value = cboDestino.Value.ToString();
                        row.Cells["i_Anticipio"].Value = "0";
                        row.Cells["t_FechaCaducidad"].Value = DateTime.Parse(Constants.FechaNula);
                        row.Cells["t_FechaFabricacion"].Value = DateTime.Parse(Constants.FechaNula);
                        row.Cells["v_NroCuenta"].Value = "-1";
                        row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    }
                }
                else
                {
                    UltraGridRow FilaDuplicada = grdData.ActiveRow;
                    if (grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null)
                    {
                        UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                        grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                        this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                        row.Cells["i_RegistroEstado"].Value = "Agregado";
                        row.Cells["i_RegistroTipo"].Value = "Temporal";
                        row.Cells["i_IdUnidadMedida"].Value = "-1";
                        row.Cells["d_Cantidad"].Value = "0";
                        row.Cells["d_Precio"].Value = "0";
                        row.Cells["v_NroCuenta"].Value = "-1";
                        row.Cells["i_IdUnidadMedida"].Value = "-1";
                        row.Cells["i_Anticipio"].Value = "0";
                        row.Cells["v_NroCuenta"].Value = FilaDuplicada.Cells["v_NroCuenta"].Value;
                        row.Cells["d_ValorVenta"].Value = FilaDuplicada.Cells["d_ValorVenta"].Value;
                        row.Cells["d_PrecioVenta"].Value = FilaDuplicada.Cells["d_PrecioVenta"].Value;
                        row.Cells["d_Igv"].Value = FilaDuplicada.Cells["d_Igv"].Value;
                        row.Cells["i_IdDestino"].Value = FilaDuplicada.Cells["i_IdDestino"].Value;
                        row.Cells["i_IdCentroCostos"].Value = FilaDuplicada.Cells["i_IdCentroCostos"].Value;
                        row.Cells["i_IdCentroCostos"].Activation = Activation.Disabled;
                        row.Cells["i_IdDestino"].Value = cboDestino.Value.ToString();
                        row.Cells["v_NroGuiaRemision"].Value = FilaDuplicada.Cells["v_NroGuiaRemision"].Value;
                        row.Cells["v_NroPedido"].Value = FilaDuplicada.Cells["v_NroPedido"].Value;
                        row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                        row.Cells["t_FechaCaducidad"].Value = DateTime.Parse(Constants.FechaNula);
                        row.Cells["t_FechaFabricacion"].Value = DateTime.Parse(Constants.FechaNula);
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
                row.Cells["i_IdUnidadMedida"].Value = "-1";
                row.Cells["d_Cantidad"].Value = "0";
                row.Cells["d_Precio"].Value = "0";
                row.Cells["i_IdCentroCostos"].Value = "0";
                row.Cells["i_IdCentroCostos"].Activation = Activation.Disabled;
                row.Cells["i_IdDestino"].Value = cboDestino.Value.ToString();
                row.Cells["i_IdUnidadMedida"].Value = "-1";
                row.Cells["i_Anticipio"].Value = "0";
                row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                row.Cells["t_FechaCaducidad"].Value = DateTime.Parse(Constants.FechaNula);
                row.Cells["t_FechaFabricacion"].Value = DateTime.Parse(Constants.FechaNula);
            }
            //UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_NroCuenta"];
            //this.grdData.ActiveCell = aCell;
            //grdData.Focus();
            //grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);


            var aCell = grdData.ActiveRow.Cells["v_CodigoInterno"];
            grdData.Focus();
            grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
            grdData.ActiveCell = aCell;
            grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();

            if (uvDatos.Validate(true, false).IsValid && _compraDto.i_IdEstado != 0)
            {
                string nroCuenta;
                if (!ValidarCuentas(grdData.Rows.ToList(), out nroCuenta))
                {
                    MessageBox.Show(string.Format("La cuenta {0} de la línea de un articulo ingresado no existe!", nroCuenta), "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }



                if (cboDocumento.Value != null && (int)TiposDocumentos.BME == int.Parse(cboDocumento.Value.ToString()))
                {
                    if (txtSerieDoc.Text.Trim() != "0001" && txtSerieDoc.Text.Trim() != "0002" && txtSerieDoc.Text.Trim() != "0003" && txtSerieDoc.Text.Trim() != "0004" && txtSerieDoc.Text.Trim() != "0005")
                    {
                        MessageBox.Show("La serie de Boleto Aéreo es incorrecto", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }



                }

                if (rbDetraccionSI.Checked)
                {

                    if (txtCodigoDetraccion.Text.Trim() == string.Empty | txtNombreDetraccion.Text.Trim() == string.Empty)
                    {
                        UltraMessageBox.Show("Por favor ingrese correctamente la Detracción", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    txtPorcDetraccion.Text = txtPorcDetraccion.Text == string.Empty ? "0" : txtPorcDetraccion.Text;
                    if (decimal.Parse(txtPorcDetraccion.Text.Trim()) == 0)
                    {
                        UltraMessageBox.Show("Por favor ingrese correctamente la Detracción", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                if (_Mode != "New")
                {
                    if (new LetrasBL().CompraFueCanjeadaALetras(ref objOperationResult, _compraDto.v_IdCompra))
                    {
                        UltraMessageBox.Show("Esta compra fue canjeada en letras, no se puede eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }

                if (cboDocumento.Value.ToString() != "-1")
                {
                    if (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) || cboDocumento.Value.ToString() == "8")
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
                decimal TipoCambioFecha = decimal.Parse(_objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date));

                if (TipoCambioFecha != decimal.Parse(txtTipoCambio.Text) && TipoCambioFecha>0)
                {
                    if (UltraMessageBox.Show("El tipo de cambio es diferente al de la Fecha Emisión ¿Desea continuar?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                }
        

                if (int.Parse(cboEstado.Value.ToString()) == 1)
                {
                    if (_compraDto.i_IdEstado == 0)
                    {
                        UltraMessageBox.Show("No es posible Activar un documento Anulado anteriormente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                if (_objComprasBL.TienePagosRealizados(_compraDto.v_IdCompra) == true)
                {
                    UltraMessageBox.Show("Imposible Guardar Cambios a un Documento con Pagos Realizados", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) || cboDocumento.Value.ToString() == "8")
                {
                    if (txtCorrelativoDocRef.Text.Trim() == "" | txtSerieDocRef.Text.Trim() == "" | cboDocumentoRef.Value.ToString() == "-1")
                    {
                        UltraMessageBox.Show("Por favor ingrese correctamente el documento de referencia", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                if (!grdData.Rows.Any())
                {
                    UltraMessageBox.Show("Por Favor ingrese almenos una fila al detalle del documento.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (ValidaCamposNulosVacios())
                {
                    foreach (UltraGridRow Fila in grdData.Rows)
                    {
                        if (Fila.Cells["v_NroCuenta"].Value == null)
                        {
                            UltraMessageBox.Show("Por favor ingrese correctamente todas las cuentas al detalle.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString()) <= 0)
                        {
                            UltraMessageBox.Show("Todos los Totales no están calculados.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (Fila.Cells["v_IdProductoDetalle"].Value != null && Fila.Cells["i_IdAlmacen"].Value == null)
                        {
                            UltraMessageBox.Show("Por favor ingrese correctamente los almacenes al detalle.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (Fila.Cells["i_IdAlmacen"].Value == null && Fila.Cells["v_IdProductoDetalle"].Value != null)
                        {
                            UltraMessageBox.Show("Porfavor especifique los Almacenes correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
                CalcularValoresDetalle();
                grdData.Rows.ToList().ForEach(f => f.Cells["i_RegistroEstado"].Value = "Modificado");
                #region Nuevo
                if (_Mode == "New")
                {
                    using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                    {
                        using (var ts = TransactionUtils.CreateTransactionScope())
                        {
                            while (_objComprasBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text) ==
                                   false)
                            {
                                txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                            }

                            #region Guarda Entidad Compra

                            _compraDto.d_TipoCambio = txtTipoCambio.Text == string.Empty
                                ? 0
                                : decimal.Parse(txtTipoCambio.Text);
                            _compraDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                            _compraDto.v_Glosa = txtGlosa.Text.Trim();
                            _compraDto.v_Mes = int.Parse(txtMes.Text.Trim()).ToString("00");
                            _compraDto.v_Periodo = txtPeriodo.Text.Trim();
                            _compraDto.v_Correlativo = txtCorrelativo.Text;
                            _compraDto.CodigoProveedor = txtCodigoProveedor.Text;
                            _compraDto.d_Anticipio = txtAnticipio.Text != string.Empty
                                ? decimal.Parse(txtAnticipio.Text)
                                : 0;
                            _compraDto.d_IGV = txtIGV.Text != string.Empty ? decimal.Parse(txtIGV.Text) : 0;
                            _compraDto.d_PorcentajeDetraccion = txtPorcDetraccion.Text != string.Empty
                                ? decimal.Parse(txtPorcDetraccion.Text)
                                : 0;
                            _compraDto.d_TipoCambio = txtTipoCambio.Text != string.Empty
                                ? decimal.Parse(txtTipoCambio.Text)
                                : 0;
                            _compraDto.d_Total = txtTotal.Text != string.Empty ? decimal.Parse(txtTotal.Text) : 0;
                            _compraDto.d_ValorVenta = txtValorVenta.Text != string.Empty
                                ? decimal.Parse(txtValorVenta.Text)
                                : 0;
                            _compraDto.i_DeduccionAnticipio = chkDeduccionAnticipio.Checked == true ? 1 : 0;
                            _compraDto.i_EsAfectoIgv = chkAfectoIGV.Checked == true ? 1 : 0;
                            _compraDto.i_EsDetraccion = rbDetraccionSI.Checked == true ? 1 : 0;
                            _compraDto.t_FechaDetraccion = dtpFchDetraccion.Value;
                            _compraDto.t_FechaRef = dtpFechaRef.Value;
                            _compraDto.t_FechaVencimiento = dtpFechaVencimiento.Value;
                            _compraDto.t_FechaRegistro = dtpFechaRegistro.Value.Date;
                            _compraDto.t_FechaEmision = dtpFechaEmision.Value.Date;
                            _compraDto.i_IdCondicionPago = int.Parse(cboCondicionPago.Value.ToString());
                            _compraDto.i_IdDestino = int.Parse(cboDestino.Value.ToString());
                            _compraDto.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());
                            _compraDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                            _compraDto.i_IdIgv = int.Parse(cboIGV.Value.ToString());
                            _compraDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                            _compraDto.i_IdTipoCompra = int.Parse(cboTipoCompra.Value.ToString());
                            _compraDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                            _compraDto.i_IdTipoDocumentoRef = int.Parse(cboDocumentoRef.Value.ToString());
                            _compraDto.i_PreciosIncluyenIgv = chkPrecInIGV.Checked == true ? 1 : 0;
                            _compraDto.i_CodigoDetraccion = txtCodigoDetraccion.Text != string.Empty
                                ? int.Parse(txtCodigoDetraccion.Text)
                                : 0;
                            _compraDto.v_Correlativo = txtCorrelativo.Text;
                            _compraDto.v_CorrelativoDocumento = txtCorrelativoDoc.Text;
                            _compraDto.v_CorrelativoDocumentoRef = txtCorrelativoDocRef.Text;
                            _compraDto.v_Glosa = txtGlosa.Text;
                            _compraDto.v_Mes = txtMes.Text;
                            _compraDto.v_NroDetraccion = txtNroDetraccion.Text;
                            _compraDto.v_Periodo = txtPeriodo.Text;
                            _compraDto.v_SerieDocumento = txtSerieDoc.Text;
                            _compraDto.v_SerieDocumentoRef = txtSerieDocRef.Text;
                            _compraDto.v_GuiaRemisionSerie = txtSerieGR.Text;
                            _compraDto.v_GuiaRemisionCorrelativo = txtCorrelativoGR.Text;
                            _compraDto.v_ODCCorrelativo = txtCorrelativoODC.Text.Trim();
                            _compraDto.v_ODCSerie = txtSerieODC.Text.Trim();
                            _compraDto.NombreProveedor = txtRazonSocial.Text;
                            _compraDto.d_total_isc = decimal.Parse(txtISC.Text);
                            _compraDto.d_total_otrostributos = decimal.Parse(txtOtrosAtributos.Text);
                            LlenarTemporalesCompra();
                            strIdcompra = _objComprasBL.InsertarCompra(ref objOperationResult, _compraDto,
                                Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, null);

                            #endregion

                            if (objOperationResult.Success == 0)
                            {
                                UltraMessageBox.Show(
                                    objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage +
                                    "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                _TempDetalle_AgregarDto = new List<compradetalleDto>();
                                _TempDetalle_ModificarDto = new List<compradetalleDto>();
                                _TempDetalle_EliminarDto = new List<compradetalleDto>();
                                return;
                            }
                            ts.Complete();
                        }

                    }
                }
                #endregion
                #region Editar
                else if (_Mode == "Edit")
                {
                    string RegTesoreria;
                    if (_objComprasBL.ComprobarSiFueLlamadoEnTesoreria(_compraDto.v_IdCompra, out RegTesoreria) == false)
                    {
                        using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                        {
                            using (var ts = TransactionUtils.CreateTransactionScope())
                            {
                                #region Actualiza Entidad Compra
                                _compraDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                                _compraDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                                _compraDto.v_Glosa = txtGlosa.Text.Trim();
                                _compraDto.v_Mes = int.Parse(txtMes.Text.Trim()).ToString("00");
                                _compraDto.v_Periodo = txtPeriodo.Text.Trim();
                                _compraDto.v_Correlativo = txtCorrelativo.Text;
                                _compraDto.CodigoProveedor = txtCodigoProveedor.Text;
                                _compraDto.d_Anticipio = txtAnticipio.Text != string.Empty ? decimal.Parse(txtAnticipio.Text) : 0;
                                _compraDto.d_IGV = txtIGV.Text != string.Empty ? decimal.Parse(txtIGV.Text) : 0;
                                _compraDto.d_PorcentajeDetraccion = txtPorcDetraccion.Text != string.Empty ? decimal.Parse(txtPorcDetraccion.Text) : 0;
                                _compraDto.d_TipoCambio = txtTipoCambio.Text != string.Empty ? decimal.Parse(txtTipoCambio.Text) : 0;
                                _compraDto.d_Total = txtTotal.Text != string.Empty ? decimal.Parse(txtTotal.Text) : 0;
                                _compraDto.d_ValorVenta = txtValorVenta.Text != string.Empty ? decimal.Parse(txtValorVenta.Text) : 0;
                                _compraDto.i_DeduccionAnticipio = chkDeduccionAnticipio.Checked == true ? 1 : 0;
                                _compraDto.i_EsAfectoIgv = chkAfectoIGV.Checked == true ? 1 : 0;
                                _compraDto.i_EsDetraccion = rbDetraccionSI.Checked == true ? 1 : 0;
                                _compraDto.t_FechaDetraccion = dtpFchDetraccion.Value;
                                _compraDto.t_FechaRef = dtpFechaRef.Value;
                                _compraDto.t_FechaVencimiento = dtpFechaVencimiento.Value;
                                _compraDto.t_FechaEmision = dtpFechaEmision.Value;
                                _compraDto.t_FechaRegistro = dtpFechaRegistro.Value;
                                _compraDto.i_IdCondicionPago = int.Parse(cboCondicionPago.Value.ToString());
                                _compraDto.i_IdDestino = int.Parse(cboDestino.Value.ToString());
                                _compraDto.i_IdEstablecimiento = int.Parse(cboEstablecimiento.Value.ToString());
                                _compraDto.i_IdEstado = int.Parse(cboEstado.Value.ToString());
                                _compraDto.i_IdIgv = int.Parse(cboIGV.Value.ToString());
                                _compraDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                                _compraDto.i_IdTipoCompra = int.Parse(cboTipoCompra.Value.ToString());
                                _compraDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                                _compraDto.i_IdTipoDocumentoRef = int.Parse(cboDocumentoRef.Value.ToString());
                                _compraDto.i_PreciosIncluyenIgv = chkPrecInIGV.Checked == true ? 1 : 0;
                                _compraDto.i_CodigoDetraccion = txtCodigoDetraccion.Text != string.Empty ? int.Parse(txtCodigoDetraccion.Text) : 0;
                                _compraDto.v_Correlativo = txtCorrelativo.Text;
                                _compraDto.v_CorrelativoDocumento = txtCorrelativoDoc.Text;
                                _compraDto.v_CorrelativoDocumentoRef = txtCorrelativoDocRef.Text;
                                _compraDto.v_Glosa = txtGlosa.Text;
                                _compraDto.v_Mes = txtMes.Text;
                                _compraDto.v_NroDetraccion = txtNroDetraccion.Text;
                                _compraDto.v_Periodo = txtPeriodo.Text;
                                _compraDto.v_SerieDocumento = txtSerieDoc.Text;
                                _compraDto.v_SerieDocumentoRef = txtSerieDocRef.Text;
                                _compraDto.v_GuiaRemisionSerie = txtSerieGR.Text;
                                _compraDto.v_GuiaRemisionCorrelativo = txtCorrelativoGR.Text;
                                _compraDto.v_ODCCorrelativo = txtCorrelativoODC.Text.Trim();
                                _compraDto.v_ODCSerie = txtSerieODC.Text.Trim();
                                _compraDto.NombreProveedor = txtRazonSocial.Text;
                                _compraDto.d_total_isc = decimal.Parse(txtISC.Text);
                                _compraDto.d_total_otrostributos = decimal.Parse(txtOtrosAtributos.Text);
                                LlenarTemporalesCompra();
                                _objComprasBL.ActualizarCompra(ref objOperationResult, _compraDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto);

                                if (objOperationResult.Success == 1 && _objDocumentoBL.DocumentoEsInverso(_compraDto.i_IdTipoDocumento.Value))
                                    //    new CobranzaBL().RecalcularSaldoVenta(ref objOperationResult, _ventaDto.v_IdVenta, Globals.ClientSession.GetAsList(), false, true);
                                    new PagoBL().RecalcularSaldoCompra(ref objOperationResult, _compraDto.v_IdCompra, Globals.ClientSession.GetAsList(), false, true);
                                #endregion

                                if (objOperationResult.Success == 0)
                                {
                                    UltraMessageBox.Show(
                                        objOperationResult.ErrorMessage + "\n\n" + objOperationResult.ExceptionMessage +
                                        "\n\nTARGET: " + objOperationResult.AdditionalInformation, "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }

                                ts.Complete();
                            }
                        }
                    }
                    else
                    {
                        UltraMessageBox.Show(string.Format("El registro ya tiene registrado un pago en Tesorería [{0}], No se puede Editar", RegTesoreria), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                }
                #endregion

                if (objOperationResult.Success == 1)
                {
                    using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                    {
                        strModo = "Guardado";
                        EdicionBarraNavegacion(true);
                        ObtenerListadoCompras(txtPeriodo.Text, txtMes.Text);
                        _pstrIdMovimiento_Nuevo = _compraDto.v_IdCompra;
                        if (cboDocumento.Value != null && _objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())))
                        {
                            btnPagar.Enabled = false;
                        }
                        else
                        {
                            btnPagar.Enabled = true;
                        }
                    }
                    UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                _TempDetalle_AgregarDto = new List<compradetalleDto>();
                _TempDetalle_ModificarDto = new List<compradetalleDto>();
                _TempDetalle_EliminarDto = new List<compradetalleDto>();


            }
            else
            {
                if (_compraDto.i_IdEstado != 0)
                    UltraMessageBox.Show("Por favor llene los campos requeridos antes de guardar", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }

        private void txtRucProveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (!txtRucProveedor.IsDroppedDown && e.KeyCode == Keys.Enter)
            {
                if (txtRucProveedor.Text.Trim() != "" && txtRucProveedor.TextLength <= 5)
                {
                    Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(txtRucProveedor.Text, "RUC");
                    frm.ShowDialog();
                    if (frm._IdProveedor != null)
                    {
                        _compraDto.v_IdProveedor = frm._IdProveedor;
                        txtCodigoProveedor.Text = frm._CodigoProveedor;
                        txtRazonSocial.Text = frm._RazonSocial;
                        txtRucProveedor.Text = frm._NroDocumento;
                    }
                }
                else if (txtRucProveedor.TextLength == 11 | txtRucProveedor.TextLength == 8)
                {
                    if (cboDocumento.Value.ToString() == "1" && txtRucProveedor.TextLength == 8)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _compraDto.v_IdProveedor = null;
                        txtCodigoProveedor.Clear();
                        txtRazonSocial.Clear();
                        return;
                    }

                    if (txtRucProveedor.TextLength == 11 && Utils.Windows.ValidarRuc(txtRucProveedor.Text.Trim()) == false)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _compraDto.v_IdProveedor = null;
                        txtCodigoProveedor.Clear();
                        txtRazonSocial.Clear();
                        return;
                    }

                    OperationResult objOperationResult = new OperationResult();
                    string[] DatosProveedor = new string[3];
                    DatosProveedor = _objComprasBL.DevolverProveedorPorNroDocumento(ref objOperationResult, txtRucProveedor.Text.Trim());
                    if (DatosProveedor != null)
                    {
                        _compraDto.v_IdProveedor = DatosProveedor[0];
                        txtCodigoProveedor.Text = DatosProveedor[1];
                        txtRazonSocial.Text = DatosProveedor[2];
                    }
                    else
                    {
                        Mantenimientos.frmRegistroRapidoCliente frm = new Mantenimientos.frmRegistroRapidoCliente(txtRucProveedor.Text.Trim(), "V");
                        frm.ShowDialog();
                        if (frm._Guardado == true)
                        {
                            txtRucProveedor.Text = frm._NroDocumentoReturn;
                            DatosProveedor = _objComprasBL.DevolverProveedorPorNroDocumento(ref objOperationResult, txtRucProveedor.Text.Trim());
                            if (DatosProveedor != null)
                            {
                                _compraDto.v_IdProveedor = DatosProveedor[0];
                                txtCodigoProveedor.Text = DatosProveedor[1];
                                txtRazonSocial.Text = DatosProveedor[2];
                            }
                        }
                    }
                }
                else
                {
                    txtCodigoProveedor.Text = string.Empty;
                    txtRazonSocial.Text = string.Empty;
                }
            }
        }

        private void rbDetraccionSI_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDetraccionSI.Checked)
            {
                txtCodigoDetraccion.Enabled = true;
                dtpFchDetraccion.Enabled = true;
                txtPorcDetraccion.Enabled = true;
                dtpFchDetraccion.Enabled = true;
                txtNroDetraccion.Enabled = true;
                btnBuscarDetraccion.Enabled = true;
            }
        }

        private void rbDetraccionNO_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDetraccionNO.Checked)
            {
                _compraDto.i_CodigoDetraccion = 0;
                txtCodigoDetraccion.Text = String.Empty;
                txtPorcDetraccion.Text = String.Empty;
                txtNroDetraccion.Text = String.Empty;
                txtCodigoDetraccion.Enabled = false;
                dtpFchDetraccion.Enabled = false;
                txtPorcDetraccion.Enabled = false;
                dtpFchDetraccion.Enabled = false;
                txtNroDetraccion.Enabled = false;
                btnBuscarDetraccion.Enabled = false;
                txtNombreDetraccion.Clear();
                CalcularDetraccionesDetalle();
                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    Fila.Cells["i_RegistroEstado"].Value = "Modificado";
                    Fila.Cells["d_ValorSolesDetraccion"].Value = "0";
                    Fila.Cells["d_ValorDolaresDetraccion"].Value = "0";
                }
            }
        }

        private void btnBuscarDetraccion_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarDatahierarchy frm = new Mantenimientos.frmBuscarDatahierarchy(29, "Buscar Detracción");
            frm.ShowDialog();
            if (frm._itemId != null)
            {
                txtCodigoDetraccion.Text = frm._itemId;
                txtNombreDetraccion.Text = frm._value1;
                txtPorcDetraccion.Text = frm._value2;
                CalcularDetraccionesDetalle();
            }
        }

        private void chkDeduccionAnticipio_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDeduccionAnticipio.Checked == true)
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

        private void chkAfectoIGV_CheckedChanged(object sender, EventArgs e)
        {
            chkPrecInIGV.Checked = false;
            chkPrecInIGV.Enabled = chkAfectoIGV.Checked == true ? true : false;
            CalcularValoresDetalle(false);
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
            }
        }

        private void chkPrecInIGV_CheckedChanged(object sender, EventArgs e)
        {
            CalcularValoresDetalle(false);
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroEstado"].Value = "Modificado";
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
                bool filterRow = cboDocumento.DisplayLayout.Bands[0].Columns.Cast<UltraGridColumn>().Where(column => column.IsVisibleInLayout).All(column => !row.Cells[column].Text.Contains(cboDocumento.Text.ToUpper()));
                row.Hidden = filterRow;
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
                bool filterRow = cboDocumentoRef.DisplayLayout.Bands[0].Columns.Cast<UltraGridColumn>().Where(column => column.IsVisibleInLayout).All(column => !row.Cells[column].Text.Contains(cboDocumentoRef.Text.ToUpper()));
                row.Hidden = filterRow;
            }
        }

        private void cboDocumento_AfterDropDown(object sender, EventArgs e)
        {
            if (cboDocumento.Value == null) return;
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumento.Rows)
            {
                if (cboDocumento.Value == "-1") cboDocumento.Text = string.Empty;
                bool filterRow = cboDocumento.DisplayLayout.Bands[0].Columns.Cast<UltraGridColumn>().Where(column => column.IsVisibleInLayout).All(column => !row.Cells[column].Text.Contains(cboDocumento.Text.ToUpper()));
                row.Hidden = filterRow;
            }
        }

        private void cboDocumentoRef_AfterDropDown(object sender, EventArgs e)
        {
            foreach (Infragistics.Win.UltraWinGrid.UltraGridRow row in cboDocumentoRef.Rows)
            {
                if (cboDocumentoRef.Value == "-1") cboDocumentoRef.Text = string.Empty;
                bool filterRow = cboDocumentoRef.DisplayLayout.Bands[0].Columns.Cast<UltraGridColumn>().Where(column => column.IsVisibleInLayout).All(column => !row.Cells[column].Text.Contains(cboDocumentoRef.Text.ToUpper()));
                row.Hidden = filterRow;
            }
        }

        private void txtRazonSocial_TextChanged(object sender, EventArgs e)
        {
            if (cboDocumento.Value == null) return;

            ComprobarRelacionDocumentoProveedor();

            if (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) || cboDocumento.Value.ToString() == "8")
            {
                cboDocumentoRef.Enabled = true;
                txtSerieDocRef.Enabled = true;
                txtCorrelativoDocRef.Enabled = true;
                dtpFechaRef.Enabled = true;
                DevuelveCompraReferencia();
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
            }
        }

        private void txtPorcDetraccion_TextChanged(object sender, EventArgs e)
        {
            CalcularDetraccionesDetalle();
        }

        private void txtTipoCambio_TextChanged(object sender, EventArgs e)
        {
            CalcularDetraccionesDetalle();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _compradetalleDto = new compradetalleDto();
                    _compradetalleDto.v_IdCompraDetalle = grdData.ActiveRow.Cells["v_IdCompraDetalle"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(_compradetalleDto);

                    if (grdData.ActiveRow.Cells["v_IdMovimientoDetalle"].Value != null)
                    {
                        _movimientodetalleDto = new movimientodetalleDto
                        {
                            i_IdAlmacen =
                                grdData.ActiveRow.Cells["i_IdAlmacen"].Value != null
                                    ? grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()
                                    : Globals.ClientSession.i_IdAlmacenPredeterminado.Value.ToString(),
                            v_IdMovimientoDetalle = grdData.ActiveRow.Cells["v_IdMovimientoDetalle"].Value.ToString()
                        };
                        __TempDetalle_EliminarDto.Add(_movimientodetalleDto);
                    }


                    grdData.ActiveRow.Delete(false);
                }
            }
            else
            {
                grdData.ActiveRow.Delete(false);
            }
            CalcularValoresDetalle(false);
        }

        private void btnNuevoMovimiento_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            LimpiarCabecera();
            CargarDetalle("");
            txtCorrelativo.Text = (int.Parse(_ListadoCompras[_MaxV].Value1) + 1).ToString("00000000");
            _Mode = "New";
            _compraDto = new compraDto();
            EdicionBarraNavegacion(false);
            txtTipoCambio.Text = _objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
            HabilitarLotesSerie(true);
        }

        private void txtMes_Leave(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtMes, "{0:00}");
            if (txtMes.Text == "") return;
            var mes = int.Parse(txtMes.Text);
            if (mes >= 1 && mes <= 12)
            {
                switch (strModo)
                {
                    case "Nuevo":
                        ObtenerListadoCompras(txtPeriodo.Text, txtMes.Text);
                        break;
                    case "Guardado":
                        strModo = "Consulta";
                        ObtenerListadoCompras(txtPeriodo.Text, txtMes.Text);
                        break;
                    default:
                        ObtenerListadoCompras(txtPeriodo.Text, txtMes.Text);
                        break;
                }
            }
            else
            {
                UltraMessageBox.Show("Mes inválido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void dtpFechaEmision_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            string TipoCambio = _objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
            txtTipoCambio.Text = TipoCambio;
        }

        private void frmRegistroCompra_FormClosing(object sender, FormClosingEventArgs e)
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

        private void txtCorrelativo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtCorrelativo.Text = txtCorrelativo.Text == "" ? "" : int.Parse(txtCorrelativo.Text).ToString("00000000");
                if (txtCorrelativo.Text != "")
                {
                    var x = _ListadoCompras.Find(p => p.Value1 == txtCorrelativo.Text);
                    if (x != null)
                    {
                        CargarCabecera(x.Value2);
                    }
                    else
                    {
                        UltraMessageBox.Show("No se encontró la compra", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        ObtenerListadoCompras(txtPeriodo.Text, txtMes.Text);
                    }
                }
            }
        }

        private void txtCorrelativo_Leave(object sender, EventArgs e)
        {
            txtCorrelativo.Text = txtCorrelativo.Text == "" ? "" : int.Parse(txtCorrelativo.Text).ToString("00000000");
            if (txtCorrelativo.Text != "")
            {
                var x = _ListadoCompras.Find(p => p.Value1 == txtCorrelativo.Text);
                if (x != null)
                {
                    CargarCabecera(x.Value2);
                }
                else
                {
                    UltraMessageBox.Show("No se encontró la compra", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ObtenerListadoCompras(txtPeriodo.Text, txtMes.Text);
                }
            }
        }

        private void txtTipoCambio_Validated(object sender, EventArgs e)
        {
            if (txtTipoCambio.Text.Trim() == "")
            {
                txtTipoCambio.Text = "0";
            }
        }

        private void txtCorrelativoGR_Validated(object sender, EventArgs e)
        {

        }

        private void txtSerieGR_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieGR, e);
        }

        private void txtSerieGR_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerieGR, "{0:0000}");
        }

        private void txtCorrelativoGR_Validating(object sender, CancelEventArgs e)
        {
            txtCorrelativoGR.Text = DarFormatoGuiasRemision(txtCorrelativoGR.Text);
        }

        private static string DarFormatoGuiasRemision(string texto)
        {
            try
            {
                if (string.IsNullOrEmpty(texto)) return string.Empty;
                if (!texto.Contains(","))
                    return int.Parse(texto.Trim()).ToString("00000000");

                if (texto.StartsWith(",")) return string.Empty;
                var correlativos = texto.Split(',').Select(p => int.Parse(p.Trim()).ToString("00000000"));
                return string.Join(", ", correlativos);
            }
            catch
            {
                return string.Empty;
            }
        }

        private void txtCorrelativoGR_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroConComaUltraTextBox(txtCorrelativoGR, e);
        }

        private void txtCorrelativoDoc_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (txtCorrelativoDoc.CanUndo())
                {
                    txtCorrelativoDoc.Undo();
                }
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

        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(txtRucProveedor.Text.Trim(), "RUC");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {
                if (cboDocumento.Value != null && cboDocumento.Value.ToString() == "1" &&
                    frm._NroDocumento.Trim().Length == 8)
                    MessageBox.Show("Cliente no posee RUC Válido");
                else
                {
                    txtRazonSocial.Text = frm._RazonSocial;
                    txtCodigoProveedor.Text = frm._CodigoProveedor;
                    _compraDto.v_IdProveedor = frm._IdProveedor;
                    txtRucProveedor.Text = frm._NroDocumento;
                    ComprobarRelacionDocumentoProveedor();
                }
            }
        }
        #endregion

        #region Eventos Barra de Navegación
        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_ListadoCompras.Count() > 0)
            {
                if (_MaxV == 0) CargarCabecera(_ListadoCompras[0].Value2);

                if (_ActV > 0 && _ActV <= _MaxV)
                {
                    _ActV = _ActV - 1;
                    txtCorrelativo.Text = _ListadoCompras[_ActV].Value1;
                    CargarCabecera(_ListadoCompras[_ActV].Value2);
                }
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_ActV >= 0 && _ActV < _MaxV)
            {
                _ActV = _ActV + 1;
                txtCorrelativo.Text = _ListadoCompras[_ActV].Value1;
                CargarCabecera(_ListadoCompras[_ActV].Value2);
            }
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
                            ObtenerListadoCompras(txtPeriodo.Text, txtMes.Text);
                        }
                        else if (strModo == "Guardado")
                        {
                            strModo = "Consulta";
                            ObtenerListadoCompras(txtPeriodo.Text, txtMes.Text);
                        }
                        else
                        {
                            ObtenerListadoCompras(txtPeriodo.Text, txtMes.Text);
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
        #endregion

        #region Clases/Validaciones
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
            ucRubro.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucDestino, "Id", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 24, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucUnidadMedida, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 17, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucAlmacen, "Id", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaComboGrid(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucRubro, "Value1", "Id", _objComprasBL.ObtenRubrosParaComboGridCompra(ref objOperationResult, null), DropDownListAction.Select);
        }

        private void HabilitarLotesSerie(bool Habilitar)
        {
            grdData.DisplayLayout.Bands[0].Columns["v_NroSerie"].CellActivation = !Habilitar ? Activation.NoEdit : Activation.AllowEdit;
            grdData.DisplayLayout.Bands[0].Columns["v_NroSerie"].CellClickAction = !Habilitar ? CellClickAction.CellSelect : CellClickAction.EditAndSelectText;
            grdData.DisplayLayout.Bands[0].Columns["v_NroLote"].CellActivation = !Habilitar ? Activation.NoEdit : Activation.AllowEdit;
            grdData.DisplayLayout.Bands[0].Columns["v_NroLote"].CellClickAction = !Habilitar ? CellClickAction.CellSelect : CellClickAction.EditAndSelectText;
            grdData.DisplayLayout.Bands[0].Columns["t_FechaCaducidad"].CellActivation = !Habilitar ? Activation.NoEdit : Activation.AllowEdit;
            grdData.DisplayLayout.Bands[0].Columns["t_FechaCaducidad"].CellClickAction = !Habilitar ? CellClickAction.CellSelect : CellClickAction.EditAndSelectText;



        }

        private void ObtenerListadoCompras(string pstrPeriodo, string pstrMes)
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoCompras = _objComprasBL.ObtenerListadoCompras(ref objOperationResult, pstrPeriodo, pstrMes);
            switch (strModo)
            {
                case "Edicion":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdcompra);
                    txtCorrelativoDocRef.Enabled = false;
                    HabilitarLotesSerie(false);
                    break;

                case "Nuevo":
                    if (_ListadoCompras.Count != 0)
                    {
                        _MaxV = _ListadoCompras.Count() - 1;
                        _ActV = _MaxV;
                        LimpiarCabecera();
                        CargarDetalle("");
                        txtCorrelativo.Text = (int.Parse(_ListadoCompras[_MaxV].Value1) + 1).ToString("00000000");
                        _Mode = "New";
                        _compraDto = new compraDto();
                        _movimientoDto = new movimientoDto();
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
                        _compraDto = new compraDto();
                        _movimientoDto = new movimientoDto();
                        btnNuevoMovimiento.Enabled = false;
                        EdicionBarraNavegacion(true);
                    }
                    //txtMes.Enabled = true;
                    txtTipoCambio.Text = _objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
                    cboCondicionPago.Value = Globals.ClientSession.v_RucEmpresa ==Constants.RucAgrofergic ? "2"  : "1";
                    cboEstado.Value = "1";
                    btnPagar.Enabled = false;
                    break;

                case "Guardado":
                    _MaxV = _ListadoCompras.Count() - 1;
                    _ActV = _MaxV;
                    if (strIdcompra == "" | strIdcompra == null)
                    {
                        CargarCabecera(_ListadoCompras[_MaxV].Value2);
                    }
                    else
                    {
                        CargarCabecera(strIdcompra);
                    }
                    btnNuevoMovimiento.Enabled = true;
                    HabilitarLotesSerie(false);
                    break;

                case "Consulta":
                    if (_ListadoCompras.Count != 0)
                    {
                        _MaxV = _ListadoCompras.Count() - 1;
                        _ActV = _MaxV;
                        txtCorrelativo.Text = (int.Parse(_ListadoCompras[_MaxV].Value1)).ToString("00000000");
                        CargarCabecera(_ListadoCompras[_MaxV].Value2);
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
                        _compraDto = new compraDto();
                        _movimientoDto = new movimientoDto();
                        btnNuevoMovimiento.Enabled = false;
                        txtTipoCambio.Text = _objComprasBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaEmision.Value.Date);
                        EdicionBarraNavegacion(false);
                        txtMes.Enabled = true;
                    }
                    HabilitarLotesSerie(false);
                    break;
            }

            ultraDropDownButton1.Text = string.Format("Percepción: {0}", (_compraDto.i_AplicaRetencion ?? 0) == 1 ? "SI" : "NO");
        }

        private void EdicionBarraNavegacion(bool ON_OFF)
        {
            cboDocumento.Enabled = ON_OFF;
            txtSerieDoc.Enabled = ON_OFF;
            txtCorrelativoDoc.Enabled = ON_OFF;
            btnBuscarDetraccion.Enabled = ON_OFF;
            txtRucProveedor.Enabled = ON_OFF;
            cboDocumentoRef.Enabled = ON_OFF;
            txtSerieDocRef.Enabled = ON_OFF;
            btnBuscarProveedor.Enabled = ON_OFF;
        }

        private void CargarCabecera(string idmovimiento)
        {
            OperationResult objOperationResult = new OperationResult();
            _compraDto = new compraDto();
            _compraDto = _objComprasBL.ObtenerCompraCabecera(ref objOperationResult, idmovimiento);

            if (_compraDto != null)
            {
                cboMoneda.Value = _compraDto.i_IdMoneda.ToString();
                txtTipoCambio.Text = _compraDto.d_TipoCambio.ToString();
                txtGlosa.Text = _compraDto.v_Glosa;
                txtCorrelativo.Text = _compraDto.v_Correlativo;
                cboCondicionPago.Value = _compraDto.i_IdCondicionPago.ToString();
                cboDestino.Value = _compraDto.i_IdDestino.ToString();
                cboDocumento.Value = _compraDto.i_IdTipoDocumento.ToString();
                cboDocumentoRef.Value = _compraDto.i_IdTipoDocumentoRef.ToString();
                cboEstablecimiento.Value = _compraDto.i_IdEstablecimiento.ToString();
                cboEstado.Value = _compraDto.i_IdEstado.ToString();
                cboIGV.Value = _compraDto.i_IdIgv.ToString();
                cboMoneda.Value = _compraDto.i_IdMoneda.ToString();
                cboTipoCompra.Value = _compraDto.i_IdTipoCompra.ToString();
                txtAnticipio.Text = _compraDto.d_Anticipio.ToString();
                txtCodigoDetraccion.Text = _compraDto.i_CodigoDetraccion.ToString();
                txtCodigoProveedor.Text = _compraDto.CodigoProveedor != null ? _compraDto.CodigoProveedor.ToString() : string.Empty;
                txtCorrelativo.Text = _compraDto.v_Correlativo;
                txtCorrelativoDoc.Text = _compraDto.v_CorrelativoDocumento;
                txtCorrelativoDocRef.Text = _compraDto.v_CorrelativoDocumentoRef;
                txtGlosa.Text = _compraDto.v_Glosa;
                txtIGV.Text = _compraDto.d_IGV.ToString();
                txtMes.Text = int.Parse(_compraDto.v_Mes).ToString("00");
                txtNroDetraccion.Text = _compraDto.v_NroDetraccion;
                txtPeriodo.Text = _compraDto.v_Periodo;
                txtPorcDetraccion.Text = _compraDto.d_PorcentajeDetraccion.ToString();
                txtRazonSocial.Text = _compraDto.NombreProveedor != null ? _compraDto.NombreProveedor.ToString() : string.Empty;
                txtRucProveedor.Text = _compraDto.RUCProveedor != null ? _compraDto.RUCProveedor.ToString() : string.Empty;
                txtSerieDoc.Text = _compraDto.v_SerieDocumento;
                txtSerieDocRef.Text = _compraDto.v_SerieDocumentoRef;
                txtTotal.Text = _compraDto.d_Total.ToString();
                txtValorVenta.Text = _compraDto.d_ValorVenta.ToString();
                chkAfectoIGV.Checked = _compraDto.i_EsAfectoIgv == 1;
                chkDeduccionAnticipio.Checked = _compraDto.i_DeduccionAnticipio == 1;
                chkPrecInIGV.Checked = _compraDto.i_PreciosIncluyenIgv == 1;
                rbDetraccionSI.Checked = _compraDto.i_EsDetraccion == 1;
                rbDetraccionNO.Checked = _compraDto.i_EsDetraccion == 0;
                txtNombreDetraccion.Text = _compraDto.NombreDetraccion;
                cboTipoCompra.Value = int.Parse(_compraDto.i_IdTipoCompra.ToString()).ToString("00");
                dtpFchDetraccion.Value = _compraDto.t_FechaDetraccion.Value;
                dtpFechaRef.Value = _compraDto.t_FechaRef.Value;

                dtpFechaEmision.Value = _compraDto.t_FechaEmision.Value;
                dtpFechaRegistro.Value = _compraDto.t_FechaRegistro.Value;
                dtpFechaVencimiento.Value = _compraDto.t_FechaVencimiento.Value;
                txtTipoCambio.Text = _compraDto.d_TipoCambio.ToString();
                txtSerieGR.Text = _compraDto.v_GuiaRemisionSerie;
                txtCorrelativoGR.Text = _compraDto.v_GuiaRemisionCorrelativo;
                txtSerieODC.Text = _compraDto.v_ODCSerie;
                txtCorrelativoODC.Text = _compraDto.v_ODCCorrelativo;
                _EstadoActual = cboEstado.Value.ToString();

                //if (cboDocumento.Value.ToString() == "7")
                if (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())))
                {
                    UltraGridColumn c = grdData.DisplayLayout.Bands[0].Columns["i_IdAlmacen"];
                    c.CellActivation = Activation.ActivateOnly;
                    c.CellClickAction = CellClickAction.CellSelect;
                    btnPagar.Enabled = false;
                }
                else
                {
                    UltraGridColumn c = grdData.DisplayLayout.Bands[0].Columns["i_IdAlmacen"];
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    btnPagar.Enabled = true;
                }

                _Mode = "Edit";
                txtSerieGR.Enabled = _compraDto.v_GuiaRemisionSerie == null;
                txtCorrelativoGR.Enabled = _compraDto.v_GuiaRemisionCorrelativo == null;

                txtSerieODC.Enabled = _compraDto.v_ODCSerie == null;
                txtCorrelativoODC.Enabled = _compraDto.v_ODCCorrelativo == null;

                CargarDetalle(_compraDto.v_IdCompra);
                // btnPagar.Enabled = true;

            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar la compra", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void LimpiarCabecera()
        {
            txtGlosa.Clear();
            cboCondicionPago.Value = "-1";
            cboDocumento.Value = "-1";
            cboDocumentoRef.Value = "-1";
            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.ToString();
            cboEstado.Value = "-1";
            cboTipoCompra.Value = "-1";
            txtAnticipio.Clear();
            txtCodigoDetraccion.Clear();
            txtCodigoProveedor.Clear();
            txtCorrelativoDoc.Clear();
            txtCorrelativoDocRef.Clear();
            txtIGV.Clear();
            txtNroDetraccion.Clear();
            txtPorcDetraccion.Clear();
            txtRazonSocial.Clear();
            txtRucProveedor.Clear();
            txtSerieDoc.Clear();
            txtSerieDocRef.Clear();
            txtTipoCambio.Clear();
            txtTotal.Clear();
            txtValorVenta.Clear();
            chkDeduccionAnticipio.Checked = false;
            chkDuplicar.Checked = false;

            cboMoneda.Value = Globals.ClientSession.i_IdMonedaCompra.ToString();
            cboIGV.Value = Globals.ClientSession.i_IdIgv.ToString();
            chkAfectoIGV.Checked = Globals.ClientSession.i_AfectoIgvCompras == 1;
            chkPrecInIGV.Checked = Globals.ClientSession.i_PrecioIncluyeIgvCompras == 1;
            cboDestino.Value = Globals.ClientSession.i_IdDestinoCompras.ToString();
            dtpFechaRegistro.Value = DateTime.Parse(DateTime.Today.Day + "/" + DateTime.Today.Month + "/" + Globals.ClientSession.i_Periodo);
            dtpFechaEmision.Value = DateTime.Parse(DateTime.Today.Day + "/" + DateTime.Today.Month + "/" + Globals.ClientSession.i_Periodo);
        }

        private void CargarDetalle(string pstringIdCompra)
        {
            OperationResult objOperationResult = new OperationResult();
            grdData.DataSource = _objComprasBL.ObtenerCompraDetalles(ref objOperationResult, pstringIdCompra);
            DevolverNombreRelaciones();
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
                var cta = Utils.Windows.DevuelveCuentaDatos(grdData.Rows[i].Cells["v_NroCuenta"].Value.ToString());
                if (cta != null)
                    grdData.Rows[i].Cells["i_IdCentroCostos"].Activation = cta.i_CentroCosto == 1
                                ? Activation.AllowEdit
                                : Activation.Disabled;
            }
            CalcularTotales();
        }

        private void DevolverNombreRelaciones()
        {
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                if (grdData.Rows[i].Cells["v_IdProductoDetalle"].Value != null)
                {
                    string[] Cadena = new string[9];
                    Cadena = _objComprasBL.DevolverNombres(grdData.Rows[i].Cells["v_IdProductoDetalle"].Value.ToString(), grdData.Rows[i].Cells["v_NroCuenta"].Value.ToString(), int.Parse(grdData.Rows[i].Cells["i_IdDestino"].Value.ToString()), grdData.Rows[i].Cells["i_IdCentroCostos"].Value != null ? grdData.Rows[i].Cells["i_IdCentroCostos"].Value.ToString() : string.Empty, int.Parse(grdData.Rows[i].Cells["i_IdAlmacen"].Value.ToString()));
                    grdData.Rows[i].Cells["v_CodigoInterno"].Value = Cadena[0];
                    grdData.Rows[i].Cells["v_NombreProducto"].Value = Cadena[1];
                    grdData.Rows[i].Cells["Empaque"].Value = Cadena[2];
                    grdData.Rows[i].Cells["UMEmpaque"].Value = Cadena[3];
                    grdData.Rows[i].Cells["_NombreCuenta"].Value = Cadena[4];
                    grdData.Rows[i].Cells["_Destino"].Value = Cadena[5];
                    grdData.Rows[i].Cells["_CentroCosto"].Value = Cadena[6];
                    grdData.Rows[i].Cells["_Almacen"].Value = Cadena[7];
                    grdData.Rows[i].Cells["i_EsServicio"].Value = Cadena[8];
                    grdData.Rows[i].Cells["i_IdUnidadMedidaProducto"].Value = Cadena[9];
                    grdData.Rows[i].Cells["i_SolicitarNroLoteIngreso"].Value = Cadena[11];
                    grdData.Rows[i].Cells["i_SolicitarNroSerieIngreso"].Value = Cadena[12];

                }
            }
        }

        private void LlenarTemporalesCompra()
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
                                _compradetalleDto = new compradetalleDto();
                                _compradetalleDto.v_IdCompra = _compraDto.v_IdCompra;
                                _compradetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _compradetalleDto.v_NroGuiaRemision = Fila.Cells["v_NroGuiaRemision"].Value == null ? null : Fila.Cells["v_NroGuiaRemision"].Value.ToString();
                                _compradetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _compradetalleDto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _compradetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                _compradetalleDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                                _compradetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString();
                                _compradetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _compradetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _compradetalleDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());
                                _compradetalleDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                                _compradetalleDto.d_ValorDolaresDetraccion = Fila.Cells["d_ValorDolaresDetraccion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorDolaresDetraccion"].Value.ToString());
                                _compradetalleDto.d_ValorSolesDetraccion = Fila.Cells["d_ValorSolesDetraccion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorSolesDetraccion"].Value.ToString());
                                _compradetalleDto.i_Anticipio = Fila.Cells["i_Anticipio"].Value == null ? 0 : int.Parse(Fila.Cells["i_Anticipio"].Value.ToString());
                                _compradetalleDto.i_IdCentroCostos = Fila.Cells["i_IdCentroCostos"].Value == null ? null : Fila.Cells["i_IdCentroCostos"].Value.ToString();
                                _compradetalleDto.i_IdDestino = Fila.Cells["i_IdDestino"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdDestino"].Value.ToString());
                                _compradetalleDto.i_InsertaIdUsuario = Fila.Cells["i_InsertaIdUsuario"].Value == null ? 0 : int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _compradetalleDto.v_Glosa = Fila.Cells["v_Glosa"].Value == null ? null : Fila.Cells["v_Glosa"].Value.ToString();
                                _compradetalleDto.v_NroCuenta = Fila.Cells["v_NroCuenta"].Value == null ? null : Fila.Cells["v_NroCuenta"].Value.ToString();
                                _compradetalleDto._DesdeODC = Fila.Cells["_DesdeODC"].Value != null && (Fila.Cells["_DesdeODC"].Value.ToString() == "1" ? true : false);
                                _compradetalleDto.d_isc = Fila.Cells["d_isc"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_isc"].Value.ToString());
                                _compradetalleDto.d_otrostributos = Fila.Cells["d_otrostributos"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString());
                                _compradetalleDto.d_DescuentoItem = Fila.Cells["d_DescuentoItem"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_DescuentoItem"].Value.ToString());
                                _compradetalleDto.v_DescuentoItem = Fila.Cells["v_DescuentoItem"].Value == null ? null : Fila.Cells["v_DescuentoItem"].Value.ToString();
                                _compradetalleDto.v_NroSerie = Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim();
                                _compradetalleDto.v_NroLote = Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim();


                                _compradetalleDto.t_FechaCaducidad = Fila.Cells["t_FechaCaducidad"].Value == null ? (DateTime?)null : DateTime.Parse(Fila.Cells["t_FechaCaducidad"].Value.ToString());
                                _compradetalleDto.t_FechaFabricacion = Fila.Cells["t_FechaFabricacion"].Value == null ? (DateTime?)null : DateTime.Parse(Fila.Cells["t_FechaFabricacion"].Value.ToString());



                                _TempDetalle_AgregarDto.Add(_compradetalleDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _compradetalleDto = new compradetalleDto();
                                _compradetalleDto.v_IdCompraDetalle = Fila.Cells["v_IdCompraDetalle"].Value == null ? null : Fila.Cells["v_IdCompraDetalle"].Value.ToString();
                                _compradetalleDto.v_IdMovimientoDetalle = Fila.Cells["v_IdMovimientoDetalle"].Value == null ? null : Fila.Cells["v_IdMovimientoDetalle"].Value.ToString();
                                _compradetalleDto.v_IdCompra = Fila.Cells["v_IdCompra"].Value == null ? null : Fila.Cells["v_IdCompra"].Value.ToString();
                                _compradetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _compradetalleDto.v_NroGuiaRemision = Fila.Cells["v_NroGuiaRemision"].Value == null ? null : Fila.Cells["v_NroGuiaRemision"].Value.ToString();
                                _compradetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _compradetalleDto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _compradetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                _compradetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _compradetalleDto.d_ValorVenta = Fila.Cells["d_ValorVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                                _compradetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString();
                                _compradetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _compradetalleDto.d_Igv = Fila.Cells["d_Igv"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());
                                _compradetalleDto.d_PrecioVenta = Fila.Cells["d_PrecioVenta"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                                _compradetalleDto.d_ValorDolaresDetraccion = Fila.Cells["d_ValorDolaresDetraccion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorDolaresDetraccion"].Value.ToString());
                                _compradetalleDto.d_ValorSolesDetraccion = Fila.Cells["d_ValorSolesDetraccion"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_ValorSolesDetraccion"].Value.ToString());
                                _compradetalleDto.i_Anticipio = Fila.Cells["i_Anticipio"].Value == null ? 0 : int.Parse(Fila.Cells["i_Anticipio"].Value.ToString());
                                _compradetalleDto.i_IdCentroCostos = Fila.Cells["i_IdCentroCostos"].Value == null ? null : Fila.Cells["i_IdCentroCostos"].Value.ToString();
                                _compradetalleDto.i_IdDestino = Fila.Cells["i_IdDestino"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdDestino"].Value.ToString());
                                _compradetalleDto.i_InsertaIdUsuario = Fila.Cells["i_InsertaIdUsuario"].Value == null ? 0 : int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _compradetalleDto.v_Glosa = Fila.Cells["v_Glosa"].Value == null ? null : Fila.Cells["v_Glosa"].Value.ToString();
                                _compradetalleDto.v_NroCuenta = Fila.Cells["v_NroCuenta"].Value == null ? null : Fila.Cells["v_NroCuenta"].Value.ToString();
                                _compradetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _compradetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _compradetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _compradetalleDto.d_isc = Fila.Cells["d_isc"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_isc"].Value.ToString());
                                _compradetalleDto.d_otrostributos = Fila.Cells["d_otrostributos"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString());
                                _compradetalleDto._DesdeODC = Fila.Cells["_DesdeODC"].Value != null && (Fila.Cells["_DesdeODC"].Value.ToString() == "1");
                                _compradetalleDto.d_DescuentoItem = Fila.Cells["d_DescuentoItem"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_DescuentoItem"].Value.ToString());
                                _compradetalleDto.v_DescuentoItem = Fila.Cells["v_DescuentoItem"].Value == null ? null : Fila.Cells["v_DescuentoItem"].Value.ToString();
                                _compradetalleDto.v_NroSerie = Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim();
                                _compradetalleDto.v_NroLote = Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim();



                                _compradetalleDto.t_FechaCaducidad = Fila.Cells["t_FechaCaducidad"].Value == null ? (DateTime?)null : DateTime.Parse(Fila.Cells["t_FechaCaducidad"].Value.ToString());
                                _compradetalleDto.t_FechaFabricacion = Fila.Cells["t_FechaFabricacion"].Value == null ? (DateTime?)null : DateTime.Parse(Fila.Cells["t_FechaFabricacion"].Value.ToString());

                                _TempDetalle_ModificarDto.Add(_compradetalleDto);
                            }
                            break;
                    }
                }
            }

        }

        private void CalcularValoresDetalle(bool requiereFoco = true)
        {
            if (!grdData.Rows.Any()) return;
            foreach (UltraGridRow Fila in grdData.Rows)
            {
                CalcularValoresFila(Fila, requiereFoco);
            }
            CalcularTotales();
        }

        private void CalcularValoresFila(UltraGridRow Fila, bool requiereFoco = true)
        {
            try
            {
                if (cboIGV.Value.ToString() != "-1")
                {
                    if (requiereFoco && (!Fila.Cells["d_Precio"].IsActiveCell && !Fila.Cells["i_IdDestino"].IsActiveCell)) return;

                    if (Fila.Cells["d_Cantidad"].Value == null) { Fila.Cells["d_Cantidad"].Value = "0"; }
                    if (Fila.Cells["d_Precio"].Value == null) { Fila.Cells["d_Precio"].Value = "0"; }
                    if (Fila.Cells["d_ValorVenta"].Value == null) { Fila.Cells["d_ValorVenta"].Value = "0"; }
                    if (Fila.Cells["d_PrecioVenta"].Value == null) { Fila.Cells["d_PrecioVenta"].Value = "0"; }
                    if (Fila.Cells["d_isc"].Value == null) { Fila.Cells["d_isc"].Value = "0"; }
                    if (Fila.Cells["d_otrostributos"].Value == null) { Fila.Cells["d_otrostributos"].Value = "0"; }
                    if (Fila.Cells["d_DescuentoItem"].Value == null) { Fila.Cells["d_DescuentoItem"].Value = "0"; }
                    if (Fila.Cells["v_DescuentoItem"].Value == null) { Fila.Cells["v_DescuentoItem"].Value = "0"; }
                    var cnt = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                    var costo = decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                    var descuento = Utils.Windows.CalcularDescuentosSucesivos(Fila.Cells["v_DescuentoItem"].Value.ToString(), Utils.Windows.DevuelveValorRedondeado(cnt * costo, 4)); ;
                    Fila.Cells["d_DescuentoItem"].Value = descuento;
                    if (Fila.Cells["i_Anticipio"].Value == null) return;

                    if (Fila.Cells["v_NroCuenta"].Value != null && Fila.Cells["v_NroCuenta"].Value.ToString() != "4011301")
                    {
                        if (chkAfectoIGV.Checked == false)
                        {
                            if (Fila.Cells["i_Anticipio"].Value.ToString() == "1")
                            {
                                Fila.Cells["d_Cantidad"].Value = "0";
                                Fila.Cells["d_Precio"].Value = "0";
                                if (Fila.Cells["i_IdDestino"].Value.ToString() == "4")
                                    Fila.Cells["d_Igv"].Value = "0";
                            }
                            else
                            {
                                if (decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0 && decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) != 0)
                                {
                                    Fila.Cells["d_ValorVenta"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) - descuento), 2).ToString("0.000");
                                    Fila.Cells["d_PrecioVenta"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString()), 2);
                                    Fila.Cells["d_PrecioVenta"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString()) + decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString()) + decimal.Parse(Fila.Cells["d_isc"].Value.ToString()), 2);
                                    Fila.Cells["d_Igv"].Value = "0";
                                }
                            }
                        }
                        else
                        {
                            if (chkPrecInIGV.Checked == false)
                            {
                                if (Fila.Cells["i_Anticipio"].Value.ToString() == "1")
                                {
                                    Fila.Cells["d_Cantidad"].Value = "0";
                                    Fila.Cells["d_Precio"].Value = "0";
                                    if (Fila.Cells["i_IdDestino"].Value.ToString() == "4")
                                        Fila.Cells["d_Igv"].Value = "0";
                                }
                                else
                                {
                                    if (decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0 && decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) != 0)
                                    {
                                        if (Fila.Cells["i_IdDestino"].Value.ToString() != "4")
                                        {
                                            Fila.Cells["d_ValorVenta"].Value = Utils.Windows.DevuelveValorRedondeado((decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) - descuento), 2).ToString("0.000");
                                            Fila.Cells["d_Igv"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString()) * (decimal.Parse(cboIGV.Text) / 100) + decimal.Parse(Fila.Cells["d_isc"].Value.ToString()), 2);
                                            Fila.Cells["d_PrecioVenta"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString()) + decimal.Parse(Fila.Cells["d_Igv"].Value.ToString()) + decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString()), 2);
                                        }
                                        else
                                        {
                                            Fila.Cells["d_ValorVenta"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) - descuento + decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString()) + decimal.Parse(Fila.Cells["d_isc"].Value.ToString()), 2);
                                            Fila.Cells["d_PrecioVenta"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString()), 2);
                                            Fila.Cells["d_Igv"].Value = "0";
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Fila.Cells["i_Anticipio"].Value.ToString() == "1")
                                {
                                    Fila.Cells["d_Cantidad"].Value = "0";
                                    Fila.Cells["d_Precio"].Value = "0";
                                    if (Fila.Cells["i_IdDestino"].Value.ToString() == "4")
                                        Fila.Cells["d_Igv"].Value = "0";
                                }
                                else
                                {
                                    if (decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0 && decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) != 0)
                                    {
                                        if (Fila.Cells["i_IdDestino"].Value.ToString() != "4")
                                        {
                                            Fila.Cells["d_PrecioVenta"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) - descuento + decimal.Parse(Fila.Cells["d_isc"].Value.ToString()), 2);
                                            Fila.Cells["d_ValorVenta"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString()) / (1 + (decimal.Parse(cboIGV.Text) / 100)), 2);
                                            Fila.Cells["d_Igv"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString()) - (decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString())), 2);
                                            Fila.Cells["d_PrecioVenta"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString()) + decimal.Parse(Fila.Cells["d_isc"].Value.ToString()) + decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString()), 2);
                                        }
                                        else
                                        {
                                            Fila.Cells["d_ValorVenta"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) * decimal.Parse(Fila.Cells["d_Precio"].Value.ToString()) - descuento, 2);
                                            Fila.Cells["d_PrecioVenta"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString()) + decimal.Parse(Fila.Cells["d_isc"].Value.ToString()) + decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString()), 2);
                                            Fila.Cells["d_Igv"].Value = "0";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Fila.Cells["d_ValorVenta"].Value = Fila.Cells["d_PrecioVenta"].Value.ToString();
                    }
                    CalcularDetraccionesDetalle();
                    CalcularTotales();
                }
                else
                {
                    UltraMessageBox.Show("Porfavor seleccione el IGV", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message + "\nLinea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }
        }

        private void CalcularTotales()
        {
            if (grdData.Rows.Any())
            {
                decimal SumAntValVenta = 0, SumAntIgv = 0, SumAntTotal = 0, SumAntISC = 0, SumAntOT = 0;
                decimal SumValVenta = 0, SumIgv = 0, SumTotal = 0, SumISC = 0, SumOT = 0;

                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["d_ValorVenta"].Value == null) { Fila.Cells["d_ValorVenta"].Value = "0"; }
                    if (Fila.Cells["d_Igv"].Value == null) { Fila.Cells["d_Igv"].Value = "0"; }
                    if (Fila.Cells["d_PrecioVenta"].Value == null) { Fila.Cells["d_PrecioVenta"].Value = "0"; }
                    Fila.Cells["i_Anticipio"].Value = Fila.Cells["i_Anticipio"].Value == null ? "0" : Fila.Cells["i_Anticipio"].Value;
                    if (Fila.Cells["d_isc"].Value == null) { Fila.Cells["d_isc"].Value = "0"; }
                    if (Fila.Cells["d_otrostributos"].Value == null) { Fila.Cells["d_otrostributos"].Value = "0"; }

                    switch (Fila.Cells["i_Anticipio"].Value.ToString())
                    {
                        case "1":
                            SumAntValVenta = SumAntValVenta + decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                            SumAntIgv = SumAntIgv + decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());
                            SumAntTotal = SumAntTotal + decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                            SumAntISC = SumAntISC + decimal.Parse(Fila.Cells["d_isc"].Value.ToString());
                            SumAntOT = SumAntOT + decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString());
                            break;
                        case "0":
                            SumValVenta = SumValVenta + decimal.Parse(Fila.Cells["d_ValorVenta"].Value.ToString());
                            SumIgv = SumIgv + decimal.Parse(Fila.Cells["d_Igv"].Value.ToString());
                            SumTotal = SumTotal + decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                            SumISC = SumISC + decimal.Parse(Fila.Cells["d_isc"].Value.ToString());
                            SumOT = SumOT + decimal.Parse(Fila.Cells["d_otrostributos"].Value.ToString());
                            break;
                    }

                    if (Fila.Cells["i_IdUnidadMedida"].Value != null)
                    {
                        if (Fila.Cells["Empaque"].Value != null && Fila.Cells["v_IdProductoDetalle"].Value != null && Fila.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000" && Fila.Cells["i_IdUnidadMedida"].Value.ToString() != "-1" && decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0)
                        {
                            decimal TotalEmpaque = 0;
                            decimal Empaque = decimal.Parse(Fila.Cells["Empaque"].Value.ToString());
                            string Producto = Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                            decimal Cantidad = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                            int UM = int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                            int UMProducto = int.Parse(Fila.Cells["i_IdUnidadMedidaProducto"].Value.ToString());

                            GridKeyValueDTO _UMProducto = ((List<GridKeyValueDTO>)ucUnidadMedida.DataSource).FirstOrDefault(p => p.Id == UMProducto.ToString());
                            GridKeyValueDTO _UM = ((List<GridKeyValueDTO>)ucUnidadMedida.DataSource).FirstOrDefault(p => p.Id == UM.ToString());

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
                            Fila.Cells["d_CantidadEmpaque"].Value = TotalEmpaque.ToString(CultureInfo.InvariantCulture);
                        }
                        else
                            Fila.Cells["d_CantidadEmpaque"].Value = "0";
                    }
                }
                txtAnticipio.Text = SumAntValVenta.ToString("0.00");
                txtValorVenta.Text = (SumValVenta - SumAntValVenta).ToString("0.00");
                txtIGV.Text = (SumIgv - SumAntIgv).ToString("0.00");
                txtTotal.Text = (SumTotal - SumAntTotal).ToString("0.00");
                txtISC.Text = (SumISC - SumAntISC).ToString("0.00");
                txtOtrosAtributos.Text = (SumOT - SumAntOT).ToString("0.00");
            }
        }

        private void CalcularDetraccionesDetalle()
        {
            try
            {
                if (rbDetraccionSI.Checked)
                {
                    if (grdData.Rows.Any())
                    {
                        decimal PrecioVenta, PorcentajeDetraccion, DetraccionDolares, DetraccionSoles, TipoCambio;
                        foreach (UltraGridRow Fila in grdData.Rows)
                        {
                            if (Fila.Cells["d_Cantidad"].Value == null) { Fila.Cells["d_Cantidad"].Value = "0"; }
                            if (Fila.Cells["d_Precio"].Value == null) { Fila.Cells["d_Precio"].Value = "0"; }
                            if (Fila.Cells["d_ValorVenta"].Value == null) { Fila.Cells["d_ValorVenta"].Value = "0"; }
                            if (Fila.Cells["d_PrecioVenta"].Value == null) { Fila.Cells["d_PrecioVenta"].Value = "0"; }

                            PrecioVenta = decimal.Parse(Fila.Cells["d_PrecioVenta"].Value.ToString());
                            PorcentajeDetraccion = txtPorcDetraccion.Text.Trim() != string.Empty && txtPorcDetraccion.Text.Trim() != "." ? decimal.Parse(txtPorcDetraccion.Text.Trim()) : 0;
                            TipoCambio = txtTipoCambio.Text.Trim() != "" && txtTipoCambio.Text.Trim() != "." ? decimal.Parse(txtTipoCambio.Text.Trim()) : 0;

                            if (cboMoneda.Value.ToString() == "1")
                            {
                                DetraccionSoles = PrecioVenta * (PorcentajeDetraccion / 100);
                                DetraccionDolares = TipoCambio != 0 ? DetraccionSoles / TipoCambio : 0;
                            }
                            else
                            {
                                DetraccionDolares = PrecioVenta * (PorcentajeDetraccion / 100);
                                DetraccionSoles = TipoCambio != 0 ? DetraccionDolares * TipoCambio : 0;
                            }

                            Fila.Cells["d_ValorSolesDetraccion"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(DetraccionSoles.ToString()), 0);
                            Fila.Cells["i_RegistroEstado"].Value = "Modificado";
                            Fila.Cells["d_ValorDolaresDetraccion"].Value = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(DetraccionDolares.ToString()), 2);
                            Fila.Cells["i_RegistroEstado"].Value = "Modificado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message + "Linea: " + ex.StackTrace.Substring(ex.StackTrace.LastIndexOf(' ')));
            }

        }

        private void ComprobarRelacionDocumentoProveedor()
        {
            var objOperationResult = new OperationResult();
            if (_objComprasBL.ComprobarRelacionDocumentoProveedor(ref objOperationResult, _compraDto.v_IdProveedor, int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text, txtCorrelativoDoc.Text, _compraDto.v_IdCompra) == true)
            {
                UltraMessageBox.Show("El documento ya ha sido registrado anteriormente con este Proveedor", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGuardar.Enabled = false;
            }
            else
            {
                btnGuardar.Enabled = true;
            }
        }

        private void DevuelveCompraReferencia()
        {
            if (txtSerieDocRef.Text.Trim() != "" && txtCorrelativoDocRef.Text.Trim() != "" && cboDocumentoRef.Value.ToString() != "-1" && _compraDto.v_IdProveedor != null)
            {
                compraDto compraRefDto = new compraDto();
                OperationResult objOperationResult = new OperationResult();
                string IdCompraRef = _objComprasBL.DevolverIdCompra(ref objOperationResult, _compraDto.v_IdProveedor, int.Parse(cboDocumentoRef.Value.ToString()), txtSerieDocRef.Text, txtCorrelativoDocRef.Text);
                compraRefDto = _objComprasBL.ObtenerCompraCabecera(ref objOperationResult, IdCompraRef);
                if (compraRefDto != null)
                {
                    btnGuardar.Enabled = true;
                    dtpFechaRef.Value = compraRefDto.t_FechaEmision?? DateTime.Now;
                    cboCondicionPago.Value = compraRefDto.i_IdCondicionPago.ToString();
                    chkAfectoIGV.Checked = compraRefDto.i_EsAfectoIgv == 1 ? true : false;
                    chkPrecInIGV.Checked = compraRefDto.i_PreciosIncluyenIgv == 1 ? true : false;
                    cboMoneda.Value = compraRefDto.i_IdMoneda.ToString();
                    cboTipoCompra.Value = int.Parse(compraRefDto.i_IdTipoCompra.ToString()).ToString("00");
                    cboDestino.Value = compraRefDto.i_IdDestino.ToString();
                    cboEstablecimiento.Value = compraRefDto.i_IdEstablecimiento.ToString();
                    
                    CargarDetalle(IdCompraRef);
                    if (grdData.Rows.Count() > 0)
                    {
                        foreach (UltraGridRow Fila in grdData.Rows)
                        {
                            //cambio los estados de las filas devueltas como temporales y modificadas para que al llenarse los temporales se inserten en el presente doc.
                            Fila.Cells["i_RegistroTipo"].Value = "Temporal";
                            Fila.Cells["i_RegistroEstado"].Value = "Modificado";
                        }
                    }

                    txtTipoCambio.Text = compraRefDto.d_TipoCambio.ToString();
                }
                else
                {
                    UltraMessageBox.Show("No se ha registrado el documento de referencia", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnGuardar.Enabled = false;
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
                const string sharp = "n";
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
            _Precio.MaskInput = FormatoPrecio;
        }

        public void RecibirItems(List<UltraGridRow> Filas)
        {
            bool Repetido = false;
            bool j = false;
            for (int i = 0; i < Filas.Count; i++)
            {
                if (grdData.Rows.Count(p => p.Cells["v_IdProductoDetalle"].Value != null && p.Cells["v_IdProductoDetalle"].Value.ToString() == Filas[i].Cells["v_IdProductoDetalle"].Value.ToString()) != 0)
                {
                    UltraMessageBox.Show("El producto '" + Filas[i].Cells["v_Descripcion"].Value.ToString() + "' ya se encuentra en el detalle", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Repetido = true;
                }
                else
                {
                    Repetido = false;
                }

                if (Repetido == false)
                {
                    if (j == false)
                    {
                        grdData.ActiveRow.Cells["v_NombreProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                        grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                        grdData.ActiveRow.Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.ActiveRow.Cells["i_IdCentroCostos"].Value = "0";

                        grdData.ActiveRow.Cells["i_IdDestino"].Value = cboDestino.Value.ToString();
                        grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.ActiveRow.Cells["d_Precio"].Value = Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString());
                        grdData.ActiveRow.Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                        grdData.ActiveRow.Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                        grdData.ActiveRow.Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                        grdData.ActiveRow.Cells["v_NroCuenta"].Value = Filas[i].Cells["NroCuentaCompra"].Value != null ? Filas[i].Cells["NroCuentaCompra"].Value.ToString() : "-1";



                        grdData.ActiveRow.Cells["i_SolicitarNroSerieIngreso"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroSerieIngreso"].Value.ToString());
                        grdData.ActiveRow.Cells["i_SolicitarNroLoteIngreso"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroLoteIngreso"].Value.ToString());

                     
                        grdData.ActiveRow.Cells["t_FechaCaducidad"].Value = DateTime.Parse(Constants.FechaNula);
                        grdData.ActiveRow.Cells["t_FechaFabricacion"].Value = DateTime.Parse(Constants.FechaNula);
                        if (Utils.Windows.DevuelveCuentaDatos(grdData.ActiveRow.Cells["v_NroCuenta"].Value.ToString()).i_CentroCosto == 1)
                            grdData.ActiveRow.Cells["i_IdCentroCostos"].Activation = Activation.AllowEdit;
                        else
                            grdData.ActiveRow.Cells["i_IdCentroCostos"].Activation = Activation.Disabled;
                        grdData.ActiveRow.Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                        j = true;
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
                        row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                        row.Activate();
                        grdData.ActiveRow.Cells["v_NombreProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                        grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                        grdData.ActiveRow.Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.ActiveRow.Cells["i_IdCentroCostos"].Value = "0";
                        grdData.ActiveRow.Cells["i_IdDestino"].Value = cboDestino.Value.ToString();
                        grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.ActiveRow.Cells["d_Precio"].Value = Filas[i].Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Precio"].Value.ToString());
                        grdData.ActiveRow.Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                        grdData.ActiveRow.Cells["Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                        grdData.ActiveRow.Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                        grdData.ActiveRow.Cells["v_NroCuenta"].Value = Filas[i].Cells["NroCuentaCompra"].Value != null ? Filas[i].Cells["NroCuentaCompra"].Value.ToString() : "-1";

                        grdData.ActiveRow.Cells["i_SolicitarNroSerieIngreso"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroSerieIngreso"].Value.ToString());
                        grdData.ActiveRow.Cells["i_SolicitarNroLoteIngreso"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroLoteIngreso"].Value.ToString());
                        grdData.ActiveRow.Cells["t_FechaCaducidad"].Value = DateTime.Parse(Constants.FechaNula);
                        grdData.ActiveRow.Cells["t_FechaFabricacion"].Value = DateTime.Parse(Constants.FechaNula);
                        if (Utils.Windows.DevuelveCuentaDatos(grdData.ActiveRow.Cells["v_NroCuenta"].Value.ToString()).i_CentroCosto == 1)
                            grdData.ActiveRow.Cells["i_IdCentroCostos"].Activation = Activation.AllowEdit;
                        else
                            grdData.ActiveRow.Cells["i_IdCentroCostos"].Activation = Activation.Disabled;
                        grdData.ActiveRow.Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                    }
                }
            }
            CalcularValoresDetalle(false);
        }

        private void RestringirXGuiaRemision(bool ON_OFF)
        {
            if (ON_OFF == true)
            {
                txtRucProveedor.Enabled = false;
                txtSerieGR.Enabled = false;
                txtCorrelativoGR.Enabled = false;
                GuiaRemisionImportada = true;
            }

        }

        private bool ValidaCamposNulosVacios()
        {
            if (grdData.Rows.Count(p => p.Cells["v_NroCuenta"].Value == null || p.Cells["v_NroCuenta"].Value.ToString().Trim() == "-1") != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente todas las cuentas al detalle.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow row = grdData.Rows.FirstOrDefault(x => x.Cells["v_NroCuenta"].Value == null || x.Cells["v_NroCuenta"].Value.ToString().Trim() == "-1");
                grdData.Selected.Cells.Add(row.Cells["v_NroCuenta"]);
                grdData.Focus();
                row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = row.Cells["v_NroCuenta"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (grdData.Rows.Any(p => Utils.Windows.DevuelveCuentaDatos(p.Cells["v_NroCuenta"].Value.ToString()).i_CentroCosto == 1 && !Utils.Windows.EsCentroCostoValido(p.Cells["i_IdCentroCostos"].Value.ToString(), false)))
            {
                UltraMessageBox.Show("Por favor ingrese correctamente los centros de costo al detalle.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                var row = grdData.Rows.FirstOrDefault(p => Utils.Windows.DevuelveCuentaDatos(p.Cells["v_NroCuenta"].Value.ToString()).i_CentroCosto == 1 && !Utils.Windows.EsCentroCostoValido(p.Cells["i_IdCentroCostos"].Value.ToString(), false));
                if (row != null)
                {
                    grdData.Selected.Cells.Add(row.Cells["i_IdCentroCostos"]);
                    grdData.Focus();
                    row.Activate();
                    var aCell = row.Cells["i_IdCentroCostos"];
                    grdData.ActiveCell = aCell;
                    grdData.PerformAction(UltraGridAction.EnterEditMode);
                    return false;
                }
            }


            if (!_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) && grdData.Rows.Count(p => p.Cells["d_Cantidad"].Value == null || decimal.Parse(p.Cells["d_Cantidad"].Value.ToString().Trim()) <= 0) != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente la Cantidad.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(p => p.Cells["d_Cantidad"].Value == null || decimal.Parse(p.Cells["d_Cantidad"].Value.ToString().Trim()) <= 0);
                grdData.Selected.Cells.Add(Row.Cells["d_Cantidad"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = Row.Cells["d_Cantidad"];
                this.grdData.ActiveCell = aCell;
                return false;
            }

            if (grdData.Rows.Count(p => p.Cells["d_Precio"].Value == null || decimal.Parse(p.Cells["d_Precio"].Value.ToString().Trim()) == 0) != 0)
            {
                UltraMessageBox.Show("Por favor ingrese correctamente el Precio.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.FirstOrDefault(p => p.Cells["d_Precio"].Value == null || decimal.Parse(p.Cells["d_Precio"].Value.ToString().Trim()) <= 0);
                grdData.Selected.Cells.Add(Row.Cells["d_Precio"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = Row.Cells["d_Precio"];
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
                UltraGridCell aCell = Row.Cells["i_IdAlmacen"];
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
                UltraGridCell aCell = Row.Cells["i_IdUnidadMedida"];
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
                UltraGridCell aCell = Row.Cells["v_IdProductoDetalle"];
                this.grdData.ActiveCell = aCell;
                return false;
            }
            if (grdData.Rows.Where(p => p.Cells["i_IdDestino"].Value == null || p.Cells["i_IdDestino"].Value.ToString() == "-1" || p.Cells["i_IdDestino"].Value.ToString() == "5" || p.Cells["i_IdDestino"].Value.ToString() == "999").Count() != 0)
            {
                UltraMessageBox.Show("Porfavor especifique los destinos correctamente", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UltraGridRow Row = grdData.Rows.Where(p => p.Cells["i_IdDestino"].Value == null || p.Cells["i_IdDestino"].Value.ToString() == "-1" || p.Cells["i_IdDestino"].Value.ToString() == "5" || p.Cells["i_IdDestino"].Value.ToString() == "999").FirstOrDefault();
                grdData.Selected.Cells.Add(Row.Cells["i_IdDestino"]);
                grdData.Focus();
                Row.Activate();
                grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
                UltraGridCell aCell = this.grdData.ActiveRow.Cells["i_IdDestino"];
                this.grdData.ActiveCell = aCell;
                grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                return false;
            }

            var FilasSinSerie = grdData.Rows.Where(p => p.Cells["i_SolicitarNroSerieIngreso"].Value != null && p.Cells["i_SolicitarNroSerieIngreso"].Value.ToString() == "1" && (p.Cells["v_NroSerie"].Value == null || p.Cells["v_NroSerie"].Value.ToString().Trim() == "")).ToList();

            if (FilasSinSerie.Any())
            {
                UltraMessageBox.Show("Por favor registre el Nro. de Serie para el producto.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                UltraGridRow Fila = FilasSinSerie.FirstOrDefault();
                grdData.Focus();
                Fila.Activate();
                Fila.Cells["v_NroSerie"].Activate();
                grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown, true, false);
                return false;
            }


            var FilasSinLote = grdData.Rows.Where(p => p.Cells["i_SolicitarNroLoteIngreso"].Value != null && p.Cells["i_SolicitarNroLoteIngreso"].Value.ToString() == "1" && (p.Cells["v_NroLote"].Value == null || p.Cells["v_NroLote"].Value.ToString().Trim() == "")).ToList();

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


            var FilasConLoteSinSerRequeridas = grdData.Rows.Where(p => p.Cells["i_SolicitarNroLoteIngreso"].Value != null && p.Cells["i_SolicitarNroLoteIngreso"].Value.ToString() == "0" && (p.Cells["v_NroLote"].Value != null && p.Cells["v_NroLote"].Value != "")).ToList();

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
            var FilasConSerieSinSerRequeridas = grdData.Rows.Where(p => p.Cells["i_SolicitarNroSerieIngreso"].Value != null && p.Cells["i_SolicitarNroSerieIngreso"].Value.ToString() == "0" && (p.Cells["v_NroSerie"].Value != null && p.Cells["v_NroSerie"].Value != "")).ToList();

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

            var FilasSinFechaVencimientoLote = grdData.Rows.Where(p => p.Cells["i_SolicitarNroLoteIngreso"].Value != null && p.Cells["i_SolicitarNroLoteIngreso"].Value.ToString() == "1" && (DateTime.Parse(p.Cells["t_FechaCaducidad"].Value.ToString()) == DateTime.MinValue || DateTime.Parse(p.Cells["t_FechaCaducidad"].Value.ToString()).ToShortDateString() == Constants.FechaNula)).ToList();

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




            foreach (var item in grdData.Rows)
            {
                if (item.Cells["i_SolicitarNroLoteIngreso"].Value != null && item.Cells["i_SolicitarNroLoteIngreso"].Value.ToString() == "1")
                {
                    var LoteFechaVencimientoDistinta = _objMovimientoBL.VerificarFechaCaducidadxLote(item.Cells["v_IdProductoDetalle"].Value.ToString(), item.Cells["v_IdMovimientoDetalle"].Value == null ? null : item.Cells["v_IdMovimientoDetalle"].Value.ToString(), item.Cells["v_NroLote"].Value.ToString(), item.Cells["t_FechaCaducidad"].Value == null ? DateTime.MinValue : DateTime.Parse(item.Cells["t_FechaCaducidad"].Value.ToString()));
                    if (LoteFechaVencimientoDistinta != null)
                    {

                        UltraMessageBox.Show("Por favor verifique la fecha de vencimiento del  Lote .\n Lote fue ingresado con fecha venc. distinta en :  N/I - " + LoteFechaVencimientoDistinta.v_Mes + "-" + LoteFechaVencimientoDistinta.v_Correlativo, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        var ProductosLote = grdData.Rows.Where(p => p.Cells["v_IdProductoDetalle"].Value.ToString() == item.Cells["v_IdProductoDetalle"].Value.ToString());
                        UltraGridRow Fila = ProductosLote.FirstOrDefault();
                        grdData.Focus();
                        Fila.Activate();
                        Fila.Cells["v_NombreProducto"].Activate();
                        grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown, true, false);
                        return false;
                    }
                }
            }

            return true;
        }

        void DeshacerODCImportada()
        {
            try
            {
                List<UltraGridRow> Filas = grdData.Rows.Where(p => p.Cells["_DesdeODC"].Value != null && p.Cells["_DesdeODC"].Value.ToString() == "1").ToList();

                if (Filas != null)
                {
                    grdData.Selected.Rows.AddRange(Filas.ToArray());
                    grdData.DeleteSelectedRows(false);
                }
            }
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Eventos de la Grilla
        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdData.ActiveCell == null) return;

            if (e.KeyCode == Keys.Up | e.KeyCode == Keys.Down)
            {
                if (grdData.ActiveCell.Column.Key == "i_IdAlmacen" && grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null)
                {
                    e.SuppressKeyPress = true;
                    return;
                }
            }

            if (grdData.ActiveCell.Column.Key == "v_CodigoInterno")
            {
                if (grdData.ActiveRow.Cells["v_NroGuiaRemision"].Value != null)
                {
                    e.SuppressKeyPress = true;
                    return;
                }
            }

            if (grdData.ActiveCell.Column.Key != "v_NroCuenta" && grdData.ActiveCell.Column.Key != "i_IdAlmacen" && grdData.ActiveCell.Column.Key != "i_IdUnidadMedida" && grdData.ActiveCell.Column.Key != "i_IdDestino")
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
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            grdData.DisplayLayout.Bands[0].Columns["i_Anticipio"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.CheckBox;
            e.Layout.Bands[0].Columns["i_IdDestino"].EditorComponent = ucDestino;
            e.Layout.Bands[0].Columns["i_IdDestino"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].EditorComponent = ucUnidadMedida;
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].EditorComponent = ucAlmacen;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["v_NroCuenta"].EditorComponent = ucRubro;
            e.Layout.Bands[0].Columns["v_NroCuenta"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }

        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "v_CodigoInterno":
                    if (grdData.ActiveRow.Cells["v_CodigoInterno"].Activation == Activation.Disabled) return;
                    if (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) || cboDocumento.Value.ToString() == "8") return;
                    if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "Temporal")
                    {
                        Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(int.Parse(e.Cell.Row.Cells["i_IdAlmacen"].Value.ToString()), null, null, string.IsNullOrEmpty(grdData.ActiveRow.Cells["v_CodigoInterno"].Text) ? string.Empty : grdData.ActiveRow.Cells["v_CodigoInterno"].Text);
                        frm.ShowDialog();
                        if (frm._NombreProducto != null)
                        {
                            grdData.ActiveRow.Cells["v_NombreProducto"].Value = frm._NombreProducto;
                            grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = frm._IdProducto;
                            grdData.ActiveRow.Cells["v_CodigoInterno"].Value = frm._CodigoInternoProducto;
                            grdData.ActiveRow.Cells["Empaque"].Value = frm._Empaque.ToString(CultureInfo.InvariantCulture);
                            grdData.ActiveRow.Cells["UMEmpaque"].Value = frm._UnidadMedida;
                            grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                            grdData.ActiveRow.Cells["i_IdCentroCostos"].Value = "0";
                            grdData.ActiveRow.Cells["i_IdDestino"].Value = cboDestino.Value.ToString();
                            grdData.ActiveRow.Cells["i_IdUnidadMedida"].Value = frm._UnidadMedidaEmpaque;
                            grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = frm._UnidadMedidaEmpaque;
                            grdData.ActiveRow.Cells["v_NroCuenta"].Value = frm._NroCuentaCompra;
                            var cta = Utils.Windows.DevuelveCuentaDatos(frm._NroCuentaCompra);
                            if (cta != null && cta.i_CentroCosto.HasValue)
                            {
                                grdData.ActiveRow.Cells["i_IdCentroCostos"].Activation = cta.i_CentroCosto.Value == 1 ? Activation.AllowEdit : Activation.Disabled;
                            }
                            grdData.ActiveRow.Cells["i_EsServicio"].Value = frm._EsServicio.ToString();
                            UltraGridCell aCell = this.grdData.ActiveRow.Cells["d_Cantidad"];
                            this.grdData.ActiveCell = aCell;
                            grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                            aCell.Activate();
                        }
                    }
                    break;

                case "i_IdCentroCostos":
                    if (e.Cell.Activation == Activation.Disabled) return;
                    Mantenimientos.frmBuscarDatahierarchy frm2 = new Mantenimientos.frmBuscarDatahierarchy(31, "Buscar Centro de Costos");
                    frm2.ShowDialog();
                    if (frm2._itemId != null)
                    {
                        grdData.ActiveRow.Cells["i_IdCentroCostos"].Value = frm2._value2;
                        grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                    }
                    break;
            }
        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
            if (e.Cell.Column.Key == "i_IdDestino")
            {
                var editor = e.Cell.EditorResolved;
                e.Cell.Value = editor.Value;
                CalcularValoresFila(e.Cell.Row);
            }
        }

        private void grdData_AfterCellActivate(object sender, EventArgs e)
        {

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

                    case "v_NroGuiaRemision":
                        Celda = grdData.ActiveCell;
                        Utils.Windows.NumeroDocumentoCelda(Celda, e);
                        break;
                }
            }

        }

        private void grdData_ClickCell(object sender, ClickCellEventArgs e)
        {
            if (e.Cell.Column.Key == "v_NroCuenta" | e.Cell.Column.Key == "i_IdDestino" | e.Cell.Column.Key == "i_IdCentroCostos" | e.Cell.Column.Key == "i_IdAlmacen")
            {
                switch (e.Cell.Column.Key)
                {
                    case "v_NroCuenta":
                        txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_NombreCuenta"].Value != null ? grdData.ActiveRow.Cells["_NombreCuenta"].Value.ToString() : string.Empty;
                        break;

                    case "i_IdDestino":
                        txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_Destino"].Value != null ? grdData.ActiveRow.Cells["_Destino"].Value.ToString() : string.Empty;
                        break;

                    case "i_IdCentroCostos":
                        txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_CentroCosto"].Value != null ? grdData.ActiveRow.Cells["_CentroCosto"].Value.ToString() : string.Empty;
                        break;

                    case "i_IdAlmacen":
                        txtDescripcionColumnas.Text = grdData.ActiveRow.Cells["_Almacen"].Value != null ? grdData.ActiveRow.Cells["_Almacen"].Value.ToString() : string.Empty;
                        if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
                        {
                            e.Cell.CancelUpdate();
                        }
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

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            if (grdData.ActiveCell == null) return;
            if (grdData.ActiveCell.Column.Key.Equals("v_CodigoInterno") && string.IsNullOrWhiteSpace(grdData.ActiveCell.Text))
            {
                grdData.ActiveCell.Row.Cells["v_IdProductoDetalle"].Value = null;
                grdData.ActiveCell.Row.Cells["v_NombreProducto"].Value = null;
            }

            if (grdData.ActiveCell.Column.Key == "v_NroGuiaRemision")
            {
                if (grdData.ActiveRow.Cells["v_NroGuiaRemision"].Value != null)
                {
                    var nroGuia = grdData.ActiveRow.Cells["v_NroGuiaRemision"].Value.ToString();
                    if (nroGuia.Count(x => x.ToString() == "-") == 1)
                    {
                        var serieCorrelativo = nroGuia.Split('-');
                        grdData.ActiveRow.Cells["v_NroGuiaRemision"].Value = string.Format("{0:0000}-{1:00000000}", int.Parse(serieCorrelativo[0]), int.Parse(serieCorrelativo[1]));
                    }
                }
            }
            var columnas = new List<string>
            {
                 "d_Cantidad",
                 "d_isc",
                 "d_ValorVenta",
                 "d_Precio",
                 "d_DescuentoItem"
            };

            if (columnas.Contains(grdData.ActiveCell.Column.Key))
            {
                CalcularValoresDetalle(false);
            }

            if (grdData.ActiveCell.Column.Key == "i_IdCentroCostos")
            {
                if (!Utils.Windows.EsCentroCostoValido(grdData.ActiveCell.Value.ToString()))
                {
                    MessageBox.Show("El centro de costo ingresado no es válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            if (grdData.ActiveCell.Column.Key == "v_DescuentoItem")
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

                    celdaPrecio.SetValue(Utils.Windows.DevuelveValorRedondeado(precio, 7), true);
                    CalcularValoresFila(grdData.ActiveRow, false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ConfigurarGrilla()
        {

            if (Globals.ClientSession.i_ComprasMostrarEmpaque == null || Globals.ClientSession.i_ComprasMostrarEmpaque == 0)
            {
                this.grdData.DisplayLayout.Bands[0].Columns["Empaque"].Hidden = true;
                this.grdData.DisplayLayout.Bands[0].Columns["UMEmpaque"].Hidden = true;
            }

            if (Globals.ClientSession.i_ComprasIscOtrosTributos == null || Globals.ClientSession.i_ComprasIscOtrosTributos.Value == 0)
            {
                this.grdData.DisplayLayout.Bands[0].Columns["d_isc"].Hidden = true;
                this.grdData.DisplayLayout.Bands[0].Columns["d_otrostributos"].Hidden = true;
            }
            //grdData.DisplayLayout.Bands[0].Columns["v_NroPedido"].Hidden = Globals.ClientSession.i_IncluirNingunoCompraVenta == 1 && Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic ? false : Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta != 1;
            grdData.DisplayLayout.Bands[0].Columns["v_NroPedido"].Hidden = Globals.ClientSession.i_IncluirNingunoCompraVenta == 1 && Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic ? false : Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta != 1;


        }
        #endregion

        #region Validaciones de Cajas de Texto
        private void txtSerieDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Tab) && e.KeyChar != Convert.ToChar(Keys.Delete) && e.KeyChar != Convert.ToChar(Keys.Back);
        }

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            int esNumero;

            if (int.TryParse(txtSerieDoc.Text.Trim(), out esNumero))
            {
                Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc, "{0:0000}");
            }
            else
                txtSerieDoc.Text = txtSerieDoc.Text.Trim();

            ComprobarRelacionDocumentoProveedor();
        }

        private void txtCorrelativoDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDoc, e);
        }

        private void txtCorrelativoDoc_Validated(object sender, EventArgs e)
        {
            int i;
            if (int.TryParse(txtCorrelativoDoc.Text.Trim(), out i))
                txtCorrelativoDoc.Text = i.ToString("00000000");
            ComprobarRelacionDocumentoProveedor();
        }

        private void cboMoneda_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTipoCompra.Items.Count > 1)
            {
                switch (cboMoneda.Value.ToString())
                {
                    case "1":
                        cboTipoCompra.Value = "01";
                        break;

                    case "2":
                        cboTipoCompra.Value = "02";
                        break;
                }
            }
            CalcularDetraccionesDetalle();
        }

        private void txtCorrelativoODC_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscar")
            {
                #region Validaciones
                if (grdData.Rows.Any())
                {
                    UltraMessageBox.Show("No se puede importar la Orden de Compra cuando hay items en el detalle!", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtSerieODC.Clear();
                    txtCorrelativoODC.Clear();
                    return;
                }

                if (string.IsNullOrEmpty(txtTipoCambio.Text.Trim()))
                {
                    UltraMessageBox.Show("Por favor ingrese un tipo de cambio válido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtTipoCambio.Focus();
                    return;
                }
                #endregion

                #region Importación de Orden de Compra
                if (!string.IsNullOrEmpty(txtSerieODC.Text) && !string.IsNullOrEmpty(txtCorrelativoODC.Text))
                {
                    var objOperationResult = new OperationResult();
                    var cabecera = _objComprasBL.ObtenerOrdenDeCompraEnKeyValuesDTO(ref objOperationResult, txtSerieODC.Text.Trim(), txtCorrelativoODC.Text.Trim());

                    switch ((OrdenCompraEstados)objOperationResult.Success)
                    {
                        case OrdenCompraEstados.Pendiente:

                            List<KeyValueDetalleCompraVentaDTO> Detalles = _objComprasBL.ObtenerOrdenDeCompraDetallesEnKeyValuesDTO(ref objOperationResult, cabecera.v_IdOrdenCompra);
                            _compraDto.v_IdProveedor = cabecera.v_IdProveedor;
                            txtRazonSocial.Text = cabecera.RazonSocialProveedor;
                            txtCodigoProveedor.Text = cabecera.CodProveedor;
                            txtRucProveedor.Text = cabecera.RUCProveedor;
                            cboMoneda.Value = cabecera.i_IdMoneda.ToString();
                            chkAfectoIGV.Checked = cabecera.i_PreciosAfectosIgv == 1;
                            chkPrecInIGV.Checked = cabecera.i_PreciosIncluyeIgv == 1;

                            foreach (KeyValueDetalleCompraVentaDTO detalle in Detalles)
                            {
                                btnAgregar_Click(sender, e);
                                if (grdData.Rows.Any())
                                {
                                    var fila = grdData.Rows.ToList().Last();
                                    fila.Cells["v_NroCuenta"].Value = _objComprasBL.ObtenerCuentaContableProducto(detalle.v_IdProductoDetalle);
                                    fila.Cells["v_NombreProducto"].Value = detalle.v_Descripcion;
                                    fila.Cells["v_IdProductoDetalle"].Value = detalle.v_IdProductoDetalle;
                                    fila.Cells["v_CodigoInterno"].Value = detalle.v_CodInterno;
                                    fila.Cells["Empaque"].Value = detalle.d_Empaque.ToString();
                                    fila.Cells["UMEmpaque"].Value = detalle.EmpaqueUM;
                                    fila.Cells["i_RegistroEstado"].Value = "Modificado";
                                    fila.Cells["i_IdUnidadMedida"].Value = detalle.i_IdUnidadMedida.ToString();
                                    fila.Cells["i_IdUnidadMedidaProducto"].Value = detalle.i_IdUnidadMedida.ToString();
                                    fila.Cells["i_EsServicio"].Value = detalle.i_EsServicio.ToString();
                                    fila.Cells["d_Cantidad"].Value = detalle.d_Cantidad.ToString();
                                    fila.Cells["d_Precio"].Value = detalle.d_PrecioUnitario.ToString();
                                    fila.Cells["_DesdeODC"].Value = true;
                                    fila.Cells["v_NroPedido"].Value = cabecera.v_DocumentoInterno;
                                    fila.Cells["t_FechaCaducidad"].Value = DateTime.Parse(Constants.FechaNula);
                                    fila.Cells["t_FechaFabricacion"].Value = DateTime.Parse(Constants.FechaNula);
                                }
                            }

                            CalcularValoresDetalle(false);
                            break;

                        case OrdenCompraEstados.Cancelado:
                            UltraMessageBox.Show("La Orden de Compra fue Cancelada.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            break;

                        case OrdenCompraEstados.Procesado:
                            UltraMessageBox.Show("La Orden de Compra ya fue Procesada Anteriormente.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;

                        case OrdenCompraEstados.Terminado:
                            UltraMessageBox.Show("La Orden de Compra ha sido Terminada.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;

                        case OrdenCompraEstados.NoEncontrada:
                            UltraMessageBox.Show("Orden de compra no encontrada!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            break;

                        case OrdenCompraEstados.Error:
                            UltraMessageBox.Show(objOperationResult.ErrorMessage + "\n" + objOperationResult.ExceptionMessage + "\nTARGET: " + objOperationResult.AdditionalInformation, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }
                #endregion
            }
        }

        private void txtSerieDocRef_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDocRef, e);
        }

        private void txtSerieDocRef_Leave(object sender, EventArgs e)
        {
            int esNumero;

            if (int.TryParse(txtSerieDocRef.Text.Trim(), out esNumero))
            {
                Utils.Windows.FijarFormatoUltraTextBox(txtSerieDocRef, "{0:0000}");
            }
            else
                txtSerieDocRef.Text = txtSerieDocRef.Text.Trim();

            DevuelveCompraReferencia();
        }

        private void txtCodigoDetraccion_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCodigoDetraccion, e);
        }

        private void txtPorcDetraccion_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtPorcDetraccion, e);
        }

        private void txtNroDetraccion_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtNroDetraccion, e);
        }

        private void txtTipoCambio_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtTipoCambio, e);
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
                        var x = _ListadoComboDocumentos.Find(p => p.Id == cboDocumento.Value | p.Id == cboDocumento.Text);
                        if (x == null)
                        {
                            cboDocumento.Value = "-1";
                        }
                    }

                    if (_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())) || cboDocumento.Value.ToString() == "8")
                    {
                        cboDocumentoRef.Enabled = true;
                        txtSerieDocRef.Enabled = true;
                        txtCorrelativoDocRef.Enabled = true;
                        dtpFechaRef.Enabled = true;
                        DevuelveCompraReferencia();
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
                    }
                }

                _EsNotadeCredito = _objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString().Trim()));

                if (_EsNotadeCredito)
                {
                    UltraGridColumn c = grdData.DisplayLayout.Bands[0].Columns["i_IdAlmacen"];
                    c.CellActivation = Activation.ActivateOnly;
                    c.CellClickAction = CellClickAction.CellSelect;
                    btnPagar.Enabled = false;
                }
                else
                {
                    UltraGridColumn c = grdData.DisplayLayout.Bands[0].Columns["i_IdAlmacen"];
                    c.CellActivation = Activation.AllowEdit;
                    c.CellClickAction = CellClickAction.Edit;
                    btnPagar.Enabled = true;
                   // dtpFechaVencimiento.Enabled = cboDocumento.Value.ToString().Trim() == "14";
                }
            }
            else
            {
                cboDocumento.Value = "-1";
            }
        }

        private void cboDocumentoRef_Leave(object sender, EventArgs e)
        {
            if (cboDocumentoRef.Value != null)
            {
                if (strModo == "Nuevo")
                {
                    if (cboDocumentoRef.Text.Trim() == "")
                    {
                        cboDocumentoRef.Value = "-1";
                    }
                    else
                    {
                        var x = _ListadoComboDocumentosRef.Find(p => p.Id == cboDocumentoRef.Value | p.Id == cboDocumentoRef.Text);
                        if (x == null)
                        {
                            cboDocumentoRef.Value = "-1";
                        }
                    }
                }
            }
            else
            {
                cboDocumentoRef.Value = "-1";
            }
        }

        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
            txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Compra, _compraDto.t_FechaRegistro, dtpFechaRegistro.Value, _compraDto.v_Correlativo, 0);
            dtpFechaEmision.MaxDate = dtpFechaRegistro.Value;
            dtpFechaEmision.Value = dtpFechaRegistro.Value;
            if (new CierreMensualBL().VerificarMesCerrado(txtPeriodo.Text, txtMes.Text, (int)ModulosSistema.Compras) || Utilizado == "KARDEX")
            {
                btnGuardar.Visible = false;
                this.Text = Utilizado == "KARDEX" ? "Registro de Compra" : "Registro de Compra [MES CERRADO]";
                if (Utilizado == "KARDEX")
                {

                    btnSalir.Visible = false;
                    btnAgregar.Visible = false;
                    btnEliminar.Visible = false;
                    btnPagar.Visible = false;
                }
            }
            else
            {
                btnGuardar.Visible = true;
                Text = "Registro de Compra";
            }
        }

        private void txtCorrelativoGR_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (e.Button.Key == "btnBuscar")
            {
                txtCorrelativoGR.Text = DarFormatoGuiasRemision(txtCorrelativoGR.Text);

                if (!string.IsNullOrEmpty(txtCorrelativoGR.Text))
                {
                    if (grdData.Rows.Any() && GuiaRemisionImportada == false)
                    {
                        UltraMessageBox.Show("No se puede importar la Guia de Remisión si hay items en el detalle", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txtCorrelativoGR.Clear();
                        txtSerieGR.Clear();
                    }

                    if (decimal.Parse(txtTipoCambio.Text.Trim()) <= 0)
                    {
                        UltraMessageBox.Show("Por favor ingrese un tipo de cambio válido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txtCorrelativoGR.Clear();
                        txtSerieGR.Clear();
                        txtTipoCambio.Focus();
                    }

                    if (cboIGV.Value.ToString() == "-1")
                    {
                        UltraMessageBox.Show("Por favor seleccione un valor para el IGV", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        txtCorrelativoGR.Clear();
                        txtSerieGR.Clear();
                        cboIGV.Focus();
                    }
                    if (!string.IsNullOrEmpty(txtSerieGR.Text) && !string.IsNullOrEmpty(txtCorrelativoGR.Text))
                    {
                        var correlativos = txtCorrelativoGR.Text.Contains(",") ? txtCorrelativoGR.Text.Split(',').Select(p => p.Trim()) : new List<string> { txtCorrelativoGR.Text };
                        if (GuiaRemisionImportada) return;

                        foreach (var correlativo in correlativos)
                        {
                            var objOperationResult = new OperationResult();
                            if (_objComprasBL.ComprobarSiFueLlamadaEnCompras(ref objOperationResult, txtSerieGR.Text, correlativo, _compraDto.v_IdProveedor))
                            {
                                UltraMessageBox.Show(string.Format("Esta Guía de Remisión {0}-{1} ya fue usada en otra Compra.", txtSerieGR.Text, correlativo), "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                txtSerieGR.Clear();
                                txtCorrelativoGR.Clear();
                                return;
                            }

                            if (_compraDto.v_IdProveedor == null)
                            {
                                UltraMessageBox.Show("Por favor ingrese un proveedor.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }

                            _guiaremisioncompraDto = _objComprasBL.DevolverGuiaRemisionCompraCabecera(ref objOperationResult, txtSerieGR.Text.Trim(), correlativo, _compraDto.v_IdProveedor);

                            if (_guiaremisioncompraDto != null)
                            {
                                var lGuiaremisioncompradetalleDto = _objComprasBL.DevolverGuiaRemisionCompraDetalle(ref objOperationResult, _guiaremisioncompraDto.v_IdGuiaCompra);

                                if (lGuiaremisioncompradetalleDto.Count > 0)
                                {
                                    foreach (guiaremisioncompradetalleDto fila in lGuiaremisioncompradetalleDto)
                                    {
                                        UltraGridRow ultima = grdData.DisplayLayout.Bands[0].AddNew();
                                        grdData.Rows.Move(ultima, grdData.Rows.Count() - 1);
                                        this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(ultima);
                                        ultima.Cells["i_RegistroEstado"].Value = "Agregado";
                                        ultima.Cells["v_IdProductoDetalle"].Value = fila.v_IdProductoDetalle;
                                        ultima.Cells["v_NroCuenta"].Value = fila.v_NroCuenta;
                                        ultima.Cells["d_Cantidad"].Value = fila.d_Cantidad;
                                        ultima.Cells["i_IdAlmacen"].Value = fila.i_IdAlmacen??0;
                                        ultima.Cells["i_IdUnidadMedida"].Value = fila.i_IdUnidadMedida??0;
                                        ultima.Cells["i_IdDestino"].Value = "0";
                                        ultima.Cells["i_IdCentroCostos"].Value = "0";
                                        ultima.Cells["v_IdMovimientoDetalle"].Value = fila.v_IdMovimientoDetalle;
                                        ultima.Cells["i_Eliminado"].Value = "0";
                                        ultima.Cells["i_Anticipio"].Value = "0";
                                        ultima.Cells["_DesdeGRC"].Value = "1";
                                        ultima.Cells["i_InsertaIdUsuario"].Value = fila.i_InsertaIdUsuario.ToString();
                                        ultima.Cells["v_NroGuiaRemision"].Value = _guiaremisioncompraDto.v_SerieDocumento + "-" + _guiaremisioncompraDto.v_CorrelativoDocumento;
                                        ultima.Cells["i_RegistroTipo"].Value = "Temporal";
                                        ultima.Cells["i_RegistroEstado"].Value = "Modificado";
                                        ultima.Cells["d_Precio"].Value = fila.d_PrecioUnitario;
                                        ultima.Cells["v_NroLote"].Value = fila.v_NroLote;
                                        ultima.Cells["v_NroSerie"].Value = fila.v_NroSerie;
                                        ultima.Cells["t_FechaCaducidad"].Value = fila.t_FechaCaducidad;
                                     

                                        ultima.Cells["t_FechaFabricacion"].Value = DateTime.Parse(Constants.FechaNula);
                                        CalcularValoresFila(ultima, false);
                                    }

                                    DevolverNombreRelaciones();
                                    RestringirXGuiaRemision(true);
                                    GuiaRemisionImportada = true;
                                    CalcularTotales();
                                }
                                else
                                {
                                    UltraMessageBox.Show("Los datos del detalle de la Guía de Remisión " + txtSerieGR.Text + "-" + correlativo + " no son válidos", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    return;
                                }
                            }
                            else
                            {
                                UltraMessageBox.Show("La Guía de Remisión " + txtSerieGR.Text + "-" + correlativo + " no existe.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                txtSerieGR.Clear();
                                txtCorrelativoGR.Clear();
                            }
                        }
                    }
                }
            }

        }

        private void txtCorrelativoODC_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoODC, e);
        }

        private void txtSerieODC_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieODC, e);
        }

        private void txtCorrelativoODC_Validated(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCorrelativoODC.Text))
            {
                Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoODC, "{0:00000000}");
            }
            else
            {
                if (string.IsNullOrEmpty(txtSerieODC.Text))
                {
                    DeshacerODCImportada();
                }
            }
        }

        private void txtSerieODC_Validated(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSerieODC.Text))
            {
                Utils.Windows.FijarFormatoUltraTextBox(txtSerieODC, "{0:0000}");
            }
            else
            {
                if (string.IsNullOrEmpty(txtCorrelativoODC.Text))
                {
                    DeshacerODCImportada();
                }
            }
        }
        #endregion

        private void cboDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDestino.Value.ToString() == "5")
            {
                UltraGridColumn c = grdData.DisplayLayout.Bands[0].Columns["i_IdDestino"];
                c.CellActivation = Activation.AllowEdit;
                c.CellClickAction = CellClickAction.CellSelect;

                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    Fila.Cells["i_IdDestino"].Value = "-1";
                    Fila.Cells["i_RegistroEstado"].Value = "Modificado";
                }
            }
            else
            {
                UltraGridColumn c = grdData.DisplayLayout.Bands[0].Columns["i_IdDestino"];
                c.CellActivation = Activation.Disabled;
                c.CellClickAction = CellClickAction.RowSelect;

                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    Fila.Cells["i_IdDestino"].Value = cboDestino.Value.ToString();
                    Fila.Cells["i_RegistroEstado"].Value = "Modificado";
                }
            }

            if (cboDestino.Value.ToString() != "4")
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

            CalcularValoresDetalle();
        }

        private void btnPagar_Click(object sender, EventArgs e)
        {
            switch (strModo)
            {
                case "Guardado":
                    {
                        frmPago frm = new frmPago("PagoRapido", _pstrIdMovimiento_Nuevo, null);
                        frm.ShowDialog();
                    }
                    break;
                case "Edicion":
                    {

                        frmPago frm = new frmPago("PagoRapido", strIdcompra, null);
                        frm.ShowDialog();
                    }
                    break;
            }
        }

        void LlenarCuentasRubros()
        {
            OperationResult objOperationResult = new OperationResult();
            Task.Factory.StartNew(() =>
            {
                datahierarchyDto _datahierarchyDto = new datahierarchyDto();
                _datahierarchyDto = _objDatahierarchyBL.ObtenerDataHierarchyPorValue1(ref objOperationResult, 52, "mercadería");
                if (_datahierarchyDto != null)
                {
                    CtaRubroMercaderia = _datahierarchyDto.v_Value2;
                }

                _datahierarchyDto = _objDatahierarchyBL.ObtenerDataHierarchyPorValue1(ref objOperationResult, 52, "servicio");
                if (_datahierarchyDto != null)
                {
                    CtaRubroServicio = _datahierarchyDto.v_Value2;
                }
            }
            );
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            var f = new frmConsultaVentasDetraccion();
            f.ShowDialog();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool ValidarCuentas(List<UltraGridRow> filas, out string nroCuenta)
        {
            nroCuenta = string.Empty;
            try
            {
                var cuentas = filas.Where(f => f.Cells["v_NroCuenta"].Value != null)
                    .Select(p => p.Cells["v_NroCuenta"].Value.ToString()).Distinct().ToList();

                foreach (var cuenta in cuentas)
                {
                    if (string.IsNullOrWhiteSpace(cuenta.Trim())) { nroCuenta = "*Cuenta en Blanco*"; return false; }
                    if (!Utils.Windows.EsCuentaImputable(cuenta.Trim()))
                    {
                        nroCuenta = cuenta;
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            var vieneGuiaRemision = e.Row.Cells["v_NroGuiaRemision"].Value != null &&
                         !string.IsNullOrWhiteSpace(e.Row.Cells["v_NroGuiaRemision"].Value.ToString());

            if (vieneGuiaRemision && cboDocumento.Value != null && !_objDocumentoBL.DocumentoEsInverso(int.Parse(cboDocumento.Value.ToString())))
            {
                e.Row.Cells["d_Cantidad"].Activation = Activation.Disabled;
                e.Row.Cells["d_Cantidad"].ToolTipText = "Cantidad deshabilitada porque viene de guia de remisión";
                e.Row.Cells["i_IdUnidadMedida"].Activation = Activation.Disabled;
                e.Row.Cells["i_IdUnidadMedida"].ToolTipText = "Unidad Medida deshabilitada porque viene de guia de remisión";
                e.Row.Cells["v_CodigoInterno"].Activation = Activation.Disabled;
                e.Row.Cells["v_CodigoInterno"].ToolTipText = "Código deshabilitado porque viene de guia de remisión";
            }

            if (e.Row.Band.Index == 0 && e.Row.Cells["t_FechaCaducidad"].Value != null && DateTime.Parse(e.Row.Cells["t_FechaCaducidad"].Value.ToString()).Date.ToShortDateString() == Constants.FechaNula)
            {
                e.Row.Cells["t_FechaCaducidad"].Appearance.BackColor = Color.White;
                e.Row.Cells["t_FechaCaducidad"].Appearance.ForeColor = Color.White;
            }
            if (e.Row.Band.Index == 0 && e.Row.Cells["t_FechaFabricacion"].Value != null && DateTime.Parse(e.Row.Cells["t_FechaFabricacion"].Value.ToString()).Date.ToShortDateString() == Constants.FechaNula)
            {
                e.Row.Cells["t_FechaFabricacion"].Appearance.BackColor = Color.White;
                e.Row.Cells["t_FechaFabricacion"].Appearance.ForeColor = Color.White;
            }

        }

        private void grdData_AfterEnterEditMode(object sender, EventArgs e)
        {
            if (grdData.ActiveCell != null && grdData.ActiveCell.Value != null && grdData.ActiveCell.Column.Key.Equals("d_PrecioVenta"))
            {
                decimal.TryParse(grdData.ActiveCell.Value.ToString(), out valorOriginalCelda);
            }
        }

        private void txtGlosa_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.CaracteresValidos((sender as Infragistics.Win.UltraWinEditors.UltraTextEditor), e);
        }

        private void txtSerieDocRef_Validated(object sender, EventArgs e)
        {
           
        }

        private void ultraDropDownButton1_DroppingDown(object sender, CancelEventArgs e)
        {
            panelPercepcion.Controls.Clear();
            int i;
            decimal d;
            _compraDto.i_IdTipoDocumento = int.TryParse(cboDocumento.Value.ToString(), out i) ? i : -1;
            _compraDto.v_SerieDocumento = txtSerieDoc.Text;
            _compraDto.v_CorrelativoDocumento = txtCorrelativoDoc.Text;
            _compraDto.d_Total = decimal.TryParse(txtTotal.Text, out d) ? d : 0;
            _compraDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
            _compraDto.d_TipoCambio = decimal.TryParse(txtTipoCambio.Text, out d) ? d : 0m;
          
        }

        private void cboDocumento_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {

        }

        private void cboDocumento_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Utils.Windows.LimitarSeriePorTipoDocumento(cboDocumento, txtSerieDoc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cboDocumentoRef_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Utils.Windows.LimitarSeriePorTipoDocumento(cboDocumentoRef, txtSerieDocRef);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

