using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinMaskedEdit;
using SAMBHS.Almacen.BL;
using SAMBHS.Common.BE;
using SAMBHS.Common.Resource;
using SAMBHS.CommonWIN.BL;
using SAMBHS.Compra.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAMBHS.Windows.WinClient.UI.Procesos
{
    public partial class frmGuiaRemisionCompra : Form
    {
        UltraCombo ucRubro = new UltraCombo();
        UltraCombo ucUnidadMedida = new UltraCombo();
        GuiaRemisionCompraBL _objGuiaRemisionCompraBL = new GuiaRemisionCompraBL();
        ComprasBL _objComprasBL = new ComprasBL();
        MovimientoBL _objMovimientoBL = new MovimientoBL();
        DatahierarchyBL _objDatahierarchyBL = new DatahierarchyBL();
        DocumentoBL _objDocumentoBL = new DocumentoBL();
        guiaremisioncompraDto _guiaremisioncompraDto = new guiaremisioncompraDto();
        guiaremisioncompradetalleDto _guiaremisioncompradetalleDto = new guiaremisioncompradetalleDto();
        movimientodetalleDto _movimientodetalleDto = new movimientodetalleDto();
        movimientoDto _movimientoDto = new movimientoDto();
        List<KeyValueDTO> _ListadoCompras = new List<KeyValueDTO>();
        List<GridKeyValueDTO> _ListadoComboDocumentos = new List<GridKeyValueDTO>();
        string strModo, _Mode, strIdcompra;
        public string _pstrIdMovimiento_Nuevo;
        int _MaxV, _ActV;


        #region Temporales DetalleCompra
        List<guiaremisioncompradetalleDto> _TempDetalle_AgregarDto = new List<guiaremisioncompradetalleDto>();
        List<guiaremisioncompradetalleDto> _TempDetalle_ModificarDto = new List<guiaremisioncompradetalleDto>();
        List<guiaremisioncompradetalleDto> _TempDetalle_EliminarDto = new List<guiaremisioncompradetalleDto>();
        #endregion

        #region Temporales NotaIngresoDetalle
        List<movimientodetalleDto> __TempDetalle_AgregarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> __TempDetalle_ModificarDto = new List<movimientodetalleDto>();
        List<movimientodetalleDto> __TempDetalle_EliminarDto = new List<movimientodetalleDto>();
        #endregion

        public frmGuiaRemisionCompra(string Modo, string Parametro)
        {
            strModo = Modo;
            strIdcompra = Parametro;
            InitializeComponent();
        }

        private void frmGuiaRemisionCompra_Load(object sender, EventArgs e)
        {
            UltraStatusbarManager.Inicializar(ultraStatusBar1);
            this.BackColor = new GlobalFormColors().FormColor;
            panel1.BackColor = new GlobalFormColors().BannerColor;
            OperationResult objOperationResult = new OperationResult();
            txtPeriodo.Text = Globals.ClientSession.i_Periodo.ToString();
            txtMes.Text = DateTime.Now.Month.ToString("00");
            _ListadoComboDocumentos = _objDocumentoBL.ObtenDocumentosParaComboGrid(ref objOperationResult, null, 1, 0);
            Utils.Windows.LoadUltraComboList(cboDocumento, "Value2", "Id", _ListadoComboDocumentos, DropDownListAction.Select);
            Utils.Windows.LoadDropDownList(cboMoneda, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForCombo(ref objOperationResult, 18, null), DropDownListAction.Select);
            CargarCombosDetalle();
            cboDocumento.Value = "9";
            cboMoneda.SelectedIndex = 2;
            dtpFechaRegistro.Value = DateTime.Parse(DateTime.Today.Day + "/" + DateTime.Today.Month + "/" + Globals.ClientSession.i_Periodo);
            ObtenerListadoCompras(txtPeriodo.Text, txtMes.Text);
            FormatoDecimalesGrilla((int)Globals.ClientSession.i_CantidadDecimales);

            Utils.Windows.SetLimitesPeriodo(dtpFechaRegistro);
            #region txtRucCLiente
            txtRucProveedor.LoadConfig("V");
            txtRucProveedor.ItemSelectedAfterDropClosed += delegate
            {
                txtRucProveedor_KeyDown(null, new KeyEventArgs(Keys.Enter));
            };
            #endregion
        }

        private bool  ValidarNulos()
        {


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

            var FilasSinFechaVencimientoLote = grdData.Rows.Where(p => p.Cells["i_SolicitarNroLoteIngreso"].Value != null && p.Cells["i_SolicitarNroLoteIngreso"].Value.ToString() == "1" && (DateTime.Parse(p.Cells["t_FechaCaducidad"].Value.ToString()) == DateTime.MinValue || DateTime.Parse(p.Cells["t_FechaCaducidad"].Value.ToString ()).ToShortDateString ()==Constants.FechaNula)).ToList();

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
     
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            if (uvDatos.Validate(true, false).IsValid)
            {
                string nroCuenta;
                if (!ValidarCuentas(grdData.Rows.ToList(), out nroCuenta))
                {
                    MessageBox.Show(string.Format("La cuenta {0} de la línea de un articulo ingresado no existe!", nroCuenta), "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (grdData.Rows.Count(p => p.Cells["v_IdProductoDetalle"].Value == null) != 0)
                {
                    UltraMessageBox.Show("Algunos productos no están ingresados correctamente.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                if (grdData.Rows.Count(p => p.Cells["i_EsServicio"].Value.ToString() == "1") != 0)
                {
                    UltraMessageBox.Show("No se pueden ingresar servicios a la Guía de Remisión", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtTipoCambio.Text == "" || decimal.Parse(txtTipoCambio.Text) <= 0)
                {
                    UltraMessageBox.Show("Ingrese un tipo de cambio válido.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (!grdData.Rows.Any())
                {
                    UltraMessageBox.Show("Por Favor ingrese almenos una fila al detalle del documento.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    foreach (UltraGridRow Fila in grdData.Rows)
                    {
                        if (Fila.Cells["v_NroCuenta"].Value == null)
                        {
                            UltraMessageBox.Show("Por favor ingrese correctamente todas las cuentas al detalle.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (CalcularTotales() == 0)
                    return;

                if (ValidarNulos())
                {

                    if (_Mode == "New")
                    {
                        while (_objGuiaRemisionCompraBL.ExisteNroRegistro(txtPeriodo.Text, txtMes.Text, txtCorrelativo.Text) == false)
                        {
                            txtCorrelativo.Text = (int.Parse(txtCorrelativo.Text) + 1).ToString("00000000");
                        }

                        #region Guarda Entidad Compra
                        _guiaremisioncompraDto.v_Glosa = txtGlosa.Text.Trim();
                        _guiaremisioncompraDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                        _guiaremisioncompraDto.v_SerieDocumento = txtSerieDoc.Text.Trim();
                        _guiaremisioncompraDto.v_CorrelativoDocumento = txtCorrelativoDoc.Text.Trim();
                        _guiaremisioncompraDto.v_Mes = txtMes.Text.Trim();
                        _guiaremisioncompraDto.v_Periodo = txtPeriodo.Text.Trim();
                        _guiaremisioncompraDto.v_Correlativo = txtCorrelativo.Text;
                        _guiaremisioncompraDto.v_Glosa = txtGlosa.Text;
                        _guiaremisioncompraDto.v_Mes = txtMes.Text;
                        _guiaremisioncompraDto.v_Periodo = txtPeriodo.Text;
                        _guiaremisioncompraDto.t_Fecha = dtpFechaRegistro.Value;
                        _guiaremisioncompraDto.i_IdMoneda = int.Parse(cboMoneda.SelectedValue.ToString());
                        _guiaremisioncompraDto.d_TipoCambio = decimal.Parse(txtTipoCambio.Text);
                        _guiaremisioncompraDto.d_TotalCantidad = decimal.Parse(txtTotal.Text);
                        _guiaremisioncompraDto.v_NroOrdenCompra = btnBuscarGuiaRemision.Text;
                        LlenarTemporalesCompra();
                        _objGuiaRemisionCompraBL.InsertarGuiaRemisionCompra(ref objOperationResult, _guiaremisioncompraDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto);
                        #endregion

                    }
                    else if (_Mode == "Edit")
                    {
                        #region Actualiza Entidad Compra
                        _guiaremisioncompraDto.v_Glosa = txtGlosa.Text.Trim();
                        _guiaremisioncompraDto.i_IdTipoDocumento = int.Parse(cboDocumento.Value.ToString());
                        _guiaremisioncompraDto.v_SerieDocumento = txtSerieDoc.Text.Trim();
                        _guiaremisioncompraDto.v_CorrelativoDocumento = txtCorrelativoDoc.Text.Trim();
                        _guiaremisioncompraDto.v_Mes = txtMes.Text.Trim();
                        _guiaremisioncompraDto.v_Periodo = txtPeriodo.Text.Trim();
                        _guiaremisioncompraDto.v_Correlativo = txtCorrelativo.Text;
                        _guiaremisioncompraDto.v_Glosa = txtGlosa.Text;
                        _guiaremisioncompraDto.v_Mes = txtMes.Text;
                        _guiaremisioncompraDto.v_Periodo = txtPeriodo.Text;
                        _guiaremisioncompraDto.t_Fecha = dtpFechaRegistro.Value;
                        _guiaremisioncompraDto.i_IdMoneda = int.Parse(cboMoneda.SelectedValue.ToString());
                        _guiaremisioncompraDto.d_TipoCambio = decimal.Parse(txtTipoCambio.Text);
                        _guiaremisioncompraDto.v_NroOrdenCompra = btnBuscarGuiaRemision.Text;
                        LlenarTemporalesCompra();
                        _objGuiaRemisionCompraBL.ActualizarGuiaRemisionCompra(ref objOperationResult, _guiaremisioncompraDto, Globals.ClientSession.GetAsList(), _TempDetalle_AgregarDto, _TempDetalle_ModificarDto, _TempDetalle_EliminarDto);
                        #endregion
                    }
                    if (objOperationResult.Success == 1)
                    {
                        strModo = "Guardado";
                        _Mode = "Edit";
                        strIdcompra = _guiaremisioncompraDto.v_IdGuiaCompra;
                        _pstrIdMovimiento_Nuevo = _guiaremisioncompraDto.v_IdGuiaCompra;
                        UltraMessageBox.Show("El registro se ha guardado correctamente", "Sistema");
                        ObtenerListadoCompras(txtPeriodo.Text, txtMes.Text);
                    }
                    else
                    {
                        UltraMessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage + '\n' + objOperationResult.AdditionalInformation, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    _TempDetalle_AgregarDto = new List<guiaremisioncompradetalleDto>();
                    _TempDetalle_ModificarDto = new List<guiaremisioncompradetalleDto>();
                    _TempDetalle_EliminarDto = new List<guiaremisioncompradetalleDto>();
                }
                else
                {
                   // UltraMessageBox.Show("Por favor llene los campos requeridos antes de guardar", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (grdData.Rows.Count() >= 1 && grdData.Rows[grdData.Rows.Count() - 1].Cells["v_CodigoInterno"].Value == null) return;

            if (grdData.ActiveRow != null)
            {
                if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value != null)
                {
                    UltraGridRow row = grdData.DisplayLayout.Bands[0].AddNew();
                    grdData.Rows.Move(row, grdData.Rows.Count() - 1);
                    this.grdData.ActiveRowScrollRegion.ScrollRowIntoView(row);
                    row.Cells["i_RegistroEstado"].Value = "Agregado";
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_IdUnidadMedida"].Value = "-1";
                    row.Cells["d_Cantidad"].Value = "0";
                    row.Cells["i_IdUnidadMedida"].Value = "-1";
                    row.Cells["v_NroCuenta"].Value = "-1";
                    row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                    row.Cells["v_NroCuenta"].Value = "-1";
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
                row.Cells["i_IdUnidadMedida"].Value = "-1";
                row.Cells["v_NroCuenta"].Value = "-1";
                row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
            }
            UltraGridCell aCell = this.grdData.ActiveRow.Cells["v_CodigoInterno"];
            this.grdData.ActiveCell = aCell;
            grdData.Focus();
            grdData.ActiveColScrollRegion.Scroll(ColScrollAction.Left);
        }

        private void txtSerieDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtSerieDoc, e);
        }

        private void txtSerieDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtSerieDoc, "{0:0000}");
        }

        private void txtCorrelativoDoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroEnteroUltraTextBox(txtCorrelativoDoc, e);
        }

        private void txtCorrelativoDoc_Validated(object sender, EventArgs e)
        {
            Utils.Windows.FijarFormatoUltraTextBox(txtCorrelativoDoc, "{0:00000000}");
            ComprobarRelacionDocumentoProveedor();
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
                        _guiaremisioncompraDto.v_IdProveedor = frm._IdProveedor;
                        txtCodigoProveedor.Text = frm._CodigoProveedor;
                        txtRazonSocial.Text = frm._RazonSocial;
                        txtRucProveedor.Text = frm._NroDocumento;
                        ComprobarRelacionDocumentoProveedor();
                    }
                }
                else if (txtRucProveedor.TextLength == 11 | txtRucProveedor.TextLength == 8)
                {
                    if (cboDocumento.Value.ToString() == "1" && txtRucProveedor.TextLength == 8)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _guiaremisioncompraDto.v_IdProveedor = null;
                        txtCodigoProveedor.Clear();
                        txtRazonSocial.Clear();
                        return;
                    }

                    if (txtRucProveedor.TextLength == 11 && Utils.Windows.ValidarRuc(txtRucProveedor.Text.Trim()) == false)
                    {
                        UltraMessageBox.Show("RUC Inválido", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _guiaremisioncompraDto.v_IdProveedor = null;
                        txtCodigoProveedor.Clear();
                        txtRazonSocial.Clear();
                        return;
                    }

                    OperationResult objOperationResult = new OperationResult();
                    string[] DatosProveedor = new string[3];
                    DatosProveedor = _objComprasBL.DevolverProveedorPorNroDocumento(ref objOperationResult, txtRucProveedor.Text.Trim());
                    if (DatosProveedor != null)
                    {
                        _guiaremisioncompraDto.v_IdProveedor = DatosProveedor[0];
                        txtCodigoProveedor.Text = DatosProveedor[1];
                        txtRazonSocial.Text = DatosProveedor[2];
                        ComprobarRelacionDocumentoProveedor();
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
                                _guiaremisioncompraDto.v_IdProveedor = DatosProveedor[0];
                                txtCodigoProveedor.Text = DatosProveedor[1];
                                txtRazonSocial.Text = DatosProveedor[2];
                                ComprobarRelacionDocumentoProveedor();
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

        private void grdData_DoubleClickCell(object sender, DoubleClickCellEventArgs e)
        {
            switch (e.Cell.Column.Key)
            {
                case "v_CodigoInterno":
                    if (cboDocumento.Value.ToString() == "7" | cboDocumento.Value.ToString() == "8") return;
                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "Temporal")
                    {
                        Mantenimientos.frmBuscarProducto frm = new Mantenimientos.frmBuscarProducto(int.Parse(grdData.ActiveRow.Cells["i_IdAlmacen"].Value.ToString()), null, null, grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Text == null ? string.Empty : grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Text.ToString());
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
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreProducto"].Value = frm._NombreProducto.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = frm._IdProducto.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value = frm._CodigoInternoProducto.ToString();
                            grdData.Rows[grdData.ActiveRow.Index].Cells["d_Empaque"].Value = frm._Empaque != null ? frm._Empaque.ToString() : string.Empty;
                            grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = frm._UnidadMedida != null ? frm._UnidadMedida.ToString() : null;
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = frm._UnidadMedidaEmpaque;
                            grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value = frm._NroCuentaCompra;
                            grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = frm._UnidadMedidaEmpaque != null ? frm._UnidadMedidaEmpaque.ToString() : null;
                            grdData.ActiveRow.Cells["i_EsServicio"].Value = frm._EsServicio.ToString();
                        }
                        UltraGridCell aCell = grdData.Rows[e.Cell.Row.Index].Cells["d_Cantidad"];
                        grdData.Rows[e.Cell.Row.Index].Activate();
                        grdData.ActiveCell = aCell;
                        grdData.PerformAction(UltraGridAction.EnterEditMode, false, false);
                        grdData.Focus();
                    }
                    break;
            }
        }

        private void grdData_AfterCellUpdate(object sender, CellEventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            grdData.ActiveRow.Cells["i_RegistroEstado"].Value = "Modificado";

            if (grdData.ActiveCell != null && grdData.ActiveCell.Column.Key == "d_Cantidad")
            {
                txtTotal.Text = grdData.Rows
                                .Where(p => p.Cells["d_Cantidad"].Value != null)
                                .Sum(x => decimal.Parse(x.Cells["d_Cantidad"].Value.ToString())).ToString("0.00");
            }
        }

        private void grdData_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].EditorComponent = ucUnidadMedida;
            e.Layout.Bands[0].Columns["i_IdUnidadMedida"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
            e.Layout.Bands[0].Columns["v_NroCuenta"].EditorComponent = ucRubro;
            e.Layout.Bands[0].Columns["v_NroCuenta"].Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DropDownList;
        }

        private void txtTipoCambio_Validated(object sender, EventArgs e)
        {
            if (txtTipoCambio.Text.Trim() == "")
            {
                txtTipoCambio.Text = "0";
            }
        }

        private void txtTipoCambio_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Windows.NumeroDecimalUltraTextBox(txtTipoCambio, e);
        }

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

            if (this.grdData.ActiveCell.Column.Key != "v_NroCuenta")
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

        private void grdData_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            e.Row.Cells["Index"].Value = (e.Row.Index + 1).ToString("00");
            if (e.Row.Band.Index == 0 && e.Row.Cells["t_FechaCaducidad"].Value != null && DateTime.Parse(e.Row.Cells["t_FechaCaducidad"].Value.ToString()).Date.ToShortDateString() == Constants.FechaNula)
            {
                e.Row.Cells["t_FechaCaducidad"].Appearance.BackColor = Color.White;
                e.Row.Cells["t_FechaCaducidad"].Appearance.ForeColor = Color.White;
            }
            
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (grdData.ActiveRow == null) return;

            if (grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroTipo"].Value.ToString() == "NoTemporal")
            {
                if (UltraMessageBox.Show("¿Seguro de Eliminar este Registro?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _guiaremisioncompradetalleDto = new guiaremisioncompradetalleDto();
                    _guiaremisioncompradetalleDto.v_IdGuiaCompraDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdGuiaCompraDetalle"].Value.ToString();
                    _TempDetalle_EliminarDto.Add(_guiaremisioncompradetalleDto);

                    if (grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdMovimientoDetalle"].Value != null)
                    {
                        _movimientodetalleDto = new movimientodetalleDto();
                        _movimientodetalleDto.v_IdMovimientoDetalle = grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdMovimientoDetalle"].Value.ToString();
                        __TempDetalle_EliminarDto.Add(_movimientodetalleDto);
                    }
                    grdData.Rows[grdData.ActiveRow.Index].Delete(false);
                    btnEliminar.Enabled = false;
                }
            }
            else
            {
                grdData.Rows[grdData.ActiveRow.Index].Delete(false);
                btnEliminar.Enabled = false;
            }

            txtTotal.Text = grdData.Rows.Where(p => p.Cells["d_Cantidad"].Value != null).Sum(o => decimal.Parse(o.Cells["d_Cantidad"].Value.ToString())).ToString("0.00");
        }

        private void grdData_AfterRowActivate(object sender, EventArgs e)
        {
            if (grdData.ActiveRow != null)
            {
                btnEliminar.Enabled = true;
            }
            else
            {
                btnEliminar.Enabled = false;
            }
        }

        private void dtpFechaRegistro_ValueChanged(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            txtPeriodo.Text = dtpFechaRegistro.Value.Year.ToString();
            txtMes.Text = dtpFechaRegistro.Value.Month.ToString("00");
            txtCorrelativo.Text = Utils.Windows.RetornaCorrelativoPorFecha(ref objOperationResult, ListaProcesos.Compra, _guiaremisioncompraDto.t_Fecha, dtpFechaRegistro.Value, _guiaremisioncompraDto.v_Correlativo, 0);
        }

        #region Clases / Validaciones

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
            _ListadoCompras = _objGuiaRemisionCompraBL.ObtenerListadoGuiaRemisionCompra(ref objOperationResult, pstrPeriodo, pstrMes);
            switch (strModo)
            {
                case "Edicion":
                    CargarCabecera(strIdcompra);
                    txtSerieDoc.Enabled = false;
                    txtCorrelativoDoc.Enabled = false;
                    txtRucProveedor.Enabled = false;
                    btnBuscarGuiaRemision.Enabled = false;
                    if (_objGuiaRemisionCompraBL.ComprobarSiFueLlamadaEnCompras(ref objOperationResult, txtSerieDoc.Text, txtCorrelativoDoc.Text, _guiaremisioncompraDto.v_IdProveedor) == true)
                    {
                        btnGuardar.Visible = false;
                        btnAgregar.Visible = false;
                        btnEliminar.Visible = false;
                    }
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
                        _guiaremisioncompraDto = new guiaremisioncompraDto();
                        _movimientoDto = new movimientoDto();
                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        LimpiarCabecera();
                        CargarDetalle("");
                        _MaxV = 1;
                        _ActV = 1;
                        _guiaremisioncompraDto = new guiaremisioncompraDto();
                        _movimientoDto = new movimientoDto();

                    }
                    txtTipoCambio.Text = _objMovimientoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value.Date);
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
                    }
                    else
                    {
                        txtCorrelativo.Text = "00000001";
                        _Mode = "New";
                        LimpiarCabecera();
                        CargarDetalle("");
                        _MaxV = 1;
                        _ActV = 1;
                        _guiaremisioncompraDto = new guiaremisioncompraDto();
                        _movimientoDto = new movimientoDto();
                        txtMes.Enabled = true;
                    }
                    HabilitarLotesSerie(false);
                    break;
            }
        }

        private void CargarCabecera(string idmovimiento)
        {
            OperationResult objOperationResult = new OperationResult();
            _guiaremisioncompraDto = new guiaremisioncompraDto();
            _guiaremisioncompraDto = _objGuiaRemisionCompraBL.ObtenerGuiaRemisionCompraCabecera(ref objOperationResult, idmovimiento);

            if (_guiaremisioncompraDto != null)
            {
                txtPeriodo.Text = _guiaremisioncompraDto.v_Periodo;
                txtMes.Text = int.Parse(_guiaremisioncompraDto.v_Mes).ToString("00");
                txtGlosa.Text = _guiaremisioncompraDto.v_Glosa;
                dtpFechaRegistro.Value = _guiaremisioncompraDto.t_Fecha.Value;
                txtCorrelativo.Text = _guiaremisioncompraDto.v_Correlativo;
                txtSerieDoc.Text = _guiaremisioncompraDto.v_SerieDocumento;
                txtCorrelativoDoc.Text = _guiaremisioncompraDto.v_CorrelativoDocumento;
                cboDocumento.Value = _guiaremisioncompraDto.i_IdTipoDocumento.Value.ToString();
                txtTipoCambio.Text = _guiaremisioncompraDto.d_TipoCambio.Value.ToString();
                cboMoneda.SelectedValue = _guiaremisioncompraDto.i_IdMoneda.Value.ToString();

                string[] Proveedor = new string[3];
                Proveedor = _objGuiaRemisionCompraBL.DevolverProveedorPorID(ref objOperationResult, _guiaremisioncompraDto.v_IdProveedor);

                if (Proveedor != null)
                {
                    txtRucProveedor.Text = Proveedor[0];
                    txtCodigoProveedor.Text = Proveedor[1];
                    txtRazonSocial.Text = Proveedor[2];
                }

                btnBuscarGuiaRemision.Text = _guiaremisioncompraDto.v_NroOrdenCompra;
                _Mode = "Edit";

                CargarDetalle(_guiaremisioncompraDto.v_IdGuiaCompra);
                UltraStatusbarManager.Mensaje(ultraStatusBar1,
                    GuiaRemisionCompraBL.ObtenerNiRelacionada(_guiaremisioncompraDto.v_IdGuiaCompra), timer1);
            }
            else
            {
                UltraMessageBox.Show("Hubo un error al cargar la compra", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDetalle(string pstringIdCompra)
        {
            OperationResult objOperationResult = new OperationResult();
            grdData.DataSource = _objGuiaRemisionCompraBL.ObtenerGuiaRemisionCompraDetalles(ref objOperationResult, pstringIdCompra);
            //if (grdData.Rows.Any())
            //{
            //    //DevolverNombreRelaciones();
            //}
            for (int i = 0; i < grdData.Rows.Count(); i++)
            {
                grdData.Rows[i].Cells["i_RegistroTipo"].Value = "NoTemporal";
            }
            //CalcularTotales();
        }

        private void LimpiarCabecera()
        {

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
                                _guiaremisioncompradetalleDto = new guiaremisioncompradetalleDto();
                                _guiaremisioncompradetalleDto.v_IdGuiaCompra = _guiaremisioncompraDto.v_IdGuiaCompra;
                                _guiaremisioncompradetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _guiaremisioncompradetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _guiaremisioncompradetalleDto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _guiaremisioncompradetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _guiaremisioncompradetalleDto.i_InsertaIdUsuario = Fila.Cells["i_InsertaIdUsuario"].Value == null ? 0 : int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _guiaremisioncompradetalleDto.v_NroCuenta = Fila.Cells["v_NroCuenta"].Value == null ? null : Fila.Cells["v_NroCuenta"].Value.ToString();
                                _guiaremisioncompradetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _guiaremisioncompradetalleDto.d_PrecioUnitario = Fila.Cells["d_PrecioUnitario"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioUnitario"].Value.ToString());

                                _guiaremisioncompradetalleDto.v_NroSerie = Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim();
                                _guiaremisioncompradetalleDto.v_NroLote = Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim();
                                _guiaremisioncompradetalleDto.t_FechaCaducidad = Fila.Cells["t_FechaCaducidad"].Value == null ? DateTime.MinValue : DateTime.Parse(Fila.Cells["t_FechaCaducidad"].Value.ToString());
                                
                                _TempDetalle_AgregarDto.Add(_guiaremisioncompradetalleDto);
                            }
                            break;

                        case "NoTemporal":
                            if (Fila.Cells["i_RegistroEstado"].Value != null && Fila.Cells["i_RegistroEstado"].Value.ToString() == "Modificado")
                            {
                                _guiaremisioncompradetalleDto = new guiaremisioncompradetalleDto();
                                _guiaremisioncompradetalleDto.v_IdGuiaCompraDetalle = Fila.Cells["v_IdGuiaCompraDetalle"].Value == null ? null : Fila.Cells["v_IdGuiaCompraDetalle"].Value.ToString();
                                _guiaremisioncompradetalleDto.v_IdMovimientoDetalle = Fila.Cells["v_IdMovimientoDetalle"].Value == null ? null : Fila.Cells["v_IdMovimientoDetalle"].Value.ToString();
                                _guiaremisioncompradetalleDto.v_IdGuiaCompra = Fila.Cells["v_IdGuiaCompra"].Value == null ? null : Fila.Cells["v_IdGuiaCompra"].Value.ToString();
                                _guiaremisioncompradetalleDto.v_IdProductoDetalle = Fila.Cells["v_IdProductoDetalle"].Value == null ? null : Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                                _guiaremisioncompradetalleDto.i_IdUnidadMedida = Fila.Cells["i_IdUnidadMedida"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
                                _guiaremisioncompradetalleDto.i_IdAlmacen = Fila.Cells["i_IdAlmacen"].Value == null ? 0 : int.Parse(Fila.Cells["i_IdAlmacen"].Value.ToString());
                                _guiaremisioncompradetalleDto.d_Cantidad = Fila.Cells["d_Cantidad"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                                _guiaremisioncompradetalleDto.i_InsertaIdUsuario = Fila.Cells["i_InsertaIdUsuario"].Value == null ? 0 : int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _guiaremisioncompradetalleDto.v_NroCuenta = Fila.Cells["v_NroCuenta"].Value == null ? null : Fila.Cells["v_NroCuenta"].Value.ToString();
                                _guiaremisioncompradetalleDto.i_Eliminado = int.Parse(Fila.Cells["i_Eliminado"].Value.ToString());
                                _guiaremisioncompradetalleDto.i_InsertaIdUsuario = int.Parse(Fila.Cells["i_InsertaIdUsuario"].Value.ToString());
                                _guiaremisioncompradetalleDto.t_InsertaFecha = Convert.ToDateTime(Fila.Cells["t_InsertaFecha"].Value);
                                _guiaremisioncompradetalleDto.d_CantidadEmpaque = Fila.Cells["d_CantidadEmpaque"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_CantidadEmpaque"].Value.ToString());
                                _guiaremisioncompradetalleDto.d_PrecioUnitario = Fila.Cells["d_PrecioUnitario"].Value == null ? 0 : decimal.Parse(Fila.Cells["d_PrecioUnitario"].Value.ToString());

                                _guiaremisioncompradetalleDto.v_NroSerie = Fila.Cells["v_NroSerie"].Value == null || Fila.Cells["v_NroSerie"].Value == "" ? null : Fila.Cells["v_NroSerie"].Value.ToString().Trim();
                                _guiaremisioncompradetalleDto.v_NroLote = Fila.Cells["v_NroLote"].Value == null || Fila.Cells["v_NroLote"].Value == "" ? null : Fila.Cells["v_NroLote"].Value.ToString().Trim();
                                _guiaremisioncompradetalleDto.t_FechaCaducidad = Fila.Cells["t_FechaCaducidad"].Value == null ? DateTime.MinValue : DateTime.Parse(Fila.Cells["t_FechaCaducidad"].Value.ToString());

                                
                                _TempDetalle_ModificarDto.Add(_guiaremisioncompradetalleDto);
                            }
                            break;
                    }
                }
            }

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

            ucRubro.DisplayLayout.NewColumnLoadStyle = NewColumnLoadStyle.Hide;
            Utils.Windows.LoadUltraComboList(ucUnidadMedida, "Value1", "Id", _objDatahierarchyBL.GetDataHierarchiesForComboGrid(ref objOperationResult, 17, null), DropDownListAction.Select);
            Utils.Windows.LoadUltraComboList(ucRubro, "Value1", "Id", _objComprasBL.ObtenRubrosParaComboGridCompra(ref objOperationResult, null), DropDownListAction.Select);
        }

        public int CalcularTotales()
        {
            if (grdData.Rows.Where(x => x.Cells["d_Cantidad"].Value == null || decimal.Parse(x.Cells["d_Cantidad"].Value.ToString()) <= 0).Count() == 0)
            {
                txtTotal.Text = grdData.Rows.Where(p => p.Cells["d_Cantidad"].Value != null).Sum(o => decimal.Parse(o.Cells["d_Cantidad"].Value.ToString())).ToString("0.00");

                foreach (UltraGridRow Fila in grdData.Rows)
                {
                    if (Fila.Cells["i_IdUnidadMedida"].Value != null)
                    {
                        if (Fila.Cells["d_Empaque"].Value != null && Fila.Cells["v_IdProductoDetalle"].Value != null && Fila.Cells["v_IdProductoDetalle"].Value.ToString() != "N002-PE000000000" && Fila.Cells["i_IdUnidadMedida"].Value.ToString() != "-1" && decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString()) != 0)
                        {
                            decimal TotalEmpaque = 0;
                            decimal Empaque = decimal.Parse(Fila.Cells["d_Empaque"].Value.ToString());
                            string Producto = Fila.Cells["v_IdProductoDetalle"].Value.ToString();
                            decimal Cantidad = decimal.Parse(Fila.Cells["d_Cantidad"].Value.ToString());
                            int UM = int.Parse(Fila.Cells["i_IdUnidadMedida"].Value.ToString());
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
                    }

                }
                return 1;
            }
            else
            {
                UltraMessageBox.Show("Todas las cantidades no estas calculadas.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return 0;
            }

        }

        private void FormatoDecimalesGrilla(int DecimalesCantidad)
        {
            string FormatoCantidad;
            UltraGridColumn _Cantidad = this.grdData.DisplayLayout.Bands[0].Columns["d_Cantidad"];
            _Cantidad.MaskDataMode = MaskMode.IncludeLiterals;
            _Cantidad.MaskDisplayMode = MaskMode.IncludeLiterals;


            if (DecimalesCantidad > 0)
            {
                string sharp = "n";
                FormatoCantidad = "nnnnn.";
                for (int i = 0; i < DecimalesCantidad; i++)
                {
                    FormatoCantidad = FormatoCantidad + sharp;
                }
            }
            else
            {
                FormatoCantidad = "nnnnn";
            }
            _Cantidad.MaskInput = FormatoCantidad;
        }

        public void RecibirItems(List<UltraGridRow> Filas)
        {
            bool Repetido = false;
            bool j = false;
            for (int i = 0; i < Filas.Count; i++)
            {
                if (grdData.Rows.Where(p => p.Cells["v_IdProductoDetalle"].Value != null && p.Cells["v_IdProductoDetalle"].Value.ToString() == Filas[i].Cells["v_IdProductoDetalle"].Value.ToString()).Count() != 0)
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
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value = Filas[i].Cells["NroCuentaCompra"].Value != null ? Filas[i].Cells["NroCuentaCompra"].Value.ToString() : "-1";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_SolicitarNroSerie"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroSerie"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_SolicitarNroLote"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroLote"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroSerie"].Value = Filas[i].Cells["v_NroSerie"].Value == null || Filas[i].Cells["v_NroSerie"].Value == "" ? null : Filas[i].Cells["v_NroSerie"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroLote"].Value = Filas[i].Cells["v_NroLote"].Value == null || Filas[i].Cells["v_NroLote"].Value == "" ? null : Filas[i].Cells["v_NroLote"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["t_FechaCaducidad"].Value = Filas[i].Cells["t_FechaCaducidad"].Value == null ? null : Filas[i].Cells["t_FechaCaducidad"].Value.ToString();
                        
                        
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
                        row.Cells["i_IdUnidadMedida"].Value = "-1";
                        row.Cells["v_NroCuenta"].Value = "-1";
                        row.Cells["i_IdAlmacen"].Value = Globals.ClientSession.i_IdAlmacenPredeterminado.ToString();
                        row.Activate();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NombreProducto"].Value = Filas[i].Cells["v_Descripcion"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_IdProductoDetalle"].Value = Filas[i].Cells["v_IdProductoDetalle"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_CodigoInterno"].Value = Filas[i].Cells["v_CodInterno"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_RegistroEstado"].Value = "Modificado";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedidaProducto"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_IdUnidadMedida"].Value = Filas[i].Cells["i_IdUnidadMedida"].Value == null ? null : Filas[i].Cells["i_IdUnidadMedida"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Cantidad"].Value = Filas[i].Cells["_Cantidad"].Value == null ? 1 : decimal.Parse(Filas[i].Cells["_Cantidad"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["d_Empaque"].Value = Filas[i].Cells["d_Empaque"].Value == null ? 0 : decimal.Parse(Filas[i].Cells["d_Empaque"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["UMEmpaque"].Value = Filas[i].Cells["EmpaqueUnidadMedida"].Value == null ? null : Filas[i].Cells["EmpaqueUnidadMedida"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroCuenta"].Value = Filas[i].Cells["NroCuentaCompra"].Value != null ? Filas[i].Cells["NroCuentaCompra"].Value.ToString() : "-1";
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_EsServicio"].Value = Filas[i].Cells["i_EsServicio"].Value == null ? 0 : int.Parse(Filas[i].Cells["i_EsServicio"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_SolicitarNroSerie"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroSerie"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["i_SolicitarNroLote"].Value = int.Parse(Filas[i].Cells["i_SolicitarNroLote"].Value.ToString());
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroSerie"].Value = Filas[i].Cells["v_NroSerie"].Value == null || Filas[i].Cells["v_NroSerie"].Value == "" ? null : Filas[i].Cells["v_NroSerie"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["v_NroLote"].Value = Filas[i].Cells["v_NroLote"].Value == null || Filas[i].Cells["v_NroLote"].Value == "" ? null : Filas[i].Cells["v_NroLote"].Value.ToString();
                        grdData.Rows[grdData.ActiveRow.Index].Cells["t_FechaCaducidad"].Value = Filas[i].Cells["t_FechaCaducidad"].Value == null ? null : Filas[i].Cells["t_FechaCaducidad"].Value.ToString();
                    }
                }
            }
            //CalcularTotales();
        }

        private void ComprobarRelacionDocumentoProveedor()
        {
            OperationResult objOperationResult = new OperationResult();
            if (_objGuiaRemisionCompraBL.ComprobarRelacionDocumentoProveedor(ref objOperationResult, _guiaremisioncompraDto.v_IdProveedor, int.Parse(cboDocumento.Value.ToString()), txtSerieDoc.Text, txtCorrelativoDoc.Text, _guiaremisioncompraDto.v_IdGuiaCompra) == true)
            {
                UltraMessageBox.Show("El documento ya ha sido registrado anteriormente con este Proveedor", "Error de Validacion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGuardar.Enabled = false;
            }
            else
            {
                btnGuardar.Enabled = true;
            }
        }
        #endregion

        private void dtpFechaRegistro_Validated(object sender, EventArgs e)
        {
            OperationResult objOperationResult = new OperationResult();
            txtTipoCambio.Text = _objMovimientoBL.DevolverTipoCambioPorFecha(ref objOperationResult, dtpFechaRegistro.Value);
        }

        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            Mantenimientos.frmBuscarProveedor frm = new Mantenimientos.frmBuscarProveedor(txtRucProveedor.Text.Trim(), "RUC");
            frm.ShowDialog();
            if (frm._IdProveedor != null)
            {
                txtRazonSocial.Text = frm._RazonSocial;
                txtCodigoProveedor.Text = frm._CodigoProveedor;
                _guiaremisioncompraDto.v_IdProveedor = frm._IdProveedor;
                txtRucProveedor.Text = frm._NroDocumento;
            }
        }

        private void btnBuscarGuiaRemision_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
        {
            if (!string.IsNullOrEmpty(btnBuscarGuiaRemision.Text.Trim()))
            {
                if (ObtenerOrdenDeCompra(btnBuscarGuiaRemision.Text.Trim()))
                    return;
                btnBuscarGuiaRemision.Clear();
            }
            var frm = new frmBuscarOrdenCompra();
            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                ObtenerOrdenDeCompra(frm.Result);
        }
        private bool ObtenerOrdenDeCompra(string ordenCompra)
        {
            var split = ordenCompra.Split('-').ToList();
            var longitudValida = split.Count == 2;
            int serie, correlativo;
            if (longitudValida && int.TryParse(split[0], out serie) && int.TryParse(split[1], out correlativo))
            {
                btnBuscarGuiaRemision.Text = string.Format("{0}-{1}", serie.ToString("0000"), correlativo.ToString("00000000"));

                var objOperationResult = new OperationResult();
                var proveedor = new clienteDto();
                var ds = _objGuiaRemisionCompraBL.ObtenerOrdenDeCompra(ref objOperationResult, serie.ToString("0000"), correlativo.ToString("00000000"), out proveedor);

                txtRazonSocial.Text =
                    (proveedor.v_ApePaterno + " " + proveedor.v_ApeMaterno + " " + proveedor.v_PrimerNombre + " " +
                     proveedor.v_RazonSocial).Trim();

                txtRucProveedor.Text = proveedor.v_NroDocIdentificacion;
                txtCodigoProveedor.Text = proveedor.v_CodCliente;
                _guiaremisioncompraDto.v_IdProveedor = proveedor.v_IdCliente;

                if (objOperationResult.Success == 0)
                {
                    MessageBox.Show(objOperationResult.ErrorMessage + '\n' + objOperationResult.ExceptionMessage, @"Error en la consulta.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                txtGlosa.Text = "Obtenido desde la Orden de Compra: " + btnBuscarGuiaRemision.Text;
                grdData.DataSource = ds;
                CalcularTotales();
                grdData.Rows.ToList().ForEach(row =>
                {
                    row.Cells["i_RegistroTipo"].Value = "Temporal";
                    row.Cells["i_RegistroEstado"].Value = "Modificado";
                });
                return true;
            }
            return false;
        }
        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
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
    }
}
