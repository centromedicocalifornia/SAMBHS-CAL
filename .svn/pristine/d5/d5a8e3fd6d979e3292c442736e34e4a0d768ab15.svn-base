using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using SAMBHS.Common.BE;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Common.Resource;
using SAMBHS.Almacen.BL;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Contabilidad.BL;
using LoadingClass;
using SAMBHS.Compra.BL;
using SAMBHS.Windows.WinClient.UI.Mantenimientos;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmNotaIngreso : Form
    {

        readonly NodeWarehouseBL _objNodeWarehouseBL = new NodeWarehouseBL();
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        readonly DocumentoBL _objDocumentoBL = new DocumentoBL();
        movimientoDto _movimientoDto = new movimientoDto();
        movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
        List<KeyValueDTO> _ListadoMovimientos = new List<KeyValueDTO>();
        UltraCombo ucUnidadMedida = new UltraCombo();
        private UltraCombo ucAlmacen = new UltraCombo();
        readonly UltraCombo ucTipoDocumento = new UltraCombo();
        int _MaxV, _ActV;
        string _Mode;
        string _IdMovimientoss;
        public string _pstrIdMovimiento_Nuevo;
        string strModo, strIdmovimiento;
        string FormatoCantidad;
        public string Utilizado;
        /// <summary>
        /// Lista que almacenará los insumos de todos los productos terminados que luego iran a una nota de salida.
        /// </summary>
        private BindingList<movimientodetallerecetafinalDto> _listaRecetaFinalMovimiento = new BindingList<movimientodetallerecetafinalDto>();

        #region Temporales DetalleVenta
        List<movimientodetalleDto> _TempDetalle_AgregarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> _TempDetalle_ModificarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> _TempDetalle_EliminarDto = new List<movimientodetalleDto>();

        List<movimientodetallerecetafinalDto> _TempDetalleReceta_AgregarDto = new List<movimientodetallerecetafinalDto>();
        List<movimientodetallerecetafinalDto> _TempDetalleReceta_ModificarDto = new List<movimientodetallerecetafinalDto>();
        List<movimientodetallerecetafinalDto> _TempDetalleReceta_EliminarDto = new List<movimientodetallerecetafinalDto>();
        #endregion

        public frmNotaIngreso(string Modo, string IdMovimiento, string Utilizadoen = null)
        {
            strModo = Modo;
            strIdmovimiento = IdMovimiento;
            Utilizado = Utilizadoen;
            InitializeComponent();
        }

        private void frmNotaIngreso_Load(object sender, EventArgs e)
        {
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            OperationResult objOperationResult = new OperationResult();
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            txtMes.Text = DateTime.Now.Month.ToString("00");

            if (new CierreMensualBL().VerificarMesCerrado(Globals.ClientSession.i_Periodo.ToString().Trim(), DateTime.Now.Month.ToString("00").Trim(), (int)ModulosSistema.Almacen) || Utilizado == "KARDEX")
            {
                btnGuardar.Visible = false;
                this.Text = Utilizado == "KARDEX" ? "Nota de Ingreso" : "Nota de Ingreso [MES CERRADO]";
                if (Utilizado == "KARDEX")
                {
                    BtnImprimir.Visible = false;
                    btnSalir.Visible = false;
                    btnAgregar.Visible = false;
                    btnEliminar.Visible = false;
                }
            }
            else
            {
                btnGuardar.Visible = true;
                Text = @"Nota de Ingreso";
            }

            #region Cargar Combos

            var bl = new DatahierarchyBL();
            Utils.Windows.LoadUltraComboEditorList(cboEstablecimiento, "Value1", "Id", new EstablecimientoBL().ObtenerEstablecimientosValueDto(ref objOperationResult, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMotivo, "Value1", "Id", bl.GetDataHierarchiesForCombo(ref objOperationResult, 19, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboMoneda, "Value1", "Id", bl.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboEditorList(cboAlmacen, "Value1", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaCombo(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento ?? 1), DropDownListAction.Select);
            CargarCombosDetalle();
            CargarCombosDetalleReceta();
            #endregion
            FormatoDecimalesGrilla(Globals.ClientSession.i_CantidadDecimales ?? 2, Globals.ClientSession.i_PrecioDecimales ?? 2);
            ObtenerListadoMovimientos(txtPeriodo.Text, txtMes.Text);
            if (_movimientoDto.v_OrigenTipo != null)
            {
                btnAgregar.Enabled = false;
                dtpFecha.Enabled = false;
                btnEliminar.Enabled = false;
                btnGuardar.Enabled = false;
                btnBuscarProveedor.Enabled = false;
                cboAlmacen.Enabled = false;
                cboMoneda.Enabled = false;
                cboMotivo.Enabled = false;
                txtTipoCambio.Enabled = false;
                txtProveedor.Enabled = false;
                txtGlosa.Enabled = false;
                chkDevolucion.Enabled = false;
            }
            btnBuscarGuiaRemision.Tag = "0";

            #region Configuracion Grilla
            int DecimalesCantidad = (int)Globals.ClientSession.i_CantidadDecimales;

            if (Globals.ClientSession.i_ComprasMostrarEmpaque == null || Globals.ClientSession.i_ComprasMostrarEmpaque == 0)
            {
                this.grdData.DisplayLayout.Bands[0].Columns["Empaque"].Hidden = true;
                this.grdData.DisplayLayout.Bands[0].Columns["UMEmpaque"].Hidden = true;
            }
            if ((int)Globals.ClientSession.i_CantidadDecimales > 0)
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
            grdData.DisplayLayout.Bands[0].Columns["v_NroPedido"].Hidden = Globals.ClientSession.i_IncluirNingunoCompraVenta == 1 && Globals.ClientSession.v_RucEmpresa == Constants.RucAgrofergic ? false : Globals.ClientSession.i_IncluirPedidoExportacionCompraVenta != 1;
            #endregion
            if (Application.OpenForms["LoadingForm"] != null) ((LoadingForm)Application.OpenForms["LoadingForm"]).CloseWindow();
            if (Application.OpenForms["frmMaster"] != null) ((frmMaster)Application.OpenForms["frmMaster"]).Activate();

            cboEstablecimiento.Value = Globals.ClientSession.i_IdEstablecimiento.Value.ToString();
            lblProductoIngresar.Visible = Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita;
            txtProductoIngresar.Visible = Globals.ClientSession.v_RucEmpresa == Constants.RucHormiguita;

        }

        private void CargarCombosDetalleReceta()
        {
            OperationResult objOperationResult = new OperationResult();

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

            ucAlmacen.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucAlmacen, "Id", "Id", _objNodeWarehouseBL.ObtenerAlmacenesParaComboGrid(ref objOperationResult, null, Globals.ClientSession.i_IdEstablecimiento.Value), DropDownListAction.Select);
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var ultimaFila = grdData.Rows.LastOrDefault();

            if (ultimaFila == null || ultimaFila.Cells["v_IdProductoDetalle"].Value != null)
            {
                UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Agregado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["i_IdUnidad"].Value = "-1";
                row.Cells["i_IdTipoDocumento"].Value = "-1";
                row.Cells["d_Precio"].Value = "0";
                row.Cells["d_Cantidad"].Value = "0";
                row.Cells["d_Total"].Value = "0";
                row.Cells["i_EsProductoFinal"].Value = "0";
            }

            var aCell = grdData.ActiveRow.Cells["v_CodigoInterno"];
            grdData.ActiveCell = aCell;
            grdData.Focus();
            grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvDatos.Validate(true, false).IsValid)
            {
                if (txtTipoCambio.Value == null || txtTipoCambio.Value.ToString() == "" || decimal.Parse(txtTipoCambio.Text) <= 0)
                {
                    UltraMessageBox.Show("Por Favor ingrese un tipo de cambio válido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtTipoCambio.Focus();
                    return;
                }

                if (!grdData.Rows.Any())
                {
                    UltraMessageBox.Show("No se permite guardar mientras el detalle está vacío", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (grdData.Rows.Any(Fila => Fila.Cells["v_IdProductoDetalle"].Value == null))
                {
                    UltraMessageBox.Show("Por favor ingrese correctamente todos los productos al detalle.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!Validacion()) return;

                CalcularTotal();
                if (_Mode == "New")
                {
                    using (new PleaseWait(Location, "Por favor espere..."))
                    {
                        while (_objMovimientoBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text, (int)TipoDeMovimiento.NotadeIngreso) == false)
                        {
                            txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                        }
                        _movimientoDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                        _movimientoDto.i_IdAlmacenOrigen = int.Parse(cboAlmacen.Value.ToString());
                        _movimientoDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                        _movimientoDto.i_IdTipoMotivo = int.Parse(cboMotivo.Value.ToString());
                        _movimientoDto.t_Fecha = dtpFecha.Value;
                        _movimientoDto.v_Glosa = txtGlosa.Text.Trim();
                        _movimientoDto.v_Mes = txtMes.Text.Trim();
                        _movimientoDto.v_Periodo = txtPeriodo.Text.Trim();
                        _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                        _movimientoDto.v_Correlativo = txtCorrelativo.Text;
                        _movimientoDto.d_TotalCantidad = txtCantidad.Text == "" ? 0 : decimal.Parse(txtCantidad.Text);
                        _movimientoDto.d_TotalPrecio = txtTotal.Text == "" ? 0 : decimal.Parse(txtTotal.Text);
                        _movimientoDto.i_EsDevolucion = chkDevolucion.Checked == true ? 1 : 0;
                        _movimientoDto.v_NroGuiaVenta = btnBuscarGuiaRemision.Text;
                        _movimientoDto.i_IdEstablecimiento = int.Parse(Globals.ClientSession.i_IdEstablecimiento.Value.ToString());
                        LlenarTemporales();

                        using (var ts = TransactionUtils.CreateTransactionScope())
                        {
                            var idMov = _objMovimientoBL.InsertarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto.OrderBy(r => r.CodigoProducto).ToList());
                            if (objOperationResult.Success == 0)
                            {
                                MessageBox.Show(
                                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            if (ultraGrid1.Rows.Any())
                            {
                                LlenarTemporalesRecetasFinales();
                                new ProductoBL().InsertaRecetaFinalLista(ref objOperationResult, _TempDetalleReceta_AgregarDto, Globals.ClientSession.GetAsList(), idMov);
                                if (objOperationResult.Success == 0)
                                {
                                    MessageBox.Show(
                                        objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                                        objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }

                            ts.Complete();
                        }
                        _TempDetalleReceta_AgregarDto = new List<movimientodetallerecetafinalDto>();
                        _TempDetalleReceta_ModificarDto = new List<movimientodetallerecetafinalDto>();
                        _TempDetalleReceta_EliminarDto = new List<movimientodetallerecetafinalDto>();
                    }
                }
                else if (_Mode == "Edit")
                {
                    using (new LoadingClass.PleaseWait(this.Location, "Por favor espere..."))
                    {
                        _movimientoDto.d_TipoCambio = txtTipoCambio.Text == string.Empty ? 0 : decimal.Parse(txtTipoCambio.Text);
                        _movimientoDto.i_IdAlmacenOrigen = int.Parse(cboAlmacen.Value.ToString());
                        _movimientoDto.i_IdMoneda = int.Parse(cboMoneda.Value.ToString());
                        _movimientoDto.i_IdTipoMotivo = int.Parse(cboMotivo.Value.ToString());
                        _movimientoDto.t_Fecha = dtpFecha.Value;
                        _movimientoDto.v_Glosa = txtGlosa.Text.Trim();
                        _movimientoDto.v_Mes = txtMes.Text.Trim();
                        _movimientoDto.v_Periodo = txtPeriodo.Text.Trim();
                        _movimientoDto.i_IdTipoMovimiento = (int)TipoDeMovimiento.NotadeIngreso;
                        _movimientoDto.v_Correlativo = txtCorrelativo.Text;
                        _movimientoDto.d_TotalCantidad = txtCantidad.Text == "" ? 0 : decimal.Parse(txtCantidad.Text);
                        _movimientoDto.d_TotalPrecio = txtTotal.Text == "" ? 0 : decimal.Parse(txtTotal.Text);
                        _movimientoDto.i_EsDevolucion = chkDevolucion.Checked ? 1 : 0;
                        _movimientoDto.i_IdEstablecimiento = int.Parse(Globals.ClientSession.i_IdEstablecimiento.ToString());
                        _movimientoDto.v_NroGuiaVenta = btnBuscarGuiaRemision.Text;
                        LlenarTemporales();

                        using (var ts = TransactionUtils.CreateTransactionScope())
                        {
                            _objMovimientoBL.ActualizarMovimiento(ref objOperationResult, _movimientoDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto.OrderBy(r => r.CodigoProducto).ToList(), _TempDetalle_ModificarDto.OrderBy(r => r.CodigoProducto).ToList(), _TempDetalle_EliminarDto);
                            if (objOperationResult.Success == 0)
                            {
                                MessageBox.Show(
                                    objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                                    objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                return;
                            }

                            if (_TempDetalleReceta_EliminarDto.Any() || ultraGrid1.Rows.Any())
                            {
                                LlenarTemporalesRecetasFinales();
                                new ProductoBL().ActualizarRecetaFinalLista(ref objOperationResult, _TempDetalleReceta_AgregarDto, _TempDetalleReceta_ModificarDto, _TempDetalleReceta_EliminarDto, Globals.ClientSession.GetAsList(), _movimientoDto.v_IdMovimiento);
                                if (objOperationResult.Success == 0)
                                {
                                    MessageBox.Show(
                                        objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                                        objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }

                            ts.Complete();
                            _TempDetalleReceta_AgregarDto = new List<movimientodetallerecetafinalDto>();
                            _TempDetalleReceta_ModificarDto = new List<movimientodetallerecetafinalDto>();
                            _TempDetalleReceta_EliminarDto = new List<movimientodetallerecetafinalDto>();
                        }
                    }
                }

                if (objOperationResult.Success == 1)
                {
                    if (btnBuscarGuiaRemision.Tag != null && btnBuscarGuiaRemision.Tag.ToString() == "1" && !string.IsNullOrEmpty(btnBuscarGuiaRemision.Text.Trim()))
                    {
                        var guiaremision = btnBuscarGuiaRemision.Text.Split('-');

                        _objMovimientoBL.ActualizarGuiaRemisionDespachada(ref objOperationResult, guiaremision[0], guiaremision[1]);
                        if (objOperationResult.Success == 0)
                        {
                            MessageBox.Show(
                                objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                                objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    strModo = "Guardado";
                    EdicionBarraNavegacion(true);
                    _pstrIdMovimiento_Nuevo = _movimientoDto.v_IdMovimiento;

                    strIdmovimiento = _movimientoDto.v_IdMovimiento;
                    ObtenerListadoMovimientos(txtPeriodo.Text, txtMes.Text);

                    var mg = UltraMessageBox.Show("El registro se ha guardado correctamente \r ¿Desea realizar uno nuevo?", "Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (mg == DialogResult.Yes)
                        btnNuevoMovimiento_Click(sender, e);
                }

                _TempDetalle_AgregarDto = new List<movimientodetalleDto>();
                _TempDetalle_ModificarDto = new List<movimientodetalleDto>();
                _TempDetalle_EliminarDto = new List<movimientodetalleDto>();
            }
            else
            {
                UltraMessageBox.Show("Por favor llene los campos requeridos antes de guardar", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _movimientodetalleDto = new movimientodetalleDto();
                    _movimientodetalleDto.i_IdAlmacen = cboAlmacen.Value.ToString();
                    _movimientodetalleDto.v_IdMovimientoDetalle = grdData.ActiveRow.Cells["v_IdMovimientoDetalle"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(_movimientodetalleDto);
                    if (grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null)
                        EliminarRecetas(grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value.ToString());
                    grdData.ActiveRow.Delete(false);

                }
            }
            else
            {
                if (grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value != null)
                    EliminarRecetas(grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value.ToString());
                grdData.ActiveRow.Delete(false);
            }
            btnCopiarFila.Enabled = false;
            MuestraColumnasReceta();
        }

        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            new PleaseWait(Location, "Por favor espere...");
            using (var frm = new frmBuscarProveedor("", ""))
            {
                frm.ShowDialog();
                if (frm._IdProveedor != null)
                {
                    txtProveedor.Text = frm._RazonSocial;
                    _movimientoDto.v_IdCliente = frm._IdProveedor;
                }
            }
        }

        private void txtProveedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (txtProveedor.Text == "") return;
            using (var frm = new frmBuscarProveedor(txtProveedor.Text, "Nombre"))
            {
                frm.ShowDialog();
                if (frm._IdProveedor == null) return;
                txtProveedor.Text = frm._RazonSocial;
                _movimientoDto.v_IdCliente = frm._IdProveedor;
            }
        }

        private void dtpFecha_ValueChanged(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            txtTipoCambio.Text = _objMovimientoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFecha.Value.Date);
            txtPeriodo.Text = dtpFecha.Value.Year.ToString();
            txtMes.Text = dtpFecha.Value.Month.ToString("00");
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.NotaIngreso, _movimientoDto.t_Fecha, dtpFecha.Value, _movimientoDto.v_Correlativo, 0);

            if (new CierreMensualBL().VerificarMesCerrado(txtPeriodo.Text, txtMes.Text, (int)ModulosSistema.Almacen) || Utilizado == "KARDEX")
            {
                btnGuardar.Visible = false;
                // this.Text = "Nota de Ingreso [MES CERRADO]";
                Text = Utilizado == "KARDEX" ? "Nota de Ingreso" : "Nota de Ingreso [MES CERRADO]";
                if (Utilizado == "KARDEX")
                {
                    BtnImprimir.Visible = false;
                    btnSalir.Visible = false;
                    btnAgregar.Visible = false;
                    btnEliminar.Visible = false;

                }
            }
            else
            {
                btnGuardar.Visible = true;
                Text = @"Nota de Ingreso";
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
                var x = _ListadoMovimientos.Find(p => p.Value1 == txtCorrelativo.Text);
                if (x != null)
                {
                    CargarCabecera(x.Value2);
                }
                else
                {
                    UltraMessageBox.Show("No se encontró el movimiento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ObtenerListadoMovimientos(txtPeriodo.Text, txtMes.Text);
                }
            }
        }

        private void grdData_AfterExitEditMode(object sender, EventArgs e)
        {
            try
            {
                OperationResult objOperationResult = new OperationResult();
                if (grdData.ActiveCell.Column.Key == "v_NroGuiaRemision")
                {
                    if (grdData.ActiveRow.Cells["v_NroGuiaRemision"].Value != null)
                    {
                        var nroGuia = grdData.ActiveRow.Cells["v_NroGuiaRemision"].Value.ToString();
                        if (nroGuia.Any(x => x.ToString() == "-"))
                        {
                            var serieCorrelativo = nroGuia.Split('-');
                            grdData.ActiveRow.Cells["v_NroGuiaRemision"].Value = int.Parse(serieCorrelativo[0]).ToString("0000") + "-" + int.Parse(serieCorrelativo[1]).ToString("00000000");
                        }
                    }
                }
                else if (grdData.ActiveCell.Column.Key == "v_NumeroDocumento")
                {
                    if (grdData.ActiveRow.Cells["v_NumeroDocumento"].Value != null)
                    {
                        var nroGuia = grdData.ActiveRow.Cells["v_NumeroDocumento"].Value.ToString();
                        if (nroGuia.Contains("-"))
                        {
                            var serieCorrelativo = nroGuia.Split('-');
                            int i;
                            var serie = int.TryParse(serieCorrelativo[0], out i) ? i.ToString("0000") : serieCorrelativo[0];
                            grdData.ActiveRow.Cells["v_NumeroDocumento"].Value = serie + "-" + int.Parse(serieCorrelativo[1]).ToString("00000000");
                        }
                    }
                }

                if (grdData.ActiveCell.Column.Key.Equals("d_Total"))
                {
                    CalcularTotal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        #region Barra de Navegación
        private void ObtenerListadoMovimientos(string pstrPeriodo, string pstrMes)
        {
            OperationResult objOperationResult = new OperationResult();
            _ListadoMovimientos = _objMovimientoBL.ObtenerListadoMovimientos(ref objOperationResult, pstrPeriodo, pstrMes, (int)Common.Resource.TipoDeMovimiento.NotadeIngreso);
            switch (strModo)
            {
                case "Edicion":
                    EdicionBarraNavegacion(false);
                    CargarCabecera(strIdmovimiento);
                    cboAlmacen.Enabled = false;
                    btnBuscarGuiaRemision.Enabled = false;
                    btnImportExcel.Visible = false;
                    btnActualizarCostos.Visible = true;
                    HabilitarLotesSerie(false);
                    break;

                case "Nuevo":
                    _Mode = "New";
                    LimpiarCabecera();
                    CargarDetalle("");
                    _movimientoDto = new movimientoDto();
                    btnNuevoMovimiento.Enabled = false;
                    EdicionBarraNavegacion(false);
                    txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.NotaIngreso, _movimientoDto.t_Fecha, dtpFecha.Value, _movimientoDto.v_Correlativo, 0);
                    cboMoneda.SelectedIndex = 2;
                    cboMotivo.SelectedIndex = 2;
                    txtTipoCambio.Text = _objMovimientoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFecha.Value.Date);
                    cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    btnImportExcel.Visible = true;
                    btnActualizarCostos.Visible = false;
                    HabilitarLotesSerie(true);
                    break;

                case "Guardado":
                    _MaxV = _ListadoMovimientos.Count() - 1;
                    _ActV = _MaxV;
                    if (string.IsNullOrEmpty(strIdmovimiento))
                    {
                        CargarCabecera(_ListadoMovimientos[_MaxV].Value2);
                    }
                    else
                    {
                        CargarCabecera(strIdmovimiento);
                    }
                    btnNuevoMovimiento.Enabled = true;
                    HabilitarLotesSerie(false);
                    break;

                case "Consulta":
                    if (_ListadoMovimientos.Count != 0)
                    {
                        _MaxV = _ListadoMovimientos.Count() - 1;
                        _ActV = _MaxV;
                        txtCorrelativo.Text = (int.Parse(_ListadoMovimientos[_MaxV].Value1)).ToString("00000000");
                        CargarCabecera(_ListadoMovimientos[_MaxV].Value2);
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
                        _movimientoDto = new movimientoDto();
                        btnNuevoMovimiento.Enabled = false;
                        txtTipoCambio.Text = _objMovimientoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFecha.Value.Date);
                        EdicionBarraNavegacion(false);
                        //txtMes.Enabled = true;
                    }
                    HabilitarLotesSerie(false);
                    break;
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_ListadoMovimientos.Count() > 0)
            {
                if (_MaxV == 0) CargarCabecera(_ListadoMovimientos[0].Value2);

                if (_ActV > 0 && _ActV <= _MaxV)
                {
                    _ActV = _ActV - 1;
                    txtCorrelativo.Text = _ListadoMovimientos[_ActV].Value1;
                    CargarCabecera(_ListadoMovimientos[_ActV].Value2);
                }
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_ActV >= 0 && _ActV < _MaxV)
            {
                _ActV = _ActV + 1;
                txtCorrelativo.Text = _ListadoMovimientos[_ActV].Value1;
                CargarCabecera(_ListadoMovimientos[_ActV].Value2);
            }
        }

        private void btnNuevoMovimiento_Click(object sender, EventArgs e)
        {
            var objOperationResult = new OperationResult();
            LimpiarCabecera();
            CargarDetalle("");
            txtCorrelativo.Text = (int.Parse(_ListadoMovimientos[_MaxV].Value1) + 1).ToString("00000000");
            _Mode = "New";
            _movimientoDto = new movimientoDto();
            EdicionBarraNavegacion(false);
            txtTipoCambio.Text = _objMovimientoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFecha.Value.Date);
            cboAlmacen.Enabled = true;
            cboAlmacen.Value = Globals.ClientSession.i_IdAlmacenPredeterminado ?? -1;
            cboMotivo.SelectedIndex = 2;
            cboMoneda.Value = 1;
            btnAgregar_Click(sender, e);
            HabilitarLotesSerie(true);
        }

        private void txtCorrelativo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtCorrelativo.Text = txtCorrelativo.Text == "" ? "" : int.Parse(txtCorrelativo.Text).ToString("00000000");
                if (txtCorrelativo.Text != "")
                {
                    var x = _ListadoMovimientos.Find(p => p.Value1 == txtCorrelativo.Text);
                    if (x != null)
                    {
                        CargarCabecera(x.Value2);
                    }
                    else
                    {
                        UltraMessageBox.Show("No se encontró el movimiento", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        ObtenerListadoMovimientos(txtPeriodo.Text, txtMes.Text);
                    }
                }
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
                            ObtenerListadoMovimientos(txtPeriodo.Text, txtMes.Text);
                        }
                        else if (strModo == "Guardado")
                        {
                            strModo = "Consulta";
                            ObtenerListadoMovimientos(txtPeriodo.Text, txtMes.Text);
                        }
                        else
                        {
                            ObtenerListadoMovimientos(txtPeriodo.Text, txtMes.Text);
                        }
                    }
                    else
                    {
                        UltraMessageBox.Show("Mes inválido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                }
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
                        ObtenerListadoMovimientos(txtPeriodo.Text, txtMes.Text);
                    }
                    else if (strModo == "Guardado")
                    {
                        strModo = "Consulta";
                        ObtenerListadoMovimientos(txtPeriodo.Text, txtMes.Text);
                    }
                    else
                    {
                        ObtenerListadoMovimientos(txtPeriodo.Text, txtMes.Text);
                    }
                }
                else
                {
                    UltraMessageBox.Show("Mes inválido", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }
        #endregion

        #region Clases/Validaciones
        private void EdicionBarraNavegacion(bool ON_OFF)
        {
            txtCorrelativo.Enabled = ON_OFF;
            btnNuevoMovimiento.Enabled = ON_OFF;
            btnAnterior.Enabled = ON_OFF;
            btnSiguiente.Enabled = ON_OFF;
        }

        private void LimpiarCabecera()
        {
            cboAlmacen.Value = "-1";
            cboMoneda.Value = "-1";
            cboMotivo.Value = "-1";
            dtpFecha.Value = DateTime.Parse(DateTime.Today.Day + "/" + DateTime.Today.Month + "/" + Globals.ClientSession.i_Periodo);
            txtTipoCambio.Text = string.Empty;
            txtGlosa.Clear();
            txtProveedor.Clear();
            txtTotal.Clear();
            txtCantidad.Clear();
        }

        private void CargarCabecera(string idmovimiento)
        {
            OperationResult objOperationResult = new OperationResult();
            _movimientoDto = new movimientoDto();
            _movimientoDto = _objMovimientoBL.ObtenerMovimientoCabecera(ref objOperationResult, idmovimiento);
            _pstrIdMovimiento_Nuevo = idmovimiento;
            if (_movimientoDto != null)
            {
                _Mode = "Edit";
                cboAlmacen.Value = _movimientoDto.i_IdAlmacenOrigen.ToString();
                cboMoneda.Value = _movimientoDto.i_IdMoneda.ToString();
                cboMotivo.Value = _movimientoDto.i_IdTipoMotivo.ToString();
                txtGlosa.Text = _movimientoDto.v_Glosa;
                dtpFecha.Value = _movimientoDto.t_Fecha.Value;
                txtTipoCambio.Text = _movimientoDto.d_TipoCambio.ToString();
                txtProveedor.Text = _movimientoDto.v_NombreCliente;
                cboEstablecimiento.Value = _movimientoDto.i_IdEstablecimiento != null ? _movimientoDto.i_IdEstablecimiento.Value.ToString() : "-1";
                txtCorrelativo.Text = _movimientoDto.v_Correlativo;
                txtPeriodo.Text = _movimientoDto.v_Periodo;
                txtMes.Text = int.Parse(_movimientoDto.v_Mes).ToString("00");
                txtTotal.Text = _movimientoDto.d_TotalPrecio.Value.ToString("0.00");
                txtCantidad.Text = _movimientoDto.d_TotalCantidad.Value.ToString(FormatoCantidad);
                chkDevolucion.Checked = _movimientoDto.i_EsDevolucion == 1;
                btnBuscarGuiaRemision.Text = _movimientoDto.v_NroGuiaVenta;
                CargarDetalle(_movimientoDto.v_IdMovimiento);
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar el movimiento", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void CargarDetalle(string pstringIdMovimiento)
        {
            OperationResult objOperationResult = new OperationResult();
            grdData.DataSource = _objMovimientoBL.ObtenerMovimientoDetallesIngresos(ref objOperationResult, pstringIdMovimiento);
            if (objOperationResult.Success == 1)
            {
                var dsRecetas = _objMovimientoBL.ObtenerRecetasFinales(ref objOperationResult, pstringIdMovimiento);

                if (objOperationResult.Success == 1)
                {
                    ultraGrid1.DataSource = dsRecetas;
                    ultraGrid1.Rows.ToList().ForEach(fila =>
                    {
                        fila.Cells["i_RegistroTipo"].Value = "NoTemporal";
                        var filaProductoTerminado = grdData.Rows.FirstOrDefault(f => f.Cells["v_IdProductoDetalle"].Value.ToString().Equals(fila.Cells["v_IdProdTerminado"].Value.ToString()));
                        if (filaProductoTerminado != null)
                        {
                            var cantPt = decimal.Parse(filaProductoTerminado.Cells["d_Cantidad"].Value.ToString());
                            if (cantPt > 0)
                            {
                                var cantInsumo = decimal.Parse(fila.Cells["d_Cantidad"].Value.ToString());
                                fila.Cells["_Cantidad"].Value = Utils.Windows.DevuelveValorRedondeado(cantInsumo / cantPt, 2);
                                fila.Cells["_Multiplicador"].Value = string.Format("x{0}",
                                    filaProductoTerminado.Cells["d_Cantidad"].Value);
                            }
                        }
                    });
                    MuestraColumnasReceta();
                }
                else
                {
                    MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage,
                        @"Error al obtener las recetas.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage,
                    @"Error al obtener el detalle del ingreso.", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            Utils.Windows.LoadUltraComboList(ucTipoDocumento, "Value2", "Id", _objDocumentoBL.ObtenDocumentosParaComboGridAll(ref objOperationResult), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucUnidadMedida, "Value1", "Id", new DatahierarchyBL().GetDataHierarchiesForComboGrid(ref objOperationResult, 17, null), DropDownListAction.Select);
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
                                _movimientodetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _movimientodetalleDto.v_NroGuiaRemision = Fila.Cells["v_NroGuiaRemision"].Value == null ? null : Fila.Cells["v_NroGuiaRemision"].Value.ToString();
                                _movimientodetalleDto.i_IdTipoDocumento = Fila.Cells["i_IdTipoDocumento"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoDocumento"].Value.ToString());
                                _movimientodetalleDto.v_NumeroDocumento = Fila.Cells["v_NumeroDocumento"].Value == null ? null : Fila.Cells["v_NumeroDocumento"].Value.ToString();
                                _movimientodetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _movimientodetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _movimientodetalleDto.i_IdUnidad = Fila.Cells["i_IdUnidad"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidad"].Value.ToString());
                                _movimientodetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                _movimientodetalleDto.d_Total = Fila.Cells["d_Total"].Value == null ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Total"].Value.ToString()), 2);
                                _movimientodetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                _movimientodetalleDto.v_NroSerie = Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim();
                                _movimientodetalleDto.v_NroLote = Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim();

                                _movimientodetalleDto.t_FechaCaducidad = Fila.Cells["t_FechaCaducidad"].Value == null ? (DateTime?)null : DateTime.Parse(Fila.Cells["t_FechaCaducidad"].Value.ToString());
                                _movimientodetalleDto.t_FechaFabricacion = Fila.Cells["t_FechaFabricacion"].Value == null ? (DateTime?)null : DateTime.Parse(Fila.Cells["t_FechaFabricacion"].Value.ToString());
                                _movimientodetalleDto.i_EsProductoFinal = Fila.Cells["i_EsProductoFinal"].Value == null ? 0 : int.Parse(Fila.Cells["i_EsProductoFinal"].Value.ToString());
                                _movimientodetalleDto.CodigoProducto = Fila.Cells["v_CodigoInterno"].Value as string;
                                _movimientodetalleDto.v_NroOrdenProduccion = Fila.Cells["v_NroOrdenProduccion"].Value == null || Fila.Cells["v_NroOrdenProduccion"].Value == "" ? null : Fila.Cells["v_NroOrdenProduccion"].Value.ToString();
                                _TempDetalle_AgregarDto.Add(_movimientodetalleDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _movimientodetalleDto = new movimientodetalleDto();
                                _movimientodetalleDto.v_IdMovimientoDetalle = Fila.Cells["v_IdMovimientoDetalle"].Value == null ? null : Fila.Cells["v_IdMovimientoDetalle"].Value.ToString();
                                _movimientodetalleDto.v_IdMovimiento = Fila.Cells["v_IdMovimiento"].Value == null ? null : Fila.Cells["v_IdMovimiento"].Value.ToString();
                                _movimientodetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _movimientodetalleDto.v_NroGuiaRemision = Fila.Cells["v_NroGuiaRemision"].Value == null ? null : Fila.Cells["v_NroGuiaRemision"].Value.ToString();
                                _movimientodetalleDto.i_IdTipoDocumento = Fila.Cells["i_IdTipoDocumento"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdTipoDocumento"].Value.ToString());
                                _movimientodetalleDto.v_NumeroDocumento = Fila.Cells["v_NumeroDocumento"].Value == null ? null : Fila.Cells["v_NumeroDocumento"].Value.ToString();
                                _movimientodetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _movimientodetalleDto.i_IdUnidad = Fila.Cells["i_IdUnidad"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidad"].Value.ToString());
                                _movimientodetalleDto.d_Precio = Fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Precio"].Value.ToString());
                                _movimientodetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _movimientodetalleDto.d_Total = Fila.Cells["d_Total"].Value == null ? 0 : Utils.Windows.DevuelveValorRedondeado(decimal.Parse(Fila.Cells["d_Total"].Value.ToString()), 2);
                                _movimientodetalleDto.v_NroPedido = Fila.Cells["v_NroPedido"].Value == null ? null : Fila.Cells["v_NroPedido"].Value.ToString().Trim();
                                _movimientodetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _movimientodetalleDto.CodigoProducto = Fila.Cells["v_CodigoInterno"].Value as string;
                                _movimientodetalleDto.v_NroSerie = Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim();
                                _movimientodetalleDto.v_NroLote = Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim();

                                _movimientodetalleDto.t_FechaCaducidad = Fila.Cells["t_FechaCaducidad"].Value == null ? (DateTime?)null : DateTime.Parse(Fila.Cells["t_FechaCaducidad"].Value.ToString());
                                _movimientodetalleDto.t_FechaFabricacion = Fila.Cells["t_FechaFabricacion"].Value == null ? (DateTime?)null : DateTime.Parse(Fila.Cells["t_FechaFabricacion"].Value.ToString());
                                _movimientodetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _movimientodetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _movimientodetalleDto.i_EsProductoFinal = Fila.Cells["i_EsProductoFinal"].Value == null ? 0 : int.Parse(Fila.Cells["i_EsProductoFinal"].Value.ToString());

                                _movimientodetalleDto.v_NroOrdenProduccion = Fila.Cells["v_NroOrdenProduccion"].Value == null || Fila.Cells["v_NroOrdenProduccion"].Value == "" ? null : Fila.Cells["v_NroOrdenProduccion"].Value.ToString();
                                _TempDetalle_ModificarDto.Add(_movimientodetalleDto);
                            }
                            break;
                    }
                }
            }

        }

        private void LlenarTemporalesRecetasFinales()
        {
            if (ultraGrid1.Rows.Any())
            {
                foreach (UltraGridRow Fila in ultraGrid1.Rows)
                {
                    switch (Fila.Cells["i_RegistroTipo"].Value.ToString())
                    {
                        case "Temporal":
                            if (Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                var movimientodetallerecetafinalDto = (movimientodetallerecetafinalDto)Fila.ListObject;
                                _TempDetalleReceta_AgregarDto.Add(movimientodetallerecetafinalDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                var movimientodetallerecetafinalDto = (movimientodetallerecetafinalDto)Fila.ListObject;
                                _TempDetalleReceta_ModificarDto.Add(movimientodetallerecetafinalDto);
                            }
                            break;
                    }
                }
            }

        }

        private void CalcularTotal()
        {
            decimal totalPrecio = 0, totalCantidad = 0;
            foreach (var fila in grdData.Rows)
            {
                if (fila.Cells["d_Precio"].Value == null) fila.Cells["d_Precio"].Value = "0";
                if (fila.Cells["d_Cantidad"].Value == null) fila.Cells["d_Cantidad"].Value = "0";
                if (fila.Cells["d_Total"].Value == null) fila.Cells["d_Total"].Value = "0";

                var precio = decimal.Parse(fila.Cells["d_Precio"].Value.ToString());
                var cant = decimal.Parse(fila.Cells["d_Cantidad"].Value.ToString());
                var tot = Utils.Windows.DevuelveValorRedondeado(decimal.Parse(fila.Cells["d_Total"].Value.ToString()), 2);

                totalPrecio += tot;
                totalCantidad += cant;

                ///if(precio == 0 || cant ==0) continue;

                if (fila.Cells["i_IdUnidad"].Value != null)
                {
                    if (fila.Cells["v_IdProductoDetalle"].Value != null && fila.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000" && fila.Cells["i_IdUnidad"].Value.ToString() != "-1" && cant != 0)
                    {
                        decimal TotalEmpaque = 0;
                        decimal Empaque = decimal.Parse(fila.Cells["Empaque"].Value.ToString());
                        string Producto = fila.Cells["v_IdProductoDetalle"].Value.ToString();
                        decimal Cantidad = decimal.Parse(fila.Cells["d_Cantidad"].Value.ToString());
                        int UM = int.Parse(fila.Cells["i_IdUnidad"].Value.ToString());
                        int UMProducto = int.Parse(fila.Cells["i_IdUnidadMedidaProducto"].Value.ToString());

                        GridKeyValueDTO _UMProducto = ((List<GridKeyValueDTO>)ucUnidadMedida.DataSource).FirstOrDefault(p => p.Id == UMProducto.ToString());
                        GridKeyValueDTO _UM = ((List<GridKeyValueDTO>)ucUnidadMedida.DataSource).FirstOrDefault(p => p.Id == UM.ToString());

                        if (_UM != null)
                        {
                            switch (_UM.Value1)
                            {
                                case "CAJA":
                                    decimal caja = Empaque * (!string.IsNullOrEmpty(_UMProducto.Value2) ? decimal.Parse(_UMProducto.Value2) : 0);
                                    TotalEmpaque = Cantidad * caja;
                                    break;

                                default:
                                    TotalEmpaque = Cantidad * (!string.IsNullOrEmpty(_UM.Value2) ? decimal.Parse(_UM.Value2) : 0);
                                    break;
                            }
                        }
                        fila.Cells["d_CantidadEmpaque"].Value = TotalEmpaque.ToString();
                    }
                    else
                        fila.Cells["d_CantidadEmpaque"].Value = "0";
                }
            }

            txtCantidad.Text = totalCantidad.ToString(FormatoCantidad);
            txtTotal.Text = totalPrecio.ToString("0.00");
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
                FormatoPrecio = "nnnnnnnnnn";
            }

            _Cantidad.MaskInput = "-" + FormatoCantidad;
            _Precio.MaskInput = "-" + FormatoPrecio;
        }

        public void RecibirItems(List<UltraGridRow> filas)
        {
            var j = false;
            foreach (var fila in filas)
            {
                //bool repetido;
                //if (grdData.Rows.Count(p => p.Cells["v_IdProductoDetalle"].Value != null && p.Cells["v_IdProductoDetalle"].Value.ToString() == fila.Cells["v_IdProductoDetalle"].Value.ToString()) != 0)
                //{
                //    UltraMessageBox.Show("El producto '" + fila.Cells["v_Descripcion"].Value + "' ya se encuentra en el detalle", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    repetido = true;
                //}
                //else
                //{
                //    repetido = false;
                //}

                //if (repetido == false)
                //{
                if (j == false)
                {
                    grdData.ActiveRow.Cells["v_NombreProducto"].Value = fila.Cells["v_Descripcion"].Value.ToString();
                    grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = fila.Cells["v_IdProductoDetalle"].Value.ToString();
                    grdData.ActiveRow.Cells["v_CodigoInterno"].Value = fila.Cells["v_CodInterno"].Value.ToString();
                    grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                    grdData.ActiveRow.Cells["i_IdUnidad"].Value = fila.Cells["i_IdUnidadMedida"].Value == null ? null : fila.Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = fila.Cells["i_IdUnidadMedida"].Value == null ? null : fila.Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["d_Precio"].Value = fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(fila.Cells["d_Precio"].Value.ToString());
                    grdData.ActiveRow.Cells["d_Cantidad"].Value = fila.Cells["_Cantidad"].Value == null ? 0 : decimal.Parse(fila.Cells["_Cantidad"].Value.ToString());
                    grdData.ActiveRow.Cells["Empaque"].Value = fila.Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(fila.Cells["d_Empaque"].Value.ToString());
                    grdData.ActiveRow.Cells["UMEmpaque"].Value = fila.Cells["EmpaqueUnidadMedida"].Value == null ? null : fila.Cells["EmpaqueUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["d_Precio"].Value = fila.Cells["d_Precio"].Value;
                    grdData.ActiveRow.Cells["i_EsProductoFinal"].Value = fila.Cells["EsProductoFinal"].Value != null && Convert.ToBoolean(fila.Cells["EsProductoFinal"].Value.ToString()) ? "1" : "0";
                    grdData.ActiveRow.Cells["i_SolicitarNroSerieIngreso"].Value = int.Parse(fila.Cells["i_SolicitarNroSerieIngreso"].Value.ToString());
                    grdData.ActiveRow.Cells["i_SolicitarNroLoteIngreso"].Value = int.Parse(fila.Cells["i_SolicitarNroLoteIngreso"].Value.ToString());
                    grdData.ActiveRow.Cells["i_SolicitaOrdenProduccionIngreso"].Value = int.Parse(fila.Cells["i_SolicitaOrdenProduccionIngreso"].Value.ToString());
                    grdData.ActiveRow.Cells["t_FechaCaducidad"].Value = DateTime.Parse(Constants.FechaNula);
                    grdData.ActiveRow.Cells["t_FechaFabricacion"].Value = DateTime.Parse(Constants.FechaNula);
                    FijarRecetas(fila.Cells["v_IdProductoDetalle"].Value.ToString(), decimal.Parse(grdData.ActiveRow.Cells["d_Cantidad"].Value.ToString()));
                    j = true;
                }
                else
                {
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Agregado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_IdUnidad"].Value = "-1";
                    row.Cells["i_IdTipoDocumento"].Value = "-1";
                    row.Activate();
                    grdData.ActiveRow.Cells["v_NombreProducto"].Value = fila.Cells["v_Descripcion"].Value.ToString();
                    grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = fila.Cells["v_IdProductoDetalle"].Value.ToString();
                    grdData.ActiveRow.Cells["v_CodigoInterno"].Value = fila.Cells["v_CodInterno"].Value.ToString();
                    grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                    grdData.ActiveRow.Cells["i_IdUnidad"].Value = fila.Cells["i_IdUnidadMedida"].Value == null ? null : fila.Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["d_Precio"].Value = fila.Cells["d_Precio"].Value == null ? 0 : decimal.Parse(fila.Cells["d_Precio"].Value.ToString());
                    grdData.ActiveRow.Cells["d_Cantidad"].Value = fila.Cells["_Cantidad"].Value == null ? 0 : decimal.Parse(fila.Cells["_Cantidad"].Value.ToString());
                    grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = fila.Cells["i_IdUnidadMedida"].Value == null ? null : fila.Cells["i_IdUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["Empaque"].Value = fila.Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(fila.Cells["d_Empaque"].Value.ToString());
                    grdData.ActiveRow.Cells["UMEmpaque"].Value = fila.Cells["EmpaqueUnidadMedida"].Value == null ? null : fila.Cells["EmpaqueUnidadMedida"].Value.ToString();
                    grdData.ActiveRow.Cells["d_Precio"].Value = fila.Cells["d_Precio"].Value;
                    grdData.ActiveRow.Cells["i_EsProductoFinal"].Value = fila.Cells["EsProductoFinal"].Value != null && Convert.ToBoolean(fila.Cells["EsProductoFinal"].Value.ToString()) ? "1" : "0";
                    grdData.ActiveRow.Cells["i_SolicitarNroSerieIngreso"].Value = int.Parse(fila.Cells["i_SolicitarNroSerieIngreso"].Value.ToString());
                    grdData.ActiveRow.Cells["i_SolicitarNroLoteIngreso"].Value = int.Parse(fila.Cells["i_SolicitarNroLoteIngreso"].Value.ToString());
                    grdData.ActiveRow.Cells["i_SolicitaOrdenProduccionIngreso"].Value = int.Parse(fila.Cells["i_SolicitaOrdenProduccionIngreso"].Value.ToString());
                    grdData.ActiveRow.Cells["t_FechaCaducidad"].Value = DateTime.Parse(Constants.FechaNula);
                    grdData.ActiveRow.Cells["t_FechaFabricacion"].Value = DateTime.Parse(Constants.FechaNula);
                    FijarRecetas(fila.Cells["v_IdProductoDetalle"].Value.ToString(), decimal.Parse(grdData.ActiveRow.Cells["d_Cantidad"].Value.ToString()));
                }
                //    }
            }
            CalcularTotal();
        }

        public bool Validacion()
        {
            try
            {
                var FilasSinUM = grdData.Rows.Where(p => p.Cells["i_IdUnidad"].Value != null && p.Cells["i_IdUnidad"].Value.ToString() == "-1").ToList();

                if (FilasSinUM.Any())
                {
                    UltraMessageBox.Show("Por favor seleccione una Unidad de Medida", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    UltraGridRow Fila = FilasSinUM.FirstOrDefault();
                    grdData.Focus();
                    Fila.Activate();
                    Fila.Cells["i_IdUnidad"].Activate();
                    grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown, true, false);
                    return false;
                }

                var FilasSinSerieRequeridas = grdData.Rows.Where(p => p.Cells["i_SolicitarNroSerieIngreso"].Value != null && p.Cells["i_SolicitarNroSerieIngreso"].Value.ToString() == "1" && (p.Cells["v_NroSerie"].Value == null || p.Cells["v_NroSerie"].Value.ToString() == "")).ToList();

                if (FilasSinSerieRequeridas.Any())
                {
                    UltraMessageBox.Show("Por favor registre el Nro. de Serie para el producto.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    UltraGridRow Fila = FilasSinSerieRequeridas.FirstOrDefault();
                    grdData.Focus();
                    Fila.Activate();
                    Fila.Cells["v_NroSerie"].Activate();
                    grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown, true, false);
                    return false;
                }




                var FilasSinLote = grdData.Rows.Where(p => p.Cells["i_SolicitarNroLoteIngreso"].Value != null && p.Cells["i_SolicitarNroLoteIngreso"].Value.ToString() == "1" && (p.Cells["v_NroLote"].Value == null || p.Cells["v_NroLote"].Value.ToString() == "")).ToList();

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


                var FilaDebenTenerOrdenProduccion = grdData.Rows.Where(p => p.Cells["i_SolicitaOrdenProduccionIngreso"].Value != null && p.Cells["i_SolicitaOrdenProduccionIngreso"].Value.ToString() == "1" && (p.Cells["v_NroOrdenProduccion"].Value == null || p.Cells["v_NroOrdenProduccion"].Value.ToString().Trim() == "")).ToList();

                if (FilaDebenTenerOrdenProduccion.Any())
                {
                    UltraMessageBox.Show("Por favor registre el Nro. de Orden de Producción para el producto.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    UltraGridRow Fila = FilaDebenTenerOrdenProduccion.FirstOrDefault();
                    grdData.Focus();
                    Fila.Activate();
                    Fila.Cells["v_NroOrdenProduccion"].Activate();
                    grdData.PerformAction(UltraGridAction.EnterEditModeAndDropdown, true, false);
                    return false;
                }


                var FilasConOrdenProduccionSinSerRequeridas = grdData.Rows.Where(p => p.Cells["i_SolicitaOrdenProduccionIngreso"].Value != null && p.Cells["i_SolicitaOrdenProduccionIngreso"].Value.ToString() == "0" && (p.Cells["v_NroOrdenProduccion"].Value != null && p.Cells["v_NroOrdenProduccion"].Value != "")).ToList();

                if (FilasConOrdenProduccionSinSerRequeridas.Any())
                {
                    UltraMessageBox.Show("No es necesario registrar el Nro. de Orden de Producción para el producto", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    UltraGridRow Fila = FilasConOrdenProduccionSinSerRequeridas.FirstOrDefault();
                    grdData.Focus();
                    Fila.Activate();
                    Fila.Cells["v_NroOrdenProduccion"].Activate();
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
            catch (Exception ex)
            {
                UltraMessageBox.Show(ex.Message);
                return false;
            }
        }
        #endregion

        #region Eventos de la Grilla
        private void grdData_KeyDown(object sender, KeyEventArgs e)
        {
            if (grdData.ActiveCell == null) return;

            if (this.grdData.ActiveCell.Column.Key != "i_IdTipoDocumento" && grdData.ActiveCell.Column.Key != "i_IdUnidad")
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
            e.Layout.Bands[0].Columns["i_IdUnidad"].EditorComponent = ucUnidadMedida;
            e.Layout.Bands[0].Columns["i_IdUnidad"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["i_IdTipoDocumento"].EditorComponent = ucTipoDocumento;
            e.Layout.Bands[0].Columns["i_IdTipoDocumento"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }

        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            if (int.Parse(cboAlmacen.Value.ToString()) == -1)
            {
                UltraMessageBox.Show("Porfavor seleccione un almacén antes de buscar un producto",
                    "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (e.Cell.Column.Key != "v_CodigoInterno") return;
            if (grdData.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() != "Temporal") return;
            var codProductoIngresado = e.Cell.Text.Trim();
            var objProducto = new ComprasBL();
            var prod = objProducto.DevolverArticuloPorCodInterno(codProductoIngresado);
            if (!string.IsNullOrWhiteSpace(codProductoIngresado) && prod != null)
            {
                grdData.ActiveRow.Cells["v_NombreProducto"].Value = prod.v_Descripcion;
                grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = prod.v_IdProductoDetalle;
                grdData.ActiveRow.Cells["v_CodigoInterno"].Value = codProductoIngresado;
                grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                grdData.ActiveRow.Cells["i_IdUnidad"].Value = prod.i_IdUnidadMedida;
                grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = prod.i_IdUnidadMedida;
                grdData.ActiveRow.Cells["Empaque"].Value = prod.d_Empaque;
                grdData.ActiveRow.Cells["UMEmpaque"].Value = prod.EmpaqueUnidadMedida;
                grdData.ActiveRow.Cells["d_Precio"].Value = prod.d_Costo ?? 0;
                grdData.ActiveRow.Cells["i_EsProductoFinal"].Value = prod.EsProductoFinal ? 1 : 0;
                grdData.ActiveRow.Cells["i_SolicitarNroSerieIngreso"].Value = prod.i_SolicitarNroSerieIngreso;
                grdData.ActiveRow.Cells["i_SolicitarNroLoteIngreso"].Value = prod.i_SolicitarNroLoteIngreso;
                grdData.ActiveRow.Cells["i_SolicitaOrdenProduccionIngreso"].Value = prod.i_SolicitaOrdenProduccionIngreso;
                grdData.ActiveRow.Cells["t_FechaCaducidad"].Value = Constants.FechaNula;
                FijarRecetas(prod.v_IdProductoDetalle, decimal.Parse(grdData.ActiveRow.Cells["d_Cantidad"].Value.ToString()));
            }
            else
                using (var frm = new frmBuscarProducto(int.Parse(cboAlmacen.Value.ToString()), null, null,
                    grdData.ActiveRow.Cells["v_CodigoInterno"].Text ?? string.Empty))
                {
                    frm.ShowDialog();
                    if (frm._NombreProducto == null) return;
                    grdData.ActiveRow.Cells["v_NombreProducto"].Value = frm._NombreProducto;
                    grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value = frm._IdProducto;
                    grdData.ActiveRow.Cells["v_CodigoInterno"].Value = frm._CodigoInternoProducto;
                    grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";
                    grdData.ActiveRow.Cells["i_IdUnidad"].Value = frm._UnidadMedidaEmpaque;
                    grdData.ActiveRow.Cells["i_IdUnidadMedidaProducto"].Value = frm._UnidadMedidaEmpaque;
                    grdData.ActiveRow.Cells["Empaque"].Value = frm._Empaque.ToString(CultureInfo.InvariantCulture);
                    grdData.ActiveRow.Cells["UMEmpaque"].Value = frm._UnidadMedida;
                    grdData.ActiveRow.Cells["d_Precio"].Value = frm._PrecioUnitario;
                    grdData.ActiveRow.Cells["i_EsProductoFinal"].Value = frm._EsProductoFinal ? "1" : "0";


                    FijarRecetas(frm._IdProducto, decimal.Parse(grdData.ActiveRow.Cells["d_Cantidad"].Value.ToString()));
                }

            grdData.ActiveRow.Cells["d_Cantidad"].Activate();
        }

        private void grdData_CellChange(object sender, CellEventArgs e)
        {
            grdData.Rows[e.Cell.Row.Index].Cells["i_RegistroEstado"].Value = "Modificado";
            if (e.Cell.Column.Key == "d_Cantidad")
            {
                try
                {
                    e.Cell.Value = e.Cell.EditorResolved.Value;
                    var filas = ultraGrid1.Rows.Where(f => f.Cells["v_IdProdTerminado"].Value.ToString().Equals(grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value.ToString()));
                    filas.ToList().ForEach(fila =>
                    {
                        fila.Cells["_Multiplicador"].Value = string.Format("x{0}", e.Cell.Value);
                        var cantidadInsumo = fila.Cells["_Cantidad"].Value != null ? decimal.Parse(fila.Cells["_Cantidad"].Value.ToString()) : 0;
                        var cantidadPt = e.Cell.Value != null && !string.IsNullOrEmpty(e.Cell.Value.ToString()) ? decimal.Parse(e.Cell.Value.ToString()) : 0;
                        fila.Cells["d_Cantidad"].Value = cantidadInsumo * cantidadPt;
                        fila.Cells["i_RegistroEstado"].Value = "Modificado";
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        private void grdData_AfterCellActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow.Cells["d_Cantidad"].Value == null || grdData.ActiveRow.Cells["d_Precio"].Value == null) return;
            if (decimal.Parse(grdData.ActiveRow.Cells["d_Cantidad"].Value.ToString()) == 0 || decimal.Parse(grdData.ActiveRow.Cells["d_Precio"].Value.ToString()) == 0) return;
            var cantidad = decimal.Parse(grdData.ActiveRow.Cells["d_Cantidad"].Value.ToString());
            var precio = decimal.Parse(grdData.ActiveRow.Cells["d_Precio"].Value.ToString());
            var total = Utils.Windows.DevuelveValorRedondeado(cantidad * precio, 2);
            grdData.ActiveRow.Cells["d_Total"].Value = total;
            CalcularTotal();
        }

        private void grdData_MouseDown(object sender, MouseEventArgs e)
        {
            if (_movimientoDto.v_OrigenTipo == null)
            {
                var point = new Point(e.X, e.Y);
                Infragistics.Win.UIElement uiElement = ((UltraGridBase)sender).DisplayLayout.UIElement.ElementFromPoint(point);

                if (uiElement == null || uiElement.Parent == null) return;
                var row = (UltraGridRow)uiElement.GetContext(typeof(UltraGridRow));
                btnEliminar.Enabled = row != null;
            }
        }

        private void grdData_BeforeExitEditMode(object sender, BeforeExitEditModeEventArgs e)
        {
            try
            {
                if (e.CancellingEditOperation) return;
                if (grdData.ActiveCell.Column.Key == "d_Cantidad" || grdData.ActiveCell.Column.Key == "d_Precio")
                {
                    if (grdData.ActiveRow.Cells["d_Cantidad"].Value == null ||
                        decimal.Parse(grdData.ActiveRow.Cells["d_Cantidad"].Value.ToString()) == 0)
                    {
                        grdData.ActiveRow.Cells["d_CantidadEmpaque"].SetValue(0M, false);
                        return;
                    }

                    var cantidad = decimal.Parse(grdData.ActiveRow.Cells["d_Cantidad"].Value.ToString());
                    var precio = decimal.Parse(grdData.ActiveRow.Cells["d_Precio"].Value.ToString());
                    var total = cantidad * precio;
                    grdData.ActiveRow.Cells["d_Total"].Value = total;
                    CalcularTotal();
                }
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
                if (this.grdData.ActiveCell.Column.Key == "d_Cantidad")
                {
                    UltraGridCell Celda;
                    Celda = grdData.ActiveCell;
                    Utils.Windows.NumeroDecimalCelda(Celda, e);
                }

                else if (this.grdData.ActiveCell.Column.Key == "d_Precio")
                {
                    UltraGridCell Celda;
                    Celda = grdData.ActiveCell;
                    Utils.Windows.NumeroDecimalCelda(Celda, e);
                }

                else if (this.grdData.ActiveCell.Column.Key == "v_NroGuiaRemision")
                {
                    UltraGridCell Celda;
                    Celda = grdData.ActiveCell;
                    Utils.Windows.NumeroDocumentoCelda(Celda, e);
                }
            }
        }

        private void MuestraColumnasReceta()
        {
            try
            {
                var hayProductoFinal = grdData.Rows.Any(p => p.Cells["i_EsProductoFinal"].Value != null &&
                            p.Cells["i_EsProductoFinal"].Value.ToString() == "1");
                grdData.DisplayLayout.Bands[0].Columns["_Receta"].Hidden = !hayProductoFinal;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            var fila = e.Row;
            var filaProductoFinal = fila.Cells["i_EsProductoFinal"].Value != null &&
                    fila.Cells["i_EsProductoFinal"].Value.ToString() == "1";
            fila.Cells["_Receta"].Activation = filaProductoFinal ? Activation.AllowEdit : Activation.Disabled;


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
        #endregion

        private void grdData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (grdData.ActiveRow == null || e.Cell.Value == null) return;
            if (e.Cell.Column.Key != "d_Cantidad" && e.Cell.Column.Key != "d_Precio") return;
            var fila = grdData.ActiveRow;
            if (fila.Cells["d_Precio"].Value == null | fila.Cells["d_Cantidad"].Value == null) return;
            fila.Cells["d_Total"].Value = (decimal.Parse(fila.Cells["d_Precio"].Value.ToString()) * decimal.Parse(fila.Cells["d_Cantidad"].Value.ToString())).ToString(CultureInfo.InvariantCulture);
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            //List<string> IdVentas = grdData.Rows.Select(p => p.Cells["v_IdVenta"].Value.ToString()).Distinct().ToList();
            _IdMovimientoss = _pstrIdMovimiento_Nuevo;
            var frm = new Reportes.Almacen.frmDocumentoNotaIngresoAlmacen(_IdMovimientoss);
            frm.Show();
        }

        private void btnBuscarGuiaRemision_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            var g = btnBuscarGuiaRemision.Text.Trim().Split('-');
            int nro;
            if (g.Length == 2 && int.TryParse(g[1], out nro))
            {
                GetGuiaRemision(string.Format("{0}-{1:0000}", g[0], nro));
            }
            else
            {
                UltraMessageBox.Show("Numero de Guia Inválido!", "Error");
            }
        }

        private void GetGuiaRemision(string guia)
        {
            var serieCorrelativo = guia.Split('-');
            if (serieCorrelativo.Length != 2)
            {
                btnBuscarGuiaRemision.Clear();
                return;
            }
            string serie, correlativo;

            try
            {
                serie = int.Parse(serieCorrelativo[0]).ToString("0000");
                correlativo = int.Parse(serieCorrelativo[1]).ToString("00000000");
            }
            catch (Exception)
            {
                btnBuscarGuiaRemision.Clear();
                return;
            }

            btnBuscarGuiaRemision.Text = string.Format("{0}-{1}", serie, correlativo);
            var objOperationResult = new OperationResult();

            var ExisteGuia = _objMovimientoBL.ValidarGuiaRemision(ref  objOperationResult, serie, correlativo, int.Parse(cboAlmacen.Value.ToString()));
            if (ExisteGuia != null && objOperationResult.Success == 1)
            {
                var ds = _objMovimientoBL.ObtenerGuiaRemision(ref objOperationResult, serie, correlativo);

                if (objOperationResult.Success == 0)
                {
                    MessageBox.Show(
                        objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' +
                        objOperationResult.AdditionalInformation, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnBuscarGuiaRemision.Clear();
                    return;
                }

                grdData.DataSource = ds;
                grdData.Rows.ToList().ForEach(row =>
                {
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                });
                btnBuscarGuiaRemision.Enabled = false;
                btnBuscarGuiaRemision.Tag = "1";
            }
            else
            {
                var GuiaOtroAlmacen = _objMovimientoBL.ValidarGuiaRemision(ref  objOperationResult, serie, correlativo, -1);
                if (GuiaOtroAlmacen != null)
                {
                    if (objOperationResult.Success == 1)
                    {

                        UltraMessageBox.Show("La guia Remisión  no le pertenece a éste almacén , le pertenece a " + GuiaOtroAlmacen.AgenciaTransportes, "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else
                {
                    UltraMessageBox.Show("Guia Remisión no Existe!", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
        }

        private void btnBuscarGuiaRemision_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            var g = btnBuscarGuiaRemision.Text.Trim().Split('-');
            int nro;
            if (g.Length == 2 && int.TryParse(g[1], out nro))
            {
                GetGuiaRemision(string.Format("{0}-{1:0000}", g[0], nro));
            }
            else
            {
                UltraMessageBox.Show("Numero de Guia Inválido!", "Error");
            }
        }

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            frmMovimientosImportacionExcel frm = new frmMovimientosImportacionExcel();
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                if (!frm.ListaRetorno.Any()) return;

                grdData.DataSource = frm.ListaRetorno;
                grdData.Rows.ToList().ForEach(row =>
                {
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["t_FechaCaducidad"].Value = DateTime.Parse(Constants.FechaNula);

                });
            }
        }

        private void FijarRecetas(string pstrIdProductoDetalle, decimal multiplicador)
        {
            try
            {
                var objOperationResult = new OperationResult();
                var recetaProducto = new ProductoBL().ConvertirRecetaARecetaFinal(ref objOperationResult, pstrIdProductoDetalle);
                if (ultraGrid1.Rows.Any())
                    _listaRecetaFinalMovimiento = new BindingList<movimientodetallerecetafinalDto>(ultraGrid1.Rows.ToList().Select(fila => (movimientodetallerecetafinalDto)fila.ListObject).ToList());
                _listaRecetaFinalMovimiento = new BindingList<movimientodetallerecetafinalDto>(_listaRecetaFinalMovimiento.Concat(recetaProducto).ToList());
                ultraGrid1.DataSource = _listaRecetaFinalMovimiento;
                ultraGrid1.Rows.Where(f => f.Cells["i_RegistroTipo"].Value == null).ToList().ForEach(row =>
                {
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["_Cantidad"].Value = row.Cells["d_Cantidad"].Value;
                    row.Cells["_Multiplicador"].Value = string.Format("x{0}", multiplicador.ToString(CultureInfo.InvariantCulture));
                    if (row.Cells["i_IdAlmacen"].Value == null)
                        row.Cells["i_IdAlmacen"].Value = ((List<GridKeyValueDTO>)ucAlmacen.DataSource).FirstOrDefault().Id;
                });
                MuestraColumnasReceta();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EliminarRecetas(string pstrIdProductoDetalle)
        {
            try
            {
                var filas = ultraGrid1.Rows.Where(f => f.Cells["v_IdProdTerminado"].Value.ToString().Equals(pstrIdProductoDetalle));
                ultraGrid1.DisplayLayout.Override.SelectTypeRow = SelectType.Extended;
                ultraGrid1.Selected.Rows.AddRange(filas.ToArray());
                foreach (var row in ultraGrid1.Selected.Rows)
                {
                    if (row.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
                    {
                        var movimientodetallerfDto = new movimientodetallerecetafinalDto
                        {
                            v_IdRecetaFinal = row.Cells["v_IdRecetaFinal"].Value.ToString()
                        };
                        _TempDetalleReceta_EliminarDto.Add(movimientodetallerfDto);
                    }
                }

                ultraGrid1.DeleteSelectedRows(false);
                ultraGrid1.DisplayLayout.Override.SelectTypeRow = SelectType.None;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grdData_ClickCellButton(object sender, CellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "_Receta":


                    uPopUp.Show();
                    var strIdProductoTerminado = e.Cell.Row.Cells["v_IdProductoDetalle"].Value.ToString();
                    ultraGrid1.Text = string.Format("Receta de: {0}-{1}", e.Cell.Row.Cells["v_CodigoInterno"].Value, e.Cell.Row.Cells["v_NombreProducto"].Value);
                    var otrasFilas = ultraGrid1.Rows.Where(f => f.Cells["v_IdProdTerminado"].Value != null && !f.Cells["v_IdProdTerminado"].Value.ToString().Equals(strIdProductoTerminado)).ToList();
                    var visibleFilas = ultraGrid1.Rows.Where(f => f.Cells["v_IdProdTerminado"].Value != null && f.Cells["v_IdProdTerminado"].Value.ToString().Equals(strIdProductoTerminado)).ToList();
                    otrasFilas.ForEach(fila => fila.Hidden = true);
                    visibleFilas.ForEach(fila => fila.Hidden = false);

                    break;
            }
        }

        private void ultraButton2_Click(object sender, EventArgs e)
        {
            var f = new frmProductoReceta { TopMost = true };
            f.ShowDialog();

            if (f.Resultado != null && f.Resultado.Any())
            {
                var strIdProductoTerminado = grdData.ActiveRow.Cells["v_IdProductoDetalle"].Value.ToString();

                foreach (var fila in f.Resultado)
                {
                    UltraGridRow row = ultraGrid1.DisplayLayout.Bands[0].AddNew();
                    ultraGrid1.Rows.Move(row, ultraGrid1.Rows.Count() - 1);
                    this.ultraGrid1.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["v_IdProdTerminado"].Value = strIdProductoTerminado;
                    row.Cells["v_IdProdInsumo"].Value = fila.Cells["Id"].Value;
                    row.Cells["CodigoInsumo"].Value = fila.Cells["Value1"].Value;
                    row.Cells["NombreInsumo"].Value = fila.Cells["Value2"].Value;
                    row.Cells["Foto"].Value = fila.Cells["Imagen"].Value;
                    row.Cells["_Multiplicador"].Value = string.Format("x{0}", grdData.ActiveRow.Cells["d_Cantidad"].Value);
                    row.Cells["i_IdAlmacen"].Value = ((List<GridKeyValueDTO>)ucAlmacen.DataSource).FirstOrDefault().Id;
                    row.Cells["d_Cantidad"].Value = "0";
                    row.Cells["_Cantidad"].Value = "0";
                    UltraGridCell aCell = ultraGrid1.ActiveRow.Cells["_Cantidad"];
                    this.ultraGrid1.ActiveCell = aCell;
                    ultraGrid1.Focus();
                }
            }
        }

        private void ultraGrid1_CellChange(object sender, CellEventArgs e)
        {
            if (ultraGrid1.ActiveRow != null)
                ultraGrid1.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";

            try
            {
                if (e.Cell.Column.Key.Equals("_Cantidad"))
                {
                    e.Cell.Value = e.Cell.EditorResolved.Value;
                    var idProdTerminado = e.Cell.Row.Cells["v_IdProdTerminado"].Value.ToString();
                    var filaProdTerminado = grdData.Rows.FirstOrDefault(f => f.Cells["v_IdProductoDetalle"].Value.ToString().Equals(idProdTerminado));
                    if (filaProdTerminado != null)
                    {
                        var cantidadProdTerminado = filaProdTerminado.Cells["d_Cantidad"].Value != null ? decimal.Parse(filaProdTerminado.Cells["d_Cantidad"].Value.ToString()) : 0;
                        var cantidadInsumo = e.Cell.Value != null && !string.IsNullOrEmpty(e.Cell.Value.ToString()) ? decimal.Parse(e.Cell.Value.ToString()) : 0;
                        e.Cell.Row.Cells["d_Cantidad"].Value = cantidadInsumo * cantidadProdTerminado;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ultraButton1_Click(object sender, EventArgs e)
        {
            if (ultraGrid1.ActiveRow == null) return;

            if (ultraGrid1.ActiveRow.Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Insumo?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var movimientodetallerfDto = new movimientodetallerecetafinalDto
                    {
                        v_IdRecetaFinal = ultraGrid1.ActiveRow.Cells["v_IdRecetaFinal"].Value.ToString()
                    };
                    _TempDetalleReceta_EliminarDto.Add(movimientodetallerfDto);
                    ultraGrid1.ActiveRow.Delete(false);
                }
            }
            else
            {
                ultraGrid1.ActiveRow.Delete(false);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ultraGrid1_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdAlmacen"].EditorComponent = ucAlmacen;
            e.Layout.Bands[0].Columns["i_IdAlmacen"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }

        private void btnActualizarCostos_Click(object sender, EventArgs e)
        {
            var f = new frmActualizarCostosNotaIngreso();
            f.ShowDialog();

            if (f.DialogResult == DialogResult.OK)
            {
                try
                {
                    var listaCostos = f.Resultado;
                    foreach (var costoProducto in listaCostos)
                    {
                        var filaGrilla = grdData.Rows.FirstOrDefault(fila =>
                            fila.Cells["v_CodigoInterno"].Value.ToString().Trim().ToUpper()
                            .Equals(costoProducto.CodProducto.Trim().ToUpper()));

                        if (filaGrilla != null)
                        {
                            filaGrilla.Cells["d_Precio"].SetValue(costoProducto.Costo, false);
                            filaGrilla.Cells["i_RegistroEstado"].SetValue("Modificado", false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void txtProductoIngresar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (txtProductoIngresar.Text.Trim() == "") return;

            var codProd = txtProductoIngresar.Text.Trim();
            var row = grdData.Rows.FirstOrDefault(r => (string)r.GetCellValue("v_CodigoInterno") == codProd);
            if (row != null)
            {
                row.Cells["d_Cantidad"].Value = decimal.Parse(row.GetCellValue("d_Cantidad").ToString()) + 1;
                row.Cells["i_RegistroEstado"].Value = "Modificado";
            }
            else
            {
                var prod = _objMovimientoBL.DevolverArticuloPorCodInterno(codProd, int.Parse(cboAlmacen.Value.ToString()));
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
                row.Cells["i_IdUnidad"].Value = prod.i_IdUnidadMedida;
                row.Cells["i_IdUnidadMedidaProducto"].Value = prod.i_IdUnidadMedida;
                txtProductoIngresar.Focus();
            }
            grdData.Rows.Move(row, 0);
            if (row != grdData.ActiveRowScrollRegion.FirstRow) grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
            txtProductoIngresar.Clear();
        }



        private void cboMotivo_ValueChanged(object sender, EventArgs e)
        {
            chkDevolucion.Checked = cboMotivo.Value != null && cboMotivo.Value.ToString() == "4";
            chkDevolucion.Enabled = false;
        }

        private void btnCopiarFila_Click(object sender, EventArgs e)
        {
            try
            {
                var fila = grdData.ActiveRow;
                if (fila == null || fila.IsFilterRow || fila.IsGroupByRow) return;
                var elementoFila = (GridmovimientodetalleDto)fila.ListObject;
                var row = grdData.DisplayLayout.Bands[0].AddNew();
                grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                row.Cells["i_RegistroEstado"].Value = "Modificado";
                row.Cells["i_RegistroTipo"].Value = "Temporal";
                row.Cells["v_NombreProducto"].Value = elementoFila.v_NombreProducto.Trim();
                row.Cells["v_IdProductoDetalle"].Value = elementoFila.v_IdProductoDetalle;
                row.Cells["v_CodigoInterno"].Value = elementoFila.v_CodigoInterno;
                row.Cells["i_IdUnidad"].Value = elementoFila.i_IdUnidad;
                row.Cells["i_IdUnidadMedidaProducto"].Value = elementoFila.i_IdUnidad;
                row.Cells["Empaque"].Value = elementoFila.Empaque ?? 0;
                row.Cells["UMEmpaque"].Value = elementoFila.UMEmpaque;
                row.Cells["d_Precio"].Value = elementoFila.d_Precio ?? 0;
                row.Cells["i_EsProductoFinal"].Value = elementoFila.i_EsProductoFinal;
                row.Cells["d_Cantidad"].Value = elementoFila.d_Cantidad ?? 0M;
                row.Cells["d_CantidadEmpaque"].Value = elementoFila.d_CantidadEmpaque ?? 0M;
                row.Cells["i_IdTipoDocumento"].Value = elementoFila.i_IdTipoDocumento ?? -1;
                row.Cells["v_NumeroDocumento"].Value = elementoFila.v_NumeroDocumento;
                row.Cells["v_NroGuiaRemision"].Value = elementoFila.v_NroGuiaRemision;
                row.Cells["v_NroPedido"].Value = elementoFila.v_NroPedido;
                row.Cells["EsServicio"].Value = elementoFila.EsServicio ?? 0;
                row.Cells["d_Total"].Value = elementoFila.d_Total ?? 0;
                FijarRecetas(elementoFila.v_IdProductoDetalle, elementoFila.d_Cantidad ?? 0M);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            var fila = grdData.ActiveRow;
            if (fila == null || fila.IsFilterRow || fila.IsGroupByRow) return;
            btnCopiarFila.Enabled = true;
        }
    }
}
